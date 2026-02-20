/// <reference path="../../Consultas/TipoDespesaFinanceira.js" />
/// <reference path="../../Consultas/Veiculo.js" />
/// <reference path="../../Consultas/SegmentoVeiculo.js" />
/// <reference path="../../Consultas/CentroResultado.js" />
/// <reference path="../../Consultas/Cliente.js" />
/// <reference path="../../Consultas/Usuario.js" />

/*
 * Declaração de Objetos Globais do Arquivo
 */

var _CRUDRateioDespesa;
var _rateioDespesa, _lancamentoRateioDespesa, _detalheLancamentoRateioDespesa;
var _pesquisaRateioDespesa;
var _gridRateioDespesa, _gridVeiculoDespesa, _gridSegmentoVeiculoDespesa, _gridCentroResultadoDespesa, _gridRateioDespesaEntreVeiculos, _gridDetalheRateioDespesaEntreVeiculos;

/*
 * Declaração das Classes
 */

var CRUDRateioDespesa = function () {
    this.Adicionar = PropertyEntity({ eventClick: AdicionarDespesaClick, type: types.event, text: "Adicionar", visible: ko.observable(true) });
    this.Cancelar = PropertyEntity({ eventClick: CancelarDespesaClick, type: types.event, text: "Cancelar", visible: ko.observable(false) });
    this.Excluir = PropertyEntity({ eventClick: ExcluirDespesaClick, type: types.event, text: "Excluir", visible: ko.observable(false) });
};

