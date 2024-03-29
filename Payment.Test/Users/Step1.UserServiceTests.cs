﻿using NUnit.Framework;
using Payment.Core.Contract.Service;
using Payment.Users;
using System.Linq;
using Payment.Core.Contract.Repository;

namespace Payment.Test.Users;

[Category("Step 1: User Service")]
[TestFixture]
public class Step1UserServiceTests
{
    [Test]
    public void Test_CreateUser()
    {
        var sut = GetUserService();
        var createdUser = sut.CreateUser("John Doe");
        Assert.NotNull(createdUser);
        Assert.AreEqual("John Doe", createdUser.Name);
    }

    [Test]
    public void Test_GetUser()
    {
        var sut = GetUserService();
        var createdUser = sut.CreateUser("John Doe");

        var user = sut.GetUser(createdUser.Id);
        Assert.AreEqual(createdUser, user);
    }

    [Test]
    public void Test_AddFriendship()
    {
        var sut = GetUserService();
        var user1 = sut.CreateUser("John Doe");
        var user2 = sut.CreateUser("Sara Mc Cain");

        sut.AddFriendship(user1, user2);

        Assert.Contains(user2, user1.Friends.ToList());
    }

    [Test]
    public void Test_Friendship_Relation_Is_Mutual()
    {
        var sut = GetUserService();
        var user1 = sut.CreateUser("John Doe");
        var user2 = sut.CreateUser("Sara Mc Cain");

        sut.AddFriendship(user1, user2);

        Assert.Contains(user2, user1.Friends.ToList());
        Assert.Contains(user1, user2.Friends.ToList());
    }

    [Test]
    public void Test_Delete_User()
    {
        var sut = GetUserService();
        var user1 = sut.CreateUser("John Doe");
        sut.DeleteUser(user1);

        Assert.IsNull(sut.GetUser(user1.Id));
    }

    [Test]
    public void Test_Delete_User_Also_Delete_From_Friends()
    {
        IUserService sut = GetUserService();
        var user1 = sut.CreateUser("John Doe");
        var user2 = sut.CreateUser("Sara Mc Cain");

        sut.AddFriendship(user1, user2);
        sut.DeleteUser(user2);

        user1 = sut.GetUser(user1.Id);

        CollectionAssert.DoesNotContain(user1.Friends.ToList(), user2);
    }

    [Test]
    public void Test_Get_CommonFriends()
    {
        var sut = GetUserService();
        var user1 = sut.CreateUser("John Doe");
        var user2 = sut.CreateUser("Sara Mc Cain");
        var friendsList = Enumerable.Range(1, 100)
            .Select(i => sut.CreateUser($"friend{i}"))
            .ToList();

        friendsList.Take(75).ToList().ForEach(x => sut.AddFriendship(user1, x));
        friendsList.Skip(25).Take(75).ToList().ForEach(x => sut.AddFriendship(user2, x));

        CollectionAssert.AreEquivalent(friendsList.Skip(25).Take(50), sut.GetCommonFriends(user1, user2));
    }

    private static IUserService GetUserService()
    {
        IUserRepository userRepository = new UserMemoryRepository();
        IUserService userService = new UserService(userRepository);

        return userService;
    }
}