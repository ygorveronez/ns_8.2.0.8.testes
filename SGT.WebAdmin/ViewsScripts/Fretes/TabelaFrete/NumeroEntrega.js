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
/// <reference path="../../Enumeradores/EnumTipoNumeroEntrega.js" />
/// <reference path="TabelaFrete.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _gridNumeroEntrega, _numeroEntrega, _numeroEntregaDadosGerais;

var NumeroEntrega = function () {
    this.Grid = PropertyEntity({ type: types.local });
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Tipo = PropertyEntity({ val: ko.observable(1), options: EnumTipoNumeroEntrega.ObterOpcoes(), text: Localization.Resources.Fretes.TabelaFrete.TipoDeCobranca.getRequiredFieldDescription(), def: 1 });
    this.NumeroInicialEntrega = PropertyEntity({ text: Localization.Resources.Fretes.TabelaFrete.NumeroInicial.getFieldDescription(), val: ko.observable(0), def: 0, getType: typesKnockout.int, visible: ko.observable(true), configInt: { precision: 0, allowZero: true } });
    this.NumeroFinalEntrega = PropertyEntity({ text: Localization.Resources.Fretes.TabelaFrete.NumeroFinal.getFieldDescription(), val: ko.observable(0), def: 0, getType: typesKnockout.int, visible: ko.observable(true), configInt: { precision: 0, allowZero: true } });
    this.ComAjudante = PropertyEntity({ text: Localization.Resources.Fretes.TabelaFrete.ComAjudante, val: ko.observable(false), def: false, getType: typesKnockout.bool, visible: ko.observable(true) });

    this.Adicionar = PropertyEntity({ eventClick: adicionarNumeroEntregaClick, type: types.event, text: Localization.Resources.Fretes.TabelaFrete.Adicionar, visible: ko.observable(true) });

    this.Tipo.val.subscribe(function (novoValor) {
        ChangeTipoPacote(novoValor);
    });
}

var NumeroEntregaDadosGerais = function () {
    this.PermiteValorAdicionalEntregaExcedente = PropertyEntity({ text: Localization.Resources.Fretes.TabelaFrete.PermitirValorAdicionalPorEntregaExcedente, issue: 714, val: ko.observable(false), def: false, getType: typesKnockout.bool, visible: ko.observable(true), enable: ko.observable(true) });
    this.CalcularValorEntregaPorPercentualFrete = PropertyEntity({ text: Localization.Resources.Fretes.TabelaFrete.CalcularValorEntregaPorPercentualSobreOValorDoFrete, val: ko.observable(false), def: false, getType: typesKnockout.bool, visible: ko.observable(true), enable: ko.observable(true) });
    this.CalcularValorEntregaPorPercentualFreteComComponentes = PropertyEntity({ text: Localization.Resources.Fretes.TabelaFrete.CalcularValorEntregaPorPercentualFreteComComponentes, val: ko.observable(false), def: false, getType: typesKnockout.bool, visible: ko.observable(true), enable: ko.observable(true) });
    this.CalcularQuantidadeEntregaPorNumeroDePedidos = PropertyEntity({ text: Localization.Resources.Fretes.TabelaFrete.UsarAQuantidadeDePedidosDaCargaComoQuantidadeDeEntrega, val: ko.observable(false), def: false, getType: typesKnockout.bool, visible: ko.observable(_tabelaFrete.TipoCalculo.val() == EnumTipoCalculoTabelaFrete.PorCarga), enable: ko.observable(true) });
    this.ComponenteFreteNumeroEntregas = PropertyEntity({ text: Localization.Resources.Fretes.TabelaFrete.InformarValorComponenteDeFrete, visible: ko.observable(true), type: types.entity, codEntity: ko.observable(0), idBtnSearch: guid() });
    this.UtilizarComponenteFreteNumeroEntregas = PropertyEntity({ val: ko.observable(false), def: false, getType: typesKnockout.bool });

    this.CalcularValorEntregaPorPercentualFrete.val.subscribe(function (novoValor) {
        if (novoValor)
            _numeroEntregaDadosGerais.CalcularValorEntregaPorPercentualFreteComComponentes.val(false);
    });

    this.CalcularValorEntregaPorPercentualFreteComComponentes.val.subscribe(function (novoValor) {
        if (novoValor)
            _numeroEntregaDadosGerais.CalcularValorEntregaPorPercentualFrete.val(false);
    });
}

