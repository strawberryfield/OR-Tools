﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Orts.Formats.Msts;
using Orts.Viewer3D.Common;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Orts.Viewer3D.Common.Helpers;
using Orts.Viewer3D;

namespace ShapeViewer
{

/*
    public class Material {
        public Viewer Viewer;
    }
*/
    public abstract class RenderPrimitive
    {
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


    public class ShapePrimitive : RenderPrimitive
    {
        public Material Material { get; protected set; }
        public int[] Hierarchy { get; protected set; } // the hierarchy from the sub_object
        public int HierarchyIndex { get; protected set; } // index into the hiearchy array which provides pose for this primitive

        protected internal VertexBuffer VertexBuffer;
        protected internal IndexBuffer IndexBuffer;
        protected internal int MinVertexIndex;
        protected internal int NumVerticies;
        protected internal int PrimitiveCount;
        protected internal VertexBufferBinding[] VertexBufferBindings;


        public ShapePrimitive()
        {
        }

        
                public ShapePrimitive(Material material, SharedShape.VertexBufferSet vertexBufferSet, IndexBuffer indexBuffer, int minVertexIndex, int numVerticies, int primitiveCount, int[] hierarchy, int hierarchyIndex)
                {
                    Material = material;
                    VertexBuffer = vertexBufferSet.Buffer;
                    IndexBuffer = indexBuffer;
                    MinVertexIndex = minVertexIndex;
                    NumVerticies = numVerticies;
                    PrimitiveCount = primitiveCount;
                    Hierarchy = hierarchy;
                    HierarchyIndex = hierarchyIndex;

                    DummyVertexBuffer = new VertexBuffer(material.Viewer.GraphicsDevice, DummyVertexDeclaration, 1, BufferUsage.WriteOnly);
                    DummyVertexBuffer.SetData(DummyVertexData);
                    VertexBufferBindings = new[] { new VertexBufferBinding(VertexBuffer), new VertexBufferBinding(DummyVertexBuffer) };
                }

                public ShapePrimitive(Material material, SharedShape.VertexBufferSet vertexBufferSet, List<ushort> indexData, GraphicsDevice graphicsDevice, int[] hierarchy, int hierarchyIndex)
                    : this(material, vertexBufferSet, null, indexData.Min(), indexData.Max() - indexData.Min() + 1, indexData.Count / 3, hierarchy, hierarchyIndex)
                {
                    IndexBuffer = new IndexBuffer(graphicsDevice, typeof(short), indexData.Count, BufferUsage.WriteOnly);
                    IndexBuffer.SetData(indexData.ToArray());
                }
        

        public class VertexBufferSet
        {
            public VertexBuffer Buffer;

            public VertexBufferSet(VertexPositionNormalTexture[] vertexData, GraphicsDevice graphicsDevice)
            {
                Buffer = new VertexBuffer(graphicsDevice, typeof(VertexPositionNormalTexture), vertexData.Length, BufferUsage.WriteOnly);
                Buffer.SetData(vertexData);
            }


            public VertexBufferSet(sub_object sub_object, ShapeFile sFile, GraphicsDevice graphicsDevice)
                : this(CreateVertexData(sub_object, sFile.shape), graphicsDevice)
            {
            }

            static VertexPositionNormalTexture[] CreateVertexData(sub_object sub_object, shape shape)
            {
                // TODO - deal with vertex sets that have various numbers of texture coordinates - ie 0, 1, 2 etc
                return (from vertex vertex in sub_object.vertices
                        select XNAVertexPositionNormalTextureFromMSTS(vertex, shape)).ToArray();
            }

            static VertexPositionNormalTexture XNAVertexPositionNormalTextureFromMSTS(vertex vertex, shape shape)
            {
                var position = shape.points[vertex.ipoint];
                var normal = shape.normals[vertex.inormal];
                // TODO use a simpler vertex description when no UV's in use
                var texcoord = vertex.vertex_uvs.Length > 0 ? shape.uv_points[vertex.vertex_uvs[0]] : new uv_point(0, 0);

                return new VertexPositionNormalTexture()
                {
                    Position = new Vector3(position.X, position.Y, -position.Z),
                    Normal = new Vector3(normal.X, normal.Y, -normal.Z),
                    TextureCoordinate = new Vector2(texcoord.U, texcoord.V),
                };
            }

        }

        static Matrix XNAMatrixFromMSTS(matrix MSTSMatrix)
        {
            var XNAMatrix = Matrix.Identity;

            XNAMatrix.M11 = MSTSMatrix.AX;
            XNAMatrix.M12 = MSTSMatrix.AY;
            XNAMatrix.M13 = -MSTSMatrix.AZ;
            XNAMatrix.M21 = MSTSMatrix.BX;
            XNAMatrix.M22 = MSTSMatrix.BY;
            XNAMatrix.M23 = -MSTSMatrix.BZ;
            XNAMatrix.M31 = -MSTSMatrix.CX;
            XNAMatrix.M32 = -MSTSMatrix.CY;
            XNAMatrix.M33 = MSTSMatrix.CZ;
            XNAMatrix.M41 = MSTSMatrix.DX;
            XNAMatrix.M42 = MSTSMatrix.DY;
            XNAMatrix.M43 = -MSTSMatrix.DZ;

            return XNAMatrix;
        }


