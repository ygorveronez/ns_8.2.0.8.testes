/// <reference path="../../../js/libs/jquery-2.1.1.js" />
/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/knockout-3.1.0.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/bootstrap/bootstrap.js" />
/// <reference path="../../../js/libs/jquery.blockui.js" />
/// <reference path="../../../js/Global/knoutViewsSlides.js" />
/// <reference path="../../../js/libs/jquery.maskMoney.js" />
/// <reference path="../../../js/plugin/datatables/jquery.dataTables.js" />
/// <reference path="../../Enumeradores/EnumSituacaoOcorrencia.js" /> 
/// <reference path="../../Consultas/ComponenteFrete.js" />
/// <reference path="../../Consultas/Carga.js" />
/// <reference path="Ocorrencia.js" />
/// <reference path="../../Enumeradores/EnumSituacaoSolicitacaoCredito.js" />
/// <reference path="EtapasOcorrencia.js" />


//*******MAPEAMENTO KNOUCKOUT*******

var _ocorrenciaAutorizacao;
var _ocorrenciaAutorizacaoEmissao;
var _detalheAutorizacao;
var _gridAutorizacoes;
var _gridAutorizacoesEmissao;
var _delegarOcorrenciaAutorizacao;
var _rejeitarOcorrenciaAutorizacao;

var OcorrenciaAutorizacao = function () {
    this.Solicitante = PropertyEntity({ codEntity: ko.observable(0), text: Localization.Resources.Ocorrencias.Ocorrencia.Solicitante.getFieldDescription() });
    this.DataRetorno = PropertyEntity({ text: Localization.Resources.Ocorrencias.Ocorrencia.DataRetorno.getFieldDescription(), getType: typesKnockout.dateTime, visible: ko.observable(true) });
    this.Creditor = PropertyEntity({ text: Localization.Resources.Ocorrencias.Ocorrencia.Supervisor.getFieldDescription(), visible: ko.observable(true) });
    this.RetornoSolicitacao = PropertyEntity({ type: types.map, maxlength: 300, text: Localization.Resources.Ocorrencias.Ocorrencia.Resposta.getFieldDescription(), visible: ko.observable(true) });
    this.MotivoSolicitacao = PropertyEntity({ type: types.map, maxlength: 300, text: Localization.Resources.Ocorrencias.Ocorrencia.Resposta.getFieldDescription(), visible: ko.observable(true) });
    this.Solicitado = PropertyEntity({ text: Localization.Resources.Ocorrencias.Ocorrencia.Solicitante.getFieldDescription(), visible: ko.observable(true) });
    this.ValorLiberado = PropertyEntity({ text: Localization.Resources.Ocorrencias.Ocorrencia.ValorAprovado.getFieldDescription(), getType: typesKnockout.decimal, visible: ko.observable(true) });
    this.ValorSolicitado = PropertyEntity({ text: Localization.Resources.Ocorrencias.Ocorrencia.ValorSolicitado.getFieldDescription(), getType: typesKnockout.decimal, visible: ko.observable(true) });
    this.DataSolicitacao = PropertyEntity({ text: Localization.Resources.Ocorrencias.Ocorrencia.DataSolicitacao.getFieldDescription(), getType: typesKnockout.dateTime, visible: ko.observable(true) });
    this.SituacaoSolicitacaoCredito = PropertyEntity({ visible: ko.observable(false) });
    this.DescricaoSituacao = PropertyEntity({ visible: ko.observable(true) });
    this.ObservacaoAprovador = PropertyEntity({ type: types.map, maxlength: 5000, text: Localization.Resources.Ocorrencias.Ocorrencia.ObservacaoAprovador.getFieldDescription(), visible: ko.observable(_CONFIGURACAO_TMS.ExibirObservacaoAprovadorAutorizacaoOcorrencia) });

    this.Autorizacao = PropertyEntity({ visible: ko.observable(false) });

    this.ReabrirOcorrencia = PropertyEntity({ eventClick: reabrirOcorrenciaClick, type: types.event, text: Localization.Resources.Ocorrencias.Ocorrencia.ReabrirOcorrencia, visible: ko.observable(false) });

    this.ConfirmarOcorrencia = PropertyEntity({ eventClick: confirmarOcorrenciaClick, type: types.event, text: Localization.Resources.Ocorrencias.Ocorrencia.Confirmar, visible: ko.observable(false) });
    //this.RejeitarOcorrencia = PropertyEntity({ eventClick: cancelarOcorrenciaClick, type: types.event, text: "Rejeitar", visible: ko.observable(false) });
    this.ExibirAprovadores = PropertyEntity({ eventClick: exibirAprovadoresClick, type: types.event, text: ko.observable(""), defExibe: Localization.Resources.Ocorrencias.Ocorrencia.ExibirAprovadores, defOculta: Localization.Resources.Ocorrencias.Ocorrencia.OcultarAprovadores, visible: ko.observable(false) });
    this.ExibirAprovadores.text(this.ExibirAprovadores.defExibe);

    this.UsuariosAutorizadores = PropertyEntity({ idGrid: guid(), visible: ko.observable(true), resumido: ko.observable(false) });

    // Versao Resumida para terceiros
    this.RegraResumo = PropertyEntity({ text: Localization.Resources.Ocorrencias.Ocorrencia.Regra.getFieldDescription(), val: ko.observable("") });
    this.DataResumo = PropertyEntity({ text: Localization.Resources.Ocorrencias.Ocorrencia.Data.getFieldDescription(), val: ko.observable("") });
    this.SituacaoResumo = PropertyEntity({ text: Localization.Resources.Ocorrencias.Ocorrencia.Situacao.getFieldDescription(), val: ko.observable("") });
    this.JustificativaResumo = PropertyEntity({ text: Localization.Resources.Ocorrencias.Ocorrencia.Justificativa.getFieldDescription(), val: ko.observable(""), visible: ko.observable(true) });
    this.MotivoResumo = PropertyEntity({ text: Localization.Resources.Ocorrencias.Ocorrencia.Motivo.getFieldDescription(), val: ko.observable(""), visible: ko.observable(true) });
    // Versao Resumida para terceiros

    // Mensagem da etapa
    this.MensagemEtapaSemRegra = PropertyEntity({ type: types.local, visible: ko.observable(false), title: Localization.Resources.Ocorrencias.Ocorrencia.AutorizacaoPendente, text: Localization.Resources.Ocorrencias.Ocorrencia.NenhumaRegraEncontradaOcorrenciaPermanece });
}

