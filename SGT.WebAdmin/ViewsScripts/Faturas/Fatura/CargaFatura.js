/// <reference path="../../Consultas/ModeloDocumentoFiscal.js" />
/// <reference path="../../Enumeradores/EnumTipoCTe.js" />
/// <reference path="../../Consultas/Carga.js" />
/// <reference path="Fatura.js" />
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
/// <reference path="../../Configuracao/ConfiguracaoTMS.js" />
/// <reference path="../../Global/Notificacoes/Notificacao.js" />
/// <reference path="../../Global/SignalR/SignalR.js" />
/// <reference path="../../Configuracao/ConfiguracaoTMS.js" />
/// <reference path="../../Consultas/TipoOperacao.js" />
/// <reference path="CabecalhoFatura.js" />
/// <reference path="EtapaFatura.js" />
/// <reference path="FechamentoFatura.js" />
/// <reference path="Fatura.js" />
/// <reference path="IntegracaoFatura.js" />


//*******MAPEAMENTO KNOUCKOUT*******

var _cargaAtual;
var _carga;
var _pesquisaCarga;
var _listaKnoutsCarga;
var _HTMLCargaFatura;
var _resumoCarga;
var _knotSalvarCarga;
var _gridDocumento;
var _pesquisaDocumento;
var _percentualFatura;
var _documentosCarga;
var _gridConhecimentosFatura;
var _modalInformacaoImportacao;

var _tipoDocumento = [{ text: "CT-e", value: 1 },
{ text: "NFS-e", value: 2 }];

var _tipoCTePesquisa = [{ text: "Todos", value: -1 },
{ text: "Normal", value: EnumTipoCTe.Normal },
{ text: "Complementar", value: EnumTipoCTe.Complementar },
{ text: "Anulacao", value: EnumTipoCTe.Anulacao },
{ text: "Substituição", value: EnumTipoCTe.Substituicao }];

var PesquisaCarga = function () {
    this.NumeroCarga = PropertyEntity({ text: "Nº da Carga:", val: ko.observable(""), def: "", visible: ko.observable(true) });
    this.NumeroPedido = PropertyEntity({ text: "Nº do Pedido:", val: ko.observable(""), def: "", visible: ko.observable(true) });
    this.NumeroOcorrencia = PropertyEntity({ text: "Nº da Ocorrência:", val: ko.observable(""), def: "", visible: ko.observable(true) });
    this.NumeroCTe = PropertyEntity({ text: "Nº do CT-e:", val: ko.observable(""), def: "", visible: ko.observable(true) });
    this.NumeroCTeFinal = PropertyEntity({ text: "Nº Final:", val: ko.observable(""), def: "", visible: ko.observable(true) });
    this.ModeloDocumento = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Modelo Documento:", idBtnSearch: guid() });
    this.AliquotaICMS = PropertyEntity({ getType: typesKnockout.decimal, required: false, text: "Alíquota ICMS:", maxlength: 15, enable: ko.observable(true) });
    this.TipoCTe = PropertyEntity({ val: ko.observable(-1), options: _tipoCTePesquisa, def: -1, text: "Tipo do CT-e: ", enable: ko.observable(true) });

    this.Pesquisar = PropertyEntity({
        eventClick: function (e) {
            buscarCargas(1, false);
        }, type: types.event, text: "Pesquisar", idGrid: guid(), visible: ko.observable(true)
    });
}

var DocumentosCarga = function () {
    this.CodigoFatura = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int, idGrid: guid() });
    this.NumeroCarga = PropertyEntity({ text: "Nº da Carga:", val: ko.observable(""), def: "", visible: ko.observable(true) });
    this.NumeroPedido = PropertyEntity({ text: "Nº do Pedido:", val: ko.observable(""), def: "", visible: ko.observable(true) });
    this.NumeroOcorrencia = PropertyEntity({ text: "Nº da Ocorrência:", val: ko.observable(""), def: "", visible: ko.observable(true) });
    this.NumeroDocumento = PropertyEntity({ getType: typesKnockout.text, text: "Nº do CT-e: ", val: ko.observable(""), enable: ko.observable(true) });
    this.NumeroDocumentoFinal = PropertyEntity({ getType: typesKnockout.text, text: "Nº Final: ", val: ko.observable(""), enable: ko.observable(true) });
    this.TipoDocumento = PropertyEntity({ val: ko.observable(1), options: _tipoDocumento, def: 1, text: "Tipo Documento: ", enable: ko.observable(true), visible: ko.observable(false) });
    this.ModeloDocumento = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Modelo Documento:", idBtnSearch: guid() });
    this.ValorDocumento = PropertyEntity({ getType: typesKnockout.decimal, required: false, text: "Valor:", maxlength: 15, enable: ko.observable(true) });
    this.AliquotaICMS = PropertyEntity({ getType: typesKnockout.decimal, required: false, text: "Alíquota ICMS:", maxlength: 15, enable: ko.observable(true) });
    this.TipoCTe = PropertyEntity({ val: ko.observable(-1), options: _tipoCTePesquisa, def: -1, text: "Tipo do CT-e: ", enable: ko.observable(true) });
    this.RemoverRealocarTodos = PropertyEntity({ eventClick: RemoverRealocarTodosClick, type: types.event, text: "Remover / Realocar Documentos", visible: ko.observable(true), enable: ko.observable(true) });

    this.PesquisarDocumento = PropertyEntity({
        eventClick: function (e) {
            _gridConhecimentosFatura.CarregarGrid();
        }, type: types.event, text: "Pesquisar", idGrid: guid(), visible: ko.observable(true)
    });

    this.ConhecimentosFatura = PropertyEntity({ type: types.listEntity, list: new Array(), text: "Conhecimentos da Fatura:", codEntity: ko.observable(0), idBtnSearch: guid(), idGrid: guid() });
}

