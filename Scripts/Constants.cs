using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Constants
{
    public static string STORY_PATH = "Story/";
    public static string JSON_PATH = "StoryJson/";
    public static string DEFAULT_STORY_FILE_NAME = "Start";
    public static string EXCEL_FILE_EXTENSION = ".xlsx";
    public static int DEFAULT_START_LINE = 0;

    public static string AVATAR_PATH = "Art/image/avatar/";
    public static string CHARACTER_PATH = "Art/image/character/";
    public static string VOCAL_PATH = "audio/Vocal/";
    public static string AUDIO_LOAD_FAILED = "Failed to load audio: ";
    public static string IMAGE_LOAD_FAILED = "Failed to load image: ";

    public static string BACKGROUND_PATH = "Art/image/background/";
    public static string MUSIC_PATH = "audio/music/";
    public static string MUSIC_LOAD_FAILED = "Failed to load music: ";

    public static string BUTTON_PATH = "Art/button/";
    public static string AUTO_ON = "autoplayon";
    public static string AUTO_OFF = "autoplayoff";
    public static string SKIP_ON = "skipon";
    public static string SKIP_OFF = "skipoff";

    public static string NO_DATA_FOUND = "No data found";
    //public static string END_OF_STORY = "End of story";
    public static string CHOICE = "choice";
    public static string CONTINUE = "continue";
    public static string DAY_END = "end day";
    public static string END = "end";
    public static float DEFAULT_WAITING_SECONDS = 0.05f;
    public static float AUTO_MODE_WAITING_SECONDS = 0.5f;
    public static float SKIP_MODE_WAITING_SECONDS = 0.001f;

    public static string characterActionAppearAt = "appearAt";
    public static int DURATION_TIME = 1;
    public static string characteraActionDisappear = "disappear";
    public static string characyerActionMoveTo = "moveTo";
    public static string COORDINATE_MISSING = "Coordinate missing";

    public static string backgroundShake = "shake";
    public static string backgroundZoom = "zoom";
    public static string backgroundResetZoom = "resetzoom";
    public static string backgroundFadeOut = "fadeout";
    public static string backgroundFadeIn = "fadein";
    public static string backgroundFlash = "flash";
    public static float shakeStrength = 20f;
    public static float shakeDuration = 0.5f;
    public static float slowShakeDuration = 3f;
    public static int slowShakeTimes = 4;
    public static int shakeTimes = 3;
    public static float zoomAmount = 1.3f;
    public static float zoomDuration = 0.4f;
    public static float flashDuration = 0.5f;
    public static float fadeDuration = 0.5f;

    public static int DEFAULT_START_INDEX = 0;
    public static int SLOTS_PER_PAGE = 7;
    public static int TOTAL_SLOTS = 7;
    public static string COLON = ": ";
    public static string SAVE_GAME = "save_game";
    public static string LOAD_GAME = "load_game";
    public static string EMPTY_SOLT = "empty_slot";

    public static int MAX_LENGTH = 50;

    public static string DAY_1 = "Start";
    public static string DAY_2 = "Day2";
    public static string DAY_3 = "Day3";
    public static string DAY_4 = "Day4";
    public static string DAY_5 = "Day5";
    public static string DAY_6 = "1";
    public static string DAY_7 = "1";
}