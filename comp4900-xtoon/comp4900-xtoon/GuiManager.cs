using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Ruminate.GUI.Content;
using Ruminate.GUI.Framework;

namespace comp4900_xtoon
{
    class GuiManager
    {
        Gui gui;
        ToggleButton useToonButton, drawOutlineButton, useXToon, useTextureButton, useLightDirections, disableLighting;
        const float maxEdgeWidth = 10.0f;
        const float maxEdgeIntensity = 10.0f;
        const float maxDetail = 10000.0f;
        const float maxAttenuation = 5000.0f;
        Slider edgeWidth, edgeIntensity, detailAdjustment, lightAttentuation;
        Label edgeWidthLabel, edgeIntensityLabel, detailAdjustmentLabel, lightAttenuationLabel;

        public void Initialize(Game1 game)
        {
            var skin = new Skin(game.GreyImageMap, game.GreyMap);
            var text = new TextRenderer(game.GreySpriteFont, Color.White);
            const int margin = 10;
            const int buttonHeight = 40;

            int i = -1;

            gui = new Gui(game, skin, text)
            {
                Widgets = new Widget[] {
                    useToonButton = new ToggleButton(margin, margin + (buttonHeight * ++i), "Use Toon") {
                        IsToggled = true
                    },
                    drawOutlineButton = new ToggleButton(margin, margin + (buttonHeight * ++i), "Draw Outline") {
                        IsToggled = true
                    },
                    useXToon = new ToggleButton(margin, margin + (buttonHeight * ++i), "Use X-Toon") {
                        IsToggled = false
                    },
                    useTextureButton = new ToggleButton(margin, margin + (buttonHeight * ++i), "Use Textures") {
                        IsToggled = true
                    },
                    edgeWidth = new Slider(margin, margin + (buttonHeight * ++i), 150, delegate(Widget slider) {
                        edgeWidthLabel.Value = "Edge Width = " + (((Slider)slider).Value * maxEdgeWidth);
                    }) {
                        Value = 1 / maxEdgeWidth
                    },
                    edgeWidthLabel = new Label(margin, margin + (buttonHeight * ++i), "Edge Width = 1.0"),
                    edgeIntensity = new Slider(margin, margin + (buttonHeight * ++i), 150, delegate(Widget slider) {
                        edgeIntensityLabel.Value = "Edge Intensity = " + (((Slider)slider).Value * maxEdgeIntensity);
                    }) {
                        Value = 1 / maxEdgeIntensity
                    },
                    edgeIntensityLabel = new Label(margin, margin + (buttonHeight * ++i), "Edge Intensity = 1.0"),
                    detailAdjustment = new Slider(margin, margin + (buttonHeight * ++i), 150, delegate(Widget slider) {
                        detailAdjustmentLabel.Value = "Detail Adjustment = " + (((Slider)slider).Value * maxDetail);
                    }) {
                       Value = 1 / maxDetail 
                    },
                    detailAdjustmentLabel = new Label(margin, margin + (buttonHeight * ++i), "Detail Adjustment = 1.0"),
                    useLightDirections = new ToggleButton(margin, margin + (buttonHeight * ++i), "Use Light Directions"),
                    lightAttentuation = new Slider(margin, margin + (buttonHeight * ++i), 150, delegate(Widget slider) {
                        lightAttenuationLabel.Value = "Light attenuation = " + (((Slider)slider).Value * maxAttenuation);
                    }) {
                        Value = 1200 / maxAttenuation
                    },
                    lightAttenuationLabel = new Label(margin, margin + (buttonHeight * ++i), "Light attentuation = 1200.0"),
                    disableLighting = new ToggleButton(margin, margin + (buttonHeight * ++i), "Disable lighting")
                }
            };
        }

        public Boolean UseToon
        {
            get { return useToonButton.IsToggled; }
        }

        public Boolean DrawOutline
        {
            get { return drawOutlineButton.IsToggled; }
        }

        public float EdgeWidth
        {
            get { return edgeWidth.Value * maxEdgeWidth; }
        }

        public float EdgeIntensity
        {
            get { return edgeIntensity.Value * maxEdgeWidth; }
        }

        public bool UseXToon
        {
            get { return useXToon.IsToggled; }
        }

        public float DetailAdjustment
        {
            get { return detailAdjustment.Value * maxDetail; }
        }

        public bool UseTextures
        {
            get { return useTextureButton.IsToggled; }
        }

        public bool UseLightDirections
        {
            get { return useLightDirections.IsToggled; }
        }

        public float LightAttenuation
        {
            get { return lightAttentuation.Value * maxAttenuation; }
        }

        public bool DisableLighting
        {
            get { return disableLighting.IsToggled; }
        }

        public void Update()
        {
            gui.Update();
        }
        public void Draw()
        {
            gui.Draw();
        }
    }
}
