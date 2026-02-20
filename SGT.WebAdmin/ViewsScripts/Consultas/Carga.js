/// <reference path="../../js/Global/Buscas.js" />
/// <reference path="../../js/Global/Grid.js" />
/// <reference path="../../js/Global/CRUD.js" />
/// <reference path="Veiculo.js" />
/// <reference path="TipoOperacao.js" />
/// <reference path="../../js/libs/jquery-2.1.1.js" />
/// <reference path="../../js/Global/Globais.js" />

var BuscarCargaPermiteCTeComplementar = function (knout, callbackRetorno, knoutTipoOcorrencia) {

    var idDiv = guid();
    var GridConsulta;

    var OpcoesKnout = function () {
        this.Descricao = PropertyEntity({ text: "Número da Carga: ", col: 8 });
        this.NumeroCTe = PropertyEntity({ text: "Número do CTe: ", col: 2, getType: typesKnockout.int, maxlength: 9, configInt: { thousands: "", allowZero: false, precision: 0 } });
        this.NumeroMDFe = PropertyEntity({ text: "Número do MDFe: ", col: 2, getType: typesKnockout.int, maxlength: 9, configInt: { thousands: "", allowZero: false, precision: 0 } });
        this.TipoOcorrencia = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Tipo de Ocorrência:", idBtnSearch: guid(), issue: 0, visible: ko.observable(false) });
        this.Titulo = PropertyEntity({ text: "Buscar Cargas", type: types.local });
        this.TituloGrid = PropertyEntity({ text: "Cargas", type: types.local });
        this.Pesquisar = PropertyEntity({
            eventClick: function (e) {
                GridConsulta.CarregarGrid();
            }, type: types.event, text: "Pesquisar", visible: true
        });
    }

    var knoutOpcoes = new OpcoesKnout();

    var funcaoParametrosDinamicos = function () {
        if (knoutTipoOcorrencia != null) {
            knoutOpcoes.TipoOcorrencia.codEntity(knoutTipoOcorrencia.codEntity());
            knoutOpcoes.TipoOcorrencia.val(knoutTipoOcorrencia.val());
        }
    }

    var divBusca = new CriarBusca(idDiv, knoutOpcoes, knout, funcaoParametrosDinamicos, null, null, null, null, true);

    var callback = function (e) {
        divBusca.DefCallback(e);
    }

    if (callbackRetorno != null) {
        callback = function (e) {
            divBusca.CloseModal();
            callbackRetorno(e);
        }
    }

    GridConsulta = new GridView(idDiv + "_tabelaEntidades", "Carga/PesquisaCargaPermiteCTeComplementar", knoutOpcoes, divBusca.OpcaoPadrao(callback), null, null, null, null, null, null, 10);
    divBusca.AddEvents(GridConsulta);

    divBusca.AddTabPressEvent(function (outraPressionada) {
        if (outraPressionada) {
            knoutOpcoes.Descricao.val(knout.val());
        }
        GridConsulta.CarregarGrid(function (lista) {

            if (lista.data.length == 1) {
                callback(lista.data[0]);
            } else {
                divBusca.OpenModal();
            }
        });
    })
}

var BuscarCargaFinalizadas = function (knout, callbackRetorno) {

    var idDiv = guid();
    var GridConsulta;

    var OpcoesKnout = function () {
        this.CodigoCargaEmbarcador = PropertyEntity({ text: "Número da Carga: ", col: 12 });
        this.Titulo = PropertyEntity({ text: "Buscar Cargas Finalizadas", type: types.local });
        this.TituloGrid = PropertyEntity({ text: "Cargas", type: types.local });
        this.Situacao = PropertyEntity({ text: "Situação: ", col: 12, visible: false, val: ko.observable(11) });
        this.Pesquisar = PropertyEntity({
            eventClick: function (e) {
                GridConsulta.CarregarGrid();
            }, type: types.event, text: "Pesquisar", visible: true
        });
    }

    var knoutOpcoes = new OpcoesKnout();

    var divBusca = new CriarBusca(idDiv, knoutOpcoes, knout);

    knoutOpcoes.Situacao.val(11);

    //var callback = function (e) {
    //    divBusca.DefCallback(e);
    //}
    var callback = function (e) {
        knout.codEntity(e.Codigo);
        knout.val(e.OrigemDestino);
        knoutOpcoes.CodigoCargaEmbarcador.val(knoutOpcoes.CodigoCargaEmbarcador.def);
        divBusca.CloseModal();
        Global.setarFocoProximoCampo(knout.id);
    };

    if (callbackRetorno != null) {
        callback = function (e) {
            divBusca.CloseModal();
            callbackRetorno(e);
        }
    }


    GridConsulta = new GridView(idDiv + "_tabelaEntidades", "Carga/PesquisaCargasPorSituacao", knoutOpcoes, divBusca.OpcaoPadrao(callback), null, null, null, null, null, null, 10);
    divBusca.AddEvents(GridConsulta);

    divBusca.AddTabPressEvent(function (outraPressionada) {
        if (outraPressionada) {
            knoutOpcoes.CodigoCargaEmbarcador.val(knout.val());
        }
        GridConsulta.CarregarGrid(function (lista) {

            if (lista.data.length == 1) {
                callback(lista.data[0]);
            } else {
                divBusca.OpenModal();
            }
        });
    })
}

var BuscarCargaParaEncaixeDeSubcontratacao = function (knout, callbackRetorno) {

    var idDiv = guid();
    var GridConsulta;

    var OpcoesKnout = function () {
        this.CodigoCargaEmbarcador = PropertyEntity({ text: "Número da Carga: ", col: 12 });
        this.Titulo = PropertyEntity({ text: "Buscar Cargas para Encaixe", type: types.local });
        this.TituloGrid = PropertyEntity({ text: "Cargas", type: types.local });
        this.Situacao = PropertyEntity({ text: "Situação: ", col: 12, visible: false, val: ko.observable(11) });
        this.Pesquisar = PropertyEntity({
            eventClick: function (e) {
                GridConsulta.CarregarGrid();
            }, type: types.event, text: "Pesquisar", visible: true
        });
    }

    var knoutOpcoes = new OpcoesKnout();

    var divBusca = new CriarBusca(idDiv, knoutOpcoes, knout);

    knoutOpcoes.Situacao.val(11);

    //var callback = function (e) {
    //    divBusca.DefCallback(e);
    //}
    var callback = function (e) {
        knout.codEntity(e.Codigo);
        knout.val(e.OrigemDestino);
        knoutOpcoes.CodigoCargaEmbarcador.val(knoutOpcoes.CodigoCargaEmbarcador.def);
        divBusca.CloseModal();
        Global.setarFocoProximoCampo(knout.id);
    };

    if (callbackRetorno != null) {
        callback = function (e) {
            divBusca.CloseModal();
            callbackRetorno(e);
        }
    }


    GridConsulta = new GridView(idDiv + "_tabelaEntidades", "Carga/PesquisaCargasParaEncaixeDeSubcontratacao", knoutOpcoes, divBusca.OpcaoPadrao(callback), null, null, null, null, null, null, 10);
    divBusca.AddEvents(GridConsulta);

    divBusca.AddTabPressEvent(function (outraPressionada) {
        if (outraPressionada) {
            knoutOpcoes.CodigoCargaEmbarcador.val(knout.val());
        }
        GridConsulta.CarregarGrid(function (lista) {

            if (lista.data.length == 1) {
                callback(lista.data[0]);
            } else {
                divBusca.OpenModal();
            }
        });
    })
}

var BuscarCargaFinalizadasParaAcertoDeViagem = function (knout, callbackRetorno, basicGrid, knoutCodigoAcertoViagem) {

    var idDiv = guid();
    var GridConsulta;

    var multiplaEscolha = false;
    if (basicGrid != null) {
        multiplaEscolha = true;
    }

    var OpcoesKnout = function () {
        this.Titulo = PropertyEntity({ text: "Busca de Cargas para o Acerto de Viagem", type: types.local });
        this.TituloGrid = PropertyEntity({ text: "Cargas", type: types.local });
        this.CodigoCargaEmbarcador = PropertyEntity({ text: "Número da Carga: ", col: 3 });
        this.DataCarga = PropertyEntity({ text: "Data Carga: ", col: 3, getType: typesKnockout.date, visible: true });
        this.Veiculo = PropertyEntity({ col: 6, type: types.entity, codEntity: ko.observable(0), text: "Veículo:", idBtnSearch: guid(), visible: true });
        this.Situacao = PropertyEntity({ text: "Situacao: ", col: 12, visible: false, val: ko.observable(11) });
        this.AcertoViagem = PropertyEntity({ col: 12, type: types.entity, codEntity: ko.observable(0), text: "Acerto de Viagem:", idBtnSearch: guid(), visible: false });;
        this.Pesquisar = PropertyEntity({
            eventClick: function (e) {
                GridConsulta.CarregarGrid();
            }, type: types.event, text: "Pesquisar", visible: true
        });
    }

    var knoutOpcoes = new OpcoesKnout();
    var funcaoParamentroDinamico = null;

    if (knoutCodigoAcertoViagem != null) {
        knoutOpcoes.AcertoViagem.visible = false;
        funcaoParamentroDinamico = function () {
            knoutOpcoes.AcertoViagem.codEntity(knoutCodigoAcertoViagem.val());
            knoutOpcoes.AcertoViagem.val(knoutCodigoAcertoViagem.val());
        }
    }

    var divBusca = new CriarBusca(idDiv, knoutOpcoes, knout, funcaoParamentroDinamico, null, multiplaEscolha, function () {
        new BuscarVeiculos(knoutOpcoes.Veiculo);
    });

    var callback = function (e) {
        preecherRetornoSelecao(knout, e, idDiv, knoutOpcoes);
    };

    if (callbackRetorno != null) {
        callback = function (e) {
            divBusca.CloseModal();
            callbackRetorno(e);
        };
    }

    if (multiplaEscolha) {
        var objetoBasicGrid = { basicGrid: basicGrid, callback: callbackRetorno, eventos: knoutOpcoes.Pesquisar };
        GridConsulta = new GridView(idDiv + "_tabelaEntidades", "Carga/PesquisaCargaFinalizadasParaAcertoDeViagem", knoutOpcoes, null, null, null, null, null, null, objetoBasicGrid);
    } else {
        GridConsulta = new GridView(idDiv + "_tabelaEntidades", "Carga/PesquisaCargaFinalizadasParaAcertoDeViagem", knoutOpcoes, divBusca.OpcaoPadrao(callback), { column: 1, dir: orderDir.asc });
    }
    divBusca.AddEvents(GridConsulta);

    divBusca.AddTabPressEvent(function (outraPressionada) {
        if (outraPressionada) {
            knoutOpcoes.Nome.val(knout.val());
        }
        GridConsulta.CarregarGrid(function (lista) {
            if (lista.data.length == 1) {
                callback(lista.data[0]);
            } else {
                divBusca.OpenModal();
            }
        });
    })
}

