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
/// <reference path="../../Consultas/Empresa.js" />
/// <reference path="../../Consultas/Cliente.js" />
/// <reference path="../../Consultas/GrupoPessoa.js" />
/// <reference path="../../Consultas/PagamentoEletronico.js" />
/// <reference path="../../Consultas/BoletoConfiguracao.js" />
/// <reference path="../../Consultas/Empresa.js" />
/// <reference path="../../Consultas/LayoutEDI.js" />
/// <reference path="../../Consultas/Banco.js" />
/// <reference path="../../Enumeradores/EnumTipoDocumentoPesquisaTitulo.js" />
/// <reference path="../../Enumeradores/EnumSituacaoPagamentoEletronico.js" />
/// <reference path="../../Enumeradores/EnumSituacaoBoletoTitulo.js" />
/// <reference path="../../Enumeradores/EnumTipoContaPagamentoEletronico.js" />
/// <reference path="../../Enumeradores/EnumFinalidadePagamentoEletronico.js" />
/// <reference path="../../Enumeradores/EnumDescricaoUsoEmpresaPagamentoEletronico.js" />
/// <reference path="../../Enumeradores/EnumModalidadePagamentoEletronico.js" />
/// <reference path="../../Enumeradores/EnumSituacaoAutorizacaoPagamentoEletronico.js" />
/// <reference path="../../Enumeradores/EnumTipoServicoPagamentoEletronico.js" />
/// <reference path="../../Enumeradores/EnumFormaLancamentoPagamentoEletronico.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _pagamentoDigital, _gridTitulos, _gerarRemessa;

var PagamentoDigital = function () {
    this.Fornecedor = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Fornecedor:", idBtnSearch: guid(), visible: ko.observable(true), required: false, enable: ko.observable(false) });
    this.GrupoPessoa = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Grupo de Pessoa:", idBtnSearch: guid(), visible: ko.observable(true), required: false, enable: ko.observable(false) });
    this.DataVencimentoInicial = PropertyEntity({ text: "Data Vencimento de: ", getType: typesKnockout.date, enable: ko.observable(true) });
    this.DataVencimentoFinal = PropertyEntity({ text: "Até: ", dateRangeInit: this.DataVencimentoInicial, getType: typesKnockout.date, enable: ko.observable(true) });

    this.NumeroTitulo = PropertyEntity({ text: "Número Título: ", getType: typesKnockout.int, enable: ko.observable(true) });
    this.ValorTitulo = PropertyEntity({ text: "Valor Título: ", getType: typesKnockout.decimal, enable: ko.observable(true) });
    this.Banco = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Banco: ", issue: 49, idBtnSearch: guid(), required: false });

    this.DataEmissaoInicial = PropertyEntity({ text: "Data Emissão de: ", getType: typesKnockout.date, enable: ko.observable(true) });
    this.DataEmissaoFinal = PropertyEntity({ text: "Até: ", dateRangeInit: this.DataEmissaoInicial, getType: typesKnockout.date, enable: ko.observable(true) });
    this.DataProgramacaoPagamentoInicial = PropertyEntity({ text: "Data Programação Pagamento de: ", getType: typesKnockout.date, enable: ko.observable(true) });
    this.DataProgramacaoPagamentoFinal = PropertyEntity({ text: "Até: ", dateRangeInit: this.DataEmissaoInicial, getType: typesKnockout.date, enable: ko.observable(true) });
    this.PagamentoEletronico = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Remessa Anterior:", idBtnSearch: guid(), visible: ko.observable(true), required: false, enable: ko.observable(false) });
    this.SituacaoPagamentoEletronico = PropertyEntity({ val: ko.observable(EnumSituacaoPagamentoEletronico.Pendente), options: EnumSituacaoPagamentoEletronico.obterOpcoesPesquisa(), text: "Situação Pag. Eletrônico: ", def: EnumSituacaoPagamentoEletronico.Pendente, required: false, enable: ko.observable(true) });
    this.SituacaoBoletoTitulo = PropertyEntity({ val: ko.observable(EnumSituacaoBoletoTitulo.ComBoleto), options: EnumSituacaoBoletoTitulo.obterOpcoesPesquisa(), text: "Situação do Boleto: ", def: EnumSituacaoBoletoTitulo.ComBoleto, required: false, enable: ko.observable(true) });
    this.TipoDocumento = PropertyEntity({ text: "Tipo de Documento:", getType: typesKnockout.selectMultiple, val: ko.observable(new Array()), def: new Array(), options: EnumTipoDocumentoPesquisaTitulo.obterOpcoes(), visible: ko.observable(true) });
    this.DataAutorizacaoInicial = PropertyEntity({ text: "Data Autorização de:", getType: typesKnockout.date, val: ko.observable(""), def: "" });
    this.DataAutorizacaoFinal = PropertyEntity({ text: "Até:", getType: typesKnockout.date, val: ko.observable(""), def: "" });
    this.DataAutorizacaoInicial.dateRangeLimit = this.DataAutorizacaoFinal;
    this.DataAutorizacaoFinal.dateRangeInit = this.DataAutorizacaoInicial;

    this.PesquisarTitulos = PropertyEntity({ eventClick: PesquisarTitulosClick, type: types.event, text: "Filtrar", visible: ko.observable(true), enable: ko.observable(true) });
    this.Titulos = PropertyEntity({ idGrid: guid(), enable: ko.observable(true) });
    this.ListaTitulos = PropertyEntity({ getType: typesKnockout.dynamic, val: ko.observable(""), def: "" });
    this.SelecionarTodos = PropertyEntity({ val: ko.observable(true), def: true, type: types.map, getType: typesKnockout.bool, text: "Marcar/Desmarcar Todos", visible: ko.observable(true), enable: ko.observable(true) });

    this.Arquivo = PropertyEntity({ eventClick: ArquivoNotasClick, type: types.event, text: "Gerar", visible: ko.observable(true) });
    this.Cancelar = PropertyEntity({ eventClick: cancelarClick, type: types.event, text: "Cancelar", visible: ko.observable(true) });
};

