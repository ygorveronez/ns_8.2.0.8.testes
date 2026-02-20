/// <reference path="../../Enumeradores/EnumTipoComponenteFrete.js" />
/// <reference path="../../Consultas/Justificativa.js" />
/// <reference path="../../Consultas/Veiculo.js" />
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
/// <reference path="../../../js/libs/jquery.twbsPagination.js" />
/// <reference path="../../../js/libs/jquery.globalize.js" />
/// <reference path="../../../js/libs/jquery.globalize.pt-BR.js" />
/// <reference path="CabecalhoAcertoViagem.js" />
/// <reference path="../../Consultas/Carga.js" />
/// <reference path="PedagioAcertoViagem.js" />
/// <reference path="AcertoViagem.js" />
/// <reference path="DespesaAcertoViagem.js" />
/// <reference path="AbastecimentoAcertoViagem.js" />
/// <reference path="FechamentoAcertoViagem.js" />
/// <reference path="EtapaAcertoViagem.js" />
/// <reference path="OcorrenciaAcertoViagem.js" />


//*******MAPEAMENTO KNOUCKOUT*******

var _cargaAcertoViagem;
var _gridCargas;
var _HTMLCargaAcertoViagem;
var _HTMLDetalhesCarga;
var _HTMLPedidoCarga;
var _detalheAcerto;
var _pedidoAcerto;
var _gridConhecimento;
var _gridReboque;
var _gridBonificacoes;
var _gridPedagiosCarga;
var _canhotosCarga;
var _palletsCarga;
var _gridCanhotosCarga;
var _gridPalletsCarga;
var _dataAcertoViagem;

var PedidoAcerto = function () {
    this.Pedido = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int, idGrid: guid(), idTab: guid(), enable: ko.observable(false) });

    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.NumeroPedido = PropertyEntity({ text: "Número Pedido: ", getType: typesKnockout.int, val: ko.observable(""), visible: true });
    this.Remetente = PropertyEntity({ text: "Remetente: ", getType: typesKnockout.string, val: ko.observable(""), visible: true, issue: 55 });
    this.Destinatario = PropertyEntity({ text: "Destinatário: ", getType: typesKnockout.string, val: ko.observable(""), visible: true, issue: 55 });
    this.ProdutoPredominante = PropertyEntity({ text: "Produto Predominante: ", getType: typesKnockout.string, val: ko.observable(""), visible: true });
    this.Peso = PropertyEntity({ text: "Peso: ", getType: typesKnockout.string, val: ko.observable(""), visible: true });
    this.Emissora = PropertyEntity({ text: "Empresa Emissora: ", getType: typesKnockout.string, val: ko.observable(""), visible: true, issue: 63 });

}

var DetalheCargaAcerto = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.CodigoAcerto = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Carga = PropertyEntity({ text: "Carga: ", getType: typesKnockout.int, val: ko.observable(""), visible: true });
    this.TipoVeiculo = PropertyEntity({ text: "Tipo Veículo: ", getType: typesKnockout.string, val: ko.observable(""), visible: true, issue: 43 });
    this.TipoCarga = PropertyEntity({ text: "Tipo de Carga: ", getType: typesKnockout.string, val: ko.observable(""), visible: true, issue: 53 });
    this.NumeroEntrega = PropertyEntity({ text: "Número de Entregas: ", getType: typesKnockout.string, val: ko.observable(""), visible: true });
    this.Motorista = PropertyEntity({ text: "Motoristas: ", getType: typesKnockout.string, val: ko.observable(""), visible: true, issue: 145 });
    this.Placa = PropertyEntity({ text: "Tração: ", getType: typesKnockout.string, val: ko.observable(""), visible: true, issue: 143 });
    this.ValorFrete = PropertyEntity({ text: "Valor Frete: ", getType: typesKnockout.string, val: ko.observable(""), visible: true });
    this.ICMS = PropertyEntity({ text: "ICMS: ", getType: typesKnockout.string, val: ko.observable(""), visible: true });
    this.Pedagio = PropertyEntity({ text: "Pedágio: ", getType: typesKnockout.string, val: ko.observable(""), visible: true });
    this.Descarga = PropertyEntity({ text: "Descarga: ", getType: typesKnockout.string, val: ko.observable(""), visible: true });
    this.Frete = PropertyEntity({ text: "Frete: ", getType: typesKnockout.string, val: ko.observable(""), visible: true });
    this.AdValore = PropertyEntity({ text: "AD Valore: ", getType: typesKnockout.string, val: ko.observable(""), visible: true });
    this.ISS = PropertyEntity({ text: "ISS: ", getType: typesKnockout.string, val: ko.observable(""), visible: true });
    this.Outros = PropertyEntity({ text: "Outros: ", getType: typesKnockout.string, val: ko.observable(""), visible: true });
    this.Total = PropertyEntity({ text: "Total a Receber: ", getType: typesKnockout.string, val: ko.observable(""), visible: true });

    this.CargaFracionada = PropertyEntity({ val: ko.observable(false), getType: typesKnockout.bool, text: "Carga Fracionada", def: ko.observable(false), enable: ko.observable(false) });
    this.ValorBrutoCarga = PropertyEntity({ text: " ", getType: typesKnockout.decimal, val: ko.observable(""), visible: true, maxlength: 10, enable: ko.observable(false) });
    this.ValorICMSCarga = PropertyEntity({ text: "Valor ICMS: ", getType: typesKnockout.decimal, val: ko.observable(""), visible: true, maxlength: 10, enable: ko.observable(false) });
    this.ValorBonificacaoCliente = PropertyEntity({ text: "Bonificação do Cliente: ", getType: typesKnockout.decimal, val: ko.observable(""), visible: true, maxlength: 10, enable: ko.observable(false) });

    this.JustificativaBonificacao = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), required: false, text: "*Justificativa Bonificação:", idBtnSearch: guid(), enable: ko.observable(true), visible: ko.observable(true) });
    this.ValorBonificacaoDesconto = PropertyEntity({ text: "*Valor: ", getType: typesKnockout.decimal, val: ko.observable(""), visible: true, maxlength: 10, enable: ko.observable(true) });
    this.AdicionarBonificacao = PropertyEntity({ eventClick: AdicionarBonificacaoClick, type: types.event, text: ko.observable("Add Bonificação"), visible: ko.observable(true), enable: ko.observable(true) });
    this.Bonificacoes = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int, idGrid: guid(), idTab: guid(), enable: ko.observable(false) });

    this.JustificativaPedagio = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), required: false, text: "*Justificativa Pedágio:", idBtnSearch: guid(), enable: ko.observable(true), visible: ko.observable(true) });
    this.ValorPedagio = PropertyEntity({ text: "*Valor: ", getType: typesKnockout.decimal, val: ko.observable(""), visible: true, maxlength: 10, enable: ko.observable(true) });
    this.AdicionarPedagio = PropertyEntity({ eventClick: AdicionarPedagioCargaClick, type: types.event, text: ko.observable("Add Pedágios"), visible: ko.observable(true), enable: ko.observable(true) });
    this.Pedagios = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int, idGrid: guid(), idTab: guid(), enable: ko.observable(false) });

    this.VeiculoReboque = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), required: false, text: "*Reboque:", idBtnSearch: guid(), enable: ko.observable(true) });
    this.AdicionarVeiculo = PropertyEntity({ eventClick: AdicionarVeiculoReboqueCarga, type: types.event, text: ko.observable("Adicionar Veículo Reboque"), visible: ko.observable(true), enable: ko.observable(true) });

    this.Reboques = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int, idGrid: guid(), idTab: guid(), enable: ko.observable(false) });
    this.Conhecimento = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int, idGrid: guid(), idTab: guid(), enable: ko.observable(false) });
    this.Pedido = PropertyEntity({ type: types.map, getType: typesKnockout.dynamic, val: ko.observable(""), def: "", idGrid: guid() });
}

