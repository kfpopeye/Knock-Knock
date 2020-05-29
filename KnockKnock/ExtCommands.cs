using System;
using System.Collections.Generic;
using System.Diagnostics;

using log4net;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using Autodesk.Revit.DB.ExtensibleStorage;
using Autodesk.Revit.DB.Events;
using Autodesk.Revit.UI.Events;

namespace KnockKnock
{
    [Autodesk.Revit.Attributes.Transaction(Autodesk.Revit.Attributes.TransactionMode.Manual)]
    public class kk_command : IExternalCommand
    {
        private readonly ILog _log = LogManager.GetLogger(typeof(kk_command));
        public Autodesk.Revit.UI.Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
#if !DEBUG
            if (!UserIsEntitled(commandData))
                return Result.Failed;
#endif

            try
            {
                Door_Manager doormngr = null;
                ElementId viewID = null;
                UIDocument uiDoc = commandData.Application.ActiveUIDocument;
                Document document = commandData.Application.ActiveUIDocument.Document;
                ProjectInfo projinfo = document.ProjectInformation;

                if (document.IsWorkshared)
                {
                    Autodesk.Revit.UI.TaskDialog td = new TaskDialog("Knock Knock");
                    td.MainIcon = Autodesk.Revit.UI.TaskDialogIcon.TaskDialogIconWarning;
                    td.MainInstruction = "This project has work sharing enabled.";

                    Parameter p = projinfo.get_Parameter(BuiltInParameter.EDITED_BY);
                    if (p.HasValue && p.AsString() != document.Application.Username)
                    {
                        td.MainContent = "The Project Information is currently owned by " + p.AsString() + " and cannot be written to. This does NOT affect changes made to the door schedule however setup information will not be saved. Also only doors currently editable by you can be changed.";
                    }
                    else
                    {
                        td.MainContent = "Only doors currently editable by you can be changed.";
                    }
                    td.Show();

                    td.AddCommandLink(TaskDialogCommandLinkId.CommandLink1, "Continue");
                    td.AddCommandLink(TaskDialogCommandLinkId.CommandLink2, "Exit");
                    Autodesk.Revit.UI.TaskDialogResult res = td.Show();

                    if (res == TaskDialogResult.CommandLink2)
                    {
                        return Result.Cancelled;
                    }
                }

                doormngr = new Door_Manager(uiDoc);
                if (doormngr.DoorCount == 0)
                {
                    TaskDialog.Show("Knock Knock", "No door instances were found in the project.");
                    return Result.Cancelled;
                }

                //try to get the door hardware schema
                Schema_Handler sh = new Schema_Handler();
                Schema theschema = sh.getSchema();
                Entity ent = projinfo.GetEntity(theschema);
                if (ent.IsValid()) //if schema exists use it
                {
                    doormngr.set_TokenParameterList(ent.Get<IDictionary<string, bool>>(theschema.GetField(Schema_Handler.C_Hardware)));
                    doormngr.tokenList = ent.Get<IList<string>>(theschema.GetField(Schema_Handler.C_Tokens));
                    doormngr.set_ParameterOrder(ent.Get<IList<string>>(theschema.GetField(Schema_Handler.C_HardwareOrder)));
                    Phase p = document.GetElement(new ElementId(ent.Get<int>(theschema.GetField(Schema_Handler.C_Phase)))) as Phase;
                    if (p != null)
                        doormngr.Phase(p.Name);
                    //get and set fonts
                    string font = ent.Get<string>(theschema.GetField(Schema_Handler.C_HdrFont));
                    float size = ent.Get<float>(theschema.GetField(Schema_Handler.C_HdrFontSize), DisplayUnitType.DUT_CUSTOM);
                    Properties.Settings.Default.HeaderFont = new System.Drawing.Font(font, size, System.Drawing.GraphicsUnit.Inch);
                    font = ent.Get<string>(theschema.GetField(Schema_Handler.C_BdyFont));
                    size = ent.Get<float>(theschema.GetField(Schema_Handler.C_BdyFontSize), DisplayUnitType.DUT_CUSTOM);
                    Properties.Settings.Default.ScheduleFont = new System.Drawing.Font(font, size, System.Drawing.GraphicsUnit.Inch);

                    //let user select which parameters (fields) they want to work on
                    Parameter_Chooser_Form pcf = new Parameter_Chooser_Form(doormngr);
                    System.Diagnostics.Process process = System.Diagnostics.Process.GetCurrentProcess();
                    IntPtr h = process.MainWindowHandle;
                    if (pcf.ShowDialog(new pkhCommon.Windows.WindowHandle(h)) == System.Windows.Forms.DialogResult.Cancel)
                        return Result.Cancelled;
                    doormngr.Phase(pcf.phase);
                    doormngr.tokenizedParameters = pcf.userList;
                    doormngr.tokenList = pcf.tokenList;
                    int[] ar = new int[pcf.userList.Count];
                    pcf.userList.Keys.CopyTo(ar, 0);
                    doormngr.parameterOrder = ar;
                    pcf.Dispose();
                }
                else if ((viewID = CheckForDoorSchedules(document)) != null) //if no schema and a door schedule exists
                {
                    //get phase
                    ViewSchedule vs = document.GetElement(viewID) as ViewSchedule;
                    Parameter p = vs.get_Parameter(BuiltInParameter.VIEW_PHASE);
                    string phase = document.GetElement(p.AsElementId()).Name;

                    //get fonts
                    Element e = document.GetElement(vs.BodyTextTypeId);
                    string font = e.get_Parameter(BuiltInParameter.TEXT_FONT).AsString();
                    float size = (float)e.get_Parameter(BuiltInParameter.TEXT_SIZE).AsDouble(); //in feet
                    Properties.Settings.Default.ScheduleFont = new System.Drawing.Font(font, size * 12, System.Drawing.GraphicsUnit.Inch);
                    e = document.GetElement(vs.HeaderTextTypeId);
                    font = e.get_Parameter(BuiltInParameter.TEXT_FONT).AsString();
                    size = (float)e.get_Parameter(BuiltInParameter.TEXT_SIZE).AsDouble(); //in feet
                    Properties.Settings.Default.HeaderFont = new System.Drawing.Font(font, size * 12, System.Drawing.GraphicsUnit.Inch);
                    doormngr.ScanForTokens();

                    //let user select which parameters (fields) they want to work on
                    Parameter_Chooser_Form pcf = new Parameter_Chooser_Form(doormngr, GetScheduleFields(document, viewID), phase);
                    System.Diagnostics.Process process = System.Diagnostics.Process.GetCurrentProcess();
                    IntPtr h = process.MainWindowHandle;
                    if (pcf.ShowDialog(new pkhCommon.Windows.WindowHandle(h)) == System.Windows.Forms.DialogResult.Cancel)
                        return Result.Cancelled;
                    doormngr.Phase(pcf.phase);
                    doormngr.tokenizedParameters = pcf.userList;
                    doormngr.tokenList = pcf.tokenList;
                    int[] ar = new int[pcf.userList.Count];
                    pcf.userList.Keys.CopyTo(ar, 0);
                    doormngr.parameterOrder = ar;
                    pcf.Dispose();
                }
                else //no schema, no schedules, no service
                {
                    Autodesk.Revit.UI.TaskDialog.Show("A problem has occured.", "This project is has no door schedules to work on.");
                    return Result.Cancelled;
                }

                doormngr.setToFromRooms();

                //display editor form
                //Main_Form mainform = new Main_Form(doormngr, uiDoc);
                //if (mainform.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                MainWindow main = new MainWindow(doormngr);
                System.Windows.Interop.WindowInteropHelper x = new System.Windows.Interop.WindowInteropHelper(main);
                x.Owner = Process.GetCurrentProcess().MainWindowHandle;
                if((bool)main.ShowDialog())
                {
                    Transaction trans = new Transaction(document);
                    trans.Start("Knock Knock");

                    SaveToSchema(doormngr, theschema, projinfo);

                    if (doormngr.UpdateDoors(document)) //try to update the door instances
                    {
                        TaskDialog.Show("Knock Knock", "Successfully updated doors.");
                        trans.Commit();
                        return Result.Succeeded;
                    }
                    else
                    {
                        TaskDialog.Show("Knock Knock", "Was not able to update doors.");
                        trans.RollBack();
                        return Result.Failed;
                    }
                }

                return Result.Cancelled;
            }
            catch (Exception err)
            {
                _log.Error(err);
                Autodesk.Revit.UI.TaskDialog td = new TaskDialog("Unexpected Error");
                td.MainInstruction = "Knock Knock has encountered an error and cannot complete.";
                td.MainContent = "The developer is no longer updating this app.";
                td.ExpandedContent = err.ToString();
                //td.AddCommandLink(TaskDialogCommandLinkId.CommandLink1, "Send bug report.");
                td.MainIcon = TaskDialogIcon.TaskDialogIconWarning;
                TaskDialogResult tdr = td.Show();

                //if (tdr == TaskDialogResult.CommandLink1)
                //{
                //    pkhCommon.Email.SendErrorMessage(commandData.Application.Application.VersionName, commandData.Application.Application.VersionBuild, err, this.GetType().Assembly.GetName());
                //}

                return Result.Failed;
            }
        }

