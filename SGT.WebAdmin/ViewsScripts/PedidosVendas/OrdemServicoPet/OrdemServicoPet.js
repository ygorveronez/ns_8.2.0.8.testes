/// <reference path="OrdemServicoPetItens.js" />
/// <reference path="OrdemServicoPetEtapa.js" />
/// <reference path="../../Enumeradores/EnumStatusOrdemServicoPet.js" />

var _ordemServicoPet;
var _pesquisaOrdemServicoPet;
var _gridPesquisaOrdemServicoPet;
var _casasQuantidadeProdutoNFe;
var _casasValorProdutoNFe;
var _bloquearDataEntregaDiferenteAtual;
var _observacaoInterna;

var PesquisaOrdemServicoPet = function () {

    this.DataEmissaoInicial = PropertyEntity({ text: "Data de Chegada Inicial: ", getType: typesKnockout.date });
    this.DataEmissaoFinal = PropertyEntity({ text: "Data de Chegada Final: ", getType: typesKnockout.date });
    this.DataEntregaInicial = PropertyEntity({ text: "Data de Entrega Inicial: ", getType: typesKnockout.date });
    this.DataEntregaFinal = PropertyEntity({ text: "Data de Entrega Final: ", getType: typesKnockout.date });
    this.Status = PropertyEntity({ val: ko.observable(EnumStatusOrdemServicoPet.Todos), options: EnumStatusOrdemServicoPet.obterOpcoesPesquisa(), def: EnumStatusOrdemServicoPet.Todos, text: "Status: " });
    this.Pet = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Pet:", idBtnSearch: guid(), visible: ko.observable(true) });
    this.Pessoa = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Tutor:", idBtnSearch: guid(), visible: ko.observable(true) });
    this.Funcionario = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Funcionário:", idBtnSearch: guid(), visible: ko.observable(true) });
    this.NumeroInicial = PropertyEntity({ text: "Número Inicial: ", getType: typesKnockout.int, maxlength: 16 });
    this.NumeroFinal = PropertyEntity({ text: "Número Final: ", getType: typesKnockout.int, maxlength: 16 });

    this.Tipo = PropertyEntity({ val: ko.observable(EnumTipoPedidoVenda.OrdemServicoPet), visible: ko.observable(false) });

    this.Pesquisar = PropertyEntity({
        eventClick: function (e) {
            _gridPesquisaOrdemServicoPet.CarregarGrid();
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

var OrdemServicoPet = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });

    this.Numero = PropertyEntity({ text: "Número:", maxlength: 10, getType: typesKnockout.int, enable: ko.observable(false), val: ko.observable("") });
    this.DataEmissao = PropertyEntity({ text: "*Data Chegada: ", getType: typesKnockout.dateTime, enable: ko.observable(true), required: true, val: ko.observable(Global.DataHoraAtual()), def: ko.observable(Global.DataHoraAtual()) });
    this.Status = PropertyEntity({ text: "Status: ", val: ko.observable(EnumStatusOrdemServicoPet.Aberto), options: EnumStatusOrdemServicoPet.obterOpcoes(), def: EnumStatusOrdemServicoPet.Aberto, enable: ko.observable(true) });
    this.Funcionario = PropertyEntity({ text: "Funcionário:", type: types.entity, codEntity: ko.observable(0), idBtnSearch: guid(), enable: ko.observable(true), visible: ko.observable(true) });
    this.Pet = PropertyEntity({ text: "*Pet", type: types.entity, codEntity: ko.observable(0), idBtnSearch: guid(), enable: ko.observable(true), visible: ko.observable(true), required: true });
    this.Peso = PropertyEntity({ def: "0,00", val: ko.observable("0,00"), getType: typesKnockout.decimal, text: Localization.Resources.Patrimonio.Pet.Peso.getFieldDescription(), enable: ko.observable(true), maxlength: 10 });
    this.DataEntrega = PropertyEntity({ text: "Data Entrega: ", getType: typesKnockout.dateTime, enable: ko.observable(true), val: ko.observable(Global.DataHoraAtual()), def: ko.observable(Global.DataHoraAtual()) });
    this.Tutor = PropertyEntity({ text: Localization.Resources.Patrimonio.Pet.Tutor.getFieldDescription(), val: ko.observable(''), getType: typesKnockout.string, enable: ko.observable(false) });
    this.Raca = PropertyEntity({ text: Localization.Resources.Configuracoes.EspecieRaca.Raca.getFieldDescription(), val: ko.observable(''), getType: typesKnockout.string, enable: ko.observable(false) });
    this.Sexo = PropertyEntity({ text: Localization.Resources.Patrimonio.Pet.Sexo.getFieldDescription(), val: ko.observable(''), getType: typesKnockout.string, enable: ko.observable(false) });
    this.Cor = PropertyEntity({ text: Localization.Resources.Patrimonio.Pet.Cor.getFieldDescription(), val: ko.observable(''), getType: typesKnockout.string, enable: ko.observable(false) });
    this.FotoPet = PropertyEntity({});
    this.Observacao = PropertyEntity({ text: "Observação:", maxlength: 5000, val: ko.observable(""), enable: ko.observable(true) });
    this.ValorProdutos = PropertyEntity({ def: "0,00", val: ko.observable("0,00"), getType: typesKnockout.decimal, text: "Valor Produtos:", maxlength: 10, enable: ko.observable(false) });
    this.ValorServicos = PropertyEntity({ def: "0,00", val: ko.observable("0,00"), getType: typesKnockout.decimal, text: "Valor Serviços:", maxlength: 10, enable: ko.observable(false) });
    this.ValorTotal = PropertyEntity({ def: "0,00", val: ko.observable("0,00"), getType: typesKnockout.decimal, text: "Valor Total:", maxlength: 10, enable: ko.observable(false) });

    this.Tipo = PropertyEntity({ val: ko.observable(EnumTipoPedidoVenda.OrdemServicoPet), visible: ko.observable(false) });

    this.Limpar = PropertyEntity({ eventClick: limparClick, type: types.event, text: "Limpar", visible: ko.observable(true), enable: ko.observable(true) });
    this.Imprimir = PropertyEntity({ eventClick: imprimirClick, type: types.event, text: "Imprimir", visible: ko.observable(false), enable: ko.observable(true) });
    this.Finalizar = PropertyEntity({ eventClick: finalizarClick, type: types.event, text: "Finalizar", visible: ko.observable(false), enable: ko.observable(true) });
    this.Proximo = PropertyEntity({ eventClick: proximoClick, type: types.event, text: "Próximo", visible: ko.observable(true), enable: ko.observable(true) });

    this.ListaItens = PropertyEntity({ type: types.listEntity, list: new Array(), required: false, codEntity: ko.observable(0), idGrid: guid() });
};

function loadOrdemServicoPet() {
    executarReST("NotaFiscalEletronica/BuscarDadosEmpresa", null, function (r) {
        if (r.Success) {
            _casasQuantidadeProdutoNFe = r.Data.CasasQuantidadeProdutoNFe;
            _casasValorProdutoNFe = r.Data.CasasValorProdutoNFe;
            _bloquearDataEntregaDiferenteAtual = r.Data.BloquearFinalizacaoPedidoVendaDataEntregaDiferenteAtual;

            _ordemServicoPet = new OrdemServicoPet();
            KoBindings(_ordemServicoPet, "knockoutOrdemServicoPet");

            _pesquisaOrdemServicoPet = new PesquisaOrdemServicoPet();
            KoBindings(_pesquisaOrdemServicoPet, "knockoutPesquisaOrdemServicoPet");

            HeaderAuditoria("OrdemServicoPet", _ordemServicoPet);

            new BuscarFuncionario(_ordemServicoPet.Funcionario);
            new BuscarPet(_ordemServicoPet.Pet, retornoPet);

            new BuscarFuncionario(_pesquisaOrdemServicoPet.Funcionario);
            new BuscarPet(_pesquisaOrdemServicoPet.Pet, retornoPesquisaPet);
            new BuscarClientes(_pesquisaOrdemServicoPet.Pessoa);

            buscarOrdensServico();
            loadEtapaPedidoVenda();
            buscarFuncionarioLogado();
            loadOrdemServicoPetItens();

        } else {
            exibirMensagem(tipoMensagem.falha, "Falha!", r.Msg);
        }
    });
}

function retornoPet(args) {

    executarReST("/Pet/BuscarPorCodigo?callback=?", { Codigo: args.Codigo }, function (retorno) {
        if (!retorno.Success) {
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, retorno.Msg);
            return;
        }

        if (!retorno.Data) {
            exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.Aviso, retorno.Msg);
            return;
        }

        let { Codigo, Nome, Tutor, Raca, Sexo, Peso, Cor, FotoPet } = retorno.Data;

        let petSexoDescricao = ''
        if (retorno.Data.Sexo == 0)
            petSexoDescricao = 'Não informado'

        if (retorno.Data.Sexo == 1)
            petSexoDescricao = 'Masculino'

        if (retorno.Data.Sexo == 2)
            petSexoDescricao = 'Feminino'

        _ordemServicoPet.Pet.codEntity(Codigo)
        _ordemServicoPet.Pet.val(Nome)
        _ordemServicoPet.Tutor.val(Tutor.Descricao)
        _ordemServicoPet.Raca.val(Raca.Descricao)
        _ordemServicoPet.Sexo.val(petSexoDescricao)
        if (args.Peso != null && args.Peso > 0)
            _ordemServicoPet.Peso.val(Globalize.format(args.Peso, "n2"))
        else
            _ordemServicoPet.Peso.val(Globalize.format(Peso, "n2"))
        _ordemServicoPet.Cor.val(Cor.Descricao)
        _ordemServicoPet.FotoPet.val(FotoPet)
    });
}

