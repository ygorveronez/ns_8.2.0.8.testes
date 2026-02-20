/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="../../Consultas/Carga.js" />
/// <reference path="../../Consultas/Cliente.js" />
/// <reference path="../../Consultas/TipoOperacao.js" />
/// <reference path="../../Enumeradores/EnumSituacoesCarga.js" />

/*
 * Declaração de Objetos Globais do Arquivo
 */

var _gridRedespacho;
var _redespacho;
var _pesquisaRedespacho;
var _gridPedidos;
var _textoReentrega = _CONFIGURACAO_TMS.NaoExigirExpedidorNoRedespacho ? "/Reentrega" : "";
/*
 * Declaração das Classes
 */

var PesquisaRedespacho = function () {
    this.Carga = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Carga:", idBtnSearch: guid(), visible: ko.observable(true) });
    this.NumeroRedespacho = PropertyEntity({ val: ko.observable(""), visible: ko.observable(true), text: "Número do Redespacho" + _textoReentrega + ": ", def: "", configInt: { precision: 0, allowZero: false, thousands: "" }, getType: typesKnockout.int });
    this.Expedidor = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Expedidor:", idBtnSearch: guid(), enable: ko.observable(true), visible: ko.observable(true) });
    this.TipoOperacao = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Tipo de Operação:", idBtnSearch: guid(), enable: ko.observable(true) });
    this.DataInicio = PropertyEntity({ text: "Data início: ", getType: typesKnockout.date });
    this.DataFim = PropertyEntity({ text: "Data limite: ", dateRangeInit: this.DataInicio, getType: typesKnockout.date });
    this.DataInicio.dateRangeLimit = this.DataFim;
    this.DataFim.dateRangeInit = this.DataInicio;

    this.Pesquisar = PropertyEntity({
        eventClick: function (e) {
            _gridRedespacho.CarregarGrid();
        }, type: types.event, text: "Pesquisar", idGrid: guid(), visible: ko.observable(true)
    });

    this.ExibirFiltros = PropertyEntity({
        eventClick: function (e) {
            if (e.ExibirFiltros.visibleFade()) {
                e.ExibirFiltros.visibleFade(false);
            } else {
                e.ExibirFiltros.visibleFade(true);
            }
        }, type: types.event, text: "Filtros de Pesquisa", idFade: guid(), visibleFade: ko.observable(false), visible: ko.observable(true)
    });
};

var Redespacho = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0 });
    this.Carga = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "*Carga:", idBtnSearch: guid(), visible: ko.observable(true), required: ko.observable(true), enable: ko.observable(true), visibleFade: ko.observable(true) });
    this.Cargas = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: "*Cargas:", idBtnSearch: guid(), visible: ko.observable(true), required: ko.observable(true), enable: ko.observable(true), visibleFade: ko.observable(true) });
    this.DataRedespacho = PropertyEntity({ val: ko.observable(""), visible: ko.observable(true), text: "Data do Redespacho" + _textoReentrega + ": ", issue: 979, def: "", getType: typesKnockout.dateTime, required: false, enable: ko.observable(true) });
    this.NumeroRedespacho = PropertyEntity({ val: ko.observable(""), visible: ko.observable(true), text: "Número: ", def: "", enable: false });
    this.Expedidor = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: _CONFIGURACAO_TMS.NaoExigirExpedidorNoRedespacho ? "Expedidor:" : "*Expedidor:", idBtnSearch: guid(), required: _CONFIGURACAO_TMS.NaoExigirExpedidorNoRedespacho ? false : true, enable: ko.observable(true) });
    this.Recebedor = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Recebedor:", idBtnSearch: guid(), required: false, visible: _CONFIGURACAO_TMS.PermitirInformarRecebedorAoCriarUmRedespachoManual, enable: ko.observable(true) });
    this.TipoOperacao = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "* Tipo de Operação (Carga de Redespacho" + _textoReentrega + "):", idBtnSearch: guid(), required: true, enable: ko.observable(true) });
    this.TipoRedespacho = PropertyEntity({ val: ko.observable(false), options: EnumTipoRedespacho.obterOpcoes(), def: EnumTipoRedespacho.Redespacho, text: "Tipo do Redespacho" + _textoReentrega + ": ", visible: ko.observable(false), enable: ko.observable(true) });
    this.Distancia = PropertyEntity({ getType: typesKnockout.decimal, text: "Distância (Km):", configDecimal: { precision: 4, allowZero: false, allowNegative: false }, enable: ko.observable(true), visible: _CONFIGURACAO_TMS.PermitirInformarDistanciaNoRedespacho });
    this.Pedidos;

    this.SelecionarTodos = PropertyEntity({ val: ko.observable(true), def: true, type: types.map, getType: typesKnockout.bool, text: "Marcar/Desmarcar Todos", visible: ko.observable(true) });

    this.Pesquisar = PropertyEntity({
        eventClick: function (e) {
            buscarPedidos();
        }, type: types.event, text: "Pesquisar", idGrid: guid(), visible: ko.observable(true)
    });


    this.GerarRedespacho = PropertyEntity({ eventClick: gerarRedespachoClick, type: types.event, text: "Gerar Redespacho" + _textoReentrega, visible: ko.observable(false) });
    this.Cancelar = PropertyEntity({ eventClick: cancelarClick, type: types.event, text: "Limpar/Gerar Novo", visible: ko.observable(true) });
};

