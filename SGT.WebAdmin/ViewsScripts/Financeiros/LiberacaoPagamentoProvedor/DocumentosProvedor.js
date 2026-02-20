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
/// <reference path="LiberacaoPagamentoEtapa.js" />
/// <reference path="LiberacaoPagamentoProvedor.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _documentosProvedor;
var _importarXML;
var _importarPDF;

var DocumentosProvedor = function () {
    this.CodigoPagamentoProvedor = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.CodigoAprovacaoAlcadaRegra = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Remetente = PropertyEntity({ val: ko.observable(""), def: 0, getType: typesKnockout.string, text: "Remetente: " });
    this.Destinatario = PropertyEntity({ val: ko.observable(""), def: 0, getType: typesKnockout.string, text: "Destinatário: " });
    this.Expedidor = PropertyEntity({ val: ko.observable(""), def: 0, getType: typesKnockout.string, text: "Expedidor: " });
    this.Recebedor = PropertyEntity({ val: ko.observable(""), def: 0, getType: typesKnockout.string, text: "Recebedor: " });
    this.Tomador = PropertyEntity({ val: ko.observable(""), def: 0, getType: typesKnockout.string, text: "Tomador: " });
    this.Transportador = PropertyEntity({ val: ko.observable(""), def: 0, getType: typesKnockout.string, text: "Transportador: " });
    this.TipoDocumentoProvedor = PropertyEntity({ val: ko.observable(""), text: "Tipo de Documento: " });
    this.LocalidadePrestacao = PropertyEntity({ val: ko.observable(""), text: "Localidade Prestação: " });
    this.ValorTotalProvedorCarga = PropertyEntity({ val: ko.observable(""), text: "Valor Total Provedor: " });
    this.NumerosCargas = PropertyEntity({ val: ko.observable(""), text: "N° Carga: " });
    this.ICMS = PropertyEntity({ val: ko.observable(""), def: 0, getType: typesKnockout.string, text: "ICMS: " });
    this.Etapa = PropertyEntity({ val: ko.observable(EnumEtapaLiberacaoPagamentoProvedor.DocumentoProvedor), def: EnumEtapaLiberacaoPagamentoProvedor.DocumentoProvedor, getType: typesKnockout.int });
    this.DataEmissao = PropertyEntity({ text: "*Data Emissão: ", enable: ko.observable(true), getType: typesKnockout.date, val: ko.observable(""), def: "", required: ko.observable(false), visible: ko.observable(false) });
    this.NumeroNFS = PropertyEntity({ text: "*Número NFS: ", val: ko.observable(""), getType: typesKnockout.int, enable: ko.observable(false), enable: ko.observable(true), required: ko.observable(false), visible: ko.observable(false) });
    this.ValorTotalProvedor = PropertyEntity({ text: "*Valor NFS: ", required: ko.observable(false), visible: ko.observable(false), enable: ko.observable(true), getType: typesKnockout.decimal, configDecimal: { precision: 2, allowZero: false, allowNegative: false } });

    this.MultiplosCTe = PropertyEntity({ val: ko.observable(false), getType: typesKnockout.bool, def: false, text: "Multiplos CT-e's", visible: ko.observable(false) });
    this.NumeroCTes = PropertyEntity({ text: "*Número CT-e's: ", val: ko.observable(""), getType: typesKnockout.text, enable: ko.observable(false), visible: ko.observable(false), required: ko.observable(false)});
    this.ValorCTes = PropertyEntity({ text: "*Valor CT-e's: ", val: ko.observable(""), getType: typesKnockout.decimal, enable: ko.observable(false), visible: ko.observable(false), required: ko.observable(""), configDecimal: { precision: 2, allowZero: false, allowNegative: false } });
    this.DataEmissaoCTes = PropertyEntity({ text: "*Data Emissão CT-e's: ", enable: ko.observable(false), getType: typesKnockout.date, val: ko.observable(""), def: "", required: ko.observable(false), visible: ko.observable(false) });

    this.ConfirmarDocumentosProvedor = PropertyEntity({ eventClick: confirmarDocumentosProvedor, type: types.event, text: "Confirmar", visible: ko.observable(true), enable: ko.observable(true) });
    this.Limpar = PropertyEntity({ eventClick: limparClick, type: types.event, text: "Limpar / Novo", visible: ko.observable(true) });
    this.Cancelar = PropertyEntity({ eventClick: cancelarClick, type: types.event, text: "Cancelar", visible: ko.observable(false) });
    this.ArquivoPDF = PropertyEntity({ eventClick: abrirModalPDF, type: types.local, text: "PDF", val: ko.observable(""), idGrid: guid(), visible: ko.observable(true), enable: ko.observable(true) });
    this.ArquivoXML = PropertyEntity({ eventClick: abrirModalXML, type: types.local, text: "XML", val: ko.observable(""), idGrid: guid(), visible: ko.observable(true), enable: ko.observable(true) });
    this.Anexo = PropertyEntity({ eventClick: adicionarAnexoModalClick, type: types.event, text: "Anexos", visible: ko.observable(false), enable: ko.observable(true) });

    this.MultiplosCTe.val.subscribe(function (novoValor) {
        _documentosProvedor.NumeroCTes.required(novoValor);
        _documentosProvedor.NumeroCTes.visible(novoValor);
        _documentosProvedor.NumeroCTes.enable(novoValor);

        _documentosProvedor.ValorCTes.required(novoValor);
        _documentosProvedor.ValorCTes.visible(novoValor);
        _documentosProvedor.ValorCTes.enable(novoValor);

        _documentosProvedor.DataEmissaoCTes.required(novoValor);
        _documentosProvedor.DataEmissaoCTes.visible(novoValor);
        _documentosProvedor.DataEmissaoCTes.enable(novoValor);

        _documentosProvedor.ArquivoPDF.enable(!novoValor);
        _documentosProvedor.ArquivoXML.enable(!novoValor);

        _anexo.Arquivo.accept(novoValor ? "" : _anexo.acceptDefault);

        if (_CRUDAprovacao) {
            _CRUDAprovacao.DetalhesCTe.visible(!novoValor);
            _CRUDAprovacao.DownloadDACTE.visible(!novoValor);
            _CRUDAprovacao.DownloadXML.visible(!novoValor);
        }
    });
}

