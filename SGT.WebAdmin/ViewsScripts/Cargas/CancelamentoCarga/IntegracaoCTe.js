var _tipoIntegracaoCancelamentoCTe = new Array();
var _gridIntegracaoCancelamentoCTe;
var _gridHistoricoIntegracaoCancelamentoCTe;
var _integracaoCancelamentoCTe;
var _pesquisaHistoricoIntegracaoCancelamentoCTe;

var _situacaoIntegracaoCancelamentoCTe = [
    { value: "", text: Localization.Resources.Gerais.Geral.Todas },
    { value: EnumSituacaoIntegracaoCarga.AgIntegracao, text: Localization.Resources.Gerais.Geral.AguardandoIntegracao},
    { value: EnumSituacaoIntegracaoCarga.AgRetorno, text: Localization.Resources.Gerais.Geral.AguardandoRetorno },
    { value: EnumSituacaoIntegracaoCarga.Integrado, text: Localization.Resources.Gerais.Geral.Integrado },
    { value: EnumSituacaoIntegracaoCarga.ProblemaIntegracao, text: Localization.Resources.Gerais.Geral.Falha }
];

var PesquisaHistoricoIntegracaoCancelamentoCTe = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
}

var IntegracaoCancelamentoCTe = function () {

    this.CargaCancelamento = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Tipo = PropertyEntity({ val: ko.observable(""), options: _tipoIntegracaoCancelamentoCTe, text: Localization.Resources.Gerais.Geral.Integracao.getFieldDescription(), def: "", issue: 267, visible: ko.observable(false) });
    this.Situacao = PropertyEntity({ val: ko.observable(""), options: _situacaoIntegracaoCancelamentoCTe, text: Localization.Resources.Gerais.Geral.Situacao.getFieldDescription(), def: "", issue: 272 });

    this.TotalGeral = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int, text: Localization.Resources.Gerais.Geral.Total.getFieldDescription() });
    this.TotalAguardandoIntegracao = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int, text: Localization.Resources.Gerais.Geral.AguardandoIntegracao.getFieldDescription() });
    this.TotalAguardandoRetorno = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int, text: Localization.Resources.Gerais.Geral.AguardandoRetorno.getFieldDescription() });
    this.TotalProblemaIntegracao = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int, text: Localization.Resources.Gerais.Geral.ProblemasNaIntegracao.getFieldDescription() });
    this.TotalIntegrado = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int, text: Localization.Resources.Gerais.Geral.Integrado.getFieldDescription() });

    this.Pesquisar = PropertyEntity({
        eventClick: function (e) {
            _gridIntegracaoCancelamentoCTe.CarregarGrid();
            ObterTotaisIntegracaoCancelamentoCTe();
        }, type: types.event, text: Localization.Resources.Gerais.Geral.Pesquisar, idGrid: guid(), visible: ko.observable(true)
    });

    this.ReenviarTodos = PropertyEntity({
        eventClick: function (e) {
            ReenviarTodosIntegracaoCancelamentoCTe();
        }, type: types.event, text: Localization.Resources.Gerais.Geral.ReenviarTodos, idGrid: guid(), visible: ko.observable(true)
    });

    this.ObterTotais = PropertyEntity({
        eventClick: function (e) {
            ObterTotaisIntegracaoCancelamentoCTe();
        }, type: types.event, text: Localization.Resources.Gerais.Geral.ObterTotais, idGrid: guid(), visible: ko.observable(true)
    });
}

function LoadIntegracaoCancelamentoCTe(cargaCancelamento, idKnockoutIntegracaoCTe) {

    _integracaoCancelamentoCTe = new IntegracaoCancelamentoCTe();
    _integracaoCancelamentoCTe.CargaCancelamento.val(cargaCancelamento.Codigo.val());

    KoBindings(_integracaoCancelamentoCTe, idKnockoutIntegracaoCTe);

    ObterTotaisIntegracaoCancelamentoCTe();
    ConfigurarPesquisaIntegracaoCancelamentoCTe();
}

function ConfigurarPesquisaIntegracaoCancelamentoCTe() {

    var menuOpcoes = { tipo: TypeOptionMenu.list, descricao: Localization.Resources.Gerais.Geral.Opcoes, tamanho: 7, opcoes: [] };

    menuOpcoes.opcoes.push({ descricao: Localization.Resources.Gerais.Geral.Reenviar, id: guid(), metodo: ReenviarIntegracaoCancelamentoCTe, tamanho: "20", icone: "", visibilidade: VisibilidadeOpcaoReenviarIntegracaoCancelamentoCTe });
    menuOpcoes.opcoes.push({ descricao: Localization.Resources.Gerais.Geral.HistoricoIntegracao, id: guid(), metodo: ExibirHistoricoIntegracaoCancelamentoCTe, tamanho: "20", icone: "", visibilidade: VisibilidadeOpcaoHistoricoIntegracaoCancelamentoCTe });

    _gridIntegracaoCancelamentoCTe = new GridView(_integracaoCancelamentoCTe.Pesquisar.idGrid, "CancelamentoCargaIntegracaoCargaCTe/Pesquisa", _integracaoCancelamentoCTe, menuOpcoes);
    _gridIntegracaoCancelamentoCTe.CarregarGrid();
}

