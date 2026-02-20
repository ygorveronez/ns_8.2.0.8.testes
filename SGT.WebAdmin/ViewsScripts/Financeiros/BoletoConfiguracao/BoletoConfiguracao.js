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
/// <reference path="../../Consultas/Cliente.js" />
/// <reference path="../../Consultas/Empresa.js" />
/// <reference path="../../Consultas/PlanoConta.js" />
/// <reference path="../../Consultas/TipoMovimento.js" />


//*******MAPEAMENTO KNOUCKOUT*******

var _gridBoletoConfiguracao;
var _boletoConfiguracao;
var _pesquisaBoletoConfiguracao;

var _tipoCarteira = [
    { text: "Simples", value: 0 },
    { text: "Registrada", value: 1 }
];

var _aceite = [
    { text: "SIM", value: 1 },
    { text: "NÃO", value: 0 }
];

var _tipoCNABBoleto = [
    { text: "CNAB 240", value: 1 },
    { text: "CNAB 400", value: 2 },
    { text: "CNAB 240 PIX", value: 3 },
    { text: "CNAB 500 PIX", value: 4 },
    { text: "CNAB 750 PIX", value: 5 }
];

var _tipoLayoutoleto = [
    { text: "Boleto C/ Entrega", value: 1 },
    { text: "Boleto Carne", value: 2 },
    { text: "Boleto", value: 3 },
    { text: "Boleto C/ Beneficiário", value: 7 },
    { text: "Boleto Multimodal", value: 6 },
    { text: "Boleto C/ Tabela de CT-es", value: 8 }
];

var _responsavelEmissao = [
    { text: "Emitente", value: 0 },
    { text: "Banco Emite", value: 1 },
    { text: "Banro Reemite", value: 2 },
    { text: "Banco Não Reemite", value: 3 }
];

var _caracteristicaTitulo = [
    { text: "Simples", value: 0 },
    { text: "Vinculada", value: 1 },
    { text: "Caucionada", value: 2 },
    { text: "Descontada", value: 3 },
    { text: "Vendor", value: 4 }
];

var _tipoBanco = [
    { text: "Nenhum", value: 0 },
    { text: "Banco do Brasil", value: 1 },
    { text: "Santander", value: 2 },
    { text: "Caixa Economica", value: 3 },
    { text: "Caixa Sicob", value: 4 },
    { text: "Bradesco", value: 5 },
    { text: "Itau", value: 6 },
    { text: "Banco Mercantil", value: 7 },
    { text: "Sicred", value: 8 },
    { text: "Bancoob", value: 9 },
    { text: "Banrisul", value: 10 },
    { text: "Banestes", value: 11 },
    { text: "HSBC", value: 12 },
    { text: "Banco do Nordeste", value: 13 },
    { text: "BRB", value: 14 },
    { text: "Bic Banco", value: 15 },
    { text: "Bradesco SICOOB", value: 16 },
    { text: "Safra", value: 17 },
    { text: "Safra Bradesco", value: 18 },
    { text: "CECRED", value: 19 },
    { text: "Banco Da Amazonia", value: 20 },
    { text: "Banco Do Brasil / SICOOB", value: 21 },
    { text: "Uniprime", value: 22 },
    { text: "Unicred RS", value: 23 },
    { text: "Banese", value: 24 },
    { text: "Credi SIS", value: 25 },
    { text: "Unicred ES", value: 26 },
    { text: "Banco Cresol SC/RS", value: 27 },
    { text: "Citi Bank", value: 28 },
    { text: "Banco ABC Brasil", value: 29 },
    { text: "Daycoval", value: 30 },
    { text: "Uniprime Norte PR", value: 31 },
    { text: "Banco Pine", value: 32 },
    { text: "Banco Pine Bradesco", value: 33 },
    { text: "Unicred SC", value: 34 },
    { text: "Banco Alfa", value: 35 },
    { text: "Banco Do Brasil API", value: 36 },
    { text: "Banco Do Brasil WS", value: 37 },
    { text: "Banco Cresol", value: 38 },
    { text: "Money Plus", value: 39 },
    { text: "Banco C6", value: 40 },
    { text: "Banco Rendimento", value: 41 },
    { text: "Banco Inter", value: 42 },
    { text: "Banco Sofisa Santander", value: 43 },
    { text: "BS2", value: 44 },
    { text: "J.P. Morgan", value: 61 },
];

