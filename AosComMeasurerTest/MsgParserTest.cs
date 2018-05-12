using System;
using AosComMeasurer;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AosComMeasurerTest
{
    [TestClass]
    public class MsgParserTest
    {
        //[TestMethod]
        public void MsgParserTestMethod()
        {
            BoardState boardState = new BoardState();
            MeasurerState measurerState = new MeasurerState();

            //ComMsgParser parser = new ComMsgParser(boardState, measurerState);

            string str1 = "m0 p0 s0 d35 b0 v0 i0000000000000000 ok";

            //parser.Parse(str1);
        }
    }
}
