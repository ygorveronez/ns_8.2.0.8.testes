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
/// <reference path="../Integracao/Integracao.js" />
/// <reference path="../Integracao/IntegracaoCarga.js" />
/// <reference path="../Integracao/IntegracaoCTe.js" />
/// <reference path="../Integracao/IntegracaoEDI.js" />
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
/// <reference path="../../../../Areas/Relatorios/ViewsScripts/Relatorios/Global/Relatorio.js" />

//*******MAPEAMENTO*******

var _HTMLCargaContratoFrete;
var _cargaSubContratacao;
var _gridCIOTSubcontratacao;
var _pesquisaHistoricoIntegracaoContratoFrete;
var _gridHistoricoIntegracaoContratoFrete;

var PesquisaHistoricoIntegracaoContratoFrete = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
};

var CargasSubContratacao = function () {

    this.Carga = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Transbordo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0 });
    this.DataEmissaoContrato = PropertyEntity({ val: ko.observable(""), visible: ko.observable(true), text: Localization.Resources.Cargas.Carga.DataDeEmissaoDoContrato.getFieldDescription(), def: "" });
    this.SituacaoContratoFrete = PropertyEntity({ val: ko.observable(EnumSituacaoContratoFrete.CargaEmAndamento), def: EnumSituacaoContratoFrete.CargaEmAndamento });
    this.TipoFreteEscolhido = PropertyEntity({ val: ko.observable(EnumTipoFreteEscolhido.todos), def: EnumTipoFreteEscolhido.todos });
    this.DescricaoTipoFreteEscolhido = PropertyEntity({ val: ko.observable(""), visible: ko.observable(true), text: Localization.Resources.Cargas.Carga.FreteEscolhido.getFieldDescription(), def: "" });
    this.DescricaoSituacaoContratoFrete = PropertyEntity({ val: ko.observable(""), visible: ko.observable(true), text: Localization.Resources.Cargas.Carga.SituacaoDoContrato.getFieldDescription(), def: "" });
    this.Terceiro = PropertyEntity({ val: ko.observable(""), visible: ko.observable(true), text: Localization.Resources.Cargas.Carga.TransportadorTerceiro.getFieldDescription(), def: "" });
    this.ValorFreteSubcontratacao = PropertyEntity({ val: ko.observable(""), getType: typesKnockout.decimal, visible: ko.observable(true), text: Localization.Resources.Cargas.Carga.ValorDoFrete.getRequiredFieldDescription(), def: "", enable: ko.observable(false), required: true });

    this.ValorFreteSubContratacaoTabelaFrete = PropertyEntity({ val: ko.observable(""), visible: ko.observable(true), text: Localization.Resources.Cargas.Carga.ValorPelaTabelaDeFrete.getFieldDescription(), def: "" });
    this.ValorPedagio = PropertyEntity({ val: ko.observable(""), visible: ko.observable(true), text: Localization.Resources.Cargas.Carga.ValorDePedagio.getFieldDescription(), def: "" });
    this.ValorTotalAPagar = PropertyEntity({ val: ko.observable(""), visible: ko.observable(true), text: Localization.Resources.Cargas.Carga.TotalPagar.getFieldDescription(), def: "" });
    this.Relatorio = PropertyEntity({ getType: typesKnockout.report });

    this.ImprimirContrato = PropertyEntity({ eventClick: imprimirContratoClick, type: types.event, text: Localization.Resources.Cargas.Carga.Contrato, visible: ko.observable(false), icon: "fal fa-pencil" });
    this.ImprimirRomaneioEntrega = PropertyEntity({ eventClick: ImprimirRomaneioEntregaClick, type: types.event, text: Localization.Resources.Cargas.Carga.RomaneioDeEntrega, visible: ko.observable(false), icon: "fal fa-map-pin" });
    this.AlterarContrato = PropertyEntity({ eventClick: alterarDadosContratoClick, type: types.event, text: Localization.Resources.Gerais.Geral.Salvar, visible: ko.observable(false) });
    this.HistoricoIntegracao = PropertyEntity({ eventClick: HistoricoIntegracaoClick, type: types.event, text: Localization.Resources.Cargas.Carga.HistoricoDeIntegracao, visible: ko.observable(false), icon: "fal fa-download" });
    this.EnviarIntegracaoQuitacaoAX = PropertyEntity({ eventClick: EnviarIntegracaoQuitacaoAXClick, type: types.event, text: Localization.Resources.Cargas.Carga.EnviarIntegracaoQuitacaoAX, visible: ko.observable(false), icon: "fal fa-cogs" });

    this.PercentualAbastecimento = PropertyEntity({ val: ko.observable(""), getType: typesKnockout.decimal, visible: ko.observable(false), text: Localization.Resources.Cargas.Carga.PercentualDeAbastecimento.getFieldDescription(), def: "", enable: ko.observable(false) });
    this.ValorAbastecimento = PropertyEntity({ val: ko.observable(""), text: Localization.Resources.Cargas.Carga.Abastecimento.getFieldDescription(), def: "" });
    this.PercentualAdiantamento = PropertyEntity({ val: ko.observable(""), getType: typesKnockout.decimal, visible: ko.observable(false), text: Localization.Resources.Cargas.Carga.PercentualDeAdiantamento.getFieldDescription(), def: "", enable: ko.observable(false) });
    this.ValorAdiantamento = PropertyEntity({ val: ko.observable(""), text: Localization.Resources.Cargas.Carga.Adiantamento.getFieldDescription(), def: "" });
    //this.ValorOutrosAdiantamento = PropertyEntity({ val: ko.observable(""), getType: typesKnockout.decimal, visible: ko.observable(false), text: "Adiantamento Outros: ", def: "", enable: ko.observable(false) });

    //this.Descontos = PropertyEntity({ val: ko.observable(""), getType: typesKnockout.decimal, visible: ko.observable(false), text: "Descontos: ", def: "", enable: ko.observable(false) });
    this.Observacao = PropertyEntity({ val: ko.observable(""), visible: ko.observable(false), text: Localization.Resources.Gerais.Geral.Observacao.getFieldDescription(), def: "", enable: ko.observable(false), required: false, maxlength: 500 });
    this.PossuiCIOT = PropertyEntity({ val: ko.observable(false), def: false });
    this.EnviouContratoAXComSucesso = PropertyEntity({ val: ko.observable(false), def: false });
    this.EnviouAcertoContasContratoAXComSucesso = PropertyEntity({ val: ko.observable(false), def: false });
    this.PossuiIntegracaoAX = PropertyEntity({ val: ko.observable(false), def: false });

    this.Bloqueado = PropertyEntity({ val: ko.observable(false), def: false });
    this.JustificativaBloqueio = PropertyEntity({ val: ko.observable(""), text: Localization.Resources.Cargas.Carga.ContratoBloqueado, def: "" });

    this.AutorizarContrato = PropertyEntity({ eventClick: autorizarContratoFreteClick, type: types.event, text: Localization.Resources.Cargas.Carga.LiberarContrato, visible: ko.observable(false) });
    this.PaginaRelatorio = PropertyEntity({ val: ko.observable("Cargas/Carga") });

    this.AdicionarValor = PropertyEntity({ eventClick: AbrirTelaValorContratoFreteClick, type: types.event, text: Localization.Resources.Gerais.Geral.Adicionar, icon: "fal fa-plus", idGrid: guid(), visible: ko.observable(false) });

    this.Pesquisar = PropertyEntity({
        eventClick: function (e) {
            _gridCargaCTe.CarregarGrid();
        }, type: types.event, text: Localization.Resources.Gerais.Geral.Pesquisar, idGrid: guid(), visible: ko.observable(true)
    });

    this.PesquisarCIOT = PropertyEntity({
        eventClick: function (e) {
            _gridCIOTSubcontratacao.CarregarGrid();
        }, type: types.event, text: Localization.Resources.Gerais.Geral.Pesquisar, idGrid: guid(), visible: ko.observable(true)
    });
};

