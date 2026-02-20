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
/// <reference path="PassagemPercursoEstado.js" />
/// <reference path="../Mapas/MapaBrasil.js" />



//*******MAPEAMENTO KNOUCKOUT*******


var _gridPercursosEntreEstados;
var _percursosEntreEstados;
var _pesquisaPercursosEntreEstados;
var _mapaMDFe;
var _CRUDPercurso;

var PesquisaPercursosEntreEstados = function () {
    this.EstadoOrigem = PropertyEntity({ val: ko.observable(""), options: _estadosPesquisa, def: "", text: "*Estado de Origem: ", required: true });
    this.EstadoDestino = PropertyEntity({ val: ko.observable(""), options: _estadosPesquisa, def: "", text: "*Estado de Destino: ", required: true });
    this.Empresa = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: ko.observable("Transportador:"), idBtnSearch: guid(), visible: ko.observable(true) });
    this.Pesquisar = PropertyEntity({
        eventClick: function (e) {
            _gridPercursosEntreEstados.CarregarGrid();
        }, type: types.event, text: "Pesquisar", idGrid: guid(), visible: ko.observable(true)
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

var CRUDPercurso = function () {
    this.Adicionar = PropertyEntity({ eventClick: adicionarClick, type: types.event, text: "Adicionar", visible: ko.observable(true) });
    this.Atualizar = PropertyEntity({ eventClick: atualizarClick, type: types.event, text: "Atualizar", visible: ko.observable(false) });
    this.Excluir = PropertyEntity({ eventClick: excluirClick, type: types.event, text: "Excluir", visible: ko.observable(false) });
    this.Cancelar = PropertyEntity({ eventClick: cancelarClick, type: types.event, text: "Cancelar", visible: ko.observable(false) });
}

var PercursosEntreEstados = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.EstadoOrigem = PropertyEntity({ val: ko.observable("SC"), options: _estados, def: "SC", text: "*Estado de Origem: ", required: true, eventChange: mapaOrigemDestinoChange });
    this.EstadosDestino = PropertyEntity({ type: types.map, getType: typesKnockout.dynamic, val: ko.observable(new Array()), def: new Array, idGrid: guid() });
    this.PassagemPercursoEstado = PropertyEntity({ type: types.listEntity, list: new Array(), options: _estados, val: ko.observable("AC"), def: "AC", codEntity: ko.observable("AC"), defCodEntity: "AC", text: "*Estado de Passagem:" });
    this.Empresa = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: ko.observable("Transportador:"), idBtnSearch: guid(), visible: ko.observable(true) });
}


var PassagemPercursoEstadoMap = function () {
    this.Ordem = PropertyEntity({ val: 0, def: 0 });
    this.Codigo = PropertyEntity({ val: 0, def: 0 });
    this.EstadoDePassagem = PropertyEntity({ val: "AC", def: "AC", options: _estados });
    this.DescricaoEstado = PropertyEntity({ type: types.local, val: "" });
}


//*******EVENTOS*******

function loadPercursosEntreEstados() {

    _CRUDPercurso = new CRUDPercurso();
    KoBindings(_CRUDPercurso, "knoutCRUDPercurso");

    _percursosEntreEstados = new PercursosEntreEstados();
    KoBindings(_percursosEntreEstados, "knockoutCadastroPercursosEntreEstados");

    HeaderAuditoria("PercursoEstado", _percursosEntreEstados);

    loadEstadoDestino();
    _pesquisaPercursosEntreEstados = new PesquisaPercursosEntreEstados();
    KoBindings(_pesquisaPercursosEntreEstados, "knockoutPesquisaPercursosEntreEstados", false, _pesquisaPercursosEntreEstados.Pesquisar.id);


    if (_CONFIGURACAO_TMS.TipoServicoMultisoftware == EnumTipoServicoMultisoftware.MultiTMS) {
        _percursosEntreEstados.Empresa.text("Empresa/Filial:");
        _pesquisaPercursosEntreEstados.Empresa.text("Empresa/Filial:");
    } else if (_CONFIGURACAO_TMS.TipoServicoMultisoftware == EnumTipoServicoMultisoftware.MultiCTe) {
        _percursosEntreEstados.Empresa.visible(false);
        _pesquisaPercursosEntreEstados.Empresa.visible(false);
    }

    new BuscarTransportadores(_pesquisaPercursosEntreEstados.Empresa);
    new BuscarTransportadores(_percursosEntreEstados.Empresa);

    _mapaMDFe = new MapaMDFe();
    _mapaMDFe.LoadMapaMDFe("mapa", function () {
        buscarPercursosEntreEstados();
        setarOrigemDestinoMapa();
    });
}