        public override void Draw(GraphicsDevice graphicsDevice)
        {
            if (PrimitiveCount > 0)
            {
                // TODO consider sorting by Vertex set so we can reduce the number of SetSources required.
                graphicsDevice.SetVertexBuffers(VertexBufferBindings);
                graphicsDevice.Indices = IndexBuffer;
                graphicsDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, PrimitiveCount);
            }
        }

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

    public class ShapePrimitiveInstances : RenderPrimitive
    {
        public Material Material { get; protected set; }
        public int[] Hierarchy { get; protected set; } // the hierarchy from the sub_object
        public int HierarchyIndex { get; protected set; } // index into the hiearchy array which provides pose for this primitive
        public int SubObjectIndex { get; protected set; }

        protected VertexBuffer VertexBuffer;
        protected VertexDeclaration VertexDeclaration;
        protected int VertexBufferStride;
        protected IndexBuffer IndexBuffer;
        protected int MinVertexIndex;
        protected int NumVerticies;
        protected int PrimitiveCount;

        protected VertexBuffer InstanceBuffer;
        protected VertexDeclaration InstanceDeclaration;
        protected int InstanceBufferStride;
        protected int InstanceCount;
        protected VertexBufferBinding[] VertexBufferBindings;

        internal ShapePrimitiveInstances(GraphicsDevice graphicsDevice, ShapePrimitive shapePrimitive, Matrix[] positions, int subObjectIndex)
        {
            Material = shapePrimitive.Material;
            Hierarchy = shapePrimitive.Hierarchy;
            HierarchyIndex = shapePrimitive.HierarchyIndex;
            SubObjectIndex = subObjectIndex;
            VertexBuffer = shapePrimitive.VertexBuffer;
            VertexDeclaration = shapePrimitive.VertexBuffer.VertexDeclaration;
            IndexBuffer = shapePrimitive.IndexBuffer;
            MinVertexIndex = shapePrimitive.MinVertexIndex;
            NumVerticies = shapePrimitive.NumVerticies;
            PrimitiveCount = shapePrimitive.PrimitiveCount;

            InstanceDeclaration = new VertexDeclaration(ShapeInstanceData.SizeInBytes, ShapeInstanceData.VertexElements);
            InstanceBuffer = new VertexBuffer(graphicsDevice, InstanceDeclaration, positions.Length, BufferUsage.WriteOnly);
            InstanceBuffer.SetData(positions);
            InstanceCount = positions.Length;

            VertexBufferBindings = new[] { new VertexBufferBinding(VertexBuffer), new VertexBufferBinding(InstanceBuffer, 0, 1) };
        }

        public override void Draw(GraphicsDevice graphicsDevice)
        {
            graphicsDevice.Indices = IndexBuffer;
            graphicsDevice.SetVertexBuffers(VertexBufferBindings);
            graphicsDevice.DrawInstancedPrimitives(PrimitiveType.TriangleList, 0, MinVertexIndex, NumVerticies, 0, PrimitiveCount, InstanceCount);
        }
    }

    public class SharedShape
    {
        static List<string> ShapeWarnings = new List<string>();

        // This data is common to all instances of the shape
        public List<string> MatrixNames = new List<string>();
        public Matrix[] Matrices = new Matrix[0];  // the original natural pose for this shape - shared by all instances
        public animations Animations;
        public LodControl[] LodControls;
        public bool HasNightSubObj;
        public int RootSubObjectIndex = 0;
        //public bool negativeBogie = false;
        public string SoundFileName = "";


        readonly Viewer Viewer;
        public readonly string FilePath;
        public readonly string ReferencePath;

        /// <summary>
        /// Create an empty shape used as a sub when the shape won't load
        /// </summary>
        /// <param name="viewer"></param>
        public SharedShape(Viewer viewer)
        {
            Viewer = viewer;
            FilePath = "Empty";
            LodControls = new LodControl[0];
        }

        /// <summary>
        /// MSTS shape from shape file
        /// </summary>
        /// <param name="viewer"></param>
        /// <param name="filePath">Path to shape's S file</param>
        public SharedShape(Viewer viewer, string filePath)
        {
            Viewer = viewer;
            FilePath = filePath;
            if (filePath.Contains('\0'))
            {
                var parts = filePath.Split('\0');
                FilePath = parts[0];
                ReferencePath = parts[1];
            }
            LoadContent();
        }

