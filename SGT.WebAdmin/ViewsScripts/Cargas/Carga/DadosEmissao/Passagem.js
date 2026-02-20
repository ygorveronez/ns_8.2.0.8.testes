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
/// <reference path="LocaisPrestacao.js" />
/// <reference path="Observacao.js" />
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

var _mapaCargaDadosEmissaoPassagem, _cargaDadosEmissaoPassagem;


var CargaDadosEmissaoPassagem = function () {
    this.Carga = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Pedido = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int, idTab: guid(), enable: ko.observable(true) });
    this.AlterarPercursoMDFe = PropertyEntity({ type: types.event, eventClick: alterarCargaDadosEmissaoPassagem, text: Localization.Resources.Cargas.Carga.AtualizarPassagens, visible: ko.observable(true), enable: ko.observable(false) });
    this.CargaPercursos = PropertyEntity({ type: types.map, getType: typesKnockout.dynamic });
}

//*******EVENTOS*******

function loadCargaDadosEmissaoPassagem() {
    _cargaDadosEmissaoPassagem = new CargaDadosEmissaoPassagem();
    KoBindings(_cargaDadosEmissaoPassagem, "tabPassagemEntreEstados_" + _cargaAtual.DadosEmissaoFrete.id);
    $("#tabPassagemEntreEstados_" + _cargaAtual.DadosEmissaoFrete.id + "_li").show();
    _cargaDadosEmissaoPassagem.AlterarPercursoMDFe.enable(_cargaAtual.EtapaFreteEmbarcador.enable());
    if (_cargaAtual.ProblemaMDFe.val()) {
        _cargaDadosEmissaoPassagem.AlterarPercursoMDFe.enable(true);
    }

    _mapaCargaDadosEmissaoPassagem = new MapaMDFe();
    _mapaCargaDadosEmissaoPassagem.LoadMapaMDFe("mapa_" + _cargaAtual.DadosEmissaoFrete.id, function () {
        BuscarPercursoCargaDadosEmissaoPassagem();
        _cargaDadosEmissaoPassagem.Pedido.enable(_cargaAtual.EtapaFreteEmbarcador.enable());
        //loadCargaLocaisPrestacao();
    });

    if (_CONFIGURACAO_TMS.TipoServicoMultisoftware == EnumTipoServicoMultisoftware.MultiCTe) {
        if (!VerificaSePossuiPermissaoEspecial(EnumPermissaoPersonalizada.Carga_AlterarPassagens, _PermissoesPersonalizadasCarga)) {
            _cargaDadosEmissaoPassagem.AlterarPercursoMDFe.enable(false);
        }
    }

    if (_CONFIGURACAO_TMS.UtilizaEmissaoMultimodal) {
        $("#tabPassagemEntreEstados_" + _cargaAtual.DadosEmissaoFrete.id + "_li").hide();
    }
}

function alterarCargaDadosEmissaoPassagem(e, sender) {
    var valido = _mapaCargaDadosEmissaoPassagem.ValidarPassagens();
    if (valido) {
        e.Carga.val(_cargaAtual.Codigo.val());
        e.CargaPercursos.val(JSON.stringify(_mapaCargaDadosEmissaoPassagem.GetOrigensDestinos()));
        Salvar(e, "CargaLocaisPrestacao/AlterarPercursoPassagemMDFe", function (arg) {
            if (arg.Success) {
                if (arg.Data) {
                    exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, Localization.Resources.Cargas.Carga.PassagensEntreEstadosAtualizadaComSucesso);
                } else {
                    exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Atencao, arg.Msg);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, arg.Msg);
            }
        });
    }
}

//*******MÉTODOS*******

function BuscarPercursoCargaDadosEmissaoPassagem() {
    var data = { Carga: _cargaAtual.Codigo.val() };
    executarReST("CargaLocaisPrestacao/BuscarRotaPorCargaParaPasssagensMDFe", data, function (arg) {
        if (arg.Success) {
            if (arg.Data != false) {
                var retorno = arg.Data;
                if (retorno.EstadoOrigem != "EX" && retorno.EstadoDestino != "EX") {
                    _mapaCargaDadosEmissaoPassagem.LimparMapa();

                    _mapaCargaDadosEmissaoPassagem.SetEstadoDestino(retorno.EstadoDestino);

                    $.each(retorno.EstadosOrigensDestinos, function (i, estadosMDFe) {
                        if (estadosMDFe.Origem != null && estadosMDFe.Origem.Estado != "EX" && estadosMDFe.Destino.Estado != "EX")
                            _mapaCargaDadosEmissaoPassagem.AddOrigemDestino(estadosMDFe.Origem.Estado, estadosMDFe.Destino.Estado, estadosMDFe.Codigo);

                    });

                    $.each(retorno.rotasParaMDFe, function (i, rota) {
                        _mapaCargaDadosEmissaoPassagem.AddLocalidadesBuscaAPI(rota.Cidade + "," + rota.Estado);
                    });

                    $.each(retorno.passagens, function (i, estado) {
                        _mapaCargaDadosEmissaoPassagem.AddEstadoPassagem(estado.Sigla, estado.Posicao);
                    });

                    _mapaCargaDadosEmissaoPassagem.AtualizarDisplayMapa();
                }
            } else {
                exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.Aviso, arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, arg.Msg);
        }
    });
}