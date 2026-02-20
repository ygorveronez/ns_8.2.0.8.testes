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

//*******MAPEAMENTO KNOUCKOUT*******

var _gridPais;
var _pais;
var _pesquisaPais;

var PesquisaPais = function () {
    this.Descricao = PropertyEntity({ text: "Descrição: " });

    this.Pesquisar = PropertyEntity({
        eventClick: function (e) {
            _gridPais.CarregarGrid();
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

var Pais = function () {
    this.Codigo = PropertyEntity({ text: "Código: ", val: ko.observable(0), def: 0, getType: typesKnockout.int, enable: false });
    this.Nome = PropertyEntity({ text: "Nome: ", enable: false });
    this.Sigla = PropertyEntity({ text: "Sigla: ", enable: false });
    this.Abreviacao = PropertyEntity({ text: "Abreviação: ", enable: false });
    this.CodigoTelefonico = PropertyEntity({ text: "Código Telefone: ", enable: false });
    this.LicencaTNTI = PropertyEntity({ text: "Licença TNTI:", maxlength: 150, required: ko.observable(false) });
    this.VencimentoLicencaTNTI = PropertyEntity({ text: "Vencimento TNTI:", getType: typesKnockout.date, required: ko.observable(false) });
    this.CodigoPais = PropertyEntity({ text: "Código Pais:", val: ko.observable(""), maxlength: 150 });

    this.Atualizar = PropertyEntity({ eventClick: atualizarClick, type: types.event, text: "Atualizar", visible: ko.observable(false) });
    this.Cancelar = PropertyEntity({ eventClick: cancelarClick, type: types.event, text: "Cancelar", visible: ko.observable(false) });
};

//*******EVENTOS*******


function loadPais() {
    _pais = new Pais();
    KoBindings(_pais, "knockoutCadastroPais");

    HeaderAuditoria("Pais", _pais);

    _pesquisaPais = new PesquisaPais();
    KoBindings(_pesquisaPais, "knockoutPesquisaPais", false, _pesquisaPais.Pesquisar.id);

    buscarPaises();
}


function atualizarClick(e, sender) {
    Salvar(e, "Pais/Atualizar", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Atualizado com sucesso");
                _gridPais.CarregarGrid();
                limparCamposPais();
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    }, sender);
}

function cancelarClick(e) {
    limparCamposPais();
}

//*******MÉTODOS*******

function buscarPaises() {
    var editar = { descricao: "Editar", id: "clasEditar", evento: "onclick", metodo: editarPais, tamanho: "15", icone: "" };
    var menuOpcoes = new Object();
    menuOpcoes.tipo = TypeOptionMenu.link;
    menuOpcoes.opcoes = new Array();
    menuOpcoes.opcoes.push(editar);

    _gridPais = new GridViewExportacao(_pesquisaPais.Pesquisar.idGrid, "Pais/Pesquisa", _pesquisaPais, menuOpcoes);
    _gridPais.CarregarGrid();
}

function editarPais(paisGrid) {
    limparCamposPais();
    _pais.Codigo.val(paisGrid.Codigo);
    BuscarPorCodigo(_pais, "Pais/BuscarPorCodigo", function (arg) {
        _pesquisaPais.ExibirFiltros.visibleFade(false);
        _pais.Atualizar.visible(true);
        _pais.Cancelar.visible(true);
    }, null);
}

function limparCamposPais() {
    _pais.Atualizar.visible(false);
    _pais.Cancelar.visible(false);
    LimparCampos(_pais);
}
