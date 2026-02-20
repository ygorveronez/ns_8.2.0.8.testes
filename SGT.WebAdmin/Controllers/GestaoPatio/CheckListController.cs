using Dominio.Excecoes.Embarcador;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Dominio.ObjetosDeValor.Relatorios;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Servicos.Extensions;
using SGTAdmin.Controllers;
using System.Net.Mail;

namespace SGT.WebAdmin.Controllers.GestaoPatio
{
    [CustomAuthorize(new string[] { "Imprimir" }, "GestaoPatio/CheckList", "GestaoPatio/FluxoPatio")]
    public class CheckListController : BaseController
    {
        #region Construtores

        public CheckListController(Conexao conexao) : base(conexao) { }

        #endregion

        #region Métodos Globais

        public async Task<IActionResult> Atualizar(CancellationToken cancellationToken)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                await unitOfWork.StartAsync(cancellationToken);

                int codigo = Request.GetIntParam("Codigo");

                Repositorio.Embarcador.GestaoPatio.CheckListCarga repositorioChecklist = new Repositorio.Embarcador.GestaoPatio.CheckListCarga(unitOfWork, cancellationToken);

                Dominio.Entidades.Embarcador.GestaoPatio.CheckListCarga checklist = repositorioChecklist.BuscarPorCodigo(codigo) ?? throw new ControllerException("Não foi possível encontrar o registro.");

                if (!checklist.Situacao.IsPermiteEdicao())
                    throw new ControllerException("Não é possível alterar o checklist na situação atual.");

                if (!checklist.EtapaCheckListLiberado)
                    throw new ControllerException("O checklist ainda não foi liberado para esta carga.");

                checklist.Aprovador = this.Usuario;
                checklist.DataLiberacao = DateTime.Now;

                Servicos.Embarcador.GestaoPatio.FluxoGestaoPatioConfiguracaoEtapa servicoFluxoGestaoPatioConfiguracaoEtapa = new Servicos.Embarcador.GestaoPatio.FluxoGestaoPatioConfiguracaoEtapa(unitOfWork);
                Dominio.Entidades.Embarcador.Filiais.SequenciaGestaoPatio sequenciaGestaoPatio = servicoFluxoGestaoPatioConfiguracaoEtapa.ObterSequenciaGestaoPatio(checklist.FluxoGestaoPatio);

                await PreencherChecklist(checklist, sequenciaGestaoPatio, unitOfWork, cancellationToken);

                if (!ValidarObrigatoriedade(checklist, unitOfWork))
                    throw new ControllerException("Preencha os campos obrigatórios da checklist.");

                Repositorio.Embarcador.Configuracoes.ConfiguracaoMotorista repositorioConfiguracaoMotorista = new Repositorio.Embarcador.Configuracoes.ConfiguracaoMotorista(unitOfWork, cancellationToken);
                Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoMotorista configuracaoMotorista = await repositorioConfiguracaoMotorista.BuscarPrimeiroRegistroAsync();

                List<Dominio.Entidades.Embarcador.Transportadores.MotoristaLicenca> licencas = checklist.Carga?.Motoristas?.FirstOrDefault()?.Licencas?.ToList() ?? new List<Dominio.Entidades.Embarcador.Transportadores.MotoristaLicenca>();

                bool licencaVinculadaValidada = true;

                if (configuracaoMotorista.BloquearChecklistMotoristaSemLicencaVinculada && licencas.Count <= 0)
                    licencaVinculadaValidada = false;

                bool licencaValida = true;

                if (licencas.Count > 0 && !licencas.Any(obj => obj.DataVencimento.HasValue && obj.DataVencimento.Value.Date >= DateTime.Now.Date) && licencas.Any(obj => obj.Licenca != null && obj.Licenca.BloquearCheckListComLicencaInvalida))
                    licencaValida = false;

                if (licencaValida && licencaVinculadaValidada)
                    AtualizarFluxoGestaoPatio(checklist, unitOfWork);

                checklist.LicencaInvalida = !licencaValida || !licencaVinculadaValidada;

                await repositorioChecklist.AtualizarAsync(checklist);

                if (sequenciaGestaoPatio != null && sequenciaGestaoPatio.CheckListGerarNovoPedidoAoTerminoFluxo)
                    GerarPedido(unitOfWork, sequenciaGestaoPatio, checklist);

                if (sequenciaGestaoPatio != null && sequenciaGestaoPatio.CheckListNotificarPorEmailReprovacao && checklist.Situacao == SituacaoCheckList.Rejeitado)
                    EnviarEmailReprovacaoCheckList(checklist, sequenciaGestaoPatio.CheckListEmails, unitOfWork);

                if (sequenciaGestaoPatio != null && sequenciaGestaoPatio.CheckListCancelarPatioAoReprovar && checklist.Situacao == SituacaoCheckList.Rejeitado)
                    CancelarFluxoPatioEGerarNovo(checklist, unitOfWork);

                await unitOfWork.CommitChangesAsync(cancellationToken);

                if (!licencaValida || !licencaVinculadaValidada)
                    return new JsonpResult(false, true, $"O fluxo não foi avançado pois não existe licença vigente ou nenhuma licença está vinculada ao motorista.");

