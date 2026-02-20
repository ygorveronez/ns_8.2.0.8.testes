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
/// <reference path="../../Enumeradores/EnumTipoObservacaoFaturamentoMensal.js" />
/// <reference path="PlanoEmissaoFaturamento.js" />

//*******MAPEAMENTO KNOUCKOUT*******


var _gridPlanoEmissaoFaturamento;
var _planoEmissaoFaturamento;
var _pesquisaPlanoEmissaoFaturamento;

var _tipoObservacao = [{ text: "Usar em BOLETO", value: EnumTipoObservacaoFaturamentoMensal.Boleto },
{ text: "Nenhum", value: EnumTipoObservacaoFaturamentoMensal.Nenhum },
{ text: "Usar em NF", value: EnumTipoObservacaoFaturamentoMensal.NotaFiscal },
{ text: "Usar em NF e BOLETO", value: EnumTipoObservacaoFaturamentoMensal.NotaFiscalBoleto }];

var PesquisaPlanoEmissaoFaturamento = function () {
    this.Descricao = PropertyEntity({ text: "Descrição: ", getType: typesKnockout.string });
    this.Ativo = PropertyEntity({ val: ko.observable(true), options: _statusPesquisa, def: true, text: "Situação: " });

    this.Pesquisar = PropertyEntity({
        eventClick: function (e) {
            _gridPlanoEmissaoFaturamento.CarregarGrid();
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

var PlanoEmissaoFaturamento = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Descricao = PropertyEntity({ text: "*Descrição:", required: true, maxlength: 500 });
    this.Ativo = PropertyEntity({ val: ko.observable(true), options: _status, def: true, text: "*Situação: " });
    this.ValorAdesao = PropertyEntity({ text: "Valor da Adesão: ", required: false, getType: typesKnockout.decimal, enable: false });

    this.CobrancaNFe = PropertyEntity({ val: ko.observable(true), text: "Realizar a cobrança de notas modelo 55 autorizadas?", def: true, getType: typesKnockout.bool, visible: ko.observable(true) });
    this.CobrancaNFSe = PropertyEntity({ val: ko.observable(false), text: "Realizar a cobrança de notas de serviços autorizadas?", def: false, getType: typesKnockout.bool, visible: ko.observable(true) });
    this.CobrancaBoleto = PropertyEntity({ val: ko.observable(false), text: "Realizar a cobrança de boletos emitidos?", def: false, getType: typesKnockout.bool, visible: ko.observable(true) });
    this.CobrancaTitulo = PropertyEntity({ val: ko.observable(false), text: "Realizar a cobrança de títulos a receber lançados?", def: false, getType: typesKnockout.bool, visible: ko.observable(true) });

    this.TipoObservacao = PropertyEntity({ val: ko.observable(EnumTipoObservacaoFaturamentoMensal.NotaFiscalBoleto), options: _tipoObservacao, text: "Tipo Observacao: ", def: EnumTipoObservacaoFaturamentoMensal.NotaFiscalBoleto, required: false, enable: ko.observable(true) });
    this.Observacao = PropertyEntity({ text: "Observação:", required: false, maxlength: 500 });

    this.Adicionar = PropertyEntity({ eventClick: adicionarClick, type: types.event, text: "Adicionar", visible: ko.observable(true) });
    this.Atualizar = PropertyEntity({ eventClick: atualizarClick, type: types.event, text: "Atualizar", visible: ko.observable(false) });
    this.Excluir = PropertyEntity({ eventClick: excluirClick, type: types.event, text: "Excluir", visible: ko.observable(false) });
    this.Cancelar = PropertyEntity({ eventClick: cancelarClick, type: types.event, text: "Cancelar", visible: ko.observable(false) });

    this.ValoresPlano = PropertyEntity({ type: types.listEntity, list: new Array(), required: false, idGrid: guid(), codEntity: ko.observable(0), });

    this.CodigoValor = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.QuantidadeInicial = PropertyEntity({ text: "Quantidade Inicial: ", required: false, getType: typesKnockout.int });
    this.QuantidadeFinal = PropertyEntity({ text: "*Quantidade Final: ", required: false, getType: typesKnockout.int });
    this.Valor = PropertyEntity({ text: "*Valor: ", required: false, getType: typesKnockout.decimal });
    this.DescricaoValor = PropertyEntity({ text: "Descrição:", required: false, maxlength: 500 });

    this.TipoObservacaoValor = PropertyEntity({ val: ko.observable(EnumTipoObservacaoFaturamentoMensal.NotaFiscalBoleto), options: _tipoObservacao, text: "Tipo Observação: ", def: EnumTipoObservacaoFaturamentoMensal.NotaFiscalBoleto, required: false, enable: ko.observable(true) });
    this.ObservacaoValor = PropertyEntity({ text: "Observação:", required: false, maxlength: 500 });

    this.AdicionarValor = PropertyEntity({ eventClick: AdicionarPlanoEmissaoFaturamentoValorClick, type: types.event, text: ko.observable("Adicionar Valor"), visible: ko.observable(true) });
}

//*******EVENTOS*******


function loadPlanoEmissaoFaturamento() {

    _planoEmissaoFaturamento = new PlanoEmissaoFaturamento();
    KoBindings(_planoEmissaoFaturamento, "knockoutCadastroPlanoEmissaoFaturamento");

    HeaderAuditoria("PlanoEmissaoNFe", _planoEmissaoFaturamento);

    _pesquisaPlanoEmissaoFaturamento = new PesquisaPlanoEmissaoFaturamento();
    KoBindings(_pesquisaPlanoEmissaoFaturamento, "knockoutPesquisaPlanoEmissaoFaturamento", false, _pesquisaPlanoEmissaoFaturamento.Pesquisar.id);

    buscarPlanoEmissaoFaturamentos();
    loadPlanoEmissaoFaturamentoValor();
}


function adicionarClick(e, sender) {
    resetarTabs();
    Salvar(e, "PlanoEmissaoFaturamento/Adicionar", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Cadastrado com sucesso");
                _gridPlanoEmissaoFaturamento.CarregarGrid();
                limparCamposPlanoEmissaoFaturamento();
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    }, sender);
}

function atualizarClick(e, sender) {
    resetarTabs();
    Salvar(e, "PlanoEmissaoFaturamento/Atualizar", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Atualizado com sucesso");
                _gridPlanoEmissaoFaturamento.CarregarGrid();
                limparCamposPlanoEmissaoFaturamento();
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    }, sender);
}

function excluirClick(e, sender) {
    exibirConfirmacao("Confirmação", "Realmente deseja excluir o plano de emissão " + _planoEmissaoFaturamento.Descricao.val() + "?", function () {
        ExcluirPorCodigo(_planoEmissaoFaturamento, "PlanoEmissaoFaturamento/ExcluirPorCodigo", function (arg) {
            if (arg.Success) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Excluído com sucesso");
                _gridPlanoEmissaoFaturamento.CarregarGrid();
                limparCamposPlanoEmissaoFaturamento();
            } else {
                exibirMensagem(tipoMensagem.falha, "falha", arg.Msg);
            }
        }, null);
    });
}

