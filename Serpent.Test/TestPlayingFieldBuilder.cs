using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;

namespace Serpent.Test
{
    [TestFixture]
    public class TestPlayingFieldBuilder
    {
        [Test]
        public void TestSimplePlayingField()
        {
            var field = new PlayingFieldSquare[1,3,3];
            var builder = new PlayingFieldBuilder(field);
            builder.ConstructOneFloor(0, new[]
                                             {
                                                 "XXX",
                                                 "X X",
                                                 "XXX"
                                             }
                );
            Assert.IsTrue(field[0, 0, 0].IsFlat);
            Assert.IsTrue(field[0, 0, 1].IsFlat);
            Assert.IsTrue(field[0, 0, 2].IsFlat);
            Assert.IsTrue(field[0, 1, 0].IsFlat);
            Assert.IsTrue(field[0, 1, 1].IsNone);
            Assert.IsTrue(field[0, 1, 2].IsFlat);
            Assert.IsTrue(field[0, 2, 0].IsFlat);
            Assert.IsTrue(field[0, 2, 1].IsFlat);
            Assert.IsTrue(field[0, 2, 2].IsFlat);
        }

        [Test]
        public void TestPlayingField()
        {
            var field = new PlayingFieldSquare[1, 3, 7];
            var builder = new PlayingFieldBuilder(field);
            builder.ConstructOneFloor(0, new[]
                                             {
                                                 "XXXXXXX",
                                                 "X X X X",
                                                 "XXX XXX"
                                             }
                );
            Assert.IsTrue(field[0, 0, 0].IsFlat);
            Assert.IsTrue(field[0, 0, 1].IsFlat);
            Assert.IsTrue(field[0, 0, 2].IsFlat);
            Assert.IsTrue(field[0, 1, 0].IsFlat);
            Assert.IsTrue(field[0, 1, 1].IsNone);
            Assert.IsTrue(field[0, 1, 2].IsFlat);
            Assert.IsTrue(field[0, 2, 0].IsFlat);
            Assert.IsTrue(field[0, 2, 1].IsFlat);
            Assert.IsTrue(field[0, 2, 2].IsFlat);

            Assert.IsTrue(field[0, 2, 3].IsNone);
            Assert.IsTrue(field[0, 2, 6].IsFlat);
        }

        [Test]
        public void TestSlope1()
        {
            var field = new PlayingFieldSquare[1, 1, 3];
            var builder = new PlayingFieldBuilder(field);
            builder.ConstructOneFloor(0, new[]
                                             {
                                                 "XUU"
                                             }
                );
            Assert.IsTrue(field[0, 0, 0].IsFlat);
            Assert.AreEqual(field[0, 0, 1].Corners, new [] { 0, 0, 1, 1} );
            Assert.AreEqual(field[0, 0, 2].Corners, new[] { 1, 1, 2, 2 });
        }

        [Test]
        public void TestSlope2()
        {
            var field = new PlayingFieldSquare[1, 1, 3];
            var builder = new PlayingFieldBuilder(field);
            builder.ConstructOneFloor(0, new[]
                                             {
                                                 "UUX"
                                             }
                );
            Assert.IsTrue(field[0, 0, 2].IsFlat);
            Assert.AreEqual(field[0, 0, 0].Corners, new[] { 2, 2, 1, 1 });
            Assert.AreEqual(field[0, 0, 1].Corners, new[] { 1, 1, 0, 0 });
        }

        [Test]
        public void TestSlope3()
        {
            var field = new PlayingFieldSquare[2, 1, 6];
            var builder = new PlayingFieldBuilder(field);
            builder.ConstructOneFloor(0, new[]
                                             {
                                                 "XUUU  ",
                                             }
                );
            builder.ConstructOneFloor(1, new[]
                                             {
                                                 "    DX",
                                             }
                );
            Assert.IsTrue(field[1, 0, 4].IsPortal);
            Assert.AreEqual(field[1, 0, 4].Corners, new[] { -1, -1, 0, 0 });
        }

        [Test]
        public void TestSlope4()
        {
            var field = new PlayingFieldSquare[2, 6, 1];
            var builder = new PlayingFieldBuilder(field);
            builder.ConstructOneFloor(0, new[]
                                             {
                                                 " ",
                                                 " ",
                                                 "U",
                                                 "U",
                                                 "U",
                                                 "X",
                                             }
                );
            builder.ConstructOneFloor(1, new[]
                                             {
                                                 "X",
                                                 "D",
                                                 " ",
                                                 " ",
                                                 " ",
                                                 " ",
                                             }
                );
            Assert.IsTrue(field[1, 0, 4].IsPortal);
            Assert.AreEqual(field[1, 0, 4].Corners, new[] { -1, -1, 0, 0 });
        }


    }
}
