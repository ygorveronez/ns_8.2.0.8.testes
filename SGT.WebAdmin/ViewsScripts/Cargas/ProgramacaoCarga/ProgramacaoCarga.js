/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Globais.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="../../Consultas/ConfiguracaoProgramacaoCarga.js" />
/// <reference path="..\..\Consultas\Estado.js" />
/// <reference path="../../Consultas/Filial.js" />
/// <reference path="..\..\Consultas\Localidade.js" />
/// <reference path="../../Consultas/ModeloVeicularCarga.js" />
/// <reference path="../../Consultas/TipoDeCarga.js" />
/// <reference path="../../Consultas/TipoOperacao.js" />
/// <reference path="../../Enumeradores/EnumSituacaoSugestaoProgramacaoCarga.js" />

// #region Objetos Globais do Arquivo

var _gridDestinoProgramacaoCargaAdicionar;
var _gridEstadoDestinoProgramacaoCargaAdicionar;
var _gridProgramacaoCarga;
var _gridRegiaoDestinoProgramacaoCargaAdicionar;
var _pesquisaProgramacaoCarga;
var _programacaoCargaAdicionar;
var _programacaoCargaGerar;

// #endregion Objetos Globais do Arquivo

// #region Classes

var PesquisaProgramacaoCarga = function () {
    var dataProgramacaoInicial = Global.DataAtual();
    var dataProgramacaoFinal = moment().add(_CONFIGURACAO_TMS.DiasFiltrarDataProgramada, 'days').format("DD/MM/YYYY");

    this.ConfiguracaoProgramacaoCarga = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Configuração de Pré Planejamento:", idBtnSearch: guid() });
    this.DataProgramacaoInicial = PropertyEntity({ text: "Data de Pré Planejamento de: ", getType: typesKnockout.date, val: ko.observable(dataProgramacaoInicial) });
    this.DataProgramacaoFinal = PropertyEntity({ text: "Data de Pré Planejamento até: ", getType: typesKnockout.date, val: ko.observable(dataProgramacaoFinal) });
    this.Filial = PropertyEntity({ text: "Filial", type: types.multiplesEntities, codEntity: ko.observable(0), idBtnSearch: guid() });
    this.ModeloVeicularCarga = PropertyEntity({ text: "Modelo Veicular de Carga", type: types.multiplesEntities, codEntity: ko.observable(0), idBtnSearch: guid() });
    this.Situacao = PropertyEntity({ text: "Situação: ", getType: typesKnockout.selectMultiple, val: ko.observable([EnumSituacaoSugestaoProgramacaoCarga.Gerada]), options: EnumSituacaoSugestaoProgramacaoCarga.obterOpcoes(), def: [EnumSituacaoSugestaoProgramacaoCarga.Gerada] });
    this.TipoCarga = PropertyEntity({ text: "Tipo de Carga:", type: types.multiplesEntities, codEntity: ko.observable(0), idBtnSearch: guid() });
    this.TipoOperacao = PropertyEntity({ text: "Tipo de Operação", type: types.multiplesEntities, codEntity: ko.observable(0), idBtnSearch: guid() });

    this.DataProgramacaoInicial.dateRangeLimit = this.DataProgramacaoFinal;
    this.DataProgramacaoFinal.dateRangeInit = this.DataProgramacaoInicial;

    this.ExibirDadosResumo = PropertyEntity({ eventClick: function (e) { e.ExibirDadosResumo.visibleFade(!e.ExibirDadosResumo.visibleFade()); }, type: types.event, idFade: guid(), visibleFade: ko.observable(true) });
    this.ResumoProgramacaoCarga = PropertyEntity({ totalSugerido: ko.observable(""), totalValidado: ko.observable(""), visible: ko.observable(true) });
    this.ListaResumoProgramacaoCarga = ko.observableArray();

    this.AdicionarSugestao = PropertyEntity({ eventClick: adicionarProgramacaoCargaModalClick, type: types.event, text: "Adicionar Sugestão", idGrid: guid(), visible: ko.observable(false) });
    this.CancelarTodas = PropertyEntity({ type: types.event, val: ko.observable(false), eventClick: cancelarMultiplasSugestoesClick, text: "Cancelar Cargas", visible: ko.observable(false) });
    this.ExibirFiltros = PropertyEntity({ eventClick: function (e) { e.ExibirFiltros.visibleFade(!e.ExibirFiltros.visibleFade()); }, type: types.event, text: "Filtros de Pesquisa", idFade: guid(), visibleFade: ko.observable(true) });
    this.GerarSugestoes = PropertyEntity({ eventClick: gerarProgramacaoCargaModalClick, type: types.event, text: "Gerar Sugestões", idGrid: guid(), visible: ko.observable(false) });
    this.Pesquisar = PropertyEntity({ eventClick: carregarProgramacaoCarga, type: types.event, text: "Pesquisar", idGrid: guid() });
    this.PublicarTodas = PropertyEntity({ type: types.event, val: ko.observable(false), eventClick: publicarMultiplasSugestoesClick, text: "Publicar Cargas", visible: ko.observable(false) });

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
        }, type: types.event, text: "Avançada", idFade: guid(), icon: ko.observable("fal fa-plus"), visibleFade: ko.observable(false)
    });
}

