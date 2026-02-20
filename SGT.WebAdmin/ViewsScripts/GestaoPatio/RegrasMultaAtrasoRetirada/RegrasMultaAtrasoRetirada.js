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
/// <reference path="../../../ViewsScripts/Consultas/Filial.js" />
/// <reference path="PeriodoCarregamento.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _gridRegrasMultaAtrasoRetirada;
var _regrasMultaAtrasoRetirada;
var _pesquisaRegrasMultaAtrasoRetirada;

var PesquisaRegrasMultaAtrasoRetirada = function () {
    this.Descricao = PropertyEntity({ text: "Descrição: " });
    this.Ativo = PropertyEntity({ text: "Status: ", val: ko.observable(0), options: _statusPesquisa, def: 0 });
    this.Filial = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Filial:", idBtnSearch: guid() });
    this.Ocorrencia = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Tipo Ocorrência:", idBtnSearch: guid() });

    this.Pesquisar = PropertyEntity({
        eventClick: function (e) {
            _gridRegrasMultaAtrasoRetirada.CarregarGrid();
        }, type: types.event, text: "Pesquisar", idGrid: guid(), visible: ko.observable(true)
    });
    this.ExibirFiltros = PropertyEntity({
        eventClick: function (e) {
            if (e.ExibirFiltros.visibleFade()) {
                e.ExibirFiltros.visibleFade(false);
            } else {
                e.ExibirFiltros.visibleFade(true);
            }
        }, type: types.event, text: "Filtros de Pesquisa", idFade: guid(), visibleFade: ko.observable(false), visible: ko.observable(true)
    });
};

var RegrasMultaAtrasoRetirada = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });

    this.Descricao = PropertyEntity({ text: "*Descrição: ", required: ko.observable(true), maxlength: 500 });
    this.Ativo = PropertyEntity({ text: "*Status: ", val: ko.observable(true), options: _status, def: true, required: ko.observable(true), enable: ko.observable(true) });
    this.Filial = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Filial:", idBtnSearch: guid() });
    this.PercentualInclusao = PropertyEntity({ text: "Percentual de Inclusão:", required: false, getType: typesKnockout.decimal, maxlength: 6, val: ko.observable("0"), def: "0" });
    this.Ocorrencia = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Tipo Ocorrência:", idBtnSearch: guid() });

    //Aba Transportadores
    this.RegrasMultaAtrasoRetiradaTransportadores = PropertyEntity({ type: types.map, getType: typesKnockout.dynamic, val: ko.observable(""), def: "", idGrid: guid() });
    this.GridRegrasMultaAtrasoRetiradaTransportadores = PropertyEntity({ type: types.local });
    this.Transportadores = PropertyEntity({ type: types.event, text: "Adicionar Transportador", idBtnSearch: guid() });

    //Aba Estados
    this.RegrasMultaAtrasoRetiradaEstados = PropertyEntity({ type: types.map, getType: typesKnockout.dynamic, val: ko.observable(""), def: "", idGrid: guid() });
    this.GridRegrasMultaAtrasoRetiradaEstados = PropertyEntity({ type: types.local });
    this.Estados = PropertyEntity({ type: types.event, text: "Adicionar Estado", idBtnSearch: guid() });

    //Aba Cidades
    this.RegrasMultaAtrasoRetiradaLocalidades = PropertyEntity({ type: types.map, getType: typesKnockout.dynamic, val: ko.observable(""), def: "", idGrid: guid() });
    this.GridRegrasMultaAtrasoRetiradaLocalidades = PropertyEntity({ type: types.local });
    this.Localidades = PropertyEntity({ type: types.event, text: "Adicionar Cidade", idBtnSearch: guid() });

    //Aba Tipos de Operações
    this.RegrasMultaAtrasoRetiradaTiposOperacoes = PropertyEntity({ type: types.map, getType: typesKnockout.dynamic, val: ko.observable(""), def: "", idGrid: guid() });
    this.GridRegrasMultaAtrasoRetiradaTiposOperacoes = PropertyEntity({ type: types.local });
    this.TiposOperacoes = PropertyEntity({ type: types.event, text: "Adicionar Tipo de Operação", idBtnSearch: guid() });

    //Aba Faixas de CEP's
    this.ListaCEPs = PropertyEntity({ type: types.map, getType: typesKnockout.dynamic, val: ko.observable([]), def: [], idGrid: guid() });
    this.CEPs = PropertyEntity({ type: types.map, getType: typesKnockout.dynamic, val: ko.observable([]), def: [], idGrid: guid() });
    this.GridCEP = PropertyEntity({ type: types.local });
    this.CEPInicial = PropertyEntity({ text: "CEP Inicial:", val: ko.observable(""), def: "", getType: typesKnockout.cep, visible: ko.observable(true) });
    this.CEPFinal = PropertyEntity({ text: "CEP Final:", val: ko.observable(""), def: "", getType: typesKnockout.cep, visible: ko.observable(true) });
    this.AdicionarCEP = PropertyEntity({ eventClick: AdicionarCEPClick, type: types.event, text: "Adicionar", icon: "fal fa-plus", visible: ko.observable(true), enable: ko.observable(true) });

    //Aba Clientes
    this.RegrasMultaAtrasoRetiradaClientes = PropertyEntity({ type: types.map, getType: typesKnockout.dynamic, val: ko.observable(""), def: "", idGrid: guid() });
    this.GridRegrasMultaAtrasoRetiradaClientes = PropertyEntity({ type: types.local });
    this.Clientes = PropertyEntity({ type: types.event, text: "Adicionar Cliente", idBtnSearch: guid() });

    //Períodos de Carregamento
    this.PeriodosCarregamento = PropertyEntity({ type: types.listEntity, list: new Array(), idGrid: guid(), codEntity: ko.observable(0), text: "Períodos de Carregamento" });
};

