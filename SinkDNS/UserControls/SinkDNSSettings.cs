//MIT License

//Copyright (c) 2025 - 2026 Dimon

//Permission is hereby granted, free of charge, to any person obtaining a copy
//of this software and associated documentation files (the "Software"), to deal
//in the Software without restriction, including without limitation the rights
//to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
//copies of the Software, and to permit persons to whom the Software is
//furnished to do so, subject to the following conditions:

//The above copyright notice and this permission notice shall be included in all
//copies or substantial portions of the Software.

//THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
//IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
//FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
//AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
//LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
//OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
//SOFTWARE.

using SinkDNS.Modules;
using SinkDNS.Modules.SinkDNSInternals;
using SinkDNS.Properties;
using System.Configuration;

namespace SinkDNS.ChildForms
{
    public partial class SinkDNSSettings : UserControl
    {
        public SinkDNSSettings()
        {
            InitializeComponent();
        }

        private void SinkDNSSettings_Load(object sender, EventArgs e)
        {
            //pgSetting is the property grid that is on this form. This will load the settings from Settings.Default and list them.
            foreach (SettingsProperty property in Settings.Default.Properties)
            {
                pgSettings.SelectedObject = Settings.Default;
            }
        }

        private void pgSettings_PropertyValueChanged(object s, PropertyValueChangedEventArgs e)
        {
            if (e?.ChangedItem?.PropertyDescriptor?.Name == null)
            {
                TraceLogger.Log("Failed to save setting: Property name is null", Enums.StatusSeverityType.Error);
                return;
            }
            if (pgSettings?.SelectedObject == null)
            {
                TraceLogger.Log("Failed to save setting: Selected object is null", Enums.StatusSeverityType.Error);
                return;
            }
            if (Settings.Default == null)
            {
                TraceLogger.Log("Failed to save setting: Settings.Default is null", Enums.StatusSeverityType.Error);
                return;
            }
            try
            {
                foreach (SettingsProperty property in Settings.Default.Properties)
                {
                    if (property.Name == e.ChangedItem.PropertyDescriptor.Name)
                    {
                        if (property.Attributes[typeof(UserScopedSettingAttribute)] is UserScopedSettingAttribute)
                        {
                            Settings.Default[property.Name] = pgSettings.SelectedObject.GetType().GetProperty(property.Name)?.GetValue(pgSettings.SelectedObject, null);
                            Settings.Default.Save();
                            TraceLogger.Log($"Setting saved: {property.Name} - {Settings.Default[property.Name]}");
                        }
                    }
                }
            }
            catch(Exception ex)
            {
                TraceLogger.LogAndThrowMsgBox($"Failed to save setting: {e.ChangedItem.PropertyDescriptor.Name} - {ex.ToString()}", Enums.StatusSeverityType.Error);
                return;
            }
        }
    }
}
