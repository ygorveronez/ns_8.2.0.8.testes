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
/// <reference path="../../../Creditos/ControleSaldo/ControleSaldo.js" />
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
/// <reference path="Complemento.js" />
/// <reference path="Componente.js" />
/// <reference path="EtapaFrete.js" />
/// <reference path="Frete.js" />
/// <reference path="SemTabela.js" />
/// <reference path="TabelaCliente.js" />
/// <reference path="TabelaComissao.js" />
/// <reference path="TabelaSubContratacao.js" />
/// <reference path="TabelaTerceiros.js" />
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
/// <reference path="../../../Enumeradores/EnumTipoTabelaFrete.js" />
/// <reference path="../../../Consultas/ComponenteFrete.js" />
/// <reference path="../../../Enumeradores/EnumSituacaoRetornoFreteCliente.js" />
/// <reference path="../../../Enumeradores/EnumSituacaoRetornoFreteComissao.js" />
/// <reference path="../../../Enumeradores/EnumSituacaoComplementoFrete.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var FretePorRotaUtilizado = function () {
    this.Codigo = PropertyEntity({ type: types.local });
    this.Tabela = PropertyEntity({ type: types.local });
    this.Origem = PropertyEntity({ type: types.local });
    this.Destino = PropertyEntity({ type: types.local });
    this.TipoCarga = PropertyEntity({ type: types.local });
    this.ModeloVeicularCarga = PropertyEntity({ type: types.local });
    this.ValorFreteAPagar = PropertyEntity({ type: types.local });
    this.ValorFreteTabelaFrete = PropertyEntity({ type: types.local });
    this.DetalhesFrete = PropertyEntity({ eventClick: detalhesFreteClick, type: types.event, text: Localization.Resources.Cargas.Carga.VerDetalhes, visible: ko.observable(true) });
}

//*******EVENTOS*******

var _HTMLDetalheFretePorRota;

function loadCargaFreteRota() {
    $.get("Content/Static/Carga/DestalhesFretePorRota.html?dyn=" + guid(), function (data) {

        LocalizeCurrentPage();

        _HTMLDetalheFretePorRota = data;
    })
}

//*******MÉTODOS*******

function preencherRetornoFreteRota(e, retorno) {
    var freteUtilizado = retorno.dadosRetornoTipoFrete;
    var detalhePorRota = new FretePorRotaUtilizado();
    detalhePorRota.Codigo.val(freteUtilizado.Codigo);
    detalhePorRota.Tabela.val(freteUtilizado.Codigo + " - " + freteUtilizado.DescricaoDestinos + " (" + freteUtilizado.Tabela + ")");
    detalhePorRota.Origem.val(freteUtilizado.Origem);
    detalhePorRota.Destino.val(freteUtilizado.Destino);
    detalhePorRota.TipoCarga.val(freteUtilizado.TipoCarga);
    detalhePorRota.ModeloVeicularCarga.val(freteUtilizado.ModeloVeicularCarga);
    detalhePorRota.ValorFreteAPagar.val(Globalize.format(retorno.valorFreteAPagar, "n2"));
    detalhePorRota.ValorFreteTabelaFrete.val(Globalize.format(retorno.valorFreteTabelaFrete, "n2"));
    e.ValorFreteTabelaFrete.val(retorno.valorFreteTabelaFrete);
    e.AdicionarComplementoFrete.visible(_operadorLogistica.PermiteAdicionarComplementosDeFrete);

    preecherDetalhesTiposFrete(e, _HTMLDetalheFretePorRota, detalhePorRota, retorno);
    PreecherInformacaoValorFrete(e, retorno.valorFreteAPagar)
    preencherDetalhesFrete(retorno, false);
}

function exibirProblemaFreteRota(e, retorno) {
    var freteUtilizado = retorno.dadosRetornoTipoFrete;
    EtapaFreteEmbarcadorProblema(e);
    e.ValorFreteOperador.visible(false);
    e.AdicionarComplementoFrete.visible(false);
    if (freteUtilizado.situacao == EnumSituacaoRetornoFreteRota.NaoEncontrouTipoCarga) {
        var mensagem = Localization.Resources.Cargas.Carga.TipoDeCargaNaoPossuiUmFreteConfigurado.format(e.TipoCarga.val()) + " " + freteUtilizado.rotaSemFrete.Origem.Descricao + " " + Localization.Resources.Cargas.Carga.Ate + " " + freteUtilizado.rotaSemFrete.Destino.Descricao + ".";
        setarMensagemPendenciaFrete(e, mensagem);
    }
    if (freteUtilizado.situacao == EnumSituacaoRetornoFreteRota.NaoEncontrouModeloVeicularCarga) {
        var mensagem = Localization.Resources.Cargas.Carga.ModeloVeicularDeCargaNaoPossuiUmFreteConfiguradoParaTipoDeCarga.format(e.ModeloVeicularCarga.val()) + " " + e.TipoCarga.val() + " " + Localization.Resources.Cargas.Carga.De + " " + freteUtilizado.rotaSemFrete.Origem.Descricao + " " + Localization.Resources.Cargas.Carga.Ate + " " + freteUtilizado.rotaSemFrete.Destino.Descricao + ".";
        setarMensagemPendenciaFrete(e, mensagem);
    }
    if (freteUtilizado.situacao == EnumSituacaoRetornoFreteRota.TabelaNaoEncontrada) {
        var html = _HTMLPendenciasCarga.replace(/#idPendencia/g, freteUtilizado.rotaSemFrete.Codigo)
            .replace(/#DescricaoPendecia/g, freteUtilizado.rotaSemFrete.Origem.Descricao + " " + Localization.Resources.Cargas.Carga.Ate + " " + freteUtilizado.rotaSemFrete.Destino.Descricao);
        var titulo = Localization.Resources.Cargas.Carga.NaoFoiEncontradoUmFreteParaOrigemDestinoFinalDestaCarga;
        setarMensagemPendenciaFrete(e, html, titulo, false);
    }

    if (freteUtilizado.situacao == EnumSituacaoRetornoFreteRota.MaisQueUmaTabelaParaRota) {

        var titulo = "";
        var html = "<div class='TabelaFaltante'><section class='col col-12 no-padding'><label class='label'><b>" + Localization.Resources.Cargas.Carga.EscolhaTabelaDeFreteCorretaParaEstaCarga + ":</b></label><label class='select'><select class='selectTabelaFrete'>";
        $.each(freteUtilizado.tabelas, function (i, tabela) {
            html += " <option value='" + tabela.Codigo + "'>" + tabela.Destino.Descricao;
            if (tabela.DescricaoDestinos != "")
                html += " ( " + tabela.DescricaoDestinos + " )";

            html += "</option>";
        });
        html += "</select><i></i>";
        html += "</label></section></div>";

        setarMensagemPendenciaFrete(e, html, titulo, false);
        EtapaFreteEmbarcadorAguardando(e);
    }
}