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
/// <reference path="../../../Enumeradores/EnumSituacaoIntegracaoCarga.js" />

var _tipoIntegracaoCarga = new Array();
var _gridIntegracaoCarga;
var _gridHistoricoIntegracaoCarga;
var _integracaoCarga;
var _pesquisaHistoricoIntegracaoCarga;

var PesquisaHistoricoIntegracaoCarga = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
};

var IntegracaoCarga = function () {

    this.Carga = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Tipo = PropertyEntity({ val: ko.observable(""), options: _tipoIntegracaoCarga, text: Localization.Resources.Cargas.Carga.Integracao.getFieldDescription(), def: "", issue: 267, visible: ko.observable(false) });
    this.Situacao = PropertyEntity({ val: ko.observable(EnumSituacaoIntegracaoCarga.Todas), options: EnumSituacaoIntegracaoCarga.obterOpcoesPesquisa(), text: Localization.Resources.Cargas.Carga.Situacao.getFieldDescription(), def: EnumSituacaoIntegracaoCarga.Todas, issue: 272 });

    this.TotalGeral = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int, text: Localization.Resources.Cargas.Carga.TotalGeral.getFieldDescription() });
    this.TotalAguardandoIntegracao = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int, text: Localization.Resources.Cargas.Carga.AguardandoIntegracao.getFieldDescription() });
    this.TotalAguardandoRetorno = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int, text: Localization.Resources.Cargas.Carga.AguardandoRetorno.getFieldDescription() });
    this.TotalProblemaIntegracao = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int, text: Localization.Resources.Cargas.Carga.ProblemasNaIntegracao.getFieldDescription() });
    this.TotalIntegrado = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int, text: Localization.Resources.Cargas.Carga.Integrados.getFieldDescription() });

    this.DownloadLoteDocumentos = PropertyEntity({
        eventClick: DownloadLoteDocumentosClick, type: types.event, text: Localization.Resources.Cargas.Carga.DocumentosDaCarga, idGrid: guid(), visible: ko.observable(false)
    });

    this.IntegracaoFilialEmissora = PropertyEntity({ val: ko.observable(false), def: false, getType: typesKnockout.bool });

    this.Pesquisar = PropertyEntity({
        eventClick: function (e) {
            _gridIntegracaoCarga.CarregarGrid();
            ObterTotaisIntegracaoCarga();
        }, type: types.event, text: Localization.Resources.Gerais.Geral.Pesquisar, idGrid: guid(), visible: ko.observable(true)
    });

    this.ReenviarTodos = PropertyEntity({
        eventClick: function (e) {
            ReenviarTodosIntegracaoCarga();
        }, type: types.event, text: Localization.Resources.Cargas.Carga.ReenviarTodos, idGrid: guid(), visible: ko.observable(false)
    });

    this.ObterTotais = PropertyEntity({
        eventClick: function (e) {
            ObterTotaisIntegracaoCarga();
        }, type: types.event, text: Localization.Resources.Cargas.Carga.ObterTotais, idGrid: guid(), visible: ko.observable(true)
    });
};

function LoadIntegracaoCarga(carga, idKnockoutIntegracaoCarga) {

    _integracaoCarga = new IntegracaoCarga();
    _integracaoCarga.Carga.val(carga.Codigo.val());
    _integracaoCarga.IntegracaoFilialEmissora.val(_integracaoGeral.FilialEmissora.val());

    KoBindings(_integracaoCarga, idKnockoutIntegracaoCarga);

    if (_CONFIGURACAO_TMS.TipoServicoMultisoftware === EnumTipoServicoMultisoftware.MultiTMS)
        _integracaoCarga.DownloadLoteDocumentos.visible(true);
    else
        _integracaoCarga.DownloadLoteDocumentos.visible(false);

    if (VerificaSePossuiPermissaoEspecial(EnumPermissaoPersonalizada.Carga_ReenviarIntegracoes, _PermissoesPersonalizadasCarga))
        _integracaoCarga.ReenviarTodos.visible(true);

    if (_CONFIGURACAO_TMS.TipoServicoMultisoftware === EnumTipoServicoMultisoftware.MultiCTe) {
        _integracaoCarga.ReenviarTodos.visible(false);
    }

    ObterTotaisIntegracaoCarga();
    ConfigurarPesquisaIntegracaoCarga();
}

