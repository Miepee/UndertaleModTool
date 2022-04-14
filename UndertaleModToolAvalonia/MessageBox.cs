using System;
using System.Threading.Tasks;
using Avalonia.Controls;
using MessageBox.Avalonia;
using MessageBox.Avalonia.Enums;

namespace UndertaleModToolAvalonia;

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



}