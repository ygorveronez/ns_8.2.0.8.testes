
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
/// <reference path="../../Consultas/Filial.js" />
/// <reference path="Bloco.js" />
/// <reference path="Carga.js" />
/// <reference path="Carregamento.js" />
/// <reference path="CarregamentoPedido.js" />
/// <reference path="Carregamentos.js" />
/// <reference path="CarregamentoTransporte.js" />
/// <reference path="DirecoesGoogleMaps.js" />
/// <reference path="Distancia.js" />
/// <reference path="GoogleMaps.js" />
/// <reference path="MontagemCarga.js" />
/// <reference path="OrigemDestino.js" />
/// <reference path="Pedido.js" />
/// <reference path="PedidoProduto.js" />
/// <reference path="PedidosMapa.js" />
/// <reference path="Roteirizador.js" />
/// <reference path="SimulacaoFrete.js" />



//*******MAPEAMENTO KNOUCKOUT*******

var _carregamentoCarga;
var _gridCargasCarregamento;
var CarregamentoCarga = function () {
    this.Grid = PropertyEntity({ type: types.local });
}
//*******EVENTOS*******


function loadCarregamentoCarga() {
    _carregamentoCarga = new CarregamentoCarga();
    KoBindings(_carregamentoCarga, "knoutCargas");

    var remover = { descricao: Localization.Resources.Cargas.MontagemCarga.Remover, id: guid(), metodo: removerCargaClick, icone: "" }; //, visibilidade: VisibilidadeOpcaoEditar };
    var visualizar = { descricao: Localization.Resources.Cargas.MontagemCarga.Detalhes, id: guid(), metodo: detalhesCargaMontagemClick, icone: "" };
    var menuOpcoes = { tipo: TypeOptionMenu.list, descricao: Localization.Resources.Cargas.MontagemCarga.Opcoes, tamanho: 15, opcoes: [visualizar, remover] };
    
    var header = [
        { data: "Codigo", visible: false },
        { data: "CodigoCargaEmbarcador", title: Localization.Resources.Cargas.MontagemCarga.NumeroCarga, width: "25%" },
        { data: "Destino", title: Localization.Resources.Cargas.MontagemCarga.Destino, width: "45%" }
    ];

    _gridCargasCarregamento = new BasicDataTable(_carregamentoCarga.Grid.id, header, menuOpcoes);
    _gridCargasCarregamento.CarregarGrid(_carregamento.Cargas.val());
    //recarregarGridCargasCarregamento();

}

function removerCargaClick(dataRow) {
    exibirConfirmacao(Localization.Resources.Cargas.MontagemCarga.Confirmacao, Localization.Resources.Cargas.MontagemCarga.RealmenteDesejaRemoverCargaDestaMontagemDeCarga.format(dataRow.CodigoCargaEmbarcador), function () {
        var index = obterIndiceKnoutCarga(dataRow);
        if (index != -1)
            _knoutsCargas[index].InfoCarga.cssClass("card card-carga no-padding padding-5");
        var indiceCarga = obterIndiceCarga(dataRow);
        var carga = _carregamento.Cargas.val()[indiceCarga];
        _carregamento.Cargas.val().splice(indiceCarga, 1);
        RemoverCarga(carga);
    });
}


function detalhesCargaMontagemClick(dataRow) {
    ObterDetalhesCargaMontagem(dataRow.Codigo);
}

function RenderizarGridMotagemCargas() {
    _gridCargasCarregamento.CarregarGrid(_carregamento.Cargas.val());
}

function limparCarregamentoCarga() {
    LimparCampos(_carregamentoCarga);
}