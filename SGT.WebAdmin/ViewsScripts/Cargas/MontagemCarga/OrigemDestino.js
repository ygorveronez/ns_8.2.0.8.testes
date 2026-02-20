/// <reference path="Bloco.js" />
/// <reference path="Carga.js" />
/// <reference path="Carregamento.js" />
/// <reference path="CarregamentoCarga.js" />
/// <reference path="CarregamentoPedido.js" />
/// <reference path="Carregamentos.js" />
/// <reference path="CarregamentoTransporte.js" />
/// <reference path="DirecoesGoogleMaps.js" />
/// <reference path="Distancia.js" />
/// <reference path="GoogleMaps.js" />
/// <reference path="MontagemCarga.js" />
/// <reference path="Pedido.js" />
/// <reference path="PedidoProduto.js" />
/// <reference path="PedidosMapa.js" />
/// <reference path="Roteirizador.js" />
/// <reference path="SimulacaoFrete.js" />

//var _EnderecosOrigens;
//var _EnderecosDestinos;

//function loadOrigemDestino() {
//    _EnderecosOrigens = new Array();
//    _EnderecosDestinos = new Array();
//}

//function AdicionarEnderecosPedido(pedido) {

//    if (pedido.EnderecoOrigem != null && pedido.EnderecoOrigem.Latitude != null && pedido.EnderecoOrigem.Longitude != null && pedido.EnderecoOrigem.Latitude != "" && pedido.EnderecoOrigem.Longitude != "") {
//        var origem = retornarIndiceEnderecoOrigem(pedido.EnderecoOrigem);
//        if (origem == -1) {
//            _EnderecosOrigens.push(pedido.EnderecoOrigem);
//        }
//    }

//    if (pedido.EnderecoDestino != null && pedido.EnderecoDestino.Latitude != null && pedido.EnderecoDestino.Longitude != null && pedido.EnderecoDestino.Latitude != "" && pedido.EnderecoDestino.Longitude != "") {
//        var destino = retornarIndiceEnderecoDestino(pedido.EnderecoDestino);
//        if (destino == -1) {
//            _EnderecosDestinos.push(pedido.EnderecoDestino);
//        }
//    }

//}


//function limparOrigensDestinos() {
//    _EnderecosOrigens = new Array();
//    _EnderecosDestinos = new Array();
//}

//function retornarIndiceEnderecoOrigem(origem) {
//    if (!NavegadorIEInferiorVersao12()) {
//        return _EnderecosOrigens.findIndex(function (item) { return item.Remetente == origem.Remetente });
//    } else {
//        for (var i = 0; i < _EnderecosOrigens.length; i++) {
//            if (origem.Remetente == _EnderecosOrigens[i].Remetente)
//                return i;
//        }
//        return -1;
//    }
//}

//function retornarIndiceEnderecoDestino(destino) {
//    if (!NavegadorIEInferiorVersao12()) {
//        return _EnderecosDestinos.findIndex(function (item) { return item.Destinatario == destino.Destinatario });
//    } else {
//        for (var i = 0; i < _EnderecosDestinos.length; i++) {
//            if (destino.Destinatario == _EnderecosDestinos[i].Destinatario)
//                return i;
//        }
//        return -1;
//    }
//}
