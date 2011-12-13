using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace Serpent
{
    public class PlayingFieldBuilder
    {
        private readonly PlayingFieldSquare[,,] _field;
        private int _floor;

        public PlayingFieldBuilder( PlayingFieldSquare[,,] field)
        {
            _field = field;
        }

        public void ConstructOneFloor(int floor, string[] field)
        {
            _floor = floor;

            var height = field.Length;
            var width = field[0].Length;

            var expectedHeight = _field.GetUpperBound(1) + 1;
            var expectedWidth = _field.GetUpperBound(2) + 1;
            if (expectedHeight != height || expectedWidth != width)
                throw new Exception();

            while (true)
            {
                var mustRunAgain = false;
                for (var y = 0; y < height; y++)
                    for (var x = 0; x < width; x++)
                    {
                        if (!_field[floor, y, x].IsNone )
                            continue;
                        switch (field[y][x])
                        {
                            case ' ':
                                _field[floor, y, x] = new PlayingFieldSquare();
                                break;
                            case 'X':
                                _field[floor, y, x] = PlayingFieldSquare.CreateFlat(0);
                                break;
                            case 'D':
                                _field[floor, y, x] = createSlopeSquare(x, y, true);
                                mustRunAgain |= _field[floor, y, x].IsNone;
                                break;
                            case 'U':
                                _field[floor, y, x] = createSlopeSquare(x, y, false);
                                mustRunAgain |= _field[floor, y, x].IsNone;
                                break;
                        }
                    }
                if (!mustRunAgain)
                    return;
            }
        }

        private PlayingFieldSquare createSlopeSquare(int x, int y, bool doPortal)
        {
            foreach (var direction in Direction.AllDirections)
            {
                var square = getSquare(direction.DirectionAsPoint().Add(x,y));
                if ( square.IsNone )
                    continue;
                if ( doPortal )
                    return new PlayingFieldSquare(
                        PlayingFieldSquareType.Portal,
                        square.Elevation-1,
                        direction);
                return new PlayingFieldSquare(
                    PlayingFieldSquareType.Slope,
                    square.TopElevation,
                    direction.Backward );
            }
            return new PlayingFieldSquare();
        }

        private PlayingFieldSquare getSquare(Point p)
        {
            if (p.Y < 0 || p.Y > _field.GetUpperBound(1))
                return new PlayingFieldSquare();
            if (p.X < 0 || p.X > _field.GetUpperBound(2))
                return new PlayingFieldSquare();
            return _field[_floor, p.Y, p.X];
        }

    }
}
