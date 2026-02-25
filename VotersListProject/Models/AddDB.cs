using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.Web.CodeGeneration.EntityFrameworkCore;

namespace VotersListProject.Models
{
    public class AddDB : DbContext
    {

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            //optionsBuilder.UseMySql("Server=database-1.c1w04g6a0x81.ap-south-1.rds.amazonaws.com;Database=test_mydb;port=3306;User=admin;Password=ABCD12345",
            //    ServerVersion.AutoDetect("Server = database-1.c1w04g6a0x81.ap-south-1.rds.amazonaws.com; Database = test_mydb;port=3306; User = admin; Password = ABCD12345")
            //    );


            optionsBuilder.UseMySql("Server=localhost;Database=projectdb;User=root;Password=@Ashish01",
                ServerVersion.AutoDetect("Server = localhost; Database = projectdb; User = root; Password = @Ashish01")
                );


        }
        public DbSet<VoterModel> votertable { get; set; }
        public DbSet<UpdateRequest> UpdateRequests { get; set; }

        // test




    }
}
