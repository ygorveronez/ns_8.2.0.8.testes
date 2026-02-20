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
/// <reference path="../../Enumeradores/EnumTipoDistancia.js" />
/// <reference path="TabelaFrete.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _gridDistancia, _distancia, _distanciaDadosGerais;

var Distancia = function () {
    this.Grid = PropertyEntity({ type: types.local });
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Tipo = PropertyEntity({ val: ko.observable(EnumTipoDistancia.PorFaixaDistanciaPercorrida), options: EnumTipoDistancia.ObterOpcoes(), text: Localization.Resources.Fretes.TabelaFrete.TipoDeCobranca.getRequiredFieldDescription(), def: EnumTipoDistancia.PorFaixaDistanciaPercorrida });
    this.QuilometragemInicial = PropertyEntity({ type: types.map, text: Localization.Resources.Fretes.TabelaFrete.KMInicial.getFieldDescription(), val: ko.observable(Globalize.format(0, "n2")), def: Globalize.format(0, "n2"), getType: typesKnockout.decimal, visible: ko.observable(true), maxlength: 13, configDecimal: { precision: 2, allowZero: true } });
    this.QuilometragemFinal = PropertyEntity({ type: types.map, text: Localization.Resources.Fretes.TabelaFrete.KMFinal.getFieldDescription(), val: ko.observable(Globalize.format(0, "n2")), def: Globalize.format(0, "n2"), getType: typesKnockout.decimal, visible: ko.observable(true), maxlength: 13, configDecimal: { precision: 2, allowZero: true } });
    this.Quilometros = PropertyEntity({ type: types.map, text: "*KM:", val: ko.observable(Globalize.format(0, "n2")), def: Globalize.format(0, "n2"), getType: typesKnockout.decimal, visible: ko.observable(false), maxlength: 13, configDecimal: { precision: 2, allowZero: true } });
    this.MultiplicarValorDaFaixa = PropertyEntity({ text: Localization.Resources.Fretes.TabelaFrete.MultiplicarValorDaFaixaDeDistanciaPelaDistanciaPercorrida, issue: 1394, val: ko.observable(false), def: false, getType: typesKnockout.bool, visible: ko.observable(true) });
    this.MultiplicarPeloResultadoDaDistancia = PropertyEntity({ text: Localization.Resources.Fretes.TabelaFrete.MultiplicarPeloResultadoDaDistancia, val: ko.observable(false), def: false, getType: typesKnockout.bool, visible: ko.observable(true) });
    this.MultiplicarValorFixoFaixaDistanciaPeloPesoCarga = PropertyEntity({ text: Localization.Resources.Fretes.TabelaFrete.MultiplicarValorFixoFaixaDistanciaPeloPesoCarga, val: ko.observable(false), def: false, getType: typesKnockout.bool, visible: ko.observable(true) });

    this.Adicionar = PropertyEntity({ eventClick: adicionarDistanciaClick, type: types.event, text: Localization.Resources.Fretes.TabelaFrete.Adicionar, visible: ko.observable(true) });

    this.Tipo.val.subscribe(function (novoValor) {
        ChangeTipoDistancia(novoValor);
    });
}

var DistanciaDadosGerais = function () {
    this.PermiteValorAdicionalQuilometragemExcedente = PropertyEntity({ text: Localization.Resources.Fretes.TabelaFrete.PermitirValorAdicionalPorKMExcedente, val: ko.observable(false), def: false, getType: typesKnockout.bool, visible: ko.observable(true), enable: ko.observable(true) });
    this.QuilometragemExcedente = PropertyEntity({ type: types.map, text: Localization.Resources.Fretes.TabelaFrete.KMExcedente.getRequiredFieldDescription(), issue: 701, val: ko.observable(""), def: "", getType: typesKnockout.decimal, visible: ko.observable(false), maxlength: 13 });
    this.UsarTabelaApenasQuandoDistanciaInformadaNaIntegracaoDaCarga = PropertyEntity({ text: Localization.Resources.Fretes.TabelaFrete.UtilizarTabelaApenasQuandoInformadoIntegracao, val: ko.observable(false), def: false, required: false, getType: typesKnockout.bool, visible: ko.observable(true) });
    this.ComponenteFreteQuilometragem = PropertyEntity({ text: Localization.Resources.Fretes.TabelaFrete.InformarValorComponenteDeFrete, visible: ko.observable(true), type: types.entity, codEntity: ko.observable(0), idBtnSearch: guid() });
    this.UtilizarComponenteFreteQuilometragem = PropertyEntity({ val: ko.observable(false), def: false, getType: typesKnockout.bool });
    this.ComponenteFreteQuilometragemExcedente = PropertyEntity({ text: Localization.Resources.Fretes.TabelaFrete.InformarExcedenteComponenteFrete.getFieldDescription(), visible: ko.observable(true), type: types.entity, codEntity: ko.observable(0), idBtnSearch: guid() });
    this.UtilizarComponenteFreteQuilometragemExcedente = PropertyEntity({ val: ko.observable(false), def: false, getType: typesKnockout.bool });
    this.MultiplicarValorPorPallet = PropertyEntity({ text: Localization.Resources.Fretes.TabelaFrete.MultiplicarValorPorPallets, val: ko.observable(false), def: false, required: false, getType: typesKnockout.bool, visible: ko.observable(true) });
    this.UsarCubagemComoParametroDeDistancia = PropertyEntity({ text: Localization.Resources.Fretes.TabelaFrete.UsarCubagemNoLugarDeKM, val: ko.observable(false), def: false, required: false, getType: typesKnockout.bool, visible: ko.observable(true) });
    this.UsarCalculoFretePorPedido = PropertyEntity({ text: Localization.Resources.Fretes.TabelaFrete.UsarDistanciasPedidosDaCarga, val: ko.observable(false), def: false, required: false, getType: typesKnockout.bool, visible: ko.observable(true) });
}

