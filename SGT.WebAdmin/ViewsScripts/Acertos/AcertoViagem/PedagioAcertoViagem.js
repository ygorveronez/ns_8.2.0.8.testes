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
/// <reference path="../../../js/libs/jquery.twbsPagination.js" />
/// <reference path="../../../js/libs/jquery.globalize.js" />
/// <reference path="../../../js/libs/jquery.globalize.pt-BR.js" />
/// <reference path="CabecalhoAcertoViagem.js" />
/// <reference path="../../Consultas/Carga.js" />
/// <reference path="CargaAcertoViagem.js" />
/// <reference path="AcertoViagem.js" />
/// <reference path="../../Consultas/Veiculo.js" />
/// <reference path="../../Consultas/Pedagio.js" />
/// <reference path="DespesaAcertoViagem.js" />
/// <reference path="AbastecimentoAcertoViagem.js" />
/// <reference path="FechamentoAcertoViagem.js" />
/// <reference path="EtapaAcertoViagem.js" />
/// <reference path="OcorrenciaAcertoViagem.js" />
/// <reference path="../../Enumeradores/EnumTipoPedagio.js" />
/// <reference path="../../Enumeradores/EnumPermissaoPersonalizada.js" />

var _tipoPedagio = [
    { text: "Sem Parar", value: 1 },
    { text: "Pago pelo Motorista", value: 2 }
];
var _tipoDebitoCreditoPedagio = [{ text: "Débito", value: EnumTipoPedagio.Debito }, { text: "Crédito", value: EnumTipoPedagio.Credito }];

//*******MAPEAMENTO KNOUCKOUT*******

var _pedagioAcertoViagem;
var _gridPedagios;
var _gridPedagiosCredito;
var _HTMLPedagioAcertoViagem;
var _novoPedagio;

var PedagioAcertoViagem = function () {
    this.Pedagios = PropertyEntity({ type: types.map, required: false, text: "Buscar pedágio dos veículos", getType: typesKnockout.dynamic, idBtnSearch: guid(), idGrid: guid(), enable: ko.observable(true) });
    this.AdicionarPedagio = PropertyEntity({ eventClick: InformarPedagioClick, type: types.event, text: "Adicionar novo pedágio", visible: ko.observable(true), enable: ko.observable(true) });
    this.RemoverPegadios = PropertyEntity({ eventClick: RemoverPedagiosMultiplosClick, type: types.event, text: "Remover pedágios selecionados", visible: ko.observable(true), enable: ko.observable(true) });

    this.PedagiosCredito = PropertyEntity({ type: types.map, required: false, text: "Buscar pedágio de crédito dos veículos", getType: typesKnockout.dynamic, idBtnSearch: guid(), idGrid: guid(), enable: ko.observable(true) });
    this.AdicionarPedagioCredito = PropertyEntity({ eventClick: InformarPedagioCreditoClick, type: types.event, text: "Adicionar novo pedágio de crédito", visible: ko.observable(true), enable: ko.observable(true) });
    this.RemoverPegadiosCredito = PropertyEntity({ eventClick: RemoverPedagiosMultiplosCreditoClick, type: types.event, text: "Remover pedágios de crédito selecionados", visible: ko.observable(true), enable: ko.observable(true) });

    this.QuantidadePedagio = PropertyEntity({ text: "Número Pedágios: ", getType: typesKnockout.string, val: ko.observable(""), visible: true });
    this.PagoEmpresa = PropertyEntity({ text: "Pago pela Empresa: ", getType: typesKnockout.string, val: ko.observable(""), visible: true });
    this.PagoMotorista = PropertyEntity({ text: "Pago pelo Motorista: ", getType: typesKnockout.string, val: ko.observable(""), visible: true });
    this.ValorTotal = PropertyEntity({ text: "Valor Total: ", getType: typesKnockout.string, val: ko.observable(""), visible: true });
    this.ValorTotalCredito = PropertyEntity({ text: "Total de Crédito: ", getType: typesKnockout.string, val: ko.observable(""), visible: true });
    this.Autorizar = PropertyEntity({ eventClick: AutorizarPegadioClick, type: types.event, text: "Autorizar", visible: ko.observable(true), enable: ko.observable(true) });

    this.RetornarAbastecimento = PropertyEntity({ eventClick: RetornarAbastecimentoClick, type: types.event, text: "Retornar Abascetimentos", visible: ko.observable(false), enable: ko.observable(true) });
    this.InformarOutrasDespesas = PropertyEntity({ eventClick: InformarOutrasDespesasClick, type: types.event, text: "Salvar Pedágios", visible: ko.observable(true), enable: ko.observable(true) });
};

var AdicionarPedagio = function () {
    this.Praca = PropertyEntity({ getType: typesKnockout.string, required: false, maxlength: 300, text: "Praça:" });
    this.Data = PropertyEntity({ getType: typesKnockout.dateTime, required: true, dataLimit: _acertoViagem.DataFinal, text: "*Data Hora Pedágio:", issue: 2 });
    this.Veiculo = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "*Veiculo:", idBtnSearch: guid(), required: true });
    this.Rodovia = PropertyEntity({ getType: typesKnockout.string, required: false, maxlength: 300, text: "Rodovia:" });
    this.TipoPedagio = PropertyEntity({ val: ko.observable(2), options: _tipoPedagio, def: ko.observable(2), text: "*Tipo Pedágio: ", required: true, issue: 223 });
    this.Valor = PropertyEntity({ getType: typesKnockout.decimal, required: true, text: "*Valor Pedágio:", maxlength: 10 });
    this.TipoPedagioDebitoCredito = PropertyEntity({ text: "*Tipo: ", val: ko.observable(EnumTipoPedagio.Debito), options: _tipoDebitoCreditoPedagio, def: EnumTipoPedagio.Debito, enable: ko.observable(true), required: true, visible: ko.observable(false) });
    this.Adicionar = PropertyEntity({ type: types.event, eventClick: AdicionarPedagioClick, text: "Adicionar", visible: ko.observable(true) });

    this.MoedaCotacaoBancoCentral = PropertyEntity({ val: ko.observable(EnumMoedaCotacaoBancoCentral.Real), options: EnumMoedaCotacaoBancoCentral.obterOpcoes(), def: EnumMoedaCotacaoBancoCentral.Real, text: "Moeda: ", visible: ko.observable(false), enable: ko.observable(true) });
    this.DataBaseCRT = PropertyEntity({ text: "Data Base CRT: ", required: false, getType: typesKnockout.dateTime, enable: ko.observable(true), visible: ko.observable(false) });
    this.ValorMoedaCotacao = PropertyEntity({ text: "Valor Moeda: ", required: false, getType: typesKnockout.decimal, enable: ko.observable(true), visible: ko.observable(false), configDecimal: { precision: 10, allowZero: false, allowNegative: false }, maxlength: 22 });
    this.ValorOriginalMoedaEstrangeira = PropertyEntity({ text: "Valor Original Moeda: ", required: false, getType: typesKnockout.decimal, enable: ko.observable(true), visible: ko.observable(false) });

    this.MoedaCotacaoBancoCentral.val.subscribe(function (novoValor) {
        CalcularMoedaEstrangeiraPedagio();
    });

    this.DataBaseCRT.val.subscribe(function (novoValor) {
        CalcularMoedaEstrangeiraPedagio();
    });

    this.ValorMoedaCotacao.val.subscribe(function (novoValor) {
        ConverterValorPedagio();
    });

    this.ValorOriginalMoedaEstrangeira.val.subscribe(function (novoValor) {
        ConverterValorPedagio();
    });

    this.Valor.val.subscribe(function (novoValor) {
        ConverterValorOriginalDespesa();
    });
};


