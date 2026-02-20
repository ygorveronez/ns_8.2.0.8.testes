/// <reference path="../../../../../js/Global/Globais.js" />
/// <reference path="../../../../../js/libs/jquery-2.1.1.js" />
/// <reference path="../../../../../js/Global/CRUD.js" />
/// <reference path="../../../../../js/Global/Rest.js" />
/// <reference path="../../../../../js/Global/Mensagem.js" />
/// <reference path="../../../../../js/Global/Grid.js" />
/// <reference path="../../../../../js/bootstrap/bootstrap.js" />
/// <reference path="../../../../../js/libs/jquery.blockui.js" />
/// <reference path="../../../../../js/Global/knoutViewsSlides.js" />
/// <reference path="../../../../../js/libs/jquery.maskMoney.js" />
/// <reference path="../../../../../js/plugin/datatables/jquery.dataTables.js" />
/// <reference path="../../../../../ViewsScripts/Enumeradores/EnumTipoArquivoRelatorio.js" />
/// <reference path="../../Relatorios/Global/Relatorio.js" />
/// <reference path="../../../../../ViewsScripts/Enumeradores/EnumSituacaoTitulo.js" />
/// <reference path="../../../../../ViewsScripts/Consultas/Tranportador.js" />
/// <reference path="../../../../../ViewsScripts/Consultas/Filial.js" />
/// <reference path="../../../../../ViewsScripts/Consultas/Cliente.js" />

//*******MAPEAMENTO KNOUCKOUT*******

let _gridPedidoAguardandoIntegracao, _gridConsultaManual, _pesquisaPedidoAguardandoIntegracao, _CRUDRelatorio, _CRUDFiltrosRelatorio;

let _StatusIntegracao = [
    { text: "Todos", value: -1 },
    { text: "Aguardando integração", value: 0 },
    { text: "Integrado", value: 1 },
    { text: "Erro na integração", value: 2 },
    { text: "Aguardando Gerar Carga", value: 4 },
];

let _TipoIntegracao = [
    { text: "Todos", value: -1 },
    { text: "VTEX", value: 84 },
    { text: "E-Millenium", value: 81 },
];

let PesquisaHistoricoIntegracaoPedidoAgIntegracao = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
}

let PesquisaNumeroNFChaveDeAcesso = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.CodigoPedidoIntegracao = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.string });
    this.NumeroPedidoInformacao = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.string });
}

let PesquisaPedidoAguardandoIntegracao = function () {
    this.Pesquisa = PropertyEntity({ text: "Pesquisa (Numero Pedido):", val: ko.observable(""), def: "", getType: typesKnockout.text, visible: ko.observable(true) });
    this.DataInicio = PropertyEntity({ text: "Data Inicial:", val: ko.observable(Global.DataAtual()), def: "", getType: typesKnockout.date, visible: ko.observable(true) });
    this.DataFim = PropertyEntity({ text: "Data Final:", val: ko.observable(Global.DataAtual()), def: "", getType: typesKnockout.date, visible: ko.observable(true) });
    this.DataInicio.dateRangeLimit = this.DataFim;
    this.DataFim.dateRangeInit = this.DataInicio;

    this.DataEmbarqueInicio = PropertyEntity({ text: "(Emillenium) Data Embarque Inicial:", val: ko.observable(""), def: "", getType: typesKnockout.date, visible: ko.observable(true) });
    this.DataEmbarqueFim = PropertyEntity({ text: "(Emillenium) Data Embarque Final:", val: ko.observable(""), def: "", getType: typesKnockout.date, visible: ko.observable(true) });
    this.DataEmbarqueInicio.dateRangeLimit = this.DataEmbarqueFim;
    this.DataEmbarqueFim.dateRangeInit = this.DataEmbarqueInicio;

    this.TransIDBusca = PropertyEntity({ text: "Trans ID:", val: ko.observable(""), def: "", getType: typesKnockout.text, visible: ko.observable(true) });
    this.top = PropertyEntity({ text: "Top:", val: ko.observable("50"), def: "50", getType: typesKnockout.text, enable: false, visible: ko.observable(true) });

    this.SituacaoIntegracao = PropertyEntity({ val: ko.observable(-1), options: _StatusIntegracao, def: -1, text: "Situação: " });
    this.TipoIntegracao = PropertyEntity({ val: ko.observable(-1), options: _TipoIntegracao, def: -1, text: "Tipo: " });
    this.ConsultaManual = PropertyEntity({ val: ko.observable(false), def: false, getType: typesKnockout.bool });

};

