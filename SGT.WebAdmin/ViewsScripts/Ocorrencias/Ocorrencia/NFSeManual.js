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
/// <reference path="Ocorrencia.js" />
/// <reference path="EtapasOcorrencia.js" />

var _nfseManual;
var $modalNFSeManual;

var NFSeManual = function () {
    var self = this;
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Documento = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });

    this.ValorTotal = PropertyEntity({ type: types.map, getType: typesKnockout.decimal, val: ko.observable("0,00"), def: "", text: Localization.Resources.Ocorrencias.Ocorrencia.ValorTotal.getFieldDescription(), enable: ko.observable(false) });
    this.Numero = PropertyEntity({ type: types.map, configInt: { precision: 0, allowZero: false, thousands: '' }, getType: typesKnockout.int, val: ko.observable("0"), def: "", text: Localization.Resources.Ocorrencias.Ocorrencia.Numero.getRequiredFieldDescription(), enable: ko.observable(true) });
    this.Serie = PropertyEntity({ type: types.map, configInt: { precision: 0, allowZero: false, thousands: '' }, getType: typesKnockout.int, val: ko.observable("0"), def: "", text: Localization.Resources.Ocorrencias.Ocorrencia.Serie.getRequiredFieldDescription(), issue: 756, enable: ko.observable(true) });
    this.ValorPrestacaoServico = PropertyEntity({ type: types.map, getType: typesKnockout.decimal, val: ko.observable("0,00"), def: "", text: Localization.Resources.Ocorrencias.Ocorrencia.ValorPrestacaoServico.getRequiredFieldDescription(), enable: ko.observable(false), eventChange: CalcularValoresISS });
    this.DataEmissao = PropertyEntity({ type: types.map, getType: typesKnockout.date, val: ko.observable("0,00"), def: "", text: Localization.Resources.Ocorrencias.Ocorrencia.DataEmissao.getFieldDescription(), enable: ko.observable(true) });
    this.AliquotaISS = PropertyEntity({ type: types.map, getType: typesKnockout.decimal, val: ko.observable("0,00"), def: "", text: Localization.Resources.Ocorrencias.Ocorrencia.AliquotaISS.getRequiredFieldDescription(), enable: ko.observable(false), eventChange: CalcularValoresISS });
    this.ValorISS = PropertyEntity({ type: types.map, getType: typesKnockout.decimal, val: ko.observable("0,00"), def: "", text: Localization.Resources.Ocorrencias.Ocorrencia.ValorISS.getRequiredFieldDescription(), enable: ko.observable(false) });
    this.BaseCalculo = PropertyEntity({ type: types.map, getType: typesKnockout.decimal, val: ko.observable("0,00"), def: "", text: Localization.Resources.Ocorrencias.Ocorrencia.BaseCalculo.getRequiredFieldDescription(), enable: ko.observable(true), eventChange: CalcularValoresISS });
    this.PercentualRetencao = PropertyEntity({ type: types.map, getType: typesKnockout.decimal, val: ko.observable("0,00"), def: "", text: Localization.Resources.Ocorrencias.Ocorrencia.PercentualRetencao.getFieldDescription(), enable: ko.observable(true) });
    this.ValorRetencao = PropertyEntity({ type: types.map, getType: typesKnockout.decimal, val: ko.observable("0,00"), def: "", text: Localization.Resources.Ocorrencias.Ocorrencia.ValorRetencao.getFieldDescription() , enable: ko.observable(true) });
    this.IncluirValorBC = PropertyEntity({ type: types.map, getType: typesKnockout.bool, val: ko.observable(false), def: false, text: Localization.Resources.Ocorrencias.Ocorrencia.IncluirValorBC.getFieldDescription(), enable: ko.observable(true) });
    this.Observacao = PropertyEntity({ type: types.map, val: ko.observable(""), text: Localization.Resources.Ocorrencias.Ocorrencia.Observacao.getFieldDescription(), enable: ko.observable(true) });

    this.XMLAutorizacao = PropertyEntity({ type: types.file, val: ko.observable(""), text: Localization.Resources.Ocorrencias.Ocorrencia.XMLAutorizacao.getFieldDescription(), enable: ko.observable(true), file: null, name: ko.pureComputed(function () { return self.XMLAutorizacao.val().replace('C:\\fakepath\\', '') }) });
    this.DANFSE = PropertyEntity({ type: types.file, val: ko.observable(""), text: "DANFSE:", enable: ko.observable(true), file: null, name: ko.pureComputed(function () { return self.DANFSE.val().replace('C:\\fakepath\\', '') }) });

    this.Fechar = PropertyEntity({ eventClick: fecharClick, type: types.event, text: Localization.Resources.Ocorrencias.Ocorrencia.Fechar, visible: ko.observable(true) });
    this.Emitir = PropertyEntity({ eventClick: emitirClick, type: types.event, text: Localization.Resources.Ocorrencias.Ocorrencia.Salvar , visible: ko.observable(true) });

    this.DownloadXML = PropertyEntity({ eventClick: downloadXMLClick, type: types.event, text: "XML", visible: ko.observable(true) });
    this.DownloadDANFSE = PropertyEntity({ eventClick: downloadDANFSEClick, type: types.event, text: "DANFSE", visible: ko.observable(true) });
}

