///// <autosync enabled="true" />
///// <reference path="../../../../js/libs/jquery-2.1.1.js" />
///// <reference path="../../../../js/Global/CRUD.js" />
///// <reference path="../../../../js/Global/knockout-3.1.0.js" />
///// <reference path="../../../../js/Global/Rest.js" />
///// <reference path="../../../../js/Global/Mensagem.js" />
///// <reference path="../../../../js/Global/Grid.js" />
///// <reference path="../../../../js/bootstrap/bootstrap.js" />
///// <reference path="../../../../js/libs/jquery.blockui.js" />
///// <reference path="../../../../js/Global/knoutViewsSlides.js" />
///// <reference path="../../../../js/libs/jquery.maskMoney.js" />
///// <reference path="../../../../js/plugin/datatables/jquery.dataTables.js" />
///// <reference path="../../../../js/libs/jquery.twbsPagination.js" />
///// <reference path="../../../../js/libs/jquery.globalize.js" />
///// <reference path="../../../../js/libs/jquery.globalize.pt-BR.js" />
///// <reference path="../../../Configuracao/Sistema/ConfiguracaoTMS.js" />
///// <reference path="../DadosEmissao/Configuracao.js" />
///// <reference path="../DadosEmissao/DadosEmissao.js" />
///// <reference path="../DadosEmissao/Geral.js" />
///// <reference path="../DadosEmissao/Lacre.js" />
///// <reference path="../DadosEmissao/LocaisPrestacao.js" />
///// <reference path="../DadosEmissao/Observacao.js" />
///// <reference path="../DadosEmissao/Passagem.js" />
///// <reference path="../DadosEmissao/Percurso.js" />
///// <reference path="../DadosEmissao/Rota.js" />
///// <reference path="../DadosEmissao/Seguro.js" />
///// <reference path="../DadosTransporte/DadosTransporte.js" />
///// <reference path="../DadosTransporte/Motorista.js" />
///// <reference path="../DadosTransporte/Tipo.js" />
///// <reference path="../DadosTransporte/Transportador.js" />
///// <reference path="../Documentos/CTe.js" />
///// <reference path="../Documentos/MDFe.js" />
///// <reference path="../Documentos/NFS.js" />
///// <reference path="../Documentos/PreCTe.js" />
///// <reference path="../DocumentosEmissao/CargaPedidoDocumentoCTe.js" />
///// <reference path="../DocumentosEmissao/ConsultaReceita.js" />
///// <reference path="../DocumentosEmissao/CTe.js" />
///// <reference path="../DocumentosEmissao/Documentos.js" />
///// <reference path="../DocumentosEmissao/DropZone.js" />
///// <reference path="../DocumentosEmissao/EtapaDocumentos.js" />
///// <reference path="../DocumentosEmissao/NotaFiscal.js" />
///// <reference path="../Frete/Complemento.js" />
///// <reference path="../Frete/Componente.js" />
///// <reference path="../Frete/EtapaFrete.js" />
///// <reference path="../Frete/Frete.js" />
///// <reference path="../Frete/SemTabela.js" />
///// <reference path="../Frete/TabelaCliente.js" />
///// <reference path="../Frete/TabelaComissao.js" />
///// <reference path="../Frete/TabelaRota.js" />
///// <reference path="../Frete/TabelaSubContratacao.js" />
///// <reference path="../Frete/TabelaTerceiros.js" />
///// <reference path="../Impressao/Impressao.js" />
///// <reference path="../Integracao/Integracao.js" />
///// <reference path="../Integracao/IntegracaoCarga.js" />
///// <reference path="../Integracao/IntegracaoCTe.js" />
///// <reference path="../Integracao/IntegracaoEDI.js" />
///// <reference path="../Terceiro/ContratoFrete.js" />
///// <reference path="Carga.js" />
///// <reference path="DataCarregamento.js" />
///// <reference path="Leilao.js" />
///// <reference path="Operador.js" />
///// <reference path="SignalR.js" />
///// <reference path="../../../Consultas/Tranportador.js" />
///// <reference path="../../../Consultas/Localidade.js" />
///// <reference path="../../../Consultas/ModeloVeicularCarga.js" />
///// <reference path="../../../Consultas/TipoCarga.js" />
///// <reference path="../../../Consultas/Motorista.js" />
///// <reference path="../../../Consultas/Veiculo.js" />
///// <reference path="../../../Consultas/GrupoPessoa.js" />
///// <reference path="../../../Consultas/TipoOperacao.js" />
///// <reference path="../../../Consultas/Filial.js" />
///// <reference path="../../../Consultas/Cliente.js" />
///// <reference path="../../../Consultas/Usuario.js" />
///// <reference path="../../../Consultas/TipoCarga.js" />
///// <reference path="../../../Consultas/RotaFrete.js" />
///// <reference path="../../../Enumeradores/EnumSituacoesCarga.js" />
///// <reference path="../../../Enumeradores/EnumTipoFreteEscolhido.js" />
///// <reference path="../../../Enumeradores/EnumTipoOperacaoEmissao.js" />
///// <reference path="../../../Enumeradores/EnumMotivoPendenciaFrete.js" />
///// <reference path="../../../Enumeradores/EnumTipoContratacaoCarga.js" />
///// <reference path="../../../Enumeradores/EnumSituacaoContratoFrete.js" />
///// <reference path="../../../Enumeradores/EnumStatusCTe.js" />
///// <reference path="../../../Enumeradores/EnumTipoPagamento.js" />
///// <reference path="../../../Enumeradores/EnumTipoEmissaoCTeParticipantes.js" />
///// <reference path="../../../Enumeradores/EnumSituacaoRetornoDadosFrete.js" />

