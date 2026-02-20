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
/// <reference path="../../Enumeradores/EnumTipoPesoTransportado.js" />
/// <reference path="../../Enumeradores/EnumValorPesoTransportado.js" />
/// <reference path="../../Consultas/UnidadeMedida.js" />
/// <reference path="../../Consultas/ComponenteFrete.js" />
/// <reference path="TabelaFrete.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _gridPesoTransportado, _pesoTransportado, _pesoTransportadoDadosGerais;

var PesoTransportado = function () {
    this.Grid = PropertyEntity({ type: types.local });
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.UnidadeMedida = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), def: "", defCodEntity: 0, text: Localization.Resources.Fretes.TabelaFrete.UnidadeDeMedida.getRequiredFieldDescription(), idBtnSearch: guid(), required: true, issue: 88 });
    this.ModeloVeicularCarga = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), visible: ko.observable(true), def: "", defCodEntity: 0, text: Localization.Resources.Fretes.TabelaFrete.ModeloVeicular.getFieldDescription(), idBtnSearch: guid(), required: false });
    this.Tipo = PropertyEntity({ val: ko.observable(EnumTipoPesoTransportado.PorFaixaPesoTransportado), options: EnumTipoPesoTransportado.ObterOpcoes(), text: Localization.Resources.Fretes.TabelaFrete.TipoDeCobranca.getRequiredFieldDescription(), def: EnumTipoPesoTransportado.PorFaixaPesoTransportado });
    this.CalculoPeso = PropertyEntity({ val: ko.observable(EnumValorPesoTransportado.ValorFixo), options: EnumValorPesoTransportado.ObterOpcoes(), text: Localization.Resources.Fretes.TabelaFrete.TipoDeCalculo.getRequiredFieldDescription(), def: EnumValorPesoTransportado.ValorFixo });
    this.PesoInicial = PropertyEntity({ type: types.map, text: Localization.Resources.Fretes.TabelaFrete.PesoInicial.getFieldDescription(), val: ko.observable(Globalize.format(0, "n3")), def: Globalize.format(0, "n3"), getType: typesKnockout.decimal, visible: ko.observable(true), maxlength: 13, configDecimal: { precision: 3, allowZero: true } });
    this.PesoFinal = PropertyEntity({ type: types.map, text: Localization.Resources.Fretes.TabelaFrete.PesoFinal.getFieldDescription(), val: ko.observable(Globalize.format(0, "n3")), def: Globalize.format(0, "n3"), getType: typesKnockout.decimal, visible: ko.observable(true), maxlength: 13, configDecimal: { precision: 3, allowZero: true } });
    this.Peso = PropertyEntity({ type: types.map, text: Localization.Resources.Fretes.TabelaFrete.Peso.getRequiredFieldDescription(), val: ko.observable(Globalize.format(0, "n3")), def: Globalize.format(0, "n3"), getType: typesKnockout.decimal, visible: ko.observable(false), maxlength: 13, configDecimal: { precision: 3, allowZero: true } });
    this.ComAjudante = PropertyEntity({ text: Localization.Resources.Fretes.TabelaFrete.ComAjudante, val: ko.observable(false), def: false, getType: typesKnockout.bool, visible: ko.observable(true) });
    this.ParaCalcularValorBase = PropertyEntity({ text: Localization.Resources.Fretes.TabelaFrete.ParaCalcularOValorBaseParaDiferencaDoFretePago, val: ko.observable(false), def: false, getType: typesKnockout.bool, visible: ko.observable(visibilidadeParaValorFreteBase()) });
    this.ComponenteFrete = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Fretes.TabelaFrete.ComponenteFrete.getFieldDescription(), idBtnSearch: guid() });

    this.CodigoComponenteFrete = PropertyEntity({ val: ko.observable(0) });
    this.DescricaoComponente = PropertyEntity({ val: ko.observable('') });

    this.Adicionar = PropertyEntity({ eventClick: adicionarPesoTransportadoClick, type: types.event, text: Localization.Resources.Fretes.TabelaFrete.Adicionar, visible: ko.observable(true) });

    this.Tipo.val.subscribe(function (novoValor) {
        ChangeTipoPesoTransportado(novoValor);
    });
};

