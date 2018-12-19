// COPYRIGHT 2018 Roberto Ceccarelli - Casasoft.
//
// Original work is COPYRIGHT 2011, 2012, 2013 by the Open Rails project.
// 
// This file is part of OR Tools.
// 
// OR Tools is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// OR Tools is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License
// along with OR Tools.  If not, see <http://www.gnu.org/licenses/>.

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Orts.Formats.Msts;
using Orts.Viewer3D;
using ORTS.Settings;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;


namespace Casasoft.ShapeViewerLib
{
    public class Simulator {
        public UserSettings Settings;

        public string BasePath;     // ie c:\program files\microsoft games\train simulator
        public string RoutePath;    // ie c:\program files\microsoft games\train simulator\routes\usa1  - may be different on different pc's

        public SeasonType Season;
        public WeatherType WeatherType;

    }

    public class Viewer
    {
        public GraphicsDevice GraphicsDevice;
        public UserSettings Settings;
        public string ContentPath = ".\\content\\";

        public SharedTextureManager TextureManager { get; private set; }
        public SharedMaterialManager MaterialManager { get; private set; }
        public Simulator Simulator;

        public bool DontLoadNightTextures = false;
        public bool DontLoadDayTextures = false;
        public bool NightTexturesNotLoaded = false;
        public bool DayTexturesNotLoaded = false;

        public Viewer(GraphicsDevice GraphicsDevice, Simulator sim)
        {
            this.GraphicsDevice = GraphicsDevice;
            this.Simulator = sim;
            this.Settings = sim.Settings;
            TextureManager = new SharedTextureManager(this, GraphicsDevice);
            MaterialManager = new SharedMaterialManager(this);
        }
    }

    [Flags]
    public enum ShapeFlags
    {
        None = 0,
        // Shape casts a shadow (scenery objects according to RE setting, and all train objects).
        ShadowCaster = 1,
        // Shape needs automatic z-bias to keep it out of trouble.
        AutoZBias = 2,
        // Shape is an interior and must be rendered in a separate group.
        Interior = 4,
        // NOTE: Use powers of 2 for values!
    }

    public enum RenderPrimitiveSequence
    {
        CabOpaque,
        Sky,
        WorldOpaque,
        WorldBlended,
        Lights, // TODO: May not be needed once alpha sorting works.
        Precipitation, // TODO: May not be needed once alpha sorting works.
        Particles,
        InteriorOpaque,
        InteriorBlended,
        Labels,
        CabBlended,
        OverlayOpaque,
        OverlayBlended,
        // This value must be last.
        Sentinel
    }
    public enum RenderPrimitiveGroup
    {
        Cab,
        Sky,
        World,
        Lights, // TODO: May not be needed once alpha sorting works.
        Precipitation, // TODO: May not be needed once alpha sorting works.
        Particles,
        Interior,
        Labels,
        Overlay
    }

    struct ShapeInstanceData
    {
#pragma warning disable 0649
        public Matrix World;
#pragma warning restore 0649

        public static readonly VertexElement[] VertexElements = {
            new VertexElement(sizeof(float) * 0, VertexElementFormat.Vector4, VertexElementUsage.TextureCoordinate, 1),
            new VertexElement(sizeof(float) * 4, VertexElementFormat.Vector4, VertexElementUsage.TextureCoordinate, 2),
            new VertexElement(sizeof(float) * 8, VertexElementFormat.Vector4, VertexElementUsage.TextureCoordinate, 3),
            new VertexElement(sizeof(float) * 12, VertexElementFormat.Vector4, VertexElementUsage.TextureCoordinate, 4),
        };

        public static int SizeInBytes = sizeof(float) * 16;
    }

    public abstract class RenderPrimitive
    {
        /// <summary>
        /// Mapping from <see cref="RenderPrimitiveGroup"/> to <see cref="RenderPrimitiveSequence"/> for blended
        /// materials. The number of items in the array must equal the number of values in
        /// <see cref="RenderPrimitiveGroup"/>.
        /// </summary>
        public static readonly RenderPrimitiveSequence[] SequenceForBlended = new[] {
            RenderPrimitiveSequence.CabBlended,
            RenderPrimitiveSequence.Sky,
            RenderPrimitiveSequence.WorldBlended,
            RenderPrimitiveSequence.Lights,
            RenderPrimitiveSequence.Precipitation,
            RenderPrimitiveSequence.Particles,
            RenderPrimitiveSequence.InteriorBlended,
            RenderPrimitiveSequence.Labels,
            RenderPrimitiveSequence.OverlayBlended,
        };