let CRUDFiltrosRelatorio = function () {
    this.Preview = PropertyEntity({
        eventClick: function (e) {
            _pesquisaPedidoAguardandoIntegracao.ConsultaManual.val(false);

            _gridPedidoAguardandoIntegracao.CarregarGrid();
        }, type: types.event, text: "Pesquisar", idGrid: "gridPreviewRelatorio", visible: ko.observable(true)
    });

    this.BuscaManual = PropertyEntity({
        eventClick: function (e) {
            exibirConfirmacao("Confirmação", "Realmente deseja consultar manualmente notas fiscais na Emillenium?", function () {
                _pesquisaPedidoAguardandoIntegracao.ConsultaManual.val(true);
                _gridConsultaManual.CarregarGrid();
            });

        }, type: types.event, text: "Consultar Notas", idGrid: "gridPreviewConsultaManual", visible: ko.observable(true)
    });

};


//*******EVENTOS*******

function loadRelatorioPedidoAguardandoIntegracao() {
    _pesquisaPedidoAguardandoIntegracao = new PesquisaPedidoAguardandoIntegracao();
    _CRUDFiltrosRelatorio = new CRUDFiltrosRelatorio();

    KoBindings(_pesquisaPedidoAguardandoIntegracao, "knockoutPesquisaPedidoAguardandoIntegracao");
    KoBindings(_CRUDFiltrosRelatorio, "knockoutCRUDFiltrosPesquisaPedidoAguardandoIntegracao");

    buscarPedidoAguardandoIntegracao();
    buscarNotasManualemente();
}

function buscarPedidoAguardandoIntegracao() {

    let menuOpcoes = { tipo: TypeOptionMenu.list, descricao: "Opções", tamanho: 7, opcoes: [] };
    menuOpcoes.opcoes.push({ descricao: "Histórico de Integração", id: guid(), metodo: ExibirHistoricoIntegracaoPedidoAgIntegracao, tamanho: "20", icone: "" });
    menuOpcoes.opcoes.push({ descricao: "Detalhes", id: guid(), metodo: ExibirNumeroNFChaveDeAcesso, tamanho: "20", icone: "" });
    menuOpcoes.opcoes.push({ descricao: "Buscar NF-es Emillenium", id: guid(), metodo: BuscarNfesEmillenium, tamanho: "20", icone: "", visibilidade: VisibilidadeOpcaoBuscarNfeEmillenium });
    menuOpcoes.opcoes.push({ descricao: "Reenviar", id: guid(), metodo: ReenviarPedidoAgIntegracao, tamanho: "20", icone: "", visibilidade: VisibilidadeOpcaoReenviar });

    let configExportacao = {
        url: "Relatorios/PedidoAguardandoIntegracao/ExportarPesquisa",
        titulo: "Pedidos Aguardando Intergração"
    };

    _gridPedidoAguardandoIntegracao = new GridView(_CRUDFiltrosRelatorio.Preview.idGrid, "Relatorios/PedidoAguardandoIntegracao/Pesquisa", _pesquisaPedidoAguardandoIntegracao, menuOpcoes, null, null, null, null, null, null, null, null, configExportacao);
    _gridPedidoAguardandoIntegracao.SetQuantidadeLinhasPorPagina(20);
    _gridPedidoAguardandoIntegracao.CarregarGrid();
}


function buscarNotasManualemente() {

    let menuOpcoes = { tipo: TypeOptionMenu.list, descricao: "Opções", tamanho: 7, opcoes: [] };
    menuOpcoes.opcoes.push({ descricao: "Histórico de Integração", id: guid(), metodo: ExibirHistoricoIntegracaoPedidoAgIntegracao, tamanho: "20", icone: "" });

    _gridConsultaManual = new GridView(_CRUDFiltrosRelatorio.Preview.idGrid, "Relatorios/PedidoAguardandoIntegracao/ConsultaManualNota", _pesquisaPedidoAguardandoIntegracao, menuOpcoes, null, null, null, null, null, null, null, null, null, null, null, retornoDados);
    _gridConsultaManual.SetQuantidadeLinhasPorPagina(100);
}

function VisibilidadeOpcaoReenviar(data) {
    if (data.TipoIntegracao == "VTEX" && (data.Situacao == EnumSituacaoIntegracaoPedidoAguardandoIntegracao.ProblemaIntegracao || data.Situacao == EnumSituacaoIntegracaoPedidoAguardandoIntegracao.ErroGenerico))
        return true;
    return data.TipoIntegracao == "E-Millenium" && (data.Situacao == EnumSituacaoIntegracaoPedidoAguardandoIntegracao.ProblemaGerarCarga || data.Situacao == EnumSituacaoIntegracaoPedidoAguardandoIntegracao.ProblemaIntegracao);
}