var DetalheAutorizacao = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });

    this.Regra = PropertyEntity({ text: Localization.Resources.Ocorrencias.Ocorrencia.Regra.getFieldDescription(), val: ko.observable("") });
    this.Data = PropertyEntity({ text: Localization.Resources.Ocorrencias.Ocorrencia.Data.getFieldDescription(), val: ko.observable("") });
    this.SituacaoDescricao = PropertyEntity({ text: Localization.Resources.Ocorrencias.Ocorrencia.Situacao.getFieldDescription(), val: ko.observable("") });
    this.Usuario = PropertyEntity({ text: Localization.Resources.Ocorrencias.Ocorrencia.Usuario.getFieldDescription(), val: ko.observable("") });
    this.Justificativa = PropertyEntity({ text: Localization.Resources.Ocorrencias.Ocorrencia.Justificativa.getFieldDescription(), val: ko.observable(""), visible: ko.observable(true) });
    this.Motivo = PropertyEntity({ text: Localization.Resources.Ocorrencias.Ocorrencia.Motivo.getFieldDescription(), val: ko.observable(""), visible: ko.observable(true) });
    this.Aprovar = PropertyEntity({ eventClick: aprovarOcorrenciaClick, type: types.event, text: Localization.Resources.Ocorrencias.Ocorrencia.Aprovar, visible: ko.observable(false) });
    this.Delegar = PropertyEntity({ eventClick: delegarOcorrenciaAutorizacaoClick, type: types.event, text: Localization.Resources.Ocorrencias.Ocorrencia.Delegar, visible: ko.observable(true) });
    this.Rejeitar = PropertyEntity({ eventClick: rejeitarOcorrenciaAutorizacaoClick, type: types.event, text: Localization.Resources.Ocorrencias.Ocorrencia.Rejeitar, visible: ko.observable(true) });
}

