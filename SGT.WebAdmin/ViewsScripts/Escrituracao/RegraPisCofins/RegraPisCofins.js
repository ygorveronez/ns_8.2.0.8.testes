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


//*******MAPEAMENTO KNOUCKOUT*******

var _regraPisCofins;

var _origemDestinoFilia = [
    { value: true, text: "Filial" },
    { value: false, text: "Não Filial" }
];

var RegraPisCofins = function () {

    this.Pis = PropertyEntity({ text: "*Pis:", required: true, getType: typesKnockout.decimal, configDecimal : { precision: 4, allowZero: false, allowNegative: false }, val: ko.observable("") });
    this.Cofins = PropertyEntity({ text: "*Cofins:", required: true, getType: typesKnockout.decimal, val: ko.observable(""), configDecimal: { precision: 4, allowZero: false, allowNegative: false } });

    // CRUD
    this.Atualizar = PropertyEntity({ eventClick: atualizarClick, type: types.event, text: "Atualizar", visible: ko.observable(true) });
}



//*******EVENTOS*******
function loadRegraPisCofins() {
    //-- Knouckout
    // Instancia pesquisa
    // Instancia objeto principal
    _regraPisCofins = new RegraPisCofins();
    KoBindings(_regraPisCofins, "knockoutRegraPisCofins");

    HeaderAuditoria("RegraPisCofins", _regraPisCofins);
    
    // Inicia busca
    BuscarRegraPisCofins();
}

function atualizarClick(e, sender) {
    Salvar(_regraPisCofins, "RegraPisCofins/SalvarRegra", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Atualizado com sucesso");
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    }, sender);
}


//*******MÉTODOS*******
function BuscarRegraPisCofins() {

    executarReST("RegraPisCofins/BuscarRegra", null, function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                _regraPisCofins.Pis.val(arg.Data.Pis);
                _regraPisCofins.Cofins.val(arg.Data.Cofins);
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    })
}