//*******EVENTOS*******

function autorizarContratoFreteClick(e, sender) {
    exibirConfirmacao(Localization.Resources.Gerais.Geral.Confirmacao, Localization.Resources.Cargas.Carga.RealmenteDesejaLiberarEsseContratoDeFrete, function () {
        Salvar(e, "ContratoFrete/AprovarContrato", function (arg) {
            if (arg.Success) {
                if (arg.Data !== false) {
                    exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, Localization.Resources.Cargas.Carga.ContratoAprovadoComSucesso);
                    _cargaSubContratacao.SituacaoContratoFrete.val(EnumSituacaoContratoFrete.Aprovado);

                    if (_cargaSubContratacao.PossuiCIOT.val() === true)
                        BuscarCIOTCargaSubcontratada(_cargaSubContratacao);
                    else
                        buscarCargasPreCTe(_cargaSubContratacao);

                    _cargaSubContratacao.AutorizarContrato.visible(false);
                    _cargaSubContratacao.PercentualAdiantamento.enable(false);
                    _cargaSubContratacao.PercentualAbastecimento.enable(false);
                    //_cargaSubContratacao.Descontos.enable(false);
                    _cargaSubContratacao.Observacao.enable(false);
                    _cargaSubContratacao.AlterarContrato.visible(false);
                    _cargaSubContratacao.ImprimirContrato.visible(true);
                    _cargaSubContratacao.ImprimirRomaneioEntrega.visible(true);
                    _cargaSubContratacao.EnviarIntegracaoQuitacaoAX.visible(true);
                    _cargaSubContratacao.HistoricoIntegracao.visible(true);
                    _cargaSubContratacao.AdicionarValor.visible(false);
                    EtapaSubContratacaoAprovada(_cargaAtual);

                } else {
                    exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.Atencao, arg.Msg);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, arg.Msg);
            }
        });
    });
}

