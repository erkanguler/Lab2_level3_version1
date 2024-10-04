using System.Globalization;

static class App
{

    const string APP_MENU = "\nTo enter a new product - follow the steps | To quit - enter: \"q\"";
    const string APP_SUBMENU = """
            To enter a new product - enter: "p" | To search for a product - enter: "s" | To quit - enter "q"
            """;

    static void Main()
    {
        var productList = new ProductList();
        string? categoryName;
        string? productName;
        float? price;

        while (true)
        {
            PrintTextWithColor(APP_MENU, ConsoleColor.DarkYellow);
            string? input = null;

            while (true)
            {
            GoToCategory:
                Console.Write("Enter a Category: "); // Set category name
                input = Console.ReadLine();

                if (String.IsNullOrWhiteSpace(input))
                    continue;

                categoryName = input!.Trim(); // End of 

                if (IsTokenEqual(input!, "q"))
                {
                    productList.PrintAllProducts();

                    while (true) // Print and handle submenu
                    {
                        PrintTextWithColor(APP_SUBMENU, ConsoleColor.Blue);

                        if (String.IsNullOrWhiteSpace(input = Console.ReadLine()))
                            continue;

                        input = input!.Trim();

                        if (IsTokenEqual(input!, "q"))
                            Environment.Exit(0);
                        else if (IsTokenEqual(input!, "p"))
                            goto GoToCategory;
                        else if (IsTokenEqual(input!, "s")) // End of
                        {
                            while (true) // Search a product by name
                            {
                                Console.Write("\nEnter a Product Name: ");
                                string? prodName = Console.ReadLine();

                                if (String.IsNullOrWhiteSpace(prodName))
                                    continue;

                                prodName = prodName!.Trim();

                                if (productList.Contains(prodName!))
                                    productList.ColorPrintProducts(prodName!);
                                else
                                    PrintTextWithColor($"\nNo product with that name, {prodName}\n", ConsoleColor.Red);

                                break;
                            }
                        }
                    }
                }

                break;
            }

            while (true) // Set product name
            {
                Console.Write("Enter a Product Name: ");
                input = Console.ReadLine();

                if (String.IsNullOrWhiteSpace(input))
                    continue;

                productName = input!.Trim();

                break;
            }

            while (true) // Set price
            {
                Console.Write("Enter a Price: ");

                price = GetPriceAsFloat(input = Console.ReadLine());
                if (price is null)
                    continue;

                break;
            }

            productList.Add(new Product { Category = categoryName!, Name = productName!, Price = (float)price });
            PrintTextWithColor("\nThe product was successfully added.\n", ConsoleColor.Green);
        }
    }

    public static bool IsTokenEqual(string str1, string str2)
    {
        return str1.Trim().Equals(str2.Trim(), StringComparison.OrdinalIgnoreCase);
    }

    public static float? GetPriceAsFloat(string? input)
    {
        float price;
        input = input!.Trim();
        var style = NumberStyles.AllowDecimalPoint;
        var culture = CultureInfo.CreateSpecificCulture("en-US");

        if (!float.TryParse(input, style, culture, out price) | price < 0)
        {
            PrintTextWithColor("\nProvide a price like 100 or 11.99.\n", ConsoleColor.Red);
            return null;
        }

        return price;
    }

    public static void PrintTextWithColor(string text, ConsoleColor color)
    {
        Console.ForegroundColor = color;
        Console.WriteLine(text);
        Console.ResetColor();
    }
}

class Product
{
    public required string Category { get; set; }
    public required string Name { get; set; }
    public required float Price { get; set; }
}

class ProductList
{
    public List<Product> Products = new List<Product>();

    public void Add(Product product)
    {
        Products.Add(product);
    }

    public bool Contains(string productName)
    {
        return Products.Any(product => IsProductNamesEqual(product.Name, productName));
    }

    public bool IsProductNamesEqual(string productName, string otherProductName)
    {
        return App.IsTokenEqual(productName, otherProductName);
    }

    public void ColorPrintProducts(string productName)
    {
        PrintHeader();

        foreach (Product product in GetOrderedProducts())
        {
            if (IsProductNamesEqual(product.Name, productName))
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                PrintProduct(product);
                Console.ResetColor();
            }
            else
                PrintProduct(product);
        }

        Console.WriteLine("-----------------------------------------------------\n");
    }

    private void PrintProduct(Product p)
    {
        Console.WriteLine($"{p.Category.PadRight(20)} {p.Name.PadRight(20)} {p.Price.ToString().PadRight(20)}");
    }

    private void PrintHeader()
    {
        string category = "CATEGORY";
        string product = "PRODUCT";
        string price = "PRICE";
        string header = $"{category.PadRight(20)} {product.PadRight(20)} {price.PadRight(20)}";

        Console.WriteLine("\n-----------------------------------------------------");
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine(header);
        Console.ResetColor();
    }

    public void PrintAllProducts()
    {
        List<Product> sortedProducts = GetOrderedProducts();

        if (sortedProducts.Count < 1)
        {
            App.PrintTextWithColor("\nYou have no product.\n", ConsoleColor.Red);
            return;
        }

        PrintHeader();

        foreach (Product product in sortedProducts)
            PrintProduct(product);

        Console.WriteLine();
        Console.CursorLeft = 25;
        Console.WriteLine($"Total amount:    {GetTotalAmount().ToString()}");

        Console.WriteLine("-----------------------------------------------------\n");
    }

    public List<Product> GetOrderedProducts()
    {
        return Products.OrderBy(product => product.Price).ToList();
    }

    public float GetTotalAmount()
    {
        return Products.Sum(product => product.Price);
    }
}
