using System;
using System . Collections . Generic;
using System . Configuration;
using System . Diagnostics;
using System . Text;

namespace ConfigSettings
{
    public static class AppSettingsHandler
    {
        public static void ReadAllSettings ( )
        {
            try
            {
                var appSettings = ConfigurationManager . AppSettings;

                if ( appSettings . Count == 0 )
                {
                    Debug . WriteLine($"AppSettings is empty.");
                }
                else
                {
                    foreach ( var key in appSettings . AllKeys )
                    {
                        Debug . WriteLine ( "Key: {0} Value: {1}" , key , appSettings [ key ] );
                    }
                }
            }
            catch ( ConfigurationErrorsException )
            {
                Debug . WriteLine ( "Error reading app settings" );
            }
        }

        public static string ReadSetting ( string key )
        {
            try
            {
                var appSettings = ConfigurationManager . AppSettings;
                string result = appSettings [ key ];
                return result;
            }
            catch ( ConfigurationErrorsException )
            {
                Debug . WriteLine ( "Error reading app settings" );
            }
            return "";
        }

        public static void AddUpdateAppSettings ( string key , string value )
        {
            try
            {
                var configFile = ConfigurationManager . OpenExeConfiguration ( ConfigurationUserLevel . None );
                var settings = configFile . AppSettings . Settings;
                if ( settings [ key ] == null )
                {
                    settings . Add ( key , value );
                }
                else
                {
                    settings [ key ] . Value = value;
                }
                configFile . Save ( ConfigurationSaveMode . Modified );
                ConfigurationManager . RefreshSection ( configFile . AppSettings . SectionInformation . Name );
            }
            catch ( ConfigurationErrorsException )
            {
                Debug . WriteLine ( "Error writing app settings" );
            }
        }
    }
}
