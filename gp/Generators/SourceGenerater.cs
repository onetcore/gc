using System;
using System.IO;
using System.Text;

namespace gp
{
    internal class SourceGenerater
    {
        public IServiceProvider ServiceProvider { get; }
        protected FileInfo SourceFile { get; }
        public SourceGenerater(IServiceProvider serviceProvider, FileInfo sourceFile)
        {
            ServiceProvider = serviceProvider;
            SourceFile = sourceFile;
        }

        public void ExecuteDataMigration()
        {
            Execute(@class =>
            {
                var path = Path.Combine(SourceFile.DirectoryName, @class.Name + "DataMigration.cs");
                WriteCode(path, @class.ToDataMigration());
            });
        }

        public void ExecuteController()
        {
            Execute(@class =>
            {
                var path = Path.Combine(SourceFile.DirectoryName, @class.Name + "Controller.cs");
                WriteCode(path, new Controller(@class).ToString());
            });
        }

        private void Execute(Action<Class> action)
        {
            var code = ReadCode();
            if (string.IsNullOrWhiteSpace(code))
                return;
            var parser = new SimpleCodeParser(code);
            foreach (var @class in parser.Classes)
            {
                action(@class);
            }
        }

        private string ReadCode()
        {
            using (var fs = new FileStream(SourceFile.FullName, FileMode.Open, FileAccess.Read))
            using (var sr = new StreamReader(fs, Encoding.UTF8))
                return sr.ReadToEnd();
        }

        private void WriteCode(string path, string code)
        {
            var directoryName = Path.GetDirectoryName(path);
            if (!Directory.Exists(directoryName)) Directory.CreateDirectory(directoryName);
            using (var fs = new FileStream(path, FileMode.Create, FileAccess.Write))
            using (var sw = new StreamWriter(fs, Encoding.UTF8))
                sw.WriteLine(code);
        }

        public void ExecuteTypeScript()
        {
            Execute(projectClass =>
            {
                var typescript = Path.Combine(SourceFile.DirectoryName, "ts", projectClass.Name);
                var path = Path.Combine(typescript, "model.d.ts");
                WriteCode(path, projectClass.ToTypeScriptString());
                path = Path.Combine(typescript, "service.ts");
                WriteCode(path, new Controller(projectClass).ToTypeScriptString());
                path = Path.Combine(typescript, "create.tsx");
                WriteCode(path, projectClass.ToTypeScriptFormString());
                path = Path.Combine(typescript, "update.tsx");
                WriteCode(path, projectClass.ToTypeScriptFormString(false));
                path = Path.Combine(typescript, "index.tsx");
                WriteCode(path, projectClass.ToTypeScriptTableString());
            });
        }
    }
}