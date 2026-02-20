
var _tipoIntegracaoLote = new Array();
var _gridIntegracaoLote;
var _gridHistoricoIntegracaoLote;
var _integracaoLote;
var _pesquisaHistoricoIntegracaoLote;

var _situacaoIntegracaoLote = [{ value: "", text: "Todas" },
{ value: EnumSituacaoIntegracaoCarga.AgIntegracao, text: "Aguardando Integração" },
{ value: EnumSituacaoIntegracaoCarga.AgRetorno, text: "Aguardando Retorno" },
{ value: EnumSituacaoIntegracaoCarga.Integrado, text: "Integrado" },
{ value: EnumSituacaoIntegracaoCarga.ProblemaIntegracao, text: "Falha na Integração" }];

var PesquisaHistoricoIntegracaoLote = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
}

var IntegracaoLote = function () {

    this.LoteEscrituracao = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Tipo = PropertyEntity({ val: ko.observable(""), options: _tipoIntegracaoLote, text: "Integração:", def: "", issue: 267, visible: ko.observable(false) });
    this.Situacao = PropertyEntity({ val: ko.observable(""), options: _situacaoIntegracaoLote, text: "Situação:", def: "", issue: 272 });

    this.TotalGeral = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int, text: "Total Geral:" });
    this.TotalAguardandoIntegracao = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int, text: "Aguardando Integração:" });
    this.TotalAguardandoRetorno = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int, text: "Aguardando Retorno:" });
    this.TotalProblemaIntegracao = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int, text: "Problemas na Integração:" });
    this.TotalIntegrado = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int, text: "Integrados:" });

    this.Pesquisar = PropertyEntity({
        eventClick: function (e) {
            _gridIntegracaoLote.CarregarGrid();
            ObterTotaisIntegracaoLote();
        }, type: types.event, text: "Pesquisar", idGrid: guid(), visible: ko.observable(true)
    });

    this.ReenviarTodos = PropertyEntity({
        eventClick: function (e) {
            ReenviarTodosIntegracaoLote();
        }, type: types.event, text: "Reenviar Todos", idGrid: guid(), visible: ko.observable(false)
    });

    this.ObterTotais = PropertyEntity({
        eventClick: function (e) {
            ObterTotaisIntegracaoLote();
        }, type: types.event, text: "Obter Totais", idGrid: guid(), visible: ko.observable(true)
    });
}

function LoadIntegracaoLote(loteEscrituracao, idKnockoutIntegracaoLote) {

    _integracaoLote = new IntegracaoLote();
    _integracaoLote.LoteEscrituracao.val(loteEscrituracao.Codigo.val());

    KoBindings(_integracaoLote, idKnockoutIntegracaoLote);

    ObterTotaisIntegracaoLote();
    ConfigurarPesquisaIntegracaoLote();


    //if (VerificaSePossuiPermissaoEspecial(EnumPermissaoPersonalizada.Carga_ReenviarIntegracoes, _PermissoesPersonalizadasCarga))
    _integracaoLote.ReenviarTodos.visible(true);

}

function ConfigurarPesquisaIntegracaoLote() {

    var menuOpcoes = { tipo: TypeOptionMenu.list, descricao: "Opções", tamanho: 7, opcoes: [] };

    menuOpcoes.opcoes.push({ descricao: "Reenviar", id: guid(), metodo: ReenviarIntegracaoLote, tamanho: "20", icone: "", visibilidade: VisibilidadeOpcaoReenviarIntegracao });
    menuOpcoes.opcoes.push({ descricao: "Histórico de Integração", id: guid(), metodo: ExibirHistoricoIntegracaoLote, tamanho: "20", icone: "", visibilidade: VisibilidadeOpcaoHistoricoIntegracao });

    _gridIntegracaoLote = new GridView(_integracaoLote.Pesquisar.idGrid, "LoteEscrituracaoIntegracaoLote/Pesquisa", _integracaoLote, menuOpcoes);
    _gridIntegracaoLote.CarregarGrid();
}

function VisibilidadeOpcaoReenviarIntegracao(data) {
    if (data.SituacaoIntegracao != EnumSituacaoIntegracao.ProblemaIntegracao)
        return false;

    return true;
}

