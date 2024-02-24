using System.Collections.Generic;

namespace Payment.Core.Contract.Model;

public interface IUser
{
    string Id { get; }

    string Name { get; }

    IList<IUser> Friends { get; }

    void AddFriend(IUser friend);

    void RemoveFriend(IUser friend);
}