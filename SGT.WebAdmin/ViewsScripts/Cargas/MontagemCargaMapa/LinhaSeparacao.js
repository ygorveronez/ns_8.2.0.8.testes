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

var _linhaSeparacao;
var _gridLinhaSeparacao;

var LinhaSeparacao = function () {
    this.Grid = PropertyEntity({ id: guid() });
}
//*******EVENTOS*******


function loadLinhaSeparacao() {

    _linhaSeparacao = new LinhaSeparacao();
    KoBindings(_linhaSeparacao, "knoutLinhasSeparacao");

    var header = [{ data: "Codigo", visible: false },
    { data: "Descricao", title: Localization.Resources.Gerais.Geral.Descricao, width: "80%" }];

    _gridLinhaSeparacao = new BasicDataTable(_linhaSeparacao.Grid.id, header, null, { column: 1, dir: orderDir.asc });
}

function validaLinhaSeparacaoNaoAjustadoAgrupamentos() {
    if (_linhaSeparacao == null) {
        loadLinhaSeparacao();
    }
    var codigo = parseInt(_sessaoRoteirizador.Codigo.val());
    if (codigo > 0) {
        var filial = _sessaoRoteirizador.Filial.val();
        var data = { Codigo: codigo, Filial: filial };
        executarReST("LinhaSeparacao/LinhasSemValidacaoAgrupamento", data, function (arg) {
            if (arg.Success) {
                if (arg.Data.length > 0) {
                    var dados = new Array();
                    var r = arg.Data;
                    for (var i = 0; i < r.length; i++)
                        dados.push({ Codigo: r[i].Codigo, Descricao: r[i].Descricao });

                    _gridLinhaSeparacao.CarregarGrid(dados);                    
                    Global.abrirModal("divModalLinhasSeparacaoNaoValidada");
                }
            } else {
                exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, arg.Msg);
            }
        });
    }
}