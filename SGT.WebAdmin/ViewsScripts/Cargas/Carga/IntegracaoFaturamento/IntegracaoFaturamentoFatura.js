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

var _gridIntegracaoFaturamentoFatura;
var _integracaoFaturamentoFatura;
var _gridHistoricoIntegracaoFatura;
var _pesquisaHistoricoIntegracaoFatura;
var _modalHistoricoIntegracaoFatura;

var IntegracaoFaturamentoFatura = function () {
    this.Carga = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Situacao = PropertyEntity({ val: ko.observable(""), options: EnumSituacaoIntegracaoCarga.obterOpcoesPesquisa(), text: Localization.Resources.Cargas.Carga.Situacao.getFieldDescription(), def: "", issue: 272 });

    this.TotalGeral = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int, text: Localization.Resources.Cargas.Carga.TotalGeral.getFieldDescription() });
    this.TotalAguardandoIntegracao = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int, text: Localization.Resources.Cargas.Carga.AguardandoIntegracao.getFieldDescription() });
    this.TotalProblemaIntegracao = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int, text: Localization.Resources.Cargas.Carga.ProblemasNaIntegracao.getFieldDescription() });
    this.TotalIntegrado = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int, text: Localization.Resources.Cargas.Carga.Integrados.getFieldDescription() });

    this.Pesquisar = PropertyEntity({
        eventClick: function (e) {
            ObterTotaisIntegracaoFaturamentoFatura();
            _gridIntegracaoFaturamentoFatura.CarregarGrid();
        }, type: types.event, text: Localization.Resources.Gerais.Geral.Pesquisar, idGrid: guid(), visible: ko.observable(true)
    });

    this.ObterTotais = PropertyEntity({
        eventClick: function (e) {
            ObterTotaisIntegracaoFaturamentoFatura();
        }, type: types.event, text: Localization.Resources.Cargas.Carga.ObterTotais, idGrid: guid(), visible: ko.observable(true)
    });
}

var PesquisaHistoricoIntegracaoFatura = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
};

function LoadIntegracaoFaturamentoFatura(carga, idKnockoutIntegracaoFaturamentoFatura) {

    _integracaoFaturamentoFatura = new IntegracaoFaturamentoFatura();

    _integracaoFaturamentoFatura.Carga.val(carga.Codigo.val());

    KoBindings(_integracaoFaturamentoFatura, idKnockoutIntegracaoFaturamentoFatura);

    ObterTotaisIntegracaoFaturamentoFatura();
    ConfigurarPesquisaIntegracaoFaturamentoFatura();

    _modalHistoricoIntegracaoFatura = new bootstrap.Modal(document.getElementById("divModalHistoricoIntegracaoFatura"), { backdrop: 'static', keyboard: true });
}

function ConfigurarPesquisaIntegracaoFaturamentoFatura() {

    let reenviarEDI = { descricao: "Re-enviar", id: guid(), metodo: ReenviarIntegracaoFaturamentoFatura, icone: "" };
    let downloadEDI = { descricao: "Histórico de Envio", id: guid(), metodo: ExibirHistoricoIntegracaoFatura, icone: "" };
    let downloadFatura = { descricao: "Download", id: guid(), metodo: DownloadIntegracaoFaturamentoEDI, icone: "" };
    let menuOpcoes = { tipo: TypeOptionMenu.list, descricao: "Opções", tamanho: 7, opcoes: [reenviarEDI, downloadEDI, downloadFatura] };

    _gridIntegracaoFaturamentoFatura = new GridView(_integracaoFaturamentoFatura.Pesquisar.idGrid, "FaturaIntegracao/PesquisaIntegracaoFatura", _integracaoFaturamentoFatura, menuOpcoes, null, null, null);
    _gridIntegracaoFaturamentoFatura.CarregarGrid();
}

function DownloadIntegracaoFaturamentoEDI(historicoConsulta) {
    exibirMensagem(tipoMensagem.aviso, "Aviso", "Opção indisponível");
}

function ExibirHistoricoIntegracaoFatura(integracao) {
    BuscarHistoricoIntegracaoFatura(integracao);
    _modalHistoricoIntegracaoFatura.show();
}

function BuscarHistoricoIntegracaoFatura(integracao) {
    _pesquisaHistoricoIntegracaoFatura = new PesquisaHistoricoIntegracaoFatura();
    _pesquisaHistoricoIntegracaoFatura.Codigo.val(integracao.Codigo);

    let download = { descricao: "Download Arquivos", id: guid(), evento: "onclick", metodo: DownloadArquivosHistoricoIntegracaoFatura, tamanho: "20", icone: "" };

    let menuOpcoes = {
        tipo: TypeOptionMenu.link,
        opcoes: [download]
    };

    _gridHistoricoIntegracaoFatura = new GridView("tblHistoricoIntegracaoFatura", "FaturaIntegracao/ConsultarHistoricoIntegracao", _pesquisaHistoricoIntegracaoFatura, null, { column: 1, dir: orderDir.desc });
    _gridHistoricoIntegracaoFatura.CarregarGrid();
}

function DownloadArquivosHistoricoIntegracaoFatura(historicoConsulta) {
    exibirMensagem(tipoMensagem.aviso, "Aviso", "Opção indisponível");
    //executarDownload("FaturaIntegracao/DownloadArquivosHistoricoIntegracao", { Codigo: historicoConsulta.Codigo });
}

function DownloadIntegracaoFaturamentoFatura(data) {
    if (e.Codigo > 0) {
        var data = {
            Codigo: e.Codigo
        };
        executarDownload("FaturaIntegracao/DownloadEDI", data);
    }
}

function ReenviarIntegracaoFaturamentoFatura(data) {
    exibirConfirmacao("Atenção!", "Deseja realmente reenviar essa fatura?", function () {
        executarReST("FaturaIntegracao/EnviarLayoutFatura", { Codigo: e.Codigo }, function (arg) {
            if (arg.Success) {
                if (arg.Data) {
                    _gridArquivosFatura.CarregarGrid();
                } else {
                    exibirMensagem(tipoMensagem.aviso, "Atenção", arg.Msg);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
            }
        });
    });
}

function ObterTotaisIntegracaoFaturamentoFatura() {
    executarReST("FaturaIntegracao/ObterTotaisFatura", { Carga: _integracaoFaturamentoFatura.Carga.val() }, function (r) {
        if (r.Success) {
            _integracaoFaturamentoFatura.TotalGeral.val(r.Data.TotalGeral);
            _integracaoFaturamentoFatura.TotalAguardandoIntegracao.val(r.Data.TotalAguardandoIntegracao);
            _integracaoFaturamentoFatura.TotalProblemaIntegracao.val(r.Data.TotalProblemaIntegracao);
            _integracaoFaturamentoFatura.TotalIntegrado.val(r.Data.TotalIntegrado);
        } else {
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, r.Msg);
        }
    });
}

//function RecarregarIntegracaoFaturamentoFaturaViaSignalR(knoutCarga) {
//    if (_cargaAtual != null && _cargaAtual.DivCarga.id == knoutCarga.DivCarga.id) {
//        if ($("#divIntegracaoFaturamentoFatura_" + _cargaAtual.EtapaIntegracao.idGrid).is(":visible") && _gridIntegracaoFaturamentoFatura != null) {
//            _gridIntegracaoFaturamentoFatura.CarregarGrid();
//            ObterTotaisIntegracaoFaturamentoFatura();
//        }
//    }
//}