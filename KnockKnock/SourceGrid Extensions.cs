///
/// Custom surcegrid views, controllers and editors used by Knock Knock
using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Collections.Generic;
using System.Windows.Forms;

namespace SourceGrid.Cells.Editors
{
    /// <summary>
    /// An editor that use a TextBoxTyped for editing support.
    /// </summary>
    [System.ComponentModel.ToolboxItem(false)]
    public class PoppupBox : EditorControlBase
    {
        #region Constructor
        /// <summary>
        /// Construct a Model. Based on the Type specified the constructor populate AllowNull, DefaultValue, TypeConverter, StandardValues, StandardValueExclusive
        /// </summary>
        /// <param name="p_Type">The type of this model</param>
        public PoppupBox(Type p_Type)
            : base(p_Type)
        {
        }
        #endregion

        #region Edit Control
        /// <summary>
        /// Create the editor control
        /// </summary>
        /// <returns></returns>
        protected override Control CreateControl()
        {
            DevAge.Windows.Forms.DevAgeTextBox editor = new DevAge.Windows.Forms.DevAgeTextBox();
            editor.BorderStyle = BorderStyle.None;
            editor.AutoSize = false;
            editor.Validator = this;
            return editor;
        }

        /// <summary>
        /// Gets the control used for editing the cell.
        /// </summary>
        public new DevAge.Windows.Forms.DevAgeTextBox Control
        {
            get
            {
                return (DevAge.Windows.Forms.DevAgeTextBox)base.Control;
            }
        }
        #endregion

        /// <summary>
        /// This method is called just before the edit start. You can use this method to customize the editor with the cell informations.
        /// </summary>
        /// <param name="cellContext"></param>
        /// <param name="editorControl"></param>
        protected override void OnStartingEdit(CellContext cellContext, Control editorControl)
        {
            base.OnStartingEdit(cellContext, editorControl);

            DevAge.Windows.Forms.DevAgeTextBox l_TxtBox = (DevAge.Windows.Forms.DevAgeTextBox)editorControl;
            l_TxtBox.Multiline = true;
            l_TxtBox.WordWrap = true;
            l_TxtBox.Size = new Size(400, 200);
            l_TxtBox.TextAlign = DevAge.Windows.Forms.Utilities.ContentToHorizontalAlignment(cellContext.Cell.View.TextAlignment);

            //to set the scroll of the textbox to the initial position (otherwise the textbox use the previous scroll position)
            l_TxtBox.SelectionStart = 0;
            l_TxtBox.SelectionLength = 0;
        }

        /// <summary>
        /// Set the specified value in the current editor control.
        /// </summary>
        /// <param name="editValue"></param>
        public override void SetEditValue(object editValue)
        {
            Control.Value = editValue;
            Control.SelectAll();
        }

        /// <summary>
        /// Returns the value inserted with the current editor control
        /// </summary>
        /// <returns></returns>
        public override object GetEditedValue()
        {
            return Control.Value;
        }

        protected override void OnSendCharToEditor(char key)
        {
            Control.Text = key.ToString();
            if (Control.Text != null)
                Control.SelectionStart = Control.Text.Length;
        }
    }
}

namespace SourceGrid.Cells.Controllers
{
	/// <summary>
	/// Description of SourceGrid_Controllers.
	/// </summary>
	public class SGButton : Button
	{
		public SGButton()
		{
		}
		
		private MouseButtons mLastButton = MouseButtons.None;
		
		public event MouseEventHandler MouseUp;
		public override void OnMouseUp(CellContext sender, MouseEventArgs e)
        {
//			base.OnMouseUp(sender, e);
			
			if(MouseUp != null)
				MouseUp(sender, e);

            mLastButton = e.Button;
        }
	}
}

namespace SourceGrid.Cells.Views
{
    public class ParameterView : Cell
    {
        public ParameterView()
        {
            this.Font = KnockKnock.Properties.Settings.Default.ScheduleFont;
            this.TextAlignment = DevAge.Drawing.ContentAlignment.MiddleCenter;
            DevAge.Drawing.BorderLine border = new DevAge.Drawing.BorderLine(Color.Black, 1);
            DevAge.Drawing.RectangleBorder cellBorder = new DevAge.Drawing.RectangleBorder(border, border);
            this.Border = cellBorder;
        }
    }

    public class ChangedParameterView : Cell
    {
        public ChangedParameterView()
        {
            this.Font = KnockKnock.Properties.Settings.Default.ScheduleFont;
            this.BackColor = KnockKnock.Properties.Settings.Default.ChangedColor;
            this.TextAlignment = DevAge.Drawing.ContentAlignment.MiddleCenter;
            DevAge.Drawing.BorderLine border = new DevAge.Drawing.BorderLine(Color.Black, 1);
            DevAge.Drawing.RectangleBorder cellBorder = new DevAge.Drawing.RectangleBorder(border, border);
            this.Border = cellBorder;
        }
    }

    public class ReadOnlyParameterView : Cell
    {
        public ReadOnlyParameterView()
        {
            this.Font = KnockKnock.Properties.Settings.Default.ScheduleFont;
            this.BackColor = KnockKnock.Properties.Settings.Default.ReadOnlyColor;
            this.TextAlignment = DevAge.Drawing.ContentAlignment.MiddleCenter;
            DevAge.Drawing.BorderLine border = new DevAge.Drawing.BorderLine(Color.Black, 1);
            DevAge.Drawing.RectangleBorder cellBorder = new DevAge.Drawing.RectangleBorder(border, border);
            this.Border = cellBorder;
        }
    }

