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
/// <reference path="../../Produtos/ProdutoLote/ProdutoLote.js" />
/// <reference path="../../Enumeradores/EnumStatusVendaDireta.js" />
/// <reference path="../../Enumeradores/EnumProdutoServico.js" />
/// <reference path="../../Enumeradores/EnumTipoAssinaturaVendaDireta.js" />
/// <reference path="../../Enumeradores/EnumStatusPedidoVendaDireta.js" />
/// <reference path="../../Enumeradores/EnumPermissaoPersonalizada.js" />
/// <reference path="../../Enumeradores/EnumTipoCobrancaVendaDireta.js" />
/// <reference path="../../Enumeradores/EnumSimNao.js" />
/// <reference path="../../Enumeradores/EnumStatusCadastro.js" />
/// <reference path="../../Enumeradores/EnumTipoClienteVendaDireta.js" />
/// <reference path="../../Consultas/Usuario.js" />
/// <reference path="../../Consultas/Produto.js" />
/// <reference path="../../Consultas/Cliente.js" />
/// <reference path="../../Consultas/Servico.js" />
/// <reference path="VendaDiretaBoleto.js" />
/// <reference path="VendaDiretaEtapa.js" />
/// <reference path="VendaDiretaParcela.js" />
/// <reference path="VendaDiretaAnexo.js" />
/// <reference path="VendaDiretaContestacaoAnexo.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _gridVendaDireta, _gridVendaDiretaItens, _vendaDireta, _pesquisaVendaDireta, _PermissoesPersonalizadas;
var vendedorVendaDiretaLogado = null;
var _contestacao;
var _gridVendaDiretaContestacaoAnexo;

var PesquisaVendaDireta = function () {
    this.NumeroInicial = PropertyEntity({ text: "Número Inicial: ", getType: typesKnockout.int });
    this.NumeroFinal = PropertyEntity({ text: "Número Final: ", getType: typesKnockout.int });
    this.DataInicial = PropertyEntity({ text: "Data Inicial: ", getType: typesKnockout.date });
    this.DataFinal = PropertyEntity({ text: "Data Final: ", getType: typesKnockout.date });
    this.Status = PropertyEntity({ text: "Status:", val: ko.observable(EnumStatusVendaDireta.Todos), options: EnumStatusVendaDireta.obterOpcoesPesquisa(), def: EnumStatusVendaDireta.Todos });
    this.StatusPedidoVendaDireta = PropertyEntity({ text: "Status Pedido:", val: ko.observable(EnumStatusPedidoVendaDireta.Todos), options: EnumStatusPedidoVendaDireta.obterOpcoesPesquisa(), def: EnumStatusPedidoVendaDireta.Todos, visible: ko.observable(true) });    
    this.Cliente = PropertyEntity({ text: "Cliente:", type: types.multiplesEntities, codEntity: ko.observable(0), idBtnSearch: guid(), val: ko.observable("") });

    this.Funcionario = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Funcionário:", idBtnSearch: guid() });
    this.Agendador = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Agendador:", idBtnSearch: guid() });
    this.FuncionarioValidador = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Validador:", idBtnSearch: guid() });    
    this.DataInicialVencimentoCertificado = PropertyEntity({ text: "Vencimento Inicial: ", getType: typesKnockout.date });
    this.DataFinalVencimentoCertificado = PropertyEntity({ text: "Vencimento Final: ", getType: typesKnockout.date });
    this.ProdutoServico = PropertyEntity({ text: "Tipo:", val: ko.observable(EnumProdutoServico.Todos), options: EnumProdutoServico.obterOpcoesPesquisa(), def: EnumProdutoServico.Todos, visible: ko.observable(true) });

    this.Pesquisar = PropertyEntity({
        eventClick: function (e) {
            _gridVendaDireta.CarregarGrid();
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

var VendaDiretaContestacaoAnexoMap = function () {
    this.Codigo = PropertyEntity({ type: types.map, val: ko.observable(0), def: ko.observable(0), getType: typesKnockout.int });
    this.DescricaoAnexo = PropertyEntity({ type: types.map, val: ko.observable("") });
    this.Arquivo = PropertyEntity({ type: types.map, val: ko.observable("") });
}

var Contestacao = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.FuncionarioContestacao = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), required: true, text: "*Funcionário:", idBtnSearch: guid(), enable: ko.observable(false), visible: ko.observable(true) });
    this.DataContestacao = PropertyEntity({ text: "*Data Contestação: ", getType: typesKnockout.date, enable: ko.observable(true), required: true, val: ko.observable(Global.DataAtual()), def: ko.observable(Global.DataAtual()) });
    this.ObservacaoContestacao = PropertyEntity({ text: "Observação:", maxlength: 5000, enable: ko.observable(true), val: ko.observable("") });

    this.Grid = PropertyEntity({ type: types.local });
    this.DescricaoAnexo = PropertyEntity({ text: "*Descrição Anexo: ", enable: ko.observable(true), required: ko.observable(false), maxlength: 500 });
    this.Arquivo = PropertyEntity({ type: types.file, codEntity: ko.observable(0), text: "*Anexo:", val: ko.observable(""), enable: ko.observable(true), required: ko.observable(false), visible: ko.observable(true) });
    this.Adicionar = PropertyEntity({ eventClick: AdicionarVendaDiretaContestacaoAnexoClick, type: types.event, text: "Adicionar Anexo", enable: ko.observable(true), visible: ko.observable(true) });

    this.SalvarContestacao = PropertyEntity({ eventClick: SalvarContestacaoClick, type: types.event, text: "Salvar Contestação", visible: ko.observable(true) });

    this.ListaAnexosContestacaoExcluidos = PropertyEntity({ type: types.listEntity, list: new Array(), required: false, codEntity: ko.observable(0), idGrid: guid() });
};

