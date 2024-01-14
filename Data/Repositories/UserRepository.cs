using Dapper;

namespace LeviathanRipBlog.Data.Repositories;


public interface IUserRepository : IBaseRepository
{
    Task<List<ApplicationUser>> GetUsers();
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
}