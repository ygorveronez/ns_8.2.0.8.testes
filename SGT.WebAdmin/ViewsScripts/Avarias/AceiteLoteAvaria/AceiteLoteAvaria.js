/// <reference path="ProdutosAvariados.js" />
/// <reference path="Anexos.js" />
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
/// <reference path="../../Global/Notificacoes/Notificacao.js" />

//*******MAPEAMENTO KNOUCKOUT*******
var _aceite;
var _pesquisaLote;
var _gridLote;
var _gridAvarias;
var _gridAnexos;

var PesquisaLotes = function () {

    this.NumeroLote = PropertyEntity({ type: types.map, text: "Lote:", val: ko.observable("") });
    this.Pesquisar = PropertyEntity({
        eventClick: function (e) {
            _gridLote.CarregarGrid();
        }, type: types.event, text: "Pesquisar", idGrid: guid(), visible: ko.observable(true)
    });
    this.ExibirFiltros = PropertyEntity({
        eventClick: function (e) {
            if (e.ExibirFiltros.visibleFade() == true) {
                e.ExibirFiltros.visibleFade(false);
            } else {
                e.ExibirFiltros.visibleFade(true);
            }
        }, type: types.event, text: "Pesquisa", idFade: guid(), visibleFade: ko.observable(true), visible: ko.observable(true)
    });
}

var Aceite = function () {
    this.Codigo = PropertyEntity({ type: types.map, getType: typesKnockout.int, val: ko.observable(0), visible: ko.observable(false) });
    this.Lote = PropertyEntity({ type: types.map, getType: typesKnockout.int, val: ko.observable(0) });
    this.Situacao = PropertyEntity({ type: types.map, getType: typesKnockout.int, val: ko.observable(0) });

    // Grid Avarias
    this.Avarias = PropertyEntity({ type: types.local, text: "Avarias", val: ko.observable(""), idGrid: guid() });
    
    // Grid Anexos
    this.Anexos = PropertyEntity({ type: types.local, text: "Anexos", val: ko.observable(""), idGrid: guid() });
    this.AdicionarAnexo = PropertyEntity({ eventClick: gerenciarAnexoClick, type: types.event, text: "Adicionar", visible: ko.observable(true) });

    // Detalhes Lote
    this.Criador = PropertyEntity({ text: "Criador:", val: ko.observable("") });
    this.DataGeracao = PropertyEntity({ text: "Data da Geração:", val: ko.observable("") });
    this.Responsaveis = PropertyEntity({ text: "Responsáveis:", val: ko.observable("") });
    this.NumeroLote = PropertyEntity({ text: "Número Lote:", val: ko.observable("") });
    this.DescricaoSituacao = PropertyEntity({ text: "Situação:", val: ko.observable("") });

    // Resposta Lote
    this.Termos = PropertyEntity({ eventClick: termosClick, type: types.event, text: "Download Termos", visible: ko.observable(true) });
    this.AprovarLote = PropertyEntity({ eventClick: aprovarLoteClick, type: types.event, text: "Aprovar Lote", visible: ko.observable(true) });
    this.ReprovarLote = PropertyEntity({ eventClick: reprovarLoteClick, type: types.event, text: "Reprovar Lote", visible: ko.observable(true) });
    this.Observacao = PropertyEntity({ text: "Observação:", enable: ko.observable(true), visible: ko.observable(true) });
}



//*******EVENTOS*******
function loadAceiteLoteAvaria() {
    _aceite = new Aceite();
    KoBindings(_aceite, "knockoutAceiteLoteAvaria");

    _pesquisaLote = new PesquisaLotes();
    KoBindings(_pesquisaLote, "knockoutPesquisaLotes", false, _pesquisaLote.Pesquisar.id);

    // Grids e modulos
    loadAnexos();
    loadProdutosAvariados();
    GridAvarias();
    GridAnexos();

    // Busca as ocorrencias 
    BuscarLotes();
}

function detalharLote(dataGrid) {
    _aceite.Codigo.val(dataGrid.Codigo);

    BuscaLotePorCodigo();
}

function gerenciarAnexoClick() {
    ModalAnexo();
}

function aprovarLoteClick(e, sender) {
    RespostaLote("Aprova", sender);
}