                return new JsonpResult(true);
            }
            catch (BaseException excecao)
            {
                await unitOfWork.RollbackAsync(cancellationToken);
                return new JsonpResult(false, true, excecao.Message);
            }
            catch (Exception excecao)
            {
                await unitOfWork.RollbackAsync(cancellationToken);
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao atualizar dados.");
            }
            finally
            {
                await unitOfWork.DisposeAsync();
            }
        }

        [Obsolete("(NÂO UTILIZAR) Método adicionado para tratar apenas o problema da tarefa #58940")]
        public async Task<IActionResult> AtualizarPerguntas()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigoFluxoGestaoPatio = Request.GetIntParam("FluxoGestaoPatio");
                Repositorio.Embarcador.GestaoPatio.CheckListCarga repositorioChecklist = new Repositorio.Embarcador.GestaoPatio.CheckListCarga(unitOfWork);
                Dominio.Entidades.Embarcador.GestaoPatio.CheckListCarga checklist = repositorioChecklist.BuscarPorFluxoGestaoPatioEEtapaCheckList(codigoFluxoGestaoPatio, EtapaCheckList.Checklist);

                if (checklist == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                if (checklist.Situacao != SituacaoCheckList.Aberto)
                    return new JsonpResult(false, true, "O checklist não está mais aberto.");

                Servicos.Embarcador.GestaoPatio.CheckList servicoCheckList = new Servicos.Embarcador.GestaoPatio.CheckList(unitOfWork, Auditado);
                servicoCheckList.AtualizarPerguntas(checklist);

                return new JsonpResult(true);
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao atualizar as perguntas.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> BuscarAuditoria()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Repositorio.Embarcador.GestaoPatio.CheckListCargaPerguntaAuditoria repositorioCheckListAuditoria = new Repositorio.Embarcador.GestaoPatio.CheckListCargaPerguntaAuditoria(unitOfWork);

                List<Dominio.Entidades.Embarcador.GestaoPatio.CheckListCargaPerguntaAuditoria> listaAuditoria = repositorioCheckListAuditoria.BuscarPorChecklist(Request.GetIntParam("Codigo"));

                return new JsonpResult((from obj in listaAuditoria
                                        select new
                                        {
                                            obj.Codigo,
                                            Pergunta = obj.CheckListCargaPergunta.Descricao,
                                            Data = obj.Data.ToString("dd/MM/yyyy HH:mm"),
                                            obj.RespostaAntiga,
                                            obj.RespostaNova,
                                            obj.Observacao
                                        }).ToList());
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao buscar a auditoria do checklist.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> BuscarPorCodigo(CancellationToken cancellationToken)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigoChecklist = Request.GetIntParam("Codigo");
                int codigoFluxoGestaoPatio = Request.GetIntParam("FluxoGestaoPatio");
                EtapaCheckList etapaCheckList = Request.GetEnumParam<EtapaCheckList>("EtapaCheckList");

                Repositorio.Embarcador.GestaoPatio.CheckListCarga repositorioChecklist = new Repositorio.Embarcador.GestaoPatio.CheckListCarga(unitOfWork, cancellationToken);
                Repositorio.Embarcador.GestaoPatio.CheckListCargaAssinatura repositorioChecklistAssinatura = new Repositorio.Embarcador.GestaoPatio.CheckListCargaAssinatura(unitOfWork);
                Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repositorioConfiguracaoGeral = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork, cancellationToken);

                Dominio.Entidades.Embarcador.GestaoPatio.CheckListCarga checklist = null;
                Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configGeral = await repositorioConfiguracaoGeral.BuscarConfiguracaoPadraoAsync();

                if (codigoChecklist > 0)
                    checklist = repositorioChecklist.BuscarPorCodigo(codigoChecklist);
                else if (codigoFluxoGestaoPatio > 0)
                    checklist = repositorioChecklist.BuscarPorFluxoGestaoPatioEEtapaCheckList(codigoFluxoGestaoPatio, etapaCheckList);

                if (checklist == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                Servicos.Embarcador.GestaoPatio.FluxoGestaoPatioConfiguracaoEtapa servicoFluxoGestaoPatioConfiguracaoEtapa = new Servicos.Embarcador.GestaoPatio.FluxoGestaoPatioConfiguracaoEtapa(unitOfWork);
                Dominio.Entidades.Embarcador.Filiais.SequenciaGestaoPatio sequenciaGestaoPatio = servicoFluxoGestaoPatioConfiguracaoEtapa.ObterSequenciaGestaoPatio(checklist.FluxoGestaoPatio);
                List<string> retornosGR = new List<string>();

                if (checklist.FluxoGestaoPatio.CargaBase.IsCarga() && (checklist.Situacao == SituacaoCheckList.Aberto) && (checklist.FluxoGestaoPatio.SituacaoEtapaFluxoGestaoPatio == SituacaoEtapaFluxoGestaoPatio.Aguardando))
                {
                    if (checklist.FluxoGestaoPatio.Carga.Motoristas?.Count > 0)
                    {
                        Servicos.Embarcador.Transportadores.MotoristaGR servicoMotoristaGR = new Servicos.Embarcador.Transportadores.MotoristaGR(unitOfWork);

                        foreach (Dominio.Entidades.Usuario motorista in checklist.FluxoGestaoPatio.Carga.Motoristas)
                        {
                            Dominio.ObjetosDeValor.Embarcador.GR.RetornoGR retornoGRMotorista = servicoMotoristaGR.ValidarAdagio(checklist.FluxoGestaoPatio.Carga, motorista);

                            if (!(retornoGRMotorista?.Sucesso ?? true))
                                retornosGR.Add(retornoGRMotorista.Mensagem);
                        }
                    }

                    if (checklist.FluxoGestaoPatio.Carga.Veiculo != null)
                    {
                        Servicos.Embarcador.Veiculo.VeiculoGR servicoVeiculoGR = new Servicos.Embarcador.Veiculo.VeiculoGR(unitOfWork);
                        Dominio.ObjetosDeValor.Embarcador.GR.RetornoGR retornoGRVeiculo = servicoVeiculoGR.ValidarAdagio(checklist.FluxoGestaoPatio.Carga, checklist.FluxoGestaoPatio.Carga.Veiculo);

                        if (!(retornoGRVeiculo?.Sucesso ?? true))
                            retornosGR.Add(retornoGRVeiculo.Mensagem);
                    }
                }

                bool possuiAssinatura = (sequenciaGestaoPatio?.CheckListAssinaturaMotorista ?? false) || (sequenciaGestaoPatio?.CheckListAssinaturaCarregador ?? false) || (sequenciaGestaoPatio?.CheckListAssinaturaResponsavelAprovacao ?? false);
                List<Dominio.Entidades.Embarcador.GestaoPatio.CheckListCargaAssinatura> assinaturas = possuiAssinatura ? repositorioChecklistAssinatura.BuscarPorCheckList(checklist.Codigo) : new List<Dominio.Entidades.Embarcador.GestaoPatio.CheckListCargaAssinatura>();

                Repositorio.Embarcador.Cargas.CargaJanelaCarregamentoGuarita repositorioCargaGuarita = new Repositorio.Embarcador.Cargas.CargaJanelaCarregamentoGuarita(unitOfWork);
                Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoGuarita cargaGuarita = repositorioCargaGuarita.BuscarPorFluxoGestaoPatio(codigoFluxoGestaoPatio);

                bool edicaoRetroativa = checklist.EditadoRetroativo || (checklist.DataLiberacao.HasValue && (sequenciaGestaoPatio?.CheckListUtilizarVigencia ?? false));

                var checklistRetornar = new
                {
                    checklist.Codigo,
                    checklist.Situacao,
                    checklist.Reavaliada,
                    DataFimCheckList = checklist.FluxoGestaoPatio?.DataFimCheckList?.ToString("dd/MM/yyyy HH:mm") ?? "",
                    DataFimCheckListPrevista = checklist.FluxoGestaoPatio?.DataFimCheckListPrevista?.ToString("dd/MM/yyyy HH:mm") ?? "",
                    CheckListPermiteSalvarSemPreencher = sequenciaGestaoPatio?.CheckListPermiteSalvarSemPreencher ?? false,
                    CheckListCancelarPatioAoReprovar = sequenciaGestaoPatio?.CheckListCancelarPatioAoReprovar ?? false,
                    CheckListPermiteImpressaoApenasComCheckListFinalizada = sequenciaGestaoPatio?.CheckListPermiteImpressaoApenasComCheckListFinalizada ?? false,
                    CheckListNaoExigeObservacaoAoReprovar = sequenciaGestaoPatio?.CheckListNaoExigeObservacaoAoReprovar ?? false,
                    Pergunta = (
                        from pergunta in checklist.Perguntas
                        select new
                        {
                            pergunta.Codigo,
                            pergunta.Descricao,
                            QuantidadeLitrosExtraidos = pergunta.Descricao.Contains("Quantidade de Litros Extraídos") ? pergunta.Observacao = (cargaGuarita.QuantidadeLitros > 0 ? cargaGuarita.QuantidadeLitros.ToString() : pergunta.Observacao) : pergunta.Observacao ?? ""
                        }
                    ).ToList(),
                    Resumo = new
                    {
                        Carga = "",
                        Situacao = "",
                        Percurso = "",
                        Data = "",
                        Transportador = "",
                        Veiculo = "",
                        Motorista = ""
                    },
                    GrupoPerguntas = ObterGrupoPerguntas(checklist, edicaoRetroativa, unitOfWork),
                    ObservacoesGerais = checklist.Observacoes,
                    PermiteInformarAnexos = sequenciaGestaoPatio?.TipoChecklistImpressao != null || configGeral.PermitirAdicionarAnexosCheckListGestaoPatio,
                    EdicaoRetroativa = edicaoRetroativa,
                    RetornosGR = retornosGR,
                    PossuiAssinatura = possuiAssinatura,
                    CheckListAssinaturaMotorista = sequenciaGestaoPatio?.CheckListAssinaturaMotorista ?? false,
                    CheckListAssinaturaCarregador = sequenciaGestaoPatio?.CheckListAssinaturaCarregador ?? false,
                    CheckListAssinaturaResponsavelAprovacao = sequenciaGestaoPatio?.CheckListAssinaturaResponsavelAprovacao ?? false,
                    AssinaturaMotorista = ObterAssinaturaBase64(assinaturas.Find(o => o.TipoAssinatura == TipoAssinaturaCheckListCarga.Motorista), unitOfWork),
                    AssinaturaCarregador = ObterAssinaturaBase64(assinaturas.Find(o => o.TipoAssinatura == TipoAssinaturaCheckListCarga.Carregador), unitOfWork),
                    AssinaturaResponsavelAprovacao = ObterAssinaturaBase64(assinaturas.Find(o => o.TipoAssinatura == TipoAssinaturaCheckListCarga.ResponsavelAprovacao), unitOfWork),
                    Veiculo = checklist.Carga?.PlacasVeiculos ?? string.Empty,
                    Carga = checklist.Carga?.CodigoCargaEmbarcador ?? string.Empty,
                    Motorista = checklist.Carga?.NomeMotoristas ?? string.Empty,
                    Destinatario = checklist.Carga?.DadosSumarizados.Destinatarios ?? string.Empty,
                    Transportador = checklist.Carga?.Empresa?.RazaoSocial ?? string.Empty,
                };

                return new JsonpResult(checklistRetornar);
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

        public async Task<IActionResult> Imprimir()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigo = Request.GetIntParam("Codigo");

                Repositorio.Embarcador.GestaoPatio.CheckListCarga repositorioChecklist = new Repositorio.Embarcador.GestaoPatio.CheckListCarga(unitOfWork);
                Dominio.Entidades.Embarcador.GestaoPatio.CheckListCarga checklist = repositorioChecklist.BuscarPorCodigo(codigo);

                if (checklist == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                return Arquivo(ObterRelatorio(checklist, unitOfWork), "application/pdf", "Checklist.pdf");
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao gerar a impressão.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> Pesquisa()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Models.Grid.Grid grid = new Models.Grid.Grid(Request)
                {
                    header = new List<Models.Grid.Head>()
                };

                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("EtapaCheckList", false);
                grid.AdicionarCabecalho("Carga", "Carga", 10, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Cargas Agrupadas", "CodigosAgrupadosCarga", 10, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Data Abertura", "DataAbertura", 10, Models.Grid.Align.center, true);
                grid.AdicionarCabecalho("Situação", "Situacao", 10, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Data Liberação", "DataLiberacao", 10, Models.Grid.Align.center, true);
                grid.AdicionarCabecalho("Doca", "Doca", 10, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Etapa", "EtapaDescricao", 10, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Tempo Janela", "TempoJanela", 10, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Veículo", "Veiculo", 10, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Transportador", "Transportador", 10, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Modelo", "ModeloVeiculo", 10, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Tipo Operação", "TipoOperacao", 10, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Observação Janela", "ObservacaoFluxoPatio", 10, Models.Grid.Align.left, false);

                DateTime dataFinal = Request.GetDateTimeParam("DataFinal");
                DateTime dataInicial = Request.GetDateTimeParam("DataInicial");
                string carga = Request.GetStringParam("Carga");
                SituacaoCheckList? situacao = Request.GetNullableEnumParam<SituacaoCheckList>("Situacao");

                Servicos.Embarcador.Carga.Carga servicoCarga = new Servicos.Embarcador.Carga.Carga(unitOfWork);
                Servicos.Embarcador.Logistica.CargaJanelaCarregamentoConsulta servicoCargaJanelaCarregamentoConsulta = new Servicos.Embarcador.Logistica.CargaJanelaCarregamentoConsulta(unitOfWork);
                Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repositorioConfiguracaoEmbarcador = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork);
                Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoEmbarcador = repositorioConfiguracaoEmbarcador.BuscarConfiguracaoPadrao();
                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = grid.ObterParametrosConsulta(ObterPropriedadeOrdenar);
                Repositorio.Embarcador.GestaoPatio.CheckListCarga repositorioChecklist = new Repositorio.Embarcador.GestaoPatio.CheckListCarga(unitOfWork);
                int totalRegistros = repositorioChecklist.ContarConsulta(dataInicial, dataFinal, carga, situacao);
                List<Dominio.Entidades.Embarcador.GestaoPatio.CheckListCarga> listaChecklist = null;
                List<Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamento> listaCargaJanelaCarregamento = null;

                if (totalRegistros > 0)
                {
                    listaChecklist = repositorioChecklist.Consultar(dataInicial, dataFinal, carga, situacao, parametrosConsulta);
                    List<int> codigosCargas = (from o in listaChecklist where o.Carga != null select o.Carga.Codigo).Distinct().ToList();
                    listaCargaJanelaCarregamento = servicoCargaJanelaCarregamentoConsulta.ObterCargasJanelaCarregamentoPorCargas(codigosCargas);
                }
                else
                {
                    listaChecklist = new List<Dominio.Entidades.Embarcador.GestaoPatio.CheckListCarga>();
                    listaCargaJanelaCarregamento = new List<Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamento>();
                }

                var listaChecklistRetornar = (
                    from checklist in listaChecklist
                    select ObterChecklist(checklist, listaCargaJanelaCarregamento, configuracaoEmbarcador, unitOfWork)
                ).ToList();

                grid.AdicionaRows(listaChecklistRetornar);
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

        public async Task<IActionResult> ReavaliarChecklist()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                unitOfWork.Start();

                List<AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada> permissoesPersonalizadas = ObterPermissoesPersonalizadas("GestaoPatio/FluxoPatio");

                if (!permissoesPersonalizadas.Contains(AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada.FluxoGestaoPatio_PermiteReavaliarChecklist))
                    return new JsonpResult(false, true, "Você não possui permissões para executar esta ação.");

                int codigo = Request.GetIntParam("Codigo");
                Repositorio.Embarcador.GestaoPatio.CheckListCarga repositorioCheckList = new Repositorio.Embarcador.GestaoPatio.CheckListCarga(unitOfWork);
                Dominio.Entidades.Embarcador.GestaoPatio.CheckListCarga checkList = repositorioCheckList.BuscarPorCodigo(codigo);

                if (checkList == null)
                    return new JsonpResult(true, "O registro não foi encontrado.");

                if (checkList.FluxoGestaoPatio.GetEtapaAtual().EtapaFluxoGestaoPatio != EtapaFluxoGestaoPatio.CheckList)
                    return new JsonpResult(true, "Não é possível reavaliar o checklist na atual situação do fluxo de pátio.");

                if (checkList.Situacao != SituacaoCheckList.Rejeitado)
                    return new JsonpResult(true, "Não é possível reavaliar o checklist na situação atual.");

                Servicos.Embarcador.GestaoPatio.FluxoGestaoPatio servicoFluxoGestaoPatio = new Servicos.Embarcador.GestaoPatio.FluxoGestaoPatio(unitOfWork, Auditado, ConfiguracaoEmbarcador);
                servicoFluxoGestaoPatio.ReabrirFluxo(checkList.FluxoGestaoPatio);

                checkList.Reavaliada = true;

                repositorioCheckList.Atualizar(checkList);

                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (Exception excecao)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao reavaliar o checklist.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> Salvar(CancellationToken cancellationToken)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                await unitOfWork.StartAsync(cancellationToken);

                Repositorio.Embarcador.Filiais.ConfiguracaoGestaoPatio repositorioConfiguracaoGestaoPatio = new Repositorio.Embarcador.Filiais.ConfiguracaoGestaoPatio(unitOfWork, cancellationToken);
                Dominio.Entidades.Embarcador.Filiais.ConfiguracaoGestaoPatio configuracaoGestaoPatio = await repositorioConfiguracaoGestaoPatio.BuscarPrimeiroRegistroAsync();

                int codigo = Request.GetIntParam("Codigo");
                bool edicaoRetroativa = Request.GetBoolParam("EdicaoRetroativa");

                Repositorio.Embarcador.GestaoPatio.CheckListCarga repositorioChecklist = new Repositorio.Embarcador.GestaoPatio.CheckListCarga(unitOfWork, cancellationToken);
                Servicos.Embarcador.GestaoPatio.FluxoGestaoPatioConfiguracaoEtapa servicoFluxoGestaoPatioConfiguracaoEtapa = new Servicos.Embarcador.GestaoPatio.FluxoGestaoPatioConfiguracaoEtapa(unitOfWork);

                Dominio.Entidades.Embarcador.GestaoPatio.CheckListCarga checklist = await repositorioChecklist.BuscarPorCodigoAsync(codigo, false) ?? throw new ControllerException("Não foi possível encontrar o registro.");
                Dominio.Entidades.Embarcador.Filiais.SequenciaGestaoPatio sequenciaGestaoPatio = servicoFluxoGestaoPatioConfiguracaoEtapa.ObterSequenciaGestaoPatio(checklist.FluxoGestaoPatio);

                bool configGeralCheckListPermiteSalvarSemPreencher = configuracaoGestaoPatio?.CheckListPermiteSalvarSemPreencher ?? false;
                bool sequenciaGestaoPatioCheckListPermiteSalvarSemPreencher = sequenciaGestaoPatio?.CheckListPermiteSalvarSemPreencher ?? false;

                if (!(configGeralCheckListPermiteSalvarSemPreencher || sequenciaGestaoPatioCheckListPermiteSalvarSemPreencher) && !edicaoRetroativa)
                    throw new ControllerException("O checklist não pode ser atualizado sem realizar o preenchimento");

                if (!checklist.Situacao.IsPermiteEdicao() && !edicaoRetroativa)
                    throw new ControllerException("Não é possível alterar um checklist finalizado.");

                if (!checklist.EtapaCheckListLiberado && !(sequenciaGestaoPatio?.CheckListPermitePreencherCheckListAntesDeChegarNaEtapa ?? false))
                    throw new ControllerException("O checklist ainda não foi liberado para esta carga.");

                await PreencherChecklist(checklist, sequenciaGestaoPatio, unitOfWork, cancellationToken);

                if (checklist.EditadoRetroativo)
                {
                    if (checklist.Situacao == SituacaoCheckList.Rejeitado)
                        throw new ControllerException("O checklist não pode ser rejeitado ao realizar edição retroativa.");

                    checklist.DataLiberacao = DateTime.Now;
                }

                if (!checklist.EtapaCheckListLiberado)
                    checklist.Situacao = SituacaoCheckList.Aberto;
                else if (checklist.Situacao == SituacaoCheckList.Rejeitado)
                {
                    checklist.Aprovador = this.Usuario;
                    checklist.DataLiberacao = DateTime.Now;

                    Servicos.Embarcador.GestaoPatio.FluxoGestaoPatio servicoFluxoGestaoPatio = new Servicos.Embarcador.GestaoPatio.FluxoGestaoPatio(unitOfWork, Auditado, Cliente);

                    servicoFluxoGestaoPatio.RejeitarEtapa(checklist.FluxoGestaoPatio, checklist.EtapaFluxoGestaoPatio);
                    servicoFluxoGestaoPatio.Auditar(checklist.FluxoGestaoPatio, "Rejeitou o Checklist");
                    SalvarHistoricoChecklist(checklist, unitOfWork);
                }

                await repositorioChecklist.AtualizarAsync(checklist);

                await unitOfWork.CommitChangesAsync(cancellationToken);

                return new JsonpResult(true);
            }
            catch (BaseException excecao)
            {
                await unitOfWork.RollbackAsync(cancellationToken);
                return new JsonpResult(false, true, excecao.Message);
            }
            catch (Exception excecao)
            {
                await unitOfWork.RollbackAsync(cancellationToken);
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao salvar o checklist.");
            }
            finally
            {
                await unitOfWork.DisposeAsync();
            }
        }

        public async Task<IActionResult> VoltarEtapa()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigo = Request.GetIntParam("Codigo");
                Repositorio.Embarcador.GestaoPatio.CheckListCarga repositorioChecklist = new Repositorio.Embarcador.GestaoPatio.CheckListCarga(unitOfWork);
                Dominio.Entidades.Embarcador.GestaoPatio.CheckListCarga checklist = repositorioChecklist.BuscarPorCodigo(codigo);

                if (checklist == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                Servicos.Embarcador.GestaoPatio.FluxoGestaoPatio servicoFluxoGestaoPatio = new Servicos.Embarcador.GestaoPatio.FluxoGestaoPatio(unitOfWork, Auditado, Cliente);

                unitOfWork.Start();

                List<AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada> permissoesPersonalizadasFluxoPatio = ObterPermissoesPersonalizadas("GestaoPatio/FluxoPatio");
                servicoFluxoGestaoPatio.VoltarEtapa(checklist.FluxoGestaoPatio, checklist.EtapaFluxoGestaoPatio, this.Usuario, permissoesPersonalizadasFluxoPatio);

                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao voltar a etapa.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        #endregion

        #region Métodos Privados

        private void AtualizarFluxoGestaoPatio(Dominio.Entidades.Embarcador.GestaoPatio.CheckListCarga checklist, Repositorio.UnitOfWork unitOfWork)
        {
            Servicos.Embarcador.GestaoPatio.FluxoGestaoPatio servicoFluxoGestaoPatio = new Servicos.Embarcador.GestaoPatio.FluxoGestaoPatio(unitOfWork, Auditado, Cliente);

            if (checklist.Situacao == SituacaoCheckList.Rejeitado)
            {
                servicoFluxoGestaoPatio.RejeitarEtapa(checklist.FluxoGestaoPatio, checklist.EtapaFluxoGestaoPatio);
                servicoFluxoGestaoPatio.Auditar(checklist.FluxoGestaoPatio, "Rejeitou o Checklist");
            }
            else
            {
                checklist.Situacao = SituacaoCheckList.Finalizado;
                servicoFluxoGestaoPatio.LiberarProximaEtapa(checklist.FluxoGestaoPatio, checklist.EtapaFluxoGestaoPatio);
            }

            SalvarHistoricoChecklist(checklist, unitOfWork);
        }

        private dynamic ObterChecklist(Dominio.Entidades.Embarcador.GestaoPatio.CheckListCarga checklist, List<Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamento> listaCargaJanelaCarregamento, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoEmbarcador, Repositorio.UnitOfWork unitOfWork)
        {
            Servicos.Embarcador.Carga.Carga servicoCarga = new Servicos.Embarcador.Carga.Carga(unitOfWork);
            int codigoCargaFiltrarJanelaCarregamento = checklist.Carga?.Codigo ?? 0;
            Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamento cargaJanelaCarregamento = (from o in listaCargaJanelaCarregamento where o.Carga.Codigo == codigoCargaFiltrarJanelaCarregamento select o).FirstOrDefault();

            return new
            {
                checklist.Codigo,
                Carga = servicoCarga.ObterNumeroCarga(checklist.Carga, configuracaoEmbarcador),
                CodigosAgrupadosCarga = checklist.Carga == null ? "" : string.Join(", ", checklist.Carga.CodigosAgrupados),
                DataAbertura = checklist.DataAbertura.ToString("dd/MM/yyyy"),
                Situacao = checklist.DescricaoSituacao,
                DataLiberacao = checklist.DataLiberacao?.ToString("dd/MM/yyyy") ?? string.Empty,
                Doca = !string.IsNullOrWhiteSpace(checklist.Carga?.NumeroDocaEncosta) ? checklist.Carga?.NumeroDocaEncosta : checklist.Carga?.NumeroDoca ?? string.Empty,
                TempoJanela = cargaJanelaCarregamento?.InicioCarregamento.ToString("dd/MM/yyyy HH:mm") ?? string.Empty,
                Veiculo = checklist.Carga?.RetornarPlacas,
                Transportador = checklist.Carga?.Empresa?.Descricao ?? string.Empty,
                ModeloVeiculo = checklist.Carga?.ModeloVeicularCarga?.Descricao ?? string.Empty,
                TipoOperacao = checklist.Carga?.TipoOperacao?.Descricao ?? string.Empty,
                ObservacaoFluxoPatio = cargaJanelaCarregamento?.ObservacaoFluxoPatio ?? string.Empty,
                EtapaDescricao = checklist.EtapaCheckList.ObterDescricao(),
                checklist.EtapaCheckList
            };
        }

        private string ObterPropriedadeOrdenar(string propriedadeOrdenar)
        {
            if (propriedadeOrdenar == "Carga")
                return "CargaGuarita.CargaJanelaCarregamento.Carga.CodigoCargaEmbarcador";

            return propriedadeOrdenar;
        }

        private async Task PreencherChecklist(Dominio.Entidades.Embarcador.GestaoPatio.CheckListCarga checklist, Dominio.Entidades.Embarcador.Filiais.SequenciaGestaoPatio sequenciaGestaoPatio, Repositorio.UnitOfWork unitOfWork, CancellationToken cancellationToken)
        {
            Repositorio.Embarcador.GestaoPatio.CheckListCargaPergunta repositorioChecklistPergunta = new Repositorio.Embarcador.GestaoPatio.CheckListCargaPergunta(unitOfWork, cancellationToken);
            Repositorio.Embarcador.GestaoPatio.CheckListCargaPerguntaAlternativa repositorioCheckListCargaPerguntaAlternativa = new Repositorio.Embarcador.GestaoPatio.CheckListCargaPerguntaAlternativa(unitOfWork, cancellationToken);
            Repositorio.Embarcador.GestaoPatio.CheckListAnexo repositorioChecklistAnexo = new Repositorio.Embarcador.GestaoPatio.CheckListAnexo(unitOfWork);

            List<Dominio.Entidades.Embarcador.GestaoPatio.CheckListCargaPergunta> perguntas = await repositorioChecklistPergunta.BuscarPorCheckListAsync(checklist.Codigo);
            List<int> codigosPerguntas = (from o in perguntas select o.Codigo).ToList();
            List<Dominio.Entidades.Embarcador.GestaoPatio.CheckListCargaPerguntaAlternativa> alternativas = await repositorioCheckListCargaPerguntaAlternativa.BuscarPorPerguntasAsync(codigosPerguntas);
            List<dynamic> grupoPerguntas = JsonConvert.DeserializeObject<List<dynamic>>(Request.Params("GrupoPerguntas"));
            List<Dominio.ObjetosDeValor.Embarcador.GestaoPatio.AuditoriaCheckListPergunta> auditoriaInformacoes = new List<Dominio.ObjetosDeValor.Embarcador.GestaoPatio.AuditoriaCheckListPergunta>();

            checklist.Observacoes = Request.GetStringParam("ObservacoesGerais");
            checklist.EditadoRetroativo = Request.GetBoolParam("EdicaoRetroativa");

            foreach (dynamic grupoPergunta in grupoPerguntas)
            {
                foreach (dynamic perguntaPorCategoria in grupoPergunta.Perguntas)
                {
                    Dominio.Entidades.Embarcador.GestaoPatio.CheckListCargaPergunta pergunta = (from o in perguntas where o.Codigo == ((string)perguntaPorCategoria.Codigo).ToInt() select o).FirstOrDefault();

                    if (pergunta == null)
                        continue;

                    List<Dominio.Entidades.Embarcador.GestaoPatio.CheckListCargaPerguntaAlternativa> alternativasPorPergunta = (from o in alternativas where o.CheckListCargaPergunta.Codigo == pergunta.Codigo select o).ToList();

                    pergunta.Observacao = ((string)perguntaPorCategoria.Observacao).Trim();
                    string respostaAntiga = "";
                    string respostaNova = "";

                    switch (pergunta.Tipo)
                    {
                        case TipoOpcaoCheckList.Aprovacao:
                            respostaAntiga = pergunta.Resposta?.ObterDescricao();

                            pergunta.Resposta = ((string)perguntaPorCategoria.Resposta).ToNullableEnum<CheckListResposta>();

                            respostaNova = pergunta.Resposta?.ObterDescricao();

                            if (pergunta.Resposta == CheckListResposta.Reprovada)
                            {
                                if (pergunta.Observacao?.Length < 20 && !(sequenciaGestaoPatio?.CheckListNaoExigeObservacaoAoReprovar ?? true))
                                    throw new ControllerException("É necessário informar uma observação de no mínimo 20 caracteres para itens reprovados.");

                                checklist.Situacao = SituacaoCheckList.Rejeitado;
                            }

                            break;

                        case TipoOpcaoCheckList.Opcoes:
                            respostaAntiga = string.Join(", ", (from o in alternativasPorPergunta where o.Marcado == true select o.Descricao).ToList());
                            List<string> respostasMarcadasOpcoes = new List<string>();

                            foreach (dynamic alternativaPorPergunta in perguntaPorCategoria.Alternativas)
                            {
                                Dominio.Entidades.Embarcador.GestaoPatio.CheckListCargaPerguntaAlternativa alternativa = (from o in alternativas where o.Codigo == ((string)alternativaPorPergunta.Codigo).ToInt() select o).FirstOrDefault();

                                if (alternativa != null)
                                {
                                    bool? alternativaMarcada = ((string)alternativaPorPergunta.Marcado).ToNullableBool();

                                    if (alternativaMarcada.HasValue)
                                    {
                                        alternativa.Marcado = ((string)alternativaPorPergunta.Marcado).ToBool();

                                        if (alternativa.Marcado)
                                            respostasMarcadasOpcoes.Add(alternativa.Descricao);

                                        await repositorioCheckListCargaPerguntaAlternativa.AtualizarAsync(alternativa);
                                    }
                                }
                            }
                            respostaNova = string.Join(", ", respostasMarcadasOpcoes);
                            break;

                        case TipoOpcaoCheckList.Selecoes:
                            int codigoAlternativaSelecionada = ((string)perguntaPorCategoria.Resposta).ToInt();
                            List<string> respostasMarcadas = new List<string>();
                            respostaAntiga = string.Join(", ", (from o in alternativasPorPergunta where o.Marcado select o.Descricao).ToList());

                            foreach (dynamic alternativaPorPergunta in perguntaPorCategoria.Alternativas)
                            {
                                Dominio.Entidades.Embarcador.GestaoPatio.CheckListCargaPerguntaAlternativa alternativa = (from o in alternativas where o.Codigo == ((string)alternativaPorPergunta.Codigo).ToInt() select o).FirstOrDefault();

                                if (alternativa != null)
                                {
                                    alternativa.Marcado = (codigoAlternativaSelecionada == alternativa.Codigo);

                                    if (alternativa.Marcado)
                                        respostasMarcadas.Add(alternativa.Descricao);

                                    await repositorioCheckListCargaPerguntaAlternativa.AtualizarAsync(alternativa);
                                }
                            }

                            respostaNova = string.Join(", ", respostasMarcadas);
                            break;

                        case TipoOpcaoCheckList.SimNao:
                            respostaAntiga = pergunta.Resposta?.ObterDescricaoSimNao();

                            bool? respostaSimNao = ((string)perguntaPorCategoria.Resposta).ToNullableBool();
                            pergunta.Resposta = respostaSimNao.HasValue ? (CheckListResposta?)(respostaSimNao.Value ? CheckListResposta.Aprovada : CheckListResposta.Reprovada) : null;

                            respostaNova = pergunta.Resposta?.ObterDescricaoSimNao();
                            break;

                        case TipoOpcaoCheckList.Informativo:
                            respostaAntiga = pergunta.Observacao;

                            if (pergunta.TipoInformativo == TipoInformativo.TipoDecimal)
                                pergunta.Observacao = string.IsNullOrWhiteSpace(((string)perguntaPorCategoria.Resposta).Trim()) ? pergunta.Observacao.Replace(".", ",") : ((string)perguntaPorCategoria.Resposta).Trim();
                            else
                                pergunta.Observacao = string.IsNullOrWhiteSpace(((string)perguntaPorCategoria.Resposta).Trim()) ? pergunta.Observacao : ((string)perguntaPorCategoria.Resposta).Trim();

                            respostaNova = pergunta.Observacao;
                            break;
                    }

                    auditoriaInformacoes.Add(new Dominio.ObjetosDeValor.Embarcador.GestaoPatio.AuditoriaCheckListPergunta
                    {
                        Pergunta = pergunta,
                        Observacao = pergunta.Observacao,
                        RespostaAntiga = respostaAntiga ?? "",
                        RespostaNova = respostaNova ?? ""
                    });

                    await repositorioChecklistPergunta.AtualizarAsync(pergunta);
                }
            }

            List<Dominio.Entidades.Embarcador.GestaoPatio.CheckListAnexo> checkListAnexos = await repositorioChecklistAnexo.BuscarPorCheckListAsync(checklist.Codigo);

            if ((sequenciaGestaoPatio?.CheckListExigirAnexo ?? false) && checkListAnexos?.Count == 0)
                throw new ControllerException("Configuração exige anexos para finalizar a etapa.");

            if (checklist.Reavaliada)
                SalvarAuditoriaCheckListPerguntas(auditoriaInformacoes, unitOfWork);

            SalvarAssinaturasCheckList(checklist, unitOfWork);

            if (sequenciaGestaoPatio != null && sequenciaGestaoPatio.CheckListUtilizarVigencia)
                await VincularVigenciaCheckList(checklist, unitOfWork, cancellationToken);
        }

        private List<dynamic> ObterGrupoPerguntas(Dominio.Entidades.Embarcador.GestaoPatio.CheckListCarga checklist, bool edicaoRetroativa, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.GestaoPatio.CheckListCargaPergunta repositorioCheckListCargaPergunta = new Repositorio.Embarcador.GestaoPatio.CheckListCargaPergunta(unitOfWork);
            List<Dominio.Entidades.Embarcador.GestaoPatio.CheckListCargaPergunta> perguntas = repositorioCheckListCargaPergunta.BuscarPorCheckList(checklist.Codigo);
            List<int> codigosPerguntas = (from o in perguntas select o.Codigo).ToList();

            Repositorio.Embarcador.GestaoPatio.CheckListCargaPerguntaAlternativa repositorioCheckListCargaPerguntaAlternativa = new Repositorio.Embarcador.GestaoPatio.CheckListCargaPerguntaAlternativa(unitOfWork);
            List<Dominio.Entidades.Embarcador.GestaoPatio.CheckListCargaPerguntaAlternativa> alternativas = repositorioCheckListCargaPerguntaAlternativa.BuscarPorPerguntas(codigosPerguntas);

            List<CategoriaOpcaoCheckList> categorias = (from o in perguntas select o.Categoria).Distinct().ToList();
            bool edicaoPerguntaHabilitada = edicaoRetroativa || (checklist.Situacao.IsPermiteEdicao() && (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiEmbarcador || TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS));
            List<dynamic> categoriasRetornar = new List<dynamic>();

            foreach (CategoriaOpcaoCheckList categoria in categorias)
            {
                List<Dominio.Entidades.Embarcador.GestaoPatio.CheckListCargaPergunta> perguntasPorCategoria = (from o in perguntas where o.Categoria == categoria select o).ToList();
                List<SubCategoriaOpcaoCheckList> subcategorias = perguntasPorCategoria.Select(o => o.Subcategoria).Distinct().ToList();
                List<dynamic> perguntasRetornar = new List<dynamic>();

                foreach (SubCategoriaOpcaoCheckList subcategoria in subcategorias)
                {
                    List<Dominio.Entidades.Embarcador.GestaoPatio.CheckListCargaPergunta> perguntasPorSubategoria = perguntasPorCategoria.Where(o => o.Subcategoria == subcategoria).ToList();

                    foreach (Dominio.Entidades.Embarcador.GestaoPatio.CheckListCargaPergunta pergunta in perguntasPorSubategoria)
                    {
                        List<Dominio.Entidades.Embarcador.GestaoPatio.CheckListCargaPerguntaAlternativa> alternativasPorPergunta = (from o in alternativas where o.CheckListCargaPergunta.Codigo == pergunta.Codigo select o).ToList();
                        List<dynamic> alternativasRetornar = new List<dynamic>();
                        string resposta = "";

                        switch (pergunta.Tipo)
                        {
                            case TipoOpcaoCheckList.Aprovacao:
                                alternativasRetornar.Add(new { Codigo = (int)CheckListResposta.Aprovada, Descricao = CheckListResposta.Aprovada.ObterDescricao() });
                                alternativasRetornar.Add(new { Codigo = (int)CheckListResposta.Reprovada, Descricao = CheckListResposta.Reprovada.ObterDescricao() });
                                resposta = pergunta.Resposta.HasValue ? ((int)pergunta.Resposta.Value).ToString() : "";
                                break;

                            case TipoOpcaoCheckList.Opcoes:
                                foreach (Dominio.Entidades.Embarcador.GestaoPatio.CheckListCargaPerguntaAlternativa alternativa in alternativasPorPergunta)
                                {
                                    alternativasRetornar.Add(new
                                    {
                                        alternativa.Codigo,
                                        alternativa.Descricao,
                                        alternativa.Marcado,
                                        alternativa.OpcaoImpeditiva
                                    });
                                }
                                break;

                            case TipoOpcaoCheckList.Selecoes:
                                foreach (Dominio.Entidades.Embarcador.GestaoPatio.CheckListCargaPerguntaAlternativa alternativa in alternativasPorPergunta)
                                {
                                    alternativasRetornar.Add(new
                                    {
                                        alternativa.Codigo,
                                        alternativa.Descricao,
                                        alternativa.OpcaoImpeditiva
                                    });

                                    if (alternativa.Marcado)
                                        resposta = alternativa.Codigo.ToString();
                                }
                                break;

                            case TipoOpcaoCheckList.SimNao:
                                resposta = pergunta.Resposta.HasValue ? (pergunta.Resposta.Value == CheckListResposta.Aprovada ? "true" : "false") : "";
                                break;

                            case TipoOpcaoCheckList.Informativo:
                                resposta = ObterRespostaPorRelacaoPerguntaInformativa(pergunta, perguntas, unitOfWork);
                                break;
                        }

                        perguntasRetornar.Add(new
                        {
                            pergunta.Codigo,
                            Descricao = pergunta.DescricaoComObrigatoriedade,
                            Observacao = pergunta.TipoInformativo == TipoInformativo.TipoDecimal ? (dynamic)resposta.ToDecimal() : resposta,
                            pergunta.Tipo,
                            pergunta.TipoInformativo,
                            pergunta.Obrigatorio,
                            RespostaImpeditiva = pergunta.RespostaImpeditiva,
                            Enable = edicaoPerguntaHabilitada,
                            Alternativas = alternativasRetornar,
                            Resposta = resposta,
                            TagIntegracao = pergunta.TagIntegracao ?? string.Empty,
                            TipoRelacao = pergunta.RelacaoCampo?.CheckListOpcaoRelacaoCampo
                        });
                    }

                    categoriasRetornar.Add(new
                    {
                        Descricao = ObterDescricaoPorSubcategoria(checklist, categoria, subcategoria),
                        Perguntas = perguntasRetornar
                    });
                }
            }

            return categoriasRetornar;
        }

        private byte[] ObterRelatorio(Dominio.Entidades.Embarcador.GestaoPatio.CheckListCarga checklist, Repositorio.UnitOfWork unitOfWork)
        {
            return ReportRequest.WithType(ReportType.CheckList)
                .WithExecutionType(ExecutionType.Sync)
                .AddExtraData("CodigoCheckList", checklist.Codigo)
                .CallReport()
                .GetContentFile();
        }

        private bool ValidarObrigatoriedade(Dominio.Entidades.Embarcador.GestaoPatio.CheckListCarga checklist, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.GestaoPatio.CheckListCargaPergunta repositorioChecklistPergunta = new Repositorio.Embarcador.GestaoPatio.CheckListCargaPergunta(unitOfWork);
            List<Dominio.Entidades.Embarcador.GestaoPatio.CheckListCargaPergunta> perguntas = repositorioChecklistPergunta.BuscarPerguntasObrigatoriasPorCheckList(checklist.Codigo);

            foreach (Dominio.Entidades.Embarcador.GestaoPatio.CheckListCargaPergunta pergunta in perguntas)
            {
                switch (pergunta.Tipo)
                {
                    case TipoOpcaoCheckList.Informativo:
                        if (!ValidarRespostaPorTipoInformativo(pergunta.TipoInformativo, pergunta.Observacao))
                            return false;
                        break;
                }
            }

            return true;
        }

        private bool ValidarRespostaPorTipoInformativo(TipoInformativo tipo, string resposta)
        {
            switch (tipo)
            {
                case TipoInformativo.TipoDecimal:
                    if (resposta.ToDecimal() <= 0)
                        return false;
                    break;
                case TipoInformativo.TipoTexto:
                case TipoInformativo.TipoData:
                case TipoInformativo.TipoDataHora:
                case TipoInformativo.TipoHora:
                    if (string.IsNullOrWhiteSpace(resposta))
                        return false;
                    break;
            }

            return true;
        }

        private void EnviarEmailReprovacaoCheckList(Dominio.Entidades.Embarcador.GestaoPatio.CheckListCarga checklist, string emails, Repositorio.UnitOfWork unitOfWork)
        {
            MemoryStream stream = new MemoryStream(ObterRelatorio(checklist, unitOfWork));
            Attachment attachment = new Attachment(stream, "Checklist.pdf", "application/pdf");

            Servicos.Email servicoEmail = new Servicos.Email(unitOfWork);

            List<string> listaEmails = emails.Split(';').ToList();

            string assunto = $"Checklist carga {checklist.Carga.CodigoCargaEmbarcador} reprovado";
            string body = $@"Prezado, o checklist da carga {checklist.Carga.CodigoCargaEmbarcador} ({checklist.Carga?.Pedidos?.FirstOrDefault()?.Origem?.DescricaoCidadeEstado} até {checklist.Carga?.Pedidos?.LastOrDefault()?.Destino?.DescricaoCidadeEstado}), foi reprovado para a placa {checklist.Carga.Veiculo?.Placa_Formatada}, motorista {checklist.Carga.Motoristas?.FirstOrDefault()?.Nome}, transportador {checklist.Carga.Empresa?.Descricao}.
                            Em anexo os detalhes do checklist.";
            servicoEmail.EnviarEmail("", "", "", "", "", "", assunto, body, "", new List<Attachment>() { attachment }, "", false, "", 587, unitOfWork, 0, true, listaEmails);
        }

        private void CancelarFluxoPatioEGerarNovo(Dominio.Entidades.Embarcador.GestaoPatio.CheckListCarga checklist, Repositorio.UnitOfWork unitOfWork)
        {
            Servicos.Embarcador.GestaoPatio.FluxoGestaoPatio servicoFluxoGestaoPatio = new Servicos.Embarcador.GestaoPatio.FluxoGestaoPatio(unitOfWork, Auditado, Cliente);
            Dominio.ObjetosDeValor.Embarcador.GestaoPatio.FluxoGestaoPatioDadosReiniciar fluxoGestaoPatioDadosReiniciar = new Dominio.ObjetosDeValor.Embarcador.GestaoPatio.FluxoGestaoPatioDadosReiniciar()
            {
                FluxoGestaoPatio = checklist.FluxoGestaoPatio,
                Motivo = "Fluxo cancelado por rejeição do checklist.",
                RemoverDadosTransporte = true
            };

            servicoFluxoGestaoPatio.Reiniciar(fluxoGestaoPatioDadosReiniciar, TipoServicoMultisoftware);
        }

        private string ObterRespostaPorRelacaoPerguntaInformativa(Dominio.Entidades.Embarcador.GestaoPatio.CheckListCargaPergunta pergunta, List<Dominio.Entidades.Embarcador.GestaoPatio.CheckListCargaPergunta> perguntas, Repositorio.UnitOfWork unitOfWork)
        {
            if (pergunta.RelacaoCampo == null)
                return pergunta.Observacao;

            if (!string.IsNullOrWhiteSpace(pergunta.Observacao))
                return pergunta.Observacao;

            Repositorio.Embarcador.Cargas.CargaJanelaCarregamentoGuarita repositorioGuarita = new Repositorio.Embarcador.Cargas.CargaJanelaCarregamentoGuarita(unitOfWork);
            Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoGuarita guarita = repositorioGuarita.BuscarPorCarga(pergunta.CheckListCarga.CargaBase.Codigo);

            switch (pergunta.RelacaoCampo.CheckListOpcaoRelacaoCampo)
            {
                case CheckListOpcaoRelacaoCampo.LoteInternoUm:
                    return guarita?.LoteInterno ?? string.Empty;
                case CheckListOpcaoRelacaoCampo.LoteInternoDois:
                    return guarita?.LoteInternoDois ?? string.Empty;
                case CheckListOpcaoRelacaoCampo.PesoLiquidoPosPerdas:
                    return (((decimal)1.0 - ((guarita?.PorcentagemPerda ?? 1) / 100)) * ((guarita?.PesagemInicial ?? 1) - (guarita?.PesagemFinal ?? 1))).ToString("n2");
                case CheckListOpcaoRelacaoCampo.ResultadoRendimentoLaranja:
                    return ObterResultadoRendimentoLaranja(perguntas);
                default:
                    return pergunta.Observacao;
            }
        }

        private string ObterResultadoRendimentoLaranja(List<Dominio.Entidades.Embarcador.GestaoPatio.CheckListCargaPergunta> perguntas)
        {
            List<string> pesoTotalExtraidoFruta = perguntas.Where(x => !string.IsNullOrWhiteSpace(x.TagIntegracao) && x.TagIntegracao == "PesoTotalExtraidoFruta").Select(x => x.Observacao).ToList();
            List<string> pesoCorretoComRefugo = perguntas.Where(x => !string.IsNullOrWhiteSpace(x.TagIntegracao) && x.TagIntegracao == "PesoCorretoComRefugo").Select(x => x.Observacao).ToList();

            if (pesoTotalExtraidoFruta.Count == 0 || pesoCorretoComRefugo.Count == 0)
                return string.Empty;

            double totalPesoTotalExtraidoFruta = pesoTotalExtraidoFruta.Select(s => double.TryParse(s, out double valor) ? valor : 0).Sum();
            double totalPesoCorretoComRefugo = pesoCorretoComRefugo.Select(s => double.TryParse(s, out double valor) ? valor : 0).Sum();

            if (totalPesoTotalExtraidoFruta == 0 || totalPesoCorretoComRefugo == 0)
                return string.Empty;

            return Math.Round(totalPesoTotalExtraidoFruta / totalPesoCorretoComRefugo, 6).ToString();
        }

        private string ObterDescricaoPorSubcategoria(Dominio.Entidades.Embarcador.GestaoPatio.CheckListCarga checklist, CategoriaOpcaoCheckList categoria, SubCategoriaOpcaoCheckList subcategoria)
        {
            if (subcategoria == SubCategoriaOpcaoCheckList.NaoDefinido)
                return categoria.ObterDescricao();

            if (subcategoria == SubCategoriaOpcaoCheckList.Reboque)
                return $"Reboque {checklist.CargaBase.VeiculosVinculados?.ElementAtOrDefault(0)?.Placa_Formatada}";

            if (subcategoria == SubCategoriaOpcaoCheckList.SegundoReboque)
                return $"Segundo Reboque {checklist.CargaBase.VeiculosVinculados?.ElementAtOrDefault(1)?.Placa_Formatada}";

            return string.Empty;
        }

        private void SalvarAuditoriaCheckListPerguntas(List<Dominio.ObjetosDeValor.Embarcador.GestaoPatio.AuditoriaCheckListPergunta> auditoriaInformacoes, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.GestaoPatio.CheckListCargaPerguntaAuditoria repositorioCheckListAuditoria = new Repositorio.Embarcador.GestaoPatio.CheckListCargaPerguntaAuditoria(unitOfWork);

            foreach (Dominio.ObjetosDeValor.Embarcador.GestaoPatio.AuditoriaCheckListPergunta auditoriaInformacao in auditoriaInformacoes)
            {
                if (auditoriaInformacao.RespostaAntiga.Equals(auditoriaInformacao.RespostaNova))
                    continue;

                Dominio.Entidades.Embarcador.GestaoPatio.CheckListCargaPerguntaAuditoria auditoriaPergunta = new Dominio.Entidades.Embarcador.GestaoPatio.CheckListCargaPerguntaAuditoria()
                {
                    CheckListCargaPergunta = auditoriaInformacao.Pergunta,
                    Data = DateTime.Now,
                    Observacao = auditoriaInformacao.Pergunta.Tipo == TipoOpcaoCheckList.Aprovacao ? auditoriaInformacao.Observacao : "",
                    RespostaNova = auditoriaInformacao.RespostaNova,
                    RespostaAntiga = auditoriaInformacao.RespostaAntiga
                };

                repositorioCheckListAuditoria.Inserir(auditoriaPergunta);
            }
        }

        private void SalvarAssinaturasCheckList(Dominio.Entidades.Embarcador.GestaoPatio.CheckListCarga checklist, Repositorio.UnitOfWork unitOfWork)
        {
            Servicos.Embarcador.GestaoPatio.CheckListAssinatura servicoCheckListAssinatura = new Servicos.Embarcador.GestaoPatio.CheckListAssinatura(unitOfWork);

            string assinaturaMotorista = Request.GetStringParam("AssinaturaMotorista");
            string assinaturaCarregador = Request.GetStringParam("AssinaturaCarregador");
            string assinaturaResponsavelAprovacao = Request.GetStringParam("AssinaturaResponsavelAprovacao");

            servicoCheckListAssinatura.DeletarAssinaturas(checklist);

            if (!string.IsNullOrWhiteSpace(assinaturaMotorista))
                servicoCheckListAssinatura.ArmazenarVinculoAssinatura(checklist, assinaturaMotorista, TipoAssinaturaCheckListCarga.Motorista);

            if (!string.IsNullOrWhiteSpace(assinaturaCarregador))
                servicoCheckListAssinatura.ArmazenarVinculoAssinatura(checklist, assinaturaCarregador, TipoAssinaturaCheckListCarga.Carregador);

            if (!string.IsNullOrWhiteSpace(assinaturaResponsavelAprovacao))
                servicoCheckListAssinatura.ArmazenarVinculoAssinatura(checklist, assinaturaResponsavelAprovacao, TipoAssinaturaCheckListCarga.ResponsavelAprovacao);
        }

        private void SalvarHistoricoChecklist(Dominio.Entidades.Embarcador.GestaoPatio.CheckListCarga checklist, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.GestaoPatio.CheckListCargaHistorico repositorioCheckListCargaHistorico = new Repositorio.Embarcador.GestaoPatio.CheckListCargaHistorico(unitOfWork);

            Dominio.Entidades.Embarcador.GestaoPatio.CheckListCargaHistorico checklistHistorico = new Dominio.Entidades.Embarcador.GestaoPatio.CheckListCargaHistorico
            {
                ChecklistCarga = checklist,
                Situacao = checklist.Situacao,
                Usuario = this.Usuario,
                Observacao = checklist.Observacoes,
                Data = checklist.DataLiberacao ?? DateTime.Now
            };

            repositorioCheckListCargaHistorico.Inserir(checklistHistorico);
        }

        private string ObterCaminhoArquivoAssinatura(Repositorio.UnitOfWork unitOfWork)
        {
            return Utilidades.Directory.CriarCaminhoArquivos(new string[] { Servicos.Embarcador.Configuracoes.ConfigurationInstance.GetInstance(unitOfWork)?.ObterConfiguracaoArquivo().CaminhoArquivos, "Anexos", "CheckListCarga", "Assinaturas" });
        }

        public string ObterAssinaturaBase64(Dominio.Entidades.Embarcador.GestaoPatio.CheckListCargaAssinatura assinatura, Repositorio.UnitOfWork unitOfWork)
        {
            if (assinatura == null)
                return "";

            string caminho = ObterCaminhoArquivoAssinatura(unitOfWork);
            string nomeArquivo = Utilidades.IO.FileStorageService.Storage.Combine(caminho, $"{assinatura.GuidArquivo}.png");

            if (!Utilidades.IO.FileStorageService.Storage.Exists(nomeArquivo))
                return "";

            byte[] imageArray = Utilidades.IO.FileStorageService.Storage.ReadAllBytes(nomeArquivo);
            string base64ImageRepresentation = Convert.ToBase64String(imageArray);

            return "data:image/png;base64," + base64ImageRepresentation;
        }

        private void GerarPedido(Repositorio.UnitOfWork unitOfWork, Dominio.Entidades.Embarcador.Filiais.SequenciaGestaoPatio sequenciaGestaoPatio, Dominio.Entidades.Embarcador.GestaoPatio.CheckListCarga checklist)
        {

            Repositorio.Embarcador.Pedidos.Pedido repositorioPedido = new Repositorio.Embarcador.Pedidos.Pedido(unitOfWork);

            Repositorio.Embarcador.Cargas.CargaJanelaCarregamentoGuarita repositorioCargaGuarita = new Repositorio.Embarcador.Cargas.CargaJanelaCarregamentoGuarita(unitOfWork);
            Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoGuarita cargaGuarita = repositorioCargaGuarita.BuscarPorCarga(checklist.Carga.Codigo);
            Repositorio.Embarcador.GestaoPatio.CheckListCargaPergunta repPergunta = new Repositorio.Embarcador.GestaoPatio.CheckListCargaPergunta(unitOfWork);

            List<Dominio.Entidades.Embarcador.GestaoPatio.CheckListCargaPergunta> checkListCargaPerguntas = repPergunta.BuscarPorCheckList(checklist.Codigo);

            Dominio.Entidades.Embarcador.Pedidos.Pedido pedido = new Dominio.Entidades.Embarcador.Pedidos.Pedido()
            {
                Filial = checklist?.Carga?.Filial,
                TipoOperacao = sequenciaGestaoPatio?.CheckListTipoOperacao,
                Remetente = checklist?.Carga?.Pedidos?.FirstOrDefault()?.ObterDestinatario(),
                Destinatario = sequenciaGestaoPatio?.CheckListDestinatario,
                PesoTotal = cargaGuarita.QuantidadeLitros,
                Empresa = checklist.Carga?.Empresa.CNPJ_SemFormato == sequenciaGestaoPatio?.CheckListDestinatario?.CPF_CNPJ_SemFormato ? checklist.Carga.Empresa : null,
                Numero = repositorioPedido.BuscarProximoNumero(),
                SituacaoPedido = SituacaoPedido.Aberto,
                UltimaAtualizacao = DateTime.Now,
                Protocolo = repositorioPedido.BuscarProximoNumero(),
                NumeroPedidoEmbarcador = checklist?.DataLiberacao?.ToString().ObterSomenteNumeros()
            };

            foreach (var pergunta in checkListCargaPerguntas)
            {
                if (pergunta.TagIntegracao == "QtdeFcoj" && pergunta.Descricao.ToLower().Contains("fcoj"))
                    pedido.PesoTotal = pergunta.Observacao.ToDecimal();
            }

            repositorioPedido.Inserir(pedido);
        }

        private async Task VincularVigenciaCheckList(Dominio.Entidades.Embarcador.GestaoPatio.CheckListCarga checklist, Repositorio.UnitOfWork unitOfWork, CancellationToken cancellationToken)
        {
            if (checklist.CheckListCargaVigencia != null)
                return;

            Repositorio.Embarcador.GestaoPatio.CheckListCargaVigencia repositorioCheckListCargaVigencia = new Repositorio.Embarcador.GestaoPatio.CheckListCargaVigencia(unitOfWork, cancellationToken);
            Dominio.Entidades.Embarcador.GestaoPatio.CheckListCargaVigencia checkListCargaVigencia = await repositorioCheckListCargaVigencia.BuscarPorFilialETipoOperacaoAsync(checklist.FluxoGestaoPatio.Filial.Codigo, checklist.FluxoGestaoPatio.CargaBase.TipoOperacao?.Codigo ?? 0);

            if (checkListCargaVigencia == null)
                return;

            checkListCargaVigencia.PreenchimentoManualObrigatorio = false;

            await repositorioCheckListCargaVigencia.AtualizarAsync(checkListCargaVigencia);

            checklist.CheckListCargaVigencia = checkListCargaVigencia;
        }

        #endregion
    }
}