var CargaAcertoViagem = function () {
    this.Cargas = PropertyEntity({ type: types.map, required: false, text: "Adicionar de outras cargas", getType: typesKnockout.dynamic, idBtnSearch: guid(), idGrid: guid(), enable: ko.observable(true) });
    this.BuscarCargas = PropertyEntity({ type: types.map, required: false, text: "Buscar cargas do motorista", getType: typesKnockout.dynamic, idBtnSearch: guid(), idGrid: this.Cargas.idGrid, enable: ko.observable(true) });

    this.TotalCargas = PropertyEntity({ text: "Total de Cargas: ", getType: typesKnockout.decimal, val: ko.observable(0), visible: true });
    this.FreteTotal = PropertyEntity({ text: "Frete Total: ", getType: typesKnockout.decimal, val: ko.observable(0), visible: true });

    this.RetornarAcerto = PropertyEntity({ eventClick: RetornarAcertoClick, type: types.event, text: "Retornar Acerto", visible: ko.observable(false), enable: ko.observable(true) });
    this.IniciarAbastecimento = PropertyEntity({ eventClick: IniciarAbastecimentoClick, type: types.event, text: "Salvar Cargas", visible: ko.observable(true), enable: ko.observable(true) });
}

var PalletsCarga = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Fechar = PropertyEntity({ type: types.event, eventClick: FecharPalletsCargaClick, text: "Fechar", visible: ko.observable(true) });

    this.PalletsCarga = PropertyEntity({ type: types.local, idGrid: guid() });
};

var CanhotosCarga = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Fechar = PropertyEntity({ type: types.event, eventClick: FecharCanhotosCargaClick, text: "Fechar", visible: ko.observable(true) });

    this.CanhotosCarga = PropertyEntity({ type: types.local, idGrid: guid() });
};

var ObjetoAcertoViagem = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Etapa = PropertyEntity({ val: ko.observable(1), options: _etapa, def: 1 });
    this.Situacao = PropertyEntity({ val: ko.observable(1), options: _situacao, def: 1 });
    this.ListaCargas = PropertyEntity({ type: types.map, getType: typesKnockout.dynamic, val: ko.observable(""), def: "", idGrid: guid() });
};

//*******EVENTOS*******

