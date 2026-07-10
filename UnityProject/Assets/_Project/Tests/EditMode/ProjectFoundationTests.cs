using NUnit.Framework;

namespace TinyVanguard.Tests.EditMode
{
    public sealed class ProjectFoundationTests
    {
        [Test]
        public void ProjectIdentity_HasStableFoundationValues()
        {
            Assert.That(ProjectIdentity.ProductName, Is.EqualTo("Project Tiny Vanguard"));
            Assert.That(ProjectIdentity.FoundationVersion, Is.EqualTo("0.1.0"));
        }
    }
}