var CRUDRegrasMultaAtrasoRetirada = function () {
    this.Adicionar = PropertyEntity({ eventClick: adicionarClick, type: types.event, text: "Adicionar", visible: ko.observable(true) });
    this.Atualizar = PropertyEntity({ eventClick: atualizarClick, type: types.event, text: "Atualizar", visible: ko.observable(false) });
    this.Excluir = PropertyEntity({ eventClick: excluirClick, type: types.event, text: "Excluir", visible: ko.observable(false) });
    this.Cancelar = PropertyEntity({ eventClick: cancelarClick, type: types.event, text: "Cancelar", visible: ko.observable(false) });
};

//*******EVENTOS*******

function loadRegrasMultaAtrasoRetirada() {
    _regrasMultaAtrasoRetirada = new RegrasMultaAtrasoRetirada();
    KoBindings(_regrasMultaAtrasoRetirada, "knockoutCadastroRegrasMultaAtrasoRetirada");

    HeaderAuditoria("RegrasMultaAtrasoRetirada", _regrasMultaAtrasoRetirada);

    _crudRegrasMultaAtrasoRetirada = new CRUDRegrasMultaAtrasoRetirada();
    KoBindings(_crudRegrasMultaAtrasoRetirada, "knockoutCRUDRegrasMultaAtrasoRetirada");

    _pesquisaRegrasMultaAtrasoRetirada = new PesquisaRegrasMultaAtrasoRetirada();
    KoBindings(_pesquisaRegrasMultaAtrasoRetirada, "knockoutPesquisaRegrasMultaAtrasoRetirada", false, _pesquisaRegrasMultaAtrasoRetirada.Pesquisar.id);

    new BuscarFilial(_regrasMultaAtrasoRetirada.Filial);
    new BuscarTipoOcorrencia(_regrasMultaAtrasoRetirada.Ocorrencia);

    new BuscarTipoOcorrencia(_pesquisaRegrasMultaAtrasoRetirada.Ocorrencia);
    new BuscarFilial(_pesquisaRegrasMultaAtrasoRetirada.Filial);

    loadRegrasMultaAtrasoRetiradaClientes();
    loadRegrasMultaAtrasoRetiradaTiposOperacoes();
    loadRegrasMultaAtrasoRetiradaLocalidades();
    loadGridRegrasMultaAtrasoRetiradaEstados();
    loadRegrasMultaAtrasoRetiradaTransportadores();
    LoadFaixaCEP();
    LoadCapacidadeCarregamento();

    buscarRegrasMultaAtrasoRetirada();
}

function adicionarClick(e, sender) {
    PreencherListasDeSelecao();
    Salvar(_regrasMultaAtrasoRetirada, "RegrasMultaAtrasoRetirada/Adicionar", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Cadastrado com sucesso");
                _gridRegrasMultaAtrasoRetirada.CarregarGrid();
                limparCamposRegrasMultaAtrasoRetirada();
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    }, sender);
}

function atualizarClick(e, sender) {
    PreencherListasDeSelecao();
    Salvar(_regrasMultaAtrasoRetirada, "RegrasMultaAtrasoRetirada/Atualizar", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Atualizado com sucesso");
                _gridRegrasMultaAtrasoRetirada.CarregarGrid();
                limparCamposRegrasMultaAtrasoRetirada();
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    }, sender);
}

