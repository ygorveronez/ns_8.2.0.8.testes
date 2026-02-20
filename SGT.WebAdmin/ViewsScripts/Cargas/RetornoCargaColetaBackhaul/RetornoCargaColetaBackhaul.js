/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Globais.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="../../Consultas/Cliente.js" />
/// <reference path="../../Consultas/Empresa.js" />
/// <reference path="../../Consultas/Filial.js" />
/// <reference path="../../Consultas/Localidade.js" />
/// <reference path="../../Consultas/MotivoCancelamentoRetornoCargaColetaBackhaul.js" />
/// <reference path="../../Consultas/Motorista.js" />
/// <reference path="../../Consultas/TipoOperacao.js" />
/// <reference path="../../Consultas/Veiculo.js" />
/// <reference path="../../Enumeradores/EnumSituacaoRetornoCargaColetaBackhaul.js" />

/*
 * Declaração de Objetos Globais do Arquivo
 */

var _cancelamentoRetornoCargaColetaBackhaul;
var _gridRetornoCargaColetaBackhaul;
var _retornoCargaColetaBackhaul;
var _pesquisaRetornoCargaColetaBackhaul;

/*
 * Declaração das Classes
 */

var CancelamentoRetornoCargaColetaBackhaul = function () {
    this.Codigo = PropertyEntity({ getType: typesKnockout.int });
    this.MotivoCancelamento = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), required: true, text: "*Motivo do Cancelamento:", idBtnSearch: guid() });

    this.Confirmar = PropertyEntity({ eventClick: cancelarCargaColetaBackhaulClick, type: types.event, text: "Confirmar", visible: ko.observable(true) });
};

var PesquisaRetornoCargaColetaBackhaul = function () {
    this.CodigoCargaEmbarcador = PropertyEntity({ val: ko.observable(""), def: "", text: "Número da Carga:" });
    this.Remetente = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Remetente:", idBtnSearch: guid(), visible: ko.observable(true), cssClass: ko.observable("col col-xs-12 col-sm-12 col-md-6 col-lg-6") });
    this.Destinatario = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Destinatário:", idBtnSearch: guid() });
    this.Situacao = PropertyEntity({ val: ko.observable(EnumSituacaoRetornoCargaColetaBackhaul.Todas), options: EnumSituacaoRetornoCargaColetaBackhaul.obterOpcoesPesquisa(), def: EnumSituacaoRetornoCargaColetaBackhaul.Todas, getType: typesKnockout.int, text: "Situação: " });
    this.Veiculo = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Veículo:", issue: 143, idBtnSearch: guid() });
    this.Motorista = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Motorista:", issue: 145, idBtnSearch: guid() });
    this.Origem = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Origem:", idBtnSearch: guid() });
    this.Destino = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Destino:", idBtnSearch: guid() });
    this.Filial = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), required: false, text: "*Filial:", issue: 70, idBtnSearch: guid(), visible: ko.observable(true) });
    this.TipoOperacao = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), required: false, text: "Tipo de Operação:", issue: 121, idBtnSearch: guid(), enable: ko.observable(true) });
    this.Empresa = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), required: false, text: "Transportador:", idBtnSearch: guid(), visible: ko.observable(true) });

    this.Pesquisar = PropertyEntity({
        eventClick: function () {
            recarregarRetornoCargaColetaBackhaul()
        }, type: types.event, text: "Pesquisar", idGrid: guid(), visible: ko.observable(true)
    });

    this.ExibirFiltros = PropertyEntity({
        eventClick: function (e) {
            e.ExibirFiltros.visibleFade(!e.ExibirFiltros.visibleFade());
        }, type: types.event, text: "Filtros de Pesquisa", idFade: guid(), visibleFade: ko.observable(true), visible: ko.observable(true)
    });

    this.BuscaAvancada = PropertyEntity({
        eventClick: function (e) {
            if (e.BuscaAvancada.visibleFade()) {
                e.BuscaAvancada.visibleFade(false);
                e.BuscaAvancada.icon("fal fa-plus");
            }
            else {
                e.BuscaAvancada.visibleFade(true);
                e.BuscaAvancada.icon("fal fa-minus");
            }
        }, type: types.event, text: "Avançada", idFade: guid(), icon: ko.observable("fal fa-plus"), visibleFade: ko.observable(false), visible: ko.observable(true)
    });
}

