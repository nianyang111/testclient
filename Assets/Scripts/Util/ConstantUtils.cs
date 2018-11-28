using UnityEngine;
using System.Collections;

public class ConstantUtils
{
    public static string bundleTipsUrl = bundleDownLoadUrl + "Bundle.txt";
    public static string bundleDownLoadUrl
    {
        get
        {
            if (PageManager.Instance.isLocalVersion)
                if (Application.platform == RuntimePlatform.Android)
                    return "jar:file://" + Application.dataPath + "!/assets/AssetBundle/" + MiscUtils.GetCurrentPlatform() + "/";
                else
                    return "file://" + Application.streamingAssetsPath + "/AssetBundle/" + MiscUtils.GetCurrentPlatform() + "/";
            else
                return "http://192.168.10.101:4040/client/meimei/" + MiscUtils.GetCurrentPlatform() + "/";
        }
    }

    public static string ConfigFolderPath = Application.persistentDataPath + "/config/";
    public static string AssetBundleFolderPath = Application.persistentDataPath + "/AssetBundle/" + MiscUtils.GetCurrentPlatform() + "/";
    public static string SpriteFolderPath = Application.persistentDataPath + "/sprite/";

    //游戏json，bundle类型
    public static string fishConfigPath = "FishConfig";
    public static string fishArrayPath = "FishArray";
    public static string taskConfigPath = "DailyTaskConfig";//任务json 
    public static string levelConfigPath = "LevelConfig";
    public static string cityConfigPath = "CityConfig";
    public static string cityChineseConfigPath = "CityChineseConfig";
    public static string floatBallConfig = "FloatBallConfig";
    public static string masterScoreConfig = "MasterScoreConfig";
    public static string storeGoldConfig = "StoreGoldConfig";
    public static string storeAgConfig = "StoreAgConfig";
    public static string storeCardConfig = "StoreCardConfig";
    public static string storeVipConfig = "StoreVipConfig";
    public static string mjRoomGradeConfig = "MjRoomGradeConfig";
    public static string ddzRoomGradeConfig = "DdzRoomGradeConfig";
    public static string changyongyuConfig = "ChangyongyuConfig";
    public static string usConfig = "UsConfig";

    //系统json
    public static string urlVersionConfigPath = "http://file.xueyaokeji.com/version.txt";
    public static string versionConfigPath = ConfigFolderPath + "Version.json";
    public static string setConfigPath = ConfigFolderPath + "Set.json";
    public static string loginConfigPath = ConfigFolderPath + "Login.json";
    public static string tonkenConfigPath = ConfigFolderPath + "Token.json";
    public static string downConfigPath = ConfigFolderPath + "GameDown.json";
    public static string chatConfigPath = ConfigFolderPath + "ChatLog.json";

    public static int const0 = 0;
    public static int const1 = 1;
    public static int const90 = 90;
    public static int const180 = 180;
    public static int const360 = 360;

    public static Vector3 vecForward90 = Vector3.forward * const90;
    public static Vector3 vecForward180 = Vector3.forward * const180;
    public static Vector3 vecForward360 = Vector3.forward * const360;
    public static Vector3 vecUp90 = Vector3.up * const90;
    public static Vector3 vecUp180 = Vector3.up * const180;
    public static Vector3 vecUp360 = Vector3.up * const360;

    #region ...Tag
    #endregion

    #region ...Layer
    public const int mjLayer = 9;
    public const int modelLayer = 10;
    #endregion
}

public enum RoomType
{
    SilverCoin = 1,
    GoldBar,
    RoomCard,
    Match,
}