var ImportarXML = function () {
    this.Arquivo = PropertyEntity({ type: types.file, codEntity: ko.observable(0), text: "Arquivo:", val: ko.observable(""), visible: ko.observable(true), required: true });
    this.AdicionarXML = PropertyEntity({ eventClick: importarXML, type: types.event, text: "Adicionar", visible: ko.observable(true) });
}

var ImportarPDF = function () {
    this.ArquivoPDF = PropertyEntity({ type: types.file, codEntity: ko.observable(0), text: "Arquivo:", val: ko.observable(""), visible: ko.observable(true) });
    this.AdicionarPDF = PropertyEntity({ eventClick: importarPDF, type: types.event, text: "Adicionar", visible: ko.observable(true) });
}

//*******EVENTOS*******

function loadDocumentosProvedor() {
    _documentosProvedor = new DocumentosProvedor();
    KoBindings(_documentosProvedor, "knockoutDocumentosProvedor");

    _importarXML = new ImportarXML();
    KoBindings(_importarXML, "knockoutImportarXML");

    _importarPDF = new ImportarPDF();
    KoBindings(_importarPDF, "knockoutImportarPDF");

    loadAnexo();
}

function PreencherObjetoDocumentosProvedor(listaDocumentosProvedor, pagamentoProvedor) {
    let somaValores = listaDocumentosProvedor.Carga.reduce((total, obj) => total + obj.ValorTotalProvedor, 0);
    let valorFormatado = somaValores.toLocaleString('pt-BR', { minimumFractionDigits: 2, maximumFractionDigits: 2 });

    _documentosProvedor.ValorTotalProvedorCarga.val(valorFormatado);
    _documentosProvedor.LocalidadePrestacao.val(listaDocumentosProvedor.LocalidadePrestacao.map(obj => obj.Descricao).filter(local => local).join(', '));
    _documentosProvedor.Transportador.val(listaDocumentosProvedor.Carga.map(obj => obj.Transportador).filter(transp => transp).join(', '));
    _documentosProvedor.NumerosCargas.val(listaDocumentosProvedor.Carga.map(obj => obj.NumeroCarga).filter(carga => carga).join(', '));
    _documentosProvedor.Tomador.val(listaDocumentosProvedor.Tomador);
    _documentosProvedor.Recebedor.val(listaDocumentosProvedor.Carga.map(obj => obj.Recebedor));
    _documentosProvedor.Expedidor.val(listaDocumentosProvedor.Carga.map(obj => obj.Expedidor));
    _documentosProvedor.Destinatario.val(listaDocumentosProvedor.Carga.map(obj => obj.Destinatario));
    _documentosProvedor.Remetente.val(listaDocumentosProvedor.Carga.map(obj => obj.Remetente));
    _documentosProvedor.ICMS.val(listaDocumentosProvedor.Carga.map(obj => obj.ICMS).filter(icms => icms).join(', '));
    _documentosProvedor.TipoDocumentoProvedor.val(listaDocumentosProvedor.TipoDocumentoProvedor);
    _documentosProvedor.CodigoPagamentoProvedor.val(listaDocumentosProvedor.CodigoPagamentoProvedor);
    _documentosProvedor.CodigoAprovacaoAlcadaRegra.val(listaDocumentosProvedor.CodigoAprovacaoAlcadaRegra);

    if (_documentosProvedor.TipoDocumentoProvedor.val() == 'CTe' || _documentosProvedor.TipoDocumentoProvedor.val() == 'CTe Complementar') {
        _documentosProvedor.MultiplosCTe.visible(true);

        if (pagamentoProvedor) {
            _documentosProvedor.MultiplosCTe.val(pagamentoProvedor.MultiplosCTe);
            _documentosProvedor.ValorCTes.val(pagamentoProvedor.ValorCTes);
            _documentosProvedor.NumeroCTes.val(pagamentoProvedor.NumeroCTes);
            _documentosProvedor.DataEmissaoCTes.val(pagamentoProvedor.DataEmissaoCTes);
        }
    }

    if ((_documentosProvedor.TipoDocumentoProvedor.val() == 'CTe' || _documentosProvedor.TipoDocumentoProvedor.val() == 'CTe Complementar') && _documentosProvedor.MultiplosCTe.val() != true) {
        visibilidadeCTeAprovacao();
    } else if (_documentosProvedor.TipoDocumentoProvedor.val() == 'NFSe') {
        _documentosProvedor.ValorTotalProvedor.val(listaDocumentosProvedor.ValorTotalProvedor);
        _documentosProvedor.DataEmissao.val(listaDocumentosProvedor.DataEmissao);
        _documentosProvedor.NumeroNFS.val(listaDocumentosProvedor.NumeroNFS);

        visibilidadeNFSeAprovacao();
    }
    else if (_documentosProvedor.MultiplosCTe.val() == true) {
        _documentosProvedor.ValorTotalProvedor.val(listaDocumentosProvedor.ValorTotalProvedor);
        _documentosProvedor.DataEmissao.val(listaDocumentosProvedor.DataEmissao);
        _documentosProvedor.NumeroNFS.val(listaDocumentosProvedor.NumeroNFS);

        visibilidadeMultiploCTeAprovacao();
    }
}