var PesoTransportadoDadosGerais = function () {
    this.PermiteValorAdicionalPesoExcedente = PropertyEntity({ text: Localization.Resources.Fretes.TabelaFrete.PermitirValorAdicionalPorPesoExcedente, val: ko.observable(false), def: false, getType: typesKnockout.bool, visible: ko.observable(true), enable: ko.observable(true) });
    this.PesoExcedente = PropertyEntity({ type: types.map, text: Localization.Resources.Fretes.TabelaFrete.PesoExcedente.getRequiredFieldDescription(), issue: 700, val: ko.observable(""), def: "", getType: typesKnockout.decimal, visible: ko.observable(false), maxlength: 13, configDecimal: { precision: 3, allowZero: true } });

    this.ComponenteFretePeso = PropertyEntity({ text: Localization.Resources.Fretes.TabelaFrete.InformarValorComponenteDeFrete.getFieldDescription(), visible: ko.observable(true), type: types.entity, codEntity: ko.observable(0), idBtnSearch: guid() });
    this.UtilizarComponenteFretePeso = PropertyEntity({ val: ko.observable(false), def: false, getType: typesKnockout.bool });

    this.ObrigatorioValorFretePeso = PropertyEntity({ val: ko.observable(false), def: false, getType: typesKnockout.bool, visible: ko.observable(true), text: Localization.Resources.Fretes.TabelaFrete.ObrigatorioQueExistaValorDeFretePorPeso });

    this.PesoParametroCalculoFrete = PropertyEntity({ val: ko.observable(EnumPesoParametroCalculoFrete.PorCarga), options: EnumPesoParametroCalculoFrete.ObterOpcoes(), text: Localization.Resources.Fretes.TabelaFrete.PesoParaCalculo.getRequiredFieldDescription(), def: EnumPesoParametroCalculoFrete.PorCarga });
    this.CalcularFatorPesoPelaKM = PropertyEntity({ text: Localization.Resources.Fretes.TabelaFrete.MutiplicarOValorDoPesoPorUnidadePelaDistaciaQ, val: ko.observable(false), def: false, required: false, getType: typesKnockout.bool, visible: ko.observable(true) });
    this.UtilizarPesoLiquido = PropertyEntity({ text: Localization.Resources.Fretes.TabelaFrete.UtilizarPesoLiquido, val: ko.observable(false), def: false, getType: typesKnockout.bool, visible: ko.observable(true) });

    this.CalcularFretePorPesoCubado = PropertyEntity({ text: Localization.Resources.Fretes.TabelaFrete.CalcularFretePorPesoCubado, val: ko.observable(false), def: false, getType: typesKnockout.bool, visible: ko.observable(_CONFIGURACAO_TMS.TipoServicoMultisoftware != EnumTipoServicoMultisoftware.MultiTMS) });
    this.AplicarMaiorValorEntrePesoEPesoCubado = PropertyEntity({ text: Localization.Resources.Fretes.TabelaFrete.AplicarMaiorValorEntrePesoECubado, val: ko.observable(false), def: false, getType: typesKnockout.bool, visible: ko.observable(false) });

    this.FatorCubagem = PropertyEntity({ type: types.map, text: Localization.Resources.Fretes.TabelaFrete.FatorDaCubagem.getFieldDescription(), val: ko.observable(Globalize.format(0, "n3")), def: Globalize.format(0, "n3"), getType: typesKnockout.decimal, visible: ko.observable(false), maxlength: 13, configDecimal: { precision: 3, allowZero: true } });
    this.IsencaoCubagem = PropertyEntity({ type: types.map, text: Localization.Resources.Fretes.TabelaFrete.IsencaoDaCubagem.getFieldDescription(), val: ko.observable(Globalize.format(0, "n3")), def: Globalize.format(0, "n3"), getType: typesKnockout.decimal, visible: ko.observable(false), maxlength: 13, configDecimal: { precision: 3, allowZero: true } });
    this.DescontoCubagemCalculoFrete = PropertyEntity({ text: Localization.Resources.Fretes.TabelaFrete.DescontoCubagemCalculoFrete.getFieldDescription(), required: false, getType: typesKnockout.decimal, maxlength: 6, val: ko.observable(Globalize.format(0, "n2")), def: Globalize.format(0, "n2"), visible: ko.observable(true) });

    this.CalcularFretePorPesoCubado.val.subscribe(function (novoValor) {
        ChangeCalcularFretePorPesoCubado(novoValor);
    });
};

