/// <reference path="../../js/Global/Buscas.js" />
/// <reference path="../../js/Global/Grid.js" />
/// <reference path="../../js/Global/CRUD.js" />
/// <reference path="../../js/libs/jquery-2.1.1.js" />
/// <reference path="Tranportador.js" />
/// <reference path="Cliente.js" />
/// <reference path="../Enumeradores/EnumSituacaoColaborador.js" />
/// <reference path="../../js/Global/Validacao.js" />

var _sitaucaoMotorista = [{ text: "Todos", value: EnumSituacaoMotorista.Todos }, { text: "Ativo", value: EnumSituacaoMotorista.Ativo }, { text: "Inativo", value: EnumSituacaoMotorista.Inativos }];
var BuscarMotoristasPorStatus = function (knout, callbackRetorno, knoutTransportador, basicGrid, situacaoColaborador) {

    var idDiv = guid();
    var GridConsulta;

    var multiplaEscolha = false;
    if (basicGrid != null) {
        multiplaEscolha = true;
    }

    var _situacaoColaborador = EnumSituacaoColaborador.Todos;
    if (situacaoColaborador != null)
        _situacaoColaborador = situacaoColaborador;
    else
        _situacaoColaborador = EnumSituacaoColaborador.Todos;

    var OpcoesKnout = function () {

        var visibleEmpresa = false;
        if (_CONFIGURACAO_TMS.TipoServicoMultisoftware === EnumTipoServicoMultisoftware.MultiEmbarcador) {
            visibleEmpresa = true;
        }

        this.Titulo = PropertyEntity({ text: "Busca de Motoristas", type: types.local });
        this.TituloGrid = PropertyEntity({ text: "Motoristas", type: types.local });
        this.Empresa = PropertyEntity({ col: 8, type: types.entity, codEntity: ko.observable(0), visible: visibleEmpresa, text: "Transportador:", idBtnSearch: guid() });
        this.Nome = PropertyEntity({ col: (visibleEmpresa ? 8 : 4), text: "Nome: " });
        this.CPF = PropertyEntity({ col: 3, text: "CPF: ", maxlength: 14, getType: _CONFIGURACAO_TMS.PermitirCadastrarMotoristaEstrangeiro ? typesKnockout.string : typesKnockout.cpf });
        this.Placa = PropertyEntity({ col: 3, text: "Placa do Veículo: " });
        this.Ativo = PropertyEntity({ col: 2, val: ko.observable(true), options: _statusPesquisa, def: true, text: "Situação: ", visible: true });
        this.SituacaoColaborador = PropertyEntity({ col: 0, visible: false, val: ko.observable(_situacaoColaborador) });

        this.Pesquisar = PropertyEntity({
            eventClick: function (e) {
                GridConsulta.CarregarGrid();
            }, type: types.event, text: "Pesquisar", visible: true
        });
    }

    var knoutOpcoes = new OpcoesKnout();
    var funcaoParamentroDinamico = null;
    if (knoutTransportador != null) {
        knoutOpcoes.Empresa.visible = false;
        funcaoParamentroDinamico = function () {
            knoutOpcoes.Empresa.codEntity(knoutTransportador.codEntity());
            knoutOpcoes.Empresa.val(knoutTransportador.val());
        }
    }
    var divBusca = new CriarBusca(idDiv, knoutOpcoes, knout, funcaoParamentroDinamico, null, multiplaEscolha, function () {
        new BuscarTransportadores(knoutOpcoes.Empresa);
    });

    var callback = function (e) {
        knout.codEntity(e.Codigo);
        knout.val(e.Nome);
        knoutOpcoes.Empresa.codEntity(knoutOpcoes.Empresa.defCodEntity);
        knoutOpcoes.Empresa.val(knoutOpcoes.Empresa.def);
        knoutOpcoes.Nome.val(knoutOpcoes.Nome.def);
        knoutOpcoes.CPF.val(knoutOpcoes.CPF.def);
        knout.requiredClass("form-control");

        divBusca.CloseModal();
        Global.setarFocoProximoCampo(knout.id);
    };

    if (callbackRetorno != null) {
        callback = function (e) {
            divBusca.CloseModal();
            callbackRetorno(e);
        };
    }

    if (multiplaEscolha) {
        var objetoBasicGrid = { basicGrid: basicGrid, callback: callbackRetorno, eventos: knoutOpcoes.Pesquisar };
        GridConsulta = new GridView(idDiv + "_tabelaEntidades", "Motorista/Pesquisa", knoutOpcoes, null, null, null, null, null, null, objetoBasicGrid);
    } else {
        GridConsulta = new GridView(idDiv + "_tabelaEntidades", "Motorista/Pesquisa", knoutOpcoes, divBusca.OpcaoPadrao(callback), null);
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

var BuscarMotoristas = function (knout, callbackRetorno, knoutTransportador, basicGrid, apenasAtivos, situacaoColaborador, exibirTipoMotorista, fnCallbackRemetenteLocalCarregamentoAutorizado, apenasNaoBloqueados, apenasNaoAjudantes, desabilitarMultiplaEscolha) {

    var idDiv = guid();
    var GridConsulta;
    var buscaTransportadores;
    var buscaClientes;

    var multiplaEscolha = false;
    if (basicGrid != null && !desabilitarMultiplaEscolha) {
        multiplaEscolha = true;
    }
    var _apenasAtivos = "";
    if (apenasAtivos != null)
        _apenasAtivos = "1";

    var _situacaoColaborador = EnumSituacaoColaborador.Todos;
    if (situacaoColaborador != null)
        _situacaoColaborador = situacaoColaborador;
    else
        _situacaoColaborador = EnumSituacaoColaborador.Todos;

    var _apenasNaoBloqueados = false;
    if (apenasNaoBloqueados != null)
        _apenasNaoBloqueados = apenasNaoBloqueados;

    var _apenasNaoAjudantes = true;
    if (apenasNaoAjudantes != null)
        _apenasNaoAjudantes = apenasNaoAjudantes;

    var OpcoesKnout = function () {

        var visibleEmpresa = false;
        if (_CONFIGURACAO_TMS.TipoServicoMultisoftware == EnumTipoServicoMultisoftware.MultiEmbarcador) {
            visibleEmpresa = true;
        }

        this.Titulo = PropertyEntity({ text: Localization.Resources.Consultas.Motorista.BuscaDeMotoristas, type: types.local });
        this.TituloGrid = PropertyEntity({ text: Localization.Resources.Consultas.Motorista.Motoristas, type: types.local });

        this.Empresa = PropertyEntity({ col: 8, type: types.entity, codEntity: ko.observable(0), visible: visibleEmpresa, text: Localization.Resources.Consultas.Motorista.Transportador.getFieldDescription(), idBtnSearch: guid() });
        this.Nome = PropertyEntity({ col: (visibleEmpresa ? 8 : 4), text: Localization.Resources.Gerais.Geral.Nome.getFieldDescription() });
        this.CPF = PropertyEntity({ col: 4, text: Localization.Resources.Consultas.Motorista.Cpf.getFieldDescription(), maxlength: 14, getType: _CONFIGURACAO_TMS.PermitirCadastrarMotoristaEstrangeiro ? typesKnockout.string : typesKnockout.cpf, visible: !IsMobile() });
        this.Placa = PropertyEntity({ col: 4, text: Localization.Resources.Consultas.Motorista.PlacaDoVeiculo.getFieldDescription(), visible: !IsMobile() });

        //this.Ativo = PropertyEntity({ col: 0, visible: false, val: ko.observable(_apenasAtivos) });        
        this.SituacaoColaborador = PropertyEntity({ col: 0, visible: false, val: ko.observable(_situacaoColaborador) });
        this.Proprietario = PropertyEntity({ col: 6, type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Consultas.Motorista.Terceiro.getFieldDescription(), idBtnSearch: guid(), visible: _CONFIGURACAO_TMS.VisualizarVeiculosPropriosETerceiros });
        this.ExibirTipoMotorista = PropertyEntity({ visible: false, val: ko.observable(exibirTipoMotorista) });

        this.RemetenteLocalCarregamentoAutorizado = PropertyEntity({ visible: ko.observable(false), type: types.entity, codEntity: ko.observable(0) });
        this.NaoBloqueado = PropertyEntity({ visible: false, val: ko.observable(_apenasNaoBloqueados) });
        this.NaoAjudante = PropertyEntity({ visible: false, val: ko.observable(_apenasNaoAjudantes) });
        this.NumeroFrota = PropertyEntity({ col: 4, text: Localization.Resources.Consultas.Motorista.NumeroFrota.getFieldDescription() });
        this.Ativo = PropertyEntity({ col: 2, val: ko.observable(_statusPesquisa.Ativo), options: _statusPesquisa, def: _statusPesquisa.Ativo, text: "Situação: ", visible: true });
        //this.SituacaoMotorista = PropertyEntity({ col: 4, text: "Situação: ", val: ko.observable(EnumSituacaoMotorista.Ativo), options: _sitaucaoMotorista, def: _sitaucaoMotorista.Ativo });
 
        
        this.Pesquisar = PropertyEntity({
            eventClick: function (e) {
                GridConsulta.CarregarGrid();
            }, type: types.event, text: Localization.Resources.Gerais.Geral.Pesquisar, visible: true
        });
    };

    var knoutOpcoes = new OpcoesKnout();
    var funcaoParamentroDinamico = null;

    if (knoutTransportador != null || fnCallbackRemetenteLocalCarregamentoAutorizado instanceof Function) {
        funcaoParamentroDinamico = function () {
            if (knoutTransportador != null) {
                knoutOpcoes.Empresa.visible = false;
                knoutOpcoes.Empresa.codEntity(knoutTransportador.codEntity());
                knoutOpcoes.Empresa.val(knoutTransportador.val());
            }

            if (fnCallbackRemetenteLocalCarregamentoAutorizado instanceof Function) {
                knoutOpcoes.RemetenteLocalCarregamentoAutorizado.codEntity(fnCallbackRemetenteLocalCarregamentoAutorizado());
                knoutOpcoes.RemetenteLocalCarregamentoAutorizado.val(fnCallbackRemetenteLocalCarregamentoAutorizado());
            }
        };
    }

    var divBusca = new CriarBusca(idDiv, knoutOpcoes, knout, funcaoParamentroDinamico, null, multiplaEscolha, function () {
        buscaTransportadores = new BuscarTransportadores(knoutOpcoes.Empresa);
        buscaClientes = new BuscarClientes(knoutOpcoes.Proprietario);
    });

    var callback = function (e) {
        knout.codEntity(e.Codigo);
        knout.val(e.Nome);
        knoutOpcoes.Empresa.codEntity(knoutOpcoes.Empresa.defCodEntity);
        knoutOpcoes.Empresa.val(knoutOpcoes.Empresa.def);
        knoutOpcoes.Nome.val(knoutOpcoes.Nome.def);
        knoutOpcoes.CPF.val(knoutOpcoes.CPF.def);
        knoutOpcoes.NumeroFrota.val(knoutOpcoes.NumeroFrota.def);
        knout.requiredClass("form-control");

        divBusca.CloseModal();
        Global.setarFocoProximoCampo(knout.id);
    };

    if (callbackRetorno != null) {
        callback = function (e) {
            divBusca.CloseModal();
            callbackRetorno(e);
        };
    }

    if (multiplaEscolha) {
        var objetoBasicGrid = { basicGrid: basicGrid, callback: callbackRetorno, eventos: knoutOpcoes.Pesquisar };
        GridConsulta = new GridView(idDiv + "_tabelaEntidades", "Motorista/Pesquisa", knoutOpcoes, null, null, null, null, null, null, objetoBasicGrid);
    } else {
        GridConsulta = new GridView(idDiv + "_tabelaEntidades", "Motorista/Pesquisa", knoutOpcoes, divBusca.OpcaoPadrao(callback, 25), null);
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
    });

    this.Destroy = function () {
        if (buscaTransportadores)
            buscaTransportadores.Destroy();

        if (buscaClientes)
            buscaClientes.Destroy();

        divBusca.Destroy();
    };
};

var BuscarMotoristasPorCPF = function (knout, callbackRetorno, knoutTransportador, basicGrid, situacaoColaborador) {

    var idDiv = guid();
    var GridConsulta;

    var multiplaEscolha = false;
    if (basicGrid != null) {
        multiplaEscolha = true;
    }

    var _situacaoColaborador = EnumSituacaoColaborador.Todos;
    if (situacaoColaborador != null)
        _situacaoColaborador = situacaoColaborador;
    else
        _situacaoColaborador = EnumSituacaoColaborador.Todos;

    var visibleEmpresa = false;
    if (_CONFIGURACAO_TMS.TipoServicoMultisoftware == EnumTipoServicoMultisoftware.MultiEmbarcador) {
        visibleEmpresa = true;
    }

    var OpcoesKnout = function () {
        this.Titulo = PropertyEntity({ text: Localization.Resources.Consultas.Motorista.BuscaDeMotoristas, type: types.local });
        this.TituloGrid = PropertyEntity({ text: Localization.Resources.Consultas.Motorista.Motoristas, type: types.local });
        this.Empresa = PropertyEntity({ col: 12, type: types.entity, codEntity: ko.observable(0), visible: visibleEmpresa, text: Localization.Resources.Consultas.Motorista.Transportador.getFieldDescription(), idBtnSearch: guid() });
        this.CPF = PropertyEntity({ col: 8, text: Localization.Resources.Gerais.Geral.Nome.getFieldDescription() });
        this.Nome = PropertyEntity({ col: 4, text: Localization.Resources.Consultas.Motorista.Cpf.getFieldDescription(), maxlength: 14, getType: _CONFIGURACAO_TMS.PermitirCadastrarMotoristaEstrangeiro ? typesKnockout.string : typesKnockout.cpf });
        this.SituacaoColaborador = PropertyEntity({ col: 0, visible: false, val: ko.observable(_situacaoColaborador) });
        this.Pesquisar = PropertyEntity({
            eventClick: function (e) {
                GridConsulta.CarregarGrid();
            }, type: types.event, text: Localization.Resources.Gerais.Geral.Pesquisar, visible: true
        });
    }

    var knoutOpcoes = new OpcoesKnout();
    var funcaoParamentroDinamico = null;
    if (knoutTransportador != null) {
        knoutOpcoes.Empresa.visible = false;
        funcaoParamentroDinamico = function () {
            knoutOpcoes.Empresa.codEntity(knoutTransportador.codEntity());
            knoutOpcoes.Empresa.val(knoutTransportador.val());
        }
    }
    var divBusca = new CriarBusca(idDiv, knoutOpcoes, knout, funcaoParamentroDinamico, null, multiplaEscolha);


    new BuscarTransportadores(knoutOpcoes.Empresa);

    var callback = function (e) {
        knout.codEntity(e.Codigo);
        knout.val(e.Nome);
        knoutOpcoes.Empresa.codEntity(knoutOpcoes.Empresa.defCodEntity);
        knoutOpcoes.Empresa.val(knoutOpcoes.Empresa.def);
        knoutOpcoes.Nome.val(knoutOpcoes.Nome.def);
        knoutOpcoes.CPF.val(knoutOpcoes.CPF.def);
        knout.requiredClass("form-control");

        divBusca.CloseModal();
        Global.setarFocoProximoCampo(knout.id);
    };

    if (callbackRetorno != null) {
        callback = function (e) {
            divBusca.CloseModal();
            callbackRetorno(e);
        };
    }


    if (multiplaEscolha) {
        var objetoBasicGrid = { basicGrid: basicGrid, callback: callbackRetorno, eventos: knoutOpcoes.Pesquisar };
        GridConsulta = new GridView(idDiv + "_tabelaEntidades", "Motorista/PesquisaPorCPF", knoutOpcoes, null, null, null, null, null, null, objetoBasicGrid);
    } else {
        GridConsulta = new GridView(idDiv + "_tabelaEntidades", "Motorista/PesquisaPorCPF", knoutOpcoes, divBusca.OpcaoPadrao(callback), null);
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

var ValidarMotoristasPorCPF = function (knout, callback) {
    this.Validar = function () {
        if (ValidarCPF(knout.val())) {
            var dados = { cpf: knout.val() };
            executarReST("Motorista/BuscarPorCPF", dados, callback);
        } else {
            exibirMensagem(tipoMensagem.atencao, "CPF Inválido", "CPF Inválido, verifique e tente novamente");
        }
    }
}

var BuscarMotoristasMobile = function (knout, callbackRetorno, knoutCentroCarregamento, knoutModeloVeicularCarga, knoutTransportador, basicGrid, situacaoColaborador) {
    var idDiv = guid();
    var GridConsulta;
    var funcaoParamentroDinamico = null;
    var multiplaEscolha = (basicGrid != null);
    var exibirFiltroEmpresa = (_CONFIGURACAO_TMS.TipoServicoMultisoftware == EnumTipoServicoMultisoftware.MultiEmbarcador);

    var _situacaoColaborador = EnumSituacaoColaborador.Todos;
    if (situacaoColaborador != null)
        _situacaoColaborador = situacaoColaborador;
    else
        _situacaoColaborador = EnumSituacaoColaborador.Todos;

    var OpcoesKnout = function () {
        this.Titulo = PropertyEntity({ text: "Busca de Motoristas", type: types.local });
        this.TituloGrid = PropertyEntity({ text: "Motoristas", type: types.local });
        this.CentroCarregamento = PropertyEntity({ col: 6, type: types.entity, codEntity: ko.observable(0), text: "Centro de Carregamento:", idBtnSearch: guid() });
        this.Empresa = PropertyEntity({ col: 6, type: types.entity, codEntity: ko.observable(0), visible: exibirFiltroEmpresa, text: "Transportador:", idBtnSearch: guid() });
        this.ModeloVeicularCarga = PropertyEntity({ col: 6, type: types.entity, codEntity: ko.observable(0), visible: exibirFiltroEmpresa, text: "Modelo Veicular:", idBtnSearch: guid() });
        this.Nome = PropertyEntity({ col: 6, text: "Nome: " });
        this.CPF = PropertyEntity({ col: 3, text: "CPF: ", maxlength: 14, getType: _CONFIGURACAO_TMS.PermitirCadastrarMotoristaEstrangeiro ? typesKnockout.string : typesKnockout.cpf });
        this.Placa = PropertyEntity({ col: 3, text: "Placa do Veículo: " });
        this.SituacaoColaborador = PropertyEntity({ col: 0, visible: false, val: ko.observable(_situacaoColaborador) });

        this.Pesquisar = PropertyEntity({
            eventClick: function (e) {
                GridConsulta.CarregarGrid();
            }, type: types.event, text: "Pesquisar", visible: true
        });
    }

    var knoutOpcoes = new OpcoesKnout();

    if (knoutCentroCarregamento || knoutTransportador || knoutModeloVeicularCarga) {
        knoutOpcoes.CentroCarregamento.visible = !knoutCentroCarregamento;
        knoutOpcoes.Empresa.visible = !knoutTransportador && exibirFiltroEmpresa;
        knoutOpcoes.ModeloVeicularCarga.visible = !knoutModeloVeicularCarga;

        funcaoParamentroDinamico = function () {
            if (knoutCentroCarregamento) {
                knoutOpcoes.CentroCarregamento.codEntity(knoutCentroCarregamento.codEntity());
                knoutOpcoes.CentroCarregamento.val(knoutCentroCarregamento.val());
            }

            if (knoutTransportador) {
                knoutOpcoes.Empresa.codEntity(knoutTransportador.codEntity());
                knoutOpcoes.Empresa.val(knoutTransportador.val());
            }

            if (knoutModeloVeicularCarga) {
                knoutOpcoes.ModeloVeicularCarga.codEntity(knoutModeloVeicularCarga.codEntity());
                knoutOpcoes.ModeloVeicularCarga.val(knoutModeloVeicularCarga.val());
            }
        }
    }

    var divBusca = new CriarBusca(idDiv, knoutOpcoes, knout, funcaoParamentroDinamico, null, multiplaEscolha, function () {
        new BuscarCentrosCarregamento(knoutOpcoes.CentroCarregamento);
        new BuscarTransportadores(knoutOpcoes.Empresa);
        new BuscarModelosVeicularesCarga(knoutOpcoes.ModeloVeicularCarga);
    });

    var callback = function (e) {
        knout.codEntity(e.Codigo);
        knout.val(e.Nome);
        knoutOpcoes.Empresa.codEntity(knoutOpcoes.Empresa.defCodEntity);
        knoutOpcoes.Empresa.val(knoutOpcoes.Empresa.def);
        knoutOpcoes.Nome.val(knoutOpcoes.Nome.def);
        knoutOpcoes.CPF.val(knoutOpcoes.CPF.def);
        knout.requiredClass("form-control");

        divBusca.CloseModal();
        Global.setarFocoProximoCampo(knout.id);
    };

    if (callbackRetorno != null) {
        callback = function (e) {
            divBusca.CloseModal();
            callbackRetorno(e);
        };
    }

    if (multiplaEscolha) {
        var objetoBasicGrid = { basicGrid: basicGrid, callback: callbackRetorno, eventos: knoutOpcoes.Pesquisar };

        GridConsulta = new GridView(idDiv + "_tabelaEntidades", "Motorista/PesquisaMobile", knoutOpcoes, null, null, null, null, null, null, objetoBasicGrid);
    }
    else
        GridConsulta = new GridView(idDiv + "_tabelaEntidades", "Motorista/PesquisaMobile", knoutOpcoes, divBusca.OpcaoPadrao(callback), null);

    divBusca.AddEvents(GridConsulta);

    divBusca.AddTabPressEvent(function (outraPressionada) {
        if (outraPressionada)
            knoutOpcoes.Nome.val(knout.val());

        GridConsulta.CarregarGrid(function (lista) {
            if (lista.data.length == 1)
                callback(lista.data[0]);
            else
                divBusca.OpenModal();
        });
    })
}

var BuscarMotoristasCarga = function (knout, callbackRetorno, knoutCarga) {
    var idDiv = guid();
    var GridConsulta;

    var OpcoesKnout = function () {
        this.Titulo = PropertyEntity({ text: "Busca de Motoristas", type: types.local });
        this.TituloGrid = PropertyEntity({ text: "Motoristas", type: types.local });

        this.Nome = PropertyEntity({ col: 8, text: "Nome: " });
        this.CPF = PropertyEntity({ col: 4, text: "CPF: ", maxlength: 14, getType: _CONFIGURACAO_TMS.PermitirCadastrarMotoristaEstrangeiro ? typesKnockout.string : typesKnockout.cpf });
        this.Carga = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), required: true, text: "Carga:", idBtnSearch: guid() });

        this.Pesquisar = PropertyEntity({
            eventClick: function (e) {
                GridConsulta.CarregarGrid();
            }, type: types.event, text: "Pesquisar", visible: true
        });
    };

    var knoutOpcoes = new OpcoesKnout();
    var funcaoParamentroDinamico = null;

    if (knoutCarga != null) {
        knoutOpcoes.Carga.visible = false;
        funcaoParamentroDinamico = function () {
            knoutOpcoes.Carga.codEntity(knoutCarga.codEntity());
            knoutOpcoes.Carga.val(knoutCarga.val());
        };
    }

    var divBusca = new CriarBusca(idDiv, knoutOpcoes, knout, funcaoParamentroDinamico);
    var callback = function (e) {
        knout.codEntity(e.Codigo);
        knout.val(e.Descricao);
        divBusca.CloseModal();
    };

    if (callbackRetorno != null) {
        callback = function (e) {
            divBusca.CloseModal();
            callbackRetorno(e);
        };
    }

    GridConsulta = new GridView(idDiv + "_tabelaEntidades", "Motorista/PesquisaMotoristaCarga", knoutOpcoes, divBusca.OpcaoPadrao(callback), null, null, null, null, null, null, 10);
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
    });

    this.CarregarMotoristaCargaSelecionada = function () {
        if (knoutCarga != null) {
            LimparCampos(knoutOpcoes);
            funcaoParamentroDinamico();

            GridConsulta.CarregarGrid(function (lista) {
                if (lista.data.length == 1)
                    callback(lista.data[0]);
                else
                    LimparCampo(knout);
            });
        }
    };

    this.Destroy = function () {
        divBusca.Destroy();
    };
};

