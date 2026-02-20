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
/// <reference path="Geral.js" />
/// <reference path="Lacre.js" />
/// <reference path="LocaisPrestacao.js" />
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

//*******MAPEAMENTO*******

var _HTMLDadosEmissao = "";

//*******EVENTOS*******

function loadCargaDadosEmissao(callback) {
    _cargaAtual.DadosEmissaoFrete.visible(true);
    loadPedidoVinculado();
    var strKnoutDadosEmissao = "knoutDadosEmissao" + _cargaAtual.DadosEmissaoFrete.id;
    $("#" + _cargaAtual.DadosEmissaoFrete.id).html(_HTMLDadosEmissao.replace("#knoutDadosEmissao", strKnoutDadosEmissao).replace(/#CodigoDadosEmissao/g, _cargaAtual.DadosEmissaoFrete.id));
    loadCargaDadosEmissaoPassagem();
    loadCargaDadosEmissaoConfiguracaoPedidos();

    if (_CONFIGURACAO_TMS.TipoServicoMultisoftware != EnumTipoServicoMultisoftware.MultiCTe) {
        if (!_cargaAtual.TipoOperacao.permitirAdicionarObservacaoNaEtapaUmDaCarga)
            loadCargaDadosEmissaoObservacao();

        loadCargaDadosEmissaoObservacaoFiscoContribuinte();
        loadIsca();
        LoadCargaDadosEmissaoLacre();
        LoadCargaDadosEmissaoValePedagio();
        loadCargaPercurso();
        LocalizeCurrentPage();
    }

    Global.ResetarAba("divValores_" + _cargaAtual.DadosEmissaoFrete.id);

    if (_CONFIGURACAO_TMS.TipoServicoMultisoftware == EnumTipoServicoMultisoftware.MultiTMS) {
        Global.ExibirAba("tabGeral_" + _cargaAtual.DadosEmissaoFrete.id);

        loadCargaDadosEmissaoSeguro(function () {
            loadCargaDadosEmissaoGeral();
            loadCargaDadosEmissaoConfiguracao();
            loadCargaDadosEmissaoRota();
            obterDadosEmissaoGeralCarga();
            if (callback != null)
                callback();
        });
    } else {
        if (_CONFIGURACAO_TMS.PermiteAdicionarNotaManualmente)
            loadRoteirizador();

        if (_CONFIGURACAO_TMS.TipoServicoMultisoftware == EnumTipoServicoMultisoftware.MultiEmbarcador) {

            Global.ExibirAba("tabConfiguracoes_" + _cargaAtual.DadosEmissaoFrete.id);

            loadCargaDadosEmissaoConfiguracao();
            loadCargaDadosEmissaoGeral();
            loadCargaDadosEmissaoSeguro(function () {
                obterDadosEmissaoGeralCarga();
                _cargaDadosEmissaoConfiguracao.TipoRateio.visible(false);
                _cargaDadosEmissaoConfiguracao.FormulaRateio.visible(false);
                _cargaDadosEmissaoConfiguracaoPedidos.TipoRateio.visible(false);
                _cargaDadosEmissaoConfiguracaoPedidos.FormulaRateio.visible(false);
            });
        } else if (_CONFIGURACAO_TMS.TipoServicoMultisoftware == EnumTipoServicoMultisoftware.MultiCTe) {
            Global.ExibirAba("tabPassagemEntreEstados_" + _cargaAtual.DadosEmissaoFrete.id);

            if (_cargaAtual.PermitirTransportadorInformarObservacaoImpressaoCarga) {
                if (!_cargaAtual.TipoOperacao.permitirAdicionarObservacaoNaEtapaUmDaCarga) {
                    $("#tabObservacoes_" + _cargaAtual.DadosEmissaoFrete.id + "_li").show();
                    loadCargaDadosEmissaoObservacao();
                }
                obterDadosEmissaoGeralCarga();
            }
        } else {
            Global.ExibirAba("tabObservacoes_" + _cargaAtual.DadosEmissaoFrete.id);
        }
        if (callback != null)
            callback();
    }
}

function obterDadosEmissaoGeralCarga(callback) {
    executarReST("DadosEmissao/ObterInformacoesGeraisCarga", { Carga: _cargaAtual.Codigo.val() }, function (r) {
        if (r.Success && r.Data) {
            preencherCargaDadosEmissaoObservacao(r);
            preencherCargaDadosEmissaoObservacaoFiscoContribuinte(r);
            preencherCargaDadosEmissaoIsca(r);
            preencherCargaDadosEmissaoConfiguracao(r);
            preencherCargaDadosEmissaoSeguro(r);

            if (_CONFIGURACAO_TMS.TipoServicoMultisoftware == EnumTipoServicoMultisoftware.MultiTMS)
                preencherCargaDadosEmissaoRota(r);

            if (_CONFIGURACAO_TMS.TipoServicoMultisoftware != EnumTipoServicoMultisoftware.MultiCTe)
                preecherrCargaDadosEmissaoGeral(r);

            if (_cargaDadosEmissaoGeral) {
                _cargaDadosEmissaoGeral.FilialCargaAgrupada.options(retornarListaFiliais(r.Data.FiliaisCargasAgrupadas));

                _cargaDadosEmissaoGeral.FilialCargaAgrupada.val(r.Data.FilialCargaAgrupada);
            }

            if (callback)
                callback();
        } else {
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, r.Msg);
        }
    });
}

function carregarDadosPedido(indicePedido, callback) {
    retornarDadosCargaPedido(indicePedido, function (pedido) {
        if (_cargaDadosEmissaoGeral != null) {
            preencherTabsPedidos(_cargaDadosEmissaoGeral.Pedido.idTab, "carregarDadosEmissaoGeral", indicePedido, _cargaDadosEmissaoGeral.Pedido.enable(), _cargaDadosEmissaoGeral.AplicarGeralEmTodosPedidos, visualizarAtualizacaoNumeroPedido);
            _indiceGlobalPedidoDadosEmissao = indicePedido;
            preecherDadosEmissao(pedido);
        }

        if (_cargaDadosEmissaoConfiguracao != null) {
            preencherTabsPedidos(_cargaDadosEmissaoConfiguracao.Pedido.idTab, "carregarDadosEmissaoConfiguracao", indicePedido, _cargaDadosEmissaoConfiguracao.Pedido.enable(), _cargaDadosEmissaoConfiguracao.AplicarConfiguracaoEmTodosPedidos);
            _indiceGlobalPedidoConfiguracaoEmissao = indicePedido;
            preencherDadosPedidoConfiguracao(pedido);
        }

        if (callback != null)
            callback();
    });
}

function retornarDadosCargaPedido(indicePedido, callback) {
    var data = _cargaAtual.Pedidos.val[indicePedido];
    executarReST("Carga/BuscarDadosCargaPedido", data, function (arg) {
        if (arg.Success) {
            var pedido = arg.Data;
            callback(pedido);
        } else {
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, arg.Msg);
        }

    });
}

function BuscarDadosEmissao(indicePedido, callback) {
    retornarDadosCargaPedido(indicePedido, function (pedido) {
        callback(pedido);
    });
}