var RateioDespesa = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });

    this.MultiplosTitulos = PropertyEntity({ val: ko.observable(false), getType: typesKnockout.bool, def: false });
    this.Titulo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.MovimentoFinanceiro = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.ContratoFinanciamento = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.DocumentoEntrada = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Infracao = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });

    this.DataInicial = PropertyEntity({ text: "*Data Inicial:", required: true, getType: typesKnockout.date, val: ko.observable(""), def: "", enable: ko.observable(true) });
    this.DataFinal = PropertyEntity({ text: "*Data Final:", required: true, getType: typesKnockout.date, val: ko.observable(""), def: "", enable: ko.observable(true) });
    this.Valor = PropertyEntity({ text: "*Valor:", required: true, getType: typesKnockout.decimal, val: ko.observable(""), def: "", enable: ko.observable(true) });
    this.TipoDespesa = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "*Tipo de Despesa:", idBtnSearch: guid(), issue: 0, visible: ko.observable(true), required: true, enable: ko.observable(true) });

    this.Valor.val.subscribe((novoValor) => {
        if (_CONFIGURACAO_TMS.UtilizarValorDesproporcionalRateioDespesaVeiculo)
            DistribuirValorGrid(novoValor);
    })

    this.AlterarCentroResultadoRateio = PropertyEntity({ text: "Alterar o Centro de Resultado", val: ko.observable(false), def: false, enable: ko.observable(true) });
    this.CentroResultadoRateio = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Ratear para o Centro de Resultado:", idBtnSearch: guid(), issue: 0, visible: ko.observable(true), required: ko.observable(false), enable: ko.observable(true) });

    this.Pessoa = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Pessoa:", idBtnSearch: guid(), issue: 0, visible: ko.observable(true), required: false, enable: ko.observable(true) });
    this.Colaborador = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Colaborador:", idBtnSearch: guid(), issue: 0, visible: ko.observable(true), required: false, enable: ko.observable(true) });
    this.NumeroDocumento = PropertyEntity({ text: "*Nº do Documento:", required: true, getType: typesKnockout.string, val: ko.observable(""), def: "", maxlength: 50, enable: ko.observable(true) });
    this.TipoDocumento = PropertyEntity({ text: "*Tipo do Documento:", required: true, getType: typesKnockout.string, val: ko.observable(""), def: "", maxlength: 50, enable: ko.observable(true) });
    this.RatearPeloPercentualFaturadoVeiculo = PropertyEntity({ text: "Ratear pelo percentual faturado de cada veículo no período", val: ko.observable(false), def: false, enable: ko.observable(true) });

    this.RatearDespesaUmaVezPorMes = PropertyEntity({ text: "Deseja gerar este rateio uma vez por mês durante o período selecionado?", val: ko.observable(false), def: false, enable: ko.observable(true), visible: ko.observable(true) });
    this.DiaMesRateio = PropertyEntity({ text: "*Dia do Rateio: ", getType: typesKnockout.int, enable: ko.observable(true), maxlength: 2, visible: ko.observable(false), required: ko.observable(false) });

    this.VeiculosDespesa = PropertyEntity({ type: types.map, getType: typesKnockout.dynamic, val: ko.observable(""), def: "", idGrid: guid() });
    this.SegmentosVeiculosDespesa = PropertyEntity({ type: types.map, getType: typesKnockout.dynamic, val: ko.observable(""), def: "", idGrid: guid() });
    this.CentrosResultadoDespesa = PropertyEntity({ type: types.map, getType: typesKnockout.dynamic, val: ko.observable(""), def: "", idGrid: guid() });

    this.GridVeiculosDespesa = PropertyEntity({ type: types.local });
    this.Veiculo = PropertyEntity({ type: types.event, text: "Adicionar Veículo", idBtnSearch: guid(), issue: 0, visible: ko.observable(true), enable: ko.observable(true) });
    this.ImportarVeiculos = PropertyEntity({
        type: types.local,
        text: "Importar",
        visible: ko.observable(obterVisibilidadeImportarVeiculo()),
        accept: ".xls,.xlsx,.csv",
        cssClass: "btn-default botaoDentroSmartAdmimForm",
        UrlImportacao: "RateioDespesaVeiculo/ObterDadosImportacao",
        UrlConfiguracao: "RateioDespesaVeiculo/ConfiguracaoImportacao",
        CallbackImportacao: function (arg) {
            const retorno = arg.Data.Retorno;

            ModificarValorGeral(retorno.Total);
            _gridVeiculoDespesa.CarregarGrid(converterListaEmGridEditavel(retorno.ListaVeiculos));
            ControlarVisibilidadeSegmentoEVeiculo();
        },
        FecharModalSeSucesso: true
    });

    this.GridSegmentoVeiculoDespesa = PropertyEntity({ type: types.local });
    this.SegmentoVeiculo = PropertyEntity({ type: types.event, text: "Adicionar Segmento de Veículo", idBtnSearch: guid(), issue: 0, visible: ko.observable(true), enable: ko.observable(true) });

    this.GridCentroResultadoDespesa = PropertyEntity({ type: types.local });
    this.CentroResultado = PropertyEntity({ type: types.event, text: "Adicionar Centro de Resultado", idBtnSearch: guid(), issue: 0, visible: ko.observable(true), enable: ko.observable(true) });
    this.ImportarCentroResultado = PropertyEntity({
        type: types.local,
        text: "Importar",
        visible: ko.observable(obterVisibilidadeImportarVeiculo()),
        accept: ".xls,.xlsx,.csv",
        cssClass: "btn-default botaoDentroSmartAdmimForm",
        UrlImportacao: "RateioDespesaVeiculo/ObterDadosImportacaoCentroResultado",
        UrlConfiguracao: "RateioDespesaVeiculo/ConfiguracaoImportacaoCentroResultado",
        CallbackImportacao: function (arg) {
            const retorno = arg.Data.Retorno;

            ModificarValorGeral(retorno.Total);
            _gridCentroResultadoDespesa.CarregarGrid(converterListaEmGridEditavel(retorno.ListaCentroResultado));
            ControlarVisibilidadeSegmentoEVeiculo();
        },
        FecharModalSeSucesso: true
    });


    this.RatearDespesaUmaVezPorMes.val.subscribe(function (novoValor) {
        if (novoValor) {
            _rateioDespesa.DiaMesRateio.visible(true);
            _rateioDespesa.DiaMesRateio.required(true);
        } else {
            _rateioDespesa.DiaMesRateio.visible(false);
            _rateioDespesa.DiaMesRateio.required(false);
        }
    });

    this.AlterarCentroResultadoRateio.val.subscribe(function (novoValor) {
        if (novoValor) {
            _rateioDespesa.CentroResultadoRateio.required(true);
        } else {
            _rateioDespesa.CentroResultadoRateio.required(false);
        }
    });
};

var DetalheRateioDespesa = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });

    this.Grid = PropertyEntity({ type: types.local });
};

