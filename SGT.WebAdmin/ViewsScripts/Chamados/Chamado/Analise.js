/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Globais.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="../../../js/Global/PermissoesPersonalizadas.js" />
/// <reference path="../../Cargas/ControleEntregaDevolucao/ControleEntregaDevolucao.js" />
/// <reference path="../../Consultas/Cliente.js" />
/// <reference path="../../Consultas/SetorFuncionario.js" />
/// <reference path="../../Consultas/Usuario.js" />
/// <reference path="../../Consultas/JustificativaOcorrencia.js" />
/// <reference path="../../Consultas/MotivoDevolucaoEntrega.js" />
/// <reference path="../../Consultas/TipoCriticidade.js" />
/// <reference path="../../Enumeradores/EnumChamadoResponsavelOcorrencia.js" />
/// <reference path="../../Enumeradores/EnumSituacaoChamado.js" />
/// <reference path="../../Enumeradores/EnumPermissaoPersonalizada.js" />
/// <reference path="../../Enumeradores/EnumTratativaDevolucao.js" />
/// <reference path="../../Enumeradores/EnumTipoColetaEntregaDevolucao.js" />
/// <reference path="../../Enumeradores/EnumSimNao.js" />
/// <reference path="Chamado.js" />
/// <reference path="ChatMobile.js" />
/// <reference path="NFeDevolucao.js" />
/// <reference path="ModalRecusaCancelamento.js" />
/// <reference path="AnaliseAnexo.js" />
/// <reference path="CriticidadeAtendimento.js" />

// #region Objetos Globais do Arquivo

var _analise;
var _delegar;
var _delegarPorSetor;
var _gridAnalises;
var _configuracaoTratativaDevolucao;
var _codigoDaAnalise;
var _controleEntregaDevolucaoChamado;
var _informacaoPagamentoMotorista;
var _adicionarXmlNotaFiscalDevolucaoModal;
var _chamadoOcorrenciaModalDelegar;
var _chamadoOcorrenciaModalDelegarPorSetor;
var _chamadoOcorrenciaModalInformacaoPagamentoMotorista;
var _chamadoOcorrenciaModalGerenciarAnexosAnalise;
var _chamadoOcorrenciaModalArvore;
var _primeraEntrada = false;
var _liberarAprovarValor = false;

// #endregion

// #region  Classes

