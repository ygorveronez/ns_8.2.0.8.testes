var _gridOrigem;
var _gridCEPOrigem;

function loadOrigens() {
    LoadLocalidadeOrigem();
    LoadEstadoOrigem();
    LoadRegiaoOrigem();
    LoadClienteOrigem();
    LoadFaixaCEPOrigem();
    LoadRotaOrigem();
    LoadPaisOrigem();
}

function LoadLocalidadeOrigem() {
    var menuOpcoes = {
        tipo: TypeOptionMenu.link, tamanho: 7, opcoes: [{
            descricao: "Excluir", id: guid(), metodo: function (data) {
                ExcluirOrigemClick(_origensDestinos.Origem, data);
            }
        }]
    };

    var header = [
        { data: "Codigo", visible: false },
        { data: "Descricao", title: "Descrição", width: "80%" }
    ];

    _gridOrigem = new BasicDataTable(_origensDestinos.GridOrigem.id, header, menuOpcoes, { column: 1, dir: orderDir.asc });

    new BuscarLocalidades(_origensDestinos.Origem, null, null, null, _gridOrigem, ValidarOrigensDisponiveis);

    _origensDestinos.Origem.basicTable = _gridOrigem;
    _origensDestinos.Origem.basicTable.CarregarGrid(new Array());
}

function LoadEstadoOrigem() {
    var menuOpcoes = {
        tipo: TypeOptionMenu.link, tamanho: 7, opcoes: [{
            descricao: "Excluir", id: guid(), metodo: function (data) {
                ExcluirOrigemClick(_origensDestinos.EstadoOrigem, data);
            }
        }]
    };

    var header = [
        { data: "Codigo", visible: false },
        { data: "Descricao", title: "Descrição", width: "80%" }
    ];

    _gridEstadoOrigem = new BasicDataTable(_origensDestinos.GridEstadoOrigem.id, header, menuOpcoes, { column: 1, dir: orderDir.asc });

    new BuscarEstados(_origensDestinos.EstadoOrigem, null, _gridEstadoOrigem, ValidarOrigensDisponiveis);

    _origensDestinos.EstadoOrigem.basicTable = _gridEstadoOrigem;
    _origensDestinos.EstadoOrigem.basicTable.CarregarGrid(new Array());
}

function LoadRegiaoOrigem() {
    var menuOpcoes = {
        tipo: TypeOptionMenu.link, tamanho: 7, opcoes: [{
            descricao: "Excluir", id: guid(), metodo: function (data) {
                ExcluirOrigemClick(_origensDestinos.RegiaoOrigem, data);
            }
        }]
    };

    var header = [
        { data: "Codigo", visible: false },
        { data: "Descricao", title: "Descrição", width: "80%" }
    ];

    _gridRegiaoOrigem = new BasicDataTable(_origensDestinos.GridRegiaoOrigem.id, header, menuOpcoes, { column: 1, dir: orderDir.asc });

    new BuscarRegioes(_origensDestinos.RegiaoOrigem, null, _gridRegiaoOrigem, ValidarOrigensDisponiveis);

    _origensDestinos.RegiaoOrigem.basicTable = _gridRegiaoOrigem;
    _origensDestinos.RegiaoOrigem.basicTable.CarregarGrid(new Array());
}

function LoadRotaOrigem() {
    var menuOpcoes = {
        tipo: TypeOptionMenu.link, tamanho: 7, opcoes: [{
            descricao: "Excluir", id: guid(), metodo: function (data) {
                ExcluirOrigemClick(_origensDestinos.RotaOrigem, data);
            }
        }]
    };

    var header = [
        { data: "Codigo", visible: false },
        { data: "Descricao", title: "Descrição", width: "80%" }
    ];

    _gridRotaOrigem = new BasicDataTable(_origensDestinos.GridRotaOrigem.id, header, menuOpcoes, { column: 1, dir: orderDir.asc });

    new BuscarRotasFrete(_origensDestinos.RotaOrigem, null, _gridRotaOrigem, null, null, null, ValidarOrigensDisponiveis);

    _origensDestinos.RotaOrigem.basicTable = _gridRotaOrigem;
    _origensDestinos.RotaOrigem.basicTable.CarregarGrid(new Array());
}

