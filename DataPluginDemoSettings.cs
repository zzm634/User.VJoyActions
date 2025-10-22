using SimHub.Plugins.OutputPlugins.Dash.GLCDTemplating;
using System.ComponentModel;

namespace User.PluginSdkDemo
{
    // I recognize this is not the best way to do "a two dimensional array of very similar values", but thesw WPF bindings have me at my wits end.  

    public class DataPluginDemoSettings
    {

        public static readonly HID_USAGES[] AXIS_ORDER = {
            HID_USAGES.HID_USAGE_X,
            HID_USAGES.HID_USAGE_Y,
            HID_USAGES.HID_USAGE_Z,
            HID_USAGES.HID_USAGE_RX,
            HID_USAGES.HID_USAGE_RY,
            HID_USAGES.HID_USAGE_RZ,
            HID_USAGES.HID_USAGE_SL0,
            HID_USAGES.HID_USAGE_SL1,
            HID_USAGES.HID_USAGE_WHL,
            HID_USAGES.HID_USAGE_ACCELERATOR,
            HID_USAGES.HID_USAGE_BRAKE,
            HID_USAGES.HID_USAGE_CLUTCH,
            HID_USAGES.HID_USAGE_STEERING,
            HID_USAGES.HID_USAGE_AILERON,
            HID_USAGES.HID_USAGE_RUDDER,
            HID_USAGES.HID_USAGE_THROTTLE
        };

        public ExpressionValue[] Expresions = new ExpressionValue[16 * 16];



        private static void fillBlankExpressions(ExpressionValue[] e)
        {
           for(uint i = 0; i < e.Length; i++)
            {
                e[i] = new ExpressionValue();
            }
        }

        public static DataPluginDemoSettings newBlankSettings()
        {
            DataPluginDemoSettings settings = new DataPluginDemoSettings();
            fillBlankExpressions(settings.Expresions);

            return settings;
        }

    }
}