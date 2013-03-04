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

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            cam = new Camera(graphics.GraphicsDevice.Viewport);

            oldState = Keyboard.GetState();

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
            GraphicsDevice.Clear(Color.CornflowerBlue);

            // Normal Depth drawing
            graphics.GraphicsDevice.SetRenderTarget(normalRenderTarget);
            graphics.GraphicsDevice.Clear(Color.Black);
            DrawModel(cam.RotationMatrix, cam.ViewMatrix, cam.ProjectionMatrix, "NormalDepth", theModel);

            // Toon Effect
            graphics.GraphicsDevice.SetRenderTarget(sceneRenderTarget);
            graphics.GraphicsDevice.Clear(Color.CornflowerBlue);
            DrawModel(cam.RotationMatrix, cam.ViewMatrix, cam.ProjectionMatrix, "ToonShader", theModel);

            // Post-processing
            graphics.GraphicsDevice.SetRenderTarget(null);
            ApplyPostProcess("EdgeDetect");

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
                }
                mesh.Draw();
            }
        }

        private void ApplyPostProcess(string effectTechniqueName)
        {
            EffectParameterCollection parameters = postProcessEffect.Parameters;
            Vector2 resolution = new Vector2(sceneRenderTarget.Width, sceneRenderTarget.Height);
            Texture2D normalDepthTexture = normalRenderTarget;
            parameters["EdgeWidth"].SetValue(1);
            parameters["EdgeIntensity"].SetValue(1);
            parameters["ScreenResolution"].SetValue(resolution);
            parameters["NormalDepthTexture"].SetValue(normalDepthTexture);

            postProcessEffect.CurrentTechnique = postProcessEffect.Techniques[effectTechniqueName];
            
            // Draw a fullscreen sprite to apply the postprocessing effect
            spriteBatch.Begin(0, BlendState.Opaque, null, null, null, postProcessEffect);
            spriteBatch.Draw(sceneRenderTarget, Vector2.Zero, Color.White);
            spriteBatch.End();
        }
    }
}
