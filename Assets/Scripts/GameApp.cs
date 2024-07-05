using GameSystems;
using Interfaces;
using Models;
using QFramework;
using Utilities;

public class GameApp:Architecture<GameApp>
{
    protected override void Init()
    {
        this.RegisterModel<IGameModel>(new GameModel());
        this.RegisterSystem<IGameSystem>(new GameSystem());
        this.RegisterUtility<IGameStorage>(new GameStorage());
    }
}