function ConfigurarPesquisaIntegracaoCarga() {

    var menuOpcoes = { tipo: TypeOptionMenu.list, descricao: Localization.Resources.Gerais.Geral.Opcoes, tamanho: 7, opcoes: [] };
    menuOpcoes.opcoes.push({ descricao: Localization.Resources.Cargas.Carga.ComprovanteAgendamento, id: guid(), metodo: ObterComprovanteAgendamento, tamanho: "20", icone: "", visibilidade: VisibilidadeRotogramaIntegracaoCarga });
    menuOpcoes.opcoes.push({ descricao: Localization.Resources.Cargas.Carga.ObterRotograma, id: guid(), metodo: ObterRotogramaIntegracao, tamanho: "20", icone: "", visibilidade: VisibilidadeRotogramaIntegracaoCarga });
    menuOpcoes.opcoes.push({ descricao: Localization.Resources.Cargas.Carga.Reenviar, id: guid(), metodo: ReenviarIntegracaoCarga, tamanho: "20", icone: "", visibilidade: VisibilidadeOpcaoReenviarIntegracaoCarga });
    menuOpcoes.opcoes.push({ descricao: Localization.Resources.Cargas.Carga.AnteciparEnvio, id: guid(), metodo: AnteciparEnvioIntegracaoCarga, tamanho: "20", icone: "", visibilidade: VisibilidadeOpcaoAnteciparEnvioIntegracaoCarga });
    menuOpcoes.opcoes.push({ descricao: Localization.Resources.Cargas.Carga.HistoricoDeIntegracao, id: guid(), metodo: ExibirHistoricoIntegracaoCarga, tamanho: "20", icone: "" });
    menuOpcoes.opcoes.push({ descricao: Localization.Resources.Cargas.Carga.DownloadAutorizacaoEmbarque, id: guid(), metodo: DownloadAutorizacaoEmbarque, tamanho: "20", icone: "", visibilidade: VisibilidadeOpcaoImpressaoAutorizacaoEmbarque });
    menuOpcoes.opcoes.push({ descricao: Localization.Resources.Gerais.Geral.Auditar, id: guid(), metodo: OpcaoAuditoria("CargaCargaIntegracao"), tamanho: "20", icone: "", visibilidade: VisibilidadeOpcaoAuditoria });
    menuOpcoes.opcoes.push({ descricao: Localization.Resources.Cargas.Carga.EnviarPorEmail, id: guid(), metodo: EnviarEmailAutorizacaoEmbarque, tamanho: "20", icone: "", visibilidade: VisibilidadeOpcaoEnviarEmailAutorizacaoEmbarque });
    menuOpcoes.opcoes.push({ descricao: Localization.Resources.Cargas.Carga.DownloadMICDTA, id: guid(), metodo: DownloadMicDta, tamanho: "20", icone: "", visibilidade: VisibilidadeOpcaoDownloadMicDta });

    _gridIntegracaoCarga = new GridView(_integracaoCarga.Pesquisar.idGrid, "CargaIntegracaoCarga/Pesquisa", _integracaoCarga, menuOpcoes);
    _gridIntegracaoCarga.CarregarGrid();
}

function VisibilidadeOpcaoDownloadMicDta(data) {
    return data.Tipo == EnumTipoIntegracao.MicDta;
}

function VisibilidadeOpcaoImpressaoAutorizacaoEmbarque(data) {
    if (data.Tipo == EnumTipoIntegracao.OpenTech)
        return true;

    return false;
}

function VisibilidadeOpcaoReenviarIntegracaoCarga(data) {
    if ((!VerificaSePossuiPermissaoEspecial(EnumPermissaoPersonalizada.Carga_ReenviarIntegracoes, _PermissoesPersonalizadasCarga)) && (data.SituacaoIntegracao != EnumSituacaoIntegracao.ProblemaIntegracao))
        return false;

    if (data.Tipo == EnumTipoIntegracao.KMM && data.SituacaoIntegracao != EnumSituacaoIntegracao.ProblemaIntegracao)
        return false;

    if ((_CONFIGURACAO_TMS.NaoPermitirReenviarIntegracaoDasCargasAppTrizy) && data.Tipo == 64)
        return false;

    return true;
}

function VisibilidadeOpcaoAnteciparEnvioIntegracaoCarga(data) {
    if (VerificaSePossuiPermissaoEspecial(EnumPermissaoPersonalizada.Carga_ReenviarIntegracoes, _PermissoesPersonalizadasCarga) && (data.SituacaoIntegracao == EnumSituacaoIntegracao.AgIntegracao && data.DataEnvio < Date.now && data.Tentativas == 0))
        return true;

    return false;
}

function VisibilidadeRotogramaIntegracaoCarga(data) {
    if ((data.Tipo == EnumTipoIntegracao.AngelLira || data.Tipo == EnumTipoIntegracao.BrasilRisk) && data.SituacaoIntegracao != EnumSituacaoIntegracao.ProblemaIntegracao)
        return true;

    return false;
}

