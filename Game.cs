using SFML.Audio;
using SFML.Graphics;
using SFML.System;
using SFML.Window;
using System.Text;

namespace Arkanoid
{
    public static class Game
    {
        //окно:
        private static RenderWindow _mainWindow = null!;
        //текстуры:
        private static readonly Dictionary<string, Texture> _objectsTextures = new()
        {
            { "Background1", new Texture("Backgrounds.png", new(0, 0, 1002, 752)) },
            { "Background2", new Texture("Backgrounds.png", new(1000, 0, 2002, 752)) },
            { "Background3", new Texture("Backgrounds.png", new(0, 750, 1002, 1502)) },
            { "Background4", new Texture("Backgrounds.png", new(1000, 750, 2002, 1502)) },
            { "Board", new Texture("Objects.png", new(0, 0, 784, 754)) },
            { "Visor", new Texture("Objects.png", new(786, 0, 700, 49)) },
            { "Block", new Texture("Objects.png", new(804, 96, 33, 20)) },
            { "BlockBehind", new Texture("Objects.png", new(804, 120, 33, 20)) },
            { "BlockOpen", new Texture("Objects.png", new(804, 144, 33, 33)) },
            { "Platform", new Texture("Objects.png", new(900, 96, 93, 18)) },
            { "PowerUp1", new Texture("Objects.png", new(810, 180, 21, 15)) },
            { "PowerUp2", new Texture("Objects.png", new(810, 198, 21, 15)) },
            { "PowerUp3", new Texture("Objects.png", new(810, 216, 21, 15)) },
            { "PowerUp4", new Texture("Objects.png", new(810, 234, 21, 15)) },
            { "PowerUp5", new Texture("Objects.png", new(810, 252, 21, 15)) },
            { "SelectionFrame", new Texture("Objects.png", new(840, 96, 57, 57)) },
            { "PowerUpIcon0", new Texture("Objects.png", new(843, 159, 45, 45)) },
            { "PowerUpIcon1", new Texture("Objects.png", new(843, 213, 45, 45)) },
            { "PowerUpIcon2", new Texture("Objects.png", new(843, 267, 45, 45)) },
            { "PowerUpIcon3", new Texture("Objects.png", new(843, 321, 45, 45)) },
            { "Background1Preview", new Texture("Objects.png", new(900, 117, 33, 33)) },
            { "Background2Preview", new Texture("Objects.png", new(936, 117, 33, 33)) },
            { "Background3Preview", new Texture("Objects.png", new(900, 153, 33, 33)) },
            { "Background4Preview", new Texture("Objects.png", new(936, 153, 33, 33)) },
            { "TargetMark", new Texture("Objects.png", new(807, 270, 27, 27)) },
            { "WinSign", new Texture("Objects.png", new(972, 117, 231, 117)) },
            { "LoseSign", new Texture("Objects.png", new(972, 237, 231, 117)) },
            { "InfoSign", new Texture("Objects.png", new(1206, 117, 231, 351)) },
        };
        private static readonly Dictionary<int, Texture> _colorsTextures = new();
        private static readonly Texture[] _numbersTextures = new Texture[10];
        private static readonly List<Texture> _ballTextures = new();
        //спрайты:
        private static Sprite _background = new(_objectsTextures["Background2"]);
        private static readonly Sprite[] _backgroundButtons = new[]
        {
            new Sprite(_objectsTextures["Background1Preview"]),
            new Sprite(_objectsTextures["Background2Preview"]),
            new Sprite(_objectsTextures["Background3Preview"]),
            new Sprite(_objectsTextures["Background4Preview"]),
        };
        private static readonly Sprite _board = new(_objectsTextures["Board"]);
        private static readonly Sprite _visor = new(_objectsTextures["Visor"]);
        private static readonly Sprite _powerUpFrame = new Sprite(_objectsTextures["SelectionFrame"]);
        private static Sprite[] _scoreSprites = new Sprite[4];
        private static Sprite[] _score2Sprites = new Sprite[4];
        private static Sprite[] _scoreMaxSprites = new Sprite[4];
        private static Sprite[] _scoreWinMinSprites = new Sprite[4];
        private static Sprite[] _scoreWinMaxSprites = new Sprite[4];
        private static Sprite _lifeCounter = new Sprite();
        private static readonly Sprite _targetMark = new Sprite(_objectsTextures["TargetMark"]);
        private static readonly Sprite _winSign = new Sprite(_objectsTextures["WinSign"]);
        private static readonly Sprite _loseSign = new Sprite(_objectsTextures["LoseSign"]);
        private static readonly Sprite _infoSign = new Sprite(_objectsTextures["InfoSign"]);
        //звук:
        private static readonly SoundBuffer _sound1Buffer= new SoundBuffer("Sound1.wav");
        private static readonly Sound _sound1 = new Sound(_sound1Buffer);
        private static readonly SoundBuffer _sound2Buffer = new SoundBuffer("Sound2.wav");
        private static readonly Sound _sound2 = new Sound(_sound2Buffer);
        private static readonly SoundBuffer _soundWinBuffer = new SoundBuffer("WinSound.wav");
        private static readonly Sound _soundWin = new Sound(_soundWinBuffer);
        private static readonly SoundBuffer _soundLoseBuffer = new SoundBuffer("LoseSound.wav");
        private static readonly Sound _soundLose = new Sound(_soundLoseBuffer);
        private static readonly SoundBuffer _soundMiniLoseBuffer = new SoundBuffer("MiniLose.wav");
        private static readonly Sound _soundMiniLose = new Sound(_soundMiniLoseBuffer);
        private static readonly SoundBuffer _soundExplosionBuffer = new SoundBuffer("Explosion.wav");
        private static readonly Sound _soundExplosion = new Sound(_soundExplosionBuffer);
        //игровые данные:
        private static readonly IntRect _playZone = new IntRect(30, 15, 617, 630);
        private static readonly IntRect[] _buttonsArea =
        {
            new(950, 30, 33, 33),
            new(950, 66, 33, 33),
            new(950, 102, 33, 33),
            new(950, 138, 33, 33)
        };
        private static bool _pause = true;
        private static bool _gameStartMode = true;
        private static int _score = 0;
        private static int _scoreMax = -1;
        private static int _scoreWinMin = -1;
        private static int _scoreWinMax = -1;
        private static int _life = 3;
        private static bool[] _powerUps = { false, false, false, true };
        private static int _currentPowerUp = 0;
        private static List<Block> _blocks = new();
        private static List<Ball> _balls = new();
        private static bool _redirectionMode = false;
        private static bool _lose = false;
        private static bool _win = false;
        private static float _might = 0;
        //рандом:
        private static readonly Random _random = new Random();