var PesquisaRateioDespesa = function () {
    this.DataInicial = PropertyEntity({ text: "Data Inicial:", getType: typesKnockout.date, val: ko.observable(""), def: "" });
    this.DataFinal = PropertyEntity({ text: "Data Final:", getType: typesKnockout.date, val: ko.observable(""), def: "" });
    this.ValorInicial = PropertyEntity({ text: "Valor Inicial:", getType: typesKnockout.decimal, val: ko.observable(""), def: "" });
    this.ValorFinal = PropertyEntity({ text: "Valor Final:", getType: typesKnockout.decimal, val: ko.observable(""), def: "" });

    this.Pessoa = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Pessoa:", idBtnSearch: guid(), issue: 0, visible: ko.observable(true), required: false, enable: ko.observable(true) });
    this.Colaborador = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Colaborador:", idBtnSearch: guid(), issue: 0, visible: ko.observable(true), required: false, enable: ko.observable(true) });
    this.NumeroDocumento = PropertyEntity({ text: "*Nº do Documento:", required: true, getType: typesKnockout.string, val: ko.observable(""), def: "", maxlength: 50, enable: ko.observable(true) });
    this.TipoDespesa = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Tipo de Despesa:", idBtnSearch: guid() });
    this.Veiculo = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Veículo:", idBtnSearch: guid() });
    this.SegmentoVeiculo = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Segmento de Veículo:", idBtnSearch: guid(), visible: ko.observable(true) });
    this.CentroResultado = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Centro de Resultado:", idBtnSearch: guid(), visible: ko.observable(false) });

    this.ExibirFiltros = PropertyEntity({ eventClick: ExibirFiltrosClick, type: types.event, text: "Filtros de Pesquisa", idFade: guid(), visibleFade: ko.observable(false), visible: ko.observable(true) });
    this.Pesquisar = PropertyEntity({ eventClick: RecarregarGridRateioDespesa, type: types.event, text: "Pesquisar", idGrid: guid(), visible: ko.observable(true) });
};

/*
 * Declaração das Funções de Inicialização
 */

function carregarDespesaVeiculo(idDivConteudo, callback) {
    $.get("Content/Static/Financeiro/DespesaVeiculo.html?dyn=" + guid(), function (dataConteudo) {
        $("#" + idDivConteudo).html(dataConteudo);

        _rateioDespesa = new RateioDespesa();
        KoBindings(_rateioDespesa, "knockoutRateioDespesa");

        _lancamentoRateioDespesa = new DetalheRateioDespesa();
        KoBindings(_lancamentoRateioDespesa, "knockoutRateioDespesaEntreVeiculos");

        _CRUDRateioDespesa = new CRUDRateioDespesa();
        KoBindings(_CRUDRateioDespesa, "knockoutCRUDRateioDespesa");

        BuscarTipoDespesaFinanceira(_rateioDespesa.TipoDespesa);

        new BuscarClientes(_rateioDespesa.Pessoa);
        new BuscarFuncionario(_rateioDespesa.Colaborador);


        loadGridVeiculosECentroResultadoPorConfiguracao();

        LoadGridSegmentoVeiculo();
        LoadGridLancamentoRateioDespesaEntreVeiculos();

        _rateioDespesa.CentroResultado.visible(true);
        _rateioDespesa.SegmentoVeiculo.visible(false);

        _rateioDespesa.CentroResultado.visible(false);
        _rateioDespesa.SegmentoVeiculo.visible(true);
        if (_CONFIGURACAO_TMS.UtilizarCentroResultadoNoRateioDespesaVeiculo) {
            _rateioDespesa.CentroResultado.visible(true);
            _rateioDespesa.SegmentoVeiculo.visible(false);
        }

        if (callback !== undefined && callback !== null)
            callback();
    });
}

function LoadGridRateioDespesa() {
    var opcaoEditar = { descricao: "Editar", id: guid(), evento: "onclick", metodo: EditarClick, tamanho: "10", icone: "" };
    var menuOpcoes = { tipo: TypeOptionMenu.link, opcoes: [opcaoEditar] };
    var configuracoesExportacao = { url: "RateioDespesaVeiculo/ExportarPesquisa", titulo: "Rateio de Despesas" };

    _gridRateioDespesa = new GridViewExportacao(_pesquisaRateioDespesa.Pesquisar.idGrid, "RateioDespesaVeiculo/Pesquisa", _pesquisaRateioDespesa, menuOpcoes, configuracoesExportacao, null);
    _gridRateioDespesa.CarregarGrid();
}

