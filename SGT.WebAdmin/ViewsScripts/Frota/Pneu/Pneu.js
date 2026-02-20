/// <reference path="../../Consultas/Almoxarifado.js" />
/// <reference path="../../Consultas/BandaRodagemPneu.js" />
/// <reference path="../../Consultas/ModeloPneu.js" />
/// <reference path="../../Consultas/Produto.js" />
/// <reference path="../../Enumeradores/EnumTipoAquisicaoPneu.js" />
/// <reference path="../../Enumeradores/EnumVidaPneu.js" />
/// <reference path="../../Enumeradores/EnumSituacaoPneuTMS.js" />
/// <reference path="../../Enumeradores/EnumCodigoControleImportacao.js" />
/// <reference path="../PneuLote/PneuLote.js" />
/// <reference path="PneuImportar.js" />

/*
 * Declaração de Objetos Globais do Arquivo
 */

var _CRUDPneu;
var _pneu;
var _pesquisaPneu;
var _gridPneu;

/*
 * Declaração das Classes
 */

var Pneu = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Situacao = PropertyEntity({ text: "Situacao: ", val: ko.observable(EnumSituacaoPneuTMS.Todos), options: EnumSituacaoPneuTMS.obterOpcoes(), def: EnumSituacaoPneuTMS.Todos, visible: ko.observable(false) });
    this.Almoxarifado = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "*Almoxarifado:", idBtnSearch: guid(), required: true });
    this.BandaRodagem = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "*Banda de Rodagem:", idBtnSearch: guid(), required: true });
    this.DataEntrada = PropertyEntity({ text: "*Data de Entrada: ", getType: typesKnockout.date, required: true });
    this.DescricaoNota = PropertyEntity({ text: "Descrição da Nota:", getType: typesKnockout.string, val: ko.observable(""), maxlength: 500 });
    this.DocumentoEntradaItem = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Item do Documento de Entrada:", idBtnSearch: guid(), enable: false });
    this.DTO = PropertyEntity({ text: "DTO:", getType: typesKnockout.string, val: ko.observable(""), maxlength: 500 });
    this.KmAtualRodado = PropertyEntity({ text: "Km Atual Rodado:", getType: typesKnockout.int, maxlength: 9, configInt: { precision: 0, allowZero: false, thousands: "." }, required: false });
    this.KmRodadoEntreSulcos = PropertyEntity({ text: "Km Rodado Entre Sulcos:", getType: typesKnockout.int, maxlength: 9, configInt: { precision: 0, allowZero: false, thousands: "." } });
    this.KmRodadoPorSulco = PropertyEntity({ text: "Km Rodado por Sulco:", getType: typesKnockout.decimal, maxlength: 9, configDecimal: { precision: 2, allowZero: false }, enable: false });
    this.Modelo = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "*Modelo:", idBtnSearch: guid(), required: true });
    this.NumeroFogo = PropertyEntity({ text: "*Número de Fogo:", getType: typesKnockout.string, val: ko.observable(""), maxlength: 500, required: true });
    this.Produto = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "*Produto:", idBtnSearch: guid(), required: true });
    this.Sulco = PropertyEntity({ text: "Sulco:", getType: typesKnockout.decimal, configDecimal: { precision: 2, allowZero: false }, maxlength: 10 });
    this.SulcoAnterior = PropertyEntity({ text: "Sulco Anterior:", getType: typesKnockout.decimal, configDecimal: { precision: 2, allowZero: false }, maxlength: 10, });
    this.SulcoGasto = PropertyEntity({ text: "Sulco Gasto:", getType: typesKnockout.decimal, configDecimal: { precision: 2, allowZero: false }, maxlength: 10, enable: false });
    this.TipoAquisicao = PropertyEntity({ text: "*Tipo Aquisição: ", val: ko.observable(EnumTipoAquisicaoPneu.PneuNovoReposicao), options: EnumTipoAquisicaoPneu.obterOpcoes(), def: EnumTipoAquisicaoPneu.PneuNovoReposicao });
    this.ValorAquisicao = PropertyEntity({ text: "*Valor de Aquisição:", getType: typesKnockout.decimal, configDecimal: { precision: 2, allowZero: false }, maxlength: 10, enable: ko.observable(true), required: true });
    this.ValorCustoAtualizado = PropertyEntity({ text: "Custo Atualizado:", getType: typesKnockout.decimal, configDecimal: { precision: 2, allowZero: false }, maxlength: 10, enable: false });
    this.ValorCustoKmAtualizado = PropertyEntity({ text: "Custo Km Atualizado:", getType: typesKnockout.decimal, configDecimal: { precision: 2, allowZero: false }, maxlength: 10, enable: false });
    this.VidaAtual = PropertyEntity({ text: "*Vida Atual: ", val: ko.observable(EnumVidaPneu.PneuNovo), options: EnumVidaPneu.obterOpcoes(), def: EnumVidaPneu.PneuNovo });
    this.Calibragem = PropertyEntity({ text: "Calibragem:", getType: typesKnockout.int});
    this.Milimitragem1 = PropertyEntity({ text: "Milimitragem 1:", getType: typesKnockout.decimal, configDecimal: { precision: 2, allowZero: false }, maxlength: 10});
    this.Milimitragem2 = PropertyEntity({ text: "Milimitragem 2:", getType: typesKnockout.decimal, configDecimal: { precision: 2, allowZero: false }, maxlength: 10});
    this.Milimitragem3 = PropertyEntity({ text: "Milimitragem 3:", getType: typesKnockout.decimal, configDecimal: { precision: 2, allowZero: false }, maxlength: 10});
    this.Milimitragem4 = PropertyEntity({ text: "Milimitragem 4:", getType: typesKnockout.decimal, configDecimal: { precision: 2, allowZero: false }, maxlength: 10});


    this.KmRodadoEntreSulcos.val.subscribe(AtualizarDadosSulcos);
    this.Sulco.val.subscribe(AtualizarDadosSulcos);
    this.SulcoAnterior.val.subscribe(AtualizarDadosSulcos);
    this.ValorAquisicao.val.subscribe(atualizarValoresCusto);

    // Upload de arquivo
    this.PercentualProcessado = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int, visible: ko.observable(false) });
    this.MensagemFalhaGeracao = PropertyEntity({ visible: ko.observable(false) });
    this.Arquivo = PropertyEntity({ type: types.file, codEntity: ko.observable(0), text: "Arquivo:", val: ko.observable(""), visible: ko.observable(true) });
    this.Enviar = PropertyEntity({ eventClick: importarClick, type: types.event, text: "Importar", visible: ko.observable(true) });
};

