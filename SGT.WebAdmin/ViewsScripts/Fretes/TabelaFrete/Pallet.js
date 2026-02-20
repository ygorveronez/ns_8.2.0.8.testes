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
/// <reference path="../../Enumeradores/EnumTipoNumeroPalletsTabelaFrete.js" />
/// <reference path="TabelaFrete.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _gridPallet, _pallet, _palletDadosGerais;

var Pallet = function () {
    this.Grid = PropertyEntity({ type: types.local });
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Tipo = PropertyEntity({ val: ko.observable(1), options: EnumTipoNumeroPalletsTabelaFrete.ObterOpcoes(), text: Localization.Resources.Fretes.TabelaFrete.TipoDeCobranca.getRequiredFieldDescription(), def: 1 });
    this.NumeroInicialPallet = PropertyEntity({ text: Localization.Resources.Fretes.TabelaFrete.NumeroInicial.getFieldDescription(), val: ko.observable(0), def: 0, getType: typesKnockout.int, visible: ko.observable(true), configInt: { precision: 0, allowZero: true } });
    this.NumeroFinalPallet = PropertyEntity({ text: Localization.Resources.Fretes.TabelaFrete.NumeroFinal.getFieldDescription(), val: ko.observable(0), def: 0, getType: typesKnockout.int, visible: ko.observable(true), configInt: { precision: 0, allowZero: true } });
    
    this.Adicionar = PropertyEntity({ eventClick: adicionarPalletClick, type: types.event, text: Localization.Resources.Fretes.TabelaFrete.Adicionar, visible: ko.observable(true) });

    this.Tipo.val.subscribe(function (novoValor) {
        ChangeTipoPallet(novoValor);
    });
}

var PalletDadosGerais = function () {
    this.PermiteValorAdicionalPalletExcedente = PropertyEntity({ text: Localization.Resources.Fretes.TabelaFrete.PermitirValorAdicionalPorPalletExcedente, issue: 714, val: ko.observable(false), def: false, getType: typesKnockout.bool, visible: ko.observable(true), enable: ko.observable(true) });
    this.TabelaUsoExclusivaParaPalletsContidosNasFaixasCadastradas = PropertyEntity({ text: Localization.Resources.Fretes.TabelaFrete.TabelaUsoExclusivaParaPalletsContidosNasFaixasCadastradas, val: ko.observable(false), def: false, getType: typesKnockout.bool, visible: ko.observable(true), enable: ko.observable(true) });

    this.ComponenteFretePallet = PropertyEntity({ text: Localization.Resources.Fretes.TabelaFrete.InformarValorComponenteDeFrete, visible: ko.observable(true), type: types.entity, codEntity: ko.observable(0), idBtnSearch: guid() });
    this.UtilizarComponenteFretePallet = PropertyEntity({ val: ko.observable(false), def: false, getType: typesKnockout.bool });
}

//*******EVENTOS*******

function loadPallet() {

    _pallet = new Pallet();
    KoBindings(_pallet, "knockoutPallet");

    _palletDadosGerais = new PalletDadosGerais();
    KoBindings(_palletDadosGerais, "knockoutPalletDadosGerais");


    _tabelaFrete.PermiteValorAdicionalPalletExcedente = _palletDadosGerais.PermiteValorAdicionalPalletExcedente;
    _tabelaFrete.TabelaUsoExclusivaParaPalletsContidosNasFaixasCadastradas = _palletDadosGerais.TabelaUsoExclusivaParaPalletsContidosNasFaixasCadastradas;
    _tabelaFrete.ComponenteFretePallet = _palletDadosGerais.ComponenteFretePallet;
    _tabelaFrete.UtilizarComponenteFretePallet = _palletDadosGerais.UtilizarComponenteFretePallet;

    new BuscarComponentesDeFrete(_palletDadosGerais.ComponenteFretePallet);
    LimparComponentePorFlag(_tabelaFrete.ComponenteFretePallet, _tabelaFrete.UtilizarComponenteFretePallet);

    var menuOpcoes = { tipo: TypeOptionMenu.link, tamanho: 7, opcoes: [{ descricao: Localization.Resources.Fretes.TabelaFrete.Excluir, id: guid(), metodo: excluirPalletClick }] };

    var header = [{ data: "Codigo", visible: false },
                { data: "Tipo", visible: false },
                { data: "NumeroInicialPallet", visible: false },
                { data: "DescricaoTipo", title: Localization.Resources.Fretes.TabelaFrete.TipoDeEntrega, width: "30%" },
                { data: "Descricao", title: Localization.Resources.Fretes.TabelaFrete.Descricao, width: "50%" }];

    _gridPallet = new BasicDataTable(_pallet.Grid.id, header, menuOpcoes, { column: 1, dir: orderDir.asc });

    recarregarGridPallet();
}

