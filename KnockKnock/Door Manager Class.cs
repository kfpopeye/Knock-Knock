using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.ComponentModel;

using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

namespace KnockKnock
{
    /// <summary>
    /// Contains and manages information about all the doors in a project.
    /// </summary>
    public class Door_Manager
    {
        internal IList<Door_Information> _doors = null; //a list of all the doors
        private Dictionary<int, Door_Parameter> _masterParameterList = null; //a list of all the parameter names in all doors. Not all doors have the same parameters.
        /// <summary>
        /// a list of parameters the user wants to edit.
        /// </summary>
        private IDictionary<int, bool> _tokenizedList = null;
        private IList<string> _parameterTokens = null; //valid values the parameters can have
        private IList<int> _parameterOrder = null; //a list that keeps the user specified hardware order
        private IList<string> _keyParameters = null; //key parameters from key schedules
        private int _phase = -1; //the phase we want to focus on
        private string _phaseName = null;
        private IDictionary<string, int> _allPhases = null; //all phases stored in document
        private Document _document;
        private UIDocument _UIdoc;
        private System.Collections.Generic.Dictionary<string, int> _floorplans;

        public Door_Manager() { } //for design time use

        public Door_Manager(UIDocument uiDoc)
        {
            _document = uiDoc.Document;
            _UIdoc = uiDoc;

            //find any _KEY_ schedules that relate to doors and get the key parameter name(s).
            FilteredElementCollector vcollector = new FilteredElementCollector(_document);
            FilteredElementCollector views = vcollector.OfClass((typeof(ViewSchedule)));
            _keyParameters = new List<string>(views.Count());
            foreach (ViewSchedule vs in views)
            {
                if (vs.Definition.IsKeySchedule && (BuiltInCategory)vs.Definition.CategoryId.IntegerValue == BuiltInCategory.OST_Doors && !vs.IsTemplate)
                {
                    _keyParameters.Add(vs.KeyScheduleParameterName);
                }
            }

            // Find all door instances in the project by finding all elements that both belong to the
            // door category and are family instances.
            ElementClassFilter familyInstanceFilter = new ElementClassFilter(typeof(FamilyInstance));
            ElementCategoryFilter doorsCategoryfilter = new ElementCategoryFilter(BuiltInCategory.OST_Doors);
            LogicalAndFilter doorInstancesFilter = new LogicalAndFilter(familyInstanceFilter, doorsCategoryfilter);
            FilteredElementCollector collector = new FilteredElementCollector(_document);
            collector.WherePasses(doorInstancesFilter);

            if (_document.IsWorkshared)
            {
                ParameterValueProvider pvp = new ParameterValueProvider(new ElementId((int)BuiltInParameter.EDITED_BY));
                FilterStringRuleEvaluator fse = new FilterStringEquals();
                FilterRule fr = new FilterStringRule(pvp, fse, _document.Application.Username, true);
                ElementParameterFilter userfilter = new ElementParameterFilter(fr);
                collector.WherePasses(userfilter);
            }

            //filter out all doors that are in design options
            System.Collections.Generic.IList<Element> doors = collector.ContainedInDesignOption(ElementId.InvalidElementId).ToElements();

            _doors = new List<Door_Information>(doors.Count);
            foreach (Element door in doors)
            {
                Door_Information di = new Door_Information(door, _keyParameters);
                _doors.Add(di);
                if (_masterParameterList == null)
                    _masterParameterList = di.getDoorParameters();
                else
                    _masterParameterList = _masterParameterList.Union(di.getDoorParameters(), new MyEqualityComparer()).ToDictionary(x => x.Key, x => x.Value);
            }

            //collect construction phases
            PhaseArray pa = _document.Phases;
            _allPhases = new Dictionary<string, int>(pa.Size);
            foreach (Phase p in pa)
            {
                _allPhases.Add(p.Name, p.Id.IntegerValue);
            }

            //collect floor plan views
            FilteredElementCollector pcollector = new FilteredElementCollector(_UIdoc.Document);
            FilteredElementIterator itor = pcollector.OfClass(typeof(Autodesk.Revit.DB.View)).GetElementIterator();
            _floorplans = new System.Collections.Generic.Dictionary<string, int>();
            itor.Reset();
            while (itor.MoveNext())
            {
                Autodesk.Revit.DB.View view = itor.Current as Autodesk.Revit.DB.View;
                if (view != null && !view.IsTemplate)
                {
                    if (view.ViewType == ViewType.FloorPlan)
                        _floorplans.Add(view.Name, view.Id.IntegerValue);
                }
            }
        }