        private bool UserIsEntitled(ExternalCommandData commandData)
        {
            return true;
            //if (Properties.Settings.Default.EntCheck.AddDays(7) > System.DateTime.Today)
            //    return true;

            //string _baseApiUrl = @"https://apps.exchange.autodesk.com/";
            //string _appId = Constants.APP_STORE_ID;

            //UIApplication uiApp = commandData.Application;
            //Autodesk.Revit.ApplicationServices.Application rvtApp = commandData.Application.Application;

            ////Check to see if the user is logged in.
            //if (!Autodesk.Revit.ApplicationServices.Application.IsLoggedIn)
            //{
            //    TaskDialog td = new TaskDialog(Constants.GROUP_NAME);
            //    td.MainInstruction = "Please login to Autodesk 360 first.";
            //    td.MainContent = "This application must check if you are authorized to use it. Login to Autodesk 360 using the same account you used to purchase this app. An internet connection is required.";
            //    td.Show();
            //    return false;
            //}

            ////Get the user id, and check entitlement
            //string userId = rvtApp.LoginUserId;
            //bool isAuthorized = pkhCommon.EntitlementHelper.Entitlement(_appId, userId, _baseApiUrl);

            //if (!isAuthorized)
            //{
            //    TaskDialog td = new TaskDialog(Constants.GROUP_NAME);
            //    td.MainInstruction = "You are not authorized to use this app.";
            //    td.MainContent = "Make sure you login into Autodesk 360 with the same account you used to buy this app. If the app was purchased under a company account, contact your IT department to allow you access.";
            //    td.Show();
            //    return false;
            //}
            //else
            //{
            //    Properties.Settings.Default.EntCheck = System.DateTime.Today;
            //    Properties.Settings.Default.Save();
            //}

            //return isAuthorized;
        }

