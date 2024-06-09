﻿using Microsoft.EntityFrameworkCore;
using UsersJwtAuth.Models;

namespace UsersJwtAuth.Repositories;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<User> Users { get; set; }
}