var GerarRemessa = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(""), def: "" });

    this.QuantidadeTitulos = PropertyEntity({ text: "Qtd. Títulos: ", visible: true, required: false, val: ko.observable(""), enable: ko.observable(true) });
    this.ValorTotal = PropertyEntity({ text: "Valor Total: ", visible: true, required: false, val: ko.observable(""), enable: ko.observable(true) });

    this.BoletoConfiguracao = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "*Config. Banco:", idBtnSearch: guid(), required: ko.observable(true), visible: ko.observable(true), enable: ko.observable(true) });
    this.Empresa = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "*Empresa:", idBtnSearch: guid(), required: ko.observable(true), visible: ko.observable(true), enable: ko.observable(true) });
    this.DataPagamento = PropertyEntity({ text: "*Data Pagamento: ", getType: typesKnockout.date, enable: ko.observable(true), required: ko.observable(true), visible: ko.observable(true) });
    this.ModalidadePagamentoEletronico = PropertyEntity({ val: ko.observable(EnumModalidadePagamentoEletronico.CC_CreditoContaCorrenteo), options: EnumModalidadePagamentoEletronico.obterOpcoes(), text: "Modalidade: ", def: EnumModalidadePagamentoEletronico.CC_CreditoContaCorrente, required: true, enable: ko.observable(true) });
    this.TipoContaPagamentoEletronico = PropertyEntity({ val: ko.observable(EnumTipoContaPagamentoEletronico.ContaCorrenteIndividual), options: EnumTipoContaPagamentoEletronico.obterOpcoes(), text: "Tipo Conta: ", def: EnumTipoContaPagamentoEletronico.ContaCorrenteIndividual, required: true, enable: ko.observable(true) });
    this.FinalidadePagamentoEletronico = PropertyEntity({ val: ko.observable(EnumFinalidadePagamentoEletronico.CreditoContaCorrente), options: EnumFinalidadePagamentoEletronico.obterOpcoes(), text: "Finalidade: ", def: EnumFinalidadePagamentoEletronico.CreditoContaCorrente, required: true, enable: ko.observable(true) });
    this.DescricaoUsoEmpresaPagamentoEletronico = PropertyEntity({ val: ko.observable(EnumDescricaoUsoEmpresaPagamentoEletronico.Nenhum), options: EnumDescricaoUsoEmpresaPagamentoEletronico.obterOpcoes(), text: "Desc. Uso da Empresa: ", def: EnumDescricaoUsoEmpresaPagamentoEletronico.Nenhum, required: true, enable: false });
    this.TipoServicoPagamentoEletronico = PropertyEntity({ val: ko.observable(EnumTipoServicoPagamentoEletronico.Padrao), options: EnumTipoServicoPagamentoEletronico.obterOpcoes(), text: "Tipo de Serviço: ", def: EnumTipoServicoPagamentoEletronico.Padrao, required: false, enable: ko.observable(true) });
    this.FormaLancamentoPagamentoEletronico = PropertyEntity({ val: ko.observable(EnumFormaLancamentoPagamentoEletronico.Padrao), options: EnumFormaLancamentoPagamentoEletronico.obterOpcoes(), text: "Forma de Lançamento: ", def: EnumFormaLancamentoPagamentoEletronico.Padrao, required: false, enable: ko.observable(true) });

    this.ListaTitulos = PropertyEntity({ type: types.map, getType: typesKnockout.dynamic, val: ko.observable(""), def: "", idGrid: guid() });
    this.GerarRemessa = PropertyEntity({ type: types.event, eventClick: GerarRemessaClick, text: ko.observable("Gerar Remessa"), visible: ko.observable(true), enable: ko.observable(true) });
};

//*******EVENTOS*******

