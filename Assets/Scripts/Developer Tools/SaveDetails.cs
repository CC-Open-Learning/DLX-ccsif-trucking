namespace VARLab.CCSIF
{
    //This class is for saving the specific configuration for DevTools.
    //It being static allows it to persist between reloads
    static public class SaveDetails
    {
        private static WeatherType weather; //the weather that will be applied when the scene reloads
        public static WeatherType Weather { set { weather = value; } get { return weather; } }
        private static ModuleType module;//the module that will be applied when the scene reloads
        public static ModuleType Module { set { module = value; } get { return module; } }
    }
}
