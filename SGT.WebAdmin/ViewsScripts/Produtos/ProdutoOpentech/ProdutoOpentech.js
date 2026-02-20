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
/// <reference path="../../Consultas/TipoCarga.js" />


//*******MAPEAMENTO KNOUCKOUT*******

var _produtoOpentech;
var _pesquisaProdutoOpentech;
var _gridProdutoOpentech;

var _estados = [
    { text: "Todos", value: "" },
    { text: "Acre", value: "AC" },
    { text: "Alagoas", value: "AL" },
    { text: "Amapá", value: "AP" },
    { text: "Amazonas", value: "AM" },
    { text: "Bahia", value: "BA" },
    { text: "Ceará", value: "CE" },
    { text: "Distrito Federal", value: "DF" },
    { text: "Espírito Santo", value: "ES" },
    { text: "Goiás", value: "GO" },
    { text: "Maranhão", value: "MA" },
    { text: "Mato Grosso", value: "MT" },
    { text: "Mato Grosso do Sul", value: "MS" },
    { text: "Minas Gerais", value: "MG" },
    { text: "Pará", value: "PA" },
    { text: "Paraíba", value: "PB" },
    { text: "Paraná", value: "PR" },
    { text: "Pernambuco", value: "PE" },
    { text: "Piauí", value: "PI" },
    { text: "Rio de Janeiro", value: "RJ" },
    { text: "Rio Grande do Norte", value: "RN" },
    { text: "Rio Grande do Sul", value: "RS" },
    { text: "Rondônia", value: "RO" },
    { text: "Roraima", value: "RR" },
    { text: "Santa Catarina", value: "SC" },
    { text: "São Paulo", value: "SP" },
    { text: "Sergipe", value: "SE" },
    { text: "Tocantins", value: "TO" }
];

var ProdutoOpentech = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.TipoOperacao = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: ko.observable("*Tipo Operação:"), required: true, idBtnSearch: guid(), visible: ko.observable(true), enable: ko.observable(true) });
    this.TipoCarga = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Tipo de Carga: ", idBtnSearch: guid(), visible: ko.observable(true) });
    this.Apolice = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: ko.observable("Apolice:"), required: false, idBtnSearch: guid(), visible: ko.observable(true), enable: ko.observable(true) });
    this.ProdutoOpentech = PropertyEntity({ val: ko.observable(""), options: ko.observable([]), required: true, def: "", text: "*Produto Opentech:" });
    this.Status = PropertyEntity({ text: "Status: ", issue: 556, val: ko.observable(true), options: _status, def: true });   
    this.Estados = PropertyEntity({ type: types.map, getType: typesKnockout.dynamic, val: ko.observable(""), def: "", idGrid: guid() });
    this.Localidades = PropertyEntity({ type: types.map, getType: typesKnockout.dynamic, val: ko.observable(""), def: "", idGrid: guid() });
    this.ValorDe = PropertyEntity({ getType: typesKnockout.decimal, val: ko.observable(0), def: 0, text: "De R$:" });
    this.ValorAte = PropertyEntity({ getType: typesKnockout.decimal, val: ko.observable(0), def: 0, text: "Até R$:" });
    this.CodigoEmbarcador = PropertyEntity({ getType: typesKnockout.int, val: ko.observable(0), def: 0, text: "Código Embarcador:" });

    this.Adicionar = PropertyEntity({ eventClick: adicionarClick, type: types.event, text: "Adicionar", visible: ko.observable(true) });
    this.Atualizar = PropertyEntity({ eventClick: atualizarClick, type: types.event, text: "Atualizar", visible: ko.observable(false) });
    this.Excluir = PropertyEntity({ eventClick: excluirClick, type: types.event, text: "Excluir", visible: ko.observable(false) });
    this.Cancelar = PropertyEntity({ eventClick: cancelarClick, type: types.event, text: "Cancelar", visible: ko.observable(false) });


}

