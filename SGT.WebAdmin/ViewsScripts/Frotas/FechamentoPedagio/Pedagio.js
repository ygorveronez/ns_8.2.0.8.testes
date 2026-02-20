/// <reference path="../../Enumeradores/EnumSituacaoFechamentoPedagio.js" />
/// <reference path="../../../js/libs/jquery-2.1.1.js" />
/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/knockout-3.1.0.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/bootstrap/bootstrap.js" />
/// <reference path="../../../js/libs/jquery.blockui.js" />
/// <reference path="../../../js/Global/knoutViewsSlides.js" />
/// <reference path="../../Consultas/Cliente.js" />
/// <reference path="../../Consultas/Veiculo.js" />
/// <reference path="../../Consultas/FechamentoPedagio.js" />
/// <reference path="../../Enumeradores/EnumModalidadePessoa.js" />
/// <reference path="../../Enumeradores/EnumSituacaoFechamentoPedagio.js" />
/// <reference path="../../Consultas/Motorista.js" />
/// <reference path="../../../js/libs/jquery.maskMoney.js" />
/// <reference path="../../../js/libs/jquery.globalize.js" />
/// <reference path="../../../js/libs/jquery.globalize.pt-BR.js" />
/// <reference path="../../Consultas/Produto.js" />
/// <reference path="../../../js/plugin/datatables/jquery.dataTables.js" />
/// <reference path="FechamentoPedagio.js" />

//*******MAPEAMENTO KNOUCKOUT*******


var _listaPedagio;
var _gridPedagios;
var _pedagio;

var ListaPedagio = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });

    this.Pedagios = PropertyEntity({ idGrid: guid(), visible: ko.observable(true) });
}

var Pedagio = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Data = PropertyEntity({ text: "*Data Passagem:", required: true, getType: typesKnockout.dateTime });
    this.Veiculo = PropertyEntity({ text: "*Veículo:", textAviso: ko.observable(""), type: types.entity, codEntity: ko.observable(0), required: true, idBtnSearch: guid(), issue: 143 });
    this.Valor = PropertyEntity({ text: "*Valor:", required: true, getType: typesKnockout.decimal, configDecimal: { precision: 2, allowZero: false }, val: ko.observable(0.00), maxlength: 10 });
    this.Praca = PropertyEntity({ text: "Praça:", idBtnSearch: guid(), maxlength: 150 });
    this.Rodovia = PropertyEntity({ text: "Rodovia:", idBtnSearch: guid(), maxlength: 150 });
    this.TipoMovimento = PropertyEntity({ text: "*Movimento Financeiro:", type: types.entity, codEntity: ko.observable(0), required: ko.observable(false), idBtnSearch: guid(), visible: ko.observable(false), issue: 364 });

    this.AdicionaAoFechamento = PropertyEntity({ val: ko.observable(true), def: true, getType: typesKnockout.bool, type: types.map });

    this.Atualizar = PropertyEntity({ eventClick: atualizarPedagioClick, type: types.event, text: "Atualizar", visible: ko.observable(true) });
    this.Cancelar = PropertyEntity({ eventClick: cancelarPedagioClick, type: types.event, text: "Cancelar", visible: ko.observable(true) });
}

//*******EVENTOS*******

function loadPedagio() {
    // Pedagio (editar)
    _pedagio = new Pedagio();
    KoBindings(_pedagio, "knockoutCadastroPedagio");

    // Pedagio (grid)
    _listaPedagio = new ListaPedagio();
    KoBindings(_listaPedagio, "knockoutFechamentoPedagio");

    buscarPedagios();
}

function buscarPedagios() {
    var editar = { descricao: "Editar", id: guid(), metodo: editarPedagio, icone: "", visibilidade: visibleEditar };
    var removerDoFechamento = { descricao: "Remover", id: guid(), metodo: removerDoFechamentoClick, icone: "", visibilidade: visibleRemover };
    var adicionarAoFechamento = { descricao: "Adicionar", id: guid(), metodo: adicionarAoFechamentoClick, icone: "", visibilidade: visibleAdicionar };

    var menuOpcoes = { tipo: TypeOptionMenu.list, descricao: "Opções", tamanho: 5, opcoes: [editar, removerDoFechamento, adicionarAoFechamento] };

    var editarColuna = { permite: true, callback: callbackEditarColuna, atualizarRow: false };
    _gridPedagios = new GridView(_listaPedagio.Pedagios.idGrid, "FechamentoPedagio/BuscarPedagios", _listaPedagio, menuOpcoes, null, 50, null, null, null, null, null, editarColuna);
}

function visibleEditar(dataRow ){
    return EnumSituacaoFechamentoPedagio.Pendente == _fechamentoPedagio.Situacao.val();
}

function visibleAdicionar(dataRow) {
    return EnumSituacaoFechamentoPedagio.Pendente == _fechamentoPedagio.Situacao.val() && !dataRow.DT_Enable;
}