//*******EVENTOS*******

function loadPedagioAcertoViagem() {
    $("#contentPedagioAcertoViagem").html("");
    var idDiv = guid();
    $("#contentPedagioAcertoViagem").append(_HTMLPedagioAcertoViagem.replace(/#pedagioAcertoViagem/g, idDiv));

    _pedagioAcertoViagem = new PedagioAcertoViagem();
    KoBindings(_pedagioAcertoViagem, idDiv);

    _novoPedagio = new AdicionarPedagio();
    KoBindings(_novoPedagio, "knoutAdicionarPedagio");

    _pedagioAcertoViagem.Autorizar.visible(VerificaSePossuiPermissaoEspecial(EnumPermissaoPersonalizada.Acerto_PermiteLiberarPedagioAcerto, _PermissoesPersonalizadas));

    new BuscarVeiculos(_novoPedagio.Veiculo, null, null, null, null, null, _acertoViagem.Codigo);

    if (_CONFIGURACAO_TMS.UtilizaMoedaEstrangeira) {
        _novoPedagio.MoedaCotacaoBancoCentral.visible(true);
        _novoPedagio.DataBaseCRT.visible(true);
        _novoPedagio.ValorMoedaCotacao.visible(true);
        _novoPedagio.ValorOriginalMoedaEstrangeira.visible(true);
    }

    LoadGridPedagio();
    LoadGridPedagioCredito();

    recarregarGridPedagios();
    recarregarGridPedagiosCredito();

    carregarConteudosDespesaHTML(loadDespesaAcertoViagem);
}

function LoadGridPedagio() {
    var excluir = {
        descricao: "Remover", id: guid(), evento: "onclick", metodo: function (data) {
            RemoverPedagioClick(_pedagioAcertoViagem.Pedagios, data);
        }, tamanho: "15", icone: ""
    };
    var menuOpcoes = new Object();
    menuOpcoes.tipo = TypeOptionMenu.link;
    menuOpcoes.opcoes = new Array();
    menuOpcoes.opcoes.push(excluir);
    var header = [
        { data: "DataHoraPedagio", visible: false },
        { data: "CodigoAcertoPedagio", visible: false },
        { data: "Codigo", visible: false },
        { data: "LancadoManualmente", visible: false },
        { data: "DataHora", title: "Data", width: "20%", className: "text-align-center", orderable: false },
        { data: "Placa", title: "Placa", width: "15%", className: "text-align-center" },
        { data: "CodigoVeiculo", visible: false },
        { data: "SemParar", visible: false },
        { data: "Praca", title: "Praça", width: "50%", className: "text-align-left" },
        { data: "Rodovia", title: "Rodovia", width: "40%", className: "text-align-left" },
        { data: "Tipo", title: "Tipo", width: "25%", className: "text-align-center" },
        { data: "Valor", title: "Valor", width: "15%", className: "text-align-right" },
        { data: "PedagioDuplicado", visible: false },
        { data: "SituacaoPedagio", visible: false },
        { data: "DT_RowColor", visible: false },
        { data: "TipoPedagio", visible: false },
        { data: "MoedaCotacaoBancoCentral", visible: false },
        { data: "DataBaseCRT", visible: false },
        { data: "ValorMoedaCotacao", visible: false },
        { data: "ValorOriginalMoedaEstrangeira", visible: false }
    ];

    _gridPedagios = new BasicDataTable(_pedagioAcertoViagem.Pedagios.idGrid, header, menuOpcoes, { column: 0, dir: orderDir.asc }, { permiteSelecao: true, marcarTodos: false, permiteSelecionarTodos: true });
    _pedagioAcertoViagem.Pedagios.basicTable = _gridPedagios;

    new BuscarPedagiosSemAcertoDeViagem(_pedagioAcertoViagem.Pedagios, RetornoInserirPedagio, _gridPedagios, _acertoViagem.Codigo, EnumTipoPedagio.Debito);
}

function LoadGridPedagioCredito() {
    var excluir = {
        descricao: "Remover", id: guid(), evento: "onclick", metodo: function (data) {
            RemoverPedagioCreditoClick(_pedagioAcertoViagem.PedagiosCredito, data);
        }, tamanho: "15", icone: ""
    };
    var menuOpcoes = new Object();
    menuOpcoes.tipo = TypeOptionMenu.link;
    menuOpcoes.opcoes = new Array();
    menuOpcoes.opcoes.push(excluir);
    var header = [
        { data: "DataHoraPedagio", visible: false },
        { data: "CodigoAcertoPedagio", visible: false },
        { data: "Codigo", visible: false },
        { data: "LancadoManualmente", visible: false },
        { data: "DataHora", title: "Data", width: "20%", className: "text-align-center", orderable: false },
        { data: "Placa", title: "Placa", width: "15%", className: "text-align-center" },
        { data: "CodigoVeiculo", visible: false },
        { data: "SemParar", visible: false },
        { data: "Praca", title: "Praça", width: "50%", className: "text-align-left" },
        { data: "Rodovia", title: "Rodovia", width: "40%", className: "text-align-left" },
        { data: "Tipo", title: "Tipo", width: "25%", className: "text-align-center" },
        { data: "Valor", title: "Valor", width: "15%", className: "text-align-right" },
        { data: "PedagioDuplicado", visible: false },
        { data: "SituacaoPedagio", visible: false },
        { data: "DT_RowColor", visible: false },
        { data: "TipoPedagio", visible: false },
        { data: "MoedaCotacaoBancoCentral", visible: false },
        { data: "DataBaseCRT", visible: false },
        { data: "ValorMoedaCotacao", visible: false },
        { data: "ValorOriginalMoedaEstrangeira", visible: false }
    ];

    _gridPedagiosCredito = new BasicDataTable(_pedagioAcertoViagem.PedagiosCredito.idGrid, header, menuOpcoes, { column: 0, dir: orderDir.asc }, { permiteSelecao: true, marcarTodos: false, permiteSelecionarTodos: true });
    _pedagioAcertoViagem.PedagiosCredito.basicTable = _gridPedagiosCredito;

    new BuscarPedagiosSemAcertoDeViagem(_pedagioAcertoViagem.PedagiosCredito, RetornoInserirPedagioCredito, _gridPedagiosCredito, _acertoViagem.Codigo, EnumTipoPedagio.Credito);
}

function RetornoInserirPedagio(data) {
    if (data != null) {
        var dataGrid = _gridPedagios.BuscarRegistros();
        for (var i = 0; i < data.length; i++) {

            _pedagioAcertoViagem.QuantidadePedagio.val(_pedagioAcertoViagem.QuantidadePedagio.val() + 1);

            if (data[i].SemParar) {
                var pagoEmpresa = parseFloat(_pedagioAcertoViagem.PagoEmpresa.val().replace(".", "").replace(",", ".")) - parseFloat(data[i].Valor.replace(".", "").replace(",", "."));
                _pedagioAcertoViagem.PagoEmpresa.val(mvalor(pagoEmpresa.toFixed(2).toString()));
            } else {
                var pagoMotorista = parseFloat(_pedagioAcertoViagem.PagoMotorista.val().replace(".", "").replace(",", ".")) - parseFloat(data[i].Valor.replace(".", "").replace(",", "."));
                _pedagioAcertoViagem.PagoMotorista.val(mvalor(pagoMotorista.toFixed(2).toString()));
            }

            var valorTotal = parseFloat(_pedagioAcertoViagem.ValorTotal.val().replace(".", "").replace(",", ".")) - parseFloat(data[i].Valor.replace(".", "").replace(",", "."));
            _pedagioAcertoViagem.ValorTotal.val(mvalor(valorTotal.toFixed(2).toString()));

            var obj = new Object();

            obj.PedagioDuplicado = data[i].PedagioDuplicado;
            obj.SituacaoPedagio = data[i].SituacaoPedagio;
            obj.CodigoAcertoPedagio = 0;
            obj.Codigo = data[i].Codigo;
            obj.LancadoManualmente = true;
            obj.DataHoraPedagio = data[i].DataHora;
            obj.DataHora = data[i].DataHora;
            obj.Placa = data[i].Placa;
            obj.CodigoVeiculo = data[i].CodigoVeiculo;
            obj.SemParar = data[i].SemParar;
            obj.Praca = data[i].Praca;
            obj.Rodovia = data[i].Rodovia;
            obj.Tipo = data[i].SemParar ? "Sem Parar" : "Pago pelo Motorista";
            obj.DT_RowColor = data[i].SituacaoPedagio != 3 ? "#DCDCDC" : data[i].PedagioDuplicado ? "#FF8C69" : "#ADD8E6";
            obj.Valor = data[i].Valor;
            obj.TipoPedagio = data[i].TipoPedagio;

            obj.MoedaCotacaoBancoCentral = data[i].MoedaCotacaoBancoCentral;
            obj.DataBaseCRT = data[i].DataBaseCRT;
            obj.ValorMoedaCotacao = data[i].ValorMoedaCotacao;
            obj.ValorOriginalMoedaEstrangeira = data[i].ValorOriginalMoedaEstrangeira;

            dataGrid.push(obj);
        }
        VerificaBotoesAutorizacaoPedagio();
        _gridPedagios.CarregarGrid(dataGrid);
    }
}

function RetornoInserirPedagioCredito(data) {
    if (data != null) {
        var dataGrid = _gridPedagiosCredito.BuscarRegistros();
        for (var i = 0; i < data.length; i++) {

            _pedagioAcertoViagem.QuantidadePedagio.val(_pedagioAcertoViagem.QuantidadePedagio.val() + 1);

            var valorTotal = parseFloat(_pedagioAcertoViagem.ValorTotalCredito.val().replace(".", "").replace(",", ".")) - parseFloat(data[i].Valor.replace(".", "").replace(",", "."));
            _pedagioAcertoViagem.ValorTotalCredito.val(mvalor(valorTotal.toFixed(2).toString()));

            var obj = new Object();

            obj.PedagioDuplicado = data[i].PedagioDuplicado;
            obj.SituacaoPedagio = data[i].SituacaoPedagio;
            obj.CodigoAcertoPedagio = 0;
            obj.Codigo = data[i].Codigo;
            obj.LancadoManualmente = true;
            obj.DataHoraPedagio = data[i].DataHora;
            obj.DataHora = data[i].DataHora;
            obj.Placa = data[i].Placa;
            obj.CodigoVeiculo = data[i].CodigoVeiculo;
            obj.SemParar = data[i].SemParar;
            obj.Praca = data[i].Praca;
            obj.Rodovia = data[i].Rodovia;
            obj.Tipo = "Crédito";
            obj.DT_RowColor = data[i].SituacaoPedagio != 3 ? "#DCDCDC" : data[i].PedagioDuplicado ? "#FF8C69" : "#ADD8E6";
            obj.Valor = data[i].Valor;
            obj.TipoPedagio = data[i].TipoPedagio;

            obj.MoedaCotacaoBancoCentral = data[i].MoedaCotacaoBancoCentral;
            obj.DataBaseCRT = data[i].DataBaseCRT;
            obj.ValorMoedaCotacao = data[i].ValorMoedaCotacao;
            obj.ValorOriginalMoedaEstrangeira = data[i].ValorOriginalMoedaEstrangeira;

            dataGrid.push(obj);
        }
        _gridPedagiosCredito.CarregarGrid(dataGrid);
    }
}

function RemoverPedagiosMultiplosClick(e, data) {
    if (_acertoViagem == null || _acertoViagem.Codigo == null || _acertoViagem.Codigo.val() == null || _acertoViagem.Codigo.val() == 0) {
        exibirMensagem(tipoMensagem.aviso, "Aviso", "Por favor inicie o acerto de viagem antes.");
        return;
    }
    if (_acertoViagem.Situacao.val() == EnumSituacoesAcertoViagem.Fechado) {
        exibirMensagem(tipoMensagem.aviso, "Aviso", "Este acerto de viagem já se encontra finalizado.");
        return;
    }
    if (_acertoViagem.Situacao.val() == EnumSituacoesAcertoViagem.Cancelado) {
        exibirMensagem(tipoMensagem.aviso, "Aviso", "Este acerto de viagem se encontra cancelado.");
        return;
    }

    var pedagiosRemover = _gridPedagios.ListaSelecionados();
    var pedagioGrid = _gridPedagios.BuscarRegistros();

    for (var a = 0; a < pedagiosRemover.length; a++) {
        for (var i = 0; i < pedagioGrid.length; i++) {
            if (pedagiosRemover[a].Codigo == pedagioGrid[i].Codigo) {

                _pedagioAcertoViagem.Autorizar.enable(true);
                pedagioGrid.splice(i, 1);
                _pedagioAcertoViagem.QuantidadePedagio.val(_pedagioAcertoViagem.QuantidadePedagio.val() - 1);

                if (pedagiosRemover[a].SemParar) {
                    var pagoEmpresa = parseFloat(_pedagioAcertoViagem.PagoEmpresa.val().replace(".", "").replace(",", ".")) - parseFloat(pedagiosRemover[a].Valor.replace(".", "").replace(",", "."));
                    _pedagioAcertoViagem.PagoEmpresa.val(mvalor(pagoEmpresa.toFixed(2).toString()));
                } else {
                    var pagoMotorista = parseFloat(_pedagioAcertoViagem.PagoMotorista.val().replace(".", "").replace(",", ".")) - parseFloat(pedagiosRemover[a].Valor.replace(".", "").replace(",", "."));
                    _pedagioAcertoViagem.PagoMotorista.val(mvalor(pagoMotorista.toFixed(2).toString()));
                }

                var valorTotal = parseFloat(_pedagioAcertoViagem.ValorTotal.val().replace(".", "").replace(",", ".")) - parseFloat(pedagiosRemover[a].Valor.replace(".", "").replace(",", "."));
                _pedagioAcertoViagem.ValorTotal.val(mvalor(valorTotal.toFixed(2).toString()));
                break;
            }
        }
    }
    VerificaBotoesAutorizacaoPedagio();
    _gridPedagios.CarregarGrid(pedagioGrid);
}

function RemoverPedagiosMultiplosCreditoClick(e, data) {
    if (_acertoViagem == null || _acertoViagem.Codigo == null || _acertoViagem.Codigo.val() == null || _acertoViagem.Codigo.val() == 0) {
        exibirMensagem(tipoMensagem.aviso, "Aviso", "Por favor inicie o acerto de viagem antes.");
        return;
    }
    if (_acertoViagem.Situacao.val() == EnumSituacoesAcertoViagem.Fechado) {
        exibirMensagem(tipoMensagem.aviso, "Aviso", "Este acerto de viagem já se encontra finalizado.");
        return;
    }
    if (_acertoViagem.Situacao.val() == EnumSituacoesAcertoViagem.Cancelado) {
        exibirMensagem(tipoMensagem.aviso, "Aviso", "Este acerto de viagem se encontra cancelado.");
        return;
    }

    var pedagiosRemover = _gridPedagiosCredito.ListaSelecionados();
    var pedagioGrid = _gridPedagiosCredito.BuscarRegistros();

    for (var a = 0; a < pedagiosRemover.length; a++) {
        for (var i = 0; i < pedagioGrid.length; i++) {
            if (pedagiosRemover[a].Codigo == pedagioGrid[i].Codigo) {
                pedagioGrid.splice(i, 1);
                _pedagioAcertoViagem.QuantidadePedagio.val(_pedagioAcertoViagem.QuantidadePedagio.val() - 1);

                var valorTotal = parseFloat(_pedagioAcertoViagem.ValorTotalCredito.val().replace(".", "").replace(",", ".")) - parseFloat(pedagiosRemover[a].Valor.replace(".", "").replace(",", "."));
                _pedagioAcertoViagem.ValorTotalCredito.val(mvalor(valorTotal.toFixed(2).toString()));
                break;
            }
        }
    }
    _gridPedagiosCredito.CarregarGrid(pedagioGrid);
}

function InformarPedagioClick(e, data) {
    if (_acertoViagem == null || _acertoViagem.Codigo == null || _acertoViagem.Codigo.val() == null || _acertoViagem.Codigo.val() == 0) {
        exibirMensagem(tipoMensagem.aviso, "Aviso", "Por favor inicie o acerto de viagem antes.");
        return;
    }
    if (_acertoViagem.Situacao.val() == EnumSituacoesAcertoViagem.Fechado) {
        exibirMensagem(tipoMensagem.aviso, "Aviso", "Este acerto de viagem já se encontra finalizado.");
        return;
    }
    if (_acertoViagem.Situacao.val() == EnumSituacoesAcertoViagem.Cancelado) {
        exibirMensagem(tipoMensagem.aviso, "Aviso", "Este acerto de viagem se encontra cancelado.");
        return;
    }

    LimparCampos(_novoPedagio);
    _novoPedagio.TipoPedagioDebitoCredito.val(EnumTipoPedagio.Debito);
        
    Global.abrirModal('divAdicionarPedagio');
}

function InformarPedagioCreditoClick(e, data) {
    if (_acertoViagem == null || _acertoViagem.Codigo == null || _acertoViagem.Codigo.val() == null || _acertoViagem.Codigo.val() == 0) {
        exibirMensagem(tipoMensagem.aviso, "Aviso", "Por favor inicie o acerto de viagem antes.");
        return;
    }
    if (_acertoViagem.Situacao.val() == EnumSituacoesAcertoViagem.Fechado) {
        exibirMensagem(tipoMensagem.aviso, "Aviso", "Este acerto de viagem já se encontra finalizado.");
        return;
    }
    if (_acertoViagem.Situacao.val() == EnumSituacoesAcertoViagem.Cancelado) {
        exibirMensagem(tipoMensagem.aviso, "Aviso", "Este acerto de viagem se encontra cancelado.");
        return;
    }

    LimparCampos(_novoPedagio);
    _novoPedagio.TipoPedagioDebitoCredito.val(EnumTipoPedagio.Credito);
        
    Global.abrirModal('divAdicionarPedagio');
}

function AdicionarPedagioClick(e, sender) {
    if (_acertoViagem == null || _acertoViagem.Codigo == null || _acertoViagem.Codigo.val() == null || _acertoViagem.Codigo.val() == 0) {
        exibirMensagem(tipoMensagem.aviso, "Aviso", "Por favor inicie o acerto de viagem antes.");
        return;
    }
    if (_acertoViagem.Situacao.val() == EnumSituacoesAcertoViagem.Fechado) {
        exibirMensagem(tipoMensagem.aviso, "Aviso", "Este acerto de viagem já se encontra finalizado.");
        return;
    }
    if (_acertoViagem.Situacao.val() == EnumSituacoesAcertoViagem.Cancelado) {
        exibirMensagem(tipoMensagem.aviso, "Aviso", "Este acerto de viagem se encontra cancelado.");
        return;
    }
    if (ValidarCamposObrigatorios(e)) {

        var data = { DataHota: e.Data.val(), CodigoVeiculo: e.Veiculo.codEntity(), Praca: e.Praca.val(), Rodovia: e.Rodovia.val(), TipoPedagio: e.TipoPedagioDebitoCredito.val() };
        executarReST("AcertoPedagio/VerificaPedagioDuplicado", data, function (arg) {
            if (arg.Success) {
                e.TipoPedagio.val(2);

                var valorTotal;
                var dataGrid;
                if (e.TipoPedagioDebitoCredito.val() === EnumTipoPedagio.Debito)
                    dataGrid = _gridPedagios.BuscarRegistros();
                else
                    dataGrid = _gridPedagiosCredito.BuscarRegistros();

                _pedagioAcertoViagem.QuantidadePedagio.val(_pedagioAcertoViagem.QuantidadePedagio.val() + 1);

                if (e.TipoPedagioDebitoCredito.val() === EnumTipoPedagio.Debito) {

                    if (e.TipoPedagio.val() == 1) {
                        var pagoEmpresa = parseFloat(_pedagioAcertoViagem.PagoEmpresa.val().replace(".", "").replace(",", ".")) + parseFloat(e.Valor.val().replace(".", "").replace(",", "."));
                        _pedagioAcertoViagem.PagoEmpresa.val(mvalor(pagoEmpresa.toFixed(2).toString()));
                    } else {
                        var pagoMotorista = parseFloat(_pedagioAcertoViagem.PagoMotorista.val().replace(".", "").replace(",", ".")) + parseFloat(e.Valor.val().replace(".", "").replace(",", "."));
                        _pedagioAcertoViagem.PagoMotorista.val(mvalor(pagoMotorista.toFixed(2).toString()));
                    }

                    valorTotal = parseFloat(_pedagioAcertoViagem.ValorTotal.val().replace(".", "").replace(",", ".")) + parseFloat(e.Valor.val().replace(".", "").replace(",", "."));
                    _pedagioAcertoViagem.ValorTotal.val(mvalor(valorTotal.toFixed(2).toString()));
                } else {
                    valorTotal = parseFloat(_pedagioAcertoViagem.ValorTotalCredito.val().replace(".", "").replace(",", ".")) + parseFloat(e.Valor.val().replace(".", "").replace(",", "."));
                    _pedagioAcertoViagem.ValorTotalCredito.val(mvalor(valorTotal.toFixed(2).toString()));
                }

                var obj = new Object();

                obj.PedagioDuplicado = arg.Data.PedagioDuplicado;
                obj.SituacaoPedagio = 3;
                obj.CodigoAcertoPedagio = dataGrid.length * -1;
                obj.Codigo = 0;
                obj.LancadoManualmente = true;
                obj.DataHoraPedagio = new Date(e.Data.val());
                obj.DataHora = e.Data.val();
                obj.Placa = e.Veiculo.val();
                obj.CodigoVeiculo = e.Veiculo.codEntity();
                obj.SemParar = e.TipoPedagio.val() == 1 ? true : false;
                obj.Praca = e.Praca.val();
                obj.Rodovia = e.Rodovia.val();
                if (e.TipoPedagioDebitoCredito.val() === EnumTipoPedagio.Debito)
                    obj.Tipo = e.TipoPedagio.val() == 1 ? "Sem Parar" : "Pago pelo Motorista";
                else
                    obj.Tipo = "Crédito";
                obj.DT_RowColor = obj.SituacaoPedagio != 3 ? "#DCDCDC" : obj.PedagioDuplicado ? "#FF8C69" : "#ADD8E6";
                obj.Valor = e.Valor.val();
                obj.TipoPedagio = e.TipoPedagioDebitoCredito.val();

                obj.MoedaCotacaoBancoCentral = e.MoedaCotacaoBancoCentral.val();
                obj.DataBaseCRT = e.DataBaseCRT.val();
                obj.ValorMoedaCotacao = e.ValorMoedaCotacao.val();
                obj.ValorOriginalMoedaEstrangeira = e.ValorOriginalMoedaEstrangeira.val();

                dataGrid.push(obj);

                if (e.TipoPedagioDebitoCredito.val() === EnumTipoPedagio.Debito) {
                    _gridPedagios.CarregarGrid(dataGrid);
                    LimparCampos(e);
                    e.TipoPedagioDebitoCredito.val(EnumTipoPedagio.Debito);
                } else {
                    _gridPedagiosCredito.CarregarGrid(dataGrid);
                    LimparCampos(e);
                    e.TipoPedagioDebitoCredito.val(EnumTipoPedagio.Credito);
                }

                e.Valor.val(0);
                if (e.TipoPedagioDebitoCredito.val() === EnumTipoPedagio.Debito) {
                    _pedagioAcertoViagem.Autorizar.enable(true);
                    VerificaBotoesAutorizacaoPedagio();
                }
                $("#" + e.Praca.id).focus();
            } else {
                exibirMensagem(tipoMensagem.falha, "falha", arg.Msg);
            }
        });

    } else {
        exibirMensagem(tipoMensagem.atencao, "Campos Obrigatórios", "Informe os campos obrigatórios.");
    }
}

function AutorizarPegadioClick(e, sender) {
    if (_acertoViagem == null || _acertoViagem.Codigo == null || _acertoViagem.Codigo.val() == null || _acertoViagem.Codigo.val() == 0) {
        exibirMensagem(tipoMensagem.aviso, "Aviso", "Por favor inicie o acerto de viagem antes.");
        return;
    }
    if (_acertoViagem.Situacao.val() == EnumSituacoesAcertoViagem.Fechado) {
        exibirMensagem(tipoMensagem.aviso, "Aviso", "Este acerto de viagem já se encontra finalizado.");
        return;
    }
    if (_acertoViagem.Situacao.val() == EnumSituacoesAcertoViagem.Cancelado) {
        exibirMensagem(tipoMensagem.aviso, "Aviso", "Este acerto de viagem se encontra cancelado.");
        return;
    }
    if (!VerificaSePossuiPermissaoEspecial(EnumPermissaoPersonalizada.Acerto_PermiteLiberarPedagioAcerto, _PermissoesPersonalizadas)) {
        exibirMensagem(tipoMensagem.aviso, "Aviso", "Seu usuário não possui permissão para autorizar os pedágios duplicados.");
        return;
    }

    preencherListasSelecao();

    _acertoViagem.Etapa.val(EnumEtapasAcertoViagem.Pedagios);
    _acertoViagem.AprovacaoPedagio.val(true);
    Salvar(_acertoViagem, "AcertoPedagio/AutorizarPedagios", function (arg) {
        if (arg.Success) {
            _pedagioAcertoViagem.Autorizar.enable(false);
            if (arg.Data) {

                exibirMensagem(tipoMensagem.ok, "Sucesso", "Pedágios aprovados com sucesso.");

                _acertoViagem.AprovacaoPedagio.val(true);
                $("#" + _etapaAcertoViagem.Etapa4.idTab).attr("data-bs-toggle", "tab");
                $("#" + _etapaAcertoViagem.Etapa4.idTab + " .step").attr("class", "step green");
            } else {

                _acertoViagem.AprovacaoPedagio.val(false);
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
                $("#" + _etapaAcertoViagem.Etapa4.idTab).attr("data-bs-toggle", "tab");
                $("#" + _etapaAcertoViagem.Etapa4.idTab + " .step").attr("class", "step yellow");
            }
        } else {
            _acertoViagem.AprovacaoPedagio.val(false);
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
            $("#" + _etapaAcertoViagem.Etapa4.idTab).attr("data-bs-toggle", "tab");
            $("#" + _etapaAcertoViagem.Etapa4.idTab + " .step").attr("class", "step yellow");
        }
    }, sender, exibirCamposObrigatorio);
}

function RemoverPedagioClick(e, sender) {
    if (_acertoViagem == null || _acertoViagem.Codigo == null || _acertoViagem.Codigo.val() == null || _acertoViagem.Codigo.val() == 0) {
        exibirMensagem(tipoMensagem.aviso, "Aviso", "Por favor inicie o acerto de viagem antes.");
        return;
    }
    if (_acertoViagem.Situacao.val() == EnumSituacoesAcertoViagem.Fechado) {
        exibirMensagem(tipoMensagem.aviso, "Aviso", "Este acerto de viagem já se encontra finalizado.");
        return;
    }
    if (_acertoViagem.Situacao.val() == EnumSituacoesAcertoViagem.Cancelado) {
        exibirMensagem(tipoMensagem.aviso, "Aviso", "Este acerto de viagem se encontra cancelado.");
        return;
    }
    var pedagioGrid = e.basicTable.BuscarRegistros();

    for (var i = 0; i < pedagioGrid.length; i++) {
        if (sender.Codigo == pedagioGrid[i].Codigo) {
            _pedagioAcertoViagem.Autorizar.enable(true);
            pedagioGrid.splice(i, 1);
            _pedagioAcertoViagem.QuantidadePedagio.val(_pedagioAcertoViagem.QuantidadePedagio.val() - 1);

            if (sender.SemParar) {
                var pagoEmpresa = parseFloat(_pedagioAcertoViagem.PagoEmpresa.val().replace(".", "").replace(",", ".")) - parseFloat(sender.Valor.replace(".", "").replace(",", "."));
                _pedagioAcertoViagem.PagoEmpresa.val(mvalor(pagoEmpresa.toFixed(2).toString()));
            } else {
                var pagoMotorista = parseFloat(_pedagioAcertoViagem.PagoMotorista.val().replace(".", "").replace(",", ".")) - parseFloat(sender.Valor.replace(".", "").replace(",", "."));
                _pedagioAcertoViagem.PagoMotorista.val(mvalor(pagoMotorista.toFixed(2).toString()));
            }

            var valorTotal = parseFloat(_pedagioAcertoViagem.ValorTotal.val().replace(".", "").replace(",", ".")) - parseFloat(sender.Valor.replace(".", "").replace(",", "."));
            _pedagioAcertoViagem.ValorTotal.val(mvalor(valorTotal.toFixed(2).toString()));
            break;
        }
    }
    VerificaBotoesAutorizacaoPedagio();
    e.basicTable.CarregarGrid(pedagioGrid);
}

function RemoverPedagioCreditoClick(e, sender) {
    if (_acertoViagem == null || _acertoViagem.Codigo == null || _acertoViagem.Codigo.val() == null || _acertoViagem.Codigo.val() == 0) {
        exibirMensagem(tipoMensagem.aviso, "Aviso", "Por favor inicie o acerto de viagem antes.");
        return;
    }
    if (_acertoViagem.Situacao.val() == EnumSituacoesAcertoViagem.Fechado) {
        exibirMensagem(tipoMensagem.aviso, "Aviso", "Este acerto de viagem já se encontra finalizado.");
        return;
    }
    if (_acertoViagem.Situacao.val() == EnumSituacoesAcertoViagem.Cancelado) {
        exibirMensagem(tipoMensagem.aviso, "Aviso", "Este acerto de viagem se encontra cancelado.");
        return;
    }

    var pedagioGrid = e.basicTable.BuscarRegistros();

    for (var i = 0; i < pedagioGrid.length; i++) {
        if (sender.Codigo == pedagioGrid[i].Codigo) {

            pedagioGrid.splice(i, 1);
            _pedagioAcertoViagem.QuantidadePedagio.val(_pedagioAcertoViagem.QuantidadePedagio.val() - 1);

            var valorTotal = parseFloat(_pedagioAcertoViagem.ValorTotalCredito.val().replace(".", "").replace(",", ".")) - parseFloat(sender.Valor.replace(".", "").replace(",", "."));
            _pedagioAcertoViagem.ValorTotalCredito.val(mvalor(valorTotal.toFixed(2).toString()));
            break;
        }
    }

    e.basicTable.CarregarGrid(pedagioGrid);
}

function InformarOutrasDespesasClick(e, sender) {
    if (_acertoViagem == null || _acertoViagem.Codigo == null || _acertoViagem.Codigo.val() == null || _acertoViagem.Codigo.val() == 0) {
        exibirMensagem(tipoMensagem.aviso, "Aviso", "Por favor inicie o acerto de viagem antes.");
        return;
    }
    if (_acertoViagem.Situacao.val() == EnumSituacoesAcertoViagem.Fechado) {
        exibirMensagem(tipoMensagem.aviso, "Aviso", "Este acerto de viagem já se encontra finalizado.");
        return;
    }
    if (_acertoViagem.Situacao.val() == EnumSituacoesAcertoViagem.Cancelado) {
        exibirMensagem(tipoMensagem.aviso, "Aviso", "Este acerto de viagem se encontra cancelado.");
        return;
    }
    preencherListasSelecao();

    _acertoViagem.Etapa.val(EnumEtapasAcertoViagem.Pedagios);
    Salvar(_acertoViagem, "AcertoPedagio/AtualizarPedagios", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Pedágios salvos com sucesso.");

                //$("#" + _etapaAcertoViagem.Etapa5.idTab).click();

                CarregarDadosAcertoViagem(arg.Data.Codigo, null, EnumEtapaAcertoViagem.Pedagios);
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    }, sender, exibirCamposObrigatorio);
}

function RetornarAbastecimentoClick(e, sender) {
    if (_acertoViagem == null || _acertoViagem.Codigo == null || _acertoViagem.Codigo.val() == null || _acertoViagem.Codigo.val() == 0) {
        exibirMensagem(tipoMensagem.aviso, "Aviso", "Por favor inicie o acerto de viagem antes.");
        return;
    }
    if (_acertoViagem.Situacao.val() == EnumSituacoesAcertoViagem.Fechado) {
        exibirMensagem(tipoMensagem.aviso, "Aviso", "Este acerto de viagem já se encontra finalizado.");
        return;
    }
    if (_acertoViagem.Situacao.val() == EnumSituacoesAcertoViagem.Cancelado) {
        exibirMensagem(tipoMensagem.aviso, "Aviso", "Este acerto de viagem se encontra cancelado.");
        return;
    }
    preencherListasSelecao();

    _acertoViagem.Etapa.val(EnumEtapasAcertoViagem.Cargas);
    Salvar(_acertoViagem, "AcertoPedagio/AtualizarPedagios", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Pedágios salvos com sucesso.");

                $("#" + _etapaAcertoViagem.Etapa6.idTab).attr("data-bs-toggle", "tab");
                $("#" + _etapaAcertoViagem.Etapa6.idTab + " .step").attr("class", "step grey");

                $("#" + _etapaAcertoViagem.Etapa5.idTab).attr("data-bs-toggle", "tab");
                $("#" + _etapaAcertoViagem.Etapa5.idTab + " .step").attr("class", "step grey");

                $("#" + _etapaAcertoViagem.Etapa4.idTab).attr("data-bs-toggle", "tab");
                $("#" + _etapaAcertoViagem.Etapa4.idTab + " .step").attr("class", "step grey");

                $("#" + _etapaAcertoViagem.Etapa3.idTab).attr("data-bs-toggle", "tab");
                $("#" + _etapaAcertoViagem.Etapa3.idTab + " .step").attr("class", "step lightgreen");
                $("#" + _etapaAcertoViagem.Etapa3.idTab).click();
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    }, sender, exibirCamposObrigatorio);

}

