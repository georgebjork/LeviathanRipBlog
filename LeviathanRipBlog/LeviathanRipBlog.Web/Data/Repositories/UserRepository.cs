using Dapper;
using LeviathanRipBlog.Web.Data;
using LeviathanRipBlog.Web.Data.Models;
using LeviathanRipBlog.Web.Data.Repositories;

namespace LeviathanRipBlog.Data.Repositories;


public interface IUserRepository : IBaseRepository
{
    Task<List<ApplicationUser>> GetUsers();
    
    
    // User Invites
    Task<List<user_invitation>> GetInvites();
    Task<user_invitation?> GetInvite(string inviteId);
}

public class UserRepository : BasePgRepository, IUserRepository
{
    private readonly ILogger<UserRepository> Logger;
    
    public UserRepository(IConfiguration configuration, ILogger<UserRepository> logger) : base(configuration, logger)
    {
        Logger = logger;
    }


    public async Task<List<ApplicationUser>> GetUsers()
    {
        var sql = @"SELECT * FROM public.""AspNetUsers""";
        await using var conn = await GetNewOpenConnection();
        var rv = await conn.QueryAsync<ApplicationUser>(sql);
        return rv.ToList();
    }

    public async Task<List<user_invitation>> GetInvites()
    {
        var sql = @"SELECT * FROM user_invitation WHERE is_deleted = false";
        await using var conn = await GetNewOpenConnection();
        var rv = await conn.QueryAsync<user_invitation>(sql);
        return rv.ToList();
    }

    public async Task<user_invitation?> GetInvite(string inviteId)
    {
        var sql = @"SELECT * FROM user_invitation WHERE is_deleted = false AND invitation_identifier = @id";
        await using var conn = await GetNewOpenConnection();
        var rv = await conn.QueryFirstOrDefaultAsync<user_invitation>(sql, new {id = inviteId});
        return rv;
    }
}