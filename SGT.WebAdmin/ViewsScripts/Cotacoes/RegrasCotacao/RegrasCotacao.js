/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Globais.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="../../Enumeradores/EnumEtapaAutorizacaoOcorrencia.js" />
/// <reference path="../../Consultas/Usuario.js" />
/// <reference path="../../Consultas/TipoOcorrencia.js" />
/// <reference path="../../Consultas/Cliente.js" />
/// <reference path="cepdestino.js" />
/// <reference path="cubagem.js" />
/// <reference path="distancia.js" />
/// <reference path="estadodestino.js" />
/// <reference path="expedidor.js" />
/// <reference path="Destinatario.js" />
/// <reference path="grupoproduto.js" />
/// <reference path="marcaproduto.js" />
/// <reference path="peso.js" />
/// <reference path="produto.js" />
/// <reference path="transportador.js" />
/// <reference path="valormercadoria.js" />
/// <reference path="ValorCotacao.js" />
/// <reference path="../../enumeradores/enumcondicaoautorizaocotacao.js" />
/// <reference path="../../enumeradores/enumcondicaoautorizao.js" />
/// <reference path="../../Enumeradores/EnumAplicacaoRegraCotacao.js" />
/// <reference path="../../Consultas/ModeloVeicularCarga.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _gridRegraCotacao;
var _regraCotacao;
var _pesquisaRegraCotacao;
var _gridTransportadores;

var _configRegras = {
    infoTable: "Mova as linhas conforme a prioridade"
};


// Enum...Descricao Apenas retorna a forma descritiva do enumerador
var _condicaoAutorizaoCotacaoValor = [
    { text: EnumCondicaoAutorizaoCotacaoDescricao(EnumCondicaoAutorizaoCotacao.IgualA), value: EnumCondicaoAutorizaoCotacao.IgualA },
    { text: EnumCondicaoAutorizaoCotacaoDescricao(EnumCondicaoAutorizaoCotacao.DiferenteDe), value: EnumCondicaoAutorizaoCotacao.DiferenteDe },
    { text: EnumCondicaoAutorizaoCotacaoDescricao(EnumCondicaoAutorizaoCotacao.MaiorIgualQue), value: EnumCondicaoAutorizaoCotacao.MaiorIgualQue },
    { text: EnumCondicaoAutorizaoCotacaoDescricao(EnumCondicaoAutorizaoCotacao.MaiorQue), value: EnumCondicaoAutorizaoCotacao.MaiorQue },
    { text: EnumCondicaoAutorizaoCotacaoDescricao(EnumCondicaoAutorizaoCotacao.MenorIgualQue), value: EnumCondicaoAutorizaoCotacao.MenorIgualQue },
    { text: EnumCondicaoAutorizaoCotacaoDescricao(EnumCondicaoAutorizaoCotacao.MenorQue), value: EnumCondicaoAutorizaoCotacao.MenorQue }
];

var _condicaoAutorizaoCotacaoEntidade = [
    { text: EnumCondicaoAutorizaoCotacaoDescricao(EnumCondicaoAutorizaoCotacao.IgualA), value: EnumCondicaoAutorizaoCotacao.IgualA },
    { text: EnumCondicaoAutorizaoCotacaoDescricao(EnumCondicaoAutorizaoCotacao.DiferenteDe), value: EnumCondicaoAutorizaoCotacao.DiferenteDe }
];

// Enum...Descricao Apenas retorna a forma descritiva do enumerador
var _juncaoAutorizaoCotacao = [
    { text: EnumJuncaoAutorizaoCotacaoDescricao(EnumJuncaoAutorizao.E), value: EnumJuncaoAutorizao.E },
    { text: EnumJuncaoAutorizaoCotacaoDescricao(EnumJuncaoAutorizao.Ou), value: EnumJuncaoAutorizao.Ou }
];


