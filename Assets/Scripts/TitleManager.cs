
public class TitleManager : SingletonMonoBehaviour<TitleManager>
{

    public enum Level
    {
        level1 = 1,
        level2 = 2,
        level3 = 3,
        level4 = 4,
        other = 99,
    }

    public static Level _CPULevel;
    public static CharaSwitch.Character _chara_1P;
    public static CharaSwitch.Character _chara_2P;
    public static CPUManager.CPULevel _level;

    private void Start()
    {

        DontDestroyOnLoad(this);
    }



}
