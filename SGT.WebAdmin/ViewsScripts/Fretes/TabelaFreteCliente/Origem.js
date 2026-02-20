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
            descricao: Localization.Resources.Fretes.TabelaFreteCliente.Excluir, id: guid(), metodo: function (data) {
                ExcluirOrigemClick(_tabelaFreteCliente.Origem, data);
            }
        }]
    };

    var header = [
        { data: "Codigo", visible: false },
        { data: "Descricao", title: Localization.Resources.Fretes.TabelaFreteCliente.Descricao, width: "80%" }
    ];

    _gridOrigem = new BasicDataTable(_tabelaFreteCliente.GridOrigem.id, header, menuOpcoes, { column: 1, dir: orderDir.asc });

    new BuscarLocalidades(_tabelaFreteCliente.Origem, null, null, null, _gridOrigem, ValidarOrigensDisponiveis);

    _tabelaFreteCliente.Origem.basicTable = _gridOrigem;
    _tabelaFreteCliente.Origem.basicTable.CarregarGrid(new Array());
}

function LoadEstadoOrigem() {
    var menuOpcoes = {
        tipo: TypeOptionMenu.link, tamanho: 7, opcoes: [{
            descricao: Localization.Resources.Fretes.TabelaFreteCliente.Excluir, id: guid(), metodo: function (data) {
                ExcluirOrigemClick(_tabelaFreteCliente.EstadoOrigem, data);
            }
        }]
    };

    var header = [
        { data: "Codigo", visible: false },
        { data: "Descricao", title: Localization.Resources.Fretes.TabelaFreteCliente.Descricao, width: "80%" }
    ];

    _gridEstadoOrigem = new BasicDataTable(_tabelaFreteCliente.GridEstadoOrigem.id, header, menuOpcoes, { column: 1, dir: orderDir.asc });

    new BuscarEstados(_tabelaFreteCliente.EstadoOrigem, null, _gridEstadoOrigem, ValidarOrigensDisponiveis);

    _tabelaFreteCliente.EstadoOrigem.basicTable = _gridEstadoOrigem;
    _tabelaFreteCliente.EstadoOrigem.basicTable.CarregarGrid(new Array());
}

function LoadRegiaoOrigem() {
    var menuOpcoes = {
        tipo: TypeOptionMenu.link, tamanho: 7, opcoes: [{
            descricao: Localization.Resources.Fretes.TabelaFreteCliente.Excluir, id: guid(), metodo: function (data) {
                ExcluirOrigemClick(_tabelaFreteCliente.RegiaoOrigem, data);
            }
        }]
    };

    var header = [
        { data: "Codigo", visible: false },
        { data: "Descricao", title: Localization.Resources.Fretes.TabelaFreteCliente.Descricao, width: "80%" }
    ];

    _gridRegiaoOrigem = new BasicDataTable(_tabelaFreteCliente.GridRegiaoOrigem.id, header, menuOpcoes, { column: 1, dir: orderDir.asc });

    new BuscarRegioes(_tabelaFreteCliente.RegiaoOrigem, null, _gridRegiaoOrigem, ValidarOrigensDisponiveis);

    _tabelaFreteCliente.RegiaoOrigem.basicTable = _gridRegiaoOrigem;
    _tabelaFreteCliente.RegiaoOrigem.basicTable.CarregarGrid(new Array());
}

