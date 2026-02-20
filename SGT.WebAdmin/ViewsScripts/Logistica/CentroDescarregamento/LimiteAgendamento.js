/// <reference path="../../Consultas/GrupoPessoa.js" />
/// <reference path="../../Enumeradores/EnumDiaSemana.js" />

//#region Váriaveis Globais

var _limiteAgendamento;
var _limiteAgendamentoPadrao;
var _gridLimiteAgendamento;

//#endregion


//#region Mapeamento Knockout

var LimiteAgendamentoPadrao = function () {
    this.LimitePadrao = PropertyEntity({ type: types.map, getType: typesKnockout.int, required: false, text: Localization.Resources.Logistica.CentroDescarregamento.LimitePadraoDiario.getFieldDescription(), val: ko.observable(""), def: "" });

    this.LimitePadrao.val.subscribe(function (novoValor) {
        _centroDescarregamento.LimitePadrao.val(novoValor);
    });
}

var LimiteAgendamento = function () {
    this.Lista = PropertyEntity({ type: types.local });
    this.Grid = PropertyEntity({ type: types.local });

    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.NovoLimite = PropertyEntity({ type: types.map, getType: typesKnockout.int, required: true, text: Localization.Resources.Logistica.CentroDescarregamento.NovoLimite.getRequiredFieldDescription(), val: ko.observable(""), def: "" });
    this.PermiteUltrapassarLimiteVolume = PropertyEntity({ type: types.map, getType: typesKnockout.bool, required: false, text: Localization.Resources.Logistica.CentroDescarregamento.PermiteUltrapassarLimiteVolume.getRequiredFieldDescription(), val: ko.observable(false), def: false, valorDescritivo: Localization.Resources.Gerais.Geral.Nao });
    this.GrupoPessoa = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), required: true, text: Localization.Resources.Logistica.CentroDescarregamento.GrupoPessoa.getRequiredFieldDescription(), idBtnSearch: guid() });

    this.PermiteUltrapassarLimiteVolume.val.subscribe(function (novoValor) {
        _limiteAgendamento.PermiteUltrapassarLimiteVolume.valorDescritivo = novoValor ? Localization.Resources.Gerais.Geral.Sim : Localization.Resources.Gerais.Geral.Nao;
    });

    this.Adicionar = PropertyEntity({ eventClick: adicionarLimiteAgendamentoClick, type: types.event, text: Localization.Resources.Gerais.Geral.Adicionar, visible: ko.observable(true) });
    this.Atualizar = PropertyEntity({ eventClick: atualizarLimiteAgendamentoClick, type: types.event, text: Localization.Resources.Gerais.Geral.Atualizar, visible: ko.observable(false) });
    this.Excluir = PropertyEntity({ eventClick: excluirLimiteAgendamentoClick, type: types.event, text: Localization.Resources.Gerais.Geral.Excluir, visible: ko.observable(false) });
    this.Cancelar = PropertyEntity({ eventClick: cancelarLimiteAgendamentoClick, type: types.event, text: Localization.Resources.Gerais.Geral.Cancelar, visible: ko.observable(false) });
}

function loadLimiteAgendamento() {
    _limiteAgendamento = new LimiteAgendamento();
    KoBindings(_limiteAgendamento, "knockoutLimiteAgendamentos");

    _limiteAgendamentoPadrao = new LimiteAgendamentoPadrao();
    KoBindings(_limiteAgendamentoPadrao, "knockoutLimiteAgendamentoPadrao");

    new BuscarGruposPessoas(_limiteAgendamento.GrupoPessoa);

    loadGridLimiteAgendamento();
}

function loadGridLimiteAgendamento() {
    var menuOpcoes = { tipo: TypeOptionMenu.link, tamanho: 7, opcoes: [{ descricao: Localization.Resources.Gerais.Geral.Editar, id: guid(), metodo: editarLimiteAgendamento }] };

    var header = [
        { data: "Codigo", visible: false },
        { data: "CodigoGrupoPessoa", visible: false },
        { data: "PermiteUltrapassarLimiteVolume", visible: false },
        { data: "DescricaoGrupoPessoa", title: Localization.Resources.Logistica.CentroDescarregamento.GrupoPessoa, width: "25%" },
        { data: "Limite", title: Localization.Resources.Gerais.Geral.Limite, width: "20%" },
        { data: "PermiteUltrapassarLimiteVolumeDescricao", title: Localization.Resources.Logistica.CentroDescarregamento.PermiteUltrapassarLimiteVolume, width: "20%" },
    ];

    _gridLimiteAgendamento = new BasicDataTable(_limiteAgendamento.Grid.id, header, menuOpcoes, { column: 1, dir: orderDir.asc });
    recarregarGridLimiteAgendamento();
}

//#endregion

//#region Métodos Públicos

function editarLimiteAgendamento(registroSelecionado) {
    for (var i = 0; i < _centroDescarregamento.LimiteAgendamento.list.length; i++) {
        var registroAtual = _centroDescarregamento.LimiteAgendamento.list[i];
        if (registroSelecionado.Codigo == registroAtual.Codigo.val) {
            _limiteAgendamento.Codigo.val(registroAtual.Codigo.val);
            _limiteAgendamento.NovoLimite.val(registroAtual.NovoLimite.val);
            _limiteAgendamento.GrupoPessoa.val(registroAtual.GrupoPessoa.val);
            _limiteAgendamento.GrupoPessoa.codEntity(registroAtual.GrupoPessoa.codEntity);
            _limiteAgendamento.PermiteUltrapassarLimiteVolume.val(registroAtual.PermiteUltrapassarLimiteVolume.val);
            
            _limiteAgendamento.Adicionar.visible(false);
            _limiteAgendamento.Atualizar.visible(true);
            _limiteAgendamento.Cancelar.visible(true);
            _limiteAgendamento.Excluir.visible(true);
        }
    }
}