        #region Properties
        public Array FloorPlans
        {
            get
            {
                return _floorplans.Keys.ToArray();
            }
        }

        public IDictionary<string, int> AllPhases
        {
            get { return _allPhases; }
        }

        public int DoorCount
        {
            get { return _doors.Count; }
        }

        public IList<string> tokenList
        {
            set { _parameterTokens = value; }
            get { return _parameterTokens; }
        }

        public IList<int> parameterOrder
        {
            set { _parameterOrder = value; }
            get { return _parameterOrder; }
        }

        /// <summary>
        /// a list of parameters the user wants to edit.
        /// </summary>
        public IDictionary<int, bool> tokenizedParameters
        {
            set { _tokenizedList = value; }
            get { return _tokenizedList; }
        }
        #endregion

        public string GetLocalizedMarkHeader()
        {
            return LabelUtils.GetLabelFor(BuiltInParameter.ALL_MODEL_MARK);
        }

        public string GetLocalizedCommentHeader()
        {
            return LabelUtils.GetLabelFor(BuiltInParameter.ALL_MODEL_INSTANCE_COMMENTS);
        }

        public void ShowFloorPlaninRevit(string pname)
        {
            int viewID = _floorplans[pname];
            Autodesk.Revit.DB.View plan = _UIdoc.Document.GetElement(new ElementId(viewID)) as Autodesk.Revit.DB.View;
            _UIdoc.ActiveView = plan;
        }

        public void ShowDoorInRevit(int id)
        {
            _UIdoc.ShowElements(new Autodesk.Revit.DB.ElementId(id));
        }

        /// <summary>
        /// A list of parameters supplied from the schema. Duplicate parameters are specially named.
        /// </summary>
        /// <param name="list">A string\bool dictionary to convert to int\bool</param>
        public void set_TokenParameterList(IDictionary<string, bool> list)
        {
            if (_tokenizedList == null)
                _tokenizedList = new Dictionary<int, bool>();
            foreach (string pname in list.Keys)
            {
                if (pname.StartsWith("(")) //indicates a duplicate parameter name
                    switch (pname.Substring(0, 3))
                    {
                        case "(S)":
                            foreach (Door_Parameter dp in _masterParameterList.Values)
                            {
                                if (dp.ToString() == pname.Substring(3) &&
                                    _document.ParameterBindings.Contains(this.get_ParameterById(dp.Id).TheParameter.Definition) &&
                                    dp.TheParameter.IsShared)
                                {
                                    _tokenizedList.Add(dp.Id, list[pname]);
                                    break;
                                }
                            }
                            break;
                        case "(P)":
                            foreach (Door_Parameter dp in _masterParameterList.Values)
                            {
                                if (dp.ToString() == pname.Substring(3) &&
                                    _document.ParameterBindings.Contains(this.get_ParameterById(dp.Id).TheParameter.Definition) &&
                                    !dp.TheParameter.IsShared)
                                {
                                    _tokenizedList.Add(dp.Id, list[pname]);
                                    break;
                                }
                            }
                            break;
                        case "(F)":
                            foreach (Door_Parameter dp in _masterParameterList.Values)
                            {
                                if (dp.ToString() == pname.Substring(3) &&
                                    !dp.TheParameter.IsShared &&
                                    !_document.ParameterBindings.Contains(this.get_ParameterById(dp.Id).TheParameter.Definition))
                                {
                                    _tokenizedList.Add(dp.Id, list[pname]);
                                    break;
                                }
                            }
                            break;
                        default:
                            break;
                    }
                else
                    _tokenizedList.Add(get_ParameterByName(pname).Id, list[pname]);
            }
        }