function LoadGridLancamentoRateioDespesaEntreVeiculos() {
    var opcaoEditar = { descricao: "Detalhes", id: guid(), evento: "onclick", metodo: DetalhesRateioEntreVeiculosClick, tamanho: "10", icone: "" };
    var menuOpcoes = { tipo: TypeOptionMenu.link, opcoes: [opcaoEditar] };
    var configuracoesExportacao = { url: "RateioDespesaVeiculo/ExportarPesquisaLancamentoVeiculo", titulo: "Rateio de Despesas entre os Veículos" };

    _gridRateioDespesaEntreVeiculos = new GridViewExportacao(_lancamentoRateioDespesa.Grid.id, "RateioDespesaVeiculo/PesquisaLancamentoVeiculo", _lancamentoRateioDespesa, menuOpcoes, configuracoesExportacao, null, 10);
}

function LoadGridDetalhesLancamentoRateioDespesaEntreVeiculos() {
    var menuOpcoes = null;
    var configuracoesExportacao = { url: "RateioDespesaVeiculo/ExportarPesquisaDetalhadaLancamentoVeiculo", titulo: "Detalhes do Rateio de Despesas do Veículo" };
    var configuracoesEditarColuna = { permite: true, callback: EditarValorDetalheDespesaVeiculo, atualizarRow: false };

    _gridDetalheRateioDespesaEntreVeiculos = new GridViewExportacao(_detalheLancamentoRateioDespesa.Grid.id, "RateioDespesaVeiculo/PesquisaDetalhadaLancamentoVeiculo", _detalheLancamentoRateioDespesa, menuOpcoes, configuracoesExportacao, null, 20, null, null, configuracoesEditarColuna);
}

function LoadRateioDespesa() {
    carregarDespesaVeiculo("conteudoDespesaVeiculo", LoadPesquisaRateioDespesaVeiculo);
}

function LoadPesquisaRateioDespesaVeiculo() {
    HeaderAuditoria("RateioDespesaVeiculo", _rateioDespesa);
    _detalheLancamentoRateioDespesa = new DetalheRateioDespesa();
    KoBindings(_detalheLancamentoRateioDespesa, "divModalDetalheLancamentoDespesaVeiculo");

    _pesquisaRateioDespesa = new PesquisaRateioDespesa();
    KoBindings(_pesquisaRateioDespesa, "knockoutPesquisaRateioDespesa", false, _pesquisaRateioDespesa.Pesquisar.id);

    BuscarTipoDespesaFinanceira(_pesquisaRateioDespesa.TipoDespesa);
    BuscarVeiculos(_pesquisaRateioDespesa.Veiculo);
    BuscarSegmentoVeiculo(_pesquisaRateioDespesa.SegmentoVeiculo);
    BuscarCentroResultado(_pesquisaRateioDespesa.CentroResultado);
    BuscarClientes(_pesquisaRateioDespesa.Pessoa);
    BuscarFuncionario(_pesquisaRateioDespesa.Colaborador);

    BuscarCentroResultado(_rateioDespesa.CentroResultadoRateio);

    _rateioDespesa.CentroResultadoRateio.visible(_CONFIGURACAO_TMS.UtilizarCentroResultadoNoRateioDespesaVeiculo);
    _pesquisaRateioDespesa.CentroResultado.visible(_CONFIGURACAO_TMS.UtilizarCentroResultadoNoRateioDespesaVeiculo);
    _pesquisaRateioDespesa.SegmentoVeiculo.visible(!_CONFIGURACAO_TMS.UtilizarCentroResultadoNoRateioDespesaVeiculo);

    LoadGridRateioDespesa();
    LoadGridDetalhesLancamentoRateioDespesaEntreVeiculos();
}

