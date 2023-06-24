﻿namespace Streamfox.Server.Persistence
{
    using System.IO;
    using System.Linq;

    using Streamfox.Server.Persistence.Operations;
    using Streamfox.Server.VideoManagement;

    public class DiskVideoLoader : IVideoLoader, IThumbnailExistenceChecker
    {
        private readonly IFileLister _fileLister;

        private readonly IFileReadOpener _videoFileReadOpener;

        private readonly IFileReadOpener _thumbnailFileReadOpener;

        private readonly IFileExistenceChecker _thumbnailExistenceChecker;

        public DiskVideoLoader(
                IFileLister fileLister, IFileReadOpener videoFileReadOpener,
                IFileReadOpener thumbnailFileReadOpener, IFileExistenceChecker thumbnailExistenceChecker)
        {
            _fileLister = fileLister;
            _videoFileReadOpener = videoFileReadOpener;
            _thumbnailFileReadOpener = thumbnailFileReadOpener;
            _thumbnailExistenceChecker = thumbnailExistenceChecker;
        }

        public Stream LoadVideo(VideoId videoId)
        {
            return _videoFileReadOpener.OpenRead(videoId.ToString());
        }

        public Stream LoadThumbnail(VideoId videoId)
        {
            return _thumbnailFileReadOpener.OpenRead(videoId.ToString());
        }

        public VideoId[] ListLabels()
        {
            return _fileLister.ListFiles()
                              .Select(Path.GetFileName)
                              .Select(long.Parse)
                              .OrderBy(id => id)
                              .Select(id => new VideoId(id))
                              .ToArray();
        }

        public bool ThumbnailExists(VideoId videoId)
        {
            return _thumbnailExistenceChecker.Exists(videoId.ToString());
        }
    }
}