function imprimirContratoClick(e, sender) {

    _gridRelatorio = new GridView("qualquercoisa", "Relatorios/ContratoFrete/Pesquisa", e);
    var _relatorioFatura = new RelatorioGlobal("Relatorios/ContratoFrete/BuscarDadosRelatorio", _gridRelatorio, function () {
        _relatorioFatura.loadRelatorio(function () {
            _relatorioFatura.gerarRelatorio("Relatorios/ContratoFrete/GerarRelatorio", EnumTipoArquivoRelatorio.PDF);
        });
    }, null, null, e);
}

function ImprimirRomaneioEntregaClick(e, sender) {
    executarDownload("ContratoFrete/DownloadRomaneioEntrega", { Codigo: e.Codigo.val() });
}

function EnviarIntegracaoQuitacaoAXClick(e, sender) {
    exibirConfirmacao(Localization.Resources.Gerais.Geral.Atencao, Localization.Resources.Cargas.Carga.DesejaRealmenteEnviarIntegracaoDeQuitacaoComAX, function () {
        executarReST("ContratoFrete/EnviarIntegracaoQuitacaoAX", { Codigo: e.Codigo.val() }, function (r) {
            if (r.Success) {
                if (r.Data) {
                    exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, Localization.Resources.Cargas.Carga.IntegracaoEnviadaComSucesso);
                } else {
                    exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Atencao, r.Msg);
                }
            } else {
                exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Atencao, r.Msg);
            }
        });
    });
}

function HistoricoIntegracaoClick(e, sender) {
    BuscarHistoricoIntegracaoContratoFrete(e);
    Global.abrirModal("divModalHistoricoIntegracaoContratoFrete");
}

function BuscarHistoricoIntegracaoContratoFrete(integracao) {
    _pesquisaHistoricoIntegracaoContratoFrete = new PesquisaHistoricoIntegracaoContratoFrete();
    _pesquisaHistoricoIntegracaoContratoFrete.Codigo.val(integracao.Codigo.val());

    var download = { descricao: Localization.Resources.Cargas.Carga.DownloadArquivos, id: guid(), evento: "onclick", metodo: DownloadArquivosHistoricoIntegracaoContratoFrete, tamanho: "20", icone: "" };

    var menuOpcoes = {
        tipo: TypeOptionMenu.link,
        opcoes: [download]
    };

    _gridHistoricoIntegracaoContratoFrete = new GridView("tblHistoricoIntegracaoContratoFrete", "ContratoFrete/ConsultarHistoricoIntegracao", _pesquisaHistoricoIntegracaoContratoFrete, menuOpcoes, { column: 1, dir: orderDir.desc });
    _gridHistoricoIntegracaoContratoFrete.CarregarGrid();
}

