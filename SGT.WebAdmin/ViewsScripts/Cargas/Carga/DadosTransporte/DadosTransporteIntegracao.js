var _gridCargaDadosTransporteIntegracao;
var _gridHistoricoCargaDadosTransporteIntegracao;
var _cargaDadosTransporteIntegracao;
var _pesquisaHistoricoCargaDadosTransporteIntegracao;
var _HTMLCargaDadosTransporteIntegracao;
var _cargaDadosTransporteIntegracaoGeral;
var _ehTransportador = _CONFIGURACAO_TMS.TipoServicoMultisoftware === EnumTipoServicoMultisoftware.MultiCTe;

var CargaDadosTransporteIntegracaoGeral = function () {
    this.Carga = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });

    this.FinalizarEtapa = PropertyEntity({
        eventClick: function (e) {
            FinalizarEtapaCargaDadosTransporteIntegracao();
        }, type: types.event, text: "Finalizar Etapa", idGrid: guid(), visible: ko.observable(true)
    });
}

var PesquisaHistoricoCargaDadosTransporteIntegracao = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
}

var CargaDadosTransporteIntegracao = function () {

    this.Carga = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Situacao = PropertyEntity({ val: ko.observable(""), options: EnumSituacaoIntegracaoCarga.obterOpcoesPesquisa(), text: Localization.Resources.Gerais.Geral.Situacao.getFieldDescription(), def: "", issue: 272 });

    this.TotalGeral = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int, text: Localization.Resources.Cargas.Carga.TotalGeral.getFieldDescription() });
    this.TotalAguardandoIntegracao = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int, text: Localization.Resources.Cargas.Carga.AguardandoIntegracao.getFieldDescription() });
    this.TotalAguardandoRetorno = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int, text: Localization.Resources.Cargas.Carga.AguardandoRetorno.getFieldDescription() });
    this.TotalProblemaIntegracao = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int, text: Localization.Resources.Cargas.Carga.ProblemasNaIntegracao.getFieldDescription() });
    this.TotalIntegrado = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int, text: Localization.Resources.Cargas.Carga.Integrados.getFieldDescription() });
    this.Visibilidade = PropertyEntity({ val: ko.observable(false), def: false });

    this.Pesquisar = PropertyEntity({
        eventClick: function (e) {
            _gridCargaDadosTransporteIntegracao.CarregarGrid();
            ObterTotaisCargaDadosTransporteIntegracao();
        }, type: types.event, text: Localization.Resources.Gerais.Geral.Pesquisar, idGrid: guid(), visible: ko.observable(true)
    });

    this.ReenviarTodos = PropertyEntity({
        eventClick: function (e) {
            ReenviarTodosCargaDadosTransporteIntegracao();
        }, type: types.event, text: Localization.Resources.Cargas.Carga.ReenviarTodos, idGrid: guid(), visible: ko.observable(false)
    });

    this.ObterTotais = PropertyEntity({
        eventClick: function (e) {
            ObterTotaisCargaDadosTransporteIntegracao();
        }, type: types.event, text: Localization.Resources.Cargas.Carga.ObterTotais, idGrid: guid(), visible: ko.observable(true)
    });
}

