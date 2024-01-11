using System;
using System.ComponentModel;
using System.Globalization;
using System.Numerics;
using System.Reflection;
using System.Reflection.PortableExecutable;
using Raylib_cs;
using static System.Net.Mime.MediaTypeNames;
using static Raylib_cs.Raylib;
namespace Peko_Jump;
public enum GameState
{
    logo,
    menu,
    stage,
    credits,
    end
}

public class Program
{
    private static int timer = 0;


    private static Random Rnd;
    private static readonly (float, bool)[] Columns = new (float, bool)[4];
    private static GameState gamestate = GameState.logo;
    private static Raylib_cs.Image icon;
    private static Texture2D gameoverscreen;
    private static Texture2D title;
    private static Texture2D menubackground;
    private static Texture2D pekoface;
    private static Texture2D peko;
    private static Texture2D logo1;
    private static Texture2D space;
    private static Texture2D cForCredits;

    private static Texture2D platform;
    private static Texture2D platformbroken;
    private static Texture2D cloud;
    private static Texture2D trampoline;
    private static Texture2D bForMenu;

    private static Texture2D stars;
    private static Texture2D fallingStars;
    private static Texture2D ringStars;
    private static Texture2D dotstars;

    private static Sound ending;
    private static Sound logo;
    private static Sound jump;
    private static Sound brick;
    private static Sound boing;
    private static Music stagemusic;
    private static Music menu;
    private static Music credits;
    private static int score = 0;
    private static int maxscore = 0;
    private static float Width = 1600;
    private static float Height = 900;
    private const float windowWidth = 1600;
    private const float windowHeight = 900;
    private const float origWidth = 1920;
    private const float origHeight = 1080;
    private static RenderTexture2D rtex;
    private static Font font;
    private static Font opensans;
    private static List<Texture2D> textures = new List<Texture2D>();
    private static string creditText;