function reprovarLoteClick(e, sender) {
    RespostaLote("Reprova", sender);
}

function termosClick(e, sender) {
    var data = { Codigo: _aceite.Lote.val() };
    executarDownload("AceiteLoteAvaria/TermoAceite", data);
}

function detalharClick(dataGrid) {
    ManutencaoAvaria(dataGrid.Codigo, _aceite.Lote.val());
}




//*******MÉTODOS*******
function BuscarLotes() {
    //-- Cabecalho
    var detalhes = {
        descricao: "Detalhes",
        id: "clasEditar",
        evento: "onclick",
        metodo: detalharLote,
        tamanho: "10",
        icone: ""
    };

    var menuOpcoes = {
        tipo: TypeOptionMenu.link,
        opcoes: [detalhes]
    };

    _gridLote = new GridView(_pesquisaLote.Pesquisar.idGrid, "AceiteLoteAvaria/Pesquisa", _pesquisaLote, menuOpcoes);
    _gridLote.CarregarGrid();
}

function GridAvarias() {
    // Opcoes
    var detalhes = {
        descricao: "Detalhes",
        id: "clsEditar",
        metodo: detalharClick,
        tamanho: 7,
        icone: ""
    };

    // Menu
    var menuOpcoes = {
        tipo: TypeOptionMenu.link,
        descricao: "Detalhes",
        opcoes: [
            detalhes
        ]
    };

    _gridAvarias = new GridView(_aceite.Avarias.idGrid, "Lotes/PesquisaAvarias", _aceite, menuOpcoes);
}

function GridAnexos() {
    //-- Grid Anexos
    // Opcoes
    var download = { descricao: "Download", id: guid(), metodo: downloadAnexoClick, icone: "" };
    var remover = { descricao: "Remover", id: guid(), metodo: removerAnexoClick, icone: "" };

    // Menu
    var menuOpcoes = { tipo: TypeOptionMenu.list, descricao: "Opções", tamanho: 3, opcoes: [download, remover] };

    _gridAnexos = new GridView(_aceite.Anexos.idGrid, "AceiteLoteAvaria/PesquisaAnexo", _aceite, menuOpcoes);
}

function BuscaLotePorCodigo() {
    BuscarPorCodigo(_aceite, "AceiteLoteAvaria/BuscarPorCodigo", function (arg) {
        if (arg.Success) {
            if (arg.Data !== false) {
                // Replica o valor do lote
                _aceite.Lote.val(_aceite.Codigo.val());
                _anexos.Lote.val(_aceite.Codigo.val());

                // Carrega grid
                _gridAvarias.CarregarGrid();
                _gridAnexos.CarregarGrid();

                // Mostra tela
                _aceite.Codigo.visible(true);

                // Esconde filtro
                _pesquisaLote.ExibirFiltros.visibleFade(false);
            } else {
                exibirMensagem(tipoMensagem.atencao, "Atenção", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    });
}

function RespostaLote(url, sender) {
    // Dados para aprovar/reprovar
    var dados = {
        Lote: _aceite.Lote.val(),
        Observacao: _aceite.Observacao.val()
    };

    // Valida obs
    if (!ValidarCamposObrigatorios(_aceite))
        return exibirMensagem(tipoMensagem.aviso, "Campos Obrigatórios", "Preencha os campos obrigatórios.");
    
    // Efeito de loading
    if (sender != null) {
        var btn = $("#" + sender.currentTarget.id);
        btn.button('loading');
    }

    // Executa requisicao
    executarReST("AceiteLoteAvaria/" + url + "Lote", dados, function (arg) {
        if (sender != null) 
            btn.button('reset');
        
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Lote " + url + "do com sucesso");
                LimparCamposLote();
                _gridLote.CarregarGrid();
            } else {
                exibirMensagem(tipoMensagem.aviso, "Sugestão", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    });
}

function LimparCamposLote() {
    LimparCampos(_aceite);

    // Carrega grid
    _gridAvarias.CarregarGrid();
    _gridAnexos.CarregarGrid();

    // Esconde tela
    _aceite.Codigo.visible(false);

    // Esconde filtro
    _pesquisaLote.ExibirFiltros.visibleFade(true);
}