using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Register CommentsMgtContext
builder.Services.AddDbContext<Content_App_POC.CommentsMgt.CommentsMgtContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("CommentsMgt")));
// Register CommentsMgt repository and service
//builder.Services.AddScoped<Content_App_POC.CommentsMgt.ICommentRepository, Content_App_POC.CommentsMgt.CommentRepository>();
//builder.Services.AddScoped<Content_App_POC.CommentsMgt.ICommentService, Content_App_POC.CommentsMgt.CommentService>();

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
});
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.UseDeveloperExceptionPage();
}

app.UseCors("AllowAll");

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
