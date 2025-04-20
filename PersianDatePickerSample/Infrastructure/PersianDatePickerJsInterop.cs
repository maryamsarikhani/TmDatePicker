using Microsoft.JSInterop;

namespace TMDatePicker.Infrastructure;

public class PersianDatePickerJsInterop : IAsyncDisposable
{
    private readonly Lazy<Task<IJSObjectReference>> moduleTask;

    public PersianDatePickerJsInterop(IJSRuntime jsRuntime)
    {
        moduleTask = new(() =>
        {
            Console.WriteLine("📦 Importing JS module from: persianDateMask.js");
            return jsRuntime.InvokeAsync<IJSObjectReference>(
                "import", "/_content/TMDatePicker/js/persianDateMask.js").AsTask();
        });
    }


    public async Task AddEnterKeyListener(string inputId, object dotNetHelper)
    {
        var module = await moduleTask.Value;
        await module.InvokeVoidAsync("addEnterKeyListener", inputId, dotNetHelper);
    }

    public async Task RegisterClickOutside(string pickerId, string inputId, string yearMonthSelectorId, object dotNetHelper)
    {
        var module = await moduleTask.Value;
        await module.InvokeVoidAsync("registerClickOutside", pickerId, inputId, yearMonthSelectorId, dotNetHelper);
    }

    public async Task RemovePersianDatePickerListeners(string pickerId)
    {
        var module = await moduleTask.Value;
        await module.InvokeVoidAsync("removePersianDatePickerListeners", pickerId);
    }

    public async Task ClearInputAndApplyMask(string inputId, object dotNetHelper)
    {
        var module = await moduleTask.Value;
        await module.InvokeVoidAsync("clearInputAndApplyMask", inputId, dotNetHelper);
    }

    public async Task ClearInput(string inputId)
    {
        var module = await moduleTask.Value;
        await module.InvokeVoidAsync("clearInput", inputId);
    }

    public async ValueTask DisposeAsync()
    {
        if (moduleTask.IsValueCreated)
        {
            var module = await moduleTask.Value;
            await module.DisposeAsync();
        }
    }
}