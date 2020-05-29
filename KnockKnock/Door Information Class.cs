using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel;

using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

namespace KnockKnock
{
    /// <summary>
    /// Contains information for one door instance in the project for easy access and modification.
    /// </summary>
    [DefaultPropertyAttribute("doorNumber")]
    public class Door_Information : INotifyPropertyChanged
    {
        public enum KeyState { NoKey = 0, IsKeyed = 1, RemoveKey = 2 };
        private int _instance_id = -1; //the unique id for the instance of the door
        private bool _hasChanges = false; //indicates if this door instances has changes that need to be written back to the project.
        private string _doorNo = string.Empty; //Mark parameter
        private string _doorComment = string.Empty; //instance comment
        private string _oldComment = string.Empty; //original comment for checking if changed

        private string _level = string.Empty;           //
        private string _fromroom = "None";              //
        private string _toroom = "None";                //
        private string _size = string.Empty;            //
        private string _familyname = string.Empty;      //  used for side window information display only
        private string _typename = string.Empty;        //
        private string _phasecreated = string.Empty;    //
        private string _phasedemolished = "None";       //  
        private int _phaseIdDemolished = -1;            //

        private bool _oldCommentReadOnly = false;
        private bool _IsCommentReadOnly = false;
        private bool _IsCommentChecked = false;
        private KeyState _key = KeyState.NoKey;
        private IList<int> _phases = null;
        internal Dictionary<int, Door_Parameter> _parameters = null;
        public event PropertyChangedEventHandler PropertyChanged;

        #region Properties
        [BrowsableAttribute(false)]
        public bool IsCommentReadOnly 
        {
            get { return _IsCommentReadOnly; }
            set
            {
                _IsCommentReadOnly = value;
                OnPropertyChanged("IsCommentReadOnly");
            }
        }

        [BrowsableAttribute(false)]
        public bool IsCommentChecked
        {
            get { return _IsCommentChecked; }
            set
            {
                if (!hasNewComment && !IsCommentReadOnly)
                {
                    _IsCommentChecked = value;
                    OnPropertyChanged("IsCommentChecked");
                }
            }
        }

        [ReadOnlyAttribute(true), DisplayName("Mark")]
        public string doorNumber
        {
            get { return _doorNo; }
            set { _doorNo = value; }
        }

        [ReadOnlyAttribute(true), DisplayName("Phase Demolished")]
        public string phaseDemolished
        {
            get { return _phasedemolished; }
            set { _phasedemolished = value; }
        }

        [ReadOnlyAttribute(true), DisplayName("Phase Created")]
        public string phaseCreated
        {
            get { return _phasecreated; }
            set { _phasecreated = value; }
        }

        [ReadOnlyAttribute(true), DisplayName("Type Name")]
        public string typeName
        {
            get { return _typename; }
            set { _typename = value; }
        }

        [ReadOnlyAttribute(true), DisplayName("Family Name")]
        public string familyName
        {
            get { return _familyname; }
            set { _familyname = value; }
        }

        [ReadOnlyAttribute(true), DisplayName("Size")]
        public string size
        {
            get { return _size; }
            set { _size = value; }
        }

        [ReadOnlyAttribute(true), DisplayName("To Room")]
        public string toRoom
        {
            get { return _toroom; }
            set { _toroom = value; }
        }

        [ReadOnlyAttribute(true), DisplayName("From Room")]
        public string fromRoom
        {
            get { return _fromroom; }
            set { _fromroom = value; }
        }

        [ReadOnlyAttribute(true), DisplayName("Host Level")]
        public string hostlevel
        {
            get { return _level; }
            set { _level = value; }
        }

        [ReadOnlyAttribute(true), DisplayName("Is Keyed")]
        [TypeConverter(typeof(KeyStateEnumConverter))]
        public KeyState key
        {
            get { return _key; }
            set
            {
                _key = value;
                OnPropertyChanged("key");
            }
        }

        /// <summary>
        /// The int value of the instance elementID
        /// </summary>
        [BrowsableAttribute(false)]
        public int Id
        {
            get { return _instance_id; }
        }

