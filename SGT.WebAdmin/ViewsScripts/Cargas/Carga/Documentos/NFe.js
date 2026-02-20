/// <reference path="../../../../js/libs/jquery-2.1.1.js" />
/// <reference path="../../../../js/Global/CRUD.js" />
/// <reference path="../../../../js/Global/Rest.js" />
/// <reference path="../../../../js/Global/Mensagem.js" />
/// <reference path="../../../../js/Global/Grid.js" />
/// <reference path="../../../../js/bootstrap/bootstrap.js" />
/// <reference path="../../../../js/libs/jquery.blockui.js" />
/// <reference path="../../../../js/Global/knoutViewsSlides.js" />
/// <reference path="../../../../js/libs/jquery.maskMoney.js" />
/// <reference path="../../../../js/plugin/datatables/jquery.dataTables.js" />
/// <reference path="../../../../js/libs/jquery.twbsPagination.js" />
/// <reference path="../../../../js/libs/jquery.globalize.js" />
/// <reference path="../../../../js/libs/jquery.globalize.pt-BR.js" />
/// <reference path="../../../Enumeradores/EnumStatusAverbacaoCTe.js" />

//*******MAPEAMENTO*******

var _pesquisaNotaFiscalEletronica;
var _gridDANFERelatorio;

var PesquisaNotaFiscalEletronica = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Relatorio = PropertyEntity({ getType: typesKnockout.report });
}

function buscarCargasNFe() {
    _pesquisaNotaFiscalEletronica = new PesquisaNotaFiscalEletronica();
    _pesquisaNotaFiscalEletronica.Codigo.val(0);

    var emitirNFe = { descricao: Localization.Resources.Cargas.Carga.EmitirAutorizarNfe, id: guid(), metodo: emitirNotaFiscalEletronica, icone: "", visibilidade: VisibilidadeEmitidoRejeitadoEmProcessamentoAguardandoAssinar };
    var baixarDANFE = { descricao: Localization.Resources.Cargas.Carga.BaixarDanfe, id: guid(), metodo: baixarDANFENotaFiscalEletronica, icone: "", visibilidade: VisibilidadeAutorizadoCancelado };
    var baixarXML = { descricao: Localization.Resources.Cargas.Carga.BaixarXmlNfe, id: guid(), metodo: baixarXMLNotaFiscalEletronica, icone: "", visibilidade: VisibilidadeAutorizadoCancelado };

    var menuOpcoes = { tipo: TypeOptionMenu.list, descricao: Localization.Resources.Gerais.Geral.Opcoes, tamanho: 7, opcoes: [emitirNFe, baixarDANFE, baixarXML] };

    _gridCargaNFe = new GridView(_cargaCTe.PesquisarNFe.idGrid, "CargaCTe/ConsultarCargaNFe", _cargaCTe, menuOpcoes);
    _gridCargaNFe.CarregarGrid();
}

function VisibilidadeEmitidoRejeitadoEmProcessamentoAguardandoAssinar(notaFiscalEletronicaGrid) {
    return notaFiscalEletronicaGrid.Status === EnumStatusNFe.Emitido || notaFiscalEletronicaGrid.Status === EnumStatusNFe.Rejeitado || notaFiscalEletronicaGrid.Status === EnumStatusNFe.EmProcessamento || notaFiscalEletronicaGrid.Status === EnumStatusNFe.AguardandoAssinar;
}

function VisibilidadeAutorizadoCancelado(notaFiscalEletronicaGrid) {
    return notaFiscalEletronicaGrid.Status === EnumStatusNFe.Autorizado || notaFiscalEletronicaGrid.Status === EnumStatusNFe.Cancelado;
}

//*******METODOS*******

function emitirNotaFiscalEletronica(notaFiscalEletronicaGrid) {
    var dados = { Codigo: notaFiscalEletronicaGrid.Codigo, CodigoCarga: notaFiscalEletronicaGrid.CodigoCarga };
    executarReST("CargaCTe/EnviarNFe", dados, function (arg) {
        if (arg.Success) {
            if (arg.Data != false) {
                _gridCargaNFe.CarregarGrid();
                exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, Localization.Resources.Cargas.Carga.NfeEmitidoComSucesso);
            } else {
                exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Atencao, arg.Msg, 60000);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, arg.Msg);
        }
    });
}

function baixarDANFENotaFiscalEletronica(notaFiscalEletronicaGrid) {
    if (notaFiscalEletronicaGrid.Status === EnumStatusNFe.Autorizado || notaFiscalEletronicaGrid.Status === EnumStatusNFe.Cancelado || notaFiscalEletronicaGrid.Status === EnumStatusNFe.AguardandoCancelarAssinar || notaFiscalEletronicaGrid.Status === EnumStatusNFe.AguardandoCartaCorrecaoAssinar) {
        _gridDANFERelatorio = new GridView("qualquercoisa", "Relatorios/DANFE/Pesquisa", _pesquisaNotaFiscalEletronica);
        _pesquisaNotaFiscalEletronica.Codigo.val(notaFiscalEletronicaGrid.Codigo);
        var _relatorioDANFE = new RelatorioGlobal("Relatorios/DANFE/BuscarDadosRelatorio", _gridDANFERelatorio, function () {
            _relatorioDANFE.loadRelatorio(function () {
                _relatorioDANFE.gerarRelatorio("Relatorios/DANFE/GerarRelatorio", EnumTipoArquivoRelatorio.PDF);
            });
        }, null, null, _pesquisaNotaFiscalEletronica);
    } else
        exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Atencao, Localization.Resources.Cargas.Carga.EstaNotaFiscalSeEncontraComStatus.format(_statusNFePesquisa[notaFiscalEletronicaGrid.Status].text), 60000);
}

function baixarXMLNotaFiscalEletronica(notaFiscalEletronicaGrid) {
    if (notaFiscalEletronicaGrid.Status === EnumStatusNFe.Autorizado || notaFiscalEletronicaGrid.Status === EnumStatusNFe.Cancelado || notaFiscalEletronicaGrid.Status === EnumStatusNFe.AguardandoCancelarAssinar || notaFiscalEletronicaGrid.Status === EnumStatusNFe.AguardandoCartaCorrecaoAssinar) {
        var dados = { Codigo: notaFiscalEletronicaGrid.Codigo };
        executarDownload("NotaFiscalEletronica/DownloadXML", dados);
    } else
        exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Atencao, Localization.Resources.Cargas.Carga.EstaNotaFiscalSeEncontraComStatus.format(_statusNFePesquisa[notaFiscalEletronicaGrid.Status].text), 60000);
}