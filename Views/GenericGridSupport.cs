using System;
using System . Collections . Generic;
using System . Collections . ObjectModel;
using System . Data;
using System . Data . SqlClient;
using System . Diagnostics;
using System . Windows;
using System . Windows . Controls;
using System . Windows . Input;

using Dapper;

using DocumentFormat . OpenXml . Drawing;

using NewWpfDev;
using NewWpfDev . Models;
using NewWpfDev . ViewModels;

using UserControls;

namespace Views
{

    public class GenericGridSupport
    {

        public struct StructurefieldInfo
        {
            public string Name;
            public string Type;
            public string Decroot;
            public string Decpart;
        }
        static public StructurefieldInfo structurefieldInfo = new StructurefieldInfo ( );
        static public List<StructurefieldInfo> FieldInfo = new List<StructurefieldInfo> ( );
        static public ObservableCollection<DapperGenericsLib . GenericClass> TableStructure = new ObservableCollection<DapperGenericsLib . GenericClass> ( );
        static public DatagridControl dgControl;
        static public Genericgrid GenericControl;
        static public DataGrid Gengrid;

        public GenericGridSupport ( )
        {
            GenericControl = Genericgrid . GenControl;
        }
        static public void SetPointers ( DatagridControl dg , Genericgrid geng )
        {
            // setup master pointer to controls
            if ( dg != null )
            {
                dgControl = dg;
                Gengrid = dg . datagridControl;
            }
            if ( geng != null )
            {
                GenericControl = geng;
            }
        }
        static public List<string> CallStoredProcedure ( List<string> list , string sqlcommand )
        {
            //This call returns us a DataTable
            DataTable dt = dgControl . GetDataTable ( sqlcommand );
            if ( dt != null )
                list = GetDataDridRowsAsListOfStrings ( dt );
            return list;
        }
        public static List<string> GetDataDridRowsAsListOfStrings ( DataTable dt )
        {
            List<string> list = new List<string> ( );
            foreach ( DataRow row in dt . Rows )
            {
                var txt = row . Field<string> ( 0 );

                list . Add ( txt );
            }
            return list;
        }

