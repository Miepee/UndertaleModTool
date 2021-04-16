using Eto.Drawing;
using Eto.Forms;
using System;
using System.Windows;
using System.ComponentModel;
using UndertaleModLib;
using System.Diagnostics;
using System.Reflection;
using System.Globalization;
using System.Collections.Generic;
using System.IO.Pipes;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.IO;
using UndertaleModToolEto.Windows;
using UndertaleModLib.Scripting;

namespace UndertaleModToolEto
{
    public partial class MainWindow : Form, INotifyPropertyChanged
    {
        public UndertaleData Data { get; set; }
        public string FilePath { get; set; }
        public string ScriptPath { get; set; } // For the scripting interface specifically

        public string TitleMain { get; set; }  = "UndertaleModTool by krzys_h v" + FileVersionInfo.GetVersionInfo(Assembly.GetExecutingAssembly().Location).ProductVersion;

        private object _Highlighted;
        public object Highlighted { get { return _Highlighted; } set { _Highlighted = value; PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Highlighted")); } }
        private object _Selected;
        public object Selected { get { return _Selected; } private set { _Selected = value; PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Selected")); } }
        /*public Visibility IsGMS2 => (Data?.GeneralInfo?.Major ?? 0) >= 2 ? Visibility.Visible : Visibility.Collapsed;
        //God this is so ugly, if there's a better way, please, put in a pull request
        public Visibility IsExtProductIDEligible => (((Data?.GeneralInfo?.Major ?? 0) >= 2) || (((Data?.GeneralInfo?.Major ?? 0) == 1) && (((Data?.GeneralInfo?.Build ?? 0) >= 1773) || ((Data?.GeneralInfo?.Build ?? 0) == 1539)))) ? Visibility.Visible : Visibility.Collapsed;
        */
        public ObservableCollection<object> SelectionHistory { get; } = new ObservableCollection<object>();
        
        private bool _CanSave = false;
        public bool CanSave { get { return _CanSave; } private set { _CanSave = value; PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("CanSave")); } }
        public bool CanSafelySave = false;
        public bool FinishedMessageEnabled = true;

        public event PropertyChangedEventHandler PropertyChanged;
        private LoaderDialog scriptDialog;

        public Dictionary<string, NamedPipeServerStream> childFiles = new Dictionary<string, NamedPipeServerStream>();

        private SectionList DataList { get; set; } 


