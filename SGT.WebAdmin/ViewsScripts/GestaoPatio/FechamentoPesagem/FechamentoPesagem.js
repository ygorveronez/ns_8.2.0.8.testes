var _fechamentoPesagem;
var _pesagemQuantidadeLitrosCRUD;

var FechamentoPesagem = function () {
    this.FluxoGestaoPatio = PropertyEntity({ val: ko.observable(0), getType: typesKnockout.int });
    this.CaixasNFEntrada = PropertyEntity({text: "Caixas NF Entrada: ", val: ko.observable("")});
    this.PesoInicial = PropertyEntity({text: "Peso Inicial: ", val: ko.observable("")});
    this.PesoFinal = PropertyEntity({text: "Peso Final: ", val: ko.observable("")});
    this.PesoLiquidoKG = PropertyEntity({ text: "Peso Seco Kg: ", val: ko.observable("") });
    this.ConversaoPesoCaixa = PropertyEntity({ text: "Conversão Peso_Caixa: ", val: ko.observable("") });
    this.PesoLiquidoCXS = PropertyEntity({ text: "Peso Líquido CXS: ", val: ko.observable("") });
    this.PorcentagemPerdas = PropertyEntity({ text: "% Perdas: ", val: ko.observable("") });
    this.PesoLiquidoPosPerdas = PropertyEntity({ text: "Peso Líquido Pós Perdas CXS: ", val: ko.observable("") });
    this.PesoLiquidoPosPerdasKgs = PropertyEntity({ text: "Peso Líquido Pós Perdas KGs: ", val: ko.observable("") });
    this.ResultadoFinalProcessoCaixas = PropertyEntity({ text: "Resultado Final de Processo CXS: ", val: ko.observable("") });
    this.ResultadoFinalProcessoKgs = PropertyEntity({ text: "Resultado Final de Processo KGs: ", val: ko.observable("") });
    this.QuantidadeLitros = PropertyEntity({ getType: typesKnockout.decimal, text: "Qtd. Litros: ", val: ko.observable(""), visible: ko.observable(false) });
    this.SalvarQuantidadeLitros = PropertyEntity({ eventClick: salvarQuantidadeLitrosClick, type: types.event, text: "Salvar", visible: ko.observable(false) });
    this.PermiteInformarQtdLitros = PropertyEntity({ getType: typesKnockout.bool, val: ko.observable("") });

}

function LoadFechamentoPesagem() {
    $.get("Content/Static/GestaoPatio/FluxoPatioFechamentoPesagem.html?dyn=" + guid(), function (data) {
        $("#modalFechamentoPesagem").html(data);

        _fechamentoPesagem = new FechamentoPesagem();
        KoBindings(_fechamentoPesagem, "knockoutFechamentoPesagem");       
    });
}

function BuscarFechamentoPesagem(codigoFluxoGestaoPatio) {
    _fechamentoPesagem.FluxoGestaoPatio.val(codigoFluxoGestaoPatio);

    executarReST("CargaControleExpedicao/BuscarFechamentoPesagem", { FluxoGestaoPatio: codigoFluxoGestaoPatio}, function (r) {
        if (r.Success) {
            if (r.Data) {
                PreencherObjetoKnout(_fechamentoPesagem, r);

                if (_fechamentoPesagem.PermiteInformarQtdLitros.val() == true) {
                    _fechamentoPesagem.QuantidadeLitros.visible(true);
                    _fechamentoPesagem.SalvarQuantidadeLitros.visible(true);
                }
            } else {
                exibirMensagem(tipoMensagem.aviso, "Atenção!", r.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha!", r.Msg);
        }
    }, null, null);
}

function salvarQuantidadeLitrosClick(e, sender) {
    Salvar(_fechamentoPesagem, "CargaControleExpedicao/SalvarInformacoesFechamentoPesagem", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Dados de fechamento salvos.");

            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    }, sender);
}