function recarregarGridPallet() {

    var data = new Array();

    $.each(_tabelaFrete.Pallets.list, function (i, pallet) {
        var palletGrid = new Object();

        palletGrid.Codigo = pallet.Codigo.val;
        palletGrid.Tipo = pallet.Tipo.val;
        palletGrid.NumeroInicialPallet = pallet.NumeroInicialPallet.val;
        palletGrid.DescricaoTipo = pallet.Tipo.val == EnumTipoNumeroPalletsTabelaFrete.PorFaixaPallets ? Localization.Resources.Fretes.TabelaFrete.PorFaixaDePallets : Localization.Resources.Fretes.TabelaFrete.ValorFixoPorPallet;

        if (pallet.Tipo.val == EnumTipoNumeroPalletsTabelaFrete.PorFaixaPallets) {
            var numeroInicial = Globalize.parseInt(pallet.NumeroInicialPallet.val.toString());
            var numeroFinal = Globalize.parseInt(pallet.NumeroFinalPallet.val.toString());

            if (numeroInicial > 0 && numeroFinal > 0)
                palletGrid.Descricao = Localization.Resources.Fretes.TabelaFrete.DeXAXPallets.format(pallet.NumeroInicialPallet.val, pallet.NumeroFinalPallet.val);
            else if (numeroFinal <= 0)
                palletGrid.Descricao = Localization.Resources.Fretes.TabelaFrete.APartirDeXPallets.format(pallet.NumeroInicialPallet.val);
            else
                palletGrid.Descricao = Localization.Resources.Fretes.TabelaFrete.AteXPallets.format(pallet.NumeroFinalPallet.val);

        } else {
            palletGrid.Descricao = Localization.Resources.Fretes.TabelaFrete.ValorFixoPorPallet;
        }

        data.push(palletGrid);
    });

    if (_tabelaFrete.Pallets.list.some(function (item) { return item.Tipo.val == EnumTipoNumeroPalletsTabelaFrete.ValorFixoPorPallet || Globalize.parseInt(item.NumeroFinalPallet.val.toString()) == 0; })) {
        _palletDadosGerais.PermiteValorAdicionalPalletExcedente.enable(false);
        _palletDadosGerais.PermiteValorAdicionalPalletExcedente.val(false);
        _palletDadosGerais.TabelaUsoExclusivaParaPalletsContidosNasFaixasCadastradas.enable(false);
        _palletDadosGerais.TabelaUsoExclusivaParaPalletsContidosNasFaixasCadastradas.val(false);
    } else {
        _palletDadosGerais.PermiteValorAdicionalPalletExcedente.enable(true);
        _palletDadosGerais.TabelaUsoExclusivaParaPalletsContidosNasFaixasCadastradas.enable(true);
    }

    _gridPallet.CarregarGrid(data);
}


function excluirPalletClick(data) {
    for (var i = 0; i < _tabelaFrete.Pallets.list.length; i++) {
        if (data.Codigo == _tabelaFrete.Pallets.list[i].Codigo.val) {
            _tabelaFrete.Pallets.list.splice(i, 1);
            break;
        }
    }

    recarregarGridPallet();
}

