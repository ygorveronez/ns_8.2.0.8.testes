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
/// <reference path="../../Enumeradores/EnumSituacaoCarregamento.js" />
/// <reference path="../../Enumeradores/EnumTipoCondicaoPagamento.js" />
/// <reference path="../../Consultas/Carregamento.js" />
/// <reference path="../../Consultas/Filial.js" />
/// <reference path="../../Consultas/PreCarga.js" />
/// <reference path="../../Consultas/TipoSeparacao.js" />
/// <reference path="Bloco.js" />
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
/// <reference path="Importacao.js" />

//*******MAPEAMENTO KNOUCKOUT*******
var _carregamento;
var _crudCarregamento;
var EVITAR_SUBSCRIBE_PEDIDOS_SELECIONADOS = false;
var _gerarCargasDeColeta = false;
var _montagemPedidoProduto = false;
var _preenchendoDadosCarregamento = false;
var _precisarSetarPedidosSelecionadosTabelaPedidosSessao = true;
var _VALOR_MINIMO_VALIDADO = false;

var CarregamentoMapa = function () {
    var self = this;

    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });

    this.CarregamentoRedespacho = PropertyEntity({ val: ko.observable(false), def: 0, getType: typesKnockout.bool });
    this.Carregamento = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Cargas.MontagemCargaMapa.NumeroDoCarregamento.getFieldDescription(), idBtnSearch: guid(), enable: ko.observable(false) });
    this.InformarPeriodoCarregamento = PropertyEntity({ val: ko.observable(_CONFIGURACAO_TMS.InformarPeriodoCarregamentoMontagemCarga), def: _CONFIGURACAO_TMS.InformarPeriodoCarregamentoMontagemCarga, getType: typesKnockout.bool });
    this.EscolherHorarioCarregamentoPorLista = PropertyEntity({ val: ko.observable(false), def: false, getType: typesKnockout.bool, eventClick: escolherHorarioDisponivelClick });
    this.EncaixarHorario = PropertyEntity({ val: ko.observable(false), def: false, getType: typesKnockout.bool });

    this.ModeloVeicularCarga = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Cargas.MontagemCargaMapa.ModeloVeicularDeCarga.getRequiredFieldDescription(), issue: 44, required: true, idBtnSearch: guid(), eventChange: modeloVeicularCargaBlur, enable: ko.observable(true), visible: ko.observable(true), numeroReboques: 0, exigirDefinicaoReboquePedido: false, OcupacaoCubicaPaletes: "0" });
    this.TipoSeparacao = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Cargas.MontagemCargaMapa.TipoDeSeparacao.getFieldDescription(), issue: 0, required: ko.observable(false), idBtnSearch: guid(), enable: ko.observable(true), visible: ko.observable(_CONFIGURACAO_TMS.ExigirTipoSeparacaoMontagemCarga) });
    this.Observacao = PropertyEntity({ text: Localization.Resources.Cargas.MontagemCargaMapa.Observacao.getFieldDescription(), val: ko.observable(""), def: "", maxlength: 2000, visible: ko.observable(true) });
    this.DataCarregamento = PropertyEntity({ text: Localization.Resources.Cargas.MontagemCargaMapa.DataCarregamento.getFieldDescription(), required: ko.observable(true), getType: (_CONFIGURACAO_TMS.InformaHorarioCarregamentoMontagemCarga ? typesKnockout.dateTime : typesKnockout.date), enable: ko.observable(true), idBtnSearch: guid() });
    this.DataInicioViagemPrevista = PropertyEntity({ text: ko.observable(Localization.Resources.Cargas.MontagemCargaMapa.DataPrevisaoSaidaVeiculo.getFieldDescription()), required: ko.observable(false), getType: typesKnockout.dateTime, enable: ko.observable(true), visible: ko.observable(false), required: ko.observable(false) });
    this.DataDescarregamento = PropertyEntity({ text: Localization.Resources.Cargas.MontagemCargaMapa.DataDescarregamento.getFieldDescription(), required: ko.observable(false), defRequired: false, getType: (_CONFIGURACAO_TMS.InformaHorarioCarregamentoMontagemCarga ? typesKnockout.dateTime : typesKnockout.date), enable: ko.observable(true), visible: ko.observable(false) });
    this.TipoCondicaoPagamento = PropertyEntity({ text: Localization.Resources.Cargas.MontagemCargaMapa.TipoDeFrete.getRequiredFieldDescription(), val: ko.observable(EnumTipoCondicaoPagamento.Todos), options: EnumTipoCondicaoPagamento.ObterOpcoes(), def: EnumTipoCondicaoPagamento.Todos, visible: ko.observable(_CONFIGURACAO_TMS.InformarTipoCondicaoPagamentoMontagemCarga), required: ko.observable(_CONFIGURACAO_TMS.InformarTipoCondicaoPagamentoMontagemCarga) });
    this.TipoMontagemCarga = PropertyEntity({ val: ko.observable(EnumTipoMontagemCarga.NovaCarga), options: EnumTipoMontagemCarga.obterOpcoes(), text: Localization.Resources.Cargas.MontagemCargaMapa.TipoDaMontagem.getFieldDescription(), issue: 1141, def: EnumTipoMontagemCarga.NovaCarga, eventChange: tipoMontagemCargaChange, enable: ko.observable(true), visible: ko.observable(true) });
    this.Situacao = PropertyEntity({ val: ko.observable(EnumSituacaoCarregamento.EmMontagem), def: EnumSituacaoCarregamento.EmMontagem });
    this.PreCarga = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: ko.observable(Localization.Resources.Cargas.MontagemCargaMapa.PreCarga.getFieldDescription()), idBtnSearch: guid(), enable: ko.observable(true), visible: ko.observable(true), required: ko.observable(false) });
    this.Recebedor = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Cargas.MontagemCargaMapa.Recebedor.getRequiredFieldDescription(), idBtnSearch: guid(), issue: 0, visible: ko.observable(false), required: ko.observable(false) });
    this.Expedidor = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: ko.observable(Localization.Resources.Cargas.MontagemCargaMapa.Expedidor.getFieldDescription()), idBtnSearch: guid(), issue: 0, visible: ko.observable(true), required: ko.observable(false) });
    this.PedidoViagemNavio = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Cargas.MontagemCargaMapa.NavioBarraViagemBarraDirecao.getRequiredFieldDescription(), idBtnSearch: guid(), required: false, visible: ko.observable(_CONFIGURACAO_TMS.UtilizaEmissaoMultimodal), enable: ko.observable(true) });
    this.TipoDeCarga = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: (_CONFIGURACAO_TMS.TipoCargaObrigatorioMontagemCarga ? "*" : "") + Localization.Resources.Cargas.MontagemCargaMapa.TiposDeCarga.getFieldDescription(), idBtnSearch: guid(), enable: ko.observable(true), required: _CONFIGURACAO_TMS.TipoCargaObrigatorioMontagemCarga, eventChange: tipoCargaCarregamentoBlur, visible: _CONFIGURACAO_TMS.ExibirTipoDeCargaNaAbaCarregamentoNaMontagemCarga, Paletizado: false });

    this.PreCarga.required.subscribe(function (campoObrigatorio) {
        self.PreCarga.text(campoObrigatorio ? Localization.Resources.Cargas.MontagemCargaMapa.PreCarga.getRequiredFieldDescription() : Localization.Resources.Cargas.MontagemCargaMapa.PreCarga);
        self.PreCarga.visible(campoObrigatorio);
    });

    this.Peso = PropertyEntity({ text: Localization.Resources.Cargas.MontagemCargaMapa.Peso, val: ko.observable("0,0000"), def: "0,0000", visible: ko.observable(true) });
    this.CapacidadePeso = PropertyEntity({ text: Localization.Resources.Cargas.MontagemCargaMapa.CapacidadePeso.getFieldDescription(), val: ko.observable("0,0000"), def: "0,0000", visible: ko.observable(false) });
    this.ToleranciaPesoMenor = PropertyEntity({ text: Localization.Resources.Cargas.MontagemCargaMapa.ToleranciaPesoMinimo.getFieldDescription(), val: ko.observable("0,0000"), def: "0,0000", visible: ko.observable(false) });
    this.LotacaoPeso = PropertyEntity({ text: Localization.Resources.Cargas.MontagemCargaMapa.LotacaoPeso.getFieldDescription(), val: ko.observable("0,0000"), def: "0,0000", visible: ko.observable(false) });

    this.PesoPallet = PropertyEntity({ text: Localization.Resources.Cargas.MontagemCargaMapa.PesoPallet, val: ko.observable("0,0000"), def: "0,0000", visible: ko.observable(true) });

    this.Pallets = PropertyEntity({ text: Localization.Resources.Cargas.MontagemCargaMapa.Pallets, val: ko.observable("0,0000"), def: "0,000", visible: ko.observable(true) });
    this.CapacidadePallets = PropertyEntity({ text: Localization.Resources.Cargas.MontagemCargaMapa.CapacidadeDePallets.getFieldDescription(), val: ko.observable("0,000"), def: "0,000", visible: ko.observable(false) });
    this.ToleranciaMinimaPaletes = PropertyEntity({ text: Localization.Resources.Cargas.MontagemCargaMapa.ToleranciaMinimaDePallets.getFieldDescription(), val: ko.observable("0,000"), def: "0,000", visible: ko.observable(false) });
    this.LotacaoPallets = PropertyEntity({ text: Localization.Resources.Cargas.MontagemCargaMapa.LocacaoPallets.getFieldDescription(), val: ko.observable("0,00"), def: "0,00", visible: ko.observable(false) });

    this.CubagemPaletes = PropertyEntity({ text: Localization.Resources.Cargas.MontagemCargaMapa.CubagemDosPaletes.getFieldDescription(), val: ko.observable("0,00"), def: "0,00", getType: typesKnockout.decimal, configDecimal: { precision: 2, allowZero: true, allowNegative: false } });
    this.Cubagem = PropertyEntity({ text: Localization.Resources.Cargas.MontagemCargaMapa.Cubagem, val: ko.observable("0,0000"), def: "0,00", visible: ko.observable(true) });
    this.CapacidadeCubagem = PropertyEntity({ text: Localization.Resources.Cargas.MontagemCargaMapa.CapacidadeCubagem.getFieldDescription(), val: ko.observable("0,00"), def: "0,00", visible: ko.observable(false) });
    this.ToleranciaMinimaCubagem = PropertyEntity({ text: Localization.Resources.Cargas.MontagemCargaMapa.ToleranciaCubagemMinima.getFieldDescription(), val: ko.observable("0,00"), def: "0,00", visible: ko.observable(false) });
    this.LotacaoCubagem = PropertyEntity({ text: Localization.Resources.Cargas.MontagemCargaMapa.LotacaoCubagem.getFieldDescription(), val: ko.observable("0,00"), def: "0,00", visible: ko.observable(false) });

    this.Pedidos = PropertyEntity({ getType: typesKnockout.dynamic, val: ko.observable(new Array()), def: new Array(), visible: ko.observable(true) });
    //this.Cargas = PropertyEntity({ getType: typesKnockout.dynamic, val: ko.observable(new Array()), def: new Array(), visible: ko.observable(true) });

    this.Produtos = PropertyEntity({ getType: typesKnockout.dynamic, val: ko.observable(new Array()), def: new Array(), visible: ko.observable(true) });

    this.DataCarregamento.val.subscribe(buscarCapacidadeJanelaCarregamento);

    this.PesoCarregamento = PropertyEntity({ text: Localization.Resources.Cargas.MontagemCargaMapa.PesoParaCarregamento.getFieldDescription(), val: ko.observable("0,0000"), visible: ko.observable(false), getType: typesKnockout.decimal, eventChange: pesoCarregamentoChange });

    this.AlteracoesPendentes = PropertyEntity({ val: ko.observable(false), def: false, getType: typesKnockout.bool });
};