        /// <summary>
        /// Only one copy of the model is loaded regardless of how many copies are placed in the scene.
        /// </summary>
        void LoadContent()
        {
            Trace.Write("S");
            var filePath = FilePath;
            // commented lines allow reading the animation block from an additional file in an Openrails subfolder
            //           string dir = Path.GetDirectoryName(filePath);
            //            string file = Path.GetFileName(filePath);
            //            string orFilePath = dir + @"\openrails\" + file;
            var sFile = new ShapeFile(filePath, true);
            //            if (file.ToLower().Contains("turntable") && File.Exists(orFilePath))
            //            {
            //                sFile.ReadAnimationBlock(orFilePath);
            //            }


            var textureFlags = Helpers.TextureFlags.None;
            if (File.Exists(FilePath + "d"))
            {
                var sdFile = new ShapeDescriptorFile(FilePath + "d");
                textureFlags = (Helpers.TextureFlags)sdFile.shape.ESD_Alternative_Texture;
                if (FilePath != null && FilePath.Contains("\\global\\")) textureFlags |= Helpers.TextureFlags.SnowTrack;//roads and tracks are in global, as MSTS will always use snow texture in snow weather
                HasNightSubObj = sdFile.shape.ESD_SubObj;
                if ((textureFlags & Helpers.TextureFlags.Night) != 0 && FilePath.Contains("\\trainset\\"))
                    textureFlags |= Helpers.TextureFlags.Underground;
                SoundFileName = sdFile.shape.ESD_SoundFileName;
            }

            var matrixCount = sFile.shape.matrices.Count;
            MatrixNames.Capacity = matrixCount;
            Matrices = new Matrix[matrixCount];
            for (var i = 0; i < matrixCount; ++i)
            {
                MatrixNames.Add(sFile.shape.matrices[i].Name.ToUpper());
                Matrices[i] = XNAMatrixFromMSTS(sFile.shape.matrices[i]);
            }
            Animations = sFile.shape.animations;

#if DEBUG_SHAPE_HIERARCHY
            var debugShapeHierarchy = new StringBuilder();
            debugShapeHierarchy.AppendFormat("Shape {0}:\n", Path.GetFileNameWithoutExtension(FilePath).ToUpper());
            for (var i = 0; i < MatrixNames.Count; ++i)
                debugShapeHierarchy.AppendFormat("  Matrix {0,-2}: {1}\n", i, MatrixNames[i]);
            for (var i = 0; i < sFile.shape.prim_states.Count; ++i)
                debugShapeHierarchy.AppendFormat("  PState {0,-2}: flags={1,-8:X8} shader={2,-15} alpha={3,-2} vstate={4,-2} lstate={5,-2} zbias={6,-5:F3} zbuffer={7,-2} name={8}\n", i, sFile.shape.prim_states[i].flags, sFile.shape.shader_names[sFile.shape.prim_states[i].ishader], sFile.shape.prim_states[i].alphatestmode, sFile.shape.prim_states[i].ivtx_state, sFile.shape.prim_states[i].LightCfgIdx, sFile.shape.prim_states[i].ZBias, sFile.shape.prim_states[i].ZBufMode, sFile.shape.prim_states[i].Name);
            for (var i = 0; i < sFile.shape.vtx_states.Count; ++i)
                debugShapeHierarchy.AppendFormat("  VState {0,-2}: flags={1,-8:X8} lflags={2,-8:X8} lstate={3,-2} material={4,-3} matrix2={5,-2}\n", i, sFile.shape.vtx_states[i].flags, sFile.shape.vtx_states[i].LightFlags, sFile.shape.vtx_states[i].LightCfgIdx, sFile.shape.vtx_states[i].LightMatIdx, sFile.shape.vtx_states[i].Matrix2);
            for (var i = 0; i < sFile.shape.light_model_cfgs.Count; ++i)
            {
                debugShapeHierarchy.AppendFormat("  LState {0,-2}: flags={1,-8:X8} uv_ops={2,-2}\n", i, sFile.shape.light_model_cfgs[i].flags, sFile.shape.light_model_cfgs[i].uv_ops.Count);
                for (var j = 0; j < sFile.shape.light_model_cfgs[i].uv_ops.Count; ++j)
                    debugShapeHierarchy.AppendFormat("    UV OP {0,-2}: texture_address_mode={1,-2}\n", j, sFile.shape.light_model_cfgs[i].uv_ops[j].TexAddrMode);
            }
            Console.Write(debugShapeHierarchy.ToString());
#endif
            LodControls = (from lod_control lod in sFile.shape.lod_controls
                           select new LodControl(lod, textureFlags, sFile, this)).ToArray();
            if (LodControls.Length == 0)
                throw new InvalidDataException("Shape file missing lod_control section");
            else if (LodControls[0].DistanceLevels.Length > 0 && LodControls[0].DistanceLevels[0].SubObjects.Length > 0)
            {
                // Look for root subobject, it is not necessarily the first (see ProTrain signal)
                for (int soIndex = 0; soIndex <= LodControls[0].DistanceLevels[0].SubObjects.Length - 1; soIndex++)
                {
                    sub_object subObject = sFile.shape.lod_controls[0].distance_levels[0].sub_objects[soIndex];
                    if (subObject.sub_object_header.geometry_info.geometry_node_map[0] == 0)
                    {
                        RootSubObjectIndex = soIndex;
                        break;
                    }
                }
            }
        }

        public class LodControl
        {
            public DistanceLevel[] DistanceLevels;

            public LodControl(lod_control MSTSlod_control, Helpers.TextureFlags textureFlags, ShapeFile sFile, SharedShape sharedShape)
            {
#if DEBUG_SHAPE_HIERARCHY
                Console.WriteLine("  LOD control:");
#endif
                DistanceLevels = (from distance_level level in MSTSlod_control.distance_levels
                                  select new DistanceLevel(level, textureFlags, sFile, sharedShape)).ToArray();
                if (DistanceLevels.Length == 0)
                    throw new InvalidDataException("Shape file missing distance_level");
            }

            internal void Mark()
            {
                foreach (var dl in DistanceLevels)
                    dl.Mark();
            }
        }

        public class DistanceLevel
        {
            public float ViewingDistance;
            public float ViewSphereRadius;
            public SubObject[] SubObjects;

