namespace DETechOne.SmartWMS.Web.Services.State;

public sealed class UiStateService : IUiStateService
{
    public bool IsBusy { get; private set; }
    public string? BusyMessage { get; private set; }
    public event Action? Changed;

    public void ShowBusy(string? message = null)
    {
        IsBusy = true;
        BusyMessage = message;
        Changed?.Invoke();
    }

    public void HideBusy()
    {
        IsBusy = false;
        BusyMessage = null;
        Changed?.Invoke();
    }
}