var CRUDCarregamento = function () {
    this.DownloadEDI = PropertyEntity({ eventClick: downloadEDIClick, type: types.event, text: Localization.Resources.Cargas.MontagemCargaMapa.DownloadEDI, visible: ko.observable(false), enable: ko.observable(true) });
    this.Bloco = PropertyEntity({ eventClick: abrirGerarBlocoClick, type: types.event, text: Localization.Resources.Cargas.MontagemCargaMapa.GerarBlocos, title: Localization.Resources.Cargas.MontagemCargaMapa.GerarBlocosDeCarregamento, visible: ko.observable(false), enable: ko.observable(true) });
    this.SimularFrete = PropertyEntity({ eventClick: simularFreteClick, type: types.event, text: Localization.Resources.Cargas.MontagemCargaMapa.SimulacaoFrete, title: Localization.Resources.Cargas.MontagemCargaMapa.RealizarSimulacaoDoCalculoDeFrete, visible: ko.observable(false), enable: ko.observable(true) });
    this.Roteirizacao = PropertyEntity({ eventClick: roteirizarCargaClick, type: types.event, text: Localization.Resources.Cargas.MontagemCargaMapa.Roteirizacao, title: Localization.Resources.Cargas.MontagemCargaMapa.RealizarRoteirizacaoDoCarregamento, visible: ko.observable(false), enable: ko.observable(true) });
    this.GerarCarga = PropertyEntity({ eventClick: gerarCargaClick, type: types.event, text: Localization.Resources.Cargas.MontagemCargaMapa.GerarCarga, visible: ko.observable(false), enable: ko.observable(true) });
    this.Cancelar = PropertyEntity({ eventClick: cancelarCarregamentoClick, type: types.event, text: Localization.Resources.Gerais.Geral.Cancelar, visible: ko.observable(false), enable: ko.observable(true) });
    this.Atualizar = PropertyEntity({ eventClick: atualizarCarregamentoClick, type: types.event, text: Localization.Resources.Gerais.Geral.Salvar, visible: ko.observable(true), enable: ko.observable(true) });
    this.Limpar = PropertyEntity({ eventClick: iniciarNovoCarregamentoClick, type: types.event, text: Localization.Resources.Cargas.MontagemCargaMapa.NovoCarregamento, visible: ko.observable(true), enable: ko.observable(true) });
    this.AutorizarVeiculo = PropertyEntity({ eventClick: AutorizarVeiculoClick, type: types.event, text: Localization.Resources.Cargas.MontagemCargaMapa.AutorizarVeiculo, visible: ko.observable(false), enable: ko.observable(true) });
    this.Imprimir = PropertyEntity({ eventClick: ImprimirClick, type: types.event, text: Localization.Resources.Cargas.MontagemCargaMapa.Imprimir, visible: ko.observable(false), enable: ko.observable(true) });
    //Balanças
    this.Balancas = PropertyEntity({ getType: typesKnockout.dynamic, val: ko.observable(new Array()), def: new Array() });
    this.PracasPedagio = PropertyEntity({ getType: typesKnockout.dynamic, val: ko.observable(new Array()), def: new Array() });
};

var TituloCarregamento = function () {
    this.NumeroCarregamento = PropertyEntity({ text: Localization.Resources.Cargas.MontagemCargaMapa.Numero.getFieldDescription(), val: _carregamento.Carregamento.val, def: "", visible: _carregamento.Carregamento.enable() });
}

//*******EVENTOS*******

function retornoTipoCargaCarregamento(registroSelecionado) {
    _carregamento.TipoDeCarga.codEntity(registroSelecionado.Codigo);
    _carregamento.TipoDeCarga.entityDescription(registroSelecionado.Descricao);
    _carregamento.TipoDeCarga.val(registroSelecionado.Descricao);
    _carregamento.TipoDeCarga.Paletizado = registroSelecionado.Paletizado;

    obterPesosEAjustarCapacidade();
    buscarCapacidadeJanelaCarregamento(buscarPeriodoCarregamento);
}

function tipoCargaCarregamentoBlur() {
    if (_carregamento.TipoDeCarga.val() == "")
        limparDadosTipoDeCarga();
}

function pesoCarregamentoChange(e) {
    if (_carregamento.ModeloVeicularCarga.val() == "") {
        reiniciarCapacidadesCarregamento();
    }
    _carregamento.Peso.val(_carregamento.PesoCarregamento.val());
    _carregamento.PesoPallet.val(_carregamento.PesoPalletCarregamento.val());
    obterPesosEAjustarCapacidade();
    VerificarVisibilidadeBuscaSugestaoPedido();
}

function obterCodigoCarregamento(indice) {
    if (_Carregamentos) {
        if (_Carregamentos.length > 0 && _Carregamentos.length > indice) {
            return _Carregamentos[indice].Codigo;
        }
        else
            return null;
    }
    return null;
}

function otimizarTodosCarregamentos() {
    var codigo = obterCodigoCarregamento(0);
    if (codigo != null) {
        otimizarRotaMenuClick(codigo, 0);
    }
}

// Função que otimiza o percurso do carregamento.
// Parametros
// cod_carregamento: Código do carregamento a ser otimizado
// indica: Indice do carregamento da lista de _Carregamentos
//         Quando null, não informado irá otimizar apenas o carregamento informado.
//         Quando numerico, irá incremetar o indice até chegar o final da lista de _Carregamentos.
function otimizarRotaMenuClick(cod_carregamento, indice) {
    //Vamos obter os pontos..
    // vamos chamar o buscar rota..
    // Vamos reordenar os pontos...
    // Vamos apresentar a rota no mapa..
    carregarRoteiroCarregamento(cod_carregamento, false, function (result) {
        if (result) {
            //Aqui vamnos fazer um procedimento no server.. para retornar a lista dos pontos ordenados... igual na consulta dos carregametnos.
            gerarRoteirizacaoGoogleMapsOSM(false, function (respostaOrdenada) {
                //Agora vamos reordenar os pedidos de acordo com o retorno..
                var addPolylineFromOrig = true;
                if (_AreaCarregamento != null) {
                    addPolylineFromOrig = _AreaCarregamento.DesenharPolilinhaRotaApartirOrigem.val();
                }
                var coordinates = [];
                var pedidos_carregamento = obterNrosPedidosCarregamento(cod_carregamento);

                var aux = 0;
                if (addPolylineFromOrig) {
                    var ponto = ObterPontoRemetente();
                    if (ponto != null && ponto.lat != 0) {
                        coordinates[0] = { lat: ponto.lat, lng: ponto.lng, destinatario: 'C.D' };
                        aux = 1;
                    }
                }

                var pedidos_reordenados = [];
                for (var i in respostaOrdenada) {
                    // Contem o cpf/cnpj do destinatário..
                    var codigo = respostaOrdenada[i].codigo;
                    //Vamos achar o indice do marker pelo codigo do destinatário;
                    var index = obterIndiceMakerDestinatario(codigo, cod_carregamento);
                    if (index >= 0) {
                        coordinates[i + aux] = { lat: _arrayMarker[index].marker.position.lat(), lng: _arrayMarker[index].marker.position.lng() };
                        $(_arrayMarker[index].pedidos).each(function (p) {
                            var cp = _arrayMarker[index].pedidos[p].Codigo;
                            if ($.inArray(cp, pedidos_carregamento) >= 0) {
                                pedidos_reordenados.push(cp);
                            }
                        });
                    }
                }
                //Vamos validar se todos os pedidos foram atendidos...localizados
                if (pedidos_reordenados.length < pedidos_carregamento.length) {
                    $(pedidos_carregamento).each(function (p) {
                        if ($.inArray(pedidos_carregamento[p], pedidos_reordenados) < 0) {
                            pedidos_reordenados.push(pedidos_carregamento[p]);
                        }
                    });
                }
                atualizaNrosPedidosOrdenadoCarregamento(cod_carregamento, pedidos_reordenados);

                if (coordinates.length - aux > 0) {
                    drawPolyline(coordinates, true, cod_carregamento);
                }

                finalizarControleManualRequisicao();
                // Vamos salvar a roteirização no evento de otimizar o percurso...
                salvarRotaCarregamento();
                //Atualizar o KM
                ajustaKmTempoKnoutCarregamento(cod_carregamento, _roteirizadorCarregamento.Distancia.val(), _roteirizadorCarregamento.TempoDeViagemEmMinutos.val(), _roteirizadorCarregamento.PolilinhaRota.val());

                drawPolylineDirection(_roteirizadorCarregamento.PolilinhaRota.val());

                if (indice != null && indice != undefined && parseInt(indice) >= 0) {
                    setTimeout(function () {
                        indice++;
                        var codigo = obterCodigoCarregamento(indice);
                        if (codigo != null) {
                            otimizarRotaMenuClick(codigo, indice);
                        }
                    }, 2000);
                }
            });
        }
    });
}

function obterNrosPedidosCarregamento(cod_carregamento) {
    var index = obterIndiceCarregamentoCodigo(cod_carregamento);
    if (index >= 0)
        return _Carregamentos[index].Roteirizacao.Pedidos;
    else
        return [];
}

function atualizaNrosPedidosOrdenadoCarregamento(cod_carregamento, pedidos) {
    var index = obterIndiceCarregamentoCodigo(cod_carregamento);
    if (index >= 0) {
        _Carregamentos[index].Roteirizacao.Pedidos = pedidos;
    }
    return true;
}

function roteirizarCarregamentoSemModal(callback) {
    //Vamos obter os pontos..
    // vamos chamar o buscar rota..
    // Salvar a roteirização.. 
    carregarRoteiroCarregamento(_carregamento.Carregamento.codEntity(), false, function (result) {
        if (result) {
            //Gerando a roteirização do carregamento...
            gerarRoteirizacaoGoogleMapsOSM(false, function (respostaOrdenada) {

                finalizarControleManualRequisicao();
                // Vamos salvar a roteirização
                salvarRotaCarregamento(callback);

            });
        }
    });
}

function roteirizarCargaClick(e) {
    carregarRoteiroCarregamento(_carregamento.Carregamento.codEntity(), true);
    //calcularDistancias();
}

function tipoMontagemCargaChange(e) {
    buscarInformacoesTipoMontagem();
    PesquisarPedidos();
}

function loadCarregamento() {
    _carregamento = new CarregamentoMapa();
    KoBindings(_carregamento, "knoutCarregamento");

    _crudCarregamento = new CRUDCarregamento();
    KoBindings(_crudCarregamento, "knoutCRUDCarregamento");

    _tituloCarregamento = new TituloCarregamento();
    KoBindings(_tituloCarregamento, "koTituloCarregamento");

    BuscarPedidoViagemNavio(_carregamento.PedidoViagemNavio);
    BuscarModelosVeicularesCarga(_carregamento.ModeloVeicularCarga, retornoModeloVeicular);
    //new BuscarTiposdeCarga(_carregamento.TipoDeCarga, retornoTipoCargaCarregamento);
    BuscarTiposdeCarga(_carregamento.TipoDeCarga, retornoTipoCargaCarregamento, null, null, null, null, null, true);

    if (_CONFIGURACAO_TMS.TipoMontagemCargaPadrao === EnumTipoMontagemCarga.AgruparCargas)
        BuscarCarregamento(_carregamento.Carregamento, retornoCarregamento, EnumSituacaoCarregamento.obterSituacoesEmMontagem(), null, _CONFIGURACAO_TMS.TipoMontagemCargaPadrao, _sessaoRoteirizador.Codigo);
    else
        BuscarCarregamento(_carregamento.Carregamento, retornoCarregamento, EnumSituacaoCarregamento.obterSituacoesEmMontagem(), null, null, _sessaoRoteirizador.Codigo);

    var consultaPreCarga = new BuscarPreCarga(_carregamento.PreCarga, null, _carregamento.Filial, true);
    consultaPreCarga.SetFiltro('SemCarga', true);
    BuscarClientes(_carregamento.Recebedor);
    BuscarClientes(_carregamento.Expedidor);

    if (_CONFIGURACAO_TMS.UtilizaEmissaoMultimodal == true) {
        _carregamento.PedidoViagemNavio.required = true;

        _carregamento.ModeloVeicularCarga.required = false;
        _carregamento.ModeloVeicularCarga.visible(false);
        $("#divPesoMedida").hide();
    }

    if (_CONFIGURACAO_TMS.UtilizarDataPrevisaoSaidaVeiculo) {
        _carregamento.DataInicioViagemPrevista.visible(true);
    }


    BuscarTiposSeparacao(_carregamento.TipoSeparacao);

    loadCarregamentoTransporte();
    loadCarregamentoPedido();
    loadCarregamentoProdutos();
    //loadDirecoesGoogleMaps();
    loadPedidoMapa();
    loadBlocosCarregamento();
    loadCarregamentoAutorizacao();
    _precisarSetarPedidosSelecionadosTabelaPedidosSessao = true;
    PEDIDOS_SELECIONADOS.subscribe(PedidosSelecionadosChange);

    if (_CONFIGURACAO_TMS.TipoServicoMultisoftware == EnumTipoServicoMultisoftware.MultiTMS) {
        if (_CONFIGURACAO_TMS.InformaHorarioCarregamentoMontagemCarga) {
            _carregamento.DataCarregamento.def = Global.DataHoraAtual();
            _carregamento.DataCarregamento.val(Global.DataHoraAtual());
        } else {
            _carregamento.DataCarregamento.def = Global.DataAtual();
            _carregamento.DataCarregamento.val(Global.DataAtual());
        }
    }
}

