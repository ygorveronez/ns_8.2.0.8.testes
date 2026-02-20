/// <autosync enabled="true" />
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
/// <reference path="../../../Global/SignalR/SignalR.js" />
/// <reference path="../../../Configuracao/EmissaoCTe/EmissaoCTe.js" />
/// <reference path="../../../Configuracao/Sistema/ConfiguracaoTMS.js" />
/// <reference path="../../../CTe/CTe/ComponentePrestacaoServico.js" />
/// <reference path="../../../CTe/CTe/CTe.js" />
/// <reference path="../../../CTe/CTe/Documento.js" />
/// <reference path="../../../CTe/CTe/DocumentoTransporteAnteriorEletronico.js" />
/// <reference path="../../../CTe/CTe/DocumentoTransporteAnteriorPapel.js" />
/// <reference path="../../../CTe/CTe/Duplicata.js" />
/// <reference path="../../../CTe/CTe/DuplicataAutomatica.js" />
/// <reference path="../../../CTe/CTe/InformacaoCarga.js" />
/// <reference path="../../../CTe/CTe/Motorista.js" />
/// <reference path="../../../CTe/CTe/Observacao.js" />
/// <reference path="../../../CTe/CTe/ObservacaoGeral.js" />
/// <reference path="../../../CTe/CTe/Participante.js" />
/// <reference path="../../../CTe/CTe/ProdutoPerigoso.js" />
/// <reference path="../../../CTe/CTe/QuantidadeCarga.js" />
/// <reference path="../../../CTe/CTe/Rodoviario.js" />
/// <reference path="../../../CTe/CTe/Seguro.js" />
/// <reference path="../../../CTe/CTe/TotalServico.js" />
/// <reference path="../../../CTe/CTe/Veiculo.js" />
/// <reference path="../DadosEmissao/Configuracao.js" />
/// <reference path="../DadosEmissao/DadosEmissao.js" />
/// <reference path="../DadosEmissao/Geral.js" />
/// <reference path="../DadosEmissao/Lacre.js" />
/// <reference path="../DadosEmissao/LocaisPrestacao.js" />
/// <reference path="../DadosEmissao/Observacao.js" />
/// <reference path="../DadosEmissao/Passagem.js" />
/// <reference path="../DadosEmissao/Percurso.js" />
/// <reference path="../DadosEmissao/Rota.js" />
/// <reference path="../DadosEmissao/Seguro.js" />
/// <reference path="../DadosTransporte/DadosTransporte.js" />
/// <reference path="../DadosTransporte/Motorista.js" />
/// <reference path="../DadosTransporte/Tipo.js" />
/// <reference path="../DadosTransporte/Transportador.js" />
/// <reference path="CTe.js" />
/// <reference path="MDFe.js" />
/// <reference path="NFS.js" />
/// <reference path="PreCTe.js" />
/// <reference path="../DocumentosEmissao/CargaPedidoDocumentoCTe.js" />
/// <reference path="../DocumentosEmissao/ConsultaReceita.js" />
/// <reference path="../DocumentosEmissao/CTe.js" />
/// <reference path="../DocumentosEmissao/Documentos.js" />
/// <reference path="../DocumentosEmissao/DropZone.js" />
/// <reference path="../DocumentosEmissao/EtapaDocumentos.js" />
/// <reference path="../DocumentosEmissao/NotaFiscal.js" />
/// <reference path="../Frete/Complemento.js" />
/// <reference path="../Frete/Componente.js" />
/// <reference path="../Frete/EtapaFrete.js" />
/// <reference path="../Frete/Frete.js" />
/// <reference path="../Frete/SemTabela.js" />
/// <reference path="../Frete/TabelaCliente.js" />
/// <reference path="../Frete/TabelaComissao.js" />
/// <reference path="../Frete/TabelaRota.js" />
/// <reference path="../Frete/TabelaSubContratacao.js" />
/// <reference path="../Frete/TabelaTerceiros.js" />
/// <reference path="../Impressao/Impressao.js" />
/// <reference path="../Integracao/Integracao.js" />
/// <reference path="../Integracao/IntegracaoCarga.js" />
/// <reference path="../Integracao/IntegracaoCTe.js" />
/// <reference path="../Integracao/IntegracaoEDI.js" />
/// <reference path="../Terceiro/ContratoFrete.js" />
/// <reference path="../DadosCarga/SignalR.js" />
/// <reference path="../DadosCarga/Carga.js" />
/// <reference path="../DadosCarga/DataCarregamento.js" />
/// <reference path="../DadosCarga/Leilao.js" />
/// <reference path="../DadosCarga/Operador.js" />
/// <reference path="../../../Consultas/Tranportador.js" />
/// <reference path="../../../Consultas/Localidade.js" />
/// <reference path="../../../Consultas/ModeloVeicularCarga.js" />
/// <reference path="../../../Consultas/TipoCarga.js" />
/// <reference path="../../../Consultas/Motorista.js" />
/// <reference path="../../../Consultas/Veiculo.js" />
/// <reference path="../../../Consultas/GrupoPessoa.js" />
/// <reference path="../../../Consultas/TipoOperacao.js" />
/// <reference path="../../../Consultas/Filial.js" />
/// <reference path="../../../Consultas/Cliente.js" />
/// <reference path="../../../Consultas/Usuario.js" />
/// <reference path="../../../Consultas/TipoCarga.js" />
/// <reference path="../../../Consultas/RotaFrete.js" />
/// <reference path="../../../Enumeradores/EnumSituacoesCarga.js" />
/// <reference path="../../../Enumeradores/EnumTipoFreteEscolhido.js" />
/// <reference path="../../../Enumeradores/EnumTipoOperacaoEmissao.js" />
/// <reference path="../../../Enumeradores/EnumMotivoPendenciaFrete.js" />
/// <reference path="../../../Enumeradores/EnumTipoContratacaoCarga.js" />
/// <reference path="../../../Enumeradores/EnumSituacaoContratoFrete.js" />
/// <reference path="../../../Enumeradores/EnumStatusCTe.js" />
/// <reference path="../../../Enumeradores/EnumTipoPagamento.js" />
/// <reference path="../../../Enumeradores/EnumTipoEmissaoCTeParticipantes.js" />
/// <reference path="../../../Enumeradores/EnumSituacaoRetornoDadosFrete.js" />

