/// <reference path="../../js/Global/Buscas.js" />
/// <reference path="../../js/Global/Grid.js" />
/// <reference path="../../js/Global/CRUD.js" />
/// <reference path="../Enumeradores/EnumSituacaoColaborador.js" />
/// <reference path="../../js/libs/jquery-2.1.1.js" />

var BuscarOperador = function (knout, callbackRetorno, apenasPodeAdicionarComplemento, situacaoColaborador) {

    var idDiv = guid();
    var GridConsulta;

    if (apenasPodeAdicionarComplemento == null)
        apenasPodeAdicionarComplemento = false

    var _situacaoColaborador = EnumSituacaoColaborador.Todos;
    if (situacaoColaborador != null)
        _situacaoColaborador = situacaoColaborador;
    else
        _situacaoColaborador = EnumSituacaoColaborador.Todos;

    var OpcoesKnout = function () {
        this.Titulo = PropertyEntity({ text: Localization.Resources.Consultas.Usuario.BuscadeOperadoresdaLogistica, type: types.local });
        this.TituloGrid = PropertyEntity({ text: Localization.Resources.Consultas.Usuario.OperadoresDeLogistica, type: types.local });
        this.Nome = PropertyEntity({ col: 12, text: Localization.Resources.Consultas.Usuario.Nome.getFieldDescription() });
        this.ApenasPodeAdicionarComplemento = PropertyEntity({ visible: false, getType: typesKnockout.bool, val: ko.observable(apenasPodeAdicionarComplemento), def: apenasPodeAdicionarComplemento });
        this.SituacaoColaborador = PropertyEntity({ col: 0, visible: false, val: ko.observable(_situacaoColaborador) });
        this.Pesquisar = PropertyEntity({
            eventClick: function (e) {
                GridConsulta.CarregarGrid();
            }, type: types.event, text: Localization.Resources.Consultas.Usuario.Pesquisar, visible: true
        });
    };

    var knoutOpcoes = new OpcoesKnout();

    var divBusca = new CriarBusca(idDiv, knoutOpcoes, knout);


    var callback = function (e) {
        knout.codEntity(e.Codigo);
        knout.val(e.Nome);
        knoutOpcoes.Nome.val(knoutOpcoes.Nome.def);
        divBusca.CloseModal();
        Global.setarFocoProximoCampo(knout.id);
    };

    if (callbackRetorno != null) {
        callback = function (e) {
            divBusca.CloseModal();
            callbackRetorno(e);
        };
    }

    GridConsulta = new GridView(idDiv + "_tabelaEntidades", "Usuario/PesquisarOperadores", knoutOpcoes, divBusca.OpcaoPadrao(callback), { column: 0, dir: orderDir.desc });
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
};

var BuscarFuncionario = function (knout, callbackRetorno, basicGrid, retornarTodosTiposAcesso, ignorarSituacaoMotorista, tipoCargoFuncionario, situacaoColaborador, tipoComercial, exibirStatus, esconderCPF) {

    var idDiv = guid();
    var GridConsulta;
    var multiplaEscolha = false;

    var cargos = new Array();
    if (tipoCargoFuncionario != null)
        cargos = [].concat(tipoCargoFuncionario);

    if (exibirStatus == null)
        exibirStatus = false;

    if (basicGrid != null)
        multiplaEscolha = true;

    var _situacaoColaborador = EnumSituacaoColaborador.Todos;
    if (situacaoColaborador != null)
        _situacaoColaborador = situacaoColaborador;
    else
        _situacaoColaborador = EnumSituacaoColaborador.Todos;

    var OpcoesKnout = function () {
        this.Titulo = PropertyEntity({ text: Localization.Resources.Consultas.Funcionario.BuscaDeFuncionarios, type: types.local });
        this.TituloGrid = PropertyEntity({ text: Localization.Resources.Consultas.Funcionario.Funcionarios, type: types.local });

        this.Nome = PropertyEntity({ col: 8, text: Localization.Resources.Gerais.Geral.Nome.getFieldDescription() });
        this.TipoPessoa = PropertyEntity({ col: 4, text: Localization.Resources.Gerais.Geral.Tipo.getFieldDescription(), val: ko.observable(EnumTipoPessoa.Todas), options: EnumTipoPessoa.obterOpcoesPesquisa(false), def: EnumTipoPessoa.Todas, visible: !IsMobile() });
        this.Ativo = PropertyEntity({ col: 4, text: Localization.Resources.Gerais.Geral.Situacao.getFieldDescription(), val: ko.observable(1), options: _statusPesquisa, def: 1, visible: exibirStatus });
        this.SituacaoColaborador = PropertyEntity({ col: 0, visible: false, val: ko.observable(_situacaoColaborador) });
        this.RetornarTodosTiposAcesso = PropertyEntity({ col: 0, visible: false, getType: typesKnockout.bool, val: ko.observable(retornarTodosTiposAcesso == true) });
        this.IgnorarSituacaoMotorista = PropertyEntity({ col: 0, visible: false, getType: typesKnockout.bool, val: ko.observable(ignorarSituacaoMotorista == true) });
        this.TipoComercial = PropertyEntity({ col: 0, visible: false, getType: typesKnockout.int, val: ko.observable(tipoComercial) });
        this.TipoCargoFuncionario = PropertyEntity({ getType: typesKnockout.dynamic, val: ko.observable(JSON.stringify(cargos)), text: Localization.Resources.Consultas.Funcionario.Cargos.getFieldDescription(), visible: false });
        this.EsconderCPF = PropertyEntity({ col: 0, visible: false, getType: typesKnockout.bool, val: ko.observable(esconderCPF) });

        this.Pesquisar = PropertyEntity({
            eventClick: function (e) {
                GridConsulta.CarregarGrid();
            }, type: types.event, text: Localization.Resources.Gerais.Geral.Pesquisar, visible: true
        });
    };

    var knoutOpcoes = new OpcoesKnout();

    var divBusca = new CriarBusca(idDiv, knoutOpcoes, knout, null, null, multiplaEscolha);

    var callback = function (e) {
        knout.codEntity(e.Codigo);
        knout.val(e.Nome);
        knoutOpcoes.Nome.val(knoutOpcoes.Nome.def);
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
        GridConsulta = new GridView(idDiv + "_tabelaEntidades", "Usuario/Pesquisa", knoutOpcoes, null, { column: 0, dir: orderDir.desc }, null, null, null, null, objetoBasicGrid);
    } else
        GridConsulta = new GridView(idDiv + "_tabelaEntidades", "Usuario/Pesquisa", knoutOpcoes, divBusca.OpcaoPadrao(callback, 22), { column: 0, dir: orderDir.desc });

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
};