var Analise = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.CodigoAnalise = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Observacao = PropertyEntity({ type: types.map, val: ko.observable(""), def: "", text: Localization.Resources.Chamado.ChamadoOcorrencia.Observacao.getFieldDescription(), enable: ko.observable(false) });
    this.NaoRegistrarObservacaoTransportadora = PropertyEntity({ text: Localization.Resources.Chamado.ChamadoOcorrencia.NaoRegistrarObservacaoTransportadora, getType: typesKnockout.bool, val: ko.observable(false), def: false, visible: ko.observable(true) });
    this.Estadia = PropertyEntity({ text: Localization.Resources.Chamado.ChamadoOcorrencia.Estadia.getFieldDescription(), val: ko.observable(EnumSimNao.Nao), options: EnumSimNao.obterOpcoes(), def: EnumSimNao.Nao, visible: ko.observable(true) });
    this.DataAnalise = PropertyEntity({ type: types.map, getType: typesKnockout.dateTime, val: ko.observable(""), def: "", text: Localization.Resources.Chamado.ChamadoOcorrencia.DataAnalise.getFieldDescription(), enable: ko.observable(false) });
    this.DataRetorno = PropertyEntity({ type: types.map, getType: typesKnockout.dateTime, val: ko.observable(""), def: "", text: Localization.Resources.Chamado.ChamadoOcorrencia.DataRetorno.getFieldDescription(), enable: ko.observable(true) });
    this.PossuiRegras = PropertyEntity({ type: types.map, val: ko.observable(true), def: true });
    this.AosCuidadosDo = PropertyEntity({ type: types.map, val: ko.observable(0), def: 0 });
    this.UltimaResposta = PropertyEntity({ type: types.map, val: ko.observable(0), def: 0 });
    this.SituacaoTratativa = PropertyEntity({ val: ko.observable(EnumSituacaoChamado.Aberto), options: EnumSituacaoChamado.obterOpcoesSituacaoTratativa(), def: EnumSituacaoChamado.Aberto, text: Localization.Resources.Chamado.ChamadoOcorrencia.SituacaoTratativa.getFieldDescription(), enable: ko.observable(true), visible: ko.observable(false) });

    this.ResponsavelOcorrencia = PropertyEntity({ val: ko.observable(EnumChamadoResponsavelOcorrencia.Comercial), options: EnumChamadoResponsavelOcorrencia.obterOpcoes(), def: EnumChamadoResponsavelOcorrencia.Comercial, text: Localization.Resources.Chamado.ChamadoOcorrencia.ResponsavelOcorrencia.getFieldDescription(), visible: ko.observable(true), enable: ko.observable(true) });
    this.Representante = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Chamado.ChamadoOcorrencia.Representante, idBtnSearch: guid(), enable: ko.observable(true), visible: ko.observable(true) });
    this.ClienteNovaEntrega = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), val: ko.observable(""), def: "", text: Localization.Resources.Chamado.ChamadoOcorrencia.Cliente.getRequiredFieldDescription(), required: ko.observable(false), enable: ko.observable(false), visible: ko.observable(false), idBtnSearch: guid() });

    this.Analistas = PropertyEntity({ type: types.local, idGrid: guid() });

    this.Salvar = PropertyEntity({ eventClick: salvarAnaliseClick, type: types.event, text: Localization.Resources.Chamado.ChamadoOcorrencia.Salvar, visible: ko.observable(false) });
    this.Responder = PropertyEntity({ eventClick: responderAnaliseClick, type: types.map, text: TextoResposta(), visible: ko.observable(false), val: ko.observable(false), def: false, getType: typesKnockout.bool });
    this.Limpar = PropertyEntity({ eventClick: limparAnaliseClick, type: types.event, text: Localization.Resources.Chamado.ChamadoOcorrencia.Limpar, visible: ko.observable(false) });

    this.Reprocessar = PropertyEntity({ eventClick: reprocessarClick, type: types.event, text: Localization.Resources.Chamado.ChamadoOcorrencia.Reprocessar, visible: ko.observable(false) });

    this.Delegar = PropertyEntity({ eventClick: delegarModalClick, type: types.event, text: Localization.Resources.Chamado.ChamadoOcorrencia.DelegarParaOutroUsuario, visible: ko.observable(false) });
    this.DelegarPorSetor = PropertyEntity({ eventClick: delegarPorSetorModalClick, type: types.event, text: Localization.Resources.Chamado.ChamadoOcorrencia.DelegarParaOutroSetor, visible: ko.observable(true) });

    this.Finalizar = PropertyEntity({ eventClick: liberarOcorrenciaChamadoClick, type: types.event, text: ko.observable(Localization.Resources.Chamado.ChamadoOcorrencia.LiberarOcorrencia), visible: ko.observable(false) });
    this.LiberarParaCliente = PropertyEntity({ eventClick: liberarParaClienteClick, type: types.event, text: ko.observable(Localization.Resources.Chamado.ChamadoOcorrencia.LiberarParaCliente), visible: ko.observable(false), enable: ko.observable(true) });
    this.Fechar = PropertyEntity({ eventClick: fecharSemOcorrenciaClick, type: types.event, text: ko.observable(Localization.Resources.Chamado.ChamadoOcorrencia.FecharSemOcorrencia), visible: ko.observable(false) });
    this.Cancelar = PropertyEntity({ eventClick: cancelarChamadoComMotivo, type: types.event, text: Localization.Resources.Chamado.ChamadoOcorrencia.Cancelar, visible: ko.observable(false) });
    this.Recusar = PropertyEntity({ eventClick: recusarChamadoClick, type: types.event, text: Localization.Resources.Chamado.ChamadoOcorrencia.Recusa, visible: ko.observable(false) });
    this.Rejeitar = PropertyEntity({ eventClick: rejeitarChamadoClick, type: types.event, text: Localization.Resources.Chamado.ChamadoOcorrencia.Rejeitar, visible: ko.observable(false) });

    this.Imprimir = PropertyEntity({ eventClick: ImprimirAnalisesClick, type: types.event, text: Localization.Resources.Chamado.ChamadoOcorrencia.Imprimir, visible: ko.observable(true) });
    this.ImprimirDevolucao = PropertyEntity({ eventClick: ImprimirDevolucaoClick, type: types.event, text: Localization.Resources.Chamado.ChamadoOcorrencia.ImprimirDevolucao, visible: ko.observable(false) });
    this.ChatMotorista = PropertyEntity({ eventClick: abrirChatClick, type: types.event, text: Localization.Resources.Chamado.ChamadoOcorrencia.ChatMotorista, visible: ko.observable(true), cssClass: ko.observable("btn btn-default waves-effect waves-themed ms-2 chat-button"), notificationCount: ko.observable(0) });
    this.SalvarAnalise = PropertyEntity({ eventClick: salvarAnaliseDevolucaoClick, type: types.event, text: Localization.Resources.Chamado.ChamadoOcorrencia.FinalizarTratativa, visible: ko.observable(false) });
    this.NivelAtendimento = PropertyEntity({ text: ko.observable(Localization.Resources.Chamado.ChamadoOcorrencia.SemNivel), visible: ko.observable(false), val: ko.observable(0) });

    this.Anexo = PropertyEntity({ eventClick: gerenciarAnexosAaliseClick, type: types.event, text: Localization.Resources.Chamado.ChamadoOcorrencia.Anexos, visible: ko.observable(false), enable: ko.observable(true) });

    this.Motivo = PropertyEntity({ text: Localization.Resources.Chamado.ChamadoOcorrencia.Motivo.getFieldDescription(), visible: ko.observable(true) });
    this.MotivoTipoOcorrencia = PropertyEntity({ text: Localization.Resources.Chamado.ChamadoOcorrencia.MotivoMotorista.getFieldDescription() });
    this.ObservacaoRetornoMotorista = PropertyEntity({ text: Localization.Resources.Chamado.ChamadoOcorrencia.RetornoMotorista.getFieldDescription(), visible: ko.observable(true), enable: ko.observable(true), maxlength: 500 });
    this.TratativaDevolucao = PropertyEntity({ val: ko.observable(_configuracaoTratativaDevolucao[0].value), options: _configuracaoTratativaDevolucao, text: Localization.Resources.Chamado.ChamadoOcorrencia.Tratativa.getFieldDescription(), def: _configuracaoTratativaDevolucao[0].value, required: ko.observable(true), visible: ko.observable(true), enable: ko.observable(true) });
    this.TipoDevolucao = PropertyEntity({ val: ko.observable(EnumTipoColetaEntregaDevolucao.Total), options: EnumTipoColetaEntregaDevolucao.obterOpcoes(), def: EnumTipoColetaEntregaDevolucao.Total, text: "Tipo de Devolução das Notas Selecionadas".getFieldDescription(), visible: ko.observable(true), enable: ko.observable(true) });
    this.MotivoDaDevolucao = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: ko.observable(Localization.Resources.Chamado.ChamadoOcorrencia.MotidoDaDevolucao.getFieldDescription()), idBtnSearch: guid(), required: ko.observable(false), visible: ko.observable(true), enable: ko.observable(true), requiredClass: ko.observable("") });
    this.ExibirNFeDevolucao = PropertyEntity({ getType: typesKnockout.bool, val: ko.observable(_CONFIGURACAO_TMS.HabilitarControleFluxoNFeDevolucaoChamado), def: _CONFIGURACAO_TMS.HabilitarControleFluxoNFeDevolucaoChamado });
    this.JustificativaOcorrencia = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: ko.observable(Localization.Resources.Chamado.ChamadoOcorrencia.JustificativaOcorrencia.getRequiredFieldDescription()), idBtnSearch: guid(), required: ko.observable(false), visible: ko.observable(false) });
    this.DataReentregaMesmaCarga = PropertyEntity({ type: types.map, getType: typesKnockout.dateTime, val: ko.observable(""), def: "", text: Localization.Resources.Chamado.ChamadoOcorrencia.DataReentregaMesmaCarga.getFieldDescription(), enable: ko.observable(true), visible: ko.observable(false) });
    this.NaoAssumirDataEntregaNota = PropertyEntity({ text: Localization.Resources.Chamado.ChamadoOcorrencia.NaoAssumirDataEntregaNota, getType: typesKnockout.bool, val: ko.observable(false), def: false, visible: ko.observable(false), enable: ko.observable(true) });
    this.Arvore = PropertyEntity({ visible: ko.observable(false) });
    this.CodigoSIF = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Chamado.ChamadoOcorrencia.CodigoSIF, idBtnSearch: guid(), required: ko.observable(false), visible: ko.observable(false), enable: ko.observable(true) });
    this.FreteRetornoDevolucao = PropertyEntity({ val: ko.observable(EnumSimNao.Sim), options: EnumSimNao.obterOpcoes(), def: EnumSimNao.Sim, text: Localization.Resources.Chamado.ChamadoOcorrencia.FreteRetornoDevolucao.getFieldDescription(), visible: ko.observable(true), enable: ko.observable(true) });
    this.SenhaDevolucao = PropertyEntity({ val: ko.observable(false), text: Localization.Resources.Chamado.ChamadoOcorrencia.Senha.getFieldDescription(), visible: ko.observable(true), enable: ko.observable(true) });
    this.ExibirAnexosEscaladas = PropertyEntity({ eventClick: exibirAnexosEscaladas, type: types.event, text: Localization.Resources.Gerais.Geral.Anexos, visible: ko.observable(false) });
    this.AprovarValorChamado = PropertyEntity({ eventClick: liberarChamadoOcorrenciaClick, type: types.event, text: ko.observable(Localization.Resources.Chamado.ChamadoOcorrencia.AprovarValorCargaDescarga), visible: ko.observable(false), enable: ko.observable(false) });

    this.TratativaDevolucao.val.subscribe(function (novoValor) {
        if (novoValor != EnumTratativaDevolucao.EntregarEmOutroCliente) {
            LimparCampoEntity(_analise.ClienteNovaEntrega);

            _analise.ClienteNovaEntrega.visible(false);
            _analise.ClienteNovaEntrega.required(false);
        }
        else {
            _analise.ClienteNovaEntrega.visible(true);
            _analise.ClienteNovaEntrega.required(true);
        }

        if (novoValor == EnumTratativaDevolucao.Revertida || novoValor == EnumTratativaDevolucao.Reentregue) {
            _analise.Finalizar.visible(false);
        } else {
            _analise.Finalizar.visible(true);
        }

        _analise.TipoDevolucao.visible(true);

        if (novoValor == EnumTratativaDevolucao.Revertida) {
            _analise.TipoDevolucao.val(EnumTipoColetaEntregaDevolucao.Total);
            _analise.TipoDevolucao.visible(false);
        }

        tratarCampoSenha(novoValor, null);

        if (novoValor == EnumTratativaDevolucao.ReentregarMesmaCarga) {
            _analise.DataReentregaMesmaCarga.visible(true);
            _analise.NaoAssumirDataEntregaNota.visible(true);
        } else {
            _analise.DataReentregaMesmaCarga.visible(false);
            _analise.NaoAssumirDataEntregaNota.visible(false);
        }
    });

    this.TipoDevolucao.val.subscribe(function (novoValor) {
        if (_analise.SalvarAnalise.visible())
            _controleEntregaDevolucaoChamado.atualizarStatusDevolucaoTotalNotas(novoValor == EnumTipoColetaEntregaDevolucao.Total);

        _analise.MotivoDaDevolucao.required(false);
        _analise.MotivoDaDevolucao.visible(false);
        if (novoValor == EnumTipoColetaEntregaDevolucao.Total) {
            _analise.MotivoDaDevolucao.required(true);
            _analise.MotivoDaDevolucao.visible(true);
        }
    });
    this.MotivoDaDevolucao.required.subscribe(function (required) {
        _analise.MotivoDaDevolucao.text(Localization.Resources.Chamado.ChamadoOcorrencia.MotidoDaDevolucao.getFieldDescription());
        if (required)
            _analise.MotivoDaDevolucao.text(Localization.Resources.Chamado.ChamadoOcorrencia.MotidoDaDevolucao.getRequiredFieldDescription());
    });

    _chamado.Estadia.val.subscribe(function (newValue) {
        _analise.Estadia.val(newValue);
    });

    //Aba NF-e de Devolução
    this.NFeDevolucaoAnalise = PropertyEntity({ type: types.listEntity, list: new Array(), required: false, codEntity: ko.observable(0), idGrid: guid() });
    this.GridNFeDevolucaoAnalise = PropertyEntity({ type: types.local, id: guid() });

    this.NumeroNFeOrigemAnalise = PropertyEntity({ text: Localization.Resources.Chamado.ChamadoOcorrencia.NumeroNFeOrigem.getFieldDescription(), type: types.entity, codEntity: ko.observable(0), idBtnSearch: guid(), required: ko.observable(false), visible: ko.observable(true), enable: ko.observable(true) });
    this.ChaveNFeDevolucaoAnalise = PropertyEntity({ text: Localization.Resources.Chamado.ChamadoOcorrencia.Chave.getFieldDescription(), maxlength: 44, required: ko.observable(false), enable: ko.observable(true) });
    this.NumeroNFeDevolucaoAnalise = PropertyEntity({ text: Localization.Resources.Chamado.ChamadoOcorrencia.NumeroNFeDevolucao.getFieldDescription(), getType: typesKnockout.int, required: ko.observable(false), enable: ko.observable(true) });
    this.SerieNFeDevolucaoAnalise = PropertyEntity({ text: Localization.Resources.Chamado.ChamadoOcorrencia.Serie.getFieldDescription(), getType: typesKnockout.int, required: ko.observable(false), enable: ko.observable(true) });
    this.DataEmissaoNFeDevolucaoAnalise = PropertyEntity({ text: Localization.Resources.Chamado.ChamadoOcorrencia.DataEmissao.getFieldDescription(), getType: typesKnockout.date, required: ko.observable(false), enable: ko.observable(true) });
    this.ValorTotalProdutosNFeDevolucaoAnalise = PropertyEntity({ text: Localization.Resources.Chamado.ChamadoOcorrencia.ValorProduto.getFieldDescription(), getType: typesKnockout.decimal, required: ko.observable(false), enable: ko.observable(true) });
    this.ValorTotalNFeDevolucaoAnalise = PropertyEntity({ text: Localization.Resources.Chamado.ChamadoOcorrencia.ValorTotalNF.getFieldDescription(), getType: typesKnockout.decimal, required: ko.observable(false), enable: ko.observable(true) });
    this.PesoDevolvidoNFeDevolucaoAnalise = PropertyEntity({ text: Localization.Resources.Chamado.ChamadoOcorrencia.PesoDevolvido.getFieldDescription(), getType: typesKnockout.decimal, required: ko.observable(false), enable: ko.observable(true) });
    this.AdicionarNFeDevolucaoAnalise = PropertyEntity({ eventClick: adicionarNFeDevolucaoAnaliseClick, type: types.event, text: Localization.Resources.Chamado.ChamadoOcorrencia.AdicionarNFe, enable: ko.observable(true) });
    this.ImportarXmlNotaFiscalDevolucaoAnalise = PropertyEntity({ eventClick: notaFiscalAnaliseClick, type: types.event, text: Localization.Resources.Chamado.ChamadoOcorrencia.AdicionarXML, type: types.abrirModal, required: ko.observable(false), visible: ko.observable(true), enable: ko.observable(true) });

    //Aba Informações de Fechamento
    this.InformarDadosChamadoFinalizadoComCusto = PropertyEntity({ text: "Informar dados de chamado finalizado com custo", getType: typesKnockout.bool, val: ko.observable(false), def: false, enable: ko.observable(true) });
    this.InformarDadosChamadoFinalizadoComCusto.val.subscribe(function (novoValor) {
        controleCamposInformacoesFechamento(novoValor);
    });
    this.InformacoesFechamento = PropertyEntity({ type: types.listEntity, list: new Array(), required: false, codEntity: ko.observable(0), idGrid: guid() });
    this.GridInformacoesFechamento = PropertyEntity({ type: types.local, id: guid() });

    this.NotaFiscalInformacaoFechamento = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: ko.observable(Localization.Resources.Chamado.ChamadoOcorrencia.NotaFiscal.getRequiredFieldDescription()), idBtnSearch: guid(), required: ko.observable(false), visible: ko.observable(false), enable: ko.observable(true) });
    this.MotivoProcesso = PropertyEntity({ text: "*Motivo do Processo: ", options: ko.observable([]), val: ko.observable(0), def: 0, visible: ko.observable(false), enable: ko.observable(true) });
    this.QuantidadeDivergencia = PropertyEntity({ text: "*Quantidade: ", getType: typesKnockout.int, val: ko.observable(""), def: "", maxlength: 4, visible: ko.observable(false), enable: ko.observable(true) });
    this.AdicionarInformacaoFechamento = PropertyEntity({ eventClick: adicionarInformacoesFechamentoClick, type: types.event, text: Localization.Resources.Chamado.ChamadoOcorrencia.AdicionarNFe, visible: ko.observable(false), enable: ko.observable(true) });
    this.SalvarInformacoesFechamento = PropertyEntity({ eventClick: salvarInformacoesFechamentoEtapaAnaliseClick, type: types.event, text: Localization.Resources.Chamado.ChamadoOcorrencia.SalvarInformacoesDeFechamento, visible: ko.observable(false) });

    //Informações da Critcidade do Atendimento
    this.Critico = PropertyEntity({ text: Localization.Resources.Chamado.ChamadoOcorrencia.Critico, val: ko.observable(1), def: 0, options: ko.observable([{ text: "Sim", value: 1 }, { text: "Não", value: 0 }]), visible: ko.observable(false), enable: ko.observable(false) });
    this.Gerencial = PropertyEntity({ type: types.entity, text: Localization.Resources.Chamado.ChamadoOcorrencia.Gerencial, codEntity: ko.observable(0), val: ko.observable(""), idBtnSearch: guid(), visible: ko.observable(false), enable: ko.observable(false), required: ko.observable(false) });
    this.CausaProblema = PropertyEntity({ type: types.entity, text: Localization.Resources.Chamado.ChamadoOcorrencia.CausaProblema, codEntity: ko.observable(0), val: ko.observable(""), idBtnSearch: guid(), visible: ko.observable(false), enable: ko.observable(false), required: ko.observable(false) });
    this.FUP = PropertyEntity({ text: Localization.Resources.Chamado.ChamadoOcorrencia.FUP, val: ko.observable(""), def: "", visible: ko.observable(false), enable: ko.observable(false), maxlength: 500 });
    this.CriticidadeAtendimento = PropertyEntity({ type: types.list, list: [] });
    this.GridCriticidade = PropertyEntity({ type: types.event, idGrid: guid() });
    this.CancelarChamadoDireto = PropertyEntity({ eventClick: cancelarChamadoClick, type: types.event, text: Localization.Resources.Chamado.ChamadoOcorrencia.CancelarChamado, visible: ko.observable(false) });
    this.DataPrevisaoEntregaPedidos = PropertyEntity({ type: types.map, getType: typesKnockout.dateTime, val: ko.observable(""), def: "", text: Localization.Resources.Chamado.ChamadoOcorrencia.DataPrevisaoEntregaPedidos, visible: ko.observable(true), enable: ko.observable(true) });
    this.PermiteEditarCamposCriticidade = PropertyEntity({ val: ko.observable(false) })

    this.Critico.val.subscribe(function (novoValor) {
        if (_analise.PermiteEditarCamposCriticidade.val()) {
            controleStatusCamposCriticidade(novoValor, null);
        }
    });
};

