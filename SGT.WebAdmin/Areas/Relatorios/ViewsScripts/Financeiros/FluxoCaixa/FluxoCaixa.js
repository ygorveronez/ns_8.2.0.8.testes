/// <reference path="../../../../../js/libs/jquery-2.1.1.js" />
/// <reference path="../../../../../js/Global/CRUD.js" />
/// <reference path="../../../../../js/Global/Rest.js" />
/// <reference path="../../../../../js/Global/Mensagem.js" />
/// <reference path="../../../../../js/Global/Grid.js" />
/// <reference path="../../../../../js/bootstrap/bootstrap.js" />
/// <reference path="../../../../../js/libs/jquery.blockui.js" />
/// <reference path="../../../../../js/Global/knoutViewsSlides.js" />
/// <reference path="../../../../../js/libs/jquery.maskMoney.js" />
/// <reference path="../../../../../js/plugin/datatables/jquery.dataTables.js" />
/// <reference path="../../../../../ViewsScripts/Consultas/Empresa.js" />
/// <reference path="../../../../../ViewsScripts/Enumeradores/EnumAnaliticoSintetico.js" />
/// <reference path="../../../../../ViewsScripts/Enumeradores/EnumProvisaoPesquisaTitulo.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _fluxoCaixa;

var _provisaoPesquisaTitulo = [
    { text: "Com Provisão", value: EnumProvisaoPesquisaTitulo.ComProvisao },
    { text: "Sem Provisão", value: EnumProvisaoPesquisaTitulo.SemProvisao },
    { text: "Somente Provisão", value: EnumProvisaoPesquisaTitulo.SomenteProvisao }
];

var _analiticoSintetico = [
    { text: "Analítico", value: EnumAnaliticoSintetico.Analitico },
    { text: "Sintético", value: EnumAnaliticoSintetico.Sintetico }
];

var FluxoCaixa = function () {
    this.Empresa = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "*Empresa:", idBtnSearch: guid(), required: ko.observable(false), enable: ko.observable(true), visible: ko.observable(false) });
    this.TipoPagamentoRecebimento = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Conta de Pagamento e Recebimento:", idBtnSearch: guid(), required: false, visible: ko.observable(true) });

    this.DataVencimentoInicial = PropertyEntity({ text: "*Data Vencimento Inicial: ", val: ko.observable(""), getType: typesKnockout.date, required: true });
    this.DataVencimentoFinal = PropertyEntity({ text: "*Data Vencimento Final: ", val: ko.observable(""), getType: typesKnockout.date, required: true });
    this.Provisao = PropertyEntity({ val: ko.observable(EnumProvisaoPesquisaTitulo.ComProvisao), options: _provisaoPesquisaTitulo, def: EnumProvisaoPesquisaTitulo.ComProvisao, text: "*Provisão:", visible: ko.observable(true) });
    this.AnaliticoSintetico = PropertyEntity({ val: ko.observable(EnumAnaliticoSintetico.Analitico), options: _analiticoSintetico, def: EnumAnaliticoSintetico.Analitico, text: "*Tipo Conta:", visible: ko.observable(true) });

    this.TituloPendente = PropertyEntity({ val: ko.observable(true), getType: typesKnockout.bool, text: "Considerar Títulos Pendentes de Períodos Anteriores no Saldo Disponível Inicial?", def: true, visible: ko.observable(true) });
    this.LimiteConta = PropertyEntity({ val: ko.observable(true), getType: typesKnockout.bool, text: "Considerar Limite das Contas de Pagamento e Recebimento no Saldo Inicial?", def: true, visible: ko.observable(true) });

    this.Gerar = PropertyEntity({ eventClick: GerarClick, type: types.event, text: "Gerar", visible: ko.observable(true) });
}

//*******EVENTOS*******

function loadFluxoCaixa() {
    _fluxoCaixa = new FluxoCaixa();
    KoBindings(_fluxoCaixa, "knockoutFluxoCaixa");

    new BuscarEmpresa(_fluxoCaixa.Empresa, function (data) {
        _fluxoCaixa.Empresa.codEntity(data.Codigo);
        _fluxoCaixa.Empresa.val(data.RazaoSocial);
    });
    new BuscarTipoPagamentoRecebimento(_fluxoCaixa.TipoPagamentoRecebimento);

    //if (_CONFIGURACAO_TMS.TipoServicoMultisoftware != EnumTipoServicoMultisoftware.MultiNFe) {
    //    _fluxoCaixa.Empresa.required(true);
    //    _fluxoCaixa.Empresa.visible(true);
    //}
}

function GerarClick(e, sender) {
    var valido = ValidarCamposObrigatorios(_fluxoCaixa);

    if (Globalize.parseDate(_fluxoCaixa.DataVencimentoInicial.val()) > Globalize.parseDate(_fluxoCaixa.DataVencimentoFinal.val())) {
        valido = false;
        _fluxoCaixa.DataVencimentoFinal.requiredClass("form-control");
        _fluxoCaixa.DataVencimentoFinal.requiredClass("form-control is-invalid");
    }

    if (valido) {
        var data = {
            Empresa: _fluxoCaixa.Empresa.codEntity(),
            TipoPagamentoRecebimento: _fluxoCaixa.TipoPagamentoRecebimento.codEntity(),
            DataVencimentoInicial: _fluxoCaixa.DataVencimentoInicial.val(),
            DataVencimentoFinal: _fluxoCaixa.DataVencimentoFinal.val(),
            Provisao: _fluxoCaixa.Provisao.val(),
            AnaliticoSintetico: _fluxoCaixa.AnaliticoSintetico.val(),
            TituloPendente: _fluxoCaixa.TituloPendente.val(),
            LimiteConta: _fluxoCaixa.LimiteConta.val()
        };
        executarReST("Relatorios/FluxoCaixa/BaixarRelatorio", data, function (arg) {
            if (arg.Success) {
                if (arg.Data !== false) {
                    BuscarProcessamentosPendentes();
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Aguarde que seu relatório está sendo gerado.");
                } else {
                    exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
            }
        })
    } else
        exibirMensagem(tipoMensagem.atencao, "Campos Obrigatórios", "Informe os campos obrigatórios!");
}

//*******MÉTODOS*******

function limparCamposFluxoCaixa() {
    LimparCampos(_fluxoCaixa);
}
