/// <reference path="../../Cargas/ControleEntrega/FimViagem.js" />

var _dadosPedido;
var _fimViagem;

var DadosPedido = function (id) {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.CodigoCarga = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Descricao = PropertyEntity({ text: Localization.Resources.Gerais.Geral.Descricao.getFieldDescription(), required: true, getType: typesKnockout.string, val: ko.observable("") });
    this.NumeroPedidoEmbarcador = PropertyEntity({ text: Localization.Resources.Gerais.Geral.Numero.getFieldDescription() });
    this.Remetente = PropertyEntity({ text: Localization.Resources.Gerais.Geral.Remetente.getFieldDescription() });
    this.Destinatario = PropertyEntity({ text: Localization.Resources.Gerais.Geral.Destinatario.getFieldDescription() });
    this.Origem = PropertyEntity({ text: Localization.Resources.Gerais.Geral.Origem.getFieldDescription() });
    this.Destino = PropertyEntity({ text: Localization.Resources.Gerais.Geral.Destino.getFieldDescription() });
    this.DataCarregamentoPedido = PropertyEntity({ text: Localization.Resources.Cargas.ControleEntrega.DataCarregamento.getFieldDescription() });
    this.DadosPedido = PropertyEntity({ text: Localization.Resources.Cargas.ControleEntrega.DadosPedido });
    this.NumeroCarga = PropertyEntity({ text: Localization.Resources.Cargas.ControleEntrega.NumeroCarga.getFieldDescription() });
    this.Motorista = PropertyEntity({ text: Localization.Resources.Gerais.Geral.Motorista.getFieldDescription() });
    this.Veiculo = PropertyEntity({ text: Localization.Resources.Gerais.Geral.Veiculo.getFieldDescription() });
    this.NumerosNotas = PropertyEntity({ text: Localization.Resources.Cargas.ControleEntrega.NotasFiscais.getFieldDescription() });
    this.Situacao = PropertyEntity({ text: Localization.Resources.Gerais.Geral.Situacao.getFieldDescription() });
    this.PesoTotal = PropertyEntity({ text: Localization.Resources.Cargas.ControleEntrega.PesoTotal.getFieldDescription() });
    this.PrevisaoEntrega = PropertyEntity({ text: Localization.Resources.Cargas.ControleEntrega.DataAgenda.getFieldDescription() });
    this.URLRastreamentoEntrega = PropertyEntity({ text: Localization.Resources.Cargas.ControleEntrega.URLRastreamentoEntrega.getFieldDescription() });
    this.knockoutId = PropertyEntity({ val: "knockout-dados-pedido-" + id });
    this.ResumoEstadia = PropertyEntity({ eventClick: exibirAcompanhamentoPedidoModalClick, type: types.event, text: Localization.Resources.Cargas.ControleEntrega.ResumoEstadia.getFieldDescription(), visible: ko.observable(true), idGrid: guid() });
}


/*
 * Declaração das Funções Associadas a Eventos
 */

function carregarHTMLComponenteAcompanhamentoPedido(callback) {
    $.get('Content/Static/Pedido/AcompanhamentoPedido/ComponenteAcompanhamentoPedido.html?dyn=' + guid(), function (html) {
        $('#ComponenteAcompanhamentoPedidoContent').html(html);
        callback();

    })
}

function RegistraComponenteAcompanhamentoPedido() {
    if (ko.components.isRegistered('acompanhamento-pedido'))
        return;

    ko.components.register('acompanhamento-pedido', {
        viewModel: EtapaAcompanhamentoPedido,
        template: {
            element: 'acompanhamento-pedido-templete'
        }
    });
}

function loadDadosPedido(e) {
    let knockout = "knockout-dados-pedido-" + e.EtapaPedido.idTab;

    if (!findListaKnockout(knockout)) {
        _dadosPedido = new DadosPedido(e.EtapaPedido.idTab);
        KoBindings(_dadosPedido, knockout);
    }
}

/*
 * Declaração das Funções
 */
function exibirDadosEtapaPedido(e) {
    loadDadosPedido(e);
    fecharDados();
    loadFimViagem(true)

    executarReST("AcompanhamentoPedido/ObterDadosPedido", { CodigoPedido: e.CodigoPedido.val() }, function (arg) {
        if (arg.Success) {
            if (arg.Data !== false) {

                PreencherObjetoKnout(_dadosPedido, { Data: arg.Data });

            } else {
                exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Atencao, arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, arg.Msg);
        }
    });

    $("#" + e.EtapaPedido.idTab).addClass("active show");
}

function exibirAcompanhamentoPedidoModalClick() {
    executarReST("ControleEntregaFimViagem/BuscarPorCarga", { Carga: _dadosPedido.CodigoCarga.val() }, function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                PreencherObjetoKnout(_fimViagem, arg);

                exibeModalEtapa('#divAcompanhamentoPedidoFimViagem');
                _gridResumoRoteiro.CarregarGrid();

            } else {
                exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.Atencao, arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, arg.Msg);
        }
    });
}