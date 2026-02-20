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
/// <reference path="../../../Enumeradores/EnumTipoDocumentoEmissao.js" />

//*******EVENTOS*******

var _codigoPreCte;
var _codigoCargaCTE;
var _rowPreCTe;
var _globalDataPreCTe;
var _isPreCte;
var _isSubstituicaoCTe = false;

function AbrirEnvioCTeDoPreCTeClick(data, row) {
    _codigoPreCte = data.CodigoPreCTE;
    _codigoCargaCTE = data.Codigo;
    _rowPreCTe = row;
    _globalDataPreCTe = data;
    $('#FileEnviarCTeDoPreCTe').val("");
    $("#FileEnviarCTeDoPreCTe").trigger("click");
}

function buscarPreCTes(callback) {
    var enviarCTe = {
        descricao: Localization.Resources.Cargas.Carga.EnviarXmlDoCte, id: guid(), metodo:
            function (data, row) {
                AbrirEnvioCTeDoPreCTeClick(data, row);
            }, icone: "", visibilidade: VisibilidadeEnvioPreCTe
    };

    var baixarEDI = { descricao: Localization.Resources.Cargas.Carga.BaixarEdi, id: guid(), metodo: baixarEDIClick, icone: "", visibilidade: VisibilidadePreCTeDownloadEDI };
    var baixarXMLPreCTe = { descricao: Localization.Resources.Cargas.Carga.DonwloadDoPreCte, id: guid(), metodo: baixarPreCTeClick, icone: "", visibilidade: VisibilidadeXMLPreCTe };
    var baixarDACTE = { descricao: Localization.Resources.Cargas.Carga.BaixarDacte, id: guid(), metodo: baixarDACTEClick, icone: "", visibilidade: VisibilidadePreCTeDownloadDACTE };
    var informarNFS = { descricao: Localization.Resources.Cargas.Carga.InformarNFS, id: guid(), metodo: informarNFSPreCTeClick, icone: "", visibilidade: VisibilidadeInformarNFSPreCTe };
    var substituirCTe = { descricao: Localization.Resources.Cargas.Carga.SubstituirOCTe, id: guid(), metodo: function (data, row) { substituirCTeClick(data, row); }, icone: "", visibilidade: VisibilidadeSubstituirCTe };

    var menuOpcoes = { tipo: TypeOptionMenu.list, descricao: Localization.Resources.Gerais.Geral.Opcoes, tamanho: 7, opcoes: [enviarCTe, baixarEDI, baixarXMLPreCTe, baixarDACTE, informarNFS, substituirCTe] };

    _gridCargaPreCTe = new GridView(_cargaCTe.PreCTe.idGrid, "CargaPreCTe/ConsultarCargaPreCTe", _cargaCTe, menuOpcoes);
    _gridCargaPreCTe.CarregarGrid(function () { buscarNotfisCarga(callback) });
}

function VisibilidadePreCTeDownloadEDI(e) {

    if (e.CodigoCTE > 0 && VerificaSePossuiPermissaoEspecial(EnumPermissaoPersonalizada.Carga_ReenviarIntegracoes, _PermissoesPersonalizadasCarga) && _cargaCTe.NotFis.visible())
        return true;
    else
        return false;
}

function VisibilidadeEnvioPreCTe(e) {
    return !e.CteEnviado && e.TipoDocumentoEmissao == EnumTipoDocumentoEmissao.CTe;
}

function VisibilidadePreCTeDownloadDACTE(e) {
    return e.CodigoCTE > 0 && e.TipoDocumentoEmissao == EnumTipoDocumentoEmissao.CTe;
}

function VisibilidadeXMLPreCTe(e) {
    return e.TipoDocumentoEmissao == EnumTipoDocumentoEmissao.CTe;
}

function VisibilidadeInformarNFSPreCTe(e) {
    return !e.CteEnviado && (e.TipoDocumentoEmissao == EnumTipoDocumentoEmissao.NFSe);
}