function LoadRotaOrigem() {
    var menuOpcoes = {
        tipo: TypeOptionMenu.link, tamanho: 7, opcoes: [{
            descricao: Localization.Resources.Fretes.TabelaFreteCliente.Excluir, id: guid(), metodo: function (data) {
                ExcluirOrigemClick(_tabelaFreteCliente.RotaOrigem, data);
            }
        }]
    };

    var header = [
        { data: "Codigo", visible: false },
        { data: "Descricao", title: Localization.Resources.Fretes.TabelaFreteCliente.Descricao, width: "80%" }
    ];

    _gridRotaOrigem = new BasicDataTable(_tabelaFreteCliente.GridRotaOrigem.id, header, menuOpcoes, { column: 1, dir: orderDir.asc });

    new BuscarRotasFrete(_tabelaFreteCliente.RotaOrigem, null, _gridRotaOrigem, null, null, null, ValidarOrigensDisponiveis, null, _tabelaFreteCliente.TabelaFrete);

    _tabelaFreteCliente.RotaOrigem.basicTable = _gridRotaOrigem;
    _tabelaFreteCliente.RotaOrigem.basicTable.CarregarGrid(new Array());
}

function LoadClienteOrigem() {
    var menuOpcoes = {
        tipo: TypeOptionMenu.link, tamanho: 7, opcoes: [{
            descricao: Localization.Resources.Fretes.TabelaFreteCliente.Excluir, id: guid(), metodo: function (data) {
                ExcluirOrigemClick(_tabelaFreteCliente.ClienteOrigem, data);
            }
        }]
    };

    var header = [
        { data: "Codigo", visible: false },
        { data: "Descricao", title: Localization.Resources.Fretes.TabelaFreteCliente.Descricao, width: "80%" }
    ];

    _gridClienteOrigem = new BasicDataTable(_tabelaFreteCliente.GridClienteOrigem.id, header, menuOpcoes, { column: 1, dir: orderDir.asc });

    new BuscarClientes(_tabelaFreteCliente.ClienteOrigem, retornoCliente, false, null, null, _gridClienteOrigem, null, null, null, null, ValidarOrigensDisponiveis);

    _tabelaFreteCliente.ClienteOrigem.basicTable = _gridClienteOrigem;
    _tabelaFreteCliente.ClienteOrigem.basicTable.CarregarGrid(new Array());
}

function LoadPaisOrigem() {
    var menuOpcoes = {
        tipo: TypeOptionMenu.link, tamanho: 7, opcoes: [{
            descricao: Localization.Resources.Fretes.TabelaFreteCliente.Excluir, id: guid(), metodo: function (data) {
                ExcluirOrigemClick(_tabelaFreteCliente.PaisOrigem, data);
            }
        }]
    };

    var header = [
        { data: "Codigo", visible: false },
        { data: "Descricao", title: Localization.Resources.Fretes.TabelaFreteCliente.Descricao, width: "80%" }
    ];

    _gridPaisOrigem = new BasicDataTable(_tabelaFreteCliente.GridPaisOrigem.id, header, menuOpcoes, { column: 1, dir: orderDir.asc });

    new BuscarPaises(_tabelaFreteCliente.PaisOrigem, null, null, null, _gridPaisOrigem, ValidarOrigensDisponiveis);

    _tabelaFreteCliente.PaisOrigem.basicTable = _gridPaisOrigem;
    _tabelaFreteCliente.PaisOrigem.basicTable.CarregarGrid(new Array());
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
    var menuOpcoes = { tipo: TypeOptionMenu.link, tamanho: 7, opcoes: [{ descricao: Localization.Resources.Fretes.TabelaFreteCliente.Excluir, id: guid(), metodo: ExcluirCEPOrigemClick }] };

    var header = [
        { data: "Codigo", visible: false },
        { data: "Descricao", title: Localization.Resources.Fretes.TabelaFreteCliente.Descricao, width: "80%" }
    ];

    _gridCEPOrigem = new BasicDataTable(_tabelaFreteCliente.GridCEPOrigem.id, header, menuOpcoes, { column: 1, dir: orderDir.asc });

    RecarregarGridCEPOrigem();
}

function RecarregarGridCEPOrigem() {

    var data = new Array();

    $.each(_tabelaFreteCliente.CEPsOrigem.val(), function (i, cep) {
        var cepGrid = new Object();

        cepGrid.Codigo = cep.Codigo;
        cepGrid.Descricao = Localization.Resources.Fretes.TabelaFreteCliente.DeAte.format(cep.CEPInicial, cep.CEPFinal)

        data.push(cepGrid);
    });

    _gridCEPOrigem.CarregarGrid(data);
}

