using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Serpent
{
    public static class PlayingFields
    {
        public static List<string[]> GetZ()
        {
            var list = new List<string[]>();
            list.Add(
                new[]
                    {
                        "XUUU                ",
                        "                    ",
                        "                    ",
                        "                    ",
                        "                    ",
                        "                    ",
                        "                    ",
                        "                    ",
                        "                    ",
                        "                    ",
                        "                    ",
                        "                    ",
                        "                    ",
                        "                    ",
                        "                    ",
                        "                    ",
                        "                    ",
                        "                    ",
                        "                    ",
                        "                UUUX"
                    });

            list.Add(
                new[]
                    {
                        "    DXXXXXXXXXXXXXXX",
                        "       X           X",
                        "XXXXXXXXXXXXXXXXXXXX",
                        "X X X  X           X",
                        "X X X  X           X",
                        "XXX XXXXUUU        X",
                        "X                  X",
                        "XUUU               X",
                        "X                  X",
                        "X                  X",
                        "XXXXXXXXXXXXXXXXXXXX",
                        "X               X  X",
                        "X               X  X",
                        "X               X  X",
                        "XXXXXXXXXXXXXX  X  X",
                        "X            XXXXXXX",
                        "X    U       X X    ",
                        "X    U       XXX    ",
                        "X    U        X     ",
                        "XXXXXXXXXXXXXXXD    "
                    });
            list.Add(
                new[]
                    {
                        "                    ",
                        "                    ",
                        "                    ",
                        "  XXXXXXXXXXXXXXXX  ",
                        "  X              X  ",
                        "  XXXX     DXXXXXX  ",
                        "     X           X  ",
                        "    DXXXXXXXXXXXXX  ",
                        "     X       X U    ",
                        "  XXXXXXXXXXXX U    ",
                        "  X X X X      U    ",
                        "  XXX XXX           ",
                        "   X   X            ",
                        "   XX XX            ",
                        "    XXX             ",
                        "     D              ",
                        "                    ",
                        "                    ",
                        "                    ",
                        "                    ",
                    });
            list.Add(
                new[]
                    {
                        "                    ",
                        "                    ",
                        "                    ",
                        "                    ",
                        "                    ",
                        "                    ",
                        "                    ",
                        "                    ",
                        "                    ",
                        "                    ",
                        "                    ",
                        "               D    ",
                        "XXXXXXXXXXXXXXXXXXXX",
                        "   X   X   X   X   X",
                        "X  X XXX XXX XXX XXX",
                        "XX X   X   X   X   X",
                        "XXXX XXX   X   X XXX",
                        "X XX   X   X   X   X",
                        "X  X XXX   X   X XXX",
                        "                    "
                    });

            return list;
        }

        public static List<string[]> GetQ()
        {
            var list = new List<string[]>();
            list.Add(
                new[]
                    {
"XXXXXXXXXXXXXXXXXXXXXXXXX",
"X X       X   X   X     X",
"X XXX XXX X XXXXX XXXXX X",
"X   X X X X X   X X   X X",
"X XXX X XXX XXX X XXX X X",
"X X   X   X   X X   X X X",
"XXX XXXXX XXXXX XXXXX XXX",
"X X X X X X     X       X",
"X X X X X XXXXXXXXXXXXX X",
"X X X X X X X X       X X",
"X XXX X XXX X XXXXXXXXXXX",
"X X X X X X X X       X X",
"X X X X X XXXXXXXXXXXXX X",
"X X X X X X     X       X",
"XXX XXXXX XXXXX XXXXX XXX",
"  X   X   X   X X   X X  ",
"X XXX X XXX XXX X XXX X X",
"X   X X X X X   X X   X X",
"X XXX XXX X XXXXX XXXXX X",
"X X       X   X   X     X",
"X XXXXXXXXXXXXXXXXX XXXXX",
                           
                    });
            return list;
        }

    }
}
