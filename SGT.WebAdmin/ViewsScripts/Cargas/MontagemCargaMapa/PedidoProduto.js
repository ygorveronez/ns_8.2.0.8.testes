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
/// <reference path="../../Consultas/Filial.js" />
/// <reference path="Bloco.js" />
/// <reference path="Carregamento.js" />
/// <reference path="CarregamentoPedido.js" />
/// <reference path="Carregamentos.js" />
/// <reference path="CarregamentoTransporte.js" />
/// <reference path="DirecoesGoogleMaps.js" />
/// <reference path="Distancia.js" />
/// <reference path="GoogleMaps.js" />
/// <reference path="MontagemCarga.js" />
/// <reference path="Pedido.js" />
/// <reference path="PedidosMapa.js" />
/// <reference path="Roteirizador.js" />
/// <reference path="SimulacaoFrete.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _pedidoProduto;
var _gridPedidoProduto;
var PedidoProduto = function () {
    //this.NumeroPedidoEmbarcador = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.string });
    this.Pedido = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Carregamento = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Grid = PropertyEntity({ id: guid() });
}
//*******EVENTOS*******


function loadPedidoProduto() {
    
    _pedidoProduto = new PedidoProduto();
    KoBindings(_pedidoProduto, "knoutProdutos");

    let menuOpcoes = { tipo: TypeOptionMenu.list, descricao: "Opções", tamanho: 7, opcoes: [] };
    menuOpcoes.opcoes.push({ descricao: "Remover Produto da Sessão", id: guid(), metodo: RemoverProduto, tamanho: "20", icone: "" });
    const inforEditarColuna = {
        permite: true,
        atualizarRow: true,
        callback: AtualizarPedidoProdutoPesoCarregamentoGrid
    };
    
    _gridPedidoProduto = new GridView(_pedidoProduto.Grid.id, "PedidoProduto/Carregamento", _pedidoProduto, menuOpcoes, null, 15, null, null, null, null, 100, inforEditarColuna, null, null, null, callbackRowPedidoProdutos);
}

var _linha;

function callbackRowPedidoProdutos(nRow, aData) {
    var linha = $(nRow);
    _linha = linha;
}

function carregarPedidoProduto(codigoPedido, callback) {
    var codCarregamento = 0;
    if (_carregamento != null) {
        codCarregamento = _carregamento.Carregamento.codEntity();
    }
    _pedidoProduto.Pedido.val(codigoPedido);
    _pedidoProduto.Carregamento.val(codCarregamento);
    if (callback != null) {
        _gridPedidoProduto.CarregarGrid(callback);
    } else {
        _gridPedidoProduto.CarregarGrid(function () {            
            Global.abrirModal("divModalPedidoProdutosCarregamento");
        });
    }
}

function AtualizarPedidoProdutoPesoCarregamentoGrid(dataRow, row, head, callbackTabPress) {
    
    var alterouPeso = false;
    var alterouQtde = false;

    //Alterando o peso
    if (head.data == 'QuantidadeCarregar') {
        alterouQtde = true;
    } else if (head.data == 'PesoTotalCarregar') {
        alterouPeso = true;
    }

    var codCarregamento = 0;
    if (_carregamento != null) {
        codCarregamento = _carregamento.Carregamento.codEntity();
    }
    
    if (codCarregamento > 0) {

        var peso = parseFloat(dataRow.PesoTotalCarregar.replace(".", "").replace(",", "."));
        var pesoOutrosCarregamentos = parseFloat(dataRow.PesoTotalCarregado.replace(".", "").replace(",", "."));
        var pesoTotalPedidoProduto = parseFloat(dataRow.PesoTotal.replace(".", "").replace(",", "."));

        if (peso < 0)
            return ExibirErroDataRow(_gridPedidoProduto, row, Localization.Resources.Cargas.MontagemCargaMapa.PesoCarregarNaoPodeSerInferiorZero, tipoMensagem.aviso, Localization.Resources.Gerais.Geral.Aviso);
        else if (peso + pesoOutrosCarregamentos > pesoTotalPedidoProduto)
            return ExibirErroDataRow(row, Localization.Resources.Cargas.MontagemCargaMapa.PesoCarregarNesteCarregamentoNaoPodeSerSuperior + " " + (pesoTotalPedidoProduto - pesoOutrosCarregamentos).toFixed(3) + ".", tipoMensagem.aviso, Localization.Resources.Gerais.Geral.Aviso);

        var palletFechado = (dataRow.PalletFechado == Localization.Resources.Gerais.Geral.Sim ? true : false);

        var dados = {
            Carregamento: codCarregamento,
            Codigo: parseInt(dataRow.Codigo),
            Peso: dataRow.PesoTotalCarregar,
            Qtde: dataRow.Quantidade,
            Pallet: dataRow.QuantidadePallet,
            Cubico: dataRow.MetroCubico
        };

        if (palletFechado && alterouPeso && (peso != pesoTotalPedidoProduto))
            return ExibirErroDataRow(row, Localization.Resources.Cargas.MontagemCargaMapa.NaoPermitidoAlterarPesoDeProdutosDePalletFechado, tipoMensagem.aviso, Localization.Resources.Gerais.Geral.Aviso);

        executarReST("MontagemCarga/AlterarPedidoProduto", dados, function (arg) {
            if (arg.Success) {
                if (arg.Data !== false) {
                    _gridPedidoProduto.CarregarGrid();
                    atualizouQuantidadesPedido(arg.Data);
                }
                else
                    exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.Aviso, arg.Msg);
            } else {
                exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, arg.Msg);
            }
        }, null);
    } else {
        return ExibirErroDataRow(row, Localization.Resources.Cargas.MontagemCargaMapa.PrimeiroSalveCarregamentoParaDepoisConfigurarOsProdutosDoCarregamento, tipoMensagem.aviso, Localization.Resources.Gerais.Geral.Aviso);
    }
}

function ExibirErroDataRow(row, mensagem, tipoMensagem, titulo) {
    _gridPedidoProduto.DesfazerAlteracaoDataRow(row);
    exibirMensagem(tipoMensagem, titulo, mensagem);
}

function modalPedidosProdutos() {    
    Global.abrirModal("divModalPedidoProdutosCarregamento");
}

function RemoverProduto(pedidoProduto) {

    const data = {
        CodigoPedidoProduto: pedidoProduto.Codigo,
        CodigoSessaoRoteirizador: _sessaoRoteirizador.Codigo.val()
    }
    exibirConfirmacao(Localization.Resources.Gerais.Geral.Confirmacao, `Realmente deseja remover o produto ${pedidoProduto.Produto} da sessão? Não será possível retorna-lo mais tarde.`, function () {

        let codCarregamento = 0;
        if (_carregamento != null) {
            codCarregamento = _carregamento.Carregamento.codEntity();
        }

        if (codCarregamento == 0) {

            executarReST("SessaoRoteirizador/RemoverPedidoProdutoDaSessao", data,function (arg) {
                if (arg.Success) {
                    _gridPedidoProduto.CarregarGrid();
                }
                else {
                    exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, arg.Msg);
                }
            }, null);
        } else {
            return ExibirErroDataRow(row, "Não é possível remover um produto após gerado o carregamento.", tipoMensagem.aviso, Localization.Resources.Gerais.Geral.Aviso);
        }
    });
}