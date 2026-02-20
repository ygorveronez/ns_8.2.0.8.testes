// <autosync enabled="true" />
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
/// <reference path="IntegracaoCarga.js" />
/// <reference path="IntegracaoCTe.js" />
/// <reference path="IntegracaoEDI.js" />
/// <reference path="Avon/IntegracaoMinutaAvon.js" />
/// <reference path="Avon/IntegracaoMinutaAvonSignalR.js" />
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
/// <reference path="../../../Enumeradores/EnumSituacaoAutorizacaoIntegracaoCTe.js" />

var _container;
var _gridAverbacaoContainer;
var _gridValePedagiContainer;
var _gridDocumentoContainer;
//*******EVENTOS*******

var Container = function () {
    this.Carga = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.RetornarDocumentoOperacaoContainer = PropertyEntity({ val: ko.observable(true), def: true, getType: typesKnockout.bool });

    this.Apolice = PropertyEntity({ text: Localization.Resources.Cargas.Carga.Apolice.getFieldDescription(), val: ko.observable(""), def: "", enable: ko.observable(true), visible: ko.observable(true) });
    this.NumeroCTe = PropertyEntity({ text: Localization.Resources.Gerais.Geral.Numero.getFieldDescription(), val: ko.observable(""), def: "", enable: ko.observable(true), visible: ko.observable(true), getType: typesKnockout.int, configInt: { precision: 0, allowZero: false, thousands: "" } });
    this.SituacaoAverbacao = PropertyEntity({ val: ko.observable(EnumStatusAverbacaoCTe.Todos), enable: ko.observable(true), visible: ko.observable(true), text: Localization.Resources.Cargas.Carga.SituacaoDaAverbacao.getFieldDescription(), options: EnumStatusAverbacaoCTe.obterOpcoesPesquisa(), def: EnumStatusAverbacaoCTe.Todos });
    this.SituacaoIntegracaoValePedagio = PropertyEntity({ val: ko.observable(""), enable: ko.observable(true), visible: ko.observable(true), text: Localization.Resources.Cargas.Carga.SituacaoIntegracao.getFieldDescription(), options: EnumSituacaoIntegracaoCarga.obterOpcoesPesquisa(), def: "" });
    this.PesquisarIntegracaoValePedagio = PropertyEntity({ eventClick: function (e) { _gridValePedagiContainer.CarregarGrid(); }, type: types.event, text: Localization.Resources.Gerais.Geral.Pesquisar, idGrid: guid(), idGridNFS: guid(), visible: ko.observable(true), enable: ko.observable(true) });
    this.LiberarComProblemaValePedagio = PropertyEntity({ eventClick: LiberarComProblemaValePedagioContainerClick, enable: ko.observable(true), type: types.event, text: Localization.Resources.Cargas.Carga.AvancarEtapa, visible: ko.observable(true) });
    this.LiberarSemValePedagio = PropertyEntity({ eventClick: LiberarComProblemaValePedagioContainerClickSemFalha, enable: ko.observable(true), type: types.event, text: "Liberar Sem Vale Pedagio", visible: ko.observable(false) });
    this.NumeroContainer = PropertyEntity({ text: Localization.Resources.Cargas.Carga.NumeroContainer.getFieldDescription(), val: ko.observable(""), def: "", enable: ko.observable(true), visible: ko.observable(true), getType: typesKnockout.int, configInt: { precision: 0, allowZero: false, thousands: "" } });
    this.ValorAdiantamentoContainer = PropertyEntity({ text: Localization.Resources.Cargas.Carga.ValorAdiantamentoContainer.getFieldDescription(), val: ko.observable(""), def: "", enable: VerificaSePossuiPermissaoEspecial(EnumPermissaoPersonalizada.Carga_PermitirAlterarValorAdiantamentoEtapaContainer, _PermissoesPersonalizadasCarga), visible: ko.observable(true), getType: typesKnockout.int, configInt: { precision: 0, allowZero: false, thousands: "" } });

    this.PesquisarCTeAverbacoes = PropertyEntity({
        eventClick: function (e) {
            _gridAverbacaoContainer.CarregarGrid();
        }, type: types.event, text: Localization.Resources.Gerais.Geral.Pesquisar, idGrid: guid(), idGridNFS: guid(), visible: ko.observable(true), enable: ko.observable(true)
    });
    this.EmitirNovamenteAverbacoesPendentesCTe = PropertyEntity({
        eventClick: reaverbarPendentesContainerClick, type: types.event, text: Localization.Resources.Cargas.Carga.ReaverbarPendentes, idGrid: guid(), visible: ko.observable(false), enable: ko.observable(true)
    });
    this.EmitirNovamenteAverbacoesCTe = PropertyEntity({
        eventClick: reaverbarContainerRejeitadosClick, type: types.event, text: Localization.Resources.Cargas.Carga.ReaverbarRejeitados, idGrid: guid(), visible: ko.observable(false), enable: ko.observable(true)
    });

    this.GerarPagamentoAdiantamentoContainer = PropertyEntity({
        eventClick: gerarPagamentoAdiantamentoContainerClick, type: types.event, text: Localization.Resources.Cargas.Carga.GerarPagamentoAdiantamentoContainer, idGrid: guid(), visible: ko.observable(true), enable: ko.observable(true)
    });

    this.AtualizarValorAdiantamentoContainer = PropertyEntity({
        eventClick: atualizarValorAdiantamentoContainerClick, type: types.event, text: Localization.Resources.Cargas.Carga.AtualizarValorAdiantamentoContainer, idGrid: guid(), visible: ko.observable(true), enable: ko.observable(true)
    });
    this.Pesquisar = PropertyEntity({
        eventClick: function (e) {
            _gridDocumentoContainer.CarregarGrid();
        }, type: types.event, text: Localization.Resources.Gerais.Geral.Pesquisar, idGrid: guid(), idGridNFS: guid(), visible: ko.observable(true), enable: ko.observable(true)
    });
}

