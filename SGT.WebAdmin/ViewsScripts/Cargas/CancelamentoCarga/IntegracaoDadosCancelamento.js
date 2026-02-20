/// <reference path="../../../js/libs/jquery-2.1.1.js" />
/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/bootstrap/bootstrap.js" />
/// <reference path="../../../js/libs/jquery.blockui.js" />
/// <reference path="../../../js/Global/knoutViewsSlides.js" />
/// <reference path="../../../js/libs/jquery.maskMoney.js" />
/// <reference path="../../../js/plugin/datatables/jquery.dataTables.js" />
/// <reference path="../../../ViewsScripts/Enumeradores/EnumTipoConsulta.js" />
/// <reference path="CancelamentoCarga.js" />
/// <reference path="EtapaCancelamento.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _integracaoDadosCancelamento;
var _gridDadosCancelamento;
var _gridHistoricoIntegracaoDadosCancelamento;
var _pesquisaHistoricoIntegracaoDadosCancelamento;

var IntegracaoDadosCancelamento = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });

    this.TipoConsultaDadosCancelamento = PropertyEntity({ val: ko.observable("-1"), options: EnumTipoConsulta.obterOpcoes(), text: "Consulta:", def: "-1", enable: ko.observable(true) });
    this.ConsultarDadosCancelamento = PropertyEntity({ eventClick: ConsultarDadosCancelamentoClick, type: types.event, text: ko.observable("Consultar"), visible: ko.observable(true), enable: ko.observable(true) });
    this.EnviarDadosCancelamento = PropertyEntity({ eventClick: ReenviarTodosDadosCancelamentoClick, type: types.event, text: ko.observable("Reenviar Todos"), visible: ko.observable(true), enable: ko.observable(true) });

    this.ArquivosDadosCancelamento = PropertyEntity({ type: types.map,  getType: typesKnockout.dynamic, idBtnSearch: guid(), idGrid: guid() });

    this.TotalGeral = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int, text: Localization.Resources.Gerais.Geral.Total.getFieldDescription() });
    this.TotalAguardandoIntegracao = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int, text: Localization.Resources.Gerais.Geral.AguardandoIntegracao.getFieldDescription() });
    this.TotalAguardandoRetorno = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int, text: Localization.Resources.Gerais.Geral.AguardandoRetorno.getFieldDescription() });
    this.TotalProblemaIntegracao = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int, text: Localization.Resources.Gerais.Geral.ProblemasNaIntegracao.getFieldDescription() });
    this.TotalIntegrado = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int, text: Localization.Resources.Gerais.Geral.Integrado.getFieldDescription() });
}

var PesquisaHistoricoIntegracaoDadosCancelamento = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
};

//*******EVENTOS*******

function LoadIntegracaoDadosCancelamento() {
    _integracaoDadosCancelamento = new IntegracaoDadosCancelamento();
    KoBindings(_integracaoDadosCancelamento, "knockoutIntegracaoDadosCancelamentoCarga");

    var reenviarDadosCancelamento = { descricao: Localization.Resources.Gerais.Geral.Reenviar, id: guid(), metodo: ReenviarDadosCancelamentoClick, icone: "" };
    var historicoIntegracaoDadosCancelamento = { descricao: "Histórico de Envio", id: guid(), metodo: ExibirHistoricoIntegracaoDadosCancelamento, tamanho: "20", icone: "" };
    var menuOpcoesDadosCancelamento = { tipo: TypeOptionMenu.list, descricao: Localization.Resources.Gerais.Geral.Opcoes, tamanho: 7, opcoes: [reenviarDadosCancelamento, historicoIntegracaoDadosCancelamento] };

    _gridDadosCancelamento = new GridView(_integracaoDadosCancelamento.ArquivosDadosCancelamento.idGrid, "CancelamentoCargaIntegracaoDadosCancelamento/Pesquisa", _integracaoDadosCancelamento, menuOpcoesDadosCancelamento, null, null, null);
    _gridDadosCancelamento.CarregarGrid();
}

