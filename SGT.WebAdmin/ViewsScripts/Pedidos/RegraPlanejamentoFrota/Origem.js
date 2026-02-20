//*******MAPEAMENTO KNOUCKOUT*******

var _gridOrigem, _gridClienteOrigem, _gridEstadoOrigem, _gridRegiaoOrigem, _gridRotaOrigem, _gridCEPOrigem, _gridPaisOrigem;

//*******EVENTOS*******

function LoadOrigens() {
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
                ExcluirOrigemClick(_regraPlanejamentoFrota.Origem, data);
            }
        }]
    };

    var header = [
        { data: "Codigo", visible: false },
        { data: "Descricao", title: "Descrição", width: "80%" }
    ];

    _gridOrigem = new BasicDataTable(_regraPlanejamentoFrota.GridOrigem.id, header, menuOpcoes, { column: 1, dir: orderDir.asc });

    new BuscarLocalidades(_regraPlanejamentoFrota.Origem, null, null, null, _gridOrigem, ValidarOrigensDisponiveis);

    _regraPlanejamentoFrota.Origem.basicTable = _gridOrigem;
    _regraPlanejamentoFrota.Origem.basicTable.CarregarGrid(new Array());
}

function LoadEstadoOrigem() {
    var menuOpcoes = {
        tipo: TypeOptionMenu.link, tamanho: 7, opcoes: [{
            descricao: "Excluir", id: guid(), metodo: function (data) {
                ExcluirOrigemClick(_regraPlanejamentoFrota.EstadoOrigem, data);
            }
        }]
    };

    var header = [
        { data: "Codigo", visible: false },
        { data: "Descricao", title: "Descrição", width: "80%" }
    ];

    _gridEstadoOrigem = new BasicDataTable(_regraPlanejamentoFrota.GridEstadoOrigem.id, header, menuOpcoes, { column: 1, dir: orderDir.asc });

    new BuscarEstados(_regraPlanejamentoFrota.EstadoOrigem, null, _gridEstadoOrigem, ValidarOrigensDisponiveis);

    _regraPlanejamentoFrota.EstadoOrigem.basicTable = _gridEstadoOrigem;
    _regraPlanejamentoFrota.EstadoOrigem.basicTable.CarregarGrid(new Array());
}

function LoadRegiaoOrigem() {
    var menuOpcoes = {
        tipo: TypeOptionMenu.link, tamanho: 7, opcoes: [{
            descricao: "Excluir", id: guid(), metodo: function (data) {
                ExcluirOrigemClick(_regraPlanejamentoFrota.RegiaoOrigem, data);
            }
        }]
    };

    var header = [
        { data: "Codigo", visible: false },
        { data: "Descricao", title: "Descrição", width: "80%" }
    ];

    _gridRegiaoOrigem = new BasicDataTable(_regraPlanejamentoFrota.GridRegiaoOrigem.id, header, menuOpcoes, { column: 1, dir: orderDir.asc });

    new BuscarRegioes(_regraPlanejamentoFrota.RegiaoOrigem, null, _gridRegiaoOrigem, ValidarOrigensDisponiveis);

    _regraPlanejamentoFrota.RegiaoOrigem.basicTable = _gridRegiaoOrigem;
    _regraPlanejamentoFrota.RegiaoOrigem.basicTable.CarregarGrid(new Array());
}

function LoadRotaOrigem() {
    var menuOpcoes = {
        tipo: TypeOptionMenu.link, tamanho: 7, opcoes: [{
            descricao: "Excluir", id: guid(), metodo: function (data) {
                ExcluirOrigemClick(_regraPlanejamentoFrota.RotaOrigem, data);
            }
        }]
    };

    var header = [
        { data: "Codigo", visible: false },
        { data: "Descricao", title: "Descrição", width: "80%" }
    ];

    _gridRotaOrigem = new BasicDataTable(_regraPlanejamentoFrota.GridRotaOrigem.id, header, menuOpcoes, { column: 1, dir: orderDir.asc });

    new BuscarRotasFrete(_regraPlanejamentoFrota.RotaOrigem, null, _gridRotaOrigem, null, null, null, ValidarOrigensDisponiveis, null, _regraPlanejamentoFrota.TabelaFrete);

    _regraPlanejamentoFrota.RotaOrigem.basicTable = _gridRotaOrigem;
    _regraPlanejamentoFrota.RotaOrigem.basicTable.CarregarGrid(new Array());
}

