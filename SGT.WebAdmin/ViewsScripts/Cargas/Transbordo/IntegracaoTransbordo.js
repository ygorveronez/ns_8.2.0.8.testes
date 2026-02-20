var _tipoIntegracaoTransbordo = new Array();
var _gridIntegracaoTransbordo;
var _gridHistoricoIntegracaoTransbordo;
var _integracaoTransbordo;
var _pesquisaHistoricoIntegracaoTransbordo;

var _situacaoIntegracaoTransbordo = [
    { value: "", text: Localization.Resources.Gerais.Geral.Todas },
    { value: EnumSituacaoIntegracaoCarga.AgIntegracao, text: Localization.Resources.Gerais.Geral.AguardandoIntegracao },
    { value: EnumSituacaoIntegracaoCarga.AgRetorno, text: Localization.Resources.Gerais.Geral.AguardandoRetorno },
    { value: EnumSituacaoIntegracaoCarga.Integrado, text: Localization.Resources.Gerais.Geral.Integrado },
    { value: EnumSituacaoIntegracaoCarga.ProblemaIntegracao, text: Localization.Resources.Gerais.Geral.Falha }
];

var PesquisaHistoricoIntegracaoTransbordo = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
}

var IntegracaoTransbordo = function () {

    this.Transbordo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Tipo = PropertyEntity({ val: ko.observable(""), options: _tipoIntegracaoTransbordo, text: Localization.Resources.Gerais.Geral.Integracao.getFieldDescription(), def: "", issue: 267, visible: ko.observable(false) });
    this.Situacao = PropertyEntity({ val: ko.observable(""), options: _situacaoIntegracaoTransbordo, text: Localization.Resources.Gerais.Geral.Situacao.getFieldDescription(), def: "", issue: 272 });

    this.TotalGeral = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int, text: Localization.Resources.Gerais.Geral.Total.getFieldDescription() });
    this.TotalAguardandoIntegracao = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int, text: Localization.Resources.Gerais.Geral.AguardandoIntegracao.getFieldDescription() });
    this.TotalAguardandoRetorno = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int, text: Localization.Resources.Gerais.Geral.AguardandoRetorno.getFieldDescription() });
    this.TotalProblemaIntegracao = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int, text: Localization.Resources.Gerais.Geral.ProblemasNaIntegracao.getFieldDescription() });
    this.TotalIntegrado = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int, text: Localization.Resources.Gerais.Geral.Integrado.getFieldDescription() });

    this.Pesquisar = PropertyEntity({
        eventClick: function (e) {
            _gridIntegracaoTransbordo.CarregarGrid();
            ObterTotaisIntegracaoTransbordo();
        }, type: types.event, text: Localization.Resources.Gerais.Geral.Pesquisar, idGrid: guid(), visible: ko.observable(true)
    });

    this.ReenviarTodos = PropertyEntity({
        eventClick: function (e) {
            ReenviarTodosIntegracaoTransbordo();
        }, type: types.event, text: Localization.Resources.Gerais.Geral.ReenviarTodos, idGrid: guid(), visible: ko.observable(true)
    });

    this.FinalizarEtapa = PropertyEntity({
        eventClick: function (e) {
            FinalizarEtapaClick();
        }, type: types.event, text: Localization.Resources.Cargas.Transbordo.FinalizarEtapa, idGrid: guid(), visible: ko.observable(false)
    });

    this.ObterTotais = PropertyEntity({
        eventClick: function (e) {
            ObterTotaisIntegracaoTransbordo();
        }, type: types.event, text: Localization.Resources.Gerais.Geral.ObterTotais, idGrid: guid(), visible: ko.observable(true)
    });
}

function LoadIntegracaoTransbordo(transbordo, idKnockoutIntegracao) {

    _integracaoTransbordo = new IntegracaoTransbordo();
    _integracaoTransbordo.Transbordo.val(transbordo.Codigo.val());

    KoBindings(_integracaoTransbordo, idKnockoutIntegracao);

    ObterTotaisIntegracaoTransbordo();
    ConfigurarPesquisaIntegracaoTransbordo();
}

function ConfigurarPesquisaIntegracaoTransbordo() {

    var menuOpcoes = { tipo: TypeOptionMenu.list, descricao: Localization.Resources.Gerais.Geral.Opcoes, tamanho: 7, opcoes: [] };

    menuOpcoes.opcoes.push({ descricao: Localization.Resources.Gerais.Geral.Reenviar, id: guid(), metodo: ReenviarIntegracaoTransbordo, tamanho: "20", icone: "", visibilidade: VisibilidadeOpcaoReenviarIntegracaoTransbordo });
    menuOpcoes.opcoes.push({ descricao: Localization.Resources.Gerais.Geral.HistoricoIntegracao, id: guid(), metodo: ExibirHistoricoIntegracaoTransbordo, tamanho: "20", icone: "", visibilidade: VisibilidadeOpcaoHistoricoIntegracaoTransbordo });

    _gridIntegracaoTransbordo = new GridView(_integracaoTransbordo.Pesquisar.idGrid, "TransbordoIntegracao/Pesquisa", _integracaoTransbordo, menuOpcoes);
    _gridIntegracaoTransbordo.CarregarGrid();
}

