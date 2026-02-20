/// <reference path="../../../js/libs/jquery-2.1.1.js" />
/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/bootstrap/bootstrap.js" />
/// <reference path="../../../js/libs/jquery.blockui.js" />
/// <reference path="../../../js/Global/knoutViewsSlides.js" />
/// <reference path="../../Consultas/MarcaEquipamento.js" />
/// <reference path="../../Consultas/ModeloEquipamento.js" />
/// <reference path="../../Consultas/Veiculo.js" />
/// <reference path="../../Consultas/CentroResultado.js" />
/// <reference path="../../Consultas/GrupoServico.js" />
/// <reference path="../../../js/libs/jquery.maskMoney.js" />
/// <reference path="../../../js/plugin/datatables/jquery.dataTables.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _gridEquipamento;
var _equipamento;
var _pesquisaEquipamento;
var _crudEquipamento;
var _informarGrupoServico;
var _modalinformarGrupoServico;

var PesquisaEquipamento = function () {
    this.Descricao = PropertyEntity({ text: "Descrição: " });
    this.Numero = PropertyEntity({ text: "Número: " });
    this.Chassi = PropertyEntity({ text: "Chassi: " });
    this.MarcaEquipamento = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Marca:", idBtnSearch: guid() });
    this.Veiculo = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Veículo: ", idBtnSearch: guid() });
    this.Ativo = PropertyEntity({ val: ko.observable(1), options: _statusPesquisa, def: 1, text: "Situação: " });

    this.Pesquisar = PropertyEntity({
        eventClick: function (e) {
            _gridEquipamento.CarregarGrid();
        }, type: types.event, text: "Pesquisar", idGrid: guid(), visible: ko.observable(true)
    });
    this.ExibirFiltros = PropertyEntity({
        eventClick: function (e) {
            if (e.ExibirFiltros.visibleFade()) {
                e.ExibirFiltros.visibleFade(false);
            } else {
                e.ExibirFiltros.visibleFade(true);
            }
        }, type: types.event, text: "Filtros de Pesquisa", idFade: guid(), visibleFade: ko.observable(false), visible: ko.observable(true)
    });
};

