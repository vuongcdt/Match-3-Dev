using GameSystems;
using Interfaces;
using Models;
using QFramework;
using Utilities;

public class App:Architecture<App>
{
    protected override void Init()
    {
        this.RegisterModel<IModel>(new GameModel());
        this.RegisterSystem<IGameSystem>(new GameSystem());
        this.RegisterUtility<IStorage>(new Storage());
    }
}