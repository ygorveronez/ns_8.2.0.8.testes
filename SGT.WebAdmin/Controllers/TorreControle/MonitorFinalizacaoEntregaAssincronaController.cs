using Dominio.Excecoes.Embarcador;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Dominio.ObjetosDeValor.Embarcador.TorreControle;
using Microsoft.AspNetCore.Mvc;
using SGT.WebAdmin.Models.Grid;
using SGTAdmin.Controllers;
using System.Linq.Dynamic.Core;

namespace SGT.WebAdmin.Controllers.Cargas.CargaEntregaEventoIntegracao
{
    [CustomAuthorize("TorreControle/MonitorFinalizacaoEntregaAssincrona")]
    public class MonitorFinalizacaoEntregaAssincronaController : BaseController
    {
        #region Construtores

        public MonitorFinalizacaoEntregaAssincronaController(Conexao conexao) : base(conexao) { }

        #endregion

        #region Métodos Globais

        [AllowAuthenticate]
        public async Task<IActionResult> Pesquisa(CancellationToken cancellationToken)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                Grid grid = await ObterGridPesquisa(unitOfWork, cancellationToken);

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
        public async Task<IActionResult> ExportarPesquisa(CancellationToken cancellationToken)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                var grid = await ObterGridPesquisa(unitOfWork, cancellationToken);
                byte[] arquivoBinario = grid.GerarExcel();
                if (arquivoBinario != null)
                    return Arquivo(arquivoBinario, "application/octet-stream", $"{grid.tituloExportacao}.{grid.extensaoCSV}");

                return new JsonpResult(false, "Ocorreu uma falha ao gerar arquivo.");
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao exportar.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> Reprocessar(CancellationToken cancellationToken)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            AdminMultisoftware.Repositorio.UnitOfWork unitOfWorkAdmin = new AdminMultisoftware.Repositorio.UnitOfWork(_conexao.AdminStringConexao);

            try
            {
                unitOfWork.Start();

                int codigo = Request.GetIntParam("Codigo");

                Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntrega repositorioCargaEntrega = new Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntrega(unitOfWork, cancellationToken);
                Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntregaFinalizacaoAssincrona repositorioCargaEntregaFinalizacaoAssincrona = new Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntregaFinalizacaoAssincrona(unitOfWork, cancellationToken);

                Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega cargaEntrega = await repositorioCargaEntrega.BuscarPorCodigoAsync(codigo, false);

                if (cargaEntrega == null)
                    return new JsonpResult(false, true, "Entrega não encontrada");

                if (cargaEntrega.CargaEntregaFinalizacaoAssincrona == null)
                    return new JsonpResult(false, true, "Registro de finalização não encontrado");

                Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaFinalizacaoAssincrona cargaEntregaFinalizacaoAssincrona = cargaEntrega.CargaEntregaFinalizacaoAssincrona;

                if (cargaEntregaFinalizacaoAssincrona.SituacaoProcessamento != SituacaoProcessamentoIntegracao.ErroProcessamento)
                    return new JsonpResult(false, true, "Apenas entregas com problema de processamento podem ser reprocessadas.");

                cargaEntregaFinalizacaoAssincrona.SituacaoProcessamento = SituacaoProcessamentoIntegracao.AguardandoProcessamento;

                await repositorioCargaEntregaFinalizacaoAssincrona.AtualizarAsync(cargaEntregaFinalizacaoAssincrona);

                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (ControllerException excecao)
            {
                unitOfWork.Rollback();
                return new JsonpResult(false, true, excecao.Message);
            }
            catch (Exception excecao)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao reenviar a integração.");
            }
            finally
            {
                unitOfWork.Dispose();
                unitOfWorkAdmin.Dispose();
            }
        }
        #endregion

        #region Métodos Privados

        private async Task<Models.Grid.Grid> ObterGridPesquisa(Repositorio.UnitOfWork unitOfWork, CancellationToken cancellationToken)
        {
            Models.Grid.Grid grid = new Models.Grid.Grid(Request) { header = new List<Models.Grid.Head>() };
            grid.AdicionarCabecalho("Codigo", false);
            grid.AdicionarCabecalho("Carga", "Carga", 2, Models.Grid.Align.left, false);
            grid.AdicionarCabecalho("Cliente", "Cliente", 2, Models.Grid.Align.left, false);
            grid.AdicionarCabecalho("Data de inclusão", "DataInclusao", 2, Models.Grid.Align.left, false);
            grid.AdicionarCabecalho("Situação processamento", "SituacaoProcessamento", 2, Models.Grid.Align.left, false);
            grid.AdicionarCabecalho("Detalhes processamento", "DetalhesProcessamento", 2, Models.Grid.Align.left, false);

            Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = grid.ObterParametrosConsulta();
            FiltroPesquisaConsultaCargaEntregaFinalizacaoAssincrona filtroPesquisa = ObterFiltrosPesquisa();

            Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntrega repCargaEntrega = new Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntrega(unitOfWork, cancellationToken);

            int totalRegistros = await repCargaEntrega.ContarConsultaCargaEntregaFinalizacaoAssincronaAsync(filtroPesquisa);

            List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega> listaEntregasFinalizacaoAssincronas = totalRegistros > 0 ? (await repCargaEntrega.ConsultarCargaEntregaFinalizacaoAssincronaAsync(filtroPesquisa, parametrosConsulta)) : new List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega>();

            var listaRetornar = (
                from entrega in listaEntregasFinalizacaoAssincronas
                select new
                {
                    entrega.Codigo,
                    Carga = entrega.Carga.CodigoCargaEmbarcador ?? string.Empty,
                    Cliente = entrega.Cliente.NomeCNPJ ?? string.Empty,
                    DataInclusao = entrega.CargaEntregaFinalizacaoAssincrona.DataInclusao.ToString(),
                    SituacaoProcessamento = entrega.CargaEntregaFinalizacaoAssincrona.SituacaoProcessamento.ObterDescricao(),
                    DetalhesProcessamento = entrega.CargaEntregaFinalizacaoAssincrona.DetalhesProcessamento,
                    DT_RowColor = entrega.CargaEntregaFinalizacaoAssincrona.SituacaoProcessamento.ObterCorLinha(),
                    DT_FontColor = entrega.CargaEntregaFinalizacaoAssincrona.SituacaoProcessamento.ObterCorFonte()
                }
            ).ToList();

            grid.AdicionaRows(listaRetornar);
            grid.setarQuantidadeTotal(totalRegistros);

            return grid;
        }

        private FiltroPesquisaConsultaCargaEntregaFinalizacaoAssincrona ObterFiltrosPesquisa()
        {
            return new FiltroPesquisaConsultaCargaEntregaFinalizacaoAssincrona()
            {
                CodigoCarga = Request.GetIntParam("Carga"),
                CodigoCliente = Request.GetLongParam("Cliente"),
                SituacaoProcessamento = Request.GetNullableEnumParam<SituacaoProcessamentoIntegracao>("SituacaoProcessamento"),
                DataFinalInclusaoProcessamento = Request.GetDateTimeParam("DataFinalInclusaoProcessamento"),
                DataInicialInclusaoProcessamento = Request.GetDateTimeParam("DataInicialInclusaoProcessamento"),
            };
        }

        #endregion
    }
}
