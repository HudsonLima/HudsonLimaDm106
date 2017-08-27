namespace HudsonLimaDm106.Migrations
{
    using HudsonLimaDm106.Models;
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;

    internal sealed class Configuration : DbMigrationsConfiguration<HudsonLimaDm106.Models.ApplicationDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
            ContextKey = "HudsonLimaDm106.Models.ApplicationDbContext";
        }

        protected override void Seed(HudsonLimaDm106.Models.ApplicationDbContext context)
        {
            //  This method will be called after migrating to the latest version.

            //  You can use the DbSet<T>.AddOrUpdate() helper extension method 
            //  to avoid creating duplicate seed data. E.g.
            //
            //    context.People.AddOrUpdate(
            //      p => p.FullName,
            //      new Person { FullName = "Andrew Peters" },
            //      new Person { FullName = "Brice Lambson" },
            //      new Person { FullName = "Rowan Miller" }
            //    );
            //
            
            
                
            context.Products.AddOrUpdate(
                                    p => p.Id,
                                    new Product
                                    {
                                        Id = 1,
                                        nome = "produto	1",
                                        codigo =    "COD00001",
                                        descricao = "descrição	produto	1",
                                        modelo = "modelo produto	1",
                                        preco = 10,
                                        Url    = "www.hudsonlima.com/produto1"
                                    },
                                    new Product
                                    {
                                        Id = 2,
                                        nome = "produto	2",
                                        codigo =    "COD00002",
                                        descricao = "descrição	produto	2",
                                        modelo = "modelo produto	2",
                                        preco = 20,
                                        Url    = "www.hudsonlima.com/produto2"
                                    },
                                    new Product
                                    {
                                        Id = 3,
                                        nome = "produto	3",
                                        codigo =    "COD00003",
                                        descricao = "descrição	produto	3",
                                        modelo = "modelo produto	3",
                                        preco = 30,
                                        Url    = "www.hudsonlima.com/produto3"
                                    }
                    );
        }
    }
}