var ProgramacaoCargaAdicionar = function () {
    this.DataProgramacaoInicial = PropertyEntity({ text: "*Data de Pré Planejamento de: ", getType: typesKnockout.date, required: true });
    this.DataProgramacaoFinal = PropertyEntity({ text: "*Data de Pré Planejamento até: ", getType: typesKnockout.date, required: true });
    this.Filial = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "*Filial:", idBtnSearch: guid(), required: true });
    this.ModeloVeicularCarga = PropertyEntity({ text: "*Modelo Veicular de Carga:", type: types.entity, codEntity: ko.observable(0), idBtnSearch: guid(), required: true });
    this.TipoCarga = PropertyEntity({ text: "*Tipo de Carga:", type: types.entity, codEntity: ko.observable(0), idBtnSearch: guid(), required: true });
    this.TipoOperacao = PropertyEntity({ text: "*Tipo de Operação:", type: types.entity, codEntity: ko.observable(0), idBtnSearch: guid(), required: true });
    this.Quantidade = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int, text: "*Quantidade:", required: true });

    this.DataProgramacaoInicial.dateRangeLimit = this.DataProgramacaoFinal;
    this.DataProgramacaoFinal.dateRangeInit = this.DataProgramacaoInicial;

    this.Adicionar = PropertyEntity({ eventClick: adicionarProgramacaoCargaClick, type: types.event, text: "Adicionar", idGrid: guid() });
    this.AdicionarDestinos = PropertyEntity({ type: types.event, text: "Adicionar Cidades", idBtnSearch: guid(), idGrid: guid() });
    this.AdicionarEstadosDestino = PropertyEntity({ type: types.event, text: "Adicionar Estados", idBtnSearch: guid(), idGrid: guid() });
    this.AdicionarRegioesDestino = PropertyEntity({ type: types.event, text: "Adicionar Regiões", idBtnSearch: guid(), idGrid: guid() });
}

var ProgramacaoCargaGerar = function () {
    this.ConfiguracaoProgramacaoCarga = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "*Configuração de Pré Planejamento:", idBtnSearch: guid(), required: true });
    this.DataHistoricoInicial = PropertyEntity({ text: "*Data de Histórico de: ", getType: typesKnockout.date, required: true });
    this.DataHistoricoFinal = PropertyEntity({ text: "*Data de Histórico até: ", getType: typesKnockout.date, required: true });
    this.DataProgramacaoInicial = PropertyEntity({ text: "*Data de Pré Planejamento de: ", getType: typesKnockout.date, required: true });
    this.DataProgramacaoFinal = PropertyEntity({ text: "*Data de Pré Planejamento até: ", getType: typesKnockout.date, required: true });

    this.DataHistoricoInicial.dateRangeLimit = this.DataHistoricoFinal;
    this.DataHistoricoFinal.dateRangeInit = this.DataHistoricoInicial;
    this.DataProgramacaoInicial.dateRangeLimit = this.DataProgramacaoFinal;
    this.DataProgramacaoFinal.dateRangeInit = this.DataProgramacaoInicial;

    this.Gerar = PropertyEntity({ eventClick: gerarProgramacaoCargaClick, type: types.event, text: "Gerar", idGrid: guid() });
}

