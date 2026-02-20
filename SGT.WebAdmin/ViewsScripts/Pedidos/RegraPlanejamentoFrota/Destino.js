//*******MAPEAMENTO KNOUCKOUT*******

var _gridDestino, _gridClienteDestino, _gridEstadoDestino, _gridRegiaoDestino, _gridRotaDestino, _gridCEPDestino, _gridPaisDestino;

//*******EVENTOS*******

function LoadDestinos() {
    LoadLocalidadeDestino();
    LoadEstadoDestino();
    LoadRegiaoDestino();
    LoadClienteDestino();
    LoadFaixaCEPDestino();
    LoadRotaDestino();
    LoadPaisDestino();
}

function LoadLocalidadeDestino() {
    var menuOpcoes = {
        tipo: TypeOptionMenu.link, tamanho: 7, opcoes: [{
            descricao: "Excluir", id: guid(), metodo: function (data) {
                ExcluirDestinoClick(_regraPlanejamentoFrota.Destino, data);
            }
        }]
    };

    var header = [
        { data: "Codigo", visible: false },
        { data: "Descricao", title: "Descrição", width: "80%" }
    ];

    _gridDestino = new BasicDataTable(_regraPlanejamentoFrota.GridDestino.id, header, menuOpcoes, { column: 1, dir: orderDir.asc });

    new BuscarLocalidades(_regraPlanejamentoFrota.Destino, null, null, null, _gridDestino, ValidarDestinosDisponiveis);

    _regraPlanejamentoFrota.Destino.basicTable = _gridDestino;
    _regraPlanejamentoFrota.Destino.basicTable.CarregarGrid(new Array());
}

function LoadEstadoDestino() {
    var menuOpcoes = {
        tipo: TypeOptionMenu.link, tamanho: 7, opcoes: [{
            descricao: "Excluir", id: guid(), metodo: function (data) {
                ExcluirDestinoClick(_regraPlanejamentoFrota.EstadoDestino, data);
            }
        }]
    };

    var header = [
        { data: "Codigo", visible: false },
        { data: "Descricao", title: "Descrição", width: "80%" }
    ];

    _gridEstadoDestino = new BasicDataTable(_regraPlanejamentoFrota.GridEstadoDestino.id, header, menuOpcoes, { column: 1, dir: orderDir.asc });

    new BuscarEstados(_regraPlanejamentoFrota.EstadoDestino, null, _gridEstadoDestino, ValidarDestinosDisponiveis);

    _regraPlanejamentoFrota.EstadoDestino.basicTable = _gridEstadoDestino;
    _regraPlanejamentoFrota.EstadoDestino.basicTable.CarregarGrid(new Array());
}

function LoadRegiaoDestino() {
    var menuOpcoes = {
        tipo: TypeOptionMenu.link, tamanho: 7, opcoes: [{
            descricao: "Excluir", id: guid(), metodo: function (data) {
                ExcluirDestinoClick(_regraPlanejamentoFrota.RegiaoDestino, data);
            }
        }]
    };

    var header = [
        { data: "Codigo", visible: false },
        { data: "Descricao", title: "Descrição", width: "80%" }
    ];

    _gridRegiaoDestino = new BasicDataTable(_regraPlanejamentoFrota.GridRegiaoDestino.id, header, menuOpcoes, { column: 1, dir: orderDir.asc });

    new BuscarRegioes(_regraPlanejamentoFrota.RegiaoDestino, null, _gridRegiaoDestino, ValidarDestinosDisponiveis);

    _regraPlanejamentoFrota.RegiaoDestino.basicTable = _gridRegiaoDestino;
    _regraPlanejamentoFrota.RegiaoDestino.basicTable.CarregarGrid(new Array());
}

