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

var _integracaoDadosCancelamentoCTe;
var _gridDadosCancelamentoCTe;
var _gridHistoricoIntegracaoDadosCancelamentoCTe;
var _pesquisaHistoricoIntegracaoDadosCancelamentoCTe;

var IntegracaoDadosCancelamentoCTe = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });

    this.TipoConsultaDadosCancelamento = PropertyEntity({ val: ko.observable("-1"), options: EnumTipoConsulta.obterOpcoes(), text: "Consulta:", def: "-1", enable: ko.observable(true) });
    this.ConsultarDadosCancelamento = PropertyEntity({ eventClick: ConsultarDadosCancelamentoCTeClick, type: types.event, text: ko.observable("Consultar"), visible: ko.observable(true), enable: ko.observable(true) });
    this.EnviarDadosCancelamento = PropertyEntity({ eventClick: ReenviarTodosDadosCancelamentoCTeClick, type: types.event, text: ko.observable("Reenviar Todos"), visible: ko.observable(true), enable: ko.observable(true) });

    this.ArquivosDadosCancelamento = PropertyEntity({ type: types.map,  getType: typesKnockout.dynamic, idBtnSearch: guid(), idGrid: guid() });

    this.TotalGeral = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int, text: Localization.Resources.Gerais.Geral.Total.getFieldDescription() });
    this.TotalAguardandoIntegracao = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int, text: Localization.Resources.Gerais.Geral.AguardandoIntegracao.getFieldDescription() });
    this.TotalAguardandoRetorno = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int, text: Localization.Resources.Gerais.Geral.AguardandoRetorno.getFieldDescription() });
    this.TotalProblemaIntegracao = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int, text: Localization.Resources.Gerais.Geral.ProblemasNaIntegracao.getFieldDescription() });
    this.TotalIntegrado = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int, text: Localization.Resources.Gerais.Geral.Integrado.getFieldDescription() });
}

var PesquisaHistoricoIntegracaoDadosCancelamentoCTe = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
};

//*******EVENTOS*******

function LoadIntegracaoDadosCancelamentoCTe() {
    _integracaoDadosCancelamentoCTe = new IntegracaoDadosCancelamentoCTe();
    KoBindings(_integracaoDadosCancelamentoCTe, "knockoutIntegracaoDadosCancelamentoCTe");

    var reenviarDadosCancelamento = { descricao: Localization.Resources.Gerais.Geral.Reenviar, id: guid(), metodo: ReenviarDadosCancelamentoCTeClick, icone: "" };
    var historicoIntegracaoDadosCancelamento = { descricao: "Histórico de Envio", id: guid(), metodo: ExibirHistoricoIntegracaoDadosCancelamentoCTe, tamanho: "20", icone: "" };
    var menuOpcoesDadosCancelamento = { tipo: TypeOptionMenu.list, descricao: Localization.Resources.Gerais.Geral.Opcoes, tamanho: 7, opcoes: [reenviarDadosCancelamento, historicoIntegracaoDadosCancelamento] };

    _gridDadosCancelamentoCTe = new GridView(_integracaoDadosCancelamentoCTe.ArquivosDadosCancelamento.idGrid, "CancelamentoCargaIntegracaoDadosCancelamentoCTe/Pesquisa", _integracaoDadosCancelamentoCTe, menuOpcoesDadosCancelamento, null, null, null);
    _gridDadosCancelamentoCTe.CarregarGrid();
}

function ReenviarDadosCancelamentoCTeClick(e, sender) {
    exibirConfirmacao("Atenção", "Deseja realmente reenviar essa integração?", function () {
        executarReST("CancelamentoCargaIntegracaoDadosCancelamentoCTe/ReenviarDadosCancelamento", { Codigo: e.Codigo }, function (arg) {
            if (arg.Success) {
                if (arg.Data) {
                    _gridDadosCancelamentoCTe.CarregarGrid();
                    CarregarIntegracaoDadosCancelamentoCTe();
                } else {
                    exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.Atencao, arg.Msg);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, arg.Msg);
            }
        });
    });
}

