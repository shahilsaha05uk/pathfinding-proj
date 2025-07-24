using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;

public class PriorityQueue<TElement>
{
    /// <summary>
    /// Represents an implicit heap-ordered complete d-ary tree, stored as an array.
    /// </summary>
    private (TElement Element, int Priority)[] _nodes;

    /// <summary>
    /// Custom comparer used to order the heap.
    /// </summary>
    private readonly IComparer<int>? _comparer;


    /// <summary>
    /// The number of nodes in the heap.
    /// </summary>
    private int _size;

    /// <summary>
    /// Version updated on mutation to help validate enumerators operate on a consistent state.
    /// </summary>
    private int _version;

    /// <summary>
    /// Specifies the arity of the d-ary heap, which here is quaternary.
    /// It is assumed that this value is a power of 2.
    /// </summary>
    private const int Arity = 4;

    /// <summary>
    /// The binary logarithm of <see cref="Arity" />.
    /// </summary>
    private const int Log2Arity = 2;

#if DEBUG
    static PriorityQueue()
    {
        Debug.Assert(Log2Arity > 0 && Math.Pow(2, Log2Arity) == Arity);
    }
#endif

    /// <summary>
    ///  Initializes a new instance of the <see cref="PriorityQueue{TElement, TPriority}"/> class.
    /// </summary>
    public PriorityQueue()
    {
        _nodes = Array.Empty<(TElement, int)>();
        _comparer = InitializeComparer(null);
    }

    /// <summary>
    ///  Initializes a new instance of the <see cref="PriorityQueue{TElement, TPriority}"/> class
    ///  with the specified initial capacity.
    /// </summary>
    /// <param name="initialCapacity">Initial capacity to allocate in the underlying heap array.</param>
    /// <exception cref="ArgumentOutOfRangeException">
    ///  The specified <paramref name="initialCapacity"/> was negative.
    /// </exception>
    public PriorityQueue(int initialCapacity)
        : this(initialCapacity, comparer: null)
    {
    }

    /// <summary>
    ///  Initializes a new instance of the <see cref="PriorityQueue{TElement, TPriority}"/> class
    ///  with the specified custom priority comparer.
    /// </summary>
    /// <param name="comparer">
    ///  Custom comparer dictating the ordering of elements.
    ///  Uses <see cref="Comparer{T}.Default" /> if the argument is <see langword="null"/>.
    /// </param>
    public PriorityQueue(IComparer<int>? comparer)
    {
        _nodes = Array.Empty<(TElement, int)>();
        _comparer = InitializeComparer(comparer);
    }

    /// <summary>
    ///  Initializes a new instance of the <see cref="PriorityQueue{TElement, TPriority}"/> class
    ///  with the specified initial capacity and custom priority comparer.
    /// </summary>
    /// <param name="initialCapacity">Initial capacity to allocate in the underlying heap array.</param>
    /// <param name="comparer">
    ///  Custom comparer dictating the ordering of elements.
    ///  Uses <see cref="Comparer{T}.Default" /> if the argument is <see langword="null"/>.
    /// </param>
    ///  The specified <paramref name="initialCapacity"/> was negative.
    /// </exception>
    public PriorityQueue(int initialCapacity, IComparer<int>? comparer)
    {
        _nodes = new (TElement, int)[initialCapacity];
        _comparer = InitializeComparer(comparer);
    }


    /// <summary>
    ///  Gets the number of elements contained in the <see cref="PriorityQueue{TElement, int}"/>.
    /// </summary>
    public int Count => _size;

    /// <summary>
    ///  Gets the priority comparer used by the <see cref="PriorityQueue{TElement, int}"/>.
    /// </summary>
    public IComparer<int> Comparer => _comparer ?? Comparer<int>.Default;


    /// <summary>
    /// Gets the index of an element's parent.
    /// </summary>
    private static int GetParentIndex(int index) => (index - 1) >> Log2Arity;

    /// <summary>
    /// Gets the index of the first child of an element.
    /// </summary>
    private static int GetFirstChildIndex(int index) => (index << Log2Arity) + 1;

