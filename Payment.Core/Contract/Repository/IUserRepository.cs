using System.Collections.Generic;
using Payment.Core.Contract.Model;

namespace Payment.Core.Contract.Repository;

public interface IUserRepository
{
    public IUser FindUser(string id);

    IEnumerable<IUser> FindUsers(string[] ids);

    public void Add(IUser user);
    
    public void Delete(string id);

    public void AddFriendship(IUser friendUser1, IUser friendUser2);

    public IReadOnlyList<IUser> GetCommonFriends(string idUser1, string idUser2);

    void DeleteUserFriendship(string userId);
}