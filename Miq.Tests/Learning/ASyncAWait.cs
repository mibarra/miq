using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Miq.Tests.Learning
{
    [TestClass]
    public class ASyncAWait
    {
        [TestMethod]
        [TestCategory("Slow")]
        public async Task TestMethod1()
        {
            int x = await AnAsyncMethod();
            Assert.AreEqual(42, x);
        }

        private Task<int> AnAsyncMethod()
        {
            var taskCompletion = new TaskCompletionSource<int>();
            LaunchLengthyOperation(taskCompletion);
            return taskCompletion.Task;
        }

        private void LaunchLengthyOperation(TaskCompletionSource<int> taskCompletion)
        {
            Thread.Sleep(1000);
            taskCompletion.SetResult(42);
        }
    }
}