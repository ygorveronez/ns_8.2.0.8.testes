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
/// <reference path="../../Enumeradores/EnumSituacaoPedido.js" />
/// <reference path="Pedido.js" />
/// <reference path="../../Enumeradores/EnumSituacaoSolicitacaoCredito.js" />
/// <reference path="Etapa.js" />

//*******MAPEAMENTO KNOUCKOUT*******


var _integracaoPedido;
var _gridArquivosPedido;
var _gridHistoricoIntegracaoPedido;
var _pesquisaHistoricoIntegracaoPedido;

var IntegracaoPedido = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });

    this.TipoConsultaPedido = PropertyEntity({ val: ko.observable("-1"), options: EnumTipoConsulta.obterOpcoes(), text: Localization.Resources.Pedidos.Pedido.Consulta.getFieldDescription(), def: "-1", enable: ko.observable(true) });
    this.ConsultarPedido = PropertyEntity({ eventClick: ConsultarPedidoClick, type: types.event, text: ko.observable(Localization.Resources.Pedidos.Pedido.Consultar), visible: ko.observable(true), enable: ko.observable(true) });
    this.EnviarPedido = PropertyEntity({ eventClick: EnviarPedidoClick, type: types.event, text: ko.observable(Localization.Resources.Pedidos.Pedido.ReenviarTodos), visible: ko.observable(true), enable: ko.observable(true) });
    this.ArquivosPedido = PropertyEntity({ type: types.map, required: false, text: Localization.Resources.Pedidos.Pedido.PedidoscomS, getType: typesKnockout.dynamic, idBtnSearch: guid(), idGrid: guid() });
    this.TotalPedido = PropertyEntity({ getType: typesKnockout.string, text: Localization.Resources.Pedidos.Pedido.Total.getFieldDescription(), val: ko.observable(""), visible: true });
    this.AguardandoIntegracaoPedido = PropertyEntity({ getType: typesKnockout.string, text: Localization.Resources.Pedidos.Pedido.AgIntegracao.getFieldDescription(), val: ko.observable(""), visible: true });
    this.IntegradoPedido = PropertyEntity({ getType: typesKnockout.string, text: Localization.Resources.Pedidos.Pedido.Integrados.getFieldDescription(), val: ko.observable(""), visible: true });
    this.RejeitadoPedido = PropertyEntity({ getType: typesKnockout.string, text: Localization.Resources.Pedidos.Pedido.Rejeitados.getFieldDescription(), val: ko.observable(""), visible: true });
}


var PesquisaHistoricoIntegracaoPedido = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
};

//*******EVENTOS*******

function loadIntegracaoPedido() {
    _integracaoPedido = new IntegracaoPedido();
    KoBindings(_integracaoPedido, "knockoutIntegracaoEtapa");

    var reenviarPedido = { descricao: Localization.Resources.Gerais.Geral.Reenviar, id: guid(), metodo: ReenviarPedidoClick, icone: "" };
    var historicoIntegracao = { descricao: Localization.Resources.Pedidos.HistoricoEnvio, id: guid(), metodo: ExibirHistoricoIntegracaoPedido, tamanho: "20", icone: "" };
    var menuOpcoesPedido = { tipo: TypeOptionMenu.list, descricao: Localization.Resources.Gerais.Geral.Opcoes, tamanho: 7, opcoes: [reenviarPedido, historicoIntegracao] };

    _gridArquivosPedido = new GridView(_integracaoPedido.ArquivosPedido.idGrid, "PedidoIntegracao/PesquisaIntegracaoPedido", _integracaoPedido, menuOpcoesPedido, null, null, null);
    _gridArquivosPedido.CarregarGrid();
}

function ReenviarPedidoClick(e, sender) {
    exibirConfirmacao(Localization.Resources.Pedidos.Pedido.Atencao, Localization.Resources.Pedidos.Pedido.DesejaRealmenteReenviarEstaIntegracao, function () {
        executarReST("PedidoIntegracao/EnviarLayoutPedido", { Codigo: e.Codigo }, function (arg) {
            if (arg.Success) {
                if (arg.Data) {
                    _gridArquivosPedido.CarregarGrid();
                    CarregarIntegracaoPedido();
                } else {
                    exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.Atencao, arg.Msg);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, arg.Msg);
            }
        });
    });
}

