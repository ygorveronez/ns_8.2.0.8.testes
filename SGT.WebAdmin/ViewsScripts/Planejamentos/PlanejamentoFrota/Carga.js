/// <reference path="PlanejamentoFrota.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _planejamentoCarga;
var _pesquisaplanejamentoCarga;
var _quantidadeCargasPorVez = 4;
var _cargaDetalhes;
var _cargaDetalhesValor;


var PlanejamentoCarga = function () {

    this.DataCarga = PropertyEntity({ text: "Data: ", enable: ko.observable(true), getType: typesKnockout.date, val: ko.observable(Global.DataAtual()), diminuirData: diminuirDataClick, aumentarData: aumentarDataClick, eventChange: dataCargaChange });

    this.Cargas = PropertyEntity({ type: types.local, val: ko.observableArray([]) });
    this.Codigo = PropertyEntity({ getType: typesKnockout.int, val: ko.observable(0), def: 0 });

    this.Total = PropertyEntity({ val: ko.observable(0), def: 0, eventChange: cargasScroll, visible: ko.observable(false) });
    this.Inicio = PropertyEntity({ val: ko.observable(0), def: 0, type: types.local });

    this.ExibirFiltros = PropertyEntity({
        eventClick: function (e) {
            if (e == undefined) return;
            Global.abrirModal('divFiltrosCarga');
        }, type: types.event, idFade: guid(), visibleFade: ko.observable(false), visible: ko.observable(true)
    });

};

var PlanejamentoCargaDetalhe = function (dados) {
    this.Codigo = PropertyEntity({ getType: typesKnockout.int, val: ko.observable(dados.Codigo), def: dados.Codigo });

    this.CodigoCargaEmbarcador = PropertyEntity({ val: ko.observable(dados.CodigoCargaEmbarcador) });
    this.OrigemEDestino = PropertyEntity({ val: ko.observable(dados.OrigemEDestino) });
    this.DataCarregamento = PropertyEntity({ text: "Data:", val: ko.observable(dados.DataCarregamentoFormatada) });
    this.ModeloVeicular = PropertyEntity({ text: "Modelo:", val: ko.observable(dados.ModeloVeicular) });
    this.ValorFrete = PropertyEntity({ text: "Valor Frete:", val: ko.observable(Globalize.format(dados.ValorFrete, "n2")) });

    this.DataInicioCarregamento = PropertyEntity({ text: "Previsão Início:", val: ko.observable(dados.DataInicioCarregamentoFormatada) });
    this.DataFimCarregamento = PropertyEntity({ text: "Previsão Fim:", val: ko.observable(dados.DataFimCarregamentoFormatada) });
    this.Origem = PropertyEntity({ text: "Origem:", val: ko.observable(dados.Origem) });
    this.Destino = PropertyEntity({ text: "Destino:", val: ko.observable(dados.Destino) });
    this.Peso = PropertyEntity({ text: "Peso:", val: ko.observable(Globalize.format(dados.Peso, "n2")) });

    this.SituacaoComprometimentoCarga = PropertyEntity({ val: ko.observable(dados.SituacaoComprometimentoCarga) });
    this.LogoGrupoPessoas = PropertyEntity({ val: ko.observable(dados.LogoGrupoPessoas) });

    this.Frotas = PropertyEntity({ type: types.local, val: ko.observableArray([]) });

    this.VisualizarDadosCarga = PropertyEntity({
        eventClick: function (e) {
            if (e == undefined) return;
            exibirDadosCargaClick(e);
        }, type: types.event, text: "Dados da Carga", idFade: guid(), visibleFade: ko.observable(false), visible: ko.observable(true)
    });
};


