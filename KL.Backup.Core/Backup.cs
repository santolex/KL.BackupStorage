using System;
using KL.Backup.Core.Abstractions;

namespace KL.Backup.Core
{
    public class Backup : IBackup
    {
        public string FileName { get; }
        public DateTime Created { get; }

        public Backup(string fileName, DateTime created)
        {
            FileName = fileName;
            Created = created;
        }
    }
}