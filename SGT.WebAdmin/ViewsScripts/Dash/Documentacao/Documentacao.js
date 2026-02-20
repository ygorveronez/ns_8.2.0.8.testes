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
/// <reference path="../../Enumeradores/EnumTipoContatoAtendimento.js" />

var _documentacao, _pesquisaDetalhesModalDash, _primeiraPesquisa;

var Documentacao = function () {
    this.DadosDash = PropertyEntity(ko.observableArray([]));
    this.Embarque = PropertyEntity({ val: ko.observable(""), options: ko.observableArray([]), def: 0, text: "Embarque:" });
    this.DadosDetalhesDash = PropertyEntity(ko.observableArray([]));
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Regiao = PropertyEntity({ val: ko.observable(EnumRegiao.Sudeste), def: EnumRegiao.Sudeste, options: ko.observable(EnumRegiao.obterOpcoes()), text: "Região:" });
    this.DataInicial = PropertyEntity({ text: "De:", getType: typesKnockout.date, val: ko.observable("") });
    this.DataFinal = PropertyEntity({ text: "Até:", getType: typesKnockout.date, val: ko.observable(""), def: "", enable: ko.observable(false) });
    this.QuantidadeNavioFechado = PropertyEntity({ text: ":", getType: typesKnockout.string, val: ko.observable("") });
    this.QuantidadeNavioAberto = PropertyEntity({ text: ":", getType: typesKnockout.string, val: ko.observable("") });
    this.NomeUsuario = PropertyEntity({ text: "", getType: typesKnockout.string, val: ko.observable("") });

    this.PesquisarNavioFechado = PropertyEntity({ eventClick: CliqueNavioFechado, type: types.event, text: "Filtrar navios fechados", tooltip: ko.observable("Filtrar navios fechados"), tooltipTitle: ko.observable("Filtrar") });
    this.PesquisarNavioAberto = PropertyEntity({ eventClick: CliqueNavioAberto, type: types.event, text: "Filtrar navios abertos", tooltip: ko.observable("Filtrar navios abertos"), tooltipTitle: ko.observable("Filtrar") });
    this.PesquisaFiltroInicial = PropertyEntity({ eventClick: PesquisaDocumentacoes, type: types.event, text: "Filtro tela inicial" });

    this.NavioFechado = PropertyEntity({ text: "Filtro Navio Fechado", getType: typesKnockout.bool, val: ko.observable(false) });
    this.NavioAberto = PropertyEntity({ text: "Filtro Navio Aberto", getType: typesKnockout.bool, val: ko.observable(false) });
}

//*******EVENTOS*******
function loadDocumentacao() {
    _documentacao = new Documentacao();
    KoBindings(_documentacao, "knockoutDocumentacao");

    obterUsuarioLogado();

    _documentacao.NavioAberto.val(true);
    PesquisaDocumentacoes();

    ColorirBotaoNavioAberto();
}

function obterUsuarioLogado() {
    executarReST("Documentacao/ObterUsuarioLogado", {}, function (retorno) {
        if (retorno.Success && retorno.Data) {
            _documentacao.NomeUsuario.val("Bem-vindo, " + retorno.Data.NomeUsuario);
        }
    });
}