/*
 * Declaração das Funções de Inicialização
 */

function loadGridPedidosParaRedespacho() {
    $("#tituloPagina").text("Redespacho" + _textoReentrega);
    if (_CONFIGURACAO_TMS.NaoExigirExpedidorNoRedespacho) {
        $("#headReentrega").show();
        $("#headGerarReentrega").show();
    }

    _redespacho.SelecionarTodos.visible(true);
    var multiplaescolha = {
        basicGrid: null,
        eventos: function () { },
        selecionados: new Array(),
        naoSelecionados: new Array(),
        SelecionarTodosKnout: _redespacho.SelecionarTodos,
        somenteLeitura: false,
    };
    _gridPedidos = new GridView(_redespacho.Pesquisar.idGrid, "Redespacho/ConsultarPedidosRedespacho", _redespacho, null, null, null, null, null, null, multiplaescolha);
}

function loadGridRedespacho() {
    var editar = { descricao: "Detalhes", id: "clasEditar", evento: "onclick", metodo: editarRedespacho, tamanho: "15", icone: "" };
    var menuOpcoes = new Object();
    menuOpcoes.tipo = TypeOptionMenu.link;
    menuOpcoes.opcoes = new Array();
    menuOpcoes.opcoes.push(editar);

    _gridRedespacho = new GridView(_pesquisaRedespacho.Pesquisar.idGrid, "Redespacho/Pesquisa", _pesquisaRedespacho, menuOpcoes);
    _gridRedespacho.CarregarGrid();
}

function loadRedespacho() {

    _redespacho = new Redespacho();
    KoBindings(_redespacho, "knockoutCadastroRedespacho");

    _pesquisaRedespacho = new PesquisaRedespacho();
    KoBindings(_pesquisaRedespacho, "knockoutPesquisaRedespacho", false, _pesquisaRedespacho.Pesquisar.id);

    HeaderAuditoria("Redespacho", _redespacho);

    new BuscarCargas(_redespacho.Carga, null, null, null, null, [EnumSituacoesCarga.Cancelada, EnumSituacoesCarga.EmCancelamento, EnumSituacoesCarga.Anulada], null, null, null, !_CONFIGURACAO_TMS.GerarRedespachoDeCargasAgrupadas, null, null, null, null, null, true);
    new BuscarCargas(_redespacho.Cargas, null, null, null, null, [EnumSituacoesCarga.Cancelada, EnumSituacoesCarga.EmCancelamento, EnumSituacoesCarga.Anulada], null, null, null, !_CONFIGURACAO_TMS.GerarRedespachoDeCargasAgrupadas, null, null, null, null, null, true);
    new BuscarTiposOperacao(_redespacho.TipoOperacao, tipoOperacaoRedespachoSelecionado, undefined, undefined, undefined, undefined, undefined, undefined, true);
    _redespacho.TipoOperacao.codEntity.subscribe(tipoOperacaoRedespachoRemovido);
    new BuscarClientes(_redespacho.Expedidor);
    new BuscarClientes(_redespacho.Recebedor);

    new BuscarCargas(_pesquisaRedespacho.Carga, null, null, null, null, null, null, null, null, true);
    new BuscarTiposOperacao(_pesquisaRedespacho.TipoOperacao, undefined, undefined, undefined, undefined, undefined, undefined, undefined, true);
    new BuscarClientes(_pesquisaRedespacho.Expedidor);

    if (_CONFIGURACAO_TMS.TipoServicoMultisoftware === EnumTipoServicoMultisoftware.MultiCTe) {
        _pesquisaRedespacho.Expedidor.visible(false);
        obterExpedidorCarga();
    }

    _redespacho.Carga.visible(!_CONFIGURACAO_TMS.PermiteSelecionarMultiplasCargasParaRedespacho);
    _redespacho.Cargas.visible(_CONFIGURACAO_TMS.PermiteSelecionarMultiplasCargasParaRedespacho);

    _redespacho.Carga.required(!_CONFIGURACAO_TMS.PermiteSelecionarMultiplasCargasParaRedespacho);
    _redespacho.Cargas.required(_CONFIGURACAO_TMS.PermiteSelecionarMultiplasCargasParaRedespacho);

    loadGridRedespacho();
    loadGridPedidosParaRedespacho();

}

