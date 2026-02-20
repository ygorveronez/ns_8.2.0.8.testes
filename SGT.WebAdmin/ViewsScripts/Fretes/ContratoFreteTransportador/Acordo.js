/// <reference path="../../../js/libs/jquery-2.1.1.js" />
/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="ContratoFreteTransportador.js" />
/// <reference path="AcordoValoresOutrosRecursos .js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _acordo;
var _acordoConfiguracao;
var _gridAcordo;

var _franquiaPorKm = [
    { text: "Sim", value: true },
    { text: "Não", value: false }
];

var _fechamentoDescritivo = {
    1: "Mês",
    2: "#index#ª Quinzena",
    3: "#index#ª Dezena",
    4: "#index#ª Semana",
};

var ABA_ATIVA = 0;

var AcordoConfiguracao = function () {
    var fechamentoPorKm = (_CONFIGURACAO_TMS.TipoFechamentoFrete == EnumTipoFechamentoFrete.FechamentoPorKm);
    var tipoFranquiaPadrao = fechamentoPorKm ? EnumPeriodoAcordoContratoFreteTransportador.Decendial : EnumPeriodoAcordoContratoFreteTransportador.NaoPossui;
    var tipoFechamentoPadrao = fechamentoPorKm ? EnumPeriodoAcordoContratoFreteTransportador.Decendial : EnumPeriodoAcordoContratoFreteTransportador.Mensal;
    var opcoesTipoFranquia = (_CONFIGURACAO_TMS.ObrigatorioInformarDadosContratoFrete && fechamentoPorKm) ? EnumPeriodoAcordoContratoFreteTransportador.obterOpcoes(_CONFIGURACAO_TMS.PossuiIntegracaoLBC) : EnumPeriodoAcordoContratoFreteTransportador.obterOpcoesFranquia(_CONFIGURACAO_TMS.PossuiIntegracaoLBC);

    this.ValorPorMotorista = PropertyEntity({ text: "Valor por Motorista:", val: ko.observable(""), def: "", getType: typesKnockout.decimal, visible: ko.observable(false) });
    this.ValorMensal = PropertyEntity({ text: "*Valor Mensal:", val: ko.observable(""), def: "", getType: typesKnockout.decimal, enableout: ko.observable(!fechamentoPorKm), visible: ko.observable(false), required: false });
    this.QuantidadeMensalCargas = PropertyEntity({ text: "*Quantidade (aproximada) de Cargas Mensal :", val: ko.observable(""), def: "", getType: typesKnockout.int, maxlength: 5, enable: ko.observable(true), visible: ko.observable(false), required: false });
    this.DeduzirValorPorCarga = PropertyEntity({ val: ko.observable(false), getType: typesKnockout.bool, text: "Deduzir o valor nas cargas durante o mês", def: false, enable: ko.observable(true), visible: ko.observable(false) });
    this.UtilizarValorFixoModeloVeicular = PropertyEntity({ val: ko.observable(false), getType: typesKnockout.bool, text: "Deduzir o valor no valor informado no modelo veícular", def: false, enable: ko.observable(true), visible: ko.observable(false) });
    this.ExigeTabelaFreteComValor = PropertyEntity({ val: ko.observable(false), getType: typesKnockout.bool, text: "Exige que a tabela de frete tenha valor", def: false, enable: ko.observable(true), visible: ko.observable(false) });
    this.NaoEmitirComplementoFechamentoFrete = PropertyEntity({ val: ko.observable(false), getType: typesKnockout.bool, text: "Não emitir complementos no fechamento de frete", def: false });

    this.ComponenteFreteValorContrato = PropertyEntity({ text: "Informar valor em um componente de frete:", visible: ko.observable(false), type: types.entity, codEntity: ko.observable(0), idBtnSearch: guid(), enable: ko.observable(false) });
    this.UtilizarComponenteFreteValorContrato = PropertyEntity({ val: ko.observable(false), def: false, getType: typesKnockout.bool, enable: ko.observable(true) });

    this.TipoFranquia = PropertyEntity({ val: ko.observable(tipoFranquiaPadrao), options: ko.observable(opcoesTipoFranquia), def: ko.observable(tipoFranquiaPadrao), enable: ko.observable(true), text: ko.observable("Tipo da Franquia: "), visible: ko.observable(fechamentoPorKm), issue: 1709, required: ko.observable(false) });
    this.TipoFechamento = PropertyEntity({ val: ko.observable(tipoFechamentoPadrao), options: ko.observable(EnumPeriodoAcordoContratoFreteTransportador.obterOpcoes()), def: tipoFechamentoPadrao, enable: ko.observable(true), text: ko.observable("Tipo do Fechamento: "), visible: ko.observable(fechamentoPorKm), issue: 1708, required: ko.observable(false) });

    this.TipoFechamento.val.subscribe(function (val) {
        RenderizaResumo();

        if (val === EnumPeriodoAcordoContratoFreteTransportador.Mensal || _CAMPOS_BLOQUEADOS)
            _acordo.Replicar.visible(false);
        else
            _acordo.Replicar.visible(true);
    });

    this.UtilizarComponenteFreteValorContrato.val.subscribe(function (arg) {
        _acordoConfiguracao.ComponenteFreteValorContrato.enable(arg);
    });

    this.TipoFranquia.val.subscribe(function () {
        validarFranquia();
        GridAcordos();
        RenderizaGridAcordos();
    });

    this.DeduzirValorPorCarga.val.subscribe(validarDeducaoValorCargaMes);
    this.UtilizarValorFixoModeloVeicular.val.subscribe(validarUsoValorModeloVeicular);
    this.QuantidadeAbas = PropertyEntity({ val: ko.computed(QuantidadeAbas, this), type: types.local });
};

