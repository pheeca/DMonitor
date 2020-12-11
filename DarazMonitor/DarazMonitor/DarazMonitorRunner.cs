using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Support.UI;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using WebDriverManager;
using WebDriverManager.DriverConfigs.Impl;

 namespace DarazMonitor
{
    public class DarazMonitorRunner
    {
        IWebDriver driver;
        private TestContext testContextInstance;

        /// <summary>
        ///  Gets or sets the test context which provides
        ///  information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext
        {
            get { return testContextInstance; }
            set { testContextInstance = value; }
        }
        private ChromeOptions AddProxy(ChromeOptions options)
        {
            var proxy = new Proxy();
            proxy.Kind = ProxyKind.Manual;
            proxy.IsAutoDetect = false;
            proxy.HttpProxy =
            proxy.SslProxy = "216.158.229.67:3128";
            options.Proxy = proxy;
            options.AddArgument("ignore-certificate-errors");
            return options;
        }
        [SetUp]
        public void startBrowser()
        {
            //var options = new ChromeOptions(); 
            //options.AddArgument("--no-sandbox");
            //driver = new ChromeDriver(@"D:\ForDev\DarazMonitor\chromedriver_win32", options);
            //driver.Manage().Timeouts().PageLoad.Add(System.TimeSpan.FromSeconds(120));
            //driver.Manage().Timeouts().ImplicitWait.Add(TimeSpan.FromSeconds(120));
            new DriverManager().SetUpDriver(new ChromeConfig());
            var options = new ChromeOptions();
            options.AddExcludedArguments(new List<string>() { "enable-automation" });
            //options = AddProxy(options);
            //options.AddArguments("--disable-blink-features");
            //options.AddArguments("--disable-blink-features=AutomationControlled");
#if !DEBUG
             options.AddArguments("headless");
#endif

            driver = new ChromeDriver(options);
            //Map<String,Object> _params = new HashMap<String, Object>();
            //_params.put("source", "Object.defineProperty(navigator, 'webdriver', { get: () => undefined })");
            //driver.executeCdpCommand("Page.addScriptToEvaluateOnNewDocument", _params);
        }
        public void openSearchResult()
        {

            driver.Navigate().GoToUrl("https://www.daraz.pk/");
            //var x = driver.PageSource;
            Assert.True(driver.Title.Contains("Online Shopping"));
            Thread.Sleep(2 * 1000);
            var pushPopup = FindElement(By.CssSelector(".acc--denyLink"));
            pushPopup?.Click();
            var searchbar = FindElement(By.CssSelector("[type='search']"));
            searchbar.SendKeys("!!");
            var searchButton = FindElement(By.CssSelector("button[data-spm-click]"));
            searchButton?.Click();
            Thread.Sleep(5 * 1000);
            var minimumPrice = FindElement(By.CssSelector("input[placeholder='Min']"));
            minimumPrice.SendKeys("200");
            var maximumPrice = FindElement(By.CssSelector("input[placeholder='Max']"));
            maximumPrice.SendKeys("1000");
            var priceSearch = FindElement(By.CssSelector("[placeholder='Max']+button"));
            priceSearch?.Click();
            var fourStar = FindElement(By.CssSelector("div[data-spm='filter']>div:nth-child(6)>div:nth-child(2)>div:nth-child(2)"));
            fourStar?.Click();
            Thread.Sleep(5 * 1000);

            IJavaScriptExecutor js = (IJavaScriptExecutor)driver;
            js.ExecuteScript("document.querySelector('#topActionHeader').hidden=true");
            js.ExecuteScript("document.querySelector('.im').hidden=true");
        }
        public PotentialProduct productDetailPage(PotentialProduct product)
        {
            driver.SwitchTo().NewWindow(WindowType.Tab);
            driver.Navigate().GoToUrl(product.LinkToProduct);
            var source = driver.PageSource;
            int.TryParse(source?.Split(new string[] { "\"stock\":" }, StringSplitOptions.None)?.ElementAtOrDefault(1)?.Split(',')?.FirstOrDefault(), out int stock);
            product.stockHistory.Add(new PotentialProductStock { Date = DateTime.Now, Stock = stock });
            product.category = source?.Split(new string[] { "\"dataLayer\":{\"pdt_category\":" }, StringSplitOptions.None).ElementAtOrDefault(1)?.Split(new string[] { ",\"pagetype\"" }, StringSplitOptions.None)?.FirstOrDefault();
            var arr = JArray.Parse(source?.Split(new string[] { "\"skuBase\":{\"properties\":" }, StringSplitOptions.None).ElementAtOrDefault(1)?.Split(new string[] { ",\"skus\":" }, StringSplitOptions.None)?.FirstOrDefault());
            product.VariantTypes = arr.Count;
            var isAgeRestricted = driver.FindElements(By.CssSelector(".age-restriction-btn-over")).Count > 0;
            product.isAgeRestricted = isAgeRestricted;
            if (isAgeRestricted)
            {
                driver.FindElement(By.CssSelector(".age-restriction-btn-over")).Click();
                Thread.Sleep(1000);
            }
            var popup = driver.FindElements(By.CssSelector(".next-overlay-inner i")).Count > 0;
            if (popup)
            {

                driver.FindElement(By.CssSelector(".next-overlay-inner i")).Click();
                Thread.Sleep(1000);
            }
            //
            //var ratingValue = source.Split(new string[] { "\"AggregateRating\",\"ratingValue\":\"" }, StringSplitOptions.None).ElementAtOrDefault(1)?.Split('"')?.FirstOrDefault();
            ScrollToBottom();
            IJavaScriptExecutor js = (IJavaScriptExecutor)driver;
            js.ExecuteScript("document.querySelector('#topActionHeader').hidden=true");
            js.ExecuteScript("document.querySelector('.im').hidden=true");
            Thread.Sleep(1000);
            ScrollToBottom(.3f);
            Thread.Sleep(200);
            ScrollToBottom(.3f);
            Thread.Sleep(200);
            ScrollToBottom(.3f);
            Thread.Sleep(200);
            try
            {

                var btn = driver.FindElements(By.CssSelector(".condition"))?.Last();

                Actions _a1 = new Actions(driver);
                _a1.MoveToElement(btn);
                _a1.Perform();
                //scrollDownByPixAmount("150");
                btn?.Click();
            }
            catch (Exception e)
            {
            }
            Thread.Sleep(200);
            driver.FindElement(By.CssSelector("[role='menuitem']:nth-child(2)")).Click();
            Thread.Sleep(200);
            var reviewIndex = 1;
            var inValidCats = new string[] { "TV, Audio / Video, Gaming & Wearables", "Women", "Makeup", "Groceries","Eye Care" ,"\"Electrical\"","Cables & Converters", "Computers & Laptops" ,"\"Watches\"","Memory Cards","Medical Supplies", "\"Digital Goods\"" , "\"Mobiles & Tablets\"" , "\"Moto Electronics\"" ,"Flat Irons"};
            var isValidCategory = inValidCats.All(ic => !product.category.Contains(ic));
            if (isValidCategory && product.VariantTypes < 2)
            {

            }
            product.isValidCategory = isValidCategory;
            while (true && isValidCategory && product.VariantTypes<2)
            {
                foreach (var titleItem in driver.FindElements(By.CssSelector(".title.right")))
                {
                    try
                    {
                        var _approxDate = titleItem.Text.AsDateTime();
                        if (_approxDate.HasValue)
                        {
                            product.Reviews.Add(_approxDate.Value);
                        }
                    }
                    catch (Exception)
                    {

                    }
                }
                reviewIndex += 1;
                var collection = driver.FindElements(By.CssSelector("#module_product_review .next-pagination-list .next-btn"));
                bool next = false;
                foreach (var item in collection)
                {
                    if (item.Text.Trim() == reviewIndex.ToString())
                    {
                        ScrollToBottom();
                        Thread.Sleep(1000);
                        ScrollToBottom(.3f);
                        Thread.Sleep(200);
                        ScrollToBottom(.3f);
                        Thread.Sleep(200);
                        ScrollToBottom(.3f);
                        Thread.Sleep(200);
                        Actions _actions = new Actions(driver);
                        _actions.MoveToElement(item);
                        _actions.Perform();

                        //scrollDownByPixAmount("150");
                        item.Click();
                        next = true;
                    }
                }
                Thread.Sleep(200);
                if (!next)
                {
                    break;
                }
            }
            return product;
        }
        [Test]
        public void test()
        {
            openSearchResult();
            var productPageIndex = 1;
            var skipTo = 103;
            if (skipTo>productPageIndex)
            {
                driver.Navigate().GoToUrl($"https://www.daraz.pk/catalog/?from=input&location=-178&page={skipTo}&price=200-1000&q=%21%21&rating=4");
                productPageIndex = skipTo;
            }
            //while (skipTo>productPageIndex)
            //{
            //    Thread.Sleep(5000);
            //    ScrollToBottom();
            //    IWebElement pageButton=null;
            //    while (FindElement(By.CssSelector("[title='" + (productPageIndex+1) + "']"))!=null)
            //    {
            //        productPageIndex += 1;
            //        pageButton = FindElement(By.CssSelector("[title='" + productPageIndex + "']"));
            //        ScrollToBottom();
            //        Thread.Sleep(200);
            //    }
            //    if (pageButton == null)
            //    {
            //        break;
            //    }

            //    Actions _a2 = new Actions(driver);
            //    _a2.MoveToElement(pageButton);
            //    _a2.Perform();
            //    scrollDownByPixAmount("150");
            //    pageButton?.Click();
            //    Thread.Sleep(1 * 1000);
            //}
            Thread.Sleep(5 * 1000);
            while (true)
            {
                var pageProducts = driver.FindElements(By.CssSelector("[data-sku-simple]"));
                if (pageProducts.Count==0)
                {
                    break;
                }
                foreach (var pageProductItem in pageProducts)
                {
                    Actions actions = new Actions(driver);
                    actions.MoveToElement(pageProductItem);
                    actions.Perform();
                    var _darazMallBanner = pageProductItem.GetElement(By.CssSelector("i.ic-dynamic-badge-lazMall"));
                    if (_darazMallBanner == null)
                    {
                        var anchor = pageProductItem.GetElement(By.CssSelector("a[age='0'][title]"));
                        var link = anchor.GetAttribute("href");
                                                                                       //    pageProductItem.
                        int.TryParse(pageProductItem.GetElement(By.CssSelector("div>span"))?.Text?.Replace("Rs.", "").Trim(' '), out int _price);
                        float.TryParse(pageProductItem.GetElement(By.CssSelector("i+span"))?.Text?.Trim('(', ')'), out float _ratings);

                        var product = new PotentialProduct
                        {
                            LinkToProduct = link,
                            Price = _price,
                            Title = anchor.Text,
                            Ratings = _ratings,
                            Reviews = new List<DateTime>(),
                            isValidCategory=false,
                             stockHistory=new List<PotentialProductStock>()
                        };
                        var _pLink = product.LinkToProduct.Split('/')?.LastOrDefault()?.Split('.')?.FirstOrDefault();
                        var _saveFile = @"D:\ForDev\DarazMonitor\Data\" + _pLink + ".json";
                        _saveFile=_saveFile.Substring(0,Math.Min(259, _saveFile.Length));
                        if (product.Ratings==0)
                        {

                        }
                        if (product.Ratings >= 50 && !File.Exists(_saveFile))
                        {
                            string originalWindow = driver.CurrentWindowHandle;
                            product = productDetailPage(product);
                            try
                            {
                                File.WriteAllText(_saveFile, JsonConvert.SerializeObject(product));
                            }
                            catch (Exception e)
                            {

                            }
                            
                            driver.Close();
                            driver.SwitchTo().Window(originalWindow);
                        }

                    }
                }
                productPageIndex += 1;
                Console.WriteLine(productPageIndex);
                var pageButton = FindElement(By.CssSelector("[title='" + productPageIndex + "']"));
                if (pageButton == null)
                {
                    break;
                }

                Actions _a2 = new Actions(driver);
                _a2.MoveToElement(pageButton);
                _a2.Perform();
                scrollDownByPixAmount("150");
                pageButton?.Click();
                Thread.Sleep(5 * 1000);
            }
        }
        public void scrollDownByPixAmount(string value)
        {
            var windowScroll = string.Format("window.scrollBy(0,{0})", value);
            IJavaScriptExecutor js = driver as IJavaScriptExecutor;
            js.ExecuteScript(windowScroll, "");

        }
        private void ScrollToBottom(float percent = 1.0f)
        {
            long scrollHeight = 0;

            do
            {
                IJavaScriptExecutor js = (IJavaScriptExecutor)driver;
                var newScrollHeight = (long)js.ExecuteScript("window.scrollTo(0, document.body.scrollHeight*" + percent + "); return document.body.scrollHeight;");

                if (newScrollHeight == scrollHeight)
                {
                    break;
                }
                else
                {
                    scrollHeight = newScrollHeight;
                    Thread.Sleep(400);
                }
            } while (true);
        }
        [TearDown]
        public void closeBrowser()
        {
            driver.Quit();
        }

