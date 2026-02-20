//*******MAPEAMENTO KNOUCKOUT*******

var _valoresContasAReceber;

var ValoresContasAReceber = function () {

    this.ValorCTesSemCarga = PropertyEntity({ text: "CT-es Não Vinculados à Cargas: " });
    this.CTesNaoFaturados = PropertyEntity({ text: "CT-es Não Faturados: " });
    this.ValorTitulosEmAberto = PropertyEntity({ text: "Títulos em Aberto: " });
    this.ValorOutrosTitulosEmAberto = PropertyEntity({ text: "Outros Títulos em Aberto: " });
    this.ValorFaturaEmAberto = PropertyEntity({ text: "CT-es em Fatura: " });
    this.ValorTotal = PropertyEntity({ text: "Total: " });

    this.Pesquisar = PropertyEntity({ eventClick: PesquisarValoresContasAReceberClick, type: types.event, text: "Consultar", visible: ko.observable(true), icon: "fal fa-search" });
    this.DownloadRelatorioAnalitico = PropertyEntity({ eventClick: ImprimirRelatorioAnaliticoContasAReceberClick, type: types.event, text: "Gerar Relatório Analítico", visible: ko.observable(true), icon: "fal fa-print" });
}

//*******EVENTOS*******

function LoadValoresContasAReceber() {
    _valoresContasAReceber = new ValoresContasAReceber();
    KoBindings(_valoresContasAReceber, "knockoutContasAReceber");
}


function PesquisarValoresContasAReceberClick(e, sender) {
    executarReST("ConsultaValores/ObterResumoContasAReceber", {}, function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                PreencherObjetoKnout(_valoresContasAReceber, arg);
            } else {
                exibirMensagem(tipoMensagem.atencao, "Atenção", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    });
}

function ImprimirRelatorioAnaliticoContasAReceberClick(e, sender) {
    executarDownload("ConsultaValores/DownloadRelatorioContasAReceberAnalitico", {});
}