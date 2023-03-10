using APICatalogo.DTOs;
using APICatalogo.Models;
using APICatalogo.Pagination;
using APICatalogo.Repository;
using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace APICatalogo.Controllers
{
    [Produces("application/json")]
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class CategoriasController : ControllerBase
    {
        private readonly IUnitOfWork _context;
        private readonly IMapper _mapper;

        public CategoriasController(IUnitOfWork context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        [HttpGet("produtos")]
        public async Task<ActionResult<IEnumerable<CategoriaDTO>>> GetCategoriasProdutos()
        {
            var categorias = await _context.CategoriaRepository.GetCategoriasProdutos();
            var categoriasDto = _mapper.Map<List<CategoriaDTO>>(categorias);

            return categoriasDto;
        }

        [HttpGet("paginacao")]
        public async Task<ActionResult<IEnumerable<CategoriaDTO>>> 
            GetPaginacao([FromBody] CategoriasParameters categoriasParameters)
        {
            var categorias = await _context.CategoriaRepository.
                            GetCategorias(categoriasParameters);

            var metadata = new
            {
                categorias.TotalCount,
                categorias.PageSize,
                categorias.CurrentPage,
                categorias.TotalPages,
                categorias.HasNext,
                categorias.HasPrevious
            };

            Response.Headers.Add("X-Pagination", JsonSerializer.Serialize(metadata));

            var categoriasDto = _mapper.Map<List<CategoriaDTO>>(categorias); 

            return categoriasDto;
        }

        /// <summary>
        /// Obtem uma Ctegoria pelo seu ID
        /// </summary>
        /// <param name="id"></param>
        /// <returns>Objetos Categoria</returns>
        [HttpGet("{id:int}", Name = "ObterCategoria")]
        [ProducesResponseType(typeof(ProdutoDTO),StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<CategoriaDTO>> Get(int id)
        {
            try
            {
                var categoria = await _context.CategoriaRepository.GetById(p => p.CategoriaId == id);
                var categoriaDto = _mapper.Map<CategoriaDTO>(categoria);

                if (categoria == null)
                {
                    return NotFound($"Cateogria com id={id}  não encontrada...");
                }

                return categoriaDto;
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                                   "Ocorreu um problema ao tratar a sua solicitação");
            }
        }
        /// <summary>
        /// Incui uma nova categoria
        /// </summary>
        /// <remarks>
        /// Exemplo de request:
        ///     POST api/categorias
        ///     {
        ///         "categoriaId": 1,
        ///         "nome": "categoria1",
        ///         "imageUrl":"http://teste.net/.jpg"
        ///     }
        /// </remarks>
        /// <param name="categoriaDTO">objeto Categoria</param>
        /// <returns>O objeto Categoria incluida</returns>
        /// <remarks>Retorna um objeto Categoria incluido</remarks>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult> Post([FromBody] CategoriaDTO categoriaDTO)
        {
            var categoria = _mapper.Map<Categoria>(categoriaDTO);
            if (categoria is null)
            {
                return BadRequest("Dados inválidos");
            }
            _context.CategoriaRepository.Add(categoria);
            await _context.Commit();

            var categoriaDto = _mapper.Map<CategoriaDTO>(categoria);

            return new CreatedAtRouteResult("ObterCategoria", new { id = categoria.CategoriaId }, categoriaDto);
        }

        [HttpPut("{id:int}")]
        [ApiConventionMethod(typeof(DefaultApiConventions),nameof(DefaultApiConventions.Put))]
        public async Task<ActionResult> Put(int id,[FromBody] CategoriaDTO categoriaDto)
        {
            if (id != categoriaDto.CategoriaId)
            {
                return BadRequest("Dados inválidos");
            }

            var categoria = _mapper.Map<Categoria>(categoriaDto);

            _context.CategoriaRepository.Update(categoria);
            await _context.Commit();
            return Ok();
        }

        [HttpDelete]
        public async Task<ActionResult<CategoriaDTO>> Delete(int id)
        {
            var categoria = await _context.CategoriaRepository.GetById(p => p.CategoriaId == id);

            if (categoria == null)
            {
                return NotFound($"Cateogria com id={id} não encontrada...");
            }

            _context.CategoriaRepository.Delete(categoria);
            await _context.Commit();

            var categoriaDto = _mapper.Map<CategoriaDTO>(categoria);

            return Ok(categoriaDto);
        }
    }
}
