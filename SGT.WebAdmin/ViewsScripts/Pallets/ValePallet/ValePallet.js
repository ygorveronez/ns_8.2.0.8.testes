/// <reference path="Etapas.js" />
/// <reference path="Lancamento.js" />
/// <reference path="Recolhimento.js" />
/// <reference path="Resumo.js" />
/// <reference path="../../Consultas/Cliente.js" />
/// <reference path="../../Consultas/Filial.js" />
/// <reference path="../../Enumeradores/EnumSituacaoValePallet.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _gridValePallet;
var _valePallet;
var _CRUDValePallet;
var _pesquisaValePallet;

var EventoValePallet = {
    Adicionado: $.Event("ValePalletAdicionado"),
    Alterado: $.Event("ValePalletAlterado"),
};

var ValePallet = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0 });
    this.Situacao = PropertyEntity({ val: ko.observable(EnumSituacaoValePallet.todas), def: EnumSituacaoValePallet.todas });
};

var CRUDValePallet = function () {
    this.Adicionar = PropertyEntity({ eventClick: adicionarValePalletClick, type: types.event, text: "Adicionar", idGrid: guid(), visible: ko.observable(true) });
    this.Limpar = PropertyEntity({ eventClick: limparCamposValePalletClick, type: types.event, text: "Limpar (Gerar Novo Vale)", idGrid: guid(), visible: ko.observable(false) });
    this.Cancelar = PropertyEntity({ eventClick: cancelarValePalletClick, type: types.event, text: "Cancelar", idGrid: guid(), visible: ko.observable(false) });
    this.Finalizar = PropertyEntity({ eventClick: finalizarValePalletClick, type: types.event, text: "Finalizar", idGrid: guid(), visible: ko.observable(false) });
    this.Imprimir = PropertyEntity({ eventClick: imprimirValePalletClick, type: types.event, text: "Imprimir", idGrid: guid(), visible: ko.observable(false) });
    this.Excluir = PropertyEntity({ eventClick: excluirValePalletClick, type: types.event, text: "Excluir", idGrid: guid(), visible: ko.observable(false) });
};

