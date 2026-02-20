/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Globais.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="../../../js/Global/Validacao.js" />
/// <reference path="../../Configuracao/Sistema/ConfiguracaoTMS.js" />
/// <reference path="../../../ViewsScripts/Chamados/Chamado/Chamado.js" />
/// <reference path="../../Enumeradores/EnumEtapaFluxoGestaoPatio.js" />
/// <reference path="../../Enumeradores/EnumPermissaoPersonalizada.js" />
/// <reference path="../../Enumeradores/EnumSituacaoCargaGuarita.js" />
/// <reference path="../../Enumeradores/EnumTipoFluxoGestaoPatio.js" />
/// <reference path="GuaritaAnexo.js" />

// #region Objetos Globais do Arquivo

var _guaritaFluxoPatio;
var _gridGuaritaFluxoPatioNfe;

// #endregion Objetos Globais do Arquivo

// #region Classes

var GuaritaFluxoPatio = function () {
    this.Codigo = PropertyEntity({ getType: typesKnockout.int, val: ko.observable(0), def: 0 });
    this.Carga = PropertyEntity({ getType: typesKnockout.int, val: ko.observable(0), def: 0 });
    this.PreCarga = PropertyEntity({ getType: typesKnockout.int, val: ko.observable(0), def: 0 });
    this.Situacao = PropertyEntity({ getType: typesKnockout.int, val: ko.observable(0), def: 0 });
    this.Etapa = PropertyEntity({ getType: typesKnockout.int, val: ko.observable(0), type: types.local });
    this.EtapaAntecipada = PropertyEntity({ getType: typesKnockout.bool, val: ko.observable(false), type: types.local });

    this.NumeroCarga = PropertyEntity({ text: Localization.Resources.GestaoPatio.FluxoPatio.Carga.getFieldDescription(), val: ko.observable(""), def: "" });
    this.NumeroPreCarga = PropertyEntity({ text: Localization.Resources.GestaoPatio.FluxoPatio.PreCarga.getFieldDescription(), val: ko.observable(""), def: "" });
    this.CargaData = PropertyEntity({ text: Localization.Resources.GestaoPatio.FluxoPatio.Data.getFieldDescription(), val: ko.observable(""), def: "" });
    this.CargaHora = PropertyEntity({ text: Localization.Resources.GestaoPatio.FluxoPatio.Hora.getFieldDescription(), val: ko.observable(""), def: "" });
    this.DescricaoSituacao = PropertyEntity({ text: Localization.Resources.Gerais.Geral.Situacao.getFieldDescription(), val: ko.observable(""), def: "" });
    this.Transportador = PropertyEntity({ text: Localization.Resources.GestaoPatio.FluxoPatio.Transportador.getFieldDescription(), val: ko.observable(""), def: "", visible: ko.observable(true) });
    this.Remetente = PropertyEntity({ text: Localization.Resources.GestaoPatio.FluxoPatio.Fornecedor.getFieldDescription(), val: ko.observable(""), def: "" });
    this.TipoCarga = PropertyEntity({ text: Localization.Resources.GestaoPatio.FluxoPatio.TipoDeCarga.getFieldDescription(), val: ko.observable(""), def: "" });
    this.TipoOperacao = PropertyEntity({ text: Localization.Resources.GestaoPatio.FluxoPatio.TipoDaOperacao.getFieldDescription(), val: ko.observable(""), def: "" });
    this.Atendimento = PropertyEntity({ text: "Atendimento", val: ko.observable(0), def: 0, visible: ko.observable(false) });

    this.Auditar = PropertyEntity({ visible: ko.observable(false), eventClick: auditarGuaritaClick });

    this.DataPrevisao = PropertyEntity({ text: Localization.Resources.GestaoPatio.FluxoPatio.DataPrevisaoCarregamento.getFieldDescription(), val: ko.observable(""), def: "" });
    this.DataCarregamento = PropertyEntity({ text: Localization.Resources.GestaoPatio.FluxoPatio.DataPrevisaoChegada.getFieldDescription(), val: ko.observable(""), def: "" });
    this.Motorista = PropertyEntity({ text: Localization.Resources.GestaoPatio.FluxoPatio.Motorista.getFieldDescription(), val: ko.observable(""), def: "" });
    this.MotoristaTelefone = PropertyEntity({ text: Localization.Resources.GestaoPatio.FluxoPatio.Telefone.getFieldDescription(), val: ko.observable(""), def: "" });
    this.MotoristaCelular = PropertyEntity({ text: Localization.Resources.GestaoPatio.FluxoPatio.Celular.getFieldDescription(), val: ko.observable(""), def: "" });
    this.Veiculo = PropertyEntity({ text: Localization.Resources.GestaoPatio.FluxoPatio.Veiculo.getFieldDescription(), val: ko.observable(""), def: "" });
    this.CodigoIntegracaoDestinatario = PropertyEntity({ val: ko.observable(""), def: "" });
    this.Destinatario = PropertyEntity({ text: Localization.Resources.GestaoPatio.FluxoPatio.Destinatario.getFieldDescription(), val: ko.observable(""), def: "" });

    this.ExibirDadosChegadaVeiculo = PropertyEntity({ val: ko.observable(false), getType: typesKnockout.bool, def: false });
    this.VeiculoChegada = PropertyEntity({ text: Localization.Resources.GestaoPatio.FluxoPatio.Veiculo.getRequiredFieldDescription(), val: ko.observable(""), def: "", maxlength: 7, required: true, enable: ko.observable(false), visible: ko.observable(true) });
    this.ReboqueChegada = PropertyEntity({ text: Localization.Resources.GestaoPatio.FluxoPatio.Reboque.getFieldDescription(), val: ko.observable(""), def: "", maxlength: 7, enable: ko.observable(false) });
    this.TelefoneChegada = PropertyEntity({ text: Localization.Resources.GestaoPatio.FluxoPatio.Telefone.getFieldDescription(), val: ko.observable(""), def: "", getType: typesKnockout.phone, enable: ko.observable(false) });
    this.MotoristaChegada = PropertyEntity({ text: Localization.Resources.GestaoPatio.FluxoPatio.Motorista.getRequiredFieldDescription(), val: ko.observable(""), def: "", required: true, enable: ko.observable(false) });
    this.CodigoMotoristaChegada = PropertyEntity({ getType: typesKnockout.int, val: ko.observable(0), def: 0 });
    this.CpfMotoristaChegada = PropertyEntity({ text: Localization.Resources.GestaoPatio.FluxoPatio.CPF.getRequiredFieldDescription(), maxlength: 14, getType: typesKnockout.cpf, idBtnSearch: guid(), enable: ko.observable(false), required: true });
    this.RgMotoristaChegada = PropertyEntity({ text: ko.observable(Localization.Resources.GestaoPatio.FluxoPatio.RG.getFieldDescription()), maxlength: 20, enable: ko.observable(false) });
    this.NumeroLacreChegada = PropertyEntity({ text: ko.observable("Lacre"), maxlength: 20, enable: ko.observable(false) });
    this.DocaChegada = PropertyEntity({ text: Localization.Resources.GestaoPatio.FluxoPatio.Doca.getFieldDescription(), val: ko.observable(""), def: "", maxlength: 100 });
    this.SenhaChegada = PropertyEntity({ text: Localization.Resources.GestaoPatio.FluxoPatio.Senha.getFieldDescription(), val: ko.observable(""), def: "", maxlength: 100 });

    this.AdicionarNfeChegadaVeiculo = PropertyEntity({ eventClick: adicionarNfeChegadaVeiculoClick, type: types.event, text: Localization.Resources.Gerais.Geral.Adicionar, visible: ko.observable(false) });
    this.ChaveNfeChegadaVeiculo = PropertyEntity({ text: Localization.Resources.GestaoPatio.FluxoPatio.ChaveNFe.getFieldDescription(), val: ko.observable(""), def: "", visible: ko.observable(false), maxlength: 50 });
    this.ListaNfeChegadaVeiculo = PropertyEntity({ type: types.local, getType: typesKnockout.dynamic, def: new Array(), val: ko.observable(new Array()), idGrid: guid() });

    this.NumeroNfProdutor = PropertyEntity({ text: Localization.Resources.GestaoPatio.FluxoPatio.NumeroNotaFiscalProdutor.getRequiredFieldDescription(), val: ko.observable(0), def: "", visible: ko.observable(false), required: ko.observable(false), getType: typesKnockout.int });
    this.PossuiDevolucao = PropertyEntity({ text: Localization.Resources.GestaoPatio.FluxoPatio.PossuiDevolucao, getType: typesKnockout.bool, val: ko.observable(false), def: false, visible: ko.observable(false) });
    this.ObservacaoDevolucao = PropertyEntity({ text: Localization.Resources.GestaoPatio.FluxoPatio.ObservacaoDaDevolucao.getFieldDescription(), val: ko.observable(""), def: "", maxlength: 2000 });

    this.Pesagem = PropertyEntity({ eventClick: pesagemClick, type: types.event, text: Localization.Resources.GestaoPatio.FluxoPatio.InformacoesDePesagemInicial, visible: ko.observable(true) });
    this.PesagemFinal = PropertyEntity({ eventClick: pesagemFinalClick, type: types.event, text: Localization.Resources.GestaoPatio.FluxoPatio.InformacoesDePesagemFinal, visible: ko.observable(true) });
    this.InformarChegadaVeiculo = PropertyEntity({ eventClick: informarChegadaVeiculoClick, type: types.event, text: Localization.Resources.GestaoPatio.FluxoPatio.ConfirmarChegada, visible: ko.observable(false) });
    this.InformarEntradaVeiculo = PropertyEntity({ eventClick: informarEntradaVeiculoClick, type: types.event, text: Localization.Resources.GestaoPatio.FluxoPatio.ConfirmarEntrada, visible: ko.observable(false) });
    this.InformarSaidaVeiculo = PropertyEntity({ eventClick: informarSaidaVeiculoClick, type: types.event, text: Localization.Resources.GestaoPatio.FluxoPatio.ConfirmarSaida, visible: ko.observable(false) });

    this.VoltarEtapaChegadaVeiculo = PropertyEntity({ eventClick: VoltarEtapaChegadaVeiculoClick, type: types.event, text: Localization.Resources.GestaoPatio.FluxoPatio.VoltarEtapa, visible: ko.observable(false) });
    this.VoltarEtapaEntradaVeiculo = PropertyEntity({ eventClick: VoltarEtapaEntradaVeiculoClick, type: types.event, text: Localization.Resources.GestaoPatio.FluxoPatio.VoltarEtapa, visible: ko.observable(false) });
    this.VoltarEtapaSaidaVeiculo = PropertyEntity({ eventClick: VoltarEtapaSaidaVeiculoClick, type: types.event, text: Localization.Resources.GestaoPatio.FluxoPatio.VoltarEtapa, visible: ko.observable(false) });

    this.RejeitarEtapaChegadaVeiculo = PropertyEntity({ eventClick: RejeitarEtapaChegadaVeiculoClick, type: types.event, text: Localization.Resources.GestaoPatio.FluxoPatio.RejeitarEtapa, visible: ko.observable(false) });
    this.RejeitarEtapaEntradaVeiculo = PropertyEntity({ eventClick: RejeitarEtapaEntradaVeiculoClick, type: types.event, text: Localization.Resources.GestaoPatio.FluxoPatio.RejeitarEtapa, visible: ko.observable(false) });
    this.RejeitarEtapaSaidaVeiculo = PropertyEntity({ eventClick: RejeitarEtapaSaidaVeiculoClick, type: types.event, text: Localization.Resources.GestaoPatio.FluxoPatio.RejeitarEtapa, visible: ko.observable(false) });

    this.ReabrirFluxo = PropertyEntity({ eventClick: reabrirFluxoGuaritaClick, type: types.event, text: Localization.Resources.GestaoPatio.FluxoPatio.ReabrirFluxo, visible: ko.observable(false) });
    this.AbrirAtendimento = PropertyEntity({ eventClick: AbrirAtendimentoGuaritaClick, type: types.event, text: Localization.Resources.GestaoPatio.FluxoPatio.AbrirAtendimento, visible: ko.observable(false) });
    this.Anexos = PropertyEntity({ type: types.event, text: Localization.Resources.Gerais.Geral.Anexos, eventClick: abrirAnexosGuaritaClick, visible: ko.observable(true) });

    this.ObservacoesEtapa = PropertyEntity({ eventClick: observacoesEtapaGuaritaClick, type: types.event, text: Localization.Resources.GestaoPatio.FluxoPatio.Observacoes, visible: _configuracaoGestaoPatio.HabilitarObservacaoEtapa });
    this.ImprimirComprovanteCargaInformada = PropertyEntity({ eventClick: function () { imprimirComprovanteCargaInformada(_guaritaFluxoPatio.Carga.val()); }, type: types.event, text: Localization.Resources.GestaoPatio.FluxoPatio.ImprimirComprovanteDeCarga, visible: ko.observable(isPermitirImprimirComprovanteCargaInformada()) });
    this.ImprimirComprovante = PropertyEntity({ eventClick: imprimirComprovanteGuaritaClick, type: types.event, text: Localization.Resources.GestaoPatio.FluxoPatio.ImprimirComprovante, idGrid: guid(), visible: ko.observable(!isPermitirImprimirComprovanteCargaInformada()) });

    this.ImprimirTicket = PropertyEntity({ eventClick: imprimirTicketClick, type: types.event, text: Localization.Resources.GestaoPatio.FluxoPatio.ImprimirTicket, idGrid: guid(), visible: ko.observable(false) });

    this.ImprimirViaCega = PropertyEntity({ eventClick: downloadViaCegaClick, type: types.event, text: Localization.Resources.GestaoPatio.FluxoPatio.ImprimirViaCega, visible: ko.observable(false) });


    this.ListaNfeChegadaVeiculo.val.subscribe(function () {
        recarregarGridGuaritaFluxoPatioNfe();
    });

    this.NumeroDocumento = PropertyEntity({ text: Localization.Resources.GestaoPatio.FluxoPatio.NumeroDoDocumento.getFieldDescription(), val: ko.observable(""), def: "", visible: ko.observable(false) });
};

