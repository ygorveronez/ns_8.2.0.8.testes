var _gridIntegracaoGRVeiculo;
var _integracaoGRVeiculo;
var _pesquisaIntegracaoGRVeiculo;
var _pesquisaHistoricoIntegracaoGRVeiculo;
var _gridHistoricoIntegracaoGRVeiculo;

function VeiculoIntegracaoGR() {
    this.TotalAguardandoIntegracao = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int, text: Localization.Resources.Gerais.Geral.AguardandoIntegracao.getFieldDescription() });
    this.TotalAguardandoRetorno = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int, text: Localization.Resources.Gerais.Geral.AguardandoRetorno.getFieldDescription() });
    this.TotalGeral = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int, text: Localization.Resources.Gerais.Geral.TotalGeral.getFieldDescription() });
    this.TotalIntegrado = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int, text: Localization.Resources.Gerais.Geral.Integrados.getFieldDescription() });
    this.TotalProblemaIntegracao = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int, text: Localization.Resources.Gerais.Geral.ProblemasNaIntegracao.getFieldDescription() });

    this.ObterTotais = PropertyEntity({ eventClick: carregarTotaisIntegracaoVeiculoGR, type: types.event, text: ko.observable(Localization.Resources.Gerais.Geral.ObterTotais), idGrid: guid(), visible: ko.observable(true) });
};

function PesquisaVeiculoIntegracoesGR() {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Situacao = PropertyEntity({ val: ko.observable(""), options: EnumSituacaoIntegracaoCarga.obterOpcoesPesquisa(), text: ko.observable(Localization.Resources.Gerais.Geral.Situacao.getFieldDescription()), def: "", issue: 272 });

    this.Pesquisar = PropertyEntity({ eventClick: carregarTotaisIntegracaoVeiculoGR, type: types.event, text: ko.observable(Localization.Resources.Gerais.Geral.Pesquisar), idGrid: guid(), visible: ko.observable(true) });
    this.ReenviarTodos = PropertyEntity({ eventClick: reenviarTodasIntegracoesClick, type: types.event, text: ko.observable(Localization.Resources.Gerais.Geral.ReenviarTodos), visible: ko.observable(false) });
};

var PesquisaHistoricoIntegracaoGRVeiculo = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
};

function loadGridIntegracoesGRVeiculo() {
    var linhasPorPaginas = 5;
    var opcaoIntegrar = { descricao: Localization.Resources.Gerais.Geral.Integrar, id: guid(), metodo: integrarClick, icone: "" };

    var historico = { descricao: Localization.Resources.Gerais.Geral.HistoricoDeIntegracao, id: guid(), metodo: exibirHistoricoIntegracoesVeiculoGRClick, icone: "" };
    var menuOpcoes = { tipo: TypeOptionMenu.list, descricao: Localization.Resources.Gerais.Geral.Opcoes, tamanho: 10, opcoes: [opcaoIntegrar, historico] };

    _gridIntegracaoGRVeiculo = new GridView(_pesquisaIntegracaoGRVeiculo.Pesquisar.idGrid, "VeiculoIntegracao/PesquisaVeiculoIntegracoes", _pesquisaIntegracaoGRVeiculo, menuOpcoes, null, linhasPorPaginas);
    _gridIntegracaoGRVeiculo.CarregarGrid();
}

