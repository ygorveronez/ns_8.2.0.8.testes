using Microsoft.AspNetCore.Mvc;
using SGTAdmin.Controllers;
using System.Linq.Dynamic.Core;

namespace SGT.WebAdmin.Controllers.Pedidos
{
    [CustomAuthorize("Carga/ControleEntrega/CargaEntrega")]
    public class CargaEntregaController : BaseController
    {
		#region Construtores

		public CargaEntregaController(Conexao conexao) : base(conexao) { }

		#endregion

        [AllowAuthenticate]
        public async Task<IActionResult> Pesquisa()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                // Manipula grids
                Models.Grid.Grid grid = GridPesquisa();

                // Ordenacao da grid
                string propOrdenar = grid.header[grid.indiceColunaOrdena].data;
                PropOrdena(ref propOrdenar);

                // Busca Dados
                int totalRegistros = 0;
                dynamic lista = ExecutaPesquisa(ref totalRegistros, propOrdenar, grid.dirOrdena, grid.inicio, grid.limite, unitOfWork);

                // Seta valores na grid
                grid.AdicionaRows(lista);
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

        [AllowAuthenticate]
        public async Task<IActionResult> PesquisaColetas()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Models.Grid.Grid grid = new Models.Grid.Grid(Request)
                {
                    header = new List<Models.Grid.Head>()
                };

                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("NumeroReboques", false);
                grid.AdicionarCabecalho("ExigePlacaTracao", false);
                grid.AdicionarCabecalho("Descricao", false);
                grid.AdicionarCabecalho("Remetente", false);
                grid.AdicionarCabecalho("Destinatario", false);
                grid.AdicionarCabecalho("Carga", "CodigoCargaEmbarcador", 25, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("NumeroPedidos", "NumeroPedidos", 35, Models.Grid.Align.left, false, true, true);
                grid.AdicionarCabecalho("Remetentes", "Remetentes", 35, Models.Grid.Align.left, false, true, true);
                grid.AdicionarCabecalho("Destinatarios", "Destinatarios", 35, Models.Grid.Align.left, false, true, true);
                grid.AdicionarCabecalho("Situação", "Situacao", 25, Models.Grid.Align.center, true);

                Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntrega repositorioCargaEntrega = new Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntrega(unitOfWork);

                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = grid.ObterParametrosConsulta();
                Dominio.ObjetosDeValor.Embarcador.Carga.FiltroPesquisaCargaEntrega filtroPesquisa = ObterFiltrosPesquisaColeta();
                int totalRegistros = repositorioCargaEntrega.ContarConsultaColeta(filtroPesquisa);
                IList<Dominio.Relatorios.Embarcador.DataSource.Cargas.ControleEntrega.CargaEntregaColeta> coletas = totalRegistros > 0 ? repositorioCargaEntrega.ConsultarColeta(filtroPesquisa, parametrosConsulta) : new List<Dominio.Relatorios.Embarcador.DataSource.Cargas.ControleEntrega.CargaEntregaColeta>();

                grid.AdicionaRows(coletas);
                grid.setarQuantidadeTotal(totalRegistros);

                return new JsonpResult(grid);
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao consultar.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> BuscarPorCarga()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntrega repositorioCargaEntrega = new Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntrega(unitOfWork);
                List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega> listaEntregas = repositorioCargaEntrega.BuscarPorCarga(Request.GetIntParam("Carga"));

                return new JsonpResult(
                    from o in listaEntregas
                    select new
                    {
                        DT_RowId = o.Codigo,
                        o.Codigo,
                        Ordem = o.Ordem + 1,
                        Cliente = o.Cliente?.Descricao,
                        Cidade = o.Cliente?.Cidade,
                        Estado = o.Cliente?.Localidade?.Estado?.Sigla
                    });
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao buscar as entregas da carga.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> ReordenarEntregas()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                unitOfWork.Start();

                int novaOrdem = Request.GetIntParam("NovaOrdem") - 1;
                int codigoEntrega = Request.GetIntParam("Codigo");

                Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntrega repositorioCargaEntrega = new Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntrega(unitOfWork);
                Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega cargaEntrega = repositorioCargaEntrega.BuscarPorCodigoFetchCarga(codigoEntrega);

                if (novaOrdem > cargaEntrega.Ordem)
                    repositorioCargaEntrega.AtualizarOrdensInferiores(cargaEntrega.Carga.Codigo, novaOrdem, cargaEntrega.Ordem);
                else
                    repositorioCargaEntrega.AtualizarOrdensSuperiores(cargaEntrega.Carga.Codigo, novaOrdem, cargaEntrega.Ordem);

                cargaEntrega.Ordem = novaOrdem;
                repositorioCargaEntrega.Atualizar(cargaEntrega);

                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao reordenar as entregas.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        #region Métodos Privados

        private Dominio.ObjetosDeValor.Embarcador.Carga.FiltroPesquisaCargaEntrega ObterFiltrosPesquisaColeta()
        {
            return new Dominio.ObjetosDeValor.Embarcador.Carga.FiltroPesquisaCargaEntrega()
            {
                CodigoCargaEmbarcador = Request.GetStringParam("CodigoCargaEmbarcador"),
                NumeroPedidoEmbarcador = Request.GetStringParam("NumeroPedidoEmbarcador"),
                NumeroNotaFiscal = Request.GetIntParam("NumeroNotaFiscal"),
                CodigoTransportador = TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiCTe ? this.Empresa.Codigo : 0
            };
        }

        private Models.Grid.Grid GridPesquisa()
        {
            // Manipula grids
            Models.Grid.Grid grid = new Models.Grid.Grid(Request)
            {
                header = new List<Models.Grid.Head>()
            };

            // Cabecalhos grid
            grid.Prop("Codigo");
            grid.Prop("NumeroCTe");
            grid.Prop("Remetente");

            grid.AdicionarCabecalho("Código", "Codigo", 10, Models.Grid.Align.center, true);
            grid.AdicionarCabecalho("Notas Fiscais", "NotasFiscais", 10, Models.Grid.Align.left, true);
            grid.AdicionarCabecalho("Remetente", "Remetente", 15, Models.Grid.Align.left, false);

            return grid;
        }

        private dynamic ExecutaPesquisa(ref int totalRegistros, string propOrdenar, string dirOrdena, int inicio, int limite, Repositorio.UnitOfWork unitOfWork)
        {
            // Instancia repositorios
            Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntrega repCargaEntrega = new Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntrega(unitOfWork);
            Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntregaNotaFiscal repositorioCargaEntregaNotaFiscal = new Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntregaNotaFiscal(unitOfWork);

            // Dados do filtro
            int codigoCarga = Request.GetIntParam("Carga");
            int notaFiscal = Request.GetIntParam("NotaFiscal");

            // Consulta
            List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega> listaGrid = repCargaEntrega.Consultar(codigoCarga, propOrdenar, dirOrdena, inicio, limite, notaFiscal);
            List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaNotaFiscal> notasFiscais = new List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaNotaFiscal>();

            totalRegistros = repCargaEntrega.ContarConsulta(codigoCarga);

            if (totalRegistros > 0)
                notasFiscais = repositorioCargaEntregaNotaFiscal.BuscarPorCargaEntregas(listaGrid.Select(obj => obj.Codigo).ToList());

            var lista = from obj in listaGrid
                        select new
                        {
                            obj.Codigo,
                            Remetente = obj.Cliente.Nome,
                            NotasFiscais = string.Join(", ", (from nf in notasFiscais where nf.CargaEntrega.Codigo == obj.Codigo select nf.PedidoXMLNotaFiscal.XMLNotaFiscal.Numero).ToList()),
                            NumeroCTe = TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS && obj.NotasFiscais != null && obj.NotasFiscais.Count > 0 ? RetornarNumerosCTes(obj.NotasFiscais.Where(p => p.PedidoXMLNotaFiscal != null)?.Select(p => p.PedidoXMLNotaFiscal.Codigo).ToList() ?? null, unitOfWork) : "",
                        };

            return lista.ToList();
        }

        private void PropOrdena(ref string propOrdenar)
        {

        }

        private string RetornarNumerosCTes(List<int> codigosPedidoXMLNotaFiscal, Repositorio.UnitOfWork unitOfWork)
        {
            if (codigosPedidoXMLNotaFiscal == null || codigosPedidoXMLNotaFiscal.Count == 0)
                return "";
            Repositorio.Embarcador.Cargas.CargaPedidoXMLNotaFiscalCTe repCargaPedidoXMLNotaFiscalCTe = new Repositorio.Embarcador.Cargas.CargaPedidoXMLNotaFiscalCTe(unitOfWork);
            List<int> numerosCTes = repCargaPedidoXMLNotaFiscalCTe.NumerosCTesPorNotasFiscais(codigosPedidoXMLNotaFiscal);
            if (numerosCTes != null && numerosCTes.Count > 0)
                return string.Join(", ", numerosCTes);
            else
                return "";
        }

        #endregion
    }
}
