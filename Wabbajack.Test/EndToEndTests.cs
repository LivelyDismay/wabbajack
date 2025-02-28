﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Alphaleonis.Win32.Filesystem;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using VFS;
using Wabbajack.Common;
using Wabbajack.Lib;
using Wabbajack.Lib.Downloaders;
using Wabbajack.Lib.NexusApi;

namespace Wabbajack.Test
{
    [TestClass]
    public class EndToEndTests
    {
        private const string DOWNLOAD_FOLDER = "downloads";

        private TestUtils utils = new TestUtils();

        public TestContext TestContext { get; set; }

        [TestInitialize]
        public void TestInitialize()
        {
            Consts.TestMode = true;

            utils = new TestUtils();
            utils.GameName = "Skyrim Special Edition";

            Utils.SetStatusFn((f, idx) => { });
            Utils.SetLoggerFn(f => TestContext.WriteLine(f));
            WorkQueue.Init((x, y, z) => { }, (min, max) => { });

            if (!Directory.Exists(DOWNLOAD_FOLDER))
                Directory.CreateDirectory(DOWNLOAD_FOLDER);

        }

        [TestMethod]
        public void CreateModlist()
        {
            var profile = utils.AddProfile("Default");
            var mod = utils.AddMod();

            DownloadAndInstall(
                "https://github.com/ModOrganizer2/modorganizer/releases/download/v2.2.1/Mod.Organizer.2.2.1.7z",
                "Mod.Organizer.2.2.1.7z",
                utils.MO2Folder);
            File.WriteAllLines(Path.Combine(utils.DownloadsFolder, "Mod.Organizer.2.2.1.7z.meta"),
                new List<string>
                {
                    "[General]",
                    "directURL=https://github.com/ModOrganizer2/modorganizer/releases/download/v2.2.1/Mod.Organizer.2.2.1.7z"
                });

            DownloadAndInstall(Game.SkyrimSpecialEdition, 12604, "SkyUI");

            utils.Configure();

            var modlist = CompileAndInstall(profile);

            utils.VerifyAllFiles();

            var loot_folder = Path.Combine(utils.InstallFolder, "LOOT Config Files");
            if (Directory.Exists(loot_folder))
                Directory.Delete(loot_folder, true);

            VirtualFileSystem.Reconfigure(utils.TestFolder);
            var compiler = new Compiler(utils.InstallFolder);
            compiler.MO2DownloadsFolder = Path.Combine(utils.DownloadsFolder);
            compiler.VFS.Reset();
            compiler.MO2Profile = profile;
            compiler.ShowReportWhenFinished = false;
            Assert.IsTrue(compiler.Compile());

        }

        private void DownloadAndInstall(string url, string filename, string mod_name = null)
        {
            var src = Path.Combine(DOWNLOAD_FOLDER, filename);
            if (!File.Exists(src))
            {
                var state = DownloadDispatcher.ResolveArchive(url);
                state.Download(new Archive() { Name = "Unknown"}, src);
            }

            if (!Directory.Exists(utils.DownloadsFolder))
            {
                Directory.CreateDirectory(utils.DownloadsFolder);
            }

            File.Copy(src, Path.Combine(utils.DownloadsFolder, filename));

            if (mod_name == null)
                FileExtractor.ExtractAll(src, utils.MO2Folder);
            else
                FileExtractor.ExtractAll(src, Path.Combine(utils.ModsFolder, mod_name));

        }

        private void DownloadAndInstall(Game game, int modid, string mod_name)
        {
            utils.AddMod(mod_name);
            var client = new NexusApiClient();
            var file = client.GetModFiles(game, modid).First(f => f.is_primary);
            var src = Path.Combine(DOWNLOAD_FOLDER, file.file_name);

            var ini = string.Join("\n",
                new List<string>
                {
                    "[General]",
                    $"gameName={GameRegistry.Games[game].MO2ArchiveName}",
                    $"modID={modid}",
                    $"fileID={file.file_id}"
                });

            if (!File.Exists(file.file_name))
            {

                var state = DownloadDispatcher.ResolveArchive(ini.LoadIniString());
                state.Download(src);
            }

            if (!Directory.Exists(utils.DownloadsFolder))
            {
                Directory.CreateDirectory(utils.DownloadsFolder);
            }

            var dest = Path.Combine(utils.DownloadsFolder, file.file_name);
            File.Copy(src, dest);

            FileExtractor.ExtractAll(src, Path.Combine(utils.ModsFolder, mod_name));

            File.WriteAllText(dest + ".meta", ini);
        }

        private ModList CompileAndInstall(string profile)
        {
            var compiler = ConfigureAndRunCompiler(profile);
            Install(compiler);
            return compiler.ModList;
        }

        private void Install(Compiler compiler)
        {
            var modlist = Installer.LoadFromFile(compiler.ModListOutputFile);
            var installer = new Installer(compiler.ModListOutputFile, modlist, utils.InstallFolder);
            installer.DownloadFolder = utils.DownloadsFolder;
            installer.GameFolder = utils.GameFolder;
            installer.Install();
        }

        private Compiler ConfigureAndRunCompiler(string profile)
        {
            VirtualFileSystem.Reconfigure(utils.TestFolder);
            var compiler = new Compiler(utils.MO2Folder);
            compiler.VFS.Reset();
            compiler.MO2Profile = profile;
            compiler.ShowReportWhenFinished = false;
            Assert.IsTrue(compiler.Compile());
            return compiler;
        }
    }
}