function adicionarPalletClick(e, sender) {
    var valido = ValidarCamposObrigatorios(_pallet);

    if (valido) {

        if (_pallet.Tipo.val() == EnumTipoNumeroPalletsTabelaFrete.PorFaixaPallets) {

            var numeroInicial = Globalize.parseInt(_pallet.NumeroInicialPallet.val().toString());
            var numeroFinal = Globalize.parseInt(_pallet.NumeroFinalPallet.val().toString());

            if (numeroInicial <= 0 && numeroFinal <= 0) {
                exibirMensagem(tipoMensagem.aviso, Localization.Resources.Fretes.TabelaFrete.NumeroPalletInvalido, Localization.Resources.Fretes.TabelaFrete.OsNumerosInicialEFinalDevemSerMaiorQueZero);
                return;
            } else if (numeroFinal > 0 && numeroFinal < numeroInicial) {
                exibirMensagem(tipoMensagem.aviso, Localization.Resources.Fretes.TabelaFrete.NumeroPalletInvalido, Localization.Resources.Fretes.TabelaFrete.NumeroInicialNaoPodeSerMaiorQueONumeroFinal);
                return;
            }

            if (numeroFinal == 0)
                numeroFinal = 9999999999999999;

            for (var i = 0; i < _tabelaFrete.Pallets.list.length; i++) {
                if (_tabelaFrete.Pallets.list[i].Tipo.val == EnumTipoNumeroPalletsTabelaFrete.PorFaixaPallets) {
                    var numeroInicialCadastrado = Globalize.parseInt(_tabelaFrete.Pallets.list[i].NumeroInicialPallet.val.toString());
                    var numeroFinalCadastrado = Globalize.parseInt(_tabelaFrete.Pallets.list[i].NumeroFinalPallet.val.toString());

                    if (numeroFinalCadastrado == 0)
                        numeroFinalCadastrado = 9999999999999999;

                    if ((numeroInicial >= numeroInicialCadastrado && numeroInicial <= numeroFinalCadastrado) ||
                        (numeroFinal >= numeroInicialCadastrado && numeroFinal <= numeroFinalCadastrado) ||
                        (numeroInicialCadastrado >= numeroInicial && numeroInicialCadastrado <= numeroFinal) ||
                        (numeroFinalCadastrado >= numeroInicial && numeroFinalCadastrado <= numeroFinal)) {
                        exibirMensagem(tipoMensagem.aviso, Localization.Resources.Fretes.TabelaFrete.TipoDeEntregaJaExistente, Localization.Resources.Fretes.TabelaFrete.OTipoDeCobrancaEntrouEmConflitoComONumeroDePallet.format(_tabelaFrete.Pallets.list[i].NumeroInicialPallet.val, _tabelaFrete.Pallets.list[i].NumeroFinalPallet.val))
                        return;
                    }
                } else {
                    exibirMensagem(tipoMensagem.aviso, Localization.Resources.Fretes.TabelaFrete.TipoDeCobrancaInvalida, Localization.Resources.Fretes.TabelaFrete.SoEPermitidoAdicionarUmTipoDeCobrancaPorTabelaFrete);
                    return;
                }
            }
        } else if (_pallet.Tipo.val() == EnumTipoNumeroPalletsTabelaFrete.ValorFixoPorPallet) {
            for (var i = 0; i < _tabelaFrete.Pallets.list.length; i++) {
                if (_tabelaFrete.Pallets.list[i].Tipo.val == EnumTipoNumeroPalletsTabelaFrete.ValorFixoPorPallet) {
                    exibirMensagem(tipoMensagem.aviso, Localization.Resources.Fretes.TabelaFrete.TipoDeCobrancaJaExistente, Localization.Resources.Fretes.TabelaFrete.OTipoDeCobrancaDeValorFixoPorPalletJaExiste);
                    return;
                } else if (_tabelaFrete.Pallets.list[i].Tipo.val == EnumTipoNumeroPalletsTabelaFrete.PorFaixaPallets) {
                    exibirMensagem(tipoMensagem.aviso, Localization.Resources.Fretes.TabelaFrete.TipoDeCobrancaInvalida, Localization.Resources.Fretes.TabelaFrete.SoEPermitidoAdicionarUmTipoDeCobrancaPorTabelaFrete);
                    return;
                }
            }
        }

        _pallet.Codigo.val(guid());

        _tabelaFrete.Pallets.list.push(SalvarListEntity(_pallet));

        recarregarGridPallet();

        limparCamposPallet();
    } else {
        exibirMensagem(tipoMensagem.atencao, Localization.Resources.Fretes.TabelaFrete.CamposObrigatorios, Localization.Resources.Fretes.TabelaFrete.InformeOsCamposObrigatorios);
    }
}

function limparCamposPallet() {
    LimparCampos(_pallet);
}

function ChangeTipoPallet(novoValor) {
    if (novoValor == EnumTipoNumeroPalletsTabelaFrete.ValorFixoPorPallet) {
        _pallet.NumeroInicialPallet.visible(false);
        _pallet.NumeroFinalPallet.visible(false);
    } else if (novoValor == EnumTipoNumeroPalletsTabelaFrete.PorFaixaPallets) {
        _pallet.NumeroInicialPallet.visible(true);
        _pallet.NumeroFinalPallet.visible(true);
    }
}