    /// <summary>
    ///  Adds the specified element with associated priority to the <see cref="PriorityQueue{TElement, TPriority}"/>.
    /// </summary>
    /// <param name="element">The element to add to the <see cref="PriorityQueue{TElement, TPriority}"/>.</param>
    /// <param name="priority">The priority with which to associate the new element.</param>
    public void Enqueue(TElement element, int priority)
    {
        // Virtually add the node at the end of the underlying array.
        // Note that the node being enqueued does not need to be physically placed
        // there at this point, as such an assignment would be redundant.

        int currentSize = _size;
        _version++;

        if (_nodes.Length == currentSize)
        {
            Grow(currentSize + 1);
        }

        _size = currentSize + 1;

        if (_comparer == null)
        {
            MoveUpDefaultComparer((element, priority), currentSize);
        }
        else
        {
            MoveUpCustomComparer((element, priority), currentSize);
        }
    }

    /// <summary>
    ///  Removes and returns the minimal element from the <see cref="PriorityQueue{TElement, TPriority}"/>.
    /// </summary>
    /// <exception cref="InvalidOperationException">The queue is empty.</exception>
    /// <returns>The minimal element of the <see cref="PriorityQueue{TElement, TPriority}"/>.</returns>
    public TElement Dequeue()
    {
        if (_size == 0)
        {
            throw new System.Exception("The queue is empty");
        }

        TElement element = _nodes[0].Element;
        RemoveRootNode();
        return element;
    }

    public bool IsEmpty()
    {
        return Count == 0;
    }

    public bool Contains(TElement element)
    {
        for (int i = 0; i < _size; i++)
        {
            if (EqualityComparer<TElement>.Default.Equals(_nodes[i].Element, element))
                return true;
        }
        return false;
    }


    /// <summary>
    /// Grows the priority queue to match the specified min capacity.
    /// </summary>
    private void Grow(int minCapacity)
    {
        Debug.Assert(_nodes.Length < minCapacity);

        const int GrowFactor = 2;
        const int MinimumGrow = 4;

        int newcapacity = GrowFactor * _nodes.Length;

        // Allow the queue to grow to maximum possible capacity (~2G elements) before encountering overflow.
        // Note that this check works even when _nodes.Length overflowed thanks to the (uint) cast
        //if ((uint)newcapacity > Array.MaxLength) newcapacity = Array.MaxLength;

        // Ensure minimum growth is respected.
        newcapacity = Math.Max(newcapacity, _nodes.Length + MinimumGrow);

        // If the computed capacity is still less than specified, set to the original argument.
        // Capacities exceeding Array.MaxLength will be surfaced as OutOfMemoryException by Array.Resize.
        if (newcapacity < minCapacity) newcapacity = minCapacity;

        Array.Resize(ref _nodes, newcapacity);
    }

    /// <summary>
    /// Removes the node from the root of the heap
    /// </summary>
    private void RemoveRootNode()
    {
        int lastNodeIndex = --_size;
        _version++;

        if (lastNodeIndex > 0)
        {
            (TElement Element, int Priority) lastNode = _nodes[lastNodeIndex];
            if (_comparer == null)
            {
                MoveDownDefaultComparer(lastNode, 0);
            }
            else
            {
                MoveDownCustomComparer(lastNode, 0);
            }
        }

        if (RuntimeHelpers.IsReferenceOrContainsReferences<(TElement, int)>())
        {
            _nodes[lastNodeIndex] = default;
        }
    }

    /// <summary>
    /// Moves a node up in the tree to restore heap order.
    /// </summary>
    private void MoveUpDefaultComparer((TElement Element, int Priority) node, int nodeIndex)
    {
        // Instead of swapping items all the way to the root, we will perform
        // a similar optimization as in the insertion sort.

        Debug.Assert(_comparer is null);
        Debug.Assert(0 <= nodeIndex && nodeIndex < _size);

        (TElement Element, int Priority)[] nodes = _nodes;

        while (nodeIndex > 0)
        {
            int parentIndex = GetParentIndex(nodeIndex);
            (TElement Element, int Priority) parent = nodes[parentIndex];

            if (Comparer<int>.Default.Compare(node.Priority, parent.Priority) < 0)
            {
                nodes[nodeIndex] = parent;
                nodeIndex = parentIndex;
            }
            else
            {
                break;
            }
        }

        nodes[nodeIndex] = node;
    }

