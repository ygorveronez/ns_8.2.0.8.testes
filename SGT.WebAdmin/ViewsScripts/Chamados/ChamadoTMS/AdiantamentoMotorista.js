/// <reference path="AutorizacaoCliente.js" />
/// <reference path="AnexosAdiantamentoMotorista.js" />
/// <reference path="../../Consultas/PagamentoMotoristaTipo.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _adiantamentoMotorista;
var _gridAdiantamentoMotorista;

var AdiantamentoMotorista = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });

    this.PagamentoMotoristaTipo = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "*Tipo do Pagamento:", idBtnSearch: guid(), required: true, enable: ko.observable(true), visible: ko.observable(true) });
    this.DataPagamento = PropertyEntity({ text: "*Data Pagamento:", getType: typesKnockout.date, required: ko.observable(true), enable: ko.observable(true), visible: ko.observable(true) });
    this.Valor = PropertyEntity({ text: "*Valor:", def: "", val: ko.observable(""), getType: typesKnockout.decimal, maxlength: 15, required: true, visible: ko.observable(true), enable: ko.observable(true), configDecimal: { precision: 2, allowZero: false } });
    this.Observacao = PropertyEntity({ val: ko.observable(""), def: "", text: "Observação:", enable: ko.observable(true) });

    this.Adicionar = PropertyEntity({ eventClick: AdicionarAdiantamentoMotoristaClick, type: types.event, text: "Adicionar", visible: ko.observable(true), enable: ko.observable(true) });
    this.Anexo = PropertyEntity({ eventClick: gerenciarAnexosAdiantamentoMotoristaClick, type: types.event, text: "Adicionar Anexos", visible: ko.observable(true), enable: ko.observable(true) });

    this.Grid = PropertyEntity({ type: types.local, id: guid() });
};

//*******EVENTOS*******

function loadAdiantamentoMotorista() {
    _adiantamentoMotorista = new AdiantamentoMotorista();
    KoBindings(_adiantamentoMotorista, "tabAdiantamentoMotorista");

    new BuscarPagamentoMotoristaTipo(_adiantamentoMotorista.PagamentoMotoristaTipo);

    var excluir = { descricao: "Excluir", id: guid(), metodo: ExcluirAdiantamentoMotoristaClick, icone: "" };

    var menuOpcoes = { tipo: TypeOptionMenu.link, tamanho: 7, opcoes: [excluir] };

    var header = [
        { data: "Codigo", visible: false },
        { data: "PagamentoMotoristaTipo", title: "Tipo do Pagamento", width: "45%" },
        { data: "DataPagamento", title: "Data Pagamento", width: "15%" },
        { data: "Valor", title: "Valor", width: "15%" },
        { data: "Observacao", title: "Observação", width: "15%" }
    ];

    _gridAdiantamentoMotorista = new BasicDataTable(_adiantamentoMotorista.Grid.id, header, menuOpcoes);

    recarregarGridAdiantamentosMotorista();

    loadAnexosAdiantamentoMotorista();
}

function ExcluirAdiantamentoMotoristaClick(data) {
    exibirConfirmacao("Confirmação", "Realmente deseja excluir o adiantamento do motorista?", function () {
        $.each(_autorizacaoCliente.AdiantamentosMotorista.list, function (i, lista) {
            if (data.Codigo == lista.Codigo.val) {
                _autorizacaoCliente.AdiantamentosMotorista.list.splice(i, 1);
                return false;
            }
        });

        recarregarGridAdiantamentosMotorista();
    });
}

function AdicionarAdiantamentoMotoristaClick() {
    var valido = ValidarCamposObrigatorios(_adiantamentoMotorista);
    if (valido) {
        _adiantamentoMotorista.Codigo.val(guid());
        _autorizacaoCliente.AdiantamentosMotorista.list.push(SalvarListEntity(_adiantamentoMotorista));

        limparCamposAdiantamentoMotorista();
        $("#" + _adiantamentoMotorista.PagamentoMotoristaTipo.id).focus();
    } else
        exibirMensagem(tipoMensagem.atencao, "Campos Obrigatórios", "Informe os campos obrigatórios!");
}

//*******MÉTODOS*******

function recarregarGridAdiantamentosMotorista() {

    var data = new Array();

    $.each(_autorizacaoCliente.AdiantamentosMotorista.list, function (i, lista) {
        var listaGrid = new Object();

        listaGrid.Codigo = lista.Codigo.val;
        listaGrid.PagamentoMotoristaTipo = lista.PagamentoMotoristaTipo.val;
        listaGrid.DataPagamento = lista.DataPagamento.val;
        listaGrid.Valor = lista.Valor.val;
        listaGrid.Observacao = lista.Observacao.val;

        data.push(listaGrid);
    });

    _gridAdiantamentoMotorista.CarregarGrid(data);
}

function ControleCamposAdiantamentoMotorista(status) {
    SetarEnableCamposKnockout(_adiantamentoMotorista, status);

    _adiantamentoMotorista.Anexo.enable(true);

    if (!status)
        _gridAdiantamentoMotorista.DesabilitarOpcoes();
    else
        _gridAdiantamentoMotorista.HabilitarOpcoes();
}

function limparCamposAdiantamentoMotorista() {
    LimparCampos(_adiantamentoMotorista);
    recarregarGridAdiantamentosMotorista();
    limparOcorrenciaAnexosAdiantamentoMotorista();
}