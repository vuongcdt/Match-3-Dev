using Interfaces;
using QFramework;
using UnityEngine;

namespace Commands
{
    public class SetLevelAndUserDataCommand : AbstractCommand
    {
        private IGameModel _gameModel;

        protected override void OnExecute()
        {
            _gameModel = this.GetModel<IGameModel>();
            SetUserData();
            _gameModel.Level.Value++;
        }

        private void SetUserData()
        {
            var userDataValue = _gameModel.UserData.Value;
            // for (var index = 0; index < userDataValue.Count; index++)
            // {
            // }

            _gameModel.UserData.Value.Add(new Utils.LevelData(_gameModel.Level.Value, _gameModel.StarsTotal.Value));
            Debug.Log($"_gameModel.UserData.Value {_gameModel.UserData.Value.Count}");
        }
    }
}