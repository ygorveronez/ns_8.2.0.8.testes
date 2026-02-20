/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="../../Enumeradores/EnumSituacaoEtapaFluxoGestaoPatio.js" />
/// <reference path="../../Enumeradores/EnumEtapaFluxoGestaoPatio.js" />
/// <reference path="AnexosProdutorAnexo.js" />
/// <reference path="../DocumentosPesagem/DocumentosPesagem.js" />
/// <reference path="../DocumentosPesagem/DocumentosPesagemAnexoFornecedor.js" />
/// <reference path="../DocumentosPesagem/DocumentosPesagemNotaFiscalComplementar.js" />
/// <reference path="../DocumentosPesagem/DocumentosPesagemDevolucao.js" />
/// <reference path="../DocumentosPesagem/DocumentosPesagemTicketBalanca.js" />

var _pesquisaCargasAnexoProdutor;
var _gridCargasAnexosProdutor;

var _situacoes = [
    { text: "Todas", value: "" },
    { text: "Em Andamento", value: EnumSituacaoEtapaFluxoGestaoPatio.Aguardando },
    { text: "Finalizada", value: EnumSituacaoEtapaFluxoGestaoPatio.Aprovado },
];

var PesquisaCargasAnexoProdutor = function () {
    this.Carga = PropertyEntity({ text: "Carga: ", val: ko.observable("") });
    this.Situacao = PropertyEntity({ val: ko.observable(EnumSituacaoEtapaFluxoGestaoPatio.Aguardando), options: _situacoes, text: "Situação Fluxo: ", def: EnumSituacaoEtapaFluxoGestaoPatio.Aguardando });

    this.Pesquisar = PropertyEntity({
        eventClick: function (e) {
            pesquisarCargas();
        }, type: types.event, text: "Pesquisar", idGrid: guid(), visible: ko.observable(true)
    });
}

function loadAnexosProdutor() {
    carregarHTMLModaisFluxoPatio(function () {
        LoadDocumentosPesagem();
        _pesquisaCargasAnexoProdutor = new PesquisaCargasAnexoProdutor();
        KoBindings(_pesquisaCargasAnexoProdutor, "knockoutPesquisaAnexosProdutorCargas", false, _pesquisaCargasAnexoProdutor.Pesquisar.id);

        loadGridAnexosProdutor();
    });
}

function loadGridAnexosProdutor() {
    var opcaoAnexos = { descricao: "Anexos", id: guid(), evento: "onclick", metodo: anexosProdutorClick, tamanho: "10", icone: "" };
    var opcaoFechamentoPesagem = { descricao: "Fechamento Pesagem", id: guid(), evento: "onclick", metodo: fechamentoPesagemAnexosProdutorClick, tamanho: "10", icone: "", visibilidade: visibilidadeFechamentoPesagem };
    var menuOpcoes = { tipo: TypeOptionMenu.list, opcoes: [opcaoAnexos, opcaoFechamentoPesagem] };

    var configExportacao = {
        url: "AnexosProdutor/ExportarPesquisa",
        titulo: "Anexos Produtor"
    };

    _gridCargasAnexosProdutor = new GridViewExportacao(_pesquisaCargasAnexoProdutor.Pesquisar.idGrid, "AnexosProdutor/Pesquisa", _pesquisaCargasAnexoProdutor, menuOpcoes, configExportacao, null, 20);
    _gridCargasAnexosProdutor.CarregarGrid();
}

function pesquisarCargas() {
    _gridCargasAnexosProdutor.CarregarGrid();
}

function anexosProdutorClick(e) {
    _documentosPesagem.CodigoFluxoGestaoPatio.val(e.CodigoFluxoGestaoPatio);
    _documentosPesagem.CodigoCarga.val(e.Codigo);

    buscarDocumentosPesagem(e.CodigoFluxoGestaoPatio);
    buscarDocumentosPesagemDevolucao(e.CodigoFluxoGestaoPatio);
    buscarDocumentosPesagemNotaFiscalComplementar(e.CodigoFluxoGestaoPatio);
    buscarDocumentosPesagemTicketBalanca(e.CodigoFluxoGestaoPatio);
    buscarDocumentosFornecedor(e.Codigo);

    Global.abrirModal('divModalDocumentosPesagem');

    $("#divModalDocumentosPesagem").one('hidden.bs.modal', function () {
        LimparCampos(_documentosPesagem);
        _documentosPesagem.Anexos.val([]);
        _listaAnexoFornecedorPesagem.Anexos.val([]);
        Global.ResetarMultiplasAbas();
    });
}

function fechamentoPesagemAnexosProdutorClick(registroSelecionado) {
    Global.abrirModal('divModalFechamentoPesagem');

    $("#divModalFechamentoPesagem").one('hidden.bs.modal', function () {
        LimparCampos(_fechamentoPesagem);
    });

    BuscarFechamentoPesagem(registroSelecionado.CodigoFluxoGestaoPatio);
}

function visibilidadeFechamentoPesagem(registroSelecionado) {
    return registroSelecionado.VisibilidadeFechamentoPesagem;
}