var PesquisaBoletoConfiguracao = function () {
    this.NumeroAgencia = PropertyEntity({ text: "Número Agencia: " });
    this.NumeroConta = PropertyEntity({ text: "Número Conta: " });
    this.Banco = PropertyEntity({ val: ko.observable(0), options: _tipoBanco, def: 0, text: "Banco: " });
    this.Situacao = PropertyEntity({ text: "Situação:", options: _statusFemPesquisa, val: ko.observable(true), def: true });

    this.Pesquisar = PropertyEntity({
        eventClick: function (e) {
            _gridBoletoConfiguracao.CarregarGrid();
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

var BoletoConfiguracao = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });

    this.Beneficiario = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Beneficiário:", idBtnSearch: guid(), required: false, enable: ko.observable(true) });
    this.Portador = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Portador:", idBtnSearch: guid(), required: false, enable: ko.observable(true) });
    this.Sacador = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Sacador/Avalista:", idBtnSearch: guid(), required: false, enable: ko.observable(true) });
    this.PlanoConta = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Plano de Contas para Pagamento e Recebimento:", idBtnSearch: guid(), required: false, enable: ko.observable(true) });
    this.TipoMovimentoLiquidacao = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Tipo Movimento para Liquidação (a receber):", idBtnSearch: guid(), required: false, enable: ko.observable(true) });
    this.TipoMovimentoBaixa = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Tipo Movimento para Baixa:", idBtnSearch: guid(), required: false, enable: ko.observable(true) });
    this.TipoMovimentoTarifa = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Tipo Movimento para Tarifa:", idBtnSearch: guid(), required: false, enable: ko.observable(true) });
    this.TipoMovimentoJuros = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Tipo Movimento para Juros:", idBtnSearch: guid(), required: false, enable: ko.observable(true) });
    this.TipoMovimentoDesconto = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Tipo Movimento para Desconto:", idBtnSearch: guid(), required: false, enable: ko.observable(true) });
    this.Empresa = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "*Empresa:", idBtnSearch: guid(), required: ko.observable(false), enable: ko.observable(true), visible: ko.observable(false) });
    this.LiquidarComValorIntegral = PropertyEntity({ val: ko.observable(false), getType: typesKnockout.bool, text: "Liquidar título com o valor integral?", def: false, visible: ko.observable(true) });
    this.UtilizaConfiguracaoPagamentoEletronico = PropertyEntity({ val: ko.observable(false), getType: typesKnockout.bool, text: "Utiliza esta configuração para o Pagamento Eletrônico?", def: false, visible: ko.observable(true) });

    this.Aceite = PropertyEntity({ val: ko.observable(1), options: _aceite, def: 1, text: "*Aceite no Boleto? ", required: true });

    this.Banco = PropertyEntity({ val: ko.observable(0), options: _tipoBanco, def: 0, text: "*Banco: ", required: true });
    this.NumeroBanco = PropertyEntity({ text: "*Número banco: ", required: true, maxlength: 3 });
    this.DigitoBanco = PropertyEntity({ text: "*Dígito banco: ", required: true, maxlength: 1 });
    this.NomeBanco = PropertyEntity({ text: "*Nome do banco: ", required: true, maxlength: 120 });
    this.TagBanco = PropertyEntity({ text: "Tag banco: ", required: false, getType: typesKnockout.int, enable: ko.observable(true), configInt: { precision: 0, allowZero: true } });
    this.TamanhoMaximoNossoNumero = PropertyEntity({ text: "Tam. máximo número: ", required: false, getType: typesKnockout.int, enable: ko.observable(true) });
    this.ProximoNossoNumero = PropertyEntity({ text: "*Próximo número: ", required: true, getType: typesKnockout.int, enable: ko.observable(true) });
    this.NumeroInicialSequenciaRemessa = PropertyEntity({ text: "Nº Última Remessa: ", required: false, getType: typesKnockout.int, enable: ko.observable(true) });

    this.NumeroAgencia = PropertyEntity({ text: "*Número agência: ", required: true, maxlength: 20 });
    this.DigitoAgencia = PropertyEntity({ text: "*Dígito agência: ", required: false, maxlength: 20 });
    this.CodigoCedente = PropertyEntity({ text: "Código cedente: ", required: false, maxlength: 20 });
    this.CaracteristicaTitulo = PropertyEntity({ val: ko.observable(0), options: _caracteristicaTitulo, def: 0, text: "*Característica: ", required: true });
    this.CodigoTransmissao = PropertyEntity({ text: "Código transmissão: ", required: false, maxlength: 20 });

    this.NumeroConta = PropertyEntity({ text: "*Número conta: ", required: true, maxlength: 20 });
    this.DigitoConta = PropertyEntity({ text: "*Dígito conta: ", required: true, maxlength: 20 });
    this.NumeroConvenio = PropertyEntity({ text: "Número convenio: ", required: false, maxlength: 20 });
    this.Modalidade = PropertyEntity({ text: "Modalidade: ", required: false, maxlength: 20 });
    this.DigitoAgenciaConta = PropertyEntity({ text: "Dígito agência/conta: ", required: false, maxlength: 1 });
    this.ResponsavelEmissao = PropertyEntity({ val: ko.observable(0), options: _responsavelEmissao, def: 0, text: "*Responsável emissão: ", required: true });

    this.TagTitulo = PropertyEntity({ text: "Tag título: ", required: false, getType: typesKnockout.int, enable: ko.observable(true), configInt: { precision: 0, allowZero: true } });
    this.TipoCarteira = PropertyEntity({ val: ko.observable(0), options: _tipoCarteira, def: 0, text: "*Tipo carteira: ", required: true });
    this.TipoCNAB = PropertyEntity({ val: ko.observable(2), options: _tipoCNABBoleto, def: 2, text: "*CNAB: ", required: true });
    this.TipoLayout = PropertyEntity({ val: ko.observable(1), options: _tipoLayoutoleto, def: 1, text: "*Layout Boleto: ", required: true });

    this.EspecieTitulo = PropertyEntity({ text: "Espécie título: ", required: false, maxlength: 20 });
    this.CarteiraTitulo = PropertyEntity({ text: "*Carteira título: ", required: true, maxlength: 3 });
    this.ValorJurosAoMes = PropertyEntity({ text: "Valor juros ao mês: ", required: false, getType: typesKnockout.decimal, enable: ko.observable(true) });
    this.DiasProtesto = PropertyEntity({ text: "*Dias protesto: ", required: true, maxlength: 2 });
    this.PercentualMulta = PropertyEntity({ text: "Percentual multa: ", required: false, getType: typesKnockout.decimal, enable: ko.observable(true) });
    this.CodigoBanco = PropertyEntity({ text: "*Código banco: ", required: true, maxlength: 3 });
    this.CodigoProtesto = PropertyEntity({ text: "*Código protesto: ", required: true, maxlength: 2 });
    this.CodigoMulta = PropertyEntity({ text: "*Código multa: ", required: true, maxlength: 2 });

    this.CaminhoRemessa = PropertyEntity({ text: "Caminho da remessa: ", required: false, maxlength: 5000, visible: false });
    this.AcrescimoDiaDataMoraJuros = PropertyEntity({ text: "+ Dias Data Mora/Juros:", required: false, getType: typesKnockout.int, enable: ko.observable(true) });
    this.AcrescimoDiaDataMulta = PropertyEntity({ text: "+ Dias Data Multa: ", required: false, getType: typesKnockout.int, enable: ko.observable(true) });
    this.QuantidadeDiasRecebimentoAposVencimento = PropertyEntity({ text: "Qtd dias receb. após vencimento: ", required: false, getType: typesKnockout.int, enable: ko.observable(true) });
    this.CodigoInstrucaoMovimento = PropertyEntity({ text: "Cod. Instrução Movimento: ", required: false, maxlength: 2 });

    this.LocalPagamento = PropertyEntity({ text: "*Local do pagamento: ", required: true, maxlength: 5000 });
    this.CodigoFinalidadeTED = PropertyEntity({ text: "Cód. finalidade TED (Pg. Digital): ", required: false, maxlength: 50 });

    this.Instrucao = PropertyEntity({ text: "Instruções: ", required: false, maxlength: 5000, visible: ko.observable(true) });

    this.AssuntoEmail = PropertyEntity({ text: "Assunto E-mail: ", required: false, maxlength: 500, visible: ko.observable(true) });
    this.TagEmpresaAssuntoEmail = PropertyEntity({ eventClick: function (e) { InserirTag(_boletoConfiguracao.AssuntoEmail.id, "#Empresa"); }, type: types.event, text: "Empresa" });
    this.TagClienteAssuntoEmail = PropertyEntity({ eventClick: function (e) { InserirTag(_boletoConfiguracao.AssuntoEmail.id, "#Cliente"); }, type: types.event, text: "Cliente" });
    this.TagValorTituloAssuntoEmail = PropertyEntity({ eventClick: function (e) { InserirTag(_boletoConfiguracao.AssuntoEmail.id, "#ValorTitulo"); }, type: types.event, text: "Valor" });
    this.TagDataVencimentoAssuntoEmail = PropertyEntity({ eventClick: function (e) { InserirTag(_boletoConfiguracao.AssuntoEmail.id, "#DataVencimento"); }, type: types.event, text: "Data Vencimento" });

    this.MensagemEmail = PropertyEntity({ text: "Mensagem E-mail: ", required: false, maxlength: 5000, visible: ko.observable(true) });
    this.TagEmpresaMensagemEmail = PropertyEntity({ eventClick: function (e) { InserirTag(_boletoConfiguracao.MensagemEmail.id, "#Empresa"); }, type: types.event, text: "Empresa" });
    this.TagClienteMensagemEmail = PropertyEntity({ eventClick: function (e) { InserirTag(_boletoConfiguracao.MensagemEmail.id, "#Cliente"); }, type: types.event, text: "Cliente" });
    this.TagValorTituloMensagemEmail = PropertyEntity({ eventClick: function (e) { InserirTag(_boletoConfiguracao.MensagemEmail.id, "#ValorTitulo"); }, type: types.event, text: "Valor" });
    this.TagDataVencimentoMensagemEmail = PropertyEntity({ eventClick: function (e) { InserirTag(_boletoConfiguracao.MensagemEmail.id, "#DataVencimento"); }, type: types.event, text: "Data Vencimento" });
    this.TagQuebraLinhaMensagemEmail = PropertyEntity({ eventClick: function (e) { InserirTag(_boletoConfiguracao.MensagemEmail.id, "#qLinha#"); }, type: types.event, text: "Quebra de Linha" });
    this.TagNumeroCTeEmail = PropertyEntity({ eventClick: function (e) { InserirTag(_boletoConfiguracao.MensagemEmail.id, "#NumeroCTe"); }, type: types.event, text: "Número CT-e" });
    this.TagNumeroFaturaEmail = PropertyEntity({ eventClick: function (e) { InserirTag(_boletoConfiguracao.MensagemEmail.id, "#NumeroFatura"); }, type: types.event, text: "Nº Fatura" });
    this.Situacao = PropertyEntity({ text: "Situação da configuração:", options: _statusFem, val: ko.observable(true), def: true });

    this.Adicionar = PropertyEntity({ eventClick: adicionarClick, type: types.event, text: "Adicionar", visible: ko.observable(true) });
    this.Atualizar = PropertyEntity({ eventClick: atualizarClick, type: types.event, text: "Atualizar", visible: ko.observable(false) });
    this.Excluir = PropertyEntity({ eventClick: excluirClick, type: types.event, text: "Excluir", visible: ko.observable(false) });
    this.Cancelar = PropertyEntity({ eventClick: cancelarClick, type: types.event, text: "Cancelar", visible: ko.observable(false) });
}

