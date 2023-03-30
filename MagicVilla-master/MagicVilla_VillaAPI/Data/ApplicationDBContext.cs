using MagicVilla_VillaAPI.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace MagicVilla_VillaAPI.Data
{
    public class ApplicationDBContext : DbContext
    {
        public ApplicationDBContext(DbContextOptions<ApplicationDBContext> options) : base(options)
        {

        }
        public DbSet<Villa> Villas { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Villa>().HasData(
                new Villa()
                {
                    Id = 1,
                    Name = "Royal Villa",
                    Details = "lorem ipsum",
                    ImageUrl = "",
                    Occupancy = 5,
                    Rate = 200,
                    Sqft = 550,
                    Amenity = "",
                    CreatedDate=DateTime.Now

                },
                 new Villa()
                 {
                     Id = 2,
                     Name = "mr Villa",
                     Details = "lorem ipsum",
                     ImageUrl = "",
                     Occupancy = 4,
                     Rate = 100,
                     Sqft = 250,
                     Amenity = "",
                     CreatedDate = DateTime.Now


                 },
                  new Villa()
                  {
                      Id = 3,
                      Name = "Mrs Villa",
                      Details = "lorem ipsum",
                      ImageUrl = "",
                      Occupancy = 8,
                      Rate = 900,
                      Sqft = 150,
                      Amenity = "",
                      CreatedDate = DateTime.Now


                  },
                   new Villa()
                   {
                       Id = 4,
                       Name = "Royal Villa",
                       Details = "lorem ipsum",
                       ImageUrl = "",
                       Occupancy = 8,
                       Rate = 500,
                       Sqft = 350,
                       Amenity = "",
                       CreatedDate = DateTime.Now


                   }
                );
        }
    }
}
