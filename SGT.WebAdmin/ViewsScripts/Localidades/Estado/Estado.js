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
/// <reference path="../../../ViewsScripts/Consultas/RegiaoBrasil.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _gridEstado;
var _estado;
var _pesquisaEstado;

var PesquisaEstado = function () {
    this.Codigo = PropertyEntity({ text: "Sigla: " });
    this.Nome = PropertyEntity({ text: "Nome: " });

    this.Pesquisar = PropertyEntity({
        eventClick: function (e) {
            _gridEstado.CarregarGrid();
        }, type: types.event, text: "Pesquisar", idGrid: guid(), visible: ko.observable(true)
    });

    this.ExibirFiltros = PropertyEntity({ eventClick: exibirFiltrosClick, type: types.event, text: "Filtros de Pesquisa", idFade: guid(), visibleFade: ko.observable(false), visible: ko.observable(true) });

};

var Estado = function () {
    this.Codigo = PropertyEntity({ text: "Sigla: ", enable: false, maxlength: 2 });
    this.Nome = PropertyEntity({ text: "Nome: ", enable: false, maxlength: 80 });
    this.CodigoEstado = PropertyEntity({ text: "Código Estado:", val: ko.observable(""), maxlength: 150 });
    this.DataAtualizacao = PropertyEntity({ text: "Data Atualização:", enable: false, getType: typesKnockout.date, val: ko.observable(""), enable: false });
    this.CodigoIBGE = PropertyEntity({ text: "Código IBGE: ", enable: false, enable: false });
    this.Status = PropertyEntity({ text: "Status: ", val: ko.observable(true), getType: typesKnockout.select, options: _status, def: true });
    this.Abreviacao = PropertyEntity({ text: "Abreviação: ", enable: false, maxlength: 3 });
    this.Pais = PropertyEntity({ text: "País: ", enable: false, type: types.entity, codEntity: ko.observable(0), idBtnSearch: guid() });
    this.TipoEmissao = PropertyEntity({ text: "Tipo Emissão: ", val: ko.observable(EnumTipoEmissaoEstado.Normal), getType: typesKnockout.select, options: EnumTipoEmissaoEstado.obterOpcoes(), def: EnumTipoEmissaoEstado.Normal });
    this.SefazCte = PropertyEntity({ text: "Sefaz CTe: ", type: types.entity, codEntity: ko.observable(0), idBtnSearch: guid() });
    this.SefazCteHomologacao = PropertyEntity({ text: "Sefaz CTe Homologação: ", type: types.entity, codEntity: ko.observable(0), idBtnSearch: guid() });
    this.SefazMdfe = PropertyEntity({ text: "Sefaz MDFe: ", type: types.entity, codEntity: ko.observable(0), idBtnSearch: guid() });
    this.SefazMdfeHomologacao = PropertyEntity({ text: "Sefaz MDFe Homologação: ", type: types.entity, codEntity: ko.observable(0), idBtnSearch: guid() });
    this.RegiaoBrasil = PropertyEntity({ text: "Região: ", enable: false, type: types.entity, codEntity: ko.observable(0), idBtnSearch: guid() });
    
    this.Atualizar = PropertyEntity({ eventClick: atualizarClick, type: types.event, text: "Atualizar", visible: ko.observable(false) });
    this.Cancelar = PropertyEntity({ eventClick: cancelarClick, type: types.event, text: "Cancelar", visible: ko.observable(false) });
};

//*******EVENTOS*******


function loadEstado() {
    _estado = new Estado();
    KoBindings(_estado, "knockoutCadastroEstado");

    HeaderAuditoria("Estado", _estado);

    _pesquisaEstado = new PesquisaEstado();
    KoBindings(_pesquisaEstado, "knockoutPesquisaEstado", false, _pesquisaEstado.Pesquisar.id);

    buscarEstados();

    BuscarRegiaoBrasil(_estado.RegiaoBrasil);

    BuscarSefaz(_estado.SefazCte);
    BuscarSefaz(_estado.SefazCteHomologacao);
    BuscarSefaz(_estado.SefazMdfe);
    BuscarSefaz(_estado.SefazMdfeHomologacao);
    
}


function atualizarClick(e, sender) {
    Salvar(e, "Estado/Atualizar", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Atualizado com sucesso");
                _gridEstado.CarregarGrid();
                limparCamposEstado();
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    }, sender);
}

function cancelarClick(e) {
    limparCamposEstado();
}

function exibirFiltrosClick(e) {
    e.ExibirFiltros.visibleFade(!e.ExibirFiltros.visibleFade());
}

//*******MÉTODOS*******

function buscarEstados() {
    var editar = { descricao: "Editar", id: "clasEditar", evento: "onclick", metodo: editarEstado, tamanho: "15", icone: "" };
    var menuOpcoes = new Object();
    menuOpcoes.tipo = TypeOptionMenu.link;
    menuOpcoes.opcoes = new Array();
    menuOpcoes.opcoes.push(editar);

    _gridEstado = new GridViewExportacao(_pesquisaEstado.Pesquisar.idGrid, "Estado/PesquisaCadastro", _pesquisaEstado, menuOpcoes);
    _gridEstado.CarregarGrid();
}

function editarEstado(estadoGrid) {
    limparCamposEstado();
    _estado.Codigo.val(estadoGrid.Codigo);
    BuscarPorCodigo(_estado, "Estado/BuscarPorSigla", function (arg) {
        _pesquisaEstado.ExibirFiltros.visibleFade(false);
        _estado.Atualizar.visible(true);
        _estado.Cancelar.visible(true);
    }, null);
}

function limparCamposEstado() {
    _estado.Atualizar.visible(false);
    _estado.Cancelar.visible(false);
    LimparCampos(_estado);
}