    /// <summary>
    /// Moves a node up in the tree to restore heap order.
    /// </summary>
    private void MoveUpCustomComparer((TElement Element, int Priority) node, int nodeIndex)
    {
        // Instead of swapping items all the way to the root, we will perform
        // a similar optimization as in the insertion sort.

        Debug.Assert(_comparer is not null);
        Debug.Assert(0 <= nodeIndex && nodeIndex < _size);

        IComparer<int> comparer = _comparer;
        (TElement Element, int Priority)[] nodes = _nodes;

        while (nodeIndex > 0)
        {
            int parentIndex = GetParentIndex(nodeIndex);
            (TElement Element, int Priority) parent = nodes[parentIndex];

            if (comparer.Compare(node.Priority, parent.Priority) < 0)
            {
                nodes[nodeIndex] = parent;
                nodeIndex = parentIndex;
            }
            else
            {
                break;
            }
        }

        nodes[nodeIndex] = node;
    }

    /// <summary>
    /// Moves a node down in the tree to restore heap order.
    /// </summary>
    private void MoveDownDefaultComparer((TElement Element, int Priority) node, int nodeIndex)
    {
        // The node to move down will not actually be swapped every time.
        // Rather, values on the affected path will be moved up, thus leaving a free spot
        // for this value to drop in. Similar optimization as in the insertion sort.

        Debug.Assert(_comparer is null);
        Debug.Assert(0 <= nodeIndex && nodeIndex < _size);

        (TElement Element, int Priority)[] nodes = _nodes;
        int size = _size;

        int i;
        while ((i = GetFirstChildIndex(nodeIndex)) < size)
        {
            // Find the child node with the minimal priority
            (TElement Element, int Priority) minChild = nodes[i];
            int minChildIndex = i;

            int childIndexUpperBound = Math.Min(i + Arity, size);
            while (++i < childIndexUpperBound)
            {
                (TElement Element, int Priority) nextChild = nodes[i];
                if (Comparer<int>.Default.Compare(nextChild.Priority, minChild.Priority) < 0)
                {
                    minChild = nextChild;
                    minChildIndex = i;
                }
            }

            // Heap property is satisfied; insert node in this location.
            if (Comparer<int>.Default.Compare(node.Priority, minChild.Priority) <= 0)
            {
                break;
            }

            // Move the minimal child up by one node and
            // continue recursively from its location.
            nodes[nodeIndex] = minChild;
            nodeIndex = minChildIndex;
        }

        nodes[nodeIndex] = node;
    }

    /// <summary>
    /// Moves a node down in the tree to restore heap order.
    /// </summary>
    private void MoveDownCustomComparer((TElement Element, int Priority) node, int nodeIndex)
    {
        // The node to move down will not actually be swapped every time.
        // Rather, values on the affected path will be moved up, thus leaving a free spot
        // for this value to drop in. Similar optimization as in the insertion sort.

        Debug.Assert(_comparer is not null);
        Debug.Assert(0 <= nodeIndex && nodeIndex < _size);

        IComparer<int> comparer = _comparer;
        (TElement Element, int Priority)[] nodes = _nodes;
        int size = _size;

        int i;
        while ((i = GetFirstChildIndex(nodeIndex)) < size)
        {
            // Find the child node with the minimal priority
            (TElement Element, int Priority) minChild = nodes[i];
            int minChildIndex = i;

            int childIndexUpperBound = Math.Min(i + Arity, size);
            while (++i < childIndexUpperBound)
            {
                (TElement Element, int Priority) nextChild = nodes[i];
                if (comparer.Compare(nextChild.Priority, minChild.Priority) < 0)
                {
                    minChild = nextChild;
                    minChildIndex = i;
                }
            }

            // Heap property is satisfied; insert node in this location.
            if (comparer.Compare(node.Priority, minChild.Priority) <= 0)
            {
                break;
            }

            // Move the minimal child up by one node and continue recursively from its location.
            nodes[nodeIndex] = minChild;
            nodeIndex = minChildIndex;
        }

        nodes[nodeIndex] = node;
    }

