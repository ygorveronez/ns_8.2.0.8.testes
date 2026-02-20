/// <reference path="../../Consultas/Empresa.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _gridFechamentoDiario;
var _fechamentoDiario;
var _pesquisaFechamentoDiario;

var PesquisaFechamentoDiario = function () {
    this.DataFechamentoInicial = PropertyEntity({ text: "Data de Fechamento Inicial:", getType: typesKnockout.date });
    this.DataFechamentoFinal = PropertyEntity({ text: "Data de Fechamento Final:", getType: typesKnockout.date });
    this.Empresa = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Empresa:", idBtnSearch: guid(), visible: ko.observable(true) });

    this.DataFechamentoInicial.dateRangeLimit = this.DataFechamentoFinal;
    this.DataFechamentoFinal.dateRangeInit = this.DataFechamentoInicial;

    this.Pesquisar = PropertyEntity({
        eventClick: function (e) {
            _gridFechamentoDiario.CarregarGrid();
        }, type: types.event, text: "Pesquisar", idGrid: guid(), visible: ko.observable(true)
    });

    this.ExibirFiltros = PropertyEntity({
        eventClick: function (e) {
            if (e.ExibirFiltros.visibleFade()) {
                e.ExibirFiltros.visibleFade(false);
            } else {
                e.ExibirFiltros.visibleFade(true);
            }
        }, type: types.event, text: "Filtros de Pesquisa", idFade: guid(), visibleFade: ko.observable(false), visible: ko.observable(true)
    });
};

var FechamentoDiario = function () {
    this.DataFechamento = PropertyEntity({ text: "*Data de Fechamento:", getType: typesKnockout.date, required: true });
    this.Empresa = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Empresa:", idBtnSearch: guid(), visible: ko.observable(true) });
    this.BloquearApenasDocumentoEntrada = PropertyEntity({ text: "Realizar fechamento apenas para Documento de Entrada", val: ko.observable(false), getType: typesKnockout.bool, def: false });

    this.Gerar = PropertyEntity({ eventClick: GerarFechamentoClick, type: types.event, text: "Gerar Fechamento", visible: ko.observable(true) });
    this.Cancelar = PropertyEntity({ eventClick: CancelarClick, type: types.event, text: "Cancelar", visible: ko.observable(true) });
};

var DocumentosPendentes = function () {
    this.Descricao = PropertyEntity({ text: "", val: ko.observable(""), def: "" });
    this.Fechar = PropertyEntity({ eventClick: FecharTelaDocumentosPendentesClick, type: types.event, text: "Fechar", icon: "fal fa-ban", idGrid: guid() });
};

//*******EVENTOS*******

function LoadFechamentoDiario() {

    _fechamentoDiario = new FechamentoDiario();
    KoBindings(_fechamentoDiario, "knockoutCadastroFechamentoDiario");

    HeaderAuditoria("FechamentoDiario", _fechamentoDiario);

    _pesquisaFechamentoDiario = new PesquisaFechamentoDiario();
    KoBindings(_pesquisaFechamentoDiario, "knockoutPesquisaFechamentoDiario", false, _pesquisaFechamentoDiario.Pesquisar.id);

    _documentosPendentes = new DocumentosPendentes();
    KoBindings(_documentosPendentes, "knockoutDocumentosPendentes");

    new BuscarEmpresa(_fechamentoDiario.Empresa, null, true);
    new BuscarEmpresa(_pesquisaFechamentoDiario.Empresa, null, true);

    BuscarFechamentosDiarios();
}

function GerarFechamentoClick(e, sender) {
    Salvar(e, "FechamentoDiario/Adicionar", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Fechamento diário gerado com sucesso!");
                _gridFechamentoDiario.CarregarGrid();
                LimparCamposFechamentoDiario();
            } else {
                AbrirTelaDocumentosPendentes(arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    }, sender);
}

function CancelarClick(e) {
    LimparCamposFechamentoDiario();
}

function FecharTelaDocumentosPendentesClick(e) {
    LimparCampos(_documentosPendentes);
    Global.fecharModal("divModalDocumentosPendentes");
}

//*******MÉTODOS*******

function BuscarFechamentosDiarios() {
    var editar = { descricao: "Remover", id: guid(), evento: "onclick", metodo: RemoverFechamentoDiarioClick, tamanho: "20", icone: "" };
    var menuOpcoes = new Object();
    menuOpcoes.tipo = TypeOptionMenu.link;
    menuOpcoes.opcoes = new Array();
    menuOpcoes.opcoes.push(editar);

    _gridFechamentoDiario = new GridView(_pesquisaFechamentoDiario.Pesquisar.idGrid, "FechamentoDiario/Pesquisa", _pesquisaFechamentoDiario, menuOpcoes, { column: 1, dir: orderDir.desc });
    _gridFechamentoDiario.CarregarGrid();
}

function LimparCamposFechamentoDiario() {
    LimparCampos(_fechamentoDiario);
}

function RemoverFechamentoDiarioClick(fechamento) {
    exibirConfirmacao("Atenção!", "Deseja realmente remover o fechamento com data de " + fechamento.DataFechamento + "?", function () {
        executarReST("FechamentoDiario/Excluir", { Codigo: fechamento.Codigo }, function (arg) {
            if (arg.Success) {
                if (arg.Data) {
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Fechamento diário removido com sucesso!");
                    _gridFechamentoDiario.CarregarGrid();
                } else {
                    exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
            }
        });
    });
}

function AbrirTelaDocumentosPendentes(retornoMsg) {
    LimparCampos(_documentosPendentes);
    _documentosPendentes.Descricao.val(retornoMsg);
    Global.abrirModal("divModalDocumentosPendentes");
}