var PesquisaPlanejamentoCarga = function () {
    //FILTROS DE CARGA

    this.CodigoCargaEmbarcador = PropertyEntity({ text: "Carga: ", col: 12, placeholder: "Carga" });
    this.Veiculo = PropertyEntity({ text: "Veículos:", type: types.multiplesEntities, codEntity: ko.observable(0), idBtnSearch: guid(), visible: ko.observable(true), placeholder: "Veiculo" });
    this.GrupoPessoa = PropertyEntity({ text: "Grupo de Pessoa:", type: types.entity, codEntity: ko.observable(0), idBtnSearch: guid(), visible: ko.observable(false) });
    this.FuncionarioVendedor = PropertyEntity({ text: "Vendedor:", type: types.entity, codEntity: ko.observable(0), idBtnSearch: guid(), visible: ko.observable(true) });
    this.TipoOperacao = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: "Tipo de Operação:", idBtnSearch: guid(), visible: ko.observable(true) });
    this.Transportador = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: ko.observable("Transportador:"), issue: 69, idBtnSearch: guid(), visible: ko.observable(true) });
    this.Expedidor = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: "Expedidor:", idBtnSearch: guid(), visible: ko.observable(true) });
    this.Origem = PropertyEntity({ text: "Origem:", type: types.multiplesEntities, codEntity: ko.observable(0), idBtnSearch: guid() });
    this.Destino = PropertyEntity({ text: "Destino:", type: types.multiplesEntities, codEntity: ko.observable(0), idBtnSearch: guid(), placeholder: "Destino" });
    this.EstadoOrigem = PropertyEntity({ text: "Estado Origem:", type: types.multiplesEntities, codEntity: ko.observable(0), idBtnSearch: guid(), placeholder: "Estado Origem" });
    this.EstadoDestino = PropertyEntity({ text: "Estado Destino:", type: types.multiplesEntities, codEntity: ko.observable(0), idBtnSearch: guid(), placeholder: "Estado Destino" });
    this.CentroResultado = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: "Centro de Resultado:", idBtnSearch: guid() });
    this.ResponsavelVeiculo = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: "Responsável pelo Veículo:", idBtnSearch: guid() });
    this.FronteiraRotaFrete = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: "Fronteira:", idBtnSearch: guid() });
    this.PaisOrigem = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: "País de Origem:", idBtnSearch: guid() });
    this.PaisDestino = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: "País de Destino:", idBtnSearch: guid() });
    this.TipoOperacaoDiferenteDe = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: "Tipo de Operação diferente de:", idBtnSearch: guid() });

    this.NumeroPedido = PropertyEntity({ text: "Nº Pedido Embarcador: ", col: 12, placeholder: "Pedido" });
    this.NumeroNotaFiscal = PropertyEntity({ text: "Nota Fiscal: ", col: 12, placeholder: "Nota Fiscal" });

    this.PesquisarCargas = PropertyEntity({
        eventClick: function (e) {
            if (e == undefined) return;
            Global.fecharModal("divFiltrosCarga");
            recarregarCargas();
        }, type: types.event, idFade: guid(), text: "Pesquisar", visibleFade: ko.observable(false), visible: ko.observable(true)
    });

}
//*******EVENTOS*******

function loadPlanejamentoCarga() {
    _planejamentoCarga = new PlanejamentoCarga();
    KoBindings(_planejamentoCarga, "knoutContainerPlanejamentoCarga");

    _pesquisaplanejamentoCarga = new PesquisaPlanejamentoCarga();
    KoBindings(_pesquisaplanejamentoCarga, "knockoutfiltroPesquisaCarga");


    new BuscarTransportadores(_pesquisaplanejamentoCarga.Transportador, null, null, true);
    //new BuscarClientes(_pesquisaplanejamentoCarga.Cliente);
    //new BuscarCategoriaPessoa(_pesquisaplanejamentoCarga.CategoriaPessoa);
    new BuscarGruposPessoas(_pesquisaplanejamentoCarga.GrupoPessoa);
    new BuscarVeiculos(_pesquisaplanejamentoCarga.Veiculo);
    new BuscarFuncionario(_pesquisaplanejamentoCarga.FuncionarioVendedor);
    new BuscarTiposOperacao(_pesquisaplanejamentoCarga.TipoOperacao);
    new BuscarClientes(_pesquisaplanejamentoCarga.Expedidor);
    new BuscarLocalidades(_pesquisaplanejamentoCarga.Origem);
    new BuscarLocalidades(_pesquisaplanejamentoCarga.Destino);
    new BuscarEstados(_pesquisaplanejamentoCarga.EstadoOrigem);
    new BuscarEstados(_pesquisaplanejamentoCarga.EstadoDestino);
    new BuscarTiposOperacao(_pesquisaplanejamentoCarga.TipoOperacaoDiferenteDe);
    new BuscarFuncionario(_pesquisaplanejamentoCarga.ResponsavelVeiculo);
    new BuscarCentroResultado(_pesquisaplanejamentoCarga.CentroResultado);
    new BuscarClientes(_pesquisaplanejamentoCarga.FronteiraRotaFrete, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, true);
    new BuscarPaises(_pesquisaplanejamentoCarga.PaisOrigem);
    new BuscarPaises(_pesquisaplanejamentoCarga.PaisDestino);

    loadScrollfixPlanejamentoCarga();

    buscarCargasPlanejamentoFrota();
}

function loadScrollfixPlanejamentoCarga() {
    $('.scrollable').bind('mousewheel DOMMouseScroll', function (e) {
        if ($(this)[0].scrollHeight !== $(this).outerHeight()) {
            var e0 = e.originalEvent,
                delta = e0.wheelDelta || -e0.detail;

            this.scrollTop += (delta < 0 ? 1 : -1) * 30;
            e.preventDefault();
        }
    });
}

