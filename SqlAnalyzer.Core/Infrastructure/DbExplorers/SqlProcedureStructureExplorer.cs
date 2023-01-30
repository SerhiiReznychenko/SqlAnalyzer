using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace SqlAnalyzer.Core.Models.Infrastructure.DbExplorers
{
    public class SqlProcedureStructureExplorer
    {
        readonly string _connectionString;
        public SqlProcedureStructureExplorer(string connectionString)
        {
            _connectionString = connectionString;
        }

        public IList<string> GetProceduresFromDb()
        {
            var list = new List<string>();
            try
            {
                using var con = new SqlConnection(_connectionString);
                using var cmd = new SqlCommand()
                {
                    Connection = con,
                    CommandText = @"SELECT '['+ROUTINE_SCHEMA+'].['+  ROUTINE_NAME+']' [ProcedureName]
                                        FROM INFORMATION_SCHEMA.ROUTINES
                                        WHERE ROUTINE_TYPE = 'PROCEDURE'",
                    CommandType = CommandType.Text
                };
                con.Open();
                using var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    list.Add(reader.GetString(0));
                }
                reader.Close();
            }
            catch (Exception ex) { }
            return list;
        }

        public IList<SqlParameter> GetParamsFromProcedure(string procName)
        {
            var list = new List<SqlParameter>();
            try
            {
                using var con = new SqlConnection(_connectionString);
                using var cmd = new SqlCommand()
                {
                    Connection = con,
                    CommandText = procName,
                    CommandType = CommandType.StoredProcedure
                };
                con.Open();
                SqlCommandBuilder.DeriveParameters(cmd);
                foreach (SqlParameter p in cmd.Parameters)
                {
                    //if(!p.ParameterName.Equals("@RETURN_VALUE", StringComparison.OrdinalIgnoreCase))
                        list.Add(p);
                }
            }
            catch (Exception ex) { }
            return list;
        }
    }
}
