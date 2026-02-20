/// <reference path="../../../js/libs/jquery-2.1.1.js" />
/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/bootstrap/bootstrap.js" />
/// <reference path="../../../js/libs/jquery.blockui.js" />
/// <reference path="../../../js/Global/knoutViewsSlides.js" />
/// <reference path="../../../js/libs/jquery.maskMoney.js" />
/// <reference path="../../../js/plugin/datatables/jquery.dataTables.js" />
/// <reference path="../../Consultas/TipoOcorrencia.js" />
/// <reference path="../../Consultas/PagamentoMotoristaTipo.js" />
/// <reference path="../../Consultas/Justificativa.js" />
/// <reference path="../../Consultas/Cliente.js" />
/// <reference path="../../Consultas/GrupoMotivoChamado.js" />
/// <reference path="../../Enumeradores/EnumFinalidadeTipoOcorrencia.js" />
/// <reference path="../../Enumeradores/EnumTipoMotivoAtendimento.js" />
/// <reference path="../../Enumeradores/EnumTiposFreeTime.js" />
/// <reference path="../../Enumeradores/EnumTipoFinalidadeJustificativa.js" />
/// <reference path="../../Enumeradores/EnumTipoProprietarioVeiculo.js" />
/// <reference path="../../Enumeradores/EnumTipoParametroCriticidade.js" />
/// <reference path="MotivoChamadoData.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _motivoChamado;
var _crudMotivoChamado;
var _pesquisaObjeto;
var _gridObjeto;
var _configuracaoTipoMotivoAtendimento;
var _existeGenero, _existeAreaEnvolvida, _fazGestaoCriticidadeAtendimento;

