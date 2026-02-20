/// <reference path="../../../js/libs/jquery-2.1.1.js" />
/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/knockout-3.1.0.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/bootstrap/bootstrap.js" />
/// <reference path="../../../js/libs/jquery.blockui.js" />
/// <reference path="../../../js/Global/knoutViewsSlides.js" />
/// <reference path="../../../js/libs/jquery.maskMoney.js" />
/// <reference path="../../../js/plugin/datatables/jquery.dataTables.js" />


//*******MAPEAMENTO KNOUCKOUT*******

var _centroCustoViagem;
var _pesquisaCentroCustoViagem;
var _gridCentroCustoViagem;

var CentroCustoViagem = function () {
    // Codigo da entidade
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });

    this.CodigoIntegracao = PropertyEntity({ text: Localization.Resources.Logistica.CentroCustoViagem.CodigoIntegracao, required: true, getType: typesKnockout.string, val: ko.observable("") , maxlengh : 200});

    this.Descricao = PropertyEntity({ text: Localization.Resources.Logistica.CentroCustoViagem.Descricao, required: true, getType: typesKnockout.string, val: ko.observable("") });
    this.Status = PropertyEntity({ text: Localization.Resources.Logistica.CentroCustoViagem.Status, val: ko.observable(true), options: _status, def: true });

    this.OpenTech = PropertyEntity({ getType: typesKnockout.dynamic });
    this.RepomFrete = PropertyEntity({ getType: typesKnockout.dynamic });
}

var PesquisaCentroCustoViagem = function () {
    this.Descricao = PropertyEntity({ text: Localization.Resources.Logistica.CentroCustoViagem.Descricao, required: false, getType: typesKnockout.string, val: ko.observable("") });
    this.Status = PropertyEntity({ text: Localization.Resources.Logistica.CentroCustoViagem.Status, val: ko.observable(0), options: _statusPesquisa, def: 0 });
    this.CodigoIntegracao = PropertyEntity({ text: Localization.Resources.Logistica.CentroCustoViagem.CodigoIntegracao, required: false, getType: typesKnockout.string, val: ko.observable("")});

    this.Pesquisar = PropertyEntity({
        eventClick: function (e) {
            _gridCentroCustoViagem.CarregarGrid();
        }, type: types.event, text: Localization.Resources.Gerais.Geral.Pesquisar, idGrid: guid(), visible: ko.observable(true)
    });
    this.ExibirFiltros = PropertyEntity({
        eventClick: function (e) {
            if (e.ExibirFiltros.visibleFade()) {
                e.ExibirFiltros.visibleFade(false);
            } else {
                e.ExibirFiltros.visibleFade(true);
            }
        }, type: types.event, text: Localization.Resources.Gerais.Geral.FiltrosPesquisa, idFade: guid(), visibleFade: ko.observable(false), visible: ko.observable(true)
    });
}

var CRUDCentroCustoViagem = function () {
    this.Adicionar = PropertyEntity({ eventClick: adicionarClick, type: types.event, text: Localization.Resources.Gerais.Geral.Adicionar, visible: ko.observable(true) });
    this.Atualizar = PropertyEntity({ eventClick: atualizarClick, type: types.event, text: Localization.Resources.Gerais.Geral.Atualizar, visible: ko.observable(false) });
    this.Excluir = PropertyEntity({ eventClick: excluirClick, type: types.event, text: Localization.Resources.Gerais.Geral.Excluir, visible: ko.observable(false) });
    this.Cancelar = PropertyEntity({ eventClick: cancelarClick, type: types.event, text: Localization.Resources.Gerais.Geral.Cancelar, visible: ko.observable(false) });
};


//*******EVENTOS*******
function loadCentroCustoViagem() {
    //-- Knouckout
    // Instancia pesquisa
    _pesquisaCentroCustoViagem = new PesquisaCentroCustoViagem();
    KoBindings(_pesquisaCentroCustoViagem, "knockoutPesquisaCentroCustoViagem", false, _pesquisaCentroCustoViagem.Pesquisar.id);

    // Instancia objeto principal
    _centroCustoViagem = new CentroCustoViagem();
    KoBindings(_centroCustoViagem, "knockoutCentroCustoViagem");

    _CRUDCentroCustoViagem = new CRUDCentroCustoViagem();
    KoBindings(_CRUDCentroCustoViagem, "knockoutCRUDCentroCustoViagem");

    loadCentroCustoViagemOpenTech();
    loadCentroCustoViagemRepomFrete();

    HeaderAuditoria("CentroCustoViagem", _centroCustoViagem);

    // Inicia busca
    buscarCentroCustoViagem();
}