//*******MÉTODOS*******

function CarregarPedagios(data) {
    recarregarGridPedagios();
    recarregarGridPedagiosCredito();
    VerificaBotoesAutorizacaoPedagio();
}

function recarregarGridPedagios() {
    var cont = 0;
    var total = 0;
    var pagoMotorista = 0;
    var pagoEmpresa = 0;
    var data = new Array();

    if (_acertoViagem.ListaPedagios.val() != "" && _acertoViagem.ListaPedagios.val().length > 0) {
        $.each(_acertoViagem.ListaPedagios.val(), function (i, pedagio) {
            var obj = new Object();
            obj.CodigoAcertoPedagio = pedagio.CodigoAcertoPedagio;
            obj.Codigo = pedagio.Codigo;
            obj.LancadoManualmente = pedagio.LancadoManualmente;
            obj.Data = pedagio.Data;
            obj.Hora = pedagio.Hora;
            obj.DataHoraPedagio = pedagio.DataHoraPedagio;
            obj.DataHora = pedagio.DataHora;
            obj.Placa = pedagio.Placa;
            obj.CodigoVeiculo = pedagio.CodigoVeiculo;
            obj.Praca = pedagio.Praca;
            obj.Rodovia = pedagio.Rodovia;
            obj.Tipo = pedagio.Tipo;
            obj.Valor = pedagio.Valor;
            obj.SemParar = pedagio.SemParar;
            obj.PedagioDuplicado = pedagio.PedagioDuplicado;
            obj.SituacaoPedagio = pedagio.SituacaoPedagio;
            obj.DT_RowColor = pedagio.SituacaoPedagio != 3 ? "#DCDCDC" : pedagio.PedagioDuplicado ? "#FF8C69" : pedagio.LancadoManualmente ? "#ADD8E6" : "#FFFFFF";
            obj.TipoPedagio = pedagio.TipoPedagio;

            obj.MoedaCotacaoBancoCentral = pedagio.MoedaCotacaoBancoCentral;
            obj.DataBaseCRT = pedagio.DataBaseCRT;
            obj.ValorMoedaCotacao = pedagio.ValorMoedaCotacao;
            obj.ValorOriginalMoedaEstrangeira = pedagio.ValorOriginalMoedaEstrangeira;

            cont += 1;
            total += parseFloat(pedagio.Valor.replace(".", "").replace(",", "."));

            if (pedagio.SemParar)
                pagoEmpresa += parseFloat(pedagio.Valor.replace(".", "").replace(",", "."));
            else
                pagoMotorista += parseFloat(pedagio.Valor.replace(".", "").replace(",", "."));

            data.push(obj);
        });
    }

    _gridPedagios.CarregarGrid(data);
    _pedagioAcertoViagem.QuantidadePedagio.val(cont);
    _pedagioAcertoViagem.PagoEmpresa.val(mvalor(pagoEmpresa.toFixed(2).toString()));
    _pedagioAcertoViagem.PagoMotorista.val(mvalor(pagoMotorista.toFixed(2).toString()));
    _pedagioAcertoViagem.ValorTotal.val(mvalor(total.toFixed(2).toString()));
}

