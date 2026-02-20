/// <reference path="../../Enumeradores/EnumTipoIntegracao.js" />
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
/// <reference path="ModeloVeicularCarga.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _gridGrupoProduto;
var _grupoProduto;
var _crudGrupoProduto;
var _pesquisaGrupoProduto;

var PesquisaGrupoProduto = function () {
    this.Descricao = PropertyEntity({ text: Localization.Resources.Gerais.Geral.Descricao.getFieldDescription() });
    this.Ativo = PropertyEntity({ val: ko.observable(true), options: _statusPesquisa, def: true, text: Localization.Resources.Gerais.Geral.Situacao.getFieldDescription() });

    this.Pesquisar = PropertyEntity({
        eventClick: function (e) {
            _gridGrupoProduto.CarregarGrid();
        }, type: types.event, text: Localization.Resources.Gerais.Geral.Pesquisar, idGrid: guid(), visible: ko.observable(true)
    });

    this.ExibirFiltros = PropertyEntity({
        eventClick: function (e) {
            if (e.ExibirFiltros.visibleFade() == true) {
                e.ExibirFiltros.visibleFade(false);
            } else {
                e.ExibirFiltros.visibleFade(true);
            }
        }, type: types.event, text: Localization.Resources.Gerais.Geral.FiltrosPesquisa.getFieldDescription(), idFade: guid(), visibleFade: ko.observable(false), visible: ko.observable(true)
    });
}

var GrupoProduto = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Descricao = PropertyEntity({ text: Localization.Resources.Gerais.Geral.Descricao.getRequiredFieldDescription(), issue: 550, required: true });
    this.CodigoGrupoProdutoEmbarcador = PropertyEntity({ text: Localization.Resources.Gerais.Geral.CodigoIntegracao.getRequiredFieldDescription(), maxlength: 50, issue: 15 });
    this.Ativo = PropertyEntity({ val: ko.observable(true), options: _status, def: true, text: Localization.Resources.Gerais.Geral.Situacao.getRequiredFieldDescription(), issue: 557 });
    this.TiposCarga = PropertyEntity({ type: types.listEntity, list: new Array(), required: false, codEntity: ko.observable(0) });

    this.ProdutoValorMaiorOpenTech = PropertyEntity({ val: ko.observable(""), def: "", text: Localization.Resources.Produtos.GrupoProduto.ProdutoParaValorMaior.getFieldDescription() });
    this.ProdutoValorMenorOpenTech = PropertyEntity({ val: ko.observable(""), def: "", text: Localization.Resources.Produtos.GrupoProduto.ProdutoParaValorMenor.getFieldDescription() });

    this.Ativo = PropertyEntity({ val: ko.observable(true), options: _status, def: true, text: Localization.Resources.Gerais.Geral.Situacao.getRequiredFieldDescription() });
    
    this.QuantidadePorCaixa = PropertyEntity({ getType: typesKnockout.int, def: 0, val: ko.observable(0), text: Localization.Resources.Produtos.GrupoProduto.QuantidadePorCaixa.getFieldDescription() });
    this.PorcentagemCorrecao = PropertyEntity({ getType: typesKnockout.decimal, def: "0,00", val: ko.observable("0,00"), text: Localization.Resources.Produtos.GrupoProduto.PorcentagemCorrecao.getFieldDescription() });
    this.ListarProdutosDesteGrupoNoRelatorioDeSinteseDeMateriaisDoPatio = PropertyEntity({ getType: typesKnockout.bool, val: ko.observable(false), def: false, text: Localization.Resources.Produtos.GrupoProduto.ListarProdutosDesteGrupoRelatorioSinteseMateriaisPatio });
    this.RetornarNoChecklist = PropertyEntity({ getType: typesKnockout.bool, val: ko.observable(false), def: false, text: Localization.Resources.Produtos.GrupoProduto.RetornarNoChecklist });
    this.NaoPermitirCarregamento = PropertyEntity({ getType: typesKnockout.bool, val: ko.observable(false), def: false, text: Localization.Resources.Produtos.GrupoProduto.NaoPermitirCarregamento });

    this.ProdutoValorMaiorOpenTech.val.subscribe(function (novoValor) {
        if (_openTech != null)
            _openTech.ProdutoValorMaiorOpenTech.val(novoValor);
    });

    this.ProdutoValorMenorOpenTech.val.subscribe(function (novoValor) {
        if (_openTech != null)
            _openTech.ProdutoValorMenorOpenTech.val(novoValor);
    });
}

var CRUDGrupoProduto = function () {
    this.Adicionar = PropertyEntity({ eventClick: adicionarClick, type: types.event, text: Localization.Resources.Gerais.Geral.Adicionar, visible: ko.observable(true) });
    this.Atualizar = PropertyEntity({ eventClick: atualizarClick, type: types.event, text: Localization.Resources.Gerais.Geral.Atualizar, visible: ko.observable(false) });
    this.Excluir = PropertyEntity({ eventClick: excluirClick, type: types.event, text: Localization.Resources.Gerais.Geral.Excluir, visible: ko.observable(false) });
    this.Cancelar = PropertyEntity({ eventClick: cancelarClick, type: types.event, text: Localization.Resources.Gerais.Geral.Cancelar, visible: ko.observable(false) });
}

//*******EVENTOS*******