    /// <summary>
    /// Initializes the custom comparer to be used internally by the heap.
    /// </summary>
    private static IComparer<int>? InitializeComparer(IComparer<int>? comparer)
    {
        if (typeof(int).IsValueType)
        {
            if (comparer == Comparer<int>.Default)
            {
                // if the user manually specifies the default comparer,
                // revert to using the optimized path.
                return null;
            }

            return comparer;
        }
        else
        {
            // Currently the JIT doesn't optimize direct Comparer<T>.Default.Compare
            // calls for reference types, so we want to cache the comparer instance instead.
            // TODO https://github.com/dotnet/runtime/issues/10050: Update if this changes in the future.
            return comparer ?? Comparer<int>.Default;
        }
    }
}

public class PriorityQueue<TElement, TPriority>
{
    private (TElement Element, TPriority Priority)[] _nodes;
    private readonly IComparer<TPriority>? _comparer;
    private int _size;
    private int _version;

    private const int Arity = 4;
    private const int Log2Arity = 2;

#if DEBUG
    static PriorityQueue()
    {
        Debug.Assert(Log2Arity > 0 && Math.Pow(2, Log2Arity) == Arity);
    }
#endif

    public PriorityQueue()
    {
        _nodes = Array.Empty<(TElement, TPriority)>();
        _comparer = InitializeComparer(null);
    }

    public PriorityQueue(int initialCapacity)
        : this(initialCapacity, null) { }

    public PriorityQueue(IComparer<TPriority>? comparer)
    {
        _nodes = Array.Empty<(TElement, TPriority)>();
        _comparer = InitializeComparer(comparer);
    }

    public PriorityQueue(int initialCapacity, IComparer<TPriority>? comparer)
    {
        _nodes = new (TElement, TPriority)[initialCapacity];
        _comparer = InitializeComparer(comparer);
    }

    public int Count => _size;
    public IComparer<TPriority> Comparer => _comparer ?? Comparer<TPriority>.Default;

    private static int GetParentIndex(int index) => (index - 1) >> Log2Arity;
    private static int GetFirstChildIndex(int index) => (index << Log2Arity) + 1;

    public void Enqueue(TElement element, TPriority priority)
    {
        int currentSize = _size;
        _version++;

        if (_nodes.Length == currentSize)
            Grow(currentSize + 1);

        _size = currentSize + 1;

        if (_comparer == null)
            MoveUpDefaultComparer((element, priority), currentSize);
        else
            MoveUpCustomComparer((element, priority), currentSize);
    }

    public TElement Dequeue()
    {
        if (_size == 0)
            throw new Exception("The queue is empty");

        TElement element = _nodes[0].Element;
        RemoveRootNode();
        return element;
    }

    public bool IsEmpty() => Count == 0;

    public bool Contains(TElement element)
    {
        for (int i = 0; i < _size; i++)
        {
            if (EqualityComparer<TElement>.Default.Equals(_nodes[i].Element, element))
                return true;
        }
        return false;
    }

    private void Grow(int minCapacity)
    {
        int newCapacity = Math.Max(_nodes.Length * 2, _nodes.Length + 4);
        if (newCapacity < minCapacity)
            newCapacity = minCapacity;

        Array.Resize(ref _nodes, newCapacity);
    }

    private void RemoveRootNode()
    {
        int lastNodeIndex = --_size;
        _version++;

        if (lastNodeIndex > 0)
        {
            var lastNode = _nodes[lastNodeIndex];
            if (_comparer == null)
                MoveDownDefaultComparer(lastNode, 0);
            else
                MoveDownCustomComparer(lastNode, 0);
        }

        if (RuntimeHelpers.IsReferenceOrContainsReferences<(TElement, TPriority)>())
        {
            _nodes[lastNodeIndex] = default;
        }
    }