function LoadRotaDestino() {
    var menuOpcoes = {
        tipo: TypeOptionMenu.link, tamanho: 7, opcoes: [{
            descricao: "Excluir", id: guid(), metodo: function (data) {
                ExcluirDestinoClick(_regraPlanejamentoFrota.RotaDestino, data);
            }
        }]
    };

    var header = [
        { data: "Codigo", visible: false },
        { data: "Descricao", title: "Descrição", width: "80%" }
    ];

    _gridRotaDestino = new BasicDataTable(_regraPlanejamentoFrota.GridRotaDestino.id, header, menuOpcoes, { column: 1, dir: orderDir.asc });

    new BuscarRotasFrete(_regraPlanejamentoFrota.RotaDestino, null, _gridRotaDestino, null, null, null, ValidarDestinosDisponiveis, null, _regraPlanejamentoFrota.TabelaFrete);

    _regraPlanejamentoFrota.RotaDestino.basicTable = _gridRotaDestino;
    _regraPlanejamentoFrota.RotaDestino.basicTable.CarregarGrid(new Array());
}

function LoadClienteDestino() {
    var menuOpcoes = {
        tipo: TypeOptionMenu.link, tamanho: 7, opcoes: [{
            descricao: "Excluir", id: guid(), metodo: function (data) {
                ExcluirDestinoClick(_regraPlanejamentoFrota.ClienteDestino, data);
            }
        }]
    };

    var header = [
        { data: "Codigo", visible: false },
        { data: "Descricao", title: "Descrição", width: "80%" }
    ];

    _gridClienteDestino = new BasicDataTable(_regraPlanejamentoFrota.GridClienteDestino.id, header, menuOpcoes, { column: 1, dir: orderDir.asc });

    new BuscarClientes(_regraPlanejamentoFrota.ClienteDestino, null, false, null, null, _gridClienteDestino, null, null, null, null, ValidarDestinosDisponiveis);

    _regraPlanejamentoFrota.ClienteDestino.basicTable = _gridClienteDestino;
    _regraPlanejamentoFrota.ClienteDestino.basicTable.CarregarGrid(new Array());
}

function LoadPaisDestino() {
    var menuOpcoes = {
        tipo: TypeOptionMenu.link, tamanho: 7, opcoes: [{
            descricao: "Excluir", id: guid(), metodo: function (data) {
                ExcluirDestinoClick(_regraPlanejamentoFrota.PaisDestino, data);
            }
        }]
    };

    var header = [
        { data: "Codigo", visible: false },
        { data: "Descricao", title: "Descrição", width: "80%" }
    ];

    _gridPaisDestino = new BasicDataTable(_regraPlanejamentoFrota.GridPaisDestino.id, header, menuOpcoes, { column: 1, dir: orderDir.asc });

    new BuscarPaises(_regraPlanejamentoFrota.PaisDestino, null, null, null, _gridPaisDestino, ValidarDestinosDisponiveis);

    _regraPlanejamentoFrota.PaisDestino.basicTable = _gridPaisDestino;
    _regraPlanejamentoFrota.PaisDestino.basicTable.CarregarGrid(new Array());
}

function ExcluirDestinoClick(knoutDestino, data) {
    var origemGrid = knoutDestino.basicTable.BuscarRegistros();

    for (var i = 0; i < origemGrid.length; i++) {
        if (data.Codigo == origemGrid[i].Codigo) {
            origemGrid.splice(i, 1);
            break;
        }
    }

    knoutDestino.basicTable.CarregarGrid(origemGrid);

    ValidarDestinosDisponiveis(false);
}

//#region Faixa de CEP

function LoadFaixaCEPDestino() {
    if (_gridCEPDestino != null)
        _gridCEPDestino.Destroy();

    var menuOpcoes = { tipo: TypeOptionMenu.link, tamanho: 7, opcoes: [{ descricao: "Excluir", id: guid(), metodo: ExcluirCEPDestinoClick }] };

    var header = [
        { data: "Codigo", visible: false },
        { data: "Descricao", title: "Descrição", width: _regraPlanejamentoFrota.CEPDestinoDiasUteis.visible() ? "55%" : "80%" },
        { data: "DiasUteis", title: "Prazo (Dias Úteis)", width: "25%", visible: _regraPlanejamentoFrota.CEPDestinoDiasUteis.visible() }
    ];

    _gridCEPDestino = new BasicDataTable(_regraPlanejamentoFrota.GridCEPDestino.id, header, menuOpcoes, { column: 1, dir: orderDir.asc });

    RecarregarGridCEPDestino();
}

