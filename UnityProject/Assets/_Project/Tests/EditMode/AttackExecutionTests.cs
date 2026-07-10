using NUnit.Framework;
using TinyVanguard.Combat;

namespace TinyVanguard.Tests.EditMode
{
    public sealed class AttackExecutionTests
    {
        [Test]
        public void SameTargetRegistersOnlyOncePerExecution()
        {
            var execution = new AttackExecution(1);
            var target = new object();

            Assert.That(execution.TryRegisterTarget(target), Is.True);
            Assert.That(execution.TryRegisterTarget(target), Is.False);
            Assert.That(execution.RegisteredTargetCount, Is.EqualTo(1));
        }

        [Test]
        public void NewExecutionCanRegisterSameTargetAgain()
        {
            var target = new object();
            var first = new AttackExecution(1);
            var second = new AttackExecution(2);

            Assert.That(first.TryRegisterTarget(target), Is.True);
            Assert.That(second.TryRegisterTarget(target), Is.True);
        }
    }
}