var BuscarCargaComissaoFuncionario = function (knout, callbackRetorno, basicGrid, knoutCodigoComissaoFuncionario, knoutCodigoMotorista) {

    var idDiv = guid();
    var GridConsulta;

    var multiplaEscolha = false;
    if (basicGrid != null) {
        multiplaEscolha = true;
    }

    var OpcoesKnout = function () {
        this.Titulo = PropertyEntity({ text: "Busca de Cargas para Comissão Motorista", type: types.local });
        this.TituloGrid = PropertyEntity({ text: "Cargas", type: types.local });
        this.CodigoCargaEmbarcador = PropertyEntity({ text: "Número da Carga: ", col: 3 });
        this.DataCarga = PropertyEntity({ text: "Data Carga: ", col: 3, getType: typesKnockout.date, visible: true });
        this.Veiculo = PropertyEntity({ col: 6, type: types.entity, codEntity: ko.observable(0), text: "Veículo:", idBtnSearch: guid(), visible: true });
        this.ComissaoFuncionario = PropertyEntity({ col: 12, type: types.entity, codEntity: ko.observable(0), text: "Comissão Motorista:", idBtnSearch: guid(), visible: false });;
        this.Motorista = PropertyEntity({ col: 12, type: types.entity, codEntity: ko.observable(0), text: "Motorista:", idBtnSearch: guid(), visible: false });;
        this.Pesquisar = PropertyEntity({
            eventClick: function (e) {
                GridConsulta.CarregarGrid();
            }, type: types.event, text: "Pesquisar", visible: true
        });
    }

    var knoutOpcoes = new OpcoesKnout();
    var funcaoParamentroDinamico = null;

    if (knoutCodigoComissaoFuncionario != null && knoutCodigoMotorista != null) {
        knoutOpcoes.ComissaoFuncionario.visible = false;
        funcaoParamentroDinamico = function () {
            knoutOpcoes.ComissaoFuncionario.codEntity(knoutCodigoComissaoFuncionario.val());
            knoutOpcoes.ComissaoFuncionario.val(knoutCodigoComissaoFuncionario.val());
            knoutOpcoes.Motorista.codEntity(knoutCodigoMotorista.val());
            knoutOpcoes.Motorista.val(knoutCodigoMotorista.val());
        }
    }

    var divBusca = new CriarBusca(idDiv, knoutOpcoes, knout, funcaoParamentroDinamico, null, multiplaEscolha, function () {
        new BuscarVeiculos(knoutOpcoes.Veiculo);
    });

    var callback = function (e) {
        preecherRetornoSelecao(knout, e, idDiv, knoutOpcoes);
    };

    if (callbackRetorno != null) {
        callback = function (e) {
            divBusca.CloseModal();
            callbackRetorno(e);
        };
    }

    if (multiplaEscolha) {
        var objetoBasicGrid = { basicGrid: basicGrid, callback: callbackRetorno, eventos: knoutOpcoes.Pesquisar };
        GridConsulta = new GridView(idDiv + "_tabelaEntidades", "Carga/PesquisaCargaComissaoFuncionario", knoutOpcoes, null, null, null, null, null, null, objetoBasicGrid);
    } else {
        GridConsulta = new GridView(idDiv + "_tabelaEntidades", "Carga/PesquisaCargaComissaoFuncionario", knoutOpcoes, divBusca.OpcaoPadrao(callback), { column: 1, dir: orderDir.asc });
    }
    divBusca.AddEvents(GridConsulta);

    divBusca.AddTabPressEvent(function (outraPressionada) {
        if (outraPressionada) {
            knoutOpcoes.Nome.val(knout.val());
        }
        GridConsulta.CarregarGrid(function (lista) {
            if (lista.data.length == 1) {
                callback(lista.data[0]);
            } else {
                divBusca.OpenModal();
            }
        });
    })
}

var BuscarCargaFinalizadasSemAcertoDeViagem = function (knout, callbackRetorno, basicGrid, knoutCodigoAcertoViagem) {

    var idDiv = guid();
    var GridConsulta;

    var multiplaEscolha = false;
    if (basicGrid != null) {
        multiplaEscolha = true;
    }

    var OpcoesKnout = function () {
        this.Titulo = PropertyEntity({ text: "Busca de Cargas para o Acerto de Viagem", type: types.local });
        this.TituloGrid = PropertyEntity({ text: "Cargas", type: types.local });
        this.CodigoCargaEmbarcador = PropertyEntity({ text: "Número da Carga: ", col: 3 });
        this.DataCarga = PropertyEntity({ text: "Data Carga: ", col: 3, getType: typesKnockout.date, visible: true });
        this.Veiculo = PropertyEntity({ col: 6, type: types.entity, codEntity: ko.observable(0), text: "Veículo:", idBtnSearch: guid(), visible: true });
        this.Situacao = PropertyEntity({ text: "Situacao: ", col: 12, visible: false, val: ko.observable(11) });
        this.AcertoViagem = PropertyEntity({ col: 12, type: types.entity, codEntity: ko.observable(0), text: "Acerto de Viagem:", idBtnSearch: guid(), visible: false });;
        this.Pesquisar = PropertyEntity({
            eventClick: function (e) {
                GridConsulta.CarregarGrid();
            }, type: types.event, text: "Pesquisar", visible: true
        });
    }

    var knoutOpcoes = new OpcoesKnout();
    var funcaoParamentroDinamico = null;

    if (knoutCodigoAcertoViagem != null) {
        knoutOpcoes.AcertoViagem.visible = false;
        funcaoParamentroDinamico = function () {
            knoutOpcoes.AcertoViagem.codEntity(knoutCodigoAcertoViagem.val());
            knoutOpcoes.AcertoViagem.val(knoutCodigoAcertoViagem.val());
        }
    }

    var divBusca = new CriarBusca(idDiv, knoutOpcoes, knout, funcaoParamentroDinamico, null, multiplaEscolha, function () {
        new BuscarVeiculos(knoutOpcoes.Veiculo);
    });

    var callback = function (e) {
        preecherRetornoSelecao(knout, e, idDiv, knoutOpcoes);
    };

    if (callbackRetorno != null) {
        callback = function (e) {
            divBusca.CloseModal();
            callbackRetorno(e);
        };
    }

    if (multiplaEscolha) {
        var objetoBasicGrid = { basicGrid: basicGrid, callback: callbackRetorno, eventos: knoutOpcoes.Pesquisar };
        GridConsulta = new GridView(idDiv + "_tabelaEntidades", "Carga/PesquisaCargaFinalizadasSemAcertoDeViagem", knoutOpcoes, null, null, null, null, null, null, objetoBasicGrid);
    } else {
        GridConsulta = new GridView(idDiv + "_tabelaEntidades", "Carga/PesquisaCargaFinalizadasSemAcertoDeViagem", knoutOpcoes, divBusca.OpcaoPadrao(callback), { column: 1, dir: orderDir.asc });
    }
    divBusca.AddEvents(GridConsulta);

    divBusca.AddTabPressEvent(function (outraPressionada) {
        if (outraPressionada) {
            knoutOpcoes.Nome.val(knout.val());
        }
        GridConsulta.CarregarGrid(function (lista) {
            if (lista.data.length == 1) {
                callback(lista.data[0]);
            } else {
                divBusca.OpenModal();
            }
        });
    })
}