// #endregion Classes

// #region Funções de Inicialização

function loadGridDestinoProgramacaoCargaAdicionar() {
    var opcaoExcluir = { descricao: "Excluir", id: guid(), metodo: function (registroSelecionado) { excluirDestinoProgramacaoCargaAdicionarClick(_gridDestinoProgramacaoCargaAdicionar, registroSelecionado); } };
    var menuOpcoes = { tipo: TypeOptionMenu.link, tamanho: 15, opcoes: [opcaoExcluir] };

    var header = [
        { data: "Codigo", visible: false },
        { data: "Descricao", title: "Descrição", width: "85%" }
    ];

    _gridDestinoProgramacaoCargaAdicionar = new BasicDataTable(_programacaoCargaAdicionar.AdicionarDestinos.idGrid, header, menuOpcoes, { column: 1, dir: orderDir.asc });

    new BuscarLocalidades(_programacaoCargaAdicionar.AdicionarDestinos, null, null, null, _gridDestinoProgramacaoCargaAdicionar, controlarVisibilidadeAbasProgramacaoCargaAdicionarDestino);

    _gridDestinoProgramacaoCargaAdicionar.CarregarGrid([]);
}

function loadGridEstadoDestinoProgramacaoCargaAdicionar() {
    var opcaoExcluir = { descricao: "Excluir", id: guid(), metodo: function (registroSelecionado) { excluirDestinoProgramacaoCargaAdicionarClick(_gridEstadoDestinoProgramacaoCargaAdicionar, registroSelecionado); } };
    var menuOpcoes = { tipo: TypeOptionMenu.link, tamanho: 15, opcoes: [opcaoExcluir] };

    var header = [
        { data: "Codigo", visible: false },
        { data: "Descricao", title: "Descrição", width: "85%" }
    ];

    _gridEstadoDestinoProgramacaoCargaAdicionar = new BasicDataTable(_programacaoCargaAdicionar.AdicionarEstadosDestino.idGrid, header, menuOpcoes, { column: 1, dir: orderDir.asc });

    new BuscarEstados(_programacaoCargaAdicionar.AdicionarEstadosDestino, null, _gridEstadoDestinoProgramacaoCargaAdicionar, controlarVisibilidadeAbasProgramacaoCargaAdicionarDestino);

    _gridEstadoDestinoProgramacaoCargaAdicionar.CarregarGrid([]);
}

function loadGridProgramacaoCarga() {
    var totalRegistrosPorPagina = 12;
    var limiteRegistros = 60;

    var configExportacao = {
        url: "ProgramacaoCarga/ExportarPesquisa",
        titulo: "Sugestões de Pré Planejamento"
    };

    var inforEditarColuna = {
        permite: true,
        atualizarRow: true,
        callback: atualizarColunaGridProgramacaoCarga
    };

    var multiplaEscolha = {
        basicGrid: null,
        eventos: {},
        selecionados: new Array(),
        naoSelecionados: new Array(),
        callbackNaoSelecionado: controlarExibirMultiplasOpcoes,
        callbackSelecionado: controlarExibirMultiplasOpcoes,
        somenteLeitura: false,
        classeSelecao: "item-selecionado"
    };

    var opcaoCancelar = { descricao: "Cancelar", id: guid(), evento: "onclick", metodo: cancelarSugestoesClick, tamanho: "10", icone: "", visibilidade: isPermitirAlterarSugestao };
    var opcaoDownloadHistorico = { descricao: "Download do Histórico", id: guid(), evento: "onclick", metodo: downloadHitoricoSugestaoClick, tamanho: "10", icone: "" };
    var opcaoPublicar = { descricao: "Publicar", id: guid(), evento: "onclick", metodo: publicarSugestoesClick, tamanho: "10", icone: "", visibilidade: isPermitirAlterarSugestao };
    var menuOpcoes = { tipo: TypeOptionMenu.list, descricao: "Opções", opcoes: [opcaoCancelar, opcaoDownloadHistorico, opcaoPublicar], tamanho: 10 };

    _gridProgramacaoCarga = new GridView(_pesquisaProgramacaoCarga.Pesquisar.idGrid, "ProgramacaoCarga/Pesquisa", _pesquisaProgramacaoCarga, menuOpcoes, null, totalRegistrosPorPagina, null, null, null, multiplaEscolha, limiteRegistros, inforEditarColuna, configExportacao);
}

