using SqlAnalyzer.Core.Models.ExecPlan;
using System;
using System.Linq;
using System.Text;

namespace SqlAnalyzer.Core.Models
{
    public static class Extentions
    {
        public static string GetQuery(this MissingIndexes missingIndexes)
        {
            var bld = new StringBuilder();
            foreach(var group in missingIndexes.Items)
            {
                bld.AppendFormat("--    Impact:{0}%{1}",
                    group.Impact,
                    Environment.NewLine);
                foreach(var index in group.MissingIndex)
                {
                    bld.AppendFormat("CREATE NONCLUSTERED INDEX [IX_{1}_{2}_{3}_{4}]{0}",
                        Environment.NewLine,//0
                        index.Table.Replace("[",string.Empty).Replace("]", string.Empty),//1
                        string.Join("And",index.ColumnGroup.Where(x=>x.Usage.Equals("EQUALITY") || x.Usage.Equals("INEQUALITY")).SelectMany(x => x.Column).Select(x => x.Name.Replace("[", string.Empty).Replace("]", string.Empty))),//2
                        DateTime.Now.ToString("ddMMyyyy"),//3
                        Guid.NewGuid());//4
                    bld.AppendFormat("ON {1}.{2} ({3}){0}",
                        Environment.NewLine,//0
                        index.Schema,//1
                        index.Table,//2
                        string.Join(",", index.ColumnGroup.Where(x => x.Usage.Equals("EQUALITY") || x.Usage.Equals("INEQUALITY")).SelectMany(x => x.Column).Select(x => x.Name)));
                    {
                        bld.AppendFormat("INCLUDE ({1}){0}",
                            Environment.NewLine,//0
                            string.Join(",", index.ColumnGroup.Where(x => x.Usage.Equals("INCLUDE")).SelectMany(x => x.Column).Select(x=>x.Name)));//2
                    }
                    bld.AppendLine();
                }
            }
            return bld.ToString();
        }
    }
}
