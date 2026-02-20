/// <reference path="AutorizarRegras.js" />
/// <reference path="Anexos.js" />
/// <reference path="Delegar.js" />
/// <reference path="HistoricoAutorizacao.js" />
/// <reference path="../../Consultas/Justificativa.js" />
/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/bootstrap/bootstrap.js" />
/// <reference path="../../../js/libs/jquery.blockui.js" />
/// <reference path="../../../js/Global/knoutViewsSlides.js" />
/// <reference path="../../../js/libs/jquery.maskMoney.js" />
/// <reference path="../../../js/plugin/datatables/jquery.dataTables.js" />
/// <reference path="../../Consultas/Usuario.js" />
/// <reference path="../../Consultas/TipoOcorrencia.js" />
/// <reference path="../../Consultas/Filial.js" />
/// <reference path="../../Consultas/Tranportador.js" />
/// <reference path="../../Consultas/MotivoRejeicaoOcorrencia.js" />
/// <reference path="../../Enumeradores/EnumSituacaoOcorrencia.js" />
/// <reference path="../../Enumeradores/EnumEtapaAutorizacaoOcorrencia.js" />
/// <reference path="../../Enumeradores/EnumResponsavelOcorrencia.js" />
/// <reference path="../../Enumeradores/EnumAprovacaoRejeicao.js" />
/// <reference path="../../Enumeradores/EnumAutorizacaoOcorrenciaPagamento.js" />
/// <reference path="../../Global/Notificacoes/Notificacao.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _pesquisaEmissaoCTePortoLote;
var _gridEmissaoCTePortoLote;

var PesquisaEmissaoCTePortoLote = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.NavioViagemDirecao = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "*Navio/Viagem/Direção:", idBtnSearch: guid(), required: true });
    this.PortoOrigem = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "*Porto de Origem:", idBtnSearch: guid(), required: true });
    this.PortoDestino = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Porto de Destino:", idBtnSearch: guid() });

    this.SelecionarTodos = PropertyEntity({ val: ko.observable(false), def: false, type: types.map, getType: typesKnockout.bool, text: "Marcar/Desmarcar Todos", visible: ko.observable(true), enable: ko.observable(true) });
    this.ConfirmarEnvioDosDocumentos = PropertyEntity({ eventClick: confirmarEnvioDosDocumentos, type: types.event, text: "Confirmar o Envio do(s) Documento(s)", visible: ko.observable(true) });
    this.Limpar = PropertyEntity({ eventClick: limparClick, type: types.event, text: "Limpar / Novo", visible: ko.observable(true) });

    this.Pesquisar = PropertyEntity({
        eventClick: function () {
            recarregarGridPesquisaEmissaoCTePortoLote();
        }, type: types.event, text: "Pesquisar", idGrid: guid(), visible: ko.observable(true)
    });

    this.ExibirFiltros = PropertyEntity({
        eventClick: function (e) {
            e.ExibirFiltros.visibleFade(!e.ExibirFiltros.visibleFade());
        }, type: types.event, text: "Filtros de Pesquisa", idFade: guid(), visibleFade: ko.observable(false), visible: ko.observable(true)
    });
};

//*******EVENTOS******* 

function loadEmissaoCTePortoLote() {
    _pesquisaEmissaoCTePortoLote = new PesquisaEmissaoCTePortoLote();
    KoBindings(_pesquisaEmissaoCTePortoLote, "knockoutPesquisaEmissaoCTePortoLote", false, _pesquisaEmissaoCTePortoLote.Pesquisar.id);

    new BuscarPedidoViagemNavio(_pesquisaEmissaoCTePortoLote.NavioViagemDirecao);
    new BuscarPorto(_pesquisaEmissaoCTePortoLote.PortoOrigem);
    new BuscarPorto(_pesquisaEmissaoCTePortoLote.PortoDestino);

    loadGridEmissaoCTePortoLote();
}

//*******MÉTODOS*******

function loadGridEmissaoCTePortoLote() {
    var multiplaescolha = {
        basicGrid: null,
        callbackSelecionado: null,
        callbackNaoSelecionado: null,
        eventos: function () { },
        selecionados: new Array(),
        naoSelecionados: new Array(),
        SelecionarTodosKnout: _pesquisaEmissaoCTePortoLote.SelecionarTodos,
        somenteLeitura: false
    };

    var configExportacao = {
        url: "ConfirmarDocumentoEmLote/ExportarPesquisa",
        titulo: "Baixar em XLS"
    };

    _gridEmissaoCTePortoLote = new GridView(_pesquisaEmissaoCTePortoLote.Pesquisar.idGrid, "ConfirmarDocumentoEmLote/Pesquisa", _pesquisaEmissaoCTePortoLote, null, null, 25, null, null, null, multiplaescolha, null, null, configExportacao);
    _gridEmissaoCTePortoLote.CarregarGrid();
}

function confirmarEnvioDosDocumentos() {
    var cargasSelecionadas = _gridEmissaoCTePortoLote.ObterMultiplosSelecionados();

    var codigosCarga = new Array();
    for (var i = 0; i < cargasSelecionadas.length; i++)
        codigosCarga.push(cargasSelecionadas[i].Codigo);

    if (codigosCarga && codigosCarga.length > 0) {
        var data = {
            Codigos: JSON.stringify(codigosCarga)
        };
    };

    executarReST("ConfirmarDocumentoEmLote/ConfirmarEnvioDosDocumentosFiscais", data, function (arg) {
        if (arg.Success) {
            if (arg.Data !== false) {
                exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, "Envio dos documentos realizado com sucesso.");
                _gridEmissaoCTePortoLote.CarregarGrid();
            } else {
                exibirMensagem(tipoMensagem.atencao, "Atenção", arg.Msg, 20000);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    });
}

function limparClick() {
    LimparCampos(_pesquisaEmissaoCTePortoLote);
    _gridEmissaoCTePortoLote.CarregarGrid();
}

function recarregarGridPesquisaEmissaoCTePortoLote() {
    if (ValidarCamposObrigatorios(_pesquisaEmissaoCTePortoLote)) {
        _gridEmissaoCTePortoLote.CarregarGrid();
    } else {
        exibirMensagem("atencao", "Campos Obrigatórios", "Por favor, informe os campos obrigatórios.");
    }
}