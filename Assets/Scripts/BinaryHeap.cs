// Taken from Hamids pathfinding Lesson.

using System;
using System.Collections.Generic;

/// <summary>
/// A binary heap, useful for sorting data and priority queues.
/// </summary>
/// <typeparam name="T"><![CDATA[IComparable<T> type of item in the heap]]>.</typeparam>
public class BinaryHeap<T> : ICollection<T> where T : IComparable<T>
{
    // Fields
    private List<T> data;
    private bool sorted;

    // Properties
    /// <summary>
    /// Gets the number of values in the heap. 
    /// </summary>
    public int Count
    {
        get
        {
            return data.Count;
        }
    }
    /// <summary>
    /// Gets or sets the capacity of the heap.
    /// </summary>
    public int Capacity
    {
        get
        {
            return data.Capacity;
        }
        set
        {
            data.Capacity = value;
        }
    }

    // Methods
    /// <summary>
    /// Creates a new binary heap.
    /// </summary>
    public BinaryHeap(int capacity = 0)
    {
        data = new List<T>();
        if (capacity > 0)
        {
            data.Capacity = capacity;
        }
    }

    private BinaryHeap(List<T> data)
    {
        this.data = new List<T>(data);
    }

    /// <summary>
    /// Gets the first value in the heap without removing it.
    /// </summary>
    /// <returns>The lowest value of type T</returns>
    public T Front()
    {
        return data[0];
    }

    /// <summary>
    /// Removes all items from the heap.
    /// </summary>
    public void Clear()
    {
        data.Clear();
    }

    /// <summary>
    /// Adds a key and value to the heap.
    /// </summary>
    /// <param name="item">The item to add to the heap.</param>
    public void Add(T item)
    {
        data.Add(item);
        UpHeap();
    }

    /// <summary>
    /// Removes and returns the first item in the heap.
    /// </summary>
    /// <returns>The next value in the heap.</returns>
    public T Remove()
    {
        //if (data.Count == 0)
        //{
        //	throw new InvalidOperationException("Cannot remove item, heap is empty.");
        //}

        T removedItem = data[0];
        int lastIndex = data.Count - 1;
        if (lastIndex > 1)
        {
            data[0] = data[lastIndex];
            data.RemoveAt(lastIndex);
            DownHeap();
        }
        else
        {
            data.Clear();
        }

        return removedItem;
    }


    public void Update(T item)
    {
        int itemIndex = data.IndexOf(item);

        sorted = false;
        int lastIndex = itemIndex;
        int parentIndex = GetParentIndex(lastIndex);
        while (parentIndex > -1 && item.CompareTo(data[parentIndex]) < 0)
        {
            data[lastIndex] = data[parentIndex]; //Swap nodes
            lastIndex = parentIndex;
            parentIndex = GetParentIndex(lastIndex);
        }
        data[lastIndex] = item;
    }

    /// <summary> helper function that performs up-heap bubbling </summary>
    private void UpHeap()
    {
        sorted = false;
        int lastIndex = data.Count - 1;
        T item = data[lastIndex];
        int parentIndex = GetParentIndex(lastIndex);
        while (parentIndex > -1 && item.CompareTo(data[parentIndex]) < 0)
        {
            data[lastIndex] = data[parentIndex]; //Swap nodes
            lastIndex = parentIndex;
            parentIndex = GetParentIndex(lastIndex);
        }
        data[lastIndex] = item;
    }

    /// <summary> helper function that performs down-heap bubbling </summary>
    private void DownHeap()
    {
        int dataCount = data.Count;

        sorted = false;
        int nextIndex;
        int currentIndex = 0;
        T item = data[currentIndex];
        while (true)
        {
            int firstChildIndex = GetFirstChildIndex(currentIndex);
            if (firstChildIndex >= dataCount)
            {
                break;
            }

            int secondChildIndex = GetSecondChildIndex(currentIndex);
            if (secondChildIndex >= dataCount)
            {
                nextIndex = firstChildIndex;
            }
            else
            {
                nextIndex = data[firstChildIndex].CompareTo(data[secondChildIndex]) < 0 ? firstChildIndex : secondChildIndex;
            }

            if (item.CompareTo(data[nextIndex]) > 0)
            {
                data[currentIndex] = data[nextIndex]; //Swap nodes
                currentIndex = nextIndex;
            }
            else
            {
                break;
            }
        }
        data[currentIndex] = item;
    }
    public void Sort()
    {
        if (sorted)
        {
            return;
        }

        data.Sort();
        sorted = true;
    }

    /// <summary> helper function that calculates the parent of a node </summary>
    private static int GetParentIndex(int index)
    {
        return (index - 1) >> 1;
    }

    /// <summary> helper function that calculates the first child of a node </summary>
    private static int GetFirstChildIndex(int index)
    {
        return (index << 1) + 1;
    }

    /// <summary> helper function that calculates the second child of a node </summary>
    private static int GetSecondChildIndex(int index)
    {
        return (index << 1) + 2;
    }

    /// <summary>
    /// Creates a new instance of an identical binary heap.
    /// </summary>
    /// <returns>A BinaryHeap.</returns>
    public BinaryHeap<T> Copy()
    {
        return new BinaryHeap<T>(data);
    }

    /// <summary>
    /// Gets an enumerator for the binary heap.
    /// </summary>
    /// <returns>An IEnumerator of type T.</returns>
    public IEnumerator<T> GetEnumerator()
    {
        Sort();
        for (int dataIndex = 0; dataIndex < data.Count; ++dataIndex)
        {
            yield return data[dataIndex];
        }
    }
    System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    /// <summary>
    /// Checks to see if the binary heap contains the specified item.
    /// </summary>
    /// <param name="item">The item to search the binary heap for.</param>
    /// <returns>A boolean, true if binary heap contains item.</returns>
    public bool Contains(T item)
    {
        Sort();
        return data.BinarySearch(item) >= 0;
    }

    /// <summary>
    /// Copies the binary heap to an array at the specified index.
    /// </summary>
    /// <param name="array">One dimensional array that is the destination of the copied elements.</param>
    /// <param name="arrayIndex">The zero-based index at which copying begins.</param>
    public void CopyTo(T[] array, int arrayIndex)
    {
        Sort();
        data.CopyTo(array, arrayIndex);
    }

    /// <summary>
    /// Gets whether or not the binary heap is readonly.
    /// </summary>
    public bool IsReadOnly
    {
        get
        {
            return false;
        }
    }

    /// <summary>
    /// Removes an item from the binary heap. This utilizes the type T's Comparer and will not remove duplicates.
    /// </summary>
    /// <param name="item">The item to be removed.</param>
    /// <returns>Boolean true if the item was removed.</returns>
    public bool Remove(T item)
    {
        Sort();
        int foundIndex = data.BinarySearch(item);
        if (foundIndex < 0)
        {
            return false;
        }

        data.RemoveAt(foundIndex);
        return true;
    }
}