function ReenviarDadosCancelamentoClick(e, sender) {
    exibirConfirmacao("Atenção", "Deseja realmente reenviar essa integração?", function () {
        executarReST("CancelamentoCargaIntegracaoDadosCancelamento/ReenviarDadosCancelamento", { Codigo: e.Codigo }, function (arg) {
            if (arg.Success) {
                if (arg.Data) {
                    _gridDadosCancelamento.CarregarGrid();
                    CarregarIntegracaoDadosCancelamento();
                } else {
                    exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.Atencao, arg.Msg);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, arg.Msg);
            }
        });
    });
}

function ConsultarDadosCancelamentoClick(e, sender) {
    _gridDadosCancelamento.CarregarGrid();
    CarregarIntegracaoDadosCancelamento();
}

function ReenviarTodosDadosCancelamentoClick(e, sender) {
    exibirConfirmacao("Atenção!", "Deseja realmente reenviar todas as integrações?", function () {
        executarReST("CancelamentoCargaIntegracaoDadosCancelamento/ReenviarTodosDadosCancelamento", { Codigo: _cancelamento.Codigo.val() }, function (arg) {
            if (arg.Success) {
                _gridDadosCancelamento.CarregarGrid();
                CarregarIntegracaoDadosCancelamento();
            } else {
                exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, arg.Msg);
            }
        });
    });
}

//*******MÉTODOS*******

function CarregarIntegracaoDadosCancelamento() {
    if (_cancelamento.Codigo.val() > 0 && _cancelamento.Codigo.val() != "") {
        executarReST("CancelamentoCargaIntegracaoDadosCancelamento/ObterTotais", { Codigo: _cancelamento.Codigo.val() }, function (e) {
            if (e.Success) {
                if (e.Data != null) {
                    var dataIntegracao = { Data: e.Data };
                    PreencherObjetoKnout(_integracaoDadosCancelamento, dataIntegracao);
                    _gridDadosCancelamento.CarregarGrid();
                }
            }
            else {
                exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, e.Msg);
            }
        });
    }
}

function LimparInegracaoDadosCancelamento() {
    LimparCampos(_integracaoDadosCancelamento);
    _gridDadosCancelamento.CarregarGrid();
}

function ExibirHistoricoIntegracaoDadosCancelamento(integracao) {
    BuscarHistoricoIntegracaoDadosCancelamento(integracao);
    Global.abrirModal("divModalHistoricoIntegracaoDadosCancelamento");
}

function BuscarHistoricoIntegracaoDadosCancelamento(integracao) {
    _pesquisaHistoricoIntegracaoDadosCancelamento = new PesquisaHistoricoIntegracaoDadosCancelamento();
    _pesquisaHistoricoIntegracaoDadosCancelamento.Codigo.val(integracao.Codigo);

    var download = { descricao: Localization.Resources.Gerais.Geral.DownloadArquivos, id: guid(), evento: "onclick", metodo: DownloadArquivosHistoricoIntegracaoDadosCancelamento, tamanho: "20", icone: "" };

    var menuOpcoes = {
        tipo: TypeOptionMenu.link,
        opcoes: [download]
    };

    _gridHistoricoIntegracaoDadosCancelamento = new GridView("tblHistoricoIntegracaoDadosCancelamento", "CancelamentoCargaIntegracaoDadosCancelamento/ConsultarHistoricoIntegracao", _pesquisaHistoricoIntegracaoDadosCancelamento, menuOpcoes, { column: 1, dir: orderDir.desc });
    _gridHistoricoIntegracaoDadosCancelamento.CarregarGrid();
}

function DownloadArquivosHistoricoIntegracaoDadosCancelamento(historicoConsulta) {
    executarDownload("CancelamentoCargaIntegracaoDadosCancelamento/DownloadArquivosHistoricoIntegracao", { Codigo: historicoConsulta.Codigo });
}

function BuscarIntegracoesDadosCancelamentoCarga() {
    _integracaoDadosCancelamento.Codigo.val(_cancelamento.Codigo.val());
    _gridDadosCancelamento.CarregarGrid();
}

function BuscarIntegracoesDadosCancelamento() {
    BuscarIntegracoesDadosCancelamentoCarga();
    BuscarIntegracoesDadosCancelamentoCTe();
}
