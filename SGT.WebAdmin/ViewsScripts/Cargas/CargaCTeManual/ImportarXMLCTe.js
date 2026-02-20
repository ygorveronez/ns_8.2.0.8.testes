/// <reference path="CargaCTeManual.js" />
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


//*******MAPEAMENTO KNOUCKOUT*******

var _ArquivosParaImportar, _ImportarXMLCTe;
var listaQuantidadesCarga = new Array();

var ImportarXMLCTe = function () {
    this.ImportarXMLCTe = PropertyEntity({ eventClick: ImportarXMLCTeClick, type: types.event, text: "Importar XML de CT-e" });
};

//*******EVENTOS*******

function LoadImportarXMLCTe() {

    _ImportarXMLCTe = new ImportarXMLCTe();
    KoBindings(_ImportarXMLCTe, "divModalImportarXMLCTe");

    $("#" + _carga.ArquivoCTe.id).on("change", function () {
        AbrirTelaImportacaoXMLCTe(this.files);
    });

    $("#" + _carga.ArquivoCTe.id).click(function () {
        $('[data-toggle="dropdown"]').parent().removeClass('open');
    });
}

function ImportarXMLCTeClick() {

    var formData = new FormData();

    for (var i = 0; i < _ArquivosParaImportar.length; i++)
        formData.append("upload" + i, _ArquivosParaImportar[i]);

    var data = {
        Codigo: _carga.Codigo.val()
    };

    enviarArquivo("CargaCTeManual/ObterInformacoesXMLCTe", data, formData, function (arg) {
        if (arg.Success) {
            if (arg.Data) {

                var sucesso = true;
                var documentos = new Array();

                for (var i = 0; i < arg.Data.length; i++) {
                    var ret = arg.Data[i];

                    if (!ret.Sucesso) {
                        sucesso = false;
                        $("#xmlCTeImportar_" + ret.Indice).removeClass("bg-info-100").addClass("bg-danger-100");
                        $("#xmlCTeImportar_" + ret.Indice + " .situacaoImportacaoXMLCTe").text(ret.Mensagem).prop("title", ret.Mensagem);
                    } else {
                        $("#xmlCTeImportar_" + ret.Indice).removeClass("bg-info-100").addClass("bg-success-100");
                        $("#xmlCTeImportar_" + ret.Indice + " .situacaoImportacaoXMLCTe").text("Arquivo válido.");
                        documentos.push(ret.Documento);
                    }
                }

                if (sucesso) {
                    Global.fecharModal('divModalImportarXMLCTe');
                    abrirModalCTe(0, null, false, documentos);
                }
            } else {
                exibirMensagem(tipoMensagem.atencao, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    });
}

//*******MÉTODOS*******

function AbrirTelaImportacaoXMLCTe(files) {
    _ArquivosParaImportar = new Array();
    $("#tblXMLCTeImportar tbody").html("");

    for (var i = 0; i < files.length; i++) {
        var file = files[i];

        if (file.type == "text/xml") {
            $("#tblXMLCTeImportar tbody").append('<tr id="xmlCTeImportar_' + i + '" class="bg-info-100"><td>' + (i + 1) + '</td><td>' + file.name + '</td><td>' + HumanFileSize(file.size) + '</td><td class="situacaoImportacaoXMLCTe">Não enviado.</td></tr>');
            _ArquivosParaImportar.push(file);
        }
    }

    if (_ArquivosParaImportar.length > 0)        
        Global.abrirModal("divModalImportarXMLCTe");
    else
        exibirMensagem(tipoMensagem.atencao, "Atenção!", "Somente são aceitos arquivos XML.");

    var fileControl = $("#" + _carga.ArquivoCTe.id);
    fileControl.replaceWith(fileControl = fileControl.clone(true));
}

function SetarInformacoesCTesParaSubcontratacao(instanciaCTe, ctesParaSubcontratacao) {
    instanciaCTe.CTe.Tipo.val(EnumTipoCTe.Normal);
    instanciaCTe.CTe.TipoServico.val(EnumTipoServicoCTe.SubContratacao);
    instanciaCTe.CTe.TipoTomador.val(EnumTipoTomador.Outros);

    var cteParaSubcontratacaoBase = ctesParaSubcontratacao[0];

    instanciaCTe.InformacaoCarga.ProdutoPredominante.val(cteParaSubcontratacaoBase.ProdutoPredominante);

    PreencherDadosParticipanteCTe(instanciaCTe.Remetente, cteParaSubcontratacaoBase.Remetente);
    PreencherDadosParticipanteCTe(instanciaCTe.Destinatario, cteParaSubcontratacaoBase.Destinatario);
    PreencherDadosParticipanteCTe(instanciaCTe.Expedidor, cteParaSubcontratacaoBase.Expedidor);
    PreencherDadosParticipanteCTe(instanciaCTe.Recebedor, cteParaSubcontratacaoBase.Recebedor);

    instanciaCTe.Tomador.CPFCNPJ.val(cteParaSubcontratacaoBase.Emitente.CNPJ);
    instanciaCTe.Tomador.BuscarDadosPorCPFCNPJ();

    var valorTotalCarga = 0;
    var valorFrete = 0;
    var valorPrestacaoServico = 0;
    var valorReceber = 0;
    var listaComponentes = new Array();

    ctesParaSubcontratacao.forEach((cteParaSubcontratacao) => {

        if (cteParaSubcontratacao.NFEs != null && cteParaSubcontratacao.NFEs.length > 0) {
            cteParaSubcontratacao.NFEs.forEach((nfe) => {
                AdicionarNFeCTe(instanciaCTe, nfe, cteParaSubcontratacao);
            });
        } else if (cteParaSubcontratacao.OutrosDocumentos != null && cteParaSubcontratacao.OutrosDocumentos.length > 0) {
            cteParaSubcontratacao.OutrosDocumentos.forEach((outroDocumento) => {
                AdicionarOutroDocumentoCTe(instanciaCTe, outroDocumento, cteParaSubcontratacao);
            });
        } else if (cteParaSubcontratacao.NotasFiscais != null && cteParaSubcontratacao.NotasFiscais.length > 0) {
            cteParaSubcontratacao.NotasFiscais.forEach((notaFiscal) => {
                AdicionarNotaFiscalCTe(instanciaCTe, notaFiscal);
            });
        }

        AdicionarDocumentoTransporteAnteriorCTe(instanciaCTe, cteParaSubcontratacao);

        if (cteParaSubcontratacao.QuantidadesCarga != null && cteParaSubcontratacao.QuantidadesCarga.length > 0) {
            for (var i = 0; i < cteParaSubcontratacao.QuantidadesCarga.length; i++) {
                var quantidadeAdicional = cteParaSubcontratacao.QuantidadesCarga[i];
                var encontrou = false;
                for (var j = 0; j < listaQuantidadesCarga.length; j++) {
                    var quantidadesSalvas = listaQuantidadesCarga[j];
                    if (quantidadeAdicional.Medida == quantidadesSalvas.TipoMedida) {
                        quantidadesSalvas.Quantidade += quantidadeAdicional.Quantidade;

                        encontrou = true;
                        break;
                    }
                }
                if (!encontrou) {
                    listaQuantidadesCarga.push({
                        Codigo: quantidadeAdicional.UnidadeMedida.Codigo,
                        Descricao: quantidadeAdicional.UnidadeMedida.Descricao,
                        TipoMedida: quantidadeAdicional.Medida,
                        Quantidade: quantidadeAdicional.Quantidade
                    });
                }
            }
        }

        if (cteParaSubcontratacao.ValorFrete != null && cteParaSubcontratacao.ValorFrete.ComponentesAdicionais.length > 0) {
            for (var i = 0; i < cteParaSubcontratacao.ValorFrete.ComponentesAdicionais.length; i++) {
                var componenteAdicional = cteParaSubcontratacao.ValorFrete.ComponentesAdicionais[i];
                var encontrou = false;
                for (var j = 0; j < listaComponentes.length; j++) {
                    var componentesSalvos = listaComponentes[j];
                    if (componenteAdicional.Componente.CodigoIntegracao == componentesSalvos.Codigo) {
                        componentesSalvos.Valor += componenteAdicional.ValorComponente;

                        encontrou = true;
                        break;
                    }
                }
                if (!encontrou) {
                    listaComponentes.push({
                        Codigo: parseInt(componenteAdicional.Componente.CodigoIntegracao),
                        Descricao: componenteAdicional.Componente.Descricao,
                        Valor: componenteAdicional.ValorComponente
                    });
                }
            }
        }

        valorTotalCarga += cteParaSubcontratacao.InformacaoCarga.ValorTotalCarga;
        valorFrete += cteParaSubcontratacao.ValorFrete.FreteProprio;
        valorPrestacaoServico += cteParaSubcontratacao.ValorFrete.ValorPrestacaoServico;
        valorReceber += cteParaSubcontratacao.ValorFrete.ValorTotalAReceber;
    });

    listaQuantidadesCarga.forEach((quantidadeCarga) => {
        AdicionarQuantidadeCargaCTe(instanciaCTe, quantidadeCarga);
    });

    listaComponentes.forEach((componente) => {
        AdicionarComponenteFreteCTe(instanciaCTe, componente);
    });

    instanciaCTe.InformacaoCarga.ValorTotalCarga.val(Globalize.format(valorTotalCarga, "n2"));
    instanciaCTe.TotalServico.ValorFrete.val(Globalize.format(valorFrete, "n2"));
    instanciaCTe.TotalServico.ValorPrestacaoServico.val(Globalize.format(valorPrestacaoServico, "n2"));
    instanciaCTe.TotalServico.ValorReceber.val(Globalize.format(valorReceber, "n2"));
}

function AdicionarDocumentoTransporteAnteriorCTe(instanciaCTe, cteParaSubcontratacao) {
    instanciaCTe.DocumentoTransporteAnteriorEletronico.Emitente.codEntity(cteParaSubcontratacao.Emitente.CNPJ);
    instanciaCTe.DocumentoTransporteAnteriorEletronico.Emitente.val(cteParaSubcontratacao.Emitente.RazaoSocial);
    instanciaCTe.DocumentoTransporteAnteriorEletronico.Chave.val(cteParaSubcontratacao.Chave);

    instanciaCTe.DocumentoTransporteAnteriorEletronico.AdicionarDocumento();
}

function AdicionarNFeCTe(instanciaCTe, nfe, cteParaSubcontratacao) {

    var data = moment(nfe.DataEmissao, "YYYY-MM-DDTHH:mm:ss");

    if (data.year() == 1)
        data = moment(cteParaSubcontratacao.DataEmissao, "YYYY-MM-DDTHH:mm:ss");

    if (data.year() == 1)
        data = moment();

    instanciaCTe.Documento.TipoDocumento.val(EnumTipoDocumentoCTe.NFeNotaFiscalEletronica);
    instanciaCTe.Documento.Numero.val(nfe.Numero);
    instanciaCTe.Documento.Chave.val(nfe.Chave);
    instanciaCTe.Documento.DataEmissao.val(data.format('DD/MM/YYYY'));
    instanciaCTe.Documento.ValorNotaFiscal.val(Globalize.format(nfe.Valor, "n2"));
    instanciaCTe.Documento.Peso.val(Globalize.format(nfe.Peso, "n2"));

    instanciaCTe.Documento.AdicionarDocumento();
}

function AdicionarOutroDocumentoCTe(instanciaCTe, outroDocumento, cteParaSubcontratacao) {

    var data = moment(outroDocumento.DataEmissao, "YYYY-MM-DDTHH:mm:ss");

    if (data.year() == 1)
        data = moment(cteParaSubcontratacao.DataEmissao, "YYYY-MM-DDTHH:mm:ss");

    if (data.year() == 1)
        data = moment();

    instanciaCTe.Documento.TipoDocumento.val(EnumTipoDocumentoCTe.Outros);
    instanciaCTe.Documento.Numero.val(outroDocumento.Numero);
    instanciaCTe.Documento.Descricao.val(outroDocumento.Descricao);
    instanciaCTe.Documento.DataEmissao.val(data.format('DD/MM/YYYY'));
    instanciaCTe.Documento.ValorNotaFiscal.val(Globalize.format(outroDocumento.Valor, "n2"));
    instanciaCTe.Documento.Peso.val(Globalize.format(outroDocumento.Peso, "n2"));

    instanciaCTe.Documento.AdicionarDocumento();
}

function AdicionarNotaFiscalCTe(instanciaCTe, notaFiscal) {
    instanciaCTe.Documento.TipoDocumento.val(EnumTipoDocumentoCTe.NotaFiscal);
    instanciaCTe.Documento.Modelo.val(notaFiscal.ModeloNotaFiscal);
    instanciaCTe.Documento.Numero.val(notaFiscal.Numero);
    instanciaCTe.Documento.Serie.val(notaFiscal.Serie);
    instanciaCTe.Documento.CFOP.val(notaFiscal.CFOP);
    instanciaCTe.Documento.PIN.val(notaFiscal.PINSuframa);
    instanciaCTe.Documento.NCMPredominante.val(notaFiscal.NCMPredominante);
    instanciaCTe.Documento.DataEmissao.val(moment(notaFiscal.DataEmissao).format('DD/MM/YYYY'));
    instanciaCTe.Documento.ValorNotaFiscal.val(Globalize.format(notaFiscal.Valor, "n2"));
    instanciaCTe.Documento.BaseCalculoICMS.val(Globalize.format(notaFiscal.BaseCalculoICMS, "n2"));
    instanciaCTe.Documento.ValorICMS.val(Globalize.format(notaFiscal.ValorICMS, "n2"));
    instanciaCTe.Documento.BaseCalculoICMSST.val(Globalize.format(notaFiscal.BaseCalculoICMSST, "n2"));
    instanciaCTe.Documento.ValorICMSST.val(Globalize.format(notaFiscal.ValorICMSST, "n2"));
    instanciaCTe.Documento.ValorProdutos.val(Globalize.format(notaFiscal.ValorProdutos, "n2"));
    instanciaCTe.Documento.Peso.val(Globalize.format(notaFiscal.Peso, "n2"));

    instanciaCTe.Documento.AdicionarDocumento();
}

function PreencherDadosParticipanteCTe(participante, dados) {
    if (dados == null || participante == null)
        return;

    if (dados.ClienteExterior) {
        participante.ParticipanteExterior.val(true);
        participante.RazaoSocial.val(dados.RazaoSocial);
        participante.Endereco.val(dados.Endereco.Logradouro);
        participante.Numero.val(dados.Endereco.Numero);
        participante.Bairro.val(dados.Endereco.Bairro);
        participante.Complemento.val(dados.Endereco.Complemento);
        participante.Pais.val(dados.Endereco.Cidade.Pais.NomePais);
        participante.Pais.codEntity(dados.Endereco.Cidade.Pais.CodigoPais);
        participante.EmailGeral.val(dados.Email);
        participante.LocalidadeExterior.val(dados.Endereco.Cidade.Descricao);
    } else {
        participante.CPFCNPJ.val(dados.CPFCNPJ);
        participante.BuscarDadosPorCPFCNPJ();
    }
}

function AdicionarQuantidadeCargaCTe(instanciaCTe, quantidadeCarga) {
    instanciaCTe.QuantidadeCarga.UnidadeMedida.codEntity(quantidadeCarga.Codigo);
    instanciaCTe.QuantidadeCarga.UnidadeMedida.val(quantidadeCarga.Descricao);
    instanciaCTe.QuantidadeCarga.TipoMedida.val(quantidadeCarga.TipoMedida);
    instanciaCTe.QuantidadeCarga.Quantidade.val(Globalize.format(quantidadeCarga.Quantidade, "n2"));

    instanciaCTe.QuantidadeCarga.AdicionarQuantidadeCarga();
}

function AdicionarComponenteFreteCTe(instanciaCTe, componente) {
    instanciaCTe.Componente.Componente.codEntity(componente.Codigo);
    instanciaCTe.Componente.Componente.val(componente.Descricao);
    instanciaCTe.Componente.Valor.val(Globalize.format(componente.Valor, "n2"));
    instanciaCTe.Componente.IncluirBaseCalculoICMS.val(false);
    instanciaCTe.Componente.IncluirTotalReceber.val(false);

    instanciaCTe.Componente.AdicionarComponente();
}