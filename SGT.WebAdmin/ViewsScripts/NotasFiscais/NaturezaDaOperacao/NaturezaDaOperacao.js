/// <reference path="MovimentoNotaEntrada.js" />
/// <reference path="../../Consultas/TipoMovimento.js" />
/// <reference path="../../../js/libs/jquery-2.1.1.js" />
/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/bootstrap/bootstrap.js" />
/// <reference path="../../../js/libs/jquery.blockui.js" />
/// <reference path="../../../js/Global/knoutViewsSlides.js" />
/// <reference path="../../../js/libs/jquery.maskMoney.js" />
/// <reference path="../../Consultas/Atividade.js" />
/// <reference path="../../Consultas/Estado.js" />
/// <reference path="../../../js/plugin/datatables/jquery.dataTables.js" />
/// <reference path="../../Enumeradores/EnumCSTICMS.js" />
/// <reference path="../../Consultas/CFOP.js" />
/// <reference path="../../Consultas/NaturezaOperacao.js" />
/// <reference path="../../Enumeradores/EnumFinalidadeTipoMovimento.js" />
/// <reference path="../../Enumeradores/EnumEntradaSaida.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _gridNaturezaDaOperacao, _naturezaDaOperacao, _pesquisaNaturezaDaOperacao, _crudNaturezaOperacao;

var _statusPesquisaNaturezaOperacao = [
    { text: "Todos", value: "" },
    { text: "Ativo", value: "A" },
    { text: "Inativo", value: "I" }
];

var _statusNaturezaOperacao = [
    { text: "Ativo", value: "A" },
    { text: "Inativo", value: "I" }
];

var PesquisaNaturezaDaOperacao = function () {
    this.Descricao = PropertyEntity({ text: "Descrição: " });
    this.Status = PropertyEntity({ val: ko.observable("A"), options: _statusPesquisaNaturezaOperacao, def: true, text: "Status: " });

    this.Pesquisar = PropertyEntity({
        eventClick: function (e) {
            _gridNaturezaDaOperacao.CarregarGrid();
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

var NaturezaDaOperacao = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });

    this.Descricao = PropertyEntity({ text: "*Descrição: ", required: true, maxlength: 60 });
    this.Tipo = PropertyEntity({ val: ko.observable(EnumEntradaSaida.Todos), options: EnumEntradaSaida.obterOpcoesPesquisa(), def: EnumEntradaSaida.Todos, text: "*Tipo: ", required: true });
    this.Status = PropertyEntity({ val: ko.observable("A"), options: _statusNaturezaOperacao, def: "A", text: "*Status: ", required: true });
    this.DentroEstado = PropertyEntity({ val: ko.observable(false), getType: typesKnockout.bool, text: "Dentro do Estado", def: false, visible: ko.observable(true) });
    this.GeraTitulo = PropertyEntity({ val: ko.observable(false), getType: typesKnockout.bool, text: "Gera Título", def: false });
    this.ControlaEstoque = PropertyEntity({ val: ko.observable(false), getType: typesKnockout.bool, text: "Controla Estoque", def: false });
    this.Garantia = PropertyEntity({ val: ko.observable(false), getType: typesKnockout.bool, text: "Garantia", def: false });
    this.Demonstracao = PropertyEntity({ val: ko.observable(false), getType: typesKnockout.bool, text: "Demonstração", def: false });
    this.Bonificacao = PropertyEntity({ val: ko.observable(false), getType: typesKnockout.bool, text: "Bonificação", def: false });
    this.Outras = PropertyEntity({ val: ko.observable(false), getType: typesKnockout.bool, text: "Outras", def: false });
    this.DesconsideraICMSEfetivo = PropertyEntity({ val: ko.observable(false), getType: typesKnockout.bool, text: "Desconsidera ICMS Efetivo", def: false });
    this.Devolucao = PropertyEntity({ val: ko.observable(false), getType: typesKnockout.bool, text: "Devolução", def: false });

    this.Numero = PropertyEntity({ text: "Número (Para Nota Fiscal de Serviço - NFS-e):", maxlength: 20, getType: typesKnockout.int, configInt: { precision: 0, allowZero: true } });

    this.Localidade = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Localidade (Para Nota Fiscal de Serviço - NFS-e):", idBtnSearch: guid() });
    this.TipoMovimento = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: ko.observable("Tipo de Movimento (Para Nota Fiscal de Saída - NF-e):"), idBtnSearch: guid(), required: false, visible: true });
    this.CFOP = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "CFOP (Para Nota Fiscal de Saída - NF-e):", idBtnSearch: guid(), required: false, visible: ko.observable(true) });
    this.Empresa = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Empresa:", idBtnSearch: guid(), required: false, visible: false });

    this.MovimentoAutomaticoEntrada = PropertyEntity({ type: types.map, getType: typesKnockout.dynamic, val: ko.observable(""), def: "" });

    this.CFOPs = PropertyEntity({ type: types.map, getType: typesKnockout.dynamic, val: ko.observable(""), def: "", idGrid: guid() });
};

