using System;
using System.Drawing;
using Celeste.Mod.KeyStrokes;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste.Mod.StaminaMeter
{
    public class LargeStaminaMeterDisplay : KeyStrokesComponent
    {
        private static Vector2 UpPos = new Vector2(1572, 48);
        private static Vector2 DownPos = new Vector2(1572, 80);
        private static Vector2 RightPos = new Vector2(1604, 80);
        private static Vector2 LeftPos = new Vector2(1540, 80);

        private static Vector2 JumpPos = new Vector2(1650, 60);
        private static Vector2 DashPos = new Vector2(1682, 60);
        private static Vector2 ClimbPos = new Vector2(1714, 60);



        private static Vector2 meterSize = new Vector2(32, 32);
        private static Vector2 insideSize= new Vector2(30, 30);

        public override void Update()
        {
            base.Update();
            Visible = true;
        }

        public override void Render()
        {
            base.Render();



            // outline
            Draw.Rect(UpPos, meterSize.X , meterSize.Y, colorDark);
            Draw.Rect(DownPos, meterSize.X , meterSize.Y, colorDark);
            Draw.Rect(LeftPos, meterSize.X , meterSize.Y, colorDark);
            Draw.Rect(RightPos, meterSize.X , meterSize.Y, colorDark);
            Draw.Rect(JumpPos, meterSize.X , meterSize.Y, colorDark);
            Draw.Rect(DashPos, meterSize.X , meterSize.Y, colorDark);
            Draw.Rect(ClimbPos, meterSize.X , meterSize.Y, colorDark);

            Draw.Rect(UpPos + new Vector2(2,2), insideSize.X, insideSize.Y, fillColor[2]);
            Draw.Rect(DownPos + new Vector2(2, 2), insideSize.X, insideSize.Y, fillColor[3]);
            Draw.Rect(LeftPos + new Vector2(2, 2), insideSize.X, insideSize.Y, fillColor[1]);
            Draw.Rect(RightPos + new Vector2(2, 2), insideSize.X, insideSize.Y, fillColor[0]);
            Draw.Rect(JumpPos + new Vector2(2, 2), insideSize.X, insideSize.Y, fillColor[5]);
            Draw.Rect(DashPos + new Vector2(2, 2), insideSize.X, insideSize.Y, fillColor[4]);
            Draw.Rect(ClimbPos + new Vector2(2, 2), insideSize.X, insideSize.Y, fillColor[6]);


        }
    }

    public enum LargeMeterPositions
    {
        TopLeft,
        TopCenter,
        TopRight,
        BottomRight,
        BottomCenter,
        BottomLeft
    }
}