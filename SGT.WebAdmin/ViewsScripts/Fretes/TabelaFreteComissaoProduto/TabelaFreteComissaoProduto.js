/// <reference path="../../Enumeradores/EnumTipoGrupoPessoas.js" />
/// <reference path="../../Consultas/Cliente.js" />
/// <reference path="../../Consultas/GrupoPessoa.js" />
/// <reference path="../../Consultas/ContratoFreteTransportador.js" />
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
/// <reference path="../../Consultas/TabelaFrete.js" />
/// <reference path="../../Consultas/Tranportador.js" />
/// <reference path="../../Consultas/Produto.js" />
/// <reference path="../../Enumeradores/EnumTipoTabelaFrete.js" />
/// <reference path="TabelaFreteComissaoProdutoAjuste.js" />

//*******MAPEAMENTO KNOUCKOUT*******


var _gridTabelaFreteComissaoProduto;
var _tabelaFreteComissaoProduto;
var _pesquisaTabelaFreteComissaoProduto;
var _tabelaFrete;

var PesquisaTabelaFreteComissaoProduto = function () {
    this.TabelaFrete = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), required: true, text: "*Tabela de Frete:", idBtnSearch: guid() });
    this.ContratoFreteTransportador = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), required: true, text: "Contrato de Frete:", idBtnSearch: guid() });
    this.Empresa = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Transportador:", idBtnSearch: guid() });
    this.Pessoa = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), required: true, text: "Pessoa:", idBtnSearch: guid() });
    this.GrupoPessoas = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), required: true, text: "Grupo de Pessoas:", idBtnSearch: guid() });
    this.ProdutoEmbarcador = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Produto:", idBtnSearch: guid() });
    this.Ativo = PropertyEntity({ val: ko.observable(true), options: _statusPesquisa, def: true, text: "Situação: " });

    this.Pesquisar = PropertyEntity({
        eventClick: function (e) {
            _gridTabelaFreteComissaoProduto.CarregarGrid();
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

var TabelaFreteComissaoProduto = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.TabelaFrete = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), required: true, text: "*Tabela de Frete:", idBtnSearch: guid() });
    this.ContratoFreteTransportador = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), required: true, text: "*Contrato de Frete:",issue: 859, idBtnSearch: guid() });
    this.Pessoa = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Pessoa:", enable: ko.observable(true), idBtnSearch: guid(), issue: 52 });
    this.GrupoPessoas = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Grupo de Pessoas:", enable: ko.observable(true), idBtnSearch: guid(), issue: 58 });
    this.ProdutoEmbarcador = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "*Produto:", issue: 140, required: true, idBtnSearch: guid() });
    this.PercentualValorProduto = PropertyEntity({ eventBlur: validarPercentualBlur, text: "*% Sobre Valor Produto:", issue: 865, type: types.map, required: true, getType: typesKnockout.decimal });
    this.Ativo = PropertyEntity({ val: ko.observable(true), options: _status, def: true, text: "*Situação: ", issue: 557 });

    this.Adicionar = PropertyEntity({ eventClick: adicionarClick, type: types.event, text: "Adicionar", visible: ko.observable(true) });
    this.Atualizar = PropertyEntity({ eventClick: atualizarClick, type: types.event, text: "Atualizar", visible: ko.observable(false) });
    this.Excluir = PropertyEntity({ eventClick: excluirClick, type: types.event, text: "Excluir", visible: ko.observable(false) });
    this.Cancelar = PropertyEntity({ eventClick: cancelarClick, type: types.event, text: "Cancelar", visible: ko.observable(false) });

    this.GrupoPessoas.codEntity.subscribe(function (novoValor) {
        changeGrupoPessoas(novoValor);
    });

    this.Pessoa.codEntity.subscribe(function (novoValor) {
        changePessoa();
    });
}

//*******EVENTOS*******

