﻿using Ceras;
using Compression.BSA;
using VFS;
using Wabbajack.Common;
using Wabbajack.Lib.Downloaders;

namespace Wabbajack.Lib
{
    public class CerasConfig
    {
        public static SerializerConfig Config = new SerializerConfig
        {
            KnownTypes =
            {
                typeof(ModList), typeof(Game), typeof(Directive), typeof(IgnoredDirectly),
                typeof(NoMatch), typeof(InlineFile), typeof(PropertyType), typeof(CleanedESM),
                typeof(RemappedInlineFile), typeof(FromArchive), typeof(CreateBSA), typeof(PatchedFromArchive),
                typeof(SourcePatch), typeof(MergedPatch), typeof(Archive), typeof(IndexedArchive), typeof(IndexedEntry),
                typeof(ArchiveMeta),
                typeof(IndexedArchiveEntry), typeof(BSAIndexedEntry), typeof(VirtualFile),
                typeof(ArchiveStateObject), typeof(FileStateObject), typeof(IDownloader),
                typeof(IUrlDownloader), typeof(AbstractDownloadState), typeof(ManualDownloader.State),
                typeof(DropboxDownloader), typeof(GoogleDriveDownloader.State), typeof(HTTPDownloader.State),
                typeof(MegaDownloader.State), typeof(ModDBDownloader.State), typeof(NexusDownloader.State),
                typeof(BSAStateObject), typeof(BSAFileStateObject), typeof(BA2StateObject), typeof(BA2DX10EntryState),
                typeof(BA2FileEntryState), typeof(MediaFireDownloader.State)
            }
        };
    }
}