        /// <summary>
        /// Mapping from <see cref="RenderPrimitiveGroup"/> to <see cref="RenderPrimitiveSequence"/> for opaque
        /// materials. The number of items in the array must equal the number of values in
        /// <see cref="RenderPrimitiveGroup"/>.
        /// </summary>
        public static readonly RenderPrimitiveSequence[] SequenceForOpaque = new[] {
            RenderPrimitiveSequence.CabOpaque,
            RenderPrimitiveSequence.Sky,
            RenderPrimitiveSequence.WorldOpaque,
            RenderPrimitiveSequence.Lights,
            RenderPrimitiveSequence.Precipitation,
            RenderPrimitiveSequence.Particles,
            RenderPrimitiveSequence.InteriorOpaque,
            RenderPrimitiveSequence.Labels,
            RenderPrimitiveSequence.OverlayOpaque,
        };

        protected static VertexBuffer DummyVertexBuffer;
        protected static readonly VertexDeclaration DummyVertexDeclaration = new VertexDeclaration(ShapeInstanceData.SizeInBytes, ShapeInstanceData.VertexElements);
        protected static readonly Matrix[] DummyVertexData = new Matrix[] { Matrix.Identity };

        /// <summary>
        /// This is an adjustment for the depth buffer calculation which may be used to reduce the chance of co-planar primitives from fighting each other.
        /// </summary>
        // TODO: Does this actually make any real difference?
        public float ZBias;

        /// <summary>
        /// This is a sorting adjustment for primitives with similar/the same world location. Primitives with higher SortIndex values are rendered after others. Has no effect on non-blended primitives.
        /// </summary>
        public float SortIndex;

        /// <summary>
        /// This is when the object actually renders itself onto the screen.
        /// Do not reference any volatile data.
        /// Executes in the RenderProcess thread
        /// </summary>
        /// <param name="graphicsDevice"></param>
        public abstract void Draw(GraphicsDevice graphicsDevice);
    }

    public struct RenderItem
    {
        public Material Material;
        public RenderPrimitive RenderPrimitive;
        public Matrix XNAMatrix;
        public ShapeFlags Flags;

        public RenderItem(Material material, RenderPrimitive renderPrimitive, ref Matrix xnaMatrix, ShapeFlags flags)
        {
            Material = material;
            RenderPrimitive = renderPrimitive;
            XNAMatrix = xnaMatrix;
            Flags = flags;
        }

        public class Comparer : IComparer<RenderItem>
        {
            readonly Vector3 XNAViewerPos;

            public Comparer(Vector3 viewerPos)
            {
                XNAViewerPos = viewerPos;
                XNAViewerPos.Z *= -1;
            }

            #region IComparer<RenderItem> Members

            public int Compare(RenderItem x, RenderItem y)
            {
                // For unknown reasons, this would crash with an ArgumentException (saying Compare(x, x) != 0)
                // sometimes when calculated as two values and subtracted. Presumed cause is floating point.
                var xd = (x.XNAMatrix.Translation - XNAViewerPos).Length();
                var yd = (y.XNAMatrix.Translation - XNAViewerPos).Length();
                // If the absolute difference is >= 1mm use that; otherwise, they're effectively in the same
                // place so fall back to the SortIndex.
                if (Math.Abs(yd - xd) >= 0.001)
                    return Math.Sign(yd - xd);
                return Math.Sign(x.RenderPrimitive.SortIndex - y.RenderPrimitive.SortIndex);
            }

            #endregion
        }
    }

    public static class Helpers
    {
        [Flags]
        public enum TextureFlags
        {
            None = 0x0,
            Snow = 0x1,
            SnowTrack = 0x2,
            Spring = 0x4,
            Autumn = 0x8,
            Winter = 0x10,
            SpringSnow = 0x20,
            AutumnSnow = 0x40,
            WinterSnow = 0x80,
            Night = 0x100,
            Underground = 0x40000000,
        }