function loadTabelaFreteComissaoProduto() {

    _tabelaFreteComissaoProduto = new TabelaFreteComissaoProduto();
    KoBindings(_tabelaFreteComissaoProduto, "knockoutCadastroTabelaFreteComissaoProduto");

    _pesquisaTabelaFreteComissaoProduto = new PesquisaTabelaFreteComissaoProduto();
    KoBindings(_pesquisaTabelaFreteComissaoProduto, "knockoutPesquisaTabelaFreteComissaoProduto", false, _pesquisaTabelaFreteComissaoProduto.Pesquisar.id);

    _tabelaFrete = new PesquisaTabelaFreteComissaoProduto();
    KoBindings(_tabelaFrete, "knockoutTabelaFrete");

    new BuscarTabelasDeFrete(_tabelaFrete.TabelaFrete, retornoTabelaFrete, EnumTipoTabelaFrete.tabelaComissaoProduto);

    new BuscarContratoFreteTransportador(_tabelaFreteComissaoProduto.ContratoFreteTransportador);
    new BuscarProdutos(_tabelaFreteComissaoProduto.ProdutoEmbarcador);
    new BuscarGruposPessoas(_tabelaFreteComissaoProduto.GrupoPessoas, null, null, null, EnumTipoGrupoPessoas.Clientes);
    new BuscarClientes(_tabelaFreteComissaoProduto.Pessoa);

    new BuscarProdutos(_pesquisaTabelaFreteComissaoProduto.ProdutoEmbarcador);
    new BuscarGruposPessoas(_pesquisaTabelaFreteComissaoProduto.GrupoPessoas, null, null, null, EnumTipoGrupoPessoas.Clientes);
    new BuscarClientes(_pesquisaTabelaFreteComissaoProduto.Pessoa);
    new BuscarContratoFreteTransportador(_pesquisaTabelaFreteComissaoProduto.ContratoFreteTransportador);
    new BuscarTransportadores(_pesquisaTabelaFreteComissaoProduto.Empresa, null, null, true);

    buscarTabelaFretePadrao(retornoTabelaFrete);

    $("#" + _tabelaFreteComissaoProduto.GrupoPessoas.id).focusout(function () {
        focusOutGrupoPessoas();
    });

    $("#" + _tabelaFreteComissaoProduto.Pessoa.id).focusout(function () {
        focusOutPessoa();
    });
}

function buscarTabelaFretePadrao(callback) {
    var data = { TipoTabelaFrete: EnumTipoTabelaFrete.tabelaComissaoProduto };
    executarReST("TabelaFrete/BuscarTabelasPorTipo", data, function (arg) {
        if (arg.Success) {
            if (arg.Data.length == 1) {
                callback(arg.Data[0]);
            }
        }
    });
}

function retornoTabelaFrete(e) {
    _tabelaFrete.TabelaFrete.codEntity(e.Codigo);
    _tabelaFrete.TabelaFrete.val(e.Descricao);
    _pesquisaTabelaFreteComissaoProduto.TabelaFrete.codEntity(_tabelaFrete.TabelaFrete.codEntity());
    _pesquisaTabelaFreteComissaoProduto.TabelaFrete.val(_tabelaFrete.TabelaFrete.val());
    _pesquisaTabelaFreteComissaoProduto.TabelaFrete.defCodEntity = _tabelaFrete.TabelaFrete.codEntity();
    _pesquisaTabelaFreteComissaoProduto.TabelaFrete.def = _tabelaFrete.TabelaFrete.val();
    _tabelaFreteComissaoProduto.TabelaFrete.codEntity(_tabelaFrete.TabelaFrete.codEntity());
    _tabelaFreteComissaoProduto.TabelaFrete.defCodEntity = _tabelaFrete.TabelaFrete.codEntity();
    _tabelaFreteComissaoProduto.TabelaFrete.val(_tabelaFrete.TabelaFrete.val());
    _tabelaFreteComissaoProduto.TabelaFrete.def = _tabelaFrete.TabelaFrete.val();
    buscarTabelaFreteComissaoProdutos();
}

function adicionarClick(e, sender) {
    Salvar(e, "TabelaFreteComissaoProduto/Adicionar", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Cadastro realizado com sucesso.");
                _gridTabelaFreteComissaoProduto.CarregarGrid();
                limparCamposTabelaFreteComissaoProduto();
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    }, sender);
}

function atualizarClick(e, sender) {
    Salvar(e, "TabelaFreteComissaoProduto/Atualizar", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Atualizado com sucesso");
                _gridTabelaFreteComissaoProduto.CarregarGrid();
                limparCamposTabelaFreteComissaoProduto();
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }

        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    }, sender);
}