function validarUsoValorModeloVeicular(val) {
    if (val === true) {
        _acordoConfiguracao.QuantidadeMensalCargas.required = false;
        _acordoConfiguracao.QuantidadeMensalCargas.visible(false);
    } else {
        _acordoConfiguracao.QuantidadeMensalCargas.required = true;
        _acordoConfiguracao.QuantidadeMensalCargas.visible(true);
    }
}

function validarDeducaoValorCargaMes(val) {
    if (val === true) {
        _acordoConfiguracao.QuantidadeMensalCargas.required = true;
        _acordoConfiguracao.QuantidadeMensalCargas.visible(true);
        _acordoConfiguracao.UtilizarValorFixoModeloVeicular.visible(true);

        _acordoConfiguracao.ComponenteFreteValorContrato.visible(true);
        _acordoConfiguracao.ComponenteFreteValorContrato.enable(false);
        _acordoConfiguracao.UtilizarComponenteFreteValorContrato.val(false);
        _acordoConfiguracao.ComponenteFreteValorContrato.val("");
        $("#liTipoOperacao").show();
    } else {
        _acordoConfiguracao.QuantidadeMensalCargas.required = false;
        _acordoConfiguracao.QuantidadeMensalCargas.visible(false);
        _acordoConfiguracao.ComponenteFreteValorContrato.visible(false);
        _acordoConfiguracao.UtilizarValorFixoModeloVeicular.visible(false);
        $("#liTipoOperacao").hide();
    }
}

function validarFranquia() {
    if (_acordoConfiguracao.TipoFranquia.val() === EnumPeriodoAcordoContratoFreteTransportador.NaoPossui) {
        $("#knockoutAcordoResumo").hide();
        $("#liFranquia").hide();
        $("#knockoutAcordo").removeClass("col-md-9");
        $("#knockoutAcordo").addClass("col-md-12");
        _acordo.FranquiaPorKm.val(false);
        _acordo.FranquiaPorKm.enable(false);
        //_acordo.ValorAcordado.enable(false);
        _acordoConfiguracao.ValorMensal.required = true;
        _acordoConfiguracao.ValorMensal.visible(true);

        if (_CONFIGURACAO_TMS.TipoFechamentoFrete == EnumTipoFechamentoFrete.FechamentoPorKm) {
            _acordoConfiguracao.DeduzirValorPorCarga.visible(true);
            _acordoConfiguracao.ExigeTabelaFreteComValor.visible(true);
        }
        else {
            _acordoConfiguracao.DeduzirValorPorCarga.val(false);
            _acordoConfiguracao.DeduzirValorPorCarga.visible(false);
            _acordoConfiguracao.ExigeTabelaFreteComValor.val(false);
            _acordoConfiguracao.ExigeTabelaFreteComValor.visible(false);
        }
    }
    else {
        $("#knockoutAcordoResumo").show();
        $("#liFranquia").show();
        $("#knockoutAcordo").removeClass("col-md-12");
        $("#knockoutAcordo").addClass("col-md-9");
        _acordo.FranquiaPorKm.val(true);
        _acordo.FranquiaPorKm.enable(true);
        //_acordo.ValorAcordado.enable(true);
        _acordoConfiguracao.ValorMensal.required = false;
        _acordoConfiguracao.ValorMensal.visible(false);
        _acordoConfiguracao.DeduzirValorPorCarga.val(false);
        _acordoConfiguracao.DeduzirValorPorCarga.visible(false);
        _acordoConfiguracao.ExigeTabelaFreteComValor.visible(false);
    }
}

