namespace Account.Contexts;

using Account.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

public class AccountDbContext(DbContextOptions<AccountDbContext> options) : IdentityDbContext<AppUserEntity>(options)
{
}