var MotivoChamado = function () {
    // Codigo da entidade
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Descricao = PropertyEntity({ text: Localization.Resources.Chamado.MotivoChamado.Descricao.getRequiredFieldDescription(), issue: 586, required: true, getType: typesKnockout.string, val: ko.observable("") });
    this.CodigoIntegracao = PropertyEntity({ text: Localization.Resources.Gerais.Geral.CodigoIntegracao.getFieldDescription() });
    this.Status = PropertyEntity({ text: Localization.Resources.Chamado.MotivoChamado.Status.getFieldDescription(), issue: 557, val: ko.observable(true), options: _status, def: true });
    this.Observacao = PropertyEntity({ text: Localization.Resources.Chamado.MotivoChamado.Observacao.getFieldDescription(), issue: 593, getType: typesKnockout.string, val: ko.observable("") });
    this.TipoOcorrencia = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), enable: ko.observable(true), required: false, text: Localization.Resources.Chamado.MotivoChamado.TipoOcorrencia.getFieldDescription(), idBtnSearch: guid(), issue: 410, tipoEmissaoDocumentoOcorrencia: EnumTipoEmissaoDocumentoOcorrencia.Todos });
    this.TipoMotivoAtendimento = PropertyEntity({ val: ko.observable(_configuracaoTipoMotivoAtendimento[0].value), options: _configuracaoTipoMotivoAtendimento, text: Localization.Resources.Chamado.MotivoChamado.Tipo.getFieldDescription(), def: _configuracaoTipoMotivoAtendimento[0].value, required: ko.observable(true), visible: ko.observable(true), enable: ko.observable(true) });
    this.Genero = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Chamado.MotivoChamado.Genero.getFieldDescription(), idBtnSearch: guid(), visible: ko.observable(false) });
    this.AreaEnvolvida = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Chamado.MotivoChamado.AreaEnvolvida.getFieldDescription(), idBtnSearch: guid(), visible: ko.observable(false) });
    this.GrupoMotivoChamado = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Chamado.MotivoChamado.GrupoMotivoChamado.getFieldDescription(), idBtnSearch: guid(), visible: ko.observable(_CONFIGURACAO_TMS.ExisteGrupoMotivoChamado) });
    this.TipoQuebraRegraPallet = PropertyEntity({ val: ko.observable(EnumTipoQuebraRegra.Padrao), options: EnumTipoQuebraRegra.obterOpcoes(), def: EnumTipoQuebraRegra.Padrao, text: Localization.Resources.Chamado.MotivoChamado.TipoQuebraRegra.getFieldDescription(), visible: ko.observable(false) });
    this.NumeroCriticidadeAtendimento = PropertyEntity({ val: ko.observable(0), def: 0, text: Localization.Resources.Chamado.MotivoChamado.NumeroCriticidadeAtendimento.getFieldDescription(), getType: typesKnockout.int, visible: ko.observable(false) });

    //Campos booleanos
    //this.Devolucao = PropertyEntity({ val: ko.observable(false), getType: typesKnockout.bool, text: "Este motivo é para atendimento de devolução?", def: false, visible: ko.observable(true) });
    this.ExigeValor = PropertyEntity({ val: ko.observable(false), getType: typesKnockout.bool, text: Localization.Resources.Chamado.MotivoChamado.EsteMotivoObrigaInformarValorAtendimento, def: false, visible: ko.observable(true) });
    this.ChamadoDeveSerAbertoPeloEmbarcador = PropertyEntity({ val: ko.observable(false), getType: typesKnockout.bool, text: Localization.Resources.Chamado.MotivoChamado.EsteMotivoSoPodeSerUtilizadoParaAtendimentoAbertoEmbarcador, def: false, visible: ko.observable(true) });
    this.GerarCargaDevolucaoSeAprovado = PropertyEntity({ val: ko.observable(false), getType: typesKnockout.bool, text: Localization.Resources.Chamado.MotivoChamado.GerarCargaDevolucaoAutomaticamenteAtendimentoAprovado, def: false, visible: ko.observable(true) });
    this.GerarValePalletSeAprovado = PropertyEntity({ val: ko.observable(false), getType: typesKnockout.bool, text: Localization.Resources.Chamado.MotivoChamado.GerarUmValePalletAutomaticamenteParaAtendimentoAprovadoComEsteMotivo, def: false, visible: ko.observable(true) });
    this.ExigeFotoAbertura = PropertyEntity({ val: ko.observable(false), getType: typesKnockout.bool, text: Localization.Resources.Chamado.MotivoChamado.ExigeFotoAbertura, def: false, visible: ko.observable(false) });
    this.ExigeQRCodeAbertura = PropertyEntity({ val: ko.observable(false), getType: typesKnockout.bool, text: Localization.Resources.Chamado.MotivoChamado.ExigeQRCodeAbertura, def: false, visible: ko.observable(false) });
    this.GerarOcorrenciaAutomaticamente = PropertyEntity({ val: ko.observable(false), getType: typesKnockout.bool, text: Localization.Resources.Chamado.MotivoChamado.GerarOcorrenciaAutomaticamente, def: false, visible: ko.observable(true) });
    this.GerarCTeComValorIgualCTeAnterior = PropertyEntity({ val: ko.observable(false), getType: typesKnockout.bool, text: Localization.Resources.Chamado.MotivoChamado.GerarCTeComValorIgualCTeAnterior, def: false, visible: ko.observable(false) });
    this.CalcularOcorrenciaPorTabelaFrete = PropertyEntity({ val: ko.observable(false), getType: typesKnockout.bool, text: Localization.Resources.Chamado.MotivoChamado.CalcularOcorrenciaPorTabelaFrete, def: false, visible: ko.observable(false) });
    this.ExigeValorNaLiberacao = PropertyEntity({ val: ko.observable(false), getType: typesKnockout.bool, text: Localization.Resources.Chamado.MotivoChamado.EsteMotivoObrigaInformarValorParaLiberarOcorrencia, def: false, visible: ko.observable(true) });
    this.ObrigarMotoristaInformarMultiMobile = PropertyEntity({ val: ko.observable(false), getType: typesKnockout.bool, text: Localization.Resources.Chamado.MotivoChamado.ObrigarMotoristaInformarMultiMobile, def: false, visible: ko.observable(false) });
    this.DisponibilizaParaReeentrega = PropertyEntity({ val: ko.observable(false), getType: typesKnockout.bool, text: Localization.Resources.Chamado.MotivoChamado.DisponibilizaPedidosParaReentrega, def: false, visible: ko.observable(true) });
    this.ValidarDuplicidade = PropertyEntity({ val: ko.observable(false), getType: typesKnockout.bool, text: Localization.Resources.Chamado.MotivoChamado.ImpedirDuplicidadeCriacaoAtendimentoParaMesmoMotivoCarga, def: false, visible: ko.observable(true) });
    this.PermitirAbrirMaisAtendimentoComMesmoMotivoParaMesmaCarga = PropertyEntity({ val: ko.observable(false), getType: typesKnockout.bool, text: Localization.Resources.Chamado.MotivoChamado.PermitirAbrirMaisAtendimentoComMesmoMotivoParaMesmaCarga, def: false, visible: ko.observable(true) });
    this.ValidarDuplicidadePorDestinatario = PropertyEntity({ val: ko.observable(false), getType: typesKnockout.bool, text: Localization.Resources.Chamado.MotivoChamado.ImpedirDuplicidadeCriacaoAtendimentoParaMesmoMotivoCargaDestinatario, def: false, visible: ko.observable(true) });
    this.ReferentePagamentoDescarga = PropertyEntity({ val: ko.observable(false), getType: typesKnockout.bool, text: Localization.Resources.Chamado.MotivoChamado.ReferentePagamentoDescarga, def: false, visible: ko.observable(false) });
    this.PermiteInformarDesconto = PropertyEntity({ val: ko.observable(false), getType: typesKnockout.bool, text: Localization.Resources.Chamado.MotivoChamado.HabilitarCampoParaInformarDesconto, def: false, visible: ko.observable(true) });
    this.PermiteEstornarAtendimento = PropertyEntity({ val: ko.observable(false), getType: typesKnockout.bool, text: Localization.Resources.Chamado.MotivoChamado.PermiteEstornarReabrirAtendimento, def: false, visible: ko.observable(true) });
    this.ExigeAnaliseParaOperacao = PropertyEntity({ val: ko.observable(false), getType: typesKnockout.bool, text: Localization.Resources.Chamado.MotivoChamado.ExigeQueUmaAnaliseSejaInformadaAntesDoUsuarioEfetivar, def: false, visible: ko.observable(true) });
    this.PermiteAdicionarValorComoAdiantamentoMotorista = PropertyEntity({ val: ko.observable(false), getType: typesKnockout.bool, text: Localization.Resources.Chamado.MotivoChamado.PermiteAdicionarValorComoAdiantamentoMotorista, def: false, visible: ko.observable(false) });
    this.PermiteAdicionarValorComoDespesaMotorista = PropertyEntity({ val: ko.observable(false), getType: typesKnockout.bool, text: Localization.Resources.Chamado.MotivoChamado.PermiteAdicionarValorComoDespesaMotorista, def: false, visible: ko.observable(false) });
    this.PermiteAdicionarValorComoDescontoMotorista = PropertyEntity({ val: ko.observable(false), getType: typesKnockout.bool, text: Localization.Resources.Chamado.MotivoChamado.PermiteAdicionarValorComoDescontoMotorista, def: false, visible: ko.observable(false) });
    this.HabilitarPerfilAcessoEnvioEmail = PropertyEntity({ val: ko.observable(false), getType: typesKnockout.bool, text: Localization.Resources.Chamado.MotivoChamado.HabilitarPerfilAcessoEnvioEmail, def: false, visible: ko.observable(false) });
    this.PermiteRetornarParaAjuste = PropertyEntity({ val: ko.observable(false), getType: typesKnockout.bool, text: Localization.Resources.Chamado.MotivoChamado.PermiteRetornarParaAjuste, def: false, visible: ko.observable(false) });
    this.ObrigarAnexo = PropertyEntity({ val: ko.observable(false), getType: typesKnockout.bool, text: Localization.Resources.Chamado.MotivoChamado.ObrigarAnexo, def: false, visible: ko.observable(true) });
    this.PermiteAtendimentoSemCarga = PropertyEntity({ val: ko.observable(false), getType: typesKnockout.bool, text: Localization.Resources.Chamado.MotivoChamado.PermiteLancarAtendimentoSemCarga, def: false, visible: ko.observable(false) });
    this.PermiteAlterarDatasCargaEntrega = PropertyEntity({ val: ko.observable(false), getType: typesKnockout.bool, text: Localization.Resources.Chamado.MotivoChamado.PermiteAlterarDatasCargaEntrega, def: false, visible: ko.observable(false) });
    this.BuscaContaBancariaDestinatario = PropertyEntity({ val: ko.observable(false), getType: typesKnockout.bool, text: Localization.Resources.Chamado.MotivoChamado.BuscaContaBancariaDestinatario, def: false, visible: ko.observable(false) });
    this.ObrigarInformarResponsavelAtendimento = PropertyEntity({ getType: typesKnockout.bool, text: Localization.Resources.Chamado.MotivoChamado.ObrigarInformarPessoaGrupoPessoasResponsavel, val: ko.observable(false), def: false, visible: ko.observable(false) });
    this.NaoPermitirLancarAtendimentoSemAcertoAberto = PropertyEntity({ val: ko.observable(false), getType: typesKnockout.bool, text: Localization.Resources.Chamado.MotivoChamado.NaoPermitirLancarAtendimentoSemAcertoAberto, def: false, visible: ko.observable(false) });
    this.PermitirLancarAtendimentoEmCargasComDocumentoEmitido = PropertyEntity({ val: ko.observable(false), getType: typesKnockout.bool, text: Localization.Resources.Chamado.MotivoChamado.PermitirLancarAtendimentoEmCargasComDocumentoEmitido, def: false });
    this.PermiteInserirJustificativaOcorrencia = PropertyEntity({ val: ko.observable(false), getType: typesKnockout.bool, text: Localization.Resources.Chamado.MotivoChamado.PermitirInserirJustificativaOcorrenciaChamado, def: false, visible: ko.observable(false) });
    this.InformarQuantidade = PropertyEntity({ val: ko.observable(false), getType: typesKnockout.bool, text: "Informar Quantidade", def: false, visible: ko.observable(false) });
    this.ObrigarInformacaoLote = PropertyEntity({ val: ko.observable(false), getType: typesKnockout.bool, text: Localization.Resources.Chamado.MotivoChamado.ObrigarInformacaoLote, def: false, visible: ko.observable(false) });
    this.ObrigarDataCritica = PropertyEntity({ val: ko.observable(false), getType: typesKnockout.bool, text: Localization.Resources.Chamado.MotivoChamado.ObrigarDataCritica, def: false, visible: ko.observable(false) });
    this.ObrigarRealMotivo = PropertyEntity({ val: ko.observable(false), getType: typesKnockout.bool, text: Localization.Resources.Chamado.MotivoChamado.ObrigarRealMotivo, def: false, visible: ko.observable(false) });
    this.ExigeInformarModeloVeicularAberturaChamado = PropertyEntity({ val: ko.observable(false), getType: typesKnockout.bool, text: Localization.Resources.Chamado.MotivoChamado.ExigeInformarModeloVeicularAberturaChamado, def: false });
    this.TratativaDeveSerConfirmadaPeloCliente = PropertyEntity({ val: ko.observable(false), getType: typesKnockout.bool, text: Localization.Resources.Chamado.MotivoChamado.TratativaDeveSerConfirmadaPeloCliente, def: false });
    this.NaoExigirJustificativaOcorrenciaChamadoAoSalvarObservacao = PropertyEntity({ val: ko.observable(false), getType: typesKnockout.bool, text: Localization.Resources.Chamado.MotivoChamado.NaoExigirJustificativaOcorrenciaChamadoAoSalvarObservacao, def: false });
    this.IntegrarComDansales = PropertyEntity({ val: ko.observable(false), getType: typesKnockout.bool, text: Localization.Resources.Chamado.MotivoChamado.IntegrarComDansales, def: false, visible: ko.observable(false) });
    this.PermiteTrocarTransportadora = PropertyEntity({ val: ko.observable(false), getType: typesKnockout.bool, text: Localization.Resources.Chamado.MotivoChamado.PermiteTrocarTransportadora, def: false, visible: ko.observable(true) });
    this.AtendimentoPorLote = PropertyEntity({ val: ko.observable(false), getType: typesKnockout.bool, text: Localization.Resources.Chamado.MotivoChamado.AtendimentoPorLote, def: false, visible: ko.observable(true) });
    this.PermiteInformarNFD = PropertyEntity({ val: ko.observable(false), getType: typesKnockout.bool, text: Localization.Resources.Chamado.MotivoChamado.PermiteInformarNFD, def: false, visible: ko.observable(true) });
    this.ObrigarPreenchimentoNFD = PropertyEntity({ val: ko.observable(false), getType: typesKnockout.bool, text: Localization.Resources.Chamado.MotivoChamado.ObrigarPreenchimentoNFD, def: false, visible: ko.observable(true) });
    this.PermiteInformarQuantidadeParaCalculo = PropertyEntity({ val: ko.observable(false), getType: typesKnockout.bool, text: Localization.Resources.Chamado.MotivoChamado.PermiteInformarQuantidadeParaCalculo, def: false, visible: ko.observable(true) });
    this.PermiteInformarQuantidadeVolumesParaCalculo = PropertyEntity({ val: ko.observable(false), getType: typesKnockout.bool, text: Localization.Resources.Chamado.MotivoChamado.PermiteInformarQuantidadeVolumesParaCalculo, def: false, visible: ko.observable(true) });
    this.PermiteInformarPesoParaCalculo = PropertyEntity({ val: ko.observable(false), getType: typesKnockout.bool, text: Localization.Resources.Chamado.MotivoChamado.PermiteInformarPesoParaCalculo, def: false, visible: ko.observable(true) });
    this.PermiteInformarDataRetornoAposEstadia = PropertyEntity({ val: ko.observable(false), getType: typesKnockout.bool, text: Localization.Resources.Chamado.MotivoChamado.PermiteInformarDataRetornoAposEstadia, def: false, visible: ko.observable(true) });
    this.PermiteInformarOrdemInterna = PropertyEntity({ val: ko.observable(false), getType: typesKnockout.bool, text: Localization.Resources.Chamado.MotivoChamado.PermiteInformarOrdemInterna, def: false, visible: ko.observable(true) });
    this.PermiteInformarMotoristaNoAtendimento = PropertyEntity({ val: ko.observable(false), getType: typesKnockout.bool, text: Localization.Resources.Chamado.MotivoChamado.PermiteInformarMotoristaNoAtendimento, def: false, visible: ko.observable(true) });
    this.GerarAcrescimoDescontoContratoFrete = PropertyEntity({ val: ko.observable(false), getType: typesKnockout.bool, text: Localization.Resources.Chamado.MotivoChamado.GerarAcrescimoDescontoContratoFrete, def: false, visible: ko.observable(true) });
    this.InformarCodigoSIF = PropertyEntity({ val: ko.observable(false), getType: typesKnockout.bool, text: Localization.Resources.Chamado.MotivoChamado.InformarCodigoSIF, def: false, visible: ko.observable(true) });
    this.PossibilitarInclusaoAnexoAoEscalarAtendimento = PropertyEntity({ getType: typesKnockout.bool, val: ko.observable(false), def: ko.observable(false) });
    this.ConsiderarHorasDiasUteis = PropertyEntity({ getType: typesKnockout.bool, val: ko.observable(false), def: ko.observable(false) });
    this.PermitirFreteRetornoDevolucao = PropertyEntity({ val: ko.observable(false), getType: typesKnockout.bool, text: Localization.Resources.Chamado.MotivoChamado.PermiteFreteRetornoDevolucao, def: false, visible: ko.observable(true) });
    this.ValidarEscaladaAtendimentoUsuarioResponsavel = PropertyEntity({ getType: typesKnockout.bool, text: Localization.Resources.Chamado.MotivoChamado.ValidarEscaladaAtendimentoUsuarioResponsavel, val: ko.observable(false), def: ko.observable(false), visible: ko.observable(true) });
    this.PermitirAcessarEtapaOcorrenciaSemFinalizarEtapaAnalise = PropertyEntity({ getType: typesKnockout.bool, text: Localization.Resources.Chamado.MotivoChamado.PermitirAcessarEtapaOcorrenciaSemFinalizarEtapaAnalise, val: ko.observable(false), def: ko.observable(false), visible: ko.observable(true) });
    this.PermitirInformarCausadorOcorrencia = PropertyEntity({ getType: typesKnockout.bool, text: Localization.Resources.Chamado.MotivoChamado.PermitirInformarCausadorOcorrencia, val: ko.observable(false), def: ko.observable(false), visible: ko.observable(true) });
    this.EnviarEmailParaTransportadorAoCancelarChamado = PropertyEntity({ getType: typesKnockout.bool, text: Localization.Resources.Chamado.MotivoChamado.EnviarEmailParaTransportadorAoCancelarChamado, val: ko.observable(false), def: ko.observable(false), visible: ko.observable(true) });
    this.EnviarEmailParaTransportadorAoAlterarChamado = PropertyEntity({ getType: typesKnockout.bool, text: Localization.Resources.Chamado.MotivoChamado.EnviarEmailParaTransportadorAoAlterarChamado, val: ko.observable(false), def: ko.observable(false), visible: ko.observable(true) });
    this.EnviarEmailParaTransportadorAoFinalizarChamado = PropertyEntity({ getType: typesKnockout.bool, text: Localization.Resources.Chamado.MotivoChamado.EnviarEmailParaTransportadorAoFinalizarChamado, val: ko.observable(false), def: ko.observable(false), visible: ko.observable(true) });
    this.BloquearParadaAppTrizy = PropertyEntity({ val: ko.observable(false), getType: typesKnockout.bool, text: Localization.Resources.Chamado.MotivoChamado.BloquearParadaAppTrizy, def: false, visible: ko.observable(true) });
    this.HabilitarSenhaDevolucao = PropertyEntity({ val: ko.observable(false), getType: typesKnockout.bool, text: Localization.Resources.Chamado.MotivoChamado.HabilitarSenhaDevolucao, def: false, visible: ko.observable(true) });
    this.HabilitarEstadia = PropertyEntity({ val: ko.observable(false), getType: typesKnockout.bool, text: Localization.Resources.Chamado.MotivoChamado.HabilitarEstadia, def: false, visible: ko.observable(true) });
    this.BloquearEstornoAtendimentosFinalizadosPortalTransportador = PropertyEntity({ val: ko.observable(false), getType: typesKnockout.bool, text: Localization.Resources.Chamado.MotivoChamado.BloquearEstornoAtendimentosFinalizadosPortalTransportador, def: false, visible: ko.observable(true) });
    this.HabilitarClassificacaoCriticos = PropertyEntity({ val: ko.observable(false), getType: typesKnockout.bool, text: Localization.Resources.Chamado.MotivoChamado.HabilitarClassificacaoCriticos, def: false, visible: ko.observable(true) });
    this.ValidaValorCarga = PropertyEntity({ val: ko.observable(false), getType: typesKnockout.bool, text: Localization.Resources.Chamado.MotivoChamado.ValidaValorCarga, visible: ko.observable(true) });
    this.ValidaValorDescarga = PropertyEntity({ val: ko.observable(false), getType: typesKnockout.bool, text: Localization.Resources.Chamado.MotivoChamado.ValidaValorDescarga, visible: ko.observable(true) });
    this.GerarGestaoDeDevolucao = PropertyEntity({ text: Localization.Resources.Chamado.MotivoChamado.GerarGestaoDeDevolucao, val: ko.observable(false), getType: typesKnockout.bool, visible: ko.observable(false) });
    this.PermitirAtualizarInformacoesPedido = PropertyEntity({ val: ko.observable(false), getType: typesKnockout.bool, text: Localization.Resources.Chamado.MotivoChamado.PermitirAtualizarInformacoesPedido, def: false, visible: ko.observable(false) });


    //Entidades
    this.PagamentoMotoristaTipo = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), idBtnSearch: guid(), text: Localization.Resources.Chamado.MotivoChamado.TipoPagamentoMotorista.getRequiredFieldDescription(), required: ko.observable(false), visible: ko.observable(false) });
    this.Justificativa = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), idBtnSearch: guid(), text: Localization.Resources.Chamado.MotivoChamado.Justificativa.getRequiredFieldDescription(), required: ko.observable(false), visible: ko.observable(false) });
    this.FornecedorDespesa = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), idBtnSearch: guid(), text: Localization.Resources.Chamado.MotivoChamado.FornecedorDespesa.getFieldDescription(), required: ko.observable(false), visible: ko.observable(false) });
    this.JustificativaAcrescimoDescontoContratoFrete = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), idBtnSearch: guid(), text: Localization.Resources.Chamado.MotivoChamado.JustificativaAcrescimoDescontoContratoFrete, required: ko.observable(false), visible: ko.observable(false) });
    this.TipoTransportadorAcrescimoDescontoContratoFrete = PropertyEntity({ val: ko.observable(EnumTipoProprietarioVeiculo.Todos), options: EnumTipoProprietarioVeiculo.obterOpcoesMotivoChamado(), def: EnumTipoProprietarioVeiculo.Todos, text: Localization.Resources.Chamado.MotivoChamado.TipoTransportadorAcrescimoDescontoContratoFrete, required: ko.observable(false), visible: ko.observable(false) });

    this.Assunto = PropertyEntity({ text: Localization.Resources.Chamado.MotivoChamado.Assunto.getFieldDescription(), getType: typesKnockout.string, val: ko.observable("") });
    this.ConteudoEmail = PropertyEntity({ text: Localization.Resources.Chamado.MotivoChamado.CorpoEmail.getFieldDescription(), getType: typesKnockout.string, val: ko.observable("") });

    this.Datas = PropertyEntity({ type: types.listEntity, list: new Array(), required: false, codEntity: ko.observable(0), idGrid: guid() });
    this.Arvore = PropertyEntity({ val: ko.observable("") });
    this.GatilhosTempoList = PropertyEntity({ val: ko.observable(new Array()) });
    this.TiposIntegracao = PropertyEntity({ val: ko.observable(new Array()) });
    this.Causas = PropertyEntity({ val: ko.observable(new Array()) });
    this.GatilhosNaCarga = PropertyEntity({ val: ko.observable(new Array()) });
    this.TiposCriticidade = PropertyEntity({ val: ko.observable(new Array()) });

    // Atributos da diária automática
    this.ObrigatorioTerDiariaAutomatica = PropertyEntity({ val: ko.observable(false), getType: typesKnockout.bool, text: Localization.Resources.Chamado.MotivoChamado.ObrigatorioTerDiariaAutomatica, def: false, visible: ko.observable(true) });
    this.BloquearAprovacaoValoresSuperioresADiariaAutomatica = PropertyEntity({ val: ko.observable(false), getType: typesKnockout.bool, text: Localization.Resources.Chamado.MotivoChamado.BloquearAprovacaoValoresSuperioresADiariaAutomatica, def: false, visible: ko.observable(true) });
    this.DiasLimiteAberturaAposDiariaAutomatica = PropertyEntity({ text: Localization.Resources.Chamado.MotivoChamado.DiasLimiteAberturaAposDiariaAutomatica.getFieldDescription(), col: 3, getType: typesKnockout.int });
    this.LocalFreeTime = PropertyEntity({ val: ko.observable(EnumTiposFreeTime.Coleta), options: EnumTiposFreeTime.obterOpcoes(), def: EnumTiposFreeTime.Coleta, text: Localization.Resources.Chamado.MotivoChamado.TipoFreeTime.getFieldDescription(), visible: ko.observable(true) });

    this.TipoMotivoAtendimento.val.subscribe(function (novoValor) {

        if (novoValor == EnumTipoMotivoAtendimento.Reentrega || novoValor == EnumTipoMotivoAtendimento.Retencao || novoValor == EnumTipoMotivoAtendimento.RetencaoOrigem) {
            _motivoChamado.ExigeFotoAbertura.visible(true);
            _motivoChamado.ExigeQRCodeAbertura.visible(true);
        }

        else {
            _motivoChamado.ExigeFotoAbertura.visible(false);
            _motivoChamado.ExigeFotoAbertura.val(false);

            _motivoChamado.ExigeQRCodeAbertura.visible(false);
            _motivoChamado.ExigeQRCodeAbertura.val(false);
        }

        if (novoValor === EnumTipoMotivoAtendimento.Devolucao) {

            _motivoChamado.ObrigarInformacaoLote.visible(true);
            _motivoChamado.ObrigarDataCritica.visible(true);
            _motivoChamado.ObrigarRealMotivo.visible(true);
            _motivoChamado.GerarGestaoDeDevolucao.visible(true);
            _motivoChamado.HabilitarSenhaDevolucao.visible(true);
            _motivoChamado.HabilitarEstadia.visible(false);
            _motivoChamado.PermitirAtualizarInformacoesPedido.visible(true);
        }

        else {
            _motivoChamado.ObrigarInformacaoLote.visible(false);
            _motivoChamado.ObrigarDataCritica.visible(false);
            _motivoChamado.ObrigarRealMotivo.visible(false);
            _motivoChamado.GerarGestaoDeDevolucao.visible(false);
            _motivoChamado.HabilitarSenhaDevolucao.visible(false);
            _motivoChamado.HabilitarEstadia.visible(true);
            _motivoChamado.PermitirAtualizarInformacoesPedido.visible(false);
            _motivoChamado.PermitirAtualizarInformacoesPedido.val(false);
        }
    });

    this.PermiteAdicionarValorComoAdiantamentoMotorista.val.subscribe(function (novoValor) {
        _motivoChamado.PagamentoMotoristaTipo.required(false);
        _motivoChamado.PagamentoMotoristaTipo.visible(false);
        LimparCampoEntity(_motivoChamado.PagamentoMotoristaTipo);

        if (novoValor) {
            _motivoChamado.PagamentoMotoristaTipo.required(true);
            _motivoChamado.PagamentoMotoristaTipo.visible(true);
        }
    });

    this.PermiteAdicionarValorComoDespesaMotorista.val.subscribe(function (novoValor) {
        _motivoChamado.Justificativa.required(false);
        _motivoChamado.Justificativa.visible(false);
        _motivoChamado.FornecedorDespesa.visible(false);
        LimparCampoEntity(_motivoChamado.Justificativa);
        LimparCampoEntity(_motivoChamado.FornecedorDespesa);

        if (novoValor) {
            _motivoChamado.Justificativa.required(true);
            _motivoChamado.Justificativa.visible(true);
            _motivoChamado.FornecedorDespesa.visible(true);
        }

    });

    this.PermiteAdicionarValorComoDescontoMotorista.val.subscribe(function (novoValor) {
        _motivoChamado.Justificativa.required(false);
        _motivoChamado.Justificativa.visible(false);
        LimparCampoEntity(_motivoChamado.Justificativa);

        if (novoValor) {
            _motivoChamado.Justificativa.required(true);
            _motivoChamado.Justificativa.visible(true);
        }
    });

    this.ExigeValorNaLiberacao.val.subscribe(function (novoValor) {
        if (novoValor) {
            _motivoChamado.ObrigarMotoristaInformarMultiMobile.visible(true);
        } else {
            _motivoChamado.ObrigarMotoristaInformarMultiMobile.visible(false);
        }
    });

    this.GerarAcrescimoDescontoContratoFrete.val.subscribe(function (novoValor) {
        _motivoChamado.JustificativaAcrescimoDescontoContratoFrete.required(false);
        _motivoChamado.JustificativaAcrescimoDescontoContratoFrete.visible(false);
        _motivoChamado.TipoTransportadorAcrescimoDescontoContratoFrete.visible(false);
        LimparCampoEntity(_motivoChamado.JustificativaAcrescimoDescontoContratoFrete);

        if (novoValor) {
            _motivoChamado.JustificativaAcrescimoDescontoContratoFrete.required(true);
            _motivoChamado.JustificativaAcrescimoDescontoContratoFrete.visible(true);
            _motivoChamado.TipoTransportadorAcrescimoDescontoContratoFrete.visible(true);
        }
    });

    this.ValidarDuplicidade.val.subscribe(function () {
        atualizarVisibilidade();
    });

    this.ValidarDuplicidadePorDestinatario.val.subscribe(function () {
        atualizarVisibilidade();
    });

};