    public static float Smoothstep(float x)
    {
        return x * x * (3 - 2 * x);
    }
    public static void Main()
    {
        var folder = System.IO.Path.GetDirectoryName(Environment.GetCommandLineArgs()[0]);
        var images = Path.Combine(folder, Resource1.images);
        var audio = Path.Combine(folder, Resource1.audio);

        Rnd = new Random();
        InitWindow((int)Width, (int)Height, Resource1.PekoJump);
        icon = LoadImage(Path.Combine(Resource1.images, "carrot.png"));
        SetWindowIcon(icon);
        SetConfigFlags(ConfigFlags.FLAG_VSYNC_HINT);
        SetTargetFPS(60);
        InitAudioDevice();
        creditText = File.ReadAllText("Resources/Credits.txt");
        gameoverscreen = LoadTexture(images, "gameover.png");
        title = LoadTexture(images, "title.png");
        menubackground = LoadTexture(images, "menubackground.png");
        pekoface = LoadTexture(images, "pekoFace.png");
        platform = LoadTexture(images, "platform.png");
        platformbroken = LoadTexture(images, "platformbroken.png");
        cloud = LoadTexture(images, "cloud.png");
        trampoline = LoadTexture(images, "trampoline.png");
        peko = LoadTexture(images, "pekora.png");
        logo1 = LoadTexture(images, "logo.png");
        space = LoadTexture(images, "space.png");
        cForCredits = LoadTexture(images, "credits.png");
        bForMenu = LoadTexture(images, "returnToMenu.png");

        stars = LoadTexture(images, "stars.png");
        fallingStars = LoadTexture(images, "fallingStars.png");
        ringStars = LoadTexture(images, "ringStars.png");
        dotstars = LoadTexture(images, "dotstars.png");

        logo = LoadSound(Path.Combine(audio, "logo.mp3"));
        jump = LoadSound(Path.Combine(audio, "jump.mp3"));
        ending = LoadSound(Path.Combine(audio, "ending.mp3"));
        menu = LoadMusicStream(Path.Combine(audio, "bgm.mp3"));
        stagemusic = LoadMusicStream(Path.Combine(audio, "stage.mp3"));
        credits = LoadMusicStream(Path.Combine(audio, "credits.mp3"));
        brick = LoadSound(Path.Combine(audio, "brick.mp3"));
        boing = LoadSound(Path.Combine(audio, "boing.mp3"));
        rtex = LoadRenderTexture((int)Width, (int)Width);
        font = LoadFontEx(Path.Combine(folder, Resource1.font), 115, null, 0);
        opensans = LoadFontEx(Path.Combine(folder, Resource1.opensans), 55, null, 0);
        GenTextureMipmaps(ref font.texture);
        SetTextureFilter(font.texture, TextureFilter.TEXTURE_FILTER_BILINEAR);
        GenTextureMipmaps(ref opensans.texture);
        SetTextureFilter(opensans.texture, TextureFilter.TEXTURE_FILTER_BILINEAR);
        gamestate = GameState.logo;

        platformimages = new[] { platform, platform, platformbroken, cloud, trampoline };

        var bgcolor = new Color(145, 213, 224, 255);
        PlayMusicStream(stagemusic);
        PlayMusicStream(menu);
        while (!WindowShouldClose())
        {
            BeginDrawing();
            ClearBackground(bgcolor);
            switch (gamestate)
            {
                case GameState.logo:
                    Logo();
                    break;
                case GameState.menu:
                    Start();
                    break;
                case GameState.stage:
                    Stage();
                    break;
                case GameState.end:
                    End();
                    break;
                case GameState.credits:
                    Credits();
                    break;
            }
            if (IsKeyPressed(KeyboardKey.KEY_ENTER))
            {
                if (IsWindowFullscreen())
                {
                    ToggleFullscreen();
                    SetWindowSize((int)windowWidth, (int)windowHeight);
                    Width = windowWidth;
                    Height = windowHeight;
                }
                else
                {
                    var monitor = GetCurrentMonitor();
                    var x = GetMonitorWidth(monitor);
                    var y = GetMonitorHeight(monitor);
                    Width = x;
                    Height = y;
                    SetWindowSize(x, y);
                    ToggleFullscreen();
                }
            }
            EndDrawing();
        }
        CloseAudioDevice();
        CloseWindow();
        UnloadImage(icon);
        foreach (var texture in textures)
        
            UnloadTexture(texture);
        UnloadMusicStream(stagemusic);
        UnloadMusicStream(menu);
        UnloadMusicStream(credits);
        UnloadSound(ending);
        UnloadSound(logo);
        UnloadSound(jump);
        UnloadSound(brick);
        UnloadSound(boing);
        UnloadRenderTexture(rtex);
        UnloadFont(font);
        UnloadFont(opensans);
    }

    private static Texture2D LoadTexture(string images,string path)
    {
        var texture = Raylib.LoadTexture(Path.Combine(images, path));
        GenTextureMipmaps(ref texture);
        SetTextureFilter(texture, TextureFilter.TEXTURE_FILTER_TRILINEAR);
        textures.Add(texture);
        return texture;
    }

