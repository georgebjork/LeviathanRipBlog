using LeviathanRipBlog;
using LeviathanRipBlog.Components;
using LeviathanRipBlog.Components.Account;
using LeviathanRipBlog.Data;
using LeviathanRipBlog.Middleware;
using LeviathanRipBlog.Settings;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddCascadingAuthenticationState();
builder.Services.AddScoped<IdentityUserAccessor>();
builder.Services.AddScoped<IdentityRedirectManager>();
builder.Services.AddScoped<AuthenticationStateProvider, IdentityRevalidatingAuthenticationStateProvider>();


var connectionString = builder.Configuration.GetConnectionString("DbConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(connectionString));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options => {
        options.SignIn.RequireConfirmedAccount = true;
        options.Password.RequireDigit = false;
        options.Password.RequiredLength = 8;
        options.Password.RequireNonAlphanumeric = false;
        options.Password.RequireUppercase = false;
        options.Password.RequireLowercase = false;
        options.User.RequireUniqueEmail = true;
    })
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddSignInManager()
    .AddRoles<IdentityRole>()
    .AddDefaultTokenProviders();


builder.Services.AddSingleton<IEmailSender<ApplicationUser>, IdentityNoOpEmailSender>();

RegisterServices.Configure(services: builder.Services);

var app = builder.Build();

app.UseMiddleware<BlockIdentityPathMiddleware>();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();

    using var scope = app.Services.CreateScope();
    var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    db.Database.Migrate();
}
else
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
    
}

app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseAntiforgery();

app.UseAuthorization();

app.MapRazorComponents<App>().AddInteractiveServerRenderMode();

// Add additional endpoints required by the Identity /Account Razor components.
app.MapAdditionalIdentityEndpoints();


using (var scope = app.Services.CreateScope()) {
    await CreateRoles(scope.ServiceProvider);
}


app.Run();




async Task CreateRoles(IServiceProvider serviceProvider) {
    //initializing custom roles 
    var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
    var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();
    string[] roleNames = { Roles.ADMIN, Roles.USER, };

    foreach (var roleName in roleNames) {
        var roleExist = await roleManager.RoleExistsAsync(roleName);
        // ensure that the role does not exist
        if (!roleExist) {
            //create the roles and seed them to the database: 
            await roleManager.CreateAsync(new IdentityRole(roleName));
        }
    }

    //Here you create the super admin who will maintain the web app
    await AddAdminUser(userManager, "georgebjork@outlook.com", "password");
}

async Task AddAdminUser(UserManager<ApplicationUser> userManager, string username, string password) {
    var user = await userManager.FindByEmailAsync(username);

    // check if the user exists
    if (user == null) {
        //Here you could create the super admin who will maintain the web app
        var seedUser = new ApplicationUser {
            UserName = username,
            Email = username,
            EmailConfirmed = true
        };
        var createPowerUser = await userManager.CreateAsync(seedUser, password);
        if (createPowerUser.Succeeded) {
            //here we tie the new user to the role
            await userManager.AddToRoleAsync(seedUser, Roles.ADMIN);
        }
    }
}