//*******EVENTOS*******

function loadPesoTransportado() {

    _pesoTransportado = new PesoTransportado();
    KoBindings(_pesoTransportado, "knockoutPeso");

    _pesoTransportadoDadosGerais = new PesoTransportadoDadosGerais();
    KoBindings(_pesoTransportadoDadosGerais, "knockoutPesoDadosGerais");

    _tabelaFrete.PermiteValorAdicionalPesoExcedente = _pesoTransportadoDadosGerais.PermiteValorAdicionalPesoExcedente;
    _tabelaFrete.PesoExcedente = _pesoTransportadoDadosGerais.PesoExcedente;
    _tabelaFrete.ComponenteFretePeso = _pesoTransportadoDadosGerais.ComponenteFretePeso;
    _tabelaFrete.UtilizarComponenteFretePeso = _pesoTransportadoDadosGerais.UtilizarComponenteFretePeso;
    _tabelaFrete.ObrigatorioValorFretePeso = _pesoTransportadoDadosGerais.ObrigatorioValorFretePeso;
    _tabelaFrete.PesoParametroCalculoFrete = _pesoTransportadoDadosGerais.PesoParametroCalculoFrete;
    _tabelaFrete.CalcularFatorPesoPelaKM = _pesoTransportadoDadosGerais.CalcularFatorPesoPelaKM;
    _tabelaFrete.UtilizarPesoLiquido = _pesoTransportadoDadosGerais.UtilizarPesoLiquido;

    _tabelaFrete.CalcularFretePorPesoCubado = _pesoTransportadoDadosGerais.CalcularFretePorPesoCubado;
    _tabelaFrete.AplicarMaiorValorEntrePesoEPesoCubado = _pesoTransportadoDadosGerais.AplicarMaiorValorEntrePesoEPesoCubado;
    _tabelaFrete.FatorCubagem = _pesoTransportadoDadosGerais.FatorCubagem;
    _tabelaFrete.IsencaoCubagem = _pesoTransportadoDadosGerais.IsencaoCubagem;
    _tabelaFrete.DescontoCubagemCalculoFrete = _pesoTransportadoDadosGerais.DescontoCubagemCalculoFrete;

    new BuscarUnidadesMedida(_pesoTransportado.UnidadeMedida);
    new BuscarModelosVeicularesCarga(_pesoTransportado.ModeloVeicularCarga);
    new BuscarComponentesDeFrete(_pesoTransportadoDadosGerais.ComponenteFretePeso);
    new BuscarComponentesDeFrete(_pesoTransportado.ComponenteFrete);

    LimparComponentePorFlag(_tabelaFrete.ComponenteFretePeso, _tabelaFrete.UtilizarComponenteFretePeso);
    CarregarGridDadosTransporte();

}