var CRUDPneu = function () {
    this.Adicionar = PropertyEntity({ eventClick: adicionarClick, type: types.event, text: "Adicionar", visible: ko.observable(true) });
    this.Atualizar = PropertyEntity({ eventClick: atualizarClick, type: types.event, text: "Atualizar", visible: ko.observable(false) });
    this.Cancelar = PropertyEntity({ eventClick: cancelarClick, type: types.event, text: "Cancelar", visible: ko.observable(false) });
    this.Excluir = PropertyEntity({ eventClick: excluirClick, type: types.event, text: "Excluir", visible: ko.observable(false) });

    this.RetornarSucata = PropertyEntity({ eventClick: RetornarSucataClick, type: types.event, text: "Retornar da Sucata", visible: ko.observable(false) });
    this.ImportarLote = PropertyEntity({ eventClick: importarPneuLoteClick, type: types.event, text: "Inserir Lote", visible: ko.observable(true) });
};

var PesquisaPneu = function () {
    this.Almoxarifado = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Almoxarifado:", idBtnSearch: guid() });
    this.BandaRodagem = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Banda de Rodagem:", idBtnSearch: guid() });
    this.DataEntradaInicio = PropertyEntity({ text: "Data de Entrada início: ", getType: typesKnockout.date });
    this.DataEntradaLimite = PropertyEntity({ text: "Data de Entrada limite: ", dateRangeInit: this.DataInicio, getType: typesKnockout.date });
    this.DTO = PropertyEntity({ text: "DOT:", getType: typesKnockout.string, val: ko.observable(""), maxlength: 500 });
    this.Modelo = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Modelo:", idBtnSearch: guid() });
    this.NumeroFogo = PropertyEntity({ text: "Número de Fogo:", getType: typesKnockout.string, val: ko.observable(""), maxlength: 500 });
    this.TipoAquisicao = PropertyEntity({ text: "Tipo Aquisição: ", val: ko.observable(EnumTipoAquisicaoPneu.Todos), options: EnumTipoAquisicaoPneu.obterOpcoesPesquisa(), def: EnumTipoAquisicaoPneu.Todos });
    this.VidaAtual = PropertyEntity({ text: "Vida Atual: ", val: ko.observable(EnumVidaPneu.Todas), options: EnumVidaPneu.obterOpcoesPesquisa(), def: EnumVidaPneu.Todas });
    this.SituacaoPneu = PropertyEntity({ text: "Situação: ", val: ko.observable(EnumSituacaoPneu.Todos), options: EnumSituacaoPneu.obterOpcoesPesquisa(), def: EnumSituacaoPneu.Todos });
    this.NumeroNota = PropertyEntity({ text: "Nº Nota:", getType: typesKnockout.int, val: ko.observable(""), maxlength: 500 });

    this.DataEntradaInicio.dateRangeLimit = this.DataEntradaLimite;
    this.DataEntradaLimite.dateRangeInit = this.DataEntradaInicio;

    this.ExibirFiltros = PropertyEntity({ eventClick: exibirFiltrosClick, type: types.event, text: "Filtros de Pesquisa", idFade: guid(), visibleFade: ko.observable(false), visible: ko.observable(true) });
    this.Pesquisar = PropertyEntity({ eventClick: recarregarGridPneu, type: types.event, text: "Pesquisar", idGrid: guid(), visible: ko.observable(true) });
};

