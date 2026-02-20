//*******MAPEAMENTO KNOUCKOUT*******
var _pesquisaIntegradoraIntegracaoRetorno;
var _gridIntegradoraIntegracaoRetorno;

var _pesquisaHistoricoIntegracaoRetorno;
var _gridHistoricoIntegracaoRetorno;

var PesquisaHistoricoIntegracaoRetorno = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
}

var PesquisaIntegradoraIntegracaoRetorno = function () {

    this.NumeroIdentificacao = PropertyEntity({ text: "Nº de Identificação:", def: "", val: ko.observable(""), getType: typesKnockout.string, maxlength: 50 });
    this.DataInicial = PropertyEntity({ text: "Data Inicial:", val: ko.observable(Global.DataAtual()), getType: typesKnockout.dateTime });
    this.DataFinal = PropertyEntity({ text: "Data Final:", getType: typesKnockout.dateTime });
    this.Sucesso = PropertyEntity({ text: "Sucesso:", val: ko.observable(""), def: "", options: Global.ObterOpcoesPesquisaBooleano("Sim", "Não"), getType: typesKnockout.bool });
    this.PossuiCarga = PropertyEntity({ text: "Possui Carga:", val: ko.observable(""), def: "", options: Global.ObterOpcoesPesquisaBooleano("Sim", "Não"), getType: typesKnockout.bool });
    this.Integradora = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Integradora:", idBtnSearch: guid(), issue: 0, visible: ko.observable(true) });

    this.DataInicial.dateRangeLimit = this.DataFinal;
    this.DataFinal.dateRangeInit = this.DataInicial;

    this.Pesquisar = PropertyEntity({
        eventClick: function (e) {
            _gridIntegradoraIntegracaoRetorno.CarregarGrid();
        }, type: types.event, text: "Pesquisar", idGrid: guid(), visible: ko.observable(true)
    });
};

//*******EVENTOS*******

function LoadIntegradoraIntegracaoRetorno() {
    _pesquisaIntegradoraIntegracaoRetorno = new PesquisaIntegradoraIntegracaoRetorno();
    KoBindings(_pesquisaIntegradoraIntegracaoRetorno, "knockoutIntegradoraIntegracaoRetorno", _pesquisaIntegradoraIntegracaoRetorno.Pesquisar.id);

    new BuscarIntegradora(_pesquisaIntegradoraIntegracaoRetorno.Integradora);

    BuscarIntegradoraIntegracaoRetorno();
}

//*******MÉTODOS*******

function DownloadArquivosIntegradoraIntegracaoRetorno(integracao) {
    executarDownload("IntegradoraIntegracaoRetorno/DownloadArquivosIntegracao", { Codigo: integracao.Codigo });
}

function ExibirHistoricoIntegracaoRetorno(integracao) {
    BuscarHistoricoIntegracaoRetorno(integracao);
    Global.abrirModal("divModalHistoricoIntegracaoRetorno");
}

function BuscarHistoricoIntegracaoRetorno(integracao) {
    _pesquisaHistoricoIntegracaoRetorno = new PesquisaHistoricoIntegracaoRetorno();
    _pesquisaHistoricoIntegracaoRetorno.Codigo.val(integracao.Codigo);

    var download = { descricao: "Download", id: guid(), evento: "onclick", metodo: DownloadArquivosHistoricoIntegracaoRetorno, tamanho: "20", icone: "" };

    var menuOpcoes = {
        tipo: TypeOptionMenu.link,
        opcoes: [download]
    };

    _gridHistoricoIntegracaoRetorno = new GridView("tblHistoricoIntegracaoRetorno", "IntegradoraIntegracaoRetorno/ConsultarHistoricoIntegracaoRetorno", _pesquisaHistoricoIntegracaoRetorno, menuOpcoes, { column: 1, dir: orderDir.desc });
    _gridHistoricoIntegracaoRetorno.CarregarGrid();
}

function DownloadArquivosHistoricoIntegracaoRetorno(historicoConsulta) {
    executarDownload("IntegradoraIntegracaoRetorno/DownloadArquivosHistoricoIntegracaoRetorno", { Codigo: historicoConsulta.Codigo });
}

function ReenviarIntegracaoRetorno(integracao) {
    var data = {
        Codigo: integracao.Codigo
    }

    executarReST("IntegradoraIntegracaoRetorno/ReenviarIntegracaoRetorno", data, function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                ExibirHistoricoIntegracaoRetorno(integracao);
            } else {
                exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Atencao, arg.Msg, 20000);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, arg.Msg);
        }
    });
}

function BuscarIntegradoraIntegracaoRetorno() {
    var menuOpcoes = { tipo: TypeOptionMenu.list, opcoes: [{ id: guid(), descricao: "Download Arquivos", metodo: DownloadArquivosIntegradoraIntegracaoRetorno, tamanho: "15", icone: "" }] };
    menuOpcoes.opcoes.push({ id: guid(), descricao: "Histórico Retorno", metodo: ExibirHistoricoIntegracaoRetorno, tamanho: "15", icone: "" });
    menuOpcoes.opcoes.push({ id: guid(), descricao: "Reenviar integração retorno", metodo: ReenviarIntegracaoRetorno, tamanho: "15", icone: "" });

    var configExportacao = {
        url: "IntegradoraIntegracaoRetorno/ExportarPesquisa",
        titulo: "Retornos de Integrações"
    };

    _gridIntegradoraIntegracaoRetorno = new GridView(_pesquisaIntegradoraIntegracaoRetorno.Pesquisar.idGrid, "IntegradoraIntegracaoRetorno/Pesquisa", _pesquisaIntegradoraIntegracaoRetorno, menuOpcoes, { column: 2, dir: orderDir.desc }, null, null, null, null, null, null, null, configExportacao);
    _gridIntegradoraIntegracaoRetorno.CarregarGrid();
}