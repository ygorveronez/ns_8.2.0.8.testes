/// <reference path="../../Consultas/Cargas.js" />

//#region Declaração variaveis 
var _gridLiberacaoCargaEmMassa;
var _cargaLiberacaoEmMassa;
//#endregion

//#region Fsunções Constructoras
var CargasParaLiberacao = function () {
    this.GridCargasParaLiberacao = PropertyEntity({ type: types.local });
    this.Cargas = PropertyEntity({ type: types.event, text: "Adicionar Cargas", idBtnSearch: guid(), issue: 0, visible: ko.observable(true) });
    this.LiberarCargas = PropertyEntity({ eventClick: LiberarCargasEtapaCalculoFrete, type: types.event, text: "Liberar Emissão", visible: ko.observable(true) });
}
//#endregion

//#region Funções de Carregamento
function loadLiberacaoCargaEmMassa() {

    _cargaLiberacaoEmMassa = new CargasParaLiberacao();
    KoBindings(_cargaLiberacaoEmMassa, "knockoutCargaParaLiberacao");

    LoadGridCargasParaLiberacao();
}
//#endregion

//#region Funções Auxiliares
function LoadGridCargasParaLiberacao() {
    var menuOpcoes = {
        tipo: TypeOptionMenu.link, tamanho: 7, opcoes: [{
            descricao: "Excluir", id: guid(), metodo: ExcluirCargaParaLiberacao
        }]
    };

    var header = [
        { data: "Codigo", visible: false },
        { data: "NumeroCarga", title: "Nº Carga", width: "10%", className: "text-align-left" },
        { data: "ModeloVeicular", title: "Modelo Veicular", width: "20%", className: "text-align-left" },
        { data: "TipoCarga", title: "Tipo de Carga", width: "20%", className: "text-align-left" },
        { data: "Transportador", title: "Transportador", width: "20%", className: "text-align-left" },
    ];

    _gridLiberacaoCargaEmMassa = new BasicDataTable(_cargaLiberacaoEmMassa.GridCargasParaLiberacao.id, header, menuOpcoes, { column: 1, dir: orderDir.asc },null,20);

    new BuscarCargas(_cargaLiberacaoEmMassa.Cargas, retornoMultiplaSelecao, null, null, null, null, _gridLiberacaoCargaEmMassa, null, null, null, null, null, null, null, EnumSituacoesCarga.CalculoFrete);

    _cargaLiberacaoEmMassa.Cargas.basicTable = _gridLiberacaoCargaEmMassa;

    RecarregarGridCargasParaLiberar();
}

function RecarregarGridCargasParaLiberar() {
    const data = new Array();

    $.each(_cargaLiberacaoEmMassa.Cargas.basicTable.BuscarRegistros, function (i, carga) {
        let itemCargaGrid = new Object();

        itemCargaGrid.Codigo = carga.Codigo;
        itemCargaGrid.NumeroCarga = carga.NumeroCarga;
        itemCargaGrid.ModeloVeicular = carga.ModeloVeicular;
        itemCargaGrid.TipoCarga = carga.TipoCarga;
        itemCargaGrid.Transportador = carga.Transportador;

        data.push(itemCargaGrid);
    });

    _gridLiberacaoCargaEmMassa.CarregarGrid(data);

}

function ExcluirCargaParaLiberacao(data) {
    const listaCargaGrid = _cargaLiberacaoEmMassa.Cargas.basicTable.BuscarRegistros();

    for (var i = 0; i < listaCargaGrid.length; i++) {
        if (data.Codigo == listaCargaGrid[i].Codigo) {
            listaCargaGrid.splice(i, 1);
            break;
        }
    }
    _cargaLiberacaoEmMassa.Cargas.basicTable.CarregarGrid(listaCargaGrid);

}

function retornoMultiplaSelecao(data) {
    const novaListaCarga = _cargaLiberacaoEmMassa.Cargas.basicTable.BuscarRegistros();

    $.each(data, function (i, carga) {
        let itemCargaGrid = new Object();

        itemCargaGrid.Codigo = carga.Codigo;
        itemCargaGrid.NumeroCarga = carga.CodigoCargaEmbarcador;
        itemCargaGrid.ModeloVeicular = carga.ModeloVeicular;
        itemCargaGrid.TipoCarga = carga.TipoCarga;
        itemCargaGrid.Transportador = carga.Transportador;

        novaListaCarga.push(itemCargaGrid);
    });

    _gridLiberacaoCargaEmMassa.CarregarGrid(novaListaCarga);
}

function LiberarCargasEtapaCalculoFrete() {
    const listaRegitrosGrid = _cargaLiberacaoEmMassa.Cargas.basicTable.BuscarRegistros();

    if (listaRegitrosGrid.length == 0)
        return exibirMensagem(tipoMensagem.atencao, "Aviso", "Sem cargas selecionas para liberar.");

    const CodigosCargas = listaRegitrosGrid.map(carga => carga.Codigo);

    executarReST("LiberacaoCargaEmMassa/LiberarCargas", { CodigosCargas: JSON.stringify(CodigosCargas) }, (arg) => {
        if (!arg.Success)
            return exibirMensagem(tipoMensagem.falha, "Error", arg.Msg);

        exibirMensagem(tipoMensagem.ok, "Sucesso", "Liberação das cargas feita com sucesso ");
        LimparRegistros();
    });
}

function LimparRegistros() {
    LimparCampos(_cargaLiberacaoEmMassa);
}
//#endregion