var CargaFatura = function () {

    //****DADOS CARGA ***

    this.Descricao = PropertyEntity({ type: types.local });
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int, idGrid: guid() });
    this.CodigoFatura = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int, idGrid: guid() });
    this.NumeroCarga = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), required: true, text: "Número da Carga:", idBtnSearch: guid() });
    this.DataCarga = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), required: true, text: "Data da Carga:", idBtnSearch: guid() });
    this.ValorFatura = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), required: true, text: "Valor a Faturar:", idBtnSearch: guid() });
    this.NumeroDocumentos = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), required: true, text: "Numero de Documentos:", idBtnSearch: guid() });
    this.Motorista = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), enable: ko.observable(true), idGrid: guid(), required: true, text: "Motorista:", idBtnSearch: guid(), motoristas: ko.observable("") });
    this.Origem = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), required: true, text: "Origem:", idBtnSearch: guid() });
    this.Destino = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), required: true, text: "Destino:", idBtnSearch: guid() });
    this.Conjunto = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), enable: ko.observable(true), idGrid: guid(), required: true, text: "Conjunto:", idBtnSearch: guid(), conjuntos: ko.observable("") });

    this.DivCarga = PropertyEntity({ type: types.local });

    this.OcultarConteudo = PropertyEntity({ type: types.local, eventClick: MinimizarAbasClick, visible: ko.observable(true) });
    this.EtapaDocumentos = PropertyEntity({
        text: "Documentos", type: types.local, enable: ko.observable(true), visible: ko.observable(true), idGrid: guid(), idTab: guid(), eventClick: DocumentosCargaClick,
        step: ko.observable(2),
        tooltip: ko.observable("Visualize os documentos que estão vinculados para este faturamento."),
        tooltipTitle: ko.observable("Documentos")
    });

    this.NaoFaturarCarga = PropertyEntity({ eventClick: NaoFaturarCargaClick, type: types.event, text: "Não Faturar a Carga", visible: ko.observable(true), enable: ko.observable(true) });
    this.RealocarCarga = PropertyEntity({ eventClick: RealocarCargaClick, type: types.event, text: "Realocar a Carga", visible: ko.observable(true), enable: ko.observable(true) });

    this.NumeroDocumento = PropertyEntity({ getType: typesKnockout.text, text: "Número do documento inicial: ", val: ko.observable(""), enable: ko.observable(true) });
    this.NumeroDocumentoFinal = PropertyEntity({ getType: typesKnockout.text, text: "Nº Final: ", val: ko.observable(""), enable: ko.observable(true) });
    this.TipoDocumento = PropertyEntity({ val: ko.observable(1), options: _tipoDocumento, def: 1, text: "Tipo Documento: ", enable: ko.observable(true), visible: ko.observable(false) });
    this.ModeloDocumento = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Modelo Documento:", idBtnSearch: guid() });
    this.ValorDocumento = PropertyEntity({ getType: typesKnockout.decimal, required: false, text: "Valor:", maxlength: 15, enable: ko.observable(true) });

    this.PesquisarDocumento = PropertyEntity({
        eventClick: function (e) {
            _gridDocumento.CarregarGrid();
        }, type: types.event, text: "Pesquisar", idGrid: guid(), visible: ko.observable(true)
    });
}

var PesquisaDocumento = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int, idGrid: guid() });
    this.CodigoFatura = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int, idGrid: guid() });
    this.NumeroCarga = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int, idGrid: guid() });
    this.NaoFaturarCarga = PropertyEntity({ eventClick: NaoFaturarCargaClick, type: types.event, text: "Não Faturar a Carga", visible: ko.observable(true), enable: ko.observable(true) });
    this.RealocarCarga = PropertyEntity({ eventClick: RealocarCargaClick, type: types.event, text: "Realocar a Carga", visible: ko.observable(true), enable: ko.observable(true) });

    this.NumeroDocumento = PropertyEntity({ getType: typesKnockout.text, text: "Número do documento inicial: ", val: ko.observable(""), enable: ko.observable(true) });
    this.NumeroDocumentoFinal = PropertyEntity({ getType: typesKnockout.text, text: "Nº Final: ", val: ko.observable(""), enable: ko.observable(true) });
    this.TipoDocumento = PropertyEntity({ val: ko.observable(1), options: _tipoDocumento, def: 1, text: "Tipo Documento: ", enable: ko.observable(true), visible: ko.observable(false) });
    this.ModeloDocumento = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Modelo Documento:", idBtnSearch: guid() });
    this.ValorDocumento = PropertyEntity({ getType: typesKnockout.decimal, required: false, text: "Valor:", maxlength: 15, enable: ko.observable(true) });

    this.PesquisarDocumento = PropertyEntity({
        eventClick: function (e) {
            _gridDocumento.CarregarGrid();
        }, type: types.event, text: "Pesquisar", idGrid: guid(), visible: ko.observable(true)
    });
}

