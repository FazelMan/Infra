using System;
using System.Collections.Generic;
using System.Linq;

namespace Infra.Shared.Extensions
{
    public static class MathExtensions
	{
		public static decimal CalculatePercentageFromAmount(
			this decimal amount,
			decimal source)
		{
		    return ((amount / source) * 100).RoundShoppingAmount(0);
		}

		public static decimal RoundShoppingAmount(
			this decimal shoppingAmount,
			int digits)
		{
		    return Math.Round(shoppingAmount, digits, MidpointRounding.ToEven);
		}

		public static List<decimal> Split(
			this decimal amount,
			int count)
		{
		    var splitAmounts = new List<decimal>();

		    if (count < 1 || amount <= 0)
		        return splitAmounts;

            var amountShare = (amount / count).RoundShoppingAmount(2);

		    for (var i = 0; i < count; i++)
		        splitAmounts.Add(amountShare);

		    var remainder = amount - splitAmounts.Sum();

		    if (remainder > 0)
		        splitAmounts[0] += remainder;

            return splitAmounts;
		}
	}
}