var Acordo = function () {
    var self = this;
    this.Codigo = PropertyEntity({ val: ko.observable(""), def: "", getType: typesKnockout.string });
    this.Periodo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });

    this.Acordos = PropertyEntity({
        idGrid: guid(),
        abas: ko.computed(ControleAbas, this),
        abaClick: abaClick,
        type: types.local
    });

    this.ModeloVeicular = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "*Modelo Veicular:", idBtnSearch: guid(), required: true, enable: ko.observable(true) });
    this.ModeloCalculoFranquia = PropertyEntity({ type: types.entity, codEntity: ko.observable(0) });

    this.Quantidade = PropertyEntity({ text: "Quantidade:", val: ko.observable("0"), def: "0", type: types.map, getType: typesKnockout.int, enable: ko.observable(true), visible: ko.observable(true) });
    this.Quantidade.configInt.allowZero = true;

    this.ValorAcordado = PropertyEntity({ text: "Valor Acordado:", val: ko.observable("0,00"), def: "0,00", type: types.map, getType: typesKnockout.decimal, enable: ko.observable(true), visible: ko.observable(true) });
    this.ValorAcordado.configDecimal.allowZero = true;

    this.Total = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.decimal });
    this.Rotulo = PropertyEntity({ def: 0, getType: typesKnockout.decimal, val: GeraRotulo });
    this.Rotulo.configDecimal.allowZero = true;
    this.RotuloDescricao = PropertyEntity({ def: "0,00", val: function () { return _FormatHelper(self.Rotulo.val()); } });
    this.FranquiaPorKm = PropertyEntity({ val: ko.observable(true), options: _franquiaPorKm, def: true, text: "Franquia por KM: ", enable: ko.observable(true), visible: ko.observable(true) });

    this.Adicionar = PropertyEntity({ eventClick: AdicionarAcordoClick, type: types.event, text: "Adicionar", visible: ko.observable(true) });
    this.Atualizar = PropertyEntity({ eventClick: AtualizarAcordoClick, type: types.event, text: "Atualizar", visible: ko.observable(false) });
    this.Excluir = PropertyEntity({ eventClick: ExcluirAcordoClick, type: types.event, text: "Excluir", visible: ko.observable(false) });
    this.Cancelar = PropertyEntity({ eventClick: CancelarAcordoClick, type: types.event, text: "Cancelar", visible: ko.observable(true) });
    this.Replicar = PropertyEntity({ eventClick: ReplicarAcordoClick, type: types.event, text: "Replicar", visible: ko.observable(true) });
}

//*******EVENTOS*******

function LoadAcordo() {
    _acordoConfiguracao = new AcordoConfiguracao();
    KoBindings(_acordoConfiguracao, "knockoutAcordoConfiguracao");

    _acordo = new Acordo();
    KoBindings(_acordo, "knockoutAcordo");

    new BuscarModelosVeicularesCarga(_acordo.ModeloVeicular, RetornoSelecaoModeloVeicular);
    new BuscarComponentesDeFrete(_acordoConfiguracao.ComponenteFreteValorContrato);

    validarFranquia();
    GridAcordos();
    LoadAcordoResumo();
    LoadAcordoValoresOutrosRecursos();

    if (_CONFIGURACAO_TMS.TipoFechamentoFrete == EnumTipoFechamentoFrete.FechamentoPorFaixaKm)
        $("#knockoutAcordo").hide();
}

function abaClick(aba) {
    var ordem = aba.Aba;

    SetAba(ordem - 1);
}