var ResumoCargaFatura = function () {
    this.BuscarCargas = PropertyEntity({ type: types.map, required: false, text: "Buscar cargas para o Faturamento", getType: typesKnockout.dynamic, idBtnSearch: guid(), enable: ko.observable(true) });

    this.NumeroDocumento = PropertyEntity({ getType: typesKnockout.int, text: "Nº CT-e: ", val: ko.observable(""), def: ko.observable(""), enable: ko.observable(true) });
    this.AdicionarDocumento = PropertyEntity({ eventClick: AdicionarDocumentoClick, type: types.event, text: "Add CT-e", visible: ko.observable(true), enable: ko.observable(true) });

    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int, idGrid: guid() });
    this.QuantidadeCargas = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), required: true, text: "Número de Cargas:", idBtnSearch: guid() });
    this.QuantidadeCargasFaturadas = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), required: true, text: "Cargas Faturadas:", idBtnSearch: guid() });
    this.QuantidadeCargasParcialmenteFaturadas = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), required: true, text: "Cargas parcialmente Fat.:", idBtnSearch: guid() });
    this.QuantidadeDocumentos = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), required: true, text: "Número de Documentos:", idBtnSearch: guid() });
    this.QuantidadeDocumentosFaturados = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), required: true, text: "Totalmente Faturados:", idBtnSearch: guid() });
    this.QuantidadeDocumentosParcialmenteFaturados = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), required: true, text: "Documentos não Faturados:", idBtnSearch: guid() });
    this.ValorFaturar = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), required: true, text: "Valor a Faturar:", idBtnSearch: guid() });

    this.Arquivo = PropertyEntity({ type: types.file, codEntity: ko.observable(0), text: "Arquivo:", val: ko.observable(""), visible: ko.observable(true), enable: ko.observable(true) });
    this.ImportarPreFatura = PropertyEntity({ eventClick: ImportarPreFaturaClick, type: types.event, text: "Importar de Pré-Fatura", visible: ko.observable(true), enable: ko.observable(true) });
    this.BaixarXMLDocumentosFatura = PropertyEntity({ eventClick: BaixarXMLDocumentosFaturaClick, type: types.event, text: "XML Documentos da Fatura", visible: ko.observable(true), enable: ko.observable(true) });

}

var KnotSalvarCarga = function () {
    this.SalvarCargas = PropertyEntity({ eventClick: SalvarCargasClick, type: types.event, text: "Salvar Cargas", visible: ko.observable(true), enable: ko.observable(true) });
}

var PercentualFatura = function () {
    this.PercentualProcessado = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
}


//*******EVENTOS*******

function loadCarga() {

    _pesquisaCarga = new PesquisaCarga();
    KoBindings(_pesquisaCarga, "knotPesquisa", false, _pesquisaCarga.Pesquisar.id);
    $("#knotPesquisa").show();

    _documentosCarga = new DocumentosCarga();
    KoBindings(_documentosCarga, "knotDocumentosCarga");

    _resumoCarga = new ResumoCargaFatura();
    KoBindings(_resumoCarga, "knotResumoCargas");

    _percentualFatura = new PercentualFatura();
    KoBindings(_percentualFatura, "knotPercentualFatura");

    new BuscarModeloDocumentoFiscal(_documentosCarga.ModeloDocumento);

    new BuscarCargaFinalizadasSemFatura(_resumoCarga.BuscarCargas, RetornoInserirCargaDaFatura, _fatura);

    _knotSalvarCarga = new KnotSalvarCarga();
    KoBindings(_knotSalvarCarga, "knotSalvarCarga");

    carregarConteudosHTMLFatura(function () {
        buscarCargas(1, false);
    });

}

function SalvarCargasClick(e, sender) {
    if (_fatura.Situacao.val() == EnumSituacoesFatura.Fechado) {
        exibirMensagem(tipoMensagem.aviso, "Aviso", "Esta fatura já se encontra finalizada.");
        return;
    }
    if (_fatura.Situacao.val() == EnumSituacoesFatura.Cancelado) {
        exibirMensagem(tipoMensagem.aviso, "Aviso", "Esta fatura se encontra cancelada.");
        return;
    }

    var data = {
        Codigo: _fatura.Codigo.val(),
        Etapa: EnumEtapasFatura.Cargas
    };
    executarReST("FaturaCarga/SalvarCargasFatura", data, function (arg) {
        if (arg.Success) {
            CarregarDadosCabecalho(arg.Data);
            PosicionarEtapa(arg.Data);
        } else {
            exibirMensagem(tipoMensagem.aviso, "Falha", arg.Msg);
        }
    });
}

