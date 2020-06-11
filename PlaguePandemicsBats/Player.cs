﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace PlaguePandemicsBats
{
    class Player : DrawableGameComponent
    {
        private Game1 _game;
        private int _playerGender;
        private Dictionary<Direction, Vector2> _playerDirection;
        private Dictionary<Direction, Sprite[]> _spriteDirectionMale;
        private Dictionary<Direction, Sprite[]> _spriteDirectionFemale;

        private Direction _direction = Direction.Down;
        private Vector2 _oldPosition;
        private Vector2 _position;
        private float _acceleration;
        private int _frame = 0;
        private Sprite _currentSprite;

        /// <summary>
        /// Player Constructor
        /// </summary>
        /// <param name="game">Game1 Instance</param>
        /// <param name="playerGender">The gender of the character</param>
        public Player(Game1 game, int playerGender) : base(game)
        {
            _playerGender = playerGender;
            _game = game;
            _position = new Vector2(0, 0);

            _playerDirection = new Dictionary<Direction, Vector2>
            {
                [Direction.Up] = Vector2.UnitY,
                [Direction.Down] = -Vector2.UnitY,
                [Direction.Left] = -Vector2.UnitX,
                [Direction.Right] = Vector2.UnitX
            };

            _spriteDirectionMale = new Dictionary<Direction, Sprite[]>
            {
                [Direction.Up] = new [] { new Sprite(game, "GU0", width: 0.5f), new Sprite(game, "GU1",width: 0.5f), new Sprite(game, "GU2", width: 0.5f) },
                [Direction.Down] = new [] { new Sprite(game, "GD0", width: 0.5f), new Sprite(game, "GD1", width: 0.5f), new Sprite(game, "GD2", width: 0.5f) },
                [Direction.Left] = new [] { new Sprite(game, "GL0", width: 0.5f), new Sprite(game, "GL1", width: 0.5f), new Sprite(game, "GL2", width: 0.5f) },
                [Direction.Right] = new [] { new Sprite(game, "GR0", width: 0.5f), new Sprite(game, "GR1", width: 0.5f), new Sprite(game, "GR2", width: 0.5f) }
            };

            _spriteDirectionFemale = new Dictionary<Direction, Sprite[]>
            {
                [Direction.Up] = new[] { new Sprite(game, "WU0", width: 0.5f), new Sprite(game, "WU1", width: 0.5f), new Sprite(game, "WU2", width: 0.5f) },
                [Direction.Down] = new[] { new Sprite(game, "WD0", width: 0.5f), new Sprite(game, "WD1", width: 0.5f), new Sprite(game, "WD2", width: 0.5f) },
                [Direction.Left] = new[] { new Sprite(game, "WL0", width: 0.5f), new Sprite(game, "WL1", width: 0.5f), new Sprite(game, "WL2", width: 0.5f) },
                [Direction.Right] = new[] { new Sprite(game, "WR0", width: 0.5f), new Sprite(game, "WR1", width: 0.5f), new Sprite(game, "WR2", width: 0.5f) }
            };

            if (_playerGender == 0)
            {
                _currentSprite = _spriteDirectionFemale[_direction][_frame];
            }
            else
            {
                _currentSprite = _spriteDirectionMale[_direction][_frame];
            }
        }

        public void LateUpdate(GameTime gameTime)
        {
            if (_currentSprite.cCollider._inCollision)
            {
                _currentSprite.SetPosition(_oldPosition);
            }
        }

        public override void Update(GameTime gameTime)
        {
            float deltaTime = gameTime.DeltaTime();

            if (_playerGender == 0)
            {
                _currentSprite = _spriteDirectionFemale[_direction][_frame];
            }
            else
            {
                _currentSprite = _spriteDirectionMale[_direction][_frame];
            }

            _oldPosition = _position;

            HandleInput(gameTime);

            _position += _acceleration * deltaTime * _playerDirection[_direction];

            _currentSprite.SetPosition(_position);
            _acceleration = 0;

            if ((_currentSprite.position.X + _currentSprite.position.Y) % 8 == 0)
            {
                _frame++;
                if (_frame > 2)
                    _frame = 0;
                Console.WriteLine($"frame: {_frame}");
            }

            Camera.LookAt(_position);
        }

        public void HandleInput(GameTime gameTime)
        {
            if (KeyboardManager.IsKeyDown(Keys.W))
            {
                _direction = Direction.Up;
                _acceleration = 2f;
            }
            if (KeyboardManager.IsKeyDown(Keys.S))
            {
                _direction = Direction.Down;
                _acceleration = 2f;
            }
            if (KeyboardManager.IsKeyDown(Keys.A))
            {
                _direction = Direction.Left;
                _acceleration = 2f;
            }
            if (KeyboardManager.IsKeyDown(Keys.D))
            {
                _direction = Direction.Right;
                _acceleration = 2f;
            }
            if (KeyboardManager.IsKeyDown(Keys.Space))
            {
                _position = Vector2.Zero;
            }
            
        }

        public void Draw(SpriteBatch sb)
        {
            _currentSprite.Draw(sb);
        }
    }
}
