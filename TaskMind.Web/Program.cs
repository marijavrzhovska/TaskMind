using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using TaskMind.Repository.Data;
using TaskMind.Domain.Identity;
using TaskMind.Repository.Interface;
using TaskMind.Repository.Implementation;
using TaskMind.Service.Interface;
using TaskMind.Service.Implementation;
using TaskMind.Service.Interface.Dto;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(connectionString));

builder.Services.AddDefaultIdentity<User>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddEntityFrameworkStores<ApplicationDbContext>();
builder.Services.AddControllersWithViews();

builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
builder.Services.AddTransient<IProjectService, ProjectService>();
builder.Services.AddTransient<IFileAttachmentService, FileAttachmentService>();
builder.Services.AddTransient<ITagService, TagService>();
builder.Services.AddTransient<ITaskService, TaskService>();
builder.Services.AddTransient<ICommentService, CommentService>();
builder.Services.AddHttpClient<IExternalProjectService, ExternalProjectService>();

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
using (var scope = app.Services.CreateScope())
{
    var tagService = scope.ServiceProvider.GetRequiredService<ITagService>();
    if (!tagService.GetAll().Any())
    {
        var tags = new List<TaskMind.Domain.Models.Tag>
        {
            new TaskMind.Domain.Models.Tag { Name = "Urgent" },
            new TaskMind.Domain.Models.Tag { Name = "Bug" },
            new TaskMind.Domain.Models.Tag { Name = "Feature" },
            new TaskMind.Domain.Models.Tag { Name = "Backend" },
            new TaskMind.Domain.Models.Tag { Name = "Frontend" }
        };
        foreach (var tag in tags)
            tagService.Insert(tag);
    }
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