function BaixarXMLDocumentosFaturaClick(e, sender) {
    if (_fatura.Situacao.val() == EnumSituacoesFatura.Fechado) {
        exibirMensagem(tipoMensagem.aviso, "Aviso", "Esta fatura já se encontra finalizada.");
        return;
    }
    if (_fatura.Situacao.val() == EnumSituacoesFatura.Cancelado) {
        exibirMensagem(tipoMensagem.aviso, "Aviso", "Esta fatura se encontra cancelada.");
        return;
    }

    var data = {
        Codigo: _fatura.Codigo.val()
    };
    executarDownload("FaturaCarga/DownloadLoteXML", data);
}

function RemoverRealocarTodosClick(e, sender) {
    if (_fatura.Situacao.val() == EnumSituacoesFatura.Fechado) {
        exibirMensagem(tipoMensagem.aviso, "Aviso", "Esta fatura já se encontra finalizada.");
        return;
    }
    if (_fatura.Situacao.val() == EnumSituacoesFatura.Cancelado) {
        exibirMensagem(tipoMensagem.aviso, "Aviso", "Esta fatura se encontra cancelada.");
        return;
    }

    exibirConfirmacao("Confirmação", "Realmente deseja remover ou realocar os conhecimentos contidos nesta lista?", function () {
        var data = {
            CodigoFatura: e.CodigoFatura.val(),
            NumeroCarga: e.NumeroCarga.val(),
            NumeroPedido: e.NumeroPedido.val(),
            NumeroDocumento: e.NumeroDocumento.val(),
            NumeroDocumentoFinal: e.NumeroDocumentoFinal.val(),
            TipoDocumento: e.TipoDocumento.val(),
            ValorDocumento: e.ValorDocumento.val(),
            AliquotaICMS: e.AliquotaICMS.val(),
            TipoCTe: e.TipoCTe.val()
        };
        executarReST("FaturaCarga/RemoverRealocarCarga", data, function (arg) {
            if (!arg.Success)
                exibirMensagem(tipoMensagem.aviso, "Falha", arg.Msg);
            else {
                atualizarCargasClick(e, sender);
                BuscarResumoCargaFatura();
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Documento removido/realocados com sucesso.");
            }
        });
    });
}

function AdicionarDocumentoClick(e, sender) {
    if (_fatura.Situacao.val() == EnumSituacoesFatura.Fechado) {
        exibirMensagem(tipoMensagem.aviso, "Aviso", "Esta fatura já se encontra finalizada.");
        return;
    }
    if (_fatura.Situacao.val() == EnumSituacoesFatura.Cancelado) {
        exibirMensagem(tipoMensagem.aviso, "Aviso", "Esta fatura se encontra cancelada.");
        return;
    }

    _resumoCarga.NumeroDocumento.requiredClass("form-control");

    if (_resumoCarga.NumeroDocumento.val() != "" && _resumoCarga.NumeroDocumento.val() != "0") {
        var data = {
            Codigo: _fatura.Codigo.val(),
            Numero: _resumoCarga.NumeroDocumento.val()
        };
        executarReST("FaturaCarga/SalvarConhecimentoCargasFatura", data, function (arg) {
            if (arg.Success) {
                atualizarCargasClick(e, sender);
                BuscarResumoCargaFatura();
                _resumoCarga.NumeroDocumento.val("");
                $("#" + _resumoCarga.NumeroDocumento.id).focus();
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
                $("#" + _resumoCarga.NumeroDocumento.id).focus();
            }
        });
    } else {
        exibirMensagem(tipoMensagem.atencao, "Campos Obrigatórios", "Informe os campos obrigatórios!");
        _resumoCarga.NumeroDocumento.requiredClass("form-control is-invalid");
    }
}

function ImportarPreFaturaClick(e, sender) {
    if (_fatura.Situacao.val() == EnumSituacoesFatura.Fechado) {
        exibirMensagem(tipoMensagem.aviso, "Aviso", "Esta fatura já se encontra finalizada.");
        return;
    }
    if (_fatura.Situacao.val() == EnumSituacoesFatura.Cancelado) {
        exibirMensagem(tipoMensagem.aviso, "Aviso", "Esta fatura se encontra cancelada.");
        return;
    }

    var file = document.getElementById(_resumoCarga.Arquivo.id);

    var formData = new FormData();
    formData.append("upload", file.files[0]);

    _resumoCarga.Arquivo.requiredClass("form-control");

    if (_resumoCarga.Arquivo.val() != "") {
        AtualizarProgressFatura(_fatura.Codigo, 0);

        enviarArquivo("FaturaCarga/ImportarPreFatura?callback=?", { Codigo: _fatura.Codigo.val() }, formData, function (arg) {
            if (arg.Success) {
                if (arg.Msg == "Importação da pré-fatura foi realizada com sucesso.") {
                    exibirMensagem(tipoMensagem.ok, "Sucesso", arg.Msg);
                } else {
                    var data = { Dados: JSON.stringify(arg.Data), Codigo: _fatura.Codigo.val() };
                    executarReST("FaturaCarga/BaixarRelatorio", data, function (arg) {
                        if (arg.Success) {
                            if (arg.Data !== false) {
                                BuscarProcessamentosPendentes();
                                exibirMensagem(tipoMensagem.ok, "Sucesso", "Aguarde que seu relatório está sendo gerado.");
                            } else {
                                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
                            }
                        } else {
                            exibirMensagem(tipoMensagem.aviso, "Falha", arg.Msg);
                        }
                    })

                    $('#contentInformacaoImportacao').html("");
                }

                _resumoCarga.Arquivo.requiredClass("form-control");
                _resumoCarga.Arquivo.val("");

                atualizarCargasClick(e, sender);
                BuscarResumoCargaFatura();
            } else {
                $('#contentInformacaoImportacao').html("");
                $('#contentInformacaoImportacao').html(arg.Msg);
                _modalInformacaoImportacao = new bootstrap.Modal(document.getElementById("divModalInformacaoImportacao"), { backdrop: true, keyboard: true });
                _modalInformacaoImportacao.show();
            }
        });
    } else {
        exibirMensagem(tipoMensagem.atencao, "Campos Obrigatórios", "Informe os campos obrigatórios para a importação!");
        if (_resumoCarga.Arquivo.val() == "")
            _resumoCarga.Arquivo.requiredClass("form-control is-invalid");
    }
}

