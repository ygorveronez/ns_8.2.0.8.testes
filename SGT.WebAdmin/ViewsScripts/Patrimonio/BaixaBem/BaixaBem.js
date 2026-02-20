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
/// <reference path="../../Consultas/Bem.js" />
/// <reference path="../../Consultas/Usuario.js" />
/// <reference path="../../Consultas/NotaFiscal.js" />
/// <reference path="../../Enumeradores/EnumStatusBem.js" />


//*******MAPEAMENTO KNOUCKOUT*******

var _gridBaixaBem;
var _baixaBem;
var _pesquisaBaixaBem;

var PesquisaBaixaBem = function () {
    this.Bem = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Patrimônio:", idBtnSearch: guid() });
    this.Status = PropertyEntity({ val: ko.observable(EnumStatusBem.Todos), options: EnumStatusBem.obterOpcoesPesquisa(), def: EnumStatusBem.Todos, text: "Status: " });

    this.Pesquisar = PropertyEntity({
        eventClick: function (e) {
            _gridBaixaBem.CarregarGrid();
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

var BaixaBem = function () {
    this.Codigo = PropertyEntity({ text: "Código: ", val: ko.observable(0), def: 0, getType: typesKnockout.int, enable: ko.observable(false) });

    this.Data = PropertyEntity({ text: "*Data: ", getType: typesKnockout.date, required: ko.observable(true) });
    this.Status = PropertyEntity({ text: "*Status: ", val: ko.observable(EnumStatusBem.Aberto), options: EnumStatusBem.obterOpcoes(), def: EnumStatusBem.Aberto, required: true, enable: ko.observable(true) });
    this.ValorVenda = PropertyEntity({ text: "*Valor Venda:", getType: typesKnockout.decimal, maxlength: 15, val: ko.observable(""), def: "", configDecimal: { precision: 2, allowZero: false, allowNegative: false }, required: ko.observable(true) });

    this.Bem = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "*Patrimônio:", idBtnSearch: guid(), required: ko.observable(true), visible: ko.observable(true) });
    this.NotaFiscal = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Nota Fiscal de Saída:", idBtnSearch: guid(), required: ko.observable(false), visible: ko.observable(true) });
    this.Funcionario = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "*Funcionário Reponsável:", idBtnSearch: guid(), required: ko.observable(true), visible: ko.observable(true) });
};

var CRUDBaixaBem = function () {
    this.Adicionar = PropertyEntity({ eventClick: adicionarClick, type: types.event, text: "Adicionar", visible: ko.observable(true) });
    this.Atualizar = PropertyEntity({ eventClick: atualizarClick, type: types.event, text: "Atualizar", visible: ko.observable(false) });
    this.Excluir = PropertyEntity({ eventClick: excluirClick, type: types.event, text: "Excluir", visible: ko.observable(false) });
    this.Cancelar = PropertyEntity({ eventClick: cancelarClick, type: types.event, text: "Cancelar", visible: ko.observable(false) });
    this.TermoBaixa = PropertyEntity({ eventClick: termoBaixaClick, type: types.event, text: "Termo de Baixa", visible: ko.observable(false) });
};

//*******EVENTOS*******


function loadBaixaBem() {
    _baixaBem = new BaixaBem();
    KoBindings(_baixaBem, "knockoutCadastroBaixaBem");

    HeaderAuditoria("BaixaBem", _baixaBem);

    _crudBaixaBem = new CRUDBaixaBem();
    KoBindings(_crudBaixaBem, "knockoutCRUDBaixaBem");

    _pesquisaBaixaBem = new PesquisaBaixaBem();
    KoBindings(_pesquisaBaixaBem, "knockoutPesquisaBaixaBem", false, _pesquisaBaixaBem.Pesquisar.id);

    new BuscarBens(_pesquisaBaixaBem.Bem);
    new BuscarBens(_baixaBem.Bem);
    new BuscarFuncionario(_baixaBem.Funcionario);
    new BuscarNotaFiscal(_baixaBem.NotaFiscal);

    buscarBaixaBens();
}

function adicionarClick(e, sender) {
    Salvar(_baixaBem, "BaixaBem/Adicionar", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Cadastrado com sucesso");
                _gridBaixaBem.CarregarGrid();
                limparCamposBaixaBem();
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    }, sender);
}

function atualizarClick(e, sender) {
    Salvar(_baixaBem, "BaixaBem/Atualizar", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Atualizado com sucesso");
                _gridBaixaBem.CarregarGrid();
                limparCamposBaixaBem();
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "falha", arg.Msg);
        }
    }, sender);
}

function excluirClick(e, sender) {
    exibirConfirmacao("Confirmação", "Realmente deseja excluir a Baixa do Patrimônio?", function () {
        ExcluirPorCodigo(_baixaBem, "BaixaBem/ExcluirPorCodigo", function (arg) {
            if (arg.Success) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Excluído com sucesso");
                _gridBaixaBem.CarregarGrid();
                limparCamposBaixaBem();
            } else {
                exibirMensagem(tipoMensagem.falha, "falha", arg.Msg);
            }
        }, null);
    });
}

function cancelarClick(e) {
    limparCamposBaixaBem();
}

function termoBaixaClick(e, sender) {
    var data = { CodigoBaixa: _baixaBem.Codigo.val() };
    executarReST("RelatoriosBem/BaixarRelatorioTermoBaixaMaterial", data, function (arg) {
        if (arg.Success) {
            if (arg.Data !== false) {
                BuscarProcessamentosPendentes();
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Aguarde que seu relatório está sendo gerado.");
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    });
}

//*******MÉTODOS*******


function buscarBaixaBens() {
    var editar = { descricao: "Editar", id: "clasEditar", evento: "onclick", metodo: editarBaixaBem, tamanho: "15", icone: "" };
    var menuOpcoes = new Object();
    menuOpcoes.tipo = TypeOptionMenu.link;
    menuOpcoes.opcoes = new Array();
    menuOpcoes.opcoes.push(editar);

    _gridBaixaBem = new GridView(_pesquisaBaixaBem.Pesquisar.idGrid, "BaixaBem/Pesquisa", _pesquisaBaixaBem, menuOpcoes, null);
    _gridBaixaBem.CarregarGrid();
}

function editarBaixaBem(baixaBemGrid) {
    limparCamposBaixaBem();
    _baixaBem.Codigo.val(baixaBemGrid.Codigo);
    BuscarPorCodigo(_baixaBem, "BaixaBem/BuscarPorCodigo", function (arg) {
        _pesquisaBaixaBem.ExibirFiltros.visibleFade(false);
        _crudBaixaBem.Atualizar.visible(true);
        _crudBaixaBem.Cancelar.visible(true);
        _crudBaixaBem.Excluir.visible(true);
        _crudBaixaBem.Adicionar.visible(false);
        _crudBaixaBem.TermoBaixa.visible(true);
    }, null);
}

function limparCamposBaixaBem() {
    _crudBaixaBem.Atualizar.visible(false);
    _crudBaixaBem.Cancelar.visible(false);
    _crudBaixaBem.Excluir.visible(false);
    _crudBaixaBem.Adicionar.visible(true);
    _crudBaixaBem.TermoBaixa.visible(false);
    LimparCampos(_baixaBem);
}