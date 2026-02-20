using Dominio.Excecoes.Embarcador;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Microsoft.AspNetCore.Mvc;
using SGTAdmin.Controllers;

namespace SGT.WebAdmin.Controllers.Cargas.Carga.DadosTransporte
{
    [CustomAuthorize(new string[] { "VerificarAcaoPermitida", "BuscarTransportadoresSugeridosParaCarga" }, "Cargas/Carga", "Logistica/JanelaCarregamento")]
    public class CargaTransportadorController : BaseController
    {
        #region Construtores

        public CargaTransportadorController(Conexao conexao) : base(conexao) { }

        #endregion

        #region Métodos Globais

        public async Task<IActionResult> VerificarAcaoPermitida()
        {
            Repositorio.UnitOfWork unintOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigo = Request.GetIntParam("Carga");
                Repositorio.Embarcador.Cargas.Carga repositorioCarga = new Repositorio.Embarcador.Cargas.Carga(unintOfWork);
                Dominio.Entidades.Embarcador.Cargas.Carga carga = repositorioCarga.BuscarPorCodigo(codigo);
                bool liberarSolicitacaoNF = false;
                var mensagem = "";

                if (carga.SituacaoCarga == SituacaoCarga.AgTransportador && carga.Veiculo != null)
                {
                    liberarSolicitacaoNF = true;
                    Repositorio.Embarcador.Cargas.CargaMDFe repositorioCargaMDFe = new Repositorio.Embarcador.Cargas.CargaMDFe(unintOfWork);
                    List<Dominio.Entidades.Embarcador.Cargas.CargaMDFe> cargasMDFe = repositorioCargaMDFe.BuscarPorCargaVeiculosMDFeNaoEncerrados(carga.Veiculo.Placa);

                    //Repositorio.Embarcador.Cargas.CargaControleExpedicao repositorioCargaControleExpedicao = new Repositorio.Embarcador.Cargas.CargaControleExpedicao(unintOfWork);
                    //Dominio.Entidades.Embarcador.Cargas.CargaControleExpedicao cargaControleExpedicao = repositorioCargaControleExpedicao.BuscarPorCarga(carga.Codigo);

                    //if (!carga.CargaDePreCarga && cargaControleExpedicao != null && cargaControleExpedicao.SituacaoCargaControleExpedicao != SituacaoCargaControleExpedicao.Liberada)
                    //    liberarSolicitacaoNF = false;

                    if ((carga.Empresa == null || carga.Veiculo == null || carga.Motoristas == null || carga.Motoristas.Count == 0) && !carga.NaoExigeVeiculoParaEmissao)
                        liberarSolicitacaoNF = false;

                    if (cargasMDFe.Where(obj => obj.MDFe != null && obj.MDFe.Status == Dominio.Enumeradores.StatusMDFe.EmEncerramento).Count() > 0)
                    {
                        var retorno = new
                        {
                            LiberarSolicitacaoNF = false,
                            EncerrandoMDFe = true,
                            Mensagem = "Aguarde o encerramento do(s) MDF-e(s) " + string.Join(", ", cargasMDFe.Select(o => o.MDFe.Numero)) + " da carga " + cargasMDFe[0].Carga.RetornarCodigoCargaParaVisualizacao + " para solicitar as notas fiscais",
                            MDFe = (
                                from p in cargasMDFe
                                where p.MDFe != null
                                where p.MDFe.Status == Dominio.Enumeradores.StatusMDFe.EmEncerramento
                                select new
                                {
                                    p.Codigo,
                                    CodigoMDFE = p.MDFe.Codigo,
                                    CodigoEmpresa = p.MDFe.Empresa.Codigo,
                                    p.MDFe.Numero,
                                    MDFeAutorizado = p.MDFe.Status == Dominio.Enumeradores.StatusMDFe.Autorizado ? true : false,
                                    Serie = p.MDFe.Serie.Numero,
                                    Emissao = p.MDFe.DataEmissao.Value.ToString("dd/MM/yyyy HH:mm"),
                                    UFCarga = p.MDFe.EstadoCarregamento.Sigla + " - " + p.MDFe.EstadoCarregamento.Nome,
                                    UFDesgarga = p.MDFe.EstadoDescarregamento.Sigla + " - " + p.MDFe.EstadoDescarregamento.Nome,
                                    Status = p.MDFe.DescricaoStatus,
                                    RetornoSefaz = p.MDFe.MensagemStatus != null ? p.MDFe.MensagemStatus.MensagemDoErro : p.MDFe.MensagemRetornoSefaz,
                                    PossuiInformacoesIMO = !string.IsNullOrEmpty(p.MDFe.Empresa?.IMO) || (p.MDFe.Empresa?.DataValidadeIMO.HasValue ?? false)
                                }
                            ).ToList()
                        };

                        return new JsonpResult(retorno);
                    }
                    else if (cargasMDFe.Where(obj => obj.MDFe != null && obj.MDFe.Status == Dominio.Enumeradores.StatusMDFe.Autorizado && (obj.Carga.TipoOperacao == null || !obj.Carga.TipoOperacao.PermitirSolicitarNotasFiscaisSemEncerrarMDFeAnterior)).Count() > 0)
                    {
                        liberarSolicitacaoNF = false;
                        mensagem = "Não foi solicitado o encerramento do(s) MDF-e(s) " + string.Join(", ", cargasMDFe.Select(o => o.MDFe.Numero)) + " da carga " + cargasMDFe[0].Carga.RetornarCodigoCargaParaVisualizacao + ", desta forma não é possível avançar a etapa. Clicando em Salvar o sistema fará o encerramento do(s) mesmo(s) automaticamente.";
                    }
                }
                else if (carga.SituacaoCarga == SituacaoCarga.AgTransportador && carga.NaoExigeVeiculoParaEmissao)
                    liberarSolicitacaoNF = true;

                return new JsonpResult(new
                {
                    LiberarSolicitacaoNF = liberarSolicitacaoNF,
                    EncerrandoMDFe = false,
                    Mensagem = mensagem
                });
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Falha ao buscar os dados do Transportador.");
            }
            finally
            {
                unintOfWork.Dispose();
            }
        }

