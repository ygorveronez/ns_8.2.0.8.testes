var _retornoSenhasAgendamento;
var _retornoFinalizarAgendamento;
var _gridRetornoSenhasAgendamento;
var _gridRetornoFinalizarAgendamento;

var RetornoSenhasAgendamento = function () {
    this.Retornos = PropertyEntity({ type: types.listEntity, list: new Array(), visibleFade: ko.observable(false), required: false, codEntity: ko.observable(0), idGrid: guid() });
}

var RetornoFinalizarAgendamento = function () {
    this.Retornos = PropertyEntity({ type: types.listEntity, list: new Array(), visibleFade: ko.observable(false), required: false, codEntity: ko.observable(0), idGrid: guid() });
}

function loadSenhasAgendamento() {
    _retornoSenhasAgendamento = new RetornoSenhasAgendamento();
    KoBindings(_retornoSenhasAgendamento, "knockoutRetornoSenhasAgendamento");

    _retornoFinalizarAgendamento = new RetornoFinalizarAgendamento();
    KoBindings(_retornoFinalizarAgendamento, "knockoutRetornoFinalizarAgendamento");
    
    loadGridRetornoSenhasAgendamento();
    loadGridRetornoFinalizarAgendamento();
}

function loadGridRetornoSenhasAgendamento() {
    var header = [
        { data: "Pedido", title: "Pedido", width: "33%" },
        { data: "Mensagem", title: "Mensagem", width: "33%" },
        { data: "Senha", title: "Senha", width: "33%" }
    ];

    _gridRetornoSenhasAgendamento = new BasicDataTable(_retornoSenhasAgendamento.Retornos.idGrid, header, null, null, null, 15);
    carregarGridRetornoSenhasAgendamento([]);
}

function loadGridRetornoFinalizarAgendamento() {
    var menuOpcoes = { tipo: TypeOptionMenu.link, tamanho: 7, opcoes: [{ descricao: "Detalhes", id: guid(), metodo: DetalhesFinalizacaoAgendamento }] };
    
    var header = [
        { data: "Pedidos", visible: false },
        { data: "NumeroAgenda", title: "Número Agenda", width: "33%" },
        { data: "NumeroPedidos", title: "Quantidade Pedidos", width: "33%" },
        { data: "Mensagem", title: "Mensagem", width: "33%" },
    ];
    
    _gridRetornoFinalizarAgendamento = new BasicDataTable(_retornoFinalizarAgendamento.Retornos.idGrid, header, menuOpcoes, null, null, 15);
    carregarGridRetornoFinalizarAgendamento([]);
}

function carregarGridRetornoSenhasAgendamento(data) {
    _gridRetornoSenhasAgendamento.CarregarGrid(data);
}

function carregarGridRetornoFinalizarAgendamento(data) {
    _gridRetornoFinalizarAgendamento.CarregarGrid(data);
}

function DetalhesFinalizacaoAgendamento(registroSelecionado) {
    carregarGridRetornoSenhasAgendamento(registroSelecionado.Pedidos);
    
    Global.abrirModal('divModalRetornoSenhasAgendamento');
}