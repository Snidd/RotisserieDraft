using RotisserieDraft.Models;
using RotisserieDraft.Util;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace RotisserieDraft.Tests.Util
{
	[TestClass]
	public class TestCardUtilExtender
	{
		[TestMethod]
		public void CanCalculateConvertedManaCost()
		{
			var card = new Card {CastingCost = "1RG"};
			int convertedManaCost = card.GetConvertedManaCost();
			Assert.AreEqual(3, convertedManaCost);
		}

		[TestMethod]
		public void CanCalculateBigConvertedManaCost()
		{
			var card = new Card { CastingCost = "12GG" };
			int convertedManaCost = card.GetConvertedManaCost();
			Assert.AreEqual(14, convertedManaCost);
		}

		[TestMethod]
		public void CanCalculateColoredConvertedManaCost()
		{
			var card = new Card { CastingCost = "WRUG" };
			int convertedManaCost = card.GetConvertedManaCost();
			Assert.AreEqual(4, convertedManaCost);
		}

        [TestMethod]
        public void CanCalculateSplitConvertedManaCost()
        {
            var card = new Card { CastingCost = "1R/1U" };
            int convertedManaCost = card.GetConvertedManaCost();
            Assert.AreEqual(4, convertedManaCost);
        }

		[TestMethod]
		public void CanCalculateSimpleConvertedManaCost()
		{
			var card = new Card { CastingCost = "4U" };
			int convertedManaCost = card.GetConvertedManaCost();
			Assert.AreEqual(5, convertedManaCost);
		}

		[TestMethod]
		public void CanGetCardColors()
		{
			var card = new Card { CastingCost = "1RG" };

			Assert.IsTrue(card.IsRed());
			Assert.IsTrue(card.IsGreen());
		}
	}
}
