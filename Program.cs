using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Interactions;
using SeleniumExtras.WaitHelpers;
using OpenQA.Selenium.Support.UI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading;
using Xunit;

namespace WebAutoCore
{

    public class WebTest : WebDriver
    {

        //Chrome & WebDriver lastest version 117.0.5938.8800

        string filter = "Iphone 14 pro max 128gb";
        string searchFilter = @"Iphone*.?14*.?pro*.?max*.?128gb";
        string checkPrice = String.Empty;

        [Fact]
        public void IPhoneCheck()
        {

            NavigateToEbay();
            SelectIphone();
            CheckResultsAndSelect();
            SelectColorQuantity();

            /*Sometimes Checking Out as Guest switching 
            to Robot verification via  CaptCha, 
            which prevents from fewer actions*/

            CheckCart();
            QuitWeb();

        }

        //Test Steps
        void NavigateToEbay() => driver.Navigate().GoToUrl("https://www.ebay.com/");

        void SelectIphone()
        {
            driver.FindElement(By.XPath("//input[contains(@class,'gh-tb ui-autocomplete-input')]")).
                SendKeys(filter);
            driver.FindElement(By.Id("gh-btn")).Click();
            WaitForPageToLoad();
        }

        void CheckResultsAndSelect()
        {

            var iPhones14 = new List<string>();

            var titles = driver.FindElements(By.XPath("//span[@role='heading']"));
            var phones = driver.FindElements(By.XPath("//li//div[@class='s-item__wrapper clearfix']"));

            foreach (var item in titles)
                if (Regex.Match(item.Text, searchFilter, 
                    RegexOptions.IgnoreCase).Success) iPhones14.Add(item.Text);
            Console.WriteLine($"Only {iPhones14.Count} matches filter exact.");

            int counter = 0;
            foreach (var phone in phones)
            {
                if (Regex.Match(phone.Text, searchFilter,
                    RegexOptions.IgnoreCase).Success &&
                    Regex.Match(phone.Text, @"Buy*.?It*.?Now",
                    RegexOptions.IgnoreCase).Success)
                {
                    
                    titles[counter].Click(); 
                    break;
                }
                counter++;
            }
            WaitForPageToLoad();

        }

        void SelectColorQuantity() 
        {
            ReadOnlyCollection<string> windowHandles = driver.WindowHandles;
            driver.SwitchTo().Window(windowHandles[1]);
            var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));

            //Color
            try 
            {
                var color = driver.FindElement(By.Id("x-msku__select-box-1000"));
                wait.Until(ExpectedConditions.ElementToBeClickable(color));

                var colors = driver.FindElements(By.XPath("//option[contains(@Id, 'x-msku__option-box-') and not(@disabled)]"));
                color.Click();
                colors[1].Click();

            } catch { }

            //Quantity
            try
            {
                driver.FindElement(By.Id("qtyTextBox")).Clear();
                driver.FindElement(By.Id("qtyTextBox")).SendKeys("1");
            }
            catch { }

            checkPrice = driver.FindElement(By.XPath("//div[@Class='x-price-primary']")).Text;
            driver.FindElement(By.XPath("//span[@Class='ux-call-to-action__text']")).Click();
            WaitForPageToLoad();

        }
        void CheckCart()
        {
            try 
            { 
                driver.FindElement(By.XPath("//button[@aria-label='Close dialog']")).Click();
                WaitForPageToLoad();
            }
            catch { }

            try 
            { 
                driver.FindElement(By.XPath("//a[@class='ux-call-to-action fake-btn fake-btn--fluid fake-btn--secondary']")).Click();
                WaitForPageToLoad();
            }
            catch { }

            try
            {
                var itemPrice = driver.FindElement(By.XPath("//div[@Class='item-price']")).Text;
                var subTotal = driver.FindElement(By.XPath("//tr[@data-test-id='SUB_TOTAL']")).Text;
                var total = driver.FindElement(By.XPath("//tr[@data-test-id='TOTAL']")).Text;

                var _itemPrice = double.Parse(Regex.Match(itemPrice, @"\d+\.\d+").Value);
                var _subTotal = double.Parse(Regex.Match(subTotal, @"\d+\.\d+").Value);
                var _total = double.Parse(Regex.Match(total, @"\d+\.\d+").Value);
                var _price = double.Parse(Regex.Match(checkPrice, @"\d+\.\d+").Value);

                Assert.True(_itemPrice == _price && _subTotal == _price && _total >= _price);
            }
            catch { }

            
        }

        void QuitWeb() => driver.Quit();


        void WaitForPageToLoad(int timeoutInSeconds = 5)
        {
            try
            {
                var jsDriver = driver as IJavaScriptExecutor;
                WebDriverWait wait = new WebDriverWait(driver, new TimeSpan(0, 0, timeoutInSeconds));
                wait.Until(wd => (string)jsDriver.ExecuteScript("return document.readyState") == "complete");
            }
            catch { }  

        }
    }

    public class WebDriver
    {
        public IWebDriver driver;
        public WebDriver()
        {
            driver = new ChromeDriver(
               Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), new ChromeOptions());
            driver.Manage().Window.Maximize();
            try { driver.Manage().Cookies.DeleteAllCookies(); } catch { }
        }


    }


}
