using System;
using System.Linq;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using MoviesApp.Models;

namespace MoviesApp.Data
{
    public static class SeedData
    {
        public static void Initialize(IServiceProvider serviceProvider)
        {
            using (var context = new MoviesContext(
                serviceProvider.GetRequiredService<
                    DbContextOptions<MoviesContext>>()))
            {
                if (!context.Movies.Any())
                {
                    context.Movies.AddRange(
                        new Movie
                        {
                            Title = "Harry Potter",
                            ReleaseDate = DateTime.Parse("1989-2-12"),
                            Genre = "Fantastic",
                            Price = 7.99M
                        },
                        new Movie
                        {
                            Title = "Ghostbusters ",
                            ReleaseDate = DateTime.Parse("1984-3-13"),
                            Genre = "Comedy",
                            Price = 8.99M
                        },
                        new Movie
                        {
                            Title = "Ghostbusters 2",
                            ReleaseDate = DateTime.Parse("1986-2-23"),
                            Genre = "Comedy",
                            Price = 9.99M
                        },
                        new Movie
                        {
                            Title = "Rio Bravo",
                            ReleaseDate = DateTime.Parse("1959-4-15"),
                            Genre = "Western",
                            Price = 3.99M
                        }
                    );

                    context.SaveChanges();
                }

                if (!context.Artists.Any())
                {
                    context.Artists.AddRange(
                        new Artist
                        {
                            Firstname = "Daniel",
                            Lastname = "Radcliffe",
                            BirthdayDate = DateTime.Parse("1975-6-4")
                        },
                        new Artist
                        {
                            Firstname = "Roopert",
                            Lastname = "Green",
                            BirthdayDate = DateTime.Parse("1967-7-26")
                        },
                        new Artist
                        {
                            Firstname = "Emma",
                            Lastname = "Watson",
                            BirthdayDate = DateTime.Parse("1963-12-18")
                        }
                    );

                    context.SaveChanges();
                }

                var userManager = serviceProvider.GetService<UserManager<ApplicationUser>>();
                var roleManager = serviceProvider.GetService<RoleManager<IdentityRole>>();

                if (!roleManager.RoleExistsAsync("Admin").Result)
                {
                    roleManager.CreateAsync(new IdentityRole {Name = "Admin"}).Wait();
                }

                if (userManager.FindByEmailAsync("admin@gmail.com").Result == null)
                {
                    var user = new ApplicationUser
                    {
                        UserName = "admin@gmail.com",
                        Email = "admin@gmail.com",
                        Firstname = "Admin",
                        Lastname = "Admin"
                    };

                    IdentityResult result = userManager.CreateAsync(user, "Qwe123").Result;

                    if (result.Succeeded)
                    {
                        userManager.AddToRoleAsync(user, "Admin").Wait();
                    }
                }
            }
        }
    }
}