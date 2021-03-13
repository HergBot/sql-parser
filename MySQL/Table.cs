//
// FILE     : Table.cs
// PROJECT  : SQL Parser
// AUTHOR   : xHergz
// DATE     : 2021-03-10
// 

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace SqlParser.Data.MySQL
{
    /// <summary>
    /// Holds information for a MySQL table
    /// </summary>
    public class Table
    {
        private const string COMMENT_INDICATOR = "--";

        private const string FOREIGN_KEY_INDICATOR = "FOREIGN KEY";

        private const string PRIMARY_KEY_INDICATOR = "PRIMARY KEY";

        /// <summary>
        /// Parses a full MySQL table
        /// </summary>
        /// <param name="text">The table definition</param>
        /// <returns>The table information</returns>
        public static Table Parse(string text)
        {
            Regex tablePattern = new Regex(@"CREATE TABLE\s(?<tableName>\w+)[\n\r\s]+\((?<tableContent>.*?)\);", RegexOptions.Singleline);
            Regex primaryKeyPattern = new Regex(@"PRIMARY KEY\s\((?<primaryKeyField>[_\w,\s]+)\)");
            Match match = tablePattern.Match(text);
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

        /// <summary>
        /// Table name
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// Table columns
        /// </summary>
        public Dictionary<string, Column> Columns { get; private set; }

        /// <summary>
        /// Primary keys
        /// </summary>
        public HashSet<string> PrimaryKeys { get; private set; }

        /// <summary>
        /// Foreign keys
        /// </summary>
        public Dictionary<string, ForeignKey> ForeignKeys { get; private set; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="name">Table name</param>
        /// <param name="columns">Table columns</param>
        /// <param name="primaryKeys">Primary keys</param>
        /// <param name="foreignKeys">Foreign keys</param>
        public Table(string name, Dictionary<string, Column> columns, HashSet<string> primaryKeys, Dictionary<string, ForeignKey> foreignKeys)
        {
            Name = name;
            Columns = columns;
            PrimaryKeys = primaryKeys;
            ForeignKeys = foreignKeys;
        }

        /// <summary>
        /// Get the tables this table depends on via foreign key constraints
        /// </summary>
        /// <returns>A list of names for dependencies</returns>
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
