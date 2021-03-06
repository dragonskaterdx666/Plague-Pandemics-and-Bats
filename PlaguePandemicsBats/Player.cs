﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Threading;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Media;

namespace PlaguePandemicsBats
{
    public class Player
    {
        public static event Action SaveScore;

        #region Private variables 
        private const float playerWidth = 0.3f;
        private const float playerGWidth = 0.35f;


        private Game1 _game;
        private Dictionary<Direction, Vector2> _playerDirection;
        private Dictionary<Direction, Sprite []> _spriteDirectionMale;
        private Dictionary<Direction, Sprite []> _spriteDirectionFemale;
        private OBBCollider _playerCollider;
        private Sprite _currentSprite;

        private Direction _direction = Direction.Down;
        private Vector2 _oldPosition;
        private Vector2 _position;
        private Vector2 _lastCheckPointPosition;
        private Vector2 _playerSpawn;
        private SpriteFont _font;
        private SoundEffect _punchSound;
        private SoundEffect _corona;
        private SoundEffect _gameLoss;
        private float _acceleration;
        private float _timer = 0;
        private float _punchTimer = 0.8f;
        private int _frame = 0;
        private int _playerGender;
        private int _health = 100;
        private int _lives = 3;
        private int _ammoCount = 0;
        private int _score = 0;
        private int _lastCheckPointAmmo = 0;
        private int _punchDamage = 15;
        private bool _isPunchAvailable = false;
        private bool _playerHasCat = false;
        #endregion

        #region Public variables
        public string filePath;
        public int _lastCheckPointScore = 0;
        public bool isDragonHiden = true;
        public Vector2 TPpos;
		#endregion

        #region Constructor
        /// <summary>
        /// Player Constructor
        /// </summary>
        /// <param name="game">Game1 Instance</param>
        /// <param name="playerGender">The gender of the character</param>
        public Player(Game1 game)
        {
            _game = game;
            _position = new Vector2(0, 0);
            _lastCheckPointPosition = _position;

            //FONTS
            _font = _game.Content.Load<SpriteFont>("bigfont");

            //SOUNDS
            _punchSound = _game.Content.Load<SoundEffect>("punch");
            _gameLoss = _game.Content.Load<SoundEffect>("endgameSound");
            _corona = _game.Content.Load<SoundEffect>("coronaSpeech");

            filePath = _game.Content.RootDirectory + "/highscore.txt";

            #region Dictionaries
            _playerDirection = new Dictionary<Direction, Vector2>
            {
                [Direction.Up] = Vector2.UnitY,
                [Direction.Down] = -Vector2.UnitY,
                [Direction.Left] = -Vector2.UnitX,
                [Direction.Right] = Vector2.UnitX
            };

            _spriteDirectionMale = new Dictionary<Direction, Sprite []>
            {
                [Direction.Up] = new [] { new Sprite(game, "GuyU0", width: playerWidth), new Sprite(game, "GuyU1", width: playerWidth), new Sprite(game, "GuyU2", width: playerWidth) },
                [Direction.Down] = new [] { new Sprite(game, "GuyD0", width: playerWidth), new Sprite(game, "GuyD1", width: playerWidth), new Sprite(game, "GuyD2", width: playerWidth) },
                [Direction.Left] = new [] { new Sprite(game, "GuyL0", width: playerWidth), new Sprite(game, "GuyL1", width: playerWidth), new Sprite(game, "GuyL2", width: playerWidth) },
                [Direction.Right] = new [] { new Sprite(game, "GuyR0", width: playerWidth), new Sprite(game, "GuyR1", width: playerWidth), new Sprite(game, "GuyR2", width: playerWidth) }
            };

            _spriteDirectionFemale = new Dictionary<Direction, Sprite []>
            {
                [Direction.Up] = new [] { new Sprite(game, "GirlU0", width: playerGWidth), new Sprite(game, "GirlU1", width: playerGWidth), new Sprite(game, "GirlU2", width: playerGWidth) },
                [Direction.Down] = new [] { new Sprite(game, "GirlD0", width: playerGWidth), new Sprite(game, "GirlD1", width: playerGWidth), new Sprite(game, "GirlD2", width: playerGWidth) },
                [Direction.Left] = new [] { new Sprite(game, "GirlL0", width: playerGWidth), new Sprite(game, "GirlL1", width: playerGWidth), new Sprite(game, "GirlL2", width: playerGWidth) },
                [Direction.Right] = new [] { new Sprite(game, "GirlR0", width: playerGWidth), new Sprite(game, "GirlR1", width: playerGWidth), new Sprite(game, "GirlR2", width: playerGWidth) }
            };
            #endregion

            _currentSprite = _spriteDirectionMale [_direction] [_frame];

            _playerCollider = new OBBCollider(game, "Player", _position, _currentSprite.size, rotation: 0);
            _playerCollider.SetDebug(false);
            game.CollisionManager.Add(_playerCollider);
        }
        #endregion