function loadGridRegiaoDestinoProgramacaoCargaAdicionar() {
    var opcaoExcluir = { descricao: "Excluir", id: guid(), metodo: function (registroSelecionado) { excluirDestinoProgramacaoCargaAdicionarClick(_gridRegiaoDestinoProgramacaoCargaAdicionar, registroSelecionado); } };
    var menuOpcoes = { tipo: TypeOptionMenu.link, tamanho: 15, opcoes: [opcaoExcluir] };

    var header = [
        { data: "Codigo", visible: false },
        { data: "Descricao", title: "Descrição", width: "85%" }
    ];

    _gridRegiaoDestinoProgramacaoCargaAdicionar = new BasicDataTable(_programacaoCargaAdicionar.AdicionarRegioesDestino.idGrid, header, menuOpcoes, { column: 1, dir: orderDir.asc });

    new BuscarRegioes(_programacaoCargaAdicionar.AdicionarRegioesDestino, null, _gridRegiaoDestinoProgramacaoCargaAdicionar, controlarVisibilidadeAbasProgramacaoCargaAdicionarDestino);

    _gridRegiaoDestinoProgramacaoCargaAdicionar.CarregarGrid([]);
}

function loadProgramacaoCarga() {
    _pesquisaProgramacaoCarga = new PesquisaProgramacaoCarga();
    KoBindings(_pesquisaProgramacaoCarga, "knockoutPesquisaProgramacaoCarga", false, _pesquisaProgramacaoCarga.Pesquisar.id);

    _programacaoCargaAdicionar = new ProgramacaoCargaAdicionar();
    KoBindings(_programacaoCargaAdicionar, "knockoutProgramacaoCargaAdicionar");

    _programacaoCargaGerar = new ProgramacaoCargaGerar();
    KoBindings(_programacaoCargaGerar, "knockoutProgramacaoCargaGerar");

    new BuscarConfiguracaoProgramacaoCarga(_pesquisaProgramacaoCarga.ConfiguracaoProgramacaoCarga);
    new BuscarFilial(_pesquisaProgramacaoCarga.Filial);
    new BuscarModelosVeicularesCarga(_pesquisaProgramacaoCarga.ModeloVeicularCarga);
    new BuscarTiposdeCarga(_pesquisaProgramacaoCarga.TipoCarga);
    new BuscarTiposOperacao(_pesquisaProgramacaoCarga.TipoOperacao);

    new BuscarFilial(_programacaoCargaAdicionar.Filial);
    new BuscarModelosVeicularesCarga(_programacaoCargaAdicionar.ModeloVeicularCarga);
    new BuscarTiposdeCarga(_programacaoCargaAdicionar.TipoCarga);
    new BuscarTiposOperacao(_programacaoCargaAdicionar.TipoOperacao);

    new BuscarConfiguracaoProgramacaoCarga(_programacaoCargaGerar.ConfiguracaoProgramacaoCarga);

    loadGridProgramacaoCarga();
    loadGridDestinoProgramacaoCargaAdicionar();
    loadGridEstadoDestinoProgramacaoCargaAdicionar();
    loadGridRegiaoDestinoProgramacaoCargaAdicionar();
}