function LoadEtapaContainer(e, sender) {

    var strKnoutCargaSubContratacao = "divCargaOperacaoContainer" + e.EtapaContainer.idGrid;

    $("#" + e.EtapaContainer.idGrid).html(_HTMLOperacaoContainer.replaceAll("#divCargaOperacaoContainer", strKnoutCargaSubContratacao));

    _container = new Container();
    _container.Carga.val(e.Codigo.val());
    KoBindings(_container, strKnoutCargaSubContratacao);  
    LocalizeCurrentPage();

    if (!e.TipoOperacao.GerarPagamentoMotoristaAutomaticamenteComValorAdiantamentoContainer || !e.FreteDeTerceiro.val())
        $("#liContainerAdiantamento_divCargaOperacaoContainer" + e.EtapaContainer.idGrid).hide();
    else
        obterValorAdiantamentoGerado(e);

    if (!e.TipoOperacao.AverbarContainerComAverbacaoCarga)
        $("#liContainerAverbacao_divCargaOperacaoContainer" + e.EtapaContainer.idGrid).hide();

    if (!e.TipoOperacao.ComprarValePedagioEtapaContainer)
        $("#liContainerValePedagio_divCargaOperacaoContainer" + e.EtapaContainer.idGrid).hide();

    buscarAverbacaoContainer(e);
    buscarValePedagioContainer(e);
    buscarCargasDocumentoContainer(e);
}

function buscarAverbacaoContainer(cargaAtual) {
    var email = { descricao: Localization.Resources.Cargas.Carga.EmailProvedor, id: guid(), metodo: enviarEmailAverbacaoCTeClick, icone: "", visibilidade: VisibilidadeEnviarEmailProvedor };
    var averbar = { descricao: Localization.Resources.Cargas.Carga.Averbar, id: guid(), metodo: reemitirAverbacaoCTeClick, icone: "", visibilidade: VisibilidadeReemitirAverbacao };
    var auditar = { descricao: Localization.Resources.Gerais.Geral.Auditar, id: guid(), metodo: OpcaoAuditoria("AverbacaoCTe"), icone: "", visibilidade: VisibilidadeOpcaoAuditoria  };
    var historico = { descricao: Localization.Resources.Cargas.Carga.HistoricoDeIntegracao, id: guid(), metodo: ExibirHistoricoIntegracaoAverbacao, icone: "" };
    var menuOpcoes = { tipo: TypeOptionMenu.list, descricao: Localization.Resources.Gerais.Geral.Opcoes, tamanho: 7, opcoes: [averbar, historico, auditar, email] };

    _gridAverbacaoContainer = new GridView(_container.PesquisarCTeAverbacoes.idGrid, "CargaCTe/ConsultarCargaCTeAverbacao", _container, menuOpcoes);
    _gridAverbacaoContainer.CarregarGrid();

    // Exibe botão de reenvio de averbações
    if (cargaAtual.ProblemaAverbacaoCTe.val())
        _container.EmitirNovamenteAverbacoesCTe.visible(true);
    else
        _container.EmitirNovamenteAverbacoesCTe.visible(false);
}

