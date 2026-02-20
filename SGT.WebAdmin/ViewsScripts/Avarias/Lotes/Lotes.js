/// <reference path="Integracao.js" />
/// <reference path="Integracao.js" />
/// <reference path="ProdutosAvariados.js" />
/// <reference path="Aceite.js" />
/// <reference path="EtapasLote.js" />
/// <reference path="DescontoAvaria.js" />
/// <reference path="Ocorrencias.js" />
/// <reference path="DestinoAvarias.js" />
/// <reference path="DestinoAvariasProduto.js" />
/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/bootstrap/bootstrap.js" />
/// <reference path="../../../js/libs/jquery.blockui.js" />
/// <reference path="../../../js/Global/knoutViewsSlides.js" />
/// <reference path="../../../js/libs/jquery.maskMoney.js" />
/// <reference path="../../../js/plugin/datatables/jquery.dataTables.js" />
/// <reference path="../../Consultas/Tranportador.js" />
/// <reference path="../../Consultas/MotivoAvaria.js" />
/// <reference path="../../Consultas/Filial.js" />
/// <reference path="../../Enumeradores/EnumSituacaoLote.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _lote;
var _avaria;
var _CRUDLote;
var _pesquisaLote;

var _gridLote = null;
var _gridAvarias = null;

var Lote = function () {
    this.Codigo = PropertyEntity({ type: types.map, getType: typesKnockout.int, val: ko.observable(0), def: 0 });
    this.Situacao = PropertyEntity({ type: types.map, getType: typesKnockout.int, val: ko.observable(0), def: 0 });
    this.CodigoMotivoAvaria = PropertyEntity({ type: types.map, getType: typesKnockout.int, val: ko.observable(0), def: 0 });

    this.NumeroLote = PropertyEntity({ type: types.map, text: "Lote:", val: ko.observable("") });
    this.Transportador = PropertyEntity({ type: types.map, text: "Transportador:", val: ko.observable("") });
    this.Responsavel = PropertyEntity({ type: types.map, text: "Responsável:", val: ko.observable("") });
    this.Criador = PropertyEntity({ type: types.map, text: "Criador:", val: ko.observable("") });
    this.NumeroAvarias = PropertyEntity({ type: types.map, text: "Número de Avarias:", val: ko.observable("") });
    this.SituacaoLote = PropertyEntity({ type: types.map, text: "Situação Lote:", val: ko.observable("") });
    this.MotivoAvaria = PropertyEntity({ type: types.map, text: "Motivo Avaria:", val: ko.observable("") });
    this.ValorLote = PropertyEntity({ type: types.map, text: "Valor Lote:", val: ko.observable("") });
    this.ValorAvarias = PropertyEntity({ type: types.map, text: "Valor Avarias:", val: ko.observable("") });
    this.ValorDescontos = PropertyEntity({ type: types.map, text: "Valor Desconto:", val: ko.observable("") });
};

var Avaria = function () {
    this.Codigo = PropertyEntity({ type: types.map, getType: typesKnockout.int, val: ko.observable(0), def: 0 });
    this.Lote = PropertyEntity({ type: types.map, getType: typesKnockout.int, val: ko.observable(0), def: 0 });

    this.Manutencao = PropertyEntity({ eventClick: manutencaoClick, type: types.event, text: "Manutenção Avarias", visible: ko.observable(false) });

    this.NumeroAvaria = PropertyEntity({ type: types.map, text: "Número da Avaria:", val: ko.observable(""), visible: ko.observable(false) });
    this.Filtrar = PropertyEntity({
        eventClick: function (e) {
            _gridAvarias.CarregarGrid();
        }, type: types.event, text: "Pesquisar", idGrid: guid(), visible: ko.observable(false)
    });

    this.Avarias = PropertyEntity({ type: types.local, text: "Avarias", val: ko.observable(""), idGrid: guid() });
};

var CRUDLote = function () {
    this.Finalizar = PropertyEntity({ eventClick: finalizarClick, type: types.event, text: "Finalizar", visible: ko.observable(false) });
    this.Integrar = PropertyEntity({ eventClick: integrarClick, type: types.event, text: "Integrar", visible: ko.observable(false) });
    this.Corrigir = PropertyEntity({ eventClick: corrigirClick, type: types.event, text: "Corrigir", visible: ko.observable(false) });
    this.Voltar = PropertyEntity({ eventClick: voltarEtapaClick, type: types.event, text: "Voltar Etapa", visible: ko.observable(false) });
};

