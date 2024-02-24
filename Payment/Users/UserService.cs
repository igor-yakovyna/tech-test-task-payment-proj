using Payment.Core.Contract.Model;
using Payment.Core.Contract.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using Payment.Core.Contract.Repository;
using Payment.Core.Exception;
using Payment.Core.Extension;

namespace Payment.Users;

public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;

    public UserService(IUserRepository userRepository)
    {
        _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
    }

    public IUser GetUser(string id)
    {
        id.ThrowIfNullOrEmpty(nameof(id));

        return _userRepository.FindUser(id);
    }

    public IUser CreateUser(string name)
    {
        var user = new User(name);
        _userRepository.Add(user);

        return user;
    }

    public void DeleteUser(IUser user)
    {
        user.ThrowIfNull(nameof(user));

        _userRepository.DeleteUserFriendship(user.Id);

        _userRepository.Delete(user.Id);
    }

    public void AddFriendship(IUser user1, IUser user2)
    {
        user1.ThrowIfNull(nameof(user1));
        user2.ThrowIfNull(nameof(user2));

        ValidateUsersToAddFriendshipExist(user1, user2);

        _userRepository.AddFriendship(user1, user2);
    }

    public IReadOnlyList<IUser> GetCommonFriends(IUser user1, IUser user2)
    {
        user1.ThrowIfNull(nameof(user1));
        user2.ThrowIfNull(nameof(user2));

        return _userRepository.GetCommonFriends(user1.Id, user2.Id);
    }

    public IReadOnlyList<IUser> GetConnectionList(IUser user1, IUser user2, uint maxLevel)
    {
        user1.ThrowIfNull(nameof(user1));
        user2.ThrowIfNull(nameof(user2));
        maxLevel.ThrowIfNotGreaterThanZero(nameof(maxLevel));

        var visitedUsers = new HashSet<IUser>();
        var queue = new Queue<List<IUser>>();

        queue.Enqueue(new List<IUser> { user1 });

        while (queue.Count > 0)
        {
            var path = queue.Dequeue();
            var currentUser = path.Last();

            // Return the current path if the end user is found
            if (currentUser == user2)
            {
                return path;
            }

            // Skip further exploration if max level is reached
            if (path.Count > maxLevel)
            {
                continue;
            }

            visitedUsers.Add(currentUser);

            foreach (var friend in currentUser.Friends)
            {
                // Continue to explore the friends that haven't been visited
                if (!visitedUsers.Contains(friend))
                {
                    var newPath = new List<IUser>(path) { friend };
                    queue.Enqueue(newPath);
                }
            }
        }

        // Return an empty list if no connection is found within the max level
        return new List<IUser>();
    }

    private void ValidateUsersToAddFriendshipExist(IUser user1, IUser user2)
    {
        var existingUsers = _userRepository.FindUsers(new[] { user1.Id, user2.Id }).ToList();

        if (!existingUsers.Contains(user1))
        {
            throw new NotFoundException($"User {user1.Name} not found to add friendship with.");
        }

        if (!existingUsers.Contains(user2))
        {
            throw new NotFoundException($"User {user2.Name} not found to add friendship with.");
        }
    }
}