        //основные методы игры:
        public static void Main()
        {
            //настройки окна:
            _mainWindow = new RenderWindow(new VideoMode(1000, 750), "Arkanoid");
            _mainWindow.Closed += Window_Closed;
            _mainWindow.Resized += Window_Resized;
            _mainWindow.SetFramerateLimit(60);
            //заполнение текстур и спрайтов:
            for (int i = 1; i < 85; i++)
                _colorsTextures.Add(i, new Texture("Objects.png", new(786, 96 + i * 3, 15, 3)));
            for (int i = 0; i < 10; i++)
                _numbersTextures[i] = new Texture("Objects.png", new(786 + i * 15, 51, 12, 24));
            for (int i = 0; i < 16; i++)
                _ballTextures.Add(new Texture("Objects.png", new(786 + i * 18, 78, 15, 15)));
            _scoreSprites = new Sprite[] 
            {
                new Sprite(_numbersTextures[0]),
                new Sprite(_numbersTextures[0]),
                new Sprite(_numbersTextures[0]),
                new Sprite(_numbersTextures[0])
            };
            _score2Sprites = new Sprite[]
            {
                new Sprite(_numbersTextures[0]),
                new Sprite(_numbersTextures[0]),
                new Sprite(_numbersTextures[0]),
                new Sprite(_numbersTextures[0])
            };
            _scoreMaxSprites = new Sprite[]
            {
                new Sprite(_numbersTextures[0]),
                new Sprite(_numbersTextures[0]),
                new Sprite(_numbersTextures[0]),
                new Sprite(_numbersTextures[0])
            };
            _scoreWinMinSprites = new Sprite[]
            {
                new Sprite(_numbersTextures[0]),
                new Sprite(_numbersTextures[0]),
                new Sprite(_numbersTextures[0]),
                new Sprite(_numbersTextures[0])
            };
            _scoreWinMaxSprites = new Sprite[]
            {
                new Sprite(_numbersTextures[0]),
                new Sprite(_numbersTextures[0]),
                new Sprite(_numbersTextures[0]),
                new Sprite(_numbersTextures[0])
            };
            _lifeCounter = new Sprite(_numbersTextures[0]);
            _lifeCounter.Position = new(750, 465);
            //установка вещей на их место:
            for (int i = 0; i < 4; i++)
                _backgroundButtons[i].Position = new(_buttonsArea[i].Left, _buttonsArea[i].Top);
            _infoSign.Position = new(231, 231);
            _loseSign.Position = new(231, 135);
            _winSign.Position = new(231, 135);
            Platform.ChangePosition(347);
            //привязка логики к событиям:
            _mainWindow.MouseMoved += MouseMoved;
            _mainWindow.KeyPressed += KeyPressed;
            _mainWindow.MouseButtonPressed += MouseButtonPressed;
            _mainWindow.MouseWheelScrolled += MouseWheelScrolled;
            //заполнение массива блоков:
            for (int i = 0; i < 231; i++)
            {
                _blocks.Add(new Block(new(45 + (i % 21 * 30), 192 + (i / 21 * 33)), 21 - (i / 21 * 2), true, 21 - (i / 21 * 2)));
            }
            //создание первого шара:
            _balls.Add(new Ball(new(318, 618), 1.5f, new(0, -1), _playZone));
            //обработка файла со счетом.
            if (_scoreMax == -1)
            {
                FileStream file = new FileStream("Score.txt", FileMode.Open);
                byte[] output = new byte[17];
                string outputString;
                file.ReadAsync(output, 0, 17);
                outputString = Encoding.Default.GetString(output);
                string[] RawScore = outputString.Split(";");
                if (!int.TryParse(RawScore[0], out _scoreMax))
                    _scoreMax = -1;
                if (!int.TryParse(RawScore[1], out _scoreWinMin))
                    _scoreWinMin = -1;
                if (!int.TryParse(RawScore[2], out _scoreWinMax))
                    _scoreWinMax = -1;
                file.Close();
            }
            //главный цикл:
            while (_mainWindow.IsOpen)
            {
                _mainWindow.Clear();

                _mainWindow.DispatchEvents();

                GameLogic();

                GameDisplay();

                _mainWindow.Display();
            }
        }