        /*
        [Flags]
        public enum SceneryMaterialOptions
        {
            None = 0,
            // Diffuse
            Diffuse = 0x1,
            // Alpha test
            AlphaTest = 0x2,
            // Blending
            AlphaBlendingNone = 0x0,
            AlphaBlendingBlend = 0x4,
            AlphaBlendingAdd = 0x8,
            AlphaBlendingMask = 0xC,
            // Shader
            ShaderImage = 0x00,
            ShaderDarkShade = 0x10,
            ShaderHalfBright = 0x20,
            ShaderFullBright = 0x30,
            ShaderVegetation = 0x40,
            ShaderMask = 0x70,
            // Lighting
            Specular0 = 0x000,
            Specular25 = 0x080,
            Specular750 = 0x100,
            SpecularMask = 0x180,
            // Texture address mode
            TextureAddressModeWrap = 0x000,
            TextureAddressModeMirror = 0x200,
            TextureAddressModeClamp = 0x400,
            TextureAddressModeMask = 0x600,
            // Night texture
            NightTexture = 0x800,
            // Texture to be shown in tunnels and underground (used for 3D cab night textures)
            UndergroundTexture = 0x40000000,
        }
*/

        public static string GetForestTextureFile(Simulator simulator, string textureName)
        {
            return GetRouteTextureFile(simulator, Helpers.TextureFlags.Spring | Helpers.TextureFlags.Autumn | Helpers.TextureFlags.Winter | Helpers.TextureFlags.SpringSnow | Helpers.TextureFlags.AutumnSnow | Helpers.TextureFlags.WinterSnow, textureName);
        }

