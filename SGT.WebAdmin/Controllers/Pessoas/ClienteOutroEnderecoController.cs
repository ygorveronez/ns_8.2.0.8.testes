using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;


namespace SGT.WebAdmin.Controllers.Pessoas
{
    [CustomAuthorize("Pessoas/ClienteOutroEndereco")]
    public class ClienteOutroEnderecoController : BaseController
    {
		#region Construtores

		public ClienteOutroEnderecoController(Conexao conexao) : base(conexao) { }

		#endregion

        [AllowAuthenticate]
        public async Task<IActionResult> Pesquisa()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                double cliente = double.Parse(Request.Params("Cliente"));
                int localidade = int.Parse(Request.Params("Localidade"));

                Models.Grid.Grid grid = new Models.Grid.Grid(Request);
                grid.header = new List<Models.Grid.Head>();
                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("Descricao", false);
                grid.AdicionarCabecalho("Cidade", "Cidade", 35, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Polo", "LocalidadePolo", 25, Models.Grid.Align.center, false);
                grid.AdicionarCabecalho("Endereço", "Endereco", 30, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Estado", false);
                grid.AdicionarCabecalho("Pais", false);
                grid.AdicionarCabecalho("Pais", false);
                grid.AdicionarCabecalho("CEP", false);
                grid.AdicionarCabecalho("Numero", false);
                grid.AdicionarCabecalho("Bairro", false);
                grid.AdicionarCabecalho("Complemento", false);
                grid.AdicionarCabecalho("Telefone", false);
                grid.AdicionarCabecalho("CodigoIBGE", false);
                grid.AdicionarCabecalho("CodigoCliente", false);
                grid.AdicionarCabecalho("DescricaoCliente", false);
                grid.AdicionarCabecalho("Latitude", false);
                grid.AdicionarCabecalho("Longitude", false);

                Repositorio.Embarcador.Pessoas.ClienteOutroEndereco repClienteOutroEndereco = new Repositorio.Embarcador.Pessoas.ClienteOutroEndereco(unitOfWork);

                List<Dominio.Entidades.Embarcador.Pessoas.ClienteOutroEndereco> listaClienteOutroEndereco = repClienteOutroEndereco.Consultar(cliente, localidade, grid.header[grid.indiceColunaOrdena].data, grid.dirOrdena, grid.inicio, grid.limite);
                int totalRegistros = repClienteOutroEndereco.ContarConsulta(cliente, localidade);
                grid.setarQuantidadeTotal(totalRegistros);

                dynamic lista = (from p in listaClienteOutroEndereco
                                 select new
                                 {
                                     p.Codigo,
                                     Cidade = p.Localidade.DescricaoCidadeEstado,
                                     Estado = p.Localidade.Estado.Sigla,
                                     Pais = p.Localidade.Pais.Nome,
                                     Descricao = p.Localidade.DescricaoCidadeEstado,
                                     p.Endereco,
                                     p.CEP,
                                     p.Numero,
                                     p.Bairro,
                                     p.Complemento,
                                     Telefone = !string.IsNullOrWhiteSpace(p.Telefone) ? p.Telefone.ObterTelefoneFormatado() : p.Cliente.Telefone1.ObterTelefoneFormatado(),
                                     LocalidadePolo = p.Localidade.LocalidadePolo?.DescricaoCidadeEstado ?? "",
                                     p.Localidade.CodigoIBGE,
                                     CodigoCliente = p.Cliente.CPF_CNPJ,
                                     DescricaoCliente = p.Cliente.Descricao,
                                     Latitude = p.Latitude,
                                     Longitude = p.Longitude,
                                 }).ToList();

                grid.AdicionaRows(lista);
                return new JsonpResult(grid);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao consultar.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        private string buscarDescricaoCidade(Dominio.Entidades.Localidade cidade)
        {
            if (cidade.CodigoIBGE != 9999999 || cidade.Pais == null)
                return cidade.Descricao;
            else
                return cidade.Descricao + " (" + cidade.Pais.Nome + ")";

        }
    }
}
