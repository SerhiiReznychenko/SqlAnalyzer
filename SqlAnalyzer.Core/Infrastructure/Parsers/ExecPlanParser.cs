using SqlAnalyzer.Core.Models;
using SqlAnalyzer.Core.Models.ExecPlan;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace SqlAnalyzer.Core.Infrastructure.Parsers
{
    public class ExecPlanParser
    {
        private readonly string indexStartValue = "<MissingIndexes>";
        private readonly string indexEndValue = "</MissingIndexes>";
        public IList<string> GetMissingInsexes(string execPlanContent)
        {
            var indexes = new List<string>();
            var start = execPlanContent.IndexOf(indexStartValue);
            if (start > 0)
            {
                var end = execPlanContent.IndexOf(indexEndValue) + indexEndValue.Length;
                var xmlSerializer = new XmlSerializer(typeof(MissingIndexes));
                using (StringReader xmlReader = new StringReader(execPlanContent.Substring(start, end - start)))
                {
                    var missingIndexes = (MissingIndexes)xmlSerializer.Deserialize(xmlReader);
                    indexes.Add(missingIndexes.GetQuery());
                }
            }
            return indexes;
        }
    }
}
