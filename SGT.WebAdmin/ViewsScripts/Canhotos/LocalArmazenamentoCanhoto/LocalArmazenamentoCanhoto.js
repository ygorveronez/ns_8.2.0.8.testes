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
/// <reference path="../../Enumeradores/EnumTipoCanhoto.js" />
/// <reference path="../../Consultas/Localidade.js" />


//*******MAPEAMENTO KNOUCKOUT*******


var _gridLocalArmazenamentoCanhoto;
var _localArmazenamentoCanhoto;
var _pesquisaLocalArmazenamentoCanhoto;
var _existeLocalAtualComEspaco;
var _existeLocalAtualComEspacoNFe;
var _existeLocalAtualComEspacoAvulso;

var _LocalAtivo = [
    { text: "Sim", value: true },
    { text: "Não", value: false }
];

var _TipoCanhoto = [
    { text: "Canhoto de NF-e", value: EnumTipoCanhoto.NFe },
    { text: "Canhoto Avulso", value: EnumTipoCanhoto.Avulso }
];

var PesquisaLocalArmazenamentoCanhoto = function () {
    this.Descricao = PropertyEntity({ text: "Descrição: ", cssClass: ko.observable("col col-xs-12 col-sm-12 col-md-8 col-lg-8") });
    this.Filial = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Filial:", idBtnSearch: guid(), visible: ko.observable(false), required: false });
    
    var tipoCanhoto = _TipoCanhoto;
    tipoCanhoto.unshift({ text: "Todas", value: EnumTipoCanhoto.Todos });
    this.TipoCanhoto = PropertyEntity({ val: ko.observable(0), options: tipoCanhoto, def: 0, text: "Tipo de Canhoto para Armazenamento: ", issue: 1012, visible: ko.observable(true) });

    this.Pesquisar = PropertyEntity({
        eventClick: function (e) {
            _gridLocalArmazenamentoCanhoto.CarregarGrid(verificarLocalAtual);
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

var LocalArmazenamentoCanhoto = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Descricao = PropertyEntity({ text: "*Descrição: ", issue: 586, required: true, cssClass: ko.observable("col col-xs-12 col-sm-12 col-md-8 col-lg-8") });
    this.LocalArmazenagemAtual = PropertyEntity({ val: ko.observable(true), options: _LocalAtivo, def: true, text: "*Local Armazenamento Atual: " });
    this.TipoCanhoto = PropertyEntity({ val: ko.observable(1), options: _TipoCanhoto, def: 1, text: "*Tipo de Canhoto para Armazenamento: ", issue: 1012, visible: ko.observable(true) });
    this.CapacidadeArmazenagem = PropertyEntity({ text: "*Capacidade de Armazenamento: ", issue: 1013, maxlength: 8, getType: typesKnockout.int, required: true });
    this.QuantidadeArmazenada = PropertyEntity({ text: "Quantidade Armazenada: ", maxlength: 8, enable: false, getType: typesKnockout.int, required: false });
    this.DividirEmPacotesDe = PropertyEntity({ text: "Dividir canhotos em pacotes de : ", maxlength: 8, getType: typesKnockout.int, required: false });
    this.Filial = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "* Filial:", idBtnSearch: guid(), visible: ko.observable(false), required: false });

    this.Observacao = PropertyEntity({ text: "Observação: ", required: false, maxlength: 400 });
    this.LocalAtualJaExistente = PropertyEntity({ val: ko.observable(false), def: false, getType: typesKnockout.bool });

    this.Adicionar = PropertyEntity({ eventClick: adicionarClick, type: types.event, text: "Adicionar", visible: ko.observable(true) });
    this.Atualizar = PropertyEntity({ eventClick: atualizarClick, type: types.event, text: "Atualizar", visible: ko.observable(false) });
    this.Excluir = PropertyEntity({ eventClick: excluirClick, type: types.event, text: "Excluir", visible: ko.observable(false) });
    this.Cancelar = PropertyEntity({ eventClick: cancelarClick, type: types.event, text: "Cancelar", visible: ko.observable(false) });
}

//*******EVENTOS*******

function loadLocalArmazenamentoCanhoto() {

    _localArmazenamentoCanhoto = new LocalArmazenamentoCanhoto();
    KoBindings(_localArmazenamentoCanhoto, "knockoutCadastroLocalArmazenamentoCanhoto");

    _pesquisaLocalArmazenamentoCanhoto = new PesquisaLocalArmazenamentoCanhoto();
    KoBindings(_pesquisaLocalArmazenamentoCanhoto, "knockoutPesquisaLocalArmazenamentoCanhoto", false, _pesquisaLocalArmazenamentoCanhoto.Pesquisar.id);

    buscarLocalArmazenamentoCanhotos();

    BuscarFilial(_localArmazenamentoCanhoto.Filial);
    BuscarFilial(_pesquisaLocalArmazenamentoCanhoto.Filial);

    HeaderAuditoria("LocalArmazenamentoCanhoto", _localArmazenamentoCanhoto);

    AlterarLayoutTipoServico();
}

function adicionarClick(e, sender) {

    var existeLocalComEspaco = true;

    if (_CONFIGURACAO_TMS.TipoServicoMultisoftware == EnumTipoServicoMultisoftware.MultiTMS) {
        existeLocalComEspaco = _existeLocalAtualComEspaco;
    } else {
        if (e.TipoCanhoto.val() == EnumTipoCanhoto.NFe)
            existeLocalComEspaco = _existeLocalAtualComEspacoNFe;

        if (e.TipoCanhoto.val() == EnumTipoCanhoto.Avulso)
            existeLocalComEspaco = _existeLocalAtualComEspacoAvulso;
    }

    if (existeLocalComEspaco && e.LocalArmazenagemAtual.val()) {
        exibirConfirmacao("Confirmação", "Realmente deseja tornar o local " + e.Descricao.val() + " como padrão para armazenamento de canhotos?", function () {
            adicionar(e, sender);
        });
    } else {
        adicionar(e, sender);
    }
}


function atualizarClick(e, sender) {
    if (e.LocalAtualJaExistente.val() && e.LocalArmazenagemAtual.val()) {
        exibirConfirmacao("Confirmação", "Realmente deseja tornar o local " + e.Descricao.val() + " como padrão para armazenamento de canhotos?", function () {
            atualizar(e, sender);
        });
    } else {
        atualizar(e, sender);
    }
}


function excluirClick(e, sender) {
    exibirConfirmacao("Confirmação", "Realmente deseja excluir a localArmazenamentoCanhoto " + _localArmazenamentoCanhoto.Descricao.val() + "?", function () {
        ExcluirPorCodigo(_localArmazenamentoCanhoto, "LocalArmazenamentoCanhoto/ExcluirPorCodigo", function (arg) {
            if (arg.Success) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Excluído com sucesso");
                _gridLocalArmazenamentoCanhoto.CarregarGrid(verificarLocalAtual);
                limparCamposLocalArmazenamentoCanhoto();
            } else {
                exibirMensagem(tipoMensagem.falha, "falha", arg.Msg);
            }
        }, null);
    });
}

