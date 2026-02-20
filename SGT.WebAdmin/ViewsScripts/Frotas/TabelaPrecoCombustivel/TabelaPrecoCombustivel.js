
/// <reference path="../../Consultas/Empresa.js" />

/*
 * Declaração de Objetos Globais do Arquivo
 */

var _CRUDTabelaPrecoCombustivel;
var _tabelaPrecoCombustivel;
var _pesquisaTabelaPrecoCombustivel;
var _gridTabelaPrecoCombustivel;

/*
 * Declaração das Classes
 */

var CRUDTabelaPrecoCombustivel = function () {
    this.Adicionar = PropertyEntity({ eventClick: AdicionarClick, type: types.event, text: "Adicionar", visible: ko.observable(true) });
    this.Atualizar = PropertyEntity({ eventClick: AtualizarClick, type: types.event, text: "Atualizar", visible: ko.observable(false) });
    this.Cancelar = PropertyEntity({ eventClick: CancelarClick, type: types.event, text: "Cancelar", visible: ko.observable(false) });
    this.Excluir = PropertyEntity({ eventClick: ExcluirClick, type: types.event, text: "Excluir", visible: ko.observable(false) });
};

var TabelaPrecoCombustivel = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Descricao = PropertyEntity({ text: "*Descrição:", required: true, getType: typesKnockout.string, val: ko.observable("") });
    this.ValorExterno = PropertyEntity({ text: "*Valor Externo:", required: true, getType: typesKnockout.decimal, configDecimal: { precision: 2, allowZero: false }, val: ko.observable(0.00), maxlength: 10 });
    this.ValorInterno = PropertyEntity({ text: "*Valor Interno:", required: true, getType: typesKnockout.decimal, configDecimal: { precision: 2, allowZero: false }, val: ko.observable(0.00), maxlength: 10 });
    this.DataInicioVigencia = PropertyEntity({ text: "*Data início vigência:", required: true, getType: typesKnockout.date });
    this.TipoOleo = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: ko.observable("*Tipo de óleo:"), idBtnSearch: guid(), required: true });
    this.Empresa = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "*Empresa:", idBtnSearch: guid(), required: ko.observable(true), enable: ko.observable(true), visible: ko.observable(true) });
};

var PesquisaTabelaPrecoCombustivel = function () {
    this.Descricao = PropertyEntity({ text: "Descrição:", required: true, getType: typesKnockout.string, val: ko.observable("") });
    this.DataInicioVigencia = PropertyEntity({ text: "Data início vigência:", required: false, getType: typesKnockout.date });
    this.TipoOleo = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: ko.observable("Tipo de óleo:"), idBtnSearch: guid(), required: false });
    this.Empresa = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Empresa:", idBtnSearch: guid(), enable: ko.observable(true), visible: ko.observable(true) });

    this.ExibirFiltros = PropertyEntity({ eventClick: ExibirFiltrosClick, type: types.event, text: "Filtros de Pesquisa", idFade: guid(), visibleFade: ko.observable(false), visible: ko.observable(true) });
    this.Pesquisar = PropertyEntity({ eventClick: RecarregarGridTabelaPrecoCombustivel, type: types.event, text: "Pesquisar", idGrid: guid(), visible: ko.observable(true) });
};

/*
 * Declaração das Funções de Inicialização
 */

function LoadGridTabelaPrecoCombustivel() {
    var opcaoEditar = { descricao: "Editar", id: guid(), evento: "onclick", metodo: EditarClick, tamanho: "10", icone: "" };
    var menuOpcoes = { tipo: TypeOptionMenu.link, opcoes: [opcaoEditar] };

    _gridTabelaPrecoCombustivel = new GridViewExportacao(_pesquisaTabelaPrecoCombustivel.Pesquisar.idGrid, "TabelaPrecoCombustivel/Pesquisa", _pesquisaTabelaPrecoCombustivel, menuOpcoes);
    _gridTabelaPrecoCombustivel.CarregarGrid();
}