    private static void Logo()
    {
        if (360 < timer)
            gamestate = GameState.menu;

        if (timer == 0)
            PlaySound(logo);

        ClearBackground(Color.WHITE);

        timer++;
        var fract = Smoothstep(MathF.Min(1f, MathF.Max(0f, timer / 120f)));
        var fract2 = Smoothstep(MathF.Min(1f, MathF.Max(0f, (timer - 120) / 120f)));
        var fade = Smoothstep(MathF.Min(1f, MathF.Max(0f, (360f - timer) / 120)));

        var scale = Width / origWidth;

        var lwidth = logo1.width * scale;
        var lheight = logo1.height * scale;

        var color = new Color(255, 255, 255, (int)(255 * fract * fade));
        var color2 = new Color(203, 206, 249, (int)(255 * fract2 * fade));

        DrawTexturePro(logo1,new Rectangle(0,0, logo1.width, logo1.height),new Rectangle(Width / 2f - lwidth / 2f, Height / 2f - lheight / 2f,lwidth,lheight),Vector2.Zero,0, color);
        DrawTextEx(opensans, Resource1.Presents, new Vector2((int)(Width / 2 - MeasureTextEx(font, Resource1.Presents, 55 * scale, 0).X / 2), (int)(Height * 0.70f)), 55 *scale, 0, color2);
    }
    private static void Start()
    {
        var scale = Width / origWidth;

        if (IsKeyPressed(KeyboardKey.KEY_SPACE))
        {
            gamestate = GameState.stage;
        }
        if (IsKeyPressed(KeyboardKey.KEY_C))
        {
            timer = 0;
            gamestate = GameState.credits;
        }
        else
        {
            UpdateMusicStream(menu);

            var angle = MathF.Sin(timer / 15f) * 10f;

            DrawTexturePro(menubackground,new Rectangle(0,0,menubackground.width,menubackground.height),new Rectangle(0,0,Width,Height),Vector2.Zero,0,Color.WHITE);

            DrawTexturePro(pekoface,
                new Rectangle(0, 1, pekoface.width, pekoface.height),
                new Rectangle(Width * 0.8f, Height * 1.05f, pekoface.width * scale, pekoface.height * scale),
                new Vector2(pekoface.width * 0.5f * scale, pekoface.height * scale),
                MathF.Sin(timer / 30f) * 10f, Color.WHITE);

            DrawTexturePro(pekoface,
                new Rectangle(0, 1, -pekoface.width, pekoface.height),
                new Rectangle(Width * 0.2f, Height * 1.05f, pekoface.width * scale, pekoface.height * scale),
                new Vector2(pekoface.width * 0.5f * scale, pekoface.height * scale),
                MathF.Sin(timer / 30f) * -10f, Color.WHITE);

            if ((timer / 20) % 4 == 0)
                DrawTexturePro(space,
                new Rectangle(0, 0, space.width, space.height),
                new Rectangle((Width -space.width * scale) * 0.5f, Height * 0.8f, space.width * scale, space.height * scale),
                Vector2.Zero,
                0, Color.WHITE);
            if ((timer / 20) % 4 == 2)
                DrawTexturePro(cForCredits,
                    new Rectangle(0, 0, cForCredits.width, cForCredits.height),
                    new Rectangle((Width - cForCredits.width * scale) * 0.5f, Height * 0.8f, cForCredits.width * scale, cForCredits.height * scale),
                    Vector2.Zero,
                    0, Color.WHITE);
            DrawTexturePro(title,
                new Rectangle(0, 0, title.width, title.height),
                new Rectangle(Width * 0.5f, Height * 0.5f, title.width * 0.8f * scale, title.height * 0.8f * scale),
                new Vector2(title.width * 0.4f * scale, title.height * 0.4f * scale),
                angle, Color.WHITE);

            timer++;
        }

    }
    private static void End()
    {
        var scale = Width / origWidth;
        if (IsKeyPressed(KeyboardKey.KEY_R))
        {
            StopSound(ending);
            gamestate = GameState.stage;
        }
        else if (IsKeyPressed(KeyboardKey.KEY_B))
        {
            StopSound(ending);
            gamestate = GameState.menu;
        }
        else
        {
            DrawTexturePro(gameoverscreen, new Rectangle(0, 0, gameoverscreen.width, gameoverscreen.height), new Rectangle(0, 0, Width, Height), Vector2.Zero, 0, Color.WHITE);
            DrawTextEx(font, Resource1.PressRToRestart, new Vector2((Width * 0.7f), (Height - 165)), 35 * scale, 0, ((timer / 30) % 2 == 0) ? Color.YELLOW : Color.ORANGE);
            DrawTextEx(font, Resource1.PressBToReturnToMenu, new Vector2((Width * 0.7f), (Height - 130)), 35 * scale, 0, ((timer / 30) % 2 == 1) ? Color.YELLOW : Color.ORANGE);
            DrawTextEx(font, string.Format(Resource1.Score, score), new Vector2((Width * 0.7f), (Height - 95)), 35 * scale, 0, Color.YELLOW);
            DrawTextEx(font, string.Format(Resource1.MaxScore, maxscore), new Vector2((Width * 0.7f), (Height - 60)), 35 * scale, 0, Color.YELLOW);

            timer++;
        }
    }