function setConfiguracaoColeta() {
    _carregamento.Recebedor.required(_gerarCargasDeColeta);
    _carregamento.Recebedor.visible(_gerarCargasDeColeta);
}

function buscarPedidoBipagem() {

    if (_AreaPedido.Pedidos.val().length <= 0) {
        exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Falha, Localization.Resources.Cargas.MontagemCargaMapa.NenhumPedidoDisponivelNaSelecao);
        return;
    }

    if (_carregamento) {
        if (_carregamento.Carregamento.codEntity() > 0) {

            var TextBipagem = _carregamentoPedido.BipagemPedido.val();
            if (TextBipagem.length < 14) {
                exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Falha, Localization.Resources.Cargas.MontagemCargaMapa.CodigoDeBarrasInvalidoMesmoDeveConterQuatorzeDigitos);
                return;
            }

            var encontrou = false;
            for (var i = 0; i < _AreaPedido.Pedidos.val().length; i++) {
                var NumeroPedido = TextBipagem.substring(0, 8);
                var numeroPedidoEmbarcador = _AreaPedido.Pedidos.val()[i].NumeroPedidoEmbarcador;

                // se menor que 8, completamos com zeros..
                if (_AreaPedido.Pedidos.val()[i].NumeroPedidoEmbarcador.length > 8) {
                    numeroPedidoEmbarcador = _AreaPedido.Pedidos.val()[i].NumeroPedidoEmbarcador.substring(0, 8);
                } else
                    numeroPedidoEmbarcador = AddZeros(numeroPedidoEmbarcador, 8)

                if (numeroPedidoEmbarcador == NumeroPedido) {
                    encontrou = true;
                    var volumeBipagem = parseInt(TextBipagem.substring(8, 11));
                    var volumeBipagemTotal = parseInt(TextBipagem.substring(11, 14));

                    var pedido = PEDIDOS.where(function (ped) { return _AreaPedido.Pedidos.val()[i].Codigo == ped.Codigo; });
                    volumeBipagem += parseInt(_AreaPedido.Pedidos.val()[i].VolumesBipagem);
                    _AreaPedido.Pedidos.val()[i].QuantidadeBipada = volumeBipagem;
                    _AreaPedido.Pedidos.val()[i].QuantidadeBipagemTotal = volumeBipagemTotal;
                    _AreaPedido.Pedidos.val()[i].VolumesBipagem = volumeBipagem + "/" + volumeBipagemTotal;

                    adicionarPedidosCarregamentoBipagem(_carregamento.Carregamento.codEntity(), pedido.Codigo, volumeBipagem, volumeBipagemTotal, function () {
                        pedido.PedidoIntegradoCarregamento = true;
                        pedido.QuantidadeBipagemTotal = volumeBipagemTotal;
                        pedido.QuantidadeBipada = volumeBipagem;
                        SelecionarPedido(pedido, false);
                    });

                    continue;
                }
            }

            if (!encontrou) {
                exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Falha, Localization.Resources.Cargas.MontagemCargaMapa.NenhumPedidoEncontradoNaSelecao);
            }

            _carregamentoPedido.BipagemPedido.val("");
            $("#" + _carregamentoPedido.BipagemPedido.id).focus();

        } else {
            exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Falha, Localization.Resources.Cargas.MontagemCargaMapa.NecessarioSelecionarCriarUmCarregamentoParaIniciarBipagem);
            return;
        }
    } else {
        exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Falha, Localization.Resources.Cargas.MontagemCargaMapa.NecessarioSelecionarCriarUmCarregamentoParaIniciarBipagem);
        return;
    }
}

function atualizarCarregamentoClick() {
    var valido = true;

    if (!ValidarCamposObrigatorios(_carregamento))
        valido = false;

    if (!ValidarCamposObrigatorios(_carregamentoTransporte))
        valido = false;

    var incompletosBipagem = pedidosIncompletosBipagem();
    if (incompletosBipagem > 0) {
        exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, Localization.Resources.Cargas.MontagemCargaMapa.ExistemPedidosQueAindaNaoEstaoCompletosNoCarregamentoPorFavorVerifique.format(incompletosBipagem));
        return
    }

    if (valido) {
        if ((_CONFIGURACAO_TMS.TipoMontagemCargaPadrao === EnumTipoMontagemCarga.AgruparCargas && _carregamentoTransporte.Motoristas.basicTable.BuscarRegistros().length <= 0) || (_CONFIGURACAO_TMS.MotoristaObrigatorioMontagemCarga && _carregamentoTransporte.Motoristas.basicTable.BuscarRegistros().length <= 0 && !_CONFIGURACAO_TMS.DesativarMultiplosMotoristasMontagemCarga)) {
            exibirMensagem(tipoMensagem.atencao, Localization.Resources.Cargas.MontagemCargaMapa.MotoristaObrigatorio, Localization.Resources.Cargas.MontagemCargaMapa.ObrigatorioInformarOsMotoristas);
            return;
        }

        _pesquisaResumoCarregamentos.Reload.val(true);

        _carregamento.CarregamentoRedespacho.val(_objPesquisaMontagem.GerarCargasDeRedespacho);
        preencherListaMotorista();
        preencherListaAjudante();

        var carregamento = obterCarregamentoSalvar();

        executarReST("MontagemCarga/SalvarCarregamento", carregamento, function (arg) {
            if (arg.Success) {
                if (arg.Data !== false) {
                    exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, Localization.Resources.Cargas.MontagemCargaMapa.CarregamentoSalvoComSucesso);

                    _carregamento.Carregamento.codEntity(arg.Data.Codigo);
                    _carregamento.Carregamento.val(arg.Data.Descricao);
                    _carregamento.AlteracoesPendentes.val(false);
                    _carregamentoTransporte.TipoOperacao.NaoExigeRoteirizacaoMontagemCarga = arg.Data.NaoExigeRoteirizacaoMontagemCarga;

                    setarOpcoesCarregamento();
                    atualizarDadosPedidosSelecionados();

                    if (_CONFIGURACAO_TMS.TipoServicoMultisoftware == EnumTipoServicoMultisoftware.MultiTMS && _carregamento.TipoMontagemCarga.val() == EnumTipoMontagemCarga.NovaCarga)
                        _crudCarregamento.Imprimir.visible(true);

                    if (arg.Data.UtilizarPolilinhaRotaFrete === true)
                        drawPolylineDirection(arg.Data.PolilinhaRotaFrete);
                    else
                        drawPolylineDirection(arg.Data.PolilinhaRoteirizacao);

                    PesquisarCarregamentos();
                }
                else
                    exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Atencao, arg.Msg);
            }
            else
                exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, arg.Msg);
        });
    }
    else
        exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.CamposObrigatorios, Localization.Resources.Gerais.Geral.InformeCamposObrigatorios);
}

function AutorizarVeiculoClick() {
    exibirConfirmacao(Localization.Resources.Gerais.Geral.Confirmacao, Localization.Resources.Cargas.MontagemCargaMapa.RealmenteDesejaAutorizarGeracaoDaCargaDoVeiculoSelecionado, function () {
        var data = { Codigo: _carregamento.Carregamento.codEntity() };
        executarReST("MontagemCarga/AutorizarVeiculo", data, function (arg) {
            if (arg.Success) {
                if (arg.Data !== false) {
                    exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, Localization.Resources.Cargas.MontagemCargaMapa.AutorizadoComSucesso);
                    _crudCarregamento.AutorizarVeiculo.visible(false);

                } else {
                    exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Atencao, arg.Msg);
                }
            } else {
                exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Atencao, arg.Msg);
            }
        });
    });
}

function ImprimirClick(e, sender) {
    var data = { Codigo: _carregamento.Carregamento.codEntity(), Carregamento: true };
    executarReST("Pedido/GerarRelatorio", data, function (arg) {
        if (arg.Success) {
            if (arg.Data !== false) {
                exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, Localization.Resources.Gerais.Geral.AguardeQueSeuRelatorioEstaSendoGerado);
            } else {
                exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.Aviso, arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.Aviso, arg.Msg);
        }
    });
}

function iniciarNovoCarregamentoClick() {
    limparDadosCarregamento();
}

function gerarCargaClick() {
    var valido = true;

    if (!ValidarCamposObrigatorios(_carregamento))
        valido = false;

    if (!ValidarCamposObrigatorios(_carregamentoTransporte))
        valido = false;

    var incompletosBipagem = pedidosIncompletosBipagem();
    if (incompletosBipagem > 0) {
        exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, Localization.Resources.Cargas.MontagemCargaMapa.ExistemPedidosQueAindaNaoEstaoCompletosNoCarregamentoPorFavorVerifique.format(incompletosBipagem));
        return
    }

    if (valido) {
        executarReST("MontagemCarga/ConsultaInconsistenciaGrupoProduto", { ModeloVeicularCarga: _carregamento.ModeloVeicularCarga.codEntity(), Pedidos: JSON.stringify(ObterPedidosSelecionados()) }, function (arg) {
            if (arg.Success) {
                if (arg.Data !== false) {
                    var ok = false;
                    if (arg.Data.quantidadePedidosInconsistentes > 0)
                        exibirConfirmacao(Localization.Resources.Gerais.Geral.Confirmacao, Localization.Resources.Cargas.MontagemCargaMapa.ModeloVeicularNaoPermitidoParaGrupoDeProdutoDosPedidosDesejaContinuar, function () {
                            gerarCargaOk(gerarCargaClick);
                        });
                    else
                        gerarCargaOk(gerarCargaClick);

                } else {
                    exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Atencao, arg.Msg, 15000);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, arg.Msg);
            }
        });
    }
    else {
        exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.CamposObrigatorios, Localization.Resources.Cargas.MontagemCargaMapa.InformeOsCamposObrigatoriosDoCarregamento);
    }
}

function gerarCargaOk(callback) {
    var codigo = _carregamento.Carregamento.codEntity();
    if (codigo == 0) {
        atualizarCarregamentoClick(callback);
        return;
    }

    //#33331 
    // Verificando se o tipo de operação do pedido não obriga roteirizar.
    var tipoOperacaoNaoRequerRoteirizacao = PEDIDOS_SELECIONADOS().some(item => item.NaoExigeRoteirizacaoMontagemCarga === true);

    if (tipoOperacaoNaoRequerRoteirizacao === false) {
        // Verificando se o tipo de operação do carregamento não obriga roteirizar.
        tipoOperacaoNaoRequerRoteirizacao = _carregamentoTransporte.TipoOperacao.NaoExigeRoteirizacaoMontagemCarga;
    }

    if (PEDIDOS_SELECIONADOS().some(p => p.TipoOperacaoValidarValorMinimoCarga) && !_VALOR_MINIMO_VALIDADO) {
        ValidarValorMinimoCarga(_carregamentoTransporte.TipoOperacao.codEntity(), ObterCodigoPedidosSelecionados());
        return;
    }

    if (_CONFIGURACAO_TMS.TipoServicoMultisoftware == EnumTipoServicoMultisoftware.MultiEmbarcador && tipoOperacaoNaoRequerRoteirizacao === false) {
            var polilinhaRoteirizacao = _roteirizadorCarregamento.PolilinhaRota.val();
        if (polilinhaRoteirizacao == "") {
                roteirizarCarregamentoSemModal(callback);
                return;
            }
        }

    var balancas = _crudCarregamento.Balancas.val();
    if (balancas.length > 0) {
        var ocupacao = parseInt(_carregamento.LotacaoPeso.val());
        if (ocupacao >= 100) {
            exibirConfirmacao(Localization.Resources.Gerais.Geral.Confirmacao, Localization.Resources.Cargas.MontagemCargaMapa.PercursoDeRotaDesteCarregamentoPossuiBalancaTotalCarregadoSuperiorCapacidadeDoVeiculoDesejaContinuarMesmoAssim, function () {
                confirmGerarCarga(true);
            });
        } else {
            confirmGerarCarga(false);
        }
    } else {
        confirmGerarCarga(false);
    }
}

