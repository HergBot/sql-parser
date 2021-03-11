using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace SqlParser.Data.MySQL
{
    public class ForeignKey
    {
        public string LocalColumn;

        public string ForeignTable;

        public string ForeignColumn;

        public static ForeignKey Parse(string text)
        {
            Regex pattern = new Regex(@"FOREIGN KEY\s\((?<localColumn>[_\w]+)\)\s(?<foreignTable>[_\w]+)\((?<foreignColumn>[_\w]+)\)");
            Match match = pattern.Match(text);
            string localColumn = match.Groups["localColumn"].Value;
            string foreignTable = match.Groups["foreignTable"].Value;
            string foreignColumn = match.Groups["foreignColumn"].Value;

            if (string.IsNullOrWhiteSpace(localColumn) || string.IsNullOrWhiteSpace(foreignTable) || string.IsNullOrWhiteSpace(foreignColumn))
            {
                throw new FormatException("Foreign keys require a local column, foreign table, and foreign column");
            }

            return new ForeignKey()
            {
                LocalColumn = localColumn,
                ForeignTable = foreignTable,
                ForeignColumn = foreignColumn,
            };
        }
    }
}