var BuscarCargas = function (knout, callbackRetorno, tipoIntegracao, tipoStatus, possuiDTNatura, tipoStatusDiff, basicGrid, knoutTipoOcorrencia, permiteCTeComplementar, cargasNaoAgrupadas, cargasNaoFechadas, cargasDisponiveisParaJanela, knoutCentroDescarregamento, quantidadePorPagina, situacaoPesquisaCarga, telaRedespacho, telaCancelamento, telaPagamento, somenteNaoFechadas, telaPagametoProvedor) {
    var integracoes = tipoIntegracao != null ? JSON.stringify([].concat(tipoIntegracao)) : "";
    var status = tipoStatus != null ? JSON.stringify([].concat(tipoStatus)) : "";
    var statusDiff = tipoStatusDiff != null ? JSON.stringify([].concat(tipoStatusDiff)) : "";
    let visibilidadeCamposPorSitaucaoCarga = true;

    var idDiv = guid();
    var GridConsulta;
    var multiplaEscolha = (basicGrid != null);

    if (cargasNaoAgrupadas == null)
        cargasNaoAgrupadas = false;

    if (cargasNaoFechadas == null)
        cargasNaoFechadas = false;

    if (telaRedespacho == null)
        telaRedespacho = false;
        
    if (telaPagametoProvedor == null)
        telaPagametoProvedor = false;

    if (telaCancelamento == null)
        telaCancelamento = false;

    if (telaPagamento == null)
        telaPagamento = false;

    if (cargasDisponiveisParaJanela == null)
        cargasDisponiveisParaJanela = false;

    if (situacaoPesquisaCarga)
        visibilidadeCamposPorSitaucaoCarga = false;

    if (somenteNaoFechadas == null)
        somenteNaoFechadas = false;

    var OpcoesKnout = function () {
        this.Titulo = PropertyEntity({ text: Localization.Resources.Consultas.Carga.BuscarCargas, type: types.local });
        this.TituloGrid = PropertyEntity({ text: Localization.Resources.Consultas.Carga.Cargas, type: types.local });

        this.NumeroCarga = PropertyEntity({ text: Localization.Resources.Consultas.Carga.NumeroDaCarga.getFieldDescription(), col: 2, visible: true });
        this.NumeroNF = PropertyEntity({ text: Localization.Resources.Consultas.Carga.NumeroDaNotaFiscal.getFieldDescription(), getType: typesKnockout.int, col: 2, visible: !IsMobile() && visibilidadeCamposPorSitaucaoCarga });
        this.NumeroCTeSubcontratacao = PropertyEntity({ text: Localization.Resources.Consultas.Carga.NumeroDoCteParaSubcontratacao.getFieldDescription(), getType: typesKnockout.int, col: 2, visible: !IsMobile() && visibilidadeCamposPorSitaucaoCarga });
        this.NumeroCTe = PropertyEntity({ text: Localization.Resources.Consultas.Carga.NumeroDoCteNfse.getFieldDescription(), getType: typesKnockout.int, col: 2, visible: visibilidadeCamposPorSitaucaoCarga });
        this.NumeroMDFe = PropertyEntity({ text: Localization.Resources.Consultas.Carga.NumeroDoMdfe.getFieldDescription(), getType: typesKnockout.int, col: 2, visible: !IsMobile() && visibilidadeCamposPorSitaucaoCarga });
        this.NumeroPedidoEmbarcador = PropertyEntity({ text: Localization.Resources.Consultas.Carga.NumeroDoPedidoNoEmbarcador.getFieldDescription(), getType: typesKnockout.string, col: 2, visible: !IsMobile() });
        this.DataInicialEmissao = PropertyEntity({ text: Localization.Resources.Consultas.Carga.DataInicialEmissao.getFieldDescription(), col: 2, getType: typesKnockout.date, visible: !IsMobile() && visibilidadeCamposPorSitaucaoCarga });
        this.DataFinalEmissao = PropertyEntity({ text: Localization.Resources.Consultas.Carga.DataFinalEmissao.getFieldDescription(), col: 2, getType: typesKnockout.date, visible: !IsMobile() && visibilidadeCamposPorSitaucaoCarga });
        this.Veiculo = PropertyEntity({ col: !visibilidadeCamposPorSitaucaoCarga ? 3 : 4, type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Consultas.Carga.Veiculo.getFieldDescription(), idBtnSearch: guid(), visible: true });
        this.Motorista = PropertyEntity({ col: !visibilidadeCamposPorSitaucaoCarga ? 3 : 4, type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Consultas.Carga.Motorista.getFieldDescription(), idBtnSearch: guid(), visible: true });
        this.NumeroFrota = PropertyEntity({ text: Localization.Resources.Consultas.Carga.NumeroFrota.getFieldDescription(), col: 2, visible: true });
        this.NumeroContainer = PropertyEntity({ text: Localization.Resources.Consultas.Carga.NumeroContainer.getFieldDescription(), col: 2, visible: true });
        let opcoesSituacaoCarga = _CONFIGURACAO_TMS.TipoServicoMultisoftware === EnumTipoServicoMultisoftware.MultiTMS ? EnumSituacoesCarga.obterOpcoesPesquisaTMS() : EnumSituacoesCarga.obterOpcoesEmbarcador();
        this.Situacao = PropertyEntity({ col: 2, val: ko.observable((situacaoPesquisaCarga ? [].concat(situacaoPesquisaCarga) : new Array())), def: (situacaoPesquisaCarga ? [].concat(situacaoPesquisaCarga) : new Array()), getType: typesKnockout.selectMultiple, options: opcoesSituacaoCarga, text: Localization.Resources.Gerais.Geral.Situacao.getFieldDescription(), visible: visibilidadeCamposPorSitaucaoCarga });
        this.NumeroPedidoCliente = PropertyEntity({ text: Localization.Resources.Consultas.Carga.NumeroPedidoCliente.getFieldDescription(), col: 2, visible: true });

        this.DataInicialEmissao.dateRangeLimit = this.DataFinalEmissao;
        this.DataFinalEmissao.dateRangeInit = this.DataInicialEmissao;

        this.Pesquisar = PropertyEntity({
            eventClick: function (e) {
                GridConsulta.CarregarGrid();
            }, type: types.event, text: Localization.Resources.Gerais.Geral.Pesquisar, visible: true
        });

        this.BuscaAvancada = PropertyEntity({
            eventClick: function (e) {
                if (e.BuscaAvancada.visibleFade()) {
                    e.BuscaAvancada.visibleFade(false);
                    e.BuscaAvancada.icon("fal fa-plus");
                } else {
                    e.BuscaAvancada.visibleFade(true);
                    e.BuscaAvancada.icon("fal fa-minus");
                }
            }, buscaAvancada: true, type: types.event, text: Localization.Resources.Gerais.Geral.Avancada, idFade: guid(), cssClass: "btn btn-default", icon: ko.observable("fal fa-plus"), visibleFade: ko.observable(false), visible: true
        });

        this.Filial = PropertyEntity({ col: 4, type: types.multiplesEntities, codEntity: ko.observable(0), text: Localization.Resources.Consultas.Carga.Filial.getFieldDescription(), idBtnSearch: guid(), visible: _CONFIGURACAO_TMS.TipoServicoMultisoftware != EnumTipoServicoMultisoftware.MultiTMS ? true : false });

        this.GrupoPessoas = PropertyEntity({ col: 4, type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Consultas.Carga.GrupoDePessoas.getFieldDescription(), idBtnSearch: guid(), visible: true });

        this.Remetente = PropertyEntity({ col: 4, type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Consultas.Carga.Remetente.getFieldDescription(), idBtnSearch: guid(), visible: true });
        this.Expedidor = PropertyEntity({ col: 4, type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Consultas.Carga.Expedidor.getFieldDescription(), idBtnSearch: guid(), visible: true });
        this.Recebedor = PropertyEntity({ col: 4, type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Consultas.Carga.Recebedor.getFieldDescription(), idBtnSearch: guid(), visible: true });
        this.Destinatario = PropertyEntity({ col: 4, type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Consultas.Carga.Destinatario.getFieldDescription(), idBtnSearch: guid(), visible: true });
        this.Origem = PropertyEntity({ col: 4, type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Consultas.Carga.Origem.getFieldDescription(), idBtnSearch: guid(), visible: true });
        this.Destino = PropertyEntity({ col: 4, type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Consultas.Carga.Destino.getFieldDescription(), idBtnSearch: guid(), visible: true });
        this.CentroDescarregamento = PropertyEntity({ col: 8, type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Consultas.Carga.CentroDeDescarregamento.getFieldDescription(), idBtnSearch: guid(), visible: false });

        this.Empresa = PropertyEntity({
            col: 4, type: types.multiplesEntities, codEntity: ko.observable(0), text: (_CONFIGURACAO_TMS.TipoServicoMultisoftware == EnumTipoServicoMultisoftware.MultiTMS ? Localization.Resources.Consultas.Carga.EmpresaFilial.getFieldDescription() : Localization.Resources.Consultas.Carga.Transportador.getFieldDescription()), idBtnSearch: guid(), visible: _CONFIGURACAO_TMS.TipoServicoMultisoftware != EnumTipoServicoMultisoftware.MultiCTe && _CONFIGURACAO_TMS.TipoServicoMultisoftware != EnumTipoServicoMultisoftware.Terceiros ? true : false
        });
        this.TipoOperacao = PropertyEntity({ col: 4, type: types.multiplesEntities, codEntity: ko.observable(0), text: Localization.Resources.Consultas.Carga.TipoDeOperacao.getFieldDescription(), idBtnSearch: guid(), visible: true });
        this.CargaTransbordo = PropertyEntity({ col: 2, text: Localization.Resources.Consultas.Carga.CargaDeTransbordo.getFieldDescription(), val: ko.observable(""), options: Global.ObterOpcoesPesquisaBooleano(Localization.Resources.Gerais.Geral.Sim, Localization.Resources.Gerais.Geral.Nao), def: false, visible: true });
        this.CargaNaoFechadas = PropertyEntity({ col: 2, text: Localization.Resources.Consultas.Carga.RetornarCargasNaoFechadas.getFieldDescription(), val: ko.observable(false), options: Global.ObterOpcoesBooleano(Localization.Resources.Gerais.Geral.Sim, Localization.Resources.Gerais.Geral.Nao), def: false, visible: cargasNaoFechadas });

        this.TipoIntegracao = PropertyEntity({ val: ko.observable(integracoes), def: integracoes, visible: false });
        this.Situacao = PropertyEntity({ val: ko.observable(status), def: status, visible: false });
        this.CargasNaoAgrupadas = PropertyEntity({ val: ko.observable(cargasNaoAgrupadas), def: status, visible: false });
        this.CargasDisponiveisParaJanela = PropertyEntity({ val: ko.observable(cargasDisponiveisParaJanela), def: status, visible: false });
        this.TelaRedespacho = PropertyEntity({ val: ko.observable(telaRedespacho), def: telaRedespacho, visible: false });
        this.TelaPagametoProvedor = PropertyEntity({ val: ko.observable(telaPagametoProvedor), def: telaPagametoProvedor, visible: false });
        this.TelaCancelamento = PropertyEntity({ val: ko.observable(telaCancelamento), def: telaCancelamento, visible: false });
        this.TelaPagamento = PropertyEntity({ val: ko.observable(telaPagamento), def: telaPagamento, visible: false });

        this.StatusDiff = PropertyEntity({ val: ko.observable(statusDiff), def: statusDiff, visible: false });
        this.PossuiDTNatura = PropertyEntity({ val: ko.observable(possuiDTNatura), def: possuiDTNatura, visible: false });
        this.PermiteCTeComplementar = PropertyEntity({ val: ko.observable(permiteCTeComplementar), def: permiteCTeComplementar, visible: false });
        this.TipoOcorrencia = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Consultas.Carga.TipoDeOcorrencia.getFieldDescription(), idBtnSearch: guid(), issue: 0, visible: ko.observable(false) });

        this.DataInicialCarga = PropertyEntity({ col: 2, text: Localization.Resources.Consultas.Carga.DataInicialCarga.getFieldDescription(), col: 2, getType: typesKnockout.date, visible: !IsMobile() });
        this.DataFinalCarga = PropertyEntity({ col: 2, text: Localization.Resources.Consultas.Carga.DataFinalCarga.getFieldDescription(), col: 2, getType: typesKnockout.date, visible: !IsMobile() });

        this.DataInicialCarga.dateRangeLimit = this.DataFinalCarga;
        this.DataFinalCarga.dateRangeInit = this.DataInicialCarga;    

        this.SomenteNaoFechadas = PropertyEntity({ val: ko.observable(somenteNaoFechadas), def: somenteNaoFechadas, visible: false });
    };

    var knoutOpcoes = new OpcoesKnout();

    var funcaoParamentroDinamico = function () {
        if (knoutTipoOcorrencia != null) {
            knoutOpcoes.TipoOcorrencia.codEntity(knoutTipoOcorrencia.codEntity());
            knoutOpcoes.TipoOcorrencia.val(knoutTipoOcorrencia.val());
        }
        if (knoutCentroDescarregamento != null) {
            knoutOpcoes.CentroDescarregamento.codEntity(knoutCentroDescarregamento.codEntity());
            knoutOpcoes.CentroDescarregamento.val(knoutCentroDescarregamento.val());
        }
    };

    var divBusca = new CriarBusca(idDiv, knoutOpcoes, knout, funcaoParamentroDinamico, null, multiplaEscolha, function () {
        new BuscarTransportadores(knoutOpcoes.Empresa);
        new BuscarVeiculos(knoutOpcoes.Veiculo, null, knoutOpcoes.Empresa);
        new BuscarMotoristas(knoutOpcoes.Motorista, null, knoutOpcoes.Empresa);
        new BuscarFilial(knoutOpcoes.Filial);
        new BuscarGruposPessoas(knoutOpcoes.GrupoPessoas);
        new BuscarClientes(knoutOpcoes.Remetente);
        new BuscarClientes(knoutOpcoes.Expedidor);
        new BuscarClientes(knoutOpcoes.Recebedor);
        new BuscarClientes(knoutOpcoes.Destinatario);
        new BuscarLocalidades(knoutOpcoes.Origem);
        new BuscarLocalidades(knoutOpcoes.Destino);
        new BuscarTiposOperacao(knoutOpcoes.TipoOperacao);
    }, null, true);

    var callback = function (e) {
        knout.codEntity(e.Codigo);
        knout.val(e.CodigoCargaEmbarcador);
        knoutOpcoes.NumeroCarga.val(knoutOpcoes.NumeroCarga.def);
        divBusca.CloseModal();
        Global.setarFocoProximoCampo(knout.id);
    };

    if (callbackRetorno != null) {
        callback = function (e) {
            divBusca.CloseModal();

            if (multiplaEscolha)
                $("#" + idDiv + "_btnConfirmarMultiplaEscolha").attr("disabled", "disabled");

            callbackRetorno(e);
            Global.setarFocoProximoCampo(knout.id);
        };
    }

    if (multiplaEscolha) {
        var objetoBasicGrid = { basicGrid: basicGrid, callback: callbackRetorno, eventos: knoutOpcoes.Pesquisar };
        GridConsulta = new GridView(idDiv + "_tabelaEntidades", "Carga/Pesquisa", knoutOpcoes, null, null, quantidadePorPagina, null, null, null, objetoBasicGrid);
    } else {
        GridConsulta = new GridView(idDiv + "_tabelaEntidades", "Carga/Pesquisa", knoutOpcoes, divBusca.OpcaoPadrao(callback, 22), null, quantidadePorPagina);
    }

    divBusca.AddEvents(GridConsulta);

    divBusca.AddTabPressEvent(function (outraPressionada) {
        if (outraPressionada)
            knoutOpcoes.NumeroCarga.val(knout.val());

        GridConsulta.CarregarGrid(function (lista) {
            if (lista.data.length == 1)
                callback(lista.data[0]);
            else
                divBusca.OpenModal();
        });
    });
};