function loadGrupoProduto() {
    ConfigurarTiposIntegracao().then(function () {
        _grupoProduto = new GrupoProduto();
        KoBindings(_grupoProduto, "knockoutCadastroGrupoProduto");

        HeaderAuditoria("GrupoProduto", _grupoProduto);

        _crudGrupoProduto = new CRUDGrupoProduto();
        KoBindings(_crudGrupoProduto, "knockoutCRUDGrupoProduto");

        _pesquisaGrupoProduto = new PesquisaGrupoProduto();
        KoBindings(_pesquisaGrupoProduto, "knockoutPesquisaGrupoProduto", false, _pesquisaGrupoProduto.Pesquisar.id);

        buscarGruposProduto();
        loadGrupoProdutoTipoCarga();
    });
}

function adicionarClick(e, sender) {
    reordenarPosicoesTiposCarga();
    Salvar(_grupoProduto, "GrupoProduto/Adicionar", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, Localization.Resources.Gerais.Geral.CadastradoComSucesso);
                _gridGrupoProduto.CarregarGrid();
                limparCamposGrupoProduto();
            } else {
                exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.Aviso, arg.Msg);
            }   
        } else {
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, arg.Msg);
        }
    }, sender, exibirCamposObrigatorio);
}

function atualizarClick(e, sender) {
    reordenarPosicoesTiposCarga();
    Salvar(_grupoProduto, "GrupoProduto/Atualizar", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, Localization.Resources.Gerais.Geral.AtualizadoComSucesso);
                _gridGrupoProduto.CarregarGrid();
                limparCamposGrupoProduto();
            } else {
                exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.Aviso, arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, arg.Msg);
        }
    }, sender, exibirCamposObrigatorio);
}

function excluirClick(e, sender) {
    exibirConfirmacao(Localization.Resources.Gerais.Geral.Confirmacao, Localization.Resources.Produtos.GrupoProduto.RealmenteDesejaExcluirGrupoProduto.format(_grupoProduto.Descricao.val()), function () {
        ExcluirPorCodigo(_grupoProduto, "GrupoProduto/ExcluirPorCodigo", function (arg) {
            if (arg.Success) {
                if (arg.Data) {
                    exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, Localization.Resources.Gerais.Geral.ExcluidoComSucesso);
                    _gridGrupoProduto.CarregarGrid();
                    limparCamposGrupoProduto();
                } else {
                    exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.Sugestao, arg.Msg, 16000);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, arg.Msg);
            }

        }, null);
    });
}

function cancelarClick(e) {
    resetarTabs();
    limparCamposGrupoProduto();
}

//*******MÉTODOS*******

function editarGrupoProduto(grupoProdutoGrid) {
    limparCamposGrupoProduto();
    _grupoProduto.Codigo.val(grupoProdutoGrid.Codigo);
    BuscarPorCodigo(_grupoProduto, "GrupoProduto/BuscarPorCodigo", function (arg) {
        _pesquisaGrupoProduto.ExibirFiltros.visibleFade(false);
        _crudGrupoProduto.Atualizar.visible(true);
        _crudGrupoProduto.Cancelar.visible(true);
        _crudGrupoProduto.Excluir.visible(true);
        _crudGrupoProduto.Adicionar.visible(false);
        reordenarPosicoesTiposCarga();
        recarregarGridReorder();
    }, null);
}


function buscarGruposProduto() {
    var editar = { descricao: Localization.Resources.Gerais.Geral.Editar, id: "clasEditar", evento: "onclick", metodo: editarGrupoProduto, tamanho: "20", icone: "" };
    var menuOpcoes = new Object();
    menuOpcoes.tipo = TypeOptionMenu.link;
    menuOpcoes.opcoes = new Array();
    menuOpcoes.opcoes.push(editar);

    _gridGrupoProduto = new GridView(_pesquisaGrupoProduto.Pesquisar.idGrid, "GrupoProduto/Pesquisa", _pesquisaGrupoProduto, menuOpcoes, null);
    _gridGrupoProduto.CarregarGrid();
}


function limparCamposGrupoProduto() {
    resetarTabs();
    _crudGrupoProduto.Atualizar.visible(false);
    _crudGrupoProduto.Cancelar.visible(false);
    _crudGrupoProduto.Excluir.visible(false);
    _crudGrupoProduto.Adicionar.visible(true);
    LimparCampos(_grupoProduto);
    _grupoProduto.TiposCarga.list = new Array();
    _gridReorder.LimparGrid();
    limparCamposOpenTech();
}


function exibirCamposObrigatorio() {
    resetarTabs();
    exibirMensagem("atencao", Localization.Resources.Gerais.Geral.CampoObrigatorio, Localization.Resources.Gerais.Geral.PorFavorInformeOsCamposObrigatorios);
}

function resetarTabs() {
    $("#myTab a:first").tab("show");
}

function ConfigurarTiposIntegracao() {
    var p = new promise.Promise();

    executarReST("TipoIntegracao/BuscarTodos", { Tipos: JSON.stringify([EnumTipoIntegracao.OpenTech]) }, function (r) {
        if (r.Success) {
            if (r.Data) {

                for (var i = 0; i < r.Data.length; i++) {
                    if (r.Data[i].Codigo == EnumTipoIntegracao.OpenTech) {
                        ObterProdutosOpenTech();
                        $("#liOpenTech").show();
                    }
                }

            } else {
                exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.Aviso, r.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, r.Msg);
        }

        p.done();
    });

    return p;
}

function ObterProdutosOpenTech() {
    executarReST("GrupoProduto/BuscarProdutosOpenTech", {}, function (arg) {
        if (arg.Success) {
            loadOpenTech();
            var produtosOpenTech = [{ value: "", text: Localization.Resources.Gerais.Geral.Selecione }].concat(arg.Data);

            _openTech.ProdutoValorMaiorOpenTech.options(produtosOpenTech);
            _openTech.ProdutoValorMenorOpenTech.options(produtosOpenTech);
        } else {
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, arg.Msg);
        }
    });
}
