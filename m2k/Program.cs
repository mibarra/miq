using System;
using System.Diagnostics;
using System.Windows.Forms;

namespace miq.m2k
{
    class Program
    {
        private const string KindleGenApp = @"tools\kindlegen.exe ";

        [STAThread]
        static void Main(string[] args)
        {
            Application.EnableVisualStyles();
            // get outputfolder
            var outputFolder = GetOutputFolder();
            if (outputFolder == null)
                return;

            // download MSDN magazine
            var msdn = new MsMagazineRepository(outputFolder, "http://msdn.microsoft.com/magazine/rss/default.aspx?z=z&iss=1", "MSDN Magazine");
            try
            {
                msdn.Process();
                ConvertToMobi(msdn.OpfFileName, msdn.OutputMobiFileName);
            }
            catch (Exception ex)
            {
                Console.WriteLine(string.Format("failed to download articles: {0}", ex.ToString()));
            }

            // "http://technet.microsoft.com/en-us/magazine/rss/default.aspx?issue=tue";
            // Technet Magazine
            //// download Technet magazine
            //var technet = new TechnetMagazineRepository(outputFolder);
            //try
            //{
            //	technet.Process();
            //	ConvertToMobi(technet.OpfFileName, technet.OutputMobiFileName);
            //}
            //catch(Exception ex)
            //{
            //	Console.WriteLine(string.Format("failed to download articles: {0}", ex.ToString()));
            //}
        }

        private static string GetOutputFolder()
        {
            string outputFolder = null;
            FolderBrowserDialog folder = new FolderBrowserDialog();
            folder.SelectedPath = Properties.Settings.Default.OutputFolder;
            folder.ShowNewFolderButton = true;
            if (folder.ShowDialog() == DialogResult.OK)
            {
                outputFolder = folder.SelectedPath;
                Properties.Settings.Default.OutputFolder = outputFolder;
                Properties.Settings.Default.Save();
            }
            return outputFolder;
        }

        private static void ConvertToMobi(string sourFilePath, string outputFilePath)
        {
            //var currentFolder = Directory.GetCurrentDirectory();
            //var convertor = Path.Combine(currentFolder, KindleGenApp);
            var stratInfo = new ProcessStartInfo { FileName = KindleGenApp, Arguments = string.Format("\"{0}\"", sourFilePath) };
            RunAndWaitForExit(stratInfo, 10000);
            Console.WriteLine("Generated {0}", outputFilePath);
        }

        private static void RunAndWaitForExit(ProcessStartInfo startInfo, int milliSeconds)
        {
            startInfo.RedirectStandardOutput = true;
            startInfo.WindowStyle = ProcessWindowStyle.Hidden;
            startInfo.UseShellExecute = false;
            var convertProcess = Process.Start(startInfo);
            convertProcess.WaitForExit(milliSeconds);
        }
    }
}
