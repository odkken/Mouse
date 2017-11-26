using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Interfaces
{
    public interface IBoard
    {
        List<IBlock> GetBlocksOnSameRow(IBlock asBlock);
        List<IBlock> GetBlocksOnSameColumn(IBlock asBlock);
        List<IBlock> GetLastActivatedBlock(IBlock block);
    }
}
