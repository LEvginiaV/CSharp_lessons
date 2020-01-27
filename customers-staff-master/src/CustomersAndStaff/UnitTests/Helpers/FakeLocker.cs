using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

using SKBKontur.Catalogue.CassandraUtils.DistributedLock.Locker;

namespace Market.CustomersAndStaff.UnitTests.Helpers
{
    public class FakeLocker : ILocker
    {
        public int Delay { get; set; }

        public async Task<ILock> LockAsync(string lockId)
        {
            var lockRoot = GetLockRoot(lockId);
            lockRoot.WaitOne();
            if(Delay > 0)
            {
                await Task.Delay(500);
            }
            return new FakeLock(lockRoot, lockId);
        }

        public Task<ILock> TryGetLockAsync(string lockId)
        {
            throw new System.NotImplementedException();
        }

        public void ReleaseLock(string lockId, string threadId)
        {
            throw new System.NotImplementedException();
        }

        public void Dispose()
        {
            
        }

        private Semaphore GetLockRoot(string key)
        {
            lock(lockRoots)
            {
                if(!lockRoots.TryGetValue(key, out var lockRoot))
                {
                    lockRoot = new Semaphore(1, 1);
                    lockRoots[key] = lockRoot;
                }

                return lockRoot;
            }
        }

        private readonly Dictionary<string, Semaphore> lockRoots = new Dictionary<string, Semaphore>();
    }

    public class FakeLock : ILock
    {
        public FakeLock(Semaphore lockRoot, string lockId)
        {
            this.lockRoot = lockRoot;
            LockId = lockId;
        }

        public void Dispose()
        {
            lockRoot.Release();
        }

        public string LockId { get; }
        public string ThreadId => throw new NotImplementedException();

        private readonly Semaphore lockRoot;
    }
}