function BuscarDashboardDocumentacao() {
    executarReST("Documentacao/BuscarDashboardDocumentacao", {}, function (r) {
        if (r.Success) {
            if (r.Data != null && r.Data !== false) {
                PreencherValores(r);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", r.Msg);
        }
    }, null, false);
}

var formatarData = function (data) {
    return moment(data).format("dddd, D [de] MMMM");
};

var pesquisaDetalhesCarga = function (nomeNavio, descricaoStatusCarga) {
    document.getElementById('tituloModalLegenda').innerText = nomeNavio + ' - ' + descricaoStatusCarga;

    _pesquisaDetalhesModalDash = new PesquisaDetalhesModal();

    _pesquisaDetalhesModalDash.NomeNavio.val(nomeNavio);
    _pesquisaDetalhesModalDash.StatusCarga.val(descricaoStatusCarga);

    var opcaoVisualizarCarga = { descricao: "Visualizar Carga", id: guid(), metodo: abrirCargaClick, icone: "" };

    var menuOpcoes = {
        tipo: TypeOptionMenu.link,
        descricao: "Visualizar Carga",
        opcoes: [opcaoVisualizarCarga]
    };

    _gridDetalhesModalDash = new GridView("tblDetalhesModalDash", "Documentacao/PesquisaDetalhesModal", _pesquisaDetalhesModalDash, menuOpcoes, { column: 1, dir: orderDir.desc });
    _gridDetalhesModalDash.CarregarGrid();
}

var pesquisaDetalhesMercante = function (nomeNavio, descricaoStatusMercante) {
    document.getElementById('tituloModalLegenda').innerText = nomeNavio + ' - ' + descricaoStatusMercante;

    _pesquisaDetalhesModalDash = new PesquisaDetalhesModal();

    _pesquisaDetalhesModalDash.NomeNavio.val(nomeNavio);
    _pesquisaDetalhesModalDash.StatusMercante.val(descricaoStatusMercante);

    var opcaoVisualizarCarga = { descricao: "Visualizar Carga", id: guid(), metodo: abrirCargaClick, icone: "" };

    var menuOpcoes = {
        tipo: TypeOptionMenu.link,
        descricao: "Visualizar Carga",
        opcoes: [opcaoVisualizarCarga]
    };

    _gridDetalhesModalDash = new GridView("tblDetalhesModalDash", "Documentacao/PesquisaDetalhesModal", _pesquisaDetalhesModalDash, menuOpcoes, { column: 1, dir: orderDir.desc });
    _gridDetalhesModalDash.CarregarGrid();
}

var pesquisaDetalhesSvm = function (nomeNavio, descricaoStatusSvm) {
    document.getElementById('tituloModalLegenda').innerText = nomeNavio + ' - ' + descricaoStatusSvm;

    _pesquisaDetalhesModalDash = new PesquisaDetalhesModal();

    _pesquisaDetalhesModalDash.NomeNavio.val(nomeNavio);
    _pesquisaDetalhesModalDash.StatusSvm.val(descricaoStatusSvm);

    var opcaoVisualizarCarga = { descricao: "Visualizar Carga", id: guid(), metodo: abrirCargaClick, icone: "" };

    var menuOpcoes = {
        tipo: TypeOptionMenu.link,
        descricao: "Visualizar Carga",
        opcoes: [opcaoVisualizarCarga]
    };

    _gridDetalhesModalDash = new GridView("tblDetalhesModalDash", "Documentacao/PesquisaDetalhesModal", _pesquisaDetalhesModalDash, menuOpcoes, { column: 1, dir: orderDir.desc });
    _gridDetalhesModalDash.CarregarGrid();
}

function abrirCargaClick(registroSelecionado) {
    window.open(registroSelecionado.LinkCarga, '_blank').focus();
}

function PesquisaDocumentacoes() {
    let dataInicial = _documentacao.DataInicial ? _documentacao.DataInicial.val() : null;
    let dataMais15FormatoBR = null;
    if (dataInicial != null && dataInicial != '') {
        let dataEhoras = dataInicial.split(" ");
        let dataEmPartes = dataEhoras[0].split("/");
        let objetoData = new Date(+dataEmPartes[2], dataEmPartes[1] - 1, +dataEmPartes[0]);
        let periodoInicialDataJS = new Date(objetoData);
        let dataMais15 = periodoInicialDataJS.setDate(periodoInicialDataJS.getDate() + 15);
        dataMais15FormatoBR = moment(dataMais15).format('DD/MM/yyyy');
        _documentacao.DataFinal.val(dataMais15FormatoBR);
    }

    let regiao = _documentacao.Regiao ? _documentacao.Regiao.val() : null;
    let navioAberto = _documentacao.NavioAberto ? _documentacao.NavioAberto.val() : null;
    let navioFechado = _documentacao.NavioFechado ? _documentacao.NavioFechado.val() : null;

    executarReST("Documentacao/Pesquisa", {
        DataInicial: dataInicial,
        DataFinal: dataMais15FormatoBR,
        Regiao: regiao,
        Embarque: JSON.stringify(_documentacao.Embarque.val()),
        NavioAberto: navioAberto,
        NavioFechado: navioFechado,
    }, function (r) {
        if (r.Success) {
            if (r.Data != null && r.Data !== false) {
                PreencherValores(r, navioAberto, navioFechado);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", r.Msg);
        }
    }, null, false);
}


function CliqueNavioAberto() {
    _documentacao.NavioAberto.val(true);
    _documentacao.NavioFechado.val(false);
    _documentacao.DataInicial.val("");
    _documentacao.DataFinal.val("");
    PesquisaDocumentacoes();
    LimparBordasBotoes();
    ColorirBotaoNavioAberto();
}

function CliqueNavioFechado() {
    _documentacao.NavioFechado.val(true);
    _documentacao.NavioAberto.val(false);
    _documentacao.DataInicial.val(Global.DataHora(EnumTipoOperacaoDate.Subtract, 15, EnumTipoOperacaoObjetoDate.Days));
    _documentacao.DataFinal.val("");

    PesquisaDocumentacoes(true);
    LimparBordasBotoes();
    ColorirBotaoNavioFechado();
}

function LimparBordasBotoes() {
    $("#navio-fechado-btn").css({ "border-color": "transparent" });
    $("#navio-aberto-btn").css({ "border-color": "transparent" });
}

function PreencherValores(r, aberto, fechado) {
    MudarValoresBotoes(fechado, r, aberto);

    var arr = [];
    for (let i = 0; i < r.Data.DadosDocumentacaoDash.length; i++) {
        const obj = r.Data.DadosDocumentacaoDash[i];

        let item = new ItemArray();
        item.NomeNavio.val(obj.NomeNavio);
        item.DataFechamento.val(obj.DataFechamento);
        item.DataEtsPol.val(obj.DataEtsPol);
        item.Mdfe.val(obj.Mdfe);
        item.Bookings.val(obj.Bookings);
        item.Regiao.val(obj.Regiao);
        item.TotalCargas.val(obj.TotalCarga);
        item.TotalSvm.val(obj.TotalSvm);
        item.TotalMercante.val(obj.TotalMercante);
        item.TotalCargaEmissao.val(obj.TotalCargaEmissao);
        item.TotalCargaErro.val(obj.TotalCargaErro);
        item.TotalCargaGerada.val(obj.TotalCargaGerada);
        item.TotalCargaPedentes.val(obj.TotalCargaPedentes);
        item.TotalMercantePendente.val(obj.TotalMercantePendente);
        item.TotalMercanteRetornado.val(obj.TotalMercanteRetornado);
        item.TotalSvmErro.val(obj.TotalSvmErro);
        item.TotalSvmGerado.val(obj.TotalSvmGerado);
        item.TotalSvmPendente.val(obj.TotalSvmPendente);
        item.NavioAberto.val(obj.NavioAberto == '1');
        arr.push(getItemArray(item));
    }

    PreencherOpcoesPortos(r.Data.Embarques);

    _documentacao.DadosDash.val(arr);
    for (let i = 0; i < r.Data.DadosDocumentacaoDash.length; i++) {
        const obj = r.Data.DadosDocumentacaoDash[i];

        new Chartist.Pie('#chartCargas' + i, {
            series: [obj.TotalCargaPedentes, obj.TotalCargaEmissao, obj.TotalCargaErro, obj.TotalCargaGerada]
        }, {
            donut: true,
            donutWidth: 30,
            donutSolid: true,
            showLabel: false
        });

        new Chartist.Pie('#chartSVMs' + i, {
            series: [obj.TotalSvmPendente, obj.TotalSvmErro, obj.TotalSvmGerado]
        }, {
            donut: true,
            donutWidth: 30,
            donutSolid: true,
            showLabel: false
        });

        new Chartist.Pie('#chartMercante' + i, {
            series: [obj.TotalMercantePendente, obj.TotalMercanteRetornado]
        }, {
            donut: true,
            donutWidth: 30,
            donutSolid: true,
            showLabel: false
        });

    }
}

function PreencherOpcoesPortos(listaPortos) {
    let portos = [];

    for (let i = 0; i < listaPortos.length; i++) {
        portos.push({
            text: listaPortos[i].Descricao,
            value: listaPortos[i].Descricao
        });
    }

    _documentacao.Embarque.options(portos);
}

function MudarValoresBotoes(fechado, r, aberto) {
    ExibirPainelDados(true);
    _documentacao.QuantidadeNavioFechado.val(r.Data.QuantidadeNavioFechado);
    _documentacao.QuantidadeNavioAberto.val(r.Data.QuantidadeNavioAberto);

    if (fechado) {
        if (r.Data.QuantidadeNavioFechado == 0)
            ExibirPainelDados(false);
    }
    else if (aberto) {
        if (r.Data.QuantidadeNavioAberto == 0)
            ExibirPainelDados(false);
    }
    else {
        if (r.Data.QuantidadeNavioFechado == 0 && r.Data.QuantidadeNavioAberto == 0)
            ExibirPainelDados(false);
    }
}

function ExibirPainelDados(exibir) {
    if (exibir)
        $("#panel-dados").show();
    else
        $("#panel-dados").hide();
}

var ItemArray = function () {
    this.NomeNavio = PropertyEntity({ getType: typesKnockout.string, val: ko.observable(""), def: "" });
    this.DataFechamento = PropertyEntity({ getType: typesKnockout.date, val: ko.observable(Global.DataAtual()), def: Global.DataAtual() });
    this.DataEtsPol = PropertyEntity({ getType: typesKnockout.date, val: ko.observable(Global.DataAtual()), def: Global.DataAtual() });
    this.Mdfe = PropertyEntity({ getType: typesKnockout.string, val: ko.observable(""), def: "" });
    this.Bookings = PropertyEntity({ getType: typesKnockout.int, val: ko.observable(0), });
    this.Regiao = PropertyEntity({ getType: typesKnockout.string, val: ko.observable('') });
    this.TotalCargas = PropertyEntity({ getType: typesKnockout.int, val: ko.observable(0) });
    this.TotalSvm = PropertyEntity({ getType: typesKnockout.int, val: ko.observable(0) });
    this.TotalMercante = PropertyEntity({ getType: typesKnockout.int, val: ko.observable(0) });
    this.TotalCargaEmissao = PropertyEntity({ getType: typesKnockout.int, val: ko.observable(0) });
    this.TotalCargaErro = PropertyEntity({ getType: typesKnockout.int, val: ko.observable(0) });
    this.TotalCargaGerada = PropertyEntity({ getType: typesKnockout.int, val: ko.observable(0) });
    this.TotalCargaPedentes = PropertyEntity({ getType: typesKnockout.int, val: ko.observable(0) });
    this.TotalMercantePendente = PropertyEntity({ getType: typesKnockout.int, val: ko.observable(0) });
    this.TotalMercanteRetornado = PropertyEntity({ getType: typesKnockout.int, val: ko.observable(0) });
    this.TotalSvmErro = PropertyEntity({ getType: typesKnockout.int, val: ko.observable(0) });
    this.TotalSvmGerado = PropertyEntity({ getType: typesKnockout.int, val: ko.observable(0) });
    this.TotalSvmPendente = PropertyEntity({ getType: typesKnockout.int, val: ko.observable(0) });
    this.NavioAberto = PropertyEntity({ getType: typesKnockout.bool, val: ko.observable(false), def: false });
}

var PesquisaDetalhesModal = function () {
    this.CodigoCarga = PropertyEntity({ getType: typesKnockout.int, val: ko.observable(0), def: "" });
    this.NomeNavio = PropertyEntity({ getType: typesKnockout.string, val: ko.observable(""), def: "" });
    this.DataFechamento = PropertyEntity({ getType: typesKnockout.date, val: ko.observable(Global.DataAtual()), def: Global.DataAtual() });
    this.DataEtsPol = PropertyEntity({ getType: typesKnockout.date, val: ko.observable(Global.DataAtual()), def: Global.DataAtual() });
    this.PortoOrigem = PropertyEntity({ getType: typesKnockout.string, val: ko.observable(""), def: "" });
    this.PortoDestino = PropertyEntity({ getType: typesKnockout.string, val: ko.observable(""), });
    this.Regiao = PropertyEntity({ getType: typesKnockout.string, val: ko.observable('') });
    this.Carga = PropertyEntity({ getType: typesKnockout.string, val: ko.observable(""), def: "" });
    this.LinkCarga = PropertyEntity({ getType: typesKnockout.string, val: ko.observable(""), def: "" });
    this.Booking = PropertyEntity({ getType: typesKnockout.string, val: ko.observable(""), def: "" });
    this.StatusCarga = PropertyEntity({ getType: typesKnockout.string, val: ko.observable(""), def: "" });
    this.StatusSvm = PropertyEntity({ getType: typesKnockout.string, val: ko.observable(""), def: "" });
    this.StatusMercante = PropertyEntity({ getType: typesKnockout.string, val: ko.observable(""), def: "" });
    this.Containeres = PropertyEntity({ getType: typesKnockout.string, val: ko.observable(""), def: "" });
}

var getItemArray = function (itemKnockout) {
    let item = {
        NomeNavio: itemKnockout.NomeNavio.val(),
        DataFechamento: itemKnockout.DataFechamento.val(),
        DataEtsPol: itemKnockout.DataEtsPol.val(),
        Mdfe: itemKnockout.Mdfe.val(),
        Bookings: itemKnockout.Bookings.val(),
        Regiao: itemKnockout.Regiao.val(),
        TotalCargas: itemKnockout.TotalCargas.val(),
        TotalSvm: itemKnockout.TotalSvm.val(),
        TotalMercante: itemKnockout.TotalMercante.val(),
        TotalCargaEmissao: itemKnockout.TotalCargaEmissao.val(),
        TotalCargaErro: itemKnockout.TotalCargaErro.val(),
        TotalCargaGerada: itemKnockout.TotalCargaGerada.val(),
        TotalCargaPedentes: itemKnockout.TotalCargaPedentes.val(),
        TotalMercantePendente: itemKnockout.TotalMercantePendente.val(),
        TotalMercanteRetornado: itemKnockout.TotalMercanteRetornado.val(),
        TotalSvmErro: itemKnockout.TotalSvmErro.val(),
        TotalSvmGerado: itemKnockout.TotalSvmGerado.val(),
        TotalSvmPendente: itemKnockout.TotalSvmPendente.val(),
        NavioAberto: itemKnockout.NavioAberto.val(),
    };
    return item;
}

function ColorirBotaoNavioAberto() {
    $("#navio-aberto-btn").css({ "border-color": "#f2994a" });
}

function ColorirBotaoNavioFechado() {
    $("#navio-fechado-btn").css({ "border-color": "#14a884" });
}