//*******MAPEAMENTO*******


//*******MAPEAMENTO KNOUCKOUT*******

var GlobalPercentualPreCTe = 0;


//*******EVENTOS*******

function loadDropZonePreCTe() {
    try {
        $("#" + _cargaCTe.Dropzone.id).dropzone({
            dictDefaultMessage: '<span class="text-center"><span class="font-lg"><span class="font-md"><i class="fal fa-caret-right text-danger"></i> ' + Localization.Resources.Cargas.Carga.ArrasteOsArquivos + ' <span class="font-xs">' + Localization.Resources.Cargas.Carga.ParaEnvio + '</span></span><span><br/><h5 class="display-inline" style="margin-top:-10px">' + Localization.Resources.Cargas.Carga.OuCliqueSelecione + '</h5></span>',
            dictResponseError: Localization.Resources.Cargas.Carga.FalhaAoEnviarOsArquivos,
            acceptedFiles: ".xml,.txt,.xlsx,.pal,.edi,.PAL,.NNN,.nnn,.SAO,.sao,.json",
            dictInvalidFileType: Localization.Resources.Cargas.Carga.ExtensaoDoArquivoInvalida,
            processing: function () {
                this.options.url = "CargaPreCTe/EnviarCTesParaPreCTe?Carga=" + _cargaCTe.Carga.val();
            },
            success: DropZoneSucessPreCTe,
            uploadMultiple: true,
            queuecomplete: DropZoneCompletePreCTe,
            TotaluploadprogressPreCTe: TotaluploadprogressPreCTe,
            url: "CargaPreCTe/EnviarCTesParaPreCTe?Carga=" + _cargaCTe.Carga.val()
        });
    } catch (e) {

    }
}

function TotaluploadprogressPreCTe(percentualProgresso) {
    if (GlobalPercentualPreCTe < Math.round(percentualProgresso)) {
        GlobalPercentualPreCTe = Math.round(percentualProgresso);
        $("#" + _cargaCTe.Dropzone.idTab).parent().css("visibility", "visible");
        if (GlobalPercentualPreCTe < 100) {
            $("#" + _cargaCTe.Dropzone.idTab).css("width", GlobalPercentualPreCTe + "%");
        } else {
            $("#" + _cargaCTe.Dropzone.idTab).text(Localization.Resources.Cargas.Carga.FinalizandoEnvio);
            $("#" + _cargaCTe.Dropzone.idTab).css("width", "100%");
        }
    }
}


function preecherErroPreCTe(mensagem, file) {
    var node, _i, _len, _ref, _results;
    file.previewElement.classList.add("dz-error");
    _ref = file.previewElement.querySelectorAll("[data-dz-errormessage]");
    _results = [];
    for (_i = 0, _len = _ref.length; _i < _len; _i++) {
        node = _ref[_i];
        _results.push(node.textContent = mensagem);
    }
    return _results;
}

function DropZoneSucessPreCTe(file, response, i, b) {
    var arg = typeof response === 'object' ? response : JSON.parse(response);

    if (arg.Success && arg.Data === false) {
        exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Atencao, arg.Msg);
        return preecherErroPreCTe(arg.Msg, file);
    }
    else if (arg.Data !== false) {

        if (!arg.Success) {
            exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Atencao, arg.Msg);
        }

        $.each(arg.Data.Arquivos, function (i, arquivo) {
            if (file.name == arquivo.nome) {
                if (arquivo.processada) {
                    return file.previewElement.classList.add("dz-success");
                }
                else {
                    return preecherErroPreCTe(arquivo.mensagem, file);
                }
            }
        });
    } 
    else {
        exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, arg.Msg);
        return preecherErroPreCTe(arg.Msg, file);
    }
}

function DropZoneCompletePreCTe(arg) {
    _gridCargaPreCTe.CarregarGrid();

    $("#" + _cargaCTe.Dropzone.idTab).parent().css("visibility", "hidden");
    $("#" + _cargaCTe.Dropzone.idTab).text("");
    $("#" + _cargaCTe.Dropzone.idTab).css("width", "0%");

    GlobalPercentualPreCTe = 0;
}