        [BrowsableAttribute(false)]
        public Dictionary<int, Door_Parameter> doorparameters
        {
            get { return _parameters; }
        }

        /// <summary>
        /// The value of the instance comment
        /// </summary>
        [BrowsableAttribute(false)]
        public string doorComment
        {
            get { return _doorComment; }
            set
            {
                if (!IsCommentReadOnly)
                {
                    _doorComment = value;
                    _hasChanges = true;
                    OnPropertyChanged("doorComment");
                    OnPropertyChanged("hasNewComment");
                }
            }
        }

        [BrowsableAttribute(false)]
        public bool hasNewComment
        {
            get
            {
                if ((_doorComment == null || _doorComment == string.Empty) 
                    && 
                    (_oldComment == null || _oldComment == string.Empty))
                    return false;
                if (_doorComment == _oldComment)
                    return false;
                return true;
            }
        }

        [BrowsableAttribute(false)]
        public bool hasChanges
        {
            get
            {
                foreach (Door_Parameter dp in doorparameters.Values)
                {
                    if (dp.HasChanged)
                        return true;
                }
                return _hasChanges;
            }
        }
        #endregion

        public Door_Information(Element door, IList<string> keyparms)
        {
            _instance_id = door.Id.IntegerValue;
            _hasChanges = false;
            _doorNo = door.get_Parameter(BuiltInParameter.ALL_MODEL_MARK).AsString();
            _doorComment = door.get_Parameter(BuiltInParameter.ALL_MODEL_INSTANCE_COMMENTS).AsString();
            IsCommentReadOnly = door.get_Parameter(BuiltInParameter.ALL_MODEL_INSTANCE_COMMENTS).IsReadOnly;
            _oldCommentReadOnly = IsCommentReadOnly;
            _oldComment = _doorComment;
            _parameters = new Dictionary<int, Door_Parameter>(door.Parameters.Size);

            foreach (Parameter p in door.Parameters)
            {
                InternalDefinition idef = p.Definition as InternalDefinition;
                if (idef.Visible //shared parameters can be set to not visible. These should be ignored.
                    && idef.BuiltInParameter != BuiltInParameter.ALL_MODEL_MARK
                    && idef.BuiltInParameter != BuiltInParameter.ALL_MODEL_INSTANCE_COMMENTS
                   )
                {
                    if (idef.ParameterType == ParameterType.Text || idef.ParameterType == ParameterType.YesNo) //use only text or YesNo type parameters
                        _parameters.Add(p.Id.IntegerValue, new Door_Parameter(p));
                }
            }

            FamilyInstance fi = door as FamilyInstance;
            _level = door.Document.GetElement(door.LevelId).Name;

            foreach (string parm in keyparms)
            {
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
                ElementId id = p.AsElementId();
                Element key = fi.Document.GetElement(id);
                if (key != null)
                    _key = KeyState.IsKeyed;
            }

            _size = fi.Symbol.get_Parameter(BuiltInParameter.GENERIC_WIDTH).AsValueString() + " x " +
                fi.Symbol.get_Parameter(BuiltInParameter.WINDOW_HEIGHT).AsValueString() + " x " +
                fi.Symbol.get_Parameter(BuiltInParameter.WINDOW_THICKNESS).AsValueString();
            _familyname = fi.Symbol.Family.Name;
            _typename = fi.Symbol.Name;
            _phasecreated = door.Document.GetElement(door.CreatedPhaseId).Name;
            if (door.DemolishedPhaseId.IntegerValue != -1)
            {
                _phasedemolished = door.Document.GetElement(door.DemolishedPhaseId).Name;
                _phaseIdDemolished = door.Document.GetElement(door.DemolishedPhaseId).Id.IntegerValue;
            }

            PhaseArray pa = door.Document.Phases;
            _phases = new List<int>(pa.Size);
            foreach (Phase p in pa)
            {
                if (door.GetPhaseStatus(p.Id) != ElementOnPhaseStatus.Future && door.GetPhaseStatus(p.Id) != ElementOnPhaseStatus.Past)
                    _phases.Add(p.Id.IntegerValue);
            }
        }