            public DistanceLevel(distance_level MSTSdistance_level, Helpers.TextureFlags textureFlags, ShapeFile sFile, SharedShape sharedShape)
            {
#if DEBUG_SHAPE_HIERARCHY
                Console.WriteLine("    Distance level {0}: hierarchy={1}", MSTSdistance_level.distance_level_header.dlevel_selection, String.Join(" ", MSTSdistance_level.distance_level_header.hierarchy.Select(i => i.ToString()).ToArray()));
#endif
                ViewingDistance = MSTSdistance_level.distance_level_header.dlevel_selection;
                // TODO, work out ViewShereRadius from all sub_object radius and centers.
                if (sFile.shape.volumes.Count > 0)
                    ViewSphereRadius = sFile.shape.volumes[0].Radius;
                else
                    ViewSphereRadius = 100;

                var index = 0;
#if DEBUG_SHAPE_HIERARCHY
                var subObjectIndex = 0;
                SubObjects = (from sub_object obj in MSTSdistance_level.sub_objects
                              select new SubObject(obj, ref index, MSTSdistance_level.distance_level_header.hierarchy, textureFlags, subObjectIndex++, sFile, sharedShape)).ToArray();
#else
                SubObjects = (from sub_object obj in MSTSdistance_level.sub_objects
                              select new SubObject(obj, ref index, MSTSdistance_level.distance_level_header.hierarchy, textureFlags, sFile, sharedShape)).ToArray();
#endif
                if (SubObjects.Length == 0)
                    throw new InvalidDataException("Shape file missing sub_object");
            }

            //            [CallOnThread("Loader")]
            internal void Mark()
            {
                foreach (var so in SubObjects)
                    so.Mark();
            }
        }

        public class SubObject
        {
            static readonly SceneryMaterialOptions[] UVTextureAddressModeMap = new[] {
                SceneryMaterialOptions.TextureAddressModeWrap,
                SceneryMaterialOptions.TextureAddressModeMirror,
                SceneryMaterialOptions.TextureAddressModeClamp,
            };

            static readonly Dictionary<string, SceneryMaterialOptions> ShaderNames = new Dictionary<string, SceneryMaterialOptions> {
                { "Tex", SceneryMaterialOptions.ShaderFullBright },
                { "TexDiff", SceneryMaterialOptions.Diffuse },
                { "BlendATex", SceneryMaterialOptions.AlphaBlendingBlend | SceneryMaterialOptions.ShaderFullBright},
                { "BlendATexDiff", SceneryMaterialOptions.AlphaBlendingBlend | SceneryMaterialOptions.Diffuse },
                { "AddATex", SceneryMaterialOptions.AlphaBlendingAdd | SceneryMaterialOptions.ShaderFullBright},
                { "AddATexDiff", SceneryMaterialOptions.AlphaBlendingAdd | SceneryMaterialOptions.Diffuse },
            };

            static readonly SceneryMaterialOptions[] VertexLightModeMap = new[] {
                SceneryMaterialOptions.ShaderDarkShade,
                SceneryMaterialOptions.ShaderHalfBright,
                SceneryMaterialOptions.ShaderVegetation, // Not certain this is right.
                SceneryMaterialOptions.ShaderVegetation,
                SceneryMaterialOptions.ShaderFullBright,
                SceneryMaterialOptions.None | SceneryMaterialOptions.Specular750,
                SceneryMaterialOptions.None | SceneryMaterialOptions.Specular25,
                SceneryMaterialOptions.None | SceneryMaterialOptions.None,
            };

