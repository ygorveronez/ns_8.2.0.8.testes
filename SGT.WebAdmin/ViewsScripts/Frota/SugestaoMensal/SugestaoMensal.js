/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Globais.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="../../Consultas/Filial.js" />
/// <reference path="../../Consultas/ModeloVeicularCarga.js" />
/// <reference path="../../Consultas/TipoOperacao.js" />
/// <reference path="../../Consultas/Tranportador.js" />
/// <reference path="../../Enumeradores/EnumMes.js" />
/// <reference path="SugestaoMensalAdicionar.js" />

// #region Objetos Globais do Arquivo

var _crudSugestaoMensal;
var _gridSugestaoMensal;
var _gridSugestaoMensalVeiculos;
var _pesquisaSugestaoMensal;
var _sugestaoMensal;
var _modalEditar;
var _filtroPlanejamentoFrotaMes;

var _filtrosGridVeiculos = {
    CodigoFilial: 0,
    CodigoModeloVeicularCarga: 0,
    CodigoPlanejamentoFrotaMes: 0,
    DescricaoFilial: "",
};

// #endregion Objetos Globais do Arquivo

// #region Classes

var PesquisaSugestaoMensal = function () {
    this.Filial = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Filial:", idBtnSearch: guid() });
    this.Ano = PropertyEntity({ text: "Ano:", idBtnSearch: guid(), val: ko.observable(obterAnoAtual()), getType: typesKnockout.int, configInt: { precision: 0, allowZero: false, thousands: "" }, maxlength: 4 });
    this.Mes = PropertyEntity({ text: "Mes:", val: ko.observable(EnumMes.obterMesAtual()), options: EnumMes.obterOpcoesPesquisa(), def: EnumMes.obterMesAtual() });
    this.Situacao = PropertyEntity({ val: ko.observable(EnumSituacaoSugestaoMensal.Todos), options: EnumSituacaoSugestaoMensal.obterOpcoesPesquisa(), def: EnumSituacaoSugestaoMensal.Todos, text: "Situação: " });
    this.Transportador = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Transportador:", idBtnSearch: guid(), visible: ko.observable(_CONFIGURACAO_TMS.TipoServicoMultisoftware == EnumTipoServicoMultisoftware.MultiEmbarcador) });
    this.TipoOperacao = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Tipo Operação:", idBtnSearch: guid() });
    this.ModeloVeicular = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Modelo Veicular:", idBtnSearch: guid() });

    this.AdicionarSugestaoMensal = PropertyEntity({ eventClick: adicionarSugestaoMensalModalClick, type: types.event, text: "Adicionar Sugestão Mensal", visible: ko.observable(true) });

    this.Pesquisar = PropertyEntity({
        eventClick: function (e) {
            LimparCamposVeiculos();
            _gridSugestaoMensal.CarregarGrid();
        }, type: types.event, text: "Pesquisar", idGrid: guid(), visible: ko.observable(true)
    });

    this.ExibirFiltros = PropertyEntity({
        eventClick: function (e) {
            if (e.ExibirFiltros.visibleFade()) {
                e.ExibirFiltros.visibleFade(false);
            } else {
                e.ExibirFiltros.visibleFade(true);
            }
        }, type: types.event, text: "Filtros de Pesquisa", idFade: guid(), visibleFade: ko.observable(true), visible: ko.observable(true)
    });
}

var SugestaoMensal = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0) });
    this.Grid = PropertyEntity({ type: types.local });
    this.Veiculo = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: "Adicionar Veículos", idBtnSearch: guid(), enable: ko.observable(true) });
}

var CRUDSugestaoMensal = function () {
    this.SalvarAlteracoes = PropertyEntity({ eventClick: salvarListaDeVeiculosClick, type: types.event, text: "Salvar Alterações", visible: ko.observable(true) });
    this.PublicarParaEmbarcador = PropertyEntity({ eventClick: publicarParaEmbarcadorClick, type: types.event, text: "Enviar para Marfrig", visible: ko.observable(_CONFIGURACAO_TMS.TipoServicoMultisoftware != EnumTipoServicoMultisoftware.MultiEmbarcador) });
    this.PublicarParaTransportador = PropertyEntity({ eventClick: publicarParaTransportadorClick, type: types.event, text: "Enviar para o Transportador", visible: ko.observable(_CONFIGURACAO_TMS.TipoServicoMultisoftware == EnumTipoServicoMultisoftware.MultiEmbarcador) });
    this.GerarListaDiaria = PropertyEntity({ eventClick: gerarListaDiariaClick, type: types.event, text: "Gerar Lista Diária", visible: ko.observable(_CONFIGURACAO_TMS.TipoServicoMultisoftware == EnumTipoServicoMultisoftware.MultiEmbarcador) });
    this.CancelarAlteracoes = PropertyEntity({ eventClick: LimparCamposVeiculos, type: types.event, text: "Cancelar Alterações", visible: ko.observable(true) });
}

