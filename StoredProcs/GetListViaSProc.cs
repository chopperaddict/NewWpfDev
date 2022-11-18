using System;
using System . Collections . Generic;
using System . Data;
using System . Diagnostics;
using System . Text;
using System . Windows . Input;
using System . Windows;

using Dapper;

using NewWpfDev;
using GenericSqlLib . Models;
using NewWpfDev . Models;
using System . Data . SqlClient;
using System . Windows . Documents;
using System . Linq;

namespace StoredProcs
{
    public  class GetListViaSProc
    {
        public static List<string> CallStoredProcedure ( List<string> list , string sqlcommand , string [ ] args = null )
        {
            List<string> splist = new List<string> ( );
            splist = GetListViaSProc . ProcessUniversalQueryStoredProcedure ( "spGetStoredProcs" , args , MainWindow.CurrentSqlTableDomain, out string err );
            //This call returns us a List<string>
            // This method is NOT a dynamic method
            return splist;
        }

        ///************************************************************************************************///
        /// <summary>
        /// Class to handle calls via Dapper that returns a list<string> by calling Dapper
        /// that then runs a Stored Procedure using the args[] provided by the caller
        /// 
        /// </summary>
        /// <param name="spCommand">The SP to be run</param>
        /// <param name="args">whatever arguments the SP requires</param>
        /// <param name="CurrentTableDomain">Current Db Domain</param>
        /// <param name="err">out string to hold any error messages that may occur</param>
        /// <returns>List<string></string></returns>
        ///************************************************************************************************///
        static public List<string> ProcessUniversalQueryStoredProcedure ( string spCommand , string [ ] args , string CurrentTableDomain , out string err )
        {
            int result = -1;
            string Con = "";
            err = "";
            //if ( MainWindow . SqlCurrentConstring != CurrentTableDomain )
            //    MainWindow . SqlCurrentConstring = CurrentTableDomain;
            Con = GenericDbUtilities . CheckSetSqlDomain ( CurrentTableDomain );
            if ( Con == "" )
            {
                // set to our local definition
                Con = MainWindow . CurrentSqlTableDomain;
                MessageBox . Show ( $"It was not possible to Identify a valid Sql Connection string for \nthe Database [ {CurrentTableDomain . ToUpper ( )} ]\n\n Please report  this error to DB Technical Support" , "Connection Error" );
                return null;
            }

            SqlConnection sqlCon = new SqlConnection ( );
            List<string> queryresults = new List<string> ( );

            "" . Track ( );
            Mouse . OverrideCursor = Cursors . Wait;
            Debug . WriteLine ( $"Running Stored Procedure {spCommand}" );
            using ( sqlCon = new SqlConnection ( Con ) )
            {
                sqlCon . Open ( );
                // Now add record  to SQL table
                bool hasoutput = false;
                bool hasretval = false;
                var parameters = new DynamicParameters ( );
                parameters = SProcsSupport . ParseSqlArguments ( parameters , args , ref hasoutput , ref hasretval );
                try
                {
                    $" calling {spCommand} ()" . CW ( );

                    Debug . WriteLine ( $"ProcessUniversalQueryStoredProcedure() : [ {spCommand . ToUpper ( )} ]" );

                    //***********************************************************************************************************************************************//
                    queryresults = sqlCon . Query<string> ( spCommand , parameters , commandType: CommandType . StoredProcedure ) . ToList ( );
                    //***********************************************************************************************************************************************//

                    //result = sqlCon . Execute ( spCommand , parameters , commandType: CommandType . StoredProcedure );
                    Debug . WriteLine ( $"{spCommand} returned  RESULT = {queryresults . Count}" );
                }
                catch ( Exception ex )
                {
                    Utils . DoErrorBeep ( );
                    Debug . WriteLine ( $"{ex . Message}" );
                    result = -1;
                    err = ex . Message;
                }
                Mouse . OverrideCursor = Cursors . Arrow;
                "" . Track ( 1 );
                return queryresults;
            }
        }

    }
}
