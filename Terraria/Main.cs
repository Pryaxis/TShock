namespace Terraria
{
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Audio;
    using Microsoft.Xna.Framework.Graphics;
    using Microsoft.Xna.Framework.Input;
    using System;
    using System.IO;
    using System.Net;
    using System.Runtime.InteropServices;
    using System.Text;

    public class Main : Game
    {
        private static KeyboardState inputText;
        public static int numChatLines = 7;
        public static Texture2D[] armorArmTexture = new Texture2D[10];
        public static Texture2D[] armorBodyTexture = new Texture2D[10];
        public static Texture2D[] armorHeadTexture = new Texture2D[12];
        public static Texture2D[] armorLegTexture = new Texture2D[10];
        public static int[] availableRecipe = new int[Recipe.maxRecipes];
        public static float[] availableRecipeY = new float[Recipe.maxRecipes];
        public static int background = 0;
        public static int[] backgroundHeight = new int[7];
        public static Texture2D[] backgroundTexture = new Texture2D[7];
        public static int[] backgroundWidth = new int[7];
        private static int backSpaceCount = 0;
        private int bgScroll;
        public static Texture2D blackTileTexture;
        public static bool bloodMoon = false;
        public static Texture2D boneArmTexture;
        public static float bottomWorld = 38400f;
        public static Texture2D bubbleTexture;
        public static float caveParrallax = 1f;
        public static string cDown = "S";
        public static Texture2D cdTexture;
        public static Texture2D chain2Texture;
        public static Texture2D chain3Texture;
        public static Texture2D chain4Texture;
        public static Texture2D chain5Texture;
        public static Texture2D chain6Texture;
        public static Texture2D chainTexture;
        public static Texture2D chat2Texture;
        public static Texture2D chatBackTexture;
        public static int chatLength = 600;
        public static ChatLine[] chatLine = new ChatLine[8];
        public static bool chatMode = false;
        public static bool chatRelease = false;
        public static string chatText = "";
        public static Texture2D chatTexture;
        public static int checkForSpawns = 0;
        public static Chest[] chest = new Chest[0x3e8];
        public static string cInv = "Escape";
        public static string cJump = "Space";
        public static string cLeft = "A";
        public static Player clientPlayer = new Player();
        public static Cloud[] cloud = new Cloud[100];
        public static int cloudLimit = 100;
        public static Texture2D[] cloudTexture = new Texture2D[4];
        private int colorDelay;
        public static CombatText[] combatText = new CombatText[100];
        public static string cRight = "D";
        public static string cThrowItem = "Q";
        public static string cUp = "W";
        public int curMusic;
        public static int curRelease = 3;
        public static float cursorAlpha = 0f;
        public static Color cursorColor = Color.White;
        public static int cursorColorDirection = 1;
        public static float cursorScale = 0f;
        public static Texture2D cursorTexture;
        public const double dayLength = 54000.0;
        public static bool dayTime = true;
        public static bool debugMode = false;
        public static string defaultIP = "";
        public static int drawTime = 0;
        public static bool dumbAI = false;
        public static int dungeonTiles;
        public static int dungeonX;
        public static int dungeonY;
        public static Dust[] dust = new Dust[0x7d0];
        public static Texture2D dustTexture;
        public static bool editSign = false;
        public static AudioEngine engine;
        public static int evilTiles;
        private static float exitScale = 0.8f;
        public static int fadeCounter = 0;
        public static Texture2D fadeTexture;
        public static bool fixedTiming = false;
        private int focusColor;
        private int focusMenu = -1;
        public static int focusRecipe;
        public static SpriteFont fontCombatText;
        public static SpriteFont fontDeathText;
        public static SpriteFont fontItemStack;
        public static SpriteFont fontMouseText;
        public static int frameRate = 0;
        public static bool frameRelease = false;
        public static bool gameMenu = true;
        public static string getIP = defaultIP;
        public static bool godMode = false;
        public static Gore[] gore = new Gore[0xc9];
        public static Texture2D[] goreTexture = new Texture2D[0x49];
        public static bool grabSky = false;
        public static bool grabSun = false;
        private GraphicsDeviceManager graphics;
        public static bool hasFocus = true;
        public static Texture2D heartTexture;
        public static int helpText = 0;
        public static bool hideUI = false;
        public static float[] hotbarScale = new float[] { 1f, 0.75f, 0.75f, 0.75f, 0.75f, 0.75f, 0.75f, 0.75f, 0.75f, 0.75f };
        public static bool ignoreErrors = true;
        public static bool inputTextEnter = false;
        public static int invasionDelay = 0;
        public static int invasionSize = 0;
        public static int invasionType = 0;
        public static int invasionWarn = 0;
        public static double invasionX = 0.0;
        public static Texture2D inventoryBackTexture;
        private static float inventoryScale = 0.75f;
        public static Item[] item = new Item[0xc9];
        public static Texture2D[] itemTexture = new Texture2D[0xec];
        public static int jungleTiles;
        public static KeyboardState keyState = Keyboard.GetState();
        public static int lastItemUpdate;
        public static int lastNPCUpdate;
        public static float leftWorld = 0f;
        public static bool lightTiles = false;
        public static Liquid[] liquid = new Liquid[Liquid.resLiquid];
        public static LiquidBuffer[] liquidBuffer = new LiquidBuffer[0x2710];
        public static Texture2D[] liquidTexture = new Texture2D[2];
        public static Player[] loadPlayer = new Player[5];
        public static string[] loadPlayerPath = new string[5];
        public static string[] loadWorld = new string[5];
        public static string[] loadWorldPath = new string[5];
        private float logoRotation;
        private float logoRotationDirection = 1f;
        private float logoRotationSpeed = 1f;
        private float logoScale = 1f;
        private float logoScaleDirection = 1f;
        private float logoScaleSpeed = 1f;
        public static Texture2D logoTexture;
        public static int magmaBGFrame = 0;
        public static int magmaBGFrameCounter = 0;
        public static Texture2D manaTexture;
        public const int maxBackgrounds = 7;
        public const int maxChests = 0x3e8;
        public const int maxClouds = 100;
        public const int maxCloudTypes = 4;
        public const int maxCombatText = 100;
        public const int maxDust = 0x7d0;
        public const int maxGore = 200;
        public const int maxGoreTypes = 0x49;
        public const int maxHair = 0x11;
        public const int maxInventory = 0x2c;
        public const int maxItems = 200;
        public const int maxItemSounds = 0x10;
        public const int maxItemTypes = 0xec;
        public static int maxItemUpdates = 10;
        public const int maxLiquidTypes = 2;
        private static int maxMenuItems = 11;
        public const int maxMusic = 7;
        public const int maxNPCHitSounds = 3;
        public const int maxNPCKilledSounds = 3;
        public const int maxNPCs = 0x3e8;
        public const int maxNPCTypes = 0x2c;
        public static int maxNPCUpdates = 15;
        public const int maxPlayers = 8;
        public const int maxProjectiles = 0x3e8;
        public const int maxProjectileTypes = 0x26;
        public const int maxStars = 130;
        public const int maxStarTypes = 5;
        public const int maxTileSets = 80;
        public const int maxWallTypes = 14;
        private float[] menuItemScale = new float[maxMenuItems];
        public static int menuMode = 0;
        public static bool menuMultiplayer = false;
        public static int meteorTiles;
        private const int MF_BYPOSITION = 0x400;
        public static short moonModY = 0;
        public static int moonPhase = 0;
        public static Texture2D moonTexture;
        public static Color mouseColor = new Color(0xff, 50, 0x5f);
        private static bool mouseExit = false;
        public static Item mouseItem = new Item();
        public static bool mouseLeftRelease = false;
        public static bool mouseRightRelease = false;
        public static MouseState mouseState = Mouse.GetState();
        public static byte mouseTextColor = 0;
        public static int mouseTextColorChange = 1;
        public static Microsoft.Xna.Framework.Audio.Cue[] music = new Microsoft.Xna.Framework.Audio.Cue[7];
        public static float[] musicFade = new float[7];
        public static float musicVolume = 0.75f;
        public static int myPlayer = 0;
        public static int netMode = 0;
        public static int netPlayCounter;
        public int newMusic;
        public static string newWorldName = "";
        public const double nightLength = 32400.0;
        public static NPC[] npc = new NPC[0x3e9];
        public static bool npcChatFocus1 = false;
        public static bool npcChatFocus2 = false;
        public static bool npcChatRelease = false;
        public static string npcChatText = "";
        public static int[] npcFrameCount = new int[] { 
            1, 2, 2, 3, 6, 2, 2, 1, 1, 1, 1, 1, 1, 1, 1, 1, 
            2, 0x10, 14, 0x10, 14, 14, 0x10, 2, 10, 1, 0x10, 0x10, 0x10, 3, 1, 14, 
            3, 1, 3, 1, 1, 0x10, 0x10, 1, 1, 1, 3, 3
         };
        public static int npcShop = 0;
        public static Texture2D[] npcTexture = new Texture2D[0x2c];
        public const int numArmorBody = 10;
        public const int numArmorHead = 12;
        public const int numArmorLegs = 10;
        public static int numAvailableRecipes;

        public static int numClouds = cloudLimit;
        private static int numLoadPlayers = 0;
        private static int numLoadWorlds = 0;
        public static int numStars;
        private static KeyboardState oldInputText;
        public static MouseState oldMouseState = Mouse.GetState();
        public static Player[] player = new Player[9];
        public static Texture2D playerBeltTexture;
        public static Texture2D playerEyesTexture;
        public static Texture2D playerEyeWhitesTexture;
        public static Texture2D[] playerHairTexture = new Texture2D[0x11];
        public static Texture2D playerHands2Texture;
        public static Texture2D playerHandsTexture;
        public static Texture2D playerHeadTexture;
        public static bool playerInventory = false;
        public static Texture2D playerPantsTexture;
        public static string playerPathName;
        public static Texture2D playerShirtTexture;
        public static Texture2D playerShoesTexture;
        public static Texture2D playerUnderShirt2Texture;
        public static Texture2D playerUnderShirtTexture;
        public static Projectile[] projectile = new Projectile[0x3e9];
        public static Texture2D[] projectileTexture = new Texture2D[0x26];
        [ThreadStatic]
        public static Random rand;
        public static Texture2D raTexture;
        public static Recipe[] recipe = new Recipe[Recipe.maxRecipes];
        public static bool releaseUI = false;
        public static bool resetClouds = true;
        public static Texture2D reTexture;
        public static float rightWorld = 134400f;
        public static int maxTilesX = ((((int)rightWorld) / 0x10) + 1);
        public static int maxTilesY = ((((int)bottomWorld) / 0x10) + 1);
        public static int maxSectionsX = (maxTilesX / 200);
        public static int maxSectionsY = (maxTilesY / 150);
        public static double rockLayer;
        public static string SavePath = (Environment.GetFolderPath(Environment.SpecialFolder.Personal) + @"\My Games\Terraria");
        public static string PlayerPath = (SavePath + @"\Players");
        public static int saveTimer = 0;
        public static int screenHeight = 600;
        public static Vector2 screenLastPosition;
        public static Vector2 screenPosition;
        public static int screenWidth = 800;
        public const int sectionHeight = 150;
        public const int sectionWidth = 200;
        private Color selColor = Color.White;
        private int selectedMenu = -1;
        private int selectedPlayer;
        private int selectedWorld;
        private int setKey = -1;
        public Chest[] shop = new Chest[5];
        public static bool showFrameRate = false;
        public static bool showItemOwner = false;
        public static bool showSpam = false;
        public static bool showSplash = false;
        public static Texture2D shroomCapTexture;
        public static Sign[] sign = new Sign[0x3e8];
        public static bool signBubble = false;
        public static string signText = "";
        public static int signX = 0;
        public static int signY = 0;
        public static bool skipMenu = false;
        public static Microsoft.Xna.Framework.Audio.SoundBank soundBank;
        public static SoundEffect soundCoins;
        public static SoundEffect[] soundDig = new SoundEffect[3];
        public static SoundEffect soundDoorClosed;
        public static SoundEffect soundDoorOpen;
        public static SoundEffect soundDoubleJump;
        public static SoundEffect[] soundFemaleHit = new SoundEffect[3];
        public static SoundEffect soundGrab;
        public static SoundEffect soundGrass;
        public static SoundEffectInstance soundInstanceCoins;
        public static SoundEffectInstance[] soundInstanceDig = new SoundEffectInstance[3];
        public static SoundEffectInstance soundInstanceDoorClosed;
        public static SoundEffectInstance soundInstanceDoorOpen;
        public static SoundEffectInstance soundInstanceDoubleJump;
        public static SoundEffectInstance[] soundInstanceFemaleHit = new SoundEffectInstance[3];
        public static SoundEffectInstance soundInstanceGrab;
        public static SoundEffectInstance soundInstanceGrass;
        public static SoundEffectInstance[] soundInstanceItem = new SoundEffectInstance[0x11];
        public static SoundEffectInstance soundInstanceMenuClose;
        public static SoundEffectInstance soundInstanceMenuOpen;
        public static SoundEffectInstance soundInstanceMenuTick;
        public static SoundEffectInstance[] soundInstanceNPCHit = new SoundEffectInstance[4];
        public static SoundEffectInstance[] soundInstanceNPCKilled = new SoundEffectInstance[4];
        public static SoundEffectInstance[] soundInstancePlayerHit = new SoundEffectInstance[3];
        public static SoundEffectInstance soundInstancePlayerKilled;
        public static SoundEffectInstance[] soundInstanceRoar = new SoundEffectInstance[2];
        public static SoundEffectInstance soundInstanceRun;
        public static SoundEffectInstance soundInstanceShatter;
        public static SoundEffectInstance[] soundInstanceSplash = new SoundEffectInstance[2];
        public static SoundEffectInstance[] soundInstanceTink = new SoundEffectInstance[3];
        public static SoundEffectInstance[] soundInstanceZombie = new SoundEffectInstance[3];
        public static SoundEffect[] soundItem = new SoundEffect[0x11];
        public static SoundEffect soundMenuClose;
        public static SoundEffect soundMenuOpen;
        public static SoundEffect soundMenuTick;
        public static SoundEffect[] soundNPCHit = new SoundEffect[4];
        public static SoundEffect[] soundNPCKilled = new SoundEffect[4];
        public static SoundEffect[] soundPlayerHit = new SoundEffect[3];
        public static SoundEffect soundPlayerKilled;
        public static SoundEffect[] soundRoar = new SoundEffect[2];
        public static SoundEffect soundRun;
        public static SoundEffect soundShatter;
        public static SoundEffect[] soundSplash = new SoundEffect[2];
        public static SoundEffect[] soundTink = new SoundEffect[3];
        public static float soundVolume = 0f;
        public static SoundEffect[] soundZombie = new SoundEffect[3];
        public static int spawnTileX;
        public static int spawnTileY;
        private int splashCounter;
        public static Texture2D splashTexture;
        private SpriteBatch spriteBatch;
        public static int stackCounter = 0;
        public static int stackDelay = 7;
        public static int stackSplit;
        public static Star[] star = new Star[130];
        public static Texture2D[] starTexture = new Texture2D[5];
        public static string statusText = "";
        public static bool stopSpawns = false;
        public static bool stopTimeOuts = false;
        public static short sunModY = 0;
        public static Texture2D sunTexture;
        public static Color[] teamColor = new Color[5];
        public static Texture2D teamTexture;
        public static Texture2D textBackTexture;
        private int textBlinkerCount;
        private int textBlinkerState;
        public static Tile[,] tile = new Tile[maxTilesX, maxTilesY];
        public static bool[] tileBlockLight = new bool[80];
        public static Color tileColor;
        public static bool[] tileDungeon = new bool[80];
        public static bool[] tileFrameImportant = new bool[80];
        public static bool[] tileLavaDeath = new bool[80];
        public static bool[] tileNoAttach = new bool[80];
        public static bool[] tileNoFail = new bool[80];
        public static bool tilesLoaded = false;
        public static bool[] tileSolid = new bool[80];
        public static bool[] tileSolidTop = new bool[80];
        public static bool[] tileStone = new bool[80];
        public static bool[] tileTable = new bool[80];
        public static Texture2D[] tileTexture = new Texture2D[80];
        public static bool[] tileWaterDeath = new bool[80];
        public static double time = 13500.0;
        public static int timeOut = 120;
        public bool toggleFullscreen;
        private static Item toolTip = new Item();
        public static float topWorld = 0f;
        public static Texture2D treeBranchTexture;
        public static Texture2D treeTopTexture;
        public static int updateTime = 0;
        public static bool verboseNetplay = true;
        public static string versionNumber = "v1.0.2";
        public static bool[] wallHouse = new bool[14];
        public static Texture2D[] wallTexture = new Texture2D[14];
        public static Microsoft.Xna.Framework.Audio.WaveBank waveBank;
        private static bool webAuth = false;
        public static bool webProtect = false;
        public static float windSpeed = 0f;
        public static float windSpeedSpeed = 0f;
        public static int worldID;
        public static string worldName = "";
        public static string WorldPath = (SavePath + @"\Worlds");
        public static string worldPathName;
        public static double worldSurface;

        public Main()
        {
            this.graphics = new GraphicsDeviceManager(this);
            base.Content.RootDirectory = "Content";
            ShankShock.setupConfiguration();
        }

        public static double CalculateDamage(int Damage, int Defense)
        {
            double num = Damage - (Defense * 0.5);
            if (num < 1.0)
            {
                num = 1.0;
            }
            return num;
        }

        public static void CursorColor()
        {
            cursorAlpha += cursorColorDirection * 0.015f;
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
            float num = (cursorAlpha * 0.3f) + 0.7f;
            byte r = (byte) (mouseColor.R * cursorAlpha);
            byte g = (byte) (mouseColor.G * cursorAlpha);
            byte b = (byte) (mouseColor.B * cursorAlpha);
            byte a = (byte) (255f * num);
            cursorColor = new Color(r, g, b, a);
            cursorScale = ((cursorAlpha * 0.3f) + 0.7f) + 0.1f;
        }

        protected override void Draw(GameTime gameTime)
        {
            CursorColor();
            drawTime++;
            screenLastPosition = screenPosition;
            if (stackSplit == 0)
            {
                stackCounter = 0;
                stackDelay = 7;
            }
            else
            {
                stackCounter++;
                if (stackCounter >= 30)
                {
                    stackDelay--;
                    if (stackDelay < 2)
                    {
                        stackDelay = 2;
                    }
                    stackCounter = 0;
                }
            }
            mouseTextColor = (byte) (mouseTextColor + ((byte) mouseTextColorChange));
            if (mouseTextColor >= 250)
            {
                mouseTextColorChange = -4;
            }
            if (mouseTextColor <= 0xaf)
            {
                mouseTextColorChange = 4;
            }
            if (myPlayer >= 0)
            {
                player[myPlayer].mouseInterface = false;
            }
            toolTip = new Item();
            if ((!debugMode && !gameMenu) && (netMode != 2))
            {
                int x = mouseState.X;
                int y = mouseState.Y;
                if (x < 0)
                {
                    x = 0;
                }
                if (x > screenWidth)
                {
                    x = screenWidth;
                }
                if (y < 0)
                {
                    y = 0;
                }
                if (y > screenHeight)
                {
                    y = screenHeight;
                }
                screenPosition.X = (player[myPlayer].position.X + (player[myPlayer].width * 0.5f)) - (screenWidth * 0.5f);
                screenPosition.Y = (player[myPlayer].position.Y + (player[myPlayer].height * 0.5f)) - (screenHeight * 0.5f);
                screenPosition.X = (int) screenPosition.X;
                screenPosition.Y = (int) screenPosition.Y;
            }
            if (!gameMenu && (netMode != 2))
            {
                if (screenPosition.X < ((leftWorld + 336f) + 16f))
                {
                    screenPosition.X = (leftWorld + 336f) + 16f;
                }
                else if ((screenPosition.X + screenWidth) > ((rightWorld - 336f) - 32f))
                {
                    screenPosition.X = ((rightWorld - screenWidth) - 336f) - 32f;
                }
                if (screenPosition.Y < ((topWorld + 336f) + 16f))
                {
                    screenPosition.Y = (topWorld + 336f) + 16f;
                }
                else if ((screenPosition.Y + screenHeight) > ((bottomWorld - 336f) - 32f))
                {
                    screenPosition.Y = ((bottomWorld - screenHeight) - 336f) - 32f;
                }
            }
            if (showSplash)
            {
                this.DrawSplash(gameTime);
            }
            else
            {
                Rectangle rectangle;
                base.GraphicsDevice.Clear(Color.Black);
                base.Draw(gameTime);
                this.spriteBatch.Begin();
                double caveParrallax = 0.5;
                int num4 = ((int) -Math.IEEERemainder(screenPosition.X * caveParrallax, (double) backgroundWidth[background])) - (backgroundWidth[background] / 2);
                int num5 = (screenWidth / backgroundWidth[background]) + 2;
                int num6 = 0;
                int num7 = 0;
                int num8 = (int) ((((double) -screenPosition.Y) / ((worldSurface * 16.0) - screenHeight)) * (backgroundHeight[background] - screenHeight));
                if (gameMenu || (netMode == 2))
                {
                    num8 = -200;
                }
                Color white = Color.White;
                int num9 = ((int) ((time / 54000.0) * (screenWidth + (sunTexture.Width * 2)))) - sunTexture.Width;
                int num10 = 0;
                Color color = Color.White;
                float scale = 1f;
                float rotation = (((float) (time / 54000.0)) * 2f) - 7.3f;
                int num14 = ((int) ((time / 32400.0) * (screenWidth + (moonTexture.Width * 2)))) - moonTexture.Width;
                int num15 = 0;
                Color color3 = Color.White;
                float num16 = 1f;
                float num17 = (((float) (time / 32400.0)) * 2f) - 7.3f;
                float num19 = 0f;
                if (dayTime)
                {
                    double num13;
                    if (time < 27000.0)
                    {
                        num13 = Math.Pow(1.0 - ((time / 54000.0) * 2.0), 2.0);
                        num10 = (int) ((num8 + (num13 * 250.0)) + 180.0);
                    }
                    else
                    {
                        num13 = Math.Pow(((time / 54000.0) - 0.5) * 2.0, 2.0);
                        num10 = (int) ((num8 + (num13 * 250.0)) + 180.0);
                    }
                    scale = (float) (1.2 - (num13 * 0.4));
                }
                else
                {
                    double num18;
                    if (time < 16200.0)
                    {
                        num18 = Math.Pow(1.0 - ((time / 32400.0) * 2.0), 2.0);
                        num15 = (int) ((num8 + (num18 * 250.0)) + 180.0);
                    }
                    else
                    {
                        num18 = Math.Pow(((time / 32400.0) - 0.5) * 2.0, 2.0);
                        num15 = (int) ((num8 + (num18 * 250.0)) + 180.0);
                    }
                    num16 = (float) (1.2 - (num18 * 0.4));
                }
                if (dayTime)
                {
                    if (time < 13500.0)
                    {
                        num19 = (float) (time / 13500.0);
                        color.R = (byte) ((num19 * 200f) + 55f);
                        color.G = (byte) ((num19 * 180f) + 75f);
                        color.B = (byte) ((num19 * 250f) + 5f);
                        white.R = (byte) ((num19 * 200f) + 55f);
                        white.G = (byte) ((num19 * 200f) + 55f);
                        white.B = (byte) ((num19 * 200f) + 55f);
                    }
                    if (time > 45900.0)
                    {
                        num19 = (float) (1.0 - (((time / 54000.0) - 0.85) * 6.666666666666667));
                        color.R = (byte) ((num19 * 120f) + 55f);
                        color.G = (byte) ((num19 * 100f) + 25f);
                        color.B = (byte) ((num19 * 120f) + 55f);
                        white.R = (byte) ((num19 * 200f) + 55f);
                        white.G = (byte) ((num19 * 85f) + 55f);
                        white.B = (byte) ((num19 * 135f) + 55f);
                    }
                    else if (time > 37800.0)
                    {
                        num19 = (float) (1.0 - (((time / 54000.0) - 0.7) * 6.666666666666667));
                        color.R = (byte) ((num19 * 80f) + 175f);
                        color.G = (byte) ((num19 * 130f) + 125f);
                        color.B = (byte) ((num19 * 100f) + 155f);
                        white.R = (byte) ((num19 * 0f) + 255f);
                        white.G = (byte) ((num19 * 115f) + 140f);
                        white.B = (byte) ((num19 * 75f) + 180f);
                    }
                }
                if (!dayTime)
                {
                    if (bloodMoon)
                    {
                        if (time < 16200.0)
                        {
                            num19 = (float) (1.0 - (time / 16200.0));
                            color3.R = (byte) ((num19 * 10f) + 205f);
                            color3.G = (byte) ((num19 * 170f) + 55f);
                            color3.B = (byte) ((num19 * 200f) + 55f);
                            white.R = (byte) ((60f - (num19 * 60f)) + 55f);
                            white.G = (byte) ((num19 * 40f) + 15f);
                            white.B = (byte) ((num19 * 40f) + 15f);
                        }
                        else if (time >= 16200.0)
                        {
                            num19 = (float) (((time / 32400.0) - 0.5) * 2.0);
                            color3.R = (byte) ((num19 * 50f) + 205f);
                            color3.G = (byte) ((num19 * 100f) + 155f);
                            color3.B = (byte) ((num19 * 100f) + 155f);
                            color3.R = (byte) ((num19 * 10f) + 205f);
                            color3.G = (byte) ((num19 * 170f) + 55f);
                            color3.B = (byte) ((num19 * 200f) + 55f);
                            white.R = (byte) ((60f - (num19 * 60f)) + 55f);
                            white.G = (byte) ((num19 * 40f) + 15f);
                            white.B = (byte) ((num19 * 40f) + 15f);
                        }
                    }
                    else if (time < 16200.0)
                    {
                        num19 = (float) (1.0 - (time / 16200.0));
                        color3.R = (byte) ((num19 * 10f) + 205f);
                        color3.G = (byte) ((num19 * 70f) + 155f);
                        color3.B = (byte) ((num19 * 100f) + 155f);
                        white.R = (byte) ((num19 * 40f) + 15f);
                        white.G = (byte) ((num19 * 40f) + 15f);
                        white.B = (byte) ((num19 * 40f) + 15f);
                    }
                    else if (time >= 16200.0)
                    {
                        num19 = (float) (((time / 32400.0) - 0.5) * 2.0);
                        color3.R = (byte) ((num19 * 50f) + 205f);
                        color3.G = (byte) ((num19 * 100f) + 155f);
                        color3.B = (byte) ((num19 * 100f) + 155f);
                        white.R = (byte) ((num19 * 40f) + 15f);
                        white.G = (byte) ((num19 * 40f) + 15f);
                        white.B = (byte) ((num19 * 40f) + 15f);
                    }
                }
                if (gameMenu || (netMode == 2))
                {
                    num8 = 0;
                    if (!dayTime)
                    {
                        white.R = 0x37;
                        white.G = 0x37;
                        white.B = 0x37;
                    }
                }
                if (evilTiles > 0)
                {
                    float num20 = ((float) evilTiles) / 500f;
                    if (num20 > 1f)
                    {
                        num20 = 1f;
                    }
                    int r = white.R;
                    int g = white.G;
                    int b = white.B;
                    r += (int) (10f * num20);
                    g -= (int) ((90f * num20) * (((float) white.G) / 255f));
                    b -= (int) ((190f * num20) * (((float) white.B) / 255f));
                    if (r > 0xff)
                    {
                        r = 0xff;
                    }
                    if (g < 15)
                    {
                        g = 15;
                    }
                    if (b < 15)
                    {
                        b = 15;
                    }
                    white.R = (byte) r;
                    white.G = (byte) g;
                    white.B = (byte) b;
                    r = color.R;
                    g = color.G;
                    b = color.B;
                    r -= (int) ((100f * num20) * (((float) color.R) / 255f));
                    g -= (int) ((160f * num20) * (((float) color.G) / 255f));
                    b -= (int) ((170f * num20) * (((float) color.B) / 255f));
                    if (r < 15)
                    {
                        r = 15;
                    }
                    if (g < 15)
                    {
                        g = 15;
                    }
                    if (b < 15)
                    {
                        b = 15;
                    }
                    color.R = (byte) r;
                    color.G = (byte) g;
                    color.B = (byte) b;
                    r = color3.R;
                    g = color3.G;
                    b = color3.B;
                    r -= (int) ((140f * num20) * (((float) color3.R) / 255f));
                    g -= (int) ((170f * num20) * (((float) color3.G) / 255f));
                    b -= (int) ((190f * num20) * (((float) color3.B) / 255f));
                    if (r < 15)
                    {
                        r = 15;
                    }
                    if (g < 15)
                    {
                        g = 15;
                    }
                    if (b < 15)
                    {
                        b = 15;
                    }
                    color3.R = (byte) r;
                    color3.G = (byte) g;
                    color3.B = (byte) b;
                }
                tileColor.A = 0xff;
                tileColor.R = (byte) (((white.R + white.B) + white.G) / 3);
                tileColor.G = (byte) (((white.R + white.B) + white.G) / 3);
                tileColor.B = (byte) (((white.R + white.B) + white.G) / 3);
                if (screenPosition.Y < ((worldSurface * 16.0) + 16.0))
                {
                    for (int i = 0; i < num5; i++)
                    {
                        this.spriteBatch.Draw(backgroundTexture[background], new Rectangle(num4 + (backgroundWidth[background] * i), num8, backgroundWidth[background], backgroundHeight[background]), white);
                    }
                }
                if (((screenPosition.Y < ((worldSurface * 16.0) + 16.0)) && (((0xff - tileColor.R) - 100) > 0)) && (netMode != 2))
                {
                    Star.UpdateStars();
                    for (int j = 0; j < numStars; j++)
                    {
                        Color color4 = new Color();
                        float num26 = ((float) evilTiles) / 500f;
                        if (num26 > 1f)
                        {
                            num26 = 1f;
                        }
                        num26 = 1f - (num26 * 0.5f);
                        if (evilTiles <= 0)
                        {
                            num26 = 1f;
                        }
                        int num27 = (int) ((((0xff - tileColor.R) - 100) * star[j].twinkle) * num26);
                        int num28 = (int) ((((0xff - tileColor.G) - 100) * star[j].twinkle) * num26);
                        int num29 = (int) ((((0xff - tileColor.B) - 100) * star[j].twinkle) * num26);
                        if (num27 < 0)
                        {
                            num27 = 0;
                        }
                        if (num28 < 0)
                        {
                            num28 = 0;
                        }
                        if (num29 < 0)
                        {
                            num29 = 0;
                        }
                        color4.R = (byte) num27;
                        color4.G = (byte) (num28 * num26);
                        color4.B = (byte) (num29 * num26);
                        this.spriteBatch.Draw(starTexture[star[j].type], new Vector2(star[j].position.X + (starTexture[star[j].type].Width * 0.5f), (star[j].position.Y + (starTexture[star[j].type].Height * 0.5f)) + num8), new Rectangle(0, 0, starTexture[star[j].type].Width, starTexture[star[j].type].Height), color4, star[j].rotation, new Vector2(starTexture[star[j].type].Width * 0.5f, starTexture[star[j].type].Height * 0.5f), (float) (star[j].scale * star[j].twinkle), SpriteEffects.None, 0f);
                    }
                }
                if (dayTime)
                {
                    this.spriteBatch.Draw(sunTexture, new Vector2((float) num9, (float) (num10 + sunModY)), new Rectangle(0, 0, sunTexture.Width, sunTexture.Height), color, rotation, new Vector2((float) (sunTexture.Width / 2), (float) (sunTexture.Height / 2)), scale, SpriteEffects.None, 0f);
                }
                if (!dayTime)
                {
                    this.spriteBatch.Draw(moonTexture, new Vector2((float) num14, (float) (num15 + moonModY)), new Rectangle(0, moonTexture.Width * moonPhase, moonTexture.Width, moonTexture.Width), color3, num17, new Vector2((float) (moonTexture.Width / 2), (float) (moonTexture.Width / 2)), num16, SpriteEffects.None, 0f);
                }
                if (dayTime)
                {
                    rectangle = new Rectangle(num9 - ((int) ((sunTexture.Width * 0.5) * scale)), (int) ((num10 - ((sunTexture.Height * 0.5) * scale)) + sunModY), (int) (sunTexture.Width * scale), (int) (sunTexture.Width * scale));
                }
                else
                {
                    rectangle = new Rectangle(num14 - ((int) ((moonTexture.Width * 0.5) * num16)), (int) ((num15 - ((moonTexture.Width * 0.5) * num16)) + moonModY), (int) (moonTexture.Width * num16), (int) (moonTexture.Width * num16));
                }
                Rectangle rectangle2 = new Rectangle(mouseState.X, mouseState.Y, 1, 1);
                sunModY = (short) (sunModY * 0.999);
                moonModY = (short) (moonModY * 0.999);
                if ((gameMenu && (netMode != 1)) || grabSun)
                {
                    if ((mouseState.LeftButton == ButtonState.Pressed) && hasFocus)
                    {
                        if (rectangle2.Intersects(rectangle) || grabSky)
                        {
                            if (dayTime)
                            {
                                time = 54000.0 * (((float) (mouseState.X + sunTexture.Width)) / (screenWidth + (sunTexture.Width * 2)));
                                sunModY = (short) (mouseState.Y - num10);
                                if (time > 53990.0)
                                {
                                    time = 53990.0;
                                }
                            }
                            else
                            {
                                time = 32400.0 * (((float) (mouseState.X + moonTexture.Width)) / (screenWidth + (moonTexture.Width * 2)));
                                moonModY = (short) (mouseState.Y - num15);
                                if (time > 32390.0)
                                {
                                    time = 32390.0;
                                }
                            }
                            if (time < 10.0)
                            {
                                time = 10.0;
                            }
                            if (netMode != 0)
                            {
                                NetMessage.SendData(0x12, -1, -1, "", 0, 0f, 0f, 0f);
                            }
                            grabSky = true;
                        }
                    }
                    else
                    {
                        grabSky = false;
                    }
                }
                if (resetClouds)
                {
                    Cloud.resetClouds();
                    resetClouds = false;
                }
                if (base.IsActive || (netMode != 0))
                {
                    Cloud.UpdateClouds();
                    windSpeedSpeed += rand.Next(-10, 11) * 0.0001f;
                    if (windSpeedSpeed < -0.002)
                    {
                        windSpeedSpeed = -0.002f;
                    }
                    if (windSpeedSpeed > 0.002)
                    {
                        windSpeedSpeed = 0.002f;
                    }
                    windSpeed += windSpeedSpeed;
                    if (windSpeed < -0.3)
                    {
                        windSpeed = -0.3f;
                    }
                    if (windSpeed > 0.3)
                    {
                        windSpeed = 0.3f;
                    }
                    numClouds += rand.Next(-1, 2);
                    if (numClouds < 0)
                    {
                        numClouds = 0;
                    }
                    if (numClouds > cloudLimit)
                    {
                        numClouds = cloudLimit;
                    }
                }
                if (screenPosition.Y < ((worldSurface * 16.0) + 16.0))
                {
                    for (int k = 0; k < 100; k++)
                    {
                        if (cloud[k].active)
                        {
                            int num31 = (int) (40f * (2f - cloud[k].scale));
                            int num32 = 0;
                            Color color5 = new Color();
                            num32 = white.R - num31;
                            if (num32 <= 0)
                            {
                                num32 = 0;
                            }
                            color5.R = (byte) num32;
                            num32 = white.G - num31;
                            if (num32 <= 0)
                            {
                                num32 = 0;
                            }
                            color5.G = (byte) num32;
                            num32 = white.B - num31;
                            if (num32 <= 0)
                            {
                                num32 = 0;
                            }
                            color5.B = (byte) num32;
                            color5.A = (byte) (0xff - num31);
                            this.spriteBatch.Draw(cloudTexture[cloud[k].type], new Vector2(cloud[k].position.X + (cloudTexture[cloud[k].type].Width * 0.5f), (cloud[k].position.Y + (cloudTexture[cloud[k].type].Height * 0.5f)) + num8), new Rectangle(0, 0, cloudTexture[cloud[k].type].Width, cloudTexture[cloud[k].type].Height), color5, cloud[k].rotation, new Vector2(cloudTexture[cloud[k].type].Width * 0.5f, cloudTexture[cloud[k].type].Height * 0.5f), cloud[k].scale, SpriteEffects.None, 0f);
                        }
                    }
                }
                if (gameMenu || (netMode == 2))
                {
                    this.DrawMenu();
                }
                else
                {
                    Vector2 vector10;
                    Color color16;
                    int firstX = (int) ((screenPosition.X / 16f) - 1f);
                    int lastX = ((int) ((screenPosition.X + screenWidth) / 16f)) + 2;
                    int firstY = (int) ((screenPosition.Y / 16f) - 1f);
                    int lastY = ((int) ((screenPosition.Y + screenHeight) / 16f)) + 2;
                    if (firstX < 0)
                    {
                        firstX = 0;
                    }
                    if (lastX > maxTilesX)
                    {
                        lastX = maxTilesX;
                    }
                    if (firstY < 0)
                    {
                        firstY = 0;
                    }
                    if (lastY > maxTilesY)
                    {
                        lastY = maxTilesY;
                    }
                    Lighting.LightTiles(firstX, lastX, firstY, lastY);
                    Color color1 = Color.White;
                    this.DrawWater(true);
                    caveParrallax = Main.caveParrallax;
                    num4 = ((int) -Math.IEEERemainder(screenPosition.X * caveParrallax, (double) backgroundWidth[1])) - (backgroundWidth[1] / 2);
                    num5 = (screenWidth / backgroundWidth[1]) + 2;
                    num8 = (int) ((((((int) worldSurface) * 0x10) - backgroundHeight[1]) - screenPosition.Y) + 16f);
                    for (int m = 0; m < num5; m++)
                    {
                        for (int num38 = 0; num38 < 6; num38++)
                        {
                            int num39 = ((num4 + (backgroundWidth[1] * m)) + (num38 * 0x10)) / 0x10;
                            int num40 = num8 / 0x10;
                            Color color6 = Lighting.GetColor(num39 + ((int) (screenPosition.X / 16f)), num40 + ((int) (screenPosition.Y / 16f)));
                            this.spriteBatch.Draw(backgroundTexture[1], new Vector2((float) ((num4 + (backgroundWidth[1] * m)) + (0x10 * num38)), (float) num8), new Rectangle(0x10 * num38, 0, 0x10, 0x10), color6);
                        }
                    }
                    double num41 = maxTilesY - 230;
                    double num42 = ((int) ((num41 - worldSurface) / 6.0)) * 6;
                    num41 = (worldSurface + num42) - 5.0;
                    bool flag = false;
                    bool flag2 = false;
                    num8 = (int) (((((int) worldSurface) * 0x10) - screenPosition.Y) + 16f);
                    if ((worldSurface * 16.0) <= (screenPosition.Y + screenHeight))
                    {
                        caveParrallax = Main.caveParrallax;
                        num4 = ((int) -Math.IEEERemainder(100.0 + (screenPosition.X * caveParrallax), (double) backgroundWidth[2])) - (backgroundWidth[2] / 2);
                        num5 = (screenWidth / backgroundWidth[2]) + 2;
                        if ((worldSurface * 16.0) < (screenPosition.Y - 16f))
                        {
                            num6 = ((int) Math.IEEERemainder((double) num8, (double) backgroundHeight[2])) - backgroundHeight[2];
                            num7 = (screenHeight / backgroundHeight[2]) + 2;
                        }
                        else
                        {
                            num6 = num8;
                            num7 = ((screenHeight - num8) / backgroundHeight[2]) + 1;
                        }
                        if ((rockLayer * 16.0) < (screenPosition.Y + screenHeight))
                        {
                            num7 = ((((int) ((rockLayer * 16.0) - screenPosition.Y)) + screenHeight) - num6) / backgroundHeight[2];
                            flag2 = true;
                        }
                        for (int num43 = 0; num43 < num5; num43++)
                        {
                            for (int num44 = 0; num44 < num7; num44++)
                            {
                                this.spriteBatch.Draw(backgroundTexture[2], new Rectangle(num4 + (backgroundWidth[2] * num43), num6 + (backgroundHeight[2] * num44), backgroundWidth[2], backgroundHeight[2]), Color.White);
                            }
                        }
                        if (flag2)
                        {
                            caveParrallax = Main.caveParrallax;
                            num4 = ((int) -Math.IEEERemainder(screenPosition.X * caveParrallax, (double) backgroundWidth[1])) - (backgroundWidth[1] / 2);
                            num5 = (screenWidth / backgroundWidth[1]) + 2;
                            num8 = num6 + (num7 * backgroundHeight[2]);
                            for (int num45 = 0; num45 < num5; num45++)
                            {
                                for (int num46 = 0; num46 < 6; num46++)
                                {
                                    int num1 = ((num4 + (backgroundWidth[4] * num45)) + (num46 * 0x10)) / 0x10;
                                    int num97 = num8 / 0x10;
                                    this.spriteBatch.Draw(backgroundTexture[4], new Vector2((float) ((num4 + (backgroundWidth[4] * num45)) + (0x10 * num46)), (float) num8), new Rectangle(0x10 * num46, 0, 0x10, 0x18), Color.White);
                                }
                            }
                        }
                    }
                    num8 = ((int) (((((int) rockLayer) * 0x10) - screenPosition.Y) + 16f)) + screenHeight;
                    if ((rockLayer * 16.0) <= (screenPosition.Y + screenHeight))
                    {
                        caveParrallax = Main.caveParrallax;
                        num4 = ((int) -Math.IEEERemainder(100.0 + (screenPosition.X * caveParrallax), (double) backgroundWidth[3])) - (backgroundWidth[3] / 2);
                        num5 = (screenWidth / backgroundWidth[3]) + 2;
                        if (((rockLayer * 16.0) + screenHeight) < (screenPosition.Y - 16f))
                        {
                            num6 = ((int) Math.IEEERemainder((double) num8, (double) backgroundHeight[3])) - backgroundHeight[3];
                            num7 = (screenHeight / backgroundHeight[3]) + 2;
                        }
                        else
                        {
                            num6 = num8;
                            num7 = ((screenHeight - num8) / backgroundHeight[3]) + 1;
                        }
                        if ((num41 * 16.0) < (screenPosition.Y + screenHeight))
                        {
                            num7 = ((((int) ((num41 * 16.0) - screenPosition.Y)) + screenHeight) - num6) / backgroundHeight[2];
                            flag = true;
                        }
                        for (int num47 = 0; num47 < num5; num47++)
                        {
                            for (int num48 = 0; num48 < num7; num48++)
                            {
                                this.spriteBatch.Draw(backgroundTexture[3], new Rectangle(num4 + (backgroundWidth[2] * num47), num6 + (backgroundHeight[2] * num48), backgroundWidth[2], backgroundHeight[2]), Color.White);
                            }
                        }
                        if (flag)
                        {
                            caveParrallax = Main.caveParrallax;
                            num4 = (int) ((-Math.IEEERemainder(screenPosition.X * caveParrallax, (double) backgroundWidth[1]) - (backgroundWidth[1] / 2)) - 4.0);
                            num5 = (screenWidth / backgroundWidth[1]) + 2;
                            num8 = num6 + (num7 * backgroundHeight[2]);
                            for (int num49 = 0; num49 < num5; num49++)
                            {
                                for (int num50 = 0; num50 < 6; num50++)
                                {
                                    int num51 = ((num4 + (backgroundWidth[1] * num49)) + (num50 * 0x10)) / 0x10;
                                    int num52 = num8 / 0x10;
                                    Lighting.GetColor(num51 + ((int) (screenPosition.X / 16f)), num52 + ((int) (screenPosition.Y / 16f)));
                                    this.spriteBatch.Draw(backgroundTexture[6], new Vector2((float) ((num4 + (backgroundWidth[1] * num49)) + (0x10 * num50)), (float) num8), new Rectangle(0x10 * num50, magmaBGFrame * 0x18, 0x10, 0x18), Color.White);
                                }
                            }
                        }
                    }
                    num8 = (((int) (((((int) num41) * 0x10) - screenPosition.Y) + 16f)) + screenHeight) + 8;
                    if ((num41 * 16.0) <= (screenPosition.Y + screenHeight))
                    {
                        num4 = ((int) -Math.IEEERemainder(100.0 + (screenPosition.X * caveParrallax), (double) backgroundWidth[3])) - (backgroundWidth[3] / 2);
                        num5 = (screenWidth / backgroundWidth[3]) + 2;
                        if (((num41 * 16.0) + screenHeight) < (screenPosition.Y - 16f))
                        {
                            num6 = ((int) Math.IEEERemainder((double) num8, (double) backgroundHeight[3])) - backgroundHeight[3];
                            num7 = (screenHeight / backgroundHeight[3]) + 2;
                        }
                        else
                        {
                            num6 = num8;
                            num7 = ((screenHeight - num8) / backgroundHeight[3]) + 1;
                        }
                        for (int num53 = 0; num53 < num5; num53++)
                        {
                            for (int num54 = 0; num54 < num7; num54++)
                            {
                                vector10 = new Vector2();
                                this.spriteBatch.Draw(backgroundTexture[5], new Vector2((float) (num4 + (backgroundWidth[2] * num53)), (float) (num6 + (backgroundHeight[2] * num54))), new Rectangle(0, backgroundHeight[2] * magmaBGFrame, backgroundWidth[2], backgroundHeight[2]), Color.White, 0f, vector10, (float) 1f, SpriteEffects.None, 0f);
                            }
                        }
                    }
                    magmaBGFrameCounter++;
                    if (magmaBGFrameCounter >= 8)
                    {
                        magmaBGFrameCounter = 0;
                        magmaBGFrame++;
                        if (magmaBGFrame >= 3)
                        {
                            magmaBGFrame = 0;
                        }
                    }
                    try
                    {
                        for (int num55 = firstY; num55 < (lastY + 4); num55++)
                        {
                            for (int num56 = firstX - 2; num56 < (lastX + 2); num56++)
                            {
                                if (tile[num56, num55] == null)
                                {
                                    tile[num56, num55] = new Tile();
                                }
                                if (((Lighting.Brightness(num56, num55) * 255f) < (tileColor.R - 12)) || (num55 > worldSurface))
                                {
                                    vector10 = new Vector2();
                                    this.spriteBatch.Draw(blackTileTexture, new Vector2((float) ((num56 * 0x10) - ((int) screenPosition.X)), (float) ((num55 * 0x10) - ((int) screenPosition.Y))), new Rectangle(tile[num56, num55].frameX, tile[num56, num55].frameY, 0x10, 0x10), Lighting.GetBlackness(num56, num55), 0f, vector10, (float) 1f, SpriteEffects.None, 0f);
                                }
                            }
                        }
                        for (int num57 = firstY; num57 < (lastY + 4); num57++)
                        {
                            for (int num58 = firstX - 2; num58 < (lastX + 2); num58++)
                            {
                                if ((tile[num58, num57].wall > 0) && (Lighting.Brightness(num58, num57) > 0f))
                                {
                                    if ((tile[num58, num57].wallFrameY == 0x12) && (tile[num58, num57].wallFrameX >= 0x12))
                                    {
                                        byte wallFrameY = tile[num58, num57].wallFrameY;
                                    }
                                    Rectangle rectangle3 = new Rectangle(tile[num58, num57].wallFrameX * 2, tile[num58, num57].wallFrameY * 2, 0x20, 0x20);
                                    vector10 = new Vector2();
                                    this.spriteBatch.Draw(wallTexture[tile[num58, num57].wall], new Vector2((float) (((num58 * 0x10) - ((int) screenPosition.X)) - 8), (float) (((num57 * 0x10) - ((int) screenPosition.Y)) - 8)), new Rectangle?(rectangle3), Lighting.GetColor(num58, num57), 0f, vector10, (float) 1f, SpriteEffects.None, 0f);
                                }
                            }
                        }
                        this.DrawTiles(false);
                        this.DrawNPCs(true);
                        this.DrawTiles(true);
                        this.DrawGore();
                        this.DrawNPCs(false);
                    }
                    catch
                    {
                    }
                    for (int n = 0; n < 0x3e8; n++)
                    {
                        if (!projectile[n].active || (projectile[n].type <= 0))
                        {
                            continue;
                        }
                        if (projectile[n].type == 0x20)
                        {
                            Vector2 vector = new Vector2(projectile[n].position.X + (projectile[n].width * 0.5f), projectile[n].position.Y + (projectile[n].height * 0.5f));
                            float num60 = (player[projectile[n].owner].position.X + (player[projectile[n].owner].width / 2)) - vector.X;
                            float num61 = (player[projectile[n].owner].position.Y + (player[projectile[n].owner].height / 2)) - vector.Y;
                            float num62 = ((float) Math.Atan2((double) num61, (double) num60)) - 1.57f;
                            bool flag3 = true;
                            if ((num60 == 0f) && (num61 == 0f))
                            {
                                flag3 = false;
                            }
                            else
                            {
                                float num63 = (float) Math.Sqrt((double) ((num60 * num60) + (num61 * num61)));
                                num63 = 8f / num63;
                                num60 *= num63;
                                num61 *= num63;
                                vector.X -= num60;
                                vector.Y -= num61;
                                num60 = (player[projectile[n].owner].position.X + (player[projectile[n].owner].width / 2)) - vector.X;
                                num61 = (player[projectile[n].owner].position.Y + (player[projectile[n].owner].height / 2)) - vector.Y;
                            }
                            while (flag3)
                            {
                                float num64 = (float) Math.Sqrt((double) ((num60 * num60) + (num61 * num61)));
                                if (num64 < 28f)
                                {
                                    flag3 = false;
                                }
                                else
                                {
                                    num64 = 28f / num64;
                                    num60 *= num64;
                                    num61 *= num64;
                                    vector.X += num60;
                                    vector.Y += num61;
                                    num60 = (player[projectile[n].owner].position.X + (player[projectile[n].owner].width / 2)) - vector.X;
                                    num61 = (player[projectile[n].owner].position.Y + (player[projectile[n].owner].height / 2)) - vector.Y;
                                    Color color7 = Lighting.GetColor(((int) vector.X) / 0x10, (int) (vector.Y / 16f));
                                    this.spriteBatch.Draw(chain5Texture, new Vector2(vector.X - screenPosition.X, vector.Y - screenPosition.Y), new Rectangle(0, 0, chain5Texture.Width, chain5Texture.Height), color7, num62, new Vector2(chain5Texture.Width * 0.5f, chain5Texture.Height * 0.5f), (float) 1f, SpriteEffects.None, 0f);
                                }
                            }
                        }
                        else if (projectile[n].aiStyle == 7)
                        {
                            Vector2 vector2 = new Vector2(projectile[n].position.X + (projectile[n].width * 0.5f), projectile[n].position.Y + (projectile[n].height * 0.5f));
                            float num65 = (player[projectile[n].owner].position.X + (player[projectile[n].owner].width / 2)) - vector2.X;
                            float num66 = (player[projectile[n].owner].position.Y + (player[projectile[n].owner].height / 2)) - vector2.Y;
                            float num67 = ((float) Math.Atan2((double) num66, (double) num65)) - 1.57f;
                            bool flag4 = true;
                            while (flag4)
                            {
                                float num68 = (float) Math.Sqrt((double) ((num65 * num65) + (num66 * num66)));
                                if (num68 < 25f)
                                {
                                    flag4 = false;
                                }
                                else
                                {
                                    num68 = 12f / num68;
                                    num65 *= num68;
                                    num66 *= num68;
                                    vector2.X += num65;
                                    vector2.Y += num66;
                                    num65 = (player[projectile[n].owner].position.X + (player[projectile[n].owner].width / 2)) - vector2.X;
                                    num66 = (player[projectile[n].owner].position.Y + (player[projectile[n].owner].height / 2)) - vector2.Y;
                                    Color color8 = Lighting.GetColor(((int) vector2.X) / 0x10, (int) (vector2.Y / 16f));
                                    this.spriteBatch.Draw(chainTexture, new Vector2(vector2.X - screenPosition.X, vector2.Y - screenPosition.Y), new Rectangle(0, 0, chainTexture.Width, chainTexture.Height), color8, num67, new Vector2(chainTexture.Width * 0.5f, chainTexture.Height * 0.5f), (float) 1f, SpriteEffects.None, 0f);
                                }
                            }
                        }
                        else if (projectile[n].aiStyle == 13)
                        {
                            float num69 = projectile[n].position.X + 8f;
                            float num70 = projectile[n].position.Y + 2f;
                            float num71 = projectile[n].velocity.X;
                            float num72 = projectile[n].velocity.Y;
                            float num73 = (float) Math.Sqrt((double) ((num71 * num71) + (num72 * num72)));
                            num73 = 20f / num73;
                            if (projectile[n].ai[0] == 0f)
                            {
                                num69 -= projectile[n].velocity.X * num73;
                                num70 -= projectile[n].velocity.Y * num73;
                            }
                            else
                            {
                                num69 += projectile[n].velocity.X * num73;
                                num70 += projectile[n].velocity.Y * num73;
                            }
                            Vector2 vector3 = new Vector2(num69, num70);
                            num71 = (player[projectile[n].owner].position.X + (player[projectile[n].owner].width / 2)) - vector3.X;
                            num72 = (player[projectile[n].owner].position.Y + (player[projectile[n].owner].height / 2)) - vector3.Y;
                            float num74 = ((float) Math.Atan2((double) num72, (double) num71)) - 1.57f;
                            if (projectile[n].alpha == 0)
                            {
                                if (player[projectile[n].owner].direction == 1)
                                {
                                    player[projectile[n].owner].itemRotation = num74 - 1.57f;
                                }
                                else
                                {
                                    player[projectile[n].owner].itemRotation = num74 + 1.57f;
                                }
                            }
                            bool flag5 = true;
                            while (flag5)
                            {
                                float num75 = (float) Math.Sqrt((double) ((num71 * num71) + (num72 * num72)));
                                if (num75 < 25f)
                                {
                                    flag5 = false;
                                }
                                else
                                {
                                    num75 = 12f / num75;
                                    num71 *= num75;
                                    num72 *= num75;
                                    vector3.X += num71;
                                    vector3.Y += num72;
                                    num71 = (player[projectile[n].owner].position.X + (player[projectile[n].owner].width / 2)) - vector3.X;
                                    num72 = (player[projectile[n].owner].position.Y + (player[projectile[n].owner].height / 2)) - vector3.Y;
                                    Color color9 = Lighting.GetColor(((int) vector3.X) / 0x10, (int) (vector3.Y / 16f));
                                    this.spriteBatch.Draw(chainTexture, new Vector2(vector3.X - screenPosition.X, vector3.Y - screenPosition.Y), new Rectangle(0, 0, chainTexture.Width, chainTexture.Height), color9, num74, new Vector2(chainTexture.Width * 0.5f, chainTexture.Height * 0.5f), (float) 1f, SpriteEffects.None, 0f);
                                }
                            }
                        }
                        else if (projectile[n].aiStyle == 15)
                        {
                            Vector2 vector4 = new Vector2(projectile[n].position.X + (projectile[n].width * 0.5f), projectile[n].position.Y + (projectile[n].height * 0.5f));
                            float num76 = (player[projectile[n].owner].position.X + (player[projectile[n].owner].width / 2)) - vector4.X;
                            float num77 = (player[projectile[n].owner].position.Y + (player[projectile[n].owner].height / 2)) - vector4.Y;
                            float num78 = ((float) Math.Atan2((double) num77, (double) num76)) - 1.57f;
                            if (projectile[n].alpha == 0)
                            {
                                if (player[projectile[n].owner].direction == 1)
                                {
                                    player[projectile[n].owner].itemRotation = num78 - 1.57f;
                                }
                                else
                                {
                                    player[projectile[n].owner].itemRotation = num78 + 1.57f;
                                }
                            }
                            bool flag6 = true;
                            while (flag6)
                            {
                                float num79 = (float) Math.Sqrt((double) ((num76 * num76) + (num77 * num77)));
                                if (num79 < 25f)
                                {
                                    flag6 = false;
                                }
                                else
                                {
                                    num79 = 12f / num79;
                                    num76 *= num79;
                                    num77 *= num79;
                                    vector4.X += num76;
                                    vector4.Y += num77;
                                    num76 = (player[projectile[n].owner].position.X + (player[projectile[n].owner].width / 2)) - vector4.X;
                                    num77 = (player[projectile[n].owner].position.Y + (player[projectile[n].owner].height / 2)) - vector4.Y;
                                    Color color10 = Lighting.GetColor(((int) vector4.X) / 0x10, (int) (vector4.Y / 16f));
                                    if (projectile[n].type == 0x19)
                                    {
                                        this.spriteBatch.Draw(chain2Texture, new Vector2(vector4.X - screenPosition.X, vector4.Y - screenPosition.Y), new Rectangle(0, 0, chain2Texture.Width, chain2Texture.Height), color10, num78, new Vector2(chain2Texture.Width * 0.5f, chain2Texture.Height * 0.5f), (float) 1f, SpriteEffects.None, 0f);
                                        continue;
                                    }
                                    if (projectile[n].type == 0x23)
                                    {
                                        this.spriteBatch.Draw(chain6Texture, new Vector2(vector4.X - screenPosition.X, vector4.Y - screenPosition.Y), new Rectangle(0, 0, chain6Texture.Width, chain6Texture.Height), color10, num78, new Vector2(chain6Texture.Width * 0.5f, chain6Texture.Height * 0.5f), (float) 1f, SpriteEffects.None, 0f);
                                        continue;
                                    }
                                    this.spriteBatch.Draw(chain3Texture, new Vector2(vector4.X - screenPosition.X, vector4.Y - screenPosition.Y), new Rectangle(0, 0, chain3Texture.Width, chain3Texture.Height), color10, num78, new Vector2(chain3Texture.Width * 0.5f, chain3Texture.Height * 0.5f), (float) 1f, SpriteEffects.None, 0f);
                                }
                            }
                        }
                        Color newColor = Lighting.GetColor(((int) (projectile[n].position.X + (projectile[n].width * 0.5))) / 0x10, (int) ((projectile[n].position.Y + (projectile[n].height * 0.5)) / 16.0));
                        if (projectile[n].type == 14)
                        {
                            newColor = Color.White;
                        }
                        int num80 = 0;
                        if (projectile[n].type == 0x10)
                        {
                            num80 = 6;
                        }
                        if ((projectile[n].type == 0x11) || (projectile[n].type == 0x1f))
                        {
                            num80 = 2;
                        }
                        if (((projectile[n].type == 0x19) || (projectile[n].type == 0x1a)) || (projectile[n].type == 30))
                        {
                            num80 = 6;
                        }
                        if ((projectile[n].type == 0x1c) || (projectile[n].type == 0x25))
                        {
                            num80 = 8;
                        }
                        if (projectile[n].type == 0x1d)
                        {
                            num80 = 11;
                        }
                        float num81 = ((projectileTexture[projectile[n].type].Width - projectile[n].width) * 0.5f) + (projectile[n].width * 0.5f);
                        this.spriteBatch.Draw(projectileTexture[projectile[n].type], new Vector2((projectile[n].position.X - screenPosition.X) + num81, (projectile[n].position.Y - screenPosition.Y) + (projectile[n].height / 2)), new Rectangle(0, 0, projectileTexture[projectile[n].type].Width, projectileTexture[projectile[n].type].Height), projectile[n].GetAlpha(newColor), projectile[n].rotation, new Vector2(num81, (float) ((projectile[n].height / 2) + num80)), projectile[n].scale, SpriteEffects.None, 0f);
                    }
                    for (int num82 = 0; num82 < 8; num82++)
                    {
                        if (player[num82].active)
                        {
                            if (((((player[num82].head == 5) && (player[num82].body == 5)) && (player[num82].legs == 5)) || (((player[num82].head == 7) && (player[num82].body == 7)) && (player[num82].legs == 7))) || (((player[num82].head == 8) && (player[num82].body == 8)) && (player[num82].legs == 8)))
                            {
                                Vector2 position = player[num82].position;
                                player[num82].position = player[num82].shadowPos[0];
                                player[num82].shadow = 0.5f;
                                this.DrawPlayer(player[num82]);
                                player[num82].position = player[num82].shadowPos[1];
                                player[num82].shadow = 0.7f;
                                this.DrawPlayer(player[num82]);
                                player[num82].position = player[num82].shadowPos[2];
                                player[num82].shadow = 0.9f;
                                this.DrawPlayer(player[num82]);
                                player[num82].position = position;
                                player[num82].shadow = 0f;
                            }
                            this.DrawPlayer(player[num82]);
                        }
                    }
                    for (int num83 = 0; num83 < 200; num83++)
                    {
                        if (item[num83].active && (item[num83].type > 0))
                        {
                            int num99 = ((int) (item[num83].position.X + (item[num83].width * 0.5))) / 0x10;
                            int num100 = ((int) (item[num83].position.Y + (item[num83].height * 0.5))) / 0x10;
                            Color color12 = Lighting.GetColor(((int) (item[num83].position.X + (item[num83].width * 0.5))) / 0x10, ((int) (item[num83].position.Y + (item[num83].height * 0.5))) / 0x10);
                            vector10 = new Vector2();
                            this.spriteBatch.Draw(itemTexture[item[num83].type], new Vector2(((item[num83].position.X - screenPosition.X) + (item[num83].width / 2)) - (itemTexture[item[num83].type].Width / 2), ((item[num83].position.Y - screenPosition.Y) + (item[num83].height / 2)) - (itemTexture[item[num83].type].Height / 2)), new Rectangle(0, 0, itemTexture[item[num83].type].Width, itemTexture[item[num83].type].Height), item[num83].GetAlpha(color12), 0f, vector10, (float) 1f, SpriteEffects.None, 0f);
                            color16 = new Color();
                            if (item[num83].color != color16)
                            {
                                vector10 = new Vector2();
                                this.spriteBatch.Draw(itemTexture[item[num83].type], new Vector2(((item[num83].position.X - screenPosition.X) + (item[num83].width / 2)) - (itemTexture[item[num83].type].Width / 2), ((item[num83].position.Y - screenPosition.Y) + (item[num83].height / 2)) - (itemTexture[item[num83].type].Height / 2)), new Rectangle(0, 0, itemTexture[item[num83].type].Width, itemTexture[item[num83].type].Height), item[num83].GetColor(color12), 0f, vector10, (float) 1f, SpriteEffects.None, 0f);
                            }
                        }
                    }
                    Rectangle rectangle4 = new Rectangle(((int) screenPosition.X) - 50, ((int) screenPosition.Y) - 50, screenWidth + 100, screenHeight + 100);
                    for (int num84 = 0; num84 < 0x7d0; num84++)
                    {
                        if (dust[num84].active)
                        {
                            Rectangle rectangle5 = new Rectangle((int) dust[num84].position.X, (int) dust[num84].position.Y, 4, 4);
                            if (rectangle5.Intersects(rectangle4))
                            {
                                Color color13 = Lighting.GetColor(((int) (dust[num84].position.X + 4.0)) / 0x10, ((int) (dust[num84].position.Y + 4.0)) / 0x10);
                                if (((dust[num84].type == 6) || (dust[num84].type == 15)) || dust[num84].noLight)
                                {
                                    color13 = Color.White;
                                }
                                this.spriteBatch.Draw(dustTexture, dust[num84].position - screenPosition, new Rectangle?(dust[num84].frame), dust[num84].GetAlpha(color13), dust[num84].rotation, new Vector2(4f, 4f), dust[num84].scale, SpriteEffects.None, 0f);
                                color16 = new Color();
                                if (dust[num84].color != color16)
                                {
                                    this.spriteBatch.Draw(dustTexture, dust[num84].position - screenPosition, new Rectangle?(dust[num84].frame), dust[num84].GetColor(color13), dust[num84].rotation, new Vector2(4f, 4f), dust[num84].scale, SpriteEffects.None, 0f);
                                }
                            }
                            else
                            {
                                dust[num84].active = false;
                            }
                        }
                    }
                    this.DrawWater(false);
                    if (!hideUI)
                    {
                        for (int num85 = 0; num85 < 8; num85++)
                        {
                            if ((player[num85].active && (player[num85].chatShowTime > 0)) && ((num85 != myPlayer) && !player[num85].dead))
                            {
                                Vector2 vector6;
                                Vector2 vector7 = fontMouseText.MeasureString(player[num85].chatText);
                                vector6.X = (player[num85].position.X + (player[num85].width / 2)) - (vector7.X / 2f);
                                vector6.Y = (player[num85].position.Y - vector7.Y) - 2f;
                                for (int num86 = 0; num86 < 5; num86++)
                                {
                                    int num87 = 0;
                                    int num88 = 0;
                                    Color black = Color.Black;
                                    switch (num86)
                                    {
                                        case 0:
                                            num87 = -2;
                                            break;

                                        case 1:
                                            num87 = 2;
                                            break;

                                        case 2:
                                            num88 = -2;
                                            break;

                                        case 3:
                                            num88 = 2;
                                            break;

                                        case 4:
                                            black = new Color((int) mouseTextColor, (int) mouseTextColor, (int) mouseTextColor, (int) mouseTextColor);
                                            break;
                                    }
                                    vector10 = new Vector2();
                                    this.spriteBatch.DrawString(fontMouseText, player[num85].chatText, new Vector2((vector6.X + num87) - screenPosition.X, (vector6.Y + num88) - screenPosition.Y), black, 0f, vector10, (float) 1f, SpriteEffects.None, 0f);
                                }
                            }
                        }
                        for (int num89 = 0; num89 < 100; num89++)
                        {
                            if (combatText[num89].active)
                            {
                                Vector2 vector8 = fontCombatText.MeasureString(combatText[num89].text);
                                Vector2 origin = new Vector2(vector8.X * 0.5f, vector8.Y * 0.5f);
                                float single1 = combatText[num89].scale;
                                float num90 = combatText[num89].color.R;
                                float num91 = combatText[num89].color.G;
                                float num92 = combatText[num89].color.B;
                                float a = combatText[num89].color.A;
                                num90 *= (combatText[num89].scale * combatText[num89].alpha) * 0.3f;
                                num92 *= (combatText[num89].scale * combatText[num89].alpha) * 0.3f;
                                num91 *= (combatText[num89].scale * combatText[num89].alpha) * 0.3f;
                                a *= combatText[num89].scale * combatText[num89].alpha;
                                Color color15 = new Color((int) num90, (int) num91, (int) num92, (int) a);
                                for (int num94 = 0; num94 < 5; num94++)
                                {
                                    int num95 = 0;
                                    int num96 = 0;
                                    switch (num94)
                                    {
                                        case 0:
                                            num95--;
                                            break;

                                        case 1:
                                            num95++;
                                            break;

                                        case 2:
                                            num96--;
                                            break;

                                        case 3:
                                            num96++;
                                            break;

                                        default:
                                            num90 = (combatText[num89].color.R * combatText[num89].scale) * combatText[num89].alpha;
                                            num92 = (combatText[num89].color.B * combatText[num89].scale) * combatText[num89].alpha;
                                            num91 = (combatText[num89].color.G * combatText[num89].scale) * combatText[num89].alpha;
                                            a = (combatText[num89].color.A * combatText[num89].scale) * combatText[num89].alpha;
                                            color15 = new Color((int) num90, (int) num91, (int) num92, (int) a);
                                            break;
                                    }
                                    this.spriteBatch.DrawString(fontCombatText, combatText[num89].text, new Vector2(((combatText[num89].position.X - screenPosition.X) + num95) + origin.X, ((combatText[num89].position.Y - screenPosition.Y) + num96) + origin.Y), color15, combatText[num89].rotation, origin, combatText[num89].scale, SpriteEffects.None, 0f);
                                }
                            }
                        }
                        if (((netMode == 1) && (Netplay.clientSock.statusText != "")) && (Netplay.clientSock.statusText != null))
                        {
                            string text = string.Concat(new object[] { Netplay.clientSock.statusText, ": ", (int) ((((float) Netplay.clientSock.statusCount) / ((float) Netplay.clientSock.statusMax)) * 100f), "%" });
                            this.spriteBatch.DrawString(fontMouseText, text, new Vector2(628f - (fontMouseText.MeasureString(text).X * 0.5f), 84f), new Color(mouseTextColor, mouseTextColor, mouseTextColor, mouseTextColor), 0f, new Vector2(), (float) 1f, SpriteEffects.None, 0f);
                        }
                        this.DrawFPS();
                        this.DrawInterface();
                    }
                    this.spriteBatch.End();
                    if (mouseState.LeftButton == ButtonState.Pressed)
                    {
                        mouseLeftRelease = false;
                    }
                    else
                    {
                        mouseLeftRelease = true;
                    }
                    if (mouseState.RightButton == ButtonState.Pressed)
                    {
                        mouseRightRelease = false;
                    }
                    else
                    {
                        mouseRightRelease = true;
                    }
                    if (mouseState.RightButton != ButtonState.Pressed)
                    {
                        stackSplit = 0;
                    }
                    if (stackSplit > 0)
                    {
                        stackSplit--;
                    }
                }
            }
        }

        protected void DrawChat()
        {
            if ((player[myPlayer].talkNPC < 0) && (player[myPlayer].sign == -1))
            {
                npcChatText = "";
            }
            else
            {
                Color color = new Color(200, 200, 200, 200);
                int r = ((mouseTextColor * 2) + 0xff) / 3;
                Color color2 = new Color(r, r, r, r);
                int num2 = 10;
                int index = 0;
                string[] strArray = new string[num2];
                int startIndex = 0;
                int num5 = 0;
                if (npcChatText == null)
                {
                    npcChatText = "";
                }
                for (int i = 0; i < npcChatText.Length; i++)
                {
                    if (Encoding.ASCII.GetBytes(npcChatText.Substring(i, 1))[0] == 10)
                    {
                        strArray[index] = npcChatText.Substring(startIndex, i - startIndex);
                        index++;
                        startIndex = i + 1;
                        num5 = i + 1;
                    }
                    else if ((npcChatText.Substring(i, 1) == " ") || (i == (npcChatText.Length - 1)))
                    {
                        if (fontMouseText.MeasureString(npcChatText.Substring(startIndex, i - startIndex)).X > 470f)
                        {
                            strArray[index] = npcChatText.Substring(startIndex, num5 - startIndex);
                            index++;
                            startIndex = num5 + 1;
                        }
                        num5 = i;
                    }
                    if (index == 10)
                    {
                        npcChatText = npcChatText.Substring(0, i - 1);
                        startIndex = i - 1;
                        index = 9;
                        break;
                    }
                }
                if (index < 10)
                {
                    strArray[index] = npcChatText.Substring(startIndex, npcChatText.Length - startIndex);
                }
                if (editSign)
                {
                    this.textBlinkerCount++;
                    if (this.textBlinkerCount >= 20)
                    {
                        if (this.textBlinkerState == 0)
                        {
                            this.textBlinkerState = 1;
                        }
                        else
                        {
                            this.textBlinkerState = 0;
                        }
                        this.textBlinkerCount = 0;
                    }
                    if (this.textBlinkerState == 1)
                    {
                        string[] strArray2;
                        IntPtr ptr;
                        (strArray2 = strArray)[(int) (ptr = (IntPtr) index)] = strArray2[(int) ptr] + "|";
                    }
                }
                index++;
                this.spriteBatch.Draw(chatBackTexture, new Vector2((float) ((screenWidth / 2) - (chatBackTexture.Width / 2)), 100f), new Rectangle(0, 0, chatBackTexture.Width, (index + 1) * 30), color, 0f, new Vector2(), (float) 1f, SpriteEffects.None, 0f);
                this.spriteBatch.Draw(chatBackTexture, new Vector2((float) ((screenWidth / 2) - (chatBackTexture.Width / 2)), (float) (100 + ((index + 1) * 30))), new Rectangle(0, chatBackTexture.Height - 30, chatBackTexture.Width, 30), color, 0f, new Vector2(), (float) 1f, SpriteEffects.None, 0f);
                for (int j = 0; j < index; j++)
                {
                    for (int n = 0; n < 5; n++)
                    {
                        Color black = Color.Black;
                        int num9 = 170;
                        int num10 = 120 + (j * 30);
                        switch (n)
                        {
                            case 0:
                                num9 -= 2;
                                break;

                            case 1:
                                num9 += 2;
                                break;

                            case 2:
                                num10 -= 2;
                                break;

                            case 3:
                                num10 += 2;
                                break;

                            case 4:
                                black = color2;
                                break;
                        }
                        Vector2 origin = new Vector2();
                        this.spriteBatch.DrawString(fontMouseText, strArray[j], new Vector2((float) num9, (float) num10), black, 0f, origin, (float) 1f, SpriteEffects.None, 0f);
                    }
                }
                r = mouseTextColor;
                color2 = new Color(r, (int) (((double) r) / 1.1), r / 2, r);
                string text = "";
                int price = player[myPlayer].statLifeMax - player[myPlayer].statLife;
                if (player[myPlayer].sign > -1)
                {
                    if (editSign)
                    {
                        text = "Save";
                    }
                    else
                    {
                        text = "Edit";
                    }
                }
                else if (((npc[player[myPlayer].talkNPC].type == 0x11) || (npc[player[myPlayer].talkNPC].type == 0x13)) || ((npc[player[myPlayer].talkNPC].type == 20) || (npc[player[myPlayer].talkNPC].type == 0x26)))
                {
                    text = "Shop";
                }
                else if (npc[player[myPlayer].talkNPC].type == 0x16)
                {
                    text = "Help";
                }
                else if (npc[player[myPlayer].talkNPC].type == 0x12)
                {
                    string str2 = "";
                    int num12 = 0;
                    int num13 = 0;
                    int num14 = 0;
                    int num15 = 0;
                    int num16 = price;
                    if (num16 > 0)
                    {
                        num16 = (int) (num16 * 0.75);
                        if (num16 < 1)
                        {
                            num16 = 1;
                        }
                    }
                    if (num16 < 0)
                    {
                        num16 = 0;
                    }
                    if (num16 >= 0xf4240)
                    {
                        num12 = num16 / 0xf4240;
                        num16 -= num12 * 0xf4240;
                    }
                    if (num16 >= 0x2710)
                    {
                        num13 = num16 / 0x2710;
                        num16 -= num13 * 0x2710;
                    }
                    if (num16 >= 100)
                    {
                        num14 = num16 / 100;
                        num16 -= num14 * 100;
                    }
                    if (num16 >= 1)
                    {
                        num15 = num16;
                    }
                    if (num12 > 0)
                    {
                        str2 = str2 + num12 + " platinum ";
                    }
                    if (num13 > 0)
                    {
                        str2 = str2 + num13 + " gold ";
                    }
                    if (num14 > 0)
                    {
                        str2 = str2 + num14 + " silver ";
                    }
                    if (num15 > 0)
                    {
                        str2 = str2 + num15 + " copper ";
                    }
                    float num17 = ((float) mouseTextColor) / 255f;
                    if (num12 > 0)
                    {
                        color2 = new Color((int) ((byte) (220f * num17)), (int) ((byte) (220f * num17)), (int) ((byte) (198f * num17)), (int) mouseTextColor);
                    }
                    else if (num13 > 0)
                    {
                        color2 = new Color((int) ((byte) (224f * num17)), (int) ((byte) (201f * num17)), (int) ((byte) (92f * num17)), (int) mouseTextColor);
                    }
                    else if (num14 > 0)
                    {
                        color2 = new Color((int) ((byte) (181f * num17)), (int) ((byte) (192f * num17)), (int) ((byte) (193f * num17)), (int) mouseTextColor);
                    }
                    else if (num15 > 0)
                    {
                        color2 = new Color((int) ((byte) (246f * num17)), (int) ((byte) (138f * num17)), (int) ((byte) (96f * num17)), (int) mouseTextColor);
                    }
                    text = "Heal (" + str2 + ")";
                    if (num16 == 0)
                    {
                        text = "Heal";
                    }
                }
                int num18 = 180;
                int num19 = 130 + (index * 30);
                float scale = 0.9f;
                if (((mouseState.X > num18) && (mouseState.X < (num18 + fontMouseText.MeasureString(text).X))) && ((mouseState.Y > num19) && (mouseState.Y < (num19 + fontMouseText.MeasureString(text).Y))))
                {
                    player[myPlayer].mouseInterface = true;
                    scale = 1.1f;
                    if (!npcChatFocus2)
                    {
                        PlaySound(12, -1, -1, 1);
                    }
                    npcChatFocus2 = true;
                    player[myPlayer].releaseUseItem = false;
                }
                else
                {
                    if (npcChatFocus2)
                    {
                        PlaySound(12, -1, -1, 1);
                    }
                    npcChatFocus2 = false;
                }
                for (int k = 0; k < 5; k++)
                {
                    int num22 = num18;
                    int num23 = num19;
                    Color color4 = Color.Black;
                    switch (k)
                    {
                        case 0:
                            num22 -= 2;
                            break;

                        case 1:
                            num22 += 2;
                            break;

                        case 2:
                            num23 -= 2;
                            break;

                        case 3:
                            num23 += 2;
                            break;

                        case 4:
                            color4 = color2;
                            break;
                    }
                    Vector2 vector = (Vector2) (fontMouseText.MeasureString(text) * 0.5f);
                    this.spriteBatch.DrawString(fontMouseText, text, new Vector2(num22 + vector.X, num23 + vector.Y), color4, 0f, vector, scale, SpriteEffects.None, 0f);
                }
                color2 = new Color(r, (int) (((double) r) / 1.1), r / 2, r);
                num18 = (num18 + ((int) fontMouseText.MeasureString(text).X)) + 20;
                num19 = 130 + (index * 30);
                scale = 0.9f;
                if (((mouseState.X > num18) && (mouseState.X < (num18 + fontMouseText.MeasureString("Close").X))) && ((mouseState.Y > num19) && (mouseState.Y < (num19 + fontMouseText.MeasureString("Close").Y))))
                {
                    scale = 1.1f;
                    if (!npcChatFocus1)
                    {
                        PlaySound(12, -1, -1, 1);
                    }
                    npcChatFocus1 = true;
                    player[myPlayer].releaseUseItem = false;
                }
                else
                {
                    if (npcChatFocus1)
                    {
                        PlaySound(12, -1, -1, 1);
                    }
                    npcChatFocus1 = false;
                }
                for (int m = 0; m < 5; m++)
                {
                    int num25 = num18;
                    int num26 = num19;
                    Color color5 = Color.Black;
                    switch (m)
                    {
                        case 0:
                            num25 -= 2;
                            break;

                        case 1:
                            num25 += 2;
                            break;

                        case 2:
                            num26 -= 2;
                            break;

                        case 3:
                            num26 += 2;
                            break;

                        case 4:
                            color5 = color2;
                            break;
                    }
                    Vector2 vector2 = (Vector2) (fontMouseText.MeasureString("Close") * 0.5f);
                    this.spriteBatch.DrawString(fontMouseText, "Close", new Vector2(num25 + vector2.X, num26 + vector2.Y), color5, 0f, vector2, scale, SpriteEffects.None, 0f);
                }
                if ((mouseState.LeftButton == ButtonState.Pressed) && mouseLeftRelease)
                {
                    mouseLeftRelease = false;
                    player[myPlayer].releaseUseItem = false;
                    player[myPlayer].mouseInterface = true;
                    if (npcChatFocus1)
                    {
                        player[myPlayer].talkNPC = -1;
                        player[myPlayer].sign = -1;
                        editSign = false;
                        npcChatText = "";
                        PlaySound(11, -1, -1, 1);
                    }
                    else if (npcChatFocus2)
                    {
                        if (player[myPlayer].sign != -1)
                        {
                            if (!editSign)
                            {
                                PlaySound(12, -1, -1, 1);
                                editSign = true;
                            }
                            else
                            {
                                PlaySound(12, -1, -1, 1);
                                int sign = player[myPlayer].sign;
                                Sign.TextSign(sign, npcChatText);
                                editSign = false;
                                if (netMode == 1)
                                {
                                    NetMessage.SendData(0x2f, -1, -1, "", sign, 0f, 0f, 0f);
                                }
                            }
                        }
                        else if (npc[player[myPlayer].talkNPC].type == 0x11)
                        {
                            playerInventory = true;
                            npcChatText = "";
                            npcShop = 1;
                            PlaySound(12, -1, -1, 1);
                        }
                        else if (npc[player[myPlayer].talkNPC].type == 0x13)
                        {
                            playerInventory = true;
                            npcChatText = "";
                            npcShop = 2;
                            PlaySound(12, -1, -1, 1);
                        }
                        else if (npc[player[myPlayer].talkNPC].type == 20)
                        {
                            playerInventory = true;
                            npcChatText = "";
                            npcShop = 3;
                            PlaySound(12, -1, -1, 1);
                        }
                        else if (npc[player[myPlayer].talkNPC].type == 0x26)
                        {
                            playerInventory = true;
                            npcChatText = "";
                            npcShop = 4;
                            PlaySound(12, -1, -1, 1);
                        }
                        else if (npc[player[myPlayer].talkNPC].type == 0x16)
                        {
                            PlaySound(12, -1, -1, 1);
                            HelpText();
                        }
                        else if (npc[player[myPlayer].talkNPC].type == 0x12)
                        {
                            PlaySound(12, -1, -1, 1);
                            if (price > 0)
                            {
                                if (player[myPlayer].BuyItem(price))
                                {
                                    PlaySound(2, -1, -1, 4);
                                    player[myPlayer].HealEffect(player[myPlayer].statLifeMax - player[myPlayer].statLife);
                                    if (player[myPlayer].statLife < (player[myPlayer].statLifeMax * 0.25))
                                    {
                                        npcChatText = "I managed to sew your face back on. Be more careful next time.";
                                    }
                                    else if (player[myPlayer].statLife < (player[myPlayer].statLifeMax * 0.5))
                                    {
                                        npcChatText = "That's probably going to leave a scar.";
                                    }
                                    else if (player[myPlayer].statLife < (player[myPlayer].statLifeMax * 0.75))
                                    {
                                        npcChatText = "All better. I don't want to see you jumping off anymore cliffs.";
                                    }
                                    else
                                    {
                                        npcChatText = "That didn't hurt too bad, now did it?";
                                    }
                                    player[myPlayer].statLife = player[myPlayer].statLifeMax;
                                }
                                else
                                {
                                    switch (rand.Next(3))
                                    {
                                        case 0:
                                            npcChatText = "I'm sorry, but you can't afford me.";
                                            break;

                                        case 1:
                                            npcChatText = "I'm gonna need more gold then that.";
                                            break;

                                        case 2:
                                            npcChatText = "I don't work for free you know.";
                                            return;
                                    }
                                }
                            }
                            else
                            {
                                switch (rand.Next(3))
                                {
                                    case 0:
                                        npcChatText = "I don't give happy endings.";
                                        break;

                                    case 1:
                                        npcChatText = "I can't do anymore for you without plastic surgery.";
                                        break;

                                    case 2:
                                        npcChatText = "Quit wasting my time.";
                                        break;
                                }
                            }
                        }
                    }
                }
            }
        }

        protected void DrawFPS()
        {
            if (showFrameRate)
            {
                string text = frameRate.ToString();
                this.spriteBatch.DrawString(fontMouseText, text, new Vector2(4f, (float) (screenHeight - 0x18)), new Color(mouseTextColor, mouseTextColor, mouseTextColor, mouseTextColor), 0f, new Vector2(), (float) 1f, SpriteEffects.None, 0f);
            }
        }

        protected void DrawGore()
        {
            for (int i = 0; i < 200; i++)
            {
                if (gore[i].active && (gore[i].type > 0))
                {
                    Color alpha = gore[i].GetAlpha(Lighting.GetColor(((int) (gore[i].position.X + (goreTexture[gore[i].type].Width * 0.5))) / 0x10, (int) ((gore[i].position.Y + (goreTexture[gore[i].type].Height * 0.5)) / 16.0)));
                    this.spriteBatch.Draw(goreTexture[gore[i].type], new Vector2((gore[i].position.X - screenPosition.X) + (goreTexture[gore[i].type].Width / 2), (gore[i].position.Y - screenPosition.Y) + (goreTexture[gore[i].type].Height / 2)), new Rectangle(0, 0, goreTexture[gore[i].type].Width, goreTexture[gore[i].type].Height), alpha, gore[i].rotation, new Vector2((float) (goreTexture[gore[i].type].Width / 2), (float) (goreTexture[gore[i].type].Height / 2)), gore[i].scale, SpriteEffects.None, 0f);
                }
            }
        }

        protected void DrawInterface()
        {
            if (!hideUI)
            {
                Vector2 vector6;
                object obj2;
                Color color22;
                if (signBubble)
                {
                    int num = signX - ((int) screenPosition.X);
                    int num2 = signY - ((int) screenPosition.Y);
                    SpriteEffects none = SpriteEffects.None;
                    if (signX > (player[myPlayer].position.X + player[myPlayer].width))
                    {
                        none = SpriteEffects.FlipHorizontally;
                        num += -8 - chat2Texture.Width;
                    }
                    else
                    {
                        num += 8;
                    }
                    num2 -= 0x16;
                    vector6 = new Vector2();
                    this.spriteBatch.Draw(chat2Texture, new Vector2((float) num, (float) num2), new Rectangle(0, 0, chat2Texture.Width, chat2Texture.Height), new Color(mouseTextColor, mouseTextColor, mouseTextColor, mouseTextColor), 0f, vector6, (float) 1f, none, 0f);
                    signBubble = false;
                }
                for (int i = 0; i < 8; i++)
                {
                    if ((player[i].active && (myPlayer != i)) && !player[i].dead)
                    {
                        new Rectangle((int) ((player[i].position.X + (player[i].width * 0.5)) - 16.0), (int) ((player[i].position.Y + player[i].height) - 48f), 0x20, 0x30);
                        if ((player[myPlayer].team > 0) && (player[myPlayer].team == player[i].team))
                        {
                            new Rectangle((int) screenPosition.X, (int) screenPosition.Y, screenWidth, screenHeight);
                            string name = player[i].name;
                            if (player[i].statLife < player[i].statLifeMax)
                            {
                                obj2 = name;
                                name = string.Concat(new object[] { obj2, ": ", player[i].statLife, "/", player[i].statLifeMax });
                            }
                            Vector2 position = fontMouseText.MeasureString(name);
                            float num4 = 0f;
                            if (player[i].chatShowTime > 0)
                            {
                                num4 = -position.Y;
                            }
                            float num5 = 0f;
                            float num6 = ((float) mouseTextColor) / 255f;
                            Color color = new Color((int) ((byte) (teamColor[player[i].team].R * num6)), (int) ((byte) (teamColor[player[i].team].G * num6)), (int) ((byte) (teamColor[player[i].team].B * num6)), (int) mouseTextColor);
                            Vector2 vector2 = new Vector2((screenWidth / 2) + screenPosition.X, (screenHeight / 2) + screenPosition.Y);
                            float num7 = (player[i].position.X + (player[i].width / 2)) - vector2.X;
                            float num8 = (((player[i].position.Y - position.Y) - 2f) + num4) - vector2.Y;
                            float num9 = (float) Math.Sqrt((double) ((num7 * num7) + (num8 * num8)));
                            if (num9 < 270f)
                            {
                                position.X = ((player[i].position.X + (player[i].width / 2)) - (position.X / 2f)) - screenPosition.X;
                                position.Y = (((player[i].position.Y - position.Y) - 2f) + num4) - screenPosition.Y;
                            }
                            else
                            {
                                num5 = num9;
                                num9 = 270f / num9;
                                position.X = ((screenWidth / 2) + (num7 * num9)) - (position.X / 2f);
                                position.Y = (screenHeight / 2) + (num8 * num9);
                            }
                            if (num5 > 0f)
                            {
                                string text = "(" + ((int) ((num5 / 16f) * 2f)) + " ft)";
                                Vector2 vector3 = fontMouseText.MeasureString(text);
                                vector3.X = (position.X + (fontMouseText.MeasureString(name).X / 2f)) - (vector3.X / 2f);
                                vector3.Y = ((position.Y + (fontMouseText.MeasureString(name).Y / 2f)) - (vector3.Y / 2f)) - 20f;
                                vector6 = new Vector2();
                                this.spriteBatch.DrawString(fontMouseText, text, new Vector2(vector3.X - 2f, vector3.Y), Color.Black, 0f, vector6, (float) 1f, SpriteEffects.None, 0f);
                                vector6 = new Vector2();
                                this.spriteBatch.DrawString(fontMouseText, text, new Vector2(vector3.X + 2f, vector3.Y), Color.Black, 0f, vector6, (float) 1f, SpriteEffects.None, 0f);
                                vector6 = new Vector2();
                                this.spriteBatch.DrawString(fontMouseText, text, new Vector2(vector3.X, vector3.Y - 2f), Color.Black, 0f, vector6, (float) 1f, SpriteEffects.None, 0f);
                                vector6 = new Vector2();
                                this.spriteBatch.DrawString(fontMouseText, text, new Vector2(vector3.X, vector3.Y + 2f), Color.Black, 0f, vector6, (float) 1f, SpriteEffects.None, 0f);
                                vector6 = new Vector2();
                                this.spriteBatch.DrawString(fontMouseText, text, vector3, color, 0f, vector6, (float) 1f, SpriteEffects.None, 0f);
                            }
                            vector6 = new Vector2();
                            this.spriteBatch.DrawString(fontMouseText, name, new Vector2(position.X - 2f, position.Y), Color.Black, 0f, vector6, (float) 1f, SpriteEffects.None, 0f);
                            vector6 = new Vector2();
                            this.spriteBatch.DrawString(fontMouseText, name, new Vector2(position.X + 2f, position.Y), Color.Black, 0f, vector6, (float) 1f, SpriteEffects.None, 0f);
                            vector6 = new Vector2();
                            this.spriteBatch.DrawString(fontMouseText, name, new Vector2(position.X, position.Y - 2f), Color.Black, 0f, vector6, (float) 1f, SpriteEffects.None, 0f);
                            vector6 = new Vector2();
                            this.spriteBatch.DrawString(fontMouseText, name, new Vector2(position.X, position.Y + 2f), Color.Black, 0f, vector6, (float) 1f, SpriteEffects.None, 0f);
                            vector6 = new Vector2();
                            this.spriteBatch.DrawString(fontMouseText, name, position, color, 0f, vector6, (float) 1f, SpriteEffects.None, 0f);
                        }
                    }
                }
                if ((npcChatText != "") || (player[myPlayer].sign != -1))
                {
                    this.DrawChat();
                }
                Color color2 = new Color(200, 200, 200, 200);
                bool flag = false;
                int rare = 0;
                int num11 = player[myPlayer].statLifeMax / 20;
                if (num11 >= 10)
                {
                    num11 = 10;
                }
                vector6 = new Vector2();
                this.spriteBatch.DrawString(fontMouseText, "Life", new Vector2((500 + (13 * num11)) - (fontMouseText.MeasureString("Life").X * 0.5f), 6f), new Color(mouseTextColor, mouseTextColor, mouseTextColor, mouseTextColor), 0f, vector6, (float) 1f, SpriteEffects.None, 0f);
                int num12 = 20;
                for (int j = 1; j < ((player[myPlayer].statLifeMax / num12) + 1); j++)
                {
                    int r = 0xff;
                    float scale = 1f;
                    if (player[myPlayer].statLife >= (j * num12))
                    {
                        r = 0xff;
                    }
                    else
                    {
                        float num16 = ((float) (player[myPlayer].statLife - ((j - 1) * num12))) / ((float) num12);
                        r = (int) (30f + (225f * num16));
                        if (r < 30)
                        {
                            r = 30;
                        }
                        scale = (num16 / 4f) + 0.75f;
                        if (scale < 0.75)
                        {
                            scale = 0.75f;
                        }
                    }
                    int num17 = 0;
                    int num18 = 0;
                    if (j > 10)
                    {
                        num17 -= 260;
                        num18 += 0x1a;
                    }
                    vector6 = new Vector2();
                    this.spriteBatch.Draw(heartTexture, new Vector2((float) ((500 + (0x1a * (j - 1))) + num17), (32f + ((heartTexture.Height - (heartTexture.Height * scale)) / 2f)) + num18), new Rectangle(0, 0, heartTexture.Width, heartTexture.Height), new Color(r, r, r, r), 0f, vector6, scale, SpriteEffects.None, 0f);
                }
                int num19 = 20;
                if (player[myPlayer].statManaMax > 0)
                {
                    int num1 = player[myPlayer].statManaMax / 20;
                    vector6 = new Vector2();
                    this.spriteBatch.DrawString(fontMouseText, "Mana", new Vector2(750f, 6f), new Color(mouseTextColor, mouseTextColor, mouseTextColor, mouseTextColor), 0f, vector6, (float) 1f, SpriteEffects.None, 0f);
                    for (int n = 1; n < ((player[myPlayer].statManaMax / num19) + 1); n++)
                    {
                        int num21 = 0xff;
                        float num22 = 1f;
                        if (player[myPlayer].statMana >= (n * num19))
                        {
                            num21 = 0xff;
                        }
                        else
                        {
                            float num23 = ((float) (player[myPlayer].statMana - ((n - 1) * num19))) / ((float) num19);
                            num21 = (int) (30f + (225f * num23));
                            if (num21 < 30)
                            {
                                num21 = 30;
                            }
                            num22 = (num23 / 4f) + 0.75f;
                            if (num22 < 0.75)
                            {
                                num22 = 0.75f;
                            }
                        }
                        this.spriteBatch.Draw(manaTexture, new Vector2(775f, ((30 + (manaTexture.Height / 2)) + ((manaTexture.Height - (manaTexture.Height * num22)) / 2f)) + (0x1c * (n - 1))), new Rectangle(0, 0, manaTexture.Width, manaTexture.Height), new Color(num21, num21, num21, num21), 0f, new Vector2((float) (manaTexture.Width / 2), (float) (manaTexture.Height / 2)), num22, SpriteEffects.None, 0f);
                    }
                }
                if (player[myPlayer].breath < player[myPlayer].breathMax)
                {
                    int num24 = 0x4c;
                    int num131 = player[myPlayer].breathMax / 20;
                    vector6 = new Vector2();
                    this.spriteBatch.DrawString(fontMouseText, "Breath", new Vector2((500 + (13 * num11)) - (fontMouseText.MeasureString("Breath").X * 0.5f), (float) (6 + num24)), new Color(mouseTextColor, mouseTextColor, mouseTextColor, mouseTextColor), 0f, vector6, (float) 1f, SpriteEffects.None, 0f);
                    int num25 = 20;
                    for (int num26 = 1; num26 < ((player[myPlayer].breathMax / num25) + 1); num26++)
                    {
                        int num27 = 0xff;
                        float num28 = 1f;
                        if (player[myPlayer].breath >= (num26 * num25))
                        {
                            num27 = 0xff;
                        }
                        else
                        {
                            float num29 = ((float) (player[myPlayer].breath - ((num26 - 1) * num25))) / ((float) num25);
                            num27 = (int) (30f + (225f * num29));
                            if (num27 < 30)
                            {
                                num27 = 30;
                            }
                            num28 = (num29 / 4f) + 0.75f;
                            if (num28 < 0.75)
                            {
                                num28 = 0.75f;
                            }
                        }
                        int num30 = 0;
                        int num31 = 0;
                        if (num26 > 10)
                        {
                            num30 -= 260;
                            num31 += 0x1a;
                        }
                        vector6 = new Vector2();
                        this.spriteBatch.Draw(bubbleTexture, new Vector2((float) ((500 + (0x1a * (num26 - 1))) + num30), ((32f + ((bubbleTexture.Height - (bubbleTexture.Height * num28)) / 2f)) + num31) + num24), new Rectangle(0, 0, bubbleTexture.Width, bubbleTexture.Height), new Color(num27, num27, num27, num27), 0f, vector6, num28, SpriteEffects.None, 0f);
                    }
                }
                if (player[myPlayer].dead)
                {
                    playerInventory = false;
                }
                if (!playerInventory)
                {
                    player[myPlayer].chest = -1;
                }
                string cursorText = "";
                if (!playerInventory)
                {
                    bool flag2 = false;
                    bool flag3 = false;
                    for (int num93 = 0; num93 < 3; num93++)
                    {
                        string str6 = "";
                        if ((player[myPlayer].accDepthMeter > 0) && !flag3)
                        {
                            int num94 = (int) ((((player[myPlayer].position.Y + player[myPlayer].height) * 2f) / 16f) - (worldSurface * 2.0));
                            if (num94 > 0)
                            {
                                str6 = "Depth: " + num94 + " feet below";
                                if (num94 == 1)
                                {
                                    str6 = "Depth: " + num94 + " foot below";
                                }
                            }
                            else if (num94 < 0)
                            {
                                num94 *= -1;
                                str6 = "Depth: " + num94 + " feet above";
                                if (num94 == 1)
                                {
                                    str6 = "Depth: " + num94 + " foot above";
                                }
                            }
                            else
                            {
                                str6 = "Depth: Level";
                            }
                            flag3 = true;
                        }
                        else if ((player[myPlayer].accWatch > 0) && !flag2)
                        {
                            string str7 = "AM";
                            double time = Main.time;
                            if (!dayTime)
                            {
                                time += 54000.0;
                            }
                            time = (time / 86400.0) * 24.0;
                            double num96 = 7.5;
                            time = (time - num96) - 12.0;
                            if (time < 0.0)
                            {
                                time += 24.0;
                            }
                            if (time >= 12.0)
                            {
                                str7 = "PM";
                            }
                            int num97 = (int) time;
                            double num98 = time - num97;
                            num98 = (int) (num98 * 60.0);
                            string str8 = num98.ToString();
                            if (num98 < 10.0)
                            {
                                str8 = "0" + str8;
                            }
                            if (num97 > 12)
                            {
                                num97 -= 12;
                            }
                            if (num97 == 0)
                            {
                                num97 = 12;
                            }
                            if (player[myPlayer].accWatch == 1)
                            {
                                str8 = "00";
                            }
                            else if (player[myPlayer].accWatch == 2)
                            {
                                if (num98 < 30.0)
                                {
                                    str8 = "00";
                                }
                                else
                                {
                                    str8 = "30";
                                }
                            }
                            str6 = string.Concat(new object[] { "Time: ", num97, ":", str8, " ", str7 });
                            flag2 = true;
                        }
                        if (str6 != "")
                        {
                            for (int num99 = 0; num99 < 5; num99++)
                            {
                                int num100 = 0;
                                int num101 = 0;
                                Color black = Color.Black;
                                switch (num99)
                                {
                                    case 0:
                                        num100 = -2;
                                        break;

                                    case 1:
                                        num100 = 2;
                                        break;

                                    case 2:
                                        num101 = -2;
                                        break;

                                    case 3:
                                        num101 = 2;
                                        break;

                                    case 4:
                                        black = new Color((int) mouseTextColor, (int) mouseTextColor, (int) mouseTextColor, (int) mouseTextColor);
                                        break;
                                }
                                vector6 = new Vector2();
                                this.spriteBatch.DrawString(fontMouseText, str6, new Vector2((float) (0x16 + num100), (float) ((0x4a + (0x16 * num93)) + num101)), black, 0f, vector6, (float) 1f, SpriteEffects.None, 0f);
                            }
                        }
                    }
                }
                else
                {
                    if (netMode == 1)
                    {
                        int num32 = 0x2a3;
                        int y = 0x86;
                        if (player[myPlayer].hostile)
                        {
                            vector6 = new Vector2();
                            this.spriteBatch.Draw(itemTexture[4], new Vector2((float) (num32 - 2), (float) y), new Rectangle(0, 0, itemTexture[4].Width, itemTexture[4].Height), teamColor[player[myPlayer].team], 0f, vector6, (float) 1f, SpriteEffects.None, 0f);
                            vector6 = new Vector2();
                            this.spriteBatch.Draw(itemTexture[4], new Vector2((float) (num32 + 2), (float) y), new Rectangle(0, 0, itemTexture[4].Width, itemTexture[4].Height), teamColor[player[myPlayer].team], 0f, vector6, (float) 1f, SpriteEffects.FlipHorizontally, 0f);
                        }
                        else
                        {
                            vector6 = new Vector2();
                            this.spriteBatch.Draw(itemTexture[4], new Vector2((float) (num32 - 0x10), (float) (y + 14)), new Rectangle(0, 0, itemTexture[4].Width, itemTexture[4].Height), teamColor[player[myPlayer].team], -0.785f, vector6, (float) 1f, SpriteEffects.None, 0f);
                            vector6 = new Vector2();
                            this.spriteBatch.Draw(itemTexture[4], new Vector2((float) (num32 + 2), (float) (y + 14)), new Rectangle(0, 0, itemTexture[4].Width, itemTexture[4].Height), teamColor[player[myPlayer].team], -0.785f, vector6, (float) 1f, SpriteEffects.None, 0f);
                        }
                        if (((mouseState.X > num32) && (mouseState.X < (num32 + 0x22))) && ((mouseState.Y > (y - 2)) && (mouseState.Y < (y + 0x22))))
                        {
                            player[myPlayer].mouseInterface = true;
                            if ((mouseState.LeftButton == ButtonState.Pressed) && mouseLeftRelease)
                            {
                                PlaySound(12, -1, -1, 1);
                                if (player[myPlayer].hostile)
                                {
                                    player[myPlayer].hostile = false;
                                }
                                else
                                {
                                    player[myPlayer].hostile = true;
                                }
                                NetMessage.SendData(30, -1, -1, "", myPlayer, 0f, 0f, 0f);
                            }
                        }
                        num32 -= 3;
                        Rectangle rectangle = new Rectangle(mouseState.X, mouseState.Y, 1, 1);
                        int width = teamTexture.Width;
                        int height = teamTexture.Height;
                        for (int num36 = 0; num36 < 5; num36++)
                        {
                            Rectangle rectangle2 = new Rectangle();
                            switch (num36)
                            {
                                case 0:
                                    rectangle2 = new Rectangle(num32 + 50, y - 20, width, height);
                                    break;

                                case 1:
                                    rectangle2 = new Rectangle(num32 + 40, y, width, height);
                                    break;

                                case 2:
                                    rectangle2 = new Rectangle(num32 + 60, y, width, height);
                                    break;

                                case 3:
                                    rectangle2 = new Rectangle(num32 + 40, y + 20, width, height);
                                    break;

                                case 4:
                                    rectangle2 = new Rectangle(num32 + 60, y + 20, width, height);
                                    break;
                            }
                            if (rectangle2.Intersects(rectangle))
                            {
                                player[myPlayer].mouseInterface = true;
                                if (((mouseState.LeftButton == ButtonState.Pressed) && mouseLeftRelease) && (player[myPlayer].team != num36))
                                {
                                    PlaySound(12, -1, -1, 1);
                                    player[myPlayer].team = num36;
                                    NetMessage.SendData(0x2d, -1, -1, "", myPlayer, 0f, 0f, 0f);
                                }
                            }
                        }
                        vector6 = new Vector2();
                        this.spriteBatch.Draw(teamTexture, new Vector2((float) (num32 + 50), (float) (y - 20)), new Rectangle(0, 0, teamTexture.Width, teamTexture.Height), teamColor[0], 0f, vector6, (float) 1f, SpriteEffects.None, 0f);
                        vector6 = new Vector2();
                        this.spriteBatch.Draw(teamTexture, new Vector2((float) (num32 + 40), (float) y), new Rectangle(0, 0, teamTexture.Width, teamTexture.Height), teamColor[1], 0f, vector6, (float) 1f, SpriteEffects.None, 0f);
                        vector6 = new Vector2();
                        this.spriteBatch.Draw(teamTexture, new Vector2((float) (num32 + 60), (float) y), new Rectangle(0, 0, teamTexture.Width, teamTexture.Height), teamColor[2], 0f, vector6, (float) 1f, SpriteEffects.None, 0f);
                        vector6 = new Vector2();
                        this.spriteBatch.Draw(teamTexture, new Vector2((float) (num32 + 40), (float) (y + 20)), new Rectangle(0, 0, teamTexture.Width, teamTexture.Height), teamColor[3], 0f, vector6, (float) 1f, SpriteEffects.None, 0f);
                        vector6 = new Vector2();
                        this.spriteBatch.Draw(teamTexture, new Vector2((float) (num32 + 60), (float) (y + 20)), new Rectangle(0, 0, teamTexture.Width, teamTexture.Height), teamColor[4], 0f, vector6, (float) 1f, SpriteEffects.None, 0f);
                    }
                    string str4 = "Save & Exit";
                    if (netMode != 0)
                    {
                        str4 = "Disconnect";
                    }
                    Vector2 vector4 = fontDeathText.MeasureString(str4);
                    int num37 = screenWidth - 110;
                    int num38 = screenHeight - 20;
                    if (mouseExit)
                    {
                        if (exitScale < 1f)
                        {
                            exitScale += 0.02f;
                        }
                    }
                    else if (exitScale > 0.8)
                    {
                        exitScale -= 0.02f;
                    }
                    for (int num39 = 0; num39 < 5; num39++)
                    {
                        int num40 = 0;
                        int num41 = 0;
                        Color white = Color.Black;
                        switch (num39)
                        {
                            case 0:
                                num40 = -2;
                                break;

                            case 1:
                                num40 = 2;
                                break;

                            case 2:
                                num41 = -2;
                                break;

                            case 3:
                                num41 = 2;
                                break;

                            case 4:
                                white = Color.White;
                                break;
                        }
                        this.spriteBatch.DrawString(fontDeathText, str4, new Vector2((float) (num37 + num40), (float) (num38 + num41)), white, 0f, new Vector2(vector4.X / 2f, vector4.Y / 2f), (float) (exitScale - 0.2f), SpriteEffects.None, 0f);
                    }
                    if (((mouseState.X > (num37 - (vector4.X / 2f))) && (mouseState.X < (num37 + (vector4.X / 2f)))) && ((mouseState.Y > (num38 - (vector4.Y / 2f))) && (mouseState.Y < ((num38 + (vector4.Y / 2f)) - 10f))))
                    {
                        if (!mouseExit)
                        {
                            PlaySound(12, -1, -1, 1);
                        }
                        mouseExit = true;
                        player[myPlayer].mouseInterface = true;
                        if (mouseLeftRelease && (mouseState.LeftButton == ButtonState.Pressed))
                        {
                            menuMode = 10;
                            WorldGen.SaveAndQuit();
                        }
                    }
                    else
                    {
                        mouseExit = false;
                    }
                    vector6 = new Vector2();
                    this.spriteBatch.DrawString(fontMouseText, "Inventory", new Vector2(40f, 0f), new Color(mouseTextColor, mouseTextColor, mouseTextColor, mouseTextColor), 0f, vector6, (float) 1f, SpriteEffects.None, 0f);
                    inventoryScale = 0.85f;
                    for (int num42 = 0; num42 < 10; num42++)
                    {
                        for (int num43 = 0; num43 < 4; num43++)
                        {
                            int num44 = (int) (20f + ((num42 * 0x38) * inventoryScale));
                            int num45 = (int) (20f + ((num43 * 0x38) * inventoryScale));
                            int index = num42 + (num43 * 10);
                            Color newColor = new Color(100, 100, 100, 100);
                            if (((mouseState.X >= num44) && (mouseState.X <= (num44 + (inventoryBackTexture.Width * inventoryScale)))) && ((mouseState.Y >= num45) && (mouseState.Y <= (num45 + (inventoryBackTexture.Height * inventoryScale)))))
                            {
                                player[myPlayer].mouseInterface = true;
                                if (mouseLeftRelease && (mouseState.LeftButton == ButtonState.Pressed))
                                {
                                    if ((player[myPlayer].selectedItem != index) || (player[myPlayer].itemAnimation <= 0))
                                    {
                                        Item mouseItem = Main.mouseItem;
                                        Main.mouseItem = player[myPlayer].inventory[index];
                                        player[myPlayer].inventory[index] = mouseItem;
                                        if ((player[myPlayer].inventory[index].type == 0) || (player[myPlayer].inventory[index].stack < 1))
                                        {
                                            player[myPlayer].inventory[index] = new Item();
                                        }
                                        if ((Main.mouseItem.IsTheSameAs(player[myPlayer].inventory[index]) && (player[myPlayer].inventory[index].stack != player[myPlayer].inventory[index].maxStack)) && (Main.mouseItem.stack != Main.mouseItem.maxStack))
                                        {
                                            if ((Main.mouseItem.stack + player[myPlayer].inventory[index].stack) <= Main.mouseItem.maxStack)
                                            {
                                                Item item1 = player[myPlayer].inventory[index];
                                                item1.stack += Main.mouseItem.stack;
                                                Main.mouseItem.stack = 0;
                                            }
                                            else
                                            {
                                                int num47 = Main.mouseItem.maxStack - player[myPlayer].inventory[index].stack;
                                                Item item6 = player[myPlayer].inventory[index];
                                                item6.stack += num47;
                                                Main.mouseItem.stack -= num47;
                                            }
                                        }
                                        if ((Main.mouseItem.type == 0) || (Main.mouseItem.stack < 1))
                                        {
                                            Main.mouseItem = new Item();
                                        }
                                        if ((Main.mouseItem.type > 0) || (player[myPlayer].inventory[index].type > 0))
                                        {
                                            Recipe.FindRecipes();
                                            PlaySound(7, -1, -1, 1);
                                        }
                                    }
                                }
                                else if ((((stackSplit <= 1) && (mouseState.RightButton == ButtonState.Pressed)) && (Main.mouseItem.IsTheSameAs(player[myPlayer].inventory[index]) || (Main.mouseItem.type == 0))) && ((Main.mouseItem.stack < Main.mouseItem.maxStack) || (Main.mouseItem.type == 0)))
                                {
                                    if (Main.mouseItem.type == 0)
                                    {
                                        Main.mouseItem = (Item) player[myPlayer].inventory[index].Clone();
                                        Main.mouseItem.stack = 0;
                                    }
                                    Main.mouseItem.stack++;
                                    Item item7 = player[myPlayer].inventory[index];
                                    item7.stack--;
                                    if (player[myPlayer].inventory[index].stack <= 0)
                                    {
                                        player[myPlayer].inventory[index] = new Item();
                                    }
                                    Recipe.FindRecipes();
                                    soundInstanceMenuTick.Stop();
                                    soundInstanceMenuTick = soundMenuTick.CreateInstance();
                                    PlaySound(12, -1, -1, 1);
                                    if (stackSplit == 0)
                                    {
                                        stackSplit = 15;
                                    }
                                    else
                                    {
                                        stackSplit = stackDelay;
                                    }
                                }
                                cursorText = player[myPlayer].inventory[index].name;
                                toolTip = (Item) player[myPlayer].inventory[index].Clone();
                                if (player[myPlayer].inventory[index].stack > 1)
                                {
                                    obj2 = cursorText;
                                    cursorText = string.Concat(new object[] { obj2, " (", player[myPlayer].inventory[index].stack, ")" });
                                }
                            }
                            vector6 = new Vector2();
                            this.spriteBatch.Draw(inventoryBackTexture, new Vector2((float) num44, (float) num45), new Rectangle(0, 0, inventoryBackTexture.Width, inventoryBackTexture.Height), color2, 0f, vector6, inventoryScale, SpriteEffects.None, 0f);
                            newColor = Color.White;
                            if ((player[myPlayer].inventory[index].type > 0) && (player[myPlayer].inventory[index].stack > 0))
                            {
                                float num48 = 1f;
                                if ((itemTexture[player[myPlayer].inventory[index].type].Width > 0x20) || (itemTexture[player[myPlayer].inventory[index].type].Height > 0x20))
                                {
                                    if (itemTexture[player[myPlayer].inventory[index].type].Width > itemTexture[player[myPlayer].inventory[index].type].Height)
                                    {
                                        num48 = 32f / ((float) itemTexture[player[myPlayer].inventory[index].type].Width);
                                    }
                                    else
                                    {
                                        num48 = 32f / ((float) itemTexture[player[myPlayer].inventory[index].type].Height);
                                    }
                                }
                                num48 *= inventoryScale;
                                vector6 = new Vector2();
                                this.spriteBatch.Draw(itemTexture[player[myPlayer].inventory[index].type], new Vector2((num44 + (26f * inventoryScale)) - ((itemTexture[player[myPlayer].inventory[index].type].Width * 0.5f) * num48), (num45 + (26f * inventoryScale)) - ((itemTexture[player[myPlayer].inventory[index].type].Height * 0.5f) * num48)), new Rectangle(0, 0, itemTexture[player[myPlayer].inventory[index].type].Width, itemTexture[player[myPlayer].inventory[index].type].Height), player[myPlayer].inventory[index].GetAlpha(newColor), 0f, vector6, num48, SpriteEffects.None, 0f);
                                color22 = new Color();
                                if (player[myPlayer].inventory[index].color != color22)
                                {
                                    vector6 = new Vector2();
                                    this.spriteBatch.Draw(itemTexture[player[myPlayer].inventory[index].type], new Vector2((num44 + (26f * inventoryScale)) - ((itemTexture[player[myPlayer].inventory[index].type].Width * 0.5f) * num48), (num45 + (26f * inventoryScale)) - ((itemTexture[player[myPlayer].inventory[index].type].Height * 0.5f) * num48)), new Rectangle(0, 0, itemTexture[player[myPlayer].inventory[index].type].Width, itemTexture[player[myPlayer].inventory[index].type].Height), player[myPlayer].inventory[index].GetColor(newColor), 0f, vector6, num48, SpriteEffects.None, 0f);
                                }
                                if (player[myPlayer].inventory[index].stack > 1)
                                {
                                    vector6 = new Vector2();
                                    this.spriteBatch.DrawString(fontItemStack, player[myPlayer].inventory[index].stack.ToString(), new Vector2(num44 + (10f * inventoryScale), num45 + (26f * inventoryScale)), newColor, 0f, vector6, num48, SpriteEffects.None, 0f);
                                }
                            }
                        }
                    }
                    for (int num49 = 0; num49 < 8; num49++)
                    {
                        int num50 = (screenWidth - 0x40) - 0x1c;
                        int num51 = (int) (174f + ((num49 * 0x38) * inventoryScale));
                        Color color5 = new Color(100, 100, 100, 100);
                        string str5 = "";
                        switch (num49)
                        {
                            case 0:
                                str5 = "Helmet";
                                break;

                            case 1:
                                str5 = "Shirt";
                                break;

                            case 2:
                                str5 = "Pants";
                                break;

                            case 3:
                                str5 = "Accessories";
                                break;
                        }
                        Vector2 vector5 = fontMouseText.MeasureString(str5);
                        vector6 = new Vector2();
                        this.spriteBatch.DrawString(fontMouseText, str5, new Vector2((num50 - vector5.X) - 10f, (num51 + (inventoryBackTexture.Height * 0.5f)) - (vector5.Y * 0.5f)), new Color(mouseTextColor, mouseTextColor, mouseTextColor, mouseTextColor), 0f, vector6, (float) 1f, SpriteEffects.None, 0f);
                        if (((mouseState.X >= num50) && (mouseState.X <= (num50 + (inventoryBackTexture.Width * inventoryScale)))) && ((mouseState.Y >= num51) && (mouseState.Y <= (num51 + (inventoryBackTexture.Height * inventoryScale)))))
                        {
                            player[myPlayer].mouseInterface = true;
                            if ((mouseLeftRelease && (mouseState.LeftButton == ButtonState.Pressed)) && ((((Main.mouseItem.type == 0) || ((Main.mouseItem.headSlot > -1) && (num49 == 0))) || ((Main.mouseItem.bodySlot > -1) && (num49 == 1))) || (((Main.mouseItem.legSlot > -1) && (num49 == 2)) || (Main.mouseItem.accessory && (num49 > 2)))))
                            {
                                Item item2 = Main.mouseItem;
                                Main.mouseItem = player[myPlayer].armor[num49];
                                player[myPlayer].armor[num49] = item2;
                                if ((player[myPlayer].armor[num49].type == 0) || (player[myPlayer].armor[num49].stack < 1))
                                {
                                    player[myPlayer].armor[num49] = new Item();
                                }
                                if ((Main.mouseItem.type == 0) || (Main.mouseItem.stack < 1))
                                {
                                    Main.mouseItem = new Item();
                                }
                                if ((Main.mouseItem.type > 0) || (player[myPlayer].armor[num49].type > 0))
                                {
                                    Recipe.FindRecipes();
                                    PlaySound(7, -1, -1, 1);
                                }
                            }
                            cursorText = player[myPlayer].armor[num49].name;
                            toolTip = (Item) player[myPlayer].armor[num49].Clone();
                            if (num49 <= 2)
                            {
                                toolTip.wornArmor = true;
                            }
                            if (player[myPlayer].armor[num49].stack > 1)
                            {
                                obj2 = cursorText;
                                cursorText = string.Concat(new object[] { obj2, " (", player[myPlayer].armor[num49].stack, ")" });
                            }
                        }
                        vector6 = new Vector2();
                        this.spriteBatch.Draw(inventoryBackTexture, new Vector2((float) num50, (float) num51), new Rectangle(0, 0, inventoryBackTexture.Width, inventoryBackTexture.Height), color2, 0f, vector6, inventoryScale, SpriteEffects.None, 0f);
                        color5 = Color.White;
                        if ((player[myPlayer].armor[num49].type > 0) && (player[myPlayer].armor[num49].stack > 0))
                        {
                            float num52 = 1f;
                            if ((itemTexture[player[myPlayer].armor[num49].type].Width > 0x20) || (itemTexture[player[myPlayer].armor[num49].type].Height > 0x20))
                            {
                                if (itemTexture[player[myPlayer].armor[num49].type].Width > itemTexture[player[myPlayer].armor[num49].type].Height)
                                {
                                    num52 = 32f / ((float) itemTexture[player[myPlayer].armor[num49].type].Width);
                                }
                                else
                                {
                                    num52 = 32f / ((float) itemTexture[player[myPlayer].armor[num49].type].Height);
                                }
                            }
                            num52 *= inventoryScale;
                            vector6 = new Vector2();
                            this.spriteBatch.Draw(itemTexture[player[myPlayer].armor[num49].type], new Vector2((num50 + (26f * inventoryScale)) - ((itemTexture[player[myPlayer].armor[num49].type].Width * 0.5f) * num52), (num51 + (26f * inventoryScale)) - ((itemTexture[player[myPlayer].armor[num49].type].Height * 0.5f) * num52)), new Rectangle(0, 0, itemTexture[player[myPlayer].armor[num49].type].Width, itemTexture[player[myPlayer].armor[num49].type].Height), player[myPlayer].armor[num49].GetAlpha(color5), 0f, vector6, num52, SpriteEffects.None, 0f);
                            color22 = new Color();
                            if (player[myPlayer].armor[num49].color != color22)
                            {
                                vector6 = new Vector2();
                                this.spriteBatch.Draw(itemTexture[player[myPlayer].armor[num49].type], new Vector2((num50 + (26f * inventoryScale)) - ((itemTexture[player[myPlayer].armor[num49].type].Width * 0.5f) * num52), (num51 + (26f * inventoryScale)) - ((itemTexture[player[myPlayer].armor[num49].type].Height * 0.5f) * num52)), new Rectangle(0, 0, itemTexture[player[myPlayer].armor[num49].type].Width, itemTexture[player[myPlayer].armor[num49].type].Height), player[myPlayer].armor[num49].GetColor(color5), 0f, vector6, num52, SpriteEffects.None, 0f);
                            }
                            if (player[myPlayer].armor[num49].stack > 1)
                            {
                                vector6 = new Vector2();
                                this.spriteBatch.DrawString(fontItemStack, player[myPlayer].armor[num49].stack.ToString(), new Vector2(num50 + (10f * inventoryScale), num51 + (26f * inventoryScale)), color5, 0f, vector6, num52, SpriteEffects.None, 0f);
                            }
                        }
                    }
                    vector6 = new Vector2();
                    this.spriteBatch.DrawString(fontMouseText, "Crafting", new Vector2(76f, 414f), new Color(mouseTextColor, mouseTextColor, mouseTextColor, mouseTextColor), 0f, vector6, (float) 1f, SpriteEffects.None, 0f);
                    for (int num53 = 0; num53 < Recipe.maxRecipes; num53++)
                    {
                        inventoryScale = 100f / (Math.Abs(availableRecipeY[num53]) + 100f);
                        if (inventoryScale < 0.75)
                        {
                            inventoryScale = 0.75f;
                        }
                        if (availableRecipeY[num53] < ((num53 - focusRecipe) * 0x41))
                        {
                            if (availableRecipeY[num53] == 0f)
                            {
                                PlaySound(12, -1, -1, 1);
                            }
                            availableRecipeY[num53] += 6.5f;
                        }
                        else if (availableRecipeY[num53] > ((num53 - focusRecipe) * 0x41))
                        {
                            if (availableRecipeY[num53] == 0f)
                            {
                                PlaySound(12, -1, -1, 1);
                            }
                            availableRecipeY[num53] -= 6.5f;
                        }
                        if ((num53 < numAvailableRecipes) && (Math.Abs(availableRecipeY[num53]) <= 250f))
                        {
                            int num54 = (int) (46f - (26f * inventoryScale));
                            int num55 = (int) ((410f + (availableRecipeY[num53] * inventoryScale)) - (30f * inventoryScale));
                            double num56 = color2.A + 50;
                            double num57 = 255.0;
                            if (Math.Abs(availableRecipeY[num53]) > 150f)
                            {
                                num56 = (150f * (100f - (Math.Abs(availableRecipeY[num53]) - 150f))) * 0.01;
                                num57 = (255f * (100f - (Math.Abs(availableRecipeY[num53]) - 150f))) * 0.01;
                            }
                            Color color6 = new Color((int) ((byte) num56), (int) ((byte) num56), (int) ((byte) num56), (int) ((byte) num56));
                            Color color7 = new Color((int) ((byte) num57), (int) ((byte) num57), (int) ((byte) num57), (int) ((byte) num57));
                            if (((mouseState.X >= num54) && (mouseState.X <= (num54 + (inventoryBackTexture.Width * inventoryScale)))) && ((mouseState.Y >= num55) && (mouseState.Y <= (num55 + (inventoryBackTexture.Height * inventoryScale)))))
                            {
                                player[myPlayer].mouseInterface = true;
                                if (focusRecipe == num53)
                                {
                                    if ((Main.mouseItem.type == 0) || (Main.mouseItem.IsTheSameAs(recipe[availableRecipe[num53]].createItem) && ((Main.mouseItem.stack + recipe[availableRecipe[num53]].createItem.stack) <= Main.mouseItem.maxStack)))
                                    {
                                        if (mouseLeftRelease && (mouseState.LeftButton == ButtonState.Pressed))
                                        {
                                            int stack = Main.mouseItem.stack;
                                            Main.mouseItem = (Item) recipe[availableRecipe[num53]].createItem.Clone();
                                            Main.mouseItem.stack += stack;
                                            recipe[availableRecipe[num53]].Create();
                                            if ((Main.mouseItem.type > 0) || (recipe[availableRecipe[num53]].createItem.type > 0))
                                            {
                                                PlaySound(7, -1, -1, 1);
                                            }
                                        }
                                        else if (((stackSplit <= 1) && (mouseState.RightButton == ButtonState.Pressed)) && ((Main.mouseItem.stack < Main.mouseItem.maxStack) || (Main.mouseItem.type == 0)))
                                        {
                                            if (stackSplit == 0)
                                            {
                                                stackSplit = 15;
                                            }
                                            else
                                            {
                                                stackSplit = stackDelay;
                                            }
                                            int num59 = Main.mouseItem.stack;
                                            Main.mouseItem = (Item) recipe[availableRecipe[num53]].createItem.Clone();
                                            Main.mouseItem.stack += num59;
                                            recipe[availableRecipe[num53]].Create();
                                            if ((Main.mouseItem.type > 0) || (recipe[availableRecipe[num53]].createItem.type > 0))
                                            {
                                                PlaySound(7, -1, -1, 1);
                                            }
                                        }
                                    }
                                }
                                else if (mouseLeftRelease && (mouseState.LeftButton == ButtonState.Pressed))
                                {
                                    focusRecipe = num53;
                                }
                                cursorText = recipe[availableRecipe[num53]].createItem.name;
                                toolTip = (Item) recipe[availableRecipe[num53]].createItem.Clone();
                                if (recipe[availableRecipe[num53]].createItem.stack > 1)
                                {
                                    obj2 = cursorText;
                                    cursorText = string.Concat(new object[] { obj2, " (", recipe[availableRecipe[num53]].createItem.stack, ")" });
                                }
                            }
                            if (numAvailableRecipes > 0)
                            {
                                num56 -= 50.0;
                                if (num56 < 0.0)
                                {
                                    num56 = 0.0;
                                }
                                vector6 = new Vector2();
                                this.spriteBatch.Draw(inventoryBackTexture, new Vector2((float) num54, (float) num55), new Rectangle(0, 0, inventoryBackTexture.Width, inventoryBackTexture.Height), new Color((byte) num56, (byte) num56, (byte) num56, (byte) num56), 0f, vector6, inventoryScale, SpriteEffects.None, 0f);
                                if ((recipe[availableRecipe[num53]].createItem.type > 0) && (recipe[availableRecipe[num53]].createItem.stack > 0))
                                {
                                    float num60 = 1f;
                                    if ((itemTexture[recipe[availableRecipe[num53]].createItem.type].Width > 0x20) || (itemTexture[recipe[availableRecipe[num53]].createItem.type].Height > 0x20))
                                    {
                                        if (itemTexture[recipe[availableRecipe[num53]].createItem.type].Width > itemTexture[recipe[availableRecipe[num53]].createItem.type].Height)
                                        {
                                            num60 = 32f / ((float) itemTexture[recipe[availableRecipe[num53]].createItem.type].Width);
                                        }
                                        else
                                        {
                                            num60 = 32f / ((float) itemTexture[recipe[availableRecipe[num53]].createItem.type].Height);
                                        }
                                    }
                                    num60 *= inventoryScale;
                                    vector6 = new Vector2();
                                    this.spriteBatch.Draw(itemTexture[recipe[availableRecipe[num53]].createItem.type], new Vector2((num54 + (26f * inventoryScale)) - ((itemTexture[recipe[availableRecipe[num53]].createItem.type].Width * 0.5f) * num60), (num55 + (26f * inventoryScale)) - ((itemTexture[recipe[availableRecipe[num53]].createItem.type].Height * 0.5f) * num60)), new Rectangle(0, 0, itemTexture[recipe[availableRecipe[num53]].createItem.type].Width, itemTexture[recipe[availableRecipe[num53]].createItem.type].Height), recipe[availableRecipe[num53]].createItem.GetAlpha(color7), 0f, vector6, num60, SpriteEffects.None, 0f);
                                    color22 = new Color();
                                    if (recipe[availableRecipe[num53]].createItem.color != color22)
                                    {
                                        vector6 = new Vector2();
                                        this.spriteBatch.Draw(itemTexture[recipe[availableRecipe[num53]].createItem.type], new Vector2((num54 + (26f * inventoryScale)) - ((itemTexture[recipe[availableRecipe[num53]].createItem.type].Width * 0.5f) * num60), (num55 + (26f * inventoryScale)) - ((itemTexture[recipe[availableRecipe[num53]].createItem.type].Height * 0.5f) * num60)), new Rectangle(0, 0, itemTexture[recipe[availableRecipe[num53]].createItem.type].Width, itemTexture[recipe[availableRecipe[num53]].createItem.type].Height), recipe[availableRecipe[num53]].createItem.GetColor(color7), 0f, vector6, num60, SpriteEffects.None, 0f);
                                    }
                                    if (recipe[availableRecipe[num53]].createItem.stack > 1)
                                    {
                                        vector6 = new Vector2();
                                        this.spriteBatch.DrawString(fontItemStack, recipe[availableRecipe[num53]].createItem.stack.ToString(), new Vector2(num54 + (10f * inventoryScale), num55 + (26f * inventoryScale)), color6, 0f, vector6, num60, SpriteEffects.None, 0f);
                                    }
                                }
                            }
                        }
                    }
                    if (numAvailableRecipes > 0)
                    {
                        for (int num61 = 0; num61 < Recipe.maxRequirements; num61++)
                        {
                            if (recipe[availableRecipe[focusRecipe]].requiredItem[num61].type == 0)
                            {
                                break;
                            }
                            int num62 = 80 + (num61 * 40);
                            int num63 = 380;
                            double num64 = color2.A + 50;
                            double num65 = 255.0;
                            Color color8 = Color.White;
                            Color color9 = Color.White;
                            num64 = (color2.A + 50) - (Math.Abs(availableRecipeY[focusRecipe]) * 2f);
                            num65 = 255f - (Math.Abs(availableRecipeY[focusRecipe]) * 2f);
                            if (num64 < 0.0)
                            {
                                num64 = 0.0;
                            }
                            if (num65 < 0.0)
                            {
                                num65 = 0.0;
                            }
                            color8.R = (byte) num64;
                            color8.G = (byte) num64;
                            color8.B = (byte) num64;
                            color8.A = (byte) num64;
                            color9.R = (byte) num65;
                            color9.G = (byte) num65;
                            color9.B = (byte) num65;
                            color9.A = (byte) num65;
                            inventoryScale = 0.6f;
                            if (num64 == 0.0)
                            {
                                break;
                            }
                            if (((mouseState.X >= num62) && (mouseState.X <= (num62 + (inventoryBackTexture.Width * inventoryScale)))) && ((mouseState.Y >= num63) && (mouseState.Y <= (num63 + (inventoryBackTexture.Height * inventoryScale)))))
                            {
                                player[myPlayer].mouseInterface = true;
                                cursorText = recipe[availableRecipe[focusRecipe]].requiredItem[num61].name;
                                toolTip = (Item) recipe[availableRecipe[focusRecipe]].requiredItem[num61].Clone();
                                if (recipe[availableRecipe[focusRecipe]].requiredItem[num61].stack > 1)
                                {
                                    obj2 = cursorText;
                                    cursorText = string.Concat(new object[] { obj2, " (", recipe[availableRecipe[focusRecipe]].requiredItem[num61].stack, ")" });
                                }
                            }
                            num64 -= 50.0;
                            if (num64 < 0.0)
                            {
                                num64 = 0.0;
                            }
                            vector6 = new Vector2();
                            this.spriteBatch.Draw(inventoryBackTexture, new Vector2((float) num62, (float) num63), new Rectangle(0, 0, inventoryBackTexture.Width, inventoryBackTexture.Height), new Color((byte) num64, (byte) num64, (byte) num64, (byte) num64), 0f, vector6, inventoryScale, SpriteEffects.None, 0f);
                            if ((recipe[availableRecipe[focusRecipe]].requiredItem[num61].type > 0) && (recipe[availableRecipe[focusRecipe]].requiredItem[num61].stack > 0))
                            {
                                float num66 = 1f;
                                if ((itemTexture[recipe[availableRecipe[focusRecipe]].requiredItem[num61].type].Width > 0x20) || (itemTexture[recipe[availableRecipe[focusRecipe]].requiredItem[num61].type].Height > 0x20))
                                {
                                    if (itemTexture[recipe[availableRecipe[focusRecipe]].requiredItem[num61].type].Width > itemTexture[recipe[availableRecipe[focusRecipe]].requiredItem[num61].type].Height)
                                    {
                                        num66 = 32f / ((float) itemTexture[recipe[availableRecipe[focusRecipe]].requiredItem[num61].type].Width);
                                    }
                                    else
                                    {
                                        num66 = 32f / ((float) itemTexture[recipe[availableRecipe[focusRecipe]].requiredItem[num61].type].Height);
                                    }
                                }
                                num66 *= inventoryScale;
                                vector6 = new Vector2();
                                this.spriteBatch.Draw(itemTexture[recipe[availableRecipe[focusRecipe]].requiredItem[num61].type], new Vector2((num62 + (26f * inventoryScale)) - ((itemTexture[recipe[availableRecipe[focusRecipe]].requiredItem[num61].type].Width * 0.5f) * num66), (num63 + (26f * inventoryScale)) - ((itemTexture[recipe[availableRecipe[focusRecipe]].requiredItem[num61].type].Height * 0.5f) * num66)), new Rectangle(0, 0, itemTexture[recipe[availableRecipe[focusRecipe]].requiredItem[num61].type].Width, itemTexture[recipe[availableRecipe[focusRecipe]].requiredItem[num61].type].Height), recipe[availableRecipe[focusRecipe]].requiredItem[num61].GetAlpha(color9), 0f, vector6, num66, SpriteEffects.None, 0f);
                                color22 = new Color();
                                if (recipe[availableRecipe[focusRecipe]].requiredItem[num61].color != color22)
                                {
                                    vector6 = new Vector2();
                                    this.spriteBatch.Draw(itemTexture[recipe[availableRecipe[focusRecipe]].requiredItem[num61].type], new Vector2((num62 + (26f * inventoryScale)) - ((itemTexture[recipe[availableRecipe[focusRecipe]].requiredItem[num61].type].Width * 0.5f) * num66), (num63 + (26f * inventoryScale)) - ((itemTexture[recipe[availableRecipe[focusRecipe]].requiredItem[num61].type].Height * 0.5f) * num66)), new Rectangle(0, 0, itemTexture[recipe[availableRecipe[focusRecipe]].requiredItem[num61].type].Width, itemTexture[recipe[availableRecipe[focusRecipe]].requiredItem[num61].type].Height), recipe[availableRecipe[focusRecipe]].requiredItem[num61].GetColor(color9), 0f, vector6, num66, SpriteEffects.None, 0f);
                                }
                                if (recipe[availableRecipe[focusRecipe]].requiredItem[num61].stack > 1)
                                {
                                    vector6 = new Vector2();
                                    this.spriteBatch.DrawString(fontItemStack, recipe[availableRecipe[focusRecipe]].requiredItem[num61].stack.ToString(), new Vector2(num62 + (10f * inventoryScale), num63 + (26f * inventoryScale)), color8, 0f, vector6, num66, SpriteEffects.None, 0f);
                                }
                            }
                        }
                    }
                    vector6 = new Vector2();
                    this.spriteBatch.DrawString(fontMouseText, "Coins", new Vector2(528f, 84f), new Color(mouseTextColor, mouseTextColor, mouseTextColor, mouseTextColor), 0f, vector6, (float) 0.8f, SpriteEffects.None, 0f);
                    inventoryScale = 0.55f;
                    for (int num67 = 0; num67 < 4; num67++)
                    {
                        int num68 = 0x1f1;
                        int num69 = (int) (85f + ((num67 * 0x38) * inventoryScale));
                        int num70 = num67 + 40;
                        Color color10 = new Color(100, 100, 100, 100);
                        if (((mouseState.X >= num68) && (mouseState.X <= (num68 + (inventoryBackTexture.Width * inventoryScale)))) && ((mouseState.Y >= num69) && (mouseState.Y <= (num69 + (inventoryBackTexture.Height * inventoryScale)))))
                        {
                            player[myPlayer].mouseInterface = true;
                            if (mouseLeftRelease && (mouseState.LeftButton == ButtonState.Pressed))
                            {
                                if (((player[myPlayer].selectedItem != num70) || (player[myPlayer].itemAnimation <= 0)) && (((Main.mouseItem.type == 0) || (Main.mouseItem.type == 0x47)) || (((Main.mouseItem.type == 0x48) || (Main.mouseItem.type == 0x49)) || (Main.mouseItem.type == 0x4a))))
                                {
                                    Item item3 = Main.mouseItem;
                                    Main.mouseItem = player[myPlayer].inventory[num70];
                                    player[myPlayer].inventory[num70] = item3;
                                    if ((player[myPlayer].inventory[num70].type == 0) || (player[myPlayer].inventory[num70].stack < 1))
                                    {
                                        player[myPlayer].inventory[num70] = new Item();
                                    }
                                    if ((Main.mouseItem.IsTheSameAs(player[myPlayer].inventory[num70]) && (player[myPlayer].inventory[num70].stack != player[myPlayer].inventory[num70].maxStack)) && (Main.mouseItem.stack != Main.mouseItem.maxStack))
                                    {
                                        if ((Main.mouseItem.stack + player[myPlayer].inventory[num70].stack) <= Main.mouseItem.maxStack)
                                        {
                                            Item item8 = player[myPlayer].inventory[num70];
                                            item8.stack += Main.mouseItem.stack;
                                            Main.mouseItem.stack = 0;
                                        }
                                        else
                                        {
                                            int num71 = Main.mouseItem.maxStack - player[myPlayer].inventory[num70].stack;
                                            Item item9 = player[myPlayer].inventory[num70];
                                            item9.stack += num71;
                                            Main.mouseItem.stack -= num71;
                                        }
                                    }
                                    if ((Main.mouseItem.type == 0) || (Main.mouseItem.stack < 1))
                                    {
                                        Main.mouseItem = new Item();
                                    }
                                    if ((Main.mouseItem.type > 0) || (player[myPlayer].inventory[num70].type > 0))
                                    {
                                        PlaySound(7, -1, -1, 1);
                                    }
                                }
                            }
                            else if ((((stackSplit <= 1) && (mouseState.RightButton == ButtonState.Pressed)) && (Main.mouseItem.IsTheSameAs(player[myPlayer].inventory[num70]) || (Main.mouseItem.type == 0))) && ((Main.mouseItem.stack < Main.mouseItem.maxStack) || (Main.mouseItem.type == 0)))
                            {
                                if (Main.mouseItem.type == 0)
                                {
                                    Main.mouseItem = (Item) player[myPlayer].inventory[num70].Clone();
                                    Main.mouseItem.stack = 0;
                                }
                                Main.mouseItem.stack++;
                                Item item10 = player[myPlayer].inventory[num70];
                                item10.stack--;
                                if (player[myPlayer].inventory[num70].stack <= 0)
                                {
                                    player[myPlayer].inventory[num70] = new Item();
                                }
                                Recipe.FindRecipes();
                                soundInstanceMenuTick.Stop();
                                soundInstanceMenuTick = soundMenuTick.CreateInstance();
                                PlaySound(12, -1, -1, 1);
                                if (stackSplit == 0)
                                {
                                    stackSplit = 15;
                                }
                                else
                                {
                                    stackSplit = stackDelay;
                                }
                            }
                            cursorText = player[myPlayer].inventory[num70].name;
                            toolTip = (Item) player[myPlayer].inventory[num70].Clone();
                            if (player[myPlayer].inventory[num70].stack > 1)
                            {
                                obj2 = cursorText;
                                cursorText = string.Concat(new object[] { obj2, " (", player[myPlayer].inventory[num70].stack, ")" });
                            }
                        }
                        vector6 = new Vector2();
                        this.spriteBatch.Draw(inventoryBackTexture, new Vector2((float) num68, (float) num69), new Rectangle(0, 0, inventoryBackTexture.Width, inventoryBackTexture.Height), color2, 0f, vector6, inventoryScale, SpriteEffects.None, 0f);
                        color10 = Color.White;
                        if ((player[myPlayer].inventory[num70].type > 0) && (player[myPlayer].inventory[num70].stack > 0))
                        {
                            float num72 = 1f;
                            if ((itemTexture[player[myPlayer].inventory[num70].type].Width > 0x20) || (itemTexture[player[myPlayer].inventory[num70].type].Height > 0x20))
                            {
                                if (itemTexture[player[myPlayer].inventory[num70].type].Width > itemTexture[player[myPlayer].inventory[num70].type].Height)
                                {
                                    num72 = 32f / ((float) itemTexture[player[myPlayer].inventory[num70].type].Width);
                                }
                                else
                                {
                                    num72 = 32f / ((float) itemTexture[player[myPlayer].inventory[num70].type].Height);
                                }
                            }
                            num72 *= inventoryScale;
                            vector6 = new Vector2();
                            this.spriteBatch.Draw(itemTexture[player[myPlayer].inventory[num70].type], new Vector2((num68 + (26f * inventoryScale)) - ((itemTexture[player[myPlayer].inventory[num70].type].Width * 0.5f) * num72), (num69 + (26f * inventoryScale)) - ((itemTexture[player[myPlayer].inventory[num70].type].Height * 0.5f) * num72)), new Rectangle(0, 0, itemTexture[player[myPlayer].inventory[num70].type].Width, itemTexture[player[myPlayer].inventory[num70].type].Height), player[myPlayer].inventory[num70].GetAlpha(color10), 0f, vector6, num72, SpriteEffects.None, 0f);
                            color22 = new Color();
                            if (player[myPlayer].inventory[num70].color != color22)
                            {
                                vector6 = new Vector2();
                                this.spriteBatch.Draw(itemTexture[player[myPlayer].inventory[num70].type], new Vector2((num68 + (26f * inventoryScale)) - ((itemTexture[player[myPlayer].inventory[num70].type].Width * 0.5f) * num72), (num69 + (26f * inventoryScale)) - ((itemTexture[player[myPlayer].inventory[num70].type].Height * 0.5f) * num72)), new Rectangle(0, 0, itemTexture[player[myPlayer].inventory[num70].type].Width, itemTexture[player[myPlayer].inventory[num70].type].Height), player[myPlayer].inventory[num70].GetColor(color10), 0f, vector6, num72, SpriteEffects.None, 0f);
                            }
                            if (player[myPlayer].inventory[num70].stack > 1)
                            {
                                vector6 = new Vector2();
                                this.spriteBatch.DrawString(fontItemStack, player[myPlayer].inventory[num70].stack.ToString(), new Vector2(num68 + (10f * inventoryScale), num69 + (26f * inventoryScale)), color10, 0f, vector6, num72, SpriteEffects.None, 0f);
                            }
                        }
                    }
                    if ((npcShop > 0) && (!playerInventory || (player[myPlayer].talkNPC == -1)))
                    {
                        npcShop = 0;
                    }
                    if (npcShop > 0)
                    {
                        vector6 = new Vector2();
                        this.spriteBatch.DrawString(fontMouseText, "Shop", new Vector2(284f, 210f), new Color(mouseTextColor, mouseTextColor, mouseTextColor, mouseTextColor), 0f, vector6, (float) 1f, SpriteEffects.None, 0f);
                        inventoryScale = 0.75f;
                        for (int num73 = 0; num73 < 5; num73++)
                        {
                            for (int num74 = 0; num74 < 4; num74++)
                            {
                                int num75 = (int) (73f + ((num73 * 0x38) * inventoryScale));
                                int num76 = (int) (210f + ((num74 * 0x38) * inventoryScale));
                                int num77 = num73 + (num74 * 5);
                                Color color11 = new Color(100, 100, 100, 100);
                                if (((mouseState.X >= num75) && (mouseState.X <= (num75 + (inventoryBackTexture.Width * inventoryScale)))) && ((mouseState.Y >= num76) && (mouseState.Y <= (num76 + (inventoryBackTexture.Height * inventoryScale)))))
                                {
                                    player[myPlayer].mouseInterface = true;
                                    if (mouseLeftRelease && (mouseState.LeftButton == ButtonState.Pressed))
                                    {
                                        if (Main.mouseItem.type == 0)
                                        {
                                            if (((player[myPlayer].selectedItem != num77) || (player[myPlayer].itemAnimation <= 0)) && player[myPlayer].BuyItem(this.shop[npcShop].item[num77].value))
                                            {
                                                Main.mouseItem.SetDefaults(this.shop[npcShop].item[num77].name);
                                                PlaySound(0x12, -1, -1, 1);
                                            }
                                        }
                                        else if (this.shop[npcShop].item[num77].type == 0)
                                        {
                                            if (player[myPlayer].SellItem(Main.mouseItem.value * Main.mouseItem.stack))
                                            {
                                                Main.mouseItem.stack = 0;
                                                Main.mouseItem.type = 0;
                                                PlaySound(0x12, -1, -1, 1);
                                            }
                                            else if (Main.mouseItem.value == 0)
                                            {
                                                Main.mouseItem.stack = 0;
                                                Main.mouseItem.type = 0;
                                                PlaySound(7, -1, -1, 1);
                                            }
                                        }
                                    }
                                    else if (((((stackSplit <= 1) && (mouseState.RightButton == ButtonState.Pressed)) && (Main.mouseItem.IsTheSameAs(this.shop[npcShop].item[num77]) || (Main.mouseItem.type == 0))) && ((Main.mouseItem.stack < Main.mouseItem.maxStack) || (Main.mouseItem.type == 0))) && player[myPlayer].BuyItem(this.shop[npcShop].item[num77].value))
                                    {
                                        PlaySound(0x12, -1, -1, 1);
                                        if (Main.mouseItem.type == 0)
                                        {
                                            Main.mouseItem = (Item) this.shop[npcShop].item[num77].Clone();
                                            Main.mouseItem.stack = 0;
                                        }
                                        Main.mouseItem.stack++;
                                        if (stackSplit == 0)
                                        {
                                            stackSplit = 15;
                                        }
                                        else
                                        {
                                            stackSplit = stackDelay;
                                        }
                                    }
                                    cursorText = this.shop[npcShop].item[num77].name;
                                    toolTip = (Item) this.shop[npcShop].item[num77].Clone();
                                    toolTip.buy = true;
                                    if (this.shop[npcShop].item[num77].stack > 1)
                                    {
                                        obj2 = cursorText;
                                        cursorText = string.Concat(new object[] { obj2, " (", this.shop[npcShop].item[num77].stack, ")" });
                                    }
                                }
                                vector6 = new Vector2();
                                this.spriteBatch.Draw(inventoryBackTexture, new Vector2((float) num75, (float) num76), new Rectangle(0, 0, inventoryBackTexture.Width, inventoryBackTexture.Height), color2, 0f, vector6, inventoryScale, SpriteEffects.None, 0f);
                                color11 = Color.White;
                                if ((this.shop[npcShop].item[num77].type > 0) && (this.shop[npcShop].item[num77].stack > 0))
                                {
                                    float num78 = 1f;
                                    if ((itemTexture[this.shop[npcShop].item[num77].type].Width > 0x20) || (itemTexture[this.shop[npcShop].item[num77].type].Height > 0x20))
                                    {
                                        if (itemTexture[this.shop[npcShop].item[num77].type].Width > itemTexture[this.shop[npcShop].item[num77].type].Height)
                                        {
                                            num78 = 32f / ((float) itemTexture[this.shop[npcShop].item[num77].type].Width);
                                        }
                                        else
                                        {
                                            num78 = 32f / ((float) itemTexture[this.shop[npcShop].item[num77].type].Height);
                                        }
                                    }
                                    num78 *= inventoryScale;
                                    vector6 = new Vector2();
                                    this.spriteBatch.Draw(itemTexture[this.shop[npcShop].item[num77].type], new Vector2((num75 + (26f * inventoryScale)) - ((itemTexture[this.shop[npcShop].item[num77].type].Width * 0.5f) * num78), (num76 + (26f * inventoryScale)) - ((itemTexture[this.shop[npcShop].item[num77].type].Height * 0.5f) * num78)), new Rectangle(0, 0, itemTexture[this.shop[npcShop].item[num77].type].Width, itemTexture[this.shop[npcShop].item[num77].type].Height), this.shop[npcShop].item[num77].GetAlpha(color11), 0f, vector6, num78, SpriteEffects.None, 0f);
                                    color22 = new Color();
                                    if (this.shop[npcShop].item[num77].color != color22)
                                    {
                                        vector6 = new Vector2();
                                        this.spriteBatch.Draw(itemTexture[this.shop[npcShop].item[num77].type], new Vector2((num75 + (26f * inventoryScale)) - ((itemTexture[this.shop[npcShop].item[num77].type].Width * 0.5f) * num78), (num76 + (26f * inventoryScale)) - ((itemTexture[this.shop[npcShop].item[num77].type].Height * 0.5f) * num78)), new Rectangle(0, 0, itemTexture[this.shop[npcShop].item[num77].type].Width, itemTexture[this.shop[npcShop].item[num77].type].Height), this.shop[npcShop].item[num77].GetColor(color11), 0f, vector6, num78, SpriteEffects.None, 0f);
                                    }
                                    if (this.shop[npcShop].item[num77].stack > 1)
                                    {
                                        vector6 = new Vector2();
                                        this.spriteBatch.DrawString(fontItemStack, this.shop[npcShop].item[num77].stack.ToString(), new Vector2(num75 + (10f * inventoryScale), num76 + (26f * inventoryScale)), color11, 0f, vector6, num78, SpriteEffects.None, 0f);
                                    }
                                }
                            }
                        }
                    }
                    if ((player[myPlayer].chest > -1) && (tile[player[myPlayer].chestX, player[myPlayer].chestY].type != 0x15))
                    {
                        player[myPlayer].chest = -1;
                    }
                    if (player[myPlayer].chest > -1)
                    {
                        vector6 = new Vector2();
                        this.spriteBatch.DrawString(fontMouseText, "Chest", new Vector2(284f, 210f), new Color(mouseTextColor, mouseTextColor, mouseTextColor, mouseTextColor), 0f, vector6, (float) 1f, SpriteEffects.None, 0f);
                        inventoryScale = 0.75f;
                        for (int num79 = 0; num79 < 5; num79++)
                        {
                            for (int num80 = 0; num80 < 4; num80++)
                            {
                                int num81 = (int) (73f + ((num79 * 0x38) * inventoryScale));
                                int num82 = (int) (210f + ((num80 * 0x38) * inventoryScale));
                                int num83 = num79 + (num80 * 5);
                                Color color12 = new Color(100, 100, 100, 100);
                                if (((mouseState.X >= num81) && (mouseState.X <= (num81 + (inventoryBackTexture.Width * inventoryScale)))) && ((mouseState.Y >= num82) && (mouseState.Y <= (num82 + (inventoryBackTexture.Height * inventoryScale)))))
                                {
                                    player[myPlayer].mouseInterface = true;
                                    if (mouseLeftRelease && (mouseState.LeftButton == ButtonState.Pressed))
                                    {
                                        if ((player[myPlayer].selectedItem != num83) || (player[myPlayer].itemAnimation <= 0))
                                        {
                                            Item item4 = Main.mouseItem;
                                            Main.mouseItem = chest[player[myPlayer].chest].item[num83];
                                            chest[player[myPlayer].chest].item[num83] = item4;
                                            if ((chest[player[myPlayer].chest].item[num83].type == 0) || (chest[player[myPlayer].chest].item[num83].stack < 1))
                                            {
                                                chest[player[myPlayer].chest].item[num83] = new Item();
                                            }
                                            if ((Main.mouseItem.IsTheSameAs(chest[player[myPlayer].chest].item[num83]) && (chest[player[myPlayer].chest].item[num83].stack != chest[player[myPlayer].chest].item[num83].maxStack)) && (Main.mouseItem.stack != Main.mouseItem.maxStack))
                                            {
                                                if ((Main.mouseItem.stack + chest[player[myPlayer].chest].item[num83].stack) <= Main.mouseItem.maxStack)
                                                {
                                                    Item item11 = chest[player[myPlayer].chest].item[num83];
                                                    item11.stack += Main.mouseItem.stack;
                                                    Main.mouseItem.stack = 0;
                                                }
                                                else
                                                {
                                                    int num84 = Main.mouseItem.maxStack - chest[player[myPlayer].chest].item[num83].stack;
                                                    Item item12 = chest[player[myPlayer].chest].item[num83];
                                                    item12.stack += num84;
                                                    Main.mouseItem.stack -= num84;
                                                }
                                            }
                                            if ((Main.mouseItem.type == 0) || (Main.mouseItem.stack < 1))
                                            {
                                                Main.mouseItem = new Item();
                                            }
                                            if ((Main.mouseItem.type > 0) || (chest[player[myPlayer].chest].item[num83].type > 0))
                                            {
                                                Recipe.FindRecipes();
                                                PlaySound(7, -1, -1, 1);
                                            }
                                            if (netMode == 1)
                                            {
                                                NetMessage.SendData(0x20, -1, -1, "", player[myPlayer].chest, (float) num83, 0f, 0f);
                                            }
                                        }
                                    }
                                    else if ((((stackSplit <= 1) && (mouseState.RightButton == ButtonState.Pressed)) && (Main.mouseItem.IsTheSameAs(chest[player[myPlayer].chest].item[num83]) || (Main.mouseItem.type == 0))) && ((Main.mouseItem.stack < Main.mouseItem.maxStack) || (Main.mouseItem.type == 0)))
                                    {
                                        if (Main.mouseItem.type == 0)
                                        {
                                            Main.mouseItem = (Item) chest[player[myPlayer].chest].item[num83].Clone();
                                            Main.mouseItem.stack = 0;
                                        }
                                        Main.mouseItem.stack++;
                                        Item item13 = chest[player[myPlayer].chest].item[num83];
                                        item13.stack--;
                                        if (chest[player[myPlayer].chest].item[num83].stack <= 0)
                                        {
                                            chest[player[myPlayer].chest].item[num83] = new Item();
                                        }
                                        Recipe.FindRecipes();
                                        soundInstanceMenuTick.Stop();
                                        soundInstanceMenuTick = soundMenuTick.CreateInstance();
                                        PlaySound(12, -1, -1, 1);
                                        if (stackSplit == 0)
                                        {
                                            stackSplit = 15;
                                        }
                                        else
                                        {
                                            stackSplit = stackDelay;
                                        }
                                        if (netMode == 1)
                                        {
                                            NetMessage.SendData(0x20, -1, -1, "", player[myPlayer].chest, (float) num83, 0f, 0f);
                                        }
                                    }
                                    cursorText = chest[player[myPlayer].chest].item[num83].name;
                                    toolTip = (Item) chest[player[myPlayer].chest].item[num83].Clone();
                                    if (chest[player[myPlayer].chest].item[num83].stack > 1)
                                    {
                                        obj2 = cursorText;
                                        cursorText = string.Concat(new object[] { obj2, " (", chest[player[myPlayer].chest].item[num83].stack, ")" });
                                    }
                                }
                                vector6 = new Vector2();
                                this.spriteBatch.Draw(inventoryBackTexture, new Vector2((float) num81, (float) num82), new Rectangle(0, 0, inventoryBackTexture.Width, inventoryBackTexture.Height), color2, 0f, vector6, inventoryScale, SpriteEffects.None, 0f);
                                color12 = Color.White;
                                if ((chest[player[myPlayer].chest].item[num83].type > 0) && (chest[player[myPlayer].chest].item[num83].stack > 0))
                                {
                                    float num85 = 1f;
                                    if ((itemTexture[chest[player[myPlayer].chest].item[num83].type].Width > 0x20) || (itemTexture[chest[player[myPlayer].chest].item[num83].type].Height > 0x20))
                                    {
                                        if (itemTexture[chest[player[myPlayer].chest].item[num83].type].Width > itemTexture[chest[player[myPlayer].chest].item[num83].type].Height)
                                        {
                                            num85 = 32f / ((float) itemTexture[chest[player[myPlayer].chest].item[num83].type].Width);
                                        }
                                        else
                                        {
                                            num85 = 32f / ((float) itemTexture[chest[player[myPlayer].chest].item[num83].type].Height);
                                        }
                                    }
                                    num85 *= inventoryScale;
                                    vector6 = new Vector2();
                                    this.spriteBatch.Draw(itemTexture[chest[player[myPlayer].chest].item[num83].type], new Vector2((num81 + (26f * inventoryScale)) - ((itemTexture[chest[player[myPlayer].chest].item[num83].type].Width * 0.5f) * num85), (num82 + (26f * inventoryScale)) - ((itemTexture[chest[player[myPlayer].chest].item[num83].type].Height * 0.5f) * num85)), new Rectangle(0, 0, itemTexture[chest[player[myPlayer].chest].item[num83].type].Width, itemTexture[chest[player[myPlayer].chest].item[num83].type].Height), chest[player[myPlayer].chest].item[num83].GetAlpha(color12), 0f, vector6, num85, SpriteEffects.None, 0f);
                                    color22 = new Color();
                                    if (chest[player[myPlayer].chest].item[num83].color != color22)
                                    {
                                        vector6 = new Vector2();
                                        this.spriteBatch.Draw(itemTexture[chest[player[myPlayer].chest].item[num83].type], new Vector2((num81 + (26f * inventoryScale)) - ((itemTexture[chest[player[myPlayer].chest].item[num83].type].Width * 0.5f) * num85), (num82 + (26f * inventoryScale)) - ((itemTexture[chest[player[myPlayer].chest].item[num83].type].Height * 0.5f) * num85)), new Rectangle(0, 0, itemTexture[chest[player[myPlayer].chest].item[num83].type].Width, itemTexture[chest[player[myPlayer].chest].item[num83].type].Height), chest[player[myPlayer].chest].item[num83].GetColor(color12), 0f, vector6, num85, SpriteEffects.None, 0f);
                                    }
                                    if (chest[player[myPlayer].chest].item[num83].stack > 1)
                                    {
                                        vector6 = new Vector2();
                                        this.spriteBatch.DrawString(fontItemStack, chest[player[myPlayer].chest].item[num83].stack.ToString(), new Vector2(num81 + (10f * inventoryScale), num82 + (26f * inventoryScale)), color12, 0f, vector6, num85, SpriteEffects.None, 0f);
                                    }
                                }
                            }
                        }
                    }
                    if (player[myPlayer].chest == -2)
                    {
                        vector6 = new Vector2();
                        this.spriteBatch.DrawString(fontMouseText, "Piggy Bank", new Vector2(284f, 210f), new Color(mouseTextColor, mouseTextColor, mouseTextColor, mouseTextColor), 0f, vector6, (float) 1f, SpriteEffects.None, 0f);
                        inventoryScale = 0.75f;
                        for (int num86 = 0; num86 < 5; num86++)
                        {
                            for (int num87 = 0; num87 < 4; num87++)
                            {
                                int num88 = (int) (73f + ((num86 * 0x38) * inventoryScale));
                                int num89 = (int) (210f + ((num87 * 0x38) * inventoryScale));
                                int num90 = num86 + (num87 * 5);
                                Color color13 = new Color(100, 100, 100, 100);
                                if (((mouseState.X >= num88) && (mouseState.X <= (num88 + (inventoryBackTexture.Width * inventoryScale)))) && ((mouseState.Y >= num89) && (mouseState.Y <= (num89 + (inventoryBackTexture.Height * inventoryScale)))))
                                {
                                    player[myPlayer].mouseInterface = true;
                                    if (mouseLeftRelease && (mouseState.LeftButton == ButtonState.Pressed))
                                    {
                                        if ((player[myPlayer].selectedItem != num90) || (player[myPlayer].itemAnimation <= 0))
                                        {
                                            Item item5 = Main.mouseItem;
                                            Main.mouseItem = player[myPlayer].bank[num90];
                                            player[myPlayer].bank[num90] = item5;
                                            if ((player[myPlayer].bank[num90].type == 0) || (player[myPlayer].bank[num90].stack < 1))
                                            {
                                                player[myPlayer].bank[num90] = new Item();
                                            }
                                            if ((Main.mouseItem.IsTheSameAs(player[myPlayer].bank[num90]) && (player[myPlayer].bank[num90].stack != player[myPlayer].bank[num90].maxStack)) && (Main.mouseItem.stack != Main.mouseItem.maxStack))
                                            {
                                                if ((Main.mouseItem.stack + player[myPlayer].bank[num90].stack) <= Main.mouseItem.maxStack)
                                                {
                                                    Item item14 = player[myPlayer].bank[num90];
                                                    item14.stack += Main.mouseItem.stack;
                                                    Main.mouseItem.stack = 0;
                                                }
                                                else
                                                {
                                                    int num91 = Main.mouseItem.maxStack - player[myPlayer].bank[num90].stack;
                                                    Item item15 = player[myPlayer].bank[num90];
                                                    item15.stack += num91;
                                                    Main.mouseItem.stack -= num91;
                                                }
                                            }
                                            if ((Main.mouseItem.type == 0) || (Main.mouseItem.stack < 1))
                                            {
                                                Main.mouseItem = new Item();
                                            }
                                            if ((Main.mouseItem.type > 0) || (player[myPlayer].bank[num90].type > 0))
                                            {
                                                Recipe.FindRecipes();
                                                PlaySound(7, -1, -1, 1);
                                            }
                                        }
                                    }
                                    else if ((((stackSplit <= 1) && (mouseState.RightButton == ButtonState.Pressed)) && (Main.mouseItem.IsTheSameAs(player[myPlayer].bank[num90]) || (Main.mouseItem.type == 0))) && ((Main.mouseItem.stack < Main.mouseItem.maxStack) || (Main.mouseItem.type == 0)))
                                    {
                                        if (Main.mouseItem.type == 0)
                                        {
                                            Main.mouseItem = (Item) player[myPlayer].bank[num90].Clone();
                                            Main.mouseItem.stack = 0;
                                        }
                                        Main.mouseItem.stack++;
                                        Item item16 = player[myPlayer].bank[num90];
                                        item16.stack--;
                                        if (player[myPlayer].bank[num90].stack <= 0)
                                        {
                                            player[myPlayer].bank[num90] = new Item();
                                        }
                                        Recipe.FindRecipes();
                                        soundInstanceMenuTick.Stop();
                                        soundInstanceMenuTick = soundMenuTick.CreateInstance();
                                        PlaySound(12, -1, -1, 1);
                                        if (stackSplit == 0)
                                        {
                                            stackSplit = 15;
                                        }
                                        else
                                        {
                                            stackSplit = stackDelay;
                                        }
                                    }
                                    cursorText = player[myPlayer].bank[num90].name;
                                    toolTip = (Item) player[myPlayer].bank[num90].Clone();
                                    if (player[myPlayer].bank[num90].stack > 1)
                                    {
                                        obj2 = cursorText;
                                        cursorText = string.Concat(new object[] { obj2, " (", player[myPlayer].bank[num90].stack, ")" });
                                    }
                                }
                                vector6 = new Vector2();
                                this.spriteBatch.Draw(inventoryBackTexture, new Vector2((float) num88, (float) num89), new Rectangle(0, 0, inventoryBackTexture.Width, inventoryBackTexture.Height), color2, 0f, vector6, inventoryScale, SpriteEffects.None, 0f);
                                color13 = Color.White;
                                if ((player[myPlayer].bank[num90].type > 0) && (player[myPlayer].bank[num90].stack > 0))
                                {
                                    float num92 = 1f;
                                    if ((itemTexture[player[myPlayer].bank[num90].type].Width > 0x20) || (itemTexture[player[myPlayer].bank[num90].type].Height > 0x20))
                                    {
                                        if (itemTexture[player[myPlayer].bank[num90].type].Width > itemTexture[player[myPlayer].bank[num90].type].Height)
                                        {
                                            num92 = 32f / ((float) itemTexture[player[myPlayer].bank[num90].type].Width);
                                        }
                                        else
                                        {
                                            num92 = 32f / ((float) itemTexture[player[myPlayer].bank[num90].type].Height);
                                        }
                                    }
                                    num92 *= inventoryScale;
                                    vector6 = new Vector2();
                                    this.spriteBatch.Draw(itemTexture[player[myPlayer].bank[num90].type], new Vector2((num88 + (26f * inventoryScale)) - ((itemTexture[player[myPlayer].bank[num90].type].Width * 0.5f) * num92), (num89 + (26f * inventoryScale)) - ((itemTexture[player[myPlayer].bank[num90].type].Height * 0.5f) * num92)), new Rectangle(0, 0, itemTexture[player[myPlayer].bank[num90].type].Width, itemTexture[player[myPlayer].bank[num90].type].Height), player[myPlayer].bank[num90].GetAlpha(color13), 0f, vector6, num92, SpriteEffects.None, 0f);
                                    color22 = new Color();
                                    if (player[myPlayer].bank[num90].color != color22)
                                    {
                                        vector6 = new Vector2();
                                        this.spriteBatch.Draw(itemTexture[player[myPlayer].bank[num90].type], new Vector2((num88 + (26f * inventoryScale)) - ((itemTexture[player[myPlayer].bank[num90].type].Width * 0.5f) * num92), (num89 + (26f * inventoryScale)) - ((itemTexture[player[myPlayer].bank[num90].type].Height * 0.5f) * num92)), new Rectangle(0, 0, itemTexture[player[myPlayer].bank[num90].type].Width, itemTexture[player[myPlayer].bank[num90].type].Height), player[myPlayer].bank[num90].GetColor(color13), 0f, vector6, num92, SpriteEffects.None, 0f);
                                    }
                                    if (player[myPlayer].bank[num90].stack > 1)
                                    {
                                        vector6 = new Vector2();
                                        this.spriteBatch.DrawString(fontItemStack, player[myPlayer].bank[num90].stack.ToString(), new Vector2(num88 + (10f * inventoryScale), num89 + (26f * inventoryScale)), color13, 0f, vector6, num92, SpriteEffects.None, 0f);
                                    }
                                }
                            }
                        }
                    }
                }
                if (!playerInventory)
                {
                    vector6 = new Vector2();
                    this.spriteBatch.DrawString(fontMouseText, "Items", new Vector2(215f, 0f), new Color(mouseTextColor, mouseTextColor, mouseTextColor, mouseTextColor), 0f, vector6, (float) 1f, SpriteEffects.None, 0f);
                    int num102 = 20;
                    float num103 = 1f;
                    for (int num104 = 0; num104 < 10; num104++)
                    {
                        if (num104 == player[myPlayer].selectedItem)
                        {
                            if (hotbarScale[num104] < 1f)
                            {
                                hotbarScale[num104] += 0.05f;
                            }
                        }
                        else if (hotbarScale[num104] > 0.75)
                        {
                            hotbarScale[num104] -= 0.05f;
                        }
                        int num105 = (int) (20f + (22f * (1f - hotbarScale[num104])));
                        int a = (int) (75f + (150f * hotbarScale[num104]));
                        Color color15 = new Color(0xff, 0xff, 0xff, a);
                        vector6 = new Vector2();
                        this.spriteBatch.Draw(inventoryBackTexture, new Vector2((float) num102, (float) num105), new Rectangle(0, 0, inventoryBackTexture.Width, inventoryBackTexture.Height), new Color(100, 100, 100, 100), 0f, vector6, hotbarScale[num104], SpriteEffects.None, 0f);
                        if ((((mouseState.X >= num102) && (mouseState.X <= (num102 + (inventoryBackTexture.Width * hotbarScale[num104])))) && ((mouseState.Y >= num105) && (mouseState.Y <= (num105 + (inventoryBackTexture.Height * hotbarScale[num104]))))) && !player[myPlayer].channel)
                        {
                            player[myPlayer].mouseInterface = true;
                            if (mouseState.LeftButton == ButtonState.Pressed)
                            {
                                player[myPlayer].changeItem = num104;
                            }
                            player[myPlayer].showItemIcon = false;
                            cursorText = player[myPlayer].inventory[num104].name;
                            if (player[myPlayer].inventory[num104].stack > 1)
                            {
                                obj2 = cursorText;
                                cursorText = string.Concat(new object[] { obj2, " (", player[myPlayer].inventory[num104].stack, ")" });
                            }
                            rare = player[myPlayer].inventory[num104].rare;
                        }
                        if ((player[myPlayer].inventory[num104].type > 0) && (player[myPlayer].inventory[num104].stack > 0))
                        {
                            num103 = 1f;
                            if ((itemTexture[player[myPlayer].inventory[num104].type].Width > 0x20) || (itemTexture[player[myPlayer].inventory[num104].type].Height > 0x20))
                            {
                                if (itemTexture[player[myPlayer].inventory[num104].type].Width > itemTexture[player[myPlayer].inventory[num104].type].Height)
                                {
                                    num103 = 32f / ((float) itemTexture[player[myPlayer].inventory[num104].type].Width);
                                }
                                else
                                {
                                    num103 = 32f / ((float) itemTexture[player[myPlayer].inventory[num104].type].Height);
                                }
                            }
                            num103 *= hotbarScale[num104];
                            vector6 = new Vector2();
                            this.spriteBatch.Draw(itemTexture[player[myPlayer].inventory[num104].type], new Vector2((num102 + (26f * hotbarScale[num104])) - ((itemTexture[player[myPlayer].inventory[num104].type].Width * 0.5f) * num103), (num105 + (26f * hotbarScale[num104])) - ((itemTexture[player[myPlayer].inventory[num104].type].Height * 0.5f) * num103)), new Rectangle(0, 0, itemTexture[player[myPlayer].inventory[num104].type].Width, itemTexture[player[myPlayer].inventory[num104].type].Height), player[myPlayer].inventory[num104].GetAlpha(color15), 0f, vector6, num103, SpriteEffects.None, 0f);
                            color22 = new Color();
                            if (player[myPlayer].inventory[num104].color != color22)
                            {
                                vector6 = new Vector2();
                                this.spriteBatch.Draw(itemTexture[player[myPlayer].inventory[num104].type], new Vector2((num102 + (26f * hotbarScale[num104])) - ((itemTexture[player[myPlayer].inventory[num104].type].Width * 0.5f) * num103), (num105 + (26f * hotbarScale[num104])) - ((itemTexture[player[myPlayer].inventory[num104].type].Height * 0.5f) * num103)), new Rectangle(0, 0, itemTexture[player[myPlayer].inventory[num104].type].Width, itemTexture[player[myPlayer].inventory[num104].type].Height), player[myPlayer].inventory[num104].GetColor(color15), 0f, vector6, num103, SpriteEffects.None, 0f);
                            }
                            if (player[myPlayer].inventory[num104].stack > 1)
                            {
                                vector6 = new Vector2();
                                this.spriteBatch.DrawString(fontItemStack, player[myPlayer].inventory[num104].stack.ToString(), new Vector2(num102 + (10f * hotbarScale[num104]), num105 + (26f * hotbarScale[num104])), color15, 0f, vector6, num103, SpriteEffects.None, 0f);
                            }
                            if (player[myPlayer].inventory[num104].potion)
                            {
                                Color alpha = player[myPlayer].inventory[num104].GetAlpha(color15);
                                float num107 = ((float) player[myPlayer].potionDelay) / ((float) Item.potionDelay);
                                float num108 = alpha.R * num107;
                                float num109 = alpha.G * num107;
                                float num110 = alpha.B * num107;
                                float num111 = alpha.A * num107;
                                alpha = new Color((int) ((byte) num108), (int) ((byte) num109), (int) ((byte) num110), (int) ((byte) num111));
                                vector6 = new Vector2();
                                this.spriteBatch.Draw(cdTexture, new Vector2((num102 + (26f * hotbarScale[num104])) - ((cdTexture.Width * 0.5f) * num103), (num105 + (26f * hotbarScale[num104])) - ((cdTexture.Height * 0.5f) * num103)), new Rectangle(0, 0, cdTexture.Width, cdTexture.Height), alpha, 0f, vector6, num103, SpriteEffects.None, 0f);
                            }
                        }
                        num102 += ((int) (inventoryBackTexture.Width * hotbarScale[num104])) + 4;
                    }
                }
                if (((cursorText != null) && (cursorText != "")) && (Main.mouseItem.type == 0))
                {
                    player[myPlayer].showItemIcon = false;
                    this.MouseText(cursorText, rare);
                    flag = true;
                }
                if (chatMode)
                {
                    this.textBlinkerCount++;
                    if (this.textBlinkerCount >= 20)
                    {
                        if (this.textBlinkerState == 0)
                        {
                            this.textBlinkerState = 1;
                        }
                        else
                        {
                            this.textBlinkerState = 0;
                        }
                        this.textBlinkerCount = 0;
                    }
                    string chatText = Main.chatText;
                    if (this.textBlinkerState == 1)
                    {
                        chatText = chatText + "|";
                    }
                    vector6 = new Vector2();
                    this.spriteBatch.Draw(textBackTexture, new Vector2(78f, (float) (screenHeight - 0x24)), new Rectangle(0, 0, textBackTexture.Width, textBackTexture.Height), new Color(100, 100, 100, 100), 0f, vector6, (float) 1f, SpriteEffects.None, 0f);
                    for (int num112 = 0; num112 < 5; num112++)
                    {
                        int num113 = 0;
                        int num114 = 0;
                        Color color17 = Color.Black;
                        switch (num112)
                        {
                            case 0:
                                num113 = -2;
                                break;

                            case 1:
                                num113 = 2;
                                break;

                            case 2:
                                num114 = -2;
                                break;

                            case 3:
                                num114 = 2;
                                break;

                            case 4:
                                color17 = new Color((int) mouseTextColor, (int) mouseTextColor, (int) mouseTextColor, (int) mouseTextColor);
                                break;
                        }
                        vector6 = new Vector2();
                        this.spriteBatch.DrawString(fontMouseText, chatText, new Vector2((float) (0x58 + num113), (float) ((screenHeight - 30) + num114)), color17, 0f, vector6, (float) 1f, SpriteEffects.None, 0f);
                    }
                }
                for (int k = 0; k < numChatLines; k++)
                {
                    if (chatMode || (chatLine[k].showTime > 0))
                    {
                        float num116 = ((float) mouseTextColor) / 255f;
                        for (int num117 = 0; num117 < 5; num117++)
                        {
                            int num118 = 0;
                            int num119 = 0;
                            Color color18 = Color.Black;
                            switch (num117)
                            {
                                case 0:
                                    num118 = -2;
                                    break;

                                case 1:
                                    num118 = 2;
                                    break;

                                case 2:
                                    num119 = -2;
                                    break;

                                case 3:
                                    num119 = 2;
                                    break;

                                case 4:
                                    color18 = new Color((int) ((byte) (chatLine[k].color.R * num116)), (int) ((byte) (chatLine[k].color.G * num116)), (int) ((byte) (chatLine[k].color.B * num116)), (int) mouseTextColor);
                                    break;
                            }
                            vector6 = new Vector2();
                            this.spriteBatch.DrawString(fontMouseText, chatLine[k].text, new Vector2((float) (0x58 + num118), (float) ((((screenHeight - 30) + num119) - 0x1c) - (k * 0x15))), color18, 0f, vector6, (float) 1f, SpriteEffects.None, 0f);
                        }
                    }
                }
                if (player[myPlayer].dead)
                {
                    string str10 = "You were slain...";
                    vector6 = new Vector2();
                    this.spriteBatch.DrawString(fontDeathText, str10, new Vector2((float) ((screenWidth / 2) - (str10.Length * 10)), (float) ((screenHeight / 2) - 20)), player[myPlayer].GetDeathAlpha(new Color(0, 0, 0, 0)), 0f, vector6, (float) 1f, SpriteEffects.None, 0f);
                }
                vector6 = new Vector2();
                this.spriteBatch.Draw(cursorTexture, new Vector2((float) (mouseState.X + 1), (float) (mouseState.Y + 1)), new Rectangle(0, 0, cursorTexture.Width, cursorTexture.Height), new Color((int) (cursorColor.R * 0.2f), (int) (cursorColor.G * 0.2f), (int) (cursorColor.B * 0.2f), (int) (cursorColor.A * 0.5f)), 0f, vector6, (float) (cursorScale * 1.1f), SpriteEffects.None, 0f);
                vector6 = new Vector2();
                this.spriteBatch.Draw(cursorTexture, new Vector2((float) mouseState.X, (float) mouseState.Y), new Rectangle(0, 0, cursorTexture.Width, cursorTexture.Height), cursorColor, 0f, vector6, cursorScale, SpriteEffects.None, 0f);
                if ((Main.mouseItem.type > 0) && (Main.mouseItem.stack > 0))
                {
                    player[myPlayer].showItemIcon = false;
                    player[myPlayer].showItemIcon2 = 0;
                    flag = true;
                    float num120 = 1f;
                    if ((itemTexture[Main.mouseItem.type].Width > 0x20) || (itemTexture[Main.mouseItem.type].Height > 0x20))
                    {
                        if (itemTexture[Main.mouseItem.type].Width > itemTexture[Main.mouseItem.type].Height)
                        {
                            num120 = 32f / ((float) itemTexture[Main.mouseItem.type].Width);
                        }
                        else
                        {
                            num120 = 32f / ((float) itemTexture[Main.mouseItem.type].Height);
                        }
                    }
                    float num121 = 1f;
                    Color color19 = Color.White;
                    num120 *= num121;
                    vector6 = new Vector2();
                    this.spriteBatch.Draw(itemTexture[Main.mouseItem.type], new Vector2((mouseState.X + (26f * num121)) - ((itemTexture[Main.mouseItem.type].Width * 0.5f) * num120), (mouseState.Y + (26f * num121)) - ((itemTexture[Main.mouseItem.type].Height * 0.5f) * num120)), new Rectangle(0, 0, itemTexture[Main.mouseItem.type].Width, itemTexture[Main.mouseItem.type].Height), Main.mouseItem.GetAlpha(color19), 0f, vector6, num120, SpriteEffects.None, 0f);
                    color22 = new Color();
                    if (Main.mouseItem.color != color22)
                    {
                        vector6 = new Vector2();
                        this.spriteBatch.Draw(itemTexture[Main.mouseItem.type], new Vector2((mouseState.X + (26f * num121)) - ((itemTexture[Main.mouseItem.type].Width * 0.5f) * num120), (mouseState.Y + (26f * num121)) - ((itemTexture[Main.mouseItem.type].Height * 0.5f) * num120)), new Rectangle(0, 0, itemTexture[Main.mouseItem.type].Width, itemTexture[Main.mouseItem.type].Height), Main.mouseItem.GetColor(color19), 0f, vector6, num120, SpriteEffects.None, 0f);
                    }
                    if (Main.mouseItem.stack > 1)
                    {
                        vector6 = new Vector2();
                        this.spriteBatch.DrawString(fontItemStack, Main.mouseItem.stack.ToString(), new Vector2(mouseState.X + (10f * num121), mouseState.Y + (26f * num121)), color19, 0f, vector6, num120, SpriteEffects.None, 0f);
                    }
                }
                Rectangle rectangle3 = new Rectangle(mouseState.X + ((int) screenPosition.X), mouseState.Y + ((int) screenPosition.Y), 1, 1);
                if (!flag)
                {
                    int num122 = (0x1a * player[myPlayer].statLifeMax) / num12;
                    int num123 = 0;
                    if (player[myPlayer].statLifeMax > 200)
                    {
                        num122 = 260;
                        num123 += 0x1a;
                    }
                    if (((mouseState.X > 500) && (mouseState.X < (500 + num122))) && ((mouseState.Y > 0x20) && (mouseState.Y < ((0x20 + heartTexture.Height) + num123))))
                    {
                        player[myPlayer].showItemIcon = false;
                        string str11 = player[myPlayer].statLife + "/" + player[myPlayer].statLifeMax;
                        this.MouseText(str11, 0);
                        flag = true;
                    }
                }
                if (!flag)
                {
                    int num124 = 0x18;
                    int num125 = (0x1c * player[myPlayer].statManaMax) / num19;
                    if (((mouseState.X > 0x2fa) && (mouseState.X < (0x2fa + num124))) && ((mouseState.Y > 30) && (mouseState.Y < (30 + num125))))
                    {
                        player[myPlayer].showItemIcon = false;
                        string str12 = player[myPlayer].statMana + "/" + player[myPlayer].statManaMax;
                        this.MouseText(str12, 0);
                        flag = true;
                    }
                }
                if (!flag)
                {
                    for (int num126 = 0; num126 < 200; num126++)
                    {
                        if (item[num126].active)
                        {
                            Rectangle rectangle4 = new Rectangle((int) ((item[num126].position.X + (item[num126].width * 0.5)) - (itemTexture[item[num126].type].Width * 0.5)), (((int) item[num126].position.Y) + item[num126].height) - itemTexture[item[num126].type].Height, itemTexture[item[num126].type].Width, itemTexture[item[num126].type].Height);
                            if (rectangle3.Intersects(rectangle4))
                            {
                                player[myPlayer].showItemIcon = false;
                                string str13 = item[num126].name;
                                if (item[num126].stack > 1)
                                {
                                    obj2 = str13;
                                    str13 = string.Concat(new object[] { obj2, " (", item[num126].stack, ")" });
                                }
                                if ((item[num126].owner < 8) && showItemOwner)
                                {
                                    str13 = str13 + " <" + player[item[num126].owner].name + ">";
                                }
                                rare = item[num126].rare;
                                this.MouseText(str13, rare);
                                flag = true;
                                break;
                            }
                        }
                    }
                }
                for (int m = 0; m < 8; m++)
                {
                    if ((player[m].active && (myPlayer != m)) && !player[m].dead)
                    {
                        Rectangle rectangle5 = new Rectangle((int) ((player[m].position.X + (player[m].width * 0.5)) - 16.0), (int) ((player[m].position.Y + player[m].height) - 48f), 0x20, 0x30);
                        if (!flag && rectangle3.Intersects(rectangle5))
                        {
                            player[myPlayer].showItemIcon = false;
                            string str14 = string.Concat(new object[] { player[m].name, ": ", player[m].statLife, "/", player[m].statLifeMax });
                            if (player[m].hostile)
                            {
                                str14 = str14 + " (PvP)";
                            }
                            this.MouseText(str14, 0);
                        }
                    }
                }
                if (!flag)
                {
                    for (int num128 = 0; num128 < 0x3e8; num128++)
                    {
                        if (npc[num128].active)
                        {
                            Rectangle rectangle6 = new Rectangle((int) ((npc[num128].position.X + (npc[num128].width * 0.5)) - (npcTexture[npc[num128].type].Width * 0.5)), (((int) npc[num128].position.Y) + npc[num128].height) - (npcTexture[npc[num128].type].Height / npcFrameCount[npc[num128].type]), npcTexture[npc[num128].type].Width, npcTexture[npc[num128].type].Height / npcFrameCount[npc[num128].type]);
                            if (rectangle3.Intersects(rectangle6))
                            {
                                bool flag4 = false;
                                if (npc[num128].townNPC)
                                {
                                    Rectangle rectangle7 = new Rectangle((((int) player[myPlayer].position.X) + (player[myPlayer].width / 2)) - (Player.tileRangeX * 0x10), (((int) player[myPlayer].position.Y) + (player[myPlayer].height / 2)) - (Player.tileRangeY * 0x10), (Player.tileRangeX * 0x10) * 2, (Player.tileRangeY * 0x10) * 2);
                                    Rectangle rectangle8 = new Rectangle((int) npc[num128].position.X, (int) npc[num128].position.Y, npc[num128].width, npc[num128].height);
                                    if (rectangle7.Intersects(rectangle8))
                                    {
                                        flag4 = true;
                                    }
                                }
                                if (flag4)
                                {
                                    int num129 = -((npc[num128].width / 2) + 8);
                                    SpriteEffects effects = SpriteEffects.None;
                                    if (npc[num128].spriteDirection == -1)
                                    {
                                        effects = SpriteEffects.FlipHorizontally;
                                        num129 = (npc[num128].width / 2) + 8;
                                    }
                                    vector6 = new Vector2();
                                    this.spriteBatch.Draw(chatTexture, new Vector2((((npc[num128].position.X + (npc[num128].width / 2)) - screenPosition.X) - (chatTexture.Width / 2)) - num129, (npc[num128].position.Y - chatTexture.Height) - screenPosition.Y), new Rectangle(0, 0, chatTexture.Width, chatTexture.Height), new Color(mouseTextColor, mouseTextColor, mouseTextColor, mouseTextColor), 0f, vector6, (float) 1f, effects, 0f);
                                    if ((mouseState.RightButton == ButtonState.Pressed) && npcChatRelease)
                                    {
                                        npcChatRelease = false;
                                        if (player[myPlayer].talkNPC != num128)
                                        {
                                            player[myPlayer].sign = -1;
                                            editSign = false;
                                            player[myPlayer].talkNPC = num128;
                                            playerInventory = false;
                                            player[myPlayer].chest = -1;
                                            npcChatText = npc[num128].GetChat();
                                            PlaySound(10, -1, -1, 1);
                                        }
                                    }
                                }
                                player[myPlayer].showItemIcon = false;
                                string str15 = string.Concat(new object[] { npc[num128].name, ": ", npc[num128].life, "/", npc[num128].lifeMax });
                                this.MouseText(str15, 0);
                                break;
                            }
                        }
                    }
                }
                if (mouseState.RightButton == ButtonState.Pressed)
                {
                    npcChatRelease = false;
                }
                else
                {
                    npcChatRelease = true;
                }
                if (player[myPlayer].showItemIcon && ((player[myPlayer].inventory[player[myPlayer].selectedItem].type > 0) || (player[myPlayer].showItemIcon2 > 0)))
                {
                    int type = player[myPlayer].inventory[player[myPlayer].selectedItem].type;
                    Color color20 = player[myPlayer].inventory[player[myPlayer].selectedItem].GetAlpha(Color.White);
                    Color color21 = player[myPlayer].inventory[player[myPlayer].selectedItem].GetColor(Color.White);
                    if (player[myPlayer].showItemIcon2 > 0)
                    {
                        type = player[myPlayer].showItemIcon2;
                        color20 = Color.White;
                        color21 = new Color();
                    }
                    vector6 = new Vector2();
                    this.spriteBatch.Draw(itemTexture[type], new Vector2((float) (mouseState.X + 10), (float) (mouseState.Y + 10)), new Rectangle(0, 0, itemTexture[type].Width, itemTexture[type].Height), color20, 0f, vector6, (float) 1f, SpriteEffects.None, 0f);
                    if ((player[myPlayer].showItemIcon2 == 0) && (player[myPlayer].inventory[player[myPlayer].selectedItem].color != new Color()))
                    {
                        this.spriteBatch.Draw(itemTexture[player[myPlayer].inventory[player[myPlayer].selectedItem].type], new Vector2((float) (mouseState.X + 10), (float) (mouseState.Y + 10)), new Rectangle(0, 0, itemTexture[player[myPlayer].inventory[player[myPlayer].selectedItem].type].Width, itemTexture[player[myPlayer].inventory[player[myPlayer].selectedItem].type].Height), color21, 0f, new Vector2(), (float) 1f, SpriteEffects.None, 0f);
                    }
                }
                player[myPlayer].showItemIcon = false;
                player[myPlayer].showItemIcon2 = 0;
            }
        }

        protected void DrawMenu()
        {
            string[] strArray3;
            Vector2 vector9;
            evilTiles = 0;
            chatMode = false;
            for (int i = 0; i < numChatLines; i++)
            {
                chatLine[i] = new ChatLine();
            }
            this.DrawFPS();
            screenPosition.Y = (float) ((worldSurface * 16.0) - screenHeight);
            background = 0;
            byte r = (byte) ((0xff + (tileColor.R * 2)) / 3);
            Color color = new Color(r, r, r, 0xff);
            this.logoRotation += this.logoRotationSpeed * 3E-05f;
            if (this.logoRotation > 0.1)
            {
                this.logoRotationDirection = -1f;
            }
            else if (this.logoRotation < -0.1)
            {
                this.logoRotationDirection = 1f;
            }
            if ((this.logoRotationSpeed < 20f) & (this.logoRotationDirection == 1f))
            {
                this.logoRotationSpeed++;
            }
            else if ((this.logoRotationSpeed > -20f) & (this.logoRotationDirection == -1f))
            {
                this.logoRotationSpeed--;
            }
            this.logoScale += this.logoScaleSpeed * 1E-05f;
            if (this.logoScale > 1.1)
            {
                this.logoScaleDirection = -1f;
            }
            else if (this.logoScale < 0.9)
            {
                this.logoScaleDirection = 1f;
            }
            if ((this.logoScaleSpeed < 50f) & (this.logoScaleDirection == 1f))
            {
                this.logoScaleSpeed++;
            }
            else if ((this.logoScaleSpeed > -50f) & (this.logoScaleDirection == -1f))
            {
                this.logoScaleSpeed--;
            }
            this.spriteBatch.Draw(logoTexture, new Vector2((float) (screenWidth / 2), 100f), new Rectangle(0, 0, logoTexture.Width, logoTexture.Height), color, this.logoRotation, new Vector2((float) (logoTexture.Width / 2), (float) (logoTexture.Height / 2)), this.logoScale, SpriteEffects.None, 0f);
            int num3 = 250;
            int num4 = screenWidth / 2;
            int num5 = 80;
            int num6 = 0;
            int menuMode = Main.menuMode;
            int index = -1;
            int num9 = 0;
            int num10 = 0;
            bool flag = false;
            bool flag2 = false;
            bool flag3 = false;
            int num11 = 0;
            bool[] flagArray = new bool[maxMenuItems];
            bool[] flagArray2 = new bool[maxMenuItems];
            int[] numArray = new int[maxMenuItems];
            int[] numArray2 = new int[maxMenuItems];
            for (int j = 0; j < maxMenuItems; j++)
            {
                flagArray[j] = false;
                flagArray2[j] = false;
                numArray[j] = 0;
                numArray2[j] = 0;
            }
            string[] strArray = new string[maxMenuItems];
            if (Main.menuMode == -1)
            {
                Main.menuMode = 0;
            }
            if (netMode == 2)
            {
                bool flag4 = true;
                for (int num13 = 0; num13 < 8; num13++)
                {
                    if (num13 < 8)
                    {
                        try
                        {
                            strArray[num13] = Netplay.serverSock[num13].statusText;
                            if (Netplay.serverSock[num13].active && showSpam)
                            {
                                IntPtr ptr = (IntPtr) 12;
                                strArray3 = strArray;
                                object obj2 = strArray3[(int)ptr];
                                (strArray3 = strArray)[(int) (ptr = (IntPtr) num13)] = string.Concat(new object[] { obj2, " (", NetMessage.buffer[num13].spamCount, ")" });
                            }
                        }
                        catch
                        {
                            strArray[num13] = "";
                        }
                        flagArray[num13] = true;
                        if ((strArray[num13] != "") && (strArray[num13] != null))
                        {
                            flag4 = false;
                        }
                    }
                }
                if (flag4)
                {
                    strArray[0] = "Start a new instance of Terraria to join!";
                    strArray[1] = "Running on port " + Netplay.serverPort;
                }
                num6 = 11;
                strArray[9] = statusText;
                flagArray[9] = true;
                num3 = 170;
                num5 = 30;
                numArray[10] = 20;
                numArray[10] = 40;
                strArray[10] = "Disconnect";
                if (this.selectedMenu == 10)
                {
                    Netplay.disconnect = true;
                    PlaySound(11, -1, -1, 1);
                }
            }
            else if (Main.menuMode == 0x1f)
            {
                string password = Netplay.password;
                Netplay.password = GetInputText(Netplay.password);
                if (password != Netplay.password)
                {
                    PlaySound(12, -1, -1, 1);
                }
                strArray[0] = "Server Requires Password:";
                this.textBlinkerCount++;
                if (this.textBlinkerCount >= 20)
                {
                    if (this.textBlinkerState == 0)
                    {
                        this.textBlinkerState = 1;
                    }
                    else
                    {
                        this.textBlinkerState = 0;
                    }
                    this.textBlinkerCount = 0;
                }
                strArray[1] = Netplay.password;
                if (this.textBlinkerState == 1)
                {
                    (strArray3 = strArray)[1] = strArray3[1] + "|";
                    numArray2[1] = 1;
                }
                else
                {
                    (strArray3 = strArray)[1] = strArray3[1] + " ";
                }
                flagArray[0] = true;
                flagArray[1] = true;
                numArray[1] = -20;
                numArray[2] = 20;
                strArray[2] = "Accept";
                strArray[3] = "Back";
                num6 = 4;
                if (this.selectedMenu == 3)
                {
                    PlaySound(11, -1, -1, 1);
                    Main.menuMode = 0;
                    Netplay.disconnect = true;
                    Netplay.password = "";
                }
                else if ((this.selectedMenu == 2) || inputTextEnter)
                {
                    NetMessage.SendData(0x26, -1, -1, Netplay.password, 0, 0f, 0f, 0f);
                    Main.menuMode = 14;
                }
            }
            else if ((netMode == 1) || (Main.menuMode == 14))
            {
                num6 = 2;
                strArray[0] = statusText;
                flagArray[0] = true;
                num3 = 300;
                strArray[1] = "Cancel";
                if (this.selectedMenu == 1)
                {
                    Netplay.disconnect = true;
                    Netplay.clientSock.tcpClient.Close();
                    PlaySound(11, -1, -1, 1);
                    Main.menuMode = 0;
                    netMode = 0;
                }
            }
            else if (Main.menuMode == 30)
            {
                string str2 = Netplay.password;
                Netplay.password = GetInputText(Netplay.password);
                if (str2 != Netplay.password)
                {
                    PlaySound(12, -1, -1, 1);
                }
                strArray[0] = "Enter Server Password:";
                this.textBlinkerCount++;
                if (this.textBlinkerCount >= 20)
                {
                    if (this.textBlinkerState == 0)
                    {
                        this.textBlinkerState = 1;
                    }
                    else
                    {
                        this.textBlinkerState = 0;
                    }
                    this.textBlinkerCount = 0;
                }
                strArray[1] = Netplay.password;
                if (this.textBlinkerState == 1)
                {
                    (strArray3 = strArray)[1] = strArray3[1] + "|";
                    numArray2[1] = 1;
                }
                else
                {
                    (strArray3 = strArray)[1] = strArray3[1] + " ";
                }
                flagArray[0] = true;
                flagArray[1] = true;
                numArray[1] = -20;
                numArray[2] = 20;
                strArray[2] = "Accept";
                strArray[3] = "Back";
                num6 = 4;
                if (this.selectedMenu == 3)
                {
                    PlaySound(11, -1, -1, 1);
                    Main.menuMode = 6;
                    Netplay.password = "";
                }
                else if ((this.selectedMenu == 2) || inputTextEnter)
                {
                    WorldGen.serverLoadWorld();
                    Main.menuMode = 10;
                }
            }
            else if (Main.menuMode == 15)
            {
                num6 = 2;
                strArray[0] = statusText;
                flagArray[0] = true;
                num3 = 80;
                num5 = 400;
                strArray[1] = "Back";
                if (this.selectedMenu == 1)
                {
                    Netplay.disconnect = true;
                    PlaySound(11, -1, -1, 1);
                    Main.menuMode = 0;
                    netMode = 0;
                }
            }
            else if (Main.menuMode == 10)
            {
                num6 = 1;
                strArray[0] = statusText;
                flagArray[0] = true;
                num3 = 300;
            }
            else if (Main.menuMode == 0)
            {
                menuMultiplayer = false;
                netMode = 0;
                strArray[0] = "Start Server";
                strArray[1] = "Settings";
                strArray[2] = "Exit";
                num6 = 4;
                if (this.selectedMenu == 2)
                {
                    this.QuitGame();
                }
                if (this.selectedMenu == 0)
                {
                    LoadWorlds();
                    PlaySound(10, -1, -1, 1);
                    Main.menuMode = 6;
                    menuMultiplayer = true;
                }
                if (this.selectedMenu == 1)
                {
                    PlaySound(10, -1, -1, 1);
                    Main.menuMode = 11;
                }
            }
            else if (Main.menuMode == 1)
            {
                //This isn't Free Terraria
            }
            else if (Main.menuMode == 2)
            {
                //Nor is this
            }
            else if (Main.menuMode == 20)
            {
                //Or this
            }
            else if (Main.menuMode == 0x11)
            {
                //Especially not this
            }
            else if (Main.menuMode == 0x13)
            {
                //Nope
            }
            else if (Main.menuMode == 0x15)
            {
                //
            }
            else if (Main.menuMode == 0x16)
            {
                //
            }
            else if (Main.menuMode == 0x17)
            {
                //
            }
            else if (Main.menuMode == 0x18)
            {
                //
            }
            else if (Main.menuMode == 3)
            {
                
            }
            else if (Main.menuMode == 4)
            {
                num3 = 220;
                num5 = 60;
                strArray[5] = "Back";
                for (int num16 = 0; num16 < 5; num16++)
                {
                    if (num16 < numLoadPlayers)
                    {
                        strArray[num16] = loadPlayer[num16].name;
                    }
                    else
                    {
                        strArray[num16] = null;
                    }
                }
                num6 = 6;
                if ((this.focusMenu >= 0) && (this.focusMenu < numLoadPlayers))
                {
                    index = this.focusMenu;
                    Vector2 vector2 = fontDeathText.MeasureString(strArray[index]);
                    num9 = (int) (((screenWidth / 2) + (vector2.X * 0.5)) + 10.0);
                    num10 = (num3 + (num5 * this.focusMenu)) + 4;
                }
                if (this.selectedMenu == 5)
                {
                    PlaySound(11, -1, -1, 1);
                    Main.menuMode = 1;
                }
                else if (this.selectedMenu >= 0)
                {
                    this.selectedPlayer = this.selectedMenu;
                    PlaySound(10, -1, -1, 1);
                    Main.menuMode = 5;
                }
            }
            else if (Main.menuMode == 5)
            {
                strArray[0] = "Delete " + loadPlayer[this.selectedPlayer].name + "?";
                flagArray[0] = true;
                strArray[1] = "Yes";
                strArray[2] = "No";
                num6 = 3;
                if (this.selectedMenu == 1)
                {
                    ErasePlayer(this.selectedPlayer);
                    PlaySound(10, -1, -1, 1);
                    Main.menuMode = 1;
                }
                else if (this.selectedMenu == 2)
                {
                    PlaySound(11, -1, -1, 1);
                    Main.menuMode = 1;
                }
            }
            else if (Main.menuMode == 6)
            {
                num3 = 190;
                num5 = 50;
                strArray[5] = "Create World";
                strArray[6] = "Delete";
                if (numLoadWorlds == 5)
                {
                    flagArray2[5] = true;
                    strArray[5] = "";
                }
                else if (numLoadWorlds == 0)
                {
                    flagArray2[6] = true;
                    strArray[6] = "";
                }
                strArray[7] = "Back";
                for (int num17 = 0; num17 < 5; num17++)
                {
                    if (num17 < numLoadWorlds)
                    {
                        strArray[num17] = loadWorld[num17];
                    }
                    else
                    {
                        strArray[num17] = null;
                    }
                }
                num6 = 8;
                if (this.selectedMenu == 7)
                {
                    if (menuMultiplayer)
                    {
                        Main.menuMode = 12;
                    }
                    else
                    {
                        Main.menuMode = 1;
                    }
                    PlaySound(11, -1, -1, 1);
                }
                else if (this.selectedMenu == 5)
                {
                    PlaySound(10, -1, -1, 1);
                    Main.menuMode = 0x10;
                    Main.newWorldName = "World " + (numLoadWorlds + 1);
                }
                else if (this.selectedMenu == 6)
                {
                    PlaySound(10, -1, -1, 1);
                    Main.menuMode = 8;
                }
                else if (this.selectedMenu >= 0)
                {
                    if (menuMultiplayer)
                    {
                        PlaySound(10, -1, -1, 1);
                        worldPathName = loadWorldPath[this.selectedMenu];
                        Main.menuMode = 30;
                    }
                    else
                    {
                        PlaySound(10, -1, -1, 1);
                        worldPathName = loadWorldPath[this.selectedMenu];
                        WorldGen.playWorld();
                        Main.menuMode = 10;
                    }
                }
            }
            else if (Main.menuMode == 7)
            {
                string newWorldName = Main.newWorldName;
                Main.newWorldName = GetInputText(Main.newWorldName);
                if (Main.newWorldName.Length > 20)
                {
                    Main.newWorldName = Main.newWorldName.Substring(0, 20);
                }
                if (newWorldName != Main.newWorldName)
                {
                    PlaySound(12, -1, -1, 1);
                }
                strArray[0] = "Enter World Name:";
                flagArray2[2] = true;
                if (Main.newWorldName != "")
                {
                    if (Main.newWorldName.Substring(0, 1) == " ")
                    {
                        Main.newWorldName = "";
                    }
                    for (int num18 = 0; num18 < Main.newWorldName.Length; num18++)
                    {
                        if (Main.newWorldName != " ")
                        {
                            flagArray2[2] = false;
                        }
                    }
                }
                this.textBlinkerCount++;
                if (this.textBlinkerCount >= 20)
                {
                    if (this.textBlinkerState == 0)
                    {
                        this.textBlinkerState = 1;
                    }
                    else
                    {
                        this.textBlinkerState = 0;
                    }
                    this.textBlinkerCount = 0;
                }
                strArray[1] = Main.newWorldName;
                if (this.textBlinkerState == 1)
                {
                    (strArray3 = strArray)[1] = strArray3[1] + "|";
                    numArray2[1] = 1;
                }
                else
                {
                    (strArray3 = strArray)[1] = strArray3[1] + " ";
                }
                flagArray[0] = true;
                flagArray[1] = true;
                numArray[1] = -20;
                numArray[2] = 20;
                strArray[2] = "Accept";
                strArray[3] = "Back";
                num6 = 4;
                if (this.selectedMenu == 3)
                {
                    PlaySound(11, -1, -1, 1);
                    Main.menuMode = 0x10;
                }
                if ((this.selectedMenu == 2) || (!flagArray2[2] && inputTextEnter))
                {
                    Main.menuMode = 10;
                    worldName = Main.newWorldName;
                    worldPathName = nextLoadWorld();
                    WorldGen.CreateNewWorld();
                }
            }
            else if (Main.menuMode == 8)
            {
                num3 = 220;
                num5 = 60;
                strArray[5] = "Back";
                for (int num19 = 0; num19 < 5; num19++)
                {
                    if (num19 < numLoadWorlds)
                    {
                        strArray[num19] = loadWorld[num19];
                    }
                    else
                    {
                        strArray[num19] = null;
                    }
                }
                num6 = 6;
                if (this.selectedMenu == 5)
                {
                    PlaySound(11, -1, -1, 1);
                    Main.menuMode = 1;
                }
                else if (this.selectedMenu >= 0)
                {
                    this.selectedWorld = this.selectedMenu;
                    PlaySound(10, -1, -1, 1);
                    Main.menuMode = 9;
                }
            }
            else if (Main.menuMode == 9)
            {
                strArray[0] = "Delete " + loadWorld[this.selectedWorld] + "?";
                flagArray[0] = true;
                strArray[1] = "Yes";
                strArray[2] = "No";
                num6 = 3;
                if (this.selectedMenu == 1)
                {
                    EraseWorld(this.selectedWorld);
                    PlaySound(10, -1, -1, 1);
                    Main.menuMode = 6;
                }
                else if (this.selectedMenu == 2)
                {
                    PlaySound(11, -1, -1, 1);
                    Main.menuMode = 6;
                }
            }
            else if (Main.menuMode == 11)
            {
                num3 = 180;
                num5 = 60;
                numArray[6] = 10;
                num6 = 7;
                if (this.graphics.IsFullScreen)
                {
                    strArray[0] = "Fullscreen Mode";
                }
                else
                {
                    strArray[0] = "Windowed Mode";
                }
                this.bgScroll = (int) Math.Round((double) ((1f - caveParrallax) * 500f));
                strArray[1] = "Cursor Color";
                strArray[2] = "Volume";
                strArray[3] = "Controls";
                strArray[4] = "Parallax";
                if (fixedTiming)
                {
                    strArray[5] = "Frame Skip Off";
                }
                else
                {
                    strArray[5] = "Frame Skip On";
                }
                strArray[6] = "Back";
                if (this.selectedMenu == 6)
                {
                    PlaySound(11, -1, -1, 1);
                    this.SaveSettings();
                    Main.menuMode = 0;
                }
                if (this.selectedMenu == 5)
                {
                    PlaySound(12, -1, -1, 1);
                    if (fixedTiming)
                    {
                        fixedTiming = false;
                    }
                    else
                    {
                        fixedTiming = true;
                    }
                }
                if (this.selectedMenu == 4)
                {
                    PlaySound(11, -1, -1, 1);
                    Main.menuMode = 0x1c;
                }
                if (this.selectedMenu == 3)
                {
                    PlaySound(11, -1, -1, 1);
                    Main.menuMode = 0x1b;
                }
                if (this.selectedMenu == 2)
                {
                    PlaySound(11, -1, -1, 1);
                    Main.menuMode = 0x1a;
                }
                if (this.selectedMenu == 1)
                {
                    PlaySound(10, -1, -1, 1);
                    this.selColor = mouseColor;
                    Main.menuMode = 0x19;
                }
                if (this.selectedMenu == 0)
                {
                    this.graphics.ToggleFullScreen();
                }
            }
            else if (Main.menuMode == 0x19)
            {
                flag = true;
                num11 = 370;
                num3 = 240;
                num5 = 60;
                mouseColor = this.selColor;
                num6 = 3;
                strArray[0] = "";
                strArray[1] = "Cursor Color";
                flagArray[1] = true;
                numArray[2] = 170;
                numArray[1] = 10;
                strArray[2] = "Back";
                if (this.selectedMenu == 2)
                {
                    Main.menuMode = 11;
                    PlaySound(11, -1, -1, 1);
                }
            }
            else if (Main.menuMode == 0x1a)
            {
                flag2 = true;
                num3 = 240;
                num5 = 60;
                num6 = 3;
                strArray[0] = "";
                strArray[1] = "Volume";
                flagArray[1] = true;
                numArray[2] = 170;
                numArray[1] = 10;
                strArray[2] = "Back";
                if (this.selectedMenu == 2)
                {
                    Main.menuMode = 11;
                    PlaySound(11, -1, -1, 1);
                }
            }
            else if (Main.menuMode == 0x1c)
            {
                caveParrallax = 1f - (((float) this.bgScroll) / 500f);
                flag3 = true;
                num3 = 240;
                num5 = 60;
                num6 = 3;
                strArray[0] = "";
                strArray[1] = "Parallax";
                flagArray[1] = true;
                numArray[2] = 170;
                numArray[1] = 10;
                strArray[2] = "Back";
                if (this.selectedMenu == 2)
                {
                    Main.menuMode = 11;
                    PlaySound(11, -1, -1, 1);
                }
            }
            else if (Main.menuMode == 0x1b)
            {
                num3 = 190;
                num5 = 50;
                num6 = 8;
                string[] strArray2 = new string[] { cUp, cDown, cLeft, cRight, cJump, cThrowItem, cInv };
                if (this.setKey >= 0)
                {
                    strArray2[this.setKey] = "_";
                }
                strArray[0] = "Up........." + strArray2[0];
                strArray[1] = "Down......." + strArray2[1];
                strArray[2] = "Left......." + strArray2[2];
                strArray[3] = "Right......" + strArray2[3];
                strArray[4] = "Jump......." + strArray2[4];
                strArray[5] = "Throw......" + strArray2[5];
                strArray[6] = "Inventory.." + strArray2[6];
                numArray[7] = 10;
                strArray[7] = "Back";
                if (this.selectedMenu == 7)
                {
                    Main.menuMode = 11;
                    PlaySound(11, -1, -1, 1);
                }
                else if (this.selectedMenu >= 0)
                {
                    this.setKey = this.selectedMenu;
                }
                if (this.setKey >= 0)
                {
                    Keys[] pressedKeys = keyState.GetPressedKeys();
                    if (pressedKeys.Length > 0)
                    {
                        string str5 = Convert.ToString(pressedKeys.GetValue(0));
                        if (this.setKey == 0)
                        {
                            cUp = str5;
                        }
                        if (this.setKey == 1)
                        {
                            cDown = str5;
                        }
                        if (this.setKey == 2)
                        {
                            cLeft = str5;
                        }
                        if (this.setKey == 3)
                        {
                            cRight = str5;
                        }
                        if (this.setKey == 4)
                        {
                            cJump = str5;
                        }
                        if (this.setKey == 5)
                        {
                            cThrowItem = str5;
                        }
                        if (this.setKey == 6)
                        {
                            cInv = str5;
                        }
                        this.setKey = -1;
                    }
                }
            }
            else if (Main.menuMode == 12)
            {
                PlaySound(11, -1, -1, 1);
                Main.menuMode = 0;
            }
            else if (Main.menuMode == 13)
            {
                string getIP = Main.getIP;
                Main.getIP = GetInputText(Main.getIP);
                if (getIP != Main.getIP)
                {
                    PlaySound(12, -1, -1, 1);
                }
                strArray[0] = "Enter Server IP Address:";
                flagArray2[2] = true;
                if (Main.getIP != "")
                {
                    if (Main.getIP.Substring(0, 1) == " ")
                    {
                        Main.getIP = "";
                    }
                    for (int num20 = 0; num20 < Main.getIP.Length; num20++)
                    {
                        if (Main.getIP != " ")
                        {
                            flagArray2[2] = false;
                        }
                    }
                }
                this.textBlinkerCount++;
                if (this.textBlinkerCount >= 20)
                {
                    if (this.textBlinkerState == 0)
                    {
                        this.textBlinkerState = 1;
                    }
                    else
                    {
                        this.textBlinkerState = 0;
                    }
                    this.textBlinkerCount = 0;
                }
                strArray[1] = Main.getIP;
                if (this.textBlinkerState == 1)
                {
                    (strArray3 = strArray)[1] = strArray3[1] + "|";
                    numArray2[1] = 1;
                }
                else
                {
                    (strArray3 = strArray)[1] = strArray3[1] + " ";
                }
                flagArray[0] = true;
                flagArray[1] = true;
                numArray[1] = -20;
                numArray[2] = 20;
                strArray[2] = "Accept";
                strArray[3] = "Back";
                num6 = 4;
                if (this.selectedMenu == 3)
                {
                    PlaySound(11, -1, -1, 1);
                    Main.menuMode = 1;
                }
                if ((this.selectedMenu == 2) || (!flagArray2[2] && inputTextEnter))
                {
                    if (Netplay.SetIP(Main.getIP))
                    {
                        Main.menuMode = 10;
                        Netplay.StartClient();
                    }
                    else if (Netplay.SetIP2(Main.getIP))
                    {
                        Main.menuMode = 10;
                        Netplay.StartClient();
                    }
                }
            }
            else if (Main.menuMode == 0x10)
            {
                num3 = 200;
                num5 = 60;
                numArray[1] = 30;
                numArray[2] = 30;
                numArray[3] = 30;
                numArray[4] = 70;
                strArray[0] = "Choose world size:";
                flagArray[0] = true;
                strArray[1] = "Small";
                strArray[2] = "Medium";
                strArray[3] = "Large";
                strArray[4] = "Back";
                num6 = 5;
                if (this.selectedMenu == 4)
                {
                    Main.menuMode = 6;
                    PlaySound(11, -1, -1, 1);
                }
                else if (this.selectedMenu > 0)
                {
                    if (this.selectedMenu == 1)
                    {
                        maxTilesX = 0x1068;
                        maxTilesY = 0x4b0;
                    }
                    else if (this.selectedMenu == 2)
                    {
                        maxTilesX = 0x189c;
                        maxTilesY = 0x708;
                    }
                    else
                    {
                        maxTilesX = 0x20d0;
                        maxTilesY = 0x960;
                    }
                    Main.menuMode = 7;
                    PlaySound(10, -1, -1, 1);
                    WorldGen.setWorldSize();
                }
            }
            if (Main.menuMode != menuMode)
            {
                num6 = 0;
                for (int num21 = 0; num21 < maxMenuItems; num21++)
                {
                    this.menuItemScale[num21] = 0.8f;
                }
            }
            int focusMenu = this.focusMenu;
            this.selectedMenu = -1;
            this.focusMenu = -1;
            for (int k = 0; k < num6; k++)
            {
                if (strArray[k] != null)
                {
                    if (flag)
                    {
                        string text = "";
                        for (int num24 = 0; num24 < 6; num24++)
                        {
                            int num25 = num11;
                            int num26 = 370;
                            switch (num24)
                            {
                                case 0:
                                    text = "Red:";
                                    break;

                                case 1:
                                    text = "Green:";
                                    num25 += 30;
                                    break;

                                case 2:
                                    text = "Blue:";
                                    num25 += 60;
                                    break;

                                case 3:
                                    text = this.selColor.R.ToString();
                                    num26 += 90;
                                    break;

                                case 4:
                                    text = this.selColor.G.ToString();
                                    num26 += 90;
                                    num25 += 30;
                                    break;

                                case 5:
                                    text = this.selColor.B.ToString();;
                                    num26 += 90;
                                    num25 += 60;
                                    break;
                            }
                            for (int num27 = 0; num27 < 5; num27++)
                            {
                                Color black = Color.Black;
                                if (num27 == 4)
                                {
                                    black = color;
                                    black.R = (byte) ((0xff + black.R) / 2);
                                    black.G = (byte) ((0xff + black.R) / 2);
                                    black.B = (byte) ((0xff + black.R) / 2);
                                }
                                int num28 = 0xff;
                                int num29 = black.R - (0xff - num28);
                                if (num29 < 0)
                                {
                                    num29 = 0;
                                }
                                black = new Color((int) ((byte) num29), (int) ((byte) num29), (int) ((byte) num29), (int) ((byte) num28));
                                int num30 = 0;
                                int num31 = 0;
                                switch (num27)
                                {
                                    case 0:
                                        num30 = -2;
                                        break;

                                    case 1:
                                        num30 = 2;
                                        break;

                                    case 2:
                                        num31 = -2;
                                        break;

                                    case 3:
                                        num31 = 2;
                                        break;
                                }
                                vector9 = new Vector2();
                                this.spriteBatch.DrawString(fontDeathText, text, new Vector2((float) (num26 + num30), (float) (num25 + num31)), black, 0f, vector9, (float) 0.5f, SpriteEffects.None, 0f);
                            }
                        }
                        bool flag5 = false;
                        for (int num32 = 0; num32 < 2; num32++)
                        {
                            for (int num33 = 0; num33 < 3; num33++)
                            {
                                int num34 = (num11 + (num33 * 30)) - 12;
                                int num35 = 360;
                                float scale = 0.9f;
                                if (num32 == 0)
                                {
                                    num35 -= 70;
                                    num34 += 2;
                                }
                                else
                                {
                                    num35 -= 40;
                                }
                                text = "-";
                                if (num32 == 1)
                                {
                                    text = "+";
                                }
                                Vector2 vector3 = new Vector2(24f, 24f);
                                int num37 = 0x8e;
                                if (((mouseState.X > num35) && (mouseState.X < (num35 + vector3.X))) && ((mouseState.Y > (num34 + 13)) && (mouseState.Y < ((num34 + 13) + vector3.Y))))
                                {
                                    if (this.focusColor != ((num32 + 1) * (num33 + 10)))
                                    {
                                        PlaySound(12, -1, -1, 1);
                                    }
                                    this.focusColor = (num32 + 1) * (num33 + 10);
                                    flag5 = true;
                                    num37 = 0xff;
                                    if (mouseState.LeftButton == ButtonState.Pressed)
                                    {
                                        if (this.colorDelay <= 1)
                                        {
                                            if (this.colorDelay == 0)
                                            {
                                                this.colorDelay = 40;
                                            }
                                            else
                                            {
                                                this.colorDelay = 3;
                                            }
                                            int num38 = num32;
                                            if (num32 == 0)
                                            {
                                                num38 = -1;
                                                if (((this.selColor.R + this.selColor.G) + this.selColor.B) < 0xff)
                                                {
                                                    num38 = 0;
                                                }
                                            }
                                            if (((num33 == 0) && ((this.selColor.R + num38) >= 0)) && ((this.selColor.R + num38) <= 0xff))
                                            {
                                                this.selColor.R = (byte) (this.selColor.R + num38);
                                            }
                                            if (((num33 == 1) && ((this.selColor.G + num38) >= 0)) && ((this.selColor.G + num38) <= 0xff))
                                            {
                                                this.selColor.G = (byte) (this.selColor.G + num38);
                                            }
                                            if (((num33 == 2) && ((this.selColor.B + num38) >= 0)) && ((this.selColor.B + num38) <= 0xff))
                                            {
                                                this.selColor.B = (byte) (this.selColor.B + num38);
                                            }
                                        }
                                        this.colorDelay--;
                                    }
                                    else
                                    {
                                        this.colorDelay = 0;
                                    }
                                }
                                for (int num39 = 0; num39 < 5; num39++)
                                {
                                    Color color3 = Color.Black;
                                    if (num39 == 4)
                                    {
                                        color3 = color;
                                        color3.R = (byte) ((0xff + color3.R) / 2);
                                        color3.G = (byte) ((0xff + color3.R) / 2);
                                        color3.B = (byte) ((0xff + color3.R) / 2);
                                    }
                                    int num40 = color3.R - (0xff - num37);
                                    if (num40 < 0)
                                    {
                                        num40 = 0;
                                    }
                                    color3 = new Color((int) ((byte) num40), (int) ((byte) num40), (int) ((byte) num40), (int) ((byte) num37));
                                    int num41 = 0;
                                    int num42 = 0;
                                    switch (num39)
                                    {
                                        case 0:
                                            num41 = -2;
                                            break;

                                        case 1:
                                            num41 = 2;
                                            break;

                                        case 2:
                                            num42 = -2;
                                            break;

                                        case 3:
                                            num42 = 2;
                                            break;
                                    }
                                    vector9 = new Vector2();
                                    this.spriteBatch.DrawString(fontDeathText, text, new Vector2((float) (num35 + num41), (float) (num34 + num42)), color3, 0f, vector9, scale, SpriteEffects.None, 0f);
                                }
                            }
                        }
                        if (!flag5)
                        {
                            this.focusColor = 0;
                            this.colorDelay = 0;
                        }
                    }
                    if (flag3)
                    {
                        int num43 = 400;
                        string str8 = "";
                        for (int num44 = 0; num44 < 4; num44++)
                        {
                            int num45 = num43;
                            int num46 = 370;
                            if (num44 == 0)
                            {
                                str8 = "Parallax: " + this.bgScroll;
                            }
                            for (int num47 = 0; num47 < 5; num47++)
                            {
                                Color color4 = Color.Black;
                                if (num47 == 4)
                                {
                                    color4 = color;
                                    color4.R = (byte) ((0xff + color4.R) / 2);
                                    color4.G = (byte) ((0xff + color4.R) / 2);
                                    color4.B = (byte) ((0xff + color4.R) / 2);
                                }
                                int num48 = 0xff;
                                int num49 = color4.R - (0xff - num48);
                                if (num49 < 0)
                                {
                                    num49 = 0;
                                }
                                color4 = new Color((int) ((byte) num49), (int) ((byte) num49), (int) ((byte) num49), (int) ((byte) num48));
                                int num50 = 0;
                                int num51 = 0;
                                switch (num47)
                                {
                                    case 0:
                                        num50 = -2;
                                        break;

                                    case 1:
                                        num50 = 2;
                                        break;

                                    case 2:
                                        num51 = -2;
                                        break;

                                    case 3:
                                        num51 = 2;
                                        break;
                                }
                                vector9 = new Vector2();
                                this.spriteBatch.DrawString(fontDeathText, str8, new Vector2((float) (num46 + num50), (float) (num45 + num51)), color4, 0f, vector9, (float) 0.5f, SpriteEffects.None, 0f);
                            }
                        }
                        bool flag6 = false;
                        for (int num52 = 0; num52 < 2; num52++)
                        {
                            for (int num53 = 0; num53 < 1; num53++)
                            {
                                int num54 = (num43 + (num53 * 30)) - 12;
                                int num55 = 360;
                                float num56 = 0.9f;
                                if (num52 == 0)
                                {
                                    num55 -= 70;
                                    num54 += 2;
                                }
                                else
                                {
                                    num55 -= 40;
                                }
                                str8 = "-";
                                if (num52 == 1)
                                {
                                    str8 = "+";
                                }
                                Vector2 vector4 = new Vector2(24f, 24f);
                                int num57 = 0x8e;
                                if (((mouseState.X > num55) && (mouseState.X < (num55 + vector4.X))) && ((mouseState.Y > (num54 + 13)) && (mouseState.Y < ((num54 + 13) + vector4.Y))))
                                {
                                    if (this.focusColor != ((num52 + 1) * (num53 + 10)))
                                    {
                                        PlaySound(12, -1, -1, 1);
                                    }
                                    this.focusColor = (num52 + 1) * (num53 + 10);
                                    flag6 = true;
                                    num57 = 0xff;
                                    if (mouseState.LeftButton == ButtonState.Pressed)
                                    {
                                        if (this.colorDelay <= 1)
                                        {
                                            if (this.colorDelay == 0)
                                            {
                                                this.colorDelay = 40;
                                            }
                                            else
                                            {
                                                this.colorDelay = 3;
                                            }
                                            int num58 = num52;
                                            if (num52 == 0)
                                            {
                                                num58 = -1;
                                            }
                                            if (num53 == 0)
                                            {
                                                this.bgScroll += num58;
                                                if (this.bgScroll > 100)
                                                {
                                                    this.bgScroll = 100;
                                                }
                                                if (this.bgScroll < 0)
                                                {
                                                    this.bgScroll = 0;
                                                }
                                            }
                                        }
                                        this.colorDelay--;
                                    }
                                    else
                                    {
                                        this.colorDelay = 0;
                                    }
                                }
                                for (int num59 = 0; num59 < 5; num59++)
                                {
                                    Color color5 = Color.Black;
                                    if (num59 == 4)
                                    {
                                        color5 = color;
                                        color5.R = (byte) ((0xff + color5.R) / 2);
                                        color5.G = (byte) ((0xff + color5.R) / 2);
                                        color5.B = (byte) ((0xff + color5.R) / 2);
                                    }
                                    int num60 = color5.R - (0xff - num57);
                                    if (num60 < 0)
                                    {
                                        num60 = 0;
                                    }
                                    color5 = new Color((int) ((byte) num60), (int) ((byte) num60), (int) ((byte) num60), (int) ((byte) num57));
                                    int num61 = 0;
                                    int num62 = 0;
                                    switch (num59)
                                    {
                                        case 0:
                                            num61 = -2;
                                            break;

                                        case 1:
                                            num61 = 2;
                                            break;

                                        case 2:
                                            num62 = -2;
                                            break;

                                        case 3:
                                            num62 = 2;
                                            break;
                                    }
                                    vector9 = new Vector2();
                                    this.spriteBatch.DrawString(fontDeathText, str8, new Vector2((float) (num55 + num61), (float) (num54 + num62)), color5, 0f, vector9, num56, SpriteEffects.None, 0f);
                                }
                            }
                        }
                        if (!flag6)
                        {
                            this.focusColor = 0;
                            this.colorDelay = 0;
                        }
                    }
                    if (flag2)
                    {
                        int num63 = 400;
                        string str9 = "";
                        for (int num64 = 0; num64 < 4; num64++)
                        {
                            int num65 = num63;
                            int num66 = 370;
                            switch (num64)
                            {
                                case 0:
                                    str9 = "Sound:";
                                    break;

                                case 1:
                                    str9 = "Music:";
                                    num65 += 30;
                                    break;

                                case 2:
                                    str9 = Math.Round((double) (soundVolume * 100f)) + "%";
                                    num66 += 90;
                                    break;

                                case 3:
                                    str9 = Math.Round((double) (musicVolume * 100f)) + "%";
                                    num66 += 90;
                                    num65 += 30;
                                    break;
                            }
                            for (int num67 = 0; num67 < 5; num67++)
                            {
                                Color color6 = Color.Black;
                                if (num67 == 4)
                                {
                                    color6 = color;
                                    color6.R = (byte) ((0xff + color6.R) / 2);
                                    color6.G = (byte) ((0xff + color6.R) / 2);
                                    color6.B = (byte) ((0xff + color6.R) / 2);
                                }
                                int num68 = 0xff;
                                int num69 = color6.R - (0xff - num68);
                                if (num69 < 0)
                                {
                                    num69 = 0;
                                }
                                color6 = new Color((int) ((byte) num69), (int) ((byte) num69), (int) ((byte) num69), (int) ((byte) num68));
                                int num70 = 0;
                                int num71 = 0;
                                switch (num67)
                                {
                                    case 0:
                                        num70 = -2;
                                        break;

                                    case 1:
                                        num70 = 2;
                                        break;

                                    case 2:
                                        num71 = -2;
                                        break;

                                    case 3:
                                        num71 = 2;
                                        break;
                                }
                                vector9 = new Vector2();
                                this.spriteBatch.DrawString(fontDeathText, str9, new Vector2((float) (num66 + num70), (float) (num65 + num71)), color6, 0f, vector9, (float) 0.5f, SpriteEffects.None, 0f);
                            }
                        }
                        bool flag7 = false;
                        for (int num72 = 0; num72 < 2; num72++)
                        {
                            for (int num73 = 0; num73 < 2; num73++)
                            {
                                int num79;
                                int num74 = (num63 + (num73 * 30)) - 12;
                                int num75 = 360;
                                float num76 = 0.9f;
                                if (num72 == 0)
                                {
                                    num75 -= 70;
                                    num74 += 2;
                                }
                                else
                                {
                                    num75 -= 40;
                                }
                                str9 = "-";
                                if (num72 == 1)
                                {
                                    str9 = "+";
                                }
                                Vector2 vector5 = new Vector2(24f, 24f);
                                int num77 = 0x8e;
                                if (((mouseState.X <= num75) || (mouseState.X >= (num75 + vector5.X))) || ((mouseState.Y <= (num74 + 13)) || (mouseState.Y >= ((num74 + 13) + vector5.Y))))
                                {
                                    goto Label_3001;
                                }
                                if (this.focusColor != ((num72 + 1) * (num73 + 10)))
                                {
                                    PlaySound(12, -1, -1, 1);
                                }
                                this.focusColor = (num72 + 1) * (num73 + 10);
                                flag7 = true;
                                num77 = 0xff;
                                if (mouseState.LeftButton != ButtonState.Pressed)
                                {
                                    goto Label_2FFA;
                                }
                                if (this.colorDelay <= 1)
                                {
                                    if (this.colorDelay == 0)
                                    {
                                        this.colorDelay = 40;
                                    }
                                    else
                                    {
                                        this.colorDelay = 3;
                                    }
                                    int num78 = num72;
                                    if (num72 == 0)
                                    {
                                        num78 = -1;
                                    }
                                    switch (num73)
                                    {
                                        case 0:
                                            soundVolume += num78 * 0.01f;
                                            if (soundVolume > 1f)
                                            {
                                                soundVolume = 1f;
                                            }
                                            if (soundVolume < 0f)
                                            {
                                                soundVolume = 0f;
                                            }
                                            break;

                                        case 1:
                                            musicVolume += num78 * 0.01f;
                                            if (musicVolume > 1f)
                                            {
                                                musicVolume = 1f;
                                            }
                                            if (musicVolume < 0f)
                                            {
                                                musicVolume = 0f;
                                            }
                                            goto Label_2FEA;
                                    }
                                }
                            Label_2FEA:
                                this.colorDelay--;
                                goto Label_3001;
                            Label_2FFA:
                                this.colorDelay = 0;
                            Label_3001:
                                num79 = 0;
                                while (num79 < 5)
                                {
                                    Color color7 = Color.Black;
                                    if (num79 == 4)
                                    {
                                        color7 = color;
                                        color7.R = (byte) ((0xff + color7.R) / 2);
                                        color7.G = (byte) ((0xff + color7.R) / 2);
                                        color7.B = (byte) ((0xff + color7.R) / 2);
                                    }
                                    int num80 = color7.R - (0xff - num77);
                                    if (num80 < 0)
                                    {
                                        num80 = 0;
                                    }
                                    color7 = new Color((int) ((byte) num80), (int) ((byte) num80), (int) ((byte) num80), (int) ((byte) num77));
                                    int num81 = 0;
                                    int num82 = 0;
                                    switch (num79)
                                    {
                                        case 0:
                                            num81 = -2;
                                            break;

                                        case 1:
                                            num81 = 2;
                                            break;

                                        case 2:
                                            num82 = -2;
                                            break;

                                        case 3:
                                            num82 = 2;
                                            break;
                                    }
                                    vector9 = new Vector2();
                                    this.spriteBatch.DrawString(fontDeathText, str9, new Vector2((float) (num75 + num81), (float) (num74 + num82)), color7, 0f, vector9, num76, SpriteEffects.None, 0f);
                                    num79++;
                                }
                            }
                        }
                        if (!flag7)
                        {
                            this.focusColor = 0;
                            this.colorDelay = 0;
                        }
                    }
                    for (int num83 = 0; num83 < 5; num83++)
                    {
                        Color color8 = Color.Black;
                        if (num83 == 4)
                        {
                            color8 = color;
                            color8.R = (byte) ((0xff + color8.R) / 2);
                            color8.G = (byte) ((0xff + color8.R) / 2);
                            color8.B = (byte) ((0xff + color8.R) / 2);
                        }
                        int num84 = (int) (255f * ((this.menuItemScale[k] * 2f) - 1f));
                        if (flagArray[k])
                        {
                            num84 = 0xff;
                        }
                        int num85 = color8.R - (0xff - num84);
                        if (num85 < 0)
                        {
                            num85 = 0;
                        }
                        color8 = new Color((int) ((byte) num85), (int) ((byte) num85), (int) ((byte) num85), (int) ((byte) num84));
                        int num86 = 0;
                        int num87 = 0;
                        switch (num83)
                        {
                            case 0:
                                num86 = -2;
                                break;

                            case 1:
                                num86 = 2;
                                break;

                            case 2:
                                num87 = -2;
                                break;

                            case 3:
                                num87 = 2;
                                break;
                        }
                        Vector2 origin = fontDeathText.MeasureString(strArray[k]);
                        origin.X *= 0.5f;
                        origin.Y *= 0.5f;
                        float num88 = this.menuItemScale[k];
                        if ((Main.menuMode == 15) && (k == 0))
                        {
                            num88 *= 0.35f;
                        }
                        else if (netMode == 2)
                        {
                            num88 *= 0.5f;
                        }
                        this.spriteBatch.DrawString(fontDeathText, strArray[k], new Vector2((float) ((num4 + num86) + numArray2[k]), (((num3 + (num5 * k)) + num87) + origin.Y) + numArray[k]), color8, 0f, origin, num88, SpriteEffects.None, 0f);
                    }
                    if ((((mouseState.X > ((num4 - (strArray[k].Length * 10)) + numArray2[k])) && (mouseState.X < ((num4 + (strArray[k].Length * 10)) + numArray2[k]))) && ((mouseState.Y > ((num3 + (num5 * k)) + numArray[k])) && (mouseState.Y < (((num3 + (num5 * k)) + 50) + numArray[k])))) && hasFocus)
                    {
                        this.focusMenu = k;
                        if (flagArray[k] || flagArray2[k])
                        {
                            this.focusMenu = -1;
                        }
                        else
                        {
                            if (focusMenu != this.focusMenu)
                            {
                                PlaySound(12, -1, -1, 1);
                            }
                            if (mouseLeftRelease && (mouseState.LeftButton == ButtonState.Pressed))
                            {
                                this.selectedMenu = k;
                            }
                        }
                    }
                }
            }
            for (int m = 0; m < maxMenuItems; m++)
            {
                if (m == this.focusMenu)
                {
                    if (this.menuItemScale[m] < 1f)
                    {
                        this.menuItemScale[m] += 0.02f;
                    }
                    if (this.menuItemScale[m] > 1f)
                    {
                        this.menuItemScale[m] = 1f;
                    }
                }
                else if (this.menuItemScale[m] > 0.8)
                {
                    this.menuItemScale[m] -= 0.02f;
                }
            }
            if (index >= 0)
            {
                loadPlayer[index].PlayerFrame();
                loadPlayer[index].position.X = num9 + screenPosition.X;
                loadPlayer[index].position.Y = num10 + screenPosition.Y;
                this.DrawPlayer(loadPlayer[index]);
            }
            for (int n = 0; n < 5; n++)
            {
                Color color9 = Color.Black;
                if (n == 4)
                {
                    color9 = color;
                    color9.R = (byte) ((0xff + color9.R) / 2);
                    color9.G = (byte) ((0xff + color9.R) / 2);
                    color9.B = (byte) ((0xff + color9.R) / 2);
                }
                color9.A = (byte) (color9.A * 0.3f);
                int num91 = 0;
                int num92 = 0;
                switch (n)
                {
                    case 0:
                        num91 = -2;
                        break;

                    case 1:
                        num91 = 2;
                        break;

                    case 2:
                        num92 = -2;
                        break;

                    case 3:
                        num92 = 2;
                        break;
                }
                string str10 = "Copyright 2011 Re-Logic | TShock By: Zach & Shank | Help from: High & MMavipc";
                Vector2 vector7 = fontMouseText.MeasureString(str10);
                vector7.X *= 0.5f;
                vector7.Y *= 0.5f;
                this.spriteBatch.DrawString(fontMouseText, str10, new Vector2(((screenWidth - vector7.X) + num91) - 10f, ((screenHeight - vector7.Y) + num92) - 2f), color9, 0f, vector7, (float) 1f, SpriteEffects.None, 0f);
            }
            for (int num93 = 0; num93 < 5; num93++)
            {
                Color color10 = Color.Black;
                if (num93 == 4)
                {
                    color10 = color;
                    color10.R = (byte) ((0xff + color10.R) / 2);
                    color10.G = (byte) ((0xff + color10.R) / 2);
                    color10.B = (byte) ((0xff + color10.R) / 2);
                }
                color10.A = (byte) (color10.A * 0.3f);
                int num94 = 0;
                int num95 = 0;
                switch (num93)
                {
                    case 0:
                        num94 = -2;
                        break;

                    case 1:
                        num94 = 2;
                        break;

                    case 2:
                        num95 = -2;
                        break;

                    case 3:
                        num95 = 2;
                        break;
                }
                Vector2 vector8 = fontMouseText.MeasureString(versionNumber);
                vector8.X *= 0.5f;
                vector8.Y *= 0.5f;
                this.spriteBatch.DrawString(fontMouseText, versionNumber, new Vector2((vector8.X + num94) + 10f, ((screenHeight - vector8.Y) + num95) - 2f), color10, 0f, vector8, (float) 1f, SpriteEffects.None, 0f);
            }
            vector9 = new Vector2();
            this.spriteBatch.Draw(cursorTexture, new Vector2((float) (mouseState.X + 1), (float) (mouseState.Y + 1)), new Rectangle(0, 0, cursorTexture.Width, cursorTexture.Height), new Color((int) (cursorColor.R * 0.2f), (int) (cursorColor.G * 0.2f), (int) (cursorColor.B * 0.2f), (int) (cursorColor.A * 0.5f)), 0f, vector9, (float) (cursorScale * 1.1f), SpriteEffects.None, 0f);
            vector9 = new Vector2();
            this.spriteBatch.Draw(cursorTexture, new Vector2((float) mouseState.X, (float) mouseState.Y), new Rectangle(0, 0, cursorTexture.Width, cursorTexture.Height), cursorColor, 0f, vector9, cursorScale, SpriteEffects.None, 0f);
            if (fadeCounter > 0)
            {
                Color white = Color.White;
                byte num96 = 0;
                fadeCounter--;
                float num97 = (((float) fadeCounter) / 75f) * 255f;
                num96 = (byte) num97;
                white = new Color((int) num96, (int) num96, (int) num96, (int) num96);
                this.spriteBatch.Draw(fadeTexture, new Vector2(0f, 0f), new Rectangle(0, 0, splashTexture.Width, splashTexture.Height), white, 0f, new Vector2(), (float) 1f, SpriteEffects.None, 0f);
            }
            this.spriteBatch.End();
            if (mouseState.LeftButton == ButtonState.Pressed)
            {
                mouseLeftRelease = false;
            }
            else
            {
                mouseLeftRelease = true;
            }
            if (mouseState.RightButton == ButtonState.Pressed)
            {
                mouseRightRelease = false;
            }
            else
            {
                mouseRightRelease = true;
            }
        }

        protected void DrawNPCs(bool behindTiles = false)
        {
            Rectangle rectangle = new Rectangle(((int) screenPosition.X) - 300, ((int) screenPosition.Y) - 300, screenWidth + 600, screenHeight + 600);
            for (int i = 0x3e7; i >= 0; i--)
            {
                if ((npc[i].active && (npc[i].type > 0)) && ((npc[i].behindTiles == behindTiles) && rectangle.Intersects(new Rectangle((int) npc[i].position.X, (int) npc[i].position.Y, npc[i].width, npc[i].height))))
                {
                    if (npc[i].aiStyle == 13)
                    {
                        Vector2 vector = new Vector2(npc[i].position.X + (npc[i].width / 2), npc[i].position.Y + (npc[i].height / 2));
                        float num2 = ((npc[i].ai[0] * 16f) + 8f) - vector.X;
                        float num3 = ((npc[i].ai[1] * 16f) + 8f) - vector.Y;
                        float rotation = ((float) Math.Atan2((double) num3, (double) num2)) - 1.57f;
                        bool flag = true;
                        while (flag)
                        {
                            int height = 0x1c;
                            float num6 = (float) Math.Sqrt((double) ((num2 * num2) + (num3 * num3)));
                            if (num6 < 40f)
                            {
                                height = (((int) num6) - 40) + 0x1c;
                                flag = false;
                            }
                            num6 = 28f / num6;
                            num2 *= num6;
                            num3 *= num6;
                            vector.X += num2;
                            vector.Y += num3;
                            num2 = ((npc[i].ai[0] * 16f) + 8f) - vector.X;
                            num3 = ((npc[i].ai[1] * 16f) + 8f) - vector.Y;
                            Color color = Lighting.GetColor(((int) vector.X) / 0x10, (int) (vector.Y / 16f));
                            this.spriteBatch.Draw(chain4Texture, new Vector2(vector.X - screenPosition.X, vector.Y - screenPosition.Y), new Rectangle(0, 0, chain4Texture.Width, height), color, rotation, new Vector2(chain4Texture.Width * 0.5f, chain4Texture.Height * 0.5f), (float) 1f, SpriteEffects.None, 0f);
                        }
                    }
                    if (npc[i].type == 0x24)
                    {
                        Vector2 vector2 = new Vector2((npc[i].position.X + (npc[i].width * 0.5f)) - (5f * npc[i].ai[0]), npc[i].position.Y + 20f);
                        for (int j = 0; j < 2; j++)
                        {
                            float num8 = (npc[(int) npc[i].ai[1]].position.X + (npc[(int) npc[i].ai[1]].width / 2)) - vector2.X;
                            float num9 = (npc[(int) npc[i].ai[1]].position.Y + (npc[(int) npc[i].ai[1]].height / 2)) - vector2.Y;
                            float num10 = 0f;
                            if (j == 0)
                            {
                                num8 -= 200f * npc[i].ai[0];
                                num9 += 130f;
                                num10 = (float) Math.Sqrt((double) ((num8 * num8) + (num9 * num9)));
                                num10 = 92f / num10;
                                vector2.X += num8 * num10;
                                vector2.Y += num9 * num10;
                            }
                            else
                            {
                                num8 -= 50f * npc[i].ai[0];
                                num9 += 80f;
                                num10 = (float) Math.Sqrt((double) ((num8 * num8) + (num9 * num9)));
                                num10 = 60f / num10;
                                vector2.X += num8 * num10;
                                vector2.Y += num9 * num10;
                            }
                            float num11 = ((float) Math.Atan2((double) num9, (double) num8)) - 1.57f;
                            Color color2 = Lighting.GetColor(((int) vector2.X) / 0x10, (int) (vector2.Y / 16f));
                            this.spriteBatch.Draw(boneArmTexture, new Vector2(vector2.X - screenPosition.X, vector2.Y - screenPosition.Y), new Rectangle(0, 0, boneArmTexture.Width, boneArmTexture.Height), color2, num11, new Vector2(boneArmTexture.Width * 0.5f, boneArmTexture.Height * 0.5f), (float) 1f, SpriteEffects.None, 0f);
                            if (j == 0)
                            {
                                vector2.X += (num8 * num10) / 2f;
                                vector2.Y += (num9 * num10) / 2f;
                            }
                            else if (base.IsActive)
                            {
                                vector2.X += (num8 * num10) - 16f;
                                vector2.Y += (num9 * num10) - 6f;
                                Color color4 = new Color();
                                int index = Dust.NewDust(new Vector2(vector2.X, vector2.Y), 30, 10, 5, num8 * 0.02f, num9 * 0.02f, 0, color4, 2f);
                                dust[index].noGravity = true;
                            }
                        }
                    }
                    float num13 = 0f;
                    Vector2 origin = new Vector2((float) (npcTexture[npc[i].type].Width / 2), (float) ((npcTexture[npc[i].type].Height / npcFrameCount[npc[i].type]) / 2));
                    if (npc[i].type == 4)
                    {
                        origin = new Vector2(55f, 107f);
                    }
                    if (npc[i].type == 6)
                    {
                        num13 = 26f;
                    }
                    if (((npc[i].type == 7) || (npc[i].type == 8)) || (npc[i].type == 9))
                    {
                        num13 = 13f;
                    }
                    if (((npc[i].type == 10) || (npc[i].type == 11)) || (npc[i].type == 12))
                    {
                        num13 = 8f;
                    }
                    if (((npc[i].type == 13) || (npc[i].type == 14)) || (npc[i].type == 15))
                    {
                        num13 = 26f;
                    }
                    num13 *= npc[i].scale;
                    Color newColor = Lighting.GetColor(((int) (npc[i].position.X + (npc[i].width * 0.5))) / 0x10, (int) ((npc[i].position.Y + (npc[i].height * 0.5)) / 16.0));
                    if (npc[i].aiStyle == 10)
                    {
                        newColor = Color.White;
                    }
                    SpriteEffects none = SpriteEffects.None;
                    if (npc[i].spriteDirection == 1)
                    {
                        none = SpriteEffects.FlipHorizontally;
                    }
                    this.spriteBatch.Draw(npcTexture[npc[i].type], new Vector2((((npc[i].position.X - screenPosition.X) + (npc[i].width / 2)) - ((npcTexture[npc[i].type].Width * npc[i].scale) / 2f)) + (origin.X * npc[i].scale), (((((npc[i].position.Y - screenPosition.Y) + npc[i].height) - ((npcTexture[npc[i].type].Height * npc[i].scale) / ((float) npcFrameCount[npc[i].type]))) + 4f) + (origin.Y * npc[i].scale)) + num13), new Rectangle?(npc[i].frame), npc[i].GetAlpha(newColor), npc[i].rotation, origin, npc[i].scale, none, 0f);
                    Color color5 = new Color();
                    if (npc[i].color != color5)
                    {
                        this.spriteBatch.Draw(npcTexture[npc[i].type], new Vector2((((npc[i].position.X - screenPosition.X) + (npc[i].width / 2)) - ((npcTexture[npc[i].type].Width * npc[i].scale) / 2f)) + (origin.X * npc[i].scale), (((((npc[i].position.Y - screenPosition.Y) + npc[i].height) - ((npcTexture[npc[i].type].Height * npc[i].scale) / ((float) npcFrameCount[npc[i].type]))) + 4f) + (origin.Y * npc[i].scale)) + num13), new Rectangle?(npc[i].frame), npc[i].GetColor(newColor), npc[i].rotation, origin, npc[i].scale, none, 0f);
                    }
                }
            }
        }

        protected void DrawPlayer(Player drawPlayer)
        {
            SpriteEffects none = SpriteEffects.None;
            SpriteEffects flipHorizontally = SpriteEffects.FlipHorizontally;
            Color immuneAlpha = drawPlayer.GetImmuneAlpha(Lighting.GetColor(((int) (drawPlayer.position.X + (drawPlayer.width * 0.5))) / 0x10, (int) ((drawPlayer.position.Y + (drawPlayer.height * 0.25)) / 16.0), Color.White));
            Color color = drawPlayer.GetImmuneAlpha(Lighting.GetColor(((int) (drawPlayer.position.X + (drawPlayer.width * 0.5))) / 0x10, (int) ((drawPlayer.position.Y + (drawPlayer.height * 0.25)) / 16.0), drawPlayer.eyeColor));
            Color color3 = drawPlayer.GetImmuneAlpha(Lighting.GetColor(((int) (drawPlayer.position.X + (drawPlayer.width * 0.5))) / 0x10, (int) ((drawPlayer.position.Y + (drawPlayer.height * 0.25)) / 16.0), drawPlayer.hairColor));
            Color color4 = drawPlayer.GetImmuneAlpha(Lighting.GetColor(((int) (drawPlayer.position.X + (drawPlayer.width * 0.5))) / 0x10, (int) ((drawPlayer.position.Y + (drawPlayer.height * 0.25)) / 16.0), drawPlayer.skinColor));
            Color color5 = drawPlayer.GetImmuneAlpha(Lighting.GetColor(((int) (drawPlayer.position.X + (drawPlayer.width * 0.5))) / 0x10, (int) ((drawPlayer.position.Y + (drawPlayer.height * 0.5)) / 16.0), drawPlayer.skinColor));
            Color color6 = drawPlayer.GetImmuneAlpha(Lighting.GetColor(((int) (drawPlayer.position.X + (drawPlayer.width * 0.5))) / 0x10, (int) ((drawPlayer.position.Y + (drawPlayer.height * 0.5)) / 16.0), drawPlayer.shirtColor));
            Color color7 = drawPlayer.GetImmuneAlpha(Lighting.GetColor(((int) (drawPlayer.position.X + (drawPlayer.width * 0.5))) / 0x10, (int) ((drawPlayer.position.Y + (drawPlayer.height * 0.5)) / 16.0), drawPlayer.underShirtColor));
            Color color8 = drawPlayer.GetImmuneAlpha(Lighting.GetColor(((int) (drawPlayer.position.X + (drawPlayer.width * 0.5))) / 0x10, (int) ((drawPlayer.position.Y + (drawPlayer.height * 0.75)) / 16.0), drawPlayer.pantsColor));
            Color color9 = drawPlayer.GetImmuneAlpha(Lighting.GetColor(((int) (drawPlayer.position.X + (drawPlayer.width * 0.5))) / 0x10, (int) ((drawPlayer.position.Y + (drawPlayer.height * 0.75)) / 16.0), drawPlayer.shoeColor));
            Color color10 = drawPlayer.GetImmuneAlpha(Lighting.GetColor(((int) (drawPlayer.position.X + (drawPlayer.width * 0.5))) / 0x10, ((int) (drawPlayer.position.Y + (drawPlayer.height * 0.75))) / 0x10, Color.White));
            Color color11 = drawPlayer.GetImmuneAlpha(Lighting.GetColor(((int) (drawPlayer.position.X + (drawPlayer.width * 0.5))) / 0x10, ((int) (drawPlayer.position.Y + (drawPlayer.height * 0.5))) / 0x10, Color.White));
            Color color12 = drawPlayer.GetImmuneAlpha(Lighting.GetColor(((int) (drawPlayer.position.X + (drawPlayer.width * 0.5))) / 0x10, ((int) (drawPlayer.position.Y + (drawPlayer.height * 0.25))) / 0x10, Color.White));
            if (drawPlayer.direction == 1)
            {
                none = SpriteEffects.None;
                flipHorizontally = SpriteEffects.None;
            }
            else
            {
                none = SpriteEffects.FlipHorizontally;
                flipHorizontally = SpriteEffects.FlipHorizontally;
            }
            Vector2 origin = new Vector2(drawPlayer.legFrame.Width * 0.5f, drawPlayer.legFrame.Height * 0.75f);
            Vector2 vector2 = new Vector2(drawPlayer.legFrame.Width * 0.5f, drawPlayer.legFrame.Height * 0.5f);
            Vector2 vector3 = new Vector2(drawPlayer.legFrame.Width * 0.5f, drawPlayer.legFrame.Height * 0.25f);
            if ((drawPlayer.legs > 0) && (drawPlayer.legs < 10))
            {
                this.spriteBatch.Draw(armorLegTexture[drawPlayer.legs], (new Vector2((float) ((((int) (drawPlayer.position.X - screenPosition.X)) - (drawPlayer.legFrame.Width / 2)) + (drawPlayer.width / 2)), (float) ((int) ((((drawPlayer.position.Y - screenPosition.Y) + drawPlayer.height) - drawPlayer.legFrame.Height) + 4f))) + drawPlayer.legPosition) + origin, new Rectangle?(drawPlayer.legFrame), color12, drawPlayer.legRotation, origin, (float) 1f, none, 0f);
            }
            else
            {
                this.spriteBatch.Draw(playerPantsTexture, (new Vector2((float) ((((int) (drawPlayer.position.X - screenPosition.X)) - (drawPlayer.legFrame.Width / 2)) + (drawPlayer.width / 2)), (float) ((int) ((((drawPlayer.position.Y - screenPosition.Y) + drawPlayer.height) - drawPlayer.legFrame.Height) + 4f))) + drawPlayer.legPosition) + origin, new Rectangle?(drawPlayer.legFrame), color8, drawPlayer.legRotation, origin, (float) 1f, none, 0f);
                this.spriteBatch.Draw(playerShoesTexture, (new Vector2((float) ((((int) (drawPlayer.position.X - screenPosition.X)) - (drawPlayer.legFrame.Width / 2)) + (drawPlayer.width / 2)), (float) ((int) ((((drawPlayer.position.Y - screenPosition.Y) + drawPlayer.height) - drawPlayer.legFrame.Height) + 4f))) + drawPlayer.legPosition) + origin, new Rectangle?(drawPlayer.legFrame), color9, drawPlayer.legRotation, origin, (float) 1f, none, 0f);
            }
            if ((drawPlayer.body > 0) && (drawPlayer.body < 10))
            {
                this.spriteBatch.Draw(armorBodyTexture[drawPlayer.body], (new Vector2((float) ((((int) (drawPlayer.position.X - screenPosition.X)) - (drawPlayer.bodyFrame.Width / 2)) + (drawPlayer.width / 2)), (float) ((int) ((((drawPlayer.position.Y - screenPosition.Y) + drawPlayer.height) - drawPlayer.bodyFrame.Height) + 4f))) + drawPlayer.bodyPosition) + new Vector2((float) (drawPlayer.bodyFrame.Width / 2), (float) (drawPlayer.bodyFrame.Height / 2)), new Rectangle?(drawPlayer.bodyFrame), color11, drawPlayer.bodyRotation, vector2, (float) 1f, none, 0f);
            }
            else
            {
                this.spriteBatch.Draw(playerUnderShirtTexture, (new Vector2((float) ((((int) (drawPlayer.position.X - screenPosition.X)) - (drawPlayer.bodyFrame.Width / 2)) + (drawPlayer.width / 2)), (float) ((int) ((((drawPlayer.position.Y - screenPosition.Y) + drawPlayer.height) - drawPlayer.bodyFrame.Height) + 4f))) + drawPlayer.bodyPosition) + new Vector2((float) (drawPlayer.bodyFrame.Width / 2), (float) (drawPlayer.bodyFrame.Height / 2)), new Rectangle?(drawPlayer.bodyFrame), color7, drawPlayer.bodyRotation, vector2, (float) 1f, none, 0f);
                this.spriteBatch.Draw(playerShirtTexture, (new Vector2((float) ((((int) (drawPlayer.position.X - screenPosition.X)) - (drawPlayer.bodyFrame.Width / 2)) + (drawPlayer.width / 2)), (float) ((int) ((((drawPlayer.position.Y - screenPosition.Y) + drawPlayer.height) - drawPlayer.bodyFrame.Height) + 4f))) + drawPlayer.bodyPosition) + new Vector2((float) (drawPlayer.bodyFrame.Width / 2), (float) (drawPlayer.bodyFrame.Height / 2)), new Rectangle?(drawPlayer.bodyFrame), color6, drawPlayer.bodyRotation, vector2, (float) 1f, none, 0f);
                this.spriteBatch.Draw(playerHandsTexture, (new Vector2((float) ((((int) (drawPlayer.position.X - screenPosition.X)) - (drawPlayer.bodyFrame.Width / 2)) + (drawPlayer.width / 2)), (float) ((int) ((((drawPlayer.position.Y - screenPosition.Y) + drawPlayer.height) - drawPlayer.bodyFrame.Height) + 4f))) + drawPlayer.bodyPosition) + new Vector2((float) (drawPlayer.bodyFrame.Width / 2), (float) (drawPlayer.bodyFrame.Height / 2)), new Rectangle?(drawPlayer.bodyFrame), color5, drawPlayer.bodyRotation, vector2, (float) 1f, none, 0f);
            }
            this.spriteBatch.Draw(playerHeadTexture, (new Vector2((float) ((((int) (drawPlayer.position.X - screenPosition.X)) - (drawPlayer.bodyFrame.Width / 2)) + (drawPlayer.width / 2)), (float) ((int) ((((drawPlayer.position.Y - screenPosition.Y) + drawPlayer.height) - drawPlayer.bodyFrame.Height) + 4f))) + drawPlayer.headPosition) + vector3, new Rectangle?(drawPlayer.bodyFrame), color4, drawPlayer.headRotation, vector3, (float) 1f, none, 0f);
            this.spriteBatch.Draw(playerEyeWhitesTexture, (new Vector2((float) ((((int) (drawPlayer.position.X - screenPosition.X)) - (drawPlayer.bodyFrame.Width / 2)) + (drawPlayer.width / 2)), (float) ((int) ((((drawPlayer.position.Y - screenPosition.Y) + drawPlayer.height) - drawPlayer.bodyFrame.Height) + 4f))) + drawPlayer.headPosition) + vector3, new Rectangle?(drawPlayer.bodyFrame), immuneAlpha, drawPlayer.headRotation, vector3, (float) 1f, none, 0f);
            this.spriteBatch.Draw(playerEyesTexture, (new Vector2((float) ((((int) (drawPlayer.position.X - screenPosition.X)) - (drawPlayer.bodyFrame.Width / 2)) + (drawPlayer.width / 2)), (float) ((int) ((((drawPlayer.position.Y - screenPosition.Y) + drawPlayer.height) - drawPlayer.bodyFrame.Height) + 4f))) + drawPlayer.headPosition) + vector3, new Rectangle?(drawPlayer.bodyFrame), color, drawPlayer.headRotation, vector3, (float) 1f, none, 0f);
            if (drawPlayer.head == 10)
            {
                this.spriteBatch.Draw(armorHeadTexture[drawPlayer.head], (new Vector2((float) ((((int) (drawPlayer.position.X - screenPosition.X)) - (drawPlayer.bodyFrame.Width / 2)) + (drawPlayer.width / 2)), (float) ((int) ((((drawPlayer.position.Y - screenPosition.Y) + drawPlayer.height) - drawPlayer.bodyFrame.Height) + 4f))) + drawPlayer.headPosition) + vector3, new Rectangle?(drawPlayer.bodyFrame), color10, drawPlayer.headRotation, vector3, (float) 1f, none, 0f);
                Rectangle bodyFrame = drawPlayer.bodyFrame;
                bodyFrame.Y -= 0x150;
                if (bodyFrame.Y < 0)
                {
                    bodyFrame.Y = 0;
                }
                this.spriteBatch.Draw(playerHairTexture[drawPlayer.hair], (new Vector2((float) ((((int) (drawPlayer.position.X - screenPosition.X)) - (drawPlayer.bodyFrame.Width / 2)) + (drawPlayer.width / 2)), (float) ((int) ((((drawPlayer.position.Y - screenPosition.Y) + drawPlayer.height) - drawPlayer.bodyFrame.Height) + 4f))) + drawPlayer.headPosition) + vector3, new Rectangle?(bodyFrame), color3, drawPlayer.headRotation, vector3, (float) 1f, none, 0f);
            }
            else if ((drawPlayer.head > 0) && (drawPlayer.head < 12))
            {
                this.spriteBatch.Draw(armorHeadTexture[drawPlayer.head], (new Vector2((float) ((((int) (drawPlayer.position.X - screenPosition.X)) - (drawPlayer.bodyFrame.Width / 2)) + (drawPlayer.width / 2)), (float) ((int) ((((drawPlayer.position.Y - screenPosition.Y) + drawPlayer.height) - drawPlayer.bodyFrame.Height) + 4f))) + drawPlayer.headPosition) + vector3, new Rectangle?(drawPlayer.bodyFrame), color10, drawPlayer.headRotation, vector3, (float) 1f, none, 0f);
            }
            else
            {
                Rectangle rectangle2 = drawPlayer.bodyFrame;
                rectangle2.Y -= 0x150;
                if (rectangle2.Y < 0)
                {
                    rectangle2.Y = 0;
                }
                this.spriteBatch.Draw(playerHairTexture[drawPlayer.hair], (new Vector2((float) ((((int) (drawPlayer.position.X - screenPosition.X)) - (drawPlayer.bodyFrame.Width / 2)) + (drawPlayer.width / 2)), (float) ((int) ((((drawPlayer.position.Y - screenPosition.Y) + drawPlayer.height) - drawPlayer.bodyFrame.Height) + 4f))) + drawPlayer.headPosition) + vector3, new Rectangle?(rectangle2), color3, drawPlayer.headRotation, vector3, (float) 1f, none, 0f);
            }
            Color newColor = Lighting.GetColor(((int) (drawPlayer.position.X + (drawPlayer.width * 0.5))) / 0x10, (int) ((drawPlayer.position.Y + (drawPlayer.height * 0.5)) / 16.0));
            if (((drawPlayer.itemAnimation > 0) || (drawPlayer.inventory[drawPlayer.selectedItem].holdStyle > 0)) && (((drawPlayer.inventory[drawPlayer.selectedItem].type > 0) && !drawPlayer.dead) && !drawPlayer.inventory[drawPlayer.selectedItem].noUseGraphic))
            {
                if (drawPlayer.inventory[drawPlayer.selectedItem].useStyle == 5)
                {
                    int num = 10;
                    Vector2 vector4 = new Vector2((float) (itemTexture[drawPlayer.inventory[drawPlayer.selectedItem].type].Width / 2), (float) (itemTexture[drawPlayer.inventory[drawPlayer.selectedItem].type].Height / 2));
                    if (drawPlayer.inventory[drawPlayer.selectedItem].type == 0x5f)
                    {
                        num = 10;
                        vector4.Y += 2f;
                    }
                    else if (drawPlayer.inventory[drawPlayer.selectedItem].type == 0x60)
                    {
                        num = -5;
                    }
                    else if (drawPlayer.inventory[drawPlayer.selectedItem].type == 0x62)
                    {
                        num = -5;
                        vector4.Y -= 2f;
                    }
                    else if (drawPlayer.inventory[drawPlayer.selectedItem].type == 0xc5)
                    {
                        num = -5;
                        vector4.Y += 4f;
                    }
                    else if (drawPlayer.inventory[drawPlayer.selectedItem].type == 0x7e)
                    {
                        num = 4;
                        vector4.Y += 4f;
                    }
                    else if (drawPlayer.inventory[drawPlayer.selectedItem].type == 0x7f)
                    {
                        num = 4;
                        vector4.Y += 2f;
                    }
                    else if (drawPlayer.inventory[drawPlayer.selectedItem].type == 0x9d)
                    {
                        num = 6;
                        vector4.Y += 2f;
                    }
                    else if (drawPlayer.inventory[drawPlayer.selectedItem].type == 160)
                    {
                        num = -8;
                    }
                    else if ((drawPlayer.inventory[drawPlayer.selectedItem].type == 0xa4) || (drawPlayer.inventory[drawPlayer.selectedItem].type == 0xdb))
                    {
                        num = 2;
                        vector4.Y += 4f;
                    }
                    else if (drawPlayer.inventory[drawPlayer.selectedItem].type == 0xa5)
                    {
                        num = 12;
                        vector4.Y += 6f;
                    }
                    Vector2 vector5 = new Vector2((float) -num, (float) (itemTexture[drawPlayer.inventory[drawPlayer.selectedItem].type].Height / 2));
                    if (drawPlayer.direction == -1)
                    {
                        vector5 = new Vector2((float) (itemTexture[drawPlayer.inventory[drawPlayer.selectedItem].type].Width + num), (float) (itemTexture[drawPlayer.inventory[drawPlayer.selectedItem].type].Height / 2));
                    }
                    this.spriteBatch.Draw(itemTexture[drawPlayer.inventory[drawPlayer.selectedItem].type], new Vector2((float) ((int) ((drawPlayer.itemLocation.X - screenPosition.X) + vector4.X)), (float) ((int) ((drawPlayer.itemLocation.Y - screenPosition.Y) + vector4.Y))), new Rectangle(0, 0, itemTexture[drawPlayer.inventory[drawPlayer.selectedItem].type].Width, itemTexture[drawPlayer.inventory[drawPlayer.selectedItem].type].Height), drawPlayer.inventory[drawPlayer.selectedItem].GetAlpha(newColor), drawPlayer.itemRotation, vector5, drawPlayer.inventory[drawPlayer.selectedItem].scale, flipHorizontally, 0f);
                    if (drawPlayer.inventory[drawPlayer.selectedItem].color != new Color())
                    {
                        this.spriteBatch.Draw(itemTexture[drawPlayer.inventory[drawPlayer.selectedItem].type], new Vector2((float) ((int) ((drawPlayer.itemLocation.X - screenPosition.X) + vector4.X)), (float) ((int) ((drawPlayer.itemLocation.Y - screenPosition.Y) + vector4.Y))), new Rectangle(0, 0, itemTexture[drawPlayer.inventory[drawPlayer.selectedItem].type].Width, itemTexture[drawPlayer.inventory[drawPlayer.selectedItem].type].Height), drawPlayer.inventory[drawPlayer.selectedItem].GetColor(newColor), drawPlayer.itemRotation, vector5, drawPlayer.inventory[drawPlayer.selectedItem].scale, flipHorizontally, 0f);
                    }
                }
                else
                {
                    this.spriteBatch.Draw(itemTexture[drawPlayer.inventory[drawPlayer.selectedItem].type], new Vector2((float) ((int) (drawPlayer.itemLocation.X - screenPosition.X)), (float) ((int) (drawPlayer.itemLocation.Y - screenPosition.Y))), new Rectangle(0, 0, itemTexture[drawPlayer.inventory[drawPlayer.selectedItem].type].Width, itemTexture[drawPlayer.inventory[drawPlayer.selectedItem].type].Height), drawPlayer.inventory[drawPlayer.selectedItem].GetAlpha(newColor), drawPlayer.itemRotation, new Vector2((itemTexture[drawPlayer.inventory[drawPlayer.selectedItem].type].Width * 0.5f) - ((itemTexture[drawPlayer.inventory[drawPlayer.selectedItem].type].Width * 0.5f) * drawPlayer.direction), (float) itemTexture[drawPlayer.inventory[drawPlayer.selectedItem].type].Height), drawPlayer.inventory[drawPlayer.selectedItem].scale, flipHorizontally, 0f);
                    if (drawPlayer.inventory[drawPlayer.selectedItem].color != new Color())
                    {
                        this.spriteBatch.Draw(itemTexture[drawPlayer.inventory[drawPlayer.selectedItem].type], new Vector2((float) ((int) (drawPlayer.itemLocation.X - screenPosition.X)), (float) ((int) (drawPlayer.itemLocation.Y - screenPosition.Y))), new Rectangle(0, 0, itemTexture[drawPlayer.inventory[drawPlayer.selectedItem].type].Width, itemTexture[drawPlayer.inventory[drawPlayer.selectedItem].type].Height), drawPlayer.inventory[drawPlayer.selectedItem].GetColor(newColor), drawPlayer.itemRotation, new Vector2((itemTexture[drawPlayer.inventory[drawPlayer.selectedItem].type].Width * 0.5f) - ((itemTexture[drawPlayer.inventory[drawPlayer.selectedItem].type].Width * 0.5f) * drawPlayer.direction), (float) itemTexture[drawPlayer.inventory[drawPlayer.selectedItem].type].Height), drawPlayer.inventory[drawPlayer.selectedItem].scale, flipHorizontally, 0f);
                    }
                }
            }
            if ((drawPlayer.body > 0) && (drawPlayer.body < 10))
            {
                this.spriteBatch.Draw(armorArmTexture[drawPlayer.body], (new Vector2((float) ((((int) (drawPlayer.position.X - screenPosition.X)) - (drawPlayer.bodyFrame.Width / 2)) + (drawPlayer.width / 2)), (float) ((int) ((((drawPlayer.position.Y - screenPosition.Y) + drawPlayer.height) - drawPlayer.bodyFrame.Height) + 4f))) + drawPlayer.bodyPosition) + new Vector2((float) (drawPlayer.bodyFrame.Width / 2), (float) (drawPlayer.bodyFrame.Height / 2)), new Rectangle?(drawPlayer.bodyFrame), color11, drawPlayer.bodyRotation, vector2, (float) 1f, none, 0f);
            }
            else
            {
                this.spriteBatch.Draw(playerUnderShirt2Texture, (new Vector2((float) ((((int) (drawPlayer.position.X - screenPosition.X)) - (drawPlayer.bodyFrame.Width / 2)) + (drawPlayer.width / 2)), (float) ((int) ((((drawPlayer.position.Y - screenPosition.Y) + drawPlayer.height) - drawPlayer.bodyFrame.Height) + 4f))) + drawPlayer.bodyPosition) + new Vector2((float) (drawPlayer.bodyFrame.Width / 2), (float) (drawPlayer.bodyFrame.Height / 2)), new Rectangle?(drawPlayer.bodyFrame), color7, drawPlayer.bodyRotation, vector2, (float) 1f, none, 0f);
                this.spriteBatch.Draw(playerHands2Texture, (new Vector2((float) ((((int) (drawPlayer.position.X - screenPosition.X)) - (drawPlayer.bodyFrame.Width / 2)) + (drawPlayer.width / 2)), (float) ((int) ((((drawPlayer.position.Y - screenPosition.Y) + drawPlayer.height) - drawPlayer.bodyFrame.Height) + 4f))) + drawPlayer.bodyPosition) + new Vector2((float) (drawPlayer.bodyFrame.Width / 2), (float) (drawPlayer.bodyFrame.Height / 2)), new Rectangle?(drawPlayer.bodyFrame), color5, drawPlayer.bodyRotation, vector2, (float) 1f, none, 0f);
            }
        }

        protected void DrawSplash(GameTime gameTime)
        {
            base.GraphicsDevice.Clear(Color.Black);
            base.Draw(gameTime);
            this.spriteBatch.Begin();
            this.splashCounter++;
            Color white = Color.White;
            byte num = 0;
            if (this.splashCounter <= 0x4b)
            {
                float num2 = (((float) this.splashCounter) / 75f) * 255f;
                num = (byte) num2;
            }
            else if (this.splashCounter <= 200)
            {
                num = 0xff;
            }
            else if (this.splashCounter <= 0x113)
            {
                int num3 = 0x113 - this.splashCounter;
                float num4 = (((float) num3) / 75f) * 255f;
                num = (byte) num4;
            }
            else
            {
                showSplash = false;
                fadeCounter = 0x4b;
            }
            white = new Color((int) num, (int) num, (int) num, (int) num);
            this.spriteBatch.Draw(splashTexture, new Vector2(0f, 0f), new Rectangle(0, 0, splashTexture.Width, splashTexture.Height), white, 0f, new Vector2(), (float) 1f, SpriteEffects.None, 0f);
            this.spriteBatch.End();
        }

        protected void DrawTiles(bool solidOnly = true)
        {
            int num = (int) ((screenPosition.X / 16f) - 1f);
            int maxTilesX = ((int) ((screenPosition.X + screenWidth) / 16f)) + 2;
            int num3 = (int) ((screenPosition.Y / 16f) - 1f);
            int maxTilesY = ((int) ((screenPosition.Y + screenHeight) / 16f)) + 2;
            if (num < 0)
            {
                num = 0;
            }
            if (maxTilesX > Main.maxTilesX)
            {
                maxTilesX = Main.maxTilesX;
            }
            if (num3 < 0)
            {
                num3 = 0;
            }
            if (maxTilesY > Main.maxTilesY)
            {
                maxTilesY = Main.maxTilesY;
            }
            int height = 0x10;
            int width = 0x10;
            for (int i = num3; i < (maxTilesY + 4); i++)
            {
                for (int j = num - 2; j < (maxTilesX + 2); j++)
                {
                    if (tile[j, i].active && (tileSolid[tile[j, i].type] == solidOnly))
                    {
                        int num9 = 0;
                        if (tile[j, i].type == 0x4e)
                        {
                            num9 = 2;
                        }
                        if ((tile[j, i].type == 0x21) || (tile[j, i].type == 0x31))
                        {
                            num9 = -4;
                        }
                        if ((((tile[j, i].type == 3) || (tile[j, i].type == 4)) || ((tile[j, i].type == 5) || (tile[j, i].type == 0x18))) || (((tile[j, i].type == 0x21) || (tile[j, i].type == 0x31)) || ((tile[j, i].type == 0x3d) || (tile[j, i].type == 0x47))))
                        {
                            height = 20;
                        }
                        else if (((((tile[j, i].type == 15) || (tile[j, i].type == 14)) || ((tile[j, i].type == 0x10) || (tile[j, i].type == 0x11))) || (((tile[j, i].type == 0x12) || (tile[j, i].type == 20)) || ((tile[j, i].type == 0x15) || (tile[j, i].type == 0x1a)))) || (((tile[j, i].type == 0x1b) || (tile[j, i].type == 0x20)) || (((tile[j, i].type == 0x45) || (tile[j, i].type == 0x48)) || (tile[j, i].type == 0x4d))))
                        {
                            height = 0x12;
                        }
                        else
                        {
                            height = 0x10;
                        }
                        if ((tile[j, i].type == 4) || (tile[j, i].type == 5))
                        {
                            width = 20;
                        }
                        else
                        {
                            width = 0x10;
                        }
                        if ((tile[j, i].type == 0x49) || (tile[j, i].type == 0x4a))
                        {
                            num9 -= 12;
                            height = 0x20;
                        }
                        if (((tile[j, i].type == 4) && (rand.Next(40) == 0)) && base.IsActive)
                        {
                            if (tile[j, i].frameX == 0x16)
                            {
                                Color newColor = new Color();
                                Dust.NewDust(new Vector2((float) ((j * 0x10) + 6), (float) (i * 0x10)), 4, 4, 6, 0f, 0f, 100, newColor, 1f);
                            }
                            if (tile[j, i].frameX == 0x2c)
                            {
                                Color color4 = new Color();
                                Dust.NewDust(new Vector2((float) ((j * 0x10) + 2), (float) (i * 0x10)), 4, 4, 6, 0f, 0f, 100, color4, 1f);
                            }
                            else
                            {
                                Color color5 = new Color();
                                Dust.NewDust(new Vector2((float) ((j * 0x10) + 4), (float) (i * 0x10)), 4, 4, 6, 0f, 0f, 100, color5, 1f);
                            }
                        }
                        if (((tile[j, i].type == 0x21) && (rand.Next(40) == 0)) && base.IsActive)
                        {
                            Color color6 = new Color();
                            Dust.NewDust(new Vector2((float) ((j * 0x10) + 4), (float) ((i * 0x10) - 4)), 4, 4, 6, 0f, 0f, 100, color6, 1f);
                        }
                        if (((tile[j, i].type == 0x31) && (rand.Next(20) == 0)) && base.IsActive)
                        {
                            Color color7 = new Color();
                            Dust.NewDust(new Vector2((float) ((j * 0x10) + 4), (float) ((i * 0x10) - 4)), 4, 4, 0x1d, 0f, 0f, 100, color7, 1f);
                        }
                        if ((((tile[j, i].type == 0x22) || (tile[j, i].type == 0x23)) || (tile[j, i].type == 0x24)) && ((((rand.Next(40) == 0) && base.IsActive) && (tile[j, i].frameY == 0x12)) && ((tile[j, i].frameX == 0) || (tile[j, i].frameX == 0x24))))
                        {
                            Color color8 = new Color();
                            Dust.NewDust(new Vector2((float) (j * 0x10), (float) ((i * 0x10) + 2)), 14, 6, 6, 0f, 0f, 100, color8, 1f);
                        }
                        if (((tile[j, i].type == 0x16) && (rand.Next(400) == 0)) && base.IsActive)
                        {
                            Color color9 = new Color();
                            Dust.NewDust(new Vector2((float) (j * 0x10), (float) (i * 0x10)), 0x10, 0x10, 14, 0f, 0f, 0, color9, 1f);
                        }
                        else if ((((tile[j, i].type == 0x17) || (tile[j, i].type == 0x18)) || (tile[j, i].type == 0x20)) && ((rand.Next(500) == 0) && base.IsActive))
                        {
                            Color color10 = new Color();
                            Dust.NewDust(new Vector2((float) (j * 0x10), (float) (i * 0x10)), 0x10, 0x10, 14, 0f, 0f, 0, color10, 1f);
                        }
                        else if (((tile[j, i].type == 0x19) && (rand.Next(700) == 0)) && base.IsActive)
                        {
                            Color color11 = new Color();
                            Dust.NewDust(new Vector2((float) (j * 0x10), (float) (i * 0x10)), 0x10, 0x10, 14, 0f, 0f, 0, color11, 1f);
                        }
                        else if (((tile[j, i].type == 0x1f) && (rand.Next(20) == 0)) && base.IsActive)
                        {
                            Color color12 = new Color();
                            Dust.NewDust(new Vector2((float) (j * 0x10), (float) (i * 0x10)), 0x10, 0x10, 14, 0f, 0f, 100, color12, 1f);
                        }
                        else if (((tile[j, i].type == 0x1a) && (rand.Next(20) == 0)) && base.IsActive)
                        {
                            Color color13 = new Color();
                            Dust.NewDust(new Vector2((float) (j * 0x10), (float) (i * 0x10)), 0x10, 0x10, 14, 0f, 0f, 100, color13, 1f);
                        }
                        else if (((tile[j, i].type == 0x47) || (tile[j, i].type == 0x48)) && ((rand.Next(500) == 0) && base.IsActive))
                        {
                            Color color14 = new Color();
                            Dust.NewDust(new Vector2((float) (j * 0x10), (float) (i * 0x10)), 0x10, 0x10, 0x29, 0f, 0f, 250, color14, 0.8f);
                        }
                        else if (((tile[j, i].type == 0x11) || (tile[j, i].type == 0x4d)) && ((rand.Next(40) == 0) && base.IsActive))
                        {
                            if ((tile[j, i].frameX == 0x12) & (tile[j, i].frameY == 0x12))
                            {
                                Color color15 = new Color();
                                Dust.NewDust(new Vector2((float) ((j * 0x10) + 2), (float) (i * 0x10)), 8, 6, 6, 0f, 0f, 100, color15, 1f);
                            }
                        }
                        else if ((((tile[j, i].type == 0x25) || (tile[j, i].type == 0x3a)) || (tile[j, i].type == 0x4c)) && ((rand.Next(200) == 0) && base.IsActive))
                        {
                            Color color16 = new Color();
                            int index = Dust.NewDust(new Vector2((float) (j * 0x10), (float) (i * 0x10)), 0x10, 0x10, 6, 0f, 0f, 0, color16, (float) rand.Next(3));
                            if (dust[index].scale > 1f)
                            {
                                dust[index].noGravity = true;
                            }
                        }
                        if (((tile[j, i].type == 5) && (tile[j, i].frameY >= 0xc6)) && (tile[j, i].frameX >= 0x16))
                        {
                            int num11 = 0;
                            if (tile[j, i].frameX == 0x16)
                            {
                                if (tile[j, i].frameY == 220)
                                {
                                    num11 = 1;
                                }
                                else if (tile[j, i].frameY == 0xf2)
                                {
                                    num11 = 2;
                                }
                                Vector2 origin = new Vector2();
                                this.spriteBatch.Draw(treeTopTexture, new Vector2((float) (((j * 0x10) - ((int) screenPosition.X)) - 0x20), (float) (((i * 0x10) - ((int) screenPosition.Y)) - 0x40)), new Rectangle(num11 * 0x52, 0, 80, 80), Lighting.GetColor(j, i), 0f, origin, (float) 1f, SpriteEffects.None, 0f);
                            }
                            else if (tile[j, i].frameX == 0x2c)
                            {
                                if (tile[j, i].frameY == 220)
                                {
                                    num11 = 1;
                                }
                                else if (tile[j, i].frameY == 0xf2)
                                {
                                    num11 = 2;
                                }
                                Vector2 vector3 = new Vector2();
                                this.spriteBatch.Draw(treeBranchTexture, new Vector2((float) (((j * 0x10) - ((int) screenPosition.X)) - 0x18), (float) (((i * 0x10) - ((int) screenPosition.Y)) - 12)), new Rectangle(0, num11 * 0x2a, 40, 40), Lighting.GetColor(j, i), 0f, vector3, (float) 1f, SpriteEffects.None, 0f);
                            }
                            else if (tile[j, i].frameX == 0x42)
                            {
                                if (tile[j, i].frameY == 220)
                                {
                                    num11 = 1;
                                }
                                else if (tile[j, i].frameY == 0xf2)
                                {
                                    num11 = 2;
                                }
                                Vector2 vector4 = new Vector2();
                                this.spriteBatch.Draw(treeBranchTexture, new Vector2((float) ((j * 0x10) - ((int) screenPosition.X)), (float) (((i * 0x10) - ((int) screenPosition.Y)) - 12)), new Rectangle(0x2a, num11 * 0x2a, 40, 40), Lighting.GetColor(j, i), 0f, vector4, (float) 1f, SpriteEffects.None, 0f);
                            }
                        }
                        if ((tile[j, i].type == 0x48) && (tile[j, i].frameX >= 0x24))
                        {
                            int num12 = 0;
                            if (tile[j, i].frameY == 0x12)
                            {
                                num12 = 1;
                            }
                            else if (tile[j, i].frameY == 0x24)
                            {
                                num12 = 2;
                            }
                            Vector2 vector5 = new Vector2();
                            this.spriteBatch.Draw(shroomCapTexture, new Vector2((float) (((j * 0x10) - ((int) screenPosition.X)) - 0x16), (float) (((i * 0x10) - ((int) screenPosition.Y)) - 0x1a)), new Rectangle(num12 * 0x3e, 0, 60, 0x2a), Lighting.GetColor(j, i), 0f, vector5, (float) 1f, SpriteEffects.None, 0f);
                        }
                        if (Lighting.Brightness(j, i) > 0f)
                        {
                            if (solidOnly && (((tile[j - 1, i].liquid > 0) || (tile[j + 1, i].liquid > 0)) || ((tile[j, i - 1].liquid > 0) || (tile[j, i + 1].liquid > 0))))
                            {
                                Color color = Lighting.GetColor(j, i);
                                int liquid = 0;
                                bool flag = false;
                                bool flag2 = false;
                                bool flag3 = false;
                                bool flag4 = false;
                                int num14 = 0;
                                bool flag5 = false;
                                if (tile[j - 1, i].liquid > liquid)
                                {
                                    liquid = tile[j - 1, i].liquid;
                                    flag = true;
                                }
                                else if (tile[j - 1, i].liquid > 0)
                                {
                                    flag = true;
                                }
                                if (tile[j + 1, i].liquid > liquid)
                                {
                                    liquid = tile[j + 1, i].liquid;
                                    flag2 = true;
                                }
                                else if (tile[j + 1, i].liquid > 0)
                                {
                                    liquid = tile[j + 1, i].liquid;
                                    flag2 = true;
                                }
                                if (tile[j, i - 1].liquid > 0)
                                {
                                    flag3 = true;
                                }
                                if (tile[j, i + 1].liquid > 240)
                                {
                                    flag4 = true;
                                }
                                if (tile[j - 1, i].liquid > 0)
                                {
                                    if (tile[j - 1, i].lava)
                                    {
                                        num14 = 1;
                                    }
                                    else
                                    {
                                        flag5 = true;
                                    }
                                }
                                if (tile[j + 1, i].liquid > 0)
                                {
                                    if (tile[j + 1, i].lava)
                                    {
                                        num14 = 1;
                                    }
                                    else
                                    {
                                        flag5 = true;
                                    }
                                }
                                if (tile[j, i - 1].liquid > 0)
                                {
                                    if (tile[j, i - 1].lava)
                                    {
                                        num14 = 1;
                                    }
                                    else
                                    {
                                        flag5 = true;
                                    }
                                }
                                if (tile[j, i + 1].liquid > 0)
                                {
                                    if (tile[j, i + 1].lava)
                                    {
                                        num14 = 1;
                                    }
                                    else
                                    {
                                        flag5 = true;
                                    }
                                }
                                if (!flag5 || (num14 != 1))
                                {
                                    float num15 = 0f;
                                    Vector2 vector = new Vector2((float) (j * 0x10), (float) (i * 0x10));
                                    Rectangle rectangle = new Rectangle(0, 4, 0x10, 0x10);
                                    if (flag4 && (flag || flag2))
                                    {
                                        flag = true;
                                        flag2 = true;
                                    }
                                    if ((!flag3 || (!flag && !flag2)) && (!flag4 || !flag3))
                                    {
                                        if (flag3)
                                        {
                                            rectangle = new Rectangle(0, 4, 0x10, 4);
                                        }
                                        else if ((flag4 && !flag) && !flag2)
                                        {
                                            vector = new Vector2((float) (j * 0x10), (float) ((i * 0x10) + 12));
                                            rectangle = new Rectangle(0, 4, 0x10, 4);
                                        }
                                        else
                                        {
                                            num15 = 0x100 - liquid;
                                            num15 /= 32f;
                                            if (flag && flag2)
                                            {
                                                vector = new Vector2((float) (j * 0x10), (float) ((i * 0x10) + (((int) num15) * 2)));
                                                rectangle = new Rectangle(0, 4, 0x10, 0x10 - (((int) num15) * 2));
                                            }
                                            else if (flag)
                                            {
                                                vector = new Vector2((float) (j * 0x10), (float) ((i * 0x10) + (((int) num15) * 2)));
                                                rectangle = new Rectangle(0, 4, 4, 0x10 - (((int) num15) * 2));
                                            }
                                            else
                                            {
                                                vector = new Vector2((float) ((j * 0x10) + 12), (float) ((i * 0x10) + (((int) num15) * 2)));
                                                rectangle = new Rectangle(0, 4, 4, 0x10 - (((int) num15) * 2));
                                            }
                                        }
                                    }
                                    float num16 = 0.5f;
                                    if (num14 == 1)
                                    {
                                        num16 *= 1.6f;
                                    }
                                    if ((i < worldSurface) || (num16 > 1f))
                                    {
                                        num16 = 1f;
                                    }
                                    float num17 = color.R * num16;
                                    float num18 = color.G * num16;
                                    float num19 = color.B * num16;
                                    float num20 = color.A * num16;
                                    color = new Color((int) ((byte) num17), (int) ((byte) num18), (int) ((byte) num19), (int) ((byte) num20));
                                    Vector2 vector6 = new Vector2();
                                    this.spriteBatch.Draw(liquidTexture[num14], vector - screenPosition, new Rectangle?(rectangle), color, 0f, vector6, (float) 1f, SpriteEffects.None, 0f);
                                }
                            }
                            if (tile[j, i].type == 0x33)
                            {
                                Color color2 = Lighting.GetColor(j, i);
                                float num21 = 0.5f;
                                float num22 = color2.R * num21;
                                float num23 = color2.G * num21;
                                float num24 = color2.B * num21;
                                float num25 = color2.A * num21;
                                color2 = new Color((int) ((byte) num22), (int) ((byte) num23), (int) ((byte) num24), (int) ((byte) num25));
                                Vector2 vector7 = new Vector2();
                                this.spriteBatch.Draw(tileTexture[tile[j, i].type], new Vector2(((j * 0x10) - ((int) screenPosition.X)) - ((width - 16f) / 2f), (float) (((i * 0x10) - ((int) screenPosition.Y)) + num9)), new Rectangle(tile[j, i].frameX, tile[j, i].frameY, width, height), color2, 0f, vector7, (float) 1f, SpriteEffects.None, 0f);
                            }
                            else
                            {
                                Vector2 vector8 = new Vector2();
                                this.spriteBatch.Draw(tileTexture[tile[j, i].type], new Vector2(((j * 0x10) - ((int) screenPosition.X)) - ((width - 16f) / 2f), (float) (((i * 0x10) - ((int) screenPosition.Y)) + num9)), new Rectangle(tile[j, i].frameX, tile[j, i].frameY, width, height), Lighting.GetColor(j, i), 0f, vector8, (float) 1f, SpriteEffects.None, 0f);
                            }
                        }
                    }
                }
            }
        }

        protected void DrawWater(bool bg = false)
        {
            int num = (int) ((screenPosition.X / 16f) - 1f);
            int maxTilesX = ((int) ((screenPosition.X + screenWidth) / 16f)) + 2;
            int num3 = (int) ((screenPosition.Y / 16f) - 1f);
            int maxTilesY = ((int) ((screenPosition.Y + screenHeight) / 16f)) + 2;
            if (num < 0)
            {
                num = 0;
            }
            if (maxTilesX > Main.maxTilesX)
            {
                maxTilesX = Main.maxTilesX;
            }
            if (num3 < 0)
            {
                num3 = 0;
            }
            if (maxTilesY > Main.maxTilesY)
            {
                maxTilesY = Main.maxTilesY;
            }
            for (int i = num3; i < (maxTilesY + 4); i++)
            {
                for (int j = num - 2; j < (maxTilesX + 2); j++)
                {
                    if ((tile[j, i].liquid <= 0) || (Lighting.Brightness(j, i) <= 0f))
                    {
                        continue;
                    }
                    Color color = Lighting.GetColor(j, i);
                    float num7 = 0x100 - tile[j, i].liquid;
                    num7 /= 32f;
                    int index = 0;
                    if (tile[j, i].lava)
                    {
                        index = 1;
                    }
                    float num9 = 0.5f;
                    if (bg)
                    {
                        num9 = 1f;
                    }
                    Vector2 vector = new Vector2((float) (j * 0x10), (float) ((i * 0x10) + (((int) num7) * 2)));
                    Rectangle rectangle = new Rectangle(0, 0, 0x10, 0x10 - (((int) num7) * 2));
                    if ((tile[j, i + 1].liquid < 0xf5) && ((!tile[j, i + 1].active || !tileSolid[tile[j, i + 1].type]) || tileSolidTop[tile[j, i + 1].type]))
                    {
                        float num10 = 0x100 - tile[j, i + 1].liquid;
                        num10 /= 32f;
                        num9 = (0.5f * (8f - num7)) / 4f;
                        if (num9 > 0.55)
                        {
                            num9 = 0.55f;
                        }
                        if (num9 < 0.35)
                        {
                            num9 = 0.35f;
                        }
                        float num11 = num7 / 2f;
                        if (tile[j, i + 1].liquid < 200)
                        {
                            if (bg)
                            {
                                continue;
                            }
                            if ((tile[j, i - 1].liquid > 0) && (tile[j, i - 1].liquid > 0))
                            {
                                rectangle = new Rectangle(0, 4, 0x10, 0x10);
                                num9 = 0.5f;
                            }
                            else if (tile[j, i - 1].liquid > 0)
                            {
                                vector = new Vector2((float) (j * 0x10), (float) ((i * 0x10) + 4));
                                rectangle = new Rectangle(0, 4, 0x10, 12);
                                num9 = 0.5f;
                            }
                            else if (tile[j, i + 1].liquid > 0)
                            {
                                vector = new Vector2((float) (j * 0x10), (float) (((i * 0x10) + (((int) num7) * 2)) + (((int) num10) * 2)));
                                rectangle = new Rectangle(0, 4, 0x10, 0x10 - (((int) num7) * 2));
                            }
                            else
                            {
                                vector = new Vector2((float) ((j * 0x10) + ((int) num11)), (float) (((i * 0x10) + (((int) num11) * 2)) + (((int) num10) * 2)));
                                rectangle = new Rectangle(0, 4, 0x10 - (((int) num11) * 2), 0x10 - (((int) num11) * 2));
                            }
                        }
                        else
                        {
                            num9 = 0.5f;
                            rectangle = new Rectangle(0, 4, 0x10, (0x10 - (((int) num7) * 2)) + (((int) num10) * 2));
                        }
                    }
                    else if (tile[j, i - 1].liquid > 0x20)
                    {
                        rectangle = new Rectangle(0, 4, rectangle.Width, rectangle.Height);
                    }
                    else if (((num7 < 1f) && tile[j, i - 1].active) && (tileSolid[tile[j, i - 1].type] && !tileSolidTop[tile[j, i - 1].type]))
                    {
                        vector = new Vector2((float) (j * 0x10), (float) (i * 0x10));
                        rectangle = new Rectangle(0, 4, 0x10, 0x10);
                    }
                    else
                    {
                        bool flag = true;
                        for (int k = i + 1; k < (i + 6); k++)
                        {
                            if ((tile[j, k].active && tileSolid[tile[j, k].type]) && !tileSolidTop[tile[j, k].type])
                            {
                                break;
                            }
                            if (tile[j, k].liquid < 200)
                            {
                                flag = false;
                                break;
                            }
                        }
                        if (!flag)
                        {
                            num9 = 0.5f;
                            rectangle = new Rectangle(0, 4, 0x10, 0x10);
                        }
                        else if (tile[j, i - 1].liquid > 0)
                        {
                            rectangle = new Rectangle(0, 2, rectangle.Width, rectangle.Height);
                        }
                    }
                    if (tile[j, i].lava)
                    {
                        num9 *= 1.6f;
                        if (num9 > 1f)
                        {
                            num9 = 1f;
                        }
                        if (base.IsActive)
                        {
                            if ((tile[j, i].liquid > 200) && (rand.Next(700) == 0))
                            {
                                Color newColor = new Color();
                                Dust.NewDust(new Vector2((float) (j * 0x10), (float) (i * 0x10)), 0x10, 0x10, 0x23, 0f, 0f, 0, newColor, 1f);
                            }
                            if (((rectangle.Y == 0) && base.IsActive) && (rand.Next(300) == 0))
                            {
                                Color color3 = new Color();
                                int num13 = Dust.NewDust(new Vector2((float) (j * 0x10), ((i * 0x10) + (num7 * 2f)) - 8f), 0x10, 8, 0x23, 0f, 0f, 50, color3, 1.5f);
                                Dust dust1 = dust[num13];
                                dust1.velocity = (Vector2) (dust1.velocity * 0.8f);
                                dust[num13].velocity.X *= 2f;
                                dust[num13].velocity.Y -= rand.Next(1, 7) * 0.1f;
                                if (rand.Next(10) == 0)
                                {
                                    dust[num13].velocity.Y *= rand.Next(2, 5);
                                }
                                dust[num13].noGravity = true;
                            }
                        }
                    }
                    float num14 = color.R * num9;
                    float num15 = color.G * num9;
                    float num16 = color.B * num9;
                    float num17 = color.A * num9;
                    color = new Color((int) ((byte) num14), (int) ((byte) num15), (int) ((byte) num16), (int) ((byte) num17));
                    Vector2 origin = new Vector2();
                    this.spriteBatch.Draw(liquidTexture[index], vector - screenPosition, new Rectangle?(rectangle), color, 0f, origin, (float) 1f, SpriteEffects.None, 0f);
                }
            }
        }

        private static void ErasePlayer(int i)
        {
            try
            {
                System.IO.File.Delete(loadPlayerPath[i]);
                System.IO.File.Delete(loadPlayerPath[i] + ".bak");
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
                System.IO.File.Delete(loadWorldPath[i]);
                System.IO.File.Delete(loadWorldPath[i] + ".bak");
                LoadWorlds();
            }
            catch
            {
            }
        }

        protected void getAuth()
        {
            try
            {
                string requestUriString = "";
                StringBuilder builder = new StringBuilder();
                byte[] buffer = new byte[0x2000];
                HttpWebRequest request = (HttpWebRequest) WebRequest.Create(requestUriString);
                Stream responseStream = ((HttpWebResponse) request.GetResponse()).GetResponseStream();
                string str2 = null;
                int count = 0;
                do
                {
                    count = responseStream.Read(buffer, 0, buffer.Length);
                    if (count != 0)
                    {
                        str2 = Encoding.ASCII.GetString(buffer, 0, count);
                        builder.Append(str2);
                    }
                }
                while (count > 0);
                if (builder.ToString() == "")
                {
                    webAuth = true;
                }
            }
            catch
            {
                this.QuitGame();
            }
        }

        public static string GetInputText(string oldString)
        {
            if (!hasFocus)
            {
                return oldString;
            }
            inputTextEnter = false;
            string str = oldString;
            if (str == null)
            {
                str = "";
            }
            oldInputText = inputText;
            inputText = Keyboard.GetState();
            bool flag = (((ushort) GetKeyState(20)) & 0xffff) != 0;
            bool flag2 = false;
            if (inputText.IsKeyDown(Keys.LeftShift) || inputText.IsKeyDown(Keys.RightShift))
            {
                flag2 = true;
            }
            Keys[] pressedKeys = inputText.GetPressedKeys();
            Keys[] keysArray2 = oldInputText.GetPressedKeys();
            bool flag3 = false;
            if (inputText.IsKeyDown(Keys.Back) && oldInputText.IsKeyDown(Keys.Back))
            {
                if (backSpaceCount == 0)
                {
                    backSpaceCount = 7;
                    flag3 = true;
                }
                backSpaceCount--;
            }
            else
            {
                backSpaceCount = 15;
            }
            for (int i = 0; i < pressedKeys.Length; i++)
            {
                bool flag4 = true;
                for (int j = 0; j < keysArray2.Length; j++)
                {
                    if (pressedKeys[i] == keysArray2[j])
                    {
                        flag4 = false;
                    }
                }
                string str2 = pressedKeys[i].ToString();
                if ((str2 == "Back") && (flag4 || flag3))
                {
                    if (str.Length > 0)
                    {
                        str = str.Substring(0, str.Length - 1);
                    }
                }
                else if (flag4)
                {
                    if (str2 == "Space")
                    {
                        str2 = " ";
                    }
                    else if (str2.Length == 1)
                    {
                        int num3 = Convert.ToInt32(Convert.ToChar(str2));
                        if (((num3 >= 0x41) && (num3 <= 90)) && ((!flag2 && !flag) || (flag2 && flag)))
                        {
                            num3 += 0x20;
                            str2 = Convert.ToChar(num3).ToString();
                        }
                    }
                    else if ((str2.Length == 2) && (str2.Substring(0, 1) == "D"))
                    {
                        str2 = str2.Substring(1, 1);
                        if (flag2)
                        {
                            switch (str2)
                            {
                                case "1":
                                    str2 = "!";
                                    break;

                                case "2":
                                    str2 = "@";
                                    break;

                                case "3":
                                    str2 = "#";
                                    break;

                                case "4":
                                    str2 = "$";
                                    break;

                                case "5":
                                    str2 = "%";
                                    break;

                                case "6":
                                    str2 = "^";
                                    break;

                                case "7":
                                    str2 = "&";
                                    break;

                                case "8":
                                    str2 = "*";
                                    break;

                                case "9":
                                    str2 = "(";
                                    break;

                                case "0":
                                    str2 = ")";
                                    break;
                            }
                        }
                    }
                    else if ((str2.Length == 7) && (str2.Substring(0, 6) == "NumPad"))
                    {
                        str2 = str2.Substring(6, 1);
                    }
                    else if (str2 == "Divide")
                    {
                        str2 = "/";
                    }
                    else if (str2 == "Multiply")
                    {
                        str2 = "*";
                    }
                    else if (str2 == "Subtract")
                    {
                        str2 = "-";
                    }
                    else if (str2 == "Add")
                    {
                        str2 = "+";
                    }
                    else if (str2 == "Decimal")
                    {
                        str2 = ".";
                    }
                    else
                    {
                        if (str2 == "OemSemicolon")
                        {
                            str2 = ";";
                        }
                        else if (str2 == "OemPlus")
                        {
                            str2 = "=";
                        }
                        else if (str2 == "OemComma")
                        {
                            str2 = ",";
                        }
                        else if (str2 == "OemMinus")
                        {
                            str2 = "-";
                        }
                        else if (str2 == "OemPeriod")
                        {
                            str2 = ".";
                        }
                        else if (str2 == "OemQuestion")
                        {
                            str2 = "/";
                        }
                        else if (str2 == "OemTilde")
                        {
                            str2 = "`";
                        }
                        else if (str2 == "OemOpenBrackets")
                        {
                            str2 = "[";
                        }
                        else if (str2 == "OemPipe")
                        {
                            str2 = @"\";
                        }
                        else if (str2 == "OemCloseBrackets")
                        {
                            str2 = "]";
                        }
                        else if (str2 == "OemQuotes")
                        {
                            str2 = "'";
                        }
                        else if (str2 == "OemBackslash")
                        {
                            str2 = @"\";
                        }
                        if (flag2)
                        {
                            if (str2 == ";")
                            {
                                str2 = ":";
                            }
                            else if (str2 == "=")
                            {
                                str2 = "+";
                            }
                            else if (str2 == ",")
                            {
                                str2 = "<";
                            }
                            else if (str2 == "-")
                            {
                                str2 = "_";
                            }
                            else if (str2 == ".")
                            {
                                str2 = ">";
                            }
                            else if (str2 == "/")
                            {
                                str2 = "?";
                            }
                            else if (str2 == "`")
                            {
                                str2 = "~";
                            }
                            else if (str2 == "[")
                            {
                                str2 = "{";
                            }
                            else if (str2 == @"\")
                            {
                                str2 = "|";
                            }
                            else if (str2 == "]")
                            {
                                str2 = "}";
                            }
                            else if (str2 == "'")
                            {
                                str2 = "\"";
                            }
                        }
                    }
                    if (str2 == "Enter")
                    {
                        inputTextEnter = true;
                    }
                    if (str2.Length == 1)
                    {
                        str = str + str2;
                    }
                }
            }
            return str;
        }

        [DllImport("user32.dll", CharSet=CharSet.Auto, ExactSpelling=true)]
        public static extern short GetKeyState(int keyCode);
        [DllImport("User32")]
        private static extern int GetMenuItemCount(IntPtr hWnd);
        [DllImport("User32")]
        private static extern IntPtr GetSystemMenu(IntPtr hWnd, bool bRevert);
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
            bool flag10 = false;
            for (int i = 0; i < 0x2c; i++)
            {
                if ((player[myPlayer].inventory[i].pick > 0) && (player[myPlayer].inventory[i].name != "Copper Pickaxe"))
                {
                    flag3 = false;
                }
                if ((player[myPlayer].inventory[i].axe > 0) && (player[myPlayer].inventory[i].name != "Copper Axe"))
                {
                    flag3 = false;
                }
                if (player[myPlayer].inventory[i].hammer > 0)
                {
                    flag3 = false;
                }
                if (((player[myPlayer].inventory[i].type == 11) || (player[myPlayer].inventory[i].type == 12)) || ((player[myPlayer].inventory[i].type == 13) || (player[myPlayer].inventory[i].type == 14)))
                {
                    flag4 = true;
                }
                if (((player[myPlayer].inventory[i].type == 0x13) || (player[myPlayer].inventory[i].type == 20)) || ((player[myPlayer].inventory[i].type == 0x15) || (player[myPlayer].inventory[i].type == 0x16)))
                {
                    flag5 = true;
                }
                if (player[myPlayer].inventory[i].type == 0x4b)
                {
                    flag6 = true;
                }
                if (player[myPlayer].inventory[i].type == 0x4b)
                {
                    flag8 = true;
                }
                if ((player[myPlayer].inventory[i].type == 0x44) || (player[myPlayer].inventory[i].type == 70))
                {
                    flag9 = true;
                }
                if (player[myPlayer].inventory[i].type == 0x54)
                {
                    flag10 = true;
                }
                if (player[myPlayer].inventory[i].type == 0x75)
                {
                    flag7 = true;
                }
            }
            bool flag11 = false;
            bool flag12 = false;
            bool flag13 = false;
            bool flag14 = false;
            for (int j = 0; j < 0x3e8; j++)
            {
                if (npc[j].active)
                {
                    if (npc[j].type == 0x11)
                    {
                        flag11 = true;
                    }
                    if (npc[j].type == 0x12)
                    {
                        flag12 = true;
                    }
                    if (npc[j].type == 0x13)
                    {
                        flag14 = true;
                    }
                    if (npc[j].type == 20)
                    {
                        flag13 = true;
                    }
                }
            }
        Label_0316:
            helpText++;
            if (flag3)
            {
                if (helpText == 1)
                {
                    npcChatText = "You can use your pickaxe to dig through dirt, and your axe to chop down trees. Just place your cursor over the tile and click!";
                    return;
                }
                if (helpText == 2)
                {
                    npcChatText = "If you want to survive, you will need to create weapons and shelter. Start by chopping down trees and gathering wood.";
                    return;
                }
                if (helpText == 3)
                {
                    npcChatText = "Press ESC to access your crafting menu. When you have enough wood, create a workbench. This will allow you to create more complicated things, as long as you are standing close to it.";
                    return;
                }
                if (helpText == 4)
                {
                    npcChatText = "You can build a shelter by placing wood or other blocks in the world. Don't forget to create and place walls.";
                    return;
                }
                if (helpText == 5)
                {
                    npcChatText = "Once you have a wooden sword, you might try to gather some gel from the slimes. Combine wood and gel to make a torch!";
                    return;
                }
                if (helpText == 6)
                {
                    npcChatText = "To interact with backgrounds and placed objects, use a hammer!";
                    return;
                }
            }
            if ((flag3 && !flag4) && (!flag5 && (helpText == 11)))
            {
                npcChatText = "You should do some mining to find metal ore. You can craft very useful things with it.";
            }
            else
            {
                if ((flag3 && flag4) && !flag5)
                {
                    if (helpText == 0x15)
                    {
                        npcChatText = "Now that you have some ore, you will need to turn it into a bar in order to make items with it. This requires a furnace!";
                        return;
                    }
                    if (helpText == 0x16)
                    {
                        npcChatText = "You can create a furnace out of torches, wood, and stone. Make sure you are standing near a work bench.";
                        return;
                    }
                }
                if (flag3 && flag5)
                {
                    if (helpText == 0x1f)
                    {
                        npcChatText = "You will need an anvil to make most things out of metal bars.";
                        return;
                    }
                    if (helpText == 0x20)
                    {
                        npcChatText = "Anvils can be crafted out of iron, or purchased from a merchant.";
                        return;
                    }
                }
                if (!flag && (helpText == 0x29))
                {
                    npcChatText = "Underground are crystal hearts that can be used to increase your max life. You will need a hammer to obtain them.";
                }
                else if (!flag2 && (helpText == 0x2a))
                {
                    npcChatText = "If you gather 10 fallen stars, they can be combined to create an item that will increase your magic capacity.";
                }
                else if ((!flag2 && !flag6) && (helpText == 0x2b))
                {
                    npcChatText = "Stars fall all over the world at night. They can be used for all sorts of usefull things. If you see one, be sure to grab it because they disappear after sunrise.";
                }
                else
                {
                    if (!flag11 && !flag12)
                    {
                        if (helpText == 0x33)
                        {
                            npcChatText = "There are many different ways you can attract people to move in to our town. They will of course need a home to live in.";
                            return;
                        }
                        if (helpText == 0x34)
                        {
                            npcChatText = "In order for a room to be considered a home, it needs to have a door, chair, table, and a light source.  Make sure the house has walls as well.";
                            return;
                        }
                        if (helpText == 0x35)
                        {
                            npcChatText = "Two people will not live in the same home. Also, if their home is destroyed, they will look for a new place to live.";
                            return;
                        }
                    }
                    if (!flag11 && (helpText == 0x3d))
                    {
                        npcChatText = "If you want a merchant to move in, you will need to gather plenty of money. 50 silver coins should do the trick!";
                    }
                    else if (!flag12 && (helpText == 0x3e))
                    {
                        npcChatText = "For a nurse to move in, you might want to increase your maximum life.";
                    }
                    else if (!flag14 && (helpText == 0x3f))
                    {
                        npcChatText = "If you had a gun, I bet an arms dealer might show up to sell you some ammo!";
                    }
                    else if (!flag13 && (helpText == 0x40))
                    {
                        npcChatText = "You should prove yourself by defeating a strong monster. That will get the attention of a dryad.";
                    }
                    else if (flag8 && (helpText == 0x47))
                    {
                        npcChatText = "If you combine lenses at a demon alter, you might be able to find a way to summon a powerful monster. You will want to wait until night before using it, though.";
                    }
                    else if (flag9 && (helpText == 0x48))
                    {
                        npcChatText = "You can create worm bait with rotten chunks and vile powder. Make sure you are in a corrupt area before using it.";
                    }
                    else if ((flag8 || flag9) && (helpText == 80))
                    {
                        npcChatText = "Demonic alters can usually be found in the corruption. You will need to be near them to craft some items.";
                    }
                    else if (!flag10 && (helpText == 0xc9))
                    {
                        npcChatText = "You can make a grappling hook from a hook and 3 chains. Skeletons found deep underground usually carry hooks, and chains can be made from iron bars.";
                    }
                    else if (flag7 && (helpText == 0xca))
                    {
                        npcChatText = "You can craft a space gun using a flintlock pistol, 10 fallen stars, and 30 meteorite bars.";
                    }
                    else if (helpText == 0x3e8)
                    {
                        npcChatText = "If you see a pot, be sure to smash it open. They contain all sorts of useful supplies.";
                    }
                    else if (helpText == 0x3e9)
                    {
                        npcChatText = "There is treasure hidden all over the world. Some amazing things can be found deep underground!";
                    }
                    else if (helpText == 0x3ea)
                    {
                        npcChatText = "Smashing a shadow orb will cause a meteor to fall out of the sky. Shadow orbs can usually be found in the chasms around corrupt areas.";
                    }
                    else
                    {
                        if (helpText > 0x44c)
                        {
                            helpText = 0;
                        }
                        goto Label_0316;
                    }
                }
            }
        }

        protected override void Initialize()
        {
            if (webProtect)
            {
                this.getAuth();
                while (!webAuth)
                {
                    base.Exit();
                }
            }
            if (rand == null)
            {
                rand = new Random((int) DateTime.Now.Ticks);
            }
            if (WorldGen.genRand == null)
            {
                WorldGen.genRand = new Random((int) DateTime.Now.Ticks);
            }
            this.OpenSettings();
            switch (rand.Next(5))
            {
                case 0:
                    base.Window.Title = "Terraria: ShankShock owns your soul!";
                    break;

                case 1:
                    base.Window.Title = "Terraria: ShankShock owns your soul!";
                    break;

                case 2:
                    base.Window.Title = "Terraria: ShankShock owns your soul!";
                    break;

                case 3:
                    base.Window.Title = "Terraria: ShankShock owns your soul!";
                    goto Label_00CF;
            }
            base.Window.Title = "Terraria: ShankShock owns your soul!";
        Label_00CF:
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
            tileNoFail[0x18] = true;
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
            tileSolidTop[0x13] = true;
            tileSolid[0x13] = true;
            tileSolid[0x16] = true;
            tileSolid[0x17] = true;
            tileSolid[0x19] = true;
            tileSolid[30] = true;
            tileNoFail[0x20] = true;
            tileBlockLight[0x20] = true;
            tileSolid[0x25] = true;
            tileBlockLight[0x25] = true;
            tileSolid[0x26] = true;
            tileBlockLight[0x26] = true;
            tileSolid[0x27] = true;
            tileBlockLight[0x27] = true;
            tileSolid[40] = true;
            tileBlockLight[40] = true;
            tileSolid[0x29] = true;
            tileBlockLight[0x29] = true;
            tileSolid[0x2b] = true;
            tileBlockLight[0x2b] = true;
            tileSolid[0x2c] = true;
            tileBlockLight[0x2c] = true;
            tileSolid[0x2d] = true;
            tileBlockLight[0x2d] = true;
            tileSolid[0x2e] = true;
            tileBlockLight[0x2e] = true;
            tileSolid[0x2f] = true;
            tileBlockLight[0x2f] = true;
            tileSolid[0x30] = true;
            tileBlockLight[0x30] = true;
            tileSolid[0x35] = true;
            tileBlockLight[0x35] = true;
            tileSolid[0x36] = true;
            tileBlockLight[0x34] = true;
            tileSolid[0x38] = true;
            tileBlockLight[0x38] = true;
            tileSolid[0x39] = true;
            tileBlockLight[0x39] = true;
            tileSolid[0x3a] = true;
            tileBlockLight[0x3a] = true;
            tileSolid[0x3b] = true;
            tileBlockLight[0x3b] = true;
            tileSolid[60] = true;
            tileBlockLight[60] = true;
            tileSolid[0x3f] = true;
            tileBlockLight[0x3f] = true;
            tileStone[0x3f] = true;
            tileSolid[0x40] = true;
            tileBlockLight[0x40] = true;
            tileStone[0x40] = true;
            tileSolid[0x41] = true;
            tileBlockLight[0x41] = true;
            tileStone[0x41] = true;
            tileSolid[0x42] = true;
            tileBlockLight[0x42] = true;
            tileStone[0x42] = true;
            tileSolid[0x43] = true;
            tileBlockLight[0x43] = true;
            tileStone[0x43] = true;
            tileSolid[0x44] = true;
            tileBlockLight[0x44] = true;
            tileStone[0x44] = true;
            tileSolid[0x4b] = true;
            tileBlockLight[0x4b] = true;
            tileSolid[0x4c] = true;
            tileBlockLight[0x4c] = true;
            tileSolid[70] = true;
            tileBlockLight[70] = true;
            tileBlockLight[0x33] = true;
            tileNoFail[50] = true;
            tileNoAttach[50] = true;
            tileDungeon[0x29] = true;
            tileDungeon[0x2b] = true;
            tileDungeon[0x2c] = true;
            tileBlockLight[30] = true;
            tileBlockLight[0x19] = true;
            tileBlockLight[0x17] = true;
            tileBlockLight[0x16] = true;
            tileBlockLight[0x3e] = true;
            tileSolidTop[0x12] = true;
            tileSolidTop[14] = true;
            tileSolidTop[0x10] = true;
            tileNoAttach[20] = true;
            tileNoAttach[0x13] = true;
            tileNoAttach[13] = true;
            tileNoAttach[14] = true;
            tileNoAttach[15] = true;
            tileNoAttach[0x10] = true;
            tileNoAttach[0x11] = true;
            tileNoAttach[0x12] = true;
            tileNoAttach[0x13] = true;
            tileNoAttach[0x15] = true;
            tileNoAttach[0x1b] = true;
            tileFrameImportant[3] = true;
            tileFrameImportant[5] = true;
            tileFrameImportant[10] = true;
            tileFrameImportant[11] = true;
            tileFrameImportant[12] = true;
            tileFrameImportant[13] = true;
            tileFrameImportant[14] = true;
            tileFrameImportant[15] = true;
            tileFrameImportant[0x10] = true;
            tileFrameImportant[0x11] = true;
            tileFrameImportant[0x12] = true;
            tileFrameImportant[20] = true;
            tileFrameImportant[0x15] = true;
            tileFrameImportant[0x18] = true;
            tileFrameImportant[0x1a] = true;
            tileFrameImportant[0x1b] = true;
            tileFrameImportant[0x1c] = true;
            tileFrameImportant[0x1d] = true;
            tileFrameImportant[0x1f] = true;
            tileFrameImportant[0x21] = true;
            tileFrameImportant[0x22] = true;
            tileFrameImportant[0x23] = true;
            tileFrameImportant[0x24] = true;
            tileFrameImportant[0x2a] = true;
            tileFrameImportant[50] = true;
            tileFrameImportant[0x37] = true;
            tileFrameImportant[0x3d] = true;
            tileFrameImportant[0x47] = true;
            tileFrameImportant[0x48] = true;
            tileFrameImportant[0x49] = true;
            tileFrameImportant[0x4a] = true;
            tileFrameImportant[0x4d] = true;
            tileFrameImportant[0x4e] = true;
            tileFrameImportant[0x4f] = true;
            tileTable[14] = true;
            tileTable[0x12] = true;
            tileTable[0x13] = true;
            tileWaterDeath[4] = true;
            tileWaterDeath[0x33] = true;
            tileLavaDeath[3] = true;
            tileLavaDeath[5] = true;
            tileLavaDeath[10] = true;
            tileLavaDeath[11] = true;
            tileLavaDeath[12] = true;
            tileLavaDeath[13] = true;
            tileLavaDeath[14] = true;
            tileLavaDeath[15] = true;
            tileLavaDeath[0x10] = true;
            tileLavaDeath[0x11] = true;
            tileLavaDeath[0x12] = true;
            tileLavaDeath[0x13] = true;
            tileLavaDeath[20] = true;
            tileLavaDeath[0x1b] = true;
            tileLavaDeath[0x1c] = true;
            tileLavaDeath[0x1d] = true;
            tileLavaDeath[0x20] = true;
            tileLavaDeath[0x21] = true;
            tileLavaDeath[0x22] = true;
            tileLavaDeath[0x23] = true;
            tileLavaDeath[0x24] = true;
            tileLavaDeath[0x2a] = true;
            tileLavaDeath[0x31] = true;
            tileLavaDeath[50] = true;
            tileLavaDeath[0x34] = true;
            tileLavaDeath[0x37] = true;
            tileLavaDeath[0x3d] = true;
            tileLavaDeath[0x3e] = true;
            tileLavaDeath[0x45] = true;
            tileLavaDeath[0x47] = true;
            tileLavaDeath[0x48] = true;
            tileLavaDeath[0x49] = true;
            tileLavaDeath[0x4a] = true;
            tileLavaDeath[0x4e] = true;
            tileLavaDeath[0x4f] = true;
            wallHouse[1] = true;
            wallHouse[4] = true;
            wallHouse[5] = true;
            wallHouse[6] = true;
            wallHouse[10] = true;
            wallHouse[11] = true;
            wallHouse[12] = true;
            for (int i = 0; i < maxMenuItems; i++)
            {
                this.menuItemScale[i] = 0.8f;
            }
            for (int j = 0; j < 0x7d0; j++)
            {
                dust[j] = new Dust();
            }
            for (int k = 0; k < 0xc9; k++)
            {
                item[k] = new Item();
            }
            for (int m = 0; m < 0x3e9; m++)
            {
                npc[m] = new NPC();
                npc[m].whoAmI = m;
            }
            for (int n = 0; n < 9; n++)
            {
                player[n] = new Player();
            }
            for (int num7 = 0; num7 < 0x3e9; num7++)
            {
                projectile[num7] = new Projectile();
            }
            for (int num8 = 0; num8 < 0xc9; num8++)
            {
                gore[num8] = new Gore();
            }
            for (int num9 = 0; num9 < 100; num9++)
            {
                cloud[num9] = new Cloud();
            }
            for (int num10 = 0; num10 < 100; num10++)
            {
                combatText[num10] = new CombatText();
            }
            for (int num11 = 0; num11 < Recipe.maxRecipes; num11++)
            {
                recipe[num11] = new Recipe();
                availableRecipeY[num11] = 0x41 * num11;
            }
            Recipe.SetupRecipes();
            for (int num12 = 0; num12 < 7; num12++)
            {
                chatLine[num12] = new ChatLine();
            }
            for (int num13 = 0; num13 < Liquid.resLiquid; num13++)
            {
                liquid[num13] = new Liquid();
            }
            for (int num14 = 0; num14 < 0x2710; num14++)
            {
                liquidBuffer[num14] = new LiquidBuffer();
            }
            this.graphics.PreferredBackBufferWidth = screenWidth;
            this.graphics.PreferredBackBufferHeight = screenHeight;
            this.graphics.ApplyChanges();
            this.shop[0] = new Chest();
            this.shop[1] = new Chest();
            this.shop[1].SetupShop(1);
            this.shop[2] = new Chest();
            this.shop[2].SetupShop(2);
            this.shop[3] = new Chest();
            this.shop[3].SetupShop(3);
            this.shop[4] = new Chest();
            this.shop[4].SetupShop(4);
            teamColor[0] = Color.White;
            teamColor[1] = new Color(230, 40, 20);
            teamColor[2] = new Color(20, 200, 30);
            teamColor[3] = new Color(0x4b, 90, 0xff);
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
                IntPtr systemMenu = GetSystemMenu(base.Window.Handle, false);
                int menuItemCount = GetMenuItemCount(systemMenu);
                RemoveMenu(systemMenu, menuItemCount - 1, 0x400);
            }
            base.Initialize();
            Star.SpawnStars();
        }

        private static void InvasionWarning()
        {
            if (invasionType != 0)
            {
                string newText = "";
                if (invasionSize <= 0)
                {
                    newText = "The goblin army has been defeated!";
                }
                else if (invasionX < spawnTileX)
                {
                    newText = "A goblin army is approaching from the west!";
                }
                else if (invasionX > spawnTileX)
                {
                    newText = "A goblin army is approaching from the east!";
                }
                else
                {
                    newText = "The goblin army has arrived!";
                }
                if (netMode == 0)
                {
                    NewText(newText, 0xaf, 0x4b, 0xff);
                }
                else if (netMode == 2)
                {
                    NetMessage.SendData(0x19, -1, -1, newText, 8, 175f, 75f, 255f);
                }
            }
        }

        protected override void LoadContent()
        {
            engine = new AudioEngine(@"Content\TerrariaMusic.xgs");
            soundBank = new Microsoft.Xna.Framework.Audio.SoundBank(engine, @"Content\Sound Bank.xsb");
            waveBank = new Microsoft.Xna.Framework.Audio.WaveBank(engine, @"Content\Wave Bank.xwb");
            raTexture = base.Content.Load<Texture2D>(@"Images\ra-logo");
            reTexture = base.Content.Load<Texture2D>(@"Images\re-logo");
            splashTexture = base.Content.Load<Texture2D>(@"Images\splash");
            fadeTexture = base.Content.Load<Texture2D>(@"Images\fade-out");
            for (int i = 1; i < 7; i++)
            {
                music[i] = soundBank.GetCue("Music_" + i);
            }
            this.spriteBatch = new SpriteBatch(base.GraphicsDevice);
            for (int j = 0; j < 80; j++)
            {
                tileTexture[j] = base.Content.Load<Texture2D>(@"Images\Tiles_" + j);
            }
            for (int k = 1; k < 14; k++)
            {
                wallTexture[k] = base.Content.Load<Texture2D>(@"Images\Wall_" + k);
            }
            for (int m = 0; m < 7; m++)
            {
                backgroundTexture[m] = base.Content.Load<Texture2D>(@"Images\Background_" + m);
                backgroundWidth[m] = backgroundTexture[m].Width;
                backgroundHeight[m] = backgroundTexture[m].Height;
            }
            for (int n = 0; n < 0xec; n++)
            {
                itemTexture[n] = base.Content.Load<Texture2D>(@"Images\Item_" + n);
            }
            for (int num6 = 0; num6 < 0x2c; num6++)
            {
                npcTexture[num6] = base.Content.Load<Texture2D>(@"Images\NPC_" + num6);
            }
            for (int num7 = 0; num7 < 0x26; num7++)
            {
                projectileTexture[num7] = base.Content.Load<Texture2D>(@"Images\Projectile_" + num7);
            }
            for (int num8 = 1; num8 < 0x49; num8++)
            {
                goreTexture[num8] = base.Content.Load<Texture2D>(@"Images\Gore_" + num8);
            }
            for (int num9 = 0; num9 < 4; num9++)
            {
                cloudTexture[num9] = base.Content.Load<Texture2D>(@"Images\Cloud_" + num9);
            }
            for (int num10 = 0; num10 < 5; num10++)
            {
                starTexture[num10] = base.Content.Load<Texture2D>(@"Images\Star_" + num10);
            }
            for (int num11 = 0; num11 < 2; num11++)
            {
                liquidTexture[num11] = base.Content.Load<Texture2D>(@"Images\Liquid_" + num11);
            }
            cdTexture = base.Content.Load<Texture2D>(@"Images\CoolDown");
            logoTexture = base.Content.Load<Texture2D>(@"Images\Logo");
            dustTexture = base.Content.Load<Texture2D>(@"Images\Dust");
            sunTexture = base.Content.Load<Texture2D>(@"Images\Sun");
            moonTexture = base.Content.Load<Texture2D>(@"Images\Moon");
            blackTileTexture = base.Content.Load<Texture2D>(@"Images\Black_Tile");
            heartTexture = base.Content.Load<Texture2D>(@"Images\Heart");
            bubbleTexture = base.Content.Load<Texture2D>(@"Images\Bubble");
            manaTexture = base.Content.Load<Texture2D>(@"Images\Mana");
            cursorTexture = base.Content.Load<Texture2D>(@"Images\Cursor");
            treeTopTexture = base.Content.Load<Texture2D>(@"Images\Tree_Tops");
            treeBranchTexture = base.Content.Load<Texture2D>(@"Images\Tree_Branches");
            shroomCapTexture = base.Content.Load<Texture2D>(@"Images\Shroom_Tops");
            inventoryBackTexture = base.Content.Load<Texture2D>(@"Images\Inventory_Back");
            textBackTexture = base.Content.Load<Texture2D>(@"Images\Text_Back");
            chatTexture = base.Content.Load<Texture2D>(@"Images\Chat");
            chat2Texture = base.Content.Load<Texture2D>(@"Images\Chat2");
            chatBackTexture = base.Content.Load<Texture2D>(@"Images\Chat_Back");
            teamTexture = base.Content.Load<Texture2D>(@"Images\Team");
            for (int num12 = 1; num12 < 10; num12++)
            {
                armorBodyTexture[num12] = base.Content.Load<Texture2D>(@"Images\Armor_Body_" + num12);
                armorArmTexture[num12] = base.Content.Load<Texture2D>(@"Images\Armor_Arm_" + num12);
            }
            for (int num13 = 1; num13 < 12; num13++)
            {
                armorHeadTexture[num13] = base.Content.Load<Texture2D>(@"Images\Armor_Head_" + num13);
            }
            for (int num14 = 1; num14 < 10; num14++)
            {
                armorLegTexture[num14] = base.Content.Load<Texture2D>(@"Images\Armor_Legs_" + num14);
            }
            for (int num15 = 0; num15 < 0x11; num15++)
            {
                playerHairTexture[num15] = base.Content.Load<Texture2D>(@"Images\Player_Hair_" + (num15 + 1));
            }
            playerEyeWhitesTexture = base.Content.Load<Texture2D>(@"Images\Player_Eye_Whites");
            playerEyesTexture = base.Content.Load<Texture2D>(@"Images\Player_Eyes");
            playerHandsTexture = base.Content.Load<Texture2D>(@"Images\Player_Hands");
            playerHands2Texture = base.Content.Load<Texture2D>(@"Images\Player_Hands2");
            playerHeadTexture = base.Content.Load<Texture2D>(@"Images\Player_Head");
            playerPantsTexture = base.Content.Load<Texture2D>(@"Images\Player_Pants");
            playerShirtTexture = base.Content.Load<Texture2D>(@"Images\Player_Shirt");
            playerShoesTexture = base.Content.Load<Texture2D>(@"Images\Player_Shoes");
            playerUnderShirtTexture = base.Content.Load<Texture2D>(@"Images\Player_Undershirt");
            playerUnderShirt2Texture = base.Content.Load<Texture2D>(@"Images\Player_Undershirt2");
            chainTexture = base.Content.Load<Texture2D>(@"Images\Chain");
            chain2Texture = base.Content.Load<Texture2D>(@"Images\Chain2");
            chain3Texture = base.Content.Load<Texture2D>(@"Images\Chain3");
            chain4Texture = base.Content.Load<Texture2D>(@"Images\Chain4");
            chain5Texture = base.Content.Load<Texture2D>(@"Images\Chain5");
            chain6Texture = base.Content.Load<Texture2D>(@"Images\Chain6");
            boneArmTexture = base.Content.Load<Texture2D>(@"Images\Arm_Bone");
            soundGrab = base.Content.Load<SoundEffect>(@"Sounds\Grab");
            soundInstanceGrab = soundGrab.CreateInstance();
            soundDig[0] = base.Content.Load<SoundEffect>(@"Sounds\Dig_0");
            soundInstanceDig[0] = soundDig[0].CreateInstance();
            soundDig[1] = base.Content.Load<SoundEffect>(@"Sounds\Dig_1");
            soundInstanceDig[1] = soundDig[1].CreateInstance();
            soundDig[2] = base.Content.Load<SoundEffect>(@"Sounds\Dig_2");
            soundInstanceDig[2] = soundDig[2].CreateInstance();
            soundTink[0] = base.Content.Load<SoundEffect>(@"Sounds\Tink_0");
            soundInstanceTink[0] = soundTink[0].CreateInstance();
            soundTink[1] = base.Content.Load<SoundEffect>(@"Sounds\Tink_1");
            soundInstanceTink[1] = soundTink[1].CreateInstance();
            soundTink[2] = base.Content.Load<SoundEffect>(@"Sounds\Tink_2");
            soundInstanceTink[2] = soundTink[2].CreateInstance();
            soundPlayerHit[0] = base.Content.Load<SoundEffect>(@"Sounds\Player_Hit_0");
            soundInstancePlayerHit[0] = soundPlayerHit[0].CreateInstance();
            soundPlayerHit[1] = base.Content.Load<SoundEffect>(@"Sounds\Player_Hit_1");
            soundInstancePlayerHit[1] = soundPlayerHit[1].CreateInstance();
            soundPlayerHit[2] = base.Content.Load<SoundEffect>(@"Sounds\Player_Hit_2");
            soundInstancePlayerHit[2] = soundPlayerHit[2].CreateInstance();
            soundFemaleHit[0] = base.Content.Load<SoundEffect>(@"Sounds\Female_Hit_0");
            soundInstanceFemaleHit[0] = soundFemaleHit[0].CreateInstance();
            soundFemaleHit[1] = base.Content.Load<SoundEffect>(@"Sounds\Female_Hit_1");
            soundInstanceFemaleHit[1] = soundFemaleHit[1].CreateInstance();
            soundFemaleHit[2] = base.Content.Load<SoundEffect>(@"Sounds\Female_Hit_2");
            soundInstanceFemaleHit[2] = soundFemaleHit[2].CreateInstance();
            soundPlayerKilled = base.Content.Load<SoundEffect>(@"Sounds\Player_Killed");
            soundInstancePlayerKilled = soundPlayerKilled.CreateInstance();
            soundGrass = base.Content.Load<SoundEffect>(@"Sounds\Grass");
            soundInstanceGrass = soundGrass.CreateInstance();
            soundDoorOpen = base.Content.Load<SoundEffect>(@"Sounds\Door_Opened");
            soundInstanceDoorOpen = soundDoorOpen.CreateInstance();
            soundDoorClosed = base.Content.Load<SoundEffect>(@"Sounds\Door_Closed");
            soundInstanceDoorClosed = soundDoorClosed.CreateInstance();
            soundMenuTick = base.Content.Load<SoundEffect>(@"Sounds\Menu_Tick");
            soundInstanceMenuTick = soundMenuTick.CreateInstance();
            soundMenuOpen = base.Content.Load<SoundEffect>(@"Sounds\Menu_Open");
            soundInstanceMenuOpen = soundMenuOpen.CreateInstance();
            soundMenuClose = base.Content.Load<SoundEffect>(@"Sounds\Menu_Close");
            soundInstanceMenuClose = soundMenuClose.CreateInstance();
            soundShatter = base.Content.Load<SoundEffect>(@"Sounds\Shatter");
            soundInstanceShatter = soundShatter.CreateInstance();
            soundZombie[0] = base.Content.Load<SoundEffect>(@"Sounds\Zombie_0");
            soundInstanceZombie[0] = soundZombie[0].CreateInstance();
            soundZombie[1] = base.Content.Load<SoundEffect>(@"Sounds\Zombie_1");
            soundInstanceZombie[1] = soundZombie[1].CreateInstance();
            soundZombie[2] = base.Content.Load<SoundEffect>(@"Sounds\Zombie_2");
            soundInstanceZombie[2] = soundZombie[2].CreateInstance();
            soundRoar[0] = base.Content.Load<SoundEffect>(@"Sounds\Roar_0");
            soundInstanceRoar[0] = soundRoar[0].CreateInstance();
            soundRoar[1] = base.Content.Load<SoundEffect>(@"Sounds\Roar_1");
            soundInstanceRoar[1] = soundRoar[1].CreateInstance();
            soundSplash[0] = base.Content.Load<SoundEffect>(@"Sounds\Splash_0");
            soundInstanceSplash[0] = soundRoar[0].CreateInstance();
            soundSplash[1] = base.Content.Load<SoundEffect>(@"Sounds\Splash_1");
            soundInstanceSplash[1] = soundSplash[1].CreateInstance();
            soundDoubleJump = base.Content.Load<SoundEffect>(@"Sounds\Double_Jump");
            soundInstanceDoubleJump = soundRoar[0].CreateInstance();
            soundRun = base.Content.Load<SoundEffect>(@"Sounds\Run");
            soundInstanceRun = soundRun.CreateInstance();
            soundCoins = base.Content.Load<SoundEffect>(@"Sounds\Coins");
            soundInstanceCoins = soundCoins.CreateInstance();
            for (int num16 = 1; num16 < 0x11; num16++)
            {
                soundItem[num16] = base.Content.Load<SoundEffect>(@"Sounds\Item_" + num16);
                soundInstanceItem[num16] = soundItem[num16].CreateInstance();
            }
            for (int num17 = 1; num17 < 4; num17++)
            {
                soundNPCHit[num17] = base.Content.Load<SoundEffect>(@"Sounds\NPC_Hit_" + num17);
                soundInstanceNPCHit[num17] = soundNPCHit[num17].CreateInstance();
            }
            for (int num18 = 1; num18 < 4; num18++)
            {
                soundNPCKilled[num18] = base.Content.Load<SoundEffect>(@"Sounds\NPC_Killed_" + num18);
                soundInstanceNPCKilled[num18] = soundNPCKilled[num18].CreateInstance();
            }
            fontItemStack = base.Content.Load<SpriteFont>(@"Fonts\Item_Stack");
            fontMouseText = base.Content.Load<SpriteFont>(@"Fonts\Mouse_Text");
            fontDeathText = base.Content.Load<SpriteFont>(@"Fonts\Death_Text");
            fontCombatText = base.Content.Load<SpriteFont>(@"Fonts\Combat_Text");
        }

        private static void LoadPlayers()
        {
            Directory.CreateDirectory(PlayerPath);
            string[] files = Directory.GetFiles(PlayerPath, "*.plr");
            int length = files.Length;
            if (length > 5)
            {
                length = 5;
            }
            for (int i = 0; i < 5; i++)
            {
                loadPlayer[i] = new Player();
                if (i < length)
                {
                    loadPlayerPath[i] = files[i];
                    loadPlayer[i] = Player.LoadPlayer(loadPlayerPath[i]);
                }
            }
            numLoadPlayers = length;
        }

        public static void LoadWorlds()
        {
            Directory.CreateDirectory(WorldPath);
            string[] files = Directory.GetFiles(WorldPath, "*.wld");
            int length = files.Length;
            if (length > 5)
            {
                length = 5;
            }
            for (int i = 0; i < length; i++)
            {
                loadWorldPath[i] = files[i];
                using (FileStream stream = new FileStream(loadWorldPath[i], FileMode.Open))
                {
                    using (BinaryReader reader = new BinaryReader(stream))
                    {
                        reader.ReadInt32();
                        loadWorld[i] = reader.ReadString();
                        reader.Close();
                    }
                }
            }
            numLoadWorlds = length;
        }

        protected void MouseText(string cursorText, int rare = 0)
        {
            float num3;
            int num = mouseState.X + 10;
            int num2 = mouseState.Y + 10;
            Color color = new Color((int) mouseTextColor, (int) mouseTextColor, (int) mouseTextColor, (int) mouseTextColor);
            if (toolTip.type > 0)
            {
                rare = toolTip.rare;
                int num4 = 20;
                int index = 1;
                string[] strArray = new string[num4];
                strArray[0] = toolTip.name;
                if (toolTip.stack > 1)
                {
                    string[] strArray2 = new string[strArray.Length];
                    object obj2 = strArray2[0];
                    (strArray2 = strArray)[0] = string.Concat(new object[] { obj2, " (", toolTip.stack, ")" });
                }
                if (toolTip.damage > 0)
                {
                    strArray[index] = toolTip.damage + " damage";
                    index++;
                    if (toolTip.useStyle > 0)
                    {
                        string[] strArray3;
                        IntPtr ptr;
                        if (toolTip.useAnimation <= 8)
                        {
                            strArray[index] = "Insanely fast";
                        }
                        else if (toolTip.useAnimation <= 15)
                        {
                            strArray[index] = "Very fast";
                        }
                        else if (toolTip.useAnimation <= 20)
                        {
                            strArray[index] = "Fast";
                        }
                        else if (toolTip.useAnimation <= 0x19)
                        {
                            strArray[index] = "Average";
                        }
                        else if (toolTip.useAnimation <= 30)
                        {
                            strArray[index] = "Slow";
                        }
                        else if (toolTip.useAnimation <= 40)
                        {
                            strArray[index] = "Very slow";
                        }
                        else if (toolTip.useAnimation <= 50)
                        {
                            strArray[index] = "Extremly slow";
                        }
                        else
                        {
                            strArray[index] = "Snail";
                        }
                        (strArray3 = strArray)[(int) (ptr = (IntPtr) index)] = strArray3[(int) ptr] + " speed";
                        index++;
                    }
                }
                if (((toolTip.headSlot > 0) || (toolTip.bodySlot > 0)) || ((toolTip.legSlot > 0) || toolTip.accessory))
                {
                    strArray[index] = "Equipable";
                    index++;
                }
                if (toolTip.defense > 0)
                {
                    strArray[index] = toolTip.defense + " defense";
                    index++;
                }
                if (toolTip.pick > 0)
                {
                    strArray[index] = toolTip.pick + "% pickaxe power";
                    index++;
                }
                if (toolTip.axe > 0)
                {
                    strArray[index] = toolTip.axe + "% axe power";
                    index++;
                }
                if (toolTip.hammer > 0)
                {
                    strArray[index] = toolTip.hammer + "% hammer power";
                    index++;
                }
                if (toolTip.healLife > 0)
                {
                    strArray[index] = "Restores " + toolTip.healLife + " life";
                    index++;
                }
                if (toolTip.healMana > 0)
                {
                    strArray[index] = "Restores " + toolTip.healMana + " mana";
                    index++;
                }
                if (toolTip.mana > 0)
                {
                    strArray[index] = "Uses " + ((int) (toolTip.mana * player[myPlayer].manaCost)) + " mana";
                    index++;
                }
                if (((toolTip.createWall > 0) || (toolTip.createTile > -1)) && (toolTip.type != 0xd5))
                {
                    strArray[index] = "Can be placed";
                    index++;
                }
                if (toolTip.consumable)
                {
                    strArray[index] = "Consumable";
                    index++;
                }
                if (toolTip.toolTip != null)
                {
                    strArray[index] = toolTip.toolTip;
                    index++;
                }
                if (toolTip.wornArmor && (player[myPlayer].setBonus != ""))
                {
                    strArray[index] = "Set bonus: " + player[myPlayer].setBonus;
                    index++;
                }
                if (npcShop > 0)
                {
                    if (toolTip.value > 0)
                    {
                        string str = "";
                        int num6 = 0;
                        int num7 = 0;
                        int num8 = 0;
                        int num9 = 0;
                        int num10 = toolTip.value * toolTip.stack;
                        if (!toolTip.buy)
                        {
                            num10 /= 5;
                        }
                        if (num10 < 1)
                        {
                            num10 = 1;
                        }
                        if (num10 >= 0xf4240)
                        {
                            num6 = num10 / 0xf4240;
                            num10 -= num6 * 0xf4240;
                        }
                        if (num10 >= 0x2710)
                        {
                            num7 = num10 / 0x2710;
                            num10 -= num7 * 0x2710;
                        }
                        if (num10 >= 100)
                        {
                            num8 = num10 / 100;
                            num10 -= num8 * 100;
                        }
                        if (num10 >= 1)
                        {
                            num9 = num10;
                        }
                        if (num6 > 0)
                        {
                            str = str + num6 + " platinum ";
                        }
                        if (num7 > 0)
                        {
                            str = str + num7 + " gold ";
                        }
                        if (num8 > 0)
                        {
                            str = str + num8 + " silver ";
                        }
                        if (num9 > 0)
                        {
                            str = str + num9 + " copper ";
                        }
                        if (!toolTip.buy)
                        {
                            strArray[index] = "Sell price: " + str;
                        }
                        else
                        {
                            strArray[index] = "Buy price: " + str;
                        }
                        index++;
                        num3 = ((float) mouseTextColor) / 255f;
                        if (num6 > 0)
                        {
                            color = new Color((int) ((byte) (220f * num3)), (int) ((byte) (220f * num3)), (int) ((byte) (198f * num3)), (int) mouseTextColor);
                        }
                        else if (num7 > 0)
                        {
                            color = new Color((int) ((byte) (224f * num3)), (int) ((byte) (201f * num3)), (int) ((byte) (92f * num3)), (int) mouseTextColor);
                        }
                        else if (num8 > 0)
                        {
                            color = new Color((int) ((byte) (181f * num3)), (int) ((byte) (192f * num3)), (int) ((byte) (193f * num3)), (int) mouseTextColor);
                        }
                        else if (num9 > 0)
                        {
                            color = new Color((int) ((byte) (246f * num3)), (int) ((byte) (138f * num3)), (int) ((byte) (96f * num3)), (int) mouseTextColor);
                        }
                    }
                    else
                    {
                        num3 = ((float) mouseTextColor) / 255f;
                        strArray[index] = "No value";
                        index++;
                        color = new Color((int) ((byte) (120f * num3)), (int) ((byte) (120f * num3)), (int) ((byte) (120f * num3)), (int) mouseTextColor);
                    }
                }
                Vector2 vector = new Vector2();
                int num11 = 0;
                for (int i = 0; i < index; i++)
                {
                    Vector2 vector2 = fontMouseText.MeasureString(strArray[i]);
                    if (vector2.X > vector.X)
                    {
                        vector.X = vector2.X;
                    }
                    vector.Y += vector2.Y + num11;
                }
                if (((num + vector.X) + 4f) > screenWidth)
                {
                    num = (int) ((screenWidth - vector.X) - 4f);
                }
                if (((num2 + vector.Y) + 4f) > screenHeight)
                {
                    num2 = (int) ((screenHeight - vector.Y) - 4f);
                }
                int num13 = 0;
                num3 = ((float) mouseTextColor) / 255f;
                for (int j = 0; j < index; j++)
                {
                    for (int k = 0; k < 5; k++)
                    {
                        int num16 = num;
                        int num17 = num2 + num13;
                        Color black = Color.Black;
                        switch (k)
                        {
                            case 0:
                                num16 -= 2;
                                break;

                            case 1:
                                num16 += 2;
                                break;

                            case 2:
                                num17 -= 2;
                                break;

                            case 3:
                                num17 += 2;
                                break;

                            default:
                                black = new Color((int) mouseTextColor, (int) mouseTextColor, (int) mouseTextColor, (int) mouseTextColor);
                                if (j == 0)
                                {
                                    if (rare == 1)
                                    {
                                        black = new Color((int) ((byte) (150f * num3)), (int) ((byte) (150f * num3)), (int) ((byte) (255f * num3)), (int) mouseTextColor);
                                    }
                                    if (rare == 2)
                                    {
                                        black = new Color((int) ((byte) (150f * num3)), (int) ((byte) (255f * num3)), (int) ((byte) (150f * num3)), (int) mouseTextColor);
                                    }
                                    if (rare == 3)
                                    {
                                        black = new Color((int) ((byte) (255f * num3)), (int) ((byte) (200f * num3)), (int) ((byte) (150f * num3)), (int) mouseTextColor);
                                    }
                                    if (rare == 4)
                                    {
                                        black = new Color((int) ((byte) (255f * num3)), (int) ((byte) (150f * num3)), (int) ((byte) (150f * num3)), (int) mouseTextColor);
                                    }
                                }
                                else if (j == (index - 1))
                                {
                                    black = color;
                                }
                                break;
                        }
                        Vector2 origin = new Vector2();
                        this.spriteBatch.DrawString(fontMouseText, strArray[j], new Vector2((float) num16, (float) num17), black, 0f, origin, (float) 1f, SpriteEffects.None, 0f);
                    }
                    num13 += ((int) fontMouseText.MeasureString(strArray[j]).Y) + num11;
                }
            }
            else
            {
                Vector2 vector3 = fontMouseText.MeasureString(cursorText);
                if (((num + vector3.X) + 4f) > screenWidth)
                {
                    num = (int) ((screenWidth - vector3.X) - 4f);
                }
                if (((num2 + vector3.Y) + 4f) > screenHeight)
                {
                    num2 = (int) ((screenHeight - vector3.Y) - 4f);
                }
                this.spriteBatch.DrawString(fontMouseText, cursorText, new Vector2((float) num, (float) (num2 - 2)), Color.Black, 0f, new Vector2(), (float) 1f, SpriteEffects.None, 0f);
                this.spriteBatch.DrawString(fontMouseText, cursorText, new Vector2((float) num, (float) (num2 + 2)), Color.Black, 0f, new Vector2(), (float) 1f, SpriteEffects.None, 0f);
                this.spriteBatch.DrawString(fontMouseText, cursorText, new Vector2((float) (num - 2), (float) num2), Color.Black, 0f, new Vector2(), (float) 1f, SpriteEffects.None, 0f);
                this.spriteBatch.DrawString(fontMouseText, cursorText, new Vector2((float) (num + 2), (float) num2), Color.Black, 0f, new Vector2(), (float) 1f, SpriteEffects.None, 0f);
                num3 = ((float) mouseTextColor) / 255f;
                Color color3 = new Color((int) mouseTextColor, (int) mouseTextColor, (int) mouseTextColor, (int) mouseTextColor);
                if (rare == 1)
                {
                    color3 = new Color((int) ((byte) (150f * num3)), (int) ((byte) (150f * num3)), (int) ((byte) (255f * num3)), (int) mouseTextColor);
                }
                if (rare == 2)
                {
                    color3 = new Color((int) ((byte) (150f * num3)), (int) ((byte) (255f * num3)), (int) ((byte) (150f * num3)), (int) mouseTextColor);
                }
                if (rare == 3)
                {
                    color3 = new Color((int) ((byte) (255f * num3)), (int) ((byte) (200f * num3)), (int) ((byte) (150f * num3)), (int) mouseTextColor);
                }
                if (rare == 4)
                {
                    color3 = new Color((int) ((byte) (255f * num3)), (int) ((byte) (150f * num3)), (int) ((byte) (150f * num3)), (int) mouseTextColor);
                }
                this.spriteBatch.DrawString(fontMouseText, cursorText, new Vector2((float) num, (float) num2), color3, 0f, new Vector2(), (float) 1f, SpriteEffects.None, 0f);
            }
        }

        public static void NewText(string newText, byte R = 0xff, byte G = 0xff, byte B = 0xff)
        {
            for (int i = numChatLines - 1; i > 0; i--)
            {
                chatLine[i].text = chatLine[i - 1].text;
                chatLine[i].showTime = chatLine[i - 1].showTime;
                chatLine[i].color = chatLine[i - 1].color;
            }
            if (((R == 0) && (G == 0)) && (B == 0))
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

        private static string nextLoadPlayer()
        {
            int num = 1;
        Label_0008:;
            if (System.IO.File.Exists(string.Concat(new object[] { PlayerPath, @"\player", num, ".plr" })))
            {
                num++;
                goto Label_0008;
            }
            return string.Concat(new object[] { PlayerPath, @"\player", num, ".plr" });
        }

        private static string nextLoadWorld()
        {
            int num = 1;
        Label_0008:;
            if (System.IO.File.Exists(string.Concat(new object[] { WorldPath, @"\world", num, ".wld" })))
            {
                num++;
                goto Label_0008;
            }
            return string.Concat(new object[] { WorldPath, @"\world", num, ".wld" });
        }

        protected void OpenSettings()
        {
            try
            {
                if (System.IO.File.Exists(SavePath + @"\config.dat"))
                {
                    using (FileStream stream = new FileStream(SavePath + @"\config.dat", FileMode.Open))
                    {
                        using (BinaryReader reader = new BinaryReader(stream))
                        {
                            int num = reader.ReadInt32();
                            bool flag = reader.ReadBoolean();
                            mouseColor.R = reader.ReadByte();
                            mouseColor.G = reader.ReadByte();
                            mouseColor.B = reader.ReadByte();
                            soundVolume = reader.ReadSingle();
                            musicVolume = reader.ReadSingle();
                            cUp = reader.ReadString();
                            cDown = reader.ReadString();
                            cLeft = reader.ReadString();
                            cRight = reader.ReadString();
                            cJump = reader.ReadString();
                            cThrowItem = reader.ReadString();
                            if (num >= 1)
                            {
                                cInv = reader.ReadString();
                            }
                            caveParrallax = reader.ReadSingle();
                            if (num >= 2)
                            {
                                fixedTiming = reader.ReadBoolean();
                            }
                            reader.Close();
                            if (flag && !this.graphics.IsFullScreen)
                            {
                                this.graphics.ToggleFullScreen();
                            }
                        }
                    }
                }
            }
            catch
            {
            }
        }

        public static void PlaySound(int type, int x = -1, int y = -1, int style = 1)
        {
            if (soundVolume != 0f)
            {
                bool flag = false;
                float num = 1f;
                float num2 = 0f;
                if ((x == -1) || (y == -1))
                {
                    flag = true;
                }
                else
                {
                    if (WorldGen.gen)
                    {
                        return;
                    }
                    if (netMode == 2)
                    {
                        return;
                    }
                    Rectangle rectangle = new Rectangle(((int) screenPosition.X) - (screenWidth * 2), ((int) screenPosition.Y) - (screenHeight * 2), screenWidth * 5, screenHeight * 5);
                    Rectangle rectangle2 = new Rectangle(x, y, 1, 1);
                    Vector2 vector = new Vector2(screenPosition.X + (screenWidth * 0.5f), screenPosition.Y + (screenHeight * 0.5f));
                    if (rectangle2.Intersects(rectangle))
                    {
                        flag = true;
                    }
                    if (flag)
                    {
                        num2 = (x - vector.X) / (screenWidth * 0.5f);
                        float num3 = Math.Abs((float) (x - vector.X));
                        float num4 = Math.Abs((float) (y - vector.Y));
                        float num5 = (float) Math.Sqrt((double) ((num3 * num3) + (num4 * num4)));
                        num = 1f - (num5 / (screenWidth * 1.5f));
                    }
                }
                if (num2 < -1f)
                {
                    num2 = -1f;
                }
                if (num2 > 1f)
                {
                    num2 = 1f;
                }
                if (num > 1f)
                {
                    num = 1f;
                }
                if ((num > 0f) && flag)
                {
                    num *= soundVolume;
                    if (type == 0)
                    {
                        int index = rand.Next(3);
                        soundInstanceDig[index].Stop();
                        soundInstanceDig[index] = soundDig[index].CreateInstance();
                        soundInstanceDig[index].Volume = num;
                        soundInstanceDig[index].Pan = num2;
                        soundInstanceDig[index].Play();
                    }
                    else if (type == 1)
                    {
                        int num7 = rand.Next(3);
                        soundInstancePlayerHit[num7].Stop();
                        soundInstancePlayerHit[num7] = soundPlayerHit[num7].CreateInstance();
                        soundInstancePlayerHit[num7].Volume = num;
                        soundInstancePlayerHit[num7].Pan = num2;
                        soundInstancePlayerHit[num7].Play();
                    }
                    else if (type == 2)
                    {
                        if ((style != 9) && (style != 10))
                        {
                            soundInstanceItem[style].Stop();
                        }
                        soundInstanceItem[style] = soundItem[style].CreateInstance();
                        soundInstanceItem[style].Volume = num;
                        soundInstanceItem[style].Pan = num2;
                        soundInstanceItem[style].Play();
                    }
                    else if (type == 3)
                    {
                        soundInstanceNPCHit[style].Stop();
                        soundInstanceNPCHit[style] = soundNPCHit[style].CreateInstance();
                        soundInstanceNPCHit[style].Volume = num;
                        soundInstanceNPCHit[style].Pan = num2;
                        soundInstanceNPCHit[style].Play();
                    }
                    else if (type == 4)
                    {
                        soundInstanceNPCKilled[style] = soundNPCKilled[style].CreateInstance();
                        soundInstanceNPCKilled[style].Volume = num;
                        soundInstanceNPCKilled[style].Pan = num2;
                        soundInstanceNPCKilled[style].Play();
                    }
                    else if (type == 5)
                    {
                        soundInstancePlayerKilled.Stop();
                        soundInstancePlayerKilled = soundPlayerKilled.CreateInstance();
                        soundInstancePlayerKilled.Volume = num;
                        soundInstancePlayerKilled.Pan = num2;
                        soundInstancePlayerKilled.Play();
                    }
                    else if (type == 6)
                    {
                        soundInstanceGrass.Stop();
                        soundInstanceGrass = soundGrass.CreateInstance();
                        soundInstanceGrass.Volume = num;
                        soundInstanceGrass.Pan = num2;
                        soundInstanceGrass.Play();
                    }
                    else if (type == 7)
                    {
                        soundInstanceGrab.Stop();
                        soundInstanceGrab = soundGrab.CreateInstance();
                        soundInstanceGrab.Volume = num;
                        soundInstanceGrab.Pan = num2;
                        soundInstanceGrab.Play();
                    }
                    else if (type == 8)
                    {
                        soundInstanceDoorOpen.Stop();
                        soundInstanceDoorOpen = soundDoorOpen.CreateInstance();
                        soundInstanceDoorOpen.Volume = num;
                        soundInstanceDoorOpen.Pan = num2;
                        soundInstanceDoorOpen.Play();
                    }
                    else if (type == 9)
                    {
                        soundInstanceDoorClosed.Stop();
                        soundInstanceDoorClosed = soundDoorClosed.CreateInstance();
                        soundInstanceDoorClosed.Volume = num;
                        soundInstanceDoorClosed.Pan = num2;
                        soundInstanceDoorClosed.Play();
                    }
                    else if (type == 10)
                    {
                        soundInstanceMenuOpen.Stop();
                        soundInstanceMenuOpen = soundMenuOpen.CreateInstance();
                        soundInstanceMenuOpen.Volume = num;
                        soundInstanceMenuOpen.Pan = num2;
                        soundInstanceMenuOpen.Play();
                    }
                    else if (type == 11)
                    {
                        soundInstanceMenuClose.Stop();
                        soundInstanceMenuClose = soundMenuClose.CreateInstance();
                        soundInstanceMenuClose.Volume = num;
                        soundInstanceMenuClose.Pan = num2;
                        soundInstanceMenuClose.Play();
                    }
                    else if (type == 12)
                    {
                        soundInstanceMenuTick.Stop();
                        soundInstanceMenuTick = soundMenuTick.CreateInstance();
                        soundInstanceMenuTick.Volume = num;
                        soundInstanceMenuTick.Pan = num2;
                        soundInstanceMenuTick.Play();
                    }
                    else if (type == 13)
                    {
                        soundInstanceShatter.Stop();
                        soundInstanceShatter = soundShatter.CreateInstance();
                        soundInstanceShatter.Volume = num;
                        soundInstanceShatter.Pan = num2;
                        soundInstanceShatter.Play();
                    }
                    else if (type == 14)
                    {
                        int num8 = rand.Next(3);
                        soundInstanceZombie[num8] = soundZombie[num8].CreateInstance();
                        soundInstanceZombie[num8].Volume = num * 0.4f;
                        soundInstanceZombie[num8].Pan = num2;
                        soundInstanceZombie[num8].Play();
                    }
                    else if (type == 15)
                    {
                        soundInstanceRoar[style] = soundRoar[style].CreateInstance();
                        soundInstanceRoar[style].Volume = num;
                        soundInstanceRoar[style].Pan = num2;
                        soundInstanceRoar[style].Play();
                    }
                    else if (type == 0x10)
                    {
                        soundInstanceDoubleJump.Stop();
                        soundInstanceDoubleJump = soundDoubleJump.CreateInstance();
                        soundInstanceDoubleJump.Volume = num;
                        soundInstanceDoubleJump.Pan = num2;
                        soundInstanceDoubleJump.Play();
                    }
                    else if (type == 0x11)
                    {
                        soundInstanceRun.Stop();
                        soundInstanceRun = soundRun.CreateInstance();
                        soundInstanceRun.Volume = num;
                        soundInstanceRun.Pan = num2;
                        soundInstanceRun.Play();
                    }
                    else if (type == 0x12)
                    {
                        soundInstanceCoins = soundCoins.CreateInstance();
                        soundInstanceCoins.Volume = num;
                        soundInstanceCoins.Pan = num2;
                        soundInstanceCoins.Play();
                    }
                    else if (type == 0x13)
                    {
                        soundInstanceSplash[style] = soundSplash[style].CreateInstance();
                        soundInstanceSplash[style].Volume = num;
                        soundInstanceSplash[style].Pan = num2;
                        soundInstanceSplash[style].Play();
                    }
                    else if (type == 20)
                    {
                        int num9 = rand.Next(3);
                        soundInstanceFemaleHit[num9].Stop();
                        soundInstanceFemaleHit[num9] = soundFemaleHit[num9].CreateInstance();
                        soundInstanceFemaleHit[num9].Volume = num;
                        soundInstanceFemaleHit[num9].Pan = num2;
                        soundInstanceFemaleHit[num9].Play();
                    }
                    else if (type == 0x15)
                    {
                        int num10 = rand.Next(3);
                        soundInstanceTink[num10].Stop();
                        soundInstanceTink[num10] = soundTink[num10].CreateInstance();
                        soundInstanceTink[num10].Volume = num;
                        soundInstanceTink[num10].Pan = num2;
                        soundInstanceTink[num10].Play();
                    }
                }
            }
        }

        protected void QuitGame()
        {
            base.Exit();
        }

        [DllImport("User32")]
        private static extern int RemoveMenu(IntPtr hMenu, int nPosition, int wFlags);
        protected void SaveSettings()
        {
            Directory.CreateDirectory(SavePath);
            try
            {
                System.IO.File.SetAttributes(SavePath + @"\config.dat", FileAttributes.Normal);
            }
            catch
            {
            }
            try
            {
                using (FileStream stream = new FileStream(SavePath + @"\config.dat", FileMode.Create))
                {
                    using (BinaryWriter writer = new BinaryWriter(stream))
                    {
                        writer.Write(curRelease);
                        writer.Write(this.graphics.IsFullScreen);
                        writer.Write(mouseColor.R);
                        writer.Write(mouseColor.G);
                        writer.Write(mouseColor.B);
                        writer.Write(soundVolume);
                        writer.Write(musicVolume);
                        writer.Write(cUp);
                        writer.Write(cDown);
                        writer.Write(cLeft);
                        writer.Write(cRight);
                        writer.Write(cJump);
                        writer.Write(cThrowItem);
                        writer.Write(cInv);
                        writer.Write(caveParrallax);
                        writer.Write(fixedTiming);
                        writer.Close();
                    }
                }
            }
            catch
            {
            }
        }

        private static void StartInvasion()
        {
            if (WorldGen.shadowOrbSmashed && ((invasionType == 0) && (invasionDelay == 0)))
            {
                int num = 0;
                for (int i = 0; i < 8; i++)
                {
                    if (player[i].active)
                    {
                        num++;
                    }
                }
                if (num > 0)
                {
                    invasionType = 1;
                    invasionSize = 100 + (ShankShock.invasionMultiplier * num);
                    invasionWarn = 0;
                    if (rand.Next(2) == 0)
                    {
                        invasionX = 0.0;
                    }
                    else
                    {
                        invasionX = maxTilesX;
                    }
                }
            }
        }

        protected override void UnloadContent()
        {
        }

        protected override void Update(GameTime gameTime)
        {
            if (fixedTiming)
            {
                if (base.IsActive)
                {
                    base.IsFixedTimeStep = false;
                    this.graphics.SynchronizeWithVerticalRetrace = true;
                }
                else
                {
                    base.IsFixedTimeStep = true;
                }
            }
            else
            {
                base.IsFixedTimeStep = true;
            }
            this.UpdateMusic();
            if (!showSplash)
            {
                if (!gameMenu && (Main.netMode != 2))
                {
                    saveTimer++;
                    if (saveTimer > 0x4650)
                    {
                        saveTimer = 0;
                        WorldGen.saveToonWhilePlaying();
                    }
                }
                else
                {
                    saveTimer = 0;
                }
                if (rand.Next(0x1869f) == 0)
                {
                    rand = new Random((int) DateTime.Now.Ticks);
                }
                updateTime++;
                if (updateTime >= 60)
                {
                    frameRate = drawTime;
                    updateTime = 0;
                    drawTime = 0;
                    if (frameRate == 60)
                    {
                        Lighting.lightPasses = 2;
                        Lighting.lightSkip = 0;
                        cloudLimit = 100;
                        Gore.goreTime = 0x4b0;
                    }
                    else if (frameRate >= 0x3a)
                    {
                        Lighting.lightPasses = 2;
                        Lighting.lightSkip = 0;
                        cloudLimit = 100;
                        Gore.goreTime = 600;
                    }
                    else if (frameRate >= 0x2b)
                    {
                        Lighting.lightPasses = 2;
                        Lighting.lightSkip = 1;
                        cloudLimit = 0x4b;
                        Gore.goreTime = 300;
                    }
                    else if (frameRate >= 0x1c)
                    {
                        if (!gameMenu)
                        {
                            Liquid.maxLiquid = 0xbb8;
                            Liquid.cycles = 6;
                        }
                        Lighting.lightPasses = 2;
                        Lighting.lightSkip = 2;
                        cloudLimit = 50;
                        Gore.goreTime = 180;
                    }
                    else
                    {
                        Lighting.lightPasses = 2;
                        Lighting.lightSkip = 4;
                        cloudLimit = 0;
                        Gore.goreTime = 0;
                    }
                    if (Liquid.quickSettle)
                    {
                        Liquid.maxLiquid = Liquid.resLiquid;
                        Liquid.cycles = 1;
                    }
                    else if (frameRate == 60)
                    {
                        Liquid.maxLiquid = 0x1388;
                        Liquid.cycles = 7;
                    }
                    else if (frameRate >= 0x3a)
                    {
                        Liquid.maxLiquid = 0x1388;
                        Liquid.cycles = 12;
                    }
                    else if (frameRate >= 0x2b)
                    {
                        Liquid.maxLiquid = 0xfa0;
                        Liquid.cycles = 13;
                    }
                    else if (frameRate >= 0x1c)
                    {
                        Liquid.maxLiquid = 0xdac;
                        Liquid.cycles = 15;
                    }
                    else
                    {
                        Liquid.maxLiquid = 0xbb8;
                        Liquid.cycles = 0x11;
                    }
                    if (Main.netMode == 2)
                    {
                        cloudLimit = 0;
                    }
                }
                if (!base.IsActive)
                {
                    hasFocus = false;
                }
                else
                {
                    hasFocus = true;
                }
                if (!base.IsActive && (Main.netMode == 0))
                {
                    base.IsMouseVisible = true;
                    if ((Main.netMode != 2) && (myPlayer >= 0))
                    {
                        player[myPlayer].delayUseItem = true;
                    }
                    mouseLeftRelease = false;
                    mouseRightRelease = false;
                    if (gameMenu)
                    {
                        UpdateMenu();
                    }
                }
                else
                {
                    base.IsMouseVisible = false;
                    if ((keyState.IsKeyDown(Keys.F10) && !chatMode) && !editSign)
                    {
                        if (frameRelease)
                        {
                            PlaySound(12, -1, -1, 1);
                            if (showFrameRate)
                            {
                                showFrameRate = false;
                            }
                            else
                            {
                                showFrameRate = true;
                            }
                        }
                        frameRelease = false;
                    }
                    else
                    {
                        frameRelease = true;
                    }
                    if (keyState.IsKeyDown(Keys.F11))
                    {
                        if (releaseUI)
                        {
                            if (hideUI)
                            {
                                hideUI = false;
                            }
                            else
                            {
                                hideUI = true;
                            }
                        }
                        releaseUI = false;
                    }
                    else
                    {
                        releaseUI = true;
                    }
                    if ((keyState.IsKeyDown(Keys.LeftAlt) || keyState.IsKeyDown(Keys.RightAlt)) && keyState.IsKeyDown(Keys.Enter))
                    {
                        if (this.toggleFullscreen)
                        {
                            this.graphics.ToggleFullScreen();
                            chatRelease = false;
                        }
                        this.toggleFullscreen = false;
                    }
                    else
                    {
                        this.toggleFullscreen = true;
                    }
                    oldMouseState = mouseState;
                    mouseState = Mouse.GetState();
                    keyState = Keyboard.GetState();
                    if (editSign)
                    {
                        chatMode = false;
                    }
                    if (chatMode)
                    {
                        string chatText = Main.chatText;
                        Main.chatText = GetInputText(Main.chatText);
                        while (fontMouseText.MeasureString(Main.chatText).X > 470f)
                        {
                            Main.chatText = Main.chatText.Substring(0, Main.chatText.Length - 1);
                        }
                        if (chatText != Main.chatText)
                        {
                            PlaySound(12, -1, -1, 1);
                        }
                        if (inputTextEnter && chatRelease)
                        {
                            if (Main.chatText != "")
                            {
                                NetMessage.SendData(0x19, -1, -1, Main.chatText, myPlayer, 0f, 0f, 0f);
                            }
                            Main.chatText = "";
                            chatMode = false;
                            chatRelease = false;
                            PlaySound(11, -1, -1, 1);
                        }
                    }
                    if (keyState.IsKeyDown(Keys.Enter) && (Main.netMode == 1))
                    {
                        if ((chatRelease && !chatMode) && !editSign)
                        {
                            PlaySound(10, -1, -1, 1);
                            chatMode = true;
                            Main.chatText = "";
                        }
                        chatRelease = false;
                    }
                    else
                    {
                        chatRelease = true;
                    }
                    if (gameMenu)
                    {
                        UpdateMenu();
                        if (Main.netMode != 2)
                        {
                            return;
                        }
                    }
                    if (debugMode)
                    {
                        UpdateDebug();
                    }
                    if (Main.netMode == 1)
                    {
                        for (int num = 0; num < 0x2c; num++)
                        {
                            if (player[myPlayer].inventory[num].IsNotTheSameAs(clientPlayer.inventory[num]))
                            {
                                NetMessage.SendData(5, -1, -1, player[myPlayer].inventory[num].name, myPlayer, (float) num, 0f, 0f);
                            }
                        }
                        if (player[myPlayer].armor[0].IsNotTheSameAs(clientPlayer.armor[0]))
                        {
                            NetMessage.SendData(5, -1, -1, player[myPlayer].armor[0].name, myPlayer, 44f, 0f, 0f);
                        }
                        if (player[myPlayer].armor[1].IsNotTheSameAs(clientPlayer.armor[1]))
                        {
                            NetMessage.SendData(5, -1, -1, player[myPlayer].armor[1].name, myPlayer, 45f, 0f, 0f);
                        }
                        if (player[myPlayer].armor[2].IsNotTheSameAs(clientPlayer.armor[2]))
                        {
                            NetMessage.SendData(5, -1, -1, player[myPlayer].armor[2].name, myPlayer, 46f, 0f, 0f);
                        }
                        if (player[myPlayer].armor[3].IsNotTheSameAs(clientPlayer.armor[3]))
                        {
                            NetMessage.SendData(5, -1, -1, player[myPlayer].armor[3].name, myPlayer, 47f, 0f, 0f);
                        }
                        if (player[myPlayer].armor[4].IsNotTheSameAs(clientPlayer.armor[4]))
                        {
                            NetMessage.SendData(5, -1, -1, player[myPlayer].armor[4].name, myPlayer, 48f, 0f, 0f);
                        }
                        if (player[myPlayer].armor[5].IsNotTheSameAs(clientPlayer.armor[5]))
                        {
                            NetMessage.SendData(5, -1, -1, player[myPlayer].armor[5].name, myPlayer, 49f, 0f, 0f);
                        }
                        if (player[myPlayer].chest != clientPlayer.chest)
                        {
                            NetMessage.SendData(0x21, -1, -1, "", player[myPlayer].chest, 0f, 0f, 0f);
                        }
                        if (player[myPlayer].talkNPC != clientPlayer.talkNPC)
                        {
                            NetMessage.SendData(40, -1, -1, "", myPlayer, 0f, 0f, 0f);
                        }
                        if (player[myPlayer].zoneEvil != clientPlayer.zoneEvil)
                        {
                            NetMessage.SendData(0x24, -1, -1, "", myPlayer, 0f, 0f, 0f);
                        }
                        if (player[myPlayer].zoneMeteor != clientPlayer.zoneMeteor)
                        {
                            NetMessage.SendData(0x24, -1, -1, "", myPlayer, 0f, 0f, 0f);
                        }
                        if (player[myPlayer].zoneDungeon != clientPlayer.zoneDungeon)
                        {
                            NetMessage.SendData(0x24, -1, -1, "", myPlayer, 0f, 0f, 0f);
                        }
                        if (player[myPlayer].zoneJungle != clientPlayer.zoneJungle)
                        {
                            NetMessage.SendData(0x24, -1, -1, "", myPlayer, 0f, 0f, 0f);
                        }
                    }
                    if (Main.netMode == 1)
                    {
                        clientPlayer = (Player) player[myPlayer].clientClone();
                    }
                    for (int i = 0; i < 8; i++)
                    {
                        if (ignoreErrors)
                        {
                            try
                            {
                                player[i].UpdatePlayer(i);
                            }
                            catch
                            {
                            }
                        }
                        else
                        {
                            player[i].UpdatePlayer(i);
                        }
                    }
                    if (Main.netMode != 1)
                    {
                        NPC.SpawnNPC();
                    }
                    for (int j = 0; j < 8; j++)
                    {
                        player[j].activeNPCs = 0;
                        player[j].townNPCs = 0;
                    }
                    for (int k = 0; k < 0x3e8; k++)
                    {
                        if (ignoreErrors)
                        {
                            try
                            {
                                npc[k].UpdateNPC(k);
                            }
                            catch
                            {
                                npc[k] = new NPC();
                            }
                        }
                        else
                        {
                            npc[k].UpdateNPC(k);
                        }
                    }
                    for (int m = 0; m < 200; m++)
                    {
                        if (ignoreErrors)
                        {
                            try
                            {
                                gore[m].Update();
                            }
                            catch
                            {
                                gore[m] = new Gore();
                            }
                        }
                        else
                        {
                            gore[m].Update();
                        }
                    }
                    for (int n = 0; n < 0x3e8; n++)
                    {
                        if (ignoreErrors)
                        {
                            try
                            {
                                projectile[n].Update(n);
                            }
                            catch
                            {
                                projectile[n] = new Projectile();
                            }
                        }
                        else
                        {
                            projectile[n].Update(n);
                        }
                    }
                    for (int num7 = 0; num7 < 200; num7++)
                    {
                        if (ignoreErrors)
                        {
                            try
                            {
                                item[num7].UpdateItem(num7);
                            }
                            catch
                            {
                                item[num7] = new Item();
                            }
                        }
                        else
                        {
                            item[num7].UpdateItem(num7);
                        }
                    }
                    if (ignoreErrors)
                    {
                        try
                        {
                            Dust.UpdateDust();
                        }
                        catch
                        {
                            for (int num8 = 0; num8 < 0x7d0; num8++)
                            {
                                dust[num8] = new Dust();
                            }
                        }
                    }
                    else
                    {
                        Dust.UpdateDust();
                    }
                    if (Main.netMode != 2)
                    {
                        CombatText.UpdateCombatText();
                    }
                    if (ignoreErrors)
                    {
                        try
                        {
                            UpdateTime();
                        }
                        catch
                        {
                            checkForSpawns = 0;
                        }
                    }
                    else
                    {
                        UpdateTime();
                    }
                    if (Main.netMode != 1)
                    {
                        if (ignoreErrors)
                        {
                            try
                            {
                                WorldGen.UpdateWorld();
                                UpdateInvasion();
                            }
                            catch
                            {
                            }
                        }
                        else
                        {
                            WorldGen.UpdateWorld();
                            UpdateInvasion();
                        }
                    }
                    if (ignoreErrors)
                    {
                        try
                        {
                            if (Main.netMode == 2)
                            {
                                UpdateServer();
                            }
                            if (Main.netMode == 1)
                            {
                                UpdateClient();
                            }
                        }
                        catch
                        {
                            int netMode = Main.netMode;
                        }
                    }
                    else
                    {
                        if (Main.netMode == 2)
                        {
                            UpdateServer();
                        }
                        if (Main.netMode == 1)
                        {
                            UpdateClient();
                            if (this.graphics.PreferredBackBufferHeight == 600)
                            {
                                this.QuitGame();
                            }
                            if (this.graphics.PreferredBackBufferWidth == 800)
                            {
                                this.QuitGame();
                            }
                        }
                    }
                    if (ignoreErrors)
                    {
                        try
                        {
                            for (int num9 = 0; num9 < numChatLines; num9++)
                            {
                                if (chatLine[num9].showTime > 0)
                                {
                                    ChatLine line1 = chatLine[num9];
                                    line1.showTime--;
                                }
                            }
                        }
                        catch
                        {
                            for (int num10 = 0; num10 < numChatLines; num10++)
                            {
                                chatLine[num10] = new ChatLine();
                            }
                        }
                    }
                    else
                    {
                        for (int num11 = 0; num11 < numChatLines; num11++)
                        {
                            if (chatLine[num11].showTime > 0)
                            {
                                ChatLine line2 = chatLine[num11];
                                line2.showTime--;
                            }
                        }
                    }
                    base.Update(gameTime);
                }
            }
        }

        private static void UpdateClient()
        {
            if (myPlayer == 8)
            {
                Netplay.disconnect = true;
            }
            netPlayCounter++;
            if (netPlayCounter > 0xe10)
            {
                netPlayCounter = 0;
            }
            if (Math.IEEERemainder((double) netPlayCounter, 300.0) == 0.0)
            {
                NetMessage.SendData(13, -1, -1, "", myPlayer, 0f, 0f, 0f);
                NetMessage.SendData(0x24, -1, -1, "", myPlayer, 0f, 0f, 0f);
            }
            if (Math.IEEERemainder((double) netPlayCounter, 600.0) == 0.0)
            {
                NetMessage.SendData(0x10, -1, -1, "", myPlayer, 0f, 0f, 0f);
                NetMessage.SendData(40, -1, -1, "", myPlayer, 0f, 0f, 0f);
            }
            if (Netplay.clientSock.active)
            {
                Netplay.clientSock.timeOut++;
                if (!stopTimeOuts && (Netplay.clientSock.timeOut > (60 * timeOut)))
                {
                    statusText = "Connection timed out";
                    Netplay.disconnect = true;
                }
            }
            for (int i = 0; i < 200; i++)
            {
                if (item[i].active && (item[i].owner == myPlayer))
                {
                    item[i].FindOwner(i);
                }
            }
        }

        private static void UpdateDebug()
        {
            if (netMode != 2)
            {
                if (keyState.IsKeyDown(Keys.Left))
                {
                    screenPosition.X -= 32f;
                }
                if (keyState.IsKeyDown(Keys.Right))
                {
                    screenPosition.X += 32f;
                }
                if (keyState.IsKeyDown(Keys.Up))
                {
                    screenPosition.Y -= 32f;
                }
                if (keyState.IsKeyDown(Keys.Down))
                {
                    screenPosition.Y += 32f;
                }
                int i = 0;
                int j = 0;
                i = (int) ((mouseState.X + screenPosition.X) / 16f);
                j = (int) ((mouseState.Y + screenPosition.Y) / 16f);
                if ((((mouseState.X < screenWidth) && (mouseState.Y < screenHeight)) && ((i >= 0) && (j >= 0))) && ((i < maxTilesX) && (j < maxTilesY)))
                {
                    Lighting.addLight(i, j, 1f);
                    if (mouseState.RightButton == ButtonState.Pressed)
                    {
                        ButtonState leftButton = mouseState.LeftButton;
                    }
                    if (mouseState.RightButton == ButtonState.Pressed)
                    {
                        int myPlayer = Main.myPlayer;
                        if (player[myPlayer].active)
                        {
                            player[myPlayer].position.X = i * 0x10;
                            player[myPlayer].position.Y = j * 0x10;
                            player[myPlayer].fallStart = (int) (player[myPlayer].position.Y / 16f);
                            NetMessage.SendData(13, -1, -1, "", myPlayer, 0f, 0f, 0f);
                        }
                        for (int k = -1; k < 2; k++)
                        {
                            for (int m = -1; m < 2; m++)
                            {
                            }
                        }
                    }
                    else if (mouseState.LeftButton == ButtonState.Pressed)
                    {
                        for (int n = -5; n <= 5; n++)
                        {
                            for (int num7 = 5; num7 >= -5; num7--)
                            {
                                if (netMode != 1)
                                {
                                    Liquid.AddWater(i + n, j + num7);
                                }
                            }
                        }
                    }
                }
            }
        }

        private static void UpdateInvasion()
        {
            if (invasionType > 0)
            {
                if (invasionSize <= 0)
                {
                    InvasionWarning();
                    invasionType = 0;
                    invasionDelay = 7;
                }
                if (invasionX != spawnTileX)
                {
                    float num = 0.2f;
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
                    else if (invasionX < spawnTileX)
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
                    if (invasionWarn <= 0)
                    {
                        invasionWarn = 0xe10;
                        InvasionWarning();
                    }
                }
            }
        }

        private static void UpdateMenu()
        {
            playerInventory = false;
            exitScale = 0.8f;
            if (netMode == 0)
            {
                if (!grabSky)
                {
                    time += 86.4;
                    if (dayTime)
                    {
                        if (time > 54000.0)
                        {
                            time = 0.0;
                            dayTime = false;
                        }
                    }
                    else if (time > 32400.0)
                    {
                        bloodMoon = false;
                        time = 0.0;
                        dayTime = true;
                        moonPhase++;
                        if (moonPhase >= 8)
                        {
                            moonPhase = 0;
                        }
                    }
                }
            }
            else if (netMode == 1)
            {
                UpdateTime();
            }
        }

        protected void UpdateMusic()
        {
            if (this.curMusic > 0)
            {
                if (!base.IsActive)
                {
                    if (!music[this.curMusic].IsPaused && music[this.curMusic].IsPlaying)
                    {
                        music[this.curMusic].Pause();
                    }
                    return;
                }
                if (music[this.curMusic].IsPaused)
                {
                    music[this.curMusic].Resume();
                }
            }
            bool flag = false;
            Rectangle rectangle = new Rectangle((int) screenPosition.X, (int) screenPosition.Y, screenWidth, screenHeight);
            int num = 0x1388;
            for (int i = 0; i < 0x3e8; i++)
            {
                if (npc[i].active && (((npc[i].boss || (npc[i].type == 13)) || ((npc[i].type == 14) || (npc[i].type == 15))) || (((npc[i].type == 0x1a) || (npc[i].type == 0x1b)) || ((npc[i].type == 0x1c) || (npc[i].type == 0x1d)))))
                {
                    Rectangle rectangle2 = new Rectangle((((int) npc[i].position.X) + (npc[i].width / 2)) - num, (((int) npc[i].position.Y) + (npc[i].height / 2)) - num, num * 2, num * 2);
                    if (rectangle.Intersects(rectangle2))
                    {
                        flag = true;
                        break;
                    }
                }
            }
            if (musicVolume == 0f)
            {
                this.newMusic = 0;
            }
            else if (gameMenu)
            {
                if (netMode != 2)
                {
                    this.newMusic = 6;
                }
                else
                {
                    this.newMusic = 0;
                }
            }
            else if (flag)
            {
                this.newMusic = 5;
            }
            else if ((player[myPlayer].zoneEvil || player[myPlayer].zoneMeteor) || player[myPlayer].zoneDungeon)
            {
                this.newMusic = 2;
            }
            else if (player[myPlayer].position.Y > ((maxTilesY - 200) * 0x10))
            {
                this.newMusic = 2;
            }
            else if (player[myPlayer].position.Y > ((worldSurface * 16.0) + screenHeight))
            {
                this.newMusic = 4;
            }
            else if (dayTime)
            {
                this.newMusic = 1;
            }
            else if (!dayTime)
            {
                if (bloodMoon)
                {
                    this.newMusic = 2;
                }
                else
                {
                    this.newMusic = 3;
                }
            }
            this.curMusic = this.newMusic;
            for (int j = 1; j < 7; j++)
            {
                if (j == this.curMusic)
                {
                    if (!music[j].IsPlaying)
                    {
                        music[j] = soundBank.GetCue("Music_" + j);
                        music[j].Play();
                        music[j].SetVariable("Volume", musicFade[j] * musicVolume);
                    }
                    else
                    {
                        musicFade[j] += 0.005f;
                        if (musicFade[j] > 1f)
                        {
                            musicFade[j] = 1f;
                        }
                        music[j].SetVariable("Volume", musicFade[j] * musicVolume);
                    }
                }
                else if (music[j].IsPlaying)
                {
                    if (musicFade[this.curMusic] > 0.25f)
                    {
                        musicFade[j] -= 0.005f;
                    }
                    else if (this.curMusic == 0)
                    {
                        musicFade[j] = 0f;
                    }
                    if (musicFade[j] <= 0f)
                    {
                        musicFade[j] -= 0f;
                        music[j].Stop(AudioStopOptions.Immediate);
                    }
                    else
                    {
                        music[j].SetVariable("Volume", musicFade[j] * musicVolume);
                    }
                }
                else
                {
                    musicFade[j] = 0f;
                }
            }
        }

        private static void UpdateServer()
        {
            netPlayCounter++;
            if (netPlayCounter > 0xe10)
            {
                NetMessage.SendData(7, -1, -1, "", 0, 0f, 0f, 0f);
                NetMessage.syncPlayers();
                netPlayCounter = 0;
            }
            Math.IEEERemainder((double) netPlayCounter, 60.0);
            if (Math.IEEERemainder((double) netPlayCounter, 360.0) == 0.0)
            {
                bool flag2 = true;
                int lastItemUpdate = Main.lastItemUpdate;
                int num4 = 0;
                while (flag2)
                {
                    lastItemUpdate++;
                    if (lastItemUpdate >= 200)
                    {
                        lastItemUpdate = 0;
                    }
                    num4++;
                    if (!item[lastItemUpdate].active || (item[lastItemUpdate].owner == 8))
                    {
                        NetMessage.SendData(0x15, -1, -1, "", lastItemUpdate, 0f, 0f, 0f);
                    }
                    if ((num4 >= maxItemUpdates) || (lastItemUpdate == Main.lastItemUpdate))
                    {
                        flag2 = false;
                    }
                }
                Main.lastItemUpdate = lastItemUpdate;
            }
            for (int i = 0; i < 200; i++)
            {
                if (item[i].active && ((item[i].owner == 8) || !player[item[i].owner].active))
                {
                    item[i].FindOwner(i);
                }
            }
            for (int j = 0; j < 8; j++)
            {
                if (Netplay.serverSock[j].active)
                {
                    ServerSock sock1 = Netplay.serverSock[j];
                    sock1.timeOut++;
                    if (!stopTimeOuts && (Netplay.serverSock[j].timeOut > (60 * timeOut)))
                    {
                        Netplay.serverSock[j].kill = true;
                    }
                }
                if (player[j].active)
                {
                    int sectionX = Netplay.GetSectionX((int) (player[j].position.X / 16f));
                    int sectionY = Netplay.GetSectionY((int) (player[j].position.Y / 16f));
                    int num9 = 0;
                    for (int k = sectionX - 1; k < (sectionX + 2); k++)
                    {
                        for (int m = sectionY - 1; m < (sectionY + 2); m++)
                        {
                            if ((((k >= 0) && (k < maxSectionsX)) && ((m >= 0) && (m < maxSectionsY))) && !Netplay.serverSock[j].tileSection[k, m])
                            {
                                num9++;
                            }
                        }
                    }
                    if (num9 > 0)
                    {
                        int number = num9 * 150;
                        NetMessage.SendData(9, j, -1, "Recieving tile data", number, 0f, 0f, 0f);
                        Netplay.serverSock[j].statusText2 = "is recieving tile data";
                        ServerSock sock2 = Netplay.serverSock[j];
                        sock2.statusMax += number;
                        for (int n = sectionX - 1; n < (sectionX + 2); n++)
                        {
                            for (int num14 = sectionY - 1; num14 < (sectionY + 2); num14++)
                            {
                                if ((((n >= 0) && (n < maxSectionsX)) && ((num14 >= 0) && (num14 < maxSectionsY))) && !Netplay.serverSock[j].tileSection[n, num14])
                                {
                                    NetMessage.SendSection(j, n, num14);
                                    NetMessage.SendData(11, j, -1, "", n, (float) num14, (float) n, (float) num14);
                                }
                            }
                        }
                    }
                }
            }
        }

        public static void startInv()
        {
            invasionDelay = 0;
            invasionType = 0;
            WorldGen.shadowOrbSmashed = true;
            StartInvasion();
        }

        public static void UpdateT()
        {
            UpdateTime();
        }

        private static void UpdateTime()
        {
            bool flag;
            time++;
            if (dayTime)
            {
                if (time <= 54000.0)
                {
                    goto Label_0380;
                }
                WorldGen.spawnNPC = 0;
                checkForSpawns = 0;
                if (((rand.Next(50) == 0) && (netMode != 1)) && WorldGen.shadowOrbSmashed)
                {
                    WorldGen.spawnMeteor = true;
                }
                if (NPC.downedBoss1 || (netMode == 1))
                {
                    goto Label_0293;
                }
                flag = false;
                for (int i = 0; i < 8; i++)
                {
                    if (player[i].active && (player[i].statLifeMax >= 200))
                    {
                        flag = true;
                        break;
                    }
                }
            }
            else
            {
                if ((WorldGen.spawnEye && (netMode != 1)) && (time > 4860.0))
                {
                    for (int j = 0; j < 8; j++)
                    {
                        if ((player[j].active && !player[j].dead) && (player[j].position.Y < (worldSurface * 16.0)))
                        {
                            NPC.SpawnOnPlayer(j, 4);
                            WorldGen.spawnEye = false;
                            break;
                        }
                    }
                }
                if (time > 32400.0)
                {
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
                        NetMessage.SendData(7, -1, -1, "", 0, 0f, 0f, 0f);
                        WorldGen.saveAndPlay();
                    }
                    if ((netMode != 1) && (rand.Next(15) == 0))
                    {
                        StartInvasion();
                    }
                }
                if ((time > 16200.0) && WorldGen.spawnMeteor)
                {
                    WorldGen.spawnMeteor = false;
                    WorldGen.dropMeteor();
                }
                return;
            }
            if (flag && (rand.Next(3) == 0))
            {
                int num3 = 0;
                for (int k = 0; k < 0x3e8; k++)
                {
                    if (npc[k].active && npc[k].townNPC)
                    {
                        num3++;
                    }
                }
                if (num3 >= 4)
                {
                    WorldGen.spawnEye = true;
                    if (netMode == 0)
                    {
                        NewText("You feel an evil presence watching you...", 50, 0xff, 130);
                    }
                    else if (netMode == 2)
                    {
                        NetMessage.SendData(0x19, -1, -1, "You feel an evil presence watching you...", 8, 50f, 255f, 130f);
                    }
                }
            }
        Label_0293:
            if (((moonPhase != 4)) && ((rand.Next(7) == 0) && (netMode != 1)))
            {
                for (int m = 0; m < 8; m++)
                {
                    if (player[m].active && (player[m].statLifeMax > 100))
                    {
                        bloodMoon = true;
                        break;
                    }
                }
            }
            if (bloodMoon)
            {
                if (netMode == 0)
                {
                    NewText("The Blood Moon is rising...", 50, 0xff, 130);
                }
                else if (netMode == 2)
                {
                    NetMessage.SendData(0x19, -1, -1, "The Blood Moon is rising...", 8, 50f, 255f, 130f);
                }
            }
            time = 0.0;
            dayTime = false;
            if (netMode == 2)
            {
                NetMessage.SendData(7, -1, -1, "", 0, 0f, 0f, 0f);
            }
        Label_0380:
            if (netMode != 1)
            {
                checkForSpawns++;
                if (checkForSpawns >= 0x1c20)
                {
                    int num6 = 0;
                    for (int n = 0; n < 8; n++)
                    {
                        if (player[n].active)
                        {
                            num6++;
                        }
                    }
                    checkForSpawns = 0;
                    WorldGen.spawnNPC = 0;
                    int num8 = 0;
                    int num9 = 0;
                    int num10 = 0;
                    int num11 = 0;
                    int num12 = 0;
                    int num13 = 0;
                    int num14 = 0;
                    int num15 = 0;
                    for (int num16 = 0; num16 < 0x3e8; num16++)
                    {
                        if (npc[num16].active && npc[num16].townNPC)
                        {
                            if ((npc[num16].type != 0x25) && !npc[num16].homeless)
                            {
                                WorldGen.QuickFindHome(num16);
                            }
                            else
                            {
                                num13++;
                            }
                            if (npc[num16].type == 0x11)
                            {
                                num8++;
                            }
                            if (npc[num16].type == 0x12)
                            {
                                num9++;
                            }
                            if (npc[num16].type == 0x13)
                            {
                                num11++;
                            }
                            if (npc[num16].type == 20)
                            {
                                num10++;
                            }
                            if (npc[num16].type == 0x16)
                            {
                                num12++;
                            }
                            if (npc[num16].type == 0x26)
                            {
                                num14++;
                            }
                            num15++;
                        }
                    }
                    if (WorldGen.spawnNPC == 0)
                    {
                        int num17 = 0;
                        bool flag2 = false;
                        int num18 = 0;
                        bool flag3 = false;
                        bool flag4 = false;
                        for (int num19 = 0; num19 < 8; num19++)
                        {
                            if (player[num19].active)
                            {
                                for (int num20 = 0; num20 < 0x2c; num20++)
                                {
                                    if ((player[num19].inventory[num20] != null) & (player[num19].inventory[num20].stack > 0))
                                    {
                                        if (player[num19].inventory[num20].type == 0x47)
                                        {
                                            num17 += player[num19].inventory[num20].stack;
                                        }
                                        if (player[num19].inventory[num20].type == 0x48)
                                        {
                                            num17 += player[num19].inventory[num20].stack * 100;
                                        }
                                        if (player[num19].inventory[num20].type == 0x49)
                                        {
                                            num17 += player[num19].inventory[num20].stack * 0x2710;
                                        }
                                        if (player[num19].inventory[num20].type == 0x4a)
                                        {
                                            num17 += player[num19].inventory[num20].stack * 0xf4240;
                                        }
                                        if (((player[num19].inventory[num20].type == 0x5f) || (player[num19].inventory[num20].type == 0x60)) || (((player[num19].inventory[num20].type == 0x61) || (player[num19].inventory[num20].type == 0x62)) || (player[num19].inventory[num20].useAmmo == 14)))
                                        {
                                            flag3 = true;
                                        }
                                        if ((player[num19].inventory[num20].type == 0xa6) || (player[num19].inventory[num20].type == 0xa7))
                                        {
                                            flag4 = true;
                                        }
                                    }
                                }
                                int num21 = player[num19].statLifeMax / 20;
                                if (num21 > 5)
                                {
                                    flag2 = true;
                                }
                                num18 += num21;
                            }
                        }
                        if ((WorldGen.spawnNPC == 0) && (num12 < 1))
                        {
                            WorldGen.spawnNPC = 0x16;
                        }
                        if (((WorldGen.spawnNPC == 0) && (num17 > 5000.0)) && (num8 < 1))
                        {
                            WorldGen.spawnNPC = 0x11;
                        }
                        if (((WorldGen.spawnNPC == 0) && flag2) && (num9 < 1))
                        {
                            WorldGen.spawnNPC = 0x12;
                        }
                        if (((WorldGen.spawnNPC == 0) && flag3) && (num11 < 1))
                        {
                            WorldGen.spawnNPC = 0x13;
                        }
                        if (((WorldGen.spawnNPC == 0) && ((NPC.downedBoss1 || NPC.downedBoss2) || NPC.downedBoss3)) && (num10 < 1))
                        {
                            WorldGen.spawnNPC = 20;
                        }
                        if (((WorldGen.spawnNPC == 0) && flag4) && ((num8 > 0) && (num14 < 1)))
                        {
                            WorldGen.spawnNPC = 0x26;
                        }
                        if (((WorldGen.spawnNPC == 0) && (num17 > 0x186a0)) && ((num8 < 2) && (num6 > 2)))
                        {
                            WorldGen.spawnNPC = 0x11;
                        }
                        if (((WorldGen.spawnNPC == 0) && (num18 >= 20)) && ((num9 < 2) && (num6 > 2)))
                        {
                            WorldGen.spawnNPC = 0x12;
                        }
                        if (((WorldGen.spawnNPC == 0) && (num17 > 0x4c4b40)) && ((num8 < 3) && (num6 > 4)))
                        {
                            WorldGen.spawnNPC = 0x11;
                        }
                        if (!NPC.downedBoss3 && (num13 == 0))
                        {
                            int index = NPC.NewNPC((dungeonX * 0x10) + 8, dungeonY * 0x10, 0x25, 0);
                            npc[index].homeless = false;
                            npc[index].homeTileX = dungeonX;
                            npc[index].homeTileY = dungeonY;
                        }
                    }
                }
            }
        }
    }
}