function ConsultarPedidoClick(e, sender) {
    _gridArquivosPedido.CarregarGrid();
    CarregarIntegracaoPedido();
}

function EnviarPedidoClick(e, sender) {
    if (_pedido.Codigo.val() <= 0) {
        exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.Aviso, Localization.Resources.Pedidos.Pedido.PorFavorSelecioneUmPedido);
        return;
    }
    if (_pedido.SituacaoPedido.val() == EnumSituacaoPedido.Cancelado) {
        exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.Aviso, Localization.Resources.Pedidos.Pedido.EstePedidoSeEncontraCancelado);
        return;
    }
    
    if (_pedido.Codigo.val() > 0) {
        var data = {
            Codigo: _pedido.Codigo.val()
        };
        executarReST("PedidoIntegracao/EnviarTodosLayoutPedido", data, function (arg) {
            if (arg.Success) {
                _gridArquivosPedido.CarregarGrid();
                CarregarIntegracaoPedido();
            } else {
                exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, arg.Msg);
            }
        });
    }
}

//*******MÉTODOS*******

function CarregarIntegracaoPedido() {
    if (_pedido.Codigo.val() > 0 && _pedido.Codigo.val() != "") {
        var data =
        {
            CodigoPedido: _pedido.Codigo.val()
        }
        executarReST("PedidoIntegracao/CarregarDadosTotalizadores", data, function (e) {
            if (e.Success) {
                if (e.Data != null) {
                    var dataIntegracao = { Data: e.Data };
                    PreencherObjetoKnout(_integracaoPedido, dataIntegracao);
                    _gridArquivosPedido.CarregarGrid();
                }
            }
            else {
                exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, e.Msg);
            }
        });
    }
}

function LimparInegracaoPedido() {
    LimparCampos(_integracaoPedido);
    _gridArquivosPedido.CarregarGrid();
}

function ExibirHistoricoIntegracaoPedido(integracao) {
    BuscarHistoricoIntegracaoPedido(integracao);
    Global.abrirModal("divModalHistoricoIntegracaoPedido");
}

function BuscarHistoricoIntegracaoPedido(integracao) {
    _pesquisaHistoricoIntegracaoPedido = new PesquisaHistoricoIntegracaoPedido();
    _pesquisaHistoricoIntegracaoPedido.Codigo.val(integracao.Codigo);

    var download = { descricao: Localization.Resources.Gerais.Geral.DownloadArquivos, id: guid(), evento: "onclick", metodo: DownloadArquivosHistoricoIntegracaoPedido, tamanho: "20", icone: "" };

    var menuOpcoes = {
        tipo: TypeOptionMenu.link,
        opcoes: [download]
    };

    _gridHistoricoIntegracaoPedido = new GridView("tblHistoricoIntegracaoPedido", "PedidoIntegracao/ConsultarHistoricoIntegracao", _pesquisaHistoricoIntegracaoPedido, menuOpcoes, { column: 1, dir: orderDir.desc });
    _gridHistoricoIntegracaoPedido.CarregarGrid();
}

function DownloadArquivosHistoricoIntegracaoPedido(historicoConsulta) {
    executarDownload("PedidoIntegracao/DownloadArquivosHistoricoIntegracao", { Codigo: historicoConsulta.Codigo });
}

function obterTiposIntegracao() {
    var p = new promise.Promise();
    var tiposIntegracao = new Array();

    executarReST("TipoIntegracao/BuscarTodos", {
        Tipos: JSON.stringify([
            EnumTipoIntegracao.Isis
        ])
    }, function (r) {
        if (r.Success) {
            for (var i = 0; i < r.Data.length; i++)
                tiposIntegracao.push({ value: r.Data[i].Codigo, text: r.Data[i].Descricao });
        } else {
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, r.Msg);
        }

        p.done(tiposIntegracao);
    });

    return p;
}
