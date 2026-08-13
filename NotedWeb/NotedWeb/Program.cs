using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

NotedWeb.AppCode.Config.MyProps.Initialize(builder.Configuration);

builder.Services.AddRazorPages();
builder.Services.AddControllers();   // API controllers
builder.Services.AddOpenApi();       // OpenAPI document generation
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<NotedWeb.Services.Auth.AuthService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
	app.UseExceptionHandler("/Error");
	app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();


app.MapControllers();
app.MapOpenApi();

// Scalar UI
app.MapScalarApiReference(options =>
{
	options.WithTitle("NotedWeb Internal API");     //https://localhost:7190/scalar/v1
});

app.MapRazorPages()
   .WithStaticAssets();

app.Run();