function cancelarClick(e) {
    limparCamposPlanoEmissaoFaturamento();
}

//*******MÉTODOS*******


function buscarPlanoEmissaoFaturamentos() {
    var editar = { descricao: "Editar", id: "clasEditar", evento: "onclick", metodo: editarPlanoEmissaoFaturamento, tamanho: "15", icone: "" };
    var menuOpcoes = new Object();
    menuOpcoes.tipo = TypeOptionMenu.link;
    menuOpcoes.opcoes = new Array();
    menuOpcoes.opcoes.push(editar);

    _gridPlanoEmissaoFaturamento = new GridView(_pesquisaPlanoEmissaoFaturamento.Pesquisar.idGrid, "PlanoEmissaoFaturamento/Pesquisa", _pesquisaPlanoEmissaoFaturamento, menuOpcoes, null);
    _gridPlanoEmissaoFaturamento.CarregarGrid();
}

function editarPlanoEmissaoFaturamento(marcaGrid) {
    limparCamposPlanoEmissaoFaturamento();
    _planoEmissaoFaturamento.Codigo.val(marcaGrid.Codigo);
    BuscarPorCodigo(_planoEmissaoFaturamento, "PlanoEmissaoFaturamento/BuscarPorCodigo", function (arg) {
        resetarTabs();
        _pesquisaPlanoEmissaoFaturamento.ExibirFiltros.visibleFade(false);
        _planoEmissaoFaturamento.Atualizar.visible(true);
        _planoEmissaoFaturamento.Cancelar.visible(true);
        _planoEmissaoFaturamento.Excluir.visible(true);
        _planoEmissaoFaturamento.Adicionar.visible(false);
        preencherPlanoEmissaoFaturamentoValor();
    }, null);
}

function limparCamposPlanoEmissaoFaturamento() {
    _planoEmissaoFaturamento.Atualizar.visible(false);
    _planoEmissaoFaturamento.Cancelar.visible(false);
    _planoEmissaoFaturamento.Excluir.visible(false);
    _planoEmissaoFaturamento.Adicionar.visible(true);
    LimparCampos(_planoEmissaoFaturamento);
    preencherPlanoEmissaoFaturamentoValor();
    resetarTabs();
}

function resetarTabs() {
    $("#myTab a:first").tab("show");
}
