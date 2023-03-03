using APICatalogo.DTOs;
using APICatalogo.Models;
using APICatalogo.Pagination;
using APICatalogo.Repository;
using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace APICatalogo.Controllers
{
    [ApiConventionType(typeof(DefaultApiConventions))]
    [Produces("application/json")]
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class ProdutosController : ControllerBase
    {
        private readonly IUnitOfWork _uof;
        private readonly IMapper _mapper;
        public ProdutosController(IUnitOfWork context, IMapper mapper)
        {
            _uof = context;
            _mapper = mapper;
        }

        [HttpGet("menorpreco")]
        public async Task<ActionResult<IEnumerable<ProdutoDTO>>> GetProdutosPorPreco()
        {
            var produtos =  await _uof.ProdutoRepository.GetProdutosPorPreco();
            var produtosDto = _mapper.Map<List<ProdutoDTO>>(produtos);

            return produtosDto;
        }
        /// <summary>
        /// Exibe uma relação dos produtos
        /// </summary>
        /// <param name="produtosParameters"></param>
        /// <returns>Retorna uma lista de objetos Produto</returns>
        /// api/produtos
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ProdutoDTO>>>
            Get([FromQuery] ProdutosParameters produtosParameters)
        {
            var produtos = await _uof.ProdutoRepository.GetProdutos(produtosParameters);

            var metadata = new
            {
                produtos.TotalCount,
                produtos.PageSize,
                produtos.CurrentPage,
                produtos.TotalPages,
                produtos.HasNext,
                produtos.HasPrevious
            };

            Response.Headers.Add("X-Pagination", JsonSerializer.Serialize(metadata));
             

            var produtosDto = _mapper.Map<List<ProdutoDTO>>(produtos);

            if (produtos is null)
            {
                return NotFound("Produtos não encontrados....");
            }

            return produtosDto;
        }
        /// <summary>
        /// Obtem um produto pelo seu identificador produtoId
        /// </summary>
        /// <param name="id">Código do produto</param>
        /// <returns>Um objeto Produto</returns>
        /// api/produtos/1
        [HttpGet("{id:int}", Name="ObterProduto")]
        public async Task<ActionResult<ProdutoDTO>> Get(int id)
        {
            var produto = await _uof.ProdutoRepository.GetById(p => p.ProdutoId == id);
            var produtoDto = _mapper.Map<ProdutoDTO>(produto);

            if (produto is null)
            {
                return NotFound("Produto não encontrado...");
            }
            
            return produtoDto;
        }

        [HttpPost]
        public async Task<ActionResult> Post(ProdutoDTO produtoDto)
        {
            var produto = _mapper.Map<Produto>(produtoDto);
            if(produto is null)
            {
                return BadRequest();
            }

            _uof.ProdutoRepository.Add(produto);
            await _uof.Commit();

            var produtoDTO = _mapper.Map<ProdutoDTO>(produto);   

            return new CreatedAtRouteResult("ObterProduto",
                new { id = produto.ProdutoId }, produtoDTO);
        }

        [HttpPut("{id:int}")]
        public async Task<ActionResult> Put(int id,[FromBody] ProdutoDTO produtoDto)
        {
            if(id != produtoDto.ProdutoId)
            {
                return BadRequest();
            }

            var produto = _mapper.Map<Produto>(produtoDto);

            _uof.ProdutoRepository.Update(produto);
            await _uof.Commit();
            return Ok(produto);
        }

        [HttpDelete("{id:int}")]
        public async Task<ActionResult<ProdutoDTO>> Delete(int id)
        {
            var produto = await _uof.ProdutoRepository.GetById(p => p.ProdutoId == id);

            if(produto is null)
            {
                return NotFound();
            }
            _uof.ProdutoRepository.Delete(produto);
            await _uof.Commit();

            var produtoDto = _mapper.Map<ProdutoDTO>(produto);


            return Ok(produtoDto);
        }
    }
}