function CarregarGridDadosTransporte() {

    var menuOpcoes = { tipo: TypeOptionMenu.link, tamanho: 7, opcoes: [{ descricao: "Excluir", id: guid(), metodo: excluirPesoTransportadoClick }] };

    var header = [
        { data: "Codigo", visible: false },
        { data: "Tipo", visible: false },
        { data: "PesoInicial", visible: false },
        { data: "CalculoPeso", visible: false },
        { data: "CodigoComponenteFrete", visible: false },
        { data: "DescricaoTipo", title: Localization.Resources.Fretes.TabelaFrete.TipoDeCobranca, width: "20%" },
        { data: "Descricao", title: Localization.Resources.Fretes.TabelaFrete.Descricao, width: "25%" },
        { data: "DescricaoCalculoPeso", title: Localization.Resources.Fretes.TabelaFrete.TipoDeCalculo, width: "20%" },
        { data: "UnidadeMedida", title: Localization.Resources.Fretes.TabelaFrete.UnidadeDeMedida, width: "20%" },
        { data: "ModeloVeicularCarga", title: Localization.Resources.Fretes.TabelaFrete.ModeloVeicular, width: "20%" },
        { data: "ComAjudanteDescri", title: Localization.Resources.Fretes.TabelaFrete.ComAjudante, width: "10%" },
        { data: "ParaCalcularValorBaseDescri", title: Localization.Resources.Fretes.TabelaFrete.ParaValorBase, width: "10%", visible: visibilidadeParaValorFreteBase() },
        { data: "DescricaoComponente", title: Localization.Resources.Fretes.TabelaFrete.ComponenteFrete, width: "10%" }
    ];

    _gridPesoTransportado = new BasicDataTable(_pesoTransportado.Grid.id, header, menuOpcoes, { column: 2, dir: orderDir.asc });

    recarregarGridPesoTransportado();
}