        public async Task<IActionResult> SalvarDadosTransportador(CancellationToken cancellationToken)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork, cancellationToken);
                Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unitOfWork, cancellationToken);
                Repositorio.Embarcador.Filiais.ConfiguracaoGestaoPatio repositorioConfiguracaoGestaoPatio = new Repositorio.Embarcador.Filiais.ConfiguracaoGestaoPatio(unitOfWork, cancellationToken);
                Repositorio.Embarcador.Transportadores.TransportadorAverbacao repTransportadorAverbacao = new Repositorio.Embarcador.Transportadores.TransportadorAverbacao(unitOfWork, cancellationToken);

                List<AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada> permissoesPersonalizadas = ObterPermissoesPersonalizadas("Cargas/Carga", "Logistica/JanelaCarregamento");

                if (TipoServicoMultisoftware != AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiCTe)
                {
                    if (!permissoesPersonalizadas.Contains(AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada.Carga_SalvarDadosTransporte))
                        return new JsonpResult(false, true, "Você não possui permissões para executar esta ação.");
                }

                int codigoCarga = Request.GetIntParam("Codigo");
                Dominio.Entidades.Embarcador.Cargas.Carga carga = await repCarga.BuscarPorCodigoAsync(codigoCarga);

                carga.Initialize();

                var apolicesSeguroTransportador = repTransportadorAverbacao
                    .BuscarPorTransportador(Request.GetIntParam("Empresa"))
                    .Select(a => a.ApoliceSeguro.Codigo)
                    .ToList();

                Dominio.ObjetosDeValor.Embarcador.Carga.CargaDadosTransporte dadosTransporte = new Dominio.ObjetosDeValor.Embarcador.Carga.CargaDadosTransporte()
                {
                    Carga = carga,
                    CodigosApoliceSeguro = apolicesSeguroTransportador.Count() > 0 ? apolicesSeguroTransportador : Request.GetListParam<int>("ApoliceSeguro"),
                    CodigoEmpresa = Request.GetIntParam("Empresa"),
                    CodigoReboque = Request.GetIntParam("Reboque"),
                    CodigoSegundoReboque = Request.GetIntParam("SegundoReboque"),
                    CodigoTracao = Request.GetIntParam("Veiculo"),
                    DataRetiradaCtrnReboque = Request.GetNullableDateTimeParam("DataRetiradaCtrnReboque"),
                    DataRetiradaCtrnSegundoReboque = Request.GetNullableDateTimeParam("DataRetiradaCtrnSegundoReboque"),
                    DataRetiradaCtrnVeiculo = Request.GetNullableDateTimeParam("DataRetiradaCtrnVeiculo"),
                    GensetVeiculo = Request.GetStringParam("GensetVeiculo"),
                    GensetReboque = Request.GetStringParam("GensetReboque"),
                    GensetSegundoReboque = Request.GetStringParam("GensetSegundoReboque"),
                    InicioCarregamento = Request.GetNullableDateTimeParam("InicioCarregamento"),
                    MaxGrossReboque = Request.GetIntParam("MaxGrossReboque"),
                    MaxGrossSegundoReboque = Request.GetIntParam("MaxGrossSegundoReboque"),
                    MaxGrossVeiculo = Request.GetIntParam("MaxGrossVeiculo"),
                    NumeroContainerReboque = Request.GetStringParam("NumeroContainerReboque"),
                    NumeroContainerSegundoReboque = Request.GetStringParam("NumeroContainerSegundoReboque"),
                    NumeroContainerVeiculo = Request.GetStringParam("NumeroContainerVeiculo"),
                    TaraContainerReboque = Request.GetIntParam("TaraContainerReboque"),
                    TaraContainerSegundoReboque = Request.GetIntParam("TaraContainerSegundoReboque"),
                    TaraContainerVeiculo = Request.GetIntParam("TaraContainerVeiculo"),
                    TerminoCarregamento = Request.GetNullableDateTimeParam("TerminoCarregamento"),
                    Container = Request.GetIntParam("Container")
                };

                dynamic motoristas = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(Request.Params("Motoristas"));

                foreach (var motorista in motoristas)
                    dadosTransporte.ListaCodigoMotorista.Add((int)motorista.Codigo);

                Servicos.Embarcador.Carga.Carga servicoCarga = new(unitOfWork);

                string mensagemValidarMotoristas = servicoCarga.CriarMensagemValidarMotoristas(dadosTransporte.ListaCodigoMotorista, TipoServicoMultisoftware, unitOfWork);
                if (!string.IsNullOrWhiteSpace(mensagemValidarMotoristas))
                {
                    return new JsonpResult(false, true, mensagemValidarMotoristas);
                }

                if (carga.TipoOperacao != null && carga.TipoOperacao.TipoCobrancaMultimodal == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCobrancaMultimodal.CTEAquaviario && TipoServicoMultisoftware != AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS)
                {
                    dadosTransporte.CodigoNavio = Request.GetIntParam("Navio");
                    dadosTransporte.CodigoBalsa = Request.GetIntParam("Balsa");
                }

                if (!ConfiguracaoEmbarcador.PermitirInformarDatasCarregamentoCarga)
                {
                    dadosTransporte.InicioCarregamento = null;
                    dadosTransporte.TerminoCarregamento = null;
                }

                string mensagemErro = string.Empty;

                if (carga.CargaAgrupamento != null || carga.CargaVinculada != null)
                    return new JsonpResult(false, true, "A carga foi agrupada, sendo assim não é possível alterá-la.");

                if (carga.Empresa != null && dadosTransporte.CodigoEmpresa != carga.Empresa.Codigo)
                {
                    if (!permissoesPersonalizadas.Contains(AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada.Carga_PermiteAlterarTransportador))
                        return new JsonpResult(false, true, "Você não possui permissão para alterar a transportadora");

                    Dominio.Entidades.Empresa novaEmpresa = await repEmpresa.BuscarPorCodigoAsync(dadosTransporte.CodigoEmpresa);
                    Servicos.Auditoria.Auditoria.Auditar(Auditado, carga, null, "Alterou a transportadora de " + carga.Empresa.Descricao + " para " + novaEmpresa.Descricao, unitOfWork);
                }

                if (carga.CargaPerigosaIntegracaoLeilao && dadosTransporte.CodigoEmpresa > 0 && (carga.TipoOperacao?.ConfiguracaoTransportador?.BloquearTransportadorNaoIMOAptoCargasPerigosas ?? false))
                {
                    Dominio.Entidades.Empresa transportador = repEmpresa.BuscarPorCodigo(dadosTransporte.CodigoEmpresa);

                    bool possuiInformacoesIMO = transportador != null ? (!string.IsNullOrEmpty(transportador.IMO) && (transportador.DataValidadeIMO.HasValue && transportador.DataValidadeIMO.Value > DateTime.Now)) : false;

                    if (!possuiInformacoesIMO)
                        return new JsonpResult(false, true, "Configuração não permite salvar transportador sem documento de IMO.");
                }

                string cpfMotoristaAnterior = carga.Motoristas?.Count > 0 ? carga.Motoristas?.ElementAtOrDefault(0).CPF : string.Empty;
                string placaTracaoAnterior = carga.Veiculo?.Placa;
                string placaReboqueAnterior = carga.VeiculosVinculados?.Count > 0 ? carga.VeiculosVinculados?.ElementAtOrDefault(0).Placa : string.Empty;
                string placaReboque2Anterior = carga.VeiculosVinculados?.Count > 1 ? carga.VeiculosVinculados?.ElementAtOrDefault(1).Placa : string.Empty;

                Auditado.Texto = "Encerramento MDF-e solicitado por atualização de transportador da Carga " + carga.CodigoCargaEmbarcador;

                var retorno = servicoCarga.SalvarDadosTransporteCarga(dadosTransporte, out mensagemErro, Usuario, ConfiguracaoEmbarcador.NaoBloquearCargaComProblemaIntegracaoGrMotoristaVeiculo, TipoServicoMultisoftware, WebServiceConsultaCTe, Cliente, Auditado, unitOfWork);

                if (retorno == null)
                    return new JsonpResult(false, true, mensagemErro);

                Dominio.Entidades.Embarcador.Filiais.ConfiguracaoGestaoPatio configuracaoGestaoPatio = repositorioConfiguracaoGestaoPatio.BuscarConfiguracao();

                Servicos.Auditoria.Auditoria.Auditar(Auditado, carga, carga.GetChanges(), "Salvou Dados do Transporte da Carga", unitOfWork);
                new Servicos.Embarcador.PreCarga.PreCarga(unitOfWork).ConfirmarAlteracaoDadosPreCarga(carga, Auditado);

                Servicos.Embarcador.Carga.CargaDatas servicoCargaDatas = new Servicos.Embarcador.Carga.CargaDatas(ConfiguracaoEmbarcador, unitOfWork);

                servicoCargaDatas.SalvarDataSalvamentoDadosTransporte(carga);

                Repositorio.Embarcador.Cargas.CargaVeiculoContainer repositorioCargaVeiculoContainer = new Repositorio.Embarcador.Cargas.CargaVeiculoContainer(unitOfWork);
                List<Dominio.Entidades.Embarcador.Cargas.CargaVeiculoContainer> cargaVeiculosContainer = repositorioCargaVeiculoContainer.BuscarPorCarga(carga.Codigo);
                Dominio.Entidades.Veiculo veiculo = carga.Veiculo;
                Dominio.Entidades.Veiculo reboque = carga.VeiculosVinculados?.ElementAtOrDefault(0);
                Dominio.Entidades.Veiculo segundoReboque = carga.VeiculosVinculados?.ElementAtOrDefault(1);
                Dominio.Entidades.Embarcador.Cargas.CargaVeiculoContainer containerVeiculo = (from cargaVeiculoContainer in cargaVeiculosContainer where cargaVeiculoContainer.Veiculo.Codigo == veiculo?.Codigo select cargaVeiculoContainer).FirstOrDefault();
                Dominio.Entidades.Embarcador.Cargas.CargaVeiculoContainer containerReboque = (from cargaVeiculoContainer in cargaVeiculosContainer where cargaVeiculoContainer.Veiculo.Codigo == reboque?.Codigo select cargaVeiculoContainer).FirstOrDefault();
                Dominio.Entidades.Embarcador.Cargas.CargaVeiculoContainer containerSegundoReboque = (from cargaVeiculoContainer in cargaVeiculosContainer where cargaVeiculoContainer.Veiculo.Codigo == segundoReboque?.Codigo select cargaVeiculoContainer).FirstOrDefault();

                new Servicos.Embarcador.Integracao.OpenTech.IntegracaoCargaOpenTech(unitOfWork).AtualizarVeiculoMotoristaColeta(carga, carga.Motoristas.Count > 0 ? carga.Motoristas?.ElementAtOrDefault(0).CPF : string.Empty, cpfMotoristaAnterior, veiculo?.Placa, placaTracaoAnterior, reboque?.Placa, placaReboqueAnterior, segundoReboque?.Placa, placaReboque2Anterior);

                if ((configuracaoGestaoPatio?.ChegadaVeiculoPermiteAvancarAutomaticamenteAposInformarDadosTransporteCarga ?? false) && TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiEmbarcador)
                {
                    Servicos.Embarcador.GestaoPatio.FluxoGestaoPatio servicoFluxoGestaoPatio = new Servicos.Embarcador.GestaoPatio.FluxoGestaoPatio(unitOfWork, Auditado, Cliente);
                    Repositorio.Embarcador.GestaoPatio.FluxoGestaoPatio repositorioFluxoGestaoPatio = new Repositorio.Embarcador.GestaoPatio.FluxoGestaoPatio(unitOfWork);
                    Dominio.Entidades.Embarcador.GestaoPatio.FluxoGestaoPatio fluxoGestaoPatio = repositorioFluxoGestaoPatio.BuscarPorCargaETipo(carga.Codigo, TipoFluxoGestaoPatio.Origem);

                    servicoFluxoGestaoPatio.AvancarEtapa(fluxoGestaoPatio, EtapaFluxoGestaoPatio.ChegadaVeiculo);

                    unitOfWork.CommitChanges();
                }

                servicoCarga.GerarNotificacaoEmailFornecedorDadosTransporte(carga, unitOfWork);

                return new JsonpResult(new
                {
                    DadosFrete = retorno,
                    DadosContainer = new
                    {
                        CodigoContainerReboque = containerReboque?.Codigo ?? 0,
                        CodigoContainerSegundoReboque = containerSegundoReboque?.Codigo ?? 0,
                        CodigoContainerVeiculo = containerVeiculo?.Codigo ?? 0
                    }
                });
            }
            catch (ControllerException excecao)
            {
                unitOfWork.Rollback();
                return new JsonpResult(false, excecao.Message);
            }
            catch (Exception excecao)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao atualizar a carga.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> BuscarTransportadoresSugeridosParaCarga()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigoCarga = int.Parse(Request.Params("Carga"));
                Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
                Dominio.Entidades.Embarcador.Cargas.Carga carga = repCarga.BuscarPorCodigo(codigoCarga);

                Models.Grid.Grid grid = new Models.Grid.Grid(Request);
                grid.header = new List<Models.Grid.Head>();
                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("Razão Social", "RazaoSocial", 32, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("CNPJ", "CNPJ", 13, Models.Grid.Align.center, false);
                grid.AdicionarCabecalho("Telefone", "Telefone", 9, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Localidade", "Localidade", 23, Models.Grid.Align.left, false, false, true);

                string propOrdenacao = grid.header[grid.indiceColunaOrdena].data;
                if (propOrdenacao == "Descricao")
                {
                    propOrdenacao = "RazaoSocial";
                }

                List<Dominio.Entidades.Empresa> listaEmpresa = repCarga.BuscarEmbarcadoresSugeridosParaCarga(carga, grid.header[grid.indiceColunaOrdena].data, grid.dirOrdena, grid.inicio, grid.limite);
                grid.setarQuantidadeTotal(repCarga.ContarEmbarcadoresSugeridosParaCarga(carga));

                dynamic lista = (from p in listaEmpresa select new { p.Codigo, p.DescricaoStatus, p.Telefone, p.RazaoSocial, CNPJ = p.CNPJ_Formatado, Localidade = p.Localidade.DescricaoCidadeEstado }).ToList();

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

        public async Task<IActionResult> DisponibilizarParaTransportador()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiCTe)
                    return new JsonpResult(false, true, "Você não possui permissão para executar esta ação.");

                unitOfWork.Start();

                int codigoCarga = Request.GetIntParam("Codigo");
                Repositorio.Embarcador.Cargas.Carga repositorioCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
                Dominio.Entidades.Embarcador.Cargas.Carga carga = repositorioCarga.BuscarPorCodigo(codigoCarga);

                if (carga == null)
                    throw new ControllerException("Não foi possível encontrar o registro.");

                Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repositorioConfiguracaoEmbarcador = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork);
                Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoEmbarcador = repositorioConfiguracaoEmbarcador.BuscarConfiguracaoPadrao();

                bool situacaoCargaPermiteAlterarTransportador = (
                    (carga.ExigeNotaFiscalParaCalcularFrete && (carga.SituacaoCarga == SituacaoCarga.Nova || carga.SituacaoCarga == SituacaoCarga.AgNFe)) ||
                    (!carga.ExigeNotaFiscalParaCalcularFrete && (carga.SituacaoCarga == SituacaoCarga.AgTransportador || carga.SituacaoCarga == SituacaoCarga.CalculoFrete || (carga.SituacaoCarga == SituacaoCarga.AgNFe && configuracaoEmbarcador.PermitirInformarDadosTransportadorCargaEtapaNFe)))
                );

                if (!situacaoCargaPermiteAlterarTransportador)
                    throw new ControllerException($"A Atual situação da carga ({carga.DescricaoSituacaoCarga}) não permite essa alteração.");

                if (!carga.RejeitadaPeloTransportador && !configuracaoEmbarcador.PermitirDisponibilizarCargaParaTransportador)
                    throw new ControllerException($"A carga deve estar rejeitada pelo transportador.");

                int codigoEmpresa = Request.GetIntParam("Empresa");
                Repositorio.Empresa repositorioEmpresa = new Repositorio.Empresa(unitOfWork);
                Dominio.Entidades.Empresa empresa = repositorioEmpresa.BuscarPorCodigo(codigoEmpresa);

                if (empresa == null)
                    throw new ControllerException("Não foi possível encontrar o transportador.");

                Repositorio.Embarcador.Cargas.CargaJanelaCarregamento repositorioCargaJanelaCarregamento = new Repositorio.Embarcador.Cargas.CargaJanelaCarregamento(unitOfWork);
                Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamento cargaJanelaCarregamento = repositorioCargaJanelaCarregamento.BuscarPorCarga(carga.Codigo);

                if (cargaJanelaCarregamento == null)
                    throw new ControllerException("Não foi possível encontrar a janela de carregamento.");

                Repositorio.Embarcador.Cargas.CargaJanelaCarregamentoTransportador repositorioCargaJanelaCarregamentoTransportador = new Repositorio.Embarcador.Cargas.CargaJanelaCarregamentoTransportador(unitOfWork);
                Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoTransportador cargaJanelaCarregamentoTransportador = repositorioCargaJanelaCarregamentoTransportador.BuscarSemRejeicao(cargaJanelaCarregamento.Codigo, codigoEmpresa);

                if (
                    (cargaJanelaCarregamentoTransportador != null) &&
                    (
                        (cargaJanelaCarregamentoTransportador.Situacao == SituacaoCargaJanelaCarregamentoTransportador.AgAceite) ||
                        (cargaJanelaCarregamentoTransportador.Situacao == SituacaoCargaJanelaCarregamentoTransportador.AgConfirmacao) ||
                        (cargaJanelaCarregamentoTransportador.Situacao == SituacaoCargaJanelaCarregamentoTransportador.Confirmada && !configuracaoEmbarcador.PermitirDisponibilizarCargaParaTransportador)
                    )
                )
                    throw new ControllerException("A carga já está disponível para o transportador.");

                carga.DataAtualizacaoCarga = DateTime.Now;
                carga.Empresa = empresa;
                carga.Motoristas?.Clear();
                carga.RejeitadaPeloTransportador = false;
                carga.Veiculo = null;
                carga.VeiculosVinculados?.Clear();

                repositorioCarga.Atualizar(carga);

                Servicos.Embarcador.Monitoramento.Monitoramento.CancelarMonitoramentoAoDisponibilizarTransportador(carga, configuracaoEmbarcador, Auditado, $"Disponibilizou a carga para o transportador {empresa.Descricao}", unitOfWork);
                Servicos.Embarcador.Logistica.CargaJanelaCarregamento servicoCargaJanelaCarregamento = new Servicos.Embarcador.Logistica.CargaJanelaCarregamento(unitOfWork, configuracaoEmbarcador);
                Servicos.Embarcador.Logistica.CargaJanelaCarregamentoTransportador servicoCargaJanelaCarregamentoTransportador = new Servicos.Embarcador.Logistica.CargaJanelaCarregamentoTransportador(unitOfWork, configuracaoEmbarcador);
                Servicos.Embarcador.Logistica.CargaJanelaCarregamentoNotificacao servicoCargaJanelaCarregamentoNotificacao = new Servicos.Embarcador.Logistica.CargaJanelaCarregamentoNotificacao(unitOfWork, configuracaoEmbarcador, null);

                if (cargaJanelaCarregamentoTransportador != null)
                {
                    cargaJanelaCarregamentoTransportador.Situacao = SituacaoCargaJanelaCarregamentoTransportador.AgConfirmacao;
                    repositorioCargaJanelaCarregamentoTransportador.Atualizar(cargaJanelaCarregamentoTransportador);
                    servicoCargaJanelaCarregamentoTransportador.SalvarHistoricoAlteracao(cargaJanelaCarregamentoTransportador, "Carga disponibilizada para o transportador confirmar os dados de transporte", Usuario);
                    servicoCargaJanelaCarregamentoNotificacao.EnviarEmailCargaDisponibilizadaParaTransportador(cargaJanelaCarregamentoTransportador);
                }
                else
                {
                    cargaJanelaCarregamentoTransportador = new Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoTransportador()
                    {
                        CargaJanelaCarregamento = cargaJanelaCarregamento,
                        HorarioLiberacao = DateTime.Now,
                        PendenteCalcularFrete = configuracaoEmbarcador.CalcularFreteCargaJanelaCarregamentoTransportador,
                        Situacao = SituacaoCargaJanelaCarregamentoTransportador.AgConfirmacao,
                        Transportador = empresa,
                        Tipo = TipoCargaJanelaCarregamentoTransportador.PorTipoTransportadorCarga
                    };

                    repositorioCargaJanelaCarregamentoTransportador.Inserir(cargaJanelaCarregamentoTransportador);
                    servicoCargaJanelaCarregamento.DefinirDataDisponibilizacaoTransportadores(cargaJanelaCarregamento);
                    servicoCargaJanelaCarregamentoTransportador.SalvarHistoricoAlteracao(cargaJanelaCarregamentoTransportador, "Carga disponibilizada para o transportador confirmar os dados de transporte", Usuario);
                    servicoCargaJanelaCarregamentoNotificacao.EnviarEmailCargaDisponibilizadaParaTransportador(cargaJanelaCarregamentoTransportador);
                }

                Servicos.Embarcador.Carga.CargaIndicador servicoCargaIndicador = new Servicos.Embarcador.Carga.CargaIndicador(unitOfWork);

                servicoCargaIndicador.DefinirIndicadorTransportador(carga, CargaIndicadorTransportador.InformadoManualmente);
                servicoCargaJanelaCarregamento.AtualizarSituacao(cargaJanelaCarregamento, TipoServicoMultisoftware);
                Servicos.Auditoria.Auditoria.Auditar(Auditado, carga, null, $"Disponibilizou a carga para o transportador {empresa.Descricao}", unitOfWork);

                unitOfWork.CommitChanges();

                new Servicos.Embarcador.Hubs.Carga().InformarCargaAlterada(carga.Codigo, TipoAcaoCarga.Alterada);
                new Servicos.Embarcador.Hubs.JanelaCarregamento().InformarJanelaCarregamentoAtualizada(cargaJanelaCarregamento);
                Dominio.Entidades.Embarcador.Cargas.TipoIntegracao tipoIntegracao = new Repositorio.Embarcador.Cargas.TipoIntegracao(unitOfWork).BuscarPorTipo(TipoIntegracao.HUB);

                if (tipoIntegracao != null)
                    await new Servicos.Embarcador.Integracao.HUB.IntegracaoHUBOfertas(unitOfWork, TipoServicoMultisoftware).AdicionarIntegracaoHUB(carga, tipoIntegracao);


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
                return new JsonpResult(false, "Ocorreu uma falha ao disponibilizar a carga para o transportador.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        #endregion

        #region Métodos Privados

        private void ValidaDadosFaltantesNoVeiculo(Dominio.Entidades.Veiculo veiculo, Dominio.Entidades.Empresa empresa, Repositorio.UnitOfWork unitOfWork)
        {
            //(Rodrigo Romanovski) Cesar quem pediu para criar essa regra no dia 20/06/2016, segundo ele o processo não pode parar por falta desses dados no veiculo, por isso setamos um padrão qualquer.

            Repositorio.Veiculo repVeiculo = new Repositorio.Veiculo(unitOfWork);

            bool alterarVeiculo = false;
            if (veiculo.Tara == 0)
            {
                veiculo.Tara = 10000;
                alterarVeiculo = true;
            }
            if (veiculo.CapacidadeKG == 0)
            {
                veiculo.CapacidadeKG = 10000;
                alterarVeiculo = true;
            }
            if (veiculo.Tipo == "T")
            {
                if (veiculo.RNTRC == 0)
                {
                    veiculo.RNTRC = !string.IsNullOrWhiteSpace(empresa.RegistroANTT) ? int.Parse(empresa.RegistroANTT) : 12345678;
                    alterarVeiculo = true;
                }
            }

            if (alterarVeiculo)
                repVeiculo.Atualizar(veiculo);

            if (veiculo.VeiculosVinculados != null)
            {
                foreach (Dominio.Entidades.Veiculo veiculoVinculado in veiculo.VeiculosVinculados)
                {
                    ValidaDadosFaltantesNoVeiculo(veiculoVinculado, empresa, unitOfWork);
                }
            }
        }

        #endregion
    }
}