var Equipamento = function () {
    this.Codigo = PropertyEntity({ text: "Código:", required: false, val: ko.observable(0), def: 0, getType: typesKnockout.int, enable: false });
    this.GrupoServico = PropertyEntity({ type: types.entity, codEntity: ko.observable(0) });

    this.Descricao = PropertyEntity({ text: "*Descrição:", required: true, maxlength: 150 });
    this.Numero = PropertyEntity({ text: "*Número:", required: true, maxlength: 50 });
    this.Chassi = PropertyEntity({ text: "Chassi:", required: false, maxlength: 500 });
    this.Ativo = PropertyEntity({ val: ko.observable(true), options: _status, def: true, text: "*Situação: " });

    this.ModeloEquipamento = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), required: false, text: "Modelo:", idBtnSearch: guid() });
    this.MarcaEquipamento = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), required: false, text: "Marca:", idBtnSearch: guid(), issue: 142 });
    this.SegmentoVeiculo = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), required: false, text: "Segmento:", required: ko.observable(false), visible: ko.observable(true) });
    this.CentroResultado = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Centro de Resultado:", required: ko.observable(false), visible: ko.observable(true) });

    this.DataAquisicao = PropertyEntity({ getType: typesKnockout.date, text: "Data Aquisição: ", required: false, visible: ko.observable(true) });
    this.Hodometro = PropertyEntity({ getType: typesKnockout.int, text: "Hodômetro: ", required: false, maxlength: 15, configInt: { precision: 0, allowZero: false } });
    this.Horimetro = PropertyEntity({ getType: typesKnockout.int, text: "Horímetro: ", required: false, maxlength: 15, configInt: { precision: 0, allowZero: false } });
    this.AnoFabricacao = PropertyEntity({ getType: typesKnockout.int, text: "Ano Fabricação: ", required: false, maxlength: 4, configInt: { precision: 0, allowZero: false } });
    this.AnoModelo = PropertyEntity({ getType: typesKnockout.int, text: "Ano Modelo: ", required: false, maxlength: 4, configInt: { precision: 0, allowZero: false } });
    this.Observacao = PropertyEntity({ text: "Observação:", required: false, maxlength: 5000 });

    this.EquipamentoAceitaAbastecimento = PropertyEntity({ text: "Este equipamento aceita lançamento de abastecimento?", getType: typesKnockout.bool, val: ko.observable(false), def: false, visible: ko.observable(true) });
    this.UtilizaTanqueCompartilhado = PropertyEntity({ text: "Este equipamento utiliza tanque compartilhado?", getType: typesKnockout.bool, val: ko.observable(false), def: false, visible: ko.observable(true) });
    this.MediaPadrao = PropertyEntity({ getType: typesKnockout.decimal, maxlength: 11, text: "Média Padrão: ", required: ko.observable(false), visible: ko.observable(true) });
    this.CapacidadeTanque = PropertyEntity({ getType: typesKnockout.decimal, maxlength: 11, text: "Capacidade Tanque: ", required: ko.observable(false), visible: ko.observable(true) });
    this.CapacidadeMaximaTanque = PropertyEntity({ getType: typesKnockout.decimal, maxlength: 11, text: "Capacidade Máxima Tanque: ", required: ko.observable(false), visible: ko.observable(true) });
    this.EquipamentoAceitaAbastecimento.val.subscribe(function (novoValor) {
        if (!novoValor) {
            _equipamento.MediaPadrao.val("");
            _equipamento.CapacidadeTanque.val("");
            _equipamento.CapacidadeMaximaTanque.val("");
            _equipamento.UtilizaTanqueCompartilhado.val(false);
        }
    });

    this.ViradaHodometro = PropertyEntity({ text: "Ocorreu a virada do hodômetro?", getType: typesKnockout.bool, val: ko.observable(false), def: false, visible: ko.observable(true) });
    this.TrocaHorimetro = PropertyEntity({ text: "Ocorreu troca de horímetro?", getType: typesKnockout.bool, val: ko.observable(false), def: false, visible: ko.observable(true) });
    this.KilometragemVirada = PropertyEntity({ getType: typesKnockout.int, maxlength: 11, text: "*Horímetro da Virada: ", required: ko.observable(false), visible: ko.observable(false) });
    this.HorimetroAtual = PropertyEntity({ getType: typesKnockout.int, enable: ko.observable(false), maxlength: 15, text: "*Horímetro Atual: ", required: ko.observable(false), visible: ko.observable(false) });

    this.HistoricoHorimetros = PropertyEntity({ type: types.listEntity, list: new Array(), codEntity: ko.observable(0), text: "", idGrid: guid() });    
    this.NovoHorimetro = PropertyEntity({ getType: typesKnockout.int, text: "Novo Horímetro: ", required: false, maxlength: 15, configInt: { precision: 0, allowZero: false } });    
    this.DataAlteracao = PropertyEntity({ text: "*Data de Alteração: ", getType: typesKnockout.date, required: false });
    this.ObservacaoNovoHorimetro = PropertyEntity({ text: "Observação:", required: false, maxlength: 5000 });        
    this.AdicionarNovoHorimetro = PropertyEntity({ eventClick: AdicionarNovoHorimetroClick, type: types.event, text: "Adicionar", visible: ko.observable(true) });    
    this.CancelarNovoHorimetro = PropertyEntity({ eventClick: LimparCamposNovoHorimetro, type: types.event, text: "Cancelar", visible: ko.observable(false) });
    this.AtualizarNovoHorimetro = PropertyEntity({ eventClick: AtualizarNovoHorimetroClick, type: types.event, text: "Atualizar", visible: ko.observable(false) });

    this.ViradaHodometro.val.subscribe(function (novoValor) {
        _equipamento.KilometragemVirada.required(novoValor);
        _equipamento.KilometragemVirada.visible(novoValor);
        if (!novoValor)
            _equipamento.KilometragemVirada.val("");
    });

    this.TrocaHorimetro.val.subscribe(function (novoValor) {
        _equipamento.HorimetroAtual.required(novoValor);
        _equipamento.HorimetroAtual.visible(novoValor);
        _equipamento.HorimetroAtual.enable(false);

        if (!novoValor) {
            $("#liTabMudancaHorimetro").hide();
            _equipamento.HorimetroAtual.val("");
        } else {
            $("#liTabMudancaHorimetro").show();
        }

    });

    this.Neokohm = PropertyEntity({ text: "Neokohm:", options: EnumSimNao.obterOpcoes(), val: ko.observable(EnumSimNao.Nao), def: EnumSimNao.Nao, required: ko.observable(false), visible: ko.observable(true) });
    this.Cor = PropertyEntity({ text: "Cor:", maxlength: 50, visible: ko.observable(true) });
    this.Renavam = PropertyEntity({ text: "RENAVAM:", maxlength: 20, visible: ko.observable(true) });
    this.Integrado = PropertyEntity({ text: "Integrado", getType: typesKnockout.bool, visible: ko.observable(false) });
};