function VisibilidadeOpcaoBuscarNfeEmillenium(data) {
    return data.TipoIntegracao == "E-Millenium" && data.NumeroPedido !== '';
}


function retornoDados(nRow, data) {
    if (data.UltimotransId > 1) {
        _pesquisaPedidoAguardandoIntegracao.TransIDBusca.val(data.UltimotransId);
    }
}

function ExibirHistoricoIntegracaoPedidoAgIntegracao(integracao) {
    BuscarHistoricoIntegracaoPedidoAgIntegracao(integracao);
    Global.abrirModal("divModalHistoricoIntegracaoPedidoAgIntegracao");
}

function ExibirNumeroNFChaveDeAcesso(integracao) {
    BuscarNumeroNFChaveDeAcesso(integracao);
    Global.abrirModal("divModalNumeroNFChaveDeAcesso");
}

function ReenviarPedidoAgIntegracao(integracao) {
    exibirConfirmacao("Confirmação", "Realmente deseja reenviar a integração do pedido?", function () {
        let data = {
            Codigo: integracao.Codigo
        };
        executarReST("Relatorios/PedidoAguardandoIntegracao/ReenviarIntegracao", data, function (arg) {
            if (!arg.Success)
                exibirMensagem(tipoMensagem.aviso, "Falha", arg.Msg);
            else {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Integração reenviada com sucesso");
            }
        });
    });
}

function BuscarNfesEmillenium(integracao) {
    let data = {
        Codigo: integracao.Codigo
    };
    executarReST("Relatorios/PedidoAguardandoIntegracao/BuscarNFesEmillenium", data, function (arg) {
        if (!arg.Success)
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        else {
            exibirMensagem(tipoMensagem.ok, "Sucesso", "Sucesso em buscar NF-es");
        }
        _gridPedidoAguardandoIntegracao.CarregarGrid();
    });
}

function BuscarHistoricoIntegracaoPedidoAgIntegracao(integracao) {
    let pesquisaHistoricoIntegracaoPedidoAgIntegracao = new PesquisaHistoricoIntegracaoPedidoAgIntegracao();
    pesquisaHistoricoIntegracaoPedidoAgIntegracao.Codigo.val(integracao.Codigo);

    let download = { descricao: "Download Arquivos", id: guid(), evento: "onclick", metodo: DownloadArquivosHistoricoIntegracaoPedidoAgIntegracao, tamanho: "20", icone: "" };

    let menuOpcoes = {
        tipo: TypeOptionMenu.link,
        opcoes: [download]
    };

    let gridHistoricoIntegracaoPedidoAgIntegracao = new GridView("tblHistoricoIntegracaoPedidoAgIntegracao", "Relatorios/PedidoAguardandoIntegracao/ConsultarHistoricoIntegracao", pesquisaHistoricoIntegracaoPedidoAgIntegracao, menuOpcoes, { column: 1, dir: orderDir.desc });
    gridHistoricoIntegracaoPedidoAgIntegracao.CarregarGrid();
}


function DownloadArquivosHistoricoIntegracaoPedidoAgIntegracao(historicoConsulta) {
    executarDownload("Relatorios/PedidoAguardandoIntegracao/DownloadArquivosHistoricoIntegracao", { Codigo: historicoConsulta.Codigo });
}

function BuscarNumeroNFChaveDeAcesso(integracao) {
    let pesquisaNumeroNFChaveDeAcesso = new PesquisaNumeroNFChaveDeAcesso();
    pesquisaNumeroNFChaveDeAcesso.Codigo.val(integracao.Codigo);
    pesquisaNumeroNFChaveDeAcesso.CodigoPedidoIntegracao.val(integracao.NumeroPedido);

    let informacoes = integracao.Informacao.split(" ");
    pesquisaNumeroNFChaveDeAcesso.NumeroPedidoInformacao.val(informacoes[1]);

    let gridNumeroNFChaveDeAcesso = new GridView("tblNumeroNFChaveDeAcesso", "Relatorios/PedidoAguardandoIntegracao/ConsultarNumeroNFChaveDeAcesso", pesquisaNumeroNFChaveDeAcesso, null, { column: 1, dir: orderDir.desc });
    gridNumeroNFChaveDeAcesso.CarregarGrid();
}