function recarregarGridPedagiosCredito() {
    var total = 0;
    var data = new Array();

    if (_acertoViagem.ListaPedagiosCredito.val() != "" && _acertoViagem.ListaPedagiosCredito.val().length > 0) {
        $.each(_acertoViagem.ListaPedagiosCredito.val(), function (i, pedagio) {
            var obj = new Object();
            obj.CodigoAcertoPedagio = pedagio.CodigoAcertoPedagio;
            obj.Codigo = pedagio.Codigo;
            obj.LancadoManualmente = pedagio.LancadoManualmente;
            obj.Data = pedagio.Data;
            obj.Hora = pedagio.Hora;
            obj.DataHoraPedagio = pedagio.DataHoraPedagio;
            obj.DataHora = pedagio.DataHora;
            obj.Placa = pedagio.Placa;
            obj.CodigoVeiculo = pedagio.CodigoVeiculo;
            obj.Praca = pedagio.Praca;
            obj.Rodovia = pedagio.Rodovia;
            obj.Tipo = pedagio.Tipo;
            obj.Valor = pedagio.Valor;
            obj.SemParar = pedagio.SemParar;
            obj.PedagioDuplicado = pedagio.PedagioDuplicado;
            obj.SituacaoPedagio = pedagio.SituacaoPedagio;
            obj.DT_RowColor = pedagio.SituacaoPedagio != 3 ? "#DCDCDC" : pedagio.PedagioDuplicado ? "#FF8C69" : pedagio.LancadoManualmente ? "#ADD8E6" : "#FFFFFF";
            obj.TipoPedagio = pedagio.TipoPedagio;

            obj.MoedaCotacaoBancoCentral = pedagio.MoedaCotacaoBancoCentral;
            obj.DataBaseCRT = pedagio.DataBaseCRT;
            obj.ValorMoedaCotacao = pedagio.ValorMoedaCotacao;
            obj.ValorOriginalMoedaEstrangeira = pedagio.ValorOriginalMoedaEstrangeira;

            total += parseFloat(pedagio.Valor.replace(".", "").replace(",", "."));
            _pedagioAcertoViagem.QuantidadePedagio.val(_pedagioAcertoViagem.QuantidadePedagio.val() + 1);

            data.push(obj);
        });
    }

    _gridPedagiosCredito.CarregarGrid(data);
    _pedagioAcertoViagem.ValorTotalCredito.val(mvalor(total.toFixed(2).toString()));
}

