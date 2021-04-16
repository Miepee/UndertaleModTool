using Eto;
using Eto.Drawing;
using Eto.Forms;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Text;

namespace UndertaleModToolEto.Windows
{
    public partial class LoaderDialog : Dialog
    {

        private string _Message;

        public string MessageTitle { get; set; }
        public string Message { get => _Message; set { _Message = value; PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Message")); } }
        public bool PreventClose { get; set; }

        public string StatusText { get; set; } = "Please wait...";
        public double? Maximum
        {
            get
            {
                return !ProgressBar.Indeterminate ? ProgressBar.MaxValue : (double?)null;
            }

            set
            {
                ProgressBar.Indeterminate = !value.HasValue;
                if (value.HasValue)
                    ProgressBar.MaxValue = (int)value.Value;
            }
        }

        private string headerText = "Loading!";

        private string descriptionText = "Dummy!";

        private string title = "Loading";

        private Label headerLabel;

        private Label descriptionLabel;

        private ProgressBar ProgressBar;

        private DebugTraceListener listener;

        public event PropertyChangedEventHandler PropertyChanged;

        public LoaderDialog() 
        {

            Title = MessageTitle;
            Icon = new Icon(1f, new Bitmap(UndertaleModToolEto.Properties.Resources.icon));

            MinimumSize = new Size(400, 150);
            Size = MinimumSize;
            Resizable = false;

            headerLabel = new Label { Text = Message };
            descriptionLabel = new Label { Text = StatusText };
            ProgressBar = new ProgressBar { Size = new Size(360, 30), Indeterminate = true };

            Content = new StackLayout
            {
                Spacing = 10,
                Padding = 10,

                Items =
                {
                    new Label{ },   //just there for padding
                    headerLabel,
                    ProgressBar,
                    descriptionLabel
                }
            };
        }

        public LoaderDialog(string title, string message) : this()
        {
            Title = title;
            headerText = message;

            headerLabel.Text = headerText;

            listener = new DebugTraceListener(this);
            Trace.Listeners.Add(listener);

        }



        private void Window_Unloaded(object sender, EventArgs e)
        {
            Trace.Listeners.Remove(listener);
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            e.Cancel = this.PreventClose;
        }

        public void TryHide()
        {
            Application.Instance.Invoke(new Action(() =>
            {
                if (Visible)
                {
                    this.PreventClose = false;
                    Close();
                }
            }));

        }

        public void ReportProgress(string message)
        {
            Application.Instance.Invoke(new Action(() =>
            {
                descriptionLabel.Text = message;
            }));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("StatusText"));
        }

        public void ReportProgress(string message, double value)
        {
            Application.Instance.Invoke(new Action(() =>
            {
                ReportProgress(value + "/" + Maximum + (!String.IsNullOrEmpty(message) ? ": " + message : ""));
                ProgressBar.Value = (int)value;
            }));
        }

        public void Update(string message, string status, double progressValue, double maxValue)
        {
            if (!Visible)
                Application.Instance.Invoke(ShowModal);

            if (message != null)
               Application.Instance.Invoke(() => Message = message);

            if (maxValue != 0)
                Application.Instance.Invoke(new Action(() =>
                {
                    
                }));
            Application.Instance.Invoke(() => Maximum = maxValue);

            ReportProgress(status, progressValue);
        }

        private class DebugTraceListener : TraceListener
        {
            private LoaderDialog loaderDialog;

            public DebugTraceListener(LoaderDialog loaderDialog)
            {
                this.loaderDialog = loaderDialog;
            }

            public override void Write(string message)
            {
                WriteLine(message);
            }

            public override void WriteLine(string message)
            {
                loaderDialog.ReportProgress(message);
            }
        }
    }
}