function visibleRemover(dataRow) {
    return EnumSituacaoFechamentoPedagio.Pendente == _fechamentoPedagio.Situacao.val() && dataRow.DT_Enable;
}

function cancelarPedagioClick() {
    LimparCampos(_pedagio);
    Global.fecharModal('divModalPedagio');
}

function atualizarPedagioClick(e, sender) {
   /* _pedagio.DataInicio.val(_fechamentoPedagio.DataInicio.val());
    _pedagio.DataFim.val(_fechamentoPedagio.DataFim.val());
    _pedagio.VeiculoFechamento.val(_fechamentoPedagio.Veiculo.val());*/
    Salvar(_pedagio, "FechamentoPedagio/AtualizarPedagio", function (arg) {
        if (arg.Success) {
            if (arg.Data !== false) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Atualizado com sucesso");
                LimparCampos(_pedagio);
                Global.fecharModal('divModalPedagio');
                CompararEAtualizarGridEditableDataRow(_globalDataRow, arg.Data)
                _gridPedagios.AtualizarDataRow(_globalRow, _globalDataRow);
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    }, sender, exibirCamposObrigatorio);
}



//*******MÉTODOS*******

function carregarPedagios(callback) {
    _listaPedagio.Codigo.val(_fechamentoPedagio.Codigo.val());
    _gridPedagios.CarregarGrid(callback);
}

function editarPedagio(dataRow, row) {
    /*// So permite edicao quando o status do fechamento e pendente
    if (EnumSituacaoFechamentoPedagio.Pendente != _fechamentoPedagio.Situacao.val()) {
        exibirMensagem(tipoMensagem.aviso, "Aviso", "Não é possível editar pedágios de um fechamento finalizado.");
        return;
    }*/

    // Exibe modal para editar pedágio
    LimparCampos(_pedagio);

    _pedagio.Codigo.val(dataRow.Codigo);
    _pedagio.AdicionaAoFechamento.val(dataRow.DT_Enable ? true : false);

    BuscarPorCodigo(_pedagio, "Pedagio/BuscarPorCodigo", function (arg) {
        _globalDataRow = dataRow;
        _globalRow = row;        
        Global.abrirModal('divModalPedagio');
    }, null);
}

function adicionarAoFechamentoClick(dataRow, row) {
    /*// So permite adicao quando o status do fechamento e pendente
    if (EnumSituacaoFechamentoPedagio.Pendente != _fechamentoPedagio.Situacao.val()) {
        exibirMensagem(tipoMensagem.aviso, "Aviso", "Não é possível adicionar pedágios em um fechamento finalizado.");
        return;
    }*/

    // Adiciona pedagio
    var Dados = {
        Codigo: dataRow.Codigo,
        Fechamento: _fechamentoPedagio.Codigo.val()
    };

    executarReST("FechamentoPedagio/AdicionarAoFechamento", Dados, function (args) {
        dataRow.DT_Enable = true;
        _gridPedagios.AtualizarDataRow(row, dataRow);
    });
}

function removerDoFechamentoClick(dataRow, row) {
    /*// So permite remocao quando o status do fechamento e pendente
    if (EnumSituacaoFechamentoPedagio.Pendente != _fechamentoPedagio.Situacao.val()) {
        exibirMensagem(tipoMensagem.aviso, "Aviso", "Não é possível remover pedágios de um fechamento finalizado.");
        return;
    }*/

    // Remove pedagio
    var Dados = {
        Codigo: dataRow.Codigo,
        Fechamento: _fechamentoPedagio.Codigo.val()
    };

    executarReST("FechamentoPedagio/RemoveDoFechamento", Dados, function (args) {
        dataRow.DT_Enable = false;
        _gridPedagios.AtualizarDataRow(row, dataRow);
    });
}

function LimparDetalhesPedagios() {
    //SetarPercentualProcessamento(0);
    $("#liPedagios").hide();
}

function callbackEditarColuna(dataRow, row, head, callbackTabPress) {
    var data = {
        Codigo: dataRow.Codigo,
        Valor: dataRow.Valor,
        AdicionaAoFechamento: dataRow.DT_Enable ? true : false
    };

    executarReST("FechamentoPedagio/AlterarValorPedagio", data, function (arg) {
        if (arg.Success) {
            if (arg.Data !== false) {
                CompararEAtualizarGridEditableDataRow(dataRow, arg.Data)
                _gridPedagios.AtualizarDataRow(row, dataRow, callbackTabPress);
            } else {
                ExibirErroDataRow(row, arg.Msg, tipoMensagem.aviso, "Aviso");
            }
        } else {
            ExibirErroDataRow(row, arg.Msg, tipoMensagem.falha, "Falha");
        }
    });
}

function ExibirErroDataRow(row, mensagem, tipoMensagem, titulo) {
    _gridPedagios.DesfazerAlteracaoDataRow(row);
    exibirMensagem(tipoMensagem, titulo, mensagem);
}

function exibirCamposObrigatorio() {
    exibirMensagem("atencao", "Campo Obrigatório", "Por Favor, Informe os campos obrigatórios");
}