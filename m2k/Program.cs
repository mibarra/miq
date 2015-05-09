using System;
using System.Diagnostics;
using System.Globalization;
using System.Windows.Forms;

namespace Miq.M2K
{
    class Program
    {
        private const string KindleGenApp = @"tools\kindlegen.exe ";

        [STAThread]
        static void Main()
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
                Console.WriteLine(string.Format(CultureInfo.CurrentCulture, Properties.Resources.FailedArticlesDownload, ex.ToString()));
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
            using (FolderBrowserDialog folder = new FolderBrowserDialog())
            {
                folder.SelectedPath = Properties.Settings.Default.OutputFolder;
                folder.ShowNewFolderButton = true;
                if (folder.ShowDialog() == DialogResult.OK)
                {
                    outputFolder = folder.SelectedPath;
                    Properties.Settings.Default.OutputFolder = outputFolder;
                    Properties.Settings.Default.Save();
                }
            }
            return outputFolder;
        }

        private static void ConvertToMobi(string sourceFilePath, string outputFilePath)
        {
            var startInfo = new ProcessStartInfo { FileName = KindleGenApp, Arguments = "\"" + sourceFilePath + "\"" };
            RunAndWaitForExit(startInfo, 10000);
            Console.WriteLine(Properties.Resources.OutputGenerated, outputFilePath);
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
