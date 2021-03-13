//
// FILE     : DataType.cs
// PROJECT  : SQL Parser
// AUTHOR   : xHergz
// DATE     : 2021-03-10
// 

using System;

namespace HergBot.SqlParser.Data.MySQL
{
    /// <summary>
    /// A list of most of the MySQL Data Types
    /// </summary>
    public enum DataTypes
    {
        TINYINT,
        SMALLINT,
        MEDIUMINT,
        BIGINT,
        INT,
        INTEGER,
        DECIMAL,
        NUMERIC,
        FLOAT,
        DOUBLE,
        BIT,
        DATE,
        DATETIME,
        TIMESTAMP,
        TIME,
        YEAR,
        CHAR,
        VARCHAR,
        BINARY,
        VARBINARY,
        BLOB,
        TEXT,
        ENUM,
        SET,
    }

    /// <summary>
    /// Holds the information for a MySQL data type
    /// </summary>
    public class DataType
    {
        /// <summary>
        /// The MySQL data type
        /// </summary>
        public DataTypes Type { get; set; }

        /// <summary>
        /// The size if applicable to the type
        /// </summary>
        public int? Size { get; set; }

        /// <summary>
        /// Parses a MySQL data type
        /// </summary>
        /// <param name="text">The data type text</param>
        /// <returns>The MySQL data type</returns>
        /// <exception cref="ArgumentNullException">When the text is null</exception>
        /// <exception cref="ArgumentException">When the text is empty, whitespace, or is an invalid type</exception>
        public static DataType Parse(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
            {
                throw text == null
                    ? new ArgumentNullException("Data Type cannot be null")
                    : new ArgumentException("Data Type cannot be empty");
            }

            char[] brackets = { '(', ')' };
            string[] pieces = text.Split(brackets);
            int? size = null;
            bool validType = Enum.TryParse<DataTypes>(pieces[0], out DataTypes type);

            if (!validType)
            {
                throw new ArgumentException($"Unsupported type: {text}");
            }


            if (pieces.Length > 1 && int.TryParse(pieces[1], out int parsedSize))
            {
                size = parsedSize;
            }

            return new DataType()
            {
                Type = type,
                Size = size
            };
        }

        /// <summary>
        /// Creates a unique hash code for the type and size
        /// </summary>
        /// <returns>The unique hash code</returns>
        public override int GetHashCode()
        {
            int hash = 13;
            short multiplierPrime = 23;
            hash *= multiplierPrime + Type.GetHashCode();
            if (Size.HasValue)
            {
                hash *= multiplierPrime + Size.Value.GetHashCode();
            }
            return hash;
        }

        /// <summary>
        /// Determines if the given object is equal to the data type
        /// </summary>
        /// <param name="obj">Object to compare with</param>
        /// <returns>True if they are equal, false otherwise</returns>
        public override bool Equals(object obj)
        {
            if (obj == null || obj.GetType() != GetType())
            {
                return false;
            }
            DataType typeB = (DataType)obj;
            return Type == typeB.Type && Size == typeB.Size;
        }

        /// <summary>
        /// Determines if the given data types are equal
        /// </summary>
        /// <param name="typeA">Data type on the left side of the equation</param>
        /// <param name="typeB">Data type on the right side of the equation</param>
        /// <returns>True if they are equal, false otherwise</returns>
        public static bool operator == (DataType typeA, DataType typeB)
        {
            if ((object)typeA == null)
            {
                return (object)typeB == null;
            }
            return typeA.Equals(typeB);
        }

        /// <summary>
        /// Determines if the given data types are unequal
        /// </summary>
        /// <param name="typeA">Data type on the left side of the equation</param>
        /// <param name="typeB">Data type on the right side of the equation</param>
        /// <returns>True if they are not equal, false otherwise</returns>
        public static bool operator != (DataType typeA, DataType typeB)
        {
            return !(typeA == typeB);
        }
    }
}
