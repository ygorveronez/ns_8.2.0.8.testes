/// <reference path="../../Enumeradores/EnumTipoIntegracao.js" />
/// <reference path="../../Enumeradores/EnumSituacoesCarga.js" />
/// <reference path="../../Consultas/Carga.js" />
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
/// <reference path="../../Consultas/GrupoPessoa.js" />
/// <reference path="../../Consultas/Cliente.js" />
/// <reference path="../../Consultas/TransportadorElectrolux.js" />
/// <reference path="../../Configuracao/ConfiguracaoTMS.js" />


//*******MAPEAMENTO KNOUCKOUT*******
var _pesquisaHistoricoIntegracaoRetorno;
var _gridHistoricoIntegracaoRetorno;
var _pesquisaHistoricoIntegracaoDocumentoElectrolux;
var _gridHistoricoIntegracaoDocumentoElectrolux;
var _gridDocumentoElectrolux;
var _pesquisaDocumentoElectrolux;
var _consultaDocumentoElectrolux;
var _vinculoCargaDocumentoElectrolux;

var VinculoCarga = function () {
    this.DocumentoTransporte = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), val: ko.observable(""), idBtnSearch: guid() });
    this.Carga = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Integracoes.DocumentoElectrolux.Carga, idBtnSearch: guid() });

    this.Vincular = PropertyEntity({ eventClick: VincularCargaClick, type: types.event, text: Localization.Resources.Integracoes.DocumentoElectrolux.Vincular, visible: ko.observable(true) });
};

var ConsultaDocumentoElectrolux = function () {
    var dataAtual = moment().format("DD/MM/YYYY");
    this.IdentificadorTransportadorElectrolux = PropertyEntity({ type: types.string, codEntity: ko.observable(0), required: false, text: "Identificador Electrolux", issue: 70, visible: ko.observable(false) });
    this.Empresa = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), required: false, text: Localization.Resources.Integracoes.DocumentoElectrolux.Empresa, idBtnSearch: guid(), visible: ko.observable(true) });
    this.DataInicial = PropertyEntity({ text: Localization.Resources.Integracoes.DocumentoElectrolux.DataInicial, getType: typesKnockout.date, val: ko.observable(dataAtual), def: dataAtual });
    this.DataFinal = PropertyEntity({ text: Localization.Resources.Integracoes.DocumentoElectrolux.DataFinal, getType: typesKnockout.date, val: ko.observable(dataAtual), def: dataAtual });

    this.DataInicial.dateRangeLimit = this.DataFinal;
    this.DataFinal.dateRangeInit = this.DataInicial;

    this.ConsultarDocumentoElectrolux = PropertyEntity({ eventClick: ConsultarDocumentoElectroluxClick, type: types.event, text: Localization.Resources.Integracoes.DocumentoElectrolux.Consultar, visible: ko.observable(true) });
};

var PesquisaDocumentoElectrolux = function () {
    this.NumeroNotaFiscal = PropertyEntity({ text: Localization.Resources.Integracoes.DocumentoElectrolux.NumeroNotaFiscal, def: "", val: ko.observable(""), getType: typesKnockout.int, maxlength: 15 });
    this.NumeroIdentificacaoServico = PropertyEntity({ text: Localization.Resources.Integracoes.DocumentoElectrolux.NumeroIdentificacaoServico, def: "", val: ko.observable(""), getType: typesKnockout.int, maxlength: 20 });
    this.DataInicial = PropertyEntity({ text: Localization.Resources.Integracoes.DocumentoElectrolux.DataInicial, getType: typesKnockout.date });
    this.DataFinal = PropertyEntity({ text: Localization.Resources.Integracoes.DocumentoElectrolux.DataFinal, getType: typesKnockout.date });
    this.SemCarga = PropertyEntity({ text: Localization.Resources.Integracoes.DocumentoElectrolux.DocNaoVinculado, val: ko.observable(true), def: true, getType: typesKnockout.bool });
    
    this.DataInicial.dateRangeLimit = this.DataFinal;
    this.DataFinal.dateRangeInit = this.DataInicial;
    

    this.Pesquisar = PropertyEntity({
        eventClick: function (e) {
            _gridDocumentoElectrolux.CarregarGrid();
        }, type: types.event, text: Localization.Resources.Integracoes.DocumentoElectrolux.Pesquisar, idGrid: guid(), visible: ko.observable(true)
    });

    this.ConsultarDocumentoElectrolux = PropertyEntity({ eventClick: AbrirTelaConsultaDocumentoElectrolux, type: types.event, text: Localization.Resources.Integracoes.DocumentoElectrolux.Consulta, visible: ko.observable(true) });

    this.HistoricoIntegracoes = PropertyEntity({ eventClick: ExibirHistoricoIntegracaoGeral, type: types.event, text: Localization.Resources.Integracoes.DocumentoElectrolux.HistoricoIntegracoes, visible: ko.observable(true) });
};

