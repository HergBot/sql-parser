using System.Collections.Generic;
using System.Linq;

using NUnit.Framework;

using HergBot.SqlParser.Data.MySQL;

namespace SqlParser_Tests
{
    public class Table_Tests
    {
        private const string TABLE_NAME = "Table";

        private const string COLUMN_ONE = "Column_One";

        private const string COLUMN_TWO = "Column_Two";

        private const string COLUMN_THREE = "Column_Three";

        private const string PARENT_TABLE_NAME = "ParentTable";

        private const string PARENT_COLUMN_NAME = "Parent_Column";

        private static string FULL_TABLE =
            $@"CREATE TABLE {TABLE_NAME}
            (
                {COLUMN_ONE} INT NOT NULL AUTO_INCREMENT,
                {COLUMN_TWO} INT NOT NULL,
                {COLUMN_THREE} VARCHAR(100) NOT NULL,
                PRIMARY KEY ({COLUMN_ONE}),
                FOREIGN KEY ({COLUMN_TWO}) {PARENT_TABLE_NAME}({PARENT_COLUMN_NAME})
            );";

        private static string NO_FOREIGN_KEYS =
            $@"CREATE TABLE {TABLE_NAME}
            (
                {COLUMN_ONE} INT NOT NULL AUTO_INCREMENT,
                {COLUMN_THREE} VARCHAR(100) NOT NULL,
                PRIMARY KEY ({COLUMN_ONE})
            );";

        private static string COMPOSITE_PRIMARY_KEYS =
            $@"CREATE TABLE {TABLE_NAME}
            (
                {COLUMN_ONE} INT NOT NULL,
                {COLUMN_TWO} INT NOT NULL,
                PRIMARY KEY ({COLUMN_ONE}, {COLUMN_TWO})
            );";

        [Test]
        public void Parse_FullTable()
        {
            Table table = Table.Parse(FULL_TABLE);
            Assert.AreEqual(TABLE_NAME, table.Name);
            ColumnExpected(table.Columns[COLUMN_ONE], COLUMN_ONE, new DataType() { Type = DataTypes.INT }, false, true);
            ColumnExpected(table.Columns[COLUMN_TWO], COLUMN_TWO, new DataType() { Type = DataTypes.INT }, false);
            ColumnExpected(table.Columns[COLUMN_THREE], COLUMN_THREE, new DataType() { Type = DataTypes.VARCHAR, Size = 100 }, false);
            Assert.AreEqual(1, table.PrimaryKeys.Count);
            Assert.IsTrue(table.PrimaryKeys.Contains(COLUMN_ONE));
            Assert.AreEqual(1, table.ForeignKeys.Count);
            Assert.IsTrue(table.ForeignKeys.ContainsKey(COLUMN_TWO));
        }

        [Test]
        public void Parse_NoForeignKeyTable()
        {
            Table table = Table.Parse(NO_FOREIGN_KEYS);
            Assert.AreEqual(TABLE_NAME, table.Name);
            ColumnExpected(table.Columns[COLUMN_ONE], COLUMN_ONE, new DataType() { Type = DataTypes.INT }, false, true);
            ColumnExpected(table.Columns[COLUMN_THREE], COLUMN_THREE, new DataType() { Type = DataTypes.VARCHAR, Size = 100 }, false);
            Assert.AreEqual(1, table.PrimaryKeys.Count);
            Assert.IsTrue(table.PrimaryKeys.Contains(COLUMN_ONE));
            Assert.AreEqual(0, table.ForeignKeys.Count);
        }

        [Test]
        public void Parse_CompositeKeyTable()
        {
            Table table = Table.Parse(COMPOSITE_PRIMARY_KEYS);
            Assert.AreEqual(TABLE_NAME, table.Name);
            ColumnExpected(table.Columns[COLUMN_ONE], COLUMN_ONE, new DataType() { Type = DataTypes.INT }, false);
            ColumnExpected(table.Columns[COLUMN_TWO], COLUMN_TWO, new DataType() { Type = DataTypes.INT }, false);
            Assert.AreEqual(2, table.PrimaryKeys.Count);
            Assert.IsTrue(table.PrimaryKeys.Contains(COLUMN_ONE));
            Assert.IsTrue(table.PrimaryKeys.Contains(COLUMN_TWO));
            Assert.AreEqual(0, table.ForeignKeys.Count);
        }

        [Test]
        public void GetDependencies_NoDependencies()
        {
            Dictionary<string, Column> columns = new Dictionary<string, Column>();
            columns.Add(COLUMN_ONE, new Column() { Name = COLUMN_ONE, Type = new DataType() { Type = DataTypes.INT } });
            Table table = new Table(TABLE_NAME, columns, new HashSet<string>() { COLUMN_ONE }, new Dictionary<string, ForeignKey>());
            Assert.IsFalse(table.GetDependencies().Any());
        }

        [Test]
        public void GetDependencies_Dependencies()
        {
            Dictionary<string, Column> columns = new Dictionary<string, Column>();
            columns.Add(COLUMN_ONE, new Column() { Name = COLUMN_ONE, Type = new DataType() { Type = DataTypes.INT } });
            Dictionary<string, ForeignKey> foreignKeys = new Dictionary<string, ForeignKey>();
            foreignKeys.Add(COLUMN_ONE, new ForeignKey() { LocalColumn = COLUMN_ONE, ForeignTable = PARENT_TABLE_NAME, ForeignColumn = PARENT_COLUMN_NAME });
            Table table = new Table(TABLE_NAME, columns, new HashSet<string>() { COLUMN_ONE }, foreignKeys);
            Assert.IsTrue(table.GetDependencies().Any());
            Assert.AreEqual(1, table.GetDependencies().Count());
        }

        [Test]
        public void GetDependencies_DuplicateDependencies()
        {
            Dictionary<string, Column> columns = new Dictionary<string, Column>();
            columns.Add(COLUMN_ONE, new Column() { Name = COLUMN_ONE, Type = new DataType() { Type = DataTypes.INT } });
            columns.Add(COLUMN_TWO, new Column() { Name = COLUMN_TWO, Type = new DataType() { Type = DataTypes.INT } });
            Dictionary<string, ForeignKey> foreignKeys = new Dictionary<string, ForeignKey>();
            foreignKeys.Add(COLUMN_ONE, new ForeignKey() { LocalColumn = COLUMN_ONE, ForeignTable = PARENT_TABLE_NAME, ForeignColumn = PARENT_COLUMN_NAME });
            foreignKeys.Add(COLUMN_TWO, new ForeignKey() { LocalColumn = COLUMN_TWO, ForeignTable = PARENT_TABLE_NAME, ForeignColumn = PARENT_COLUMN_NAME });
            Table table = new Table(TABLE_NAME, columns, new HashSet<string>() { COLUMN_ONE }, foreignKeys);
            Assert.IsTrue(table.GetDependencies().Any());
            Assert.AreEqual(1, table.GetDependencies().Count());
        }

        private void ColumnExpected(Column column, string name, DataType type, bool nullable = true, bool autoIncrement = false)
        {
            Assert.AreEqual(name, column.Name);
            Assert.AreEqual(type, column.Type);
            Assert.AreEqual(nullable, column.Nullable);
            Assert.AreEqual(autoIncrement, column.AutoIncrement);
        }
    }
}
