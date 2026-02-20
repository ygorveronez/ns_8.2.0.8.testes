/// <reference path="../../js/Global/Buscas.js" />
/// <reference path="../../js/Global/Grid.js" />
/// <reference path="../../js/Global/CRUD.js" />
/// <reference path="../../js/libs/jquery-2.1.1.js" />
/// <reference path="ModeloVeicularCarga.js" />
/// <reference path="Tranportador.js" />
/// <reference path="Cliente.js" />
/// <reference path="../../ViewsScripts/Configuracao/Sistema/ConfiguracaoTMS.js" />

function preecherParametrosDinamicosVeiculos(knoutOpcoes, knoutTransportador, knoutModeloVeicularCarga, knoutAcertoViagem, knoutTipoCarga, knoutDataAbastecimento, gridModelosContratoFrete, knoutLocalAtualVeiculo, knoutCarga, knoutTipoVeiculo, knockoutProprietario) {
    var funcaoParamentroDinamico = null;
    var arrayOpcoes = new Array();

    if (knoutTransportador != null ||
        knoutModeloVeicularCarga != null ||
        knoutAcertoViagem != null ||
        knoutTipoCarga != null ||
        knoutDataAbastecimento != null ||
        knoutLocalAtualVeiculo != null ||
        knoutCarga != null ||
        knoutTipoVeiculo != null ||
        knockoutProprietario != null) {

        if (knoutTransportador != null) {
            knoutOpcoes.Empresa.visible = false;
            arrayOpcoes.push({ knout: knoutOpcoes.Empresa, valor: knoutTransportador });
        }

        if (knoutModeloVeicularCarga != null) {
            knoutOpcoes.ModeloVeicularCarga.visible = false;
            arrayOpcoes.push({ knout: knoutOpcoes.ModeloVeicularCarga, valor: knoutModeloVeicularCarga });
        }

        if (knoutAcertoViagem != null) {
            knoutOpcoes.AcertoViagem.visible = false;
            arrayOpcoes.push({ knout: knoutOpcoes.AcertoViagem, valor: knoutAcertoViagem });
        }

        if (knoutTipoCarga != null)
            arrayOpcoes.push({ knout: knoutOpcoes.TipoCarga, valor: knoutTipoCarga });

        if (knoutDataAbastecimento != null)
            arrayOpcoes.push({ knout: knoutOpcoes.DataAbastecimento, valor: knoutDataAbastecimento });

        if (knoutLocalAtualVeiculo != null)
            arrayOpcoes.push({ knout: knoutOpcoes.LocalAtualFisicoDoVeiculo, valor: knoutLocalAtualVeiculo });

        if (knoutCarga != null)
            arrayOpcoes.push({ knout: knoutOpcoes.Carga, valor: knoutCarga });

        if (knoutTipoVeiculo != null)
            arrayOpcoes.push({ knout: knoutOpcoes.TipoVeiculo, valor: knoutTipoVeiculo });

        if (knockoutProprietario != null)
            arrayOpcoes.push({ knout: knoutOpcoes.Proprietario, valor: knockoutProprietario });

        funcaoParamentroDinamico = function () {
            $.each(arrayOpcoes, function (i, opcao) {
                if (opcao.knout.codEntity instanceof Function) {
                    if (opcao.valor.codEntity instanceof Function)
                        opcao.knout.codEntity(opcao.valor.codEntity());
                    else
                        opcao.knout.codEntity(opcao.valor.val());
                }

                if (opcao.knout.getType != typesKnockout.dynamic)
                    opcao.knout.val(opcao.valor.val());
            });

            if (gridModelosContratoFrete) {
                var idModelos = new Array();
                var dataModelos = gridModelosContratoFrete.BuscarRegistros();

                for (var i = 0; i < dataModelos.length; i++)
                    idModelos.push(dataModelos[i].CodigoModelo);

                knoutOpcoes.ModelosVeiculares.val(JSON.stringify(idModelos));
            }
        };
    }

    return funcaoParamentroDinamico;
}