// #endregion Funções de Inicialização

// #region Funções Associadas a Eventos

function adicionarProgramacaoCargaClick() {
    if (!validarCamposObrigatoriosProgramacaoCargaAdicionar())
        return;

    executarReST("ProgramacaoCarga/AdicionarSugestao", obterProgramacaoCargaAdicionar(), function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                if (retorno.Data.TotalSugestoesAdicionadas == 1)
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "1 sugestão de pré planejamento adicionada com sucesso!");
                else
                    exibirMensagem(tipoMensagem.ok, "Sucesso", retorno.Data.TotalSugestoesAdicionadas + " sugestões de pré planejamento adicionadas com sucesso!");

                Global.fecharModal("divModalProgramacaoCargaAdicionar");

                if (retorno.Data.TotalSugestoesAdicionadas > 0)
                    recarregarProgramacaoCarga();
            }
            else
                exibirMensagem(tipoMensagem.atencao, "Atenção", retorno.Msg);
        }
        else
            exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
    });
}

function adicionarProgramacaoCargaModalClick() {
    var dataAtual = Global.DataAtual();

    _programacaoCargaAdicionar.DataProgramacaoInicial.minDate(dataAtual);
    _programacaoCargaAdicionar.DataProgramacaoFinal.minDate(dataAtual);

    Global.abrirModal('divModalProgramacaoCargaAdicionar');
    $("#divModalProgramacaoCargaAdicionar").one('hidden.bs.modal', function () {
        limparCamposProgramacaoCargaAdicionar();
    });
}

function atualizarColunaGridProgramacaoCarga(dataRow, row, head) {
    if (head.data != "QuantidadeValidada") {
        _gridProgramacaoCarga.DesfazerAlteracaoDataRow(row);
        return;
    }

    var quantidadeValidada = parseInt(dataRow.QuantidadeValidada.replace(/\./g, "").replace(/,/g, "."))

    if (isNaN(quantidadeValidada) || (quantidadeValidada <= 0)) {
        _gridProgramacaoCarga.DesfazerAlteracaoDataRow(row);
        return;
    }

    var dados = {
        Codigo: dataRow.Codigo,
        QuantidadeValidada: dataRow.QuantidadeValidada
    };

    executarReST("ProgramacaoCarga/AlterarSugestao", dados, function (retorno) {
        if (retorno.Success) {
            if (retorno.Data)
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Sugestão de pré planejamento atualizada com sucesso!");
            else {
                exibirMensagem(tipoMensagem.atencao, "Atenção", retorno.Msg);
                _gridProgramacaoCarga.DesfazerAlteracaoDataRow(row);
            }
        }
        else {
            exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
            _gridProgramacaoCarga.DesfazerAlteracaoDataRow(row);
        }
    });
}

function cancelarMultiplasSugestoesClick() {
    exibirConfirmacao("Confirmação", "Você realmente deseja cancelar todas as sugestões de pré planejamento selecionadas?", function () {
        var dados = {
            ItensSelecionados: JSON.stringify(_gridProgramacaoCarga.ObterMultiplosSelecionados())
        }

        executarReST("ProgramacaoCarga/CancelarMultiplasSugestoes", dados, function (retorno) {
            if (retorno.Success) {
                if (retorno.Data) {
                    if (retorno.Data.TotalSugestoesCanceladas > 0) {
                        if (retorno.Data.TotalSugestoesCanceladas > 1)
                            exibirMensagem(tipoMensagem.ok, "Sucesso", retorno.Data.TotalSugestoesCanceladas + " sugestões de pré planejamento foram canceladas.");
                        else
                            exibirMensagem(tipoMensagem.ok, "Sucesso", "1 sugestão de pré planejamento foi cancelada.");
                    }
                    else
                        exibirMensagem(tipoMensagem.aviso, "Sucesso", "Nenhuma sugestão de pré planejamento disponível para cancelamento.");

                    recarregarProgramacaoCarga();
                }
                else
                    exibirMensagem(tipoMensagem.atencao, "Atenção", retorno.Msg);
            }
            else
                exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
        });
    });
}