// #endregion Classes

// #region Funções de Inicialização

function imprimirComprovanteGuaritaClick(e) {
    executarDownload("Guarita/ComprovanteSaida", { Codigo: e.Codigo.val() });
}
function imprimirTicketClick(e) {
    executarDownload("Guarita/DownloadTicket", { Codigo: e.Codigo.val() });
}


function LoadGuaritaFluxoPatio() {
    _guaritaFluxoPatio = new GuaritaFluxoPatio();
    KoBindings(_guaritaFluxoPatio, "knockoutGuaritaFluxoPatio");

    _guaritaFluxoPatio.DataPrevisao.val.subscribe(carregarDataHoraCarga);

    if (_configuracaoGestaoPatio.OcultarTransportador)
        _guaritaFluxoPatio.Transportador.visible(false);

    $("#" + _guaritaFluxoPatio.VeiculoChegada.id).mask("AAAAAAA", { selectOnFocus: true, clearIfNotMatch: true });
    $("#" + _guaritaFluxoPatio.ReboqueChegada.id).mask("AAAAAAA", { selectOnFocus: true, clearIfNotMatch: true });

    $("#" + _guaritaFluxoPatio.CpfMotoristaChegada.idBtnSearch).on('click', buscarMotoristaPorCpf);
    $('#' + _guaritaFluxoPatio.CpfMotoristaChegada.id).on('keypress', function (e) {
        var keyCode = e.keyCode || e.which;

        if (keyCode == 13)
            buscarMotoristaPorCpf();
    });
    $('#' + _guaritaFluxoPatio.ChaveNfeChegadaVeiculo.id).on('keypress', function (e) {
        var keyCode = e.keyCode || e.which;

        if (keyCode == 13)
            adicionarNfeChegadaVeiculoClick();
    });

    loadChamado(false);
    loadGridGuaritaFluxoPatioNfe();
    loadGuaritaAnexo();
}

