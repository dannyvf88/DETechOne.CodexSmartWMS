using System.Globalization;
using System.Text.Json;
using DETechOne.SmartWMS.Application.SAP;
using DETechOne.SmartWMS.Contracts.Dtos.SAP;
using DETechOne.SmartWMS.Domain.Common;

namespace DETechOne.SmartWMS.SAP.Sales;

/// <summary>
/// Real SAP Business One Sales Order reader backed by Service Layer.
/// This class intentionally does not reference SAPbobsCOM so the solution can compile
/// on developer machines without the DI API installed.
/// </summary>
public sealed class SapSalesOrderReader : ISapSalesOrderReader
{
    private readonly IServiceLayerClient _serviceLayerClient;

    public SapSalesOrderReader(IServiceLayerClient serviceLayerClient)
    {
        _serviceLayerClient = serviceLayerClient;
    }

    public async Task<Result<SapSalesOrderDto>> GetByDocEntryAsync(
        int docEntry,
        CancellationToken cancellationToken = default)
    {
        if (docEntry <= 0)
        {
            return Result<SapSalesOrderDto>.Fail("SAP_DOC_ENTRY_INVALID", "DocEntry must be greater than zero.");
        }

        string relativeUrl = BuildSalesOrderUrl(docEntry);

        Result<string> response = await _serviceLayerClient
            .GetAsync(relativeUrl, cancellationToken)
            .ConfigureAwait(false);

        if (!response.Success || string.IsNullOrWhiteSpace(response.Value))
        {
            return Result<SapSalesOrderDto>.Fail(
                response.ErrorCode ?? "SAP_SALES_ORDER_READ_ERROR",
                response.Message ?? "Sales Order could not be read from SAP Service Layer.");
        }

        try
        {
            SapSalesOrderDto salesOrder = MapSalesOrder(response.Value);
            return Result<SapSalesOrderDto>.Ok(salesOrder, $"Sales Order {salesOrder.DocNum} loaded from SAP.");
        }
        catch (JsonException ex)
        {
            return Result<SapSalesOrderDto>.Fail(
                "SAP_SALES_ORDER_JSON_ERROR",
                $"SAP Sales Order response could not be parsed. {ex.Message}");
        }
        catch (InvalidOperationException ex)
        {
            return Result<SapSalesOrderDto>.Fail(
                "SAP_SALES_ORDER_MAPPING_ERROR",
                ex.Message);
        }
    }

    private static string BuildSalesOrderUrl(int docEntry)
    {
        // Keep the query broad on purpose. SAP Business One Service Layer property names
        // can vary slightly between patch levels/localizations, and unknown properties in
        // $select may cause a 400 response. The mapper below reads only the fields that
        // SmartWMS needs and ignores everything else.
        return $"Orders({docEntry})?$expand=DocumentLines";
    }

    private static SapSalesOrderDto MapSalesOrder(string json)
    {
        using JsonDocument document = JsonDocument.Parse(json);
        JsonElement root = document.RootElement;

        int docEntry = GetRequiredInt32(root, "DocEntry");
        int docNum = GetRequiredInt32(root, "DocNum");

        SapSalesOrderLineDto[] lines = root.TryGetProperty("DocumentLines", out JsonElement linesElement)
            && linesElement.ValueKind == JsonValueKind.Array
                ? linesElement
                    .EnumerateArray()
                    .Select(MapSalesOrderLine)
                    .Where(line => !string.IsNullOrWhiteSpace(line.ItemCode))
                    .ToArray()
                : Array.Empty<SapSalesOrderLineDto>();

        return new SapSalesOrderDto
        {
            DocEntry = docEntry,
            DocNum = docNum,
            CardCode = GetString(root, "CardCode"),
            CardName = GetString(root, "CardName"),
            DocStatus = NormalizeDocumentStatus(GetFirstString(root, "DocStatus", "DocumentStatus")),
            DocDate = GetDateTime(root, "DocDate"),
            Lines = lines
        };
    }

