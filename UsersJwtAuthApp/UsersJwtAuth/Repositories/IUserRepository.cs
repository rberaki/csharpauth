﻿using UsersJwtAuth.Models;

namespace UsersJwtAuth.Repositories;

public interface IUserRepository
{
    Task<IEnumerable<User>> GetAll();
    Task<User> GetById(int id);
    Task Add(User user);
    Task Update(User user);
    Task Delete(int id);
}