var ModalEditar = function () {
    this.CodigoPlanejamentoFrotaMesVeiculo = PropertyEntity({ val: ko.observable(0), type: types.map, required: false });
    this.ObservacaoMarfrig = PropertyEntity({ enable: ko.observable(_CONFIGURACAO_TMS.TipoServicoMultisoftware == EnumTipoServicoMultisoftware.MultiEmbarcador), text: 'Observação:', val: ko.observable(''), maxlength: 500, type: types.map, required: ko.observable(false), visible: ko.observable(_CONFIGURACAO_TMS.TipoServicoMultisoftware == EnumTipoServicoMultisoftware.MultiEmbarcador) });
    this.ObservacaoTransportador = PropertyEntity({ enable: ko.observable(_CONFIGURACAO_TMS.TipoServicoMultisoftware != EnumTipoServicoMultisoftware.MultiEmbarcador), text: ko.observable('Observação Transportador:'), val: ko.observable(''), maxlength: 500, type: types.map, required: ko.observable(false), visible: ko.observable(_CONFIGURACAO_TMS.TipoServicoMultisoftware != EnumTipoServicoMultisoftware.MultiEmbarcador) });
    this.Rejeitar = PropertyEntity({ type: types.bool, val: ko.observable(false), def: false, text: "Rejeitar", visible: ko.observable(true) });
    this.Confirmar = PropertyEntity({
        eventClick: function () {
            salvarVeiculo();
        }, type: types.event, text: "Confirmar", idGrid: guid()
    });
}

var FiltroPlanejamentoFrotaMes = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.CodigoModeloVeicular = PropertyEntity({ codEntity: ko.observable(0), val: ko.observable(0) });
}

// #endregion Classes

// #region Funções de Inicialização

function loadDadosUsuarioLogado(callback) {
    executarReST("Usuario/DadosUsuarioLogado", {}, function (retorno) {
        if (retorno.Success && retorno.Data)
            callback(retorno.Data);
        else
            exibirMensagem(tipoMensagem.falha, "Falha", "Bão foi possível obter os dados do usuáio logado");
    });
}

function loadGridSugestaoMensal() {
    var editar = {
        descricao: "Editar", id: guid(), evento: "onclick", metodo: function (registroSelecionado) {
            _pesquisaSugestaoMensal.ExibirFiltros.visibleFade(false);
            EditarSugestaoMensal(registroSelecionado);
        }, tamanho: "20", icone: ""
    };

    var menuOpcoes = { tipo: TypeOptionMenu.link, opcoes: [editar] };
    var configExportacao = {
        url: "SugestaoMensal/ExportarPesquisa",
        titulo: "Sugestão de Frota Mensal"
    };

    _gridSugestaoMensal = new GridView(_pesquisaSugestaoMensal.Pesquisar.idGrid, "SugestaoMensal/Pesquisa", _pesquisaSugestaoMensal, menuOpcoes, null, null, null, null, null, null, null, null, configExportacao);
    _gridSugestaoMensal.CarregarGrid();
}

