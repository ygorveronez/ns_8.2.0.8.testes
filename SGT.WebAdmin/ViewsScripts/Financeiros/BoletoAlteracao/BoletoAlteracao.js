/// <reference path="../../../js/libs/jquery-2.1.1.js" />
/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/knockout-3.1.0.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/bootstrap/bootstrap.js" />
/// <reference path="../../../js/libs/jquery.blockui.js" />
/// <reference path="../../../js/Global/knoutViewsSlides.js" />
/// <reference path="../../../js/plugin/chartjs/chart.js" />
/// <reference path="../../../js/libs/jquery.maskMoney.js" />
/// <reference path="../../../js/plugin/datatables/jquery.dataTables.js" />
/// <reference path="../../Enumeradores/EnumBoletoAlteracaoEtapa.js" />
/// <reference path="../../Consultas/Cliente.js" />
/// <reference path="../../Consultas/BoletoConfiguracao.js" />
/// <reference path="EtapaBoletoAlteracao.js" />
/// <reference path="AlteracaoBoletoAlteracao.js" />
/// <reference path="ImpressaoBoletoAlteracao.js" />
/// <reference path="EmailBoletoAlteracao.js" />
/// <reference path="RemessaBoletoAlteracao.js" />

var _etapaBoletoAlteracao = [{ text: "Seleção", value: EnumBoletoAlteracaoEtapa.Selecao },
{ text: "Alteração", value: EnumBoletoAlteracaoEtapa.Alteracao },
{ text: "Impressão", value: EnumBoletoAlteracaoEtapa.Impressao },
{ text: "Remessa", value: EnumBoletoAlteracaoEtapa.Remessa },
{ text: "E-mail", value: EnumBoletoAlteracaoEtapa.Email }];

var _gridBoletoAlteracao;
var _boletoAlteracao;
var _pesquisaBoletoAlteracao;
var _gridBoletos;

