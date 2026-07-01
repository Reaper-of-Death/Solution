using System;
using System.Collections;
using System.Collections.Generic;

class SmartStack<T> : IEnumerable<T>
{
    private T[] _items;
    private int _count;

    public SmartStack()
    {
        _items = new T[4];
        _count = 0;
    }

    public SmartStack(int capacity)
    {
        if (capacity < 0)
            throw new ArgumentOutOfRangeException(nameof(capacity), "Capacity cannot be negative");

        _items = new T[capacity];
        _count = 0;
    }

    public SmartStack(IEnumerable<T> collection)
    {
        if (collection == null)
            throw new ArgumentNullException(nameof(collection));

        // ИСПРАВЛЕНО: Используем List<T> для сбора элементов, а не синтаксис массива
        var tempList = new List<T>(collection);
        _items = new T[tempList.Count];
        _count = 0;

        for (var i = tempList.Count - 1; i >= 0; i--)
        {
            _items[_count++] = tempList[i];
        }
    }

    public int Count => _count;

    public int Capacity => _items.Length;

    private void ResizeArray(int newCapacity)
    {
        T[] newArray = new T[newCapacity];
        Array.Copy(_items, 0, newArray, 0, _count);
        _items = newArray;
    }

    public void PushRange(IEnumerable<T> collection)
    {
        if (collection == null)
            throw new ArgumentNullException(nameof(collection));

        var tempList = new List<T>(collection);
        int itemsToAdd = tempList.Count;

        if (itemsToAdd == 0)
            return;

        while (_count + itemsToAdd > _items.Length)
        {
            ResizeArray(_items.Length * 2);
        }

        for (int i = tempList.Count - 1; i >= 0; i--)
        {
            _items[_count++] = tempList[i];
        }
    }

    public void Push(T item)
    {
        if (_count == _items.Length)
        {
            ResizeArray(_items.Length * 2);
        }
        _items[_count++] = item;
    }

    public T Pop()
    {
        if (_count == 0)
            throw new InvalidOperationException("Stack is empty");

        T item = _items[--_count];
        _items[_count] = default(T);

        return item;
    }

    public T Peek()
    {
        if (_count == 0)
            throw new InvalidOperationException("Stack is empty");

        return _items[_count - 1];
    }

    public bool Contains(T item)
    {
        EqualityComparer<T> comparer = EqualityComparer<T>.Default;
        
        for (int i = 0; i < _count; i++)
        {
            if (comparer.Equals(_items[i], item))
                return true;
        }
        return false;
    }

