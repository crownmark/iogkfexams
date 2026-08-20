using IOGKFExams.Client;
using IOGKFExams.Server.Components;
using IOGKFExams.Server.Data;
using IOGKFExams.Server.Models;
using IOGKFExams.Server.Services;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.OData;
using Microsoft.EntityFrameworkCore;
using Microsoft.OData.ModelBuilder;
using QuestPDF.Infrastructure;
using Radzen;
using QuestPDF.Fluent;


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
builder.Services.AddHttpClient<BatchFunctionsService>(client =>
{
    client.BaseAddress = new Uri(builder.Configuration["ApiBaseUrl"] ?? "https://localhost:5001/");
});
builder.Services.AddScoped<IOGKFExams.Server.IOGKFExamsDbService>();
builder.Services.AddDbContext<IOGKFExams.Server.Data.IOGKFExamsDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("IOGKFExamsDbConnection"));
});
builder.Services.AddControllers().AddOData(opt =>
{
    var oDataBuilderIOGKFExamsDb = new ODataConventionModelBuilder();
    oDataBuilderIOGKFExamsDb.EntitySet<IOGKFExams.Server.Models.IOGKFExamsDb.Country>("Countries");
    oDataBuilderIOGKFExamsDb.EntitySet<IOGKFExams.Server.Models.IOGKFExamsDb.ExamAnswer>("ExamAnswers");
    oDataBuilderIOGKFExamsDb.EntitySet<IOGKFExams.Server.Models.IOGKFExamsDb.ExamQuestion>("ExamQuestions");
    oDataBuilderIOGKFExamsDb.EntitySet<IOGKFExams.Server.Models.IOGKFExamsDb.Exam>("Exams");
    oDataBuilderIOGKFExamsDb.EntitySet<IOGKFExams.Server.Models.IOGKFExamsDb.ExamSection>("ExamSections");
    oDataBuilderIOGKFExamsDb.EntitySet<IOGKFExams.Server.Models.IOGKFExamsDb.ExamStatus>("ExamStatuses");
    oDataBuilderIOGKFExamsDb.EntitySet<IOGKFExams.Server.Models.IOGKFExamsDb.ExamTemplateAnswer>("ExamTemplateAnswers");
    oDataBuilderIOGKFExamsDb.EntitySet<IOGKFExams.Server.Models.IOGKFExamsDb.ExamTemplateQuestion>("ExamTemplateQuestions");
    oDataBuilderIOGKFExamsDb.EntitySet<IOGKFExams.Server.Models.IOGKFExamsDb.ExamTemplate>("ExamTemplates");
    oDataBuilderIOGKFExamsDb.EntitySet<IOGKFExams.Server.Models.IOGKFExamsDb.Language>("Languages");
    oDataBuilderIOGKFExamsDb.EntitySet<IOGKFExams.Server.Models.IOGKFExamsDb.NotificationTemplate>("NotificationTemplates");
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
builder.Services.AddDbContext<IOGKFExams.Server.Data.IOGKFExamsDbContext>(options =>
{
    options.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
    options.UseSqlServer(builder.Configuration.GetConnectionString("IOGKFExamsDbConnection"));
});
builder.Services.AddScoped<IExamPdfService, ExamPdfService>();
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

QuestPDF.Settings.License = LicenseType.Community;
app.MapGet(
    "/api/exams/{examGuid}/pdf",
    async (
        string examGuid,
        IExamPdfService pdfService,
        IOGKFExams.Server.IOGKFExamsDbService examService) =>
    {
        var exam = new ExamPdfModel();
            await examService.GetExamForPdf(examGuid);

        if (exam == null)
            return Results.NotFound();

        var pdf =
            pdfService.GenerateStudentExam(exam);

        return Results.File(
            pdf,
            "application/pdf",
            $"IOGKF-Exam-{exam.ExamId}.pdf");
    });
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