var CRUDEquipamento = function () {
    this.Adicionar = PropertyEntity({ eventClick: adicionarClick, type: types.event, text: "Adicionar", visible: ko.observable(true) });
    this.Atualizar = PropertyEntity({ eventClick: atualizarClick, type: types.event, text: "Atualizar", visible: ko.observable(false) });
    this.Excluir = PropertyEntity({ eventClick: excluirClick, type: types.event, text: "Excluir", visible: ko.observable(false) });
    this.Cancelar = PropertyEntity({ eventClick: cancelarClick, type: types.event, text: "Cancelar", visible: ko.observable(false) });
};

var InformarGrupoServico = function () {
    this.GrupoServico = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Grupo de Serviço:", idBtnSearch: guid(), required: ko.observable(true) });

    this.Confirmar = PropertyEntity({
        eventClick: function (e) {
            informarGrupoServicoClick(e);
        }, type: types.event, text: "Confirmar", idGrid: guid(), visible: ko.observable(true)
    });
};

//*******EVENTOS*******

function loadEquipamento() {
    $("#liTabMudancaHorimetro").hide();

    _pesquisaEquipamento = new PesquisaEquipamento();
    KoBindings(_pesquisaEquipamento, "knockoutPesquisaEquipamento", false, _pesquisaEquipamento.Pesquisar.id);

    _equipamento = new Equipamento();
    KoBindings(_equipamento, "knockoutCadastroEquipamento");

    HeaderAuditoria("Equipamento", _equipamento);

    _crudEquipamento = new CRUDEquipamento();
    KoBindings(_crudEquipamento, "knockoutCRUDCadastroEquipamento");

    _informarGrupoServico = new InformarGrupoServico();
    KoBindings(_informarGrupoServico, "divModalInformarGrupoServico");

    new BuscarMarcaEquipamentos(_equipamento.MarcaEquipamento);
    new BuscarMarcaEquipamentos(_pesquisaEquipamento.MarcaEquipamento);
    new BuscarVeiculos(_pesquisaEquipamento.Veiculo);
    new BuscarModeloEquipamentos(_equipamento.ModeloEquipamento, null, null, RetornoModeloEquipamento);
    new BuscarSegmentoVeiculo(_equipamento.SegmentoVeiculo);
    new BuscarCentroResultado(_equipamento.CentroResultado);
    new BuscarGrupoServico(_informarGrupoServico.GrupoServico);

    buscarMarcasEquipamento();
    _modalinformarGrupoServico = new bootstrap.Modal(document.getElementById("divModalInformarGrupoServico"), { backdrop: true, keyboard: true });

    loadEquipamentoNovoHorimetro();

    if (_CONFIGURACAO_TMS.TipoServicoMultisoftware == EnumTipoServicoMultisoftware.MultiEmbarcador) {
        _equipamento.SegmentoVeiculo.visible(false);
        _equipamento.CentroResultado.visible(false);
        _equipamento.Neokohm.visible(false);
        _equipamento.EquipamentoAceitaAbastecimento.visible(false);
        _equipamento.ViradaHodometro.visible(false);
    }
}

function RetornoModeloEquipamento(data) {
    _equipamento.ModeloEquipamento.val(data.Descricao);
    _equipamento.ModeloEquipamento.codEntity(data.Codigo);

    if (data.CodigoMarca > 0) {
        _equipamento.MarcaEquipamento.val(data.DescricaoMarca);
        _equipamento.MarcaEquipamento.codEntity(data.CodigoMarca);
    }
}