var BuscarCargaFinalizadasSemFatura = function (knout, callbackRetorno, knoutFatura) {

    var idDiv = guid();
    var GridConsulta;

    var OpcoesKnout = function () {
        this.CodigoCargaEmbarcador = PropertyEntity({ text: "Número da Carga: ", col: 12 });
        this.Titulo = PropertyEntity({ text: "Buscar Cargas Finalizadas", type: types.local });
        this.TituloGrid = PropertyEntity({ text: "Cargas", type: types.local });
        this.CodigoCargaEmbarcador = PropertyEntity({ text: "Número da Carga: ", col: 3 });
        this.NumeroDocumento = PropertyEntity({ text: "Número do Documento: ", col: 3, getType: typesKnockout.int });
        this.DataCarga = PropertyEntity({ text: "Data Carga: ", col: 3, getType: typesKnockout.date, visible: true });
        this.Situacao = PropertyEntity({ text: "Situação: ", col: 12, visible: false, val: ko.observable(11) });
        this.Fatura = PropertyEntity({ col: 12, type: types.entity, codEntity: ko.observable(0), text: "Fatura:", idBtnSearch: guid(), visible: false });;
        this.Pesquisar = PropertyEntity({
            eventClick: function (e) {
                GridConsulta.CarregarGrid();
            }, type: types.event, text: "Pesquisar", visible: true
        });
    }

    var knoutOpcoes = new OpcoesKnout();
    var funcaoParamentroDinamico = null;

    if (knoutFatura != null) {
        knoutOpcoes.Fatura.visible = false;
        funcaoParamentroDinamico = function () {
            knoutOpcoes.Fatura.codEntity(knoutFatura.Codigo.val());
            knoutOpcoes.Fatura.val(knoutFatura.Codigo.val());
        }
    }

    var divBusca = new CriarBusca(idDiv, knoutOpcoes, knout, funcaoParamentroDinamico);
    //KoBindings(knoutOpcoes, idDiv, false);
    knoutOpcoes.Situacao.val(11);

    var callback = function (e) {
        knout.codEntity(e.Codigo);
        knout.val(e.OrigemDestino);
        knoutOpcoes.CodigoCargaEmbarcador.val(knoutOpcoes.CodigoCargaEmbarcador.def);
        divBusca.CloseModal();
        Global.setarFocoProximoCampo(knout.id);
    };

    if (callbackRetorno != null) {
        callback = function (e) {
            divBusca.CloseModal();
            callbackRetorno(e);
        }
    }

    GridConsulta = new GridView(idDiv + "_tabelaEntidades", "FaturaCarga/PesquisaCargasSemFatura", knoutOpcoes, divBusca.OpcaoPadrao(callback), null);
    divBusca.AddEvents(GridConsulta);

    divBusca.AddTabPressEvent(function (outraPressionada) {
        if (outraPressionada) {
            knoutOpcoes.CodigoCargaEmbarcador.val(knout.val());
        }
        GridConsulta.CarregarGrid(function (lista) {

            if (lista.data.length == 1) {
                callback(lista.data[0]);
            } else {
                divBusca.OpenModal();
            }
        });
    })
}

var BuscarCargaAgrupada = function (knout, callbackRetorno) {
    var idDiv = guid();
    var gridConsulta;

    var OpcoesKnout = function () {
        this.NumeroCarga = PropertyEntity({ text: "Número da Carga: ", col: 12 });
        this.Titulo = PropertyEntity({ text: "Buscar Cargas Agrupadas", type: types.local });
        this.TituloGrid = PropertyEntity({ text: "Cargas", type: types.local });
        this.Pesquisar = PropertyEntity({
            eventClick: function (e) {
                gridConsulta.CarregarGrid();
            }, type: types.event, text: "Pesquisar", visible: true
        });
    }

    var knoutOpcoes = new OpcoesKnout();
    var divBusca = new CriarBusca(idDiv, knoutOpcoes, knout);

    var callback = function (e) {
        divBusca.DefCallback(e);
    }

    if (callbackRetorno != null) {
        callback = function (e) {
            divBusca.CloseModal();
            callbackRetorno(e);
        }
    }

    gridConsulta = new GridView(idDiv + "_tabelaEntidades", "CargaAgrupada/BuscarCargasAgrupadas", knoutOpcoes, divBusca.OpcaoPadrao(callback), { column: 0, dir: orderDir.desc });

    divBusca.AddEvents(gridConsulta);

    divBusca.AddTabPressEvent(function (outraPressionada) {
        if (outraPressionada) {
            knoutOpcoes.NumeroCarga.val(knout.val());
        }
        gridConsulta.CarregarGrid(function (lista) {
            if (lista.data.length == 1) {
                callback(lista.data[0]);
            } else {
                divBusca.OpenModal();
            }
        });
    });
}