function atualizarVisibilidade() {
    const validarDuplicidade = _motivoChamado.ValidarDuplicidade.val();
    const validarDuplicidadePorDestinatario = _motivoChamado.ValidarDuplicidadePorDestinatario.val();

    if (validarDuplicidade || validarDuplicidadePorDestinatario) {
        _motivoChamado.PermitirAbrirMaisAtendimentoComMesmoMotivoParaMesmaCarga.visible(false);
        _motivoChamado.PermitirAbrirMaisAtendimentoComMesmoMotivoParaMesmaCarga.val(false);
    } else {
        _motivoChamado.PermitirAbrirMaisAtendimentoComMesmoMotivoParaMesmaCarga.visible(true);
    }
}

var CRUDMotivoChamado = function () {
    this.Adicionar = PropertyEntity({ eventClick: adicionarClick, type: types.event, text: Localization.Resources.Gerais.Geral.Adicionar, visible: ko.observable(true) });
    this.Atualizar = PropertyEntity({ eventClick: atualizarClick, type: types.event, text: Localization.Resources.Gerais.Geral.Atualizar, visible: ko.observable(false) });
    this.Excluir = PropertyEntity({ eventClick: excluirClick, type: types.event, text: Localization.Resources.Gerais.Geral.Excluir, visible: ko.observable(false) });
    this.Cancelar = PropertyEntity({ eventClick: cancelarClick, type: types.event, text: Localization.Resources.Gerais.Geral.Cancelar, visible: ko.observable(false) });
};

