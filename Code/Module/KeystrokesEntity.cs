using Celeste.Mod.StaminaMeter;
using Monocle;

namespace Celeste.Mod.Mia.KeystrokesEntity
{
	class KeyStokesEntity : Entity
	{
		private LargeStaminaMeterDisplay largeStaminaDisplay;

		public KeyStokesEntity()
		{
			Tag = Tags.TransitionUpdate | Tags.PauseUpdate | TagsExt.SubHUD;
		}
        public override void Added(Scene scene)
		{
			base.Added(scene);
			largeStaminaDisplay = new LargeStaminaMeterDisplay();
			Add(largeStaminaDisplay);
		}
	}
}