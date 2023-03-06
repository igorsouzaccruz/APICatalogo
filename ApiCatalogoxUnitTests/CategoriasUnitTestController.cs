using APICatalogo.Context;
using APICatalogo.Controllers;
using APICatalogo.DTOs;
using APICatalogo.DTOs.Mappings;
using APICatalogo.Repository;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApiCatalogoxUnitTests
{
    internal class CategoriasUnitTestController
    {
        private IMapper mapper;
        private IUnitOfWork repository;

        public static DbContextOptions<AppDbContext> dbContextOptions { get; }

        public static string connectionString =
            "Server=localhost;DataBase=CatalogoDB;Uid=developer;Pwd=1234567";

        static CategoriasUnitTestController()
        {
            dbContextOptions = new DbContextOptionsBuilder<AppDbContext>()
                .UseMySql(connectionString, ServerVersion.AutoDetect(connectionString))
               .Options;
        }

        public CategoriasUnitTestController()
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile(new MappingProfile());
            });
            mapper = config.CreateMapper();

            var context = new AppDbContext(dbContextOptions);

            //DBUnitTestsMockInitializer db = new DBUnitTestsMockInitializer();
            //db.Seed(context);

            repository = new UnitOfWork(context);
        }

        //testes Unitários==========================================
        //testar o método GET
        //[Fact] 
        //public void GetCategorias_Return_OkResult()
        //{
        //    //Arrange
        //    var controller = new CategoriasController(repository, mapper);

        //    //Act
        //    var data = controller.Get();

        //    //Assert
        //    Assert.IsType<List<CategoriaDTO>>(data.Value);
        //}

        //GET - BADREQUEST
        [Fact]
        public void GetCategorias_Return_BadRequestResult()
        {
            //Arrange
            var controller = new CategoriasController(repository, mapper);

            //Act
            var data = controller.GetCategoriasProdutos();

            //Assert
            Assert.IsType<BadRequestResult>(data.Result);
        }

        //GET retornar uma lista de objetos categoria
        //[Fact]
        //public void GetCategorias_MatchResult()
        //{
        //    //Arrange
        //    var controller = new CategoriasController(repository, mapper);

        //    //Act
        //    var data = controller.GetCategoriasProdutos();

        //    //Assert
        //    Assert.IsType<List<CategoriaDTO>>(data.Value);
        //    var cat = data.Value.Should().BeAssignableTo < List<CategoriaDTO>().Subject;

        //    Assert.Equal("Bebidas alterada", cat[0].Nome);
        //    Assert.Equal("bebidas21.jpg", cat[0].ImagemUrl);

        //    Assert.Equal("Sobremesas", cat[0].Nome);
        //    Assert.Equal("sobremesas.jpg", cat[0].ImagemUrl);
        //}

        //get categoria por id
        [Fact]
        public void GetCategoriaById_Return_OkResult()
        {
            //Arrange
            var controller = new CategoriasController(repository, mapper);
            var catId = 2;

            //Act
            var data = controller.Get(catId);

            //Assert
            Assert.IsType<CategoriaDTO>(data.Result);
        }



    }
}