function ExcluirCEPOrigemClick(data) {
    var cepsOrigem = _tabelaFreteCliente.CEPsOrigem.val();

    for (var i = 0; i < cepsOrigem.length; i++) {
        if (data.Codigo == cepsOrigem[i].Codigo) {
            cepsOrigem.splice(i, 1);
            break;
        }
    }

    _tabelaFreteCliente.CEPsOrigem.val(cepsOrigem);

    RecarregarGridCEPOrigem();
    ValidarOrigensDisponiveis(false);
}

function AdicionarCEPOrigemClick(e, sender) {

    var cepInicial = Globalize.parseInt(string.OnlyNumbers(_tabelaFreteCliente.CEPOrigemInicial.val()));
    var cepFinal = Globalize.parseInt(string.OnlyNumbers(_tabelaFreteCliente.CEPOrigemFinal.val()));

    if (isNaN(cepInicial) || isNaN(cepFinal)) {
        exibirMensagem(tipoMensagem.aviso, Localization.Resources.Fretes.TabelaFreteCliente.FaixaDeCepInvalida, Localization.Resources.Fretes.TabelaFreteCliente.InformeUmaFaixaDeCepValida);
        return;
    }

    if (cepFinal < cepInicial) {
        exibirMensagem(tipoMensagem.aviso, Localization.Resources.Fretes.TabelaFreteCliente.FaixaDeCepInvalida, Localization.Resources.Fretes.TabelaFreteCliente.OCepInicialNaoPodeSerMenorQueOCepFinal);
        return;
    }

    var cepsOrigem = _tabelaFreteCliente.CEPsOrigem.val();

    if (cepsOrigem.some(function (item) {
        var cepInicialCadastrado = Globalize.parseInt(string.OnlyNumbers(item.CEPInicial));
        var cepFinalCadastrado = Globalize.parseInt(string.OnlyNumbers(item.CEPFinal));

        if ((cepInicialCadastrado >= cepInicial && cepInicialCadastrado <= cepFinal) ||
            (cepFinalCadastrado >= cepInicial && cepFinalCadastrado <= cepFinal) ||
            (cepInicial >= cepInicialCadastrado && cepInicial <= cepFinalCadastrado) ||
            (cepFinal >= cepInicialCadastrado && cepFinal <= cepFinalCadastrado))
            return true;
    })) {
        exibirMensagem(tipoMensagem.aviso, Localization.Resources.Fretes.TabelaFreteCliente.FaixaDeCepJaExiste, Localization.Resources.Fretes.TabelaFreteCliente.EstaFaixaDeCepEntrouEmConflitoComOutraJaCadastrada);
        return;
    }

    cepsOrigem.push({ Codigo: guid(), CEPInicial: _tabelaFreteCliente.CEPOrigemInicial.val(), CEPFinal: _tabelaFreteCliente.CEPOrigemFinal.val() })

    _tabelaFreteCliente.CEPsOrigem.val(cepsOrigem);

    RecarregarGridCEPOrigem();

    LimparCamposCEPOrigem();

    ValidarOrigensDisponiveis();
}

function LimparCamposCEPOrigem() {
    _tabelaFreteCliente.CEPOrigemInicial.val("");
    _tabelaFreteCliente.CEPOrigemFinal.val("");
}

//#endregion Faixa de CEP