function loadGridSugestaoMensalVeiculos() {
    var excluirVeiculo = { descricao: "Excluir", id: guid(), metodo: ExcluirVeiculo, icone: "" };
    var editarVeiculo = { descricao: "Editar", id: guid(), metodo: EditarVeiculo, icone: "" };
    var menuOpcoes = { tipo: TypeOptionMenu.list, descricao: "Opções", tamanho: 7, opcoes: [editarVeiculo, excluirVeiculo] };

    var header = [
        { data: "Codigo", visible: false },
        { data: "CodigoVeiculo", visible: false },
        { data: "Rejeitado", visible: false },
        { data: "DT_FontColor", visible: false },
        { data: "DT_RowColor", visible: false },
        { data: "Filial", title: "Filial", width: "11%" },
        { data: "Transportador", title: "Transportador", width: "10%" },
        { data: "CNPJTransportador", title: "CNPJ Transportador", width: "13%", className: "text-align-center" },
        { data: "Placa", title: "Placa", width: "7%", className: "text-align-center" },
        { data: "ModeloVeicular", title: "Modelo Veicular", width: "8%" },
        { data: "Capacidade", title: "Capacidade", width: "8%", className: "text-align-center" },
        { data: "TesteFrio", title: "Teste de Frio", width: "10%", className: "text-align-center" },
        { data: "RespostaTransportador", title: "Resposta Transportador", width: "8%", className: "text-align-center" },
        { data: "ListaDiariaGerada", title: "Lista Diaria Gerada", width: "12%" },
        { data: "ObservacaoTransp", title: "Observação Transp.", width: "12%" },
        { data: "ObservacaoMarfrig", title: "Observação Marfrig", width: "12%" }];

    _gridSugestaoMensalVeiculos = new BasicDataTable(_sugestaoMensal.Grid.id, header, menuOpcoes);

    if (_CONFIGURACAO_TMS.TipoServicoMultisoftware == EnumTipoServicoMultisoftware.MultiEmbarcador)
        new BuscarVeiculos(_sugestaoMensal.Veiculo, retornoBuscaVeiculos, null, _filtroPlanejamentoFrotaMes.CodigoModeloVeicular, null, null, null, null, null, null, null, null, null, _gridSugestaoMensalVeiculos, null, null, null, true);
    else
        new BuscarVeiculos(_sugestaoMensal.Veiculo, retornoBuscaVeiculos, _pesquisaSugestaoMensal.Transportador, _filtroPlanejamentoFrotaMes.CodigoModeloVeicular, null, null, null, null, null, null, null, null, null, _gridSugestaoMensalVeiculos, null, null, null, true);

    _gridSugestaoMensalVeiculos.CarregarGrid([]);
}

function loadSugestaoMensal() {
    loadDadosUsuarioLogado(function (dadosUsuarioLogado) {
        limparFiltrosGridVeiculos();

        _sugestaoMensal = new SugestaoMensal();
        KoBindings(_sugestaoMensal, "knockoutSugestaoMensal");

        _pesquisaSugestaoMensal = new PesquisaSugestaoMensal();
        KoBindings(_pesquisaSugestaoMensal, "knockoutPesquisaSugestaoMensal", false, _pesquisaSugestaoMensal.Pesquisar.id);

        _crudSugestaoMensal = new CRUDSugestaoMensal();
        KoBindings(_crudSugestaoMensal, "knockoutCRUDSugestaoMensal");

        _modalEditar = new ModalEditar();
        KoBindings(_modalEditar, "knockoutModalEditar");

        _filtroPlanejamentoFrotaMes = new FiltroPlanejamentoFrotaMes();

        HeaderAuditoria("PlanejamentoFrotaMesModelo", _filtroPlanejamentoFrotaMes);

        if (_CONFIGURACAO_TMS.TipoServicoMultisoftware != EnumTipoServicoMultisoftware.MultiEmbarcador) {
            _modalEditar.ObservacaoTransportador.text('Observação:');
            _pesquisaSugestaoMensal.Transportador.codEntity(dadosUsuarioLogado.Empresa.Codigo);
            _pesquisaSugestaoMensal.Transportador.val(dadosUsuarioLogado.Empresa.Descricao);
        }

        new BuscarFilial(_pesquisaSugestaoMensal.Filial);
        new BuscarTiposOperacao(_pesquisaSugestaoMensal.TipoOperacao);
        new BuscarModelosVeicularesCarga(_pesquisaSugestaoMensal.ModeloVeicular);
        new BuscarTransportadores(_pesquisaSugestaoMensal.Transportador);

        loadSugestaoMensalAdicionar();
        loadGridSugestaoMensal();
        loadGridSugestaoMensalVeiculos();
    });
}

// #endregion Funções de Inicialização

// #region Funções Associadas a Eventos

function adicionarSugestaoMensalModalClick() {
    exibirModalSugestaoMensalAdicionar();
}

