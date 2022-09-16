using System;
using System . Collections . Generic;
using System . Data;
using System . Data . SqlClient;
using System . Diagnostics;
using System . Threading . Tasks;

using Dapper;

namespace NewWpfDev
{
    public abstract class SqlAbstractSupport
    {
        public Type DbType { get; set; }
        public string DbName { get; set; }

        public int MyProperty { get; set; }
        //public ObservableCollection<typeof(DbType)> Database{ get; set; }
        public object LoadSqlTable<T> ( T collection , string Sqlcommand , string ConString , out string ResultString , int max = 0 , bool Notify = false )
        {
            string argument = "";
            string [ ] args = Sqlcommand . Split ( " " );
            if ( Sqlcommand . Contains ( " " ) )
            {
                argument = args [ 1 ];
                Sqlcommand = args [ 0 ] . Trim ( );

            }
            string err = "";
            string [ ] newargs = new string [ 1 ];
            newargs [ 0 ] = argument;
            object data = ExecuteStoredProcedure<T> ( Sqlcommand ,
             newargs ,
             ConString ,
             out err );
            ResultString = "";

            return data;
        }

        //--------------------------------------------------------------------------------------------------------------------------------------------------------
        public object ExecuteStoredProcedure<T> ( string SqlCommand , string [ ] args , string ConString , out string err )
        {
            //####################################################################################//
            // Handles running a dapper stored procedure call with transaction support & thrws exceptions back to caller
            //####################################################################################//
            int gresult = -1;
            //string Con = Flags . CurrentConnectionString;
            string Con = ConString;
            SqlConnection sqlCon = null;
            err = "";

            try
            {
                using ( sqlCon = new SqlConnection ( Con ) )
                {
                    sqlCon . Open ( );
                    using ( var tran = sqlCon . BeginTransaction ( ) )
                    {
                        var parameters = new DynamicParameters ( );
                        if ( args . Length > 0 )
                        {
                            for ( int x = 0 ; x < args . Length ; x++ )
                            {
                                parameters . Add ( $"{args [ x ]}" , args [ x ] , System . Data . DbType . String , ParameterDirection . Input , args [ x ] . Length );
                            }
                        }
                        // Perform the sql command requested
                        gresult = sqlCon . Execute ( @SqlCommand , parameters , commandType: CommandType . StoredProcedure , transaction: tran );
                        // Commit the transaction
                        tran . Commit ( );
                    }
                }
            }
            catch ( Exception ex )
            {
                Debug . WriteLine ( $"Error {ex . Message}, {ex . Data}" );
                err = $"Error {ex . Message}";
            }
            return gresult;
        }

        public async Task<IEnumerable<T>> GetSqlData <T> (string table , string constring)
        {
            IEnumerable<T> data;

            using ( var connection = new SqlConnection ( constring ) )
            {
                await connection . OpenAsync ( );
                var query = $@"SELECT * FROM {table }";
                data = await connection . QueryAsync<T> ( query );
            }
            return data;
        }

    }
}