function LoadGridVeiculosDespesa() {
    var menuOpcoes = {
        tipo: TypeOptionMenu.link, tamanho: 7, opcoes: [{
            descricao: "Excluir", id: guid(), metodo: function (data) {
                ExcluirVeiculoDespesaClick(_rateioDespesa.Veiculo, data);
            }
        }]
    };

    var header = [
        { data: "Codigo", visible: false },
        { data: "Placa", title: "Placa", width: "12%" },
        { data: "NumeroFrota", title: "Nº Frota", width: "12%" },
        { data: "ModeloVeicularCarga", title: "Modelo Veicular", width: "33%" },
        { data: "SegmentoVeiculo", title: "Segmento do Veículo", width: "33%" }
    ];

    _gridVeiculoDespesa = new BasicDataTable(_rateioDespesa.GridVeiculosDespesa.id, header, menuOpcoes, { column: 1, dir: orderDir.asc });

    new BuscarVeiculos(_rateioDespesa.Veiculo, null, null, null, null, null, null, null, null, null, null, null, null, _gridVeiculoDespesa, null, function () { ControlarVisibilidadeSegmentoEVeiculo(); });

    _rateioDespesa.Veiculo.basicTable = _gridVeiculoDespesa;

    RecarregarGridVeiculoDespesa();
}

function LoadGridSegmentoVeiculo() {
    var menuOpcoes = {
        tipo: TypeOptionMenu.link, tamanho: 7, opcoes: [{
            descricao: "Excluir", id: guid(), metodo: function (data) {
                ExcluirSegmentoVeiculoClick(_rateioDespesa.SegmentoVeiculo, data);
            }
        }]
    };

    var header = [
        { data: "Codigo", visible: false },
        { data: "Descricao", title: "Descrição", width: "90%" }
    ];

    _gridSegmentoVeiculoDespesa = new BasicDataTable(_rateioDespesa.GridSegmentoVeiculoDespesa.id, header, menuOpcoes, { column: 1, dir: orderDir.asc });

    new BuscarSegmentoVeiculo(_rateioDespesa.SegmentoVeiculo, null, null, null, _gridSegmentoVeiculoDespesa, function () { ControlarVisibilidadeSegmentoEVeiculo(); });

    _rateioDespesa.SegmentoVeiculo.basicTable = _gridSegmentoVeiculoDespesa;

    RecarregarGridSegmentoVeiculoDespesa();
}

function LoadGridCentroResultado() {
    var menuOpcoes = {
        tipo: TypeOptionMenu.link, tamanho: 7, opcoes: [{
            descricao: "Excluir", id: guid(), metodo: function (data) {
                ExcluirCentroResultadoClick(_rateioDespesa.CentroResultado, data);
            }
        }]
    };

    var header = [
        { data: "Codigo", visible: false },
        { data: "Descricao", title: "Descrição", width: "70%" },
        { data: "Plano", title: "Número do centro", width: "20%" }
    ];

    _gridCentroResultadoDespesa = new BasicDataTable(_rateioDespesa.GridCentroResultadoDespesa.id, header, menuOpcoes, { column: 1, dir: orderDir.asc });

    new BuscarCentroResultado(_rateioDespesa.CentroResultado, null, null, null, null, null, _gridCentroResultadoDespesa, function () { ControlarVisibilidadeSegmentoEVeiculo(); });

    _rateioDespesa.CentroResultado.basicTable = _gridCentroResultadoDespesa;

    RecarregarGridCentroResultadoDespesa();
}

/*
 * Declaração das Funções Associadas a Eventos
 */

function DetalhesRateioEntreVeiculosClick(data) {
    _detalheLancamentoRateioDespesa.Codigo.val(data.Codigo);
    _gridDetalheRateioDespesaEntreVeiculos.CarregarGrid();
    Global.abrirModal('divModalDetalheLancamentoDespesaVeiculo');
}

function AdicionarDespesaClick(e, sender) {
    PreencherListasSelecaoDespesa();

    Salvar(_rateioDespesa, "RateioDespesaVeiculo/Adicionar", function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Cadastrado com sucesso.");

                if (!retorno.Data.AdicionadoAutomaticamente) {
                    LimparCamposRateioDespesa();
                    RecarregarGridRateioDespesa();
                } else {
                    LimparCamposRateioDespesa(true);
                    Global.fecharModal('divModalDespesaVeiculo');
                }
            }
            else
                exibirMensagem(tipoMensagem.aviso, "Aviso", retorno.Msg);
        }
        else
            exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
    }, sender);
}

