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

//*******MAPEAMENTO KNOUCKOUT*******

var _planoOrcamentario;

var PlanoOrcamentario = function () {
    this.Empresa = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "*Empresa:", idBtnSearch: guid(), required: ko.observable(false), enable: ko.observable(true), visible: ko.observable(false) });
    this.CentroResultado = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Centro de Resultado:", idBtnSearch: guid(), enable: ko.observable(true) });

    this.DataInicial = PropertyEntity({ getType: typesKnockout.date, def: Global.PrimeiraDataDoMesAnterior(), val: ko.observable(Global.PrimeiraDataDoMesAnterior()), text: "*Mês Anterior:", required: ko.observable(true), enable: ko.observable(true), visible: ko.observable(true) });
    this.DataFinal = PropertyEntity({ getType: typesKnockout.date, def: Global.UltimaDataDoMesAtual(), val: ko.observable(Global.UltimaDataDoMesAtual()), text: "*Mês Atual:", required: ko.observable(true), enable: ko.observable(true), visible: ko.observable(true) });
    this.Gerar = PropertyEntity({ eventClick: GerarClick, type: types.event, text: "Gerar", visible: ko.observable(true) });
}

//*******EVENTOS*******

function loadPlanoOrcamentario() {
    _planoOrcamentario = new PlanoOrcamentario();
    KoBindings(_planoOrcamentario, "knockoutPlanoOrcamentario");

    new BuscarEmpresa(_planoOrcamentario.Empresa, function (data) {
        _planoOrcamentario.Empresa.codEntity(data.Codigo);
        _planoOrcamentario.Empresa.val(data.RazaoSocial);
    });
    new BuscarCentroResultado(_planoOrcamentario.CentroResultado);

    if (_CONFIGURACAO_TMS.TipoServicoMultisoftware != EnumTipoServicoMultisoftware.MultiNFe) {
        _planoOrcamentario.Empresa.required(true);
        _planoOrcamentario.Empresa.visible(true);
    }
}

function GerarClick(e, sender) {
    var valido = ValidarCamposObrigatorios(_planoOrcamentario);

    if (valido) {
        var data = {
            Empresa: _planoOrcamentario.Empresa.codEntity(),
            CentroResultado: _planoOrcamentario.CentroResultado.codEntity(),
            DataInicial: _planoOrcamentario.DataInicial.val(),
            DataFinal: _planoOrcamentario.DataFinal.val()
        };
        executarReST("Relatorios/PlanoOrcamentario/BaixarRelatorio", data, function (arg) {
            if (arg.Success) {
                if (arg.Data !== false) {
                    BuscarProcessamentosPendentes();
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Aguarde que seu relatório está sendo gerado.");
                    limparCamposPlanoOrcamentario();
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

function limparCamposPlanoOrcamentario() {
    LimparCampos(_planoOrcamentario);
}