///// <reference path="../../Enumeradores/EnumSituacaoLeilao.js" />

////*******MAPEAMENTO KNOUCKOUT*******


//var PesquisaTransportadorSugeridoLeilao = function () {
//    this.Carga = PropertyEntity();
//}

//var PesquisaParticipantesLeilao = function () {
//    this.Carga = PropertyEntity();
//}

//var PesquisaLancesLeilao = function () {
//    this.CodigoLeilao = PropertyEntity();
//}



//var EncerraLeilao = function () {
//    this.CodigoLeilao = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
//    this.Carga = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
//    this.Lance = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), required: true, text: "Lance:", idGrid: guid() });
//    this.Empresa = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: ":", idGrid: guid() });
//    this.ValorLance = PropertyEntity({ text: ":", idGrid: guid() });
//    this.EncerrarLeilaoSemEscolherLance = PropertyEntity({ type: types.event, eventClick: encerrarLeilaoClick, text: "Encerrar leilão sem escolher um lance", visible: ko.observable(true) });
//}

//var knoutCargaSelecionadaLeilao;

////*******EVENTOS*******

////Selecão da opção de Leilão
//function etapaLeilaoClick(e) {
//    ocultarTodasAbas(e);
//    var data = { Carga: e.Codigo.val() };
//    executarReST("CargaLeilao/BuscarLeilaoPorCarga", data, function (arg) {
//        if (arg.Success) {
//            var leilao = arg.Data;
//            if (leilao != null) {
//                e.CodigoLeilao.val(leilao.Codigo);
//                if (leilao.SituacaoLeilao == EnumSituacaoLeilao.novo) {
//                    e.ConfigurarLeilao.visible(true);
//                    e.AtivarLeilao.visible(false);
//                    e.LeilaoEmAndamento.visible(false);
//                    buscarSugestaoDeTransportadorParaLeilao(e);

//                } else {
//                    if (leilao.SituacaoLeilao == EnumSituacaoLeilao.iniciado) {
//                        buscarLancesLeilao(e);
//                        preecherDadosLeilao(e, leilao, leilao.SituacaoLeilao);
//                    } else {
//                        preecherDadosLeilao(e, leilao, leilao.SituacaoLeilao);
//                    }
//                }
//            } else {
//                e.ConfigurarLeilao.visible(false);
//                e.AtivarLeilao.visible(true);
//                e.LeilaoEmAndamento.visible(false);
//            }
//        } else {
//            exibirMensagem(tipoMensagem.falha, "falha", arg.Msg);
//        }
//    });
//}

//function preecherDadosLeilao(e, leilao, situacaoLeilao) {

//    e.LeilaoEmAndamento.visible(true);
//    e.LeilaoEmAndamento.val("O leilão está em andamento");
//    e.ConfigurarLeilao.visible(false);
//    e.AtivarLeilao.visible(false);
//    e.CodigoLeilao.val(leilao.Codigo);
//    e.VerParticipantes.visible(true);