var BuscarCargaAvarias = function (knout, callbackRetorno) {
    var idDiv = guid();
    var GridConsulta;

    var OpcoesKnout = function () {
        this.CodigoCargaEmbarcador = PropertyEntity({ text: "Número da Carga: ", col: 12 });
        this.Titulo = PropertyEntity({ text: "Consultar Cargas", type: types.local });
        this.TituloGrid = PropertyEntity({ text: "Cargas", type: types.local });
        this.Situacao = PropertyEntity({ text: "Situação: ", col: 12, visible: false, val: ko.observable(11) });
        this.Pesquisar = PropertyEntity({
            eventClick: function (e) {
                GridConsulta.CarregarGrid();
            }, type: types.event, text: "Pesquisar", visible: true
        });
    }

    var knoutOpcoes = new OpcoesKnout();

    var divBusca = new CriarBusca(idDiv, knoutOpcoes, knout);

    knoutOpcoes.Situacao.val(11);

    var callback = function (e) {
        knout.codEntity(e.Codigo);
        knout.val(e.CodigoCargaEmbarcador);
        divBusca.CloseModal();
    }

    if (callbackRetorno != null) {
        callback = function (e) {
            divBusca.CloseModal();
            callbackRetorno(e);
        }
    }


    GridConsulta = new GridView(idDiv + "_tabelaEntidades", "Carga/PesquisaCargasAvarias", knoutOpcoes, divBusca.OpcaoPadrao(callback), null, null, null, null, null, null, 10);
    divBusca.AddEvents(GridConsulta);

    divBusca.AddTabPressEvent(function (outraPressionada) {
        if (outraPressionada) {
            knoutOpcoes.CodigoCargaEmbarcador.val(knout.val());
        }
        GridConsulta.CarregarGrid(function (lista) {

            if (lista.data.length == 1) {
                callback(lista.data[0]);
            } else {
                divBusca.OpenModal();
            }
        });
    })
}

var BuscarCargasFinalizadasAmbiente = function (knout, callbackRetorno, ordernarPorFaturamento) {
    var idDiv = guid();
    var GridConsulta;

    var OpcoesKnout = function () {
        this.Codigo = PropertyEntity({ text: "Número da Carga: ", col: 3 });
        this.Transportador = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Transportador:", idBtnSearch: guid(), visible: true, col: 9 });
        this.Titulo = PropertyEntity({ text: "Consultar Cargas", type: types.local });
        this.TituloGrid = PropertyEntity({ text: "Cargas", type: types.local });
        this.Pesquisar = PropertyEntity({
            eventClick: function (e) {
                GridConsulta.CarregarGrid();
            }, type: types.event, text: "Pesquisar", visible: true
        });
    }

    var knoutOpcoes = new OpcoesKnout();

    if (ordernarPorFaturamento == null)
        ordernarPorFaturamento = false;

    if (EnumTipoServicoMultisoftware == EnumTipoServicoMultisoftware.MultiCTe)
        knoutOpcoes.Transportador.visible = false;

    var divBusca = new CriarBusca(idDiv, knoutOpcoes, knout, null, null, null, function () {
        new BuscarTransportadores(knoutOpcoes.Transportador);
    }, true);
    var callback = function (e) {
        knout.codEntity(e.Codigo);
        knout.val(e.CodigoCargaEmbarcador);
        divBusca.CloseModal();
    }

    if (callbackRetorno != null) {
        callback = function (e) {
            divBusca.CloseModal();
            callbackRetorno(e);
        }
    }

    var ordenacaoGrid = null;
    if (ordernarPorFaturamento) {
        ordenacaoGrid = { column: 2, dir: orderDir.desc };
    }

    GridConsulta = new GridView(idDiv + "_tabelaEntidades", "Carga/PesquisaCargasFinalizadasAmbiente", knoutOpcoes, divBusca.OpcaoPadrao(callback), ordenacaoGrid, null, null, null, null, null, 10);
    divBusca.AddEvents(GridConsulta);

    divBusca.AddTabPressEvent(function (outraPressionada) {
        if (outraPressionada) {
            knoutOpcoes.Codigo.val(knout.val());
        }
        GridConsulta.CarregarGrid(function (lista) {

            if (lista.data.length == 1) {
                callback(lista.data[0]);
            } else {
                divBusca.OpenModal();
            }
        });
    })
}

var BuscarCargasParaChamados = function (knout, callbackRetorno, ordernarPorFaturamento) {
    var idDiv = guid();
    var GridConsulta;

    var OpcoesKnout = function () {
        this.Codigo = PropertyEntity({ text: "Número da Carga: ", col: 3 });
        this.Transportador = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Transportador:", idBtnSearch: guid(), visible: true, col: 9 });
        this.Titulo = PropertyEntity({ text: "Consultar Cargas", type: types.local });
        this.TituloGrid = PropertyEntity({ text: "Cargas", type: types.local });
        this.Pesquisar = PropertyEntity({
            eventClick: function (e) {
                GridConsulta.CarregarGrid();
            }, type: types.event, text: "Pesquisar", visible: true
        });
    }

    var knoutOpcoes = new OpcoesKnout();

    if (ordernarPorFaturamento == null)
        ordernarPorFaturamento = false;

    if (EnumTipoServicoMultisoftware == EnumTipoServicoMultisoftware.MultiCTe)
        knoutOpcoes.Transportador.visible = false;

    var divBusca = new CriarBusca(idDiv, knoutOpcoes, knout, null, null, null, function () {
        new BuscarTransportadores(knoutOpcoes.Transportador);
    }, true);
    var callback = function (e) {
        knout.codEntity(e.Codigo);
        knout.val(e.CodigoCargaEmbarcador);
        divBusca.CloseModal();
    }

    if (callbackRetorno != null) {
        callback = function (e) {
            divBusca.CloseModal();
            callbackRetorno(e);
        }
    }

    var ordenacaoGrid = null;
    if (ordernarPorFaturamento) {
        ordenacaoGrid = { column: 2, dir: orderDir.desc };
    }

    GridConsulta = new GridView(idDiv + "_tabelaEntidades", "Carga/BuscarCargasParaChamados", knoutOpcoes, divBusca.OpcaoPadrao(callback), ordenacaoGrid, null, null, null, null, null, 10);
    divBusca.AddEvents(GridConsulta);

    divBusca.AddTabPressEvent(function (outraPressionada) {
        if (outraPressionada) {
            knoutOpcoes.Codigo.val(knout.val());
        }
        GridConsulta.CarregarGrid(function (lista) {

            if (lista.data.length == 1) {
                callback(lista.data[0]);
            } else {
                divBusca.OpenModal();
            }
        });
    })
}

