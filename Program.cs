using BugTrackerAPI.Entities;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using System.Data.SqlClient;


var builder = WebApplication.CreateBuilder(args); 
builder.Services.AddControllers();
var app = builder.Build(); 


app.MapGet("/", () => {
    return "Okay";
});

app.UseRouting();
app.MapControllers();
app.Run();
