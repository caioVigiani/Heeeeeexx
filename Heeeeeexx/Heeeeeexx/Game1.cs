using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace Heeeeeexx
{
    public class Game1 : Microsoft.Xna.Framework.Game
    {

        #region Variáveis
            #region De Sistema
                GraphicsDeviceManager graphics;
                SpriteBatch spriteBatch;
            #endregion

            #region De Recursos
                Texture2D titleScreen;
                Texture2D ballImage;
                SpriteFont pericles14;
                Color col = Color.White;
                Vector2 vetor = new Vector2(675, 365);
            #endregion

            #region De Localização
                int Movimentos;
                Vector2 MovimentoLocation = new Vector2(0, 0);
                Vector2 MovimentoFinal = new Vector2(600, 375);
                Vector2 CorLocation = new Vector2(0, 20);
            #endregion

            #region De Camera
                Rectangle current;
            #endregion

            #region De Jogo

                #region Balls
                    public const int ballsWide = 21;
                    public const int ballsHigh = 13;

                    Ball[,] balls;
                    private Ball b1;
                    private Ball b2;
                    private Ball b3;
                    private Ball b4;
                    private Ball b5;
                    private Ball b6;
                #endregion

                enum GameStates { TitleScreen, Playing, GameOver };
                GameStates gameStates = GameStates.TitleScreen;
                private Random rand = new Random();

                private Color[] colors = { Color.Green, Color.Red, Color.IndianRed, Color.DeepPink, Color.Yellow, Color.AntiqueWhite, Color.Aqua, Color.DarkOrange };
                int Indice1;
                int Indice2;

                private bool temp;
                private bool IsPressed = false;
                private int VerificaAlive;

                int tempoColocação = 150;
                int tempoExit = 0;
            #endregion
        #endregion

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        protected override void Initialize()
        {
            base.Initialize();

            this.IsMouseVisible = true;

            graphics.PreferredBackBufferWidth = 1360;
            graphics.PreferredBackBufferHeight = 760;

            Camera.WorldRectangle = new Rectangle(0, 0, 1360, 760);
            Camera.ViewPortWidth = 1292;
            Camera.ViewPortHeight = 652;

            graphics.ApplyChanges();
        }

        protected override void LoadContent()
        {
            titleScreen = Content.Load<Texture2D>(@"Textures\TitleScreen");
            pericles14 = Content.Load<SpriteFont>(@"Fonts\Pericles14");

            spriteBatch = new SpriteBatch(GraphicsDevice);

            ballImage = Content.Load<Texture2D>("GlassBall");
        }

        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        // atualiza a cada frame
        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                this.Exit();

            // Quando estiver em estado de jogo
            // verifica ações do mouse e tecla esc para sair do jogo
            if (gameStates == GameStates.Playing)
            {
                HandleMouseInput(Mouse.GetState());

                if (Keyboard.GetState().IsKeyDown(Keys.Escape))
                {
                    this.Exit();
                }
            }

            if (gameStates == GameStates.GameOver)
                tempoColocação--;

            if (tempoColocação == 0)
                gameStates = GameStates.TitleScreen;

            // Quando estiver em estado de início
            // verifica tecla espaço para começar jogo
            if (gameStates == GameStates.TitleScreen)
            {
                if (Keyboard.GetState().IsKeyDown(Keys.Space))
                    StartGame();
                if (Keyboard.GetState().IsKeyDown(Keys.Escape))
                    this.Exit();
            }

            base.Update(gameTime);
        }

        // desenha a cada frame
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            spriteBatch.Begin();

            // Quando estiver em estado de início
            // desenha imagem da tela de início
            if (gameStates == GameStates.TitleScreen)
            {
                spriteBatch.Draw(titleScreen, new Rectangle(0, 0, this.Window.ClientBounds.Width, this.Window.ClientBounds.Height), Color.White);
                spriteBatch.DrawString(pericles14, "Barra de espaco para iniciar o jogo!", new Vector2(500, 720), Color.Red);
            }

            // Quando estiver em estado de jogo
            // desenha todas as bolas individualmente e as frases de verificação
            if (gameStates == GameStates.Playing)
            {
                foreach (Ball ball in balls)
                    ball.Draw(spriteBatch);

                spriteBatch.DrawString(pericles14, "Movimentos: " + Movimentos.ToString(), MovimentoLocation, Color.White);
                //spriteBatch.DrawString(pericles14, "Cor Atual: " + balls[10, 6].tint, CorLocation, Color.Red);
            }

            // Quando estiver em estado de fim de jogo
            // desenha frase de pontuação
            if (gameStates == GameStates.GameOver)
            {
                spriteBatch.DrawString(pericles14, "Total de Movimentos: " + Movimentos.ToString(), MovimentoFinal, Color.Yellow);

                if (Movimentos >= 1 && Movimentos <= 10)
                    spriteBatch.DrawString(pericles14, "Hacker", new Vector2(680,410), Color.Red);

                if (Movimentos > 10 && Movimentos <= 20)
                    spriteBatch.DrawString(pericles14, "TOP", new Vector2(700, 410), Color.Green);

                if (Movimentos > 20 && Movimentos <= 25)
                    spriteBatch.DrawString(pericles14, "Ruinzinho em", new Vector2(660, 410), Color.Yellow);

                if (Movimentos > 25)
                    spriteBatch.DrawString(pericles14, "Patetico", new Vector2(680, 410), Color.Gray);
            }

            spriteBatch.End();

            base.Draw(gameTime);
        }

        // Quando começar um novo jogo
        // preenche a lista com bolas de novas cores
        // reseta variáveis de controle do jogo
        private void StartGame()
        {
            gameStates = GameStates.Playing;

            balls = new Ball[ballsWide, ballsHigh];

            for (int y = 0; y < ballsHigh; y++)
            {
                int a = 0;
                if (IsImpar(y)) a = 34;

                for (int x = 0; x < ballsWide; x++)
                {
                    Color tint = colors[rand.Next(0, colors.Count())];
                    if (x == 0 || y == 0) tint = Color.Black;
                    if (x == 20 || y == 12) tint = Color.Black;
                    Indice1 = x;
                    Indice2 = y;
                    balls[x, y] = new Ball(Indice1, Indice2, ballImage, new Rectangle(x * 66 + a, y * 58, 66, 66), tint);
                }
                VerificaAlive = 0;
                tempoColocação = 150;
                Movimentos = 0;
            }

            foreach (Ball ball in balls)
            {
                current = ball.location;

                if (current.Contains(675, 365))
                {
                    ball.alive = true;
                    ball.tint = Color.Gray;
                }
            }
        }

        // Verifica ações do mouse
        private void HandleMouseInput(MouseState mouseState)
        {
            MouseState mouse = Mouse.GetState();

            if ((mouseState.LeftButton == ButtonState.Released) && (IsPressed == true))
                IsPressed = false;

            if ((mouseState.LeftButton == ButtonState.Pressed) && (IsPressed == false))
            {
                IsPressed = true;
                foreach (Ball ball in balls)
                {
                    current = ball.location;
                    if (current.Contains(mouse.X, mouse.Y))
                    {
                        col = ball.tint;
                        VerificaClick(ball);
                        VerificaAlive = 0;
                        foreach (Ball bola in balls)
                        {
                            if (bola.alive == true)
                                VerificaAlive++;
                            if (VerificaAlive == 209)
                            {
                                gameStates = GameStates.GameOver;
                            }
                        }
                    }
                }
            }
        }

        // lógica do click do mouse
        private void VerificaClick(Ball b)
        {
            if (b.tint != Color.Black)
            {
                if (!IsImpar(b.indice2))
                {
                    b1 = balls[b.indice1 - 1, b.indice2 - 1];
                    b2 = balls[b.indice1, b.indice2 - 1];
                    b3 = balls[b.indice1 - 1, b.indice2];
                    b4 = balls[b.indice1 + 1, b.indice2];
                    b5 = balls[b.indice1 - 1, b.indice2 + 1];
                    b6 = balls[b.indice1, b.indice2 + 1];
                }
                else
                {
                    b1 = balls[b.indice1, b.indice2 - 1];
                    b2 = balls[b.indice1 + 1, b.indice2 - 1];
                    b3 = balls[b.indice1 - 1, b.indice2];
                    b4 = balls[b.indice1 + 1, b.indice2];
                    b5 = balls[b.indice1, b.indice2 + 1];
                    b6 = balls[b.indice1 + 1, b.indice2 + 1];
                }

                if (b1.alive == true || b2.alive == true || b3.alive == true || b4.alive == true || b5.alive == true || b6.alive == true)
                {
                    Movimentos++;
                    foreach (Ball ball in balls)
                    {
                        if (ball.alive == true)
                        {
                            ball.tint = b.tint;
                            b.alive = true;
                            temp = true;
                            Conect();
                        }
                    }
                }

            }
        }

        // lógica de conexão entre as bolas
        private void Conect()
        {
            while (temp == true)
            {
                temp = false;
                foreach (Ball b in balls)
                {

                    if (b.tint == col && b.alive == false)
                    {

                        if (!IsImpar(b.indice2))
                        {
                            b1 = balls[b.indice1 - 1, b.indice2 - 1];
                            b2 = balls[b.indice1, b.indice2 - 1];
                            b3 = balls[b.indice1 - 1, b.indice2];
                            b4 = balls[b.indice1 + 1, b.indice2];
                            b5 = balls[b.indice1 - 1, b.indice2 + 1];
                            b6 = balls[b.indice1, b.indice2 + 1];
                        }
                        else
                        {
                            b1 = balls[b.indice1, b.indice2 - 1];
                            b2 = balls[b.indice1 + 1, b.indice2 - 1];
                            b3 = balls[b.indice1 - 1, b.indice2];
                            b4 = balls[b.indice1 + 1, b.indice2];
                            b5 = balls[b.indice1, b.indice2 + 1];
                            b6 = balls[b.indice1 + 1, b.indice2 + 1];
                        }

                        if (b1.alive == true || b2.alive == true || b3.alive == true || b4.alive == true || b5.alive == true || b6.alive == true)
                        {
                            b.alive = true;
                            temp = true;
                        }
                    }
                }
            }
        }

        // verifica linha da bola
        // altera lógica de conexão
        private bool IsImpar(int x)
        {
            if (x % 2 != 0) return true;
            else return false;
        }

    }
}
