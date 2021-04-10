using System;
using System.ComponentModel.Design;
using gp.Transfers;
using Microsoft.VisualStudio.Shell;
using Task = System.Threading.Tasks.Task;

namespace gp.Commands
{
    /// <summary>
    /// Command handler
    /// </summary>
    internal sealed class GpCommand
    {
        /// <summary>
        /// Command ID.
        /// </summary>
        public const int CommandId = 0x0100;
        public const int ControllerId = 0x0101;
        public const int TsId = 0x0102;

        /// <summary>
        /// Command menu group (command set GUID).
        /// </summary>
        public static readonly Guid CommandSet = new Guid("03945c9d-c259-4b3a-8703-d7433f913f71");

        /// <summary>
        /// VS Package that provides this command, not null.
        /// </summary>
        private readonly AsyncPackage _package;

        /// <summary>
        /// 服务提供者。
        /// </summary>
        public IServiceProvider ServiceProvider => _package;

        /// <summary>
        /// Initializes a new instance of the <see cref="GpCommand"/> class.
        /// Adds our command handlers for menu (commands must exist in the command table file)
        /// </summary>
        /// <param name="package">Owner package, not null.</param>
        /// <param name="commandService">Command service to add command to, not null.</param>
        private GpCommand(AsyncPackage package, OleMenuCommandService commandService)
        {
            _package = package ?? throw new ArgumentNullException(nameof(package));
            commandService = commandService ?? throw new ArgumentNullException(nameof(commandService));

            var cmdId = new CommandID(CommandSet, CommandId);
            var menuItem = new MenuCommand(Execute, cmdId);
            commandService.AddCommand(menuItem);

            var ctrlId = new CommandID(CommandSet, ControllerId);
            var ctrlItem = new MenuCommand(ExecuteManager, ctrlId);
            commandService.AddCommand(ctrlItem);

            var tsId = new CommandID(CommandSet, TsId);
            var tsItem = new MenuCommand(ExecuteResource, tsId);
            commandService.AddCommand(tsItem);
        }

        private void ExecuteResource(object sender, EventArgs e)
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            var sourceFile = ServiceProvider.GetCurrentFile();
            if (sourceFile?.Exists != true || !sourceFile.Name.Equals("Resources.resx", StringComparison.OrdinalIgnoreCase) || sourceFile.Directory?.Parent == null)
                return;
            var @namespace = sourceFile.Directory.Parent.Name;
            var directoryName = sourceFile.Directory.Parent.FullName;
            if (sourceFile.Directory.Name.Equals("Properties", StringComparison.OrdinalIgnoreCase))
            {
                directoryName = sourceFile.Directory.FullName;
                @namespace = $"{@namespace}.{sourceFile.Directory.Name}";
            }
            var transfer = new ResourceTransfer(sourceFile.FullName, @namespace);
            transfer.Save(directoryName);
        }

        private void ExecuteManager(object sender, EventArgs e)
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            var sourceFile = ServiceProvider.GetCurrentFile();
            if (sourceFile?.Exists != true || !sourceFile.Extension.Equals(".cs", StringComparison.OrdinalIgnoreCase))
                return;

            var transfer = new ClassManagerTransfer(FileElement.FromFile(sourceFile.FullName), sourceFile.Name);
            transfer.Save(sourceFile.DirectoryName);
        }

        /// <summary>
        /// Gets the instance of the command.
        /// </summary>
        public static GpCommand Instance
        {
            get;
            private set;
        }

        /// <summary>
        /// Initializes the singleton instance of the command.
        /// </summary>
        /// <param name="package">Owner package, not null.</param>
        public static async Task InitializeAsync(AsyncPackage package)
        {
            // Switch to the main thread - the call to AddCommand in GpCommand's constructor requires
            // the UI thread.
            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync(package.DisposalToken);
            OleMenuCommandService commandService = await package.GetServiceAsync(typeof(IMenuCommandService)) as OleMenuCommandService;
            Instance = new GpCommand(package, commandService);
        }

        /// <summary>
        /// This function is the callback used to execute the command when the menu item is clicked.
        /// See the constructor to see how the menu item is associated with this function using
        /// OleMenuCommandService service and MenuCommand class.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event args.</param>
        private void Execute(object sender, EventArgs e)
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            var sourceFile = ServiceProvider.GetCurrentFile();
            if (sourceFile?.Exists != true || !sourceFile.Extension.Equals(".cs", StringComparison.OrdinalIgnoreCase))
                return;

            var transfer = new ClassDataTransfer(FileElement.FromFile(sourceFile.FullName), sourceFile.Name);
            transfer.Save(sourceFile.DirectoryName);
        }
    }
}