function retornoPesquisaPet(args) {

    let { Codigo, Nome } = args;

    _pesquisaOrdemServicoPet.Pet.codEntity(Codigo)
    _pesquisaOrdemServicoPet.Pet.val(Nome)
}

function buscarFuncionarioLogado() {
    executarReST("OrdemServicoPet/BuscarFuncionarioLogado", null, function (r) {
        if (r.Success) {
            _ordemServicoPet.Funcionario.codEntity(r.Data.Codigo);
            _ordemServicoPet.Funcionario.val(r.Data.Nome);
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha!", r.Msg);
        }
    });
}

function finalizarClick(e, sender) {
    exibirConfirmacao("Confirmação", "Deseja finalizar a Ordem de Serviço " + _ordemServicoPet.Numero.val() + "? Não será mais possível editar!", function () {
        _ordemServicoPet.Status.val(EnumStatusOrdemServicoPet.Finalizado);
        gravarOrdemServicoPetItensClick();
    });
}

function limparClick(e, sender) {
    _gridPesquisaOrdemServicoPet.CarregarGrid();
    limparCamposOrdemServicoPet();
    _ordemServicoPetItens.limparCampos;    
    _ordemServicoPetItens.ValorProdutos.val("0,00");
    _ordemServicoPetItens.ValorServicos.val("0,00");
    _ordemServicoPetItens.ValorTotal.val("0,00");
    _ordemServicoPet.ListaItens.list = new Array();
    recarregarGridListaItens();
    _ordemServicoPet.Tipo.val(EnumTipoPedidoVenda.OrdemServicoPet);
    _ordemServicoPet.Finalizar.visible(false);
    _ordemServicoPet.Limpar.visible(false);
    _ordemServicoPet.Imprimir.visible(false);

    SetarEnableCamposKnockout(_ordemServicoPet, true);
    SetarEnableCamposKnockout(_ordemServicoPetItens, true);
    _ordemServicoPet.Numero.enable(false);

    _ordemServicoPet.Tutor.enable(false);
    _ordemServicoPet.Raca.enable(false);
    _ordemServicoPet.Sexo.enable(false);
    _ordemServicoPet.Cor.enable(false);

    _ordemServicoPet.ValorProdutos.enable(false);
    _ordemServicoPet.ValorServicos.enable(false);
    _ordemServicoPet.ValorTotal.enable(false);
    _ordemServicoPetItens.ValorProdutos.enable(false);
    _ordemServicoPetItens.ValorServicos.enable(false);
    _ordemServicoPetItens.ValorTotal.enable(false);
    _ordemServicoPetItens.ValorTotalItem.enable(false);

    buscarFuncionarioLogado();
}

