using NUnit.Framework;

using SqlParser.Data.MySQL;
using System;

namespace SqlParser_Tests
{
    public class DataType_Tests
    {
        private const string REGULAR_TYPE = "INT";

        private const string SIZE_TYPE = "VARCHAR(100)";

        [Test]
        public void Parse_RegularType()
        {
            DataType type = DataType.Parse(REGULAR_TYPE);
            Assert.AreEqual(DataTypes.INT, type.Type);
            Assert.IsNull(type.Size);
        }

        [Test]
        public void Parse_SizeType()
        {
            DataType type = DataType.Parse(SIZE_TYPE);
            Assert.AreEqual(DataTypes.VARCHAR, type.Type);
            Assert.AreEqual(100, type.Size);
        }

        [TestCase("unknown", typeof(ArgumentException), TestName = "Parse_UnknownType")]
        [TestCase("", typeof(ArgumentException), TestName = "Parse_EmptyString")]
        [TestCase(null, typeof(ArgumentNullException), TestName = "Parse_Null")]
        public void Parse_InvalidInput(string value, Type exceptionType)
        {
            TestDelegate testMethod = () => DataType.Parse(value);
            typeof(Assert).GetMethod("Throws", new Type[] { typeof(TestDelegate) }).MakeGenericMethod(exceptionType).Invoke(null, new object[] { testMethod });
        }

        [TestCase(DataTypes.INT, null, 351, TestName = "GetHashCode_OnlyType")]
        [TestCase(DataTypes.VARCHAR, 100, 63960, TestName = "GetHashCode_TypeAndSize")]
        public void GetHashCode_Tests(DataTypes type, int? size, int expected)
        {
            DataType testType = new DataType()
            {
                Type = type,
                Size = size
            };
            Assert.AreEqual(expected, testType.GetHashCode());
        }

        [TestCase(null, TestName = "Equals_Null")]
        [TestCase("string", TestName = "Equals_OtherType")]
        public void Equals_OtherTypeTests(object compareTo)
        {
            DataType testType = new DataType()
            {
                Type = DataTypes.INT
            };
            Assert.IsFalse(testType.Equals(compareTo));
        }

        [TestCase(null, TestName = "Equals_SameTypeNullSize")]
        [TestCase(105, TestName = "Equals_SameTypeDifferentSize")]
        public void Equals_SameTypeDifferentSize(int? size)
        {
            DataType typeA = new DataType()
            {
                Type = DataTypes.VARCHAR,
                Size = 100
            };
            DataType typeB = new DataType()
            {
                Type = DataTypes.VARCHAR,
                Size = size
            };
            Assert.IsFalse(typeA.Equals(typeB));
        }

        [TestCase(null, TestName = "Equals_SameTypeNoSize")]
        [TestCase(100, TestName = "Equals_SameTypeSameSize")]
        public void Equals_SameTypeSameSize(int? size)
        {
            DataType typeA = new DataType()
            {
                Type = DataTypes.VARCHAR,
                Size = size
            };
            DataType typeB = new DataType()
            {
                Type = DataTypes.VARCHAR,
                Size = size
            };
            Assert.IsTrue(typeA.Equals(typeB));
        }

        [Test]
        public void EqualityOperators_BothNull()
        {
            DataType typeA = null;
            DataType typeB = null;
            Assert.IsTrue(typeA == typeB);
            Assert.IsFalse(typeA != typeB);
        }

        [Test]
        public void EqualityOperators_OneNull()
        {
            DataType typeA = new DataType()
            {
                Type = DataTypes.INT
            };
            DataType typeB = null;
            Assert.IsFalse(typeA == typeB);
            Assert.IsTrue(typeA != typeB);
        }
    }
}