function loadGridGuaritaFluxoPatioNfe() {
    var linhasPorPaginas = 5;
    var ordenacao = { column: 0, dir: orderDir.asc };
    var menuOpcoes = undefined;
    var header = [
        { data: "Codigo", visible: false },
        { data: "Chave", title: Localization.Resources.GestaoPatio.FluxoPatio.ChaveNFe, width: "80%" }
    ];

    if (_guaritaFluxoPatio.InformarChegadaVeiculo.visible()) {
        var opcaoExcluir = { descricao: Localization.Resources.Gerais.Geral.Excluir, id: guid(), metodo: excluirGuaritaFluxoPatioNfeClick, icone: "" };

        menuOpcoes = { tipo: TypeOptionMenu.link, tamanho: 20, opcoes: [opcaoExcluir] };
    }

    _gridGuaritaFluxoPatioNfe = new BasicDataTable(_guaritaFluxoPatio.ListaNfeChegadaVeiculo.idGrid, header, menuOpcoes, ordenacao, null, linhasPorPaginas);
    _gridGuaritaFluxoPatioNfe.CarregarGrid([]);
}

// #endregion Funções de Inicialização

// #region Funções Associadas a Eventos

function adicionarNfeChegadaVeiculoClick() {
    if (_guaritaFluxoPatio.AdicionarNfeChegadaVeiculo.visible()) {
        if (_guaritaFluxoPatio.ChaveNfeChegadaVeiculo.val()) {
            if (!ValidarChaveAcesso(_guaritaFluxoPatio.ChaveNfeChegadaVeiculo.val()))
                exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.DadosInvalidos, Localization.Resources.GestaoPatio.FluxoPatio.ChaveNFeInformadaInvalida);
            else if (isNfeChegadaVeiculoDuplicada())
                exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.RegistroDuplicado, Localization.Resources.GestaoPatio.FluxoPatio.ChaveNFeInformadaJaEstaCadastrada);
            else {
                _guaritaFluxoPatio.ListaNfeChegadaVeiculo.val().push(obterNfeChegadaVeiculoSalvar());

                recarregarGridGuaritaFluxoPatioNfe();
            }
        }

        limparCamposNfeChegadaVeiculoCadastro();
    }
}

function auditarGuaritaClick() {
    var _fn = OpcaoAuditoria("CargaJanelaCarregamentoGuarita", "Codigo", _guaritaFluxoPatio);

    _fn({ Codigo: _guaritaFluxoPatio.Codigo.val() });
}

function excluirGuaritaFluxoPatioNfeClick(registroSelecionado) {
    var listaNfeChegadaVeiculo = obterListaGuaritaFluxoPatioNfe();

    for (var i = 0; i < listaNfeChegadaVeiculo.length; i++) {
        if (registroSelecionado.Codigo == listaNfeChegadaVeiculo[i].Codigo) {
            listaNfeChegadaVeiculo.splice(i, 1);
            break;
        }
    }

    _guaritaFluxoPatio.ListaNfeChegadaVeiculo.val(listaNfeChegadaVeiculo);
}

