﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Essential.Diagnostics.Tests
{
    [TestClass()]
    public class StructuredDataTests
    {
        [TestMethod()]
        public void PropertyValue()
        {
            var template = "x{a}";

            IStructuredData data = new StructuredData(template, "A");
            var actual = data.Properties;

            Assert.AreEqual(1, actual.Count);
            Assert.AreEqual("a", actual.Keys.First());
            Assert.AreEqual("A", actual["a"]);
        }

        [TestMethod()]
        public void PropertyValueInOrder()
        {
            var template = "{b}x{a}";
            var a = "A";
            var b = "B";

            IStructuredData data = new StructuredData(template, a, b);
            var actual = data.Properties;

            Assert.AreEqual(2, actual.Count);
            Assert.AreEqual("A", actual["b"]);
            Assert.AreEqual("B", actual["a"]);
        }

        [TestMethod()]
        public void PropertyWithException()
        {
            var template = "x{a}";

            IStructuredData data;
            try
            {
                throw new ApplicationException("B");
            }
            catch (Exception ex)
            {
                data = new StructuredData(ex, template, "A");
            }
            var actual = data.Properties;

            Assert.AreEqual(2, actual.Count);
            Assert.AreEqual("A", actual["a"]);
            Assert.AreEqual("B", ((Exception)actual["Exception"]).Message);
        }

        [TestMethod()]
        public void PropertyWithAdditionalData()
        {
            var template = "x{a}";
            var additional = new Dictionary<string, object>() { { "b", "B" } };

            IStructuredData data = new StructuredData(additional, template, "A");
            var actual = data.Properties;

            Assert.AreEqual(2, actual.Count);
            Assert.AreEqual("A", actual["a"]);
            Assert.AreEqual("B", actual["b"]);
        }

        [TestMethod()]
        public void StringFormatProperty()
        {
            var template = "x{a}";
            var properties = new Dictionary<string, object>() {
                { "a", 1 },
            };                  

            IStructuredData data = new StructuredData(properties, template);
            var actual = data.ToString();

            Assert.AreEqual("x1", actual);
        }

        [TestMethod()]
        public void StringFormatValue()
        {
            var template = "x{a}";

            IStructuredData data = new StructuredData(template, 1);
            var actual = data.ToString();

            Assert.AreEqual("x1", actual);
        }

        [TestMethod()]
        public void StringValuesInOrder()
        {
            var template = "{b}x{a}";
            var a = "A";
            var b = "B";

            IStructuredData data = new StructuredData(template, a, b);
            var actual = data.ToString();

            Assert.AreEqual("AxB", actual);
        }

        [TestMethod()]
        public void StringValuesMissingAreBlank()
        {
            var template = "{a}x{b}y";
            var a = "A";

            IStructuredData data = new StructuredData(template, a);
            var actual = data.ToString();

            Assert.AreEqual("Axy", actual);
        }

        [TestMethod()]
        public void StringValuesOverrideProperties()
        {
            var template = "{a}x{b}";
            var a = "A";

            var properties = new Dictionary<string, object>() {
                { "a", 1 },
                { "b", 2 },
            };

            IStructuredData data = new StructuredData(properties, template, a);
            var actual = data.ToString();

            Assert.AreEqual("Ax2", actual);
        }

        [TestMethod()]
        public void StringPropertyOnly()
        {
            var properties = new Dictionary<string, object>() {
                { "a", 1 },
            };

            IStructuredData data = new StructuredData(properties);
            var actual = data.ToString();

            Assert.AreEqual("a=1", actual);
        }

        [TestMethod()]
        public void StringMessageAndProperty()
        {
            var properties = new Dictionary<string, object>() {
                { "a", 1 },
            };

            IStructuredData data = new StructuredData(properties, "X");
            var actual = data.ToString();

            Assert.AreEqual("X; a=1", actual);
        }

        [TestMethod()]
        public void StringPropetiesAndTemplateValues()
        {
            var properties = new Dictionary<string, object>() {
                { "a", 1 },
                { "b", 2 },
                { "d", 4 },
            };

            IStructuredData data = new StructuredData(properties, "x{b}y{c}z{d}", "B", "C");
            var actual = data.ToString();

            Assert.AreEqual(@"xByCz4; a=1", actual);
        }

        [TestMethod()]
        public void StringExtraValues()
        {
            IStructuredData data = new StructuredData("x{a}", 1, 2);
            var actual = data.ToString();

            Assert.AreEqual(@"x1; Extra1=2", actual);
        }

        [TestMethod()]
        public void StringWithException()
        {
            var template = "x{a}";

            IStructuredData data;
            try
            {
                throw new ApplicationException("B");
            }
            catch (Exception ex)
            {
                data = new StructuredData(ex, template, "A");
            }
            var actual = data.ToString();

            var expectedMessageWithStartOfStackTrace = "xA; Exception='System.ApplicationException: B\r\n   at Essential.Diagnostics.Tests.StructuredDataTests.StringWithException() ";
            StringAssert.StartsWith(actual, expectedMessageWithStartOfStackTrace);
        }

        [TestMethod()]
        public void StringWithAdditionalData()
        {
            var template = "x{a}";
            var additional = new Dictionary<string, object>() { { "b", "B" } };

            IStructuredData data = new StructuredData(additional, template, "A");
            var actual = data.ToString();

            Assert.AreEqual("xA; b='B'", actual);
        }

        [TestMethod()]
        public void StringWithDuplicateTemplateValues()
        {
            IStructuredData data = new StructuredData("{a}x{a}", "A", "B");
            var actual = data.ToString();

            Assert.AreEqual("BxB", actual);
        }
    }
}