using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.Pessoas
{
    [CustomAuthorize("Pessoas/ClienteComplementar")]
    public class ClienteComplementarController : BaseController
    {
		#region Construtores

		public ClienteComplementarController(Conexao conexao) : base(conexao) { }

		#endregion

        [AllowAuthenticate]
        public async Task<IActionResult> Pesquisa()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Models.Grid.Grid grid = new Models.Grid.Grid(Request);
                grid.header = new List<Models.Grid.Head>();

                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("Cliente", "Descricao", 40, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("CPF/CNPJ Cliente", "CPFCNPJCliente", 30, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Escritório de Vendas", "EscritorioVendas", 35, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Matriz", "Matriz", 25, Models.Grid.Align.center, true);

                Repositorio.Embarcador.Pessoas.ClienteComplementar repositorioClienteComplementar = new Repositorio.Embarcador.Pessoas.ClienteComplementar(unitOfWork);

                Dominio.ObjetosDeValor.Embarcador.Pessoas.FiltroPesquisaClienteComplementar filtrosPesquisa = ObterFiltrosPesquisa();
                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = grid.ObterParametrosConsulta(ObterPropriedadeOrdenar);
                List<Dominio.Entidades.Embarcador.Pessoas.ClienteComplementar> listaClienteComplementar = repositorioClienteComplementar.Consultar(filtrosPesquisa, parametrosConsulta);
                int totalRegistros = repositorioClienteComplementar.ContarConsulta(filtrosPesquisa);

                grid.setarQuantidadeTotal(totalRegistros);

                dynamic lista = (from clienteComplementar in listaClienteComplementar
                                 select new
                                 {
                                     Codigo = clienteComplementar.Cliente.CPF_CNPJ,
                                     clienteComplementar.Cliente.Descricao,
                                     CPFCNPJCliente = clienteComplementar.Cliente.CPF_CNPJ_Formatado,
                                     clienteComplementar.EscritorioVendas,
                                     clienteComplementar.Matriz
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

        private Dominio.ObjetosDeValor.Embarcador.Pessoas.FiltroPesquisaClienteComplementar ObterFiltrosPesquisa()
        {
            Dominio.ObjetosDeValor.Embarcador.Pessoas.FiltroPesquisaClienteComplementar filtrosPesquisa = new Dominio.ObjetosDeValor.Embarcador.Pessoas.FiltroPesquisaClienteComplementar()
            {
                CodigoCliente = Request.GetDoubleParam("Cliente"),
                EscritorioVenda = Request.GetStringParam("EscritorioVenda"),
                Matriz = Request.GetStringParam("Matriz")
            };

            return filtrosPesquisa;
        }

        private string ObterPropriedadeOrdenar(string propriedadeOrdenar)
        {
            if (propriedadeOrdenar == "Descricao")
                return "Cliente.Nome";

            if (propriedadeOrdenar == "CPFCNPJCliente")
                return "Cliente.CPF_CNPJ";

            return propriedadeOrdenar;
        }
    }
}
