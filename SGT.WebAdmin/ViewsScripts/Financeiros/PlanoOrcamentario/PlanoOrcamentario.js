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
/// <reference path="../../Consultas/CentroResultado.js" />
/// <reference path="../../Enumeradores/EnumPeriodicidade.js" />
/// <reference path="ContaPlanoOrcamentario.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _gridPlanoOrcamentario;
var _planoOrcamentario;
var _pesquisaPlanoOrcamentario;

var _periodo = [
    { text: "Mensal", value: EnumPeriodicidade.Mensal },
    { text: "Semanal", value: EnumPeriodicidade.Semanal },
    { text: "Bimestral", value: EnumPeriodicidade.Bimestral },
    { text: "Trimestral", value: EnumPeriodicidade.Trimestral },
    { text: "Semestral", value: EnumPeriodicidade.Semestral },
    { text: "Anual", value: EnumPeriodicidade.Anual },
];

var PesquisaPlanoOrcamentario = function () {
    this.CentroResultado = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Centro de Resultado:", idBtnSearch: guid() });
    this.DataBase = PropertyEntity({ text: "Data Base: ", getType: typesKnockout.date });

    this.Pesquisar = PropertyEntity({
        eventClick: function (e) {
            _gridPlanoOrcamentario.CarregarGrid();
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

var PlanoOrcamentario = function () {
    this.Codigo = PropertyEntity({ text: "Código:", val: ko.observable(0), def: 0, getType: typesKnockout.int, enable: ko.observable(false) });
    this.Observacao = PropertyEntity({ text: "*Descrição: ", required: true, maxlength: 1000 });
    this.DataBase = PropertyEntity({ text: "*Data Base: ", required: true, getType: typesKnockout.date, enable: ko.observable(true) });
    this.Valor = PropertyEntity({ text: "*Valor: ", required: true, getType: typesKnockout.decimal, enable: ko.observable(true) });
    this.NumeroOcorrencia = PropertyEntity({ getType: typesKnockout.int, text: "*Numero Ocorrência:", visible: ko.observable(false), enable: ko.observable(true) });
    this.Repetir = PropertyEntity({ val: ko.observable(false), getType: typesKnockout.bool, text: "Deseja REPETIR o plano orçamentário? Isso gerará mais planos do mesmo valor informado.", def: false, enable: ko.observable(true), eventChange: HabilitarCamposRepetir });
    this.Dividir = PropertyEntity({ val: ko.observable(false), getType: typesKnockout.bool, text: "Deseja DIVIDIR o plano orçamentário? Isso gerará mais planos com o valor informado dividido entre eles.", def: false, enable: ko.observable(true), eventChange: HabilitarCamposDividir });

    this.Periodicidade = PropertyEntity({ val: ko.observable(EnumPeriodicidade.Mensal), def: EnumPeriodicidade.Mensal, options: _periodo, text: "*Repetição:", required: false, visible: ko.observable(false), enable: ko.observable(true) });
    this.CentroResultado = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "*Centro de Resultado:", idBtnSearch: guid(), required: true, enable: ko.observable(true) });
    this.ListaConta = PropertyEntity({ type: types.listEntity, list: new Array(), required: false, codEntity: ko.observable(0), idGrid: guid() });

    this.Adicionar = PropertyEntity({ eventClick: adicionarClick, type: types.event, text: "Adicionar", visible: ko.observable(true) });
    this.Atualizar = PropertyEntity({ eventClick: atualizarClick, type: types.event, text: "Atualizar", visible: ko.observable(false) });
    this.Excluir = PropertyEntity({ eventClick: excluirClick, type: types.event, text: "Excluir", visible: ko.observable(false) });
    this.Cancelar = PropertyEntity({ eventClick: cancelarClick, type: types.event, text: "Cancelar", visible: ko.observable(false) });
}

//*******EVENTOS*******


function loadPlanoOrcamentario() {
    _planoOrcamentario = new PlanoOrcamentario();
    KoBindings(_planoOrcamentario, "knockoutCadastroPlanoOrcamentario");

    HeaderAuditoria("PlanoOrcamentario", _planoOrcamentario);

    _pesquisaPlanoOrcamentario = new PesquisaPlanoOrcamentario();
    KoBindings(_pesquisaPlanoOrcamentario, "knockoutPesquisaPlanoOrcamentario", false, _pesquisaPlanoOrcamentario.Pesquisar.id);

    $("#" + _planoOrcamentario.Repetir.id).click(HabilitarCamposRepetir);
    $("#" + _planoOrcamentario.Dividir.id).click(HabilitarCamposDividir);

    new BuscarCentroResultado(_planoOrcamentario.CentroResultado);
    new BuscarCentroResultado(_pesquisaPlanoOrcamentario.CentroResultado);

    buscarPlanoOrcamentarios();
    loadContaPlanoOrcamentario();
}

function adicionarClick(e, sender) {
    var valido = ValidarCamposObrigatorios(e);

    if (valido) {
        var percentual = 0;
        var valor = 0;

        if (_planoOrcamentario.ListaConta.list.length > 0) {
            $.each(_planoOrcamentario.ListaConta.list, function (i, listaConta) {
                percentual += Globalize.parseFloat(listaConta.Percentual.val);
                valor += Globalize.parseFloat(listaConta.Valor.val);
            });

            //if (percentual != 100) {
            //    valido = false;
            //    exibirMensagem("atencao", "Contas", "Percentual das contas lançadas não fechou 100%!<br>Está em: " + Globalize.format(percentual, "n2")+"%, favor verificar.");
            //    return;
            //}
            if (valor != Globalize.parseFloat(_planoOrcamentario.Valor.val())) {
                valido = false;
                exibirMensagem("atencao", "Contas", "Valores das contas lançadas não é igual ao valor orçado na primeira aba! Favor verificar.");
                return;
            }
        }
    }

    Salvar(e, "PlanoOrcamentario/Adicionar", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Cadastrado com sucesso");
                _gridPlanoOrcamentario.CarregarGrid();
                limparCamposPlanoOrcamentario();
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    }, sender);
}

function atualizarClick(e, sender) {
    var valido = ValidarCamposObrigatorios(e);

    if (valido) {
        var percentual = 0;
        var valor = 0;

        if (_planoOrcamentario.ListaConta.list.length > 0) {
            $.each(_planoOrcamentario.ListaConta.list, function (i, listaConta) {
                percentual += Globalize.parseFloat(listaConta.Percentual.val);
                valor += Globalize.parseFloat(listaConta.Valor.val);
            });

            //if (percentual != 100) {
            //    valido = false;
            //    exibirMensagem("atencao", "Contas", "Percentual das contas lançadas não fechou 100%!<br>Está em: " + Globalize.format(percentual, "n2") + "%, favor verificar.");
            //    return;
            //} 
            if (valor != Globalize.parseFloat(_planoOrcamentario.Valor.val())) {
                valido = false;
                exibirMensagem("atencao", "Contas", "Valor das contas lançadas não é igual ao valor orçado na primeira aba! Favor verificar.");
                return;
            }
        }
    }

    Salvar(e, "PlanoOrcamentario/Atualizar", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Atualizado com sucesso");
                _gridPlanoOrcamentario.CarregarGrid();
                limparCamposPlanoOrcamentario();
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    }, sender);
}