//*******EVENTOS*******

function loadDistancia() {

    _distancia = new Distancia();
    KoBindings(_distancia, "knockoutDistancia");

    _distanciaDadosGerais = new DistanciaDadosGerais();
    KoBindings(_distanciaDadosGerais, "knockoutDistanciaDadosGerais");

    _tabelaFrete.PermiteValorAdicionalQuilometragemExcedente = _distanciaDadosGerais.PermiteValorAdicionalQuilometragemExcedente;
    _tabelaFrete.QuilometragemExcedente = _distanciaDadosGerais.QuilometragemExcedente;
    _tabelaFrete.ComponenteFreteQuilometragem = _distanciaDadosGerais.ComponenteFreteQuilometragem;
    _tabelaFrete.UtilizarComponenteFreteQuilometragem = _distanciaDadosGerais.UtilizarComponenteFreteQuilometragem;
    _tabelaFrete.UsarTabelaApenasQuandoDistanciaInformadaNaIntegracaoDaCarga = _distanciaDadosGerais.UsarTabelaApenasQuandoDistanciaInformadaNaIntegracaoDaCarga;
    _tabelaFrete.ComponenteFreteQuilometragemExcedente = _distanciaDadosGerais.ComponenteFreteQuilometragemExcedente;
    _tabelaFrete.UtilizarComponenteFreteQuilometragemExcedente = _distanciaDadosGerais.UtilizarComponenteFreteQuilometragemExcedente;
    _tabelaFrete.MultiplicarValorPorPallet = _distanciaDadosGerais.MultiplicarValorPorPallet;
    _tabelaFrete.UsarCubagemComoParametroDeDistancia = _distanciaDadosGerais.UsarCubagemComoParametroDeDistancia;
    _tabelaFrete.UsarCalculoFretePorPedido = _distanciaDadosGerais.UsarCalculoFretePorPedido;

    _tabelaFrete.ContratoFreteTransportadorPossuiFranquia.val.subscribe(PossuiFranquiaAlterado);

    new BuscarComponentesDeFrete(_distanciaDadosGerais.ComponenteFreteQuilometragem);
    LimparComponentePorFlag(_tabelaFrete.ComponenteFreteQuilometragem, _tabelaFrete.UtilizarComponenteFreteQuilometragem);

    new BuscarComponentesDeFrete(_distanciaDadosGerais.ComponenteFreteQuilometragemExcedente);
    LimparComponentePorFlag(_tabelaFrete.ComponenteFreteQuilometragemExcedente, _tabelaFrete.UtilizarComponenteFreteQuilometragemExcedente);

    var menuOpcoes = { tipo: TypeOptionMenu.link, tamanho: 7, opcoes: [{ descricao: "Excluir", id: guid(), metodo: excluirDistanciaClick }] };

    var header = [{ data: "Codigo", visible: false },
    { data: "Tipo", visible: false },
    { data: "QuilometragemInicial", visible: false },
    { data: "DescricaoTipo", title: Localization.Resources.Fretes.TabelaFrete.TipoDeCobranca, width: "25%" },
    { data: "Descricao", title: Localization.Resources.Fretes.TabelaFrete.Descricao, width: "55%" },
    { data: "MultiplicarValorDaFaixaDescri", title: Localization.Resources.Fretes.TabelaFrete.MultiplicaValorFaixa, width: "10%" }];


    _gridDistancia = new BasicDataTable(_distancia.Grid.id, header, menuOpcoes, { column: 2, dir: orderDir.asc });

    recarregarGridDistancia();
}

