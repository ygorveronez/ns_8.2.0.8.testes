/// <reference path="../../../js/libs/jquery-2.1.1.js" />
/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/knockout-3.1.0.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/bootstrap/bootstrap.js" />
/// <reference path="../../../js/libs/jquery.blockui.js" />
/// <reference path="../../../js/Global/knoutViewsSlides.js" />
/// <reference path="../../../js/libs/jquery.maskMoney.js" />
/// <reference path="../../../js/plugin/datatables/jquery.dataTables.js" />
/// <reference path="loteintegracaocarregamento.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _tipoConsulta = [
    { text: "Todos", value: -1 },
    { text: "Aguardando Integração", value: 0 },
    { text: "Integrados", value: 1 },
    { text: "Falha na Integração", value: 2 }
];

var _integracaoLoteCarregamento;
var _gridArquivosLote;
var _gridHistoricoIntegracao;
var _pesquisaHistoricoIntegracao;

var LoteIntegracaoCarregamento = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });

    this.TipoConsultaLote = PropertyEntity({ val: ko.observable("-1"), options: EnumTipoConsulta.obterOpcoes(), text: "Consulta: ", def: "-1", enable: ko.observable(true) });
    this.ConsultarLote = PropertyEntity({ eventClick: ConsultarIntegracaoClick, type: types.event, text: ko.observable("Consultar"), visible: ko.observable(true), enable: ko.observable(true) });
    this.EnviarLote = PropertyEntity({ eventClick: ReenviarIntegracaoClick, type: types.event, text: ko.observable("Re-enviar todos"), visible: ko.observable(true), enable: ko.observable(true) });
    this.ArquivosLote = PropertyEntity({ type: types.map, required: false, text: "Carregamento's", getType: typesKnockout.dynamic, idBtnSearch: guid(), idGrid: guid() });
};

var PesquisaHistoricoIntegracao = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
};

//*******EVENTOS*******

function loadIntegracaoLoteCarregamentos() {
    _integracaoLoteCarregamento = new LoteIntegracaoCarregamento();
    KoBindings(_integracaoLoteCarregamento, "knockoutIntegracaoLoteCarregamento");

    var reenviarLote = { descricao: "Re-enviar", id: guid(), metodo: ReenviarIntegracaoClick, icone: "" };
    var historicoIntegracao = { descricao: "Histórico de Envio", id: guid(), metodo: ExibirHistoricoIntegracao, tamanho: "20", icone: "" };
    var menuOpcoes = { tipo: TypeOptionMenu.list, descricao: "Opções", tamanho: 7, opcoes: [reenviarLote, historicoIntegracao] };

    _gridArquivosLote = new GridView(_integracaoLoteCarregamento.ArquivosLote.idGrid, "LoteIntegracaoCarregamento/PesquisaPorIntegracaoLote", _integracaoLoteCarregamento, menuOpcoes, null, null, null);
}

function ReenviarIntegracaoClick(e, sender) {
    exibirConfirmacao("Atenção!", "Deseja realmente reenviar esta integração?", function () {
        executarReST("LoteIntegracaoCarregamento/Reenviar", { Codigo: e.Codigo }, function (arg) {
            if (arg.Success) {
                if (arg.Data) {
                    _gridArquivosLote.CarregarGrid();
                } else {
                    exibirMensagem(tipoMensagem.aviso, "Atenção", arg.Msg);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
            }
        });
    });
}

function ConsultarIntegracaoClick(e, sender) {
    _gridArquivosLote.CarregarGrid();
}

function ConsultaIntegracao() {
    setarEtapaIntegracao();
    _gridArquivosLote.CarregarGrid();
}

//*******MÉTODOS*******


function ExibirHistoricoIntegracao(integracao) {
    BuscarHistoricoIntegracaoLoteCarregamento(integracao);
    Global.abrirModal('divModalHistoricoIntegracao');
   
}

function BuscarHistoricoIntegracaoLoteCarregamento(integracao) {
    _pesquisaHistoricoIntegracao = new PesquisaHistoricoIntegracao();
    _pesquisaHistoricoIntegracao.Codigo.val(integracao.Codigo);

    var download = { descricao: "Download Arquivos", id: guid(), evento: "onclick", metodo: DownloadArquivosHistoricoIntegracao, tamanho: "20", icone: "" };

    var menuOpcoes = {
        tipo: TypeOptionMenu.link,
        opcoes: [download]
    };

    _gridHistoricoIntegracao = new GridView("tblHistoricoIntegracao", "LoteIntegracaoCarregamento/ConsultarHistoricoIntegracao", _pesquisaHistoricoIntegracao, menuOpcoes, { column: 1, dir: orderDir.desc });
    _gridHistoricoIntegracao.CarregarGrid();
}

function DownloadArquivosHistoricoIntegracao(historicoConsulta) {
    executarDownload("LoteIntegracaoCarregamento/DownloadArquivosHistoricoIntegracao", { Codigo: historicoConsulta.Codigo });
}