/*
 * Declaração das Funções Associadas a Eventos
 */

function obterExpedidorCarga() {
    executarReST("Redespacho/ObterExpedidorCarga", null, function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                var data = arg.Data;
                _redespacho.Expedidor.codEntity(data.CNPJExpedidor);
                _redespacho.Expedidor.val(data.Expedidor);
                if (data.CNPJExpedidor > 0)
                    _redespacho.Expedidor.enable(false);
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    });
}

function cancelarClick(e) {
    limparCamposRedespacho();
    _redespacho.Carga.visibleFade(true);
}

function gerarRedespachoClick(e) {
    if (ValidarCamposObrigatorios(e)) {
        exibirConfirmacao("Confirmação", "Realmente deseja criar a carga de redespacho?", function () {
            var dados = RetornarObjetoPesquisa(_redespacho);
            dados.SelecionarTodos = _redespacho.SelecionarTodos.val();
            dados.DocumentosSelecionadas = JSON.stringify(_gridPedidos.ObterMultiplosSelecionados());
            dados.DocumentosNaoSelecionadas = JSON.stringify(_gridPedidos.ObterMultiplosNaoSelecionados());

            executarReST("Redespacho/GerarRedespacho", dados, function (arg) {
                if (arg.Success) {
                    if (arg.Data) {
                        exibirMensagem(tipoMensagem.ok, "Sucesso", "Criado com sucesso");
                        _gridRedespacho.CarregarGrid();
                        limparCamposRedespacho();
                    } else {
                        exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
                    }
                } else {
                    exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
                }
            });
        });
    } else {
        exibirMensagem(tipoMensagem.atencao, "Campos Obrigatórios", "Favor informar os campos obrigatórios.");
    }
}

/*
 * Declaração das Funções
 */

//function PreencherRedespacho(redespacho) {
//    _redespacho.Carga.val(redespacho.Carga.CodigoCargaEmbarcador);
//    _redespacho.Carga.codEntity(redespacho.Carga.Codigo);

//    _redespacho.Codigo.val(redespacho.Codigo);
//    _redespacho.NumeroRedespacho.val(redespacho.NumeroRedespacho);
//    _redespacho.DataRedespacho.val(redespacho.DataRedespacho);
//    _redespacho.Veiculo.val(redespacho.Veiculo.Descricao);
//    _redespacho.Veiculo.codEntity(redespacho.Veiculo.Codigo);
//    //_redespacho.Veiculo.val(redespacho.Motoristas[0].Descricao);
//    //_redespacho.Veiculo.codEntity(redespacho.Motoristas[0].Codigo);
//    _redespacho.LocalidadeRedespacho.codEntity(redespacho.LocalidadeRedespacho.Codigo);
//    _redespacho.LocalidadeRedespacho.val(redespacho.LocalidadeRedespacho.Descricao);
//    _redespacho.MotivoRedespacho.val(redespacho.MotivoRedespacho);
//    _redespacho.DataRedespacho.enable(false);
//    _redespacho.Veiculo.enable(false);
//    _redespacho.Motorista.enable(false);
//    _redespacho.LocalidadeRedespacho.enable(false);
//    _redespacho.Empresa.enable(false);
//    _redespacho.MotivoRedespacho.enable(false);
//    PreencherGridCTesDoRedespacho();
//    _redespacho.GerarRedespacho.visible(false);

