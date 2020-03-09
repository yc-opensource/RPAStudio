using Fluent;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xceed.Wpf.Toolkit;

namespace RPAStudio.Helpers
{
    public static class SettingsManager
    {
        public static string GetUserLanguageOrDefault()
        {
            var language = Properties.Settings.Default.Language;
            if (!string.IsNullOrWhiteSpace(language) &&
                IsSupportedLanguage(language))
            {
                return language;
            }
            return App.SupportedLanguages.First().Name;
        }

        public static bool SetLanguage(string language)
        {
            if (IsSupportedLanguage(language))
            {
                Properties.Settings.Default.Language = language;
                SaveSettings();
                return true;
            }
            return false;
        }

        public static (bool autoSaveIsEnabled, int autoSaveSeconds) GetAutoSaveSettings()
            => (Properties.Settings.Default.AutoSaveIsEnabled, Properties.Settings.Default.AutoSaveSeconds);

        public static string GetUserThemeOrDefault()
        {
            var themeName = Properties.Settings.Default.Theme;
            Theme theme = null;
            if (!string.IsNullOrWhiteSpace(themeName))
            {
                theme = ThemeManager.GetTheme(themeName);
            }

            return (theme ?? ThemeManager.GetTheme("MoRPA Studio")).Name;
        }

        public static bool SetTheme(Theme theme)
        {
            if (theme == null)
            {
                return false;
            }
            Properties.Settings.Default.Theme = theme.Name;
            SaveSettings();
            return true;
        }

        private static void SaveSettings()
        {
            Properties.Settings.Default.Save();
        }

        public static bool IsSupportedLanguage(string langName)
        {
            return App.SupportedLanguages.Any(s => s.Name == langName);
        }

        internal static void SetAutoSaveEnabledValue(bool value)
        {
            Properties.Settings.Default.AutoSaveIsEnabled = value;
            SaveSettings();
        }

        internal static void SetAutoSaveSecondsValue(int value)
        {
            Properties.Settings.Default.AutoSaveSeconds = value;
            SaveSettings();
        }
    }
}
