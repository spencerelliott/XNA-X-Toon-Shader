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
using PrimitivesSample;

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

        // Models
        LinkedList<Model> models;
        LinkedListNode<Model> theModel;

        // Textures
        Texture2D Tone1DDetailTexture;
        LinkedList<Texture2D> detailTextures;
        LinkedListNode<Texture2D> Tone2DDetailTexture;

        // Used for post processing effects
        RenderTarget2D sceneRenderTarget;
        RenderTarget2D normalRenderTarget;

        Camera cam;

        KeyboardState oldState;

        //Lighting
        List<Light> lights;
        int degrees = 0;
        PrimitiveBatch primBatch;

        int modelId = 0;
        List<Vector3> lightPos = new List<Vector3>();
        List<float> lightIntensity = new List<float>();
        List<float> radiusSize = new List<float>();

        bool hideGui = false;
        bool showDetail = false;

        // GUI stuff
        GuiManager gui;
        public SpriteFont GreySpriteFont;
        public Texture2D GreyImageMap;
        public string GreyMap;

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

            //Add 2 new lights
            lights = new List<Light>();
            lights.Add(new Light());
            lights.Add(new Light());

            lightPos.Add(new Vector3(100.0f, 2000.0f, 100.0f));
            lightIntensity.Add(1200.0f);
            radiusSize.Add(1000.0f);

            lightPos.Add(new Vector3(2000.0f, 2000.0f, -2000.0f));
            lightIntensity.Add(1500.0f);
            radiusSize.Add(1500.0f);

            lightPos.Add(new Vector3(100.0f, 1000.0f, 100.0f));
            lightIntensity.Add(800.0f);
            radiusSize.Add(1000.0f);


            primBatch = new PrimitiveBatch(graphics.GraphicsDevice);

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

            // Models
            models = new LinkedList<Model>();
            models.AddLast(Content.Load<Model>(@"Models\dude"));
            models.AddLast(Content.Load<Model>(@"Models\ant"));
            models.AddLast(Content.Load<Model>(@"Models\Axe\FREEAXE"));
            theModel = models.First;

            // Detail textures
            Tone1DDetailTexture = Content.Load<Texture2D>(@"ToneTextures\cel_shading");
            detailTextures = new LinkedList<Texture2D>();
            detailTextures.AddLast(Content.Load<Texture2D>(@"ToneTextures\xtoon_white"));
            detailTextures.AddLast(Content.Load<Texture2D>(@"ToneTextures\xtoon_shading_gradient"));
            detailTextures.AddLast(Content.Load<Texture2D>(@"ToneTextures\xtoon_shading_gradient2"));
            detailTextures.AddLast(Content.Load<Texture2D>(@"ToneTextures\xtoon_shading_alpha"));
            detailTextures.AddLast(Content.Load<Texture2D>(@"ToneTextures\xtoon_shading"));
            detailTextures.AddLast(Content.Load<Texture2D>(@"ToneTextures\xtoon_shading_grayscale"));
            detailTextures.AddLast(Content.Load<Texture2D>(@"ToneTextures\xtoon_shading2"));
            detailTextures.AddLast(Content.Load<Texture2D>(@"ToneTextures\xtoon_shading_angle"));
            detailTextures.AddLast(Content.Load<Texture2D>(@"ToneTextures\xtoon_shading_angle2"));
            detailTextures.AddLast(Content.Load<Texture2D>(@"ToneTextures\xtoon_shading_angle3"));
            detailTextures.AddLast(Content.Load<Texture2D>(@"ToneTextures\xtoon_shading_angle4"));
            detailTextures.AddLast(Content.Load<Texture2D>(@"ToneTextures\xtoon_star"));
            detailTextures.AddLast(Content.Load<Texture2D>(@"ToneTextures\xtoon_star2"));
            detailTextures.AddLast(Content.Load<Texture2D>(@"ToneTextures\xtoon_star3"));
            detailTextures.AddLast(Content.Load<Texture2D>(@"ToneTextures\xtoon_skin"));
            detailTextures.AddLast(Content.Load<Texture2D>(@"ToneTextures\xtoon_red"));
            detailTextures.AddLast(Content.Load<Texture2D>(@"ToneTextures\noisy"));
            Tone2DDetailTexture = detailTextures.First;

            // Used for GUI
            GreyImageMap = Content.Load<Texture2D>(@"GreySkin\ImageMap");
            GreyMap = File.OpenText(@"Content\GreySkin\Map.txt").ReadToEnd();
            GreySpriteFont = Content.Load<SpriteFont>(@"GreySkin\Texture");

            DebugUtils.Init(graphics.GraphicsDevice, GreySpriteFont);

            gui.Initialize(this);

            foreach (Model model in models) {
                ChangeEffectUsedByModel(model, celShader);
            }
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

            if(!hideGui) gui.Update();
            UpdateInput();
            cam.Update();

            degrees++;
            if (degrees == 360) degrees = 0;

            lights[0].Position = new Vector3(
                lightPos[modelId].X,
                lightPos[modelId].Y + (float)Math.Cos((double)(degrees * Math.PI) / 180) * radiusSize[modelId],
                lightPos[modelId].Z + (float)Math.Sin((double)(degrees * Math.PI) / 180) * radiusSize[modelId]);

            lights[1].Position = new Vector3(
                lightPos[modelId].X + (float)Math.Sin((double)((degrees + 90) * Math.PI) / 180) * radiusSize[modelId],
                lightPos[modelId].Y + (float)Math.Cos((double)((degrees + 90) * Math.PI) / 180) * radiusSize[modelId],
                lightPos[modelId].Z);

            lights[0].Intensity = lightIntensity[modelId];
            lights[1].Intensity = lightIntensity[modelId];

            base.Update(gameTime);
        }

        private void UpdateInput()
        {
            KeyboardState newState = Keyboard.GetState();
            const int offset = 50;
            const float rotateAmount = 0.05f;

            // Camera
            if (newState.IsKeyDown(Keys.W))
            {
                cam.Position.X -= offset;
                cam.LookAt.X -= offset;
            }
            if (newState.IsKeyDown(Keys.S))
            {
                cam.Position.X += offset;
                cam.LookAt.X += offset;
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
                cam.LookAt.Y -= offset;
            }
            if (newState.IsKeyDown(Keys.Up))
            {
                cam.Position.Y += offset;
                cam.LookAt.Y += offset;
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

            //Gui hiding
            if (oldState.IsKeyDown(Keys.H) && newState.IsKeyUp(Keys.H))
            {
                hideGui = !hideGui;
            }

            if (oldState.IsKeyDown(Keys.J) && newState.IsKeyUp(Keys.J))
            {
                showDetail = !showDetail;
            }

            // Models
            if (oldState.IsKeyDown(Keys.L) && newState.IsKeyUp(Keys.L))
            {
                modelId = theModel.Next == null ? 0 : ++modelId;
                theModel = theModel.Next == null ? models.First : theModel.Next;
            }
            if (oldState.IsKeyDown(Keys.K) && newState.IsKeyUp(Keys.K))
            {
                modelId = theModel.Previous == null ? models.Count-1 : --modelId;
                theModel = theModel.Previous == null ? models.Last : theModel.Previous;
            }

            // Detail Textures
            if (oldState.IsKeyDown(Keys.P) && newState.IsKeyUp(Keys.P))
            {
                Tone2DDetailTexture = Tone2DDetailTexture.Next == null ? detailTextures.First : Tone2DDetailTexture.Next;
            }
            if (oldState.IsKeyDown(Keys.O) && newState.IsKeyUp(Keys.O))
            {
                Tone2DDetailTexture = Tone2DDetailTexture.Previous == null ? detailTextures.Last : Tone2DDetailTexture.Previous;
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
            DrawModel(cam.RotationMatrix, cam.ViewMatrix, cam.ProjectionMatrix, "NormalDepth", theModel.Value);

            // Toon Effect
            graphics.GraphicsDevice.SetRenderTarget(sceneRenderTarget);
            graphics.GraphicsDevice.Clear(Color.CornflowerBlue);

            if (!gui.DisableLighting)
            {
                foreach (Light l in lights)
                {
                    l.Draw3D(primBatch, cam.RotationMatrix, cam.ProjectionMatrix, cam.ViewMatrix, l.Position);
                    //l.Draw3D(primBatch, cam.ProjectionMatrix, new Vector3(0.0f, 0.0f, 1000.0f));
                }
            }

            DrawModel(cam.RotationMatrix, cam.ViewMatrix, cam.ProjectionMatrix, "ToonShader", theModel.Value);

            // Post-processing
            graphics.GraphicsDevice.SetRenderTarget(null);
            ApplyPostProcess("EdgeDetect");

            if (!hideGui) gui.Draw();

            // Draw the current texture
            if (gui.UseXToon)
            {
                const int rectSize = 100;
                Rectangle destRectangle = new Rectangle(graphics.GraphicsDevice.Viewport.Width - rectSize,
                                                        graphics.GraphicsDevice.Viewport.Height - rectSize,
                                                        rectSize, rectSize);
                spriteBatch.Begin();
                if (!hideGui || showDetail) spriteBatch.Draw(Tone2DDetailTexture.Value, destRectangle, Color.White);
                spriteBatch.End();
            }

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
            GraphicsDevice.BlendState = BlendState.AlphaBlend;
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
                    effect.Parameters["UseDistance"].SetValue(gui.UseDistance);
                    effect.Parameters["ToneTexture"].SetValue(gui.UseXToon ? Tone2DDetailTexture.Value : Tone1DDetailTexture);
                    effect.Parameters["Use2D"].SetValue(gui.UseXToon);
                    effect.Parameters["UseTexture"].SetValue(gui.UseTextures);
                    effect.Parameters["DetailAdjustment"].SetValue(gui.DetailAdjustment);
                    effect.Parameters["UseLightDirections"].SetValue(gui.UseLightDirections);
                    effect.Parameters["LookAt"].SetValue(cam.LookAt);
                    effect.Parameters["LightAttenuation"].SetValue(gui.LightAttenuation);
                    effect.Parameters["DisableLighting"].SetValue(gui.DisableLighting);

                    //Update lights
                    int i = 0;
                    foreach (Light l in lights)
                    {
                        effect.Parameters["LightPosition"].Elements[i].SetValue(l.Position);
                        effect.Parameters["LightIntensity"].Elements[i].SetValue(l.Intensity);
                        i++;
                    }
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
