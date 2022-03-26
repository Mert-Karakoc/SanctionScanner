using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Linq;
using System.Collections.Generic;

namespace DataFromWebPage
{
    internal class Program
    {
        static void Main(string[] args)
        {
            IWebDriver driver = new ChromeDriver(); // Created Chrome driver for main page
            driver.Navigate().GoToUrl("https://www.sahibinden.com/"); // Url opened

            // Advert links found with javascript
            string jsCommand = "const list = document.querySelectorAll('#container > div.homepage > div > div.homepage-content > div.uiBox.showcase > ul > li > a');" +
                "return list;";
            IJavaScriptExecutor executor = (IJavaScriptExecutor)driver;
            IReadOnlyCollection<IWebElement> elements = (IReadOnlyCollection<IWebElement>)executor.ExecuteScript(jsCommand);


            IWebDriver detailDriver = new ChromeDriver(); // Created Chrome driver for detail page
            
            // Found div tag that contains advert detail
            string jsCommandDetail = "const detailDiv = document.querySelectorAll('#classifiedDetail > div > div.classifiedDetailContent > div.classifiedInfo');" +
                    "return detailDiv;";
            IJavaScriptExecutor executorDetail = (IJavaScriptExecutor)detailDriver;

            List<string> prices = new List<string>(); // Created a list for prices

            foreach (IWebElement element in elements)
            {
                detailDriver.Navigate().GoToUrl(element.GetAttribute("href")); // Opened detail page

                if (element.Text != "") // Control for advert is advertisement or not
                {
                    IReadOnlyCollection<IWebElement> detailElements = (IReadOnlyCollection<IWebElement>)executorDetail.ExecuteScript(jsCommandDetail); // Details found

                    string rawPrice = detailElements.First().FindElement(By.TagName("h3")).Text; // Price found
                    string price = string.Empty;

                    // Price arranged
                    if (rawPrice.Contains("$") || rawPrice.Contains("€"))
                    {
                        price = detailElements.First().FindElement(By.TagName("h3")).Text.Split(" ")[1].Replace(".", "");
                    }
                    else
                    {
                        price = detailElements.First().FindElement(By.TagName("h3")).Text.Split(" ")[0].Replace(".", "");
                    }

                    prices.Add(price);
                    WriteToTxt(element.Text, price); // Title and price written to txt file
                    //Console.WriteLine(detailElements.First().Text);  // Can acces every detail from here
                    Console.WriteLine(element.Text + " : " + price);
                }
            }
            Console.WriteLine("AVERAGE PRICE : " + AveragePrice(prices));
        }

        // Created a method for writing txt file
        static void WriteToTxt(string advertName, string advertPrice)
        {
            using (StreamWriter writer = new StreamWriter("data.txt", true))
            {
                writer.WriteLine(advertName + " : " + advertPrice);
                writer.Close();
            }
        }
        // Created a method for average price
        static double AveragePrice(List<string> prices)
        {
            int sumPrice = 0;
            foreach (string price in prices)
            {
                sumPrice += Convert.ToInt32(price);
            }
            return (sumPrice / prices.Count);
        }
    }
}