        //***************************************************//
        // support method  for  saving selected items onlly
        //***************************************************//
        static public int CreateNewTable ( string NewDbName , string CurrentTable , ObservableCollection<DapperGenericsLib . GenericClass> collection , out string err )
        {
            // All working WELL 30/9/2022
            err = "";
            string [ ] args2 = { "" , "" , "" , "" , "" , "" , "" , "" , "" , "" , "" , "" , "" , "" , "" , "" , "" , "" , "" , "" };
            args2 [ 0 ] = $"{NewDbName}";
            // args2 [ 1 ] = $"{CurrentTable}";
            string flddata = "";
            bool success = false;

            try
            {
                int elementcount = 0;
                for ( int row = 0 ; row < FieldInfo . Count ; row++ )
                {
                    elementcount = 0;
                    flddata = "";
                    StructurefieldInfo fldinfo = FieldInfo [ row ];
                    flddata += $" {fldinfo . Name}";

                    //Type
                    if ( fldinfo . Type != "" )
                        flddata += $" {fldinfo . Type}";

                    // Dec Root
                    if ( fldinfo . Decroot != "" )
                    {
                        if ( fldinfo . Type . ToUpper ( ) == "DECIMAL"
                             || fldinfo . Type . ToUpper ( ) == "DOUBLE"
                                || fldinfo . Type . ToUpper ( ) == "FLOAT"
                                || fldinfo . Type . ToUpper ( ) == "DOUBLE_PRECISION"
                                || fldinfo . Type . ToUpper ( ) == "DEC" )
                            flddata += $" ({fldinfo . Decroot} ";
                        else if ( fldinfo . Type . ToUpper ( ) == "INT"
                            || fldinfo . Type . ToUpper ( ) == "BOOL"
                            || fldinfo . Type . ToUpper ( ) == "SMALLINT" )
                        {
                            //flddata += $" ( {fldinfo . Decroot} ) ";
                            if ( MainWindow . USE_ID_IDENTITY )
                                flddata += $" IDENTITY(1,1) NOT NULL";
                            else
                                flddata += $" NOT NULL";
                        }
                        else if ( fldinfo . Type . ToUpper ( ) == "SMALLINT"
                            || fldinfo . Type . ToUpper ( ) == "TINYINT"
                            || fldinfo . Type . ToUpper ( ) == "BIT"
                            || fldinfo . Type . ToUpper ( ) == "INT"
                            || fldinfo . Type . ToUpper ( ) == "MEDIUMINT" )
                        {
                            flddata += $" ( {fldinfo . Decroot} ) ";
                        }
                        else if ( fldinfo . Type . ToUpper ( ) == "DATETIME"
                            || fldinfo . Type . ToUpper ( ) == "TIMESTAMP"
                            || fldinfo . Type . ToUpper ( ) == "TIME"
                            || fldinfo . Type . ToUpper ( ) == "MEDIUMINT" )
                        {
                            if ( fldinfo . Decroot != "" )
                                flddata += $" ( {fldinfo . Decroot} ) ";
                            else
                                flddata += $"  ";
                        }
                        else if ( fldinfo . Type . ToUpper ( ) == "VARCHAR"
                            || fldinfo . Type . ToUpper ( ) == "CHAR"
                            || fldinfo . Type . ToUpper ( ) == "NCHAR"
                            || fldinfo . Type . ToUpper ( ) == "NVARCHAR"
                            || fldinfo . Type . ToUpper ( ) == "VARBINARY" )
                        {
                            if ( fldinfo . Decroot != "" )
                                flddata += $" ( {fldinfo . Decroot} ) ";
                            else
                                flddata += $" ";
                        }
                        else
                            flddata += $"  ";
                    }
                    else
                        flddata += $"";

                    // Dec Part 
                    if ( fldinfo . Decpart != "" )
                    {
                        if ( fldinfo . Decpart != "" && fldinfo . Decpart != "0" )
                        {
                            if ( fldinfo . Type . ToUpper ( ) == "DECIMAL"
                                || fldinfo . Type . ToUpper ( ) == "DOUBLE"
                                || fldinfo . Type . ToUpper ( ) == "FLOAT"
                                || fldinfo . Type . ToUpper ( ) == "DOUBLE_PRECISION"
                                || fldinfo . Type . ToUpper ( ) == "DEC" )
                                flddata += $", {fldinfo . Decpart} ) ";
                            else
                                flddata += $" ";
                        }
                        else
                            flddata += $" ";
                    }
                    else
                    {
                        if ( fldinfo . Type . ToUpper ( ) == "VARCHAR"
                            || fldinfo . Type . ToUpper ( ) == "CHAR"
                            || fldinfo . Type . ToUpper ( ) == "NCHAR"
                            || fldinfo . Type . ToUpper ( ) == "NVARCHAR"
                            || fldinfo . Type . ToUpper ( ) == "VARBINARY" )
                            flddata += $" ";
                        else if ( fldinfo . Type . ToUpper ( ) == "DATETIME" || fldinfo . Type . ToUpper ( ) == "DATE" )
                            flddata += $" ";
                        else
                            flddata += $") ";
                    }

                    if ( row >= collection . Count )
                        args2 [ row + 1 ] = $"{flddata}, ";
                    else
                        args2 [ row + 1 ] = $"{flddata}, ";
                    if ( args2 [ row + 1 ] . Contains ( ", ," ) ) args2 [ row + 1 ] = args2 [ row + 1 ] . Substring ( 0 , args2 [ row + 1 ] . Length - 4 );
                    args2 [ row + 1 ] = NewWpfDev . Utils . ReverseString ( args2 [ row + 1 ] . Trim ( ) );
                    if ( args2 [ row + 1 ] [ 0 ] != ',' ) args2 [ row + 1 ] = $",{args2 [ row + 1 ] . Substring ( 1 )}";
                    args2 [ row + 1 ] = NewWpfDev . Utils . ReverseString ( args2 [ row + 1 ] );
                    if ( row + 1 >= FieldInfo . Count )
                        args2 [ row + 1 ] = args2 [ row + 1 ] . Substring ( 0 , args2 [ row + 1 ] . Length - 1 );
                }
                // now working 24/9/2022
                //**********************************//
                // now Create new table structure as a  single string for sql
                //**********************************//
                string [ ] args = { "" , "" };
                args [ 0 ] = args2 [ 0 ];
                args [ 1 ] = CreateSqlTablestructureString ( args2 );

                //**********************************//
                // now Create new table structure
                //**********************************//
                //while ( true )
                //{   2nd call
                int recordcount = 0;
                string Tablename = "";
                recordcount = dgControl . ExecuteStoredProcedure ( "CreateTableTest" , args , out err );
                if ( err != "" && err . ToUpper ( ) . Contains ( "PROBLEM-FILE  EXISTS" ) == false )
                {
                    Debug . WriteLine ( $"Table creation failed [{err}]" );
                    MessageBoxResult result2 = MessageBox . Show ( $"A table with the selected name already exists in the database.\nDo you want to overwrite it with the newly selected data ?\n\nError message  ={err}" , "" ,
                        MessageBoxButton . YesNoCancel , MessageBoxImage . Question , MessageBoxResult . No );
                    if ( result2 == MessageBoxResult . Yes )
                    {
                        string [ ] args3 = { "" };
                        err = "";
                        args3 [ 0 ] = NewDbName;
                        recordcount = dgControl . ExecuteDapperCommand ( "Drop Table if exists " , args3 , out err );
                        if ( err != "" && err . ToUpper ( ) . Contains ( "PROBLEM-FILE  EXISTS" ) == false )
                        {
                            MessageBoxResult result3 = MessageBox . Show ( $"Failed to delete (Drop) the original table, so processing will now end\n Error message  ={err}" , "Deletion failed" ,
                                MessageBoxButton . OK , MessageBoxImage . Error , MessageBoxResult . OK );
                        }
                        else
                        {
                            recordcount = dgControl . ExecuteStoredProcedure ( "CreateTableTest" , args , out err );
                            GenericControl . statusbar . Text = $"New table named [{NewDbName . ToUpper ( )}] was created succesfully...."; Debug . WriteLine ( $"{GenericControl . statusbar . Text}" );
                            success = true;
                        }
                    }
                }
                else
                {
                    // Table created successfulllly !!!!!!
                    // Add data here 
                    GenericControl . statusbar . Text = $"New table named [{NewDbName . ToUpper ( )}] was created succesfully...."; Debug . WriteLine ( $"{GenericControl . statusbar . Text}" );
                    success = true;
                }
            }
            catch ( Exception ex )
            {
                Debug . WriteLine ( $"{ex . Message}" );
            }
            if ( success == true )
                return -1;
            else
                return -8;
        }

        // saveselected support methods