//    if (leilao.DataInicioLeilao != "") {
//        e.DataInicioLeilao.val(leilao.DataInicioLeilao);
//        e.DataInicioLeilao.visible(true);
//    }

//    if (leilao.DataParaEncerramentoLeilao != "") {
//        e.EncerramentoLeilao.val(leilao.DataParaEncerramentoLeilao);
//        e.EncerramentoLeilao.visible(true);
//    }
//    if (leilao.DataDeEncerramentoLeilao != "") {
//        e.DatadeEncerramentoLeilao.val(leilao.DataDeEncerramentoLeilao);
//        e.DatadeEncerramentoLeilao.visible(true);
//    }
//    if (leilao.ValorDoLanceVencedor > 0) {
//        e.ValorDoLanceVencedor.val(Globalize.format(leilao.ValorDoLanceVencedor, "n2"));
//        e.ValorDoLanceVencedor.visible(true);
//    }
//    if (situacaoLeilao == EnumSituacaoLeilao.cancelado) {
//        e.LeilaoEmAndamento.val("Leilão Cancelado");

//        e.VencedorDoLeilao.val("O leilão foi cancelado e nenhum lance foi escolhido");
//        e.VencedorDoLeilao.visible(true);
//        e.LeilaoLances.visible(false);
//    }

//    if (situacaoLeilao == EnumSituacaoLeilao.encerrado) {
//        e.LeilaoEmAndamento.val("Leilão Encerrado");
//        if (leilao.VencedorDoLeilao != null) {
//            e.VencedorDoLeilao.val(leilao.VencedorDoLeilao.Descricao);
//            e.VencedorDoLeilao.codEntity(leilao.VencedorDoLeilao.Codigo);
//        } else {
//            e.VencedorDoLeilao.val("O leilão foi encerrado e nenhum lance foi escolhido");
//        }
//        e.VencedorDoLeilao.visible(true);
//        e.LeilaoLances.visible(false);
//    }

//}

////Criação ou não do leilão

//function criarLeilaoClick(e, sender) {
//    var data = { Carga: e.Codigo.val() };
//    executarReST("CargaLeilao/CriarLeilao", data, function (arg) {
//        if (arg.Success) {
//            $("#" + e.DivCarga.id + "_ribbonCargaNova").hide();
//            if (arg.Data) {
//                EtapaLeilaoAguardando(e);
//                e.ConfigurarLeilao.visible(true);
//                e.AtivarLeilao.visible(false);
//                buscarSugestaoDeTransportadorParaLeilao(e);
//                exibirMensagem(tipoMensagem.ok, "Sucesso", "Salvo com sucesso");
//            } else {
//                exibirMensagem(tipoMensagem.atencao, "Atenção", arg.Msg, 20000);
//            }
//        } else {
//            exibirMensagem(tipoMensagem.falha, "falha", arg.Msg);
//        }
//    });
//}


//function naoUsarLeilaoClick(e, sender) {
//    var data = { Carga: e.Codigo.val() };
//    executarReST("CargaLeilao/NaoUsarLeilao", data, function (arg) {
//        if (arg.Success) {
//            $("#" + e.DivCarga.id + "_ribbonCargaNova").hide();
//            if (arg.Data != false) {
//                e.SituacaoCarga.val(EnumSituacoesCarga.AgTransportador);
//                EtapaDadosTransportadorLiberada(e);
//                $("#" + e.EtapaDadosTransportador.idTab).trigger("click");
//            } else {
//                exibirMensagem(tipoMensagem.atencao, "Atenção", arg.Msg, 20000);
//            }
//        } else {
//            exibirMensagem(tipoMensagem.falha, "falha", arg.Msg);
//        }
//    });
//}


////Gerenciamento dos participantes

//function adicionarTranportadorLeilaoClick(e, knout) {

//    var data = { Carga: knout.Codigo.val(), Empresa: e.Codigo };
//    executarReST("CargaLeilao/AdicionarParticipanteNoLeilao", data, function (arg) {
//        if (arg.Success) {
//            if (arg.Data) {
//                buscarParticipantesDoLeilao(knout);
//                exibirMensagem(tipoMensagem.ok, "Sucesso", "Participante adicionado com sucesso");
//            } else {
//                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
//            }
//        } else {
//            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
//        }
//    });
//}