var PesquisaMotivoChamado = function () {
    this.Descricao = PropertyEntity({ text: Localization.Resources.Chamado.MotivoChamado.Descricao.getFieldDescription(), issue: 586, required: true, getType: typesKnockout.string, val: ko.observable("") });
    this.Status = PropertyEntity({ text: Localization.Resources.Chamado.MotivoChamado.Status.getFieldDescription(), issue: 556, val: ko.observable(0), options: _statusPesquisa, def: 0 });

    this.Pesquisar = PropertyEntity({
        eventClick: function (e) {
            _gridMotivoChamado.CarregarGrid();
        }, type: types.event, text: Localization.Resources.Chamado.MotivoChamado.Pesquisar, idGrid: guid(), visible: ko.observable(true)
    });
    this.ExibirFiltros = PropertyEntity({
        eventClick: function (e) {
            if (e.ExibirFiltros.visibleFade()) {
                e.ExibirFiltros.visibleFade(false);
            } else {
                e.ExibirFiltros.visibleFade(true);
            }
        }, type: types.event, text: Localization.Resources.Chamado.MotivoChamado.FiltrosPesquisa, idFade: guid(), visibleFade: ko.observable(false), visible: ko.observable(true)
    });
};

//*******EVENTOS*******

function loadMotivoChamado() {
    ObterTipoMotivoAtendimento()
        .then(BuscarExisteGenero)
        .then(BuscarExisteAreaEnvolvida)
        .then(BuscaConfiguracaoChamado)
        .then(function () {
            _pesquisaMotivoChamado = new PesquisaMotivoChamado();
            KoBindings(_pesquisaMotivoChamado, "knockoutPesquisaMotivoChamado", false, _pesquisaMotivoChamado.Pesquisar.id);

            _motivoChamado = new MotivoChamado();
            KoBindings(_motivoChamado, "knockoutMotivoChamado");
            KoBindings(_motivoChamado, "knockoutMotivoChamadoEmail");

            _crudMotivoChamado = new CRUDMotivoChamado();
            KoBindings(_crudMotivoChamado, "knockoutCRUDMotivoChamado");

            new BuscarTipoOcorrencia(_motivoChamado.TipoOcorrencia, callbackTipoOcorrencia);
            new BuscarPagamentoMotoristaTipo(_motivoChamado.PagamentoMotoristaTipo);
            new BuscarJustificativas(_motivoChamado.Justificativa, null, null, [EnumTipoFinalidadeJustificativa.Todas, EnumTipoFinalidadeJustificativa.AcertoViagemOutrasDespesas]);
            new BuscarJustificativas(_motivoChamado.JustificativaAcrescimoDescontoContratoFrete, null, null, [EnumTipoFinalidadeJustificativa.Todas, EnumTipoFinalidadeJustificativa.ContratoFrete]);
            new BuscarClientes(_motivoChamado.FornecedorDespesa, null, false, [EnumModalidadePessoa.Fornecedor]);
            new BuscarGeneroMotivoChamado(_motivoChamado.Genero);
            new BuscarAreaEnvolvidaMotivoChamado(_motivoChamado.AreaEnvolvida);
            BuscarGrupoMotivoChamado(_motivoChamado.GrupoMotivoChamado);

            HeaderAuditoria("MotivoChamado", _motivoChamado);

            SetarLayoutPorTipoServico();
            loadMotivoChamadoData();

            buscarMotivoChamado();
            buscarConfiguracaoDiariaAutomatica();
            LoadArvoreDecisao();
            LoadGatilhosTempoEscaltionList();
            loadGridGatilhosIntegracao();
            loadCausasParaFinalizacaoArvoreDecisao();
            loadGridGatilhosNaCarga();
            loadCriticidadeAtendimento();

            if (_existeGenero)
                _motivoChamado.Genero.visible(true);

            if (_existeAreaEnvolvida)
                _motivoChamado.AreaEnvolvida.visible(true);

            if (_CONFIGURACAO_TMS.PossuiIntegracaoJJ)
                _motivoChamado.IntegrarComDansales.visible(true);

            if (_fazGestaoCriticidadeAtendimento)
                _motivoChamado.NumeroCriticidadeAtendimento.visible(true);
        });
}

