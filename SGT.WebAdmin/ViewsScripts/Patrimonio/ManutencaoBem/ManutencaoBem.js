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
/// <reference path="../../Consultas/Cliente.js" />
/// <reference path="../../Consultas/NotaFiscal.js" />
/// <reference path="../../Consultas/DocumentoEntrada.js" />
/// <reference path="../../Enumeradores/EnumStatusBem.js" />
/// <reference path="../../Consultas/motivodefeito.js" />


//*******MAPEAMENTO KNOUCKOUT*******

var _gridManutencaoBem;
var _manutencaoBem;
var _pesquisaManutencaoBem;

var PesquisaManutencaoBem = function () {
    this.Bem = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Patrimônio:", idBtnSearch: guid() });
    this.MotivoDefeito = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Motivo do Defeito:", idBtnSearch: guid() });
    this.Status = PropertyEntity({ val: ko.observable(EnumStatusBem.Todos), options: EnumStatusBem.obterOpcoesPesquisa(), def: EnumStatusBem.Todos, text: "Status: " });
    this.DataEntrega = PropertyEntity({ text: "Data Entrega: ", getType: typesKnockout.date });
    this.DataRetorno = PropertyEntity({ text: "Data Retorno: ", getType: typesKnockout.date });

    this.Pesquisar = PropertyEntity({
        eventClick: function (e) {
            _gridManutencaoBem.CarregarGrid();
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

var ManutencaoBem = function () {
    this.Codigo = PropertyEntity({ text: "Código: ", val: ko.observable(0), def: 0, getType: typesKnockout.int, enable: ko.observable(false) });

    this.DataEntrega = PropertyEntity({ text: "Data Entrega: ", getType: typesKnockout.date, required: ko.observable(false) });
    this.DataGarantia = PropertyEntity({ text: "Data Garantia: ", getType: typesKnockout.date, required: ko.observable(false) });
    this.Status = PropertyEntity({ text: "*Status: ", val: ko.observable(EnumStatusBem.Aberto), options: EnumStatusBem.obterOpcoes(), def: EnumStatusBem.Aberto, required: true, enable: ko.observable(true) });
    this.ObservacaoSaida = PropertyEntity({ text: "Observação Saída:", maxlength: 5000, val: ko.observable("") });
    this.ObservacaoRetorno = PropertyEntity({ text: "Observação Retorno:", maxlength: 5000, val: ko.observable("") });
    this.ValorOrcado = PropertyEntity({ text: "Valor Orçado:", getType: typesKnockout.decimal, maxlength: 15, val: ko.observable("0,00"), def: "0,00", configDecimal: { precision: 2, allowZero: true, allowNegative: false } });
    this.ValorPago = PropertyEntity({ text: "Valor Pago:", getType: typesKnockout.decimal, maxlength: 15, val: ko.observable("0,00"), def: "0,00", configDecimal: { precision: 2, allowZero: true, allowNegative: false } });
    this.DataRetorno = PropertyEntity({ text: "Data Retorno: ", getType: typesKnockout.date, required: ko.observable(false) });

    this.Bem = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "*Patrimônio:", idBtnSearch: guid(), required: ko.observable(true), visible: ko.observable(true) });
    this.Pessoa = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Pessoa:", idBtnSearch: guid(), required: ko.observable(false), visible: ko.observable(true) });
    this.NotaFiscal = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Nota Fiscal de Saída:", idBtnSearch: guid(), required: ko.observable(false), visible: ko.observable(true) });
    this.DocumentoEntrada = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Documento de Entrada:", idBtnSearch: guid(), required: ko.observable(false), visible: ko.observable(true) });
    this.Defeito = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Defeito:", idBtnSearch: guid(), required: ko.observable(false), visible: ko.observable(true) });
};

var CRUDManutencaoBem = function () {
    this.Adicionar = PropertyEntity({ eventClick: adicionarClick, type: types.event, text: "Adicionar", visible: ko.observable(true) });
    this.Atualizar = PropertyEntity({ eventClick: atualizarClick, type: types.event, text: "Atualizar", visible: ko.observable(false) });
    this.Excluir = PropertyEntity({ eventClick: excluirClick, type: types.event, text: "Excluir", visible: ko.observable(false) });
    this.Cancelar = PropertyEntity({ eventClick: cancelarClick, type: types.event, text: "Cancelar", visible: ko.observable(false) });
};

//*******EVENTOS*******


function loadManutencaoBem() {
    _manutencaoBem = new ManutencaoBem();
    KoBindings(_manutencaoBem, "knockoutCadastroManutencaoBem");

    HeaderAuditoria("ManutencaoBem", _manutencaoBem);

    _crudManutencaoBem = new CRUDManutencaoBem();
    KoBindings(_crudManutencaoBem, "knockoutCRUDManutencaoBem");

    _pesquisaManutencaoBem = new PesquisaManutencaoBem();
    KoBindings(_pesquisaManutencaoBem, "knockoutPesquisaManutencaoBem", false, _pesquisaManutencaoBem.Pesquisar.id);

    new BuscarBens(_pesquisaManutencaoBem.Bem);
    new BuscarBens(_manutencaoBem.Bem);
    new BuscarClientes(_manutencaoBem.Pessoa);
    new BuscarNotaFiscal(_manutencaoBem.NotaFiscal);
    new BuscarDocumentoEntrada(_manutencaoBem.DocumentoEntrada, RetornoDocumentoEntrada);
    new BuscarMotivoDefeito(_manutencaoBem.Defeito);
    new BuscarMotivoDefeito(_pesquisaManutencaoBem.MotivoDefeito);

    buscarManutencaoBens();
}

function RetornoDocumentoEntrada(data) {
    _manutencaoBem.DocumentoEntrada.codEntity(data.Codigo);
    _manutencaoBem.DocumentoEntrada.val(data.Numero);
}

function adicionarClick(e, sender) {
    Salvar(_manutencaoBem, "ManutencaoBem/Adicionar", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Cadastrado com sucesso");
                _gridManutencaoBem.CarregarGrid();
                limparCamposManutencaoBem();
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    }, sender);
}

function atualizarClick(e, sender) {
    Salvar(_manutencaoBem, "ManutencaoBem/Atualizar", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Atualizado com sucesso");
                _gridManutencaoBem.CarregarGrid();
                limparCamposManutencaoBem();
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "falha", arg.Msg);
        }
    }, sender);
}