function recarregarGridPesoTransportado() {

    var data = new Array();

    $.each(_tabelaFrete.PesosTransportados.list, function (i, pesoTransportado) {
        var pesoTransportadoGrid = new Object();
        pesoTransportadoGrid.Codigo = pesoTransportado.Codigo.val;
        pesoTransportadoGrid.Tipo = pesoTransportado.Tipo.val;
        pesoTransportadoGrid.CalculoPeso = pesoTransportado.CalculoPeso.val;
        pesoTransportadoGrid.PesoInicial = pesoTransportado.PesoInicial.val;
        pesoTransportadoGrid.DescricaoTipo = pesoTransportado.Tipo.val == EnumTipoPesoTransportado.PorFaixaPesoTransportado ? Localization.Resources.Fretes.TabelaFrete.PorFaixaDePeso : Localization.Resources.Fretes.TabelaFrete.PorUnidadeDeMedida;
        pesoTransportadoGrid.DescricaoCalculoPeso = pesoTransportado.CalculoPeso.val == EnumValorPesoTransportado.ValorFixo ? Localization.Resources.Fretes.TabelaFrete.PorValorFixo : Localization.Resources.Fretes.TabelaFrete.PorMultiplicacao;
        pesoTransportadoGrid.ComAjudanteDescri = "";
        pesoTransportadoGrid.ParaCalcularValorBaseDescri = "";
        pesoTransportadoGrid.CodigoComponenteFrete = pesoTransportado.CodigoComponenteFrete != null ? pesoTransportado.CodigoComponenteFrete?.val : 0;
        pesoTransportadoGrid.DescricaoComponente = pesoTransportado.DescricaoComponente != null ? pesoTransportado.DescricaoComponente.val : "";

        if (pesoTransportado.ComAjudante.val)
            pesoTransportadoGrid.ComAjudanteDescri = Localization.Resources.Fretes.TabelaFrete.Sim;
        else
            pesoTransportadoGrid.ComAjudanteDescri = Localization.Resources.Fretes.TabelaFrete.Nao;

        pesoTransportadoGrid.ComAjudante = pesoTransportado.ComAjudante.val;

        if (pesoTransportado.ParaCalcularValorBase.val)
            pesoTransportadoGrid.ParaCalcularValorBaseDescri = Localization.Resources.Fretes.TabelaFrete.Sim;
        else
            pesoTransportadoGrid.ParaCalcularValorBaseDescri = Localization.Resources.Fretes.TabelaFrete.Nao;

        pesoTransportadoGrid.ParaCalcularValorBase = pesoTransportado.ParaCalcularValorBase.val;

        if (pesoTransportado.Tipo.val == EnumTipoPesoTransportado.PorFaixaPesoTransportado) {
            var pesoInicial = Globalize.parseFloat(pesoTransportado.PesoInicial.val.toString());
            var pesoFinal = Globalize.parseFloat(pesoTransportado.PesoFinal.val.toString());

            if (pesoInicial > 0 && pesoFinal > 0)
                pesoTransportadoGrid.Descricao = Localization.Resources.Fretes.TabelaFrete.DeXAteX.format(pesoTransportado.PesoInicial.val, pesoTransportado.PesoFinal.val);
            else if (pesoFinal <= 0)
                pesoTransportadoGrid.Descricao = Localization.Resources.Fretes.TabelaFrete.APartirDeX.format(pesoTransportado.PesoInicial.val);
            else
                pesoTransportadoGrid.Descricao = Localization.Resources.Fretes.TabelaFrete.AteX.format(pesoTransportado.PesoFinal.val);

        } else {
            pesoTransportadoGrid.ComAjudanteDescri = "";
            pesoTransportadoGrid.ParaCalcularValorBaseDescri = "";
            pesoTransportadoGrid.Descricao = Localization.Resources.Fretes.TabelaFrete.ACadaX.format(pesoTransportado.Peso.val);
        }

        pesoTransportadoGrid.UnidadeMedida = pesoTransportado.UnidadeMedida.val;
        pesoTransportadoGrid.ModeloVeicularCarga = pesoTransportado.ModeloVeicularCarga.val;

        data.push(pesoTransportadoGrid);
    });

    if (_tabelaFrete.PesosTransportados.list.some(function (item) { return item.Tipo.val == EnumTipoPesoTransportado.ValorFixoPorPesoTransportado || Globalize.parseFloat(item.PesoFinal.val.toString()) == 0 })) {
        _pesoTransportadoDadosGerais.PermiteValorAdicionalPesoExcedente.enable(false);
        _pesoTransportadoDadosGerais.PermiteValorAdicionalPesoExcedente.val(false);
        _pesoTransportadoDadosGerais.PesoExcedente.val('');
    } else {
        _pesoTransportadoDadosGerais.PermiteValorAdicionalPesoExcedente.enable(true);
    }

    if (_tabelaFrete.PesosTransportados.list.some((item) => { return item.UnidadeMedida.val === "Metro Cúbico" }))
        _pesoTransportadoDadosGerais.DescontoCubagemCalculoFrete.visible(true);
    else 
        _pesoTransportadoDadosGerais.DescontoCubagemCalculoFrete.visible(false);

    _gridPesoTransportado.CarregarGrid(data);
}


function excluirPesoTransportadoClick(data) {
    for (var i = 0; i < _tabelaFrete.PesosTransportados.list.length; i++) {
        if (data.Codigo == _tabelaFrete.PesosTransportados.list[i].Codigo.val) {
            _tabelaFrete.PesosTransportados.list.splice(i, 1);
            break;
        }
    }

    recarregarGridPesoTransportado();
}