function confirmGerarCarga(ocupacaoExcedenteBalanca) {
    if (_CONFIGURACAO_TMS.MotoristaObrigatorioMontagemCarga && _carregamentoTransporte.Motoristas.basicTable.BuscarRegistros().length <= 0 && !_CONFIGURACAO_TMS.DesativarMultiplosMotoristasMontagemCarga) {
        exibirMensagem(tipoMensagem.atencao, Localization.Resources.Cargas.MontagemCargaMapa.MotoristaObrigatorio, Localization.Resources.Cargas.MontagemCargaMapa.ObrigatorioInformarOsMotoristas);
        return;
    }

    exibirConfirmacao(Localization.Resources.Gerais.Geral.Confirmacao, Localization.Resources.Cargas.MontagemCargaMapa.RealmenteDesejaGerarCargaDesteCarregamento, function () {
        preencherListaMotorista();
        preencherListaAjudante();

        var carregamento = obterCarregamentoSalvar(ocupacaoExcedenteBalanca);

        executarReST("MontagemCarga/GerarCarga", carregamento, function (arg) {
            if (arg.Success) {
                if (arg.Data !== false) {

                    if (arg.Data.GerandoCargaBackground)
                        exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, Localization.Resources.Cargas.MontagemCargaMapa.CarregamentoEmFilaParaProcessoDeGeracaoDeCarga);
                    else if (arg.Data.CarregamentoAguardandoAprovacao)
                        exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, Localization.Resources.Cargas.MontagemCargaMapa.CarregamentoAguardandoAprovacaoParaGerarCarga);
                    else
                        exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, Localization.Resources.Cargas.MontagemCargaMapa.CargaGeradaComSucesso);

                    if (!arg.Data.GerandoCargaBackground) {
                        limparDadosCarregamento();
                        BuscarDadosMontagemCarga(2);
                    }
                    else {
                        var codigo = _carregamento.Carregamento.codEntity();
                        var index = obterIndiceCarregamentoCodigo(codigo);
                        if (index >= 0) {
                            _Carregamentos[index].GerandoCargaBackground = true;
                            index = obterIndiceKnoutCarregamento(codigo);
                            if (index >= 0) {
                                _knoutsCarregamentos[index].GerandoCargaBackground.val(true);
                            }
                        }
                        limparDadosCarregamento();
                    }

                } else {
                    exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Atencao, arg.Msg, 15000);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, arg.Msg);
            }
        });
    });
}

function cancelarCarregamentoClick() {
    var codigo = _carregamento.Carregamento.codEntity();
    exibirConfirmacao(Localization.Resources.Gerais.Geral.Confirmacao, Localization.Resources.Cargas.MontagemCargaMapa.RealmenteDesejaCancelarCarregamento, function () {
        _pesquisaResumoCarregamentos.Reload.val(true);
        executarReST("MontagemCarga/CancelarCarregamento", { Codigo: codigo }, function (arg) {
            if (arg.Success) {
                if (arg.Data !== false) {
                    if (permiteAdicionarNotaManualmente()) {
                        var index = obterIndiceCarregamentoCodigo(codigo);
                        if (index >= 0) {
                            _Carregamentos.splice(index, 1);
                        }
                        disposeCarregamentoMapa(codigo);
                    }
                    exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, Localization.Resources.Cargas.MontagemCargaMapa.CarregamentoCanceladoComSucesso);
                    limparDadosCarregamento();
                    BuscarDadosMontagemCarga(2);
                } else {
                    exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Atencao, arg.Msg);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, arg.Msg);
            }
        });
    });
}

function validaOpcoesCarregamentoSessaoFinalizada() {
    if (_sessaoRoteirizador != null) {
        if (_sessaoRoteirizador.SituacaoSessaoRoteirizador.val() == 2) { // Finalizada
            _crudCarregamento.Limpar.visible(false);
        } else {
            _crudCarregamento.Limpar.visible(true);
        }
    }
}

function retornoCarregamento(carregamento) {
    //PEDIDOS_SELECIONADOS.removeAll();
    executarReST("MontagemCarga/BuscarPorCodigo", carregamento, function (arg) {
        if (arg.Success) {
            if (arg.Data !== false) {

                PreencherCarregamento(arg.Data, carregamento.Codigo);

                validaOpcoesCarregamentoSessaoFinalizada();

                //Atualizando o percentual da loja...
                if (_sessaoRoteirizador.MontagemCarregamentoPedidoProduto.val()) {
                    progressPercentualCarregamentoProdutos(arg.Data, false, 0);
                }
                //Recarregar o outro carregamento.
                //Comentado por questão de performance....o mesmo é processado ao clicar no editar para abrir a janela de edição..
                // Removido comentário e adicionado para reconsultar apenas quando o Outro carregamento está visível.
                if (modalOutroCarregamentoVisible()) {
                    editarProdutoCarregamentoClick(true);
                }
            } else {
                exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Atencao, arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, arg.Msg);
        }
    });
}

function removerPedidosCarregamento(cod_carregamento, codigos_pedidos, callbackSucess) {
    executarReST("MontagemCarga/RemoverPedidosCarregamento", { Codigo: cod_carregamento, PedidosCodigo: JSON.stringify(codigos_pedidos) }, function (arg) {
        if (arg.Success) {
            if (arg.Data !== false) {
                // Remover o pedido da objeto _Carregametnos...
                removerCarregamentoPedidos(cod_carregamento, codigos_pedidos);
                if (callbackSucess != undefined) {
                    callbackSucess();
                }
            } else {
                exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Atencao, arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, arg.Msg);
        }
    });
}

function removerCarregamentoPedidos(cod_carregamento, codigos_pedidos) {
    if (cod_carregamento > 0) {
        var index = obterIndiceCarregamentoCodigo(cod_carregamento);
        if (index >= 0) {
            for (i in codigos_pedidos) {
                var idx_pedido = $.inArray(codigos_pedidos[i], _Carregamentos[index].Roteirizacao.Pedidos);
                if (idx_pedido >= 0) {
                    _Carregamentos[index].Roteirizacao.Pedidos.splice(idx_pedido, 1);
                }
            }
        }
    }
}

function adicionarPedidosCarregamento(cod_carregamento, codigos_pedidos, callbackSucess) {
    executarReST("MontagemCarga/AdicionarPedidosCarregamento", { Codigo: cod_carregamento, PedidosCodigo: JSON.stringify(codigos_pedidos) }, function (arg) {
        if (arg.Success) {
            if (arg.Data !== false) {
                if (callbackSucess != undefined) {
                    callbackSucess();
                }
            } else {
                exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Atencao, arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, arg.Msg);
        }
    });
}

function adicionarPedidosCarregamentoBipagem(cod_carregamento, codigo_pedido, volumes_bipagem, volumes_total, callbackSucess) {
    executarReST("MontagemCarga/AdicionarPedidosCarregamentoBipagem", { Codigo: cod_carregamento, CodigoPedido: codigo_pedido, BipagemPedido: volumes_bipagem, BipagemPedidoTotal: volumes_total }, function (arg) {
        if (arg.Success) {
            if (arg.Data !== false) {
                if (callbackSucess != undefined) {
                    callbackSucess();
                }
            } else {
                exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Atencao, arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, arg.Msg);
        }
    });
}

function modeloVeicularCargaBlur() {
    if (_carregamento.ModeloVeicularCarga.val() == "")
        limparDadosModeloVeicularCarga();
}

function retornoModeloVeicular(dadosModelo) {
    preencherModeloVeicularCarga(dadosModelo);
}

function PedidosSelecionadosChange() {
    if (EVITAR_SUBSCRIBE_PEDIDOS_SELECIONADOS) return;

    let peso = 0;
    let pesoLiquido = 0;
    let pallets = 0;
    let cubagem = 0;
    let pesoRestante = 0;
    let volume = 0;
    let entregas = 0;
    let valor = 0;

    const pedidos = PEDIDOS_SELECIONADOS();
    for (let i in pedidos) {
        const pedido = pedidos[i];
        peso += Globalize.parseFloat(pedido.Peso);
        pesoLiquido += Globalize.parseFloat(pedido.PesoLiquido);
        cubagem += Globalize.parseFloat(pedido.Cubagem);
        pallets += Globalize.parseFloat(pedido.TotalPallets);
        pesoRestante += Globalize.parseFloat(pedido.PesoSaldoRestante);
        volume += Globalize.parseFloat(pedido.Volumes);
        valor += Globalize.parseFloat(pedido.Valor);
    }

    if (_sessaoRoteirizador.TipoRoteirizacaoColetaEntrega.val() == EnumTipoRoteirizacaoColetaEntrega.Coleta) {
        entregas = PEDIDOS_SELECIONADOS().map(item => item.Remetente).filter((value, index, self) => self.indexOf(value) === index);
    } else {
        entregas = PEDIDOS_SELECIONADOS().map(item => item.Destinatario).filter((value, index, self) => self.indexOf(value) === index);
    }

    // Atualizando totais pedidos selecionados area pedidos.
    _AreaPedido.TotalPedidosSelecionados.val(pedidos.length);
    _AreaPedido.PesoTotalSelecionados.val(Globalize.format(peso, "n3"));
    _AreaPedido.PesoLiquidoTotalSelecionados.val(Globalize.format(pesoLiquido, "n3"));
    _AreaPedido.PesoSaldoRestanteSelecionados.val(Globalize.format(pesoRestante, "n3"));
    _AreaPedido.VolumeTotalSelecionados.val(volume);
    _AreaPedido.ValorPedidosSelecionados.val(Globalize.format(valor, "n2"));

    _AreaPedido.TotalEntregasSelecionados.val(entregas.length);
    _AreaPedido.TotalCubagemSelecionados.val(cubagem);

    const dataCarregamentoPorDoca = _carregamento.EscolherHorarioCarregamentoPorLista.val();

    ajustarCapacidades(peso, cubagem, pallets);
    RenderizarGridMotagemPedidos();
    VerificarVisibilidadeBuscaSugestaoPedido();

    _carregamento.PreCarga.required(isCampoPreCargaObrigatorio());

    //if (PEDIDOS_SELECIONADOS() != null && PEDIDOS_SELECIONADOS().some(p => p.TipoOperacaoExigirInformarDataPrevisaoInicioViagem)) {
    //    _carregamento.DataInicioViagemPrevista.visible(true);
    //    _carregamento.DataInicioViagemPrevista.required(true);
    //    _carregamento.DataInicioViagemPrevista.text(Localization.Resources.Cargas.MontagemCargaMapa.DataPrevisaoInicioViagem.getFieldDescription())

    //}

    RemarcarPontosPedidosMapa();
    //#9388
    if (_carregamento.Carregamento.codEntity() == 0) {

        const datasPedidosSelecionados = _CONFIGURACAO_TMS.InformaHorarioCarregamentoMontagemCarga ? PEDIDOS_SELECIONADOS().map(item => item.DataHoraCarregamentoPedido) : PEDIDOS_SELECIONADOS().map(item => item.DataCarregamentoPedido);
        const datas = datasPedidosSelecionados.filter((value, index, self) => self.indexOf(value) === index);

        if (!dataCarregamentoPorDoca && _carregamento.DataCarregamento.val() == '') {
            _carregamento.DataCarregamento.val(datas.length == 1 ? datas[0] : '');
        }

        const tiposDeCarga = PEDIDOS_SELECIONADOS().map(item => item.TipoCarga).filter((value, index, self) => self.indexOf(value) === index);

        if (tiposDeCarga.length >= 1) {
            let codigo = 0;
            let descricao = '';
            if (tiposDeCarga.length == 1) {
                const tiposDeCargaCodigos = PEDIDOS_SELECIONADOS().map(item => item.CodigoTipoCarga).filter((value, index, self) => self.indexOf(value) === index);
                codigo = tiposDeCargaCodigos[0];
                descricao = tiposDeCarga[0];
            } else {
                //#21034 - Setar automaticamente a com maior peso.
                let peso = 0;
                for (let i in tiposDeCarga) {
                    const tipoDeCarga = tiposDeCarga[i];
                    const pedidosTipoCarga = PEDIDOS_SELECIONADOS().filter(p => p.TipoCarga === tipoDeCarga).map(p => p);
                    const tmp = pedidosTipoCarga.reduce((sum, pedido) => {
                        let pesoPedido = pedido.Peso;
                        pesoPedido = pesoPedido.replace('.', '');
                        pesoPedido = pesoPedido.replace(',', '.');
                        return sum + parseFloat(pesoPedido);
                    }, 0);

                    if (tmp > peso) {
                        peso = tmp;
                        descricao = tipoDeCarga;
                        codigo = pedidosTipoCarga[0].CodigoTipoCarga;
                    }
                }
            }
            _carregamentoTransporte.TipoDeCarga.val(descricao);
            _carregamentoTransporte.TipoDeCarga.entityDescription(descricao);
            _carregamentoTransporte.TipoDeCarga.codEntity(codigo);
        } else {
            LimparCampo(_carregamento.TipoDeCarga);
            limparDadosTipoDeCarga();
        }

        if (!_sessaoRoteirizador.PreencherAutomaticamenteDadosCentroTelaMontagemCarga.val()) {

            const tiposDeOperacao = PEDIDOS_SELECIONADOS().map(item => item.CodigoTipoOperacao).filter((value, index, self) => self.indexOf(value) === index);

            if (tiposDeOperacao.length == 1) {
                _carregamentoTransporte.TipoOperacao.val(PEDIDOS_SELECIONADOS()[0].TipoOperacao);
                _carregamentoTransporte.TipoOperacao.entityDescription(PEDIDOS_SELECIONADOS()[0].TipoOperacao);
                _carregamentoTransporte.TipoOperacao.codEntity(PEDIDOS_SELECIONADOS()[0].CodigoTipoOperacao);
                _carregamentoTransporte.Recebedor.visible(PEDIDOS_SELECIONADOS()[0].TipoOperacaoInformarRecebedor);
            } else {
                _carregamentoTransporte.TipoOperacao.val('');
                _carregamentoTransporte.TipoOperacao.entityDescription('');
                _carregamentoTransporte.TipoOperacao.codEntity(0);
            }
        }

        if (!_sessaoRoteirizador.PreencherAutomaticamenteDadosCentroTelaMontagemCarga.val()) {
            const modelosVeicularesCarga = PEDIDOS_SELECIONADOS().map(item => item.ModeloVeicularCarga).filter((value, index, self) => self.map(item => item.Codigo).indexOf(value.Codigo) === index);

            if ((modelosVeicularesCarga.length == 1) && (modelosVeicularesCarga[0].Codigo > 0)) {
                preencherModeloVeicularCarga(modelosVeicularesCarga[0]);
                //} else {
                //    LimparCampo(_carregamento.ModeloVeicularCarga);
                //    limparDadosModeloVeicularCarga();
            }
        }

        if (_CONFIGURACAO_TMS.InformarTipoCondicaoPagamentoMontagemCarga) {
            const tiposCondicaoPagamento = PEDIDOS_SELECIONADOS().map(item => item.TipoCondicaoPagamento).filter((value, index, self) => self.indexOf(value) === index);

            if (tiposCondicaoPagamento.length == 1)
                _carregamento.TipoCondicaoPagamento.val(tiposCondicaoPagamento[0]);
            else
                _carregamento.TipoCondicaoPagamento.val(EnumTipoCondicaoPagamento.Todos);
        }

        const codigosExpedidores = PEDIDOS_SELECIONADOS().map(item => item.CodigoExpedidor).filter((value, index, self) => self.indexOf(value) === index);

        if (codigosExpedidores.length == 1) {
            if (PEDIDOS_SELECIONADOS()[0].CodigoExpedidor > 0) {
                _carregamentoTransporte.Expedidor.val(PEDIDOS_SELECIONADOS()[0].Expedidor);
                _carregamentoTransporte.Expedidor.entityDescription(PEDIDOS_SELECIONADOS()[0].Expedidor);
                _carregamentoTransporte.Expedidor.codEntity(PEDIDOS_SELECIONADOS()[0].CodigoExpedidor);
            } else {
                _carregamentoTransporte.Expedidor.val('');
                _carregamentoTransporte.Expedidor.entityDescription('');
                _carregamentoTransporte.Expedidor.codEntity(0);
            }
        } else {
            _carregamentoTransporte.Expedidor.val('');
            _carregamentoTransporte.Expedidor.entityDescription('');
            _carregamentoTransporte.Expedidor.codEntity(0);
        }
    }

    buscarCapacidadeJanelaCarregamento(buscarPeriodoCarregamento);

    if (_CONFIGURACAO_TMS.TipoServicoMultisoftware == EnumTipoServicoMultisoftware.MultiTMS)
        ObterDetalhesPedidoParaMontagemCarga(pedidos);

    opcoesPedidosSelecionados();

    if (_precisarSetarPedidosSelecionadosTabelaPedidosSessao)
        setarPedidosSelecionadosTabelaPedidosSessao();

}

