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
/// <reference path="../../../../js/plugin/dropzone/dropzone.js" />
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
/// <reference path="../Documentos/CTe.js" />
/// <reference path="../Documentos/MDFe.js" />
/// <reference path="../Documentos/NFS.js" />
/// <reference path="../Documentos/PreCTe.js" />
/// <reference path="CargaPedidoDocumentoCTe.js" />
/// <reference path="ConsultaReceita.js" />
/// <reference path="Documentos.js" />
/// <reference path="DropZone.js" />
/// <reference path="EtapaDocumentos.js" />
/// <reference path="NotaFiscal.js" />
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
/// <reference path="../../../Enumeradores/EnumTipoConsultaPortalFazenda.js" />
/// <reference path="../../../Enumeradores/EnumPermissoesEdicaoCTe.js" />
/// <reference path="../../../Enumeradores/EnumSituacaoMinutaAvon.js" />
/// <reference path="CTeDimensao.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _cteTerceiro, _CRUDCTeTerceiro, _CRUDCTeTerceiroIndicarPaletes;

var CTeTerceiro = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.CargaPedido = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });

    this.MetrosCubicos = PropertyEntity({ val: ko.observable(""), def: "", text: Localization.Resources.Cargas.Carga.MetrosCubicos.getFieldDescription(), enable: ko.observable(true), getType: typesKnockout.decimal, maxlength: 15, configDecimal: { precision: 6, allowZero: false, allowNegative: false } });
    this.Volumes = PropertyEntity({ val: ko.observable(""), def: "", text: Localization.Resources.Cargas.Carga.Volumes.getFieldDescription(), enable: ko.observable(true), getType: typesKnockout.int, maxlength: 7, configInt: { precision: 0, allowZero: true, thousand: '' } });

    this.Dimensoes = PropertyEntity({ type: types.listEntity, list: new Array(), required: false, codEntity: ko.observable(0), idGrid: guid(), text: Localization.Resources.Cargas.Carga.Dimensoes });
    this.QuantidadePaletes = PropertyEntity({ val: ko.observable(""), def: "", text: Localization.Resources.Cargas.Carga.QuantidadePaletes.getFieldDescription(), enable: ko.observable(true), getType: typesKnockout.int, maxlength: 7, configInt: { precision: 0, allowZero: true, thousand: '' } });


    //Variaveis Tradução DocumentosParaEmissao.html
    this.ExcluirTodasNotasFiscais = PropertyEntity({ text: Localization.Resources.Cargas.Carga.ExcluirTodasAsNotasFiscais });
    this.ExcluirTodosCTes = PropertyEntity({ text: Localization.Resources.Cargas.Carga.ExcluirTodosOsCTes });
    this.NFeEmitidaEmContigencia = PropertyEntity({ text: Localization.Resources.Cargas.Carga.NotaFiscalEmitidaEmContingenciaFSDA });
    this.DocumentoVinculadoEmOutraCarga = PropertyEntity({ text: Localization.Resources.Cargas.Carga.DocumentoVinculadoEmOutraCarga });
    this.EspelhoIntercement = PropertyEntity({ text: Localization.Resources.Cargas.Carga.EspelhoIntercement });        
    this.Validar = PropertyEntity({ text: Localization.Resources.Cargas.Carga.Validar });
    this.AdicionarDocumentosManualmente = PropertyEntity({ text: Localization.Resources.Cargas.Carga.AdicionarDocumentosManualmente });
    this.NotasNaoRecebidas = PropertyEntity({ text: Localization.Resources.Cargas.NotaNaoRecebidas });
    this.NotasCompativeisComCarga = PropertyEntity({ text: Localization.Resources.Cargas.Carga.NotasCompativeisComCarga });
    this.DropZone = PropertyEntity({ text: Localization.Resources.Cargas.Carga.Dropzone });
    this.MultiplasChaves = PropertyEntity({ text: Localization.Resources.Cargas.Carga.MultiplasChaves });
    this.EnvioDeArquivos = PropertyEntity({ text: Localization.Resources.Cargas.Carga.EnvioDeArquivos });
    this.LancamentoDeChaves = PropertyEntity({ text: Localization.Resources.Cargas.Carga.LancamentoDeChaves });
    this.EditarDadosDaNotaFiscal = PropertyEntity({ text: Localization.Resources.Cargas.Carga.EditarDadosDaNotaFiscal });
    this.Detalhes = PropertyEntity({ text: Localization.Resources.Gerais.Geral.Detalhes });
    this.EditarDadosDoCTeDeSubcontratacao = PropertyEntity({ text: Localization.Resources.Cargas.Carga.EditarDadosDoCTeDeSubcontratacao });

    this.CTesVinculadosOSMae = PropertyEntity({ text: Localization.Resources.Cargas.Carga.CTesVinculadosOSMae });
};

