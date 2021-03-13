//
// FILE     : ForeignKey.cs
// PROJECT  : SQL Parser
// AUTHOR   : xHergz
// DATE     : 2021-03-10
// 

using System;
using System.Text.RegularExpressions;

namespace HergBot.SqlParser.Data.MySQL
{
    /// <summary>
    /// Holds the data for a foreign key constraint
    /// </summary>
    public class ForeignKey
    {
        /// <summary>
        /// The column referenced in the local table
        /// </summary>
        public string LocalColumn;

        /// <summary>
        /// The linked foreign tables name
        /// </summary>
        public string ForeignTable;

        /// <summary>
        /// The linked foreign tables column
        /// </summary>
        public string ForeignColumn;

        /// <summary>
        /// Parses a foreign key constraint
        /// </summary>
        /// <param name="text">The foreign key constraint definition</param>
        /// <returns>The foreign key data</returns>
        /// <exception cref="FormatException">If the definition is missing a local column, foreign table, or foreign column</exception>
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