function LoadCargaDadosTransporteIntegracao(carga, estapaInicioSemExigirNota) {

    let exigeNota = carga.ExigeNotaFiscalParaCalcularFrete.val();
    let origemId = ObterOrigemId(carga, exigeNota, estapaInicioSemExigirNota);

    $("#liTabCargaDadosTransporteIntegracao_" + carga.EtapaInicioTMS.idGrid).removeClass("d-none");

    CarregarHTMLCargaDadosTransporteIntegracao().then(function () {
        var idContent = ObterIdContent(origemId, exigeNota, estapaInicioSemExigirNota);
        var idKnockout = 'divIntegracaoCarga_' + origemId;
        var idKnockoutGeral = 'divIntegracao_' + origemId;

        SetarHTMLCargaDadosTransporteIntegracao(idContent, origemId);

        Global.ResetarAba(idKnockoutGeral);

        _cargaDadosTransporteIntegracaoGeral = new CargaDadosTransporteIntegracaoGeral();
        _cargaDadosTransporteIntegracaoGeral.Carga.val(carga.Codigo.val());

        KoBindings(_cargaDadosTransporteIntegracaoGeral, idKnockoutGeral);

        _cargaDadosTransporteIntegracao = new CargaDadosTransporteIntegracao();
        _cargaDadosTransporteIntegracao.Carga.val(carga.Codigo.val());

        KoBindings(_cargaDadosTransporteIntegracao, idKnockout);

        LocalizeCurrentPage();

        if (VerificaSePossuiPermissaoEspecial(EnumPermissaoPersonalizada.Carga_ReenviarIntegracoes, _PermissoesPersonalizadasCarga)) {
            if (_ehTransportador)
                _cargaDadosTransporteIntegracao.ReenviarTodos.visible(false);
            else
                _cargaDadosTransporteIntegracao.ReenviarTodos.visible(true);
        } 

        if (!VerificaSePossuiPermissaoEspecial(EnumPermissaoPersonalizada.Carga_ConfirmarIntegracao, _PermissoesPersonalizadasCarga))
            _cargaDadosTransporteIntegracaoGeral.FinalizarEtapa.visible(false);
        else if (carga.SituacaoCarga.val() == EnumSituacoesCarga.Nova)
            _cargaDadosTransporteIntegracaoGeral.FinalizarEtapa.visible(true);

        if (!_CONFIGURACAO_TMS.UsuarioAdministrador && !VerificaSePossuiPermissaoEspecial(EnumPermissaoPersonalizada.Carga_PermiteFinalizarIntegracaoEtapa1Carga, _PermissoesPersonalizadasCarga))
            _cargaDadosTransporteIntegracaoGeral.FinalizarEtapa.visible(false);

        ObterTotaisCargaDadosTransporteIntegracao(origemId, idContent, estapaInicioSemExigirNota);
        ConfigurarPesquisaCargaDadosTransporteIntegracao(estapaInicioSemExigirNota);
    });
}

function CarregarHTMLCargaDadosTransporteIntegracao() {
    var p = new promise.Promise();

    if (_HTMLCargaDadosTransporteIntegracao == null) {
        $.get("Content/Static/Carga/CargaDadosTransporteIntegracao.html?dyn=" + guid(), function (data) {
            _HTMLCargaDadosTransporteIntegracao = data;
            p.done();
        });
    } else {
        p.done();
    }

    return p;
}