var Delegar = function () {
    this.Usuario = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Chamado.ChamadoOcorrencia.Usuario.getRequiredFieldDescription(), idBtnSearch: guid(), required: true });

    this.Delegar = PropertyEntity({ type: types.event, eventClick: delegarClick, text: Localization.Resources.Chamado.ChamadoOcorrencia.Delegar });
};

var DelegarPorSetor = function () {
    this.Setor = PropertyEntity({
        type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Chamado.ChamadoOcorrencia.Setor.getRequiredFieldDescription(), idBtnSearch: guid(), required: true
    });

    this.Delegar = PropertyEntity({ type: types.event, eventClick: delegarPorSetorClick, text: Localization.Resources.Chamado.ChamadoOcorrencia.Delegar });
};

var InformacaoPagamentoMotorista = function () {
    this.Fornecedor = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), idBtnSearch: guid(), text: Localization.Resources.Chamado.ChamadoOcorrencia.Fornecedor.getRequiredFieldDescription(), required: ko.observable(true) });
    this.LiberarOcorrencia = PropertyEntity({ val: ko.observable(false), def: false, getType: typesKnockout.bool });

    this.Enviar = PropertyEntity({ type: types.event, eventClick: confirmarInformacaoPagamentoMotoristaClick, text: Localization.Resources.Chamado.ChamadoOcorrencia.Enviar });
};

let ChamadoOcorrenciaArvore = function () {
    this.CodigoMotivo = PropertyEntity({ val: ko.observable(0) });
    this.PerguntasArvore = PropertyEntity({ val: ko.observable([]) });
    this.Causas = PropertyEntity({ text: Localization.Resources.Chamado.ChamadoOcorrencia.Causa, val: ko.observable(""), def: "", options: ko.observable([]), visible: ko.observable(false) });

    // Levara o controle das perguntas resolvida da arvore
    this.PerguntasRespondida = PropertyEntity({ val: ko.observable([]) });

    this.Finalizar = PropertyEntity({ eventClick: FinalizarAtendimento, type: types.event, text: Localization.Resources.Chamado.ChamadoOcorrencia.Finalizar, visible: ko.observable(false) });
    this.Salvar = PropertyEntity({ eventClick: SalvarArvore, type: types.event, text: Localization.Resources.Chamado.ChamadoOcorrencia.Salvar, visible: ko.observable(false) });

}
// #endregion


// #region Funções de Inicialização

function loadAnalise() {
    return ObterTratativasDevolucao().then(function () {

        _analise = new Analise();
        KoBindings(_analise, "knockoutAnalise");

        _delegar = new Delegar();
        KoBindings(_delegar, "knockoutDelegar");

        _delegarPorSetor = new DelegarPorSetor();
        KoBindings(_delegarPorSetor, "knockoutDelegarPorSetor");

        _chamadoOcorrenciaModalArvore = new ChamadoOcorrenciaArvore();
        KoBindings(_chamadoOcorrenciaModalArvore, "knockoutChamadoOcorrenciaArvore");

        _informacaoPagamentoMotorista = new InformacaoPagamentoMotorista();
        KoBindings(_informacaoPagamentoMotorista, "knoutInformacaoPagamentoMotorista");

        _controleEntregaDevolucaoChamado = new ControleEntregaDevolucaoContainer("controle-entrega-devolucao-chamado-container");

        BuscarXMLNotaFiscal(_analise.NumeroNFeOrigemAnalise, null, _abertura?.Carga);
        BuscarFuncionario(_delegar.Usuario);
        BuscarClientes(_analise.ClienteNovaEntrega);
        BuscarSetorFuncionario(_delegarPorSetor.Setor);
        BuscarClientes(_informacaoPagamentoMotorista.Fornecedor);
        BuscarJustificativaOcorrencia(_analise.JustificativaOcorrencia);
        BuscarMotivosDevolucaoEntrega(_analise.MotivoDaDevolucao);
        BuscarSIF(_analise.CodigoSIF);

        $("#divModalDelegar").on('hidden.bs.modal', function () {
            LimparCampoEntity(_delegar.Usuario);
        });

        $("#" + _analise.ChaveNFeDevolucaoAnalise.id).mask("00000000000000000000000000000000000000000000", { selectOnFocus: true, clearIfNotMatch: true });

        if (_CONFIGURACAO_TMS.TipoServicoMultisoftware == EnumTipoServicoMultisoftware.MultiCTe) {
            _analise.ResponsavelOcorrencia.visible(false);
            _analise.Representante.visible(false);
        }
        else
            new BuscarRepresentante(_analise.Representante);

        loadNFeDevolucaoAnalise();
        loadSobrasChamadoOcorrencia();
        loadInformacoesFechamento();
        setarVisibilidadeCampos();
        loadAnexoAnalise();
        loadAnexoNivelAtendimento();
        loadCriticidadeAtendimento();
        BuscarAnalises();

        if (_notificacaoGlobal && _notificacaoGlobal.CodigoObjeto.val() > 0) {
            buscarChamadoPorCodigo(_notificacaoGlobal.CodigoObjeto.val());
            _notificacaoGlobal.CodigoObjeto.val(0);
        }
        VisibilidadeBotoesArvore()

        _adicionarXmlNotaFiscalDevolucaoModal = new bootstrap.Modal(document.getElementById("divModalImportarXmlNFe"), { backdrop: 'static' });
        _chamadoOcorrenciaModalDelegar = new bootstrap.Modal(document.getElementById("divModalDelegar"), { backdrop: 'static' });
        _chamadoOcorrenciaModalDelegarPorSetor = new bootstrap.Modal(document.getElementById("divModalDelegarPorSetor"), { backdrop: 'static' });
        _chamadoOcorrenciaModalInformacaoPagamentoMotorista = new bootstrap.Modal(document.getElementById("divModalInformacaoPagamentoMotorista"), { backdrop: 'static' });
        _chamadoOcorrenciaModalGerenciarAnexosAnalise = new bootstrap.Modal(document.getElementById("divModalGerenciarAnexosAnalise"), { backdrop: 'static' });

        if (_CONFIGURACAO_TMS.PermitirRegistrarObservacoesSemVisualizacaoTransportadora == true && _CONFIGURACAO_TMS.TipoServicoMultisoftware == EnumTipoServicoMultisoftware.MultiCTe)
            _analise.NaoRegistrarObservacaoTransportadora.visible(false);
        if (_CONFIGURACAO_TMS.PermitirRegistrarObservacoesSemVisualizacaoTransportadora == false)
            _analise.NaoRegistrarObservacaoTransportadora.visible(false);

    });
}

// #endregion

// #region Funções Associadas a Eventos

function ImprimirAnalisesClick() {
    downloadAnalises(false);
}

function ImprimirDevolucaoClick() {
    executarDownload("ChamadoOcorrencia/GerarRelatorioDevolucao", { Codigo: _chamado.Codigo.val() });
}

function gerenciarAnexosAaliseClick() {
    _gridAnexo.CarregarGrid(obterAnexos());

    _chamadoOcorrenciaModalGerenciarAnexosAnalise.show();
}

function salvarAnaliseClick(e, sender) {
    NaoExigirJustificativaOcorrenciaChamadoAoSalvarObservacao();
    Salvar(_analise, "ChamadoAnalise/Adicionar", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                var dados = arg.Data;
                _chamado.Situacao.val(dados.Situacao);
                _resumoChamado.Situacao.val(dados.SituacaoDescricao);

                _analise.Observacao.val(_analise.Observacao.def);
                _analise.DataRetorno.val(Global.DataHoraAtual());
                LimparCampoEntity(_analise.JustificativaOcorrencia);

                _gridAnalises.CarregarGrid();

                enviarArquivosAnexados(arg.Data.Codigo);
                exibirMensagem(tipoMensagem.ok, Localization.Resources.Chamado.ChamadoOcorrencia.Sucesso, Localization.Resources.Chamado.ChamadoOcorrencia.AnaliseSalvaComSucesso);

                if (_CONFIGURACAO_TMS.TipoServicoMultisoftware === EnumTipoServicoMultisoftware.MultiCTe)
                    _analise.UltimaResposta.val(EnumChamadoAosCuidadosDo.Transporador);
                else
                    _analise.UltimaResposta.val(EnumChamadoAosCuidadosDo.Embarcador);

                if (_analise.Responder.visible()) {
                    var retornopara = "";
                    if (_CONFIGURACAO_TMS.TipoServicoMultisoftware === EnumTipoServicoMultisoftware.MultiCTe)
                        retornopara = "Embarcador";
                    else
                        retornopara = Localization.Resources.Chamado.ChamadoOcorrencia.Transportador;

                    exibirMensagem(tipoMensagem.aviso, Localization.Resources.Chamado.ChamadoOcorrencia.Resposta, Localization.Resources.Chamado.ChamadoOcorrencia.SuaMensagemFoiSalvaPoremChamadoContinuaAosSeusCuidados.format(retornopara, TextoResposta()), 10000);
                }
            } else {
                exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.Aviso, arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, arg.Msg);
        }
    }, sender);
}

