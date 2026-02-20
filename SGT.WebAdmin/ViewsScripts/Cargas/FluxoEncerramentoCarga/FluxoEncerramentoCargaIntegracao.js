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

//*******MAPEAMENTO KNOUCKOUT*******

var _integracaoFluxoEncerramentoCarga;
var _gridIntegracaoFluxoEncerramentoCarga;
var _gridHistoricoIntegracaoFluxoEncerramentoCarga;
var _pesquisaHistoricoIntegracaoFluxoEncerramentoCarga;

var IntegracaoFluxoEncerramentoCarga = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });

    this.Tipo = PropertyEntity({ val: ko.observable("-1"), options: EnumTipoConsulta.obterOpcoes(), text: "Consulta:", def: "-1", enable: ko.observable(true) });
    this.ConsultaFluxoEncerramentoCarga = PropertyEntity({ eventClick: ConsultarFluxoEncerramentoCargaClick, type: types.event, text: ko.observable("Consultar"), visible: ko.observable(true), enable: ko.observable(true) });
    this.EnviarIntegracoesFluxoEncerramentoCarga = PropertyEntity({ eventClick: ReenviarTodosFluxoEncerramentoCargaClick, type: types.event, text: ko.observable("Reenviar Todos"), visible: ko.observable(true), enable: ko.observable(true) });

    this.GridIntegracoesFluxoEncerramentoCarga = PropertyEntity({ type: types.map,  getType: typesKnockout.dynamic, idBtnSearch: guid(), idGrid: guid() });

    this.TotalGeral = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int, text: Localization.Resources.Gerais.Geral.Total.getFieldDescription() });
    this.TotalAguardandoIntegracao = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int, text: Localization.Resources.Gerais.Geral.AguardandoIntegracao.getFieldDescription() });
    this.TotalAguardandoRetorno = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int, text: Localization.Resources.Gerais.Geral.AguardandoRetorno.getFieldDescription() });
    this.TotalProblemaIntegracao = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int, text: Localization.Resources.Gerais.Geral.ProblemasNaIntegracao.getFieldDescription() });
    this.TotalIntegrado = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int, text: Localization.Resources.Gerais.Geral.Integrado.getFieldDescription() });

    this.PossuiIntegracao = PropertyEntity({ val: ko.observable(false), def: false, getType: typesKnockout.bool });
}

var PesquisaHistoricoIntegracaoFluxoEncerramentoCarga = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
};

//*******EVENTOS*******

function LoadIntegracoesFluxoEncerramentoCarga() {
    _integracaoFluxoEncerramentoCarga = new IntegracaoFluxoEncerramentoCarga();
    KoBindings(_integracaoFluxoEncerramentoCarga, "knockoutIntegracoesFluxoEncerramentoCarga");

    var reenviarIntegracaoFluxoEncerramentoCarga = { descricao: Localization.Resources.Gerais.Geral.Reenviar, id: guid(), metodo: ReenviarFluxoEncerramentoCargaClick, icone: "" };
    var historicoIntegracaoFluxoEncerramentoCarga = { descricao: "Histórico de Envio", id: guid(), metodo: ExibirHistoricoIntegracaoFluxoEncerramentoCarga, tamanho: "20", icone: "" };
    var menuOpcoesIntegracaoFluxoEncerramentoCarga = { tipo: TypeOptionMenu.list, descricao: Localization.Resources.Gerais.Geral.Opcoes, tamanho: 7, opcoes: [reenviarIntegracaoFluxoEncerramentoCarga, historicoIntegracaoFluxoEncerramentoCarga] };

    _gridIntegracaoFluxoEncerramentoCarga = new GridView(_integracaoFluxoEncerramentoCarga.GridIntegracoesFluxoEncerramentoCarga.idGrid, "FluxoEncerramentoCargaIntegracao/Pesquisa", _integracaoFluxoEncerramentoCarga, menuOpcoesIntegracaoFluxoEncerramentoCarga, null, null, null);
    _gridIntegracaoFluxoEncerramentoCarga.CarregarGrid();
}

