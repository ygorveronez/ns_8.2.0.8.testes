/// <reference path="../../../js/libs/jquery-2.1.1.js" />
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
/// <reference path="../../Enumeradores/EnumSituacaoSelecaoSeparacao.js" />
/// <reference path="../../Consultas/Filial.js" />
/// <reference path="Carga.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _pesquisaSelecaoWMS;
var _selecaoWMS;
var _crudSelecaoWMS;
var _gridFuncionarios;
var _pesquisaSelecoes;
var _gridSelecoes;

var _situacaoSelecao = [{ text: "Todos", value: EnumSituacaoSelecaoSeparacao.Todos },
{ text: "Finalizado", value: EnumSituacaoSelecaoSeparacao.Finalizada },
{ text: "Enviado", value: EnumSituacaoSelecaoSeparacao.Enviada },
{ text: "Cancelado", value: EnumSituacaoSelecaoSeparacao.Cancelada },
{ text: "Pendente", value: EnumSituacaoSelecaoSeparacao.Pendente }];

var PesquisaSelecoes = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.DataInicial = PropertyEntity({ text: "Data início: ", getType: typesKnockout.date, val: ko.observable("") });
    this.DataFinal = PropertyEntity({ text: "Data final: ", getType: typesKnockout.date, val: ko.observable("") });
    this.Funcionario = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Funcionário:", idBtnSearch: guid(), visible: ko.observable(true) });
    this.Situacao = PropertyEntity({ val: ko.observable(EnumSituacaoSelecaoSeparacao.Todos), options: _situacaoSelecao, def: EnumSituacaoSelecaoSeparacao.Todos, text: "Situação: " });

    this.Pesquisar = PropertyEntity({
        eventClick: function (e) {
            _gridSelecoes.CarregarGrid();
        }, type: types.event, text: "Pesquisar", idGrid: guid(), visible: ko.observable(true)
    });

    this.ExibirFiltrosPesquisa = PropertyEntity({
        eventClick: function (e) {
            if (e.ExibirFiltrosPesquisa.visibleFade() == true) {
                e.ExibirFiltrosPesquisa.visibleFade(false);
            } else {
                e.ExibirFiltrosPesquisa.visibleFade(true);
            }
        }, type: types.event, text: "Filtros de Pesquisa", idFade: guid(), visibleFade: ko.observable(false), visible: ko.observable(true)
    });
}

