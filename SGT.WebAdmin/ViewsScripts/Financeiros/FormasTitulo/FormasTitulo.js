/// <reference path="../../Consultas/CFOP.js" />
/// <reference path="../../Enumeradores/EnumFormaTitulo.js" />


//*******MAPEAMENTO KNOUCKOUT*******

var _gridFormasTitulo;
var _formasTitulo;
var _pesquisaFormasTitulo;


var PesquisaFormasTitulo = function () {
    this.Descricao = PropertyEntity({ text: "Descrição: " });
    this.CodigoIntegracao = PropertyEntity({ text: "Código de Integracao: ", getType: typesKnockout.string, maxlength: 50 });

    this.Pesquisar = PropertyEntity({
        eventClick: function (e) {
            _gridFormasTitulo.CarregarGrid();
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

var FormasTitulo = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });

    this.Descricao = PropertyEntity({ text: "Descrição: ", required: true, maxlength: 200 });
    this.CodigoIntegracao = PropertyEntity({ text: "*Código de Integracao: ", getType: typesKnockout.string, maxlength: 50, required: true });

    this.Atualizar = PropertyEntity({ eventClick: atualizarClick, type: types.event, text: "Atualizar", visible: ko.observable(false) });
    this.Cancelar = PropertyEntity({ eventClick: cancelarClick, type: types.event, text: "Cancelar", visible: ko.observable(false) });
}

//*******EVENTOS*******

function loadFormasTitulo() {
    _pesquisaFormasTitulo = new PesquisaFormasTitulo();
    KoBindings(_pesquisaFormasTitulo, "knockoutPesquisaFormasTitulo", false, _pesquisaFormasTitulo.Pesquisar.id);

    _formasTitulo = new FormasTitulo();
    KoBindings(_formasTitulo, "knockoutCadastroFormasTitulo");

    HeaderAuditoria("Formas de Título", _formasTitulo);

    buscarFormasTitulo();
}

function atualizarClick(e, sender) {
    Salvar(e, "FormasTitulo/Atualizar", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Atualizado com sucesso");
                _gridFormasTitulo.CarregarGrid();
                limparCamposFormasTitulo();
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "falha", arg.Msg);
        }
    }, sender);
}

function cancelarClick(e) {
    limparCamposFormasTitulo();
}

//*******MÉTODOS*******


function buscarFormasTitulo() {
    var editar = { descricao: "Editar", id: "clasEditar", evento: "onclick", metodo: editarFormasTitulo, tamanho: "15", icone: "" };
    var menuOpcoes = new Object();
    menuOpcoes.tipo = TypeOptionMenu.link;
    menuOpcoes.opcoes = new Array();
    menuOpcoes.opcoes.push(editar);

    _gridFormasTitulo = new GridView(_pesquisaFormasTitulo.Pesquisar.idGrid, "FormasTitulo/Pesquisa", _pesquisaFormasTitulo, menuOpcoes, null);
    _gridFormasTitulo.CarregarGrid();
}

function editarFormasTitulo(formasTituloGrid) {
    limparCamposFormasTitulo();
    _formasTitulo.Codigo.val(formasTituloGrid.Codigo);
    BuscarPorCodigo(_formasTitulo, "FormasTitulo/BuscarPorCodigo", function (arg) {
        _pesquisaFormasTitulo.ExibirFiltros.visibleFade(false);
        _formasTitulo.Atualizar.visible(true);
        _formasTitulo.Cancelar.visible(true);
    }, null);
}

function limparCamposFormasTitulo() {
    _formasTitulo.Atualizar.visible(false);
    _formasTitulo.Cancelar.visible(false);

    LimparCampos(_formasTitulo);
}