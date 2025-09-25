using System;
using System.Collections.Generic;
using System.Linq;

namespace StoreApp
{
    // Категории товаров для удобной сортировки
    public enum Category { Электроника, Одежда, Еда, Книги, Спорт }

    // Класс товара - здесь хранится вся информация о каждом товаре
    public class Product
    {
        public string Code { get; set; }          // Уникальный код товара (как артикул)
        public string Name { get; set; }          // Название товара
        public decimal Price { get; set; }        // Цена за штуку
        public int Quantity { get; set; }         // Сколько штук на складе
        public bool InStock => Quantity > 0;      // Автоматически проверяет есть ли в наличии
        public Category Category { get; set; }    // К какой категории относится

        // Создаем новый товар
        public Product(string name, decimal price, int quantity, Category category, string code)
        {
            Code = code;
            Name = name;
            Price = price;
            Quantity = quantity;
            Category = category;
        }

        // Красиво выводим информацию о товаре
        public override string ToString()
        {
            return $"{Code} | {Name} | {Price}₽ | {Quantity}шт | {Category} | {(InStock ? "В наличии" : "Нет")}";
        }
    }

    // Класс для записи каждой продажи - чтобы помнить что и когда продали
    public class Sale
    {
        public string ProductCode { get; set; }   // Код товара который продали
        public string ProductName { get; set; }   // Название товара
        public int Quantity { get; set; }         // Сколько штук продали
        public decimal Total { get; set; }        // На какую сумму продали
        public DateTime Date { get; set; }        // Когда продали

        // Записываем информацию о продаже
        public Sale(string code, string name, int quantity, decimal total)
        {
            ProductCode = code;
            ProductName = name;
            Quantity = quantity;
            Total = total;
            Date = DateTime.Now;  // Автоматически ставим текущее время
        }

        // Красиво выводим информацию о продаже
        public override string ToString()
        {
            return $"{Date:dd.MM HH:mm} - {ProductName} ({Quantity}шт) - {Total}₽";
        }
    }

    class Program
    {
        // Список всех товаров в магазине
        static List<Product> products = new List<Product>();
        // История продаж (стек чтобы можно было отменять последние продажи)
        static Stack<Sale> sales = new Stack<Sale>();
        // Счетчик для генерации уникальных кодов товаров
        static int nextId = 1001;

        // Главная функция которая запускается при старте программы
        static void Main()
        {
            AddTestData();  // Добавляем тестовые товары чтобы не вводить вручную
            RunMenu();      // Запускаем главное меню
        }

        // Добавляем несколько товаров для примера
        static void AddTestData()
        {
            AddProduct("Наушики", 10000, 10, Category.Электроника);
            AddProduct("Куртка", 3000, 150, Category.Одежда);
            AddProduct("Хлебцы", 50, 100, Category.Еда);
            AddProduct("Учебник", 1200, 15, Category.Книги);
            AddProduct("Наколенники", 5000, 20, Category.Спорт);
        }

        // Функция для добавления нового товара в список
        static void AddProduct(string name, decimal price, int quantity, Category category)
        {
            string code = "1" + nextId++;  // Генерируем уникальный код типа "11001", "11002" и т.д.
            products.Add(new Product(name, price, quantity, category, code));
        }

