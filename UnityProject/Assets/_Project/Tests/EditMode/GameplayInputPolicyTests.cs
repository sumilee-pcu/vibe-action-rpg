using NUnit.Framework;
using TinyVanguard.Session;

namespace TinyVanguard.Tests.EditMode
{
    public sealed class GameplayInputPolicyTests
    {
        [TestCase(GameSessionState.Playing, true, true, false)]
        [TestCase(GameSessionState.Paused, false, true, true)]
        [TestCase(GameSessionState.Victory, false, true, true)]
        [TestCase(GameSessionState.Defeat, false, true, true)]
        public void StateSelectsExpectedInputMaps(
            GameSessionState state,
            bool gameplay,
            bool system,
            bool ui)
        {
            var activation = GameplayInputPolicy.GetActivation(state);

            Assert.That(activation.Gameplay, Is.EqualTo(gameplay));
            Assert.That(activation.System, Is.EqualTo(system));
            Assert.That(activation.UI, Is.EqualTo(ui));
        }

        [TestCase(GameSessionState.Playing, true, GameSessionState.Paused)]
        [TestCase(GameSessionState.Paused, true, GameSessionState.Playing)]
        [TestCase(GameSessionState.Victory, false, GameSessionState.Victory)]
        [TestCase(GameSessionState.Defeat, false, GameSessionState.Defeat)]
        public void PauseToggleOnlyChangesPlayingAndPaused(
            GameSessionState current,
            bool expectedChanged,
            GameSessionState expectedNext)
        {
            var changed = GameplayInputPolicy.TryGetPauseToggleState(
                current,
                out var next);

            Assert.That(changed, Is.EqualTo(expectedChanged));
            Assert.That(next, Is.EqualTo(expectedNext));
        }
    }
}
