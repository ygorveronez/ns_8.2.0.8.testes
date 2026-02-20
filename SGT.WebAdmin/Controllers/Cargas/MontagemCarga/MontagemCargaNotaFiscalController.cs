using Microsoft.AspNetCore.Mvc;
using SGTAdmin.Controllers;

namespace SGT.WebAdmin.Controllers.Cargas.MontagemCarga
{
    [CustomAuthorize("Cargas/MontagemCarga", "Cargas/MontagemCargaMapa")]
    public class MontagemCargaNotaFiscalController : BaseController
    {
        #region Construtores

        public MontagemCargaNotaFiscalController(Conexao conexao) : base(conexao) { }

        #endregion

        #region Métodos Globais

        [AllowAuthenticate]
        public async Task<IActionResult> BuscarNotasFiscais()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                List<int> codigoPedidos = Request.GetListParam<int>("CodigoPedidos");

                (int totalRegistros, List<Dominio.ObjetosDeValor.Embarcador.Carga.MontagemCargaPedidoNotaFiscal> montagemCargaPedidoNotas) = ExecutarPesquisa(codigoPedidos, unitOfWork);

                var retorno = new
                {
                    QuantidadeRegistros = totalRegistros,
                    Registros = montagemCargaPedidoNotas,
                    Totalizadores = ObterTotalizadores(montagemCargaPedidoNotas)
                };

                return new JsonpResult(retorno);
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, Localization.Resources.Gerais.Geral.OcorreuFalhaAoConsultar);
            }
            finally
            {
                await unitOfWork.DisposeAsync();
            }
        }

        public async Task<IActionResult> ExportarPesquisa()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                Models.Grid.Grid grid = new Models.Grid.Grid()
                {
                    header = new List<Models.Grid.Head>()
                };

                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho(Localization.Resources.Cargas.MontagemCarga.NumeroNota, "NumeroNota", 10, Models.Grid.Align.left);
                grid.AdicionarCabecalho(Localization.Resources.Cargas.MontagemCarga.Serie, "Serie", 5, Models.Grid.Align.left);
                grid.AdicionarCabecalho(Localization.Resources.Cargas.MontagemCarga.ChaveNFe, "ChaveNota", 15, Models.Grid.Align.left);
                grid.AdicionarCabecalho(Localization.Resources.Cargas.MontagemCarga.DataNota, "DataNota", 10, Models.Grid.Align.left);
                grid.AdicionarCabecalho(Localization.Resources.Cargas.MontagemCarga.Remetente, "Remetente", 10, Models.Grid.Align.left);
                grid.AdicionarCabecalho(Localization.Resources.Cargas.MontagemCarga.Destinatario, "Destinatario", 10, Models.Grid.Align.left);
                grid.AdicionarCabecalho(Localization.Resources.Cargas.MontagemCarga.Origem, "Origem", 10, Models.Grid.Align.left);
                grid.AdicionarCabecalho(Localization.Resources.Cargas.MontagemCarga.Destino, "Destino", 10, Models.Grid.Align.left);
                grid.AdicionarCabecalho(Localization.Resources.Cargas.MontagemCarga.DiasPendentes, "DiasPendentes", 10, Models.Grid.Align.left);

                List<int> codigoPedidos = Request.GetListParam<int>("CodigoPedidos");

                (int totalRegistros, List<Dominio.ObjetosDeValor.Embarcador.Carga.MontagemCargaPedidoNotaFiscal> montagemCargaPedidoNotas) = ExecutarPesquisa(codigoPedidos, unitOfWork);

                if (totalRegistros == 0)
                    return new JsonpResult(false, true, "Nenhum registro encontrado!");

                grid.AdicionaRows(montagemCargaPedidoNotas);
                grid.setarQuantidadeTotal(totalRegistros);

                byte[] arquivoBinario = grid.GerarExcel();

                if (arquivoBinario == null)
                    return new JsonpResult(false, Localization.Resources.Gerais.Geral.OcorreuUmaFalhaAoGerarArquivo);

                return Arquivo(arquivoBinario, "application/octet-stream", "Notas Fiscais Montagem Carga." + grid.extensaoCSV);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, Localization.Resources.Gerais.Geral.OcorreuFalhaAoExportar);
            }
            finally
            {
                await unitOfWork.DisposeAsync();
            }
        }

        #endregion

        #region Métodos Privados

        private (int, List<Dominio.ObjetosDeValor.Embarcador.Carga.MontagemCargaPedidoNotaFiscal>) ExecutarPesquisa(List<int> codigoPedidos, Repositorio.UnitOfWork unitOfWork)
        {
            Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = new Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta()
            {
                DirecaoOrdenar = "desc",
                InicioRegistros = 0,
                LimiteRegistros = 0,
                PropriedadeOrdenar = "CodigoPedido"
            };

            Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal repositorioPedidoXMLNotaFiscal = new Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal(unitOfWork);

            int totalRegistros = repositorioPedidoXMLNotaFiscal.ContarConsultaParaMontagemCarga(codigoPedidos, parametrosConsulta);
            List<Dominio.ObjetosDeValor.Embarcador.Carga.MontagemCargaPedidoNotaFiscal> notasFiscais = (totalRegistros > 0) ? repositorioPedidoXMLNotaFiscal.ConsultarParaMontagemCarga(codigoPedidos, parametrosConsulta) : new List<Dominio.ObjetosDeValor.Embarcador.Carga.MontagemCargaPedidoNotaFiscal>();

            return (totalRegistros, notasFiscais);
        }

        private Dominio.ObjetosDeValor.Embarcador.Carga.MontagemCargaPedidoNotaFiscalTotalizadores ObterTotalizadores(List<Dominio.ObjetosDeValor.Embarcador.Carga.MontagemCargaPedidoNotaFiscal> notasFiscaisMontagem)
        {
            Dominio.ObjetosDeValor.Embarcador.Carga.MontagemCargaPedidoNotaFiscalTotalizadores totalizadores = new Dominio.ObjetosDeValor.Embarcador.Carga.MontagemCargaPedidoNotaFiscalTotalizadores();

            foreach (Dominio.ObjetosDeValor.Embarcador.Carga.MontagemCargaPedidoNotaFiscal notaFiscal in notasFiscaisMontagem)
            {
                totalizadores.PesoTotal += notaFiscal.Peso;
                totalizadores.PesoLiquidoTotal += notaFiscal.PesoLiquido;
                totalizadores.PesoSaldoRestante += notaFiscal.Peso;
                totalizadores.VolumeTotal += notaFiscal.Volumes;
                totalizadores.SaldoVolumesRestante += notaFiscal.Volumes;
            }

            return totalizadores;
        }

        #endregion
    }
}
