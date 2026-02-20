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
/// <reference path="../../Enumeradores/EnumVeiculoEquipamento.js" />
/// <reference path="Servico.js" />
/// <reference path="MarcaVeiculo.js" />
/// <reference path="MarcaEquipamento.js" />
/// <reference path="ModeloVeiculo.js" />
/// <reference path="ModeloEquipamento.js" />

//#region Variáveis Globais

var _pesquisaGrupoServico;
var _grupoServico;
var _CRUDGrupoServico;
var _gridGrupoServico;

//#endregion

//#region Mapeamento Knockout 

var PesquisaGrupoServico = function () {
    this.Descricao = PropertyEntity({ text: "Descrição:", getType: typesKnockout.string, val: ko.observable("") });
    this.Status = PropertyEntity({ text: "Status: ", val: ko.observable(0), options: _statusPesquisa, def: 0 });

    this.ExibirFiltros = PropertyEntity({ eventClick: ExibirFiltrosClick, type: types.event, text: "Filtros de Pesquisa", idFade: guid(), visibleFade: ko.observable(false), visible: ko.observable(true) });
    this.Pesquisar = PropertyEntity({ eventClick: RecarregarGridGrupoServico, type: types.event, text: "Pesquisar", idGrid: guid(), visible: ko.observable(true) });
};

var GrupoServico = function () {
    this.Codigo = PropertyEntity({ getType: typesKnockout.int });
    this.Descricao = PropertyEntity({ text: "*Descrição:", required: true, maxlength: 50 });
    this.Status = PropertyEntity({ text: "*Status: ", val: ko.observable(true), options: _status, def: true, required: true });
    this.CodigoIntegracao = PropertyEntity({ text: "Código Integração:", val: ko.observable(""), def: "", visible: ko.observable(true), maxlength: 100 });
    this.Observacao = PropertyEntity({ text: "Observação:", required: false, getType: typesKnockout.string, maxlength: 300 });
    this.KmInicial = PropertyEntity({ text: "Km Inicial:", required: false, getType: typesKnockout.int });
    this.KmFinal = PropertyEntity({ text: "Km Final:", required: false, getType: typesKnockout.int });
    this.DiaInicial = PropertyEntity({ text: "Dia Inicial (Aquisição):", required: false, getType: typesKnockout.int });
    this.DiaFinal = PropertyEntity({ text: "Dia Final (Aquisição):", required: false, getType: typesKnockout.int });
    this.TipoVeiculoEquipamento = PropertyEntity({ text: "Tipo: ", val: ko.observable(EnumVeiculoEquipamento.Todos), options: EnumVeiculoEquipamento.obterOpcoesPesquisa(), def: EnumVeiculoEquipamento.Todos });

    this.ServicosVeiculo = PropertyEntity({ type: types.listEntity, list: new Array(), required: false, codEntity: ko.observable(0), idGrid: guid() });
    this.MarcasVeiculo = PropertyEntity({ type: types.map, getType: typesKnockout.dynamic, val: ko.observable(""), def: "", idGrid: guid() });
    this.MarcasEquipamento = PropertyEntity({ type: types.map, getType: typesKnockout.dynamic, val: ko.observable(""), def: "", idGrid: guid() });
    this.ModelosVeiculo = PropertyEntity({ type: types.map, getType: typesKnockout.dynamic, val: ko.observable(""), def: "", idGrid: guid() });
    this.ModelosEquipamento = PropertyEntity({ type: types.map, getType: typesKnockout.dynamic, val: ko.observable(""), def: "", idGrid: guid() });
    this.LocaisManutencao = PropertyEntity({ type: types.map, getType: typesKnockout.dynamic, val: ko.observable(""), def: "", idGrid: guid() });
    this.TipoOrdemServico = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Tipo Ordem Serviço:", idBtnSearch: guid(), enable: ko.observable(true), visible: ko.observable(true) });


    this.TipoVeiculoEquipamento.val.subscribe(function (novoValor) {
        $("#knockoutGrupoServicoMarcaVeiculo").show();
        $("#knockoutGrupoServicoMarcaEquipamento").show();
        $("#knockoutGrupoServicoModeloVeiculo").show();
        $("#knockoutGrupoServicoModeloEquipamento").show();

        if (novoValor === EnumVeiculoEquipamento.Veiculo) {
            LimparCamposMarcaEquipamento();
            LimparCamposModeloEquipamento();
            $("#knockoutGrupoServicoMarcaEquipamento").hide();
            $("#knockoutGrupoServicoModeloEquipamento").hide();
        } else if (novoValor === EnumVeiculoEquipamento.Equipamento) {
            LimparCamposMarcaVeiculo();
            LimparCamposModeloVeiculo();
            $("#knockoutGrupoServicoMarcaVeiculo").hide();
            $("#knockoutGrupoServicoModeloVeiculo").hide();
        }
    });
};

var CRUDGrupoServico = function () {
    this.Adicionar = PropertyEntity({ eventClick: AdicionarClick, type: types.event, text: "Adicionar", visible: ko.observable(true) });
    this.Atualizar = PropertyEntity({ eventClick: AtualizarClick, type: types.event, text: "Atualizar", visible: ko.observable(false) });
    this.Limpar = PropertyEntity({ eventClick: LimparClick, type: types.event, text: "Limpar", visible: ko.observable(false) });
    this.Excluir = PropertyEntity({ eventClick: ExcluirClick, type: types.event, text: "Excluir", visible: ko.observable(false) });
};

