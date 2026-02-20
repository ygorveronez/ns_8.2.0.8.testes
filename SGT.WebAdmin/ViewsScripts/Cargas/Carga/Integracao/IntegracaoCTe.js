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
/// <reference path="Integracao.js" />
/// <reference path="IntegracaoCarga.js" />
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

var _tipoIntegracaoCTe = new Array();
var _gridIntegracaoCTe;
var _gridHistoricoIntegracaoCTe;
var _integracaoCTe;
var _pesquisaHistoricoIntegracaoCTe;

var PesquisaHistoricoIntegracaoCTe = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
}

var IntegracaoCTe = function () {

    this.Carga = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Tipo = PropertyEntity({ val: ko.observable(""), options: _tipoIntegracaoCTe, text: Localization.Resources.Cargas.Carga.Integracao.getFieldDescription(), def: "", issue: 267, visible: ko.observable(false) });
    this.Situacao = PropertyEntity({ val: ko.observable(""), options: EnumSituacaoIntegracaoCarga.obterOpcoesPesquisa(), text: Localization.Resources.Cargas.Carga.Situacao.getFieldDescription(), def: "", issue: 272 });

    this.TotalGeral = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int, text: Localization.Resources.Cargas.Carga.TotalGeral.getFieldDescription() });
    this.TotalAguardandoIntegracao = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int, text: Localization.Resources.Cargas.Carga.AguardandoIntegracao.getFieldDescription() });
    this.TotalAguardandoRetorno = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int, text: Localization.Resources.Cargas.Carga.AguardandoRetorno.getFieldDescription() });
    this.TotalProblemaIntegracao = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int, text: Localization.Resources.Cargas.Carga.ProblemasNaIntregracao.getFieldDescription() });
    this.TotalIntegrado = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int, text: Localization.Resources.Cargas.Carga.Integrados.getFieldDescription() });

    this.Pesquisar = PropertyEntity({
        eventClick: function (e) {
            _gridIntegracaoCTe.CarregarGrid();
            ObterTotaisIntegracaoCTe();
        }, type: types.event, text: Localization.Resources.Gerais.Geral.Pesquisar, idGrid: guid(), visible: ko.observable(true)
    });

    this.ReenviarTodos = PropertyEntity({
        eventClick: function (e) {
            ReenviarTodosIntegracaoCTe();
        }, type: types.event, text: Localization.Resources.Cargas.Carga.ReenviarTodos, idGrid: guid(), visible: ko.observable(false)
    });

    this.ObterTotais = PropertyEntity({
        eventClick: function (e) {
            ObterTotaisIntegracaoCTe();
        }, type: types.event, text: Localization.Resources.Cargas.Carga.ObterTotais, idGrid: guid(), visible: ko.observable(true)
    });
}

function LoadIntegracaoCTe(carga, idKnockoutIntegracaoCTe) {

    _integracaoCTe = new IntegracaoCTe();
    _integracaoCTe.Carga.val(carga.Codigo.val());

    KoBindings(_integracaoCTe, idKnockoutIntegracaoCTe);

    ObterTotaisIntegracaoCTe();
    ConfigurarPesquisaIntegracaoCTe();


    if (VerificaSePossuiPermissaoEspecial(EnumPermissaoPersonalizada.Carga_ReenviarIntegracoes, _PermissoesPersonalizadasCarga))
        _integracaoCTe.ReenviarTodos.visible(true);

}

function ConfigurarPesquisaIntegracaoCTe() {

    var menuOpcoes = { tipo: TypeOptionMenu.list, descricao: Localization.Resources.Gerais.Geral.Opcoes, tamanho: 7, opcoes: [] };

    menuOpcoes.opcoes.push({ descricao: Localization.Resources.Cargas.Carga.Reenviar, id: guid(), metodo: ReenviarIntegracaoCTe, tamanho: "20", icone: "", visibilidade: VisibilidadeOpcaoReenviarIntegracao });
    menuOpcoes.opcoes.push({ descricao: Localization.Resources.Cargas.Carga.HistoricoDeIntegracao, id: guid(), metodo: ExibirHistoricoIntegracaoCTe, tamanho: "20", icone: "", visibilidade: VisibilidadeOpcaoHistoricoIntegracao });

    _gridIntegracaoCTe = new GridView(_integracaoCTe.Pesquisar.idGrid, "CargaIntegracaoCTe/Pesquisa", _integracaoCTe, menuOpcoes);
    _gridIntegracaoCTe.CarregarGrid();
}

function VisibilidadeOpcaoReenviarIntegracao(data) {
    if (data.Tipo == EnumTipoIntegracao.Natura || (!VerificaSePossuiPermissaoEspecial(EnumPermissaoPersonalizada.Carga_ReenviarIntegracoes, _PermissoesPersonalizadasCarga) && data.SituacaoIntegracao != EnumSituacaoIntegracao.ProblemaIntegracao))
        return false;

    if (data.Tipo == EnumTipoIntegracao.KMM && data.SituacaoIntegracao != EnumSituacaoIntegracao.ProblemaIntegracao)
        return false;

    return true;
}

