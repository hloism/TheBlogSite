using TheBlogSite.Components.Account;
using TheBlogSite.Components;
using TheBlogSite.Data;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using TheBlogSite.Services;
using TheBlogSite.Client.Services.Interfaces;
using TheBlogSite.Services.Interfaces;
using TheBlogSite.Services.Repository;
using TheBlogSite.Client.Models;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Net.Http.Headers;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.Filters;
using Scalar.AspNetCore;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents()
    .AddInteractiveWebAssemblyComponents();

builder.Services.AddCascadingAuthenticationState();
builder.Services.AddScoped<IdentityUserAccessor>();
builder.Services.AddScoped<IdentityRedirectManager>();
builder.Services.AddScoped<AuthenticationStateProvider, PersistingRevalidatingAuthenticationStateProvider>();

builder.Services.AddControllers();
builder.Services.AddOutputCache();

builder.Services.AddAuthentication(options =>
{
    options.DefaultScheme = IdentityConstants.ApplicationScheme;
    options.DefaultSignInScheme = IdentityConstants.ExternalScheme;
})
  .AddIdentityCookies(cookieOpts =>
  { // Make our API controllers return 401/403 instead of the HTML login page
      cookieOpts.ApplicationCookie!.Configure(cfg =>
      {
          cfg.Events.OnRedirectToLogin += (ctx) =>
          {
              if (ctx.Request.Path.StartsWithSegments("/api"))
              {
                  ctx.Response.StatusCode = 401;
              }

              return Task.CompletedTask;
          };

          cfg.Events.OnRedirectToAccessDenied += (ctx) =>
          {
              if (ctx.Request.Path.StartsWithSegments("/api"))
              {
                  ctx.Response.StatusCode = 403;
              }

              return Task.CompletedTask;
          };
      });
  });

//looks in json file for getconnectionstring
var connectionString = DataUtility.GetConnectionString(builder.Configuration) ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
builder.Services.AddDbContextFactory<ApplicationDbContext>(options =>
    options.UseNpgsql(connectionString));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddIdentityCore<ApplicationUser>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddClaimsPrincipalFactory<CustomUserClaimsPrincipalFactory>()
    .AddSignInManager()
    .AddDefaultTokenProviders();

builder.Services.AddHttpClient();

builder.Services.AddScoped<IBlogPostRepository, BlogRepository>();
builder.Services.AddScoped<IBlogPostService, BlogPostService>();


builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
builder.Services.AddScoped<ICategoryService, CategoryService>();

builder.Services.AddScoped<ICommentRepository, CommentRepository>();
builder.Services.AddScoped<ICommentService, CommentService>();

builder.Services.Configure<MailSettings>(builder.Configuration.GetSection("EmailSettings"));
builder.Services.AddSingleton<IEmailSender<ApplicationUser>, GoogleEmailService>();
builder.Services.AddSingleton<IEmailSender, GoogleEmailService>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(opts =>
{
    opts.SwaggerDoc("v1", new() { Title = "YOUR PROJECT NAME HERE", Version = "v1" });

    //Add this if you want to use tokens to test the API
    opts.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = HeaderNames.Authorization,
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",
    });
    //Add this if you want to use cookies to test the API
    opts.AddSecurityDefinition("Cookie", new OpenApiSecurityScheme
    {
        Name = ".AspNetCore.Identity.Application",
        In = ParameterLocation.Cookie,
        Type = SecuritySchemeType.Http,
        Scheme = "Cookie",
    });
    opts.OperationFilter<SecurityRequirementsOperationFilter>();
});




// ...

builder.Services.AddCors(builder =>
{
	builder.AddPolicy("DefaultPolicy", policy =>
	{
		policy.AllowAnyOrigin()
			.AllowAnyHeader()
			.AllowAnyMethod();
	});
});

// AddCors should be above this line
var app = builder.Build();
// UseCors should be below this line

app.UseCors("DefaultPolicy");

// ...

var scope = app.Services.CreateScope();
await DataUtility.ManageDataAsync(scope.ServiceProvider);

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseWebAssemblyDebugging();
    app.UseMigrationsEndPoint();

	app.UseSwagger(o => o.RouteTemplate = "/openapi/{documentName}.json");
	app.MapScalarApiReference(); // /scalar/v1

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

app.UseOutputCache();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode()
    .AddInteractiveWebAssemblyRenderMode()
    .AddAdditionalAssemblies(typeof(TheBlogSite.Client._Imports).Assembly);

// Add additional endpoints required by the Identity /Account Razor components.
app.MapAdditionalIdentityEndpoints();

app.MapControllers();

// GET: api/blogposts
app.MapGet("api/blogposts", async ([FromServices] IBlogPostService blogService,
                                   [FromQuery] int page = 1,
                                   [FromQuery] int pageSize = 4) =>
{
    try
    {
        PagedList<BlogPostDTO> blogPosts = await blogService.GetPublishedPostsAsync(page, pageSize);
        return Results.Ok(blogPosts);
    }
    catch (Exception ex)
    {
        Console.WriteLine(ex);
        return Results.Problem();
    }
});

app.Run();

