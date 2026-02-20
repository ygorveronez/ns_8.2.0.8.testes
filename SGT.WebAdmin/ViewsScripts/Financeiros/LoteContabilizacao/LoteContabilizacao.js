//*******MAPEAMENTO KNOUCKOUT*******

var _gridLoteContabilizacao;
var _loteContabilizacao;
var _CRUDLoteContabilizacao;
var _pesquisaLoteContabilizacao;

var LoteContabilizacao = function () {
    this.Situacao = PropertyEntity({ val: ko.observable(EnumSituacaoLoteContabilizacao.EmCriacao), def: EnumSituacaoLoteContabilizacao.EmCriacao, text: "Situação: " });
    //this.SituacaoNoCancelamento = PropertyEntity({ val: ko.observable(EnumSituacaoLoteContabilizacao.EmCriacao), options: _situacaoLoteEscrituracao, def: EnumSituacaoLoteEscrituracao.Todas, text: "Situação: " });
};

var CRUDLoteContabilizacao = function () {
    this.Limpar = PropertyEntity({ eventClick: LimparLoteContabilizacaoClick, type: types.event, text: "Limpar (Gerar Novo Lote de Contabilização)", idGrid: guid(), visible: ko.observable(false) });
};

var PesquisaLoteContabilizacao = function () {
    this.DataInicio = PropertyEntity({ text: "Data Inicial: ", getType: typesKnockout.date, visible: true, val: ko.observable() });
    this.DataFim = PropertyEntity({ text: "Data Final: ", getType: typesKnockout.date, visible: true });
    this.DataInicio.dateRangeLimit = this.DataFim;
    this.DataFim.dateRangeInit = this.DataInicio;

    this.Numero = PropertyEntity({ text: "Nº do Lote:", maxlength: 12, enable: ko.observable(true), getType: typesKnockout.int });
    this.NumeroDocumento = PropertyEntity({ text: "Nº do Documento:", maxlength: 15, enable: ko.observable(true) });
    this.Tipo = PropertyEntity({ text: "Tipo:", options: EnumTipoMovimentoExportacao.ObterOpcoesPesquisa(), val: ko.observable(""), def: "", issue: 0, visible: ko.observable(true) });
    this.Empresa = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: ko.observable("Transportador:"), idBtnSearch: guid(), visible: ko.observable(true) });
    this.Tomador = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Tomador:", idBtnSearch: guid(), visible: ko.observable(true) });
    this.Situacao = PropertyEntity({ val: ko.observable(""), options: EnumSituacaoLoteContabilizacao.ObterOpcoesPesquisa(), def: "", text: "Situação: " });

    this.Pesquisar = PropertyEntity({
        eventClick: function (e) {
            _gridLoteContabilizacao.CarregarGrid();
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

function LoadLoteContabilizacao() {
    _loteContabilizacao = new LoteContabilizacao();
    HeaderAuditoria("LoteContabilizacao", _loteContabilizacao);

    _CRUDLoteContabilizacao = new CRUDLoteContabilizacao();
    KoBindings(_CRUDLoteContabilizacao, "knockoutCRUD");

    _pesquisaLoteContabilizacao = new PesquisaLoteContabilizacao();
    KoBindings(_pesquisaLoteContabilizacao, "knockoutPesquisaLoteContabilizacao", false, _pesquisaLoteContabilizacao.Pesquisar.id);

    LoadEtapasLoteContabilizacao();
    LoadSelecaoMovimentos();

    BuscarHTMLIntegracaoLoteContabilizacao();

    new BuscarTransportadores(_pesquisaLoteContabilizacao.Empresa);
    new BuscarClientes(_pesquisaLoteContabilizacao.Tomador);

    BuscarLoteContabilizacao();
}

function LimparLoteContabilizacaoClick(e, sender) {
    LimparCamposLoteContabilizacao();
    GridSelecaoMovimentos();
}


//*******MÉTODOS*******

function BuscarLoteContabilizacao() {
    var editar = { descricao: "Editar", id: guid(), evento: "onclick", metodo: EditarLoteContabilizacaoClick, tamanho: "15", icone: "" };
    var menuOpcoes = {
        tipo: TypeOptionMenu.link,
        tamanho: 7,
        opcoes: [editar]
    };

    _gridLoteContabilizacao = new GridView(_pesquisaLoteContabilizacao.Pesquisar.idGrid, "LoteContabilizacao/Pesquisa", _pesquisaLoteContabilizacao, menuOpcoes);
    _gridLoteContabilizacao.CarregarGrid();
}

function EditarLoteContabilizacaoClick(itemGrid) {
    // Limpa os campos
    LimparCamposLoteContabilizacao();

    // Esconde filtros
    _pesquisaLoteContabilizacao.ExibirFiltros.visibleFade(false);

    // Busca dados
    BuscarLoteContabilizacaoPorCodigo(itemGrid.Codigo);
}

function BuscarLoteContabilizacaoPorCodigo(codigo, cb) {
    executarReST("LoteContabilizacao/BuscarPorCodigo", { Codigo: codigo }, function (arg) {
        if (arg.Data != null) {
            EditarLoteContabilizacao(arg.Data);
            EditarSelecaoMovimentos(arg.Data);
            SetarEtapasLoteContabilizacao();
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    }, null);
}

function EditarLoteContabilizacao(data) {
    _loteContabilizacao.Situacao.val(data.Situacao);
    //_loteEscrituracao.SituacaoNoCancelamento.val(data.SituacaoNoCancelamento);
    _CRUDLoteContabilizacao.Limpar.visible(true);
}

function LimparCamposLoteContabilizacao() {
    LimparCampos(_loteContabilizacao);
    _CRUDLoteContabilizacao.Limpar.visible(false);
    _loteContabilizacao.Situacao.val(EnumSituacaoLoteContabilizacao.EmCriacao);
    SetarEtapasLoteContabilizacao();
    LimparCamposSelecaoMovimentos();
    $("#" + _etapaLoteContabilizacao.Etapa1.idTab).click();
}