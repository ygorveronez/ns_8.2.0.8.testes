/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Globais.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="../../Consultas/MotivoIrregularidade.js" />
/// <reference path="../../Consultas/SetorFuncionario.js" />
/// <reference path="../../Consultas/GrupoTipoOperacao.js" />

/*
 * Declaração de Objetos Globais do Arquivo
 */

var _tratativaIrregularidade;

/*
 * Declaração das Classes
 */

var TratativaIrregularidade = function () {

    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Sequencia = PropertyEntity({ getType: typesKnockout.int, text: "*Sequência:", val: ko.observable(0), def: 0, required: true });
    this.Setor = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "*Setor:", idBtnSearch: guid(), required: true, enable: ko.observable(true) });
    this.ProximaSequencia = PropertyEntity({ getType: typesKnockout.int, val: ko.observable(0), def: 0, text: "Próxima Sequência" });
    this.GrupoOperacao = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Grupo de Tipo de Operação:", idBtnSearch: guid() });
    this.InformarMotivo = PropertyEntity({ getType: typesKnockout.bool, text: "Informar Motivo(s)" });

    this.TratativaIrregularidadeMotivos = PropertyEntity({ text: "Adicionar Motivo", type: types.map, required: ko.observable(false), getType: typesKnockout.dynamic, idGrid: guid(), idBtnSearch: guid(), visible: ko.observable(true) });
    this.TratativaIrregularidadeMotivos.val.subscribe(function () {
        renderizarGridTratativaIrregularidadeMotivos();
    });

    this.TratativaIrregularidadeAcoes = ko.observableArray([]);

    this.InformarMotivo.val.subscribe(function (newVal) {
        _tratativaIrregularidade.TratativaIrregularidadeMotivos.required(newVal);
    });

    this.Adicionar = PropertyEntity({ type: types.event, eventClick: AdicionarTratativaClick, text: "Adicionar", visible: ko.observable(true), enable: ko.observable(true), idBtnSearch: guid() });
    this.Atualizar = PropertyEntity({ type: types.event, eventClick: AtualizarTratativaClick, text: "Atualizar", visible: ko.observable(false), enable: ko.observable(false), idBtnSearch: guid() });
    this.Cancelar = PropertyEntity({ type: types.event, eventClick: LimparCamposTratativaIrregularidadeClick, text: "Cancelar", visible: ko.observable(true), enable: ko.observable(true), idBtnSearch: guid() });
}


function LoadTratativaIrregularidade() {

    _tratativaIrregularidade = new TratativaIrregularidade();
    KoBindings(_tratativaIrregularidade, "knockoutTratativasIrregularidade");

    loadGridTratativaIrregularidadeMotivos();

    BuscarSetorFuncionario(_tratativaIrregularidade.Setor);
    BuscarGrupoTipoOperacao(_tratativaIrregularidade.GrupoOperacao);
}

//*******EVENTOS*******

function ExcluirTratativaClick(tratativa) {

    let data = _definicaoTratativasIrregularidade.TratativasIrregularidade.val();

    for (let i = 0; i < data.length; i++) {
        if (tratativa.Codigo == data[i].Codigo) {
            data.splice(i, 1);

            break;
        }
    }

    _definicaoTratativasIrregularidade.TratativasIrregularidade.val(data);
}

function EditarTratativaClick(tratativa) {
    LimparCamposTratativaIrregularidadeClick();
    GerarCheckboxsAcoes(tratativa.Gatilho);
    ConfigurarAcoesSelecionadas(tratativa);

    let data = _definicaoTratativasIrregularidade.TratativasIrregularidade.val();

    for (var i = 0; i < data.length; i++) {
        let element = data[i];
        if (tratativa.Codigo == element.Codigo) {
            _tratativaIrregularidade.Codigo.val(element.Codigo);
            _tratativaIrregularidade.Sequencia.val(element.Sequencia);
            _tratativaIrregularidade.Setor.val(element.Setor);
            _tratativaIrregularidade.Setor.codEntity(element.CodigoSetor);
            _tratativaIrregularidade.ProximaSequencia.val(element.ProximaSequencia);
            _tratativaIrregularidade.GrupoOperacao.val(element.GrupoOperacao);
            _tratativaIrregularidade.GrupoOperacao.codEntity(element.CodigoGrupoOperacao);
            _tratativaIrregularidade.InformarMotivo.val(element.InformarMotivo);
            _tratativaIrregularidade.TratativaIrregularidadeMotivos.val(element.Motivos);
            ConfigurarAcoesSelecionadas(element);
            break;
        }
    }

    _tratativaIrregularidade.Adicionar.visible(false);
    _tratativaIrregularidade.Adicionar.enable(false);
    _tratativaIrregularidade.Atualizar.visible(true);
    _tratativaIrregularidade.Atualizar.enable(true);

    Global.ResetarMultiplasAbas();
    Global.abrirModal('divAdicionarTratativa');

}

function LimparCamposTratativaIrregularidadeClick() {
    LimparCampos(_tratativaIrregularidade);
    recarregarGridTratativaIrregularidadeMotivos();

    _tratativaIrregularidade.Atualizar.visible(false);
    _tratativaIrregularidade.Atualizar.enable(false);
    _tratativaIrregularidade.Adicionar.visible(true);
    _tratativaIrregularidade.Adicionar.enable(true);
}