        static public bool SqlInsertGenericRecord ( string DataString , string fldnames , string NewDbName , out string err , out int recsinserted )
        {
            err = "";
            recsinserted = 0;
            bool success = true;
            // Insert  a single record into SQL table 'NewDbName'
            Dictionary<string , string> dict = new Dictionary<string , string> ( );
            string err2 = "";

            Debug . WriteLine ( $"Inserting new record containing\n[{fldnames} + \n{DataString} ]\n" );
            SqlConnection sqlCon = null;
            string ConString = GenericDbUtilities . CheckSetSqlDomain ( "" );
            if ( ConString == "" )
            {
                GenericDbUtilities . CheckDbDomain ( "" );
                ConString = MainWindow . CurrentSqlTableDomain;
            }
            string Con = ConString;
            string argument2 = "";
            argument2 += $" {fldnames . Trim ( )} values {DataString . Trim ( )}";
            try
            {
                using ( sqlCon = new SqlConnection ( Con ) )
                {
                    string cmd = "";
                    sqlCon . Open ( );
                    // Now add argument(s)  to SQL command
                    var parameters = new DynamicParameters ( );
                    parameters . Add ( $"Arg1" , $"{NewDbName}" ,
                       DbType . String ,
                       ParameterDirection . Input ,
                       NewDbName . Length );
                    parameters . Add ( $"Arg2" , argument2 ,
                       DbType . String ,
                       ParameterDirection . Input ,
                       argument2 . Length );

                    try
                    {
                        int res3 = sqlCon . Execute ( "spInsertGenericRecord" , parameters , commandType: CommandType . StoredProcedure );
                        $"RESULT of  [{DataString}] = {res3} (Success)" . CW ( );
                        recsinserted++;
                        success = true;
                    }
                    catch ( Exception ex )
                    {
                        $"Record Insertion failed = {ex . Message}" . CW ( );
                        err = $"Record Insertion failed = {ex . Message}";
                        success = false;
                    }
                }
                return success;
            }
            catch ( Exception ex )
            {
                Debug . WriteLine ( $"{ex . Message}" );
                return false;
            }
            return true;
        }

        public bool PerformSingleRecInsert ( string processQuery , Dictionary<string , string> dict , out string err )
        {
            string ConString = GenericDbUtilities . CheckSetSqlDomain ( "" );
            if ( ConString == "" )
            {
                GenericDbUtilities . CheckDbDomain ( "" );
                ConString = MainWindow . CurrentSqlTableDomain;
            }
            string Con = ConString;
            int index = 0;
            Mouse . OverrideCursor = Cursors . Wait;
            SqlConnection sqlCon = null;
            Debug . WriteLine ( $"Running Stored Procedure {processQuery}" );
            using ( sqlCon = new SqlConnection ( Con ) )
            {
                string cmd = "";
                sqlCon . Open ( );
                // Now add argument(s)  to SQL command
                var parameters = new DynamicParameters ( );
                foreach ( var key in dict )
                    if ( key . Key . Length > 0 && key . Key != "-" )
                    {
                        string fldname = key . Key . ToString ( );
                        parameters . Add ( $"Arg[{index++}]," , $"{fldname}" ,
                       DbType . String ,
                       ParameterDirection . Input ,
                       fldname . Length );
                    }
                err = "";
                try
                {
                    int result = sqlCon . Execute ( processQuery , parameters , commandType: CommandType . Text );
                    Debug . WriteLine ( $"RESULT of  [{processQuery}] = {result} (Success)" );
                }
                catch ( Exception ex )
                {
                    Debug . WriteLine ( $"{ex . Message}" );
                }
            }
            return true;
        }

