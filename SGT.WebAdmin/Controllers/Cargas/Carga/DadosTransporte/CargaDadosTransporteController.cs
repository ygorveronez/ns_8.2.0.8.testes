using Dominio.Excecoes.Embarcador;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Microsoft.AspNetCore.Mvc;
using Servicos;
using SGTAdmin.Controllers;

namespace SGT.WebAdmin.Controllers.Cargas.Carga.DadosTransporte
{
    [CustomAuthorize(new string[] { "VerificarDadosTransporteCarga" }, "Cargas/Carga", "Logistica/JanelaCarregamento", "CargaDadosTransporte/VerificarTipoImportacaoDocumentos")]
    public class CargaDadosTransporteController : BaseController
    {
        #region Construtores

        public CargaDadosTransporteController(Conexao conexao) : base(conexao) { }

        #endregion

        #region Métodos Globais

        public async Task<IActionResult> VerificarDadosTransporteCargaAsync(CancellationToken cancellationToken)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigo = int.Parse(Request.Params("Carga"));
                Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork, cancellationToken);
                Repositorio.Embarcador.Cargas.TipoCarregamento repTipoCarregamento = new Repositorio.Embarcador.Cargas.TipoCarregamento(unitOfWork);
                Repositorio.Embarcador.Cargas.CargaDadosTransporteIntegracao repCargaDadosTransporteIntegracao = new Repositorio.Embarcador.Cargas.CargaDadosTransporteIntegracao(unitOfWork, cancellationToken);
                Repositorio.Embarcador.Configuracoes.Integracao repositorioConfiguracaoIntegracao = new Repositorio.Embarcador.Configuracoes.Integracao(unitOfWork, cancellationToken);
                Repositorio.Embarcador.Transportadores.MotoristaIntegracao repMotoristaIntegracao = new Repositorio.Embarcador.Transportadores.MotoristaIntegracao(unitOfWork, cancellationToken);
                Repositorio.Embarcador.Veiculos.VeiculoIntegracao repVeiculoIntegracao = new Repositorio.Embarcador.Veiculos.VeiculoIntegracao(unitOfWork, cancellationToken);
                Repositorio.Embarcador.Configuracoes.IntegracaoGeralEFrete configuracaoIntegracaoGeralEFrete = new(unitOfWork, cancellationToken);

                Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoGeralCarga configuracaoGeralCarga = new Repositorio.Embarcador.Configuracoes.ConfiguracaoGeralCarga(unitOfWork).BuscarPrimeiroRegistro();
                Dominio.Entidades.Embarcador.Cargas.Carga carga = await repCarga.BuscarPorCodigoAsync(codigo);
                Dominio.Entidades.Embarcador.Configuracoes.Integracao configuracaoIntegracao = repositorioConfiguracaoIntegracao.Buscar();
                Dominio.Entidades.Embarcador.Configuracoes.IntegracaoGeralEFrete configuracaoIntegracaoEFrete = await configuracaoIntegracaoGeralEFrete.BuscarPrimeiroRegistroAsync();

                Servicos.Embarcador.Carga.Carga serCarga = new Servicos.Embarcador.Carga.Carga(unitOfWork);
                Dominio.ObjetosDeValor.Embarcador.Carga.RetornoCargaPossuiOperacao retorno = serCarga.VerificarSeCargaPossuiOperacao(carga, unitOfWork);

                List<TipoIntegracao> tipoIntegracoesVeiculos = new List<TipoIntegracao>() { TipoIntegracao.BrasilRiskGestao };
                if (configuracaoIntegracaoEFrete?.ConsultarTagAoIncluirVeiculoNaCarga ?? false)
                    tipoIntegracoesVeiculos.Add(TipoIntegracao.EFrete);

