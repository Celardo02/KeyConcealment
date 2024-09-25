using System.Threading.Tasks;
using MsBox.Avalonia;
using MsBox.Avalonia.Enums;

namespace KeyConcealment.Service;

/// <summary>
/// Service class that provides opo up message logic
/// </summary>
public static class PopUpMessageHandler
{
    public static Task<ButtonResult> ShowMessage(string title, string msg,Icon i = Icon.None, ButtonEnum b = ButtonEnum.Ok)
    {
        return MessageBoxManager.GetMessageBoxStandard(title, msg, b, i).ShowAsync();
    }
}
