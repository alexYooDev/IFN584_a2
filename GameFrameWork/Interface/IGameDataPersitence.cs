namespace GameFrameWork
{
    /* Interface for Game data saving and loading */
    public interface IGameDataPersistence
    {
        void SaveGameData<Type>(Type gameData, string filename) where Type : GameData;
        Type LoadGameData<Type>(string filename) where Type : GameData;
    }
}