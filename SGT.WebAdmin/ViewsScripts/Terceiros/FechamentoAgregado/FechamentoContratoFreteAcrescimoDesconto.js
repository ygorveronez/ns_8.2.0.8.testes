/// <reference path="FechamentoAgregado.js" />
/// <reference path="EtapaFechamentoAgregado.js" />
/// <reference path="Etapa1SelecaoCIOT.js" />
/// <reference path="Etapa2Consolidacao.js" />
/// <reference path="Etapa3Integracao.js" />
/// <reference path="../../Consultas/ContratoFreteAcrescimoDesconto.js" />
/// <reference path="../../Consultas/ContratoFrete.js" />
/// <reference path="../../Enumeradores/EnumTipoJustificativa.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _novoFechamentoContratoFreteAcrescimoDesconto;
var _gridAcrescimo;
var _gridDesconto;

var FechamentoContratoFreteAcrescimoDesconto = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Consolidado = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.bool });
    this.CIOT = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), idBtnSearch: guid(), visible: false, enable: ko.observable(true) });
    this.NumeroCIOT = PropertyEntity({ text: "Número CIOT: " });
    this.CodigoTransportadorContratoFreteOrigem = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });

    this.AdicionarAcrescimo = PropertyEntity({ eventClick: AbrirModalAdicionarFechamentoContratoFreteAcrescimo, type: types.event, text: ko.observable("Adicionar"), visible: ko.observable(true) });

    this.PesquisarAcrescimo = PropertyEntity({
        eventClick: function (e) {
            BuscarFechamentoAgregadoPorCodigo(_fechamentoAgregado.Codigo.val(), false);
            GridAcrescimo();
        }, type: types.event, text: "Pesquisar", idGrid: guid(), visible: ko.observable(true)
    });

    this.AdicionarDesconto = PropertyEntity({ eventClick: AbrirModalAdicionarFechamentoContratoFreteDesconto, type: types.event, text: ko.observable("Adicionar"), visible: ko.observable(true) });

    this.PesquisarDesconto = PropertyEntity({
        eventClick: function (e) {
            BuscarFechamentoAgregadoPorCodigo(_fechamentoAgregado.Codigo.val(), false);
            GridDesconto();
        }, type: types.event, text: "Pesquisar", idGrid: guid(), visible: ko.observable(true)
    });
};

//*******EVENTOS*******

function LoadFechamentoContratoFreteAcrescimoDesconto() {
    _novoFechamentoContratoFreteAcrescimoDesconto = new FechamentoContratoFreteAcrescimoDesconto();
    KoBindings(_novoFechamentoContratoFreteAcrescimoDesconto, "knoutFechamentoContratoFreteAcrescimoDesconto");

    // Inicia grid de dados
}

//*******MÉTODOS*******

function GridAcrescimo() {
    let linhasPorPaginas = 5;

    //-- Cabecalho
    let remover = {
        descricao: "Remover",
        id: guid(),
        evento: "onclick",
        metodo: RemoverVinculoContratoFreteAcrescimoDesconto,
        tamanho: "10",
        icone: "",
        visibilidade: VisibilidadeRemoverAcrescimoDesconto
    };

    let menuOpcoes = {
        tipo: TypeOptionMenu.link,
        opcoes: [remover]
    };

    let configExportacao = {
        url: "FechamentoAgregado/ExportarPesquisaAcrescimo",
        titulo: "Acréscimos",
        id: "btnExportarAcrescimo"
    };

    _novoFechamentoContratoFreteAcrescimoDesconto.Codigo.val(_fechamentoAgregado.Codigo.val());
    _gridAcrescimo = new GridView(_novoFechamentoContratoFreteAcrescimoDesconto.PesquisarAcrescimo.idGrid, "FechamentoAgregado/PesquisaAcrescimo", _novoFechamentoContratoFreteAcrescimoDesconto, menuOpcoes, null, linhasPorPaginas, null, null, null, null, null, null, configExportacao);
    _gridAcrescimo.SetPermitirRedimencionarColunas(true);
    _gridAcrescimo.CarregarGrid(function () {
        setTimeout(function () {
            if (_fechamentoAgregado.Codigo.val() > 0)
                $("#btnExportarAcrescimo").show();
            else
                $("#btnExportarAcrescimo").hide();
        }, 200);
    });
}

