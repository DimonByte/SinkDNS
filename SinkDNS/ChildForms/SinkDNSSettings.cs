using SinkDNS.Modules;
using SinkDNS.Modules.SinkDNSInternals;
using SinkDNS.Properties;
using System.Configuration;

namespace SinkDNS.ChildForms
{
    public partial class SinkDNSSettings : Form
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
            //Save the setting that was changed only!
            //Check if settings are null
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
                        }
                    }
                }
            }
            catch(Exception ex)
            {
                TraceLogger.LogAndThrowMsgBox($"Failed to save setting: {e.ChangedItem.PropertyDescriptor.Name} - {ex.Message}", Enums.StatusSeverityType.Error);
                return;
            }
        }
    }
}