function DownloadArquivosHistoricoIntegracaoContratoFrete(historicoConsulta) {
    executarDownload("ContratoFrete/DownloadArquivosHistoricoIntegracao", { Codigo: historicoConsulta.Codigo });
}


function alterarDadosContratoClick(e, sender) {
    Salvar(e, "ContratoFrete/AlterarDadosContratoFrete", function (arg) {
        if (arg.Success) {
            if (arg.Data !== false) {
                exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, Localization.Resources.Cargas.Carga.ContratoAlteradoComSucesso);
                preecherDadosContrato(arg.Data, e);
            } else {
                exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.Atencao, arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, arg.Msg);
        }
    });
}


function verificarSubContratacaoClick(e) {
    ocultarTodasAbas(e);
    _cargaAtual = e;

    var contratoFrete = _cargaAtual.ContratoFreteTerceiro.val();
    _cargaSubContratacao = new CargasSubContratacao();
    preecherDadosContrato(contratoFrete, _cargaSubContratacao);
    var strKnoutCargaSubContratacao = "knoutCargaSubContratacao" + e.EtapaSubContratacao.idGrid;
    $("#" + e.EtapaSubContratacao.idGrid).html(_HTMLCargaContratoFrete.replace("#knoutCargaSubContratacao", strKnoutCargaSubContratacao));

    KoBindings(_cargaSubContratacao, strKnoutCargaSubContratacao);
    LocalizeCurrentPage();

    _cargaSubContratacao.AdicionarValor.visible(false);
    CarregarGridValorContratoFrete(_cargaSubContratacao);

    if (_CONFIGURACAO_TMS.NaoPermitirImpressaoContratoFretePendente === true &&
        contratoFrete.SituacaoContratoFrete != EnumSituacaoContratoFrete.Aprovado &&
        contratoFrete.SituacaoContratoFrete != EnumSituacaoContratoFrete.Finalizada) {
        _cargaSubContratacao.ImprimirContrato.visible(false);
        _cargaSubContratacao.ImprimirRomaneioEntrega.visible(false);
    } else {
        if (contratoFrete.SituacaoContratoFrete == EnumSituacaoContratoFrete.AgAprovacao || contratoFrete.SituacaoContratoFrete == EnumSituacaoContratoFrete.Aprovado || contratoFrete.SituacaoContratoFrete == EnumSituacaoContratoFrete.Finalizada) {
            _cargaSubContratacao.ImprimirContrato.visible(true);
            _cargaSubContratacao.ImprimirRomaneioEntrega.visible(true);
        } else {
            _cargaSubContratacao.ImprimirContrato.visible(false);
            _cargaSubContratacao.ImprimirRomaneioEntrega.visible(false);
        }
    }

    if (contratoFrete.PossuiIntegracaoAX && (!contratoFrete.EnviouContratoAXComSucesso || !contratoFrete.EnviouAcertoContasContratoAXComSucesso))
        _cargaSubContratacao.EnviarIntegracaoQuitacaoAX.visible(true);
    else
        _cargaSubContratacao.EnviarIntegracaoQuitacaoAX.visible(false);

    _cargaSubContratacao.HistoricoIntegracao.visible(true);
    _cargaSubContratacao.PercentualAdiantamento.visible(true);
    _cargaSubContratacao.PercentualAbastecimento.visible(true);
    _cargaSubContratacao.Observacao.visible(true);

    if (_cargaSubContratacao.PossuiCIOT.val() === true)
        BuscarCIOTCargaSubcontratada(_cargaSubContratacao);
    else
        buscarCargasPreCTe(_cargaSubContratacao);
}