function informarChegadaVeiculoClick(e) {
    if (_ConfiguracaoInformarDadosChegadaVeiculoNoFluxoPatio && !ValidarCamposObrigatorios(_guaritaFluxoPatio)) {
        exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.Atencao, Localization.Resources.Gerais.Geral.InformeOsDadosObrigatorios);
        return;
    }

    if (_fluxoAtual.GuaritaEntradaPermiteInformacoesPesagem.val() && _fluxoAtual.GuaritaEntradaPermiteInformacoesProdutor.val() && !ValidarCampoObrigatorioMap(_guaritaFluxoPatio.NumeroNfProdutor)) {
        exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Atencao, Localization.Resources.GestaoPatio.FluxoPatio.InformeNumeroDaNotaFiscalDoProdutor);
        return;
    }
    
    var dados = {
        Codigo: _guaritaFluxoPatio.Codigo.val(),
        CodigoMotoristaChegada: _guaritaFluxoPatio.CodigoMotoristaChegada.val(),
        CpfMotoristaChegada: _guaritaFluxoPatio.CpfMotoristaChegada.val(),
        MotoristaChegada: _guaritaFluxoPatio.MotoristaChegada.val(),
        ReboqueChegada: _guaritaFluxoPatio.ReboqueChegada.val(),
        RgMotoristaChegada: _guaritaFluxoPatio.RgMotoristaChegada.val(),
        TelefoneChegada: _guaritaFluxoPatio.TelefoneChegada.val(),
        VeiculoChegada: _guaritaFluxoPatio.VeiculoChegada.val(),
        ListaNfeChegadaVeiculo: obterListaGuaritaFluxoPatioNfeSalvar(),
        NumeroNfProdutor: _guaritaFluxoPatio.NumeroNfProdutor.val(),
        NumeroLacre: _guaritaFluxoPatio.NumeroLacreChegada.val(),
        DocaChegada: _guaritaFluxoPatio.DocaChegada.val(),
        SenhaChegada: _guaritaFluxoPatio.SenhaChegada.val(),
    };

    if (_configuracaoGestaoPatio.ChegadaVeiculoPermiteAntecipar && _guaritaFluxoPatio.EtapaAntecipada.val())
        exibirConfirmacao(Localization.Resources.GestaoPatio.FluxoPatio.AnteciparEtapa, Localization.Resources.GestaoPatio.FluxoPatio.AoConfirmarEtapaSeraAntecipadaPermanecendoSequenciaUltimaetapaConfirmada.format(Localization.Resources.GestaoPatio.FluxoPatio.ChegadaVeiculo), () => executarInformarChegadaVeiculo(dados));
    else
        executarInformarChegadaVeiculo(dados);
}

function executarInformarChegadaVeiculo(dados) {
    executarReST("Guarita/InformarChegadaVeiculo", dados, function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, Localization.Resources.Gerais.Geral.DadosSalvosComSucesso);
                atualizarFluxoPatio();
                Global.fecharModal('divModalGuaritaFluxoPatio');
            }
            else
                exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.Atencao, retorno.Msg);
        }
        else
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, retorno.Msg);
    });
}

function informarEntradaVeiculoClick(e) {
    var dados = {
        Codigo: e.Codigo.val(),
        PossuiDevolucao: e.PossuiDevolucao.val(),
        ObservacaoDevolucao: e.ObservacaoDevolucao.val(),
        EtapaAntecipada: _guaritaFluxoPatio.EtapaAntecipada.val()
    };

    if (_configuracaoGestaoPatio.ChegadaVeiculoPermiteAntecipar && _guaritaFluxoPatio.EtapaAntecipada.val())
        exibirConfirmacao(Localization.Resources.GestaoPatio.FluxoPatio.AnteciparEtapa, Localization.Resources.GestaoPatio.FluxoPatio.AoConfirmarEtapaSeraAntecipadaPermanecendoSequenciaUltimaetapaConfirmada.format(Localization.Resources.GestaoPatio.FluxoPatio.ChegadaPortaria), () => executarLiberarCarga(dados));
    else
        executarLiberarCarga(dados);
}

function executarLiberarCarga(dados) {
    executarReST("Guarita/LiberarCarga", dados, function (r) {
        if (r.Success) {
            if (r.Data) {
                exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, Localization.Resources.GestaoPatio.FluxoPatio.LiberadoComSucesso);
                atualizarFluxoPatio();
                Global.fecharModal('divModalGuaritaFluxoPatio');
            } else {
                exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.Atencao, r.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, r.Msg);
        }
    });
}

function informarSaidaVeiculoClick(e) {
    executarReST("Guarita/SaidaVeiculo", { Codigo: e.Codigo.val() }, function (r) {
        if (r.Success) {
            if (r.Data) {
                exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, Localization.Resources.GestaoPatio.FluxoPatio.VeiculoLiberadoComSucesso);
                atualizarFluxoPatio();
                Global.fecharModal('divModalGuaritaFluxoPatio');

            } else {
                exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.Atencao, r.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, r.Msg);
        }
    });
}

function observacoesEtapaGuaritaClick() {
    buscarObservacoesEtapa(_guaritaFluxoPatio.Etapa.val());
}

function reabrirFluxoGuaritaClick(e) {
    exibirConfirmacao(Localization.Resources.GestaoPatio.FluxoPatio.ReabrirFluxo, Localization.Resources.GestaoPatio.FluxoPatio.VoceTemCertezaQueDesejaReabrirFluxo, function () {
        executarReST("Guarita/ReabrirFluxo", { Codigo: e.Codigo.val() }, function (arg) {
            if (arg.Success) {
                if (arg.Data !== false) {
                    exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, Localization.Resources.GestaoPatio.FluxoPatio.FluxoReaberto);
                    atualizarFluxoPatio();
                    Global.fecharModal('divModalGuaritaFluxoPatio');
                } else {
                    exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.Atencao, arg.Msg);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, arg.Msg);
            }
        });
    });
}

function RejeitarEtapaChegadaVeiculoClick(e) {
    exibirConfirmacao(Localization.Resources.GestaoPatio.FluxoPatio.RejeitarEtapa, Localization.Resources.GestaoPatio.FluxoPatio.VoceTemCertezaQueDesejaRejeitarNessaEtapa, function () {
        executarReST("Guarita/RejeitarEtapaChegadaVeiculo", { Codigo: e.Codigo.val() }, function (arg) {
            if (arg.Success) {
                if (arg.Data !== false) {
                    exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, Localization.Resources.GestaoPatio.FluxoPatio.EtapaRejeitada);
                    atualizarFluxoPatio();
                    Global.fecharModal('divModalGuaritaFluxoPatio');
                } else {
                    exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.Atencao, arg.Msg);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, arg.Msg);
            }
        });
    });
}