function VisibilidadeOpcaoEnviarEmailAutorizacaoEmbarque(data) {
    if (_CONFIGURACAO_TMS.PermitirEnviarEmailAutorizacaoEmbarque && data.Tipo == EnumTipoIntegracao.OpenTech)
        return true;

    return false;
}

function ObterTiposIntegracaoCarga() {
    var p = new promise.Promise();

    var dados = {
        Tipos: JSON.stringify([
            EnumTipoIntegracao.OpenTech,
            EnumTipoIntegracao.AngelLira,
            EnumTipoIntegracao.BrasilRisk,
            EnumTipoIntegracao.NOX,
            EnumTipoIntegracao.MundialRisk,
            EnumTipoIntegracao.Logiun,
            EnumTipoIntegracao.Raster,
            EnumTipoIntegracao.UnileverFourKites
        ]),
        IntegracaoTransportador: false
    };

    executarReST("TipoIntegracao/BuscarTodos", dados, function (r) {
        if (r.Success) {

            _tipoIntegracaoCarga.push({ value: "", text: Localization.Resources.Gerais.Geral.Todas });

            for (var i = 0; i < r.Data.length; i++)
                _tipoIntegracaoCarga.push({ value: r.Data[i].Codigo, text: r.Data[i].Descricao });

        } else {
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, r.Msg);
        }

        p.done();
    });

    return p;
}

function ObterComprovanteAgendamento(data) {
    executarReST("CargaIntegracaoCarga/ObterComprovanteAgendamento", { Codigo: data.Codigo }, function (r) {
        if (r.Success) {
            if (r.Data !== false) {
                if (r.Data.status != false) {
                    window.open(r.Data.link);
                } else {
                    exibirMensagem(tipoMensagem.aviso, Localization.Resources.Cargas.Carga.RetornoDaIntegradora, r.Data.mensagemErro);
                }
            } else {
                exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.Aviso, r.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, r.Msg);
        }
    });
}

function ObterRotogramaIntegracao(data) {
    if (data.Tipo === EnumTipoIntegracao.AngelLira) {
        executarReST("CargaIntegracaoCarga/ObterRotogramaIntegracao", { Codigo: data.Codigo }, function (r) {
            if (r.Success) {
                if (r.Data !== false) {
                    if (r.Data.status != false) {
                        window.open(r.Data.link);
                    } else {
                        exibirMensagem(tipoMensagem.aviso, Localization.Resources.Cargas.Carga.RetornoDaIntegradora, r.Data.mensagemErro);
                    }
                } else {
                    exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.Aviso, r.Msg);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, r.Msg);
            }
        });
    }
    else if (data.Tipo === EnumTipoIntegracao.BrasilRisk) {
        executarDownload("CargaIntegracaoCarga/BaixarRotograma", { Codigo: data.Codigo });
    }
}

function ReenviarIntegracaoCarga(data) {
    executarReST("CargaIntegracaoCarga/Reenviar", { Codigo: data.Codigo }, function (r) {
        if (r.Success) {
            if (r.Data) {
                exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, Localization.Resources.Cargas.Carga.ReenvioSolicitadoComSucesso);
                _gridIntegracaoCarga.CarregarGrid();
            } else {
                exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.Aviso, r.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, r.Msg);
        }
    });
}

function AnteciparEnvioIntegracaoCarga(data) {
    executarReST("CargaIntegracaoCarga/AnteciparEnvio", { Codigo: data.Codigo }, function (r) {
        if (r.Success) {
            exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, Localization.Resources.Cargas.Carga.ReenvioSolicitadoComSucesso);
            _gridIntegracaoCarga.CarregarGrid();
        } else {
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, r.Msg);
        }
    });
}

function DownloadLoteDocumentosClick(e) {
    executarDownload("CargaIntegracaoCarga/DownloadLoteDocumentos", { Carga: _integracaoCarga.Carga.val() });
    //executarReST("CargaIntegracaoCarga/DownloadLoteDocumentos", { Carga: _integracaoCarga.Carga.val() }, function (arg) {
    //    if (arg.Success) {
    //        if (arg.Data) {
    //            exibirMensagem(tipoMensagem.ok, "Sucesso", "Solicitação realizada com sucesso, favor aguarde o arquivo ser gerado.", 20000);
    //        } else {
    //            exibirMensagem(tipoMensagem.atencao, "Atenção", arg.Msg, 20000);
    //        }
    //    } else {
    //        exibirMensagem(tipoMensagem.falha, "falha", arg.Msg);
    //    }
    //});    
}

