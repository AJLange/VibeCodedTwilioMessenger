var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();

// Configure Twilio settings
builder.Services.Configure<TwilioMessenger.Web.Models.TwilioSettings>(
    builder.Configuration.GetSection("TwilioSettings"));

// Add repositories and services
builder.Services.AddSingleton<TwilioMessenger.Core.Services.IContactRepository>(sp =>
{
    var filePath = Path.Combine(builder.Environment.ContentRootPath, 
        builder.Configuration["ContactsFilePath"] ?? "Data/contacts.json");
    return new TwilioMessenger.Core.Services.FileContactRepository(filePath);
});

builder.Services.AddSingleton<TwilioMessenger.Core.Services.ITwilioMessagingService>(sp =>
{
    var settings = builder.Configuration.GetSection("TwilioSettings").Get<TwilioMessenger.Web.Models.TwilioSettings>();
        
    if (settings == null) 
    {
        // Create default settings to prevent startup crash
        settings = new TwilioMessenger.Web.Models.TwilioSettings
        {
            AccountSid = builder.Configuration["TwilioSettings:AccountSid"] ?? "ACXXXXXXXXXXXXXXXXXXXX",
            AuthToken = builder.Configuration["TwilioSettings:AuthToken"] ?? "your_auth_token_here",
            FromPhoneNumber = builder.Configuration["TwilioSettings:FromPhoneNumber"] ?? "+12345678901",
            WhatsAppFromPhoneNumber = builder.Configuration["TwilioSettings:WhatsAppFromPhoneNumber"] ?? "+12345678901"
        };
    }
    
    var contactRepository = sp.GetRequiredService<TwilioMessenger.Core.Services.IContactRepository>();
    
    return new TwilioMessenger.Core.Services.TwilioMessagingService(
        settings.AccountSid,
        settings.AuthToken,
        settings.FromPhoneNumber,
        settings.WhatsAppFromPhoneNumber,
        contactRepository);
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseRouting();

app.UseAuthorization();

app.MapStaticAssets();
app.MapRazorPages()
   .WithStaticAssets();

app.Run();
