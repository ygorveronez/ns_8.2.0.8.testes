/// <reference path="../../Consultas/Filial.js" />
/// <reference path="../../Consultas/Tranportador.js" />
/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/bootstrap/bootstrap.js" />
/// <reference path="../../../js/libs/jquery.blockui.js" />
/// <reference path="../../../js/Global/knoutViewsSlides.js" />
/// <reference path="../../../js/libs/jquery.maskMoney.js" />
/// <reference path="../../../js/plugin/datatables/jquery.dataTables.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _gridLoteEscrituracaoCancelamento;
var _loteEscrituracaoCancelamento;
var _CRUDLoteEscrituracaoCancelamento;
var _pesquisaLoteEscrituracaoCancelamento;

var LoteEscrituracaoCancelamento = function () {
    this.Situacao = PropertyEntity({ val: ko.observable(""), def: "", text: "Situação: " });
    this.SituacaoNoCancelamento = PropertyEntity({ val: ko.observable(""), def: "", options: EnumSituacaoLoteEscrituracaoCancelamento.ObterOpcoesPesquisa(), text: "Situação: " });
};

var CRUDLoteEscrituracaoCancelamento = function () {
    this.Limpar = PropertyEntity({ eventClick: LimparLoteEscrituracaoCancelamentoClick, type: types.event, text: "Limpar (Gerar Novo Lote)", idGrid: guid(), visible: ko.observable(false) });
};

var PesquisaLoteEscrituracaoCancelamento = function () {

    this.DataInicio = PropertyEntity({ text: "Data Inicial: ", getType: typesKnockout.date, visible: true, val: ko.observable() });
    this.DataFim = PropertyEntity({ text: "Data Final: ", getType: typesKnockout.date, visible: true });
    this.DataInicio.dateRangeLimit = this.DataFim;
    this.DataFim.dateRangeInit = this.DataInicio;
    this.Numero = PropertyEntity({ text: "Número Lote:", maxlength: 12, enable: ko.observable(true), getType: typesKnockout.int });
    this.NumeroDOC = PropertyEntity({ text: "Número NF-e:", maxlength: 12, enable: ko.observable(true), getType: typesKnockout.int });

    this.Ocorrencia = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Ocorrência:", idBtnSearch: guid(), visible: ko.observable(true) });
    this.Carga = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Carga:", idBtnSearch: guid(), visible: ko.observable(true) });
    this.Filial = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Filial:", idBtnSearch: guid(), visible: ko.observable(true) });
    this.Empresa = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: ko.observable("Transportador:"), idBtnSearch: guid(), visible: ko.observable(true) });

    this.Tomador = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Tomador:", idBtnSearch: guid(), visible: ko.observable(true) });
    this.LocalidadePrestacao = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Local da Prestação:", idBtnSearch: guid(), visible: ko.observable(true) });
    this.Situacao = PropertyEntity({ val: ko.observable(""), def: "", options: EnumSituacaoLoteEscrituracaoCancelamento.ObterOpcoesPesquisa(), text: "Situação: " });

    this.Pesquisar = PropertyEntity({
        eventClick: function (e) {
            _gridLoteEscrituracaoCancelamento.CarregarGrid();
        }, type: types.event, text: "Pesquisar", idGrid: guid(), visible: ko.observable(true)
    });

    this.ExibirFiltros = PropertyEntity({
        eventClick: function (e) {
            if (e.ExibirFiltros.visibleFade() == true) {
                e.ExibirFiltros.visibleFade(false);
            } else {
                e.ExibirFiltros.visibleFade(true);
            }
        }, type: types.event, text: "Filtros de Pesquisa", idFade: guid(), visibleFade: ko.observable(false), visible: ko.observable(true)
    });

    this.BuscaAvancada = PropertyEntity({
        eventClick: function (e) {
            if (e.BuscaAvancada.visibleFade() == true) {
                e.BuscaAvancada.visibleFade(false);
                e.BuscaAvancada.icon("fal fa-plus");
            } else {
                e.BuscaAvancada.visibleFade(true);
                e.BuscaAvancada.icon("fal fa-minus");
            }
        }, type: types.event, text: "Avançada", idFade: guid(), icon: ko.observable("fal fa-plus"), visibleFade: ko.observable(false), visible: ko.observable(true)
    });
};


//*******EVENTOS*******