    private static void Credits()
    {
        var scale = Width / origWidth;
        if (timer == 0)
        {
            StopMusicStream(credits);
            PlayMusicStream(credits);
        }
        if (IsKeyPressed(KeyboardKey.KEY_B))
        {
            StopSound(ending);
            gamestate = GameState.menu;
            return;
        }
        UpdateMusicStream(credits);
        DrawTexturePro(menubackground, new Rectangle(0, 0, menubackground.width, menubackground.height), new Rectangle(0, 0, Width, Height), Vector2.Zero, 0, Color.WHITE);
        const int Fontsize = 50;
        const int Spacing = 2;

        var scroll = (int)MeasureTextEx(font, creditText, Fontsize * scale, Spacing).Y * 1.5f;

        var y = Height - (timer % scroll);

        TextAligned.DrawTextAligned(font, creditText, new Vector2(Width * 0.5f, y), Fontsize * scale, Spacing, TextAligned.AlignCenter, new Color(255, 144, 224,255));

        var overlay = Fontsize * 2 * scale;

        if ((timer / 30) % 2 == 0)
            DrawTexturePro(bForMenu,
            new Rectangle(0, 0, bForMenu.width, bForMenu.height),
            new Rectangle((Width - bForMenu.width * scale) * 0.5f, Height - bForMenu.height * scale * 1.1f, bForMenu.width * scale, bForMenu.height * scale),
            Vector2.Zero,
            0, Color.WHITE);

        timer++;
    }


    private static bool started = false;
    private static bool colliding = false;
    private static float pekoX = 0f;
    private static float pekoY = 0f;
    private static float pekoSpeed = 0;
    private static List<Tuple<int, int, int, bool>> platforms;
    private static Texture2D[] platformimages;
    const float pekoScale = 0.6f;
    private static float pekoWith;
    private static float pekoheight;
    private static float pekorientation = 1;
    private static float cameraY;
    private static float multiplicator;

    private static void GeneratePlatform(int y)
    {
        platforms.Add(Tuple.Create(Rnd.Next(25, (int)(origWidth - 25 - platform.width)), y, Rnd.Next(0, 5), true));
    }
    private static void Initialize()
    {
        timer = 0;
        platforms = new List<Tuple<int, int, int, bool>>();
        cameraY = 0;
        pekoWith = pekoScale * peko.width;
        pekoheight = pekoScale * peko.height;
        pekoX = (origWidth - pekoWith) / 2f;
        pekoY = (origHeight - pekoheight);
        colliding = true;
        multiplicator = 2;
        var y = (int)origHeight - 50;
        while (y > 0)
        {
            y -= platform.height + Rnd.Next((int)(platform.height * 0.75f), platform.height);
            GeneratePlatform((int)y);
        }
    }

    private static void DrawRectangleLines(Rectangle rect, Color color)
    {
        Raylib.DrawRectangleLines((int)rect.x, (int)rect.y, (int)rect.width, (int)rect.height, color);
    }