function confirmarDocumentosProvedor() {
    exibirConfirmacao("Confirmar Documentos do Provedor", "Deseja confirmar os Documentos Provedor?", function () {
        var data = {
            CodigoPagamentoProvedor: _documentosProvedor.CodigoPagamentoProvedor.val(),
            ValorTotalProvedor: _documentosProvedor.ValorTotalProvedor.val(),
            DataEmissao: _documentosProvedor.DataEmissao.val(),
            NumeroNFS: _documentosProvedor.NumeroNFS.val(),
            MultiplosCTe: _documentosProvedor.MultiplosCTe.val(),
            ValorCTes: _documentosProvedor.ValorCTes.val(),
            NumeroCTes: _documentosProvedor.NumeroCTes.val(),
            DataEmissaoCTes: _documentosProvedor.DataEmissaoCTes.val(),
        }

        if (!_documentosProvedor.MultiplosCTe.val() && _documentosEmpresaFilial.TipoDocumentoProvedor.val() != EnumTipoDocumentoProvedor.NFSe && (_importarXML.Arquivo.val() == 0 || _importarPDF.ArquivoPDF.val() == 0)) {
            var mensagem = _importarXML.Arquivo.val() == 0 ? "É obrigatório importar um XML." : "É obrigatório importar um PDF.";
            exibirMensagem(tipoMensagem.atencao, "Atenção", mensagem);
            return;
        }

        if (!_documentosProvedor.MultiplosCTe.val() && _documentosEmpresaFilial.TipoDocumentoProvedor.val() == EnumTipoDocumentoProvedor.NFSe && _importarPDF.ArquivoPDF.val() == 0) {
            exibirMensagem(tipoMensagem.atencao, "Atenção", "É obrigatório importar um PDF.");
            return;
        }

        if (_documentosProvedor.MultiplosCTe.val() && _anexo.Anexos.val() == 0) {
            exibirMensagem(tipoMensagem.atencao, "Atenção", "É obrigatório anexar ao menos um documento.");
            return;
        }

        executarReST("LiberacaoPagamentoProvedor/ConfirmarDocumentosProvedor", data, function (arg) {
            if (arg.Success) {
                if (arg.Data !== false) {
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Confirmado com sucesso");

                    SetarEtapasPagamentoProvedor(arg.Data.Status, arg.Data.Situacao);
                    preencherDocumentosAprovacao();

                    validarEtapa(arg.Data.Status);
                }
                else
                    exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            } else {
                exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
            }
        }, null);
    });
}

