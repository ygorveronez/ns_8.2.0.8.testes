var _gridIntegracaoEDI;
var _integracaoEDI;

var _situacaoIntegracaoEDI = [{ value: "", text: "Todas" },
{ value: EnumSituacaoIntegracaoCarga.AgIntegracao, text: "Aguardando Integração" },
{ value: EnumSituacaoIntegracaoCarga.Integrado, text: "Integrado" },
{ value: EnumSituacaoIntegracaoCarga.ProblemaIntegracao, text: "Falha na Integração" }];


var IntegracaoEDI = function () {

    this.LancamentoNFSManual = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Situacao = PropertyEntity({ val: ko.observable(""), options: _situacaoIntegracaoEDI, text: "Situação:", def: "", issue: 272 });

    this.TotalGeral = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int, text: "Total Geral:" });
    this.TotalAguardandoIntegracao = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int, text: "Aguardando Integração:" });
    this.TotalProblemaIntegracao = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int, text: "Problemas na Integração:" });
    this.TotalIntegrado = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int, text: "Integrados:" });

    this.Pesquisar = PropertyEntity({
        eventClick: function (e) {
            ObterTotaisIntegracaoEDI();
            _gridIntegracaoEDI.CarregarGrid();
        }, type: types.event, text: "Pesquisar", idGrid: guid(), visible: ko.observable(true)
    });

    this.ReenviarTodos = PropertyEntity({
        eventClick: function (e) {
            ReenviarTodosIntegracaoEDI();
        }, type: types.event, text: "Reenviar Todos", idGrid: guid(), visible: ko.observable(false)
    });

    this.ObterTotais = PropertyEntity({
        eventClick: function (e) {
            ObterTotaisIntegracaoEDI();
        }, type: types.event, text: "Obter Totais", idGrid: guid(), visible: ko.observable(true)
    });
}

function LoadIntegracaoEDI(nfsManual, idKnockoutIntegracaoEDI) {

    _integracaoEDI = new IntegracaoEDI();

    _integracaoEDI.LancamentoNFSManual.val(nfsManual.Codigo.val());

    KoBindings(_integracaoEDI, idKnockoutIntegracaoEDI);

    ObterTotaisIntegracaoEDI();
    ConfigurarPesquisaIntegracaoEDI();

    //if (VerificaSePossuiPermissaoEspecial(EnumPermissaoPersonalizada.Carga_ReenviarIntegracoes, _PermissoesPersonalizadasCarga))
    _integracaoEDI.ReenviarTodos.visible(true);
}

function ConfigurarPesquisaIntegracaoEDI() {
    var download = { descricao: "Download", id: guid(), metodo: DownloadIntegracaoEDI, tamanho: "20", icone: "" };
    var reenviar = { descricao: "Reenviar", id: guid(), metodo: ReenviarIntegracaoEDI, tamanho: "20", icone: "", visibilidade: VisibilidadeOpcaoReenviarIntegracaoEDI };

    var menuOpcoes = { tipo: TypeOptionMenu.list, descricao: "Opções", tamanho: 7, opcoes: [download, reenviar] };

    _gridIntegracaoEDI = new GridView(_integracaoEDI.Pesquisar.idGrid, "NFSManualIntegracaoEDI/Pesquisa", _integracaoEDI, menuOpcoes);

    _gridIntegracaoEDI.CarregarGrid();
}

function VisibilidadeOpcaoReenviarIntegracaoEDI(data) {
    //if (data.Tipo == EnumTipoIntegracao.NaoPossuiIntegracao || (!VerificaSePossuiPermissaoEspecial(EnumPermissaoPersonalizada.Carga_ReenviarIntegracoes, _PermissoesPersonalizadasCarga) && data.SituacaoIntegracao != EnumSituacaoIntegracao.ProblemaIntegracao))
    if (data.Tipo == EnumTipoIntegracao.NaoPossuiIntegracao || data.SituacaoIntegracao != EnumSituacaoIntegracao.ProblemaIntegracao)
        return false;
    return true;
}

function DownloadIntegracaoEDI(data) {
    executarDownload("NFSManualIntegracaoEDI/Download", { Codigo: data.Codigo });
}

function ReenviarIntegracaoEDI(data) {
    executarReST("NFSManualIntegracaoEDI/Reenviar", { Codigo: data.Codigo }, function (r) {
        if (r.Success) {
            exibirMensagem(tipoMensagem.ok, "Sucesso!", "Reenvio solicitado com sucesso.");
            _gridIntegracaoEDI.CarregarGrid();
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha!", r.Msg);
        }
    });
}

function ReenviarTodosIntegracaoEDI() {
    exibirConfirmacao("Atenção!", "Deseja realmente reenviar todas as integrações de EDI?", function () {
        executarReST("NFSManualIntegracaoEDI/ReenviarTodos", { LancamentoNFSManual: _integracaoEDI.LancamentoNFSManual.val(), Situacao: _integracaoEDI.Situacao.val() }, function (r) {
            if (r.Success) {
                exibirMensagem(tipoMensagem.ok, "Sucesso!", "Reenvio solicitado com sucesso.");
                _gridIntegracaoEDI.CarregarGrid();
            } else {
                exibirMensagem(tipoMensagem.falha, "Falha!", r.Msg);
            }
        });
    });
}

function ObterTotaisIntegracaoEDI() {
    executarReST("NFSManualIntegracaoEDI/ObterTotais", { LancamentoNFSManual: _integracaoEDI.LancamentoNFSManual.val() }, function (r) {
        if (r.Success) {
            _integracaoEDI.TotalGeral.val(r.Data.TotalGeral);
            _integracaoEDI.TotalAguardandoIntegracao.val(r.Data.TotalAguardandoIntegracao);
            _integracaoEDI.TotalProblemaIntegracao.val(r.Data.TotalProblemaIntegracao);
            _integracaoEDI.TotalIntegrado.val(r.Data.TotalIntegrado);
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha!", r.Msg);
        }
    });
}

//function RecarregarIntegracaoEDIViaSignalR(knoutCarga) {
//    if (_cargaAtual != null && _cargaAtual.DivCarga.id == knoutCarga.DivCarga.id) {
//        if ($("#divIntegracaoEDI_" + _cargaAtual.EtapaIntegracao.idGrid).is(":visible") && _gridIntegracaoEDI != null) {
//            _gridIntegracaoEDI.CarregarGrid();
//            ObterTotaisIntegracaoEDI();
//        }
//    }
//}