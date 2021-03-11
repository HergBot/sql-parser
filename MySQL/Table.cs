using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace SqlParser.Data.MySQL
{
    public class Table
    {
        private const string COMMENT_INDICATOR = "--";

        private const string FOREIGN_KEY_INDICATOR = "FOREIGN KEY";

        private const string PRIMARY_KEY_INDICATOR = "PRIMARY KEY";

        public static Table Parse(string command)
        {
            Regex tablePattern = new Regex(@"CREATE TABLE\s(?<tableName>\w+)[\n\r\s]+\((?<tableContent>.*?)\);", RegexOptions.Singleline);
            Regex primaryKeyPattern = new Regex(@"PRIMARY KEY\s\((?<primaryKeyField>[_\w,\s]+)\)");
            Match match = tablePattern.Match(command);
            string tableName = match.Groups["tableName"].Value;
            IEnumerable<string> tableContents = match.Groups["tableContent"].Value
                .Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries)
                .Where(x => !string.IsNullOrWhiteSpace(x));

            Dictionary<string, Column> columns = new Dictionary<string, Column>();
            HashSet<string> primaryKeys = new HashSet<string>();
            Dictionary<string, ForeignKey> foreignKeys = new Dictionary<string, ForeignKey>();
            foreach(string definition in tableContents)
            {
                if (definition.Contains(PRIMARY_KEY_INDICATOR))
                {
                    Match primaryKeyMatch = primaryKeyPattern.Match(definition);
                    string[] compositeKeys = primaryKeyMatch.Groups["primaryKeyField"].Value.Split(',');
                    foreach(string key in compositeKeys)
                    {
                        primaryKeys.Add(key.Trim());
                    }
                }
                else if (definition.Contains(FOREIGN_KEY_INDICATOR))
                {
                    ForeignKey key = ForeignKey.Parse(definition);
                    foreignKeys.Add(key.LocalColumn, key);
                }
                else
                {
                    Column newColumn = Column.Parse(definition);
                    columns.Add(newColumn.Name, newColumn);
                }
            }

            
            return new Table(tableName, columns, primaryKeys, foreignKeys);
        }

        public string Name { get; private set; }

        public Dictionary<string, Column> Columns { get; private set; }

        public HashSet<string> PrimaryKeys { get; private set; }

        public Dictionary<string, ForeignKey> ForeignKeys { get; private set; }

        public Table(string name, Dictionary<string, Column> columns, HashSet<string> primaryKeys, Dictionary<string, ForeignKey> foreignKeys)
        {
            Name = name;
            Columns = columns;
            PrimaryKeys = primaryKeys;
            ForeignKeys = foreignKeys;
        }

        public IEnumerable<string> GetDependencies()
        {
            if (!ForeignKeys.Any())
            {
                return Enumerable.Empty<string>();
            }

            return ForeignKeys.Values.Select(fKey => fKey.ForeignTable).Distinct();
        }
    }
}
