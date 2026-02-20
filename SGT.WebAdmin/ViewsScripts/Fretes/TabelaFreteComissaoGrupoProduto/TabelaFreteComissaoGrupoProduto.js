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


var _gridTabelaFreteComissaoGrupoProduto;
var _tabelaFreteComissaoGrupoProduto;
var _pesquisaTabelaFreteComissaoGrupoProduto;
var _tabelaFrete;

var PesquisaTabelaFreteComissaoGrupoProduto = function () {
    this.TabelaFrete = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), required: true, text: "*Tabela de Frete:", idBtnSearch: guid() });
    this.ContratoFreteTransportador = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), required: true, text: "Contrato de Frete:", idBtnSearch: guid() });
    this.Empresa = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Transportador:", idBtnSearch: guid() });
    this.Pessoa = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), required: true, text: "Pessoa:", idBtnSearch: guid() });
    this.GrupoPessoas = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), required: true, text: "Grupo de Pessoas:", idBtnSearch: guid() });
    this.GrupoProduto = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Grupo de Produto:", idBtnSearch: guid() });
    this.Ativo = PropertyEntity({ val: ko.observable(true), options: _statusPesquisa, def: true, text: "Situação: " });

    this.Pesquisar = PropertyEntity({
        eventClick: function (e) {
            _gridTabelaFreteComissaoGrupoProduto.CarregarGrid();
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

var TabelaFreteComissaoGrupoProduto = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.TabelaFrete = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), required: true, text: "*Tabela de Frete:", idBtnSearch: guid() });
    this.ContratoFreteTransportador = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), required: true, text: "*Contrato de Frete:",issue: 859, idBtnSearch: guid() });
    this.Pessoa = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Pessoa:", enable: ko.observable(true), idBtnSearch: guid(), issue: 52 });
    this.GrupoPessoas = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Grupo de Pessoas:", enable: ko.observable(true), idBtnSearch: guid(), issue: 58 });
    this.PercentualValorProduto = PropertyEntity({ eventBlur: validarPercentualBlur, text: "*% Sobre Valor Produto:",issue: 865, type: types.map, required: true, getType: typesKnockout.decimal });
    this.Ativo = PropertyEntity({ val: ko.observable(true), options: _status, def: true, text: "*Situação: ", issue: 557 });

    this.GridGrupoProduto = PropertyEntity({ type: types.local });
    this.GrupoProduto = PropertyEntity({ type: types.event, text: "Adicionar Grupo de Produto", idBtnSearch: guid() });
    this.GruposProdutos = PropertyEntity({ type: types.map, getType: typesKnockout.dynamic, val: ko.observable(""), def: "", idGrid: guid() });

    this.Adicionar = PropertyEntity({ eventClick: adicionarClick, type: types.event, text: "Adicionar", visible: ko.observable(true) });

    this.GrupoPessoas.codEntity.subscribe(function (novoValor) {
        changeGrupoPessoas(novoValor);
    });

    this.Pessoa.codEntity.subscribe(function (novoValor) {
        changePessoa();
    });
}

//*******EVENTOS*******

