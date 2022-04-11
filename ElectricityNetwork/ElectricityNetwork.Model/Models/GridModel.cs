using ElectricityNetwork.Model.Models.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElectricityNetwork.Model.Models
{
    public class GridModel
    {
        public BlockModel[,] BlockMatrix { get; set; }

        public GridModel(int xBlocks, int yBlocks) { BlockMatrix = new BlockModel[xBlocks, yBlocks]; }

        public void Add(double x, double y, EBlockType blockType)
        {
            for (int i = 0; i < BlockMatrix.GetLength(0); i++)
                if (x == BlockMatrix[i, 0].X)
                    for (int j = 0; j < BlockMatrix.GetLength(1); j++)
                        if (y == BlockMatrix[i, j].Y)
                            BlockMatrix[i, j].BlockType = blockType;
        }

        public void AddLine(double x, double y, EBlockType blockType)
        {
            for (int i = 0; i < BlockMatrix.GetLength(0); i++)
                if (x == BlockMatrix[i, 0].X)
                    for (int j = 0; j < BlockMatrix.GetLength(1); j++)
                        if (y == BlockMatrix[i, j].Y)
                        {
                            if (BlockMatrix[i, j].BlockType == EBlockType.NODE || BlockMatrix[i, j].BlockType == EBlockType.SUBSTATION || BlockMatrix[i, j].BlockType == EBlockType.SWITCH)
                                return;

                            if (BlockMatrix[i, j].BlockType == EBlockType.LINE || BlockMatrix[i, j].BlockType == EBlockType.EMPTY)
                                BlockMatrix[i, j].BlockType = blockType;
                            else if (BlockMatrix[i, j].BlockType != blockType)
                                BlockMatrix[i, j].BlockType = EBlockType.CROSS_LINE;
                        }
        }
    }
}
