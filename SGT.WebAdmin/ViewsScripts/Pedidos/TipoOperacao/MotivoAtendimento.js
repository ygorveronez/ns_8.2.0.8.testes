/// <reference path="../../Consultas/MotivoChamado.js" />
/// <reference path="TipoOperacao.js" />

var _gridMotivoAtendimento;
var _gridMotivoAtendimentoTransportador;

function loadMotivoAtendimento() {
    var excluir = { descricao: Localization.Resources.Gerais.Geral.Excluir, id: guid(), evento: "onclick", metodo: function (data) { excluirMotivoAtendimentoTransportador(_gridMotivoAtendimento, data) }, tamanho: "20", icone: "" };
    var menuOpcoes = { tipo: TypeOptionMenu.link, opcoes: [excluir], tamanho: 5 };

    var header = [
        { data: "Codigo", visible: false },
        { data: "Descricao", title: Localization.Resources.Gerais.Geral.Descricao, width: "80%" },
    ];
    _gridMotivoAtendimento = new BasicDataTable(_tipoOperacao.MotivosAtendimento.id, header, menuOpcoes, { column: 1, dir: orderDir.asc });
    _gridMotivoAtendimento.CarregarGrid([]);
    _tipoOperacao.MotivosAtendimento.BasicDataTable = _gridMotivoAtendimento;

    new BuscarMotivoChamado(_tipoOperacao.AdicionarMotivosAtendimento, null, _gridMotivoAtendimento);
}

function loadGridMotivoAtendimentoTransportador() {
    var excluir = { descricao: Localization.Resources.Gerais.Geral.Excluir, id: guid(), evento: "onclick", metodo: function (data) { excluirMotivoAtendimentoTransportador(_gridMotivoAtendimentoTransportador, data) }, tamanho: "20", icone: "" };
    var menuOpcoes = { tipo: TypeOptionMenu.link, opcoes: [excluir], tamanho: 5 };

    var header = [
        { data: "Codigo", visible: false },
        { data: "Descricao", title: Localization.Resources.Gerais.Geral.Descricao, width: "80%" },
    ];
    _gridMotivoAtendimentoTransportador = new BasicDataTable(_tipoOperacao.TransportadoresMotivoAtendimento.id, header, menuOpcoes, { column: 1, dir: orderDir.asc });
    _gridMotivoAtendimentoTransportador.CarregarGrid([]);
    _tipoOperacao.TransportadoresMotivoAtendimento.BasicDataTable = _gridMotivoAtendimentoTransportador;

    BuscarTransportadores(_tipoOperacao.AdicionarTransportador, null, null, null, _gridMotivoAtendimentoTransportador);
}

function obterMotivosAtendimento() {
    var listaMotivosAtendimento = _tipoOperacao.MotivosAtendimento.BasicDataTable.BuscarRegistros();
    var listaMotivosAtendimentoRetornar = new Array();

    for (var i = 0; i < listaMotivosAtendimento.length; i++) {
        listaMotivosAtendimentoRetornar.push({
            Codigo: listaMotivosAtendimento[i].Codigo
        });
    }

    return JSON.stringify(listaMotivosAtendimentoRetornar);
}

function obterMotivoAtendimentoTransportadores() {
    let listaMotivosAtendimentoTransportadores = _tipoOperacao.TransportadoresMotivoAtendimento.BasicDataTable.BuscarRegistros();
    let listaMotivosAtendimentoTransportadoresRetornar = new Array();

    for (var i = 0; i < listaMotivosAtendimentoTransportadores.length; i++) {
        listaMotivosAtendimentoTransportadoresRetornar.push({
            Codigo: listaMotivosAtendimentoTransportadores[i].Codigo
        });
    }

    return JSON.stringify(listaMotivosAtendimentoTransportadoresRetornar);
}

function excluirMotivoAtendimentoTransportador(knout, data) {
    var dados = knout.BuscarRegistros();

    for (var i = 0; i < dados.length; i++) {
        if (data.Codigo == dados[i].Codigo) {
            dados.splice(i, 1);
            break;
        }
    }

    knout.CarregarGrid(dados);
}

function preencherListaMotivosAtendimento(data) {
    _gridMotivoAtendimento.CarregarGrid(data.ConfiguracaoMotivosChamados);
}

function preencherListaMotivoAtendimentoTransportador(data) {
    _gridMotivoAtendimentoTransportador.CarregarGrid(data.ConfiguracaoChamadoTransportador);
}
function limparListaMotivosAtendimentoTransportador() {
    _gridMotivoAtendimentoTransportador.CarregarGrid([]);
}

function limparListaMotivosAtendimento() {
    _gridMotivoAtendimento.CarregarGrid([]);
}