function ReenviarFluxoEncerramentoCargaClick(e, sender) {
    exibirConfirmacao("Atenção", "Deseja realmente reenviar essa integração?", function () {
        executarReST("FluxoEncerramentoCargaIntegracao/Reenviar", { Codigo: e.Codigo }, function (arg) {
            if (arg.Success) {
                if (arg.Data) {
                    _gridIntegracaoFluxoEncerramentoCarga.CarregarGrid();
                    CarregarIntegracaoFluxoEncerramentoCarga();
                } else {
                    exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.Atencao, arg.Msg);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, arg.Msg);
            }
        });
    });
}

function ConsultarFluxoEncerramentoCargaClick(e, sender) {
    _gridIntegracaoFluxoEncerramentoCarga.CarregarGrid();
    CarregarIntegracaoFluxoEncerramentoCarga();
}

function ReenviarTodosFluxoEncerramentoCargaClick(e, sender) {
    exibirConfirmacao("Atenção!", "Deseja realmente reenviar todas as integrações?", function () {
        executarReST("FluxoEncerramentoCargaIntegracao/ReenviarTodos", { Codigo: _fluxoEncerramentoCarga.Codigo.val() }, function (arg) {
            if (arg.Success) {
                _gridIntegracaoFluxoEncerramentoCarga.CarregarGrid();
                CarregarIntegracaoFluxoEncerramentoCarga();
            } else {
                exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, arg.Msg);
            }
        });
    });
}

//*******MÉTODOS*******

function CarregarIntegracaoFluxoEncerramentoCarga() {
    if (_fluxoEncerramentoCarga.Codigo.val() > 0 && _fluxoEncerramentoCarga.Codigo.val() != "") {
        executarReST("FluxoEncerramentoCargaIntegracao/ObterTotais", { Codigo: _fluxoEncerramentoCarga.Codigo.val() }, function (e) {
            if (e.Success) {
                if (e.Data != null) {
                    var dataIntegracao = { Data: e.Data };
                    PreencherObjetoKnout(_integracaoFluxoEncerramentoCarga, dataIntegracao);
                    _gridIntegracaoFluxoEncerramentoCarga.CarregarGrid();
                }
            }
            else {
                exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, e.Msg);
            }
        });
    }
}

function ExibirHistoricoIntegracaoFluxoEncerramentoCarga(integracao) {
    BuscarHistoricoIntegracaoFluxoEncerramentoCarga(integracao);
    Global.abrirModal("divModalHistoricoFluxoEncerramentoCargaIntegracao");
}

function BuscarHistoricoIntegracaoFluxoEncerramentoCarga(integracao) {
    _pesquisaHistoricoIntegracaoFluxoEncerramentoCarga = new PesquisaHistoricoIntegracaoFluxoEncerramentoCarga();
    _pesquisaHistoricoIntegracaoFluxoEncerramentoCarga.Codigo.val(integracao.Codigo);

    var download = { descricao: Localization.Resources.Gerais.Geral.DownloadArquivos, id: guid(), evento: "onclick", metodo: DownloadArquivosHistoricoIntegracaoFluxoEncerramentoCarga, tamanho: "20", icone: "" };

    var menuOpcoes = {
        tipo: TypeOptionMenu.link,
        opcoes: [download]
    };

    _gridHistoricoIntegracaoFluxoEncerramentoCarga = new GridView("tblHistoricoIntegracaoFluxoEncerramentoCarga", "FluxoEncerramentoCargaIntegracao/ConsultarHistoricoIntegracao", _pesquisaHistoricoIntegracaoFluxoEncerramentoCarga, menuOpcoes, { column: 1, dir: orderDir.desc });
    _gridHistoricoIntegracaoFluxoEncerramentoCarga.CarregarGrid();
}

function DownloadArquivosHistoricoIntegracaoFluxoEncerramentoCarga(historicoConsulta) {
    executarDownload("FluxoEncerramentoCargaIntegracao/DownloadArquivosHistoricoIntegracao", { Codigo: historicoConsulta.Codigo });
}

function BuscarIntegracoesFluxoEncerramentoCargaIntegracaoCarga() {
    _integracaoFluxoEncerramentoCarga.Codigo.val(_fluxoEncerramentoCarga.Codigo.val());
    _gridIntegracaoFluxoEncerramentoCarga.CarregarGrid();
}