    private void MoveUpDefaultComparer((TElement Element, TPriority Priority) node, int nodeIndex)
    {
        var nodes = _nodes;
        while (nodeIndex > 0)
        {
            int parentIndex = GetParentIndex(nodeIndex);
            var parent = nodes[parentIndex];

            if (Comparer<TPriority>.Default.Compare(node.Priority, parent.Priority) < 0)
            {
                nodes[nodeIndex] = parent;
                nodeIndex = parentIndex;
            }
            else break;
        }
        nodes[nodeIndex] = node;
    }

    private void MoveUpCustomComparer((TElement Element, TPriority Priority) node, int nodeIndex)
    {
        var comparer = _comparer!;
        var nodes = _nodes;
        while (nodeIndex > 0)
        {
            int parentIndex = GetParentIndex(nodeIndex);
            var parent = nodes[parentIndex];

            if (comparer.Compare(node.Priority, parent.Priority) < 0)
            {
                nodes[nodeIndex] = parent;
                nodeIndex = parentIndex;
            }
            else break;
        }
        nodes[nodeIndex] = node;
    }

    private void MoveDownDefaultComparer((TElement Element, TPriority Priority) node, int nodeIndex)
    {
        var nodes = _nodes;
        int size = _size;
        int i;
        while ((i = GetFirstChildIndex(nodeIndex)) < size)
        {
            var minChild = nodes[i];
            int minChildIndex = i;

            int upper = Math.Min(i + Arity, size);
            while (++i < upper)
            {
                var nextChild = nodes[i];
                if (Comparer<TPriority>.Default.Compare(nextChild.Priority, minChild.Priority) < 0)
                {
                    minChild = nextChild;
                    minChildIndex = i;
                }
            }

            if (Comparer<TPriority>.Default.Compare(node.Priority, minChild.Priority) <= 0)
                break;

            nodes[nodeIndex] = minChild;
            nodeIndex = minChildIndex;
        }

        nodes[nodeIndex] = node;
    }

    private void MoveDownCustomComparer((TElement Element, TPriority Priority) node, int nodeIndex)
    {
        var comparer = _comparer!;
        var nodes = _nodes;
        int size = _size;
        int i;
        while ((i = GetFirstChildIndex(nodeIndex)) < size)
        {
            var minChild = nodes[i];
            int minChildIndex = i;

            int upper = Math.Min(i + Arity, size);
            while (++i < upper)
            {
                var nextChild = nodes[i];
                if (comparer.Compare(nextChild.Priority, minChild.Priority) < 0)
                {
                    minChild = nextChild;
                    minChildIndex = i;
                }
            }

            if (comparer.Compare(node.Priority, minChild.Priority) <= 0)
                break;

            nodes[nodeIndex] = minChild;
            nodeIndex = minChildIndex;
        }

        nodes[nodeIndex] = node;
    }

    private static IComparer<TPriority>? InitializeComparer(IComparer<TPriority>? comparer)
    {
        if (typeof(TPriority).IsValueType)
        {
            if (comparer == Comparer<TPriority>.Default)
                return null;
            return comparer;
        }
        else
        {
            return comparer ?? Comparer<TPriority>.Default;
        }
    }

    public void UpdatePriority(Node jumpPoint, float fCost)
    {
        int index = -1;

        // Locate the element
        for (int i = 0; i < _size; i++)
        {
            if (EqualityComparer<TElement>.Default.Equals(_nodes[i].Element, (TElement)(object)jumpPoint))
            {
                index = i;
                break;
            }
        }

        if (index == -1)
            throw new InvalidOperationException("Element not found in priority queue.");

        var oldPriority = _nodes[index].Priority;
        var newPriority = (TPriority)(object)fCost;
        _nodes[index] = ((TElement)(object)jumpPoint, newPriority);

        // Determine if we need to move up or down the heap
        if (_comparer == null)
        {
            if (Comparer<TPriority>.Default.Compare(newPriority, oldPriority) < 0)
                MoveUpDefaultComparer(_nodes[index], index);
            else
                MoveDownDefaultComparer(_nodes[index], index);
        }
        else
        {
            if (_comparer.Compare(newPriority, oldPriority) < 0)
                MoveUpCustomComparer(_nodes[index], index);
            else
                MoveDownCustomComparer(_nodes[index], index);
        }

        _version++;
    }

}
