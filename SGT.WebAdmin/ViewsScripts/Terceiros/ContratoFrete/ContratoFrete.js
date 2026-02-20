/// <reference path="../../../js/libs/jquery-2.1.1.js" />
/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/bootstrap/bootstrap.js" />
/// <reference path="../../../js/libs/jquery.blockui.js" />
/// <reference path="../../../js/Global/knoutViewsSlides.js" />
/// <reference path="../../../js/libs/jquery.maskMoney.js" />
/// <reference path="../../../js/plugin/datatables/jquery.dataTables.js" />
/// <reference path="../../Enumeradores/EnumSituacaoContratoFrete.js" />
/// <reference path="../../Consultas/Cliente.js" />
/// <reference path="../../Global/Notificacoes/Notificacao.js" />
/// <reference path="../../Configuracao/Sistema/ConfiguracaoTMS.js" />
/// <reference path="../../Enumeradores/EnumPermissaoPersonalizada.js" />
/// <reference path="Etapa.js" />
/// <reference path="Aprovacao.js" />
/// <reference path="Bloqueio.js" />
/// <reference path="AcrescimoDesconto.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _gridContratoFrete;
var _contratoFrete;
var _pesquisaContratoFrete;
var _detalhesContratoFrete;
var _PermissoesPersonalizadas;

//var _situacaoBloqueioContratoFrete = [
//    { text: "Todos", value: "" },
//    { text: "Bloqueado", value: true },
//    { text: "Desbloqueado", value: false }
//];

var PesquisaContratoFrete = function () {
    this.Carga = PropertyEntity({ text: "Carga: " });
    //this.SituacaoContratoFrete = PropertyEntity({ val: ko.observable(new Array()), def: new Array(), getType: typesKnockout.selectMultiple, options: EnumSituacaoContratoFrete.ObterOpcoes(), text: "Situação: ", issue: 120, visible: ko.observable(true)});
    this.SituacaoContratoFrete = PropertyEntity({ val: ko.observable(new Array()), def: new Array(), getType: typesKnockout.selectMultiple, options: EnumSituacaoContratoFrete.ObterOpcoes(), text: "Situação: ", issue: 395 ,visible: ko.observable(true) });
    //this.SituacaoBloqueio = PropertyEntity({ val: ko.observable(""), options: _situacaoBloqueioContratoFrete, def: "", text: "Situação de Bloqueio: ", visible: ko.observable(false) })
    this.NumeroContrato = PropertyEntity({ text: "Número do Contrato: " });
    this.TransportadorTerceiro = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Terceiro:", idBtnSearch: guid(), issue: 56 });
    this.NumeroCIOT = PropertyEntity({ text: "Número CIOT: " });
    this.DataInicialContratoFrete = PropertyEntity({ text: "Data Inicial: ", getType: typesKnockout.date });
    this.DataFinalContratoFrete = PropertyEntity({ text: "Data Final: ", getType: typesKnockout.date });

    this.Pesquisar = PropertyEntity({
        eventClick: function (e) {
            _gridContratoFrete.CarregarGrid();
        }, type: types.event, text: "Pesquisar", idGrid: guid(), visible: ko.observable(true)
    });
    this.ExibirFiltros = PropertyEntity({
        eventClick: function (e) {
            if (e.ExibirFiltros.visibleFade()) {
                e.ExibirFiltros.visibleFade(false);
            } else {
                e.ExibirFiltros.visibleFade(true);
            }
        }, type: types.event, text: "Filtros de Pesquisa", idFade: guid(), visibleFade: ko.observable(true), visible: ko.observable(true)
    });
};