function adicionarClick(e, sender) {
    _motivoChamado.Arvore.val(ObterArvore());
    _motivoChamado.PossibilitarInclusaoAnexoAoEscalarAtendimento.val(_gatilhosTempoEscaltionList.PossibilitarInclusaoAnexoAoEscalarAtendimento.val());
    ObterListaGatilhosGrid();
    ObterTiposIntegracaoSalvar();
    ObterCausasSalvar();
    ObterGatilhosNaCargaSalvar();
    ObterTiposCriticidadeSalvar();
    if (!ValidaCargaDescarga()) {
        return;
    }
    Salvar(_motivoChamado, "MotivoChamado/Adicionar", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, Localization.Resources.Chamado.MotivoChamado.Sucesso, Localization.Resources.Chamado.MotivoChamado.CadastradoComSucesso);
                _gridMotivoChamado.CarregarGrid();
                limparCamposMotivoChamado();
                LimparTodosCamposArvore();
            } else {
                exibirMensagem(tipoMensagem.aviso, Localization.Resources.Chamado.MotivoChamado.Aviso, arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Chamado.MotivoChamado.Falha, arg.Msg);
        }
    }, sender);
}

function atualizarClick(e, sender) {
    _motivoChamado.Arvore.val(ObterArvore());
    _motivoChamado.PossibilitarInclusaoAnexoAoEscalarAtendimento.val(_gatilhosTempoEscaltionList.PossibilitarInclusaoAnexoAoEscalarAtendimento.val());
    _motivoChamado.ConsiderarHorasDiasUteis.val(_gatilhosTempoEscaltionList.ConsiderarHorasDiasUteis.val());
    ObterListaGatilhosGrid();
    ObterTiposIntegracaoSalvar();
    ObterCausasSalvar();
    ObterGatilhosNaCargaSalvar();
    ObterTiposCriticidadeSalvar();
    if (!ValidaCargaDescarga()) {
        return;
    }

    Salvar(_motivoChamado, "MotivoChamado/Atualizar", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, Localization.Resources.Chamado.MotivoChamado.Sucesso, Localization.Resources.Chamado.MotivoChamado.AtualizadoComSucesso);
                _gridMotivoChamado.CarregarGrid();
                limparCamposMotivoChamado();
                LimparTodosCamposArvore();
                RecarregarGridCriticidadeAposAtualizar();
            } else {
                exibirMensagem(tipoMensagem.aviso, Localization.Resources.Chamado.MotivoChamado.Aviso, arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Chamado.MotivoChamado.Falha, arg.Msg);
        }
    }, sender);
}