var PesquisaHistoricoIntegracaoDocumentoElectrolux = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
};

//*******EVENTOS*******

function LoadDocumentoElectrolux() {
    _pesquisaDocumentoElectrolux = new PesquisaDocumentoElectrolux();
    KoBindings(_pesquisaDocumentoElectrolux, "knockoutDocumentoElectrolux", _pesquisaDocumentoElectrolux.Pesquisar.id);

    _consultaDocumentoElectrolux = new ConsultaDocumentoElectrolux();
    KoBindings(_consultaDocumentoElectrolux, "divConsultaDocumentoElectrolux");

    HeaderAuditoria("DocumentoElectrolux", {});

    _vinculoCargaDocumentoElectrolux = new VinculoCarga();
    KoBindings(_vinculoCargaDocumentoElectrolux, "divVinculoCarga");

    new BuscarCargas(_vinculoCargaDocumentoElectrolux.Carga, null, EnumTipoIntegracao.Electrolux, [EnumSituacoesCarga.Nova, EnumSituacoesCarga.AgNFe, EnumSituacoesCarga.AgTransportador, EnumSituacoesCarga.CalculoFrete, EnumSituacoesCarga.AgIntegracao, EnumSituacoesCarga.EmTransporte, EnumSituacoesCarga.Encerrada], null);


    BuscarDocumentosElectrolux();
    BuscarTransportadores(_consultaDocumentoElectrolux.Empresa, retornoEmpresaElectrolux, null, true);
}

function ConsultarDocumentoElectroluxClick(e, sender) {
    var identificadorEletrolux = Globalize.parseInt(_consultaDocumentoElectrolux.IdentificadorTransportadorElectrolux.val());
    if (isNaN(identificadorEletrolux) || identificadorEletrolux <= 0) {
        if (_consultaDocumentoElectrolux.DataInicial.val() == "" || _consultaDocumentoElectrolux.DataFinal.val() == "") {
            exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Atencao, Localization.Resources.Integracoes.DocumentoElectrolux.AtencaoPreenchimento);
            return;
        }
    }

    var dados = RetornarObjetoPesquisa(_consultaDocumentoElectrolux);

    executarReST("DocumentoElectrolux/ConsultarDocumentoElectrolux", dados, function (r) {
        if (r.Success) {
            if (r.Data) {
                exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Successo, "Documentos consultados com sucesso!");
                _gridDocumentoElectrolux.CarregarGrid();
                Global.fecharModal("divConsultaDocumentoElectrolux");
            } else {
                exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.Atencao, r.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, r.Msg);
        }
    });
}

function VincularCargaClick(e, sender) {
    exibirConfirmacao(Localization.Resources.Gerais.Geral.Atencao, "Deseja realmente vincular o documento de transporte " + e.DocumentoTransporte.val() + " à carga " + e.Carga.val() + "?", function () {
        executarReST("DocumentoElectrolux/VincularCargaAoDocumentoTransporte", { DocumentoTransporte: _vinculoCargaDocumentoElectrolux.DocumentoTransporte.codEntity(), Carga: _vinculoCargaDocumentoElectrolux.Carga.codEntity() }, function (r) {
            if (r.Success) {
                if (r.Data) {
                    exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Successo, "Documento de transporte vinculado à carga com sucesso.");
                    Global.fecharModal("divVinculoCarga");
                    _gridDocumentoElectrolux.CarregarGrid();
                } else {
                    exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.Atencao, r.Msg);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, r.Msg);
            }
        });
    });
}

