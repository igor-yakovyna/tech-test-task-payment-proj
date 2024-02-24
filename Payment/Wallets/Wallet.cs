using Payment.Core;
using Payment.Core.Contract.Model;
using System;
using System.Collections.Generic;

namespace Payment.Wallets;

public class Wallet : IWallet
{
    public Wallet(IUser owner, Currency currency)
    {
        Id = Guid.NewGuid().ToString("N");
        Owner = owner;
        Amount = new Money(0, currency);
        Shares = new List<IShare>();
    }

    public string Id { get; }
    
    public Money Amount { get; private set; }
    
    public IUser Owner { get; }
    
    public IList<IShare> Shares { get; }

    public void AddAmount(Money amount)
    {
        if (amount.Value <= 0)
        {
            throw new ArgumentException(
                "Contributions to the wallet must be greater than zero; negative values or zero are not allowed.");
        }

        if (Amount.Currency != amount.Currency)
        {
            throw new ArgumentException("Contributions in a different currency than the wallet has are not allowed.");
        }

        Amount += amount;
    }
}