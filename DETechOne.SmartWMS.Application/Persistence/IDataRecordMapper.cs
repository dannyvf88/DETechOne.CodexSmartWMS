namespace DETechOne.SmartWMS.Application.Persistence;

public interface IDataRecordMapper
{
    bool HasColumn(string columnName);
    bool IsNull(string columnName);
    string? GetString(string columnName);
    int GetInt32(string columnName);
    long GetInt64(string columnName);
    decimal GetDecimal(string columnName);
    DateTime GetDateTime(string columnName);
    Guid GetGuid(string columnName);
    T? GetValue<T>(string columnName);
}
