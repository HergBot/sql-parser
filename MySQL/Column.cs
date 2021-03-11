//
// FILE     : Column.cs
// PROJECT  : SQL Parser
// AUTHOR   : xHergz
// DATE     : 2021-03-10
// 

using System;

namespace SqlParser.Data.MySQL
{
    /// <summary>
    /// Holds the information for a SQL table column.
    /// </summary>
    public class Column
    {
        private const string AUTO_INCREMENT = "AUTO_INCREMENT";

        private const string NOT_NULL = "NOT NULL";

        /// <summary>
        /// Column name
        /// </summary>
        public string Name;
    
        /// <summary>
        /// Column SQL data type
        /// </summary>
        public DataType Type;

        /// <summary>
        /// If the column is nullable
        /// </summary>
        public bool Nullable = false;

        /// <summary>
        /// If the column is marked auto increment
        /// </summary>
        public bool AutoIncrement = false;

        /// <summary>
        /// Parse a SQL column from a line of text
        /// </summary>
        /// <param name="text">The SQL text for the column</param>
        /// <returns>The column</returns>
        /// <exception cref="FormatException">When the column text is malformed (i.e. not having atleast a name and type)</exception>
        public static Column Parse(string text)
        {
            string[] pieces = text.Trim().Split(' ');
            if (pieces.Length < 2)
            {
                throw new FormatException("A column requires at least 2 space separated pieces (name and type)");
            }

            string columnName = pieces[0];
            DataType type = DataType.Parse(pieces[1]);

            return new Column()
            {
                Name = columnName,
                Type = type,
                Nullable = !text.Contains(NOT_NULL),
                AutoIncrement = text.Contains(AUTO_INCREMENT),
            };
        }
    }
}