//function adicionarTotasSugeridasAoLeilaoClick(e, sender) {

//    exibirConfirmacao("Confirmação", "Realmente deseja adicionar todos os transportadores sugeridos ao Leilão?", function () {
//        var data = { Carga: e.Codigo.val() };
//        executarReST("CargaLeilao/AdicionarTodasSugeridasNoLeilao", data, function (arg) {
//            if (arg.Success) {
//                if (arg.Data) {
//                    buscarParticipantesDoLeilao(e);
//                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Todos os participantes foram adicionados com sucesso");
//                } else {
//                    exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
//                }
//            } else {
//                exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
//            }
//        });
//    })
//}


//function removerParticipanteLeilaoClick(e, knout) {
//    var data = { Carga: knout.Codigo.val(), CodigoCargaLeilaoParticipante: e.Codigo };
//    executarReST("CargaLeilao/RemoverParticipanteNoLeilao", data, function (arg) {
//        if (arg.Success) {
//            if (arg.Data) {
//                buscarParticipantesDoLeilao(knout);
//                exibirMensagem(tipoMensagem.ok, "Sucesso", "Participante removido com sucesso");
//            } else {
//                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
//            }
//        } else {
//            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
//        }
//    });
//}

//function verParticipantesClick(e, sender) {
//    var pesquisaParticipantesLeilao = new PesquisaParticipantesLeilao();
//    pesquisaParticipantesLeilao.Carga.val(e.Codigo.val());
//    var grid = new GridView("tableParticipantes", "CargaLeilao/BuscarTransportadoresParticipantesLeilao", pesquisaParticipantesLeilao, null, { column: 1, dir: orderDir.asc }, null, function () {
//        $('#divModalParticipantesDoLeilao').modal({ keyboard: true, backdrop: 'static' });
//    });
//    grid.CarregarGrid();
//}

////Inicio do leilão

//function cancelarLeilaoClick(e, sender) {
//    exibirConfirmacao("Confirmação", "Deseja Cancelar o leilão?", function () {
//        var data = { Carga: e.Codigo.val() };
//        executarReST("CargaLeilao/CancelarLeilao", data, function (arg) {
//            if (arg.Success) {
//                if (arg.Data != false) {
//                    e.SituacaoCarga.val(EnumSituacoesCarga.AgTransportador);
//                    EtapaDadosTransportadorLiberada(e);
//                    $("#" + e.EtapaDadosTransportador.idTab).trigger("click");
//                } else {
//                    exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
//                }
//            } else {
//                exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
//            }
//        });
//    });
//}

//function iniciarLeilaoClick(e, sender) {
//    exibirConfirmacao("Confirmação", "Realmente deseja iniciar o leilão?", function () {
//        var data = { Carga: e.Codigo.val() };
//        executarReST("CargaLeilao/IniciarLeilao", data, function (arg) {
//            if (arg.Success) {
//                if (arg.Data != false) {
//                    var leilao = arg.Data;
//                    preecherDadosLeilao(e, leilao, EnumSituacaoLeilao.iniciado);
//                    buscarLancesLeilao(e);
//                } else {
//                    exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
//                }
//            } else {
//                exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
//            }
//        });
//    })
//}


////Encerramento do leilão

//function abrirModalEncerrarLeilaoClick(e, sender) {
//    var encerraLeilao = new EncerraLeilao();
//    encerraLeilao.CodigoLeilao.val(e.CodigoLeilao.val());
//    encerraLeilao.Carga.val(e.Codigo.val());
//    KoBindings(encerraLeilao, "contentEncerramentoLeilao");
//    var pesquisaLancesLeilao = new PesquisaLancesLeilao();
//    pesquisaLancesLeilao.CodigoLeilao.val(e.CodigoLeilao.val());


//    var editar = {
//        descricao: "Escolher", id: guid(), evento: "onclick", metodo: function (arg) {
//            encerrarLeilaoComLanceClick(encerraLeilao, arg);
//        }, tamanho: "10", icone: ""
//    };
//    var menuOpcoes = { tipo: TypeOptionMenu.link, opcoes: [editar] };

