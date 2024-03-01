using LeviathanRipBlog.Services;
using LeviathanRipBlog.Web;
using LeviathanRipBlog.Web.Data;
using LeviathanRipBlog.Web.Services.Authorization;
using LeviathanRipBlog.Web.Services.Documents;
using LeviathanRipBlog.Web.Settings;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Supabase;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddResponseCaching();

// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("DbConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(connectionString));


builder.Services.AddSingleton<IAuthorizationHandler, RecordAuthorizationHandler>();

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy(Policy.CanEditCampaign, policy =>
        policy.Requirements.Add(new CampaignOwnerRequirement())
    );
    options.AddPolicy(Policy.CanEditBlog, policy =>
        policy.Requirements.Add(new BlogOwnerRequirement())
    );
});

builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddHttpClient();
builder.Services.AddHttpContextAccessor();

builder.Services.ConfigureApplicationCookie(o => {
    o.Cookie.Name = ".leviathanrip.blog";
    o.LoginPath = "/login";
    o.LogoutPath = "/logout";
    o.AccessDeniedPath = "/401";
    o.Events.OnSigningOut = ctx => {
        ctx.HttpContext.Session.Clear();
        return Task.CompletedTask;
    };
});

builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options => {
        options.SignIn.RequireConfirmedAccount = false;
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

builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/login";
    options.AccessDeniedPath = "/401";
});

builder.Services.AddMvc();
builder.Services.AddControllersWithViews().AddRazorRuntimeCompilation();

var spacesUrl = builder.Configuration.GetSection("Spaces")["SpacesUrl"];
var supabaseUrl = builder.Configuration.GetSection("Supabase")["SupabaseUrl"];

if (!spacesUrl.IsNullOrEmpty())
{
    Console.WriteLine("Using Spaces for document storage.");
    builder.Services.AddTransient<IDocumentStorage, SpacesDocumentStorage>();
}
else if (!supabaseUrl.IsNullOrEmpty())
{
    Console.WriteLine("Using Supabase for document storage.");
    builder.Services.AddTransient<IDocumentStorage, SupabaseDocumentStorage>();
}
else
{
    Console.WriteLine("Using local file storage for document storage.");
    builder.Services.AddTransient<IDocumentStorage, FileDocumentStorage>();
}

// Register all services
ConfigureSettings(builder.Services, builder.Configuration);
RegisterServices.ConfigureServices(services: builder.Services);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseResponseCaching();
app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapGet("/login", context => Task.Factory.StartNew(() => context.Response.Redirect("/Identity/Account/Login", true, true)));
app.MapGet("/register", context => Task.Factory.StartNew(() => context.Response.Redirect("/Identity/Account/Register", true, true)));
app.MapGet("/logout", context => Task.Factory.StartNew(() => context.Response.Redirect("/Identity/Account/Logout", true, true)));
app.MapControllers();
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapRazorPages();


using (var scope = app.Services.CreateScope()) {
    await CreateRoles(scope.ServiceProvider);
}

app.Run();

void ConfigureSettings(IServiceCollection services, IConfiguration config) {
    services.Configure<S3Settings>(config.GetSection("Spaces"));
    services.Configure<SupabaseSettings>(config.GetSection("Supabase"));
}



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