var BuscarTodosMotorista = function (knout, callbackRetorno, basicGrid, situacaoColaborador) {

    var idDiv = guid();
    var GridConsulta;

    var multiplaEscolha = false;

    if (basicGrid != null)
        multiplaEscolha = true;

    var _situacaoColaborador = EnumSituacaoColaborador.Todos;
    if (situacaoColaborador != null)
        _situacaoColaborador = situacaoColaborador;
    else
        _situacaoColaborador = EnumSituacaoColaborador.Todos;

    var OpcoesKnout = function () {
        this.Titulo = PropertyEntity({ text: Localization.Resources.Consultas.Usuario.BuscaDeMotoristas, type: types.local });
        this.TituloGrid = PropertyEntity({ text: Localization.Resources.Consultas.Usuario.Motoristas, type: types.local });
        this.Nome = PropertyEntity({ col: 8, text: Localization.Resources.Consultas.Usuario.Nome.getFieldDescription() });
        this.CPF = PropertyEntity({ col: 4, text: Localization.Resources.Consultas.Usuario.CPF.getFieldDescription() });
        this.SituacaoColaborador = PropertyEntity({ col: 0, visible: false, val: ko.observable(_situacaoColaborador) });

        this.Pesquisar = PropertyEntity({
            eventClick: function (e) {
                GridConsulta.CarregarGrid();
            }, type: types.event, text: Localization.Resources.Consultas.Usuario.Pesquisar, visible: true
        });
    };

    var knoutOpcoes = new OpcoesKnout();
    var funcaoParamentroDinamico = null;

    var divBusca = new CriarBusca(idDiv, knoutOpcoes, knout, funcaoParamentroDinamico, null, multiplaEscolha);


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
        GridConsulta = new GridView(idDiv + "_tabelaEntidades", "Usuario/PesquisarMotorista", knoutOpcoes, null, null, null, null, null, null, objetoBasicGrid);
    } else {
        GridConsulta = new GridView(idDiv + "_tabelaEntidades", "Usuario/PesquisarMotorista", knoutOpcoes, divBusca.OpcaoPadrao(callback), { column: 1, dir: orderDir.asc });
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
};

