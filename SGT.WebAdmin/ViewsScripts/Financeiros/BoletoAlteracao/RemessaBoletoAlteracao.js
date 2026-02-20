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
/// <reference path="../../../js/libs/jquery.twbsPagination.js" />
/// <reference path="../../../js/libs/jquery.globalize.js" />
/// <reference path="../../../js/libs/jquery.globalize.pt-BR.js" />
/// <reference path="BoletoAlteracao.js" />
/// <reference path="EtapaBoletoAlteracao.js" />
/// <reference path="AlteracaoBoletoAlteracao.js" />
/// <reference path="ImpressaoBoletoAlteracao.js" />
/// <reference path="EmailBoletoAlteracao.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _remessaBoletoAlteracao;
var _gridRemessaBoleto;

var RemessaBoletoAlteracao = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });

    this.BaixarRemessa = PropertyEntity({ eventClick: DownloadRemessasClick, type: types.event, text: "Download Remessa(s)", visible: ko.observable(true), enable: ko.observable(true) });
    this.Gerar = PropertyEntity({ eventClick: GerarRemessaClick, type: types.event, text: "Gerar Remessa de Alteração", visible: ko.observable(true), enable: ko.observable(true) });
    this.Atualizar = PropertyEntity({ eventClick: AtualizarBoletosRemessaClick, type: types.event, text: "Atualizar Tabela", visible: ko.observable(true), enable: ko.observable(true) });
    this.Boletos = PropertyEntity({ idGrid: guid(), enable: ko.observable(true) });

    this.Anterior = PropertyEntity({ eventClick: AnteriorRemessaBoletoAlteracaoClick, type: types.event, text: "Anterior", visible: ko.observable(true), enable: ko.observable(true) });
    this.Proximo = PropertyEntity({ eventClick: ProximoRemessaBoletoAlteracaoClick, type: types.event, text: "Próximo", visible: ko.observable(true), enable: ko.observable(true) });
}

//*******EVENTOS*******

function loadRemessaBoletoAlteracao() {
    _remessaBoletoAlteracao = new RemessaBoletoAlteracao();
    KoBindings(_remessaBoletoAlteracao, "knockoutRemessaBoletoAlteracao");

    var download = { descricao: "Download", id: "clasEditar", evento: "onclick", metodo: DownloadRemessaClick, tamanho: "10", icone: "" };
    var menuOpcoes = new Object();
    menuOpcoes.tipo = TypeOptionMenu.link;
    menuOpcoes.opcoes = new Array();
    menuOpcoes.opcoes.push(download);

    var header = [{ data: "Codigo", visible: false },
    { data: "CodigoRemessa", visible: false },
    { data: "BoletoStatusTitulo", visible: false },
    { data: "Pessoa", title: "Pessoa", width: "20%", className: "text-align-left" },
    { data: "CodigoTitulo", title: "Nº Título", width: "20%", className: "text-align-left" },
    { data: "DataEmissao", title: "Data Emissão", width: "8%", className: "text-align-right" },
    { data: "DataVencimentoOriginal", title: "Vencimento Original", width: "10%", className: "text-align-center" },
    { data: "DataVencimentoAlterado", title: "Vencimento Alterado", width: "10%", className: "text-align-center" },
    { data: "Valor", title: "Valor", width: "8%", className: "text-align-right" },
    { data: "NossoNumero", title: "Nosso Número", width: "8%", className: "text-align-center" },
    { data: "NumeroDocumento", title: "Nº Documento", width: "8%", className: "text-align-center" },
    { data: "Observacao", title: "Observação", width: "10%", className: "text-align-left" },
    { data: "NumeroRemessa", title: "Número Remessa", width: "8%", className: "text-align-center" }
    ];

    _gridRemessaBoleto = new BasicDataTable(_remessaBoletoAlteracao.Boletos.idGrid, header, menuOpcoes);
}

function DownloadRemessaClick(e) {
    var dados = { Codigo: e.CodigoRemessa }
    executarDownload("BoletoAlteracao/DownloadRemessaAlteracao", dados);
}

function DownloadRemessasClick(e, sender) {
    var dados = { Codigo: _remessaBoletoAlteracao.Codigo.val() }
    executarDownload("BoletoAlteracao/DownloadRemessas", dados);
}

function GerarRemessaClick(e, sender) {
    var data = { Codigo: _remessaBoletoAlteracao.Codigo.val() };
    executarReST("BoletoAlteracao/GerarRemessaAlteracao", data, function (arg) {
        if (arg.Success) {
            BuscarBoletosRemessa();
            exibirMensagem(tipoMensagem.ok, "Sucesso", "Por favor aguarde a geração da remessa pelo integrador.");
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    });
}

function AtualizarBoletosRemessaClick(e, sender) {
    BuscarBoletosRemessa();
}

function ProximoRemessaBoletoAlteracaoClick(e, sender) {
    var data = { Codigo: _remessaBoletoAlteracao.Codigo.val(), Etapa: EnumBoletoAlteracaoEtapa.Email };
    executarReST("BoletoAlteracao/AtualizarEtapa", data, function (arg) {
        if (arg.Success) {
            _boletoAlteracao.Etapa.val(arg.Data.Etapa);
            PosicionarEtapa(arg.Data);
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    });
}

function AnteriorRemessaBoletoAlteracaoClick(e, sender) {
    var data = { Codigo: _remessaBoletoAlteracao.Codigo.val(), Etapa: EnumBoletoAlteracaoEtapa.Impressao };
    executarReST("BoletoAlteracao/AtualizarEtapa", data, function (arg) {
        if (arg.Success) {
            _boletoAlteracao.Etapa.val(arg.Data.Etapa);
            PosicionarEtapa(arg.Data);
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    });
}

//*******MÉTODOS*******

function BuscarBoletosRemessa() {
    if (_remessaBoletoAlteracao.Codigo.val() > 0) {
        var data = { Codigo: _remessaBoletoAlteracao.Codigo.val() };
        executarReST("BoletoAlteracao/AtualizarBoletos", data, function (arg) {
            if (arg.Success) {
                recarregarGridRemessaBoleto(arg.Data);
            } else {
                exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
            }
        });
    } else {
        $("#knockoutRemessaBoletoAlteracao").hide();
    }
}

function recarregarGridRemessaBoleto(data) {
    var dataGrid = new Array();

    $.each(data, function (i, titulo) {
        var obj = new Object();

        obj.Codigo = titulo.Codigo;
        obj.CodigoRemessa = titulo.CodigoRemessa;        
        obj.BoletoStatusTitulo = titulo.BoletoStatusTitulo;
        obj.Pessoa = titulo.Pessoa;
        obj.CodigoTitulo = titulo.CodigoTitulo;
        obj.DataEmissao = titulo.DataEmissao;
        obj.DataVencimentoOriginal = titulo.DataVencimentoOriginal;
        obj.DataVencimentoAlterado = titulo.DataVencimentoAlterado;
        obj.Valor = titulo.Valor;
        obj.NossoNumero = titulo.NossoNumero;
        obj.NumeroDocumento = titulo.NumeroDocumento;
        obj.Observacao = titulo.Observacao;
        obj.NumeroRemessa = titulo.NumeroRemessa;

        dataGrid.push(obj);
    });

    _gridRemessaBoleto.CarregarGrid(dataGrid);
}


function limparCamposRemessaBoleto() {
    LimparCampos(_remessaBoletoAlteracao);
}

function exibirCamposObrigatorio() {
    exibirMensagem("atencao", "Campo Obrigatório", "Por Favor, Informe os campos obrigatórios");
}