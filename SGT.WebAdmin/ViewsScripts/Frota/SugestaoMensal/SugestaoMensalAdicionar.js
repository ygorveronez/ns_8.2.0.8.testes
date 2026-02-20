/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Globais.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="../../Consultas/Filial.js" />
/// <reference path="../../Consultas/ModeloVeicularCarga.js" />
/// <reference path="../../Consultas/Veiculo.js" />

// #region Objetos Globais do Arquivo

var _sugestaoMensalAdicionar;
var _gridSugestaoMensalAdicionarVeiculo;
var _gridSugestaoMensalRetornoAdicionarVeiculo;

// #endregion Objetos Globais do Arquivo

// #region Classes

var SugestaoMensalAdicionar = function () {
    this.Filial = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "*Filial:", idBtnSearch: guid(), required: true });
    this.ModeloVeicularCarga = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "*Modelo Veicular:", idBtnSearch: guid(), required: true });
    this.Ano = PropertyEntity({ text: "*Ano:", val: ko.observable(""), getType: typesKnockout.int, configInt: { precision: 0, allowZero: false, thousands: "" }, maxlength: 4, required: true });
    this.Mes = PropertyEntity({ text: "*Mes:", val: ko.observable(""), options: EnumMes.obterOpcoes(), required: true });

    this.Adicionar = PropertyEntity({ type: types.event, text: "Adicionar", eventClick: adicionarSugestaoMensalClick, idGrid: guid() });
    this.Veiculo = PropertyEntity({ type: types.event, text: "Adicionar Veículos", idBtnSearch: guid(), idGrid: guid() });
}

// #endregion Classes

// #region Funções de Inicialização

function loadSugestaoMensalAdicionar() {
    _sugestaoMensalAdicionar = new SugestaoMensalAdicionar();
    KoBindings(_sugestaoMensalAdicionar, "knockoutSugestaoMensalAdicionar");

    new BuscarFilial(_sugestaoMensalAdicionar.Filial);
    new BuscarModelosVeicularesCarga(_sugestaoMensalAdicionar.ModeloVeicularCarga);

    loadGridSugestaoMensalAdicionarVeiculo();
    loadGridSugestaoMensalRetornoAdicionarVeiculo();
}

function loadGridSugestaoMensalAdicionarVeiculo() {
    var opcaoExcluir = { descricao: "Excluir", id: guid(), metodo: excluirSugestaoMensalAdicionarVeiculoClick }
    var menuOpcoes = { tipo: TypeOptionMenu.link, tamanho: 15, opcoes: [opcaoExcluir] };

    var header = [
        { data: "Codigo", visible: false },
        { data: "Descricao", title: "Descrição", width: "85%" }
    ];

    _gridSugestaoMensalAdicionarVeiculo = new BasicDataTable(_sugestaoMensalAdicionar.Veiculo.idGrid, header, menuOpcoes, { column: 1, dir: orderDir.asc });

    if (_CONFIGURACAO_TMS.TipoServicoMultisoftware == EnumTipoServicoMultisoftware.MultiEmbarcador)
        new BuscarVeiculos(_sugestaoMensalAdicionar.Veiculo, null, null, _sugestaoMensalAdicionar.ModeloVeicularCarga, null, null, null, null, null, null, null, null, null, _gridSugestaoMensalAdicionarVeiculo, null, null, null, true);
    else
        new BuscarVeiculos(_sugestaoMensalAdicionar.Veiculo, null, _pesquisaSugestaoMensal.Transportador, _sugestaoMensalAdicionar.ModeloVeicularCarga, null, null, null, null, null, null, null, null, null, _gridSugestaoMensalAdicionarVeiculo, null, null, null, true);

    _gridSugestaoMensalAdicionarVeiculo.CarregarGrid([]);
}

function loadGridSugestaoMensalRetornoAdicionarVeiculo() {
    var quantidadePorPagina = 10;
    var ordenacao = { column: 0, dir: orderDir.asc };
    var header = [
        { data: "Placa", title: "Placa", width: "20%", className: 'text-align-center', orderable: true },
        { data: "MensagemRetorno", title: "Mensagem de Retorno", width: "80%", orderable: false }
    ];

    _gridSugestaoMensalRetornoAdicionarVeiculo = new BasicDataTable("grid-sugestao-mensal-retorno-adicionar-veiculo", header, null, ordenacao, null, quantidadePorPagina);
    _gridSugestaoMensalRetornoAdicionarVeiculo.CarregarGrid([]);
}

// #endregion Funções de Inicialização

// #region Funções Associadas a Eventos

function adicionarSugestaoMensalClick() {
    if (!ValidarCamposObrigatorios(_sugestaoMensalAdicionar))
        return exibirMensagem(tipoMensagem.atencao, "Atenção", "Preencha os campos obrigatórios.");

    var codigosVeiculos = [];

    _gridSugestaoMensalAdicionarVeiculo.BuscarRegistros().slice().forEach(function (veiculo) {
        codigosVeiculos.push(veiculo.Codigo);
    });

    if (codigosVeiculos.length == 0)
        return exibirMensagem(tipoMensagem.atencao, "Atenção", "Informe um ou mais veículos.");

    var dados = RetornarObjetoPesquisa(_sugestaoMensalAdicionar);

    dados.Veiculos = JSON.stringify(codigosVeiculos);

    executarReST("SugestaoMensal/AdicionarListaVeiculos", dados, function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                fecharModalSugestaoMensalAdicionar();
                _gridSugestaoMensalRetornoAdicionarVeiculo.CarregarGrid(retorno.Data);
                Global.abrirModal('divModalRetornoSugestaoMensalAdicionarVeiculo');
            }
            else
                exibirMensagem(tipoMensagem.aviso, "Aviso", retorno.Msg);
        }
        else
            exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
    }, null);
}

function excluirSugestaoMensalAdicionarVeiculoClick(registroSelecionado) {
    var listaVeiculo = _gridSugestaoMensalAdicionarVeiculo.BuscarRegistros().slice();

    for (var i = 0; i < listaVeiculo.length; i++) {
        if (registroSelecionado.Codigo == listaVeiculo[i].Codigo) {
            listaVeiculo.splice(i, 1);
            break;
        }
    }

    _gridSugestaoMensalAdicionarVeiculo.CarregarGrid(listaVeiculo);
}

// #endregion Funções Associadas a Eventos

// #region Métodos Públicos

function exibirModalSugestaoMensalAdicionar() {
    _sugestaoMensalAdicionar.Ano.val(obterAnoAtual());
    _sugestaoMensalAdicionar.Mes.val(EnumMes.obterMesAtual());

    Global.abrirModal('divModalSugestaoMensalAdicionar');
    $("#divModalSugestaoMensalAdicionar").one('hidden.bs.modal', function () {
        limparCamposSugestaoMensalAdicionar();
    });
}

// #endregion Funções Públicas

// #region Funções Privadas

function fecharModalSugestaoMensalAdicionar() {
    Global.fecharModal("divModalSugestaoMensalAdicionar");
}

function limparCamposSugestaoMensalAdicionar() {
    LimparCampos(_sugestaoMensalAdicionar);

    _gridSugestaoMensalAdicionarVeiculo.CarregarGrid([]);
}

// #endregion Funções Privadas