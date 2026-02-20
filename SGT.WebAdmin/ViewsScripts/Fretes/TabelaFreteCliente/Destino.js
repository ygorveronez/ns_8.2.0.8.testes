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
            descricao: Localization.Resources.Fretes.TabelaFreteCliente.Excluir, id: guid(), metodo: function (data) {
                ExcluirDestinoClick(_tabelaFreteCliente.Destino, data);
            }
        }]
    };

    var header = [
        { data: "Codigo", visible: false },
        { data: "Descricao", title: Localization.Resources.Fretes.TabelaFreteCliente.Descricao, width: "80%" }
    ];

    _gridDestino = new BasicDataTable(_tabelaFreteCliente.GridDestino.id, header, menuOpcoes, { column: 1, dir: orderDir.asc });

    new BuscarLocalidades(_tabelaFreteCliente.Destino, null, null, null, _gridDestino, ValidarDestinosDisponiveis);

    _tabelaFreteCliente.Destino.basicTable = _gridDestino;
    _tabelaFreteCliente.Destino.basicTable.CarregarGrid(new Array());
}

function LoadEstadoDestino() {
    var menuOpcoes = {
        tipo: TypeOptionMenu.link, tamanho: 7, opcoes: [{
            descricao: Localization.Resources.Fretes.TabelaFreteCliente.Excluir, id: guid(), metodo: function (data) {
                ExcluirDestinoClick(_tabelaFreteCliente.EstadoDestino, data);
            }
        }]
    };

    var header = [
        { data: "Codigo", visible: false },
        { data: "Descricao", title: Localization.Resources.Fretes.TabelaFreteCliente.Descricao, width: "80%" }
    ];

    _gridEstadoDestino = new BasicDataTable(_tabelaFreteCliente.GridEstadoDestino.id, header, menuOpcoes, { column: 1, dir: orderDir.asc });

    new BuscarEstados(_tabelaFreteCliente.EstadoDestino, null, _gridEstadoDestino, ValidarDestinosDisponiveis);

    _tabelaFreteCliente.EstadoDestino.basicTable = _gridEstadoDestino;
    _tabelaFreteCliente.EstadoDestino.basicTable.CarregarGrid(new Array());
}

function LoadRegiaoDestino() {
    var menuOpcoes = {
        tipo: TypeOptionMenu.link, tamanho: 7, opcoes: [{
            descricao: Localization.Resources.Fretes.TabelaFreteCliente.Excluir, id: guid(), metodo: function (data) {
                ExcluirDestinoClick(_tabelaFreteCliente.RegiaoDestino, data);
            }
        }]
    };

    var header = [
        { data: "Codigo", visible: false },
        { data: "Descricao", title: Localization.Resources.Fretes.TabelaFreteCliente.Descricao, width: "80%" }
    ];

    _gridRegiaoDestino = new BasicDataTable(_tabelaFreteCliente.GridRegiaoDestino.id, header, menuOpcoes, { column: 1, dir: orderDir.asc });

    new BuscarRegioes(_tabelaFreteCliente.RegiaoDestino, null, _gridRegiaoDestino, ValidarDestinosDisponiveis);

    _tabelaFreteCliente.RegiaoDestino.basicTable = _gridRegiaoDestino;
    _tabelaFreteCliente.RegiaoDestino.basicTable.CarregarGrid(new Array());
}

function LoadRotaDestino() {
    var menuOpcoes = {
        tipo: TypeOptionMenu.link, tamanho: 7, opcoes: [{
            descricao: Localization.Resources.Fretes.TabelaFreteCliente.Excluir, id: guid(), metodo: function (data) {
                ExcluirDestinoClick(_tabelaFreteCliente.RotaDestino, data);
            }
        }]
    };

    var header = [
        { data: "Codigo", visible: false },
        { data: "Descricao", title: Localization.Resources.Fretes.TabelaFreteCliente.Descricao, width: "80%" }
    ];

    _gridRotaDestino = new BasicDataTable(_tabelaFreteCliente.GridRotaDestino.id, header, menuOpcoes, { column: 1, dir: orderDir.asc });

    new BuscarRotasFrete(_tabelaFreteCliente.RotaDestino, null, _gridRotaDestino, null, null, null, ValidarDestinosDisponiveis, null, _tabelaFreteCliente.TabelaFrete);

    _tabelaFreteCliente.RotaDestino.basicTable = _gridRotaDestino;
    _tabelaFreteCliente.RotaDestino.basicTable.CarregarGrid(new Array());
}