function responderAnaliseClick(e, sender) {
    var _handleSave = function () {
        Salvar(_analise, "ChamadoOcorrencia/Responder", function (arg) {
            if (arg.Success) {
                if (arg.Data) {
                    _analise.Observacao.val(_analise.Observacao.def);
                    _analise.DataRetorno.val(Global.DataHoraAtual());
                    exibirMensagem(tipoMensagem.ok, Localization.Resources.Chamado.ChamadoOcorrencia.Sucesso, Localization.Resources.Chamado.ChamadoOcorrencia.RespondidoComSucesso);
                    _analise.Responder.visible(false);
                } else {
                    exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.Aviso, arg.Msg);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, arg.Msg);
            }
        }, sender);
    };

    if (
        (_CONFIGURACAO_TMS.TipoServicoMultisoftware == EnumTipoServicoMultisoftware.MultiCTe && _analise.UltimaResposta.val() == EnumChamadoAosCuidadosDo.Embarcador)
        || (_CONFIGURACAO_TMS.TipoServicoMultisoftware == EnumTipoServicoMultisoftware.MultiEmbarcador && _analise.UltimaResposta.val() == EnumChamadoAosCuidadosDo.Transporador)
    ) {
        var retornopara = "";
        if (_CONFIGURACAO_TMS.TipoServicoMultisoftware == EnumTipoServicoMultisoftware.MultiCTe)
            retornopara = "Embarcador";
        else
            retornopara = Localization.Resources.Chamado.ChamadoOcorrencia.Transportador;

        exibirConfirmacao(Localization.Resources.Chamado.ChamadoOcorrencia.RetornoChamado, Localization.Resources.Chamado.ChamadoOcorrencia.VoceNaoRegistrouNenhumaInformacaoNovaAoChamadoRealmenteDeseja.format(retornopara), _handleSave);
    } else {
        _handleSave();
    }
}

function reprocessarClick(e, sender) {
    executarReST("ChamadoAnalise/ReprocessarRegras", { Codigo: _analise.Codigo.val() }, function (arg) {
        if (arg.Success) {
            if (arg.Data !== false) {
                _chamado.Situacao.val(arg.Data);
                AvaliarRegras();
                SetarEtapaChamado();
            } else {
                exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.Aviso, arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, arg.Msg);
        }
    });
}

function delegarClick() {
    if (!ValidarCamposObrigatorios(_delegar)) {
        exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.CamposObrigatorios, Localization.Resources.Gerais.Geral.InformeCamposObrigatorios);
        return;
    }

    var dados = {
        Chamado: _analise.Codigo.val(),
        Usuario: _delegar.Usuario.codEntity()
    };

    executarReST("ChamadoAnalise/Delegar", dados, function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                _chamadoOcorrenciaModalDelegar.hide();
                buscarChamadoPorCodigo(_chamado.Codigo.val());
            }
            else
                exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.Aviso, retorno.Msg);
        }
        else
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, retorno.Msg);
    });
}

function delegarModalClick() {
    _chamadoOcorrenciaModalDelegar.show();
    $("#divModalDelegar").one('hidden.bs.modal', function () {
        LimparCampos(_delegar);
    });
}

function delegarPorSetorClick() {
    if (!ValidarCamposObrigatorios(_delegarPorSetor)) {
        exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.CamposObrigatorios, Localization.Resources.Gerais.Geral.InformeCamposObrigatorios);
        return;
    }

    var dados = {
        Chamado: _analise.Codigo.val(),
        Setor: _delegarPorSetor.Setor.codEntity()
    };

    executarReST("ChamadoAnalise/DelegarPorSetor", dados, function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                _chamadoOcorrenciaModalDelegarPorSetor.hide();
                buscarChamadoPorCodigo(_chamado.Codigo.val());
            }
            else
                exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.Aviso, retorno.Msg);
        }
        else
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, retorno.Msg);
    });
}

function delegarPorSetorModalClick() {
    _chamadoOcorrenciaModalDelegarPorSetor.show();
    $("#divModalDelegarPorSetor").one('hidden.bs.modal', function () {
        LimparCampos(_delegarPorSetor);
    });
}

function confirmarInformacaoPagamentoMotoristaClick() {
    if (!ValidarCamposObrigatorios(_informacaoPagamentoMotorista))
        return exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.CamposObrigatorios, Localization.Resources.Gerais.Geral.PorFavorInformeOsCamposObrigatorios);

    if (_informacaoPagamentoMotorista.LiberarOcorrencia.val())
        abrirOcorrenciaChamadoClick();
    else
        finalizarChamadoClick();
}

function liberarOcorrenciaChamadoClick() {
    if (!validarCriticidadeObrigatoria()) {
        return;
    }
    if (_motivoChamadoConfiguracao.PessoaSeraInformadaGeracaoPagamento) {
        LimparCampos(_informacaoPagamentoMotorista);
        _informacaoPagamentoMotorista.LiberarOcorrencia.val(true);
        _chamadoOcorrenciaModalInformacaoPagamentoMotorista.show();
    } else
        abrirOcorrenciaChamadoClick();
}

function liberarParaClienteClick() {
    exibirConfirmacao(Localization.Resources.Gerais.Geral.Confirmacao, Localization.Resources.Chamado.ChamadoOcorrencia.VoceRealementeDesejaLiberarOcorrenciaParaCliente, function () {
        var data = {
            Chamado: _analise.Codigo.val(),
        };
        executarReST("ChamadoAnalise/LiberarParaCliente", data, function (arg) {
            if (arg.Success) {
                if (arg.Data) {
                    exibirMensagem(tipoMensagem.ok, Localization.Resources.Chamado.ChamadoOcorrencia.Sucesso, Localization.Resources.Chamado.ChamadoOcorrencia.EnviadoParaCliente);
                    _analise.LiberarParaCliente.enable(false);
                } else {
                    exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.Aviso, arg.Msg);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, arg.Msg);
            }
        });
    });
}

function abrirOcorrenciaChamadoClick(e, sender) {
    if (!validarCriticidadeObrigatoria()) return;
    exibirConfirmacao(Localization.Resources.Gerais.Geral.Confirmacao, Localization.Resources.Chamado.ChamadoOcorrencia.VoceRealementeDesejaLiberarlancamentoOcorrencia, function () {
        abrirOcorrencia()
    });
}

function abrirOcorrencia() {
    var data = {
        Codigo: _analise.Codigo.val(),
        ResponsavelOcorrencia: _analise.ResponsavelOcorrencia.val(),
        Representante: _analise.Representante.codEntity(),
        PodeResponder: BotaoResponder(),
        PessoaTituloPagar: _informacaoPagamentoMotorista.Fornecedor.codEntity(),
        CriticidadeAtendimento: obterDadosCriticidade()
    };

    executarReST("ChamadoAnalise/AbrirOcorrencia", data, function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                if (_CONFIGURACAO_TMS.SalvarAnaliseEmAnexoAoLiberarOcorrenciaChamado)
                    downloadAnalises(true);

                buscarChamadoPorCodigo(_chamado.Codigo.val());
                _chamadoOcorrenciaModalInformacaoPagamentoMotorista.hide();
            } else {
                exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.Aviso, arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, arg.Msg);
        }
    });
}

function salvarAnaliseDevolucaoClick(e, sender) {
    NaoExigirJustificativaOcorrenciaChamadoAoSalvarObservacao();
    if (!ValidarCamposObrigatorios(_analise)) {
        exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.CamposObrigatorios, Localization.Resources.Gerais.Geral.PorFavorInformeOsCamposObrigatorios);
        return;
    }

    let dadosNota = _controleEntregaDevolucaoChamado.obter();
    let objDadosNota = JSON.parse(dadosNota);
    let possuiNotaEntregue = objDadosNota.NotasFiscais.some(nota => nota.SituacaoNotaFiscal === EnumSituacaoNotaFiscal.Entregue);

    if (possuiNotaEntregue) {
        exibirConfirmacao(Localization.Resources.Gerais.Geral.Confirmacao, "Este atendimento contém pelo menos uma nota com a situação <b>'Entregue'</b>. Ao confirmar, todas as notas devolvidas terão a situação de devolução selecionada (Total ou Parcial). Deseja realmente confirmar este atendimento?", function () {
            enviarSalvarAnaliseDevolucaoClick();
        });

    } else {
        exibirConfirmacao(Localization.Resources.Gerais.Geral.Confirmacao, Localization.Resources.Chamado.ChamadoOcorrencia.RealmenteDesejaFinalizarTratativa, function () {
            enviarSalvarAnaliseDevolucaoClick();
        });
    }
}

function enviarSalvarAnaliseDevolucaoClick() {
    var analise = RetornarObjetoPesquisa(_analise);

    //Atualiza o status de Devolução total das notas antes de salvar a Análise
    _controleEntregaDevolucaoChamado.atualizarStatusDevolucaoTotalNotas(_analise.TipoDevolucao.val() === EnumTipoColetaEntregaDevolucao.Total);

    const notas = JSON.parse(_controleEntregaDevolucaoChamado.obter()).NotasFiscais || [];

    if (!notas.every(nota => nota.DevolucaoParcial === true || nota.DevolucaoTotal === true)) {
        exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Aviso, Localization.Resources.Chamado.ChamadoOcorrencia.TipoDeDevolucaoObrigatorio);
        return;
    }

    analise["ItensDevolver"] = _controleEntregaDevolucaoChamado.obter();
    analise["CriticidadeAtendimento"] = obterDadosCriticidade();

    executarReST("ChamadoAnalise/SalvarAnaliseDevolucao", analise, function (retorno) {
        if (retorno.Success) {
            if (retorno.Data !== false) {
                if (_analise.TratativaDevolucao.val() == EnumTratativaDevolucao.Rejeitada && _motivoChamadoConfiguracao.GerarCargaDevolucao) {
                    abrirOcorrencia();
                    exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Aviso, Localization.Resources.Chamado.ChamadoOcorrencia.GerouCargaDevolucao);
                }
                else
                    exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.Aviso, Localization.Resources.Chamado.ChamadoOcorrencia.NaoGerouCargaDevolucao);

                exibirMensagem(tipoMensagem.ok, Localization.Resources.Chamado.ChamadoOcorrencia.Sucesso, Localization.Resources.Chamado.ChamadoOcorrencia.AtendimentoAtualizadoComSucesso);
                buscarChamadoPorCodigo(_chamado.Codigo.val());
                limparCampos();
            }
            else
                exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.Aviso, retorno.Msg);
        }
        else
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, retorno.Msg);
    });
}

function fecharSemOcorrenciaClick(e, sender) {
    if (_motivoChamadoConfiguracao.PessoaSeraInformadaGeracaoPagamento) {
        LimparCampos(_informacaoPagamentoMotorista);
        _informacaoPagamentoMotorista.LiberarOcorrencia.val(false);
        _chamadoOcorrenciaModalInformacaoPagamentoMotorista.show();
    } else
        finalizarChamadoClick();
}

