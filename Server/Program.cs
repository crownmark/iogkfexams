using Radzen;
using IOGKFExams.Server.Components;
using Microsoft.EntityFrameworkCore;
using Microsoft.OData.ModelBuilder;
using Microsoft.AspNetCore.OData;
using IOGKFExams.Server.Data;
using Microsoft.AspNetCore.Identity;
using IOGKFExams.Server.Models;
using Microsoft.AspNetCore.Components.Authorization;

var builder = WebApplication.CreateBuilder(args);
// Add services to the container.
builder.Services.AddRazorComponents().AddInteractiveWebAssemblyComponents();
builder.Services.AddControllers();
builder.Services.AddRadzenComponents();
builder.Services.AddRadzenCookieThemeService(options =>
{
    options.Name = "IOGKFExamsTheme";
    options.Duration = TimeSpan.FromDays(365);
});
builder.Services.AddHttpClient();
builder.Services.AddScoped<IOGKFExams.Server.IOGKFExamsDbService>();
builder.Services.AddDbContext<IOGKFExams.Server.Data.IOGKFExamsDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("IOGKFExamsDbConnection"));
});
builder.Services.AddControllers().AddOData(opt =>
{
    var oDataBuilderIOGKFExamsDb = new ODataConventionModelBuilder();
    oDataBuilderIOGKFExamsDb.EntitySet<IOGKFExams.Server.Models.IOGKFExamsDb.ExamAnswer>("ExamAnswers");
    oDataBuilderIOGKFExamsDb.EntitySet<IOGKFExams.Server.Models.IOGKFExamsDb.ExamQuestion>("ExamQuestions");
    oDataBuilderIOGKFExamsDb.EntitySet<IOGKFExams.Server.Models.IOGKFExamsDb.Exam>("Exams");
    oDataBuilderIOGKFExamsDb.EntitySet<IOGKFExams.Server.Models.IOGKFExamsDb.ExamStatus>("ExamStatuses");
    oDataBuilderIOGKFExamsDb.EntitySet<IOGKFExams.Server.Models.IOGKFExamsDb.ExamTemplateAnswer>("ExamTemplateAnswers");
    oDataBuilderIOGKFExamsDb.EntitySet<IOGKFExams.Server.Models.IOGKFExamsDb.ExamTemplateQuestion>("ExamTemplateQuestions");
    oDataBuilderIOGKFExamsDb.EntitySet<IOGKFExams.Server.Models.IOGKFExamsDb.ExamTemplate>("ExamTemplates");
    oDataBuilderIOGKFExamsDb.EntitySet<IOGKFExams.Server.Models.IOGKFExamsDb.Language>("Languages");
    oDataBuilderIOGKFExamsDb.EntitySet<IOGKFExams.Server.Models.IOGKFExamsDb.Rank>("Ranks");
    opt.AddRouteComponents("odata/IOGKFExamsDb", oDataBuilderIOGKFExamsDb.GetEdmModel()).Count().Filter().OrderBy().Expand().Select().SetMaxTop(null).TimeZone = TimeZoneInfo.Utc;
});
builder.Services.AddScoped<IOGKFExams.Client.IOGKFExamsDbService>();
builder.Services.AddHttpClient("IOGKFExams.Server").ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler { UseCookies = false }).AddHeaderPropagation(o => o.Headers.Add("Cookie"));
builder.Services.AddHeaderPropagation(o => o.Headers.Add("Cookie"));
builder.Services.AddAuthentication();
builder.Services.AddAuthorization();
builder.Services.AddScoped<IOGKFExams.Client.SecurityService>();
builder.Services.AddDbContext<ApplicationIdentityDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("IOGKFExamsDbConnection"));
});
builder.Services.AddIdentity<ApplicationUser, ApplicationRole>().AddEntityFrameworkStores<ApplicationIdentityDbContext>().AddDefaultTokenProviders();
builder.Services.AddControllers().AddOData(o =>
{
    var oDataBuilder = new ODataConventionModelBuilder();
    oDataBuilder.EntitySet<ApplicationUser>("ApplicationUsers");
    var usersType = oDataBuilder.StructuralTypes.First(x => x.ClrType == typeof(ApplicationUser));
    usersType.AddProperty(typeof(ApplicationUser).GetProperty(nameof(ApplicationUser.Password)));
    usersType.AddProperty(typeof(ApplicationUser).GetProperty(nameof(ApplicationUser.ConfirmPassword)));
    oDataBuilder.EntitySet<ApplicationRole>("ApplicationRoles");
    o.AddRouteComponents("odata/Identity", oDataBuilder.GetEdmModel()).Count().Filter().OrderBy().Expand().Select().SetMaxTop(null).TimeZone = TimeZoneInfo.Utc;
});
builder.Services.AddScoped<AuthenticationStateProvider, IOGKFExams.Client.ApplicationAuthenticationStateProvider>();
builder.Services.AddLocalization();
var app = builder.Build();
var forwardingOptions = new ForwardedHeadersOptions()
{
    ForwardedHeaders = Microsoft.AspNetCore.HttpOverrides.ForwardedHeaders.XForwardedFor | Microsoft.AspNetCore.HttpOverrides.ForwardedHeaders.XForwardedProto
};
forwardingOptions.KnownNetworks.Clear();
forwardingOptions.KnownProxies.Clear();
app.UseForwardedHeaders(forwardingOptions);
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseWebAssemblyDebugging();
}
else
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseStatusCodePagesWithReExecute("/not-found");
app.UseHttpsRedirection();
app.MapControllers();
app.UseHeaderPropagation();
app.UseRequestLocalization(options => options.AddSupportedCultures("en", "es-MX", "ja-JP").AddSupportedUICultures("en", "es-MX", "ja-JP").SetDefaultCulture("en"));
app.UseStaticFiles();
app.UseAuthentication();
app.UseAuthorization();
app.UseAntiforgery();
app.MapRazorComponents<App>().AddInteractiveWebAssemblyRenderMode().AddAdditionalAssemblies(typeof(IOGKFExams.Client._Imports).Assembly);
app.Services.CreateScope().ServiceProvider.GetRequiredService<ApplicationIdentityDbContext>().Database.Migrate();
app.Run();