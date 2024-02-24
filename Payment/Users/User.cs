using Payment.Core.Contract.Model;
using System;
using System.Collections.Generic;
using Payment.Core.Extension;

namespace Payment.Users;

public class User : IUser
{
    public User(string name)
    {
        name.ThrowIfNullOrEmpty(nameof(name));

        Id = Guid.NewGuid().ToString("N");
        Name = name;
        Friends = new List<IUser>();
    }

    public string Id { get; }
    
    public string Name { get; }
    
    public IList<IUser> Friends { get; }

    public void AddFriend(IUser friend)
    {
        if (!Friends.Contains(friend))
        {
            Friends.Add(friend);
        }
    }

    public void RemoveFriend(IUser friend)
    {
        if (Friends.Contains(friend))
        {
            Friends.Remove(friend);
        }
    }
}