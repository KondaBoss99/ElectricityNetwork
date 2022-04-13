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

        #region BFS Algorithm

        public List<BlockModel> BFSCreateLine(double x1, double y1, double x2, double y2, bool cross)
        {
            List<int> gridIndexes = GetGridIndexes(x1, y1, x2, y2);
            int approximateX1 = gridIndexes[0];
            int approximateY1 = gridIndexes[1];
            int approximateX2 = gridIndexes[2];
            int approximateY2 = gridIndexes[3];

            BlockModel b1 = new BlockModel()
            {
                X = x1,
                Y = y1,
                ApproximateX = approximateX1,
                ApproximateY = approximateY1
            };

            BlockModel b2 = new BlockModel()
            {
                X = x2,
                Y = y2,
                ApproximateX = approximateX2,
                ApproximateY = approximateY2
            };

            BlockModel[,] visited = new BlockModel[BlockMatrix.GetLength(0), BlockMatrix.GetLength(1)];
            visited[approximateX1, approximateY1] = b1;

            Queue<BlockModel> searchChain = new Queue<BlockModel>();
            searchChain.Enqueue(b1);

            bool found = false;
            while (searchChain.Count > 0)
            {
                BlockModel tBlock = searchChain.Dequeue();
                if (found = (tBlock.ApproximateX == approximateX2 && tBlock.ApproximateY == approximateY2))
                    break;

                IterateBlock(b1, b2, tBlock, visited, searchChain, cross);
            }

            return GetOptimalPath(b1, b2, visited, found);
        }

        #region BFS_Helpers

        private List<int> GetGridIndexes(double x1, double y1, double x2, double y2)
        {
            int x1Index = -1, y1Index = -1, x2Index = -1, y2Index = -1;

            for (int i = 0; i < BlockMatrix.GetLength(0); i++)
                if (x1 == BlockMatrix[i, 0].X)
                    x1Index = i;

            for (int j = 0; j < BlockMatrix.GetLength(1); j++)
                if (y1 == BlockMatrix[0, j].Y)
                    y1Index = j;

            for (int i = 0; i < BlockMatrix.GetLength(0); i++)
                if (x2 == BlockMatrix[i, 0].X)
                    x2Index = i;

            for (int j = 0; j < BlockMatrix.GetLength(1); j++)
                if (y2 == BlockMatrix[0, j].Y)
                    y2Index = j;

            return new List<int>() { x1Index, y1Index, x2Index, y2Index };
        }
        private void IterateBlock(BlockModel b1, BlockModel b2, BlockModel tBlock, BlockModel[,] visited, Queue<BlockModel> searchChain, bool cross)
        {
            for (int i = 0; i < 4; i++)
            {
                if (!CheckLines(b1, b2, tBlock, visited, i, cross, out int nextRow, out int nextColumn))
                    continue;

                searchChain.Enqueue(new BlockModel()
                {
                    X = BlockMatrix[nextRow, nextColumn].X,
                    Y = BlockMatrix[nextRow, nextColumn].Y,
                    ApproximateX = nextRow,
                    ApproximateY = nextColumn
                });

                visited[nextRow, nextColumn] = tBlock;
            }
        }
        private bool CheckLines(BlockModel b1, BlockModel b2, BlockModel tBlock, BlockModel[,] visited, int iterator, bool cross, out int nextRow, out int nextColumn)
        {
            List<int> dr = new List<int> { -1, 1, 0, 0 };
            List<int> dc = new List<int> { 0, 0, 1, -1 };

            nextRow = tBlock.ApproximateX + dr[iterator];
            nextColumn = tBlock.ApproximateY + dc[iterator];

            if ((nextRow < 0 || nextColumn < 0 || nextRow >= BlockMatrix.GetLength(0) || nextColumn >= BlockMatrix.GetLength(1))
                    || (visited[nextRow, nextColumn] != null)
                    || (!(nextRow == b2.ApproximateX && nextColumn == b2.ApproximateY) && (BlockMatrix[nextRow, nextColumn].BlockType != EBlockType.EMPTY) && cross == false)
                    || (!(nextRow == b2.ApproximateX && nextColumn == b2.ApproximateY) && (BlockMatrix[nextRow, nextColumn].BlockType == EBlockType.NODE) && cross == true)
                    || (!(nextRow == b2.ApproximateX && nextColumn == b2.ApproximateY) && (BlockMatrix[nextRow, nextColumn].BlockType == EBlockType.SWITCH) && cross == true)
                    || (!(nextRow == b2.ApproximateX && nextColumn == b2.ApproximateY) && (BlockMatrix[nextRow, nextColumn].BlockType == EBlockType.SUBSTATION) && cross == true))
                return false;

            return true;
        }
        private List<BlockModel> GetOptimalPath(BlockModel b1, BlockModel b2, BlockModel[,] visited, bool found)
        {
            List<BlockModel> optimalPath = new List<BlockModel>();

            if (found)
            {
                optimalPath.Add(b2);
                BlockModel previousBlock = visited[b2.ApproximateX, b2.ApproximateY];
                while (previousBlock.ApproximateX > 0 && !(previousBlock.X == b1.X && previousBlock.Y == b1.Y && previousBlock.ApproximateX == b1.ApproximateX && previousBlock.ApproximateY == b1.ApproximateY))
                {
                    if (BlockMatrix[previousBlock.ApproximateX, previousBlock.ApproximateY].BlockType == EBlockType.EMPTY)
                        BlockMatrix[previousBlock.ApproximateX, previousBlock.ApproximateY].BlockType = EBlockType.LINE;

                    optimalPath.Add(previousBlock);
                    previousBlock = visited[previousBlock.ApproximateX, previousBlock.ApproximateY];
                }
                optimalPath.Add(previousBlock);
            }

            return optimalPath;
        }

        #endregion

        #endregion
    }
}