function cancelarSugestoesClick(registroSelecionado) {
    exibirConfirmacao("Confirmação", "Você realmente deseja cancelar a sugestão de pré planejamento?", function () {
        executarReST("ProgramacaoCarga/CancelarSugestao", { Codigo: registroSelecionado.Codigo }, function (retorno) {
            if (retorno.Success) {
                if (retorno.Data) {
                    exibirMensagem(tipoMensagem.aviso, "Sucesso", "Sugestão de pré planejamento cancelada com sucesso!");

                    recarregarProgramacaoCarga();
                }
                else
                    exibirMensagem(tipoMensagem.atencao, "Atenção", retorno.Msg);
            }
            else
                exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
        });
    });
}

function downloadHitoricoSugestaoClick(registroSelecionado) {
    executarDownload("ProgramacaoCarga/DownloadHistoricoCarga", { Codigo: registroSelecionado.Codigo })
}

function excluirDestinoProgramacaoCargaAdicionarClick(gridDestino, registroSelecionado) {
    var listaDestino = gridDestino.BuscarRegistros().slice();

    for (var i = 0; i < listaDestino.length; i++) {
        if (registroSelecionado.Codigo == listaDestino[i].Codigo) {
            listaDestino.splice(i, 1);
            break;
        }
    }

    gridDestino.CarregarGrid(listaDestino);
    controlarVisibilidadeAbasProgramacaoCargaAdicionarDestino();
}

function gerarProgramacaoCargaClick() {
    if (!ValidarCamposObrigatorios(_programacaoCargaGerar))
        return exibirMensagem(tipoMensagem.atencao, "Campos Obrigatórios", "Por favor, informe os campos obrigatórios.");

    executarReST("ProgramacaoCarga/GerarSugestoes", RetornarObjetoPesquisa(_programacaoCargaGerar), function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Sugestões de pré planejamento geradas com sucesso!");
                Global.fecharModal("divModalProgramacaoCargaGerar");
                recarregarProgramacaoCarga();
            }
            else
                exibirMensagem(tipoMensagem.atencao, "Atenção", retorno.Msg);
        }
        else
            exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
    });
}

function gerarProgramacaoCargaModalClick() {
    var dataAtual = Global.DataAtual();

    _programacaoCargaGerar.DataProgramacaoInicial.minDate(dataAtual);
    _programacaoCargaGerar.DataProgramacaoFinal.minDate(dataAtual);

    Global.abrirModal('divModalProgramacaoCargaGerar');
    $("#divModalProgramacaoCargaGerar").one('hidden.bs.modal', function () {
        LimparCampos(_programacaoCargaGerar);
    });
}

function publicarMultiplasSugestoesClick() {
    exibirConfirmacao("Confirmação", "Você realmente deseja publicar todas as sugestões de pré planejamento selecionadas?", function () {
        var dados = {
            ItensSelecionados: JSON.stringify(_gridProgramacaoCarga.ObterMultiplosSelecionados())
        }

        executarReST("ProgramacaoCarga/PublicarMultiplasSugestoes", dados, function (retorno) {
            if (retorno.Success) {
                if (retorno.Data) {
                    if (retorno.Data.TotalSugestoesPublicadas > 0) {
                        if (retorno.Data.TotalSugestoesPublicadas > 1)
                            exibirMensagem(tipoMensagem.ok, "Sucesso", retorno.Data.TotalSugestoesPublicadas + " sugestões de pré planejamento foram publicadas.");
                        else
                            exibirMensagem(tipoMensagem.ok, "Sucesso", "1 sugestão de pré planejamento foi publicada.");
                    }
                    else
                        exibirMensagem(tipoMensagem.aviso, "Sucesso", "Nenhuma sugestão de pré planejamento disponível para publicação.");

                    recarregarProgramacaoCarga();
                }
                else
                    exibirMensagem(tipoMensagem.atencao, "Atenção", retorno.Msg);
            }
            else
                exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
        });
    });
}

