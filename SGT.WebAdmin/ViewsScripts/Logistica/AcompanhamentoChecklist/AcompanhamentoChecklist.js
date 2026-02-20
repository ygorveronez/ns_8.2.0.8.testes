/// <reference path="../../../js/libs/jquery-2.1.1.js" />
/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/knockout-3.1.0.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../wwwroot/js/Global/Grid.js" />
/// <reference path="../../../js/bootstrap/bootstrap.js" />
/// <reference path="../../../js/libs/jquery.blockui.js" />
/// <reference path="../../../js/Global/knoutViewsSlides.js" />
/// <reference path="../../../js/libs/jquery.maskMoney.js" />
/// <reference path="../../../js/plugin/datatables/jquery.dataTables.js" />
/// <reference path="../../../../../ViewsScripts/Consultas/Carga.js/" />
/// <reference path="AnexosAcompanhamentoChecklist.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _pesquisaAcompanhamentoChecklist, _acompanhamentoChecklist, detalhesChecklist ;

var PesquisaAcompanhamentoChecklist = function () {
    this.Filial = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), required: false, text: "Filial", idBtnSearch: guid(), enable: ko.observable(true), visible: ko.observable(true) });
    this.TipoOperacao = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Tipo Operação", idBtnSearch: guid(), visible: ko.observable(true) });
    this.DataCarregamento = PropertyEntity({ text: "Data de Carregamento", getType: typesKnockout.date });
    this.Transportador = PropertyEntity({ text: "Transportador", type: types.entity, required: ko.observable(true), codEntity: ko.observable(0), idBtnSearch: guid() });
    this.Situacao = PropertyEntity({ text: "Situação", getType: typesKnockout.select, val: ko.observable("Todos"), options: Global.ObterOpcoesPesquisaBooleano("Lido", "Não Lido"), def: "", visible: ko.observable(true) });
    this.Carga = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: "Carga", idBtnSearch: guid() });

    this.Pesquisar = PropertyEntity({
        eventClick: function (e) {
            _gridAcompanhamentoChecklist.CarregarGrid();
        }, type: types.event, text: "Pesquisar", idGrid: guid(), visible: ko.observable(true)
    });

    this.ExibirFiltros = PropertyEntity({
        eventClick: function (e) {
            if (e.ExibirFiltros.visibleFade() == true) {
                e.ExibirFiltros.visibleFade(false);
            } else {
                e.ExibirFiltros.visibleFade(true);
            }
        }, type: types.event, text: "Filtros de Pesquisa", idFade: guid(), visibleFade: ko.observable(true), visible: ko.observable(true)
    });
}

var AcompanhamentoChecklistVeiculo = function (titulo) {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int, id: guid() });
    this.CodigoVeiculo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.CodigoJanelaCarregamentoTransportador = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.ChecklistVeiculo1 = PropertyEntity({ val: ko.observable(""), def: ko.observable(""), visible: ko.observable(false) });
    this.ChecklistVeiculo2 = PropertyEntity({ val: ko.observable(""), def: ko.observable(""), visible: ko.observable(false) });
    this.ChecklistVeiculo3 = PropertyEntity({ val: ko.observable(""), def: ko.observable(""), visible: ko.observable(false) });
};

var ChecklistCargaAcompanhamento = function (titulo) {
    this.CodigoChecklist = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Titulo = PropertyEntity({ text: titulo });
    this.DataChecklist = PropertyEntity({ text: "Data do Checklist", val: ko.observable(), getType: typesKnockout.date, enable: ko.observable(false), required: true });
    this.RegimeLimpeza = PropertyEntity({ getType: typesKnockout.select, val: ko.observable(EnumRegimeLimpeza.Nenhum), text: "Regime de Limpeza", options: EnumRegimeLimpeza.obterOpcoes(), def: EnumRegimeLimpeza.Nenhum, enable: ko.observable(false), required: true });
    this.GrupoProduto = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Grupo do Produto", idBtnSearch: guid(), enable: ko.observable(false), required: true });
    this.OrdemCargaChecklist = PropertyEntity({ getType: typesKnockout.select, val: ko.observable(1), options: EnumOrdemCargaChecklist.obterOpcoes(), def: -1 });
    this.Anexos = PropertyEntity({ eventClick: AcompanhamentoAnexosChecklistClick, type: types.event, text: "Anexos", val: ko.observableArray() });
}

var DetalhesChecklist = function () {
    this.CodigoJanelaCarregamentoTransportador = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int, id: guid() });
    this.CodigoCarga = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
}