//#endregion

//#region Métodos Privados

function limparCamposLimiteAgendamento() {
    LimparCampos(_limiteAgendamento);

    _limiteAgendamento.Adicionar.visible(true);
    _limiteAgendamento.Atualizar.visible(false);
    _limiteAgendamento.Cancelar.visible(false);
    _limiteAgendamento.Excluir.visible(false);
}

function limparCamposLimiteAgendamentoPadrao() {
    LimparCampos(_limiteAgendamentoPadrao);
}

function recarregarGridLimiteAgendamento() {
    var data = new Array();

    $.each(_centroDescarregamento.LimiteAgendamento.list, function (i, limiteAgendamento) {
        var limiteAgendamentoGrid = new Object();

        limiteAgendamentoGrid.Codigo = limiteAgendamento.Codigo.val;
        limiteAgendamentoGrid.CodigoGrupoPessoa = limiteAgendamento.GrupoPessoa.codEntity;
        limiteAgendamentoGrid.DescricaoGrupoPessoa = limiteAgendamento.GrupoPessoa.val;
        limiteAgendamentoGrid.Limite = limiteAgendamento.NovoLimite.val;
        limiteAgendamentoGrid.PermiteUltrapassarLimiteVolume = limiteAgendamento.PermiteUltrapassarLimiteVolume.val;
        limiteAgendamentoGrid.PermiteUltrapassarLimiteVolumeDescricao = limiteAgendamento.PermiteUltrapassarLimiteVolume.val ? Localization.Resources.Gerais.Geral.Sim : Localization.Resources.Gerais.Geral.Nao;

        data.push(limiteAgendamentoGrid);
    });

    _gridLimiteAgendamento.CarregarGrid(data);
}

function atualizarLimiteAgendamentoPorCodigo(codigo, limiteAgendamento) {
    for (var i = 0; i < _centroDescarregamento.LimiteAgendamento.list.length; i++) {
        if (codigo == _centroDescarregamento.LimiteAgendamento.list[i].Codigo.val) {
            _centroDescarregamento.LimiteAgendamento.list[i] = limiteAgendamento;
            break;
        }
    }
}

//#endregion

//#region Eventos

function adicionarLimiteAgendamentoClick() {
    var valido = ValidarCamposObrigatorios(_limiteAgendamento);

    if (!valido) {
        exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.CamposObrigatorios, Localization.Resources.Gerais.Geral.InformeCamposObrigatorios);
        return;
    }

    if (!validarExistenciaGrupoPessoa()) {
        exibirMensagem(tipoMensagem.aviso, Localization.Resources.Logistica.CentroDescarregamento.ConfiguracaoExistente, Localization.Resources.Logistica.CentroDescarregamento.JaFoiCadastradaConfiguracaoGrupPessoa);
        return;
    }

    _limiteAgendamento.Codigo.val(guid());
    _centroDescarregamento.LimiteAgendamento.list.push(SalvarListEntity(_limiteAgendamento));

    recarregarGridLimiteAgendamento();
    limparCamposLimiteAgendamento();
}

function atualizarLimiteAgendamentoClick() {
    for (var i = 0; i < _centroDescarregamento.LimiteAgendamento.list.length; i++) {
        var registroAtual = _centroDescarregamento.LimiteAgendamento.list[i];
        if (registroAtual.Codigo.val == _limiteAgendamento.Codigo.val()) {
            if (!validarExistenciaGrupoPessoa()) {
                exibirMensagem(tipoMensagem.aviso, Localization.Resources.Logistica.CentroDescarregamento.ConfiguracaoExistente, Localization.Resources.Logistica.CentroDescarregamento.JaFoiCadastradaConfiguracaoGrupPessoa);
                return; 
            }

            var limiteAgendamento = SalvarListEntity(_limiteAgendamento);
            atualizarLimiteAgendamentoPorCodigo(_limiteAgendamento.Codigo.val(), limiteAgendamento);

            recarregarGridLimiteAgendamento();
            limparCamposLimiteAgendamento();

            break;
        }
    }
}

function excluirLimiteAgendamentoClick() {
    for (var i = 0; i < _centroDescarregamento.LimiteAgendamento.list.length; i++) {
        var registroAtual = _centroDescarregamento.LimiteAgendamento.list[i];
        if (registroAtual.Codigo.val = _limiteAgendamento.Codigo.val()) {
            _centroDescarregamento.LimiteAgendamento.list.splice(i, 1);
            recarregarGridLimiteAgendamento();
            limparCamposLimiteAgendamento();
            break;
        }
    }
}

function cancelarLimiteAgendamentoClick() {
    limparCamposLimiteAgendamento();
}

function validarExistenciaGrupoPessoa() {
    for (var i = 0; i < _centroDescarregamento.LimiteAgendamento.list.length; i++) {
        if (_centroDescarregamento.LimiteAgendamento.list[i].GrupoPessoa.codEntity == _limiteAgendamento.GrupoPessoa.codEntity() && _limiteAgendamento.Codigo.val() != _centroDescarregamento.LimiteAgendamento.list[i].Codigo.val) {
            return false;
        }
    }

    return true;
}

//#endregion