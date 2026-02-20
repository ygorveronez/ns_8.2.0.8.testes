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
/// <reference path="../../../js/libs/jquery.twbsPagination.js" />
/// <reference path="../../../js/libs/jquery.globalize.js" />
/// <reference path="../../../js/libs/jquery.globalize.pt-BR.js" />
/// <reference path="BoletoGeracaoEtapa.js" />
/// <reference path="GeracaoFrancesinha.js" />
/// <reference path="GeracaoRemessa.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _envioEmail;
var _gridEnvioEmail;

var EnvioEmail = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });

    this.TitulosParaEnvioEmail = PropertyEntity({ idGrid: guid(), enable: ko.observable(true) });
    this.MensagemEmail = PropertyEntity({ text: "Mensagem para envio de E-mail: ", required: false, maxlength: 3000, enable: ko.observable(true), val: ko.observable("") });

    this.TagRazaoSocialCliente = PropertyEntity({ eventClick: function (e) { InserirTag(_envioEmail.MensagemEmail.id, "#TagRazaoSocialCliente#"); }, type: types.event, text: "Razão Social Cliente" });
    this.TagCNPJCliente = PropertyEntity({ eventClick: function (e) { InserirTag(_envioEmail.MensagemEmail.id, "#TagCNPJCliente#"); }, type: types.event, text: "CNPJ Cliente" });
    this.TagRazaoSocialAdmin = PropertyEntity({ eventClick: function (e) { InserirTag(_envioEmail.MensagemEmail.id, "#TagRazaoSocialAdmin#"); }, type: types.event, text: "Razão Social Admin" });
    this.TagCNPJAdmin = PropertyEntity({ eventClick: function (e) { InserirTag(_envioEmail.MensagemEmail.id, "#TagCNPJAdmin#"); }, type: types.event, text: "CNPJ Admin" });
    this.TagQuebraLinha = PropertyEntity({ eventClick: function (e) { InserirTag(_envioEmail.MensagemEmail.id, "#qLinha#"); }, type: types.event, text: "Quebra Linha" });

    this.EnviarEmail = PropertyEntity({ eventClick: EnviarEmailClick, type: types.event, text: "Enviar E-mail", visible: ko.observable(false), enable: ko.observable(true) });
    this.SelecionarTodos = PropertyEntity({ val: ko.observable(false), def: false, type: types.map, getType: typesKnockout.bool, text: "Marcar/Desmarcar Todos", visible: ko.observable(false), enable: ko.observable(true) });
    this.ListaTitulos = PropertyEntity({ type: types.map, getType: typesKnockout.dynamic, val: ko.observable(""), def: "", idGrid: guid() });
    this.TotalizadorValorSelecionado = PropertyEntity({ text: "Valor total títulos: ", getType: typesKnockout.string, def: "0,00", val: ko.observable("0,00"), enable: ko.observable(true), visible: ko.observable(true) });

    this.Proximo = PropertyEntity({ eventClick: ProximoGeracaoFrancesinhaClick, type: types.event, text: "Próximo", visible: ko.observable(true), enable: ko.observable(true) });
}

//*******EVENTOS*******

function loadEnvioEmail() {
    _envioEmail = new EnvioEmail();
    KoBindings(_envioEmail, "knockoutEnvioEmail");
}

function ProximoGeracaoFrancesinhaClick(e, sender) {
    var boletos = _gridRemessas.BuscarRegistros();
    if (boletos.length > 0) {
        var dataGrid = new Array();

        $.each(boletos, function (i, titulo) {

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

            dataGrid.push(obj);
        });

        _gridGeracaoFrancesinha.CarregarGrid(dataGrid);
        _etapaAtual = 5;



        $("#" + _etapaBoletoGeracao.Etapa1.idTab + " .step").attr("class", "step green");
        $("#" + _etapaBoletoGeracao.Etapa2.idTab + " .step").attr("class", "step green");
        $("#" + _etapaBoletoGeracao.Etapa3.idTab + " .step").attr("class", "step green");
        $("#" + _etapaBoletoGeracao.Etapa4.idTab + " .step").attr("class", "step green");


        $("#" + _etapaBoletoGeracao.Etapa5.idTab).attr("data-bs-toggle", "tab");
        $("#" + _etapaBoletoGeracao.Etapa5.idTab + " .step").attr("class", "step lightgreen");
        $("#" + _etapaBoletoGeracao.Etapa5.idTab).tab("show");
        $("#knockoutFrancesinha").show();
    } else {
        exibirMensagem(tipoMensagem.aviso, "Geração de Francesinha", "Por favor gere os boletos na etapa 2 antes de gerar a francesinha.");
        $("#knockoutFrancesinha").hide();
    }
}