            public ShapePrimitive[] ShapePrimitives;

#if DEBUG_SHAPE_HIERARCHY
            public SubObject(sub_object sub_object, ref int totalPrimitiveIndex, int[] hierarchy, Helpers.TextureFlags textureFlags, int subObjectIndex, SFile sFile, SharedShape sharedShape)
#else
            public SubObject(sub_object sub_object, ref int totalPrimitiveIndex, int[] hierarchy, Helpers.TextureFlags textureFlags, ShapeFile sFile, SharedShape sharedShape)
#endif
            {
#if DEBUG_SHAPE_HIERARCHY
                var debugShapeHierarchy = new StringBuilder();
                debugShapeHierarchy.AppendFormat("      Sub object {0}:\n", subObjectIndex);
#endif
                var vertexBufferSet = new VertexBufferSet(sub_object, sFile, sharedShape.Viewer.GraphicsDevice);
#if DEBUG_SHAPE_NORMALS
                var debugNormalsMaterial = sharedShape.Viewer.MaterialManager.Load("DebugNormals");
#endif

#if OPTIMIZE_SHAPES_ON_LOAD
                var primitiveMaterials = sub_object.primitives.Cast<primitive>().Select((primitive) =>
#else
                var primitiveIndex = 0;
#if DEBUG_SHAPE_NORMALS
                ShapePrimitives = new ShapePrimitive[sub_object.primitives.Count * 2];
#else
                ShapePrimitives = new ShapePrimitive[sub_object.primitives.Count];
#endif
                foreach (primitive primitive in sub_object.primitives)
#endif
                {
                    var primitiveState = sFile.shape.prim_states[primitive.prim_state_idx];
                    var vertexState = sFile.shape.vtx_states[primitiveState.ivtx_state];
                    var lightModelConfiguration = sFile.shape.light_model_cfgs[vertexState.LightCfgIdx];
                    var options = SceneryMaterialOptions.None;

                    // Validate hierarchy position.
                    var hierarchyIndex = vertexState.imatrix;
                    while (hierarchyIndex != -1)
                    {
                        if (hierarchyIndex < 0 || hierarchyIndex >= hierarchy.Length)
                        {
                            var hierarchyList = new List<int>();
                            hierarchyIndex = vertexState.imatrix;
                            while (hierarchyIndex >= 0 && hierarchyIndex < hierarchy.Length)
                            {
                                hierarchyList.Add(hierarchyIndex);
                                hierarchyIndex = hierarchy[hierarchyIndex];
                            }
                            hierarchyList.Add(hierarchyIndex);
                            Trace.TraceWarning("Ignored invalid primitive hierarchy {1} in shape {0}", sharedShape.FilePath, string.Join(" ", hierarchyList.Select(hi => hi.ToString()).ToArray()));
                            break;
                        }
                        hierarchyIndex = hierarchy[hierarchyIndex];
                    }

                    if (lightModelConfiguration.uv_ops.Count > 0)
                        if (lightModelConfiguration.uv_ops[0].TexAddrMode - 1 >= 0 && lightModelConfiguration.uv_ops[0].TexAddrMode - 1 < UVTextureAddressModeMap.Length)
                            options |= UVTextureAddressModeMap[lightModelConfiguration.uv_ops[0].TexAddrMode - 1];
                        else if (!ShapeWarnings.Contains("texture_addressing_mode:" + lightModelConfiguration.uv_ops[0].TexAddrMode))
                        {
                            Trace.TraceInformation("Skipped unknown texture addressing mode {1} first seen in shape {0}", sharedShape.FilePath, lightModelConfiguration.uv_ops[0].TexAddrMode);
                            ShapeWarnings.Add("texture_addressing_mode:" + lightModelConfiguration.uv_ops[0].TexAddrMode);
                        }

                    if (primitiveState.alphatestmode == 1)
                        options |= SceneryMaterialOptions.AlphaTest;

                    if (ShaderNames.ContainsKey(sFile.shape.shader_names[primitiveState.ishader]))
                        options |= ShaderNames[sFile.shape.shader_names[primitiveState.ishader]];
                    else if (!ShapeWarnings.Contains("shader_name:" + sFile.shape.shader_names[primitiveState.ishader]))
                    {
                        Trace.TraceInformation("Skipped unknown shader name {1} first seen in shape {0}", sharedShape.FilePath, sFile.shape.shader_names[primitiveState.ishader]);
                        ShapeWarnings.Add("shader_name:" + sFile.shape.shader_names[primitiveState.ishader]);
                    }

                    if (12 + vertexState.LightMatIdx >= 0 && 12 + vertexState.LightMatIdx < VertexLightModeMap.Length)
                        options |= VertexLightModeMap[12 + vertexState.LightMatIdx];
                    else if (!ShapeWarnings.Contains("lighting_model:" + vertexState.LightMatIdx))
                    {
                        Trace.TraceInformation("Skipped unknown lighting model index {1} first seen in shape {0}", sharedShape.FilePath, vertexState.LightMatIdx);
                        ShapeWarnings.Add("lighting_model:" + vertexState.LightMatIdx);
                    }

                    if ((textureFlags & Helpers.TextureFlags.Night) != 0)
                        options |= SceneryMaterialOptions.NightTexture;

                    if ((textureFlags & Helpers.TextureFlags.Underground) != 0)
                        options |= SceneryMaterialOptions.UndergroundTexture;

                    Material material;

                    if (primitiveState.tex_idxs.Length != 0)
                    {
                        var texture = sFile.shape.textures[primitiveState.tex_idxs[0]];
                        var imageName = sFile.shape.images[texture.iImage];
                        if (string.IsNullOrEmpty(sharedShape.ReferencePath))
                            material = sharedShape.Viewer.MaterialManager.Load("Scenery", Helpers.GetRouteTextureFile(sharedShape.Viewer.Simulator, textureFlags, imageName), (int)options, texture.MipMapLODBias);
                        else
                            material = sharedShape.Viewer.MaterialManager.Load("Scenery", Helpers.GetTextureFile(sharedShape.Viewer.Simulator, textureFlags, sharedShape.ReferencePath, imageName), (int)options, texture.MipMapLODBias);
                    }
                    else
                    {
                        material = sharedShape.Viewer.MaterialManager.Load("Scenery", null, (int)options);
                    }

#if DEBUG_SHAPE_HIERARCHY
                    debugShapeHierarchy.AppendFormat("        Primitive {0,-2}: pstate={1,-2} vstate={2,-2} lstate={3,-2} matrix={4,-2}", primitiveIndex, primitive.prim_state_idx, primitiveState.ivtx_state, vertexState.LightCfgIdx, vertexState.imatrix);
                    var debugMatrix = vertexState.imatrix;
                    while (debugMatrix >= 0)
                    {
                        debugShapeHierarchy.AppendFormat(" {0}", sharedShape.MatrixNames[debugMatrix]);
                        debugMatrix = hierarchy[debugMatrix];
                    }
                    debugShapeHierarchy.Append("\n");
#endif

#if OPTIMIZE_SHAPES_ON_LOAD
                    return new { Key = material.ToString() + "/" + vertexState.imatrix.ToString(), Primitive = primitive, Material = material, HierachyIndex = vertexState.imatrix };
                }).ToArray();
#else
                    if (primitive.indexed_trilist.vertex_idxs.Count == 0)
                    {
                        Trace.TraceWarning("Skipped primitive with 0 indices in {0}", sharedShape.FilePath);
                        continue;
                    }

                    var indexData = new List<ushort>(primitive.indexed_trilist.vertex_idxs.Count * 3);
                    foreach (vertex_idx vertex_idx in primitive.indexed_trilist.vertex_idxs)
                        foreach (var index in new[] { vertex_idx.a, vertex_idx.b, vertex_idx.c })
                            indexData.Add((ushort)index);

                    ShapePrimitives[primitiveIndex] = new ShapePrimitive(material, vertexBufferSet, indexData, sharedShape.Viewer.GraphicsDevice, hierarchy, vertexState.imatrix);
                    ShapePrimitives[primitiveIndex].SortIndex = ++totalPrimitiveIndex;
                    ++primitiveIndex;
#if DEBUG_SHAPE_NORMALS
                    ShapePrimitives[primitiveIndex] = new ShapeDebugNormalsPrimitive(debugNormalsMaterial, vertexBufferSet, indexData, sharedShape.Viewer.GraphicsDevice, hierarchy, vertexState.imatrix);
                    ShapePrimitives[primitiveIndex].SortIndex = totalPrimitiveIndex;
                    ++primitiveIndex;
#endif
                }
#endif

#if OPTIMIZE_SHAPES_ON_LOAD
                var indexes = new Dictionary<string, List<short>>(sub_object.primitives.Count);
                foreach (var primitiveMaterial in primitiveMaterials)
                {
                    var baseIndex = 0;
                    var indexData = new List<short>(0);
                    if (indexes.TryGetValue(primitiveMaterial.Key, out indexData))
                    {
                        baseIndex = indexData.Count;
                        indexData.Capacity += primitiveMaterial.Primitive.indexed_trilist.vertex_idxs.Count * 3;
                    }
                    else
                    {
                        indexData = new List<short>(primitiveMaterial.Primitive.indexed_trilist.vertex_idxs.Count * 3);
                        indexes.Add(primitiveMaterial.Key, indexData);
                    }

                    var primitiveState = sFile.shape.prim_states[primitiveMaterial.Primitive.prim_state_idx];
                    foreach (vertex_idx vertex_idx in primitiveMaterial.Primitive.indexed_trilist.vertex_idxs)
                    {
                        indexData.Add((short)vertex_idx.a);
                        indexData.Add((short)vertex_idx.b);
                        indexData.Add((short)vertex_idx.c);
                    }
                }

