using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using KL.Backup.Core;
using KL.Backup.Core.Abstractions;
using KL.Backup.Core.StoragePolicyRules;
using KL.Backup.Service;
using Moq;
using Xunit;

namespace KL.Backup.Tests
{
    public class BackupTests
    {

        private static IBackup[] Backups { get; } = {
            new Core.Backup("1", DateTime.Now),
            new Core.Backup("2", DateTime.Now.Subtract(TimeSpan.FromDays(100))),
            new Core.Backup("3", DateTime.Now.Subtract(TimeSpan.FromDays(10))),
            new Core.Backup("4", DateTime.Now.Subtract(TimeSpan.FromDays(1))),
            new Core.Backup("5", DateTime.Now.Subtract(TimeSpan.FromDays(5)))
        };
        
        private static IStoragePolicy GetPolicy()
        {
            return new StoragePolicy(new IStoragePolicyRule[]
            {
                new OlderThanRule(TimeSpan.FromDays(6)),
                new NotOlderThanRule(TimeSpan.FromDays(4)),
                new NoMoreThanRule(2)
            });
        }

        [Fact]
        public void OlderThanRuleTest()
        {
            var rule = new OlderThanRule(TimeSpan.FromDays(6));

            var res = rule.Apply(Backups).ToArray();
            
            Assert.Equal(2,res.Length);
            Assert.Contains(Backups[1], res);
            Assert.Contains(Backups[2], res);
        }

        [Fact]
        public void NotOlderThanRuleTest()
        {
            var rule = new NotOlderThanRule(TimeSpan.FromDays(6));

            var res = rule.Apply(Backups).ToArray();
            
            Assert.Equal(3,res.Length);
            Assert.Contains(Backups[3], res);
            Assert.Contains(Backups[4], res);
            Assert.Contains(Backups[0], res);
        }

        [Fact]
        public void NoMoreThanRuleTest()
        {
            var rule = new NoMoreThanRule(3);

            var res = rule.Apply(Backups).ToArray();

            Assert.Equal(3,res.Length);
        }

        [Fact]
        public void CompositeRuleTest()
        {
            var rule = new CompositeRule(new IStoragePolicyRule[]
            {
                new OlderThanRule(TimeSpan.FromDays(0.5)),
                new NotOlderThanRule(TimeSpan.FromDays(50)),
                new NoMoreThanRule(2)
            });
            
            var res = rule.Apply(Backups).ToArray();

            Assert.Equal(2,res.Length);
            Assert.Contains(Backups[2], res);
            Assert.Contains(Backups[3], res);
        }

        [Fact]
        public void StoragePolicyTest()
        {
            var policy = GetPolicy();

            var res = policy.Apply(Backups).ToArray();
            
            Assert.Equal(4,res.Length);
            Assert.Contains(Backups[0], res);
            Assert.Contains(Backups[1], res);
            Assert.Contains(Backups[2], res);
            Assert.Contains(Backups[3], res);
        }

        [Fact]
        public void BackupWatcherApplyPolicyTest()
        {
            var mockRepo = new Mock<IStorageRepository>();
            mockRepo.Setup(x => x.List()).Returns(() => Backups);
            mockRepo.Setup(x => x.Remove(It.IsAny<IEnumerable<IBackup>>())).Callback<IEnumerable<IBackup>>(backups =>
            {
                Assert.Single(backups);
                Assert.Equal(Backups[4],backups.FirstOrDefault());
            });
                
            var mockWatcher = new Mock<BackupWatcher>(GetPolicy(), mockRepo.Object);
            mockWatcher.Setup(x => x.ApplyPolicy()).CallBase();
            var watcher = mockWatcher.Object;
            
            watcher.ApplyPolicy();
        }

        [Fact]
        public void KLBackupWatcherTest()
        {
            var path = Path.Combine(Path.GetTempPath(), "KL");
            if (Directory.Exists(path))
                Directory.Delete(path, true);
            Directory.CreateDirectory(path);
            
            var policy = KLStoragePolicyFactory.Get();
            var repo = new FileSystemStoreRepository(path);
            var watcher = new KLBackupWatcher(policy,repo,Timeout.InfiniteTimeSpan);

            CreateFile(path,20);
            CreateFile(path,22);
            CreateFile(path,15);
            
            CreateFile(path,11);
            CreateFile(path,12);
            CreateFile(path,8);

            CreateFile(path,4);
            CreateFile(path,4);
            CreateFile(path,5);
            CreateFile(path,6);
            CreateFile(path,4);
            CreateFile(path,5);
            CreateFile(path,6);

            CreateFile(path,1);
            CreateFile(path,1);
            CreateFile(path,0);
            CreateFile(path,2);
            CreateFile(path,1);
            CreateFile(path,2);
            
            watcher.ApplyPolicy();

            var files = Directory.GetFiles(path).Select(x => new FileInfo(x)).ToList();

            var now = DateTime.Now;
            var ts14 = TimeSpan.FromDays(14);
            var ts7 = TimeSpan.FromDays(7);
            var ts3 = TimeSpan.FromDays(3);
            Assert.Single(files.Where(x => now - x.CreationTime > ts14));

            Assert.Equal(3,files.Count(x =>
            {
                var dif = now - x.CreationTime;
                return dif > ts7 && dif < ts14;
            }));

            Assert.Equal(4,files.Count(x =>
            {
                var dif = now - x.CreationTime;
                return dif > ts3 && dif < ts7;
            }));
            
            Assert.Equal(4,files.Count(x =>
            {
                var dif = now - x.CreationTime;
                return dif > TimeSpan.Zero && dif < ts3;
            }));
            
            Directory.Delete(path, true);
        }

        private static void CreateFile(string path, int subDays)
        {
            var guid = Guid.NewGuid();
            var fileName = Path.Combine(path, guid.ToString()) + ".bin";
            File.Create(fileName).Dispose();
            File.SetCreationTime(fileName, DateTime.Now.AddDays(-subDays));
        }       
    }
}