var PesquisaLote = function () {
    this.DataInicio = PropertyEntity({ text: "Data Inicial: ", getType: typesKnockout.date });
    this.DataFim = PropertyEntity({ text: "Data Final: ", getType: typesKnockout.date });
    this.DataInicio.dateRangeLimit = this.DataFim;
    this.DataFim.dateRangeInit = this.DataInicio;

    this.NumeroLote = PropertyEntity({ type: types.map, text: "Número Lote:", val: ko.observable("") });
    this.Transportadora = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Transportadora:", issue: 69, idBtnSearch: guid() });
    this.Filial = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Filial:", issue: 70, idBtnSearch: guid(), visible: ko.observable(_CONFIGURACAO_TMS.TipoServicoMultisoftware !== EnumTipoServicoMultisoftware.MultiTMS) });
    this.Motivo = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Motivo:", issue: 943, idBtnSearch: guid() });
    this.Situacao = PropertyEntity({ val: ko.observable(EnumSituacaoLote.Todas), options: EnumSituacaoLote.obterOpcoesPesquisa(), def: EnumSituacaoLote.Todas, text: "Situação: " });

    this.Pesquisar = PropertyEntity({
        eventClick: function (e) {
            _gridLote.CarregarGrid();
        }, type: types.event, text: "Pesquisar", idGrid: guid(), visible: ko.observable(true)
    });
    this.ExibirFiltros = PropertyEntity({
        eventClick: function (e) {
            if (e.ExibirFiltros.visibleFade()) {
                e.ExibirFiltros.visibleFade(false);
            } else {
                e.ExibirFiltros.visibleFade(true);
            }
        }, type: types.event, text: "Filtros de Pesquisa", idFade: guid(), visibleFade: ko.observable(true), visible: ko.observable(true)
    });
};

//*******EVENTOS*******

function loadLote() {
    _lote = new Lote();
    KoBindings(_lote, "knockoutDetalhesLotes");

    _avaria = new Avaria();
    KoBindings(_avaria, "knockoutManutencaoLote");

    _CRUDLote = new CRUDLote();
    KoBindings(_CRUDLote, "knockoutCRUD");

    _pesquisaLote = new PesquisaLote();
    KoBindings(_pesquisaLote, "knockoutPesquisaLote");

    loadEtapasLote();
    loadAvarias();
    loadProdutosAvariados();
    loadDescontoAvaria();
    loadAceite();
    loadIntegracao();
    setarEtapaInicioLote();
    LoadConexaoSignalRAvarias();
    loadOcorrenciaLoteAvaria();
    LoadDestinoAvarias();

    HeaderAuditoria("Lote", _lote);

    new BuscarTransportadores(_pesquisaLote.Transportadora);
    new BuscarFilial(_pesquisaLote.Filial);
    new BuscarMotivoAvaria(_pesquisaLote.Motivo, EnumFinalidadeMotivoAvaria.MotivoAvaria);

    buscarLotes();
}

function loadAvarias() {
    var detalhe = {
        descricao: "Detalhes",
        metodo: detalharAvaria,
        id: guid(),
        icone: ""
    };
    var desconto = {
        descricao: "Desconto",
        metodo: descontoAvaria,
        id: guid(),
        icone: ""
    };

    var menuOpcoes = {
        tipo: TypeOptionMenu.list,
        tamanho: 5,
        descricao: "Opções",
        opcoes: [
            detalhe, desconto
        ]
    };

    if (!PodeManutencaoAvaria())
        menuOpcoes = null;

    if (_gridAvarias != null)
        _gridAvarias.Destroy();

    _gridAvarias = new GridView(_avaria.Avarias.idGrid, "Lotes/PesquisaAvarias", _avaria, menuOpcoes);
    _gridAvarias.CarregarGrid();
}

