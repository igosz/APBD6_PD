using WebApplication1.DTOs;

namespace WebApplication1.Endpoints;
using System.Data;
using System.Data.SqlClient;
using FluentValidation;
using Dapper;


/*
 * SqlClient with Dapper example
 * https://www.learndapper.com/
 */

public static class AnimalsDapperEndpoints
{
    public static void RegisterAnimalsDapperEndpoints(this WebApplication app)
    {
        var students = app.MapGroup("minimal-animals-dapper");

        students.MapGet("/", GetAnimals);
        students.MapGet("{id:int}", GetAnimal);
        students.MapPost("/", CreateAnimal);
        students.MapDelete("{id:int}", RemoveAnimal);
        students.MapPut("{id:int}", ReplaceAnimal);
    }

    private static IResult ReplaceAnimal(IConfiguration configuration, IValidator<ReplaceAnimalRequest> validator, int id, ReplaceAnimalRequest request)
    {
        
        var validation = validator.Validate(request);
        if (!validation.IsValid)
        {
            return Results.ValidationProblem(validation.ToDictionary());
        }
        
        using (var sqlConnection = new SqlConnection(configuration.GetConnectionString("Default")))
        {
            var affectedRows = sqlConnection.Execute(
                "UPDATE Animals SET Name = @Name, Description = @Description, Category = @Category, Area = @Area WHERE ID = @Id",
                new
                {
                    Name = request.Name, 
                    Description = request.Description, 
                    Category = request.Category,
                    Area = request.Area, 
                    Id = id
                }
            );
            
            if (affectedRows == 0) return Results.NotFound();
        }

        return Results.NoContent();
    }

    private static IResult RemoveAnimal(IConfiguration configuration, int id)
    {
        using (var sqlConnection = new SqlConnection(configuration.GetConnectionString("Default")))
        {
            var affectedRows = sqlConnection.Execute(
                "DELETE FROM Animals WHERE ID = @Id",
                new { Id = id }
            );
            return affectedRows == 0 ? Results.NotFound() : Results.NoContent();
        }
    }

    private static IResult CreateAnimal(IConfiguration configuration, IValidator<CreateAnimalRequest> validator, CreateAnimalRequest request)
    {
        var validation = validator.Validate(request);
        if (!validation.IsValid)
        {
            return Results.ValidationProblem(validation.ToDictionary());
        }

        using (var sqlConnection = new SqlConnection(configuration.GetConnectionString("Default")))
        {
            var id = sqlConnection.ExecuteScalar<int>(
                "INSERT INTO Animals (Name, Description, Category, Area) values (@Name, @Description, @Category, @Area); SELECT CAST(SCOPE_IDENTITY() as int)",
                new
                {
                    Name = request.Name, 
                    Description = request.Description, 
                    Category = request.Category,
                    Area = request.Area
                }
            );

            return Results.Created($"/animals-dapper/{id}", new CreateAnimalResponse(id, request));
        }
    }

    private static IResult GetAnimals(IConfiguration configuration)
    {
        using (var sqlConnection = new SqlConnection(configuration.GetConnectionString("Default")))
        {
            var animals = sqlConnection.Query<GetAnimalsResponse>("SELECT * FROM Animals");
            return Results.Ok(animals);
        }
    }

    private static IResult GetAnimal(IConfiguration configuration, int id)
    {
        using (var sqlConnection = new SqlConnection(configuration.GetConnectionString("Default")))
        {
            var student = sqlConnection.QuerySingleOrDefault<GetAnimalDetailsResponse>(
                "SELECT * FROM Animals WHERE ID = @Id",
                new { Id = id }
            );

            if (student is null) return Results.NotFound();
            return Results.Ok(student);
        }
    }
}