function publicarSugestoesClick(registroSelecionado) {
    exibirConfirmacao("Confirmação", "Você realmente deseja publicar a sugestão de pré planejamento?", function () {
        executarReST("ProgramacaoCarga/PublicarSugestao", { Codigo: registroSelecionado.Codigo }, function (retorno) {
            if (retorno.Success) {
                if (retorno.Data) {
                    exibirMensagem(tipoMensagem.aviso, "Sucesso", "Sugestão de pré planejamento publicada com sucesso!");

                    recarregarProgramacaoCarga();
                }
                else
                    exibirMensagem(tipoMensagem.atencao, "Atenção", retorno.Msg);
            }
            else
                exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
        });
    });
}

// #endregion Funções Associadas a Eventos

// #region Métodos Privados

function carregarProgramacaoCarga() {
    $("#container-dados-programacao-carga").show();

    _pesquisaProgramacaoCarga.ExibirFiltros.visibleFade(false);
    _pesquisaProgramacaoCarga.AdicionarSugestao.visible(true);
    _pesquisaProgramacaoCarga.GerarSugestoes.visible(true);

    recarregarProgramacaoCarga();
}

function carregarResumoProgramacaoCarga() {
    executarReST("ProgramacaoCarga/ObterResumoSugestoes", RetornarObjetoPesquisa(_pesquisaProgramacaoCarga), function (retorno) {
        if (retorno.Success) {
            _pesquisaProgramacaoCarga.ListaResumoProgramacaoCarga(retorno.Data.ListaResumida);
            _pesquisaProgramacaoCarga.ResumoProgramacaoCarga.totalSugerido(retorno.Data.TotalSugerido);
            _pesquisaProgramacaoCarga.ResumoProgramacaoCarga.totalValidado(retorno.Data.TotalValidado);
            _pesquisaProgramacaoCarga.ResumoProgramacaoCarga.visible(true);
        }
        else {
            exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
            _pesquisaProgramacaoCarga.ResumoProgramacaoCarga.visible(false);
        }
    });
}

function controlarExibirMultiplasOpcoes() {
    var existemRegistrosSelecionados = _gridProgramacaoCarga.ObterMultiplosSelecionados().length > 0;

    _pesquisaProgramacaoCarga.CancelarTodas.visible(existemRegistrosSelecionados);
    _pesquisaProgramacaoCarga.PublicarTodas.visible(existemRegistrosSelecionados);
}

function controlarVisibilidadeAbasProgramacaoCargaAdicionarDestino() {
    if (_gridDestinoProgramacaoCargaAdicionar.BuscarRegistros().length > 0) {
        $("#liTabProgramacaoCargaAdicionarCidadesDestino").show();
        $("#liTabProgramacaoCargaAdicionarEstadosDestino").hide();
        $("#liTabProgramacaoCargaAdicionarRegioesDestino").hide();

        $(".nav-tabs a[href='#tabProgramacaoCargaAdicionarCidadesDestino']").tab('show');
    }
    else if (_gridEstadoDestinoProgramacaoCargaAdicionar.BuscarRegistros().length > 0) {
        $("#liTabProgramacaoCargaAdicionarCidadesDestino").hide();
        $("#liTabProgramacaoCargaAdicionarEstadosDestino").show();
        $("#liTabProgramacaoCargaAdicionarRegioesDestino").hide();

        $(".nav-tabs a[href='#tabProgramacaoCargaAdicionarEstadosDestino']").tab('show');
    }
    else if (_gridRegiaoDestinoProgramacaoCargaAdicionar.BuscarRegistros().length > 0) {
        $("#liTabProgramacaoCargaAdicionarCidadesDestino").hide();
        $("#liTabProgramacaoCargaAdicionarEstadosDestino").hide();
        $("#liTabProgramacaoCargaAdicionarRegioesDestino").show();

        $(".nav-tabs a[href='#tabProgramacaoCargaAdicionarRegioesDestino']").tab('show');
    }
    else {
        $("#liTabProgramacaoCargaAdicionarCidadesDestino").show();
        $("#liTabProgramacaoCargaAdicionarEstadosDestino").show();
        $("#liTabProgramacaoCargaAdicionarRegioesDestino").show();
    }
}

