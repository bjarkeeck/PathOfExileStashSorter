using HtmlAgilityPack;
using Microsoft.CSharp;
using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using System.Xml.Linq;

namespace POEStashSorterModels
{
    public static class PoeSorter
    {
        private static Dictionary<string, BitmapImage> downloadedImages = new Dictionary<string, BitmapImage>();
        private static League selectedLeague;
        private static ComboBox ddlSortMode { get; set; }
        private static ComboBox ddlSortOption { get; set; }
        public static Canvas ItemCanvas { get; private set; }
        public static League SelectedLeague
        {
            get
            {
                if (selectedLeague == null)
                    selectedLeague = PoeSorter.Leagues.FirstOrDefault(x => x.Name == Settings.Instance.LastSelectedLeague) ?? PoeSorter.Leagues.First();

                return selectedLeague;
            }
            private set
            {
                if (Settings.Instance.LastSelectedLeague != value.Name)
                {
                    Settings.Instance.LastSelectedLeague = value.Name;
                    Settings.Instance.SaveChanges();
                }
                selectedLeague = value;
            }
        }

        private static SortingAlgorithm selectedSortingAlgorithm;
        public static SortingAlgorithm SelectedSortingAlgorithm
        {
            get
            {
                if (selectedSortingAlgorithm == null && SortingAlgorithms != null)
                {
                    selectedSortingAlgorithm = SortingAlgorithms.FirstOrDefault();
                }
                return selectedSortingAlgorithm;
            }
            set
            {
                ddlSortMode.SelectedItem = value;
                selectedSortingAlgorithm = value;
            }
        }

        public static List<League> Leagues { get; private set; }
        public static List<Character> Character { get; private set; }
        public static Tab SelectedTab { get; private set; }
        public static Tab SelectedTabSorted { get; private set; }
        public static List<SortingAlgorithm> SortingAlgorithms = new List<SortingAlgorithm>();
        public static Dispatcher Dispatcher { get; private set; }
        public static bool Initialized { get; private set; }
        public static void Initialize(Canvas itemCanvas, Dispatcher dispatcher, ComboBox ddlSortMode, ComboBox ddlSortOption)
        {
            Initialized = true;
            var sortingAlgorithmsExtern = GetAlgorithmsAssembly()
                .GetTypes().Where(x => typeof(SortingAlgorithm).IsAssignableFrom(x) && x != typeof(SortingAlgorithm))
                .ToList();

            var sortingAlgorithmsExternIntern = Assembly.GetExecutingAssembly()
                .GetTypes().Where(x => typeof(SortingAlgorithm).IsAssignableFrom(x) && x != typeof(SortingAlgorithm))
                .Where(x => sortingAlgorithmsExtern.Any(c => c.Name == x.Name) == false)
                .ToList();

            foreach (var item in sortingAlgorithmsExtern)
                SortingAlgorithms.Add((SortingAlgorithm)Activator.CreateInstance(item));

            foreach (var item in sortingAlgorithmsExternIntern)
                SortingAlgorithms.Add((SortingAlgorithm)Activator.CreateInstance(item));

            PoeSorter.ddlSortMode = ddlSortMode;
            PoeSorter.ddlSortOption = ddlSortOption;
            PoeSorter.Dispatcher = dispatcher;
            PoeSorter.ItemCanvas = itemCanvas;
            PoeSorter.Leagues = PoeConnector.FetchLeagues();
            PoeSorter.Character = PoeConnector.FetchCharecters();
        }

        public static void ReloadAlgorithms()
        {
            Assembly assembly = GetAlgorithmsAssembly();
            if (assembly != null)
            {
                var sortingAlgorithmsExtern = assembly
                    .GetTypes().Where(x => typeof(SortingAlgorithm).IsAssignableFrom(x) && x != typeof(SortingAlgorithm))
                    .ToList();

                //Replace exsisting
                int i = 0;
                foreach (var item in SortingAlgorithms.ToList())
                {
                    var type = sortingAlgorithmsExtern.FirstOrDefault(x => x.Name == item.GetType().Name);
                    if (type != null)
                        SortingAlgorithms[i] = (SortingAlgorithm)Activator.CreateInstance(type);
                    i++;
                }

                //add new
                foreach (var item in SortingAlgorithms.ToList())
                {
                    var type = sortingAlgorithmsExtern.FirstOrDefault(x => x.Name == item.GetType().Name);
                    if (type == null)
                        SortingAlgorithms.Add((SortingAlgorithm)Activator.CreateInstance(type));
                }

                //Re sort...
                Tab tmpTab = SelectedTab;
                SelectedTab = null;
                SetSelectedTab(tmpTab);
            }

        }

