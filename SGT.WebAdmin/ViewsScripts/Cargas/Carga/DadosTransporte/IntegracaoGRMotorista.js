var _gridIntegracaoGRMotorista;
var _integracaoGRMotorista;
var _pesquisaIntegracaoGRMotorista;
var _pesquisaHistoricoIntegracaoGRMotorista;
var _gridHistoricoIntegracaoGRMotorista;

function MotoristaIntegracaoGR() {
    this.TotalAguardandoIntegracao = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int, text: Localization.Resources.Gerais.Geral.AguardandoIntegracao.getFieldDescription() });
    this.TotalAguardandoRetorno = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int, text: Localization.Resources.Gerais.Geral.AguardandoRetorno.getFieldDescription() });
    this.TotalGeral = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int, text: Localization.Resources.Gerais.Geral.TotalGeral.getFieldDescription() });
    this.TotalIntegrado = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int, text: Localization.Resources.Gerais.Geral.Integrados.getFieldDescription() });
    this.TotalProblemaIntegracao = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int, text: Localization.Resources.Gerais.Geral.ProblemasNaIntegracao.getFieldDescription() });

    this.ObterTotais = PropertyEntity({ eventClick: carregarTotaisIntegracaoMotoristaGR, type: types.event, text: ko.observable(Localization.Resources.Gerais.Geral.ObterTotais), idGrid: guid(), visible: ko.observable(true) });
};

function PesquisaMotoristaIntegracoesGR() {
    this.CodigoCarga = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Situacao = PropertyEntity({ val: ko.observable(""), options: EnumSituacaoIntegracaoCarga.obterOpcoesPesquisa(), text: ko.observable(Localization.Resources.Gerais.Geral.Situacao.getFieldDescription()), def: "", issue: 272 });
    this.Codigos = PropertyEntity({ getType: typesKnockout.dynamic, val: ko.observable(new Array()) });

    this.Pesquisar = PropertyEntity({ eventClick: carregarTotaisIntegracaoMotoristaGR, type: types.event, text: ko.observable(Localization.Resources.Gerais.Geral.Pesquisar), idGrid: guid(), visible: ko.observable(true) });
    this.ReenviarTodos = PropertyEntity({ eventClick: reenviarTodasIntegracoesClick, type: types.event, text: ko.observable(Localization.Resources.Gerais.Geral.ReenviarTodos), visible: ko.observable(false) });
};

var PesquisaHistoricoIntegracaoGRMotorista = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
};

function loadGridIntegracoesGRMotorista() {
    var linhasPorPaginas = 5;
    var opcaoIntegrar = { descricao: Localization.Resources.Gerais.Geral.Integrar, id: guid(), metodo: integrarClick, icone: "" };

    var historico = { descricao: Localization.Resources.Gerais.Geral.HistoricoDeIntegracao, id: guid(), metodo: exibirHistoricoIntegracoesMotoristaGRClick, icone: "" };
    var menuOpcoes = { tipo: TypeOptionMenu.list, descricao: Localization.Resources.Gerais.Geral.Opcoes, tamanho: 10, opcoes: [opcaoIntegrar, historico] };
    _gridIntegracaoGRMotorista = new GridView(_pesquisaIntegracaoGRMotorista.Pesquisar.idGrid, "MotoristaIntegracao/PesquisaMotoristaIntegracoes", _pesquisaIntegracaoGRMotorista, menuOpcoes, null, linhasPorPaginas);
    _gridIntegracaoGRMotorista.CarregarGrid();
}

function loadIntegracoesGRMotorista(e) {

    let origemId = e.Codigo.val()

    $.get("Content/Static/Transportador/IntegracaoMotoristaGR.html?dyn=" + guid(), function (html) {
        var knockoutPesquisaIntegracao = "knockoutPesquisaIntegracaoMotoristaGR";
        var knockoutPesquisaIntegracaoDinamico = knockoutPesquisaIntegracao + origemId;

        var knockoutDadosIntegracao = "knockoutDadosIntegracaoMotoristaGR";
        var knockoutDadosIntegracaoDinamico = knockoutDadosIntegracao + origemId;

        html = html.replaceAll(knockoutPesquisaIntegracao, knockoutPesquisaIntegracaoDinamico);
        html = html.replaceAll(knockoutDadosIntegracao, knockoutDadosIntegracaoDinamico);

        $("#divIntegracaoMotorista_" + e.EtapaInicioTMS.idGrid).html(html);

        _integracaoGRMotorista = new MotoristaIntegracaoGR();
        KoBindings(_integracaoGRMotorista, knockoutDadosIntegracaoDinamico);

        _pesquisaIntegracaoGRMotorista = new PesquisaMotoristaIntegracoesGR();
        KoBindings(_pesquisaIntegracaoGRMotorista, knockoutPesquisaIntegracaoDinamico);

        _pesquisaIntegracaoGRMotorista.CodigoCarga.val(e.Codigo.val());
        LocalizeCurrentPage();
        buscarCodigosMotoristasCarga();

        $("#liTabCargaDadosTransporteIntegracoesVeiculoMotorista_" + e.EtapaInicioTMS.idGrid).removeClass("d-none");
    });
}