        /// <summary>
        /// A list of parameters safe for storage in the schema. Duplicate parameters are specially named.
        /// </summary>
        /// <returns>A string\bool dictionary</returns>
        public IDictionary<string, bool> get_TokenizedParameters()
        {
            Dictionary<string, bool> list = new Dictionary<string, bool>();
            List<string> duplicates = new List<string>();
            List<string> temp = new List<string>();
            foreach (string s in this.get_AllParameterNames())
            {
                if (temp.Contains(s))
                    duplicates.Add(s);
                else
                    temp.Add(s);
            }

            if (_tokenizedList == null)
                throw new NullReferenceException("Door_Manager:get_TokenizedParameters was null in get_TokenParameterList()");
            foreach (int pid in _tokenizedList.Keys)
            {
                string pname = this.get_ParameterById(pid).ToString();
                if (!duplicates.Contains(pname))
                    list.Add(get_ParameterById(pid).ToString(), this.IsParameterTokenized(pid));
                else
                {
                    if (this.get_ParameterById(pid).TheParameter.IsShared &&
                        _document.ParameterBindings.Contains(this.get_ParameterById(pid).TheParameter.Definition))
                        list.Add("(S)" + get_ParameterById(pid).ToString(), this.IsParameterTokenized(pid));
                    else if (_document.ParameterBindings.Contains(this.get_ParameterById(pid).TheParameter.Definition) &&
                        !this.get_ParameterById(pid).TheParameter.IsShared)
                        list.Add("(P)" + get_ParameterById(pid).ToString(), this.IsParameterTokenized(pid));
                    else
                        list.Add("(F)" + get_ParameterById(pid).ToString(), this.IsParameterTokenized(pid));
                }
            }

            return list;
        }

        /// <summary>
        /// Get the list of parameters that are tokenized.
        /// </summary>
        /// <returns>String array of parameter names. Throws an exception if list is null.</returns>
        public string[] get_TokenParameterList()
        {
            if (_tokenizedList == null)
                throw new NullReferenceException("Door_Manager:_tokenizedList was null in get_TokenParameterList()");
            else
            {
                string[] list = new string[_tokenizedList.Count];
                int x = 0;
                foreach (int pid in _tokenizedList.Keys)
                {
                    if (_tokenizedList[pid] == true)
                    {
                        list[x] = _masterParameterList[pid].ToString();
                        x++;
                    }
                }
                return list;
            }
        }

        /// <summary>
        /// Takes the parameter order from the schema information.
        /// </summary>
        /// <param name="ar"></param>
        public void set_ParameterOrder(IList<string> ar)
        {
            if (_parameterOrder == null)
                _parameterOrder = new List<int>();
            foreach (string pname in ar)
            {
                if (pname.StartsWith("(")) //indicates a duplicate parameter name
                    switch (pname.Substring(0, 3))
                    {
                        case "(S)":
                            foreach (Door_Parameter dp in _masterParameterList.Values)
                            {
                                if (dp.ToString() == pname.Substring(3) &&
                                    _document.ParameterBindings.Contains(this.get_ParameterById(dp.Id).TheParameter.Definition) &&
                                    dp.TheParameter.IsShared)
                                {
                                    _parameterOrder.Add(dp.Id);
                                    break;
                                }
                            }
                            break;
                        case "(P)":
                            foreach (Door_Parameter dp in _masterParameterList.Values)
                            {
                                if (dp.ToString() == pname.Substring(3) &&
                                    _document.ParameterBindings.Contains(this.get_ParameterById(dp.Id).TheParameter.Definition) &&
                                    !dp.TheParameter.IsShared)
                                {
                                    _parameterOrder.Add(dp.Id);
                                    break;
                                }
                            }
                            break;
                        case "(F)":
                            foreach (Door_Parameter dp in _masterParameterList.Values)
                            {
                                if (dp.ToString() == pname.Substring(3) &&
                                    !dp.TheParameter.IsShared &&
                                    !_document.ParameterBindings.Contains(this.get_ParameterById(dp.Id).TheParameter.Definition))
                                {
                                    _parameterOrder.Add(dp.Id);
                                    break;
                                }
                            }
                            break;
                        default:
                            break;
                    }
                else
                    _parameterOrder.Add(get_ParameterByName(pname).Id);
            }
        }

