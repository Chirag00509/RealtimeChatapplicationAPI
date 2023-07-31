﻿using WebApplication1.Modal;

namespace WebApplication1.Interfaces
{
    public interface IUserRepository
    {
        IEnumerable<UserProfile> GetUsersExcludingId(string id);

        Task<List<string>> GetUserNameId(string id);


        //bool DoesEmailExist(string email);
        //User getEmailAndPassword(string email, string password);
        //void AddUser(User user);
        //Task<User> GetUserByEmail(string email);
    }
}