var ContratoFrete = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Carga = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.SituacaoContratoFrete = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });

    //this.RejeitarContrato = PropertyEntity({ eventClick: rejeitarContratoClick, type: types.event, text: "Rejeitar o Contrato", visible: ko.observable(false) });
    //this.AutorizarContrato = PropertyEntity({ eventClick: autorizarContratoClick, type: types.event, text: "Autorizar o Contrato", visible: ko.observable(false) });
    this.Reprocessar = PropertyEntity({ eventClick: reprocessarClick, type: types.event, text: "Reprocessar", idGrid: guid(), visible: ko.observable(true) });
    this.Atualizar = PropertyEntity({ eventClick: atualizarContratoClick, type: types.event, text: "Atualizar", idGrid: guid(), visible: ko.observable(false) });

    //this.CancelarContrato = PropertyEntity({ eventClick: rejeitarContratoClick, type: types.event, text: "Cancelar o Contrato", visible: ko.observable(false) });
    this.FinalizarContrato = PropertyEntity({ eventClick: finalizarContratoClick, type: types.event, text: "Encerrar o Contrato", visible: ko.observable(false) });

    //this.BloquearContrato = PropertyEntity({ eventClick: bloquearContratoClick, type: types.event, text: "Bloquear o Contrato", visible: ko.observable(false) });
    //this.DesbloquearContrato = PropertyEntity({ eventClick: DesbloquearContratoFreteClick, type: types.event, text: "Desbloquear o Contrato", visible: ko.observable(false) });

    this.ReabrirContrato = PropertyEntity({ eventClick: reabrirContratoClick, type: types.event, text: "Reabrir o Contrato", visible: ko.observable(false) });

    this.DataInicialContratoFrete = PropertyEntity({ text: "Data Inicial: ", getType: typesKnockout.dateTime, required: false, issue: 2, visible: ko.observable(true) });
    this.DataFinalContratoFrete = PropertyEntity({ text: "Data Final: ", getType: typesKnockout.dateTime, required: false, issue: 2, visible: ko.observable(true) });
};

//*******EVENTOS*******

function reabrirContratoClick(e) {
    exibirConfirmacao("Confirmação", "Realmente deseja reabrir esse contrato de frete?", function () {
        Salvar(e, "ContratoFrete/ReabrirContrato", function (arg) {
            if (arg.Success) {
                if (arg.Data !== false) {
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Contrato reaberto com sucesso");

                    var reload = {
                        Codigo: _contratoFrete.Codigo.val(),
                        CodigoCarga: _contratoFrete.Carga.val()
                    };
                    editarContratoFrete(reload);
                    ////_contratoFrete.CancelarContrato.visible(false);
                    //_contratoFrete.FinalizarContrato.visible(false);
                    //_detalhesContratoFrete.SituacaoContratoFrete.val(EnumSituacaoContratoFrete.Aberto);

                    //if (_detalhesContratoFrete.PossuiCIOT.val() === true)
                    //    BuscarCIOTCargaSubcontratada(_detalhesContratoFrete);
                    //else
                    //    buscarCargasPreCTe(_detalhesContratoFrete);

                    //_gridContratoFrete.CarregarGrid();
                    ////_contratoFrete.RejeitarContrato.visible(true);
                    ////_contratoFrete.AutorizarContrato.visible(true);
                    //_detalhesContratoFrete.PercentualAdiantamento.enable(true);
                    ////_detalhesContratoFrete.Descontos.enable(true);
                    //_detalhesContratoFrete.Observacao.enable(true);
                    ////_detalhesContratoFrete.ValorOutrosAdiantamento.enable(true);
                    //_detalhesContratoFrete.AlterarContrato.visible(true);
                    //_detalhesContratoFrete.ValorFreteSubcontratacao.enable(true);
                    //preecherDadosContrato(arg.Data, _detalhesContratoFrete);
                    //_detalhesContratoFrete.AdicionarValor.visible(true);
                    //CarregarGridValorContratoFrete(_detalhesContratoFrete);
                    //_contratoFrete.ReabrirContrato.visible(false);
                } else {
                    exibirMensagem(tipoMensagem.aviso, "Atenção", arg.Msg);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
            }
        });
    });
}

function loadContratoFrete() {
    buscarDetalhesOperador(function () {
        _pesquisaContratoFrete = new PesquisaContratoFrete();
        KoBindings(_pesquisaContratoFrete, "knockoutPesquisaContratoFrete", false, _pesquisaContratoFrete.Pesquisar.id);

        _contratoFrete = new ContratoFrete();
        KoBindings(_contratoFrete, "knockoutCadastroContratoFrete");
        new BuscarClientes(_pesquisaContratoFrete.TransportadorTerceiro);

        HeaderAuditoria("ContratoFrete", _contratoFrete);

        carregarConteudosHTML(function () {
            buscarContratoFretes();
            //LoadBloqueioContratoFrete();
            LoadValorContratoFrete(_detalhesContratoFrete);
            LoadAcrescimoDesconto();
            loadEtapas();
            loadAprovacao();
        });
    });
}

function reprocessarClick(e, sender) {
    executarReST("ContratoFrete/ReprocessarRegras", { Codigo: _contratoFrete.Codigo.val() }, function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                if (arg.Data.PossuiRegra) {
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Contrato está aguardando aprovação.");
                    editarContratoFrete(arg.Data);
                }
                else
                    exibirMensagem(tipoMensagem.aviso, "Sem Regra", "Nenhuma regra para aprovar o Contrato.");
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    });
}

