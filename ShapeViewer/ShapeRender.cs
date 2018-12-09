using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShapeViewer
{
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

    public class Material { }

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

/*
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
*/
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
}