var DelegarOcorrencia = function () {
    this.UsuarioDelegado = PropertyEntity({ text: Localization.Resources.Ocorrencias.Ocorrencia.Usuario.getRequiredFieldDescription(), type: types.entity, codEntity: ko.observable(0), required: true, idBtnSearch: guid() });
    this.Observacao = PropertyEntity({ text: Localization.Resources.Ocorrencias.Ocorrencia.Observacao.getRequiredFieldDescription(), maxlength: 5000, required: ko.observable(false), visible: ko.observable(false) });
    this.Delegar = PropertyEntity({ eventClick: delegarOcorrenciaClick, type: types.event, text: Localization.Resources.Ocorrencias.Ocorrencia.Delegar, visible: ko.observable(true) });
    this.CancelarDelegar = PropertyEntity({ eventClick: cancelarDelegarClick, type: types.event, text: Localization.Resources.Gerais.Geral.Cancelar, visible: ko.observable(true) });
};

var RejeitarOcorrencia = function () {
    this.Justificativa = PropertyEntity({ text: Localization.Resources.Ocorrencias.Ocorrencia.Justificativa.getRequiredFieldDescription(), type: types.entity, codEntity: ko.observable(0), required: true, enable: true, idBtnSearch: guid(), visible: ko.observable(true) });
    this.Motivo = PropertyEntity({ text: Localization.Resources.Ocorrencias.Ocorrencia.Motivo.getRequiredFieldDescription(), maxlength: 1000, required: true, enable: ko.observable(true), visible: ko.observable(true) });

    this.Rejeitar = PropertyEntity({ eventClick: rejeitarOcorrenciaClick, type: types.event, text: Localization.Resources.Ocorrencias.Ocorrencia.Rejeitar, visible: ko.observable(true) });
    this.Cancelar = PropertyEntity({ eventClick: cancelarRejeitarClick, type: types.event, text: Localization.Resources.Gerais.Geral.Cancelar, visible: ko.observable(true) });
};


//*******EVENTOS*******

function loadOcorrenciaAutorizacao() {
    _ocorrenciaAutorizacao = new OcorrenciaAutorizacao();
    KoBindings(_ocorrenciaAutorizacao, "knockoutOcorrenciaAprovacao");

    _ocorrenciaAutorizacaoEmissao = new OcorrenciaAutorizacao();
    KoBindings(_ocorrenciaAutorizacaoEmissao, "knockoutOcorrenciaAprovacaoEmissao");

    _detalheAutorizacao = new DetalheAutorizacao();
    KoBindings(_detalheAutorizacao, "knockoutDetalheAutorizacao");

    _delegarOcorrenciaAutorizacao = new DelegarOcorrencia();
    KoBindings(_delegarOcorrenciaAutorizacao, "knockoutDelegarOcorrencia");

    _rejeitarOcorrenciaAutorizacao = new RejeitarOcorrencia();
    KoBindings(_rejeitarOcorrenciaAutorizacao, "knockoutRejeitarOcorrencia");

    BuscarMotivoRejeicaoOcorrencia(_rejeitarOcorrenciaAutorizacao.Justificativa, null, EnumAprovacaoRejeicao.Rejeicao);
    BuscarFuncionario(_delegarOcorrenciaAutorizacao.UsuarioDelegado);

    if ((_CONFIGURACAO_TMS.TipoServicoMultisoftware == EnumTipoServicoMultisoftware.MultiCTe) && !_CONFIGURACAO_TMS.ExibirAprovadoresOcorrenciaPortalTransportador) {
        _ocorrenciaAutorizacao.UsuariosAutorizadores.resumido(true);
        _ocorrenciaAutorizacaoEmissao.UsuariosAutorizadores.resumido(true);
    }

    //-- Grid autorizadores
    var detalhes = { descricao: Localization.Resources.Ocorrencias.Ocorrencia.Detalhes, id: "clasEditar", evento: "onclick", metodo: detalharAutorizacaoEmissaoOcorrenciaClick, tamanho: "15", icone: "" };
    var menuOpcoes = new Object();
    menuOpcoes.tipo = TypeOptionMenu.link;
    menuOpcoes.opcoes = new Array();
    menuOpcoes.opcoes.push(detalhes);

    _gridAutorizacoes = new GridView(_ocorrenciaAutorizacao.UsuariosAutorizadores.idGrid, "Ocorrencia/ConsultarAutorizacoes", _ocorrencia, menuOpcoes, null, null, null, null, null, null);
    _gridAutorizacoesEmissao = new GridView(_ocorrenciaAutorizacaoEmissao.UsuariosAutorizadores.idGrid, "Ocorrencia/ConsultarAutorizacoesEmissao", _ocorrencia, menuOpcoes, null, null, null, null, null, null);
}