var BuscarCargasPermiteMDFeManual = function (knout, knoutEmpresa, callbackRetorno, basicGrid) {

    var idDiv = guid();
    var GridConsulta;

    var multiplaEscolha = false;
    if (basicGrid != null) {
        multiplaEscolha = true;
    }

    var situacoesPesquisa = [
        EnumSituacoesCarga.Encerrada,
        EnumSituacoesCarga.EmTransporte,
        EnumSituacoesCarga.AgIntegracao
    ];

    var coluna = 4;
    var visibleCarregamento = false;
    if (_CONFIGURACAO_TMS.TipoServicoMultisoftware != EnumTipoServicoMultisoftware.MultiCTe) {
        coluna = 3;
        visibleCarregamento = true;
    }

    var OpcoesKnout = function () {
        var dataAtual = moment().add(-1, 'days').format("DD/MM/YYYY");

        this.Titulo = PropertyEntity({ text: "Buscar Cargas para MDF-e Manual", type: types.local });
        this.TituloGrid = PropertyEntity({ text: "Cargas", type: types.local });

        this.CodigoCargaEmbarcador = PropertyEntity({ text: "Número da Carga: ", col: coluna });
        this.Carregamento = PropertyEntity({ type: types.entity, col: coluna, codEntity: ko.observable(0), visible: visibleCarregamento, text: "Carregamento:", idBtnSearch: guid(), enable: ko.observable(false) });
        this.DataInicio = PropertyEntity({ text: "Data Inicial: ", col: coluna, getType: typesKnockout.date, visible: true, val: ko.observable(dataAtual) });
        this.DataFim = PropertyEntity({ text: "Data Final: ", col: coluna, getType: typesKnockout.date, visible: true });


        this.DataInicio.dateRangeLimit = this.DataFim;
        this.DataFim.dateRangeInit = this.DataInicio;

        this.Pesquisar = PropertyEntity({
            eventClick: function (e) {
                GridConsulta.CarregarGrid();
            }, type: types.event, text: "Pesquisar", visible: true
        });

        this.BuscaAvancada = PropertyEntity({
            eventClick: function (e) {
                if (e.BuscaAvancada.visibleFade() == true) {
                    e.BuscaAvancada.visibleFade(false);
                    e.BuscaAvancada.icon("fal fa-plus");
                } else {
                    e.BuscaAvancada.visibleFade(true);
                    e.BuscaAvancada.icon("fal fa-minus");
                }
            }, buscaAvancada: true, type: types.event, text: "Avançada", idFade: guid(), cssClass: "btn btn-default", icon: ko.observable("fal fa-plus"), visibleFade: ko.observable(false), visible: true
        });

        this.Filial = PropertyEntity({ col: 6, type: types.multiplesEntities, codEntity: ko.observable(0), text: "Filial:", idBtnSearch: guid(), visible: _CONFIGURACAO_TMS.TipoServicoMultisoftware != EnumTipoServicoMultisoftware.MultiTMS ? true : false });
        this.NumeroNF = PropertyEntity({ text: "Número da NF-e: ", col: 2 });
        this.NumeroCTe = PropertyEntity({ text: "Número do CT-e: ", col: 2 });
        this.NumeroMDFe = PropertyEntity({ text: "Número do MDF-e: ", col: 2 });
        this.Origem = PropertyEntity({ col: 6, type: types.entity, codEntity: ko.observable(0), text: "Origem:", idBtnSearch: guid(), visible: true });
        this.Destino = PropertyEntity({ col: 6, type: types.entity, codEntity: ko.observable(0), text: "Destino:", idBtnSearch: guid(), visible: true });
        this.Veiculo = PropertyEntity({ col: 6, type: types.entity, codEntity: ko.observable(0), text: "Veículo:", idBtnSearch: guid(), visible: true });
        this.Motorista = PropertyEntity({ col: _CONFIGURACAO_TMS.TipoServicoMultisoftware != EnumTipoServicoMultisoftware.MultiTMS ? 6 : 12, type: types.entity, codEntity: ko.observable(0), text: "Motorista:", idBtnSearch: guid(), visible: true });

        this.Empresa = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Transportador:", idBtnSearch: guid(), visible: false });
        this.Operador = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Operador:", visible: false });
        this.Situacao = PropertyEntity({ text: "Situacao: ", col: 12, visible: false, val: ko.observable(0) });
        this.Situacoes = PropertyEntity({ getType: typesKnockout.dynamic, val: ko.observable(JSON.stringify(situacoesPesquisa)), visible: false });
        this.SomentePermiteMDFeManual = PropertyEntity({ type: types.bool, val: ko.observable(true), def: true, visible: false });
    }

    var knoutOpcoes = new OpcoesKnout();
    var funcaoParamentroDinamico = null;
    if (knoutEmpresa != null) {
        knoutOpcoes.Empresa.visible = false;
        funcaoParamentroDinamico = function () {
            knoutOpcoes.Empresa.codEntity(knoutEmpresa.codEntity());
            knoutOpcoes.Empresa.val(knoutEmpresa.val());
        }
    }

    var divBusca = new CriarBusca(idDiv, knoutOpcoes, knout, funcaoParamentroDinamico, null, multiplaEscolha, function () {
        new BuscarTransportadores(knoutOpcoes.Empresa);
        new BuscarVeiculos(knoutOpcoes.Veiculo, null, knoutOpcoes.Empresa);
        new BuscarMotoristas(knoutOpcoes.Motorista, null, knoutOpcoes.Empresa);
        new BuscarFilial(knoutOpcoes.Filial, null);
        new BuscarLocalidades(knoutOpcoes.Origem, null);
        new BuscarLocalidades(knoutOpcoes.Destino, null);
        new BuscarCarregamento(knoutOpcoes.Carregamento, null, EnumSituacaoCarregamento.Fechado, knoutEmpresa, EnumTipoMontagemCarga.AgruparCargas);
    }, null, true);

    var callback = function (e) {
        knout.codEntity(e.Codigo);
        knout.val(e.OrigemDestino);
        knoutOpcoes.CodigoCargaEmbarcador.val(knoutOpcoes.CodigoCargaEmbarcador.def);
        divBusca.CloseModal();
        Global.setarFocoProximoCampo(knout.id);
    };

    if (callbackRetorno != null) {
        callback = function (e) {
            divBusca.CloseModal();
            if (multiplaEscolha)
                $("#" + idDiv + "_btnConfirmarMultiplaEscolha").attr("disabled", "disabled");
            callbackRetorno(e);
        }
    }

    if (multiplaEscolha) {
        var objetoBasicGrid = { basicGrid: basicGrid, callback: callbackRetorno, eventos: knoutOpcoes.Pesquisar };
        GridConsulta = new GridView(idDiv + "_tabelaEntidades", "Carga/PesquisaCargasNaGrid", knoutOpcoes, null, null, null, null, null, null, objetoBasicGrid);
    } else {
        GridConsulta = new GridView(idDiv + "_tabelaEntidades", "Carga/PesquisaCargasNaGrid", knoutOpcoes, divBusca.OpcaoPadrao(callback), null);
    }
    divBusca.AddEvents(GridConsulta);

    divBusca.AddTabPressEvent(function (outraPressionada) {
        if (outraPressionada) {
            knoutOpcoes.CodigoCargaEmbarcador.val(knout.val());
        }
        GridConsulta.CarregarGrid(function (lista) {

            if (lista.data.length == 1) {
                callback(lista.data[0]);
            } else {
                divBusca.OpenModal();
            }
        });
    })
}

var BuscarCargaParaPreCarga = function (knout, callbackRetorno, knoutRemetentes, knoutDestinatarios, knoutFilial, knoutTransportador) {

    var idDiv = guid();
    var GridConsulta;

    var OpcoesKnout = function () {
        this.Titulo = PropertyEntity({ text: "Buscar Cargas", type: types.local });
        this.TituloGrid = PropertyEntity({ text: "Cargas", type: types.local });
        this.CodigoCargaEmbarcador = PropertyEntity({ text: "Número da Carga: ", col: 4 });
        this.DataInicial = PropertyEntity({ text: "Data Carga Inicial: ", col: 4, getType: typesKnockout.date, visible: true });
        this.DataFinal = PropertyEntity({ text: "Data Carga Final: ", col: 4, getType: typesKnockout.date, visible: true });
        this.Filial = PropertyEntity({ col: 12, type: types.entity, codEntity: ko.observable(0), text: "Filial", visible: true });

        this.Remetentes = PropertyEntity({ col: 0, type: types.entity, codEntity: ko.observable(0), visible: false });
        this.Destinatarios = PropertyEntity({ col: 0, type: types.entity, codEntity: ko.observable(0), visible: false });
        this.Pesquisar = PropertyEntity({
            eventClick: function (e) {
                GridConsulta.CarregarGrid();
            }, type: types.event, text: "Pesquisar", visible: true
        });
    }

    var knoutOpcoes = new OpcoesKnout();
    var funcaoParamentroDinamico = null;
    var arrayOpcoes = [];

    if (knoutRemetentes != null) {
        arrayOpcoes.push({ knout: knoutRemetentes, prop: "Remetentes" });
    }

    if (knoutDestinatarios != null) {
        arrayOpcoes.push({ knout: knoutDestinatarios, prop: "Destinatarios" });
    }

    if (knoutFilial != null) {
        knoutOpcoes.Filial.visible = false;
        arrayOpcoes.push({ knout: knoutFilial, prop: "Filial" });
    }

    if (knoutTransportador != null) {
        arrayOpcoes.push({ knout: knoutTransportador, prop: "Transportador" });
    }

    if (arrayOpcoes.length > 0) {
        funcaoParamentroDinamico = function () {
            arrayOpcoes.forEach(function (opt) {
                if (opt.knout.type == types.entity)
                    knoutOpcoes[opt.prop].codEntity(opt.knout.codEntity());
                else
                    knoutOpcoes[opt.prop].codEntity(opt.knout.val());
                knoutOpcoes[opt.prop].val(opt.knout.val());
            });
        }
    }

    var divBusca = new CriarBusca(idDiv, knoutOpcoes, knout, funcaoParamentroDinamico, null, null, function () {
        new BuscarFilial(knoutOpcoes.Filial);
    });

    var callback = function (e) {
        knout.codEntity(e.Codigo);
        knout.val(e.Descricao);

        divBusca.CloseModal();
        Global.setarFocoProximoCampo(knout.id);
    };

    if (callbackRetorno != null) {
        callback = function (e) {
            divBusca.CloseModal();
            Global.setarFocoProximoCampo(knout.id);
            callbackRetorno(e);
        }
    }

    GridConsulta = new GridView(idDiv + "_tabelaEntidades", "PreCarga/PesquisaCargasParaVinculo", knoutOpcoes, divBusca.OpcaoPadrao(callback), null);
    divBusca.AddEvents(GridConsulta);

    divBusca.AddTabPressEvent(function (outraPressionada) {
        if (outraPressionada) {
            knoutOpcoes.CodigoCargaEmbarcador.val(knout.val());
        }
        GridConsulta.CarregarGrid(function (lista) {

            if (lista.data.length == 1) {
                callback(lista.data[0]);
            } else {
                divBusca.OpenModal();
            }
        });
    })
}

var BuscarCargaParaTrocaNota = function (knout, callbackRetorno) {
    var idDiv = guid();
    var GridConsulta;

    var OpcoesKnout = function () {
        this.Titulo = PropertyEntity({ text: "Consultar Cargas", type: types.local });
        this.TituloGrid = PropertyEntity({ text: "Cargas", type: types.local });
        this.CodigoCargaEmbarcador = PropertyEntity({ text: "Número da Carga: ", col: 12 });

        this.Pesquisar = PropertyEntity({
            eventClick: function (e) {
                GridConsulta.CarregarGrid();
            }, type: types.event, text: "Pesquisar", visible: true
        });
    }

    var knoutOpcoes = new OpcoesKnout();
    var divBusca = new CriarBusca(idDiv, knoutOpcoes, knout);

    var callback = function (e) {
        knout.codEntity(e.Codigo);
        knout.val(e.CodigoCargaEmbarcador);
        divBusca.CloseModal();
    }

    if (callbackRetorno != null) {
        callback = function (e) {
            divBusca.CloseModal();
            callbackRetorno(e);
        }
    }

    GridConsulta = new GridView(idDiv + "_tabelaEntidades", "Carga/PesquisaCargasParaTrocaNota", knoutOpcoes, divBusca.OpcaoPadrao(callback), null, null, null, null, null, null, 10);
    divBusca.AddEvents(GridConsulta);

    divBusca.AddTabPressEvent(function (outraPressionada) {
        if (outraPressionada) {
            knoutOpcoes.CodigoCargaEmbarcador.val(knout.val());
        }
        GridConsulta.CarregarGrid(function (lista) {
            if (lista.data.length == 1) {
                callback(lista.data[0]);
            } else {
                divBusca.OpenModal();
            }
        });
    })
}

