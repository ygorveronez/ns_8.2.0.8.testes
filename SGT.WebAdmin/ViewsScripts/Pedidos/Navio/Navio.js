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
/// <reference path="../../Enumeradores/EnumTipoEmbarcacao.js" />


//*******MAPEAMENTO KNOUCKOUT*******

var _gridNavio;
var _navio;
var _pesquisaNavio;

var PesquisaNavio = function () {
    this.Descricao = PropertyEntity({ text: "Descrição: " });
    this.CodigoIntegracao = PropertyEntity({ text: "Código Integração: " });
    this.Status = PropertyEntity({ text: "Status: ", val: ko.observable(1), options: _statusPesquisa, def: 1 });

    this.Pesquisar = PropertyEntity({
        eventClick: function (e) {
            _gridNavio.CarregarGrid();
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

var Navio = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Descricao = PropertyEntity({ text: "*Descrição: ", required: ko.observable(true), maxlength: 500 });
    this.CodigoIntegracao = PropertyEntity({ text: "Código Integração: ", required: ko.observable(false), maxlength: 50 });
    this.Status = PropertyEntity({ text: "Status: ", val: ko.observable(true), options: _status, def: true, required: ko.observable(true), enable: ko.observable(true) });
    this.Irin = PropertyEntity({ text: "*Código Irin: ", required: ko.observable(true), maxlength: 10 });
    this.CodigoEmbarcacao = PropertyEntity({ text: ko.observable("*Código Embarcação: "), required: ko.observable(true), maxlength: 100 });
    this.NavioID = PropertyEntity({ text: ko.observable("Navio ID: "), required: ko.observable(false), maxlength: 500, visible: ko.observable(false) });
    this.CodigoDocumento = PropertyEntity({ text: "Cod. Documentação: ", required: ko.observable(false), maxlength: 50, visible: ko.observable(_CONFIGURACAO_TMS.TipoServicoMultisoftware === EnumTipoServicoMultisoftware.MultiTMS) });
    this.CodigoIMO = PropertyEntity({ text: (_CONFIGURACAO_TMS.AtivarIntegracaoRecebimentoNavioEMP ? "*Cod. IMO: " : "Cod. IMO: "), required: ko.observable(_CONFIGURACAO_TMS.AtivarIntegracaoRecebimentoNavioEMP ? true : false), maxlength: 50, visible: ko.observable(_CONFIGURACAO_TMS.TipoServicoMultisoftware === EnumTipoServicoMultisoftware.MultiTMS) });
    this.TipoEmbarcacao = PropertyEntity({ val: ko.observable(""), options: EnumTipoEmbarcacao.obterOpcoes(), def: "", text: "*Tipo Embarcação: ", required: ko.observable(true), visible: ko.observable(true), enable: ko.observable(true) });
    this.CapacidadePlug = PropertyEntity({ getType: typesKnockout.decimal, text: "Capacidade Plugs:", configDecimal: { precision: 4, allowZero: true }, visible: ko.observable(_CONFIGURACAO_TMS.TipoServicoMultisoftware === EnumTipoServicoMultisoftware.MultiTMS && _CONFIGURACAO_TMS.Transportador.AtivarControleCarregamentoNavio) });
    this.CapacidadeTeus = PropertyEntity({ getType: typesKnockout.decimal, text: "Capacidade Teus:", configDecimal: { precision: 4, allowZero: true }, visible: ko.observable(_CONFIGURACAO_TMS.TipoServicoMultisoftware === EnumTipoServicoMultisoftware.MultiTMS && _CONFIGURACAO_TMS.Transportador.AtivarControleCarregamentoNavio )});
    this.CapacidadeTons = PropertyEntity({ getType: typesKnockout.decimal, text: "Capacidade Tons:", configDecimal: { precision: 4, allowZero: true }, visible: ko.observable(_CONFIGURACAO_TMS.TipoServicoMultisoftware === EnumTipoServicoMultisoftware.MultiTMS && _CONFIGURACAO_TMS.Transportador.AtivarControleCarregamentoNavio )});
    this.CodigoOperador = PropertyEntity({ text: ko.observable("Código Operador: "), required: ko.observable(false), maxlength: 200, visible: ko.observable(_CONFIGURACAO_TMS.TipoServicoMultisoftware === EnumTipoServicoMultisoftware.MultiTMS && _CONFIGURACAO_TMS.PossuiIntegracaoIntercab) });
    this.CodigoNavio = PropertyEntity({ text: ko.observable("Código do Navio: "), required: ko.observable(false), maxlength: 3, visible: ko.observable(_CONFIGURACAO_TMS.TipoServicoMultisoftware === EnumTipoServicoMultisoftware.MultiTMS && _CONFIGURACAO_TMS.PossuiIntegracaoIntercab) });

    this.Operadores = PropertyEntity({ type: types.listEntity, list: new Array(), required: false, codEntity: ko.observable(0), idGrid: guid() });
};

var CRUDNavio = function () {
    this.Adicionar = PropertyEntity({ eventClick: adicionarClick, type: types.event, text: "Adicionar", visible: ko.observable(true) });
    this.Atualizar = PropertyEntity({ eventClick: atualizarClick, type: types.event, text: "Atualizar", visible: ko.observable(false) });
    this.Excluir = PropertyEntity({ eventClick: excluirClick, type: types.event, text: "Excluir", visible: ko.observable(false) });
    this.Cancelar = PropertyEntity({ eventClick: cancelarClick, type: types.event, text: "Cancelar", visible: ko.observable(false) });
};

//*******EVENTOS*******


function loadNavio() {
    _navio = new Navio();
    KoBindings(_navio, "knockoutCadastroNavio");

    HeaderAuditoria("Navio", _navio);

    _crudNavio = new CRUDNavio();
    KoBindings(_crudNavio, "knockoutCRUDNavio");

    _pesquisaNavio = new PesquisaNavio();
    KoBindings(_pesquisaNavio, "knockoutPesquisaNavio", false, _pesquisaNavio.Pesquisar.id);

    if (_CONFIGURACAO_TMS.RemoverObrigacaoCodigoEmbarcacaoCadastroNavio) {
        _navio.CodigoEmbarcacao.required(false);
        _navio.CodigoEmbarcacao.text("Código Embarcação: ");
    }

    if (_CONFIGURACAO_TMS.PossuiIntegracaoIntercab) {
        _navio.NavioID.required(true);
        _navio.NavioID.visible(true);
        _navio.NavioID.text("*Navio ID: ");
    }

    buscarNavio();

    LoadNaviosOperadores();
}

function adicionarClick(e, sender) {
    Salvar(_navio, "Navio/Adicionar", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Cadastrado com sucesso");
                _gridNavio.CarregarGrid();
                limparCamposNavio();
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    }, sender);
}

function atualizarClick(e, sender) {
    Salvar(_navio, "Navio/Atualizar", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Atualizado com sucesso");
                _gridNavio.CarregarGrid();
                limparCamposNavio();
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    }, sender);
}

function excluirClick(e, sender) {
    exibirConfirmacao("Confirmação", "Realmente deseja excluir o navio" + _navio.Descricao.val() + "?", function () {
        ExcluirPorCodigo(_navio, "Navio/ExcluirPorCodigo", function (arg) {
            if (arg.Success) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Excluído com sucesso");
                _gridNavio.CarregarGrid();
                limparCamposNavio();
            } else {
                exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
            }
        }, null);
    });
}