/*
 * Declaração das Funções de Inicialização
 */

function loadGridPneu() {
    var opcaoEditar = { descricao: "Editar", id: "clasEditar", evento: "onclick", metodo: editarClick, tamanho: "10", icone: "" };
    var menuOpcoes = { tipo: TypeOptionMenu.link, opcoes: [opcaoEditar] };
    var configuracoesExportacao = { url: "Pneu/ExportarPesquisa", titulo: "Pneu" };

    _gridPneu = new GridViewExportacao(_pesquisaPneu.Pesquisar.idGrid, "Pneu/Pesquisa", _pesquisaPneu, menuOpcoes, configuracoesExportacao);
    _gridPneu.CarregarGrid();
}

function loadPneu() {
    _pneu = new Pneu();
    KoBindings(_pneu, "knockoutPneu");

    HeaderAuditoria("Pneu", _pneu);

    _CRUDPneu = new CRUDPneu();
    KoBindings(_CRUDPneu, "knockoutCRUDPneu");

    _pesquisaPneu = new PesquisaPneu();
    KoBindings(_pesquisaPneu, "knockoutPesquisaPneu", false, _pesquisaPneu.Pesquisar.id);

    new BuscarAlmoxarifado(_pesquisaPneu.Almoxarifado);
    new BuscarBandaRodagemPneu(_pesquisaPneu.BandaRodagem);
    new BuscarModeloPneu(_pesquisaPneu.Modelo);
    new BuscarAlmoxarifado(_pneu.Almoxarifado);
    new BuscarBandaRodagemPneu(_pneu.BandaRodagem);
    new BuscarModeloPneu(_pneu.Modelo);
    new BuscarProdutoTMS(_pneu.Produto);

    loadGridPneu();
    loadPneuImportar();
}

/*
 * Declaração das Funções Associadas a Eventos
 */

function importarPneuLoteClick() {
    new LancarPneuLote();
}

function adicionarClick(e, sender) {
    Salvar(_pneu, "Pneu/Adicionar", function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Cadastrado com sucesso.");

                recarregarGridPneu();
                limparCamposPneu();
                _CRUDPneu.RetornarSucata.visible(false);
            }
            else
                exibirMensagem(tipoMensagem.aviso, "Aviso", retorno.Msg);
        }
        else
            exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
    }, sender);
}

function atualizarClick(e, sender) {
    Salvar(_pneu, "Pneu/Atualizar", function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Atualizado com sucesso");

                recarregarGridPneu();
                limparCamposPneu();
                _CRUDPneu.RetornarSucata.visible(false);
            }
            else
                exibirMensagem(tipoMensagem.aviso, "Aviso", retorno.Msg);
        }
        else
            exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
    }, sender);
}

function cancelarClick() {
    limparCamposPneu();
}

function editarClick(registroSelecionado) {
    limparCamposPneu();

    _pneu.Codigo.val(registroSelecionado.Codigo);

    BuscarPorCodigo(_pneu, "Pneu/BuscarPorCodigo", function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {

                if (retorno.Data.Situacao === EnumSituacaoPneuTMS.Sucata)
                    _CRUDPneu.RetornarSucata.visible(true);
                else
                    _CRUDPneu.RetornarSucata.visible(false);

                _pesquisaPneu.ExibirFiltros.visibleFade(false);

                controlarComponentesHabilitados();
            }
            else
                exibirMensagem(tipoMensagem.aviso, "Aviso", retorno.Msg);
        }
        else
            exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
    }, null);
}

function RetornarSucataClick(e, sender) {
    exibirConfirmacao("Confirmação", "Realmente deseja retornar este pneu da sucata?", function () {
        Salvar(_pneu, "Pneu/RetornarDaSucata", function (retorno) {
            if (retorno.Success) {
                if (retorno.Data) {
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Retornado com sucesso");
                    _CRUDPneu.RetornarSucata.visible(false);
                    recarregarGridPneu();
                    limparCamposPneu();
                }
                else
                    exibirMensagem(tipoMensagem.aviso, "Aviso", retorno.Msg);
            }
            else
                exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
        }, sender);
    });
}

function excluirClick() {
    exibirConfirmacao("Confirmação", "Realmente deseja excluir esse cadastro?", function () {
        ExcluirPorCodigo(_pneu, "Pneu/ExcluirPorCodigo", function (retorno) {
            if (retorno.Success) {
                if (retorno.Data) {
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Excluído com sucesso");

                    recarregarGridPneu();
                    limparCamposPneu();
                }
                else
                    exibirMensagem(tipoMensagem.aviso, "Sugestão", retorno.Msg, 16000);
            }
            else
                exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
        }, null);
    });
}