function excluirClick(e, sender) {
    exibirConfirmacao("Confirmação", "Realmente deseja excluir o Plano Orçamentário " + _planoOrcamentario.Observacao.val() + "?", function () {
        ExcluirPorCodigo(_planoOrcamentario, "PlanoOrcamentario/ExcluirPorCodigo", function (arg) {
            if (arg.Success) {
                if (arg.Data) {
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Excluído com sucesso");
                    _gridPlanoOrcamentario.CarregarGrid();
                    limparCamposPlanoOrcamentario();
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
    limparCamposPlanoOrcamentario();
}

//*******MÉTODOS*******

function buscarPlanoOrcamentarios() {
    var editar = { descricao: "Editar", id: "clasEditar", evento: "onclick", metodo: editarPlanoOrcamentario, tamanho: "15", icone: "" };
    var menuOpcoes = new Object();
    menuOpcoes.tipo = TypeOptionMenu.link;
    menuOpcoes.opcoes = new Array();
    menuOpcoes.opcoes.push(editar);

    _gridPlanoOrcamentario = new GridView(_pesquisaPlanoOrcamentario.Pesquisar.idGrid, "PlanoOrcamentario/Pesquisa", _pesquisaPlanoOrcamentario, menuOpcoes, { column: 0, dir: orderDir.desc });
    _gridPlanoOrcamentario.CarregarGrid();
}

function editarPlanoOrcamentario(PlanoOrcamentarioGrid) {
    limparCamposPlanoOrcamentario();
    _planoOrcamentario.Codigo.val(PlanoOrcamentarioGrid.Codigo);
    BuscarPorCodigo(_planoOrcamentario, "PlanoOrcamentario/BuscarPorCodigo", function (arg) {
        _pesquisaPlanoOrcamentario.ExibirFiltros.visibleFade(false);
        _planoOrcamentario.Atualizar.visible(true);
        _planoOrcamentario.Cancelar.visible(true);
        _planoOrcamentario.Excluir.visible(true);
        _planoOrcamentario.Adicionar.visible(false);

        if (_planoOrcamentario.Dividir.val() == true || _planoOrcamentario.Repetir.val() == true) {
            _planoOrcamentario.Periodicidade.visible(true);
            _planoOrcamentario.NumeroOcorrencia.visible(true);
            _planoOrcamentario.Periodicidade.required = true;
            _planoOrcamentario.NumeroOcorrencia.required = true;
        }
        _planoOrcamentario.Repetir.enable(false);
        _planoOrcamentario.Dividir.enable(false);
        _planoOrcamentario.Periodicidade.enable(false);
        _planoOrcamentario.NumeroOcorrencia.enable(false);

        recarregarGridContaPlanoOrcamentario();
    }, null);
}

function limparCamposPlanoOrcamentario() {
    _planoOrcamentario.Atualizar.visible(false);
    _planoOrcamentario.Cancelar.visible(false);
    _planoOrcamentario.Excluir.visible(false);
    _planoOrcamentario.Adicionar.visible(true);
    LimparCampos(_planoOrcamentario);
    LimparCampos(_contaPlanoOrcamentario);
    recarregarGridContaPlanoOrcamentario();

    _planoOrcamentario.Periodicidade.visible(false);
    _planoOrcamentario.NumeroOcorrencia.visible(false);
    _planoOrcamentario.Periodicidade.required = false;
    _planoOrcamentario.NumeroOcorrencia.required = false;
    _planoOrcamentario.Periodicidade.enable(true);
    _planoOrcamentario.NumeroOcorrencia.enable(true);
    _planoOrcamentario.Repetir.enable(true);
    _planoOrcamentario.Dividir.enable(true);

    resetarTabs();
}

function HabilitarCamposDividir(e, sender) {
    _planoOrcamentario.Repetir.val(false);

    if (_planoOrcamentario.Dividir.val() == false) {
        _planoOrcamentario.Periodicidade.val(EnumPeriodicidade.Mensal);
        _planoOrcamentario.NumeroOcorrencia.val("");

        _planoOrcamentario.Periodicidade.visible(false);
        _planoOrcamentario.NumeroOcorrencia.visible(false);
        _planoOrcamentario.Periodicidade.required = false;
        _planoOrcamentario.NumeroOcorrencia.required = false;
    } else {
        _planoOrcamentario.Periodicidade.visible(true);
        _planoOrcamentario.NumeroOcorrencia.visible(true);
        _planoOrcamentario.Periodicidade.required = true;
        _planoOrcamentario.NumeroOcorrencia.required = true;
    }
}

function HabilitarCamposRepetir(e, sender) {
    _planoOrcamentario.Dividir.val(false);

    if (_planoOrcamentario.Repetir.val() == false) {
        _planoOrcamentario.Periodicidade.val(EnumPeriodicidade.Mensal);
        _planoOrcamentario.NumeroOcorrencia.val("");

        _planoOrcamentario.Periodicidade.visible(false);
        _planoOrcamentario.NumeroOcorrencia.visible(false);
        _planoOrcamentario.Periodicidade.required = false;
        _planoOrcamentario.NumeroOcorrencia.required = false;
    } else {
        _planoOrcamentario.Periodicidade.visible(true);
        _planoOrcamentario.NumeroOcorrencia.visible(true);
        _planoOrcamentario.Periodicidade.required = true;
        _planoOrcamentario.NumeroOcorrencia.required = true;
    }
}

function resetarTabs() {
    $("#myTab a:first").tab("show");
}
