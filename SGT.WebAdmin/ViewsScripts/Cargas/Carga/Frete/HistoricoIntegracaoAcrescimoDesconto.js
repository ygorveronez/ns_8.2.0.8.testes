var _historicoIntegracaoAcrescimoDesconto;
var _gridHistoricoIntegracaoAcrescimoDesconto;

var HistoricoIntegracaoAcrescimoDesconto = function () {
    this.Grid = PropertyEntity({ type: types.local, idGrid: guid(), });
}

function loadHistoricoIntegracaoAcrescimoDesconto(carga) {
    
    var $divModal = $("#divModalHistoricoIntegracaoAcrescimoDesconto");

    if ($divModal.length == 0) {
        $.get("Content/Static/Carga/CargaContratoFreteHistoricoIntegracaoAcrescimoDesconto.html" + "?dyn=" + guid(), function (data) {
            $("#widget-grid").after(data);
            Global.abrirModal("divModalHistoricoIntegracaoAcrescimoDesconto");
        }).done(function () {
            _historicoIntegracaoAcrescimoDesconto= new HistoricoIntegracaoAcrescimoDesconto();
            KoBindings(_historicoIntegracaoAcrescimoDesconto, "knockoutHistoricoIntegracaoAcrescimoDesconto");
            Global.fecharModal("divModalHistoricoIntegracaoAcrescimoDesconto");
            LimparCampos(_historicoIntegracaoAcrescimoDesconto);
        });
    }

    ConsultarHistoricoIntegracaoAcrescimoDesconto(carga);
}

function fecharModalHistoricoIntegracaoAcrescimoDesconto() {
    Global.fecharModal('divModalHistoricoIntegracaoAcrescimoDesconto');
}

function ConsultarHistoricoIntegracaoAcrescimoDesconto(carga) {
    _gridHistoricoIntegracaoAcrescimoDesconto = new GridView("tblHistoricoIntegracao", "CargaFreteTerceiro/PesquisaHistoricoIntegracaoContratoFreteValores", carga, null, { column: 1, dir: orderDir.desc });
    _gridHistoricoIntegracaoAcrescimoDesconto.CarregarGrid();
}

