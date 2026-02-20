//*******MAPEAMENTO KNOUCKOUT*******
var _pesquisaHistoricoIntegracaoPreFatura;
var _gridHistoricoIntegracaoPreFatura;
var _gridDocumentosPreFatura;
var _gridPreFaturaNatura;
var _pesquisaPreFaturaNatura;
var _consultaPreFaturaNatura;
var _pesquisaDocumentosPreFatura;

var _situacaoPreFatura = [
    { value: "", text: "Todas" },
    { value: EnumSituacaoPreFaturaNatura.Pendente, text: "Pendente" },
    { value: EnumSituacaoPreFaturaNatura.Gerada, text: "Gerada" },
    { value: EnumSituacaoPreFaturaNatura.Falha, text: "Falha" },
    { value: EnumSituacaoPreFaturaNatura.Atualizando, text: "Atualizando" }
];

var VinculoCarga = function () {
    this.PreFatura = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), val: ko.observable(""), idBtnSearch: guid() });
    this.Carga = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Carga: ", idBtnSearch: guid() });

    this.Vincular = PropertyEntity({ eventClick: VincularCargaClick, type: types.event, text: "Vincular", visible: ko.observable(true) });
};

var PesquisaDocumentosPreFatura = function () {
    this.Codigo = PropertyEntity({ getType: typesKnockout.int, val: ko.observable(0), def: 0 });
    this.Numero = PropertyEntity({ val: ko.observable(""), def: "", getType: typesKnockout.int, text: "Número:" });

    this.Pesquisar = PropertyEntity({ eventClick: function () { _gridDocumentosPreFatura.CarregarGrid(); }, type: types.event, text: "Pesquisa", visible: ko.observable(true) });
    this.RemoverDocumentosCancelados = PropertyEntity({ eventClick: RemoverDocumentosCanceladosClick, type: types.event, text: "Remover Docs. Cancelados/Anulados", visible: ko.observable(true) });
};


var ConsultaPreFaturaNatura = function () {
    var dataAtual = moment().format("DD/MM/YYYY");

    this.NumeroPreFatura = PropertyEntity({ text: "Número da Pré Fatura:", def: "", val: ko.observable(""), getType: typesKnockout.int, maxlength: 15 });
    this.DataInicial = PropertyEntity({ text: "Data Inicial:", getType: typesKnockout.date, val: ko.observable(dataAtual), def: dataAtual });
    this.DataFinal = PropertyEntity({ text: "Data Final:", getType: typesKnockout.date, val: ko.observable(dataAtual), def: dataAtual });
    this.Empresa = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "*Empresa/Filial:", idBtnSearch: guid(), issue: 0, visible: ko.observable(true), required: true });

    this.DataInicial.dateRangeLimit = this.DataFinal;
    this.DataFinal.dateRangeInit = this.DataInicial;

    this.ConsultarPreFatura = PropertyEntity({ eventClick: ConsultarPreFaturaClick, type: types.event, text: "Consultar", visible: ko.observable(true) });
};

var PesquisaPreFaturaNatura = function () {

    this.NumeroPreFatura = PropertyEntity({ text: "Número da Pré Fatura:", def: "", val: ko.observable(""), getType: typesKnockout.int, maxlength: 15 });
    this.DataInicial = PropertyEntity({ text: "Data Inicial:", getType: typesKnockout.date });
    this.DataFinal = PropertyEntity({ text: "Data Final:", getType: typesKnockout.date });
    this.Empresa = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Empresa/Filial:", idBtnSearch: guid() });
    this.Situacao = PropertyEntity({ val: ko.observable(""), options: _situacaoPreFatura, def: "", text: "Situação: " });

    this.DataInicial.dateRangeLimit = this.DataFinal;
    this.DataFinal.dateRangeInit = this.DataInicial;

    this.Pesquisar = PropertyEntity({
        eventClick: function (e) {
            _gridPreFaturaNatura.CarregarGrid();
        }, type: types.event, text: "Pesquisar", idGrid: guid(), visible: ko.observable(true)
    });

    this.ConsultarPreFatura = PropertyEntity({ eventClick: AbrirTelaConsultaPreFatura, type: types.event, text: "Consultar Pré Fatura", visible: ko.observable(true) });

    this.HistoricoIntegracoes = PropertyEntity({ eventClick: ExibirHistoricoIntegracaoGeral, type: types.event, text: "Histórico de Integrações", visible: ko.observable(true) });
};

var PesquisaHistoricoIntegracaoPreFatura = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
};

//*******EVENTOS*******