var PesquisaRegraCotacao = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });

    this.DataInicio = PropertyEntity({ text: "Data início: ", getType: typesKnockout.date });
    this.DataFinal = PropertyEntity({ text: "Data limite: ", dateRangeInit: this.DataInicio, getType: typesKnockout.date });
    this.DataInicio.dateRangeLimit = this.DataFinal;
    this.DataFinal.dateRangeInit = this.DataInicio;
    this.Ativo = PropertyEntity({ val: ko.observable(true), options: _statusPesquisa, def: true, text: "Situação: " });
    this.Descricao = PropertyEntity({ text: "Descrição:", issue: 586, val: ko.observable(""), def: "" });

    this.Pesquisar = PropertyEntity({
        eventClick: function (e) {
            _gridRegraCotacao.CarregarGrid();
        }, type: types.event, text: "Pesquisar", idGrid: guid(), visible: ko.observable(true)
    });
    this.ExibirFiltros = PropertyEntity({
        eventClick: function (e) {
            if (e.ExibirFiltros.visibleFade() == true) {
                e.ExibirFiltros.visibleFade(false);
            } else {
                e.ExibirFiltros.visibleFade(true);
            }
        }, type: types.event, text: "Filtros de Pesquisa", idFade: guid(), visibleFade: ko.observable(false), visible: ko.observable(true)
    });
}