function RejeitarEtapaEntradaVeiculoClick(e) {
    exibirConfirmacao(Localization.Resources.GestaoPatio.FluxoPatio.RejeitarEtapa, Localization.Resources.GestaoPatio.FluxoPatio.VoceTemCertezaQueDesejaRejeitarNessaEtapa, function () {
        executarReST("Guarita/RejeitarEtapaEntradaVeiculo", { Codigo: e.Codigo.val() }, function (arg) {
            if (arg.Success) {
                if (arg.Data !== false) {
                    exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, Localization.Resources.GestaoPatio.FluxoPatio.EtapaRejeitada);
                    atualizarFluxoPatio();
                    Global.fecharModal('divModalGuaritaFluxoPatio');
                } else {
                    exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.Atencao, arg.Msg);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, arg.Msg);
            }
        });
    });
}

function RejeitarEtapaSaidaVeiculoClick(e) {
    exibirConfirmacao(Localization.Resources.GestaoPatio.FluxoPatio.RejeitarEtapa, Localization.Resources.GestaoPatio.FluxoPatio.VoceTemCertezaQueDesejaRejeitarNessaEtapa, function () {
        executarReST("Guarita/RejeitarEtapaSaidaVeiculo", { Codigo: e.Codigo.val() }, function (arg) {
            if (arg.Success) {
                if (arg.Data !== false) {
                    exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, Localization.Resources.GestaoPatio.FluxoPatio.EtapaRejeitada);
                    atualizarFluxoPatio();
                    Global.fecharModal('divModalGuaritaFluxoPatio');
                } else {
                    exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.Atencao, arg.Msg);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, arg.Msg);
            }
        });
    });
}

function pesagemClick(e) {
    abrirPesagemFluxoPatioClick(e.Codigo.val());
}

function pesagemFinalClick(e) {
    abrirPesagemFinalFluxoPatioClick(e.Codigo.val());
}

function VoltarEtapaChegadaVeiculoClick() {
    console.log("VoltarEtapaChegadaVeiculoClick");
}

function VoltarEtapaEntradaVeiculoClick(e) {
    exibirConfirmacao(Localization.Resources.GestaoPatio.FluxoPatio.VoltarEtapa, Localization.Resources.GestaoPatio.FluxoPatio.VoceTemCertezaQueDesejaRetornarEtapaAnterior, function () {
        executarReST("Guarita/VoltarEtapaEntradaVeiculo", { Codigo: e.Codigo.val() }, function (arg) {
            if (arg.Success) {
                if (arg.Data !== false) {
                    exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, Localization.Resources.GestaoPatio.FluxoPatio.EtapaRetornada);
                    atualizarFluxoPatio();
                    Global.fecharModal('divModalGuaritaFluxoPatio');
                } else {
                    exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.Atencao, arg.Msg);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, arg.Msg);
            }
        });
    });
}

function VoltarEtapaSaidaVeiculoClick(e) {
    exibirConfirmacao(Localization.Resources.GestaoPatio.FluxoPatio.VoltarEtapa, Localization.Resources.GestaoPatio.FluxoPatio.VoceTemCertezaQueDesejaRetornarEtapaAnterior, function () {
        executarReST("Guarita/VoltarEtapaSaidaVeiculo", { Codigo: e.Codigo.val() }, function (arg) {
            if (arg.Success) {
                if (arg.Data !== false) {
                    exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, Localization.Resources.GestaoPatio.FluxoPatio.EtapaRetornada);
                    atualizarFluxoPatio();
                    Global.fecharModal('divModalGuaritaFluxoPatio');
                } else {
                    exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.Atencao, arg.Msg);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, arg.Msg);
            }
        });
    });
}

// #endregion Funções Associadas a Eventos

// #region Funções Públicas

function ExibirDetalhesChegadaVeiculoFluxoPatio(knoutFluxo, opt) {
    atualizarTituloGuarita(opt.text);
    _guaritaFluxoPatio.EtapaAntecipada.val(!opt.etapaLiberada);

    exibirDetalhesGuaritaFluxoPatio(knoutFluxo, EnumEtapaFluxoGestaoPatio.ChegadaVeiculo, function (dadosRetornados) {
        if (!knoutFluxo.GuaritaEntradaPermiteInformacoesPesagem.val() || _ConfiguracaoInformarDadosChegadaVeiculoNoFluxoPatio)
            _guaritaFluxoPatio.ExibirDadosChegadaVeiculo.val(true);
        else
            _guaritaFluxoPatio.ExibirDadosChegadaVeiculo.val(false);

        _guaritaFluxoPatio.InformarEntradaVeiculo.visible(false);
        _guaritaFluxoPatio.InformarSaidaVeiculo.visible(false);
        _guaritaFluxoPatio.Pesagem.visible(false);
        _guaritaFluxoPatio.PesagemFinal.visible(false);

        if (knoutFluxo.GuaritaEntradaPermiteInformacoesPesagem.val() && knoutFluxo.GuaritaEntradaPermiteInformacoesProdutor.val()) {
            _guaritaFluxoPatio.NumeroNfProdutor.visible(true);
            _guaritaFluxoPatio.NumeroNfProdutor.required(true);
        }
        else {
            _guaritaFluxoPatio.NumeroNfProdutor.visible(false);
            _guaritaFluxoPatio.NumeroNfProdutor.required(false);
        }

        controlarCamposDadosChegadaGuaritaHabilitados(_guaritaFluxoPatio.InformarChegadaVeiculo.visible());

        if (!_configuracaoGestaoPatio.ChegadaVeiculoPermiteVoltar)
            _guaritaFluxoPatio.VoltarEtapaChegadaVeiculo.visible(false);

        if (!_configuracaoGestaoPatio.PermitirRejeicaoFluxo)
            _guaritaFluxoPatio.RejeitarEtapaChegadaVeiculo.visible(false);

        _guaritaFluxoPatio.VoltarEtapaEntradaVeiculo.visible(false);
        _guaritaFluxoPatio.VoltarEtapaSaidaVeiculo.visible(false);

        _guaritaFluxoPatio.RejeitarEtapaEntradaVeiculo.visible(false);
        _guaritaFluxoPatio.RejeitarEtapaSaidaVeiculo.visible(false);

        if (knoutFluxo.EtapaFluxoGestaoPatioAtual.val() != EnumEtapaFluxoGestaoPatio.ChegadaVeiculo)
            _guaritaFluxoPatio.ReabrirFluxo.visible(false);

        if (knoutFluxo.Tipo.val() == EnumTipoFluxoGestaoPatio.Destino) {
            carregarNumeroDocumento(knoutFluxo);
            _guaritaFluxoPatio.NumeroDocumento.visible(true);
        } else
            _guaritaFluxoPatio.NumeroDocumento.visible(false);

    });
}

