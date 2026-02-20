/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="../../Enumeradores/EnumCondicaoAutorizaoValorFrete.js" />
/// <reference path="../../Enumeradores/EnumJuncaoAutorizaoTabelaFrete.js" />
/// <reference path="../../Enumeradores/EnumTipoAprovadorRegra.js" />
/// <reference path="../../Enumeradores/EnumTipoAutorizaoTabelaFrete.js" />
/// <reference path="../../Enumeradores/EnumEtapaAutorizacaoTabelaFrete.js" />
/// <reference path="../../Consultas/TabelaFrete.js" />
/// <reference path="../../Consultas/Usuario.js" />
/// <reference path="CRUDRegras.js" />
/// <reference path="Origem.js" />
/// <reference path="MotivoReajuste.js" />
/// <reference path="ValorPedagio.js" />
/// <reference path="ValorFrete.js" />
/// <reference path="Transportador.js" />
/// <reference path="Origem.js" />
/// <reference path="Destino.js" />
/// <reference path="AdValorem.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _gridRegraTabelaFrete;
var _gridAprovadores;
var _regraTabelaFrete;
var _pesquisaRegraTabelaFrete;

var _configRegras = {
    Aprovadores: 3,
    infoTable: "Mova as linhas conforme a prioridade"
};

// Enum...Descricao Apenas retorna a forma descritiva do enumerador
var _condicaoAutorizaoTabelaFreteValor = [
    { text: EnumCondicaoAutorizaoValorFreteDescricao(EnumCondicaoAutorizaoValorFrete.IgualA), value: EnumCondicaoAutorizaoValorFrete.IgualA },
    { text: EnumCondicaoAutorizaoValorFreteDescricao(EnumCondicaoAutorizaoValorFrete.DiferenteDe), value: EnumCondicaoAutorizaoValorFrete.DiferenteDe },
    { text: EnumCondicaoAutorizaoValorFreteDescricao(EnumCondicaoAutorizaoValorFrete.MaiorIgualQue), value: EnumCondicaoAutorizaoValorFrete.MaiorIgualQue },
    { text: EnumCondicaoAutorizaoValorFreteDescricao(EnumCondicaoAutorizaoValorFrete.MaiorQue), value: EnumCondicaoAutorizaoValorFrete.MaiorQue },
    { text: EnumCondicaoAutorizaoValorFreteDescricao(EnumCondicaoAutorizaoValorFrete.MenorIgualQue), value: EnumCondicaoAutorizaoValorFrete.MenorIgualQue },
    { text: EnumCondicaoAutorizaoValorFreteDescricao(EnumCondicaoAutorizaoValorFrete.MenorQue), value: EnumCondicaoAutorizaoValorFrete.MenorQue }
];

var _condicaoAutorizaoTabelaFreteEntidade = [
    { text: EnumCondicaoAutorizaoValorFreteDescricao(EnumCondicaoAutorizaoValorFrete.IgualA), value: EnumCondicaoAutorizaoValorFrete.IgualA },
    { text: EnumCondicaoAutorizaoValorFreteDescricao(EnumCondicaoAutorizaoValorFrete.DiferenteDe), value: EnumCondicaoAutorizaoValorFrete.DiferenteDe }
];

// Enum...Descricao Apenas retorna a forma descritiva do enumerador
var _juncaoAutorizaoTabelaFrete = [
    { text: EnumJuncaoAutorizaoTabelaFreteDescricao(EnumJuncaoAutorizaoTabelaFrete.E), value: EnumJuncaoAutorizaoTabelaFrete.E },
    { text: EnumJuncaoAutorizaoTabelaFreteDescricao(EnumJuncaoAutorizaoTabelaFrete.Ou), value: EnumJuncaoAutorizaoTabelaFrete.Ou }
];

var _tipoAutorizaoTabelaFrete = [
    { text: "Valor Fixo", value: EnumTipoAutorizaoTabelaFrete.ValorFixo },
    { text: "Percentual Reajuste", value: EnumTipoAutorizaoTabelaFrete.Percentual }
];