function cancelarClick(e) {
    limparCamposLocalArmazenamentoCanhoto();
}

//*******MÉTODOS*******

function adicionar(e, sender) {
    Salvar(e, "LocalArmazenamentoCanhoto/Adicionar", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Adicionado com sucesso.");
                _gridLocalArmazenamentoCanhoto.CarregarGrid(verificarLocalAtual);
                limparCamposLocalArmazenamentoCanhoto();
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    }, sender);
}

function atualizar(e, sender) {
    Salvar(e, "LocalArmazenamentoCanhoto/Atualizar", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Atualizado com sucesso.");
                _gridLocalArmazenamentoCanhoto.CarregarGrid(verificarLocalAtual);
                limparCamposLocalArmazenamentoCanhoto();
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "falha", arg.Msg);
        }
    }, sender);
}

function buscarLocalArmazenamentoCanhotos() {
    
    var editar = { descricao: "Editar", id: "clasEditar", evento: "onclick", metodo: editarLocalArmazenamentoCanhoto, tamanho: "15", icone: "" };
    var menuOpcoes = { tipo: TypeOptionMenu.link, opcoes: [editar] };
    _gridLocalArmazenamentoCanhoto = new GridView(_pesquisaLocalArmazenamentoCanhoto.Pesquisar.idGrid, "LocalArmazenamentoCanhoto/Pesquisa", _pesquisaLocalArmazenamentoCanhoto, menuOpcoes);
    _gridLocalArmazenamentoCanhoto.CarregarGrid(verificarLocalAtual);
    
}

