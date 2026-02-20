using Dominio.Excecoes.Embarcador;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Dominio.ObjetosDeValor.Embarcador.TorreControle;
using Microsoft.AspNetCore.Mvc;
using SGTAdmin.Controllers;
using System.Linq.Dynamic.Core;

namespace SGT.WebAdmin.Controllers.Cargas.CargaEntregaEventoIntegracao
{
    [CustomAuthorize("TorreControle/MonitorIntegracoesSuperApp")]
    public class MonitorIntegracoesSuperAppController : BaseController
    {
        #region Construtores

        public MonitorIntegracoesSuperAppController(Conexao conexao) : base(conexao) { }

        #endregion

        #region Métodos Globais

        [AllowAuthenticate]
        public async Task<IActionResult> Pesquisa(CancellationToken cancellationToken)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                return new JsonpResult(await ObterGridPesquisaAsync(unitOfWork, cancellationToken));
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao consultar.");
            }
            finally
            {
                await unitOfWork.DisposeAsync();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> ExportarPesquisa(CancellationToken cancellationToken)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                var grid = await ObterGridPesquisaAsync(unitOfWork, cancellationToken);
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
                await unitOfWork.DisposeAsync();
            }
        }

        public async Task<IActionResult> Reenviar(CancellationToken cancellationToken)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                await unitOfWork.StartAsync();

                int codigo = Request.GetIntParam("Codigo");

                Repositorio.Embarcador.SuperApp.IntegracaoSuperApp repIntegracoesSuperApp = new Repositorio.Embarcador.SuperApp.IntegracaoSuperApp(unitOfWork, cancellationToken);

                Dominio.Entidades.Embarcador.SuperApp.IntegracaoSuperApp integracaoSuperApp = await repIntegracoesSuperApp.BuscarPorCodigoAsync(codigo, auditavel: true);
                if (integracaoSuperApp == null)
                    return new JsonpResult(false, true, "Integração não encontrada.");

                if (integracaoSuperApp.SituacaoProcessamento != SituacaoProcessamentoIntegracao.ErroProcessamento)
                    return new JsonpResult(false, true, "Apenas integrações com problema de processamento podem ser reenviadas.");

                integracaoSuperApp.SituacaoProcessamento = SituacaoProcessamentoIntegracao.AguardandoProcessamento;

                Servicos.Auditoria.Auditoria.Auditar(Auditado, integracaoSuperApp, null, "Solicitou o reenvio da integração.", unitOfWork);

                await unitOfWork.CommitChangesAsync();

                return new JsonpResult(true);
            }
            catch (ControllerException excecao)
            {
                await unitOfWork.RollbackAsync();
                return new JsonpResult(false, true, excecao.Message);
            }
            catch (Exception excecao)
            {
                await unitOfWork.RollbackAsync();
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao reenviar a integração.");
            }
            finally
            {
                await unitOfWork.DisposeAsync();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> DownloadArquivosIntegracaoHistorico(CancellationToken cancellationToken)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                int codigo = Request.GetIntParam("Codigo");

                Repositorio.Embarcador.SuperApp.IntegracaoSuperApp repIntegracoesSuperApp = new Repositorio.Embarcador.SuperApp.IntegracaoSuperApp(unitOfWork, cancellationToken);

                Dominio.Entidades.Embarcador.SuperApp.IntegracaoSuperApp integracaoSuperApp = await repIntegracoesSuperApp.BuscarPorCodigoAsync(codigo, true);

                if (integracaoSuperApp == null)
                    return new JsonpResult(false, "Histórico de integração não encontrada.");

                if (!string.IsNullOrEmpty(integracaoSuperApp.StringJsonRequest) && !string.IsNullOrEmpty(integracaoSuperApp.StringJsonResponse))
                {
                    var arquivoRequest = Servicos.Embarcador.Integracao.ArquivoIntegracao.SalvarArquivoIntegracao(integracaoSuperApp.StringJsonRequest, "json", unitOfWork);
                    var arquivoRespose = Servicos.Embarcador.Integracao.ArquivoIntegracao.SalvarArquivoIntegracao(integracaoSuperApp.StringJsonResponse, "json", unitOfWork);

                    byte[] arquivoCompactado = Servicos.Embarcador.Integracao.ArquivoIntegracao.CriarZip(new List<Dominio.Entidades.Embarcador.Integracao.ArquivoIntegracao>() { arquivoRequest, arquivoRespose });
                    return Arquivo(arquivoCompactado, "application/zip", $"Arquivos da Carga {integracaoSuperApp.Carga.CodigoCargaEmbarcador} Evento {obterDescricaoTipoEvento(integracaoSuperApp)}.zip");
                }
                else
                {
                    if ((integracaoSuperApp.ArquivoRequisicao == null) && (integracaoSuperApp.ArquivoResposta == null))
                        return new JsonpResult(false, true, "Não há registros de arquivos salvos para este histórico de consulta.");

                    byte[] arquivoCompactado = Servicos.Embarcador.Integracao.ArquivoIntegracao.CriarZip(new List<Dominio.Entidades.Embarcador.Integracao.ArquivoIntegracao>() { integracaoSuperApp.ArquivoRequisicao, integracaoSuperApp.ArquivoResposta });
                    return Arquivo(arquivoCompactado, "application/zip", $"Arquivos da Carga {integracaoSuperApp.Carga.CodigoCargaEmbarcador} Evento {obterDescricaoTipoEvento(integracaoSuperApp)}.zip");
                }
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao realizar o download dos arquivos da integração.");
            }
            finally
            {
                await unitOfWork.DisposeAsync();
            }
        }

        #endregion

        #region Métodos Privados

        private async Task<Models.Grid.Grid> ObterGridPesquisaAsync(Repositorio.UnitOfWork unitOfWork, CancellationToken cancellationToken)
        {
            Models.Grid.Grid grid = new Models.Grid.Grid(Request) { header = new List<Models.Grid.Head>() };
            grid.AdicionarCabecalho("Codigo", false);
            grid.AdicionarCabecalho("Carga", "Carga", 2, Models.Grid.Align.left, true);
            grid.AdicionarCabecalho("Evento", "TipoEvento", 10, Models.Grid.Align.left, false);
            grid.AdicionarCabecalho("Integradora", "Integradora", 2, Models.Grid.Align.left, false);
            grid.AdicionarCabecalho("Data Recebimento", "DataRecebimento", 3, Models.Grid.Align.left, true);
            grid.AdicionarCabecalho("Tentativas", "NumeroTentativas", 2, Models.Grid.Align.left, true);
            grid.AdicionarCabecalho("Situação", "SituacaoProcessamento", 4, Models.Grid.Align.center, true);
            grid.AdicionarCabecalho("Retorno", "Retorno", 10, Models.Grid.Align.center, false);

            Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = grid.ObterParametrosConsulta();
            FiltroPesquisaConsultaMonitorIntegracoesSuperApp filtroPesquisa = ObterFiltrosPesquisa();

            Repositorio.Embarcador.SuperApp.IntegracaoSuperApp repIntegracoesSuperApp = new Repositorio.Embarcador.SuperApp.IntegracaoSuperApp(unitOfWork, cancellationToken);
            int totalRegistros = await repIntegracoesSuperApp.ContarConsultaAsync(filtroPesquisa);
            List<Dominio.Entidades.Embarcador.SuperApp.IntegracaoSuperApp> integracoesSuperApp = totalRegistros > 0 ? await repIntegracoesSuperApp.ConsultarAsync(filtroPesquisa, parametrosConsulta) : new List<Dominio.Entidades.Embarcador.SuperApp.IntegracaoSuperApp>();

            var listaRetornar = (
                from integracao in integracoesSuperApp
                select new
                {
                    integracao.Codigo,
                    Carga = integracao.Carga?.CodigoCargaEmbarcador ?? string.Empty,
                    TipoEvento = obterDescricaoTipoEvento(integracao),
                    Integradora = "Trizy",
                    DataRecebimento = integracao.DataRecebimento.ToString("dd/MM/yyyy HH:mm") ?? string.Empty,
                    NumeroTentativas = integracao.NumeroTentativas,
                    SituacaoProcessamento = integracao.SituacaoProcessamento.ObterDescricao() ?? string.Empty,
                    Retorno = integracao.DetalhesProcessamento,
                    DT_RowColor = integracao.SituacaoProcessamento.ObterCorLinha(),
                    DT_FontColor = integracao.SituacaoProcessamento.ObterCorFonte()
                }
            ).ToList();

            grid.AdicionaRows(listaRetornar);
            grid.setarQuantidadeTotal(totalRegistros);

            return grid;
        }

        private FiltroPesquisaConsultaMonitorIntegracoesSuperApp ObterFiltrosPesquisa()
        {
            return new FiltroPesquisaConsultaMonitorIntegracoesSuperApp()
            {
                CodigoCargaEmbarcador = Request.GetIntParam("Carga"),
                TipoEventoApp = Request.GetNullableEnumParam<TipoEventoApp>("TipoEventoApp"),
                SituacaoIntegracao = Request.GetNullableEnumParam<SituacaoProcessamentoIntegracao>("SituacaoProcessamento"),
                DataInicioRecebimento = Request.GetNullableDateTimeParam("DataInicioRecebimento"),
                DataFimRecebimento = Request.GetNullableDateTimeParam("DataFimRecebimento"),
                CodigoTransportador = Request.GetIntParam("Transportador")
            };
        }

        private string obterDescricaoTipoEvento(Dominio.Entidades.Embarcador.SuperApp.IntegracaoSuperApp integracao)
        {
            string descricaoTipoEvento = integracao.TipoEvento.ObterDescricao();

            if (integracao.TipoEvento == TipoEventoApp.EventsSubmit)
            {

                string jsonRequisicao = integracao.ArquivoRequisicao != null ? Servicos.Embarcador.Integracao.ArquivoIntegracao.RetornarArquivoTexto(integracao.ArquivoRequisicao) : integracao.StringJsonRequest;
                Dominio.ObjetosDeValor.WebService.Rest.Webhook.SuperApp.EventoEventsSubmit eventoEventsSubmit = Newtonsoft.Json.JsonConvert.DeserializeObject<Dominio.ObjetosDeValor.WebService.Rest.Webhook.SuperApp.EventoEventsSubmit>(jsonRequisicao);

                if (eventoEventsSubmit != null)
                {
                    switch (eventoEventsSubmit.Data.Event.Type)
                    {
                        case "START_TRAVEL":
                            descricaoTipoEvento += " - Início de Viagem";
                            break;
                        case "START_OPERATION":
                            descricaoTipoEvento += " - Início da ";
                            if (integracao.CargaEntrega != null) descricaoTipoEvento += integracao.CargaEntrega.Coleta ? "Coleta" : "Entrega";
                            else descricaoTipoEvento += "Coleta/Entrega";
                            break;
                        case "CUSTOM":
                            switch ((TipoCustomEventAppTrizy)eventoEventsSubmit.Data.Event.ExternalId.ToInt())
                            {
                                case TipoCustomEventAppTrizy.EstouIndo:
                                    descricaoTipoEvento += " - Estou Indo";
                                    break;
                                case TipoCustomEventAppTrizy.SolicitacaoDataeHoraCanhoto:
                                    descricaoTipoEvento += " - Solicitação Data e Hora Canhoto";
                                    break;
                            }
                            break;
                        case "END_OPERATION":
                            descricaoTipoEvento += " - Confirmar ";
                            if (integracao.CargaEntrega != null) descricaoTipoEvento += integracao.CargaEntrega.Coleta ? "Coleta" : "Entrega";
                            else descricaoTipoEvento += "Coleta/Entrega";
                            break;
                        default:
                            descricaoTipoEvento += " - " + eventoEventsSubmit.Data.Event.Type;
                            break;
                    }
                }
            }

            if (integracao.CargaEntrega != null)
                descricaoTipoEvento += $" ({integracao.CargaEntrega?.Cliente?.CPF_CNPJ_Formatado ?? string.Empty} - {integracao.CargaEntrega.Ordem + 1}/{(integracao.Carga ?? integracao.CargaEntrega.Carga).Entregas.Count})";

            return descricaoTipoEvento;
        }
        #endregion
    }
}
