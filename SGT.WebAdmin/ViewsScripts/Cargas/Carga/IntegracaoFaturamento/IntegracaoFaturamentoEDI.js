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
/// <reference path="../../../Enumeradores/EnumSituacaoIntegracaoCarga.js" />

var _gridIntegracaoFaturamentoEDI;
var _integracaoFaturamentoEDI;
var _gridHistoricoIntegracaoEDI;
var _pesquisaHistoricoIntegracaoEDI;
var _modalHistoricoIntegracaoEDI;

var IntegracaoFaturamentoEDI = function () {
    this.Carga = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Situacao = PropertyEntity({ val: ko.observable(""), options: EnumSituacaoIntegracaoCarga.obterOpcoesPesquisa(), text: Localization.Resources.Cargas.Carga.Situacao.getFieldDescription(), def: "", issue: 272 });

    this.TotalGeral = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int, text: Localization.Resources.Cargas.Carga.TotalGeral.getFieldDescription() });
    this.TotalAguardandoIntegracao = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int, text: Localization.Resources.Cargas.Carga.AguardandoIntegracao.getFieldDescription() });
    this.TotalProblemaIntegracao = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int, text: Localization.Resources.Cargas.Carga.ProblemasNaIntegracao.getFieldDescription() });
    this.TotalIntegrado = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int, text: Localization.Resources.Cargas.Carga.Integrados.getFieldDescription() });

    this.Pesquisar = PropertyEntity({
        eventClick: function (e) {
            ObterTotaisIntegracaoFaturamentoEDI();
            _gridIntegracaoFaturamentoEDI.CarregarGrid();
        }, type: types.event, text: Localization.Resources.Gerais.Geral.Pesquisar, idGrid: guid(), visible: ko.observable(true)
    });

    this.ObterTotais = PropertyEntity({
        eventClick: function (e) {
            ObterTotaisIntegracaoFaturamentoEDI();
        }, type: types.event, text: Localization.Resources.Cargas.Carga.ObterTotais, idGrid: guid(), visible: ko.observable(true)
    });
}

var PesquisaHistoricoIntegracaoEDI = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
};

function LoadIntegracaoFaturamentoEDI(carga, idKnockoutIntegracaoFaturamentoEDI) {

    _integracaoFaturamentoEDI = new IntegracaoFaturamentoEDI();

    _integracaoFaturamentoEDI.Carga.val(carga.Codigo.val());

    KoBindings(_integracaoFaturamentoEDI, idKnockoutIntegracaoFaturamentoEDI);

    ObterTotaisIntegracaoFaturamentoEDI();
    ConfigurarPesquisaIntegracaoFaturamentoEDI();

    _modalHistoricoIntegracaoEDI = new bootstrap.Modal(document.getElementById("divModalHistoricoIntegracaoFaturaEDI"), { backdrop: 'static', keyboard: true });
}

function ConfigurarPesquisaIntegracaoFaturamentoEDI() {

    let reenviarEDI = { descricao: "Re-enviar", id: guid(), metodo: ReenviarIntegracaoFaturamentoEDI, icone: "", visibilidade: VisibilidadeOpcaoReenviarIntegracaoEDI };
    let historicoEnvio = { descricao: "Histórico de Envio", id: guid(), metodo: ExibirHistoricoIntegracaoEDI, icone: "" };
    let downloadEDI = { descricao: "Download", id: guid(), metodo: DownloadIntegracaoFaturamentoEDI, icone: "", visibilidade: VisibilidadeOpcaoDownloadIntegracaoEDI };    
    let menuOpcoes = { tipo: TypeOptionMenu.list, descricao: "Opções", tamanho: 7, opcoes: [reenviarEDI, downloadEDI, historicoEnvio] };

    _gridIntegracaoFaturamentoEDI = new GridView(_integracaoFaturamentoEDI.Pesquisar.idGrid, "FaturaIntegracao/PesquisaIntegracaoEDI", _integracaoFaturamentoEDI, menuOpcoes, null, null, null);
    _gridIntegracaoFaturamentoEDI.CarregarGrid();
}

