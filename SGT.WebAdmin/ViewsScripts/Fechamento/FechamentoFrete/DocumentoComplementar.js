/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Globais.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Rest.js" />

// #region Objetos Globais do Arquivo

var _fechamentoFreteCTeComplementar;
var _fechamentoFreteDocumentoParaEmissaoNFSManual;
var _gridFechamentoFreteCTeComplementar;
var _gridFechamentoFreteDocumentoParaEmissaoNFSManual;

// #endregion Objetos Globais do Arquivo

// #region Classes

var FechamentoFreteCTeComplementar = function () {
    this.PossuiCTeComplementar = PropertyEntity({ getType: typesKnockout.bool, val: ko.observable(false), def: false });

    this.Pesquisar = PropertyEntity({ eventClick: recarregarGridFechamentoFreteCTeComplementar, type: types.event, text: "Buscar / Atualizar", visible: ko.observable(true) });
    this.ReenviarTodos = PropertyEntity({ eventClick: reenviarCTesComplementaresRejeitadosClick, type: types.event, text: "Reemitir CT-es Rejeitados", visible: ko.observable(_fechamentoFrete.Situacao.val() == EnumSituacaoFechamentoFrete.PendenciaEmissao) });
}

var FechamentoFreteDocumentoParaEmissaoNFSManual = function () {
    this.PossuiDocumentoParaEmissaoNFSManual = PropertyEntity({ getType: typesKnockout.bool, val: ko.observable(false), def: false });

    this.Pesquisar = PropertyEntity({ eventClick: recarregarGridFechamentoFreteDocumentoParaEmissaoNFSManual, type: types.event, text: "Buscar / Atualizar", visible: ko.observable(true) });
}

// #endregion Classes

// #region Funções de Inicialização

function loadFechamentoFreteDocumentoComplementar() {
    _fechamentoFreteCTeComplementar = new FechamentoFreteCTeComplementar();
    KoBindings(_fechamentoFreteCTeComplementar, "knockoutFechamentoFreteCTeComplementar");

    _fechamentoFreteDocumentoParaEmissaoNFSManual = new FechamentoFreteDocumentoParaEmissaoNFSManual();
    KoBindings(_fechamentoFreteDocumentoParaEmissaoNFSManual, "knockoutFechamentoFreteDocumentoParaEmissaoNFSManual");

    loadGridFechamentoFreteCTeComplementar();
    loadGridFechamentoFreteDocumentoParaEmissaoNFSManual();
}

function loadGridFechamentoFreteCTeComplementar() {
    var baixarXMLNFSe = { descricao: "Baixar XML", id: guid(), metodo: baixarXMLCTeClick, icone: "", visibilidade: VisibilidadeOpcaoDownloadDANFSE };
    var baixarDANFSE = { descricao: "Baixar DANFSE", id: guid(), metodo: baixarDacteClick, icone: "", visibilidade: VisibilidadeOpcaoDownloadDANFSE };
    var EnviarXMLCancelamento = { descricao: "Enviar XML Cancelamento", id: guid(), metodo: enviarXMLCancelamentoClick, icone: "", visibilidade: VisibilidadeEnviarXMLCancelamento };
    var baixarDACTE = { descricao: "Baixar DACTE", id: guid(), metodo: baixarDacteClick, icone: "", visibilidade: VisibilidadeOpcaoDownload };
    var baixarPDF = { descricao: "Baixar PDF", id: guid(), metodo: baixarDacteClick, icone: "", visibilidade: VisibilidadeDownloadOutrosDoc };
    var baixarXML = { descricao: "Baixar XML", id: guid(), metodo: baixarXMLCTeClick, icone: "", visibilidade: VisibilidadeOpcaoDownload };
    var retornoSefaz = { descricao: "Mensagem Sefaz", id: guid(), metodo: retoronoSefazClick, icone: "", visibilidade: VisibilidadeMensagemSefaz };
    var emitir = { descricao: "Emitir", id: guid(), metodo: function (datagrid) { emitirCTeClick(datagrid); }, icone: "", visibilidade: VisibilidadeRejeicao };

    if (_CONFIGURACAO_TMS.TipoServicoMultisoftware != EnumTipoServicoMultisoftware.Terceiros) {
        var visualizar = { descricao: "Detalhes", id: guid(), metodo: detalhesCTeClick, icone: "", visibilidade: VisibilidadeOpcaoDownload };
        var menuOpcoes = { tipo: TypeOptionMenu.list, descricao: "Opções", tamanho: 7, opcoes: [baixarDACTE, baixarDANFSE, baixarXML, baixarXMLNFSe, baixarPDF, retornoSefaz, visualizar, emitir, EnviarXMLCancelamento] };
    }
    else
        var menuOpcoes = { tipo: TypeOptionMenu.list, descricao: "Opções", tamanho: 7, opcoes: [baixarDACTE, baixarDANFSE, baixarXML, baixarXMLNFSe, baixarPDF, retornoSefaz, emitir] };

    _gridFechamentoFreteCTeComplementar = new GridView("grid-fechamento-frete-cte-complementar", "FechamentoFrete/ConsultarCTesFechamento", _fechamentoFrete, menuOpcoes, null);
}

function loadGridFechamentoFreteDocumentoParaEmissaoNFSManual() {
    _gridFechamentoFreteDocumentoParaEmissaoNFSManual = new GridView("grid-fechamento-frete-documento-para-emissao-nfs-manual", "FechamentoFrete/ConsultarDocumentoParaEmissaoNFSManual", _fechamentoFrete);
}

// #endregion Funções de Inicialização

// #region Funções Associadas a Eventos

function reenviarCTesComplementaresRejeitadosClick() {
    executarReST("FechamentoFrete/AutorizarEmissaoCTes", { Codigo: _fechamentoFrete.Codigo.val() }, function (retorno) {
        if (retorno.Success) {
            exibirMensagem(tipoMensagem.ok, "Sucesso", retorno.Msg);
            recarregarGridFechamentoFreteCTeComplementar();
        }
        else
            exibirMensagem(tipoMensagem.aviso, "Aviso", retorno.Msg);
    });
}

// #endregion Funções Associadas a Eventos

// #region Funções Públicas

function preencherFechamentoFreteDocumentoComplementar() {
    recarregarGridFechamentoFreteCTeComplementar();
    recarregarGridFechamentoFreteDocumentoParaEmissaoNFSManual();
}

// #endregion Funções Públicas

// #region Funções Privadas

function recarregarGridFechamentoFreteCTeComplementar() {
    _gridFechamentoFreteCTeComplementar.CarregarGrid(function () {
        _fechamentoFreteCTeComplementar.PossuiCTeComplementar.val(_gridFechamentoFreteCTeComplementar.NumeroRegistros() > 0);
    });
}

function recarregarGridFechamentoFreteDocumentoParaEmissaoNFSManual() {
    _gridFechamentoFreteDocumentoParaEmissaoNFSManual.CarregarGrid(function () {
        _fechamentoFreteDocumentoParaEmissaoNFSManual.PossuiDocumentoParaEmissaoNFSManual.val(_gridFechamentoFreteDocumentoParaEmissaoNFSManual.NumeroRegistros() > 0);
    });
}

// #endregion Funções Privadas
