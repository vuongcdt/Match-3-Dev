using QFramework;

namespace Interfaces
{
    public interface IStorage:IUtility
    {
        void SaveInt(string key, int value);
        int LoadInt(string key, int defaultValue = 0);
        
        void SaveFloat(string key, float value);
        int LoadFloat(string key, float defaultValue = 0);
        
        void SaveString(string key, string value);
        int LoadString(string key, string defaultValue = "");
    }
}