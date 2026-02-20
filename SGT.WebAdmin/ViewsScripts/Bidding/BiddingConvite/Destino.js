//*******MAPEAMENTO KNOUCKOUT*******

var _gridDestino, _gridClienteDestino, _gridEstadoDestino, _gridRegiaoDestino, _gridRotaDestino, _gridCEPDestino, _gridPaisDestino;

//*******EVENTOS*******

function loadDestinos() {
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
                ExcluirDestinoClick(_origensDestinos.Destino, data);
            }
        }]
    };

    var header = [
        { data: "Codigo", visible: false },
        { data: "Descricao", title: Localization.Resources.Gerais.Geral.Descricao, width: "80%" }
    ];

    _gridDestino = new BasicDataTable(_origensDestinos.GridDestino.id, header, menuOpcoes, { column: 1, dir: orderDir.asc });

    new BuscarLocalidades(_origensDestinos.Destino, null, null, null, _gridDestino, ValidarDestinosDisponiveis);

    _origensDestinos.Destino.basicTable = _gridDestino;
    _origensDestinos.Destino.basicTable.CarregarGrid(new Array());
}

function LoadEstadoDestino() {
    var menuOpcoes = {
        tipo: TypeOptionMenu.link, tamanho: 7, opcoes: [{
            descricao: "Excluir", id: guid(), metodo: function (data) {
                ExcluirDestinoClick(_origensDestinos.EstadoDestino, data);
            }
        }]
    };

    var header = [
        { data: "Codigo", visible: false },
        { data: "Descricao", title: "Descrição", width: "80%" }
    ];

    _gridEstadoDestino = new BasicDataTable(_origensDestinos.GridEstadoDestino.id, header, menuOpcoes, { column: 1, dir: orderDir.asc });

    new BuscarEstados(_origensDestinos.EstadoDestino, null, _gridEstadoDestino, ValidarDestinosDisponiveis);

    _origensDestinos.EstadoDestino.basicTable = _gridEstadoDestino;
    _origensDestinos.EstadoDestino.basicTable.CarregarGrid(new Array());
}

function LoadRegiaoDestino() {
    var menuOpcoes = {
        tipo: TypeOptionMenu.link, tamanho: 7, opcoes: [{
            descricao: "Excluir", id: guid(), metodo: function (data) {
                ExcluirDestinoClick(_origensDestinos.RegiaoDestino, data);
            }
        }]
    };

    var header = [
        { data: "Codigo", visible: false },
        { data: "Descricao", title: "Descrição", width: "80%" }
    ];

    _gridRegiaoDestino = new BasicDataTable(_origensDestinos.GridRegiaoDestino.id, header, menuOpcoes, { column: 1, dir: orderDir.asc });

    new BuscarRegioes(_origensDestinos.RegiaoDestino, null, _gridRegiaoDestino, ValidarDestinosDisponiveis);

    _origensDestinos.RegiaoDestino.basicTable = _gridRegiaoDestino;
    _origensDestinos.RegiaoDestino.basicTable.CarregarGrid(new Array());
}

function LoadRotaDestino() {
    var menuOpcoes = {
        tipo: TypeOptionMenu.link, tamanho: 7, opcoes: [{
            descricao: "Excluir", id: guid(), metodo: function (data) {
                ExcluirDestinoClick(_origensDestinos.RotaDestino, data);
            }
        }]
    };

    var header = [
        { data: "Codigo", visible: false },
        { data: "Descricao", title: "Descrição", width: "80%" }
    ];

    _gridRotaDestino = new BasicDataTable(_origensDestinos.GridRotaDestino.id, header, menuOpcoes, { column: 1, dir: orderDir.asc });

    new BuscarRotasFrete(_origensDestinos.RotaDestino, null, _gridRotaDestino, null, null, null, ValidarDestinosDisponiveis);

    _origensDestinos.RotaDestino.basicTable = _gridRotaDestino;
    _origensDestinos.RotaDestino.basicTable.CarregarGrid(new Array());
}

function LoadClienteDestino() {
    var menuOpcoes = {
        tipo: TypeOptionMenu.link, tamanho: 7, opcoes: [{
            descricao: "Excluir", id: guid(), metodo: function (data) {
                ExcluirDestinoClick(_origensDestinos.ClienteDestino, data);
            }
        }]
    };

    var header = [
        { data: "Codigo", visible: false },
        { data: "Descricao", title: "Descrição", width: "80%" }
    ];

    _gridClienteDestino = new BasicDataTable(_origensDestinos.GridClienteDestino.id, header, menuOpcoes, { column: 1, dir: orderDir.asc });

    new BuscarClientes(_origensDestinos.ClienteDestino, null, false, null, null, _gridClienteDestino, null, null, null, null, ValidarDestinosDisponiveis);

    _origensDestinos.ClienteDestino.basicTable = _gridClienteDestino;
    _origensDestinos.ClienteDestino.basicTable.CarregarGrid(new Array());
}

