using System;
using Microsoft.Xna.Framework;

namespace Serpent.Util
{
    public struct SinBump
    {
        private float _time;
        private readonly float _duration;

        public SinBump( float duration )
            : this(duration,0)
        {
        }

        public SinBump(float duration, float timeToStart)
        {
            _duration = duration;
            _time = -timeToStart;
        }

        public void Update(float elapsedTime)
        {
            _time += elapsedTime;
        }

        public float Value
        {
            get
            {
                if (_time < 0 || _time > _duration)
                    return 0;
                return (float) Math.Sin(_time/_duration*MathHelper.Pi);
            }    
        }

    }

}