function carregarTotaisIntegracaoMotoristaGR() {
    executarReST("MotoristaIntegracao/ObterTotaisIntegracoes", { Codigos: _pesquisaIntegracaoGRMotorista.Codigos.val() }, function (retorno) {
        if (retorno.Success) {
            _integracaoGRMotorista.TotalGeral.val(retorno.Data.TotalGeral);
            _integracaoGRMotorista.TotalAguardandoIntegracao.val(retorno.Data.TotalAguardandoIntegracao);
            _integracaoGRMotorista.TotalAguardandoRetorno.val(retorno.Data.TotalAguardandoRetorno);
            _integracaoGRMotorista.TotalProblemaIntegracao.val(retorno.Data.TotalProblemaIntegracao);
            _integracaoGRMotorista.TotalIntegrado.val(retorno.Data.TotalIntegrado);
        } else {
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, retorno.Msg);
        }
    });
}

function buscarCodigosMotoristasCarga() {
    executarReST("MotoristaIntegracao/ObterCodigosMotoristasCargas", { CodigoCarga: _pesquisaIntegracaoGRMotorista.CodigoCarga.val() }, function (retorno) {
        if (retorno.Success) {

            var listaCodigos = new Array();

            for (var i = 0; i < retorno.Data.length; i++) {
                listaCodigos.push(retorno.Data[i].Codigo);
            }

            _pesquisaIntegracaoGRMotorista.Codigos.val(JSON.stringify(listaCodigos));

            loadGridIntegracoesGRMotorista();
        } else {
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, retorno.Msg);
        }
    });
}

function reenviarTodasIntegracoesClick() {
    return;
}

function integrarClick(registroSelecionado) {
    executarReST("MotoristaIntegracao/Integrar", { Codigo: registroSelecionado.Codigo }, function (retorno) {
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
    _gridIntegracaoGRMotorista.CarregarGrid();

    carregarTotaisIntegracaoMotoristaGR();
}

function exibirHistoricoIntegracoesMotoristaGRClick(integracao) {
    BuscarHistoricoIntegracaoMotorista(integracao);
    Global.abrirModal("divModalHistoricoIntegracaoMotorista");
}

function BuscarHistoricoIntegracaoMotorista(integracao) {
    _pesquisaHistoricoIntegracaoGRMotorista = new PesquisaHistoricoIntegracaoGRMotorista();
    _pesquisaHistoricoIntegracaoGRMotorista.Codigo.val(integracao.Codigo);

    var download = { descricao: Localization.Resources.Gerais.Geral.Download, id: guid(), evento: "onclick", metodo: DownloadArquivosHistoricoIntegracaoClick, tamanho: "20", icone: "" };

    var menuOpcoes = {
        tipo: TypeOptionMenu.link,
        opcoes: [download]
    };

    _gridHistoricoIntegracaoGRMotorista = new GridView("tblHistoricoIntegracaoMotorista", "MotoristaIntegracao/ConsultarHistoricoIntegracao", _pesquisaHistoricoIntegracaoGRMotorista, menuOpcoes, { column: 1, dir: orderDir.desc });
    _gridHistoricoIntegracaoGRMotorista.CarregarGrid();
}

function DownloadArquivosHistoricoIntegracaoClick(historicoConsulta) {
    executarDownload("MotoristaIntegracao/DownloadArquivosHistoricoIntegracao", { Codigo: historicoConsulta.Codigo });
}

function SetarHTMLIntegracaoMotoristaGR(idContent, idReplace) {
    $("#" + idContent).html(_HTMLCargaDadosTransporteIntegracao.replace(/#divIntegracaoGR/g, idReplace));
}