var VendaDireta = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Numero = PropertyEntity({ text: "Número: ", val: ko.observable(""), def: "", enable: ko.observable(false) });
    this.Data = PropertyEntity({ text: "Data Venda: ", val: ko.observable(Global.DataAtual()), def: ko.observable(Global.DataAtual()), enable: ko.observable(false) });
    this.Funcionario = PropertyEntity({ text: "Funcionário:", enable: ko.observable(false) });
    this.Status = PropertyEntity({ text: "*Status:", val: ko.observable(EnumStatusVendaDireta.Pendente), options: EnumStatusVendaDireta.obterOpcoes(), def: EnumStatusVendaDireta.Pendente, enable: ko.observable(true) });
    this.Observacao = PropertyEntity({ text: "Observação:", maxlength: 5000, val: ko.observable(""), enable: ko.observable(true) });
    this.ValorTotal = PropertyEntity({ text: "Valor Total:", getType: typesKnockout.decimal, maxlength: 15, val: ko.observable("0,00"), def: "0,00", configDecimal: { precision: 2, allowZero: false, allowNegative: false }, enable: ko.observable(false) });

    this.Tipo = PropertyEntity({ text: "Tipo:", val: ko.observable(EnumProdutoServico.Produto), options: EnumProdutoServico.obterOpcoes(), def: EnumProdutoServico.Produto, enable: ko.observable(true), eventChange: function () { tipoItemVendaDiretaChange(true) } });

    //produto
    this.DataAgendamento = PropertyEntity({ text: "Data Agendamento: ", getType: typesKnockout.dateTime, enable: ko.observable(true), visible: ko.observable(true) });
    this.DataVencimentoCertificado = PropertyEntity({ text: "*Vencimento Certificado: ", getType: typesKnockout.date, enable: ko.observable(true), required: ko.observable(true), visible: ko.observable(true) });
    this.DataVencimentoCobranca = PropertyEntity({ text: "Vencimento Cobrança: ", getType: typesKnockout.date, enable: ko.observable(true), visible: ko.observable(true) });
    this.NumeroPedido = PropertyEntity({ text: "Número Pedido: ", val: ko.observable(""), def: "", maxlength: 500, enable: ko.observable(true), visible: ko.observable(true) });
    this.CodigoEmissao1 = PropertyEntity({ text: "Código Emissão 1: ", val: ko.observable(""), def: "", maxlength: 500, enable: ko.observable(true), visible: ko.observable(true) });
    this.CodigoEmissao2 = PropertyEntity({ text: "Código Emissão 2: ", val: ko.observable(""), def: "", maxlength: 500, enable: ko.observable(true), visible: ko.observable(true) });
    this.TipoAssinatura = PropertyEntity({ text: "Tipo Assinatura:", val: ko.observable(0), options: EnumTipoAssinaturaVendaDireta.obterOpcoes(), def: 0, enable: ko.observable(true), visible: ko.observable(true) });
    this.StatusPedido = PropertyEntity({ text: "Status Pedido:", val: ko.observable(""), options: EnumStatusPedidoVendaDireta.obterOpcoes(), def: "", enable: ko.observable(true), visible: ko.observable(true) });
    this.TipoCobrancaVendaDireta = PropertyEntity({ text: "Tipo de Cobrança da Venda:", val: ko.observable(EnumTipoCobrancaVendaDireta.NaoInformado), options: EnumTipoCobrancaVendaDireta.obterOpcoes(), def: EnumTipoCobrancaVendaDireta.NaoInformado, enable: ko.observable(true), visible: ko.observable(true) });
    this.GerarNF = PropertyEntity({ val: ko.observable(false), getType: typesKnockout.bool, text: "Necessário gerar NF?", def: false, enable: ko.observable(true), visible: ko.observable(true) });
    this.Agendador = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Agendador:", idBtnSearch: guid(), required: ko.observable(false), visible: ko.observable(true), enable: ko.observable(true) });
    this.FuncionarioValidador = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Validador:", idBtnSearch: guid(), required: ko.observable(false), visible: ko.observable(true), enable: ko.observable(true) });    
    this.DataValidacao = PropertyEntity({ text: "Data Validação: ", getType: typesKnockout.date, enable: ko.observable(true), visible: ko.observable(true) });    

    //serviços
    this.TipoCobrancaServico = PropertyEntity({ text: "Tipo de Cobrança:", val: ko.observable(EnumTipoCobrancaVendaDireta.Boleto), options: EnumTipoCobrancaVendaDireta.obterOpcoesServico(), def: EnumTipoCobrancaVendaDireta.Boleto, enable: ko.observable(true), visible: ko.observable(false) });
    this.DataTreinamento = PropertyEntity({ text: "Data Treinamento: ", getType: typesKnockout.dateTime, enable: ko.observable(true), visible: ko.observable(false) });
    this.FuncionarioTreinamento = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Quem treinou:", idBtnSearch: guid(), required: ko.observable(false), visible: ko.observable(false), enable: ko.observable(true) });
    this.StatusCadastro = PropertyEntity({ text: "Status Cadastro:", val: ko.observable(EnumSatusCadastro.Pendente), options: EnumSatusCadastro.obterOpcoes(), def: EnumSatusCadastro.Pendente, enable: ko.observable(true), visible: ko.observable(false) });
    this.TipoCliente = PropertyEntity({ text: "Tipo Cliente:", val: ko.observable(EnumTipoClienteVendaDireta.Web), options: EnumTipoClienteVendaDireta.obterOpcoes(), def: EnumTipoClienteVendaDireta.Web, enable: ko.observable(true), visible: ko.observable(false) });
    this.EmitidoDocumentos = PropertyEntity({ text: "Emitido Documento:", val: ko.observable(EnumSimNao.Nao), options: EnumSimNao.obterOpcoes(), def: EnumSimNao.Nao, enable: ko.observable(true), visible: ko.observable(false) });
    this.Pendencia = PropertyEntity({ text: "Pendência:", val: ko.observable(EnumSimNao.Sim), options: EnumSimNao.obterOpcoes(), def: EnumSimNao.Sim, enable: ko.observable(true), visible: ko.observable(false) });
    this.Certificado = PropertyEntity({ val: ko.observable(false), getType: typesKnockout.bool, text: "Certificado", def: false, enable: ko.observable(true), visible: ko.observable(false) });

    //Lançamento de Itens
    this.CodigoItem = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.CodigoProduto = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.CodigoServico = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.DescricaoItem = PropertyEntity({ val: ko.observable(""), def: "" });
    this.GrupoProdutoItem = PropertyEntity({ val: ko.observable(""), def: "" });
    this.ValorMinimoVenda = PropertyEntity({ val: ko.observable("0,00"), def: "0,00", getType: typesKnockout.decimal });
    this.CodigoTabelaPreco = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });

    this.Quantidade = PropertyEntity({ text: "*Quantidade:", getType: typesKnockout.decimal, maxlength: 18, val: ko.observable(""), def: "", configDecimal: { precision: 2, allowZero: false, allowNegative: false }, enable: ko.observable(true), required: ko.observable(true) });
    this.ValorUnitario = PropertyEntity({ text: "*Valor Unitário:", getType: typesKnockout.decimal, maxlength: 18, val: ko.observable(""), def: "", configDecimal: { precision: 2, allowZero: false, allowNegative: false }, enable: ko.observable(false), required: ko.observable(true) });
    this.ValorDesconto = PropertyEntity({ text: "Valor Desconto:", getType: typesKnockout.decimal, maxlength: 18, val: ko.observable("0,00"), def: "0,00", configDecimal: { precision: 2, allowZero: true, allowNegative: false }, enable: ko.observable(true), required: ko.observable(false) });
    this.ValorTotalItem = PropertyEntity({ text: "*Valor Total:", getType: typesKnockout.decimal, maxlength: 18, val: ko.observable("0,00"), def: "0,00", enable: ko.observable(false) });
    this.SalvarItem = PropertyEntity({ eventClick: salvarItemClick, type: types.event, text: "Salvar Item", enable: ko.observable(true), visible: ko.observable(true) });
    this.Grid = PropertyEntity({ type: types.map, getType: typesKnockout.dynamic, val: ko.observable(""), def: "", idGrid: guid() });

    this.Cliente = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "*Cliente:", idBtnSearch: guid(), required: ko.observable(true), visible: ko.observable(true), enable: ko.observable(true) });
    this.Produto = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "*Produto:", idBtnSearch: guid(), required: ko.observable(true), visible: ko.observable(true), enable: ko.observable(true) });
    this.Servico = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "*Serviço:", idBtnSearch: guid(), required: ko.observable(false), visible: ko.observable(false), enable: ko.observable(true) });

    this.Itens = PropertyEntity({ type: types.listEntity, list: new Array(), required: false, codEntity: ko.observable(0), idGrid: guid() });
    this.Parcelas = PropertyEntity({ type: types.listEntity, list: new Array(), required: false, codEntity: ko.observable(0), idGrid: guid() });
    this.TipoCobranca = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.BoletoConfiguracao = PropertyEntity({ type: types.entity, codEntity: ko.observable(0) });
    this.ListaAnexosContestacao = PropertyEntity({ type: types.listEntity, list: new Array(), required: false, codEntity: ko.observable(0), idGrid: guid() });
    this.ListaAnexosContestacaoNovos = PropertyEntity({ type: types.listEntity, list: new Array(), required: false, codEntity: ko.observable(0), idGrid: guid() });
    

    this.ListaAnexos = PropertyEntity({ type: types.listEntity, list: new Array(), required: false, codEntity: ko.observable(0), idGrid: guid() });
    this.ListaAnexosNovos = PropertyEntity({ type: types.listEntity, list: new Array(), required: false, codEntity: ko.observable(0), idGrid: guid() });
    this.ListaAnexosExcluidos = PropertyEntity({ type: types.listEntity, list: new Array(), required: false, codEntity: ko.observable(0), idGrid: guid() });

    this.DataAgendamento.val.subscribe(function () {
        calcularDataVencimentoCobranca();
    });
};