var BuscarMotorista = function (knout, callbackRetorno, basicGrid, ativo, situacaoColaborador, configuracaoTMS) {
    var idDiv = guid();
    var GridConsulta;

    var multiplaEscolha = false;

    if (basicGrid != null)
        multiplaEscolha = true;

    if (ativo == null)
        ativo = true;

    var _situacaoColaborador = EnumSituacaoColaborador.Todos;
    if (situacaoColaborador != null)
        _situacaoColaborador = situacaoColaborador;
    else
        _situacaoColaborador = EnumSituacaoColaborador.Todos;

    var OpcoesKnout = function () {
        this.Titulo = PropertyEntity({ text: Localization.Resources.Consultas.Usuario.BuscaDeMotoristas, type: types.local });
        this.TituloGrid = PropertyEntity({ text: Localization.Resources.Consultas.Usuario.Motoristas, type: types.local });
        this.Nome = PropertyEntity({ col: 6, text: Localization.Resources.Consultas.Usuario.Nome.getFieldDescription() });
        this.CPF = PropertyEntity({ col: 4, text: Localization.Resources.Consultas.Usuario.CPF.getFieldDescription() });
        this.Ativo = PropertyEntity({ col: 2, val: ko.observable(ativo), options: _status, def: true, text: Localization.Resources.Gerais.Geral.Situacao.getFieldDescription(), visible: true });
        this.SituacaoColaborador = PropertyEntity({ col: 0, visible: false, val: ko.observable(_situacaoColaborador) });

        this.Pesquisar = PropertyEntity({
            eventClick: function (e) {
                GridConsulta.CarregarGrid();
            }, type: types.event, text: Localization.Resources.Consultas.Usuario.Pesquisar, visible: true
        });
    };

    var knoutOpcoes = new OpcoesKnout();
    var funcaoParamentroDinamico = null;

    var divBusca = new CriarBusca(idDiv, knoutOpcoes, knout, funcaoParamentroDinamico, null, multiplaEscolha);


    var callback = function (e) {
        knout.codEntity(e.Codigo);
        knout.val(e.Nome);
        divBusca.CloseModal();
        Global.setarFocoProximoCampo(knout.id);
    };

    if (callbackRetorno != null) {
        callback = function (e) {
            callbackRetorno(e);
            divBusca.CloseModal();
            Global.setarFocoProximoCampo(knout.id);
        };
    }

    if (configuracaoTMS)
        GridConsulta = new GridView(idDiv + "_tabelaEntidades", "Usuario/PesquisarMotoristaProprio", knoutOpcoes, divBusca.OpcaoPadrao(callback), { column: 1, dir: orderDir.asc });
    else {
        if (multiplaEscolha) {
            var objetoBasicGrid = { basicGrid: basicGrid, callback: callbackRetorno, eventos: knoutOpcoes.Pesquisar };
            GridConsulta = new GridView(idDiv + "_tabelaEntidades", "Usuario/PesquisarMotorista", knoutOpcoes, null, null, null, null, null, null, objetoBasicGrid);
        } else {
            GridConsulta = new GridView(idDiv + "_tabelaEntidades", "Usuario/PesquisarMotorista", knoutOpcoes, divBusca.OpcaoPadrao(callback), { column: 1, dir: orderDir.asc });
        }
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
};

var BuscarUsuarioTerceiro = function (knout, callbackRetorno, situacaoColaborador) {

    var idDiv = guid();
    var GridConsulta;

    var _situacaoColaborador = EnumSituacaoColaborador.Todos;
    if (situacaoColaborador != null)
        _situacaoColaborador = situacaoColaborador;
    else
        _situacaoColaborador = EnumSituacaoColaborador.Todos;

    var OpcoesKnout = function () {
        this.Titulo = PropertyEntity({ text: Localization.Resources.Consultas.Usuario.BuscaDeTerceiros, type: types.local });
        this.TituloGrid = PropertyEntity({ text: Localization.Resources.Consultas.Usuario.Terceiros, type: types.local });
        this.Descricao = PropertyEntity({ col: 12, text: Localization.Resources.Consultas.Usuario.Nome.getFieldDescription() });
        this.SituacaoColaborador = PropertyEntity({ col: 0, visible: false, val: ko.observable(_situacaoColaborador) });
        this.Pesquisar = PropertyEntity({
            eventClick: function (e) {
                GridConsulta.CarregarGrid();
            }, type: types.event, text: Localization.Resources.Consultas.Usuario.Pesquisar, visible: true
        });
    };

    var knoutOpcoes = new OpcoesKnout();

    var divBusca = new CriarBusca(idDiv, knoutOpcoes, knout);


    var callback = function (e) {
        knout.codEntity(e.Codigo);
        knout.val(e.Nome);
        divBusca.CloseModal();
        Global.setarFocoProximoCampo(knout.id);
    };

    if (callbackRetorno != null) {
        callback = function (e) {
            divBusca.CloseModal();
            callbackRetorno(e);
        };
    }

    GridConsulta = new GridView(idDiv + "_tabelaEntidades", "Usuario/PesquisarTerceiros", knoutOpcoes, divBusca.OpcaoPadrao(callback), { column: 0, dir: orderDir.desc });
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
    });
};