        public MainWindow()
        {
            Title = TitleMain;
            Icon = new Icon(1f, new Bitmap(UndertaleModToolEto.Properties.Resources.icon));
            MinimumSize = new Size(450, 800);

            ClientSize = new Size(1200, 800);

            var treeDataHead = new List<Section>();
            var nodes = new List<Section>();

            var foo = Data;

            nodes.Add(new Section("General info"));
            nodes.Add(new Section("Global init"));
            nodes.Add(new Section("Game End scripts"));
            nodes.Add(new Section("Audio groups"));
            nodes.Add(new Section("Sounds"));
            nodes.Add(new Section("Sprites"));
            nodes.Add(new Section("Backgrounds & Tile sets"));
            nodes.Add(new Section("Paths"));
            nodes.Add(new Section("Scripts"));
            nodes.Add(new Section("Shaders"));
            nodes.Add(new Section("Fonts"));
            nodes.Add(new Section("Timelines"));
            nodes.Add(new Section("Game objects"));
            nodes.Add(new Section("Rooms"));
            nodes.Add(new Section("Extensions"));
            nodes.Add(new Section("Texture page items"));
            nodes.Add(new Section("Code"));
            nodes.Add(new Section("Variables"));
            nodes.Add(new Section("Functions"));
            nodes.Add(new Section("Code locals (unused?)"));
            nodes.Add(new Section("Strings"));
            nodes.Add(new Section("Embedded textures"));
            nodes.Add(new Section("Embedded audio"));
            nodes.Add(new Section("Texture group information"));
            nodes.Add(new Section("Embedded images"));

            treeDataHead.Add(new Section("Data", nodes));
            DataList = new SectionListTreeGridView(treeDataHead);

            DataList.Focus();
            Content = new Splitter
            {
                SplitterWidth = 5,
                Position = 161,
                Panel1MinimumSize = 15,
                Panel2MinimumSize = 15,
                Panel1 = DataList.Control,
                Panel2 = new Panel { }
            };
            

            

            var aboutCommand = new Command { MenuText = "About..." };
            aboutCommand.Executed += (sender, e) => new AboutDialog() { ProgramDescription = "A tool that lets you modify Game Maker: Studio files", WebsiteLabel = "Source Code", Website = new Uri("https://github.com/krzys-h/UndertaleModTool"), ProgramName = "UndertaleModTool by krzys_h v" , Version = FileVersionInfo.GetVersionInfo(Assembly.GetExecutingAssembly().Location).ProductVersion, Title = "About" , Logo = new Bitmap(Icon)}.ShowDialog(this);


            #region file buttons

            var fileNew = new Command { MenuText = "&New", Shortcut = Application.Instance.CommonModifier | Keys.N };
            fileNew.Executed += Command_New;

            var fileOpen = new Command { MenuText = "&Open", Shortcut = Application.Instance.CommonModifier | Keys.O };
            fileOpen.Executed += Command_Open;

            var fileSave = new Command { MenuText = "&Save", Shortcut = Application.Instance.CommonModifier | Keys.S, Enabled = CanSave };

            var fileRun = new Command { MenuText = "&Run Game", Shortcut = Keys.F5, Enabled = CanSave };
            var fileDebug = new Command { MenuText = "Run under &debugger", Shortcut = Keys.Shift | Keys.F5, Enabled = CanSave };
           
            var fileOffset = new Command { MenuText = "Generate o&ffset map", Enabled = CanSave };
            
            var fileSettings = new Command { MenuText = "Settings", Shortcut = Keys.F4 };

            var quitCommand = new Command { MenuText = "Quit", Shortcut = Application.Instance.CommonModifier | Keys.Q };
            quitCommand.Executed += (sender, e) => Application.Instance.Quit();
            #endregion



            // create menu
            Menu = new MenuBar
            {
                Items =
                {
					// File submenu
					new ButtonMenuItem
                    {
                        Text = "&File",
                        Items =
                        {
                             fileNew,
                             fileOpen,
                             fileSave,
                             new SeparatorMenuItem { },
                             fileRun,
                             fileDebug,
                             new SeparatorMenuItem { },
                             fileOffset,
                             new SeparatorMenuItem { },
                             fileSettings
                        }
                    },
                    // Scripts submenu
					new ButtonMenuItem
                    {
                        Text = "&Scripts",
                        Items =
                        {
                            new ButtonMenuItem
                            {
                                Text =  "Run &builtin script",
                                
                                Items =
                                {
                                    new Command { MenuText = "&New" },
                                    new Command { MenuText = "&Open" }
                                }
                            },
                            new ButtonMenuItem
                            {
                                Text = "Run &experimental script",
                                Items =
                                {
                                    new Command { MenuText = "&Open" }
                                }
                            },
                            new Command { MenuText = "Run &other script..."}
                        }
                    },
                    new ButtonMenuItem
                    {
                        Text = "&Help",
                        Items =
                        {
                            new Command { MenuText = "&GitHub"}
                        }
                    }
				},
                
                /*ApplicationItems =
                {
					// application (OS X) or file menu (others)
					new ButtonMenuItem { Text = "&Preferences..." },
                },*/
                QuitItem = quitCommand,
                AboutItem = aboutCommand
            };
        }

        private Label objectLabel = new Label();

        private void Command_New(object sender, EventArgs e)
        {
            if (Data != null)
            {
                if (MessageBox.Show("Warning: you currently have a project open.\nAre you sure you want to make a new project?", "UndertaleModTool", MessageBoxButtons.YesNo, MessageBoxType.Warning) == DialogResult.No)
                    return;
            }

            FilePath = null;
            Data = UndertaleData.CreateNew();
            CloseChildFiles();
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Data"));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("IsGMS2"));
            ChangeSelection(Highlighted = new DescriptionView("Welcome to UndertaleModTool!", "New file created, have fun making a game out of nothing\nI TOLD YOU to open data.win, not create a new file! :P"));
            SelectionHistory.Clear();

            CanSave = true;
            CanSafelySave = true;
        }

        public void CloseChildFiles()
        {
            foreach (var pair in childFiles)
            {
                pair.Value.Close();
            }
            childFiles.Clear();
        }

        public void ChangeSelection(object newsel)
        {
            SelectionHistory.Add(Selected);
            Selected = newsel;
            UpdateObjectLabel(newsel);
        }

