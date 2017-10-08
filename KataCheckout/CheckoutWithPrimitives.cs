using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;


namespace KataCheckout
{
    public class CheckoutWithPrimitives
    {
        private int _runningTotal;
        private readonly Dictionary<char, int> _prices = new Dictionary<char, int>()
        {
            {'A', 50},
            {'B', 30},
            {'C', 20},
            {'D', 15},
        };
        private readonly List<char> _basket = new List<char>();
        private readonly List<Tuple<char, int, int>> _discounts;

        public CheckoutWithPrimitives(List<Tuple<char, int, int>> discounts)
        {
            _discounts = discounts;
        }

        public void Scan(char sku)
        {
            _basket.Add(sku);
        }

        public int GetTotal()
        {
            _runningTotal = _basket.Aggregate(0, (previousMoney, currentProduct) => previousMoney + _prices.TryGetValueOrDefault(currentProduct));

            _discounts.ForEach(discount => ApplyDiscount(discount.Item1, discount.Item2, discount.Item3));

            return _runningTotal;
        }

        private void ApplyDiscount(char product, int discountableQuantity, int discountAmount)
        {
            var productsCount = _basket.Count(x => x == product);
            for (var instances = productsCount / discountableQuantity; instances > 0; instances--)
            {
                _runningTotal += discountAmount;
            }
        }
    }

    public static class DictionaryExtensions
    {
        public static TValue TryGetValueOrDefault<TKey, TValue>(
            this Dictionary<TKey, TValue> input,
            TKey key,
            TValue ifNotFound = default(TValue))
        {
            TValue val;
            return input.TryGetValue(key, out val) ? val : ifNotFound;
        }
    }

    [TestClass]
    public class CheckoutSpecTests
    {
        private CheckoutWithPrimitives _checkout;
        [TestInitialize]
        public void TestSetup()
        {
            var discounts = new List<Tuple<char, int, int>>
            {
                new Tuple<char, int, int>('A', 3, -20),
                new Tuple<char, int, int>('B', 2, -15),
            };
            _checkout = new CheckoutWithPrimitives(discounts);
        }

        [TestMethod]
        public void When_Scanning_1_A_SubTotal_Is_50()
        {
            _checkout.Scan('A');
            Assert.AreEqual(50, _checkout.GetTotal());
        }

        [TestMethod]
        public void When_Scanning_2_A_SubTotal_Is_100()
        {
            _checkout.Scan('A');
            _checkout.Scan('A');
            Assert.AreEqual(100, _checkout.GetTotal());
        }

        [TestMethod]
        public void When_Scanning_3_A_SubTotal_Is_130()
        {
            _checkout.Scan('A');
            _checkout.Scan('A');
            _checkout.Scan('A');
            Assert.AreEqual(130, _checkout.GetTotal());
        }
        [TestMethod]
        public void When_Scanning_6_A_SubTotal_Is_260()
        {
            _checkout.Scan('A');
            _checkout.Scan('A');
            _checkout.Scan('A');
            _checkout.Scan('A');
            _checkout.Scan('A');
            _checkout.Scan('A');
            Assert.AreEqual(260, _checkout.GetTotal());
        }
        [TestMethod]
        public void When_Scanning_1_B_SubTotal_Is_30()
        {
            _checkout.Scan('B');
            Assert.AreEqual(30, _checkout.GetTotal());
        }
        [TestMethod]
        public void When_Scanning_2_B_SubTotal_Is_45()
        {
            _checkout.Scan('B');
            _checkout.Scan('B');
            Assert.AreEqual(45, _checkout.GetTotal());
        }

        [TestMethod]
        public void When_Scanning_1_C_SubTotal_Is_20()
        {
            _checkout.Scan('C');
            Assert.AreEqual(20, _checkout.GetTotal());
        }
        [TestMethod]
        public void When_Scanning_1_D_SubTotal_Is_15()
        {
            _checkout.Scan('D');
            Assert.AreEqual(15, _checkout.GetTotal());
        }
    }
}