function exibirAprovadoresClick() {
    var exibir = _ocorrenciaAutorizacaoEmissao.UsuariosAutorizadores.visible() == false;
    _ocorrenciaAutorizacaoEmissao.UsuariosAutorizadores.visible(exibir);
    _ocorrenciaAutorizacaoEmissao.SituacaoSolicitacaoCredito.visible(exibir);

    if (exibir)
        _ocorrenciaAutorizacaoEmissao.ExibirAprovadores.text(_ocorrenciaAutorizacaoEmissao.ExibirAprovadores.defOculta);
    else
        _ocorrenciaAutorizacaoEmissao.ExibirAprovadores.text(_ocorrenciaAutorizacaoEmissao.ExibirAprovadores.defExibe);
}

function detalharAutorizacaoEmissaoOcorrenciaClick(dataRow) {
    _detalheAutorizacao.Codigo.val(dataRow.Codigo);
    BuscarPorCodigo(_detalheAutorizacao, "Ocorrencia/DetalhesAutorizacao", function (arg) {
        if (arg.Success) {
            if (arg.Data != null) {
                Global.abrirModal('divModalDetalhesAutorizacao');

                if (arg.Data.CodigoUsuario == arg.Data.UsuarioLogado) {
                    _detalheAutorizacao.Rejeitar.visible(true);
                    _detalheAutorizacao.Delegar.visible(true);

                    if (arg.Data.Situacao == EnumSituacaoOcorrenciaAutorizacao.Pendente)
                        _detalheAutorizacao.Aprovar.visible(true);
                    else if (arg.Data.Situacao == EnumSituacaoOcorrenciaAutorizacao.Rejeitada || arg.Data.Situacao == EnumSituacaoOcorrenciaAutorizacao.Aprovada ) {
                            _detalheAutorizacao.Aprovar.visible(false);
                            _detalheAutorizacao.Rejeitar.visible(false);
                            _detalheAutorizacao.Delegar.visible(false);
                    }

                }
                else {
                    _detalheAutorizacao.Aprovar.visible(false);
                    _detalheAutorizacao.Rejeitar.visible(false);
                    _detalheAutorizacao.Delegar.visible(false);
                }

                $("#divModalDetalhesAutorizacao").one('hidden.bs.modal', function () {
                    LimparCamposDetalheAutorizacao();
                });
            }
        } else {
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, arg.Msg);
        }
    });
}

function reabrirOcorrenciaClick(e, sender) {
    exibirConfirmacao(Localization.Resources.Ocorrencias.Ocorrencia.Confirmacao, Localization.Resources.Ocorrencias.Ocorrencia.RealmenteDesejaReabrirOcorrencia, function () {
        Salvar(_ocorrencia, "Ocorrencia/Reabrir", function (arg) {
            if (arg.Success) {
                if (arg.Data) {                    

                    limparCamposOcorrencia();
                    _ocorrencia.Codigo.val(arg.Data.Codigo);
                    configuracaoTipoOcorrencia = null;
                    buscarOcorrenciaPorCodigo();

                    //_ocorrencia.SituacaoOcorrencia.val(arg.Data.SituacaoOcorrencia);
                    //preecherCamposEdicaoOcorrencia(arg.Data.OrigemOcorrencia, arg);
                } else {
                    exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.Aviso, arg.Msg);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, arg.Msg);
            }
        }, sender);
    });
}