//}

//function PreencherGridCTesDoRedespacho() {
//    _redespacho.SelecionarTodos.visible(false);
//    _gridCTesTransbordados.CarregarGrid(function () {
//        _redespacho.Carga.visibleFade(false);
//    });
//}

function buscarPedidos() {
    if (!ValidarCampoObrigatorioEntity(_redespacho.Carga) || !ValidarCampoObrigatorioEntity(_redespacho.Expedidor)) {
        exibirMensagem(tipoMensagem.atencao, "Campos obrigatórios", "Por favor, informe os campos obrigatórios");
        return;
    }

    _gridPedidos.CarregarGrid(function () {
        if (_gridPedidos.NumeroRegistros() > 0) {
            desabilitarCamposRedespacho();
            _redespacho.TipoOperacao.enable(true);
            _redespacho.TipoRedespacho.enable(true);
            _redespacho.Recebedor.enable(true);
            carregarDadosCarga();
        }
        else
            exibirMensagem(tipoMensagem.atencao, "Sem pedidos", "Não foi localizado nenhum pedido para essa carga e esse expedidor");
    });
}

function carregarDadosCarga() {
    if (!_CONFIGURACAO_TMS.PermitirInformarDistanciaNoRedespacho)
        return;
    _redespacho.Distancia.enable(true);
    executarReST("Redespacho/BuscarDadosCargaPorCodigo", { Codigo: _redespacho.Carga.codEntity() }, function (retorno) {
        if (retorno.Success) {
            if (retorno.Data)
                _redespacho.Distancia.val(retorno.Data.Distancia);
            else
                exibirMensagem(tipoMensagem.aviso, "Aviso", retorno.Msg);
        }
        else
            exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
    });
}

function desabilitarCamposRedespacho() {
    _redespacho.GerarRedespacho.visible(true);
    _redespacho.Pesquisar.visible(false);
    SetarEnableCamposKnockout(_redespacho, false);
}

function editarRedespacho(redespachoGrid) {
    limparCamposRedespacho();
    _redespacho.Codigo.val(redespachoGrid.Codigo);

    BuscarPorCodigo(_redespacho, "Redespacho/BuscarPorCodigo", function (arg) {
        var dados = arg.Data;
        //PreencherRedespacho(arg.Data);
        _redespacho.TipoRedespacho.visible(dados.TipoOperacao.Reentrega || dados.TipoRedespacho == EnumTipoRedespacho.Reentrega);
        _pesquisaRedespacho.ExibirFiltros.visibleFade(false);
        desabilitarCamposRedespacho();
        _redespacho.SelecionarTodos.visible(false);
        _gridPedidos.SetarRegistrosSomenteLeitura(true);
        _gridPedidos.CarregarGrid();

    }, null);
}

function limparCamposRedespacho() {
    LimparCampos(_redespacho);
    SetarEnableCamposKnockout(_redespacho, true);
    _redespacho.GerarRedespacho.visible(true);
    _redespacho.GerarRedespacho.visible(false);
    _redespacho.Pesquisar.visible(true);
    _gridPedidos.SetarRegistrosSomenteLeitura(false);

    if (_CONFIGURACAO_TMS.TipoServicoMultisoftware === EnumTipoServicoMultisoftware.MultiCTe)
        obterExpedidorCarga();

}

function tipoOperacaoRedespachoSelecionado(data) {
    _redespacho.TipoOperacao.val(data.Descricao);
    _redespacho.TipoOperacao.entityDescription(data.Descricao);
    _redespacho.TipoOperacao.codEntity(data.Codigo);
    _redespacho.TipoRedespacho.visible(data.Reentrega);
    if (data.CodigoExpedidor > 0) {
        _redespacho.Expedidor.codEntity(data.CodigoExpedidor);
        _redespacho.Expedidor.val(data.Expedidor);
    }
}

function tipoOperacaoRedespachoRemovido(data) {
    if (data != 0)
        return;

    _redespacho.TipoOperacao.val("");
    _redespacho.TipoOperacao.entityDescription("");
    _redespacho.TipoOperacao.codEntity(0);
    _redespacho.TipoRedespacho.visible(false);
    _redespacho.TipoRedespacho.val(_redespacho.TipoRedespacho.def);
}