function preecherDadosContrato(contratoFrete, knoutDadosContrato) {
    knoutDadosContrato.Carga.val(_cargaAtual.Codigo.val());
    knoutDadosContrato.Transbordo.val(contratoFrete.Transbordo);
    knoutDadosContrato.Codigo.val(contratoFrete.Codigo);
    knoutDadosContrato.DataEmissaoContrato.val(contratoFrete.DataEmissaoContrato);
    knoutDadosContrato.Observacao.val(contratoFrete.Observacao);
    knoutDadosContrato.SituacaoContratoFrete.val(contratoFrete.SituacaoContratoFrete);
    knoutDadosContrato.TipoFreteEscolhido.val(contratoFrete.TipoFreteEscolhido);
    knoutDadosContrato.DescricaoTipoFreteEscolhido.val(contratoFrete.DescricaoTipoFreteEscolhido);
    knoutDadosContrato.DescricaoSituacaoContratoFrete.val(contratoFrete.DescricaoSituacaoContratoFrete);
    knoutDadosContrato.Terceiro.val(contratoFrete.Terceiro);
    knoutDadosContrato.ValorFreteSubcontratacao.val(Globalize.format(contratoFrete.ValorFreteSubcontratacao, "n2"));
    knoutDadosContrato.ValorFreteSubContratacaoTabelaFrete.val(Globalize.format(contratoFrete.ValorFreteSubContratacaoTabelaFrete, "n2"));
    knoutDadosContrato.PossuiCIOT.val(contratoFrete.PossuiCIOT);
    knoutDadosContrato.Bloqueado.val(contratoFrete.Bloqueado);
    knoutDadosContrato.JustificativaBloqueio.val(contratoFrete.JustificativaBloqueio);

    knoutDadosContrato.ValorAdiantamento.val(Globalize.format(contratoFrete.ValorAdiantamento, "n2"));
    knoutDadosContrato.ValorAbastecimento.val(Globalize.format(contratoFrete.ValorAbastecimento, "n2"));

    knoutDadosContrato.PercentualAdiantamento.val(Globalize.format(contratoFrete.PercentualAdiantamento, "n2"));
    knoutDadosContrato.PercentualAbastecimento.val(Globalize.format(contratoFrete.PercentualAbastecimento, "n2"));

    knoutDadosContrato.ValorPedagio.val(Globalize.format(contratoFrete.ValorPedagio, "n2"));
    knoutDadosContrato.ValorTotalAPagar.val(Globalize.format(contratoFrete.ValorLiquidoSemAdiantamento, "n2"));
}

function BuscarCIOTCargaSubcontratada(knoutContrato, callback) {
    $("#divCTesCargaContratoFrete").hide();
    $("#divCIOTsCargaContratoFrete").show();

    var baixarContratoTransporte = { descricao: Localization.Resources.Cargas.Carga.BaixarContratoDeTransporte, id: guid(), metodo: BaixarContratoTransporteCIOT, icone: "" };
    var menuOpcoes = { tipo: TypeOptionMenu.list, descricao: Localization.Resources.Gerais.Geral.Opcoes, tamanho: 7, opcoes: [baixarContratoTransporte] };
    _gridCIOTSubcontratacao = new GridView(knoutContrato.PesquisarCIOT.idGrid, "ContratoFrete/ConsultarCIOT", knoutContrato, menuOpcoes, null);
    _gridCIOTSubcontratacao.CarregarGrid(callback);
}

function buscarCargasPreCTe(knoutContrato, callback) {
    $("#divCTesCargaContratoFrete").show();
    $("#divCIOTsCargaContratoFrete").hide();

    var enviarCTeContrato = {
        descricao: "Enviar CT-e", id: guid(), metodo:
            function (row) {
                AbrirEnvioCTeDoPreCTeContratoClick(row);
            }, icone: "", visibilidade: VisibilidadeEnvio
    };
    var baixarXMLPreCTe = { descricao: Localization.Resources.Cargas.Carga.BaixarXMLDoPreCTe, id: guid(), metodo: baixarXMLPreCTeClick, icone: "" };
    var baixarXMLCTeTerceiro = { descricao: Localization.Resources.Cargas.Carga.BaixarXMLDoCTeDoTerceiro, id: guid(), metodo: baixarXMLTerceiroClick, icone: "", visibilidade: VisibilidadeEnviado };
    var menuOpcoes = { tipo: TypeOptionMenu.list, descricao: Localization.Resources.Gerais.Geral.Opcoes, tamanho: 7, opcoes: [baixarXMLPreCTe, enviarCTeContrato, baixarXMLCTeTerceiro] };
    _gridCargaCTe = new GridView(knoutContrato.Pesquisar.idGrid, "ContratoFrete/ConsultarContratoFreteCTe", knoutContrato, menuOpcoes, null);
    _gridCargaCTe.CarregarGrid(callback);
}

function BaixarContratoTransporteCIOT(data) {
    executarDownload("CIOT/DownloadContrato", { Codigo: data.Codigo });
}

