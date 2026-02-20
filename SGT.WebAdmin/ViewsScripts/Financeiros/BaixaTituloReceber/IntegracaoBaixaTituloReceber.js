/// <reference path="../../Consultas/Justificativa.js" />
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
/// <reference path="../../../js/libs/jquery.twbsPagination.js" />
/// <reference path="../../../js/libs/jquery.globalize.js" />
/// <reference path="NegociacaoBaixaTituloReceber.js" />
/// <reference path="../../../js/libs/jquery.globalize.pt-BR.js" />
/// <reference path="CabecalhoBaixaTituloReceber.js" />
/// <reference path="BaixaTituloReceber.js" />
/// <reference path="EtapaBaixaTituloReceber.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _integracaoBaixa;
var _gridIntegracaoBaixa;
var _HTMLIntegracaoBaixa;

var IntegracaoBaixa = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Integracoes = PropertyEntity({ type: types.map, required: false, text: "Integrações", getType: typesKnockout.dynamic, idBtnSearch: guid(), idGrid: guid() });
}

//*******EVENTOS*******

function loadIntegracaoBaixa() {
    carregarConteudosHTMLIntegracaoBaixa(function () {
        $("#contentIntegracaoTituloReceber").html("");
        var idDiv = guid();
        $("#contentIntegracaoTituloReceber").append(_HTMLIntegracaoBaixa.replace(/#divIntegracaoBaixaTituloReceber/g, idDiv));
        _integracaoBaixa = new IntegracaoBaixa();
        KoBindings(_integracaoBaixa, idDiv);

        var detalhe = { descricao: "Reenviar", id: "clasEditar", evento: "onclick", metodo: ReenviarIntegracaoClick, tamanho: "10", icone: "" };
        var menuOpcoes = new Object();
        menuOpcoes.tipo = TypeOptionMenu.link;
        menuOpcoes.opcoes = new Array();
        menuOpcoes.opcoes.push(detalhe);

        _gridIntegracaoBaixa = new GridView(_integracaoBaixa.Integracoes.idGrid, "IntegracaoBaixaTituloReceber/PesquisaIntegracao", _integracaoBaixa, menuOpcoes, null, null, null);
        _gridIntegracaoBaixa.CarregarGrid();
    });
}

function ReenviarIntegracaoClick(e, sender) {
    if (e.Codigo > 0 && e.Codigo != "") {
        var data =
        {
            Codigo: e.Codigo
        }
        executarReST("IntegracaoBaixaTituloReceber/ReenviarIntegracao", data, function (e) {
            if (!e.Success) {
                exibirMensagem(tipoMensagem.falha, "Falha", e.Msg);
            } else if (_gridIntegracaoBaixa != null)
                _gridIntegracaoBaixa.CarregarGrid();
        });
    }
}

//*******MÉTODOS*******

function LimparCamposIntegracao() {
    LimparCampos(_integracaoBaixa);
    _gridIntegracaoBaixa.CarregarGrid();
}

function CarregarIntegracaoBaixa() {
    _integracaoBaixa.Codigo.val(_baixaTituloReceber.Codigo.val());
    if (_gridIntegracaoBaixa != null)
        _gridIntegracaoBaixa.CarregarGrid();
}

function carregarConteudosHTMLIntegracaoBaixa(callback) {
    $.get("Content/Static/Financeiro/IntegracaoBaixaTituloReceber.html?dyn=" + guid(), function (data) {
        _HTMLIntegracaoBaixa = data;
        callback();
    });
}