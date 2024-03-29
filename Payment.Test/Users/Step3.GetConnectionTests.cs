﻿using NUnit.Framework;
using Payment.Core.Contract.Model;
using Payment.Core.Contract.Service;
using Payment.Users;
using System.Collections.Generic;
using System.Linq;
using Payment.Core.Contract.Repository;

namespace Payment.Test.Users;

[Category("Step 3: User Service")]
[TestFixture]
public class Step2GetConnectionTests
{
    [Test]
    public void Test_GetConnectionList_With_Not_Connected_Users()
    {
        var sut = GetUserService();
        var user1 = sut.CreateUser("John Doe");
        var user2 = sut.CreateUser("Sara Mc Cain");
        AddFriends(user1, CreateUsers(10, sut), sut);
        user1.Friends.ToList().ForEach(f => AddFriends(f, CreateUsers(10, sut), sut));
        AddFriends(user2, CreateUsers(10, sut), sut);
        user2.Friends.ToList().ForEach(f => AddFriends(f, CreateUsers(10, sut), sut));

        CollectionAssert.IsEmpty(sut.GetConnectionList(user1, user2, 100));
    }

    [Test]
    public void Test_GetConnectionList_With_Friend_Users()
    {
        var sut = GetUserService();
        var user1 = sut.CreateUser("John Doe");
        var user2 = sut.CreateUser("Sara Mc Cain");
        sut.AddFriendship(user1, user2);

        CollectionAssert.AreEqual(new List<IUser> { user1, user2 }, sut.GetConnectionList(user1, user2, 100));
    }

    [Test]
    public void Test_GetConnectionList_With_Users_Having_Connection()
    {
        var sut = GetUserService();
        var user1 = sut.CreateUser("John Doe");
        var user2 = sut.CreateUser("Sara Mc Cain");
        var users = CreateUsers(4, sut);

        sut.AddFriendship(user1, users.First());
        sut.AddFriendship(users.First(), users[1]);
        sut.AddFriendship(users[1], users[2]);
        sut.AddFriendship(users[2], users[3]);
        sut.AddFriendship(users[3], user2);

        var expectedConnection = new List<IUser> { user1 };
        expectedConnection.AddRange(users);
        expectedConnection.Add(user2);

        CollectionAssert.AreEqual(expectedConnection, sut.GetConnectionList(user1, user2, 100));
    }

    [Test]
    public void Test_GetConnectionList_Should_Return_Shortest_Connection()
    {
        var sut = GetUserService();
        var user1 = sut.CreateUser("John Doe");
        var user2 = sut.CreateUser("Sara Mc Cain");
        var users = CreateUsers(4, sut);

        sut.AddFriendship(user1, users.First());
        sut.AddFriendship(users.First(), users[1]);
        sut.AddFriendship(users[1], users[2]);
        sut.AddFriendship(users[2], users[3]);
        sut.AddFriendship(users[3], user2);

        var bestFriend = sut.CreateUser("Best Friend");
        var coolFriend = sut.CreateUser("Cool Friend");
        sut.AddFriendship(user1, bestFriend);
        sut.AddFriendship(bestFriend, coolFriend);
        sut.AddFriendship(coolFriend, user2);

        var expectedConnection = new List<IUser> { user1, bestFriend, coolFriend, user2 };

        CollectionAssert.AreEqual(expectedConnection, sut.GetConnectionList(user1, user2, 100));
    }

    [Test]
    public void Test_GetConnectionList_Should_Return_Empty_List_If_MaxLevel_IsReached()
    {
        var sut = GetUserService();
        var user1 = sut.CreateUser("John Doe");
        var user2 = sut.CreateUser("Sara Mc Cain");
        var users = CreateUsers(4, sut);

        sut.AddFriendship(user1, users.First());
        sut.AddFriendship(users.First(), users[1]);
        sut.AddFriendship(users[1], users[2]);
        sut.AddFriendship(users[2], users[3]);
        sut.AddFriendship(users[3], user2);

        CollectionAssert.IsEmpty(sut.GetConnectionList(user1, user2, 3));
    }

    private List<IUser> CreateUsers(int nbUsers, IUserService userService)
    {
        return Enumerable.Range(1, nbUsers)
            .Select(i => userService.CreateUser($"user{i}"))
            .ToList();
    }

    private void AddFriends(IUser user, IList<IUser> friends, IUserService userService)
    {
        foreach (var friend in friends)
        {
            userService.AddFriendship(user, friend);
        }
    }

    private static IUserService GetUserService()
    {
        IUserRepository userRepository = new UserMemoryRepository();
        IUserService userService = new UserService(userRepository);

        return userService;
    }
}