/// <reference path="../../Enumeradores/EnumSituacaoIntegracaoCargaCTeAgrupado.js" />


var _tipoIntegracaoCTeAgrupado = new Array();
var _gridIntegracaoCTeAgrupado;
var _gridHistoricoIntegracaoCTeAgrupado;
var _integracaoCTeAgrupado;
var _pesquisaHistoricoIntegracaoCTeAgrupado;

var _situacaoIntegracaoCTeAgrupado = [
    { value: "", text: Localization.Resources.Gerais.Geral.Todas },
    { value: EnumSituacaoIntegracaoCargaCTeAgrupado.AgIntegracao, text: Localization.Resources.Gerais.Geral.AguardandoIntegracao },
    { value: EnumSituacaoIntegracaoCargaCTeAgrupado.AgRetorno, text: Localization.Resources.Gerais.Geral.AguardandoRetorno },
    { value: EnumSituacaoIntegracaoCargaCTeAgrupado.Integrado, text: Localization.Resources.Gerais.Geral.Integrado },
    { value: EnumSituacaoIntegracaoCargaCTeAgrupado.ProblemaIntegracao, text: Localization.Resources.Gerais.Geral.Falha }
];

var PesquisaHistoricoIntegracaoCTeAgrupado = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
}

var IntegracaoCTeAgrupado = function () {

    this.CargaCTeAgrupado = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Tipo = PropertyEntity({ val: ko.observable(""), options: _tipoIntegracaoCTeAgrupado, text: Localization.Resources.Gerais.Geral.Integracao.getFieldDescription(), def: "", issue: 267, visible: ko.observable(false) });
    this.Situacao = PropertyEntity({ val: ko.observable(""), options: _situacaoIntegracaoCTeAgrupado, text: Localization.Resources.Gerais.Geral.Situacao.getFieldDescription(), def: "", issue: 272 });

    this.TotalGeral = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int, text: Localization.Resources.Gerais.Geral.Total.getFieldDescription() });
    this.TotalAguardandoIntegracao = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int, text: Localization.Resources.Gerais.Geral.AguardandoIntegracao.getFieldDescription() });
    this.TotalAguardandoRetorno = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int, text: Localization.Resources.Gerais.Geral.AguardandoRetorno.getFieldDescription() });
    this.TotalProblemaIntegracao = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int, text: Localization.Resources.Gerais.Geral.ProblemasNaIntegracao.getFieldDescription() });
    this.TotalIntegrado = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int, text: Localization.Resources.Gerais.Geral.Integrado.getFieldDescription() });

    this.Pesquisar = PropertyEntity({
        eventClick: function (e) {
            _gridIntegracaoCTeAgrupado.CarregarGrid();
            ObterTotaisIntegracaoCTeAgrupado();
        }, type: types.event, text: Localization.Resources.Gerais.Geral.Pesquisar, idGrid: guid(), visible: ko.observable(true)
    });

    this.ReenviarTodos = PropertyEntity({
        eventClick: function (e) {
            ReenviarTodosIntegracaoCTeAgrupado();
        }, type: types.event, text: Localization.Resources.Gerais.Geral.ReenviarTodos, idGrid: guid(), visible: ko.observable(true)
    });

    this.ObterTotais = PropertyEntity({
        eventClick: function (e) {
            ObterTotaisIntegracaoCTeAgrupado();
        }, type: types.event, text: Localization.Resources.Gerais.Geral.ObterTotais, idGrid: guid(), visible: ko.observable(true)
    });
}

function LoadIntegracaoCTeAgrupado(cteAgrupado, idKnockoutIntegracaoCTe) {

    _integracaoCTeAgrupado = new IntegracaoCTeAgrupado();
    _integracaoCTeAgrupado.CargaCTeAgrupado.val(cteAgrupado.Codigo.val());

    KoBindings(_integracaoCTeAgrupado, idKnockoutIntegracaoCTe);

    ObterTotaisIntegracaoCTeAgrupado();
    ConfigurarPesquisaIntegracaoCTeAgrupado();
}

function ConfigurarPesquisaIntegracaoCTeAgrupado() {

    var menuOpcoes = { tipo: TypeOptionMenu.list, descricao: Localization.Resources.Gerais.Geral.Opcoes, tamanho: 7, opcoes: [] };

    menuOpcoes.opcoes.push({ descricao: Localization.Resources.Gerais.Geral.Reenviar, id: guid(), metodo: ReenviarIntegracaoCTeAgrupado, tamanho: "20", icone: "", visibilidade: VisibilidadeOpcaoReenviarIntegracaoCTeAgrupado });
    menuOpcoes.opcoes.push({ descricao: Localization.Resources.Gerais.Geral.HistoricoIntegracao, id: guid(), metodo: ExibirHistoricoIntegracaoCTeAgrupado, tamanho: "20", icone: "", visibilidade: VisibilidadeOpcaoHistoricoIntegracaoCTeAgrupado });

    _gridIntegracaoCTeAgrupado = new GridView(_integracaoCTeAgrupado.Pesquisar.idGrid, "CargaCTeAgrupadoIntegracao/Pesquisa", _integracaoCTeAgrupado, menuOpcoes);
    _gridIntegracaoCTeAgrupado.CarregarGrid();
}

