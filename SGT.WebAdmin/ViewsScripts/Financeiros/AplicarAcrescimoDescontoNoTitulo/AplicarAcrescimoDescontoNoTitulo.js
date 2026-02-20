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
/// <reference path="../../Consultas/Titulo.js" />
/// <reference path="../../Consultas/Justificativa.js" />




var _gridAplicarAcrescimoDescontoNoTitulo;
var _aplicarAcrescimoDescontoNoTitulo
var _pesquisaAplicarAcrescimoDescontoNoTitulo;
var _crudAplicarAcrescimoDescontoNoTitulo;

var PesquisaAplicarAcrescimoDescontoNoTitulo = function () {
    this.Descricao = PropertyEntity({ text: "Descrição: ", val: ko.observable("") });
    this.Status = PropertyEntity({ val: ko.observable(true), options: _statusPesquisa, def: true, text: "Status: " });
    this.Titulo = PropertyEntity({ text: "Titulo: ", type: types.entity, codEntity: ko.observable(0), idBtnSearch: guid() });
    this.Justificativa = PropertyEntity({ text: "Justificativa: ", type: types.entity, codEntity: ko.observable(0), idBtnSearch: guid() });

    this.Pesquisar = PropertyEntity({
        eventClick: function (e) {
            _gridAplicarAcrescimoDescontoNoTitulo.CarregarGrid();
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
}

//#region Constructores

function AplicarAcrescimoDescontoNoTitulo() {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });

    this.Observacao = PropertyEntity({ text: "Observação: ", val: ko.observable(""), enable: ko.observable(true) });
    this.RemoverProvisaoTitulo = PropertyEntity({ text: "Remover Provisão Titulo", val: ko.observable(false), def: false, enable: ko.observable(true) });
    this.DataAutorizacao = PropertyEntity({ text: "Data Autorização: ", getType: typesKnockout.date, required: false, enable: ko.observable(true), val: ko.observable("") });
    this.Valor = PropertyEntity({ text: "Valor: ", val: ko.observable(0), def: 0, getType: typesKnockout.decimal, enable: ko.observable(true), required: true });
    this.Titulo = PropertyEntity({ text: "Titulo: ", type: types.entity, codEntity: ko.observable(0), idBtnSearch: guid() });
    this.Justificativa = PropertyEntity({ text: "Justificativa: ", type: types.entity, codEntity: ko.observable(0), idBtnSearch: guid(), required: true });
}

function CrudAplicarAcrescimoDescontoNoTitulo() {
    this.Adicionar = PropertyEntity({ eventClick: adicionarClick, type: types.event, text: "Adicionar", visible: ko.observable(true) });
    this.Excluir = PropertyEntity({ eventClick: excluirClick, type: types.event, text: "Excluir", visible: ko.observable(false) });
    this.Cancelar = PropertyEntity({ eventClick: cancelarClick, type: types.event, text: "Cancelar", visible: ko.observable(false) });
}

//#endregion


//#region Funções Principais

function loadAplicarAcrescimoDescontoNoTitulo() {
    _aplicarAcrescimoDescontoNoTitulo = new AplicarAcrescimoDescontoNoTitulo();
    KoBindings(_aplicarAcrescimoDescontoNoTitulo, "knockoutAplicarAcrescimoDescontoNoTitulo");

    _crudAplicarAcrescimoDescontoNoTitulo = new CrudAplicarAcrescimoDescontoNoTitulo();
    KoBindings(_crudAplicarAcrescimoDescontoNoTitulo, "knockoutCRUDAplicarAcrescimoDescontoNoTitulo");

    _pesquisaAplicarAcrescimoDescontoNoTitulo = new PesquisaAplicarAcrescimoDescontoNoTitulo();
    KoBindings(_pesquisaAplicarAcrescimoDescontoNoTitulo, "knockoutPesquisaAplicarAcrescimoDescontoNoTitulo");


    BuscarJustificativas(_aplicarAcrescimoDescontoNoTitulo.Justificativa, (arg) => retornoBusca(_aplicarAcrescimoDescontoNoTitulo.Justificativa, arg));
    BuscarTitulo(_aplicarAcrescimoDescontoNoTitulo.Titulo, null, null, (arg) => retornoBusca(_aplicarAcrescimoDescontoNoTitulo.Titulo, arg), true, true);

    BuscarJustificativas(_pesquisaAplicarAcrescimoDescontoNoTitulo.Justificativa, (arg) => retornoBusca(_pesquisaAplicarAcrescimoDescontoNoTitulo.Justificativa, arg));
    BuscarTitulo(_pesquisaAplicarAcrescimoDescontoNoTitulo.Titulo, null, null, (arg) => retornoBusca(_pesquisaAplicarAcrescimoDescontoNoTitulo.Titulo, arg), true, true);

    buscarAplicacoesAcrescimosDescontos();
}


function LimparCamposAplicacoesAcrescimosDescontos() {
    LimparCampos(_aplicarAcrescimoDescontoNoTitulo);
}

function retornoBusca(knokout, arg) {
    knokout.val(arg.Descricao);
    knokout.codEntity(arg.Codigo);
}

//#endregion

//#region Funções Crud
function adicionarClick(e, sender) {
    Salvar(_aplicarAcrescimoDescontoNoTitulo, "AplicarAcrescimoDescontoNoTitulo/Adicionar", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Cadastrado com sucesso");
                _gridAplicarAcrescimoDescontoNoTitulo.CarregarGrid();
                LimparCamposAplicacoesAcrescimosDescontos();

            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    }, sender);
}