function ObterDetalhesPedidoParaMontagemCarga(pedidos) {
    let codigosPedidos = new Array();

    for (let i = 0; i < pedidos.length; i++)
        codigosPedidos.push(pedidos[i].Codigo);

    executarReST("MontagemCarga/ObterDetalhesPedidosMontagemCarga", { Pedidos: JSON.stringify(codigosPedidos), TipoOperacao: _carregamentoTransporte.TipoOperacao.codEntity() }, function (r) {
        if (r.Success) {
            if (r.Data) {
                if (!string.IsNullOrWhiteSpace(r.Data.Mensagem)) {
                    $("#divAlertaPedidosCarregamento").html('<i class="fal fa-info-circle"></i>&nbsp;<span>' + r.Data.Mensagem + '</span>');
                    $("#divAlertaPedidosCarregamento").show();
                } else {
                    $("#divAlertaPedidosCarregamento").hide();
                }
            } else {
                exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Atencao, r.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, r.Msg);
        }
    });
}

function downloadEDIClick(e) {
    executarDownload("MontagemCarga/BaixarEDICarregamento", { Codigo: _carregamento.Carregamento.codEntity() });
}

function escolherHorarioDisponivelClick() {
    alterarDataCarregamentoMontagemCarga(_carregamento);
}

//*******MÉTODOS*******
function PesquisarCarregamentos() {
    //if (_CONFIGURACAO_TMS.TipoMontagemCargaPadrao === EnumTipoMontagemCarga.Todos || _CONFIGURACAO_TMS.TipoMontagemCargaPadrao === EnumTipoMontagemCarga.AgruparCargas) {
    limparAreaCarregamentos();
    buscarCarregamentos();
    setConfiguracaoColeta();
    //}
}

function limparAreaCarregamentos() {
    $("#" + _AreaCarregamento.Carregamentos.id).html("");
    _AreaCarregamento.Inicio.val(0);
    _knoutsCarregamentos = new Array();
}

function buscarInformacoesTipoMontagem() {
    if (_carregamento.TipoMontagemCarga.val() == EnumTipoMontagemCarga.NovaCarga) {
        if (_CONFIGURACAO_TMS.TipoMontagemCargaPadrao !== EnumTipoMontagemCarga.Todos) {
            if (_CONFIGURACAO_TMS.PermiteAdicionarNotaManualmente)
                $("#liMapa").show();
        }
    }
    else {
        $("#liCargas").show();
        //$("#liCargas a").click();
        $("#liPedidos").hide();
        $("#liAreaCargas").show();
        $("#liAreaCargas a").click();
        $("#liAreaPedidos").hide();
        if (_CONFIGURACAO_TMS.TipoMontagemCargaPadrao !== EnumTipoMontagemCarga.Todos) {
            $("#liAreaCargas").show();
            $("#liAreaCargas a").click();
            $("#liAreaPedidos").hide();
            $("#liMapa").hide();
        }
        //PEDIDOS_SELECIONADOS.removeAll();
        LimparPedidosSelecionados();
        _carregamento.PreCarga.required(false);

        LimparCampoEntity(_carregamento.PreCarga);

        LimparPedidosSelecionados();
        RenderizarGridMotagemPedidos();
        buscarCapacidadeJanelaCarregamento();
    }
}

function setarOpcoesCarregamento() {
    if (_carregamento.TipoMontagemCarga.val() == EnumTipoMontagemCarga.NovaCarga) {
        var permitirEditarCarregamento = EnumSituacaoCarregamento.permitirEditarCarregamento(_carregamento.Situacao.val());

        _crudCarregamento.Atualizar.visible(permitirEditarCarregamento);
        _crudCarregamento.GerarCarga.visible(permitirEditarCarregamento);
        _crudCarregamento.Roteirizacao.visible(permitirEditarCarregamento);
        _crudCarregamento.SimularFrete.visible(permitirEditarCarregamento);
        _crudCarregamento.DownloadEDI.visible(false);
        _crudCarregamento.Bloco.visible(!_carregamento.CarregamentoRedespacho.val() && permitirEditarCarregamento);

        _carregamentoProdutos.EditarProdutoCarregamento.visible(true);

        if (_CONFIGURACAO_TMS.TipoServicoMultisoftware == EnumTipoServicoMultisoftware.MultiEmbarcador && EnumSituacaoCarregamento.permitirCancelarCarregamento(_carregamento.Situacao.val()))
            _crudCarregamento.Cancelar.visible(true);
    }

    _carregamento.TipoMontagemCarga.enable(false);
}

function ConfigurarCentroCarregamento(centroCarregamento) {
    if (centroCarregamento.EscolherHorarioCarregamentoPorLista) {
        _carregamento.DataCarregamento.val(_carregamento.DataCarregamento.def);
    }
}

function PreencherCarregamento(carregamento, codigo) {
    //Removido consulta de motorista/s da consutla de todos os carregamentos, quando selecionar um carregamento vai atualizar.
    var balancas = new Array();
    var pracasPedagio = new Array();
    if (codigo != null) {
        var index = obterIndiceCarregamentoCodigo(codigo);

        if (index >= 0) {
            _Carregamentos[index].Carregamento.Transporte.Motorista = carregamento.Transporte.Motorista;
            _Carregamentos[index].Carregamento.Transporte.ListaMotoristas = carregamento.Transporte.ListaMotoristas;
            _Carregamentos[index].Carregamento.Transporte.ListaAjudantes = carregamento.Transporte.ListaAjudantes;
            var marcado = false;
            if (_AreaCarregamento != null) {
                marcado = _AreaCarregamento.ExibirPolilinhaRotaQuandoRoteirizado.val();
            }
            if (marcado) {
                drawPolylineDirection(_Carregamentos[index].Roteirizacao.PolilinhaRota);
            }
            balancas = _Carregamentos[index].Roteirizacao.Balancas;
            pracasPedagio = _Carregamentos[index].Roteirizacao.PracasPedagio;
        } else {
            disposeDirection();
        }
    } else {
        disposeDirection();
    }

    limparCarregamentoTransporte();
    LimparPedidosSelecionados();

    try {
        _preenchendoDadosCarregamento = true;

        PreencherObjetoKnout(_carregamento, { Data: carregamento.Carregamento });

        _crudCarregamento.SimularFrete.visible(false);
        _crudCarregamento.Bloco.visible(false);
        _crudCarregamento.DownloadEDI.visible(false);
        _crudCarregamento.Balancas.val(balancas);
        _crudCarregamento.PracasPedagio.val(pracasPedagio);

        _carregamento.PreCarga.required(isCampoPreCargaObrigatorio());
        _carregamento.AlteracoesPendentes.val(false);

        if (carregamento.Carregamento.VeiculoBloqueado) {
            if (VerificaSePossuiPermissaoEspecial(EnumPermissaoPersonalizada.MontagemCarga_LiberacaoVeiculo, _PermissoesPersonalizadasCarga))
                _crudCarregamento.AutorizarVeiculo.visible(true);
            else
                _crudCarregamento.AutorizarVeiculo.visible(false);
        }
        else
            _crudCarregamento.AutorizarVeiculo.visible(false);

        if (_CONFIGURACAO_TMS.TipoServicoMultisoftware == EnumTipoServicoMultisoftware.MultiTMS && _carregamento.TipoMontagemCarga.val() == EnumTipoMontagemCarga.NovaCarga)
            _crudCarregamento.Imprimir.visible(true);

        preencherDadosTransporte(carregamento.Transporte);

        if (_carregamento.CarregamentoRedespacho.val() != _objPesquisaMontagem.GerarCargasDeRedespacho) {
            _pesquisaMontegemCarga.GerarCargasDeRedespacho.val(_carregamento.CarregamentoRedespacho.val());
            BuscarDadosMontagemCarga(2);
        };

        RenderizarGridMotagemPedidos();
        desmarcarKnoutsCarregamentos();
        preencherModeloVeicularCarga(carregamento.Carregamento.ModeloVeicularCarga);

        //Selecionando os pedidos do carregamento
        CarregarPedidosCarragmento(carregamento.Carregamento.Pedidos);
        ajustarPesosCapacidades(carregamento);

        setarOpcoesCarregamento();
        buscarInformacoesTipoMontagem();

        ValidarFronteira();
        //if (_CONFIGURACAO_TMS.PermiteAdicionarNotaManualmente) {
        AtualizarPontosFaltantes();
        //Aki vamos centralizar a carga no mapa...
        var centralizar = _AreaCarregamento.CentralizarCarregamentoMapaAoSelecionar.val();
        if (centralizar) {
            var markersSelecionados = obterMarkersSelecionados();
            if (markersSelecionados.length > 0) {
                var bounds = new google.maps.LatLngBounds();
                for (i = 0; i < markersSelecionados.length; i++) {
                    bounds.extend(markersSelecionados[i].marker.position);
                }
                _map.fitBounds(bounds);
            }
        }
        //}

        buscarCapacidadeJanelaCarregamento();
        preencherCarregamentoAutorizacao(carregamento.Carregamento.Carregamento.Codigo);
        // 
        validaProdutosCarregamento(carregamento)
    }
    finally {
        _preenchendoDadosCarregamento = false;
    }
}