function RealocarCargaClick(e, sender) {
    if (_fatura.Situacao.val() == EnumSituacoesFatura.Fechado) {
        exibirMensagem(tipoMensagem.aviso, "Aviso", "Esta fatura já se encontra finalizada.");
        return;
    }
    if (_fatura.Situacao.val() == EnumSituacoesFatura.Cancelado) {
        exibirMensagem(tipoMensagem.aviso, "Aviso", "Esta fatura se encontra cancelada.");
        return;
    }

    exibirConfirmacao("Confirmação", "Realmente deseja realmente realocar a carga " + e.NumeroCarga.val() + "?", function () {
        var data = {
            CodigoCarga: e.Codigo.val(),
            CodigoFatura: e.CodigoFatura.val()
        };
        executarReST("FaturaCarga/RealocarCarga", data, function (arg) {
            if (!arg.Success)
                exibirMensagem(tipoMensagem.aviso, "Falha", arg.Msg);
            else {
                atualizarCargasClick(e, sender);
                BuscarResumoCargaFatura();
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Documento removido com sucesso.");
            }
        });
    });
}

function NaoFaturarCargaClick(e, sender) {
    if (_fatura.Situacao.val() == EnumSituacoesFatura.Fechado) {
        exibirMensagem(tipoMensagem.aviso, "Aviso", "Esta fatura já se encontra finalizada.");
        return;
    }
    if (_fatura.Situacao.val() == EnumSituacoesFatura.Cancelado) {
        exibirMensagem(tipoMensagem.aviso, "Aviso", "Esta fatura se encontra cancelada.");
        return;
    }

    exibirConfirmacao("Confirmação", "Realmente deseja realmente remover a carga " + e.NumeroCarga.val() + "?", function () {
        var data = {
            CodigoCarga: e.Codigo.val(),
            CodigoFatura: e.CodigoFatura.val()
        };
        executarReST("FaturaCarga/RemoverCarga", data, function (arg) {
            if (!arg.Success)
                exibirMensagem(tipoMensagem.aviso, "Falha", arg.Msg);
            else {
                atualizarCargasClick(e, sender);
                BuscarResumoCargaFatura();
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Documento removido com sucesso.");
            }
        });
    });
}

function atualizarCargasClick(e, sender) {
    LimparCampos(_pesquisaCarga);
    buscarCargas(1, false);
    buscarConhecimentosFatura();
}

function DocumentosCargaClick(e, sender) {
    var idDivCarga = "knoutPesquisaDocumento" + e.DivCarga.id;
    _pesquisaDocumento = new PesquisaDocumento();
    KoBindings(_pesquisaDocumento, idDivCarga);
    _pesquisaDocumento.Codigo.val(e.Codigo.val());
    _pesquisaDocumento.CodigoFatura.val(_fatura.Codigo.val());
    _pesquisaDocumento.NumeroCarga.val(e.NumeroCarga.val());
    VerificarBotoes();
    new BuscarModeloDocumentoFiscal(_pesquisaDocumento.ModeloDocumento);

    var realocarDocumento = { descricao: "Realocar", id: guid(), metodo: RealocarDocumento, icone: "" };
    var removerDocumento = { descricao: "Remover", id: guid(), metodo: RemoverDocumento, icone: "" };
    var detalheDocumento = { descricao: "Detalhe", id: guid(), metodo: DetalheDocumento, icone: "" };
    var menuOpcoes = { tipo: TypeOptionMenu.list, descricao: "Opções", tamanho: 7, opcoes: [removerDocumento, realocarDocumento, detalheDocumento] };
    _gridDocumento = new GridView(_pesquisaDocumento.PesquisarDocumento.idGrid, "FaturaCarga/PesquisaDocumentosCarga", _pesquisaDocumento, menuOpcoes, null, null, null);
    _gridDocumento.CarregarGrid();
}