var BuscarVeiculos = function (knout, callbackRetorno, knoutTransportador, knoutModeloVeicularCarga, knoutSetarMotorista, apenasComRenavam, knoutAcertoViagem, knoutTipoCarga, somenteEmpresasAtivas, somenteDisponveis, knoutOuCodigoCarga, knoutOuTipoVeiculo, knoutDataAbastecimento, basicGrid, basicGridModelosContratoFrete, fnAfterDefaultCallback, knoutLocalAtualVeiculo, knoutForcarFiltroModelo, knockoutProprietario, tipoVeiculo, somenteEmEscala, somenteAtivo, concatenarPlaca, basicGridTransportadores, tipoPropriedade, exibirTodosVeiculos, knoutTipoOperacao) {
    var idDiv = guid();
    var GridConsulta;
    var multiplaEscolha = false;

    if (basicGrid != null)
        multiplaEscolha = true;

    if (somenteDisponveis == null)
        somenteDisponveis = false;

    if (tipoVeiculo == null)
        tipoVeiculo = "";

    var forcarFiltroModelo = false;
    if (knoutForcarFiltroModelo !== undefined && knoutForcarFiltroModelo !== null)
        forcarFiltroModelo = knoutForcarFiltroModelo;

    var OpcoesKnout = function () {
        this.Titulo = PropertyEntity({ text: Localization.Resources.Consultas.Veiculo.BuscaDeVeiculos, type: types.local });
        this.TituloGrid = PropertyEntity({ text: Localization.Resources.Consultas.Veiculo.Veiculos, type: types.local });

        this.Empresa = PropertyEntity({ col: 12, type: types.entity, codEntity: ko.observable(0), visible: ko.observable(true), text: Localization.Resources.Consultas.Veiculo.Transportador.getFieldDescription(), idBtnSearch: guid() });
        this.Empresas = PropertyEntity({ getType: typesKnockout.dynamic, visible: false });
        this.Placa = PropertyEntity({ col: 3, text: Localization.Resources.Consultas.Veiculo.Placa.getFieldDescription() });
        this.ModeloVeicularCarga = PropertyEntity({ col: 5, type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Consultas.Veiculo.ModeloVeicularDeCarga.getFieldDescription(), required: false, idBtnSearch: guid(), visible: !IsMobile() });
        this.NumeroFrota = PropertyEntity({ col: 2, text: Localization.Resources.Consultas.Veiculo.NumeroFrota.getFieldDescription(), maxlength: 30, visible: !IsMobile() });
        this.Ativo = PropertyEntity({ col: 2, text: Localization.Resources.Gerais.Geral.Situacao.getFieldDescription(), val: ko.observable(1), options: _statusPesquisa, def: 1, visible: !knout.somenteVeiculosAtivos });

        this.PlacaFormatada = PropertyEntity({ type: types.string, visible: false, val: ko.observable("") });
        this.TipoCarga = PropertyEntity({ type: types.entity, visible: false, codEntity: ko.observable(0), required: false });
        this.SomenteDisponveis = PropertyEntity({ visible: false, getType: typesKnockout.bool, val: ko.observable(somenteDisponveis), def: false });
        this.Carga = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), visible: false });
        this.SomenteEmpresasAtivas = PropertyEntity({ visible: false, getType: typesKnockout.bool, val: ko.observable(true), def: true });
        this.TipoVeiculo = PropertyEntity({ val: ko.observable(tipoVeiculo), visible: false });
        this.DataAbastecimento = PropertyEntity({ col: 12, type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Consultas.Veiculo.DataAbastecimento.getFieldDescription(), required: false, idBtnSearch: guid(), visible: false });
        this.LocalAtualFisicoDoVeiculo = PropertyEntity({ col: 12, type: types.entity, codEntity: ko.observable(0), visible: false, text: Localization.Resources.Consultas.Veiculo.LocalFisicoDoVeiculo.getFieldDescription(), idBtnSearch: guid() });
        this.AcertoViagem = PropertyEntity({ col: 12, type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Consultas.Veiculo.AcertoDeViagem.getFieldDescription(), required: false, idBtnSearch: guid(), visible: false });
        this.ModelosVeiculares = PropertyEntity({ getType: typesKnockout.dynamic, val: ko.observable(new Array()), def: new Array(), visible: false });
        this.ForcarFiltroModelo = PropertyEntity({ val: ko.observable(forcarFiltroModelo), def: false, visible: false });
        this.SomenteEmEscala = PropertyEntity({ visible: false, getType: typesKnockout.bool, val: ko.observable(Boolean(somenteEmEscala)), def: false });
        this.TipoPropriedade = PropertyEntity({ visible: false, val: ko.observable(""), def: "" });
        this.TipoOperacao = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), idBtnSearch: guid(), visible: false });

        this.BuscaAvancada = PropertyEntity({
            eventClick: function (e) {
                if (e.BuscaAvancada.visibleFade()) {
                    e.BuscaAvancada.visibleFade(false);
                    e.BuscaAvancada.icon("fal fa-plus");
                } else {
                    e.BuscaAvancada.visibleFade(true);
                    e.BuscaAvancada.icon("fal fa-minus");
                }
            }, buscaAvancada: true, type: types.event, text: Localization.Resources.Consultas.Veiculo.Avancada, idFade: guid(), cssClass: "btn btn-default", icon: ko.observable("fal fa-plus"), visibleFade: ko.observable(false), visible: true
        });

        this.Segmento = PropertyEntity({ col: 6, type: types.multiplesEntities, codEntity: ko.observable(0), text: Localization.Resources.Consultas.Veiculo.Segmento.getFieldDescription(), idBtnSearch: guid(), visible: true });
        this.Proprietario = PropertyEntity({ col: 6, type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Consultas.Veiculo.Terceiro.getFieldDescription(), idBtnSearch: guid(), visible: _CONFIGURACAO_TMS.VisualizarVeiculosPropriosETerceiros });
        this.Motorista = PropertyEntity({ col: 6, type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Consultas.Veiculo.Motorista.getFieldDescription(), idBtnSearch: guid(), visible: ko.observable(true) });
        this.Chassi = PropertyEntity({ col: 3, text: Localization.Resources.Consultas.Veiculo.NumeroChassi.getFieldDescription(), val: ko.observable(""), def: "", visible: true });

        this.Pesquisar = PropertyEntity({
            eventClick: function (e) {
                GridConsulta.CarregarGrid();
            }, type: types.event, text: Localization.Resources.Gerais.Geral.Pesquisar, visible: true
        });
    };
    
    var knoutOpcoes = new OpcoesKnout();
    var knoutCarga = null;
    var knoutTipoVeiculo, knoutObjetoTipoOperacao;

    if (knoutOuCodigoCarga instanceof Object)
        knoutCarga = knoutOuCodigoCarga;
    else if (knoutOuCodigoCarga > 0) {
        knoutOpcoes.Carga.codEntity(knoutOuCodigoCarga);
        knoutOpcoes.Carga.val(knoutOuCodigoCarga);
    }

    if (knoutTipoOperacao instanceof Object) {
        knoutOpcoes.TipoOperacao = knoutTipoOperacao;
        knoutOpcoes.TipoOperacao.visible = false;
    }
    else if (knoutTipoOperacao != null) {
        knoutOpcoes.TipoOperacao.codEntity(knoutTipoOperacao);
        knoutOpcoes.TipoOperacao.val(knoutTipoOperacao);
        knoutOpcoes.TipoOperacao.visible = false;
    }

    if (knoutOuTipoVeiculo instanceof Object)
        knoutTipoVeiculo = knoutOuTipoVeiculo;
    else if (knoutOuTipoVeiculo)
        knoutOpcoes.TipoVeiculo.val(knoutOuTipoVeiculo);

    if (somenteEmpresasAtivas != null)
        knoutOpcoes.SomenteEmpresasAtivas.val(somenteEmpresasAtivas);

    if (exibirTodosVeiculos) {
        knoutOpcoes.Ativo.val(0);
    }
    else if (somenteAtivo || _CONFIGURACAO_TMS.VisualizarApenasVeiculosAtivos) {
        knoutOpcoes.Ativo.val(1);
        knoutOpcoes.Ativo.visible = false;
        knoutOpcoes.ModeloVeicularCarga.col = 7;
    }
    
    if (tipoPropriedade != null)
        knoutOpcoes.TipoPropriedade.val(tipoPropriedade);

    var funcaoParamentroDinamico = function () {
        if (basicGridTransportadores) {
            var transportadores = basicGridTransportadores.BuscarRegistros();

            if (transportadores.length > 0) {
                var codigosTransportadores = transportadores.map(transportador => transportador.Codigo);

                knoutOpcoes.Empresas.val(JSON.stringify(codigosTransportadores));
                knoutOpcoes.Empresa.visible(false);
            }
            else {
                knoutOpcoes.Empresas.val("");
                knoutOpcoes.Empresa.visible(true);
            }
        }
        else if (knoutTransportador) {
            knoutOpcoes.Empresa.visible(false);

            if (knoutTransportador.type == types.multiplesEntities) {
                var codigosTransportadores = knoutTransportador.multiplesEntities().map(transportador => transportador.Codigo);

                if (codigosTransportadores?.length > 0)
                    knoutOpcoes.Empresas.val(JSON.stringify(codigosTransportadores));
            } else {
                knoutOpcoes.Empresa.codEntity(knoutTransportador.codEntity());
                knoutOpcoes.Empresa.val(knoutTransportador.val());
            }
        }

        if (knoutModeloVeicularCarga) {
            knoutOpcoes.ModeloVeicularCarga.visible = false;
            knoutOpcoes.ModeloVeicularCarga.codEntity(knoutModeloVeicularCarga.codEntity());
            knoutOpcoes.ModeloVeicularCarga.val(knoutModeloVeicularCarga.val());
        }

        if (knoutAcertoViagem) {
            knoutOpcoes.AcertoViagem.visible = false;
            knoutOpcoes.AcertoViagem.codEntity(knoutAcertoViagem.val());
            knoutOpcoes.AcertoViagem.val(knoutAcertoViagem.val());
        }

        if (knoutTipoCarga) {
            knoutOpcoes.TipoCarga.codEntity(knoutTipoCarga.codEntity());
            knoutOpcoes.TipoCarga.val(knoutTipoCarga.val());
        }

        if (knoutDataAbastecimento)
            knoutOpcoes.DataAbastecimento.val(knoutDataAbastecimento.val());

        if (knoutLocalAtualVeiculo) {
            knoutOpcoes.LocalAtualFisicoDoVeiculo.codEntity(knoutLocalAtualVeiculo.codEntity());
            knoutOpcoes.LocalAtualFisicoDoVeiculo.val(knoutLocalAtualVeiculo.val());
        }

        if (knoutCarga instanceof Object) {
            knoutOpcoes.Carga.codEntity(knoutCarga.codEntity());
            knoutOpcoes.Carga.val(knoutCarga.val());
        }

        if (knoutTipoVeiculo)
            knoutOpcoes.TipoVeiculo.val(knoutTipoVeiculo.val());

        if (knockoutProprietario) {
            knoutOpcoes.Proprietario.codEntity(knockoutProprietario.codEntity());
            knoutOpcoes.Proprietario.val(knockoutProprietario.val());
        }

        if (basicGridModelosContratoFrete) {
            var modelosContratoFrete = basicGridModelosContratoFrete.BuscarRegistros();
            var codigosModelosVeicularesCarga = modelosContratoFrete.map(modelo => modelo.CodigoModelo);

            knoutOpcoes.ModelosVeiculares.val(JSON.stringify(codigosModelosVeicularesCarga));
        }
    }

    if (
        _CONFIGURACAO_TMS.TipoServicoMultisoftware === EnumTipoServicoMultisoftware.MultiTMS ||
        _CONFIGURACAO_TMS.TipoServicoMultisoftware === EnumTipoServicoMultisoftware.MultiCTe ||
        _CONFIGURACAO_TMS.TipoServicoMultisoftware === EnumTipoServicoMultisoftware.MultiNFe ||
        _CONFIGURACAO_TMS.TipoServicoMultisoftware === EnumTipoServicoMultisoftware.Terceiros
    )
        knoutOpcoes.Empresa.visible(false);

    if (_CONFIGURACAO_TMS.TipoServicoMultisoftware === EnumTipoServicoMultisoftware.MultiNFe) {
        knoutOpcoes.ModeloVeicularCarga.visible = false;
        knoutOpcoes.Ativo.col = 4;
        knoutOpcoes.Placa.col = 4;
        knoutOpcoes.NumeroFrota.col = 4;
    }

    var divBusca = new CriarBusca(idDiv, knoutOpcoes, knout, funcaoParamentroDinamico, null, multiplaEscolha, function () {
        new BuscarModelosVeicularesCarga(knoutOpcoes.ModeloVeicularCarga);
        new BuscarTransportadores(knoutOpcoes.Empresa);
        new BuscarSegmentoVeiculo(knoutOpcoes.Segmento);
        new BuscarClientes(knoutOpcoes.Proprietario);
        new BuscarMotoristas(knoutOpcoes.Motorista);
    });

    if (_CONFIGURACAO_TMS.Pais == EnumPaises.Brasil)
        $("#" + knoutOpcoes.Placa.id).mask("AAAAAAA", { selectOnFocus: true, clearIfNotMatch: true });

    var callback = function (e) {
        var aprovado = true;

        if (apenasComRenavam != null && apenasComRenavam) {
            if (e.Renavam == "") {
                exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Atencao, Localization.Resources.Consultas.Veiculo.NaoPermitidoSelecionarUmVeiculoSemRenavam, 10000);
                aprovado = false;
            }
        }

        if (aprovado) {
            knout.codEntity(e.Codigo);
            knout.val(e.Placa);
            knoutOpcoes.Empresa.codEntity(knoutOpcoes.Empresa.defCodEntity);
            knoutOpcoes.Empresa.val(knoutOpcoes.Empresa.def);
            knoutOpcoes.ModeloVeicularCarga.codEntity(knoutOpcoes.ModeloVeicularCarga.defCodEntity);
            knoutOpcoes.ModeloVeicularCarga.val(knoutOpcoes.ModeloVeicularCarga.def);
            knoutOpcoes.Placa.val(knoutOpcoes.Placa.def);

            knout.requiredClass("form-control");

            if (knoutSetarMotorista != null) {
                knoutSetarMotorista.val(e.Motorista);
                knoutSetarMotorista.codEntity(e.CodigoMotorista);

                if (e.CodigoMotorista > 0)
                    knoutSetarMotorista.requiredClass("form-control");
            }

            divBusca.CloseModal();
            Global.setarFocoProximoCampo(knout.id);
        }

        if (concatenarPlaca)
            knoutOpcoes.Placa.val(knoutOpcoes.PlacaFormatada.val());
    };

    if (callbackRetorno != null) {
        callback = function (e) {
            divBusca.CloseModal();
            callbackRetorno(e);
        };
    }

    if (multiplaEscolha) {
        var objetoBasicGrid = { basicGrid: basicGrid, callback: callbackRetorno, eventos: knoutOpcoes.Pesquisar, afterDefaultCallback: fnAfterDefaultCallback };
        GridConsulta = new GridView(idDiv + "_tabelaEntidades", "Veiculo/Pesquisa", knoutOpcoes, null, { column: 0, dir: orderDir.desc }, null, null, null, null, objetoBasicGrid, 10);
    }
    else
        GridConsulta = new GridView(idDiv + "_tabelaEntidades", "Veiculo/Pesquisa", knoutOpcoes, divBusca.OpcaoPadrao(callback, 22), null, null, null, null, null, null, 10);

    divBusca.AddEvents(GridConsulta);

    divBusca.AddTabPressEvent(function (outraPressionada) {
        if (outraPressionada) {
            knoutOpcoes.Placa.val(knout.val());
        }
        GridConsulta.CarregarGrid(function (lista) {
            if (lista.data.length == 1) {
                callback(lista.data[0]);
            } else {
                divBusca.OpenModal();
            }
        });
    });

    this.registroUnico = function () {
        LimparCampos(knoutOpcoes);
        funcaoParamentroDinamico();
        GridConsulta.CarregarGrid(function (lista) {
            if (lista.data.length == 1) {
                callback(lista.data[0]);
                return;
            }
        });
    };
};

