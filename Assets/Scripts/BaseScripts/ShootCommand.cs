using UnityEngine;

namespace BaseScripts
{
    public class ShootCommand : CommandBase
    {
        private Grid _grid;
        private CellBase[,] _cellArray;

        public ShootCommand(Grid grid)
        {
            _grid = grid;
        }

        public override void Init()
        {
        }

        public override void Execute()
        {
            foreach (var type in _grid.GetGridArray())
            {
                if (type == (int)CONSTANTS.CellType.Obstacle)
                {
                    Debug.Log("Obstacle");
                }
            }

            _cellArray = _grid.GetCellArray();
            Debug.Log(_cellArray.Length);
            for (int x = 0; x < _cellArray.GetLength(0); x++)
            {
                for (int y = 0; y < _cellArray.GetLength(1); y++)
                {
                    // Debug.Log($"_cellArray {_cellArray[x, y]}");
                    if (_cellArray[x, y].Type == CONSTANTS.CellType.Obstacle)
                    {
                        Debug.Log("Obstacle type");
                    }
                }
            }
        }

        public override void Undo()
        {
        }
    }
}