function LoadGrupoServico() {
    _pesquisaGrupoServico = new PesquisaGrupoServico();
    KoBindings(_pesquisaGrupoServico, "knockoutPesquisaGrupoServico", false, _pesquisaGrupoServico.Pesquisar.id);

    _grupoServico = new GrupoServico();
    KoBindings(_grupoServico, "knockoutGrupoServico");

    HeaderAuditoria("GrupoServico", _grupoServico);

    _CRUDGrupoServico = new CRUDGrupoServico();
    KoBindings(_CRUDGrupoServico, "knockoutCRUDGrupoServico");

    new BuscarTipoOrdemServico(_grupoServico.TipoOrdemServico);

    LoadGridGrupoServico();

    LoadServicoVeiculo();
    LoadMarcaVeiculo();
    LoadMarcaEquipamento();
    LoadModeloVeiculo();
    LoadModeloEquipamento();
    LoadLocalManutencao();
    loadGeolocalizacaoMotorista();
}

//#endregion

//#region Funções Load

function LoadGridGrupoServico() {
    var opcaoEditar = { descricao: "Editar", id: guid(), evento: "onclick", metodo: EditarClick, tamanho: "10", icone: "" };
    var menuOpcoes = { tipo: TypeOptionMenu.link, opcoes: [opcaoEditar] };
    var configuracoesExportacao = { url: "GrupoServico/ExportarPesquisa", titulo: "Grupo de Serviço" };

    _gridGrupoServico = new GridViewExportacao(_pesquisaGrupoServico.Pesquisar.idGrid, "GrupoServico/Pesquisa", _pesquisaGrupoServico, menuOpcoes, configuracoesExportacao);
    _gridGrupoServico.CarregarGrid();
}

//#endregion

//#region Funções Privadas

function ExibirFiltrosClick(e) {
    e.ExibirFiltros.visibleFade(!e.ExibirFiltros.visibleFade());
}

function RecarregarGridGrupoServico() {
    _gridGrupoServico.CarregarGrid();
}

function ControlarBotoesHabilitados(isEdicao) {
    _CRUDGrupoServico.Atualizar.visible(isEdicao);
    _CRUDGrupoServico.Excluir.visible(isEdicao);
    _CRUDGrupoServico.Limpar.visible(isEdicao);
    _CRUDGrupoServico.Adicionar.visible(!isEdicao);
}

//#endregion

//#region Funções Click

function EditarClick(registroSelecionado) {
    LimparCamposGrupoServico();

    _grupoServico.Codigo.val(registroSelecionado.Codigo);

    BuscarPorCodigo(_grupoServico, "GrupoServico/BuscarPorCodigo", function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                _pesquisaGrupoServico.ExibirFiltros.visibleFade(false);
                ControlarBotoesHabilitados(true);
                RecarregarGridServicoVeiculo();
                RecarregarGridMarcaVeiculo();
                RecarregarGridMarcaEquipamento();
                RecarregarGridModeloVeiculo();
                RecarregarGridModeloEquipamento();
                RecarregarGridLocalManutencao();
            }
            else
                exibirMensagem(tipoMensagem.aviso, "Aviso", retorno.Msg);
        }
        else
            exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
    }, null);
}

function AdicionarClick(e, sender) {
    preencherListasSelecao();
    Salvar(_grupoServico, "GrupoServico/Adicionar", function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Cadastrado com sucesso.");

                RecarregarGridGrupoServico();
                LimparCamposGrupoServico();
            }
            else
                exibirMensagem(tipoMensagem.aviso, "Aviso", retorno.Msg);
        }
        else
            exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
    }, sender);
}

function AtualizarClick(e, sender) {
    preencherListasSelecao();
    Salvar(_grupoServico, "GrupoServico/Atualizar", function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Atualizado com sucesso");

                RecarregarGridGrupoServico();
                LimparCamposGrupoServico();
            }
            else
                exibirMensagem(tipoMensagem.aviso, "Aviso", retorno.Msg);
        }
        else
            exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
    }, sender);
}

function preencherListasSelecao() {
    _grupoServico.MarcasVeiculo.val(JSON.stringify(_marcaVeiculo.MarcaVeiculo.basicTable.BuscarRegistros()));
    _grupoServico.MarcasEquipamento.val(JSON.stringify(_marcaEquipamento.MarcaEquipamento.basicTable.BuscarRegistros()));
    _grupoServico.ModelosVeiculo.val(JSON.stringify(_modeloVeiculo.ModeloVeiculo.basicTable.BuscarRegistros()));
    _grupoServico.ModelosEquipamento.val(JSON.stringify(_modeloEquipamento.ModeloEquipamento.basicTable.BuscarRegistros()));
    _grupoServico.LocaisManutencao.val(JSON.stringify(_gridLocalManutencao.BuscarRegistros()));
}

function LimparClick() {
    LimparCamposGrupoServico();
}

function ExcluirClick(e) {
    exibirConfirmacao("Confirmação", "Realmente deseja excluir esse cadastro?", function () {
        ExcluirPorCodigo(_grupoServico, "GrupoServico/ExcluirPorCodigo", function (retorno) {
            if (retorno.Success) {
                if (retorno.Data) {
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Excluído com sucesso");
                    RecarregarGridGrupoServico();
                    LimparCamposGrupoServico();
                }
                else
                    exibirMensagem(tipoMensagem.aviso, "Sugestão", retorno.Msg, 16000);
            }
            else
                exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
        }, null);
    });
}

function LimparCamposGrupoServico() {
    ControlarBotoesHabilitados(false);
    LimparCampos(_grupoServico);
    LimparCamposServicoVeiculo();
    Global.ResetarAbas();

    LimparCamposMarcaVeiculo();
    LimparCamposMarcaEquipamento();
    LimparCamposModeloVeiculo();
    LimparCamposModeloEquipamento();
    LimparCamposLocalManutencao();
}

//#endregion