        By last;
        int last_time = 0;
        private IWebElement FindElement(By by)
        {
            try
            {
                if (last == by)
                {
                    last_time += 1;
                }
                else
                {
                    last_time = 0;
                    last = by;
                }
                if (last_time >= 3)
                {
                    return null;
                }
                return driver.FindElement(by);
            }
            catch (Exception e)
            {
                if (!(e.GetType() == typeof(NoSuchElementException) || e.GetType() == typeof(WebDriverException)))
                {

                }
                try
                {

                    bool wait = new WebDriverWait(driver, TimeSpan.FromSeconds(60)).Until(d => ((IJavaScriptExecutor)d).ExecuteScript("return document.readyState").Equals("complete"));

                    if (wait == true)
                    {
                        return null;
                    }
                }
                catch (Exception ex)
                {
                }
                return FindElement(by);
            }
        }

        private void ScrollToElement(IWebElement element)
        {
            try
            {
                ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].scrollIntoView(true);", element);
                Thread.Sleep(500);
            }
            catch (Exception e)
            {
            }
        }
        private void CloseAllNotifications()
        {
            try
            {
                var _closerElements = driver.FindElements(By.CssSelector("#asyncProgressBar > span > div > span.pul-toast__action > button > span > span.pul-icon.pul-icon--size-12.pul-icon--on-dark.pul-button__icon"));
                while (_closerElements.Count() > 0)
                {
                    _closerElements[0].Click();
                    _closerElements = driver.FindElements(By.CssSelector("#asyncProgressBar > span > div > span.pul-toast__action > button > span > span.pul-icon.pul-icon--size-12.pul-icon--on-dark.pul-button__icon"));
                }
            }
            catch (Exception e)
            {
                //if (!(e.GetType() == typeof(NoSuchElementException) || e.GetType() == typeof(WebDriverException)))
                //{

                //}
                //bool wait = new WebDriverWait(driver, TimeSpan.FromSeconds(60)).Until(d => ((IJavaScriptExecutor)d).ExecuteScript("return document.readyState").Equals("complete"));

                //if (wait == true)
                //{
                //    return;
                //}
                CloseAllNotifications();
            }
        }

        private void WaitWhileNotificationProgress()
        {
            try
            {
                var _closerElements = driver.FindElements(By.CssSelector(".pul-progress-step__body"));
                while (_closerElements.Count() > 0)
                {
                    _closerElements[0].Click();
                    Thread.Sleep(500);
                    _closerElements = driver.FindElements(By.CssSelector(".pul-progress-step__body"));
                }
            }
            catch (Exception e)
            {
                //if (!(e.GetType() == typeof(NoSuchElementException) || e.GetType() == typeof(WebDriverException)))
                //{

                //}
                //bool wait = new WebDriverWait(driver, TimeSpan.FromSeconds(60)).Until(d => ((IJavaScriptExecutor)d).ExecuteScript("return document.readyState").Equals("complete"));

                //if (wait == true)
                //{
                //    return;
                //}
                CloseAllNotifications();
            }
        }
    }
}