        /// <summary>
        /// Get a list of parameters in the order they should appear in the grid.
        /// </summary>
        /// <returns>An ordered string array of parameter names. Throws an exception if list is null.</returns>
        public string[] get_ParameterOrder()
        {
            List<string> list = new List<string>(this.get_AllParameterNames().Length);
            List<string> duplicates = new List<string>();
            List<string> temp = new List<string>();
            foreach (string s in this.get_AllParameterNames())
            {
                if (temp.Contains(s))
                    duplicates.Add(s);
                else
                    temp.Add(s);
            }

            if (_parameterOrder == null)
                throw new NullReferenceException("Door_Manager:_parameterOrder was null in get_ParameterOrder()");
            foreach (int pid in _parameterOrder)
            {
                string pname = this.get_ParameterById(pid).ToString();
                if (!duplicates.Contains(pname))
                    list.Add(get_ParameterById(pid).ToString());
                else
                {
                    if (this.get_ParameterById(pid).TheParameter.IsShared && _document.ParameterBindings.Contains(this.get_ParameterById(pid).TheParameter.Definition))
                        list.Add("(S)" + get_ParameterById(pid).ToString());
                    else if (_document.ParameterBindings.Contains(this.get_ParameterById(pid).TheParameter.Definition) &&
                        !this.get_ParameterById(pid).TheParameter.IsShared)
                    {
                        list.Add("(P)" + get_ParameterById(pid).ToString());
                    }
                    else
                        list.Add("(F)" + get_ParameterById(pid).ToString());
                }
            }

            return list.ToArray();
        }

        public void setToFromRooms()
        {
            foreach (Door_Information door in _doors)
            {
                door.setRooms(PhaseId(), ref _document);
            }
        }

        public string PhaseName()
        {
            return _phaseName;
        }

        public int PhaseId()
        {
            return _phase;
        }

        public void Phase(string phase)
        {
            _phase = _allPhases[phase];
            _phaseName = phase;
        }

        public void RemoveKeyFromDoor(int ID)
        {
            Door_Information di = getDoorInformation(ID);
            if (di.key == Door_Information.KeyState.IsKeyed)
            {
                di.removeKey();
            }
        }

        public void UndoCurrentDoor(int ID)
        {
            Door_Information di = getDoorInformation(ID);
            di.resetValues();
        }

        public void UndoAllDoors()
        {
            foreach (Door_Information di in _doors)
            {
                di.resetValues();
            }
        }

        public void ClearAllDoors()
        {
            foreach (Door_Information di in _doors)
            {
                di.clearValues();
            }
        }

        public void UnCheckAllDoors()
        {
            foreach (Door_Information di in _doors)
            {
                di.unCheckValues();
            }
        }

        public void ClearCurrentDoor(int ID)
        {
            Door_Information di = getDoorInformation(ID);
            di.clearValues();
        }

        public void ScanForTokens()
        {
            //TODO: create code to scan thru parameters and see which ones have the same values more than once. Present as possible tokens to user.
        }

        public bool IsParameterTokenized(int pid)
        {
            if (_tokenizedList == null)
                return false;
            if (_tokenizedList.ContainsKey(pid))
                return _tokenizedList[pid];
            else
                return false;
        }