var BuscarMotoristasPorCPFPortalRetira = function (knout, callbackRetorno, knoutTransportador, basicGrid) {

    var idDiv = guid();
    var GridConsulta;

    var multiplaEscolha = false;
    if (basicGrid != null) {
        multiplaEscolha = true;
    }

    var OpcoesKnout = function () {
        this.Titulo = PropertyEntity({ text: Localization.Resources.Consultas.Motorista.BuscaDeMotoristas, type: types.local });
        this.TituloGrid = PropertyEntity({ text: Localization.Resources.Consultas.Motorista.Motoristas, type: types.local });
        this.CPF = PropertyEntity({ col: 12, getType: typesKnockout.string, text: Localization.Resources.Consultas.Motorista.Cpf.getFieldDescription() });
        this.Empresa = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Consultas.Motorista.Transportador.getFieldDescription(), idBtnSearch: guid(), visible: false });
        this.Pesquisar = PropertyEntity({
            eventClick: function (e) {
                GridConsulta.CarregarGrid();
            }, type: types.event, text: Localization.Resources.Gerais.Geral.Pesquisar, visible: true
        });
    }

    var knoutOpcoes = new OpcoesKnout();
    var funcaoParamentroDinamico = function () {
        knoutOpcoes.CPF.val(knout.val());
    };

    if (knoutTransportador != null) {
        funcaoParamentroDinamico = function () {
            knoutOpcoes.Empresa.codEntity(knoutTransportador.codEntity());
            knoutOpcoes.Empresa.val(knoutTransportador.val());

            knoutOpcoes.CPF.val(knout.val());
        }
    }

    var divBusca = new CriarBusca(idDiv, knoutOpcoes, knout, funcaoParamentroDinamico, null, multiplaEscolha);

    var callback = function (e) {
        knout.codEntity(e.Codigo);
        knout.val(e.Nome);
        knoutOpcoes.Empresa.codEntity(knoutOpcoes.Empresa.defCodEntity);
        knoutOpcoes.Empresa.val(knoutOpcoes.Empresa.def);
        knoutOpcoes.Nome.val(knoutOpcoes.Nome.def);
        knoutOpcoes.CPF.val(knoutOpcoes.CPF.def);
        knout.requiredClass("form-control");

        divBusca.CloseModal();
        Global.setarFocoProximoCampo(knout.id);
    };

    if (callbackRetorno != null) {
        callback = function (e) {
            divBusca.CloseModal();
            callbackRetorno(e);
        };
    }


    if (multiplaEscolha) {
        var objetoBasicGrid = { basicGrid: basicGrid, callback: callbackRetorno, eventos: knoutOpcoes.Pesquisar };
        GridConsulta = new GridView(idDiv + "_tabelaEntidades", "Motorista/PesquisaPorCPFPortalRetira", knoutOpcoes, null, null, null, null, null, null, objetoBasicGrid);
    } else {
        GridConsulta = new GridView(idDiv + "_tabelaEntidades", "Motorista/PesquisaPorCPFPortalRetira", knoutOpcoes, divBusca.OpcaoPadrao(callback), null);
    }
    divBusca.AddEvents(GridConsulta);

    divBusca.AddTabPressEvent(function (outraPressionada) {
        if (outraPressionada) {
            knoutOpcoes.CPF.val(knout.val());
        }
        GridConsulta.CarregarGrid(function (lista) {
            if (lista.data.length == 1) {
                callback(lista.data[0]);
            } else if (lista.data.length == 0) {
                callback(null);
            } else {
                divBusca.OpenModal();
            }
        });
    })
}