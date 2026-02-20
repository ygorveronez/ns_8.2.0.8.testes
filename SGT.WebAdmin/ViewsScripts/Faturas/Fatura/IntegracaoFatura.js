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
/// <reference path="Fatura.js" />
/// <reference path="EtapaFatura.js" />
/// <reference path="CargaFatura.js" />
/// <reference path="CabecalhoFatura.js" />
/// <reference path="FechamentoFatura.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _tipoConsulta = [
    { text: "Todos", value: -1 },
    { text: "Aguardando Integração", value: 0 },
    { text: "Integrados", value: 1 },
    { text: "Falha na Integração", value: 2 },
    { text: "Aguardando Retorno", value: 3 }
];

var _integracaoFatura;
var _gridArquivosEDI;
var _gridArquivosFatura;
var _HTMLIntegracaoFatura;
var _gridHistoricoIntegracaoFatura;
var _pesquisaHistoricoIntegracaoFatura;
var _modalHistoricoIntegracaoFatura;

var IntegracaoFatura = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });

    this.TipoConsultaEDI = PropertyEntity({ val: ko.observable("-1"), options: _tipoConsulta, text: "Consulta: ", def: "-1", enable: ko.observable(true) });
    this.ConsultarEDI = PropertyEntity({ eventClick: ConsultarEDIClick, type: types.event, text: ko.observable("Consultar"), visible: ko.observable(true), enable: ko.observable(true) });
    this.EnviarEDI = PropertyEntity({ eventClick: EnviarEDIClick, type: types.event, text: ko.observable("Download"), visible: ko.observable(true), enable: ko.observable(true) });
    this.ArquivosEDI = PropertyEntity({ type: types.map, required: false, text: "EDI's", getType: typesKnockout.dynamic, idBtnSearch: guid(), idGrid: guid() });
    this.TotalEDI = PropertyEntity({ getType: typesKnockout.string, text: "Total: ", val: ko.observable(""), visible: true });
    this.AguardandoIntegracaoEDI = PropertyEntity({ getType: typesKnockout.string, text: "Ag. Integração: ", val: ko.observable(""), visible: true });
    this.IntegradoEDI = PropertyEntity({ getType: typesKnockout.string, text: "Integrados: ", val: ko.observable(""), visible: true });
    this.RejeitadoEDI = PropertyEntity({ getType: typesKnockout.string, text: "Rejeitados: ", val: ko.observable(""), visible: true });

    this.TipoConsultaFatura = PropertyEntity({ val: ko.observable("-1"), options: _tipoConsulta, text: "Consulta: ", def: "-1", enable: ko.observable(true) });
    this.ConsultarFatura = PropertyEntity({ eventClick: ConsultarFaturaClick, type: types.event, text: ko.observable("Consultar"), visible: ko.observable(true), enable: ko.observable(true) });
    this.EnviarFatura = PropertyEntity({ eventClick: EnviarFaturaClick, type: types.event, text: ko.observable("Re-enviar todos"), visible: ko.observable(true), enable: ko.observable(true) });
    this.ArquivosFatura = PropertyEntity({ type: types.map, required: false, text: "Fatura's", getType: typesKnockout.dynamic, idBtnSearch: guid(), idGrid: guid() });
    this.TotalFatura = PropertyEntity({ getType: typesKnockout.string, text: "Total: ", val: ko.observable(""), visible: true });
    this.AguardandoIntegracaoFatura = PropertyEntity({ getType: typesKnockout.string, text: "Ag. Integração: ", val: ko.observable(""), visible: true });
    this.IntegradoFatura = PropertyEntity({ getType: typesKnockout.string, text: "Integrados: ", val: ko.observable(""), visible: true });
    this.RejeitadoFatura = PropertyEntity({ getType: typesKnockout.string, text: "Rejeitados: ", val: ko.observable(""), visible: true });
}

var PesquisaHistoricoIntegracaoFatura = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
};

//*******EVENTOS*******

