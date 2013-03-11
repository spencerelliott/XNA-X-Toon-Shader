using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Ruminate.Utils;

namespace comp4900_xtoon
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        // Shaders
        Effect celShader;
        Effect postProcessEffect;

        Model theModel;

        // Used for post processing effects
        RenderTarget2D sceneRenderTarget;
        RenderTarget2D normalRenderTarget;

        Camera cam;

        KeyboardState oldState;

        // GUI stuff
        GuiManager gui;
        public SpriteFont GreySpriteFont;
        public Texture2D GreyImageMap;
        public string GreyMap;

        public Texture2D Tone1DDetailTexture;
        public Texture2D Tone2DDetailTexture;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            /* In case we allow user to resize window
            Window.ClientSizeChanged += delegate
            {
                if (gui != null) { gui.OnResize(); }
            };
             */
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            graphics.PreferredBackBufferWidth = 1067;
            graphics.PreferredBackBufferHeight = 600;
            graphics.ApplyChanges();
            IsMouseVisible = true;

            cam = new Camera(graphics.GraphicsDevice.Viewport);

            oldState = Keyboard.GetState();

            gui = new GuiManager();

            // Initialize shaders
            celShader = Content.Load<Effect>(@"Effects\Cel");
            postProcessEffect = Content.Load<Effect>(@"Effects\PostProcess");

            // Initialize render targets
            PresentationParameters pp = graphics.GraphicsDevice.PresentationParameters;
            normalRenderTarget = new RenderTarget2D(graphics.GraphicsDevice, pp.BackBufferWidth, pp.BackBufferHeight, false, 
                                                    pp.BackBufferFormat, pp.DepthStencilFormat, pp.MultiSampleCount, RenderTargetUsage.DiscardContents);

            sceneRenderTarget = new RenderTarget2D(graphics.GraphicsDevice, pp.BackBufferWidth, pp.BackBufferHeight, false,
                                                   pp.BackBufferFormat, pp.DepthStencilFormat, pp.MultiSampleCount, RenderTargetUsage.DiscardContents);
            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            theModel = Content.Load<Model>(@"Models\dude");

            GreyImageMap = Content.Load<Texture2D>(@"GreySkin\ImageMap");
            GreyMap = File.OpenText(@"Content\GreySkin\Map.txt").ReadToEnd();
            GreySpriteFont = Content.Load<SpriteFont>(@"GreySkin\Texture");

            Tone1DDetailTexture = Content.Load<Texture2D>(@"ToneTextures\cel_shading");
            Tone2DDetailTexture = Content.Load<Texture2D>(@"ToneTextures\xtoon_shading_gray");

            DebugUtils.Init(graphics.GraphicsDevice, GreySpriteFont);

            gui.Initialize(this);

            ChangeEffectUsedByModel(theModel, celShader);
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            // Allows the game to exit
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState(PlayerIndex.One).IsKeyDown(Keys.Escape))
                this.Exit();

            gui.Update();
            UpdateInput();
            cam.Update();

            base.Update(gameTime);
        }

        private void UpdateInput()
        {
            KeyboardState newState = Keyboard.GetState();
            const int offset = 50;
            const float rotateAmount = 0.05f;

            if (newState.IsKeyDown(Keys.W))
            {
                cam.Position.X -= offset;
            }
            if (newState.IsKeyDown(Keys.S))
            {
                cam.Position.X += offset;
            }
            if (newState.IsKeyDown(Keys.A))
            {
                cam.CameraYaw += rotateAmount;
            }
            if (newState.IsKeyDown(Keys.D))
            {
                cam.CameraYaw -= rotateAmount;
            }

            if (newState.IsKeyDown(Keys.Down))
            {
                cam.Position.Y -= offset;
            }
            if (newState.IsKeyDown(Keys.Up))
            {
                cam.Position.Y += offset;
            }
            if (newState.IsKeyDown(Keys.Left))
            {
                cam.CameraRoll -= rotateAmount;
            }
            if (newState.IsKeyDown(Keys.Right))
            {
                cam.CameraRoll += rotateAmount;
            }
            if (newState.IsKeyDown(Keys.Q))
            {
                cam.CameraPitch -= rotateAmount;
            }
            if (newState.IsKeyDown(Keys.Z))
            {
                cam.CameraPitch += rotateAmount;
            }

            // Save state
            oldState = newState;
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.White);

            // Normal Depth drawing
            graphics.GraphicsDevice.SetRenderTarget(normalRenderTarget);
            graphics.GraphicsDevice.Clear(Color.Black);
            DrawModel(cam.RotationMatrix, cam.ViewMatrix, cam.ProjectionMatrix, "NormalDepth", theModel);

            // Toon Effect
            graphics.GraphicsDevice.SetRenderTarget(sceneRenderTarget);
            graphics.GraphicsDevice.Clear(new Color(131, 125, 151));
            DrawModel(cam.RotationMatrix, cam.ViewMatrix, cam.ProjectionMatrix, "ToonShader", theModel);

            // Post-processing
            graphics.GraphicsDevice.SetRenderTarget(null);
            ApplyPostProcess("EdgeDetect");

            gui.Draw();

            base.Draw(gameTime);
        }

        private static void ChangeEffectUsedByModel(Model model, Effect replacementEffect)
        {
            Dictionary<Effect, Effect> effectMapping = new Dictionary<Effect, Effect>();

            foreach (ModelMesh mesh in model.Meshes)
            {
                foreach (BasicEffect oldEffect in mesh.Effects)
                {
                    if (!effectMapping.ContainsKey(oldEffect))
                    {
                        Effect newEffect = replacementEffect.Clone();
                        newEffect.Parameters["Texture"].SetValue(oldEffect.Texture);
                        newEffect.Parameters["TextureEnabled"].SetValue(oldEffect.TextureEnabled);
                        effectMapping.Add(oldEffect, newEffect);
                    }
                }
                foreach (ModelMeshPart meshPart in mesh.MeshParts)
                {
                    meshPart.Effect = effectMapping[meshPart.Effect];
                }
            }
        }

        private void DrawModel(Matrix world, Matrix view, Matrix projection, String effectTechniqueName, Model model)
        {
            // Set suitable RenderStates for drawing a 3D Model
            GraphicsDevice.BlendState = BlendState.Opaque;
            GraphicsDevice.DepthStencilState = DepthStencilState.Default;

            Matrix[] transforms = new Matrix[model.Bones.Count];
            model.CopyAbsoluteBoneTransformsTo(transforms);

            foreach (ModelMesh mesh in model.Meshes)
            {
                foreach (Effect effect in mesh.Effects)
                {
                    effect.CurrentTechnique = effect.Techniques[effectTechniqueName];

                    Matrix localWorld = transforms[mesh.ParentBone.Index] * world * Matrix.CreateScale(40.0f);
                    effect.Parameters["World"].SetValue(localWorld);
                    effect.Parameters["View"].SetValue(view);
                    effect.Parameters["Projection"].SetValue(projection);
                    effect.Parameters["UseToon"].SetValue(gui.UseToon);
                    effect.Parameters["ToneTexture"].SetValue(gui.UseXToon ? Tone2DDetailTexture : Tone1DDetailTexture);
                    effect.Parameters["Use2D"].SetValue(gui.UseXToon);
                    effect.Parameters["UseTexture"].SetValue(gui.UseTextures);
                    effect.Parameters["DetailAdjustment"].SetValue(gui.DetailAdjustment);
                }
                mesh.Draw();
            }
        }

        private void ApplyPostProcess(string effectTechniqueName)
        {
            EffectParameterCollection parameters = postProcessEffect.Parameters;
            Vector2 resolution = new Vector2(sceneRenderTarget.Width, sceneRenderTarget.Height);
            Texture2D normalDepthTexture = normalRenderTarget;
            parameters["EdgeWidth"].SetValue(gui.EdgeWidth);
            parameters["EdgeIntensity"].SetValue(gui.EdgeIntensity);
            parameters["ScreenResolution"].SetValue(resolution);
            parameters["NormalDepthTexture"].SetValue(normalDepthTexture);
            parameters["UseToon"].SetValue(gui.UseToon);
            parameters["DrawOutline"].SetValue(gui.DrawOutline);

            postProcessEffect.CurrentTechnique = postProcessEffect.Techniques[effectTechniqueName];
            
            // Draw a fullscreen sprite to apply the postprocessing effect
            spriteBatch.Begin(0, BlendState.Opaque, null, null, null, postProcessEffect);
            spriteBatch.Draw(sceneRenderTarget, Vector2.Zero, Color.White);
            spriteBatch.End();
        }
    }
}
