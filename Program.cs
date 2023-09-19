using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System;
using System.IO;
using System.Reflection;
using Xunit;

namespace WebAutoCore
{

    public class WebTest : WebDriver
    {

        [Fact]
        public void IPhoneCheck()
        {

            NavigateToEbay();
            SelectIphone();
            //chrome.Navigate().GoToUrl("https://www.ebay.com/");
            //chrome.SwitchTo().Frame("callout");
            //chrome.FindElement(By.ClassName("QlyBfb")).Click();

            QuitChrome();

        }

        //Test Steps
        void NavigateToEbay() => chrome.Navigate().GoToUrl("https://www.ebay.com/");
        void SelectIphone()
        {

        }
        void CheckResults() { }
        void OpenAdvertisement() { }
        void SelectColorQuantity() { }
        void ClickAdd() { }
        void CheckPrice() { }

        void QuitChrome() => chrome.Quit();
    }

    public class TestSteps
    {
        
    }

    public class WebDriver
    {
        public IWebDriver chrome;
        public WebDriver()
        {
            chrome = new ChromeDriver(
               Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), new ChromeOptions());
            chrome.Manage().Window.Maximize();
        }


    }
}
