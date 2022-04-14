using System;
using System.Diagnostics;
using System.IO;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using log4net;

namespace UndertaleModToolAvalonia
{
	class Program
	{
		public static string GetExecutableDirectory()
		{
			return Path.GetDirectoryName(Process.GetCurrentProcess().MainModule.FileName);
		}


		// Initialization code. Don't use any Avalonia, third-party APIs or any
		// SynchronizationContext-reliant code before AppMain is called: things aren't initialized
		// yet and stuff might break.
		[STAThread]
		public static void Main(string[] args)
		{
			try
			{
				AppDomain currentDomain = AppDomain.CurrentDomain;
				// Handler for unhandled exceptions.
				currentDomain.UnhandledException += GlobalUnhandledExceptionHandler;
				// Handler for exceptions in threads behind forms.
				// Not sure yet how to do this
				//System.Windows.Forms.Application.ThreadException += GlobalThreadExceptionHandler;

				BuildAvaloniaApp().StartWithClassicDesktopLifetime(args);
			}
			catch (Exception e)
			{
				File.WriteAllText(Path.Combine(GetExecutableDirectory(), "crash.txt"), e.ToString());
				MessageBox.Show(e.ToString());
			}
		}

		// Avalonia configuration, don't remove; also used by visual designer.
		public static AppBuilder BuildAvaloniaApp()
			=> AppBuilder.Configure<App>()
				.UsePlatformDetect()
				.LogToTrace();

		private static void GlobalUnhandledExceptionHandler(object sender, UnhandledExceptionEventArgs e)
		{
			Exception ex = default(Exception);
			ex = (Exception)e.ExceptionObject;
			ILog log = LogManager.GetLogger(typeof(Program));
			log.Error(ex.Message + "\n" + ex.StackTrace);
			File.WriteAllText(Path.Combine(GetExecutableDirectory(), "crash2.txt"), (ex.ToString() + "\n" + ex.Message + "\n" + ex.StackTrace));
		}

		private static void GlobalThreadExceptionHandler(object sender, System.Threading.ThreadExceptionEventArgs e)
		{
			Exception ex = default(Exception);
			ex = e.Exception;
			ILog log = LogManager.GetLogger(typeof(Program)); //Log4NET
			log.Error(ex.Message + "\n" + ex.StackTrace);
			File.WriteAllText(Path.Combine(GetExecutableDirectory(), "crash3.txt"), (ex.Message + "\n" + ex.StackTrace));
		}
	}
}