                ShapePrimitives = new ShapePrimitive[indexes.Count];
                var primitiveIndex = 0;
                foreach (var index in indexes)
                {
                    var indexBuffer = new IndexBuffer(sharedShape.Viewer.GraphicsDevice, typeof(short), index.Value.Count, BufferUsage.WriteOnly);
                    indexBuffer.SetData(index.Value.ToArray());
                    var primitiveMaterial = primitiveMaterials.First(d => d.Key == index.Key);
                    ShapePrimitives[primitiveIndex] = new ShapePrimitive(primitiveMaterial.Material, vertexBufferSet, indexBuffer, index.Value.Min(), index.Value.Max() - index.Value.Min() + 1, index.Value.Count / 3, hierarchy, primitiveMaterial.HierachyIndex);
                    ++primitiveIndex;
                }
                if (sub_object.primitives.Count != indexes.Count)
                    Trace.TraceInformation("{1} -> {2} primitives in {0}", sharedShape.FilePath, sub_object.primitives.Count, indexes.Count);
#else
                if (primitiveIndex < ShapePrimitives.Length)
                    ShapePrimitives = ShapePrimitives.Take(primitiveIndex).ToArray();
#endif

#if DEBUG_SHAPE_HIERARCHY
                Console.Write(debugShapeHierarchy.ToString());
#endif
            }

            //            [CallOnThread("Loader")]
            internal void Mark()
            {
                foreach (var prim in ShapePrimitives) ;
                //    prim.Mark();
            }
        }

        public class VertexBufferSet
        {
            public VertexBuffer Buffer;

#if DEBUG_SHAPE_NORMALS
            public VertexBuffer DebugNormalsBuffer;
            public VertexDeclaration DebugNormalsDeclaration;
            public int DebugNormalsVertexCount;
            public const int DebugNormalsVertexPerVertex = 3 * 4;
#endif

            public VertexBufferSet(VertexPositionNormalTexture[] vertexData, GraphicsDevice graphicsDevice)
            {
                Buffer = new VertexBuffer(graphicsDevice, typeof(VertexPositionNormalTexture), vertexData.Length, BufferUsage.WriteOnly);
                Buffer.SetData(vertexData);
            }

#if DEBUG_SHAPE_NORMALS
            public VertexBufferSet(VertexPositionNormalTexture[] vertexData, VertexPositionColor[] debugNormalsVertexData, GraphicsDevice graphicsDevice)
                :this(vertexData, graphicsDevice)
            {
                DebugNormalsVertexCount = debugNormalsVertexData.Length;
                DebugNormalsDeclaration = new VertexDeclaration(graphicsDevice, VertexPositionColor.VertexElements);
                DebugNormalsBuffer = new VertexBuffer(graphicsDevice, typeof(VertexPositionColor), DebugNormalsVertexCount, BufferUsage.WriteOnly);
                DebugNormalsBuffer.SetData(debugNormalsVertexData);
            }
#endif