    public class CheckedParameterView : Cell
    {
        public CheckedParameterView()
        {
            this.Font = KnockKnock.Properties.Settings.Default.ScheduleFont;
            this.BackColor = KnockKnock.Properties.Settings.Default.CheckedColor;
            this.TextAlignment = DevAge.Drawing.ContentAlignment.MiddleCenter;
            DevAge.Drawing.BorderLine border = new DevAge.Drawing.BorderLine(Color.Black, 1);
            DevAge.Drawing.RectangleBorder cellBorder = new DevAge.Drawing.RectangleBorder(border, border);
            this.Border = cellBorder;
        }
    }

    public class CheckedCommentView : Cell
    {
        public CheckedCommentView()
        {
            this.Font = KnockKnock.Properties.Settings.Default.ScheduleFont;
            this.BackColor = KnockKnock.Properties.Settings.Default.CheckedColor;
            this.TextAlignment = DevAge.Drawing.ContentAlignment.TopLeft;
            DevAge.Drawing.BorderLine border = new DevAge.Drawing.BorderLine(Color.Black, 1);
            DevAge.Drawing.RectangleBorder cellBorder = new DevAge.Drawing.RectangleBorder(border, border);
            this.Border = cellBorder;
        }
    }

    public class CommentView : Cell
    {
        public CommentView()
        {
            this.Font = KnockKnock.Properties.Settings.Default.ScheduleFont;
            this.TextAlignment = DevAge.Drawing.ContentAlignment.TopLeft;
            DevAge.Drawing.BorderLine border = new DevAge.Drawing.BorderLine(Color.Black, 1);
            DevAge.Drawing.RectangleBorder cellBorder = new DevAge.Drawing.RectangleBorder(border, border);
            this.Border = cellBorder;
        }
    }

    public class ChangedCommentView : Cell
    {
        public ChangedCommentView()
        {
            this.Font = KnockKnock.Properties.Settings.Default.ScheduleFont;
            this.TextAlignment = DevAge.Drawing.ContentAlignment.TopLeft;
            this.BackColor = KnockKnock.Properties.Settings.Default.ChangedColor;
            DevAge.Drawing.BorderLine border = new DevAge.Drawing.BorderLine(Color.Black, 1);
            DevAge.Drawing.RectangleBorder cellBorder = new DevAge.Drawing.RectangleBorder(border, border);
            this.Border = cellBorder;
        }
    }

    public class ReadOnlyCommentView : Cell
    {
        public ReadOnlyCommentView()
        {
            this.Font = KnockKnock.Properties.Settings.Default.ScheduleFont;
            this.BackColor = KnockKnock.Properties.Settings.Default.ReadOnlyColor;
            this.TextAlignment = DevAge.Drawing.ContentAlignment.TopLeft;
            DevAge.Drawing.BorderLine border = new DevAge.Drawing.BorderLine(Color.Black, 1);
            DevAge.Drawing.RectangleBorder cellBorder = new DevAge.Drawing.RectangleBorder(border, border);
            this.Border = cellBorder;
        }
    }


    public class VerticalColumnHeader : Cell
    {
        public VerticalColumnHeader(bool b)
        {
            ElementText = new RotatedText(180);
            if (b)
                this.BackColor = Color.LightGray;
            DevAge.Drawing.BorderLine border = new DevAge.Drawing.BorderLine(Color.Black, 1);
            DevAge.Drawing.RectangleBorder cellBorder = new DevAge.Drawing.RectangleBorder(border, border);
            this.Border = cellBorder;
            this.Font = KnockKnock.Properties.Settings.Default.HeaderFont;
        }

        private System.Drawing.StringFormatFlags mFormatFlags = StringFormatFlags.DirectionVertical | StringFormatFlags.NoWrap;
        public System.Drawing.StringFormatFlags FormatFlags
        {
            get { return mFormatFlags; }
            set { mFormatFlags = value; }
        }

        protected override void PrepareVisualElementText(SourceGrid.CellContext context)
        {
            base.PrepareVisualElementText(context);

            ((DevAge.Drawing.VisualElements.TextGDI)ElementText).StringFormat.FormatFlags = FormatFlags;
        }

        protected override SizeF OnMeasureContent(DevAge.Drawing.MeasureHelper measure, SizeF maxSize)
        {
            SizeF result = base.OnMeasureContent(measure, maxSize);
            return new SizeF(result.Width + 1, result.Height + 1);
        }
    }

    public class RotatedText : DevAge.Drawing.VisualElements.TextGDI
    {
        public RotatedText(float angle)
        {
            this.Font = KnockKnock.Properties.Settings.Default.HeaderFont;
            Angle = angle;
        }

        public float Angle = 0;

        protected override void OnDraw(DevAge.Drawing.GraphicsCache graphics, RectangleF area)
        {
            System.Drawing.Drawing2D.GraphicsState state = graphics.Graphics.Save();
            try
            {
                float width2 = area.Width / 2;
                float height2 = area.Height / 2;

                //For a better drawing use the clear type rendering
                graphics.Graphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;

                //Move the origin to the center of the cell (for a more easy rotation)
                graphics.Graphics.TranslateTransform(area.X + width2, area.Y + height2);

                graphics.Graphics.RotateTransform(Angle);

                StringFormat.Alignment = StringAlignment.Near;
                StringFormat.LineAlignment = StringAlignment.Center;
                graphics.Graphics.DrawString(Value, Font, graphics.BrushsCache.GetBrush(ForeColor), 0, -height2, StringFormat);
            }
            finally
            {
                graphics.Graphics.Restore(state);
            }
        }
    }
}