function atualizarContratoClick(e, sender) {
    executarReST("ContratoFrete/Atualizar", { Codigo: _contratoFrete.Codigo.val() }, function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                var reload = {
                    Codigo: _contratoFrete.Codigo.val(),
                    CodigoCarga: _contratoFrete.Carga.val()
                };
                editarContratoFrete(reload);
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    });
}

//function rejeitarContratoClick(e) {
//    exibirConfirmacao("Confirmação", "Realmente deseja rejeitar esse contrato de frete?", function () {
//        Salvar(e, "ContratoFrete/RejeitarContrato", function (arg) {
//            if (arg.Success) {
//                if (arg.Data) {
//                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Contrato rejeitado com sucesso");
//                } else {
//                    limparCamposContratoFrete();
//                    _gridContratoFrete.CarregarGrid();
//                    exibirMensagem(tipoMensagem.aviso, "Atenção", arg.Msg);
//                }
//            } else {
//                exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
//            }
//        });
//    });
//}

//function autorizarContratoClick(e) {
//    exibirConfirmacao("Confirmação", "Realmente deseja autorizar esse contrato de frete?", function () {
//        Salvar(e, "ContratoFrete/AprovarContrato", function (arg) {
//            if (arg.Success) {
//                if (arg.Data !== false) {
//                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Contrato aprovado com sucesso");
//                    _contratoFrete.CancelarContrato.visible(true);
//                    _contratoFrete.FinalizarContrato.visible(true);
//                    _detalhesContratoFrete.SituacaoContratoFrete.val(EnumSituacaoContratoFrete.Aprovado);

//                    if (_detalhesContratoFrete.PossuiCIOT.val() === true)
//                        BuscarCIOTCargaSubcontratada(_detalhesContratoFrete);
//                    else
//                        buscarCargasPreCTe(_detalhesContratoFrete);

//                    _gridContratoFrete.CarregarGrid();
//                    _contratoFrete.ReabrirContrato.visible(true);
//                    _contratoFrete.RejeitarContrato.visible(false);
//                    _contratoFrete.AutorizarContrato.visible(false);
//                    _detalhesContratoFrete.PercentualAdiantamento.enable(false);
//                    //_detalhesContratoFrete.Descontos.enable(false);
//                    //_detalhesContratoFrete.ValorOutrosAdiantamento.enable(false);
//                    _detalhesContratoFrete.Observacao.enable(false);
//                    _detalhesContratoFrete.AlterarContrato.visible(false);
//                    _detalhesContratoFrete.ValorFreteSubcontratacao.enable(false);
//                    _detalhesContratoFrete.AdicionarValor.visible(false);
//                    CarregarGridValorContratoFrete(_detalhesContratoFrete);
//                    preecherDadosContrato(arg.Data, _detalhesContratoFrete);
//                } else {
//                    exibirMensagem(tipoMensagem.aviso, "Atenção", arg.Msg);
//                }
//            } else {
//                exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
//            }
//        });
//    });
//}

//function cancelarContratoClick(e) {
//    exibirConfirmacao("Confirmação", "Realmente deseja cancelar esse contrato de frete?", function () {
//        Salvar(e, "ContratoFrete/CancelarContrato", function (arg) {
//            if (arg.Success) {
//                if (arg.Data) {
//                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Contrato cancelado com sucesso");
//                } else {
//                    limparCamposContratoFrete();
//                    _gridContratoFrete.CarregarGrid();
//                    exibirMensagem(tipoMensagem.aviso, "Atenção", arg.Msg);
//                }
//            } else {
//                exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
//            }
//        });
//    });
//}

