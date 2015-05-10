// <copyright company="XATA">
//      Copyright (c) 2015, All Right Reserved
// </copyright>
// <author>Ivan Ivchenko</author>
// <email>iivchenko@live.com</email>

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.IO.Packaging;
using System.Linq;
using System.Xml.Serialization;
using Darch.Deduplication;
using Darch.Deduplication.Maps;

// TODO: Make thread safe in the future.
namespace Darch.ViewModels.Archiving
{
    public class Archive : IArchive
    {
        private readonly Package _package;
        private readonly Repository _repository;
        private readonly Stream _metadata;

        public Archive(Package package, Repository repository, Stream metadata)
        {
            _package = package;
            _repository = repository;
            _metadata = metadata;
        }

        public IEnumerable<ArchiveFile> Files
        {
            get
            {
                return GetFiles();
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1309:UseOrdinalStringComparison", MessageId = "System.String.Equals(System.String,System.StringComparison)", Justification = "Some where in the future fix this.")]
        public IMapProcessor Add(string path)
        {
            var file = new FileInfo(path);

            if (GetFiles().Any(x => x.Name.Equals(file.Name, StringComparison.InvariantCultureIgnoreCase)))
            {
                throw new InvalidOperationException("File duplicate!!");
            }

            var stream = file.OpenRead();
            var writer = _repository.Write(stream);
            
            writer.StatusChanged += (sender, args) =>
            {
                if (args.Status == MapStatus.Succeeded)
                {
                    stream.Close();

                    var archiveFile = new ArchiveFile
                    {
                        Name = file.Name,
                        Id = writer.Id,
                        Length = file.Length
                    };

                    var files = GetFiles();
                    files.Add(archiveFile);

                    SaveMetadata(files);
                }
            };

            return writer;
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1309:UseOrdinalStringComparison", MessageId = "System.String.Equals(System.String,System.StringComparison)", Justification = "Fix in the future!")]
        public IMapProcessor Remove(string name)
        {
            var file = GetFiles().FirstOrDefault(x => x.Name.Equals(name, StringComparison.InvariantCultureIgnoreCase));
            if (file == null)
            {
                throw new InvalidOperationException("File doesn't exist!");
            }

            var remover = _repository.Delete(file.Id);

            remover.StatusChanged += (sender, args) =>
            {
                if (args.Status == MapStatus.Succeeded)
                {
                    var files = GetFiles();

                    files.Remove(files.First(x => x.Name.Equals(name, StringComparison.InvariantCultureIgnoreCase)));

                    SaveMetadata(files);
                }
            };

            return remover;
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1309:UseOrdinalStringComparison", MessageId = "System.String.Equals(System.String,System.StringComparison)", Justification = "Delete in the future.!")]
        public IMapProcessor Extract(string name)
        {
            var file = GetFiles().FirstOrDefault(x => x.Name.Equals(name, StringComparison.InvariantCultureIgnoreCase));
            if (file == null)
            {
                throw new InvalidOperationException("File doesn't exist!");
            }
            
            var stream = File.Create(file.Name);
            var extractor = _repository.Read(file.Id, stream);

            extractor.StatusChanged += (sender, args) =>
            {
                if (args.Status == MapStatus.Succeeded)
                {
                    stream.Close();
                }
            };

            return extractor;
        }

        public void Flush()
        {
            _package.Flush();
        }

        public void Close()
        {
            _package.Close();
        }

        private Collection<ArchiveFile> GetFiles()
        {
            if (_metadata.Length == 0)
            {
                // TODO: This is a hack. So in the future make metadata initialization during archive creation.
                return new Collection<ArchiveFile>();
            }

            var serializer = new XmlSerializer(typeof(Collection<ArchiveFile>));
            
            _metadata.Seek(0, SeekOrigin.Begin);

            return (Collection<ArchiveFile>)serializer.Deserialize(_metadata);
        }

        private void SaveMetadata(Collection<ArchiveFile> files)
        {
            _metadata.SetLength(0);
            var serializer = new XmlSerializer(typeof(Collection<ArchiveFile>));
            serializer.Serialize(_metadata, files);
        }
    }
}
