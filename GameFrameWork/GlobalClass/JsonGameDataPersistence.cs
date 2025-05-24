namespace GameFrameWork
{
    public class JsonGameDataPersistence : IGameDataPersistence
    {
        public void SaveGameData<Type>(Type gameData, string filename) where Type : GameData
        {
            gameData.SaveToFile(filename);
        }

        public Type LoadGameData<Type>(string filename) where Type : GameData
        {
            return GameData.LoadFromFile<Type>(filename);
        }

    }
}