var PesquisaProdutoOpentech = function () {   
    this.TipoOperacao = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: ko.observable("Tipo Operação:"), idBtnSearch: guid(), visible: ko.observable(true), enable: ko.observable(true) });
    this.Estado = PropertyEntity({ val: ko.observable(""), options: _estados, def: "", text: "UF: ", });
    this.Apolice = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: ko.observable("Apolice:"), idBtnSearch: guid(), visible: ko.observable(true), enable: ko.observable(true) });
    this.Status = PropertyEntity({ val: ko.observable(true), options: _statusPesquisa, def: true, text: "Status:" });
    this.TipoCarga = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: "Tipo de Carga: ", idBtnSearch: guid(), visible: ko.observable(true) });
    this.Pesquisar = PropertyEntity({
        eventClick: function (e) {
            _gridProdutoOpentech.CarregarGrid();
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


//*******EVENTOS*******
function loadProdutoOpentech() {
    ConfigurarTiposIntegracao().then(function () {
        _pesquisaProdutoOpentech = new PesquisaProdutoOpentech();
        KoBindings(_pesquisaProdutoOpentech, "knockoutPesquisaProdutoOpentech", false, _pesquisaProdutoOpentech.Pesquisar.id);

        new BuscarTiposdeCarga(_pesquisaProdutoOpentech.TipoCarga, null, null);
        

        _produtoOpentech = new ProdutoOpentech();
        KoBindings(_produtoOpentech, "knockoutProdutoOpentech");

        new BuscarTiposdeCarga(_produtoOpentech.TipoCarga, null, null);

        new BuscarTiposOperacao(_produtoOpentech.TipoOperacao);
        new BuscarApolicesSeguro(_produtoOpentech.Apolice, null, null, null, apoliceSeguroChange);

        new BuscarTiposOperacao(_pesquisaProdutoOpentech.TipoOperacao);
        new BuscarApolicesSeguro(_pesquisaProdutoOpentech.Apolice, null, null, null, pesquisaApoliceSeguroChange);

        HeaderAuditoria("ProdutoOpentech", _produtoOpentech);

        LoadEstado();
        LoadLocalidade();

        buscarProdutoOpentech();

        if (!_CONFIGURACAO_TMS.ConsiderarLocalidadeProdutoIntegracaoEntrega)
            $("#ConsideraLocalidade").hide();
        
    });
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
                exibirMensagem(tipoMensagem.aviso, "Aviso", r.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", r.Msg);
        }

        p.done();
    });

    return p;
}

function apoliceSeguroChange(apolice) {
    _produtoOpentech.Apolice.val(apolice.NumeroApolice + ' - ' + apolice.Seguradora);
    _produtoOpentech.Apolice.codEntity(apolice.Codigo);
}

function pesquisaApoliceSeguroChange(apolice) {
    _pesquisaProdutoOpentech.Apolice.val(apolice.NumeroApolice + ' - ' + apolice.Seguradora);
    _pesquisaProdutoOpentech.Apolice.codEntity(apolice.Codigo);
}

function adicionarClick(e, sender) {

    _produtoOpentech.Estados.val(JSON.stringify(_estado.Estado.basicTable.BuscarRegistros()));
    _produtoOpentech.Localidades.val(JSON.stringify(_localidade.Localidade.basicTable.BuscarRegistros()));

    Salvar(_produtoOpentech, "ProdutoOpentech/Adicionar", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Cadastrado com sucesso.");
                _gridProdutoOpentech.CarregarGrid();
                limparCamposProdutoOpentech();
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    }, sender);
}

function atualizarClick(e, sender) {

    _produtoOpentech.Estados.val(JSON.stringify(_estado.Estado.basicTable.BuscarRegistros()));
    _produtoOpentech.Localidades.val(JSON.stringify(_localidade.Localidade.basicTable.BuscarRegistros()));

    Salvar(_produtoOpentech, "ProdutoOpentech/Atualizar", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Atualizado com sucesso");
                _gridProdutoOpentech.CarregarGrid();
                limparCamposProdutoOpentech();
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    }, sender);
}

function excluirClick(e, sender) {
    exibirConfirmacao("Confirmação", "Realmente deseja excluir esse cadastro?", function () {
        ExcluirPorCodigo(_produtoOpentech, "ProdutoOpentech/ExcluirPorCodigo", function (arg) {
            if (arg.Success) {
                if (arg.Data) {
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Excluído com sucesso");
                    _gridProdutoOpentech.CarregarGrid();
                    limparCamposProdutoOpentech();
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
    limparCamposProdutoOpentech();
}

function editarProdutoOpentechClick(itemGrid) {
    // Limpa os campos
    limparCamposProdutoOpentech();

    _produtoOpentech.Codigo.val(itemGrid.Codigo);

    // Busca informacoes para edicao
    BuscarPorCodigo(_produtoOpentech, "ProdutoOpentech/BuscarPorCodigo", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                // Esconde pesqusia
                _pesquisaProdutoOpentech.ExibirFiltros.visibleFade(false);

                RecarregarGridEstado();
                RecarregarGridLocalidade();

                // Alternas os campos de CRUD
                _produtoOpentech.Atualizar.visible(true);
                _produtoOpentech.Excluir.visible(true);
                _produtoOpentech.Cancelar.visible(true);
                _produtoOpentech.Adicionar.visible(false);
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    }, null);
}



//*******MÉTODOS*******
function buscarProdutoOpentech() {
    //-- Grid
    // Opcoes
    var editar = { descricao: "Editar", id: "clasEditar", evento: "onclick", metodo: editarProdutoOpentechClick, tamanho: "7", icone: "" };

    // Menu
    var menuOpcoes = {
        tipo: TypeOptionMenu.link,
        opcoes: [editar]
    };


    var configExportacao = {
        url: "ProdutoOpentech/ExportarPesquisa",
        titulo: "Motivo Rejeição"
    };


    // Inicia Grid de busca
    _gridProdutoOpentech = new GridViewExportacao(_pesquisaProdutoOpentech.Pesquisar.idGrid, "ProdutoOpentech/Pesquisa", _pesquisaProdutoOpentech, menuOpcoes, configExportacao);
    _gridProdutoOpentech.CarregarGrid();
}

function limparCamposProdutoOpentech() {
    _produtoOpentech.Atualizar.visible(false);
    _produtoOpentech.Cancelar.visible(false);
    _produtoOpentech.Excluir.visible(false);
    _produtoOpentech.Adicionar.visible(true);
    LimparCamposEstado();
    LimparCamposLocalidade()
    LimparCampos(_produtoOpentech);

    RecarregarGridEstado();
    RecarregarGridLocalidade();
}

function ObterProdutosOpenTech() {
    executarReST("ProdutoOpentech/BuscarProdutosOpenTech", {}, function (arg) {
        if (arg.Success) {
            var produtosOpenTech = [{ value: "", text: "Selecione" }].concat(arg.Data);

            _produtoOpentech.ProdutoOpentech.options(produtosOpenTech);
        } else {
            var produtosOpenTech = [{ value: "", text: "Selecione" }];

            _produtoOpentech.ProdutoOpentech.options(produtosOpenTech);

            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    });
}