function VisibilidadeOpcaoHistoricoIntegracao(data) {
    if (data.Tipo == EnumTipoIntegracao.FTP)
        return false;

    return true;
}

function ReenviarIntegracaoLote(data) {
    executarReST("LoteEscrituracaoIntegracaoLote/Reenviar", { Codigo: data.Codigo }, function (r) {
        if (r.Success) {
            exibirMensagem(tipoMensagem.ok, "Sucesso!", "Reenvio solicitado com sucesso.");
            _gridIntegracaoLote.CarregarGrid();
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha!", r.Msg);
        }
    });
}

function ReenviarTodosIntegracaoLote() {
    exibirConfirmacao("Atenção!", "Deseja realmente reenviar todas as integrações do lote?", function () {
        executarReST("LoteEscrituracaoIntegracaoLote/ReenviarTodos", { LoteEscrituracao: _integracaoLote.LoteEscrituracao.val(), Tipo: _integracaoLote.Tipo.val(), Situacao: _integracaoLote.Situacao.val() }, function (r) {
            if (r.Success) {
                exibirMensagem(tipoMensagem.ok, "Sucesso!", "Reenvio solicitado com sucesso.");
                _gridIntegracaoLote.CarregarGrid();
            } else {
                exibirMensagem(tipoMensagem.falha, "Falha!", r.Msg);
            }
        });
    });
}

function ObterTotaisIntegracaoLote() {
    executarReST("LoteEscrituracaoIntegracaoLote/ObterTotais", { LoteEscrituracao: _integracaoLote.LoteEscrituracao.val() }, function (r) {
        if (r.Success) {
            _integracaoLote.TotalGeral.val(r.Data.TotalGeral);
            _integracaoLote.TotalAguardandoIntegracao.val(r.Data.TotalAguardandoIntegracao);
            _integracaoLote.TotalAguardandoRetorno.val(r.Data.TotalAguardandoRetorno);
            _integracaoLote.TotalProblemaIntegracao.val(r.Data.TotalProblemaIntegracao);
            _integracaoLote.TotalIntegrado.val(r.Data.TotalIntegrado);
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha!", r.Msg);
        }
    });
}

function ExibirHistoricoIntegracaoLote(integracao) {
    BuscarHistoricoIntegracaoLote(integracao);
    Global.abrirModal('divModalHistoricoIntegracaoLote');
}

function BuscarHistoricoIntegracaoLote(integracao) {
    _pesquisaHistoricoIntegracaoLote = new PesquisaHistoricoIntegracaoLote();
    _pesquisaHistoricoIntegracaoLote.Codigo.val(integracao.Codigo);

    var download = { descricao: "Download Arquivos", id: guid(), evento: "onclick", metodo: DownloadArquivosHistoricoIntegracaoLote, tamanho: "20", icone: "" };

    var menuOpcoes = {
        tipo: TypeOptionMenu.link,
        opcoes: [download]
    };

    _gridHistoricoIntegracaoLote = new GridView("tblHistoricoIntegracaoLote", "LoteEscrituracaoIntegracaoLote/ConsultarHistoricoIntegracao", _pesquisaHistoricoIntegracaoLote, menuOpcoes, { column: 1, dir: orderDir.desc });
    _gridHistoricoIntegracaoLote.CarregarGrid();
}

function DownloadArquivosHistoricoIntegracaoLote(historicoConsulta) {
    executarDownload("LoteEscrituracaoIntegracaoLote/DownloadArquivosHistoricoIntegracao", { Codigo: historicoConsulta.Codigo });
}

//function RecarregarIntegracaoLoteViaSignalR(knoutCarga) {
//    if (_cargaAtual != null && _cargaAtual.DivCarga.id == knoutCarga.DivCarga.id) {
//        if ($("#divIntegracaoLote_" + _cargaAtual.EtapaIntegracao.idGrid).is(":visible") && _gridIntegracaoLote != null) {
//            _gridIntegracaoLote.CarregarGrid();
//            ObterTotaisIntegracaoLote();
//        }
//    }
//}