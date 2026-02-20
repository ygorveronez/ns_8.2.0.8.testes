/// <reference path="../../Enumeradores/EnumSituacaoIntegracaoCargaMDFeManual.js" />


var _tipoIntegracaoMDFeManual = new Array();
var _gridIntegracaoMDFeManual;
var _gridHistoricoIntegracaoMDFeManual;
var _integracaoMDFeManual;
var _pesquisaHistoricoIntegracaoMDFeManual;

var _situacaoIntegracaoMDFeManual = [
    { value: "", text: Localization.Resources.Gerais.Geral.Todas },
    { value: EnumSituacaoIntegracaoCargaMDFeManual.AgIntegracao, text: Localization.Resources.Gerais.Geral.AguardandoIntegracao },
    { value: EnumSituacaoIntegracaoCargaMDFeManual.AgRetorno, text: Localization.Resources.Gerais.Geral.AguardandoRetorno },
    { value: EnumSituacaoIntegracaoCargaMDFeManual.Integrado, text: Localization.Resources.Gerais.Geral.Integrado },
    { value: EnumSituacaoIntegracaoCargaMDFeManual.ProblemaIntegracao, text: Localization.Resources.Gerais.Geral.Falha }
];

var PesquisaHistoricoIntegracaoMDFeManual = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
}

var IntegracaoMDFeManual = function () {

    this.CargaMDFeManual = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Tipo = PropertyEntity({ val: ko.observable(""), options: _tipoIntegracaoMDFeManual, text: Localization.Resources.Gerais.Geral.Integracao.getFieldDescription(), def: "", issue: 267, visible: ko.observable(false) });
    this.Situacao = PropertyEntity({ val: ko.observable(""), options: _situacaoIntegracaoMDFeManual, text: Localization.Resources.Gerais.Geral.Situacao.getFieldDescription(), def: "", issue: 272 });

    this.TotalGeral = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int, text: Localization.Resources.Gerais.Geral.Total.getFieldDescription() });
    this.TotalAguardandoIntegracao = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int, text: Localization.Resources.Gerais.Geral.AguardandoIntegracao.getFieldDescription() });
    this.TotalAguardandoRetorno = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int, text: Localization.Resources.Gerais.Geral.AguardandoRetorno.getFieldDescription() });
    this.TotalProblemaIntegracao = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int, text: Localization.Resources.Gerais.Geral.ProblemasNaIntegracao.getFieldDescription() });
    this.TotalIntegrado = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int, text: Localization.Resources.Gerais.Geral.Integrado.getFieldDescription() });

    this.Pesquisar = PropertyEntity({
        eventClick: function (e) {
            _gridIntegracaoMDFeManual.CarregarGrid();
            ObterTotaisIntegracaoMDFeManual();
        }, type: types.event, text: Localization.Resources.Gerais.Geral.Pesquisar, idGrid: guid(), visible: ko.observable(true)
    });

    this.ReenviarTodos = PropertyEntity({
        eventClick: function (e) {
            ReenviarTodosIntegracaoMDFeManual();
        }, type: types.event, text: Localization.Resources.Gerais.Geral.ReenviarTodos, idGrid: guid(), visible: ko.observable(true)
    });

    this.ObterTotais = PropertyEntity({
        eventClick: function (e) {
            ObterTotaisIntegracaoMDFeManual();
        }, type: types.event, text: Localization.Resources.Gerais.Geral.ObterTotais, idGrid: guid(), visible: ko.observable(true)
    });
}

function LoadIntegracaoMDFeManual(MDFeManual, idKnockoutIntegracaoMDFe) {

    _integracaoMDFeManual = new IntegracaoMDFeManual();
    _integracaoMDFeManual.CargaMDFeManual.val(MDFeManual.Codigo.val());

    KoBindings(_integracaoMDFeManual, idKnockoutIntegracaoMDFe);

    ObterTotaisIntegracaoMDFeManual();
    ConfigurarPesquisaIntegracaoMDFeManual();
}

function ConfigurarPesquisaIntegracaoMDFeManual() {

    var menuOpcoes = { tipo: TypeOptionMenu.list, descricao: Localization.Resources.Gerais.Geral.Opcoes, tamanho: 7, opcoes: [] };

    menuOpcoes.opcoes.push({ descricao: Localization.Resources.Gerais.Geral.Reenviar, id: guid(), metodo: ReenviarIntegracaoMDFeManual, tamanho: "20", icone: "", visibilidade: VisibilidadeOpcaoReenviarIntegracaoMDFeManual });
    menuOpcoes.opcoes.push({ descricao: Localization.Resources.Gerais.Geral.HistoricoIntegracao, id: guid(), metodo: ExibirHistoricoIntegracaoMDFeManual, tamanho: "20", icone: "", visibilidade: VisibilidadeOpcaoHistoricoIntegracaoMDFeManual });

    _gridIntegracaoMDFeManual = new GridView(_integracaoMDFeManual.Pesquisar.idGrid, "CargaMDFeManualIntegracao/Pesquisa", _integracaoMDFeManual, menuOpcoes);
    _gridIntegracaoMDFeManual.CarregarGrid();
}

