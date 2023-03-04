using MediatR;
using MusicDownloader.Business.Requests.Youtube.Metadata;
using MusicDownloader.Startup;

// Retrieve FFmpeg binaries for local use
await FFmpegConfigurator.ConfigureFFmpeg();

// Create web builder
var builder = WebApplication.CreateBuilder(args);

// Cors domains
// const string allowSpecificOrigins = "AllowSpecificOrigins";
const string allowAllOrigins = "AllowAllOrigins";
// var allowedOrigins = new[]
// {
//     "http://localhost:5173",
//     "https://localhost:5173",
//     "http://localhost:4173",
//     "https://localhost:4173",
//     "https://music-downloader.goffincedric.be"
// };
builder.Services.AddCors(options => options.AddPolicy(allowAllOrigins, policy =>
    policy.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader()
));

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

app.UseCors(allowAllOrigins);
app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();

/*
 * TODO:
 *  - Logging
 *  - Error handling
 *  - API Key auth
 *  - Filter out 'artist name -' and '- artist name' from song title + trim song title
 *  - add restriction to max 15 mins of video download + no livestreams
 */
// Linux: sudo apt-get install -y ffmpeg libgdiplus