function cancelarClick(e) {
    limparCamposNavio();
}

//*******MÉTODOS*******


function buscarNavio() {
    let editar = { descricao: "Editar", id: "clasEditar", evento: "onclick", metodo: editarNavio, tamanho: "15", icone: "" };
    let menuOpcoes = new Object();
    menuOpcoes.tipo = TypeOptionMenu.link;
    menuOpcoes.opcoes = new Array();
    menuOpcoes.opcoes.push(editar);

    var configuracoesExportacao = { url: "Navio/ExportarPesquisa", titulo: "Navio" };

    _gridNavio = new GridViewExportacao(_pesquisaNavio.Pesquisar.idGrid, "Navio/Pesquisa", _pesquisaNavio, menuOpcoes, configuracoesExportacao);
    _gridNavio.CarregarGrid();
}

function editarNavio(navioGrid) {
    limparCamposNavio();
    _navio.Codigo.val(navioGrid.Codigo);

    BuscarPorCodigo(_navio, "Navio/BuscarPorCodigo", function (arg) {
        _pesquisaNavio.ExibirFiltros.visibleFade(false);
        _crudNavio.Atualizar.visible(true);
        _crudNavio.Cancelar.visible(true);
        _crudNavio.Excluir.visible(true);
        _crudNavio.Adicionar.visible(false);

        RecarregarGridNaviosOperadores();
        
    }, null);
}

function limparCamposNavio() {
    _crudNavio.Atualizar.visible(false);
    _crudNavio.Cancelar.visible(false);
    _crudNavio.Excluir.visible(false);
    _crudNavio.Adicionar.visible(true);
    LimparCampos(_navio);
}