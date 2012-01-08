using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Threading;
using TerrariaServer.Hooks;

namespace Terraria
{
    public class Main
    {
        private const int MF_BYPOSITION = 1024;
        public const int sectionWidth = 200;
        public const int sectionHeight = 150;
        public const int maxTileSets = 150;
        public const int maxWallTypes = 32;
        public const int maxBackgrounds = 32;
        public const int maxDust = 2000;
        public const int maxCombatText = 100;
        public const int maxItemText = 20;
        public const int maxPlayers = 255;
        public const int maxChests = 1000;
        public const int maxItemTypes = 603;
        public const int maxItems = 200;
        public const int maxBuffs = 40;
        public const int maxProjectileTypes = 111;
        public const int maxProjectiles = 1000;
        public const int maxNPCTypes = 147;
        public const int maxNPCs = 200;
        public const int maxGoreTypes = 160;
        public const int maxGore = 200;
        public const int maxInventory = 48;
        public const int maxItemSounds = 37;
        public const int maxNPCHitSounds = 11;
        public const int maxNPCKilledSounds = 15;
        public const int maxLiquidTypes = 2;
        public const int maxMusic = 14;
        public const int numArmorHead = 45;
        public const int numArmorBody = 26;
        public const int numArmorLegs = 25;
        public const double dayLength = 54000.0;
        public const double nightLength = 32400.0;
        public const int maxStars = 130;
        public const int maxStarTypes = 5;
        public const int maxClouds = 100;
        public const int maxCloudTypes = 4;
        public const int maxHair = 36;
        public static int curRelease = 37;
        public static string versionNumber = "v1.1.1";
        public static string versionNumber2 = "v1.1.1";
        public static bool skipMenu;
        public static bool verboseNetplay;
        public static bool stopTimeOuts;
        public static bool showSpam;
        public static bool showItemOwner;
        public static int oldTempLightCount;
        public static int musicBox = -1;
        public static int musicBox2 = -1;
        public static float upTimer;
        public static float upTimerMax;
        public static float upTimerMaxDelay;
        public static float[] drawTimer = new float[10];
        public static float[] drawTimerMax = new float[10];
        public static float[] drawTimerMaxDelay = new float[10];
        public static float[] renderTimer = new float[10];
        public static float[] lightTimer = new float[10];
        public static bool drawDiag;
        public static bool drawRelease;
        public static bool renderNow;
        public static bool drawToScreen;
        public static bool targetSet;
        public static int mouseX;
        public static int mouseY;
        public static bool mouseLeft;
        public static bool mouseRight;
        public static float essScale = 1f;
        public static int essDir = -1;
        public static string debugWords = "";
        public static bool gamePad;
        public static bool xMas;
        public static int snowDust;
        public static bool netDiag;
        public static int txData;
        public static int rxData;
        public static int txMsg;
        public static int rxMsg;
        public static int maxMsg = 61;
        public static int[] rxMsgType = new int[maxMsg];
        public static int[] rxDataType = new int[maxMsg];
        public static int[] txMsgType = new int[maxMsg];
        public static int[] txDataType = new int[maxMsg];
        public static float uCarry;
        public static bool drawSkip;
        public static int fpsCount;
        public static Stopwatch fpsTimer = new Stopwatch();
        public static Stopwatch updateTimer = new Stopwatch();
        public static bool showSplash = true;
        public static bool ignoreErrors = true;
        public static string defaultIP = "";
        public static int dayRate = 1;
        public static int maxScreenW = 1920;
        public static int minScreenW = 800;
        public static int maxScreenH = 1200;
        public static int minScreenH = 600;
        public static float iS = 1f;
        public static bool render;
        public static int qaStyle;
        public static int zoneX = 99;
        public static int zoneY = 87;
        public static float harpNote;
        public static bool[] debuff = new bool[40];
        public static string[] buffName = new string[40];
        public static string[] buffTip = new string[40];
        public static int maxMP = 10;
        public static string[] recentWorld = new string[maxMP];
        public static string[] recentIP = new string[maxMP];
        public static int[] recentPort = new int[maxMP];
        public static bool shortRender = true;
        public static bool owBack = true;
        public static int quickBG = 2;
        public static int bgDelay;
        public static int bgStyle;
        public static float[] bgAlpha = new float[10];
        public static float[] bgAlpha2 = new float[10];
        public static int wof = -1;
        public static int wofT;
        public static int wofB;
        public static int wofF;
        private static int offScreenRange = 200;
        public static int renderCount = 99;
        private static Stopwatch saveTime = new Stopwatch();
        public static Color mcColor = new Color(125, 125, 255);
        public static Color hcColor = new Color(200, 125, 255);
        public static Color bgColor;
        public static bool mouseHC;
        public static string chestText = "Chest";
        public static bool craftingHide;
        public static bool armorHide;
        public static float craftingAlpha = 1f;
        public static float armorAlpha = 1f;
        public static float[] buffAlpha = new float[40];
        public static Item trashItem = new Item();
        public static bool hardMode;
        public static bool drawScene;
        public static Vector2 sceneWaterPos = default(Vector2);
        public static Vector2 sceneTilePos = default(Vector2);
        public static Vector2 sceneTile2Pos = default(Vector2);
        public static Vector2 sceneWallPos = default(Vector2);
        public static Vector2 sceneBackgroundPos = default(Vector2);
        public static bool maxQ = true;
        public static float gfxQuality = 1f;
        public static float gfxRate = 0.01f;
        public static int DiscoR = 255;
        public static int DiscoB;
        public static int DiscoG;
        public static int teamCooldown;
        public static int teamCooldownLen = 300;
        public static bool gamePaused;
        public static int updateTime;
        public static int drawTime;
        public static int uCount;
        public static int updateRate;
        public static int frameRate;
        public static bool RGBRelease;
        public static bool qRelease;
        public static bool netRelease;
        public static bool frameRelease;
        public static bool showFrameRate;
        public static int magmaBGFrame;
        public static int magmaBGFrameCounter;
        public static int saveTimer;
        public static bool autoJoin;
        public static bool serverStarting;
        public static float leftWorld;
        public static float rightWorld = 134400f;
        public static float topWorld;
        public static float bottomWorld = 38400f;
        public static int maxTilesX = (int) rightWorld/16 + 1;
        public static int maxTilesY = (int) bottomWorld/16 + 1;
        public static int maxSectionsX = maxTilesX/200;
        public static int maxSectionsY = maxTilesY/150;
        public static int numDust = 2000;
        public static int maxNetPlayers = 255;
        public static string[] chrName = new string[147];
        public static int worldRate = 1;
        public static float caveParrallax = 1f;
        public static string[] tileName = new string[150];
        public static int dungeonX;
        public static int dungeonY;
        public static Liquid[] liquid = new Liquid[Liquid.resLiquid];
        public static LiquidBuffer[] liquidBuffer = new LiquidBuffer[10000];
        public static bool dedServ;
        public static int spamCount;
        public static int curMusic;
        public static bool showItemText = true;
        public static bool autoSave = true;
        public static string buffString = "";
        public static string libPath = "";
        public static int lo;
        public static int LogoA = 255;
        public static int LogoB;
        public static bool LogoT;
        public static string statusText = "";
        public static string worldName = "";
        public static int worldID;
        public static int background;
        public static Color tileColor;
        public static double worldSurface;
        public static double rockLayer;
        public static Color[] teamColor = new Color[5];
        public static bool dayTime = true;
        public static double time = 13500.0;
        public static int moonPhase;
        public static short sunModY;
        public static short moonModY;
        public static bool grabSky;
        public static bool bloodMoon;
        public static int checkForSpawns;
        public static int helpText;
        public static bool autoGen;
        public static bool autoPause;
        public static int[] projFrames = new int[111];
        public static float demonTorch = 1f;
        public static int demonTorchDir = 1;
        public static int numStars;
        public static int cloudLimit = 100;
        public static int numClouds = cloudLimit;
        public static float windSpeed;
        public static float windSpeedSpeed;
        public static Cloud[] cloud = new Cloud[100];
        public static bool resetClouds = true;
        public static int sandTiles;
        public static int evilTiles;
        public static int snowTiles;
        public static int holyTiles;
        public static int meteorTiles;
        public static int jungleTiles;
        public static int dungeonTiles;
        public static int fadeCounter;
        public static float invAlpha = 1f;
        public static float invDir = 1f;
        [ThreadStatic] public static Random rand;
        public static float[] musicFade = new float[14];
        public static float musicVolume = 0.75f;
        public static float soundVolume = 1f;
        public static bool[] tileLighted = new bool[150];
        public static bool[] tileMergeDirt = new bool[150];
        public static bool[] tileCut = new bool[150];
        public static bool[] tileAlch = new bool[150];
        public static int[] tileShine = new int[150];
        public static bool[] tileShine2 = new bool[150];
        public static bool[] wallHouse = new bool[32];
        public static int[] wallBlend = new int[32];
        public static bool[] tileStone = new bool[150];
        public static bool[] tilePick = new bool[150];
        public static bool[] tileAxe = new bool[150];
        public static bool[] tileHammer = new bool[150];
        public static bool[] tileWaterDeath = new bool[150];
        public static bool[] tileLavaDeath = new bool[150];
        public static bool[] tileTable = new bool[150];
        public static bool[] tileBlockLight = new bool[150];
        public static bool[] tileNoSunLight = new bool[150];
        public static bool[] tileDungeon = new bool[150];
        public static bool[] tileSolidTop = new bool[150];
        public static bool[] tileSolid = new bool[150];
        public static bool[] tileNoAttach = new bool[150];
        public static bool[] tileNoFail = new bool[150];
        public static bool[] tileFrameImportant = new bool[150];
        public static int[] backgroundWidth = new int[32];
        public static int[] backgroundHeight = new int[32];
        public static bool tilesLoaded;
        public static TileCollection tile = new TileCollection();
        public static Dust[] dust = new Dust[2001];
        public static Star[] star = new Star[130];
        public static Item[] item = new Item[201];
        public static NPC[] npc = new NPC[201];
        public static Gore[] gore = new Gore[201];
        public static Projectile[] projectile = new Projectile[1001];
        public static CombatText[] combatText = new CombatText[100];
        public static ItemText[] itemText = new ItemText[20];
        public static Chest[] chest = new Chest[1000];
        public static Sign[] sign = new Sign[1000];
        public static Vector2 screenPosition;
        public static Vector2 screenLastPosition;
        public static int screenWidth = 800;
        public static int screenHeight = 600;
        public static int chatLength = 600;
        public static bool chatMode;
        public static bool chatRelease;
        public static int numChatLines = 7;
        public static string chatText = "";
        public static ChatLine[] chatLine = new ChatLine[numChatLines];
        public static bool inputTextEnter;

        public static float[] hotbarScale = new[]
                                                {
                                                    1f,
                                                    0.75f,
                                                    0.75f,
                                                    0.75f,
                                                    0.75f,
                                                    0.75f,
                                                    0.75f,
                                                    0.75f,
                                                    0.75f,
                                                    0.75f
                                                };

        public static byte mouseTextColor;
        public static int mouseTextColorChange = 1;
        public static bool mouseLeftRelease;
        public static bool mouseRightRelease;
        public static bool playerInventory;
        public static int stackSplit;
        public static int stackCounter;
        public static int stackDelay = 7;
        public static Item mouseItem = new Item();
        public static Item guideItem = new Item();
        public static Item reforgeItem = new Item();
        private static float inventoryScale = 0.75f;
        public static bool hasFocus = true;
        public static Recipe[] recipe = new Recipe[Recipe.maxRecipes];
        public static int[] availableRecipe = new int[Recipe.maxRecipes];
        public static float[] availableRecipeY = new float[Recipe.maxRecipes];
        public static int numAvailableRecipes;
        public static int focusRecipe;
        public static int myPlayer;
        public static Player[] player = new Player[256];
        public static int spawnTileX;
        public static int spawnTileY;
        public static bool npcChatRelease;
        public static bool editSign;
        public static string signText = "";
        public static string npcChatText = "";
        public static bool npcChatFocus1;
        public static bool npcChatFocus2;
        public static bool npcChatFocus3;
        public static int npcShop;
        public static bool craftGuide;
        public static bool reforge;
        private static Item toolTip = new Item();
        private static int backSpaceCount;
        public static string motd = "";
        public static bool gameMenu = true;
        public static Player[] loadPlayer = new Player[5];
        public static string[] loadPlayerPath = new string[5];
        private static int numLoadPlayers;
        public static string playerPathName;
        public static string[] loadWorld = new string[999];
        public static string[] loadWorldPath = new string[999];
        private static int numLoadWorlds;
        public static string worldPathName;
        public static string SavePath;
        public static string WorldPath;
        public static string PlayerPath;
        public static string[] itemName = new string[603];
        public static string[] npcName = new string[147];
        public static int invasionType;
        public static double invasionX;
        public static int invasionSize;
        public static int invasionDelay;
        public static int invasionWarn;

        public static int[] npcFrameCount = new[]
                                                {
                                                    1,
                                                    2,
                                                    2,
                                                    3,
                                                    6,
                                                    2,
                                                    2,
                                                    1,
                                                    1,
                                                    1,
                                                    1,
                                                    1,
                                                    1,
                                                    1,
                                                    1,
                                                    1,
                                                    2,
                                                    16,
                                                    14,
                                                    16,
                                                    14,
                                                    15,
                                                    16,
                                                    2,
                                                    10,
                                                    1,
                                                    16,
                                                    16,
                                                    16,
                                                    3,
                                                    1,
                                                    15,
                                                    3,
                                                    1,
                                                    3,
                                                    1,
                                                    1,
                                                    16,
                                                    16,
                                                    1,
                                                    1,
                                                    1,
                                                    3,
                                                    3,
                                                    15,
                                                    3,
                                                    7,
                                                    7,
                                                    4,
                                                    5,
                                                    5,
                                                    5,
                                                    3,
                                                    3,
                                                    16,
                                                    6,
                                                    3,
                                                    6,
                                                    6,
                                                    2,
                                                    5,
                                                    3,
                                                    2,
                                                    7,
                                                    7,
                                                    4,
                                                    2,
                                                    8,
                                                    1,
                                                    5,
                                                    1,
                                                    2,
                                                    4,
                                                    16,
                                                    5,
                                                    4,
                                                    4,
                                                    15,
                                                    15,
                                                    15,
                                                    15,
                                                    2,
                                                    4,
                                                    6,
                                                    6,
                                                    18,
                                                    16,
                                                    1,
                                                    1,
                                                    1,
                                                    1,
                                                    1,
                                                    1,
                                                    4,
                                                    3,
                                                    1,
                                                    1,
                                                    1,
                                                    1,
                                                    1,
                                                    1,
                                                    5,
                                                    6,
                                                    7,
                                                    16,
                                                    1,
                                                    1,
                                                    16,
                                                    16,
                                                    12,
                                                    20,
                                                    21,
                                                    1,
                                                    2,
                                                    2,
                                                    3,
                                                    6,
                                                    1,
                                                    1,
                                                    1,
                                                    15,
                                                    4,
                                                    11,
                                                    1,
                                                    14,
                                                    6,
                                                    6,
                                                    3,
                                                    1,
                                                    2,
                                                    2,
                                                    1,
                                                    3,
                                                    4,
                                                    1,
                                                    2,
                                                    1,
                                                    4,
                                                    2,
                                                    1,
                                                    15,
                                                    3,
                                                    16,
                                                    4,
                                                    5,
                                                    7,
                                                    3
                                                };