function imprimirClick(e, sender) {
    var data = { Codigo: _ordemServicoPet.Codigo.val() };
    executarReST("OrdemServicoPet/BaixarRelatorio", data, function (arg) {
        if (arg.Success) {
            if (arg.Data !== false) {
                BuscarProcessamentosPendentes();
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Aguarde que seu relatório está sendo gerado.");
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    })
}

function proximoClick(e, sender) {
    $("#" + _etapaPedidoVenda.Etapa1.idTab + " .step").attr("class", "step green");
    $("#" + _etapaPedidoVenda.Etapa2.idTab + " .step").attr("class", "step lightgreen");
    $("#" + _etapaPedidoVenda.Etapa2.idTab).tab('show');

}

function limparCamposOrdemServicoPet() {
    LimparCampos(_ordemServicoPet);

    //_ordemServicoPet.ValorProdutos.val("0,00");
    //_ordemServicoPet.ValorServicos.val("0,00");
    //_ordemServicoPet.ValorTotal.val("0,00");

    _ordemServicoPet.DataEmissao.val(Global.DataHoraAtual());
    _ordemServicoPet.DataEntrega.val(Global.DataHoraAtual());
}

function buscarOrdensServico() {
    var editar = { descricao: "Editar", id: "clasEditar", evento: "onclick", metodo: editarOrdemServico, tamanho: "20", icone: "" };
    var menuOpcoes = new Object();
    menuOpcoes.tipo = TypeOptionMenu.link;
    menuOpcoes.opcoes = new Array();
    menuOpcoes.opcoes.push(editar);

    _gridPesquisaOrdemServicoPet = new GridView(_pesquisaOrdemServicoPet.Pesquisar.idGrid, "OrdemServicoPet/Pesquisar", _pesquisaOrdemServicoPet, menuOpcoes, null, null, null);
    _gridPesquisaOrdemServicoPet.CarregarGrid();
}

function editarOrdemServico(pedidoVendaGrid) {
    limparClick();
    _ordemServicoPet.Codigo.val(pedidoVendaGrid.Codigo);
    BuscarPorCodigo(_ordemServicoPet, "OrdemServicoPet/BuscarPorCodigo", function (arg) {
        _pesquisaOrdemServicoPet.ExibirFiltros.visibleFade(false);
        retornoPet({ Codigo: arg.Data.PetCodigo, Peso: arg.Data.Peso })
        _ordemServicoPetItens.ValorProdutos.val(_ordemServicoPet.ValorProdutos.val());
        _ordemServicoPetItens.ValorServicos.val(_ordemServicoPet.ValorServicos.val());
        _ordemServicoPetItens.ValorTotal.val(_ordemServicoPet.ValorTotal.val());
        recarregarGridListaItens();

        $("#" + _etapaPedidoVenda.Etapa2.idTab + " .step").attr("class", "step ");
        $("#" + _etapaPedidoVenda.Etapa1.idTab).tab('show');

        _ordemServicoPet.Finalizar.visible(false);
        _ordemServicoPet.Limpar.visible(true);
        _ordemServicoPet.Imprimir.visible(true);
        SetarEnableCamposKnockout(_ordemServicoPet, true);
        SetarEnableCamposKnockout(_ordemServicoPetItens, true);

        if (arg.Data.Status == EnumStatusOrdemServicoPet.Finalizado) {
            SetarEnableCamposKnockout(_ordemServicoPet, false);
            SetarEnableCamposKnockout(_ordemServicoPetItens, false);
        }
        else
            _ordemServicoPet.Finalizar.visible(true);

        _ordemServicoPet.Tutor.enable(false);
        _ordemServicoPet.Raca.enable(false);
        _ordemServicoPet.Sexo.enable(false);
        _ordemServicoPet.Cor.enable(false);

        _ordemServicoPet.Numero.enable(false);
        _ordemServicoPet.ValorProdutos.enable(false);
        _ordemServicoPet.ValorServicos.enable(false);
        _ordemServicoPet.ValorTotal.enable(false);
        _ordemServicoPetItens.ValorProdutos.enable(false);
        _ordemServicoPetItens.ValorServicos.enable(false);
        _ordemServicoPetItens.ValorTotal.enable(false);
        _ordemServicoPetItens.ValorTotalItem.enable(false);
        if (_bloquearDataEntregaDiferenteAtual)
            _ordemServicoPet.DataEmissao.enable(false);

    }, null);
}