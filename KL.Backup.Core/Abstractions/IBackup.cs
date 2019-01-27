using System;

namespace KL.Backup.Core.Abstractions
{
    public interface IBackup
    {
        string   FileName { get; }
        DateTime Created { get; }
    }
}