function adicionarPesoTransportadoClick(e, sender) {
    var valido = ValidarCamposObrigatorios(_pesoTransportado);

    if (valido) {

        if (_pesoTransportado.Tipo.val() == EnumTipoPesoTransportado.PorFaixaPesoTransportado) {
            var comAjudante = _pesoTransportado.ComAjudante.val();
            var paraCalcularValorBase = _pesoTransportado.ParaCalcularValorBase.val();
            var pesoInicial = Globalize.parseFloat(_pesoTransportado.PesoInicial.val().toString());
            var pesoFinal = Globalize.parseFloat(_pesoTransportado.PesoFinal.val().toString());
            var modeloVeicularCarga = _pesoTransportado.ModeloVeicularCarga.val();

            if (pesoInicial <= 0 && pesoFinal <= 0) {
                exibirMensagem(tipoMensagem.aviso, Localization.Resources.Fretes.TabelaFrete.PesoTransportadoInvalido, Localization.Resources.Fretes.TabelaFrete.PesosInicialFinalDevemSerMaiorQueZero);
                return;
            } else if (pesoFinal > 0 && pesoFinal < pesoInicial) {
                exibirMensagem(tipoMensagem.aviso, Localization.Resources.Fretes.TabelaFrete.PesoTransportadoInvalido, Localization.Resources.Fretes.TabelaFrete.PesoInicialNaoPodeSerMaiorQueFinal);
                return;
            }

            if (pesoFinal == 0)
                pesoFinal = 9999999999999999;

            for (var i = 0; i < _tabelaFrete.PesosTransportados.list.length; i++) {
                //if (_tabelaFrete.PesosTransportados.list[i].Tipo.val == _pesoTransportado.Tipo.val()) {
                var comAjudanteCadastrado = _tabelaFrete.PesosTransportados.list[i].ComAjudante.val;
                var paraCalcularValorBaseCadastrado = _tabelaFrete.PesosTransportados.list[i].ParaCalcularValorBase.val;
                var pesoInicialCadastrado = Globalize.parseFloat(_tabelaFrete.PesosTransportados.list[i].PesoInicial.val.toString());
                var pesoFinalCadastrado = Globalize.parseFloat(_tabelaFrete.PesosTransportados.list[i].PesoFinal.val.toString());

                if (pesoFinalCadastrado == 0)
                    pesoFinalCadastrado = 9999999999999999;

                if ((pesoInicial >= pesoInicialCadastrado && pesoInicial <= pesoFinalCadastrado) ||
                    (pesoFinal >= pesoInicialCadastrado && pesoFinal <= pesoFinalCadastrado) ||
                    (pesoInicialCadastrado >= pesoInicial && pesoInicialCadastrado <= pesoFinal) ||
                    (pesoFinalCadastrado >= pesoInicial && pesoFinalCadastrado <= pesoFinal)) {
                    if (_tabelaFrete.PesosTransportados.list[i].ModeloVeicularCarga.val == modeloVeicularCarga && comAjudante == comAjudanteCadastrado && paraCalcularValorBase == paraCalcularValorBaseCadastrado) {
                        if (_tabelaFrete.PesosTransportados.list[i].Tipo.val == EnumTipoPesoTransportado.PorFaixaPesoTransportado) {
                            exibirMensagem(tipoMensagem.aviso, Localization.Resources.Fretes.TabelaFrete.TipoDeCobrancaJaExistente, Localization.Resources.Fretes.TabelaFrete.TipoDeCobrancaEntrouEmConflitoComPesoDeXAX.format(Globalize.format(pesoInicialCadastrado, "n3"), Globalize.format(pesoFinalCadastrado, "n3")))
                            return;
                        }
                    }
                } else if (_tabelaFrete.PesosTransportados.list[i].UnidadeMedida.codEntity != _pesoTransportado.UnidadeMedida.codEntity()) {
                    exibirMensagem(tipoMensagem.aviso, Localization.Resources.Fretes.TabelaFrete.UnidadeDeMedidaDiferente, Localization.Resources.Fretes.TabelaFrete.SomentePossivelCadastrarUmaUnidadeDeMedidaPorTabelaFrete);
                    return;
                }
            }
        } else if (_pesoTransportado.Tipo.val() == EnumTipoPesoTransportado.ValorFixoPorPesoTransportado) {
            for (var i = 0; i < _tabelaFrete.PesosTransportados.list.length; i++) {
                if (_tabelaFrete.PesosTransportados.list[i].Tipo.val == EnumTipoPesoTransportado.ValorFixoPorPesoTransportado) {
                    exibirMensagem(tipoMensagem.aviso, Localization.Resources.Fretes.TabelaFrete.TipoDeCobrancaJaExistente, Localization.Resources.Fretes.TabelaFrete.TipoDeCobrancaPorUnidadeJaExiste);
                    return;
                } else if (_tabelaFrete.PesosTransportados.list[i].UnidadeMedida.codEntity != _pesoTransportado.UnidadeMedida.codEntity()) {
                    exibirMensagem(tipoMensagem.aviso, Localization.Resources.Fretes.TabelaFrete.UnidadeDeMedidaDiferente, Localization.Resources.Fretes.TabelaFrete.SomentePossivelCadastrarUmaUnidadeDeMedidaPorTabelaFrete);
                    return;
                }
            }
        }
        
        _pesoTransportado.Codigo.val(guid());
        _pesoTransportado.DescricaoComponente.val(_pesoTransportado.ComponenteFrete.val());
        _pesoTransportado.CodigoComponenteFrete.val(_pesoTransportado.ComponenteFrete.codEntity());

        _tabelaFrete.PesosTransportados.list.push(SalvarListEntity(_pesoTransportado));

        recarregarGridPesoTransportado();

        _pesoTransportado.UnidadeMedida.defCodEntity = _pesoTransportado.UnidadeMedida.codEntity();
        _pesoTransportado.UnidadeMedida.def = _pesoTransportado.UnidadeMedida.val();

        limparCamposPesoTransportado();
    } else {
        exibirMensagem(tipoMensagem.atencao, Localization.Resources.Fretes.TabelaFrete.CamposObrigatorios, Localization.Resources.Fretes.TabelaFrete.InformeOsCamposObrigatorios);
    }
}

