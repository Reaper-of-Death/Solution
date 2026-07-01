using System;
using System.Text;

namespace TaskOOP
{
    public class Product
    {
        private string _name;
        private string _manufacturer;
        private double _cost;
        private string _expirationDate;
        private string _productionDate;

        public Product(string name, string manufacturer, double cost, string expirationDate, string productionDate)
        {
            _name = name;
            _manufacturer = manufacturer;
            _cost = cost;
            _expirationDate = expirationDate;
            _productionDate = productionDate;
        }

        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }

        public string Manufacturer
        {
            get { return _manufacturer; }
            set { _manufacturer = value; }
        }

        public double Cost
        {
            get { return _cost; }
            set 
            { 
                if (value > 0)
                    _cost = value;
                else
                    Console.WriteLine("Cost must be positive!");
            }
        }

        public string ExpirationDate
        {
            get { return _expirationDate; }
            set { _expirationDate = value; }
        }

        public string ProductionDate
        {
            get { return _productionDate; }
            set { _productionDate = value; }
        }

        public override string ToString()
        {
            return $"Имя продукта: {_name}\n" +
                   $"Имя производителя: {_manufacturer}\n" +
                   $"Цена: {_cost:C}\n" +
                   $"Дата производства: {_productionDate}\n" +
                   $"Дата истечения срока годности: {_expirationDate}\n";
        }
    }

    public class DiscountedProduct : Product
    {
        private double _discountPercentage;
        private double _discountedPrice;

        public DiscountedProduct(string name, string manufacturer, double cost, 
                                string expirationDate, string productionDate, 
                                double discountPercentage) 
            : base(name, manufacturer, cost, expirationDate, productionDate)
        {
            DiscountPercentage = discountPercentage;
        }

        public double DiscountPercentage
        {
            get { return _discountPercentage; }
            set 
            { 
                if (value >= 0 && value <= 100)
                {
                    _discountPercentage = value;
                    UpdateDiscountedPrice();
                }
                else
                {
                    Console.WriteLine("Скидка должна быть от 0 до 100%!");
                    _discountPercentage = 0;
                    UpdateDiscountedPrice();
                }
            }
        }

        public double DiscountedPrice
        {
            get { return _discountedPrice; }
            private set { _discountedPrice = value; }
        }

        private void UpdateDiscountedPrice()
        {
            _discountedPrice = Cost * (1 - _discountPercentage / 100);
        }

        public new double Cost
        {
            get { return base.Cost; }
            set 
            { 
                base.Cost = value;
                UpdateDiscountedPrice();
            }
        }

        public override string ToString()
        {
            return base.ToString() +
                   $"Размер скидки: {_discountPercentage:F2}%\n" +
                   $"Акционная цена: {_discountedPrice:C}\n";
        }
    }

    public class Program
    {
        public static void Main(string[] args)
        {
            Console.OutputEncoding = Encoding.UTF8;
            Console.InputEncoding = Encoding.UTF8;

            Console.Write("Введите имя продукта: ");
            string name = Console.ReadLine();

            Console.Write("Введите имя производителя: ");
            string manufacturer = Console.ReadLine();

            Console.Write("Введите цену: ");
            double cost;
            while (!double.TryParse(Console.ReadLine(), out cost) || cost <= 0)
            {
                Console.Write("Некорректный ввод! Цена должна быть положительной. Введите цену: ");
            }

            Console.Write("Введите дату производства (дд.мм.гггг): ");
            string productionDate = Console.ReadLine();

            Console.Write("Введите дату истечения срока годности (дд.мм.гггг): ");
            string expirationDate = Console.ReadLine();

            Console.Write("Введите размер скидки (%): ");
            double discount;
            while (!double.TryParse(Console.ReadLine(), out discount) || discount < 0 || discount > 100)
            {
                Console.Write("Некорректный ввод! Скидка должна быть от 0 до 100%. Введите скидку: ");
            }

            DiscountedProduct discountedProduct = new DiscountedProduct(
                name, manufacturer, cost, expirationDate, productionDate, discount);

            Console.WriteLine("\n=== Информация о товаре со скидкой ===");
            Console.WriteLine(discountedProduct.ToString());

            Console.WriteLine("\n=== Демонстрация свойств товара со скидкой ===");
            Console.WriteLine($"Имя продукта: {discountedProduct.Name}");
            Console.WriteLine($"Имя производителя: {discountedProduct.Manufacturer}");
            Console.WriteLine($"Обычная цена: {discountedProduct.Cost:C}");
            Console.WriteLine($"Размер скидки: {discountedProduct.DiscountPercentage:F2}%");
            Console.WriteLine($"Акционная цена: {discountedProduct.DiscountedPrice:C}");
            Console.WriteLine($"Дата производства: {discountedProduct.ProductionDate}");
            Console.WriteLine($"Дата истечения срока годности: {discountedProduct.ExpirationDate}");

            Console.WriteLine("\n=== Изменение информации ===");
            Console.Write("Введите новую цену: ");
            double newCost;
            if (double.TryParse(Console.ReadLine(), out newCost) && newCost > 0)
            {
                discountedProduct.Cost = newCost;
                Console.WriteLine($"Измененная обычная цена: {discountedProduct.Cost:C}");
                Console.WriteLine($"Новая акционная цена: {discountedProduct.DiscountedPrice:C}");
            }
            else
            {
                Console.WriteLine("Некорректный ввод! Цена должна быть положительной.");
            }

            Console.Write("Введите новый размер скидки (%): ");
            double newDiscount;
            if (double.TryParse(Console.ReadLine(), out newDiscount) && newDiscount >= 0 && newDiscount <= 100)
            {
                discountedProduct.DiscountPercentage = newDiscount;
                Console.WriteLine($"Новый размер скидки: {discountedProduct.DiscountPercentage:F2}%");
                Console.WriteLine($"Новая акционная цена: {discountedProduct.DiscountedPrice:C}");
            }
            else
            {
                Console.WriteLine("Некорректный ввод! Скидка должна быть от 0 до 100%.");
            }

            Console.WriteLine("\n=== Обновленная информация о товаре со скидкой ===");
            Console.WriteLine(discountedProduct.ToString());
        }
    }
}