    private static void Stage()
    {
        var scale = Width / origWidth;
        UpdateMusicStream(menu);
        if (!started)
        {
            Initialize();
            started = true;
        }

        const float xspeed = 10f;
        const float startspeed = 20f;
        if (IsKeyDown(KeyboardKey.KEY_LEFT))
        {
            pekorientation = 1;
            pekoX = MathF.Max(-pekoWith *0.5f, pekoX - xspeed);
            
        }
        else if (IsKeyDown(KeyboardKey.KEY_RIGHT))
        {
            pekorientation = -1;
            pekoX = MathF.Min(origWidth - (pekoWith *0.5f), pekoX + xspeed);
        }

        if (colliding)
        {
            PlaySound(jump);
            pekoSpeed = startspeed * multiplicator;
            colliding = false;
        }

        pekoY -= pekoSpeed;
        pekoSpeed -= 0.5f;
        var pekorect = new Rectangle(pekoX + pekoWith / 2 - pekoWith / 8, pekoY + pekoheight / 2f, pekoWith / 4, pekoheight / 2f);

        cameraY = MathF.Min(cameraY, pekoY);
        score = (int)(MathF.Abs(MathF.Min(-timer * 0.1f, cameraY)) * 0.1f);
        maxscore = Math.Max(maxscore, score);

        DrawTexturePro(stars, new Rectangle(-timer * 0.0625f, cameraY * 0.0625f, Width, Height),
            new Rectangle(0, 0, Width, Height),
            Vector2.Zero, 0, Color.WHITE);
        DrawTexturePro(ringStars, new Rectangle(-timer * 0.125f, cameraY * 0.125f, Width, Height),
            new Rectangle(0, 0, Width, Height),
            Vector2.Zero, 0, Color.WHITE);
        DrawTexturePro(dotstars, new Rectangle(-timer * 0.25f, cameraY * 0.25f, Width, Height),
            new Rectangle(0, 0, Width, Height),
            Vector2.Zero, 0, Color.WHITE);
        DrawTexturePro(fallingStars, new Rectangle(-timer * 0.5f, cameraY * 0.5f, Width, Height),
            new Rectangle(0, 0, Width, Height),
            Vector2.Zero, 0, Color.WHITE);

        if (pekoY > cameraY + origHeight)
        {
            started = false;
            gamestate = GameState.end;
            PlaySound(ending);
        }

        while (platforms[^1].Item2 > cameraY)
        {
            GeneratePlatform(platforms[^1].Item2 - (platform.height +
                             Rnd.Next((int)(platform.height * 0.75f), platform.height)));
        }

        for (var i = 0; i < platforms.Count; i++)
        {
            var p = platforms[i];
            if (!p.Item4)
                continue;
            var platformrect = new Rectangle(p.Item1, p.Item2, platform.width, 10);
            if (!colliding && pekoSpeed < 0)
            {
                if (CheckCollisionRecs(pekorect, platformrect) && p.Item2 > pekoY)
                {
                    colliding = true;
                    multiplicator = 1;
                    switch (p.Item3)
                    {
                        case 4:
                            multiplicator = 3;
                            PlaySound(boing);
                            break;
                        case 2:
                            platforms[i] = Tuple.Create(p.Item1, p.Item2, p.Item3, false);
                            PlaySound(brick);
                            break;
                    }
                }
            }

            //DrawTexture(platformimages[p.Item3], p.Item1, p.Item2 - (int)cameraY - (int)(p.Item3 == 4 ? platform.height * 1.5f : 0), Color.WHITE);

            var platformImage = platformimages[p.Item3];
            DrawTexturePro(platformImage,
                new Rectangle(1, 1, platformImage.width -2, platformImage.height -2),
                new Rectangle(p.Item1 * scale, (p.Item2 - cameraY - (p.Item3 == 4 ? platform.height * 1.5f : 0)) * scale, platformImage.width * scale, platformImage.height * scale),
                Vector2.Zero,
                0, Color.WHITE);

        }

        DrawTexturePro(peko, new Rectangle(0, 0, peko.width * pekorientation, peko.height),
            new Rectangle(pekoX * scale, (pekoY - cameraY) * scale, pekoWith * scale, pekoheight * scale),
            Vector2.Zero, 0, Color.WHITE);

        //DrawText(pekoY.ToString(CultureInfo.InvariantCulture), 10, 10, 20, Color.WHITE);
        //DrawText(cameraY.ToString(CultureInfo.InvariantCulture), 10, 30, 20, Color.WHITE);


        timer++;
    }
}