var RetornoCargaColetaBackhaul = function () {
    this.Codigo = PropertyEntity({ getType: typesKnockout.int });
    this.Situacao = PropertyEntity({ val: ko.observable(EnumSituacaoRetornoCargaColetaBackhaul.AguardandoGerarCarga), options: EnumSituacaoRetornoCargaColetaBackhaul.obterOpcoes(), def: EnumSituacaoRetornoCargaColetaBackhaul.AguardandoGerarCarga, getType: typesKnockout.int, text: "Situação: " });

    this.DescricaoSituacao = PropertyEntity({ text: "Situação: " });
    this.Empresa = PropertyEntity({ text: "Transportador: " });
    this.Carga = PropertyEntity({ text: "Numero da Carga: " });
    this.Filial = PropertyEntity({ text: "Filial: " });
    this.Origem = PropertyEntity({ text: "Origem: " });
    this.Destino = PropertyEntity({ text: "Destino: " });
    this.Remetente = PropertyEntity({ text: "Remetente: " });
    this.Destinatarios = PropertyEntity({ text: "Destinatários: " });
    this.Destinatario = PropertyEntity({ text: "Destinatário Final: " });
    this.MotivoCancelamento = PropertyEntity({ text: "Motivo do Cancelamento: " });

    this.CancelarCargaColetaBackhaul = PropertyEntity({ eventClick: cancelarCargaColetaBackhaulModalClick, type: types.event, text: "Cancelar Carga de Coleta", visible: ko.observable(true) });
    this.ConfirmarCargaColetaBackhaul = PropertyEntity({ eventClick: confirmarCargaColetaBackhaulClick, type: types.event, text: "Gerar Carga de Coleta", visible: ko.observable(true) });
};

/*
 * Declaração das Funções de Inicialização
 */

function loadGridRetornoCargaColetaBackhaul() {
    var editar = { descricao: "Detalhes", id: "clasEditar", evento: "onclick", metodo: detalhesRetornoCargaColetaBackhaulClick, tamanho: "15", icone: "" };
    var menuOpcoes = { tipo: TypeOptionMenu.link, opcoes: [editar] };

    _gridRetornoCargaColetaBackhaul = new GridView(_pesquisaRetornoCargaColetaBackhaul.Pesquisar.idGrid, "RetornoCargaColetaBackhaul/Pesquisa", _pesquisaRetornoCargaColetaBackhaul, menuOpcoes, null);
    _gridRetornoCargaColetaBackhaul.CarregarGrid();
}

function loadRetornoCargaColetaBackhaul() {
    _pesquisaRetornoCargaColetaBackhaul = new PesquisaRetornoCargaColetaBackhaul();
    KoBindings(_pesquisaRetornoCargaColetaBackhaul, "knockoutPesquisaRetornoCargaColetaBackhaul", false, _pesquisaRetornoCargaColetaBackhaul.Pesquisar.id);

    _retornoCargaColetaBackhaul = new RetornoCargaColetaBackhaul();
    KoBindings(_retornoCargaColetaBackhaul, "knoutRetornoCargaColetaBackhaul");

    _cancelamentoRetornoCargaColetaBackhaul = new CancelamentoRetornoCargaColetaBackhaul();
    KoBindings(_cancelamentoRetornoCargaColetaBackhaul, "knoutCancelarRetornoCargaColetaBackhaul");

    new BuscarClientes(_pesquisaRetornoCargaColetaBackhaul.Remetente);
    new BuscarClientes(_pesquisaRetornoCargaColetaBackhaul.Destinatario);
    new BuscarEmpresa(_pesquisaRetornoCargaColetaBackhaul.Empresa);
    new BuscarFilial(_pesquisaRetornoCargaColetaBackhaul.Filial);
    new BuscarLocalidades(_pesquisaRetornoCargaColetaBackhaul.Origem);
    new BuscarLocalidades(_pesquisaRetornoCargaColetaBackhaul.Destino);
    new BuscarMotivoCancelamentoRetornoCargaColetaBackhaul(_cancelamentoRetornoCargaColetaBackhaul.MotivoCancelamento);
    new BuscarMotoristas(_pesquisaRetornoCargaColetaBackhaul.Motorista, null, _pesquisaRetornoCargaColetaBackhaul.Empresa);
    new BuscarTiposOperacao(_pesquisaRetornoCargaColetaBackhaul.TipoOperacao);
    new BuscarVeiculos(_pesquisaRetornoCargaColetaBackhaul.Veiculo, null, _pesquisaRetornoCargaColetaBackhaul.Empresa);

    loadGridRetornoCargaColetaBackhaul();
}

