
var _tipoIntegracaoCTe = new Array();
var _gridIntegracaoCTe;
var _gridHistoricoIntegracaoCTe;
var _integracaoCTe;
var _pesquisaHistoricoIntegracaoCTe;

var PesquisaHistoricoIntegracaoCTe = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
};

var IntegracaoCTe = function () {

    this.NFSManualCancelamento = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Tipo = PropertyEntity({ val: ko.observable(""), options: _tipoIntegracaoCTe, text: "Integração:", def: "", issue: 267, visible: ko.observable(false) });
    this.Situacao = PropertyEntity({ val: ko.observable(""), options: EnumSituacaoIntegracaoCarga.obterOpcoesPesquisa(), text: "Situação:", def: "", issue: 272 });

    this.TotalGeral = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int, text: "Total Geral:" });
    this.TotalAguardandoIntegracao = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int, text: "Aguardando Integração:" });
    this.TotalAguardandoRetorno = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int, text: "Aguardando Retorno:" });
    this.TotalProblemaIntegracao = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int, text: "Problemas na Integração:" });
    this.TotalIntegrado = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int, text: "Integrados:" });

    this.Pesquisar = PropertyEntity({
        eventClick: function (e) {
            _gridIntegracaoCTe.CarregarGrid();
            ObterTotaisIntegracaoCTe();
        }, type: types.event, text: "Pesquisar", idGrid: guid(), visible: ko.observable(true)
    });

    this.ReenviarTodos = PropertyEntity({
        eventClick: function (e) {
            ReenviarTodosIntegracaoCTe();
        }, type: types.event, text: "Reenviar Todos", idGrid: guid(), visible: ko.observable(false)
    });

    this.ObterTotais = PropertyEntity({
        eventClick: function (e) {
            ObterTotaisIntegracaoCTe();
        }, type: types.event, text: "Obter Totais", idGrid: guid(), visible: ko.observable(true)
    });
};

function LoadIntegracaoCTe(nfsManualCancelamento, idKnockoutIntegracaoCTe) {

    _integracaoCTe = new IntegracaoCTe();
    _integracaoCTe.NFSManualCancelamento.val(nfsManualCancelamento.Codigo.val());

    KoBindings(_integracaoCTe, idKnockoutIntegracaoCTe);

    ObterTotaisIntegracaoCTe();
    ConfigurarPesquisaIntegracaoCTe();


    //if (VerificaSePossuiPermissaoEspecial(EnumPermissaoPersonalizada.Carga_ReenviarIntegracoes, _PermissoesPersonalizadasCarga))
    _integracaoCTe.ReenviarTodos.visible(true);

}

function ConfigurarPesquisaIntegracaoCTe() {

    var menuOpcoes = { tipo: TypeOptionMenu.list, descricao: "Opções", tamanho: 7, opcoes: [] };

    menuOpcoes.opcoes.push({ descricao: "Reenviar", id: guid(), metodo: ReenviarIntegracaoCTe, tamanho: "20", icone: "", visibilidade: VisibilidadeOpcaoReenviarIntegracao });
    menuOpcoes.opcoes.push({ descricao: "Histórico de Integração", id: guid(), metodo: ExibirHistoricoIntegracaoCTe, tamanho: "20", icone: "", visibilidade: VisibilidadeOpcaoHistoricoIntegracao });

    _gridIntegracaoCTe = new GridView(_integracaoCTe.Pesquisar.idGrid, "NFSManualCancelamentoIntegracaoCTe/Pesquisa", _integracaoCTe, menuOpcoes);
    _gridIntegracaoCTe.CarregarGrid();
}

function VisibilidadeOpcaoReenviarIntegracao(data) {
    //if (data.Tipo == EnumTipoIntegracao.Natura || (!VerificaSePossuiPermissaoEspecial(EnumPermissaoPersonalizada.Carga_ReenviarIntegracoes, _PermissoesPersonalizadasCarga) && data.SituacaoIntegracao != EnumSituacaoIntegracao.ProblemaIntegracao))
    if (data.Tipo == EnumTipoIntegracao.Natura || data.SituacaoIntegracao != EnumSituacaoIntegracao.ProblemaIntegracao)
        return false;

    return true;
}