function AdicionarAcordoClick() {
    if (_CAMPOS_BLOQUEADOS) return;
    var objeto = SalvarObjetoAcordo(true);

    var acordos = GetInfoPorAba();

    if (!ValidarCamposObrigatorios(_acordo))
        return exibirMensagem(tipoMensagem.atencao, "Campos Obrigatórios", "Preencha os campos obrigatórios.");

    if (!ValidarDuplicidadeAcordo(objeto))
        return exibirMensagem(tipoMensagem.atencao, "Duplicidade", "Já existe um cadastro para esse modelo veicular.");

    acordos.push(objeto);
    SetInfoPorAba(acordos);

    LimparCamposCRUDAcordo();
    RenderizaGridAcordos();
}

function AtualizarAcordoClick() {
    var objeto = SalvarObjetoAcordo(false);

    var acordos = GetInfoPorAba();

    if (!ValidarCamposObrigatorios(_acordo))
        return exibirMensagem(tipoMensagem.atencao, "Campos Obrigatórios", "Preencha os campos obrigatórios.");

    if (!ValidarDuplicidadeAcordo(objeto))
        return exibirMensagem(tipoMensagem.atencao, "Duplicidade", "Já existe um cadastro para esse modelo veicular.");

    for (var i = 0; i < acordos.length; i++) {
        if (acordos[i].Codigo.val == objeto.Codigo.val) {
            acordos[i] = objeto;
            break;
        }
    }

    SetInfoPorAba(acordos);
    LimparCamposCRUDAcordo();
    RenderizaGridAcordos();
    RecalcularFranquia();
}

function ExcluirAcordoClick() {
    var acordos = GetInfoPorAba();

    var modeloVeicular = parseInt(_acordo.ModeloVeicular.codEntity());
    if (modeloVeicular > 0 && ValidaModeloVeicularDependente(modeloVeicular))
        return exibirMensagem(tipoMensagem.atencao, "Modelo Veicular", "Existem acordos que dependem desse modelo veicular.");

    for (var i = 0; i < acordos.length; i++) {
        if (acordos[i].Codigo.val == _acordo.Codigo.val()) {
            acordos.splice(i, 1);
            break;
        }
    }

    SetInfoPorAba(acordos);
    LimparCamposCRUDAcordo();
    RenderizaGridAcordos();
    RecalcularFranquia();
}

function CancelarAcordoClick() {
    LimparCamposCRUDAcordo();
}

function ReplicarAcordoClick() {
    if (_CAMPOS_BLOQUEADOS) return;

    exibirConfirmacao("Replicar Dados", "Tem certeza que deseja replicar os dados dessa aba para as demais?", function () {
        var dadosAba = GetInfoPorAba();

        for (var i = 0; i < _acordoConfiguracao.QuantidadeAbas.val(); i++) {
            if (i != ABA_ATIVA) {
                var data = [];
                dadosAba.forEach(function (d) {
                    var replicado = jQuery.extend(true, {}, d);
                    replicado.Periodo.val = i;
                    replicado.Codigo.val = guid();

                    data.push(replicado);
                });

                SetInfoPorIndexAba(data, i);
            }
        }
    });
}

function AcordoChange() {
    RenderizaResumo();
    GerenciarAbasContratoFrete();
    RecalcularFranquia();
}

//*******MÉTODOS*******
function CarregarAcordos(data) {
    RenderizaGridAcordos();
    RenderizaResumo();
}

function EditarAcordo(data) {
    var obj = null;
    GetInfoPorAba().forEach(function (acordo) {
        if (acordo.Codigo.val == data.Codigo)
            obj = acordo;
    });

    if (obj != null) {
        PreencherEditarListEntity(_acordo, obj);

        _acordo.Adicionar.visible(false);
        _acordo.Atualizar.visible(true);
        _acordo.Excluir.visible(true);
    }
}

function ValidarDuplicidadeAcordo(data) {
    var acordos = GetInfoPorAba();

    for (var i = 0; i < acordos.length; i++) {
        var item = acordos[i];
        if (item.ModeloVeicular.codEntity == data.ModeloVeicular.codEntity && item.Codigo.val != data.Codigo.val)
            return false;
    }

    return true;
}

