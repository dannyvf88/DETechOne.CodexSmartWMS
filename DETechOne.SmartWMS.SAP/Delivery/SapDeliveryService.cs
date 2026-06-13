using System.Globalization;
using System.Text.Json;
using DETechOne.SmartWMS.Application.SAP;
using DETechOne.SmartWMS.Application.Shipping;
using DETechOne.SmartWMS.Contracts.Dtos.Shipping;
using DETechOne.SmartWMS.Domain.Common;
using DETechOne.SmartWMS.Domain.Entities.Shipping;
using DETechOne.SmartWMS.Domain.Enums;
using DETechOne.SmartWMS.SAP.Configuration;
using Microsoft.Extensions.Options;

namespace DETechOne.SmartWMS.SAP.Delivery;

/// <summary>
/// Creates SAP Business One Delivery Notes through Service Layer.
/// The implementation intentionally depends only on the Service Layer abstraction so the solution
/// keeps compiling on machines without SAP DI API installed.
/// </summary>
public sealed class SapDeliveryService : ISapDeliveryService
{
    private readonly IServiceLayerClient _serviceLayerClient;
    private readonly SapOptions _options;

    public SapDeliveryService(IServiceLayerClient serviceLayerClient, IOptions<SapOptions> options)
    {
        _serviceLayerClient = serviceLayerClient;
        _options = options.Value;
    }

    public async Task<Result<SapDeliveryResultDto>> CreateDeliveryAsync(
        ShippingDocument shippingDocument,
        string userName,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(shippingDocument);

        string validationError = Validate(shippingDocument, userName);
        if (!string.IsNullOrWhiteSpace(validationError))
        {
            return Result<SapDeliveryResultDto>.Fail("SAP_DELIVERY_VALIDATION", validationError);
        }

        if (IsDisabledMode())
        {
            return Result<SapDeliveryResultDto>.Fail(
                "SAP_DELIVERY_DISABLED",
                "SAP delivery creation is disabled. Set SAP:Mode to ServiceLayer and configure SAP:ServiceLayerBaseUrl.");
        }

        SapDeliveryNotePayload payload = SapDeliveryNotePayloadFactory.Create(shippingDocument, userName);

        Result<string> createResult = await _serviceLayerClient
            .PostAsync("DeliveryNotes", payload, cancellationToken)
            .ConfigureAwait(false);

        if (!createResult.Success || string.IsNullOrWhiteSpace(createResult.Value))
        {
            return Result<SapDeliveryResultDto>.Fail(
                createResult.ErrorCode ?? "SAP_DELIVERY_CREATE_ERROR",
                createResult.Message ?? "SAP Delivery Note could not be created through Service Layer.");
        }

        return ParseDeliveryResponse(createResult.Value);
    }

    private bool IsDisabledMode()
    {
        return string.Equals(_options.Mode, "Disabled", StringComparison.OrdinalIgnoreCase)
            || string.Equals(_options.Mode, "None", StringComparison.OrdinalIgnoreCase);
    }

    private static string Validate(ShippingDocument shippingDocument, string userName)
    {
        if (shippingDocument.Status == ShippingStatus.DeliveryCreated)
        {
            return "Shipping document already has a SAP delivery created.";
        }

        if (shippingDocument.Status != ShippingStatus.Confirmed)
        {
            return "Shipping document must be confirmed before creating SAP delivery.";
        }

        if (string.IsNullOrWhiteSpace(shippingDocument.CustomerCode))
        {
            return "Shipping document requires CustomerCode.";
        }

        if (shippingDocument.Lines.Count == 0)
        {
            return "Shipping document must contain at least one line.";
        }

        if (shippingDocument.Lines.Any(line => string.IsNullOrWhiteSpace(line.ItemCode)))
        {
            return "All delivery lines require ItemCode.";
        }

        if (shippingDocument.Lines.Any(line => string.IsNullOrWhiteSpace(line.WarehouseCode)))
        {
            return "All delivery lines require WarehouseCode.";
        }

        if (shippingDocument.Lines.Any(line => line.PackedQuantity <= 0))
        {
            return "All delivery lines require PackedQuantity greater than zero.";
        }

        if (string.IsNullOrWhiteSpace(userName))
        {
            return "UserName is required.";
        }

        return string.Empty;
    }