function LoadClienteDestino() {
    var menuOpcoes = {
        tipo: TypeOptionMenu.link, tamanho: 7, opcoes: [{
            descricao: Localization.Resources.Fretes.TabelaFreteCliente.Excluir, id: guid(), metodo: function (data) {
                ExcluirDestinoClick(_tabelaFreteCliente.ClienteDestino, data);
            }
        }]
    };

    var header = [
        { data: "Codigo", visible: false },
        { data: "Descricao", title: Localization.Resources.Fretes.TabelaFreteCliente.Descricao, width: "80%" }
    ];

    _gridClienteDestino = new BasicDataTable(_tabelaFreteCliente.GridClienteDestino.id, header, menuOpcoes, { column: 1, dir: orderDir.asc });

    new BuscarClientes(_tabelaFreteCliente.ClienteDestino, retornoClienteDestino, false, null, null, _gridClienteDestino, null, null, null, null, ValidarDestinosDisponiveis);

    _tabelaFreteCliente.ClienteDestino.basicTable = _gridClienteDestino;
    _tabelaFreteCliente.ClienteDestino.basicTable.CarregarGrid(new Array());
}

function LoadPaisDestino() {
    var menuOpcoes = {
        tipo: TypeOptionMenu.link, tamanho: 7, opcoes: [{
            descricao: Localization.Resources.Fretes.TabelaFreteCliente.Excluir, id: guid(), metodo: function (data) {
                ExcluirDestinoClick(_tabelaFreteCliente.PaisDestino, data);
            }
        }]
    };

    var header = [
        { data: "Codigo", visible: false },
        { data: "Descricao", title: Localization.Resources.Fretes.TabelaFreteCliente.Descricao, width: "80%" }
    ];

    _gridPaisDestino = new BasicDataTable(_tabelaFreteCliente.GridPaisDestino.id, header, menuOpcoes, { column: 1, dir: orderDir.asc });

    new BuscarPaises(_tabelaFreteCliente.PaisDestino, null, null, null, _gridPaisDestino, ValidarDestinosDisponiveis);

    _tabelaFreteCliente.PaisDestino.basicTable = _gridPaisDestino;
    _tabelaFreteCliente.PaisDestino.basicTable.CarregarGrid(new Array());
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

    var menuOpcoes = { tipo: TypeOptionMenu.link, tamanho: 7, opcoes: [{ descricao: Localization.Resources.Fretes.TabelaFreteCliente.Excluir, id: guid(), metodo: ExcluirCEPDestinoClick }] };

    var header = [
        { data: "Codigo", visible: false },
        { data: "Descricao", title: Localization.Resources.Fretes.TabelaFreteCliente.Descricao, width: _tabelaFreteCliente.CEPDestinoDiasUteis.visible() ? "55%" : "80%" },
        { data: "DiasUteis", title: Localization.Resources.Fretes.TabelaFreteCliente.PrazoDiasUteis, width: "25%", visible: _tabelaFreteCliente.CEPDestinoDiasUteis.visible() }
    ];

    _gridCEPDestino = new BasicDataTable(_tabelaFreteCliente.GridCEPDestino.id, header, menuOpcoes, { column: 1, dir: orderDir.asc });

    RecarregarGridCEPDestino();
}

function RecarregarGridCEPDestino() {

    var data = new Array();

    $.each(_tabelaFreteCliente.CEPsDestino.val(), function (i, cep) {
        var cepGrid = new Object();

        cepGrid.Codigo = cep.Codigo;
        cepGrid.Descricao = Localization.Resources.Fretes.TabelaFreteCliente.DeAte.format(cep.CEPInicial, cep.CEPFinal )
        cepGrid.DiasUteis = cep.DiasUteis;

        data.push(cepGrid);
    });

    _gridCEPDestino.CarregarGrid(data);
}

function ExcluirCEPDestinoClick(data) {
    var cepsDestino = _tabelaFreteCliente.CEPsDestino.val();

    for (var i = 0; i < cepsDestino.length; i++) {
        if (data.Codigo == cepsDestino[i].Codigo) {
            cepsDestino.splice(i, 1);
            break;
        }
    }

    _tabelaFreteCliente.CEPsDestino.val(cepsDestino);

    RecarregarGridCEPDestino();
    ValidarDestinosDisponiveis(false);
}

