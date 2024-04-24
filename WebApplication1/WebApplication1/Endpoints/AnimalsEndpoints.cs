namespace WebApplication1.Endpoints;

using System.Data.SqlClient;
using FluentValidation;
using WebApplication1.DTOs;


// SqlClient example with minimal api

public static class AnimalsEndpoints
{
    public static void RegisterAnimalsEndpoints(this WebApplication app)
    {
        var students = app.MapGroup("minimal-animals");

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
            var sqlCommand = new SqlCommand(
                "UPDATE Animals SET Name = @1, Description = @2, Category = @3, Area = @4 WHERE ID = @5",
                sqlConnection
            );
            sqlCommand.Parameters.AddWithValue("@1", request.Name);
            sqlCommand.Parameters.AddWithValue("@2", request.Description);
            sqlCommand.Parameters.AddWithValue("@3", request.Category);
            sqlCommand.Parameters.AddWithValue("@4", request.Area);
            sqlCommand.Parameters.AddWithValue("@5", id);
            sqlCommand.Connection.Open();

            var affectedRows = sqlCommand.ExecuteNonQuery();
            return affectedRows == 0 ? Results.NotFound() : Results.NoContent();
        }
    }

    private static IResult RemoveAnimal(IConfiguration configuration, int id)
    {
        using (var sqlConnection = new SqlConnection(configuration.GetConnectionString("Default")))
        {
            var command = new SqlCommand("DELETE FROM Animals WHERE ID = @1", sqlConnection);
            command.Parameters.AddWithValue("@1", id);
            command.Connection.Open();

            var affectedRows = command.ExecuteNonQuery();

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
            var sqlCommand = new SqlCommand(
                "INSERT INTO Animals (Name, Description, Category, Area) values (@1, @2, @3, @4); SELECT CAST(SCOPE_IDENTITY() as int)",
                sqlConnection
                );
            sqlCommand.Parameters.AddWithValue("@1", request.Name);
            sqlCommand.Parameters.AddWithValue("@2", request.Description);
            sqlCommand.Parameters.AddWithValue("@3", request.Category);
            sqlCommand.Parameters.AddWithValue("@4", request.Area);
            sqlCommand.Connection.Open();
            
            var id = sqlCommand.ExecuteScalar();
            
            return Results.Created($"students/{id}", new CreateAnimalResponse((int)id, request));
        }
    }

    private static IResult GetAnimals(IConfiguration configuration)
    {
        var response = new List<GetAnimalsResponse>();
        using (var sqlConnection = new SqlConnection(configuration.GetConnectionString("Default")))
        {
            var sqlCommand = new SqlCommand("SELECT * FROM Animals", sqlConnection);
            sqlCommand.Connection.Open();
            var reader = sqlCommand.ExecuteReader();
            while (reader.Read())
            {
                response.Add(new GetAnimalsResponse(
                        reader.GetInt32(0), 
                        reader.GetString(1), 
                        reader.GetString(2), 
                        reader.GetString(3), 
                        reader.GetString(4)
                    )
                );
            }
        }
        return Results.Ok(response);
    }

    private static IResult GetAnimal(IConfiguration configuration, int id)
    {
        using var sqlConnection = new SqlConnection(configuration.GetConnectionString("Default"));
        var sqlCommand = new SqlCommand("SELECT * FROM Animals WHERE ID = @1", sqlConnection);
        sqlCommand.Parameters.AddWithValue("@1", id);
        sqlCommand.Connection.Open();

        var reader = sqlCommand.ExecuteReader();
        if (!reader.Read()) return Results.NotFound();

        return Results.Ok(new GetAnimalDetailsResponse(
                reader.GetInt32(0), 
                reader.GetString(1),
                reader.GetString(2), 
                reader.GetString(3), 
                reader.GetString(4)
            )
        );
    }
}