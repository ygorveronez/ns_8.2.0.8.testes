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
/// <reference path="ModeloVeicularCarga.js" />
/// <reference path="../../Consultas/GrupoPessoa.js" />
/// <reference path="../../Consultas/Cliente.js" />
/// <reference path="../../Configuracao/ConfiguracaoTMS.js" />


//*******MAPEAMENTO KNOUCKOUT*******
var _pesquisaHistoricoIntegracaoRetorno;
var _gridHistoricoIntegracaoRetorno;
var _pesquisaHistoricoIntegracaoDocumentoTransporte;
var _gridHistoricoIntegracaoDocumentoTransporte;
var _gridDocumentoTransporteNatura;
var _pesquisaDocumentoTransporteNatura;
var _consultaDocumentoTransporteNatura;
var _vinculoCargaDocumentoTransporteNatura;

var VinculoCarga = function () {
    this.DocumentoTransporte = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), val: ko.observable(""), idBtnSearch: guid() });
    this.Carga = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Carga: ", idBtnSearch: guid() });

    this.Vincular = PropertyEntity({ eventClick: VincularCargaClick, type: types.event, text: "Vincular", visible: ko.observable(true) });
};

var ConsultaDocumentoTransporteNatura = function () {
    var dataAtual = moment().format("DD/MM/YYYY");

    this.NumeroDocumentoTransporte = PropertyEntity({ text: "Número do DT:", def: "", val: ko.observable(""), getType: typesKnockout.int, maxlength: 20 });
    this.DataInicial = PropertyEntity({ text: "Data Inicial:", getType: typesKnockout.date, val: ko.observable(dataAtual), def: dataAtual });
    this.DataFinal = PropertyEntity({ text: "Data Final:", getType: typesKnockout.date, val: ko.observable(dataAtual), def: dataAtual });

    this.DataInicial.dateRangeLimit = this.DataFinal;
    this.DataFinal.dateRangeInit = this.DataInicial;

    this.ConsultarDocumentoTransporte = PropertyEntity({ eventClick: ConsultarDocumentoTransporteClick, type: types.event, text: "Consultar", visible: ko.observable(true) });
};

var PesquisaDocumentoTransporteNatura = function () {

    this.NumeroNotaFiscal = PropertyEntity({ text: "Número da NF:", def: "", val: ko.observable(""), getType: typesKnockout.int, maxlength: 15 });
    this.NumeroDocumentoTransporte = PropertyEntity({ text: "Número do DT:", def: "", val: ko.observable(""), getType: typesKnockout.int, maxlength: 20 });
    this.DataInicial = PropertyEntity({ text: "Data Inicial:", getType: typesKnockout.date });
    this.DataFinal = PropertyEntity({ text: "Data Final:", getType: typesKnockout.date });
    this.SemCarga = PropertyEntity({ text: "Documentos não vinculados à cargas", val: ko.observable(true), def: true, getType: typesKnockout.bool });

    this.DataInicial.dateRangeLimit = this.DataFinal;
    this.DataFinal.dateRangeInit = this.DataInicial;

    this.Pesquisar = PropertyEntity({
        eventClick: function (e) {
            _gridDocumentoTransporteNatura.CarregarGrid();
        }, type: types.event, text: "Pesquisar", idGrid: guid(), visible: ko.observable(true)
    });

    this.ConsultarDocumentoTransporte = PropertyEntity({ eventClick: AbrirTelaConsultaDocumentoTransporte, type: types.event, text: "Consultar Documento de Transporte", visible: ko.observable(true) });

    this.HistoricoIntegracoes = PropertyEntity({ eventClick: ExibirHistoricoIntegracaoGeral, type: types.event, text: "Histórico de Integrações", visible: ko.observable(true) });

    this.ArquivoNOTFIS = PropertyEntity({ type: types.file, eventChange: function () { }, text: "Importar NOTFIS", val: ko.observable("") });
};