var BuscarReboques = function (knout, callbackRetorno, basicGrid) {

    var idDiv = guid();
    var GridConsulta;

    var multiplaEscolha = false;

    if (basicGrid != null)
        multiplaEscolha = true;

    var OpcoesKnout = function () {
        this.Titulo = PropertyEntity({ text: Localization.Resources.Consultas.Veiculo.BuscaDeReboque, type: types.local });
        this.TituloGrid = PropertyEntity({ text: Localization.Resources.Consultas.Veiculo.Reboques, type: types.local });

        this.Placa = PropertyEntity({ col: 5, text: Localization.Resources.Consultas.Veiculo.Placa.getFieldDescription() });
        this.NumeroFrota = PropertyEntity({ col: 4, text: Localization.Resources.Consultas.Veiculo.NumeroFrota.getFieldDescription(), maxlength: 30 });
        this.Ativo = PropertyEntity({ col: 3, text: Localization.Resources.Gerais.Geral.Situacao.getFieldDescription(), val: ko.observable(1), options: _statusPesquisa, def: 1, visible: true });

        this.TipoVeiculo = PropertyEntity({ col: 12, type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Consultas.Veiculo.TipoVeiculo.getFieldDescription(), required: false, visible: false, idBtnSearch: guid() });

        this.Pesquisar = PropertyEntity({
            eventClick: function (e) {
                GridConsulta.CarregarGrid();
            }, type: types.event, text: Localization.Resources.Gerais.Geral.Pesquisar, visible: true
        });
    };

    var knoutOpcoes = new OpcoesKnout();

    var funcaoParamentroDinamico = null;

    var divBusca = new CriarBusca(idDiv, knoutOpcoes, knout, funcaoParamentroDinamico, null, multiplaEscolha);

    knoutOpcoes.TipoVeiculo.codEntity("1");
    knoutOpcoes.TipoVeiculo.val("1");

    if (_CONFIGURACAO_TMS.Pais == EnumPaises.Brasil)
        $("#" + knoutOpcoes.Placa.id).mask("AAAAAAA", { selectOnFocus: true, clearIfNotMatch: true });

    if (_CONFIGURACAO_TMS.VisualizarApenasVeiculosAtivos) {
        knoutOpcoes.Ativo.val(1);
        knoutOpcoes.Ativo.visible = false;
        knoutOpcoes.NumeroFrota.col = 7;
    }

    var callback = function (e) {
        var aprovado = true;
        if (aprovado) {
            knout.codEntity(e.Codigo);
            knout.val(e.Placa);
            knoutOpcoes.Placa.val(knoutOpcoes.Placa.def);
            knoutOpcoes.NumeroFrota.val(knoutOpcoes.NumeroFrota.def);

            knout.requiredClass("form-control");

            divBusca.CloseModal();
            Global.setarFocoProximoCampo(knout.id);
        }
    };

    if (callbackRetorno != null) {
        callback = function (e) {
            divBusca.CloseModal();
            callbackRetorno(e);
        };
    }

    if (multiplaEscolha) {
        var objetoBasicGrid = { basicGrid: basicGrid, callback: callbackRetorno, eventos: knoutOpcoes.Pesquisar };
        GridConsulta = new GridView(idDiv + "_tabelaEntidades", "Veiculo/Pesquisa", knoutOpcoes, null, null, null, null, null, null, objetoBasicGrid);
    } else
        GridConsulta = new GridView(idDiv + "_tabelaEntidades", "Veiculo/Pesquisa", knoutOpcoes, divBusca.OpcaoPadrao(callback), null);

    divBusca.AddEvents(GridConsulta);

    divBusca.AddTabPressEvent(function (outraPressionada) {
        if (outraPressionada) {
            knoutOpcoes.Placa.val(knout.val());
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

var BuscarTracaoManobra = function (knout, callbackRetorno, basicGrid) {

    var idDiv = guid();
    var GridConsulta;
    var multiplaEscolha = (basicGrid != null);

    var OpcoesKnout = function () {

        this.Titulo = PropertyEntity({ text: Localization.Resources.Consultas.Veiculo.BuscaDeTracaoDeManobra, type: types.local });
        this.TituloGrid = PropertyEntity({ text: Localization.Resources.Consultas.Veiculo.TracoesDeManobra, type: types.local });

        this.Empresa = PropertyEntity({ col: 12, type: types.entity, codEntity: ko.observable(0), visible: true, text: Localization.Resources.Consultas.Veiculo.Transportador.getFieldDescription(), idBtnSearch: guid() });
        this.Placa = PropertyEntity({ col: 3, text: Localization.Resources.Consultas.Veiculo.Placa.getFieldDescription() });
        this.ModeloVeicularCarga = PropertyEntity({ col: 6, type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Consultas.Veiculo.ModeloVeicularDeCarga.getFieldDescription(), idBtnSearch: guid() });
        this.Ativo = PropertyEntity({ col: 3, text: Localization.Resources.Gerais.Geral.Situacao.getFieldDescription(), val: ko.observable(1), options: _statusPesquisa, def: 1, visible: true });

        this.TipoVeiculo = PropertyEntity({ visible: false, val: ko.observable("0"), def: "0" });
        this.SomenteEmpresasAtivas = PropertyEntity({ visible: false, getType: typesKnockout.bool, val: ko.observable(true), def: true });

        this.Pesquisar = PropertyEntity({
            eventClick: function (e) {
                GridConsulta.CarregarGrid();
            }, type: types.event, text: Localization.Resources.Gerais.Geral.Pesquisar, visible: true
        });
    };

    var knoutOpcoes = new OpcoesKnout();

    if (
        _CONFIGURACAO_TMS.TipoServicoMultisoftware === EnumTipoServicoMultisoftware.MultiTMS ||
        _CONFIGURACAO_TMS.TipoServicoMultisoftware === EnumTipoServicoMultisoftware.MultiCTe ||
        _CONFIGURACAO_TMS.TipoServicoMultisoftware === EnumTipoServicoMultisoftware.MultiNFe ||
        _CONFIGURACAO_TMS.TipoServicoMultisoftware === EnumTipoServicoMultisoftware.Terceiros
    )
        knoutOpcoes.Empresa.visible = false;

    var divBusca = new CriarBusca(idDiv, knoutOpcoes, knout, null, null, multiplaEscolha, function () {
        new BuscarModelosVeicularesCarga(knoutOpcoes.ModeloVeicularCarga);
    });

    if (_CONFIGURACAO_TMS.Pais == EnumPaises.Brasil)
        $("#" + knoutOpcoes.Placa.id).mask("AAAAAAA", { selectOnFocus: true, clearIfNotMatch: true });

    if (_CONFIGURACAO_TMS.VisualizarApenasVeiculosAtivos) {
        knoutOpcoes.Ativo.val(1);
        knoutOpcoes.Ativo.visible = false;
        knoutOpcoes.Placa.col = 6;
    }

    var callback = function (e) {
        knout.codEntity(e.Codigo);
        knout.val(e.Placa);
        knoutOpcoes.Placa.val(knoutOpcoes.Placa.def);

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
        GridConsulta = new GridView(idDiv + "_tabelaEntidades", "Veiculo/Pesquisa", knoutOpcoes, null, null, null, null, null, null, objetoBasicGrid);
    } else
        GridConsulta = new GridView(idDiv + "_tabelaEntidades", "Veiculo/Pesquisa", knoutOpcoes, divBusca.OpcaoPadrao(callback), null);

    divBusca.AddEvents(GridConsulta);

    divBusca.AddTabPressEvent(function (outraPressionada) {
        if (outraPressionada) {
            knoutOpcoes.Placa.val(knout.val());
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