        /// <summary>
        /// Sets the to and from room variables to the rooms that exist in the supplied phase
        /// </summary>
        /// <param name="phaseID">Int value of the phase to look for rooms</param>
        /// <param name="doc">The project document (usually supplied from the door manager)</param>
        public void setRooms(int phaseID, ref Document doc)
        {
            if (IsPhaseValid(phaseID) && phaseID != _phaseIdDemolished)
            {
                //TODO: in-place doors don't report rooms (error in family creation??)
                Phase phase = doc.GetElement(new ElementId(phaseID)) as Phase;
                FamilyInstance fi = doc.GetElement(new ElementId(_instance_id)) as FamilyInstance;
                Autodesk.Revit.DB.Architecture.Room room = fi.get_FromRoom(phase);
                if (room != null)
                    _fromroom = room.Name;
                room = fi.get_ToRoom(phase);
                if (room != null)
                    _toroom = room.Name;
            }
            else
            {
                _fromroom = "None";
                _toroom = "None";
            }
        }

        public bool IsPhaseValid(int phaseID)
        {
            if (_phases.Contains(phaseID))
                return true;

            return false;
        }

        public void removeKey()
        {
            _key = KeyState.RemoveKey;
            foreach (Door_Parameter dp in _parameters.Values)
            {
                dp.IsReadOnly = false;
            }
            IsCommentReadOnly = false;
            _hasChanges = true;
        }

        public void checkValues()
        {
            foreach (Door_Parameter dp in _parameters.Values)
            {
                dp.IsChecked = true;
            }
            _IsCommentChecked = true;
        }

        public void unCheckValues()
        {
            foreach (Door_Parameter dp in _parameters.Values)
            {
                dp.IsChecked = false;
            }
            _IsCommentChecked = false;
        }

        public void resetValues()
        {
            foreach (Door_Parameter dp in _parameters.Values)
            {
                dp.Reset();
            }
            _doorComment = _oldComment;
            _hasChanges = false;
            if (_key == KeyState.RemoveKey)
                _key = KeyState.IsKeyed;
        }

        public void clearValues()
        {
            foreach (Door_Parameter dp in _parameters.Values)
            {
                if (!dp.IsReadOnly)
                {
                    dp.Value = String.Empty;
                    dp.IsReadOnly = false;
                    _hasChanges = true;
                }
            }
            if (!IsCommentReadOnly)
            {
                _doorComment = null;
                _hasChanges = true;
            }
        }

        public bool setParameter(int pid, string value)
        {
            if (value == null)
                value = string.Empty;

            if (_parameters.ContainsKey(pid))
            {
                _parameters[pid].Value = value;
                this._hasChanges = true;
                return true;
            }
            else
                return false;
        }

        public bool setParameter(int pid, bool? value)
        {
            if (_parameters.ContainsKey(pid))
            {
                if (value != null && value == true)
                    _parameters[pid].Value = "Yes";
                else
                    _parameters[pid].Value = string.Empty;
                this._hasChanges = true;
                return true;
            }
            else
                return false;
        }

        public string[] getParameterNames()
        {
            int x = 0;
            string[] array = new string[_parameters.Count];
            foreach (Door_Parameter p in _parameters.Values)
            {
                array[x] = p.TheParameter.Definition.Name;
                x++;
            }
            return array;
        }

        public Dictionary<int, Door_Parameter> getDoorParameters()
        {
            Dictionary<int, Door_Parameter> second = new Dictionary<int, Door_Parameter>(_parameters);
            return second;
        }

        public Door_Parameter getParameter(int pid)
        {
            if (_parameters.ContainsKey(pid))
                return _parameters[pid];
            else
                return new Door_Parameter();
        }

        // Create the OnPropertyChanged method to raise the event 
        protected void OnPropertyChanged(string name)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(name));
            }
        }
    }

    public class KeyStateEnumConverter : TypeConverter
    {
        // Overrides the ConvertTo method of TypeConverter.
        public override object ConvertTo(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value, Type destinationType)
        {
            if (destinationType == typeof(string))
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
            return base.ConvertTo(context, culture, value, destinationType);
        }
    }
}
