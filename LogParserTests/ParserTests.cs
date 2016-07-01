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
    public class ParserTests
    {
        //[TestMethod()]
        //public void ParseFileTest()
        //{
        //    Parser p = new Parser("C:\\EDLOGS\\");
        //}

        //[TestMethod()]
        //public void ParseFileTest()
        //{
        //    Assert.IsTrue(true);
        //}

        //[TestMethod()]
        //public void ReadSystemTest()
        //{
        //    //Arrange
        //    Parser p = new Parser();
        //    string line = "{15:04:23} System:\"Ross 41\" StarPos:(7.094,-6.500,-27.094)ly Body:1 RelPos:(-7.13563e+06,9.40997e+06,8.41996e+06)km Supercruise";
        //    string expected = "\"Ross 41\"";

        //    //Act
        //    string actual = p.ReadSystem(line);

        //    //Assert
        //    Assert.AreEqual(expected, actual);
        //}

        //[TestMethod()]
        //public void ReadLineTime()
        //{
        //    //Arrange
        //    Parser p = new Parser();
        //    string line = "{15:04:23} System:\"Ross 41\" StarPos:(7.094,-6.500,-27.094)ly Body:1 RelPos:(-7.13563e+06,9.40997e+06,8.41996e+06)km Supercruise";
        //    DateTime expected = DateTime.Parse("15:04:23");

        //    //Act
        //    DateTime actual = p.ReadLineTime(line);

        //    //Assert
        //    Assert.AreEqual(expected, actual);
        //}

        //[TestMethod()]
        //public void ReadStarPosTest()
        //{
        //    //Arrange
        //    Parser p = new Parser();
        //    string line = "{15:04:23} System:\"Ross 41\" StarPos:(7.094,-6.500,-27.094)ly Body:1 RelPos:(-7.13563e+06,9.40997e+06,8.41996e+06)km Supercruise";
        //    System.Windows.Media.Media3D.Vector3D expected =
        //        new System.Windows.Media.Media3D.Vector3D(7.094d, -6.500d, -27.094d);

        //    //Act
        //    System.Windows.Media.Media3D.Vector3D actual = p.ReadStarPos(line);

        //    //Assert
        //    Assert.AreEqual(expected, actual);
        //}

        //[TestMethod()]
        //public void ReadScreenshotFilenameTest()
        //{
        //    //Arrange
        //    Parser p = new Parser();
        //    string line = @"{15:03:41} SCREENSHOT: Saved \ED_Pictures\Screenshot_0026.bmp , w:2560, h:1440, pitch:10240, sourceFormat:R8G8B8A8un_In8";
        //    string expected = "\\ED_Pictures\\Screenshot_0026.bmp";

        //    //Act
        //    string actual = p.ReadScreenshotFilename(line);

        //    //Assert
        //    Assert.AreEqual(expected, actual);
        //}

        //[TestMethod()]
        //public void ReadHeaderLocalDateTimeTest()
        //{
        //    //Arrange
        //    Parser p = new Parser();
        //    string line = @"16-05-25-15:02 GMT Daylight Time  (14:02 GMT) - part 1";
        //    DateTime expected = new DateTime(2016, 05, 25, 15, 02, 0);

        //    //Act
        //    DateTime actual = p.ReadHeaderLocalDateTime(line);

        //    //Assert
        //    Assert.AreEqual(expected, actual);
        //}
    }
}