function buscarCargasDocumentoContainer(cargaAtual) {

    var editarColuna = { permite: true, callback: callbackEditarColuna, atualizarRow: false };
    _gridDocumentoContainer = new GridView(_container.Pesquisar.idGrid, "CargaCTe/ConsultarCargaCTe", _container, obterMenuOpcoesCargasCTeContainer(cargaAtual), null, null, null, null, null, null, null, editarColuna);
    _gridDocumentoContainer.CarregarGrid();
    
}

function buscarValePedagioContainer(cargaAtual) {
    var reenviarIntegracao = { descricao: Localization.Resources.Cargas.Carga.Integrar, id: guid(), metodo: integrarValePedagioContainerClick, icone: "", visibilidade: VisibilidadeReemitirIntegracaoValePedagio };
    var imprimirPortal = { descricao: Localization.Resources.Cargas.Carga.Imprimir, id: guid(), metodo: ImpressaoValePedagioViaPortalClick, icone: "", visibilidade: VisibilidadeReemitirIntegracaoValePedagioViaPortal };
    var imprimir = { descricao: Localization.Resources.Cargas.Carga.Imprimir, id: guid(), metodo: ImpressaoValePedagioClick, icone: "", visibilidade: VisibilidadeImprimirIntegracaoValePedagio };
    var cancelarValePedagioNaCarga = { descricao: Localization.Resources.Gerais.Geral.Cancelar, id: guid(), metodo: CancelarValePedagioNaCargaClick, icone: "", visibilidade: VisibilidadeCancelarValePedagioNaCarga };
    var historico = { descricao: Localization.Resources.Cargas.Carga.HistoricoDeIntegracao, id: guid(), metodo: ExibirHistoricoIntegracaoValePedagio, icone: "", visibilidade: VisibilidadeHistoricoIntegracao };
    var auditar = { descricao: Localization.Resources.Gerais.Geral.Auditar, id: guid(), metodo: OpcaoAuditoria("CargaIntegracaoValePedagio"), icone: "", visibilidade: VisibilidadeOpcaoAuditoria };

    _container.SituacaoIntegracaoValePedagio.visible(true);

    var menuOpcoes = { tipo: TypeOptionMenu.list, descricao: Localization.Resources.Gerais.Geral.Opcoes, tamanho: 7, opcoes: [imprimir, imprimirPortal, reenviarIntegracao, cancelarValePedagioNaCarga, historico, auditar] };

    var isProblemaValePedagio = _CONFIGURACAO_TMS.TipoServicoMultisoftware != EnumTipoServicoMultisoftware.MultiCTe && (cargaAtual.ProblemaIntegracaoValePedagio.val() || cargaAtual.LiberadoComProblemaValePedagio.val() || cargaAtual.NaoPermitirLiberarSemValePedagio.val());


    _gridValePedagiContainer = new GridView(_container.PesquisarIntegracaoValePedagio.idGrid, "CargaIntegracaoValePedagio/ConsultarCargaValePedagio", _container, menuOpcoes);
    _gridValePedagiContainer.CarregarGrid();

    if (isProblemaValePedagio) {
        _container.LiberarComProblemaValePedagio.visible(true);

        _container.LiberarSemValePedagio.visible(true);
        _container.LiberarSemValePedagio.enable(!cargaAtual.LiberadoComProblemaValePedagio.val() || cargaAtual.NaoPermitirLiberarSemValePedagio.val());

        if (cargaAtual.LiberadoComProblemaValePedagio.val())
            _container.LiberarComProblemaValePedagio.enable(false);
        else
            _container.LiberarComProblemaValePedagio.enable(true);
    } else {
        _container.LiberarComProblemaValePedagio.visible(false);
        _container.LiberarSemValePedagio.visible(false);
    }
    
}
function integrarValePedagioContainerClick(datagrid) {
    var data = {
        Codigo: datagrid.Codigo,
        Carga: _container.Carga.val()
    }

    executarReST("CargaIntegracaoValePedagio/ReenviarIntegracaoRejeitadas", data, function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                if (_isPreCte) {
                    _gridCargaIntegracaoValePedagioPreCTe.CarregarGrid();
                } else {
                    _gridCargaIntegracaoValePedagio.CarregarGrid();
                }
            } else {
                exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Atencao, arg.Msg, 20000);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, arg.Msg);
        }
    });
}
function LiberarComProblemaValePedagioContainerClickSemFalha(cargaAtual) {
    var data = {
        Carga: _container.Carga.val()
    }
    executarReST("CargaIntegracaoValePedagio/LiberarComProblemaValePedagio", data, function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                _container.LiberarSemValePedagio.enable(false);
            } else {
                exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Atencao, arg.Msg, 20000);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, arg.Msg);
        }
    });
}
function LiberarComProblemaValePedagioContainerClick(cargaAtual) {
    var data = {
        Carga: _container.Carga.val()
    }
    exibirConfirmacao(Localization.Resources.Cargas.Carga.AvancarEtapa, Localization.Resources.Cargas.Carga.VoceTemCertezaQueDesejaAvancarEtapaMesmoQueTenhaValePedagiosComFalhaNaIntegracao, function () {
        executarReST("CargaIntegracaoValePedagio/LiberarComProblemaValePedagio", data, function (arg) {
            if (arg.Success) {
                if (arg.Data) {
                    _container.LiberarComProblemaValePedagio.enable(false);
                } else {
                    exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Atencao, arg.Msg, 20000);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, arg.Msg);
            }
        });
    });
}
function atualizarValorAdiantamentoContainerClick(data) {
    var data = {
        Carga: _container.Carga.val()
    }
    executarReST("CargaFreteTerceiro/RecalcularValorFreteSubContratacaoEtapaContainer", data, function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                _container.ValorAdiantamentoContainer.val(Globalize.format(arg.Data.ValorAdiantamento, "n2"));
                exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, Localization.Resources.Cargas.Carga.ValorAlteradoComSucesso);
            } else {
                exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Atencao, arg.Msg, 20000);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, arg.Msg);
        }
    });
}
function obterValorAdiantamentoGerado(data) {
    var data = {
        Carga: _container.Carga.val()
    }
    executarReST("CargaFreteTerceiro/ObterValorAdiantamentoEtapaContainer", data, function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                _container.ValorAdiantamentoContainer.val(Globalize.format(arg.Data.ValorAdiantamento, "n2"));
            } else {
                exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Atencao, arg.Msg, 20000);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, arg.Msg);
        }
    });
}
function gerarPagamentoAdiantamentoContainerClick() {
    var data = {
        Carga: _container.Carga.val(),
        ValorAdiantamento: _container.ValorAdiantamentoContainer.val()
    }
    executarReST("CargaFreteTerceiro/GerarAdiantamentoTerceiroContainer", data, function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, Localization.Resources.Cargas.Carga.GeradoAdiantamentoComSucesso);
            } else {
                exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Atencao, arg.Msg, 20000);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, arg.Msg);
        }
    });
}
function alterarObservacaoCTeContainerClick(data) {
    LimparDadosAlterarObservacao();

    _alterarObservacao.CodigoCTe.val(data.CodigoCTE);
    _alterarObservacao.CodigoCargaCTe.val(data.Codigo);
    _alterarObservacao.Observacao.val(data.Observacao);

    Global.abrirModal("divModalAlterarObservacao");
}
function informarContainerCTeContainerClick(data) {
    LimparDadosInformacaoContainer();

    _informarContainer.CodigoCTe.val(data.CodigoCTE);
    _informarContainer.CodigoCargaCTe.val(data.Codigo);

    Global.abrirModal("divModalInformarContainer");
}
function obterMenuOpcoesCargasCTeContainer(cargaAtual) {
    var baixarEDI = { descricao: Localization.Resources.Cargas.Carga.BaixarEdi, id: guid(), metodo: baixarEDIClick, icone: "", visibilidade: VisibilidadeOpcaoDownloadEDI };
    var baixarXMLNFSe = { descricao: Localization.Resources.Cargas.Carga.BaixarXml, id: guid(), metodo: baixarXMLCTeClick, icone: "", visibilidade: VisibilidadeOpcaoDownloadDANFSE };
    var baixarDANFSE = { descricao: Localization.Resources.Cargas.Carga.BaixarDanfse, id: guid(), metodo: baixarDacteClick, icone: "", visibilidade: VisibilidadeOpcaoDownloadDANFSE };
    var baixarDACTEComp = { descricao: Localization.Resources.Cargas.Carga.BaixarDacteComp, id: guid(), metodo: baixarDacteCompClick, icone: "", visibilidade: VisibilidadeOpcaoDownload };
    var baixarDACTE = { descricao: Localization.Resources.Cargas.Carga.BaixarDacte, id: guid(), metodo: baixarDacteClick, icone: "", visibilidade: VisibilidadeOpcaoDownload };
    var baixarPDF = { descricao: Localization.Resources.Cargas.Carga.BaixarPdf, id: guid(), metodo: baixarDacteClick, icone: "", visibilidade: VisibilidadeDownloadOutrosDoc };
    var baixarXML = { descricao: Localization.Resources.Cargas.Carga.BaixarXml, id: guid(), metodo: baixarXMLCTeClick, icone: "", visibilidade: VisibilidadeOpcaoDownload };
    var retornoSefaz = { descricao: Localization.Resources.Cargas.Carga.MensagemSefaz, id: guid(), metodo: retoronoSefazClick, icone: "", visibilidade: VisibilidadeMensagemSefaz };
    var editar = { descricao: Localization.Resources.Gerais.Geral.Editar, id: guid(), metodo: editarCTeClick, icone: "", visibilidade: VisibilidadeOpcaoEditar };
    var visualizar = { descricao: Localization.Resources.Gerais.Geral.Detalhes, id: guid(), metodo: detalhesCTeClick, icone: "", visibilidade: VisibilidadeOpcaoDownload };
    var auditar = { descricao: Localization.Resources.Gerais.Geral.Auditar, id: guid(), metodo: OpcaoAuditoria("CargaCTe"), icone: "", visibilidade: VisibilidadeOpcaoAuditoria };
    var emitirCCe = { descricao: Localization.Resources.Cargas.Carga.CartaDeCorrecao, id: guid(), metodo: EmitirCartaCorrecaoCTeClick, icone: "", visibilidade: VisibilidadeOpcaoCartaCorrecaoCTe };
    var informarContainer = { descricao: Localization.Resources.Cargas.Carga.InformarContainer, id: guid(), metodo: informarContainerCTeContainerClick, icone: "", visibilidade: VisibilidadeInformarContainer };
    var alterarObservacao = { descricao: Localization.Resources.Cargas.Carga.AlterarObservacao, id: guid(), metodo: alterarObservacaoCTeContainerClick, icone: "", visibilidade: VisibilidadeAlterarObservacao };
    var emitir = {
        descricao: Localization.Resources.Cargas.Carga.Emitir, id: guid(), metodo: function (datagrid) {
            emitirCTeClick(datagrid, cargaAtual);
        }, icone: "", visibilidade: VisibilidadeRejeicao
    };
    var protocoloIntegracao = { descricao: Localization.Resources.Cargas.Carga.ProtocoloDeIntegracao, id: guid(), metodo: protocoloIntegracaoClick, icone: "", visibilidade: VisibilidadeProtocoloIntegracao };
    let anexosContribuinte = { descricao: Localization.Resources.Cargas.Carga.AnexosContribuinte, id: guid(), metodo: abrirAnexosContribuinteClick, icone: "", visibilidade: (datagrid) => VisibilidadeInformacaoContribuinte(datagrid, true) };
    let aprovarDocContribuinte = { descricao: Localization.Resources.Cargas.Carga.AprovarDocumentoContribuinte, id: guid(), metodo: aprovarDocumentoContribuinteClick, icone: "", visibilidade: VisibilidadeInformacaoContribuinte };
    let reprovarDocContribuinte = { descricao: Localization.Resources.Cargas.Carga.ReprovarDocumentoContribuinte, id: guid(), metodo: reprovarDocumentoContribuinteClick, icone: "", visibilidade: VisibilidadeInformacaoContribuinte };
    let sincronizarDocumento = { descricao: Localization.Resources.Cargas.Carga.SincronizarDocumento, id: guid(), metodo: function (datagrid) { sincronizarCTeClick(datagrid, cargaAtual); }, visibilidade: VisibilidadeSincronizarDocumento };
    let DesvincularCTeGerarCopia = { descricao: Localization.Resources.Cargas.Carga.DesvincularCTeEGerarCopia, id: guid(), metodo: function (datagrid) { desvincularCTeEGerarCopiaClick(datagrid, cargaAtual); }, visibilidade: VisibilidadeDesvincularCTeEGerarCopia };
    let baixarXMLMigrate = { descricao: "Baixar XML Migrate", id: guid(), metodo: baixarXMLMigrateClick, icone: "", visibilidade: VisibilidadeOpcaoDownloadMigrate };

    var menuOpcoes = { tipo: TypeOptionMenu.list, descricao: Localization.Resources.Gerais.Geral.Opcoes, tamanho: 7, opcoes: [] };

    if (!cargaAtual.PossuiOcultarInformacoesCarga.val()) {
        menuOpcoes.opcoes.push(baixarDACTE);
        menuOpcoes.opcoes.push(baixarXML);
        menuOpcoes.opcoes.push(baixarXMLMigrate);
        menuOpcoes.opcoes.push(baixarDANFSE);
        menuOpcoes.opcoes.push(baixarXMLNFSe);
        menuOpcoes.opcoes.push(baixarPDF);
        menuOpcoes.opcoes.push(visualizar);
        menuOpcoes.opcoes.push(baixarDACTEComp);
    }

    menuOpcoes.opcoes.push(baixarEDI);
    menuOpcoes.opcoes.push(sincronizarDocumento);
    menuOpcoes.opcoes.push(DesvincularCTeGerarCopia);
    menuOpcoes.opcoes.push(retornoSefaz);
    menuOpcoes.opcoes.push(emitir);
    menuOpcoes.opcoes.push(editar);

    if (VerificaSePossuiPermissaoEspecial(EnumPermissaoPersonalizada.Carga_EmitirCartaCorrecao, _PermissoesPersonalizadasCarga))
        menuOpcoes.opcoes.push(emitirCCe);

    menuOpcoes.opcoes.push(informarContainer);
    menuOpcoes.opcoes.push(alterarObservacao);
    menuOpcoes.opcoes.push(auditar);
    menuOpcoes.opcoes.push(protocoloIntegracao);

    if (_CONFIGURACAO_TMS.TipoServicoMultisoftware == EnumTipoServicoMultisoftware.MultiEmbarcador) {
        menuOpcoes.opcoes.push(anexosContribuinte);
        menuOpcoes.opcoes.push(aprovarDocContribuinte);
        menuOpcoes.opcoes.push(reprovarDocContribuinte);
    }

    if (_CONFIGURACAO_TMS.TipoServicoMultisoftware == EnumTipoServicoMultisoftware.MultiCTe && VerificaSePossuiPermissaoEspecial(EnumPermissaoPersonalizada.Carga_PermitirAnexoDocumentoNaoContribuinte, _PermissoesPersonalizadasCarga))
        menuOpcoes.opcoes.push(anexosContribuinte);

    return menuOpcoes;
}