//*******EVENTOS*******

function loadNFSeManual() {
    _nfseManual = new NFSeManual();
    KoBindings(_nfseManual, "knockoutNFSeManual");

    _nfseManual.XMLAutorizacao.file = document.getElementById(_nfseManual.XMLAutorizacao.id);
    _nfseManual.DANFSE.file = document.getElementById(_nfseManual.DANFSE.id);

    _nfseManual.IncluirValorBC.val.subscribe(CalcularValoresISS)

    $modalNFSeManual = $("#divModalNFSeManual");
    $modalNFSeManual.on('hidden.bs.modal', LimparModalNFSeManual);
}

function fecharClick(e, sender) {
    FecharModalNFSeManual();
}

function emitirClick(e, sender) {
    if (!ValidaNFSeManual()) return;

    exibirConfirmacao(Localization.Resources.Ocorrencias.Ocorrencia.EmitirNFS, Localization.Resources.Ocorrencias.Ocorrencia.VoceTemCertezaQueDesejaSalvarNFSe, function () {
        Salvar(_nfseManual, "OcorrenciaNFSeManual/Emitir", function (arg) {
            if (arg.Success) {
                if (arg.Data) {
                    exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucessp, Localization.Resources.Ocorrencias.Ocorrencia.NFSCriadaComSucesso);

                    // Envia arquivos
                    var anexos = [];
                    if (_nfseManual.XMLAutorizacao.file.files.length > 0) {
                        anexos.push({
                            Tipo: "XML",
                            Arquivo: _nfseManual.XMLAutorizacao.file.files[0]
                        });
                    }
                    if (_nfseManual.DANFSE.file.files.length > 0) {
                        anexos.push({
                            Tipo: "DANFSE",
                            Arquivo: _nfseManual.DANFSE.file.files[0]
                        });
                    }
                    if (anexos.length > 0)
                        AnexarArquivos(anexos);

                    _nfseManual.Emitir.visible(false);
                    FecharModalNFSeManual();
                } else {
                    exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.Aviso, arg.Msg);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, arg.Msg);
            }
        }, sender);
    });
}

function downloadXMLClick(e) {
    executarDownload("OcorrenciaNFSeManual/DownloadXML", { Codigo: e.Codigo.val() });
}

function downloadDANFSEClick(e) {
    executarDownload("OcorrenciaNFSeManual/DownloadDANFSE", { Codigo: e.Codigo.val() });
}

function abrirEmissaoNFSeManual(data) {
    _nfseManual.Codigo.val(data.Codigo);
    BuscarPorCodigo(_nfseManual, "OcorrenciaNFSeManual/BuscarPorCodigo", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                if (_nfseManual.Documento.val() == 0)
                    ControleCamposDadosNFSe(true);
                else
                    ControleCamposDadosNFSe(false);
                AbrirModalNFSeManual();
            } else {
                exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.Aviso, arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, arg.Msg);
        }
    });
}