function SetarHTMLCargaDadosTransporteIntegracao(idContent, idReplace) {
    $("#" + idContent).html(_HTMLCargaDadosTransporteIntegracao.replace(/#divIntegracao/g, idReplace));
}

function ConfigurarPesquisaCargaDadosTransporteIntegracao(estapaInicioSemExigirNota) {

    var menuOpcoes = { tipo: TypeOptionMenu.list, descricao: Localization.Resources.Gerais.Geral.Opcoes, tamanho: 7, opcoes: [] };

    menuOpcoes.opcoes.push({ descricao: Localization.Resources.Cargas.Carga.ComprovanteAgendamento, id: guid(), metodo: ObterComprovanteAgendamentoCargaDadosTransporteIntegracao, tamanho: "20", icone: "", visibilidade: VisibilidadeRotogramaCargaDadosTransporteIntegracao });
    menuOpcoes.opcoes.push({ descricao: Localization.Resources.Cargas.Carga.ObterRotograma, id: guid(), metodo: ObterRotogramaCargaDadosTransporteIntegracao, tamanho: "20", icone: "", visibilidade: VisibilidadeRotogramaCargaDadosTransporteIntegracao });
    menuOpcoes.opcoes.push({ descricao: Localization.Resources.Cargas.Carga.DownloadAutorizacaoEmbarque, id: guid(), metodo: DownloadAutorizacaoEmbarqueTransporte, tamanho: "20", icone: "", visibilidade: VisibilidadeOpcaoImpressaoAutorizacaoEmbarque });
    menuOpcoes.opcoes.push({ descricao: Localization.Resources.Cargas.Carga.Reenviar, id: guid(), metodo: ReenviarCargaDadosTransporteIntegracao, tamanho: "20", icone: "", visibilidade: VisibilidadeOpcaoReenviarCargaDadosTransporteIntegracao });
    menuOpcoes.opcoes.push({ descricao: Localization.Resources.Cargas.Carga.HistoricoDeIntegracao, id: guid(), metodo: ExibirHistoricoCargaDadosTransporteIntegracao, tamanho: "20", icone: "" });
    menuOpcoes.opcoes.push({ descricao: Localization.Resources.Gerais.Geral.Auditar, id: guid(), metodo: OpcaoAuditoria("CargaDadosTransporteIntegracao"), tamanho: "20", icone: "", visibilidade: VisibilidadeOpcaoAuditoria });

    _gridCargaDadosTransporteIntegracao = new GridView(_cargaDadosTransporteIntegracao.Pesquisar.idGrid, `CargaDadosTransporteIntegracao/Pesquisa?EtapaInicio=${estapaInicioSemExigirNota}`, _cargaDadosTransporteIntegracao, menuOpcoes);

    _gridCargaDadosTransporteIntegracao.CarregarGrid();
}

function DownloadAutorizacaoEmbarqueTransporte(data) {
    executarDownload("CargaDadosTransporteIntegracao/DownloadAutorizacaoEmbarque", { Codigo: data.Codigo });
}

function VisibilidadeOpcaoImpressaoAutorizacaoEmbarque(data) {
    if (data.Tipo == EnumTipoIntegracao.OpenTech)
        return true;

    return false;
}

function VisibilidadeOpcaoReenviarCargaDadosTransporteIntegracao(data) {
    if ((data.SituacaoIntegracao !== EnumSituacaoIntegracao.ProblemaIntegracao) && (!VerificaSePossuiPermissaoEspecial(EnumPermissaoPersonalizada.Carga_ReenviarIntegracoes, _PermissoesPersonalizadasCarga)))
        return false;

    if ((_CONFIGURACAO_TMS.NaoPermitirReenviarIntegracaoDasCargasAppTrizy) && data.Tipo == 64)
        return false;

    return true;
}

function VisibilidadeRotogramaCargaDadosTransporteIntegracao(data) {
    if (data.Tipo === EnumTipoIntegracao.AngelLira && data.SituacaoIntegracao !== EnumSituacaoIntegracao.ProblemaIntegracao)
        return true;

    return false;
}

function ObterComprovanteAgendamentoCargaDadosTransporteIntegracao(data) {
    executarReST("CargaDadosTransporteIntegracao/ObterComprovanteAgendamento", { Codigo: data.Codigo }, function (r) {
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

function ObterRotogramaCargaDadosTransporteIntegracao(data) {
    executarReST("CargaDadosTransporteIntegracao/ObterRotogramaIntegracao", { Codigo: data.Codigo }, function (r) {
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

function ReenviarCargaDadosTransporteIntegracao(data) {
    executarReST("CargaDadosTransporteIntegracao/Reenviar", { Codigo: data.Codigo }, function (r) {
        if (r.Success) {
            exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, Localization.Resources.Cargas.Carga.ReenvioSolicitadoComSucesso);
            _gridCargaDadosTransporteIntegracao.CarregarGrid();
        } else {
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, r.Msg);
        }
    });
}

function ReenviarTodosCargaDadosTransporteIntegracao() {
    exibirConfirmacao(Localization.Resources.Gerais.Geral.Atencao, Localization.Resources.Cargas.Carga.DesejaRealmenteReenviarTodasAsIntegracoes, function () {
        executarReST("CargaDadosTransporteIntegracao/ReenviarTodos", { Carga: _cargaDadosTransporteIntegracao.Carga.val(), Situacao: _cargaDadosTransporteIntegracao.Situacao.val() }, function (r) {
            if (r.Success) {
                exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, Localization.Resources.Cargas.Carga.ReenvioSolicitadoComSucesso);
                _gridCargaDadosTransporteIntegracao.CarregarGrid();
            } else {
                exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, r.Msg);
            }
        });
    });
}

function ObterTotaisCargaDadosTransporteIntegracao(origemId, idContent, estapaInicioSemExigirNota) {
    var dados = { Carga: _cargaDadosTransporteIntegracao.Carga.val(), EtapaInicio: estapaInicioSemExigirNota }
    executarReST("CargaDadosTransporteIntegracao/ObterTotais", dados, function (r) {
        if (r.Success) {
            _cargaDadosTransporteIntegracao.TotalGeral.val(r.Data.TotalGeral);
            _cargaDadosTransporteIntegracao.TotalAguardandoIntegracao.val(r.Data.TotalAguardandoIntegracao);
            _cargaDadosTransporteIntegracao.TotalAguardandoRetorno.val(r.Data.TotalAguardandoRetorno);
            _cargaDadosTransporteIntegracao.TotalProblemaIntegracao.val(r.Data.TotalProblemaIntegracao);
            _cargaDadosTransporteIntegracao.TotalIntegrado.val(r.Data.TotalIntegrado);
            _cargaDadosTransporteIntegracao.Visibilidade.val(r.Data.Visibilidade);

            if (!r.Data.Visibilidade || !origemId)
                $(`#liTabIntegracoesCarga_${origemId}`).hide();
            else
                $(`#liTabIntegracoesCarga_${origemId}`).show();

            ValidarVisibilidadeTab(idContent);
        } else {
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, r.Msg);
        }
    });
}

function ExibirHistoricoCargaDadosTransporteIntegracao(integracao) {
    BuscarHistoricoCargaDadosTransporteIntegracao(integracao);
    Global.abrirModal("divModalHistoricoIntegracaoCTe");
}

function BuscarHistoricoCargaDadosTransporteIntegracao(integracao) {
    _pesquisaHistoricoCargaDadosTransporteIntegracao = new PesquisaHistoricoCargaDadosTransporteIntegracao();
    _pesquisaHistoricoCargaDadosTransporteIntegracao.Codigo.val(integracao.Codigo);

    var download = { descricao: Localization.Resources.Cargas.Carga.DownloadArquivos, id: guid(), evento: "onclick", metodo: DownloadArquivosHistoricoCargaDadosTransporteIntegracao, tamanho: "20", icone: "" };

    var menuOpcoes = {
        tipo: TypeOptionMenu.link,
        opcoes: [download]
    };

    _gridHistoricoCargaDadosTransporteIntegracao = new GridView("tblHistoricoIntegracaoCTe", "CargaDadosTransporteIntegracao/ConsultarHistoricoIntegracao", _pesquisaHistoricoCargaDadosTransporteIntegracao, menuOpcoes, { column: 1, dir: orderDir.desc });
    _gridHistoricoCargaDadosTransporteIntegracao.CarregarGrid();
}

function DownloadArquivosHistoricoCargaDadosTransporteIntegracao(historicoConsulta) {
    executarDownload("CargaDadosTransporteIntegracao/DownloadArquivosHistoricoIntegracao", { Codigo: historicoConsulta.Codigo });
}

function RecarregarIntegracaoCargaDadosTransporteViaSignalR(knoutCarga) {
    if (_cargaAtual != null && _cargaAtual.DivCarga != null && knoutCarga != null && knoutCarga.DivCarga != null && _cargaAtual.DivCarga.id === knoutCarga.DivCarga.id) {
        if ($("#" + _cargaAtual.EtapaInicioTMS.idGrid).is(":visible") && _gridCargaDadosTransporteIntegracao !== null) {
            _gridCargaDadosTransporteIntegracao.CarregarGrid();
            ObterTotaisCargaDadosTransporteIntegracao();
        }
    }
}

function FinalizarEtapaCargaDadosTransporteIntegracao() {
    exibirConfirmacao("Atenção!", "Deseja realmente finalizar a etapa de dados de transporte sem concluir as integrações?", function () {
        executarReST("CargaDadosTransporteIntegracao/Finalizar", { Carga: _cargaDadosTransporteIntegracaoGeral.Carga.val() }, function (r) {
            if (r.Data != null) {
                if (r.Success) {
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Etapa finalizada com sucesso!");
                } else {
                    exibirMensagem(tipoMensagem.aviso, "Atenção!", r.Msg);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, "Falha!", r.Msg);
            }
        });
    });
}

function ObterOrigemId(carga, exigeNota, estapaInicioSemExigirNota) {
    if (estapaInicioSemExigirNota) {
        return carga.EtapaInicioEmbarcador.idGrid;
    }

    let id = exigeNota ? carga.EtapaInicioTMS.idGrid : carga.EtapaDadosTransportador.idGrid;
    return id;
}

function ObterIdContent(origemId, exigeNota, estapaInicoSemNota) {
    if (estapaInicoSemNota)
        return `tabIntegracoesCarga_${origemId}`;

    let id = exigeNota ? `tabCargaDadosTransporteIntegracao_${origemId}` : `tabTransportadorIntegracaoSAP_${origemId}`
    return id;
}

function ValidarVisibilidadeTab(id) {
    if (!id || !id.includes("tabTransportadorIntegracaoSAP_"))
        return;

    if (!_cargaDadosTransporteIntegracao.Visibilidade.val())
        $(`#${id}_li`).hide();
}