function exibirDadosCargaClick(e) {
    if (_cargaDetalhes != e.Codigo.id) {
        $('#' + _cargaDetalhes + '_detalhes').slideUp();
        $('#carga_' + _cargaDetalhesValor).removeClass('carga-selected');

        $('#' + e.Codigo.id + '_detalhes').slideDown();
        $('#carga_' + e.Codigo.val()).addClass('carga-selected');

        _cargaDetalhes = e.Codigo.id;
        _cargaDetalhesValor = e.Codigo.val();

        buscarPlanejamentoFrotas(1, false, e.Codigo.val(), e.Frotas.val());
    }
    else {
        $('#' + e.Codigo.id + '_detalhes').slideUp();
        $('#carga_' + e.Codigo.val()).removeClass('carga-selected');

        _cargaDetalhes = 0;
        _cargaDetalhesValor = 0;

        for (var i = 0; i < _planejamentoFrota.Frotas.val().length; i++) {
            var data = _planejamentoFrota.Frotas.val()[i];
            $("#card_" + data.CodigoFrota.val()).removeClass('card-selected');
        }
    }
}

function aumentarDataClick() {
    definirDataPlanejamentoCarga(1);
}

function diminuirDataClick() {
    definirDataPlanejamentoCarga(-1);
}

function cargasScroll(e, sender) {
    var elem = sender.target;
    if (_planejamentoCarga.Inicio.val() < _planejamentoCarga.Total.val()) {
        if (elem.scrollTop > (elem.scrollHeight - elem.offsetHeight)) {
            buscarCargasPlanejamentoFrota();
        }
    }
}

//*******MÉTODOS*******

function dataCargaChange() {
    if (!_planejamentoCarga.DataCarga.val())
        _planejamentoCarga.DataCarga.val(Global.DataAtual());

    recarregarCargas();
}

function definirDataPlanejamentoCarga(dias) {
    if (!_planejamentoCarga.DataCarga.val())
        _planejamentoCarga.DataCarga.val(Global.DataAtual());

    var dataCarga = moment(_planejamentoCarga.DataCarga.val(), 'DD/MM/YYYY');

    dataCarga.add(dias, 'day');
    _planejamentoCarga.DataCarga.val(dataCarga.format('DD/MM/YYYY'));

    recarregarCargas();
}

function recarregarCargas() {
    _planejamentoCarga.Inicio.val(0);
    _planejamentoCarga.Total.val(0);
    _planejamentoCarga.Cargas.val.removeAll();

    buscarCargasPlanejamentoFrota();
}

function buscarCargasPlanejamentoFrota() {
    var data = RetornarObjetoPesquisa(_pesquisaplanejamentoCarga);
    data.Data = _planejamentoCarga.DataCarga.val()

    data.Inicio = _planejamentoCarga.Inicio.val();
    data.Limite = _quantidadeCargasPorVez;

    //var data = { Data: _planejamentoCarga.DataCarga.val(), Inicio: _planejamentoCarga.Inicio.val(), Limite: _quantidadeCargasPorVez };
    executarReST("PlanejamentoFrota/BuscarCargasPorData", data, function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                var data = retorno.Data;

                _planejamentoCarga.Total.val(data.Total);
                _planejamentoCarga.Inicio.val(_planejamentoCarga.Inicio.val() + _quantidadeCargasPorVez);

                for (var i = 0; i < data.Cargas.length; i++)
                    AdicionarCargaALista(data.Cargas[i]);

                buscarPlanejamentoFrotas(1, false, null, null);
            } else {
                exibirMensagem(tipoMensagem.atencao, "Atenção", retorno.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
        }
    });
}

function AdicionarCargaALista(dados) {
    var carga = new PlanejamentoCargaDetalhe(dados);
    carga.Frotas.val.removeAll();

    if (dados.PlanejamentoCarga != undefined) {
        for (var i = 0; i < dados.PlanejamentoCarga.length; i++) {
            var data = dados.PlanejamentoCarga[i];
            var card = new CardFrotaDetalheCarga(data);
            carga.Frotas.val.push(card);
        }
    }

    _planejamentoCarga.Cargas.val.push(carga);
}


var CardFrotaDetalheCarga = function (data) {
    if (data == null)
        data = {};

    this.Data = data;
    this.CodigoFrota = PropertyEntity({ val: ko.observable(data["CodigoFrota"]) });
    this.CodigoCarga = PropertyEntity({ val: ko.observable(data["CodigoCarga"]) });
    this.Placas = PropertyEntity({ val: ko.observable(data["PlacaVeiculo"]) });
    this.Motorista = PropertyEntity({ val: ko.observable(data["Motorista"]) });

    this.RemoverFrotaCarga = PropertyEntity({
        eventClick: function (e) {
            if (e == undefined) return;
            removerFrotaCarga(e);
        }, type: types.event, text: "", idFade: guid(), visibleFade: ko.observable(false), visible: ko.observable(true)
    });

}