//*******EVENTOS*******


function loadBoletoConfiguracao() {

    _boletoConfiguracao = new BoletoConfiguracao();
    KoBindings(_boletoConfiguracao, "knockoutCadastroBoletoConfiguracao");

    HeaderAuditoria("BoletoConfiguracao", _boletoConfiguracao);

    _pesquisaBoletoConfiguracao = new PesquisaBoletoConfiguracao();
    KoBindings(_pesquisaBoletoConfiguracao, "knockoutPesquisaBoletoConfiguracao", false, _pesquisaBoletoConfiguracao.Pesquisar.id);

    new BuscarClientes(_boletoConfiguracao.Beneficiario);
    new BuscarClientes(_boletoConfiguracao.Portador);
    new BuscarClientes(_boletoConfiguracao.Sacador);
    new BuscarPlanoConta(_boletoConfiguracao.PlanoConta);
    new BuscarTipoMovimento(_boletoConfiguracao.TipoMovimentoLiquidacao);
    new BuscarTipoMovimento(_boletoConfiguracao.TipoMovimentoBaixa);
    new BuscarTipoMovimento(_boletoConfiguracao.TipoMovimentoTarifa);
    new BuscarTipoMovimento(_boletoConfiguracao.TipoMovimentoJuros);
    new BuscarTipoMovimento(_boletoConfiguracao.TipoMovimentoDesconto);
    new BuscarEmpresa(_boletoConfiguracao.Empresa);

    if (_CONFIGURACAO_TMS.TipoServicoMultisoftware === EnumTipoServicoMultisoftware.MultiTMS) {
        _boletoConfiguracao.Empresa.visible(true);
        _boletoConfiguracao.Empresa.required(true);
    }

    buscarBoletoConfiguracaos();
}

