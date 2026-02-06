using SimHub.Plugins;
using SimHub.Plugins.OutputPlugins.Dash.GLCDTemplating;
using SimHub.Plugins.OutputPlugins.Dash.TemplatingCommon;
using System;
using System.Collections.Generic;
using System.Timers;
using System.Windows.Controls;
using System.Windows.Media;
using vJoyInterfaceWrap;

namespace User.ZJoyActions
{
    [PluginDescription("Control vJoy axes and buttons with SimHub expressions and events.")]
    [PluginAuthor("Zach C Miller")]
    [PluginName("vJoy Actions")]
    public class ZJoyActionsPlugin : IPlugin, IWPFSettingsV2, IInputPlugin
    {
        private HashSet<uint> acquiredVJoyDevices = new HashSet<uint>();

        public ZJoyActionsSettings settings;

        private vJoy vjoy = new vJoy();

        private NCalcEngineBase nCalcEngine = new NCalcEngineBase();

        private Timer axisUpdateTimer;

        private bool ensureAcquired(uint vJoyDeviceId)
        {
            if (!acquiredVJoyDevices.Contains(vJoyDeviceId))
            {
                if(!(vjoy.isVJDExists(vJoyDeviceId)))
                {
                    return false;
                }

                if (vjoy.AcquireVJD(vJoyDeviceId))
                {
                    acquiredVJoyDevices.Add(vJoyDeviceId);
                    return true;
                }

                SimHub.Logging.Current.Warn("Failed to acquire vJoy device " + vJoyDeviceId);
                return false;
            }
            return true;
        }

        public PluginManager PluginManager { get; set; }

        public ImageSource PictureIcon => this.ToIcon(Properties.Resources.sdkmenuicon);
        public string LeftMenuTitle => "vJoy Actions";

        public void End(PluginManager pluginManager)
        {
            this.SaveCommonSettings("VJoyAxisSettings3", this.settings);

            foreach (uint device in acquiredVJoyDevices)
            {
                vjoy.RelinquishVJD(device);
            }
            acquiredVJoyDevices.Clear();
            this.axisUpdateTimer?.Stop();
            this.axisUpdateTimer?.Dispose();
        }

        public void Init(PluginManager pluginManager)
        {
            SimHub.Logging.Current.Info("ZJoy v1.1.0.1");

            this.settings = this.ReadCommonSettings<ZJoyActionsSettings>("VJoyAxisSettings3", () => ZJoyActionsSettings.newBlankSettings());

            if (this.settings.Expresions[0] == null)
            {
                SimHub.Logging.Current.Error("Just created new settings and they were null");
            }

            if (vjoy.vJoyEnabled())
            {
                for (uint device = 1; device <= 16; device++)
                {
                    if (vjoy.isVJDExists(device))
                    {
                        // Not using "ensureAcquired" here because this is just a temporary scan.
                        if (vjoy.AcquireVJD(device))
                        {
                            try
                            {
                                uint d = device;
                                // buttons
                                for (uint button = 1; button <= vjoy.GetVJDButtonNumber(device); button++)
                                {
                                    uint b = button;

                                    pluginManager.AddInputMapping<ZJoyActionsPlugin>(String.Format("Joy{0}_Button{1}", d, b), (pm, s) =>
                                    {
                                        if (ensureAcquired(d))
                                        {
                                            vjoy.SetBtn(true, d, b);
                                        }
                                    }, (pm, s) =>
                                    {

                                        if (ensureAcquired(d))
                                        {
                                            vjoy.SetBtn(false, d, b);
                                        }
                                    });
                                }

                                // TODO: pov hats
                                // Need a way to keep track of whether another direction has been pushed before the previous one was released. Since POV hats can only be pushed in one direction at a time, we don't want the "release" event of the first direction to cancel out the "pressed" event of the second one.
                            }
                            finally
                            {
                                vjoy.RelinquishVJD(device);
                            }
                        }
                    }
                }

                this.axisUpdateTimer = new Timer();
                this.axisUpdateTimer.Interval = 1000.0 / 30.0;
                this.axisUpdateTimer.Elapsed += AxisUpdateTimer_Elapsed;
                this.axisUpdateTimer.Start();
            }
        }

        private void AxisUpdateTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            // TODO: Instead of looping through and updating all of these, we could keep track of which expressions were null, updating it only when the settings change.
            // However, it's only 256 checks max, and it short-circuits pretty early in the process if there's no vjoy device, axis, or if the expression was null. I just don't know how performant ExpressionValue.ExpressionIsNullOrWhiteSpace is.
            // Also, caching this information would require restarting SimHub if the user reconfigures vJoy to add devices or axes, since we can't detect that AFAIK

            updateAxisExpressions(settings.Expresions, ZJoyActionsSettings.AXIS_ORDER);
        }

        private void updateAxisExpressions(ExpressionValue[] expressions, HID_USAGES[] axisOrder)
        {
            for(uint e=0; e<expressions.Length; e++)
            {
                HID_USAGES axis = axisOrder[e/16];
                uint device = (e % 16) + 1;
                ExpressionValue expression = expressions[e];

                if (!expression.ExpressionIsNullOrWhiteSpace)
                {
                    if (ensureAcquired(device) && vjoy.GetVJDAxisExist(device, axis))
                    {
                        // axis is 0-0x7FFF
                        var expResult = this.nCalcEngine.ParseValue(expression);

                        double result = double.NaN;
                        if (expResult is double d)
                        {
                            result = d;
                        }
                        else if (expResult is float f)
                        {
                            result = (double)f;
                        }
                        else if (expResult is int i)
                        {
                            result = (double)(i) / ((double)0x8000);
                        }
                        else if (expResult is string s)
                        {
                            try { result = Convert.ToDouble(s); } catch { }
                        }

                        if (!(double.IsNaN(result)))
                        {
                            int intAxisVal = (int)(Math.Min(1.0, Math.Max(0.0, result)) * ((double)0x7FFF));
                            vjoy.SetAxis(intAxisVal, device, axis);
                        }
                    }
                }
            }
        }

        public Control GetWPFSettingsControl(PluginManager pluginManager)
        {
            return new ZJoyActionsSettingsControl(this);
        }
    }
}