        public bool IsParameterYesNo(int pid)
        {
            Door_Parameter p = null;
            if (_masterParameterList.TryGetValue(pid, out p))
                return p.IsYesNo;
            else
                throw new Exception("Door_Manager:IsParameterYesNo could not find parameter");
        }

        public bool UpdateDoors(Document doc)
        {
            bool result = true;
            bool keyRemoved = false; //removing keys causes Revit to remove the parameter value as well. We need to add it back it.

            foreach (Door_Information door in _doors)
            {
                if (door.hasChanges)
                {
                    Element doorElement = doc.GetElement(new ElementId(door.Id));

                    //remove keys from parameters
                    if (door.key == Door_Information.KeyState.RemoveKey)
                    {
                        SubTransaction keytrans = new SubTransaction(doc);
                        keytrans.Start();
                        foreach (string parm in _keyParameters)
                        {
                            FamilyInstance fi = doorElement as FamilyInstance;
                            Parameter p = fi.LookupParameter(parm);
                            //project and shared parameters can be duplicates of family parameters. Check for correct parameter type
                            if (p.StorageType != StorageType.ElementId || p.Definition.ParameterType != ParameterType.Invalid)
                            {
                                foreach (Parameter prmtr in fi.Parameters)
                                {
                                    if (prmtr.StorageType == StorageType.ElementId && p.Definition.ParameterType == ParameterType.Invalid)
                                        if (prmtr.Definition.Name == parm)
                                            p = prmtr;
                                }
                            }
                            result = p.Set(ElementId.InvalidElementId);
                            if (!result)
                            {
                                keytrans.RollBack();
                                return result;
                            }
                        }
                        keyRemoved = true;
                        keytrans.Commit();
                    }

                    //update comment
                    if (door.hasNewComment || keyRemoved)
                    {
                        Parameter p = doorElement.get_Parameter(BuiltInParameter.ALL_MODEL_INSTANCE_COMMENTS);
                        result = p.Set(door.doorComment);
                    }
                    if (!result)
                        return result;

                    //update parameters
                    foreach (Door_Parameter dr_parm in door.doorparameters.Values)
                    {
                        if (dr_parm.HasChanged || keyRemoved)
                        {
                            Parameter p = dr_parm.TheParameter;
                            if (p.IsReadOnly)
                                result = true;
                            else
                            {
                                if (dr_parm.IsYesNo)
                                {
                                    if (dr_parm.Value == "Yes")
                                        p.Set(1);
                                    else
                                        p.Set(0);
                                }
                                else
                                    result = p.Set(dr_parm.Value);
                            }
                        }
                        if (!result)
                            return result;
                    }
                }
            }

            return result;
        }

        public bool IsCommentReadOnly(int doorID)
        {
            return getDoorInformation(doorID).IsCommentReadOnly;
        }

        public string GetDoorComment(int doorID)
        {
            return getDoorInformation(doorID).doorComment;
        }

        public void UpdateDoorComment(int doorID, string value)
        {
            getDoorInformation(doorID).doorComment = value;
        }

        public bool IsDoorCommentNew(int doorID)
        {
            return getDoorInformation(doorID).hasNewComment;
        }

        /// <summary>
        /// Updated the parameter of the door to the supplied value.
        /// </summary>
        /// <param name="doorID">ElementID of the door</param>
        /// <param name="parmName">Parameter to update</param>
        /// <param name="newValue">Value to update to.</param>
        /// <returns>Returns true if the door has the parameter, false otherwise</returns>
        public bool UpdateDoorParameter(int doorID, int pid, string newValue)
        {
            return getDoorInformation(doorID).setParameter(pid, newValue);
        }

