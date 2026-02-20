//#region Variaveis
var _gridStagePedido;
var _stagePedido;
//#endregion

//#region Funções Constructoras
function Stages() {
    this.Grid = PropertyEntity({ type: types.map, getType: typesKnockout.dynamic, idGrid: guid(), idTab: guid() });
}

function LoadStages() {
    _stagePedido = new Stages();
    KoBindings(_stagePedido, "knockoutStages");

    LoadGridStages();
}

function LoadGridStages() {

    const headerGridStage = [
        { data: "Codigo", visible: false },
        { data: "NumeroStage", title: "Número da Stage" },
        { data: "Expedidor", title: "Expedidor" },
        { data: "Recebedor", title: "Recebedor" },
        { data: "ModeloVeicularCarga", title: "Modelo veícular" },
        { data: "Distancia", title: "Distância" },
        { data: "OrdemEntrega", title: "Ordem Entrega" },
        { data: "TipoModal", title: "Tipo Modal" },
        { data: "CanalEntrega", title: "Canal Entrega" },
        { data: "CanalVenda", title: "Canal Venda" },
        { data: "RelevanciaCusto", title: "Relevancia Custo" },
        { data: "TipoPercurso", title: "Tipo Percurso" },
        { data: "Agrupamento", title: "Agrupamento" },
        { data: "NumeroVeiculo", title: "Número Veiculo" },
    ]

    _gridStagePedido = new BasicDataTable(_stagePedido.Grid.idGrid, headerGridStage, null, { column: 0, dir: orderDir.asc });
    CarregarGridStage(new Array());
}

function CarregarGridStage(dataGrid) {
    _gridStagePedido.CarregarGrid(dataGrid);
}

//#endregion