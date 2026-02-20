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
/// <reference path="../Terceiro/ContratoFrete.js" />
/// <reference path="Carga.js" />
/// <reference path="Leilao.js" />
/// <reference path="Operador.js" />
/// <reference path="SignalR.js" />

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

// #region Objetos Globais do Arquivo

var _knoutCargaAlterarData;
var _knoutEdicaoDatasCarregamento;
var _knoutInfoCarregamento;
var _gridHorariosInfoCarregamento;
var _gridDatasCarregamentoCarga;

// #endregion Objetos Globais do Arquivo

// #region Classes

var EdicaoDatasCarregamento = function () {
    this.Carga = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.InicioCarregamento = PropertyEntity({ text: ko.observable(Localization.Resources.Cargas.Carga.DataDoCarregamento.getRequiredFieldDescription()), getType: typesKnockout.dateTime, required: true });
    this.TerminoCarregamento = PropertyEntity({ text: ko.observable(Localization.Resources.Cargas.Carga.DataDoDescarregamento.getRequiredFieldDescription()), getType: typesKnockout.dateTime, required: true });

    this.InicioCarregamento.dateRangeLimit = this.TerminoCarregamento;
    this.TerminoCarregamento.dateRangeInit = this.InicioCarregamento;

    this.Confirmar = PropertyEntity({ type: types.event, eventClick: confirmarEdicaoDatasCarregamentoClick, text: Localization.Resources.Gerais.Geral.Confirmar });
}

var InfoCarregamento = function () {
    this.Carga = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.EscolherHorarioCarregamentoPorLista = PropertyEntity({ val: ko.observable(false), def: false, getType: typesKnockout.bool });
    this.EncaixarHorario = PropertyEntity({ val: ko.observable(false), def: false, text: Localization.Resources.Cargas.Carga.EncaixarHorario, eventClick: encaixarHorarioClick, getType: typesKnockout.bool, visible: ko.observable(false) });
    this.DataCarregamento = PropertyEntity({ text: Localization.Resources.Cargas.Carga.DataCarregamento.getRequiredFieldDescription(), required: true, getType: typesKnockout.dateTime, visible: ko.observable(true) });
    this.DataCarregamentoDisponibilidade = PropertyEntity({ text: Localization.Resources.Cargas.Carga.Data.getFieldDescription(), getType: typesKnockout.date, val: ko.observable(Global.DataAtual()), diminuirData: diminuirDataClick, aumentarData: aumentarDataClick, idGrid: guid() });

    this.DataCarregamentoDisponibilidade.val.subscribe(atualizarHorarios)

    this.ConfirmarAlteracao = PropertyEntity({ type: types.event, eventClick: confirmarAlteracaoCarregamentoClick, text: Localization.Resources.Gerais.Geral.Confirmar, visible: ko.observable(true) });
    this.CancelarHorario = PropertyEntity({ type: types.event, eventClick: cancelarHorarioCarregamentoClick, text: Localization.Resources.Cargas.Carga.CancelarHorario, visible: ko.observable(false) });
}

// #endregion Classes

// #region Funções Associadas a Eventos

function aumentarDataClick() {
    definirDataCarregamentoDisponibilidade(1);
}

function atualizarHorarios() {
    if (!_knoutInfoCarregamento.DataCarregamentoDisponibilidade.val())
        return;

    _gridHorariosInfoCarregamento.CarregarGrid();
}

function cancelarHorarioCarregamentoClick() {
    executarReST("JanelaCarregamento/VerificarPossibilidadeModificacaoJanela", { Carga: _knoutInfoCarregamento.Carga.val(), Data: Global.DataHoraAtual() }, function (retornoVerificacao) {
        exibirConfirmacao(Localization.Resources.Cargas.Carga.CancelarHorario, Localization.Resources.Cargas.Carga.TemCertezaQueDesejaCancelarHorario, function () {
            if (!retornoVerificacao.Success)
                return exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, retornoVerificacao.Msg);

            var _confirmarNaoComparecimento = function (setarComoNaoComparecimento) {
                executarReST("JanelaCarregamento/MandarCargaExcedentesCarregamento", { Carga: _knoutInfoCarregamento.Carga.val(), NaoComparecimento: setarComoNaoComparecimento, HorarioDesencaixado: true }, function (retorno) {
                    if (retorno.Success) {
                        if (retorno.Data) {
                            exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, Localization.Resources.Cargas.Carga.CargaRetornouAsExcedentes);
                            _knoutCargaAlterarData.DataCarregamento.val('');
                            Global.fecharModal("divModalAlterarCarregamento");
                        }
                        else
                            exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Atencao, retorno.Msg);
                    }
                    else
                        exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, retorno.Msg);
                });
            };

            if (retornoVerificacao.Data.PossibilidadeNoShow)
                exibirConfirmacao(Localization.Resources.Cargas.Carga.NoShow, Localization.Resources.Cargas.Carga.DesejaMarcarCargaComoNoShow, function () { _confirmarNaoComparecimento(true) }, function () { _confirmarNaoComparecimento(false) });
            else
                _confirmarNaoComparecimento(false);
        });
    });
}

