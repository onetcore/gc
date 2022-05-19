using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using gp.Transfers;

namespace gp.Tests
{
    [TestClass()]
    public class CSharpFileTests
    {
        private string CurrentDirectory => Path.Combine(Directory.GetCurrentDirectory(), "Result");
        private const string DirectoryName = "../../";
        private const string FileName = DirectoryName + "Professions/Profession.txt";

        [TestMethod()]
        public void CSharpDataTest()
        {
            var file = new FileInfo(FileName);
            var transfer = new ClassDataTransfer(file);
            transfer.Save(CurrentDirectory, true);
            Assert.IsTrue(true);
        }


        [TestMethod()]
        public void CSharpManagerTest()
        {
            var file = new FileInfo(FileName);
            var transfer = new ClassManagerTransfer(file);
            transfer.Save(CurrentDirectory, true);
            Assert.IsTrue(true);
        }

        [TestMethod()]
        public void RazorTest()
        {
            var file = new FileInfo(FileName);
            var transfer = new RazorPageTransfer(file);
            transfer.Save(CurrentDirectory, true);
            Assert.IsTrue(true);
        }

        //[TestMethod()]
        //public void BlazorTest()
        //{
        //    var file = new FileInfo(@"NavMenu.razor");
        //    var transfer = new BlazorCodeBehindTransfer(file);
        //    transfer.Save(CurrentDirectory, true);
        //    Assert.IsTrue(true);
        //}
    }
}