function RealocarDocumento(e, sender) {
    if (_fatura.Situacao.val() == EnumSituacoesFatura.Fechado) {
        exibirMensagem(tipoMensagem.aviso, "Aviso", "Esta fatura já se encontra finalizada.");
        return;
    }
    if (_fatura.Situacao.val() == EnumSituacoesFatura.Cancelado) {
        exibirMensagem(tipoMensagem.aviso, "Aviso", "Esta fatura se encontra cancelada.");
        return;
    }

    exibirConfirmacao("Confirmação", "Realmente deseja realmente realocar o documento " + e.Numero + "?", function () {
        var data = {
            CodigoCarga: e.CodigoCarga,//_pesquisaDocumento.Codigo.val(),
            CodigoFatura: e.CodigoFatura,//_pesquisaDocumento.CodigoFatura.val(),
            CodigoConhecimento: e.Codigo
        };
        executarReST("FaturaCarga/RealocarConhecimento", data, function (arg) {
            if (!arg.Success)
                exibirMensagem(tipoMensagem.aviso, "Falha", arg.Msg);
            else {
                atualizarCargasClick(e, sender);
                BuscarResumoCargaFatura();
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Documento removido com sucesso.");
            }
        });
    });
}

function RemoverDocumento(e, sender) {
    if (_fatura.Situacao.val() == EnumSituacoesFatura.Fechado) {
        exibirMensagem(tipoMensagem.aviso, "Aviso", "Esta fatura já se encontra finalizada.");
        return;
    }
    if (_fatura.Situacao.val() == EnumSituacoesFatura.Cancelado) {
        exibirMensagem(tipoMensagem.aviso, "Aviso", "Esta fatura se encontra cancelada.");
        return;
    }

    if (e.Fatura != "") {
        exibirConfirmacao("Confirmação", "Realmente deseja realmente remover o documento " + e.Numero + "?", function () {
            var data = {
                CodigoCarga: e.CodigoCarga,//_pesquisaDocumento.Codigo.val(),
                CodigoFatura: e.CodigoFatura,//_pesquisaDocumento.CodigoFatura.val(),
                CodigoConhecimento: e.Codigo
            };
            executarReST("FaturaCarga/RemoverConhecimento", data, function (arg) {
                if (!arg.Success)
                    exibirMensagem(tipoMensagem.aviso, "Falha", arg.Msg);
                else {
                    atualizarCargasClick(e, sender);
                    BuscarResumoCargaFatura();
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Documento removido com sucesso.");
                }
            });
        });
    } else
        exibirMensagem(tipoMensagem.atencao, "Atenção", "Este conhecimento já está removido da fatura.");
}

function DetalheDocumento(e, sender) {
    var data = { CodigoCTe: e.Codigo, CodigoEmpresa: 0 };
    executarDownload("CargaCTe/DownloadDacte", data);
}

//*******MÉTODOS*******

function RetornoInserirCargaDaFatura(data) {
    if (data != null) {
        var data =
            {
                CodigoFatura: _fatura.Codigo.val(),
                CodigoCarga: data.Codigo
            }
        executarReST("FaturaCarga/IniserirNovaCarga", data, function (e) {
            if (e.Success) {
                atualizarCargasClick();
                BuscarResumoCargaFatura();
            }
            else {
                exibirMensagem(tipoMensagem.aviso, "falha", e.Msg);
            }
        });
    }
}

function scrollToAnchor(aid) {
    var aTag = $("#" + aid);
    $('html,body').animate({ scrollTop: aTag.offset().top }, 'slow');
}

function ocultarAbas(e) {
    //$("div .modal-content").attr("class", "well modal-content");
    //$("#" + e.DivCarga.id).attr("class", "p-2");
    //$("#" + e.DivCarga.id + " .tab-pane.active").attr("class", "tab-pane");
    //$("#" + e.DivCarga.id + " li.active").removeAttr("class");
    $("#fdsFaturaCargas .tab-pane.active").attr("class", "tab-pane");
    $("#fdsFaturaCargas li.active").removeAttr("class");
}

function ocultarTodasAbas(e) {
    ocultarAbas(e);
    scrollToAnchor(e.DivCarga.id);
    $(".iconeMinimizarCargas").hide();
    $("#" + e.OcultarConteudo.id).show();
}

function MinimizarAbasClick(e) {
    ocultarAbas(e)
    //$("#" + e.OcultarConteudo.id).hide();
}