        private static bool mouseExit;
        private static float exitScale = 0.8f;
        private static bool mouseReforge;
        private static float reforgeScale = 0.8f;
        public static Player clientPlayer = new Player();
        public static string getIP = defaultIP;
        public static string getPort = Convert.ToString(Netplay.serverPort);
        public static bool menuMultiplayer;
        public static bool menuServer;
        public static int netMode;
        public static int timeOut = 120;
        public static int netPlayCounter;
        public static int lastNPCUpdate;
        public static int lastItemUpdate;
        public static int maxNPCUpdates = 5;
        public static int maxItemUpdates = 5;
        public static string cUp = "W";
        public static string cLeft = "A";
        public static string cDown = "S";
        public static string cRight = "D";
        public static string cJump = "Space";
        public static string cThrowItem = "T";
        public static string cInv = "Escape";
        public static string cHeal = "H";
        public static string cMana = "M";
        public static string cBuff = "B";
        public static string cHook = "E";
        public static string cTorch = "LeftShift";
        public static Color mouseColor = new Color(255, 50, 95);
        public static Color cursorColor = Color.White;
        public static int cursorColorDirection = 1;
        public static float cursorAlpha;
        public static float cursorScale;
        public static bool signBubble;
        public static int signX;
        public static int signY;
        public static bool hideUI;
        public static bool releaseUI;
        public static bool fixedTiming;
        public static string oldStatusText = "";
        public static bool autoShutdown;
        private static int maxMenuItems = 14;
        public static int menuMode;
        private static Item cpItem = new Item();
        public static string newWorldName = "";
        private static int accSlotCount;
        public static bool autoPass;
        public static int menuFocus;
        public static bool runningMono;
        public int DiscoStyle;
        private int bgLoops;
        private int bgLoopsY;
        private double bgParrallax;
        private int bgScroll;
        private int bgStart;
        private int bgStartY;
        private int bgTop;
        public bool chestDepositHover;
        public float chestDepositScale = 1f;
        public bool chestLootHover;
        public float chestLootScale = 1f;
        public bool chestStackHover;
        public float chestStackScale = 1f;
        private int colorDelay;
        private int[] displayHeight = new int[99];
        private int[] displayWidth = new int[99];
        private int firstTileX;
        private int firstTileY;
        private int focusColor;
        private int focusMenu = -1;
        public bool gammaTest;
        private int lastTileX;
        private int lastTileY;
        private float logoRotation;
        private float logoRotationDirection = 1f;
        private float logoRotationSpeed = 1f;
        private float logoScale = 1f;
        private float logoScaleDirection = 1f;
        private float logoScaleSpeed = 1f;
        private float[] menuItemScale = new float[maxMenuItems];
        public int mouseNPC = -1;
        public int newMusic;
        private int numDisplayModes;
        private Color selColor = Color.White;
        private int selectedMenu = -1;
        private int selectedMenu2 = -1;
        private int selectedPlayer;
        private int selectedWorld;
        private int setKey = -1;
        public Chest[] shop = new Chest[10];
        public bool showNPCs;
        private int splashCounter;
        private Process tServer = new Process();
        private int textBlinkerCount;
        private int textBlinkerState;
        public bool toggleFullscreen;

        public static void LoadWorlds()
        {
            Directory.CreateDirectory(WorldPath);
            string[] files = Directory.GetFiles(WorldPath, "*.wld");
            int num = files.Length;
            if (!dedServ && num > 5)
            {
                num = 5;
            }
            for (int i = 0; i < num; i++)
            {
                loadWorldPath[i] = files[i];
                try
                {
                    using (var fileStream = new FileStream(loadWorldPath[i], FileMode.Open))
                    {
                        using (var binaryReader = new BinaryReader(fileStream))
                        {
                            binaryReader.ReadInt32();
                            loadWorld[i] = binaryReader.ReadString();
                            binaryReader.Close();
                        }
                    }
                }
                catch
                {
                    loadWorld[i] = loadWorldPath[i];
                }
            }
            numLoadWorlds = num;
        }

        private static void LoadPlayers()
        {
            Directory.CreateDirectory(PlayerPath);
            string[] files = Directory.GetFiles(PlayerPath, "*.plr");
            int num = files.Length;
            if (num > 5)
            {
                num = 5;
            }
            for (int i = 0; i < 5; i++)
            {
                loadPlayer[i] = new Player();
                if (i < num)
                {
                    loadPlayerPath[i] = files[i];
                    loadPlayer[i] = Player.LoadPlayer(loadPlayerPath[i]);
                }
            }
            numLoadPlayers = num;
        }

        private static void ErasePlayer(int i)
        {
            try
            {
                File.Delete(loadPlayerPath[i]);
                File.Delete(loadPlayerPath[i] + ".bak");
                LoadPlayers();
            }
            catch
            {
            }
        }

        private static void EraseWorld(int i)
        {
            try
            {
                File.Delete(loadWorldPath[i]);
                File.Delete(loadWorldPath[i] + ".bak");
                LoadWorlds();
            }
            catch
            {
            }
        }

        private static string nextLoadPlayer()
        {
            int num = 1;
            while (File.Exists(Path.Combine(
                PlayerPath,
                "player" +
                    num.ToString() +
                        ".plr")))
            {
                num++;
            }
            return Path.Combine(
                PlayerPath,
                "player" +
                    num.ToString() +
                        ".plr");
        }

        private static string nextLoadWorld()
        {
            int num = 1;
            while (File.Exists(Path.Combine(
                WorldPath,
                "world" + num.ToString() + ".wld")))
            {
                num++;
            }
            return Path.Combine(
                WorldPath,
                "world" + num.ToString() + ".wld");
        }

        public void autoCreate(string newOpt)
        {
            if (newOpt == "0")
            {
                autoGen = false;
                return;
            }
            if (newOpt == "1")
            {
                maxTilesX = 4200;
                maxTilesY = 1200;
                autoGen = true;
                return;
            }
            if (newOpt == "2")
            {
                maxTilesX = 6300;
                maxTilesY = 1800;
                autoGen = true;
                return;
            }
            if (newOpt == "3")
            {
                maxTilesX = 8400;
                maxTilesY = 2400;
                autoGen = true;
            }
        }

        public void NewMOTD(string newMOTD)
        {
            motd = newMOTD;
        }

        public void LoadDedConfig(string configPath)
        {
            if (File.Exists(configPath))
            {
                using (var streamReader = new StreamReader(configPath))
                {
                    string text;
                    while ((text = streamReader.ReadLine()) != null)
                    {
                        try
                        {
                            if (text.Length > 6 && text.Substring(0, 6).ToLower() == "world=")
                            {
                                string text2 = text.Substring(6);
                                worldPathName = text2;
                            }
                            if (text.Length > 5 && text.Substring(0, 5).ToLower() == "port=")
                            {
                                string value = text.Substring(5);
                                try
                                {
                                    int serverPort = Convert.ToInt32(value);
                                    Netplay.serverPort = serverPort;
                                }
                                catch
                                {
                                }
                            }
                            if (text.Length > 11 && text.Substring(0, 9).ToLower() == "priority=")
                            {
                                string value3 = text.Substring(9);
                                try
                                {
                                    int num2 = Convert.ToInt32(value3);
                                    if (num2 >= 0 && num2 <= 5)
                                    {
                                        Process currentProcess = Process.GetCurrentProcess();
                                        switch (num2)
                                        {
                                            case 0:
                                                currentProcess.PriorityClass = ProcessPriorityClass.RealTime;
                                                break;
                                            case 1:
                                                currentProcess.PriorityClass = ProcessPriorityClass.High;
                                                break;
                                            case 2:
                                                currentProcess.PriorityClass = ProcessPriorityClass.AboveNormal;
                                                break;
                                            case 3:
                                                currentProcess.PriorityClass = ProcessPriorityClass.Normal;
                                                break;
                                            case 4:
                                                currentProcess.PriorityClass = ProcessPriorityClass.BelowNormal;
                                                break;
                                            case 5:
                                                currentProcess.PriorityClass = ProcessPriorityClass.Idle;
                                                break;
                                        }
                                    }
                                }
                                catch
                                {
                                }
                            }
                            if (text.Length >= 10 && text.Substring(0, 10).ToLower() == "worldpath=")
                            {
                                string worldPath = text.Substring(10);
                                WorldPath = worldPath;
                            }
                            if (text.Length >= 10 && text.Substring(0, 10).ToLower() == "worldname=")
                            {
                                string text4 = text.Substring(10);
                                worldName = text4;
                            }
                            if (text.Length > 11 && text.Substring(0, 11).ToLower() == "autocreate=")
                            {
                                string a = text.Substring(11);
                                switch (a)
                                {
                                    case "0":
                                        autoGen = false;
                                        break;
                                    case "1":
                                        maxTilesX = 4200;
                                        maxTilesY = 1200;
                                        autoGen = true;
                                        break;
                                    case "2":
                                        maxTilesX = 6300;
                                        maxTilesY = 1800;
                                        autoGen = true;
                                        break;
                                    case "3":
                                        maxTilesX = 8400;
                                        maxTilesY = 2400;
                                        autoGen = true;
                                        break;
                                }
                            }
                        }
                        catch
                        {
                        }
                    }
                }
            }
        }

        public void SetWorld(string wrold)
        {
            worldPathName = wrold;
        }

        public void SetWorldName(string wrold)
        {
            worldName = wrold;
        }

        public void autoShut()
        {
            autoShutdown = true;
        }

        public void AutoPass()
        {
            autoPass = true;
        }

        public void AutoJoin(string IP)
        {
            defaultIP = IP;
            getIP = IP;
            Netplay.SetIP(defaultIP);
            autoJoin = true;
        }

        public void AutoHost()
        {
            menuMultiplayer = true;
            menuServer = true;
            menuMode = 1;
        }

        public void DedServ()
        {
            Type t = Type.GetType("Mono.Runtime");
            runningMono = (t != null);
            GameHooks.OnInitialize(true);
            rand = new Random();
            Console.Title = "Terraria Server " + versionNumber2;
            dedServ = true;
            showSplash = false;
            Initialize();
            for (int i = 0; i < 147; i++)
            {
                var nPC = new NPC();
                nPC.SetDefaults(i, -1f);
                npcName[i] = nPC.name;
            }
            while (worldPathName == null || worldPathName == "")
            {
                LoadWorlds();
                bool flag = true;
                while (flag)
                {
                    Console.WriteLine("Terraria Server " + versionNumber2);
                    Console.WriteLine("");
                    for (int j = 0; j < numLoadWorlds; j++)
                    {
                        Console.WriteLine(string.Concat(new object[]
                                                            {
                                                                j + 1,
                                                                '\t',
                                                                '\t',
                                                                loadWorld[j]
                                                            }));
                    }
                    Console.WriteLine(string.Concat(new object[]
                                                        {
                                                            "n",
                                                            '\t',
                                                            '\t',
                                                            "New World"
                                                        }));
                    Console.WriteLine("d <number>" + '\t' + "Delete World");
                    Console.WriteLine("");
                    Console.Write("Choose World: ");
                    string text2 = Console.ReadLine();
                    try
                    {
                        Console.Clear();
                    }
                    catch
                    {
                    }
                    if (text2.Length >= 2 && text2.Substring(0, 2).ToLower() == "d ")
                    {
                        try
                        {
                            int num = Convert.ToInt32(text2.Substring(2)) - 1;
                            if (num < numLoadWorlds)
                            {
                                Console.WriteLine("Terraria Server " + versionNumber2);
                                Console.WriteLine("");
                                Console.WriteLine("Really delete " + loadWorld[num] + "?");
                                Console.Write("(y/n): ");
                                string text3 = Console.ReadLine();
                                if (text3.ToLower() == "y")
                                {
                                    EraseWorld(num);
                                }
                            }
                        }
                        catch
                        {
                        }
                        try
                        {
                            Console.Clear();
                            continue;
                        }
                        catch
                        {
                            continue;
                        }
                    }
                    if (text2 == "n" || text2 == "N")
                    {
                        bool flag2 = true;
                        while (flag2)
                        {
                            Console.WriteLine("Terraria Server " + versionNumber2);
                            Console.WriteLine("");
                            Console.WriteLine("1" + '\t' + "Small");
                            Console.WriteLine("2" + '\t' + "Medium");
                            Console.WriteLine("3" + '\t' + "Large");
                            Console.WriteLine("");
                            Console.Write("Choose size: ");
                            string value = Console.ReadLine();
                            try
                            {
                                int num2 = Convert.ToInt32(value);
                                if (num2 == 1)
                                {
                                    maxTilesX = 4200;
                                    maxTilesY = 1200;
                                    flag2 = false;
                                }
                                else
                                {
                                    if (num2 == 2)
                                    {
                                        maxTilesX = 6300;
                                        maxTilesY = 1800;
                                        flag2 = false;
                                    }
                                    else
                                    {
                                        if (num2 == 3)
                                        {
                                            maxTilesX = 8400;
                                            maxTilesY = 2400;
                                            flag2 = false;
                                        }
                                    }
                                }
                            }
                            catch
                            {
                            }
                            try
                            {
                                Console.Clear();
                            }
                            catch
                            {
                            }
                        }
                        flag2 = true;
                        while (flag2)
                        {
                            Console.WriteLine("Terraria Server " + versionNumber2);
                            Console.WriteLine("");
                            Console.Write("Enter world name: ");
                            newWorldName = Console.ReadLine();
                            if (newWorldName != "" && newWorldName != " " && newWorldName != null)
                            {
                                flag2 = false;
                            }
                            try
                            {
                                Console.Clear();
                            }
                            catch
                            {
                            }
                        }
                        worldName = newWorldName;
                        worldPathName = nextLoadWorld();
                        menuMode = 10;
                        WorldGen.CreateNewWorld();
                        flag2 = false;
                        while (menuMode == 10)
                        {
                            if (oldStatusText != statusText)
                            {
                                oldStatusText = statusText;
                                Console.WriteLine(statusText);
                            }
                        }
                        try
                        {
                            Console.Clear();
                            continue;
                        }
                        catch
                        {
                            continue;
                        }
                    }
                    try
                    {
                        int num3 = Convert.ToInt32(text2);
                        num3--;
                        if (num3 >= 0 && num3 < numLoadWorlds)
                        {
                            bool flag3 = true;
                            while (flag3)
                            {
                                Console.WriteLine("Terraria Server " + versionNumber2);
                                Console.WriteLine("");
                                Console.Write("Server port (press enter for 7777): ");
                                string text5 = Console.ReadLine();
                                try
                                {
                                    if (text5 == "")
                                    {
                                        text5 = "7777";
                                    }
                                    int num5 = Convert.ToInt32(text5);
                                    if (num5 <= 65535)
                                    {
                                        Netplay.serverPort = num5;
                                        flag3 = false;
                                    }
                                }
                                catch
                                {
                                }
                                try
                                {
                                    Console.Clear();
                                }
                                catch
                                {
                                }
                            }
                            worldPathName = loadWorldPath[num3];
                            flag = false;
                            try
                            {
                                Console.Clear();
                            }
                            catch
                            {
                            }
                        }
                    }
                    catch
                    {
                    }
                }
            }
            try
            {
                Console.Clear();
            }
            catch
            {
            }
            WorldGen.serverLoadWorld();
            Console.WriteLine("Terraria Server " + versionNumber);
            Console.WriteLine("");
            while (!Netplay.ServerUp)
            {
                if (oldStatusText != statusText)
                {
                    oldStatusText = statusText;
                    Console.WriteLine(statusText);
                }
            }
            try
            {
                Console.Clear();
            }
            catch
            {
            }
            Console.WriteLine("Terraria Server " + versionNumber);
            Console.WriteLine("");
            Console.WriteLine("Listening on {0}:{1}",
                Netplay.serverListenIP != IPAddress.Any ? Netplay.serverListenIP.ToString() : "*", Netplay.serverPort);
            Console.WriteLine("Type 'help' for a list of commands.");
            Console.WriteLine("");
            Console.Title = "Terraria Server: " + worldName;
            var stopwatch = new Stopwatch();
            if (!autoShutdown)
            {
                startDedInput();
            }
            GameHooks.OnInitialize(false);
            stopwatch.Start();
            double num6 = 16.666666666666668;
            double num7 = 0.0;
            int num8 = 0;
            var stopwatch2 = new Stopwatch();
            stopwatch2.Start();
            while (!Netplay.disconnect)
            {
                double num9 = stopwatch.ElapsedMilliseconds;
                if (num9 + num7 >= num6)
                {
                    num8++;
                    num7 += num9 - num6;
                    stopwatch.Reset();
                    stopwatch.Start();
                    if (oldStatusText != statusText)
                    {
                        oldStatusText = statusText;
                        Console.WriteLine(statusText);
                    }
                    if (num7 > 1000.0)
                    {
                        num7 = 1000.0;
                    }
                    if (Netplay.anyClients)
                    {
                        GameHooks.OnUpdate(true);
                        Update();
                        GameHooks.OnUpdate(false);
                    }
                    double num10 = stopwatch.ElapsedMilliseconds + num7;
                    if (num10 < num6)
                    {
                        int num11 = (int) (num6 - num10) - 1;
                        if (num11 > 1)
                        {
                            Thread.Sleep(num11);
                            if (!Netplay.anyClients)
                            {
                                num7 = 0.0;
                                Thread.Sleep(10);
                            }
                        }
                    }
                }
                Thread.Sleep(0);
            }
        }