function excluirClick(e, sender) {
    exibirConfirmacao("Confirmação", "Realmente deseja excluir a Manutenção do Patrimônio?", function () {
        ExcluirPorCodigo(_manutencaoBem, "ManutencaoBem/ExcluirPorCodigo", function (arg) {
            if (arg.Success) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Excluído com sucesso");
                _gridManutencaoBem.CarregarGrid();
                limparCamposManutencaoBem();
            } else {
                exibirMensagem(tipoMensagem.falha, "falha", arg.Msg);
            }
        }, null);
    });
}

function cancelarClick(e) {
    limparCamposManutencaoBem();
}

//*******MÉTODOS*******


function buscarManutencaoBens() {
    var editar = { descricao: "Editar", id: "clasEditar", evento: "onclick", metodo: editarManutencaoBem, tamanho: "15", icone: "" };
    var menuOpcoes = new Object();
    menuOpcoes.tipo = TypeOptionMenu.link;
    menuOpcoes.opcoes = new Array();
    menuOpcoes.opcoes.push(editar);

    _gridManutencaoBem = new GridView(_pesquisaManutencaoBem.Pesquisar.idGrid, "ManutencaoBem/Pesquisa", _pesquisaManutencaoBem, menuOpcoes, null);
    _gridManutencaoBem.CarregarGrid();
}

function editarManutencaoBem(manutencaoBemGrid) {
    limparCamposManutencaoBem();
    _manutencaoBem.Codigo.val(manutencaoBemGrid.Codigo);
    BuscarPorCodigo(_manutencaoBem, "ManutencaoBem/BuscarPorCodigo", function (arg) {
        _pesquisaManutencaoBem.ExibirFiltros.visibleFade(false);
        _crudManutencaoBem.Atualizar.visible(true);
        _crudManutencaoBem.Cancelar.visible(true);
        _crudManutencaoBem.Excluir.visible(true);
        _crudManutencaoBem.Adicionar.visible(false);
    }, null);
}

function limparCamposManutencaoBem() {
    _crudManutencaoBem.Atualizar.visible(false);
    _crudManutencaoBem.Cancelar.visible(false);
    _crudManutencaoBem.Excluir.visible(false);
    _crudManutencaoBem.Adicionar.visible(true);
    LimparCampos(_manutencaoBem);
}