function LoadClienteOrigem() {
    var menuOpcoes = {
        tipo: TypeOptionMenu.link, tamanho: 7, opcoes: [{
            descricao: "Excluir", id: guid(), metodo: function (data) {
                ExcluirOrigemClick(_regraPlanejamentoFrota.ClienteOrigem, data);
            }
        }]
    };

    var header = [
        { data: "Codigo", visible: false },
        { data: "Descricao", title: "Descrição", width: "80%" }
    ];

    _gridClienteOrigem = new BasicDataTable(_regraPlanejamentoFrota.GridClienteOrigem.id, header, menuOpcoes, { column: 1, dir: orderDir.asc });

    new BuscarClientes(_regraPlanejamentoFrota.ClienteOrigem, null, false, null, null, _gridClienteOrigem, null, null, null, null, ValidarOrigensDisponiveis);

    _regraPlanejamentoFrota.ClienteOrigem.basicTable = _gridClienteOrigem;
    _regraPlanejamentoFrota.ClienteOrigem.basicTable.CarregarGrid(new Array());
}

function LoadPaisOrigem() {
    var menuOpcoes = {
        tipo: TypeOptionMenu.link, tamanho: 7, opcoes: [{
            descricao: "Excluir", id: guid(), metodo: function (data) {
                ExcluirOrigemClick(_regraPlanejamentoFrota.PaisOrigem, data);
            }
        }]
    };

    var header = [
        { data: "Codigo", visible: false },
        { data: "Descricao", title: "Descrição", width: "80%" }
    ];

    _gridPaisOrigem = new BasicDataTable(_regraPlanejamentoFrota.GridPaisOrigem.id, header, menuOpcoes, { column: 1, dir: orderDir.asc });

    new BuscarPaises(_regraPlanejamentoFrota.PaisOrigem, null, null, null, _gridPaisOrigem, ValidarOrigensDisponiveis);

    _regraPlanejamentoFrota.PaisOrigem.basicTable = _gridPaisOrigem;
    _regraPlanejamentoFrota.PaisOrigem.basicTable.CarregarGrid(new Array());
}

function ExcluirOrigemClick(knoutOrigem, data) {
    var origemGrid = knoutOrigem.basicTable.BuscarRegistros();

    for (var i = 0; i < origemGrid.length; i++) {
        if (data.Codigo == origemGrid[i].Codigo) {
            origemGrid.splice(i, 1);
            break;
        }
    }

    knoutOrigem.basicTable.CarregarGrid(origemGrid);

    ValidarOrigensDisponiveis(false);
}

//#region Faixa de CEP

function LoadFaixaCEPOrigem() {
    var menuOpcoes = { tipo: TypeOptionMenu.link, tamanho: 7, opcoes: [{ descricao: "Excluir", id: guid(), metodo: ExcluirCEPOrigemClick }] };

    var header = [
        { data: "Codigo", visible: false },
        { data: "Descricao", title: "Descrição", width: "80%" }
    ];

    _gridCEPOrigem = new BasicDataTable(_regraPlanejamentoFrota.GridCEPOrigem.id, header, menuOpcoes, { column: 1, dir: orderDir.asc });

    RecarregarGridCEPOrigem();
}

function RecarregarGridCEPOrigem() {

    var data = new Array();

    $.each(_regraPlanejamentoFrota.CEPsOrigem.val(), function (i, cep) {
        var cepGrid = new Object();

        cepGrid.Codigo = cep.Codigo;
        cepGrid.Descricao = "De " + cep.CEPInicial + " até " + cep.CEPFinal;

        data.push(cepGrid);
    });

    _gridCEPOrigem.CarregarGrid(data);
}

function ExcluirCEPOrigemClick(data) {
    var cepsOrigem = _regraPlanejamentoFrota.CEPsOrigem.val();

    for (var i = 0; i < cepsOrigem.length; i++) {
        if (data.Codigo == cepsOrigem[i].Codigo) {
            cepsOrigem.splice(i, 1);
            break;
        }
    }

    _regraPlanejamentoFrota.CEPsOrigem.val(cepsOrigem);

    RecarregarGridCEPOrigem();
    ValidarOrigensDisponiveis(false);
}

function AdicionarCEPOrigemClick(e, sender) {

    var cepInicial = Globalize.parseInt(string.OnlyNumbers(_regraPlanejamentoFrota.CEPOrigemInicial.val()));
    var cepFinal = Globalize.parseInt(string.OnlyNumbers(_regraPlanejamentoFrota.CEPOrigemFinal.val()));

    if (isNaN(cepInicial) || isNaN(cepFinal)) {
        exibirMensagem(tipoMensagem.aviso, "Faixa de CEP inválida", "Informe uma faixa de CEP válida.");
        return;
    }

    if (cepFinal < cepInicial) {
        exibirMensagem(tipoMensagem.aviso, "Faixa de CEP inválida", "O CEP inicial não pode ser menor que o CEP final.");
        return;
    }

    var cepsOrigem = _regraPlanejamentoFrota.CEPsOrigem.val();

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

    cepsOrigem.push({ Codigo: guid(), CEPInicial: _regraPlanejamentoFrota.CEPOrigemInicial.val(), CEPFinal: _regraPlanejamentoFrota.CEPOrigemFinal.val() })

    _regraPlanejamentoFrota.CEPsOrigem.val(cepsOrigem);

    RecarregarGridCEPOrigem();

    LimparCamposCEPOrigem();

    ValidarOrigensDisponiveis();
}