        #region Properties

        /// <summary>
        /// Get's the players name
        /// </summary>
        public string Name { get; set; }

        public int Lives => _lives;

        /// <summary>
        /// Get the Player's Position
        /// </summary>
        public Vector2 Position => _position;

        /// <summary>
        /// Get the Player's Direction;
        /// </summary>
        public Direction Direction => _direction;

        /// <summary>
        /// Get the Player's Current Sprite;
        /// </summary>
        public Sprite CurrentSprite => _currentSprite;

        /// <summary>
        /// Get the Player's Collider
        /// </summary>
        public Collider Collider => _playerCollider;

        public int Health => _health;

        /// <summary>
        /// Gets the Player's Highscore
        /// </summary>
        public int Highscore { get; set; }

        /// <summary>
        /// Gets the Player's current Score
        /// </summary>
        public int Score => _score;

        /// <summary>
        /// Assigning player's ammo
        /// </summary>
        public int AmmoQuantity => _ammoCount;

        #endregion

        #region Methods
        /// <summary>
        /// Checks the if the player is in Collision and take action
        /// </summary>
        /// <param name="gameTime"></param>
        public void LateUpdate(GameTime gameTime)
        {
            if (_playerCollider._inCollision)
            {
                foreach (Collider c in _playerCollider.collisions)
                {
                    if (c.Tag == "Obstacle")
                    {
                        _position = _oldPosition;
                    }

                    if (c.Tag == "RedTree" && !_playerHasCat)
                    {
                        _position = _oldPosition;
                    }

                    if (c.Tag == "BlueHouse")
                    {
                        MediaPlayer.Stop();
                        _position = _oldPosition;
                        _game.hasPlayerTouchedBlueHouse = true;
                        _corona.Play();
                        SetPosition(new Vector2(290, 300));
                                                
                        _lastCheckPointPosition = _position;
                        _lastCheckPointScore = _score;
                        _lastCheckPointAmmo = _ammoCount;
                    }

                    if (c.Tag == "TP")
                    {
                        SetPosition(TPpos);
                        if (_playerHasCat)
                        {
                            _game.Cat.SetPosition(TPpos);
                        }
                    }

                    if (c.Tag == "Cat")
                    {
                        _playerHasCat = true;
                    }

                    if(c.Tag == "Dragon")
                    {
                        isDragonHiden = false;
                    }

                    if (c.Tag == "CheckPoint")
                    {
                        _lastCheckPointPosition = c._position;
                        _lastCheckPointScore = _score;
                        _lastCheckPointAmmo = _ammoCount;

                        _game.CollisionManager.Remove(c);
                    }
                }
            }
        }

        /// <summary>
        /// Updates the player
        /// </summary>
        /// <param name="gameTime"></param>
        public void Update(GameTime gameTime)
        {
            //Delta time and Total time
            float deltaTime = gameTime.DeltaTime();
            float totalTime = gameTime.TotalTime();

            //Timer
            _timer += gameTime.DeltaTime();

            //Check if the punch is available
            if (_punchTimer - _timer <= 0)
            {
                _isPunchAvailable = true;
                _timer = 0;
            }

            //Changes the sprites according to the player gender
            if (_playerGender == 0)
            {
                _currentSprite = _spriteDirectionFemale[_direction][_frame];
            }
            else
            {
                _currentSprite = _spriteDirectionMale[_direction][_frame];
            }

            _oldPosition = _position;

            HandleInput();

            //Change the player position
            _position += _acceleration * deltaTime * _playerDirection[_direction];
            _currentSprite.SetPosition(_position);
            _playerCollider.SetPosition(_position);

            _acceleration = 0;

            //Do the animation when player is moving
            if (_oldPosition != _position)
            {
                _frame = (int)(totalTime * 6) % 3;
                if (_frame > 2)
                    _frame = 1;
            }
            else
                _frame = 0;

            //Check if the player is dead
            if (_health <= 0)
                Die();

            //The Camera will always follow the player
            Camera.LookAt(_position);
        }

        /// <summary>
        /// Update the player Score
        /// </summary>
        /// <param name="x">Score to be added</param>
        public void UpdateScore(int x)
        {
            _score += x;

            if (Score > Highscore)
            {
                Highscore = Score;
            }
        }