        static public string CreateNewSqlTableStructure ( string CurrentTable , string NewDbName , ObservableCollection<DapperGenericsLib . GenericClass> collection , out string err )
        {
            // Called iteratively by CreateNewTableStructure ( ) for each record selected
            // Creates && Returns the list of fields in SQL INSERT format
            // and  also creates the new table
            bool GetFullDetails = true;
            string SqlDataString = "";
            string InsertFieldsString = "";
            err = "";
            if ( GetFullDetails )
            {
                collection . Clear ( );

                //*****************************//
                // Get full column info with sizes
                //*****************************//
                string ConString = GenericDbUtilities . CheckSetSqlDomain ( "" );
                if ( ConString == "" )
                {
                    GenericDbUtilities . CheckDbDomain ( "" );
                    ConString = MainWindow . CurrentSqlTableDomain;
                }

                SqlDataString = dgControl . GetFullColumnInfo ( NewDbName , CurrentTable , ConString , false , false );
                // SqlDataString  has column info delimited  by " \n"
                SqlDataString = SqlDataString . Trim ( );
                string [ ] args = SqlDataString . Split ( "\n" );
                string buffer = "";
                FieldInfo . Clear ( );
                collection . Clear ( );
                //**********************************************//
                // create new collection of all structure info from args
                //which has valid column info for the current table
                //**********************************************//
                DapperGenericsLib . GenericClass tem = new DapperGenericsLib . GenericClass ( );
                for ( int y = 0 ; y < args . Length ; y++ )
                {
                    string [ ] RawFldNames = args [ y ] . Split ( ',' );
                    for ( int z = 0 ; z < RawFldNames . Length ; z++ )
                    {
                        tem = new DapperGenericsLib . GenericClass ( );
                        tem . field1 = RawFldNames [ 0 ] . ToUpper ( );    // fname
                        tem . field2 = RawFldNames [ 1 ];   //ftype
                        if ( RawFldNames . Length >= 3 ) tem . field3 = RawFldNames [ 2 ];   // decroot
                        else tem . field3 = "";
                        if ( RawFldNames . Length >= 4 ) tem . field4 = RawFldNames [ 3 ];   // decpart
                        else tem . field4 = "";
                        structurefieldInfo . Name = tem . field1;
                        structurefieldInfo . Type = tem . field2;
                        structurefieldInfo . Decroot = tem . field3;
                        structurefieldInfo . Decpart = tem . field4;
                    }
                    FieldInfo . Add ( structurefieldInfo );
                    collection . Add ( tem );
                    InsertFieldsString += $"{structurefieldInfo . Name},";
                }
            }
            //remove final comma
            InsertFieldsString = InsertFieldsString . TrimEnd ( );
            InsertFieldsString = InsertFieldsString . Substring ( 0 , InsertFieldsString . Length - 1 );
            InsertFieldsString = $"({InsertFieldsString})";

            //**************************************************************************************//
            // collection is obscollection<DapperGenericsLib.GenericClass> contains true field names + types + sizes
            // so lets create table structure here !
            //**************************************************************************************//
            int res = CreateNewTable ( NewDbName , CurrentTable , collection , out err );
            if ( err != "" || res == -8 )
            {
                MessageBox . Show ( $"An error occurred while trying to create the table {NewDbName . ToUpper ( )}\nProcessing has been cancelled" , "Table Creation" );
                err = "Table creation failed....";
                return "";
            }
            return InsertFieldsString;  // success
        }
        public int ProcessSelectedRows ( out int recsinserted , string CurrentTable , bool ShowDuplicateErrors = true )
        {
            // process to save all selected records to specified SQL table
            List<string> FldNames = new List<string> ( );
            List<string> selrecords = new List<string> ( );
            recsinserted = 0;
            ObservableCollection<DapperGenericsLib . GenericClass> collection = new ObservableCollection<DapperGenericsLib . GenericClass> ( );
            string NewDbName = GenericControl . NewTableName . Text . Trim ( );
            CurrentTable = NewDbName;
            // Save a set with only selected rows included
            string SqlDataString = "";

            if ( dgControl . datagridControl . SelectedItems . Count == 1 )
            {
                MessageBoxResult res2 = MessageBox . Show ( $"Only a single record is selected ?  \nDo you really want to save it to a new and seperate SQL Table ?" , "Selected Record(s) Question ?" ,
                    MessageBoxButton . YesNo , MessageBoxImage . Question , MessageBoxResult . No );
                if ( res2 == MessageBoxResult . No ) return -1;
            }

            if ( NewDbName == "" )  // Sanity check
            {
                MessageBox . Show ( "Please enter a suitable name for the table you want to create !" , "Table name required" );
                return -9;
            }
            //"1 - Started in ProcessSelectedRows()" . CW ( );
            // Now check to ensure we do have the table on disk already
            string err = "";
            string [ ] args = { $"{NewDbName}" };

            #region Check Table Exists

            //********************************************************************//
            // Ckeck if new table already exists - works well 26/9/2022
            //********************************************************************//
            int ret = DatagridControl . TestIfFileExists ( NewDbName , out err );
            if ( ret > 0 && err != "successs" )
            {
                Debug . WriteLine ( $"Table {NewDbName} already Exists." );
                MessageBoxResult res3 = MessageBox . Show ( $"The table named [{NewDbName . ToUpper ( )}] already exists\n YES will overwrite it annd add selected data  to it\nNO will cancel processing to let you enter a new table name??" , "File already Exists" ,
                  MessageBoxButton . YesNo , MessageBoxImage . Warning , MessageBoxResult . Yes );
                if ( res3 == MessageBoxResult . Yes )
                {
                    int retv = DropTable ( NewDbName , CurrentTable , out err );
                    if ( err != "" )
                    {
                        Debug . WriteLine ( $"{err}" );
                        return -9;
                    }
                    else
                        Debug . WriteLine ( $"{NewDbName} does not exist. so we are carrying on" );
                }
                else
                    return -9;
            }
            #endregion Check Table Exists

            #region Create new table

            //********************************************************************************//
            //  All well, table does not exist, so create it here based on currently open table- 1ST TRY
            //********************************************************************************//
            string insertfieldsstring = CreateNewSqlTableStructure ( GenericControl . SqlTables . SelectedItem . ToString ( ) , NewDbName , collection , out err );
            if ( err != "" || insertfieldsstring == "" )
            {
                Debug . WriteLine ( $"ERROR : {err}" );
                return -8;
            }
            else
            {
                // We  now have a nice SQL Insert fields string in insertfieldsstring
                Debug . WriteLine ( $"Table Creation for {NewDbName} was completed  successfully" );
                GenericControl . statusbar . Text = $"Table Creation for {NewDbName} was completed  successfully";
                //****************************************************************************//
                // Grab stucture details for all fields so we can reference them later - 3rd call 
                //****************************************************************************//
                TableStructure = collection;
            }
            #endregion Create new table

            //****************************************************************************//
            // Now iiterate thru all selected records & get them inserted into the Sql table
            //****************************************************************************//

            err = "";
            string [ ] flds = { "" , "" , "" , "" , "" , "" , "" , "" , "" , "" , "" , "" , "" , "" , "" , "" , "" , "" , "" , "" };
            string datarow = "";
            int insertcount = 0;

            foreach ( object item4 in GenericControl . GenGridCtrl . datagridControl . SelectedItems )
            {

                #region  create Sql Data string
                //*****************************************************************************//
                // create Sql string of each record's data and call CreateSqlInsertCommand( )
                // to get Sql string for the fields  as  we iterate
                //*****************************************************************************//
                datarow = item4 . ToString ( );
                SqlDataString = "";
                Debug . WriteLine ( $"Item = {datarow}\n" );
                string part1 = "";
                string [ ] dataflds2 = { "" , "" };
                string [ ] dataflds = datarow . Split ( ',' );
                DapperGenericsLib . GenericClass gc = new DapperGenericsLib . GenericClass ( );
                for ( int cnt = 0 ; cnt < dataflds . Length ; cnt++ )
                {
                    part1 = dataflds [ cnt ];
                    dataflds2 = part1 . Split ( '=' );
                    dataflds2 [ 1 ] = dataflds2 [ 1 ] . Trim ( );
                    if ( dataflds2 [ 1 ] . Contains ( '}' ) )
                        dataflds2 [ 1 ] = dataflds2 [ 1 ] . Substring ( 0 , dataflds2 [ 1 ] . Length - 1 ) . Trim ( );
                    gc = TableStructure [ cnt ];

                    #region Data type checking
                    //*********************************************************************************//
                    // we now have the field name - so check its field type from DapperGenericsLib
                    // If it is not a  numeriic type, then wrap it in SQL single quote marks
                    //*********************************************************************************//
                    if ( gc . field2 . ToUpper ( ) == "CHAR" ||
                        gc . field2 . ToUpper ( ) == "VARCHAR" ||
                        gc . field2 . ToUpper ( ) == "NVARCHAR" ||
                        gc . field2 . ToUpper ( ) == "TEXT" ||
                        gc . field2 . ToUpper ( ) == "BLOB" ||
                        gc . field2 . ToUpper ( ) == "BINARY" ||
                        gc . field2 . ToUpper ( ) == "TINYTEXT" )
                    {   // its a string type, quotes are required
                        dataflds2 [ 1 ] = $"'{dataflds2 [ 1 ]}'";
                    }
                    else
                    {
                        // Its a non string type, quotes not required
                        dataflds2 [ 1 ] = dataflds2 [ 1 ];
                    }
                    //*********************************************************************************//
                    #endregion Data type checking

                    if ( dataflds2 [ 1 ] . Contains ( '/' ) )
                        dataflds2 [ 1 ] = NewWpfDev . Utils . ConvertInputDate ( dataflds2 [ 1 ] );
                    if ( cnt >= dataflds . Length )
                        dataflds2 [ 1 ] = dataflds2 [ 1 ] . Substring ( 0 , dataflds2 [ 1 ] . Length - 1 );
                    flds [ cnt ] = dataflds2 [ 1 ];
                }
                dataflds2 [ 1 ] = "(";
                for ( int x = 0 ; x < 20 ; x++ )
                {
                    if ( flds [ x ] == "" )
                    {
                        dataflds2 [ 1 ] = dataflds2 [ 1 ] . Substring ( 0 , dataflds2 [ 1 ] . Length - 1 );
                        break;
                    }
                    else
                        dataflds2 [ 1 ] += flds [ x ] + ",";
                }
                dataflds2 [ 1 ] = dataflds2 [ 1 ] . Trim ( ) + ")";
                SqlDataString = dataflds2 [ 1 ];
                Debug . WriteLine ( $"DATA= {SqlDataString}" );

                #endregion  create Sql Data string

                //***************************//
                // SsqlSqlDataString  CONTAINS FORMATTED DATA FOR A SINGLE RECORD
                // and we  already have THE FIELD'S STRING in insertfieldsstring
                // so we can now call Insert for new table which occurs from insertfieldsString()
                //***************************//
                GenericControl . statusbar . Text = $"Saving Selected Rows only to new  table named [{NewDbName . ToUpper ( )}]....";
                "3 - Calling CreateSqlInsertCommand ()" . CW ( );
                insertcount++;
                string res4 = "";
                // This call actualy inserts the new record data
                string insertfieldsString = CreateSqlInsertCommand ( NewDbName , SqlDataString , insertfieldsstring , args , "" , dgControl . datagridControl . Columns . Count , out res4 );
                if ( res4 == "" )
                {
                    $"4 - CREATESQLINSERTCOMMAND returned Insert string = \n[{insertfieldsString}]'" . CW ( );
                    Debug . WriteLine ( $"Record {insertcount} for {SqlDataString} was added to new table {NewDbName}" );
                    $"Record {insertcount} for {SqlDataString} was added to new table {NewDbName}" . CW ( );
                    recsinserted++;
                }
                else
                {
                    Debug . WriteLine ( $"Record {insertcount} for {SqlDataString} \nto new table {NewDbName} FAILED" );
                    $"Creation of field info string for {SqlDataString} FAILED\n{res4}" . CW ( );
                    return -9;
                }
                SqlDataString = "";
            }
            //   end of selected records processing
            return -1;
        }
        static public int DropTable ( string NewDbName , string CurrentTable , out string err )
        {
            err = "";
            string [ ] args = { "" };
            args [ 0 ] = NewDbName;
            //args [ 1 ] = CurrentTable;
            int recordcount = 0;
            string Tablename = "";
            recordcount = dgControl . ExecuteStoredProcedure ( "spDropTable" , args , out err );
            if ( err != "" )
            {
                err = $"Unable to Drop/Create the Table {args [ 0 ]} REASON = {err} ";
                return -9;
            }
            else MessageBox . Show ( $"Table {NewDbName} has been deleted successfully !" , "SQL tablle deletion" , MessageBoxButton . OK );
            return -1;
        }
        static public string CreateSqlTablestructureString ( string [ ] row )
        {
            string SqlDataString = "";
            bool alldone = false;
            string validflds = "";
            string data = "";
            "" . Track ( );

            for ( int q = 1 ; q <= 20 ; q++ )
            {
                if ( alldone ) break;
                switch ( q )
                {
                    case 1:
                        if ( row [ q ] == null || row [ q ] == "" ) alldone = true;
                        else data = $"{row [ q ]}";
                        continue;
                    case 2:
                        if ( row [ q ] == null || row [ q ] == "" ) alldone = true;
                        else data += $"{row [ q ]}";
                        continue;
                    case 3:
                        if ( row [ q ] == null || row [ q ] == "" ) alldone = true;
                        else data += $"{row [ q ]}";
                        continue;
                    case 4:
                        if ( row [ q ] == null || row [ q ] == "" ) alldone = true;
                        else if ( row [ q ] . Contains ( "," ) ) data += row [ q ] . Trim ( ) . Substring ( 0 , row [ q ] . Length - 1 ) + ",";
                        else data += $"{row [ q ]}";
                        continue;
                    case 5:
                        if ( row [ q ] == null || row [ q ] == "" ) alldone = true;
                        else data += $"{row [ q ]}";
                        continue;
                    case 6:
                        if ( row [ q ] == null || row [ q ] == "" ) alldone = true;
                        else data += $"{row [ q ]}";
                        continue;
                    case 7:
                        if ( row [ q ] == null || row [ q ] == "" ) alldone = true;
                        else data += $"{row [ q ]}";
                        continue;
                    case 8:
                        if ( row [ q ] == null || row [ q ] == "" ) alldone = true;
                        else data += $"{row [ q ]}";
                        continue;
                    case 9:
                        if ( row [ q ] == null || row [ q ] == "" ) alldone = true;
                        else data += $"{row [ q ]}";
                        continue;
                    case 10:
                        if ( row [ q ] == null || row [ q ] == "" ) alldone = true;
                        else data += $"{row [ q ]}";
                        continue;
                    case 11:
                        if ( row [ q ] == null || row [ q ] == "" ) alldone = true;
                        else data += $"{row [ q ]}";
                        continue;
                    case 12:
                        if ( row [ q ] == null || row [ q ] == "" ) alldone = true;
                        else data += $"{row [ q ]}";
                        continue;
                    case 13:
                        if ( row [ q ] == null || row [ q ] == "" ) alldone = true;
                        else data += $"{row [ q ]}";
                        continue;
                    case 14:
                        if ( row [ q ] == null || row [ q ] == "" ) alldone = true;
                        else data += $"{row [ q ]}";
                        continue;
                    case 15:
                        if ( row [ q ] == null || row [ q ] == "" ) alldone = true;
                        else data += $"{row [ q ]}";
                        continue;
                    case 16:
                        if ( row [ q ] == null || row [ q ] == "" ) alldone = true;
                        else data += $"{row [ q ]}";
                        continue;
                    case 17:
                        if ( row [ q ] == null || row [ q ] == "" ) alldone = true;
                        else data += $"{row [ q ]}";
                        continue;
                    case 18:
                        if ( row [ q ] == null || row [ q ] == "" ) alldone = true;
                        else data += $"{row [ q ]}";
                        continue;
                    case 19:
                        if ( row [ q ] == null || row [ q ] == "" ) alldone = true;
                        else data += $"{row [ q ]}";
                        continue;
                    case 20:
                        if ( row [ q ] == null || row [ q ] == "" ) alldone = true;
                        else data += $"{row [ q ]}";
                        continue;
                }
            }

            if ( data [ data . Length - 1 ] == ',' )
                data = data . Trim ( ) . Substring ( 0 , data . Length - 1 );

            return data;
        }
        static public string CreateDataString ( DapperGenericsLib . GenericClass row )
        {
            string SqlDataString = "";
            bool alldone = false;
            string validflds = "";
            string data = "";
            for ( int q = 1 ; q <= 20 ; q++ )
            {
                if ( alldone ) break;
                switch ( q )
                {
                    case 1:
                        if ( row . field1 == null ) alldone = true;
                        else data = $"{row . field1},";
                        continue;
                    case 2:
                        if ( row . field2 == null ) alldone = true;
                        else data += $"{row . field2},";
                        continue;
                    case 3:
                        if ( row . field3 == null ) alldone = true;
                        else data += $"{row . field3},";
                        continue;
                    case 4:
                        if ( row . field4 == null ) alldone = true;
                        else if ( row . field4 . Contains ( "," ) ) data += row . field4 . Trim ( ) . Substring ( 0 , row . field4 . Length - 1 ) + ",";
                        else data += $"{row . field4},";
                        continue;
                    case 5:
                        if ( row . field5 == null ) alldone = true;
                        else data += $"{row . field5},";
                        continue;
                    case 6:
                        if ( row . field6 == null ) alldone = true;
                        else data += $"{row . field6},";
                        continue;
                    case 7:
                        if ( row . field7 == null ) alldone = true;
                        else data += $"{row . field7},";
                        continue;
                    case 8:
                        if ( row . field8 == null ) alldone = true;
                        else data += $"{row . field8},";
                        continue;
                    case 9:
                        if ( row . field9 == null ) alldone = true;
                        else data += $"{row . field9},";
                        continue;
                    case 10:
                        if ( row . field10 == null ) alldone = true;
                        else data += $"{row . field10},";
                        continue;
                    case 11:
                        if ( row . field11 == null ) alldone = true;
                        else data += $"{row . field11},";
                        continue;
                    case 12:
                        if ( row . field12 == null ) alldone = true;
                        else data += $"{row . field12},";
                        continue;
                    case 13:
                        if ( row . field13 == null ) alldone = true;
                        else data += $"{row . field13},";
                        continue;
                    case 14:
                        if ( row . field14 == null ) alldone = true;
                        else data += $"{row . field14},";
                        continue;
                    case 15:
                        if ( row . field15 == null ) alldone = true;
                        else data += $"{row . field15},";
                        continue;
                    case 16:
                        if ( row . field16 == null ) alldone = true;
                        else data += $"{row . field16},";
                        continue;
                    case 17:
                        if ( row . field17 == null ) alldone = true;
                        else data += $"{row . field17},";
                        continue;
                    case 18:
                        if ( row . field18 == null ) alldone = true;
                        else data += $"{row . field18},";
                        continue;
                    case 19:
                        if ( row . field19 == null ) alldone = true;
                        else data += $"{row . field19},";
                        continue;
                    case 20:
                        if ( row . field20 == null ) alldone = true;
                        else data += $"{row . field20},";
                        continue;
                }
            }

            if ( data [ data . Length - 1 ] == ',' )
                data = data . Trim ( ) . Substring ( 0 , data . Length - 1 );
            string [ ] temp = data . Split ( "," );
            string fld = "";
            for ( int p = 0 ; p < dgControl . datagridControl . SelectedItems . Count ; p++ )
            {
                bool ID_Record = false;
                for ( int x = 0 ; x < temp . Length ; x++ )
                {
                    if ( ID_Record == false )
                    {
                        validflds = temp [ x ];
                        if ( validflds == "ID" && MainWindow . USE_ID_IDENTITY == true )
                            fld = validflds . Trim ( );
                        else if ( validflds == "ID" )
                        {
                            ID_Record = true;
                            break;
                        }
                        else
                            fld = validflds . Trim ( );
                        if ( fld . Contains ( "})" ) )
                        {
                            fld = fld . Substring ( 0 , fld . Length - 1 );
                            if ( fld . Contains ( " " ) )
                            {
                                string [ ] splitter = fld . Split ( " " );
                                if ( splitter [ 0 ] . Contains ( "/" ) )
                                {
                                    splitter [ 0 ] = NewWpfDev . Utils . ConvertInputDate ( splitter [ 0 ] );
                                    fld = splitter [ 0 ];
                                }
                            }
                        }
                        else
                        {
                            if ( fld . Contains ( " " ) )
                            {
                                string [ ] splitter = fld . Split ( " " );
                                if ( splitter [ 0 ] . Contains ( "/" ) )
                                {
                                    splitter [ 0 ] = NewWpfDev . Utils . ConvertInputDate ( splitter [ 0 ] );
                                    fld = $" {splitter [ 0 ]}" . TrimStart ( ) . TrimEnd ( );
                                }
                            }
                        }
                    }
                    if ( fld == "" )
                        continue;

                    SqlDataString += $"{fld},";
                    if ( SqlDataString . Length > 0 )
                        SqlDataString = SqlDataString . Substring ( 0 , SqlDataString . Length - 1 );
                }
            }   // lower for ()
            return SqlDataString;
        }