function CancelarDespesaClick() {
    LimparCamposRateioDespesa();
}

function EditarClick(registroSelecionado) {
    LimparCamposRateioDespesa();

    _rateioDespesa.Codigo.val(registroSelecionado.Codigo);

    BuscarPorCodigo(_rateioDespesa, "RateioDespesaVeiculo/BuscarPorCodigo", function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                _pesquisaRateioDespesa.ExibirFiltros.visibleFade(false);

                _rateioDespesa.AlterarCentroResultadoRateio.val(retorno.Data.CentroResultadoRateio.Codigo > 0);

                ControlarBotoesHabilitadosDespesa(true);

                SetarEnableCamposKnockout(_rateioDespesa, false);

                _gridSegmentoVeiculoDespesa.DesabilitarOpcoes();
                _gridVeiculoDespesa.DesabilitarOpcoes();
                _gridCentroResultadoDespesa.DesabilitarOpcoes();

                _lancamentoRateioDespesa.Codigo.val(_rateioDespesa.Codigo.val());
                _gridRateioDespesaEntreVeiculos.CarregarGrid();

                $("#liTabRateio").show();

                RecarregarGridSegmentoVeiculoDespesa();

                if (!_CONFIGURACAO_TMS.UtilizarValorDesproporcionalRateioDespesaVeiculo) {
                    RecarregarGridVeiculoDespesa();
                    RecarregarGridCentroResultadoDespesa();

                }
                if (_CONFIGURACAO_TMS.UtilizarValorDesproporcionalRateioDespesaVeiculo) {
                    if (retorno.Data.VeiculosDespesa.length > 0)
                        RecarregarGridModificavel('Veiculo');
                    if (retorno.Data.CentrosResultadoDespesa.length > 0) {
                        _rateioDespesa.ce
                        RecarregarGridModificavel('CentroResultado');

                    }
                }

            }
            else
                exibirMensagem(tipoMensagem.aviso, "Aviso", retorno.Msg);
        }
        else
            exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
    }, null);
}

function ExcluirDespesaClick() {
    exibirConfirmacao("Confirmação", "Realmente deseja excluir este registro?", function () {
        ExcluirPorCodigo(_rateioDespesa, "RateioDespesaVeiculo/ExcluirPorCodigo", function (retorno) {
            if (retorno.Success) {
                if (retorno.Data) {
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Excluído com sucesso");

                    RecarregarGridRateioDespesa();
                    LimparCamposRateioDespesa();
                }
                else
                    exibirMensagem(tipoMensagem.aviso, "Sugestão", retorno.Msg, 16000);
            }
            else
                exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
        }, null);
    });
}

function ExibirFiltrosClick(e) {
    e.ExibirFiltros.visibleFade(!e.ExibirFiltros.visibleFade());
}

function EditarValorDetalheDespesaVeiculo(dataRow, row, head, callbackTabPress) {
    var data = { Codigo: dataRow.Codigo, Valor: dataRow.Valor };
    executarReST("RateioDespesaVeiculo/AlterarValorRateioDespesaVeiculoLancamentoDia", data, function (r) {
        if (r.Success) {
            if (r.Data !== false) {
                CompararEAtualizarGridEditableDataRow(dataRow, r.Data.Registro);

                _rateioDespesa.Valor.val(r.Data.ValorTotalRateio);

                _gridDetalheRateioDespesaEntreVeiculos.AtualizarDataRow(row, dataRow, callbackTabPress);
                _gridRateioDespesaEntreVeiculos.CarregarGrid();
            } else {
                _gridDetalheRateioDespesaEntreVeiculos.DesfazerAlteracaoDataRow(row);
                exibirMensagem(tipoMensagem.aviso, "Atenção!", r.Msg);
            }
        } else {
            _gridDetalheRateioDespesaEntreVeiculos.DesfazerAlteracaoDataRow(row);
            exibirMensagem(tipoMensagem.falha, "Falha!", r.Msg);
        }
    });
}

/*
 * Declaração das Funções
 */

