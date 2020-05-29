using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.ComponentModel;
using System.Reflection;

namespace KnockKnock
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public Door_Manager DoorMngr = null;
        public CommentList Clist = null;

        public MainWindow(Door_Manager _doorMngr)
        {
            InitializeComponent();

            Clist = (CommentList)_myDataGrid.FindResource("_cmmntslst");
            DoorMngr = _doorMngr;
        }

        private bool FilterResults(object obj)
        {
            Door_Information di = obj as Door_Information;
            if (!di.IsPhaseValid(DoorMngr.PhaseId()))
                return false;
            if (!(bool)ShowUnmarkedCB.IsChecked && (di.doorNumber == string.Empty || di.doorNumber == null))
                return false;
            if (!(bool)ShowDemolishedCB.IsChecked && di.phaseDemolished == DoorMngr.PhaseName())
                return false;
            if (FilterTB.Text != "Filter" && FilterTB.Text.Length > 0)
            {
                pkhCommon.Wildcard wildcard = new pkhCommon.Wildcard(FilterTB.Text, System.Text.RegularExpressions.RegexOptions.IgnoreCase);
                if (!wildcard.IsMatch(di.doorNumber))
                    return false;
            }

            return true;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            IList<int> doorParameters = DoorMngr.parameterOrder;

            _myDataGrid.BeginInit();
            if (sender == null && e == null)
            {
                _myDataGrid.ItemsSource = null;
                _myDataGrid.Items.Clear();
                _myDataGrid.Columns.Clear();
            }

            //create comment column
            DataGridTemplateColumn CommentColumn = (DataGridTemplateColumn)_myDataGrid.FindResource("CommentColumnTemplate");
            CommentColumn.Width = DataGridLength.SizeToCells;
            CommentColumn.Header = DoorMngr.GetLocalizedCommentHeader();
            _myDataGrid.Columns.Add(CommentColumn);

            foreach (int pid in doorParameters)
            {
                DataTemplate dt = null;
                dt = new DataTemplate();
                Door_Parameter dp = DoorMngr.get_ParameterById(pid);

                //if-else order matters as yes/no parameters are also tokenized
                if (DoorMngr.IsParameterYesNo(pid))
                {
                    //create celltemplate for yes\no parameters
                    FrameworkElementFactory tb = new FrameworkElementFactory(typeof(CheckBox));
                    tb.SetBinding(TextBlock.DataContextProperty, new Binding("doorparameters[" + pid.ToString() + "]"));
                    tb.SetResourceReference(TextBlock.StyleProperty, "ParameterCheckboxStyle");
                    tb.AddHandler(CheckBox.MouseRightButtonUpEvent, new MouseButtonEventHandler(this.CheckboxParameter_MouseRightButtonUp));

                    FrameworkElementFactory vb = new FrameworkElementFactory(typeof(Viewbox));
                    vb.SetValue(Viewbox.StretchDirectionProperty, StretchDirection.Both);
                    vb.SetValue(Viewbox.StretchProperty, Stretch.Uniform);
                    vb.AppendChild(tb);
                    dt.VisualTree = vb;

                    DataGridTemplateColumn dgtc = new DataGridTemplateColumn();
                    dgtc.HeaderStyle = (Style)_myDataGrid.FindResource("ParameterHeaderStyle");
                    dgtc.Width = DataGridLength.SizeToCells;
                    dgtc.Header = dp.Name;
                    dgtc.CellTemplate = dt;
                    _myDataGrid.Columns.Add(dgtc);
                }
                else if (DoorMngr.IsParameterTokenized(pid))
                {
                    //create celltemplate for tokenized parameters
                    FrameworkElementFactory tb = new FrameworkElementFactory(typeof(TextBlock));
                    tb.SetBinding(TextBlock.DataContextProperty, new Binding("doorparameters[" + pid.ToString() + "]"));
                    tb.SetResourceReference(TextBlock.StyleProperty, "ParameterTextBlockStyle");

                    FrameworkElementFactory b = new FrameworkElementFactory(typeof(Grid));
                    b.SetResourceReference(Button.StyleProperty, "ParameterGridStyle");
                    b.AddHandler(Grid.MouseRightButtonUpEvent, new MouseButtonEventHandler(this.TokenParameter_MouseRightButtonUp));
                    b.AddHandler(Grid.MouseLeftButtonUpEvent, new MouseButtonEventHandler(this.TokenParameter_MouseLeftButtonUp));
                    b.AppendChild(tb);
                    dt.VisualTree = b;

                    DataGridTemplateColumn dgtc = new DataGridTemplateColumn();
                    dgtc.HeaderStyle = (Style)_myDataGrid.FindResource("ParameterHeaderStyle");
                    dgtc.Width = DataGridLength.SizeToCells;
                    dgtc.Header = dp.Name;
                    dgtc.CellTemplate = dt;
                    _myDataGrid.Columns.Add(dgtc);
                }
                else
                {
                    //create template for EDITING text parameters
                    FrameworkElementFactory tb = new FrameworkElementFactory(typeof(TextBox));
                    tb.SetBinding(TextBox.DataContextProperty, new Binding("doorparameters[" + pid.ToString() + "]"));
                    tb.SetResourceReference(TextBox.StyleProperty, "ParameterStringTextBoxStyle");
                    tb.SetValue(TextBox.NameProperty, "ParameterStringTB");
                    //tb.SetResourceReference(TextBox.ContextMenuProperty, "TextBoxContextMenu");
                    dt.VisualTree = tb;

                    //create template for SHOWING text parameters
                    DataTemplate dt2 = new DataTemplate();
                    FrameworkElementFactory tblk = new FrameworkElementFactory(typeof(TextBlock));
                    tblk.SetBinding(TextBlock.DataContextProperty, new Binding("doorparameters[" + pid.ToString() + "]"));
                    tblk.SetResourceReference(TextBlock.StyleProperty, "ParameterStringTextBlockStyle");
                    tblk.AddHandler(TextBlock.MouseRightButtonUpEvent, new MouseButtonEventHandler(this.StringParameter_MouseRightButtonUp));

                    FrameworkElementFactory b = new FrameworkElementFactory(typeof(Border));
                    b.SetResourceReference(Border.StyleProperty, "ParameterStringBorderStyle");
                    b.AppendChild(tblk);
                    dt2.VisualTree = b;

                    DataGridTemplateColumn dgtc = new DataGridTemplateColumn();
                    dgtc.HeaderStyle = (Style)_myDataGrid.FindResource("ParameterHeaderStyle");
                    dgtc.Width = DataGridLength.SizeToCells;
                    dgtc.Header = dp.Name;
                    dgtc.CellEditingTemplate = dt;
                    dgtc.CellTemplate = dt2;
                    _myDataGrid.Columns.Add(dgtc);
                }
            }

            foreach (Door_Information di in DoorMngr._doors)
            {
                if (di.doorComment != null || di.doorComment != string.Empty)
                    if (!Clist.Contains(di.doorComment))
                        Clist.Add(di.doorComment);
            }

            _myDataGrid.ItemsSource = DoorMngr._doors;
            _myDataGrid.EndInit();
            ListCollectionView plist_view = (ListCollectionView)CollectionViewSource.GetDefaultView(_myDataGrid.ItemsSource);
            plist_view.Filter = FilterResults;
            plist_view.CustomSort = new DoorMarkSort(ListSortDirection.Ascending);
            _myDataGrid.Tag = DoorMngr.GetLocalizedMarkHeader();

            AdornerLayer al = AdornerLayer.GetAdornerLayer(_myDataGrid);
            if (sender == null && e == null)
            {
                this.Measure(new Size(this.Width, this.Height));
                this.Arrange(new Rect(this.DesiredSize));
                Adorner[] toRemoveArray = al.GetAdorners(_myDataGrid);
                Adorner toRemove;
                if (toRemoveArray != null)
                {
                    toRemove = toRemoveArray[0];
                    al.Remove(toRemove);
                }
            }
            al.Add(new DataGridAdorner(_myDataGrid));

            PlanViewCB.BeginInit();
            PlanViewCB.ItemsSource = null;
            PlanViewCB.Items.Clear();
            PlanViewCB.ItemsSource = DoorMngr.FloorPlans;
            PlanViewCB.EndInit();

            phaseTB.Text = "Editing phase: " + DoorMngr.PhaseName();
            countTB.Text = "Showing " + _myDataGrid.Items.Count.ToString() + " of " + DoorMngr.DoorCount.ToString() + " doors.";
        }

        private void StringParameter_MouseRightButtonUp(object sender, MouseButtonEventArgs e)
        {
            TextBlock b = sender as TextBlock;
            if (b != null)
            {
                DataGridRow dgr = pkhCommon.WPF.Helpers.FindAncestorOrSelf<DataGridRow>(b);
                Door_Information di = dgr.Item as Door_Information;
                Door_Parameter dp = di.getParameter((int)b.Tag);
                dp.IsChecked = !dp.IsChecked;
            }
        }

        private void CheckboxParameter_MouseRightButtonUp(object sender, MouseButtonEventArgs e)
        {
            CheckBox tb = sender as CheckBox;
            if (tb != null)
            {
                DataGridRow dgr = pkhCommon.WPF.Helpers.FindAncestorOrSelf<DataGridRow>(tb);
                Door_Information di = dgr.Item as Door_Information;
                Door_Parameter dp = di.getParameter((int)tb.Tag);
                dp.IsChecked = !dp.IsChecked;
            }
        }

        private void TokenParameter_MouseRightButtonUp(object sender, MouseButtonEventArgs e)
        {
            Grid b = sender as Grid;
            if (b != null)
            {
                Door_Information di = DoorMngr.getDoorInformation((int)b.Tag);
                TextBlock tb = b.Children[0] as TextBlock;
                Door_Parameter dp = di.getParameter((int)tb.Tag);
                dp.IsChecked = !dp.IsChecked;
            }
        }

        private void TokenParameter_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            Grid b = sender as Grid;
            if (b != null)
            {
                Door_Information di = DoorMngr.getDoorInformation((int)b.Tag);
                TextBlock tb = b.Children[0] as TextBlock;
                Door_Parameter dp = di.getParameter((int)tb.Tag);
                dp.Value = DoorMngr.get_NextToken(tb.Text);
            }
        }

        private void _myDataGrid_PreparingCellForEdit(object sender, DataGridPreparingCellForEditEventArgs e)
        {
            TextBox tb = null;
            ContentPresenter cp = e.EditingElement as ContentPresenter;
            if (e.Column.Header as string == DoorMngr.GetLocalizedCommentHeader())
                tb = cp.ContentTemplate.FindName("CommentTB", cp) as TextBox;
            else
            {
                tb = pkhCommon.WPF.Helpers.FindVisualChild<TextBox>(cp);
            }

            if (tb != null)
                tb.Focus();
        }

        private void _myDataGrid_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            if (e.EditAction == DataGridEditAction.Commit)
            {
                ContentPresenter cp = e.EditingElement as ContentPresenter;
                Door_Information di = (Door_Information)_myDataGrid.SelectedCells[0].Item;
                if (di != null && !di.IsCommentReadOnly)
                {
                    TextBox tb = cp.ContentTemplate.FindName("CommentTB", cp) as TextBox;
                    if (tb != null)
                    {
                        string s = tb.Text;
                        if (!Clist.Contains(s))
                            Clist.Add(s);
                    }
                }
            }
        }

        private void _myDataGrid_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter || e.Key == Key.Return)
            {
                _myDataGrid.CommitEdit();
                e.Handled = true;
            }
        }

        private void commentCB_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBox cb = sender as ComboBox;
            StackPanel sp = cb.Parent as StackPanel;
            TextBox tb = sp.FindName("CommentTB") as TextBox;
            tb.Text = (string)cb.SelectedItem;
        }

        private void TextBlock_MouseRightButtonUp(object sender, MouseButtonEventArgs e)
        {
            TextBlock tb = sender as TextBlock;
            if (tb != null)
            {
                DataGridRow dgr = pkhCommon.WPF.Helpers.FindAncestorOrSelf<DataGridRow>(tb);
                Door_Information di = dgr.Item as Door_Information;
                di.IsCommentChecked = !di.IsCommentChecked;
            }
        }

        private void extendedDataCB_Click(object sender, RoutedEventArgs e)
        {
            if ((bool)extendedDataCB.IsChecked)
                _myDataGrid.RowDetailsVisibilityMode = DataGridRowDetailsVisibilityMode.VisibleWhenSelected;
            else
                _myDataGrid.RowDetailsVisibilityMode = DataGridRowDetailsVisibilityMode.Collapsed;
        }

        private void _myDataGrid_Sorting(object sender, DataGridSortingEventArgs e)
        {
            DataGridColumn column = e.Column;
            if (column.SortMemberPath != "doorNumber") return;

            System.Collections.IComparer comparer = null;
            e.Handled = true;
            ListSortDirection direction = (column.SortDirection != ListSortDirection.Ascending) ? ListSortDirection.Ascending : ListSortDirection.Descending;
            column.SortDirection = direction;
            ListCollectionView lcv = (ListCollectionView)CollectionViewSource.GetDefaultView(_myDataGrid.ItemsSource);
            comparer = new DoorMarkSort(direction);
            lcv.CustomSort = comparer;
        }
    }

    public class DoorMarkSort : System.Collections.IComparer
    {
        ListSortDirection _direction;

        public DoorMarkSort(ListSortDirection direction)
        {
            _direction = direction;
        }

        public int Compare(object x, object y)
        {
            Door_Information d1 = x as Door_Information;
            Door_Information d2 = y as Door_Information;
            string s1 = d1.doorNumber;
            string s2 = d2.doorNumber;

            if (s1 == null)
            {
                return 0;
            }
            if (s2 == null)
            {
                return 0;
            }

            int len1 = s1.Length;
            int len2 = s2.Length;
            int marker1 = 0;
            int marker2 = 0;

            // Walk through two the strings with two markers.
            while (marker1 < len1 && marker2 < len2)
            {
                char ch1 = s1[marker1];
                char ch2 = s2[marker2];

                // Some buffers we can build up characters in for each chunk.
                char[] space1 = new char[len1];
                int loc1 = 0;
                char[] space2 = new char[len2];
                int loc2 = 0;

                // Walk through all following characters that are digits or
                // characters in BOTH strings starting at the appropriate marker.
                // Collect char arrays.
                do
                {
                    space1[loc1++] = ch1;
                    marker1++;

                    if (marker1 < len1)
                    {
                        ch1 = s1[marker1];
                    }
                    else
                    {
                        break;
                    }
                } while (char.IsDigit(ch1) == char.IsDigit(space1[0]));

                do
                {
                    space2[loc2++] = ch2;
                    marker2++;

                    if (marker2 < len2)
                    {
                        ch2 = s2[marker2];
                    }
                    else
                    {
                        break;
                    }
                } while (char.IsDigit(ch2) == char.IsDigit(space2[0]));

                // If we have collected numbers, compare them numerically.
                // Otherwise, if we have strings, compare them alphabetically.
                string str1 = new string(space1);
                string str2 = new string(space2);

                int result;

                if (char.IsDigit(space1[0]) && char.IsDigit(space2[0]))
                {
                    int thisNumericChunk = int.Parse(str1);
                    int thatNumericChunk = int.Parse(str2);
                    result = thisNumericChunk.CompareTo(thatNumericChunk);
                }
                else
                {
                    result = str1.CompareTo(str2);
                }

                if (result != 0)
                {
                    return result;
                }
            }
            return len1 - len2;
        }
    }

    [ValueConversion(typeof(Door_Information.KeyState), typeof(string))]
    public class KeyStateConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            Door_Information.KeyState? state = null;
            state = (Door_Information.KeyState)value;

            switch (state)
            {
                case Door_Information.KeyState.IsKeyed:
                    return "Yes";
                case Door_Information.KeyState.NoKey:
                    return "No";
                case Door_Information.KeyState.RemoveKey:
                    return "Removed";
                default:
                    return "Error";
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return null;
        }
    }

    public class DataGridAdorner : Adorner
    {
        // Be sure to call the base class constructor. 
        public DataGridAdorner(UIElement adornedElement)
            : base(adornedElement)
        {

        }

        // A common way to implement an adorner's rendering behavior is to override the OnRender 
        // method, which is called by the layout system as part of a rendering pass. 
        protected override void OnRender(DrawingContext drawingContext)
        {
            DataGrid theGrid = this.AdornedElement as DataGrid;
            System.Windows.Controls.Primitives.DataGridRowHeader Row_header = pkhCommon.WPF.Helpers.FindVisualChild<System.Windows.Controls.Primitives.DataGridRowHeader>(theGrid);
            System.Windows.Controls.Primitives.DataGridColumnHeader Col_header = pkhCommon.WPF.Helpers.FindVisualChild<System.Windows.Controls.Primitives.DataGridColumnHeader>(theGrid);

            double width = Row_header.RenderSize.Width;
            double height = Col_header.RenderSize.Height;
            if (width < 1)
                width++;
            if (height < 1)
                height++;
            Size s = new Size(width - 1, height - 1);
            Rect GradientRect = new Rect(s);
            GradientRect.Location = new Point(1, 1);
            s = new Size(width + 1, height + 1);
            Rect BlackRect = new Rect(s);

            RadialGradientBrush radialGradient = new RadialGradientBrush();
            radialGradient.GradientOrigin = new Point(1, 1);
            radialGradient.Center = new Point(0.5, 0.5);
            radialGradient.RadiusX = 1;
            radialGradient.RadiusY = 1;
            radialGradient.GradientStops.Add(new GradientStop(Color.FromRgb(100, 207, 60), 1.0d)); // #FF64CF3C
            radialGradient.GradientStops.Add(new GradientStop(Colors.White, 0.0));
            radialGradient.Freeze();

            ConvertToPixelConverter c = new ConvertToPixelConverter();
            double fontsize = (double)c.Convert(Properties.Settings.Default.HeaderFont, null, null, null);
            FormattedText ft = new FormattedText(
                theGrid.Tag as string,
                System.Threading.Thread.CurrentThread.CurrentCulture,
                System.Windows.FlowDirection.LeftToRight,
                new Typeface(Properties.Settings.Default.HeaderFont.FontFamily.Name),
                fontsize, Brushes.Black,
                VisualTreeHelper.GetDpi(this).PixelsPerDip);
            Point origin = new Point(3, height - (3 + fontsize));

            drawingContext.DrawRectangle(new SolidColorBrush(Colors.Black), null, BlackRect);
            drawingContext.DrawRectangle(radialGradient, null, GradientRect);
            drawingContext.DrawText(ft, origin);
        }
    }

    [ValueConversion(typeof(bool), typeof(bool))]
    public class InvertBooleanConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            bool original = (bool)value;
            return !original;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            bool original = (bool)value;
            return !original;
        }
    }

    [ValueConversion(typeof(System.Drawing.Font), typeof(double))]
    public class ConvertToPixelConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            double result = 0;
            System.Drawing.Font f = (System.Drawing.Font)value;
            switch (f.Unit)
            {
                case System.Drawing.GraphicsUnit.Inch:
                    result = (double)f.Size * 96;
                    break;
                case System.Drawing.GraphicsUnit.Point:
                    result = (double)f.Size * (96 / 72);
                    break;
                case System.Drawing.GraphicsUnit.Millimeter:
                    result = (double)f.Size * (96 / 25.4);
                    break;
                default:
                    throw new Exception("ConvertToPixelConverter had an unkown unit type.");
            }

            return result;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return null;
        }
    }

    public class CalculateTextWidth : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            string str = values[0] as string;
            TextBlock tb = values[1] as TextBlock;
            if (str == null)
                return double.NaN;
            FormattedText ft = new FormattedText(
                str,
                culture,
                FlowDirection.LeftToRight,
                new Typeface(tb.FontFamily, tb.FontStyle, tb.FontWeight, tb.FontStretch),
                tb.FontSize,
                Brushes.Black,
                VisualTreeHelper.GetDpi(tb).PixelsPerDip);
            if ((ft.Width + 10) < tb.MinWidth)
                return double.NaN;
            else
                return ft.Width + 10;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, System.Globalization.CultureInfo culture)
        {
            return null;
        }
    }

    public class CommentList : System.Collections.ObjectModel.ObservableCollection<string>
    {
        public CommentList() : base() { }
        public CommentList theList { get { return this; } }
    }
}