function finalizarClick(e, sender) {
    exibirConfirmacao("Confirmação", "Você realmente deseja finalizar o lote?", function () {
        Salvar(_lote, "Lotes/Finalizar", function (arg) {
            if (arg.Success) {
                if (arg.Data !== false) {
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Lote finalizado com sucesso.");
                    LimparCamposLote();
                    LimparTabelaAvarias();
                    buscarLotes();
                } else {
                    exibirMensagem(tipoMensagem.atencao, "Atenção", arg.Msg);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
            }
        }, sender);
    });
}

function corrigirClick(e, sender) {
    Salvar(_lote, "Lotes/Corrigir", function (arg) {
        if (arg.Success) {
            if (arg.Data !== false) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Lote reaberto para edição.");
                editarLote({ Codigo: _lote.Codigo.val() });
            } else {
                exibirMensagem(tipoMensagem.atencao, "Atenção", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    }, sender);
}

function manutencaoClick(e, sender) {
    ManutencaoLote(_avaria.Lote.val());
}

function descontoAvaria(dataGrid) {
    DescontarDaAvaria(dataGrid);
}

function detalharAvaria(dataGrid) {
    ManutencaoAvaria(dataGrid.Codigo, _lote.Codigo.val());
}

//*******MÉTODOS*******

function buscarLotes() {
    var editar = {
        descricao: "Editar",
        id: "clasEditar",
        evento: "onclick",
        metodo: editarLote,
        tamanho: "15",
        icone: ""
    };

    var menuOpcoes = {
        tipo: TypeOptionMenu.link,
        opcoes: [editar]
    };

    var configExportacao = {
        url: "Lotes/ExportarPesquisa",
        titulo: "Lotes"
    };

    _gridLote = new GridViewExportacao(_pesquisaLote.Pesquisar.idGrid, "Lotes/Pesquisa", _pesquisaLote, menuOpcoes, configExportacao);
    _gridLote.CarregarGrid();
}

function editarLote(dataGrid) {
    LimparCamposLote();

    _lote.Codigo.val(dataGrid.Codigo);
    BuscarLotePorCodigo();

    _pesquisaLote.ExibirFiltros.visibleFade(false);
}

function exibirCamposObrigatorios() {
    exibirMensagem("atencao", "Campo Obrigatório", "Por Favor, Informe os campos obrigatórios");
}

function LimparCamposLote() {
    LimparCampos(_lote);
    LimparCampos(_avaria);

    setarEtapaInicioLote();

    _avaria.Manutencao.visible(false);
    _avaria.NumeroAvaria.visible(false);
    _avaria.Filtrar.visible(false);
    _CRUDLote.Finalizar.visible(false);
    _CRUDLote.Corrigir.visible(false);
    _CRUDLote.Integrar.visible(false);
    _CRUDLote.Voltar.visible(false);

    LimparCamposDestinoAvariasProduto();
}

function ControleDeCampos() {
    var situacao = _lote.Situacao.val();

    if (situacao == EnumSituacaoLote.EmCriacao || situacao == EnumSituacaoLote.EmCorrecao)
        _CRUDLote.Finalizar.visible(true);
    else
        _CRUDLote.Finalizar.visible(false);

    if (situacao == EnumSituacaoLote.Reprovacao)
        _CRUDLote.Corrigir.visible(true);
    else
        _CRUDLote.Corrigir.visible(false);

    if (PodeManutencaoAvaria()) {
        _avaria.Manutencao.visible(true);
        _avaria.NumeroAvaria.visible(true);
        _avaria.Filtrar.visible(true);
    } else {
        _avaria.Manutencao.visible(false);
        _avaria.NumeroAvaria.visible(false);
        _avaria.Filtrar.visible(false);
    }

    // Alterna conforme situacao. Ou mostra a mensagem e esconde as grids ou vice versa
    if (situacao == EnumSituacaoLote.Reprovacao ||
        situacao == EnumSituacaoLote.AgAprovacaoIntegracao ||
        situacao == EnumSituacaoLote.IntegracaoReprovada ||
        situacao == EnumSituacaoLote.EmCorrecao ||
        situacao == EnumSituacaoLote.Finalizada || situacao == EnumSituacaoLote.FinalizadaComDestino ||
        situacao == EnumSituacaoLote.FalhaIntegracao ||
        situacao == EnumSituacaoLote.EmIntegracao
    )
        _aceite.Mensagem.visible(false);
    else if (situacao == EnumSituacaoLote.AgAprovacao)
        _aceite.Mensagem.visible(true);

    if (situacao == EnumSituacaoLote.AgAprovacaoIntegracao) {
        _CRUDLote.Integrar.visible(true);
        _CRUDLote.Voltar.visible(true);
    }
}

function BuscarLotePorCodigo(cb) {
    BuscarPorCodigo(_lote, "Lotes/BuscarPorCodigo", function (arg) {
        _avaria.Lote.val(_lote.Codigo.val());
        setarEtapasAvaria();
        ControleDeCampos();
        loadAvarias();
        AceiteTransportador();
        CarregaIntegracao();

        if (cb != null)
            cb();
    });
}

function AtualizarValoresDoLote() {
    executarReST("Lotes/BuscarPorCodigo", { Codigo: _lote.Codigo.val() }, function (arg) {
        if (arg.Success && arg.Data != null) {
            _lote.ValorLote.val(arg.Data.ValorLote);
            _lote.ValorAvarias.val(arg.Data.ValorAvarias);
            _lote.ValorDescontos.val(arg.Data.ValorDescontos);
        }
    });
}

function PodeManutencaoAvaria() {
    return (_lote.Situacao.val() == EnumSituacaoLote.EmCriacao) || (_lote.Situacao.val() == EnumSituacaoLote.EmCorrecao);
}
function LimparTabelaAvarias() {
    loadAvarias();
}