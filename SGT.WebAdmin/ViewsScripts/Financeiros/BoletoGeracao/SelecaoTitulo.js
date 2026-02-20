/// <reference path="BoletoGeracaoEtapa.js" />
/// <reference path="../../../js/libs/jquery-2.1.1.js" />
/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/bootstrap/bootstrap.js" />
/// <reference path="../../../js/libs/jquery.blockui.js" />
/// <reference path="../../../js/Global/knoutViewsSlides.js" />
/// <reference path="../../../js/libs/jquery.maskMoney.js" />
/// <reference path="../../Consultas/Cliente.js" />
/// <reference path="../../../js/plugin/datatables/jquery.dataTables.js" />
/// <reference path="../../../js/libs/jquery.twbsPagination.js" />
/// <reference path="../../../js/libs/jquery.globalize.js" />
/// <reference path="../../../js/libs/jquery.globalize.pt-BR.js" />
/// <reference path="BoletoGeracao.js" />
/// <reference path="../../Consultas/Remessa.js" />
/// <reference path="../../Consultas/Fatura.js" />
/// <reference path="../../Consultas/Conhecimento.js" />
/// <reference path="../../Enumeradores/EnumFormaTitulo.js" />
/// <reference path="../../Enumeradores/EnumTipoPropostaMultimodal.js" />
/// <reference path="../../Consultas/TipoTerminalImportacao.js" />
/// <reference path="../../Consultas/PedidoViagemNavio.js" />
/// <reference path="../../Consultas/Usuario.js" />
/// <reference path="../../Consultas/Empresa.js" />


//*******MAPEAMENTO KNOUCKOUT*******

var _selecaoTitulo;
var _gridSelecaoTitulo;