function RecarregarGridCEPDestino() {

    var data = new Array();

    $.each(_regraPlanejamentoFrota.CEPsDestino.val(), function (i, cep) {
        var cepGrid = new Object();

        cepGrid.Codigo = cep.Codigo;
        cepGrid.Descricao = "De " + cep.CEPInicial + " até " + cep.CEPFinal;
        cepGrid.DiasUteis = cep.DiasUteis;

        data.push(cepGrid);
    });

    _gridCEPDestino.CarregarGrid(data);
}

function ExcluirCEPDestinoClick(data) {
    var cepsDestino = _regraPlanejamentoFrota.CEPsDestino.val();

    for (var i = 0; i < cepsDestino.length; i++) {
        if (data.Codigo == cepsDestino[i].Codigo) {
            cepsDestino.splice(i, 1);
            break;
        }
    }

    _regraPlanejamentoFrota.CEPsDestino.val(cepsDestino);

    RecarregarGridCEPDestino();
    ValidarDestinosDisponiveis(false);
}

function AdicionarCEPDestinoClick(e, sender) {

    var cepInicial = Globalize.parseInt(string.OnlyNumbers(_regraPlanejamentoFrota.CEPDestinoInicial.val()));
    var cepFinal = Globalize.parseInt(string.OnlyNumbers(_regraPlanejamentoFrota.CEPDestinoFinal.val()));

    if (isNaN(cepInicial) || isNaN(cepFinal)) {
        exibirMensagem(tipoMensagem.aviso, "Faixa de CEP inválida", "Informe uma faixa de CEP válida.");
        return;
    }

    if (cepFinal < cepInicial) {
        exibirMensagem(tipoMensagem.aviso, "Faixa de CEP inválida", "O CEP inicial não pode ser menor que o CEP final.");
        return;
    }

    var cepsDestino = _regraPlanejamentoFrota.CEPsDestino.val();

    if (cepsDestino.some(function (item) {
        var cepInicialCadastrado = Globalize.parseInt(string.OnlyNumbers(item.CEPInicial));
        var cepFinalCadastrado = Globalize.parseInt(string.OnlyNumbers(item.CEPFinal));

        if ((cepInicialCadastrado >= cepInicial && cepInicialCadastrado <= cepFinal) ||
            (cepFinalCadastrado >= cepInicial && cepFinalCadastrado <= cepFinal) ||
            (cepInicial >= cepInicialCadastrado && cepInicial <= cepFinalCadastrado) ||
            (cepFinal >= cepInicialCadastrado && cepFinal <= cepFinalCadastrado))
            return true;
    })) {
        exibirMensagem(tipoMensagem.aviso, "Faixa de CEP já existe", "Esta faixa de CEP entrou em conflito com outra já cadastrada.");
        return;
    }

    cepsDestino.push({
        Codigo: guid(),
        CEPInicial: _regraPlanejamentoFrota.CEPDestinoInicial.val(),
        CEPFinal: _regraPlanejamentoFrota.CEPDestinoFinal.val(),
        DiasUteis: _regraPlanejamentoFrota.CEPDestinoDiasUteis.val()
    })

    _regraPlanejamentoFrota.CEPsDestino.val(cepsDestino);

    RecarregarGridCEPDestino();

    LimparCamposCEPDestino();

    ValidarDestinosDisponiveis();
}

function LimparCamposCEPDestino() {
    _regraPlanejamentoFrota.CEPDestinoInicial.val("");
    _regraPlanejamentoFrota.CEPDestinoFinal.val("");
    _regraPlanejamentoFrota.CEPDestinoDiasUteis.val("");
}

//#endregion Faixa de CEP