function loadTabelaFreteComissaoGrupoProduto() {

    _tabelaFreteComissaoGrupoProduto = new TabelaFreteComissaoGrupoProduto();
    KoBindings(_tabelaFreteComissaoGrupoProduto, "knockoutCadastroTabelaFreteComissaoGrupoProduto");

    _pesquisaTabelaFreteComissaoGrupoProduto = new PesquisaTabelaFreteComissaoGrupoProduto();
    KoBindings(_pesquisaTabelaFreteComissaoGrupoProduto, "knockoutPesquisaTabelaFreteComissaoGrupoProduto", false, _pesquisaTabelaFreteComissaoGrupoProduto.Pesquisar.id);

    _tabelaFrete = new PesquisaTabelaFreteComissaoGrupoProduto();
    KoBindings(_tabelaFrete, "knockoutTabelaFrete");

    new BuscarTabelasDeFrete(_tabelaFrete.TabelaFrete, retornoTabelaFrete, EnumTipoTabelaFrete.tabelaComissaoProduto);

    new BuscarContratoFreteTransportador(_tabelaFreteComissaoGrupoProduto.ContratoFreteTransportador);
    new BuscarGruposPessoas(_tabelaFreteComissaoGrupoProduto.GrupoPessoas, null, null, null, EnumTipoGrupoPessoas.Clientes);
    new BuscarClientes(_tabelaFreteComissaoGrupoProduto.Pessoa);

    new BuscarGruposProdutos(_pesquisaTabelaFreteComissaoGrupoProduto.GrupoProduto);
    new BuscarGruposPessoas(_pesquisaTabelaFreteComissaoGrupoProduto.GrupoPessoas, null, null, null, EnumTipoGrupoPessoas.Clientes);
    new BuscarClientes(_pesquisaTabelaFreteComissaoGrupoProduto.Pessoa);
    new BuscarContratoFreteTransportador(_pesquisaTabelaFreteComissaoGrupoProduto.ContratoFreteTransportador);
    new BuscarTransportadores(_pesquisaTabelaFreteComissaoGrupoProduto.Empresa, null, null, true);

    buscarTabelaFretePadrao(retornoTabelaFrete);

    $("#" + _tabelaFreteComissaoGrupoProduto.GrupoPessoas.id).focusout(function () {
        focusOutGrupoPessoas();
    });

    $("#" + _tabelaFreteComissaoGrupoProduto.Pessoa.id).focusout(function () {
        focusOutPessoa();
    });

    loadGrupoProduto();
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
    _pesquisaTabelaFreteComissaoGrupoProduto.TabelaFrete.codEntity(_tabelaFrete.TabelaFrete.codEntity());
    _pesquisaTabelaFreteComissaoGrupoProduto.TabelaFrete.val(_tabelaFrete.TabelaFrete.val());
    _pesquisaTabelaFreteComissaoGrupoProduto.TabelaFrete.defCodEntity = _tabelaFrete.TabelaFrete.codEntity();
    _pesquisaTabelaFreteComissaoGrupoProduto.TabelaFrete.def = _tabelaFrete.TabelaFrete.val();
    _tabelaFreteComissaoGrupoProduto.TabelaFrete.codEntity(_tabelaFrete.TabelaFrete.codEntity());
    _tabelaFreteComissaoGrupoProduto.TabelaFrete.defCodEntity = _tabelaFrete.TabelaFrete.codEntity();
    _tabelaFreteComissaoGrupoProduto.TabelaFrete.val(_tabelaFrete.TabelaFrete.val());
    _tabelaFreteComissaoGrupoProduto.TabelaFrete.def = _tabelaFrete.TabelaFrete.val();
    buscarTabelaFreteComissaoGrupoProdutos();
}

function adicionarClick(e, sender) {
    _tabelaFreteComissaoGrupoProduto.GruposProdutos.val(JSON.stringify(_tabelaFreteComissaoGrupoProduto.GrupoProduto.basicTable.BuscarRegistros()));

    Salvar(e, "TabelaFreteComissaoGrupoProduto/Adicionar", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Cadastro realizado com sucesso.");
                _gridTabelaFreteComissaoGrupoProduto.CarregarGrid();
                limparCamposTabelaFreteComissaoGrupoProduto();
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    }, sender);
}

function validarPercentualBlur(e, sender) {
    if (Globalize.parseFloat(e.PercentualValorProduto.val()) > 100) {
        e.PercentualValorProduto.val(Globalize.format(100, "n2"));
    }
}

function cancelarClick(e) {
    limparCamposTabelaFreteComissaoGrupoProduto();
}

//********METODOS*********

function buscarTabelaFreteComissaoGrupoProdutos() {
    _gridTabelaFreteComissaoGrupoProduto = new GridView(_pesquisaTabelaFreteComissaoGrupoProduto.Pesquisar.idGrid, "TabelaFreteComissaoGrupoProduto/Pesquisa", _pesquisaTabelaFreteComissaoGrupoProduto, null, null, null, retornoGridTabelaFreteComissao);
    _gridTabelaFreteComissaoGrupoProduto.CarregarGrid();
}

function retornoGridTabelaFreteComissao() {
    $("#divContentTabelaFreteComissaoGrupoProduto").show();
    $("#wid-id-6").hide();
    $("#spanNomeTabela").text("Tabela de Frete (" + _tabelaFrete.TabelaFrete.val() + ")");
}

function limparCamposTabelaFreteComissaoGrupoProduto() {
    LimparCampos(_tabelaFreteComissaoGrupoProduto);
    recarregarGridGrupoProduto();
}

function changeGrupoPessoas(data) {
    if (Globalize.parseInt(_tabelaFreteComissaoGrupoProduto.GrupoPessoas.codEntity().toString()) > 0)
        _tabelaFreteComissaoGrupoProduto.Pessoa.enable(false);
    else
        _tabelaFreteComissaoGrupoProduto.Pessoa.enable(true);
}

function focusOutGrupoPessoas() {
    if (_tabelaFreteComissaoGrupoProduto.GrupoPessoas.val().trim() == "")
        _tabelaFreteComissaoGrupoProduto.GrupoPessoas.codEntity(0);
}

function changePessoa(data) {
    if (Globalize.parseInt(_tabelaFreteComissaoGrupoProduto.Pessoa.codEntity().toString()) > 0)
        _tabelaFreteComissaoGrupoProduto.GrupoPessoas.enable(false);
    else
        _tabelaFreteComissaoGrupoProduto.GrupoPessoas.enable(true);
}

function focusOutPessoa() {
    if (_tabelaFreteComissaoGrupoProduto.Pessoa.val().trim() == "")
        _tabelaFreteComissaoGrupoProduto.Pessoa.codEntity(0);
}