function recarregarGridDistancia() {

    var data = new Array();

    $.each(_tabelaFrete.Distancias.list, function (i, distancia) {
        var distanciaGrid = new Object();

        distanciaGrid.Codigo = distancia.Codigo.val;
        distanciaGrid.Tipo = distancia.Tipo.val;
        distanciaGrid.QuilometragemInicial = distancia.QuilometragemInicial.val;
        distanciaGrid.MultiplicarValorDaFaixaDescri = "";

        if (distancia.MultiplicarValorDaFaixa.val)
            distanciaGrid.MultiplicarValorDaFaixaDescri = Localization.Resources.Fretes.TabelaFrete.Sim;
        else
            distanciaGrid.MultiplicarValorDaFaixaDescri = Localization.Resources.Fretes.TabelaFrete.Nao;

        distanciaGrid.MultiplicarValorDaFaixa = distancia.MultiplicarValorDaFaixa.val;
        distanciaGrid.DescricaoTipo = distancia.Tipo.val == EnumTipoDistancia.PorFaixaDistanciaPercorrida ? Localization.Resources.Fretes.TabelaFrete.PorFaixaDeKM : Localization.Resources.Fretes.TabelaFrete.PorKMRodados;

        if (distancia.Tipo.val == EnumTipoDistancia.PorFaixaDistanciaPercorrida) {
            var kmInicial = Globalize.parseFloat(distancia.QuilometragemInicial.val.toString());
            var kmFinal = Globalize.parseFloat(distancia.QuilometragemFinal.val.toString());

            if (kmInicial > 0 && kmFinal > 0)
                distanciaGrid.Descricao = Localization.Resources.Fretes.TabelaFrete.DeXAteXKM.format(distancia.QuilometragemInicial.val, distancia.QuilometragemFinal.val);
            else if (kmFinal <= 0)
                distanciaGrid.Descricao = Localization.Resources.Fretes.TabelaFrete.APartirDeXKM.format(distancia.QuilometragemInicial.val);
            else
                distanciaGrid.Descricao = Localization.Resources.Fretes.TabelaFrete.AteXKM.format(distancia.QuilometragemFinal.val);

        } else {
            distanciaGrid.MultiplicarValorDaFaixaDescri = "";
            distanciaGrid.Descricao = Localization.Resources.Fretes.TabelaFrete.ACadaXKM.format(distancia.Quilometros.val);
        }

        data.push(distanciaGrid);
    });

    if (_tabelaFrete.Distancias.list.some(function (item) { return item.Tipo.val == EnumTipoDistancia.ValorFixoPorDistanciaPercorrida || Globalize.parseInt(item.QuilometragemFinal.val.toString()) == 0 })) {
        _distanciaDadosGerais.PermiteValorAdicionalQuilometragemExcedente.enable(false);
        _distanciaDadosGerais.PermiteValorAdicionalQuilometragemExcedente.val(false);
        _distanciaDadosGerais.QuilometragemExcedente.val('');
    } else {
        _distanciaDadosGerais.PermiteValorAdicionalQuilometragemExcedente.enable(true);
    }

    _gridDistancia.CarregarGrid(data);
}

function excluirDistanciaClick(data) {
    for (var i = 0; i < _tabelaFrete.Distancias.list.length; i++) {
        if (data.Codigo == _tabelaFrete.Distancias.list[i].Codigo.val) {
            _tabelaFrete.Distancias.list.splice(i, 1);
            break;
        }
    }

    recarregarGridDistancia();
}

