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

namespace UndertaleModToolEto
{
    public partial class MainWindow : Form
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
        
        public ObservableCollection<object> SelectionHistory { get; } = new ObservableCollection<object>();
        */
        private bool _CanSave = false;
        public bool CanSave { get { return _CanSave; } private set { _CanSave = value; PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("CanSave")); } }
        public bool CanSafelySave = false;
        public bool FinishedMessageEnabled = true;

        public event PropertyChangedEventHandler PropertyChanged;
        //private LoaderDialog scriptDialog;

        public TreeGridView treeGridView1;

        public static TextArea eventLog;
        public static Panel contentContainer;
        public static Navigation navigation;
        public static Control testMethod()
        {
            contentContainer = new Panel();

            if (Splitter.IsSupported)
            {
                var splitter = new Splitter
                {
                    Position = 200,
                    FixedPanel = SplitterFixedPanel.Panel1,
                    Panel1 = new Label { Text = "ha"},
                    Panel1MinimumSize = 150,
                    Panel2MinimumSize = 300,
                    // for now, don't show log in mobile
                    //Panel2 = RightPane()
                    Panel2 = new Label { Text = "haaaa" }
                };

                return splitter;
            }
            throw new ArgumentException(string.Format(CultureInfo.CurrentCulture, "Platform must support splitter or navigation"));
        }



        public SectionList SectionList { get; set; } 
        public Control testMethod2()
        {
            

            SectionList.Focus();
            treeGridView1 = new TreeGridView { };
            var treeGridItemCollection = new TreeGridItemCollection();
            treeGridView1.Columns.Add(new GridColumn() { HeaderText = "irgendwas", DataCell = new TextBoxCell(" test123") });
            treeGridView1.Columns.Add(new GridColumn() { HeaderText = "liste2", DataCell = new TextBoxCell("sdf") });
            int i = 0;
            for (i = 0; i < 5; i++)
            {
                var item = new TreeGridItem(new string[] { "sasdfads", "irgendwas", "liste2" } );
                item.Values = new TextBox[] { new TextBox { Text = "ele1" }, new TextBox { Text = "ele2" } };
                item.Parent = new TreeGridItem();
                item.Tag =  "asdfadsf" ;
                treeGridItemCollection.Add(item);
            }

          

            treeGridView1.DataStore = treeGridItemCollection;

            var splitter = new Splitter
            {
                SplitterWidth = 5,
                Panel1MinimumSize = 100,
                Panel1 = SectionList.Control,
                
                Panel2  = new Panel { }
            };
       
            return splitter;
        }

        public MainWindow()
        {
            Title = TitleMain;
            Icon = new Icon(1f, new Bitmap(UndertaleModToolEto.Properties.Resources.icon));
            MinimumSize = new Size(450, 800);

            var emptyList = new List<Section>();
            var treeDataHead = new List<Section>();
            var nodes = new List<Section>();

            nodes.Add(new Section("General info", emptyList));
            nodes.Add(new Section("Global init", emptyList));
            nodes.Add(new Section("Game End scripts", emptyList));
            nodes.Add(new Section("Audio groups", emptyList));
            nodes.Add(new Section("Sounds", emptyList));
            nodes.Add(new Section("Sprites", emptyList));
            nodes.Add(new Section("Backgrounds & Tile sets", emptyList));
            nodes.Add(new Section("Paths", emptyList));
            nodes.Add(new Section("Scripts", emptyList));
            nodes.Add(new Section("Shaders", emptyList));
            nodes.Add(new Section("Fonts", emptyList));
            nodes.Add(new Section("Timelines", emptyList));
            nodes.Add(new Section("Game objects", emptyList));
            nodes.Add(new Section("Rooms", emptyList));
            nodes.Add(new Section("Extensions", emptyList));
            nodes.Add(new Section("Texture page items", emptyList));
            nodes.Add(new Section("Code", emptyList));
            nodes.Add(new Section("Variables", emptyList));
            nodes.Add(new Section("Functions", emptyList));
            nodes.Add(new Section("Code locals (unused?)", emptyList));
            nodes.Add(new Section("Strings", emptyList));
            nodes.Add(new Section("Embedded textures", emptyList));
            nodes.Add(new Section("Embedded audio", emptyList));
            nodes.Add(new Section("Texture group information", emptyList));
            nodes.Add(new Section("Embedded images", emptyList));

            treeDataHead.Add(new Section("Data", nodes));

            SectionList = new SectionListTreeGridView(treeDataHead);


            Content = testMethod2();

            this.MouseDoubleClick += MainWindow_MouseDoubleClick;
            

            // create a few commands that can be used for the menu and toolbar
            var clickMe = new Command { MenuText = "Click Me!", ToolBarText = "Click Me!" };
            clickMe.Executed += (sender, e) => MessageBox.Show(this, "I was clicked!");

            var quitCommand = new Command { MenuText = "Quit", Shortcut = Application.Instance.CommonModifier | Keys.Q };
            quitCommand.Executed += (sender, e) => Application.Instance.Quit();

            var aboutCommand = new Command { MenuText = "About..." };
            aboutCommand.Executed += (sender, e) => new AboutDialog() { ProgramDescription = "A tool that lets you modify Game Maker: Studio files", WebsiteLabel = "Source Code", Website = new Uri("https://github.com/krzys-h/UndertaleModTool"), ProgramName = "UndertaleModTool by krzys_h v" , Version = FileVersionInfo.GetVersionInfo(Assembly.GetExecutingAssembly().Location).ProductVersion, Title = "About" , Logo = new Bitmap(Icon)}.ShowDialog(this);

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
                            new Command { MenuText = "&New", Shortcut = Application.Instance.CommonModifier | Keys.N },
                            new Command { MenuText = "&Open", Shortcut = Application.Instance.CommonModifier | Keys.O },
                            new Command { MenuText = "&Save", Shortcut = Application.Instance.CommonModifier | Keys.S, Enabled = CanSave},
                            new SeparatorMenuItem { },
                            new Command { MenuText = "&Run Game", Shortcut = Keys.F5, Enabled = CanSave },
                            new Command { MenuText = "Run under &debugger", Shortcut = Keys.Shift | Keys.F5, Enabled = CanSave },
                            new SeparatorMenuItem { },
                            new Command { MenuText = "Generate o&ffset map", Enabled = CanSave },
                            new SeparatorMenuItem { },
                            new Command { MenuText = "Settings", Shortcut = Keys.F4 },
                        }
                    },
                    // Scripts submenu
					new ButtonMenuItem
                    {
                        Text = "&Sripts",
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
					// new ButtonMenuItem { Text = "&Edit", Items = { /* commands/items */ } },
					// new ButtonMenuItem { Text = "&View", Items = { /* commands/items */ } },
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

        private void MainWindow_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            int val = 3;
        }
    }
}