function ConsultarDadosCancelamentoCTeClick(e, sender) {
    _gridDadosCancelamentoCTe.CarregarGrid();
    CarregarIntegracaoDadosCancelamentoCTe();
}

function ReenviarTodosDadosCancelamentoCTeClick(e, sender) {
    exibirConfirmacao("Atenção!", "Deseja realmente reenviar todas as integrações?", function () {
        executarReST("CancelamentoCargaIntegracaoDadosCancelamentoCTe/ReenviarTodosDadosCancelamento", { Codigo: _cancelamento.Codigo.val() }, function (arg) {
            if (arg.Success) {
                _gridDadosCancelamentoCTe.CarregarGrid();
                CarregarIntegracaoDadosCancelamentoCTe();
            } else {
                exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, arg.Msg);
            }
        });
    });
}

//*******MÉTODOS*******

function CarregarIntegracaoDadosCancelamentoCTe() {
    if (_cancelamento.Codigo.val() > 0 && _cancelamento.Codigo.val() != "") {
        executarReST("CancelamentoCargaIntegracaoDadosCancelamentoCTe/ObterTotais", { Codigo: _cancelamento.Codigo.val() }, function (e) {
            if (e.Success) {
                if (e.Data != null) {
                    var dataIntegracao = { Data: e.Data };
                    PreencherObjetoKnout(_integracaoDadosCancelamentoCTe, dataIntegracao);
                    _gridDadosCancelamentoCTe.CarregarGrid();
                }
            }
            else {
                exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, e.Msg);
            }
        });
    }
}

function LimparInegracaoDadosCancelamentoCTe() {
    LimparCampos(_integracaoDadosCancelamentoCTe);
    _gridDadosCancelamentoCTe.CarregarGrid();
}

function ExibirHistoricoIntegracaoDadosCancelamentoCTe(integracao) {
    BuscarHistoricoIntegracaoDadosCancelamentoCTe(integracao);
    Global.abrirModal("divModalHistoricoIntegracaoDadosCancelamento");
}

function BuscarHistoricoIntegracaoDadosCancelamentoCTe(integracao) {
    _pesquisaHistoricoIntegracaoDadosCancelamentoCTe = new PesquisaHistoricoIntegracaoDadosCancelamentoCTe();
    _pesquisaHistoricoIntegracaoDadosCancelamentoCTe.Codigo.val(integracao.Codigo);

    var download = { descricao: Localization.Resources.Gerais.Geral.DownloadArquivos, id: guid(), evento: "onclick", metodo: DownloadArquivosHistoricoIntegracaoDadosCancelamentoCTe, tamanho: "20", icone: "" };

    var menuOpcoes = {
        tipo: TypeOptionMenu.link,
        opcoes: [download]
    };

    _gridHistoricoIntegracaoDadosCancelamentoCTe = new GridView("tblHistoricoIntegracaoDadosCancelamento", "CancelamentoCargaIntegracaoDadosCancelamentoCTe/ConsultarHistoricoIntegracao", _pesquisaHistoricoIntegracaoDadosCancelamentoCTe, menuOpcoes, { column: 1, dir: orderDir.desc });
    _gridHistoricoIntegracaoDadosCancelamentoCTe.CarregarGrid();
}

function DownloadArquivosHistoricoIntegracaoDadosCancelamentoCTe(historicoConsulta) {
    executarDownload("CancelamentoCargaIntegracaoDadosCancelamentoCTe/DownloadArquivosHistoricoIntegracao", { Codigo: historicoConsulta.Codigo });
}

function BuscarIntegracoesDadosCancelamentoCTe() {
    _integracaoDadosCancelamentoCTe.Codigo.val(_cancelamento.Codigo.val());
    _gridDadosCancelamentoCTe.CarregarGrid();
}