function LimparCamposCRUDAcordo() {
    LimparCampoEntity(_acordo.ModeloVeicular);
    _acordo.Quantidade.val(_acordo.Quantidade.def);
    _acordo.ValorAcordado.val(_acordo.ValorAcordado.def);
    _acordo.FranquiaPorKm.val(_acordo.FranquiaPorKm.def);

    _acordo.Adicionar.visible(true);
    _acordo.Atualizar.visible(false);
    _acordo.Excluir.visible(false);
    //limparCamposValoresOutrosRecurso();
}

function SetAba(ordem) {
    ABA_ATIVA = ordem;

    LimparCamposCRUDAcordo();
    RenderizaGridAcordos();
}

function GetInfoPorAba() {
    return GetInfoPorIndex(ABA_ATIVA);
}

function GetInfoPorIndex(index) {
    var data = _contratoFreteTransportador.Acordos.list.filter(function (a) { return a.Periodo.val == index });
    return data != null ? data.slice() : [];
}

function SetInfoPorIndexAba(acordos, index) {
    var data = _contratoFreteTransportador.Acordos.list.filter(function (a) { return a.Periodo.val != index });
    data = data.concat(acordos);
    _contratoFreteTransportador.Acordos.list = data.slice();

    AcordoChange();

    if (_acordoConfiguracao.TipoFranquia.val() === EnumPeriodoAcordoContratoFreteTransportador.NaoPossui) {
        var valorAcordo = 0;
        for (var i = 0; i < _contratoFreteTransportador.Acordos.list.length; i++) {
            valorAcordo += Globalize.parseFloat(_contratoFreteTransportador.Acordos.list[i].ValorAcordado.val) * _contratoFreteTransportador.Acordos.list[i].Quantidade.val;
        };
        _acordoConfiguracao.ValorMensal.val(_FormatHelper(valorAcordo));
    };
}

function SetInfoPorAba(acordos) {
    SetInfoPorIndexAba(acordos, ABA_ATIVA);
}

function RenderizaGridAcordos() {
    var data = GetInfoPorAba().map(function (item) {
        return {
            Codigo: item.Codigo.val,
            ModeloVeicular: item.ModeloVeicular.val,
            ValorAcordado: item.ValorAcordado.val,
            Quantidade: item.Quantidade.val,
            Rotulo: item.RotuloDescricao.val,
            Total: item.Total.val,
            FranquiaPorKm: item.FranquiaPorKm.val ? "Sim" : "Não"
        };
    });

    _gridAcordo.CarregarGrid(data);
}

function GridAcordos() {
    //-- Grid
    // Menu
    var menuOpcoes = {
        tipo: TypeOptionMenu.link,
        opcoes: [
            {
                descricao: "Editar",
                id: guid(),
                evento: "onclick",
                tamanho: "10",
                icone: "",
                metodo: function (data) {
                    if (_CAMPOS_BLOQUEADOS) return;
                    EditarAcordo(data);
                }
            }
        ]
    };

    // Cabecalho
    var header = [
        { data: "Codigo", visible: false },
        { data: "ModeloVeicular", title: "Modelo do Veiculo", width: "30%", className: "text-align-left" },
        { data: "ValorAcordado", title: "Valor Acordado", width: "10%", className: "text-align-right" },
        { data: "Quantidade", title: "Quan. Veículo", width: "10%", className: "text-align-center" },
        { data: "Rotulo", title: "Rótulo", width: "10%", className: "text-align-right", visible: _acordoConfiguracao.TipoFranquia.val() !== EnumPeriodoAcordoContratoFreteTransportador.NaoPossui },
        { data: "Total", title: "Total", width: "10%", className: "text-align-right", visible: _acordoConfiguracao.TipoFranquia.val() !== EnumPeriodoAcordoContratoFreteTransportador.NaoPossui },
        { data: "FranquiaPorKm", title: "Franquia KM", width: "10%", className: "text-align-center", visible: _acordoConfiguracao.TipoFranquia.val() !== EnumPeriodoAcordoContratoFreteTransportador.NaoPossui },
    ];

    // Grid
    _gridAcordo = new BasicDataTable(_acordo.Acordos.idGrid, header, menuOpcoes, { column: 3, dir: orderDir.desc }, null, 10);
    _gridAcordo.CarregarGrid([]);
}

function LimparCamposAcordo() {
    LimparCampos(_acordo);
    RenderizaGridAcordos();
    RenderizaResumo();
}

