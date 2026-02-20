/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Globais.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="DetalhesPedido.js" />

// #region Objetos Globais do Arquivo

var _gridAgrupamentoStagesPedido;
var _agrupamentoStagePedido

// #endregion Objetos Globais do Arquivo

// #region Classes

function AgrupamentoStagesPedido() {
    this.Grid = PropertyEntity({ type: types.map, getType: typesKnockout.dynamic, idGrid: guid(), idTab: guid() });
}

function loadDetalhePedidoAgrupamentoStagePedidoEmbarcador() {
    LoadLocalizationResources("Cargas.Carga").then(function () {

        _agrupamentoStagePedido = new AgrupamentoStagesPedido();
        KoBindings(_agrupamentoStagePedido, "divModalAgrupamentoStagePedidoEmbarcador");

        const headerGridStage = [
            { data: "Codigo", visible: false },
            { data: "NumeroStage", title: "Números Stages" },
            { data: "Fornecedor", title: "Fornecedor" },
            { data: "Endereco", title: "Endereço Fornecedor" },
            { data: "Placa", title: "Placa" },
            { data: "Reboque", title: "Reboque" },
            { data: "SegundoReboque", title: "2º Reboque" },
            { data: "Motorista", title: "Motorista" },
            { data: "Frete", title: "Frete" }
        ]

        _gridAgrupamentoStagesPedido = new BasicDataTable(_agrupamentoStagePedido.Grid.idGrid, headerGridStage, null, { column: 0, dir: orderDir.asc });
        //_gridDetalhePedidoAnexoEmbarcador.CarregarGrid([]);
    });
}

function carregarAgrupamentoStagesPedido(Codigocarga) {
    executarReST("Pedido/ObterListaStagesAgrupadas", { Codigo: Codigocarga, PorCargaGerada: true }, function (retorno) {
        console.log(retorno);
        if (retorno.Success) {
            if (retorno.Data) {
                if (retorno.Data.StagesColetas.length > 0)
                    _gridAgrupamentoStagesPedido.CarregarGrid(retorno.Data.StagesColetas);
                else if (retorno.Data.StagesColetas.length > 0)
                    _gridAgrupamentoStagesPedido.CarregarGrid(retorno.Data.StagesTransferencia);
                else if (retorno.Data.StagesTransferencia.length > 0)
                    _gridAgrupamentoStagesPedido.CarregarGrid(retorno.Data.StagesEntregas);
            }
            else
                exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.Aviso, retorno.Msg, 16000);
        }
        else
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, retorno.Msg);
    });
}