function buscarCargas(page, paginou, callback) {

    var itensPorPagina = 10;
    var data =
        {
            CodigoFatura: _fatura.Codigo.val(),
            inicio: itensPorPagina * (page - 1),
            limite: itensPorPagina,
            NumeroCarga: _pesquisaCarga.NumeroCarga.val(),
            NumeroCTe: _pesquisaCarga.NumeroCTe.val(),
            NumeroCTeFinal: _pesquisaCarga.NumeroCTeFinal.val(),
            NumeroPedido: _pesquisaCarga.NumeroPedido.val(),
            NumeroOcorrencia: _pesquisaCarga.NumeroOcorrencia.val(),
            TipoCTe: _pesquisaCarga.TipoCTe.val()
        }

    executarReST("FaturaCarga/PesquisaCargas", data, function (e) {
        if (e.Success) {
            _listaKnoutsCarga = new Array();
            $("#fdsFaturaCargas").html("");
            $.each(e.Data, function (i, carga) {
                var knoutCarga = GerarTagHTMLDaCargaFatura("fdsFaturaCargas", carga);
                _listaKnoutsCarga.push(knoutCarga)
            });

            if (!paginou) {
                if (e.QuantidadeRegistros > 0) {
                    $("#divPagination").html('<ul style="float:right" id="paginacao" class="pagination"></ul>');
                    var paginas = Math.ceil((e.QuantidadeRegistros / 10));
                    $('#paginacao').twbsPagination({
                        first: 'Primeiro',
                        prev: 'Anterior',
                        next: 'Próximo',
                        last: 'Último',
                        totalPages: paginas,
                        visiblePages: 5,
                        onPageClick: function (event, page) {
                            buscarCargas(page, true);
                        }
                    });
                } else {
                    $("#divPagination").html('<span>Nenhum Registro Encontrado</span>');
                }
            }
        }
        else {
            exibirMensagem(tipoMensagem.aviso, "falha", e.Msg);
        }
    });

}