//*******MÉTODOS*******

function AbrirTelaConsultaDocumentoElectrolux() {
    LimparCampos(_consultaDocumentoElectrolux);

    var modalConsultaDocumentoElectrolux = new bootstrap.Modal(document.getElementById("divConsultaDocumentoElectrolux"), { backdrop: true, keyboard: true });
    modalConsultaDocumentoElectrolux.show();
}

function AbrirTelaVinculoCarga(e, sender) {
    LimparCampos(_vinculoCargaDocumentoElectrolux);

    _vinculoCargaDocumentoElectrolux.DocumentoTransporte.codEntity(e.Codigo);
    _vinculoCargaDocumentoElectrolux.DocumentoTransporte.val(e.Numero);

    Global.abrirModal('divVinculoCarga');
}

function CriarCargaPorDT(e) {
    exibirConfirmacao(Localization.Resources.Gerais.Geral.Atencao, "Deseja realmente criar a carga com o documento de transporte " + e.Numero + "?", function () {
        executarReST("DocumentoElectrolux/GerarCarga", { DocumentoTransporte: e.Codigo }, function (r) {
            if (r.Success) {
                if (r.Data) {
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Carga " + r.Data.Carga + " criada com sucesso.");
                    _gridDocumentoElectrolux.CarregarGrid();
                } else {
                    exibirMensagem(tipoMensagem.aviso, "Atenção!", r.Msg);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, "Falha", r.Msg);
            }
        });
    });
}
function AtualizaDocumentoTransporteElectrolux(e) {
    exibirConfirmacao("Atenção!", "Deseja realmente atualizar os dados do documento de transporte " + e.Numero + "?", function () {
        executarReST("DocumentoElectrolux/AtualizarDocumentoElectrolux", { DocumentoTransporte: e.Codigo }, function (r) {
            if (r.Success) {
                if (r.Data) {
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Dados atualizados com sucesso.");
                    _gridDocumentoElectrolux.CarregarGrid();
                } else {
                    exibirMensagem(tipoMensagem.aviso, "Atenção!", r.Msg);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, "Falha", r.Msg);
            }
        });
    });
}
function ExibirHistoricoIntegracaoDocumentoElectrolux(documentoTransporte) {
    BuscarHistoricoIntegracaoDocumentoElectrolux(documentoTransporte);
    Global.abrirModal('divModalHistoricoIntegracaoDocumentoElectrolux');
}

function BuscarHistoricoIntegracaoDocumentoElectrolux(documentoTransporte) {
    _pesquisaHistoricoIntegracaoDocumentoElectrolux = new PesquisaHistoricoIntegracaoDocumentoElectrolux();
    _pesquisaHistoricoIntegracaoDocumentoElectrolux.Codigo.val(documentoTransporte.Codigo);

    var download = { descricao: "Download Arquivos", id: guid(), metodo: DownloadArquivosHistoricoIntegracaoDocumentoElectrolux, tamanho: "8", icone: "" };
    var integracoesRetorno = { descricao: "Integrações de Retorno", id: guid(), metodo: ExibirHistoricoIntegracaoDocumentoElectroluxRetorno, tamanho: "8", icone: "" };

    var menuOpcoes = {
        tipo: TypeOptionMenu.list,
        opcoes: [download, integracoesRetorno]
    };

    _gridHistoricoIntegracaoDocumentoElectrolux = new GridView("tblHistoricoIntegracaoDocumentoElectrolux", "DocumentoElectrolux/ConsultarHistoricoIntegracaoDocumentoElectrolux", _pesquisaHistoricoIntegracaoDocumentoElectrolux, menuOpcoes, { column: 1, dir: orderDir.desc });
    _gridHistoricoIntegracaoDocumentoElectrolux.CarregarGrid();
}

function DownloadArquivosHistoricoIntegracaoDocumentoElectrolux(integracao) {
    executarDownload("DocumentoElectrolux/DownloadArquivosHistoricoIntegracaoDocumentoTransporte", { Codigo: integracao.Codigo });
}