//*******EVENTOS*******

function loadNumeroEntrega() {
    _numeroEntrega = new NumeroEntrega();
    KoBindings(_numeroEntrega, "knockoutNumeroEntrega");

    _numeroEntregaDadosGerais = new NumeroEntregaDadosGerais();
    KoBindings(_numeroEntregaDadosGerais, "knockoutNumeroEntregaDadosGerais");

    _tabelaFrete.PermiteValorAdicionalEntregaExcedente = _numeroEntregaDadosGerais.PermiteValorAdicionalEntregaExcedente;
    _tabelaFrete.CalcularValorEntregaPorPercentualFrete = _numeroEntregaDadosGerais.CalcularValorEntregaPorPercentualFrete;
    _tabelaFrete.CalcularValorEntregaPorPercentualFreteComComponentes = _numeroEntregaDadosGerais.CalcularValorEntregaPorPercentualFreteComComponentes;
    _tabelaFrete.CalcularQuantidadeEntregaPorNumeroDePedidos = _numeroEntregaDadosGerais.CalcularQuantidadeEntregaPorNumeroDePedidos;
    _tabelaFrete.UtilizarComponenteFreteNumeroEntregas = _numeroEntregaDadosGerais.UtilizarComponenteFreteNumeroEntregas;
    _tabelaFrete.ComponenteFreteNumeroEntregas = _numeroEntregaDadosGerais.ComponenteFreteNumeroEntregas;

    new BuscarComponentesDeFrete(_numeroEntregaDadosGerais.ComponenteFreteNumeroEntregas);
    LimparComponentePorFlag(_tabelaFrete.ComponenteFreteNumeroEntregas, _tabelaFrete.UtilizarComponenteFreteNumeroEntregas);

    var menuOpcoes = { tipo: TypeOptionMenu.link, tamanho: 7, opcoes: [{ descricao: Localization.Resources.Fretes.TabelaFrete.Excluir, id: guid(), metodo: excluirNumeroEntregaClick }] };

    var header = [
        { data: "Codigo", visible: false },
        { data: "Tipo", visible: false },
        { data: "NumeroInicialEntrega", visible: false },
        { data: "DescricaoTipo", title: Localization.Resources.Fretes.TabelaFrete.TipoEntrega, width: "30%" },
        { data: "Descricao", title: Localization.Resources.Fretes.TabelaFrete.Descricao, width: "50%" },
        { data: "ComAjudanteDescri", title: Localization.Resources.Fretes.TabelaFrete.ComAjudante, width: "10%" }
    ];

    _gridNumeroEntrega = new BasicDataTable(_numeroEntrega.Grid.id, header, menuOpcoes, { column: 3, dir: orderDir.asc });

    recarregarGridNumeroEntrega();
}

