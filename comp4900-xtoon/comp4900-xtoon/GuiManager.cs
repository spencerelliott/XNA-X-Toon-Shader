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
        ToggleButton useToonButton, drawOutlineButton, useXToon;
        const float maxEdgeWidth = 10.0f;
        const float maxEdgeIntensity = 10.0f;
        const float maxDetail = 1000.0f;
        Slider edgeWidth, edgeIntensity, detailAdjustment;
        Label edgeWidthLabel, edgeIntensityLabel, detailAdjustmentLabel;

        public void Initialize(Game1 game)
        {
            var skin = new Skin(game.GreyImageMap, game.GreyMap);
            var text = new TextRenderer(game.GreySpriteFont, Color.White);
            const int margin = 10;
            const int buttonHeight = 40;
            gui = new Gui(game, skin, text)
            {
                Widgets = new Widget[] {
                    useToonButton = new ToggleButton(margin, margin + (buttonHeight * 0), "Use Toon") {
                        IsToggled = true
                    },
                    drawOutlineButton = new ToggleButton(margin, margin + (buttonHeight * 1), "Draw Outline") {
                        IsToggled = true
                    },
                    useXToon = new ToggleButton(margin, margin + (buttonHeight * 2), "Use X-Toon") {
                        IsToggled = false
                    },
                    edgeWidth = new Slider(margin, margin + (buttonHeight * 3), 150, delegate(Widget slider) {
                        edgeWidthLabel.Value = "Edge Width = " + (((Slider)slider).Value * maxEdgeWidth);
                    }) {
                        Value = 1 / maxEdgeWidth
                    },
                    edgeWidthLabel = new Label(margin, margin + (buttonHeight * 4), "Edge Width = 1.0"),
                    edgeIntensity = new Slider(margin, margin + (buttonHeight * 5), 150, delegate(Widget slider) {
                        edgeIntensityLabel.Value = "Edge Intensity = " + (((Slider)slider).Value * maxEdgeIntensity);
                    }) {
                        Value = 1 / maxEdgeIntensity
                    },
                    edgeIntensityLabel = new Label(margin, margin + (buttonHeight * 6), "Edge Intensity = 1.0"),
                    detailAdjustment = new Slider(margin, margin + (buttonHeight * 7), 150, delegate(Widget slider) {
                        detailAdjustmentLabel.Value = "Detail Adjustment = " + (((Slider)slider).Value * maxDetail);
                    }) {
                       Value = 1 / maxDetail 
                    },
                    detailAdjustmentLabel = new Label(margin, margin + (buttonHeight * 8), "Detail Adjustment = 1.0")
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
