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
/// <reference path="../../Consultas/TipoContainer.js" />
/// <reference path="../../Enumeradores/EnumTipoPropriedadeContainer.js" />
/// <reference path="../../Enumeradores/EnumTipoCarregamentoNavio.js" />
/// <reference path="../../Enumeradores/EnumPermissaoPersonalizada.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _gridContainer;
var _container;
var _pesquisaContainer;

var PesquisaContainer = function () {
    this.Descricao = PropertyEntity({ text: "Descrição: " });
    this.CodigoIntegracao = PropertyEntity({ text: "Código Integração: " });
    this.Numero = PropertyEntity({ text: "Número: " });
    this.Status = PropertyEntity({ text: "Status: ", val: ko.observable(1), options: _statusPesquisa, def: 1 });

    this.Pesquisar = PropertyEntity({
        eventClick: function (e) {
            _gridContainer.CarregarGrid();
        }, type: types.event, text: "Pesquisar", idGrid: guid(), visible: ko.observable(true)
    });
    this.ExibirFiltros = PropertyEntity({
        eventClick: function (e) {
            if (e.ExibirFiltros.visibleFade() === true) {
                e.ExibirFiltros.visibleFade(false);
            } else {
                e.ExibirFiltros.visibleFade(true);
            }
        }, type: types.event, text: "Filtros de Pesquisa", idFade: guid(), visibleFade: ko.observable(false), visible: ko.observable(true)
    });
};

var Container = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Descricao = PropertyEntity({ text: "*Descrição: ", required: ko.observable(true), maxlength: 500, enable: ko.observable(true) });
    this.CodigoIntegracao = PropertyEntity({ text: "Código Integração: ", required: ko.observable(false), maxlength: 50, enable: ko.observable(true) });
    this.Status = PropertyEntity({ text: "Status: ", val: ko.observable(true), options: _status, def: true, required: ko.observable(true), enable: ko.observable(true) });
    this.Numero = PropertyEntity({ text: "*Número: ", maxlength: 20, required: ko.observable(true), enable: ko.observable(true) });
    this.PesoLiquido = PropertyEntity({ text: "Peso Líquido:", getType: typesKnockout.decimal, maxlength: 18, val: ko.observable("0,00"), def: "0,00", configDecimal: { precision: 2, allowZero: true, allowNegative: false }, enable: ko.observable(true), required: ko.observable(false) });
    this.Tara = PropertyEntity({ text: "*Tara:", getType: typesKnockout.decimal, maxlength: 18, val: ko.observable("0,00"), def: "0,00", configDecimal: { precision: 2, allowZero: false, allowNegative: false }, enable: ko.observable(true), required: ko.observable(true) });
    this.MetrosCubicos = PropertyEntity({ text: "M³:", getType: typesKnockout.decimal, maxlength: 18, val: ko.observable("0,00"), def: "0,00", configDecimal: { precision: 2, allowZero: true, allowNegative: false }, enable: ko.observable(true), required: ko.observable(true) });
    this.Valor = PropertyEntity({ text: "Valor:", getType: typesKnockout.decimal, maxlength: 18, val: ko.observable("0,00"), def: "0,00", configDecimal: { precision: 2, allowZero: true, allowNegative: false }, enable: ko.observable(true), required: ko.observable(false), visible: ko.observable(false) });
    this.TipoContainer = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "*Tipo de Container:", idBtnSearch: guid(), required: ko.observable(true), visible: ko.observable(true), enable: ko.observable(true) });
    this.TipoPropriedade = PropertyEntity({ val: ko.observable(EnumTipoPropriedadeContainer.Proprio), options: EnumTipoPropriedadeContainer.obterOpcoes(), def: EnumTipoPropriedadeContainer.Proprio, text: "*Propriedade: ", required: ko.observable(true), visible: ko.observable(true), enable: ko.observable(true) });
    this.TipoCarregamentoNavio = PropertyEntity({ val: ko.observable(EnumTipoCarregamentoNavio.NaoInformado), options: EnumTipoCarregamentoNavio.obterOpcoes(), def: EnumTipoCarregamentoNavio.NaoInformado, text: "Tipo: ", visible: ko.observable( _CONFIGURACAO_TMS.Transportador.AtivarControleCarregamentoNavio), enable: ko.observable(true) });
};