function recarregarGridNumeroEntrega() {

    var data = new Array();

    $.each(_tabelaFrete.NumeroEntregas.list, function (i, numeroEntrega) {
        var numeroEntregaGrid = new Object();

        numeroEntregaGrid.Codigo = numeroEntrega.Codigo.val;
        numeroEntregaGrid.Tipo = numeroEntrega.Tipo.val;
        numeroEntregaGrid.NumeroInicialEntrega = numeroEntrega.NumeroInicialEntrega.val;
        numeroEntregaGrid.DescricaoTipo = numeroEntrega.Tipo.val == EnumTipoNumeroEntrega.PorFaixaEntrega ? Localization.Resources.Fretes.TabelaFrete.PorFaixaDeEntrega : Localization.Resources.Fretes.TabelaFrete.ValorFixoPorEntrega;
        numeroEntregaGrid.ComAjudanteDescri = "";

        if (numeroEntrega.ComAjudante.val)
            numeroEntregaGrid.ComAjudanteDescri = Localization.Resources.Fretes.TabelaFrete.Sim;
        else
            numeroEntregaGrid.ComAjudanteDescri = Localization.Resources.Fretes.TabelaFrete.Nao;


        numeroEntregaGrid.ComAjudante = numeroEntrega.ComAjudante.val;

        if (numeroEntrega.Tipo.val == EnumTipoNumeroEntrega.PorFaixaEntrega) {
            var numeroInicial = Globalize.parseInt(numeroEntrega.NumeroInicialEntrega.val.toString());
            var numeroFinal = Globalize.parseInt(numeroEntrega.NumeroFinalEntrega.val.toString());

            if (numeroInicial > 0 && numeroFinal > 0)
                numeroEntregaGrid.Descricao = Localization.Resources.Fretes.TabelaFrete.DeXAXEntregas.format(numeroEntrega.NumeroInicialEntrega.val, numeroEntrega.NumeroFinalEntrega.val);
            else if (numeroFinal <= 0)
                numeroEntregaGrid.Descricao = Localization.Resources.Fretes.TabelaFrete.APartirDeXEntregas.format(numeroEntrega.NumeroInicialEntrega.val);
            else
                numeroEntregaGrid.Descricao = Localization.Resources.Fretes.TabelaFrete.AteXEntregas.format(numeroEntrega.NumeroFinalEntrega.val);

        } else {
            numeroEntregaGrid.ComAjudanteDescri = "";
            numeroEntregaGrid.Descricao = Localization.Resources.Fretes.TabelaFrete.ValorFixoPorEntrega;
        }

        data.push(numeroEntregaGrid);
    });

    if (_tabelaFrete.NumeroEntregas.list.some(function (item) { return item.Tipo.val == EnumTipoNumeroEntrega.ValorFixoPorEntrega || Globalize.parseInt(item.NumeroFinalEntrega.val.toString()) == 0; })) {
        _numeroEntregaDadosGerais.PermiteValorAdicionalEntregaExcedente.enable(false);
        _numeroEntregaDadosGerais.PermiteValorAdicionalEntregaExcedente.val(false);
    } else {
        _numeroEntregaDadosGerais.PermiteValorAdicionalEntregaExcedente.enable(true);
    }

    _gridNumeroEntrega.CarregarGrid(data);
}


function excluirNumeroEntregaClick(data) {
    for (var i = 0; i < _tabelaFrete.NumeroEntregas.list.length; i++) {
        if (data.Codigo == _tabelaFrete.NumeroEntregas.list[i].Codigo.val) {
            _tabelaFrete.NumeroEntregas.list.splice(i, 1);
            break;
        }
    }

    recarregarGridNumeroEntrega();
}