function excluirClick(e, sender) {
    exibirConfirmacao("Confirmação", "Realmente deseja excluir a Marca EPI " + _regrasMultaAtrasoRetirada.Descricao.val() + "?", function () {
        ExcluirPorCodigo(_regrasMultaAtrasoRetirada, "RegrasMultaAtrasoRetirada/ExcluirPorCodigo", function (arg) {
            if (arg.Success) {
                if (arg.Data) {
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Excluído com sucesso");
                    _gridRegrasMultaAtrasoRetirada.CarregarGrid();
                    limparCamposRegrasMultaAtrasoRetirada();
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
    limparCamposRegrasMultaAtrasoRetirada();
}

//*******MÉTODOS*******

function buscarRegrasMultaAtrasoRetirada() {
    var editar = { descricao: "Editar", id: "clasEditar", evento: "onclick", metodo: editarRegrasMultaAtrasoRetirada, tamanho: "15", icone: "" };
    var menuOpcoes = new Object();
    menuOpcoes.tipo = TypeOptionMenu.link;
    menuOpcoes.opcoes = new Array();
    menuOpcoes.opcoes.push(editar);

    _gridRegrasMultaAtrasoRetirada = new GridView(_pesquisaRegrasMultaAtrasoRetirada.Pesquisar.idGrid, "RegrasMultaAtrasoRetirada/Pesquisa", _pesquisaRegrasMultaAtrasoRetirada, menuOpcoes);
    _gridRegrasMultaAtrasoRetirada.CarregarGrid();
}

function editarRegrasMultaAtrasoRetirada(RegrasMultaAtrasoRetiradaGrid) {
    limparCamposRegrasMultaAtrasoRetirada();
    _regrasMultaAtrasoRetirada.Codigo.val(RegrasMultaAtrasoRetiradaGrid.Codigo);
    BuscarPorCodigo(_regrasMultaAtrasoRetirada, "RegrasMultaAtrasoRetirada/BuscarPorCodigo", function (arg) {
        _pesquisaRegrasMultaAtrasoRetirada.ExibirFiltros.visibleFade(false);
        _crudRegrasMultaAtrasoRetirada.Atualizar.visible(true);
        _crudRegrasMultaAtrasoRetirada.Cancelar.visible(true);
        _crudRegrasMultaAtrasoRetirada.Excluir.visible(true);
        _crudRegrasMultaAtrasoRetirada.Adicionar.visible(false);

        _regrasMultaAtrasoRetirada.Transportadores.basicTable.CarregarGrid(_regrasMultaAtrasoRetirada.RegrasMultaAtrasoRetiradaTransportadores.val());
        _regrasMultaAtrasoRetirada.Estados.basicTable.CarregarGrid(_regrasMultaAtrasoRetirada.RegrasMultaAtrasoRetiradaEstados.val());
        _regrasMultaAtrasoRetirada.Localidades.basicTable.CarregarGrid(_regrasMultaAtrasoRetirada.RegrasMultaAtrasoRetiradaLocalidades.val());
        _regrasMultaAtrasoRetirada.TiposOperacoes.basicTable.CarregarGrid(_regrasMultaAtrasoRetirada.RegrasMultaAtrasoRetiradaTiposOperacoes.val());
        _regrasMultaAtrasoRetirada.Clientes.basicTable.CarregarGrid(_regrasMultaAtrasoRetirada.RegrasMultaAtrasoRetiradaClientes.val());

        recarregarGridsCapacidadeCarregamento();
        RecarregarGridCEP();
    }, null);
}

function limparCamposRegrasMultaAtrasoRetirada() {
    LimparCampos(_regrasMultaAtrasoRetirada);

    _crudRegrasMultaAtrasoRetirada.Atualizar.visible(false);
    _crudRegrasMultaAtrasoRetirada.Cancelar.visible(false);
    _crudRegrasMultaAtrasoRetirada.Excluir.visible(false);
    _crudRegrasMultaAtrasoRetirada.Adicionar.visible(true);

    _regrasMultaAtrasoRetirada.Transportadores.basicTable.CarregarGrid(new Array());
    _regrasMultaAtrasoRetirada.Estados.basicTable.CarregarGrid(new Array());
    _regrasMultaAtrasoRetirada.Localidades.basicTable.CarregarGrid(new Array());
    _regrasMultaAtrasoRetirada.TiposOperacoes.basicTable.CarregarGrid(new Array());
    _regrasMultaAtrasoRetirada.Clientes.basicTable.CarregarGrid(new Array());
    _regrasMultaAtrasoRetirada.CEPs.val([]);
    LimparCamposCapacidadeCarregamento();


    RecarregarGridCEP();
    recarregarGridsCapacidadeCarregamento()

    //Global.ResetarAbas();
}

function PreencherListasDeSelecao() {
    _regrasMultaAtrasoRetirada.RegrasMultaAtrasoRetiradaTransportadores.val(JSON.stringify(_regrasMultaAtrasoRetirada.Transportadores.basicTable.BuscarRegistros()));
    _regrasMultaAtrasoRetirada.RegrasMultaAtrasoRetiradaEstados.val(JSON.stringify(_regrasMultaAtrasoRetirada.Estados.basicTable.BuscarRegistros()));
    _regrasMultaAtrasoRetirada.RegrasMultaAtrasoRetiradaLocalidades.val(JSON.stringify(_regrasMultaAtrasoRetirada.Localidades.basicTable.BuscarRegistros()));
    _regrasMultaAtrasoRetirada.RegrasMultaAtrasoRetiradaTiposOperacoes.val(JSON.stringify(_regrasMultaAtrasoRetirada.TiposOperacoes.basicTable.BuscarRegistros()));
    _regrasMultaAtrasoRetirada.ListaCEPs.val(JSON.stringify(_regrasMultaAtrasoRetirada.CEPs.val()));
    _regrasMultaAtrasoRetirada.RegrasMultaAtrasoRetiradaClientes.val(JSON.stringify(_regrasMultaAtrasoRetirada.Clientes.basicTable.BuscarRegistros()));
}