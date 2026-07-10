using System.Collections;
using NUnit.Framework;
using UnityEngine.TestTools;

namespace TinyVanguard.Tests.PlayMode
{
    public sealed class ProjectFoundationPlayModeTests
    {
        [UnityTest]
        public IEnumerator RuntimeAssembly_LoadsInPlayMode()
        {
            yield return null;

            Assert.That(ProjectIdentity.ProductName, Is.Not.Empty);
        }
    }
}
