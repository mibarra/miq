using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.TeamFoundation.Client;
using Microsoft.TeamFoundation.WorkItemTracking.Client;
using System.Net;
using Microsoft.TeamFoundation.VersionControl.Client;

namespace Miq.Tests.Nursery
{
    [TestClass]
    public class TFSClientVersionControlTests
    {
        [TestMethod]
        public void TestMethod1()
        {
            ICredentials credential = new NetworkCredential("luis.miguel.ibarra@gmail.com", "4jKRwEahd");
            var tfs = new TfsTeamProjectCollection(new Uri("https://nonspinning.visualstudio.com/DefaultCollection/"), credential);
            tfs.EnsureAuthenticated();
            var vc = tfs.GetService<VersionControlServer>();

            var projects = vc.GetAllTeamProjects(false);

            var x = vc.QueryHistory("$/", RecursionType.Full);
            foreach (var changeset in x)
            {
                foreach (var change in changeset.Changes)
                {
                    var y = change.ChangeType;
                }
            }
                //            var x = vc.GetChangeset
            //var y = vc.GetChangesForChangeset
        }
    }
}