function LoadPaisDestino() {
    var menuOpcoes = {
        tipo: TypeOptionMenu.link, tamanho: 7, opcoes: [{
            descricao: "Excluir", id: guid(), metodo: function (data) {
                ExcluirDestinoClick(_origensDestinos.PaisDestino, data);
            }
        }]
    };

    var header = [
        { data: "Codigo", visible: false },
        { data: "Descricao", title: "Descrição", width: "80%" }
    ];

    _gridPaisDestino = new BasicDataTable(_origensDestinos.GridPaisDestino.id, header, menuOpcoes, { column: 1, dir: orderDir.asc });

    new BuscarPaises(_origensDestinos.PaisDestino, null, null, null, _gridPaisDestino, ValidarDestinosDisponiveis);

    _origensDestinos.PaisDestino.basicTable = _gridPaisDestino;
    _origensDestinos.PaisDestino.basicTable.CarregarGrid(new Array());
}

function ExcluirDestinoClick(knoutDestino, data) {
    var origemGrid = knoutDestino.basicTable.BuscarRegistros();

    for (var i = 0; i < origemGrid.length; i++) {
        if (data.Codigo == origemGrid[i].Codigo) {
            origemGrid.splice(i, 1);
            break;
        }
    }

    _camposRota.QuilometragemMedia.val("");
    knoutDestino.basicTable.CarregarGrid(origemGrid);

    ValidarDestinosDisponiveis(false);
}

//#region Faixa de CEP

function LoadFaixaCEPDestino() {
    var menuOpcoes = { tipo: TypeOptionMenu.link, tamanho: 7, opcoes: [{ descricao: "Excluir", id: guid(), metodo: ExcluirCEPDestinoClick }] };

    var header = [
        { data: "Codigo", visible: false },
        { data: "Descricao", title: "Descrição", width: "80%" }
    ];

    _gridCEPDestino = new BasicDataTable(_origensDestinos.GridCEPDestino.id, header, menuOpcoes, { column: 1, dir: orderDir.asc });

    RecarregarGridCEPDestino();
}

function RecarregarGridCEPDestino() {

    var data = new Array();

    $.each(_origensDestinos.CEPsDestino.val(), function (i, cep) {
        var cepGrid = new Object();

        cepGrid.Codigo = cep.Codigo;
        cepGrid.Descricao = "De " + cep.CEPInicial + " até " + cep.CEPFinal;

        data.push(cepGrid);
    });

    _gridCEPDestino.CarregarGrid(data);
}

function ExcluirCEPDestinoClick(data) {
    var cepsDestino = _origensDestinos.CEPsDestino.val();

    for (var i = 0; i < cepsDestino.length; i++) {
        if (data.Codigo == cepsDestino[i].Codigo) {
            cepsDestino.splice(i, 1);
            break;
        }
    }

    RecarregarGridCEPDestino();
    ValidarDestinosDisponiveis(false);
}

function AdicionarCEPDestinoClick(e, sender) {

    var cepInicial = Globalize.parseInt(string.OnlyNumbers(_origensDestinos.CEPDestinoInicial.val()));
    var cepFinal = Globalize.parseInt(string.OnlyNumbers(_origensDestinos.CEPDestinoFinal.val()));

    if (cepFinal.length < 8 || cepInicial.length < 8) {
        return;
    }

    if (isNaN(cepInicial) || isNaN(cepFinal)) {
        exibirMensagem(tipoMensagem.aviso, "Faixa de CEP inválida", "Informe uma faixa de CEP válida.");
        return;
    }

    if (cepFinal < cepInicial) {
        exibirMensagem(tipoMensagem.aviso, "Faixa de CEP inválida", "O CEP inicial não pode ser menor que o CEP final.");
        return;
    }

    var cepsDestino = _origensDestinos.CEPsDestino.val();

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

    cepsDestino.push({ Codigo: guid(), CEPInicial: _origensDestinos.CEPDestinoInicial.val(), CEPFinal: _origensDestinos.CEPDestinoFinal.val() });
    _origensDestinos.CEPsDestino.val(cepsDestino);

    RecarregarGridCEPDestino();

    LimparCamposCEPDestino();

    ValidarDestinosDisponiveis();
}

function LimparCamposCEPDestino() {
    _origensDestinos.CEPDestinoInicial.val("");
    _origensDestinos.CEPDestinoFinal.val("");
}

//#endregion Faixa de CEP