function LoadLoteEscrituracaoCancelamento() {
    _loteEscrituracaoCancelamento = new LoteEscrituracaoCancelamento();
    HeaderAuditoria("LoteEscrituracaoCancelamento", _loteEscrituracaoCancelamento);

    _CRUDLoteEscrituracaoCancelamento = new CRUDLoteEscrituracaoCancelamento();
    KoBindings(_CRUDLoteEscrituracaoCancelamento, "knockoutCRUD");

    _pesquisaLoteEscrituracaoCancelamento = new PesquisaLoteEscrituracaoCancelamento();
    KoBindings(_pesquisaLoteEscrituracaoCancelamento, "knockoutPesquisaLoteEscrituracaoCancelamento", false, _pesquisaLoteEscrituracaoCancelamento.Pesquisar.id);

    LoadEtapasLoteEscrituracaoCancelamento();
    LoadSelecaoDocumentosEscrituracaoCancelamento();
    BuscarHTMLIntegracaoLoteEscrituracaoCancelamento();

    new BuscarTransportadores(_pesquisaLoteEscrituracaoCancelamento.Empresa);
    new BuscarCargas(_pesquisaLoteEscrituracaoCancelamento.Carga, null, null, null, null, null, null, null, null, true);
    new BuscarOcorrencias(_pesquisaLoteEscrituracaoCancelamento.Ocorrencia);
    new BuscarClientes(_pesquisaLoteEscrituracaoCancelamento.Tomador);
    new BuscarLocalidades(_pesquisaLoteEscrituracaoCancelamento.LocalidadePrestacao);
    new BuscarFilial(_pesquisaLoteEscrituracaoCancelamento.Filial);

    if (_CONFIGURACAO_TMS.TipoServicoMultisoftware == EnumTipoServicoMultisoftware.MultiTMS) {
        _pesquisaLoteEscrituracaoCancelamento.Filial.visible(false);
        _pesquisaLoteEscrituracaoCancelamento.Empresa.text("Empresa/Filial:");
    }

    BuscarLoteEscrituracaoCancelamento();
}


function LimparLoteEscrituracaoCancelamentoClick(e, sender) {
    LimparCamposLoteEscrituracaoCancelamento();
    GridSelecaoDocumentos();
}


//*******MÉTODOS*******
function BuscarLoteEscrituracaoCancelamento() {
    var editar = { descricao: "Editar", id: guid(), evento: "onclick", metodo: EditarLoteEscrituracaoCancelamento, tamanho: "15", icone: "" };
    var menuOpcoes = {
        tipo: TypeOptionMenu.link,
        opcoes: [editar]
    };

    _gridLoteEscrituracaoCancelamento = new GridView(_pesquisaLoteEscrituracaoCancelamento.Pesquisar.idGrid, "LoteEscrituracaoCancelamento/Pesquisa", _pesquisaLoteEscrituracaoCancelamento, menuOpcoes);
    _gridLoteEscrituracaoCancelamento.CarregarGrid();
}

function EditarLoteEscrituracaoCancelamento(itemGrid) {
    // Limpa os campos
    LimparCamposLoteEscrituracaoCancelamento();

    // Esconde filtros
    _pesquisaLoteEscrituracaoCancelamento.ExibirFiltros.visibleFade(false);

    // Busca dados
    BuscarLoteEscrituracaoCancelamentoPorCodigo(itemGrid.Codigo);
}

function BuscarLoteEscrituracaoCancelamentoPorCodigo(codigo, cb) {
    executarReST("LoteEscrituracaoCancelamento/BuscarPorCodigo", { Codigo: codigo }, function (arg) {
        if (arg.Data != null) {
            SetarDadosLoteEscrituracaoCancelamento(arg.Data);
            EditarSelecaoDocumentos(arg.Data);
            SetarEtapasLoteEscrituracaoCancelamento();
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    }, null);
}

function SetarDadosLoteEscrituracaoCancelamento(data) {
    _loteEscrituracaoCancelamento.Situacao.val(data.Situacao);
    _loteEscrituracaoCancelamento.SituacaoNoCancelamento.val(data.SituacaoNoCancelamento);
    _CRUDLoteEscrituracaoCancelamento.Limpar.visible(true);
}

function LimparCamposLoteEscrituracaoCancelamento() {
    LimparCampos(_loteEscrituracaoCancelamento);
    _loteEscrituracaoCancelamento.Situacao.val("");

    _CRUDLoteEscrituracaoCancelamento.Limpar.visible(false);
    
    SetarEtapasLoteEscrituracaoCancelamento();

    LimparCamposSelecaoDocumentos();

    $("#" + _etapaLoteEscrituracaoCancelamento.Etapa1.idTab).click();
}