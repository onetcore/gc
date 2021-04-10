using Microsoft.VisualStudio.TestTools.UnitTesting;
using gp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using gp.Transfers;

namespace gp.Tests
{
    [TestClass()]
    public class CSharpFileTests
    {
        [TestMethod()]
        public void CSharpDataTest()
        {
            var file = FileElement.FromFile("Document.cs");
            var transfer = new ClassDataTransfer(file, "Document");
            transfer.Save(Directory.GetCurrentDirectory());
            Assert.IsTrue(true);
        }


        [TestMethod()]
        public void CSharpManagerTest()
        {
            var file = FileElement.FromFile("Document.cs");
            var transfer = new ClassManagerTransfer(file, "Document");
            transfer.Save(Directory.GetCurrentDirectory());
            Assert.IsTrue(true);
        }


        [TestMethod()]
        public void CSharpResourceTest()
        {
            var transfer = new ResourceTransfer(@"..\..\Properties\Resources.resx", "gpTests");
            transfer.Save(@"..\..\");
            Assert.IsTrue(true);
        }
    }
}