var PesquisaBoletoAlteracao = function () {
    this.DataVencimentoInicial = PropertyEntity({ text: "Data Vencimento de: ", getType: typesKnockout.date, enable: ko.observable(true), required: false });
    this.DataVencimentoFinal = PropertyEntity({ text: "Até: ", getType: typesKnockout.date, enable: ko.observable(true), required: false });
    this.DataEmissaoInicial = PropertyEntity({ text: "Data Emissão de: ", getType: typesKnockout.date, enable: ko.observable(true), required: false });
    this.DataEmissaoFinal = PropertyEntity({ text: "Até: ", getType: typesKnockout.date, enable: ko.observable(true), required: false });
    this.BoletoConfiguracao = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Banco:", idBtnSearch: guid(), visible: ko.observable(true) });
    this.Cliente = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Cliente:", idBtnSearch: guid(), visible: ko.observable(true) });

    this.Pesquisar = PropertyEntity({
        eventClick: function (e) {
            _gridBoletoAlteracao.CarregarGrid();
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

var SelecaoBoletoAlteracao = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Etapa = PropertyEntity({ val: ko.observable(EnumBoletoAlteracaoEtapa.Selecao), options: _etapaBoletoAlteracao, def: EnumBoletoAlteracaoEtapa.Selecao });;

    this.Cliente = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Cliente:", idBtnSearch: guid(), visible: ko.observable(true), enable: ko.observable(true) });
    this.BoletoConfiguracao = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Banco:", idBtnSearch: guid(), visible: ko.observable(true), enable: ko.observable(true) });

    this.DataVencimentoInicial = PropertyEntity({ text: "Data Vencimento de: ", getType: typesKnockout.date, enable: ko.observable(true), required: false });
    this.DataVencimentoFinal = PropertyEntity({ text: "Até: ", getType: typesKnockout.date, enable: ko.observable(true), required: false });
    this.DataEmissaoInicial = PropertyEntity({ text: "Data Emissão de: ", getType: typesKnockout.date, enable: ko.observable(true), required: false });
    this.DataEmissaoFinal = PropertyEntity({ text: "Até: ", getType: typesKnockout.date, enable: ko.observable(true), required: false });

    this.PesquisarBoletos = PropertyEntity({ eventClick: PesquisarSelecaoBoletosClick, type: types.event, text: "Pesquisar", visible: ko.observable(true), enable: ko.observable(true) });
    this.Boletos = PropertyEntity({ idGrid: guid(), enable: ko.observable(true) });
    this.SelecionarTodos = PropertyEntity({ val: ko.observable(false), def: false, type: types.map, getType: typesKnockout.bool, text: "Marcar/Desmarcar Todos", visible: ko.observable(false), enable: ko.observable(true) });

    this.ListaBoletos = PropertyEntity({ type: types.map, getType: typesKnockout.dynamic, val: ko.observable(""), def: "", idGrid: guid() });

    this.Proximo = PropertyEntity({ eventClick: ProximoSelecaoBoletoClick, type: types.event, text: "Próximo", visible: ko.observable(true), enable: ko.observable(true) });
}

//*******EVENTOS*******

function loadBoletoAlteracao() {

    _pesquisaBoletoAlteracao = new PesquisaBoletoAlteracao();
    KoBindings(_pesquisaBoletoAlteracao, "knockoutPesquisaBoletoAlteracao");

    new BuscarClientes(_pesquisaBoletoAlteracao.Cliente);
    new BuscarBoletoConfiguracao(_pesquisaBoletoAlteracao.BoletoConfiguracao);

    _boletoAlteracao = new SelecaoBoletoAlteracao();
    KoBindings(_boletoAlteracao, "knockoutSelecaoBoletoAlteracao");

    HeaderAuditoria("BoletoAlteracao", _boletoAlteracao);

    new BuscarClientes(_boletoAlteracao.Cliente);
    new BuscarBoletoConfiguracao(_boletoAlteracao.BoletoConfiguracao, RetornoConfiguracaoBanco);

    _boletoAlteracao.Etapa.val(EnumBoletoAlteracaoEtapa.Selecao);

    buscarAlteracoesBoleto();
    buscarBoletosAlteracao();

    loadEtapaBoletoAlteracao();
    loadAlteracaoBoletoAlteracao();
    loadImpressaoBoletoAlteracao();
    loadRemessaBoletoAlteracao();
    loadEmailBoletoAlteracao();

}

function RetornoConfiguracaoBanco(data) {
    _boletoAlteracao.BoletoConfiguracao.codEntity(data.Codigo);
    _boletoAlteracao.BoletoConfiguracao.val(data.DescricaoBanco);
}

function PesquisarSelecaoBoletosClick(e, sender) {
    buscarBoletosAlteracao();
}

function ProximoSelecaoBoletoClick(e, sender) {
    if (_gridBoletos == undefined) {
        exibirMensagem(tipoMensagem.aviso, "Selecione os Boletos", "Por favor selecione ao menos um boleto para realizar a sua alteração.");
        return;
    }
    var boletosSelecionados = _gridBoletos.ObterMultiplosSelecionados();

    if (boletosSelecionados.length > 0 || _boletoAlteracao.Codigo.val() > 0) {
        var dataGrid = new Array();

        $.each(boletosSelecionados, function (i, boleto) {

            var obj = new Object();
            obj.Codigo = boleto.Codigo;

            dataGrid.push(obj);
        });

        _boletoAlteracao.ListaBoletos.val(JSON.stringify(dataGrid));

        var data = { ListaBoletos: _boletoAlteracao.ListaBoletos.val(), Codigo: _boletoAlteracao.Codigo.val() };
        executarReST("BoletoAlteracao/IniciarAlteracaoBoleto", data, function (arg) {
            if (arg.Success) {
                PosicionarEtapa(arg.Data);
                _boletoAlteracao.Etapa.val(EnumBoletoAlteracaoEtapa.Alteracao);
                _boletoAlteracao.Codigo.val(arg.Data.Codigo);
                _alteracaoBoletoAlteracao.Codigo.val(arg.Data.Codigo);
                _emailBoletoAlteracao.Codigo.val(arg.Data.Codigo);
                _remessaBoletoAlteracao.Codigo.val(arg.Data.Codigo);
                _impressaoBoletoAlteracao.Codigo.val(arg.Data.Codigo);

                $("#knockoutAlteracaoBoletoAlteracao").show();
                _etapaAtual = 2;
                $("#" + _etapaBoletoAlteracao.Etapa2.idTab).click();
                
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Etapa inicial concluída, siga a etapa 2.");
            } else {
                exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
            }
        });

    }
    else {
        exibirMensagem(tipoMensagem.aviso, "Selecione os Boletos", "Por favor selecione ao menos um boleto para realizar a sua alteração.");
        $("#knockoutSelecaoBoletoAlteracao").show();
        $("#knockoutAlteracaoBoletoAlteracao").hide();
        $("#knockoutImpressaoBoletoAlteracao").hide();
        $("#knockoutRemessaBoletoAlteracao").hide();
        $("#knockoutEmailBoletoAlteracao").hide();
    }
}

//*******MÉTODOS*******

function editarBoletoAlteracao(grid) {

    LimparOcultarAbas();
    $("#" + _etapaBoletoAlteracao.Etapa1.idTab).attr("data-bs-toggle", "tab");
    $("#" + _etapaBoletoAlteracao.Etapa1.idTab + " .step").attr("class", "step lightgreen");
    $("#" + _etapaBoletoAlteracao.Etapa1.idTab).click();

    limparCamposBoletoAlteracao();
    limparCamposAlteracao();
    limparCamposImpressaoBoleto();
    limparCamposRemessaBoleto();
    limparCamposEmailBoleto();

    _boletoAlteracao.Codigo.val(grid.Codigo);
    BuscarPorCodigo(_boletoAlteracao, "BoletoAlteracao/BuscarPorCodigo", function (arg) {
        _pesquisaBoletoAlteracao.ExibirFiltros.visibleFade(false);

        _boletoAlteracao.Codigo.val(arg.Data.Codigo);
        _alteracaoBoletoAlteracao.Codigo.val(arg.Data.Codigo);
        _emailBoletoAlteracao.Codigo.val(arg.Data.Codigo);
        _remessaBoletoAlteracao.Codigo.val(arg.Data.Codigo);
        _impressaoBoletoAlteracao.Codigo.val(arg.Data.Codigo);

        PosicionarEtapa(arg.Data);

        buscarBoletosAlteracao();
        BuscarBoletosImpressao();
        BuscarBoletosRemessa();
        BuscarBoletosEmail();
    }, null);
}

function buscarAlteracoesBoleto() {
    var editar = { descricao: "Editar", id: "clasEditar", evento: "onclick", metodo: editarBoletoAlteracao, tamanho: "20", icone: "" };
    var menuOpcoes = new Object();
    menuOpcoes.tipo = TypeOptionMenu.link;
    menuOpcoes.opcoes = new Array();
    menuOpcoes.opcoes.push(editar);

    _gridBoletoAlteracao = new GridView(_pesquisaBoletoAlteracao.Pesquisar.idGrid, "BoletoAlteracao/Pesquisa", _pesquisaBoletoAlteracao, menuOpcoes, null, null, null);
    _gridBoletoAlteracao.CarregarGrid();
}

function buscarBoletosAlteracao() {
    var somenteLeitura = false;

    _boletoAlteracao.SelecionarTodos.visible(true);
    _boletoAlteracao.SelecionarTodos.val(false);

    var multiplaescolha = {
        basicGrid: null,
        eventos: function () { },
        selecionados: new Array(),
        naoSelecionados: new Array(),
        SelecionarTodosKnout: _boletoAlteracao.SelecionarTodos,
        somenteLeitura: somenteLeitura,
    }

    _gridBoletos = new GridView(_boletoAlteracao.Boletos.idGrid, "BoletoAlteracao/PesquisaBoletosSelecao", _boletoAlteracao, null, null, null, null, null, null, multiplaescolha);
    _gridBoletos.CarregarGrid();
}

function limparCamposBoletoAlteracao() {
    LimparCampos(_boletoAlteracao);
    buscarBoletosAlteracao();
}

function exibirCamposObrigatorio() {
    exibirMensagem("atencao", "Campo Obrigatório", "Por Favor, Informe os campos obrigatórios");
}