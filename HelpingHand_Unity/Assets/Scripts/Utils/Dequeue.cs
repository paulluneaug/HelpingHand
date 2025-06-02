using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class Dequeue<T> : IEnumerable<T>
{
    public int Count => m_size;
    public int Capacity => m_capacity;

    private readonly T[] m_array;
    private int m_front;
    private int m_rear;
    private int m_size;
    private readonly int m_capacity;

    public Dequeue(int capacity)
    {
        m_array = new T[capacity];
        m_front = -1;
        m_rear = -1;
        m_size = 0;
        m_capacity = capacity;
    }

    public bool IsEmpty()
    {
        return m_size == 0;
    }

    public bool IsFull()
    {
        return m_size == m_capacity;
    }

    public void Clear()
    {
        m_front = -1;
        m_rear = -1;
        m_size = 0;
    }

    public T At(int index)
    {
        if (index >= m_size)
        {
            throw new ArgumentOutOfRangeException("index");
        }
        int itemIndex = (m_rear + index) % m_capacity;
        return m_array[itemIndex];
    }

    public void EnqueueFront(T value)
    {
        if (IsFull())
        {
            throw new ArgumentOutOfRangeException("The queue is full");
        }

        if (m_front == -1)
        {
            m_front = 0;
            m_rear = 0;
        }
        else if (m_front == 0)
        {
            m_front = m_capacity - 1;
        }
        else
        {
            m_front--;
        }
        m_array[m_front] = value;
        m_size++;
    }

    // Add to the rear of the deque
    public void EnqueueRear(T value)
    {
        if (IsFull())
        {
            throw new ArgumentOutOfRangeException("The queue is full");
        }
        // If it's the first element
        if (m_front == -1)
        {
            m_front = 0;
            m_rear = 0;
        }
        else
        {
            // Wrap around if rear is at the end
            m_rear = (m_rear + 1) % m_capacity;
        }
        m_array[m_rear] = value;
        m_size++;
    }

    // Remove from the front of the deque
    public T DequeueFront()
    {
        if (IsEmpty())
        {
            throw new ArgumentOutOfRangeException("The queue is empty");
        }

        T removedValue = m_array[m_front];
        // Only one element in the queue
        if (m_front == m_rear)
        {
            m_front = -1;
            m_rear = -1;
        }
        else
        {
            // Move the front pointer
            m_front = (m_front + 1) % m_capacity;
        }
        m_size--;
        return removedValue;
    }

    // Remove from the rear of the deque
    public T DequeueRear()
    {
        if (IsEmpty())
        {
            throw new ArgumentOutOfRangeException("The queue is empty");
        }

        T removedValue = m_array[m_rear];
        // Only one element in the queue
        if (m_front == m_rear)
        {
            m_front = -1;
            m_rear = -1;
        }
        else
        {
            // Move the rear pointer
            m_rear = (m_rear - 1 + m_capacity) % m_capacity;
        }
        m_size--;
        return removedValue;
    }

    public IEnumerator<T> GetEnumerator()
    {
        return Enumerable.Range(0, m_size).Select((int index) => At(index)).GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}