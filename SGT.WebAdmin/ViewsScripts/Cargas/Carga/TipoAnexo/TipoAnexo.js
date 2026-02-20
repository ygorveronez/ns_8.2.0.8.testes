//#region Declaração variaveis 
var _gridTipoAnexo;
var _tipoAnexo;
var _pesquisaTipoAnexo;
var _crudTipoAnexo;
//#endregion

//#region Funções Constructoras
var PesquisaTipoAnexo = function () {
    this.Descricao = PropertyEntity({ text: "Descrição: ", val: ko.observable("") });
    this.Status = PropertyEntity({ val: ko.observable(true), options: _status, def: true, text: "Status: " });

    this.Pesquisar = PropertyEntity({
        eventClick: function (e) {
            _gridTipoAnexo.CarregarGrid();
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
function TipoAnexo() {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });

    this.Descricao = PropertyEntity({ text: "Descrição: ", val: ko.observable(""), enable: ko.observable(true) });
    this.Status = PropertyEntity({ val: ko.observable(true), options: _status, def: true, enable: ko.observable(true), text: "Status: " });
}
function CrudTipoAnexo() {
    this.Adicionar = PropertyEntity({ eventClick: adicionarClick, type: types.event, text: "Adicionar", visible: ko.observable(true) });
    this.Atualizar = PropertyEntity({ eventClick: atualizarClick, type: types.event, text: "Atualizar", visible: ko.observable(false) });
    this.Excluir = PropertyEntity({ eventClick: excluirClick, type: types.event, text: "Excluir", visible: ko.observable(false) });
    this.Cancelar = PropertyEntity({ eventClick: cancelarClick, type: types.event, text: "Cancelar", visible: ko.observable(false) });
}
//#endregion

//#region Funções de Carregamento
function loadTipoAnexo() {
    _tipoAnexo = new TipoAnexo();
    KoBindings(_tipoAnexo, "knockoutTipoAnexo");

    _crudTipoAnexo = new CrudTipoAnexo();
    KoBindings(_crudTipoAnexo, "knockoutCRUDTipoAnexo");

    _pesquisaTipoAnexo = new PesquisaTipoAnexo();
    KoBindings(_pesquisaTipoAnexo, "knockoutPesquisaTipoAnexo");

    BuscarTipoAnexoGrid();
}
//#endregion

//#region Funções Crud
function adicionarClick(e, sender) {
    Salvar(_tipoAnexo, "TipoAnexo/Adicionar", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Cadastrado com sucesso");
                _gridTipoAnexo.CarregarGrid();
                LimparCamposEditarTipoAnexo();

            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    }, sender);
}
function EditarTipoAnexo(item) {
    LimparCamposEditarTipoAnexo();
    _tipoAnexo.Codigo.val(item.Codigo);
    BuscarPorCodigo(_tipoAnexo, "TipoAnexo/BuscarPorCodigo", function (arg) {
        controlarBotoesGrid(true);
    }, null);
}
function atualizarClick(e, sender) {
    Salvar(_tipoAnexo, "TipoAnexo/Atualizar", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Atualizado com sucesso");
                _gridTipoAnexo.CarregarGrid();
                LimparCamposEditarTipoAnexo();
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
        ExcluirPorCodigo(_tipoAnexo, "TipoAnexo/ExcluirPorCodigo", function (arg) {
            if (arg.Success) {
                if (arg.Data) {
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Excluído com sucesso");
                    _gridTipoAnexo.CarregarGrid();
                    LimparCamposEditarTipoAnexo();
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
    LimparCamposEditarTipoAnexo();
    controlarBotoesGrid(false);
}
//#endregion


//#region Funções Auxiliares
function controlarBotoesGrid(valor) {
    _crudTipoAnexo.Atualizar.visible(valor);
    _crudTipoAnexo.Cancelar.visible(valor);
    _crudTipoAnexo.Excluir.visible(valor);
    _crudTipoAnexo.Adicionar.visible(!valor);
}
function BuscarTipoAnexoGrid() {
    const editar = { descricao: "Editar", id: "clasEditar", evento: "onclick", metodo: EditarTipoAnexo, tamanho: "15", icone: "" };
    const menuOpcoes = new Object();
    menuOpcoes.tipo = TypeOptionMenu.link;
    menuOpcoes.opcoes = new Array();
    menuOpcoes.opcoes.push(editar);

    _gridTipoAnexo = new GridView(_pesquisaTipoAnexo.Pesquisar.idGrid, "TipoAnexo/Pesquisa", _pesquisaTipoAnexo, menuOpcoes, null);
    _gridTipoAnexo.CarregarGrid();
}
function LimparCamposEditarTipoAnexo() {
    LimparCampos(_tipoAnexo);
}
//#endregion