                retorno.PossuiIntegracao = repCargaDadosTransporteIntegracao.ExistePorCarga(carga.Codigo);
                retorno.PossuiObservacao = carga.TipoOperacao?.ConfiguracaoCarga?.PermitirAdicionarObservacaoNaEtapaUmDaCarga ?? false;
                retorno.PermitirTransportadorReenviarIntegracoesComProblemasOpenTech = configuracaoIntegracao?.PermitirTransportadorReenviarIntegracoesComProblemasOpenTech ?? false;
                retorno.PortalMultiTransportador = TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiCTe ? true : false;
                retorno.PermitirVisualizarOrdernarZonasTransporte = carga.TipoOperacao?.ConfiguracaoCarga?.PermitirVisualizarOrdenarAsZonasDeTransporte ?? false;
                retorno.PermitirVisualizarTipoCarregamento = repTipoCarregamento.BuscarTodos().ToList().Count() > 0;
                retorno.PermitirVisualizarCentroResultado = configuracaoGeralCarga?.InformarCentroResultadoNaEtapaUmDaCarga ?? false;
                retorno.PermitirInformarAjudantesNaCarga = carga.TipoOperacao?.ConfiguracaoCarga?.PermitirInformarAjudantesNaCarga ?? false;
                retorno.PossuiIntegracaoIntegrada = repCargaDadosTransporteIntegracao.ContarPorCarga(carga.Codigo, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.Integrado) > 0;
                retorno.ExigePlacaTracao = carga.TipoOperacao?.ExigePlacaTracao ?? false;
                retorno.NaoPermitirAlterarMotoristaAposAverbacaoContainer = carga.TipoOperacao?.ConfiguracaoContainer?.NaoPermitirAlterarMotoristaAposAverbacaoContainer ?? false;
                if (carga.TipoOperacao?.PossuiIntegracaoBrasilRisk ?? false)
                {
                    retorno.PossuiIntegracaoMotorista = carga.Motoristas?.Count() > 0 ? repMotoristaIntegracao.PossuiIntegracaoPorMotoristaETipo(carga.Motoristas.FirstOrDefault().Codigo, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.BrasilRiskGestao) : false;
                    retorno.PossuiIntegracaoVeiculo = carga.Veiculo != null ? repVeiculoIntegracao.PossuiIntegracaoVeiculoeTipos(carga.Veiculo.Codigo, tipoIntegracoesVeiculos) : false;
                }
                return new JsonpResult(retorno);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao verificar a carga.");
            }
            finally
            {
                await unitOfWork.DisposeAsync();
            }

        }

        public async Task<IActionResult> SalvarDadosTransporteCargaAsync(CancellationToken cancellationToken)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unitOfWork, cancellationToken);
                Repositorio.Setor repSetor = new Repositorio.Setor(unitOfWork, cancellationToken);
                Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork, cancellationToken);
                Repositorio.Veiculo repVeiculo = new Repositorio.Veiculo(unitOfWork, cancellationToken);
                Repositorio.Embarcador.Cargas.CargaDadosTransporteIntegracao repCargaDadosTransporteIntegracao = new Repositorio.Embarcador.Cargas.CargaDadosTransporteIntegracao(unitOfWork);
                Repositorio.Embarcador.Cargas.CargaPedido repCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(unitOfWork, cancellationToken);
                Repositorio.Embarcador.Pedidos.StageAgrupamento repositorioStageAgrupamento = new Repositorio.Embarcador.Pedidos.StageAgrupamento(unitOfWork);
                Repositorio.Embarcador.Filiais.ConfiguracaoGestaoPatio repositorioConfiguracaoGestaoPatio = new Repositorio.Embarcador.Filiais.ConfiguracaoGestaoPatio(unitOfWork);
                Repositorio.Embarcador.Configuracoes.ConfiguracaoCargaDadosTransporte repositorioConfiguracaoCargaDadosTransporte = new Repositorio.Embarcador.Configuracoes.ConfiguracaoCargaDadosTransporte(unitOfWork);
                Repositorio.Embarcador.Cargas.TipoIntegracao repositorioTipoIntegracao = new Repositorio.Embarcador.Cargas.TipoIntegracao(unitOfWork);
                Repositorio.Embarcador.Configuracoes.ConfiguracaoGeralCarga repositorioConfiguracaoGeralCarga = new Repositorio.Embarcador.Configuracoes.ConfiguracaoGeralCarga(unitOfWork);
                Repositorio.Embarcador.Configuracoes.Integracao repConfiguracaoIntegracao = new Repositorio.Embarcador.Configuracoes.Integracao(unitOfWork);
                Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repositorioTMS = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork);

                Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoTMS = repositorioTMS.BuscarConfiguracaoPadrao();
                Dominio.Entidades.Embarcador.Configuracoes.Integracao configuracaoIntegracao = repConfiguracaoIntegracao.Buscar();
                Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoCargaDadosTransporte configuracaoCargaDadosTransporte = await repositorioConfiguracaoCargaDadosTransporte.BuscarPrimeiroRegistroAsync();
                Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoGeralCarga configuracaoGeralCarga = await repositorioConfiguracaoGeralCarga.BuscarPrimeiroRegistroAsync();

                Servicos.Embarcador.Carga.HistoricoVinculo serHistorico = new Servicos.Embarcador.Carga.HistoricoVinculo(unitOfWork);

                Servicos.Embarcador.Logistica.FilaCarregamentoVeiculo servicoFilaCarregamentoVeiculo = new Servicos.Embarcador.Logistica.FilaCarregamentoVeiculo(unitOfWork, OrigemAlteracaoFilaCarregamento.Sistema);

                List<AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada> permissoesPersonalizadas = ObterPermissoesPersonalizadas("Cargas/Carga", "Logistica/JanelaCarregamento");

                if (TipoServicoMultisoftware != AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiCTe)
                {
                    if (!permissoesPersonalizadas.Contains(AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada.Carga_SalvarDadosTransporte))
                        return new JsonpResult(false, true, "Você não possui permissões para executar esta ação.");
                }

                int codigoCarga = Request.GetIntParam("Codigo");
                Dominio.Entidades.Embarcador.Cargas.Carga carga = await repCarga.BuscarPorCodigoAsync(codigoCarga);

                bool? avancadaComProblemaIntegracaoDigitalCom = Request.GetNullableBoolParam("ProblemaValePedagio");
                bool atualizarIntegracoes = false;

                if (carga.CargaBloqueadaParaEdicaoIntegracao)
                    return new JsonpResult(false, true, "A carga não pode ser editada.");

                var setor = await repSetor.BuscarPorCodigoAsync(Request.GetIntParam("Setor"));

                carga.Initialize();

                carga.Observacao = Request.GetStringParam("ObservacaoCarga");
                carga.Setor = setor;

                Dominio.ObjetosDeValor.Embarcador.Carga.CargaDadosTransporte dadosTransporte = new Dominio.ObjetosDeValor.Embarcador.Carga.CargaDadosTransporte()
                {
                    Carga = carga,
                    CodigosApoliceSeguro = Request.GetListParam<int>("ApoliceSeguro"),
                    CodigoEmpresa = Request.GetIntParam("Empresa"),
                    CodigoModeloVeicular = Request.GetIntParam("ModeloVeicular"),
                    CodigoReboque = Request.GetIntParam("Reboque"),
                    CodigoSegundoReboque = Request.GetIntParam("SegundoReboque"),
                    CodigoTerceiroReboque = Request.GetIntParam("TerceiroReboque"),
                    CodigoPedidoViagemNavio = Request.GetIntParam("PedidoViagemNavio"),
                    CodigoPortoDestino = Request.GetIntParam("PortoDestino"),
                    CodigoPortoOrigem = Request.GetIntParam("PortoOrigem"),
                    CodigoTerminalDestino = Request.GetIntParam("TerminalDestino"),
                    CodigoTerminalOrigem = Request.GetIntParam("TerminalOrigem"),
                    CodigoTipoCarga = Request.GetIntParam("TipoCarga"),
                    CodigoTipoOperacao = Request.GetIntParam("TipoOperacao"),
                    CodigoSetor = Request.GetIntParam("Setor"),
                    CodigoTracao = Request.GetIntParam("Veiculo"),
                    InicioCarregamento = Request.GetNullableDateTimeParam("InicioCarregamento"),
                    TerminoCarregamento = Request.GetNullableDateTimeParam("TerminoCarregamento"),
                    NumeroPager = Request.GetStringParam("NumeroPager"),
                    SalvarDadosTransporteSemSolicitarNFes = Request.GetBoolParam("SalvarDadosTransporteSemSolicitarNFes"),
                    ProtocoloIntegracaoGR = Request.GetStringParam("ProtocoloIntegracaoGR"),
                    ObservacaoTransportador = Request.GetStringParam("ObservacaoTransportador"),
                    CodigoTipoContainer = Request.GetIntParam("TipoContainer"),
                    DataBaseCRT = Request.GetNullableDateTimeParam("DataBaseCRT"),
                    QuantidadePaletes = Request.GetIntParam("QuantidadePaletes"),
                    CargaEstaEmParqueamento = Request.GetBoolParam("CargaEstaEmParqueamento"),
                    CodigoTipoCarregamento = Request.GetIntParam("TipoCarregamento"),
                    CodigoCentroResultado = Request.GetIntParam("CentroResultado"),
                    ListaCodigoAjudante = Request.GetListParam<int>("Ajudantes"),
                    CodigoJustificativaAutorizacaoCarga = Request.GetIntParam("JustificativaAutorizacaoCarga"),
                    LiberadaComCargaSemPlanejamento = Request.GetBoolParam("LiberadaComCargaSemPlanejamento"),
                    NumeroContainerVeiculo = Request.GetStringParam("NumeroContainerVeiculo"),
                    Container = Request.GetIntParam("Container")
                };


                if (!(carga.RejeitadaPeloTransportador && ConfiguracaoEmbarcador.InformaApoliceSeguroMontagemCarga))
                    dadosTransporte.CodigosApoliceSeguro = null;

                if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS && dadosTransporte.CodigoTracao != (carga.Veiculo?.Codigo ?? 0) && !permissoesPersonalizadas.Contains(AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada.Carga_PermiteInserirAlterarVeiculoNaCargaOuPedido))
                    return new JsonpResult(false, true, "Usuário não tem permissão para alterar o veículo.");

                Dominio.Entidades.Veiculo veiculoSubstituido = new Dominio.Entidades.Veiculo();
                List<Dominio.Entidades.Veiculo> veiculoSubstituidos = new List<Dominio.Entidades.Veiculo>();

                if (carga.Veiculo != null && dadosTransporte.CodigoTracao > 0 && carga.Veiculo.Codigo != dadosTransporte.CodigoTracao)
                {
                    veiculoSubstituido = carga.Veiculo;
                    veiculoSubstituidos.AddRange(carga.VeiculosVinculados);
                }

                if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS && dadosTransporte.CodigoTracao > 0)
                {
                    Dominio.Entidades.Veiculo veiculoValidacaoPropriedade = repVeiculo.BuscarPorCodigo(dadosTransporte.CodigoTracao);

                    if (!permissoesPersonalizadas.Contains(AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada.Carga_PermiteInserirVeiculoProprio) && veiculoValidacaoPropriedade.Tipo.Equals("P"))
                        throw new ControllerException(Localization.Resources.Cargas.Carga.VoceNaoPossuiPermissaoParaInserirVeiculoProprio);

                    if (!permissoesPersonalizadas.Contains(AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada.Carga_PermiteInserirVeiculoTerceiro) && veiculoValidacaoPropriedade.Tipo.Equals("T"))
                        throw new ControllerException(Localization.Resources.Cargas.Carga.VoceNaoPossuiPermissaoParaInserirVeiculoTerceiro);
                }

                bool liberarComProblemaIntegracaoGrMotoristaVeiculo = Request.GetBoolParam("LiberarComProblemaIntegracaoGrMotoristaVeiculo");

                if (!ConfiguracaoEmbarcador.PermitirInformarDatasCarregamentoCarga)
                {
                    dadosTransporte.InicioCarregamento = null;
                    dadosTransporte.TerminoCarregamento = null;
                }

                dynamic motoristas = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(Request.Params("Motoristas"));
                atualizarIntegracoes = atualizarIntegracoes || !(dadosTransporte.CodigoTipoCarga == (carga.TipoDeCarga?.Codigo ?? 0)); // houve alteração no tipo de carga

                foreach (var motorista in motoristas)
                    dadosTransporte.ListaCodigoMotorista.Add((int)motorista.Codigo);

                if (dadosTransporte.ListaCodigoMotorista.Count > 0)
                {
                    Repositorio.Usuario repositorioMotorista = new Repositorio.Usuario(unitOfWork);
                    List<Dominio.Entidades.Usuario> motoristasParaValidar = repositorioMotorista.BuscarMotoristaPorCodigo(dadosTransporte.ListaCodigoMotorista.ToArray());
                    List<Dominio.Entidades.Usuario> motoristasBloqueados = repositorioMotorista.BuscarMotoristaBloqueadoPorCodigo(null, motoristasParaValidar);

                    if (motoristasBloqueados.Count > 0)
                    {
                        string mensagemBloqueioMotorista = $"O(s) seguinte(s) motorista(s) estão bloqueados {string.Join(", ", (from motoristasBloqueado in motoristasBloqueados select motoristasBloqueado.Nome))}";

                        if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiEmbarcador)
                            mensagemBloqueioMotorista += $" pelo(s) motivo(s) {string.Join(", ", (from motoristaBloqueado in motoristasBloqueados select motoristaBloqueado.MotivoBloqueio))}.";
                        else
                            mensagemBloqueioMotorista += ". Entre em contato com o embarcador para entender o motivo.";

                        return new JsonpResult(false, true, mensagemBloqueioMotorista);
                    }

                    DateTime dataAtual = DateTime.Now;
                    string mensagemMotoristaSuspenso = string.Empty;
                    foreach (var motorista in motoristasParaValidar)
                    {
                        if (motorista.DataSuspensaoInicio.HasValue && motorista.DataSuspensaoFim.HasValue)
                            if ((dataAtual >= motorista.DataSuspensaoInicio) && (dataAtual <= motorista.DataSuspensaoFim))
                                mensagemMotoristaSuspenso += $"Nome {motorista.Nome} está suspenso.";
                    }
                    if (!string.IsNullOrEmpty(mensagemMotoristaSuspenso))
                        return new JsonpResult(false, true, $"Existem motorista(s) suspenso(s), entrar em contato com a transportadora: {mensagemMotoristaSuspenso}");
                }

                bool houveAlteracaoDeMotoristaOuVeiculo = VerificarAlteracaoDeMotoristasOuVeiculos(carga, dadosTransporte);
                bool houveAlteracaoDeReboques = VerificarAlteracaoDeReboques(carga, dadosTransporte);
                atualizarIntegracoes = atualizarIntegracoes || houveAlteracaoDeMotoristaOuVeiculo || houveAlteracaoDeReboques; // houve alteração no reboque, tração, ou motorista
                atualizarIntegracoes = atualizarIntegracoes || !(dadosTransporte.CodigoTipoOperacao == (carga.TipoOperacao?.Codigo ?? 0)); // houve alteração no tipo operação
                Repositorio.Embarcador.Cargas.CargaMotorista repositorioCargaMotorista = new Repositorio.Embarcador.Cargas.CargaMotorista(unitOfWork);
                var agrupamentos = await repositorioStageAgrupamento.BuscarPorCargaDtAsync(carga.Codigo);
                if (agrupamentos != null && agrupamentos.Count > 0)
                {
                    if (agrupamentos.Any(x => x.Veiculo == null || x.Motorista == null))
                    {
                        Repositorio.Embarcador.Pedidos.Stage repositorioStage = new Repositorio.Embarcador.Pedidos.Stage(unitOfWork);
                        List<int> numerosStagesSemVeiculosOuMotorista = agrupamentos.Where(x => x.Veiculo == null || x.Motorista == null).Select(x => x.Codigo).ToList();
                        List<string> stages = repositorioStage.ObterNumerosStagesPorAgrupamento(numerosStagesSemVeiculosOuMotorista);
                        return new JsonpResult(false, true, $"Obrigatório informar motorista(s) e Placa(s) nos agrupamentos ({string.Join(",", stages)})para seguir.");
                    }

                    var agrupamento = agrupamentos.FirstOrDefault();
                    dadosTransporte.CodigoReboque = agrupamento?.Reboque?.Codigo ?? 0;
                    dadosTransporte.CodigoSegundoReboque = agrupamento?.SegundoReboque?.Codigo ?? 0;
                    dadosTransporte.CodigoTracao = agrupamento?.Veiculo?.Codigo ?? 0;

                    if (dadosTransporte.ListaCodigoMotorista.Count == 0)
                    {
                        dadosTransporte.ListaCodigoMotorista.AddRange(await repositorioCargaMotorista.BuscarCodigoMotoristasPorCargaAsync(carga.Codigo));

                        if (dadosTransporte.ListaCodigoMotorista.Count == 0)
                            dadosTransporte.ListaCodigoMotorista.AddRange(await repositorioStageAgrupamento.BuscarCodigoMotoristaPorAgrupamentoAsync(carga.Codigo));
                    }
                }

                if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiEmbarcador
                && (carga.SituacaoCarga != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.Nova
                && carga.SituacaoCarga != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.AgTransportador))
                {
                    if (!(carga.TipoOperacao?.NaoExigeVeiculoParaEmissao ?? false) && dadosTransporte.ListaCodigoMotorista.Count == 0)
                        return new JsonpResult(false, true, $"Obrigatório informar motorista(s) para seguir.");

                    if (!(carga.TipoOperacao?.NaoExigeVeiculoParaEmissao ?? false) && dadosTransporte.CodigoTracao <= 0)
                        return new JsonpResult(false, true, $"Obrigatório informar o veiculo.");
                }

                if ((configuracaoCargaDadosTransporte?.ExigirQueVeiculoCavaloTenhaReboqueVinculado ?? false) && dadosTransporte.CodigoTracao != 0)
                {
                    var veiculoValidar = await repVeiculo.BuscarPorCodigoAsync(dadosTransporte.CodigoTracao);

                    if (TipoServicoMultisoftware != AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS)
                    {
                        if (veiculoValidar != null && veiculoValidar.TipoVeiculo == "0" && (veiculoValidar.VeiculosVinculados == null || veiculoValidar.VeiculosVinculados.Count == 0))
                            return new JsonpResult(false, true, $"Não é possível selecionar um veículo do tipo Tração que não tenha nenhum Reboque atrelado.");
                    }
                    else if (veiculoValidar != null && veiculoValidar.ModeloVeicularCarga != null && veiculoValidar.ModeloVeicularCarga.Tipo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoModeloVeicularCarga.Tracao && (veiculoValidar.VeiculosVinculados == null || veiculoValidar.VeiculosVinculados.Count == 0))
                        return new JsonpResult(false, true, $"Não é possível selecionar um veículo do tipo Tração que não tenha nenhum Reboque atrelado.");
                }

                Servicos.Embarcador.Carga.Carga svcCarga = new Servicos.Embarcador.Carga.Carga(unitOfWork);

                string mensagemErro = string.Empty;

                carga.LiberadaComLicencaInvalida = Request.GetBoolParam("LiberadaComLicencaInvalida");
                carga.LiberadaComCargaSemPlanejamento = dadosTransporte.LiberadaComCargaSemPlanejamento;

                if (carga.PedidoViagemNavio != null && carga.PedidoViagemNavio.Codigo != dadosTransporte.CodigoPedidoViagemNavio)
                    return new JsonpResult(false, true, "Não é permitido alterar a viagem quando a carga já está gerada.");

                if (Usuario.LimitarOperacaoPorEmpresa && dadosTransporte.CodigoEmpresa > 0 && !Usuario.Empresas.Any(e => e.Codigo == dadosTransporte.CodigoEmpresa))
                    return new JsonpResult(false, true, "Você não possui permissão para utilizar a Empresa selecionada");

                if ((carga.Empresa != null && dadosTransporte.CodigoEmpresa > 0) && dadosTransporte.CodigoEmpresa != carga.Empresa.Codigo)
                {
                    if (!permissoesPersonalizadas.Contains(AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada.Carga_PermiteAlterarTransportador))
                        return new JsonpResult(false, true, "Você não possui permissão para alterar a transportadora");

                    Dominio.Entidades.Empresa novaEmpresa = await repEmpresa.BuscarPorCodigoAsync(dadosTransporte.CodigoEmpresa);
                    await Servicos.Auditoria.Auditoria.AuditarAsync(Auditado, carga, null, "Alterou a transportadora de " + carga.Empresa.Descricao + " para " + novaEmpresa.Descricao, unitOfWork);
                }

                if (carga.CargaAgrupamento != null || carga.CargaVinculada != null)
                    return new JsonpResult(false, true, "A carga foi agrupada, sendo assim não é possível alterá-la.");

                Auditado.Texto = "Encerramento MDF-e solicitado por atualização de transportador da Carga " + carga.CodigoCargaEmbarcador;

                if (ConfiguracaoEmbarcador.NaoBloquearCargaComProblemaIntegracaoGrMotoristaVeiculo)
                    liberarComProblemaIntegracaoGrMotoristaVeiculo = true;

                if ((carga.TipoOperacao?.NaoPermiteAvancarCargaSemDataPrevisaoDeEntrega ?? false) && await repCargaPedido.BuscarPedidoSemDataPrevisaoEntregaAsync(codigoCarga))
                    return new JsonpResult(false, true, "Não é possível salvar. Pedido(s) sem data de previsão de entrega.");

                if ((carga.TipoOperacao?.BloquearAvancoCargaVolumesZerados ?? false) && await repCargaPedido.PossuiPedidoComVolumesZeradoAsync(codigoCarga))
                    return new JsonpResult(false, true, "Não é possível salvar. É necessário que todos os pedidos tenham os volumes informados.");

                if (carga.TipoOperacao?.ConfiguracaoCarga?.NecessitaInformarPlacaCarregamento ?? false)
                    ValidarPreenchimentoPlacaCarregamento(ref carga, unitOfWork);

                if (Servicos.Embarcador.Carga.Carga.IsCargaComPedidosSemProdutos(carga, unitOfWork))
                {
                    if (dadosTransporte.SalvarDadosTransporteSemSolicitarNFes)
                        throw new ServicoException("Não é possível salvar sem adicionar pelo menos um produto na carga.");

                    throw new ServicoException("Não é possível solicitar as NF-e sem adicionar pelo menos um produto na carga.");
                }

                if (Servicos.Embarcador.Carga.Carga.IsCargaComClienteSemLocalidade(carga, unitOfWork))
                {
                    if (!Servicos.Embarcador.Carga.Carga.ConfirmarCargaComClienteSemLocalidade(carga, unitOfWork))
                    {
                        if (dadosTransporte.SalvarDadosTransporteSemSolicitarNFes)
                            throw new ServicoException("Não é possível salvar, carga possui cliente sem localidade.");

                        throw new ServicoException("Não é possível solicitar as NF-e, carga possui cliente sem localidade.");
                    }
                    else
                        svcCarga.FecharCarga(carga, unitOfWork, TipoServicoMultisoftware, this.Cliente);
                }

                if (carga.ModeloVeicularCarga != null && dadosTransporte.CodigoModeloVeicular != carga.ModeloVeicularCarga?.Codigo)
                    if (!(this.Usuario.UsuarioAdministrador || permissoesPersonalizadas.Contains(AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada.Carga_PermitirAlterarModeloVeicularNaCarga)))
                        return new JsonpResult(false, true, "Você não possui permissão para alterar o modelo veicular.");

                if (dadosTransporte.CodigoEmpresa != (carga.Empresa?.Codigo ?? 0))
                    ProcessarIntegracoesAgRetornoBoticario(carga, unitOfWork);

                IntegrarDadosERP(carga, dadosTransporte, unitOfWork);

                string mensagemRetorno = ValidarDadosJanelaCarregamento(carga, dadosTransporte, unitOfWork);

                if (!string.IsNullOrWhiteSpace(mensagemRetorno))
                    throw new ControllerException(mensagemRetorno, Dominio.ObjetosDeValor.Enumerador.CodigoExcecao.VeiculosInvalidosCarga);

                if (configuracaoGeralCarga.NaoPermitirAvançarEtapaUmCargaComTransportadorSemApoliceVigente)
                {
                    Repositorio.Embarcador.Transportadores.TransportadorAverbacao repositorioTransportadorAverbacao = new Repositorio.Embarcador.Transportadores.TransportadorAverbacao(unitOfWork, cancellationToken);
                    Dominio.Entidades.Empresa transporadora = await repEmpresa.BuscarPorCodigoAsync(dadosTransporte.CodigoEmpresa);

                    List<Dominio.Entidades.Embarcador.Transportadores.TransportadorAverbacao> transportadorAverbacoes = await repositorioTransportadorAverbacao.BuscarPorTodosTransportadorAsync(transporadora.Codigo);

                    if (transporadora.UsarTipoOperacaoApolice && transportadorAverbacoes.Count <= 0)
                        throw new ServicoException("Não é possível salvar os dados de transporte, pois o transportador está sem apólice vigente");
                }

                //SE PARQUEAMENTO, O USUARIO PODE ATUALIZAR A TRAÇÃO, E REINICIAR DADOS DO MONITORAMENTO PARA O NOVO VEICULO
                if (dadosTransporte.CargaEstaEmParqueamento)
                {
                    if (dadosTransporte.CodigoTracao > 0)
                        carga.Veiculo = await repVeiculo.BuscarPorCodigoAsync(dadosTransporte.CodigoTracao);
                    else
                        carga.Veiculo = null;

                    await repCarga.AtualizarAsync(carga);
                    await Servicos.Embarcador.Monitoramento.Monitoramento.GerarMonitoramentoEIniciarAsync(carga, ConfiguracaoEmbarcador, Auditado, "Salvar dados de transporte da carga (Carga em Parqueamento)", unitOfWork);

                    return new JsonpResult(new { AlterouVeiculoEmParqueamento = true });
                }

                if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS)
                {
                    if (veiculoSubstituido != null)
                        servicoFilaCarregamentoVeiculo.RealocarVeiculoNaFila(veiculoSubstituido.Codigo, TipoServicoMultisoftware);

                    if (veiculoSubstituidos.Count > 0)
                    {
                        foreach (var veiculoS in veiculoSubstituidos)
                        {
                            servicoFilaCarregamentoVeiculo.RealocarVeiculoNaFila(veiculoS.Codigo, TipoServicoMultisoftware);
                        }
                    }
                }

                if (avancadaComProblemaIntegracaoDigitalCom.HasValue && avancadaComProblemaIntegracaoDigitalCom.Value)
                    Servicos.Auditoria.Auditoria.Auditar(Auditado, carga, "Avançou a carga sem vale pedagio.", unitOfWork);

                if (carga.TipoOperacao != null && carga.TipoOperacao.TipoCobrancaMultimodal == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCobrancaMultimodal.CTEAquaviario && TipoServicoMultisoftware != AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS)
                {
                    dadosTransporte.CodigoNavio = Request.GetIntParam("Navio");
                    dadosTransporte.CodigoBalsa = Request.GetIntParam("Balsa");
                }

                if (carga.TipoOperacao?.ConfiguracaoCarga?.InformarTransportadorSubcontratadoEtapaUm ?? false)
                {
                    dadosTransporte.CodigoTransportadorSubcontratado = Request.GetIntParam("TransportadorSubcontratado");
                }

                string cpfMotoristaAnterior = carga.Motoristas?.Count > 0 ? carga.Motoristas.ElementAtOrDefault(0).CPF : string.Empty;
                string placaTracaoAnterior = carga.Veiculo?.Placa;
                string placaReboqueAnterior = carga.VeiculosVinculados?.Count > 0 ? carga.VeiculosVinculados.ElementAtOrDefault(0).Placa : string.Empty;
                string placaReboque2Anterior = carga.VeiculosVinculados?.Count > 1 ? carga.VeiculosVinculados.ElementAtOrDefault(1).Placa : string.Empty;

                dynamic retorno = svcCarga.SalvarDadosTransporteCarga(dadosTransporte, out mensagemErro, Usuario, liberarComProblemaIntegracaoGrMotoristaVeiculo, TipoServicoMultisoftware, WebServiceConsultaCTe, Cliente, Auditado, unitOfWork, true, true);

                if (retorno == null)
                    return new JsonpResult(false, true, mensagemErro);

                if (atualizarIntegracoes && carga.Pedidos.Count > 0)
                    Servicos.Embarcador.Integracao.IntegracaoPedido.ReenviarIntegracaoPedidos(carga.Pedidos.Select(obj => obj.Pedido.Codigo).ToList(), unitOfWork);

                retorno.PossuiIntegracao = repCargaDadosTransporteIntegracao.ExistePorCarga(carga.Codigo);

                if (carga.LiberadaComCargaSemPlanejamento)
                    Servicos.Auditoria.Auditoria.Auditar(Auditado, carga, carga.GetChanges(), "Salvou Dados do Transporte da Carga liberando sem planejamento", unitOfWork);
                else if (!liberarComProblemaIntegracaoGrMotoristaVeiculo || (carga.LicencaInvalida && !carga.LiberadaComLicencaInvalida))
                    Servicos.Auditoria.Auditoria.Auditar(Auditado, carga, carga.GetChanges(), "Salvou Dados do Transporte da Carga", unitOfWork);
                else if (carga.ProblemaIntegracaoGrMotoristaVeiculo && ConfiguracaoEmbarcador.NaoBloquearCargaComProblemaIntegracaoGrMotoristaVeiculo)
                    Servicos.Auditoria.Auditoria.Auditar(Auditado, carga, carga.GetChanges(), "Salvou Dados do Transporte da Carga. (Rejeição na GR, configurado para não bloquear)", unitOfWork);
                else if (carga.ProblemaIntegracaoGrMotoristaVeiculo && carga.LiberadoComProblemaIntegracaoGrMotoristaVeiculo)
                    Servicos.Auditoria.Auditoria.Auditar(Auditado, carga, carga.GetChanges(), "Salvou Dados do Transporte da Carga liberando com rejeição na GR", unitOfWork);
                else if (carga.LicencaInvalida && carga.LiberadaComLicencaInvalida)
                    Servicos.Auditoria.Auditoria.Auditar(Auditado, carga, carga.GetChanges(), "Salvou Dados do Transporte da Carga. (Licença inválida, configurado para não bloquear)", unitOfWork);
                else
                    Servicos.Auditoria.Auditoria.Auditar(Auditado, carga, carga.GetChanges(), "Salvou Dados do Transporte da Carga", unitOfWork);

                bool salvouDadosTransporteSemVinculoPreCarga = Request.GetBoolParam("SalvarDadosTransporteCargaSemVinculoPreCarga");

                if (salvouDadosTransporteSemVinculoPreCarga)
                    Servicos.Auditoria.Auditoria.Auditar(Auditado, carga, "Salvou dados do transporte da carga sem o vínculo com a pré carga.", unitOfWork);

                Servicos.Embarcador.Carga.CargaDatas servicoCargaDatas = new Servicos.Embarcador.Carga.CargaDatas(ConfiguracaoEmbarcador, unitOfWork);

                servicoCargaDatas.SalvarDataSalvamentoDadosTransporte(carga);


                List<Dominio.Entidades.Auditoria.HistoricoPropriedade> alterouDadosDaCarga = carga.GetChanges();

                Repositorio.Embarcador.Cargas.CargaDadosTransporteIntegracao repositorioCargaPedido = new Repositorio.Embarcador.Cargas.CargaDadosTransporteIntegracao(unitOfWork, cancellationToken);

                List<Dominio.Entidades.Embarcador.Cargas.CargaDadosTransporteIntegracao> integracoesdadosTransporte = await repositorioCargaPedido.BuscarPorCargaAsync(carga.Codigo);

                Dominio.Entidades.Veiculo veiculo = carga.Veiculo;
                Dominio.Entidades.Veiculo reboque = carga.VeiculosVinculados?.ElementAtOrDefault(0);
                Dominio.Entidades.Veiculo segundoReboque = carga.VeiculosVinculados?.ElementAtOrDefault(1);
                Dominio.Entidades.Veiculo terceiroReboque = carga.VeiculosVinculados?.ElementAtOrDefault(2) != null ? carga.VeiculosVinculados?.ElementAtOrDefault(2) : null;

                if (alterouDadosDaCarga.Count() > 0)
                {
                    List<Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao> lstTipos = new List<Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao>
                        {
                            Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.OpenTech,
                            Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.ArcelorMittal
                        };

                    if (configuracaoIntegracao.ReenviarIntegracaoDadosTransporteAoAlterarDadosTransporteRaster)
                        lstTipos.Add(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Raster);

                    Servicos.Embarcador.Integracao.OpenTech.IntegracaoCargaOpenTech servicoIntegracaoCargaOpenTech = new Servicos.Embarcador.Integracao.OpenTech.IntegracaoCargaOpenTech(unitOfWork);

                    string cpfMotoristaNovo = string.Empty;

                    if (configuracaoIntegracao.AtualizarVeiculoMotoristaOpentech)
                        cpfMotoristaNovo = await repositorioCargaMotorista.BuscarPrimeiroCPFMotoristasPorCargaAsync(carga.Codigo);

                    foreach (Dominio.Entidades.Embarcador.Cargas.CargaDadosTransporteIntegracao cargaDadosTransporteIntegracao in integracoesdadosTransporte)
                    {
                        if (configuracaoIntegracao.AtualizarVeiculoMotoristaOpentech && !string.IsNullOrWhiteSpace(cargaDadosTransporteIntegracao.Protocolo) && cargaDadosTransporteIntegracao.TipoIntegracao.Tipo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.OpenTech)
                        {
                            servicoIntegracaoCargaOpenTech.AtualizarVeiculoMotoristaDadosTransporte(cargaDadosTransporteIntegracao, cpfMotoristaNovo, cpfMotoristaAnterior, veiculo?.Placa, placaTracaoAnterior, placaReboqueAnterior, reboque?.Placa, placaReboque2Anterior, segundoReboque?.Placa);
                            continue;
                        }

                        if (lstTipos.Contains(cargaDadosTransporteIntegracao.TipoIntegracao.Tipo))
                        {
                            cargaDadosTransporteIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.AgIntegracao;
                            await repositorioCargaPedido.AtualizarAsync(cargaDadosTransporteIntegracao);
                        }
                    }
                }

                if (carga.ProblemaIntegracaoGrMotoristaVeiculo && !string.IsNullOrWhiteSpace(carga.MensagemProblemaIntegracaoGrMotoristaVeiculo) && (!carga.LiberadoComProblemaIntegracaoGrMotoristaVeiculo || ConfiguracaoEmbarcador.NaoBloquearCargaComProblemaIntegracaoGrMotoristaVeiculo))
                    retorno.MensagemProblemaIntegracaoGrMotoristaVeiculo = "Retorno GR: " + carga.MensagemProblemaIntegracaoGrMotoristaVeiculo.Replace("\n", "");

                retorno.LicencaInvalida = carga.LicencaInvalida;
                retorno.MensagemLicencaInvalida = "";
                retorno.LiberarComLicencaInvalida = false;

                if (carga.LicencaInvalida)
                {
                    retorno.LiberarComLicencaInvalida = permissoesPersonalizadas.Contains(AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada.Carga_PermiteAvancarLicencaInvalida);
                    Repositorio.Embarcador.Cargas.CargaLicenca repositorioCargaLicenca = new Repositorio.Embarcador.Cargas.CargaLicenca(unitOfWork, cancellationToken);
                    Dominio.Entidades.Embarcador.Cargas.CargaLicenca cargaLicenca = await repositorioCargaLicenca.BuscarPorCargaAsync(carga.Codigo);
                    retorno.MensagemLicencaInvalida = !string.IsNullOrWhiteSpace(cargaLicenca?.Mensagem) ? cargaLicenca.Mensagem : "Licença inválida";
                }

                Dominio.Entidades.Embarcador.Filiais.ConfiguracaoGestaoPatio configuracaoGestaoPatio = repositorioConfiguracaoGestaoPatio.BuscarConfiguracao();

                if ((configuracaoGestaoPatio?.ChegadaVeiculoPermiteAvancarAutomaticamenteAposInformarDadosTransporteCarga ?? false) && TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiEmbarcador)
                {
                    Servicos.Embarcador.GestaoPatio.FluxoGestaoPatio servicoFluxoGestaoPatio = new Servicos.Embarcador.GestaoPatio.FluxoGestaoPatio(unitOfWork, Auditado, Cliente);
                    Repositorio.Embarcador.GestaoPatio.FluxoGestaoPatio repositorioFluxoGestaoPatio = new Repositorio.Embarcador.GestaoPatio.FluxoGestaoPatio(unitOfWork);
                    Dominio.Entidades.Embarcador.GestaoPatio.FluxoGestaoPatio fluxoGestaoPatio = await repositorioFluxoGestaoPatio.BuscarPorCargaETipoAsync(carga.Codigo, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoFluxoGestaoPatio.Origem);

                    servicoFluxoGestaoPatio.AvancarEtapa(fluxoGestaoPatio, Dominio.ObjetosDeValor.Embarcador.Enumeradores.EtapaFluxoGestaoPatio.ChegadaVeiculo);

                    await unitOfWork.CommitChangesAsync();
                }

                if (configuracaoGeralCarga.InformarDocaNaEtapaUmDaCarga)
                {
                    Servicos.Embarcador.GestaoPatio.InformarDoca servicoInformarDoca = new Servicos.Embarcador.GestaoPatio.InformarDoca(unitOfWork, Auditado, Cliente);

                    Dominio.ObjetosDeValor.Embarcador.GestaoPatio.InformarDocaSalvarDadosTransporteCarga informarDocaSalvarDadosTransporteCarga = new Dominio.ObjetosDeValor.Embarcador.GestaoPatio.InformarDocaSalvarDadosTransporteCarga()
                    {
                        CodigoCarga = carga.Codigo,
                        CodigoLocalCarregamento = Request.GetIntParam("LocalCarregamento")
                    };

                    servicoInformarDoca.AtualizarDocaDadosTransporteCarga(informarDocaSalvarDadosTransporteCarga);
                }

                if (!carga.LiberadoComProblemaIntegracaoGrMotoristaVeiculo)
                    retorno.ProblemaIntegracaoGrMotoristaVeiculo = carga.ProblemaIntegracaoGrMotoristaVeiculo;

                retorno.PermitirLiberarComProblemaIntegracaoGrMotoristaVeiculo = Usuario.UsuarioAdministrador || permissoesPersonalizadas.Contains(AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada.Carga_ConfirmarIntegracao);

                Repositorio.Embarcador.Cargas.CargaVeiculoContainer repositorioCargaVeiculoContainer = new Repositorio.Embarcador.Cargas.CargaVeiculoContainer(unitOfWork);
                List<Dominio.Entidades.Embarcador.Cargas.CargaVeiculoContainer> cargaVeiculosContainer = await repositorioCargaVeiculoContainer.BuscarPorCargaAsync(carga.Codigo);
                Dominio.Entidades.Embarcador.Cargas.CargaVeiculoContainer containerVeiculo = (from cargaVeiculoContainer in cargaVeiculosContainer where cargaVeiculoContainer.Veiculo.Codigo == veiculo?.Codigo select cargaVeiculoContainer).FirstOrDefault();
                Dominio.Entidades.Embarcador.Cargas.CargaVeiculoContainer containerReboque = (from cargaVeiculoContainer in cargaVeiculosContainer where cargaVeiculoContainer.Veiculo.Codigo == reboque?.Codigo select cargaVeiculoContainer).FirstOrDefault();
                Dominio.Entidades.Embarcador.Cargas.CargaVeiculoContainer containerSegundoReboque = (from cargaVeiculoContainer in cargaVeiculosContainer where cargaVeiculoContainer.Veiculo.Codigo == segundoReboque?.Codigo select cargaVeiculoContainer).FirstOrDefault();
                Dominio.Entidades.Embarcador.Cargas.CargaVeiculoContainer containerTerceiroReboque = (from cargaVeiculoContainer in cargaVeiculosContainer where cargaVeiculoContainer.Veiculo.Codigo == terceiroReboque?.Codigo select cargaVeiculoContainer).FirstOrDefault();



                if (carga.SituacaoCarga == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.AgNFe && (carga.TipoOperacao?.EnviarEmailPlanoViagemSolicitarNotasCarga ?? false))
                {
                    Servicos.Embarcador.Carga.Impressao servicoImpressao = new Servicos.Embarcador.Carga.Impressao(unitOfWork);
                    servicoImpressao.EnviarPlanoViagemParaDestinatariosPorEmail(carga, "Aviso de Carregamento");
                }

                if (carga.PossuiPendencia && (carga.AguardarIntegracaoEtapaTransportador || carga.AguardarIntegracaoDadosTransporte))
                    throw new ControllerException("Existem integrações pendentes, favor verificar.");

                svcCarga.GerarNotificacaoEmailFornecedorDadosTransporte(carga, unitOfWork);

                svcCarga.ConfirmarPendenciasTipoOperacao(carga, unitOfWork);
                svcCarga.ObterDadosVeiculosEMotoristasExternal1(ref carga, unitOfWork);
                svcCarga.VincularDocumentoContainerVazio(carga, unitOfWork, Auditado, TipoServicoMultisoftware, configuracaoTMS);

                //svcCarga.RemoverPedidosDaMontagemCarga(new List<Dominio.Entidades.Embarcador.Cargas.Carga>() { carga }, repCargaPedido.BuscarPedidosPorCarga(carga.Codigo), unitOfWork);

                ProcessarIntegracoesAgRetornoValePedagio(carga, unitOfWork);

                Servicos.Embarcador.Hubs.Carga serHubCarga = new Servicos.Embarcador.Hubs.Carga();
                serHubCarga.InformarCargaAtualizada(carga.Codigo, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoAcaoCarga.Alterada, unitOfWork.StringConexao);
                Dominio.Entidades.Embarcador.Cargas.TipoIntegracao tipoIntegracao = new Repositorio.Embarcador.Cargas.TipoIntegracao(unitOfWork).BuscarPorTipo(TipoIntegracao.HUB);

                if (tipoIntegracao != null)
                {
                    Servicos.Embarcador.Integracao.HUB.IntegracaoHUBOfertas servicoIntegracaoHUBOfertas = new Servicos.Embarcador.Integracao.HUB.IntegracaoHUBOfertas(unitOfWork, TipoServicoMultisoftware);
                    bool retornoAdicionarIntegracao = servicoIntegracaoHUBOfertas.AdicionarIntegracaoHUB(carga, tipoIntegracao).GetAwaiter().GetResult();
                }

                if (configuracaoGeralCarga.NaoPermitirAtribuirVeiculoCargaSeExistirMonitoramentoAtivoParaPlaca)
                {
                    Repositorio.Embarcador.Logistica.Monitoramento repMonitoramento = new Repositorio.Embarcador.Logistica.Monitoramento(unitOfWork);
                    var monitoramentosAtivos = await repMonitoramento.BuscarMonitoramentoEmAbertoPorVeiculoPlacaAsync(carga.Veiculo.Placa);

                    if (monitoramentosAtivos.Any())
                    {
                        throw new ControllerException("Veículo está em um monitoramento ativo e não poderá ser atribuído a uma carga.");
                    }
                }

                try
                {
                    string erros = string.Empty;
                    serHistorico.InserirHistoricoVinculo(unitOfWork, ref erros, Dominio.ObjetosDeValor.Embarcador.Enumeradores.LocalVinculo.Carga, carga.Veiculo, carga.VeiculosVinculados, carga.Motoristas, DateTime.Now, null, carga?.Pedidos?.FirstOrDefault().Pedido, carga);
                }
                catch (Exception ex)
                {
                    Servicos.Log.TratarErro(ex);
                }

                return new JsonpResult(new
                {
                    AlterouVeiculoEmParqueamento = false,
                    DadosFrete = retorno,
                    DadosContainer = new
                    {
                        CodigoContainerReboque = containerReboque?.Codigo ?? 0,
                        CodigoContainerSegundoReboque = containerSegundoReboque?.Codigo ?? 0,
                        CodigoContainerVeiculo = containerVeiculo?.Codigo ?? 0
                    }
                });
            }
            catch (BaseException excecao)
            {
                await unitOfWork.RollbackAsync();

                if (excecao.ErrorCode == Dominio.ObjetosDeValor.Enumerador.CodigoExcecao.VeiculosInvalidosCarga)
                    return new JsonpResult(new
                    {
                        Mensagem = excecao.Message,
                        VeiculosInvalidosCarga = true
                    }, false, excecao.Message);

                return new JsonpResult(false, true, excecao.Message);
            }
            catch (Exception ex)
            {
                await unitOfWork.RollbackAsync();

                Servicos.Log.TratarErro(ex);

                return new JsonpResult(false, "Ocorreu uma falha ao atualizar a carga.");
            }
            finally
            {
                await unitOfWork.DisposeAsync();
            }
        }

        public async Task<IActionResult> SalvarDadosTransporteCargaListaAsync(CancellationToken cancellationToken)
        {
            {
                Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

                try
                {
                    Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unitOfWork, cancellationToken);
                    Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork, cancellationToken);
                    Repositorio.Embarcador.Cargas.CargaDadosTransporteIntegracao repCargaDadosTransporteIntegracao = new Repositorio.Embarcador.Cargas.CargaDadosTransporteIntegracao(unitOfWork, cancellationToken);
                    Repositorio.Embarcador.Cargas.CargaPedido repCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(unitOfWork, cancellationToken);
                    Repositorio.Embarcador.Filiais.ConfiguracaoGestaoPatio repositorioConfiguracaoGestaoPatio = new Repositorio.Embarcador.Filiais.ConfiguracaoGestaoPatio(unitOfWork, cancellationToken);

                    Servicos.Embarcador.Carga.Carga svcCarga = new Servicos.Embarcador.Carga.Carga(unitOfWork);

                    List<AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada> permissoesPersonalizadas = ObterPermissoesPersonalizadas("Cargas/Carga", "Logistica/JanelaCarregamento");

                    List<int> listaCargas = new List<int>() { 202984 };

                    for (var i = 0; i < listaCargas.Count; i++)
                    {
                        Dominio.Entidades.Embarcador.Cargas.Carga carga = await repCarga.BuscarPorCodigoAsync(listaCargas[i]);

                        if (carga.SituacaoCarga == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.Nova)
                        {

                            carga.Initialize();

                            Dominio.ObjetosDeValor.Embarcador.Carga.CargaDadosTransporte dadosTransporte = new Dominio.ObjetosDeValor.Embarcador.Carga.CargaDadosTransporte()
                            {
                                Carga = carga,
                                CodigoEmpresa = carga.Empresa.Codigo,
                                CodigoModeloVeicular = 0,
                                CodigoReboque = 0,
                                CodigoSegundoReboque = 0,
                                CodigoTerceiroReboque = 0,
                                CodigoPedidoViagemNavio = 0,
                                CodigoPortoDestino = 0,
                                CodigoPortoOrigem = 0,
                                CodigoTerminalDestino = 0,
                                CodigoTerminalOrigem = 0,
                                CodigoTipoCarga = carga.TipoDeCarga.Codigo,
                                CodigoTipoOperacao = carga.TipoOperacao.Codigo,
                                CodigoTracao = 0,
                                SalvarDadosTransporteSemSolicitarNFes = false
                            };

                            string mensagemErro;
                            var retorno = svcCarga.SalvarDadosTransporteCarga(dadosTransporte, out mensagemErro, Usuario, true, TipoServicoMultisoftware, WebServiceConsultaCTe, Cliente, Auditado, unitOfWork);

                            if (retorno == null)
                                Servicos.Log.TratarErro("Carga " + listaCargas[i].ToString() + " " + mensagemErro);
                        }

                        unitOfWork.FlushAndClear();
                    }

                    return new JsonpResult(true);
                }
                catch (Exception ex)
                {
                    await unitOfWork.RollbackAsync();

                    Servicos.Log.TratarErro(ex);

                    return new JsonpResult(false, "Ocorreu uma falha ao atualizar a carga.");
                }
                finally
                {
                    await unitOfWork.DisposeAsync();
                }
            }
        }

        public async Task<IActionResult> VerificarTipoImportacaoDocumentosAsync(CancellationToken cancellationToken)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Repositorio.Embarcador.Cargas.Carga repositorioCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork, cancellationToken);
                Dominio.Entidades.Embarcador.Cargas.Carga existeCarga = await repositorioCarga.BuscarPorCodigoAsync(Request.GetIntParam("Codigo"));
                if (existeCarga == null)
                    return new JsonpResult(false);

                return new JsonpResult(existeCarga?.TipoOperacao?.PermiteImportarDocumentosManualmente ?? false);
            }
            catch (Exception ex)
            {
                Log.TratarErro(ex);
                return new JsonpResult(false);
            }
        }

        public async Task<IActionResult> IntegracaoDigitamcomComProblemaAsync(CancellationToken cancellationToken)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Repositorio.Embarcador.Cargas.CargaDadosTransporteIntegracao repositorioCargaDadosTransporteIntegracoa = new Repositorio.Embarcador.Cargas.CargaDadosTransporteIntegracao(unitOfWork, cancellationToken);
                List<Dominio.Entidades.Embarcador.Cargas.CargaDadosTransporteIntegracao> integracaoesDigitalCom = await repositorioCargaDadosTransporteIntegracoa.BuscarPorCargaAsync(Request.GetIntParam("Codigo"));

                bool existeIntegracaoDigitalCom = integracaoesDigitalCom.Count > 0 ? integracaoesDigitalCom.Any(obj => obj.TipoIntegracao.Tipo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.DigitalCom && obj.Carga.TAGPedagio != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TAGPedagio.Valida) : false;

                return new JsonpResult(new
                {
                    PossuiIntegracaoComProblema = existeIntegracaoDigitalCom
                });
            }
            catch (Exception ex)
            {
                await unitOfWork.RollbackAsync();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao verificar a carga.");
            }
            finally
            {
                await unitOfWork.DisposeAsync();
            }

        }

        public async Task<IActionResult> ObterVeiculosVinculadosAsync(CancellationToken cancellationToken)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork, cancellationToken);
                Repositorio.Veiculo repVeiculo = new Repositorio.Veiculo(unitOfWork, cancellationToken);

                int codigoCarga = Request.GetIntParam("Codigo");
                int codigoVeiculo = Request.GetIntParam("Veiculo");

                Dominio.Entidades.Embarcador.Cargas.Carga carga = await repCarga.BuscarPorCodigoAsync(codigoCarga);
                Dominio.Entidades.Veiculo veiculo = await repVeiculo.BuscarPorCodigoAsync(codigoVeiculo);

                return new JsonpResult(ObterGridPlacaCarregamento(veiculo));
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao alterar os dados do pedido.");
            }
            finally
            {
                await unitOfWork.DisposeAsync();
            }
        }
        public async Task<IActionResult> ObterVeiculosVinculadosSelecionadosAsync(CancellationToken cancellation)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork, cancellation);


                int codigoCarga = Request.GetIntParam("Codigo");
                Dominio.Entidades.Embarcador.Cargas.Carga carga = await repCarga.BuscarPorCodigoAsync(codigoCarga);

                var lista = (from obj in carga.VeiculosCarregamento
                             select new
                             {
                                 obj.Codigo,

                             }).ToList();

                return new JsonpResult(lista);
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao alterar os dados do pedido.");
            }
            finally
            {
                await unitOfWork.DisposeAsync();
            }
        }

        #endregion

        #region Métodos Privados
        private void ValidarPreenchimentoPlacaCarregamento(ref Dominio.Entidades.Embarcador.Cargas.Carga carga, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Veiculo repVeiculo = new Repositorio.Veiculo(unitOfWork);
            Repositorio.Embarcador.Pedidos.Pedido repPedido = new Repositorio.Embarcador.Pedidos.Pedido(unitOfWork);

            if (carga.Veiculo?.VeiculosVinculados != null && carga.Veiculo?.VeiculosVinculados.Count > 0)
            {
                bool encontrouSelecao = false;
                List<dynamic> PlacasCarregamento = Request.GetListParam<dynamic>("PlacasCarregamento");
                List<Dominio.Entidades.Embarcador.Pedidos.Pedido> ListaPedido = repPedido.BuscarPedidosPorCarga(carga.Codigo);

                carga.VeiculosCarregamento.Clear();
                foreach (var placa in PlacasCarregamento)
                {
                    int codigoVeiculo = ((string)placa.Codigo).ToInt();

                    Dominio.Entidades.Veiculo veiculo = repVeiculo.BuscarPorCodigo(codigoVeiculo);
                    if (veiculo != null)
                    {
                        if (!carga.VeiculosCarregamento.Any(x => x.Codigo == codigoVeiculo))
                        {
                            carga.VeiculosCarregamento.Add(veiculo);
                        }

                        if (ListaPedido != null && ListaPedido.Count > 0)
                        {
                            foreach (var pedido in ListaPedido)
                            {
                                if (!pedido.VeiculosCarregamento.Any(x => x.Codigo == codigoVeiculo))
                                {
                                    pedido.VeiculosCarregamento.Add(veiculo);
                                    repPedido.Atualizar(pedido);
                                }
                            }
                        }
                        encontrouSelecao = true;

                    }
                }

                if (!encontrouSelecao)
                    throw new ServicoException("Necessário selecionar uma placa para carregamento");
            }
        }
        private bool ShoulIntegrarAntesDePersistir(Dominio.Entidades.Embarcador.Cargas.Carga carga, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Cargas.CargaCargaIntegracao repCargaCargaIntegracao = new Repositorio.Embarcador.Cargas.CargaCargaIntegracao(unitOfWork);

            Dominio.Entidades.Embarcador.Cargas.CargaCargaIntegracao cargaIntegracao = repCargaCargaIntegracao.BuscarPorCargaETipoIntegracao(carga.Codigo, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.SaintGobain);

            return cargaIntegracao != null;
        }

        private void IntegrarDadosERP(Dominio.Entidades.Embarcador.Cargas.Carga carga, Dominio.ObjetosDeValor.Embarcador.Carga.CargaDadosTransporte dadosTrans, Repositorio.UnitOfWork unitOfWork)
        {
            /**
             * Só permite persistir os dados quando ocorre integração com sucesso ao ERP
             */

            if (!carga.CarregamentoIntegradoERP)
                return;

            Repositorio.Embarcador.Cargas.CargaDadosTransporteIntegracao repositorioCargaDadosTransporteIntegracao = new Repositorio.Embarcador.Cargas.CargaDadosTransporteIntegracao(unitOfWork);
            Dominio.Entidades.Embarcador.Cargas.CargaDadosTransporteIntegracao cargaDadosTransporteIntegracao = repositorioCargaDadosTransporteIntegracao.BuscarPorCargaETipoIntegracao(carga.Codigo, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.SaintGobain);

            if (cargaDadosTransporteIntegracao == null)
                return;

            new Servicos.Embarcador.Integracao.SaintGobain.IntegracaoSaintGobain(unitOfWork).IntegrarCarga(cargaDadosTransporteIntegracao, dadosTrans, TipoServicoMultisoftware);

            if (cargaDadosTransporteIntegracao.SituacaoIntegracao != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.Integrado)
            {
                if (Utilidades.String.RemoveAccents(cargaDadosTransporteIntegracao.ProblemaIntegracao.ToLower()) != "documento de transporte nao encontrado")
                    throw new ControllerException(cargaDadosTransporteIntegracao.ProblemaIntegracao);
            }
        }

        private string ValidarDadosJanelaCarregamento(Dominio.Entidades.Embarcador.Cargas.Carga carga, Dominio.ObjetosDeValor.Embarcador.Carga.CargaDadosTransporte dadosTrans, Repositorio.UnitOfWork unitOfWork)
        {
            Servicos.Embarcador.Logistica.JanelaCarregamentoTransportadorValidacoes servicoJanelaCarregamentoTransportadorValidacoes = new Servicos.Embarcador.Logistica.JanelaCarregamentoTransportadorValidacoes(unitOfWork, TipoServicoMultisoftware, ConfiguracaoEmbarcador);

            Repositorio.Embarcador.Cargas.CargaJanelaCarregamento repCargaJanelaCarregamento = new Repositorio.Embarcador.Cargas.CargaJanelaCarregamento(unitOfWork);
            Repositorio.Veiculo repVeiculo = new Repositorio.Veiculo(unitOfWork);
            Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unitOfWork);
            Repositorio.Usuario repUsuario = new Repositorio.Usuario(unitOfWork);
            Repositorio.Embarcador.Logistica.PosicaoAtual repPosicaoAtual = new Repositorio.Embarcador.Logistica.PosicaoAtual(unitOfWork);

            Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamento cargaJanelaCarregamento = repCargaJanelaCarregamento.BuscarPorCarga(carga.Codigo);
            Dominio.Entidades.Veiculo veiculo = repVeiculo.BuscarPorCodigo(dadosTrans.CodigoTracao);
            Dominio.Entidades.Empresa empresa = repEmpresa.BuscarPorCodigo(dadosTrans.CodigoEmpresa);
            Dominio.Entidades.Usuario motorista = repUsuario.BuscarPorCodigo(dadosTrans.CodigoMotorista);

            string mensagemRetorno = "";

            List<Dominio.Entidades.Veiculo> veiculosDaCarga = new List<Dominio.Entidades.Veiculo>();

            if (veiculo != null)
                veiculosDaCarga.Add(veiculo);

            if (carga.VeiculosVinculados != null && carga.VeiculosVinculados.Count > 0)
                veiculosDaCarga.AddRange(carga.VeiculosVinculados);

            servicoJanelaCarregamentoTransportadorValidacoes.ValidarDadosViaIntegracao(carga, veiculosDaCarga, empresa, out mensagemRetorno);

            if (cargaJanelaCarregamento != null)
            {
                servicoJanelaCarregamentoTransportadorValidacoes.ValidarLimiteCargasParaMotorista(cargaJanelaCarregamento, motorista);
                servicoJanelaCarregamentoTransportadorValidacoes.ValidarLimiteCargasParaVeiculo(cargaJanelaCarregamento, veiculo);

                if (veiculo != null)
                {
                    Dominio.Entidades.Embarcador.Logistica.PosicaoAtual posicaoAtual = repPosicaoAtual.BuscarPorVeiculo(veiculo.Codigo);
                    if ((cargaJanelaCarregamento.CentroCarregamento?.BloquearVeiculoSemEspelhamentoTelaCarga ?? false))
                        servicoJanelaCarregamentoTransportadorValidacoes.ValidarEspelhamentoVeiculo(veiculo, posicaoAtual);
                }
            }

            if ((carga.TipoOperacao?.ConfiguracaoTransportador?.BloquearVeiculoSemEspelhamento ?? false) && veiculo != null)
            {
                Dominio.Entidades.Embarcador.Logistica.PosicaoAtual posicaoAtual = repPosicaoAtual.BuscarPorVeiculo(veiculo.Codigo);
                servicoJanelaCarregamentoTransportadorValidacoes.ValidarEspelhamentoVeiculo(veiculo, posicaoAtual);
            }
            return mensagemRetorno;
        }

        private void ProcessarIntegracoesAgRetornoValePedagio(Dominio.Entidades.Embarcador.Cargas.Carga carga, Repositorio.UnitOfWork unitOfWork)
        {
            if (carga.Veiculo == null)
                return;

            Repositorio.Embarcador.Cargas.CargaDadosTransporteIntegracao repositoriCargaDadosTransporte = new Repositorio.Embarcador.Cargas.CargaDadosTransporteIntegracao(unitOfWork);
            Dominio.Entidades.Embarcador.Cargas.CargaDadosTransporteIntegracao existeIntegracaoDadosTransporte = repositoriCargaDadosTransporte.BuscarIntegracaoAguardandoPorCarga(carga.Codigo);

            if (existeIntegracaoDadosTransporte == null)
                return;

            existeIntegracaoDadosTransporte.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.AgIntegracao;
            existeIntegracaoDadosTransporte.ProblemaIntegracao = "";
            existeIntegracaoDadosTransporte.NumeroTentativas++;
            repositoriCargaDadosTransporte.Atualizar(existeIntegracaoDadosTransporte);
        }

        private void ProcessarIntegracoesAgRetornoBoticario(Dominio.Entidades.Embarcador.Cargas.Carga carga, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Cargas.CargaDadosTransporteIntegracao repositoriCargaDadosTransporte = new Repositorio.Embarcador.Cargas.CargaDadosTransporteIntegracao(unitOfWork);
            Dominio.Entidades.Embarcador.Cargas.CargaDadosTransporteIntegracao existeIntegracaoDadosTransporteBoticario = repositoriCargaDadosTransporte.BuscarIntegracaoBoticario(carga.Codigo);

            if (existeIntegracaoDadosTransporteBoticario == null)
                return;

            existeIntegracaoDadosTransporteBoticario.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.AgIntegracao;
            existeIntegracaoDadosTransporteBoticario.ProblemaIntegracao = "";
            existeIntegracaoDadosTransporteBoticario.NumeroTentativas++;
            repositoriCargaDadosTransporte.Atualizar(existeIntegracaoDadosTransporteBoticario);
        }

        private bool VerificarAlteracaoDeMotoristasOuVeiculos(Dominio.Entidades.Embarcador.Cargas.Carga carga, Dominio.ObjetosDeValor.Embarcador.Carga.CargaDadosTransporte dadosTransporte)
        {
            var idsMotoristasAtuais = carga.Motoristas?.Select(s => s.Codigo).ToList() ?? new List<int>();
            if (!idsMotoristasAtuais.SequenceEqual(dadosTransporte.ListaCodigoMotorista) || carga.Veiculo?.Codigo != dadosTransporte.CodigoTracao)
                return true;

            return false;
        }

        private bool VerificarAlteracaoDeReboques(Dominio.Entidades.Embarcador.Cargas.Carga carga, Dominio.ObjetosDeValor.Embarcador.Carga.CargaDadosTransporte dadosTransporte)
        {
            var reboquesCarga = carga.VeiculosVinculados?.Select(s => s.Codigo).ToList() ?? new List<int>();
            List<int> reboques = new List<int>();

            if (dadosTransporte.CodigoReboque > 0)
                reboques.Add(dadosTransporte.CodigoReboque);
            if (dadosTransporte.CodigoSegundoReboque > 0)
                reboques.Add(dadosTransporte.CodigoSegundoReboque);
            if (dadosTransporte.CodigoTerceiroReboque > 0)
                reboques.Add(dadosTransporte.CodigoTerceiroReboque);

            if (reboquesCarga.SequenceEqual(reboques))
                return false;
            return true;
        }

        private Models.Grid.Grid ObterGridPlacaCarregamento(Dominio.Entidades.Veiculo veiculo)
        {
            try
            {
                Models.Grid.Grid grid = new Models.Grid.Grid(Request)
                {
                    header = new List<Models.Grid.Head>()
                };

                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("Placa", "Descricao", 6, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("N° Frota", "NumeroFrota", 6, Models.Grid.Align.left, false);

                int totalRegistros = veiculo?.VeiculosVinculados?.Count() ?? 0;

                var lista = totalRegistros > 0 ? (from obj in veiculo?.VeiculosVinculados
                                                  select new
                                                  {

                                                      obj.Codigo,
                                                      obj.Descricao,
                                                      NumeroFrota = obj.NumeroFrota ?? "",

                                                  }).ToList() : null;

                grid.AdicionaRows(lista);
                grid.setarQuantidadeTotal(totalRegistros);

                return grid;
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                throw;
            }
        }
        #endregion
    }
}