function VisibilidadeOpcaoReenviarIntegracaoMDFeManual(data) {
    if (data.Tipo == EnumTipoIntegracao.Natura || data.SituacaoIntegracao != EnumSituacaoIntegracao.ProblemaIntegracao)
        return false;

    return true;
}

function VisibilidadeOpcaoHistoricoIntegracaoMDFeManual(data) {
    if (data.Tipo == EnumTipoIntegracao.FTP)
        return false;

    return true;
}

function ObterTiposIntegracaoMDFeManual() {
    var p = new promise.Promise();

    executarReST("TipoIntegracao/BuscarTodos", {
        Tipos: JSON.stringify([
            EnumTipoIntegracao.Avior,
            EnumTipoIntegracao.Avon,
            EnumTipoIntegracao.Natura,
            EnumTipoIntegracao.FTP,
            EnumTipoIntegracao.Michelin])
    }, function (r) {
        if (r.Success) {

            _tipoIntegracaoMDFeManual.push({ value: "", text: Localization.Resources.Gerais.Geral.Todas });

            for (var i = 0; i < r.Data.length; i++)
                _tipoIntegracaoMDFeManual.push({ value: r.Data[i].Codigo, text: r.Data[i].Descricao });

        } else {
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, r.Msg);
        }

        p.done();
    });

    return p;
}

function ReenviarIntegracaoMDFeManual(data) {
    executarReST("CargaMDFeManualIntegracao/Reenviar", { Codigo: data.Codigo }, function (r) {
        if (r.Success) {
            exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, Localization.Resources.Gerais.Geral.ReenvioSolicitadoComSucesso);
            _gridIntegracaoMDFeManual.CarregarGrid();
        } else {
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, r.Msg);
        }
    });
}

function ReenviarTodosIntegracaoMDFeManual() {
    exibirConfirmacao("Atenção!", "Deseja reenviar integrações deste MDF-e manual?", function () {
        executarReST("CargaMDFeManualIntegracao/ReenviarTodos", { CargaMDFeManual: _integracaoMDFeManual.CargaMDFeManual.val(), Situacao: _integracaoMDFeManual.Situacao.val() }, function (r) {
            if (r.Success) {
                exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, Localization.Resources.Gerais.Geral.ReenvioSolicitadoComSucesso);
                _gridIntegracaoMDFeManual.CarregarGrid();
            } else {
                exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, r.Msg);
            }
        });
    });
}

function ObterTotaisIntegracaoMDFeManual() {
    executarReST("CargaMDFeManualIntegracao/ObterTotais", { CargaMDFeManual: _integracaoMDFeManual.CargaMDFeManual.val() }, function (r) {
        if (r.Success) {
            _integracaoMDFeManual.TotalGeral.val(r.Data.TotalGeral);
            _integracaoMDFeManual.TotalAguardandoIntegracao.val(r.Data.TotalAguardandoIntegracao);
            _integracaoMDFeManual.TotalAguardandoRetorno.val(r.Data.TotalAguardandoRetorno);
            _integracaoMDFeManual.TotalProblemaIntegracao.val(r.Data.TotalProblemaIntegracao);
            _integracaoMDFeManual.TotalIntegrado.val(r.Data.TotalIntegrado);
        } else {
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, r.Msg);
        }
    });
}

function ExibirHistoricoIntegracaoMDFeManual(integracao) {
    BuscarHistoricoIntegracaoMDFeManual(integracao);
    Global.abrirModal("divModalHistoricoIntegracao");
}

function BuscarHistoricoIntegracaoMDFeManual(integracao) {
    _pesquisaHistoricoIntegracaoMDFeManual = new PesquisaHistoricoIntegracaoMDFeManual();
    _pesquisaHistoricoIntegracaoMDFeManual.Codigo.val(integracao.Codigo);

    var download = { descricao: Localization.Resources.Gerais.Geral.DownloadArquivos, id: guid(), evento: "onclick", metodo: DownloadArquivosHistoricoIntegracaoMDFeManual, tamanho: "20", icone: "" };

    var menuOpcoes = {
        tipo: TypeOptionMenu.link,
        opcoes: [download]
    };

    _gridHistoricoIntegracaoMDFeManual = new GridView("tblHistoricoIntegracao", "CargaMDFeManualIntegracao/ConsultarHistoricoIntegracao", _pesquisaHistoricoIntegracaoMDFeManual, menuOpcoes, { column: 1, dir: orderDir.desc });
    _gridHistoricoIntegracaoMDFeManual.CarregarGrid();
}

function DownloadArquivosHistoricoIntegracaoMDFeManual(historicoConsulta) {
    executarDownload("CargaMDFeManualIntegracao/DownloadArquivosHistoricoIntegracao", { Codigo: historicoConsulta.Codigo });
}