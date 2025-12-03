using Back_End_tb4.Domain.Entities;

namespace Back_End_tb4.Infrastructure.Persistence;

public static class SeedData
{
    public static async Task InitializeAsync(EiraMindDbContext context)
    {
        if (!context.Users.Any())
        {
            var users = new List<User>
            {
                new User
                {
                    Email = "admin@eiramind.com",
                    Password = "admin123",
                    Nombre = "Administrador",
                    Tipo = "paciente",
                    CreatedAt = DateTime.Parse("2024-01-01T00:00:00.000Z")
                },
                new User
                {
                    Email = "usuario@test.com",
                    Password = "test123",
                    Nombre = "Usuario Test",
                    Tipo = "paciente",
                    CreatedAt = DateTime.Parse("2024-01-15T00:00:00.000Z")
                }
            };
            await context.Users.AddRangeAsync(users);
        }
            
        if (!context.Psychologists.Any())
        {
            var psychologists = new List<Psychologist>
            {
                new Psychologist
                {
                    Email = "alberto.salas@eiramind.com",
                    Password = "psi123",
                    Nombre = "Alberto Salas",
                    Tipo = "psicologo",
                    Especialidad = "violencia, ansiedad",
                    Calificacion = 5,
                    Imagen = "https://i.pravatar.cc/150?img=12",
                    Descripcion = "Especialista en terapia cognitivo-conductual con 10 años de experiencia trabajando con pacientes con ansiedad y estrés.",
                    Biografia = "El Dr. Alberto Salas es un psicólogo clínico dedicado con más de una década de experiencia ayudando a individuos a superar desafíos relacionados con la ansiedad, el estrés y la violencia.",
                    Educacion = "Doctorado en Psicología Clínica - Universidad Nacional Mayor de San Marcos",
                    Certificaciones = "Certificado en Terapia Cognitivo-Conductual (TCC),Especialización en Manejo de Crisis,Diplomado en Violencia Intrafamiliar",
                    Idiomas = "Español,Inglés",
                    Metodologias = "Terapia Cognitivo-Conductual,Terapia de Exposición,Mindfulness",
                    AnosExperiencia = 10,
                    AtendeEdades = "Adultos (18-65 años)",
                    Horarios = @"[
                            {
                                ""fecha"": ""2024-10-22"",
                                ""dia"": ""Martes 22 octubre"",
                                ""horas"": [""10:00"", ""18:00"", ""20:00"", ""22:00""]
                            },
                            {
                                ""fecha"": ""2024-10-23"",
                                ""dia"": ""Miércoles 23 octubre"",
                                ""horas"": [""09:00"", ""14:00"", ""16:00"", ""21:00""]
                            }
                        ]"
                }
                // Puedes agregar más psicólogos aquí
            };
            await context.Psychologists.AddRangeAsync(psychologists);
        }
            
        await context.SaveChangesAsync();
    }
}