function GridDesconto() {
    let linhasPorPaginas = 5;

    //-- Cabecalho
    let remover = {
        descricao: "Remover",
        id: guid(),
        evento: "onclick",
        metodo: RemoverVinculoContratoFreteAcrescimoDesconto,
        tamanho: "10",
        icone: "",
        visibilidade: VisibilidadeRemoverAcrescimoDesconto
    };

    let menuOpcoes = {
        tipo: TypeOptionMenu.link,
        opcoes: [remover]
    };

    let configExportacao = {
        url: "FechamentoAgregado/ExportarPesquisaDesconto",
        titulo: "Descontos",
        id: "btnExportarDesconto"
    };

    _novoFechamentoContratoFreteAcrescimoDesconto.Codigo.val(_fechamentoAgregado.Codigo.val());
    _gridDesconto = new GridView(_novoFechamentoContratoFreteAcrescimoDesconto.PesquisarDesconto.idGrid, "FechamentoAgregado/PesquisaDesconto", _novoFechamentoContratoFreteAcrescimoDesconto, menuOpcoes, null, linhasPorPaginas, null, null, null, null, null, null, configExportacao);
    _gridDesconto.SetPermitirRedimencionarColunas(true);
    _gridDesconto.CarregarGrid(function () {
        setTimeout(function () {
            if (_fechamentoAgregado.Codigo.val() > 0)
                $("#btnExportarDesconto").show();
            else
                $("#btnExportarDesconto").hide();
        }, 200);
    });
}

function RemoverVinculoContratoFreteAcrescimo(itemGrid) {
    RemoverVinculoContratoFreteAcrescimoDesconto(itemGrid, true);
}

function RemoverVinculoContratoFreteDesconto(itemGrid) {
    RemoverVinculoContratoFreteAcrescimoDesconto(itemGrid, false);
}

function RemoverVinculoContratoFreteAcrescimoDesconto(itemGrid, acrescimo) {

    exibirConfirmacao("Remover Vinculo Contrato Frete", "Deseja realmente remover o vinculo com o contrato de frete do acréscimo/desconto selecionado?", function () {

        executarReST("FechamentoAgregado/RemoverVinculoContratoFreteAcrescimoDesconto", { Codigo: itemGrid.Codigo, CodigoFechamentoAgregado: _fechamentoAgregado.Codigo.val() }, function (arg) {
            if (arg.Success) {
                if (arg.Data) {
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Acréscimo/desconto removido com sucesso!");

                    BuscarFechamentoAgregadoPorCodigo(_fechamentoAgregado.Codigo.val(), false);
                    if (acrescimo) {
                        GridAcrescimo();
                    }
                    else {
                        GridDesconto();
                    }
                } else {
                    exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
            }
        });
    });
}

function VisibilidadeRemoverAcrescimoDesconto(registro) {

    return !_etapa2Consolidacao.Consolidado.val();
}

function AbrirModalAcrescimoDesconto(itemGrid) {
    _novoFechamentoContratoFreteAcrescimoDesconto.CIOT.codEntity(itemGrid.CodigoCIOT);
    _novoFechamentoContratoFreteAcrescimoDesconto.CIOT.val(itemGrid.Descricao);
    _novoFechamentoContratoFreteAcrescimoDesconto.NumeroCIOT.val(itemGrid.NumeroCIOT);
    _novoFechamentoContratoFreteAcrescimoDesconto.Consolidado.val(itemGrid.Consolidado);
    _novoFechamentoContratoFreteAcrescimoDesconto.CodigoTransportadorContratoFreteOrigem.val(itemGrid.CodigoTransportadorContratoFreteOrigem);

    if (_novoFechamentoContratoFreteAcrescimoDesconto.Consolidado.val()) {
        _novoFechamentoContratoFreteAcrescimoDesconto.AdicionarAcrescimo.visible(false);
        _novoFechamentoContratoFreteAcrescimoDesconto.AdicionarDesconto.visible(false);
    }
    else {
        _novoFechamentoContratoFreteAcrescimoDesconto.AdicionarAcrescimo.visible(true);
        _novoFechamentoContratoFreteAcrescimoDesconto.AdicionarDesconto.visible(true);
    }

    GridAcrescimo();
    GridDesconto();

    Global.abrirModal('divFechamentoContratoFreteAcrescimoDesconto');
}