function ValidarDestinosDisponiveis(reposicionarTabPrincipal) {
    if (_regraPlanejamentoFrota.ClienteDestino.basicTable.BuscarRegistros().length > 0) {
        $("#liTabCidadesDestino").hide();
        $("#liTabClientesDestino").show();
        $("#liTabEstadosDestino").hide();
        $("#liTabRegioesDestino").hide();
        $("#liTabRotasDestino").hide();
        $("#liTabCEPsDestino").hide();
        $("#liTabPaisesDestino").hide();
        $(".nav-tabs a[href='#tabClientesDestino']").tab('show');
    } else if (_regraPlanejamentoFrota.Destino.basicTable.BuscarRegistros().length > 0) {
        $("#liTabCidadesDestino").show();
        $("#liTabClientesDestino").hide();
        $("#liTabEstadosDestino").hide();
        $("#liTabRegioesDestino").hide();
        $("#liTabRotasDestino").hide();
        $("#liTabCEPsDestino").hide();
        $("#liTabPaisesDestino").hide();
        $(".nav-tabs a[href='#tabCidadesDestino']").tab('show');
    } else if (_regraPlanejamentoFrota.EstadoDestino.basicTable.BuscarRegistros().length > 0) {
        $("#liTabCidadesDestino").hide();
        $("#liTabClientesDestino").hide();
        $("#liTabEstadosDestino").show();
        $("#liTabRegioesDestino").hide();
        $("#liTabRotasDestino").hide();
        $("#liTabCEPsDestino").hide();
        $("#liTabPaisesDestino").hide();
        $(".nav-tabs a[href='#tabEstadosDestino']").tab('show');
    } else if (_regraPlanejamentoFrota.RegiaoDestino.basicTable.BuscarRegistros().length > 0) {
        $("#liTabCidadesDestino").hide();
        $("#liTabClientesDestino").hide();
        $("#liTabEstadosDestino").hide();
        $("#liTabRegioesDestino").show();
        $("#liTabRotasDestino").hide();
        $("#liTabCEPsDestino").hide();
        $("#liTabPaisesDestino").hide();
        $(".nav-tabs a[href='#tabRegioesDestino']").tab('show');
    } else if (_regraPlanejamentoFrota.RotaDestino.basicTable.BuscarRegistros().length > 0) {
        $("#liTabCidadesDestino").hide();
        $("#liTabClientesDestino").hide();
        $("#liTabEstadosDestino").hide();
        $("#liTabRegioesDestino").hide();
        $("#liTabRotasDestino").show();
        $("#liTabCEPsDestino").hide();
        $("#liTabPaisesDestino").hide();
        $(".nav-tabs a[href='#tabRotasDestino']").tab('show');
    } else if (_regraPlanejamentoFrota.CEPsDestino.val().length > 0) {
        $("#liTabCidadesDestino").hide();
        $("#liTabClientesDestino").hide();
        $("#liTabEstadosDestino").hide();
        $("#liTabRegioesDestino").hide();
        $("#liTabRotasDestino").hide();
        $("#liTabCEPsDestino").show();
        $("#liTabPaisesDestino").hide();
        $(".nav-tabs a[href='#tabCEPsDestino']").tab('show');
    } else if (_regraPlanejamentoFrota.PaisDestino.basicTable.BuscarRegistros().length > 0) {
        $("#liTabCidadesDestino").hide();
        $("#liTabClientesDestino").hide();
        $("#liTabEstadosDestino").hide();
        $("#liTabRegioesDestino").hide();
        $("#liTabRotasDestino").hide();
        $("#liTabCEPsDestino").hide();
        $("#liTabPaisesDestino").show();
        $(".nav-tabs a[href='#tabPaisesDestino']").tab('show');
    } else {
        $("#liTabCidadesDestino").show();
        $("#liTabClientesDestino").show();
        $("#liTabEstadosDestino").show();
        $("#liTabPaisesDestino").show();
        $("#liTabRegioesDestino").show();

        if (_CONFIGURACAO_TMS.TipoServicoMultisoftware == EnumTipoServicoMultisoftware.MultiEmbarcador) {
            $("#liTabRotasDestino").show();
            //$("#liTabRegioesDestino").hide();
        }
        //else {
        //    //$("#liTabRotasDestino").hide();
        //    $("#liTabRegioesDestino").show();
        //}

        $("#liTabCEPsDestino").show();

        if (reposicionarTabPrincipal !== false)
            $(".nav-tabs a[href='#tabCidadesDestino']").tab('show');
    }
}