function gerarListaDiariaClick() {
    let invalido = (_filtrosGridVeiculos.CodigoFilial == 0 || _filtrosGridVeiculos.CodigoModeloVeicularCarga == 0 || _filtrosGridVeiculos.CodigoPlanejamentoFrotaMes == 0);

    if (invalido) {
        exibirMensagem(tipoMensagem.atencao, "Selecione um planejamento mensal.", "Selecione um planejamento mensal.", 60000);
        return;
    }

    exibirConfirmacao("Confirmação", "Deseja prosseguir com a geração da Lista Diária? Não será possível fazer alterações após prosseguir.", function () {
        var dados = {
            CodigoFilial: _filtrosGridVeiculos.CodigoFilial,
            Transportador: _pesquisaSugestaoMensal.Transportador.codEntity(),
            CodigoModeloVeicularCarga: _filtrosGridVeiculos.CodigoModeloVeicularCarga,
            CodigoPlanejamentoFrotaMes: _filtrosGridVeiculos.CodigoPlanejamentoFrotaMes
        };

        executarReST('SugestaoMensal/GerarListaDiaria', dados, function (arg) {
            if (!arg || !arg.Success) {
                exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, arg.Msg);
                return;
            }
            _gridSugestaoMensalVeiculos.CarregarGrid([]);
            LimparCamposVeiculos();
            _gridSugestaoMensal.CarregarGrid();
            exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, "Geração da lista diária realizada com sucesso.");
        });
    });
}

function publicarParaEmbarcadorClick() {
    let invalido = (_filtrosGridVeiculos.CodigoFilial == 0 || _filtrosGridVeiculos.CodigoModeloVeicularCarga == 0 || _filtrosGridVeiculos.CodigoPlanejamentoFrotaMes == 0);

    if (invalido) {
        exibirMensagem(tipoMensagem.atencao, "Selecione um planejamento mensal.", "Selecione um planejamento mensal.", 60000);
        return;
    }

    exibirConfirmacao("Confirmação", "Deseja prosseguir com o envio dessa lista para a Marfrig? Não será possível fazer alterações após prosseguir.", function () {
        executarReST('SugestaoMensal/PublicarParaEmbarcador', _filtrosGridVeiculos, function (retorno) {
            if (!retorno || !retorno.Success) {
                exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, retorno.Msg);
                return;
            }
            if (!retorno.Data) {
                exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Atencao, retorno.Msg, 60000);
                return;
            }
            _gridSugestaoMensalVeiculos.CarregarGrid([]);
            LimparCamposVeiculos();
            _gridSugestaoMensal.CarregarGrid();
            exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, "Publicação da lista mensal realizada com sucesso.");
        });
    });
}

function publicarParaTransportadorClick() {
    let invalido = (_filtrosGridVeiculos.CodigoFilial == 0 || _filtrosGridVeiculos.CodigoModeloVeicularCarga == 0 || _filtrosGridVeiculos.CodigoPlanejamentoFrotaMes == 0);

    if (invalido) {
        exibirMensagem(tipoMensagem.atencao, "Selecione um planejamento mensal.", "Selecione um planejamento mensal.", 60000);
        return;
    }

    exibirConfirmacao("Confirmação", "Deseja prosseguir com o envio dessa lista para o Transportador? Não será possível fazer alterações após prosseguir.", function () {
        executarReST('SugestaoMensal/PublicarParaTransportador', _filtrosGridVeiculos, function (retorno) {
            if (!retorno || !retorno.Success) {
                exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, retorno.Msg);
                return;
            }
            if (!retorno.Data) {
                exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Atencao, retorno.Msg, 60000);
                return;
            }
            _gridSugestaoMensalVeiculos.CarregarGrid([]);
            LimparCamposVeiculos();
            _gridSugestaoMensal.CarregarGrid();
            exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, "Publicação da lista mensal realizada com sucesso.");
        });
    });
}