function confirmarAlteracaoCarregamentoClick() {
    var _executarAlteracaoData = function (setarComoNaoComparecimento) {
        var dados = {
            Carga: _knoutInfoCarregamento.Carga.val(),
            DataCarregamento: _knoutInfoCarregamento.DataCarregamento.val(),
            NaoComparecimento: setarComoNaoComparecimento
        };
        executarReST("Carga/AlterarDataCarregamento", dados, function (arg) {
            if (arg.Success) {
                if (arg.Data != false) {
                    exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, Localization.Resources.Cargas.Carga.DataAlteradaComSucesso);
                    _knoutCargaAlterarData.DataCarregamento.val(_knoutInfoCarregamento.DataCarregamento.val());
                    Global.fecharModal("divModalAlterarCarregamento");
                } else {
                    exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Atencao, arg.Msg, 20000);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, arg.Msg);
            }
        });
    }

    if (!_knoutInfoCarregamento.EscolherHorarioCarregamentoPorLista.val())
        return _executarAlteracaoData();

    executarReST("JanelaCarregamento/VerificarPossibilidadeModificacaoJanela", { Carga: _knoutInfoCarregamento.Carga.val(), Data: _knoutInfoCarregamento.DataCarregamento.val() }, function (retornoVerificacao) {
        if (!retornoVerificacao.Success)
            return exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, retornoVerificacao.Msg);

        if (retornoVerificacao.Data.PossibilidadeNoShow)
            exibirConfirmacao(Localization.Resources.Cargas.Carga.NoShow, Localization.Resources.Cargas.Carga.DesejaMarcarCargaComoNoShow, function () { _executarAlteracaoData(true) }, function () { _executarAlteracaoData(false) });
        else
            _executarAlteracaoData(false);
    });
}

function confirmarEdicaoDatasCarregamentoClick() {
    if (!ValidarCamposObrigatorios(_knoutEdicaoDatasCarregamento)) {
        exibirMensagem(tipoMensagem.atencao, Localization.Resources.Cargas.Carga.CamposObrigatorios, Localizaton.Resources.Cargas.Carga.PreenchaOsCamposObrigatorios);
        return;
    }

    executarReST("Carga/AlterarDatasCarregamento", RetornarObjetoPesquisa(_knoutEdicaoDatasCarregamento), function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                Global.fecharModal("divModalEdicaoDatasCarregamento")
                recarregarGridDatasCarregamento();
            }
            else
                exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Atencao, retorno.Msg);
        }
        else
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, retorno.Msg);
    });
}

function diminuirDataClick() {
    definirDataCarregamentoDisponibilidade(-1);
}

function editarDatasCarregamentoClick(registroSelecionado) {
    PreencherObjetoKnout(_knoutEdicaoDatasCarregamento, { Data: registroSelecionado });

    Global.abrirModal("divModalEdicaoDatasCarregamento");
    $("#divModalEdicaoDatasCarregamento").one('hidden.bs.modal', function () {
        LimparCampos(_knoutEdicaoDatasCarregamento);
    });
}

// #endregion Funções Associadas a Eventos

// #region Funções Públicas

function alterarDataCarregamentoClick(e) {
    _knoutCargaAlterarData = e;

    if (!_knoutInfoCarregamento) {
        _knoutInfoCarregamento = new InfoCarregamento();
        KoBindings(_knoutInfoCarregamento, "knoutAlterarCarregamento");
    }

    _knoutInfoCarregamento.Carga.val(e.Codigo.val());
    _knoutInfoCarregamento.EscolherHorarioCarregamentoPorLista.val(e.EscolherHorarioCarregamentoPorLista.val());

    carregarHorariosinfoCarregamento();

    _knoutInfoCarregamento.CancelarHorario.visible(
        e.EscolherHorarioCarregamentoPorLista.val() &&
        _CONFIGURACAO_TMS.TipoServicoMultisoftware !== EnumTipoServicoMultisoftware.MultiCTe
    );
    _knoutInfoCarregamento.DataCarregamento.visible(!e.EscolherHorarioCarregamentoPorLista.val());
    _knoutInfoCarregamento.EncaixarHorario.visible(_CONFIGURACAO_TMS.PermiteSelecionarHorarioEncaixe);

    Global.abrirModal("divModalAlterarCarregamento");
    $("#divModalAlterarCarregamento")
        .off('hidden.bs.modal')
        .on('hidden.bs.modal', function () {

            LimparCampos(_knoutInfoCarregamento);
    });
}