var PesquisaHistoricoIntegracaoDocumentoTransporte = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
};

//*******EVENTOS*******

function LoadDocumentoTransporteNatura() {
    _pesquisaDocumentoTransporteNatura = new PesquisaDocumentoTransporteNatura();
    KoBindings(_pesquisaDocumentoTransporteNatura, "knockoutDocumentoTransporteNatura", _pesquisaDocumentoTransporteNatura.Pesquisar.id);

    _consultaDocumentoTransporteNatura = new ConsultaDocumentoTransporteNatura();
    KoBindings(_consultaDocumentoTransporteNatura, "divConsultaDocumentoTransporte");

    HeaderAuditoria("DTNatura", {});

    _vinculoCargaDocumentoTransporteNatura = new VinculoCarga();
    KoBindings(_vinculoCargaDocumentoTransporteNatura, "divVinculoCarga");

    new BuscarCargas(_vinculoCargaDocumentoTransporteNatura.Carga, null, EnumTipoIntegracao.Natura, [EnumSituacoesCarga.Nova, EnumSituacoesCarga.AgNFe, EnumSituacoesCarga.AgTransportador, EnumSituacoesCarga.CalculoFrete, EnumSituacoesCarga.AgIntegracao, EnumSituacoesCarga.EmTransporte, EnumSituacoesCarga.Encerrada], null);

    LoadAlteracaoDTNatura();
    LoadNOTFISDocumentoTransporteNatura();

    BuscarDocumentosTransporteNatura();
}