var PesquisaSelecaoWMS = function () {

    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });

    var dataAtual = moment().add(-1, 'days').format("DD/MM/YYYY");
    this.DataInicio = PropertyEntity({ text: "Data início: ", getType: typesKnockout.date, val: ko.observable(dataAtual) });
    this.DataFim = PropertyEntity({ text: "Data limite: ", dateRangeInit: this.DataInicio, getType: typesKnockout.date });
    this.DataInicio.dateRangeLimit = this.DataFim;
    this.DataFim.dateRangeInit = this.DataInicio;

    this.CodigoCargaEmbarcador = PropertyEntity({ text: "Número da Carga:", val: ko.observable(""), def: "", visible: ko.observable(true) });
    this.CodigoPedidoEmbarcador = PropertyEntity({ text: "Número do Pedido:", val: ko.observable(""), def: "", visible: ko.observable(true) });

    this.Empresa = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Transportador:", issue: 69, idBtnSearch: guid() });
    this.Filial = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Filial:", issue: 70, idBtnSearch: guid(), visible: ko.observable(true) });
    this.Remetente = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Remetente:", issue: 52, idBtnSearch: guid() });
    this.Destinatario = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Destinatário:", issue: 52, idBtnSearch: guid() });
    this.Origem = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Origem:", issue: 16, idBtnSearch: guid() });
    this.Destino = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Destino:", issue: 16, idBtnSearch: guid() });

    this.Pesquisar = PropertyEntity({
        eventClick: function (e) {
            pesquisarCargas();
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
    this.BuscaAvancada = PropertyEntity({
        eventClick: function (e) {
            if (e.BuscaAvancada.visibleFade() == true) {
                e.BuscaAvancada.visibleFade(false);
                e.BuscaAvancada.icon("fal fa-plus");
            } else {
                e.BuscaAvancada.visibleFade(true);
                e.BuscaAvancada.icon("fal fa-minus");
            }
        }, type: types.event, text: "Avançada", idFade: guid(), icon: ko.observable("fal fa-plus"), visibleFade: ko.observable(false), visible: ko.observable(true)
    });
}

var SelecaoWMS = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Situacao = PropertyEntity({ val: ko.observable(EnumSituacaoSelecaoSeparacao.Pendente), def: EnumSituacaoSelecaoSeparacao.Pendente, getType: typesKnockout.int });

    this.Funcionarios = PropertyEntity({ type: types.map, required: false, text: "Informar Colaboradores", getType: typesKnockout.dynamic, idBtnSearch: guid(), idGrid: guid(), enable: ko.observable(true), visible: ko.observable(true) });
    this.ListaFuncionarios = PropertyEntity({ type: types.map, getType: typesKnockout.dynamic, val: ko.observable(""), def: "", idGrid: guid() });

    this.Cargas = PropertyEntity({ getType: typesKnockout.dynamic, val: ko.observable(new Array()), def: new Array(), visible: ko.observable(true) });
}

var CRUDSelecaoWMS = function () {
    this.Cancelar = PropertyEntity({ eventClick: CancelarSeparacaoClick, type: types.event, text: "Cancelar", visible: ko.observable(false), enable: ko.observable(true) });
    this.EnviarSeparacao = PropertyEntity({ eventClick: EnviarSeparacaoClick, type: types.event, text: "Enviar para a Separação", visible: ko.observable(false), enable: ko.observable(true) });
    this.Salvar = PropertyEntity({ eventClick: SalvarClick, type: types.event, text: "Salvar", visible: ko.observable(true), enable: ko.observable(true) });
    this.Limpar = PropertyEntity({ eventClick: LimparClick, type: types.event, text: "Limpar/Nova Seleção", visible: ko.observable(true), enable: ko.observable(true) });
}

//*******EVENTOS*******

function loadSelecaoWMS() {
    buscarDetalhesOperador(function () {
        carregarConteudosHTML(function () {

            _selecaoWMS = new SelecaoWMS();
            KoBindings(_selecaoWMS, "knoutSelecaoWMS");

            _crudSelecaoWMS = new CRUDSelecaoWMS();
            KoBindings(_crudSelecaoWMS, "knoutCRUDSelecaoWMS");

            _pesquisaSelecaoWMS = new PesquisaSelecaoWMS();
            KoBindings(_pesquisaSelecaoWMS, "knoutPesquisaSelecaoWMS");

            _pesquisaSelecoes = new PesquisaSelecoes();
            KoBindings(_pesquisaSelecoes, "knockoutPesquisaSelecoes");

            new BuscarFuncionario(_pesquisaSelecoes.Funcionario);

            new BuscarClientes(_pesquisaSelecaoWMS.Remetente);
            new BuscarLocalidades(_pesquisaSelecaoWMS.Origem);
            new BuscarClientes(_pesquisaSelecaoWMS.Destinatario);
            new BuscarLocalidades(_pesquisaSelecaoWMS.Destino);
            new BuscarFilial(_pesquisaSelecaoWMS.Filial);
            new BuscarTransportadores(_pesquisaSelecaoWMS.Empresa);

            var excluir = {
                descricao: "Remover", id: guid(), evento: "onclick", metodo: function (data) {
                    RemoverFuncionarioClick(_selecaoWMS.Funcionarios, data)
                }, tamanho: "15", icone: ""
            };
            var menuOpcoes = new Object();
            menuOpcoes.tipo = TypeOptionMenu.link;
            menuOpcoes.opcoes = new Array();
            menuOpcoes.opcoes.push(excluir);

            var header = [{ data: "Codigo", visible: false },
            { data: "CPF", title: "CPF", width: "20%", className: "text-align-left" },
            { data: "Nome", title: "Nome", width: "60%", className: "text-align-left" }
            ];

            _gridFuncionarios = new BasicDataTable(_selecaoWMS.Funcionarios.idGrid, header, menuOpcoes);
            _selecaoWMS.Funcionarios.basicTable = _gridFuncionarios;

            new BuscarFuncionario(_selecaoWMS.Funcionarios, RetornoInserirFuncionario, _gridFuncionarios);
            RecarregarListaFuncionarios();

            loadDetalhesCarga(buscarCargasMontagem);
            buscarSelecoes();
        });
    });
}

function RemoverFuncionarioClick(e, sender) {
    if (_selecaoWMS.Situacao.val() != EnumSituacaoSelecaoSeparacao.Pendente) {
        exibirMensagem(tipoMensagem.aviso, "Aviso", "Esta seleção não se encontra mais pendente para a manipulação dos dados.");
        return;
    }

    exibirConfirmacao("Confirmação", "Realmente deseja remover o funcionário selecionado?", function () {
        var funcionarioGrid = e.basicTable.BuscarRegistros();

        for (var i = 0; i < funcionarioGrid.length; i++) {
            if (sender.Codigo == funcionarioGrid[i].Codigo) {
                funcionarioGrid.splice(i, 1);
                break;
            }
        }
        e.basicTable.CarregarGrid(funcionarioGrid);
    });
}

function SalvarClick() {
    preencherListaFuncionario();
    var data = { Selecao: JSON.stringify(RetornarObjetoPesquisa(_selecaoWMS)), ListaFuncionario: _selecaoWMS.ListaFuncionarios.val() };

    executarReST("SelecaoWMS/SalvarSelecao", data, function (arg) {
        if (arg.Success) {
            if (arg.Data !== false) {
                exibirMensagem(tipoMensagem.ok, "Salvo com sucesso");
                limparDadosSelecao();
                buscarSelecoes();
                _selecaoWMS.Codigo.val(arg.Data.Codigo);
                var data = { Codigo: _selecaoWMS.Codigo.val() };
                retornoSelecao(data);
            } else {
                exibirMensagem(tipoMensagem.atencao, "Atenção", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.atencao, "Atenção", arg.Msg);
        }
    });
}

function LimparClick() {
    limparDadosSelecao();
}

function EnviarSeparacaoClick() {
    exibirConfirmacao("Confirmação", "Realmente deseja enviar para a separação as cargas selecionadas?", function () {
        preencherListaFuncionario();
        var data = { Selecao: JSON.stringify(RetornarObjetoPesquisa(_selecaoWMS)), ListaFuncionario: _selecaoWMS.ListaFuncionarios.val() };

        executarReST("SelecaoWMS/EnviarSeparacao", data, function (arg) {
            if (arg.Success) {
                if (arg.Data !== false) {
                    exibirMensagem(tipoMensagem.ok, "Salvo com sucesso");
                    limparDadosSelecao();
                    buscarSelecoes();
                } else {
                    exibirMensagem(tipoMensagem.atencao, "Atenção", arg.Msg);
                }
            } else {
                exibirMensagem(tipoMensagem.atencao, "Atenção", arg.Msg);
            }
        });
    });
}

function CancelarSeparacaoClick() {
    exibirConfirmacao("Confirmação", "Realmente deseja cancelar a separação selecionada?", function () {
        preencherListaFuncionario();
        var data = { Selecao: JSON.stringify(RetornarObjetoPesquisa(_selecaoWMS)), ListaFuncionario: _selecaoWMS.ListaFuncionarios.val() };

        executarReST("SelecaoWMS/CancelarSeparacao", data, function (arg) {
            if (arg.Success) {
                if (arg.Data !== false) {
                    exibirMensagem(tipoMensagem.ok, "Salvo com sucesso");
                    limparDadosSelecao();
                    buscarSelecoes();
                } else {
                    exibirMensagem(tipoMensagem.atencao, "Atenção", arg.Msg);
                }
            } else {
                exibirMensagem(tipoMensagem.atencao, "Atenção", arg.Msg);
            }
        });
    });
}

function retornoSelecao(selecao) {
    executarReST("SelecaoWMS/BuscarPorCodigo", selecao, function (arg) {
        if (arg.Success) {
            if (arg.Data !== false) {
                preencherSelecao(arg.Data);
            } else {
                exibirMensagem(tipoMensagem.atencao, "Atenção", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    });
}

function preencherSelecao(selecaoWMS) {
    var dataSelecaoWMS = { Data: selecaoWMS.SelecaoWMS };
    PreencherObjetoKnout(_selecaoWMS, dataSelecaoWMS);

    RecarregarListaFuncionarios();

    _pesquisaSelecaoWMS.Codigo.val(_selecaoWMS.Codigo.val());
    buscarCargasMontagem();

    desmarcarKnoutsCarga();
    for (var i = 0; i < selecaoWMS.SelecaoWMS.Cargas.length; i++) {
        var carga = selecaoWMS.SelecaoWMS.Cargas[i];
        var index = obterIndiceKnoutCarga(carga);
        if (index >= 0) {
            _knoutsCargas[index].InfoCarga.cssClass("well well-carga-selecionada no-padding padding-5");
        }
    }
    VerificarCompatibilidasKnoutsCarga();

    ValidarBotoes();
}

if (!string.IsNullOrWhitespace)

function RecarregarListaFuncionarios() {
    var cont = 0;
    var total = 0;
    var data = new Array();

    if (!string.IsNullOrWhiteSpace(_selecaoWMS.ListaFuncionarios.val())) {
        $.each(_selecaoWMS.ListaFuncionarios.val(), function (i, func) {
            var obj = new Object();

            obj.Codigo = func.Codigo;
            obj.CPF = func.CPF;
            obj.Nome = func.Nome;

            data.push(obj);            
        });
    };

        _gridFuncionarios.CarregarGrid(data);

}

function preencherListaFuncionario() {
    _selecaoWMS.ListaFuncionarios.list = new Array();

    var funcionarios = new Array();

    $.each(_selecaoWMS.Funcionarios.basicTable.BuscarRegistros(), function (i, func) {
        funcionarios.push({ Funcionario: func });
    });

    _selecaoWMS.ListaFuncionarios.val(JSON.stringify(funcionarios))
}

function RetornoInserirFuncionario(data) {
    if (data != null) {
        var dataGrid = _gridFuncionarios.BuscarRegistros();

        for (var i = 0; i < data.length; i++) {
            var obj = new Object();
            obj.Codigo = data[i].Codigo;
            obj.CPF = data[i].CPF;
            obj.Nome = data[i].Nome;

            dataGrid.push(obj);
        }
        _gridFuncionarios.CarregarGrid(dataGrid);
    }
}

function pesquisarCargas() {
    limparDadosSelecao();

    buscarCargasMontagem();
}

function buscarSelecoes() {
    var editar = { descricao: "Editar", id: "clasEditar", evento: "onclick", metodo: editarSelecao, tamanho: "15", icone: "" };
    var menuOpcoes = new Object();
    menuOpcoes.tipo = TypeOptionMenu.link;
    menuOpcoes.opcoes = new Array();
    menuOpcoes.opcoes.push(editar);

    _gridSelecoes = new GridView(_pesquisaSelecoes.Pesquisar.idGrid, "SelecaoWMS/Pesquisa", _pesquisaSelecoes, menuOpcoes, null);
    _gridSelecoes.CarregarGrid();
}

function editarSelecao(data) {
    limparDadosSelecao();
    _selecaoWMS.Codigo.val(data.Codigo);
    var data = { Codigo: _selecaoWMS.Codigo.val() };
    _pesquisaSelecoes.ExibirFiltrosPesquisa.visibleFade(false);
    retornoSelecao(data);
}

function limparDadosSelecao() {
    $("#" + _AreaCarga.Carga.id).html("");
    _AreaCarga.Inicio.val(0);
    _knoutsCargas = new Array();
    _pesquisaSelecaoWMS.Codigo.val(0);
    LimparCampos(_selecaoWMS);

    _crudSelecaoWMS.Cancelar.visible(false);
    _crudSelecaoWMS.EnviarSeparacao.visible(false);
    _crudSelecaoWMS.Salvar.visible(true);
    _crudSelecaoWMS.Limpar.visible(true);

    _selecaoWMS.ListaFuncionarios.list = new Array();
    RecarregarListaFuncionarios();
    //pesquisarCargas();
}

function ValidarBotoes() {
    _crudSelecaoWMS.EnviarSeparacao.visible(false);
    _crudSelecaoWMS.Cancelar.visible(false);
    _crudSelecaoWMS.Salvar.visible(true);
    _crudSelecaoWMS.Limpar.visible(true);

    if (_selecaoWMS.Situacao.val() == EnumSituacaoSelecaoSeparacao.Cancelada) {
        _crudSelecaoWMS.EnviarSeparacao.visible(false);
        _crudSelecaoWMS.Salvar.visible(false);
    } else if (_selecaoWMS.Situacao.val() == EnumSituacaoSelecaoSeparacao.Enviada) {
        _crudSelecaoWMS.EnviarSeparacao.visible(false);
        _crudSelecaoWMS.Salvar.visible(false);
    } else if (_selecaoWMS.Situacao.val() == EnumSituacaoSelecaoSeparacao.Finalizada) {
        _crudSelecaoWMS.EnviarSeparacao.visible(false);
        _crudSelecaoWMS.Salvar.visible(false);
    } else if (_selecaoWMS.Situacao.val() == EnumSituacaoSelecaoSeparacao.Pendente) {
        if (_selecaoWMS.Codigo.val() > 0)
            _crudSelecaoWMS.EnviarSeparacao.visible(true);
        else
            _crudSelecaoWMS.EnviarSeparacao.visible(false);
        _crudSelecaoWMS.Salvar.visible(true);
        _crudSelecaoWMS.Cancelar.visible(true);
    }
}