function adicionarNumeroEntregaClick(e, sender) {
    var valido = ValidarCamposObrigatorios(_numeroEntrega);

    if (valido) {

        if (_numeroEntrega.Tipo.val() == EnumTipoNumeroEntrega.PorFaixaEntrega) {

            var comAjudante = _numeroEntrega.ComAjudante.val();
            var numeroInicial = Globalize.parseInt(_numeroEntrega.NumeroInicialEntrega.val().toString());
            var numeroFinal = Globalize.parseInt(_numeroEntrega.NumeroFinalEntrega.val().toString());

            if (numeroInicial <= 0 && numeroFinal <= 0) {
                exibirMensagem(tipoMensagem.aviso, Localization.Resources.Fretes.TabelaFrete.NumeroDeEntregaInvalido, Localization.Resources.Fretes.TabelaFrete.OsNumerosInicialEFinalDevemSerMaiorQueZero);
                return;
            } else if (numeroFinal > 0 && numeroFinal < numeroInicial) {
                exibirMensagem(tipoMensagem.aviso, Localization.Resources.Fretes.TabelaFrete.NumeroDeEntregaInvalido, Localization.Resources.Fretes.TabelaFrete.NumeroInicialNaoPodeSerMaiorQueONumeroFinal);
                return;
            }

            if (numeroFinal == 0)
                numeroFinal = 9999999999999999;

            for (var i = 0; i < _tabelaFrete.NumeroEntregas.list.length; i++) {
                if (_tabelaFrete.NumeroEntregas.list[i].Tipo.val == EnumTipoNumeroEntrega.PorFaixaEntrega) {
                    var comAjudanteCadastrado = _tabelaFrete.NumeroEntregas.list[i].ComAjudante.val;
                    var numeroInicialCadastrado = Globalize.parseInt(_tabelaFrete.NumeroEntregas.list[i].NumeroInicialEntrega.val.toString());
                    var numeroFinalCadastrado = Globalize.parseInt(_tabelaFrete.NumeroEntregas.list[i].NumeroFinalEntrega.val.toString());

                    if (numeroFinalCadastrado == 0)
                        numeroFinalCadastrado = 9999999999999999;

                    if ((numeroInicial >= numeroInicialCadastrado && numeroInicial <= numeroFinalCadastrado) ||
                        (numeroFinal >= numeroInicialCadastrado && numeroFinal <= numeroFinalCadastrado) ||
                        (numeroInicialCadastrado >= numeroInicial && numeroInicialCadastrado <= numeroFinal) ||
                        (numeroFinalCadastrado >= numeroInicial && numeroFinalCadastrado <= numeroFinal)) {
                        if (comAjudante == comAjudanteCadastrado) {
                            exibirMensagem(tipoMensagem.aviso, Localization.Resources.Fretes.TabelaFrete.TipoDeEntregaJaExistente, Localization.Resources.Fretes.TabelaFrete.OTipoDeEntregaEntrouEmConflitoComONumeroDeEntrega.format(_tabelaFrete.NumeroEntregas.list[i].NumeroInicialEntrega.val, _tabelaFrete.NumeroEntregas.list[i].NumeroFinalEntrega.val))
                            return;
                        }
                    }
                } else {
                    exibirMensagem(tipoMensagem.aviso, Localization.Resources.Fretes.TabelaFrete.TipoDeCobrancaInvalida, Localization.Resources.Fretes.TabelaFrete.SoEPermitidoAdicionarUmTipoDeCobrancaPorTabelaFrete);
                    return;
                }
            }
        } else if (_numeroEntrega.Tipo.val() == EnumTipoNumeroEntrega.ValorFixoPorEntrega) {
            for (var i = 0; i < _tabelaFrete.NumeroEntregas.list.length; i++) {
                if (_tabelaFrete.NumeroEntregas.list[i].Tipo.val == EnumTipoNumeroEntrega.ValorFixoPorEntrega) {
                    exibirMensagem(tipoMensagem.aviso, Localization.Resources.Fretes.TabelaFrete.TipoDeCobrancaJaExistente, Localization.Resources.Fretes.TabelaFrete.OTipoDeCobrancaDeValorFixoPorEntregaJaExiste);
                    return;
                } else if (_tabelaFrete.NumeroEntregas.list[i].Tipo.val == EnumTipoNumeroEntrega.PorFaixaEntrega) {
                    exibirMensagem(tipoMensagem.aviso, Localization.Resources.Fretes.TabelaFrete.TipoDeCobrancaInvalida, Localization.Resources.Fretes.TabelaFrete.SoEPermitidoAdicionarUmTipoDeCobrancaPorTabelaFrete);
                    return;
                }
            }
        }

        _numeroEntrega.Codigo.val(guid());

        _tabelaFrete.NumeroEntregas.list.push(SalvarListEntity(_numeroEntrega));

        recarregarGridNumeroEntrega();

        limparCamposNumeroEntrega();
    } else {
        exibirMensagem(tipoMensagem.atencao, Localization.Resources.Fretes.TabelaFrete.CamposObrigatorios, Localization.Resources.Fretes.TabelaFrete.InformeOsCamposObrigatorios);
    }
}

function limparCamposNumeroEntrega() {
    LimparCampos(_numeroEntrega);
}

function ChangeTipoPacote(novoValor) {
    if (novoValor == EnumTipoNumeroEntrega.ValorFixoPorEntrega) {
        _numeroEntrega.NumeroInicialEntrega.visible(false);
        _numeroEntrega.NumeroFinalEntrega.visible(false);
        _numeroEntrega.ComAjudante.visible(false);
    } else if (novoValor == EnumTipoNumeroEntrega.PorFaixaEntrega) {
        _numeroEntrega.NumeroInicialEntrega.visible(true);
        _numeroEntrega.NumeroFinalEntrega.visible(true);
        _numeroEntrega.ComAjudante.visible(true);
    }
}