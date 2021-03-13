using System;

using NUnit.Framework;

using HergBot.SqlParser.Data.MySQL;

namespace SqlParser_Tests
{
    public class ForeignKey_Tests
    {
        private const string LOCAL_COLUMN = "Local_Field";

        private const string FOREIGN_TABLE = "ForeignTable";

        private const string FOREIGN_COLUMN = "Foreign_Field";

        [Test]
        public void Parse_Valid()
        {
            ForeignKey key = ForeignKey.Parse($"FOREIGN KEY ({LOCAL_COLUMN}) {FOREIGN_TABLE}({FOREIGN_COLUMN})");
            Assert.AreEqual(LOCAL_COLUMN, key.LocalColumn);
            Assert.AreEqual(FOREIGN_TABLE, key.ForeignTable);
            Assert.AreEqual(FOREIGN_COLUMN, key.ForeignColumn);
        }

        [TestCase("")]
        [TestCase("FOREIGN KEY () ()")]
        [TestCase("FOREIGN KEY (Local_Field) ()")]
        [TestCase("FOREIGN KEY (Local_Field) ForeignTable()")]
        public void Parse_Invalid(string text)
        {
            Assert.Throws<FormatException>(() => ForeignKey.Parse(text));
        }
    }
}