function AdicionarTratativaClick() {

    if (!ValidarTratativa())
        return;

    let data = _gridTratativasIrregularidade.BuscarRegistros();

    let Tratativa = {

        Codigo: guid(),
        Sequencia: _tratativaIrregularidade.Sequencia.val(),
        Setor: _tratativaIrregularidade.Setor.val(),
        CodigoSetor: _tratativaIrregularidade.Setor.codEntity(),
        ProximaSequencia: _tratativaIrregularidade.ProximaSequencia.val() == '' ? 0 : _tratativaIrregularidade.ProximaSequencia.val(),
        GrupoOperacao: _tratativaIrregularidade.GrupoOperacao.val(),
        CodigoGrupoOperacao: _tratativaIrregularidade.GrupoOperacao.codEntity(),
        InformarMotivo: _tratativaIrregularidade.InformarMotivo.val(),
        Motivos: _gridTratativaIrregularidadeMotivos.BuscarRegistros(),
        Acoes: ObterAcoesSelecionadas()
    };

    data.push(Tratativa);
    _definicaoTratativasIrregularidade.TratativasIrregularidade.val(data);

    Global.fecharModal('divAdicionarTratativa');
    LimparCamposTratativaIrregularidadeClick();
}

function AtualizarTratativaClick() {

    if (!ValidarTratativa())
        return;

    let data = _gridTratativasIrregularidade.BuscarRegistros();

    for (var i = 0; i < data.length; i++) {
        let element = data[i];
        if (_tratativaIrregularidade.Codigo.val() == element.Codigo) {
            element.Codigo = _tratativaIrregularidade.Codigo.val();
            element.Sequencia = _tratativaIrregularidade.Sequencia.val();
            element.Setor = _tratativaIrregularidade.Setor.val();
            element.CodigoSetor = _tratativaIrregularidade.Setor.codEntity();
            element.ProximaSequencia = _tratativaIrregularidade.ProximaSequencia.val() == '' ? 0 : _tratativaIrregularidade.ProximaSequencia.val(),
            element.GrupoOperacao = _tratativaIrregularidade.GrupoOperacao.val();
            element.CodigoGrupoOperacao = _tratativaIrregularidade.GrupoOperacao.codEntity();
            element.InformarMotivo = _tratativaIrregularidade.InformarMotivo.val();
            element.Motivos = _gridTratativaIrregularidadeMotivos.BuscarRegistros();
            element.Acoes = ObterAcoesSelecionadas();

            break;
        }
    }

    _definicaoTratativasIrregularidade.TratativasIrregularidade.val(data);

    Global.fecharModal('divAdicionarTratativa');
    LimparCamposTratativaIrregularidadeClick();
}

//*******MÉTODOS*******

function ValidarTratativa() {

    var msg = "";

    if (_tratativaIrregularidade.Sequencia.val() == "" || _tratativaIrregularidade.Sequencia.val() == 0)
        msg = ValidarSequencia(msg, _tratativaIrregularidade.Sequencia.val());

    if (_tratativaIrregularidade.Setor.val() == "" || _tratativaIrregularidade.Setor.codEntity() == 0)
        msg = "É necessário informar um Setor";

    //msg = ValidarAcao(_tratativaIrregularidade.TratativaIrregularidadeAcoes(), msg);

    if (_tratativaIrregularidade.ProximaSequencia.val() != "" || _tratativaIrregularidade.ProximaSequencia.val() == 0)
        msg = ValidarProximaSequencia(msg, _tratativaIrregularidade.ProximaSequencia.val());

    if (_tratativaIrregularidade.InformarMotivo.val() && (_gridTratativaIrregularidadeMotivos.BuscarRegistros().length == 0 || _gridTratativaIrregularidadeMotivos.BuscarRegistros() === undefined))
        msg = "É necessário informar um Motivo";

    if (msg != "")
        exibirMensagem(tipoMensagem.atencao, "Campo(s) Inválido(s)", msg);

    ValidarCamposObrigatorios(_tratativaIrregularidade);

    return msg == "";
}

function converterTipoParaGrid(itens) {
    for (var i = 0; i < itens.length; i++) {
        let element = itens[i]
        if (element.Tipo == 1)
            element.Tipo = "Entrada"

        else if (element.Tipo == 2)
            element.Tipo = "Saída"
    }

}

function ValidarSequencia(msg, seq) {

    var tratativas = _definicaoTratativasIrregularidade.TratativasIrregularidade.val();
    for (var i = 0; i < tratativas.length; i++) {
        let element = tratativas[i]
        if (element.Sequencia == seq) {
            msg = "A Sequência já existe em outra Tratativa desta Definição";
            break;
        }
    }

    return msg;
}

function ValidarProximaSequencia(msg, proximaSeq) {

    if (proximaSeq == _tratativaIrregularidade.Sequencia.val()) {
        return "Próxima Sequência não pode ser igual a Sequência";
    }

    if (proximaSeq < _tratativaIrregularidade.Sequencia.val() && (proximaSeq != 0 && !string.IsNullOrWhiteSpace(proximaSeq))) {
        return "Próxima Sequência não pode ser menor que a Sequência";
    }

    return msg;
}

function ValidarAcao(acoes, msg) {

    let NenhumaAcaoSelecionada = true;
    for (var i = 0; i < acoes.length; i++) {
        let element = acoes[i];
        if (element.val())
            NenhumaAcaoSelecionada = false
    }

    if (NenhumaAcaoSelecionada)
        return "É necessário selecionar pelo menos uma Ação";
    else
        return msg;
}

function LimparCamposTratativaIrregularidade() {
    LimparCamposTratativaIrregularidadeClick();
}