        // Главное меню программы - здесь пользователь выбирает что делать
        static void RunMenu()
        {
            while (true)  // Работаем пока не выберем выход
            {
                Console.Clear();  // Очищаем экран перед показом меню
                Console.WriteLine("🏪 Магазин - Учет товаров");
                Console.WriteLine("1. Все товары");           // Посмотреть весь список товаров
                Console.WriteLine("2. Добавить товар");       // Добавить новый товар
                Console.WriteLine("3. Удалить товар");        // Удалить товар из системы
                Console.WriteLine("4. Поставка");            // Пополнить количество товара
                Console.WriteLine("5. Продажа");             // Продать товар
                Console.WriteLine("6. Поиск");               // Найти товар по разным критериям
                Console.WriteLine("7. История продаж");      // Посмотреть все продажи
                Console.WriteLine("8. Отчет");               // Статистика по продажам
                Console.WriteLine("9. Отменить продажу");    // Отменить последнюю продажу (возврат)
                Console.WriteLine("0. Выход");               // Закрыть программу
                Console.Write("Выберите: ");

                // В зависимости от выбора запускаем нужную функцию
                switch (Console.ReadLine())
                {
                    case "1": ShowProducts(); break;      // Показать все товары
                    case "2": AddNewProduct(); break;     // Добавить новый товар
                    case "3": DeleteProduct(); break;     // Удалить товар
                    case "4": SupplyProduct(); break;     // Пополнить склад
                    case "5": SellProduct(); break;       // Продать товар
                    case "6": SearchProducts(); break;    // Поиск товаров
                    case "7": ShowSales(); break;         // История продаж
                    case "8": ShowReport(); break;        // Отчет
                    case "9": UndoSale(); break;          // Отмена продажи
                    case "0": return;                     // Выход
                }
            }
        }

        // Показываем все товары которые есть в системе
        static void ShowProducts()
        {
            Console.Clear();
            Console.WriteLine("📦 Список товаров:\n");
            // Проходим по всем товарам и выводим каждый
            foreach (var p in products) Console.WriteLine(p);
            Wait();  // Ждем пока пользователь посмотрит
        }

        // Добавляем новый товар через консоль
        static void AddNewProduct()
        {
            Console.Clear();
            Console.WriteLine("➕ Добавление товара");

            try  // На случай если пользователь введет что-то неправильно
            {
                Console.Write("Название: ");
                string name = Console.ReadLine();
                if (string.IsNullOrEmpty(name)) throw new Exception("Название обязательно");

                Console.Write("Цена: ");
                if (!decimal.TryParse(Console.ReadLine(), out decimal price) || price <= 0)
                    throw new Exception("Цена должна быть > 0");

                Console.Write("Количество: ");
                if (!int.TryParse(Console.ReadLine(), out int quantity) || quantity < 0)
                    throw new Exception("Количество не может быть отрицательным");

                Console.WriteLine("Категории: 0-Электроника, 1-Одежда, 2-Еда, 3-Книги, 4-Спорт");
                Console.Write("Категория: ");
                if (!Enum.TryParse(Console.ReadLine(), out Category category))
                    throw new Exception("Неверная категория");

                // Если все ввели правильно - добавляем товар
                AddProduct(name, price, quantity, category);
                Console.WriteLine("✅ Товар добавлен!");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Ошибка: {ex.Message}");
            }
            Wait();
        }

        // Удаляем товар по коду
        static void DeleteProduct()
        {
            Console.Clear();
            Console.Write("Введите код товара для удаления: ");
            string code = Console.ReadLine();

            // Ищем товар с таким кодом
            var product = products.FirstOrDefault(p => p.Code == code);
            if (product != null)
            {
                products.Remove(product);  // Удаляем из списка
                Console.WriteLine("✅ Товар удален!");
            }
            else Console.WriteLine("❌ Товар не найден!");
            Wait();
        }

        // Пополняем количество товара на складе (поставка)
        static void SupplyProduct()
        {
            Console.Clear();
            Console.Write("Код товара для поставки: ");
            string code = Console.ReadLine();

            // Ищем товар
            var product = products.FirstOrDefault(p => p.Code == code);
            if (product == null)
            {
                Console.WriteLine("❌ Товар не найден!");
                Wait();
                return;
            }

            Console.Write("Количество: ");
            if (int.TryParse(Console.ReadLine(), out int quantity) && quantity > 0)
            {
                product.Quantity += quantity;  // Увеличиваем количество
                Console.WriteLine($"✅ Поставка добавлена! Теперь: {product.Quantity}шт");
            }
            else Console.WriteLine("❌ Неверное количество!");
            Wait();
        }