function isPermitirAlterarSugestao(registroSelecionado) {
    return (registroSelecionado.Situacao == EnumSituacaoSugestaoProgramacaoCarga.Gerada);
}

function limparCamposProgramacaoCargaAdicionar() {
    LimparCampos(_programacaoCargaAdicionar);

    _gridDestinoProgramacaoCargaAdicionar.CarregarGrid([]);
    _gridEstadoDestinoProgramacaoCargaAdicionar.CarregarGrid([]);
    _gridRegiaoDestinoProgramacaoCargaAdicionar.CarregarGrid([]);

    controlarVisibilidadeAbasProgramacaoCargaAdicionarDestino();

    $(".nav-tabs a[href='#tabProgramacaoCargaAdicionarCidadesDestino']").tab('show');
}

function obterProgramacaoCargaAdicionar() {
    var codigosDestinos = [];
    var codigosEstadosDestino = [];
    var codigosRegioesDestino = [];

    _gridDestinoProgramacaoCargaAdicionar.BuscarRegistros().slice().forEach(function (destino) {
        codigosDestinos.push(destino.Codigo);
    });

    _gridEstadoDestinoProgramacaoCargaAdicionar.BuscarRegistros().slice().forEach(function (estadoDestino) {
        codigosEstadosDestino.push(estadoDestino.Codigo);
    });

    _gridRegiaoDestinoProgramacaoCargaAdicionar.BuscarRegistros().slice().forEach(function (regiaoDestino) {
        codigosRegioesDestino.push(regiaoDestino.Codigo);
    });

    var programacaoCargaAdicionar = RetornarObjetoPesquisa(_programacaoCargaAdicionar);

    programacaoCargaAdicionar.Destinos = JSON.stringify(codigosDestinos);
    programacaoCargaAdicionar.EstadosDestino = JSON.stringify(codigosEstadosDestino);
    programacaoCargaAdicionar.RegioesDestino = JSON.stringify(codigosRegioesDestino);

    return programacaoCargaAdicionar;
}

function recarregarProgramacaoCarga() {
    _gridProgramacaoCarga.AtualizarRegistrosSelecionados([]);
    _gridProgramacaoCarga.CarregarGrid();

    carregarResumoProgramacaoCarga();
    controlarExibirMultiplasOpcoes();
}

function validarCamposObrigatoriosProgramacaoCargaAdicionar() {
    if (!ValidarCamposObrigatorios(_programacaoCargaAdicionar)) {
        exibirMensagem(tipoMensagem.atencao, "Campos Obrigatórios", "Por Favor, informe os campos obrigatórios");
        return false;
    }

    var totalDestinos = _gridDestinoProgramacaoCargaAdicionar.BuscarRegistros().length;
    var totalEstadosDestino = _gridEstadoDestinoProgramacaoCargaAdicionar.BuscarRegistros().length;
    var totalRegioesDestino = _gridRegiaoDestinoProgramacaoCargaAdicionar.BuscarRegistros().length;
    var possuiDestinoinformado = (totalDestinos > 0) || (totalEstadosDestino > 0) || (totalRegioesDestino > 0);

    if (!possuiDestinoinformado) {
        exibirMensagem(tipoMensagem.atencao, "Campos Obrigatórios", "Por Favor, informe pelo menos um destino");
        return false;
    }

    return true;
}

// #endregion Métodos Privados