function loadCargaAcertoViagem() {
    $("#contentCargaAcertoViagem").html("");
    var idDiv = guid();
    $("#contentCargaAcertoViagem").append(_HTMLCargaAcertoViagem.replace(/#cargaAcertoViagem/g, idDiv));
    _cargaAcertoViagem = new CargaAcertoViagem();
    KoBindings(_cargaAcertoViagem, idDiv);

    $("#contentDetalheCarga").html("");
    var idDivDetalheAcerto = guid();
    $("#contentDetalheCarga").append(_HTMLDetalhesCarga.replace(/#detalheCarga/g, idDivDetalheAcerto));
    _detalheAcerto = new DetalheCargaAcerto();
    KoBindings(_detalheAcerto, idDivDetalheAcerto);

    _dataAcertoViagem = new ObjetoAcertoViagem();

    var detalhe = {
        descricao: "Detalhe", id: guid(), evento: "onclick", metodo: function (data) {
            DetalheCargaClick(_cargaAcertoViagem.Cargas, data)
        }, tamanho: "10", icone: ""
    };
    var excluir = {
        descricao: "Remover", id: guid(), evento: "onclick", metodo: function (data) {
            RemoverCargaClick(_cargaAcertoViagem.Cargas, data)
        }, tamanho: "10", icone: ""
    };
    var canhotos = {
        descricao: "Canhotos", id: guid(), evento: "onclick", metodo: function (data) {
            CanhotosCargaClick(_cargaAcertoViagem.Cargas, data)
        }, tamanho: "10", icone: ""
    };
    var pallets = { descricao: "Pallets", id: guid(), evento: "onclick", metodo: function (data) { PalletsCargaClick(_cargaAcertoViagem.Cargas, data) }, tamanho: "10", icone: "" };

    var auditar = { descricao: "Auditar", id: "clasEditar", evento: "onclick", metodo: OpcaoAuditoria("AcertoCarga", null, _detalheAcerto), tamanho: "7", icone: "" };

    var menuOpcoes = null;
    if (_CONFIGURACAO_TMS.VisualizarPalletsCanhotosNasCargas)
        menuOpcoes = { tipo: TypeOptionMenu.list, descricao: "Opções", opcoes: [detalhe, canhotos, pallets, excluir], tamanho: 9 };
    else
        menuOpcoes = { tipo: TypeOptionMenu.link, opcoes: [detalhe, excluir] };

    if (VisibilidadeOpcaoAuditoria())
        menuOpcoes.opcoes.push(auditar);

    var header = null;
    if (_CONFIGURACAO_TMS.VisualizarPalletsCanhotosNasCargas)
        header = [{ data: "CodigoAcertoCarga", visible: false },
        { data: "Codigo", visible: false },
        { data: "LancadoManualmente", visible: false },
        { data: "DataCriacaoCarga", visible: false },
        { data: "Data", title: "Data", width: "12%", className: "text-align-center" },
        { data: "Numero", title: "Carga", width: "10%", className: "text-align-center" },
        { data: "Placa", title: "Placa", width: "11%", className: "text-align-center" },
        { data: "Emitente", title: "Emitente", width: "45%", className: "text-align-left" },
        { data: "Destino", title: "Destino", width: "35%", className: "text-align-left" },
        { data: "PedagioAcertoCredito", title: "Pedágio Crédito", width: "15%", className: "text-align-right" },
        { data: "PedagioAcerto", title: "Pedágio", width: "10%", className: "text-align-right" },
        { data: "ValorBonificacaoCliente", title: "Bonificação", width: "10%", className: "text-align-right" },
        { data: "ValorFracionada", title: "Frete Frac.", width: "13%", className: "text-align-right" },
        { data: "CargaFracionada", visible: false },
        { data: "ValorBrutoCarga", visible: false },
        { data: "ValorICMSCarga", visible: false },
        { data: "ValorFrete", title: "Frete a Receber", width: "13%", className: "text-align-right" },
        { data: "PercentualAcerto", title: "% Frete Motorista", width: "13%", className: "text-align-right" },
        { data: "ContemMaisDeUmMotorista", visible: false },
        { data: "ContemMDFeEncerrado", visible: false },
        { data: "DT_RowColor", visible: false },
        { data: "SituacaoCanhotos", title: "Canhotos", width: "15%", className: "text-align-left" },
        { data: "SituacaoPallets", title: "Palletes", width: "15%", className: "text-align-left" }
        ];
    else
        header = [{ data: "CodigoAcertoCarga", visible: false },
        { data: "Codigo", visible: false },
        { data: "LancadoManualmente", visible: false },
        { data: "DataCriacaoCarga", visible: false },
        { data: "Data", title: "Data", width: "12%", className: "text-align-center" },
        { data: "Numero", title: "Carga", width: "10%", className: "text-align-center" },
        { data: "Placa", title: "Placa", width: "11%", className: "text-align-center" },
        { data: "Emitente", title: "Emitente", width: "45%", className: "text-align-left" },
        { data: "Destino", title: "Destino", width: "35%", className: "text-align-left" },
        { data: "PedagioAcertoCredito", title: "Pedágio Crédito", width: "15%", className: "text-align-right" },
        { data: "PedagioAcerto", title: "Pedágio", width: "10%", className: "text-align-right" },
        { data: "ValorBonificacaoCliente", title: "Bonificação", width: "10%", className: "text-align-right" },
        { data: "ValorFracionada", title: "Frete Frac.", width: "13%", className: "text-align-right" },
        { data: "CargaFracionada", visible: false },
        { data: "ValorBrutoCarga", visible: false },
        { data: "ValorICMSCarga", visible: false },
        { data: "ValorFrete", title: "Frete a Receber", width: "13%", className: "text-align-right" },
        { data: "PercentualAcerto", title: "% Frete Motorista", width: "13%", className: "text-align-right" },
        { data: "ContemMaisDeUmMotorista", visible: false },
        { data: "ContemMDFeEncerrado", visible: false },
        { data: "DT_RowColor", visible: false },
        { data: "SituacaoCanhotos", visible: false },
        { data: "SituacaoPallets", visible: false },
        ];

    _gridCargas = new BasicDataTable(_cargaAcertoViagem.Cargas.idGrid, header, menuOpcoes, { column: 3, dir: orderDir.asc });
    _gridCargas.CarregarGrid([]);

    _cargaAcertoViagem.Cargas.basicTable = _gridCargas;
    _cargaAcertoViagem.BuscarCargas.basicTable = _gridCargas;

    new BuscarCargaFinalizadasParaAcertoDeViagem(_cargaAcertoViagem.Cargas, RetornoInserirCarga, _gridCargas);
    new BuscarCargaFinalizadasSemAcertoDeViagem(_cargaAcertoViagem.BuscarCargas, RetornoInserirCargaDoAcerto, _gridCargas, _acertoViagem.Codigo);
    new BuscarReboques(_detalheAcerto.VeiculoReboque, RetornoSelecaoReboque);
    new BuscarJustificativas(_detalheAcerto.JustificativaBonificacao, null, null, [EnumTipoFinalidadeJustificativa.AcertoViagemEmbarcador, EnumTipoFinalidadeJustificativa.Todas]);
    new BuscarJustificativas(_detalheAcerto.JustificativaPedagio, null, null, [EnumTipoFinalidadeJustificativa.AcertoViagemEmbarcador, EnumTipoFinalidadeJustificativa.Todas]);

    $('#' + _cargaAcertoViagem.Cargas.idGrid).on('click', 'tbody td', function (e) {
        if (!_CONFIGURACAO_TMS.VisualizarPalletsCanhotosNasCargas)
            e.stopPropagation();
        editCell(this, e);
    });

    recarregarGridCargas();
    carregarConteudosAbastecimentoHTML(loadAbastecimentoAcertoViagem);

    _canhotosCarga = new CanhotosCarga();
    KoBindings(_canhotosCarga, "knoutCanhotosCarga");

    _gridCanhotosCarga = new GridView(_canhotosCarga.CanhotosCarga.idGrid, "AcertoCarga/ConsultarCanhotos", _canhotosCarga);
    _gridCanhotosCarga.CarregarGrid();

    _palletsCarga = new PalletsCarga();
    KoBindings(_palletsCarga, "knoutPalletsCarga");

    _gridPalletsCarga = new GridView(_palletsCarga.PalletsCarga.idGrid, "AcertoCarga/ConsultarPallets", _palletsCarga);
    _gridPalletsCarga.CarregarGrid();
}

var valorItemEdicao = null;
var htmlItemEdicao = null;
var codigoItemEdicao = null;
var permiteAlterar = true;

function editCell(cell, data) {
    var span = $(cell).find("span")[0];

    if ($(cell).index() == 5 || $(cell).index() == 6 || $(cell).index() == 10 && $(span).data("codigoItem") != null) {
        codigoItemEdicao = $(span).data("codigoItem");
        if ($(cell).index() == 5 || $(cell).index() == 6 || $(cell).index() == 10) {
            permiteAlterar = $(span).data("permiteAlterar");
            if (!permiteAlterar)
                return;
        }

        var idTxt = guid();

        htmlItemEdicao = cell.innerHTML;
        valorItemEdicao = Globalize.parseFloat(cell.innerHTML.split("</span>")[1].trim());
        cell.innerHTML = '<input id="' + idTxt + '" type="text" value="' + Globalize.format(valorItemEdicao, "n2") + '" style="width: 100%; height: 100%;" />';

        $("#" + idTxt).maskMoney();

        switch ($(cell).index()) {
            case 10:

                $("#" + idTxt).attr("maxlength", "6");

                break;
            case 5:

                $("#" + idTxt).attr("maxlength", "10");

                break;
            case 6:

                $("#" + idTxt).attr("maxlength", "10");

                break;
            default:
                break;
        }

        $("#" + idTxt).focus();

        $("#" + idTxt).focusout(function () {
            var valor = Globalize.parseFloat($("#" + idTxt).val());

            if (isNaN(valor) || valor == valorItemEdicao) {
                $(this).closest("td").html(htmlItemEdicao);
                htmlItemEdicao = null;
                valorItemEdicao = null;
            } else {
                var cargaGrid = _gridCargas.BuscarRegistros();

                for (var i = 0; i < cargaGrid.length; i++) {
                    if (codigoItemEdicao == cargaGrid[i].Codigo) {
                        if ($(cell).index() == 5)
                            cargaGrid[i].PedagioAcertoCredito = "<span class='spnTipoValorItem' data-codigo-item='" + codigoItemEdicao + "'></span> " + Globalize.format(valor, "n2");
                        else if ($(cell).index() == 6)
                            cargaGrid[i].PedagioAcerto = "<span class='spnTipoValorItem' data-codigo-item='" + codigoItemEdicao + "'></span> " + Globalize.format(valor, "n2");
                        else if ($(cell).index() == 10)
                            cargaGrid[i].PercentualAcerto = "<span class='spnTipoValorItem' data-codigo-item='" + codigoItemEdicao + "'></span> " + Globalize.format(valor, "n2");
                        break;
                    }
                }
                _gridCargas.CarregarGrid(cargaGrid);
            }
        });
    }
}

function CanhotosCargaClick(e, sender) {
    _canhotosCarga.Codigo.val(sender.Codigo);
    _gridCanhotosCarga.CarregarGrid();    
    Global.abrirModal('divCanhotosCarga');
}

function FecharCanhotosCargaClick(e, sender) {
    LimparCampos(_canhotosCarga);
    Global.fecharModal('divCanhotosCarga');
}

function PalletsCargaClick(e, sender) {
    _palletsCarga.Codigo.val(sender.Codigo);
    _gridPalletsCarga.CarregarGrid();    
    Global.abrirModal('divPalletsCarga');
}

function FecharPalletsCargaClick(e, sender) {
    LimparCampos(_palletsCarga);
    Global.fecharModal('divPalletsCarga');
}

function DetalheCargaClick(e, sender) {
    var data = { codigo: sender.Codigo, CodigoAcerto: _acertoViagem.Codigo.val() };
    executarReST("Carga/BuscarDetalhesDaCargaAcertoViagem", data, function (arg) {
        if (arg.Success) {
            $.each(arg.Data, function (i, carga) {
                var dataCarga = { Data: carga };
                PreencherObjetoKnout(_detalheAcerto, dataCarga);
                LimparCampoEntity(_detalheAcerto.JustificativaBonificacao);
                _detalheAcerto.ValorBonificacaoCliente.enable(false);
                _detalheAcerto.JustificativaBonificacao.visible(true);

                LimparCampoEntity(_detalheAcerto.JustificativaPedagio);
                _detalheAcerto.JustificativaPedagio.visible(true);

                _detalheAcerto.CargaFracionada.val(dataCarga.Data.CargaFracionada);
                $("#" + _detalheAcerto.CargaFracionada.id).click(cargaFracionada);

                if (VerificaSePossuiPermissaoEspecial(EnumPermissaoPersonalizada.Acerto_PermiteInformarBonificacaoCliente, _PermissoesPersonalizadas)) {
                    _detalheAcerto.ValorBonificacaoCliente.enable(false);
                } else {
                    _detalheAcerto.ValorBonificacaoCliente.enable(false);
                }

                if (VerificaSePossuiPermissaoEspecial(EnumPermissaoPersonalizada.Acerto_PermiteInformarCargaFracionada, _PermissoesPersonalizadas)) {
                    _detalheAcerto.CargaFracionada.enable(true);
                } else {
                    _detalheAcerto.CargaFracionada.enable(false);
                }

                if (VerificaSePossuiPermissaoEspecial(EnumPermissaoPersonalizada.Acerto_PermiteAlterarReboquesCarga, _PermissoesPersonalizadas)) {
                    _detalheAcerto.VeiculoReboque.enable(true);
                    _detalheAcerto.AdicionarVeiculo.enable(true);
                } else {
                    _detalheAcerto.VeiculoReboque.enable(false);
                    _detalheAcerto.AdicionarVeiculo.enable(false);
                }

                var valorPedagio = 0.00;
                var valorDescarga = 0.00;
                var valorFrete = 0.00;
                var valorAdValore = 0.00;
                var valorISS = 0.00;
                var valorOutros = 0.00;
                var valorICMS = 0.00;

                if (carga.ComponentesFrete != null && carga.ComponentesFrete.length > 0) {
                    for (var j = 0; j < carga.ComponentesFrete.length; j++) {
                        if (carga.ComponentesFrete[j].TipoComponenteFrete == EnumTipoComponenteFrete.PEDAGIO)
                            valorPedagio += carga.ComponentesFrete[j].ValorComponente;
                        else if (carga.ComponentesFrete[j].TipoComponenteFrete == EnumTipoComponenteFrete.DESCARGA)
                            valorDescarga += carga.ComponentesFrete[j].ValorComponente;
                        else if (carga.ComponentesFrete[j].TipoComponenteFrete == EnumTipoComponenteFrete.ADVALOREM)
                            valorAdValore += carga.ComponentesFrete[j].ValorComponente;
                        else if (carga.ComponentesFrete[j].TipoComponenteFrete == EnumTipoComponenteFrete.FRETE)
                            valorFrete += carga.ComponentesFrete[j].ValorComponente;
                        else if (carga.ComponentesFrete[j].TipoComponenteFrete == EnumTipoComponenteFrete.ICMS)
                            valorICMS += carga.ComponentesFrete[j].ValorComponente;
                        else if (carga.ComponentesFrete[j].TipoComponenteFrete == EnumTipoComponenteFrete.ISS)
                            valorISS += carga.ComponentesFrete[j].ValorComponente;
                        else
                            valorOutros += carga.ComponentesFrete[j].ValorComponente;
                    }
                }
                _detalheAcerto.Pedagio.val(mvalor(valorPedagio.toFixed(2).toString()));
                _detalheAcerto.Descarga.val(mvalor(valorDescarga.toFixed(2).toString()));
                _detalheAcerto.Frete.val(mvalor(valorFrete.toFixed(2).toString()));
                _detalheAcerto.AdValore.val(mvalor(valorAdValore.toFixed(2).toString()));
                _detalheAcerto.ISS.val(mvalor(valorISS.toFixed(2).toString()));
                _detalheAcerto.Outros.val(mvalor(valorOutros.toFixed(2).toString()));
                _detalheAcerto.ICMS.val(mvalor(valorICMS.toFixed(2).toString()));

                var baixarDACTE = { descricao: "Baixar DACTE", id: guid(), metodo: BaixarDacteClick, icone: "" };
                var baixarXML = { descricao: "Baixar XML", id: guid(), metodo: BaixarXMLClick, icone: "" };
                var menuOpcoes = { tipo: TypeOptionMenu.list, descricao: "Opções", tamanho: 7, opcoes: [baixarDACTE, baixarXML] };

                _gridConhecimento = new GridView(_detalheAcerto.Conhecimento.idGrid, "Carga/PesquisaConhecimentoCarga", _detalheAcerto, menuOpcoes, null);
                _gridConhecimento.CarregarGrid();

                var removerReboque = { descricao: "Remover", id: guid(), metodo: RemoverReboqueCarga, icone: "" };
                var menuOpcoes2 = { tipo: TypeOptionMenu.list, descricao: "Opções", tamanho: 7, opcoes: [removerReboque] };

                _gridReboque = new GridView(_detalheAcerto.Reboques.idGrid, "Carga/PesquisaReboquesCarga", _detalheAcerto, menuOpcoes2, null);
                _gridReboque.CarregarGrid();

                var removerBonificacao = { descricao: "Remover", id: guid(), metodo: RemoverBonificacaoClick, icone: "" };
                var auditarBonificacao = { descricao: "Auditar", id: "clasEditar", evento: "onclick", metodo: OpcaoAuditoria("AcertoCargaBonificacao", null, _detalheAcerto), tamanho: "10", icone: "", visibilidade: VisibilidadeOpcaoAuditoria };
                var menuOpcoes3 = { tipo: TypeOptionMenu.list, descricao: "Opções", tamanho: 15, opcoes: [removerBonificacao, auditarBonificacao] };

                _detalheAcerto.CodigoAcerto.val(_acertoViagem.Codigo.val());
                _gridBonificacoes = new GridView(_detalheAcerto.Bonificacoes.idGrid, "AcertoCarga/PesquisaBonificacoes", _detalheAcerto, menuOpcoes3, null);
                _gridBonificacoes.CarregarGrid();


                var removerPedagio = { descricao: "Remover", id: guid(), metodo: RemoverPedagioCargaClick, icone: "" };
                var auditarPedagio = { descricao: "Auditar", id: "clasEditar", evento: "onclick", metodo: OpcaoAuditoria("AcertoCargaPedagio", null, _detalheAcerto), tamanho: "10", icone: "", visibilidade: VisibilidadeOpcaoAuditoria };
                var menuOpcoes4 = { tipo: TypeOptionMenu.list, descricao: "Opções", tamanho: 15, opcoes: [removerPedagio, auditarPedagio] };

                _gridPedagiosCarga = new GridView(_detalheAcerto.Pedagios.idGrid, "AcertoCarga/PesquisaPedagios", _detalheAcerto, menuOpcoes4, null);
                _gridPedagiosCarga.CarregarGrid();

                VerificarBotoes();
                CarregarPedidoCarga(0);
            });            
            Global.abrirModal('divModalDetalheCarga');

            if (_detalheAcerto.CargaFracionada.val() == true && VerificaSePossuiPermissaoEspecial(EnumPermissaoPersonalizada.Acerto_PermiteInformarCargaFracionada, _PermissoesPersonalizadas)) {
                _detalheAcerto.ValorBrutoCarga.enable(true);
                _detalheAcerto.ValorICMSCarga.enable(true);
            } else {
                _detalheAcerto.ValorBrutoCarga.enable(false);
                _detalheAcerto.ValorICMSCarga.enable(false);
            }

            $('#divModalDetalheCarga').on('hidden.bs.modal', function () {
                if (_acertoViagem == null || _acertoViagem.Codigo == null || _acertoViagem.Codigo.val() == null || _acertoViagem.Codigo.val() == 0) {
                    return;
                }
                if (_acertoViagem.Situacao.val() == EnumSituacoesAcertoViagem.Fechado) {
                    return;
                }
                if (_acertoViagem.Situacao.val() == EnumSituacoesAcertoViagem.Cancelado) {
                    return;
                }

                var data = {
                    codigo: _detalheAcerto.Codigo.val(),
                    CargaFracionada: _detalheAcerto.CargaFracionada.val(),
                    ValorBrutoCarga: _detalheAcerto.ValorBrutoCarga.val(),
                    ValorICMSCarga: _detalheAcerto.ValorICMSCarga.val(),
                    ValorBonificacaoCliente: _detalheAcerto.ValorBonificacaoCliente.val()
                };
                executarReST("AcertoCarga/SalvarDetalhesCarga", data, function (arg) {
                    if (arg.Success) {
                        var cargaGrid = _gridCargas.BuscarRegistros();

                        for (var i = 0; i < cargaGrid.length; i++) {
                            if (arg.Data.Codigo == cargaGrid[i].Codigo) {
                                cargaGrid[i].ValorBonificacaoCliente = data.ValorBonificacaoCliente != "" ? data.ValorBonificacaoCliente : "0,00";
                                cargaGrid[i].CargaFracionada = data.CargaFracionada;
                                cargaGrid[i].ValorFracionada = _detalheAcerto.CargaFracionada.val() == true ? _detalheAcerto.ValorBrutoCarga.val() : "0,00";
                                cargaGrid[i].ValorBrutoCarga = data.ValorBrutoCarga != "" ? data.ValorBrutoCarga : "0,00";
                                cargaGrid[i].ValorICMSCarga = data.ValorICMSCarga != "" ? data.ValorICMSCarga : "0,00";
                                cargaGrid[i].DT_RowColor = cargaGrid[i].ContemMaisDeUmMotorista ? "#FFFACD" : cargaGrid[i].CargaFracionada ? "#FFE4C4" : cargaGrid[i].ContemMDFeEncerrado ? "#C4FFC9" : cargaGrid[i].LancadoManualmente ? "#ADD8E6" : "#FFFFFF";
                                break;
                            }
                        }
                        _gridCargas.CarregarGrid(cargaGrid);
                    } else {
                        exibirMensagem(tipoMensagem.aviso, "Falha", arg.Msg);
                    }
                });
            })
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    });
}

function cargaFracionada(e, sender) {
    if (_detalheAcerto.CargaFracionada.val() == true) {
        _detalheAcerto.ValorBrutoCarga.enable(true);
        _detalheAcerto.ValorICMSCarga.enable(true);
    } else {
        _detalheAcerto.ValorBrutoCarga.enable(false);
        _detalheAcerto.ValorICMSCarga.enable(false);

        _detalheAcerto.ValorBrutoCarga.val(_detalheAcerto.Total.val());
        _detalheAcerto.ValorICMSCarga.val(_detalheAcerto.ICMS.val());
    }
}

function calculaValorFrete(e) {
    var valorFreteBruto = 0.0;
    var valorICMSBruto = 0.0;
    var aliquotaICMS = 0.0;
    var pedagio = 0.0;
    var valorICMS = 0.0;

    valorFreteBruto = parseFloat(formatarStrFloat(Globalize.format(_detalheAcerto.Total.val()), "n2")).toFixed(2);
    valorICMSBruto = parseFloat(formatarStrFloat(Globalize.format(_detalheAcerto.ICMS.val()), "n2")).toFixed(2);
    pedagio = parseFloat(formatarStrFloat(Globalize.format(_detalheAcerto.Pedagio.val()), "n2")).toFixed(2);
    if (valorFreteBruto > 0 && valorICMSBruto > 0) {
        aliquotaICMS = (valorICMSBruto * 100) / valorFreteBruto;
        aliquotaICMS = aliquotaICMS.toFixed(2);
    }

    var valorFreteDigitado = 0;
    valorFreteDigitado = parseFloat(formatarStrFloat(Globalize.format($("#" + e.ValorBrutoCarga.id).val()), "n2")).toFixed(2);

    valorFreteBruto = parseFloat(valorFreteBruto);
    valorICMSBruto = parseFloat(valorICMSBruto);
    aliquotaICMS = parseFloat(aliquotaICMS);
    valorFreteDigitado = parseFloat(valorFreteDigitado);
    pedagio = parseFloat(pedagio);

    if (valorFreteBruto > 0 && valorICMSBruto > 0 && aliquotaICMS > 0 && valorFreteDigitado > 0) {
        aliquotaICMS = (100 - aliquotaICMS) / 100;

        valorICMS = (valorFreteDigitado + pedagio)
        valorICMS = (valorICMS / aliquotaICMS);
        valorICMS = valorICMS - (valorFreteDigitado + pedagio);

        _detalheAcerto.ValorICMSCarga.val(Globalize.format(valorICMS, "n2"));
    }
}

function CarregarPedidoCarga(indice) {
    var idDiv = guid();
    $("#contentPedidoCarga").html(_HTMLPedidoCarga.replace("#knoutPedidosDoAcerto", idDiv + "_knoutPedidosDoAcerto"));

    _pedidoAcerto = new PedidoAcerto();
    KoBindings(_pedidoAcerto, idDiv + "_knoutPedidosDoAcerto");

    var classActive = '"';
    if (_detalheAcerto.Pedido.val().length > 1) {
        var html = "";
        $.each(_detalheAcerto.Pedido.val(), function (i, pedido) {
            if (i == indice)
                html += '<li class="active">';
            else
                html += '<li>';

            html += '<a href="javascript:void(0);" onclick="CarregarPedidoCarga(' + i + ')"><span class="hidden-mobile hidden-tablet">' + pedido.NumeroPedido + '</span></a>';
            html += '</li>';
        });
        $("#" + _pedidoAcerto.Pedido.idTab).html(html);
        $("#" + _pedidoAcerto.Pedido.idTab).show();
    }

    _pedidoAcerto.NumeroPedido.val(_detalheAcerto.Pedido.val()[indice].NumeroPedido);
    _pedidoAcerto.Remetente.val(_detalheAcerto.Pedido.val()[indice].Remetente);
    _pedidoAcerto.Destinatario.val(_detalheAcerto.Pedido.val()[indice].Destinatario);
    _pedidoAcerto.ProdutoPredominante.val(_detalheAcerto.Pedido.val()[indice].ProdutoPredominante);
    _pedidoAcerto.Peso.val(_detalheAcerto.Pedido.val()[indice].Peso);
    _pedidoAcerto.Emissora.val(_detalheAcerto.Pedido.val()[indice].Emissora);
}

function BaixarXMLClick(e) {
    var data = { CodigoCTe: e.Codigo, CodigoEmpresa: 0 };
    executarDownload("CargaCTe/DownloadXML", data);
}

function BaixarDacteClick(e) {
    var data = { CodigoCTe: e.Codigo, CodigoEmpresa: 0 };
    executarDownload("CargaCTe/DownloadDacte", data);
}

function RetornoSelecaoReboque(data) {
    if (data != null) {
        _detalheAcerto.VeiculoReboque.codEntity(data.Codigo);
        _detalheAcerto.VeiculoReboque.val(data.Placa);
    }
}

function RemoverBonificacaoClick(e) {
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
    exibirConfirmacao("Confirmação", "Realmente deseja remover o valor selecionado?", function () {
        var data = {
            Codigo: e.Codigo
        };
        var valor = e.Valor;
        var tipoBonificaca = e.TipoBonificaca;
        executarReST("AcertoCarga/RemoverBonificacao", data, function (arg) {
            if (!arg.Success)
                exibirMensagem(tipoMensagem.aviso, "Falha", arg.Msg);
            else {
                var valorBonificacao = parseFloat(_detalheAcerto.ValorBonificacaoCliente.val().toString().replace(".", "").replace(",", "."));
                valorBonificacao = parseFloat(valorBonificacao);

                valor = parseFloat(valor.toString().replace(".", "").replace(",", "."));
                valor = parseFloat(valor);

                if (tipoBonificaca == 1)
                    valorBonificacao = valorBonificacao + valor;
                else
                    valorBonificacao = valorBonificacao - valor;

                _detalheAcerto.ValorBonificacaoCliente.val(Globalize.format(valorBonificacao, "n2"));

                _gridBonificacoes.CarregarGrid();
            }
        });
    });
}

function RemoverPedagioCargaClick(e) {
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
    exibirConfirmacao("Confirmação", "Realmente deseja remover o valor selecionado?", function () {
        var data = {
            Codigo: e.Codigo
        };
        executarReST("AcertoCarga/RemoverPedagio", data, function (arg) {
            if (!arg.Success)
                exibirMensagem(tipoMensagem.aviso, "Falha", arg.Msg);
            else {
                _gridPedagiosCarga.CarregarGrid();
            }
        });
    });
}

function RemoverReboqueCarga(e) {
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
    exibirConfirmacao("Confirmação", "Realmente deseja remover o reboque " + e.Placa + "?", function () {
        preencherListasSelecao();
        var data = {
            CodigoCarga: _detalheAcerto.Codigo.val(),
            CodigoVeiculo: e.Codigo,
            CodigoAcerto: _acertoViagem.Codigo.val(),
            ListaCargas: _acertoViagem.ListaCargas.val()
        };
        executarReST("Carga/RemoverReboqueCarga", data, function (arg) {
            if (!arg.Success)
                exibirMensagem(tipoMensagem.aviso, "Falha", arg.Msg);
            else {
                _gridReboque.CarregarGrid();
                CarregarDadosAcertoViagem(arg.Data.Codigo, false, EnumEtapaAcertoViagem.Cargas);
            }
        });
    });
}

function AdicionarPedagioCargaClick(e) {
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
    _detalheAcerto.JustificativaPedagio.requiredClass("form-control");
    _detalheAcerto.ValorPedagio.requiredClass("form-control");

    if (_detalheAcerto.ValorPedagio.val() == "" || _detalheAcerto.ValorPedagio.val() == 0 || _detalheAcerto.ValorPedagio.val() < 0 || _detalheAcerto.JustificativaPedagio.val() == 0) {
        _detalheAcerto.ValorPedagio.requiredClass("form-control is-invalid");
        _detalheAcerto.JustificativaPedagio.requiredClass("form-control is-invalid");
        exibirMensagem(tipoMensagem.atencao, "Atenção", "Por favor informe o valor e a justificativa!");
        return;
    }

    exibirConfirmacao("Confirmação", "Realmente deseja adicionar o valor digitado a esta carga?", function () {
        var data = {
            CodigoCarga: _detalheAcerto.Codigo.val(),
            CodigoAcerto: _acertoViagem.Codigo.val(),
            JustificativaPedagio: _detalheAcerto.JustificativaPedagio.codEntity(),
            ValorPedagio: _detalheAcerto.ValorPedagio.val()
        };
        executarReST("AcertoCarga/AdicionarPedagioCarga", data, function (arg) {
            if (!arg.Success)
                exibirMensagem(tipoMensagem.aviso, "Falha", arg.Msg);
            else {
                LimparCampoEntity(_detalheAcerto.JustificativaPedagio);
                _detalheAcerto.ValorPedagio.val("");
                _gridPedagiosCarga.CarregarGrid();
            }
        });
    });
}

function AdicionarBonificacaoClick(e) {
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
    _detalheAcerto.JustificativaBonificacao.requiredClass("form-control");
    _detalheAcerto.ValorBonificacaoDesconto.requiredClass("form-control");

    if (_detalheAcerto.ValorBonificacaoDesconto.val() == "" || _detalheAcerto.ValorBonificacaoDesconto.val() == 0 || _detalheAcerto.ValorBonificacaoDesconto.val() < 0 || _detalheAcerto.JustificativaBonificacao.val() == 0) {
        _detalheAcerto.ValorBonificacaoDesconto.requiredClass("form-control is-invalid");
        _detalheAcerto.JustificativaBonificacao.requiredClass("form-control is-invalid");
        exibirMensagem(tipoMensagem.atencao, "Atenção", "Por favor informe o valor e a justificativa!");
        return;
    }

    exibirConfirmacao("Confirmação", "Realmente deseja adicionar o valor digitado a esta carga?", function () {
        var data = {
            CodigoCarga: _detalheAcerto.Codigo.val(),
            CodigoAcerto: _acertoViagem.Codigo.val(),
            JustificativaBonificacao: _detalheAcerto.JustificativaBonificacao.codEntity(),
            ValorBonificacaoDesconto: _detalheAcerto.ValorBonificacaoDesconto.val()
        };
        executarReST("AcertoCarga/AdicionarBonificacaoCarga", data, function (arg) {
            if (!arg.Success)
                exibirMensagem(tipoMensagem.aviso, "Falha", arg.Msg);
            else {
                var valor = parseFloat(_detalheAcerto.ValorBonificacaoDesconto.val().toString().replace(".", "").replace(",", "."));
                valor = parseFloat(valor);

                var tipoBonificaca = arg.Data.TipoBonificaca;

                var valorBonificacao = parseFloat(_detalheAcerto.ValorBonificacaoCliente.val().toString().replace(".", "").replace(",", "."));
                valorBonificacao = parseFloat(valorBonificacao);

                if (tipoBonificaca == 1)
                    valorBonificacao = valorBonificacao - valor;
                else
                    valorBonificacao = valorBonificacao + valor;

                _detalheAcerto.ValorBonificacaoCliente.val(Globalize.format(valorBonificacao, "n2"));

                LimparCampoEntity(_detalheAcerto.JustificativaBonificacao);
                _detalheAcerto.ValorBonificacaoDesconto.val("");
                _gridBonificacoes.CarregarGrid();
            }
        });
    });
}

function AdicionarVeiculoReboqueCarga(e) {
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
    _detalheAcerto.VeiculoReboque.requiredClass("form-control");

    if (_detalheAcerto.VeiculoReboque.val() == "" || _detalheAcerto.VeiculoReboque.codEntity() == 0 || _detalheAcerto.VeiculoReboque.codEntity() == "") {
        _detalheAcerto.VeiculoReboque.requiredClass("form-control is-invalid");
        exibirMensagem(tipoMensagem.atencao, "Atenção", "Por favor selecione o veículo de reboque!");
        return;
    }

    exibirConfirmacao("Confirmação", "Realmente deseja adicionar o reboque " + _detalheAcerto.VeiculoReboque.val() + " a carga selecionada?", function () {
        preencherListasSelecao();
        var data = {
            CodigoCarga: _detalheAcerto.Codigo.val(),
            CodigoVeiculo: _detalheAcerto.VeiculoReboque.codEntity(),
            CodigoAcerto: _acertoViagem.Codigo.val(),
            ListaCargas: _acertoViagem.ListaCargas.val()
        };
        executarReST("Carga/AdicionarReboqueCarga", data, function (arg) {
            if (!arg.Success)
                exibirMensagem(tipoMensagem.aviso, "Falha", arg.Msg);
            else {
                LimparCampoEntity(_detalheAcerto.VeiculoReboque);
                _gridReboque.CarregarGrid();
                CarregarDadosAcertoViagem(arg.Data.Codigo, false, EnumEtapaAcertoViagem.Cargas);
            }
        });
    });
}

function RetornoInserirCargaDoAcerto(data) {
    InserirCargaDoRetorno(data, false);
}

function RetornoInserirCarga(data) {
    InserirCargaDoRetorno(data, true);
}

function InserirCargaDoRetorno(data, tipLancamento) {
    if (data != null) {
        var dataGrid = _gridCargas.BuscarRegistros();
        var codigosCarga = new Array();
        _cargaAcertoViagem.TotalCargas.val(_cargaAcertoViagem.TotalCargas.val() + 1);
        if (data.length > 0) {
            for (var i = 0; i < data.length; i++) {

                if (data[i].CargaLancadaEmOutroAcerto)
                    exibirMensagem(tipoMensagem.aviso, "Aviso", "A carga de número " + data[i].Numero + " já se encotra em outro Acerto de Viagem. Favor verifique o seu percentual de compartilhamento.");

                var valorTotal = parseFloat(_cargaAcertoViagem.FreteTotal.val().replace(".", "").replace(",", ".")) + parseFloat(data[i].ValorFrete.replace(".", "").replace(",", "."));
                _cargaAcertoViagem.FreteTotal.val(mvalor(valorTotal.toFixed(2).toString()));

                var obj = new Object();
                obj.CodigoAcertoCarga = 0;
                obj.Codigo = data[i].Codigo;
                codigosCarga.push({ Codigo: data[i].Codigo });
                obj.DataCriacaoCarga = data[i].DataCriacaoCarga;
                obj.LancadoManualmente = tipLancamento;
                obj.Data = data[i].Data;
                obj.Numero = data[i].Numero;
                obj.Placa = data[i].Placa;
                obj.Emitente = data[i].Emitente;
                obj.Destino = data[i].Destino;
                obj.ValorBonificacaoCliente = data[i].ValorBonificacaoCliente;
                obj.ValorFracionada = "0,00";
                obj.CargaFracionada = data[i].CargaFracionada;
                obj.ValorBrutoCarga = data[i].ValorBrutoCarga;
                obj.ValorICMSCarga = data[i].ValorICMSCarga;
                obj.PedagioAcertoCredito = data[i].PedagioAcertoCredito;
                obj.PedagioAcerto = data[i].PedagioAcerto;
                obj.ValorFrete = data[i].ValorFrete;
                obj.PercentualAcerto = data[i].PercentualAcerto;
                obj.ContemMaisDeUmMotorista = data[i].ContemMaisDeUmMotorista;
                obj.ContemMDFeEncerrado = data[i].ContemMDFeEncerrado;
                obj.SituacaoCanhotos = data[i].SituacaoCanhotos;
                obj.SituacaoPallets = data[i].SituacaoPallets;
                obj.DT_RowColor = data[i].ContemMaisDeUmMotorista ? "#FFFACD" : data[i].CargaFracionada ? "#FFE4C4" : Globalize.parseFloat(obj.PercentualAcerto.split("</span>")[1].trim()) < 100 ? "#FFFACD" : data[i].ContemMDFeEncerrado ? "#C4FFC9" : data[i].LancadoManualmente ? "#ADD8E6" : "#FFFFFF";

                dataGrid.push(obj);

            }
        }
        _gridCargas.CarregarGrid(dataGrid);

        if (_CONFIGURACAO_TMS.VisualizarPalletsCanhotosNasCargas) {

            var data = { Codigos: JSON.stringify(codigosCarga) };
            executarReST("AcertoCarga/BuscarSituacaoCanhotoPalletCarga", data, function (arg) {
                if (arg.Success) {
                    for (var a = 0; a < arg.Data.length; a++) {
                        var cargaGrid = _gridCargas.BuscarRegistros();
                        for (var i = 0; i < cargaGrid.length; i++) {
                            if (arg.Data[a].Codigo == cargaGrid[i].Codigo) {
                                cargaGrid[i].SituacaoCanhotos = arg.Data[a].SituacaoCanhotos;
                                cargaGrid[i].SituacaoPallets = arg.Data[a].SituacaoPallets;
                            }
                        }
                    }
                    _gridCargas.CarregarGrid(dataGrid);
                } else {
                    exibirMensagem(tipoMensagem.aviso, "Falha", arg.Msg);
                }
            });
        }
    }
}

function RemoverCargaClick(e, sender) {
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
    exibirConfirmacao("Confirmação", "Realmente deseja excluir a carga " + sender.Codigo + "?", function () {
        var cargaGrid = e.basicTable.BuscarRegistros();

        for (var i = 0; i < cargaGrid.length; i++) {
            if (sender.Codigo == cargaGrid[i].Codigo) {
                cargaGrid.splice(i, 1);
                _cargaAcertoViagem.TotalCargas.val(_cargaAcertoViagem.TotalCargas.val() - 1);
                var valorTotal = parseFloat(_cargaAcertoViagem.FreteTotal.val().replace(".", "").replace(",", ".")) - parseFloat(sender.ValorFrete.replace(".", "").replace(",", "."));
                _cargaAcertoViagem.FreteTotal.val(mvalor(valorTotal.toFixed(2).toString()));
                break;
            }
        }
        e.basicTable.CarregarGrid(cargaGrid);
    });
}

function IniciarAbastecimentoClick(e, sender) {
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

    if (_CONFIGURACAO_TMS.TipoServicoMultisoftware == EnumTipoServicoMultisoftware.MultiTMS) {
        _dataAcertoViagem.Codigo = _acertoViagem.Codigo.val();
        _dataAcertoViagem.Etapa = _acertoViagem.Etapa.val();
        _dataAcertoViagem.Situacao = _acertoViagem.Situacao.val();
        _dataAcertoViagem.ListaCargas = _acertoViagem.ListaCargas.val();

        executarReST("AcertoCarga/AtualizarCargas", _dataAcertoViagem, function (arg) {
            if (arg.Success) {
                if (arg.Data) {
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Cargas salvas com sucesso.");

                    CarregarDadosAcertoViagem(arg.Data.Codigo, null, EnumEtapaAcertoViagem.Cargas);
                    CarregarOcorrenciasAcerto();
                } else {
                    exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
            }
        });

    } else {
        _acertoViagem.Etapa.val(EnumEtapasAcertoViagem.Cargas);
        Salvar(_acertoViagem, "AcertoCarga/AtualizarCargas", function (arg) {
            if (arg.Success) {
                if (arg.Data) {
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Cargas salvas com sucesso.");

                    CarregarDadosAcertoViagem(arg.Data.Codigo, null, EnumEtapaAcertoViagem.Cargas);
                    CarregarOcorrenciasAcerto();
                } else {
                    exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
            }
        }, sender, exibirCamposObrigatorio);
    }
}

function RetornarAcertoClick(e, sender) {
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

    _acertoViagem.Etapa.val(EnumEtapasAcertoViagem.Acerto);
    Salvar(_acertoViagem, "AcertoCarga/AtualizarCargas", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Cargas salvas com sucesso.");

                $("#" + _etapaAcertoViagem.Etapa6.idTab).attr("data-bs-toggle", "tab");
                $("#" + _etapaAcertoViagem.Etapa6.idTab + " .step").attr("class", "step grey");

                $("#" + _etapaAcertoViagem.Etapa5.idTab).attr("data-bs-toggle", "tab");
                $("#" + _etapaAcertoViagem.Etapa5.idTab + " .step").attr("class", "step grey");

                $("#" + _etapaAcertoViagem.Etapa4.idTab).attr("data-bs-toggle", "tab");
                $("#" + _etapaAcertoViagem.Etapa4.idTab + " .step").attr("class", "step grey");

                $("#" + _etapaAcertoViagem.Etapa3.idTab).attr("data-bs-toggle", "tab");
                $("#" + _etapaAcertoViagem.Etapa3.idTab + " .step").attr("class", "step grey");

                $("#" + _etapaAcertoViagem.Etapa2.idTab).attr("data-bs-toggle", "tab");
                $("#" + _etapaAcertoViagem.Etapa2.idTab + " .step").attr("class", "step grey");

                $("#" + _etapaAcertoViagem.Etapa1.idTab).attr("data-bs-toggle", "tab");
                $("#" + _etapaAcertoViagem.Etapa1.idTab + " .step").attr("class", "step lightgreen");
                $("#" + _etapaAcertoViagem.Etapa1.idTab).click();
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    }, sender, exibirCamposObrigatorio);

}

//*******MÉTODOS*******

function CarregarCargas(data) {
    recarregarGridCargas();
}

function recarregarGridCargas() {
    var cont = 0;
    var total = 0;
    var data = new Array();

    if (_acertoViagem.ListaCargas.val() != "" && _acertoViagem.ListaCargas.val().length > 0) {

        $.each(_acertoViagem.ListaCargas.val(), function (i, carga) {
            var obj = new Object();
            obj.CodigoAcertoCarga = carga.CodigoAcertoCarga;
            obj.Codigo = carga.Codigo;
            obj.DataCriacaoCarga = carga.DataCriacaoCarga;
            obj.LancadoManualmente = carga.LancadoManualmente;
            obj.Data = carga.Data;
            obj.Numero = carga.Numero;
            obj.Placa = carga.Placa;
            obj.Emitente = carga.Emitente;
            obj.Destino = carga.Destino;
            obj.ValorBonificacaoCliente = carga.ValorBonificacaoCliente;
            obj.CargaFracionada = carga.CargaFracionada;
            obj.ValorICMSCarga = carga.ValorICMSCarga;
            obj.ValorFracionada = carga.ValorFracionada;
            obj.ValorBrutoCarga = carga.ValorBrutoCarga;
            obj.PedagioAcertoCredito = carga.PedagioAcertoCredito;
            obj.PedagioAcerto = carga.PedagioAcerto;
            obj.ValorFrete = carga.ValorFrete;
            obj.PercentualAcerto = carga.PercentualAcerto;
            obj.ContemMaisDeUmMotorista = carga.ContemMaisDeUmMotorista;
            obj.ContemMDFeEncerrado = carga.ContemMDFeEncerrado;
            obj.SituacaoCanhotos = carga.SituacaoCanhotos;
            obj.SituacaoPallets = carga.SituacaoPallets;

            obj.DT_RowColor = carga.ContemMaisDeUmMotorista ? "#FFFACD" : carga.CargaFracionada ? "#FFE4C4" : Globalize.parseFloat(obj.PercentualAcerto.split("</span>")[1].trim()) < 100 ? "#FFFACD" : carga.ContemMDFeEncerrado ? "#C4FFC9" : carga.LancadoManualmente ? "#ADD8E6" : "#FFFFFF";

            cont += 1;
            total += parseFloat(carga.ValorFrete.replace(".", "").replace(",", "."));

            data.push(obj);
        });
    }

    _gridCargas.CarregarGrid(data);
    _cargaAcertoViagem.TotalCargas.val(cont);
    _cargaAcertoViagem.FreteTotal.val(mvalor(total.toFixed(2).toString()));
}

function mvalor(v) {
    v = v.replace(/\D/g, "");
    v = v.replace(/(\d)(\d{8})$/, "$1.$2");
    v = v.replace(/(\d)(\d{5})$/, "$1.$2");

    v = v.replace(/(\d)(\d{2})$/, "$1,$2");
    return v;
}

function carregarConteudosCargaHTML(calback) {
    $.get("Content/Static/Acerto/AcertoModais.html?dyn=" + guid(), function (data) {
        $("#ModaisAcerto").html(data);
        $.get("Content/Static/Acerto/CargaAcertoViagem.html?dyn=" + guid(), function (data) {
            _HTMLCargaAcertoViagem = data;
            $.get("Content/Static/Acerto/DetalheDaCarga.html?dyn=" + guid(), function (data) {
                _HTMLDetalhesCarga = data;
                $.get("Content/Static/Acerto/PedidoDoAcerto.html?dyn=" + guid(), function (data) {
                    _HTMLPedidoCarga = data;
                    calback();
                });
            });

        });
    });
}

function formatarStrFloat(valor) {
    valor = valor.replace(".", "");
    return valor.replace(",", ".");
}
