using Microsoft.EntityFrameworkCore;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

// Register CommentsMgtContext
builder.Services.AddDbContext<Content_App_POC.CommentsMgt.CommentsMgtContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("CommentsMgt")));
// Register CommentsMgt repository and service
builder.Services.AddScoped<Content_App_POC.CommentsMgt.ICommentRepository, Content_App_POC.CommentsMgt.CommentRepository>();
builder.Services.AddScoped<Content_App_POC.CommentsMgt.ICommentService, Content_App_POC.CommentsMgt.CommentService>();

builder.Services.AddControllers();
//builder.Services.AddCors(options =>
//{
//    options.AddPolicy("AllowAll", policy =>
//    {
//        policy.AllowAnyOrigin()
//              .AllowAnyHeader()
//              .AllowAnyMethod()
//              .AllowCredentials();
//    });
//});

builder.CreateUmbracoBuilder()
    .AddBackOffice()
    .AddWebsite()
    .AddDeliveryApi()
    .AddComposers()
    .Build();

WebApplication app = builder.Build();

//app.UseCors("AllowAll");

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

await app.BootUmbracoAsync();


app.UseUmbraco()
    .WithMiddleware(u =>
    {
        u.UseBackOffice();
        u.UseWebsite();
    })
    .WithEndpoints(u =>
    {
        u.UseInstallerEndpoints();
        u.UseBackOfficeEndpoints();
        u.UseWebsiteEndpoints();
    });

await app.RunAsync();