function VisibilidadeOpcaoReenviarIntegracaoTransbordo(data) {
    if (data.SituacaoIntegracao != EnumSituacaoIntegracao.ProblemaIntegracao)
        return false;

    return true;
}

function VisibilidadeOpcaoHistoricoIntegracaoTransbordo(data) {
    if (data.Tipo == EnumTipoIntegracao.FTP)
        return false;

    return true;
}

function ReenviarIntegracaoTransbordo(data) {
    executarReST("TransbordoIntegracao/Reenviar", { Codigo: data.Codigo }, function (r) {
        if (r.Success) {
            exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, Localization.Resources.Gerais.Geral.ReenvioSolicitadoComSucesso);
            _gridIntegracaoTransbordo.CarregarGrid();
        } else {
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, r.Msg);
        }
    });
}

function ReenviarTodosIntegracaoTransbordo() {
    exibirConfirmacao("Atenção!", Localization.Resources.Cargas.Transbordo.DesejaRealmenteReenviarAsIntegracoes, function () {
        executarReST("TransbordoIntegracao/ReenviarTodos", { Transbordo: _integracaoTransbordo.Transbordo.val(), Tipo: _integracaoTransbordo.Tipo.val(), Situacao: _integracaoTransbordo.Situacao.val() }, function (r) {
            if (r.Success) {
                exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, Localization.Resources.Gerais.Geral.ReenvioSolicitadoComSucesso);
                _gridIntegracaoTransbordo.CarregarGrid();
            } else {
                exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, r.Msg);
            }
        });
    });
}

function FinalizarEtapaClick() {
    exibirConfirmacao("Atenção!", Localization.Resources.Cargas.Transbordo.DesejaRealmenteFinalizarAEtapa, function () {
        executarReST("Transbordo/FinalizarIntegracao", { Transbordo: _integracaoTransbordo.Transbordo.val(), Tipo: _integracaoTransbordo.Tipo.val(), Situacao: _integracaoTransbordo.Situacao.val() }, function (r) {
            if (r.Success) {
                if (r.Data) {
                    exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, r.Msg);
                    _gridIntegracaoTransbordo.CarregarGrid();
                    _gridTransbordo.CarregarGrid();
                    Etapa2Aprovada();
                }
                else {
                    exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.Aviso, r.Msg);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, r.Msg);
            }
        });
    });
}


function ObterTotaisIntegracaoTransbordo() {
    executarReST("TransbordoIntegracao/ObterTotais", { Transbordo: _integracaoTransbordo.Transbordo.val() }, function (r) {
        if (r.Success) {
            _integracaoTransbordo.TotalGeral.val(r.Data.TotalGeral);
            _integracaoTransbordo.TotalAguardandoIntegracao.val(r.Data.TotalAguardandoIntegracao);
            _integracaoTransbordo.TotalAguardandoRetorno.val(r.Data.TotalAguardandoRetorno);
            _integracaoTransbordo.TotalProblemaIntegracao.val(r.Data.TotalProblemaIntegracao);
            _integracaoTransbordo.TotalIntegrado.val(r.Data.TotalIntegrado);

            if (r.Data.TotalProblemaIntegracao == r.Data.TotalGeral || r.Data.TotalGeral == 0)
                _integracaoTransbordo.FinalizarEtapa.visible(true);

        } else {
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, r.Msg);
        }
    });
}

function ExibirHistoricoIntegracaoTransbordo(integracao) {
    BuscarHistoricoIntegracaoTransbordo(integracao);
    Global.abrirModal("divModalHistoricoIntegracaoTransbordo");
}

function BuscarHistoricoIntegracaoTransbordo(integracao) {
    _pesquisaHistoricoIntegracaoTransbordo = new PesquisaHistoricoIntegracaoTransbordo();
    _pesquisaHistoricoIntegracaoTransbordo.Codigo.val(integracao.Codigo);

    var download = { descricao: Localization.Resources.Gerais.Geral.DownloadArquivos, id: guid(), evento: "onclick", metodo: DownloadArquivosHistoricoIntegracaoTransbordo, tamanho: "20", icone: "" };

    var menuOpcoes = {
        tipo: TypeOptionMenu.link,
        opcoes: [download]
    };

    _gridHistoricoIntegracaoTransbordo = new GridView("tblHistoricoIntegracaoTransbordo", "TransbordoIntegracao/ConsultarHistoricoIntegracao", _pesquisaHistoricoIntegracaoTransbordo, menuOpcoes, { column: 1, dir: orderDir.desc });
    _gridHistoricoIntegracaoTransbordo.CarregarGrid();
}

function DownloadArquivosHistoricoIntegracaoTransbordo(historicoConsulta) {
    executarDownload("TransbordoIntegracao/DownloadArquivosHistoricoIntegracao", { Codigo: historicoConsulta.Codigo });
}