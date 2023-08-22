using System.Collections;

namespace Celeste.Mod.CommunalHelper.Entities;

    [CustomEntity("CommunalHelper/ChainedPendulumBlock")]

    public class ChainedPendulumBlock : ChainedFallingBlock
    {
        private bool move, stickOnDash;
        private bool landed = false;
        private Vector2 origPosition;
        private float phase = 0f;
        private float rate, maxAngle, decayRate;
        private float dashScale = 1;
        private bool pause = false;

        public ChainedPendulumBlock(Vector2 position, int width, int height, char tileType, bool climbFall, bool behind, int maxFallDistance, bool centeredChain, bool chainOutline, bool indicator, bool indicatorAtStart, float rate, float maxAngle, float decayRate, bool stickOnDash)
            : base(position, width, height, tileType, climbFall, behind, maxFallDistance, centeredChain, chainOutline, indicator, indicatorAtStart)
        {
             move = false;
            OnDashCollide = OnDash;
            origPosition = new Vector2(X, Y);
            this.rate = Math.Abs(rate);
            this.stickOnDash = stickOnDash;
            this.maxAngle = Math.Min(Math.Abs(maxAngle / 90), 2);
            this.decayRate = Math.Abs(decayRate / 100);
        }

        public ChainedPendulumBlock(EntityData data, Vector2 offset)
        : this(data.Position + offset, data.Width, data.Height, data.Char("tiletype", '3'), data.Bool("climbFall", true), data.Bool("behind"), data.Int("fallDistance"), data.Bool("centeredChain"), data.Bool("chainOutline", true), data.Bool("indicator"), data.Bool("indicatorAtStart"), data.Float("frequency", 4f), data.Float("maxAngle"), data.Float("decayRate"), data.Bool("stickOnDash", false)) { }


        public override void Update()
        {
            base.Update();
            if (move)
            {

                if (!pause) phase += Engine.DeltaTime;
                double _p1 = Math.PI * rate * dashScale * maxAngle / 2 * Math.Pow(Math.E, -rate * phase * decayRate) * (decayRate * Math.Sin(rate * phase) - Math.Cos(rate * phase));
                double _p2 = Math.PI * dashScale * maxAngle / 2 * Math.Pow(Math.E, -rate * phase * decayRate) * Math.Sin(rate * phase);
                float moveH = (left ? 1 : -1) * (float) (_p1 * Math.Cos(_p2));
                float moveV = (float) (_p1 * Math.Sin(_p2));
                int _movingRight = moveH > 0 ? 1 : moveH == 0 ? 0 : -1;
                int _movingUp = moveV < 0 ? 1 : moveV == 0 ? 0 : -1;
                if (!pause)
                {
                MoveH(moveH);
                MoveV(moveV);
                if (Math.Pow(Math.E, -rate * phase * decayRate) < 0.15 && Math.Abs(X - origPosition.X) < 2)
                {
                    MoveTo(new Vector2(origPosition.X, chainStopY));
                    move = false;
                }
                }
            
                //Jackal - check for collisions
                
                bool hitDown = CollideCheck<Solid>(TopCenter + Vector2.UnitY) || CollideCheck<Platform>(TopCenter + Vector2.UnitY);
                bool hitLeft = CollideCheck<Solid>(CenterLeft - Vector2.UnitX) || CollideCheck<Platform>(CenterLeft - Vector2.UnitX);
                bool hitUp = CollideCheck<Solid>(TopCenter - Vector2.UnitY) || CollideCheck<Platform>(TopCenter - Vector2.UnitY);
                bool hitRight = CollideCheck<Solid>(CenterRight + Vector2.UnitX) || CollideCheck<Platform>(CenterRight + Vector2.UnitX);
            if (_movingUp == -1 && hitDown)
            {
                if (!pause)
                {
                    ImpactSfx();
                    base.LandParticles();
                }
                pause = true;
            }
            else
            {
                pause = false;
            }
            }
        }

        private bool left = false;
        //Jackal - new method, placeholder to set impulse
        private DashCollisionResults OnDash(Player player, Vector2 direction)
        {
            if (base.hasFallen && !move && !landed && direction.Y == 0)
            {
                move = true;
                phase = 0;
                left = direction.X < 0;
                dashScale = Math.Abs(player.Speed.X) / 240;

                if (!stickOnDash) return DashCollisionResults.Rebound;
            }
            return DashCollisionResults.NormalCollision;
        }
    }