    private static SapSalesOrderLineDto MapSalesOrderLine(JsonElement line)
    {
        decimal quantity = GetDecimal(line, "Quantity");
        decimal openQuantity = GetFirstDecimal(line, "OpenQuantity", "RemainingOpenQuantity");
        if (openQuantity <= 0 && IsOpenLine(line))
        {
            openQuantity = quantity;
        }

        return new SapSalesOrderLineDto
        {
            LineNum = GetInt32(line, "LineNum"),
            ItemCode = GetString(line, "ItemCode"),
            ItemDescription = GetFirstString(line, "ItemDescription", "Dscription"),
            WarehouseCode = GetFirstString(line, "WarehouseCode", "WhsCode"),
            Quantity = quantity,
            OpenQuantity = openQuantity,
            UomCode = GetFirstString(line, "UoMCode", "UomCode", "MeasureUnit")
        };
    }

    private static bool IsOpenLine(JsonElement line)
    {
        string lineStatus = GetString(line, "LineStatus");
        return string.IsNullOrWhiteSpace(lineStatus)
            || lineStatus.Equals("bost_Open", StringComparison.OrdinalIgnoreCase)
            || lineStatus.Equals("O", StringComparison.OrdinalIgnoreCase)
            || lineStatus.Equals("Open", StringComparison.OrdinalIgnoreCase);
    }

    private static string NormalizeDocumentStatus(string status)
    {
        return status switch
        {
            "bost_Open" => "O",
            "bost_Close" => "C",
            _ when status.Equals("Open", StringComparison.OrdinalIgnoreCase) => "O",
            _ when status.Equals("Closed", StringComparison.OrdinalIgnoreCase) => "C",
            _ => status
        };
    }

    private static int GetRequiredInt32(JsonElement element, string propertyName)
    {
        int value = GetInt32(element, propertyName);
        if (value <= 0)
        {
            throw new InvalidOperationException($"{propertyName} was not returned by SAP Service Layer.");
        }

        return value;
    }

    private static int GetInt32(JsonElement element, string propertyName)
    {
        if (!element.TryGetProperty(propertyName, out JsonElement property))
        {
            return 0;
        }

        return property.ValueKind switch
        {
            JsonValueKind.Number when property.TryGetInt32(out int value) => value,
            JsonValueKind.String when int.TryParse(property.GetString(), NumberStyles.Integer, CultureInfo.InvariantCulture, out int value) => value,
            _ => 0
        };
    }

    private static decimal GetFirstDecimal(JsonElement element, params string[] propertyNames)
    {
        foreach (string propertyName in propertyNames)
        {
            decimal value = GetDecimal(element, propertyName);
            if (value != 0)
            {
                return value;
            }
        }

        return 0;
    }

    private static decimal GetDecimal(JsonElement element, string propertyName)
    {
        if (!element.TryGetProperty(propertyName, out JsonElement property))
        {
            return 0;
        }

        return property.ValueKind switch
        {
            JsonValueKind.Number when property.TryGetDecimal(out decimal value) => value,
            JsonValueKind.String when decimal.TryParse(property.GetString(), NumberStyles.Number, CultureInfo.InvariantCulture, out decimal value) => value,
            _ => 0
        };
    }

    private static string GetFirstString(JsonElement element, params string[] propertyNames)
    {
        foreach (string propertyName in propertyNames)
        {
            string value = GetString(element, propertyName);
            if (!string.IsNullOrWhiteSpace(value))
            {
                return value;
            }
        }

        return string.Empty;
    }

    private static string GetString(JsonElement element, string propertyName)
    {
        if (!element.TryGetProperty(propertyName, out JsonElement property))
        {
            return string.Empty;
        }

        return property.ValueKind switch
        {
            JsonValueKind.String => property.GetString() ?? string.Empty,
            JsonValueKind.Number => property.GetRawText(),
            JsonValueKind.True => "true",
            JsonValueKind.False => "false",
            _ => string.Empty
        };
    }

    private static DateTime? GetDateTime(JsonElement element, string propertyName)
    {
        string value = GetString(element, propertyName);
        if (string.IsNullOrWhiteSpace(value))
        {
            return null;
        }

        return DateTime.TryParse(value, CultureInfo.InvariantCulture, DateTimeStyles.AssumeLocal, out DateTime date)
            ? date
            : null;
    }
}
