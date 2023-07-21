using NUnit.Framework;
using Playwright;
using System;

[TestFixture]
public class DodopizzaTests
{
    private IPlaywright playwright;
    private IBrowser browser;
    private IPage page;

    private string baseUrl;
    private int timeoutInSeconds;
    private int maxRetries;

    [SetUp]
    public async Task SetUp()
    {
        playwright = await Playwright.CreateAsync();
        browser = await playwright.Chromium.LaunchAsync();
        page = await browser.NewPageAsync();

        baseUrl = AppSettings.GetBaseUrl();
        timeoutInSeconds = AppSettings.GetTimeoutInSeconds();
        maxRetries = AppSettings.GetMaxRetries();
    }

    [TearDown]
    public async Task TearDown()
    {
        await browser.CloseAsync();
        await playwright.DisposeAsync();
    }

    [Test]
    public async Task PizzaSectionDisplaysCorrectNumberOfItems()
    {
        // Step 1: Go to 'https://dodopizza.ru/'
        await page.GotoAsync("https://dodopizza.ru/");

        // Step 2: Check the total number of items in the "Pizza" section
        var pizzaSection = await page.QuerySelectorAsync("#pizza-section");
        var pizzaItems = await pizzaSection.QuerySelectorAllAsync(".pizza-item");
        var numberOfItems = pizzaItems.Length;

        // Step 3: Check the region name displayed next to "Доставка пиццы"
        var regionName = await page.TextContentAsync(".region-name");

        // Expected Results
        const string expectedUrl = "https://dodopizza.ru/moscow";
        const int expectedNumberOfItems = 34;
        const string expectedRegionName = "Москва";

        Assert.AreEqual(expectedUrl, page.Url);
        Assert.AreEqual(expectedNumberOfItems, numberOfItems);
        Assert.AreEqual(expectedRegionName, regionName);
    }

     [Test]
    public async Task AddPizzaToCart()
    {
        // Step 1: Go to 'https://dodopizza.ru/'
        await page.GotoAsync("https://dodopizza.ru/");

        // Step 2: Add a random pizza from the "Pizza" section to the cart
        var pizzaSection = await page.QuerySelectorAsync("#pizza-section");
        var pizzaItems = await pizzaSection.QuerySelectorAllAsync(".pizza-item");
        var randomIndex = new Random().Next(0, pizzaItems.Length);
        var randomPizza = pizzaItems[randomIndex];
        await randomPizza.ClickAsync(".button-select");

        // Step 3: Select the small size of the pizza
        await page.ClickAsync(".button-size-small");

        // Step 4: Add to cart
        await page.ClickAsync(".button-add-to-cart");

        // Expected Results
        const string expectedUrl = "https://dodopizza.ru/";
        var expectedPizzaName = await randomPizza.TextContentAsync(".pizza-item__title");
        var expectedPrice = await page.TextContentAsync(".button-add-to-cart");

        Assert.AreEqual(expectedUrl, page.Url);

        var addedPizzaName = await page.TextContentAsync(".cart-item__title");
        Assert.AreEqual(expectedPizzaName, addedPizzaName);

        var priceRegex = new Regex(@"\d+");
        var addedPriceMatch = priceRegex.Match(await page.TextContentAsync(".button-add-to-cart"));
        var mainPagePriceMatch = priceRegex.Match(expectedPrice);
        Assert.AreEqual(mainPagePriceMatch.Value, addedPriceMatch.Value);

        var cartItemCount = await page.TextContentAsync(".cart-button__count");
        Assert.AreEqual("1", cartItemCount);
    }

    [Test]
    public async Task AddMultiplePizzasToCart()
    {
        Console.WriteLine("Step 1: Go to '{0}'", baseUrl);
        await page.GotoAsync(baseUrl);

        Console.WriteLine("Step 2: Add 5 random pizzas from the 'Pizza' section to the cart");
        var pizzaSection = await page.QuerySelectorAsync("#pizza-section");
        var pizzaItems = await pizzaSection.QuerySelectorAllAsync(".pizza-item");
        var random = new Random();

        for (int i = 0; i < 5; i++)
        {
            var randomIndex = random.Next(0, pizzaItems.Length);
            var randomPizza = pizzaItems[randomIndex];
            await randomPizza.ClickAsync(".button-select");
            await page.ClickAsync(".button-size-small");
            await page.ClickAsync(".button-add-to-cart");
            await page.WaitForTimeoutAsync(timeoutInSeconds * 1000); // Wait a bit before adding the next pizza
        }

        Console.WriteLine("Step 3: Click on the cart button");
        await page.ClickAsync(".cart-button");

        // Expected Results
        const string expectedUrl = baseUrl;
        const string expectedCartItemCount = "5";

        Assert.AreEqual(expectedUrl, page.Url);

        var cartItemCount = await page.TextContentAsync(".cart-button__count");
        Assert.AreEqual(expectedCartItemCount, cartItemCount);

        var addedPizzaNames = await page.TextContentAsync(".cart-item__title");
        var addedPizzas = addedPizzaNames.Split("\n");

        Assert.AreEqual(5, addedPizzas.Length);

        Console.WriteLine("Test completed successfully.");
    }
}