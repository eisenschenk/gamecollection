using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VnodeTest.Solitaire.GameEntities
{
    public class Foundations
    {
        public FoundationStack Club { get; } = new FoundationStack(PipValue.Club);
        public FoundationStack Spade { get; } = new FoundationStack(PipValue.Spade);
        public FoundationStack Heart { get; } = new FoundationStack(PipValue.Heart);
        public FoundationStack Diamond { get; } = new FoundationStack(PipValue.Diamond);
    }
}