function adicionarDistanciaClick(e, sender) {
    var valido = ValidarCamposObrigatorios(_distancia);

    if (valido) {

        if (_distancia.Tipo.val() == EnumTipoDistancia.PorFaixaDistanciaPercorrida) {

            //if (_distancia.HabilitarValorExcedentePorQuilometroExcedido.val()) {

            //    var quilometragemExcedente = Globalize.parseFloat(_distancia.QuilometragemExcedente.val().toString());

            //    if (quilometragemExcedente <= 0) {
            //        exibirMensagem(tipoMensagem.aviso, "Quilometragem excedente inválida", "A quilometragem excedente deve ser maior que zero.");
            //        return;
            //    }
            //}

            var quilometragemInicial = Globalize.parseFloat(_distancia.QuilometragemInicial.val().toString());
            var quilometragemFinal = Globalize.parseFloat(_distancia.QuilometragemFinal.val().toString());

            if (quilometragemInicial <= 0 && quilometragemFinal <= 0) {
                exibirMensagem(tipoMensagem.aviso, Localization.Resources.Fretes.TabelaFrete.KMInvalido, Localization.Resources.Fretes.TabelaFrete.KMInicialFinalDeveSerMaiorQueZero);
                return;
            } else if (quilometragemFinal > 0 && quilometragemFinal < quilometragemInicial) {
                exibirMensagem(tipoMensagem.aviso, Localization.Resources.Fretes.TabelaFrete.KMInvalido, Localization.Resources.Fretes.TabelaFrete.KMInicialNaoPodeSerMaiorQueOFinal);
                return;
            }

            if (quilometragemFinal == 0)
                quilometragemFinal = 9999999999999999;

            for (var i = 0; i < _tabelaFrete.Distancias.list.length; i++) {
                if (_tabelaFrete.Distancias.list[i].Tipo.val == _distancia.Tipo.val()) {
                    var quilometragemInicialCadastrado = Globalize.parseFloat(_tabelaFrete.Distancias.list[i].QuilometragemInicial.val.toString());
                    var quilometragemFinalCadastrado = Globalize.parseFloat(_tabelaFrete.Distancias.list[i].QuilometragemFinal.val.toString());

                    if (quilometragemFinalCadastrado == 0)
                        quilometragemFinalCadastrado = 9999999999999999;

                    if ((quilometragemInicial >= quilometragemInicialCadastrado && quilometragemInicial <= quilometragemFinalCadastrado) ||
                        (quilometragemFinal >= quilometragemInicialCadastrado && quilometragemFinal <= quilometragemFinalCadastrado) ||
                        (quilometragemInicialCadastrado >= quilometragemInicial && quilometragemInicialCadastrado <= quilometragemFinal) ||
                        (quilometragemFinalCadastrado >= quilometragemInicial && quilometragemFinalCadastrado <= quilometragemFinal)) {
                        exibirMensagem(tipoMensagem.aviso, Localization.Resources.Fretes.TabelaFrete.TipoDeCobrancaJaExistente, Localization.Resources.Fretes.TabelaFrete.OTipoDeCobrancaEntrouEmConflitoComAKMDeAte.format(Globalize.format(quilometragemInicialCadastrado, "n2"), Globalize.format(quilometragemFinalCadastrado, "n2")))
                        return;
                    }
                } else {
                    exibirMensagem(tipoMensagem.aviso, Localization.Resources.Fretes.TabelaFrete.TipoDeCobrancaInvalida, Localization.Resources.Fretes.TabelaFrete.SoEPermitidoAdicionarUmTipoDeCobrancaPorTabelaFrete);
                    return;
                }
            }
        } else if (_distancia.Tipo.val() == EnumTipoDistancia.ValorFixoPorDistanciaPercorrida) {
            for (var i = 0; i < _tabelaFrete.Distancias.list.length; i++) {
                if (_tabelaFrete.Distancias.list[i].Tipo.val == EnumTipoDistancia.ValorFixoPorDistanciaPercorrida) {
                    exibirMensagem(tipoMensagem.aviso, Localization.Resources.Fretes.TabelaFrete.TipoDeCobrancaJaExistente, Localization.Resources.Fretes.TabelaFrete.TipoDeCobrancaPorKMRodadosJaExiste);
                    return;
                } else if (_tabelaFrete.Distancias.list[i].Tipo.val == EnumTipoDistancia.PorFaixaDistanciaPercorrida) {
                    exibirMensagem(tipoMensagem.aviso, Localization.Resources.Fretes.TabelaFrete.TipoDeCobrancaInvalida, Localization.Resources.Fretes.TabelaFrete.SoEPermitidoAdicionarUmTipoDeCobrancaPorTabelaFrete);
                    return;
                }
            }
        }

        _distancia.Codigo.val(guid());

        _tabelaFrete.Distancias.list.push(SalvarListEntity(_distancia));

        recarregarGridDistancia();

        limparCamposDistancia();
    } else {
        exibirMensagem(tipoMensagem.atencao, Localization.Resources.Fretes.TabelaFrete.CamposObrigatorios, Localization.Resources.Fretes.TabelaFrete.InformeOsCamposObrigatorios);
    }
}

function limparCamposDistancia() {
    LimparCampos(_distancia);
}

function ChangeTipoDistancia(novoValor) {
    if (novoValor == EnumTipoDistancia.ValorFixoPorDistanciaPercorrida) {
        _distancia.QuilometragemInicial.visible(false);
        _distancia.MultiplicarValorDaFaixa.visible(false);
        _distancia.QuilometragemFinal.visible(false);
        _distancia.Quilometros.visible(true);
    } else if (novoValor == EnumTipoDistancia.PorFaixaDistanciaPercorrida) {
        _distancia.QuilometragemInicial.visible(true);
        _distancia.QuilometragemInicial.visible(true);
        _distancia.QuilometragemFinal.visible(true);
        if (!_tabelaFrete.MultiplicarValorDaFaixa.val())
            _distancia.MultiplicarValorDaFaixa.visible(true);
        else
            _distancia.MultiplicarValorDaFaixa.visible(false);

        _distancia.Quilometros.visible(false);
    }
}


function PossuiFranquiaAlterado(val) {
    if (val)
        $("#liTabDistancia").hide();
    else
        $("#liTabDistancia").show();
}