function finalizarChamadoClick() {
    if (!validarCriticidadeObrigatoria()) {
        return;
    }
    let dadosNota = _controleEntregaDevolucaoChamado.obter();
    let objDadosNota = JSON.parse(dadosNota);
    let possuiNotaEntregue = _motivoChamadoConfiguracao.MotivoDevolucao && objDadosNota.NotasFiscais.some(nota => nota.SituacaoNotaFiscal === EnumSituacaoNotaFiscal.Entregue);

    let data = {
        Codigo: _analise.Codigo.val(),
        PessoaTituloPagar: _informacaoPagamentoMotorista.Fornecedor.codEntity(),
        ItensDevolver: possuiNotaEntregue ? dadosNota : null,
        CriticidadeAtendimento: obterDadosCriticidade()
    };

    exibirConfirmacao(Localization.Resources.Gerais.Geral.Confirmacao,
        possuiNotaEntregue ?
            "Este atendimento contém pelo menos uma nota com a situação <b>'Entregue'</b>. Ao confirmar, todas as notas devolvidas terão a situação de devolução selecionada (Total ou Parcial). Deseja realmente confirmar este atendimento?" :
            Localization.Resources.Chamado.ChamadoOcorrencia.VoceRealmenteDesejaFinalizarEsseAtendimento,
        function () {
            executarFinalizacaoChamado(data);
        }
    );
}

function executarFinalizacaoChamado(data) {
    debugger;
    executarReST("ChamadoAnalise/FinalizarChamado", data, function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                _chamado.Situacao.val(EnumSituacaoChamado.Finalizado);
                SetarEtapaChamado();
                recarregarGridChamados();
                limparCampos();
            } else {
                exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.Aviso, arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, arg.Msg);
        }
    });
}

function confirmarCancelarChamadoClick(e, sender) {
    if (!ValidarCamposObrigatorios(_modalMotivoRecusaCancelamento)) {
        exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.CamposObrigatorios, Localization.Resources.Chamado.ChamadoOcorrencia.NecessarioInformarMotivo);
        return;
    }
    exibirConfirmacao(Localization.Resources.Gerais.Geral.Confirmacao, Localization.Resources.Chamado.ChamadoOcorrencia.VoceRealmenteDesejaCancelarEsseAtendimento, function () {
        executarReST("ChamadoAnalise/CancelarChamado", { Codigo: _analise.Codigo.val(), MotivoRecusaCancelamento: _modalMotivoRecusaCancelamento.MotivoRecusaCancelamento.codEntity() }, function (arg) {
            if (arg.Success) {
                if (arg.Data) {
                    _chamado.Situacao.val(EnumSituacaoChamado.Cancelada);
                    SetarEtapaChamado();
                    _chamadoOcorrenciaModalMotivoRecusa.hide();
                    limparCampos();
                } else {
                    exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.Aviso, arg.Msg);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, arg.Msg);
            }
        });
    });
}

function cancelarChamadoComMotivo() {
    if (_CONFIGURACAO_TMS.TipoServicoMultisoftware === EnumTipoServicoMultisoftware.MultiTMS) {
        loadModalMotivoRecusaCancelamento(false, true);
        _chamadoOcorrenciaModalMotivoRecusa.show();
    } else
        cancelarChamadoClick();
}

function cancelarChamadoClick(e, sender) {
    exibirConfirmacao(Localization.Resources.Gerais.Geral.Confirmacao, Localization.Resources.Chamado.ChamadoOcorrencia.VoceRealmenteDesejaCancelarEsseAtendimento, function () {
        executarReST("ChamadoAnalise/CancelarChamado", { Codigo: _analise.Codigo.val() }, function (arg) {
            if (arg.Success) {
                if (arg.Data) {
                    _chamado.Situacao.val(EnumSituacaoChamado.Cancelada);
                    SetarEtapaChamado();
                    limparCampos();
                } else {
                    exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.Aviso, arg.Msg);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, arg.Msg);
            }
        });
    });
}

function confirmacaoRecusarChamadoClick(e, sender) {
    if (!ValidarCamposObrigatorios(_modalMotivoRecusaCancelamento)) {
        exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.CamposObrigatorios, Localization.Resources.Chamado.ChamadoOcorrencia.NecessarioInformarMotivo);
        return;
    }
    exibirConfirmacao(Localization.Resources.Gerais.Geral.Confirmacao, Localization.Resources.Chamado.ChamadoOcorrencia.VoceRealmenteDesejaRecusarEsseAtendimento, function () {
        executarReST("ChamadoAnalise/RecusarChamado", { Codigo: _analise.Codigo.val(), MotivoRecusaCancelamento: _modalMotivoRecusaCancelamento.MotivoRecusaCancelamento.codEntity() }, function (arg) {
            if (arg.Success) {
                if (arg.Data) {
                    _chamado.Situacao.val(EnumSituacaoChamado.RecusadoPeloCliente);
                    SetarEtapaChamado();
                    recarregarGridChamados();
                    _chamadoOcorrenciaModalMotivoRecusa.hide();
                    limparCampos();
                } else {
                    exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.Aviso, arg.Msg);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, arg.Msg);
            }
        });
    });
}

function recusarChamadoClick(e, sender) {
    loadModalMotivoRecusaCancelamento(true, false);
    _chamadoOcorrenciaModalMotivoRecusa.show();
}

function atualizarNotificacoes(count) {
    _analise.ChatMotorista.notificationCount(count);
}

function abrirChatClick() {
    atualizarNotificacoes(0)
    loadChatModal(_abertura.CodigoCarga.val(), false)

}

function BuscarNotificacoesMensagensNaoLida() {

    executarReST("ControleEntrega/BuscarMensagensNaoLida", { Carga: _abertura.CodigoCarga.val() }, function (arg) {
        if (arg.Data) {
            atualizarNotificacoes(arg.Data);
        }
    }, null);

}



function rejeitarChamadoClick() {
    exibirConfirmacao(Localization.Resources.Gerais.Geral.Confirmacao, Localization.Resources.Chamado.ChamadoOcorrencia.VoceRealmenteDesejaRejeitarEsseAtendimentoParaQue, function () {
        executarReST("ChamadoAnalise/RejeitarChamado", { Codigo: _analise.Codigo.val() }, function (arg) {
            if (arg.Success) {
                if (arg.Data) {
                    exibirMensagem(tipoMensagem.ok, Localization.Resources.Chamado.ChamadoOcorrencia.Sucesso, Localization.Resources.Chamado.ChamadoOcorrencia.AtendimentoAtualizadoComSucesso);
                    buscarChamadoPorCodigo(_chamado.Codigo.val());
                    limparCampos();
                } else {
                    exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.Aviso, arg.Msg);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, arg.Msg);
            }
        });
    });
}

function detalharAnaliseClick(dataRow) {
    _pesquisaAnexo.Codigo.val(dataRow.Codigo);

    executarReST("ChamadoAnalise/BuscarPorCodigo", { Codigo: dataRow.Codigo }, function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                _analise.Observacao.val(arg.Data.Observacao);
                _analise.DataRetorno.val(arg.Data.DataRetorno);
                _analise.DataAnalise.val(arg.Data.DataAnalise);
                _analise.Estadia.val(arg.Data.Estadia);
                _analise.NaoRegistrarObservacaoTransportadora.val(arg.Data.NaoRegistrarObservacaoTransportadora);
                _analise.CodigoAnalise.val(arg.Data.Codigo);
                _analise.JustificativaOcorrencia.val(arg.Data.JustificativaOcorrencia.Descricao);
                _analise.JustificativaOcorrencia.codEntity(arg.Data.JustificativaOcorrencia.Codigo);

                ControleCamposAnalise(false);
                _analise.Limpar.visible(true);
                _analise.Anexo.visible(true);
                _analise.Responder.visible(false);
                _analise.Cancelar.visible(false);

                _analise.CancelarChamadoDireto.visible(arg.Data.SituacaoChamadoAberto);

                _anexoAnalise.Anexos.val(arg.Data.Anexos);

                Global.ResetarAbas();
            } else {
                exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.Aviso, arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, arg.Msg);
        }
    });
}

function limparAnaliseClick() {
    limparCamposAnexo();
    _analise.Observacao.val(_analise.Observacao.def);
    _analise.DataRetorno.val(Global.DataHoraAtual());
    _analise.DataAnalise.val(_analise.DataAnalise.def);
    _analise.Estadia.val(_analise.Estadia.def);
    _analise.NaoRegistrarObservacaoTransportadora.val(_analise.NaoRegistrarObservacaoTransportadora.def);
    _analise.Responder.val(_analise.Responder.def);
    _analise.CodigoAnalise.val(_analise.CodigoAnalise.def);
    _analise.Anexo.visible(true);
    LimparCampoEntity(_analise.JustificativaOcorrencia);

    if (_chamado.PodeEditar.val()) {
        _analise.ResponsavelOcorrencia.enable(true);
        _analise.Representante.enable(true);
    }

    AvaliarRegras();
    _analise.Anexo.visible(true);
    _pesquisaAnexo.Codigo.val("0");
    BotaoResponder();
}

// #endregion

// #region  Funções Públicas

function AvaliarRegras() {
    // Bloqueia campos
    var situacao = _chamado.Situacao.val();

    if ((situacao === EnumSituacaoChamado.Aberto || situacao === EnumSituacaoChamado.EmTratativa) && (_CONFIGURACAO_TMS.TipoServicoMultisoftware == EnumTipoServicoMultisoftware.MultiCTe || _chamado.PodeEditar.val()))
        ControleCamposAnalise(true);
    else
        ControleCamposAnalise(false);

    if (situacao === EnumSituacaoChamado.SemRegra)
        _analise.PossuiRegras.val(false);
    else
        _analise.PossuiRegras.val(true);
}

