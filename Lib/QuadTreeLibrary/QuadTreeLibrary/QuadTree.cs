// --------------------------------------------------------------------------------------------------------------------
// <copyright file="QuadTree.cs" company="Unison Networks Limited">
// **********************************************************
// The quad tree was taken from the codeplex project here: https://csharpquadtree.codeplex.com/ 
// The released date of the used version was July 9th 2009. 
// **********************************************************
// License: There is no license listed in the project, however the authoir posted here: https://csharpquadtree.codeplex.com/discussions/205459
// "use the code for whatever you want, I posted it to share - I'm not too familiar with the licensing - but you can use it for profit, non-profit, whatever :)  Glad it helps!"
// **********************************************************
// 
// **********************Custom Changes**********************
// The source files have then been moved, modified, significantly altered and put into a C# library to suit the AMST application's needs.
// </copyright>
// <summary>
//   Exception used to specify when a method was unable to retrieve a location object associated with an ILocatableObject.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace QuadTreeLibrary
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Windows;
    using CSharpQuadTree;
    using Point = System.Windows.Point;
    using Size = System.Windows.Size;

    /// <summary>
    /// Exception used to specify when a method was unable to retrieve a location object associated with an ILocatableObject. 
    /// </summary>
    public class QuadTree_NoLocationException : Exception
    {
        /// <summary>
        /// Constructor for the QuadTree_NoLocationException that simply immitates the constructor header of Exception class and passes the "message" on to it's parent. 
        /// </summary>
        /// <param name="message">The message that details the error that occured. </param>
        public QuadTree_NoLocationException(string message) : base(message) { }
        /// <summary>
        /// Constructor for the QuadTree_NoLocationException that simply immitates the constructor header of Exception class and passes the "message" on that way. 
        /// </summary>
        /// <param name="message">The message that details the error that occured. </param>
        /// <param name="innerException">Allows a second exception to be contained within this custom exception.</param>
        public QuadTree_NoLocationException(string message, Exception innerException) : base(message, innerException) { }
    }
    [Serializable]
    public class QuadTree<T> where T : class, IPoint
    {
        private readonly bool sort;
        private readonly Size minLeafSize;
        private readonly int maxObjectsPerLeaf;
        private QuadNode root = null;
        private Dictionary<T, QuadNode> objectToNodeLookup = new Dictionary<T, QuadNode>();
        private Dictionary<T, int> objectSortOrder = new Dictionary<T, int>();
        public QuadNode Root { get { return root; } }
        private object syncLock = new object();
        private int objectSortId = 0;
        [Serializable]
        public delegate Rect GetBoundsDelegate(IPoint locatableObject);

        public QuadTree(Size minLeafSize, int maxObjectsPerLeaf)
        {
            this.minLeafSize = minLeafSize;
            this.maxObjectsPerLeaf = maxObjectsPerLeaf;
        }

        public int GetSortOrder(T quadObject)
        {
            lock (objectSortOrder)
            {
                if (!objectSortOrder.ContainsKey(quadObject))
                    return -1;
                else
                {
                    return objectSortOrder[quadObject];
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="minLeafSize">The smallest size a leaf will split into</param>
        /// <param name="maxObjectsPerLeaf">Maximum number of objects per leaf before it forces a split into sub quadrants</param>
        /// <param name="sort">Whether or not queries will return objects in the order in which they were added</param>
        public QuadTree(Size minLeafSize, int maxObjectsPerLeaf, bool sort)
            : this(minLeafSize, maxObjectsPerLeaf)
        {
            this.sort = sort;
        }
        /// <summary>
        /// Inserts an object into the quadtree and creates any new roots/sections if needed.
        /// </summary>
        /// <param name="quadObject">The UniquePositionPoint you want to insert into the quadtree.</param>
        public void Insert(T quadObject)
        {
            lock (syncLock)
            {
                try
                {
                    Point point = quadObject.GetPoint();
                    if (sort & !objectSortOrder.ContainsKey(quadObject))
                    {
                        objectSortOrder.Add(quadObject, objectSortId++);
                    }
                    //If this is the first object to be added.
                    if (root == null)
                    {
                        //We need to setup the origional root. 
                        //NOTE: A lot of this code was commented out and altered because the original QuadTree library took Rectangles not points as inputs. 
                        
                        //var rootSize = new Size(Math.Ceiling(point.Width / minLeafSize.Width),
                        //                        Math.Ceiling(point.Height / minLeafSize.Height));
                        //double multiplier = Math.Max(minLeafSize.Width, minLeafSize.Height);
                        // minLeafSize = new Size(minLeafSize.Width * multiplier, minLeafSize.Height * multiplier);
                        var center = new Point(point.X  / 2, point.Y / 2);
                        //var rootOrigin = new Point(center.X - minLeafSize.Width / 2, center.Y - minLeafSize.Height / 2);
                        root = new QuadNode(new Rect(point, minLeafSize));
                    }
                    //Expand the root section until it contains a section that the point will fit within.
                    while (!root.Bounds.Contains(point))
                    {
                        ExpandRoot(point);
                    }
                    //Now we add the new object to the root section. 
                    InsertNodeObject(root, quadObject, point);
                }
                catch (QuadTree_NoLocationException)
                {
                    //Do nothing we could not retrieve a location so therefore this object cannot be added.
                }
            }
        }
        /// <summary>
        /// Searches for and attempts to retrieve all of the uniquePositionPoints's that fit within a given rectangle.
        /// </summary>
        /// <param name="bounds">The rectangle to search for.</param>
        /// <returns>A list of UniquePositionPoints.</returns>
        public List<T> Query(Rect bounds)
        {
            lock (syncLock)
            {
                List<T> results = new List<T>();
                if (root != null)
                    Query(bounds, root, results);
                if (sort)
                    results.Sort((a, b) => { return objectSortOrder[a].CompareTo(objectSortOrder[b]); });
                return results;
            }
        }

        public List<T> Query(int x, int y)
        {
            return Query(new Rect(new Point(x, y), new Size(1, 1)));
        }
        
        private void Query(Rect bounds, QuadNode node, List<T> results)
        {
            lock (syncLock)
            {
                if (node == null) return;

                if (bounds.IntersectsWith(node.Bounds))
                {
                    foreach (T quadObject in node.Objects)
                    {
                        Point objectBounds = quadObject.GetPoint();
                        if (bounds.Contains(objectBounds))
                            results.Add(quadObject);
                    }

                    foreach (QuadNode childNode in node.Nodes)
                    {
                        Query(bounds, childNode, results);
                    }
                }
            }
        }
        /// <summary>
        /// This method "expands the root" What this means is that it will take one "root" and if this root does not contain the point it will create a new "root" and the original root will then fit within the new one in a NW/NE/SE/SW position.
        /// </summary>
        /// <param name="newChildPoint">The new Point that the root needs to encompass. </param>
        private void ExpandRoot(Point newChildPoint)
        {
            lock (syncLock)
            {
                bool isNorth = root.Bounds.Y < newChildPoint.Y;
                bool isWest = root.Bounds.X < newChildPoint.X;

                Direction rootDirection;
                if (isNorth)
                {
                    rootDirection = isWest ? Direction.NW : Direction.NE;
                }
                else
                {
                    rootDirection = isWest ? Direction.SW : Direction.SE;
                }

                double newX = (rootDirection == Direction.NW || rootDirection == Direction.SW)
                                  ? root.Bounds.X
                                  : root.Bounds.X - root.Bounds.Width;
                double newY = (rootDirection == Direction.NW || rootDirection == Direction.NE)
                                  ? root.Bounds.Y
                                  : root.Bounds.Y - root.Bounds.Height;
                Rect newRootBounds = new Rect(newX, newY, root.Bounds.Width * 2, root.Bounds.Height * 2);
                QuadNode newRoot = new QuadNode(newRootBounds);
                SetupChildNodes(newRoot);
                newRoot[rootDirection] = root;
                root = newRoot;
            }
        }

        private void InsertNodeObject(QuadNode node, T quadObject, Point newObjectPoint)
        {
            lock (syncLock)
            {
                if (!node.Bounds.Contains(newObjectPoint))
                    throw new Exception("This should not happen, child does not fit within node bounds");

                if (!node.HasChildNodes() && node.Objects.Count + 1 > maxObjectsPerLeaf)
                {
                    SetupChildNodes(node);

                    List<T> childObjects = new List<T>(node.Objects);
                    List<T> childrenToRelocate = new List<T>();

                    foreach (T childObject in childObjects)
                    {
                        foreach (QuadNode childNode in node.Nodes)
                        {
                            if (childNode == null)
                                continue;

                            if (childNode.Bounds.Contains(newObjectPoint))
                            {
                                childrenToRelocate.Add(childObject);
                            }
                        }
                    }

                    foreach (T childObject in childrenToRelocate)
                    {
                        RemoveQuadObjectFromNode(childObject);
                        InsertNodeObject(node, childObject, childObject.GetPoint());
                    }
                }

                foreach (QuadNode childNode in node.Nodes)
                {
                    if (childNode != null)
                    {
                        if (childNode.Bounds.Contains(newObjectPoint))
                        {
                            InsertNodeObject(childNode, quadObject, newObjectPoint);
                            return;
                        }
                    }
                }

                AddQuadObjectToNode(node, quadObject);
            }
        }

        private void ClearQuadObjectsFromNode(QuadNode node)
        {
            lock (syncLock)
            {
                List<T> quadObjects = new List<T>(node.Objects);
                foreach (T quadObject in quadObjects)
                {
                    RemoveQuadObjectFromNode(quadObject);
                }
            }
        }

        private void RemoveQuadObjectFromNode(T quadObject)
        {
            lock (syncLock)
            {
                QuadNode node = objectToNodeLookup[quadObject];
                node.quadObjects.Remove(quadObject);
                objectToNodeLookup.Remove(quadObject);
                //quadObject.BoundsChanged -= new EventHandler(quadObject_BoundsChanged);
            }
        }

        private void AddQuadObjectToNode(QuadNode node, T quadObject)
        {
            lock (syncLock)
            {
                try
                {
                    objectToNodeLookup.Add(quadObject, node);
                    node.quadObjects.Add(quadObject);
                    //quadObject.BoundsChanged += new EventHandler(quadObject_BoundsChanged);
                }
                catch (ArgumentException)
                {
                    // Do nothing, this means we are simply dealing with duplicate CIM_PositionPoints.
                }
            }
        }

        void quadObject_BoundsChanged(object sender, EventArgs e)
        {
            lock (syncLock)
            {
                T quadObject = sender as T;
                if (quadObject != null)
                {
                    QuadNode node = objectToNodeLookup[quadObject];
                    if (!node.Bounds.Contains(quadObject.GetPoint()) || node.HasChildNodes())
                    {
                        RemoveQuadObjectFromNode(quadObject);
                        Insert(quadObject);
                        if (node.Parent != null)
                        {
                            CheckChildNodes(node.Parent);
                        }
                    }
                }
            }
        }

        private void SetupChildNodes(QuadNode node)
        {
            lock (syncLock)
            {
                if (minLeafSize.Width <= node.Bounds.Width / 2 && minLeafSize.Height <= node.Bounds.Height / 2)
                {
                    node[Direction.NW] = new QuadNode(node.Bounds.X, node.Bounds.Y, node.Bounds.Width / 2,
                                                      node.Bounds.Height / 2);
                    node[Direction.NE] = new QuadNode(node.Bounds.X + node.Bounds.Width / 2, node.Bounds.Y,
                                                      node.Bounds.Width / 2,
                                                      node.Bounds.Height / 2);
                    node[Direction.SW] = new QuadNode(node.Bounds.X, node.Bounds.Y + node.Bounds.Height / 2,
                                                      node.Bounds.Width / 2,
                                                      node.Bounds.Height / 2);
                    node[Direction.SE] = new QuadNode(node.Bounds.X + node.Bounds.Width / 2,
                                                      node.Bounds.Y + node.Bounds.Height / 2,
                                                      node.Bounds.Width / 2, node.Bounds.Height / 2);

                }
            }
        }

        public void Remove(T quadObject)
        {
            lock (syncLock)
            {
                if (sort && objectSortOrder.ContainsKey(quadObject))
                {
                    objectSortOrder.Remove(quadObject);
                }

                if (!objectToNodeLookup.ContainsKey(quadObject))
                    throw new KeyNotFoundException("QuadObject not found in dictionary for removal");

                QuadNode containingNode = objectToNodeLookup[quadObject];
                RemoveQuadObjectFromNode(quadObject);

                if (containingNode.Parent != null)
                    CheckChildNodes(containingNode.Parent);
            }
        }



        private void CheckChildNodes(QuadNode node)
        {
            lock (syncLock)
            {
                if (GetQuadObjectCount(node) <= maxObjectsPerLeaf)
                {
                    // Move child objects into this node, and delete sub nodes
                    List<T> subChildObjects = GetChildObjects(node);
                    foreach (T childObject in subChildObjects)
                    {
                        if (!node.Objects.Contains(childObject))
                        {
                            RemoveQuadObjectFromNode(childObject);
                            AddQuadObjectToNode(node, childObject);
                        }
                    }
                    if (node[Direction.NW] != null)
                    {
                        node[Direction.NW].Parent = null;
                        node[Direction.NW] = null;
                    }
                    if (node[Direction.NE] != null)
                    {
                        node[Direction.NE].Parent = null;
                        node[Direction.NE] = null;
                    }
                    if (node[Direction.SW] != null)
                    {
                        node[Direction.SW].Parent = null;
                        node[Direction.SW] = null;
                    }
                    if (node[Direction.SE] != null)
                    {
                        node[Direction.SE].Parent = null;
                        node[Direction.SE] = null;
                    }

                    if (node.Parent != null)
                        CheckChildNodes(node.Parent);
                    else
                    {
                        // Its the root node, see if we're down to one quadrant, with none in local storage - if so, ditch the other three
                        int numQuadrantsWithObjects = 0;
                        QuadNode nodeWithObjects = null;
                        foreach (QuadNode childNode in node.Nodes)
                        {
                            if (childNode != null && GetQuadObjectCount(childNode) > 0)
                            {
                                numQuadrantsWithObjects++;
                                nodeWithObjects = childNode;
                                if (numQuadrantsWithObjects > 1) break;
                            }
                        }
                        if (numQuadrantsWithObjects == 1)
                        {
                            foreach (QuadNode childNode in node.Nodes)
                            {
                                if (childNode != nodeWithObjects)
                                    childNode.Parent = null;
                            }
                            root = nodeWithObjects;
                        }
                    }
                }
            }
        }


        private List<T> GetChildObjects(QuadNode node)
        {
            lock (syncLock)
            {
                List<T> results = new List<T>();
                results.AddRange(node.quadObjects);
                foreach (QuadNode childNode in node.Nodes)
                {
                    if (childNode != null)
                        results.AddRange(GetChildObjects(childNode));
                }
                return results;
            }
        }

        public int GetQuadObjectCount()
        {
            lock (syncLock)
            {
                if (root == null)
                    return 0;
                int count = GetQuadObjectCount(root);
                return count;
            }
        }

        private int GetQuadObjectCount(QuadNode node)
        {
            lock (syncLock)
            {
                int count = node.Objects.Count;
                foreach (QuadNode childNode in node.Nodes)
                {
                    if (childNode != null)
                    {
                        count += GetQuadObjectCount(childNode);
                    }
                }
                return count;
            }
        }

        public int GetQuadNodeCount()
        {
            lock (syncLock)
            {
                if (root == null)
                    return 0;
                int count = GetQuadNodeCount(root, 1);
                return count;
            }
        }

        private int GetQuadNodeCount(QuadNode node, int count)
        {
            lock (syncLock)
            {
                if (node == null) return count;

                foreach (QuadNode childNode in node.Nodes)
                {
                    if (childNode != null)
                        count++;
                }
                return count;
            }
        }

        public List<QuadNode> GetAllNodes()
        {
            lock (syncLock)
            {
                List<QuadNode> results = new List<QuadNode>();
                if (root != null)
                {
                    results.Add(root);
                    GetChildNodes(root, results);
                }
                return results;
            }
        }

        private void GetChildNodes(QuadNode node, ICollection<QuadNode> results)
        {
            lock (syncLock)
            {
                foreach (QuadNode childNode in node.Nodes)
                {
                    if (childNode != null)
                    {
                        results.Add(childNode);
                        GetChildNodes(childNode, results);
                    }
                }
            }
        }
        [Serializable]
        public class QuadNode
        {
            private static int _id = 0;
            public readonly int ID = _id++;

            public QuadNode Parent { get; internal set; }

            private QuadNode[] _nodes = new QuadNode[4];
            public QuadNode this[Direction direction]
            {
                get
                {
                    switch (direction)
                    {
                        case Direction.NW:
                            return _nodes[0];
                        case Direction.NE:
                            return _nodes[1];
                        case Direction.SW:
                            return _nodes[2];
                        case Direction.SE:
                            return _nodes[3];
                        default:
                            return null;
                    }
                }
                set
                {
                    switch (direction)
                    {
                        case Direction.NW:
                            _nodes[0] = value;
                            break;
                        case Direction.NE:
                            _nodes[1] = value;
                            break;
                        case Direction.SW:
                            _nodes[2] = value;
                            break;
                        case Direction.SE:
                            _nodes[3] = value;
                            break;
                    }
                    if (value != null)
                        value.Parent = this;
                }
            }

            public ReadOnlyCollection<QuadNode> Nodes;

            internal List<T> quadObjects = new List<T>();
            public ReadOnlyCollection<T> Objects;

            public Rect Bounds { get; internal set; }

            public bool HasChildNodes()
            {
                return _nodes[0] != null;
            }

            public QuadNode(Rect bounds)
            {
                Bounds = bounds;
                Nodes = new ReadOnlyCollection<QuadNode>(_nodes);
                Objects = new ReadOnlyCollection<T>(quadObjects);
            }

            public QuadNode(double x, double y, double width, double height)
                : this(new Rect(x, y, width, height))
            {

            }
        }
    }

    public enum Direction : int
    {
        NW = 0,
        NE = 1,
        SW = 2,
        SE = 3
    }
}