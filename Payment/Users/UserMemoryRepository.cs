using Payment.Core.Contract.Model;
using System.Collections.Generic;
using System.Linq;
using Payment.Core.Contract.Repository;

namespace Payment.Users;

public class UserMemoryRepository : IUserRepository
{
    private static Dictionary<string, IUser> _users = new();

    public IUser FindUser(string id)
    {
        return _users.TryGetValue(id, out var user) ? user : null;
    }

    public IEnumerable<IUser> FindUsers(string[] ids)
    {
        return ids.Where(_users.ContainsKey)
            .Select(x => _users[x])
            .ToList();
    }

    public void Add(IUser user)
    {
        _users.TryAdd(user.Id, user);
    }

    public void Delete(string id)
    {
        _users.Remove(id);
    }

    public void AddFriendship(IUser friendUser1, IUser friendUser2)
    {
        friendUser1.AddFriend(friendUser2);
        friendUser2.AddFriend(friendUser1);

        UpdateRange(new[] { friendUser1, friendUser2 });
    }

    public IReadOnlyList<IUser> GetCommonFriends(string idUser1, string idUser2)
    {
        var user1Friends = FindUser(idUser1).Friends;
        var user2Friends = FindUser(idUser2).Friends;

        return user1Friends.Intersect(user2Friends).ToList();
    }

    public void DeleteUserFriendship(string userId)
    {
        var userFriends = _users.Values.Where(p => p.Friends.Any(s => s.Id == userId)).ToList();
        if (userFriends.Any())
        {
            userFriends.ForEach(a =>
            {
                var friendToRemove = a.Friends.FirstOrDefault(p => p.Id == userId);
                a.RemoveFriend(friendToRemove);
            });

            UpdateRange(userFriends);
        }
    }

    private static void UpdateRange(IEnumerable<IUser> users)
    {
        var usersKeys = users.Select(s => s.Id);
        foreach (var userKey in usersKeys)
        {
            _users.Remove(userKey);
        }

        foreach (var user in users)
        {
            _users.TryAdd(user.Id, user);
        }
    }
}