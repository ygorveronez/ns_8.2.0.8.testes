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
/// <reference path="Configuracao.js" />
/// <reference path="DadosEmissao.js" />
/// <reference path="Geral.js" />
/// <reference path="Lacre.js" />
/// <reference path="Observacao.js" />
/// <reference path="Passagem.js" />
/// <reference path="Percurso.js" />
/// <reference path="Rota.js" />
/// <reference path="Seguro.js" />
/// <reference path="../DadosTransporte/DadosTransporte.js" />
/// <reference path="../DadosTransporte/Motorista.js" />
/// <reference path="../DadosTransporte/Tipo.js" />
/// <reference path="../DadosTransporte/Transportador.js" />
/// <reference path="../Documentos/CTe.js" />
/// <reference path="../Documentos/MDFe.js" />
/// <reference path="../Documentos/NFS.js" />
/// <reference path="../Documentos/PreCTe.js" />
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

//*******MAPEAMENTO KNOUCKOUT*******
var _mapaMDFe;


var PercursoCargaMDFe = function () {
    this.Carga = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.AlterarPercursoMDFe = PropertyEntity({ type: types.event, eventClick: alterarPercursoMDFeClick, text: Localization.Resources.Cargas.Carga.AtualizarPercurso, visible: ko.observable(true) });
    this.CargaPercursos = PropertyEntity({ type: types.map, getType: typesKnockout.dynamic });
}

//*******EVENTOS*******

function loadCargaLocaisPrestacao(callback) {
    $("#mapa").empty();

    var percursoCargaMDFe = new PercursoCargaMDFe();
    KoBindings(percursoCargaMDFe, "knoutContentPercursoCargaMDFe");

    _mapaMDFe = new MapaMDFe();
    _mapaMDFe.LoadMapaMDFe("mapa", function () {
        callback();
    });
}


function alterarPercursoMDFeClick(e, sender) {
    var valido = _mapaMDFe.ValidarPassagens();
    if (valido) {
        e.Carga.val(_cargaAtual.Codigo.val());
        e.CargaPercursos.val(JSON.stringify(_mapaMDFe.GetOrigensDestinos()));
        Salvar(e, "CargaLocaisPrestacao/AlterarPercursoPassagemMDFe", function (arg) {
            if (arg.Success) {
                if (arg.Data) {
                    Global.fecharModal("divModalCargaPercursoMDFe");
                    exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, Localization.Resources.Cargas.Carga.PassagemEntreEstadosAtualizadaComSucesso);
                } else {
                    exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Atencao, arg.Msg);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, arg.Msg);
            }
        });
    }
}

function percursoMDFeClick(e, sender) {
    _cargaAtual = e;
    BuscarPercursoParaMDFe();
}


//*******MÉTODOS*******

function BuscarPercursoParaMDFe(codigoCarga) {
    var data = null
    if (codigoCarga != null)
        data = { Carga: codigoCarga };
    else
        data = { Carga: _cargaAtual.Codigo.val() };

    executarReST("CargaLocaisPrestacao/BuscarRotaPorCargaParaPasssagensMDFe", data, function (arg) {
        if (arg.Success) {
            if (arg.Data != false) {
                loadCargaLocaisPrestacao(function () {
                    _mapaMDFe.LimparMapa();
                    var retorno = arg.Data;
                    if (retorno.EstadoDestino != "EX" && retorno.EstadoDestino != "EX") {
                        _mapaMDFe.SetEstadoDestino(retorno.EstadoDestino);
                        $.each(retorno.EstadosOrigensDestinos, function (i, estadosMDFe) {
                            if (estadosMDFe.Origem != null)
                                _mapaMDFe.AddOrigemDestino(estadosMDFe.Origem.Estado, estadosMDFe.Destino.Estado, estadosMDFe.Codigo);

                        });

                        $.each(retorno.rotasParaMDFe, function (i, rota) {
                            _mapaMDFe.AddLocalidadesBuscaAPI(rota.Cidade + "," + rota.Estado);
                        });

                        $.each(arg.Data.passagens, function (i, estado) {
                            _mapaMDFe.AddEstadoPassagem(estado.Sigla, estado.Posicao);
                        });
                        _mapaMDFe.AtualizarDisplayMapa();
                        Global.abrirModal("divModalCargaPercursoMDFe");
                    } else {
                        exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Atencao, Localization.Resources.Cargas.Carga.ObrigatorioInformarFronteiraParaOperacoesDeImportacaoExportacao);
                    }
                });
            } else {
                exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.Aviso, arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, arg.Msg);
        }
    });
}