function excluirClick(e, sender) {
    exibirConfirmacao("Confirmação", "Realmente deseja excluir esse cadastro?", function () {
        ExcluirPorCodigo(_motivoChamado, "MotivoChamado/ExcluirPorCodigo", function (arg) {
            if (arg.Success) {
                if (arg.Data) {
                    exibirMensagem(tipoMensagem.ok, Localization.Resources.Chamado.MotivoChamado.Sucesso, Localization.Resources.Chamado.MotivoChamado.ExcluidoComSucesso);
                    _gridMotivoChamado.CarregarGrid();
                    limparCamposMotivoChamado();
                    LimparTodosCamposArvore();
                } else {
                    exibirMensagem(tipoMensagem.aviso, Localization.Resources.Chamado.MotivoChamado.Sugestao, arg.Msg, 16000);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, Localization.Resources.Chamado.MotivoChamado.Falha, arg.Msg);
            }
        }, null);
    });
}

function cancelarClick(e) {
    limparCamposMotivoChamado();
}

function editarMotivoChamadoClick(itemGrid) {
    limparCamposMotivoChamado();
    LimparTodosCamposArvore();
    _motivoChamado.Codigo.val(itemGrid.Codigo);

    BuscarPorCodigo(_motivoChamado, "MotivoChamado/BuscarPorCodigo", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                _pesquisaMotivoChamado.ExibirFiltros.visibleFade(false);

                _crudMotivoChamado.Atualizar.visible(true);
                _crudMotivoChamado.Excluir.visible(true);
                _crudMotivoChamado.Cancelar.visible(true);
                _crudMotivoChamado.Adicionar.visible(false);

                RecarregarGridMotivoChamadoData();
                ListarArvore();
                SetaGridGatilhos();
                SetaGridIntegracoes(arg.Data.TiposIntegracao);
                SetarGridCausas();
                SetaGridGatilhosNaCarga();
                SetarGridCriticidade(arg.Data.TiposCriticidade);
                visibilidadeQuebraRegra(arg.Data.TipoOcorrenciaQuebraRegra);

            } else {
                exibirMensagem(tipoMensagem.aviso, Localization.Resources.Chamado.MotivoChamado.Aviso, arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Chamado.MotivoChamado.Falha, arg.Msg);
        }
    }, null);
}

