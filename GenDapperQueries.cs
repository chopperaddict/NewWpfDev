using System;
using System . Collections . Generic;
using System . Collections . ObjectModel;
using System . Data;
using System . Data . SqlClient;
using System . Diagnostics;
using System . Linq;
using System . Reflection . Metadata;
using System . Text;
using System . Windows;

using Dapper;
//using NewWpfDev.Views.StoredprocsProcessing;

using GenericSqlLib . Models;

using NewWpfDev . Views;

namespace NewWpfDev
{
    /* Main file of (3) partial files
    GenDapperQueries
    GenDapperDynamicQueries
    GenDapperQueriesSupport
     */
    public static partial class GenDapperQueries
    {
        static public void ExecuteSqlCommandWithNoReturnValue ( int method, string SqlCommand , List<string [ ]> argsbuff, out string error )
        {
            IEnumerable<dynamic>? reslt;
            error = "";
            string connectionString = MainWindow . SqlCurrentConstring;
            SqlConnection sqlCon = null;

            string Con = CheckSetSqlDomain ( MainWindow . CurrentSqlTableDomain );
            if ( Con == "" )
            {
                // set to our local definition
                Con = MainWindow . CurrentSqlTableDomain;
                MessageBox . Show ( $"It was not possible to Identify a valid Sql Connection string for \nthe Database [ {MainWindow . CurrentSqlTableDomain . ToUpper ( )} ]\n\n Please report  this error to DB Technical Support" , "Connection Error" );
                error = $"Could not get correct SQL initialization string for domain {MainWindow . CurrentSqlTableDomain}";
                return;
            }
            if ( method == 0 )
            {
                try
                {
                    using ( sqlCon = new SqlConnection ( Con ) )
                    {
                        Debug . WriteLine ( $"Executing [{SqlCommand}" );
                        {
                            //=================================================================================//
                            sqlCon . Execute ( SqlCommand , null , commandType: CommandType . Text );
                            //=================================================================================//
                        }
                    }
                }
                catch ( Exception ex )
                {
                    error = ex . Message;
                    Debug . WriteLine ( $"Error Executing Command {SqlCommand} : {ex . Message}" );
                    return;
                }
                error = $"Command [{SqlCommand}] has completed successfully...";
            }
            else if ( method == 1 )
            {
                try
                {
                    var parameters = new DynamicParameters ( );
                    parameters = StoredprocsProcessing . ParseNewSqlArgs ( parameters , argsbuff , out error );

                    using ( sqlCon = new SqlConnection ( Con ) )
                    {
                        Debug . WriteLine ( $"Executing [{SqlCommand}" );
                        {
                            //=================================================================================//
                            sqlCon . Execute ( SqlCommand , parameters , commandType: CommandType . StoredProcedure );
                            //=================================================================================//
                        }
                    }
                }
                catch ( Exception ex )
                {
                    error = ex . Message;
                    Debug . WriteLine ( $"Error Executing Command {SqlCommand} : {ex . Message}" );
                }
            }
        }
        public static DataTable GetSqlDataAsDataTable( string SqlCommand , string ConnectionString )
        {
            SqlConnection con;
            DataTable dt = new DataTable ( );
            string filterline = "";
            string ConString = Flags . CurrentConnectionString;
             con = new SqlConnection ( ConString );
            try
            {
                Debug . WriteLine ( $"Using new SQL connection in PROCESSSQLCOMMAND" );
                using ( con )
                {
                    SqlCommand cmd = new SqlCommand ( SqlCommand , con );
                    SqlDataAdapter sda = new SqlDataAdapter ( cmd );
                    sda . Fill ( dt );
                }
            }
            catch ( Exception ex )
            {
                Debug . WriteLine ( $"ERROR in PROCESSSQLCOMMAND(): Failed to load Datatable :\n {ex . Message}, {ex . Data}" );
                MessageBox . Show ( $"ERROR in PROCESSSQLCOMMAND(): Failed to load datatable\n{ex . Message}" );
            }
            finally
            {
                $" SQL dataTable loaded from SQLCommand [{SqlCommand . ToUpper ( )}]" . cwinfo ( );
                con . Close ( );
            }
            return dt;
        }

    }
}
