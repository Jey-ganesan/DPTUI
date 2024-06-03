using Microsoft.Extensions.FileProviders;

//Log.Logger = new LoggerConfiguration().MinimumLevel.Debug().MinimumLevel.Override("Microsoft", LogEventLevel.Warning).Enrich.FromLogContext().WriteTo.File(
//                        $"logs/log-{DateTime.Now:yyyy-MM-dd}.txt",
//                        rollingInterval: RollingInterval.Day).CreateLogger();

try
{
    //Log.Information("Log File");

    var builder = WebApplication.CreateBuilder(args);

    string baseUrl = builder.Configuration.GetSection("MySetting:ApiURL").Value;

    builder.Services.AddHttpClient("DPTClient", configClient =>
    {
        configClient.BaseAddress = new Uri(baseUrl);
        configClient.Timeout = new TimeSpan(0, 1, 0);
        configClient.DefaultRequestHeaders.Add("Accept", "applications/json");

    }).ConfigurePrimaryHttpMessageHandler(() =>
    {
        var handler = new SocketsHttpHandler();
        //handler.AllowAutoRedirect = false;
        return handler;
    });

    // Add services to the container.
    builder.Services.AddControllersWithViews();

    builder.Services.AddSession(option =>
    {
        option.IdleTimeout = TimeSpan.FromHours(8);
        option.Cookie.SecurePolicy = CookieSecurePolicy.Always;
        option.Cookie.IsEssential = true;
    });

    var app = builder.Build();

    // Configure the HTTP request pipeline.
    if (app.Environment.IsDevelopment())
    {
        app.UseExceptionHandler("/Home/Error");
        // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
        app.UseHsts();
    }

    app.UseSession();

    app.UseHttpsRedirection();

    app.UseStaticFiles(new StaticFileOptions
    {
        FileProvider = new PhysicalFileProvider(Path.Combine(Directory.GetCurrentDirectory(), "themes")),
        RequestPath = "/themes"
    });

    app.UseStaticFiles();

    app.UseRouting();

    app.UseAuthorization();

    app.MapControllerRoute(
        name: "default",
        pattern: "{controller=Home}/{action=Index}/{id?}");

    app.Run();
}
catch (Exception ex)
{
    //Log.Fatal(ex, "Application terminated unexpectedly");
}
finally
{
    //Log.CloseAndFlush();
}