function VisibilidadeOpcaoHistoricoIntegracao(data) {
    if (data.Tipo == EnumTipoIntegracao.FTP)
        return false;

    return true;
}

function ObterTiposIntegracaoCTe() {
    var p = new promise.Promise();

    executarReST("TipoIntegracao/BuscarTodos", {
        Tipos: JSON.stringify([
            EnumTipoIntegracao.Avior,
            EnumTipoIntegracao.Avon,
            EnumTipoIntegracao.Natura,
            EnumTipoIntegracao.FTP,
            EnumTipoIntegracao.GPAEscrituracaoCTe])
    }, function (r) {
        if (r.Success) {

            _tipoIntegracaoCTe.push({ value: "", text: Localization.Resources.Gerais.Geral.Todas });

            for (var i = 0; i < r.Data.length; i++)
                _tipoIntegracaoCTe.push({ value: r.Data[i].Codigo, text: r.Data[i].Descricao });

        } else {
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, r.Msg);
        }

        p.done();
    });

    return p;
}

function ReenviarIntegracaoCTe(data) {
    executarReST("CargaIntegracaoCTe/Reenviar", { Codigo: data.Codigo }, function (r) {
        if (r.Success) {
            exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, Localization.Resources.Cargas.Carga.ReenvioSolicitadoComSucesso);
            _gridIntegracaoCTe.CarregarGrid();
        } else {
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, r.Msg);
        }
    });
}

function ReenviarTodosIntegracaoCTe() {
    exibirConfirmacao(Localization.Resources.Gerais.Geral.Atencao, Localization.Resources.Cargas.Carga.DesejaRealmenteReenviarTodasAsIntegracoesDeCTe, function () {
        executarReST("CargaIntegracaoCTe/ReenviarTodos", { Carga: _integracaoCTe.Carga.val(), Tipo: _integracaoCTe.Tipo.val(), Situacao: _integracaoCTe.Situacao.val() }, function (r) {
            if (r.Success) {
                exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, Localization.Resources.Cargas.Carga.ReenvioSolicitadoComSucesso);
                _gridIntegracaoCTe.CarregarGrid();
            } else {
                exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, r.Msg);
            }
        });
    });
}

function ObterTotaisIntegracaoCTe() {
    executarReST("CargaIntegracaoCTe/ObterTotais", { Carga: _integracaoCTe.Carga.val() }, function (r) {
        if (r.Success) {
            _integracaoCTe.TotalGeral.val(r.Data.TotalGeral);
            _integracaoCTe.TotalAguardandoIntegracao.val(r.Data.TotalAguardandoIntegracao);
            _integracaoCTe.TotalAguardandoRetorno.val(r.Data.TotalAguardandoRetorno);
            _integracaoCTe.TotalProblemaIntegracao.val(r.Data.TotalProblemaIntegracao);
            _integracaoCTe.TotalIntegrado.val(r.Data.TotalIntegrado);
        } else {
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, r.Msg);
        }
    });
}

function ExibirHistoricoIntegracaoCTe(integracao) {
    BuscarHistoricoIntegracaoCTe(integracao);
    Global.abrirModal("divModalHistoricoIntegracaoCTe");
}

function BuscarHistoricoIntegracaoCTe(integracao) {
    _pesquisaHistoricoIntegracaoCTe = new PesquisaHistoricoIntegracaoCTe();
    _pesquisaHistoricoIntegracaoCTe.Codigo.val(integracao.Codigo);

    var download = { descricao: Localization.Resources.Cargas.Carga.DownloadArquivos, id: guid(), evento: "onclick", metodo: DownloadArquivosHistoricoIntegracaoCTe, tamanho: "20", icone: "" };

    var menuOpcoes = {
        tipo: TypeOptionMenu.link,
        opcoes: [download]
    };

    _gridHistoricoIntegracaoCTe = new GridView("tblHistoricoIntegracaoCTe", "CargaIntegracaoCTe/ConsultarHistoricoIntegracao", _pesquisaHistoricoIntegracaoCTe, menuOpcoes, { column: 1, dir: orderDir.desc });
    _gridHistoricoIntegracaoCTe.CarregarGrid();
}

function DownloadArquivosHistoricoIntegracaoCTe(historicoConsulta) {
    executarDownload("CargaIntegracaoCTe/DownloadArquivosHistoricoIntegracao", { Codigo: historicoConsulta.Codigo });
}

function RecarregarIntegracaoCTeViaSignalR(knoutCarga) {
    if (_cargaAtual != null && _cargaAtual.DivCarga.id == knoutCarga.DivCarga.id) {
        if ($("#divIntegracaoCTe_" + _cargaAtual.EtapaIntegracao.idGrid).is(":visible") && _gridIntegracaoCTe != null) {
            _gridIntegracaoCTe.CarregarGrid();
            ObterTotaisIntegracaoCTe();
        }
    }
}