var RegraCotacao = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });

    // Informações da regra
    this.Descricao = PropertyEntity({ text: "*Descrição: ", issue: 586, maxlength: 150, required: true });
    this.Vigencia = PropertyEntity({ text: "Vigência: ", issue: 872, getType: typesKnockout.date, val: ko.observable("") });
    this.Observacao = PropertyEntity({ text: "Observação: ", issue: 593, maxlength: 2000 });
    this.Ativo = PropertyEntity({ val: ko.observable(true), options: _status, def: true, text: "*Situação: ", issue: 557 });

    this.OpcaoAplicacao = PropertyEntity({ val: ko.observable(EnumAplicacaoRegraCotacao.ExcluirTransportador), options: EnumAplicacaoRegraCotacao.obterOpcoes(), def: EnumAplicacaoRegraCotacao.ExcluirTransportador, text: "Informe a aplicação da regra: ", visible: ko.observable(true) });

    this.Transportadores = PropertyEntity({ type: types.map, getType: typesKnockout.dynamic, def: "", val: ko.observable("") });
    this.GridTransportadores = PropertyEntity({ type: types.local, getType: typesKnockout.dynamic, def: new Array(), val: ko.observable(new Array()), required: true, text: "Adicionar", idBtnSearch: guid(), idGrid: guid() });
    this.GridTransportadores.val.subscribe(function () {
        _regraCotacao.Transportadores.val(JSON.stringify(_regraCotacao.GridTransportadores.val()))
        RenderizarGridTransportadores();
    });

    //this.Transportador = PropertyEntity({ text: "Transportador:", type: types.entity, required: ko.observable(false), codEntity: ko.observable(0), idBtnSearch: guid() });

    this.PrioridadeRegra = PropertyEntity({ text: "Prioridade da Regra:", type: types.map, getType: typesKnockout.int, required: ko.observable(false), def: "" });
    this.PercentualCobranca = PropertyEntity({ text: "Percentual Cobrança:", type: types.map, getType: typesKnockout.decimal, required: ko.observable(false), def: 0.00 });
    this.ValorFixoCotacaoFrete = PropertyEntity({ text: "Valor Fixo Cotação de Frete:", type: types.map, getType: typesKnockout.decimal, required: ko.observable(false), val: ko.observable("0,00"), def: "0,00", configDecimal: { precision: 2, allowZero: true } });
    this.ValorCobranca = PropertyEntity({ text: "Valor Cobrança:", type: types.map, getType: typesKnockout.decimal, required: ko.observable(false), val: ko.observable("0,00"), def: "0,00", configDecimal: { precision: 2, allowZero: true } });
    this.NumeroDiasFrete = PropertyEntity({ text: "Número de Dias:", type: types.map, getType: typesKnockout.int, required: ko.observable(false), def: 0 });    
    this.ModeloVeicularCarga = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "*Modelo Veicular:", idBtnSearch: guid(), visible: ko.observable(true), issue: 44 });

    // Regras
    this.UsarRegraPorPeso = PropertyEntity({ getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.RegrasPeso = PropertyEntity({ type: types.map, getType: typesKnockout.dynamic, def: new Array(), val: ko.observable(new Array()) });

    this.UsarRegraPorDistancia = PropertyEntity({ getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.RegrasDistancia = PropertyEntity({ type: types.map, getType: typesKnockout.dynamic, def: new Array(), val: ko.observable(new Array()) });

    this.UsarRegraPorValorMercadoria = PropertyEntity({ getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.RegrasValorMercadoria = PropertyEntity({ type: types.map, getType: typesKnockout.dynamic, def: new Array(), val: ko.observable(new Array()) });

    this.UsarRegraPorGrupoProduto = PropertyEntity({ getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.RegrasGrupoProduto = PropertyEntity({ type: types.map, getType: typesKnockout.dynamic, def: new Array(), val: ko.observable(new Array()) });

    this.UsarRegraPorEstadoDestino = PropertyEntity({ getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.RegrasEstadoDestino = PropertyEntity({ type: types.map, getType: typesKnockout.dynamic, def: new Array(), val: ko.observable(new Array()) });

    this.UsarRegraPorExpedidor = PropertyEntity({ getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.RegrasExpedidor = PropertyEntity({ type: types.map, getType: typesKnockout.dynamic, def: new Array(), val: ko.observable(new Array()) });

    this.UsarRegraPorDestinatario = PropertyEntity({ getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.RegrasDestinatario = PropertyEntity({ type: types.map, getType: typesKnockout.dynamic, def: new Array(), val: ko.observable(new Array()) });

    this.UsarRegraPorTransportador = PropertyEntity({ getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.RegrasTransportador = PropertyEntity({ type: types.map, getType: typesKnockout.dynamic, def: new Array(), val: ko.observable(new Array()) });

    this.UsarRegraPorProduto = PropertyEntity({ getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.RegrasProduto = PropertyEntity({ type: types.map, getType: typesKnockout.dynamic, def: new Array(), val: ko.observable(new Array()) });

    this.UsarRegraPorCubagem = PropertyEntity({ getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.RegrasCubagem = PropertyEntity({ type: types.map, getType: typesKnockout.dynamic, def: new Array(), val: ko.observable(new Array()) });

    this.UsarRegraPorCepDestino = PropertyEntity({ getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.RegrasCepDestino = PropertyEntity({ type: types.map, getType: typesKnockout.dynamic, def: new Array(), val: ko.observable(new Array()) });

    this.UsarRegraPorMarcaProduto = PropertyEntity({ getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.RegrasMarcaProduto = PropertyEntity({ type: types.map, getType: typesKnockout.dynamic, def: new Array(), val: ko.observable(new Array()) });

    this.UsarRegraPorLinhaSeparacao = PropertyEntity({ getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.RegrasLinhaSeparacao = PropertyEntity({ type: types.map, getType: typesKnockout.dynamic, def: new Array(), val: ko.observable(new Array()) });

    this.UsarRegraPorVolume = PropertyEntity({ getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.RegrasVolume = PropertyEntity({ type: types.map, getType: typesKnockout.dynamic, def: new Array(), val: ko.observable(new Array()) });

    this.UsarRegraPorArestaProduto = PropertyEntity({ getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.RegrasArestaProduto = PropertyEntity({ type: types.map, getType: typesKnockout.dynamic, def: new Array(), val: ko.observable(new Array()) });

    this.UsarRegraPorValorCotacao = PropertyEntity({ getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.RegrasValorCotacao = PropertyEntity({ type: types.map, getType: typesKnockout.dynamic, def: new Array(), val: ko.observable(new Array()) });

}

//*******EVENTOS*******

function loadRegrasCotacao() {
    _regraCotacao = new RegraCotacao();
    KoBindings(_regraCotacao, "knockoutCadastroRegraCotacao");

    _pesquisaRegraCotacao = new PesquisaRegraCotacao();
    KoBindings(_pesquisaRegraCotacao, "knockoutPesquisaRegraCotacao", false, _pesquisaRegraCotacao.Pesquisar.id);

    HeaderAuditoria("RegraCotacao", _regraCotacao);
    new BuscarTransportadores(_regraCotacao.GridTransportadores, RetornoInserirTransportador);
    new BuscarModelosVeicularesCarga(_regraCotacao.ModeloVeicularCarga);

    //-- Grid Aprovadores
    // Menu
    var menuOpcoes = {
        tipo: TypeOptionMenu.link,
        opcoes: [
            {
                descricao: "Excluir",
                id: guid(),
                evento: "onclick",
                tamanho: "15",
                icone: "",
                metodo: function (data) {
                    RemoverTransportadorClick(_regraCotacao.GridTransportadores, data);
                }
            }
        ]
    };

    // Cabecalho
    var header = [
        { data: "Codigo", visible: false },
        { data: "Descricao", title: "Transportador", width: "100%", className: "text-align-left" }
    ];

    // Grid
    _gridTransportadores = new BasicDataTable(_regraCotacao.GridTransportadores.idGrid, header, menuOpcoes, null, null, _configRegras.GridTransportadores);
    _gridTransportadores.CarregarGrid([]);

    //-- Carrega os loads
    loadCRUDRegras();
    loadPeso();
    loadDistancia();
    loadValorMercadoria();
    loadGrupoProduto();
    loadEstadoDestino();
    loadExpedidor();
    loadDestinatario();
    loadTransportador();
    loadProduto();
    loadCubagem();
    loadCepDestino();
    loadMarcaProduto();
    loadVolume();
    loadLinhaSeparacao();
    loadArestaProduto();
    loadValorCotacao();

    //-- Busca Regras
    buscarRegrasAutorizacaoCotacao();
}

function buscarRegrasAutorizacaoCotacao() {
    var editar = { descricao: "Editar", id: guid(), evento: "onclick", metodo: function (data) { editarRegrasCotacao(data, false); }, tamanho: "10", icone: "" };
    var duplicar = { descricao: "Duplicar", id: guid(), evento: "onclick", metodo: function (data) { editarRegrasCotacao(data, true); }, tamanho: "10", icone: "" };

    var menuOpcoes = { tipo: TypeOptionMenu.list, opcoes: [editar, duplicar], tamanho: 10 };

    _gridRegraCotacao = new GridView(_pesquisaRegraCotacao.Pesquisar.idGrid, "RegraCotacao/Pesquisa", _pesquisaRegraCotacao, menuOpcoes);
    _gridRegraCotacao.CarregarGrid();
}


function RemoverTransportadorClick(e, sender) {
    // Busca lista de aprovadores
    var trasportadores = _regraCotacao.GridTransportadores.val();

    // Itera lista para remover o aprovador
    for (var i = 0; i < trasportadores.length; i++) {
        if (sender.Codigo == trasportadores[i].Codigo) {
            trasportadores.splice(i, 1);
            break;
        }
    }

    // Salva nova lista
    _regraCotacao.GridTransportadores.val(trasportadores);
}

function RetornoInserirTransportador(data) {
    if (data != null) {
        // Pega registros
        console.log(data);
        var dataGrid = _regraCotacao.GridTransportadores.val();

        var transportador = {
            Codigo: data.Codigo,
            Descricao: data.Descricao,
        };

        // Valida se ja nao existe o aprovador
        if (TransportadorJaExiste(dataGrid, transportador)) {
            exibirMensagem(tipoMensagem.aviso, "Transportador", "O transportador " + transportador.Descricao + " já consta da lista de transportadores.");
            return;
        }

        // Adiciona a lista e atualiza a grid
        dataGrid.push(transportador);
        _regraCotacao.GridTransportadores.val(dataGrid);
    }
}

function TransportadorJaExiste(listaAprovadores, aprovador) {
    // Percorre lista para averiguar duplicidade
    for (var i in listaAprovadores) {
        if (listaAprovadores[i].Codigo == aprovador.Codigo)
            return true;
    }

    return false;
}

function RenderizarGridTransportadores() {
    var transportadores = _regraCotacao.GridTransportadores.val();

    // E chama o metodo da grid
    _gridTransportadores.CarregarGrid(transportadores);
}

function editarRegrasCotacao(data, duplicar) {
    LimparTodosCampos();

    _regraCotacao.Codigo.val(data.Codigo);

    BuscarPorCodigo(_regraCotacao, "RegraCotacao/BuscarPorCodigo", function (arg) {
        if (duplicar)
            _regraCotacao.Codigo.val(0);

        // Escondo filtros
        _pesquisaRegraCotacao.ExibirFiltros.visibleFade(false);

        // Carrega tranportadores
        _regraCotacao.GridTransportadores.val(arg.Data.Transportadores);

        // Carrega as regras
        _peso.Regras.val(arg.Data.Peso);
        _peso.UsarRegraPorPeso.val(arg.Data.UsarRegraPorPeso);

        _distancia.Regras.val(arg.Data.Distancia);
        _distancia.UsarRegraPorDistancia.val(arg.Data.UsarRegraPorDistancia);

        _valorMercadoria.Regras.val(arg.Data.ValorMercadoria);
        _valorMercadoria.UsarRegraPorValorMercadoria.val(arg.Data.UsarRegraPorValorMercadoria);

        _grupoProduto.Regras.val(arg.Data.GrupoProduto);
        _grupoProduto.UsarRegraPorGrupoProduto.val(arg.Data.UsarRegraPorGrupoProduto);

        _estadoDestino.Regras.val(arg.Data.EstadoDestino);
        _estadoDestino.UsarRegraPorEstadoDestino.val(arg.Data.UsarRegraPorEstadoDestino);

        _expedidor.Regras.val(arg.Data.Expedidor);
        _expedidor.UsarRegraPorExpedidor.val(arg.Data.UsarRegraPorExpedidor);

        _destinatario.Regras.val(arg.Data.Destinatario);
        _destinatario.UsarRegraPorDestinatario.val(arg.Data.UsarRegraPorDestinatario);

        _transportador.Regras.val(arg.Data.Transportador);
        _transportador.UsarRegraPorTransportador.val(arg.Data.UsarRegraPorTransportador);

        _produto.Regras.val(arg.Data.Produto);
        _produto.UsarRegraPorProduto.val(arg.Data.UsarRegraPorProduto);

        _cubagem.Regras.val(arg.Data.Cubagem);
        _cubagem.UsarRegraPorCubagem.val(arg.Data.UsarRegraPorCubagem);

        _cepDestino.Regras.val(arg.Data.CepDestino);
        _cepDestino.UsarRegraPorCepDestino.val(arg.Data.UsarRegraPorCepDestino);

        _marcaProduto.Regras.val(arg.Data.MarcaProduto);
        _marcaProduto.UsarRegraPorMarcaProduto.val(arg.Data.UsarRegraPorMarcaProduto);

        _volume.Regras.val(arg.Data.Volume);
        _volume.UsarRegraPorVolume.val(arg.Data.UsarRegraPorVolume);

        _linhaSeparacao.Regras.val(arg.Data.LinhaSeparacao);
        _linhaSeparacao.UsarRegraPorLinhaSeparacao.val(arg.Data.UsarRegraPorLinhaSeparacao);

        _arestaProduto.Regras.val(arg.Data.ArestaProduto);
        _arestaProduto.UsarRegraPorArestaProduto.val(arg.Data.UsarRegraPorArestaProduto);
        console.log(arg.Data.UsarRegraPorArestaProduto);

        _valorCotacao.Regras.val(arg.Data.ValorCotacao);
        _valorCotacao.UsarRegraPorValorCotacao.val(arg.Data.UsarRegraPorValorCotacao);

        // Alterna os botões
        _CRUDRegras.Adicionar.visible(duplicar);
        _CRUDRegras.Cancelar.visible(!duplicar);
        _CRUDRegras.Atualizar.visible(!duplicar);
        _CRUDRegras.Excluir.visible(!duplicar);
    }, null);
}



//*******GLOBAL*******

function EnumCondicaoAutorizaoCotacaoDescricao(valor) {
    switch (valor) {
        case EnumCondicaoAutorizaoCotacao.IgualA: return "Igual a (==)";
        case EnumCondicaoAutorizaoCotacao.DiferenteDe: return "Diferente de (!=)";
        case EnumCondicaoAutorizaoCotacao.MaiorIgualQue: return "Maior ou igual que (>=)";
        case EnumCondicaoAutorizaoCotacao.MaiorQue: return "Maior que (>)";
        case EnumCondicaoAutorizaoCotacao.MenorIgualQue: return "Menor ou igual que (<=)";
        case EnumCondicaoAutorizaoCotacao.MenorQue: return "Menor que (<)";
        default: return "";
    }
}

function EnumJuncaoAutorizaoCotacaoDescricao(valor) {
    switch (valor) {
        case EnumJuncaoAutorizao.E: return "E (Todas verdadeiras)";
        case EnumJuncaoAutorizao.Ou: return "Ou (Apenas uma verdadeira)";
        default: return "";
    }
}

function SincronzarRegras() {
    _regraCotacao.UsarRegraPorCepDestino.val(_cepDestino.UsarRegraPorCepDestino.val());
    _regraCotacao.RegrasCepDestino.val(JSON.stringify(_cepDestino.Regras.val()));

    _regraCotacao.UsarRegraPorCubagem.val(_cubagem.UsarRegraPorCubagem.val());
    _regraCotacao.RegrasCubagem.val(JSON.stringify(_cubagem.Regras.val()));

    _regraCotacao.UsarRegraPorDistancia.val(_distancia.UsarRegraPorDistancia.val());
    _regraCotacao.RegrasDistancia.val(JSON.stringify(_distancia.Regras.val()));

    _regraCotacao.UsarRegraPorEstadoDestino.val(_estadoDestino.UsarRegraPorEstadoDestino.val());
    _regraCotacao.RegrasEstadoDestino.val(JSON.stringify(_estadoDestino.Regras.val()));

    _regraCotacao.UsarRegraPorExpedidor.val(_expedidor.UsarRegraPorExpedidor.val());
    _regraCotacao.RegrasExpedidor.val(JSON.stringify(_expedidor.Regras.val()));

    _regraCotacao.UsarRegraPorDestinatario.val(_destinatario.UsarRegraPorDestinatario.val());
    _regraCotacao.RegrasDestinatario.val(JSON.stringify(_destinatario.Regras.val()));

    _regraCotacao.UsarRegraPorGrupoProduto.val(_grupoProduto.UsarRegraPorGrupoProduto.val());
    _regraCotacao.RegrasGrupoProduto.val(JSON.stringify(_grupoProduto.Regras.val()));

    _regraCotacao.UsarRegraPorMarcaProduto.val(_marcaProduto.UsarRegraPorMarcaProduto.val());
    _regraCotacao.RegrasMarcaProduto.val(JSON.stringify(_marcaProduto.Regras.val()));

    _regraCotacao.UsarRegraPorPeso.val(_peso.UsarRegraPorPeso.val());
    _regraCotacao.RegrasPeso.val(JSON.stringify(_peso.Regras.val()));

    _regraCotacao.UsarRegraPorProduto.val(_produto.UsarRegraPorProduto.val());
    _regraCotacao.RegrasProduto.val(JSON.stringify(_produto.Regras.val()));

    _regraCotacao.UsarRegraPorTransportador.val(_transportador.UsarRegraPorTransportador.val());
    _regraCotacao.RegrasTransportador.val(JSON.stringify(_transportador.Regras.val()));

    _regraCotacao.UsarRegraPorLinhaSeparacao.val(_linhaSeparacao.UsarRegraPorLinhaSeparacao.val());
    _regraCotacao.RegrasLinhaSeparacao.val(JSON.stringify(_linhaSeparacao.Regras.val()));

    _regraCotacao.UsarRegraPorVolume.val(_volume.UsarRegraPorVolume.val());
    _regraCotacao.RegrasVolume.val(JSON.stringify(_volume.Regras.val()));

    _regraCotacao.UsarRegraPorValorMercadoria.val(_valorMercadoria.UsarRegraPorValorMercadoria.val());
    _regraCotacao.RegrasValorMercadoria.val(JSON.stringify(_valorMercadoria.Regras.val()));

    _regraCotacao.UsarRegraPorArestaProduto.val(_arestaProduto.UsarRegraPorArestaProduto.val());
    _regraCotacao.RegrasArestaProduto.val(JSON.stringify(_arestaProduto.Regras.val()));

    _regraCotacao.UsarRegraPorValorCotacao.val(_valorCotacao.UsarRegraPorValorCotacao.val());
    _regraCotacao.RegrasValorCotacao.val(JSON.stringify(_valorCotacao.Regras.val()));

}

function LimparTodosCampos() {
    LimparCampos(_regraCotacao);
    LimparCampos(_peso);
    LimparCampos(_distancia);
    LimparCampos(_valorMercadoria);
    LimparCampos(_estadoDestino);
    LimparCampos(_expedidor);
    LimparCampos(_destinatario);
    LimparCampos(_transportador);
    LimparCampos(_produto);
    LimparCampos(_cubagem);
    LimparCampos(_cepDestino);
    LimparCampos(_marcaProduto);
    LimparCampos(_volume);
    LimparCampos(_linhaSeparacao);
    LimparCampos(_grupoProduto);
    LimparCampos(_arestaProduto);
    LimparCampos(_valorCotacao);
    _regraCotacao.GridTransportadores.val([]);

    $("#myTab li:first a").click();

    _CRUDRegras.Adicionar.visible(true);
    _CRUDRegras.Cancelar.visible(true);
    _CRUDRegras.Atualizar.visible(false);
    _CRUDRegras.Excluir.visible(false);
}

function GeraHeadTable(nomeCampo) {
    return '<tr>' +
        '<th width="15%" class="text-align-center">Ordem</th>' +
        '<th width="30%" class="text-align-center">Junção</th>' +
        '<th width="30%" class="text-align-center">Condição</th>' +
        '<th width="40%" class="text-align-left">' + nomeCampo + '</th>' +
        '<th width="15%" class="text-align-center">Editar</th>' +
        '</tr>';
}

function GeraHeadTableTransportadores(nomeCampo) {
    return '<tr>' +
        '<th width="40%" class="text-align-left">' + nomeCampo + '</th>' +
        '<th width="15%" class="text-align-center">Editar</th>' +
        '</tr>';
}

function ObterRegrasOrdenadas(kout) {
    var regras = kout.Regras.val().slice();

    regras.sort(function (a, b) { return a.Ordem - b.Ordem });
    return regras;
}

function ObterTransportadoresOrdenados(knockout) {
    var transp = knockout.Transportadores.val().slice();

    transp.sort(function (a, b) { return a.Codigo - b.Codigo });
    return transp;
}

function LinhasReordenadasCotacao(kout) {
    var listaRegrasAtualizada = [];
    var listaRegras = kout.Regras.val();

    var BuscaRegraPorCodigo = function (codigo) {
        for (var i in listaRegras)
            if (listaRegras[i].Codigo == codigo)
                return listaRegras[i];

        return null;
    }

    $("#" + kout.Regras.idGrid + " table tbody tr").each(function (i) {
        var regra = BuscaRegraPorCodigo($(this).data('codigo'));
        regra.Ordem = i + 1;
        listaRegrasAtualizada.push(regra);
    });

    kout.Regras.val(listaRegrasAtualizada);
}


function LinhasReordenadasTransportador(kout) {
    var listaRegrasAtualizada = [];
    var listaRegras = kout.Transportadores.val();

    var BuscaRegraPorCodigo = function (codigo) {
        for (var i in listaRegras)
            if (listaRegras[i].Codigo == codigo)
                return listaRegras[i];

        return null;
    }

    $("#" + kout.Transportadores.idGrid + " table tbody tr").each(function (i) {
        var transp = BuscaRegraPorCodigo($(this).data('codigo'));
        listaRegrasAtualizada.push(transp);
    });

    kout.Transportadores.val(listaRegrasAtualizada);
}

function RenderizarGridRegras(kout, grid, fnEditarRegra, tipo) {
    var html = "";
    var listaRegras = ObterRegrasOrdenadas(kout)

    $.each(listaRegras, function (i, regra) {
        html += '<tr data-position="' + regra.Ordem + '" data-codigo="' + regra.Codigo + '" id="sort_tipoOcorrencia_' + regra.Ordem + '"><td>' + regra.Ordem + '</td>';
        html += '<td>' + EnumJuncaoAutorizaoCotacaoDescricao(regra.Juncao) + '</td>';
        html += '<td>' + EnumCondicaoAutorizaoCotacaoDescricao(regra.Condicao) + '</td>';
        if (!tipo)
            html += '<td>' + regra.Entidade.Descricao + '</td>';
        else if (tipo == typesKnockout.decimal)
            html += '<td>' + Globalize.format(regra.Valor, "n2") + '</td>';
        else
            html += '<td>' + regra.Valor + '</td>';

        html += '<td class="text-align-center"><a href="javascript:;" onclick="' + fnEditarRegra + '(\'' + regra.Codigo + '\')">Editar</a></td></tr>';
    });
    grid.RecarregarGrid(html);
}

//function RenderizarGridTransportadores(kout, grid, fnEditarRegra) {
//    var html = "";
//    var listaTransportadores = ObterTransportadoresOrdenados(kout)

//    $.each(listaTransportadores, function (i, transportador) {
//        html += '<tr data-position="' + transportador.Codigo + '" data-codigo="' + transportador.Codigo + '" id="sort_transportador_' + transportador.Codigo + '">';
//        html += '<td>' + transportador.Descricao + '</td>';

//        html += '<td class="text-align-center"><a href="javascript:;" onclick="' + fnEditarRegra + '(\'' + transportador.Codigo + '\')">Editar</a></td></tr>';
//    });
//    grid.RecarregarGrid(html);
//}

function ValidarRegraDuplicada(listaRegras, regra, usarValor) {
    // Percorre lista para averiguar duplicidade
    for (var i in listaRegras) {
        if (
            (listaRegras[i].Codigo != regra.Codigo) &&
            (listaRegras[i].Condicao == regra.Condicao) &&
            (listaRegras[i].Juncao == regra.Juncao) &&
            ((!usarValor && listaRegras[i].Entidade.Codigo == regra.Entidade.Codigo) || usarValor && listaRegras[i].Valor == regra.Valor)
        )
            return false;
    }

    return true;
}

function ValidarTransportadorDuplicado(listaTransportador, transportador) {
    // Percorre lista para averiguar duplicidade
    for (var i in listaTransportador) {
        if (listaTransportador[i].Codigo == transportador.Codigo)
            return false;
    }

    return true;
}