            public VertexBufferSet(sub_object sub_object, ShapeFile sFile, GraphicsDevice graphicsDevice)
#if DEBUG_SHAPE_NORMALS
                : this(CreateVertexData(sub_object, sFile.shape), CreateDebugNormalsVertexData(sub_object, sFile.shape), graphicsDevice)
#else
                : this(CreateVertexData(sub_object, sFile.shape), graphicsDevice)
#endif
            {
            }

            static VertexPositionNormalTexture[] CreateVertexData(sub_object sub_object, shape shape)
            {
                // TODO - deal with vertex sets that have various numbers of texture coordinates - ie 0, 1, 2 etc
                return (from vertex vertex in sub_object.vertices
                        select XNAVertexPositionNormalTextureFromMSTS(vertex, shape)).ToArray();
            }

            static VertexPositionNormalTexture XNAVertexPositionNormalTextureFromMSTS(vertex vertex, shape shape)
            {
                var position = shape.points[vertex.ipoint];
                var normal = shape.normals[vertex.inormal];
                // TODO use a simpler vertex description when no UV's in use
                var texcoord = vertex.vertex_uvs.Length > 0 ? shape.uv_points[vertex.vertex_uvs[0]] : new uv_point(0, 0);

                return new VertexPositionNormalTexture()
                {
                    Position = new Vector3(position.X, position.Y, -position.Z),
                    Normal = new Vector3(normal.X, normal.Y, -normal.Z),
                    TextureCoordinate = new Vector2(texcoord.U, texcoord.V),
                };
            }

#if DEBUG_SHAPE_NORMALS
            static VertexPositionColor[] CreateDebugNormalsVertexData(sub_object sub_object, shape shape)
            {
                var vertexData = new List<VertexPositionColor>();
                foreach (vertex vertex in sub_object.vertices)
                {
                    var position = new Vector3(shape.points[vertex.ipoint].X, shape.points[vertex.ipoint].Y, -shape.points[vertex.ipoint].Z);
                    var normal = new Vector3(shape.normals[vertex.inormal].X, shape.normals[vertex.inormal].Y, -shape.normals[vertex.inormal].Z);
                    var right = Vector3.Cross(normal, Math.Abs(normal.Y) > 0.5 ? Vector3.Left : Vector3.Up);
                    var up = Vector3.Cross(normal, right);
                    right /= 50;
                    up /= 50;
                    vertexData.Add(new VertexPositionColor(position + right, Color.LightGreen));
                    vertexData.Add(new VertexPositionColor(position + normal, Color.LightGreen));
                    vertexData.Add(new VertexPositionColor(position + up, Color.LightGreen));
                    vertexData.Add(new VertexPositionColor(position + up, Color.LightGreen));
                    vertexData.Add(new VertexPositionColor(position + normal, Color.LightGreen));
                    vertexData.Add(new VertexPositionColor(position - right, Color.LightGreen));
                    vertexData.Add(new VertexPositionColor(position - right, Color.LightGreen));
                    vertexData.Add(new VertexPositionColor(position + normal, Color.LightGreen));
                    vertexData.Add(new VertexPositionColor(position - up, Color.LightGreen));
                    vertexData.Add(new VertexPositionColor(position - up, Color.LightGreen));
                    vertexData.Add(new VertexPositionColor(position + normal, Color.LightGreen));
                    vertexData.Add(new VertexPositionColor(position + right, Color.LightGreen));
                }
                return vertexData.ToArray();
            }
#endif
        }

