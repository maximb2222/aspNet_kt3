using CoreWCF;
using CoreWCF.Configuration;
using CoreWCF.Description;
using KT3.BookingPrototype.Core.Contracts;
using KT3.BookingPrototype.Service.Repositories;
using KT3.BookingPrototype.Service.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddServiceModelServices();
builder.Services.AddServiceModelMetadata();
builder.Services.AddSingleton<IBookingRepository, InMemoryBookingRepository>();
builder.Services.AddSingleton<BookingService>();

var app = builder.Build();

var metadata = app.Services.GetRequiredService<ServiceMetadataBehavior>();
metadata.HttpGetEnabled = true;

app.UseServiceModel(serviceBuilder =>
{
    serviceBuilder.AddService<BookingService>();
    serviceBuilder.AddServiceEndpoint<BookingService, IBookingService>(new BasicHttpBinding(), "/BookingService.svc");
});

app.MapGet("/", () => Results.Ok(new
{
    service = "KT3 Booking CoreWCF Service",
    wsdl = "http://localhost:8080/BookingService.svc?wsdl",
    apiKey = AuthHeader.DemoApiKey
}));

app.Run("http://localhost:8080");