function EditarAnalise(data) {
    _analise.Codigo.val(data.Codigo);
    _analise.AosCuidadosDo.val(data.AosCuidadosDo);
    _analise.UltimaResposta.val(data.UltimaResposta);
    _analise.ResponsavelOcorrencia.val(data.ResponsavelOcorrencia);
    _analise.SituacaoTratativa.val(_chamado.Situacao.val());
    _analise.NivelAtendimento.visible(data.NivelAtendimento != EnumEscalationList.Nenhum);
    _analise.NivelAtendimento.text(EnumEscalationList.obterDescricao(data.NivelAtendimento));
    _analise.InformarDadosChamadoFinalizadoComCusto.val(data.InformarDadosChamadoFinalizadoComCusto);
    _analise.CodigoSIF.visible(_motivoChamadoConfiguracao.InformarCodigoSIF);
    _analise.CodigoSIF.required(_motivoChamadoConfiguracao.InformarCodigoSIF);
    _analise.DataPrevisaoEntregaPedidos.val(data.DataPrevisaoEntregaPedidos);
    _analise.DataPrevisaoEntregaPedidos.enable(data.DataPrevisaoEntregaPedidos);

    ValidarValores(data);

    if (data.LiberarOpcaoAprovarValor && !data.LiberadoValorChamadoOcorrencia) {
        _analise.AprovarValorChamado.visible(true);
        _analise.AprovarValorChamado.enable(true);
    }

    if (data.InformarDadosChamadoFinalizadoComCusto)
        controleStatusCamposInformacoesFechamento(false);

    if (data.Representante != null) {
        _analise.Representante.val(data.Representante.Descricao);
        _analise.Representante.codEntity(data.Representante.Codigo);
    }

    if (_CONFIGURACAO_TMS.TipoServicoMultisoftware != EnumTipoServicoMultisoftware.MultiCTe && (_chamado.Situacao.val() === EnumSituacaoChamado.Aberto || _chamado.Situacao.val() === EnumSituacaoChamado.EmTratativa) &&
        (_PermissaoDelegar || data.CodigoResponsavel == _CONFIGURACAO_TMS.CodigoUsuarioLogado || data.CodigoAutor == _CONFIGURACAO_TMS.CodigoUsuarioLogado)) {

        if (!_CONFIGURACAO_TMS.UsuarioAdministrador) {
            if (!VerificaSePossuiPermissaoEspecial(EnumPermissaoPersonalizada.ChamadoOcorrencia_PermitirDelegarParaOutroUsuario, _PermissoesPersonalizadasChamado))
                _analise.Delegar.visible(false);
            else
                _analise.Delegar.visible(true);

            if (!VerificaSePossuiPermissaoEspecial(EnumPermissaoPersonalizada.ChamadoOcorrencia_PermitirDelegarParaUmSetor, _PermissoesPersonalizadasChamado))
                _analise.DelegarPorSetor.visible(false);
            else
                _analise.DelegarPorSetor.visible(true);
        } else {
            _analise.Delegar.visible(true);
            _analise.DelegarPorSetor.visible(true);
        }
    }
    else {
        _analise.Delegar.visible(false);
        _analise.DelegarPorSetor.visible(false);
    }

    if (data.ExigirMotivoDeDevolucao)
        _analise.MotivoDaDevolucao.required(true);
    else
        _analise.MotivoDaDevolucao.required(false);

    _gridAnalises.CarregarGrid(ExibirUltimaAnalise);
    AvaliarRegras();
    BotaoResponder();
    BuscarNotificacoesMensagensNaoLida()
    preencherCriticidadeDoChamado({ Critico: data.Critico, FUP: data.FUP, CodigoGerencial: data.CodigoGerencial, GerencialDescricao: data.GerencialDescricao, CodigoCausaProblema: data.CodigoCausaProblema, CausaProblemaDescricao: data.CausaProblemaDescricao, Gerencial: data.Gerencial, CausaProblema: data.CausaProblema });
    VerificarEConfigurarCriticidade(data.Abertura?.MotivoChamado?.Codigo);
}
function LimparCamposAnalise() {
    _analise.TratativaDevolucao.visible(false);
    _analise.MotivoDaDevolucao.visible(false);
    _analise.Delegar.visible(false);
    _analise.DelegarPorSetor.visible(false);
    LimparCampos(_analise);
    _analise.DataRetorno.val(Global.DataHoraAtual());
    ControleCamposAnalise(true);
    _controleEntregaDevolucaoChamado._carregado.then(function () {
        _controleEntregaDevolucaoChamado.limpar();
    });
    LimparCampos(_informacaoPagamentoMotorista);

    controleStatusCamposInformacoesFechamento(true);
}

function ValidarValores(dataAnalise) {
    var data = {
        Codigo: dataAnalise.Codigo,
        Carga: dataAnalise.Abertura.Carga.Codigo,
        MotivoChamado: dataAnalise.Abertura.MotivoChamado.Codigo,
        Cliente: dataAnalise.Abertura.Cliente.Codigo,
        Valor: dataAnalise.Valor,
        ValorCarga: dataAnalise.ValorCarga,
        ValorDescarga: dataAnalise.ValorDescarga
    };

    executarReST("ChamadoOcorrencia/ValidarValoresCargaDescarga", data, function (arg) {
        if (arg.Success && arg.Data) {
            return true;
        } else if (!arg.Success && !arg.Data) {
            return exibirConfirmacao(Localization.Resources.Gerais.Geral.Confirmacao, "Valor do atendimento maior que valor da Carga/Descarga     " + Localization.Resources.Chamado.ChamadoOcorrencia.AprovarValorCargaDescarga + " ?", function () {
                executarReST("ChamadoOcorrencia/AprovarValorCargaDescarga", data, function (arg) {
                    if (arg.Success && arg.Data) {
                        return true;
                    } else if (!arg.Success && arg.Data) {
                        exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, arg.Msg);
                    }
                });
            });
        }
        else if (arg.Sucess == false) {
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, arg.Msg);
            return false;
        }
    });
}

function PreecherAnaliseDevolucao(codigoChamado, codigoCargaEntrega) {
    executarReST("ChamadoAnalise/BuscarPorAnaliseDevolucaoCodigo", { Codigo: codigoChamado }, function (arg) {
        if (arg.Data != null) {
            _analise.Motivo.val(arg.Data.Motivo);
            _analise.MotivoTipoOcorrencia.val(arg.Data.MotivoTipoOcorrencia);
            _analise.TipoDevolucao.val(arg.Data.TipoDevolucao);
            _notaFiscal.TipoDevolucao.val(arg.Data.TipoDevolucao);
            _analise.ObservacaoRetornoMotorista.val(arg.Data.ObservacaoRetornoMotorista);
            _analise.TratativaDevolucao.val(arg.Data.TratativaDevolucao);
            _analise.MotivoDaDevolucao.codEntity(arg.Data.MotivoDaDevolucao.Codigo);
            _analise.MotivoDaDevolucao.val(arg.Data.MotivoDaDevolucao.Descricao);
            _analise.NFeDevolucaoAnalise.list = recursiveObjetoRetorno(arg.Data.NFeDevolucaoAnalise);
            _analise.InformacoesFechamento.list = recursiveObjetoRetorno(arg.Data.InformacoesFechamento);
            _analise.DataReentregaMesmaCarga.val(arg.Data.DataReentregaMesmaCarga);
            _analise.NaoAssumirDataEntregaNota.val(arg.Data.NaoAssumirDataEntregaNota);
            _analise.CodigoSIF.codEntity(arg.Data.CodigoSIF.Codigo);
            _analise.CodigoSIF.val(arg.Data.CodigoSIF.Descricao);
            _analise.FreteRetornoDevolucao.val(arg.Data.FreteRetornoDevolucao);
            _analise.SenhaDevolucao.val(arg.Data.SenhaDevolucao);

            _controleEntregaDevolucaoChamado.preencher(codigoCargaEntrega, _analise.SalvarAnalise.visible(), codigoChamado);
            recarregarGridNFeDevolucaoAnalise();
            recarregarGridInformacoesFechamento();
            ControleCamposNFeDevolucaoAnalise();

            _analise.CancelarChamadoDireto.visible(arg.Data.SituacaoChamadoAberto);
        }
        else
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, arg.Msg);
    }, null);
}

// #endregion

// #region Funções Privadas

function downloadAnalises(liberouOcorrencia) {
    executarDownload("ChamadoAnalise/DownloadRelatorioAnalises", { Codigo: _chamado.Codigo.val(), LiberouOcorrencia: liberouOcorrencia });
}

function ExibirUltimaAnalise(response) {
    //if (response.data.length > 0) {
    //    detalharAnaliseClick(response.data[0]);
    //}
}

function BotaoResponder() {
    if (
        (
            (_CONFIGURACAO_TMS.TipoServicoMultisoftware == EnumTipoServicoMultisoftware.MultiCTe && _analise.AosCuidadosDo.val() == EnumChamadoAosCuidadosDo.Transporador) ||
            (_CONFIGURACAO_TMS.TipoServicoMultisoftware == EnumTipoServicoMultisoftware.MultiEmbarcador && _analise.AosCuidadosDo.val() == EnumChamadoAosCuidadosDo.Embarcador && _chamado.PodeEditar.val())
        ) &&
        (_chamado.Situacao.val() === EnumSituacaoChamado.Aberto || _chamado.Situacao.val() === EnumSituacaoChamado.EmTratativa) && !_chamado.GerarCargaDevolucao.val() &&
        !_motivoChamadoConfiguracao.MotivoReentrega &&
        !_motivoChamadoConfiguracao.MotivoRetencao && !_motivoChamadoConfiguracao.TratativaDeveSerConfirmadaPeloCliente
    ) {
        _analise.Responder.visible(true);
        return true;
    }
    else {
        _analise.Responder.visible(false);
        return false;
    }
}

function BuscarAnalises() {
    var detalhar = { descricao: "Detalhar", id: "clasEditar", evento: "onclick", metodo: detalharAnaliseClick, tamanho: "10", icone: "" };
    var menuOpcoes = {
        tipo: TypeOptionMenu.link,
        opcoes: [detalhar]
    };

    _gridAnalises = new GridView(_analise.Analistas.idGrid, "ChamadoAnalise/Pesquisa", _analise, menuOpcoes);
}

function ControleCamposAnalise(status) {
    _analise.Observacao.enable(status);
    _analise.DataRetorno.enable(status);
    _analise.ResponsavelOcorrencia.enable(status);
    _analise.Representante.enable(status);
    _analise.Anexo.visible(status);
    _anexoAnalise.Adicionar.visible(status);
    _analise.Salvar.visible(status);
    _analise.Limpar.visible(status);
    _analise.PermiteEditarCamposCriticidade.val(status);
    _analise.Critico.enable(status);
    controleStatusCamposCriticidade(null, status);
    if (_motivoChamadoConfiguracao.HabilitarEstadia)
        _analise.Estadia.visible(true);
    else
        _analise.Estadia.visible(false);

    if (_motivoChamadoConfiguracao.HabilitarEstadia)
        _analise.Estadia.visible(true);
    else
        _analise.Estadia.visible(false);

    if (_CONFIGURACAO_TMS.TipoServicoMultisoftware != EnumTipoServicoMultisoftware.MultiCTe) {
        _analise.Fechar.visible(true);
        _analise.Cancelar.visible(false);
        _analise.Recusar.visible(false);
        _analise.Rejeitar.visible(false);

        if (_motivoChamadoConfiguracao.MotivoDevolucao || _motivoChamadoConfiguracao.MotivoReentregaMesmaCarga) {
            _analise.Finalizar.visible(status);
            _analise.TratativaDevolucao.visible(true);
            _analise.MotivoDaDevolucao.visible(true);
            _analise.SalvarAnalise.visible(status);
            _analise.Anexo.visible(status);
            _analise.TratativaDevolucao.enable(status);
            _analise.MotivoDaDevolucao.enable(status);
            _analise.ClienteNovaEntrega.enable(status);
            _analise.TipoDevolucao.enable(status);
            _analise.ObservacaoRetornoMotorista.enable(status);
            _analise.DataReentregaMesmaCarga.enable(status);
            _analise.InformarDadosChamadoFinalizadoComCusto.enable(status);
            _analise.CodigoSIF.enable(status);
            _analise.FreteRetornoDevolucao.enable(status);
            tratarCampoSenha(null, status);
        } else {
            _analise.TratativaDevolucao.visible(false);
            _analise.MotivoDaDevolucao.visible(false);
            _analise.SalvarAnalise.visible(false);
            _analise.Finalizar.visible(status && (VerificaSePossuiPermissaoEspecial(EnumPermissaoPersonalizada.ChamadoOcorrencia_PermitirLiberarParaOcorrencia, _PermissoesPersonalizadasChamado) || _CONFIGURACAO_TMS.TipoServicoMultisoftware != EnumTipoServicoMultisoftware.MultiTMS));
            _analise.Fechar.visible(status && (VerificaSePossuiPermissaoEspecial(EnumPermissaoPersonalizada.ChamadoOcorrencia_PermitirFecharSemOcorrencia, _PermissoesPersonalizadasChamado) || _CONFIGURACAO_TMS.TipoServicoMultisoftware != EnumTipoServicoMultisoftware.MultiTMS));
            _analise.Cancelar.visible(status);
            if (_CONFIGURACAO_TMS.TipoServicoMultisoftware === EnumTipoServicoMultisoftware.MultiTMS)
                _analise.Recusar.visible(status);

            if (_motivoChamadoConfiguracao.MotivoReentrega || _motivoChamadoConfiguracao.MotivoRetencao) {
                _analise.Fechar.visible(false);
                _analise.Recusar.visible(false);
                _analise.ResponsavelOcorrencia.visible(false);
                _analise.Representante.visible(false);
            } else if (_motivoChamadoConfiguracao.PermiteRetornarParaAjuste)
                _analise.Rejeitar.visible(status);
        }
    }
    else if (_motivoChamadoConfiguracao.MotivoDevolucao) {
        _analise.TratativaDevolucao.visible(true);
        _analise.MotivoDaDevolucao.visible(true);
        _analise.TratativaDevolucao.enable(false);
        _analise.MotivoDaDevolucao.enable(false);
        _analise.TipoDevolucao.enable(false);
        _analise.FreteRetornoDevolucao.enable(false);
        tratarCampoSenha(null, status);

        ControleCamposNFeDevolucaoAnalise();
    }

    if (_motivoChamadoConfiguracao.TratativaDeveSerConfirmadaPeloCliente && _CONFIGURACAO_TMS.TipoServicoMultisoftware != EnumTipoServicoMultisoftware.Fornecedor) {
        _analise.Fechar.visible(false);
        _analise.Finalizar.visible(false);
        _analise.Responder.visible(true);
        _analise.LiberarParaCliente.visible(true);
    } else {
        _analise.LiberarParaCliente.visible(false);
    }

    if (_CONFIGURACAO_TMS.TipoServicoMultisoftware === EnumTipoServicoMultisoftware.MultiTMS)
        _analise.SituacaoTratativa.visible(status);

    _controleEntregaDevolucaoChamado._carregado.then(function () {
        _controleEntregaDevolucaoChamado.controlarEdicaoHabilitada(_analise.SalvarAnalise.visible());
    });

    if (_motivoChamadoConfiguracao.PermitirAtualizarInformacoesPedido)
        _analise.DataPrevisaoEntregaPedidos.enable(status);
}

