/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Globais.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="DetalhesPedido.js" />

// #region Objetos Globais do Arquivo

var _gridStagesPedido;

// #endregion Objetos Globais do Arquivo

// #region Classes

function Stages() {
    this.Grid = PropertyEntity({ type: types.map, getType: typesKnockout.dynamic, idGrid: guid(), idTab: guid() });
}


function loadDetalhePedidoStagePedidoEmbarcador() {
    LoadLocalizationResources("Cargas.Carga").then(function () {

        _stagePedido = new Stages();
        KoBindings(_stagePedido, "divModalStagePedidoEmbarcador");

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
            { data: "NaoPossuiValePedagio", title: "Não Possui Vale Pedagio" },
            { data: "StatusVP", title: "Status VP" }
        ]

        _gridStagesPedido = new BasicDataTable(_stagePedido.Grid.idGrid, headerGridStage, null, { column: 0, dir: orderDir.asc });
        //_gridDetalhePedidoAnexoEmbarcador.CarregarGrid([]);
    });
}

function carregarStagesPedido(codigo, carga) {
    executarReST("Pedido/ObterStagesPedido", { Codigo: codigo, Carga: carga }, function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                _gridStagesPedido.CarregarGrid(retorno.Data.Stages);
            }
            else
                exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.Aviso, retorno.Msg, 16000);
        }
        else
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, retorno.Msg);
    });
}