function ExibirHistoricoIntegracaoEDI(integracao) {
    BuscarHistoricoIntegracaoEDI(integracao);
    _modalHistoricoIntegracaoEDI.show();
}

function BuscarHistoricoIntegracaoEDI(integracao) {
    _pesquisaHistoricoIntegracaoEDI = new PesquisaHistoricoIntegracaoFatura();
    _pesquisaHistoricoIntegracaoEDI.Codigo.val(integracao.Codigo);

    let download = { descricao: "Download", id: guid(), evento: "onclick", metodo: DownloadArquivosHistoricoIntegracaoFaturaEDI, tamanho: "20", icone: "" };

    let menuOpcoes = {
        tipo: TypeOptionMenu.link,
        opcoes: [download]
    };

    _pesquisaHistoricoIntegracaoEDI = new GridView("tblHistoricoIntegracaoFaturaEDI", "FaturaIntegracao/ConsultarHistoricoEnvioEDI", _pesquisaHistoricoIntegracaoEDI, null, { column: 1, dir: orderDir.desc });
    _pesquisaHistoricoIntegracaoEDI.CarregarGrid();
}

function DownloadArquivosHistoricoIntegracaoFaturaEDI(historicoConsulta) {
    exibirMensagem(tipoMensagem.aviso, "Aviso", "Opção indisponível");
}

function VisibilidadeOpcaoDownloadIntegracaoEDI(data) {
    return true;//VerificaSePossuiPermissaoEspecial(EnumPermissaoPersonalizada.Carga_DownloadArquivoIntegracoes, _PermissoesPersonalizadas);
}

function VisibilidadeOpcaoReenviarIntegracaoEDI(data) {
    //if (data.Tipo == EnumTipoIntegracao.NaoPossuiIntegracao || !VerificaSePossuiPermissaoEspecial(EnumPermissaoPersonalizada.Fatura_PermiteEnviarEDI, _PermissoesPersonalizadas))
    //    return false;

    return true;
}

function DownloadIntegracaoFaturamentoEDI(data) {
    if (e.Codigo > 0) {
        var data = {
            Codigo: e.Codigo
        };
        executarDownload("FaturaIntegracao/DownloadEDI", data);
    }
}

function ReenviarIntegracaoFaturamentoEDI(data) {
    exibirConfirmacao("Atenção!", "Deseja realmente reenviar esse arquivo EDI?", function () {
        if (e.Codigo > 0) {
            var data = {
                Codigo: e.Codigo
            };
            executarReST("FaturaIntegracao/EnviarLayoutEDI", data, function (arg) {
                if (arg.Success) {
                    _gridIntegracaoFaturamentoEDI.CarregarGrid();
                } else {
                    exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
                }
            });
        }
    });
}

function ObterTotaisIntegracaoFaturamentoEDI() {
    executarReST("FaturaIntegracao/ObterTotaisEDI", { Carga: _integracaoFaturamentoEDI.Carga.val() }, function (r) {
        if (r.Success) {
            _integracaoFaturamentoEDI.TotalGeral.val(r.Data.TotalGeral);
            _integracaoFaturamentoEDI.TotalAguardandoIntegracao.val(r.Data.TotalAguardandoIntegracao);
            _integracaoFaturamentoEDI.TotalProblemaIntegracao.val(r.Data.TotalProblemaIntegracao);
            _integracaoFaturamentoEDI.TotalIntegrado.val(r.Data.TotalIntegrado);
        } else {
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, r.Msg);
        }
    });
}

//function RecarregarIntegracaoFaturamentoEDIViaSignalR(knoutCarga) {
//    if (_cargaAtual != null && _cargaAtual.DivCarga.id == knoutCarga.DivCarga.id) {
//        if ($("#divIntegracaoFaturamentoEDI_" + _cargaAtual.EtapaIntegracao.idGrid).is(":visible") && _gridIntegracaoFaturamentoEDI != null) {
//            _gridIntegracaoFaturamentoEDI.CarregarGrid();
//            ObterTotaisIntegracaoFaturamentoEDI();
//        }
//    }
//}