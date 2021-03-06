﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace PlaguePandemicsBats
{
    public class TilingBackground
    {
        #region Private variables
        private Texture2D _background;
        private Vector2 _realSize;
        private Game _game;
        private SpriteBatch _spriteBatch;
        #endregion

        #region Constructor
        public TilingBackground(Game game, string texture, Vector2 realSize)
        {
            _game = game;
            _realSize = realSize;

            _background = game.Content.Load<Texture2D>(texture);
            _spriteBatch = new SpriteBatch(_game.GraphicsDevice);

        }
        #endregion

        #region Methods

        /// <summary>
        /// Draws the whole scene of the background 
        /// </summary>
        /// <param name="gameTime"></param>
        public void Draw(GameTime gameTime)
        {
            //Compute the furthest camera viewing points
            Vector2 camTopLeft = Camera.Target() - Camera.Size() / 2f;
            Vector2 camBottomRight = Camera.Target() + Camera.Size() / 2f;

            Vector2 bottomleft = new Vector2(x: ((int)(camTopLeft.X / _realSize.X) - 1) * _realSize.X,
                                             y: ((int)(camTopLeft.Y / _realSize.Y) - 1) * _realSize.Y);

            Vector2 topright = new Vector2(x: ((int)(camBottomRight.X / _realSize.X) + 1) * _realSize.X,
                                           y: ((int)(camBottomRight.Y / _realSize.Y) + 1) * _realSize.Y);

            _spriteBatch.Begin();

            //Draw the sprite multiple times in the camera point of view
            for (float x = bottomleft.X; x <= topright.X; x += _realSize.X)
            {
                for (float y = bottomleft.Y; y <= topright.Y; y += _realSize.Y)
                {
                    Rectangle outRectangle = new Rectangle(Camera.ToPixel(new Vector2(x, y)).ToPoint(),(Camera.ToLength(_realSize) + Vector2.One).ToPoint());
                    
                    _spriteBatch.Draw(_background, 
                        outRectangle,   
                        null, Color.White, 0f,  
                        _background.Bounds.Size.ToVector2() / 2f, 
                        SpriteEffects.None, 0);
                }
            }
            _spriteBatch.End();
        }
        #endregion
    }
}