function baixarXMLPreCTeClick(data) {
    var data = { CodigoPreCTe: parseInt(data.CodigoPreCTE) };
    executarDownload("CargaPreCTe/DownloadPreXML", data);
}

function baixarXMLTerceiroClick(data) {
    var data = { ContratoFreteCTe: parseInt(data.Codigo) };
    executarDownload("ContratoFrete/DownloadXMLCTeTerceiro", data);
}


var _codigoPreCteContrato;
var _codigoContratoCTe;
function AbrirEnvioCTeDoPreCTeContratoClick(row) {
    _codigoContratoCTe = row.Codigo;
    _codigoPreCteContrato = row.CodigoPreCTE;
    $('#FileEnviarCTeDoPreCTeContrato').val("");
    $("#FileEnviarCTeDoPreCTeContrato").trigger("click");
}

function EnviarCTeDoPreCTeContratoClick() {
    if ($('#FileEnviarCTeDoPreCTeContrato').val() != "") {
        var file = document.getElementById("FileEnviarCTeDoPreCTeContrato");
        exibirConfirmacao(Localization.Resources.Gerais.Geral.Confirmacao, Localization.Resources.Cargas.Carga.RealmenteDesejaEnviarArquivoComoCTeDestePreCTe.format(file.files[0].name), function () {

            var formData = new FormData();
            formData.append("upload", file.files[0]);
            var data = {
                CodigoPreCTe: _codigoPreCteContrato,
                ContratoFreteCTe: _codigoContratoCTe
            };

            enviarArquivo("ContratoFrete/EnviarCTe?callback=?", data, formData, function (arg) {
                if (arg.Success) {
                    if (arg.Data) {
                        exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, Localization.Resources.Cargas.Carga.CTeEnviadoComSucesso);
                        _gridCargaCTe.CarregarGrid();
                    } else {
                        exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Atencao, arg.Msg, 30000);
                    }
                } else {
                    exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, arg.Msg);
                }
            });
        });
    }
}

//*******MÉTODOS*******


function VisibilidadeEnvio(data) {
    if (data.CteEnviado)
        return false;
    else
        return true;
}

function VisibilidadeEnviado(data) {
    if (data.CteEnviado)
        return true;
    else
        return false;
}


function EtapaSubContratacaoDesabilitada(e) {
    $("#" + e.EtapaSubContratacao.idTab).removeAttr("data-bs-toggle");
    $("#" + e.EtapaSubContratacao.idTab + " .step").attr("class", "step");
    e.EtapaSubContratacao.eventClick = function (e) { };
}

function EtapaSubContratacaoLiberada(e) {
    $("#" + e.EtapaSubContratacao.idTab).attr("data-bs-toggle", "tab");
    $("#" + e.EtapaSubContratacao.idTab + " .step").attr("class", "step yellow");
    e.EtapaSubContratacao.eventClick = verificarSubContratacaoClick;
}

function EtapaSubContratacaoAguardando(e) {
    $("#" + e.EtapaSubContratacao.idTab).attr("data-bs-toggle", "tab");
    $("#" + e.EtapaSubContratacao.idTab + " .step").attr("class", "step yellow");
    e.EtapaSubContratacao.eventClick = verificarSubContratacaoClick;
}


function EtapaSubContratacaoAprovada(e) {
    $("#" + e.EtapaSubContratacao.idTab).attr("data-bs-toggle", "tab");
    $("#" + e.EtapaSubContratacao.idTab + " .step").attr("class", "step green");

    e.EtapaSubContratacao.eventClick = verificarSubContratacaoClick;
}

function EtapaSubContratacaoProblema(e) {
    $("#" + e.EtapaSubContratacao.idTab).attr("data-bs-toggle", "tab");
    $("#" + e.EtapaSubContratacao.idTab + " .step").attr("class", "step red");
    EtapaDadosTransportadorDesabilitada(e);
    e.EtapaSubContratacao.eventClick = verificarSubContratacaoClick;
}

function EtapaSubContratacaoEdicaoDesabilitada(e) {
    e.EtapaFreteEmbarcador.enable(false);
    e.AutorizarEmissaoDocumentos.enable(false);
    e.EtapaSubContratacao.eventClick = null;
}


