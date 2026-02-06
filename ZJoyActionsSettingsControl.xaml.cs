using SimHub.Plugins.OutputPlugins.Dash.GLCDTemplating;
using SimHub.Plugins.Styles;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Controls;

namespace User.ZJoyActions
{
    

    /// <summary>
    /// Logique d'interaction pour VJoyActionsSettingsControl.xaml
    /// </summary>
    public partial class ZJoyActionsSettingsControl : UserControl
    {
        public ZJoyActionsPlugin Plugin { get; }

        public ZJoyActionsSettingsControl()
        {
            InitializeComponent();
        }

        public ZJoyActionsSettingsControl(ZJoyActionsPlugin plugin) : this()
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