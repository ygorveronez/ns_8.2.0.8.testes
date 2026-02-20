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
/// <reference path="EmailBoletoAlteracao.js" />
/// <reference path="RemessaBoletoAlteracao.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _impressaoBoletoAlteracao;
var _gridImpressaoBoleto;

var ImpressaoBoletoAlteracao = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });

    this.DownloadBoleto = PropertyEntity({ eventClick: DownloadBoletosClick, type: types.event, text: "Download Boleto(s)", visible: ko.observable(true), enable: ko.observable(true) });
    this.Atualizar = PropertyEntity({ eventClick: AtualizarBoletosImpressaoClick, type: types.event, text: "Atualizar Tabela", visible: ko.observable(true), enable: ko.observable(true) });
    this.Boletos = PropertyEntity({ idGrid: guid(), enable: ko.observable(true) });

    this.Anterior = PropertyEntity({ eventClick: AnteriorImpressaoBoletoAlteracaoClick, type: types.event, text: "Anterior", visible: ko.observable(true), enable: ko.observable(true) });
    this.Proximo = PropertyEntity({ eventClick: ProximoImpressaoBoletoAlteracaoClick, type: types.event, text: "Próximo", visible: ko.observable(true), enable: ko.observable(true) });
}

//*******EVENTOS*******

function loadImpressaoBoletoAlteracao() {
    _impressaoBoletoAlteracao = new ImpressaoBoletoAlteracao();
    KoBindings(_impressaoBoletoAlteracao, "knockoutImpressaoBoletoAlteracao");

    var download = { descricao: "Download", id: "clasEditar", evento: "onclick", metodo: DownloadBoletoClick, tamanho: "10", icone: "" };
    var menuOpcoes = new Object();
    menuOpcoes.tipo = TypeOptionMenu.link;
    menuOpcoes.opcoes = new Array();
    menuOpcoes.opcoes.push(download);

    var header = [{ data: "Codigo", visible: false },
    { data: "BoletoStatusTitulo", visible: false },
    { data: "Pessoa", title: "Pessoa", width: "20%", className: "text-align-left" },
    { data: "CodigoTitulo", title: "Nº Título", width: "20%", className: "text-align-left" },
    { data: "DataEmissao", title: "Data Emissão", width: "8%", className: "text-align-right" },
    { data: "DataVencimentoOriginal", title: "Vencimento Original", width: "10%", className: "text-align-center" },
    { data: "DataVencimentoAlterado", title: "Vencimento Alterado", width: "10%", className: "text-align-center" },
    { data: "Valor", title: "Valor", width: "8%", className: "text-align-right" },
    { data: "NossoNumero", title: "Nosso Número", width: "8%", className: "text-align-center" },
    { data: "NumeroDocumento", title: "Nº Documento", width: "8%", className: "text-align-center" },
    { data: "Observacao", title: "Observação", width: "20%", className: "text-align-left" }
    ];

    _gridImpressaoBoleto = new BasicDataTable(_impressaoBoletoAlteracao.Boletos.idGrid, header, menuOpcoes);
}

function DownloadBoletoClick(e) {
    var dados = { Codigo: e.CodigoTitulo }
    executarDownload("TituloFinanceiro/DownloadBoleto", dados);
}

function DownloadBoletosClick(e, sender) {
    var dados = { Codigo: _impressaoBoletoAlteracao.Codigo.val() }
    executarDownload("BoletoAlteracao/DownloadBoletos", dados);
}

function AtualizarBoletosImpressaoClick(e, sender) {
    BuscarBoletosImpressao();
}

function ProximoImpressaoBoletoAlteracaoClick(e, sender) {
    var data = { Codigo: _impressaoBoletoAlteracao.Codigo.val(), Etapa: EnumBoletoAlteracaoEtapa.Remessa };
    executarReST("BoletoAlteracao/AtualizarEtapa", data, function (arg) {
        if (arg.Success) {
            _boletoAlteracao.Etapa.val(arg.Data.Etapa);
            PosicionarEtapa(arg.Data);
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    });
}

function AnteriorImpressaoBoletoAlteracaoClick(e, sender) {
    var data = { Codigo: _impressaoBoletoAlteracao.Codigo.val(), Etapa: EnumBoletoAlteracaoEtapa.Alteracao };
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

function BuscarBoletosImpressao() {
    if (_impressaoBoletoAlteracao.Codigo.val() > 0) {
        var data = { Codigo: _impressaoBoletoAlteracao.Codigo.val() };
        executarReST("BoletoAlteracao/AtualizarBoletos", data, function (arg) {
            if (arg.Success) {
                recarregarGridImpressaoBoleto(arg.Data);
            } else {
                exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
            }
        });
    } else {
        $("#knockoutImpressaoBoletoAlteracao").hide();
    }
}

function recarregarGridImpressaoBoleto(data) {
    var dataGrid = new Array();

    $.each(data, function (i, titulo) {
        var obj = new Object();

        obj.Codigo = titulo.Codigo;
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

        dataGrid.push(obj);
    });

    _gridImpressaoBoleto.CarregarGrid(dataGrid);
}


function limparCamposImpressaoBoleto() {
    LimparCampos(_impressaoBoletoAlteracao);
}

function exibirCamposObrigatorio() {
    exibirMensagem("atencao", "Campo Obrigatório", "Por Favor, Informe os campos obrigatórios");
}