using MediatR;
using MusicDownloader.Business.Requests.Youtube.Metadata;
using MusicDownloader.Shared.Extensions;
using MusicDownloader.Startup;

// Retrieve FFmpeg binaries for local use
await FFmpegConfigurator.ConfigureFFmpeg();

// Create web builder
var builder = WebApplication.CreateBuilder(args);

// Cors domains
const string allowSpecificOrigins = "AllowSpecificOrigins";
var allowedOrigins = new[]
{
    "http://localhost:5173",
    "https://localhost:5173"
};
builder.Services.AddCors(options => options.AddPolicy(allowSpecificOrigins, policy => policy.WithOrigins(
    allowedOrigins
)));

// Add services to the container.
builder.Services.AddHttpClient<ResolveVideoCoverImageRequestHandler>();
builder.Services.AddMediatR(typeof(Program));
builder.Services.AddLibraries();
builder.Services.AddBusiness();

builder.Services.AddRouting(options => { options.LowercaseUrls = true; });
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Build web application
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors(allowSpecificOrigins);
app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();

/*
 * TODO:
 *  - Logging
 *  - Error handling
 *  - API Key auth
 */
// Linux: sudo apt-get install -y ffmpeg libgdiplus