        private void UpdateObjectLabel(object obj)
        {
            int foundIndex = obj is UndertaleNamedResource ? Data.IndexOf(obj as UndertaleNamedResource, false) : -1;
            SetIDString(foundIndex == -1 ? "None" : (foundIndex == -2 ? "N/A" : Convert.ToString(foundIndex)));
        }

        private void SetIDString(string str)
        {
            objectLabel.Text = str;
        }

        public class DescriptionView
        {
            public string Heading { get; private set; }
            public string Description { get; private set; }

            public DescriptionView(string heading, string description)
            {
                Heading = heading;
                Description = description;
            }
        }

        private void Command_Open(object sender, EventArgs e)
        {
            DoOpenDialog();
        }

        private async Task<bool> DoOpenDialog()
        {
            OpenFileDialog dlg = new OpenFileDialog()
            {
                Filters =
                {
                    new FileFilter("Game Maker Studio data files", ".win", ".unx", ".ios", ".audiogroup", ".dat"),
                    new FileFilter("All Files", "")
                }
            };

            if (dlg.ShowDialog(this) != DialogResult.Abort)
            {
                //TODO: implement this!!!
                await LoadFile(dlg.FileName);
                return true;
            }
            return false;
        }

        private async Task LoadFile(string filename)
        {
            LoaderDialog dialog = new LoaderDialog("Loading", "Loading, please wait...");
            dialog.Owner = this;
            Task t = Task.Run(() =>
            {
                bool hadWarnings = false;
                UndertaleData data = null;
                try
                {
                    using (var stream = new FileStream(filename, FileMode.Open, FileAccess.Read))
                    {
                        data = UndertaleIO.Read(stream, warning =>
                        {
                            MessageBox.Show(warning, "Loading warning", MessageBoxButtons.OK, MessageBoxType.Warning);
                            hadWarnings = true;
                        });
                    }
                }
                catch (Exception e)
                {
                    MessageBox.Show("An error occured while trying to load:\n" + e.Message, "Load error", MessageBoxButtons.OK, MessageBoxType.Error);
                }

                Task.Run(() =>
                {
                    if (data != null)
                    {
                        if (data.UnsupportedBytecodeVersion)
                        {
                            MessageBox.Show("Only bytecode versions 14 to 17 are supported for now, you are trying to load " + data.GeneralInfo.BytecodeVersion + ". A lot of code is disabled and will likely break something. Saving/exporting is disabled.",
                                "Unsupported bytecode version", MessageBoxButtons.OK, MessageBoxType.Warning);
                            CanSave = false;
                            CanSafelySave = false;
                        }
                        else if (hadWarnings)
                        {
                            MessageBox.Show("Warnings occurred during loading. Data loss will likely occur when trying to save!", "Loading problems", MessageBoxButtons.OK, MessageBoxType.Warning);
                            CanSave = true;
                            CanSafelySave = false;
                        }
                        else
                        {
                            CanSave = true;
                            CanSafelySave = true;
                        }
                        if (data.GMS2_3)
                        {
                            MessageBox.Show("This game was built using GameMaker Studio 2.3 (or above). Support for this version is a work in progress, and you will likely run into issues decompiling code or in other places.", "GMS 2.3", MessageBoxButtons.OK, MessageBoxType.Warning);
                        }
                        if (data.IsYYC())
                        {
                            MessageBox.Show("This game uses YYC (YoYo Compiler), which means the code is embedded into the game executable. This configuration is currently not fully supported; continue at your own risk.", "YYC", MessageBoxButtons.OK, MessageBoxType.Warning);
                        }
                        if (System.IO.Path.GetDirectoryName(FilePath) != System.IO.Path.GetDirectoryName(filename))
                            CloseChildFiles();
                        this.Data = data;
                        this.FilePath = filename;
                        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Data"));
                        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("FilePath"));
                        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("IsGMS2"));
                        ChangeSelection(Highlighted = new DescriptionView("Welcome to UndertaleModTool!", "Double click on the items on the left to view them!"));
                        SelectionHistory.Clear();
                    }

                    Application.Instance.Invoke(new Action(() =>
                    {
                        //foreach()
                        //DataList = new SectionListTreeGridView(null);

                        ((Splitter)Content).Panel1 = DataList.Control;

                        dialog.Close();
                    }));
                });
            });
            dialog.ShowModal();
            await t;
        }

    }
}