function visibilidadeCTeAprovacao() {
    _detalheDocumento.AliquotaICMS.visible(true);
    _detalheDocumento.ValorICMS.visible(true);
    _detalheDocumento.Remetente.visible(true);
    _detalheDocumento.Destinatario.visible(true);
    _detalheDocumento.Expedidor.visible(true);
    _detalheDocumento.Recebedor.visible(true);
    _detalheDocumento.Emissor.visible(true);

    _detalheDocumentoRecebido.AliquotaICMS.visible(true);
    _detalheDocumentoRecebido.ValorICMS.visible(true);
    _detalheDocumentoRecebido.Remetente.visible(true);
    _detalheDocumentoRecebido.Destinatario.visible(true);
    _detalheDocumentoRecebido.Expedidor.visible(true);
    _detalheDocumentoRecebido.Recebedor.visible(true);
    _detalheDocumentoRecebido.Tomador.visible(true);
    _detalheDocumentoRecebido.Emissor.visible(true);
    _detalheDocumentoRecebido.DataEmissaoNFSe.visible(false);
    _detalheDocumentoRecebido.NumeroNFSe.visible(false);
}

function visibilidadeMultiploCTeAprovacao() {
    _detalheDocumento.AliquotaICMS.visible(false);
    _detalheDocumento.ValorICMS.visible(false);
    _detalheDocumento.Remetente.visible(false);
    _detalheDocumento.Destinatario.visible(false);
    _detalheDocumento.Expedidor.visible(false);
    _detalheDocumento.Recebedor.visible(false);
    _detalheDocumento.Emissor.visible(false);

    _detalheDocumentoRecebido.AliquotaICMS.visible(false);
    _detalheDocumentoRecebido.ValorICMS.visible(false);
    _detalheDocumentoRecebido.Remetente.visible(false);
    _detalheDocumentoRecebido.Destinatario.visible(false);
    _detalheDocumentoRecebido.Expedidor.visible(false);
    _detalheDocumentoRecebido.Recebedor.visible(false);
    _detalheDocumentoRecebido.Tomador.visible(false);
    _detalheDocumentoRecebido.Emissor.visible(false);
    _detalheDocumentoRecebido.DataEmissaoNFSe.visible(true);
    _detalheDocumentoRecebido.NumeroNFSe.visible(false);
}