function VisibilidadeOpcaoHistoricoIntegracao(data) {
    if (data.Tipo == EnumTipoIntegracao.FTP)
        return false;

    return true;
}

function ObterTiposIntegracaoCTe() {
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

            _tipoIntegracaoCTe.push({ value: "", text: "Todas" });

            for (var i = 0; i < r.Data.length; i++)
                _tipoIntegracaoCTe.push({ value: r.Data[i].Codigo, text: r.Data[i].Descricao });

        } else {
            exibirMensagem(tipoMensagem.falha, "Falha!", r.Msg);
        }

        p.done();
    });

    return p;
}

function ReenviarIntegracaoCTe(data) {
    executarReST("NFSManualCancelamentoIntegracaoCTe/Reenviar", { Codigo: data.Codigo }, function (r) {
        if (r.Success) {
            exibirMensagem(tipoMensagem.ok, "Sucesso!", "Reenvio solicitado com sucesso.");
            _gridIntegracaoCTe.CarregarGrid();
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha!", r.Msg);
        }
    });
}

function ReenviarTodosIntegracaoCTe() {
    exibirConfirmacao("Atenção!", "Deseja realmente reenviar todas as integrações de CT-e?", function () {
        executarReST("NFSManualCancelamentoIntegracaoCTe/ReenviarTodos", { NFSManualCancelamento: _integracaoCTe.NFSManualCancelamento.val(), Tipo: _integracaoCTe.Tipo.val(), Situacao: _integracaoCTe.Situacao.val() }, function (r) {
            if (r.Success) {
                exibirMensagem(tipoMensagem.ok, "Sucesso!", "Reenvio solicitado com sucesso.");
                _gridIntegracaoCTe.CarregarGrid();
            } else {
                exibirMensagem(tipoMensagem.falha, "Falha!", r.Msg);
            }
        });
    });
}

function ObterTotaisIntegracaoCTe() {
    executarReST("NFSManualCancelamentoIntegracaoCTe/ObterTotais", { NFSManualCancelamento: _integracaoCTe.NFSManualCancelamento.val() }, function (r) {
        if (r.Success) {
            _integracaoCTe.TotalGeral.val(r.Data.TotalGeral);
            _integracaoCTe.TotalAguardandoIntegracao.val(r.Data.TotalAguardandoIntegracao);
            _integracaoCTe.TotalAguardandoRetorno.val(r.Data.TotalAguardandoRetorno);
            _integracaoCTe.TotalProblemaIntegracao.val(r.Data.TotalProblemaIntegracao);
            _integracaoCTe.TotalIntegrado.val(r.Data.TotalIntegrado);
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha!", r.Msg);
        }
    });
}

function ExibirHistoricoIntegracaoCTe(integracao) {
    BuscarHistoricoIntegracaoCTe(integracao);
    Global.abrirModal("divModalHistoricoIntegracaoCTe");
}

function BuscarHistoricoIntegracaoCTe(integracao) {
    _pesquisaHistoricoIntegracaoCTe = new PesquisaHistoricoIntegracaoCTe();
    _pesquisaHistoricoIntegracaoCTe.Codigo.val(integracao.Codigo);

    var download = { descricao: "Download Arquivos", id: guid(), evento: "onclick", metodo: DownloadArquivosHistoricoIntegracaoCTe, tamanho: "20", icone: "" };

    var menuOpcoes = {
        tipo: TypeOptionMenu.link,
        opcoes: [download]
    };

    _gridHistoricoIntegracaoCTe = new GridView("tblHistoricoIntegracaoCTe", "NFSManualCancelamentoIntegracaoCTe/ConsultarHistoricoIntegracao", _pesquisaHistoricoIntegracaoCTe, menuOpcoes, { column: 1, dir: orderDir.desc });
    _gridHistoricoIntegracaoCTe.CarregarGrid();
}

function DownloadArquivosHistoricoIntegracaoCTe(historicoConsulta) {
    executarDownload("NFSManualCancelamentoIntegracaoCTe/DownloadArquivosHistoricoIntegracao", { Codigo: historicoConsulta.Codigo });
}