function limparCamposPesoTransportado() {
    LimparCampos(_pesoTransportado);
}

function visibilidadeParaValorFreteBase() {
    return _tabelaFrete.UtilizarDiferencaDoValorBaseApenasFretePagos.val() && _pesoTransportado.Tipo.val() == EnumTipoPesoTransportado.PorFaixaPesoTransportado;
}

function ChangeTipoPesoTransportado(novoValor) {
    if (novoValor == EnumTipoPesoTransportado.ValorFixoPorPesoTransportado) {
        //_pesoTransportado.ModeloVeicularCarga.visible(false);
        _pesoTransportado.ModeloVeicularCarga.val("");
        _pesoTransportado.ModeloVeicularCarga.codEntity(0);
        _pesoTransportado.PesoInicial.visible(false);
        _pesoTransportado.PesoFinal.visible(false);
        _pesoTransportado.Peso.visible(true);
        _pesoTransportado.ComAjudante.visible(false);
    } else if (novoValor == EnumTipoPesoTransportado.PorFaixaPesoTransportado) {
        //_pesoTransportado.ModeloVeicularCarga.visible(_tabelaFrete.ParametroBase.val() == EnumTipoParametroBaseTabelaFrete.Peso);
        _pesoTransportado.PesoInicial.visible(true);
        _pesoTransportado.PesoFinal.visible(true);
        _pesoTransportado.Peso.visible(false);
        _pesoTransportado.ComAjudante.visible(true);
    }

    _pesoTransportado.ParaCalcularValorBase.visible(visibilidadeParaValorFreteBase());
    CarregarGridDadosTransporte();
}

function ChangeCalcularFretePorPesoCubado(novoValor) {
    if (novoValor) {
        _pesoTransportadoDadosGerais.AplicarMaiorValorEntrePesoEPesoCubado.visible(true);
        _pesoTransportadoDadosGerais.FatorCubagem.visible(true);
        _pesoTransportadoDadosGerais.IsencaoCubagem.visible(true);

    } else {
        _pesoTransportadoDadosGerais.AplicarMaiorValorEntrePesoEPesoCubado.visible(false);
        _pesoTransportadoDadosGerais.FatorCubagem.visible(false);
        _pesoTransportadoDadosGerais.IsencaoCubagem.visible(false);
    }
}