function LoadTabelaPrecoCombustivel() {
    _tabelaPrecoCombustivel = new TabelaPrecoCombustivel();
    KoBindings(_tabelaPrecoCombustivel, "knockoutTabelaPrecoCombustivel");

    _CRUDTabelaPrecoCombustivel = new CRUDTabelaPrecoCombustivel();
    KoBindings(_CRUDTabelaPrecoCombustivel, "knockoutCRUDTabelaPrecoCombustivel");

    _pesquisaTabelaPrecoCombustivel = new PesquisaTabelaPrecoCombustivel();
    KoBindings(_pesquisaTabelaPrecoCombustivel, "knockoutPesquisaTabelaPrecoCombustivel", false, _pesquisaTabelaPrecoCombustivel.Pesquisar.id);

    BuscarEmpresa(_tabelaPrecoCombustivel.Empresa);
    BuscarEmpresa(_pesquisaTabelaPrecoCombustivel.Empresa);

    BuscarTipoOleo(_tabelaPrecoCombustivel.TipoOleo);
    BuscarTipoOleo(_pesquisaTabelaPrecoCombustivel.TipoOleo);

    LoadGridTabelaPrecoCombustivel();
}

/*
 * Declaração das Funções Associadas a Eventos
 */

function AdicionarClick(e, sender) {
    Salvar(_tabelaPrecoCombustivel, "TabelaPrecoCombustivel/Adicionar", function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Cadastrado com sucesso.");

                RecarregarGridTabelaPrecoCombustivel();
                LimparCamposTabelaPrecoCombustivel();
            }
            else
                exibirMensagem(tipoMensagem.aviso, "Aviso", retorno.Msg);
        }
        else
            exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
    }, sender);
}

function AtualizarClick(e, sender) {
    Salvar(_tabelaPrecoCombustivel, "TabelaPrecoCombustivel/Atualizar", function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Atualizado com sucesso");

                RecarregarGridTabelaPrecoCombustivel();
                LimparCamposTabelaPrecoCombustivel();
            }
            else
                exibirMensagem(tipoMensagem.aviso, "Aviso", retorno.Msg);
        }
        else
            exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
    }, sender);
}

function CancelarClick() {
    LimparCamposTabelaPrecoCombustivel();
}

function EditarClick(registroSelecionado) {
    LimparCamposTabelaPrecoCombustivel();

    _tabelaPrecoCombustivel.Codigo.val(registroSelecionado.Codigo);

    BuscarPorCodigo(_tabelaPrecoCombustivel, "TabelaPrecoCombustivel/BuscarPorCodigo", function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                _pesquisaTabelaPrecoCombustivel.ExibirFiltros.visibleFade(false);

                var isEdicao = true;

                ControlarBotoesHabilitados(isEdicao);
            }
            else
                exibirMensagem(tipoMensagem.aviso, "Aviso", retorno.Msg);
        }
        else
            exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
    }, null);
}

function ExcluirClick() {
    exibirConfirmacao("Confirmação", "Realmente deseja excluir esse cadastro?", function () {
        ExcluirPorCodigo(_tabelaPrecoCombustivel, "TabelaPrecoCombustivel/ExcluirPorCodigo", function (retorno) {
            if (retorno.Success) {
                if (retorno.Data) {
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Excluído com sucesso!");

                    RecarregarGridTabelaPrecoCombustivel();
                    LimparCamposTabelaPrecoCombustivel();
                }
                else
                    exibirMensagem(tipoMensagem.aviso, "Sugestão", retorno.Msg, 16000);
            }
            else
                exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
        }, null);
    });
}

function ExibirFiltrosClick(e) {
    e.ExibirFiltros.visibleFade(!e.ExibirFiltros.visibleFade());
}

/*
 * Declaração das Funções
 */

function ControlarBotoesHabilitados(isEdicao) {
    _CRUDTabelaPrecoCombustivel.Atualizar.visible(isEdicao);
    _CRUDTabelaPrecoCombustivel.Excluir.visible(isEdicao);
    _CRUDTabelaPrecoCombustivel.Cancelar.visible(isEdicao);
    _CRUDTabelaPrecoCombustivel.Adicionar.visible(!isEdicao);
}

function LimparCamposTabelaPrecoCombustivel() {
    var isEdicao = false;

    ControlarBotoesHabilitados(isEdicao);
    LimparCampos(_tabelaPrecoCombustivel);
}

function RecarregarGridTabelaPrecoCombustivel() {
    _gridTabelaPrecoCombustivel.CarregarGrid();
}