        public static string GetNightTextureFile(Simulator simulator, string textureFilePath)
        {
            var texturePath = Path.GetDirectoryName(textureFilePath);
            var textureName = Path.GetFileName(textureFilePath);
            var nightTexturePath = !File.Exists(texturePath + @"\Night\" + textureName) ? Path.GetDirectoryName(texturePath) + @"\Night\" : texturePath + @"\Night\";

            if (!String.IsNullOrEmpty(nightTexturePath + textureName) && Path.GetExtension(nightTexturePath + textureName) == ".dds" && File.Exists(nightTexturePath + textureName))
            {
                return nightTexturePath + textureName;
            }
            else if (!String.IsNullOrEmpty(nightTexturePath + textureName) && Path.GetExtension(nightTexturePath + textureName) == ".ace")
            {
                var alternativeTexture = Path.ChangeExtension(nightTexturePath + textureName, ".dds");
                if (simulator.Settings.PreferDDSTexture && !String.IsNullOrEmpty(alternativeTexture.ToLower()) && File.Exists(alternativeTexture))
                {
                    return alternativeTexture;
                }
                else if (File.Exists(nightTexturePath + textureName))
                {
                    return nightTexturePath + textureName;
                }
                else
                {
                    return null;
                }
            }
            else
            {
                return null;
            }
        }

        public static string GetRouteTextureFile(Simulator simulator, TextureFlags textureFlags, string textureName)
        {
            return GetTextureFile(simulator, textureFlags, simulator.RoutePath + @"\Textures", textureName);
        }

        public static string GetTransferTextureFile(Simulator simulator, string textureName)
        {
            return GetTextureFile(simulator, Helpers.TextureFlags.Snow, simulator.RoutePath + @"\Textures", textureName);
        }

        public static string GetTerrainTextureFile(Simulator simulator, string textureName)
        {
            return GetTextureFile(simulator, Helpers.TextureFlags.Snow, simulator.RoutePath + @"\TerrTex", textureName);
        }

        public static string GetTextureFile(Simulator simulator, TextureFlags textureFlags, string texturePath, string textureName)
        {
            var alternativePath = @"\";
            if ((textureFlags & TextureFlags.Snow) != 0 || (textureFlags & TextureFlags.SnowTrack) != 0)
                if (IsSnow(simulator))
                    alternativePath = @"\Snow\";
                else
                    alternativePath = @"\";
            else if ((textureFlags & TextureFlags.Spring) != 0 && simulator.Season == SeasonType.Spring && simulator.WeatherType != WeatherType.Snow)
                alternativePath = @"\Spring\";
            else if ((textureFlags & TextureFlags.Autumn) != 0 && simulator.Season == SeasonType.Autumn && simulator.WeatherType != WeatherType.Snow)
                alternativePath = @"\Autumn\";
            else if ((textureFlags & TextureFlags.Winter) != 0 && simulator.Season == SeasonType.Winter && simulator.WeatherType != WeatherType.Snow)
                alternativePath = @"\Winter\";
            else if ((textureFlags & TextureFlags.SpringSnow) != 0 && simulator.Season == SeasonType.Spring && simulator.WeatherType == WeatherType.Snow)
                alternativePath = @"\SpringSnow\";
            else if ((textureFlags & TextureFlags.AutumnSnow) != 0 && simulator.Season == SeasonType.Autumn && simulator.WeatherType == WeatherType.Snow)
                alternativePath = @"\AutumnSnow\";
            else if ((textureFlags & TextureFlags.WinterSnow) != 0 && simulator.Season == SeasonType.Winter && simulator.WeatherType == WeatherType.Snow)
                alternativePath = @"\WinterSnow\";

            if (alternativePath.Length > 0) return texturePath + alternativePath + textureName;
            return texturePath + @"\" + textureName;
        }

        public static bool IsSnow(Simulator simulator)
        {
            // MSTS shows snow textures:
            //   - In winter, no matter what the weather is.
            //   - In spring and autumn, if the weather is snow.
            return (simulator.Season == SeasonType.Winter) || ((simulator.Season != SeasonType.Summer) && (simulator.WeatherType == WeatherType.Snow));
        }

        static readonly Dictionary<string, SceneryMaterialOptions> TextureAddressingModeNames = new Dictionary<string, SceneryMaterialOptions> {
            { "Wrap", SceneryMaterialOptions.TextureAddressModeWrap },
            { "Mirror", SceneryMaterialOptions.TextureAddressModeMirror },
            { "Clamp", SceneryMaterialOptions.TextureAddressModeClamp },
        };

        static readonly Dictionary<string, SceneryMaterialOptions> ShaderNames = new Dictionary<string, SceneryMaterialOptions> {
            { "Tex", SceneryMaterialOptions.None },
            { "TexDiff", SceneryMaterialOptions.Diffuse },
            { "BlendATex", SceneryMaterialOptions.AlphaBlendingBlend },
            { "BlendATexDiff", SceneryMaterialOptions.AlphaBlendingBlend | SceneryMaterialOptions.Diffuse },
            { "AddATex", SceneryMaterialOptions.AlphaBlendingAdd },
            { "AddATexDiff", SceneryMaterialOptions.AlphaBlendingAdd | SceneryMaterialOptions.Diffuse },
        };

        static readonly Dictionary<string, SceneryMaterialOptions> LightingModelNames = new Dictionary<string, SceneryMaterialOptions> {
            { "DarkShade", SceneryMaterialOptions.ShaderDarkShade },
            { "OptHalfBright", SceneryMaterialOptions.ShaderHalfBright },
            { "Cruciform", SceneryMaterialOptions.ShaderVegetation },
            { "OptFullBright", SceneryMaterialOptions.ShaderFullBright },
            { "OptSpecular750", SceneryMaterialOptions.None | SceneryMaterialOptions.Specular750 },
            { "OptSpecular25", SceneryMaterialOptions.None | SceneryMaterialOptions.Specular25 },
            { "OptSpecular0", SceneryMaterialOptions.None | SceneryMaterialOptions.None },
        };
/*
        /// <summary>
        /// Encodes material options code from parameterized options.
        /// Material options encoding is documented in SharedShape.SubObject() (Shapes.cs)
        /// or SceneryMaterial.SetState() (Materials.cs).
        /// </summary>
        /// <param name="lod">LODItem instance.</param>
        /// <returns>Options code.</returns>
        public static SceneryMaterialOptions EncodeMaterialOptions(LODItem lod)
        {
            var options = SceneryMaterialOptions.None;

            if (TextureAddressingModeNames.ContainsKey(lod.TexAddrModeName))
                options |= TextureAddressingModeNames[lod.TexAddrModeName];
            else
                Trace.TraceWarning("Skipped unknown texture addressing mode {1} in shape {0}", lod.Name, lod.TexAddrModeName);

            if (lod.AlphaTestMode == 1)
                options |= SceneryMaterialOptions.AlphaTest;

            if (ShaderNames.ContainsKey(lod.ShaderName))
                options |= ShaderNames[lod.ShaderName];
            else
                Trace.TraceWarning("Skipped unknown shader name {1} in shape {0}", lod.Name, lod.ShaderName);

            if (LightingModelNames.ContainsKey(lod.LightModelName))
                options |= LightingModelNames[lod.LightModelName];
            else
                Trace.TraceWarning("Skipped unknown lighting model index {1} in shape {0}", lod.Name, lod.LightModelName);

            if ((lod.ESD_Alternative_Texture & (int)TextureFlags.Night) != 0)
                options |= SceneryMaterialOptions.NightTexture;

            return options;
        } // end EncodeMaterialOptions
        */
    } // end class Helpers
}