function LimparCamposCEPOrigem() {
    _regraPlanejamentoFrota.CEPOrigemInicial.val("");
    _regraPlanejamentoFrota.CEPOrigemFinal.val("");
}

//#endregion Faixa de CEP

function ValidarOrigensDisponiveis(reposicionarTabPrincipal) {
    if (_regraPlanejamentoFrota.ClienteOrigem.basicTable.BuscarRegistros().length > 0) {
        $("#liTabCidadesOrigem").hide();
        $("#liTabClientesOrigem").show();
        $("#liTabEstadosOrigem").hide();
        $("#liTabRegioesOrigem").hide();
        $("#liTabCEPsOrigem").hide();
        $("#liTabRotasOrigem").hide();
        $("#liTabPaisesOrigem").hide();
        $(".nav-tabs a[href='#tabClientesOrigem']").tab('show');
    } else if (_regraPlanejamentoFrota.Origem.basicTable.BuscarRegistros().length > 0) {
        $("#liTabCidadesOrigem").show();
        $("#liTabClientesOrigem").hide();
        $("#liTabEstadosOrigem").hide();
        $("#liTabRegioesOrigem").hide();
        $("#liTabCEPsOrigem").hide();
        $("#liTabRotasOrigem").hide();
        $("#liTabPaisesOrigem").hide();
        $(".nav-tabs a[href='#tabCidadesOrigem']").tab('show');
    } else if (_regraPlanejamentoFrota.EstadoOrigem.basicTable.BuscarRegistros().length > 0) {
        $("#liTabCidadesOrigem").hide();
        $("#liTabClientesOrigem").hide();
        $("#liTabEstadosOrigem").show();
        $("#liTabRegioesOrigem").hide();
        $("#liTabCEPsOrigem").hide();
        $("#liTabRotasOrigem").hide();
        $("#liTabPaisesOrigem").hide();
        $(".nav-tabs a[href='#tabEstadosOrigem']").tab('show');
    } else if (_regraPlanejamentoFrota.RegiaoOrigem.basicTable.BuscarRegistros().length > 0) {
        $("#liTabCidadesOrigem").hide();
        $("#liTabClientesOrigem").hide();
        $("#liTabEstadosOrigem").hide();
        $("#liTabRegioesOrigem").show();
        $("#liTabCEPsOrigem").hide();
        $("#liTabRotasOrigem").hide();
        $("#liTabPaisesOrigem").hide();
        $(".nav-tabs a[href='#tabRegioesOrigem']").tab('show');
    } else if (_regraPlanejamentoFrota.RotaOrigem.basicTable.BuscarRegistros().length > 0) {
        $("#liTabCidadesOrigem").hide();
        $("#liTabClientesOrigem").hide();
        $("#liTabEstadosOrigem").hide();
        $("#liTabRegioesOrigem").hide();
        $("#liTabCEPsOrigem").hide();
        $("#liTabRotasOrigem").show();
        $("#liTabPaisesOrigem").hide();
        $(".nav-tabs a[href='#tabRotasOrigem']").tab('show');
    } else if (_regraPlanejamentoFrota.CEPsOrigem.val().length > 0) {
        $("#liTabCidadesOrigem").hide();
        $("#liTabClientesOrigem").hide();
        $("#liTabEstadosOrigem").hide();
        $("#liTabRegioesOrigem").hide();
        $("#liTabCEPsOrigem").show();
        $("#liTabRotasOrigem").hide();
        $("#liTabPaisesOrigem").hide();
        $(".nav-tabs a[href='#tabCEPsOrigem']").tab('show');
    } else if (_regraPlanejamentoFrota.PaisOrigem.basicTable.BuscarRegistros().length > 0) {
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

        if (_CONFIGURACAO_TMS.TipoServicoMultisoftware == EnumTipoServicoMultisoftware.MultiEmbarcador) {
            $("#liTabRotasOrigem").show();
            $("#liTabRegioesOrigem").hide();
        }
        else {
            $("#liTabRotasOrigem").show();
            $("#liTabRegioesOrigem").show();
        }

        $("#liTabCEPsOrigem").show();

        if (reposicionarTabPrincipal !== false)
            $(".nav-tabs a[href='#tabCidadesOrigem']").tab('show');
    }
}