        private static void GameLogic()
        {
            if (_lose || _win)
                return;
            if (_balls.Count == 0)
            {
                _gameStartMode = true;
                if (_life > 0)
                {
                    _life -= 1;
                    _balls.Add(new Ball(new(318, 618), 1.5f, new(-0.2f, -1), _playZone));
                } 
                else
                {
                    _soundLose.Play();
                    if (_score > _scoreMax)
                        _scoreMax = _score;
                    if (_scoreWinMax > _scoreMax)
                        _scoreMax = _scoreWinMax;
                    _lose = true;
                    return;
                }
            }
            if (!_gameStartMode && !_pause)
            {
                for (int i = 0; i < _balls.Count; i++)
                {
                    _balls[i].Move();
                }
            }
            else
            {
                _balls[0].WaitingToStart();
            }
        }
        private static void GameDisplay()
        {
            //фон:
            _mainWindow.Draw(_background);
            //доска:
            _mainWindow.Draw(_board);
            //интерфейс:
            _powerUpFrame.Position = new(714, 219 + _currentPowerUp * 60);
            _mainWindow.Draw(_powerUpFrame);
            for (int i = 0; i < 4; i++)
            {
                if (_powerUps[i])
                {
                    Sprite icon = new Sprite(_objectsTextures["PowerUpIcon" + i]);
                    icon.Position = new(720, 225 + i * 60);
                    _mainWindow.Draw(icon);
                }
            }
            for (int i = 0; i < 4; i++)
            {
                _mainWindow.Draw(_backgroundButtons[i]);
            }
            if (_score < 9)
            {
                _scoreSprites[0].Texture = _numbersTextures[_score];
                _scoreSprites[0].Position = new(732, 108);
                _mainWindow.Draw(_scoreSprites[0]);
            }
            else if (_score < 99)
            {
                _scoreSprites[0].Texture = _numbersTextures[_score%10];
                _scoreSprites[1].Texture = _numbersTextures[_score/10];
                _scoreSprites[0].Position = new(738, 108);
                _scoreSprites[1].Position = new(723, 108);
                _mainWindow.Draw(_scoreSprites[0]);
                _mainWindow.Draw(_scoreSprites[1]);
            }
            else if (_score < 999)
            {
                _scoreSprites[0].Texture = _numbersTextures[_score % 10];
                _scoreSprites[1].Texture = _numbersTextures[_score / 10 % 10];
                _scoreSprites[2].Texture = _numbersTextures[_score / 100];
                _scoreSprites[0].Position = new(747, 108);
                _scoreSprites[1].Position = new(732, 108);
                _scoreSprites[2].Position = new(717, 108);
                _mainWindow.Draw(_scoreSprites[0]);
                _mainWindow.Draw(_scoreSprites[1]);
                _mainWindow.Draw(_scoreSprites[2]);
            }
            else
            {
                _scoreSprites[0].Texture = _numbersTextures[_score % 10];
                _scoreSprites[1].Texture = _numbersTextures[_score / 10 % 10];
                _scoreSprites[2].Texture = _numbersTextures[_score / 100 % 10];
                _scoreSprites[3].Texture = _numbersTextures[_score / 1000];
                _scoreSprites[0].Position = new(756, 108);
                _scoreSprites[1].Position = new(741, 108);
                _scoreSprites[2].Position = new(726, 108);
                _scoreSprites[3].Position = new(711, 108);
                _mainWindow.Draw(_scoreSprites[0]);
                _mainWindow.Draw(_scoreSprites[1]);
                _mainWindow.Draw(_scoreSprites[2]);
                _mainWindow.Draw(_scoreSprites[3]);
            }
            _lifeCounter.Texture = _numbersTextures[_life];
            _mainWindow.Draw(_lifeCounter);
            //разрушаемые блоки:
            foreach (Block block in _blocks)
            {
                if (!block.BlockActive)
                    block.Draw();
            }
            foreach (Block block in _blocks)
            {
                if (block.BlockActive)
                    block.Draw();
            }
            //управляемая платформа:
            Platform.Draw();
            //мячик:
            foreach (Ball ball in _balls)
                ball.Draw();
            //навес:
            _mainWindow.Draw(_visor);
            //цель для перенаправления
            if (_redirectionMode)
                _mainWindow.Draw(_targetMark);
            //таблички
            if (_pause)
                _mainWindow.Draw(_infoSign);
            if (_win)
            {
                _mainWindow.Draw(_winSign);

                _score2Sprites[0].Texture = _numbersTextures[_score % 10];
                _score2Sprites[1].Texture = _numbersTextures[_score / 10 % 10];
                _score2Sprites[2].Texture = _numbersTextures[_score / 100 % 10];
                _score2Sprites[3].Texture = _numbersTextures[_score / 1000];
                _score2Sprites[3].Position = new(246, 192);
                _score2Sprites[2].Position = new(261, 192);
                _score2Sprites[1].Position = new(276, 192);
                _score2Sprites[0].Position = new(291, 192);
                _mainWindow.Draw(_score2Sprites[0]);
                _mainWindow.Draw(_score2Sprites[1]);
                _mainWindow.Draw(_score2Sprites[2]);
                _mainWindow.Draw(_score2Sprites[3]);

                _scoreWinMaxSprites[0].Texture = _numbersTextures[_scoreWinMax % 10];
                _scoreWinMaxSprites[1].Texture = _numbersTextures[_scoreWinMax / 10 % 10];
                _scoreWinMaxSprites[2].Texture = _numbersTextures[_scoreWinMax / 100 % 10];
                _scoreWinMaxSprites[3].Texture = _numbersTextures[_scoreWinMax / 1000];
                _scoreWinMaxSprites[3].Position = new(318, 192);
                _scoreWinMaxSprites[2].Position = new(333, 192);
                _scoreWinMaxSprites[1].Position = new(348, 192);
                _scoreWinMaxSprites[0].Position = new(363, 192);
                _mainWindow.Draw(_scoreWinMaxSprites[0]);
                _mainWindow.Draw(_scoreWinMaxSprites[1]);
                _mainWindow.Draw(_scoreWinMaxSprites[2]);
                _mainWindow.Draw(_scoreWinMaxSprites[3]);

                _scoreWinMinSprites[0].Texture = _numbersTextures[_scoreWinMin % 10];
                _scoreWinMinSprites[1].Texture = _numbersTextures[_scoreWinMin / 10 % 10];
                _scoreWinMinSprites[2].Texture = _numbersTextures[_scoreWinMin / 100 % 10];
                _scoreWinMinSprites[3].Texture = _numbersTextures[_scoreWinMin / 1000];
                _scoreWinMinSprites[3].Position = new(390, 192);
                _scoreWinMinSprites[2].Position = new(405, 192);
                _scoreWinMinSprites[1].Position = new(420, 192);
                _scoreWinMinSprites[0].Position = new(435, 192);
                _mainWindow.Draw(_scoreWinMinSprites[0]);
                _mainWindow.Draw(_scoreWinMinSprites[1]);
                _mainWindow.Draw(_scoreWinMinSprites[2]);
                _mainWindow.Draw(_scoreWinMinSprites[3]);
            }
            if (_lose)
            {
                _mainWindow.Draw(_loseSign); 

                _score2Sprites[0].Texture = _numbersTextures[_score % 10];
                _score2Sprites[1].Texture = _numbersTextures[_score / 10 % 10];
                _score2Sprites[2].Texture = _numbersTextures[_score / 100 % 10];
                _score2Sprites[3].Texture = _numbersTextures[_score / 1000];
                _score2Sprites[3].Position = new(246, 192);
                _score2Sprites[2].Position = new(261, 192);
                _score2Sprites[1].Position = new(276, 192);
                _score2Sprites[0].Position = new(291, 192);
                _mainWindow.Draw(_score2Sprites[0]);
                _mainWindow.Draw(_score2Sprites[1]);
                _mainWindow.Draw(_score2Sprites[2]);
                _mainWindow.Draw(_score2Sprites[3]);

                _scoreMaxSprites[0].Texture = _numbersTextures[_scoreMax % 10];
                _scoreMaxSprites[1].Texture = _numbersTextures[_scoreMax / 10 % 10];
                _scoreMaxSprites[2].Texture = _numbersTextures[_scoreMax / 100 % 10];
                _scoreMaxSprites[3].Texture = _numbersTextures[_scoreMax / 1000];
                _scoreMaxSprites[3].Position = new(390, 192);
                _scoreMaxSprites[2].Position = new(405, 192);
                _scoreMaxSprites[1].Position = new(420, 192);
                _scoreMaxSprites[0].Position = new(435, 192);
                _mainWindow.Draw(_scoreMaxSprites[0]);
                _mainWindow.Draw(_scoreMaxSprites[1]);
                _mainWindow.Draw(_scoreMaxSprites[2]);
                _mainWindow.Draw(_scoreMaxSprites[3]);
            }
                
        }