function finalizarContratoClick(e) {
    exibirConfirmacao("Confirmação", "Realmente deseja finalizar esse contrato de frete?", function () {
        Salvar(e, "ContratoFrete/FinalizarContrato", function (arg) {
            if (arg.Success) {
                if (arg.Data) {
                    limparCamposContratoFrete();
                    _gridContratoFrete.CarregarGrid();
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Contrato finalizado com sucesso");
                } else {
                    exibirMensagem(tipoMensagem.aviso, "Atenção", arg.Msg);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
            }
        });
    });
}

//function bloquearContratoClick(e) {
//    AbrirTelaBloqueioContratoFrete();
//}

function cancelarClick(e) {
    limparCamposContratoFrete();
}

//*******MÉTODOS*******

function buscarContratoFretes() {
    var editar = { descricao: "Editar", id: "clasEditar", evento: "onclick", metodo: editarContratoFrete, tamanho: 7, icone: "" };

    // Menu
    var menuOpcoes = {
        tipo: TypeOptionMenu.link,
        opcoes: [editar]
    };

    //if (VerificaSePossuiPermissaoEspecial(EnumPermissaoPersonalizada.ContratoFrete_PermiteAutorizarContrato, _PermissoesPersonalizadas))
    //    menuOpcoes.opcoes.push({ descricao: "Autorizar", id: guid(), evento: "onclick", metodo: autorizarContratoFrete, tamanho: "15", icone: "", visibilidade: visibilidadeOpcaoAutorizar });

    _gridContratoFrete = new GridView(_pesquisaContratoFrete.Pesquisar.idGrid, "ContratoFrete/Pesquisa", _pesquisaContratoFrete, menuOpcoes, null);
    _gridContratoFrete.CarregarGrid();
}

//function visibilidadeOpcaoAutorizar(contrato) {
//    if (contrato.Bloqueado)
//        return false;
//    else if (contrato.Situacao == EnumSituacaoContratoFrete.AgAprovacao)
//        return true;
//    else
//        return false;
//}

//function autorizarContratoFrete(contratoFrete) {
//    exibirConfirmacao("Confirmação", "Realmente deseja autorizar esse contrato de frete?", function () {
//        executarReST("ContratoFrete/AprovarContrato", { Codigo: contratoFrete.Codigo }, function (arg) {
//            if (arg.Success) {
//                if (arg.Data !== false) {
//                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Contrato aprovado com sucesso");
//                    _gridContratoFrete.CarregarGrid();
//                    limparCamposContratoFrete();
//                } else {
//                    exibirMensagem(tipoMensagem.aviso, "Atenção", arg.Msg);
//                }
//            } else {
//                exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
//            }
//        });
//    });
//}

function editarContratoFrete(contratoFreteGrid) {
    limparCamposContratoFrete();
    _contratoFrete.Codigo.val(contratoFreteGrid.Codigo);
    _contratoFrete.Carga.val(contratoFreteGrid.CodigoCarga);
    Salvar(_contratoFrete, "Carga/BuscarCargaPorCodigo", function (arg) {
        BuscarPorCodigo(_contratoFrete, "ContratoFrete/BuscarPorCodigo", function (retorno) {
            if (retorno.Success) {
                if (retorno.Data !== false) {
                    _pesquisaContratoFrete.ExibirFiltros.visibleFade(false);
                    $("#fdsCarga").html("");
                    var knoutCarga = GerarTagHTMLDaCarga("fdsCarga", arg.Data);
                    $("#" + knoutCarga.DivCarga.id).attr("class", "p-2");
                    desabilitarTodasOpcoes(knoutCarga);
                    _cargaAtual = knoutCarga;

                    var contratoFrete = retorno.Data;

                    SetarEtapa();

                    ListarAprovacoes(retorno.Data);

                    _detalhesContratoFrete = new CargasSubContratacao();
                    _detalhesContratoFrete.PaginaRelatorio.val("Terceiros/ContratoFrete");
                    preecherDadosContrato(contratoFrete, _detalhesContratoFrete);
                    if (contratoFrete.SituacaoContratoFrete == EnumSituacaoContratoFrete.Aberto) {
                        //if (VerificaSePossuiPermissaoEspecial(EnumPermissaoPersonalizada.ContratoFrete_PermiteAutorizarContrato, _PermissoesPersonalizadas)) {
                        //    _contratoFrete.RejeitarContrato.visible(true);
                        //    _contratoFrete.AutorizarContrato.visible(true);
                        //}

                        if (VerificaSePossuiPermissaoEspecial(EnumPermissaoPersonalizada.Alterar, _PermissoesPersonalizadas)) {
                            _detalhesContratoFrete.PercentualAdiantamento.enable(true);
                            _detalhesContratoFrete.PercentualAbastecimento.enable(true);
                            //_detalhesContratoFrete.Descontos.enable(true);
                            _detalhesContratoFrete.AdicionarValor.visible(true);
                            _detalhesContratoFrete.Observacao.enable(true);
                            //_detalhesContratoFrete.ValorOutrosAdiantamento.enable(true);
                            _detalhesContratoFrete.ValorFreteSubcontratacao.enable(true);

                            _detalhesContratoFrete.AlterarContrato.visible(true);
                        }

                        _contratoFrete.ReabrirContrato.visible(false);
                        _contratoFrete.Atualizar.visible(true);

                    } else if (contratoFrete.SituacaoContratoFrete == EnumSituacaoContratoFrete.Aprovado) {
                        //_contratoFrete.CancelarContrato.visible(true);
                        if (VerificaSePossuiPermissaoEspecial(EnumPermissaoPersonalizada.Finalizar, _PermissoesPersonalizadas))
                            _contratoFrete.FinalizarContrato.visible(true);

                        if (VerificaSePossuiPermissaoEspecial(EnumPermissaoPersonalizada.ReAbrir, _PermissoesPersonalizadas))
                            _contratoFrete.ReabrirContrato.visible(true);
                    } else if (contratoFrete.SituacaoContratoFrete == EnumSituacaoContratoFrete.SemRegra) {
                        _contratoFrete.Reprocessar.visible(true);
                    } else if (contratoFrete.SituacaoContratoFrete == EnumSituacaoContratoFrete.Rejeitado) {
                        if (VerificaSePossuiPermissaoEspecial(EnumPermissaoPersonalizada.ReAbrir, _PermissoesPersonalizadas))
                            _contratoFrete.ReabrirContrato.visible(true);
                    } else if (contratoFrete.SituacaoContratoFrete == EnumSituacaoContratoFrete.Finalizada) {
                        if (VerificaSePossuiPermissaoEspecial(EnumPermissaoPersonalizada.ReAbrir, _PermissoesPersonalizadas))
                            _contratoFrete.ReabrirContrato.visible(true);
                    }

                    if (_CONFIGURACAO_TMS.NaoPermitirImpressaoContratoFretePendente &&
                        contratoFrete.SituacaoContratoFrete != EnumSituacaoContratoFrete.Aprovado &&
                        contratoFrete.SituacaoContratoFrete != EnumSituacaoContratoFrete.Finalizada) {
                        _detalhesContratoFrete.ImprimirContrato.visible(false);
                        _detalhesContratoFrete.ImprimirRomaneioEntrega.visible(false);
                    } else {
                        _detalhesContratoFrete.ImprimirContrato.visible(true);
                        _detalhesContratoFrete.ImprimirRomaneioEntrega.visible(true);
                    }

                    if (contratoFrete.PossuiIntegracaoAX && (!contratoFrete.EnviouContratoAXComSucesso || !contratoFrete.EnviouAcertoContasContratoAXComSucesso))
                        _detalhesContratoFrete.EnviarIntegracaoQuitacaoAX.visible(true);
                    else
                        _detalhesContratoFrete.EnviarIntegracaoQuitacaoAX.visible(false);

                    _detalhesContratoFrete.HistoricoIntegracao.visible(true);
                    _detalhesContratoFrete.PercentualAdiantamento.visible(true);
                    _detalhesContratoFrete.PercentualAbastecimento.visible(true);
                    //_detalhesContratoFrete.Descontos.visible(true);
                    //_detalhesContratoFrete.AdicionarValor.visible(true);
                    _detalhesContratoFrete.Observacao.visible(true);
                    //_detalhesContratoFrete.ValorOutrosAdiantamento.visible(true);

                    if (contratoFrete.SituacaoContratoFrete == EnumSituacaoContratoFrete.Cancelado || contratoFrete.SituacaoContratoFrete == EnumSituacaoContratoFrete.Finalizada) {
                        //_contratoFrete.BloquearContrato.visible(false);
                        //_contratoFrete.DesbloquearContrato.visible(false);
                    } else {
                        if (contratoFrete.Bloqueado) {
                            //_contratoFrete.BloquearContrato.visible(false);

                            //if (VerificaSePossuiPermissaoEspecial(EnumPermissaoPersonalizada.ContratoFrete_PermiteBloquearContrato, _PermissoesPersonalizadas))
                            //    _contratoFrete.DesbloquearContrato.visible(true);

                            //_contratoFrete.RejeitarContrato.visible(false);
                            //_contratoFrete.AutorizarContrato.visible(false);
                            _contratoFrete.ReabrirContrato.visible(false);
                            _contratoFrete.FinalizarContrato.visible(false);

                            _detalhesContratoFrete.PercentualAdiantamento.enable(false);
                            _detalhesContratoFrete.PercentualAbastecimento.enable(false);
                            //_detalhesContratoFrete.Descontos.enable(false);
                            _detalhesContratoFrete.AdicionarValor.visible(false);
                            _detalhesContratoFrete.Observacao.enable(false);
                            _detalhesContratoFrete.AlterarContrato.visible(false);
                            //_detalhesContratoFrete.ValorOutrosAdiantamento.enable(false);
                            _detalhesContratoFrete.ValorFreteSubcontratacao.enable(false);
                        } else {
                            //if (VerificaSePossuiPermissaoEspecial(EnumPermissaoPersonalizada.ContratoFrete_PermiteBloquearContrato, _PermissoesPersonalizadas))
                            //    _contratoFrete.BloquearContrato.visible(true);

                            //_contratoFrete.DesbloquearContrato.visible(false);
                        }
                    }

                    BuscarAcrescimosDescontos();

                    var strKnoutCargaSubContratacao = guid();
                    $("#knockoutContratoFrete").html(_HTMLCargaContratoFrete.replace("#knoutCargaSubContratacao", strKnoutCargaSubContratacao));
                    KoBindings(_detalhesContratoFrete, strKnoutCargaSubContratacao);
                    LocalizeCurrentPage();

                    _valorContratoFrete.ContratoFrete.val(_detalhesContratoFrete.Codigo.val());

                    CarregarGridValorContratoFrete(_detalhesContratoFrete, _detalhesContratoFrete.AdicionarValor.visible());

                    if (_detalhesContratoFrete.PossuiCIOT.val() === true)
                        BuscarCIOTCargaSubcontratada(_detalhesContratoFrete);
                    else
                        buscarCargasPreCTe(_detalhesContratoFrete);

                    if (!VerificaSePossuiPermissaoEspecial(EnumPermissaoPersonalizada.ContratoFrete_PermiteInformarAcrescimoDesconto, _PermissoesPersonalizadas))
                        _detalhesContratoFrete.AdicionarValor.visible(false);

                    if (!VerificaSePossuiPermissaoEspecial(EnumPermissaoPersonalizada.ContratoFrete_PermiteInformarValorFrete, _PermissoesPersonalizadas))
                        _detalhesContratoFrete.ValorFreteSubcontratacao.enable(false);

                    $("#wid-id-4").show();
                } else {
                    exibirMensagem(tipoMensagem.atencao, "Atenção", retorno.Msg);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
            }
        });
    }, null);

}

function limparCamposContratoFrete() {
    //_contratoFrete.RejeitarContrato.visible(false);
    //_contratoFrete.AutorizarContrato.visible(false);
    //_contratoFrete.CancelarContrato.visible(false);
    _contratoFrete.FinalizarContrato.visible(false);
    //_contratoFrete.BloquearContrato.visible(false);
    //_contratoFrete.DesbloquearContrato.visible(false);
    _contratoFrete.Reprocessar.visible(false);
    _contratoFrete.Atualizar.visible(false);
    LimparCampos(_contratoFrete);
    _pesquisaContratoFrete.ExibirFiltros.visibleFade(true);
    $("#wid-id-4").hide();
    Global.ResetarAbas();
}