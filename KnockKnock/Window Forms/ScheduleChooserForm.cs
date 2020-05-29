
using System;
using System.Drawing;
using System.Windows.Forms;
using System.Collections.Generic;

namespace KnockKnock
{
	/// <summary>
	/// Description of ScheduleChooserForm.
	/// </summary>
	public partial class ScheduleChooserForm : Form
	{
		public Autodesk.Revit.DB.ElementId Choice = null;
		private Dictionary<string, Autodesk.Revit.DB.ElementId> _schedules = null;
		
		public ScheduleChooserForm(Dictionary<string, Autodesk.Revit.DB.ElementId> schedules)
		{
			//
			// The InitializeComponent() call is required for Windows Forms designer support.
			//
			InitializeComponent();
			
			_schedules = schedules;
			
			foreach(string s in schedules.Keys)
			{
				listBox1.Items.Add(s);
			}
		}
		
		void Button2Click(object sender, EventArgs e)
		{
			this.DialogResult = DialogResult.OK;
			_schedules.TryGetValue(listBox1.SelectedItem.ToString(), out Choice);
			this.Close();
		}
		
		void ListBox1SelectedIndexChanged(object sender, EventArgs e)
		{
			if(listBox1.SelectedItem != null)
				buttonOK.Enabled = true;
		}
	}
}