function VisibilidadeOpcaoReenviarIntegracaoCTeAgrupado(data) {
    if (data.Tipo == EnumTipoIntegracao.Natura || data.SituacaoIntegracao != EnumSituacaoIntegracao.ProblemaIntegracao)
        return false;

    return true;
}

function VisibilidadeOpcaoHistoricoIntegracaoCTeAgrupado(data) {
    if (data.Tipo == EnumTipoIntegracao.FTP)
        return false;

    return true;
}

function ObterTiposIntegracaoCTeAgrupado() {
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

            _tipoIntegracaoCTeAgrupado.push({ value: "", text: Localization.Resources.Gerais.Geral.Todas });

            for (var i = 0; i < r.Data.length; i++)
                _tipoIntegracaoCTeAgrupado.push({ value: r.Data[i].Codigo, text: r.Data[i].Descricao });

        } else {
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, r.Msg);
        }

        p.done();
    });

    return p;
}

function ReenviarIntegracaoCTeAgrupado(data) {
    executarReST("CargaCTeAgrupadoIntegracao/Reenviar", { Codigo: data.Codigo }, function (r) {
        if (r.Success) {
            exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, Localization.Resources.Gerais.Geral.ReenvioSolicitadoComSucesso);
            _gridIntegracaoCTeAgrupado.CarregarGrid();
        } else {
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, r.Msg);
        }
    });
}

function ReenviarTodosIntegracaoCTeAgrupado() {
    exibirConfirmacao("Atenção!", "Deseja reenviar integrações deste CT-ee Agrupado?", function () {
        executarReST("CargaCTeAgrupadoIntegracao/ReenviarTodos", { CargaCTeAgrupado: _integracaoCTeAgrupado.CargaCTeAgrupado.val(), Tipo: _integracaoCTeAgrupado.Tipo.val(), Situacao: _integracaoCTeAgrupado.Situacao.val() }, function (r) {
            if (r.Success) {
                exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, Localization.Resources.Gerais.Geral.ReenvioSolicitadoComSucesso);
                _gridIntegracaoCTeAgrupado.CarregarGrid();
            } else {
                exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, r.Msg);
            }
        });
    });
}

function ObterTotaisIntegracaoCTeAgrupado() {
    executarReST("CargaCTeAgrupadoIntegracao/ObterTotais", { CargaCTeAgrupado: _integracaoCTeAgrupado.CargaCTeAgrupado.val() }, function (r) {
        if (r.Success) {
            _integracaoCTeAgrupado.TotalGeral.val(r.Data.TotalGeral);
            _integracaoCTeAgrupado.TotalAguardandoIntegracao.val(r.Data.TotalAguardandoIntegracao);
            _integracaoCTeAgrupado.TotalAguardandoRetorno.val(r.Data.TotalAguardandoRetorno);
            _integracaoCTeAgrupado.TotalProblemaIntegracao.val(r.Data.TotalProblemaIntegracao);
            _integracaoCTeAgrupado.TotalIntegrado.val(r.Data.TotalIntegrado);
        } else {
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, r.Msg);
        }
    });
}

function ExibirHistoricoIntegracaoCTeAgrupado(integracao) {
    BuscarHistoricoIntegracaoCTeAgrupado(integracao);
    Global.abrirModal("divModalHistoricoCTeAgrupadoIntegracaoCTe");
}

function BuscarHistoricoIntegracaoCTeAgrupado(integracao) {
    _pesquisaHistoricoIntegracaoCTeAgrupado = new PesquisaHistoricoIntegracaoCTeAgrupado();
    _pesquisaHistoricoIntegracaoCTeAgrupado.Codigo.val(integracao.Codigo);

    var download = { descricao: Localization.Resources.Gerais.Geral.DownloadArquivos, id: guid(), evento: "onclick", metodo: DownloadArquivosHistoricoIntegracaoCTeAgrupado, tamanho: "20", icone: "" };

    var menuOpcoes = {
        tipo: TypeOptionMenu.link,
        opcoes: [download]
    };

    _gridHistoricoIntegracaoCTeAgrupado = new GridView("tblHistoricoIntegracaoCTeAgrupado", "CargaCTeAgrupadoIntegracao/ConsultarHistoricoIntegracao", _pesquisaHistoricoIntegracaoCTeAgrupado, menuOpcoes, { column: 1, dir: orderDir.desc });
    _gridHistoricoIntegracaoCTeAgrupado.CarregarGrid();
}

function DownloadArquivosHistoricoIntegracaoCTeAgrupado(historicoConsulta) {
    executarDownload("CargaCTeAgrupadoIntegracao/DownloadArquivosHistoricoIntegracao", { Codigo: historicoConsulta.Codigo });
}