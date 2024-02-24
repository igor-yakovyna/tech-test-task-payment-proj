using System.Collections.Generic;

namespace Payment.Core.Contract.Model
{
    public interface IWallet
    {
        string Id { get; }
        
        Money Amount { get; }
        
        IUser Owner { get; }
        
        IList<IShare> Shares { get; }
        
        void AddAmount(Money amount);
    }
}