        static public string RemoveLeadingSpaces ( string processQuery )
        {
            if ( processQuery . Trim ( ) . Contains ( " " ) )
            {
                processQuery = NewWpfDev . Utils . ReverseString ( processQuery );
                processQuery = processQuery . Substring ( 0 , processQuery . Length - 1 );
                processQuery = NewWpfDev . Utils . ReverseString ( processQuery );
            }
            return processQuery;
        }

        static public string CreateSqlInsertCommand ( string NewDbName , string recorddata , string insertstring , string [ ] args , string AllFields , int maxcols , out string res )
        {
            res = "";
            Debug . WriteLine ( $"Using INSERT INTO {NewDbName} {insertstring}\n{recorddata}" );
            $"11 - Calling SqlInsertGenericRecord() with \n{insertstring} + {recorddata}" . CW ( );
            int recsinserted = 0;
            string err = "";
            if ( SqlInsertGenericRecord ( recorddata , insertstring , NewDbName , out err , out recsinserted ) == true )
            {
                Debug . WriteLine ( $"Insertion of {recorddata} Succeeded" );
            }
            else
            {
                res = err;
            }
            // Go get next record
            return insertstring;
        }
        static public int SaveAsNewTable ( string CurrentTable , string newtable , out string err )
        {
            err = "";
            List<string> FldNames = new List<string> ( );
            ObservableCollection<DapperGenericsLib . GenericClass> collection = new ObservableCollection<DapperGenericsLib . GenericClass> ( );
            string NewDbName = GenericControl . NewTableName . Text . Trim ( );
            MessageBoxResult mbresult = MessageBox . Show ( $"Click YES to save entire table as a new table that will\nbe named [ {CurrentTable . ToUpper ( )} ], (or you can change that name in the field provided before continuing)."
                + $"\n\nTo only save certain columns to the new table click the NO button & then choose the columns required from the popup that will follow... " , "Table Duplication ?" ,
                MessageBoxButton . YesNoCancel ,
                MessageBoxImage . Question ,
                MessageBoxResult . No );

            if ( mbresult == MessageBoxResult . Cancel )
                return -1;
            if ( mbresult == MessageBoxResult . No )
            {
                if ( DatagridControl . CurrentTable == null )
                    DatagridControl . CurrentTable = CurrentTable;
                // Save a set with only user selected columns
                string ConString = GenericDbUtilities . CheckSetSqlDomain ( "" );
                if ( ConString == "" )
                {
                    GenericDbUtilities . CheckDbDomain ( "" );
                    ConString = MainWindow . CurrentSqlTableDomain;
                }
                
                string SqlDataString = dgControl . GetFullColumnInfo ( newtable,  CurrentTable , ConString , false );

                string [ ] args = new string [ 20 ];
                string buffer = "";
                int index = 0;

                if ( NewDbName == "" )  // Sanity check
                {
                    MessageBox . Show ( "Please enter a suitable name for the table you want to create !" , "Naming Error" );
                    return -1;
                }

                // Create data  for columns in donor table to populate selection dialog
                args = SqlDataString . Split ( '\n' );
                foreach ( var item in args )
                {
                    string [ ] flds = { "" , "" , "" , "" };
                    if ( item != null && item . Trim ( ) != "" )
                    {
                        string [ ] RawFldNames = item .TrimStart(). Split ( ':' );
                        DapperGenericsLib . GenericClass tem = new DapperGenericsLib . GenericClass ( );
                        //int y = 0;
                        if( RawFldNames [ 0 ] !=null)
                        tem . field1 = RawFldNames [ 0 ];
                        if ( RawFldNames . Length > 1 )
                        {
                            if ( RawFldNames [ 1 ] != null )
                                tem . field2 = RawFldNames [ 1 ];
                        }
                        if ( RawFldNames . Length > 2 )
                        {
                            if ( RawFldNames [ 2 ] != null )
                                tem . field3 = RawFldNames [ 2 ];
                        }
                        if ( RawFldNames . Length > 3 )
                        {
                            if ( RawFldNames [ 3 ] != null )
                                tem . field4 = RawFldNames [ 3 ];
                        }
                        
                        collection . Add ( tem );
                    }
                 }
                //ALL WORKING  20/9/2022 - We now have a list of ALL Column names with
                //column type & size data, so let user choose what to save to a new table!
                //Genericgrid. SelectedRows . Clear ( );
                // load selection dialog with available columns
                GenericControl . ColNames . ItemsSource = collection;
                // Show dialog
                GenericControl . FieldSelectionGrid . Visibility = Visibility . Visible;
            }
            else
            {
                // just  do a direct copy
                // Works just perfectly 21/10/22
                if ( NewDbName == "" )  // Sanity check
                {
                    MessageBox . Show ( "Please enter a suitable name for the table you want to create !" , "Naming Error" );
                    return -1;
                }
                string [ ] args = { $"{GenericControl . SqlTables . SelectedItem . ToString ( )}" , $"{NewDbName}" };
                int recordcount = 0;
                string Tablename = "";
                recordcount = dgControl . ExecuteStoredProcedure ( "spCopyDb" , args , out err );
                if ( recordcount == -9 )
                     return -9;

                // make deep copy of table else it gets cleared elsewhere
                // Create a completely new instance via seriazable Clone method stored in NewWpfDev.Utils (in ObjectCopier class file)
                string originalname = $"{GenericControl . SqlTables . SelectedItem . ToString ( )}";
                ObservableCollection<NewWpfDev . GenericClass> deepcopy = new ObservableCollection<NewWpfDev . GenericClass> ( );
                deepcopy = NewWpfDev . Utils . CopyCollection ( ( ObservableCollection<NewWpfDev . GenericClass> ) GenericControl . GridData , ( ObservableCollection<NewWpfDev . GenericClass> ) deepcopy );
                GenericControl . GridData = deepcopy;
                string [ ] args1 = { $"{NewDbName}" };
                int colcount = dgControl . datagridControl . Columns . Count;
                DatagridControl . LoadActiveRowsOnlyInGrid ( dgControl . datagridControl , GenericControl . GridData , colcount );
                GenericControl . ResetColumnHeaderToTrueNames ( NewDbName , dgControl . datagridControl );
                GenericControl . LoadDbTables ( NewDbName );
                GenericControl . statusbar . Text = $"The current table [{originalname . ToUpper ( )}] was saved as [{NewDbName . ToUpper ( )}] successfully ...";
            }
            Mouse . OverrideCursor = Cursors . Arrow;
            return 1;
        }