var CRUDNaturezaOperacao = function () {
    this.Adicionar = PropertyEntity({ eventClick: adicionarClick, type: types.event, text: "Adicionar", visible: ko.observable(true) });
    this.Atualizar = PropertyEntity({ eventClick: atualizarClick, type: types.event, text: "Atualizar", visible: ko.observable(false) });
    this.Excluir = PropertyEntity({ eventClick: excluirClick, type: types.event, text: "Excluir", visible: ko.observable(false) });
    this.Cancelar = PropertyEntity({ eventClick: cancelarClick, type: types.event, text: "Cancelar", visible: ko.observable(false) });
};

//*******EVENTOS*******

function loadNaturezaDaOperacao() {

    _pesquisaNaturezaDaOperacao = new PesquisaNaturezaDaOperacao();
    KoBindings(_pesquisaNaturezaDaOperacao, "knockoutPesquisaNaturezaDaOperacao", false, _pesquisaNaturezaDaOperacao.Pesquisar.id);

    _naturezaDaOperacao = new NaturezaDaOperacao();
    KoBindings(_naturezaDaOperacao, "knockoutCadastroNaturezaDaOperacao");

    _crudNaturezaOperacao = new CRUDNaturezaOperacao();
    KoBindings(_crudNaturezaOperacao, "knockoutCRUDNaturezaOperacao");

    HeaderAuditoria("NaturezaDaOperacao", _naturezaDaOperacao);

    new BuscarCFOPNotaFiscal(_naturezaDaOperacao.CFOP);
    new BuscarTipoMovimento(_naturezaDaOperacao.TipoMovimento, null, null, null, null, EnumFinalidadeTipoMovimento.NaturezaOperacao);
    new BuscarLocalidades(_naturezaDaOperacao.Localidade);

    LoadCFOP();
    LoadMovimentoNotaEntrada();

    buscarNaturezaDaOperacao();

    ConfigurarLayout();
}

function adicionarClick(e, sender) {
    preencherListasSelecao();

    Salvar(_naturezaDaOperacao, "NaturezaDaOperacao/Adicionar", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Cadastrado com sucesso");
                _gridNaturezaDaOperacao.CarregarGrid();
                limparCamposNaturezaDaOperacao();
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    }, sender);
}

function atualizarClick(e, sender) {
    preencherListasSelecao();

    Salvar(_naturezaDaOperacao, "NaturezaDaOperacao/Atualizar", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Atualizado com sucesso");
                _gridNaturezaDaOperacao.CarregarGrid();
                limparCamposNaturezaDaOperacao();
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    }, sender);
}