//*******MÉTODOS*******

function buscarMotivoChamado() {
    var editar = { descricao: Localization.Resources.Chamado.MotivoChamado.Editar, id: "clasEditar", evento: "onclick", metodo: editarMotivoChamadoClick, tamanho: "20", icone: "" };

    var menuOpcoes = {
        tipo: TypeOptionMenu.link,
        opcoes: [editar]
    };

    var configuracoesExportacao = { url: "MotivoChamado/ExportarPesquisa", titulo: "Motivo de Chamado." };
    _gridMotivoChamado = new GridViewExportacao(_pesquisaMotivoChamado.Pesquisar.idGrid, "MotivoChamado/Pesquisa", _pesquisaMotivoChamado, menuOpcoes, configuracoesExportacao);
    _gridMotivoChamado.CarregarGrid();
}

function limparCamposMotivoChamado() {
    _crudMotivoChamado.Atualizar.visible(false);
    _crudMotivoChamado.Cancelar.visible(false);
    _crudMotivoChamado.Excluir.visible(false);
    _crudMotivoChamado.Adicionar.visible(true);
    LimparCampos(_motivoChamado);
    LimparCamposGatilhos();
    RecarregarGridGatilhosTempoEscaltionList();
    limparCamposIntegracoes();
    _motivoChamado.Datas.list = new Array();
    limparCamposMotivoChamadoData();
    limparGridCriticidade();
    Global.ResetarAbas();
}