function exibirFiltrosClick(e) {
    e.ExibirFiltros.visibleFade(!e.ExibirFiltros.visibleFade());
}

function importarClick(e, sender) {
    var file = document.getElementById(_pedagio.Arquivo.id);

    var formData = new FormData();
    formData.append("upload", file.files[0]);

    SetarPercentualProcessamento(0);
    _pedagio.PercentualProcessado.visible(true);
    _pedagio.MensagemFalhaGeracao.visible(false);
    _pedagio.Arquivo.visible(false);

    enviarArquivo("ImportacaoDePedagio/ImportarPedagios?callback=?", { Codigo: 0 }, formData, function (arg) {
        if (arg.Success) {
            exibirMensagem(tipoMensagem.ok, "Sucesso", "Importação dos pedágios realizada com sucesso.");
            SetarPercentualProcessamento(0);
            _pedagio.PercentualProcessado.visible(false);
            _pedagio.MensagemFalhaGeracao.visible(false);
            _pedagio.Arquivo.visible(true);

            _pedagio.Arquivo.val("");
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    });
}

/*
 * Declaração das Funções
 */

function AtualizarDadosSulcos() {

    var sulco = parseFloat(_pneu.Sulco.val().replace(/\./g, "").replace(",", "."));
    var sulcoAnterior = parseFloat(_pneu.SulcoAnterior.val().replace(/\./g, "").replace(",", "."));
    var sulcoGasto = 0;

    if (!isNaN(sulco) && !isNaN(sulcoAnterior))
        sulcoGasto = (sulco > sulcoAnterior) ? sulco - sulcoAnterior : sulcoAnterior - sulco;

    var kmRodadoEntreSulcos = parseInt(_pneu.KmRodadoEntreSulcos.val().replace(/\./g, ""));
    var kmRodadoPorSulco = 0;

    if (!isNaN(kmRodadoEntreSulcos) && (sulcoGasto > 0))
        kmRodadoPorSulco = kmRodadoEntreSulcos / sulcoGasto;

    _pneu.SulcoGasto.val(Globalize.format(sulcoGasto, "n2"));
    _pneu.KmRodadoPorSulco.val(Globalize.format(kmRodadoPorSulco, "n2"));
}

function atualizarValoresCusto() {
    if (isNovoCadastro()) {
        var valorAquisicao = parseFloat(_pneu.ValorAquisicao.val().replace(/\./g, "").replace(",", "."));

        if (!isNaN(valorAquisicao) && (valorAquisicao > 0)) {
            _pneu.ValorCustoAtualizado.val(valorAquisicao);
            _pneu.ValorCustoKmAtualizado.val(valorAquisicao);
        }
        else {
            _pneu.ValorCustoAtualizado.val("");
            _pneu.ValorCustoKmAtualizado.val("");
        }
    }
}

function controlarBotoesHabilitados(isEdicao) {
    _CRUDPneu.Atualizar.visible(isEdicao);
    _CRUDPneu.Excluir.visible(isEdicao);
    _CRUDPneu.Cancelar.visible(isEdicao);
    _CRUDPneu.Adicionar.visible(!isEdicao);
}

function controlarCamposHabilitados(isEdicao) {
    _pneu.ValorAquisicao.enable(!isEdicao);
}

function controlarComponentesHabilitados() {
    var isEdicao = !isNovoCadastro();

    controlarBotoesHabilitados(isEdicao);
    controlarCamposHabilitados(isEdicao);
}

function isNovoCadastro() {
    return _pneu.Codigo.val() == 0;
}

function limparCamposPneu() {
    LimparCampos(_pneu);
    controlarComponentesHabilitados();
}

function recarregarGridPneu() {
    _gridPneu.CarregarGrid();
}

function VerificarImportacaoPedagioEstaSelecionada() {

    exibirMensagem(tipoMensagem.ok, "Sucesso", "Importação dos pedágios realizada com sucesso.");

    SetarPercentualProcessamento(0);
    _pedagio.PercentualProcessado.visible(false);
    _pedagio.MensagemFalhaGeracao.visible(false);
    _pedagio.Arquivo.visible(true);

    _pedagio.Arquivo.val("");
    _gridPedagio.CarregarGrid();
    resetarTabs();
}

function AtualizarProgressImportacaoPedagio(percentual) {
    finalizarRequisicao();
    SetarPercentualProcessamento(percentual);
}

function SetarPercentualProcessamento(percentual) {
    var strPercentual = parseInt(percentual) + "%";
    _pedagio.PercentualProcessado.val(strPercentual);
    $("#" + _pedagio.PercentualProcessado.id).css("width", strPercentual)
}