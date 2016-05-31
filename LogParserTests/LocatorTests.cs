using Microsoft.VisualStudio.TestTools.UnitTesting;
using LogParser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogParser.Tests
{
    [TestClass()]
    public class LocatorTests
    {
        [TestMethod()]
        public void FindLogFolderTest()
        {
            var log = Locator.FindProductFolder();
        }

        [TestMethod()]
        public void getNetlogsTest()
        {
            Locator.AppFolderPath = "C:\\edtest";
            var netlogs = Locator.getNetlogs();
        }
    }
}