        public static void startDedInput()
        {
            ThreadPool.QueueUserWorkItem(startDedInputCallBack, 1);
        }

        public static void startDedInputCallBack(object threadContext)
        {
            while (!Netplay.disconnect)
            {
                Console.Write(": ");
                string text = Console.ReadLine();
                if (!ServerHooks.OnCommand(text))
                {
                    string text2 = text;
                    text = text.ToLower();
                    try
                    {
                        switch (text)
                        {
                            case "help":
                                Console.WriteLine("Available commands:");
                                Console.WriteLine("");
                                Console.WriteLine(string.Concat(new object[]
                                                                    {
                                                                        "help ",
                                                                        '\t',
                                                                        '\t',
                                                                        " Displays a list of commands."
                                                                    }));
                                Console.WriteLine("playing " + '\t' + " Shows the list of players");
                                Console.WriteLine(string.Concat(new object[]
                                                                    {
                                                                        "clear ",
                                                                        '\t',
                                                                        '\t',
                                                                        " Clear the console window."
                                                                    }));
                                Console.WriteLine(string.Concat(new object[]
                                                                    {
                                                                        "exit ",
                                                                        '\t',
                                                                        '\t',
                                                                        " Shutdown the server and save."
                                                                    }));
                                Console.WriteLine("exit-nosave " + '\t' + " Shutdown the server without saving.");
                                Console.WriteLine(string.Concat(new object[]
                                                                    {
                                                                        "save ",
                                                                        '\t',
                                                                        '\t',
                                                                        " Save the game world."
                                                                    }));
                                Console.WriteLine("kick <player> " + '\t' + " Kicks a player from the server.");
                                Console.WriteLine("ban <player> " + '\t' + " Bans a player from the server.");
                                Console.WriteLine("password" + '\t' + " Show password.");
                                Console.WriteLine("password <pass>" + '\t' + " Change password.");
                                Console.WriteLine(string.Concat(new object[]
                                                                    {
                                                                        "version",
                                                                        '\t',
                                                                        '\t',
                                                                        " Print version number."
                                                                    }));
                                Console.WriteLine(string.Concat(new object[]
                                                                    {
                                                                        "time",
                                                                        '\t',
                                                                        '\t',
                                                                        " Display game time."
                                                                    }));
                                Console.WriteLine(string.Concat(new object[]
                                                                    {
                                                                        "port",
                                                                        '\t',
                                                                        '\t',
                                                                        " Print the listening port."
                                                                    }));
                                Console.WriteLine("maxplayers" + '\t' + " Print the max number of players.");
                                Console.WriteLine("say <words>" + '\t' + " Send a message.");
                                Console.WriteLine(string.Concat(new object[]
                                                                    {
                                                                        "motd",
                                                                        '\t',
                                                                        '\t',
                                                                        " Print MOTD."
                                                                    }));
                                Console.WriteLine("motd <words>" + '\t' + " Change MOTD.");
                                Console.WriteLine(string.Concat(new object[]
                                                                    {
                                                                        "dawn",
                                                                        '\t',
                                                                        '\t',
                                                                        " Change time to dawn."
                                                                    }));
                                Console.WriteLine(string.Concat(new object[]
                                                                    {
                                                                        "noon",
                                                                        '\t',
                                                                        '\t',
                                                                        " Change time to noon."
                                                                    }));
                                Console.WriteLine(string.Concat(new object[]
                                                                    {
                                                                        "dusk",
                                                                        '\t',
                                                                        '\t',
                                                                        " Change time to dusk."
                                                                    }));
                                Console.WriteLine("midnight" + '\t' + " Change time to midnight.");
                                Console.WriteLine(string.Concat(new object[]
                                                                    {
                                                                        "settle",
                                                                        '\t',
                                                                        '\t',
                                                                        " Settle all water."
                                                                    }));
                                Console.WriteLine(string.Concat(new object[]
                                                                    {
                                                                        "reload",
                                                                        '\t',
                                                                        '\t',
                                                                        " Reloads plugins."
                                                                    }));
                                break;
                            case "settle":
                                if (!Liquid.panicMode)
                                {
                                    Liquid.StartPanic();
                                }
                                else
                                {
                                    Console.WriteLine("Water is already settling");
                                }
                                break;
                            case "dawn":
                                dayTime = true;
                                time = 0.0;
                                NetMessage.SendData(7, -1, -1, "", 0, 0f, 0f, 0f, 0);
                                break;
                            case "dusk":
                                dayTime = false;
                                time = 0.0;
                                NetMessage.SendData(7, -1, -1, "", 0, 0f, 0f, 0f, 0);
                                break;
                            case "noon":
                                dayTime = true;
                                time = 27000.0;
                                NetMessage.SendData(7, -1, -1, "", 0, 0f, 0f, 0f, 0);
                                break;
                            case "midnight":
                                dayTime = false;
                                time = 16200.0;
                                NetMessage.SendData(7, -1, -1, "", 0, 0f, 0f, 0f, 0);
                                break;
                            case "exit-nosave":
                                Netplay.disconnect = true;
                                break;
                            case "exit":
                                WorldGen.saveWorld(false);
                                Netplay.disconnect = true;
                                break;
                            case "save":
                                WorldGen.saveWorld(false);
                                break;
                            case "time":
                                {
                                    string text3 = "AM";
                                    double num = time;
                                    if (!dayTime)
                                    {
                                        num += 54000.0;
                                    }
                                    num = num/86400.0*24.0;
                                    double num2 = 7.5;
                                    num = num - num2 - 12.0;
                                    if (num < 0.0)
                                    {
                                        num += 24.0;
                                    }
                                    if (num >= 12.0)
                                    {
                                        text3 = "PM";
                                    }
                                    var num3 = (int) num;
                                    double num4 = num - num3;
                                    num4 = ((int) (num4*60.0));
                                    string text4 = string.Concat(num4);
                                    if (num4 < 10.0)
                                    {
                                        text4 = "0" + text4;
                                    }
                                    if (num3 > 12)
                                    {
                                        num3 -= 12;
                                    }
                                    if (num3 == 0)
                                    {
                                        num3 = 12;
                                    }
                                    Console.WriteLine(string.Concat(new object[]
                                                                        {
                                                                            "Time: ",
                                                                            num3,
                                                                            ":",
                                                                            text4,
                                                                            " ",
                                                                            text3
                                                                        }));
                                }
                                break;
                            case "maxplayers":
                                Console.WriteLine("Player limit: " + maxNetPlayers);
                                break;
                            case "port":
                                Console.WriteLine("Port: " + Netplay.serverPort);
                                break;
                            case "version":
                                Console.WriteLine("Terraria Server " + versionNumber);
                                break;
                            default:
                                if (text == "clear")
                                {
                                    try
                                    {
                                        Console.Clear();
                                        continue;
                                    }
                                    catch
                                    {
                                        continue;
                                    }
                                }
                                if (text == "playing")
                                {
                                    int num5 = 0;
                                    for (int i = 0; i < 255; i++)
                                    {
                                        if (player[i].active)
                                        {
                                            num5++;
                                            Console.WriteLine(string.Concat(new object[]
                                                                                {
                                                                                    player[i].name,
                                                                                    " (",
                                                                                    Netplay.serverSock[i].tcpClient.Client.RemoteEndPoint,
                                                                                    ")"
                                                                                }));
                                        }
                                    }
                                    if (num5 == 0)
                                    {
                                        Console.WriteLine("No players connected.");
                                    }
                                    else
                                    {
                                        if (num5 == 1)
                                        {
                                            Console.WriteLine("1 player connected.");
                                        }
                                        else
                                        {
                                            Console.WriteLine(num5 + " players connected.");
                                        }
                                    }
                                }
                                else
                                {
                                    if (!(text == ""))
                                    {
                                        if (text == "motd")
                                        {
                                            if (motd == "")
                                            {
                                                Console.WriteLine("Welcome to " + worldName + "!");
                                            }
                                            else
                                            {
                                                Console.WriteLine("MOTD: " + motd);
                                            }
                                        }
                                        else
                                        {
                                            if (text.Length >= 5 && text.Substring(0, 5) == "motd ")
                                            {
                                                string text5 = text2.Substring(5);
                                                motd = text5;
                                            }
                                            else
                                            {
                                                if (text.Length == 8 && text.Substring(0, 8) == "password")
                                                {
                                                    if (Netplay.password == "")
                                                    {
                                                        Console.WriteLine("No password set.");
                                                    }
                                                    else
                                                    {
                                                        Console.WriteLine("Password: " + Netplay.password);
                                                    }
                                                }
                                                else
                                                {
                                                    if (text.Length >= 9 && text.Substring(0, 9) == "password ")
                                                    {
                                                        string text6 = text2.Substring(9);
                                                        if (text6 == "")
                                                        {
                                                            Netplay.password = "";
                                                            Console.WriteLine("Password disabled.");
                                                        }
                                                        else
                                                        {
                                                            Netplay.password = text6;
                                                            Console.WriteLine("Password: " + Netplay.password);
                                                        }
                                                    }
                                                    else
                                                    {
                                                        if (text == "say")
                                                        {
                                                            Console.WriteLine("Usage: say <words>");
                                                        }
                                                        else
                                                        {
                                                            if (text.Length >= 4 && text.Substring(0, 4) == "say ")
                                                            {
                                                                string text7 = text2.Substring(4);
                                                                if (text7 == "")
                                                                {
                                                                    Console.WriteLine("Usage: say <words>");
                                                                }
                                                                else
                                                                {
                                                                    Console.WriteLine("<Server> " + text7);
                                                                    NetMessage.SendData(25, -1, -1, "<Server> " + text7, 255, 255f, 240f, 20f, 0);
                                                                }
                                                            }
                                                            else
                                                            {
                                                                if (text.Length == 4 && text.Substring(0, 4) == "kick")
                                                                {
                                                                    Console.WriteLine("Usage: kick <player>");
                                                                }
                                                                else
                                                                {
                                                                    if (text.Length >= 5 && text.Substring(0, 5) == "kick ")
                                                                    {
                                                                        string text8 = text.Substring(5);
                                                                        text8 = text8.ToLower();
                                                                        if (text8 == "")
                                                                        {
                                                                            Console.WriteLine("Usage: kick <player>");
                                                                        }
                                                                        else
                                                                        {
                                                                            for (int j = 0; j < 255; j++)
                                                                            {
                                                                                if (player[j].active && player[j].name.ToLower() == text8)
                                                                                {
                                                                                    NetMessage.SendData(2, j, -1, "Kicked from server.", 0, 0f, 0f, 0f, 0);
                                                                                }
                                                                            }
                                                                        }
                                                                    }
                                                                    else
                                                                    {
                                                                        if (text.Length == 3 && text.Substring(0, 3) == "ban")
                                                                        {
                                                                            Console.WriteLine("Usage: ban <player>");
                                                                        }
                                                                        else
                                                                        {
                                                                            if (text.Length >= 4 && text.Substring(0, 4) == "ban ")
                                                                            {
                                                                                string text9 = text.Substring(4);
                                                                                text9 = text9.ToLower();
                                                                                if (text9 == "")
                                                                                {
                                                                                    Console.WriteLine("Usage: ban <player>");
                                                                                }
                                                                                else
                                                                                {
                                                                                    for (int k = 0; k < 255; k++)
                                                                                    {
                                                                                        if (player[k].active && player[k].name.ToLower() == text9)
                                                                                        {
                                                                                            Netplay.AddBan(k);
                                                                                            NetMessage.SendData(2, k, -1, "Banned from server.", 0, 0f, 0f, 0f, 0);
                                                                                        }
                                                                                    }
                                                                                }
                                                                            }
                                                                            else
                                                                            {
                                                                                Console.WriteLine("Invalid command.");
                                                                            }
                                                                        }
                                                                    }
                                                                }
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                                break;
                        }
                    }
                    catch
                    {
                        Console.WriteLine("Invalid command.");
                    }
                }
            }
        }

        protected void Initialize()
        {
            NPC.clrNames();
            NPC.setNames();
            bgAlpha[0] = 1f;
            bgAlpha2[0] = 1f;
            for (int i = 0; i < 111; i++)
            {
                projFrames[i] = 1;
            }
            projFrames[72] = 4;
            projFrames[86] = 4;
            projFrames[87] = 4;
            projFrames[102] = 2;
            debuff[20] = true;
            debuff[21] = true;
            debuff[22] = true;
            debuff[23] = true;
            debuff[24] = true;
            debuff[25] = true;
            debuff[28] = true;
            debuff[30] = true;
            debuff[31] = true;
            debuff[32] = true;
            debuff[33] = true;
            debuff[34] = true;
            debuff[35] = true;
            debuff[36] = true;
            debuff[37] = true;
            debuff[38] = true;
            debuff[39] = true;
            buffName[1] = "Obsidian Skin";
            buffTip[1] = "Immune to lava";
            buffName[2] = "Regeneration";
            buffTip[2] = "Provides life regeneration";
            buffName[3] = "Swiftness";
            buffTip[3] = "25% increased movement speed";
            buffName[4] = "Gills";
            buffTip[4] = "Breathe water instead of air";
            buffName[5] = "Ironskin";
            buffTip[5] = "Increase defense by 8";
            buffName[6] = "Mana Regeneration";
            buffTip[6] = "Increased mana regeneration";
            buffName[7] = "Magic Power";
            buffTip[7] = "20% increased magic damage";
            buffName[8] = "Featherfall";
            buffTip[8] = "Press UP or DOWN to control speed of descent";
            buffName[9] = "Spelunker";
            buffTip[9] = "Shows the location of treasure and ore";
            buffName[10] = "Invisibility";
            buffTip[10] = "Grants invisibility";
            buffName[11] = "Shine";
            buffTip[11] = "Emitting light";
            buffName[12] = "Night Owl";
            buffTip[12] = "Increased night vision";
            buffName[13] = "Battle";
            buffTip[13] = "Increased enemy spawn rate";
            buffName[14] = "Thorns";
            buffTip[14] = "Attackers also take damage";
            buffName[15] = "Water Walking";
            buffTip[15] = "Press DOWN to enter water";
            buffName[16] = "Archery";
            buffTip[16] = "20% increased arrow damage and speed";
            buffName[17] = "Hunter";
            buffTip[17] = "Shows the location of enemies";
            buffName[18] = "Gravitation";
            buffTip[18] = "Press UP or DOWN to reverse gravity";
            buffName[19] = "Orb of Light";
            buffTip[19] = "A magical orb that provides light";
            buffName[20] = "Poisoned";
            buffTip[20] = "Slowly losing life";
            buffName[21] = "Potion Sickness";
            buffTip[21] = "Cannot consume anymore healing items";
            buffName[22] = "Darkness";
            buffTip[22] = "Decreased light vision";
            buffName[23] = "Cursed";
            buffTip[23] = "Cannot use any items";
            buffName[24] = "On Fire!";
            buffTip[24] = "Slowly losing life";
            buffName[25] = "Tipsy";
            buffTip[25] = "Increased melee abilities, lowered defense";
            buffName[26] = "Well Fed";
            buffTip[26] = "Minor improvements to all stats";
            buffName[27] = "Fairy";
            buffTip[27] = "A fairy is following you";
            buffName[28] = "Werewolf";
            buffTip[28] = "Physical abilities are increased";
            buffName[29] = "Clairvoyance";
            buffTip[29] = "Magic powers are increased";
            buffName[30] = "Bleeding";
            buffTip[30] = "Cannot regenerate life";
            buffName[31] = "Confused";
            buffTip[31] = "Movement is reversed";
            buffName[32] = "Slow";
            buffTip[32] = "Movement speed is reduced";
            buffName[33] = "Weak";
            buffTip[33] = "Physical abilities are decreased";
            buffName[34] = "Merfolk";
            buffTip[34] = "Can breathe and move easily underwater";
            buffName[35] = "Silenced";
            buffTip[35] = "Cannot use items that require mana";
            buffName[36] = "Broken Armor";
            buffTip[36] = "Defense is cut in half";
            buffName[37] = "Horrified";
            buffTip[37] = "You have seen something nasty, there is no escape.";
            buffName[38] = "The Tongue";
            buffTip[38] = "You are being sucked into the mouth";
            buffName[39] = "Cursed Inferno";
            buffTip[39] = "Losing life";
            for (int j = 0; j < 10; j++)
            {
                recentWorld[j] = "";
                recentIP[j] = "";
                recentPort[j] = 0;
            }
            if (rand == null)
            {
                rand = new Random((int) DateTime.Now.Ticks);
            }
            if (WorldGen.genRand == null)
            {
                WorldGen.genRand = new Random((int) DateTime.Now.Ticks);
            }
            int num = rand.Next(15);
            lo = rand.Next(6);
            tileShine2[6] = true;
            tileShine2[7] = true;
            tileShine2[8] = true;
            tileShine2[9] = true;
            tileShine2[12] = true;
            tileShine2[21] = true;
            tileShine2[22] = true;
            tileShine2[25] = true;
            tileShine2[45] = true;
            tileShine2[46] = true;
            tileShine2[47] = true;
            tileShine2[63] = true;
            tileShine2[64] = true;
            tileShine2[65] = true;
            tileShine2[66] = true;
            tileShine2[67] = true;
            tileShine2[68] = true;
            tileShine2[107] = true;
            tileShine2[108] = true;
            tileShine2[111] = true;
            tileShine2[121] = true;
            tileShine2[122] = true;
            tileShine2[117] = true;
            tileShine[129] = 300;
            tileHammer[141] = true;
            tileHammer[4] = true;
            tileHammer[10] = true;
            tileHammer[11] = true;
            tileHammer[12] = true;
            tileHammer[13] = true;
            tileHammer[14] = true;
            tileHammer[15] = true;
            tileHammer[16] = true;
            tileHammer[17] = true;
            tileHammer[18] = true;
            tileHammer[19] = true;
            tileHammer[21] = true;
            tileHammer[26] = true;
            tileHammer[28] = true;
            tileHammer[29] = true;
            tileHammer[31] = true;
            tileHammer[33] = true;
            tileHammer[34] = true;
            tileHammer[35] = true;
            tileHammer[36] = true;
            tileHammer[42] = true;
            tileHammer[48] = true;
            tileHammer[49] = true;
            tileHammer[50] = true;
            tileHammer[54] = true;
            tileHammer[55] = true;
            tileHammer[77] = true;
            tileHammer[78] = true;
            tileHammer[79] = true;
            tileHammer[81] = true;
            tileHammer[85] = true;
            tileHammer[86] = true;
            tileHammer[87] = true;
            tileHammer[88] = true;
            tileHammer[89] = true;
            tileHammer[90] = true;
            tileHammer[91] = true;
            tileHammer[92] = true;
            tileHammer[93] = true;
            tileHammer[94] = true;
            tileHammer[95] = true;
            tileHammer[96] = true;
            tileHammer[97] = true;
            tileHammer[98] = true;
            tileHammer[99] = true;
            tileHammer[100] = true;
            tileHammer[101] = true;
            tileHammer[102] = true;
            tileHammer[103] = true;
            tileHammer[104] = true;
            tileHammer[105] = true;
            tileHammer[106] = true;
            tileHammer[114] = true;
            tileHammer[125] = true;
            tileHammer[126] = true;
            tileHammer[128] = true;
            tileHammer[129] = true;
            tileHammer[132] = true;
            tileHammer[133] = true;
            tileHammer[134] = true;
            tileHammer[135] = true;
            tileHammer[136] = true;
            tileFrameImportant[139] = true;
            tileHammer[139] = true;
            tileLighted[149] = true;
            tileFrameImportant[149] = true;
            tileHammer[149] = true;
            tileFrameImportant[142] = true;
            tileHammer[142] = true;
            tileFrameImportant[143] = true;
            tileHammer[143] = true;
            tileFrameImportant[144] = true;
            tileHammer[144] = true;
            tileStone[131] = true;
            tileFrameImportant[136] = true;
            tileFrameImportant[137] = true;
            tileFrameImportant[138] = true;
            tileBlockLight[137] = true;
            tileSolid[137] = true;
            tileBlockLight[145] = true;
            tileSolid[145] = true;
            tileMergeDirt[145] = true;
            tileBlockLight[146] = true;
            tileSolid[146] = true;
            tileMergeDirt[146] = true;
            tileBlockLight[147] = true;
            tileSolid[147] = true;
            tileMergeDirt[147] = true;
            tileBlockLight[148] = true;
            tileSolid[148] = true;
            tileMergeDirt[148] = true;
            tileBlockLight[138] = true;
            tileSolid[138] = true;
            tileBlockLight[140] = true;
            tileSolid[140] = true;
            tileAxe[5] = true;
            tileAxe[30] = true;
            tileAxe[72] = true;
            tileAxe[80] = true;
            tileAxe[124] = true;
            tileShine[22] = 1150;
            tileShine[6] = 1150;
            tileShine[7] = 1100;
            tileShine[8] = 1000;
            tileShine[9] = 1050;
            tileShine[12] = 1000;
            tileShine[21] = 1200;
            tileShine[63] = 900;
            tileShine[64] = 900;
            tileShine[65] = 900;
            tileShine[66] = 900;
            tileShine[67] = 900;
            tileShine[68] = 900;
            tileShine[45] = 1900;
            tileShine[46] = 2000;
            tileShine[47] = 2100;
            tileShine[122] = 1800;
            tileShine[121] = 1850;
            tileShine[125] = 600;
            tileShine[109] = 9000;
            tileShine[110] = 9000;
            tileShine[116] = 9000;
            tileShine[117] = 9000;
            tileShine[118] = 8000;
            tileShine[107] = 950;
            tileShine[108] = 900;
            tileShine[111] = 850;
            tileLighted[4] = true;
            tileLighted[17] = true;
            tileLighted[133] = true;
            tileLighted[31] = true;
            tileLighted[33] = true;
            tileLighted[34] = true;
            tileLighted[35] = true;
            tileLighted[36] = true;
            tileLighted[37] = true;
            tileLighted[42] = true;
            tileLighted[49] = true;
            tileLighted[58] = true;
            tileLighted[61] = true;
            tileLighted[70] = true;
            tileLighted[71] = true;
            tileLighted[72] = true;
            tileLighted[76] = true;
            tileLighted[77] = true;
            tileLighted[19] = true;
            tileLighted[22] = true;
            tileLighted[26] = true;
            tileLighted[83] = true;
            tileLighted[84] = true;
            tileLighted[92] = true;
            tileLighted[93] = true;
            tileLighted[95] = true;
            tileLighted[98] = true;
            tileLighted[100] = true;
            tileLighted[109] = true;
            tileLighted[125] = true;
            tileLighted[126] = true;
            tileLighted[129] = true;
            tileLighted[140] = true;
            tileMergeDirt[1] = true;
            tileMergeDirt[6] = true;
            tileMergeDirt[7] = true;
            tileMergeDirt[8] = true;
            tileMergeDirt[9] = true;
            tileMergeDirt[22] = true;
            tileMergeDirt[25] = true;
            tileMergeDirt[30] = true;
            tileMergeDirt[37] = true;
            tileMergeDirt[38] = true;
            tileMergeDirt[40] = true;
            tileMergeDirt[53] = true;
            tileMergeDirt[56] = true;
            tileMergeDirt[107] = true;
            tileMergeDirt[108] = true;
            tileMergeDirt[111] = true;
            tileMergeDirt[112] = true;
            tileMergeDirt[116] = true;
            tileMergeDirt[117] = true;
            tileMergeDirt[123] = true;
            tileMergeDirt[140] = true;
            tileMergeDirt[39] = true;
            tileMergeDirt[122] = true;
            tileMergeDirt[121] = true;
            tileMergeDirt[120] = true;
            tileMergeDirt[119] = true;
            tileMergeDirt[118] = true;
            tileMergeDirt[47] = true;
            tileMergeDirt[46] = true;
            tileMergeDirt[45] = true;
            tileMergeDirt[44] = true;
            tileMergeDirt[43] = true;
            tileMergeDirt[41] = true;
            tileFrameImportant[3] = true;
            tileFrameImportant[4] = true;
            tileFrameImportant[5] = true;
            tileFrameImportant[10] = true;
            tileFrameImportant[11] = true;
            tileFrameImportant[12] = true;
            tileFrameImportant[13] = true;
            tileFrameImportant[14] = true;
            tileFrameImportant[15] = true;
            tileFrameImportant[16] = true;
            tileFrameImportant[17] = true;
            tileFrameImportant[18] = true;
            tileFrameImportant[20] = true;
            tileFrameImportant[21] = true;
            tileFrameImportant[24] = true;
            tileFrameImportant[26] = true;
            tileFrameImportant[27] = true;
            tileFrameImportant[28] = true;
            tileFrameImportant[29] = true;
            tileFrameImportant[31] = true;
            tileFrameImportant[33] = true;
            tileFrameImportant[34] = true;
            tileFrameImportant[35] = true;
            tileFrameImportant[36] = true;
            tileFrameImportant[42] = true;
            tileFrameImportant[50] = true;
            tileFrameImportant[55] = true;
            tileFrameImportant[61] = true;
            tileFrameImportant[71] = true;
            tileFrameImportant[72] = true;
            tileFrameImportant[73] = true;
            tileFrameImportant[74] = true;
            tileFrameImportant[77] = true;
            tileFrameImportant[78] = true;
            tileFrameImportant[79] = true;
            tileFrameImportant[81] = true;
            tileFrameImportant[82] = true;
            tileFrameImportant[83] = true;
            tileFrameImportant[84] = true;
            tileFrameImportant[85] = true;
            tileFrameImportant[86] = true;
            tileFrameImportant[87] = true;
            tileFrameImportant[88] = true;
            tileFrameImportant[89] = true;
            tileFrameImportant[90] = true;
            tileFrameImportant[91] = true;
            tileFrameImportant[92] = true;
            tileFrameImportant[93] = true;
            tileFrameImportant[94] = true;
            tileFrameImportant[95] = true;
            tileFrameImportant[96] = true;
            tileFrameImportant[97] = true;
            tileFrameImportant[98] = true;
            tileFrameImportant[99] = true;
            tileFrameImportant[101] = true;
            tileFrameImportant[102] = true;
            tileFrameImportant[103] = true;
            tileFrameImportant[104] = true;
            tileFrameImportant[105] = true;
            tileFrameImportant[100] = true;
            tileFrameImportant[106] = true;
            tileFrameImportant[110] = true;
            tileFrameImportant[113] = true;
            tileFrameImportant[114] = true;
            tileFrameImportant[125] = true;
            tileFrameImportant[126] = true;
            tileFrameImportant[128] = true;
            tileFrameImportant[129] = true;
            tileFrameImportant[132] = true;
            tileFrameImportant[133] = true;
            tileFrameImportant[134] = true;
            tileFrameImportant[135] = true;
            tileFrameImportant[141] = true;
            tileCut[3] = true;
            tileCut[24] = true;
            tileCut[28] = true;
            tileCut[32] = true;
            tileCut[51] = true;
            tileCut[52] = true;
            tileCut[61] = true;
            tileCut[62] = true;
            tileCut[69] = true;
            tileCut[71] = true;
            tileCut[73] = true;
            tileCut[74] = true;
            tileCut[82] = true;
            tileCut[83] = true;
            tileCut[84] = true;
            tileCut[110] = true;
            tileCut[113] = true;
            tileCut[115] = true;
            tileAlch[82] = true;
            tileAlch[83] = true;
            tileAlch[84] = true;
            tileLavaDeath[104] = true;
            tileLavaDeath[110] = true;
            tileLavaDeath[113] = true;
            tileLavaDeath[115] = true;
            tileSolid[127] = true;
            tileSolid[130] = true;
            tileBlockLight[130] = true;
            tileBlockLight[131] = true;
            tileSolid[107] = true;
            tileBlockLight[107] = true;
            tileSolid[108] = true;
            tileBlockLight[108] = true;
            tileSolid[111] = true;
            tileBlockLight[111] = true;
            tileSolid[109] = true;
            tileBlockLight[109] = true;
            tileSolid[110] = false;
            tileNoAttach[110] = true;
            tileNoFail[110] = true;
            tileSolid[112] = true;
            tileBlockLight[112] = true;
            tileSolid[116] = true;
            tileBlockLight[116] = true;
            tileSolid[117] = true;
            tileBlockLight[117] = true;
            tileSolid[123] = true;
            tileBlockLight[123] = true;
            tileSolid[118] = true;
            tileBlockLight[118] = true;
            tileSolid[119] = true;
            tileBlockLight[119] = true;
            tileSolid[120] = true;
            tileBlockLight[120] = true;
            tileSolid[121] = true;
            tileBlockLight[121] = true;
            tileSolid[122] = true;
            tileBlockLight[122] = true;
            tileBlockLight[115] = true;
            tileSolid[0] = true;
            tileBlockLight[0] = true;
            tileSolid[1] = true;
            tileBlockLight[1] = true;
            tileSolid[2] = true;
            tileBlockLight[2] = true;
            tileSolid[3] = false;
            tileNoAttach[3] = true;
            tileNoFail[3] = true;
            tileSolid[4] = false;
            tileNoAttach[4] = true;
            tileNoFail[4] = true;
            tileNoFail[24] = true;
            tileSolid[5] = false;
            tileSolid[6] = true;
            tileBlockLight[6] = true;
            tileSolid[7] = true;
            tileBlockLight[7] = true;
            tileSolid[8] = true;
            tileBlockLight[8] = true;
            tileSolid[9] = true;
            tileBlockLight[9] = true;
            tileBlockLight[10] = true;
            tileSolid[10] = true;
            tileNoAttach[10] = true;
            tileBlockLight[10] = true;
            tileSolid[11] = false;
            tileSolidTop[19] = true;
            tileSolid[19] = true;
            tileSolid[22] = true;
            tileSolid[23] = true;
            tileSolid[25] = true;
            tileSolid[30] = true;
            tileNoFail[32] = true;
            tileBlockLight[32] = true;
            tileSolid[37] = true;
            tileBlockLight[37] = true;
            tileSolid[38] = true;
            tileBlockLight[38] = true;
            tileSolid[39] = true;
            tileBlockLight[39] = true;
            tileSolid[40] = true;
            tileBlockLight[40] = true;
            tileSolid[41] = true;
            tileBlockLight[41] = true;
            tileSolid[43] = true;
            tileBlockLight[43] = true;
            tileSolid[44] = true;
            tileBlockLight[44] = true;
            tileSolid[45] = true;
            tileBlockLight[45] = true;
            tileSolid[46] = true;
            tileBlockLight[46] = true;
            tileSolid[47] = true;
            tileBlockLight[47] = true;
            tileSolid[48] = true;
            tileBlockLight[48] = true;
            tileSolid[53] = true;
            tileBlockLight[53] = true;
            tileSolid[54] = true;
            tileBlockLight[52] = true;
            tileSolid[56] = true;
            tileBlockLight[56] = true;
            tileSolid[57] = true;
            tileBlockLight[57] = true;
            tileSolid[58] = true;
            tileBlockLight[58] = true;
            tileSolid[59] = true;
            tileBlockLight[59] = true;
            tileSolid[60] = true;
            tileBlockLight[60] = true;
            tileSolid[63] = true;
            tileBlockLight[63] = true;
            tileStone[63] = true;
            tileStone[130] = true;
            tileSolid[64] = true;
            tileBlockLight[64] = true;
            tileStone[64] = true;
            tileSolid[65] = true;
            tileBlockLight[65] = true;
            tileStone[65] = true;
            tileSolid[66] = true;
            tileBlockLight[66] = true;
            tileStone[66] = true;
            tileSolid[67] = true;
            tileBlockLight[67] = true;
            tileStone[67] = true;
            tileSolid[68] = true;
            tileBlockLight[68] = true;
            tileStone[68] = true;
            tileSolid[75] = true;
            tileBlockLight[75] = true;
            tileSolid[76] = true;
            tileBlockLight[76] = true;
            tileSolid[70] = true;
            tileBlockLight[70] = true;
            tileNoFail[50] = true;
            tileNoAttach[50] = true;
            tileDungeon[41] = true;
            tileDungeon[43] = true;
            tileDungeon[44] = true;
            tileBlockLight[30] = true;
            tileBlockLight[25] = true;
            tileBlockLight[23] = true;
            tileBlockLight[22] = true;
            tileBlockLight[62] = true;
            tileSolidTop[18] = true;
            tileSolidTop[14] = true;
            tileSolidTop[16] = true;
            tileSolidTop[114] = true;
            tileNoAttach[20] = true;
            tileNoAttach[19] = true;
            tileNoAttach[13] = true;
            tileNoAttach[14] = true;
            tileNoAttach[15] = true;
            tileNoAttach[16] = true;
            tileNoAttach[17] = true;
            tileNoAttach[18] = true;
            tileNoAttach[19] = true;
            tileNoAttach[21] = true;
            tileNoAttach[27] = true;
            tileNoAttach[114] = true;
            tileTable[14] = true;
            tileTable[18] = true;
            tileTable[19] = true;
            tileTable[114] = true;
            tileNoAttach[86] = true;
            tileNoAttach[87] = true;
            tileNoAttach[88] = true;
            tileNoAttach[89] = true;
            tileNoAttach[90] = true;
            tileLavaDeath[86] = true;
            tileLavaDeath[87] = true;
            tileLavaDeath[88] = true;
            tileLavaDeath[89] = true;
            tileLavaDeath[125] = true;
            tileLavaDeath[126] = true;
            tileLavaDeath[101] = true;
            tileTable[101] = true;
            tileNoAttach[101] = true;
            tileLavaDeath[102] = true;
            tileNoAttach[102] = true;
            tileNoAttach[94] = true;
            tileNoAttach[95] = true;
            tileNoAttach[96] = true;
            tileNoAttach[97] = true;
            tileNoAttach[98] = true;
            tileNoAttach[99] = true;
            tileLavaDeath[94] = true;
            tileLavaDeath[95] = true;
            tileLavaDeath[96] = true;
            tileLavaDeath[97] = true;
            tileLavaDeath[98] = true;
            tileLavaDeath[99] = true;
            tileLavaDeath[100] = true;
            tileLavaDeath[103] = true;
            tileTable[87] = true;
            tileTable[88] = true;
            tileSolidTop[87] = true;
            tileSolidTop[88] = true;
            tileSolidTop[101] = true;
            tileNoAttach[91] = true;
            tileLavaDeath[91] = true;
            tileNoAttach[92] = true;
            tileLavaDeath[92] = true;
            tileNoAttach[93] = true;
            tileLavaDeath[93] = true;
            tileWaterDeath[4] = true;
            tileWaterDeath[51] = true;
            tileWaterDeath[93] = true;
            tileWaterDeath[98] = true;
            tileLavaDeath[3] = true;
            tileLavaDeath[5] = true;
            tileLavaDeath[10] = true;
            tileLavaDeath[11] = true;
            tileLavaDeath[12] = true;
            tileLavaDeath[13] = true;
            tileLavaDeath[14] = true;
            tileLavaDeath[15] = true;
            tileLavaDeath[16] = true;
            tileLavaDeath[17] = true;
            tileLavaDeath[18] = true;
            tileLavaDeath[19] = true;
            tileLavaDeath[20] = true;
            tileLavaDeath[27] = true;
            tileLavaDeath[28] = true;
            tileLavaDeath[29] = true;
            tileLavaDeath[32] = true;
            tileLavaDeath[33] = true;
            tileLavaDeath[34] = true;
            tileLavaDeath[35] = true;
            tileLavaDeath[36] = true;
            tileLavaDeath[42] = true;
            tileLavaDeath[49] = true;
            tileLavaDeath[50] = true;
            tileLavaDeath[52] = true;
            tileLavaDeath[55] = true;
            tileLavaDeath[61] = true;
            tileLavaDeath[62] = true;
            tileLavaDeath[69] = true;
            tileLavaDeath[71] = true;
            tileLavaDeath[72] = true;
            tileLavaDeath[73] = true;
            tileLavaDeath[74] = true;
            tileLavaDeath[79] = true;
            tileLavaDeath[80] = true;
            tileLavaDeath[81] = true;
            tileLavaDeath[106] = true;
            wallHouse[1] = true;
            wallHouse[4] = true;
            wallHouse[5] = true;
            wallHouse[6] = true;
            wallHouse[10] = true;
            wallHouse[11] = true;
            wallHouse[12] = true;
            wallHouse[16] = true;
            wallHouse[17] = true;
            wallHouse[18] = true;
            wallHouse[19] = true;
            wallHouse[20] = true;
            wallHouse[21] = true;
            wallHouse[22] = true;
            wallHouse[23] = true;
            wallHouse[24] = true;
            wallHouse[25] = true;
            wallHouse[26] = true;
            wallHouse[27] = true;
            wallHouse[29] = true;
            wallHouse[30] = true;
            wallHouse[31] = true;
            for (int k = 0; k < 32; k++)
            {
                switch (k)
                {
                    case 20:
                        wallBlend[k] = 14;
                        break;
                    case 19:
                        wallBlend[k] = 9;
                        break;
                    case 18:
                        wallBlend[k] = 8;
                        break;
                    case 17:
                        wallBlend[k] = 7;
                        break;
                    case 16:
                        wallBlend[k] = 2;
                        break;
                    default:
                        wallBlend[k] = k;
                        break;
                }
            }
            tileNoFail[32] = true;
            tileNoFail[61] = true;
            tileNoFail[69] = true;
            tileNoFail[73] = true;
            tileNoFail[74] = true;
            tileNoFail[82] = true;
            tileNoFail[83] = true;
            tileNoFail[84] = true;
            tileNoFail[110] = true;
            tileNoFail[113] = true;
            for (int l = 0; l < 150; l++)
            {
                tileName[l] = "";
                if (tileSolid[l])
                {
                    tileNoSunLight[l] = true;
                }
            }
            tileNoSunLight[19] = false;
            tileNoSunLight[11] = true;
            tileName[13] = "Bottle";
            tileName[14] = "Table";
            tileName[15] = "Chair";
            tileName[16] = "Anvil";
            tileName[17] = "Furnace";
            tileName[18] = "Workbench";
            tileName[26] = "Demon Altar";
            tileName[77] = "Hellforge";
            tileName[86] = "Loom";
            tileName[94] = "Keg";
            tileName[96] = "Cooking Pot";
            tileName[101] = "Bookcase";
            tileName[106] = "Sawmill";
            tileName[114] = "Tinkerer's Workshop";
            tileName[133] = "Adamantite Forge";
            tileName[134] = "Mythril Anvil";
            for (int m = 0; m < maxMenuItems; m++)
            {
                menuItemScale[m] = 0.8f;
            }
            for (int n = 0; n < 2001; n++)
            {
                dust[n] = new Dust();
            }
            for (int num2 = 0; num2 < 201; num2++)
            {
                item[num2] = new Item();
            }
            for (int num3 = 0; num3 < 201; num3++)
            {
                npc[num3] = new NPC();
                npc[num3].whoAmI = num3;
            }
            for (int num4 = 0; num4 < 256; num4++)
            {
                player[num4] = new Player();
            }
            for (int num5 = 0; num5 < 1001; num5++)
            {
                projectile[num5] = new Projectile();
            }
            for (int num6 = 0; num6 < 201; num6++)
            {
                gore[num6] = new Gore();
            }
            for (int num7 = 0; num7 < 100; num7++)
            {
                cloud[num7] = new Cloud();
            }
            for (int num8 = 0; num8 < 100; num8++)
            {
                combatText[num8] = new CombatText();
            }
            for (int num9 = 0; num9 < 20; num9++)
            {
                itemText[num9] = new ItemText();
            }
            for (int num10 = 0; num10 < 603; num10++)
            {
                var item = new Item();
                item.SetDefaults(num10, false);
                itemName[num10] = item.name;
                if (item.headSlot > 0)
                {
                    Item.headType[item.headSlot] = item.type;
                }
                if (item.bodySlot > 0)
                {
                    Item.bodyType[item.bodySlot] = item.type;
                }
                if (item.legSlot > 0)
                {
                    Item.legType[item.legSlot] = item.type;
                }
            }
            for (int num11 = 0; num11 < Recipe.maxRecipes; num11++)
            {
                recipe[num11] = new Recipe();
                availableRecipeY[num11] = (65*num11);
            }
            Recipe.SetupRecipes();
            for (int num12 = 0; num12 < numChatLines; num12++)
            {
                chatLine[num12] = new ChatLine();
            }
            for (int num13 = 0; num13 < Liquid.resLiquid; num13++)
            {
                liquid[num13] = new Liquid();
            }
            for (int num14 = 0; num14 < 10000; num14++)
            {
                liquidBuffer[num14] = new LiquidBuffer();
            }
            shop[0] = new Chest();
            shop[1] = new Chest();
            shop[1].SetupShop(1);
            shop[2] = new Chest();
            shop[2].SetupShop(2);
            shop[3] = new Chest();
            shop[3].SetupShop(3);
            shop[4] = new Chest();
            shop[4].SetupShop(4);
            shop[5] = new Chest();
            shop[5].SetupShop(5);
            shop[6] = new Chest();
            shop[6].SetupShop(6);
            shop[7] = new Chest();
            shop[7].SetupShop(7);
            shop[8] = new Chest();
            shop[8].SetupShop(8);
            shop[9] = new Chest();
            shop[9].SetupShop(9);
            teamColor[0] = Color.White;
            teamColor[1] = new Color(230, 40, 20);
            teamColor[2] = new Color(20, 200, 30);
            teamColor[3] = new Color(75, 90, 255);
            teamColor[4] = new Color(200, 180, 0);
            Netplay.Init();
            if (skipMenu)
            {
                WorldGen.clearWorld();
                gameMenu = false;
                LoadPlayers();
                player[myPlayer] = (Player) loadPlayer[0].Clone();
                PlayerPath = loadPlayerPath[0];
                LoadWorlds();
                WorldGen.generateWorld(-1);
                WorldGen.EveryTileFrame();
                player[myPlayer].Spawn();
            }
            else
            {
            }
            if (dedServ)
            {
                return;
            }
        }

        public static void checkXMas()
        {
            DateTime now = DateTime.Now;
            int day = now.Day;
            int month = now.Month;
            if (day >= 15 && month == 12)
            {
                xMas = true;
                return;
            }
            xMas = false;
        }

        protected void Update()
        {
            var stopwatch = new Stopwatch();
            stopwatch.Start();
            WorldGen.destroyObject = false;
            gamePaused = false;
            int n = 0;
            while (n < 255)
            {
                if (ignoreErrors)
                {
                    try
                    {
                        player[n].UpdatePlayer(n);
                        goto IL_15A6;
                    }
                    catch
                    {
                        goto IL_15A6;
                    }
                    goto IL_1597;
                }
                goto IL_1597;
                IL_15A6:
                n++;
                continue;
                IL_1597:
                player[n].UpdatePlayer(n);
                goto IL_15A6;
            }
            if (netMode != 1)
            {
                NPC.SpawnNPC();
            }
            for (int num7 = 0; num7 < 255; num7++)
            {
                player[num7].activeNPCs = 0f;
                player[num7].townNPCs = 0f;
            }
            if (wof >= 0 && !npc[wof].active)
            {
                wof = -1;
            }
            int num8 = 0;
            while (num8 < 200)
            {
                if (ignoreErrors)
                {
                    try
                    {
                        npc[num8].UpdateNPC(num8);
                        goto IL_1656;
                    }
                    catch (Exception)
                    {
                        npc[num8] = new NPC();
                        goto IL_1656;
                    }
                    goto IL_1647;
                }
                goto IL_1647;
                IL_1656:
                num8++;
                continue;
                IL_1647:
                npc[num8].UpdateNPC(num8);
                goto IL_1656;
            }
            int num9 = 0;
            while (num9 < 200)
            {
                if (ignoreErrors)
                {
                    try
                    {
                        gore[num9].Update();
                        goto IL_169D;
                    }
                    catch
                    {
                        gore[num9] = new Gore();
                        goto IL_169D;
                    }
                    goto IL_1690;
                }
                goto IL_1690;
                IL_169D:
                num9++;
                continue;
                IL_1690:
                gore[num9].Update();
                goto IL_169D;
            }
            int num10 = 0;
            while (num10 < 1000)
            {
                if (ignoreErrors)
                {
                    try
                    {
                        projectile[num10].Update(num10);
                        goto IL_16E8;
                    }
                    catch
                    {
                        projectile[num10] = new Projectile();
                        goto IL_16E8;
                    }
                    goto IL_16D9;
                }
                goto IL_16D9;
                IL_16E8:
                num10++;
                continue;
                IL_16D9:
                projectile[num10].Update(num10);
                goto IL_16E8;
            }
            int num11 = 0;
            while (num11 < 200)
            {
                if (ignoreErrors)
                {
                    try
                    {
                        item[num11].UpdateItem(num11);
                        goto IL_1733;
                    }
                    catch
                    {
                        item[num11] = new Item();
                        goto IL_1733;
                    }
                    goto IL_1724;
                }
                goto IL_1724;
                IL_1733:
                num11++;
                continue;
                IL_1724:
                item[num11].UpdateItem(num11);
                goto IL_1733;
            }
            if (ignoreErrors)
            {
                try
                {
                    Dust.UpdateDust();
                    goto IL_1779;
                }
                catch
                {
                    for (int num12 = 0; num12 < 2000; num12++)
                    {
                        dust[num12] = new Dust();
                    }
                    goto IL_1779;
                }
            }
            Dust.UpdateDust();
            IL_1779:
            if (netMode != 2)
            {
                CombatText.UpdateCombatText();
                ItemText.UpdateItemText();
            }
            if (ignoreErrors)
            {
                try
                {
                    UpdateTime();
                    goto IL_17A7;
                }
                catch
                {
                    checkForSpawns = 0;
                    goto IL_17A7;
                }
            }
            UpdateTime();
            IL_17A7:
            if (netMode != 1)
            {
                if (ignoreErrors)
                {
                    try
                    {
                        WorldGen.UpdateWorld();
                        UpdateInvasion();
                        goto IL_17CF;
                    }
                    catch
                    {
                        goto IL_17CF;
                    }
                }
                WorldGen.UpdateWorld();
                UpdateInvasion();
            }
            IL_17CF:
            if (ignoreErrors)
            {
                try
                {
                    if (netMode == 2)
                    {
                        UpdateServer();
                    }
                    if (netMode == 1)
                    {
                    }
                    goto IL_1817;
                }
                catch
                {
                    int arg_17FA_0 = netMode;
                    goto IL_1817;
                }
            }
            if (netMode == 2)
            {
                UpdateServer();
                goto IL_180A;
            }
            goto IL_180A;
            IL_1817:
            if (ignoreErrors)
            {
                try
                {
                    for (int num13 = 0; num13 < numChatLines; num13++)
                    {
                        if (chatLine[num13].showTime > 0)
                        {
                            chatLine[num13].showTime--;
                        }
                    }
                    goto IL_18B6;
                }
                catch
                {
                    for (int num14 = 0; num14 < numChatLines; num14++)
                    {
                        chatLine[num14] = new ChatLine();
                    }
                    goto IL_18B6;
                }
                goto IL_187D;
            }
            goto IL_187D;
            IL_18B6:
            upTimer = stopwatch.ElapsedMilliseconds;
            if (upTimerMaxDelay > 0f)
            {
                upTimerMaxDelay -= 1f;
                goto IL_18EA;
            }
            upTimerMax = 0f;
            goto IL_18EA;
            IL_187D:
            for (int num15 = 0; num15 < numChatLines; num15++)
            {
                if (chatLine[num15].showTime > 0)
                {
                    chatLine[num15].showTime--;
                }
            }
            goto IL_18B6;
            IL_180A:
            if (netMode == 1)
            {
                goto IL_1817;
            }
            goto IL_1817;
            IL_18EA:
            if (upTimer > upTimerMax)
            {
                upTimerMax = upTimer;
                upTimerMaxDelay = 400f;
            }
        }

        public static Color shine(Color newColor, int type)
        {
            int num = newColor.R;
            int num2 = newColor.R;
            int num3 = newColor.R;
            float num4 = 0.6f;
            if (type == 25)
            {
                num = (int) (newColor.R*0.95f);
                num2 = (int) (newColor.G*0.85f);
                num3 = (int) ((newColor.B)*1.1);
            }
            else
            {
                if (type == 117)
                {
                    num = (int) (newColor.R*1.1f);
                    num2 = (int) (newColor.G*1f);
                    num3 = (int) ((newColor.B)*1.2);
                }
                else
                {
                    num = (int) (newColor.R*(1f + num4));
                    num2 = (int) (newColor.G*(1f + num4));
                    num3 = (int) (newColor.B*(1f + num4));
                }
            }
            if (num > 255)
            {
                num = 255;
            }
            if (num2 > 255)
            {
                num2 = 255;
            }
            if (num3 > 255)
            {
                num3 = 255;
            }
            newColor.R = (byte) num;
            newColor.G = (byte) num2;
            newColor.B = (byte) num3;
            return new Color(((byte) num), ((byte) num2), ((byte) num3), newColor.A);
        }

        private static Color buffColor(Color newColor, float R, float G, float B, float A)
        {
            newColor.R = (byte) (newColor.R*R);
            newColor.G = (byte) (newColor.G*G);
            newColor.B = (byte) (newColor.B*B);
            newColor.A = (byte) (newColor.A*A);
            return newColor;
        }

        private static void HelpText()
        {
            bool flag = false;
            if (player[myPlayer].statLifeMax > 100)
            {
                flag = true;
            }
            bool flag2 = false;
            if (player[myPlayer].statManaMax > 0)
            {
                flag2 = true;
            }
            bool flag3 = true;
            bool flag4 = false;
            bool flag5 = false;
            bool flag6 = false;
            bool flag7 = false;
            bool flag8 = false;
            bool flag9 = false;
            for (int i = 0; i < 48; i++)
            {
                if (player[myPlayer].inventory[i].pick > 0 && player[myPlayer].inventory[i].name != "Copper Pickaxe")
                {
                    flag3 = false;
                }
                if (player[myPlayer].inventory[i].axe > 0 && player[myPlayer].inventory[i].name != "Copper Axe")
                {
                    flag3 = false;
                }
                if (player[myPlayer].inventory[i].hammer > 0)
                {
                    flag3 = false;
                }
                if (player[myPlayer].inventory[i].type == 11 || player[myPlayer].inventory[i].type == 12 || player[myPlayer].inventory[i].type == 13 || player[myPlayer].inventory[i].type == 14)
                {
                    flag4 = true;
                }
                if (player[myPlayer].inventory[i].type == 19 || player[myPlayer].inventory[i].type == 20 || player[myPlayer].inventory[i].type == 21 || player[myPlayer].inventory[i].type == 22)
                {
                    flag5 = true;
                }
                if (player[myPlayer].inventory[i].type == 75)
                {
                    flag6 = true;
                }
                if (player[myPlayer].inventory[i].type == 75)
                {
                    flag7 = true;
                }
                if (player[myPlayer].inventory[i].type == 68 || player[myPlayer].inventory[i].type == 70)
                {
                    flag8 = true;
                }
                if (player[myPlayer].inventory[i].type == 84)
                {
                    flag9 = true;
                }
            }
            bool flag10 = false;
            bool flag11 = false;
            bool flag12 = false;
            bool flag13 = false;
            bool flag14 = false;
            bool flag15 = false;
            bool flag16 = false;
            bool flag17 = false;
            bool flag18 = false;
            for (int j = 0; j < 200; j++)
            {
                if (npc[j].active)
                {
                    if (npc[j].type == 17)
                    {
                        flag10 = true;
                    }
                    if (npc[j].type == 18)
                    {
                        flag11 = true;
                    }
                    if (npc[j].type == 19)
                    {
                        flag13 = true;
                    }
                    if (npc[j].type == 20)
                    {
                        flag12 = true;
                    }
                    if (npc[j].type == 54)
                    {
                        flag18 = true;
                    }
                    if (npc[j].type == 124)
                    {
                        flag15 = true;
                    }
                    if (npc[j].type == 107)
                    {
                        flag14 = true;
                    }
                    if (npc[j].type == 108)
                    {
                        flag16 = true;
                    }
                    if (npc[j].type == 38)
                    {
                        flag17 = true;
                    }
                }
            }
            while (true)
            {
                helpText++;
                if (flag3)
                {
                    if (helpText == 1)
                    {
                        break;
                    }
                    if (helpText == 2)
                    {
                        goto Block_31;
                    }
                    if (helpText == 3)
                    {
                        goto Block_32;
                    }
                    if (helpText == 4)
                    {
                        goto Block_33;
                    }
                    if (helpText == 5)
                    {
                        goto Block_34;
                    }
                    if (helpText == 6)
                    {
                        goto Block_35;
                    }
                }
                if (flag3 && !flag4 && !flag5 && helpText == 11)
                {
                    goto Block_39;
                }
                if (flag3 && flag4 && !flag5)
                {
                    if (helpText == 21)
                    {
                        goto Block_43;
                    }
                    if (helpText == 22)
                    {
                        goto Block_44;
                    }
                }
                if (flag3 && flag5)
                {
                    if (helpText == 31)
                    {
                        goto Block_47;
                    }
                    if (helpText == 32)
                    {
                        goto Block_48;
                    }
                }
                if (!flag && helpText == 41)
                {
                    goto Block_50;
                }
                if (!flag2 && helpText == 42)
                {
                    goto Block_52;
                }
                if (!flag2 && !flag6 && helpText == 43)
                {
                    goto Block_55;
                }
                if (!flag10 && !flag11)
                {
                    if (helpText == 51)
                    {
                        goto Block_58;
                    }
                    if (helpText == 52)
                    {
                        goto Block_59;
                    }
                    if (helpText == 53)
                    {
                        goto Block_60;
                    }
                    if (helpText == 54)
                    {
                        goto Block_61;
                    }
                }
                if (!flag10 && helpText == 61)
                {
                    goto Block_63;
                }
                if (!flag11 && helpText == 62)
                {
                    goto Block_65;
                }
                if (!flag13 && helpText == 63)
                {
                    goto Block_67;
                }
                if (!flag12 && helpText == 64)
                {
                    goto Block_69;
                }
                if (!flag15 && helpText == 65 && NPC.downedBoss3)
                {
                    goto Block_72;
                }
                if (!flag18 && helpText == 66 && NPC.downedBoss3)
                {
                    goto Block_75;
                }
                if (!flag14 && helpText == 67)
                {
                    goto Block_77;
                }
                if (!flag17 && NPC.downedBoss2 && helpText == 68)
                {
                    goto Block_80;
                }
                if (!flag16 && hardMode && helpText == 69)
                {
                    goto Block_83;
                }
                if (flag7 && helpText == 71)
                {
                    goto Block_85;
                }
                if (flag8 && helpText == 72)
                {
                    goto Block_87;
                }
                if ((flag7 || flag8) && helpText == 80)
                {
                    goto Block_89;
                }
                if (!flag9 && helpText == 201 && !hardMode && !NPC.downedBoss3 && !NPC.downedBoss2)
                {
                    goto Block_94;
                }
                if (helpText == 1000 && !NPC.downedBoss1 && !NPC.downedBoss2)
                {
                    goto Block_97;
                }
                if (helpText == 1001 && !NPC.downedBoss1 && !NPC.downedBoss2)
                {
                    goto Block_100;
                }
                if (helpText == 1002 && !NPC.downedBoss3)
                {
                    goto Block_102;
                }
                if (helpText == 1050 && !NPC.downedBoss1 && player[myPlayer].statLifeMax < 200)
                {
                    goto Block_105;
                }
                if (helpText == 1051 && !NPC.downedBoss1 && player[myPlayer].statDefense <= 10)
                {
                    goto Block_108;
                }
                if (helpText == 1052 && !NPC.downedBoss1 && player[myPlayer].statLifeMax >= 200 && player[myPlayer].statDefense > 10)
                {
                    goto Block_112;
                }
                if (helpText == 1053 && NPC.downedBoss1 && !NPC.downedBoss2 && player[myPlayer].statLifeMax < 300)
                {
                    goto Block_116;
                }
                if (helpText == 1054 && NPC.downedBoss1 && !NPC.downedBoss2 && player[myPlayer].statLifeMax >= 300)
                {
                    goto Block_120;
                }
                if (helpText == 1055 && NPC.downedBoss1 && !NPC.downedBoss2 && player[myPlayer].statLifeMax >= 300)
                {
                    goto Block_124;
                }
                if (helpText == 1056 && NPC.downedBoss1 && NPC.downedBoss2 && !NPC.downedBoss3)
                {
                    goto Block_128;
                }
                if (helpText == 1057 && NPC.downedBoss1 && NPC.downedBoss2 && NPC.downedBoss3 && !hardMode && player[myPlayer].statLifeMax < 400)
                {
                    goto Block_134;
                }
                if (helpText == 1058 && NPC.downedBoss1 && NPC.downedBoss2 && NPC.downedBoss3 && !hardMode && player[myPlayer].statLifeMax >= 400)
                {
                    goto Block_140;
                }
                if (helpText == 1059 && NPC.downedBoss1 && NPC.downedBoss2 && NPC.downedBoss3 && !hardMode && player[myPlayer].statLifeMax >= 400)
                {
                    goto Block_146;
                }
                if (helpText == 1060 && NPC.downedBoss1 && NPC.downedBoss2 && NPC.downedBoss3 && !hardMode && player[myPlayer].statLifeMax >= 400)
                {
                    goto Block_152;
                }
                if (helpText == 1061 && hardMode)
                {
                    goto Block_154;
                }
                if (helpText == 1062 && hardMode)
                {
                    goto Block_156;
                }
                if (helpText > 1100)
                {
                    helpText = 0;
                }
            }
            npcChatText = "You can use your pickaxe to dig through dirt, and your axe to chop down trees. Just place your cursor over the tile and click!";
            return;
            Block_31:
            npcChatText = "If you want to survive, you will need to create weapons and shelter. Start by chopping down trees and gathering wood.";
            return;
            Block_32:
            npcChatText = "Press ESC to access your crafting menu. When you have enough wood, create a workbench. This will allow you to create more complicated things, as long as you are standing close to it.";
            return;
            Block_33:
            npcChatText = "You can build a shelter by placing wood or other blocks in the world. Don't forget to create and place walls.";
            return;
            Block_34:
            npcChatText = "Once you have a wooden sword, you might try to gather some gel from the slimes. Combine wood and gel to make a torch!";
            return;
            Block_35:
            npcChatText = "To interact with backgrounds and placed objects, use a hammer!";
            return;
            Block_39:
            npcChatText = "You should do some mining to find metal ore. You can craft very useful things with it.";
            return;
            Block_43:
            npcChatText = "Now that you have some ore, you will need to turn it into a bar in order to make items with it. This requires a furnace!";
            return;
            Block_44:
            npcChatText = "You can create a furnace out of torches, wood, and stone. Make sure you are standing near a work bench.";
            return;
            Block_47:
            npcChatText = "You will need an anvil to make most things out of metal bars.";
            return;
            Block_48:
            npcChatText = "Anvils can be crafted out of iron, or purchased from a merchant.";
            return;
            Block_50:
            npcChatText = "Underground are crystal hearts that can be used to increase your max life. You will need a hammer to obtain them.";
            return;
            Block_52:
            npcChatText = "If you gather 10 fallen stars, they can be combined to create an item that will increase your magic capacity.";
            return;
            Block_55:
            npcChatText = "Stars fall all over the world at night. They can be used for all sorts of usefull things. If you see one, be sure to grab it because they disappear after sunrise.";
            return;
            Block_58:
            npcChatText = "There are many different ways you can attract people to move in to our town. They will of course need a home to live in.";
            return;
            Block_59:
            npcChatText = "In order for a room to be considered a home, it needs to have a door, chair, table, and a light source.  Make sure the house has walls as well.";
            return;
            Block_60:
            npcChatText = "Two people will not live in the same home. Also, if their home is destroyed, they will look for a new place to live.";
            return;
            Block_61:
            npcChatText = "You can use the housing interface to assign and view housing. Open you inventory and click the house icon.";
            return;
            Block_63:
            npcChatText = "If you want a merchant to move in, you will need to gather plenty of money. 50 silver coins should do the trick!";
            return;
            Block_65:
            npcChatText = "For a nurse to move in, you might want to increase your maximum life.";
            return;
            Block_67:
            npcChatText = "If you had a gun, I bet an arms dealer might show up to sell you some ammo!";
            return;
            Block_69:
            npcChatText = "You should prove yourself by defeating a strong monster. That will get the attention of a dryad.";
            return;
            Block_72:
            npcChatText = "Make sure to explore the dungeon thoroughly. There may be prisoners held deep within.";
            return;
            Block_75:
            npcChatText = "Perhaps the old man by the dungeon would like to join us now that his curse has been lifted.";
            return;
            Block_77:
            npcChatText = "Hang on to any bombs you might find. A demolitionist may want to have a look at them.";
            return;
            Block_80:
            npcChatText = "Are goblins really so different from us that we couldn't live together peacefully?";
            return;
            Block_83:
            npcChatText = "I heard there was a powerfully wizard who lives in these parts.  Make sure to keep an eye out for him next time you go underground.";
            return;
            Block_85:
            npcChatText = "If you combine lenses at a demon altar, you might be able to find a way to summon a powerful monster. You will want to wait until night before using it, though.";
            return;
            Block_87:
            npcChatText = "You can create worm bait with rotten chunks and vile powder. Make sure you are in a corrupt area before using it.";
            return;
            Block_89:
            npcChatText = "Demonic altars can usually be found in the corruption. You will need to be near them to craft some items.";
            return;
            Block_94:
            npcChatText = "You can make a grappling hook from a hook and 3 chains. Skeletons found deep underground usually carry hooks, and chains can be made from iron bars.";
            return;
            Block_97:
            npcChatText = "If you see a pot, be sure to smash it open. They contain all sorts of useful supplies.";
            return;
            Block_100:
            npcChatText = "There is treasure hidden all over the world. Some amazing things can be found deep underground!";
            return;
            Block_102:
            npcChatText = "Smashing a shadow orb will sometimes cause a meteor to fall out of the sky. Shadow orbs can usually be found in the chasms around corrupt areas.";
            return;
            Block_105:
            npcChatText = "You should focus on gathering more heart crystals to increase your maximum life.";
            return;
            Block_108:
            npcChatText = "Your current equipment simply won't do. You need to make better armor.";
            return;
            Block_112:
            npcChatText = "I think you are ready for your first major battle. Gather some lenses from the eyeballs at night and take them to a demon altar.";
            return;
            Block_116:
            npcChatText = "You wil want to increase your life before facing your next challenge. Fifteen hearts should be enough.";
            return;
            Block_120:
            npcChatText = "The ebonstone in the corruption can be purified using some powder from a dryad, or it can be destroyed with explosives.";
            return;
            Block_124:
            npcChatText = "Your next step should be to explore the corrupt chasms.  Find and destroy any shadow orb you find.";
            return;
            Block_128:
            npcChatText = "There is a old dungeon not far from here. Now would be a good time to go check it out.";
            return;
            Block_134:
            npcChatText = "You should make an attempt to max out your available life. Try to gather twenty hearts.";
            return;
            Block_140:
            npcChatText = "There are many treasures to be discovered in the jungle, if you are willing to dig deep enough.";
            return;
            Block_146:
            npcChatText = "The underworld is made of a material called hellstone. It's perfect for making weapons and armor.";
            return;
            Block_152:
            npcChatText = "When you are ready to challenge the keeper of the underworld, you will have to make a living sacrifice. Everything you need for it can be found in the underworld.";
            return;
            Block_154:
            npcChatText = "Make sure to smash any demon altar you can find. Something good is bound to happen if you do!";
            return;
            Block_156:
            npcChatText = "Souls can sometimes be gathered from fallen creatures in places of extreme light or dark.";
        }

        private static bool AccCheck(Item newItem, int slot)
        {
            if (player[myPlayer].armor[slot].IsTheSameAs(newItem))
            {
                return false;
            }
            for (int i = 0; i < player[myPlayer].armor.Length; i++)
            {
                if (newItem.IsTheSameAs(player[myPlayer].armor[i]))
                {
                    return true;
                }
            }
            return false;
        }

        public static Item armorSwap(Item newItem)
        {
            for (int i = 0; i < player[myPlayer].armor.Length; i++)
            {
                if (newItem.IsTheSameAs(player[myPlayer].armor[i]))
                {
                    accSlotCount = i;
                }
            }
            if (newItem.headSlot == -1 && newItem.bodySlot == -1 && newItem.legSlot == -1 && !newItem.accessory)
            {
                return newItem;
            }
            Item result = newItem;
            if (newItem.headSlot != -1)
            {
                result = (Item) player[myPlayer].armor[0].Clone();
                player[myPlayer].armor[0] = (Item) newItem.Clone();
            }
            else
            {
                if (newItem.bodySlot != -1)
                {
                    result = (Item) player[myPlayer].armor[1].Clone();
                    player[myPlayer].armor[1] = (Item) newItem.Clone();
                }
                else
                {
                    if (newItem.legSlot != -1)
                    {
                        result = (Item) player[myPlayer].armor[2].Clone();
                        player[myPlayer].armor[2] = (Item) newItem.Clone();
                    }
                    else
                    {
                        if (newItem.accessory)
                        {
                            for (int j = 3; j < 8; j++)
                            {
                                if (player[myPlayer].armor[j].type == 0)
                                {
                                    accSlotCount = j - 3;
                                    break;
                                }
                            }
                            for (int k = 0; k < player[myPlayer].armor.Length; k++)
                            {
                                if (newItem.IsTheSameAs(player[myPlayer].armor[k]))
                                {
                                    accSlotCount = k - 3;
                                }
                            }
                            if (accSlotCount >= 5)
                            {
                                accSlotCount = 0;
                            }
                            if (accSlotCount < 0)
                            {
                                accSlotCount = 4;
                            }
                            result = (Item) player[myPlayer].armor[3 + accSlotCount].Clone();
                            player[myPlayer].armor[3 + accSlotCount] = (Item) newItem.Clone();
                            accSlotCount++;
                            if (accSlotCount >= 5)
                            {
                                accSlotCount = 0;
                            }
                        }
                    }
                }
            }
            PlaySound(7, -1, -1, 1);
            Recipe.FindRecipes();
            return result;
        }

        public static void BankCoins()
        {
            for (int i = 0; i < 20; i++)
            {
                if (player[myPlayer].bank[i].type >= 71 && player[myPlayer].bank[i].type <= 73 && player[myPlayer].bank[i].stack == player[myPlayer].bank[i].maxStack)
                {
                    player[myPlayer].bank[i].SetDefaults(player[myPlayer].bank[i].type + 1, false);
                    for (int j = 0; j < 20; j++)
                    {
                        if (j != i && player[myPlayer].bank[j].type == player[myPlayer].bank[i].type && player[myPlayer].bank[j].stack < player[myPlayer].bank[j].maxStack)
                        {
                            player[myPlayer].bank[j].stack++;
                            player[myPlayer].bank[i].SetDefaults(0, false);
                            BankCoins();
                        }
                    }
                }
            }
        }

        public static void ChestCoins()
        {
            for (int i = 0; i < 20; i++)
            {
                if (chest[player[myPlayer].chest].item[i].type >= 71 && chest[player[myPlayer].chest].item[i].type <= 73 && chest[player[myPlayer].chest].item[i].stack == chest[player[myPlayer].chest].item[i].maxStack)
                {
                    chest[player[myPlayer].chest].item[i].SetDefaults(chest[player[myPlayer].chest].item[i].type + 1, false);
                    for (int j = 0; j < 20; j++)
                    {
                        if (j != i && chest[player[myPlayer].chest].item[j].type == chest[player[myPlayer].chest].item[i].type && chest[player[myPlayer].chest].item[j].stack < chest[player[myPlayer].chest].item[j].maxStack)
                        {
                            if (netMode == 1)
                            {
                                NetMessage.SendData(32, -1, -1, "", player[myPlayer].chest, j, 0f, 0f, 0);
                            }
                            chest[player[myPlayer].chest].item[j].stack++;
                            chest[player[myPlayer].chest].item[i].SetDefaults(0, false);
                            ChestCoins();
                        }
                    }
                }
            }
        }

        protected Color randColor()
        {
            int num = 0;
            int num2 = 0;
            int num3 = 0;
            while (num + num3 + num2 <= 150)
            {
                num = rand.Next(256);
                num2 = rand.Next(256);
                num3 = rand.Next(256);
            }
            return new Color(num, num2, num3, 255);
        }

        public static void CursorColor()
        {
            cursorAlpha += cursorColorDirection*0.015f;
            if (cursorAlpha >= 1f)
            {
                cursorAlpha = 1f;
                cursorColorDirection = -1;
            }
            if (cursorAlpha <= 0.6)
            {
                cursorAlpha = 0.6f;
                cursorColorDirection = 1;
            }
            float num = cursorAlpha*0.3f + 0.7f;
            var r = (byte) (mouseColor.R*cursorAlpha);
            var g = (byte) (mouseColor.G*cursorAlpha);
            var b = (byte) (mouseColor.B*cursorAlpha);
            var a = (byte) (255f*num);
            cursorColor = new Color(r, g, b, a);
            cursorScale = cursorAlpha*0.3f + 0.7f + 0.1f;
        }

        protected bool FullTile(int x, int y)
        {
            if (tile[x, y].active && tileSolid[tile[x, y].type] && !tileSolidTop[tile[x, y].type] && tile[x, y].type != 10 && tile[x, y].type != 54 && tile[x, y].type != 138)
            {
                int frameX = tile[x, y].frameX;
                int frameY = tile[x, y].frameY;
                if (frameY == 18)
                {
                    if (frameX >= 18 && frameX <= 54)
                    {
                        return true;
                    }
                    if (frameX >= 108 && frameX <= 144)
                    {
                        return true;
                    }
                }
                else
                {
                    if (frameY >= 90 && frameY <= 196)
                    {
                        if (frameX <= 70)
                        {
                            return true;
                        }
                        if (frameX >= 144 && frameX <= 232)
                        {
                            return true;
                        }
                    }
                }
            }
            return false;
        }

        private static void UpdateInvasion()
        {
            if (invasionType > 0)
            {
                if (invasionSize <= 0)
                {
                    if (invasionType == 1)
                    {
                        NPC.downedGoblins = true;
                        if (netMode == 2)
                        {
                            NetMessage.SendData(7, -1, -1, "", 0, 0f, 0f, 0f, 0);
                        }
                    }
                    else
                    {
                        if (invasionType == 2)
                        {
                            NPC.downedFrost = true;
                        }
                    }
                    InvasionWarning();
                    invasionType = 0;
                    invasionDelay = 7;
                }
                if (invasionX == spawnTileX)
                {
                    return;
                }
                float num = 1f;
                if (invasionX > spawnTileX)
                {
                    invasionX -= num;
                    if (invasionX <= spawnTileX)
                    {
                        invasionX = spawnTileX;
                        InvasionWarning();
                    }
                    else
                    {
                        invasionWarn--;
                    }
                }
                else
                {
                    if (invasionX < spawnTileX)
                    {
                        invasionX += num;
                        if (invasionX >= spawnTileX)
                        {
                            invasionX = spawnTileX;
                            InvasionWarning();
                        }
                        else
                        {
                            invasionWarn--;
                        }
                    }
                }
                if (invasionWarn <= 0)
                {
                    invasionWarn = 3600;
                    InvasionWarning();
                }
            }
        }

        private static void InvasionWarning()
        {
            string str = "The goblin army";
            if (invasionType == 2)
            {
                str = "The Frost Legion";
            }
            string text = "";
            if (invasionSize <= 0)
            {
                text = str + " has been defeated!";
            }
            else
            {
                if (invasionX < spawnTileX)
                {
                    text = str + " is approaching from the west!";
                }
                else
                {
                    if (invasionX > spawnTileX)
                    {
                        text = str + " is approaching from the east!";
                    }
                    else
                    {
                        text = str + " has arrived!";
                    }
                }
            }
            if (netMode == 0)
            {
                NewText(text, 175, 75, 255);
                return;
            }
            if (netMode == 2)
            {
                NetMessage.SendData(25, -1, -1, text, 255, 175f, 75f, 255f, 0);
            }
        }

        public static void StartInvasion(int type = 1)
        {
            if (invasionType == 0 && invasionDelay == 0)
            {
                int num = 0;
                for (int i = 0; i < 255; i++)
                {
                    if (player[i].active && player[i].statLifeMax >= 200)
                    {
                        num++;
                    }
                }
                if (num > 0)
                {
                    invasionType = type;
                    invasionSize = 80 + 40*num;
                    invasionWarn = 0;
                    if (rand.Next(2) == 0)
                    {
                        invasionX = 0.0;
                        return;
                    }
                    invasionX = maxTilesX;
                }
            }
        }

        private static void UpdateServer()
        {
            netPlayCounter++;
            if (netPlayCounter > 3600)
            {
                NetMessage.SendData(7, -1, -1, "", 0, 0f, 0f, 0f, 0);
                NetMessage.syncPlayers();
                netPlayCounter = 0;
            }
            for (int i = 0; i < maxNetPlayers; i++)
            {
                if (player[i].active && Netplay.serverSock[i].active)
                {
                    Netplay.serverSock[i].SpamUpdate();
                }
            }
            if (Math.IEEERemainder(netPlayCounter, 900.0) == 0.0)
            {
                bool flag = true;
                int num = lastItemUpdate;
                int num2 = 0;
                while (flag)
                {
                    num++;
                    if (num >= 200)
                    {
                        num = 0;
                    }
                    num2++;
                    if (!item[num].active || item[num].owner == 255)
                    {
                        NetMessage.SendData(21, -1, -1, "", num, 0f, 0f, 0f, 0);
                    }
                    if (num2 >= maxItemUpdates || num == lastItemUpdate)
                    {
                        flag = false;
                    }
                }
                lastItemUpdate = num;
            }
            for (int j = 0; j < 200; j++)
            {
                if (item[j].active && (item[j].owner == 255 || !player[item[j].owner].active))
                {
                    item[j].FindOwner(j);
                }
            }
            for (int k = 0; k < 255; k++)
            {
                if (Netplay.serverSock[k].active)
                {
                    Netplay.serverSock[k].timeOut++;
                    if (!stopTimeOuts && Netplay.serverSock[k].timeOut > 60*timeOut)
                    {
                        Netplay.serverSock[k].kill = true;
                    }
                }
                if (player[k].active)
                {
                    int sectionX = Netplay.GetSectionX((int) (player[k].position.X/16f));
                    int sectionY = Netplay.GetSectionY((int) (player[k].position.Y/16f));
                    int num3 = 0;
                    for (int l = sectionX - 1; l < sectionX + 2; l++)
                    {
                        for (int m = sectionY - 1; m < sectionY + 2; m++)
                        {
                            if (l >= 0 && l < maxSectionsX && m >= 0 && m < maxSectionsY && !Netplay.serverSock[k].tileSection[l, m])
                            {
                                num3++;
                            }
                        }
                    }
                    if (num3 > 0)
                    {
                        int num4 = num3*150;
                        NetMessage.SendData(9, k, -1, "Receiving tile data", num4, 0f, 0f, 0f, 0);
                        Netplay.serverSock[k].statusText2 = "is receiving tile data";
                        Netplay.serverSock[k].statusMax += num4;
                        for (int n = sectionX - 1; n < sectionX + 2; n++)
                        {
                            for (int num5 = sectionY - 1; num5 < sectionY + 2; num5++)
                            {
                                if (n >= 0 && n < maxSectionsX && num5 >= 0 && num5 < maxSectionsY && !Netplay.serverSock[k].tileSection[n, num5])
                                {
                                    NetMessage.SendSection(k, n, num5);
                                    NetMessage.SendData(11, k, -1, "", n, num5, n, num5, 0);
                                }
                            }
                        }
                    }
                }
            }
        }

        public static void NewText(string newText, byte R = 255, byte G = 255, byte B = 255)
        {
            for (int i = numChatLines - 1; i > 0; i--)
            {
                chatLine[i].text = chatLine[i - 1].text;
                chatLine[i].showTime = chatLine[i - 1].showTime;
                chatLine[i].color = chatLine[i - 1].color;
            }
            if (R == 0 && G == 0 && B == 0)
            {
                chatLine[0].color = Color.White;
            }
            else
            {
                chatLine[0].color = new Color(R, G, B);
            }
            chatLine[0].text = newText;
            chatLine[0].showTime = chatLength;
            PlaySound(12, -1, -1, 1);
        }

        private static void UpdateTime()
        {
            time += dayRate;
            if (!dayTime)
            {
                if (WorldGen.spawnEye && netMode != 1 && time > 4860.0)
                {
                    for (int i = 0; i < 255; i++)
                    {
                        if (player[i].active && !player[i].dead && player[i].position.Y < worldSurface*16.0)
                        {
                            NPC.SpawnOnPlayer(i, 4);
                            WorldGen.spawnEye = false;
                            break;
                        }
                    }
                }
                if (time > 32400.0)
                {
                    checkXMas();
                    if (invasionDelay > 0)
                    {
                        invasionDelay--;
                    }
                    WorldGen.spawnNPC = 0;
                    checkForSpawns = 0;
                    time = 0.0;
                    bloodMoon = false;
                    dayTime = true;
                    moonPhase++;
                    if (moonPhase >= 8)
                    {
                        moonPhase = 0;
                    }
                    if (netMode == 2)
                    {
                        NetMessage.SendData(7, -1, -1, "", 0, 0f, 0f, 0f, 0);
                        WorldGen.saveAndPlay();
                    }
                    if (netMode != 1 && WorldGen.shadowOrbSmashed)
                    {
                        if (!NPC.downedGoblins)
                        {
                            if (rand.Next(3) == 0)
                            {
                                StartInvasion(1);
                            }
                        }
                        else
                        {
                            if (rand.Next(15) == 0)
                            {
                                StartInvasion(1);
                            }
                        }
                    }
                }
                if (time > 16200.0 && WorldGen.spawnMeteor)
                {
                    WorldGen.spawnMeteor = false;
                    WorldGen.dropMeteor();
                    return;
                }
            }
            else
            {
                bloodMoon = false;
                if (time > 54000.0)
                {
                    WorldGen.spawnNPC = 0;
                    checkForSpawns = 0;
                    if (rand.Next(50) == 0 && netMode != 1 && WorldGen.shadowOrbSmashed)
                    {
                        WorldGen.spawnMeteor = true;
                    }
                    if (!NPC.downedBoss1 && netMode != 1)
                    {
                        bool flag = false;
                        for (int j = 0; j < 255; j++)
                        {
                            if (player[j].active && player[j].statLifeMax >= 200 && player[j].statDefense > 10)
                            {
                                flag = true;
                                break;
                            }
                        }
                        if (flag && rand.Next(3) == 0)
                        {
                            int num = 0;
                            for (int k = 0; k < 200; k++)
                            {
                                if (npc[k].active && npc[k].townNPC)
                                {
                                    num++;
                                }
                            }
                            if (num >= 4)
                            {
                                WorldGen.spawnEye = true;
                                if (netMode == 0)
                                {
                                    NewText("You feel an evil presence watching you...", 50, 255, 130);
                                }
                                else
                                {
                                    if (netMode == 2)
                                    {
                                        NetMessage.SendData(25, -1, -1, "You feel an evil presence watching you...", 255, 50f, 255f, 130f, 0);
                                    }
                                }
                            }
                        }
                    }
                    if (!WorldGen.spawnEye && moonPhase != 4 && rand.Next(9) == 0 && netMode != 1)
                    {
                        for (int l = 0; l < 255; l++)
                        {
                            if (player[l].active && player[l].statLifeMax > 120)
                            {
                                bloodMoon = true;
                                break;
                            }
                        }
                        if (bloodMoon)
                        {
                            if (netMode == 0)
                            {
                                NewText("The Blood Moon is rising...", 50, 255, 130);
                            }
                            else
                            {
                                if (netMode == 2)
                                {
                                    NetMessage.SendData(25, -1, -1, "The Blood Moon is rising...", 255, 50f, 255f, 130f, 0);
                                }
                            }
                        }
                    }
                    time = 0.0;
                    dayTime = false;
                    if (netMode == 2)
                    {
                        NetMessage.SendData(7, -1, -1, "", 0, 0f, 0f, 0f, 0);
                    }
                }
                if (netMode != 1)
                {
                    checkForSpawns++;
                    if (checkForSpawns >= 7200)
                    {
                        int num2 = 0;
                        for (int m = 0; m < 255; m++)
                        {
                            if (player[m].active)
                            {
                                num2++;
                            }
                        }
                        checkForSpawns = 0;
                        WorldGen.spawnNPC = 0;
                        int num3 = 0;
                        int num4 = 0;
                        int num5 = 0;
                        int num6 = 0;
                        int num7 = 0;
                        int num8 = 0;
                        int num9 = 0;
                        int num10 = 0;
                        int num11 = 0;
                        int num12 = 0;
                        int num13 = 0;
                        int num14 = 0;
                        int num15 = 0;
                        for (int n = 0; n < 200; n++)
                        {
                            if (npc[n].active && npc[n].townNPC)
                            {
                                if (npc[n].type != 37 && !npc[n].homeless)
                                {
                                    WorldGen.QuickFindHome(n);
                                }
                                if (npc[n].type == 37)
                                {
                                    num8++;
                                }
                                if (npc[n].type == 17)
                                {
                                    num3++;
                                }
                                if (npc[n].type == 18)
                                {
                                    num4++;
                                }
                                if (npc[n].type == 19)
                                {
                                    num6++;
                                }
                                if (npc[n].type == 20)
                                {
                                    num5++;
                                }
                                if (npc[n].type == 22)
                                {
                                    num7++;
                                }
                                if (npc[n].type == 38)
                                {
                                    num9++;
                                }
                                if (npc[n].type == 54)
                                {
                                    num10++;
                                }
                                if (npc[n].type == 107)
                                {
                                    num12++;
                                }
                                if (npc[n].type == 108)
                                {
                                    num11++;
                                }
                                if (npc[n].type == 124)
                                {
                                    num13++;
                                }
                                if (npc[n].type == 142)
                                {
                                    num14++;
                                }
                                num15++;
                            }
                        }
                        if (WorldGen.spawnNPC == 0)
                        {
                            int num16 = 0;
                            bool flag2 = false;
                            int num17 = 0;
                            bool flag3 = false;
                            bool flag4 = false;
                            for (int num18 = 0; num18 < 255; num18++)
                            {
                                if (player[num18].active)
                                {
                                    for (int num19 = 0; num19 < 48; num19++)
                                    {
                                        if (player[num18].inventory[num19] != null & player[num18].inventory[num19].stack > 0)
                                        {
                                            if (player[num18].inventory[num19].type == 71)
                                            {
                                                num16 += player[num18].inventory[num19].stack;
                                            }
                                            if (player[num18].inventory[num19].type == 72)
                                            {
                                                num16 += player[num18].inventory[num19].stack*100;
                                            }
                                            if (player[num18].inventory[num19].type == 73)
                                            {
                                                num16 += player[num18].inventory[num19].stack*10000;
                                            }
                                            if (player[num18].inventory[num19].type == 74)
                                            {
                                                num16 += player[num18].inventory[num19].stack*1000000;
                                            }
                                            if (player[num18].inventory[num19].ammo == 14 || player[num18].inventory[num19].useAmmo == 14)
                                            {
                                                flag3 = true;
                                            }
                                            if (player[num18].inventory[num19].type == 166 || player[num18].inventory[num19].type == 167 || player[num18].inventory[num19].type == 168 || player[num18].inventory[num19].type == 235)
                                            {
                                                flag4 = true;
                                            }
                                        }
                                    }
                                    int num20 = player[num18].statLifeMax/20;
                                    if (num20 > 5)
                                    {
                                        flag2 = true;
                                    }
                                    num17 += num20;
                                }
                            }
                            if (!NPC.downedBoss3 && num8 == 0)
                            {
                                int num21 = NPC.NewNPC(dungeonX*16 + 8, dungeonY*16, 37, 0);
                                npc[num21].homeless = false;
                                npc[num21].homeTileX = dungeonX;
                                npc[num21].homeTileY = dungeonY;
                            }
                            if (WorldGen.spawnNPC == 0 && num7 < 1)
                            {
                                WorldGen.spawnNPC = 22;
                            }
                            if (WorldGen.spawnNPC == 0 && num16 > 5000.0 && num3 < 1)
                            {
                                WorldGen.spawnNPC = 17;
                            }
                            if (WorldGen.spawnNPC == 0 && flag2 && num4 < 1)
                            {
                                WorldGen.spawnNPC = 18;
                            }
                            if (WorldGen.spawnNPC == 0 && flag3 && num6 < 1)
                            {
                                WorldGen.spawnNPC = 19;
                            }
                            if (WorldGen.spawnNPC == 0 && (NPC.downedBoss1 || NPC.downedBoss2 || NPC.downedBoss3) && num5 < 1)
                            {
                                WorldGen.spawnNPC = 20;
                            }
                            if (WorldGen.spawnNPC == 0 && flag4 && num3 > 0 && num9 < 1)
                            {
                                WorldGen.spawnNPC = 38;
                            }
                            if (WorldGen.spawnNPC == 0 && NPC.downedBoss3 && num10 < 1)
                            {
                                WorldGen.spawnNPC = 54;
                            }
                            if (WorldGen.spawnNPC == 0 && NPC.savedGoblin && num12 < 1)
                            {
                                WorldGen.spawnNPC = 107;
                            }
                            if (WorldGen.spawnNPC == 0 && NPC.savedWizard && num11 < 1)
                            {
                                WorldGen.spawnNPC = 108;
                            }
                            if (WorldGen.spawnNPC == 0 && NPC.savedMech && num13 < 1)
                            {
                                WorldGen.spawnNPC = 124;
                            }
                            if (WorldGen.spawnNPC == 0 && NPC.downedFrost && num14 < 1 && xMas)
                            {
                                WorldGen.spawnNPC = 142;
                            }
                        }
                    }
                }
            }
        }

        public static int DamageVar(float dmg)
        {
            float num = dmg*(1f + rand.Next(-15, 16)*0.01f);
            return (int) Math.Round(num);
        }

        public static double CalculateDamage(int Damage, int Defense)
        {
            double num = Damage - Defense*0.5;
            if (num < 1.0)
            {
                num = 1.0;
            }
            return num;
        }

        public static void PlaySound(int type, int x = -1, int y = -1, int Style = 1)
        {
        }
    }
}