var CRUDContainer = function () {
    this.Adicionar = PropertyEntity({ eventClick: adicionarClick, type: types.event, text: "Adicionar", visible: ko.observable(true) });
    this.Atualizar = PropertyEntity({ eventClick: atualizarClick, type: types.event, text: "Atualizar", visible: ko.observable(false) });
    this.Excluir = PropertyEntity({ eventClick: excluirClick, type: types.event, text: "Excluir", visible: ko.observable(false) });
    this.Cancelar = PropertyEntity({ eventClick: cancelarClick, type: types.event, text: "Cancelar", visible: ko.observable(false) });
};

//*******EVENTOS*******


function loadContainer() {
    _container = new Container();
    KoBindings(_container, "knockoutCadastroContainer");

    HeaderAuditoria("Container", _container);

    _crudContainer = new CRUDContainer();
    KoBindings(_crudContainer, "knockoutCRUDContainer");

    _pesquisaContainer = new PesquisaContainer();
    KoBindings(_pesquisaContainer, "knockoutPesquisaContainer", false, _pesquisaContainer.Pesquisar.id);

    new BuscarTiposContainer(_container.TipoContainer);

    buscarContainer();
}

function adicionarClick(e, sender) {
    Salvar(_container, "Container/Adicionar", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Cadastrado com sucesso");
                _gridContainer.CarregarGrid();
                limparCamposContainer();
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
        }
    }, sender);
}

function atualizarClick(e, sender) {
    Salvar(_container, "Container/Atualizar", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Atualizado com sucesso");
                _gridContainer.CarregarGrid();
                limparCamposContainer();
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
        }
    }, sender);
}

function excluirClick(e, sender) {
    exibirConfirmacao("Confirmação", "Realmente deseja excluir o container " + _container.Descricao.val() + "?", function () {
        ExcluirPorCodigo(_container, "Container/ExcluirPorCodigo", function (arg) {
            if (arg.Success) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Excluído com sucesso");
                _gridContainer.CarregarGrid();
                limparCamposContainer();
            } else {
                exibirMensagem(tipoMensagem.falha, "falha", arg.Msg);
            }
        }, null);
    });
}

function cancelarClick(e) {
    limparCamposContainer();
}

//*******MÉTODOS*******


function buscarContainer() {
    var editar = { descricao: "Editar", id: "clasEditar", evento: "onclick", metodo: editarContainer, tamanho: "15", icone: "" };
    var menuOpcoes = new Object();
    menuOpcoes.tipo = TypeOptionMenu.link;
    menuOpcoes.opcoes = new Array();
    menuOpcoes.opcoes.push(editar);

    var configExportacao = {
        url: "Container/ExportarPesquisa",
        titulo: "Containers"
    }

    _gridContainer = new GridView(_pesquisaContainer.Pesquisar.idGrid, "Container/Pesquisa", _pesquisaContainer, menuOpcoes, null, 10, null, null, null, null, null, null, configExportacao);
    _gridContainer.CarregarGrid();
}

function editarContainer(containerGrid) {
    limparCamposContainer();
    _container.Codigo.val(containerGrid.Codigo);
    BuscarPorCodigo(_container, "Container/BuscarPorCodigo", function (arg) {
        _pesquisaContainer.ExibirFiltros.visibleFade(false);
        _crudContainer.Atualizar.visible(true);
        _crudContainer.Cancelar.visible(true);
        _crudContainer.Excluir.visible(true);
        _crudContainer.Adicionar.visible(false);

        verificarCamposHabilitados();

    }, null);
}

function limparCamposContainer() {
    _crudContainer.Atualizar.visible(false);
    _crudContainer.Cancelar.visible(false);
    _crudContainer.Excluir.visible(false);
    _crudContainer.Adicionar.visible(true);
    LimparCampos(_container);
    verificarCamposHabilitados();
}

function verificarCamposHabilitados() {
    var isEdicao = _container.Codigo.val() > 0;
    var possuiPermissao = _CONFIGURACAO_TMS.UsuarioAdministrador || VerificaSePossuiPermissaoEspecial(EnumPermissaoPersonalizada.Container_PermitirOperadorEditarTodasInformacoesContainer, _PermissoesPersonalizadas)

    var habiltado = !isEdicao || possuiPermissao;
    
    _container.Descricao.enable(habiltado);
    _container.CodigoIntegracao.enable(habiltado);
    _container.Status.enable(habiltado);
    _container.Numero.enable(habiltado);
    _container.PesoLiquido.enable(habiltado);
    _container.TipoContainer.enable(habiltado);
    _container.MetrosCubicos.enable(habiltado);
    _container.Valor.enable(habiltado);
    _container.TipoContainer.enable(habiltado);
    _container.TipoPropriedade.enable(habiltado);
    _container.TipoCarregamentoNavio.enable(habiltado);
}