    private static Result<SapDeliveryResultDto> ParseDeliveryResponse(string json)
    {
        try
        {
            using JsonDocument document = JsonDocument.Parse(json);
            JsonElement root = document.RootElement;

            int docEntry = ReadInt(root, "DocEntry");
            int docNum = ReadInt(root, "DocNum");

            if (docEntry <= 0 || docNum <= 0)
            {
                return Result<SapDeliveryResultDto>.Fail(
                    "SAP_DELIVERY_RESPONSE_INVALID",
                    "Service Layer response did not include valid DocEntry and DocNum.");
            }

            var result = new SapDeliveryResultDto
            {
                DocEntry = docEntry,
                DocNum = docNum,
                Message = $"SAP Delivery {docNum} created successfully."
            };

            return Result<SapDeliveryResultDto>.Ok(result, result.Message);
        }
        catch (JsonException exception)
        {
            return Result<SapDeliveryResultDto>.Fail(
                "SAP_DELIVERY_RESPONSE_INVALID",
                $"Service Layer response could not be parsed: {exception.Message}");
        }
    }

    private static int ReadInt(JsonElement element, string propertyName)
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
}

internal static class SapDeliveryNotePayloadFactory
{
    public static SapDeliveryNotePayload Create(ShippingDocument shippingDocument, string userName)
    {
        return new SapDeliveryNotePayload
        {
            CardCode = shippingDocument.CustomerCode,
            DocDate = DateTime.UtcNow.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture),
            TaxDate = DateTime.UtcNow.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture),
            Comments = $"SmartWMS shipping {shippingDocument.ShippingNumber} created by {userName}.",
            U_DTO_SW_ShippingId = shippingDocument.Id.ToString("D"),
            U_DTO_SW_ShippingNo = shippingDocument.ShippingNumber,
            DocumentLines = shippingDocument.Lines
                .OrderBy(line => line.LineNumber)
                .Select(CreateLine)
                .ToArray()
        };
    }

    private static SapDeliveryNoteLinePayload CreateLine(ShippingLine line)
    {
        var payload = new SapDeliveryNoteLinePayload
        {
            ItemCode = line.ItemCode,
            Quantity = line.PackedQuantity,
            WarehouseCode = line.WarehouseCode,
            U_DTO_SW_LineNum = line.LineNumber
        };

        if (!string.IsNullOrWhiteSpace(line.LotNumber))
        {
            payload.BatchNumbers = new[]
            {
                new SapDeliveryBatchPayload
                {
                    BatchNumber = line.LotNumber,
                    Quantity = line.PackedQuantity
                }
            };
        }

        return payload;
    }
}

internal sealed class SapDeliveryNotePayload
{
    public string CardCode { get; init; } = string.Empty;
    public string DocDate { get; init; } = string.Empty;
    public string TaxDate { get; init; } = string.Empty;
    public string Comments { get; init; } = string.Empty;
    public string U_DTO_SW_ShippingId { get; init; } = string.Empty;
    public string U_DTO_SW_ShippingNo { get; init; } = string.Empty;
    public IReadOnlyCollection<SapDeliveryNoteLinePayload> DocumentLines { get; init; } = Array.Empty<SapDeliveryNoteLinePayload>();
}

internal sealed class SapDeliveryNoteLinePayload
{
    public string ItemCode { get; init; } = string.Empty;
    public decimal Quantity { get; init; }
    public string WarehouseCode { get; init; } = string.Empty;
    public int U_DTO_SW_LineNum { get; init; }
    public IReadOnlyCollection<SapDeliveryBatchPayload>? BatchNumbers { get; set; }

    // Future enhancement:
    // When the WMS persistence layer stores SAP base document data, set:
    // BaseType = 17, BaseEntry = SalesOrderDocEntry, BaseLine = SalesOrderLineNum.
    public int? BaseType { get; init; }
    public int? BaseEntry { get; init; }
    public int? BaseLine { get; init; }
}

internal sealed class SapDeliveryBatchPayload
{
    public string BatchNumber { get; init; } = string.Empty;
    public decimal Quantity { get; init; }
}
