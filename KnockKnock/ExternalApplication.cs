using System;
using System.IO;
using System.Windows.Media.Imaging;
using log4net;
using log4net.Config;

using Autodesk.Revit.UI;

namespace KnockKnock
{
    [Autodesk.Revit.Attributes.Transaction(Autodesk.Revit.Attributes.TransactionMode.Manual)]
    [Autodesk.Revit.Attributes.Journaling(Autodesk.Revit.Attributes.JournalingMode.NoCommandData)]
    public partial class KnockKnockApp : IExternalApplication
    {
        internal static ContextualHelp kk_help = null;
        private static readonly ILog _log = LogManager.GetLogger(typeof(KnockKnockApp));

        private void CreateRibbonPanel(UIControlledApplication application)
        {
            // This method is used to create the ribbon panel.
            // which contains the controlled application.

            string AddinPath = Properties.Settings.Default.AddinPath;
            string DLLPath = AddinPath + @"\KnockKnock.dll";
            RibbonPanel pkhlPanel = null;

            //see if pkh Lineworks panel is already created by another of my fabulous apps
            System.Collections.Generic.List<RibbonPanel> panels = application.GetRibbonPanels();
            foreach (RibbonPanel rp in panels)
            {
                if (rp.Name == "pkh Lineworks")
                {
                    pkhlPanel = rp;
                    break;
                }
            }
            if(pkhlPanel == null)
                pkhlPanel = application.CreateRibbonPanel("pkh Lineworks");

            // Create a Button for KNOCK KNOCK
            PushButton kk_Button = pkhlPanel.AddItem(new PushButtonData("kkButton", "Knock Knock", DLLPath, "KnockKnock.kk_command")) as PushButton;
            kk_Button.Image = NewBitmapImage(this.GetType().Assembly, "knockknock16x16.png");
            kk_Button.LargeImage = NewBitmapImage(this.GetType().Assembly, "knockknock.png");
            kk_Button.ToolTip = "Opens the Knock Knock command";
            kk_Button.Visible = true;
            kk_Button.AvailabilityClassName = "KnockKnock.command_AvailableCheck";
            kk_Button.SetContextualHelp(kk_help);

            //delete schema
            PushButton ds_Button = pkhlPanel.AddItem(new PushButtonData("dsButton", "Delete Settings", DLLPath, "KnockKnock.delete_schema")) as PushButton;
            ds_Button.Image = NewBitmapImage(this.GetType().Assembly, "delete_schema16x16.png");
            ds_Button.LargeImage = NewBitmapImage(this.GetType().Assembly, "delete_schema.png");
            ds_Button.ToolTip = "Deletes the settings stored by the Knock Knock command";
            ds_Button.Visible = true;
            ds_Button.AvailabilityClassName = "KnockKnock.command_AvailableCheck";
            ds_Button.SetContextualHelp(kk_help);

            // Create a slide out
            pkhlPanel.AddSlideOut();

            PushButtonData about_Button = new PushButtonData("aboutButton", "About", DLLPath, "KnockKnock.aboutbox");
            about_Button.Image = NewBitmapImage(this.GetType().Assembly, "about16x16.png");
            about_Button.LargeImage = NewBitmapImage(this.GetType().Assembly, "about.png");
            about_Button.ToolTip = "All about Knock Knock.";
            about_Button.AvailabilityClassName = "KnockKnock.subcommand_AvailableCheck";

            PushButtonData help_Button = new PushButtonData("helpButton", "Help", DLLPath, "KnockKnock.kk_help");
            help_Button.Image = NewBitmapImage(this.GetType().Assembly, "kk_help16x16.png");
            help_Button.LargeImage = NewBitmapImage(this.GetType().Assembly, "kk_help.png");
            help_Button.ToolTip = "Help about Knock Knock.";
            help_Button.AvailabilityClassName = "KnockKnock.subcommand_AvailableCheck";

            pkhlPanel.AddStackedItems(about_Button, help_Button);
        }

        /// <summary>
        /// Load a new icon bitmap from embedded resources.
        /// For the BitmapImage, make sure you reference WindowsBase and PresentationCore, and import the System.Windows.Media.Imaging namespace.
        /// Drag images into Resources folder in solution explorer and set build action to "Embedded Resource"
        /// </summary>
        private BitmapImage NewBitmapImage(System.Reflection.Assembly a, string imageName)
        {
            Stream s = a.GetManifestResourceStream("KnockKnock.Resources." + imageName);
            BitmapImage img = new BitmapImage();

            img.BeginInit();
            img.StreamSource = s;
            img.EndInit();

            return img;
        }

        #region Event Handlers
        public Autodesk.Revit.UI.Result OnShutdown(UIControlledApplication application)
        {
            return Autodesk.Revit.UI.Result.Succeeded;
        }

        public Autodesk.Revit.UI.Result OnStartup(UIControlledApplication application)
        {
            string s = this.GetType().Assembly.Location;
            int x = s.IndexOf(@"\knockknock.dll", StringComparison.CurrentCultureIgnoreCase);
            s = s.Substring(0, x);
            Properties.Settings.Default.AddinPath = s;

            XmlConfigurator.Configure(new FileInfo(Properties.Settings.Default.AddinPath + "\\knockknock.log4net.config"));
            _log.InfoFormat("Running version: {0}", this.GetType().Assembly.GetName().Version.ToString());
            _log.InfoFormat("Found myself at: {0}", Properties.Settings.Default.AddinPath);

            kk_help = new ContextualHelp(
                ContextualHelpType.ChmFile,
                Path.Combine(
                    Directory.GetParent(Properties.Settings.Default.AddinPath).ToString(), //contents directory
                    "kk_help.chm"));

            CreateRibbonPanel(application);
            return Autodesk.Revit.UI.Result.Succeeded;
        }
        #endregion
    }
}