function reaverbarPendentesContainerClick(cargaAtual) {
    exibirConfirmacao(Localization.Resources.Gerais.Geral.Atencao, Localization.Resources.Cargas.Carga.DesejaRealmenteReaverbarTodasAsAverbacoesPendentesDestaCarga, function () {
        var dados = {
            Carga: _container.Carga.val()
        }

        executarReST("CargaCTe/ReaverbarPendentes", dados, function (retorno) {
            if (retorno.Success) {
                if (retorno.Data) {
                    exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, retorno.Msg, 20000);

                    _gridAverbacaoContainer.CarregarGrid();
                } else {
                    exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Atencao, retorno.Msg, 20000);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, retorno.Msg);
            }
        });
    });
}
function reaverbarContainerRejeitadosClick(cargaAtual) {
    exibirConfirmacao(Localization.Resources.Gerais.Geral.Atencao, Localization.Resources.Cargas.Carga.DesejaRealmenteReaverbarTodasAsAverbacoesRejeitadasDestaCarga, function () {
        cargaAtual.PossuiPendencia.val(false);
        EtapaCTeNFsAguardando(cargaAtual);
        $("#" + cargaAtual.EtapaCTeNFs.idGrid + " .DivMensagemAverbacaoCTe").hide();
        _container.EmitirNovamenteAverbacoesCTe.visible(false);

        var data = {
            Carga: cargaAtual.Codigo.val()
        }

        executarReST("CargaCTe/ReaverbarRejeitadas", data, function (arg) {
            if (arg.Success) {
                if (arg.Data) {
                    _gridAverbacaoContainer.CarregarGrid();
                } else {
                    exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Atencao, arg.Msg, 20000);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, arg.Msg);
            }
        });
    });
}