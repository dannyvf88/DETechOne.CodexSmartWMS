using System.Data;
using DETechOne.SmartWMS.Application.Persistence;

namespace DETechOne.SmartWMS.Infrastructure.Persistence;

internal sealed class DataRecordMapper : IDataRecordMapper
{
    private readonly IDataRecord _record;
    private readonly Dictionary<string, int> _columns;

    public DataRecordMapper(IDataRecord record)
    {
        _record = record;
        _columns = Enumerable.Range(0, record.FieldCount)
            .ToDictionary(record.GetName, index => index, StringComparer.OrdinalIgnoreCase);
    }

    public bool HasColumn(string columnName) => _columns.ContainsKey(columnName);

    public bool IsNull(string columnName) => _record.IsDBNull(GetOrdinal(columnName));

    public string? GetString(string columnName) => GetValue<string>(columnName);

    public int GetInt32(string columnName) => Convert.ToInt32(GetRequiredValue(columnName));

    public long GetInt64(string columnName) => Convert.ToInt64(GetRequiredValue(columnName));

    public decimal GetDecimal(string columnName) => Convert.ToDecimal(GetRequiredValue(columnName));

    public DateTime GetDateTime(string columnName) => Convert.ToDateTime(GetRequiredValue(columnName));

    public Guid GetGuid(string columnName)
    {
        object value = GetRequiredValue(columnName);
        return value switch
        {
            Guid guid => guid,
            string text => Guid.Parse(text),
            _ => Guid.Parse(Convert.ToString(value) ?? string.Empty)
        };
    }

    public T? GetValue<T>(string columnName)
    {
        if (IsNull(columnName))
        {
            return default;
        }

        object value = _record.GetValue(GetOrdinal(columnName));

        if (value is T typedValue)
        {
            return typedValue;
        }

        Type targetType = Nullable.GetUnderlyingType(typeof(T)) ?? typeof(T);
        return (T)Convert.ChangeType(value, targetType);
    }

    private object GetRequiredValue(string columnName)
    {
        if (IsNull(columnName))
        {
            throw new InvalidOperationException($"Column '{columnName}' contains NULL.");
        }

        return _record.GetValue(GetOrdinal(columnName));
    }

    private int GetOrdinal(string columnName)
    {
        if (!_columns.TryGetValue(columnName, out int ordinal))
        {
            throw new InvalidOperationException($"Column '{columnName}' was not found in the data reader.");
        }

        return ordinal;
    }
}