function loadPagamentoDigital() {
    _pagamentoDigital = new PagamentoDigital();
    KoBindings(_pagamentoDigital, "knockoutGeracaoPagamentoDigital", false, _pagamentoDigital.PesquisarTitulos.id);

    new BuscarClientes(_pagamentoDigital.Fornecedor);
    new BuscarGruposPessoas(_pagamentoDigital.GrupoPessoa);
    new BuscarPagamentoEletronico(_pagamentoDigital.PagamentoEletronico);
    new BuscarBanco(_pagamentoDigital.Banco);

    _gerarRemessa = new GerarRemessa();
    KoBindings(_gerarRemessa, "knoutGerarRemessa");

    new BuscarBoletoConfiguracao(_gerarRemessa.BoletoConfiguracao, RetornoBoletoConfiguracao, true);
    new BuscarEmpresa(_gerarRemessa.Empresa);

    if (_CONFIGURACAO_TMS.TipoServicoMultisoftware === EnumTipoServicoMultisoftware.MultiNFe) {
        _gerarRemessa.Empresa.visible(false);
        _gerarRemessa.Empresa.required(false);
    }

    buscarTitulos();
}

function RetornoBoletoConfiguracao(data) {
    _gerarRemessa.BoletoConfiguracao.codEntity(data.Codigo);
    _gerarRemessa.BoletoConfiguracao.val(data.Descricao);
}

function PesquisarTitulosClick(e, sender) {
    _gridTitulos.CarregarGrid();
}

function ArquivoNotasClick(e, sender) {
    limparCamposGerarRemessa();

    var titulosSelecionadas = null;

    if (_pagamentoDigital.SelecionarTodos.val())
        titulosSelecionadas = _gridTitulos.ObterMultiplosNaoSelecionados();
    else
        titulosSelecionadas = _gridTitulos.ObterMultiplosSelecionados();

    var codigosTitulos = new Array();

    for (var i = 0; i < titulosSelecionadas.length; i++)
        codigosTitulos.push(titulosSelecionadas[i].DT_RowId);

    if (codigosTitulos && (codigosTitulos.length > 0 || _pagamentoDigital.SelecionarTodos.val())) {

        _pagamentoDigital.ListaTitulos.val(JSON.stringify(codigosTitulos));

        executarReST("PagamentoDigital/BuscarValoresTitulos", RetornarObjetoPesquisa(_pagamentoDigital), function (arg) {
            if (arg.Success) {
                var retorno = arg.Data;

                _gerarRemessa.QuantidadeTitulos.val(retorno.QuantidadeTitulos);
                _gerarRemessa.ValorTotal.val(retorno.ValorTotal);
                _gerarRemessa.ListaTitulos.val(JSON.stringify(retorno.CodigosTitulos));

                Global.abrirModal('#divGerarRemessa');
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        });

    } else {
        exibirMensagem(tipoMensagem.aviso, "Aviso", "Nenhum título selecionado para gerar o arquivo.");
    }
}

function GerarRemessaClick(e, sender) {
    Salvar(_gerarRemessa, "PagamentoDigital/Salvar", function (arg) {
        if (arg.Success) {
            var data = {
                Codigo: arg.Data.Codigo,
            };

            if (arg.Data.SituacaoAutorizacaoPagamentoEletronico == null || arg.Data.SituacaoAutorizacaoPagamentoEletronico == EnumSituacaoAutorizacaoPagamentoEletronico.Finalizada) {
                executarDownload("PagamentoDigital/DownloadArquivoRemessa", data);
                setTimeout(function () {
                    executarDownload("PagamentoDigital/DownloadRelatorioRemessa", data);
                }, 5000);
            }
            else
                exibirMensagem(tipoMensagem.atencao, "Aprovação", "Remessa aguardando a aprovação de sua alçada para realizar o download.");

            limparCamposPagamentoDigital();
            Global.fecharModal('#divGerarRemessa');
        } else {
            exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
        }
    }, sender, exibirCamposObrigatorio);
}

function cancelarClick(e) {
    limparCamposPagamentoDigital();
}

//*******MÉTODOS*******

function buscarTitulos() {
    var somenteLeitura = false;

    _pagamentoDigital.SelecionarTodos.visible(true);
    _pagamentoDigital.SelecionarTodos.val(true);

    var multiplaescolha = {
        basicGrid: null,
        eventos: function () { },
        selecionados: new Array(),
        naoSelecionados: new Array(),
        SelecionarTodosKnout: _pagamentoDigital.SelecionarTodos,
        somenteLeitura: somenteLeitura
    };

    _gridTitulos = new GridView(_pagamentoDigital.Titulos.idGrid, "PagamentoDigital/PesquisaTitulos", _pagamentoDigital, null, null, 10, null, null, null, multiplaescolha);
    _gridTitulos.CarregarGrid();
}

function limparCamposPagamentoDigital() {
    LimparCampos(_pagamentoDigital);
    LimparCampos(_gerarRemessa);
    _gridTitulos.CarregarGrid();
}

function limparCamposGerarRemessa() {
    LimparCampos(_gerarRemessa);
}

function exibirCamposObrigatorio() {
    exibirMensagem("atencao", "Campo Obrigatório", "Por Favor, Informe os campos obrigatórios");
}