using DETechOne.SmartWMS.Contracts.Dtos.Schema;

namespace DETechOne.SmartWMS.Application.Schema;

public interface IDatabaseSchemaDefinitionProvider
{
    SchemaInstallPlanDto GetPlan();
}