function loadIntegracaoFatura() {
    carregarConteudosHTMLIntegracaoCarga(function () {
        $("#contentIntegracaoFatura").html("");
        let idDiv = guid();
        $("#contentIntegracaoFatura").append(_HTMLIntegracaoFatura.replace(/#divIntegracaoFatura/g, idDiv));
        _integracaoFatura = new IntegracaoFatura();
        KoBindings(_integracaoFatura, idDiv);

        let reenviarEDI = { descricao: "Re-enviar", id: guid(), metodo: ReenviarEDIClick, icone: "", visibilidade: VisibilidadeOpcaoReenviarIntegracaoEDI };
        let downloadEDI = { descricao: "Download", id: guid(), metodo: DownloadEDIClick, icone: "", visibilidade: VisibilidadeOpcaoDownloadIntegracaoEDI };
        let menuOpcoes = { tipo: TypeOptionMenu.list, descricao: "Opções", tamanho: 7, opcoes: [reenviarEDI, downloadEDI] };

        _gridArquivosEDI = new GridView(_integracaoFatura.ArquivosEDI.idGrid, "FaturaIntegracao/PesquisaIntegracaoEDI", _integracaoFatura, menuOpcoes, null, null, null);
        _gridArquivosEDI.CarregarGrid();

        let reenviarFatura = { descricao: "Re-enviar", id: guid(), metodo: ReenviarFaturaClick, icone: "" };
        let historicoIntegracao = { descricao: "Histórico de Envio", id: guid(), metodo: ExibirHistoricoIntegracaoFatura, tamanho: "20", icone: "" };
        let menuOpcoesFatura = { tipo: TypeOptionMenu.list, descricao: "Opções", tamanho: 7, opcoes: [reenviarFatura, historicoIntegracao] };

        _gridArquivosFatura = new GridView(_integracaoFatura.ArquivosFatura.idGrid, "FaturaIntegracao/PesquisaIntegracaoFatura", _integracaoFatura, menuOpcoesFatura, null, null, null);
        _gridArquivosFatura.CarregarGrid();
    });
   // _modalHistoricoIntegracaoFatura = new bootstrap.Modal(document.getElementById("divModalHistoricoIntegracaoFatura"), { backdrop: true, keyboard: true });
}

function VisibilidadeOpcaoDownloadIntegracaoEDI(data) {
    return VerificaSePossuiPermissaoEspecial(EnumPermissaoPersonalizada.Carga_DownloadArquivoIntegracoes, _PermissoesPersonalizadas);
}

function VisibilidadeOpcaoReenviarIntegracaoEDI(data) {
    if (data.Tipo == EnumTipoIntegracao.NaoPossuiIntegracao || !VerificaSePossuiPermissaoEspecial(EnumPermissaoPersonalizada.Fatura_PermiteEnviarEDI, _PermissoesPersonalizadas))
        return false;

    return true;
}

function ReenviarFaturaClick(e, sender) {
    exibirConfirmacao("Atenção!", "Deseja realmente reenviar essa fatura?", function () {
        executarReST("FaturaIntegracao/EnviarLayoutFatura", { Codigo: e.Codigo }, function (arg) {
            if (arg.Success) {
                if (arg.Data) {
                    _gridArquivosFatura.CarregarGrid();
                } else {
                    exibirMensagem(tipoMensagem.aviso, "Atenção", arg.Msg);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
            }
        });
    });
}

function DownloadFaturaClick(e, sender) {
    if (e.Codigo > 0) {
        let data = {
            Codigo: e.Codigo
        };
        executarDownload("FaturaIntegracao/DownloadLayout", data);
    }
}

function ReenviarEDIClick(e, sender) {
    exibirConfirmacao("Atenção!", "Deseja realmente reenviar esse arquivo EDI?", function () {
        if (_fatura.Codigo.val() <= 0) {
            exibirMensagem(tipoMensagem.aviso, "Aviso", "Por favor seleciona uma fatura.");
            return;
        }
        if (_fatura.Situacao.val() == EnumSituacoesFatura.Cancelado) {
            exibirMensagem(tipoMensagem.aviso, "Aviso", "Esta fatura se encontra cancelada.");
            return;
        }
        if (_fatura.Situacao.val() != EnumSituacoesFatura.Fechado && _fatura.Situacao.val() != EnumSituacoesFatura.ProblemaIntegracao) {
            exibirMensagem(tipoMensagem.aviso, "Aviso", "Esta fatura não se encontra finalizada.");
            return;
        }
        if (e.Codigo > 0) {
            var data = {
                Codigo: e.Codigo
            };
            executarReST("FaturaIntegracao/EnviarLayoutEDI", data, function (arg) {
                if (arg.Success) {
                    _gridArquivosEDI.CarregarGrid();
                } else {
                    exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
                }
            });
        }
    });
}

function DownloadEDIClick(e, sender) {
    if (e.Codigo > 0) {
        var data = {
            Codigo: e.Codigo
        };
        executarDownload("FaturaIntegracao/DownloadEDI", data);
    }
}

function ConsultarEDIClick(e, sender) {
    _gridArquivosEDI.CarregarGrid();
}

function ConsultarFaturaClick(e, sender) {
    _gridArquivosFatura.CarregarGrid();
}

function EnviarFaturaClick(e, sender) {
    if (_fatura.Codigo.val() <= 0) {
        exibirMensagem(tipoMensagem.aviso, "Aviso", "Por favor seleciona uma fatura.");
        return;
    }
    if (_fatura.Situacao.val() == EnumSituacoesFatura.Cancelado) {
        exibirMensagem(tipoMensagem.aviso, "Aviso", "Esta fatura se encontra cancelada.");
        return;
    }
    if (_fatura.Situacao.val() != EnumSituacoesFatura.Fechado && _fatura.Situacao.val() != EnumSituacoesFatura.ProblemaIntegracao) {
        exibirMensagem(tipoMensagem.aviso, "Aviso", "Esta fatura não se encontra finalizada.");
        return;
    }
    if (_fatura.Codigo.val() > 0) {
        var data = {
            Codigo: _fatura.Codigo.val()
        };
        executarReST("FaturaIntegracao/EnviarTodosLayoutFatura", data, function (arg) {
            if (arg.Success) {
                _gridArquivosFatura.CarregarGrid();
            } else {
                exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
            }
        });
    }
}

function EnviarEDIClick(e, sender) {
    exibirConfirmacao("Atenção!", "Deseja realmente realizar o download todos os os arquivos EDI desta fatura?", function () {
        if (_fatura.Codigo.val() <= 0) {
            exibirMensagem(tipoMensagem.aviso, "Aviso", "Por favor seleciona uma fatura.");
            return;
        }
        if (_fatura.Situacao.val() == EnumSituacoesFatura.Cancelado) {
            exibirMensagem(tipoMensagem.aviso, "Aviso", "Esta fatura se encontra cancelada.");
            return;
        }
        if (_fatura.Situacao.val() != EnumSituacoesFatura.Fechado && _fatura.Situacao.val() != EnumSituacoesFatura.ProblemaIntegracao) {
            exibirMensagem(tipoMensagem.aviso, "Aviso", "Esta fatura não se encontra finalizada.");
            return;
        }
        if (_fatura.Codigo.val() > 0) {
            var data = {
                Codigo: _fatura.Codigo.val()
            };
            executarDownload("FaturaIntegracao/DownloadTodosEDI", data);
        }
    });
}

//*******MÉTODOS*******

function CarregarIntegracaoFatura() {
    if (_fatura.Codigo.val() > 0 && _fatura.Codigo.val() != "") {
        let data =
            {
                CodigoFatura: _fatura.Codigo.val()
            }
        executarReST("FaturaIntegracao/CarregarDadosTotalizadores", data, function (e) {
            if (e.Success) {
                if (e.Data != null) {
                    var dataIntegracao = { Data: e.Data };
                    PreencherObjetoKnout(_integracaoFatura, dataIntegracao);
                    _gridArquivosFatura.CarregarGrid();
                    _gridArquivosEDI.CarregarGrid();
                }
            }
            else {
                exibirMensagem(tipoMensagem.falha, "Falha", e.Msg);
            }
        });
    }
}

function carregarConteudosHTMLIntegracaoCarga(callback) {
    $.get("Content/Static/Fatura/IntegracaoFatura.html?dyn=" + guid(), function (data) {
        _HTMLIntegracaoFatura = data;
        callback();
    });
}

function LimparInegracaoFatura() {
    LimparCampos(_integracaoFatura);
    _gridArquivosEDI.CarregarGrid();
    _gridArquivosFatura.CarregarGrid();
}

function ExibirHistoricoIntegracaoFatura(integracao) {
    BuscarHistoricoIntegracaoFatura(integracao);
    _modalHistoricoIntegracaoFatura.show();
}

function BuscarHistoricoIntegracaoFatura(integracao) {
    _pesquisaHistoricoIntegracaoFatura = new PesquisaHistoricoIntegracaoFatura();
    _pesquisaHistoricoIntegracaoFatura.Codigo.val(integracao.Codigo);

    let download = { descricao: "Download Arquivos", id: guid(), evento: "onclick", metodo: DownloadArquivosHistoricoIntegracaoFatura, tamanho: "20", icone: "" };

    let menuOpcoes = {
        tipo: TypeOptionMenu.link,
        opcoes: [download]
    };

    _gridHistoricoIntegracaoFatura = new GridView("tblHistoricoIntegracaoFatura", "FaturaIntegracao/ConsultarHistoricoIntegracao", _pesquisaHistoricoIntegracaoFatura, menuOpcoes, { column: 1, dir: orderDir.desc });
    _gridHistoricoIntegracaoFatura.CarregarGrid();
}

function DownloadArquivosHistoricoIntegracaoFatura(historicoConsulta) {
    executarDownload("FaturaIntegracao/DownloadArquivosHistoricoIntegracao", { Codigo: historicoConsulta.Codigo });
}

