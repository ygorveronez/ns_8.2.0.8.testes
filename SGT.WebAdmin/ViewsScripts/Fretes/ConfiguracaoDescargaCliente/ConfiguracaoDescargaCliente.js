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
/// <reference path="../../Consultas/Localidade.js" />
/// <reference path="../../Fretes/ConfiguracaoDescargaCliente/TipoOperacao.js" />
/// <reference path="../../Enumeradores/EnumSituacaoAjusteConfiguracaoDescargaCliente.js" />

//*******MAPEAMENTO KNOUCKOUT*******
var _pesquisaConfiguracaoDescargaCliente;
var _configuracaoDescargaCliente;
var _gridConfiguracaoDescargaCliente;

var PesquisaConfiguracaoDescargaCliente = function () {
    this.Ativo = PropertyEntity({ text: "Status: ", val: ko.observable(1), options: _statusPesquisa, def: 1 });
    this.Situacao = PropertyEntity({ text: "Situação: ", val: ko.observable(EnumSituacaoAjusteConfiguracaoDescargaCliente.Todos), options: EnumSituacaoAjusteConfiguracaoDescargaCliente.obterOpcoesPesquisa(), def: EnumSituacaoAjusteConfiguracaoDescargaCliente.Todos });
    this.Filial = PropertyEntity({ text: "Filial:", type: types.entity, codEntity: ko.observable(0), idBtnSearch: guid() });
    this.Cliente = PropertyEntity({ text: "Cliente:", type: types.entity, codEntity: ko.observable(0), idBtnSearch: guid() });
    this.SomenteVigentes = PropertyEntity({ val: ko.observable(false), getType: typesKnockout.bool, text: "Somente Vigentes", def: false, visible: ko.observable(true) });
    this.ModelosVeiculares = PropertyEntity({ text: "Modelos Veiculares:", type: types.multiplesEntities, codEntity: ko.observable(0), idBtnSearch: guid() });

    this.DataVigenciaInicial = PropertyEntity({ text: "Data Vigência Inicial: ", getType: typesKnockout.dateTime, val: ko.observable("") });
    this.DataVigenciaFinal = PropertyEntity({ text: "Data Vigência Final: ", getType: typesKnockout.dateTime, val: ko.observable("") });
    this.DataVigenciaInicial.dateRangeLimit = this.DataVigenciaFinal;
    this.DataVigenciaFinal.dateRangeInit = this.DataVigenciaInicial;

    this.Pesquisar = PropertyEntity({
        eventClick: function (e) {
            _gridConfiguracaoDescargaCliente.CarregarGrid();
        }, type: types.event, text: "Pesquisar", id: guid(), idGrid: guid(), visible: ko.observable(true)
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

var ConfiguracaoDescargaCliente = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Ativo = PropertyEntity({ val: ko.observable(true), options: _status, def: true, text: "*Status: " });
    this.Situacao = PropertyEntity({ text: "Situação", enable: ko.observable(false), val: ko.observable(EnumSituacaoAjusteConfiguracaoDescargaCliente.SemRegraAprovacao), def: EnumSituacaoAjusteConfiguracaoDescargaCliente.SemRegraAprovacao, options: EnumSituacaoAjusteConfiguracaoDescargaCliente.obterOpcoes() });
    this.Filial = PropertyEntity({ text: "Filial:", type: types.entity, codEntity: ko.observable(0), idBtnSearch: guid() });
    this.ModeloVeicular = PropertyEntity({ text: "Modelo Veicular:", type: types.entity, codEntity: ko.observable(0), idBtnSearch: guid() });
    this.TipoCarga = PropertyEntity({ text: "Tipo de Carga:", type: types.entity, codEntity: ko.observable(0), idBtnSearch: guid() });
    this.Valor = PropertyEntity({ text: "Valor:", required: false, type: types.map, getType: typesKnockout.decimal });
    this.ValorTonelada = PropertyEntity({ text: "Valor por Tonelada:", required: false, type: types.map, getType: typesKnockout.decimal });
    this.ValorUnidade = PropertyEntity({ text: "Valor por unidade:", required: false, type: types.map, getType: typesKnockout.decimal });
    this.ValorPallet = PropertyEntity({ text: "Valor por pallet:", required: false, type: types.map, getType: typesKnockout.decimal });
    this.ValorAjudante = PropertyEntity({ text: "Valor por ajudante:", required: false, type: types.map, getType: typesKnockout.decimal });
    this.InicioVigencia = PropertyEntity({ text: "Início da Vigência:", getType: typesKnockout.date, visible: _CONFIGURACAO_TMS.UtilizarVigenciaConfiguracaoDescargaCliente });
    this.FimVigencia = PropertyEntity({ text: "Fim da Vigência:", getType: typesKnockout.date, visible: _CONFIGURACAO_TMS.UtilizarVigenciaConfiguracaoDescargaCliente });
    this.ValorDePreDescarga = PropertyEntity({ val: ko.observable(false), getType: typesKnockout.bool, text: "Valor de Pré Descarga", def: false, visible: ko.observable(true) });
    this.ComponentePreDescarga = PropertyEntity({ text: "Componente Pré Descarga:", type: types.entity, codEntity: ko.observable(0), idBtnSearch: guid(), visible: ko.observable(false) });
    this.AdicionarCliente = PropertyEntity({ type: types.event, text: "Adicionar Clientes", idBtnSearch: guid() });
    this.AdicionarGrupoCliente = PropertyEntity({ type: types.event, text: "Adicionar Grupos de Clientes", idBtnSearch: guid() });
    this.Clientes = PropertyEntity({ type: types.val, idGrid: guid(), val: GetSetCliente, def: '[]' });
    this.GrupoClientes = PropertyEntity({ type: types.val, idGrid: guid(), val: GetSetGrupoCliente, def: '[]' });
    this.TiposOperacoes = PropertyEntity({ type: types.map, getType: typesKnockout.dynamic, val: ko.observable(""), def: "", idGrid: guid() });
    this.Transportadores = PropertyEntity({ type: types.map, getType: typesKnockout.dynamic, val: ko.observable(""), idGrid: guid(), text: "Adicionar Transportadores", idBtnSearch: guid() });

    this.Grid = PropertyEntity({ type: types.local });
    this.Tipo = PropertyEntity({ type: types.event, text: "Adicionar Tipos de Operação", idBtnSearch: guid() });

    this.Importar = PropertyEntity({
        type: types.local,
        text: "Importar",
        visible: ko.observable(true),
        accept: ".xls,.xlsx,.csv",
        cssClass: "btn-default",

        UrlImportacao: "ConfiguracaoDescargaCliente/Importar",
        UrlConfiguracao: "ConfiguracaoDescargaCliente/ConfiguracaoImportacao",
        CodigoControleImportacao: EnumCodigoControleImportacao.O062_ConfiguracaoDescargaCliente,
        CallbackImportacao: function () {
            _gridConfiguracaoDescargaCliente.CarregarGrid();
        }
    });

    this.Adicionar = PropertyEntity({ eventClick: adicionarClick, type: types.event, text: "Adicionar", visible: ko.observable(true) });
    this.Atualizar = PropertyEntity({ eventClick: atualizarClick, type: types.event, text: "Atualizar", visible: ko.observable(false) });
    this.Excluir = PropertyEntity({ eventClick: excluirClick, type: types.event, text: "Excluir", visible: ko.observable(false) });
    this.Cancelar = PropertyEntity({ eventClick: cancelarClick, type: types.event, text: "Cancelar", visible: ko.observable(false) });

    this.ValorDePreDescarga.val.subscribe(function (novoValor) {
        if (novoValor)
            _configuracaoDescargaCliente.ComponentePreDescarga.visible(true);
        else
            _configuracaoDescargaCliente.ComponentePreDescarga.visible(false);
    });
}

//*******EVENTOS*******
function loadConfiguracaoDescargaCliente() {

    _configuracaoDescargaCliente = new ConfiguracaoDescargaCliente();
    KoBindings(_configuracaoDescargaCliente, "knockoutConfiguracaoDescargaCliente");

    _pesquisaConfiguracaoDescargaCliente = new PesquisaConfiguracaoDescargaCliente();
    KoBindings(_pesquisaConfiguracaoDescargaCliente, "knockoutPesquisaConfiguracaoDescargaCliente", false, _pesquisaConfiguracaoDescargaCliente.Pesquisar.id);

    HeaderAuditoria("ConfiguracaoDescargaCliente");

    CarregarGrid();
    CarregarGridGrupoCliente();
    loadTipoOperacao();
    loadGridTransportadores();

    BuscarClientes(_pesquisaConfiguracaoDescargaCliente.Cliente);
    BuscarFilial(_pesquisaConfiguracaoDescargaCliente.Filial);
    BuscarModelosVeicularesCarga(_pesquisaConfiguracaoDescargaCliente.ModelosVeiculares);

    BuscarFilial(_configuracaoDescargaCliente.Filial);
    BuscarTiposdeCarga(_configuracaoDescargaCliente.TipoCarga);
    BuscarModelosVeicularesCarga(_configuracaoDescargaCliente.ModeloVeicular);
    BuscarClientes(_configuracaoDescargaCliente.AdicionarCliente, null, null, null, null, _gridConfiguracoes);
    BuscarGruposPessoas(_configuracaoDescargaCliente.AdicionarGrupoCliente, null, null, _gridGrupoCliente);

    BuscarComponentesDeFrete(_configuracaoDescargaCliente.ComponentePreDescarga);

    BuscarConfiguracaoDescargaCliente();
}

function adicionarClick(e, sender) {
    preencherListasSelecao();

    if (ValidarCamposConfiguracaoDescargaCliente()) {
        Salvar(e, "ConfiguracaoDescargaCliente/Adicionar", function (arg) {
            if (arg.Success) {
                if (arg.Data) {
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Cadastrado com sucesso");
                    _gridConfiguracaoDescargaCliente.CarregarGrid();
                    LimparCamposConfiguracaoDescargaCliente();
                } else {
                    exibirMensagem(tipoMensagem.atencao, "Atenção", arg.Msg);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
            }
        }, sender);
    }
}

function atualizarClick(e, sender) {
    preencherListasSelecao();

    if (ValidarCamposConfiguracaoDescargaCliente()) {
        Salvar(_configuracaoDescargaCliente, "ConfiguracaoDescargaCliente/Atualizar", function (arg) {
            if (arg.Success) {
                if (arg.Data) {
                    exibirMensagem(tipoMensagem.ok, "sucesso", "Atualizado com sucesso");
                    _gridConfiguracaoDescargaCliente.CarregarGrid();
                    LimparCamposConfiguracaoDescargaCliente();
                } else {
                    exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, "falha", arg.Msg);
            }
        }, sender);
    }
}

function excluirClick(e, sender) {
    exibirConfirmacao("Confirmação", "Realmente deseja excluir a configuração?", function () {
        ExcluirPorCodigo(_configuracaoDescargaCliente, "ConfiguracaoDescargaCliente/ExcluirPorCodigo", function (arg) {
            if (arg.Success) {
                if (arg.Data) {
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Excluído com sucesso");
                    _gridConfiguracaoDescargaCliente.CarregarGrid();
                    LimparCamposConfiguracaoDescargaCliente();
                } else {
                    exibirMensagem(tipoMensagem.aviso, "Sugestão", arg.Msg, 16000);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
            }
        }, null);
    });
}

function cancelarClick(e) {
    LimparCamposConfiguracaoDescargaCliente();
}

function BuscarConfiguracaoDescargaCliente() {
    var editar = { descricao: "Editar", id: "clasEditar", evento: "onclick", metodo: editarConfiguracaoPrioridade, tamanho: 5, icone: "" };
    var menuOpcoes = new Object();
    menuOpcoes.tipo = TypeOptionMenu.link;
    menuOpcoes.opcoes = new Array();
    menuOpcoes.opcoes.push(editar);

    var configExportacao = {
        url: "ConfiguracaoDescargaCliente/ExportarPesquisa",
        titulo: ""
    };

    _gridConfiguracaoDescargaCliente = new GridViewExportacao(_pesquisaConfiguracaoDescargaCliente.Pesquisar.idGrid, "ConfiguracaoDescargaCliente/Pesquisa", _pesquisaConfiguracaoDescargaCliente, menuOpcoes, configExportacao, null);
    _gridConfiguracaoDescargaCliente.CarregarGrid();
}

function editarConfiguracaoPrioridade(data) {
    LimparCamposConfiguracaoDescargaCliente();
    BuscarPorCodigo(data.Codigo);
}

//*******MÉTODOS*******

function BuscarPorCodigo(codigo) {
    executarReST("ConfiguracaoDescargaCliente/BuscarPorCodigo", { Codigo: codigo }, function (arg) {
        if (arg.Success) {
            if (arg.Data !== false) {
                PreencherObjetoKnout(_configuracaoDescargaCliente, { Data: arg.Data });

                _pesquisaConfiguracaoDescargaCliente.ExibirFiltros.visibleFade(false);
                _configuracaoDescargaCliente.Atualizar.visible(true);
                _configuracaoDescargaCliente.Cancelar.visible(true);
                _configuracaoDescargaCliente.Excluir.visible(true);
                _configuracaoDescargaCliente.Adicionar.visible(false);

                if (arg.Data.Clientes)
                    _gridConfiguracoes.CarregarGrid(arg.Data.Clientes);

                if (arg.Data.GrupoPessoas)
                    _gridGrupoCliente.CarregarGrid(arg.Data.GrupoPessoas);

                if (arg.Data.Transportadores)
                    _gridTransportador.CarregarGrid(arg.Data.Transportadores);

                recarregarGridTipoOperacao();
            }
            else
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    }, null);
}

function LimparCamposConfiguracaoDescargaCliente() {
    _gridConfiguracoes.CarregarGrid([]);
    _gridGrupoCliente.CarregarGrid([]);
    _gridTransportador.CarregarGrid([]);

    LimparCampos(_configuracaoDescargaCliente);
    limparCamposTipoOperacao();

    _configuracaoDescargaCliente.Atualizar.visible(false);
    _configuracaoDescargaCliente.Cancelar.visible(false);
    _configuracaoDescargaCliente.Excluir.visible(false);
    _configuracaoDescargaCliente.Adicionar.visible(true);

    Global.ResetarAbas();
}

function ValidarCamposConfiguracaoDescargaCliente() {
    if (_configuracaoDescargaCliente.Valor.val() == "" && _configuracaoDescargaCliente.ValorTonelada.val() == "" && _configuracaoDescargaCliente.ValorUnidade.val() == "" && _configuracaoDescargaCliente.ValorPallet.val() == "") {
        exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Atencao, "Necessário informar ao menos um dos campos de valor.");
        return false;
    }

    return true;
}

function preencherListasSelecao() {
    let tiposOperacoes = new Array();

    $.each(_configuracaoDescargaCliente.Tipo.basicTable.BuscarRegistros(), function (i, tipoOperacao) {
        tiposOperacoes.push({ Tipo: tipoOperacao });
    });

    _configuracaoDescargaCliente.TiposOperacoes.val(JSON.stringify(tiposOperacoes));
    _configuracaoDescargaCliente.Transportadores.val(JSON.stringify(_gridTransportador.BuscarRegistros().map(x => parseInt(x.Codigo))));
}
