using System.IO;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Input;
using Microsoft.VisualStudio.ExtensionManager;
using Microsoft.VisualStudio.Shell;
using EnvDTE;
using System.Windows.Threading;
using System;
using Atmel.Studio.Services;

namespace FourWalledCubicle.VizMap
{
    public partial class VisualMap : UserControl
    {
        private bool _isZooming = false;

        private readonly DTE _DTE; 
        
        private BuildEvents _buildEvents;
        private SolutionEvents _solutionEvents;

        private SymbolParser _symbolParser;

        public VisualMap()
        {
            InitializeComponent();

            _symbolParser = new SymbolParser();

            _DTE = Package.GetGlobalService(typeof(DTE)) as DTE;

            _buildEvents = _DTE.Events.BuildEvents;
            _buildEvents.OnBuildDone += (Scope, Action) => ReloadProjectSymbols();

            _solutionEvents = _DTE.Events.SolutionEvents;
            _solutionEvents.Opened += () => UpdateProjectList();
            _solutionEvents.AfterClosing += () => UpdateProjectList();
            _solutionEvents.ProjectAdded += (p) => UpdateProjectList();
            _solutionEvents.ProjectRemoved += (p) => UpdateProjectList();
            _solutionEvents.ProjectRenamed += (p, s) => UpdateProjectList();

            cmbProjectList.SelectionChanged += (s, e) => ReloadProjectSymbols();

            svMapScroller.KeyDown += vacVisualMapView_KeyChange;
            svMapScroller.KeyUp += vacVisualMapView_KeyChange;
            svMapScroller.PreviewMouseWheel += vacVisualMapView_MouseWheel;

            UpdateProjectList();
        }

        private void ReloadProjectSymbols()
        {
            vacVisualMapView.Symbols.Clear();
            _symbolParser.ClearSymbols();

            if (cmbProjectList.SelectedItem == null)
                return;

            String projectName = (String)cmbProjectList.SelectedItem.ToString();
            if (String.IsNullOrEmpty(projectName))
                return;

            Project project = null;
            try
            {
                project = _DTE.Solution.Item(projectName);
            }
            catch { }

            if (project == null)
                return;

            IProjectHandle projectNode = project.Object as IProjectHandle;
            if (projectNode == null)
                return;

            string elfPath = null;

            string previousDirectory = Directory.GetCurrentDirectory();
            try
            {
                Directory.SetCurrentDirectory(Path.GetDirectoryName(project.FullName));
                elfPath = Path.GetFullPath(Path.Combine(projectNode.GetProperty("OutputDirectory"), projectNode.GetProperty("OutputFilename") + ".elf"));
            }
            finally
            {
                Directory.SetCurrentDirectory(previousDirectory);
            }

            string toolchainNMPath = GetContentLocation("GNU-NM");

            if (File.Exists(elfPath) && File.Exists(toolchainNMPath))
            {
                DispatcherOperation dispatcher = Dispatcher.BeginInvoke(
                        new Action(
                                () => _symbolParser.ReloadSymbols(elfPath, toolchainNMPath)
                            )
                    );

                dispatcher.Completed += (s, e) =>
                {
                    vacVisualMapView.Symbols.AddRange(_symbolParser.SymbolSizes);
                    vacVisualMapView.InvalidateVisual();
                };

                if (dispatcher.Status == DispatcherOperationStatus.Completed)
                {
                    vacVisualMapView.Symbols.AddRange(_symbolParser.SymbolSizes);
                    vacVisualMapView.InvalidateVisual();
                }
            }
        }

        private void UpdateProjectList()
        {
            string currentSelection = string.Empty;

            if (cmbProjectList.SelectedItem != null)
                currentSelection = cmbProjectList.SelectedItem.ToString();

            cmbProjectList.Items.Clear();
            foreach (Project p in _DTE.Solution.Projects)
            {
                if (File.Exists(p.FullName))
                    cmbProjectList.Items.Add(p.UniqueName);
            }

            if (string.IsNullOrEmpty(currentSelection))
                currentSelection = GetStartupProjectName(_DTE);

            try
            {
                cmbProjectList.SelectedItem = _DTE.Solution.Projects.Item(currentSelection).UniqueName;
            }
            catch { }

            ReloadProjectSymbols();
        }

        void vacVisualMapView_KeyChange(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.LeftCtrl)
                _isZooming = e.IsDown;

            if (e.Key == Key.OemPlus)
            {
                if (_isZooming)
                    vacVisualMapView.Zoom -= 10;
            }
            else if (e.Key == Key.OemMinus)
            {
                if (_isZooming)
                    vacVisualMapView.Zoom += 10;
            }
        }

        void vacVisualMapView_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (_isZooming)
            {
                e.Handled = true;
                vacVisualMapView.Zoom += (e.Delta > 0 ? 1 : -1);
            }
        }

        private static string GetStartupProjectName(DTE myDTE)
        {
            string startupProjectName = string.Empty;
            SolutionBuild solutionBuild = myDTE.Solution.SolutionBuild;

            if ((solutionBuild == null) || (solutionBuild.StartupProjects == null))
                return startupProjectName;

            foreach (string el in (Array)solutionBuild.StartupProjects)
                startupProjectName += el;

            return startupProjectName;
        }

        private static string GetContentLocation(string contentName)
        {
            IVsExtensionManager extensionManagerService = Package.GetGlobalService(typeof(SVsExtensionManager)) as IVsExtensionManager;
            if (extensionManagerService == null)
                return null;

            IInstalledExtension dsExt = null;
            if (extensionManagerService.TryGetInstalledExtension(GuidList.guidVizMapPkgString, out dsExt) == false)
                return null;

            string contentPath = null;

            try
            {
                string contentRelativePath = dsExt.Content.Where(c => c.ContentTypeName == contentName).First().RelativePath;
                contentPath = Path.Combine(dsExt.InstallPath, contentRelativePath);
            }
            catch { }

            return contentPath;
        }    
    }
}