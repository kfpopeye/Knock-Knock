using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Input;
using System.Windows.Controls;
using System.Windows.Data;

namespace KnockKnock
{
    public partial class MainWindow : Window
    {
        //floor plan combobox
        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (PlanViewCB.SelectedItem != null)
                DoorMngr.ShowFloorPlaninRevit(PlanViewCB.SelectedItem as string);
        }

        private void FilterTB_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                FilterTB.Text = "Filter";
                CollectionView plist_view = (CollectionView)CollectionViewSource.GetDefaultView(_myDataGrid.ItemsSource);
                plist_view.Refresh();
                countTB.Text = "Showing " + _myDataGrid.Items.Count.ToString() + " of " + DoorMngr.DoorCount.ToString() + " doors.";
            }

            if (e.Key == Key.Return || e.Key == Key.Enter)
            {
                CollectionView plist_view = (CollectionView)CollectionViewSource.GetDefaultView(_myDataGrid.ItemsSource);
                plist_view.Refresh();
                countTB.Text = "Showing " + _myDataGrid.Items.Count.ToString() + " of " + DoorMngr.DoorCount.ToString() + " doors.";
            }
        }

        private void FilterTB_GotFocus(object sender, RoutedEventArgs e)
        {
            FilterTB.Text = "";
        }

        //works for ShowUnmarked and ShowDemolished checkboxes
        private void ShowUnmarkedCB_Click(object sender, RoutedEventArgs e)
        {
            if (_myDataGrid.CancelEdit(DataGridEditingUnit.Row))
            {
                CollectionView plist_view = (CollectionView)CollectionViewSource.GetDefaultView(_myDataGrid.ItemsSource);
                plist_view.Refresh();
                countTB.Text = "Showing " + _myDataGrid.Items.Count.ToString() + " of " + DoorMngr.DoorCount.ToString() + " doors.";
            }
        }

        private void Always_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private void UnkeyDoor_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            if (_myDataGrid != null && _myDataGrid.IsInitialized)
            {
                if (_myDataGrid.SelectedCells != null && _myDataGrid.SelectedCells.Count > 0)
                {
                    Door_Information di = _myDataGrid.SelectedItem as Door_Information;
                    if (di.key == Door_Information.KeyState.IsKeyed)
                        e.CanExecute = true;
                    else
                        e.CanExecute = false;
                }
                else
                    e.CanExecute = false;
            }
        }

        private void IfDoorSelected_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            if (_myDataGrid != null && _myDataGrid.IsInitialized)
            {
                if (_myDataGrid.SelectedCells != null && _myDataGrid.SelectedCells.Count > 0)
                    e.CanExecute = true;
                else
                    e.CanExecute = false;
            }
        }

        private void ShowDoorCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            Door_Information di = _myDataGrid.SelectedItem as Door_Information;
            DoorMngr.ShowDoorInRevit(di.Id);
        }

        private void UncheckDoorCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            Door_Information di = _myDataGrid.SelectedItem as Door_Information;
            di.unCheckValues();
        }

        private void ClearDoorCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            Door_Information di = _myDataGrid.SelectedItem as Door_Information;
            di.clearValues();
        }

        private void UndoDoorCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            Door_Information di = _myDataGrid.SelectedItem as Door_Information;
            DoorMngr.UndoCurrentDoor(di.Id);
        }

        private void UnkeyDoorCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            Door_Information di = _myDataGrid.SelectedItem as Door_Information;
            di.removeKey();
        }

        private void UncheckAllCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            Door_Information di = _myDataGrid.SelectedItem as Door_Information;
            DoorMngr.UnCheckAllDoors();
        }

        private void ClearAllCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            Door_Information di = _myDataGrid.SelectedItem as Door_Information;
            DoorMngr.ClearAllDoors();
        }

        private void UndoAllCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            Door_Information di = _myDataGrid.SelectedItem as Door_Information;
            DoorMngr.UndoAllDoors();
        }

        private void OKCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            this.DialogResult = true;
            this.Close();
        }

        private void SettingsCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            Parameter_Chooser_Form pcf = new Parameter_Chooser_Form(DoorMngr);
            System.Diagnostics.Process process = System.Diagnostics.Process.GetCurrentProcess();
            IntPtr h = process.MainWindowHandle;
            if (pcf.ShowDialog(new pkhCommon.Windows.WindowHandle(h)) == System.Windows.Forms.DialogResult.Cancel)
                return;
            DoorMngr.Phase(pcf.phase);
            DoorMngr.tokenizedParameters = pcf.userList;
            DoorMngr.tokenList = pcf.tokenList;
            int[] ar = new int[pcf.userList.Count];
            pcf.userList.Keys.CopyTo(ar, 0);
            DoorMngr.parameterOrder = ar;
            pcf.Dispose();
            Window_Loaded(null, null);
        }

        private void HelpCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            KnockKnockApp.kk_help.Launch();
        }

        private void CheckParameterCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            Door_Parameter dp = DoorMngr.get_ParameterByName(_myDataGrid.CurrentCell.Column.Header.ToString());
            Door_Information di = _myDataGrid.CurrentCell.Item as Door_Information;
            di.getParameter(dp.Id).IsChecked = !di.getParameter(dp.Id).IsChecked;
        }
    }

    public static class CustomCommands
    {
        public static readonly RoutedUICommand ShowDoor = new RoutedUICommand("Causes Revit to zoom in on the selected door.", "Show Door", typeof(CustomCommands));

        public static readonly RoutedUICommand UncheckDoor = new RoutedUICommand("Removes checked symbol from all parameters on the selected door.", "Uncheck Door", typeof(CustomCommands));
        public static readonly RoutedUICommand ClearDoor = new RoutedUICommand("Removes all values from the selected door unless they are read only.", "Clear Door", typeof(CustomCommands));
        public static readonly RoutedUICommand UndoDoor = new RoutedUICommand("Removes all changes made to all parameters on the selected door.", "Undo Door", typeof(CustomCommands));
        public static readonly RoutedUICommand UnkeyDoor = new RoutedUICommand("Removes key value from the selected door.", "Unkey Door", typeof(CustomCommands));

        public static readonly RoutedUICommand UncheckAll = new RoutedUICommand("Removes checked symbol from all parameters on all doors.", "Uncheck All", typeof(CustomCommands));
        public static readonly RoutedUICommand ClearAll = new RoutedUICommand("Removes all values from all the doors unless they are read only.", "Clear All", typeof(CustomCommands));
        public static readonly RoutedUICommand UndoAll = new RoutedUICommand("Removes all changes made to all parameters on all the doors.", "Undo All", typeof(CustomCommands));

        public static readonly RoutedUICommand OK = new RoutedUICommand("Save changes and quit.", "OK", typeof(CustomCommands), new InputGestureCollection() { new KeyGesture(Key.F4, ModifierKeys.Alt) });
        public static readonly RoutedUICommand Settings = new RoutedUICommand("Opens settings dialog.", "Settings", typeof(CustomCommands));
        public static readonly RoutedUICommand Help = new RoutedUICommand("Shows help file.", "Help", typeof(CustomCommands));

        public static readonly RoutedUICommand CheckParameter = new RoutedUICommand("Marks the parameter as checked.", "Mark Checked", typeof(CustomCommands));
    }
}