function confirmarOcorrenciaClick(e, sender) {
    _ocorrencia.ObservacaoCancelamento.required = false;
    exibirConfirmacao(Localization.Resources.Ocorrencias.Ocorrencia.Confirmacao, Localization.Resources.Ocorrencias.Ocorrencia.RealmenteDesejaConfirmarOcorrencia, function () {
        Salvar(_ocorrencia, "Ocorrencia/ConfirmarUtilizacao", function (arg) {
            if (arg.Success) {
                if (arg.Data) {
                    _gridViewOcorrencia.CarregarGrid();
                    _ocorrencia.SituacaoOcorrencia.val(arg.Data.SituacaoOcorrencia);
                    if (arg.Data.SituacaoOcorrencia == EnumSituacaoOcorrencia.Finalizada) {
                        exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, Localization.Resources.Ocorrencias.Ocorrencia.OcorrenciaFinalizadaComSucesso);
                        limparCamposOcorrencia();
                    };
                    if (arg.Data.SituacaoOcorrencia == EnumSituacaoOcorrencia.EmEmissaoCTeComplementar) {
                        _ocorrencia.Codigo.val(arg.Data.Codigo);
                        _CRUDOcorrencia.GerarNovaOcorrencia.visible(true);
                        _CRUDOcorrencia.Adicionar.visible(false);
                        exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, Localization.Resources.Ocorrencias.Ocorrencia.AguardeEmissaoDosCTesComplementaresParaFinalizarOcorrencia);
                        //PreencherCTesComplementares(arg.Data.CargaCTesComplementares);
                        PreencherCTesComplementares();
                    }
                    preecherCamposEdicaoOcorrencia();
                    //AtualizarDadosControleSaldo();
                } else {
                    exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.Aviso, arg.Msg);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, arg.Msg);
            }
        }, sender);
    });
}

function LimparCamposDetalheAutorizacao() {
    LimparCampos(_detalheAutorizacao)
}

function buscarRegrasOcorrenciaClick() {
    BuscarRegrasDaEtapa();
}

//*******MÉTODOS*******

function AtualizarAprovadoresEmissao() {
    _documentoComplementar.PreCTesComplementares.visible(false);
    _documentoComplementar.ImportarCTes.visible(false);

    _ocorrenciaAutorizacaoEmissao.UsuariosAutorizadores.visible(true);
    _ocorrenciaAutorizacaoEmissao.SituacaoSolicitacaoCredito.visible(true);
}

function preecherOcorrenciaAutorizacaoKnout(knout, grid, dadosAutorizacao) {
    if (dadosAutorizacao != null) {
        knout.UsuariosAutorizadores.visible(false);
        knout.DescricaoSituacao.visible(true);
        knout.SituacaoSolicitacaoCredito.visible(true);
        knout.Autorizacao.visible(false);
        knout.ReabrirOcorrencia.visible(false);
        var data = { Data: dadosAutorizacao };
        PreencherObjetoKnout(knout, data);

        if (dadosAutorizacao.SituacaoSolicitacaoCredito == EnumSituacaoSolicitacaoCredito.AgLiberacao) {
            knout.DataRetorno.visible(false);
            knout.Creditor.visible(false);
            knout.ValorLiberado.visible(false);
            knout.RetornoSolicitacao.visible(false);
            if (_CONFIGURACAO_TMS.TipoServicoMultisoftware == EnumTipoServicoMultisoftware.MultiTMS)
                knout.ReabrirOcorrencia.visible(true);
        }

        if (dadosAutorizacao.SituacaoSolicitacaoCredito == EnumSituacaoSolicitacaoCredito.Liberado) {
            knout.ConfirmarOcorrencia.visible(true);
            knout.RejeitarOcorrencia.visible(true);
        }

        if (dadosAutorizacao.SituacaoSolicitacaoCredito == EnumSituacaoSolicitacaoCredito.Utilizado) {
            knout.ConfirmarOcorrencia.visible(false);
            knout.RejeitarOcorrencia.visible(false);
        }

        if (dadosAutorizacao.SituacaoSolicitacaoCredito == EnumSituacaoSolicitacaoCredito.Rejeitado) {
            knout.ValorLiberado.visible(false);
        }

        if (dadosAutorizacao.SituacaoSolicitacaoCredito == EnumSituacaoSolicitacaoCredito.Todos) {
            knout.DataRetorno.visible(false);
            knout.Creditor.visible(false);
            knout.ValorLiberado.visible(false);
            knout.RetornoSolicitacao.visible(false);
            knout.DescricaoSituacao.visible(false)
        }
    } else {
        knout.UsuariosAutorizadores.visible(false);
        knout.SituacaoSolicitacaoCredito.visible(false);
        knout.Autorizacao.visible(true);
    }

    grid.CarregarGrid(function () {
        if (grid.NumeroRegistros() > 0) {
            knout.UsuariosAutorizadores.visible(true);
            knout.Autorizacao.visible(false);

            if (_ocorrencia.SituacaoOcorrencia.val() != EnumSituacaoOcorrencia.AgAutorizacaoEmissao) {
                _ocorrenciaAutorizacaoEmissao.ExibirAprovadores.visible(true);
                if (_CONFIGURACAO_TMS.TipoServicoMultisoftware == EnumTipoServicoMultisoftware.MultiTMS)
                    knout.ReabrirOcorrencia.visible(true);
            }
        } else {
            _ocorrenciaAutorizacaoEmissao.ExibirAprovadores.visible(false);
        }
    });
}