function verificarLocalAtual() {
    if (_CONFIGURACAO_TMS.TipoServicoMultisoftware == EnumTipoServicoMultisoftware.MultiTMS) {
        executarReST("LocalArmazenamentoCanhoto/VerificarSeExisteLocalArmazenagemAtualComEspaco", null, function (arg) {
            if (arg.Success) {
                _existeLocalAtualComEspaco = arg.Data;
            } else {
                exibirMensagem(tipoMensagem.falha, "falha", arg.Msg);
            }
        });
    } else {
        executarReST("LocalArmazenamentoCanhoto/VerificarSeExisteLocalArmazenagemNFeAtualComEspaco", null, function (arg) {
            if (arg.Success) {
                _existeLocalAtualComEspacoNFe = arg.Data;
                executarReST("LocalArmazenamentoCanhoto/VerificarSeExisteLocalArmazenagemAvulsoAtualComEspaco", null, function (arg) {
                    if (arg.Success) {
                        _existeLocalAtualComEspacoAvulso = arg.Data;
                    } else {
                        exibirMensagem(tipoMensagem.falha, "falha", arg.Msg);
                    }
                });
            } else {
                exibirMensagem(tipoMensagem.falha, "falha", arg.Msg);
            }
        });
    }
}

function AlterarLayoutTipoServico() {
    if (_CONFIGURACAO_TMS.TipoServicoMultisoftware == EnumTipoServicoMultisoftware.MultiTMS) {
        _localArmazenamentoCanhoto.TipoCanhoto.visible(false);
        _pesquisaLocalArmazenamentoCanhoto.TipoCanhoto.visible(false);

        _localArmazenamentoCanhoto.Descricao.cssClass("col col-xs-12 col-sm-12 col-md-12 col-lg-12");
        _pesquisaLocalArmazenamentoCanhoto.Descricao.cssClass("col col-xs-12 col-sm-12 col-md-12 col-lg-12");

    } if (_CONFIGURACAO_TMS.TipoServicoMultisoftware == EnumTipoServicoMultisoftware.MultiEmbarcador) {

        classCSS = "col col-xs-12 col-sm-12 col-md-8 col-lg-8";
        if (_CONFIGURACAO_TMS.ArmazenamentoCanhotoComFilial) {
            classCSS = "col col-xs-12 col-sm-12 col-md-4 col-lg-4";

            _localArmazenamentoCanhoto.Filial.visible(true).required = true;
            _pesquisaLocalArmazenamentoCanhoto.Filial.visible(true).required = true;
        }

        _localArmazenamentoCanhoto.Descricao.cssClass(classCSS);
        _pesquisaLocalArmazenamentoCanhoto.Descricao.cssClass(classCSS);
    }
}

function editarLocalArmazenamentoCanhoto(localArmazenamentoCanhotoGrid) {
    limparCamposLocalArmazenamentoCanhoto();
    _localArmazenamentoCanhoto.Codigo.val(localArmazenamentoCanhotoGrid.Codigo);
    BuscarPorCodigo(_localArmazenamentoCanhoto, "LocalArmazenamentoCanhoto/BuscarPorCodigo", function (arg) {
        _pesquisaLocalArmazenamentoCanhoto.ExibirFiltros.visibleFade(false);
        _localArmazenamentoCanhoto.Atualizar.visible(true);
        _localArmazenamentoCanhoto.Cancelar.visible(true);
        _localArmazenamentoCanhoto.Excluir.visible(true);
        _localArmazenamentoCanhoto.Adicionar.visible(false);
    }, null);
}

function limparCamposLocalArmazenamentoCanhoto() {
    _localArmazenamentoCanhoto.Atualizar.visible(false);
    _localArmazenamentoCanhoto.Cancelar.visible(false);
    _localArmazenamentoCanhoto.Excluir.visible(false);
    _localArmazenamentoCanhoto.Adicionar.visible(true);
    LimparCampos(_localArmazenamentoCanhoto);
}