function excluirClick(e, sender) {
    exibirConfirmacao("Confirmação", "Realmente deseja excluir a comissão para o produto " + _tabelaFreteComissaoProduto.ProdutoEmbarcador.val() + "?", function () {
        ExcluirPorCodigo(_tabelaFreteComissaoProduto, "TabelaFreteComissaoProduto/ExcluirPorCodigo", function (arg) {
            if (arg.Success) {
                if (arg.Data) {
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Excluído com sucesso");
                    _gridTabelaFreteComissaoProduto.CarregarGrid();
                    limparCamposTabelaFreteComissaoProduto();
                } else {
                    exibirMensagem(tipoMensagem.aviso, "Sugestão", arg.Msg, 16000);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
            }
        }, null);
    });
}

function validarPercentualBlur(e, sender) {
    if (Globalize.parseFloat(e.PercentualValorProduto.val()) > 100) {
        e.PercentualValorProduto.val(Globalize.format(100, "n2"));
    }
}

function cancelarClick(e) {
    limparCamposTabelaFreteComissaoProduto();
}

//********METODOS*********

function buscarTabelaFreteComissaoProdutos() {
    var editar = { descricao: "Editar", id: "clasEditar", evento: "onclick", metodo: editarTabelaFreteComissaoProduto, tamanho: "15", icone: "" };
    var menuOpcoes = new Object();
    menuOpcoes.tipo = TypeOptionMenu.link;
    menuOpcoes.opcoes = new Array();
    menuOpcoes.opcoes.push(editar);

    _gridTabelaFreteComissaoProduto = new GridView(_pesquisaTabelaFreteComissaoProduto.Pesquisar.idGrid, "TabelaFreteComissaoProduto/Pesquisa", _pesquisaTabelaFreteComissaoProduto, menuOpcoes, null, null, retornoGridTabelaFreteComissao);
    _gridTabelaFreteComissaoProduto.CarregarGrid();
}

function retornoGridTabelaFreteComissao() {
    $("#divContentTabelaFreteComissaoProduto").show();
    $("#wid-id-6").hide();
    $("#spanNomeTabela").text("Tabela de Frete (" + _tabelaFrete.TabelaFrete.val() + ")");
}

function editarTabelaFreteComissaoProduto(tabelaFreteComissaoProdutoGrid) {
    limparCamposTabelaFreteComissaoProduto();
    _tabelaFreteComissaoProduto.Codigo.val(tabelaFreteComissaoProdutoGrid.Codigo);
    BuscarPorCodigo(_tabelaFreteComissaoProduto, "TabelaFreteComissaoProduto/BuscarPorCodigo", function (arg) {
        _pesquisaTabelaFreteComissaoProduto.ExibirFiltros.visibleFade(false);
        _tabelaFreteComissaoProduto.Atualizar.visible(true);
        _tabelaFreteComissaoProduto.Cancelar.visible(true);
        _tabelaFreteComissaoProduto.Excluir.visible(true);
        _tabelaFreteComissaoProduto.Adicionar.visible(false);
    }, null);
}

function limparCamposTabelaFreteComissaoProduto() {
    _tabelaFreteComissaoProduto.Atualizar.visible(false);
    _tabelaFreteComissaoProduto.Cancelar.visible(false);
    _tabelaFreteComissaoProduto.Excluir.visible(false);
    _tabelaFreteComissaoProduto.Adicionar.visible(true);
    LimparCampos(_tabelaFreteComissaoProduto);
}

function changeGrupoPessoas(data) {
    if (Globalize.parseInt(_tabelaFreteComissaoProduto.GrupoPessoas.codEntity().toString()) > 0)
        _tabelaFreteComissaoProduto.Pessoa.enable(false);
    else
        _tabelaFreteComissaoProduto.Pessoa.enable(true);
}

function focusOutGrupoPessoas() {
    if (_tabelaFreteComissaoProduto.GrupoPessoas.val().trim() == "")
        _tabelaFreteComissaoProduto.GrupoPessoas.codEntity(0);
}

function changePessoa(data) {
    if (Globalize.parseInt(_tabelaFreteComissaoProduto.Pessoa.codEntity().toString()) > 0) 
        _tabelaFreteComissaoProduto.GrupoPessoas.enable(false);
    else
        _tabelaFreteComissaoProduto.GrupoPessoas.enable(true);
}

function focusOutPessoa() {
    if (_tabelaFreteComissaoProduto.Pessoa.val().trim() == "")
        _tabelaFreteComissaoProduto.Pessoa.codEntity(0);
}