function LoadClienteOrigem() {
    var menuOpcoes = {
        tipo: TypeOptionMenu.link, tamanho: 7, opcoes: [{
            descricao: "Excluir", id: guid(), metodo: function (data) {
                ExcluirOrigemClick(_origensDestinos.ClienteOrigem, data);
            }
        }]
    };

    var header = [
        { data: "Codigo", visible: false },
        { data: "Descricao", title: "Descrição", width: "80%" }
    ];

    _gridClienteOrigem = new BasicDataTable(_origensDestinos.GridClienteOrigem.id, header, menuOpcoes, { column: 1, dir: orderDir.asc });

    new BuscarClientes(_origensDestinos.ClienteOrigem, null, false, null, null, _gridClienteOrigem, null, null, null, null, ValidarOrigensDisponiveis);

    _origensDestinos.ClienteOrigem.basicTable = _gridClienteOrigem;
    _origensDestinos.ClienteOrigem.basicTable.CarregarGrid(new Array());
}

function LoadPaisOrigem() {
    var menuOpcoes = {
        tipo: TypeOptionMenu.link, tamanho: 7, opcoes: [{
            descricao: "Excluir", id: guid(), metodo: function (data) {
                ExcluirOrigemClick(_origensDestinos.PaisOrigem, data);
            }
        }]
    };

    var header = [
        { data: "Codigo", visible: false },
        { data: "Descricao", title: "Descrição", width: "80%" }
    ];

    _gridPaisOrigem = new BasicDataTable(_origensDestinos.GridPaisOrigem.id, header, menuOpcoes, { column: 1, dir: orderDir.asc });

    new BuscarPaises(_origensDestinos.PaisOrigem, null, null, null, _gridPaisOrigem, ValidarOrigensDisponiveis);

    _origensDestinos.PaisOrigem.basicTable = _gridPaisOrigem;
    _origensDestinos.PaisOrigem.basicTable.CarregarGrid(new Array());
}

function LoadFaixaCEPOrigem() {
    var menuOpcoes = { tipo: TypeOptionMenu.link, tamanho: 7, opcoes: [{ descricao: "Excluir", id: guid(), metodo: ExcluirCEPOrigemClick }] };

    var header = [
        { data: "Codigo", visible: false },
        { data: "Descricao", title: "Descrição", width: "80%" }
    ];

    _gridCEPOrigem = new BasicDataTable(_origensDestinos.GridCEPOrigem.id, header, menuOpcoes, { column: 1, dir: orderDir.asc });

    RecarregarGridCEPOrigem();
}