function adicionarClick(e, sender) {
    if (!ValidarCamposObrigatorios(_equipamento)) {
        exibirCamposObrigatorio();
        return;
    }

    if (_CONFIGURACAO_TMS.GerarOSAutomaticamenteCadastroVeiculoEquipamento)
        exibirConfirmacao("Confirmação", "Deseja gerar uma Ordem de Serviço para o novo equipamento?", function () {
            _modalinformarGrupoServico.show();
        }, function () {
            adicionarEquipamento();
        });
    else
        adicionarEquipamento();
}

function adicionarEquipamento() {
    Salvar(_equipamento, "Equipamento/Adicionar", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Cadastrado com sucesso.");
                _gridEquipamento.CarregarGrid();
                limparCamposEquipamento();
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    });
}

function atualizarClick(e, sender) {
    Salvar(_equipamento, "Equipamento/Atualizar", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Atualizado com sucesso");
                _gridEquipamento.CarregarGrid();
                limparCamposEquipamento();
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    }, sender, exibirCamposObrigatorio);
}

function excluirClick(e, sender) {
    exibirConfirmacao("Confirmação", "Realmente deseja excluir o equipamento " + _equipamento.Descricao.val() + "?", function () {
        ExcluirPorCodigo(_equipamento, "Equipamento/ExcluirPorCodigo", function (arg) {
            if (arg.Success) {
                if (arg.Data) {
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Excluído com sucesso");
                    _gridEquipamento.CarregarGrid();
                    limparCamposEquipamento();
                } else {
                    exibirMensagem(tipoMensagem.aviso, "Sugestão", arg.Msg, 16000);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
            }

        }, null);
    });
}

function cancelarClick(e) {
    limparCamposEquipamento();
}

function informarGrupoServicoClick(e) {
    var valido = ValidarCamposObrigatorios(e);
    if (!valido) {
        exibirCamposObrigatorio();
        return;
    }

    _equipamento.GrupoServico.codEntity(e.GrupoServico.codEntity());
    _equipamento.GrupoServico.val(e.GrupoServico.val());
    _modalinformarGrupoServico.hide();

    adicionarEquipamento();
}

//*******MÉTODOS*******

function editarEquipamento(equipamentoGrid) {
    limparCamposEquipamento();
    _equipamento.Codigo.val(equipamentoGrid.Codigo);
    BuscarPorCodigo(_equipamento, "Equipamento/BuscarPorCodigo", function (arg) {
        _pesquisaEquipamento.ExibirFiltros.visibleFade(false);
        _crudEquipamento.Atualizar.visible(true);
        _crudEquipamento.Cancelar.visible(true);
        _crudEquipamento.Excluir.visible(true);
        _crudEquipamento.Adicionar.visible(false);
        carregarGridHorimetros(arg.Data.HistoricoHorimetros);
    }, null);
}

function buscarMarcasEquipamento() {
    var editar = { descricao: "Editar", id: "clasEditar", evento: "onclick", metodo: editarEquipamento, tamanho: "20", icone: "" };
    var menuOpcoes = new Object();
    menuOpcoes.tipo = TypeOptionMenu.link;
    menuOpcoes.opcoes = new Array();
    menuOpcoes.opcoes.push(editar);

    _gridEquipamento = new GridView(_pesquisaEquipamento.Pesquisar.idGrid, "Equipamento/Pesquisa", _pesquisaEquipamento, menuOpcoes, null);
    _gridEquipamento.CarregarGrid();
}

function limparCamposEquipamento() {
    _crudEquipamento.Atualizar.visible(false);
    _crudEquipamento.Cancelar.visible(false);
    _crudEquipamento.Excluir.visible(false);
    _crudEquipamento.Adicionar.visible(true);
    LimparCampos(_equipamento);
    LimparCampos(_informarGrupoServico);
    recarregarGridEquipamentoNovosHorimetros();
}

function exibirCamposObrigatorio() {
    exibirMensagem(tipoMensagem.atencao, "Campo Obrigatório", "Por Favor, Informe os campos obrigatórios");
}
