using BaseScripts;
using Commands;
using QFramework;
using UnityEngine;
using Grid = BaseScripts.Grid;

namespace GameControllers
{
    public class GameManager : MonoBehaviour, IController
    {
        [SerializeField] private CellBase cell;
        [SerializeField] private Grid grid;
        [SerializeField] private Sprite backgroundSprite;
        [SerializeField] private Sprite obstacleSprite;
        [SerializeField] private Transform backgroundBlock;
        [SerializeField] private Vector2[] obstacles;

        private void Start()
        {
            this.SendCommand(new InitGridCommand());


            // var background = new BaseScripts.Grid(10, 8, 1.2f, cell, backgroundBlock, backgroundSprite, 0.9f);
            // grid = new BaseScripts.Grid(10, 8, 1.2f, cell, this.transform, 0.6f);
            //
            // foreach (var obstacle in obstacles)
            // {
            //     grid.SetValue((int)obstacle.x, (int)obstacle.y, obstacleSprite,CONSTANTS.CellType.Obstacle);
            // }
            //
            // var shootCommand = new ShootCommand(grid);
            // shootCommand.Execute();
        }

        public IArchitecture GetArchitecture()
        {
            return App.Interface;
        }
    }
}