        private static Assembly GetAlgorithmsAssembly()
        {
            CSharpCodeProvider provider = new CSharpCodeProvider();
            CompilerParameters parameters = new CompilerParameters();


            // refrences all dll files from bin folder
            foreach (FileInfo file in (new DirectoryInfo(AppDomain.CurrentDomain.BaseDirectory)).GetFiles("*.dll"))
                parameters.ReferencedAssemblies.Add(file.Name);

            parameters.GenerateInMemory = true;
            parameters.GenerateExecutable = false;
            if (Debugger.IsAttached)
            {
                parameters.GenerateInMemory = false;
                parameters.TempFiles = new TempFileCollection(Environment.GetEnvironmentVariable("TEMP"), true);
                parameters.IncludeDebugInformation = true;
            }

            string csFolder = (Debugger.IsAttached ? "../source\\PoeStashSorter\\SortingAlgorithms" : "\\SortingAlgorithms");
            DirectoryInfo sortingAlgorithmFolder = new DirectoryInfo(AppDomain.CurrentDomain.BaseDirectory + csFolder);
            string[] csFiles = sortingAlgorithmFolder.GetFiles("*.cs").Select(x => x.FullName).ToArray();

            CompilerResults results = provider.CompileAssemblyFromFile(parameters, csFiles);

            if (results.Errors.HasErrors)
            {
                StringBuilder sb = new StringBuilder();
                foreach (CompilerError error in results.Errors)
                    sb.AppendLine(String.Format("Error ({0}): {1} \n", error.ErrorNumber, error.ErrorText));
                MessageBox.Show(sb.ToString());
            }
            else
            {
                return results.CompiledAssembly;
            }
            return null;

        }

        public static async void SetSelectedTab(Tab tab)
        {
            if (tab != null)
            {
                // Unselect current tab
                if (SelectedTab != null)
                {
                    SelectedTab.IsSelected = false;
                    SelectedTabSorted.IsSelected = false;

                    //if (SelectedTab.Items != null)
                    //{
                    //    SelectedTab.Items.ForEach(x => x.Image.Visibility = Visibility.Hidden);
                    //}
                    ////Remove old sorted tab preview
                    //if (SelectedTabSorted != null)
                    //SelectedTabSorted.Items.ForEach(x => ItemCanvas.Children.Remove(x.Image));
                }
                // Set new selected tab
                tab.IsSelected = true;
                SelectedTab = tab;

                // Download selected tab;
                if (tab.Items == null)
                    tab.Items = (await PoeConnector.FetchTabAsync(tab.Index, tab.League)).Items;
                tab.Items.ForEach(x => x.Tab = tab);

                // If the tab still is selected after download is complete.
                if (tab.IsSelected)
                {
                    tab.Items.ForEach(x => x.Image.Visibility = Visibility.Visible);
                    SelectSortingAlgoritm(null);
                }
            }
        }
        static bool init = false;

        private static void SortTab(Tab tab)
        {
            if (init && tab.IsSelected)
            {
                //Remove old sorted tab preview
                if (SelectedTabSorted != null)
                {
                    SelectedTabSorted.IsSelected = false;
                    //SelectedTabSorted.Items.ForEach(x => ItemCanvas.Children.Remove(x.Image));
                    ItemCanvas.Children.Clear();
                }
                SelectedTabSorted = SelectedSortingAlgorithm.SortTab(tab);

                SelectedTab.Items.ForEach(x => ItemCanvas.Children.Add(x.Image));
                SelectedTabSorted.Items.ForEach(x => ItemCanvas.Children.Add(x.Image));


            }
            init = true;
        }

        private static bool selectSortOption = true;

        public static void SelectSortingAlgoritm(SortingAlgorithm sort)
        {
            if (SelectedTab.Items != null)
            {
                if (sort == null)
                {
                    var sortingAlgoInfo = Settings.Instance.GetSortingAlgorithmForTab(SelectedTab);
                    sort = SortingAlgorithms.FirstOrDefault(x => x.Name == sortingAlgoInfo.Name) ?? SortingAlgorithms.FirstOrDefault();
                    sort.SortOption.SelectedOption = sortingAlgoInfo.Option;
                }
                else
                {
                    Settings.Instance.SetSortingAlgorithmForTab(sort.Name, sort.SortOption.SelectedOption, SelectedTab);
                }
                SelectedSortingAlgorithm = sort;
                selectSortOption = false;
                ddlSortOption.ItemsSource = SelectedSortingAlgorithm.SortOption.Options;
                ddlSortOption.SelectedItem = SelectedSortingAlgorithm.SortOption.SelectedOption;
                selectSortOption = true;
                SortTab(SelectedTab);
            }
        }

        public static void ChangeLeague(League league)
        {
            if (league != SelectedLeague)
            {
                SelectedLeague = league;
                SetSelectedTab(league.Tabs.FirstOrDefault());
            }
        }

        public static void SelectSortOption(string option)
        {
            if (selectSortOption)
            {
                SelectedSortingAlgorithm.SortOption.SelectedOption = option ?? SelectedSortingAlgorithm.SortOption.Options.FirstOrDefault();
                Settings.Instance.SetSortingAlgorithmForTab(SelectedSortingAlgorithm.Name, SelectedSortingAlgorithm.SortOption.SelectedOption, SelectedTab);
                SortTab(SelectedTab);
            }
        }

        public static void StartSorting()
        {
            new Thread(() =>
            {
                SelectedSortingAlgorithm.StartSorting(SelectedTab, SelectedTabSorted);
            }).Start();
        }

        public static int SortingSpeed { get; set; }

    }
}