function ResumoAutorizacaoAprovacaoOcorrencia(data) {
    if (data != null)
        PreencherObjetoKnout(_ocorrenciaAutorizacao, { Data: data });
    else
        LimparCampos(_ocorrenciaAutorizacao);
}

function ResumoAutorizacaoEmissaoOcorrencia(data) {
    if (data != null)
        PreencherObjetoKnout(_ocorrenciaAutorizacaoEmissao, { Data: data });
    else
        LimparCampos(_ocorrenciaAutorizacaoEmissao);
}

function preecherOcorrenciaAutorizacao(dadosAutorizacao) {
    preecherOcorrenciaAutorizacaoKnout(_ocorrenciaAutorizacao, _gridAutorizacoes, dadosAutorizacao);
    preecherOcorrenciaAutorizacaoKnout(_ocorrenciaAutorizacaoEmissao, _gridAutorizacoesEmissao, dadosAutorizacao);
}

function limparOcorrenciaAutorizacaoKnout(knout) {
    knout.DataRetorno.visible(true);
    knout.Creditor.visible(true);
    knout.RetornoSolicitacao.visible(true);
    knout.ValorLiberado.visible(true);
    knout.ConfirmarOcorrencia.visible(false);
    knout.SituacaoSolicitacaoCredito.visible(false);
}

function limparOcorrenciaAutorizacao() {
    limparOcorrenciaAutorizacaoKnout(_ocorrenciaAutorizacao);
    limparOcorrenciaAutorizacaoKnout(_ocorrenciaAutorizacaoEmissao);
    LimparCampos(_documentoComplementar);
    _ocorrenciaAutorizacaoEmissao.ExibirAprovadores.text(_ocorrenciaAutorizacaoEmissao.ExibirAprovadores.defExibe);
}

function BuscarRegrasDaEtapa() {
    var dados = {
        Ocorrencia: _ocorrencia.Codigo.val()
    };
    executarReST("Ocorrencia/AtualizarRegrasEtapas", dados, function (arg) {
        if (arg.Success) {
            if (arg.Data != null) {
                Etapa3Desabilitada();
                Etapa4Desabilitada();
                _ocorrencia.SituacaoOcorrencia.val(arg.Data.SituacaoOcorrencia);
                setarEtapasOcorrencia();
                preecherOcorrenciaAutorizacao(_ocorrencia.SolicitacaoCredito.val());
                _CRUDOcorrencia.ReprocessarRegras.visible(false);
            } else if (arg.Msg != null) {
                exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.Aviso, arg.Msg);
            } else {
                EtapaSemRegra();
            }
        } else {
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, arg.Msg);
        }
    });
}
function aprovarOcorrenciaClick() {
    var dados = {
        Codigo: _detalheAutorizacao.Codigo.val(),
    };
    executarReST("AutorizacaoOcorrencia/Aprovar", dados, function (arg) {
        if (arg.Success) {
            if (arg.Data !== false) {
                exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, "Aprovado com sucesso.");

                _gridAutorizacoes.CarregarGrid();
                _gridAutorizacoesEmissao.CarregarGrid();
                Global.fecharModal("divModalDetalhesAutorizacao");
            } else {
                exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Atencao, arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, arg.Msg);
        }
    });
}