        /// <summary>
        /// Updated the parameter of the door to the supplied true\false value.
        /// </summary>
        /// <param name="doorID">ElementID of the door</param>
        /// <param name="parmName">Parameter to update</param>
        /// <param name="newValue">Value to update to.</param>
        /// <returns>Returns true if the door has the parameter, false otherwise</returns>
        public bool UpdateDoorParameter(int doorID, int pid, bool? newValue)
        {
            if (newValue == null)
                throw new NullReferenceException();
            return getDoorInformation(doorID).setParameter(pid, newValue);
        }

        /// <summary>
        /// Check to see if the parameter value has changed from its original value
        /// </summary>
        /// <param name="doorID">ElementID of the door</param>
        /// <param name="parmName">The parameter to check</param>
        /// <returns>True if value is different from original value, false otherwise</returns>
        public bool DoorParameterChanged(int doorID, int pid)
        {
            return getDoorInformation(doorID).getParameter(pid).HasChanged;
        }

        /// <summary>
        /// Check to see if the parameter is read only
        /// </summary>
        /// <param name="doorID">ElementID of the door</param>
        /// <param name="parmName">The parameter to check</param>
        /// <returns>True if read only, false otherwise</returns>
        public bool DoorParameterIsReadOnly(int doorID, int pid)
        {
            return getDoorInformation(doorID).getParameter(pid).IsReadOnly;
        }

        public IEnumerator<Door_Information> GetEnumerator()
        {
            return _doors.GetEnumerator();
        }

        /// <summary>
        /// Gets the next valid token the parameter can have.
        /// </summary>
        /// <param name="token">Current value of the token</param>
        /// <returns>Next valid token value.</returns>
        public string get_NextToken(string token)
        {
            int index = 0;
            if (token == null || token == string.Empty)
                index = -1;
            else
            {
                if (_parameterTokens.Contains(token))
                    index = _parameterTokens.IndexOf(token);
            }
            index++;
            if (index == _parameterTokens.Count)
                return null;
            else
                return _parameterTokens[index] as string;
        }

        /// <summary>
        /// Get a list of all text and YesNo type parameters that doors have.
        /// </summary>
        /// <returns>An alphabetized string array of parameter names. Throws an exception if list is null.</returns>
        public string[] get_AllParameterNames()
        {
            if (_masterParameterList == null)
                throw new NullReferenceException("Door_Manager:_masterParameterList was null in get_AllParameters()");
            else
            {
                int x = 0;
                string[] array = new string[_masterParameterList.Values.Count];
                foreach (Door_Parameter p in _masterParameterList.Values)
                {
                    array[x] = p.TheParameter.Definition.Name;
                    x++;
                }
                return array;
            }
        }

        public int[] get_AllParameterIds()
        {
            if (_masterParameterList == null)
                throw new NullReferenceException("Door_Manager:_masterParameterList was null in get_AllParameters()");
            else
                return _masterParameterList.Keys.ToArray<int>();
        }

        public Door_Parameter[] get_AllParameters()
        {
            if (_masterParameterList == null)
                throw new NullReferenceException("Door_Manager:_masterParameterList was null in get_AllParameters()");
            else
                return _masterParameterList.Values.ToArray<Door_Parameter>();
        }

        public Door_Parameter get_ParameterById(int id)
        {
            return _masterParameterList[id];
        }

        public Door_Parameter get_ParameterByName(string pname)
        {
            foreach (Door_Parameter dp in _masterParameterList.Values)
            {
                if (dp.ToString() == pname)
                    return dp;
            }
            return null;
        }

        public Door_Information getDoorInformation(int doorID)
        {
            IEnumerator en = _doors.GetEnumerator();
            en.Reset();
            en.MoveNext();
            Door_Information di = en.Current as Door_Information;
            while (di.Id != doorID)
            {
                en.MoveNext();
                di = en.Current as Door_Information;
            }
            return di;
        }

        public bool phaseHasDoors(int phaseId)
        {
            foreach(Door_Information di in _doors)
            {
                if (di.IsPhaseValid(phaseId))
                    return true;
            }
            return false;
        }
    }
}
