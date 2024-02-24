using Payment.Core;
using Payment.Core.Contract.Model;
using Payment.Core.Contract.Repository;
using Payment.Core.Contract.Service;
using System;
using System.Linq;
using Payment.Core.Extension;

namespace Payment.Wallets;

public class WalletService : IWalletService
{
    private readonly IWalletRepository _walletRepository;

    public WalletService(IWalletRepository walletRepository)
    {
        _walletRepository = walletRepository ?? throw new ArgumentNullException(nameof(walletRepository));
    }

    public IWallet CreateWallet(IUser owner, Currency currency)
    {
        owner.ThrowIfNull(nameof(owner));

        var wallet = new Wallet(owner, currency);
        _walletRepository.Save(wallet);

        return wallet;
    }

    public IWallet GetWallet(string id)
    {
        id.ThrowIfNullOrEmpty(nameof(id));

        return _walletRepository.FindWallet(id);
    }

    public void Contribute(IWallet wallet, IUser author, Money amount)
    {
        wallet.ThrowIfNull(nameof(wallet));
        author.ThrowIfNull(nameof(author));

        var share = wallet.Shares.FirstOrDefault(s => s.Owner.Id == author.Id);

        if (share == null)
        {
            share = new Share(author, wallet.Id, amount);
            wallet.Shares.Add(share);
        }
        else
        {
            share.AddAmount(amount);
        }

        wallet.AddAmount(amount);
    }

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
    public void Transfer(IWallet sourceWallet, IWallet destWallet, Money amount)
    {
        sourceWallet.Amount.Value.ThrowIfNotGreaterThanZero();
        amount.Value.ThrowIfNotGreaterThanZero();
        
        if (sourceWallet.Amount.Value < amount.Value)
        {
            throw new InvalidOperationException("Insufficient funds in the source wallet.");
        }

        // Calculate the total amount in the source wallet
        var totalAmountInSource = sourceWallet.Shares.Sum(share => share.Amount.Value);
        
        // Transfer funds from each share
        foreach (var sourceShare in sourceWallet.Shares)
        {
            // Calculate the amount to transfer from this share
            var transferAmountFromShare =
                (long)Math.Round((double)sourceShare.Amount.Value / totalAmountInSource * amount.Value);
            
            // Deduct this amount from the source share
            sourceShare.AddAmount(new Money(-transferAmountFromShare, amount.Currency));
            
            var destShare = destWallet.Shares.FirstOrDefault(s => s.Id == sourceShare.Id);
            if (destShare == null)
            {
                destShare = new Share(destWallet.Owner, destWallet.Id, new Money(0, amount.Currency));
                destWallet.Shares.Add(destShare);
            }
            
            // Add the amount to the destination share
            destShare.AddAmount(new Money(transferAmountFromShare, amount.Currency));
        }
    }
}