
using TwitterClone.Application.Interfaces;
using TwitterClone.Application.Services;
using TwitterClone.Infrastructure.Repositories;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Repositories: Singleton so the in-memory lists survive across requests
// for the lifetime of the application, which is fine for an in-memory store.
builder.Services.AddSingleton<IUserRepository, UserRepository>();
builder.Services.AddSingleton<ITweetRepository, TweetRepository>();
builder.Services.AddSingleton<IFollowRepository, FollowRepository>();
builder.Services.AddSingleton<ILikeRepository, LikeRepository>();
builder.Services.AddSingleton<IBookmarkRepository, BookmarkRepository>();
builder.Services.AddSingleton<IRetweetRepository, RetweetRepository>();
builder.Services.AddSingleton<IMessageRepository, MessageRepository>();
builder.Services.AddSingleton<INotificationRepository, NotificationRepository>();

// Application services: Scoped is the conventional lifetime for application services.
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<ITweetService, TweetService>();
builder.Services.AddScoped<IFollowService, FollowService>();
builder.Services.AddScoped<ILikeService, LikeService>();
builder.Services.AddScoped<IBookmarkService, BookmarkService>();
builder.Services.AddScoped<IRetweetService, RetweetService>();
builder.Services.AddScoped<IMessageService, MessageService>();
builder.Services.AddScoped<INotificationService, NotificationService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
