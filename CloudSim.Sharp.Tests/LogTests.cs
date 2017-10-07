using System;
using NUnit.Framework;
using System.IO;

namespace CloudSim.Sharp.Tests
{
    [TestFixture]
    public class LogTests
    {
        private MemoryStream _logStream;

        [SetUp]
        public void Init()
        {
            _logStream = new MemoryStream();
            Log.SetOutput(_logStream);
        }


        [TearDown]
        public void Cleanup()
        {
            _logStream.Dispose();
            _logStream = null;
        }

        [Test]
        public void LogWriteAndReadTextSuccessful()
        {
            string testText = "Test text 1";
            Log.Write(testText);
            Assert.AreEqual(testText, Log.GetLogText());
        }

        [Test]
        public void LogWriteWithDisabledReadEmpty()
        {
            string testText = "Test text 1";
            Log.Disable();
            Log.Write(testText);

            Assert.IsFalse(Log.GetLogText().Contains(testText));
            Assert.IsEmpty(Log.GetLogText());
        }

        [Test]
        public void LogWriteConcatLine()
        {
            string s1 = "Hello ", s2 = ", ", s3 = "World!";
            Log.WriteConcatLine(s1, s2, s3);

            Assert.AreEqual(Log.GetLogText(), s1 + s2 + s3 + Environment.NewLine);
        }
    }
}