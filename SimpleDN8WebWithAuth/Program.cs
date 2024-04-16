using Azure.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SimpleDN8WebWithAuth.Data;

namespace SimpleDN8WebWithAuth
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
            builder.Services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(connectionString));
            builder.Services.AddDatabaseDeveloperPageExceptionFilter();

            var contextOptions = new DbContextOptionsBuilder<ApplicationDbContext>().UseSqlServer(connectionString).Options;
            using (var context = new ApplicationDbContext(contextOptions))
            {
                context.Database.Migrate();
            }

            builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
                .AddEntityFrameworkStores<ApplicationDbContext>();
            builder.Services.AddControllersWithViews();

            builder.Host.ConfigureAppConfiguration((hostingContext, config) =>
            {
                var settings = config.Build();
                var env = settings["Application:Environment"];
                if (env == null || !env.Trim().Equals("develop", StringComparison.OrdinalIgnoreCase))
                {
                    //requires managed identity on both app service and app config
                    var cred = new ManagedIdentityCredential();
                    config.AddAzureAppConfiguration(options =>
                                options.Connect(new Uri(settings["AzureAppConfigConnection"]), cred));

                    //enable this and disable the two lines above to utilize Key vault in combination with the Azure App Configuration:
                    //note: This required an access policy for key vault for the app service and app config
                    //config.AddAzureAppConfiguration(options =>
                    //        options.Connect(new Uri(settings["AzureAppConfigConnection"]), cred)
                    //                .ConfigureKeyVault(kv => { kv.SetCredential(cred); }));
                }
                else
                {
                    var cred = new DefaultAzureCredential();
                    config.AddAzureAppConfiguration(options =>
                        options.Connect(settings["AzureAppConfigConnection"]));

                    //enable this and disable the two lines above to utilize Key vault in combination with the Azure App Configuration:
                    //note: This required an access policy for key vault for the developer
                    //config.AddAzureAppConfiguration(options =>
                    //    options.Connect(settings["AzureAppConfigConnection"])
                    //                    .ConfigureKeyVault(kv => { kv.SetCredential(cred); }));
                }
            });

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

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapRazorPages();

app.Run();
        }
    }
}
