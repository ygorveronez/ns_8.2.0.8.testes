//*******MAPEAMENTO KNOUCKOUT*******

var _gridGestaoNotasFiscais;
var _pesquisaGestaoNotasFiscais;

var PesquisaGestaoNotasFiscais = function () {
    this.Numero = PropertyEntity({ text: "Número:", getType: typesKnockout.int });
    this.Serie = PropertyEntity({ text: "Série:", getType: typesKnockout.int });

    this.DataEmissaoNotaFiscalInicial = PropertyEntity({ text: "Dt. Emissão NF Inicial:", getType: typesKnockout.date });
    this.DataEmissaoNotaFiscalFinal = PropertyEntity({ text: "Dt. Emissão NF Final:", getType: typesKnockout.date });
    this.DataEmissaoNotaFiscalFinal.dateRangeInit = this.DataEmissaoNotaFiscalInicial;
    this.DataEmissaoNotaFiscalInicial.dateRangeLimit = this.DataEmissaoNotaFiscalFinal;

    this.DataEmissaoCTeInicial = PropertyEntity({ text: "Dt. Emissão CT-e Inicial:", getType: typesKnockout.date });
    this.DataEmissaoCTeFinal = PropertyEntity({ text: "Dt. Emissão CT-e Final:", getType: typesKnockout.date });
    this.DataEmissaoCTeFinal.dateRangeInit = this.DataEmissaoCTeInicial;
    this.DataEmissaoCTeInicial.dateRangeLimit = this.DataEmissaoCTeFinal;

    this.DataEmissaoCargaInicial = PropertyEntity({ text: "Dt. Emissão Carga Inicial:", getType: typesKnockout.date });
    this.DataEmissaoCargaFinal = PropertyEntity({ text: "Dt. Emissão Carga Final:", getType: typesKnockout.date });
    this.DataEmissaoCargaFinal.dateRangeInit = this.DataEmissaoCargaInicial;
    this.DataEmissaoCargaInicial.dateRangeLimit = this.DataEmissaoCargaFinal;

    this.CTe = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "CT-e:", idBtnSearch: guid(), visible: ko.observable(true) });
    this.Carga = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Carga:", idBtnSearch: guid(), visible: ko.observable(true) });
    this.Emitente = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Emitente:", idBtnSearch: guid(), visible: ko.observable(true) });
    this.Empresa = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Empresa/Filial:", idBtnSearch: guid(), visible: ko.observable(true) });

    this.Produto = PropertyEntity({ text: "Produto:", getType: typesKnockout.string });
    this.Veiculo = PropertyEntity({ text: "Veículo:", getType: typesKnockout.placa });
    this.PossuiCTe = PropertyEntity({ text: "Possui Doc. Emitido:", options: Global.ObterOpcoesPesquisaBooleano("Sim", "Não"), val: ko.observable(""), def: true, issue: 0, visible: ko.observable(true) });

    this.Pesquisar = PropertyEntity({
        eventClick: function (e) {
            _gridGestaoNotasFiscais.CarregarGrid();
        }, type: types.event, text: "Pesquisar", idGrid: guid(), visible: ko.observable(true)
    });

    this.BuscaAvancada = PropertyEntity({
        eventClick: function (e) {
            if (e.BuscaAvancada.visibleFade() == true) {
                e.BuscaAvancada.visibleFade(false);
                e.BuscaAvancada.icon("fal fa-plus");
            } else {
                e.BuscaAvancada.visibleFade(true);
                e.BuscaAvancada.icon("fal fa-minus");
            }
        }, type: types.event, text: "Avançada", idFade: guid(), icon: ko.observable("fal fa-plus"), visibleFade: ko.observable(false), visible: ko.observable(true)
    });
}

//*******EVENTOS*******

function LoadGestaoNotasFiscais() {
    _pesquisaGestaoNotasFiscais = new PesquisaGestaoNotasFiscais();
    KoBindings(_pesquisaGestaoNotasFiscais, "knockoutPesquisaGestaoNotasFiscais", false, _pesquisaGestaoNotasFiscais.Pesquisar.id);

    new BuscarTransportadores(_pesquisaGestaoNotasFiscais.Empresa, null, null, true);
    new BuscarCTes(_pesquisaGestaoNotasFiscais.CTe);
    new BuscarCargas(_pesquisaGestaoNotasFiscais.Carga);
    new BuscarClientes(_pesquisaGestaoNotasFiscais.Emitente);

    BuscarGestaoNotasFiscais();
}

//*******MÉTODOS*******

function BuscarGestaoNotasFiscais() {
    
    var menuOpcoes = null;

    var configExportacao = {
        url: "GestaoNotasFiscais/ExportarPesquisa",
        titulo: "Gestão de Notas Fiscais"
    };

    _gridGestaoNotasFiscais = new GridViewExportacao(_pesquisaGestaoNotasFiscais.Pesquisar.idGrid, "GestaoNotasFiscais/Pesquisa", _pesquisaGestaoNotasFiscais, menuOpcoes, configExportacao, { column: 6, dir: orderDir.desc }, 10, null, 10);
    _gridGestaoNotasFiscais.CarregarGrid();

}