function PreencherListasSelecaoDespesa() {
    var veiculosSelecao = new Array();
    var segmentosSelecao = new Array();
    var centrosResultadoSelecao = new Array();

    $.each(_rateioDespesa.Veiculo.basicTable.BuscarRegistros(), function (i, veiculo) {
        veiculosSelecao.push({
            Codigo: veiculo.Codigo,
            Valor: veiculo.Valor
        });
    });

    $.each(_rateioDespesa.SegmentoVeiculo.basicTable.BuscarRegistros(), function (i, veiculo) {
        segmentosSelecao.push(veiculo.Codigo);
    });

    $.each(_rateioDespesa.CentroResultado.basicTable.BuscarRegistros(), function (i, centro) {
        centrosResultadoSelecao.push({
            Codigo: centro.Codigo,
            Valor: centro.Valor
        });
    });

    _rateioDespesa.VeiculosDespesa.val(JSON.stringify(veiculosSelecao));
    _rateioDespesa.SegmentosVeiculosDespesa.val(JSON.stringify(segmentosSelecao));
    _rateioDespesa.CentrosResultadoDespesa.val(JSON.stringify(centrosResultadoSelecao));
}

function ControlarBotoesHabilitadosDespesa(isEdicao) {
    _CRUDRateioDespesa.Excluir.visible(isEdicao);
    _CRUDRateioDespesa.Cancelar.visible(isEdicao);
    _CRUDRateioDespesa.Adicionar.visible(!isEdicao);
}

function LimparCamposRateioDespesa(naoRecetarAbas) {
    ControlarBotoesHabilitadosDespesa(false);
    LimparCampos(_rateioDespesa);

    if (naoRecetarAbas === undefined || naoRecetarAbas === null || naoRecetarAbas === false) {
        $("#liTabRateio").hide();
        Global.ResetarMultiplasAbas();
    }

    SetarEnableCamposKnockout(_rateioDespesa, true);

    _gridSegmentoVeiculoDespesa.HabilitarOpcoes();
    _gridVeiculoDespesa.HabilitarOpcoes();
    _gridCentroResultadoDespesa.HabilitarOpcoes();

    RecarregarGridVeiculoDespesa();
    RecarregarGridSegmentoVeiculoDespesa();
    RecarregarGridCentroResultadoDespesa();
}

function RecarregarGridRateioDespesa() {
    _gridRateioDespesa.CarregarGrid();
}

function RecarregarGridVeiculoDespesa() {
    var data = new Array();

    if (!string.IsNullOrWhiteSpace(_rateioDespesa.VeiculosDespesa.val())) {
        $.each(_rateioDespesa.VeiculosDespesa.val(), function (i, veiculo) {
            var veiculoGrid = new Object();

            veiculoGrid.Codigo = veiculo.Codigo;
            veiculoGrid.Placa = veiculo.Placa;
            veiculoGrid.ModeloVeicularCarga = veiculo.ModeloVeicularCarga;
            veiculoGrid.SegmentoVeiculo = veiculo.SegmentoVeiculo;
            veiculoGrid.NumeroFrota = veiculo.NumeroFrota;

            data.push(veiculoGrid);
        });
    }

    _gridVeiculoDespesa.CarregarGrid(data);

    ControlarVisibilidadeSegmentoEVeiculo();
}

function ExcluirVeiculoDespesaClick(knoutVeiculo, data) {
    var veiculosGrid = knoutVeiculo.basicTable.BuscarRegistros();

    for (var i = 0; i < veiculosGrid.length; i++) {
        if (data.Codigo == veiculosGrid[i].Codigo) {
            veiculosGrid.splice(i, 1);
            break;
        }
    }

    knoutVeiculo.basicTable.CarregarGrid(veiculosGrid);

    ControlarVisibilidadeSegmentoEVeiculo();
}

function RecarregarGridSegmentoVeiculoDespesa() {
    var data = new Array();

    if (!string.IsNullOrWhiteSpace(_rateioDespesa.SegmentosVeiculosDespesa.val())) {
        $.each(_rateioDespesa.SegmentosVeiculosDespesa.val(), function (i, segmentoVeiculo) {
            var segmentoVeiculoGrid = new Object();

            segmentoVeiculoGrid.Codigo = segmentoVeiculo.Codigo;
            segmentoVeiculoGrid.Descricao = segmentoVeiculo.Descricao;

            data.push(segmentoVeiculoGrid);
        });
    }

    _gridSegmentoVeiculoDespesa.CarregarGrid(data);

    ControlarVisibilidadeSegmentoEVeiculo();
}