function ValidarOrigensDisponiveis(reposicionarTabPrincipal) {
    if (_origensDestinos.ClienteOrigem.basicTable.BuscarRegistros().length > 0) {
        flagOrigem = "Cliente";
        $("#liTabCidadesOrigem").hide();
        $("#liTabClientesOrigem").show();
        $("#liTabEstadosOrigem").hide();
        $("#liTabRegioesOrigem").hide();
        $("#liTabCEPsOrigem").hide();
        $("#liTabRotasOrigem").hide();
        $("#liTabPaisesOrigem").hide();
        $(".nav-tabs a[href='#tabClientesOrigem']").tab('show');

        CalculoQuilometragemMediaClienteOrigem(_origensDestinos.ClienteOrigem.basicTable.BuscarCodigosRegistros());
    } else if (_origensDestinos.Origem.basicTable.BuscarRegistros().length > 0) {
        flagOrigem = "Cidade";
        $("#liTabCidadesOrigem").show();
        $("#liTabClientesOrigem").hide();
        $("#liTabEstadosOrigem").hide();
        $("#liTabRegioesOrigem").hide();
        $("#liTabCEPsOrigem").hide();
        $("#liTabRotasOrigem").hide();
        $("#liTabPaisesOrigem").hide();
        $(".nav-tabs a[href='#tabCidadesOrigem']").tab('show');

        CalculoQuilometragemMediaCidadeOrigem(_origensDestinos.Origem.basicTable.BuscarCodigosRegistros());
    } else if (_origensDestinos.EstadoOrigem.basicTable.BuscarRegistros().length > 0) {
        flagOrigem = "Estado";
        $("#liTabCidadesOrigem").hide();
        $("#liTabClientesOrigem").hide();
        $("#liTabEstadosOrigem").show();
        $("#liTabRegioesOrigem").hide();
        $("#liTabCEPsOrigem").hide();
        $("#liTabRotasOrigem").hide();
        $("#liTabPaisesOrigem").hide();
        $(".nav-tabs a[href='#tabEstadosOrigem']").tab('show');
    } else if (_origensDestinos.RegiaoOrigem.basicTable.BuscarRegistros().length > 0) {
        flagOrigem = "Regiao";
        $("#liTabCidadesOrigem").hide();
        $("#liTabClientesOrigem").hide();
        $("#liTabEstadosOrigem").hide();
        $("#liTabRegioesOrigem").show();
        $("#liTabCEPsOrigem").hide();
        $("#liTabRotasOrigem").hide();
        $("#liTabPaisesOrigem").hide();
        $(".nav-tabs a[href='#tabRegioesOrigem']").tab('show');
    } else if (_origensDestinos.RotaOrigem.basicTable.BuscarRegistros().length > 0) {
        flagOrigem = "Rota";
        $("#liTabCidadesOrigem").hide();
        $("#liTabClientesOrigem").hide();
        $("#liTabEstadosOrigem").hide();
        $("#liTabRegioesOrigem").hide();
        $("#liTabCEPsOrigem").hide();
        $("#liTabRotasOrigem").show();
        $("#liTabPaisesOrigem").hide();
        $(".nav-tabs a[href='#tabRotasOrigem']").tab('show');

        CalculoQuilometragemMediaRotaOrigem(_origensDestinos.RotaOrigem.basicTable.BuscarCodigosRegistros());
    } else if (_origensDestinos.CEPsOrigem.val().length > 0) {
        flagOrigem = "CEP";
        $("#liTabCidadesOrigem").hide();
        $("#liTabClientesOrigem").hide();
        $("#liTabEstadosOrigem").hide();
        $("#liTabRegioesOrigem").hide();
        $("#liTabCEPsOrigem").show();
        $("#liTabRotasOrigem").hide();
        $("#liTabPaisesOrigem").hide();
        $(".nav-tabs a[href='#tabCEPsOrigem']").tab('show');
    } else if (_origensDestinos.PaisOrigem.basicTable.BuscarRegistros().length > 0) {
        flagOrigem = "Pais";
        $("#liTabCidadesOrigem").hide();
        $("#liTabClientesOrigem").hide();
        $("#liTabEstadosOrigem").hide();
        $("#liTabRegioesOrigem").hide();
        $("#liTabCEPsOrigem").hide();
        $("#liTabRotasOrigem").hide();
        $("#liTabPaisesOrigem").show();
        $(".nav-tabs a[href='#tabPaisesOrigem']").tab('show');
    } else {
        $("#liTabCidadesOrigem").show();
        $("#liTabClientesOrigem").show();
        $("#liTabEstadosOrigem").show();
        $("#liTabPaisesOrigem").show();
        $("#liTabRotasOrigem").show();
        $("#liTabRegioesOrigem").show();
        $("#liTabCEPsOrigem").show();

        if (reposicionarTabPrincipal !== false)
            $(".nav-tabs a[href='#tabCidadesOrigem']").tab('show');
    }
}

