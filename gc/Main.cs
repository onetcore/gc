using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;

namespace gc
{
    class Main
    {
        private readonly IDictionary<string, string> _args;
        public Main(IDictionary<string, string> args)
        {
            _args = args;
        }

        private const string Entry = "gc.json";
        public void Run()
        {
            if (!_args.TryGetValue("--f", out var file) && !_args.TryGetValue("--file", out file))
                file = Entry;
            else if (!file.EndsWith(Entry, StringComparison.OrdinalIgnoreCase))
                file = Path.Combine(file, Entry);
            var configFile = new FileInfo(file);
            if (!configFile.Exists)
            {
                WriteLine(ConsoleColor.DarkRed, "未找到“{0}”文件！", configFile.FullName);
                return;
            }
            WriteLine(ConsoleColor.DarkGreen, "载入“{0}”文件！", configFile.FullName);
            var project = Load(configFile);
            if (project == null)
                return;

            if (!_args.TryGetValue("--o", out var dir) && !_args.TryGetValue("--out", out dir))
                dir = "dist";
            if (dir.Length > 1 && dir[1] != ':')
                dir = Path.Combine(configFile.DirectoryName, dir);
            CreatePaths(project, dir, out var netCore, out var typeScript);
            Write(project, netCore, typeScript);
            WriteLine(ConsoleColor.DarkGreen, "恭喜你，你已经完成了操作，请到“{0}”中进行查看！", dir);
        }

        private void CreatePaths(Project project, string path, out string netCore, out string typeScript)
        {
            var dir = new DirectoryInfo(Path.Combine(path, "ts", project.Namespace.ToLower()));
            if (!dir.Exists) dir.Create();
            typeScript = dir.FullName;
            dir = new DirectoryInfo(Path.Combine(path, "netcore", project.Namespace));
            if (!dir.Exists) dir.Create();
            netCore = dir.FullName;
        }

        private Project Load(FileInfo configFile)
        {
            var project = Read<Project>(configFile.FullName);
            if (project == null)
                return null;
            project.Namespace = project.Namespace.Trim(' ', ';', '.').NormalizedName();
            if (project.Using == null)
                project.Using = new List<string>();

            var classes = configFile.Directory?.GetFiles("*.json", SearchOption.AllDirectories)
                .Where(x => !x.Name.Equals(Entry))
                .ToArray();
            Load(project, classes, configFile.DirectoryName?.Length ?? 0);
            return project;
        }

        private static readonly JsonSerializerOptions _options = new JsonSerializerOptions
        {
            IgnoreNullValues = true,
            PropertyNameCaseInsensitive = true,
            AllowTrailingCommas = true,
        };

        private TModel Read<TModel>(string fileName)
        {
            var code = File.ReadAllText(fileName, Encoding.UTF8);
            var model = JsonSerializer.Deserialize<TModel>(code, _options);
            return model;
        }

        private readonly IDictionary<string, string> _namespaces = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        private void Load(Project project, FileInfo[] files, int basePathStart)
        {
            foreach (var file in files)
            {
                WriteLine(ConsoleColor.DarkGreen, "加载类文件：{0}", file.FullName);
                var @class = Read<Class>(file.FullName);
                project.Classes.Add(@class);
                if (@class.Namespace == null)
                {
                    if (!_namespaces.TryGetValue(file.DirectoryName, out var @namespace))
                    {
                        var names = file.DirectoryName?.Substring(basePathStart).Trim('/', '\\')
                            .Split(new[] { '/', '\\', '.' })
                            .Where(x => !string.IsNullOrEmpty(x))
                            .Select(x => x.NormalizedName());
                        if (names.Any())
                            @namespace = $"{project.Namespace}.{string.Join(".", names)}";
                        else
                            @namespace = project.Namespace;
                        _namespaces[file.DirectoryName] = @namespace;
                    }

                    @class.Namespace = @namespace;
                }

                @class.Name = (@class.Name ?? Path.GetFileNameWithoutExtension(file.Name)).NormalizedName();
                if (@class.Using == null || @class.Using.Count == 0)
                    @class.Using = project.Using;
                else
                    @class.Using.AddRange(project.Using);
                @class.Using = @class.Using.Distinct().ToList();
            }
        }

        private void Write(Project project, string netcore, string typescript)
        {
            foreach (var projectClass in project.Classes)
            {
                var directoryName = projectClass.Namespace.Substring(project.Namespace.Length).Trim('.').Replace('.', '\\');
                Write(projectClass, new Controller(projectClass), MakeDir(netcore, directoryName), MakeDir(typescript, directoryName));
            }
        }

        private string MakeDir(string path, string dir)
        {
            path = Path.Combine(path, dir);
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);
            return path;
        }

        private void Write(Class projectClass, Controller controller, string netcore, string typescript)
        {
            //netcore
            var path = Path.Combine(netcore, projectClass.Name + ".cs");
            WriteFile(path, projectClass.ToString());
            path = Path.Combine(netcore, projectClass.Name + "DataMigration.cs");
            WriteFile(path, projectClass.ToDataMigration());
            path = Path.Combine(netcore, projectClass.Name + "Controller.cs");
            WriteFile(path, controller.ToString());
            //typescript
            path = Path.Combine(typescript, "model.d.ts");
            WriteFile(path, projectClass.ToTypeScriptString());
            path = Path.Combine(typescript, "service.ts");
            WriteFile(path, controller.ToTypeScriptString());
            path = Path.Combine(typescript, "create.tsx");
            WriteFile(path, projectClass.ToTypeScriptFormString());
            path = Path.Combine(typescript, "update.tsx");
            WriteFile(path, projectClass.ToTypeScriptFormString(false));
            path = Path.Combine(typescript, "index.tsx");
            WriteFile(path, projectClass.ToTypeScriptTableString());
        }

        private void WriteFile(string path, string code)
        {
            WriteLine(ConsoleColor.DarkGreen, "写入代码文件：{0}", path);
            using (var fs = new FileStream(path, FileMode.Create, FileAccess.Write))
            using (var sw = new StreamWriter(fs, Encoding.UTF8))
                sw.WriteLine(code);
        }

        /// <summary>
        /// 输出控制台。
        /// </summary>
        public static void WriteLine(ConsoleColor color, string message, params object[] args)
        {
            message = string.Format(message, args);
            Console.ForegroundColor = color;
            Console.WriteLine(message);
            Console.ResetColor();
        }
    }
}