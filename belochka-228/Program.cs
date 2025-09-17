using System;
using System.Collections.Generic;
using System.Linq;

namespace StoreApp
{
    public enum Category { Электроника, Одежда, Еда, Книги, Спорт }

    public class Product
    {
        public string Code { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }
        public bool InStock => Quantity > 0;
        public Category Category { get; set; }

        public Product(string name, decimal price, int quantity, Category category, string code)
        {
            Code = code;
            Name = name;
            Price = price;
            Quantity = quantity;
            Category = category;
        }

        public override string ToString()
        {
            return $"{Code} | {Name} | {Price}₽ | {Quantity}шт | {Category} | {(InStock ? "В наличии" : "Нет")}";
        }
    }

    public class Sale
    {
        public string ProductCode { get; set; }
        public string ProductName { get; set; }
        public int Quantity { get; set; }
        public decimal Total { get; set; }
        public DateTime Date { get; set; }

        public Sale(string code, string name, int quantity, decimal total)
        {
            ProductCode = code;
            ProductName = name;
            Quantity = quantity;
            Total = total;
            Date = DateTime.Now;
        }

        public override string ToString()
        {
            return $"{Date:dd.MM HH:mm} - {ProductName} ({Quantity}шт) - {Total}₽";
        }
    }

    class Program
    {
        static List<Product> products = new List<Product>();
        static Stack<Sale> sales = new Stack<Sale>();
        static int nextId = 1001;

        static void Main()
        {
            AddTestData();
            RunMenu();
        }

        static void AddTestData()
        {
            AddProduct("Ноутбук", 50000, 5, Category.Электроника);
            AddProduct("Футболка", 1500, 20, Category.Одежда);
            AddProduct("Шоколадка", 80, 100, Category.Еда);
            AddProduct("Учебник", 1200, 15, Category.Книги);
            AddProduct("Мяч", 2000, 8, Category.Спорт);
        }

        static void AddProduct(string name, decimal price, int quantity, Category category)
        {
            string code = "1" + nextId++;
            products.Add(new Product(name, price, quantity, category, code));
        }

        static void RunMenu()
        {
            while (true)
            {
                Console.Clear();
                Console.WriteLine("🏪 Магазин - Учет товаров");
                Console.WriteLine("1. Все товары");
                Console.WriteLine("2. Добавить товар");
                Console.WriteLine("3. Удалить товар");
                Console.WriteLine("4. Поставка");
                Console.WriteLine("5. Продажа");
                Console.WriteLine("6. Поиск");
                Console.WriteLine("7. История продаж");
                Console.WriteLine("8. Отчет");
                Console.WriteLine("9. Отменить продажу");
                Console.WriteLine("0. Выход");
                Console.Write("Выберите: ");

                switch (Console.ReadLine())
                {
                    case "1": ShowProducts(); break;
                    case "2": AddNewProduct(); break;
                    case "3": DeleteProduct(); break;
                    case "4": SupplyProduct(); break;
                    case "5": SellProduct(); break;
                    case "6": SearchProducts(); break;
                    case "7": ShowSales(); break;
                    case "8": ShowReport(); break;
                    case "9": UndoSale(); break;
                    case "0": return;
                }
            }
        }

        static void ShowProducts()
        {
            Console.Clear();
            Console.WriteLine("📦 Список товаров:\n");
            foreach (var p in products) Console.WriteLine(p);
            Wait();
        }
        static void AddNewProduct()
        {
            Console.Clear();
            Console.WriteLine("➕ Добавление товара");

            try
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

                AddProduct(name, price, quantity, category);
                Console.WriteLine("✅ Товар добавлен!");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Ошибка: {ex.Message}");
            }
            Wait();
        }

        static void DeleteProduct()
        {
            Console.Clear();
            Console.Write("Введите код товара для удаления: ");
            string code = Console.ReadLine();

            var product = products.FirstOrDefault(p => p.Code == code);
            if (product != null)
            {
                products.Remove(product);
                Console.WriteLine("✅ Товар удален!");
            }
            else Console.WriteLine("❌ Товар не найден!");
            Wait();
        }

        static void SupplyProduct()
        {
            Console.Clear();
            Console.Write("Код товара для поставки: ");
            string code = Console.ReadLine();

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
                product.Quantity += quantity;
                Console.WriteLine($"✅ Поставка добавлена! Теперь: {product.Quantity}шт");
            }
            else Console.WriteLine("❌ Неверное количество!");
            Wait();
        }

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

            if (!product.InStock)
            {
                Console.WriteLine("❌ Товара нет в наличии!");
                Wait();
                return;
            }

            Console.Write("Количество для продажи: ");
            if (int.TryParse(Console.ReadLine(), out int quantity) && quantity > 0)
            {
                if (quantity <= product.Quantity)
                {
                    product.Quantity -= quantity;
                    decimal total = product.Price * quantity;
                    sales.Push(new Sale(product.Code, product.Name, quantity, total));
                    Console.WriteLine($"✅ Продано! Сумма: {total}₽, Остаток: {product.Quantity}шт");
                }
                else Console.WriteLine($"❌ Недостаточно! В наличии: {product.Quantity}шт");
            }
            else Console.WriteLine("❌ Неверное количество!");
            Wait();
        }
        static void SearchProducts()
        {
            Console.Clear();
            Console.WriteLine("🔍 Поиск товаров");
            Console.WriteLine("1 - По коду, 2 - По названию, 3 - По категории");
            Console.Write("Выберите: ");

            var choice = Console.ReadLine();
            IEnumerable<Product> results = null;

            switch (choice)
            {
                case "1":
                    Console.Write("Код: ");
                    results = products.Where(p => p.Code.Contains(Console.ReadLine()));
                    break;
                case "2":
                    Console.Write("Название: ");
                    string name = Console.ReadLine();
                    results = products.Where(p => p.Name.Contains(name, StringComparison.OrdinalIgnoreCase));
                    break;
                case "3":
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

        static void ShowSales()
        {
            Console.Clear();
            Console.WriteLine("📊 История продаж:\n");
            foreach (var sale in sales) Console.WriteLine(sale);
            Wait();
        }

        static void ShowReport()
        {
            Console.Clear();
            Console.WriteLine("📈 Отчет о продажах");
            Console.WriteLine($"Всего продаж: {sales.Count}");
            Console.WriteLine($"Общая сумма: {sales.Sum(s => s.Total)}₽");
            Console.WriteLine($"Продано товаров: {sales.Sum(s => s.Quantity)}шт");
            Wait();
        }

        static void UndoSale()
        {
            Console.Clear();
            if (sales.Count == 0)
            {
                Console.WriteLine("❌ Нет продаж для отмены!");
                Wait();
                return;
            }

            var lastSale = sales.Pop();
            var product = products.FirstOrDefault(p => p.Code == lastSale.ProductCode);
            if (product != null)
            {
                product.Quantity += lastSale.Quantity;
                Console.WriteLine($"✅ Отменена продажа: {lastSale.ProductName}");
            }
            else Console.WriteLine("❌ Товар не найден!");
            Wait();
        }

        static void Wait()
        {
            Console.WriteLine("\nНажмите Enter...");
            Console.ReadLine();
        }
    }
}