function loadIntegracoesGRVeiculo(e) {

    var origemId = _cargaAtual.Codigo.val();

    $.get("Content/Static/Veiculo/VeiculoIntegracaoGR.html?dyn=" + guid(), function (html) {

        var knockoutPesquisaIntegracao = "knockoutPesquisaIntegracaoVeiculoGR";
        var knockoutPesquisaIntegracaoDinamico = knockoutPesquisaIntegracao + origemId;

        var knockoutDadosIntegracao = "knockoutDadosIntegracaoVeiculoGR";
        var knockoutDadosIntegracaoDinamico = knockoutDadosIntegracao + origemId;

        html = html.replaceAll(knockoutPesquisaIntegracao, knockoutPesquisaIntegracaoDinamico);
        html = html.replaceAll(knockoutDadosIntegracao, knockoutDadosIntegracaoDinamico);

        $("#divIntegracaoVeiculo_" + e.EtapaInicioTMS.idGrid).html(html);

        _integracaoGRVeiculo = new VeiculoIntegracaoGR();
        KoBindings(_integracaoGRVeiculo, knockoutDadosIntegracaoDinamico);

        _pesquisaIntegracaoGRVeiculo = new PesquisaVeiculoIntegracoesGR();
        KoBindings(_pesquisaIntegracaoGRVeiculo, knockoutPesquisaIntegracaoDinamico);

        LocalizeCurrentPage();

        _pesquisaIntegracaoGRVeiculo.Codigo.val(e.Veiculo.codEntity());
        loadGridIntegracoesGRVeiculo();

        $("#liTabCargaDadosTransporteIntegracoesVeiculoMotorista_" + e.EtapaInicioTMS.idGrid).removeClass("d-none");
    });
}

function carregarTotaisIntegracaoVeiculoGR() {
    executarReST("VeiculoIntegracao/ObterTotaisIntegracoes", { Codigo: _pesquisaIntegracaoGRVeiculo.Codigo.val() }, function (retorno) {
        if (retorno.Success) {
            _integracaoGRVeiculo.TotalGeral.val(retorno.Data.TotalGeral);
            _integracaoGRVeiculo.TotalAguardandoIntegracao.val(retorno.Data.TotalAguardandoIntegracao);
            _integracaoGRVeiculo.TotalAguardandoRetorno.val(retorno.Data.TotalAguardandoRetorno);
            _integracaoGRVeiculo.TotalProblemaIntegracao.val(retorno.Data.TotalProblemaIntegracao);
            _integracaoGRVeiculo.TotalIntegrado.val(retorno.Data.TotalIntegrado);
        } else {
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, retorno.Msg);
        }
    });
}

function integrarClick(registroSelecionado) {
    executarReST("VeiculoIntegracao/Integrar", { Codigo: registroSelecionado.Codigo }, function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                carregarIntegracoes();
            } else {
                exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Atencao, retorno.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, retorno.Msg);
        }
    });
}

function carregarIntegracoes() {
    _gridIntegracaoGRVeiculo.CarregarGrid();

    carregarTotaisIntegracaoVeiculoGR();
}

function exibirHistoricoIntegracoesVeiculoGRClick(integracao) {

    BuscarHistoricoIntegracaoVeiculo(integracao);
    Global.abrirModal("divModalHistoricoIntegracaoVeiculo");
}

function BuscarHistoricoIntegracaoVeiculo(integracao) {

    _pesquisaHistoricoIntegracaoGRVeiculo = new PesquisaHistoricoIntegracaoGRVeiculo();
    _pesquisaHistoricoIntegracaoGRVeiculo.Codigo.val(integracao.Codigo);

    var download = { descricao: Localization.Resources.Gerais.Geral.Download, id: guid(), evento: "onclick", metodo: DownloadArquivosHistoricoIntegracaoClick, tamanho: "20", icone: "" };

    var menuOpcoes = {
        tipo: TypeOptionMenu.link,
        opcoes: [download]
    };

    _gridHistoricoIntegracaoGRVeiculo = new GridView("tblHistoricoIntegracaoVeiculo", "VeiculoIntegracao/ConsultarHistoricoIntegracao", _pesquisaHistoricoIntegracaoGRVeiculo, menuOpcoes, { column: 1, dir: orderDir.desc });
    _gridHistoricoIntegracaoGRVeiculo.CarregarGrid();
}

function DownloadArquivosHistoricoIntegracaoClick(historicoConsulta) {
    executarDownload("VeiculoIntegracao/DownloadArquivosHistoricoIntegracao", { Codigo: historicoConsulta.Codigo });
}