var PesquisaValePallet = function () {
    this.Numero = PropertyEntity({ text: "Número:", getType: typesKnockout.int });
    this.Numero.configInt.thousands = '';
    this.NFe = PropertyEntity({ text: "NF-e:", getType: typesKnockout.int });
    this.NFe.configInt.thousands = '';

    this.DataInicio = PropertyEntity({ text: "Data Inicial: ", getType: typesKnockout.date, visible: true });
    this.DataFim = PropertyEntity({ text: "Data Final: ", getType: typesKnockout.date, visible: true });
    this.DataInicio.dateRangeLimit = this.DataFim;
    this.DataFim.dateRangeInit = this.DataInicio;

    this.Filial = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Filial:", idBtnSearch: guid(), visible: ko.observable(true) });
    this.Cliente = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Cliente:", idBtnSearch: guid(), visible: ko.observable(true) });
    this.Situacao = PropertyEntity({ val: ko.observable(EnumSituacaoValePallet.todas), options: EnumSituacaoValePallet.obterOpcoes(), def: EnumSituacaoValePallet.todas, text: "Situação: " });

    this.Pesquisar = PropertyEntity({
        eventClick: function (e) {
            _gridValePallet.CarregarGrid();
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

//*******EVENTOS*******

function loadValePallet() {
    LoadValePalletConstrutor();

    _pesquisaValePallet = new PesquisaValePallet();
    KoBindings(_pesquisaValePallet, "knockoutPesquisaValePallet", false, _pesquisaValePallet.Pesquisar.id);

    new BuscarClientes(_pesquisaValePallet.Cliente);
    new BuscarFilial(_pesquisaValePallet.Filial);

    HeaderAuditoria("ValePallet", _valePallet);

    BuscarValePallet();
}

function LoadValePalletConstrutor(knoutCarga) {
    _valePallet = new ValePallet();

    _CRUDValePallet = new CRUDValePallet();
    KoBindings(_CRUDValePallet, "knockoutCRUDValePallet");

    LoadEtapasValePallet();
    LoadLancamento(knoutCarga);
    LoadRecolhimento();
    LoadResumoValePallet();
    LoadDevolucao();
}

function LoadValePalletExterno(containerHtml, knoutCarga) {
    $.get("Pallets/ValePallet?dyn=" + guid(), function (viewContent) {
        var $view = $(viewContent);

        var $content = $view.find("#container-externo");

        $(containerHtml).html($content.html());

        LoadValePalletConstrutor(knoutCarga);
    });
}

function limparCamposValePalletClick(e, sender) {
    LimparCamposValePallet();
}

function adicionarValePalletClick(e, sender) {
    Salvar(_lancamento, "ValePallet/Adicionar", function (arg) {
        if (arg.Success) {
            if (arg.Data !== false) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Vale Pallet adicionado com sucesso.");
                if (_gridValePallet != null) {
                    _gridValePallet.CarregarGrid();

                    BuscarValePalletPorCodigo(arg.Data.Codigo);
                }
                $('body').trigger(EventoValePallet.Adicionado);
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    }, sender);
}

function cancelarValePalletClick(e, sender) {
    var msg = "Você tem certeza que deseja cancelar o Vale Pallet?";
    exibirConfirmacao("Vale Pallet", msg, function () {
        Salvar(_valePallet, "ValePallet/Cancelar", function (arg) {
            if (arg.Success) {
                if (arg.Data !== false) {
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "ValePallet cancelado com sucesso.");
                    if (_gridValePallet != null)
                        _gridValePallet.CarregarGrid();

                    LimparCamposValePallet();
                } else {
                    exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
            }
        }, sender);
    });
}

function finalizarValePalletClick(e, sender) {
    var msg = "Você tem certeza que deseja finalizar o Vale Pallet?";
    exibirConfirmacao("Finalizar Vale Pallet", msg, function () {
        Salvar(_recolhimento, "ValePallet/Finalizar", function (arg) {
            if (arg.Success) {
                if (arg.Data !== false) {
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Vale Pallet finalizado com sucesso.");
                    if (_gridValePallet != null) {
                        _gridValePallet.CarregarGrid();

                        BuscarValePalletPorCodigo(arg.Data.Codigo);
                    }
                    $('body').trigger(EventoValePallet.Alterado);
                } else {
                    exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
            }
        }, sender);
    });
}

function imprimirValePalletClick() {
    executarDownload("ValePallet/Imprimir", { Codigo: _valePallet.Codigo.val() });
}

function excluirValePalletClick(e, sender) {
    var msg = "Você tem certeza que deseja excluir o Vale Pallet?";
    exibirConfirmacao("Vale Pallet", msg, function () {
        Salvar(_valePallet, "ValePallet/ExcluirPorCodigo", function (arg) {
            if (arg.Success) {
                if (arg.Data !== false) {
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "ValePallet excluído com sucesso.");
                    if (_gridValePallet != null)
                        _gridValePallet.CarregarGrid();

                    LimparCamposValePallet();
                } else {
                    exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
            }
        }, sender);
    });
}

//*******MÉTODOS*******

function BuscarValePallet() {
    var editar = { descricao: "Editar", id: "clasEditar", evento: "onclick", metodo: editarValePalletGrid, tamanho: "15", icone: "" };
    var menuOpcoes = {
        tipo: TypeOptionMenu.link,
        opcoes: [editar]
    };

    _gridValePallet = new GridView(_pesquisaValePallet.Pesquisar.idGrid, "ValePallet/Pesquisa", _pesquisaValePallet, menuOpcoes);
    _gridValePallet.CarregarGrid();
}

function editarValePalletGrid(itemGrid) {
    LimparCamposValePallet();

    _pesquisaValePallet.ExibirFiltros.visibleFade(false);

    BuscarValePalletPorCodigo(itemGrid.Codigo);
}

function BuscarValePalletPorCodigo(codigo, callback) {
    executarReST("ValePallet/BuscarPorCodigo", { Codigo: codigo }, function (arg) {
        if (arg.Data != null) {
            EditarValePallet(arg.Data);

            DadosLancamento(arg.Data);

            DadosDevolucao(arg.Data);

            DadosRecolhimento(arg.Data);

            PreencherResumoValePallet(arg.Data);

            SetarEtapasValePallet();
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
        if (callback != null)
            callback();
    }, null);
}

function EditarValePallet(data) {
    _valePallet.Situacao.val(data.Situacao);
    _valePallet.Codigo.val(data.Codigo);

    if (data.Situacao == EnumSituacaoValePallet.AgFinalizacao) {
        _CRUDValePallet.Adicionar.visible(false);
        _CRUDValePallet.Cancelar.visible(true);
        _CRUDValePallet.Finalizar.visible(true);
        //_CRUDValePallet.Imprimir.visible(true);
        _CRUDValePallet.Limpar.visible(true);
    } else if (data.Situacao == EnumSituacaoValePallet.AgDevolucao) {
        _CRUDValePallet.Adicionar.visible(false);
        _CRUDValePallet.Limpar.visible(true);
        _CRUDValePallet.Excluir.visible(true);
    } else if (data.Situacao == EnumSituacaoValePallet.Finalizado) {
        _CRUDValePallet.Adicionar.visible(false);
        _CRUDValePallet.Limpar.visible(true);
    } else if (data.Situacao == EnumSituacaoValePallet.Cancelado) {
        _CRUDValePallet.Adicionar.visible(false);
        _CRUDValePallet.Limpar.visible(true);
    }
}

function LimparCamposValePallet() {
    LimparCampos(_valePallet);

    _CRUDValePallet.Adicionar.visible(true);
    _CRUDValePallet.Cancelar.visible(false);
    _CRUDValePallet.Finalizar.visible(false);
    _CRUDValePallet.Imprimir.visible(false);
    _CRUDValePallet.Limpar.visible(false);
    _CRUDValePallet.Excluir.visible(false);

    LimparCamposDadosLancamento();
    LimparCamposDadosRecolhimento();
    LimparResumoValePallet();
    SetarEtapaValePalletInicio();
}