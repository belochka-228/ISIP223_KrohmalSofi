using System;
using System.Collections.Generic;

class Program
{
    static void Main()
    {
        int n = GetOperationCount();
        List<string> names = new List<string>();
        List<double> amounts = new List<double>();
        EnterOperations(n, names, amounts);
        ShowMenu(names, amounts);
    }

    static int GetOperationCount()
    {
        int n;
        while (true)
        {
            Console.Write("Введите количество операций (2-40): ");
            if (int.TryParse(Console.ReadLine(), out n) && n >= 2 && n <= 40) return n;
            Console.WriteLine("Введите число от 2 до 40");
        }
    }

    static void EnterOperations(int n, List<string> names, List<double> amounts)
    {
        Console.WriteLine($"\nВведите {n} операций (Название; Сумма):");
        for (int i = 0; i < n; i++)
        {
            while (true)
            {
                Console.Write($"Операция {i + 1}: ");
                string input = Console.ReadLine();

                if (!input.Contains(";"))
                {
                    Console.WriteLine("Формат: Название; Сумма");
                    continue;
                }

                string[] parts = input.Split(';');
                if (parts.Length < 2)
                {
                    Console.WriteLine("Пример: Влажные салфетки; 235");
                    continue;
                }

                string name = parts[0].Trim();
                if (!double.TryParse(parts[1].Trim(), out double amount) || amount <= 0)
                {
                    Console.WriteLine("Сумма должна быть > 0");
                    continue;
                }

                names.Add(name);
                amounts.Add(amount);
                break;
            }
        }
    }

    static void ShowMenu(List<string> names, List<double> amounts)
    {
        while (true)
        {
            Console.WriteLine("\n=== МЕНЮ ===");
            Console.WriteLine("1. Вывод данных");
            Console.WriteLine("2. Статистика");
            Console.WriteLine("3. Сортировка по цене");
            Console.WriteLine("4. Конвертация валюты");
            Console.WriteLine("5. Поиск по названию");
            Console.WriteLine("0. Выход");

            Console.Write("Выберите пункт: ");
            string choice = Console.ReadLine();

            switch (choice)
            {
                case "1": ShowData(names, amounts); break;
                case "2": ShowStatistics(amounts); break;
                case "3": BubbleSort(names, amounts); break;
                case "4": ConvertCurrency(amounts); break;
                case "5": SearchByName(names, amounts); break;
                case "0": Console.WriteLine("Выход..."); return;
                default: Console.WriteLine("Неверный выбор"); break;
            }
        }
    }

    static void ShowData(List<string> names, List<double> amounts)
    {
        Console.WriteLine("\n--- ОПЕРАЦИИ ---");
        double total = 0;
        for (int i = 0; i < names.Count; i++)
        {
            Console.WriteLine($"{i + 1}. {names[i],-25} {amounts[i],8:F2} руб.");
            total += amounts[i];
        }
        Console.WriteLine($"ИТОГО: {total,33:F2} руб.");
    }

    static void ShowStatistics(List<double> amounts)
    {
        if (amounts.Count == 0) return;

        double total = 0, max = amounts[0], min = amounts[0];
        foreach (var amount in amounts)
        {
            total += amount;
            if (amount > max) max = amount;
            if (amount < min) min = amount;
        }

        Console.WriteLine("\n--- СТАТИСТИКА ---");
        Console.WriteLine($"Сумма: {total:F2} руб.");
        Console.WriteLine($"Среднее: {total / amounts.Count:F2} руб.");
        Console.WriteLine($"Максимум: {max:F2} руб.");
        Console.WriteLine($"Минимум: {min:F2} руб.");
    }

    static void BubbleSort(List<string> names, List<double> amounts)
    {
        for (int i = 0; i < amounts.Count - 1; i++)
        {
            for (int j = 0; j < amounts.Count - i - 1; j++)
            {
                if (amounts[j] > amounts[j + 1])
                {
                    (amounts[j], amounts[j + 1]) = (amounts[j + 1], amounts[j]);
                    (names[j], names[j + 1]) = (names[j + 1], names[j]);
                }
            }
        }
        Console.WriteLine("Данные отсортированы!");
    }

    static void ConvertCurrency(List<double> amounts)
    {
        Console.WriteLine("\nВалюты: 1-USD, 2-EUR, 3-CNY, 4-Другая");
        Console.Write("Выберите валюту: ");
        string choice = Console.ReadLine();

        double rate;
        string currency;

        switch (choice)
        {
            case "1": rate = 0.011; currency = "USD"; break;
            case "2": rate = 0.010; currency = "EUR"; break;
            case "3": rate = 0.080; currency = "CNY"; break;
            case "4":
                Console.Write("Введите курс: ");
                if (!double.TryParse(Console.ReadLine(), out rate) || rate <= 0) return;
                Console.Write("Введите валюту: ");
                currency = Console.ReadLine();
                break;
            default: Console.WriteLine("Неверный выбор"); return;
        }

        double total = 0;
        foreach (var amount in amounts) total += amount;

        Console.WriteLine($"\nКонвертация в {currency}:");
        Console.WriteLine($"Общее: {total:F2} руб. = {total * rate:F2} {currency}");
    }

    static void SearchByName(List<string> names, List<double> amounts)
    {
        Console.Write("Введите часть названия: ");
        string search = Console.ReadLine().ToLower();

        bool found = false;
        for (int i = 0; i < names.Count; i++)
        {
            if (names[i].ToLower().Contains(search))
            {
                Console.WriteLine($"{names[i],-25} {amounts[i],8:F2} руб.");
                found = true;
            }
        }
        if (!found) Console.WriteLine("Ничего не найдено");
    }
}