var CRUDCTeTerceiro = function () {
    this.Atualizar = PropertyEntity({ eventClick: atualizarCTeSubcontratacaoClick, type: types.event, text: Localization.Resources.Gerais.Geral.Atualizar, visible: ko.observable(true) });
};

var CRUDCTeTerceiroIndicarPaletes = function () {
    this.Atualizar = PropertyEntity({ eventClick: atualizarCTeSubcontratacaoIndicarPaletesClick, type: types.event, text: Localization.Resources.Gerais.Geral.Atualizar, visible: ko.observable(true) });
};

//*******EVENTOS*******

function loadCargaDocumentoCTe() {

    _cteTerceiro = new CTeTerceiro();
    KoBindings(_cteTerceiro, "knockoutDetalhesCTeTerceiro");

    _CRUDCTeTerceiro = new CRUDCTeTerceiro();
    KoBindings(_CRUDCTeTerceiro, "divCRUDCTeTerceiro");

    _cteTerceiro = new CTeTerceiro();
    KoBindings(_cteTerceiro, "knoutIndicarPaletesCTeTerceiro");

    _CRUDCTeTerceiroIndicarPaletes = new CRUDCTeTerceiroIndicarPaletes();
    KoBindings(_CRUDCTeTerceiroIndicarPaletes, "divCRUDCTeTerceiroIndicarPaletes");   

    

    LoadCTeTerceiroDimensao();
}

function iniciarEnvioCTeManual() {
    abrirModalCTeManual(0);
}

function abrirTelaEdicaoCTeSubcontratacao(e, sender) {
    abrirModalCTeManual(e.Codigo);
}

function abrirModalCTeManual(codigoCTeTerceiro) {
    iniciarControleManualRequisicao();
    var instancia = new EmissaoCTe(0, function () {
        finalizarControleManualRequisicao();
        instancia.CRUDCTe.Emitir.visible(false);
        instancia.CRUDCTe.Salvar.visible(true);
        instancia.CRUDCTe.Salvar.eventClick = function () {
            var objetoCTe = ObterObjetoCTe(instancia);
            AdicionarCTeTerceiro(objetoCTe, instancia);
        };
    }, null, true, false, _documentoEmissao.CargaPedido.val(), codigoCTeTerceiro);
}

function AdicionarCTeTerceiro(cte, instancia) {
    if (instancia.Validar() === true) {
        var dados = { CTe: cte, CargaPedido: _documentoEmissao.CargaPedido.val() };
        executarReST("DocumentoCTe/Adicionar", dados, function (arg) {
            if (arg.Success) {
                if (arg.Data !== false) {
                    instancia.FecharModal();
                    veririficarSeCargaMudouTipoContratacao(arg.Data);
                    exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, Localization.Resources.Cargas.Carga.CTeAtualizadoComSucesso);
                    carregarGridDocumentosParaEmissao();
                } else {
                    exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Atencao, arg.Msg, 60000);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, arg.Msg);
            }
        });
    }
}

function excluirCTeSubcontratacaoClick(e) {
    exibirConfirmacao(Localization.Resources.Gerais.Geral.Confirmacao, Localization.Resources.Cargas.Carga.RealmenteDesejaExcluirCTe.format(e.Numero), function () {
        var data = { Pedido: _documentoEmissao.Pedido.val(), CargaPedido: _documentoEmissao.CargaPedido.val(), Codigo: e.Codigo };
        executarReST("DocumentoCTe/Excluir", data, function (arg) {
            if (arg.Success) {
                if (arg.Data !== false) {
                    exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, Localization.Resources.Cargas.Carga.CTExcluidoComSucesso);
                    if (arg.Data == _TipoContratacaoCargaAtual) {
                        carregarGridDocumentosParaEmissao();
                        limparCamposDocumentosParaEmissao();
                    } else {
                        _cargaAtual.TipoContratacaoCarga.val(arg.Data);
                        carregarDocumentosParaEmissaoPedido(0);
                    }
                } else {
                    exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Atencao, arg.Msg, 20000);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, arg.Msg);
            }
        });
    });
}