        //классы:
        private class Ball
        {
            private readonly Sprite _sprite;
            public Vector2f Position { private set; get; }
            public float Speed { private set; get; }
            private Vector2f _direction;
            private IntRect Area;
            private float _textureIndex = 0;
            private FloatRect _previousOverlap = default;
            private bool _leader;
            public Ball(Vector2f position, float speed, Vector2f direction, IntRect area)
            {
                _leader = _balls.Count == 0;
                _sprite = new Sprite(_ballTextures[_leader ? 0 : 8]);
                Position = position;
                Speed = speed;
                _direction = direction;
                Area = area;
            }
            public void Move()
            {
                if (_redirectionMode)
                    return;
                float length = (float)Math.Sqrt(_direction.X * _direction.X + _direction.Y * _direction.Y);
                _direction /= length;
                Position += _direction * Speed;
                _sprite.Position = Position;

                //проверка выигрыша
                if (Position.Y < 100)
                {
                    _soundWin.Play();
                    _win = true;
                    if (_score > _scoreWinMax)
                        _scoreWinMax = _score;
                    if (_score < _scoreWinMin || _scoreWinMin == -1)
                        _scoreWinMin = _score;
                    return;
                }
                //проверка столкновений
                if ((Position.X < Area.Left && _previousOverlap.Top != 0.1f || Position.X > Area.Left + Area.Width && _previousOverlap.Top != 0.2f))
                {
                    _sound1.Play();
                    _direction.X *= -1;
                    _previousOverlap = default;
                    if (Position.X < Area.Left)
                        _previousOverlap.Top = 0.1f;
                    else
                        _previousOverlap.Top = 0.2f;

                }
                if (Position.Y > Area.Top + Area.Height)
                {
                    _soundMiniLose.Play();
                    _balls.Remove(this);
                    if (_balls.Count > 0)
                        _balls[0].ChangeLeader();
                    return;
                }
                FloatRect overlap;
                FloatRect allOverlap = default;
                int scoreDelta = 0;
                List<Block> UsedBlocks = new List<Block>();
                foreach (Block block in _blocks)
                    if (block.BlockActive)
                    {
                        if (CheckCollision(block.BlockSprite, out overlap))
                        {
                            _sound1.Play();
                            scoreDelta += 1;
                            block.ChangeHp(-(int)(Speed+_might), this);
                            if (allOverlap == default)
                                allOverlap = overlap;
                            else
                            {
                                if (allOverlap.Intersects(overlap))
                                {
                                    float left = overlap.Left < allOverlap.Left ? overlap.Left : allOverlap.Left;
                                    float top = overlap.Top < allOverlap.Top ? overlap.Top : allOverlap.Top;
                                    float widght = overlap.Width + overlap.Left > allOverlap.Width + allOverlap.Left ? overlap.Width : allOverlap.Width;
                                    float height = overlap.Height + overlap.Top > allOverlap.Height + allOverlap.Top ? overlap.Height : allOverlap.Height;
                                    
                                    allOverlap = new FloatRect(left, top, widght, height);
                                }
                            }
                            UsedBlocks.Add(block);
                        }
                    }
                if (scoreDelta != 0)
                {
                    foreach (Block block in _blocks)
                    {
                        if (!UsedBlocks.Contains(block) && !block.BlockActive)
                        {
                            block.ChangeHp(scoreDelta, this);
                        }
                    }
                }
                _score += scoreDelta;
                if (allOverlap != default && !allOverlap.Intersects(_previousOverlap))
                {
                    if (allOverlap.Height <= allOverlap.Width)
                        _direction.Y *= -1;
                    if (allOverlap.Width <= allOverlap.Height)
                        _direction.X *= -1;
                    _previousOverlap = allOverlap;
                }
                if (CheckCollision(Platform.Sprite, out overlap) && !overlap.Intersects(_previousOverlap))
                {
                    _sound2.Play();
                    Vector2f vector = new Vector2f(Position.X + 7 - Platform.Position + 1, Position.Y + 7 - 645 + 1);
                    float vectorL = (float)Math.Sqrt(vector.X * vector.X + vector.Y * vector.Y);
                    _direction = new(vector.X / vectorL, vector.Y / vectorL);
                    _previousOverlap = overlap;
                }
                

                //перерисовка спрайта.
                _textureIndex += Speed / 5;
                if ((int)_textureIndex > 7)
                    _textureIndex = 0;
                _sprite.Texture = _ballTextures[(int)_textureIndex + (_leader ? 0 : 8)];
            }
            public void WaitingToStart()
            {
                if (_gameStartMode)
                    Position = new(Platform.Position - 8, 630);
            }
            public void Draw()
            {
                _sprite.Position = Position;
                _mainWindow.Draw(_sprite);
            }
            public bool CheckCollision(Sprite sprite, out FloatRect overlap)
            {
                if (_sprite.GetGlobalBounds().Intersects(sprite.GetGlobalBounds(), out overlap))
                {
                    return true;
                }
                return false;
            }
            public void ChangeSpeed(float mod = 1)
            {
                if (Speed < 15)
                {
                    Speed += mod;
                }
                if (Speed > 15)
                {
                    Speed = 15;
                }
            }
            public void ChangeLeader()
            {
                _leader = true;
            }
            public void Redirect()
            {
                Vector2f markPosition = new(_targetMark.Position.X + 12, _targetMark.Position.Y + 12);
                float x = markPosition.X - Position.X;
                float y = markPosition.Y - Position.Y;
                float length = (float)Math.Sqrt(x * x + y * y);
                y += y == 0 ? 0.2f : 0;
                _direction = new(x / length, y / length);
            }
        }
        private class Block
        {
            private Vector2f _position;
            public Sprite BlockSprite { get; private set; }
            private Sprite _hpSprite;
            private Sprite _maxHpSprite;
            public bool BlockOpen { private set; get; } = false;
            public bool BlockActive { private set; get; } = true;
            private int _maxHp;
            private int _hp;
            private int _cooldown = 20;
            public int PowerUpIndex { get; private set; } = 0;
            private Sprite _powerUpSprite = null!;
            public Block(Vector2f position, int maxhp, bool active = true, int hp = 1)
            {
                _maxHp = maxhp;
                _hp = hp;
                BlockActive = active;
                BlockSprite = new Sprite(_objectsTextures["Block" + (BlockActive ? "" : "Behind")]);
                _position = position;
                BlockSprite.Position = new Vector2f(_position.X - 15, _position.Y - 9);
                _hpSprite = new Sprite(_colorsTextures[_hp]);
                _hpSprite.Position = new Vector2f(_position.X - 6, _position.Y - 3);
                _maxHpSprite = new Sprite(_colorsTextures[_maxHp]);
                _maxHpSprite.Position = new Vector2f(_position.X - 6, _position.Y + 3);
            }
            public void Draw()
            {
                _hpSprite.Texture = _colorsTextures[_hp];
                _maxHpSprite.Texture = _colorsTextures[_maxHp];
                _mainWindow.Draw(_hpSprite);
                _mainWindow.Draw(_maxHpSprite);
                _mainWindow.Draw(BlockSprite);
                if (BlockOpen)
                {
                    _mainWindow.Draw(_powerUpSprite);
                }
            }
            public void OpenSwitch()
            {
                if (!BlockOpen)
                {
                    BlockSprite = new Sprite(_objectsTextures["BlockOpen"]);
                    BlockSprite.Position = new(_position.X - 15, _position.Y - 15);
                    _hpSprite.Position = new(_position.X - 6, _position.Y - 9);
                    _maxHpSprite.Position = new(_position.X - 6, _position.Y + 9);
                }
                else
                {
                    BlockSprite = new Sprite(_objectsTextures["Block"]);
                    BlockSprite.Position = new Vector2f(_position.X - 15, _position.Y - 9);
                    _hpSprite.Position = new Vector2f(_position.X - 6, _position.Y - 3);
                    _maxHpSprite.Position = new Vector2f(_position.X - 6, _position.Y + 3);
                }
                BlockOpen = !BlockOpen;
            }
            public void ActiveSwitch()
            {
                if (BlockOpen)
                {
                    OpenSwitch();
                    BlockOpen = true;
                }
                    
                if (BlockActive)
                {
                    BlockSprite.Texture = _objectsTextures["BlockBehind"];
                }
                else
                {
                    BlockSprite.Texture = _objectsTextures["Block"];
                }
                BlockActive = !BlockActive;
            }
            public void ChangeHp(int hpModification, Ball ball)
            {
                if (_hp + hpModification <= 0)
                {
                    _hp = 1;
                    ActiveSwitch();
                    _maxHp = _score/25+1;
                    if (_maxHp >= _colorsTextures.Count)
                        _maxHp = _colorsTextures.Count - 1;
                    if (BlockOpen)
                    {
                        _might += 0.2f;
                        switch (PowerUpIndex)
                        {
                            case 1:
                                if (_powerUps[0])
                                {
                                    _balls.Add(new Ball(new(ball.Position.X, ball.Position.Y), ball.Speed, new(-0.7f, -0.7f), _playZone));
                                    _balls.Add(new Ball(new(ball.Position.X, ball.Position.Y), ball.Speed, new(0, -1), _playZone));
                                    _balls.Add(new Ball(new(ball.Position.X, ball.Position.Y), ball.Speed, new(0.7f, -0.7f), _playZone));
                                }
                                _powerUps[0] = true;
                                break;
                            case 2:
                                if (_powerUps[1])
                                {
                                    _soundMiniLose.Play();
                                }
                                _powerUps[1] = true;
                                break;
                            case 3:
                                if (_powerUps[2])
                                {
                                    _soundExplosion.Play();
                                    for (int i = 0; i < _blocks.Count; i++)
                                    {
                                        if (i >= _blocks.Count)
                                            break;
                                        if (_blocks[i].CheckExplosionHit(ball.Position, 0))
                                        {
                                            _score += 5;
                                            _blocks.Remove(_blocks[i]);
                                            i--;
                                        }
                                    }
                                }
                                _powerUps[2] = true;
                                break;
                            case 4:
                                if (_powerUps[3])
                                {
                                    ball.ChangeSpeed(0.2f);
                                }
                                _powerUps[3] = true;
                                break;
                            case 5:
                                if (_life < 9)
                                    _life++;
                                break;
                        }
                        PowerUpIndex = 0;
                        BlockOpen = false;
                    }
                    return;
                }
                if (_hp + hpModification >= _maxHp)
                {
                    if (_cooldown == 0)
                        _cooldown = 20 + _score/5;
                    else
                    {
                        _cooldown--;
                        return;
                    }
                    ActiveSwitch();
                    float randomValue = _random.NextSingle();
                    if (randomValue > 0.9f)
                    {
                        OpenSwitch();
                        randomValue = _random.NextSingle();
                        if (randomValue > 0.8f)
                            PowerUpIndex = 1;
                        else if (randomValue > 0.6f)
                            PowerUpIndex = 2;
                        else if (randomValue > 0.4f && ball.Speed < 15)
                            PowerUpIndex = 4;
                        else if (randomValue > 0.2f && _life < 9)
                            PowerUpIndex = 5;
                        else
                            PowerUpIndex = 3;
                        _powerUpSprite = new Sprite(_objectsTextures["PowerUp"+PowerUpIndex]);
                        _powerUpSprite.Position = new(_position.X - 9, _position.Y - 6);
                    }
                    return;
                }
                _hp += hpModification;
            }
            public bool CheckExplosionHit(Vector2f bombPosition, float power)
            {
                double distance = Math.Sqrt((_position.X - bombPosition.X) * (_position.X - bombPosition.X) + (_position.Y - bombPosition.Y) * (_position.Y - bombPosition.Y));
                if ( distance <= power)
                {
                    return true;
                }
                if (distance <= power + 20)
                {
                    if (!BlockOpen)
                        ChangeHp(-_hp, _balls[0]);
                    _score += 5;
                    foreach (Block block in _blocks)
                    {
                        if (!block.BlockActive)
                        {
                            if (block == this)
                                continue;
                            block.ChangeHp(5, _balls[0]);
                        }
                    }
                    return false;
                }
                return false;
            }
        }
        private static class Platform
        {
            public static int Position { get; private set; }
            public static bool KeyboardMode = false;
            public static readonly Sprite Sprite = new Sprite(_objectsTextures["Platform"]);
            public static readonly int MaxPosition = 617;
            public static readonly int MinPosition = 77;
            public static void Draw()
            {
                _mainWindow.Draw(Sprite);
            }
            public static void ChangePosition(int x)
            {
                Position = x;
                if (Position > MaxPosition)
                {
                    Position = MaxPosition;
                }
                if (Position < MinPosition)
                {
                    Position = MinPosition;
                }
                Sprite.Position = new(Position - 47, 645);
            }
        }

