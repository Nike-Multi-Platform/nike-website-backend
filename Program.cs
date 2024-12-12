using Microsoft.EntityFrameworkCore;
using nike_website_backend.Interfaces;
using nike_website_backend.Models;
using nike_website_backend.Services;
using FirebaseAdmin;
using Google;
using Google.Apis.Auth.OAuth2;
using Quartz;
using nike_website_backend.Jobs;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
});

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;  // Tương tự như ReferenceLoopHandling.Ignore trong Newtonsoft.Json

    });

builder.Services.AddQuartz(q =>
{
    q.UseMicrosoftDependencyInjectionJobFactory();

    var flashSaleJobKey = new JobKey("FlashSaleJob");
    q.AddJob<FlashSaleJob>(opts => opts.WithIdentity(flashSaleJobKey));

    q.AddTrigger(opts => opts
        .ForJob(flashSaleJobKey)
        .WithIdentity("FlashSaleJob-trigger")
        .WithCronSchedule("0 0/1 * * * ?"));
    //0 0 0 * * ?

    var flashSaleTimeFrameJobKey = new JobKey("FlashSaleTimeFrameJob");
    q.AddJob<FlashSaleTimeFrameJob>(opts => opts.WithIdentity(flashSaleTimeFrameJobKey));

    q.AddTrigger(opts => opts
        .ForJob(flashSaleTimeFrameJobKey)
        .WithIdentity("FlashSaleTimeFrameJob-trigger")
        .WithCronSchedule("0 0/1 * * * ?"));
    //0 0 0 / 2 * * ?
});

builder.Services.AddQuartzHostedService(q => q.WaitForJobsToComplete = true);
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<IProductRepository, ProductService>();
builder.Services.AddScoped<ICategoryRepository, CategoryService>();
builder.Services.AddScoped<IUserAccountRepository, UserAccountService>();
builder.Services.AddScoped<IFlashSaleRepository, FlashSaleService>();
builder.Services.AddScoped<IBagRepository, BagService>();
builder.Services.AddScoped<IFavoriteRepository, FavoriteService>();
builder.Services.AddScoped<IPaymentRepository, PaymentService>();
builder.Services.AddScoped<IUserOrderRepository, UserOrderService>();
builder.Services.AddCors(options =>
{
    options.AddPolicy("ReactApp", policyBuilder =>
    {
        policyBuilder.WithOrigins("http://localhost:3000").AllowAnyMethod().AllowAnyHeader().AllowCredentials();
        //policyBuilder.AllowAnyMethod();
        //policyBuilder.AllowAnyHeader();
        //policyBuilder.AllowCredentials();
    });
});


// firebase create 
try
{
    FirebaseApp.Create(new AppOptions()
    {
        Credential = GoogleCredential.FromFile("Configs/nike-d3392-firebase-adminsdk-t6ndk-364532f7b5.json")
    });
}
catch (Exception ex)
{
    Console.WriteLine($"Error initializing Firebase: {ex.Message}");
    throw;
}
builder.Services.AddHttpClient();
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors("ReactApp");
app.UseAuthorization();

app.MapControllers();

app.Run();

