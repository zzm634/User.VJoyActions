using SimHub.Plugins.OutputPlugins.Dash.GLCDTemplating;
using System.ComponentModel;

namespace User.ZJoyActions
{
    // I recognize this is not the best way to do "a two dimensional array of very similar values", but thesw WPF bindings have me at my wits end.  

    public class ZJoyActionsSettings
    {
        // The order in which vJoy axes are listed and configured in the UI.
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

        // Expressions for each axis and device
        public ExpressionValue[] Expresions = new ExpressionValue[16 * 16];

        private static void fillBlankExpressions(ExpressionValue[] e)
        {
           for(uint i = 0; i < e.Length; i++)
            {
                e[i] = new ExpressionValue();
            }
        }

        // Create a new settings object filled with empty expressions.
        public static ZJoyActionsSettings newBlankSettings()
        {
            ZJoyActionsSettings settings = new ZJoyActionsSettings();
            fillBlankExpressions(settings.Expresions);

            return settings;
        }

    }
}