using System;
using KL.Backup.Core;
using KL.Backup.Core.Abstractions;
using KL.Backup.Core.StoragePolicyRules;

namespace KL.Backup.Service
{
    class Program
    {
        static void Main(string[] args)
        {
            IBackupWatcher watcher = GetWatcher();

            Console.CancelKeyPress += (sender, eventArgs) => watcher.Stop();

            using (watcher)
            {
                watcher.Start();
                Console.ReadLine();
                watcher.Stop();
            }
        }

        private static BackupWatcher GetWatcher()
        {
            var policy = KLStoragePolicyFactory.Get();
            
            var repo = new FileSystemStoreRepository("D:\\backups");
            
            return new KLBackupWatcher(policy,repo,TimeSpan.FromHours(1));
        }
    }

    public static class KLStoragePolicyFactory
    {
        public static IStoragePolicy Get()
        {
            return new StoragePolicy(new[]
            {
                new CompositeRule(new IStoragePolicyRule[]
                {
                    new OlderThanRule(TimeSpan.FromDays(14.0)),
                    new NoMoreThanRule(1)
                }),
                new CompositeRule(new IStoragePolicyRule[]
                {
                    new OlderThanRule(TimeSpan.FromDays(7.0)),
                    new NotOlderThanRule(TimeSpan.FromDays(14.0)),
                    new NoMoreThanRule(4)
                }),
                new CompositeRule(new IStoragePolicyRule[]
                {
                    new OlderThanRule(TimeSpan.FromDays(3)),
                    new NotOlderThanRule(TimeSpan.FromDays(7)),
                    new NoMoreThanRule(4)
                }),
                new CompositeRule(new IStoragePolicyRule[]
                {
                    new OlderThanRule(TimeSpan.FromDays(0)),
                    new NotOlderThanRule(TimeSpan.FromDays(3)),
                    new NoMoreThanRule(4)
                }),
            });
        }
    }
}
