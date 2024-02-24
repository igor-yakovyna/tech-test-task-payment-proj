using System.Collections.Generic;
using Payment.Core.Contract.Model;
using Payment.Core.Contract.Repository;

namespace Payment.Wallets;

public class WalletMemoryRepository : IWalletRepository
{
    private static Dictionary<string, IWallet> _wallets = new();

    public IWallet FindWallet(string id)
    {
        return _wallets.TryGetValue(id, out var wallet) ? wallet : null;
    }

    public void Save(IWallet wallet)
    {
        _wallets.TryAdd(wallet.Id, wallet);
    }
}