var SelecaoTitulo = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });

    this.FormaTitulo = PropertyEntity({ val: ko.observable(EnumFormaTitulo.Todos), options: EnumFormaTitulo.obterOpcoesPesquisa(), text: "Forma do Título: ", def: EnumFormaTitulo.Todos, enable: ko.observable(true) });
    this.DataVencimentoInicial = PropertyEntity({ text: "Vencimento inicial: ", getType: typesKnockout.date, enable: ko.observable(true), required: false });
    this.DataVencimentoFinal = PropertyEntity({ text: "Vencimento final: ", dateRangeInit: this.DataVencimentoInicial, getType: typesKnockout.date, enable: ko.observable(true), required: false });
    this.DataEmissaoInicial = PropertyEntity({ text: "Emissão inicial: ", getType: typesKnockout.date, enable: ko.observable(true), required: false });
    this.DataEmissaoFinal = PropertyEntity({ text: "Emissão final: ", dateRangeInit: this.DataEmissaoInicial, getType: typesKnockout.date, enable: ko.observable(true), required: false });
    this.Pessoa = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), required: false, text: "Pessoa:", idBtnSearch: guid(), enable: ko.observable(true), visible: ko.observable(true), issue: 52 });
    this.Remessa = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), required: false, text: "Remessa:", idBtnSearch: guid(), enable: ko.observable(true), visible: ko.observable(true) });
    this.Fatura = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), required: false, text: "Fatura:", idBtnSearch: guid(), enable: ko.observable(true), visible: ko.observable(true) });
    this.SomentePendentes = PropertyEntity({ val: ko.observable(true), getType: typesKnockout.bool, text: "Somente títulos pendentes para gerar boleto?", def: true, visible: ko.observable(true) });
    this.SomenteSemRemessa = PropertyEntity({ val: ko.observable(false), getType: typesKnockout.bool, text: "Somente boletos sem remessa?", def: false, visible: ko.observable(true) });
    this.Conhecimento = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), required: false, text: "CT-e:", idBtnSearch: guid(), enable: ko.observable(true), visible: ko.observable(true) });    
    this.OperadorFatura = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Operador Fatura", idBtnSearch: guid(), visible: ko.observable(true) });
    this.EmpresaFilial = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: "Empresa/Filial", idBtnSearch: guid(), visible: ko.observable(true) });
    this.ConfiguracaoBoleto = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Configuração Boleto", idBtnSearch: guid(), visible: ko.observable(true) });

    //Emissão Multimodal
    this.NumeroBooking = PropertyEntity({ text: "Número Booking:" });
    this.NumeroOS = PropertyEntity({ text: "Número OS:" });
    this.NumeroCarga = PropertyEntity({ text: "Número Carga:" });
    this.NumeroNota = PropertyEntity({ text: "Número Nota:", getType: typesKnockout.int });
    this.NumeroControleCliente = PropertyEntity({ text: "Número Controle Cliente:" });
    this.NumeroControle = PropertyEntity({ text: "Número Controle:" });
    this.TipoProposta = PropertyEntity({ val: ko.observable(EnumTipoPropostaMultimodal.Todos), def: EnumTipoPropostaMultimodal.Todos, text: "Tipo Proposta:", options: EnumTipoPropostaMultimodal.obterOpcoesPesquisa() });
    this.TerminalOrigem = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Terminal Origem:", idBtnSearch: guid() });
    this.TerminalDestino = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Terminal Destino:", idBtnSearch: guid() });
    this.Viagem = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Viagem:", idBtnSearch: guid() });

    this.TitulosParaGerarBoleto = PropertyEntity({ idGrid: guid(), enable: ko.observable(true) });
    this.SelecionarTodos = PropertyEntity({ val: ko.observable(false), def: false, type: types.map, getType: typesKnockout.bool, text: "Marcar/Desmarcar Todos", visible: ko.observable(false), enable: ko.observable(true) });
    this.TotalizadorValorSelecionado = PropertyEntity({ text: "Valor total selecionado: ", getType: typesKnockout.string, def: "0,00", val: ko.observable("0,00"), enable: ko.observable(true), visible: ko.observable(false) });
    this.TotalizadorValorFiltrado = PropertyEntity({ text: "Valor total filtrado: ", getType: typesKnockout.string, def: "0,00", val: ko.observable("0,00"), enable: ko.observable(true), visible: ko.observable(false) });

    this.PesquisaTitulos = PropertyEntity({ eventClick: PesquisaTitulosClick, type: types.event, text: "Pesquisar títulos", visible: ko.observable(true), enable: ko.observable(true) });

    this.Proximo = PropertyEntity({ eventClick: ProximoSelecaoTituloClick, type: types.event, text: "Próximo", visible: ko.observable(true), enable: ko.observable(true) });
};

//*******EVENTOS*******

function loadSelecaoTitulo() {
    _selecaoTitulo = new SelecaoTitulo();
    KoBindings(_selecaoTitulo, "knockoutSelecaoTitulos");

    new BuscarClientes(_selecaoTitulo.Pessoa);
    new BuscarRemessa(_selecaoTitulo.Remessa);
    new BuscarConhecimentoNotaReferencia(_selecaoTitulo.Conhecimento, RetornoBuscarPesquisaConhecimento);

    new BuscarTipoTerminalImportacao(_selecaoTitulo.TerminalOrigem);
    new BuscarTipoTerminalImportacao(_selecaoTitulo.TerminalDestino);
    new BuscarPedidoViagemNavio(_selecaoTitulo.Viagem);
    new BuscarFatura(_selecaoTitulo.Fatura, retornoFatura);
    new BuscarFuncionario(_selecaoTitulo.OperadorFatura);
    new BuscarEmpresa(_selecaoTitulo.EmpresaFilial);
    new BuscarBoletoConfiguracao(_selecaoTitulo.ConfiguracaoBoleto);

    if (_CONFIGURACAO_TMS.TipoServicoMultisoftware === EnumTipoServicoMultisoftware.MultiNFe)
    {
        _selecaoTitulo.Fatura.visible(false);
        _selecaoTitulo.OperadorFatura.visible(false);
        _selecaoTitulo.EmpresaFilial.visible(false);
    }

    if (!_CONFIGURACAO_TMS.UtilizaEmissaoMultimodal)
        $("#liFiltrosEmissaoMultimodal").hide();
}

