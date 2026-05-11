using Microsoft.EntityFrameworkCore;
using SMRS.Web.Components;
using SMRS.Web.Data;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddDbContext<SmrsDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection") ?? "Server=(localdb)\\mssqllocaldb;Database=SMRS_V7;Trusted_Connection=True;MultipleActiveResultSets=true"));

builder.Services.AddScoped<SMRS.Web.Services.IEmailService, SMRS.Web.Services.EmailService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}

app.UseStatusCodePagesWithReExecute("/not-found", createScopeForStatusCodePages: true);
app.UseHttpsRedirection();
app.UseAntiforgery();
app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

// Seed Database
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<SmrsDbContext>();
    db.Database.EnsureCreated();
    
    if (!db.JobRoles.Any())
    {
        var role = new JobRole { JobRoleName = "Academic Management" };
        var dept = new Department { DepartmentName = "Science", JobRole = role };
        var pos = new Position { PositionName = "Physics Teacher", Department = dept, JobRole = role };
        var listing = new PositionListing { Position = pos, Status = "Open" };
        
        db.JobRoles.Add(role);
        db.Departments.Add(dept);
        db.Positions.Add(pos);
        db.PositionListings.Add(listing);
        db.SaveChanges();
    }
}

app.Run();
