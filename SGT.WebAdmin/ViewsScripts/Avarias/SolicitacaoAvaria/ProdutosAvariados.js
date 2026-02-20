/// <reference path="../../Consultas/NotaFiscal.js" />
/// <reference path="../../Consultas/Produto.js" />
/// <reference path="../../Consultas/MotivoAvaria.js" />
/// <reference path="../../Consultas/Carga.js" />
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
/// <reference path="../../Enumeradores/EnumUnidadeSituacaoAvaria.js" />

//*******MAPEAMENTO KNOUCKOUT*******


var _produtosAvariados;
var _gridProdutosAvariados;
var $modalProdutosAvariados;

var ProdutoAvariado = function () {
    this.Solicitacao = PropertyEntity({ val: ko.observable(0), def: 0 });
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0 });

    this.NotaFiscal = PropertyEntity({ type: types.map, text: "Nota Fiscal:", enable: ko.observable(true) });
    this.Produto = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Produto:", idBtnSearch: guid(), enable: ko.observable(true) });
    this.CaixasAvariadas = PropertyEntity({ type: types.map, getType: typesKnockout.int, text: "Caixas Avariadas:" });
    this.UnidadesAvariadas = PropertyEntity({ type: types.map, getType: typesKnockout.int, text: "Unidades Avariadas:" });
    this.ValorInformadoOperador = PropertyEntity({ type: types.map, getType: typesKnockout.decimal, text: "Informar Valor Manualmente:", visible: ko.observable(false) });

    this.Adicionar = PropertyEntity({ eventClick: adicionarProdutoClick, type: types.event, text: "Adicionar", visible: ko.observable(true) });
    this.Atualizar = PropertyEntity({ eventClick: atualizarProdutoClick, type: types.event, text: "Atualizar", visible: ko.observable(false) });
    this.Excluir = PropertyEntity({ eventClick: excluirProdutoClick, type: types.event, text: "Excluir", visible: ko.observable(false) });
    this.Cancelar = PropertyEntity({ eventClick: cancelarProdutoClick, type: types.event, text: "Cancelar", visible: ko.observable(false) });
}



//*******EVENTOS*******
function loadCadastroProdutosAvariados() {
    _produtoAvariado = new ProdutoAvariado();
    KoBindings(_produtoAvariado, "knockoutCadastroProdutoAvariado");

    if (_CONFIGURACAO_TMS.TipoServicoMultisoftware == EnumTipoServicoMultisoftware.MultiEmbarcador || _CONFIGURACAO_TMS.TipoServicoMultisoftware == EnumTipoServicoMultisoftware.MultiTMS)
        _produtoAvariado.ValorInformadoOperador.visible(true);

    // Cache do DOM modal
    $modalProdutosAvariados = $("#divModalProdutoAvariado");

    // Buscar
    new BuscarProdutoAvaria(_produtoAvariado.Produto, null, _solicitacaoAvaria.Carga);
}

function adicionarProdutoClick(e, sender) {
    Salvar(e, "SolicitacaoAvaria/AdicionarProduto", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Adicionado com sucesso");
                _gridProdutosAvariados.CarregarGrid();
                LimparCamposProdutoAvariado();
                AtualizarValorAvaria();
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    }, sender);
}

function atualizarProdutoClick(e, sender) {
    Salvar(e, "SolicitacaoAvaria/AtualizarProduto", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Atualizado com sucesso");
                _gridProdutosAvariados.CarregarGrid();
                LimparCamposProdutoAvariado();
                AtualizarValorAvaria();
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "falha", arg.Msg);
        }
    }, sender);
}

function excluirProdutoClick(e, sender) {
    exibirConfirmacao("Confirmação", "Realmente deseja excluir o produto " + e.Produto.val() + "?", function () {
        var dados = {
            Codigo: e.Codigo.val(),
            Solicitacao: e.Solicitacao.val(),
        };
        if (sender != null) {
            var btn = $("#" + sender.currentTarget.id);
            btn.button('loading');
        }

        executarReST("SolicitacaoAvaria/ExcluirProduto", dados, function (arg) {
            if (sender != null)  btn.button('reset');
            
            if (arg.Success) {
                if (arg.Data != false) {
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Excluído com sucesso");
                    LimparCamposProdutoAvariado();
                    _gridProdutosAvariados.CarregarGrid();
                    AtualizarValorAvaria();
                } else {
                    exibirMensagem(tipoMensagem.aviso, "falha", arg.Msg);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, "falha", arg.Msg);
            }
        }, null);
    });
}

function cancelarProdutoClick(e, sender) {
    LimparCamposProdutoAvariado();
}



//*******METODOS*******
function AbrirModalProdutosAvariados() {
    // Abre modal
    $modalProdutosAvariados.modal('show');
}

function LimparCamposProdutoAvariado() {
    _produtoAvariado.Adicionar.visible(true);
    _produtoAvariado.Cancelar.visible(true);
    _produtoAvariado.Atualizar.visible(false);
    _produtoAvariado.Excluir.visible(false);
    LimparCampos(_produtoAvariado);

    _produtoAvariado.Produto.enable(true);
    _produtoAvariado.NotaFiscal.enable(true);

    $modalProdutosAvariados.modal('hide');    
}

function BuscarProdutoPorCodigo(cb) {
    BuscarPorCodigo(_produtoAvariado, "SolicitacaoAvaria/BuscarProdutoPorCodigo", function (arg) {
        if(cb != null)
            cb();
    }, null);
}