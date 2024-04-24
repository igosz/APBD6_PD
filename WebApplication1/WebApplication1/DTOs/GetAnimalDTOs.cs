namespace WebApplication1.DTOs;

public record GetAnimalDetailsResponse(int Id, string Name, string Description, string Category, string Area);