        // Продаем товар - уменьшаем количество и записываем продажу
        static void SellProduct()
        {
            Console.Clear();
            Console.Write("Код товара для продажи: ");
            string code = Console.ReadLine();

            var product = products.FirstOrDefault(p => p.Code == code);
            if (product == null)
            {
                Console.WriteLine("❌ Товар не найден!");
                Wait();
                return;
            }

            // Проверяем есть ли товар в наличии
            if (!product.InStock)
            {
                Console.WriteLine("❌ Товара нет в наличии!");
                Wait();
                return;
            }

            Console.Write("Количество для продажи: ");
            if (int.TryParse(Console.ReadLine(), out int quantity) && quantity > 0)
            {
                // Проверяем что хотим продать не больше чем есть
                if (quantity <= product.Quantity)
                {
                    product.Quantity -= quantity;  // Уменьшаем количество на складе
                    decimal total = product.Price * quantity;  // Считаем сумму продажи
                    
                    // Записываем продажу в историю
                    sales.Push(new Sale(product.Code, product.Name, quantity, total));
                    Console.WriteLine($"✅ Продано! Сумма: {total}₽, Остаток: {product.Quantity}шт");
                }
                else Console.WriteLine($"❌ Недостаточно! В наличии: {product.Quantity}шт");
            }
            else Console.WriteLine("❌ Неверное количество!");
            Wait();
        }

        // Поиск товаров по разным критериям
        static void SearchProducts()
        {
            Console.Clear();
            Console.WriteLine("🔍 Поиск товаров");
            Console.WriteLine("1 - По коду, 2 - По названию, 3 - По категории");
            Console.Write("Выберите: ");

            var choice = Console.ReadLine();
            IEnumerable<Product> results = null;  // Здесь будут результаты поиска

            // В зависимости от выбора ищем по-разному
            switch (choice)
            {
                case "1":  // Поиск по коду
                    Console.Write("Код: ");
                    results = products.Where(p => p.Code.Contains(Console.ReadLine()));
                    break;
                case "2":  // Поиск по названию
                    Console.Write("Название: ");
                    string name = Console.ReadLine();
                    results = products.Where(p => p.Name.Contains(name, StringComparison.OrdinalIgnoreCase));
                    break;
                case "3":  // Поиск по категории
                    Console.Write("Категория (0-4): ");
                    if (Enum.TryParse(Console.ReadLine(), out Category category))
                        results = products.Where(p => p.Category == category);
                    break;
            }

            Console.WriteLine("\nРезультаты:");
            if (results != null && results.Any())
                foreach (var p in results) Console.WriteLine(p);
            else Console.WriteLine("Ничего не найдено");
            Wait();
        }

        // Показываем всю историю продаж
        static void ShowSales()
        {
            Console.Clear();
            Console.WriteLine("📊 История продаж:\n");
            foreach (var sale in sales) Console.WriteLine(sale);
            Wait();
        }

        // Показываем статистику по продажам
        static void ShowReport()
        {
            Console.Clear();
            Console.WriteLine("📈 Отчет о продажах");
            Console.WriteLine($"Всего продаж: {sales.Count}");
            Console.WriteLine($"Общая сумма: {sales.Sum(s => s.Total)}₽");
            Console.WriteLine($"Продано товаров: {sales.Sum(s => s.Quantity)}шт");
            Wait();
        }

        // Отменяем последнюю продажу (возврат товара)
        static void UndoSale()
        {
            Console.Clear();
            if (sales.Count == 0)
            {
                Console.WriteLine("❌ Нет продаж для отмены!");
                Wait();
                return;
            }

            // Берем последнюю продажу из стека
            var lastSale = sales.Pop();
            // Находим товар который продавали
            var product = products.FirstOrDefault(p => p.Code == lastSale.ProductCode);
            if (product != null)
            {
                product.Quantity += lastSale.Quantity;  // Возвращаем товар на склад
                Console.WriteLine($"✅ Отменена продажа: {lastSale.ProductName}");
            }
            else Console.WriteLine("❌ Товар не найден!");
            Wait();
        }

        // Простая функция чтобы программа ждала пока я прочитаю информацию
        static void Wait()
        {
            Console.WriteLine("\nНажмите Enter...");
            Console.ReadLine();
        }
    }
}