function ObterTratativasDevolucao() {
    var p = new promise.Promise();

    executarReST("ChamadoAnalise/BuscarTratativasDevolucao", {
        Tipos: JSON.stringify([
            EnumTratativaDevolucao.Rejeitada,
            EnumTratativaDevolucao.Revertida,
            EnumTratativaDevolucao.Reentregue,
            EnumTratativaDevolucao.EntregarEmOutroCliente,
            EnumTratativaDevolucao.DescartarMercadoria,
            EnumTratativaDevolucao.QuebraPeso,
            EnumTratativaDevolucao.ReentregarMesmaCarga])
    }, function (r) {
        if (r.Success) {
            _configuracaoTratativaDevolucao = new Array();

            for (var i = 0; i < r.Data.length; i++)
                _configuracaoTratativaDevolucao.push({ value: r.Data[i].Codigo, text: r.Data[i].Descricao });
        } else {
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Chamado.ChamadoOcorrencia.Falha, r.Msg);
        }

        p.done();
    });

    return p;
};

function setarVisibilidadeCampos() {
    if (_CONFIGURACAO_TMS.TipoChamado == EnumTipoChamado.PadraoEmbarcador) {
        _analise.ResponsavelOcorrencia.visible(false);
        _analise.Representante.visible(false);
        _abertura?.ResponsavelChamado.visible(true);
        _abertura?.NumeroPallet.visible(true);
        _abertura?.QuantidadeItens.visible(true);
    }
    else {
        if (_CONFIGURACAO_TMS.TipoServicoMultisoftware === EnumTipoServicoMultisoftware.MultiTMS) {
            _analise.ResponsavelOcorrencia.visible(false);
            _analise.Representante.visible(false);
        } else if (_CONFIGURACAO_TMS.TipoServicoMultisoftware === EnumTipoServicoMultisoftware.MultiCTe) {
            _analise.Representante.visible(false);
        }
        else {
            _analise.ResponsavelOcorrencia.visible(true);
            _analise.Representante.visible(true);
        }
        _abertura?.ResponsavelChamado.visible(false);
        _abertura?.NumeroPallet.visible(false);
        _abertura?.QuantidadeItens.visible(false);
    }
}

function MostrarJustificativaOcorrencia(permitir) {
    if (permitir) {
        _analise.JustificativaOcorrencia.visible(true);
        _analise.JustificativaOcorrencia.required(true);
    }
    else {
        _analise.JustificativaOcorrencia.visible(false);
        _analise.JustificativaOcorrencia.required(false);
    }
}

function TextoResposta() {
    var str = Localization.Resources.Chamado.ChamadoOcorrencia.RetornarChamadoAo;
    if (_CONFIGURACAO_TMS.TipoServicoMultisoftware == EnumTipoServicoMultisoftware.MultiCTe)
        str += "Embarcador";
    else
        str += Localization.Resources.Chamado.ChamadoOcorrencia.Transportador;

    return str;
}

function AbrirModalArvore() {
    const CodigoMotivo = _abertura.MotivoChamado.codEntity();
    executarReST("MotivoChamado/BuscarArvorePorCodigoMotivo", { CodigoMotivo, CodigoAnalisis: _analise.Codigo.val(), CodigoCausa: _chamadoOcorrenciaModalArvore.Causas.val() }, function (arg) {
        if (!arg.Success)
            return exibirMensagem(tipoMensagem.falha, "Erro", arg.Msg);
        limparCampos();
        _chamadoOcorrenciaModalArvore.Causas.visible(false);
        _primeraEntrada = true;
        MontarArvoreSalva(arg.Data.ListaArvore, arg.Data.Causas, arg.Data.CausaSelecionada);
        _primeraEntrada = false;
        MontarRespostasArvore();

    })

}


function MontarArvoreSalva(lista, causas, causaSelecionada) {
    if (lista == null) {
        return exibirMensagem(tipoMensagem, "Erro", Localization.Resources.Chamado.ChamadoOcorrencia.ArvoreNaoEncontrada)
    }
    $("#ModalArvorePerguntas").modal("show");
    _chamadoOcorrenciaModalArvore.PerguntasArvore.val(lista);
    const perguntasRepondidas = lista.filter(pergunta => pergunta.Situacao == 1);

    if (causas.length > 0) {
        _chamadoOcorrenciaModalArvore.Causas.visible(true);
        PreencherOpcoesCausas(causas, causaSelecionada);
    }

    if (perguntasRepondidas.length == 0)
        return _chamadoOcorrenciaModalArvore.PerguntasRespondida.val([lista[0]])

    _chamadoOcorrenciaModalArvore.PerguntasRespondida.val(perguntasRepondidas);
}

function PreencherOpcoesCausas(causas, causaSelecionada) {
    let listaOpcoesCausas = [{ text: "Selecione", value: 0 }];
    causas.map(function (d) {
        listaOpcoesCausas.push({
            text: d.Descricao,
            value: d.Codigo
        })
    });

    _chamadoOcorrenciaModalArvore.Causas.options(listaOpcoesCausas);

    if (causaSelecionada.Codigo > 0)
        _chamadoOcorrenciaModalArvore.Causas.val(causaSelecionada.Codigo);
}

function ControlePerguntasFilhas(item) {

    if (_primeraEntrada)
        return;

    let valorToogle = $("#switch" + item.Codigo).prop('checked');

    let perguntas = ObterTodasPerguntas();
    let pergunta;

    //Pergunta Sim 
    if (valorToogle) {
        [pergunta] = perguntas.filter(pergunta => pergunta.Pai == item.Key && pergunta.Resposta == EnumTipoPerguntas.Sim);
    }
    //Pergunta Não
    if (!valorToogle) {
        [pergunta] = perguntas.filter(pergunta => pergunta.Pai == item.Key && pergunta.Resposta == EnumTipoPerguntas.Nao);
    }

    if (pergunta == null)
        return RemoverRespostAnterior(item.Key);

    InserirPergunta(pergunta, item.Key);
}

function ObterTodasPerguntas() {
    return _chamadoOcorrenciaModalArvore.PerguntasArvore.val();
}

function ObterPerguntasRepondidas() {
    return _chamadoOcorrenciaModalArvore.PerguntasRespondida.val();
}

function InserirPergunta(itemPergunta, keyPai) {

    const perguntasRepondidas = ObterPerguntasRepondidas();

    let [existeRepostaFilhaRespondida] = perguntasRepondidas.filter(pergunta => (pergunta.Pai == keyPai && itemPergunta.Resposta != pergunta.Resposta))

    if (!existeRepostaFilhaRespondida) {
        perguntasRepondidas.push(itemPergunta);
        return _chamadoOcorrenciaModalArvore.PerguntasRespondida.val(perguntasRepondidas);
    } else {

        for (var i = 0; i < perguntasRepondidas.length; i++) {
            if (perguntasRepondidas[i].Key == existeRepostaFilhaRespondida.Key) {
                perguntasRepondidas.splice(i);
                break;
            }
        }
        perguntasRepondidas.push(itemPergunta);
        return _chamadoOcorrenciaModalArvore.PerguntasRespondida.val(perguntasRepondidas);
    }

}

function AtivarPerguntaRespondia() {
    const perguntas = ObterPerguntasRepondidas();

    for (var i = 0; i < perguntas.length; i++)
        perguntas[i].Situacao = 1;
}

function RemoverRespostAnterior(idPai) {

    const perguntasRepondidas = ObterPerguntasRepondidas();

    for (var i = 0; i < perguntasRepondidas.length; i++) {
        if (perguntasRepondidas[i].Pai == idPai) {
            perguntasRepondidas.splice(i);
            break;
        }
    }
    _chamadoOcorrenciaModalArvore.PerguntasRespondida.val(perguntasRepondidas);
}

function VisibilidadeDoCheck(key) {
    const perguntas = ObterTodasPerguntas()

    if (!perguntas)
        return

    const [exiteRespostaDestaPergunta] = perguntas.filter(pergunta => pergunta.Pai == key);
    return exiteRespostaDestaPergunta ? true : false;
}

function SalvarArvore() {
    const Arvore = ObterArvoreRespondida();
    const CodigoMotivo = _abertura.MotivoChamado.codEntity();
    const CodigoCausa = _chamadoOcorrenciaModalArvore.Causas.val();

    if (CodigoCausa == 0) {
        exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.CamposObrigatorios, Localization.Resources.Chamado.ChamadoOcorrencia.NecessarioIncluirUmaCausaNaArvore);
        return;
    }

    executarReST("MotivoChamado/SalvarArvoreAtendimento", { Arvore, CodigoMotivo, CodigoAnalisis: _analise.Codigo.val(), CodigoCausa }, function (arg) {

        if (!arg.Success)
            return exibirMensagem(tipoMensagem.falha, "Error", arg.Msg);

        exibirMensagem(tipoMensagem.ok, Localization.Resources.Chamado.ChamadoOcorrencia.Sucesso, Localization.Resources.Chamado.ChamadoOcorrencia.ArvoreSalvaComSucesso);
        $("#ModalArvorePerguntas").modal("hide");
        limparCampos();
    });

}
function ObterArvoreRespondida() {
    AtivarPerguntaRespondia();
    let arvore = ObterPerguntasRepondidas();
    return JSON.stringify(arvore);
}

