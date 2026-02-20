
/// <reference path="../../../../js/Global/Globais.js" />
/// <reference path="../../../../js/Global/Grid.js" />
/// <reference path="../../../../js/Global/Mensagem.js" />
/// <reference path="../../../../js/Global/Rest.js" />

var _composicaoImpostos;
var _gridComposicaoImpostosPedido;
var _gridComposicaoImpostosPedidoNotaFiscal;

function ComposicaoImpostos() {

    this.Codigo = PropertyEntity({ getType: typesKnockout.int, val: ko.observable(0), def: 0 });
    this.CodigoPedido = PropertyEntity({ getType: typesKnockout.int, val: ko.observable(0), def: 0 });
    this.GridComposicaoPedido = PropertyEntity({ type: types.local, idGrid: guid(), visible: ko.observable(true) });
    this.GridComposicaoPedidoNotaFiscal = PropertyEntity({ type: types.local, idGrid: guid(), visible: ko.observable(true) });
}

function CarregarGridComposicaoImpostosPedido(e) {
    _composicaoImpostos = new ComposicaoImpostos();
    KoBindings(_composicaoImpostos, "knockoutComposicaoPedido");

    _composicaoImpostos.Codigo.val(e.Codigo.val());

    var configExportacao = {
        url: "CargaFrete/ExportarPesquisaComponenteImpostosFretePedido",
        btnText: "Exportar Excel",
        funcaoObterParametros: function () {
            return { Codigo: _composicaoImpostos.Codigo.val() };
        }
    }

    var menuOpcoes = null;
    menuOpcoes = { tipo: TypeOptionMenu.link, opcoes: [{ descricao: Localization.Resources.Cargas.Carga.DetalhesPorNota, id: guid(), metodo: detalhesImpostosNotaFiscalClick, tamanho: 10 }] };

    _gridComposicaoImpostosPedido = new GridView(_composicaoImpostos.GridComposicaoPedido.idGrid, "CargaFrete/PesquisaComponenteImpostosFretePedido", _composicaoImpostos, menuOpcoes, null, 10, null, null, null, null, null, null, configExportacao);
    _gridComposicaoImpostosPedido.CarregarGrid();

    Global.abrirModal('#divModalComposicaoPedido');
}

function detalhesImpostosNotaFiscalClick(e) {

    _composicaoImpostos = new ComposicaoImpostos();
    KoBindings(_composicaoImpostos, "knockoutComposicaoPedidoNotaFiscal");

    var configExportacao = {
        url: "CargaFrete/ExportarPesquisaComponenteImpostosFretePedidoNotaFiscal",
        btnText: "Exportar Excel",
        funcaoObterParametros: function () {
            return { Codigo: _composicaoImpostos.Codigo.val() };
        }
    }

    _composicaoImpostos.CodigoPedido.val(e.CodigoPedido);

    _gridComposicaoImpostosPedidoNotaFiscal = new GridView(_composicaoImpostos.GridComposicaoPedidoNotaFiscal.idGrid, "CargaFrete/PesquisaComponenteImpostosFretePedidoNotaFiscal", _composicaoImpostos, null, null, 10, null, null, null, null, null, null, configExportacao);
    _gridComposicaoImpostosPedidoNotaFiscal.CarregarGrid();

    Global.abrirModal("#divModalComposicaoPedidoNotaFiscal");



}