function LoadPreFaturaNatura() {
    _pesquisaPreFaturaNatura = new PesquisaPreFaturaNatura();
    KoBindings(_pesquisaPreFaturaNatura, "knockoutPreFaturaNatura", _pesquisaPreFaturaNatura.Pesquisar.id);

    _consultaPreFaturaNatura = new ConsultaPreFaturaNatura();
    KoBindings(_consultaPreFaturaNatura, "divConsultaPreFatura");

    _pesquisaDocumentosPreFatura = new PesquisaDocumentosPreFatura();
    KoBindings(_pesquisaDocumentosPreFatura, "divModalDocumentosPreFaturaNatura", _pesquisaDocumentosPreFatura.Pesquisar.id);

    LoadDocumentosPreFatura();

    HeaderAuditoria("PreFaturaNatura", {});

    new BuscarTransportadores(_pesquisaPreFaturaNatura.Empresa);
    new BuscarTransportadores(_consultaPreFaturaNatura.Empresa);

    BuscarPreFaturasNatura();
}

function ConsultarPreFaturaClick(e, sender) {
    var numeroDT = Globalize.parseInt(_consultaPreFaturaNatura.NumeroPreFatura.val());

    if (isNaN(numeroDT) || numeroDT <= 0) {
        if (string.IsNullOrWhiteSpace(_consultaPreFaturaNatura.DataInicial.val()) || string.IsNullOrWhiteSpace(_consultaPreFaturaNatura.DataFinal.val())) {
            exibirMensagem(tipoMensagem.atencao, "Atenção", "É necessário informar o número da pré fatura ou um período para realizar a consulta.");
            return;
        }
    }

    Salvar(_consultaPreFaturaNatura, "PreFaturaNatura/ConsultarPreFatura", function(r) {
        if (r.Success) {
            if (r.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Pré faturas consultadas com sucesso!");
                _gridPreFaturaNatura.CarregarGrid();
                Global.fecharModal("divConsultaPreFatura");
            } else {
                exibirMensagem(tipoMensagem.aviso, "Atenção", r.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", r.Msg);
        }
    });
}

//*******MÉTODOS*******

function AbrirTelaConsultaPreFatura() {
    LimparCampos(_consultaPreFaturaNatura);

    Global.abrirModal("divConsultaPreFatura");
}

function AtualizaPreFatura(e) {
    exibirConfirmacao("Atenção!", "Deseja realmente atualizar os dados da pré fatura " + e.NumeroPreFatura + "?", function() {
        executarReST("PreFaturaNatura/AtualizarPreFatura", { Codigo: e.Codigo }, function(r) {
            if (r.Success) {
                if (r.Data) {
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Dados atualizados com sucesso.");
                    _gridPreFaturaNatura.CarregarGrid();
                } else {
                    exibirMensagem(tipoMensagem.aviso, "Atenção!", r.Msg);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, "Falha", r.Msg);
            }
        });
    });
}

function GerarFaturaNatura(e) {
    exibirConfirmacao("Atenção!", "Deseja realmente gerar a fatura da pré fatura " + e.NumeroPreFatura + "?", function() {
        executarReST("PreFaturaNatura/GerarFatura", { Codigo: e.Codigo }, function(r) {
            if (r.Success) {
                if (r.Data) {
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Fatura gerada com sucesso.");
                    _gridPreFaturaNatura.CarregarGrid();
                } else {
                    exibirMensagem(tipoMensagem.aviso, "Atenção!", r.Msg);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, "Falha", r.Msg);
            }
        });
    });
}

function ExibirHistoricoIntegracaoPreFatura(preFatura) {
    BuscarHistoricoIntegracaoPreFatura(preFatura);
    Global.abrirModal("divModalHistoricoIntegracaoPreFaturaNatura");
}

function BuscarHistoricoIntegracaoPreFatura(preFatura) {
    _pesquisaHistoricoIntegracaoPreFatura = new PesquisaHistoricoIntegracaoPreFatura();
    _pesquisaHistoricoIntegracaoPreFatura.Codigo.val(preFatura.Codigo);

    var download = { descricao: "Download Arquivos", id: guid(), metodo: DownloadArquivosHistoricoIntegracaoPreFatura, tamanho: "15", icone: "" };

    var menuOpcoes = {
        tipo: TypeOptionMenu.list,
        opcoes: [download]
    };

    _gridHistoricoIntegracaoPreFatura = new GridView("tblHistoricoIntegracaoPreFaturaNatura", "PreFaturaNatura/ConsultarHistoricoIntegracaoPreFatura", _pesquisaHistoricoIntegracaoPreFatura, menuOpcoes, { column: 1, dir: orderDir.desc });
    _gridHistoricoIntegracaoPreFatura.CarregarGrid();
}

function DownloadArquivosHistoricoIntegracaoPreFatura(integracao) {
    executarDownload("PreFaturaNatura/DownloadArquivosHistoricoIntegracaoPreFatura", { Codigo: integracao.Codigo });
}

function ExibirHistoricoIntegracaoGeral() {
    BuscarHistoricoIntegracaoGeral();
    Global.abrirModal('divModalHistoricoIntegracaoGeralNatura');
}

function BuscarHistoricoIntegracaoGeral() {
    var download = { descricao: "Download Arquivos", id: guid(), metodo: DownloadArquivosHistoricoIntegracaoPreFatura, tamanho: "15", icone: "" };

    var menuOpcoes = {
        tipo: TypeOptionMenu.list,
        opcoes: [download]
    };

    _gridHistoricoIntegracaoPreFatura = new GridView("tblHistoricoIntegracaoGeralNatura", "PreFaturaNatura/ConsultarHistoricoIntegracaoGeral", null, menuOpcoes, { column: 1, dir: orderDir.desc });
    _gridHistoricoIntegracaoPreFatura.CarregarGrid();
}

function BuscarPreFaturasNatura() {
    var menuOpcoes = new Object();
    menuOpcoes.tipo = TypeOptionMenu.list;
    menuOpcoes.opcoes = new Array();
    menuOpcoes.opcoes.push({ id: guid(), descricao: "Atualizar Pré Fatura", metodo: AtualizaPreFatura, tamanho: "15", icone: "" });
    menuOpcoes.opcoes.push({ id: guid(), descricao: "Gerar Fatura", metodo: GerarFaturaNatura, tamanho: "15", icone: "" });
    menuOpcoes.opcoes.push({ id: guid(), descricao: "Documentos", metodo: AbrirTelaDocumentosPreFatura, tamanho: "15", icone: "" });
    menuOpcoes.opcoes.push({ id: guid(), descricao: "Histórico de Integrações", metodo: ExibirHistoricoIntegracaoPreFatura, tamanho: "15", icone: "" });

    _gridPreFaturaNatura = new GridView(_pesquisaPreFaturaNatura.Pesquisar.idGrid, "PreFaturaNatura/Pesquisa", _pesquisaPreFaturaNatura, menuOpcoes, null);
    _gridPreFaturaNatura.CarregarGrid();
}

function AbrirTelaDocumentosPreFatura(e) {
    LimparCampos(_pesquisaDocumentosPreFatura);
    _pesquisaDocumentosPreFatura.Codigo.val(e.Codigo);
    _gridDocumentosPreFatura.CarregarGrid();
    Global.abrirModal("divModalDocumentosPreFaturaNatura");
}

function LoadDocumentosPreFatura() {
    var downloadDACTE = { descricao: "Baixar DACTE", id: guid(), metodo: DownloadDACTE, tamanho: "15", icone: "" };
    var downloadXML = { descricao: "Baixar XML", id: guid(), metodo: DownloadXML, tamanho: "15", icone: "" };
    var excluirDocumento = { descricao: "Excluir", id: guid(), metodo: ExcluirDocumentoPreFaturaNatura, tamanho: "15", icone: "" };

    var menuOpcoes = {
        tipo: TypeOptionMenu.list,
        opcoes: [downloadDACTE, downloadXML, excluirDocumento]
    };

    _gridDocumentosPreFatura = new GridView("tblDocumentosPreFaturaNatura", "PreFaturaNatura/ConsultarDocumentos", _pesquisaDocumentosPreFatura, menuOpcoes, { column: 2, dir: orderDir.asc });
}

function RemoverDocumentosCanceladosClick() {
    exibirConfirmacao("Atenção!", "Deseja realmente excluir os documentos cancelados e anulados da pré-fatura?", function () {
        executarReST("PreFaturaNatura/ExcluirDocumentosCancelados", { Codigo: _pesquisaDocumentosPreFatura.Codigo.val() }, function (r) {
            if (r.Success) {
                if (r.Data) {
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Documentos cancelados e anulados excluídos com sucesso.");
                    _gridDocumentosPreFatura.CarregarGrid();
                } else {
                    exibirMensagem(tipoMensagem.aviso, "Atenção!", r.Msg);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, "Falha", r.Msg);
            }
        });
    });
}

function DownloadDACTE(e) {
    executarDownload("CargaCTe/DownloadDacte", { CodigoCTe: e.Codigo });
}

function DownloadXML(e) {
    executarDownload("CargaCTe/DownloadXML", { CodigoCTe: e.Codigo });
}

function ExcluirDocumentoPreFaturaNatura(e) {
    exibirConfirmacao("Atenção!", "Deseja realmente excluir o documento " + e.Numero + " da pré-fatura?", function() {
        executarReST("PreFaturaNatura/ExcluirDocumento", { Codigo: e.CodigoItem }, function(r) {
            if (r.Success) {
                if (r.Data) {
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Documento excluído com sucesso.");
                    _gridDocumentosPreFatura.CarregarGrid();
                } else {
                    exibirMensagem(tipoMensagem.aviso, "Atenção!", r.Msg);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, "Falha", r.Msg);
            }
        });
    });
}