function limparCampos() {
    LimparCampo(_chamadoOcorrenciaModalArvore.Codigo);
    LimparCampo(_chamadoOcorrenciaModalArvore.PerguntasRespondida);
    AtivarRelogioAlertaDeNivelChamado();
}

function MontarRespostasArvore() {
    const lista = ObterPerguntasRepondidas();
    for (var i = 0; i < lista.length; i++) {
        const reposta = lista[i].Resposta;

        if (reposta == 0)
            continue;

        if (!lista[i - 1])
            continue;

        const codigoPerguntaPai = lista[i - 1].Codigo;
        $("#switch" + codigoPerguntaPai).prop('checked', reposta == EnumTipoPerguntas.Sim ? true : false);
    }
}

function VisibilidadeBotoesArvore() {
    if (_resumoChamado.Situacao.val() != "Finalizado")
        return;

    _chamadoOcorrenciaModalArvore.Salvar.visible(false);
    _chamadoOcorrenciaModalArvore.Finalizar.visible(false);
}

function FinalizarAtendimento() {

    const Arvore = ObterPerguntasRepondidas();
    const PerguntaFinalizadora = Arvore.at(-1);

    if (PerguntaFinalizadora.StatusFinalizacaoAtendimento == EnumStatusFinalizacaoAtendimento.Cancelar) {

        return exibirConfirmacao(Localization.Resources.Gerais.Geral.Confirmacao, Localization.Resources.Chamado.ChamadoOcorrencia.VoceRealmenteDesejaCancelarEsseAtendimento, function () {
            executarReST("ChamadoAnalise/CancelarChamado", { Codigo: _analise.Codigo.val(), MotivoRecusaCancelamento: 0 }, function (arg) {
                if (!arg.Success)
                    return exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, arg.Msg);

                if (arg.Data) {
                    _chamado.Situacao.val(EnumSituacaoChamado.Cancelada);
                    SetarEtapaChamado();
                    SalvarArvore();
                } else {
                    exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.Aviso, arg.Msg);
                }

            });
        });

    }

    if (PerguntaFinalizadora.StatusFinalizacaoAtendimento == EnumStatusFinalizacaoAtendimento.Finalizar) {
        return exibirConfirmacao(Localization.Resources.Gerais.Geral.Confirmacao, Localization.Resources.Chamado.ChamadoOcorrencia.VoceRealmenteDesejaFinalizarEsseAtendimento, function () {
            var data = {
                Codigo: _analise.Codigo.val(),
                PessoaTituloPagar: _informacaoPagamentoMotorista.Fornecedor.codEntity()
            };
            executarReST("ChamadoAnalise/FinalizarChamado", data, function (arg) {
                if (arg.Success) {
                    if (arg.Data) {
                        _chamado.Situacao.val(EnumSituacaoChamado.Finalizado);
                        SetarEtapaChamado();
                        recarregarGridChamados();
                        SalvarArvore();
                    } else {
                        exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.Aviso, arg.Msg);
                    }
                } else {
                    exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, arg.Msg);
                }
            });
        });
    }

    return exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Aviso, Localization.Resources.Chamado.ChamadoOcorrencia.EstaPerguntaNaoEFinalizadoraDeChamado)

}

function EscalarNivel() {
    exibirConfirmacao(Localization.Resources.Gerais.Geral.Confirmacao, Localization.Resources.Chamado.ChamadoOcorrencia.DesejaEscalarAtendimentoSeraAtualizadoProximoNivel, function () {
        if (_motivoChamadoConfiguracao.PossibilitarInclusaoAnexoAoEscalarAtendimento)
            exibirConfirmacao(Localization.Resources.Chamado.ChamadoOcorrencia.Confirmacao, Localization.Resources.Chamado.ChamadoOcorrencia.DesejaAdicionarAnexosAEscaladaDoAtendimento, function () {
                _anexoNivelAtendimento.Adicionar.visible(true);
                _anexoNivelAtendimento.Arquivo.visible(true);
                _anexoNivelAtendimento.Descricao.visible(true);
                _anexoNivelAtendimento.DescricaoModal.text(Localization.Resources.Chamado.ChamadoOcorrencia.AdicionarAnexosAEscalada);
                $("#grids-anexos").hide();

                Global.abrirModal('divModalAnexoEscalada');

            }, function () {
                executarRestEscalarNivel();
            });
        else {
            executarRestEscalarNivel();
        }
    });
}

function MostrarContactoResponsavel() {
    Global.abrirModal("ModalResponsaveisNivel");
}

function RetornarParaNivelUm() {
    exibirConfirmacao(Localization.Resources.Chamado.ChamadoOcorrencia.Confirmacao, Localization.Resources.Chamado.ChamadoOcorrencia.DesejaRetornarOAtendimentoParaNivel1, function () {
        executarRestRetornarNivelUm()
    });
}

function exibirAnexosEscaladas() {
    exibirAnexos();
    Global.abrirModal('divModalAnexoEscalada');

}

function liberarChamadoOcorrenciaClick() {

    exibirConfirmacao("Confirmação", "Aprovar valor maior do que o valor do pedido ?", function () {
        var data = {
            Codigo: _analise.Codigo.val()
        };
        executarReST("ChamadoOcorrencia/AprovarValorCargaDescarga", data, function (arg) {
            if (arg.Success && arg.Data) {
                valido = true;
            } else {
                exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, arg.Msg);
                valido = false;
            }
        });
    });
}

function NaoExigirJustificativaOcorrenciaChamadoAoSalvarObservacao() {
    if (_motivoChamadoConfiguracao.NaoExigirJustificativaOcorrenciaChamadoAoSalvarObservacao && _analise.Observacao.val()) {
        _analise.JustificativaOcorrencia.required(false);
        _analise.JustificativaOcorrencia.text(Localization.Resources.Chamado.ChamadoOcorrencia.JustificativaOcorrencia.getFieldDescription());
    } else {
        _analise.JustificativaOcorrencia.required(true);
        _analise.JustificativaOcorrencia.text(Localization.Resources.Chamado.ChamadoOcorrencia.JustificativaOcorrencia.getRequiredFieldDescription());
    }
}

function NaoExigirJustificativaOcorrenciaChamadoAoSalvarObservacao() {
    if (_motivoChamadoConfiguracao.NaoExigirJustificativaOcorrenciaChamadoAoSalvarObservacao && _analise.Observacao.val()) {
        _analise.JustificativaOcorrencia.required(false);
        _analise.JustificativaOcorrencia.text(Localization.Resources.Chamado.ChamadoOcorrencia.JustificativaOcorrencia.getFieldDescription());
    } else {
        _analise.JustificativaOcorrencia.required(true);
        _analise.JustificativaOcorrencia.text(Localization.Resources.Chamado.ChamadoOcorrencia.JustificativaOcorrencia.getRequiredFieldDescription());
    }
}

function executarRestEscalarNivel() {
    executarReST("ChamadoOcorrencia/EscalarAtendimento", { Codigo: _chamado.Codigo.val(), NivelAtual: _chamado.NivelAtendimento.val() }, (arg) => {
        if (!arg.Success)
            return exibirMensagem(tipoMensagem.falha, 'Erro', arg.Msg);

        if (arg.Success && arg.Msg)
            return exibirMensagem(tipoMensagem.atencao, Localization.Resources.Chamado.ChamadoOcorrencia.Atencao, arg.Msg);

        buscarChamadoPorCodigo(_chamado.Codigo.val());
        return exibirMensagem(tipoMensagem.ok, Localization.Resources.Chamado.ChamadoOcorrencia.Sucesso, Localization.Resources.Chamado.ChamadoOcorrencia.NivelEscaladoComSucesso);
    })
}

function executarRestRetornarNivelUm() {
    executarReST("ChamadoOcorrencia/RetornarNivelUm", { Codigo: _chamado.Codigo.val() }, (arg) => {
        if (!arg.Success)
            return exibirMensagem(tipoMensagem.falha, 'Erro', arg.Msg);

        if (arg.Success && arg.Msg)
            return exibirMensagem(tipoMensagem.atencao, Localization.Resources.Chamado.ChamadoOcorrencia.Atencao, arg.Msg);

        buscarChamadoPorCodigo(_chamado.Codigo.val());
        return exibirMensagem(tipoMensagem.ok, Localization.Resources.Chamado.ChamadoOcorrencia.Sucesso, Localization.Resources.Chamado.ChamadoOcorrencia.NivelRetornadoComSucesso);
    })
}

function tratarCampoSenha(valorTratativaDevolucao, status) {
    let tratativa = valorTratativaDevolucao !== null && valorTratativaDevolucao !== undefined ? valorTratativaDevolucao : _analise.TratativaDevolucao.val();
    let podeEditar = (status !== null && status !== undefined) ? status : !!_analise.TratativaDevolucao.enable();

    if (_motivoChamadoConfiguracao.HabilitarSenhaDevolucao) {
        _analise.SenhaDevolucao.visible(true);

        if (podeEditar && tratativa == EnumTratativaDevolucao.Rejeitada) {
            _analise.SenhaDevolucao.enable(true);
        } else {
            _analise.SenhaDevolucao.enable(false);
        }
    } else {
        _analise.SenhaDevolucao.visible(false);
        _analise.SenhaDevolucao.enable(false);
    }
}

function controleStatusCamposCriticidade(valorCritico, status) {
    if (valorCritico !== null) {
        _analise.Gerencial.enable(valorCritico);
        _analise.CausaProblema.enable(valorCritico);
        _analise.FUP.enable(valorCritico);
    } else if (status !== null && _analise.Critico.val() === 0) {
        _analise.Gerencial.enable(false);
        _analise.CausaProblema.enable(false);
        _analise.FUP.enable(false);
    }else {
        _analise.Critico.enable(status);
        _analise.Gerencial.enable(status);
        _analise.CausaProblema.enable(status);
        _analise.FUP.enable(status);
    }
}

function salvarEstadia() {
    exibirConfirmacao(Localization.Resources.Gerais.Geral.Confirmacao, Localization.Resources.Chamado.ChamadoOcorrencia.DesejaRealmenteAtualizarEstadia, function () {
        executarReST("ChamadoOcorrencia/SalvarEstadia", { Codigo: _chamado.Codigo.val(), Estadia: _analise.Estadia.val() }, (arg) => {
            if (!arg.Success)
                return exibirMensagem(tipoMensagem.falha, 'Erro', arg.Msg);

            return exibirMensagem(tipoMensagem.ok, Localization.Resources.Chamado.ChamadoOcorrencia.Sucesso, Localization.Resources.Chamado.ChamadoOcorrencia.EstadiaAtualizadaComSucesso);
        });
    });
}

// #endregion