function preencherModeloVeicularCarga(dadosModeloVeicularCarga) {

    _carregamento.ModeloVeicularCarga.codEntity(dadosModeloVeicularCarga.Codigo);
    _carregamento.ModeloVeicularCarga.val(dadosModeloVeicularCarga.Descricao);
    _carregamento.ModeloVeicularCarga.numeroReboques = dadosModeloVeicularCarga.NumeroReboques;
    _carregamento.ModeloVeicularCarga.exigirDefinicaoReboquePedido = dadosModeloVeicularCarga.ExigirDefinicaoReboquePedido;
    _carregamento.ModeloVeicularCarga.OcupacaoCubicaPaletes = dadosModeloVeicularCarga.OcupacaoCubicaPaletes;
    _carregamento.CapacidadePeso.val(dadosModeloVeicularCarga.CapacidadePesoTransporte);
    _carregamento.CapacidadeCubagem.val(dadosModeloVeicularCarga.Cubagem);
    _carregamento.CapacidadePallets.val(dadosModeloVeicularCarga.NumeroPaletes);
    _carregamento.ToleranciaMinimaPaletes.val(dadosModeloVeicularCarga.ToleranciaMinimaPaletes);
    _carregamento.ToleranciaMinimaCubagem.val(dadosModeloVeicularCarga.ToleranciaMinimaCubagem);
    _carregamento.ToleranciaPesoMenor.val(dadosModeloVeicularCarga.ToleranciaPesoMenor);

    if (dadosModeloVeicularCarga.ModeloControlaCubagem) {
        _carregamento.Cubagem.visible(true);
        _carregamentoProdutos.Cubagem.visible(true);
        _carregamentoProdutos.CubagemLoja.visible(true);
    } else {
        _carregamento.Cubagem.visible(false);
        _carregamentoProdutos.Cubagem.visible(false);
        _carregamentoProdutos.CubagemLoja.visible(false);
    }

    if (dadosModeloVeicularCarga.VeiculoPaletizado) {
        _carregamento.Pallets.visible(true);
        _carregamentoProdutos.Pallets.visible(true);
        _carregamentoProdutos.PalletsLoja.visible(true);
    } else {
        _carregamento.Pallets.visible(false);
        _carregamentoProdutos.Pallets.visible(false);
        _carregamentoProdutos.PalletsLoja.visible(false);
    }

    obterPesosEAjustarCapacidade();
    VerificarVisibilidadeBuscaSugestaoPedido();
    carregarDefinicaoReboquePedidosSelecionados();
    carregarDefinicaoTipoCarregamentoPedidosSelecionados();
}

function validaProdutosCarregamento(carregamento) {

    _montagemPedidoProduto = carregamento.MontagemCarregamentoPedidoProduto;
    if (_montagemPedidoProduto) {
        $("#liCarregamentoProdutos").show();
        //Ajustando os tipos de pedidos do carregamento
        let clientes = [];
        let clientesOptions = [{ text: "", value: '0-' + carregamento.Carregamento.Carregamento.Codigo }];

        if (carregamento.ClientesCarregamento != null) {
            for (var i = 0; i < carregamento.ClientesCarregamento.length; i++) {
                clientes.push({ active: (i === 0 ? 'active' : ''), icon: 'fal fa-lg fa-list', text: carregamento.ClientesCarregamento[i].Nome, value: carregamento.ClientesCarregamento[i].CPF_CNPJ, carregamento: carregamento.Carregamento.Carregamento.Codigo });
                clientesOptions.push({
                    text: carregamento.ClientesCarregamento[i].Nome,
                    value: carregamento.ClientesCarregamento[i].CPF_CNPJ + "-" + carregamento.Carregamento.Carregamento.Codigo,
                });
            }
        }

        _carregamentoProdutos.ClientesCarregamento.val(clientes);
        _carregamentoProdutos.ClienteCarregamentoSelect.options(clientesOptions);
        
        if (clientes.length > 0) {
            selecionouAbaClienteProdutosCarregamento(clientes[0]);
        }

        //Ajustando os tipos de pedidos do carregamento
        var tipos = [
            { active: 'nav-link', icon: 'fal fa-lg fa-truck', tipo: 'TOTAL', value: 0, carregamento: carregamento.Codigo }
        ];
        if (carregamento.CanaisDeEntrega != null) {
            for (var i = 0; i < carregamento.CanaisDeEntrega.length; i++) {
                tipos.push({ active: 'nav-link', icon: 'fal fa-lg fa-list', tipo: carregamento.CanaisDeEntrega[i].Descricao, value: carregamento.CanaisDeEntrega[i].Codigo, carregamento: carregamento.Codigo });
            }
        }
        _carregamentoProdutos.TiposDePedidos.val(tipos);
        //Carregando os valores da aba TODOS
        carregamentoProdutosResumoLoja(tipos[0]);
    } else {

        $("#liCarregamentoProdutos").hide();

        //#41781, não possui nenhuma categoria de cliente cadastrada
        // mas quer que aparaceça "COR" no carregamento da categorias dos clientes :-/
        var categorias = [];
        var cores = [];
        if (carregamento.ClientesCarregamento != null) {
            for (var i = 0; i < carregamento.ClientesCarregamento.length; i++) {
                var cor = carregamento.ClientesCarregamento[i].Cor;
                if (cor.length > 0) {
                    if (!cores.includes(cor)) {
                        cores.push(cor);
                        categorias.push({ Categoria: carregamento.ClientesCarregamento[i].Categoria, Cor: cor })
                    }
                }
            }
        }
        // Atualizando as cores...
        var index = obterIndiceKnoutCarregamento(carregamento.Carregamento.Carregamento.Codigo);
        if (index >= 0) {
            _knoutsCarregamentos[index].CategoriasPessoas.visible(cores.length > 0)
            if (cores.length > 0) {
                _knoutsCarregamentos[index].CategoriasPessoas.val(categorias);
            }
        }
    }
}

function ajustarPesosCapacidades(carregamento, usar_pedidos_selecionados, selecionarKnout) {

    if (selecionarKnout == null || selecionarKnout == undefined) {
        selecionarKnout = true;
    }

    var peso = 0;
    var pesoPallet = 0;
    var cubagem = 0;
    var pallets = 0;

    var totalizadoresPedidos = null;
    if (usar_pedidos_selecionados == true) {
        totalizadoresPedidos = totalizadoresPedidosCarregamento(PEDIDOS_SELECIONADOS());
    } else {
        totalizadoresPedidos = totalizadoresPedidosCarregamento(carregamento.Carregamento.Pedidos);//CarregarPedidosCarragmento(carregamento.Carregamento.Pedidos);
    }
    peso = totalizadoresPedidos.peso;
    pesoPallet = totalizadoresPedidos.pesoPallet;
    cubagem = totalizadoresPedidos.cubagem;
    pallets = totalizadoresPedidos.pallets;

    ajustarCapacidades(peso, cubagem, pallets, pesoPallet);
    //Ajustando a knout carregamento
    ajustarCapacidadesKnoutCarregamento(carregamento.Carregamento.Carregamento.Codigo, carregamento.Carregamento.ModeloVeicularCarga, carregamento.Transporte.TipoDeCarga, peso, cubagem, pallets, totalizadoresPedidos.qtde_entregas, totalizadoresPedidos.qtde_pedidos, totalizadoresPedidos.qtde_iddemanda, selecionarKnout, carregamento.Roteirizacao);
}

function ajustarCapacidadesKnoutCarregamento(codigo, dadosModeloVeicularCarga, dadosTipoDeCarga, peso, cubagem, pallets, qtde_entregas, qtde_pedidos, qtde_iddemanda, selecionarKnout, roteirizacao) {
    var index = obterIndiceKnoutCarregamento(codigo);
    if (index >= 0) {
        if ((qtde_entregas + qtde_pedidos) == 0) {
            var tmp = _knoutsCarregamentos[index].QtdeEntregasPedidos.val();
            var ep = tmp.split('/');

            if (ep.length == 2) {
                qtde_entregas = ep[0];
                qtde_pedidos = ep[1];
            } else if (ep.length == 3) {
                qtde_entregas = ep[0];
                qtde_pedidos = ep[1];
                qtde_iddemanda = ep[2];
            }
        }

        ajustarQuantidadesKnoutCarregamento(_knoutsCarregamentos[index], dadosModeloVeicularCarga, dadosTipoDeCarga, peso, cubagem, pallets, qtde_entregas, qtde_pedidos, qtde_iddemanda, selecionarKnout, roteirizacao)
    }
}

function ajustarQuantidadesKnoutCarregamento(knout, dadosModeloVeicularCarga, dadosTipoDeCarga, peso, cubagem, pallets, qtde_entregas, qtde_pedidos, qtde_iddemanda, selecionada, roteirizacao) {
    if (knout) {
        if (selecionada) {
            knout.InfoCarregamento.cssClass("card card-carga-selecionada");
        }

        if (Boolean(dadosTipoDeCarga) && Boolean(dadosTipoDeCarga.Paletizado))
            cubagem += Globalize.parseFloat(dadosModeloVeicularCarga.OcupacaoCubicaPaletes);

        knout.Peso.val(Globalize.format(peso, "n2"));
        knout.Pallets.val(Globalize.format(pallets, "n2"));
        knout.Cubagem.val(Globalize.format(cubagem, "n2"));
        knout.QtdeEntregasPedidos.val(qtde_entregas + '/' + qtde_pedidos + '/' + qtde_iddemanda);

        var maximo = _sessaoRoteirizadorParametros.QuantidadeMaximaEntregasRoteirizar.val();
        knout.AlertaLimiteEntregas.val(maximo < qtde_entregas && maximo > 0);

        //Ajustar km e tempo...
        if (roteirizacao != null && roteirizacao != undefined) {
            knout.Distancia.val(Globalize.format(roteirizacao.DistanciaKM, "n2") + " km");
            knout.TempoDeViagemEmMinutos.val(formatHora(roteirizacao.TempoDeViagemEmMinutos));
        }
        knout.QuantidadeEntregras.val(qtde_entregas);
        if (dadosModeloVeicularCarga.ModeloControlaCubagem)
            knout.Cubagem.visible(true);
        else
            knout.Cubagem.visible(false);

        if (dadosModeloVeicularCarga.VeiculoPaletizado)
            knout.Pallets.visible(true);
        else
            knout.Pallets.visible(false);

        var capacidadePeso = Globalize.parseFloat(dadosModeloVeicularCarga.CapacidadePesoTransporte);
        var capacidadePallets = Globalize.parseFloat(dadosModeloVeicularCarga.NumeroPaletes);
        var capacidadeCubagem = Globalize.parseFloat(dadosModeloVeicularCarga.Cubagem);

        knout.CapacidadePeso.val(n2(capacidadePeso));
        knout.CapacidadePallets.val(n2(capacidadePallets));
        knout.CapacidadeCubagem.val(n2(capacidadeCubagem));

        //Ajustar as cores
        var toleranciaPesoMenor = Globalize.parseFloat(dadosModeloVeicularCarga.ToleranciaPesoMenor);
        var cor = "";
        var corAprovado = "#9dde88ad";
        var corReprovado = "#FF6347";
        var corExcedida = "#FFFF00";
        cor = "";
        if (peso > 0) {
            if (peso >= toleranciaPesoMenor)
                cor = (peso > capacidadePeso) ? corExcedida : corAprovado
            else
                cor = corReprovado;
        }

        $("#" + knout.ProgressPeso.id).css("background-color", cor);

        if (selecionada && cor == corExcedida) {
            $("#" + knout.Peso.id).css("color", '#666');
        }

        if (capacidadePeso > 0) {
            knout.OcupacaoPeso.val((peso * 100) / capacidadePeso);
        }

        cor = "";
        if (capacidadePallets > 0) {
            var toleranciaMinimaPaletes = Globalize.parseFloat(dadosModeloVeicularCarga.ToleranciaMinimaPaletes);
            if (pallets >= toleranciaMinimaPaletes)
                cor = (pallets > capacidadePallets) ? corExcedida : corAprovado;
            else
                cor = corReprovado;
            knout.OcupacaoPallets.val((pallets * 100) / capacidadePallets);
        }

        $("#" + knout.ProgressPallets.id).css("background-color", cor);

        if (selecionada && cor == corExcedida) {
            $("#" + knout.Pallets.id).css("color", '#666');
        }

        cor = "";
        if (capacidadeCubagem > 0) {
            var toleranciaMinimaCubagem = Globalize.parseFloat(dadosModeloVeicularCarga.ToleranciaMinimaCubagem);
            if (cubagem >= toleranciaMinimaCubagem)
                cor = (cubagem > capacidadeCubagem) ? corExcedida : corAprovado;
            else
                cor = corReprovado;

            knout.OcupacaoCubagem.val((cubagem * 100) / capacidadeCubagem);
        }

        $("#" + knout.ProgressCubagem.id).css("background-color", cor);

        if (selecionada && cor == corExcedida) {
            $("#" + knout.Cubagem.id).css("color", '#666');
        }
    }
}