        /// <summary>
        /// Adds ammo to the player
        /// </summary>
        /// <param name="ammoQuantity">Ammo to be accrued</param>
        public void AddAmmo(int ammoQuantity)
        {
            _ammoCount += ammoQuantity;
        }

        /// <summary>
        /// Damage nearby enemies
        /// </summary>
        private void Punch()
        {
            foreach (Enemy enemy in _game.Enemies)
            {
                if (Vector2.Distance(enemy._position, _position) <= 0.8)
                {
                    _punchSound.Play();
                    enemy.DamageEnemy(_punchDamage);
                }
            }
        }

        /// <summary>
        /// Handles the user input commands
        /// </summary>
        /// <param name="gameTime"></param>
        public void HandleInput()
        {
            if (KeyboardManager.IsKeyDown(Keys.W) || KeyboardManager.IsKeyDown(Keys.Up))
            {
                _direction = Direction.Up;
                _acceleration = 1.5f;
            }
            if (KeyboardManager.IsKeyDown(Keys.S) || KeyboardManager.IsKeyDown(Keys.Down))
            {
                _direction = Direction.Down;
                _acceleration = 1.5f;
            }
            if (KeyboardManager.IsKeyDown(Keys.A) || KeyboardManager.IsKeyDown(Keys.Left))
            {
                _direction = Direction.Left;
                _acceleration = 1.5f;
            }
            if (KeyboardManager.IsKeyDown(Keys.D) || KeyboardManager.IsKeyDown(Keys.Right))
            {
                _direction = Direction.Right;
                _acceleration = 1.5f;
            }
            if (KeyboardManager.IsKeyDown(Keys.LeftShift))
            {
                _acceleration = 2f;
            }
            if (KeyboardManager.IsKeyGoingDown(Keys.Space) && _ammoCount > 0)
            {
                Projectile proj = new Projectile(_game);
                _game.Projectiles.Add(proj);
                proj.Shoot();
                _ammoCount--;
            }
            if ((KeyboardManager.IsKeyGoingDown(Keys.J) || KeyboardManager.IsKeyGoingDown(Keys.Z)) && _isPunchAvailable)
            {
                Punch();
                _isPunchAvailable = false;
            }
            if (KeyboardManager.IsKeyGoingDown(Keys.E) && !isDragonHiden)
            {
                _game.Dragon.SetPosition(_position - new Vector2(4, 4) * _playerDirection[_game.Player.Direction]);
                _game.Dragon.dragonAttackActive = true;
            }

        }

        /// <summary>
        /// This method allows the manipulation of the player's health
        /// </summary>
        /// <param name="damage">damage done to the player</param>
        public void UpdateHealth(int damage)
        {
            _health -= damage;
        }

        /// <summary>
        /// Initialize Dying sequence for the player
        /// </summary>
        public void Die()
        {
            _lives--;

            SetPosition(_lastCheckPointPosition);

            _score = _lastCheckPointScore;
            _ammoCount = _lastCheckPointAmmo;
            _health = 100;

            if (_lives <= 0)
            {
                _gameLoss.Play();
                SaveScore?.Invoke();
            }
        }

        /// <summary>
        /// This function allows to place the player in a certain position
        /// </summary>
        /// <param name="position">Position to put the player</param>
        public void SetPosition(Vector2 position)
        {
            _position = position;
            _playerCollider.SetPosition(position);
        }

        /// <summary>
        /// Set Spawn of the player when creating him
        /// </summary>
        /// <param name="position"></param>
        public void SetSpawn(Vector2 position)
        {
            _playerSpawn = position;
            _lastCheckPointPosition = _playerSpawn;
            SetPosition(position);
        }

        /// <summary>
        /// Sets the player's gender
        /// </summary>
        /// <param name="playerGender">0 for female, 1 for male</param>
        public void SetGender(int playerGender)
        {
            if (playerGender == 0)
                _playerGender = 0;
            else
                _playerGender = 1;
        }

        /// <summary>
        /// Draws the player's sprites
        /// </summary>
        /// <param name="sb"></param>
        public void Draw(SpriteBatch sb)
        {
            sb.DrawString(_font, $"{Name}", new Vector2(_game.GraphicsDevice.Viewport.Width / 2.07f, _game.GraphicsDevice.Viewport.Height / 2.32f), Color.DarkSlateGray);

            _currentSprite.Draw(sb);
            _playerCollider?.Draw(null);
        }
        #endregion
    }
}
