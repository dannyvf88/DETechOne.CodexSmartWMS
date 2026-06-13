using System.Collections;
using System.Data.Common;
using System.Reflection;

namespace DETechOne.SmartWMS.Infrastructure.Persistence;

internal static class DbParameterBinder
{
    public static void Bind(DbCommand command, object? parameters)
    {
        if (parameters is null)
        {
            return;
        }

        if (parameters is IReadOnlyDictionary<string, object?> readOnlyDictionary)
        {
            foreach (KeyValuePair<string, object?> parameter in readOnlyDictionary)
            {
                Add(command, parameter.Key, parameter.Value);
            }

            return;
        }

        if (parameters is IDictionary dictionary)
        {
            foreach (DictionaryEntry entry in dictionary)
            {
                Add(command, Convert.ToString(entry.Key) ?? string.Empty, entry.Value);
            }

            return;
        }

        foreach (PropertyInfo property in parameters.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public))
        {
            Add(command, property.Name, property.GetValue(parameters));
        }
    }

    private static void Add(DbCommand command, string name, object? value)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            return;
        }

        DbParameter parameter = command.CreateParameter();
        parameter.ParameterName = name.StartsWith('@') ? name : $"@{name}";
        parameter.Value = value ?? DBNull.Value;
        command.Parameters.Add(parameter);
    }
}