function AdicionarCEPDestinoClick(e, sender) {

    var cepInicial = Globalize.parseInt(string.OnlyNumbers(_tabelaFreteCliente.CEPDestinoInicial.val()));
    var cepFinal = Globalize.parseInt(string.OnlyNumbers(_tabelaFreteCliente.CEPDestinoFinal.val()));

    if (isNaN(cepInicial) || isNaN(cepFinal)) {
        exibirMensagem(tipoMensagem.aviso, Localization.Resources.Fretes.TabelaFreteCliente.FaixaDeCepInvalida, Localization.Resources.Fretes.TabelaFreteCliente.InformeUmaFaixaDeCepValida);
        return;
    }

    if (cepFinal < cepInicial) {
        exibirMensagem(tipoMensagem.aviso, Localization.Resources.Fretes.TabelaFreteCliente.FaixaDeCepInvalida, Localization.Resources.Fretes.TabelaFreteCliente.OCepInicialNaoPodeSerMenorQueOCepFinal);
        return;
    }

    var cepsDestino = _tabelaFreteCliente.CEPsDestino.val();

    if (cepsDestino.some(function (item) {
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

    cepsDestino.push({
        Codigo: guid(),
        CEPInicial: _tabelaFreteCliente.CEPDestinoInicial.val(),
        CEPFinal: _tabelaFreteCliente.CEPDestinoFinal.val(),
        DiasUteis: _tabelaFreteCliente.CEPDestinoDiasUteis.val()
    })

    _tabelaFreteCliente.CEPsDestino.val(cepsDestino);

    RecarregarGridCEPDestino();

    LimparCamposCEPDestino();

    ValidarDestinosDisponiveis();
}

function LimparCamposCEPDestino() {
    _tabelaFreteCliente.CEPDestinoInicial.val("");
    _tabelaFreteCliente.CEPDestinoFinal.val("");
    _tabelaFreteCliente.CEPDestinoDiasUteis.val("");
}

//#endregion Faixa de CEP

function ValidarDestinosDisponiveis(reposicionarTabPrincipal) {
    if (_tabelaFreteCliente.ClienteDestino.basicTable.BuscarRegistros().length > 0) {
        $("#liTabCidadesDestino").hide();
        $("#liTabClientesDestino").show();
        $("#liTabEstadosDestino").hide();
        $("#liTabRegioesDestino").hide();
        $("#liTabRotasDestino").hide();
        $("#liTabCEPsDestino").hide();
        $("#liTabPaisesDestino").hide();
        $(".nav-tabs a[href='#tabClientesDestino']").tab('show');
    } else if (_tabelaFreteCliente.Destino.basicTable.BuscarRegistros().length > 0) {
        $("#liTabCidadesDestino").show();
        $("#liTabClientesDestino").hide();
        $("#liTabEstadosDestino").hide();
        $("#liTabRegioesDestino").hide();
        $("#liTabRotasDestino").hide();
        $("#liTabCEPsDestino").hide();
        $("#liTabPaisesDestino").hide();
        $(".nav-tabs a[href='#tabCidadesDestino']").tab('show');
    } else if (_tabelaFreteCliente.EstadoDestino.basicTable.BuscarRegistros().length > 0) {
        $("#liTabCidadesDestino").hide();
        $("#liTabClientesDestino").hide();
        $("#liTabEstadosDestino").show();
        $("#liTabRegioesDestino").hide();
        $("#liTabRotasDestino").hide();
        $("#liTabCEPsDestino").hide();
        $("#liTabPaisesDestino").hide();
        $(".nav-tabs a[href='#tabEstadosDestino']").tab('show');
    } else if (_tabelaFreteCliente.RegiaoDestino.basicTable.BuscarRegistros().length > 0) {
        $("#liTabCidadesDestino").hide();
        $("#liTabClientesDestino").hide();
        $("#liTabEstadosDestino").hide();
        $("#liTabRegioesDestino").show();
        $("#liTabRotasDestino").hide();
        $("#liTabCEPsDestino").hide();
        $("#liTabPaisesDestino").hide();
        $(".nav-tabs a[href='#tabRegioesDestino']").tab('show');
    } else if (_tabelaFreteCliente.RotaDestino.basicTable.BuscarRegistros().length > 0) {
        $("#liTabCidadesDestino").hide();
        $("#liTabClientesDestino").hide();
        $("#liTabEstadosDestino").hide();
        $("#liTabRegioesDestino").hide();
        $("#liTabRotasDestino").show();
        $("#liTabCEPsDestino").hide();
        $("#liTabPaisesDestino").hide();
        $(".nav-tabs a[href='#tabRotasDestino']").tab('show');
    } else if (_tabelaFreteCliente.CEPsDestino.val().length > 0) {
        $("#liTabCidadesDestino").hide();
        $("#liTabClientesDestino").hide();
        $("#liTabEstadosDestino").hide();
        $("#liTabRegioesDestino").hide();
        $("#liTabRotasDestino").hide();
        $("#liTabCEPsDestino").show();
        $("#liTabPaisesDestino").hide();
        $(".nav-tabs a[href='#tabCEPsDestino']").tab('show');
    } else if (_tabelaFreteCliente.PaisDestino.basicTable.BuscarRegistros().length > 0) {
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

function retornoClienteDestino(e) {
    let cnpjPesquisa = e.map(x => x.Codigo);
    BuscarFiliaisRelacionadas(cnpjPesquisa).then(values => {
        let listaRetornar = [...ObteRegistrosOrigemClienteDestino(), ...values];
        let arrayComSet = new Set(listaRetornar);
        _tabelaFreteCliente.ClienteDestino.basicTable.CarregarGrid([...arrayComSet]);
    })
}

function ObteRegistrosOrigemClienteDestino() {
    return _tabelaFreteCliente.ClienteDestino.basicTable.BuscarRegistros();
}