function ExibirDetalhesEntradaGuaritaFluxoPatio(knoutFluxo, opt) {
    atualizarTituloGuarita(opt.text);
    _guaritaFluxoPatio.EtapaAntecipada.val(!opt.etapaLiberada);

    exibirDetalhesGuaritaFluxoPatio(knoutFluxo, EnumEtapaFluxoGestaoPatio.Guarita, function (dadosRetornados) {
        _guaritaFluxoPatio.InformarChegadaVeiculo.visible(false);
        _guaritaFluxoPatio.InformarSaidaVeiculo.visible(false);

        if (!_configuracaoGestaoPatio.GuaritaEntradaPermiteVoltar)
            _guaritaFluxoPatio.VoltarEtapaEntradaVeiculo.visible(false);

        if (!_configuracaoGestaoPatio.PermitirRejeicaoFluxo)
            _guaritaFluxoPatio.RejeitarEtapaEntradaVeiculo.visible(false);

        _guaritaFluxoPatio.VoltarEtapaChegadaVeiculo.visible(false);
        _guaritaFluxoPatio.VoltarEtapaSaidaVeiculo.visible(false);

        _guaritaFluxoPatio.RejeitarEtapaChegadaVeiculo.visible(false);
        _guaritaFluxoPatio.RejeitarEtapaSaidaVeiculo.visible(false);

        if (knoutFluxo.EtapaFluxoGestaoPatioAtual.val() != EnumEtapaFluxoGestaoPatio.Guarita)
            _guaritaFluxoPatio.ReabrirFluxo.visible(false);

        _guaritaFluxoPatio.NumeroDocumento.visible(false);
        _guaritaFluxoPatio.Pesagem.visible(knoutFluxo.GuaritaEntradaPermiteInformacoesPesagem.val());
        _guaritaFluxoPatio.PesagemFinal.visible(false);
        _guaritaFluxoPatio.PossuiDevolucao.visible(dadosRetornados.PermiteInformarDadosDevolucao);
    });
}

function ExibirDetalhesPesagemFinalFluxoPatio(knoutFluxo, opt) {
    executarReST("Guarita/BuscarCodigoGuarita", { Codigo: knoutFluxo.Codigo.val() }, function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                abrirPesagemFinalFluxoPatioClick(retorno.Data);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, retorno.Msg);
        }
    });
}

function ExibirDetalhesSaidaGuaritaFluxoPatio(knoutFluxo, opt) {
    atualizarTituloGuarita(opt.text);

    exibirDetalhesGuaritaFluxoPatio(knoutFluxo, EnumEtapaFluxoGestaoPatio.InicioViagem, function (dadosRetornados) {
        _guaritaFluxoPatio.InformarChegadaVeiculo.visible(false);
        _guaritaFluxoPatio.InformarEntradaVeiculo.visible(false);

        if (!_configuracaoGestaoPatio.GuaritaSaidaPermiteVoltar)
            _guaritaFluxoPatio.VoltarEtapaSaidaVeiculo.visible(false);

        if (!_configuracaoGestaoPatio.PermitirRejeicaoFluxo)
            _guaritaFluxoPatio.RejeitarEtapaSaidaVeiculo.visible(false);

        _guaritaFluxoPatio.Pesagem.visible(false);

        if (knoutFluxo.GuaritaSaidaPermiteInformacoesPesagem.val())
            _guaritaFluxoPatio.PesagemFinal.visible(true);
        else
            _guaritaFluxoPatio.PesagemFinal.visible(false);

        _guaritaFluxoPatio.VoltarEtapaChegadaVeiculo.visible(false);
        _guaritaFluxoPatio.VoltarEtapaEntradaVeiculo.visible(false);

        _guaritaFluxoPatio.RejeitarEtapaChegadaVeiculo.visible(false);
        _guaritaFluxoPatio.RejeitarEtapaEntradaVeiculo.visible(false);

        if (knoutFluxo.EtapaFluxoGestaoPatioAtual.val() != EnumEtapaFluxoGestaoPatio.InicioViagem)
            _guaritaFluxoPatio.ReabrirFluxo.visible(false);

        _guaritaFluxoPatio.NumeroDocumento.visible(false);
    });
}

// #endregion Funções Públicas

// #region Funções Privadas

function atualizarTituloGuarita(titulo) {
    $("#guarita-titulo").text(titulo);
}

function buscarMotoristaPorCpf() {
    if (!validarCpfMotorista()) {
        _guaritaFluxoPatio.CodigoMotoristaChegada.val(0);
        $("#" + _guaritaFluxoPatio.MotoristaChegada.id).focus();
        return;
    }

    executarReST("Motorista/BuscarPorCPF", { Cpf: _guaritaFluxoPatio.CpfMotoristaChegada.val(), BuscarPorEmpresaPai: true }, function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                _guaritaFluxoPatio.CodigoMotoristaChegada.val(retorno.Data.Codigo);
                _guaritaFluxoPatio.MotoristaChegada.val(retorno.Data.Nome);
                _guaritaFluxoPatio.RgMotoristaChegada.val(retorno.Data.RG);
                _guaritaFluxoPatio.TelefoneChegada.val(retorno.Data.Telefone);

                $("#" + _guaritaFluxoPatio.MotoristaChegada.id).focus();
            }
            else {
                _guaritaFluxoPatio.CodigoMotoristaChegada.val(0);
                $("#" + _guaritaFluxoPatio.MotoristaChegada.id).focus();
            }
        }
        else {
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, retorno.Msg);
            $("#" + _guaritaFluxoPatio.CpfMotoristaChegada.id).focus();
        }
    });
}

function carregarDataHoraCarga() {
    var dataHora = _guaritaFluxoPatio.DataPrevisao.val();

    if (dataHora) {
        var splittedTime = dataHora.split(" ");

        _guaritaFluxoPatio.CargaData.val(splittedTime[0] || "");
        _guaritaFluxoPatio.CargaHora.val(splittedTime[1] || "");
    }
}