function adicionarClick(e, sender) {
    Salvar(e, "BoletoConfiguracao/Adicionar", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Cadastrado com sucesso");
                _gridBoletoConfiguracao.CarregarGrid();
                limparCamposBoletoConfiguracao();
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            LimparCampoEntity(_boletoConfiguracao.Plano);
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    }, sender);
}

function atualizarClick(e, sender) {
    Salvar(e, "BoletoConfiguracao/Atualizar", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Atualizado com sucesso");
                _gridBoletoConfiguracao.CarregarGrid();
                limparCamposBoletoConfiguracao();
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    }, sender);
}


function excluirClick(e, sender) {
    exibirConfirmacao("Confirmação", "Realmente deseja excluir a configuração selecionada?", function () {
        ExcluirPorCodigo(_boletoConfiguracao, "BoletoConfiguracao/ExcluirPorCodigo", function (arg) {
            if (arg.Success) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Excluído com sucesso");
                _gridBoletoConfiguracao.CarregarGrid();
                limparCamposBoletoConfiguracao();
            } else {
                exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
            }
        }, null);
    });
}

function cancelarClick(e) {
    limparCamposBoletoConfiguracao();
}

function ConsultaPlanoClick(e, sender) {

}

//*******MÉTODOS*******


function buscarBoletoConfiguracaos() {
    var editar = { descricao: "Editar", id: "clasEditar", evento: "onclick", metodo: editarBoletoConfiguracao, tamanho: "15", icone: "" };
    var menuOpcoes = new Object();
    menuOpcoes.tipo = TypeOptionMenu.link;
    menuOpcoes.opcoes = new Array();
    menuOpcoes.opcoes.push(editar);

    _gridBoletoConfiguracao = new GridView(_pesquisaBoletoConfiguracao.Pesquisar.idGrid, "BoletoConfiguracao/Pesquisa", _pesquisaBoletoConfiguracao, menuOpcoes, { column: 2, dir: orderDir.asc });
    _gridBoletoConfiguracao.CarregarGrid();
}

function editarBoletoConfiguracao(boletoConfiguracaoGrid) {
    limparCamposBoletoConfiguracao();
    _boletoConfiguracao.Codigo.val(boletoConfiguracaoGrid.Codigo);
    BuscarPorCodigo(_boletoConfiguracao, "BoletoConfiguracao/BuscarPorCodigo", function (arg) {
        _pesquisaBoletoConfiguracao.ExibirFiltros.visibleFade(false);
        _boletoConfiguracao.Atualizar.visible(true);
        _boletoConfiguracao.Cancelar.visible(true);
        _boletoConfiguracao.Excluir.visible(true);
        _boletoConfiguracao.Adicionar.visible(false);
    }, null);
}

function limparCamposBoletoConfiguracao() {
    _boletoConfiguracao.Atualizar.visible(false);
    _boletoConfiguracao.Cancelar.visible(false);
    _boletoConfiguracao.Excluir.visible(false);
    _boletoConfiguracao.Adicionar.visible(true);
    LimparCampos(_boletoConfiguracao);
}