function RetornoSelecaoModeloVeicular(data) {
    var modeloCalculoFranquiaCodigo = parseInt(data.ModeloCalculoFranquiaCodigo);

    if (modeloCalculoFranquiaCodigo == 0 || PossuiAcordoPorModeloVeicular(modeloCalculoFranquiaCodigo)) {
        _acordo.ModeloVeicular.val(data.Descricao);
        _acordo.ModeloVeicular.codEntity(data.Codigo);

        _acordo.ModeloCalculoFranquia.val(data.ModeloCalculoFranquiaDescricao);
        _acordo.ModeloCalculoFranquia.codEntity(modeloCalculoFranquiaCodigo);
    } else if (modeloCalculoFranquiaCodigo > 0) {
        exibirMensagem(tipoMensagem.aviso, "Modelo Veiculo", "Nenhum modelo " + data.ModeloCalculoFranquiaDescricao + " configurado.");
        LimparCampoEntity(_acordo.ModeloVeicular);
        LimparCampoEntity(_acordo.ModeloCalculoFranquia);
    }
}

function ValidaModeloVeicularDependente(codigo) {
    var possuiDependencia = false;

    GetInfoPorAba().forEach(function (item) {
        if (item.ModeloCalculoFranquia.codEntity == codigo)
            possuiDependencia = true;
    });

    return possuiDependencia;
}

function QuantidadeAbas() {
    var qtdAbas = Math.floor(30 / this.TipoFechamento.val());

    return qtdAbas;
}

function BuscarAcordoPorModeloVeicular(codigo) {
    var acordo = null;

    GetInfoPorAba().forEach(function (item) {
        if (item.ModeloVeicular.codEntity == codigo)
            acordo = item;
    });

    return acordo;
}

function PossuiAcordoPorModeloVeicular(codigo) {
    var acordo = BuscarAcordoPorModeloVeicular(codigo);

    return acordo != null;
}

function ControleAbas() {
    var qtdAbas = _acordoConfiguracao.QuantidadeAbas.val();
    var descricaoAba = _fechamentoDescritivo[qtdAbas];
    var arrayAbas = [];
    
    for (var i = 1; i <= qtdAbas; i++) arrayAbas.push(i);

    if (ABA_ATIVA > qtdAbas) {
        SetAba(0);
    }

    return arrayAbas.map(function (_index) {
        return {
            Descricao: descricaoAba.replace("#index#", _index),
            Aba: _index
        };
    });
}

function GeraRotulo() {
    if (arguments.length != 0) return;

    var valorAcordado = _ParseFloatHelper(_acordo.ValorAcordado.val());
    var quantidadeVeiculo = _ParseFloatHelper(_acordo.Quantidade.val());
    var quantidadeDezenasMes = _acordoConfiguracao.QuantidadeAbas.val();
    var veiculoDependenteCalculo = BuscarAcordoPorModeloVeicular(_acordo.ModeloCalculoFranquia.codEntity());
    var fatorDivisao = quantidadeVeiculo;
    if (veiculoDependenteCalculo != null)
        fatorDivisao = _ParseIntHelper(veiculoDependenteCalculo.Quantidade.val);

    return valorAcordado * quantidadeVeiculo / quantidadeDezenasMes / fatorDivisao;
}

function SalvarObjetoAcordo(novo) {
    var objeto = SalvarListEntity(_acordo);

    var veiculoDependenteCalculo = BuscarAcordoPorModeloVeicular(objeto.ModeloCalculoFranquia.codEntity);
    var quantidadeVeiculos = _ParseIntHelper(objeto.Quantidade.val);
    var rotulo = objeto.Rotulo.val;

    var fatorMultiplicacao = quantidadeVeiculos;
    if (veiculoDependenteCalculo != null)
        fatorMultiplicacao = _ParseIntHelper(veiculoDependenteCalculo.Quantidade.val);

    var total = fatorMultiplicacao * rotulo;

    if (novo)
        objeto.Codigo.val = guid();

    objeto.Periodo.val = ABA_ATIVA;
    objeto.Total.val = _FormatHelper(total);

    return objeto;
}

function _FormatHelper(val, format) {
    format = format || "n2";
    if (isNaN(val))
        val = 0;

    return Globalize.format(val, format);
}

function _ParseIntHelper(val) {
    return Globalize.parseInt(val + "");
}

function _ParseFloatHelper(val) {
    return Globalize.parseFloat(val + "");
}