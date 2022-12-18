using System;
using System . Collections . Generic;
using System . Configuration;
using System . Diagnostics;
using System . Security . Cryptography . Xml;
using System . Text;

namespace NewWpfDev
{
    public static class AppSettingsHandler
    {
        public static List<string> ReadAllSettings ( )
        {
            List<string> list = new List<string> ( );
            try
            {
                var appSettings = ConfigurationManager . AppSettings;

                if ( appSettings . Count == 0 )
                {
                    Debug . WriteLine ( $"AppSettings is empty." );
                }
                else
                {
                    foreach ( var key in appSettings . AllKeys )
                    {
                        Debug . WriteLine ( "Key: {0} Value: {1}" , key , appSettings [ key ] );
                        string str = $"{key}, { appSettings [ key ]}";
                        list . Add ( str);
                    }
                }
            }
            catch ( ConfigurationErrorsException )
            {
                Debug . WriteLine ( "Error reading app settings" );
            }
            return list;
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
                configFile . Save ( ConfigurationSaveMode . Full );
                ConfigurationManager . RefreshSection ( configFile . AppSettings . SectionInformation . Name );
            }
            catch ( ConfigurationErrorsException )
            {
                Debug . WriteLine ( "Error writing app settings" );
            }
        }
    }
}