function controlarCamposDadosChegadaGuaritaHabilitados(habilitar) {
    _guaritaFluxoPatio.MotoristaChegada.enable(habilitar);
    _guaritaFluxoPatio.CpfMotoristaChegada.enable(habilitar);
    _guaritaFluxoPatio.RgMotoristaChegada.enable(habilitar);
    _guaritaFluxoPatio.ReboqueChegada.enable(habilitar);
    _guaritaFluxoPatio.TelefoneChegada.enable(habilitar);
    _guaritaFluxoPatio.VeiculoChegada.enable(habilitar);
    _guaritaFluxoPatio.AdicionarNfeChegadaVeiculo.visible(habilitar);
}

function exibirDetalhesGuaritaFluxoPatio(knoutFluxo, etapa, callback) {
    _fluxoAtual = knoutFluxo;

    limparCamposGuaritaFluxoPatio();

    executarReST("Guarita/BuscarPorCodigo", { FluxoGestaoPatio: knoutFluxo.Codigo.val() }, function (retorno) {
        if (retorno.Success) {
            if (retorno.Data !== false) {
                PreencherObjetoKnout(_guaritaFluxoPatio, retorno);

                _guaritaFluxoPatio.Etapa.val(etapa);
                _guaritaFluxoPatio.InformarChegadaVeiculo.visible(false);
                _guaritaFluxoPatio.InformarEntradaVeiculo.visible(false);
                _guaritaFluxoPatio.InformarSaidaVeiculo.visible(false);

                _guaritaFluxoPatio.ExibirDadosChegadaVeiculo.val(false);
                _guaritaFluxoPatio.AdicionarNfeChegadaVeiculo.visible(false);

                _guaritaFluxoPatio.VoltarEtapaChegadaVeiculo.visible(false);
                _guaritaFluxoPatio.VoltarEtapaEntradaVeiculo.visible(false);
                _guaritaFluxoPatio.VoltarEtapaSaidaVeiculo.visible(false);

                _guaritaFluxoPatio.RejeitarEtapaChegadaVeiculo.visible(false);
                _guaritaFluxoPatio.RejeitarEtapaEntradaVeiculo.visible(false);
                _guaritaFluxoPatio.RejeitarEtapaSaidaVeiculo.visible(false);

                _guaritaFluxoPatio.NumeroNfProdutor.visible(false);
                _guaritaFluxoPatio.PossuiDevolucao.visible(false);

                _guaritaFluxoPatio.ReabrirFluxo.visible(false);
                _guaritaFluxoPatio.AbrirAtendimento.visible(retorno.Data.PermiteGerarAtendimento);

                _guaritaFluxoPatio.ImprimirTicket.visible(retorno.Data.ExibirImprimirTicketBalanca);

                controlarCamposDadosChegadaGuaritaHabilitados(false);

                var primeiraEtapa = _fluxoAtual.EtapaAtual.val() == 0;
                var fluxoAberto = (_fluxoAtual.SituacaoEtapaFluxoGestaoPatio.val() == EnumSituacaoEtapaFluxoGestaoPatio.Aguardando);
                var fluxoRejeitado = (_fluxoAtual.SituacaoEtapaFluxoGestaoPatio.val() == EnumSituacaoEtapaFluxoGestaoPatio.Rejeitado || _fluxoAtual.SituacaoEtapaFluxoGestaoPatio.val() == EnumSituacaoEtapaFluxoGestaoPatio.Cancelado);

                _guaritaFluxoPatio.DataPrevisao.val(_guaritaFluxoPatio.DataCarregamento.val());

                if (_guaritaFluxoPatio.Situacao.val() == EnumSituacaoCargaGuarita.AgChegadaVeiculo) {
                    if (fluxoAberto) {
                        _guaritaFluxoPatio.InformarChegadaVeiculo.visible(true);

                        if (_configuracaoGestaoPatio.ChegadaVeiculoPermiteVoltar) {
                            _guaritaFluxoPatio.VoltarEtapaChegadaVeiculo.visible(!primeiraEtapa);
                            _guaritaFluxoPatio.RejeitarEtapaChegadaVeiculo.visible(_configuracaoGestaoPatio.PermitirRejeicaoFluxo);
                        }
                    }

                    if (_configuracaoGestaoPatio.ChegadaVeiculoPermiteVoltar && fluxoRejeitado && _fluxoAtual.SituacaoEtapaFluxoGestaoPatio.val() != EnumSituacaoEtapaFluxoGestaoPatio.Cancelado)
                        _guaritaFluxoPatio.ReabrirFluxo.visible(true);

                }
                else if (_guaritaFluxoPatio.Situacao.val() == EnumSituacaoCargaGuarita.AguardandoLiberacao) {
                    if (fluxoAberto) {
                        _guaritaFluxoPatio.InformarEntradaVeiculo.visible(true);

                        if (_configuracaoGestaoPatio.GuaritaEntradaPermiteVoltar) {
                            _guaritaFluxoPatio.VoltarEtapaEntradaVeiculo.visible(!primeiraEtapa);
                            _guaritaFluxoPatio.RejeitarEtapaEntradaVeiculo.visible(_configuracaoGestaoPatio.PermitirRejeicaoFluxo);
                        }

                        if (_configuracaoGestaoPatio.IniciarViagemSemGuarita)
                            _guaritaFluxoPatio.InformarSaidaVeiculo.visible(true);
                    }

                    if (fluxoRejeitado && _configuracaoGestaoPatio.GuaritaEntradaPermiteVoltar && _fluxoAtual.SituacaoEtapaFluxoGestaoPatio.val() != EnumSituacaoEtapaFluxoGestaoPatio.Cancelado)
                        _guaritaFluxoPatio.ReabrirFluxo.visible(true);

                }
                else if (_guaritaFluxoPatio.Situacao.val() == EnumSituacaoCargaGuarita.Liberada) {
                    if (fluxoAberto) {
                        _guaritaFluxoPatio.InformarSaidaVeiculo.visible(true);

                        if (_configuracaoGestaoPatio.GuaritaSaidaPermiteVoltar) {
                            _guaritaFluxoPatio.VoltarEtapaSaidaVeiculo.visible(!primeiraEtapa);
                            _guaritaFluxoPatio.RejeitarEtapaSaidaVeiculo.visible(_configuracaoGestaoPatio.PermitirRejeicaoFluxo);
                        }
                    }

                    if (_configuracaoGestaoPatio.GuaritaSaidaPermiteVoltar && fluxoRejeitado && VerificaSePossuiPermissaoEspecial(EnumPermissaoPersonalizada.FluxoGestaoPatio_PermiteRetornarEtapaSaidaCD, _PermissoesPersonalizadasFluxoPatio) && _fluxoAtual.SituacaoEtapaFluxoGestaoPatio.val() != EnumSituacaoEtapaFluxoGestaoPatio.Cancelado) {
                        _guaritaFluxoPatio.ReabrirFluxo.visible(true);
                    }
                }

                // Finalizado e permite retornar Etapa de saída 
                if (_fluxoAtual.SituacaoEtapaFluxoGestaoPatio.val() == EnumSituacaoEtapaFluxoGestaoPatio.Aprovado && _fluxoAtual.SituacaoEtapaFluxoGestaoPatio.val() != EnumSituacaoEtapaFluxoGestaoPatio.Cancelado && VerificaSePossuiPermissaoEspecial(EnumPermissaoPersonalizada.FluxoGestaoPatio_PermiteRetornarEtapaSaidaCD, _PermissoesPersonalizadasFluxoPatio)) {
                    _guaritaFluxoPatio.ReabrirFluxo.visible(true);
                }

                //if (!VerificaSePossuiPermissaoEspecial(EnumPermissaoPersonalizada.FluxoGestaoPatio_InformarGuaritaEntrada, _PermissoesPersonalizadasFluxoPatio)) {
                //    _guaritaFluxoPatio.InformarChegadaVeiculo.visible(false);
                //    _guaritaFluxoPatio.InformarEntradaVeiculo.visible(false);
                //    _guaritaFluxoPatio.InformarSaidaVeiculo.visible(false);
                //}

                if (!VerificaSePossuiPermissaoEspecial(EnumPermissaoPersonalizada.FluxoGestaoPatio_InformarChegadaVeiculo, _PermissoesPersonalizadasFluxoPatio))
                    _guaritaFluxoPatio.InformarChegadaVeiculo.visible(false);

                if (!VerificaSePossuiPermissaoEspecial(EnumPermissaoPersonalizada.FluxoGestaoPatio_InformarGuaritaEntrada, _PermissoesPersonalizadasFluxoPatio))
                    _guaritaFluxoPatio.InformarEntradaVeiculo.visible(false);

                if (!VerificaSePossuiPermissaoEspecial(EnumPermissaoPersonalizada.FluxoGestaoPatio_InformarInicioViagem, _PermissoesPersonalizadasFluxoPatio))
                    _guaritaFluxoPatio.InformarSaidaVeiculo.visible(false);

                if (retorno.Data.Atendimento > 0) {
                    _guaritaFluxoPatio.Atendimento.visible(true);
                }

                loadGridGuaritaFluxoPatioNfe();

                _guaritaFluxoPatio.ListaNfeChegadaVeiculo.val(retorno.Data.ListaNfeChegadaVeiculo);

                callback(retorno.Data);

                if (edicaoEtapaFluxoPatioBloqueada())
                    ocultarBotoesEtapa(_guaritaFluxoPatio);

                Global.abrirModal('divModalGuaritaFluxoPatio');

                $(window).one('keyup', function (e) {
                    if (e.keyCode == 27)
                        Global.fecharModal('divModalGuaritaFluxoPatio');
                });
            }
            else
                exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.Atencao, retorno.Msg);
        }
        else
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, retorno.Msg);
    });
}