var PesquisaRegraTabelaFrete = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });

    this.DataInicio = PropertyEntity({ text: "Data inicio: ", getType: typesKnockout.date });
    this.DataFinal = PropertyEntity({ text: "Data limite: ", dateRangeInit: this.DataInicio, getType: typesKnockout.date });
    this.DataInicio.dateRangeLimit = this.DataFinal;
    this.DataFinal.dateRangeInit = this.DataInicio;
    this.Status = PropertyEntity({ text: "Situação: ", val: ko.observable(0), options: _statusPesquisa, def: 0 });

    this.Descricao = PropertyEntity({ text: "Descrição:", val: ko.observable(""), def: "" });
    this.EtapaAutorizacao = PropertyEntity({ text: "Etapa da Autorização: ", val: ko.observable(EnumEtapaAutorizacaoTabelaFrete.Todas), options: EnumEtapaAutorizacaoTabelaFrete.obterOpcoesPesquisa(), def: EnumEtapaAutorizacaoTabelaFrete.Todas });
    this.Aprovador = PropertyEntity({ text: "Aprovador:", type: types.entity, codEntity: ko.observable(0), idBtnSearch: guid() });
    this.TabelaFrete = PropertyEntity({ text: "Tabela de Frete: ", type: types.entity, codEntity: ko.observable(0), idBtnSearch: guid() });

    this.Pesquisar = PropertyEntity({
        eventClick: function (e) {
            _gridRegraTabelaFrete.CarregarGrid();
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

var RegraTabelaFrete = function () {
    var self = this;

    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });

    // Informações da regra
    this.Descricao = PropertyEntity({ text: "*Descrição: ", issue: 586, maxlength: 150, required: true, val: ko.observable("") });
    this.TabelaFrete = PropertyEntity({ text: "Tabela de Frete: ",issue: 78, type: types.entity, codEntity: ko.observable(0), idBtnSearch: guid() });
    this.Vigencia = PropertyEntity({ text: "Vigência: ", issue: 872, getType: typesKnockout.date, val: ko.observable("") });
    this.NumeroAprovadores = PropertyEntity({ text: (_CONFIGURACAO_TMS.ExigeNumeroDeAprovadoresNasAlcadas ? "*Número de Aprovadores: " : "Número de Aprovadores: "), issue: 873, def: 0, getType: typesKnockout.int, configInt: { precision: 0, allowZero: true }, enable: ko.observable(true), required: _CONFIGURACAO_TMS.ExigeNumeroDeAprovadoresNasAlcadas });
    this.Observacao = PropertyEntity({ text: "Observação: ", issue: 593, maxlength: 2000, val: ko.observable("") });
    this.EtapaAutorizacao = PropertyEntity({ text: "Etapa da Autorização: ", issue: 871, val: ko.observable(EnumEtapaAutorizacaoTabelaFrete.AprovacaoReajuste), options: EnumEtapaAutorizacaoTabelaFrete.obterOpcoes(), def: EnumEtapaAutorizacaoTabelaFrete.AprovacaoReajuste });
    this.PrioridadeAprovacao = PropertyEntity({ val: ko.observable(EnumPrioridadeAutorizacao.Zero), options: EnumPrioridadeAutorizacao.obterOpcoes(), def: EnumPrioridadeAutorizacao.Zero, text: "*Prioridade: " });
    this.Status = PropertyEntity({ text: "Situação: ", val: ko.observable(true), options: _status, def: true });
    this.TipoAprovadorRegra = PropertyEntity({ val: ko.observable(EnumTipoAprovadorRegra.Usuario), options: EnumTipoAprovadorRegra.obterOpcoesPorTransportador(), def: EnumTipoAprovadorRegra.Usuario, text: "*Tipo do Aprovador: ", visible: _CONFIGURACAO_TMS.ObrigatorioInformarTransportadorAjusteTabelaFrete || _CONFIGURACAO_TMS.ObrigatorioInformarContratoTransporteFreteAjusteTabelaFrete });
    this.EnviarLinkParaAprovacaoPorEmail = PropertyEntity({ val: ko.observable(false), def: false, text: "Enviar Link para aprovação por e-mail" });

    // Aprovadores
    this.Aprovadores = PropertyEntity({ type: types.map, getType: typesKnockout.dynamic, def: "", val: ko.observable("") });
    this.GridAprovadores = PropertyEntity({ type: types.local, getType: typesKnockout.dynamic, def: new Array(), val: ko.observable(new Array()), required: true, text: "Adicionar", idBtnSearch: guid(), idGrid: guid() });
    this.GridAprovadores.val.subscribe(function () {
        _regraTabelaFrete.Aprovadores.val(JSON.stringify(_regraTabelaFrete.GridAprovadores.val()))
        RenderizarGridAprovadores();
    });

    // Regras
    this.UsarRegraPorMotivoReajuste = PropertyEntity({ getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.RegrasMotivoReajuste = PropertyEntity({ type: types.map, getType: typesKnockout.dynamic, def: new Array(), val: ko.observable(new Array()) });

    this.UsarRegraPorOrigemFrete = PropertyEntity({ getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.RegrasOrigemFrete = PropertyEntity({ type: types.map, getType: typesKnockout.dynamic, def: new Array(), val: ko.observable(new Array()) });

    this.UsarRegraPorDestinoFrete = PropertyEntity({ getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.RegrasDestinoFrete = PropertyEntity({ type: types.map, getType: typesKnockout.dynamic, def: new Array(), val: ko.observable(new Array()) });

    this.UsarRegraPorTransportador = PropertyEntity({ getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.RegrasTransportador = PropertyEntity({ type: types.map, getType: typesKnockout.dynamic, def: new Array(), val: ko.observable(new Array()) });

    this.UsarRegraPorTipoOperacao = PropertyEntity({ getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.RegrasTipoOperacao = PropertyEntity({ type: types.map, getType: typesKnockout.dynamic, def: new Array(), val: ko.observable(new Array()) });

    this.UsarRegraPorValorFrete = PropertyEntity({ getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.RegrasValorFrete = PropertyEntity({ type: types.map, getType: typesKnockout.dynamic, def: new Array(), val: ko.observable(new Array()) });

    this.UsarRegraPorValorPedagio = PropertyEntity({ getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.RegrasValorPedagio = PropertyEntity({ type: types.map, getType: typesKnockout.dynamic, def: new Array(), val: ko.observable(new Array()) });

    this.UsarRegraPorAdValorem = PropertyEntity({ getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.RegrasAdValorem = PropertyEntity({ type: types.map, getType: typesKnockout.dynamic, def: new Array(), val: ko.observable(new Array()) });

    this.UsarRegraPorFilial = PropertyEntity({ getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.RegrasFilial = PropertyEntity({ type: types.map, getType: typesKnockout.dynamic, def: new Array(), val: ko.observable(new Array()) });

    this.TipoAprovadorRegra.val.subscribe(function (novoValor) {
        if (novoValor == EnumTipoAprovadorRegra.Transportador) {
            self.NumeroAprovadores.val("1");
            self.NumeroAprovadores.enable(false);
        }
        else
            self.NumeroAprovadores.enable(true);
    });
}



//*******EVENTOS*******
function loadRegrasAutorizacaoTabelaFrete() {
    _regraTabelaFrete = new RegraTabelaFrete();
    KoBindings(_regraTabelaFrete, "knockoutCadastroRegraTabelaFrete");

    _pesquisaRegraTabelaFrete = new PesquisaRegraTabelaFrete();
    KoBindings(_pesquisaRegraTabelaFrete, "knockoutPesquisaRegraTabelaFrete", false, _pesquisaRegraTabelaFrete.Pesquisar.id);



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
                    RemoverAprovadorClick(_regraTabelaFrete.GridAprovadores, data);
                }
            }
        ]
    };

    // Cabecalho
    var header = [
        { data: "Codigo", visible: false },
        { data: "Nome", title: "Usuário", width: "100%", className: "text-align-left" }
    ];

    // Grid
    _gridAprovadores = new BasicDataTable(_regraTabelaFrete.GridAprovadores.idGrid, header, menuOpcoes, null, null, _configRegras.GridAprovadores);
    _gridAprovadores.CarregarGrid([]);



    //-- Pesquisa
    new BuscarTabelasDeFrete(_regraTabelaFrete.TabelaFrete);
    new BuscarTabelasDeFrete(_pesquisaRegraTabelaFrete.TabelaFrete);
    new BuscarFuncionario(_regraTabelaFrete.GridAprovadores, RetornoInserirAprovador);
    new BuscarFuncionario(_pesquisaRegraTabelaFrete.Aprovador);



    //-- Carrega os loads
    loadCRUDRegras();
    loadMotivoReajuste();
    loadOrigemFrete();
    loadDestinoFrete();
    loadTransportador();
    loadTipoOperacao();
    loadValorFrete();
    loadValorPedagio();
    loadAdValorem();
    loadFilial();



    //-- Busca Regras
    buscarRegrasAutorizacaoTabelaFrete();
}

function buscarRegrasAutorizacaoTabelaFrete() {
    var editar = { descricao: "Editar", id: "clasEditar", evento: "onclick", metodo: editarRegrasTabelaFrete, tamanho: "20", icone: "" };
    var menuOpcoes = new Object();
    menuOpcoes.tipo = TypeOptionMenu.link;
    menuOpcoes.opcoes = new Array();
    menuOpcoes.opcoes.push(editar);


    var configExportacao = {
        url: "RegrasAutorizacaoTabelaFrete/ExportarPesquisa",
        titulo: "Regras Autorização TabelaFretes"
    };

    _gridRegraTabelaFrete = new GridViewExportacao(_pesquisaRegraTabelaFrete.Pesquisar.idGrid, "RegrasAutorizacaoTabelaFrete/Pesquisa", _pesquisaRegraTabelaFrete, menuOpcoes, configExportacao);
    _gridRegraTabelaFrete.CarregarGrid();
}

function editarRegrasTabelaFrete(data) {
    LimparTodosCampos();

    _regraTabelaFrete.Codigo.val(data.Codigo);

    BuscarPorCodigo(_regraTabelaFrete, "RegrasAutorizacaoTabelaFrete/BuscarPorCodigo", function (arg) {
        // Escondo filtros
        _pesquisaRegraTabelaFrete.ExibirFiltros.visibleFade(false);

        // Carrega aprovadores
        _regraTabelaFrete.GridAprovadores.val(arg.Data.Aprovadores);

        // Carrega as regras
        _motivoReajuste.Regras.val(arg.Data.MotivoReajuste);
        _motivoReajuste.UsarRegraPorMotivoReajuste.val(arg.Data.UsarRegraPorMotivoReajuste);

        _origemFrete.Regras.val(arg.Data.OrigemFrete);
        _origemFrete.UsarRegraPorOrigemFrete.val(arg.Data.UsarRegraPorOrigemFrete);

        _destinoFrete.Regras.val(arg.Data.DestinoFrete);
        _destinoFrete.UsarRegraPorDestinoFrete.val(arg.Data.UsarRegraPorDestinoFrete); 

        _transportador.Regras.val(arg.Data.Transportador);
        _transportador.UsarRegraPorTransportador.val(arg.Data.UsarRegraPorTransportador);

        _tipoOperacao.Regras.val(arg.Data.TipoOperacao);
        _tipoOperacao.UsarRegraPorTipoOperacao.val(arg.Data.UsarRegraPorTipoOperacao);

        _valorFrete.Regras.val(arg.Data.ValorFrete);
        _valorFrete.UsarRegraPorValorFrete.val(arg.Data.UsarRegraPorValorFrete);

        _valorPedagio.Regras.val(arg.Data.ValorPedagio);
        _valorPedagio.UsarRegraPorValorPedagio.val(arg.Data.UsarRegraPorValorPedagio);

        _adValorem.Regras.val(arg.Data.AdValorem);
        _adValorem.UsarRegraPorAdValorem.val(arg.Data.UsarRegraPorAdValorem);

        _filial.Regras.val(arg.Data.Filial);
        _filial.UsarRegraPorFilial.val(arg.Data.UsarRegraPorFilial);
        _regraTabelaFrete.EnviarLinkParaAprovacaoPorEmail.val(arg.Data.EnviarLinkParaAprovacaoPorEmail);
        
        // Alterna os botões
        _CRUDRegras.Adicionar.visible(false);
        _CRUDRegras.Cancelar.visible(true);
        _CRUDRegras.Atualizar.visible(true);
        _CRUDRegras.Excluir.visible(true);
    }, null);
}



//*******MÉTODOS*******
function RemoverAprovadorClick(e, sender) {
    // Busca lista de aprovadores
    var aprovadores = _regraTabelaFrete.GridAprovadores.val();

    // Itera lista para remover o aprovador
    for (var i = 0; i < aprovadores.length; i++) {
        if (sender.Codigo == aprovadores[i].Codigo) {
            aprovadores.splice(i, 1);
            break;
        }
    }

    // Salva nova lista
    _regraTabelaFrete.GridAprovadores.val(aprovadores);
}

function RetornoInserirAprovador(data) {
    if (data != null) {
        // Pega registros
        var dataGrid = _regraTabelaFrete.GridAprovadores.val();

        // Objeto aprovador
        var aprovador = {
            Codigo: data.Codigo,
            Nome: data.Nome,
        };

        // Valida se ja nao existe o aprovador
        if (AprovadorJaExiste(dataGrid, aprovador)) {
            exibirMensagem(tipoMensagem.aviso, "Aprovador", "O usuário " + aprovador.Nome + " já consta da lista de aprovadores.");
            return;
        }

        // Adiciona a lista e atualiza a grid
        dataGrid.push(aprovador);
        _regraTabelaFrete.GridAprovadores.val(dataGrid);
    }
}

function AprovadorJaExiste(listaAprovadores, aprovador) {
    // Percorre lista para averiguar duplicidade
    for (var i in listaAprovadores) {
        if (listaAprovadores[i].Codigo == aprovador.Codigo)
            return true;
    }

    return false;
}

function RenderizarGridAprovadores() {
    // Apensa pega os valores
    var aprovadores = _regraTabelaFrete.GridAprovadores.val();

    // E chama o metodo da grid
    _gridAprovadores.CarregarGrid(aprovadores);
}



//*******GLOBAL*******
function EnumCondicaoAutorizaoValorFreteDescricao(valor) {
    switch (valor) {
        case EnumCondicaoAutorizaoValorFrete.IgualA: return "Igual a (==)";
        case EnumCondicaoAutorizaoValorFrete.DiferenteDe: return "Diferente de (!=)";
        case EnumCondicaoAutorizaoValorFrete.MaiorIgualQue: return "Maior ou igual que (>=)";
        case EnumCondicaoAutorizaoValorFrete.MaiorQue: return "Maior que (>)";
        case EnumCondicaoAutorizaoValorFrete.MenorIgualQue: return "Menor ou igual que (<=)";
        case EnumCondicaoAutorizaoValorFrete.MenorQue: return "Menor que (<)";
        default: return "";
    }
}

function EnumJuncaoAutorizaoTabelaFreteDescricao(valor) {
    switch (valor) {
        case EnumJuncaoAutorizaoTabelaFrete.E: return "E (Todas verdadeiras)";
        case EnumJuncaoAutorizaoTabelaFrete.Ou: return "Ou (Apenas uma verdadeira)";
        default: return "";
    }
}

function EnumTipoAutorizaoTabelaFreteDescricao(valor) {
    switch (valor) {
        case EnumTipoAutorizaoTabelaFrete.ValorFixo: return "Valor Fixo";
        case EnumTipoAutorizaoTabelaFrete.Percentual: return "Percentual Reajuste";
        default: return "";
    }
}

function SincronzarRegras() {
    _regraTabelaFrete.UsarRegraPorMotivoReajuste.val(_motivoReajuste.UsarRegraPorMotivoReajuste.val());
    _regraTabelaFrete.RegrasMotivoReajuste.val(JSON.stringify(_motivoReajuste.Regras.val()));

    _regraTabelaFrete.UsarRegraPorOrigemFrete.val(_origemFrete.UsarRegraPorOrigemFrete.val());
    _regraTabelaFrete.RegrasOrigemFrete.val(JSON.stringify(_origemFrete.Regras.val()));

    _regraTabelaFrete.UsarRegraPorDestinoFrete.val(_destinoFrete.UsarRegraPorDestinoFrete.val());
    _regraTabelaFrete.RegrasDestinoFrete.val(JSON.stringify(_destinoFrete.Regras.val()));

    _regraTabelaFrete.UsarRegraPorTransportador.val(_transportador.UsarRegraPorTransportador.val());
    _regraTabelaFrete.RegrasTransportador.val(JSON.stringify(_transportador.Regras.val()));

    _regraTabelaFrete.UsarRegraPorTipoOperacao.val(_tipoOperacao.UsarRegraPorTipoOperacao.val());
    _regraTabelaFrete.RegrasTipoOperacao.val(JSON.stringify(_tipoOperacao.Regras.val()));
    
    _regraTabelaFrete.UsarRegraPorValorFrete.val(_valorFrete.UsarRegraPorValorFrete.val());
    _regraTabelaFrete.RegrasValorFrete.val(JSON.stringify(_valorFrete.Regras.val()));

    _regraTabelaFrete.UsarRegraPorValorPedagio.val(_valorPedagio.UsarRegraPorValorPedagio.val());
    _regraTabelaFrete.RegrasValorPedagio.val(JSON.stringify(_valorPedagio.Regras.val()));

    _regraTabelaFrete.UsarRegraPorAdValorem.val(_adValorem.UsarRegraPorAdValorem.val());
    _regraTabelaFrete.RegrasAdValorem.val(JSON.stringify(_adValorem.Regras.val()));

    _regraTabelaFrete.UsarRegraPorFilial.val(_filial.UsarRegraPorFilial.val());
    _regraTabelaFrete.RegrasFilial.val(JSON.stringify(_filial.Regras.val()));
}

function LimparTodosCampos() {
    LimparCampos(_regraTabelaFrete);
    LimparCampos(_motivoReajuste);
    LimparCampos(_origemFrete);
    LimparCampos(_destinoFrete);
    LimparCampos(_transportador);
    LimparCampos(_tipoOperacao);
    LimparCampos(_valorFrete);
    LimparCampos(_valorPedagio);
    LimparCampos(_adValorem);
    LimparCampos(_filial);
    _regraTabelaFrete.GridAprovadores.val([]);

    $("#myTab li:first a").click();

    _CRUDRegras.Adicionar.visible(true);
    _CRUDRegras.Cancelar.visible(true);
    _CRUDRegras.Atualizar.visible(false);
    _CRUDRegras.Excluir.visible(false);
}

function GeraHeadTable(nomeCampo, campoTipo) {
    if (campoTipo == null)
        campoTipo = false;

    return '<tr>' +
                '<th width="15%" class="text-align-center">Ordem</th>' +
                '<th width="30%" class="text-align-center">Junção</th>' +
                '<th width="30%" class="text-align-center">Condição</th>' +
                (campoTipo ? '<th width="30%" class="text-align-center">Tipo</th>' : "") +
                '<th width="40%" class="text-align-left">' + nomeCampo + '</th>' +
                '<th width="15%" class="text-align-center">Editar</th>' +
            '</tr>';
}

function ObterRegrasOrdenadas(kout) {
    var regras = kout.Regras.val().slice();

    regras.sort(function (a, b) { return a.Ordem - b.Ordem });
    return regras;
}

function LinhasReordenadasTabelaFrete(kout) {
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

function RenderizarGridRegras(kout, grid, fnEditarRegra, usarValor, campoTipo) {
    var html = "";
    var listaRegras = ObterRegrasOrdenadas(kout);
    campoTipo = campoTipo == null ? false : campoTipo;

    $.each(listaRegras, function (i, regra) {
        html += '<tr data-position="' + regra.Ordem + '" data-codigo="' + regra.Codigo + '" id="sort_tipoTabelaFrete_' + regra.Ordem + '"><td>' + regra.Ordem + '</td>';
        html += '<td>' + EnumJuncaoAutorizaoTabelaFreteDescricao(regra.Juncao) + '</td>';
        html += '<td>' + EnumCondicaoAutorizaoValorFreteDescricao(regra.Condicao) + '</td>';
        if (campoTipo)
            html += '<td>' + EnumTipoAutorizaoTabelaFreteDescricao(regra.TipoRegra) + '</td>';
        if (!usarValor)
            html += '<td>' + regra.Entidade.Descricao + '</td>';
        else
            html += '<td>' + Globalize.format(regra.Valor, "n2") + '</td>';
        html += '<td class="text-align-center"><a href="javascript:;" onclick="' + fnEditarRegra + '(\'' + regra.Codigo + '\')">Editar</a></td></tr>';
    });
    grid.RecarregarGrid(html);
}


function ValidarRegraDuplicada(listaRegras, regra, usarValor, campoTipo) {
    // Percorre lista para averiguar duplicidade
    for (var i in listaRegras) {
        if (
            (listaRegras[i].Codigo != regra.Codigo) &&
            (listaRegras[i].Condicao == regra.Condicao) &&
            (listaRegras[i].Juncao == regra.Juncao) &&
            (!campoTipo || (campoTipo && listaRegras[i].TipoRegra == regra.TipoRegra)) &&
            ((!usarValor && listaRegras[i].Entidade.Codigo == regra.Entidade.Codigo) || usarValor && listaRegras[i].Valor == regra.Valor)
           )
            return false;
    }

    return true;
}