//*******EVENTOS*******

function loadAcompanhamentoChecklist    () {
    _pesquisaAcompanhamentoChecklist = new PesquisaAcompanhamentoChecklist();
    _detalhesChecklist = new DetalhesChecklist()

    KoBindings(_pesquisaAcompanhamentoChecklist, "knockoutPesquisaAcompanhamentoChecklist", false, _pesquisaAcompanhamentoChecklist.Pesquisar.id);

    BuscarFilial(_pesquisaAcompanhamentoChecklist.Filial);
    BuscarTransportadores(_pesquisaAcompanhamentoChecklist.Transportador);
    BuscarTiposOperacao(_pesquisaAcompanhamentoChecklist.TipoOperacao);
    BuscarCargas(_pesquisaAcompanhamentoChecklist.Carga);
    CarregarAcompanhamentoChecklist();
    loadAcompanhamentoChecklistVeiculo();
}

function loadAcompanhamentoChecklistVeiculo() {
    _cadastroChecklistVeiculo = new AcompanhamentoChecklistVeiculo();
    KoBindings(_cadastroChecklistVeiculo, "knockoutChecklist");
}

function ExibirChecklist(cargaJanelaTransportador) {
    confirmarEExibirChecklist(cargaJanelaTransportador);
}

function AcompanhamentoAnexosChecklistClick(e) {
    loadAcompanhamentoAnexosChecklist(e);
    Global.abrirModal('divModalGerenciarAnexosChecklist');
}

function limparCamposAcompanhamentoChecklistVeiculo() {
    LimparCampos(_cadastroChecklistVeiculo);

    _cadastroChecklistVeiculo.ChecklistVeiculo1.visible(false);
    _cadastroChecklistVeiculo.ChecklistVeiculo2.visible(false);
    _cadastroChecklistVeiculo.ChecklistVeiculo3.visible(false);

    $('.nav-tabs .active').removeClass('active');
    $('.tab-content .active').removeClass('active');
    $('.nav-tabs li:first-child a').addClass('active');
    $('.tab-content div:first-child').addClass('active');
}

function CarregarAcompanhamentoChecklist() {
    const limiteRegistros = 20;
    const totalRegistrosPorPagina = 20;

    const Opcoes = {
        descricao: "Visualizar Checklist",
        id: guid(),
        evento: "onclick",
        metodo: ExibirChecklist,
        tamanho: "15",
        icone: ""
    };

    const menuOpcoes = new Object();
    menuOpcoes.tipo = TypeOptionMenu.list;
    menuOpcoes.opcoes = new Array();
    menuOpcoes.opcoes.push(Opcoes);

    var configExportacao = {
        url: "AcompanhamentoChecklist/ExportarPesquisa",
        titulo: "Acompanhamento Checklist"
    };

    _gridAcompanhamentoChecklist = new GridView(
        'grid-acompanhamento-checklist',
        "AcompanhamentoChecklist/Pesquisar",
        _pesquisaAcompanhamentoChecklist,
        menuOpcoes,
        null,
        totalRegistrosPorPagina,
        null,
        false,
        false,
        undefined,
        limiteRegistros,
        null,
        configExportacao,
        null,
        null,
        null,
        callbackColumnDefaultGridChecklist
    );

    _gridAcompanhamentoChecklist.SetHabilitarExpansaoLinha(true);

    _gridAcompanhamentoChecklist.OnLinhaExpandida(function (rowData, openedRow) {
        const idTabela = guid();
        _detalhesChecklist.CodigoJanelaCarregamentoTransportador.val(rowData.Codigo);
        _detalhesChecklist.CodigoCarga.val(rowData.CodigoCarga);

        const gridDetalhes = new GridView(
            idTabela,
            "AcompanhamentoChecklist/PesquisarDetalhes",
            _detalhesChecklist,
            null,
            null,
            20
        );

        let html = `
            <table id="${idTabela}" width="100%" class="table table-bordered  table-hover" cellspacing="0"></table>
        `;

        gridDetalhes.CarregarGrid();
        return { html: html };
    });

    _gridAcompanhamentoChecklist.SetPermitirEdicaoColunas(true);
    _gridAcompanhamentoChecklist.SetSalvarPreferenciasGrid(true);
    _gridAcompanhamentoChecklist.CarregarGrid();
}

