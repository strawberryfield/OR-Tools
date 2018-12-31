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

using Casasoft.ShapeViewerLib;
using Microsoft.Xna.Framework;
using Orts.Parsers.Msts;
using System;
using System.Collections;

namespace Orts.Viewer3D
{
    public class LOD
    {
        public float CutoffRadius; // Distance beyond which LODItem is not seen
        public ArrayList LODItems = new ArrayList(); // Array of arrays of LODItems
        public int PrimIndexStart; // Start index of ShapePrimitive block for this LOD
        public int PrimIndexStop;

        /// <summary>
        /// LOD class constructor
        /// </summary>
        /// <param name="cutoffRadius">Distance beyond which LODItem is not seen</param>
        public LOD(float cutoffRadius)
        {
            CutoffRadius = cutoffRadius;
        }

        public LOD(Viewer viewer, STFReader stf)
        {
            stf.MustMatch("(");
            stf.ParseBlock(new STFReader.TokenProcessor[] {
                new STFReader.TokenProcessor("cutoffradius", ()=>{ CutoffRadius = stf.ReadFloatBlock(STFReader.UNITS.Distance, null); }),
                new STFReader.TokenProcessor("loditem", ()=>{
                    LODItem lodItem = new LODItem(viewer, stf);
                    LODItems.Add(lodItem); // Append to Polylines array
                    }),
            });
            if (CutoffRadius == 0) throw new Exception("missing CutoffRadius");
        }

        public void Mark()
        {
            foreach (LODItem lodItem in LODItems)
                lodItem.Mark();
        }
    }

    public class LODItem
    {
        public ArrayList Polylines = new ArrayList();  // Array of arrays of vertices 

        public string Name;                            // e.g., "Rail sides"
        public string ShaderName;
        public string LightModelName;
        public int AlphaTestMode;
        public string TexAddrModeName;
        public int ESD_Alternative_Texture; // Equivalent to that of .sd file
        public float MipMapLevelOfDetailBias;

        public string TexName; // Texture file name

        public Material LODMaterial; // SceneryMaterial reference

        // NumVertices and NumSegments used for sizing vertex and index buffers
        public uint NumVertices;                     // Total independent vertices in LOD
        public uint NumSegments;                     // Total line segment count in LOD

        /// <summary>
        /// LODITem constructor (used for default and XML-style profiles)
        /// </summary>
        public LODItem(string name)
        {
            Name = name;
        }

        /// <summary>
        /// LODITem constructor (used for STF-style profile)
        /// </summary>
        public LODItem(Viewer viewer, STFReader stf)
        {
            stf.MustMatch("(");
            stf.ParseBlock(new STFReader.TokenProcessor[] {
                new STFReader.TokenProcessor("name", ()=>{ Name = stf.ReadStringBlock(null); }),
                new STFReader.TokenProcessor("texname", ()=>{ TexName = stf.ReadStringBlock(null); }),
                new STFReader.TokenProcessor("shadername", ()=>{ ShaderName = stf.ReadStringBlock(null); }),
                new STFReader.TokenProcessor("lightmodelname", ()=>{ LightModelName = stf.ReadStringBlock(null); }),
                new STFReader.TokenProcessor("alphatestmode", ()=>{ AlphaTestMode = stf.ReadIntBlock(null); }),
                new STFReader.TokenProcessor("texaddrmodename", ()=>{ TexAddrModeName = stf.ReadStringBlock(null); }),
                new STFReader.TokenProcessor("esd_alternative_texture", ()=>{ ESD_Alternative_Texture = stf.ReadIntBlock(null); }),
                new STFReader.TokenProcessor("mipmaplevelofdetailbias", ()=>{ MipMapLevelOfDetailBias = stf.ReadFloatBlock(STFReader.UNITS.None, null); }),
                new STFReader.TokenProcessor("polyline", ()=>{
                    Polyline pl = new Polyline(stf);
                    Polylines.Add(pl); // Append to Polylines array
                    //parent.Accum(pl.Vertices.Count); }),
                    Accum(pl.Vertices.Count); }),
            });

            // Checks for required member variables:
            // Name not required.
            // MipMapLevelOfDetail bias initializes to 0.
            if (Polylines.Count == 0) throw new Exception("missing Polylines");

            LoadMaterial(viewer, this);
        }