function ReenviarTodosIntegracaoCarga() {
    exibirConfirmacao(Localization.Resources.Gerais.Geral.Atencao, Localization.Resources.Cargas.Carga.DesejaRealmenteReenviarTodasAsIntegracoes, function () {
        executarReST("CargaIntegracaoCarga/ReenviarTodos", { Carga: _integracaoCarga.Carga.val(), Tipo: _integracaoCarga.Tipo.val(), Situacao: _integracaoCarga.Situacao.val() }, function (r) {
            if (r.Success) {
                exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, Localization.Resources.Cargas.Carga.ReenvioSolicitadoComSucesso);
                _gridIntegracaoCarga.CarregarGrid();
            } else {
                exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, r.Msg);
            }
        });
    });
}

function ObterTotaisIntegracaoCarga() {
    executarReST("CargaIntegracaoCarga/ObterTotais", { Carga: _integracaoCarga.Carga.val() }, function (r) {
        if (r.Success) {
            _integracaoCarga.TotalGeral.val(r.Data.TotalGeral);
            _integracaoCarga.TotalAguardandoIntegracao.val(r.Data.TotalAguardandoIntegracao);
            _integracaoCarga.TotalAguardandoRetorno.val(r.Data.TotalAguardandoRetorno);
            _integracaoCarga.TotalProblemaIntegracao.val(r.Data.TotalProblemaIntegracao);
            _integracaoCarga.TotalIntegrado.val(r.Data.TotalIntegrado);
        } else {
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, r.Msg);
        }
    });
}

function ExibirHistoricoIntegracaoCarga(integracao) {
    BuscarHistoricoIntegracaoCarga(integracao);
    Global.abrirModal("divModalHistoricoIntegracaoCTe");
}

function BuscarHistoricoIntegracaoCarga(integracao) {
    _pesquisaHistoricoIntegracaoCarga = new PesquisaHistoricoIntegracaoCarga();
    _pesquisaHistoricoIntegracaoCarga.Codigo.val(integracao.Codigo);

    var download = { descricao: Localization.Resources.Cargas.Carga.DownloadArquivos, id: guid(), evento: "onclick", metodo: DownloadArquivosHistoricoIntegracaoCarga, tamanho: "20", icone: "" };

    var menuOpcoes = {
        tipo: TypeOptionMenu.link,
        opcoes: [download]
    };

    _gridHistoricoIntegracaoCarga = new GridView("tblHistoricoIntegracaoCTe", "CargaIntegracaoCarga/ConsultarHistoricoIntegracao", _pesquisaHistoricoIntegracaoCarga, menuOpcoes, { column: 1, dir: orderDir.desc });
    _gridHistoricoIntegracaoCarga.CarregarGrid();
}

function DownloadArquivosHistoricoIntegracaoCarga(historicoConsulta) {
    executarDownload("CargaIntegracaoCarga/DownloadArquivosHistoricoIntegracao", { Codigo: historicoConsulta.Codigo });
}

function DownloadAutorizacaoEmbarque(data) {
    executarDownload("CargaIntegracaoOpenTech/DownloadAutorizacaoEmbarque", { Codigo: data.Codigo });
}

function EnviarEmailAutorizacaoEmbarque(data) {
    exibirConfirmacao(Localization.Resources.Gerais.Geral.Atencao, Localization.Resources.Cargas.Carga.DesejaEnviarEmailParaOsParticipantesDaCarga, function () {
        executarReST("CargaIntegracaoOpenTech/EnviarEmailAutorizacaoEmbarque", { Codigo: data.Codigo }, function (r) {
            if (r.Success) {
                exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, Localization.Resources.Cargas.Carga.EmailEnviadoComSucesso);
                _gridIntegracaoCarga.CarregarGrid();
            } else {
                exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, r.Msg);
            }
        });
    });
}

function DownloadMicDta(data) {
    executarDownload("CargaIntegracaoCarga/DownloadMicDta", { Codigo: data.Codigo });
}

function RecarregarIntegracaoCargaViaSignalR(knoutCarga) {
    if (_cargaAtual != null && _cargaAtual.DivCarga.id == knoutCarga.DivCarga.id) {
        if ($("#divIntegracaoCarga_" + _cargaAtual.EtapaIntegracao.idGrid).is(":visible") && _gridIntegracaoCarga != null) {
            _gridIntegracaoCarga.CarregarGrid();
            ObterTotaisIntegracaoCarga();
        }
    }
}