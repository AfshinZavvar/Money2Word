using Money2Word.Services.Interfaces;
using Money2Word.Services;
using Newtonsoft.Json.Serialization;
using Microsoft.AspNetCore.Mvc;

namespace Money2Word
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddControllersWithViews().AddJsonOptions(opts => opts.JsonSerializerOptions.PropertyNamingPolicy = null);
            builder.Services.AddApplicationInsightsTelemetry();
            builder.Services.AddTransient<IMoney2WordConvertor, Money2WordConvertor>();
            builder.Services.AddTransient<IMoney2WordService, Money2WordService>();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            app.Run();
        }
    }
}