function VisibilidadeOpcaoReenviarIntegracaoCancelamentoCTe(data) {
    if (data.Tipo == EnumTipoIntegracao.Natura || data.SituacaoIntegracao != EnumSituacaoIntegracao.ProblemaIntegracao)
        return false;

    if (data.Tipo == EnumTipoIntegracao.KMM && data.SituacaoIntegracao != EnumSituacaoIntegracao.ProblemaIntegracao)
        return false;

    return true;
}

function VisibilidadeOpcaoHistoricoIntegracaoCancelamentoCTe(data) {
    if (data.Tipo == EnumTipoIntegracao.FTP)
        return false;

    return true;
}

function ObterTiposIntegracaoCancelamentoCTe() {
    var p = new promise.Promise();

    executarReST("TipoIntegracao/BuscarTodos", {
        Tipos: JSON.stringify([
            EnumTipoIntegracao.Avior,
            EnumTipoIntegracao.Avon,
            EnumTipoIntegracao.Natura,
            EnumTipoIntegracao.FTP,
            EnumTipoIntegracao.Michelin ])
    }, function (r) {
        if (r.Success) {

            _tipoIntegracaoCancelamentoCTe.push({ value: "", text: Localization.Resources.Gerais.Geral.Todas });

            for (var i = 0; i < r.Data.length; i++)
                _tipoIntegracaoCancelamentoCTe.push({ value: r.Data[i].Codigo, text: r.Data[i].Descricao });

        } else {
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, r.Msg);
        }

        p.done();
    });

    return p;
}

function ReenviarIntegracaoCancelamentoCTe(data) {
    executarReST("CancelamentoCargaIntegracaoCargaCTe/Reenviar", { Codigo: data.Codigo }, function (r) {
        if (r.Success) {
            exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, Localization.Resources.Gerais.Geral.ReenvioSolicitadoComSucesso);
            _gridIntegracaoCancelamentoCTe.CarregarGrid();
        } else {
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, r.Msg);
        }
    });
}

function ReenviarTodosIntegracaoCancelamentoCTe() {
    exibirConfirmacao("Atenção!", Localization.Resources.Cargas.CancelamentoCarga.ReenviarIntegraçõesCancelamentoCTe, function () {
        executarReST("CancelamentoCargaIntegracaoCargaCTe/ReenviarTodos", { CargaCancelamento: _integracaoCancelamentoCTe.CargaCancelamento.val(), Tipo: _integracaoCancelamentoCTe.Tipo.val(), Situacao: _integracaoCancelamentoCTe.Situacao.val() }, function (r) {
            if (r.Success) {
                exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, Localization.Resources.Gerais.Geral.ReenvioSolicitadoComSucesso);
                _gridIntegracaoCancelamentoCTe.CarregarGrid();
            } else {
                exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, r.Msg);
            }
        });
    });
}

function ObterTotaisIntegracaoCancelamentoCTe() {
    executarReST("CancelamentoCargaIntegracaoCargaCTe/ObterTotais", { CargaCancelamento: _integracaoCancelamentoCTe.CargaCancelamento.val() }, function (r) {
        if (r.Success) {
            _integracaoCancelamentoCTe.TotalGeral.val(r.Data.TotalGeral);
            _integracaoCancelamentoCTe.TotalAguardandoIntegracao.val(r.Data.TotalAguardandoIntegracao);
            _integracaoCancelamentoCTe.TotalAguardandoRetorno.val(r.Data.TotalAguardandoRetorno);
            _integracaoCancelamentoCTe.TotalProblemaIntegracao.val(r.Data.TotalProblemaIntegracao);
            _integracaoCancelamentoCTe.TotalIntegrado.val(r.Data.TotalIntegrado);
        } else {
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, r.Msg);
        }
    });
}

function ExibirHistoricoIntegracaoCancelamentoCTe(integracao) {
    BuscarHistoricoIntegracaoCancelamentoCTe(integracao);
    Global.abrirModal("divModalHistoricoCancelamentoIntegracaoCTe");
}

function BuscarHistoricoIntegracaoCancelamentoCTe(integracao) {
    _pesquisaHistoricoIntegracaoCancelamentoCTe = new PesquisaHistoricoIntegracaoCancelamentoCTe();
    _pesquisaHistoricoIntegracaoCancelamentoCTe.Codigo.val(integracao.Codigo);

    var download = { descricao: Localization.Resources.Gerais.Geral.DownloadArquivos, id: guid(), evento: "onclick", metodo: DownloadArquivosHistoricoIntegracaoCancelamentoCTe, tamanho: "20", icone: "" };

    var menuOpcoes = {
        tipo: TypeOptionMenu.link,
        opcoes: [download]
    };

    _gridHistoricoIntegracaoCancelamentoCTe = new GridView("tblHistoricoIntegracaoCancelamentoCTe", "CancelamentoCargaIntegracaoCargaCTe/ConsultarHistoricoIntegracao", _pesquisaHistoricoIntegracaoCancelamentoCTe, menuOpcoes, { column: 1, dir: orderDir.desc });
    _gridHistoricoIntegracaoCancelamentoCTe.CarregarGrid();
}

function DownloadArquivosHistoricoIntegracaoCancelamentoCTe(historicoConsulta) {
    executarDownload("CancelamentoCargaIntegracaoCargaCTe/DownloadArquivosHistoricoIntegracao", { Codigo: historicoConsulta.Codigo });
}