function excluirClick(e, sender) {
    exibirConfirmacao("Confirmação", "Realmente deseja excluir a natureza da operação " + _naturezaDaOperacao.Descricao.val() + "?", function () {
        ExcluirPorCodigo(_naturezaDaOperacao, "NaturezaDaOperacao/ExcluirPorCodigo", function (arg) {
            if (arg.Success) {
                if (arg.Data) {
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Excluído com sucesso");
                    _gridNaturezaDaOperacao.CarregarGrid();
                    limparCamposNaturezaDaOperacao();
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
    limparCamposNaturezaDaOperacao();
}

//*******MÉTODOS*******

function ConfigurarLayout() {
    if (_CONFIGURACAO_TMS.TipoServicoMultisoftware === EnumTipoServicoMultisoftware.MultiTMS) {
        _naturezaDaOperacao.CFOP.visible(false);
        _naturezaDaOperacao.DentroEstado.visible(true);
    } else if (_CONFIGURACAO_TMS.TipoServicoMultisoftware === EnumTipoServicoMultisoftware.MultiNFe) {
        _naturezaDaOperacao.TipoMovimento.text("Tipo de Movimento (Para Documento de Entrada e Nota Fiscal de Saída - NF-e):");
    }
}

function preencherListasSelecao() {
    var cfops = new Array();
    var configuracoes = new Array();

    $.each(_cfop.CFOP.basicTable.BuscarRegistros(), function (i, cfop) {
        cfops.push({ CFOP: cfop });
    });

    configuracoes.push({
        GerarMovimentoAutomaticoEntrada: _movimentoNotaEntrada.GerarMovimentoAutomaticoEntrada.val(),
        TipoMovimentoUsoEntrada: _movimentoNotaEntrada.TipoMovimentoUsoEntrada.codEntity(),
        TipoMovimentoReversaoEntrada: _movimentoNotaEntrada.TipoMovimentoReversaoEntrada.codEntity()
    });

    _naturezaDaOperacao.CFOPs.val(JSON.stringify(cfops));
    _naturezaDaOperacao.MovimentoAutomaticoEntrada.val(JSON.stringify(configuracoes));
}

function buscarNaturezaDaOperacao() {
    var editar = { descricao: "Editar", id: "clasEditar", evento: "onclick", metodo: editarNaturezaDaOperacao, tamanho: "15", icone: "" };
    var menuOpcoes = new Object();
    menuOpcoes.tipo = TypeOptionMenu.link;
    menuOpcoes.opcoes = new Array();
    menuOpcoes.opcoes.push(editar);

    _gridNaturezaDaOperacao = new GridView(_pesquisaNaturezaDaOperacao.Pesquisar.idGrid, "NaturezaDaOperacao/Pesquisa", _pesquisaNaturezaDaOperacao, menuOpcoes, null);
    _gridNaturezaDaOperacao.CarregarGrid();
}

function editarNaturezaDaOperacao(naturezaDaOperacaoGrid) {
    limparCamposNaturezaDaOperacao();
    _naturezaDaOperacao.Codigo.val(naturezaDaOperacaoGrid.Codigo);
    BuscarPorCodigo(_naturezaDaOperacao, "NaturezaDaOperacao/BuscarPorCodigo", function (arg) {
        _pesquisaNaturezaDaOperacao.ExibirFiltros.visibleFade(false);
        _crudNaturezaOperacao.Atualizar.visible(true);
        _crudNaturezaOperacao.Cancelar.visible(true);
        _crudNaturezaOperacao.Excluir.visible(true);
        _crudNaturezaOperacao.Adicionar.visible(false);
        var dataMovimentoEntrada = { Data: arg.Data };
        PreencherObjetoKnout(_movimentoNotaEntrada, dataMovimentoEntrada);
        RecarregarGridCFOP();
    }, null);
}

function limparCamposNaturezaDaOperacao() {
    _crudNaturezaOperacao.Atualizar.visible(false);
    _crudNaturezaOperacao.Cancelar.visible(false);
    _crudNaturezaOperacao.Excluir.visible(false);
    _crudNaturezaOperacao.Adicionar.visible(true);
    LimparCampos(_naturezaDaOperacao);

    LimparCamposCFOP();
    LimparCamposMovimentoNotaEntrada();
    RecarregarGridCFOP();
    Global.ResetarAbas();
}