//*******METODOS*******
function CalcularValoresISS() {
    AtualizarBCISS();
}

function AtualizarBCISS() {
    _nfseManual.BaseCalculo.val(_nfseManual.ValorPrestacaoServico.val());
    CalcularISS();
}

function CalcularISS() {
    if (_nfseManual.BaseCalculo.val() != "" && _nfseManual.AliquotaISS.val() != "") {

        var aliquota = Globalize.parseFloat(_nfseManual.AliquotaISS.val());
        var baseCalculo = Globalize.parseFloat(_nfseManual.BaseCalculo.val());
        var incluirBC = _nfseManual.IncluirValorBC.val();

        var valorISS = baseCalculo * (aliquota / 100);

        if (incluirBC) {
            baseCalculo += (aliquota > 0 ? ((baseCalculo / ((100 - aliquota) / 100)) - baseCalculo) : 0);
            valorISS = baseCalculo * (aliquota / 100);
        }
        _nfseManual.BaseCalculo.val(Globalize.format(baseCalculo, "n2"));
        _nfseManual.ValorISS.val(Globalize.format(valorISS, "n2"));
    }
}

function FecharModalNFSeManual() {
    $modalNFSeManual.modal('hide');
    _nfseManual.Emitir.visible(true);
}

function ControleCamposDadosNFSe(status) {
    _nfseManual.Numero.enable(status);
    _nfseManual.Serie.enable(status);
    _nfseManual.ValorPrestacaoServico.enable(status);
    _nfseManual.DataEmissao.enable(status);
    _nfseManual.AliquotaISS.enable(status);
    _nfseManual.IncluirValorBC.enable(status);
    _nfseManual.ValorRetencao.enable(status);
    _nfseManual.Observacao.enable(status);
    
    _nfseManual.BaseCalculo.enable(status);
    _nfseManual.PercentualRetencao.enable(status);
    _nfseManual.ValorRetencao.enable(status);

    _nfseManual.XMLAutorizacao.enable(status);
    _nfseManual.DANFSE.enable(status);

    _nfseManual.Emitir.visible(status);
}

function AbrirModalNFSeManual() {
    $modalNFSeManual.modal('show');
}

function LimparModalNFSeManual() {
    LimparCampos(_nfseManual);
    _nfseManual.XMLAutorizacao.value = null;
    _nfseManual.DANFSE.value = null;
}

function AnexarArquivos(anexos) {
    // Dados da req
    var dados = {
        Codigo: _nfseManual.Codigo.val()
    };

    // Arquivos
    var formData = new FormData();
    anexos.forEach(function (anexo) {
        formData.append("Tipo", anexo.Tipo);
        formData.append("Arquivo", anexo.Arquivo);
    });

    enviarArquivo("OcorrenciaNFSeManual/Anexar", dados, formData, function (arg) {
        if (arg.Success) {
            if (arg.Data != false) {
                exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, Localization.Resources.Ocorrencias.Ocorrencia.ArquivosAnexadosComSucesso);
                _nfseManual.XMLAutorizacao.file.files = null;
                _nfseManual.DANFSE.file.files = null;
            } else {
                exibirMensagem(tipoMensagem.falha, Localization.Resources.Ocorrencias.Ocorrencia.NaoFoiPossivelAnexarAquivos, arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, arg.Msg);
        }
    });
} 
function ValidaNFSeManual() {
    var valido = true;
    var msg = "";

    if (!_nfseManual.Serie.val() > 0) {
        valido = false;
        msg = "Série é obrigatória";
    }

    if (_nfseManual.DANFSE.file.files.length == 0) {
        valido = false;
        msg = "DANFSE é obrigatória";
    }

    if (!valido) exibirMensagem(tipoMensagem.atencao, Localization.Resources.Ocorrencias.Ocorrencia.Invalido , msg);

    return valido;
}