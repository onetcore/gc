using Microsoft.VisualStudio.TestTools.UnitTesting;
using gp;
using System;
using System.IO;
using gp.Transfers;

namespace gp.Tests
{
    [TestClass()]
    public class CSharpFileTests
    {
        [TestMethod()]
        public void CSharpDataTest()
        {
            var file = new FileInfo("Document.cs");
            var transfer = new ClassDataTransfer(file);
            transfer.Save(Directory.GetCurrentDirectory());
            Assert.IsTrue(true);
        }


        [TestMethod()]
        public void CSharpManagerTest()
        {
            var file = new FileInfo("Document.cs");
            var transfer = new ClassManagerTransfer(file);
            transfer.Save(Directory.GetCurrentDirectory());
            Assert.IsTrue(true);
        }


        [TestMethod()]
        public void BlazorTest()
        {
            var file = new FileInfo(@"NavMenu.razor");
            var transfer = new BlazorCodeBehindTransfer(file);
            transfer.Save();
            Assert.IsTrue(true);
        }
    }
}