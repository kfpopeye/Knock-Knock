﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace KnockKnock.Properties {
    
    
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.VisualStudio.Editors.SettingsDesigner.SettingsSingleFileGenerator", "16.4.0.0")]
    internal sealed partial class Settings : global::System.Configuration.ApplicationSettingsBase {
        
        private static Settings defaultInstance = ((Settings)(global::System.Configuration.ApplicationSettingsBase.Synchronized(new Settings())));
        
        public static Settings Default {
            get {
                return defaultInstance;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("")]
        public string AddinPath {
            get {
                return ((string)(this["AddinPath"]));
            }
            set {
                this["AddinPath"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("Arial, 12pt")]
        public global::System.Drawing.Font ScheduleFont {
            get {
                return ((global::System.Drawing.Font)(this["ScheduleFont"]));
            }
            set {
                this["ScheduleFont"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("Yellow")]
        public global::System.Drawing.Color ReadOnlyColor {
            get {
                return ((global::System.Drawing.Color)(this["ReadOnlyColor"]));
            }
            set {
                this["ReadOnlyColor"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("SpringGreen")]
        public global::System.Drawing.Color CheckedColor {
            get {
                return ((global::System.Drawing.Color)(this["CheckedColor"]));
            }
            set {
                this["CheckedColor"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("Salmon")]
        public global::System.Drawing.Color ChangedColor {
            get {
                return ((global::System.Drawing.Color)(this["ChangedColor"]));
            }
            set {
                this["ChangedColor"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("Arial, 12pt")]
        public global::System.Drawing.Font HeaderFont {
            get {
                return ((global::System.Drawing.Font)(this["HeaderFont"]));
            }
            set {
                this["HeaderFont"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("2015-01-01")]
        public global::System.DateTime EntCheck {
            get {
                return ((global::System.DateTime)(this["EntCheck"]));
            }
            set {
                this["EntCheck"] = value;
            }
        }
    }
}