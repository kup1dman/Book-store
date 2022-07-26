﻿using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace BookStore.Models.Data
{
    public class Db : DbContext
    {
        public DbSet<PagesDTO> Pages { get; set; }
        
        public DbSet<SidebarsDTO> SideBars { get; set; }

        public DbSet<CategoryDTO> Categories { get; set; }

        public DbSet<ProductDTO> Products { get; set; }

        public DbSet<UserDTO> Users { get; set; }

        public DbSet<RoleDTO> Roles { get; set; }

        public DbSet<UserRoleDTO> UserRoles { get; set; }

    }
}