function ExcluirOrigemClick(knoutOrigem, data) {
    var origemGrid = knoutOrigem.basicTable.BuscarRegistros();

    for (var i = 0; i < origemGrid.length; i++) {
        if (data.Codigo == origemGrid[i].Codigo) {
            origemGrid.splice(i, 1);
            break;
        }
    }

    _camposRota.QuilometragemMedia.val("");
    knoutOrigem.basicTable.CarregarGrid(origemGrid);

    ValidarOrigensDisponiveis(false);
}

function ExcluirCEPOrigemClick(data) {
    var cepsOrigem = _origensDestinos.CEPsOrigem.val();

    for (var i = 0; i < cepsOrigem.length; i++) {
        if (data.Codigo == cepsOrigem[i].Codigo) {
            cepsOrigem.splice(i, 1);
            break;
        }
    }

    _origensDestinos.CEPsOrigem.val(cepsOrigem);

    RecarregarGridCEPOrigem();
    ValidarOrigensDisponiveis(false);
}

function RecarregarGridCEPOrigem() {

    var data = new Array();

    $.each(_origensDestinos.CEPsOrigem.val(), function (i, cep) {
        var cepGrid = new Object();

        cepGrid.Codigo = cep.Codigo;
        cepGrid.Descricao = "De " + cep.CEPInicial + " até " + cep.CEPFinal;

        data.push(cepGrid);
    });

    _gridCEPOrigem.CarregarGrid(data);
}

function AdicionarCEPOrigemClick(e, sender) {

    var cepInicial = Globalize.parseInt(string.OnlyNumbers(_origensDestinos.CEPOrigemInicial.val()));
    var cepFinal = Globalize.parseInt(string.OnlyNumbers(_origensDestinos.CEPOrigemFinal.val()));

    if (isNaN(cepInicial) || isNaN(cepFinal)) {
        exibirMensagem(tipoMensagem.aviso, "Faixa de CEP inválida", "Informe uma faixa de CEP válida.");
        return;
    }

    if (cepFinal < cepInicial) {
        exibirMensagem(tipoMensagem.aviso, "Faixa de CEP inválida", "O CEP inicial não pode ser menor que o CEP final.");
        return;
    }

    var cepsOrigem = _origensDestinos.CEPsOrigem.val();

    if (cepsOrigem.some(function (item) {
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

    cepsOrigem.push({ Codigo: guid(), CEPInicial: _origensDestinos.CEPOrigemInicial.val(), CEPFinal: _origensDestinos.CEPOrigemFinal.val() })

    _origensDestinos.CEPsOrigem.val(cepsOrigem);

    RecarregarGridCEPOrigem();

    LimparCamposCEPOrigem();

    ValidarOrigensDisponiveis();
}

function LimparCamposCEPOrigem() {
    _origensDestinos.CEPOrigemInicial.val("");
    _origensDestinos.CEPOrigemFinal.val("");
}

function ZerarTabelasOrigem() {
    _origensDestinos.ClienteOrigem.basicTable.SetarRegistros([]);
    _origensDestinos.Origem.basicTable.SetarRegistros([]);
    _origensDestinos.EstadoOrigem.basicTable.SetarRegistros([]);
    _origensDestinos.RegiaoOrigem.basicTable.SetarRegistros([]);
    _origensDestinos.RotaOrigem.basicTable.SetarRegistros([]);
    _origensDestinos.CEPsOrigem.val([]);
    _origensDestinos.PaisOrigem.basicTable.SetarRegistros([]);

    _origensDestinos.ClienteOrigem.basicTable.CarregarGrid([]);
    _origensDestinos.Origem.basicTable.CarregarGrid([]);
    _origensDestinos.EstadoOrigem.basicTable.CarregarGrid([]);
    _origensDestinos.RegiaoOrigem.basicTable.CarregarGrid([]);
    _origensDestinos.RotaOrigem.basicTable.CarregarGrid([]);
    _origensDestinos.PaisOrigem.basicTable.CarregarGrid([]);

    RecarregarGridCEPOrigem();
}