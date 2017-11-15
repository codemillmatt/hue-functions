using System;
using Plugin.Settings;
using Plugin.Settings.Abstractions;

namespace BusyLights
{
    public static class Settings
    {
        static ISettings AppSettings => CrossSettings.Current;

        readonly static string _HueKey = "hue_key";
        readonly static string _IWasHere = "i_was_here_key";

        public static string HueKey
        {
            get => AppSettings.GetValueOrDefault(_HueKey, "");
            set => AppSettings.AddOrUpdateValue(_HueKey, value);
        }

        public static int IWasHere
        {
            get => AppSettings.GetValueOrDefault(_IWasHere, 0);
            set => AppSettings.AddOrUpdateValue(_IWasHere, value);
        }
    }
}