function preencherListasSelecao() {
    var estadosDestino = new Array();

    $.each(_estadoDestino.Tipo.basicTable.BuscarRegistros(), function (i, estadoDestino) {
        estadosDestino.push({ Tipo: estadoDestino });
    });

    _percursosEntreEstados.EstadosDestino.val(JSON.stringify(estadosDestino));
}


function mapaOrigemDestinoChange(e, sender) {
    _percursosEntreEstados.PassagemPercursoEstado.list = new Array();
    setarOrigemDestinoMapa();
}

function setarOrigemDestinoMapa() {
    if (_mapaMDFe != null) {
        _mapaMDFe.LimparMapa();
        if (_estadoDestino.Tipo.basicTable.BuscarRegistros().length > 0) {
            var ultimoEstado = "";
            $.each(_estadoDestino.Tipo.basicTable.BuscarRegistros(), function (i, estadoDestino) {
                _mapaMDFe.AddOrigemDestino(_percursosEntreEstados.EstadoOrigem.val(), estadoDestino.Codigo);
                ultimoEstado = estadoDestino.Codigo;
            });

            _mapaMDFe.SetEstadoDestino(ultimoEstado);

            _mapaMDFe.AddLocalidadesBuscaAPI(DescricaoCapitalEstado(_percursosEntreEstados.EstadoOrigem.val()));
            _mapaMDFe.AddLocalidadesBuscaAPI(DescricaoCapitalEstado(ultimoEstado));

            $.each(_percursosEntreEstados.PassagemPercursoEstado.list, function (i, estado) {
                _mapaMDFe.AddEstadoPassagem(estado.EstadoDePassagem.val, estado.Ordem.val);
            });
            _mapaMDFe.AtualizarDisplayMapa();
        }
    }
}

function adicionarClick(e, sender) {
    if (SetarPassagens()) {
        preencherListasSelecao();
        Salvar(_percursosEntreEstados, "PercursosEntreEstados/Adicionar", function (arg) {
            if (arg.Success) {
                if (arg.Data) {
                    exibirMensagem(tipoMensagem.ok, "sucesso", "cadastrado");
                    _gridPercursosEntreEstados.CarregarGrid();
                    limparCamposPercursosEntreEstados();
                } else {
                    exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
            }
        }, sender, exibirCamposObrigatorio);
    }
}


function atualizarClick(e, sender) {
    if (SetarPassagens()) {
        preencherListasSelecao();
        Salvar(_percursosEntreEstados, "PercursosEntreEstados/Atualizar", function (arg) {
            if (arg.Success) {
                if (arg.Data) {
                    exibirMensagem(tipoMensagem.ok, "sucesso", "Atualizado com sucesso");
                    _gridPercursosEntreEstados.CarregarGrid();
                    limparCamposPercursosEntreEstados();
                } else {
                    exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, "falha", arg.Msg);
            }
        }, sender, exibirCamposObrigatorio);
    }

}

function SetarPassagens() {
    if (_estadoDestino.Tipo.basicTable.BuscarRegistros().length == 0) {
        exibirMensagem(tipoMensagem.aviso, "Aviso", "É obrigatório informar o destino");
        return false;
    }
    _percursosEntreEstados.PassagemPercursoEstado.list = new Array();
    var valido = _mapaMDFe.ValidarPassagens();
    if (valido) {
        var estadosPassagem = _mapaMDFe.GetEstadosPassagem();
        $.each(estadosPassagem, function (i, estadoPassagem) {
            var passagem = new PassagemPercursoEstadoMap();
            passagem.Ordem.val = estadoPassagem.Posicao;
            passagem.EstadoDePassagem.val = estadoPassagem.Sigla;
            _percursosEntreEstados.PassagemPercursoEstado.list.push(passagem);
        });
    }
    return valido;
}


function excluirClick(e, sender) {
    exibirConfirmacao("Confirmação", "Realmente deseja excluir o percurso ?", function () {
        ExcluirPorCodigo(_percursosEntreEstados, "PercursosEntreEstados/ExcluirPorCodigo", function (arg) {
            if (arg.Success) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Excluído com sucesso");
                _gridPercursosEntreEstados.CarregarGrid();
                limparCamposPercursosEntreEstados();
            } else {
                exibirMensagem(tipoMensagem.falha, "falha", arg.Msg);
            }
        }, null);
    });
}