var BuscarCargaParaFilaCarregamento = function (knout, callbackRetorno, basicGrid, knoutFilial, knoutModeloVeicularCarga) {
    var idDiv = guid();
    var GridConsulta;
    var multiplaEscolha = (basicGrid != null);

    var OpcoesKnout = function () {
        this.Titulo = PropertyEntity({ text: "Buscar Cargas", type: types.local });
        this.TituloGrid = PropertyEntity({ text: "Cargas", type: types.local });
        this.CodigoCargaEmbarcador = PropertyEntity({ text: "Número da Carga: ", col: 4 });
        this.DataInicial = PropertyEntity({ text: "Data Carga Inicial: ", col: 4, getType: typesKnockout.date, visible: true });
        this.DataFinal = PropertyEntity({ text: "Data Carga Final: ", col: 4, getType: typesKnockout.date, visible: true });
        this.Filial = PropertyEntity({ col: 12, type: types.entity, codEntity: ko.observable(0), text: "Filial", visible: true });
        this.ModeloVeicularCarga = PropertyEntity({ col: 12, type: types.entity, codEntity: ko.observable(0), text: "Modelo Veicular", visible: true });

        this.Pesquisar = PropertyEntity({ eventClick: function (e) { GridConsulta.CarregarGrid(); }, type: types.event, text: "Pesquisar", visible: true });
    }

    var knoutOpcoes = new OpcoesKnout();
    var funcaoParamentroDinamico = null;
    var arrayOpcoes = [];

    if (knoutFilial != null) {
        knoutOpcoes.Filial.visible = false;
        arrayOpcoes.push({ knout: knoutFilial, prop: "Filial" });
    }

    if (knoutModeloVeicularCarga != null) {
        knoutOpcoes.ModeloVeicularCarga.visible = false;
        arrayOpcoes.push({ knout: knoutModeloVeicularCarga, prop: "ModeloVeicularCarga" });
    }

    if (arrayOpcoes.length > 0) {
        funcaoParamentroDinamico = function () {
            arrayOpcoes.forEach(function (opt) {
                knoutOpcoes[opt.prop].codEntity((opt.knout.type == types.entity ? opt.knout.codEntity() : opt.knout.val()));
                knoutOpcoes[opt.prop].val(opt.knout.val());
            });
        }
    }

    var divBusca = new CriarBusca(idDiv, knoutOpcoes, knout, funcaoParamentroDinamico, null, multiplaEscolha, function () {
        new BuscarFilial(knoutOpcoes.Filial);
        new BuscarModelosVeicularesCarga(knoutOpcoes.ModeloVeicularCarga);
    });

    var callback = function (e) {
        knout.codEntity(e.Codigo);
        knout.val(e.CodigoCargaEmbarcador);

        divBusca.CloseModal();
        Global.setarFocoProximoCampo(knout.id);
    };

    if (callbackRetorno != null) {
        callback = function (e) {
            divBusca.CloseModal();
            Global.setarFocoProximoCampo(knout.id);
            callbackRetorno(e);
        }
    }

    if (multiplaEscolha) {
        var objetoBasicGrid = { basicGrid: basicGrid, callback: callbackRetorno, eventos: knoutOpcoes.Pesquisar };
        GridConsulta = new GridView(idDiv + "_tabelaEntidades", "Carga/PesquisaCargasParaFilaCarregamento", knoutOpcoes, null, null, null, null, null, null, objetoBasicGrid);
    }
    else
        GridConsulta = new GridView(idDiv + "_tabelaEntidades", "Carga/PesquisaCargasParaFilaCarregamento", knoutOpcoes, divBusca.OpcaoPadrao(callback), null);

    divBusca.AddEvents(GridConsulta);

    divBusca.AddTabPressEvent(function (outraPressionada) {
        if (outraPressionada)
            knoutOpcoes.CodigoCargaEmbarcador.val(knout.val());

        GridConsulta.CarregarGrid(function (lista) {
            if (lista.data.length == 1)
                callback(lista.data[0]);
            else
                divBusca.OpenModal();
        });
    })
}

var BuscarCargaLiberadasGuarita = function (knout, callbackRetorno) {

    var idDiv = guid();
    var GridConsulta;

    var OpcoesKnout = function () {
        this.CodigoCargaEmbarcador = PropertyEntity({ text: "Número da Carga: ", col: 12 });
        this.Titulo = PropertyEntity({ text: "Buscar Cargas Liberadas pela Guarita", type: types.local });
        this.TituloGrid = PropertyEntity({ text: "Cargas", type: types.local });
        this.Pesquisar = PropertyEntity({
            eventClick: function (e) {
                GridConsulta.CarregarGrid();
            }, type: types.event, text: "Pesquisar", visible: true
        });
    }

    var knoutOpcoes = new OpcoesKnout();

    var divBusca = new CriarBusca(idDiv, knoutOpcoes, knout);
    //var callback = function (e) {
    //    divBusca.DefCallback(e);
    //}
    var callback = function (e) {
        knout.codEntity(e.Codigo);
        knout.val(e.OrigemDestino);
        knoutOpcoes.CodigoCargaEmbarcador.val(knoutOpcoes.CodigoCargaEmbarcador.def);
        divBusca.CloseModal();
        Global.setarFocoProximoCampo(knout.id);
    };

    if (callbackRetorno != null) {
        callback = function (e) {
            divBusca.CloseModal();
            callbackRetorno(e);
        }
    }


    GridConsulta = new GridView(idDiv + "_tabelaEntidades", "Carga/PesquisaCargasLiberadasGuarita", knoutOpcoes, divBusca.OpcaoPadrao(callback), null, null, null, null, null, null, 10);
    divBusca.AddEvents(GridConsulta);

    divBusca.AddTabPressEvent(function (outraPressionada) {
        if (outraPressionada) {
            knoutOpcoes.CodigoCargaEmbarcador.val(knout.val());
        }
        GridConsulta.CarregarGrid(function (lista) {

            if (lista.data.length == 1) {
                callback(lista.data[0]);
            } else {
                divBusca.OpenModal();
            }
        });
    })
}

var BuscarCargaEXPRetiradaContainer = function (knout, callbackRetorno, knoutContainerTipo) {
    var idDiv = guid();
    var gridConsulta;

    var OpcoesKnout = function () {
        this.Titulo = PropertyEntity({ text: "Buscar Carga EXP para retirada de Container", type: types.local });
        this.TituloGrid = PropertyEntity({ text: "Carga EXP para retirada de Container", type: types.local });

        this.CodigoCargaEmbarcador = PropertyEntity({ text: "Número da Carga: ", col: 4 });
        this.ContainerTipo = PropertyEntity({ col: 12, type: types.entity, codEntity: ko.observable(0), text: "Tipo do Container", idBtnSearch: guid(), visible: true, enable: false });

        this.Pesquisar = PropertyEntity({ eventClick: function (e) { gridConsulta.CarregarGrid(); }, type: types.event, text: "Pesquisar" });
    }

    var knoutOpcoes = new OpcoesKnout();

    var funcaoParametroDinamico = function () {
        if (knoutContainerTipo) {
            knoutOpcoes.ContainerTipo.codEntity(knoutContainerTipo.codEntity());
            knoutOpcoes.ContainerTipo.val(knoutContainerTipo.val());
        }
    }

    var divBusca = new CriarBusca(idDiv, knoutOpcoes, knout, funcaoParametroDinamico, null, null, function () {
        new BuscarTiposContainer(knoutOpcoes.ContainerTipo);
    });

    var callback = function (e) {
        knout.codEntity(e.Codigo);
        knout.val(e.OrigemDestino);
        knoutOpcoes.CodigoCargaEmbarcador.val(knoutOpcoes.CodigoCargaEmbarcador.def);
        divBusca.CloseModal();
        Global.setarFocoProximoCampo(knout.id);
    };

    if (callbackRetorno != null) {
        callback = function (e) {
            divBusca.CloseModal();
            callbackRetorno(e);
        }
    }

    gridConsulta = new GridView(idDiv + "_tabelaEntidades", "Carga/PesquisaCargasLiberadaRetiradaContainer", knoutOpcoes, divBusca.OpcaoPadrao(callback), null);

    divBusca.AddEvents(gridConsulta);

    divBusca.AddTabPressEvent(function (outraPressionada) {
        if (outraPressionada)
            knoutOpcoes.CodigoCargaEmbarcador.val(knout.val());

        gridConsulta.CarregarGrid(function (lista) {
            if (lista.data.length == 1) {
                callback(lista.data[0]);
                return;
            }

            divBusca.OpenModal();
        });
    });

    this.abrirBusca = function () {
        LimparCampos(knoutOpcoes);

        divBusca.UpdateGrid();
        divBusca.OpenModal();
    };
}