function isNfeChegadaVeiculoDuplicada() {
    var listaNfeChegadaVeiculo = obterListaGuaritaFluxoPatioNfe();

    for (var i = 0; i < listaNfeChegadaVeiculo.length; i++) {
        if (_guaritaFluxoPatio.ChaveNfeChegadaVeiculo.val() == listaNfeChegadaVeiculo[i].Chave)
            return true;
    }

    return false;
}

function limparCamposGuaritaFluxoPatio() {
    LimparCampos(_guaritaFluxoPatio);

    _guaritaFluxoPatio.ListaNfeChegadaVeiculo.val([]);

    $('a[href="#tabGuaritaFluxoPatioDados"]').tab("show");
}

function limparCamposNfeChegadaVeiculoCadastro() {
    _guaritaFluxoPatio.ChaveNfeChegadaVeiculo.val("");

    $("#" + _guaritaFluxoPatio.ChaveNfeChegadaVeiculo.id).focus();
}

function obterListaGuaritaFluxoPatioNfe() {
    return _guaritaFluxoPatio.ListaNfeChegadaVeiculo.val().slice();
}

function obterListaGuaritaFluxoPatioNfeSalvar() {
    var listaNfeChegadaVeiculo = obterListaGuaritaFluxoPatioNfe();

    return JSON.stringify(listaNfeChegadaVeiculo);
}

function obterNfeChegadaVeiculoSalvar() {
    return {
        Codigo: guid(),
        Chave: _guaritaFluxoPatio.ChaveNfeChegadaVeiculo.val()
    };
}

function recarregarGridGuaritaFluxoPatioNfe() {
    var listaNfeChegadaVeiculo = obterListaGuaritaFluxoPatioNfe();

    _gridGuaritaFluxoPatioNfe.CarregarGrid(listaNfeChegadaVeiculo);
}

function validarCpfMotorista() {
    var cpfMotorista = _guaritaFluxoPatio.CpfMotoristaChegada.val().replace(/[^0-9]/g, "");

    if (!cpfMotorista)
        return false;

    return ValidarCPF(cpfMotorista);
}

// #endregion Funções Privadas

function carregarNumeroDocumento(knoutFluxo) {
    _guaritaFluxoPatio.NumeroDocumento.val("");
    var codigoCarga = knoutFluxo.Carga.val();

    executarReST("Guarita/BuscarPorCarga", { Carga: codigoCarga }, function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                var codigoFluxoGestaoPatio = retorno.Data.CodigoFluxoGestaoPatio;

                executarReST("DocumentoFiscal/BuscarPorCodigo", { FluxoGestaoPatio: codigoFluxoGestaoPatio }, function (retorno) {
                    if (retorno.Success) {
                        if (retorno.Data) {
                            _guaritaFluxoPatio.NumeroDocumento.val(retorno.Data.NumeroDocumento);
                        }
                    }
                });
            }
        }
    });
}

function AbrirAtendimentoGuaritaClick() {
    var dadosCarga = { Carga: _guaritaFluxoPatio.Carga };

    CriarNovoChamado(dadosCarga, "divModalChamadoOcorrencia");
}

function abrirAnexosGuaritaClick() {
    buscarAnexosGuarita();
    Global.abrirModal('divModalAnexosGuarita');
    $("#divModalAnexosGuarita").one('hidden.bs.modal', function () {
        _listaAnexosGuarita.Anexos.val([]);
    });
}