        //методы событий:
        private static void Window_Resized(object? sender, SizeEventArgs e)
        {
            if (sender == _mainWindow)
                _mainWindow.Size = new Vector2u(1000, 750);
        }
        private static void Window_Closed(object? sender, EventArgs e)
        {
            if (_score > _scoreMax)
                _scoreMax = _score;
            string inputString = (_scoreMax == -1 ? "-NoD-" : new string('0', 5 - _scoreMax.ToString().Length) + _scoreMax)
                + ";" + (_scoreWinMin == -1 ? "-NoD-" : new string('0', 5 - _scoreWinMin.ToString().Length) + _scoreWinMin)
                + ";" + (_scoreWinMax == -1 ? "-NoD-" : new string('0', 5 - _scoreWinMax.ToString().Length) + _scoreWinMax);
            byte[] input = Encoding.Default.GetBytes(inputString);
            FileStream file = new FileStream("Score.txt", FileMode.Truncate);
            file.Write(input);
            file.Close();
            if (sender == _mainWindow)
                _mainWindow.Close();
        }
        private static void MouseMoved(object? sender, MouseMoveEventArgs e)
        {
            if (_pause || _win || _lose)
                return;
            if (_redirectionMode)
            {
                if (_playZone.Contains(e.X, e.Y))
                {
                    _mainWindow.SetMouseCursorVisible(false);
                    _targetMark.Position = new(e.X - 12, e.Y - 12);
                }
                else
                    _mainWindow.SetMouseCursorVisible(true);
                return;
            }
            if (Platform.KeyboardMode && (e.X > Platform.MaxPosition || e.X < Platform.MinPosition))
                return;
            Platform.ChangePosition(e.X);
        }
        private static void KeyPressed(object? sender, KeyEventArgs e)
        {
            switch (e.Code)
            {
                case Keyboard.Key.Left:
                    if (_pause)
                        break;
                    Platform.ChangePosition(Platform.Position - 9);
                    Platform.KeyboardMode = true;
                    break;
                case Keyboard.Key.Right:
                    if (_pause)
                        break;
                    Platform.ChangePosition(Platform.Position + 9);
                    Platform.KeyboardMode = true;
                    break;
                case Keyboard.Key.A:
                    goto case Keyboard.Key.Left;
                case Keyboard.Key.D:
                    goto case Keyboard.Key.Right;
                case Keyboard.Key.Num1:
                    _currentPowerUp = 0;
                    break;
                case Keyboard.Key.Num2:
                    _currentPowerUp = 1;
                    break;
                case Keyboard.Key.Num3:
                    _currentPowerUp = 2;
                    break;
                case Keyboard.Key.Num4:
                    _currentPowerUp = 3;
                    break;
                case Keyboard.Key.Space:
                    if (_win || _lose)
                    {
                        if (_score > _scoreMax)
                            _scoreMax = _score;
                        string inputString = (_scoreMax == -1 ? "-NoD-" : new string('0', 5 - _scoreMax.ToString().Length) + _scoreMax)
                            + ";" + (_scoreWinMin == -1 ? "-NoD-" : new string('0', 5 - _scoreWinMin.ToString().Length) + _scoreWinMin)
                            + ";" + (_scoreWinMax == -1 ? "-NoD-" : new string('0', 5 - _scoreWinMax.ToString().Length) + _scoreWinMax);
                        byte[] input = Encoding.Default.GetBytes(inputString);
                        FileStream file = new FileStream("Score.txt", FileMode.Truncate);
                        file.Write(input);
                        file.Close();
                        _pause = true;
                        _gameStartMode = true;
                        _score = 0;
                        _life = 3;
                        _powerUps = new bool[] { false, false, false, true };
                        _currentPowerUp = 0;
                        List<Block> _blocks = new();
                        List<Ball> _balls = new();
                        _redirectionMode = false;
                        _lose = false;
                        _win = false;
                        _colorsTextures.Clear();
                        _ballTextures.Clear();
                        _mainWindow.Close();
                        Main();
                    }
                    if (_pause)
                    {
                        _pause = false;
                        return;
                    }
                    if (_gameStartMode)
                    {
                        _gameStartMode = false;
                        break;
                    }
                    break;
                case Keyboard.Key.Return:
                    goto case Keyboard.Key.Space;
                case Keyboard.Key.P:
                    if (_gameStartMode && _pause)
                        return;
                    _pause = !_pause;
                    break;
            }
        }
        private static void MouseWheelScrolled(object? sender, MouseWheelScrollEventArgs e)
        {
            if (e.Delta < 0)
            {
                _currentPowerUp = _currentPowerUp == 3 ? 0 : _currentPowerUp + 1;
            }
            else
            {
                _currentPowerUp = _currentPowerUp == 0 ? 3 : _currentPowerUp - 1;
            }
        }
        private static void MouseButtonPressed(object? sender, EventArgs e)
        {
            if (Mouse.IsButtonPressed(Mouse.Button.Left))
            {
                Vector2i mouse = Mouse.GetPosition();
                for (int i = 0; i < 4; i++)
                {
                    if (_buttonsArea[i].Contains(mouse.X - _mainWindow.Position.X - 8, mouse.Y - _mainWindow.Position.Y - 29))
                    {
                        _background.Texture = _objectsTextures["Background" + (i + 1)];
                        return;
                    }
                }
                if (_win || _lose && new IntRect(333, 225, 27, 27).Contains(mouse.X - _mainWindow.Position.X - 8, mouse.Y - _mainWindow.Position.Y - 29))
                {
                    if (_score > _scoreMax)
                        _scoreMax = _score;
                    string inputString = (_scoreMax == -1 ? "-NoD-" : new string('0', 5 - _scoreMax.ToString().Length) + _scoreMax)
                        + ";" + (_scoreWinMin == -1 ? "-NoD-" : new string('0', 5 - _scoreWinMin.ToString().Length) + _scoreWinMin)
                        + ";" + (_scoreWinMax == -1 ? "-NoD-" : new string('0', 5 - _scoreWinMax.ToString().Length) + _scoreWinMax);
                    byte[] input = Encoding.Default.GetBytes(inputString);
                    FileStream file = new FileStream("Score.txt", FileMode.Truncate);
                    file.Write(input);
                    file.Close();
                    _pause = true;
                    _gameStartMode = true;
                    _score = 0;
                    _life = 3;
                    _powerUps = new bool[] { false, false, false, true };
                    _currentPowerUp = 0;
                    _blocks = new List<Block>();
                    _balls = new List<Ball>();
                    _redirectionMode = false;
                    _lose = false;
                    _win = false;
                    _colorsTextures.Clear();
                    _ballTextures.Clear();
                    _mainWindow.Close();
                    Main();
                }
                if (_gameStartMode && _playZone.Contains(mouse.X - _mainWindow.Position.X - 8, mouse.Y - _mainWindow.Position.Y - 29) && !_pause)
                    _gameStartMode = false;
                if (_pause && new IntRect(333, 555, 27, 27).Contains(mouse.X - _mainWindow.Position.X - 8, mouse.Y - _mainWindow.Position.Y - 29))
                    _pause = false;
            }
            if (Mouse.IsButtonPressed(Mouse.Button.Right))
            {
                if (_redirectionMode)
                {
                    _redirectionMode = false;
                    _balls[0].Redirect();
                    _mainWindow.SetMouseCursorVisible(true);
                    return;
                }
                if (_powerUps[_currentPowerUp])
                {
                    switch (_currentPowerUp)
                    {
                        case 0:
                            _balls.Add(new Ball(new(_balls[0].Position.X, _balls[0].Position.Y), _balls[0].Speed, new(-0.7f, -0.7f), _playZone));
                            _balls.Add(new Ball(new(_balls[0].Position.X, _balls[0].Position.Y), _balls[0].Speed, new(0, -1), _playZone));
                            _balls.Add(new Ball(new(_balls[0].Position.X, _balls[0].Position.Y), _balls[0].Speed, new(0.7f, -0.7f), _playZone));
                            _balls.Add(new Ball(new(_balls[0].Position.X, _balls[0].Position.Y), _balls[0].Speed, new(-0.7f, 0.7f), _playZone));
                            _balls.Add(new Ball(new(_balls[0].Position.X, _balls[0].Position.Y), _balls[0].Speed, new(0, 1), _playZone));
                            _balls.Add(new Ball(new(_balls[0].Position.X, _balls[0].Position.Y), _balls[0].Speed, new(0.7f, 0.7f), _playZone));
                            break;
                        case 1:
                            _redirectionMode = true;
                            _mainWindow.SetMouseCursorVisible(false);
                            _targetMark.Position = new(333, 546); 
                            break;
                        case 2:
                            _soundExplosion.Play();
                            for (int i = 0; i < _blocks.Count; i++)
                            {
                                if (i >= _blocks.Count)
                                    break;
                                if (_blocks[i].CheckExplosionHit(_balls[0].Position, 40))
                                {
                                    _score += 5;
                                    _blocks.Remove(_blocks[i]);
                                    i--;
                                }  
                            }
                            break;
                        case 3:
                            foreach (Ball ball in _balls)
                            {
                                ball.ChangeSpeed(0.4f);
                            }
                            break;
                    }
                    _powerUps[_currentPowerUp] = false;
                }
            }
        }
    }
}

