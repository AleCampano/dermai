using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.Google;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();

builder.Services.AddSingleton<Kernel>(sp =>
{
    var kernelBuilder = Kernel.CreateBuilder();

    kernelBuilder.AddGoogleAIGeminiChatCompletion(
        modelId: "gemini-2.5-flash",
        apiKey: "AIzaSyDlCPMG-_7TjIDlY5dRkktpqPL7iPO2Q88"
    );

    return kernelBuilder.Build();
});

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.UseSession();
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