function ajustaKmTempoKnoutCarregamento(codigo, km, tempo, polilinha) {
    var index = obterIndiceKnoutCarregamento(codigo);
    if (index >= 0) {
        _knoutsCarregamentos[index].Distancia.val(km);                 ///Globalize.format(km, "n2") + " km");
        _knoutsCarregamentos[index].TempoDeViagemEmMinutos.val(formatHora(tempo));
    }
    index = obterIndiceCarregamentoCodigo(codigo);
    if (index >= 0) {
        if (km.indexOf('km') >= 0) {
            var tmp = km.replace(' km', '');
            tmp = tmp.replace(',', '.');
            km = parseFloat(tmp);
        }
        _Carregamentos[index].Roteirizacao.DistanciaKM = km;
        if (polilinha != null) {
            _Carregamentos[index].Roteirizacao.PolilinhaRota = polilinha;
        }
        AtualizarKMTotalCarregamentos();
    }
}

function CarregarPedidoProtudosCarragmento(produtosCarregamento) {
    var peso = 0;
    var pesoPallet = 0;
    var cubagem = 0;
    var pallets = 0;
    for (var i = 0; i < produtosCarregamento.length; i++) {
        var produto = produtosCarregamento[i];
        peso += Globalize.parseFloat(produto.PesoCarregar); // Globalize.parseFloat(pedido.Peso);
        //pesoPallet += Globalize.parseFloat(produto.PesoCarregar); // Globalize.parseFloat(pedido.Peso);
        cubagem += Globalize.parseFloat(produto.MetroCarregar); //.Metro);
        pallets += Globalize.parseFloat(produto.PalletCarregar);// produto.Pallet);
    }

    return {
        peso: peso,
        pesoPallet: pesoPallet,
        cubagem: cubagem,
        pallets: pallets,
    };
}

function CarregarPedidosCarragmento(pedidosCarregamento) {
    let pedidos = PEDIDOS();

    let peso = 0;
    let pesoPallet = 0;
    let cubagem = 0;
    let pallets = 0;
    let destinos = [];

    EVITAR_SUBSCRIBE_PEDIDOS_SELECIONADOS = true;
    for (let i = 0; i < pedidosCarregamento.length; i++) {
        let pedido = pedidosCarregamento[i];
        let pedidoListado = PEDIDOS.update(function (ped) { return ped.Codigo == pedido.Codigo }, function (ped) { ped.Selecionado = true; setarInformacoesBipagem(pedido, ped); return ped; });
        if (!pedidoListado) {
            pedido.Selecionado = true;
            PEDIDOS_NAO_LISTADOS.push(pedido);
        }
        VerificarPontosFaltantes(pedido);
        peso += pedido.PesoPedidoCarregamento;
        pesoPallet += pedido.PesoPalletPedidoCarregamento;
        cubagem += pedido.CubagemPedidoCarregamento;
        pallets += pedido.PalletPedidoCarregamento;
        if (pedido.QuantidadeBipada > 0) {
            if ((pedido.QuantidadeBipagemTotal - pedido.QuantidadeBipada) <= 0) {
                pedido.PedidoSelecionadoCompleto = true;
            }
            else {
                pedido.PedidoSelecionadoCompleto = false;
            }
            pedido.VolumesBipagem = pedido.QuantidadeBipada + "/" + pedido.QuantidadeBipagemTotal;
        } else {
            if (pedido.QuantidadeBipagemTotal <= 0 && pedido.QuantidadeBipada <= 0) {
                pedido.PedidoSelecionadoCompleto = true;
                pedido.VolumesBipagem = "0";
            }
        }

        PEDIDOS_SELECIONADOS.remove(function (ped) { return ped.Codigo == pedido.Codigo });
        PEDIDOS_SELECIONADOS.push(pedido);

        let destino = pedido.EnderecoDestino.Destinatario;
        if (pedido.EnderecoRecebedor != null)
            destino = pedido.EnderecoRecebedor.Destinatario;

        if ($.inArray(destino, destinos) < 0) {
            destinos.push(destino);
        }
    }
    EVITAR_SUBSCRIBE_PEDIDOS_SELECIONADOS = false;
    _precisarSetarPedidosSelecionadosTabelaPedidosSessao = true;
    PedidosSelecionadosChange();

    pedidos = PEDIDOS_NAO_LISTADOS().concat(pedidos);
    PEDIDOS(pedidos);

    return {
        peso: peso,
        pesoPallet: pesoPallet,
        cubagem: cubagem,
        pallets: pallets,
        qtde_entregas: destinos.length,
        qtde_pedidos: pedidosCarregamento.length
    }
}

function setarInformacoesBipagem(pedidocarregamento, pedidoListado) {
    if (pedidocarregamento.QuantidadeBipada > 0) {
        if ((pedidocarregamento.QuantidadeBipagemTotal - pedidocarregamento.QuantidadeBipada) <= 0) {
            //selecionou todos os volumes do pedido, deve ficar azul (ta no cshtml)
            pedidoListado.PedidoSelecionadoCompleto = true;
        }
        else {
            //ainda nao selecionou todos os volumes do pedido.. tem q ficar amarelo.. (ta no cshtml)
            pedidoListado.PedidoSelecionadoCompleto = false;
        }

        pedidoListado.VolumesBipagem = pedidocarregamento.QuantidadeBipada + "/" + pedidocarregamento.QuantidadeBipagemTotal;
    } else {
        if (pedidocarregamento.QuantidadeBipagemTotal <= 0 && pedidocarregamento.QuantidadeBipada <= 0) {
            pedidoListado.PedidoSelecionadoCompleto = true;
            pedidoListado.VolumesBipagem = "0";
        }
    }
}

function totalizadoresPedidosCarregamento(pedidosCarregamento) {
    var peso = 0;
    var pesoPallet = 0;
    var cubagem = 0;
    var pallets = 0;
    var destinos = [];
    var qtde_iddemanda = 0;
    for (var i = 0; i < pedidosCarregamento.length; i++) {
        var pedido = pedidosCarregamento[i];
        //Está adicionando o pedido no carregamento.. peso no carregamento ainda não definido...
        if (pedido.PesoPedidoCarregamento == undefined) {
            peso += Globalize.parseFloat(pedido.Peso);
        } else {
            peso += pedido.PesoPedidoCarregamento;
        }

        if (pedido.PesoPalletPedidoCarregamento == undefined) {
            pesoPallet += 0;
        } else {
            pesoPallet += pedido.PesoPalletPedidoCarregamento;
        }

        if (pedido.PalletPedidoCarregamento == undefined) {
            pallets += Globalize.parseFloat(pedido.TotalPallets);
        } else {
            pallets += pedido.PalletPedidoCarregamento;
        }

        if (pedido.CubagemPedidoCarregamento == undefined) {
            cubagem += Globalize.parseFloat(pedido.Cubagem);
        } else {
            cubagem += pedido.CubagemPedidoCarregamento;
        }

        var destino = pedido.EnderecoDestino.Destinatario;
        var latLng = pedido.EnderecoDestino.Latitude + pedido.EnderecoDestino.Longitude;
        if (pedido.EnderecoRecebedor != null) {
            destino = pedido.EnderecoRecebedor.Destinatario;
            latLng = pedido.EnderecoRecebedor.Latitude + pedido.EnderecoRecebedor.Longitude;
        }

        var chave = destino + latLng;
        if ($.inArray(chave, destinos) < 0) {
            destinos.push(chave);
        }

        if (pedido.QuantidadeIdDemanda != undefined) {
            if (!isNaN(pedido.QuantidadeIdDemanda))
                qtde_iddemanda += pedido.QuantidadeIdDemanda;
        }
    }
    return {
        peso: peso,
        pesoPallet: pesoPallet,
        cubagem: cubagem,
        pallets: pallets,
        qtde_entregas: destinos.length,
        qtde_pedidos: pedidosCarregamento.length,
        qtde_iddemanda: qtde_iddemanda
    }
}

function obterPesosEAjustarCapacidade() {
    var peso = Globalize.parseFloat(_carregamento.Peso.val());
    var pesoPallet = 0;
    if (_carregamento.PesoPallet.val() != undefined)
        pesoPallet = Globalize.parseFloat(_carregamento.PesoPallet.val());
    var pallets = Globalize.parseFloat(_carregamento.Pallets.val());
    var cubagem = Globalize.parseFloat(_carregamento.Cubagem.val()) - Globalize.parseFloat(_carregamento.CubagemPaletes.val());

    ajustarCapacidades(peso, cubagem, pallets, pesoPallet);
}

function ajustarCapacidades(peso, cubagem, pallets, pesoPallet) {
    if (_carregamento.Carregamento.codEntity() > 0 && PEDIDOS_SELECIONADOS().length == 1)
        peso = Globalize.parseFloat(_carregamento.PesoCarregamento.val());

    var cubagemPaletes = _carregamento.TipoDeCarga.Paletizado ? Globalize.parseFloat(_carregamento.ModeloVeicularCarga.OcupacaoCubicaPaletes) : 0;

    _carregamento.CubagemPaletes.val(Globalize.format(cubagemPaletes, "n2"));

    cubagem += cubagemPaletes;

    var capacidadePeso = Globalize.parseFloat(_carregamento.CapacidadePeso.val());
    var capacidadePallets = Globalize.parseFloat(_carregamento.CapacidadePallets.val());
    var capacidadeCubagem = Globalize.parseFloat(_carregamento.CapacidadeCubagem.val());
    var toleranciaPesoMenor = Globalize.parseFloat(_carregamento.ToleranciaPesoMenor.val());

    var cor = "";
    var corAprovado = "#9dde88ad";
    var corReprovado = "#FF6347";
    var corExcedida = "#FFFF00";

    cor = "";
    if (peso > 0) {
        if (peso >= toleranciaPesoMenor)
            cor = (peso > capacidadePeso) ? corExcedida : corAprovado
        else
            cor = corReprovado;
    }
    $("#" + _carregamento.Peso.id).css("background-color", cor);

    var lotacaoPeso = 0;
    if (capacidadePeso > 0)
        lotacaoPeso = (peso * 100) / capacidadePeso;
    _carregamento.LotacaoPeso.val(Globalize.format(lotacaoPeso, "n4"));

    cor = "";
    var lotacaoPallets = 0;
    if (capacidadePallets > 0) {
        var toleranciaMinimaPaletes = Globalize.parseFloat(_carregamento.ToleranciaMinimaPaletes.val());
        if (pallets >= toleranciaMinimaPaletes)
            cor = (pallets > capacidadePallets) ? corExcedida : corAprovado;
        else
            cor = corReprovado;

        lotacaoPallets = (pallets * 100) / capacidadePallets;
        _carregamento.LotacaoPallets.val(Globalize.format(lotacaoPallets, "n2"));
    }
    $("#" + _carregamento.Pallets.id).css("background-color", cor);


    cor = "";
    var lotacaoCubagem = 0;
    if (capacidadeCubagem > 0) {
        var toleranciaMinimaCubagem = Globalize.parseFloat(_carregamento.ToleranciaMinimaCubagem.val());
        if (cubagem >= toleranciaMinimaCubagem)
            cor = (cubagem > capacidadeCubagem) ? corExcedida : corAprovado;
        else
            cor = corReprovado;

        lotacaoCubagem = (cubagem * 100) / capacidadeCubagem;
        _carregamento.LotacaoCubagem.val(Globalize.format(lotacaoCubagem, "n2"));
    }
    $("#" + _carregamento.Cubagem.id).css("background-color", cor);

    _carregamento.Peso.val(Globalize.format(peso, "n4"));
    _carregamento.PesoPallet.val(Globalize.format(pesoPallet, "n4"));
    _carregamento.Cubagem.val(Globalize.format(cubagem, "n2"));
    _carregamento.Pallets.val(Globalize.format(pallets, "n2"));

    _carregamentoProdutos.Peso.val(Globalize.format(peso, "n4"));
    _carregamentoProdutos.PesoPallet.val(Globalize.format(pesoPallet, "n4"));
    _carregamentoProdutos.Cubagem.val(Globalize.format(cubagem, "n2"));
    _carregamentoProdutos.Pallets.val(Globalize.format(pallets, "n2"));

    if (_carregamento.Carregamento.codEntity() == 0)
        _carregamento.PesoCarregamento.val(_carregamento.Peso.val());
}