function ExcluirSegmentoVeiculoClick(knoutSegmentoVeiculo, data) {
    var segmentosVeiculosGrid = knoutSegmentoVeiculo.basicTable.BuscarRegistros();

    for (var i = 0; i < segmentosVeiculosGrid.length; i++) {
        if (data.Codigo == segmentosVeiculosGrid[i].Codigo) {
            segmentosVeiculosGrid.splice(i, 1);
            break;
        }
    }

    knoutSegmentoVeiculo.basicTable.CarregarGrid(segmentosVeiculosGrid);

    ControlarVisibilidadeSegmentoEVeiculo();
}

function RecarregarGridCentroResultadoDespesa() {
    var data = new Array();

    if (!string.IsNullOrWhiteSpace(_rateioDespesa.CentrosResultadoDespesa.val())) {
        $.each(_rateioDespesa.CentrosResultadoDespesa.val(), function (i, centroResultado) {
            var centroResultadoGrid = new Object();

            centroResultadoGrid.Codigo = centroResultado.Codigo;
            centroResultadoGrid.Descricao = centroResultado.Descricao;
            centroResultadoGrid.Plano = centroResultado.Plano;

            data.push(centroResultadoGrid);
        });
    }

    _gridCentroResultadoDespesa.CarregarGrid(data);

    ControlarVisibilidadeSegmentoEVeiculo();
}

function ExcluirCentroResultadoClick(knoutCentroResultado, data) {
    var centrosResultadoGrid = knoutCentroResultado.basicTable.BuscarRegistros();

    for (var i = 0; i < centrosResultadoGrid.length; i++) {
        if (data.Codigo == centrosResultadoGrid[i].Codigo) {
            centrosResultadoGrid.splice(i, 1);
            break;
        }
    }

    knoutCentroResultado.basicTable.CarregarGrid(centrosResultadoGrid);

    ControlarVisibilidadeSegmentoEVeiculo();
}

function ControlarVisibilidadeSegmentoEVeiculo() {
    var segmentosVeiculosGrid = _rateioDespesa.SegmentoVeiculo.basicTable != null ? _rateioDespesa.SegmentoVeiculo.basicTable.BuscarRegistros() : new Array();
    var veiculosGrid = _rateioDespesa.Veiculo.basicTable != null ? _rateioDespesa.Veiculo.basicTable.BuscarRegistros() : new Array();
    var centrosResultadoGrid = _rateioDespesa.CentroResultado.basicTable != null ? _rateioDespesa.CentroResultado.basicTable.BuscarRegistros() : new Array();

    if (segmentosVeiculosGrid.length > 0 || centrosResultadoGrid.length > 0)
        _rateioDespesa.Veiculo.visible(false);
    else
        _rateioDespesa.Veiculo.visible(true);

    if (veiculosGrid.length > 0) {
        _rateioDespesa.SegmentoVeiculo.visible(false);
        _rateioDespesa.CentroResultado.visible(false);
    }
    else {
        if (_CONFIGURACAO_TMS.UtilizarCentroResultadoNoRateioDespesaVeiculo)
            _rateioDespesa.CentroResultado.visible(true);
        else
            _rateioDespesa.SegmentoVeiculo.visible(true);
    }
}


function loadGridVeiculosECentroResultadoPorConfiguracao() {
    if (_CONFIGURACAO_TMS.UtilizarValorDesproporcionalRateioDespesaVeiculo) {
        LoadGridVeiculosModificavel();
        LoadGridCentroResultadoModificavel();

        return;
    }

    LoadGridCentroResultado();
    LoadGridVeiculosDespesa();

}

const obterVisibilidadeImportarVeiculo = () => _CONFIGURACAO_TMS.UtilizarValorDesproporcionalRateioDespesaVeiculo;


function converterListaEmGridEditavel(lista) {
    for (var i = 0; i < lista.length; i++) {
        lista[i].DT_Enable = true;
        lista[i].DT_FontColor = "";
        lista[i].DT_RowId = guid();
    }
    return lista;
}