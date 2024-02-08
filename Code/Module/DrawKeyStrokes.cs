using System;
using Celeste.Mod.Mia.UtilsClass;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste.Mod.KeyStrokes
{
    public abstract class KeyStrokesComponent : Component
    {
        protected Player player;
        protected float drawStamina;
        protected float currentStamina;
        protected float displayTimer;
        protected Level level;

        protected Color color;
        protected Color colorDark;
        protected Color[] fillColor = { Calc.HexToColor("#000000"), Calc.HexToColor("#000000"), Calc.HexToColor("#000000"), Calc.HexToColor("#000000"), Calc.HexToColor("#000000"), Calc.HexToColor("#000000"), Calc.HexToColor("#000000") };

        protected int[] inputs;

        public KeyStrokesComponent()
            : base(active: true, visible: true)
        {
        }

        public override void Added(Entity entity)
        {
            base.Added(entity);
            level = SceneAs<Level>();
            // hopefully getting player once is enough, it hasn't caused any issues yet
            player = level.Tracker.GetEntity<Player>();
        }

        public override void Update()
        {
            base.Update();
            inputs = Utils.GetInputs();
        }

        public override void Render()
        {
            for (int i = 0; i < inputs.Length; i++)
            {
                fillColor[i] = (inputs[i] == 1) ? Color.Red : Color.Black;
            }
            color = Calc.HexToColor("#00FF00");
            colorDark = Color.Lerp(color, Calc.HexToColor("#000000"), 0.5f);
        }
    }
}