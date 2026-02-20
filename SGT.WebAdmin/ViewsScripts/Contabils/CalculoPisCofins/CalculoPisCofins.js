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
/// <reference path="../../../ViewsScripts/Enumeradores/EnumCSTPISCOFINS.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _calculoPisCofins;

var _cstPISTributavel = [
    { text: "01 - Operação Tributável com Alíquota Básica", value: EnumCSTPISCOFINS.CST01 },
    { text: "02 - Operação Tributável com Alíquota Diferenciada", value: EnumCSTPISCOFINS.CST02 },
    { text: "03 - Operação Tributável com Alíquota por Unidade de Medida de Produto", value: EnumCSTPISCOFINS.CST03 },
    { text: "04 - Operação Tributável Monofásica - Revenda a Alíquota Zero", value: EnumCSTPISCOFINS.CST04 },
    { text: "05 - Operação Tributável por Substituição Tributária", value: EnumCSTPISCOFINS.CST05 },
    { text: "06 - Operação Tributável a Alíquota Zero", value: EnumCSTPISCOFINS.CST06 },
    { text: "07 - Operação Isenta da Contribuição", value: EnumCSTPISCOFINS.CST07 },
    { text: "08 - Operação sem Incidência da Contribuição", value: EnumCSTPISCOFINS.CST08 },
    { text: "09 - Operação com Suspensão da Contribuição", value: EnumCSTPISCOFINS.CST09 }
];

var _cstPISNaoTributavel = [
    { text: "07 - Operação Isenta da Contribuição", value: EnumCSTPISCOFINS.CST07 },
    { text: "08 - Operação sem Incidência da Contribuição", value: EnumCSTPISCOFINS.CST08 },
    { text: "09 - Operação com Suspensão da Contribuição", value: EnumCSTPISCOFINS.CST09 },
    { text: "49 - Outras Operações de Saída", value: EnumCSTPISCOFINS.CST49 }
];

var _cstCOFINSTributavel = [
    { text: "01 - Operação Tributável com Alíquota Básica", value: EnumCSTPISCOFINS.CST01 },
    { text: "02 - Operação Tributável com Alíquota Diferenciada", value: EnumCSTPISCOFINS.CST02 },
    { text: "03 - Operação Tributável com Alíquota por Unidade de Medida de Produto", value: EnumCSTPISCOFINS.CST03 },
    { text: "04 - Operação Tributável Monofásica - Revenda a Alíquota Zero", value: EnumCSTPISCOFINS.CST04 },
    { text: "05 - Operação Tributável por Substituição Tributária", value: EnumCSTPISCOFINS.CST05 },
    { text: "06 - Operação Tributável a Alíquota Zero", value: EnumCSTPISCOFINS.CST06 },
    { text: "07 - Operação Isenta da Contribuição", value: EnumCSTPISCOFINS.CST07 },
    { text: "08 - Operação sem Incidência da Contribuição", value: EnumCSTPISCOFINS.CST08 },
    { text: "09 - Operação com Suspensão da Contribuição", value: EnumCSTPISCOFINS.CST09 }
];

var _cstCOFINSNaoTributavel = [
    { text: "07 - Operação Isenta da Contribuição", value: EnumCSTPISCOFINS.CST07 },
    { text: "08 - Operação sem Incidência da Contribuição", value: EnumCSTPISCOFINS.CST08 },
    { text: "09 - Operação com Suspensão da Contribuição", value: EnumCSTPISCOFINS.CST09 },
    { text: "49 - Outras Operações de Saída", value: EnumCSTPISCOFINS.CST49 }
];

var CalculoPisCofins = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });

    this.CSTPISTributavel = PropertyEntity({ val: ko.observable(EnumCSTPISCOFINS.CST01), def: EnumCSTPISCOFINS.CST01, options: ko.observable(_cstPISTributavel), text: "*CST PIS Tributável:", required: true });
    this.CSTPISNaoTributavel = PropertyEntity({ val: ko.observable(EnumCSTPISCOFINS.CST07), def: EnumCSTPISCOFINS.CST07, options: ko.observable(_cstPISNaoTributavel), text: "*CST PIS Não Tributável:", required: true });

    this.CSTCOFINSTributavel = PropertyEntity({ val: ko.observable(EnumCSTPISCOFINS.CST01), def: EnumCSTPISCOFINS.CST01, options: ko.observable(_cstCOFINSTributavel), text: "*CST COFINS Tributável:", required: true });
    this.CSTCOFINSNaoTributavel = PropertyEntity({ val: ko.observable(EnumCSTPISCOFINS.CST07), def: EnumCSTPISCOFINS.CST07, options: ko.observable(_cstCOFINSNaoTributavel), text: "*CST COFINS Não Tributável:", required: true  });

    this.AliquotaPIS = PropertyEntity({ def: "", val: ko.observable(""), text: "*Alíquota PIS:", getType: typesKnockout.decimal, maxlength: 20, required: true });
    this.AliquotaCOFINS = PropertyEntity({ def: "", val: ko.observable(""), text: "*Alíquota COFINS:", getType: typesKnockout.decimal, maxlength: 20, required: true });

    this.Salvar = PropertyEntity({ eventClick: salvarClick, type: types.event, text: "Salvar" });
};

//*******EVENTOS*******

function loadCalculoPisCofins() {
    _calculoPisCofins = new CalculoPisCofins();
    KoBindings(_calculoPisCofins, "knockoutConfigCalculoPisCofins");

    HeaderAuditoria("CalculoPisCofins", _calculoPisCofins);

    BuscarPorCodigo(_calculoPisCofins, "CalculoPisCofins/BuscarCalculoPisCofins");
}

function salvarClick(e, sender) {
    Salvar(e, "CalculoPisCofins/Salvar", function (arg) {
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