        static Matrix XNAMatrixFromMSTS(matrix MSTSMatrix)
        {
            var XNAMatrix = Matrix.Identity;

            XNAMatrix.M11 = MSTSMatrix.AX;
            XNAMatrix.M12 = MSTSMatrix.AY;
            XNAMatrix.M13 = -MSTSMatrix.AZ;
            XNAMatrix.M21 = MSTSMatrix.BX;
            XNAMatrix.M22 = MSTSMatrix.BY;
            XNAMatrix.M23 = -MSTSMatrix.BZ;
            XNAMatrix.M31 = -MSTSMatrix.CX;
            XNAMatrix.M32 = -MSTSMatrix.CY;
            XNAMatrix.M33 = MSTSMatrix.CZ;
            XNAMatrix.M41 = MSTSMatrix.DX;
            XNAMatrix.M42 = MSTSMatrix.DY;
            XNAMatrix.M43 = -MSTSMatrix.DZ;

            return XNAMatrix;
        }
        /*
                public void PrepareFrame(RenderFrame frame, WorldPosition location, ShapeFlags flags)
                {
                    PrepareFrame(frame, location, Matrices, null, flags);
                }

                public void PrepareFrame(RenderFrame frame, WorldPosition location, Matrix[] animatedXNAMatrices, ShapeFlags flags)
                {
                    PrepareFrame(frame, location, animatedXNAMatrices, null, flags);
                }

                public void PrepareFrame(RenderFrame frame, WorldPosition location, Matrix[] animatedXNAMatrices, bool[] subObjVisible, ShapeFlags flags)
                {
                    var lodBias = ((float)Viewer.Settings.LODBias / 100 + 1);

                    // Locate relative to the camera
                    var dTileX = location.TileX - Viewer.Camera.TileX;
                    var dTileZ = location.TileZ - Viewer.Camera.TileZ;
                    var mstsLocation = location.Location;
                    mstsLocation.X += dTileX * 2048;
                    mstsLocation.Z += dTileZ * 2048;
                    var xnaDTileTranslation = location.XNAMatrix;
                    xnaDTileTranslation.M41 += dTileX * 2048;
                    xnaDTileTranslation.M43 -= dTileZ * 2048;

                    foreach (var lodControl in LodControls)
                    {
                        // Start with the furthest away distance, then look for a nearer one in range of the camera.
                        var displayDetailLevel = lodControl.DistanceLevels.Length - 1;

                        // If this LOD group is not in the FOV, skip the whole LOD group.
                        // TODO: This might imair some shadows.
                        if (!Viewer.Camera.InFov(mstsLocation, lodControl.DistanceLevels[displayDetailLevel].ViewSphereRadius))
                            continue;

                        // We choose the distance level (LOD) to display first:
                        //   - LODBias = 100 means we always use the highest detail.
                        //   - LODBias < 100 means we operate as normal (using the highest detail in-range of the camera) but
                        //     scaling it by LODBias.
                        //
                        // However, for the viewing distance (and view sphere), we use a slightly different calculation:
                        //   - LODBias = 100 means we always use the *lowest* detail viewing distance.
                        //   - LODBias < 100 means we operate as normal (see above).
                        //
                        // The reason for this disparity is that LODBias = 100 is special, because it means "always use
                        // highest detail", but this by itself is not useful unless we keep using the normal (LODBias-scaled)
                        // viewing distance - right down to the lowest detail viewing distance. Otherwise, we'll scale the
                        // highest detail viewing distance up by 100% and then the object will just disappear!

                        if (Viewer.Settings.LODBias == 100)
                            // Maximum detail!
                            displayDetailLevel = 0;
                        else if (Viewer.Settings.LODBias > -100)
                            // Not minimum detail, so find the correct level (with scaling by LODBias)
                            while ((displayDetailLevel > 0) && Viewer.Camera.InRange(mstsLocation, lodControl.DistanceLevels[displayDetailLevel - 1].ViewSphereRadius, lodControl.DistanceLevels[displayDetailLevel - 1].ViewingDistance * lodBias))
                                displayDetailLevel--;

                        var displayDetail = lodControl.DistanceLevels[displayDetailLevel];
                        var distanceDetail = Viewer.Settings.LODBias == 100
                            ? lodControl.DistanceLevels[lodControl.DistanceLevels.Length - 1]
                            : displayDetail;

                        // If set, extend the lowest LOD to the maximum viewing distance.
                        if (Viewer.Settings.LODViewingExtention && displayDetailLevel == lodControl.DistanceLevels.Length - 1)
                            distanceDetail.ViewingDistance = float.MaxValue;

                        for (var i = 0; i < displayDetail.SubObjects.Length; i++)
                        {
                            var subObject = displayDetail.SubObjects[i];

                            // The 1st subobject (note that index 0 is the main object itself) is hidden during the day if HasNightSubObj is true.
                            if ((subObjVisible != null && !subObjVisible[i]) || (i == 1 && HasNightSubObj && Viewer.MaterialManager.sunDirection.Y >= 0))
                                continue;

                            foreach (var shapePrimitive in subObject.ShapePrimitives)
                            {
                                var xnaMatrix = Matrix.Identity;
                                var hi = shapePrimitive.HierarchyIndex;
                                while (hi >= 0 && hi < shapePrimitive.Hierarchy.Length && shapePrimitive.Hierarchy[hi] != -1)
                                {
                                    Matrix.Multiply(ref xnaMatrix, ref animatedXNAMatrices[hi], out xnaMatrix);
                                    hi = shapePrimitive.Hierarchy[hi];
                                }
                                Matrix.Multiply(ref xnaMatrix, ref xnaDTileTranslation, out xnaMatrix);

                                // TODO make shadows depend on shape overrides

                                var interior = (flags & ShapeFlags.Interior) != 0;
                                frame.AddAutoPrimitive(mstsLocation, distanceDetail.ViewSphereRadius, distanceDetail.ViewingDistance * lodBias, shapePrimitive.Material, shapePrimitive, interior ? RenderPrimitiveGroup.Interior : RenderPrimitiveGroup.World, ref xnaMatrix, flags);
                            }
                        }
                    }
                }
        */
        public Matrix GetMatrixProduct(int iNode)
        {
            int[] h = LodControls[0].DistanceLevels[0].SubObjects[0].ShapePrimitives[0].Hierarchy;
            Matrix matrix = Matrix.Identity;
            while (iNode != -1)
            {
                matrix *= Matrices[iNode];
                iNode = h[iNode];
            }
            return matrix;
        }

        public int GetParentMatrix(int iNode)
        {
            return LodControls[0].DistanceLevels[0].SubObjects[0].ShapePrimitives[0].Hierarchy[iNode];
        }

        internal void Mark()
        {
            //            Viewer.ShapeManager.Mark(this);
            foreach (var lod in LodControls)
                lod.Mark();
        }
    }

}
