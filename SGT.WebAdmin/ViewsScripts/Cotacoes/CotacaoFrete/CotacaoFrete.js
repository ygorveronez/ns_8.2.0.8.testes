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
/// <reference path="../../Consultas/Cliente.js" />
/// <reference path="../../Consultas/Produto.js" />
/// <reference path="../../Consultas/Tranportador.js" />
/// <reference path="../../Consultas/Localidade.js" />
/// <reference path="Transportador.js" />
/// <reference path="Etapa.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _gridCotacaoFrete;
var _pesquisaCotacaoFrete;
var _cotacaoFrete;
var _CRUDCotacaoFrete;
var _opcoesTipoOperacao;

var PesquisaCotacaoFrete = function () {
    this.DataCotacaoInicial = PropertyEntity({ text: "Data Cotação Inicial: ", val: ko.observable(Global.DataAtual()), def: Global.DataAtual(), getType: typesKnockout.date });
    this.DataCotacaoFinal = PropertyEntity({ text: "Data Cotação Final: ", val: ko.observable(""), def: "", getType: typesKnockout.date });
    this.DataCotacaoInicial.dateRangeLimit = this.DataCotacaoFinal;
    this.DataCotacaoFinal.dateRangeInit = this.DataCotacaoInicial;
    this.NumeroPedido = PropertyEntity({ text: "Número Pedido: ", maxlength: 50 });

    this.Expedidor = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Expedidor:", idBtnSearch: guid() });
    this.Destino = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Destino:", idBtnSearch: guid() });
    this.Transportador = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Transportador:", idBtnSearch: guid() });
    this.NumeroCotacao = PropertyEntity({ getType: typesKnockout.int, val: ko.observable(""), def: "", text: "Número cotação:", configInt: { precision: 0, allowZero: true, thousands: "" } });

    this.Pesquisar = PropertyEntity({
        eventClick: function (e) {
            _gridCotacaoFrete.CarregarGrid();
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

    this.BuscaAvancada = PropertyEntity({
        eventClick: function (e) {
            if (e.BuscaAvancada.visibleFade()) {
                e.BuscaAvancada.visibleFade(false);
                e.BuscaAvancada.icon("fal fa-plus");
            } else {
                e.BuscaAvancada.visibleFade(true);
                e.BuscaAvancada.icon("fal fa-minus");
            }
        }, type: types.event, text: "Avançada", idFade: guid(), icon: ko.observable("fal fa-plus"), visibleFade: ko.observable(false), visible: ko.observable(true)
    });
};

var CotacaoFrete = function () {
    this.Codigo = PropertyEntity({ getType: typesKnockout.int, val: ko.observable(0), def: 0 });

    this.Expedidor = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "*Expedidor:", idBtnSearch: guid(), required: true, enable: ko.observable(true) });
    this.Destinatario = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: ko.observable("Destinatário:"), idBtnSearch: guid(), required: ko.observable(false) ,enable: ko.observable(true) });
    this.EnderecoExpedidor = PropertyEntity({ text: "Endereço: " });
    this.CEPDestino = PropertyEntity({ text: ko.observable("CEP de Destino:"), getType: typesKnockout.cep, required: ko.observable(false), enable: ko.observable(true) });
    this.TipoOperacao = PropertyEntity({ text: "*Tipo de Operação:", val: ko.observable(_opcoesTipoOperacao[0].value), options: _opcoesTipoOperacao, def: _opcoesTipoOperacao[0].value, required: ko.observable(true), visible: ko.observable(true) });
    this.ValorNotaFiscal = PropertyEntity({ getType: typesKnockout.decimal, text: "*Valor Nota Fiscal:", val: ko.observable(""), def: "", required: true, enable: ko.observable(true) });

    this.Mercadorias = PropertyEntity({ type: types.listEntity, list: new Array(), codEntity: ko.observable(0), idGrid: guid() });

    this.CEPDestino.val.subscribe(function (novoValor) {
        if (novoValor) {
            _cotacaoFrete.CEPDestino.required(true);
            _cotacaoFrete.CEPDestino.text("*CEP de Destino:");

            _cotacaoFrete.Destinatario.required(false);
            _cotacaoFrete.Destinatario.text("Destinatário:");
        } 
    });

    this.Destinatario.val.subscribe(function (novoValor) {
        if (novoValor) {
            _cotacaoFrete.Destinatario.required(true);
            _cotacaoFrete.Destinatario.text("*Destinatário:");

            _cotacaoFrete.CEPDestino.required(false);
            _cotacaoFrete.CEPDestino.text("CEP de Destino:");
        }
    });
};