function mvalor(v) {
    v = v.replace(/\D/g, "");
    v = v.replace(/(\d)(\d{8})$/, "$1.$2");
    v = v.replace(/(\d)(\d{5})$/, "$1.$2");

    v = v.replace(/(\d)(\d{2})$/, "$1,$2");
    return v;
}

function carregarConteudosPedagioHTML(calback) {
    $.get("Content/Static/Acerto/PedagioAcertoViagem.html?dyn=" + guid(), function (data) {
        _HTMLPedagioAcertoViagem = data;
        calback();
    });
}

function VerificaBotoesAutorizacaoPedagio() {
    var data = {
        Codigo: _acertoViagem.Codigo.val()
    };
    executarReST("AcertoPedagio/ContemPedagioPendenteAutorizacao", data, function (arg) {
        if (arg.Success) {
            if (arg.Data != null) {
                if (arg.Data.ContemPedagioPendente) {
                    _pedagioAcertoViagem.Autorizar.visible(VerificaSePossuiPermissaoEspecial(EnumPermissaoPersonalizada.Acerto_PermiteLiberarPedagioAcerto, _PermissoesPersonalizadas));
                } else {
                    _pedagioAcertoViagem.Autorizar.visible(false);
                }
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "falha", arg.Msg);
        }
    });
}