function PesquisaTitulosClick(e, sender) {
    buscarTitulosParaGerarBoleto();
}

function ProximoSelecaoTituloClick(e, sender) {
    if (_gridSelecaoTitulo == undefined) {
        exibirMensagem(tipoMensagem.aviso, "Selecione os Títulos", "Por favor selecione ao menos um título para avançar à próxima etapa.");
        return;
    }
    if (!_selecaoTitulo.SelecionarTodos.val()) {
        var titulosSelecionados = _gridSelecaoTitulo.ObterMultiplosSelecionados();
        if (titulosSelecionados.length > 0)
            AvancarEtapaBoleto(titulosSelecionados);
    } else {
        BuscarTodosTitulos(true);
    }
}

//*******MÉTODOS*******
function retornoFatura(data) {
    _selecaoTitulo.Fatura.codEntity(data.Codigo);
    _selecaoTitulo.Fatura.val(data.DescricaoPeriodo);
}
function BuscarTodosTitulos(avancarEtapa) {
    executarReST("BoletoGeracao/BuscarTitulosParaGerarBoleto", RetornarObjetoPesquisa(_selecaoTitulo), function (arg) {
        if (arg.Success) {
            if (arg.Data !== false) {
                var titulosSelecionados = arg.Data;
                if (!avancarEtapa)
                    ValorTitulosFiltrados(titulosSelecionados);
                else
                    AvancarEtapaBoleto(titulosSelecionados);
            } else {
                exibirMensagem(tipoMensagem.atencao, "Atenção", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    });
}

function AvancarEtapaBoleto(titulosSelecionados) {
    if (titulosSelecionados.length > 0) {
        var dataGrid = new Array();
        var contemTitulosNaoGeradosBoletos = false;
        var valorTotalTitulos = 0.0;

        $.each(titulosSelecionados, function (i, titulo) {

            var obj = new Object();
            obj.Codigo = titulo.Codigo;
            obj.CodigoRemessa = titulo.CodigoRemessa;
            obj.BoletoStatusTitulo = titulo.BoletoStatusTitulo;
            obj.Pessoa = titulo.Pessoa;
            obj.DescricaoStatusBoleto = titulo.DescricaoStatusBoleto;
            obj.DataEmissao = titulo.DataEmissao;
            obj.DataVencimento = titulo.DataVencimento;
            obj.Valor = titulo.Valor;
            obj.NossoNumero = titulo.NossoNumero;
            obj.NumeroRemessa = titulo.NumeroRemessa;
            obj.CaminhoBoleto = titulo.CaminhoBoleto;

            if (!contemTitulosNaoGeradosBoletos)
                contemTitulosNaoGeradosBoletos = titulo.BoletoStatusTitulo === 0 || titulo.BoletoStatusTitulo === 3 || titulo.BoletoStatusTitulo == "";
            valorTotalTitulos += Globalize.parseFloat(titulo.Valor);

            dataGrid.push(obj);
        });
        _geracaoBoleto.TotalizadorValorSelecionado.val(Globalize.format(valorTotalTitulos, "n2"));
        _geracaoRemessa.TotalizadorValorSelecionado.val(Globalize.format(valorTotalTitulos, "n2"));
        _envioEmail.TotalizadorValorSelecionado.val(Globalize.format(valorTotalTitulos, "n2"));
        _geracaoFrancesinha.TotalizadorValorSelecionado.val(Globalize.format(valorTotalTitulos, "n2"));

        if (!contemTitulosNaoGeradosBoletos) {
            _geracaoBoleto.ConfiguracaoBoleto.visible(false);
            _geracaoBoleto.GerarBoletosEtapa2.visible(false);
        } else {
            _geracaoBoleto.ConfiguracaoBoleto.visible(true);
            _geracaoBoleto.GerarBoletosEtapa2.visible(true);
        }

        _gridBoletosEtapa2.CarregarGrid(dataGrid);
        _etapaAtual = 2;
        $("#" + _etapaBoletoGeracao.Etapa2.idTab).click();
        _geracaoBoleto.AtualizarBoletos.visible(true);
        $("#knockoutGeracaoBoletos").show();
        $("#" + _etapaBoletoGeracao.Etapa1.idTab + " .step").attr("class", "step green");

        $("#" + _etapaBoletoGeracao.Etapa2.idTab).attr("data-bs-toggle", "tab");
        $("#" + _etapaBoletoGeracao.Etapa2.idTab + " .step").attr("class", "step lightgreen");
        $("#" + _etapaBoletoGeracao.Etapa2.idTab).tab("show");
    } else {
        exibirMensagem(tipoMensagem.aviso, "Selecione os Títulos", "Por favor selecione ao menos um título para avançar à próxima etapa.");
        $("#knockoutGeracaoBoletos").hide();
        $("#knockoutGeracaoRemessa").hide();
        $("#knockoutEnvioEmail").hide();
    }
}

function RetornoBuscarPesquisaConhecimento(data) {
    _selecaoTitulo.Conhecimento.codEntity(data.Codigo);
    _selecaoTitulo.Conhecimento.val(data.Numero + "-" + data.Serie);
}

function buscarTitulosParaGerarBoleto() {
    var somenteLeitura = false;

    _selecaoTitulo.SelecionarTodos.visible(true);
    _selecaoTitulo.SelecionarTodos.val(false);
    _selecaoTitulo.TotalizadorValorSelecionado.visible(false);
    _selecaoTitulo.TotalizadorValorFiltrado.visible(true);
    _selecaoTitulo.TotalizadorValorSelecionado.val("0,00");

    var multiplaescolha = {
        basicGrid: null,
        //callbackSelecionado: function () {
        //    AtualizarValorTituloSelecionado();
        //},
        //callbackNaoSelecionado: function () {
        //    AtualizarValorTituloSelecionado();
        //},
        eventos: function () { },
        selecionados: new Array(),
        naoSelecionados: new Array(),
        SelecionarTodosKnout: _selecaoTitulo.SelecionarTodos,
        somenteLeitura: somenteLeitura
    };

    _gridSelecaoTitulo = new GridView(_selecaoTitulo.TitulosParaGerarBoleto.idGrid, "BoletoGeracao/PesquisaTitulosParaGerarBoleto", _selecaoTitulo, null, null, null, null, null, null, multiplaescolha);
    _gridSelecaoTitulo.CarregarGrid(function () {
        BuscarTodosTitulos(false);
    });
}

function limparCamposFatura() {
    LimparCampos(_selecaoTitulo);
}

function exibirCamposObrigatorio() {
    exibirMensagem("atencao", "Campo Obrigatório", "Por Favor, Informe os campos obrigatórios");
}

function AtualizarValorTituloSelecionado() {

    var titulosSelecionados = _gridSelecaoTitulo.ObterMultiplosSelecionados();
    var valorTotalSelecionado = 0.0;

    if (titulosSelecionados.length > 0) {
        $.each(titulosSelecionados, function (i, titulo) {
            valorTotalSelecionado += Globalize.parseFloat(titulo.Valor);
        });
        _selecaoTitulo.TotalizadorValorSelecionado.val(Globalize.format(valorTotalSelecionado, "n2"));
    } else
        _selecaoTitulo.TotalizadorValorSelecionado.val("0,00");
}

function ValorTitulosFiltrados(titulosSelecionados) {
    var valorTotalSelecionado = 0.0;

    if (titulosSelecionados.length > 0) {
        $.each(titulosSelecionados, function (i, titulo) {
            valorTotalSelecionado += Globalize.parseFloat(titulo.Valor);
        });

        _selecaoTitulo.TotalizadorValorFiltrado.val(Globalize.format(valorTotalSelecionado, "n2"));
    } else
        _selecaoTitulo.TotalizadorValorFiltrado.val("0,00");
}