function ValidarOrigensDisponiveis(reposicionarTabPrincipal) {
    if (_tabelaFreteCliente.ClienteOrigem.basicTable.BuscarRegistros().length > 0) {
        $("#liTabCidadesOrigem").hide();
        $("#liTabClientesOrigem").show();
        $("#liTabEstadosOrigem").hide();
        $("#liTabRegioesOrigem").hide();
        $("#liTabCEPsOrigem").hide();
        $("#liTabRotasOrigem").hide();
        $("#liTabPaisesOrigem").hide();
        $(".nav-tabs a[href='#tabClientesOrigem']").tab('show');
    } else if (_tabelaFreteCliente.Origem.basicTable.BuscarRegistros().length > 0) {
        $("#liTabCidadesOrigem").show();
        $("#liTabClientesOrigem").hide();
        $("#liTabEstadosOrigem").hide();
        $("#liTabRegioesOrigem").hide();
        $("#liTabCEPsOrigem").hide();
        $("#liTabRotasOrigem").hide();
        $("#liTabPaisesOrigem").hide();
        $(".nav-tabs a[href='#tabCidadesOrigem']").tab('show');
    } else if (_tabelaFreteCliente.EstadoOrigem.basicTable.BuscarRegistros().length > 0) {
        $("#liTabCidadesOrigem").hide();
        $("#liTabClientesOrigem").hide();
        $("#liTabEstadosOrigem").show();
        $("#liTabRegioesOrigem").hide();
        $("#liTabCEPsOrigem").hide();
        $("#liTabRotasOrigem").hide();
        $("#liTabPaisesOrigem").hide();
        $(".nav-tabs a[href='#tabEstadosOrigem']").tab('show');
    } else if (_tabelaFreteCliente.RegiaoOrigem.basicTable.BuscarRegistros().length > 0) {
        $("#liTabCidadesOrigem").hide();
        $("#liTabClientesOrigem").hide();
        $("#liTabEstadosOrigem").hide();
        $("#liTabRegioesOrigem").show();
        $("#liTabCEPsOrigem").hide();
        $("#liTabRotasOrigem").hide();
        $("#liTabPaisesOrigem").hide();
        $(".nav-tabs a[href='#tabRegioesOrigem']").tab('show');
    } else if (_tabelaFreteCliente.RotaOrigem.basicTable.BuscarRegistros().length > 0) {
        $("#liTabCidadesOrigem").hide();
        $("#liTabClientesOrigem").hide();
        $("#liTabEstadosOrigem").hide();
        $("#liTabRegioesOrigem").hide();
        $("#liTabCEPsOrigem").hide();
        $("#liTabRotasOrigem").show();
        $("#liTabPaisesOrigem").hide();
        $(".nav-tabs a[href='#tabRotasOrigem']").tab('show');
    } else if (_tabelaFreteCliente.CEPsOrigem.val().length > 0) {
        $("#liTabCidadesOrigem").hide();
        $("#liTabClientesOrigem").hide();
        $("#liTabEstadosOrigem").hide();
        $("#liTabRegioesOrigem").hide();
        $("#liTabCEPsOrigem").show();
        $("#liTabRotasOrigem").hide();
        $("#liTabPaisesOrigem").hide();
        $(".nav-tabs a[href='#tabCEPsOrigem']").tab('show');
    } else if (_tabelaFreteCliente.PaisOrigem.basicTable.BuscarRegistros().length > 0) {
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

function retornoCliente(e) {
    let cnpjPesquisa = e.map(x => x.Codigo);
    BuscarFiliaisRelacionadas(cnpjPesquisa).then(values => {
        let listaRetornar = [...ObteRegistrosOrigemCliente(), ...values];
        let arrayComSet = new Set(listaRetornar);
        _tabelaFreteCliente.ClienteOrigem.basicTable.CarregarGrid([...arrayComSet]);
    })
}

function ObteRegistrosOrigemCliente() {
    return _tabelaFreteCliente.ClienteOrigem.basicTable.BuscarRegistros();
}
function BuscarFiliaisRelacionadas(listCnpjs) {

    const promise = new Promise((resolve, reject) => {
        executarReST("Cliente/BuscarFiliaisRelacionadas", { Clientes: JSON.stringify(listCnpjs) }, (arg) => {
            if (!arg.Success)
                resolve(listCnpjs);
            else {
                resolve(arg.Data);
            }
        })

    });

    return promise;
}