function EnviarEmailClick(e, sender) {
    if (_gridRemessas === undefined) {
        exibirMensagem(tipoMensagem.aviso, "Envio de E-mail", "Por favor selecione ao menos um título para enviar os boletos.");
        return;
    }
    var data = { ListaTitulos: PreencherListaCodigosEmail(), MensagemEmail: _envioEmail.MensagemEmail.val() };
    executarReST("BoletoGeracao/EnviarEmailBoletos", data, function (arg) {
        if (arg.Success) {
            exibirMensagem(tipoMensagem.ok, "Sucesso", "Processo de envio de e-mail concluido.");
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    });

    //var titulosSelecionados = _gridEnvioEmail.ObterMultiplosSelecionados();

    //if (titulosSelecionados.length > 0) {
    //    var dataGrid = new Array();

    //    $.each(titulosSelecionados, function (i, titulo) {

    //        var obj = new Object();
    //        obj.Codigo = titulo.Codigo;
    //        obj.CodigoRemessa = titulo.CodigoRemessa;
    //        obj.BoletoStatusTitulo = titulo.BoletoStatusTitulo;
    //        obj.Pessoa = titulo.Pessoa;
    //        obj.DescricaoStatusBoleto = titulo.DescricaoStatusBoleto;
    //        obj.DataEmissao = titulo.DataEmissao;
    //        obj.DataVencimento = titulo.DataVencimento;
    //        obj.Valor = titulo.Valor;
    //        obj.NossoNumero = titulo.NossoNumero;
    //        obj.NumeroRemessa = titulo.NumeroRemessa;
    //        obj.CaminhoBoleto = titulo.CaminhoBoleto;

    //        dataGrid.push(obj);
    //    });

    //    _envioEmail.ListaTitulos.val(JSON.stringify(dataGrid));

    //    var data = { ListaTitulos: _envioEmail.ListaTitulos.val() };
    //    executarReST("BoletoGeracao/EnviarEmailBoletos", data, function (arg) {
    //        if (arg.Success) {
    //            exibirMensagem(tipoMensagem.ok, "Sucesso", "Processo de envio de e-mail concluido.");
    //        } else {
    //            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
    //        }
    //    });
    //} else {
    //    exibirMensagem(tipoMensagem.aviso, "Envio de E-mail", "Por favor gere a remessa na etapa 3 antes de enviar o e-mail.");
    //    $("#knockoutEnvioEmail").hide();
    //}
}

//*******MÉTODOS*******

function PreencherListaCodigosEmail() {
    var codigosEmail = new Array();

    $.each(_gridRemessas.BuscarRegistros(), function (i, titulo) {
        codigosEmail.push({ Codigo: titulo.Codigo });
    });
    return JSON.stringify(codigosEmail);
}

function buscarTitulosParaEnvioEmail() {
    var somenteLeitura = false;

    _envioEmail.EnviarEmail.visible(true);
    _envioEmail.SelecionarTodos.visible(false);
    _envioEmail.SelecionarTodos.val(false);

    var multiplaescolha = {
        basicGrid: null,
        eventos: function () { },
        selecionados: new Array(),
        naoSelecionados: new Array(),
        SelecionarTodosKnout: _envioEmail.SelecionarTodos,
        somenteLeitura: somenteLeitura
    }

    _gridEnvioEmail = new GridView(_envioEmail.TitulosParaEnvioEmail.idGrid, "BoletoGeracao/PesquisaTitulosParaEnvioEmail", _envioEmail, null, null, null, null, null, null, null);
    _gridEnvioEmail.CarregarGrid();
}

function limparCamposFatura() {
    LimparCampos(_envioEmail);
}

function exibirCamposObrigatorio() {
    exibirMensagem("atencao", "Campo Obrigatório", "Por Favor, Informe os campos obrigatórios");
}