function ExibirHistoricoIntegracaoDocumentoElectroluxRetorno(integracao) {
    BuscarHistoricoIntegracaoRetorno(integracao);
    Global.abrirModal('divModalHistoricoIntegracaoRetorno');
}

function BuscarHistoricoIntegracaoRetorno(integracao) {
    _pesquisaHistoricoIntegracaoRetorno = new PesquisaHistoricoIntegracaoDocumentoElectrolux();
    _pesquisaHistoricoIntegracaoRetorno.Codigo.val(integracao.Codigo);

    var download = { descricao: "Download Arquivos", id: guid(), metodo: DownloadArquivosHistoricoIntegracaoDocumentoElectrolux, tamanho: "15", icone: "" };

    var menuOpcoes = {
        tipo: TypeOptionMenu.link,
        opcoes: [download]
    };

    _gridHistoricoIntegracaoRetorno = new GridView("tblHistoricoIntegracaoRetorno", "DocumentoElectrolux/ConsultarIntegracoesRetorno", _pesquisaHistoricoIntegracaoRetorno, menuOpcoes, { column: 1, dir: orderDir.desc });
    _gridHistoricoIntegracaoRetorno.CarregarGrid();
}

function ExibirHistoricoIntegracaoGeral() {
    BuscarHistoricoIntegracaoGeral();

    var modalHistoricoIntegracaoGeralElectrolux = new bootstrap.Modal(document.getElementById("divModalHistoricoIntegracaoGeralElectrolux"), { backdrop: true, keyboard: true });
    modalHistoricoIntegracaoGeralElectrolux.show();
}

function BuscarHistoricoIntegracaoGeral() {
    var download = { descricao: "Download Arquivos", id: guid(), metodo: DownloadArquivosHistoricoIntegracaoDocumentoElectrolux, tamanho: "15", icone: "" };

    var menuOpcoes = {
        tipo: TypeOptionMenu.list,
        opcoes: [download]
    };

    _gridHistoricoIntegracaoDocumentoElectrolux = new GridView("tblHistoricoIntegracaoGeralElectrolux", "DocumentoElectrolux/ConsultarHistoricoIntegracaoGeral", null, menuOpcoes, { column: 1, dir: orderDir.desc });
    _gridHistoricoIntegracaoDocumentoElectrolux.CarregarGrid();
}

function BuscarDocumentosElectrolux() {
    var menuOpcoes = new Object();
    menuOpcoes.tipo = TypeOptionMenu.list;
    menuOpcoes.opcoes = new Array();
    menuOpcoes.opcoes.push({ id: guid(), descricao: "Vincular à Carga", metodo: AbrirTelaVinculoCarga, tamanho: "15", icone: "" });
    menuOpcoes.opcoes.push({ id: guid(), descricao: "Criar Carga", metodo: CriarCargaPorDT, tamanho: "15", icone: "" });
    menuOpcoes.opcoes.push({ id: guid(), descricao: "Atualizar DT", metodo: AtualizaDocumentoTransporteElectrolux, tamanho: "15", icone: "" });
    menuOpcoes.opcoes.push({ id: guid(), descricao: "Histórico de Integrações", metodo: ExibirHistoricoIntegracaoDocumentoElectrolux, tamanho: "15", icone: "" });
    
    var configExportacao = {
        url: "DocumentoElectrolux/ExportarPesquisa",
        titulo: "Documentos de Transporte da Natura"
    };

    _gridDocumentoElectrolux = new GridView(_pesquisaDocumentoElectrolux.Pesquisar.idGrid, "DocumentoElectrolux/Pesquisa", _pesquisaDocumentoElectrolux, menuOpcoes, null, null, null, null, null, null, null, null, configExportacao);
    _gridDocumentoElectrolux.CarregarGrid();
}

function retornoEmpresaElectrolux(registroSelecionado) {
    _consultaDocumentoElectrolux.Empresa.codEntity(registroSelecionado.Codigo);
    _consultaDocumentoElectrolux.Empresa.val(registroSelecionado.RazaoSocial);
    _consultaDocumentoElectrolux.IdentificadorTransportadorElectrolux.val(registroSelecionado.IdentificadorTransportadorElectrolux);

}