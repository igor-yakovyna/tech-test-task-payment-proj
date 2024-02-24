using Payment.Core.Contract.Model;

namespace Payment.Core.Contract.Repository;

public interface IWalletRepository
{
    public IWallet FindWallet(string id);

    public void Save(IWallet wallet);
}