function salvarListaDeVeiculosClick() {
    let invalido = (_filtrosGridVeiculos.CodigoFilial == 0 || _filtrosGridVeiculos.CodigoModeloVeicularCarga == 0 || _filtrosGridVeiculos.CodigoPlanejamentoFrotaMes == 0);

    if (invalido) {
        exibirMensagem(tipoMensagem.atencao, "Selecione um planejamento mensal.", "Selecione um planejamento mensal.", 60000);
        return;
    }

    var veiculos = [];

    _gridSugestaoMensalVeiculos.BuscarRegistros().slice().forEach(function (veiculo) {
        veiculos.push({
            CodigoVeiculo: veiculo.CodigoVeiculo,
            ObservacaoMarfrig: veiculo.ObservacaoMarfrig,
            Placa: veiculo.Placa
        });
    });

    var dados = {
        CodigoFilial: _filtrosGridVeiculos.CodigoFilial,
        CodigoModeloVeicularCarga: _filtrosGridVeiculos.CodigoModeloVeicularCarga,
        CodigoPlanejamentoFrotaMes: _filtrosGridVeiculos.CodigoPlanejamentoFrotaMes,
        Veiculos: JSON.stringify(veiculos)
    };

    executarReST('SugestaoMensal/SalvarListaDeVeiculos', dados, function (retorno) {
        if (!retorno || !retorno.Success) {
            EditarSugestaoMensal();
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, retorno.Msg);
            return;
        }
        if (!retorno.Data) {
            EditarSugestaoMensal();
            exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Atencao, retorno.Msg, 60000);
            return;
        }
        EditarSugestaoMensal();
        exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, Localization.Resources.Gerais.Geral.CadastradoComSucesso);
        _gridSugestaoMensal.CarregarGrid();
    });
}

// #endregion Funções Associadas a Eventos

// #region Funções Auxiliares

function LimparCamposVeiculos() {
    LimparCampos(_filtroPlanejamentoFrotaMes);
    limparFiltrosGridVeiculos();
    _gridSugestaoMensalVeiculos.CarregarGrid([]);
    $('#container-dados-sugestao-mensal').slideUp();
    _pesquisaSugestaoMensal.ExibirFiltros.visibleFade(true);
}

function EditarSugestaoMensal(rowJson) {
    if (rowJson) {
        _filtrosGridVeiculos.CodigoFilial = rowJson.CodigoFilial;
        _filtrosGridVeiculos.CodigoModeloVeicularCarga = rowJson.CodigoModeloVeicular;
        _filtrosGridVeiculos.CodigoPlanejamentoFrotaMes = rowJson.CodigoPlanejamentoFrotaMes;
        _filtrosGridVeiculos.DescricaoFilial = rowJson.Filial;
        _filtrosGridVeiculos.Transportador = rowJson.Transportador;

        _filtroPlanejamentoFrotaMes.Codigo.val(rowJson.CodigoPorModelo);
        _filtroPlanejamentoFrotaMes.CodigoModeloVeicular.val(rowJson.CodigoModeloVeicular);
        _filtroPlanejamentoFrotaMes.CodigoModeloVeicular.codEntity(rowJson.CodigoModeloVeicular);
    }

    executarReST('SugestaoMensal/ObterVeiculos', _filtrosGridVeiculos, function (arg) {
        if (!arg || !arg.Success) {
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, arg.Msg);
            return;
        }
        if (!arg.Data) {
            exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Atencao, arg.Msg, 60000);
            return;
        }
        var data = new Array();

        for (let i = 0; i < arg.Data.length; i++) {
            let veiculo = arg.Data[i];
            var veiculoGrid = new Object();
            veiculoGrid.Codigo = veiculo.Codigo;
            veiculoGrid.Placa = veiculo.Placa;
            veiculoGrid.ModeloVeicular = veiculo.ModeloVeicular;
            veiculoGrid.Capacidade = veiculo.Capacidade;
            veiculoGrid.TesteFrio = veiculo.TesteFrio;
            veiculoGrid.ObservacaoTransp = veiculo.ObservacaoTransp;
            veiculoGrid.ObservacaoMarfrig = veiculo.ObservacaoMarfrig;
            veiculoGrid.CodigoVeiculo = veiculo.CodigoVeiculo;
            veiculoGrid.RespostaTransportador = veiculo.RespostaTransportador;
            veiculoGrid.ListaDiariaGerada = veiculo.ListaDiariaGerada;
            veiculoGrid.Rejeitado = veiculo.Rejeitado;
            veiculoGrid.Filial = veiculo.Filial;
            veiculoGrid.Transportador = veiculo.Transportador;
            veiculoGrid.CNPJTransportador = veiculo.CNPJTransportador;
            veiculoGrid.DT_FontColor = veiculo.DT_FontColor;
            veiculoGrid.DT_RowColor = veiculo.DT_RowColor;

            data.push(veiculoGrid);
        }
        _gridSugestaoMensalVeiculos.CarregarGrid(data);
        $('#container-dados-sugestao-mensal').slideDown();
    });
}