async function carregarChecklistVeiculo(cargaJanelaTransportador) {
    let data = { CodigoJanelaCarregamentoTransportador: cargaJanelaTransportador.Codigo };

    _cadastroChecklistVeiculo.CodigoJanelaCarregamentoTransportador.val(cargaJanelaTransportador);

    executarReST("CargaJanelaCarregamentoTransportadorChecklist/ObterDadosAcompanhamentoChecklist", data, async function (retorno) {
        if (retorno.Success) {
            let checklistData = retorno.Data.CargaJanelaCarregamentoTransportadorChecklist;
            let codigosVeiculos = retorno.Data.CodigosVeiculos;

            if (!checklistData || checklistData.length === 0) {
                exibirMensagem(tipoMensagem.atencao, "Não existe Checklist Gerado para as Placas da Carga", retorno.Msg);
                return;
            }

            for (let i = 0; i < codigosVeiculos.length; i++) {
                let codigoVeiculo = codigosVeiculos[i];
                let checklistsPorPlaca = checklistData.filter(checklist => checklist.CodigoVeiculo == codigoVeiculo);
                let tabIndex = i + 1;

                if (checklistsPorPlaca.length > 0) {
                    const checklistMap = {
                        1: _cadastroChecklistVeiculo.ChecklistVeiculo1,
                        2: _cadastroChecklistVeiculo.ChecklistVeiculo2,
                        3: _cadastroChecklistVeiculo.ChecklistVeiculo3
                    };

                    const checklist = checklistMap[tabIndex];

                    if (checklist) {
                        checklist.val(checklistsPorPlaca[0].Placa);
                        checklist.visible(true);
                    }
                }

                $(`#divChecklistCarga${tabIndex}`).empty();

                for (let j = 0; j < checklistsPorPlaca.length; j++) {
                    await adicionarCargaAcompanhamentoChecklist(checklistsPorPlaca[j], false, checklistsPorPlaca[j].OrdemCargaChecklist, tabIndex);
                }
            }

            Global.abrirModal('divModalAcompanhamentoChecklist');
        } else {
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, retorno.Msg);
        }
    });
}

async function adicionarCargaAcompanhamentoChecklist(checklist, enable, enumOrdem, tabIndex) {
    await $.get("Content/Static/Logistica/JanelaCarregamentoTransportador/ChecklistCarga.html?dyn=" + guid(), function (html) {
        let _checklistCarga = new ChecklistCargaAcompanhamento(EnumOrdemCargaChecklist.obterDescricao(enumOrdem));

        if (checklist) {
            PreencherObjetoKnout(_checklistCarga, { Data: checklist });
        } else {
            _checklistCarga.OrdemCargaChecklist.val(enumOrdem);
        }

        let knockoutGeracaoLaudoProduto = "knockoutCargaChecklistVeiculo";
        let knockoutGeracaoLaudoProdutoDinamico = knockoutGeracaoLaudoProduto + guid();

        html = html.replaceAll(knockoutGeracaoLaudoProduto, knockoutGeracaoLaudoProdutoDinamico);

        let divChecklistTarget = `#divChecklistCarga${tabIndex}`;

        $(divChecklistTarget).append(html);

        KoBindings(_checklistCarga, knockoutGeracaoLaudoProdutoDinamico);

        BuscarGruposProdutos(_checklistCarga.GrupoProduto, null, null, true);
        SetarEnableCamposKnockout(_checklistCarga, enable);
    });
}

function confirmarEExibirChecklist(cargaJanelaTransportador) {
    let data = { CodigoJanelaCarregamentoTransportador: cargaJanelaTransportador.Codigo, Situacao: cargaJanelaTransportador.Situacao };

    executarReST("AcompanhamentoChecklist/ConfirmarVisualizacaoChecklist", data, function (retorno) {
        if (retorno.Success) {
            _gridAcompanhamentoChecklist.CarregarGrid();
            limparCamposAcompanhamentoChecklistVeiculo();
            carregarChecklistVeiculo(cargaJanelaTransportador);
        }
        else {
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, retorno.Msg);
        }
    });
}

function callbackColumnDefaultGridChecklist(cabecalho, valorColuna, dadosLinha) {
    if (cabecalho.name !== "SituacaoFormatada") return valorColuna;

    let situacao = dadosLinha.Situacao;
    let corFundo = situacao ? "#66ff66" : "#ffff66";

    return `<span style="background-color: ${corFundo}; padding: 5px 10px; border-radius: 5px; display: inline-block;">${valorColuna}</span>`;
}