    public IEnumerator<T> GetEnumerator()
    {
        for (int i = _count - 1; i >= 0; i--)
        {
            yield return _items[i];
        }
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    public override string ToString()
    {
        return $"SmartStack<T> [Count: {_count}, Capacity: {_items.Length}]";
    }

    public T this[int depth]
    {
        get
        {
            if (depth < 0 || depth >= _count)
                throw new ArgumentOutOfRangeException(nameof(depth),
                    $"Depth must be between 0 and {_count - 1}");

            return _items[_count - 1 - depth];
        }
        set
        {
            if (depth < 0 || depth >= _count)
                throw new ArgumentOutOfRangeException(nameof(depth),
                    $"Depth must be between 0 and {_count - 1}");

            _items[_count - 1 - depth] = value;
        }
    }
}

class Program
{
    static void Main()
    {
        Console.WriteLine("=== Демонстрация работы SmartStack<T> ===\n");

        var stack1 = new SmartStack<int>();
        Console.WriteLine($"Стек1 (пустой): {stack1}");
        Console.WriteLine($"Count: {stack1.Count}, Capacity: {stack1.Capacity}\n");

        stack1.Push(10);
        stack1.Push(20);
        stack1.Push(30);
        Console.WriteLine("После добавления 10, 20, 30:");
        Console.WriteLine($"Count: {stack1.Count}, Capacity: {stack1.Capacity}");
        Console.WriteLine($"Вершина: {stack1.Peek()}\n");

        var numbers = new List<int> { 100, 200, 300 };
        stack1.PushRange(numbers);
        Console.WriteLine("После PushRange [100, 200, 300]:");
        Console.WriteLine($"Count: {stack1.Count}, Capacity: {stack1.Capacity}");

        // Вывод всех элементов (от вершины к основанию)
        Console.Write("Содержимое стека (вершина -> основание): ");
        foreach (var item in stack1)
        {
            Console.Write($"{item} ");
        }
        Console.WriteLine("\n");

        var initialData = new[] { "A", "B", "C", "D" };
        var stack2 = new SmartStack<string>(initialData);
        Console.WriteLine("Стек2 из коллекции [A, B, C, D]:");
        Console.WriteLine($"Count: {stack2.Count}, Capacity: {stack2.Capacity}");
        Console.Write("Содержимое (вершина -> основание): ");
        foreach (var item in stack2)
        {
            Console.Write($"{item} ");
        }
        Console.WriteLine("\n");

        Console.WriteLine("Индексатор (глубина 0 - вершина):");
        Console.WriteLine($"Элемент на глубине 0: {stack2[0]}");
        Console.WriteLine($"Элемент на глубине 1: {stack2[1]}");
        Console.WriteLine($"Элемент на глубине 2: {stack2[2]}");
        Console.WriteLine($"Элемент на глубине 3: {stack2[3]}");

        stack2[2] = "Z";
        Console.WriteLine($"\nПосле изменения элемента на глубине 2: {stack2[2]}");
        Console.Write("Обновлённое содержимое: ");
        foreach (var item in stack2)
        {
            Console.Write($"{item} ");
        }
        Console.WriteLine("\n");

        Console.WriteLine($"Содержит 'B': {stack2.Contains("B")}");
        Console.WriteLine($"Содержит 'Z': {stack2.Contains("Z")}\n");

        var popped = stack2.Pop();
        Console.WriteLine($"Удалён элемент: {popped}");
        Console.WriteLine($"Новый стек: Count: {stack2.Count}, Capacity: {stack2.Capacity}");
        Console.Write("Содержимое: ");
        foreach (var item in stack2)
        {
            Console.Write($"{item} ");
        }
        Console.WriteLine("\n");

        try
        {
            var emptyStack = new SmartStack<object>();
            Console.WriteLine("Попытка Pop из пустого стека...");
            emptyStack.Pop();
        }
        catch (InvalidOperationException ex)
        {
            Console.WriteLine($"Исключение: {ex.Message}\n");
        }

        var smallStack = new SmartStack<int>(2);
        Console.WriteLine($"Маленький стек: Capacity = {smallStack.Capacity}");
        smallStack.Push(1);
        smallStack.Push(2);
        Console.WriteLine($"После добавления 2 элементов: Count = {smallStack.Count}, Capacity = {smallStack.Capacity}");
        smallStack.Push(3);
        Console.WriteLine($"После добавления 3-го элемента: Count = {smallStack.Count}, Capacity = {smallStack.Capacity}");
        smallStack.PushRange(new[] { 4, 5, 6, 7, 8 });
        Console.WriteLine($"После PushRange 5 элементов: Count = {smallStack.Count}, Capacity = {smallStack.Capacity}");
        Console.Write("Содержимое: ");
        foreach (var item in smallStack)
        {
            Console.Write($"{item} ");
        }
        Console.WriteLine();

        try
        {
            var invalidStack = new SmartStack<int>(-5);
        }
        catch (ArgumentOutOfRangeException ex)
        {
            Console.WriteLine($"\nИсключение при создании с отрицательной ёмкостью: {ex.Message}");
        }

        Console.WriteLine("\nПеребор стека через foreach:");
        foreach (var item in stack1)
        {
            Console.Write($"{item} ");
        }
        Console.WriteLine();
    }
}