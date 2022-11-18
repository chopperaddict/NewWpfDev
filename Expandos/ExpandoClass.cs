using System;
using System . Collections . Generic;
using System . Diagnostics;
using System . Dynamic;
using System . Text;

using Views;

using static IronPython . Modules . _ast;


namespace Expandos
{
    public class ExpandoClass
    {
        public static ExpandoObject expobj = new ExpandoObject ( );

        #region Expando CRUD handlers

        // Constructor()
        public static ExpandoObject GetNewExpandoObject ( )
        {
            expobj = new ExpandoObject ( );
            return expobj;
        }
       public static ExpandoObject ToExpandoObject ( string key , object value )
        {
            ExpandoObject expobj = new ExpandoObject ( );
            IDictionary<string , object> dict = new ExpandoObject ( );
            dict . Add ( key , value );

            if ( expobj . TryAdd ( key , dict ) == true )
                return expobj;

            else return null;
        }

        public static ExpandoObject expobjAdd ( ExpandoObject obj , string name , object value , out bool success )
        {
            Dictionary<string , object> dict = new Dictionary<string , object> ( );
            success = false;
            // add single item into Expando Object
            if ( obj . TryAdd ( name , dict ) == true )
                success = true;
            return obj;
        }
        public static bool expobjDel ( ExpandoObject dyn , string name , object value )
        {
            try
            {
                // Save default (original object)
                ExpandoObject newdyno = dyn;
                // remove single item from Expando Object
                foreach ( KeyValuePair<string , object> item in dyn )
                {
                    if ( item . Key . ToString ( ) . ToUpper ( ) != value . ToString ( ) . ToUpper ( ) )
                    {
                        object val = null;
                        dyn . Remove ( item . Key , out val );
                        return true;
                    }
                }
            }
            catch ( Exception ex )
            {
                Debug . WriteLine ( $"expobjDel failed : {ex . Message}" );
            }
            return false;
        }
        public static ExpandoObject expobjFind ( ExpandoObject dyn , string name )
        {
            // Find item in Expando Object
            try
            {
                Dictionary<string , object> dict = new Dictionary<string , object> ( );
                foreach ( KeyValuePair<string , object> item in dyn )
                {
                    if ( item . Key . ToString ( ) . ToUpper ( ) == name . ToUpper ( ) )
                    {
                        ExpandoObject obj = new ExpandoObject ( );
                        AddDictionaryEntry ( dyn , dict , item . Key . ToString ( ) , ( object ) item . Value );
                        // return result as an  ExpandoObject containing a single entry (the one found)
                        if ( obj . TryAdd ( name , dict ) == true )
                            return obj;
                        else return null;
                    }
                }
            }
            catch ( Exception ex ) { Debug . WriteLine ( $"expobjFind failed : {ex . Message}" ); }
            return null;
        }

        #endregion Expando CRUD handlers

        #region Expando support  handlers

        public static ExpandoObject AddDictionaryEntry ( ExpandoObject expo , Dictionary<string , object> dictionary , string key , object value )
        {
            //if ( dictionary == null )
            //{
            //    throw new ArgumentNullException ( nameof ( dictionary ) );
            //}
            Dictionary<string , object> dict = new Dictionary<string , object> ( );

            dict . Add ( key , value );
            if ( expo . TryAdd ( key , dict ) == true )
                return expo;
            //}
            return null;
        }
        
        #endregion Expando support  handlers
        
        private dynamic TestExpando ( )
        {
            // set it to default
            string str1 = "";
            string str2 = "";
            try
            {
                ExpandoObject dynobj = Genericgrid.Gengridexpobj;
                if ( Genericgrid .Gengridexpobj == null )
                {
                    Genericgrid .Gengridexpobj = ExpandoClass . GetNewExpandoObject ( );
                    dynobj = Genericgrid . Gengridexpobj;
                }
                Debug . WriteLine ( $"dynobj = {dynobj . ToString ( )}" );
                bool success = false;
                Genericgrid .Gengridexpobj = ExpandoClass . expobjAdd ( Genericgrid .Gengridexpobj , "Phone" , ( object ) "0757 9062440" , out success );
                if ( success )
                {
                    foreach ( var item in Genericgrid .Gengridexpobj )
                    {
                        str1 = item . Key . ToString ( );
                        str2 = item . Value . ToString ( );
                        Debug . WriteLine ( $"dynobj.Added = {str1}  : {str2}" );
                    }
                    Genericgrid.Gengridexpobj = ExpandoClass . expobjFind ( Genericgrid .Gengridexpobj , "Phone" );
                    foreach ( var item in Genericgrid.Gengridexpobj )
                    {
                        str1 = item . Key . ToString ( );
                        str2 = item . Value . ToString ( );
                        Debug . WriteLine ( $"dynobjFind() returned :  {str1}  : {str2}" );
                    }

                    if ( Genericgrid.Gengridexpobj = ExpandoClass . expobjDel ( Genericgrid.Gengridexpobj , "Phone" , ( object ) "0757 9062440" ) == false )
                        Debug . WriteLine ( $"dynobjDel() returned :  FALSE, deletion of \"Phone\" failed" );
                }
                return dynobj;
            }
            catch ( Exception ex ) { Debug . WriteLine ( $"Expando in testexpand() failed : {ex . Message}" ); }
            return null;
        }

    }
}