        /// <summary>
        /// Check a document for door schedules and prompt user if more than one exists.
        /// </summary>
        /// <param name="document">The document to check for schedules</param>
        /// <returns>The ElementID of the door schedule to use.</returns>
        private ElementId CheckForDoorSchedules(Document document)
        {
            ElementId temp_vs = null;

            FilteredElementCollector collector = new FilteredElementCollector(document);
            FilteredElementCollector views = collector.OfClass((typeof(ViewSchedule)));
            Dictionary<string, ElementId> doorschedules = new Dictionary<string, ElementId>();

            foreach (ViewSchedule vs in views)
            {
                if ((BuiltInCategory)vs.Definition.CategoryId.IntegerValue == BuiltInCategory.OST_Doors && !vs.IsTemplate && !vs.Definition.IsKeySchedule)
                {
                    doorschedules.Add(vs.Name, vs.Id);
                    temp_vs = vs.Id;
                }
            }

            if (doorschedules.Count == 1)
                return temp_vs;

            if (doorschedules.Count > 1)
            {
                ScheduleChooserForm scf = new ScheduleChooserForm(doorschedules);
                scf.ShowDialog();

                if (scf.DialogResult == System.Windows.Forms.DialogResult.OK)
                    return scf.Choice;
            }

            return null;
        }