function CalcularMoedaEstrangeiraPedagio() {
    if (_CONFIGURACAO_TMS.UtilizaMoedaEstrangeira) {
        executarReST("Cotacao/ConverterMoedaEstrangeira", { MoedaCotacaoBancoCentral: _novoPedagio.MoedaCotacaoBancoCentral.val(), DataBaseCRT: _novoPedagio.DataBaseCRT.val() }, function (r) {
            if (r.Success) {
                _novoPedagio.ValorMoedaCotacao.val(Globalize.format(r.Data, "n10"));
                ConverterValorPedagio();
            } else {
                exibirMensagem(tipoMensagem.falha, "Falha", r.Msg);
            }
        });
    }
}

function ConverterValorPedagio() {
    if (_CONFIGURACAO_TMS.UtilizaMoedaEstrangeira) {
        var valorMoedaCotacao = Globalize.parseFloat(_novoPedagio.ValorMoedaCotacao.val());
        var valorOriginal = Globalize.parseFloat(_novoPedagio.ValorOriginalMoedaEstrangeira.val());
        if (valorOriginal > 0 && valorMoedaCotacao > 0) {
            _novoPedagio.Valor.val(Globalize.format(valorOriginal * valorMoedaCotacao, "n2"));
        }
    }
}

function ConverterValorOriginalPedagio() {
    if (_CONFIGURACAO_TMS.UtilizaMoedaEstrangeira) {
        var valorMoedaCotacao = Globalize.parseFloat(_novoPedagio.ValorMoedaCotacao.val());
        var valorOriginal = Globalize.parseFloat(_novoPedagio.Valor.val());
        if (valorOriginal > 0 && valorMoedaCotacao > 0) {
            _novoPedagio.ValorOriginalMoedaEstrangeira.val(Globalize.format(valorOriginal / valorMoedaCotacao, "n2"));
        }
    }
}