/*
 * Declaração das Funções Associadas a Eventos
 */

function cancelarCargaColetaBackhaulClick() {
    if (ValidarCamposObrigatorios(_cancelamentoRetornoCargaColetaBackhaul)) {
        executarReST("RetornoCargaColetaBackhaul/CancelarCargaRetornoColetaBackhaul", RetornarObjetoPesquisa(_cancelamentoRetornoCargaColetaBackhaul), function (retorno) {
            if (retorno.Success) {
                if (retorno.Data) {
                    fecharModalCancelarRetornoCargaColetaBackhaul();
                    fecharModalDetalhesRetornoCargaColetaBackhaul();
                    recarregarRetornoCargaColetaBackhaul();
                }
                else
                    exibirMensagem(tipoMensagem.aviso, "Aviso", retorno.Msg);
            }
            else
                exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
        });
    }
    else
        exibirMensagem("Atencao", "Campos Obrigatórios", "Por Favor, informe os campos obrigatórios");
}

function cancelarCargaColetaBackhaulModalClick() {
    _cancelamentoRetornoCargaColetaBackhaul.Codigo.val(_retornoCargaColetaBackhaul.Codigo.val());

    exibirModalCancelarRetornoCargaColetaBackhaul();
}

function confirmarCargaColetaBackhaulClick() {
    exibirConfirmacao("Confirmação", "Realmente deseja confirmar o retorno de coleta backhaul?", function () {
        executarReST("RetornoCargaColetaBackhaul/GerarCargaRetornoColetaBackhaul", { Codigo: _retornoCargaColetaBackhaul.Codigo.val() }, function (retorno) {
            if (retorno.Success) {
                if (retorno.Data) {
                    fecharModalDetalhesRetornoCargaColetaBackhaul();
                    recarregarRetornoCargaColetaBackhaul();
                }
                else
                    exibirMensagem(tipoMensagem.aviso, "Aviso", retorno.Msg);
            }
            else
                exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
        });
    });
}

function detalhesRetornoCargaColetaBackhaulClick(registroSelecionado) {
    _retornoCargaColetaBackhaul.Codigo.val(registroSelecionado.Codigo);

    BuscarPorCodigo(_retornoCargaColetaBackhaul, "RetornoCargaColetaBackhaul/BuscarPorCodigo", function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                controlarExibicaoBotoesRetornoCargaColetaBackhaul();
                exibirModalDetalhesRetornoCargaColetaBackhaul();
            }
            else
                exibirMensagem(tipoMensagem.aviso, "Aviso", retorno.Msg);
        }
        else
            exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
    });
}

/*
 * Declaração das Funções Privadas
 */

function controlarExibicaoBotoesRetornoCargaColetaBackhaul() {
    var exibirBotoes = _retornoCargaColetaBackhaul.Situacao.val() == EnumSituacaoRetornoCargaColetaBackhaul.AguardandoGerarCarga;

    _retornoCargaColetaBackhaul.CancelarCargaColetaBackhaul.visible(exibirBotoes);
    _retornoCargaColetaBackhaul.ConfirmarCargaColetaBackhaul.visible(exibirBotoes);
}

function exibirModalCancelarRetornoCargaColetaBackhaul() {
    Global.abrirModal('divModalCancelarRetornoCargaColetaBackhaul');
    $("#divModalCancelarRetornoCargaColetaBackhaul").one('hidden.bs.modal', function () {
        LimparCampos(_cancelamentoRetornoCargaColetaBackhaul);
    });
}

function exibirModalDetalhesRetornoCargaColetaBackhaul() {
    Global.abrirModal('divModalDetalhesRetornoCargaColetaBackhaul');
    $("#divModalDetalhesRetornoCargaColetaBackhaul").one('hidden.bs.modal', function () {
        LimparCampos(_retornoCargaColetaBackhaul);
    });
}

function fecharModalCancelarRetornoCargaColetaBackhaul() {
    Global.fecharModal('divModalCancelarRetornoCargaColetaBackhaul');
}

function fecharModalDetalhesRetornoCargaColetaBackhaul() {
    Global.fecharModal('divModalDetalhesRetornoCargaColetaBackhaul');
}

function recarregarRetornoCargaColetaBackhaul() {
    _gridRetornoCargaColetaBackhaul.CarregarGrid();
}