var BuscarCargasCargaRelacionada = function (knout, callbackRetorno, tipoIntegracao, basicGrid, knoutTipoOcorrencia, quantidadePorPagina) {
    var integracoes = tipoIntegracao != null ? JSON.stringify([].concat(tipoIntegracao)) : "";

    var idDiv = guid();
    var GridConsulta;
    var multiplaEscolha = (basicGrid != null);

    var OpcoesKnout = function () {
        this.Titulo = PropertyEntity({ text: Localization.Resources.Consultas.Carga.BuscarCargas, type: types.local });
        this.TituloGrid = PropertyEntity({ text: Localization.Resources.Consultas.Carga.Cargas, type: types.local });

        this.NumeroCarga = PropertyEntity({ text: Localization.Resources.Consultas.Carga.NumeroDaCarga.getFieldDescription(), col: 2, visible: true });

        this.DataInicialCarga = PropertyEntity({ col: 2, text: Localization.Resources.Consultas.Carga.DataInicialCarga.getFieldDescription(), col: 2, getType: typesKnockout.date, visible: true });
        this.DataFinalCarga = PropertyEntity({ col: 2, text: Localization.Resources.Consultas.Carga.DataFinalCarga.getFieldDescription(), col: 2, getType: typesKnockout.date, visible: true });

        this.DataInicialCarga.dateRangeLimit = this.DataFinalCarga;
        this.DataFinalCarga.dateRangeInit = this.DataInicialCarga;

        this.Filial = PropertyEntity({ col: 4, type: types.multiplesEntities, codEntity: ko.observable(0), text: Localization.Resources.Consultas.Carga.Filial.getFieldDescription(), idBtnSearch: guid(), visible: true });

        var opcoesSituacaoCarga = _CONFIGURACAO_TMS.TipoServicoMultisoftware === EnumTipoServicoMultisoftware.MultiTMS ? EnumSituacoesCarga.obterOpcoesPesquisaTMS() : EnumSituacoesCarga.obterOpcoesEmbarcador();
        this.Situacao = PropertyEntity({ col: 2, val: ko.observable(new Array()), def: new Array(), getType: typesKnockout.selectMultiple, options: opcoesSituacaoCarga, text: Localization.Resources.Gerais.Geral.Situacao.getFieldDescription(), visible: true });

        this.TipoCarga = PropertyEntity({ col: 4, text: "Tipo de Carga:", type: types.entity, codEntity: ko.observable(0), idBtnSearch: guid(), visible: true });
        this.TipoOperacao = PropertyEntity({ col: 4, text: "Tipo de Operação:", type: types.entity, codEntity: ko.observable(0), idBtnSearch: guid(), visible: true });
        this.CanalEntrega = PropertyEntity({ col: 4, text: "Canal de Entrega:", type: types.entity, codEntity: ko.observable(0), idBtnSearch: guid(), visible: true });
        this.Empresa = PropertyEntity({ col: 12, text: "Transportador:", type: types.entity, codEntity: ko.observable(0), idBtnSearch: guid(), visible: true });

        this.TipoIntegracao = PropertyEntity({ val: ko.observable(integracoes), def: integracoes, visible: false });

        this.Pesquisar = PropertyEntity({
            eventClick: function (e) {
                GridConsulta.CarregarGrid();
            }, type: types.event, text: Localization.Resources.Gerais.Geral.Pesquisar, visible: true
        });
    };

    var knoutOpcoes = new OpcoesKnout();

    var funcaoParamentroDinamico = function () {
        if (knoutTipoOcorrencia != null) {
            knoutOpcoes.TipoOcorrencia.codEntity(knoutTipoOcorrencia.codEntity());
            knoutOpcoes.TipoOcorrencia.val(knoutTipoOcorrencia.val());
        }
    };

    var divBusca = new CriarBusca(idDiv, knoutOpcoes, knout, funcaoParamentroDinamico, null, multiplaEscolha, function () {
        new BuscarTransportadores(knoutOpcoes.Empresa);
        new BuscarFilial(knoutOpcoes.Filial);
        new BuscarTiposOperacao(knoutOpcoes.TipoOperacao);
        new BuscarTiposdeCarga(knoutOpcoes.TipoCarga);
        new BuscarCanaisEntrega(knoutOpcoes.CanalEntrega);
    }, null, true);

    var callback = function (e) {
        knout.codEntity(e.Codigo);
        knout.val(e.CodigoCargaEmbarcador);
        knoutOpcoes.NumeroCarga.val(knoutOpcoes.NumeroCarga.def);
        divBusca.CloseModal();
        Global.setarFocoProximoCampo(knout.id);
    };

    if (callbackRetorno != null) {
        callback = function (e) {
            divBusca.CloseModal();

            if (multiplaEscolha)
                $("#" + idDiv + "_btnConfirmarMultiplaEscolha").attr("disabled", "disabled");

            callbackRetorno(e);
            Global.setarFocoProximoCampo(knout.id);
        };
    }

    if (multiplaEscolha) {
        var objetoBasicGrid = { basicGrid: basicGrid, callback: callbackRetorno, eventos: knoutOpcoes.Pesquisar };
        GridConsulta = new GridView(idDiv + "_tabelaEntidades", "Carga/Pesquisa", knoutOpcoes, null, null, quantidadePorPagina, null, null, null, objetoBasicGrid);
    } else {
        GridConsulta = new GridView(idDiv + "_tabelaEntidades", "Carga/Pesquisa", knoutOpcoes, divBusca.OpcaoPadrao(callback, 22), null, quantidadePorPagina);
    }

    divBusca.AddEvents(GridConsulta);

    divBusca.AddTabPressEvent(function (outraPressionada) {
        if (outraPressionada)
            knoutOpcoes.NumeroCarga.val(knout.val());

        GridConsulta.CarregarGrid(function (lista) {
            if (lista.data.length == 1)
                callback(lista.data[0]);
            else
                divBusca.OpenModal();
        });
    });
};

var BuscarCargaOrganizacao = function (knout, basicGrid, callbackRetorno) {
    let idDiv = guid();
    let GridConsulta;

    let multiplaEscolha = false;
    if (basicGrid != null) {
        multiplaEscolha = true;
    }

    let OpcoesKnout = function () {
        this.Titulo = PropertyEntity({ text: "Buscar Cargas", type: types.local });
        this.TituloGrid = PropertyEntity({ text: "Cargas", type: types.local });
        this.CodigoCargaEmbarcador = PropertyEntity({ text: "Número da Carga: ", col: 6 });
        this.Remetente = PropertyEntity({ col: 4, type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Consultas.Carga.Remetente.getFieldDescription(), idBtnSearch: guid(), visible: true });
        this.Destinatario = PropertyEntity({ col: 4, type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Consultas.Carga.Destinatario.getFieldDescription(), idBtnSearch: guid(), visible: true });
        this.Origem = PropertyEntity({ col: 4, type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Consultas.Carga.Origem.getFieldDescription(), idBtnSearch: guid(), visible: true });
        this.Destino = PropertyEntity({ col: 4, type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Consultas.Carga.Destino.getFieldDescription(), idBtnSearch: guid(), visible: true });

        this.Pesquisar = PropertyEntity({
            eventClick: function (e) {
                GridConsulta.CarregarGrid();
            },
            type: types.event,
            text: Localization.Resources.Gerais.Geral.Pesquisar,
            visible: true
        });
    }

    let knoutOpcoes = new OpcoesKnout();
    let funcaoParamentroDinamico = null;

    let divBusca = new CriarBusca(idDiv, knoutOpcoes, knout, funcaoParamentroDinamico, null, multiplaEscolha, function () {
        BuscarClientes(knoutOpcoes.Remetente);
        BuscarClientes(knoutOpcoes.Destinatario);
        BuscarLocalidades(knoutOpcoes.Origem);
        BuscarLocalidades(knoutOpcoes.Destino);
    });

    let callback = function (e) {
        knout.codEntity(e.Codigo);
        knout.val(e.CodigoCargaEmbarcador);

        divBusca.CloseModal();
        Global.setarFocoProximoCampo(knout.id);
    };

    if (callbackRetorno != null) {
        callback = function (e) {
            divBusca.CloseModal();
            Global.setarFocoProximoCampo(knout.id);
            callbackRetorno(e);
        }
    }
    if (multiplaEscolha) {
        const objetoBasicGrid = { basicGrid: basicGrid, callback: callbackRetorno, eventos: knoutOpcoes.Pesquisar };
        GridConsulta = new GridView(idDiv + "_tabelaEntidades", "Carga/PesquisaCargaCarOrganizacao", knoutOpcoes, null, null, null, null, null, null, objetoBasicGrid);
    } else {
        GridConsulta = new GridView(idDiv + "_tabelaEntidades", "Carga/PesquisaCargaCarOrganizacao", knoutOpcoes, divBusca.OpcaoPadrao(callback), null);
    }
    divBusca.AddEvents(GridConsulta);

    divBusca.AddTabPressEvent(function (outraPressionada) {
        if (outraPressionada) {
            knoutOpcoes.CodigoCargaEmbarcador.val(knout.val());
        }
        GridConsulta.CarregarGrid(function (lista) {
            if (lista.data.length == 1) {
                callback(lista.data[0]);
            } else {
                divBusca.OpenModal();
            }
        });
    })
}

var BuscarCargasFinalizadasPelaOSMaeOuOS = function (knout, cargaAtual, callbackRetorno) {
    var idDiv = guid();
    var gridConsulta;

    var OpcoesKnout = function () {
        this.Titulo = PropertyEntity({ text: "Buscar Cargas", type: types.local });
        this.TituloGrid = PropertyEntity({ text: "Cargas", type: types.local });
        this.CargaAtual = PropertyEntity({ val: ko.observable(cargaAtual), def: cargaAtual, visible: false });
        this.NumeroCarga = PropertyEntity({ text: "Número da Carga: ", col: 12 });

        this.Pesquisar = PropertyEntity({
            eventClick: function (e) {
                gridConsulta.CarregarGrid();
            }, type: types.event, text: "Pesquisar", visible: true
        });
    }

    var knoutOpcoes = new OpcoesKnout();
    var divBusca = new CriarBusca(idDiv, knoutOpcoes, knout);

    var callback = function (e) {
        divBusca.DefCallback(e);
    }

    if (callbackRetorno != null) {
        callback = function (e) {
            divBusca.CloseModal();
            callbackRetorno(e);
        }
    }

    gridConsulta = new GridView(idDiv + "_tabelaEntidades", "Carga/PesquisaCargasFinalizadasPelaOSMaeOuOS", knoutOpcoes, divBusca.OpcaoPadrao(callback), { column: 0, dir: orderDir.desc });

    divBusca.AddEvents(gridConsulta);

    divBusca.AddTabPressEvent(function (outraPressionada) {
        if (outraPressionada) {
            knoutOpcoes.NumeroCarga.val(knout.val());
        }
        gridConsulta.CarregarGrid(function (lista) {
            if (lista.data.length == 1) {
                callback(lista.data[0]);
            } else {
                divBusca.OpenModal();
            }
        });
    });
}