var CRUDCotacaoFrete = function () {
    this.Continuar = PropertyEntity({ eventClick: ContinuarClick, type: types.event, text: "Continuar", visible: ko.observable(true) });
    this.Limpar = PropertyEntity({ eventClick: LimparCamposClick, type: types.event, text: "Limpar Campos / Novo", visible: ko.observable(true) });
};

//*******EVENTOS*******

function LoadCotacaoFrete() {
    ObterTiposOperacao().then(function () {
        _cotacaoFrete = new CotacaoFrete();
        KoBindings(_cotacaoFrete, "knockoutCadastroCotacaoFrete");

        _CRUDCotacaoFrete = new CRUDCotacaoFrete();
        KoBindings(_CRUDCotacaoFrete, "knockoutCRUDCotacaoFrete");

        _pesquisaCotacaoFrete = new PesquisaCotacaoFrete();
        KoBindings(_pesquisaCotacaoFrete, "knockoutPesquisaCotacaoFrete", false, _pesquisaCotacaoFrete.Pesquisar.id);

        new BuscarClientes(_cotacaoFrete.Expedidor, retornoExpedido);
        new BuscarClientes(_cotacaoFrete.Destinatario);

        new BuscarClientes(_pesquisaCotacaoFrete.Expedidor);
        new BuscarLocalidades(_pesquisaCotacaoFrete.Destino);
        new BuscarTransportadores(_pesquisaCotacaoFrete.Transportador);

        LoadEtapaCotacaoFrete();
        LoadMercadoria();
        LoadTransportador();
        LoadDetalheCotacaoFreteTransportador();

        BuscarCotacoesFrete();
    });
}

function retornoExpedido(data) {
    _cotacaoFrete.Expedidor.codEntity(data.Codigo);
    _cotacaoFrete.Expedidor.val(data.Descricao);
    _cotacaoFrete.EnderecoExpedidor.val(data.Endereco + ", " + data.Numero + " - CEP: " + data.CEP);
}

function ContinuarClick() {
    Salvar(_cotacaoFrete, "CotacaoFrete/BuscarTransportadores", function (r) {
        if (r.Success) {
            if (r.Data) {
                _transportador.Transportadores.list = r.Data.ListaTransportador;
                RecarregarGridTransportador();

                Etapa2Aguardando();
                $("#" + _etapaCotacaoFrete.Etapa2.idTab).click();
                $("#" + _etapaCotacaoFrete.Etapa2.idTab).tab("show");
            } else {
                exibirMensagem(tipoMensagem.atencao, "Atenção", r.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", r.Msg);
        }
    });
}

function LimparCamposClick() {
    LimparCamposCotacaoFrete();
}

////*******MÉTODOS*******

function ObterTiposOperacao() {
    var p = new promise.Promise();

    executarReST("TipoOperacao/ObterTodos", null, function (r) {
        if (r.Success) {
            _opcoesTipoOperacao = new Array();

            for (var i = 0; i < r.Data.length; i++)
                _opcoesTipoOperacao.push({ value: r.Data[i].Codigo, text: r.Data[i].Descricao });
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha!", r.Msg);
        }

        p.done();
    });

    return p;
};

function BuscarCotacoesFrete() {
    _gridCotacaoFrete = new GridView(_pesquisaCotacaoFrete.Pesquisar.idGrid, "CotacaoFrete/Pesquisa", _pesquisaCotacaoFrete);
    _gridCotacaoFrete.CarregarGrid();
}

function LimparCamposCotacaoFrete() {
    _CRUDCotacaoFrete.Continuar.visible(true);

    SetarEnableCamposKnockout(_cotacaoFrete, true);

    LimparCampos(_cotacaoFrete);
    LimparCamposMercadoria();
    LimparCamposTransportador();

    SetarEtapaInicioCotacaoFrete();
}