function adicionarClick(e, sender) {
    _centroCustoViagem.OpenTech.val(JSON.stringify(RetornarObjetoPesquisa(_openTech)));
    _centroCustoViagem.RepomFrete.val(JSON.stringify(RetornarObjetoPesquisa(_repomFrete)));
    let _dadosSalvar = RetornarObjetoPesquisa(_centroCustoViagem);

    executarReST("CentroCustoViagem/Adicionar", _dadosSalvar, function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, Localization.Resources.Gerais.Geral.CadastradoComSucesso);
                _gridCentroCustoViagem.CarregarGrid();
                limparCamposCentroCustoViagem();
            } else {
                exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.Aviso, arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, arg.Msg);
        }
    }, sender);
}

function atualizarClick(e, sender) {
    _centroCustoViagem.OpenTech.val(JSON.stringify(RetornarObjetoPesquisa(_openTech)));
    _centroCustoViagem.RepomFrete.val(JSON.stringify(RetornarObjetoPesquisa(_repomFrete)));
    let _dadosSalvar = RetornarObjetoPesquisa(_centroCustoViagem);

    executarReST("CentroCustoViagem/Atualizar", _dadosSalvar, function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, Localization.Resources.Gerais.Geral.AtualizadoComSucesso);
                _gridCentroCustoViagem.CarregarGrid();
                limparCamposCentroCustoViagem();
            } else {
                exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.Aviso, arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, arg.Msg);
        }
    })

    /*
    Salvar(_dadosSalvar, "CentroCustoViagem/Atualizar", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, Localization.Resources.Gerais.Geral.AtualizadoComSucesso);
                _gridCentroCustoViagem.CarregarGrid();
                limparCamposCentroCustoViagem();
            } else {
                exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.Aviso, arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, arg.Msg);
        }
    }, sender);
    */
}

function excluirClick(e, sender) {
    exibirConfirmacao(Localization.Resources.Gerais.Geral.Atencao, Localization.Resources.Logistica.CentroCustoViagem.DesejaRealmenteExcluirCadastro, function () {
        ExcluirPorCodigo(_centroCustoViagem, "CentroCustoViagem/ExcluirPorCodigo", function (arg) {
            if (arg.Success) {
                if (arg.Data) {
                    exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, Localization.Resources.Gerais.Geral.ExcluidoComSucesso);
                    _gridCentroCustoViagem.CarregarGrid();
                    limparCamposCentroCustoViagem();
                } else {
                    exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.Aviso, arg.Msg);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, arg.Msg);
            }

        }, null);
    });
}

function cancelarClick(e) {
    limparCamposCentroCustoViagem();
}

function editarCentroCustoViagemClick(itemGrid) {
    // Limpa os campos
    limparCamposCentroCustoViagem();

    // Seta o codigo do objeto
    _centroCustoViagem.Codigo.val(itemGrid.Codigo);

    // Busca informacoes para edicao
    BuscarPorCodigo(_centroCustoViagem, "CentroCustoViagem/BuscarPorCodigo", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                preencherOpenTech(arg.Data.OpenTech);
                preencherRepomFrete(arg.Data.RepomFrete);
                // Esconde pesqusia
                _pesquisaCentroCustoViagem.ExibirFiltros.visibleFade(false);

                // Alternas os campos de CRUD
                _CRUDCentroCustoViagem.Atualizar.visible(true);
                _CRUDCentroCustoViagem.Excluir.visible(true);
                _CRUDCentroCustoViagem.Cancelar.visible(true);
                _CRUDCentroCustoViagem.Adicionar.visible(false);
            } else {
                exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.Aviso, arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, arg.Msg);
        }
    }, null);
}



//*******MÉTODOS*******
function buscarCentroCustoViagem() {
    //-- Grid
    // Opcoes
    let editar = { descricao: Localization.Resources.Gerais.Geral.Editar, id: "clasEditar", evento: "onclick", metodo: editarCentroCustoViagemClick, tamanho: "20", icone: "" };

    // Menu
    let menuOpcoes = {
        tipo: TypeOptionMenu.link,
        opcoes: [editar]
    };

    // Inicia Grid de busca
    _gridCentroCustoViagem = new GridView(_pesquisaCentroCustoViagem.Pesquisar.idGrid, "CentroCustoViagem/Pesquisa", _pesquisaCentroCustoViagem, menuOpcoes, null);
    _gridCentroCustoViagem.CarregarGrid();
}

function limparCamposCentroCustoViagem() {
    _CRUDCentroCustoViagem.Atualizar.visible(false);
    _CRUDCentroCustoViagem.Cancelar.visible(false);
    _CRUDCentroCustoViagem.Excluir.visible(false);
    _CRUDCentroCustoViagem.Adicionar.visible(true);
    LimparCampos(_centroCustoViagem);
    LimparCampos(_openTech);
    LimparCampos(_repomFrete);
}

function preencherOpenTech(dadosOpenTech) {
    if (dadosOpenTech) {
        _openTech.CodigoTransportadorOpenTech.val(dadosOpenTech.CodigoTransportadorOpenTech);
    }
}

function preencherRepomFrete(dadosRepomFrete) {
    if (dadosRepomFrete) {
        _repomFrete.CodigoFilialRepom.val(dadosRepomFrete.CodigoFilialRepom);
    }
}