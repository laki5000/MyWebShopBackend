﻿using Shared.BaseClasses.Repositories;
using UserService.App.Data;
using UserService.App.Interfaces.Repositories;
using UserService.App.Models;

namespace UserService.App.Repositories
{
    public class UserRepositoryImpl : BasePgRepositoryImpl<UserEntity>, IUserRepository
    {
        protected new readonly UserDbContext _context;

        public UserRepositoryImpl(UserDbContext context) : base(context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }
    }
}
