using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.Fretes
{
    [CustomAuthorize("Fretes/AgruparTabelaFreteCliente")]
    public class AgruparTabelaFreteClienteController : BaseController
    {
		#region Construtores

		public AgruparTabelaFreteClienteController(Conexao conexao) : base(conexao) { }

		#endregion

        public async Task<IActionResult> Pesquisa()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                // Manipula grids
                Models.Grid.Grid grid = GridPesquisa();

                // Ordenacao da grid
                string propOrdenar = grid.header[grid.indiceColunaOrdena].data;
                PropOrdena(ref propOrdenar, grid.dirOrdena);

                // Busca Dados
                int totalRegistros = 0;
                var lista = ExecutaPesquisa(ref totalRegistros, propOrdenar, grid.dirOrdena, grid.inicio, grid.limite, unitOfWork);

                // Seta valores na grid
                grid.AdicionaRows(FormataPesquisaGrid(lista));
                grid.setarQuantidadeTotal(totalRegistros);

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

        public async Task<IActionResult> Processar()
        {

            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                List<Dominio.Entidades.Embarcador.Frete.TabelaFreteCliente> tabelasFrete = ObterTabelaFreteSelecionados(unitOfWork);

                if(tabelasFrete.Count <= 1)
                    return new JsonpResult(false, true, "É necessário selecionar mais que uma tabela.");

                unitOfWork.Start();

                new Servicos.Embarcador.Frete.TabelaFreteCliente(unitOfWork).AgruparTabelasFreteCliente(tabelasFrete);

                unitOfWork.CommitChanges();
                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao agrupas as tabelas.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }


        #region Métodos Privados
        private Models.Grid.Grid GridPesquisa()
        {
            // Manipula grids
            Models.Grid.Grid grid = new Models.Grid.Grid(Request)
            {
                header = new List<Models.Grid.Head>()
            };

            // Cabecalhos grid
            grid.AdicionarCabecalho("Codigo", false);
            grid.AdicionarCabecalho("Código", "CodigoIntegracao", 10, Models.Grid.Align.left, true);
            grid.AdicionarCabecalho("Tabela", "TabelaFrete", 15, Models.Grid.Align.left, true);
            grid.AdicionarCabecalho("Transportador", "Empresa", 15, Models.Grid.Align.left, true);
            grid.AdicionarCabecalho("Origem", "Origem", 15, Models.Grid.Align.left, true);
            grid.AdicionarCabecalho("Destino", "Destino", 15, Models.Grid.Align.left, true);
            grid.AdicionarCabecalho("Vigência", "Vigencia", 12, Models.Grid.Align.left, true);
            grid.AdicionarCabecalho("Situação", "Ativo", 10, Models.Grid.Align.left, true);

            return grid;
        }

        private void PropOrdena(ref string propOrdenar, string dirOrdena)
        {
            if (propOrdenar == "TabelaFrete")
                propOrdenar = "TabelaFrete.Descricao";
            else if (propOrdenar == "Origem")
                propOrdenar = "ClienteOrigem.Nome";
            else if (propOrdenar == "Destino")
                propOrdenar = "ClienteDestino.Nome " + dirOrdena + ", RegiaoDestino.Descricao " + dirOrdena + ", EstadoDestino.Nome";
            else if (propOrdenar == "Vigencia")
                propOrdenar = "Vigencia.DataInicial";
            else if (propOrdenar == "Empresa")
                propOrdenar = "Empresa.RazaoSocial";
        }

        private List<Dominio.Entidades.Embarcador.Frete.TabelaFreteCliente> ExecutaPesquisa(ref int totalRegistros, string propOrdenar, string dirOrdena, int inicio, int limite, Repositorio.UnitOfWork unitOfWork)
        {
            // Instancia repositorios
            Repositorio.Embarcador.Frete.TabelaFreteCliente repTabelaFreteCliente = new Repositorio.Embarcador.Frete.TabelaFreteCliente(unitOfWork);

            // Dados do filtro
            string codigo = Request.GetStringParam("CodigoIntegracao");

            // Consulta
            List<Dominio.Entidades.Embarcador.Frete.TabelaFreteCliente> lista = repTabelaFreteCliente.ConsultarTabelasAgrupamento(codigo, propOrdenar, dirOrdena, inicio, limite);
            totalRegistros = repTabelaFreteCliente.ContarConsultarTabelasAgrupamento(codigo);

            return lista;
        }

        private dynamic FormataPesquisaGrid(List<Dominio.Entidades.Embarcador.Frete.TabelaFreteCliente> lista)
        {
            return (from obj in lista
                    select new
                    {
                        obj.Codigo,
                        obj.CodigoIntegracao,
                        TabelaFrete = obj.TabelaFrete.Descricao,
                        Origem = obj.DescricaoOrigem,
                        Empresa = obj.Empresa?.Descricao ?? "",
                        Destino = obj.DescricaoDestino,
                        Vigencia = obj.DescricaoVigencia,
                        Ativo = obj.DescricaoAtivo
                    }).ToList();
        }

        private List<Dominio.Entidades.Embarcador.Frete.TabelaFreteCliente> ObterTabelaFreteSelecionados(Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Frete.TabelaFreteCliente repTabelaFreteCliente = new Repositorio.Embarcador.Frete.TabelaFreteCliente(unitOfWork);

            bool.TryParse(Request.Params("SelecionarTodos"), out bool todosSelecionados);
            List<Dominio.Entidades.Embarcador.Frete.TabelaFreteCliente> tabelasFrete = new List<Dominio.Entidades.Embarcador.Frete.TabelaFreteCliente>();

            if (todosSelecionados)
            {
                int totalRegistros = 0;

                tabelasFrete = ExecutaPesquisa(ref totalRegistros, "", "", 0, 0, unitOfWork);

                dynamic listaItensNaoSelecionados = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>((string)Request.Params("ItensNaoSelecionados"));
                foreach (var dybItenNaoSelecionado in listaItensNaoSelecionados)
                    tabelasFrete.Remove(new Dominio.Entidades.Embarcador.Frete.TabelaFreteCliente() { Codigo = (int)dybItenNaoSelecionado.Codigo });
            }
            else
            {
                dynamic listaItensSelecionados = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>((string)Request.Params("ItensSelecionados"));
                foreach (var dynItenSelecionado in listaItensSelecionados)
                    tabelasFrete.Add(repTabelaFreteCliente.BuscarPorCodigo((int)dynItenSelecionado.Codigo));
            }

            return tabelasFrete;
        }
        #endregion  
    }
}
