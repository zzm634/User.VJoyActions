using SimHub.Plugins.OutputPlugins.Dash.GLCDTemplating;
using SimHub.Plugins.Styles;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Controls;

namespace User.VJoyActions
{
    

    /// <summary>
    /// Logique d'interaction pour VJoyActionsSettingsControl.xaml
    /// </summary>
    public partial class VJoyActionsSettingsControl : UserControl
    {
        public VJoyActionsPlugin Plugin { get; }

        public VJoyActionsSettingsControl()
        {
            InitializeComponent();
        }

        public VJoyActionsSettingsControl(VJoyActionsPlugin plugin) : this()
        {
            this.Plugin = plugin;

            if(plugin.settings == null)
            {
                SimHub.Logging.Current.Error("settings null");
            }

            this.DataContext = plugin.settings;
            this.buttons.ItemsSource = plugin.settings.Expresions;
        }
    }
}