function SetarLayoutPorTipoServico() {
    if (_CONFIGURACAO_TMS.TipoServicoMultisoftware === EnumTipoServicoMultisoftware.MultiTMS) {
        _motivoChamado.GerarCargaDevolucaoSeAprovado.visible(false);
        _motivoChamado.ChamadoDeveSerAbertoPeloEmbarcador.visible(false);
        _motivoChamado.ReferentePagamentoDescarga.visible(true);
        _motivoChamado.PermiteAdicionarValorComoAdiantamentoMotorista.visible(true);
        _motivoChamado.PermiteAdicionarValorComoDespesaMotorista.visible(true);
        _motivoChamado.PermiteAdicionarValorComoDescontoMotorista.visible(true);
        _motivoChamado.HabilitarPerfilAcessoEnvioEmail.visible(true);
        _motivoChamado.PermiteRetornarParaAjuste.visible(true);
        _motivoChamado.PermiteAtendimentoSemCarga.visible(true);
        _motivoChamado.BuscaContaBancariaDestinatario.visible(true);
        _motivoChamado.ObrigarInformarResponsavelAtendimento.visible(true);
        _motivoChamado.NaoPermitirLancarAtendimentoSemAcertoAberto.visible(true);
        _motivoChamado.PermiteInformarMotoristaNoAtendimento.visible(false);
    } else if (_CONFIGURACAO_TMS.TipoServicoMultisoftware === EnumTipoServicoMultisoftware.MultiEmbarcador) {
        _motivoChamado.PermiteAlterarDatasCargaEntrega.visible(true);
        _motivoChamado.PermiteInserirJustificativaOcorrencia.visible(true);
        _motivoChamado.InformarQuantidade.visible(true);
    }
}

function ObterTipoMotivoAtendimento() {
    return new Promise((resolve) => {
        executarReST("MotivoChamado/BuscarTiposMotivoAtendimento", {
            Tipos: JSON.stringify([
                EnumTipoMotivoAtendimento.Atendimento,
                EnumTipoMotivoAtendimento.Devolucao,
                EnumTipoMotivoAtendimento.Reentrega,
                EnumTipoMotivoAtendimento.Retencao,
                EnumTipoMotivoAtendimento.RetencaoOrigem,
                EnumTipoMotivoAtendimento.ReentregarMesmaCarga])
        }, function (r) {
            if (r.Success) {
                _configuracaoTipoMotivoAtendimento = new Array();

                for (var i = 0; i < r.Data.length; i++)
                    _configuracaoTipoMotivoAtendimento.push({ value: r.Data[i].Codigo, text: r.Data[i].Descricao });
            } else {
                exibirMensagem(tipoMensagem.falha, Localization.Resources.Chamado.MotivoChamado.Falha, r.Msg);
            }

            resolve();
        });
    });
};

function buscarConfiguracaoDiariaAutomatica() {
    executarReST("ConfiguracaoDiariaAutomatica/ObterConfiguracao", {}, function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                if (retorno.Data.HabilitarDiariaAutomatica) {
                    $('#atributosDiariaAutomatica').show();
                }
            }
            else
                exibirMensagem(tipoMensagem.atencao, Localization.Resources.Chamado.MotivoChamado.Atencao, retorno.Msg);
        }
        else
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Chamado.MotivoChamado.Falha, retorno.Msg);
    });
}

function BuscarExisteGenero() {
    return new Promise(function (resolve) {
        executarReST("MotivoChamado/BuscarExisteGeneroCadastrado", {}, function (r) {
            if (r.Success) {
                _existeGenero = r.Data;
            } else {
                _existeGenero = false;
            }
            resolve();
        });
    });
}

function BuscarExisteAreaEnvolvida() {
    return new Promise(function (resolve) {
        executarReST("MotivoChamado/BuscarExisteAreaEnvolvidaCadastrada", {}, function (r) {
            if (r.Success) {
                _existeAreaEnvolvida = r.Data;
            } else {
                _existeAreaEnvolvida = false;
            }
            resolve();
        });
    });
}

function BuscaConfiguracaoChamado() {
    return new Promise(function (resolve) {
        executarReST("MotivoChamado/BuscarConfiguracaoChamado", {}, function (r) {
            if (r.Success) {
                _fazGestaoCriticidadeAtendimento = r.Data.FazerGestaoCriticidade;
            } else {
                _fazGestaoCriticidadeAtendimento = false;
            }
            resolve();
        });
    });
}

function ValidaCargaDescarga() {
    if (_motivoChamado.ValidaValorCarga.val() && _motivoChamado.ValidaValorDescarga.val()) {
        exibirMensagem(tipoMensagem.falha, Localization.Resources.Chamado.MotivoChamado.Falha, Localization.Resources.Chamado.MotivoChamado.PorFavorVerifiqueValorCargaDescarga);
        return false;
    }

    return true;
}

function callbackTipoOcorrencia(tipoOcorrencia) {
    _motivoChamado.TipoOcorrencia.codEntity(tipoOcorrencia.Codigo);
    _motivoChamado.TipoOcorrencia.val(tipoOcorrencia.Descricao);

    visibilidadeQuebraRegra(tipoOcorrencia.OcorrenciaParaQuebraRegraPallet);
}

function visibilidadeQuebraRegra(permiteQuebraRegra) {
    _motivoChamado.TipoQuebraRegraPallet.visible(true);

    if (!permiteQuebraRegra) {
        _motivoChamado.TipoQuebraRegraPallet.val(EnumTipoQuebraRegra.Padrao);
        _motivoChamado.TipoQuebraRegraPallet.visible(false);
    }
}