        public void Accum(int count)
        {
            // Accumulates total independent vertices and total line segments
            // Used for sizing of vertex and index buffers
            NumVertices += (uint)count;
            NumSegments += (uint)count - 1;
        }

        public static void LoadMaterial(Viewer viewer, LODItem lod)
        {
            var options = Helpers.EncodeMaterialOptions(lod);
            lod.LODMaterial = viewer.MaterialManager.Load("Scenery", Helpers.GetRouteTextureFile(viewer.Simulator, (Helpers.TextureFlags)lod.ESD_Alternative_Texture, lod.TexName), (int)options, lod.MipMapLevelOfDetailBias);
        }

        public void Mark()
        {
            LODMaterial.Mark();
        }
    }

    public class Polyline
    {
        public ArrayList Vertices = new ArrayList();    // Array of vertices 

        public string Name;                             // e.g., "1:1 embankment"
        public Vector2 DeltaTexCoord;                   // Incremental change in (u, v) from one cross section to the next

        /// <summary>
        /// Polyline constructor (DAT)
        /// </summary>
        public Polyline(STFReader stf)
        {
            stf.MustMatch("(");
            stf.ParseBlock(new STFReader.TokenProcessor[] {
                new STFReader.TokenProcessor("name", ()=>{ Name = stf.ReadStringBlock(null); }),
                new STFReader.TokenProcessor("vertex", ()=>{ Vertices.Add(new Vertex(stf)); }),
                new STFReader.TokenProcessor("deltatexcoord", ()=>{
                    stf.MustMatch("(");
                    DeltaTexCoord.X = stf.ReadFloat(STFReader.UNITS.None, null);
                    DeltaTexCoord.Y = stf.ReadFloat(STFReader.UNITS.None, null);
                    stf.SkipRestOfBlock();
                }),
            });
            // Checks for required member variables: 
            // Name not required.
            if (DeltaTexCoord == Vector2.Zero) throw new Exception("missing DeltaTexCoord");
            if (Vertices.Count == 0) throw new Exception("missing Vertices");
        }

        /// <summary>
        /// Bare-bones Polyline constructor (used for XML)
        /// </summary>
        public Polyline()
        {
        }

    }

    public struct Vertex
    {
        public Vector3 Position;                           // Position vector (x, y, z)
        public Vector3 Normal;                             // Normal vector (nx, ny, nz)
        public Vector2 TexCoord;                           // Texture coordinate (u, v)

        // Vertex constructor (default)
        public Vertex(float x, float y, float z, float nx, float ny, float nz, float u, float v)
        {
            Position = new Vector3(x, y, z);
            Normal = new Vector3(nx, ny, nz);
            TexCoord = new Vector2(u, v);
        }

        // Vertex constructor (DAT)
        public Vertex(STFReader stf)
        {
            Vertex v = new Vertex(); // Temp variable used to construct the struct in ParseBlock
            v.Position = new Vector3();
            v.Normal = new Vector3();
            v.TexCoord = new Vector2();
            stf.MustMatch("(");
            stf.ParseBlock(new STFReader.TokenProcessor[] {
                new STFReader.TokenProcessor("position", ()=>{
                    stf.MustMatch("(");
                    v.Position.X = stf.ReadFloat(STFReader.UNITS.None, null);
                    v.Position.Y = stf.ReadFloat(STFReader.UNITS.None, null);
                    v.Position.Z = 0.0f;
                    stf.SkipRestOfBlock();
                }),
                new STFReader.TokenProcessor("normal", ()=>{
                    stf.MustMatch("(");
                    v.Normal.X = stf.ReadFloat(STFReader.UNITS.None, null);
                    v.Normal.Y = stf.ReadFloat(STFReader.UNITS.None, null);
                    v.Normal.Z = stf.ReadFloat(STFReader.UNITS.None, null);
                    stf.SkipRestOfBlock();
                }),
                new STFReader.TokenProcessor("texcoord", ()=>{
                    stf.MustMatch("(");
                    v.TexCoord.X = stf.ReadFloat(STFReader.UNITS.None, null);
                    v.TexCoord.Y = stf.ReadFloat(STFReader.UNITS.None, null);
                    stf.SkipRestOfBlock();
                }),
            });
            this = v;
            // Checks for required member variables
            // No way to check for missing Position.
            if (Normal == Vector3.Zero) throw new Exception("improper Normal");
            // No way to check for missing TexCoord
        }
    }

}
