using AutoMapper;
using BackendEPPO.Controllers;
using BusinessObjects.Models;
using DTOs.Settings;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Repository.Implements;
using Repository.Interfaces;
using Service;
using Service.Implements;
using Service.Interfaces;
using System.Reflection;
using System.Text;
using System.Text.Json.Serialization;
using static BackendEPPO.Extenstion.ApiEndPointConstant;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddHttpClient();

builder.Services.AddControllers().AddJsonOptions(x =>
                x.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles); ;
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<IConversationService, ConversationService>();
builder.Services.AddScoped<IMessageService, MessageService>();
builder.Services.AddScoped<ITransactionService, TransactionService>();
builder.Services.AddScoped<IOrderService, OrderService>();
builder.Services.AddScoped<FirebaseStorageService>();

//Do Huu Thuan
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IContractService, ContractService>();
builder.Services.AddScoped<IPlantService, PlantService>();
builder.Services.AddScoped<ICategoryService, CategoryService>();
builder.Services.AddScoped<IAddressService, AddressService>();
builder.Services.AddScoped<IContractDetailServices, ContractDetailServices>();
builder.Services.AddScoped<IRoomService, RoomService>();
builder.Services.AddScoped<INotificationService, NotificationService>();
builder.Services.AddScoped<IWalletService, WalletService>(); 
builder.Services.AddScoped<ITypeEcommerceService, TypeEcommerceService>();
builder.Services.AddScoped<IFeedbackService, FeedbackService>();
builder.Services.AddScoped<IImageFeedbackService, ImageFeedbackService>();
builder.Services.AddScoped<IUserRoomService, UserRoomService>();
builder.Services.AddScoped<IHistoryBidService, HistoryBidService>();
builder.Services.AddScoped<ITokenService, TokenService>();
builder.Services.AddHttpClient<DistanceController>();
builder.Services.Configure<RentalSettings>(builder.Configuration.GetSection("RentalSettings"));


// AutoMapper configuration
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());


builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;  // Thêm dòng này
    options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme; // Thêm dòng này
}).AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters()
    {
        ValidateIssuer = false,
        ValidateAudience = false,
        ValidateIssuerSigningKey = true,
        IssuerSigningKey =
            new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
    };
    options.Events = new JwtBearerEvents
    {
        OnMessageReceived = context =>
        {
            var token = context.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
            if (!string.IsNullOrEmpty(token))
            {
                context.Token = token;
            }
            return Task.CompletedTask;
        }
    };
})
.AddCookie()
.AddGoogle(options =>
{
    options.ClientId = builder.Configuration["GoogleAuth:ClientId"];
    options.ClientSecret = builder.Configuration["GoogleAuth:ClientSecret"];
    options.CallbackPath = "/api/Login/GoogleResponse";
    options.SaveTokens = true;
});

// Add bearn to using the Authorization
builder.Services.AddSwaggerGen(option =>
{
    option.DescribeAllParametersInCamelCase();

    option.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Description = "Please enter a valid token",
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        BearerFormat = "JWT",
        Scheme = "Bearer"
    });
    option.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type=ReferenceType.SecurityScheme,
                    Id="Bearer"
                },
                Scheme = "oauth2",
                Name = "Bearer",
                In = ParameterLocation.Header,
            },
              new List<string>()
        }
    });


    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    option.IncludeXmlComments(xmlPath);


});


// Add CORS policy
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policyBuilder =>
    {
        policyBuilder.AllowAnyOrigin()
                     .AllowAnyHeader()
                     .AllowAnyMethod();
    });
});


//DBContext
builder.Services.AddDbContext<bef4qvhxkgrn0oa7ipg0Context>(options =>
    options.UseMySQL(builder.Configuration.GetConnectionString("DefaultConnection")));



// WebSocket (chat service)
builder.Services.AddSingleton<ChatHandler>();

builder.Services.AddSingleton<AuctionHandler>();

builder.Services.AddHostedService<AuctionMonitorService>();
builder.Services.AddHostedService<OrderCancellationService>();

//Add cors for website
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAllOrigins",
        builder =>
        {
            builder.WithOrigins("https://localhost:7152", "https://localhost:7026", "http://localhost:3000")
                   .AllowAnyMethod()
                   .AllowAnyHeader()
                   .AllowCredentials();
        });
});


var app = builder.Build();

var webSocketOptions = new WebSocketOptions
{
    KeepAliveInterval = TimeSpan.FromMinutes(2),
    ReceiveBufferSize = 4 * 1024
};

app.UseWebSockets(webSocketOptions);

// Middleware để xử lý WebSocket
app.Use(async (context, next) =>
{
    if (context.Request.Path == "/ws/chat")
    {
        if (context.WebSockets.IsWebSocketRequest)
        {
            var chatHandler = context.RequestServices.GetRequiredService<ChatHandler>();
            var webSocket = await context.WebSockets.AcceptWebSocketAsync();
            await chatHandler.HandleAsync(webSocket);
        }
        else
        {
            context.Response.StatusCode = 400;
        }
    }
    else
    {
        await next();
    }
});

app.Use(async (context, next) =>
{
    if (context.Request.Path == "/ws/auction")
    {
        if (context.WebSockets.IsWebSocketRequest)
        {
            var auctionHandler = context.RequestServices.GetRequiredService<AuctionHandler>();
            var webSocket = await context.WebSockets.AcceptWebSocketAsync();
            await auctionHandler.HandleAsync(webSocket);
        }
        else
        {
            context.Response.StatusCode = 400;
        }
    }
    else
    {
        await next();
    }
});

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();

app.UseCors();

//app.UseCors("AllowAllOrigins");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