function VisibilidadeSubstituirCTe(e) {
    return e.CteEnviado && e.TipoDocumentoEmissao == EnumTipoDocumentoEmissao.CTe;
}

function EnviarCTeDoPreCTeClick() {
    if ($('#FileEnviarCTeDoPreCTe').val() != "") {
        var file = document.getElementById("FileEnviarCTeDoPreCTe");
        exibirConfirmacao(Localization.Resources.Gerais.Geral.Confirmacao, Localization.Resources.Cargas.Carga.RealmenteDesejaEnviarArquivoComoCteDestePreCte.format(file.files[0].name), function () {

            var formData = new FormData();
            formData.append("upload", file.files[0]);

            var data = {
                CodigoPreCTe: _codigoPreCte,
                Codigo: _codigoCargaCTE,
                SubstituicaoCTe: _isSubstituicaoCTe
            };

            enviarArquivo("CargaPreCTe/EnviarCTe?callback=?", data, formData, function (arg) {
                if (arg.Success) {
                    if (arg.Data !== false) {
                        if (arg.Data !== true) {
                            CompararEAtualizarGridEditableDataRow(_globalDataPreCTe, arg.Data)
                            _gridCargaPreCTe.AtualizarDataRow(_rowPreCTe, _globalDataPreCTe);
                        }
                        exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, Localization.Resources.Cargas.Carga.CteEnviadoComSucesso);
                    } else {
                        exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Atencao, arg.Msg, 30000);
                    }
                } else {
                    exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, arg.Msg);
                }
            });

            _isSubstituicaoCTe = false;
        }, function () { _isSubstituicaoCTe = false; });
    }
}

function buscarNotfisCarga(callback) {
    loadDropZonePreCTe();
    var baixarNotFisCarga = { descricao: Localization.Resources.Gerais.Geral.Download, id: guid(), metodo: baixarNotfis, icone: "", tamanho: 10 };
    var menuOpcoes = { tipo: TypeOptionMenu.link, descricao: Localization.Resources.Gerais.Geral.Opcoes, opcoes: [baixarNotFisCarga] };

    var gridNotfis = new GridView(_cargaCTe.NotFis.idGrid, "ControleGeracaoEDI/Pesquisa?TelaCarga=true", { Carga: _cargaCTe.Carga, }, menuOpcoes, null);
    gridNotfis.CarregarGrid(function () {
        if (gridNotfis.NumeroRegistros() > 0) {
            _cargaCTe.NotFis.visible(true);
        }
        callback();
    });
}

function baixarPreCTeClick(e) {
    var data = { CodigoPreCTE: e.CodigoPreCTE, CodigoEmpresa: e.CodigoEmpresa };
    executarDownload("CargaPreCTe/DownloadPreXML", data);
}

function baixarNotfis(e) {
    var data = { Codigo: e.Codigo, Carga: _cargaAtual.Codigo.val() };
    executarDownload("CargaPreCTe/DownloadArquivoEDI", data);
}

function baixarDACTEClick(e) {
    var data = { CodigoCTe: e.CodigoCTE, CodigoEmpresa: e.CodigoEmpresa };
    executarDownload("CargaCTe/DownloadDacte", data);
}

function informarNFSPreCTeClick(e) {
    abrirModalPreCTeInformarNFSCarga(e);
}

function substituirCTeClick(data, row) {
    exibirConfirmacao(Localization.Resources.Gerais.Geral.Atencao, Localization.Resources.Cargas.Carga.OCTeSerieFoiImportadoAnteriormenteParaEssaCargaEssaAcaoIraExcluirODocumentoAnteriorETodoOFluxoDeAprovacaoJaRealizadoTemCertezaQueDesejaSeguirComASubstituicao.format(data.NumeroCteEnviado, data.SerieCteEnviado), function () {
        _isSubstituicaoCTe = true;
        AbrirEnvioCTeDoPreCTeClick(data, row);
    });
}