﻿namespace Amicitia
{
    using AtlusLibSharp;
    using AtlusLibSharp.Generic.Archives;
    using AtlusLibSharp.Persona3.Archives;
    using AtlusLibSharp.SMT3.ChunkResources.Graphics;
    using ResourceWrappers;
    using System.IO;

    internal static class ResourceFactory
    {
        public static ResourceWrapper GetResource(string path)
        {
            return GetResource(Path.GetFileName(path), new FileStream(path, FileMode.Open), SupportedFileHandler.GetSupportedFileIndex(path));
        }

        public static ResourceWrapper GetResource(string text, Stream stream)
        {
            return GetResource(text, stream, SupportedFileHandler.GetSupportedFileIndex(text));
        }

        public static ResourceWrapper GetResource(string text, Stream stream, int supportedFileIndex)
        {
            switch (SupportedFileHandler.GetType(supportedFileIndex))
            {
                case SupportedFileType.GenericPAK:
                    return new GenericPAKFileWrapper(text, new GenericPAK(stream, 252));
                case SupportedFileType.TMX:
                    return new TMXWrapper(text, TMXChunk.LoadFrom(stream));
                case SupportedFileType.SPR:
                    return new SPRWrapper(text, SPRChunk.LoadFrom(stream));
                case SupportedFileType.BVPArchive:
                    return new BVPArchiveWrapper(text, new BVPArchive(stream));
                default:
                    return new ResourceWrapper(text, new GenericBinaryFile(stream));
            }
        }
    }
}