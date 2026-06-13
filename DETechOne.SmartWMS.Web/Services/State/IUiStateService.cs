namespace DETechOne.SmartWMS.Web.Services.State;

public interface IUiStateService
{
    bool IsBusy { get; }
    string? BusyMessage { get; }
    event Action? Changed;
    void ShowBusy(string? message = null);
    void HideBusy();
}