function atualizarClick(e, sender) {
    Salvar(_aplicarAcrescimoDescontoNoTitulo, "AplicarAcrescimoDescontoNoTitulo/Atualizar", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Atualizado com sucesso");
                _gridAplicarAcrescimoDescontoNoTitulo.CarregarGrid();
                LimparCamposAplicacoesAcrescimosDescontos();
                controlarBotoesGrid(false);
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "falha", arg.Msg);
        }
    }, sender);
}

function excluirClick(e, sender) {
    exibirConfirmacao("Confirmação", "Realmente deseja excluir o registro?", function () {
        ExcluirPorCodigo(_aplicarAcrescimoDescontoNoTitulo, "AplicarAcrescimoDescontoNoTitulo/ExcluirPorCodigo", function (arg) {
            if (arg.Success) {
                if (arg.Data) {
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Excluído com sucesso");
                    _gridAplicarAcrescimoDescontoNoTitulo.CarregarGrid();
                    LimparCamposAplicacoesAcrescimosDescontos();
                    controlarBotoesGrid(false);
                } else {
                    exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, "falha", arg.Msg);
            }
        }, null);
    });
}

function cancelarClick(e) {
    LimparCamposAplicacoesAcrescimosDescontos();
    controlarBotoesGrid(false);
}

function controlarBotoesGrid(valor) {
    _crudAplicarAcrescimoDescontoNoTitulo.Cancelar.visible(valor);
    _crudAplicarAcrescimoDescontoNoTitulo.Excluir.visible(valor);
    _crudAplicarAcrescimoDescontoNoTitulo.Adicionar.visible(!valor);
}

function buscarAplicacoesAcrescimosDescontos() {
    const editar = { descricao: "Editar", id: "clasEditar", evento: "onclick", metodo: editarAplicacoesAcrescimosDescontos, tamanho: "15", icone: "" };
    const menuOpcoes = new Object();
    menuOpcoes.tipo = TypeOptionMenu.link;
    menuOpcoes.opcoes = new Array();
    menuOpcoes.opcoes.push(editar);

    _gridAplicarAcrescimoDescontoNoTitulo = new GridView(_pesquisaAplicarAcrescimoDescontoNoTitulo.Pesquisar.idGrid, "AplicarAcrescimoDescontoNoTitulo/Pesquisa", _pesquisaAplicarAcrescimoDescontoNoTitulo, menuOpcoes, null);
    _gridAplicarAcrescimoDescontoNoTitulo.CarregarGrid();
}

function editarAplicacoesAcrescimosDescontos(item) {
    LimparCamposAplicacoesAcrescimosDescontos();
    _aplicarAcrescimoDescontoNoTitulo.Codigo.val(item.Codigo);

    BuscarPorCodigo(_aplicarAcrescimoDescontoNoTitulo, "AplicarAcrescimoDescontoNoTitulo/BuscarPorCodigo", function (arg) {
        
        if (!arg.Success)
            exibirMensagem(tipoMensagem.falha, "Error", arg.msg);
        _pesquisaAplicarAcrescimoDescontoNoTitulo.ExibirFiltros.visibleFade(false);
        controlarBotoesGrid(true);

    }, null);
}
//#endregion