function delegarOcorrenciaAutorizacaoClick() {
    Global.abrirModal('divModalDelegarOcorrencia');
}
function rejeitarOcorrenciaAutorizacaoClick() {
    Global.abrirModal('divModalRejeitarOcorrencia');
}

function rejeitarOcorrenciaClick() {
    if (!ValidarCamposObrigatorios(_rejeitarOcorrenciaAutorizacao)) {
        exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Atencao, Localization.Resources.Gerais.Geral.PorFavorInformeOsCamposObrigatorios);
        return;
    }

    if (_rejeitarOcorrenciaAutorizacao.Motivo.val().length < 20)
        return exibirMensagem(tipoMensagem.aviso, Localization.Resources.Ocorrencias.AutorizacaoOcorrencia.CaracteresMinimos, Localization.Resources.Ocorrencias.AutorizacaoOcorrencia.ParaRejeitarOcorrenciaENecessarioInformarAoMinimoVinteCarcteres);


    var dados = {
        Codigo: _detalheAutorizacao.Codigo.val(),
        Motivo: _rejeitarOcorrenciaAutorizacao.Motivo.val(),
        Justificativa: _rejeitarOcorrenciaAutorizacao.Justificativa.codEntity(),
    };

    executarReST("AutorizacaoOcorrencia/Rejeitar", dados, function (arg) {
        if (arg.Success) {
            if (arg.Data !== false) {
                exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, Localization.Resources.Ocorrencias.Ocorrencia.RejeitadoComSucesso);
                _gridAutorizacoes.CarregarGrid();
                _gridAutorizacoesEmissao.CarregarGrid();
                Global.fecharModal('divModalRejeitarOcorrencia');
                Global.fecharModal("divModalDetalhesAutorizacao");

            } else {
                exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Atencao, arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, arg.Msg);
        }
    });
}

function delegarOcorrenciaClick() {

    if (!ValidarCamposObrigatorios(_delegarOcorrenciaAutorizacao)) {
        exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Atencao, Localization.Resources.Gerais.Geral.PorFavorInformeOsCamposObrigatorios);
        return;
    }

    exibirConfirmacao(Localization.Resources.Gerais.Geral.Confirmacao, Localization.Resources.Ocorrencias.Ocorrencia.VoceRealmenteDesejaDelegarOcorrencia, function () {
        var dados = {
            Ocorrencia: _ocorrencia.Codigo.val(),
            UsuarioDelegado: _delegarOcorrenciaAutorizacao.UsuarioDelegado.codEntity(),
            Observacao: _delegarOcorrenciaAutorizacao.Observacao.val()
        };

        executarReST("AutorizacaoOcorrencia/DelegarOcorrencia", dados, function (arg) {
            if (arg.Success) {
                if (arg.Data !== false) {
                    exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, Localization.Resources.Gerais.Geral.EnviadoComSucesso);
                    _gridAutorizacoes.CarregarGrid();
                    _gridAutorizacoesEmissao.CarregarGrid();
                    Global.fecharModal('divModalDelegarOcorrencia');
                    Global.fecharModal("divModalDetalhesAutorizacao");


                } else {
                    exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Atencao, arg.Msg);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, arg.Msg);
            }
        })
    });
}

function cancelarDelegarClick() {
    LimparCampos(_delegarOcorrenciaAutorizacao);
    Global.fecharModal('divModalDelegarOcorrencia');
}

function cancelarRejeitarClick() {
    LimparCampos(_rejeitarOcorrenciaAutorizacao);
    Global.fecharModal('divModalRejeitarOcorrencia');
}