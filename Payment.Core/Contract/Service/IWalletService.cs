using Payment.Core.Contract.Model;

namespace Payment.Core.Contract.Service;

public interface IWalletService
{
    /// <summary>
    /// Create a wallet
    /// </summary>
    /// <param name="owner"></param>
    /// <param name="currency"></param>
    /// <returns></returns>
    IWallet CreateWallet(IUser owner, Currency currency);

    /// <summary>
    /// Get a wallet by its ID
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    IWallet GetWallet(string id);

    /// <summary>
    /// Add Amount to a wallet
    /// - Adds amount into the wallet amount
    /// - If there's no share from this author in the wallet,  Creates a new Share of this amount from the author
    /// - If there's already a share from the author , adds the amount to the existing share
    /// </summary>
    /// <param name="wallet"></param>
    /// <param name="author"></param>
    /// <param name="amount"></param>
    void Contribute(IWallet wallet, IUser author, Money amount);
    
    /// <summary>
    /// Transfer an amount from a wallet to another.
    /// For each share in the sourceWallet we transfer the pro-rate amount to the destWallet
    /// Example: If sourceWallet has three shares:
    /// Share1: 50 Euro
    /// Share2: 25 Euro
    /// Share3: 25 Euro
    /// and we want to transfer 20 Euro to destWallet(Amount=0 and having no shares), then we will have
    /// for sourceWallet three shares of : Share1 = 40 Euro, Share2 = 20 Euro and Share3 = 20 Euro
    /// for destWallet we will have three shares of: Share1 = 10 Euro, Share2 = 5 Euro and Share3 = 5 Euro
    /// </summary>
    /// <param name="sourceWallet">source Wallet</param>
    /// <param name="destWallet">destination Wallet</param>
    /// <param name="amount">Amount to transfer</param>
    void Transfer(IWallet sourceWallet, IWallet destWallet, Money amount);
}