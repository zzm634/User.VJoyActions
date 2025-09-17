
using SimHub.Plugins;
using System;
using System.Collections.Generic;
using vJoyInterfaceWrap;

namespace User.VJoy
{
    [PluginDescription("Add VJoy Buttons as mappable Actions")]
    [PluginAuthor("Zach Miller")]
    [PluginName("VJoy Actions")]
    public class VJoyActionsPlugin : IPlugin
    {
        private HashSet<uint> acquiredVJoyDevices = new HashSet<uint>();

        private vJoy vjoy = new vJoy();

        private bool ensureAcquired(uint vJoyDeviceId)
        {
            if (!acquiredVJoyDevices.Contains(vJoyDeviceId))
            {
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

        public void End(PluginManager pluginManager)
        {
            foreach (uint device in acquiredVJoyDevices)
            {
                vjoy.RelinquishVJD(device);
            }
            acquiredVJoyDevices.Clear();
        }

        public void Init(PluginManager pluginManager)
        {

            if (vjoy.vJoyEnabled())
            {
                for (uint device = 1; device <= 16; device++)
                {
                    if (vjoy.isVJDExists(device))
                    {
                        vjoy.AcquireVJD(device);
                        uint d = device;
                        // buttons
                        for (uint button = 1; button <= vjoy.GetVJDButtonNumber(device); button++)
                        {
                            uint b = button;

                            pluginManager.AddInputMapping<VJoyActionsPlugin>(String.Format("Joy{0}_Button{1}", d, b), (pm, s) =>
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

                        vjoy.RelinquishVJD(device);
                    }
                }
            }
        }
    }
}