        /// <summary>
        /// Get a list of fields shown by the door schedule.
        /// </summary>
        /// <param name="document">The document containing the door schedule</param>
        /// <param name="schedule">The door schedule</param>
        /// <returns>An string array of parameters shown by the schedule</returns>
        private string[] GetScheduleFields(Document document, ElementId schedule)
        {
            string[] fields = null;

            ViewSchedule vs = document.GetElement(schedule) as ViewSchedule;

            int x = 0;
            fields = new string[vs.Definition.GetFieldCount()];
            while (x < vs.Definition.GetFieldCount())
            {
                if(!vs.Definition.GetField(x).IsCalculatedField)
                    fields[x] = vs.Definition.GetField(x).GetName();
                x++;
            }

            return fields;
        }

        private void SaveToSchema(Door_Manager drmngr, Schema schm, ProjectInfo projinfo)
        {
            //Note: this works on workset enabled files. ProjectInfo can be "owned" by API.
            Parameter p = projinfo.get_Parameter(BuiltInParameter.EDITED_BY);
            if (projinfo.Document.IsWorkshared && p.AsString() != projinfo.Document.Application.Username && p.AsString() != string.Empty)
            {
                Autodesk.Revit.UI.TaskDialog td = new TaskDialog("Knock Knock");
                td.MainInstruction = "Cannot save configuration to project.";
                td.MainContent = "The Project Information is currently owned by " + p.AsString() + " and cannot be written to. This does NOT affect changes made to the door schedule.";
                td.Show();
                return;
            }

            Entity ent = new Entity(schm);
            Field hardware = schm.GetField(Schema_Handler.C_Hardware);
            Field tokens = schm.GetField(Schema_Handler.C_Tokens);
            Field orderedlist = schm.GetField(Schema_Handler.C_HardwareOrder);
            Field phase = schm.GetField(Schema_Handler.C_Phase);
            Field headerfont = schm.GetField(Schema_Handler.C_HdrFont);
            Field headerfontsize = schm.GetField(Schema_Handler.C_HdrFontSize);
            Field bodyfont = schm.GetField(Schema_Handler.C_BdyFont);
            Field bodyfontsize = schm.GetField(Schema_Handler.C_BdyFontSize);
            ent.Set<IDictionary<string, bool>>(hardware, drmngr.get_TokenizedParameters());
            ent.Set<IList<string>>(tokens, drmngr.tokenList);
            ent.Set<IList<string>>(orderedlist, drmngr.get_ParameterOrder());
            ent.Set<int>(phase, drmngr.PhaseId());
            ent.Set<string>(headerfont, Properties.Settings.Default.HeaderFont.Name);
            ent.Set<float>(headerfontsize, Properties.Settings.Default.HeaderFont.Size, DisplayUnitType.DUT_CUSTOM);
            ent.Set<string>(bodyfont, Properties.Settings.Default.ScheduleFont.Name);
            ent.Set<float>(bodyfontsize, Properties.Settings.Default.ScheduleFont.Size, DisplayUnitType.DUT_CUSTOM);
            projinfo.SetEntity(ent);
        }
    }

    [Autodesk.Revit.Attributes.Transaction(Autodesk.Revit.Attributes.TransactionMode.Manual)]
    public class delete_schema : IExternalCommand
    {
        private readonly ILog _log = LogManager.GetLogger(typeof(delete_schema));
        public Autodesk.Revit.UI.Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            Autodesk.Revit.UI.TaskDialog td_confirm = new TaskDialog("Knock Knock");
            td_confirm.MainInstruction = "Delete data from this project?";
            td_confirm.MainContent = "This will remove data used by " + "Knock Knock" + " only. This will NOT affect project data.";
            td_confirm.AddCommandLink(TaskDialogCommandLinkId.CommandLink1, "Yes, delete the data.");
            td_confirm.AddCommandLink(TaskDialogCommandLinkId.CommandLink2, "No, don't do anything.");
            td_confirm.MainIcon = TaskDialogIcon.TaskDialogIconWarning;
            TaskDialogResult td_result = td_confirm.Show();

            if (td_result != TaskDialogResult.CommandLink1)
            {
                return Result.Cancelled;
            }

            Document document = commandData.Application.ActiveUIDocument.Document;
            ProjectInfo projinfo = document.ProjectInformation;

            Schema_Handler sh = new Schema_Handler();
            Schema theschema = sh.getSchema();

            Entity ent = projinfo.GetEntity(theschema);
            if (!ent.IsValid()) //if schema does not exist
            {
                TaskDialog.Show("Knock Knock", "There was no data to remove from this project.");
                return Result.Cancelled;
            }

            Transaction trans = new Transaction(document);
            trans.Start("Remove KK data");

            try
            {
                if (projinfo.DeleteEntity(theschema))
                {
                    trans.Commit();
                    TaskDialog.Show("Knock Knock", "Successfully removed data.");
                }
                else
                {
                    trans.RollBack();
                    TaskDialog.Show("Knock Knock", "Was NOT able to remove data.");
                }
            }
            catch (Exception err)
            {
                _log.Error(err);

                Autodesk.Revit.UI.TaskDialog td = new TaskDialog("Unexpected Error");
                td.MainInstruction = "Knock Knock has encountered an error and cannot complete.";
                td.MainContent = "The developer is no longer updating this app.";
                td.ExpandedContent = err.ToString();
                //td.AddCommandLink(TaskDialogCommandLinkId.CommandLink1, "Send bug report.");
                td.MainIcon = TaskDialogIcon.TaskDialogIconWarning;
                TaskDialogResult tdr = td.Show();

                //if (tdr == TaskDialogResult.CommandLink1)
                //{
                //    pkhCommon.Email.SendErrorMessage(commandData.Application.Application.VersionName, commandData.Application.Application.VersionBuild, err, this.GetType().Assembly.GetName());
                //}
            }

            return Result.Succeeded;
        }
    }

    [Autodesk.Revit.Attributes.Transaction(Autodesk.Revit.Attributes.TransactionMode.Manual)]
    public class aboutbox : IExternalCommand
    {
        private readonly ILog _log = LogManager.GetLogger(typeof(aboutbox));
        public Autodesk.Revit.UI.Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            try
            {
                pkhCommon.Windows.About_Box ab = new pkhCommon.Windows.About_Box(this.GetType().Assembly);
                System.Windows.Interop.WindowInteropHelper x = new System.Windows.Interop.WindowInteropHelper(ab);
                x.Owner = Process.GetCurrentProcess().MainWindowHandle;
                ab.ShowDialog();
            }
            catch (Exception err)
            {
                _log.Error(err);

                Autodesk.Revit.UI.TaskDialog td = new TaskDialog("Unexpected Error");
                td.MainInstruction = "Knock Knock has encountered an error and cannot complete.";
                td.MainContent = "The developer is no longer updating this app.";
                td.ExpandedContent = err.ToString();
                //td.AddCommandLink(TaskDialogCommandLinkId.CommandLink1, "Send bug report.");
                td.MainIcon = TaskDialogIcon.TaskDialogIconWarning;
                TaskDialogResult tdr = td.Show();

                //if (tdr == TaskDialogResult.CommandLink1)
                //{
                //    pkhCommon.Email.SendErrorMessage(commandData.Application.Application.VersionName, commandData.Application.Application.VersionBuild, err, this.GetType().Assembly.GetName());
                //}
            }

            return Result.Succeeded;
        }
    }

    [Autodesk.Revit.Attributes.Transaction(Autodesk.Revit.Attributes.TransactionMode.Manual)]
    public class kk_help : IExternalCommand
    {
        public Autodesk.Revit.UI.Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            KnockKnockApp.kk_help.Launch();
            return Result.Succeeded;
        }
    }

    public class command_AvailableCheck : IExternalCommandAvailability
    {
        public bool IsCommandAvailable(UIApplication appdata, CategorySet selectCatagories)
        {
            if (appdata.Application.Documents.Size == 0)
                return false;

            return true;
        }
    }

    public class subcommand_AvailableCheck : IExternalCommandAvailability
    {
        public bool IsCommandAvailable(UIApplication appdata, CategorySet selectCatagories)
        {
            return true;
        }
    }
}