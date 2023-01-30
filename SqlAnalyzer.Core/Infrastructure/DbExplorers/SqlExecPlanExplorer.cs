using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Text;

namespace SqlAnalyzer.Core.Models.Infrastructure.DbExplorers
{
    public class SqlExecPlanExplorer
    {
        readonly string _connectionString;
        private readonly string _enableExPlan = "SET SHOWPLAN_XML ON";
        private readonly string _disableExPlan = "SET SHOWPLAN_XML OFF";

        public SqlExecPlanExplorer(string connectionString)
        {
            _connectionString = connectionString;
        }

        public string GetExecPlan(string procName)
        {
            try
            {
                var query = GetPreparedQuery(procName);
                using var con = new SqlConnection(_connectionString);
                con.Open();
                using var cmd = new SqlCommand()
                {
                    Connection = con,
                    CommandText = _enableExPlan,
                    CommandType = CommandType.Text
                };

                //ON
                cmd.ExecuteNonQuery();

                //Result
                cmd.CommandText = query;
                var result = cmd.ExecuteScalar();

                //OFF
                cmd.CommandText = _disableExPlan;
                cmd.ExecuteNonQuery();

                return result.ToString();
            }
            catch (Exception ex) { }
            return string.Empty;
        }

        private string GetPreparedQuery(string procName)
        {
            var procExplorer = new SqlProcedureStructureExplorer(_connectionString);
            var procParams = procExplorer.GetParamsFromProcedure(procName);
            var pars = new List<string>();
            var bld = new StringBuilder();
            foreach (SqlParameter p in procParams)
            {
                bld.AppendFormat("DECLARE {0} {1} {2}",
                        p.ParameterName,
                        p.SqlDbType,
                        p.Size > 0 ? $"({p.Size})" : string.Empty
                        );
                bld.AppendLine();
                pars.Add(string.Format("{0} {1}",
                    p.ParameterName,
                    p.Direction == ParameterDirection.Output ? "OUTPUT" : string.Empty));
            }
            bld.AppendLine();
            bld.AppendFormat($"EXECUTE @RETURN_VALUE = {procName} {string.Join(",", pars)}");
            return bld.ToString();

        }
    }
}
