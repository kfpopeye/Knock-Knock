using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace KnockKnock
{
	public partial class Parameter_Chooser_Form : Form
	{
		private IDictionary<int,bool> _userList = null;
		private string _phase = null;
        private Door_Manager _drmngr = null;
        private SourceGrid.Cells.Controllers.CheckBox _controller = null;
        private SourceGrid.Cells.Controllers.SGButton _viewController = null;
        private SourceGrid.Cells.Views.Cell _yesNoCell = null;
        private SourceGrid.Cells.Views.CheckBox _yesNoCheck = null;
        private SourceGrid.Cells.Views.Cell _viewCell = null;
        private SourceGrid.Cells.Views.CheckBox _viewCheck = null;

        /// <summary>
        /// The list of parameter Id's in the order specified by the user and whether or not the are tokenized.
        /// </summary>
		public IDictionary<int, bool> userList
		{
			get
			{
				if (_userList != null)
					return _userList;
				else
					throw new NullReferenceException("Parameter_Chooser_Form.Userlist was null.");
			}
		}

		public string[] tokenList
		{
			get{ return tokenTextBox.Lines; }
		}
		
		public string phase
		{
			get{ return _phase; }
		}

		/// <summary>
		/// Constructor is used when the door manager contains all needed information.
		/// </summary>
		/// <param name="drmngr">The door manager class.</param>
		public Parameter_Chooser_Form(Door_Manager drmngr)
		{
			InitializeComponent();

            _drmngr = drmngr;

            Door_Parameter[] allParameters = _drmngr.get_AllParameters();

            foreach (Door_Parameter dp in allParameters)
            {
                if (!_drmngr.parameterOrder.Contains(dp.Id))  //set the left side list
                    listBox1.Items.Add(dp);
            }

            //set the right side list
            grid1.BorderStyle = BorderStyle.Fixed3D;
            grid1.ColumnsCount = 2;

            int r = 0;
            foreach (int pid in _drmngr.parameterOrder)
            {
                Door_Parameter dp = _drmngr.get_ParameterById(pid);
                if (dp != null)
                {
                    addRow(r, dp);
                    r++;
                }
            }

            //set the tokens list
            if (_drmngr.tokenList != null)
                tokenTextBox.Lines = _drmngr.tokenList.ToArray<string>();

            //set the phases
            int p = 0;
            int x = 0;
            foreach (string phase in _drmngr.AllPhases.Keys)
            {
                if (_drmngr.phaseHasDoors(_drmngr.AllPhases[phase]))
                {
                    comboBox_Phase.Items.Add(phase);
                    if (phase == _drmngr.PhaseName())
                        p = x;
                    x++;
                }
            }
            comboBox_Phase.SelectedIndex = p;
            _phase = comboBox_Phase.SelectedItem.ToString();

            setButtonStates();
		}

		/// <summary>
		/// Constructor is used when the door manager has incomplete information. IE/ when no schema exists on the project.
		/// </summary>
		/// <param name="drmngr">The class that holds the door information</param>
		/// <param name="schedulelist">A list of parameters should appear on the right side of the chooser form. Either from a schema or schedule</param>
		/// <param name="setphase">The phase name the chooser form should be set to</param>
		public Parameter_Chooser_Form(Door_Manager drmngr, string[] schedulelist, string setphase)
		{
			InitializeComponent();

            _drmngr = drmngr;

            Door_Parameter[] allParameters = _drmngr.get_AllParameters();

            foreach (Door_Parameter dp in allParameters)
            {
                if (!schedulelist.Contains(dp.ToString()))  //set the left side list
                    listBox1.Items.Add(dp);
            }

            //set the right side list
            grid1.BorderStyle = BorderStyle.Fixed3D;
            grid1.ColumnsCount = 2;

            int r = 0;
            foreach (string field in schedulelist)
            {
                Door_Parameter dp = _drmngr.get_ParameterByName(field);
                if (dp != null)
                {
                    addRow(r, dp);
                    r++;
                }
            }

            //set the tokens list
            if (_drmngr.tokenList != null)
                tokenTextBox.Lines = _drmngr.tokenList.ToArray<string>();

            //set the phases
            int p = 0;
            int x = 0;
            foreach (string phase in _drmngr.AllPhases.Keys)
            {
                if (_drmngr.phaseHasDoors(_drmngr.AllPhases[phase]))
                {
                    comboBox_Phase.Items.Add(phase);
                    if (phase == setphase)
                        p = x;
                    x++;
                }
            }
            comboBox_Phase.SelectedIndex = p;
            _phase = comboBox_Phase.SelectedItem.ToString();

            setButtonStates();
		}

        private void addRow(int r, Door_Parameter dp)
        {
            if (_controller == null)
            {
                _controller = new SourceGrid.Cells.Controllers.CheckBox();
                _controller.CheckedChanged += new EventHandler(controller_CheckedChanged);
                _viewController = new SourceGrid.Cells.Controllers.SGButton();
                _viewController.Executed += new EventHandler(controller_CheckedChanged);
                //checkboxes
                _yesNoCheck = new SourceGrid.Cells.Views.CheckBox();
                _yesNoCheck.Border = DevAge.Drawing.RectangleBorder.NoBorder;
                _yesNoCheck.BackColor = Color.Yellow;
                _yesNoCheck.CheckBoxAlignment = DevAge.Drawing.ContentAlignment.MiddleLeft;
                _viewCheck = new SourceGrid.Cells.Views.CheckBox();
                _viewCheck.Border = DevAge.Drawing.RectangleBorder.NoBorder;
                _viewCheck.CheckBoxAlignment = DevAge.Drawing.ContentAlignment.MiddleLeft;
                //cells
                _yesNoCell = new SourceGrid.Cells.Views.Cell();
                _yesNoCell.Border = DevAge.Drawing.RectangleBorder.NoBorder;
                _yesNoCell.BackColor = Color.Yellow;
                _viewCell = new SourceGrid.Cells.Views.Cell();
                _viewCell.Border = DevAge.Drawing.RectangleBorder.NoBorder;
            }

            grid1.Rows.Insert(r);
            if (_drmngr.IsParameterYesNo(dp.Id))
            {
                grid1[r, 0] = new SourceGrid.Cells.CheckBox(null, true);
                grid1[r, 0].Editor.EnableEdit = false;
                grid1[r, 0].View = _yesNoCheck;
                grid1[r, 1] = new SourceGrid.Cells.Cell(dp.ToString());
                grid1[r, 1].Tag = dp.Id;
                grid1[r, 1].View = _yesNoCell;
            }
            else
            {
                grid1[r, 0] = new SourceGrid.Cells.CheckBox(null, _drmngr.IsParameterTokenized(dp.Id));
                grid1[r, 0].View = _viewCheck;
                grid1[r, 1] = new SourceGrid.Cells.Cell(dp.ToString());
                grid1[r, 1].Tag = dp.Id;
                grid1[r, 1].View = _viewCell;
            }
            grid1[r, 0].AddController(_controller);
            grid1[r, 1].AddController(_viewController);
        }

		private void setButtonStates()
		{
			if (grid1.Rows.Count > 0)
			{
                if (!grid1.Selection.IsEmpty())
					RemoveButton.Enabled = true;
				OKbutton.Enabled = true;
				RemoveAllButton.Enabled = true;
			}
			else
			{
				OKbutton.Enabled = false;
                if (!grid1.Selection.IsEmpty())
					RemoveButton.Enabled = false;
				RemoveAllButton.Enabled = false;
			}

			if (listBox1.Items.Count > 0)
			{
				if(listBox1.SelectedItem != null)
					AddButton.Enabled = true;
				AddAllButton.Enabled = true;
			}
			else
			{
				if(listBox1.SelectedItem != null)
					AddButton.Enabled = false;
				AddAllButton.Enabled = false;
			}
		}

		private void OKbutton_Click(object sender, EventArgs e)
		{
            bool hasTokenParams = false;
            _userList = new Dictionary<int, bool>(grid1.Rows.Count);

            for (int x = 0; x < grid1.Rows.Count; x++)
            {
                SourceGrid.Cells.CheckBox cb = grid1[x, 0] as SourceGrid.Cells.CheckBox;
                SourceGrid.Cells.Cell cl = grid1[x, 1] as SourceGrid.Cells.Cell;

                if ((bool)cb.Checked)
                {
                    hasTokenParams = true;
                    _userList.Add((int)cl.Tag, true);
                }
                else
                    _userList.Add((int)cl.Tag, false);
            }
            _phase = comboBox_Phase.SelectedItem.ToString();

			if (hasTokenParams && tokenTextBox.Lines.Length == 0)
				Autodesk.Revit.UI.TaskDialog.Show("Knock Knock", "You have specified certain fields are tokenized but have not provided a list of allowable tokens.");
			else
			{
                Properties.Settings.Default.Save();
				this.Close();
			}
		}

		private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (listBox1.SelectedItems.Count != 0)
				AddButton.Enabled = true;
		}

        private void controller_CheckedChanged(object sender, EventArgs e)
        {
            SourceGrid.Position pos = grid1.Selection.ActivePosition;
            SourceGrid.Cells.CheckBox c = grid1[pos.Row, 0] as SourceGrid.Cells.CheckBox;
            c.Value = !((bool)c.Checked);
            grid1.Selection.Focus(new SourceGrid.Position(pos.Row, 0), true);
        }

		private void AddButton_Click(object sender, EventArgs e)
		{

            List<Door_Parameter> list = new List<Door_Parameter>();
            foreach (Door_Parameter dp in listBox1.SelectedItems)
            {
                list.Add(dp);
            }

			foreach (Door_Parameter dp in listBox1.SelectedItems)
			{
                int x = grid1.RowsCount;
				addRow(x, dp);
			}

            foreach (Door_Parameter dp in list)
            {
                listBox1.Items.Remove(dp);
            }

			setButtonStates();
			AddButton.Enabled = false;
		}

		private void RemoveButton_Click(object sender, EventArgs e)
		{
            if (!grid1.Selection.IsEmpty())
            {
                foreach (int r in grid1.Selection.GetSelectionRegion().GetRowsIndex())
                {
                    SourceGrid.Cells.CheckBox cb = grid1[r, 0] as SourceGrid.Cells.CheckBox;
                    SourceGrid.Cells.Cell c = grid1[r, 1] as SourceGrid.Cells.Cell;
                    Door_Parameter dp = _drmngr.get_ParameterByName((string)c.Value);
                    listBox1.Items.Add(dp);
                    cb.Tag = "Delete";
                }

                for (int x=0; x < grid1.RowsCount; x++)
                {
                    SourceGrid.Cells.CheckBox c = grid1[x, 0] as SourceGrid.Cells.CheckBox;
                    if ((string)c.Tag == "Delete")
                    {
                        grid1.Rows.Remove(x);
                        x--;
                    }
                }

                setButtonStates();
                RemoveButton.Enabled = false;
                UpButton.Enabled = false;
                DownButton.Enabled = false;
            }
		}

		private void UpButton_Click(object sender, EventArgs e)
		{
            if (grid1.Selection.GetSelectionRegion().GetRowsIndex().Length == 1)
            {
                int r = grid1.Selection.GetSelectionRegion().GetRowsIndex()[0];
                if (r != 0)
                {
                    grid1.Rows.Move(r, r - 1);
                    grid1.Selection.ResetSelection(true);
                    grid1.Selection.SelectCell(new SourceGrid.Position(r - 1, 0), true);
                    if (r - 1 <= 0)
                        UpButton.Enabled = false;
                    DownButton.Enabled = true;
                }
            }
		}

		private void DownButton_Click(object sender, EventArgs e)
		{
            if (grid1.Selection.GetSelectionRegion().GetRowsIndex().Length == 1)
            {
                int r = grid1.Selection.GetSelectionRegion().GetRowsIndex()[0];
                if (r != grid1.Rows.Count - 1)
                {
                    grid1.Rows.Move(r, r + 1);
                    grid1.Selection.ResetSelection(true);
                    grid1.Selection.SelectCell(new SourceGrid.Position(r + 1, 0), true);
                    if (r + 2 >= grid1.Rows.Count)
                        DownButton.Enabled = false;
                    UpButton.Enabled = true;
                }
            }
		}

		private void CancelButton_Click(object sender, EventArgs e)
		{
			this.Close();
		}

		private void RemoveAllButton_Click(object sender, EventArgs e)
		{
            while(grid1.RowsCount != 0)
            {
                SourceGrid.Cells.Cell c = grid1[0, 1] as SourceGrid.Cells.Cell;
                Door_Parameter dp = _drmngr.get_ParameterByName((string)c.Value);
                listBox1.Items.Add(dp);
                grid1.Rows.Remove(0);
            }

            setButtonStates();
            RemoveButton.Enabled = false;
		}

		private void AddAllButton_Click(object sender, EventArgs e)
		{
            int x = grid1.RowsCount;
            foreach (Door_Parameter dp in listBox1.Items)
            {
                addRow(x, dp);
                x++;
            }
            listBox1.Items.Clear();

            setButtonStates();
            AddAllButton.Enabled = false;
            AddButton.Enabled = false;
		}

		private void CheckAllButton_Click(object sender, EventArgs e)
		{
            for (int x = 0; x < grid1.RowsCount; x++)
            {
                SourceGrid.Cells.CheckBox c = grid1[x,0] as SourceGrid.Cells.CheckBox;
                c.Value = true;
            }
		}

        private void UncheckAllButton_Click(object sender, EventArgs e)
        {
            for (int x = 0; x < grid1.RowsCount; x++)
            {
                SourceGrid.Cells.CheckBox c = grid1[x, 0] as SourceGrid.Cells.CheckBox;
                c.Value = false;
            }
        }
		
		private void HelpButtonClick(object sender, EventArgs e)
		{
            KnockKnockApp.kk_help.Launch();
		}

        private void Parameter_Chooser_Form_Load(object sender, EventArgs e)
        {
            grid1.SelectionMode = SourceGrid.GridSelectionMode.Row;
            grid1.FixedColumns = 1;
            grid1.Columns[1].AutoSizeMode = SourceGrid.AutoSizeMode.EnableStretch;
            grid1.Columns[1].Width = grid1.Width - grid1.Columns[0].Width;
            grid1.HorizontalScroll.Enabled = false;
            grid1.Height = splitContainer1.Panel1.Height - grid1.Location.Y;
            grid1.Font = Properties.Settings.Default.HeaderFont;
            grid1.AutoSizeCells();
            splitContainer1.SplitterDistance = splitContainer1.Size.Height - 146;
            listBox1.Height = splitContainer1.Panel1.Height - listBox1.Location.Y;
            OKbutton.Focus();
        }

        private void Parameter_Chooser_Form_Resize(object sender, EventArgs e)
        {
            splitContainer1.SplitterDistance = splitContainer1.Size.Height - 146;
            listBox1.Height = splitContainer1.Panel1.Height - listBox1.Location.Y;
            grid1.Height = splitContainer1.Panel1.Height - grid1.Location.Y;
        }

        private void grid1_Click(object sender, EventArgs e)
        {
            base.OnClick(e);

            setButtonStates();

            if (grid1.Selection.ActivePosition != SourceGrid.Position.Empty)
            {
                RemoveButton.Enabled = true;
                if (grid1.Selection.ActivePosition.Row == grid1.RowsCount - 1)
                {
                    UpButton.Enabled = true;
                    DownButton.Enabled = false;
                }
                else
                    DownButton.Enabled = true;
                if (grid1.Selection.ActivePosition.Row == 0)
                {
                    UpButton.Enabled = false;
                    DownButton.Enabled = true;
                }
                else
                    UpButton.Enabled = true;
            }

            if (grid1.Selection.GetSelectionRegion().GetRowsIndex().Length > 1)
            {
                DownButton.Enabled = false;
                UpButton.Enabled = false;
            }
        }

        private void headerFontButton_Click(object sender, EventArgs e)
        {
            try
            {
                fontDialog.Font = Properties.Settings.Default.HeaderFont;
                fontDialog.AllowVerticalFonts = false;
                fontDialog.AllowVectorFonts = false;
                if (fontDialog.ShowDialog() != System.Windows.Forms.DialogResult.Cancel)
                {
                    Properties.Settings.Default.HeaderFont = fontDialog.Font;
                    grid1.Font = Properties.Settings.Default.HeaderFont;
                    grid1.Refresh();
                }
            }
            catch (ArgumentException)
            {
                Autodesk.Revit.UI.TaskDialog.Show("Invalid Font", "Non-True Type fonts cannot be used. Please try another.");
            }
        }

        private void bodyFontButton_Click(object sender, EventArgs e)
        {
            try
            {
                fontDialog.Font = Properties.Settings.Default.ScheduleFont;
                fontDialog.AllowVerticalFonts = false;
                fontDialog.AllowVectorFonts = false;
                if (fontDialog.ShowDialog() != System.Windows.Forms.DialogResult.Cancel)
                    Properties.Settings.Default.ScheduleFont = fontDialog.Font;
            }
            catch (ArgumentException)
            {
                Autodesk.Revit.UI.TaskDialog.Show("Invalid Font", "Non-True Type fonts cannot be used. Please try another.");
            }
        }

        private void charMapButton_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process myProcess = new System.Diagnostics.Process();
            myProcess.StartInfo.FileName = "charmap.exe";
            myProcess.StartInfo.UseShellExecute = true;
            myProcess.StartInfo.RedirectStandardOutput = false;
            myProcess.Start();
            myProcess.Dispose();
        }
	}
}
