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
/// <reference path="Pedido.js" />
/// <reference path="../../Enumeradores/EnumSituacaoCarregamento.js" />
/// <reference path="../../Enumeradores/EnumSituacaoAgendamento.js" />
/// <reference path="../../Consultas/Cliente.js" />
/// <reference path="../../Consultas/Filial.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _pesquisaRetiradaProduto;

var PesquisaRetiradaProduto = function () {

    this.NumeroPedidoEmbarcador = PropertyEntity({ text: Localization.Resources.Pedidos.RetiradaProdutoLista.NumeroPedidoEmbarcador.getFieldDescription(), maxlength: 50, visible: true, required: false, enable: ko.observable(true) });
    this.NomeTransportadora = PropertyEntity({ text: Localization.Resources.Pedidos.RetiradaProdutoLista.NomeTransportadora.getFieldDescription(), maxlength: 150, visible: true, required: false, enable: ko.observable(true) });
    this.SituacaoCarregamento = PropertyEntity({ text: Localization.Resources.Pedidos.RetiradaProdutoLista.SituacaoCarregamento.getFieldDescription(), getType: typesKnockout.options, options: EnumSituacaoCarregamento.obterOpcoesPesquisa(), val: ko.observable(EnumSituacaoCarregamento.Todas), def: EnumSituacaoCarregamento.Todas });
    this.NumeroCarregamento = PropertyEntity({ text: Localization.Resources.Pedidos.RetiradaProdutoLista.NumeroCarregamento.getFieldDescription(), maxlength: 50, visible: true, required: false, enable: ko.observable(true) });
    this.Filial = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), required: false, text: Localization.Resources.Gerais.Geral.Filial.getRequiredFieldDescription(), issue: 70, idBtnSearch: guid(), visible: ko.observable(true) });
    this.Destinatario = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Gerais.Geral.Destinatario.getFieldDescription(), idBtnSearch: guid() });

    this.Situacao = PropertyEntity({ text: Localization.Resources.Gerais.Geral.Situacao.getFieldDescription(), issue: 557, val: ko.observable(true), options: _statusPesquisa, def: true });

    this.SituacaoAgendamento = PropertyEntity({ /*text: Localization.Resources.Pedidos.RetiradaProdutoLista.SituacaoAgendamento.getFieldDescription(),*/ getType: typesKnockout.options, options: EnumSituacaoAgendamento.obterOpcoesPesquisa(), val: ko.observable(EnumSituacaoAgendamento.Todas), def: EnumSituacaoAgendamento.Todas });
    this.Pesquisar = PropertyEntity({
        eventClick: function (e) {
            _gridRetiradaProduto.CarregarGrid();
        }, type: types.event, text: Localization.Resources.Gerais.Geral.Pesquisar, idGrid: guid(), visible: ko.observable(true)
    });

    this.ExibirFiltros = PropertyEntity({
        eventClick: function (e) {
            if (e.ExibirFiltros.visibleFade() == true) {
                e.ExibirFiltros.visibleFade(false);
            } else {
                e.ExibirFiltros.visibleFade(true);
            }
        }, type: types.event, text: Localization.Resources.Gerais.Geral.FiltrosPesquisa, idFade: guid(), visibleFade: ko.observable(false), visible: ko.observable(false)
    });
}


function editarRetiradaProdutoClick(itemGrid) {
    // Limpa os campos

    GlobalCodigoRetiradaProduto = itemGrid.Codigo;
    window.location.hash = "Pedidos/RetiradaProduto";
}


function buscarRetiradaProduto() {
    //-- Grid
    // Opcoes    
    var editar = { descricao: Localization.Resources.Gerais.Geral.Editar, id: "clasEditar", evento: "onclick", metodo: editarRetiradaProdutoClick, tamanho: "7", icone: "" };
    // Menu
    var menuOpcoes = {
        tipo: TypeOptionMenu.link,
        opcoes: [editar]
    };


    var configExportacao = {
        url: "RetiradaProduto/ExportarPesquisa",
        titulo: Localization.Resources.Pedidos.RetiradaProdutoLista.RetiradaProduto
    };


    // Inicia Grid de busca
    _gridRetiradaProduto = new GridViewExportacao(_pesquisaRetiradaProduto.Pesquisar.idGrid, "RetiradaProduto/Pesquisa", _pesquisaRetiradaProduto, menuOpcoes, configExportacao);
    _gridRetiradaProduto.CarregarGrid();
}

function loadRetiradaProdutoLista(gerente) {
    _pesquisaRetiradaProduto = new PesquisaRetiradaProduto();
    /*KoBindings(_pesquisaRetiradaProduto, "knockoutPesquisaRetiradaProduto", false, _pesquisaRetiradaProduto.Pesquisar.id);*/
    KoBindings(_pesquisaRetiradaProduto, "knockoutPesquisaRetiradaProduto", false, _pesquisaRetiradaProduto.Pesquisar.id);

    LocalizeCurrentPage();

    new BuscarFilial(_pesquisaRetiradaProduto.Filial);
    new BuscarClientes(_pesquisaRetiradaProduto.Destinatario);

    if (_CONFIGURACAO_TMS.TipoServicoMultisoftware === EnumTipoServicoMultisoftware.MultiEmbarcador)
        _pesquisaRetiradaProduto.ExibirFiltros.visible(true);
    else
        _pesquisaRetiradaProduto.ExibirFiltros.visible(gerente == 1);

    buscarRetiradaProduto();
}