function visibilidadeNFSeAprovacao() {
    _documentosProvedor.DataEmissao.visible(true);
    _documentosProvedor.NumeroNFS.visible(true);
    _documentosProvedor.ValorTotalProvedor.visible(true);
    _documentosProvedor.DataEmissao.required(true);
    _documentosProvedor.NumeroNFS.required(true);

    _detalheDocumento.Tomador.visible(true);
    _detalheDocumento.Emissor.visible(true);

    _detalheDocumentoRecebido.DataEmissaoNFSe.visible(true);
    _detalheDocumentoRecebido.NumeroNFSe.visible(true);

    _detalheDocumento.AliquotaICMS.visible(false);
    _detalheDocumento.ValorICMS.visible(false);
    _detalheDocumento.Remetente.visible(false);
    _detalheDocumento.Destinatario.visible(false);
    _detalheDocumento.Expedidor.visible(false);
    _detalheDocumento.Recebedor.visible(false);

    _detalheDocumentoRecebido.AliquotaICMS.visible(false);
    _detalheDocumentoRecebido.ValorICMS.visible(false);
    _detalheDocumentoRecebido.Remetente.visible(false);
    _detalheDocumentoRecebido.Destinatario.visible(false);
    _detalheDocumentoRecebido.Expedidor.visible(false);
    _detalheDocumentoRecebido.Recebedor.visible(false);
    _detalheDocumentoRecebido.Tomador.visible(false);
    _detalheDocumentoRecebido.Emissor.visible(false);
}

//*******Importações*******

function importarXML(e, sender) {
    var file = document.getElementById(_importarXML.Arquivo.id);

    var formData = new FormData();
    formData.append("upload", file.files[0]);

    enviarArquivo("LiberacaoPagamentoProvedor/ImportarXML?callback=?", { Codigo: _documentosProvedor.CodigoPagamentoProvedor.val() }, formData, function (arg) {
        if (arg.Success) {
            exibirMensagem(tipoMensagem.ok, "Sucesso", "XML importado com sucesso.")
            Global.fecharModal("divModalAdicionarXML");
            LimparCampo(_importarXML.Arquivo);
        } else {
            exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg, 99999999);
        }
    });
}

function importarPDF(e, sender) {
    var file = document.getElementById(_importarPDF.ArquivoPDF.id);

    var formData = new FormData();
    formData.append("upload", file.files[0]);

    enviarArquivo("LiberacaoPagamentoProvedor/ImportarPDF?callback=?", { Codigo: _documentosProvedor.CodigoPagamentoProvedor.val() }, formData, function (arg) {
        if (arg.Success) {
            exibirMensagem(tipoMensagem.ok, "Sucesso", "PDF importado com sucesso.")
            Global.fecharModal("divModalAdicionarPDF");
            LimparCampo(_importarPDF.ArquivoPDF);
        } else {
            exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg, 99999999);
        }
    });
}

function abrirModalXML() {
    Global.abrirModal("divModalAdicionarXML");
}

function abrirModalPDF() {
    Global.abrirModal("divModalAdicionarPDF");
}