var CRUDVendaDireta = function () {
    this.Contestar = PropertyEntity({ eventClick: ContestarClick, type: types.event, text: "Contestar", visible: ko.observable(false) });
    this.Adicionar = PropertyEntity({ eventClick: adicionarClick, type: types.event, text: "Adicionar", visible: ko.observable(true) });
    this.Atualizar = PropertyEntity({ eventClick: atualizarClick, type: types.event, text: "Atualizar", visible: ko.observable(false) });
    this.Limpar = PropertyEntity({ eventClick: limparClick, type: types.event, text: "Limpar / Nova Venda", visible: ko.observable(false) });
    this.Cancelar = PropertyEntity({ eventClick: cancelarClick, type: types.event, text: "Cancelar", visible: ko.observable(false) });
    this.Contestar = PropertyEntity({ eventClick: ContestarClick, type: types.event, text: "Contestar", visible: ko.observable(false), enable: ko.observable(true) });
};

//*******EVENTOS*******


function loadVendaDireta() {
    _vendaDireta = new VendaDireta();
    KoBindings(_vendaDireta, "knockoutCadastroVendaDireta");

    HeaderAuditoria("VendaDireta", _vendaDireta);

    _crudVendaDireta = new CRUDVendaDireta();
    KoBindings(_crudVendaDireta, "knockoutCRUDVendaDireta");

    _pesquisaVendaDireta = new PesquisaVendaDireta();
    KoBindings(_pesquisaVendaDireta, "knockoutPesquisaVendaDireta", false, _pesquisaVendaDireta.Pesquisar.id);

    _contestacao = new Contestacao();
    KoBindings(_contestacao, "knockoutContestacao");
    var baixarAnexo = { descricao: "Baixar", id: guid(), metodo: BaixarVendaDiretaContestacaoAnexoClick, icone: "" };
    var excluirAnexo = { descricao: "Excluir", id: guid(), metodo: ExcluirVendaDiretaContestacaoAnexoClick, icone: "" };
    var menuOpcoes = { tipo: TypeOptionMenu.list, descricao: "Opções", tamanho: 15, opcoes: [baixarAnexo, excluirAnexo] };

    var header = [
        { data: "Codigo", visible: false },
        { data: "DescricaoAnexo", title: "Descrição", width: "40%" },
        { data: "Arquivo", title: "Caminho / Nome Arquivo", width: "50%" }];

    _gridVendaDiretaContestacaoAnexo = new BasicDataTable(_contestacao.Grid.id, header, menuOpcoes, { column: 1, dir: orderDir.asc });

    RecarregarGridVendaDiretaContestacaoAnexo();

    new BuscarClientes(_pesquisaVendaDireta.Cliente);
    new BuscarFuncionario(_pesquisaVendaDireta.Funcionario);
    new BuscarFuncionario(_pesquisaVendaDireta.Agendador);
    new BuscarFuncionario(_pesquisaVendaDireta.FuncionarioValidador);    

    new BuscarFuncionario(_vendaDireta.Agendador);
    new BuscarFuncionario(_vendaDireta.FuncionarioValidador);    
    new BuscarFuncionario(_vendaDireta.FuncionarioTreinamento);
    new BuscarClientes(_vendaDireta.Cliente, null, true);
    new BuscarProdutoTMSTabelaPreco(_vendaDireta.Produto, function (data) {
        _vendaDireta.Produto.codEntity(data.Codigo);
        _vendaDireta.Produto.val(data.Descricao);
        _vendaDireta.GrupoProdutoItem.val(data.GrupoProduto);
        _vendaDireta.CodigoTabelaPreco.val(data.CodigoTabelaPreco);
        if (data.ValorVenda != null && data.ValorVenda != "" && Globalize.parseFloat(data.ValorVenda) > 0) {
            _vendaDireta.ValorUnitario.val(data.ValorVenda);
            _vendaDireta.ValorMinimoVenda.val(data.ValorMinimoVenda);
        }
        if (data.CodigoTabelaPreco > 0)
            _vendaDireta.ValorDesconto.enable(false);
        _vendaDireta.Quantidade.val("1,00");
        $("#" + _vendaDireta.Quantidade.id).focus();
    }, _vendaDireta.Cliente, true);
    new BuscarServicoTMS(_vendaDireta.Servico, function (data) {
        _vendaDireta.Servico.codEntity(data.Codigo);
        _vendaDireta.Servico.val(data.Descricao);
        if (data.ValorVenda != null && data.ValorVenda != "" && Globalize.parseFloat(data.ValorVenda) > 0) {
            _vendaDireta.ValorUnitario.val(data.ValorVenda);
        }
        _vendaDireta.Quantidade.val("1,00");
        $("#" + _vendaDireta.Quantidade.id).focus();
    });

    var menuOpcoes = { tipo: TypeOptionMenu.link, tamanho: 7, opcoes: [{ descricao: "Remover", id: guid(), metodo: RemoverItemVendaDiretaClick }] };

    var header = [
        { data: "CodigoItem", visible: false },
        { data: "CodigoProduto", visible: false },
        { data: "CodigoServico", visible: false },
        { data: "DescricaoItem", title: "Produto / Serviço", width: "32%", orderable: true },
        { data: "GrupoProdutoItem", title: "Grupo", width: "15%" },
        { data: "Quantidade", title: "Quantidade", width: "10%" },
        { data: "ValorUnitario", title: "Valor Unit.", width: "10%" },
        { data: "ValorDesconto", title: "Valor Desc.", width: "10%" },
        { data: "ValorTotalItem", title: "Total", width: "10%" },
        { data: "CodigoTabelaPreco", visible: false },
    ];

    _gridVendaDiretaItens = new BasicDataTable(_vendaDireta.Grid.id, header, menuOpcoes, { column: 1, dir: orderDir.asc });

    RecarregarGridVendaDiretaItens();

    buscarVendaDireta();
    LoadEtapasVendaDireta();
    loadVendaDiretaParcela();
    loadVendaDiretaAnexo();
    loadVendaDiretaBoleto();
    loadVendaDiretaContestacaoAnexo();
    PreencheVendedorLogado();
    tipoItemVendaDiretaChange(true);
}