function cancelarClick(e) {
    resetarTabs();
    limparCamposPercursosEntreEstados();
}

//*******MÉTODOS*******


function buscarPercursosEntreEstados() {
    var editar = { descricao: "Editar", id: "clasEditar", evento: "onclick", metodo: editarPercursosEntreEstados, tamanho: "15", icone: "" };
    var menuOpcoes = new Object();
    menuOpcoes.tipo = TypeOptionMenu.link;
    menuOpcoes.opcoes = new Array();
    menuOpcoes.opcoes.push(editar);

    _gridPercursosEntreEstados = new GridView(_pesquisaPercursosEntreEstados.Pesquisar.idGrid, "PercursosEntreEstados/Pesquisa", _pesquisaPercursosEntreEstados, menuOpcoes);
    _gridPercursosEntreEstados.CarregarGrid();
}

function editarPercursosEntreEstados(percursosEntreEstadosGrid) {
    limparCamposPercursosEntreEstados();
    _percursosEntreEstados.Codigo.val(percursosEntreEstadosGrid.Codigo);
    BuscarPorCodigo(_percursosEntreEstados, "PercursosEntreEstados/BuscarPorCodigo", function (arg) {
        _pesquisaPercursosEntreEstados.ExibirFiltros.visibleFade(false);
        _CRUDPercurso.Atualizar.visible(true);
        _CRUDPercurso.Cancelar.visible(true);
        _CRUDPercurso.Excluir.visible(true);
        _CRUDPercurso.Adicionar.visible(false);
        setarOrigemDestinoMapa();
        recarregarGridEstadoDestino();
    }, null);
}

function limparCamposPercursosEntreEstados() {
    _CRUDPercurso.Atualizar.visible(false);
    _CRUDPercurso.Cancelar.visible(false);
    _CRUDPercurso.Excluir.visible(false);
    _CRUDPercurso.Adicionar.visible(true);
    _percursosEntreEstados.EstadosDestino.val(new Array());
    LimparCampos(_percursosEntreEstados);
    limparCamposEstadoDestino();
    _percursosEntreEstados.PassagemPercursoEstado.list = new Array();
    setarOrigemDestinoMapa();
    recarregarGridEstadoDestino();
    resetarTabs();
}


function exibirCamposObrigatorio() {
    resetarTabs();
    exibirMensagem("atencao", "Campo Obrigatório", "Por Favor, Informe os campos obrigatórios");
}

function resetarTabs() {
    $("#liPassagems").removeAttr("class");
    $("#liTabEstadoDestino").removeAttr("class");
    $("#liPercursosEntreEstados").attr("class", "active");
    $("#knockoutCadastroPercursosEntreEstados").attr("class", "tab-pane fade active in padding-10 no-padding-bottom");
    $("#tabPassagem").attr("class", "tab-pane fade");
    $("#knockoutEstadoDestino").attr("class", "tab-pane fade");
}