function excluirCTesSubContratacaoClick(e, sender) {
    exibirConfirmacao(Localization.Resources.Gerais.Geral.Confirmacao, Localization.Resources.Cargas.Carga.DesejaRealmenteExcluirTodosOsCTes, function () {
        var data = { Carga: _cargaAtual.Codigo.val(), CargaPedido: _documentoEmissao.CargaPedido.val() };
        executarReST("DocumentoCTe/ExcluirTodosCTes", data, function (arg) {
            if (arg.Success) {
                if (arg.Data != false) {
                    exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, Localization.Resources.Cargas.Carga.OsCTesForamExcluidosComSucesso);
                    carregarGridDocumentosParaEmissao();
                } else {
                    exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Atencao, arg.Msg, 20000);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, arg.Msg);
            }
        });
    });
}

function abrirTelaAtualizarDimensaoCTeSubcontratacao(e) {
    limparCargaDocumentoCTe();

    _cteTerceiro.Codigo.val(e.Codigo);

    BuscarPorCodigo(_cteTerceiro, "DocumentoCTe/BuscarPorCodigo", function (r) {
        if (r.Success) {
            if (r.Data) {
                RecarregarGridCTeTerceiroDimensao();

                if (_cargaAtual.SituacaoCarga.val() != EnumSituacoesCarga.AgNFe) {
                    SetarEnableCamposKnockout(_cteTerceiro, false);

                    _CRUDCTeTerceiro.Atualizar.visible(false);
                }

                Global.abrirModal("divEdicaoCTeTerceiro");
                $("#divEdicaoCTeTerceiro").on('hidden.bs.modal', function () {
                    limparCargaDocumentoCTe();
                });

            } else {
                exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Atencao, arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, arg.Msg);
        }
    });
}

function atualizarCTeSubcontratacaoClick() {
    _cteTerceiro.CargaPedido.val(_documentoEmissao.CargaPedido.val());

    Salvar(_cteTerceiro, "DocumentoCTe/Atualizar", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, Localization.Resources.Cargas.Carga.CTeAtualizadoComSucesso);
                //carregarGridDocumentosParaEmissao();
                Global.fecharModal("divEdicaoCTeTerceiro");
                limparCargaDocumentoCTe();
            } else {
                exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Atencao, arg.Msg, 200000);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, arg.Msg);
        }
    });
}
function abrirTelaIndicarPaletes(e) {
    limparCargaDocumentoCTe();

    _cteTerceiro.Codigo.val(e.Codigo);

    BuscarPorCodigo(_cteTerceiro, "DocumentoCTe/BuscarPorCodigo", function (r) {
        if (r.Success) {
            if (r.Data) {

                if (_cargaAtual.SituacaoCarga.val() != EnumSituacoesCarga.AgNFe) {
                    SetarEnableCamposKnockout(_cteTerceiro, false);

                    CRUDCTeTerceiroIndicarPaletes.Atualizar.visible(false);
                }

                Global.abrirModal("divEdicaoCTeTerceiroIndicarPaletes");
                $("#divEdicaoCTeTerceiroIndicarPaletes").on('hidden.bs.modal', function () {
                    limparCargaDocumentoCTe();
                });

            } else {
                exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Atencao, arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, arg.Msg);
        }
    });
}
function atualizarCTeSubcontratacaoIndicarPaletesClick() {
    _cteTerceiro.CargaPedido.val(_documentoEmissao.CargaPedido.val());
    Salvar(_cteTerceiro, "DocumentoCTe/AtualizarIndicarPaletes", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, Localization.Resources.Cargas.Carga.CTeAtualizadoComSucesso);
                //carregarGridDocumentosParaEmissao();
                Global.fecharModal("divEdicaoCTeTerceiroIndicarPaletes");
                limparCargaDocumentoCTe();
            } else {
                exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Atencao, arg.Msg, 200000);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, arg.Msg);
        }
    });
}

function limparCargaDocumentoCTe() {
    LimparCampos(_cteTerceiro);
    LimparCamposCTeTerceiroDimensao();
    RecarregarGridCTeTerceiroDimensao();
}