function adicionarClick(e, sender) {
    alterarRequiredCamposObrigatorios(false);
    preencherCamposVendaDiretaOutrasAbas();

    if (ValidarCamposPreenchidos()) {
        Salvar(_vendaDireta, "VendaDireta/Adicionar", function (arg) {
            if (arg.Success) {
                if (arg.Data) {
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Cadastrado com sucesso");
                    if (_vendaDireta.ListaAnexosNovos.list.length > 0)
                        EnviarVendaDiretaAnexos(arg.Data.Codigo);
                    else {
                        _gridVendaDireta.CarregarGrid();
                        limparCamposVendaDireta();
                    }
                } else {
                    exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
            }
        }, sender);
    }
}

function ContestarClick(e, sender) {
    Global.abrirModal('divContestacaoAnexo');
    Global.ResetarAba("divContestacaoAnexo");
}

function atualizarClick(e, sender) {
    alterarRequiredCamposObrigatorios(false);
    preencherCamposVendaDiretaOutrasAbas();

    if (ValidarCamposPreenchidos()) {
        if (_vendaDireta.ListaAnexosExcluidos.list.length > 0)
            _vendaDireta.ListaAnexosExcluidos.text = JSON.stringify(_vendaDireta.ListaAnexosExcluidos.list);

        Salvar(_vendaDireta, "VendaDireta/Atualizar", function (arg) {
            if (arg.Success) {
                if (arg.Data) {
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Atualizado com sucesso");
                    if (_vendaDireta.ListaAnexosNovos.list.length > 0)
                        EnviarVendaDiretaAnexos(_vendaDireta.Codigo.val());
                    else {
                        _gridVendaDireta.CarregarGrid();
                        limparCamposVendaDireta();
                    }
                } else {
                    exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, "falha", arg.Msg);
            }
        }, sender);
    }
}

function cancelarClick(e) {
    exibirConfirmacao("Atenção!", "Deseja realmente cancelar a venda de número " + _vendaDireta.Numero.val() + "?", function () {
        executarReST("VendaDireta/Estornar", { Codigo: _vendaDireta.Codigo.val() }, function (arg) {
            if (arg.Success) {
                if (arg.Data) {
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Cancelado com sucesso.");
                    _gridVendaDireta.CarregarGrid();
                    limparCamposVendaDireta();
                    new LancarProdutoLote();
                } else {
                    exibirMensagem(tipoMensagem.aviso, "Atenção", arg.Msg);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
            }
        }, null);
    });
}

function ContestarClick(e, sender) {
    executarReST("VendaDireta/CarregarContestacao", { Codigo: _vendaDireta.Codigo.val() }, function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                LimparCamposVendaDiretaContestacaoAnexo();
                _contestacao.Codigo.val(_vendaDireta.Codigo.val());
                _contestacao.FuncionarioContestacao.codEntity(arg.Data.CodigoFuncionarioContestacao);
                _contestacao.FuncionarioContestacao.val(arg.Data.FuncionarioContestacao);
                _contestacao.DataContestacao.val(arg.Data.DataContestacao);
                _contestacao.ObservacaoContestacao.val(arg.Data.ObservacaoContestacao);

                //_contestacao.ListaAnexosContestacao.list = arg.Data.ListaAnexosContestacao;
                //_contestacao.ListaAnexosContestacao.list.push(arg.Data.ListaAnexosContestacao);

                RecarregarGridVendaDiretaContestacaoAnexo();

                Global.abrirModal('divContestacao');
            } else {
                exibirMensagem(tipoMensagem.aviso, "Atenção", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    }, null);

}

function preencherCamposVendaDiretaOutrasAbas() {
    _vendaDireta.TipoCobranca.val(_vendaDiretaParcela.TipoCobranca.val());
    _vendaDireta.BoletoConfiguracao.codEntity(_vendaDiretaParcela.BoletoConfiguracao.codEntity());
    _vendaDireta.BoletoConfiguracao.val(_vendaDiretaParcela.BoletoConfiguracao.val());
}

function ValidarCamposPreenchidos() {
    var tudoCerto = true;
    var valido = ValidarCamposObrigatorios(_vendaDireta);

    if (!valido) {
        exibirMensagem(tipoMensagem.atencao, "Campos Obrigatórios", "Informe os campos obrigatórios!");
        tudoCerto = false;
    } else if (_vendaDireta.Itens.list.length == 0 && _vendaDireta.Status.val() == EnumStatusVendaDireta.Finalizado) {
        exibirMensagem(tipoMensagem.atencao, "Campos Obrigatórios", "Favor informar ao menos um item para finalizar a venda!");
        tudoCerto = false;
    } else if (_vendaDireta.TipoCobrancaVendaDireta.val() != EnumTipoCobrancaVendaDireta.Bonificado && _vendaDireta.Parcelas.list.length == 0 && _vendaDireta.Status.val() == EnumStatusVendaDireta.Finalizado) {
        exibirMensagem(tipoMensagem.atencao, "Campos Obrigatórios", "Favor informar ao menos uma parcela para finalizar a venda!");
        tudoCerto = false;
    } else if (_vendaDireta.TipoCobrancaVendaDireta.val() != EnumTipoCobrancaVendaDireta.Bonificado && _vendaDireta.Status.val() == EnumStatusVendaDireta.Finalizado && (Globalize.parseFloat(_vendaDireta.ValorTotal.val()) != Globalize.parseFloat(_vendaDiretaParcela.ValorTotal.val()))) {
        exibirMensagem(tipoMensagem.atencao, "Campos Obrigatórios", "Valor total da venda difere do total das parcelas!");
        tudoCerto = false;
    }

    return tudoCerto;
}

function limparClick(e, sender) {
    limparCamposVendaDireta();
}

function salvarItemClick() {
    alterarRequiredCamposObrigatorios(true);

    var valido = ValidarCamposObrigatorios(_vendaDireta);
    if (valido) {
        _vendaDireta.CodigoItem.val(guid());
        _vendaDireta.CodigoProduto.val(_vendaDireta.Produto.codEntity());
        _vendaDireta.CodigoServico.val(_vendaDireta.Servico.codEntity());
        if (_vendaDireta.Produto.codEntity() > 0)
            _vendaDireta.DescricaoItem.val(_vendaDireta.Produto.val());
        else
            _vendaDireta.DescricaoItem.val(_vendaDireta.Servico.val());
        _vendaDireta.Itens.list.push(SalvarListEntity(_vendaDireta));

        atualizarTotalizarVenda();
        limparCamposVendaDiretaItens();
    } else {
        exibirMensagem(tipoMensagem.atencao, "Campos Obrigatórios", "Informe os campos obrigatórios!");
    }
}

function tipoItemVendaDiretaChange(limparValores) {
    if (limparValores) {
        _vendaDireta.ValorDesconto.val("0,00");
        _vendaDireta.ValorUnitario.val("");
    }

    if (_vendaDireta.Tipo.val() == EnumProdutoServico.Produto) {
        _vendaDireta.Servico.visible(false);
        _vendaDireta.Servico.required(false);
        _vendaDireta.Produto.visible(true);
        _vendaDireta.Produto.required(true);
        LimparCampoEntity(_vendaDireta.Servico);
        _vendaDireta.ValorDesconto.enable(true);

        //produtos
        _vendaDireta.DataAgendamento.visible(true);
        _vendaDireta.DataVencimentoCertificado.visible(true);
        _vendaDireta.DataVencimentoCertificado.required(true);
        _vendaDireta.DataVencimentoCobranca.visible(true);
        _vendaDireta.DataValidacao.visible(true);        
        _vendaDireta.NumeroPedido.visible(true);
        _vendaDireta.CodigoEmissao1.visible(true);
        _vendaDireta.CodigoEmissao2.visible(true);
        _vendaDireta.TipoAssinatura.visible(true);
        _vendaDireta.StatusPedido.visible(true);
        _vendaDireta.TipoCobrancaVendaDireta.visible(true);
        _vendaDireta.GerarNF.visible(true);
        _vendaDireta.Agendador.visible(true);
        _vendaDireta.FuncionarioValidador.visible(true);

        //serviços
        _vendaDireta.TipoCobrancaServico.visible(false);
        _vendaDireta.DataTreinamento.visible(false);
        _vendaDireta.FuncionarioTreinamento.visible(false);
        _vendaDireta.StatusCadastro.visible(false);
        _vendaDireta.TipoCliente.visible(false);
        _vendaDireta.EmitidoDocumentos.visible(false);
        _vendaDireta.Pendencia.visible(false);
        _vendaDireta.Certificado.visible(false);

    }
    else {
        _vendaDireta.Servico.visible(true);
        _vendaDireta.Servico.required(true);
        _vendaDireta.Produto.visible(false);
        _vendaDireta.Produto.required(false);
        LimparCampoEntity(_vendaDireta.Produto);
        _vendaDireta.ValorDesconto.enable(false);

        //produtos
        _vendaDireta.DataAgendamento.visible(false);
        _vendaDireta.DataVencimentoCertificado.visible(false);
        _vendaDireta.DataVencimentoCertificado.required(false);
        _vendaDireta.DataVencimentoCobranca.visible(false);
        _vendaDireta.DataValidacao.visible(false);
        _vendaDireta.NumeroPedido.visible(false);
        _vendaDireta.CodigoEmissao1.visible(false);
        _vendaDireta.CodigoEmissao2.visible(false);
        _vendaDireta.TipoAssinatura.visible(false);
        _vendaDireta.StatusPedido.visible(false);
        _vendaDireta.TipoCobrancaVendaDireta.visible(false);
        _vendaDireta.GerarNF.visible(false);
        _vendaDireta.Agendador.visible(false);
        _vendaDireta.FuncionarioValidador.visible(false);

        //serviços
        _vendaDireta.TipoCobrancaServico.visible(true);
        _vendaDireta.DataTreinamento.visible(true);
        _vendaDireta.FuncionarioTreinamento.visible(true);
        _vendaDireta.StatusCadastro.visible(true);
        _vendaDireta.TipoCliente.visible(true);
        _vendaDireta.EmitidoDocumentos.visible(true);
        _vendaDireta.Pendencia.visible(true);
        _vendaDireta.Certificado.visible(true);
    }
}

function alterarRequiredCamposObrigatorios(salvarItem) {
    if (salvarItem) {
        tipoItemVendaDiretaChange(false);
        _vendaDireta.Quantidade.required(true);
        _vendaDireta.ValorUnitario.required(true);

        _vendaDireta.Cliente.required(false);
        _vendaDireta.DataVencimentoCertificado.required(false);
    }
    else {
        _vendaDireta.Cliente.required(true);
        if (_vendaDireta.Tipo.val() == EnumProdutoServico.Produto)
            _vendaDireta.DataVencimentoCertificado.required(true);

        _vendaDireta.Servico.required(false);
        _vendaDireta.Produto.required(false);
        _vendaDireta.Quantidade.required(false);
        _vendaDireta.ValorUnitario.required(false);
    }
}

function calcularDataVencimentoCobranca() {
    var dataAgendamento = _vendaDireta.DataAgendamento.val();
    if (dataAgendamento != "" && dataAgendamento != "  /  /    ") {
        var dataAgendamentoConvertida = dataAgendamento.substr(6, 4) + "/" + dataAgendamento.substr(3, 2) + "/" + dataAgendamento.substr(0, 2);

        var dataConvertida = new Date(dataAgendamentoConvertida);
        dataConvertida.setDate(dataConvertida.getDate() + 3);

        _vendaDireta.DataVencimentoCobranca.val(moment(dataConvertida).format("DD/MM/YYYY"));
    }
}

function calcularTotalItemVendaDireta() {
    var quantidade = Globalize.parseFloat(_vendaDireta.Quantidade.val());
    var valorUnitario = Globalize.parseFloat(_vendaDireta.ValorUnitario.val());
    var valorDesconto = Globalize.parseFloat(_vendaDireta.ValorDesconto.val());
    var valorVendaMinima = Globalize.parseFloat(_vendaDireta.ValorMinimoVenda.val());

    if (quantidade > 0 && valorUnitario > 0) {
        var valorTotal = quantidade * valorUnitario;

        if (valorDesconto < valorTotal && valorDesconto > 0) {
            valorTotal = valorTotal - valorDesconto;

            if ((valorTotal / quantidade) < valorVendaMinima) {
                _vendaDireta.ValorDesconto.val(Globalize.format(0, "n2"));
                exibirMensagem(tipoMensagem.atencao, "Desconto Máximo", "Desconto informado maior que o permitido!");
                calcularTotalItemVendaDireta();
                return;
            }
        }
        else
            _vendaDireta.ValorDesconto.val(Globalize.format(0, "n2"));

        _vendaDireta.ValorTotalItem.val(Globalize.format(valorTotal, "n2"));
    }
}

function atualizarTotalizarVenda() {
    var valorItems = 0;

    for (var i = 0; i < _vendaDireta.Itens.list.length; i++) {
        valorItems = valorItems + Globalize.parseFloat(_vendaDireta.Itens.list[i].ValorTotalItem.val);
    }
    _vendaDireta.ValorTotal.val(Globalize.format(valorItems, "n2"));
}

//*******MÉTODOS*******

function PreencheVendedorLogado() {
    var _fillName = function () {
        _vendaDireta.Funcionario.val(vendedorVendaDiretaLogado.Nome);
    }

    if (vendedorVendaDiretaLogado != null) _fillName();

    executarReST("Usuario/DadosUsuarioLogado", {}, function (arg) {
        if (arg.Success) {
            if (arg.Data !== false && arg.Data != null) {
                vendedorVendaDiretaLogado = {
                    Nome: arg.Data.Nome
                };
                _fillName();
            }
        }
    });
}

function RecarregarGridVendaDiretaItens() {
    var data = new Array();

    _vendaDireta.Cliente.enable(true);
    if (_vendaDireta.Itens.list.length > 0)
        _vendaDireta.Cliente.enable(false);

    $.each(_vendaDireta.Itens.list, function (i, item) {
        var itemGrid = new Object();

        itemGrid.CodigoItem = item.CodigoItem.val;
        itemGrid.CodigoProduto = item.CodigoProduto.val;
        itemGrid.CodigoServico = item.CodigoServico.val;
        itemGrid.DescricaoItem = item.DescricaoItem.val;
        itemGrid.GrupoProdutoItem = item.GrupoProdutoItem.val;
        itemGrid.Quantidade = item.Quantidade.val;
        itemGrid.ValorUnitario = item.ValorUnitario.val;
        itemGrid.ValorDesconto = item.ValorDesconto.val;
        itemGrid.ValorTotalItem = item.ValorTotalItem.val;
        itemGrid.CodigoTabelaPreco = item.CodigoTabelaPreco.val;

        data.push(itemGrid);
    });

    _gridVendaDiretaItens.CarregarGrid(data);
}

function RemoverItemVendaDiretaClick(data) {
    exibirConfirmacao("Confirmação", "Realmente deseja excluir o item " + data.DescricaoItem + "?", function () {
        $.each(_vendaDireta.Itens.list, function (i, listaItens) {
            if (data.CodigoItem == listaItens.CodigoItem.val) {
                _vendaDireta.Itens.list.splice(i, 1);
                atualizarTotalizarVenda();
                return false;
            }
        });

        RecarregarGridVendaDiretaItens();
    });
}

function EnviarVendaDiretaAnexos(codigoVendaDireta) {
    var formData = new FormData();
    for (var i = 0; i < _vendaDireta.ListaAnexosNovos.list.length; i++) {
        formData.append("Arquivo", _vendaDireta.ListaAnexosNovos.list[i].Arquivo);
        formData.append("Descricao", _vendaDireta.ListaAnexosNovos.list[i].DescricaoAnexo.val);
    }

    _gridVendaDireta.CarregarGrid();
    limparCamposVendaDireta();

    var data = {
        CodigoVendaDireta: codigoVendaDireta
    };
    enviarArquivo("VendaDireta/EnviarAnexos?callback=?", data, formData, function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Anexos salvos com sucesso");
            } else {
                exibirMensagem(tipoMensagem.atencao, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    });
}

function buscarVendaDireta() {
    var editar = { descricao: "Editar", id: "clasEditar", evento: "onclick", metodo: editarVendaDireta, tamanho: "15", icone: "" };
    var menuOpcoes = new Object();
    menuOpcoes.tipo = TypeOptionMenu.link;
    menuOpcoes.opcoes = new Array();
    menuOpcoes.opcoes.push(editar);

    _gridVendaDireta = new GridView(_pesquisaVendaDireta.Pesquisar.idGrid, "VendaDireta/Pesquisa", _pesquisaVendaDireta, menuOpcoes, null);
    _gridVendaDireta.CarregarGrid();
}

function editarVendaDireta(vendaDiretaGrid) {
    limparCamposVendaDireta();
    _vendaDireta.Codigo.val(vendaDiretaGrid.Codigo);
    BuscarPorCodigo(_vendaDireta, "VendaDireta/BuscarPorCodigo", function (arg) {
        _pesquisaVendaDireta.ExibirFiltros.visibleFade(false);
        _crudVendaDireta.Contestar.visible(true);
        _crudVendaDireta.Atualizar.visible(true);
        _crudVendaDireta.Limpar.visible(true);
        _crudVendaDireta.Adicionar.visible(false);
        _crudVendaDireta.Contestar.visible(true);

        if (_vendaDireta.Status.val() == EnumStatusVendaDireta.Finalizado)
            _crudVendaDireta.Cancelar.visible(true);

        if (_vendaDireta.Status.val() == EnumStatusVendaDireta.Cancelado)
            _crudVendaDireta.Atualizar.visible(false);

        RecarregarGridVendaDiretaItens();
        RecarregarGridVendaDiretaParcela();
        RecarregarGridVendaDiretaAnexo();
        RecarregarGridVendaDiretaContestacaoAnexo();

        _vendaDiretaParcela.TipoCobranca.val(_vendaDireta.TipoCobranca.val());
        tipoCobrancaVendaDiretaParcelaChange();
        _vendaDiretaParcela.BoletoConfiguracao.codEntity(_vendaDireta.BoletoConfiguracao.codEntity());
        _vendaDiretaParcela.BoletoConfiguracao.val(_vendaDireta.BoletoConfiguracao.val());

        if (_vendaDireta.Status.val() === EnumStatusVendaDireta.Cancelado || _vendaDireta.Status.val() === EnumStatusVendaDireta.Finalizado) {
            SetarEnableCamposKnockout(_vendaDireta, false);
            SetarEnableCamposKnockout(_vendaDiretaParcela, false);

            _vendaDireta.FuncionarioValidador.enable(true);

            _gridVendaDiretaItens.DesabilitarOpcoes();
            _gridVendaDiretaParcela.DesabilitarOpcoes();
        }

        SetarEtapasVendaDireta();
        AtualizarBoletosClick();
        tipoItemVendaDiretaChange(true);
    }, null);
}

function limparCamposVendaDiretaItens() {
    LimparCampoEntity(_vendaDireta.Produto);
    LimparCampoEntity(_vendaDireta.Servico);
    _vendaDireta.Quantidade.val("");
    _vendaDireta.ValorUnitario.val("");
    _vendaDireta.ValorDesconto.val("0,00");
    _vendaDireta.ValorTotalItem.val("0,00");
    //_vendaDireta.Tipo.val(EnumProdutoServico.Produto);
    _vendaDireta.CodigoItem.val(0);
    _vendaDireta.CodigoProduto.val(0);
    _vendaDireta.CodigoServico.val(0);
    _vendaDireta.DescricaoItem.val("");
    _vendaDireta.GrupoProdutoItem.val("");
    _vendaDireta.ValorMinimoVenda.val("0,00");
    _vendaDireta.CodigoTabelaPreco.val(0);

    tipoItemVendaDiretaChange(true);
    RecarregarGridVendaDiretaItens();
}

function limparCamposVendaDireta() {
    _crudVendaDireta.Contestar.visible(false);
    _crudVendaDireta.Atualizar.visible(false);
    _crudVendaDireta.Cancelar.visible(false);
    _crudVendaDireta.Limpar.visible(false);
    _crudVendaDireta.Adicionar.visible(true);
    LimparCampos(_vendaDireta);

    SetarEnableCamposKnockout(_vendaDireta, true);
    SetarEnableCamposKnockout(_vendaDiretaParcela, true);
    _vendaDireta.Numero.enable(false);
    _vendaDireta.Data.enable(false);
    _vendaDireta.Funcionario.enable(false);
    _vendaDireta.ValorUnitario.enable(false);
    _vendaDireta.ValorTotalItem.enable(false);
    _vendaDireta.ValorTotal.enable(false);
    _vendaDireta.Cliente.enable(true);

    _vendaDireta.Data.val(Global.DataAtual());
    PreencheVendedorLogado();
    tipoItemVendaDiretaChange(true);
    RecarregarGridVendaDiretaItens();
    LimparCampos(_vendaDiretaParcela);
    LimparCamposVendaDiretaParcela();
    LimparCamposVendaDiretaAnexo();
    RecarregarGridVendaDiretaAnexo();
    RecarregarGridVendaDiretaContestacaoAnexo();

    _gridVendaDiretaItens.HabilitarOpcoes();
    _gridVendaDiretaParcela.HabilitarOpcoes();

    SetarEtapaInicioVendaDireta();
    Global.ResetarAbas();
}

function RecarregarGridVendaDiretaContestacaoAnexo() {
    var data = new Array();

    $.each(_vendaDireta.ListaAnexosContestacao.list, function (i, listaAnexos) {
        var listaAnexosGrid = new Object();

        listaAnexosGrid.Codigo = listaAnexos.Codigo.val;
        listaAnexosGrid.DescricaoAnexo = listaAnexos.DescricaoAnexo.val;
        listaAnexosGrid.Arquivo = listaAnexos.Arquivo.val;

        data.push(listaAnexosGrid);
    });

    _gridVendaDiretaContestacaoAnexo.CarregarGrid(data);
}

function BaixarVendaDiretaContestacaoAnexoClick(data) {
    if (VerificaNovosVendaDiretaContestacaoAnexosLancados(data.Codigo))
        executarDownload("VendaDireta/DownloadAnexoContestacao", { CodigoAnexo: data.Codigo });
    else
        exibirMensagem(tipoMensagem.aviso, "Aviso", "Só é possível baixar os Anexos após gravar a venda");
}

function ExcluirVendaDiretaContestacaoAnexoClick(data) {
    exibirConfirmacao("Confirmação", "Realmente deseja excluir o anexo " + data.DescricaoAnexo + "?", function () {
        $.each(_vendaDireta.ListaAnexosContestacao.list, function (i, listaAnexos) {
            if (data.Codigo == listaAnexos.Codigo.val) {
                _vendaDireta.ListaAnexosContestacao.list.splice(i, 1);

                if (VerificaNovosVendaDiretaContestacaoAnexosLancados(data.Codigo)) {
                    var listaAnexosExcluidos = new VendaDiretaContestacaoAnexoMap();
                    listaAnexosExcluidos.Codigo.val = data.Codigo;
                    _contestacao.ListaAnexosContestacaoExcluidos.list.push(listaAnexosExcluidos);
                }

                return false;
            }
        });

        $.each(_vendaDireta.ListaAnexosContestacaoNovos.list, function (i, listaAnexosNovos) {
            if (data.Codigo == listaAnexosNovos.Codigo.val) {
                _vendaDireta.ListaAnexosContestacaoNovos.list.splice(i, 1);

                return false;
            }
        });

        RecarregarGridVendaDiretaContestacaoAnexo();
    });
}

function AdicionarVendaDiretaContestacaoAnexoClick(e, sender) {
    var valido = ValidarCamposObrigatorios(_contestacao);
    var file = document.getElementById(_contestacao.Arquivo.id);
    if (file.files[0] == null)
        valido = false;

    if (valido) {
        var codigo = guid();
        var listaAnexosGrid = new VendaDiretaContestacaoAnexoMap();

        listaAnexosGrid.Codigo.val = codigo;
        listaAnexosGrid.DescricaoAnexo.val = _contestacao.DescricaoAnexo.val();
        listaAnexosGrid.Arquivo.val = _contestacao.Arquivo.val();
        _vendaDireta.ListaAnexosContestacao.list.push(listaAnexosGrid);

        var listaAnexos = new VendaDiretaContestacaoAnexoMap();

        listaAnexos.Codigo.val = codigo;
        listaAnexos.DescricaoAnexo.val = _contestacao.DescricaoAnexo.val();
        listaAnexos.Arquivo = file.files[0];
        _vendaDireta.ListaAnexosContestacaoNovos.list.push(listaAnexos);

        RecarregarGridVendaDiretaContestacaoAnexo();

        _contestacao.Arquivo.val("");
        _contestacao.DescricaoAnexo.val("");

        $("#" + _contestacao.DescricaoAnexo.id).focus();
    } else {
        exibirMensagem(tipoMensagem.atencao, "Campos Obrigatórios", "Informe os campos obrigatórios!");
    }
}

function VerificaNovosVendaDiretaContestacaoAnexosLancados(codigoAnexo) {
    var valido = true;
    $.each(_vendaDireta.ListaAnexosContestacaoNovos.list, function (i, listaAnexos) {
        if (codigoAnexo == listaAnexos.Codigo.val) {
            valido = false;
        }
    });

    return valido;
}

function LimparCamposVendaDiretaContestacaoAnexo() {
    //LimparCampos(_contestacao);
    _contestacao.Arquivo.val("");
    _contestacao.DescricaoAnexo.val("");
    LimparCampoEntity(_contestacao.FuncionarioContestacao);
    _contestacao.DataContestacao.val("");
    _contestacao.ObservacaoContestacao.val("");

    LimparCampo(_vendaDireta.ListaAnexosContestacaoNovos);
    LimparCampo(_contestacao.ListaAnexosContestacaoExcluidos);
}

function EnviarVendaDiretaContestacaoAnexos(codigoVendaDireta) {
    var formData = new FormData();
    for (var i = 0; i < _vendaDireta.ListaAnexosContestacaoNovos.list.length; i++) {
        formData.append("Arquivo", _vendaDireta.ListaAnexosContestacaoNovos.list[i].Arquivo);
        formData.append("Descricao", _vendaDireta.ListaAnexosContestacaoNovos.list[i].DescricaoAnexo.val);
    }

    var data = {
        CodigoVendaDireta: codigoVendaDireta
    };
    enviarArquivo("VendaDireta/EnviarAnexosContestacao?callback=?", data, formData, function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Contestação salva com sucesso");
                LimparCamposVendaDiretaContestacaoAnexo();
            } else {
                exibirMensagem(tipoMensagem.atencao, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    });
}

function SalvarContestacaoClick(e, sender) {
    Salvar(_contestacao, "VendaDireta/SalvarContestacao", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                if (_vendaDireta.ListaAnexosContestacaoNovos.list.length > 0)
                    EnviarVendaDiretaContestacaoAnexos(_vendaDireta.Codigo.val());
                else {
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Contestação salva com sucesso");
                    LimparCamposVendaDiretaContestacaoAnexo();
                }
                Global.fecharModal('divContestacao');
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    }, sender);
}