function ConsultarDocumentoTransporteClick(e, sender) {
    var numeroDT = Globalize.parseInt(_consultaDocumentoTransporteNatura.NumeroDocumentoTransporte.val());

    if (isNaN(numeroDT) || numeroDT <= 0) {
        if (_consultaDocumentoTransporteNatura.DataInicial.val() == "" || _consultaDocumentoTransporteNatura.DataFinal.val() == "") {
            exibirMensagem(tipoMensagem.atencao, "Atenção", "É necessário informar o número do DT ou um período para realizar a consulta.");
            return;
        }
    }

    var dados = RetornarObjetoPesquisa(_consultaDocumentoTransporteNatura);

    executarReST("DocumentoTransporteNatura/ConsultarDocumentoTransporte", dados, function (r) {
        if (r.Success) {
            if (r.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Documentos de transporte consultados com sucesso!");
                _gridDocumentoTransporteNatura.CarregarGrid();
                Global.fecharModal("divConsultaDocumentoTransporte");
            } else {
                exibirMensagem(tipoMensagem.aviso, "Atenção", r.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", r.Msg);
        }
    });
}

function VincularCargaClick(e, sender) {
    exibirConfirmacao("Atenção!", "Deseja realmente vincular o documento de transporte " + e.DocumentoTransporte.val() + " à carga " + e.Carga.val() + "?", function () {
        executarReST("DocumentoTransporteNatura/VincularCargaAoDocumentoTransporte", { DocumentoTransporte: _vinculoCargaDocumentoTransporteNatura.DocumentoTransporte.codEntity(), Carga: _vinculoCargaDocumentoTransporteNatura.Carga.codEntity() }, function (r) {
            if (r.Success) {
                if (r.Data) {
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Documento de transporte vinculado à carga com sucesso.");
                    Global.fecharModal("divVinculoCarga");
                    _gridDocumentoTransporteNatura.CarregarGrid();
                } else {
                    exibirMensagem(tipoMensagem.aviso, "Atenção", r.Msg);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, "Falha", r.Msg);
            }
        });
    });
}

//*******MÉTODOS*******

function AbrirTelaConsultaDocumentoTransporte() {
    LimparCampos(_consultaDocumentoTransporteNatura);

    var modalConsultaDocumentoTransporte = new bootstrap.Modal(document.getElementById("divConsultaDocumentoTransporte"), { backdrop: true, keyboard: true });
    modalConsultaDocumentoTransporte.show();
}

function AbrirTelaVinculoCarga(e, sender) {
    LimparCampos(_vinculoCargaDocumentoTransporteNatura);

    _vinculoCargaDocumentoTransporteNatura.DocumentoTransporte.codEntity(e.Codigo);
    _vinculoCargaDocumentoTransporteNatura.DocumentoTransporte.val(e.Numero);

    Global.abrirModal('divVinculoCarga');
}

function CriarCargaPorDT(e) {
    exibirConfirmacao("Atenção!", "Deseja realmente criar a carga com o documento de transporte " + e.Numero + "?", function () {
        executarReST("DocumentoTransporteNatura/GerarCarga", { DocumentoTransporte: e.Codigo }, function (r) {
            if (r.Success) {
                if (r.Data) {
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Carga " + r.Data.Carga + " criada com sucesso.");
                    _gridDocumentoTransporteNatura.CarregarGrid();
                } else {
                    exibirMensagem(tipoMensagem.aviso, "Atenção!", r.Msg);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, "Falha", r.Msg);
            }
        });
    });
}

function AtualizaDocumentoTransporte(e) {
    exibirConfirmacao("Atenção!", "Deseja realmente atualizar os dados do documento de transporte " + e.Numero + "?", function () {
        executarReST("DocumentoTransporteNatura/AtualizarDocumentoTransporte", { DocumentoTransporte: e.Codigo }, function (r) {
            if (r.Success) {
                if (r.Data) {
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Dados atualizados com sucesso.");
                    _gridDocumentoTransporteNatura.CarregarGrid();
                } else {
                    exibirMensagem(tipoMensagem.aviso, "Atenção!", r.Msg);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, "Falha", r.Msg);
            }
        });
    });
}

function ExibirHistoricoIntegracaoDocumentoTransporte(documentoTransporte) {
    BuscarHistoricoIntegracaoDocumentoTransporte(documentoTransporte);
    Global.abrirModal('divModalHistoricoIntegracaoDocumentoTransporteNatura');
}

function BuscarHistoricoIntegracaoDocumentoTransporte(documentoTransporte) {
    _pesquisaHistoricoIntegracaoDocumentoTransporte = new PesquisaHistoricoIntegracaoDocumentoTransporte();
    _pesquisaHistoricoIntegracaoDocumentoTransporte.Codigo.val(documentoTransporte.Codigo);

    var download = { descricao: "Download Arquivos", id: guid(), metodo: DownloadArquivosHistoricoIntegracaoDocumentoTransporte, tamanho: "8", icone: "" };
    var integracoesRetorno = { descricao: "Integrações de Retorno", id: guid(), metodo: ExibirHistoricoIntegracaoDocumentoTransporteRetorno, tamanho: "8", icone: "" };

    var menuOpcoes = {
        tipo: TypeOptionMenu.list,
        opcoes: [download, integracoesRetorno]
    };

    _gridHistoricoIntegracaoDocumentoTransporte = new GridView("tblHistoricoIntegracaoDocumentoTransporteNatura", "DocumentoTransporteNatura/ConsultarHistoricoIntegracaoDocumentoTransporte", _pesquisaHistoricoIntegracaoDocumentoTransporte, menuOpcoes, { column: 1, dir: orderDir.desc });
    _gridHistoricoIntegracaoDocumentoTransporte.CarregarGrid();
}

function DownloadArquivosHistoricoIntegracaoDocumentoTransporte(integracao) {
    executarDownload("DocumentoTransporteNatura/DownloadArquivosHistoricoIntegracaoDocumentoTransporte", { Codigo: integracao.Codigo });
}

function ExibirHistoricoIntegracaoDocumentoTransporteRetorno(integracao) {
    BuscarHistoricoIntegracaoRetorno(integracao);
    Global.abrirModal('divModalHistoricoIntegracaoRetorno');
}

function BuscarHistoricoIntegracaoRetorno(integracao) {
    _pesquisaHistoricoIntegracaoRetorno = new PesquisaHistoricoIntegracaoDocumentoTransporte();
    _pesquisaHistoricoIntegracaoRetorno.Codigo.val(integracao.Codigo);

    var download = { descricao: "Download Arquivos", id: guid(), metodo: DownloadArquivosHistoricoIntegracaoDocumentoTransporte, tamanho: "15", icone: "" };

    var menuOpcoes = {
        tipo: TypeOptionMenu.link,
        opcoes: [download]
    };

    _gridHistoricoIntegracaoRetorno = new GridView("tblHistoricoIntegracaoRetorno", "DocumentoTransporteNatura/ConsultarIntegracoesRetorno", _pesquisaHistoricoIntegracaoRetorno, menuOpcoes, { column: 1, dir: orderDir.desc });
    _gridHistoricoIntegracaoRetorno.CarregarGrid();
}

function ExibirHistoricoIntegracaoGeral() {
    BuscarHistoricoIntegracaoGeral();

    var modalHistoricoIntegracaoGeralNatura = new bootstrap.Modal(document.getElementById("divModalHistoricoIntegracaoGeralNatura"), { backdrop: true, keyboard: true });
    modalHistoricoIntegracaoGeralNatura.show();
}

function BuscarHistoricoIntegracaoGeral() {
    var download = { descricao: "Download Arquivos", id: guid(), metodo: DownloadArquivosHistoricoIntegracaoDocumentoTransporte, tamanho: "15", icone: "" };
    var integracoesRetorno = { descricao: "Integrações de Retorno", id: guid(), metodo: ExibirHistoricoIntegracaoDocumentoTransporteRetorno, tamanho: "15", icone: "" };

    var menuOpcoes = {
        tipo: TypeOptionMenu.list,
        opcoes: [download, integracoesRetorno]
    };

    _gridHistoricoIntegracaoDocumentoTransporte = new GridView("tblHistoricoIntegracaoGeralNatura", "DocumentoTransporteNatura/ConsultarHistoricoIntegracaoGeral", null, menuOpcoes, { column: 1, dir: orderDir.desc });
    _gridHistoricoIntegracaoDocumentoTransporte.CarregarGrid();
}

function BuscarDocumentosTransporteNatura() {
    var menuOpcoes = new Object();
    menuOpcoes.tipo = TypeOptionMenu.list;
    menuOpcoes.opcoes = new Array();
    menuOpcoes.opcoes.push({ id: guid(), descricao: "Vincular à Carga", metodo: AbrirTelaVinculoCarga, tamanho: "15", icone: "" });
    menuOpcoes.opcoes.push({ id: guid(), descricao: "Criar Carga", metodo: CriarCargaPorDT, tamanho: "15", icone: "" });
    menuOpcoes.opcoes.push({ id: guid(), descricao: "Atualizar DT", metodo: AtualizaDocumentoTransporte, tamanho: "15", icone: "" });
    menuOpcoes.opcoes.push({ id: guid(), descricao: "Histórico de Integrações", metodo: ExibirHistoricoIntegracaoDocumentoTransporte, tamanho: "15", icone: "" });
    menuOpcoes.opcoes.push({ id: guid(), descricao: "Editar DT", metodo: AbrirTelaAlteracaoDTNaturaClick, tamanho: "15", icone: "" });

    var configExportacao = {
        url: "DocumentoTransporteNatura/ExportarPesquisa",
        titulo: "Documentos de Transporte da Natura"
    };

    _gridDocumentoTransporteNatura = new GridView(_pesquisaDocumentoTransporteNatura.Pesquisar.idGrid, "DocumentoTransporteNatura/Pesquisa", _pesquisaDocumentoTransporteNatura, menuOpcoes, null, null, null, null, null, null, null, null, configExportacao);
    _gridDocumentoTransporteNatura.CarregarGrid();
}