//    var grid = new GridView(encerraLeilao.Lance.idGrid, "CargaLeilao/BuscarLancesDoLeilao", pesquisaLancesLeilao, menuOpcoes, { column: 2, dir: orderDir.asc }, null, function () {
//        knoutCargaSelecionadaLeilao = e;
//        $('#divModalEncerramentoLeilao').modal({ keyboard: true, backdrop: 'static' });
//    });
//    grid.CarregarGrid();
//}

//function encerrarLeilaoClick(e, sender) {
//    exibirConfirmacao("Confirmação", "Você realmente deseja finalizar o leilão sem escolher um lance?", function () {
//        encerrarLeilao(e);
//    });
//}

//function encerrarLeilaoComLanceClick(e, data) {
//    exibirConfirmacao("Confirmação", "Você realmente deseja selecionar o lance do participante " + data.RazaoSocial + " no valor de " + data.ValorLance + "?", function () {
//        e.Empresa.val(data.RazaoSocial);
//        e.Empresa.codEntity(data.CodigoEmpresa);
//        e.Lance.codEntity(data.Codigo);
//        e.ValorLance.val(data.ValorLance);
//        encerrarLeilao(e);
//    });
//}


////*******MÉTODOS*******


//function buscarSugestaoDeTransportadorParaLeilao(e) {
//    var pesquisaTransportadorSugeridoLeilao = new PesquisaTransportadorSugeridoLeilao();
//    pesquisaTransportadorSugeridoLeilao.Carga.val(e.Codigo.val());

//    var editar = {
//        descricao: "Adicionar", id: guid(), evento: "onclick", metodo: function (arg) {
//            adicionarTranportadorLeilaoClick(arg, e);
//        }, tamanho: "20", icone: ""
//    };
//    var menuOpcoes = new Object();
//    menuOpcoes.tipo = TypeOptionMenu.link;
//    menuOpcoes.opcoes = new Array();
//    menuOpcoes.opcoes.push(editar);

//    var gridTranportadores = new GridView(e.LeilaoTransportadorasSugeridas.idGrid, "CargaLeilao/BuscarTransportadoresSugeridosParaLeilao", pesquisaTransportadorSugeridoLeilao, menuOpcoes, { column: 1, dir: orderDir.asc }, null, function () {
//        buscarParticipantesDoLeilao(e);

//        var adicionar = [{
//            descricao: "Adicionar", id: guid(), evento: "onclick", metodo: function (ret) {
//                adicionarTranportadorLeilaoClick(ret, e);
//            }, tamanho: "10", icone: ""
//        }];
//        var menuOpcoes = { tipo: TypeOptionMenu.link, opcoes: adicionar };
//        new BuscarTransportadores(e.BuscarPorOutrasTransportadoras, null, menuOpcoes);
//    });
//    gridTranportadores.CarregarGrid();
//}

//function buscarParticipantesDoLeilao(e) {
//    var pesquisaParticipantesLeilao = new PesquisaParticipantesLeilao();
//    pesquisaParticipantesLeilao.Carga.val(e.Codigo.val());

//    var editar = {
//        descricao: "Remover", id: guid(), evento: "onclick", metodo: function (arg) {
//            removerParticipanteLeilaoClick(arg, e);
//        }, tamanho: "20", icone: ""
//    };
//    var menuOpcoes = new Object();
//    menuOpcoes.tipo = TypeOptionMenu.link;
//    menuOpcoes.opcoes = new Array();
//    menuOpcoes.opcoes.push(editar);

//    var grid = new GridView(e.LeilaoTransportadorasParticipantes.idGrid, "CargaLeilao/BuscarTransportadoresParticipantesLeilao", pesquisaParticipantesLeilao, menuOpcoes, { column: 1, dir: orderDir.asc });
//    grid.CarregarGrid();
//}


//function buscarLancesLeilao(e) {
//    var pesquisaLancesLeilao = new PesquisaLancesLeilao();
//    pesquisaLancesLeilao.CodigoLeilao.val(e.CodigoLeilao.val());
//    var grid = new GridView(e.LeilaoLances.idGrid, "CargaLeilao/BuscarLancesDoLeilao", pesquisaLancesLeilao, null, { column: 2, dir: orderDir.asc });
//    grid.CarregarGrid();
//}