var BuscarFuncionarioEmbarcador = function (knout, callbackRetorno, basicGrid, ignorarSituacaoMotorista, knoutCarga, situacaoColaborador) {

    var idDiv = guid();
    var GridConsulta;
    var multiplaEscolha = false;

    if (basicGrid != null)
        multiplaEscolha = true;

    var _situacaoColaborador = EnumSituacaoColaborador.Todos;
    if (situacaoColaborador != null)
        _situacaoColaborador = situacaoColaborador;
    else
        _situacaoColaborador = EnumSituacaoColaborador.Todos;

    var OpcoesKnout = function () {
        this.Titulo = PropertyEntity({ text: Localization.Resources.Consultas.Usuario.BuscaDeFuncionario, type: types.local });
        this.TituloGrid = PropertyEntity({ text: Localization.Resources.Consultas.Usuario.Funcionarios, type: types.local });

        this.Nome = PropertyEntity({ col: 12, text: Localization.Resources.Consultas.Usuario.Nome.getFieldDescription() });
        this.SituacaoColaborador = PropertyEntity({ col: 0, visible: false, val: ko.observable(_situacaoColaborador) });
        this.IgnorarSituacaoMotorista = PropertyEntity({ col: 0, visible: false, getType: typesKnockout.bool, val: ko.observable(ignorarSituacaoMotorista == true) });
        this.Carga = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), visible: false });

        this.Pesquisar = PropertyEntity({
            eventClick: function (e) {
                GridConsulta.CarregarGrid();
            }, type: types.event, text: Localization.Resources.Consultas.Usuario.Pesquisar, visible: true
        });
    };

    var knoutOpcoes = new OpcoesKnout();
    var funcaoParamentroDinamico = null;

    if (knoutCarga != null) {
        funcaoParamentroDinamico = function () {
            if (knoutCarga != null) {
                knoutOpcoes.Carga.codEntity(knoutCarga.codEntity());
                knoutOpcoes.Carga.val(knoutCarga.val());
            }
        };
    }

    var divBusca = new CriarBusca(idDiv, knoutOpcoes, knout, funcaoParamentroDinamico, null, multiplaEscolha);

    var callback = function (e) {
        knout.codEntity(e.Codigo);
        knout.val(e.Nome);
        knoutOpcoes.Nome.val(knoutOpcoes.Nome.def);
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
        GridConsulta = new GridView(idDiv + "_tabelaEntidades", "Usuario/PesquisaUsuariosEmbarcador", knoutOpcoes, null, { column: 0, dir: orderDir.desc }, null, null, null, null, objetoBasicGrid);
    } else
        GridConsulta = new GridView(idDiv + "_tabelaEntidades", "Usuario/PesquisaUsuariosEmbarcador", knoutOpcoes, divBusca.OpcaoPadrao(callback), { column: 0, dir: orderDir.desc });

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
};

var BuscarAjudante = function (knout, callbackRetorno, situacaoColaborador) {

    var idDiv = guid();
    var GridConsulta;

    var _situacaoColaborador = EnumSituacaoColaborador.Todos;
    if (situacaoColaborador != null)
        _situacaoColaborador = situacaoColaborador;
    else
        _situacaoColaborador = EnumSituacaoColaborador.Todos;

    var OpcoesKnout = function () {
        this.Titulo = PropertyEntity({ text: Localization.Resources.Consultas.Usuario.BuscaDeMotoristas, type: types.local });
        this.TituloGrid = PropertyEntity({ text: Localization.Resources.Consultas.Usuario.Motoristas, type: types.local });
        this.Nome = PropertyEntity({ col: 12, text: Localization.Resources.Consultas.Usuario.Nome.getFieldDescription() });
        this.SituacaoColaborador = PropertyEntity({ col: 0, visible: false, val: ko.observable(_situacaoColaborador) });
        this.Pesquisar = PropertyEntity({
            eventClick: function (e) {
                GridConsulta.CarregarGrid();
            }, type: types.event, text: Localization.Resources.Consultas.Usuario.Pesquisar, visible: true
        });
    }

    var knoutOpcoes = new OpcoesKnout();

    var divBusca = new CriarBusca(idDiv, knoutOpcoes, knout);


    var callback = function (e) {
        knout.codEntity(e.Codigo);
        knout.val(e.Nome);
        knoutOpcoes.Nome.val(knoutOpcoes.Nome.def);
        divBusca.CloseModal();
        Global.setarFocoProximoCampo(knout.id);
    };

    if (callbackRetorno != null) {
        callback = function (e) {
            divBusca.CloseModal();
            callbackRetorno(e);
        };
    }

    GridConsulta = new GridView(idDiv + "_tabelaEntidades", "Usuario/PesquisarAjudantes", knoutOpcoes, divBusca.OpcaoPadrao(callback), { column: 0, dir: orderDir.desc });
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
};