function encaixarHorarioClick(e) {
    Global.fecharModal("divModalAlterarCarregamento");

    abrirModalEncaixeCarregamento(e);
}

function exibirDatasCarregamentoClick(e) {
    _knoutCargaAlterarData = e;

    carregarDatasCarregamentoCarga();

    recarregarGridDatasCarregamento(function () {
        Global.abrirModal("divModalDatasCarregamento");
    });
}

// #endregion Funções Públicas

// #region Funções Privadas

function carregarDatasCarregamentoCarga() {
    _knoutEdicaoDatasCarregamento = new EdicaoDatasCarregamento();
    KoBindings(_knoutEdicaoDatasCarregamento, "knoutEdicaoDatasCarregamento");

    var linhasPorPaginas = 5;
    var ordenacao = { column: 1, dir: orderDir.asc };
    var opcaoEditar = { descricao: Localization.Resources.Gerais.Geral.Editar, id: guid(), metodo: editarDatasCarregamentoClick, icone: "" };
    var menuOpcoes = { tipo: TypeOptionMenu.link, tamanho: 20, opcoes: [opcaoEditar] };

    var header = [
        { data: "Carga", visible: false },
        { data: "CentroCarregamento", title: Localization.Resources.Cargas.Carga.CentroDeCarregamento, width: "40%", className: "text-align-left", orderable: false },
        { data: "InicioCarregamento", title: Localization.Resources.Cargas.Carga.DataDoCarregamento, width: "25%", className: "text-align-center", orderable: false },
        { data: "TerminoCarregamento", title: Localization.Resources.Cargas.Carga.DataDoDescarregamento, width: "25%", className: "text-align-center", orderable: false }
    ];

    _gridDatasCarregamentoCarga = new BasicDataTable("grid-datas-carregamento-carga", header, menuOpcoes, ordenacao, null, linhasPorPaginas);
}

function carregarHorariosinfoCarregamento() {

    var multiplaescolha = {
        basicGrid: null,
        callbackSelecionado: function (obj, data) {
            horarioSelecionado(data);
        },
        callbackNaoSelecionado: function () { },
        eventos: function () { },
        selecionados: new Array(),
        naoSelecionados: new Array(),
        somenteLeitura: false
    };

    _gridHorariosInfoCarregamento = new GridView(_knoutInfoCarregamento.DataCarregamentoDisponibilidade.idGrid, "JanelaCarregamento/ObterHorariosDisponiveis", _knoutInfoCarregamento, null, null, 10, null, null, null, multiplaescolha, 2000);

    if (string.IsNullOrWhiteSpace(_knoutInfoCarregamento.DataCarregamentoDisponibilidade.val()))
        _knoutInfoCarregamento.DataCarregamentoDisponibilidade.val(Global.DataAtual());
    else {

        if (_knoutInfoCarregamento.EscolherHorarioCarregamentoPorLista.val())
            _gridHorariosInfoCarregamento.CarregarGrid();
    }
}

function definirDataCarregamentoDisponibilidade(dias) {
    if (!_knoutInfoCarregamento.DataCarregamentoDisponibilidade.val())
        return;

    var objData = moment(_knoutInfoCarregamento.DataCarregamentoDisponibilidade.val(), 'DD/MM/YYYY');

    objData.add(dias, 'day');

    _knoutInfoCarregamento.DataCarregamentoDisponibilidade.val(objData.format('DD/MM/YYYY'));

    limparHorarioSelecionado();
}

function horarioSelecionado(data) {
    _gridHorariosInfoCarregamento.AtualizarRegistrosSelecionados([data]);
    _gridHorariosInfoCarregamento.DrawTable(true);

    var dataHoraComSegundos = _knoutInfoCarregamento.DataCarregamentoDisponibilidade.val() + ' ' + data.HoraInicio;
    var dataHoraSemSegundos = dataHoraComSegundos.substring(0, 16);

    _knoutInfoCarregamento.DataCarregamento.val(dataHoraSemSegundos);
}

function limparHorarioSelecionado() {
    _gridHorariosInfoCarregamento.AtualizarRegistrosSelecionados([]);
    _gridHorariosInfoCarregamento.DrawTable();

    _knoutInfoCarregamento.DataCarregamento.val('');
}

function recarregarGridDatasCarregamento(callback) {
    executarReST("Carga/ObterDatasCarregamento", { Carga: _knoutCargaAlterarData.Codigo.val() }, function (retorno) {
        if (retorno.Success) {
            var edicaoHabilidata = (_knoutCargaAlterarData.EtapaDadosTransportador.enable() && _knoutCargaAlterarData.ExibirDatasCarregamento.enable());

            _gridDatasCarregamentoCarga.CarregarGrid(retorno.Data, edicaoHabilidata);

            if (callback instanceof Function)
                callback();
        }
        else
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, retorno.Msg);
    });
}

// #endregion Funções Privadas