function salvarVeiculo() {

    if (_modalEditar.Rejeitar.val()) {
        if (_CONFIGURACAO_TMS.TipoServicoMultisoftware == EnumTipoServicoMultisoftware.MultiEmbarcador) {
            _modalEditar.ObservacaoMarfrig.required(true);
            _modalEditar.ObservacaoTransportador.required(false);
        } else {
            _modalEditar.ObservacaoMarfrig.required(false);
            _modalEditar.ObservacaoTransportador.required(true);
        }
        if (!ValidarCamposObrigatorios(_modalEditar)) {
            return;
        }
    }

    executarReST('SugestaoMensal/SalvarVeiculo', {
        CodigoPlanejamentoFrotaMesVeiculo: _modalEditar.CodigoPlanejamentoFrotaMesVeiculo.val(),
        ObservacaoMarfrig: _modalEditar.ObservacaoMarfrig.val(),
        ObservacaoTransportador: _modalEditar.ObservacaoTransportador.val(),
        Rejeitar: _modalEditar.Rejeitar.val()
    }, function (arg) {
        Global.fecharModal("divModalEditar");
        if (!arg || !arg.Success) {
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, arg.Msg);
            return;
        }
        if (!arg.Data) {
            exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Atencao, arg.Msg, 60000);
            return;
        }
        LimparCampos(_modalEditar);
        EditarSugestaoMensal();
        exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, "Dados Atualizados com Sucesso.");
    });
}

function EditarVeiculo(veiculoSelecionado) {
    console.log("veiculoSelecionado.Rejeitado");
    console.log(veiculoSelecionado.Rejeitado);
    _modalEditar.Rejeitar.val(veiculoSelecionado.Rejeitado);
    _modalEditar.CodigoPlanejamentoFrotaMesVeiculo.val(veiculoSelecionado.Codigo);
    Global.abrirModal('divModalEditar');

    console.log(veiculoSelecionado)

    $("#divModalEditar").one('hidden.bs.modal', function () {
        LimparCampos(_modalEditar);
    });
}

function ExcluirVeiculo(veiculoSelecionado) {
    let veiculos = _gridSugestaoMensalVeiculos.BuscarRegistros();

    for (var i = 0; i < veiculos.length; i++) {
        if (veiculoSelecionado.Codigo == veiculos[i].Codigo) {
            veiculos.splice(i, 1);
            break;
        }
    }

    _gridSugestaoMensalVeiculos.CarregarGrid(veiculos);
}

function obterAnoAtual() {
    return new Date().getFullYear();
}

function limparFiltrosGridVeiculos() {
    _filtrosGridVeiculos = {
        CodigoFilial: 0,
        CodigoModeloVeicularCarga: 0,
        CodigoPlanejamentoFrotaMes: 0,
        DescricaoFilial: ""
    };
}

function retornoBuscaVeiculos(retornoBuscar) {
    let listaRegistros = _gridSugestaoMensalVeiculos.BuscarRegistros();
    let jaExiste = false;
    for (let i = 0; i < retornoBuscar.length; i++) {
        jaExiste = false;
        if (listaRegistros && listaRegistros.length > 0) {
            for (let j = 0; j < listaRegistros.length; j++) {
                if (listaRegistros[j].Placa == retornoBuscar[i].Placa) {
                    jaExiste = true;
                    exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Atencao, "O veículo " + retornoBuscar[i].Placa + " já está na lista.", 5000);
                }
            }
        }
        if (jaExiste)
            continue;

        let veiculo = {
            Codigo: 0,
            Placa: retornoBuscar[i].Placa,
            ModeloVeicular: retornoBuscar[i].ModeloVeicularCarga,
            Capacidade: retornoBuscar[i].CapacidadeKG,
            TesteFrio: "",
            ObservacaoTransp: "",
            ObservacaoMarfrig: "",
            CodigoVeiculo: retornoBuscar[i].Codigo,
            RespostaTransportador: "Pendente",
            Rejeitado: false,
            Filial: _filtrosGridVeiculos.DescricaoFilial,
            Transportador: retornoBuscar[i].EmpresaDescricao,
            CNPJTransportador: retornoBuscar[i].CNPJEmpresa,
            DT_FontColor: "",
            DT_RowColor: "#91a8ee",
            ListaDiariaGerada: retornoBuscar[i].ListaDiariaGerada ? "Sim" :"Não"
        };
        listaRegistros.push(veiculo);
    }

    _gridSugestaoMensalVeiculos.CarregarGrid(listaRegistros);
}

// #endregion Funções Auxiliares
