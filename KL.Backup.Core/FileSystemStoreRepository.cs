using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using KL.Backup.Core.Abstractions;

namespace KL.Backup.Core
{
    public class FileSystemStoreRepository : IStorageRepository
    {
        private readonly string _directoryName;

        public FileSystemStoreRepository(string directoryName)
        {
            if (!Directory.Exists(directoryName))
                throw new Exception($"Directory does not exist. {directoryName}");
            
            _directoryName = directoryName;
        }
        
        public IEnumerable<IBackup> List()
        {
            return Directory.GetFiles(_directoryName)
                .Select(x => new FileInfo(x))
                .Select(x => new Backup(x.FullName, x.CreationTime));
        }
        
        public void Remove(IBackup backup)
        {
            throw new NotImplementedException();
        }

        public void Remove(IEnumerable<IBackup> backups)
        {
            
            foreach (var backup in backups)
            {
                try
                {
                    File.Delete(backup.FileName);
                }
                catch (Exception e)
                {
                    //Some logging logic should be here
                    Console.WriteLine(e);
                }
            }
        }
    }
}