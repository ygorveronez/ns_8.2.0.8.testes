
var FreteAgrupado = function () {
    this.Tabela = PropertyEntity({ type: types.local, visible: ko.observable(true) });
    this.ContratoFrete = PropertyEntity({ type: types.local, visible: ko.observable(false) });
    this.Origem = PropertyEntity({ type: types.local, visible: ko.observable(true) });
    this.Destino = PropertyEntity({ type: types.local, visible: ko.observable(true) });
    this.TipoCalculo = PropertyEntity({ type: types.local, visible: ko.observable(false) });
    this.ValorFreteAPagar = PropertyEntity({ type: types.local });
    this.Impostos = PropertyEntity({ type: types.local });
    this.Aliquotas = PropertyEntity({ type: types.local, visible: ko.observable(false) });
    this.CSTs = PropertyEntity({ type: types.local, visible: ko.observable(false) });
    this.TaxaDocumentacao = PropertyEntity({ type: types.local, visible: ko.observable(false) });
    this.ValorTotal = PropertyEntity({ type: types.local, visible: ko.observable(true) });
    this.ValorFreteLiquido = PropertyEntity({ type: types.local, visible: ko.observable(true) });
    this.ValorFreteTabelaFrete = PropertyEntity({ type: types.local, visible: ko.observable(true) });
    this.ValorFreteNegociado = PropertyEntity({ type: types.local, visible: ko.observable(false) });
    this.ValorContratoFrete = PropertyEntity({ type: types.local, visible: ko.observable(false) });
    this.DetalhesFrete = PropertyEntity({ eventClick: detalhesFreteClick, type: types.event, text: Localization.Resources.Cargas.Carga.VerDetalhes, visible: ko.observable(true) });
    this.ValorMercadoria = PropertyEntity({ type: types.local, visible: ko.observable(false) });
    this.Peso = PropertyEntity({ type: types.local, visible: ko.observable(false) });
    this.Moeda = PropertyEntity({ type: types.local, visible: ko.observable(false) });
    this.ValorCotacaoMoeda = PropertyEntity({ type: types.local, visible: ko.observable(false) });
    this.ValorTotalMoeda = PropertyEntity({ type: types.local, visible: ko.observable(false) });
    this.CEPDestinoDiasUteis = PropertyEntity({ type: types.local, visible: ko.observable(false) });

    this.PercentualEmRelacaoTabela = PropertyEntity({ visible: ko.observable(true) });
    this.PercentualEmRelacaoValorFrete = PropertyEntity({ visible: ko.observable(true) });
    this.CustoFrete = PropertyEntity({ visible: ko.observable(true) });
};

var _knoutFreteAgrupado;
var _cargaTemp;

function preencherTabsCargas(dados, e) {
    _cargaTemp = e;
    //
    var html = "<ul class='nav nav-tabs'>";
    var idTab = "tabsCargasAgrupadas";
    if (dados.length > 1) {
        $.each(dados, function (i, carga) {
            var idLi = carga.Codigo;

            if (i == 0)
                html += '<li class="active" id="' + idLi + '">';
            else
                html += '<li id="' + idLi + '">';

            var bgColor = "";
            var color = "";

            color = "color:#000000;";

            html += "<a href='javascript:void(0);' onclick='buscarValorFrete(" + carga.Codigo + ")'" + " style='position: relative; border: solid 1px white; margin - bottom: 1px; " + bgColor + color + "'><span>";

            html += " Nº " + carga.CodigoCargaEmbarcador;

            html += '</span>';

            html += '</a></li>';

        });
    }

    html += "</ul>";
    $("#" + idTab).html(html);

    _knoutFreteAgrupado = new FreteAgrupado();
}


function buscarValorFrete(codigo) {
    if (localStorage.getItem(codigo)) {
        preencherRetornoFreteEmbarcador(_cargaTemp, JSON.parse(localStorage.getItem(codigo)));
    } else {
        executarReST("CargaFrete/VerificarFrete", { Codigo: codigo }, function (retorno) {
            if (retorno.Success) {
                if (retorno.Data) {
                    localStorage.setItem(codigo, JSON.stringify(retorno.Data));
                    preencherRetornoFreteEmbarcador(_cargaTemp, _HTMLDetalheFretePorFreteCliente, _knoutFreteAgrupado, JSON.parse(localStorage.getItem(codigo)));
                }
                else
                    exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.Aviso, retorno.Msg);
            }
            else
                exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, retorno.Msg);
        });
    }

    $("#tabsCargasAgrupadas").find('li.active').removeClass("active");

    $("#" + codigo).addClass("active");

}