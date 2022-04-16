using System;
using System.Threading.Tasks;
using Avalonia.Controls;
using MessageBox.Avalonia;
using MessageBox.Avalonia.Enums;

namespace UndertaleModToolAvalonia;

public enum MessageBoxButton
{
	OK = ButtonEnum.Ok,
	OKCancel = ButtonEnum.OkCancel,
	YesNo = ButtonEnum.YesNo,
	YesNoCancel = ButtonEnum.YesNoCancel
}

public enum MessageBoxImage
{
	None = Icon.None,

	Error = Icon.Error,
	Hand = Error,
	Stop = Error,

	Question = Icon.Question,

	Warning = Icon.Warning,
	Exclamation = Warning,

	Information = Icon.Info,
	Asterisk = Information,

}

/// <summary>
/// Wrapper for Avalonia messagebox to act more like Wpf message box.
/// TODO: properly document
/// </summary>
public static class MessageBox
{
	public static ButtonResult Show(string messageBoxText)
	{
		return MessageBoxManager.GetMessageBoxStandardWindow("", messageBoxText).ShowDialog(null).Result;
	}

	public static ButtonResult Show(string messageBoxText, string title, MessageBoxButton buttons, MessageBoxImage image)
	{
		return MessageBoxManager.GetMessageBoxStandardWindow(title, messageBoxText, (ButtonEnum)buttons, (Icon)image).ShowDialog(null).Result;
	}



}