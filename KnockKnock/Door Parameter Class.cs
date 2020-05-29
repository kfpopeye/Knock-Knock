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
    /// Contains information for one door parameter.
    /// </summary>
    public class Door_Parameter : INotifyPropertyChanged
    {
        private string _value = string.Empty; //the value to set the parameter
        private string _oldvalue;
        private bool _IsReadOnly = false;
        private bool _oldreadonly;
        private bool _IsChecked = false; //checked (right clicked) by user
        public bool IsYesNo = false;
        public Parameter TheParameter = null;
        public event PropertyChangedEventHandler PropertyChanged;

        public bool IsReadOnly
        {
            get { return _IsReadOnly; }
            set
            {
                _IsReadOnly = value;
                OnPropertyChanged("IsReadOnly");
            }
        }

        public bool IsChecked
        {
            get { return _IsChecked; }
            set
            {
                if (!this.IsReadOnly && !this.HasChanged)
                {
                    _IsChecked = value;
                    OnPropertyChanged("IsChecked");
                }
            }
        }

        public string Value
        {
            get { return _value; }
            set
            {
                if (!this.IsChecked && !this.IsReadOnly)
                {
                    if (value == null)
                        _value = string.Empty;
                    else
                        _value = value;
                    OnPropertyChanged("Value");
                    OnPropertyChanged("HasChanged");
                }
            }
        }

        public bool HasChanged //has the user changed this value
        {
            get
            {
                if (_value == _oldvalue)
                    return false;
                else
                    return true;
            }
        }

        public int Id
        {
            get
            {
                return TheParameter.Id.IntegerValue;
            }
        }

        public string Name
        {
            get
            {
                return TheParameter.Definition.Name;
            }
        }

        public bool YesNoState
        {
            get
            {
                if (_value == "Yes")
                    return true;
                else
                    return false;
            }
            set
            {
                if (!_IsReadOnly && !_IsChecked)
                    if (value == true)
                        _value = "Yes";
                    else
                        _value = string.Empty;
                OnPropertyChanged("YesNoState");
                OnPropertyChanged("HasChanged");
            }
        }

        /// <summary>
        /// Not all doors contain the same parameters. Some parameters are added in the family file.
        /// This constructors returns a n/a parameter.
        /// </summary>
        public Door_Parameter()
        {
            _value = "n/a";
            _oldvalue = _value;
            IsReadOnly = true;
            _oldreadonly = IsReadOnly;
        }

        public Door_Parameter(Parameter p)
        {
            TheParameter = p;

            if (p.Definition.ParameterType == ParameterType.YesNo)
            {
                if (p.AsInteger() == 1)
                    _value = "Yes";
                IsYesNo = true;
            }
            else
            {
                if (p.HasValue)
                    _value = p.AsString();
            }
            if (_value == null)
                _value = string.Empty;
            _oldvalue = _value;
            IsReadOnly = p.IsReadOnly;
            _oldreadonly = IsReadOnly;
        }

        public void Reset()
        {
            _value = _oldvalue;
            IsReadOnly = _oldreadonly;
            OnPropertyChanged("Value");
            OnPropertyChanged("HasChanged");
            OnPropertyChanged("IsReadOnly");
            OnPropertyChanged("IsChecked");
        }

        /// <summary>
        /// Overridden to return parameter name.
        /// </summary>
        /// <returns>Name of the parameter</returns>
        public override string ToString()
        {
            return TheParameter.Definition.Name;
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

    public class MyEqualityComparer : IEqualityComparer<KeyValuePair<int, Door_Parameter>>
    {
        public bool Equals(KeyValuePair<int, Door_Parameter> x, KeyValuePair<int, Door_Parameter> y)
        {
            //we are comparing the keys only.
            return x.Key == y.Key;
        }

        public int GetHashCode(KeyValuePair<int, Door_Parameter> obj)
        {
            return obj.Key.GetHashCode();
        }
    }
}