        static public void SelectCurrentTable ( string table )
        {
            int index = 0;
            string currentTable = table . ToUpper ( );
            foreach ( string item in GenericControl . SqlTables . Items )
            {
                if ( currentTable == item . ToUpper ( ) )
                {
                    GenericControl . SqlTables . SelectedIndex = index;
                    break;
                }
                index++;
            }
        }
        static public int GetTableColumnsCount ( string tname , string [ ] args, string CurrentTableDomain )
        {
            List<string> SqlQuerylist = new List<string> ( );
            string spCommand = "drop table if exists zz; drop table if exists zzz; " +
                $"select column_name,upper(Table_name) as name into zz from information_schema.columns; " +
                 $"select column_name, name into zzz from zz where name='{tname}'; " +
                 $"select count(name) as cnt from zzz;";
            SqlQuerylist = DatagridControl . ProcessUniversalQueryStoredProcedure ( spCommand , args , CurrentTableDomain, out string err2 );
            Debug . WriteLine ( $"Direct SQL query returned [ {SqlQuerylist [ 0 ]} ]" );
            if ( err2 != "" )
                Debug . WriteLine ( $"SqlCommand [ {spCommand} ] failed : Reason [ {err2} ]" );
            // result is returned in List<strng with just one row.
            int colcount = Convert . ToInt32 ( SqlQuerylist [ 0 ] );
            return colcount;
        }
        // EOF EOF EOF EOF EOF EOF EOF EOF EOF EOF EOF EOF EOF EOF EOF EOF EOF EOF EOF EOF EOF EOF 
    }
}