function preencherDadosCargaFatura(knoutCarga, carga) {
    var idDivCarga = knoutCarga.DivCarga.id;
    IniciarBindKnoutCargaFatura(knoutCarga, carga);

    var html = _HTMLCargaFatura.replace(/#idDivCarga/g, idDivCarga)
        .replace(/#knoutPesquisaDocumento/g, "knoutPesquisaDocumento" + idDivCarga)
        .replace(/tabDocumento/g, "tabDocumento" + idDivCarga);

    $("#" + "conteudo_" + idDivCarga).html(html);
    KoBindings(knoutCarga, idDivCarga);

    if (carga.SituacaoCarga == 1)
        $("#" + idDivCarga).css("background-color", "#008000");
    else if (carga.SituacaoCarga == 2)
        $("#" + idDivCarga).css("background-color", "#FFD700");
    else
        $("#" + idDivCarga).css("background-color", "#FF0000");

    $("[rel=popover]").popover();
    $("[rel=popover-hover]").popover({ trigger: "hover" });
}

function IniciarBindKnoutCargaFatura(knoutCarga, carga) {

    knoutCarga.CodigoFatura.val(_fatura.Codigo.val());
    knoutCarga.Codigo.val(carga.Codigo);
    knoutCarga.NumeroCarga.val(carga.NumeroCarga);
    knoutCarga.DataCarga.val(carga.DataCarga);
    knoutCarga.ValorFatura.val(Globalize.format(carga.ValorFatura, "n2"));
    knoutCarga.NumeroDocumentos.val(carga.NumeroDocumentos);
    knoutCarga.Origem.val(carga.Origem);
    knoutCarga.Destino.val(carga.Destino);

    if (carga.Motoristas != null) {
        if (carga.Motoristas.length > 0) {
            knoutCarga.Motorista.val(carga.Motoristas[0].Descricao);
            knoutCarga.Motorista.codEntity(carga.Motoristas[0].Codigo);
            knoutCarga.Motorista.motoristas(carga.Motoristas[0].Descricao);

            if (carga.Motoristas.length > 1) {
                var motoristas = carga.Motoristas[1].Descricao;
                for (var i = 2; i < carga.Motoristas.length; i++) {
                    motoristas = motoristas + " / " + carga.Motoristas[i].Descricao;
                }
                knoutCarga.Motorista.motoristas(carga.Motoristas[0].Descricao + " / " + motoristas);
            }
        }
    } else {
        knoutCarga.Motorista.val("");
        knoutCarga.Motorista.codEntity(0);
        knoutCarga.Motorista.motoristas("");
    }

    knoutCarga.Conjunto.val(carga.Veiculo.Descricao);
    knoutCarga.Conjunto.codEntity(carga.Veiculo.Codigo);
    knoutCarga.Conjunto.conjuntos(carga.Veiculo.Descricao);

    if (carga.Conjuntos != null) {
        if (carga.Conjuntos.length > 0) {
            var placas = carga.Conjuntos[0].Descricao;
            for (var i = 1; i < carga.Conjuntos.length; i++) {
                placas = placas + " / " + carga.Conjuntos[i].Descricao;
            }
            knoutCarga.Conjunto.conjuntos(carga.Veiculo.Descricao + " / " + placas);
        }
    } else
        knoutCarga.Conjunto.conjuntos("");
}

//*******MÉTODOS COMPARTILHADOS*******

function buscarConhecimentosFatura() {
    _documentosCarga.CodigoFatura.val(_fatura.Codigo.val());

    var realocarDocumento = { descricao: "Realocar", id: guid(), metodo: RealocarDocumento, icone: "" };
    var removerDocumento = { descricao: "Remover", id: guid(), metodo: RemoverDocumento, icone: "" };
    var detalheDocumento = { descricao: "Detalhe", id: guid(), metodo: DetalheDocumento, icone: "" };
    var menuOpcoes = { tipo: TypeOptionMenu.list, descricao: "Opções", tamanho: 7, opcoes: [removerDocumento, realocarDocumento, detalheDocumento] };

    _gridConhecimentosFatura = new GridView(_documentosCarga.ConhecimentosFatura.idGrid, "FaturaCarga/PesquisaConhecimentosCargaFatura", _documentosCarga, menuOpcoes, null, 20, null);
    _gridConhecimentosFatura.CarregarGrid();
}

function AtualizarProgressFatura(codigo, percentual) {
    if (_fatura.Codigo.val() == codigo) {
        if (_fatura.NovoModelo.val()) {
            AtualizarProgressFaturaNovo(codigo, percentual);
        } else {
            SetarPercentualProcessamento(percentual);
            $("#fdsCargasDaFatura").hide();
            $("#fdsPercentualCargasDocumento").show();
            if (_knotSalvarCarga != null) {
                _knotSalvarCarga.SalvarCargas.enable(false);
            }
        }
    }
}


function SetarPercentualProcessamento(percentual) {
    finalizarRequisicao();
    var strPercentual = parseInt(percentual) + "%";
    _percentualFatura.PercentualProcessado.val(strPercentual);
    $("#" + _percentualFatura.PercentualProcessado.id).css("width", strPercentual)
}


function VerificarSeFaturaNotificadaEstaSelecionada(codigoFatura) {
    if (_fatura.Codigo.val() == codigoFatura) {

        if (_fatura.NovoModelo.val()) {
            $("#knockoutFaturaDocumento").show();
            $("#knockoutPercentualFaturaDocumento").hide();

            if (_crudDocumentoFatura != null)
                _crudDocumentoFatura.ConfirmarDocumentosFatura.visible(true);

            SetarPercentualProcessamentoFaturaNovo(0);
        } else {
            $("#fdsCargasDaFatura").show();
            $("#fdsPercentualCargasDocumento").hide();
            if (_knotSalvarCarga != null) {
                _knotSalvarCarga.SalvarCargas.enable(true);
            }

            SetarPercentualProcessamento(0);
        }

        limparCamposFatura();
        _fatura.Codigo.val(codigoFatura);
        BuscarPorCodigo(_fatura, "Fatura/BuscarPorCodigo", function (arg) {
            _pesquisaFatura.ExibirFiltros.visibleFade(false);

            TipoPessoaChange();
            CarregarDadosCabecalho(arg.Data);
            PosicionarEtapa(arg.Data);

            _fatura.CancelarFatura.visible(true);
            $("#knockoutCabecalhoFatura").show();
        }, null);
    }
}

function BuscarResumoCargaFatura() {
    if (_resumoCarga != null) {
        _resumoCarga.Arquivo.requiredClass("form-control");
        _resumoCarga.Arquivo.val("");
    }
    var data = { Codigo: _fatura.Codigo.val() };
    if (data.Codigo > 0) {
        executarReST("FaturaCarga/ObterResumoCargas", data, function (arg) {
            if (arg.Success) {
                if (arg.Data != null) {
                    var dataResumo = { Data: arg.Data };
                    PreencherObjetoKnout(_resumoCarga, dataResumo);
                    _resumoCarga.QuantidadeCargas.val(dataResumo.Data.QuantidadeCargas);
                    _resumoCarga.QuantidadeCargasFaturadas.val(dataResumo.Data.QuantidadeCargasFaturadas);
                    _resumoCarga.QuantidadeCargasParcialmenteFaturadas.val(dataResumo.Data.QuantidadeCargasParcialmenteFaturadas);
                    _resumoCarga.QuantidadeDocumentos.val(dataResumo.Data.QuantidadeDocumentos);
                    _resumoCarga.QuantidadeDocumentosFaturados.val(dataResumo.Data.QuantidadeDocumentosFaturados);
                    _resumoCarga.QuantidadeDocumentosParcialmenteFaturados.val(dataResumo.Data.QuantidadeDocumentosParcialmenteFaturados);
                    _resumoCarga.ValorFaturar.val(dataResumo.Data.ValorFaturar);
                }
            } else {
                exibirMensagem(tipoMensagem.aviso, "Falha", arg.Msg);
            }
        });
        buscarConhecimentosFatura();
    }
}

function GerarTagHTMLDaCargaFatura(idElemento, carga) {
    var knoutCarga = new CargaFatura();
    knoutCarga.DivCarga.id = knoutCarga.Codigo.idGrid;
    new BuscarModeloDocumentoFiscal(knoutCarga.ModeloDocumento);

    var html = "<div id='conteudo_" + knoutCarga.DivCarga.id + "'></div>";
    $("#" + idElemento).append(html);
    preencherDadosCargaFatura(knoutCarga, carga);
    ocultarAbas(knoutCarga);
    return knoutCarga;
}

function carregarConteudosHTMLFatura(callback) {
    $.get("Content/Static/Fatura/CargaFatura.html?dyn=" + guid(), function (data) {
        _HTMLCargaFatura = data;
        callback();
    });
}

function LimparCargaFatura() {
    LimparCampos(_resumoCarga);
    $("#myTabContentCarga a:eq(0)").tab("show");
}