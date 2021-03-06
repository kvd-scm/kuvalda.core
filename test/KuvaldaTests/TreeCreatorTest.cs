using System;
using System.Collections.Generic;
using System.IO.Abstractions.TestingHelpers;
using System.Threading.Tasks;
using Kuvalda.Core;
using NUnit.Framework;

namespace KuvaldaTests
{
    [TestFixture]
    public class TreeCreatorTest
    {
        [Test]
        public async Task Test_ShouldCreateFileTree()
        {
            // Arrange
            var file = new MockFileData("content")
            {
                LastWriteTime = DateTime.Today
            };
            var fs = new MockFileSystem(new Dictionary<string, MockFileData>
            {
                { @"c:/file", file },
            });

            var creator = new TreeCreator(fs);
            
            // Arrange
            var tree = await creator.Create("c:/");
            
            // Assert
            Assert.AreEqual(
                new TreeNodeFolder("") {Nodes = new[] {new TreeNodeFile("file", DateTime.Today.ToUniversalTime())}}, tree);
        }
        
        [Test]
        public async Task Test_ShouldCreateFolder()
        {
            // Arrange
            var folder = new MockDirectoryData();
            var fs = new MockFileSystem(new Dictionary<string, MockFileData>
            {
                { @"c:/folder", folder },
            });

            var creator = new TreeCreator(fs);
            
            // Arrange
            var tree = await creator.Create("c:/");
            
            // Assert
            Assert.AreEqual(
                new TreeNodeFolder("") {Nodes = new[] {new TreeNodeFolder("folder"), }}, tree);
        }
        
        [Test]
        public async Task Test_ShouldCreateFolderWithFiles()
        {
            // Arrange
            var file = new MockFileData("content")
            {
                LastWriteTime = DateTime.Today
            };
            var file1 = new MockFileData("content2")
            {
                LastWriteTime = DateTime.Today
            };
            var fs = new MockFileSystem(new Dictionary<string, MockFileData>
            {
                { @"c:/file", file },
                { @"c:/file1", file1 },
            });

            var creator = new TreeCreator(fs);
            
            // Arrange
            var tree = await creator.Create("c:/");
            
            // Assert
            Assert.AreEqual(
                new TreeNodeFolder("")
                {
                    Nodes = new[]
                    {
                        new TreeNodeFile("file", DateTime.Today.ToUniversalTime()),
                        new TreeNodeFile("file1", DateTime.Today.ToUniversalTime())
                    }
                }, tree);
        }
    }
}