function ZerarTabelasDestino() {
    _origensDestinos.ClienteDestino.basicTable.SetarRegistros([]);
    _origensDestinos.Destino.basicTable.SetarRegistros([]);
    _origensDestinos.EstadoDestino.basicTable.SetarRegistros([]);
    _origensDestinos.RegiaoDestino.basicTable.SetarRegistros([]);
    _origensDestinos.RotaDestino.basicTable.SetarRegistros([]);
    _origensDestinos.CEPsDestino.val([]);
    _origensDestinos.PaisDestino.basicTable.SetarRegistros([]);

    _origensDestinos.ClienteDestino.basicTable.CarregarGrid([]);
    _origensDestinos.Destino.basicTable.CarregarGrid([]);
    _origensDestinos.EstadoDestino.basicTable.CarregarGrid([]);
    _origensDestinos.RegiaoDestino.basicTable.CarregarGrid([]);
    _origensDestinos.RotaDestino.basicTable.CarregarGrid([]);
    _origensDestinos.PaisDestino.basicTable.CarregarGrid([]);

    RecarregarGridCEPDestino();
}

function ValidarDestinosDisponiveis(reposicionarTabPrincipal) {
    if (_origensDestinos.ClienteDestino.basicTable.BuscarRegistros().length > 0) {
        flagDestino = "Cliente";
        $("#liTabCidadesDestino").hide();
        $("#liTabClientesDestino").show();
        $("#liTabEstadosDestino").hide();
        $("#liTabRegioesDestino").hide();
        $("#liTabRotasDestino").hide();
        $("#liTabCEPsDestino").hide();
        $("#liTabPaisesDestino").hide();
        $(".nav-tabs a[href='#tabClientesDestino']").tab('show');

        CalculoQuilometragemMediaClienteDestino(_origensDestinos.ClienteDestino.basicTable.BuscarCodigosRegistros())
    } else if (_origensDestinos.Destino.basicTable.BuscarRegistros().length > 0) {
        flagDestino = "Cidade";
        $("#liTabCidadesDestino").show();
        $("#liTabClientesDestino").hide();
        $("#liTabEstadosDestino").hide();
        $("#liTabRegioesDestino").hide();
        $("#liTabRotasDestino").hide();
        $("#liTabCEPsDestino").hide();
        $("#liTabPaisesDestino").hide();
        $(".nav-tabs a[href='#tabCidadesDestino']").tab('show');

        CalculoQuilometragemMediaCidadeDestino(_origensDestinos.Destino.basicTable.BuscarCodigosRegistros());
    } else if (_origensDestinos.EstadoDestino.basicTable.BuscarRegistros().length > 0) {
        flagDestino = "Estado";
        $("#liTabCidadesDestino").hide();
        $("#liTabClientesDestino").hide();
        $("#liTabEstadosDestino").show();
        $("#liTabRegioesDestino").hide();
        $("#liTabRotasDestino").hide();
        $("#liTabCEPsDestino").hide();
        $("#liTabPaisesDestino").hide();
        $(".nav-tabs a[href='#tabEstadosDestino']").tab('show');
    } else if (_origensDestinos.RegiaoDestino.basicTable.BuscarRegistros().length > 0) {
        flagDestino = "Regiao";
        $("#liTabCidadesDestino").hide();
        $("#liTabClientesDestino").hide();
        $("#liTabEstadosDestino").hide();
        $("#liTabRegioesDestino").show();
        $("#liTabRotasDestino").hide();
        $("#liTabCEPsDestino").hide();
        $("#liTabPaisesDestino").hide();
        $(".nav-tabs a[href='#tabRegioesDestino']").tab('show');
    } else if (_origensDestinos.RotaDestino.basicTable.BuscarRegistros().length > 0) {
        flagDestino = "Rota";
        $("#liTabCidadesDestino").hide();
        $("#liTabClientesDestino").hide();
        $("#liTabEstadosDestino").hide();
        $("#liTabRegioesDestino").hide();
        $("#liTabRotasDestino").show();
        $("#liTabCEPsDestino").hide();
        $("#liTabPaisesDestino").hide();
        $(".nav-tabs a[href='#tabRotasDestino']").tab('show');

        CalculoQuilometragemMediaRotaDestino(_origensDestinos.RotaDestino.basicTable.BuscarCodigosRegistros());
    } else if (_origensDestinos.CEPsDestino.val().length > 0) {
        flagDestino = "CEP";
        $("#liTabCidadesDestino").hide();
        $("#liTabClientesDestino").hide();
        $("#liTabEstadosDestino").hide();
        $("#liTabRegioesDestino").hide();
        $("#liTabRotasDestino").hide();
        $("#liTabCEPsDestino").show();
        $("#liTabPaisesDestino").hide();
        $(".nav-tabs a[href='#tabCEPsDestino']").tab('show');
    } else if (_origensDestinos.PaisDestino.basicTable.BuscarRegistros().length > 0) {
        flagDestino = "Pais";
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
        $("#liTabRotasDestino").show();
        $("#liTabRegioesDestino").show();
        $("#liTabCEPsDestino").show();

        if (reposicionarTabPrincipal !== false)
            $(".nav-tabs a[href='#tabCidadesDestino']").tab('show');
    }
}