function reiniciarCapacidadesCarregamento() {
    _carregamento.CapacidadePeso.val(_carregamento.CapacidadePeso.def);
    _carregamento.LotacaoPeso.val(_carregamento.LotacaoPeso.def);
    _carregamento.CapacidadePallets.val(_carregamento.CapacidadePallets.def);
    _carregamento.LotacaoPallets.val(_carregamento.LotacaoPallets.def);
    _carregamento.CapacidadeCubagem.val(_carregamento.CapacidadeCubagem.def);
    _carregamento.LotacaoCubagem.val(_carregamento.LotacaoCubagem.def);
    _carregamento.Pallets.visible(true);
    _carregamento.Cubagem.visible(true);
}

function limparDadosCarregamento() {
    $("#" + _carregamento.Peso.id).css("background-color", "");
    $("#" + _carregamento.Pallets.id).css("background-color", "");
    $("#" + _carregamento.Cubagem.id).css("background-color", "");

    reiniciarCapacidadesCarregamento();
    LimparCampos(_carregamento);
    limparCarregamentoPedido();
    limparCarregamentoProdutos(false);
    limparCarregamentoProdutos(true);
    limparCarregamentoTransporte();
    LimparSimulacaoFrete();

    _carregamento.ModeloVeicularCarga.numeroReboques = 0;
    _carregamento.ModeloVeicularCarga.exigirDefinicaoReboquePedido = false;
    _carregamento.ModeloVeicularCarga.OcupacaoCubicaPaletes = "0";
    _carregamento.TipoDeCarga.Paletizado = false;
    _carregamento.TipoMontagemCarga.enable(true);
    _carregamento.DataCarregamento.enable(true);
    _carregamento.ModeloVeicularCarga.enable(true);
    _carregamento.PedidoViagemNavio.enable(true);
    _carregamento.PreCarga.required(false);

    _carregamentoProdutos.EditarProdutoCarregamento.visible(false);

    _crudCarregamento.Atualizar.visible(!sessaoRoteirizadorFinalizada());
    _crudCarregamento.AutorizarVeiculo.visible(false);
    _crudCarregamento.Bloco.visible(false);
    _crudCarregamento.Cancelar.visible(false);
    _crudCarregamento.GerarCarga.visible(false);
    _crudCarregamento.Imprimir.visible(false);
    _crudCarregamento.Roteirizacao.visible(false);
    _crudCarregamento.SimularFrete.visible(false);

    desmarcarKnoutsCarregamentos();
    LimparPedidosSelecionados();
    RenderizarGridMotagemPedidos();
    limparCamposCapacidadeJanelaCarregamento();
    limparCamposPeriodoCarregamento();
    buscarInformacoesTipoMontagem();
    LimparPontoMarcados();
    limparCarregamentoAutorizacao();
    ObterPontosPedidos();
    carregarDefinicaoReboquePedidosSelecionados();
    carregarDefinicaoTipoCarregamentoPedidosSelecionados();
    obterDadosPadrao();

    if (_CONFIGURACAO_TMS.TipoServicoMultisoftware == EnumTipoServicoMultisoftware.MultiTMS) {
        if (_CONFIGURACAO_TMS.InformaHorarioCarregamentoMontagemCarga) {
            _carregamento.DataCarregamento.def = Global.DataHoraAtual();
            _carregamento.DataCarregamento.val(Global.DataHoraAtual());
        }
        else {
            _carregamento.DataCarregamento.def = Global.DataAtual();
            _carregamento.DataCarregamento.val(Global.DataAtual());
        }
    }
}

function limparDadosModeloVeicularCarga() {
    _carregamento.ModeloVeicularCarga.numeroReboques = 0;
    _carregamento.ModeloVeicularCarga.exigirDefinicaoReboquePedido = false;
    _carregamento.ModeloVeicularCarga.OcupacaoCubicaPaletes = "0";
    reiniciarCapacidadesCarregamento();
    obterPesosEAjustarCapacidade();
    VerificarVisibilidadeBuscaSugestaoPedido();
    carregarDefinicaoReboquePedidosSelecionados();
    carregarDefinicaoTipoCarregamentoPedidosSelecionados();
}

function limparDadosTipoDeCarga() {
    _carregamento.TipoDeCarga.Paletizado = false;

    obterPesosEAjustarCapacidade();
    buscarCapacidadeJanelaCarregamento(buscarPeriodoCarregamento);
}

function isCampoPreCargaObrigatorio() {
    var pedidos = PEDIDOS_SELECIONADOS();
    for (var i in pedidos) {
        var pedido = pedidos[i];
        if (pedido.ExigirPreCargaMontagemCarga)
            return true;
    }

    return false;
}

function obterCarregamentoSalvar(ocupacaoExcedenteBalanca) {
    var carregamento = {
        Carregamento: JSON.stringify(RetornarObjetoPesquisa(_carregamento)),
        Pedidos: JSON.stringify(ObterPedidosSelecionados()),
        Transporte: JSON.stringify(RetornarObjetoPesquisa(_carregamentoTransporte)),
        ListaMotoristas: _carregamentoTransporte.ListaMotoristas.val(),
        ListaAjudantes: _carregamentoTransporte.ListaAjudantes.val(),
        CarregamentoRedespacho: _carregamento.CarregamentoRedespacho.val(),
        SessaoRoteirizador: _pesquisaMontegemCarga.SessaoRoteirizador.codEntity(),
        OcupacaoExcedenteBalanca: Boolean(ocupacaoExcedenteBalanca), // true: quando a rota tem balança e a ocupação do veiculo é >= 100%
        NotasParaEnviar: JSON.stringify([])
    };

    carregamento["DadosPorFilialPadrao"] = JSON.stringify({
        DataCarregamento: _carregamento.DataCarregamento.val(),
        DataDescarregamento: _carregamento.DataDescarregamento.val(),
        EncaixarHorario: _carregamento.EncaixarHorario.val(),
        DataInicioViagemPrevista: _carregamento.DataInicioViagemPrevista.val()
    });

    return carregamento;
}

function setarRecebedorCarregamento(pedido) {
    if ((_gerarCargasDeColeta) && (_carregamento.Recebedor.val() == "")) {
        _carregamento.Recebedor.codEntity(pedido.CodigoRecebedorColeta);
        _carregamento.Recebedor.entityDescription(pedido.DescricaoRecebedorColeta);
        _carregamento.Recebedor.val(pedido.DescricaoRecebedorColeta);
    }

}

function setarDataCarregamento(pedido) {
    if ((_gerarCargasDeColeta) && (_carregamento.DataCarregamento.val() == "") && !_carregamento.EscolherHorarioCarregamentoPorLista.val()) {
        _carregamento.DataCarregamento.val(_CONFIGURACAO_TMS.InformaHorarioCarregamentoMontagemCarga ? pedido.DataHoraCarregamentoPedido : pedido.DataCarregamentoPedido);
    }
}

function setarModeloVeicularCarga(pedido) {
    if ((_gerarCargasDeColeta) && (_carregamento.ModeloVeicularCarga.val() == "")) {
        preencherModeloVeicularCarga(pedido.ModeloVeicularCarga);
    }
}

function setarTipoCondicaoCarregamento(pedido) {
    if (_CONFIGURACAO_TMS.InformarTipoCondicaoPagamentoMontagemCarga && (_gerarCargasDeColeta) && (_carregamento.TipoCondicaoPagamento.val() == EnumTipoCondicaoPagamento.Todos))
        _carregamento.TipoCondicaoPagamento.val(pedido.TipoCondicaoPagamento);
}

function SetarGerandoCargaBackgroundFinalizado(dados) {
    Global.fecharModal("knockoutPercentualCargaEmLote");
    // Atualizar o ribbon para finalizado...
    if (dados.erro !== "") {
        exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, Localization.Resources.Cargas.MontagemCargaMapa.OcorreuUmErroAoGerarCargaDoCarregamento.format(dados.NumeroCarregamento, dados.erro));
        atualizaRibbonKnoutCarregamentoCargaBackgroundFinalizado(dados.Carregamento, false);
    } else {
        /* erro,
           SessaoRoterizador ,
           Carregamento  
           NumeroCarregamento */
        exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, Localization.Resources.Cargas.MontagemCargaMapa.CargaReferenteAoCarregamentoGeradaComSucesso.format(dados.NumeroCarregamento));
        atualizaRibbonKnoutCarregamentoCargaBackgroundFinalizado(dados.Carregamento, true);
    }
}

function ValidarValorMinimoCarga(codigoTipoOperacao, codigosPedidos) {
    executarReST("MontagemCarga/ValidarValorMinimoPorCarga", { CodigoTipoOperacao: codigoTipoOperacao, CodigosPedidos: JSON.stringify(codigosPedidos) }, function (arg) {
        if (arg.Success) {
            if (arg.Data.CargasNaoAtingiramValorMinimo != null && arg.Data.CargasNaoAtingiramValorMinimo.length > 0) {
                const mensagem = MontarTabelaRetornoComCargaNaoAtingiramValorMinimo(arg.Data.CargasNaoAtingiramValorMinimo);
                exibirConfirmacaoComTamanhoMaior(Localization.Resources.Cargas.MontagemCargaMapa.Confirmacao, mensagem, function () {
                    _VALOR_MINIMO_VALIDADO = true;
                    PEDIDOS_SELECIONADOS.remove(function (pedido) {
                        return arg.Data.PedidosParaRemoverCarregamento.includes(pedido.Codigo);
                    });

                    PedidosSelecionadosChange();

                    removerPedidosCarregamento(_carregamento.Carregamento.codEntity(), arg.Data.PedidosParaRemoverCarregamento, function () {
                        gerarCargaOk()
                    });
                });
            } else {
                _VALOR_MINIMO_VALIDADO = true;
                gerarCargaOk();
            }
        } else {
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, arg.Msg);
        }
    });
}
function MontarTabelaRetornoComCargaNaoAtingiramValorMinimo(pedidosCargasNaoAtingiramValorMinimo) {
    let html = "<div>";
    html += "<h4>Serão removidas as cargas que não atingiram o valor mínimo previsto no Tipo de Operação. Deseja confirmar a remoção?</h4> <br>";
    html += "<table class='table' > <thead> <tr> <th scope='col'>Números pedidos</th> <th scope='col'>Filial</th> <th scope='col'>Valor da carga</th> <th scope='col'>Valor mínimo da carga</th> </tr> </thead>";
    html += "<tbody>";
    for (let i in pedidosCargasNaoAtingiramValorMinimo) {
        html += "<tr>";
        html += `<td>${pedidosCargasNaoAtingiramValorMinimo[i].NumerosPedido}</td>`;
        html += `<td>${pedidosCargasNaoAtingiramValorMinimo[i].Filial}</td>`;
        html += `<td>${pedidosCargasNaoAtingiramValorMinimo[i].ValorTotal}</td>`;
        html += `<td>${pedidosCargasNaoAtingiramValorMinimo[i].ValorMinimoCarga}</td>`;
        html += '</tr>';
    }

    html += "</tbody> </table > </div >";

    return html;

    return html;
}