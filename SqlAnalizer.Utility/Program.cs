using SqlAnalyzer.Core.Infrastructure.Parsers;
using SqlAnalyzer.Core.Models.Infrastructure.DbExplorers;
using System;
using System.Linq;

namespace SqlAnalizer.Utility
{
    class Program
    {
        static void Main(string[] args)
        {
            //TODO: set connection string value
            var conString = string.Empty;

            if (string.IsNullOrEmpty(conString))
            {
                Console.WriteLine("Connection string is empty");
                return;
            }

            var an = new SqlProcedureStructureExplorer(conString);
            var procNames = an.GetProceduresFromDb();

            var explorer = new SqlExecPlanExplorer(conString);
            var parser = new ExecPlanParser();

            foreach (var procName in procNames)
            {
                Console.WriteLine($"--  Procedure name: {procName}");
                var planContent = explorer.GetExecPlan(procName);
                if (!string.IsNullOrEmpty(planContent))
                {
                    var missingIndexes = parser.GetMissingInsexes(planContent);
                    if (missingIndexes.Any())
                    {
                        Console.WriteLine(string.Join(",", parser.GetMissingInsexes(planContent)));
                    }
                    else
                        Console.WriteLine("--  Indexes are not required :)");
                }
                else
                {
                    Console.WriteLine("--  Execution plan generation error (!!!)");
                }
                Console.WriteLine();
            }

            Console.ReadLine();
        }
    }
}
