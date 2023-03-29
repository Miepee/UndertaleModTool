﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using UndertaleModLib;
using UndertaleModLib.Models;

namespace UndertaleModTool.Windows
{
    /// <summary>
    /// Interaction logic for FindReferencesResults.xaml
    /// </summary>
    public partial class FindReferencesResults : Window
    {
        private static readonly MainWindow mainWindow = Application.Current.MainWindow as MainWindow;
        private object highlighted;
        private readonly UndertaleData data;

        private void Window_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (!IsVisible || IsLoaded)
                return;

            if (Settings.Instance.EnableDarkMode)
                MainWindow.SetDarkTitleBarForWindow(this, true, false);
        }

        public FindReferencesResults(UndertaleResource sourceObj, UndertaleData data, (string, object[])[] results)
        {
            InitializeComponent();

            this.data = data;

            string sourceObjName;
            if (sourceObj is UndertaleNamedResource namedObj)
                sourceObjName = namedObj.Name.Content;
            else
                sourceObjName = sourceObj.GetType().Name;

            Title = $"The references of game object \"{sourceObjName}\"";
            label.Text = $"The search results for the game object\n\"{sourceObjName}\".";

            if (results is null)
                ResultsTree.Background = new VisualBrush(new Label()
                {
                    Content = "No references found.",
                    FontSize = 16
                }) { Stretch = Stretch.None };
            else
                ProcessResults(results);
        }

        private void SearchBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            foreach (var child in ResultsTree.Items)
                ((child as TreeViewItem)?.ItemsSource as ICollectionView)?.Refresh();
        }

        private void DoneButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void ProcessResults((string, object[])[] results)
        {
            var filterConv = new FilteredViewConverter();
            BindingOperations.SetBinding(filterConv, FilteredViewConverter.FilterProperty, new Binding("Text")
            {
                Source = SearchBox,
                UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged
            });

            var namedResTemplate = XamlReader.Parse(
            @"
                <HierarchicalDataTemplate xmlns='http://schemas.microsoft.com/winfx/2006/xaml/presentation'>
                    <TextBlock Text='{Binding Name.Content}'/>
                </HierarchicalDataTemplate>
            ") as HierarchicalDataTemplate;

            foreach (var result in results)
            {
                var item = new TreeViewItem()
                {
                    Header = result.Item1,
                    DataContext = result.Item2
                };
                item.SetBinding(TreeView.ItemsSourceProperty, new Binding(".")
                {
                    Converter = filterConv,
                    Mode = BindingMode.OneWay
                });
                if (result.Item2[0] is UndertaleNamedResource)
                    item.ItemTemplate = namedResTemplate;
                else if (result.Item2[0] is GeneralInfoEditor)
                {
                    ResultsTree.Items.Add(new TextBlock()
                    {
                        Text = "General Info",
                        DataContext = result.Item2[0],
                        ContextMenu = TryFindResource("StandaloneTabMenu") as ContextMenu
                    });
                    continue;
                }
                else if (result.Item2[0] is object[])
                    item.ItemTemplate = TryFindResource("ChildInstTemplate") as HierarchicalDataTemplate;

                ResultsTree.Items.Add(item);

                item.IsExpanded = true;
            }
        }

        private void Open(object obj, bool inNewTab = false)
        {
            mainWindow.Focus();

            if (obj is object[] inst)
            {
                if (inst[^1] is UndertaleRoom room)
                {
                    mainWindow.ChangeSelection(room, inNewTab);
                    mainWindow.CurrentTab.LastContentState = new RoomTabState()
                    {
                        SelectedObject = inst[0],
                        ObjectTreeItemsStates = new[] { false, false, false, false, true }
                    };
                    mainWindow.CurrentTab.RestoreTabContentState();
                }
            }
            else
                mainWindow.ChangeSelection(obj, inNewTab);

        }

        private void MenuItem_ContextMenuOpened(object sender, RoutedEventArgs e)
        {
            var menu = sender as ContextMenu;
            foreach (var item in menu.Items)
            {
                var menuItem = item as MenuItem;
                if ((menuItem.Header as string) == "Find all references")
                {
                    Type objType = menu.DataContext is object[] inst
                                   ? inst[^1].GetType() : menu.DataContext.GetType();
                    menuItem.Visibility = UndertaleResourceReferenceMap.IsTypeReferenceable(objType)
                                          ? Visibility.Visible : Visibility.Collapsed;

                    break;
                }
            }
        }
        private void MenuItem_OpenInNewTab_Click(object sender, RoutedEventArgs e)
        {
            Open(highlighted, true);
        }
        private void MenuItem_FindAllReferences_Click(object sender, RoutedEventArgs e)
        {
            UndertaleResource res = null;

            var obj = (sender as FrameworkElement)?.DataContext;
            if (obj is UndertaleResource res1)
                res = res1;
            else if (obj is object[] inst && inst[^1] is UndertaleResource res2)
                res = res2;

            if (res is null)
            {
                this.ShowError("The selected object is not an \"UndertaleResource\".");
                return;
            }

            FindReferencesTypesDialog dialog = null;
            try
            {
                dialog = new(res, data);
                dialog.ShowDialog();
            }
            catch (Exception ex)
            {
                this.ShowError("An error occured in the object references related window.\n" +
                               $"Please report this on GitHub.\n\n{ex}");
            }
            finally
            {
                dialog?.Close();
            }
        }


        private void ResultsTree_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (e.OriginalSource is not TextBlock textBlock)
                return;
            if (textBlock.DataContext is string)
                return;

            Open(highlighted);
        }
        private void ResultsTree_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return)
                Open(highlighted);
        }
        private void ResultsTree_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            if (e.NewValue is TreeViewItem)
                return;
            if (e.NewValue is TextBlock block)
            {
                if (block.DataContext is GeneralInfoEditor or GlobalInitEditor or GameEndEditor)
                {
                    highlighted = block.DataContext;
                    return;
                }
            }

            highlighted = e.NewValue;
        }
        private void ResultsTree_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ButtonState == MouseButtonState.Pressed && e.ChangedButton == MouseButton.Middle)
            {
                if (e.OriginalSource is not TextBlock textBlock)
                    return;

                TreeViewItem item = MainWindow.GetNearestParent<TreeViewItem>(textBlock);
                if (item is null)
                    return;

                item.IsSelected = true;

                if (item.DataContext is Array)
                    return;

                Open(highlighted, true);
            }
        }
        private void ResultsTree_PreviewMouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            TreeViewItem treeViewItem = MainWindow.VisualUpwardSearch<TreeViewItem>(e.OriginalSource as DependencyObject);

            if (treeViewItem != null)
            {
                treeViewItem.Focus();
                e.Handled = true;
            }
        }
    }

    public class ChildInstanceNameConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is object[] inst)
            {
                StringBuilder sb = new();
                for (int i = 0; i < inst.Length; i++)
                {
                    var link = inst[i];
                    if (link is UndertaleNamedResource namedObj)
                        sb.Append(namedObj.Name);
                    else
                        sb.Append(link.ToString());

                    if (i != inst.Length - 1)
                        sb.Append(" — ");
                }

                return sb.ToString();
            }

            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