//function encerrarLeilao(e) {
//    var data = { Carga: e.Carga.val(), CodigoLeilao: e.CodigoLeilao.val(), Lance: e.Lance.codEntity() };
//    executarReST("CargaLeilao/FinalizarLeilao", data, function (arg) {
//        if (arg.Success) {
//            if (arg.Data) {
//                knoutCargaSelecionadaLeilao.SituacaoCarga.val(EnumSituacoesCarga.AgTransportador);
//                EtapaDadosTransportadorLiberada(knoutCargaSelecionadaLeilao);
//                $("#" + knoutCargaSelecionadaLeilao.EtapaDadosTransportador.idTab).trigger("click");
//                exibirMensagem(tipoMensagem.ok, "Sucesso", "Leilao Finalizado com sucesso");
//                $('#divModalEncerramentoLeilao').modal('hide');
//                if (e.Lance.codEntity() > 0) {
//                    knoutCargaSelecionadaLeilao.TipoFreteEscolhido.val(EnumTipoFreteEscolhido.Leilao);
//                    knoutCargaSelecionadaLeilao.ValorFrete.val(e.ValorLance.val());
//                    knoutCargaSelecionadaLeilao.InfoTipoFreteEscolhido.val("(Frete escolhido por leilão)");
//                    knoutCargaSelecionadaLeilao.InfoTipoFreteEscolhido.visible(true);
//                    knoutCargaSelecionadaLeilao.Empresa.codEntity(e.Empresa.codEntity());
//                    knoutCargaSelecionadaLeilao.Empresa.val(e.Empresa.val());
//                }
//            } else {
//                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
//            }
//        } else {
//            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
//        }
//    });
//}

//function EtapaLeilaoDesabilitada(e) {
//    $("#" + e.EtapaLeilao.idTab).removeAttr("data-toggle");
//    $("#" + e.EtapaLeilao.idTab + " .step").attr("class", "step");
//    e.EtapaLeilao.eventClick = function (e) { };
//}

//function EtapaLeilaoAguardandoComLance(e) {
//    $("#" + e.EtapaLeilao.idTab).attr("data-toggle", "tab");
//    $("#" + e.EtapaLeilao.idTab + " .step").attr("class", "step orange");
//    e.EtapaLeilao.eventClick = etapaLeilaoClick;
//    EtapaFreteEmbarcadorAprovada(e);
//}

//function EtapaLeilaoAguardando(e) {
//    $("#" + e.EtapaLeilao.idTab).attr("data-toggle", "tab");
//    $("#" + e.EtapaLeilao.idTab + " .step").attr("class", "step yellow");
//    e.EtapaLeilao.eventClick = etapaLeilaoClick;
//    EtapaFreteEmbarcadorAprovada(e);
//}

//function EtapaLeilaoLiberada(e) {
//    $("#" + e.EtapaLeilao.idTab + " .step").attr("class", "step yellow");
//    $("#" + e.EtapaLeilao.idTab).attr("data-toggle", "tab");
//    e.EtapaLeilao.eventClick = etapaLeilaoClick;
//    EtapaFreteEmbarcadorAprovada(e);
//}

//function EtapaLeilaoAprovada(e) {
//    if (e.PermiteLeilao.val()) {
//        $("#" + e.EtapaLeilao.idTab + " .step").attr("class", "step green");
//        $("#" + e.EtapaLeilao.idTab).attr("data-toggle", "tab");
//        e.EtapaLeilao.eventClick = etapaLeilaoClick;
//    } else {
//        $("#" + e.EtapaLeilao.idTab).removeAttr("data-toggle");
//        $("#" + e.EtapaLeilao.idTab + " .step").attr("class", "step");
//        e.EtapaLeilao.eventClick = function (e) { };
//    }
//    EtapaFreteEmbarcadorAprovada(e);
//}

//function EtapaLeilaoProblema(e) {
//    $("#" + e.EtapaLeilao.idTab + " .step").attr("class", "step green");
//    $("#" + e.EtapaLeilao.idTab).attr("data-toggle", "tab");
//    e.EtapaLeilao.eventClick = etapaLeilaoClick;
//    EtapaFreteEmbarcadorAprovada(e);
//}

//function EtapaLeilaoEdicaoDesabilitada(e) {
//    e.EtapaLeilao.enable(false);
//    EtapaFreteEmbarcadorEdicaoDesabilitada(e);
//}
