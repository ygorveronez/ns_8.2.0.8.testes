/// <reference path="../../Enumeradores/EnumTipoOpcaoCheckList.js" />
/// <reference path="Checklist.js" />
/// <reference path="ChecklistPerguntaOpcoes.js" />

//////*******MAPEAMENTO KNOUCKOUT*******

var _gridChecklistPergunta;
var _checklistPergunta;
var _crudChecklistPergunta;

var ChecklistPergunta = function () {
    this.Grid = PropertyEntity({ type: types.local });

    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Ordem = PropertyEntity({ getType: typesKnockout.int, text: "Ordem:", val: ko.observable(0), def: 0, visible: ko.observable(true), maxlength: 5 });
    this.Descricao = PropertyEntity({ text: "*Descrição:", maxlength: 500, val: ko.observable(""), def: "", visible: ko.observable(true), required: true });
    this.CodigoIntegracao = PropertyEntity({ text: "Código de integração:", val: ko.observable(""), visible: ko.observable(true), def: "" });
    this.TipoResposta = PropertyEntity({ text: "Tipo de Resposta: ", options: EnumTipoOpcaoCheckList.obterOpcoes(), visible: ko.observable(true), val: ko.observable(EnumTipoOpcaoCheckList.Aprovacao), def: EnumTipoOpcaoCheckList.Aprovacao });
    this.Obrigatorio = PropertyEntity({ getType: typesKnockout.bool, text: "Obrigatório", val: ko.observable(false), visible: ko.observable(true), def: false });
    this.PermiteOpcaoNaoSeAplica = PropertyEntity({ getType: typesKnockout.bool, text: 'Permite opção "Não se Aplica (N/A)"', visible: ko.observable(true), val: ko.observable(false), def: false });

    this.Perguntas = PropertyEntity({ type: types.listEntity, list: new Array(), codEntity: ko.observable(0), text: "", idGrid: guid() });

    this.CodigoOpcao = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.OrdemOpcao = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.DescricaoOpcao = PropertyEntity({ text: "Descrição:", getType: typesKnockout.string, maxlength: 1000, eventKeydown: adicionarOpcaoEnter });
    this.OpcaoImpeditiva = PropertyEntity({ text: "Resposta Impeditiva", getType: typesKnockout.bool, def: false, val: ko.observable(false) });
    this.ValorOpcao = PropertyEntity({ text: "Valor:", getType: typesKnockout.int, visible: ko.observable(false), eventKeydown: adicionarOpcaoEnter });
    this.CodigoIntegracaoOpcao = PropertyEntity({ text: "Código de integração: ", val: ko.observable(null), getType: typesKnockout.int });

    this.Opcoes = PropertyEntity({ idGrid: guid(), val: ko.observable(new Array()), list: [], visible: ko.observable(false) });
    this.OpcoesSalvar = PropertyEntity({ getType: typesKnockout.text, val: ko.observable(""), visible: ko.observable(false) });

    this.AdicionarOpcao = PropertyEntity({ eventClick: adicionarOpcao, type: types.event, text: "Adicionar", visible: ko.observable(true) });
    this.CancelarOpcao = PropertyEntity({ eventClick: cancelarOpcao, type: types.event, text: "Cancelar", visible: ko.observable(true) });
    this.AtualizarOpcao = PropertyEntity({ eventClick: atualizarOpcao, type: types.event, text: "Atualizar", visible: ko.observable(false) });
    this.ExcluirOpcao = PropertyEntity({ eventClick: excluirOpcao, type: types.event, text: "Excluir", visible: ko.observable(false) });

};

var CRUDChecklistPergunta = function () {
    this.Adicionar = PropertyEntity({ eventClick: adicionarPerguntaClick, type: types.event, text: "Adicionar Pergunta", visible: ko.observable(true) });
    this.Atualizar = PropertyEntity({ eventClick: atualizarPerguntaClick, type: types.event, text: "Atualizar Pergunta", visible: ko.observable(false) });
    this.Excluir = PropertyEntity({ eventClick: excluirPerguntaClick, type: types.event, text: "Excluir Pergunta", visible: ko.observable(false) });
    this.Cancelar = PropertyEntity({ eventClick: cancelarPerguntaClick, type: types.event, text: Localization.Resources.Gerais.Geral.Cancelar, visible: ko.observable(false) });
};

function loadChecklistPergunta() {
    _checklistPergunta = new ChecklistPergunta();
    KoBindings(_checklistPergunta, "knockoutChecklistPergunta");

    _crudChecklistPergunta = new CRUDChecklistPergunta();
    KoBindings(_crudChecklistPergunta, "knockoutCRUDChecklistPergunta");

    GridOpcoes();

    _checklistPergunta.TipoResposta.val.subscribe(onTipoCheckListChange);

    loadGridChecklistPergunta();
    recarregarGridPergunta();
}

function onTipoCheckListChange() {
    var typesEnableOptions = [EnumTipoOpcaoCheckList.Selecoes, EnumTipoOpcaoCheckList.Opcoes, EnumTipoOpcaoCheckList.Escala];

    if (typesEnableOptions.indexOf(_checklistPergunta.TipoResposta.val()) >= 0)
        _checklistPergunta.Opcoes.visible(true);
    else
        _checklistPergunta.Opcoes.visible(false);

    if (isEscala())
        _checklistPergunta.ValorOpcao.visible(true);
    else
        _checklistPergunta.ValorOpcao.visible(false);

}

function isEscala() {
    return _checklistPergunta.TipoResposta.val() === EnumTipoOpcaoCheckList.Escala;
}

function loadGridChecklistPergunta() {
    var menuOpcoes = { tipo: TypeOptionMenu.link, tamanho: 10, opcoes: [{ descricao: Localization.Resources.Gerais.Geral.Editar, id: guid(), metodo: editarPerguntaClick }] };

    var header = [
        { data: "Codigo", visible: false },
        { data: "Ordem", visible: false },
        { data: "Descricao", title: "Descrição", width: "40%" },
        { data: "CodigoIntegracao", title: "Código de integração", width: "20%" },
    ];

    _gridChecklistPergunta = new BasicDataTable(_checklistPergunta.Perguntas.idGrid, header, menuOpcoes, { column: 1, dir: orderDir.asc });
}

function editarPerguntaClick(data) {
    limparCamposPerguntas();
    $.each(_checklist.Perguntas.list, function (i, pergunta) {
        if (pergunta.Codigo.val == data.Codigo) {

            _checklistPergunta.Codigo.val(pergunta.Codigo.val);
            _checklistPergunta.Ordem.val(pergunta.Ordem.val);
            _checklistPergunta.Descricao.val(pergunta.Descricao.val);
            _checklistPergunta.CodigoIntegracao.val(pergunta.CodigoIntegracao.val);
            _checklistPergunta.TipoResposta.val(pergunta.TipoResposta.val);
            _checklistPergunta.Obrigatorio.val(pergunta.Obrigatorio.val);
            _checklistPergunta.PermiteOpcaoNaoSeAplica.val(pergunta.PermiteOpcaoNaoSeAplica.val);

            //_checklistPergunta.Opcoes.val = JSON.parse(_checklist.Perguntas.list[i].Opcoes.val)
            _checklistPergunta.Opcoes.list = _checklist.Perguntas.list[i].Opcoes.list

            //_checklistPergunta.Opcoes.val(pergunta.Opcoes.val);

            EditarOpcoes();

            _crudChecklistPergunta.Atualizar.visible(true);
            _crudChecklistPergunta.Cancelar.visible(true);
            _crudChecklistPergunta.Excluir.visible(true);
            _crudChecklistPergunta.Adicionar.visible(false);

            return;
        }
    });
}

function adicionarPerguntaClick(e, sender) {
    if (ValidarCamposObrigatorios(_checklistPergunta)) {

        _checklistPergunta.Codigo.val(guid());
        _checklist.Perguntas.list.push(SalvarListEntity(_checklistPergunta));

        $.each(_checklist.Perguntas.list, function (i, pergunta) {
            if (pergunta.Codigo.val == _checklistPergunta.Codigo.val()) {
                //_checklistPergunta.Opcoes.val = JSON.parse(_checklist.Perguntas.list[i].Opcoes.val)
                _checklist.Perguntas.list[i].Opcoes.val = _checklistPergunta.Opcoes.list;
                _checklist.Perguntas.list[i].Opcoes.list = _checklistPergunta.Opcoes.list;
                return;
            }
        });

        recarregarGridPergunta();

        $("#" + _checklistPergunta.Descricao.id).focus();

        limparCamposPerguntas();

    } else {
        exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.CamposObrigatorios, Localization.Resources.Gerais.Geral.InformeCamposObrigatorios);
    }
}

function atualizarPerguntaClick(e, sender) {
    if (ValidarCamposObrigatorios(_checklistPergunta)) {
        $.each(_checklist.Perguntas.list, function (i, pergunta) {
            if (pergunta.Codigo.val == _checklistPergunta.Codigo.val()) {

                _checklist.Perguntas.list[i].CodigoIntegracao.val = _checklistPergunta.CodigoIntegracao.val();
                _checklist.Perguntas.list[i].Ordem.val = _checklistPergunta.Ordem.val();
                _checklist.Perguntas.list[i].Descricao.val = _checklistPergunta.Descricao.val();
                _checklist.Perguntas.list[i].Opcoes.val = _checklistPergunta.Opcoes.list;
                _checklist.Perguntas.list[i].Opcoes.list = _checklistPergunta.Opcoes.list;

                return;
            }
        });

        limparCamposPerguntas();
        recarregarGridPergunta();
    } else {
        exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.CamposObrigatorios, Localization.Resources.Gerais.Geral.InformeCamposObrigatorios);
    }
}

function excluirPerguntaClick() {
    $.each(_checklist.Perguntas.list, function (i, pergunta) {
        if (_checklistPergunta.Codigo.val() == pergunta.Codigo.val) {
            _checklist.Perguntas.list.splice(i, 1);
            return false;
        }
    });

    limparCamposPerguntas();
    recarregarGridPergunta();
}

function cancelarPerguntaClick(e) {
    limparCamposPerguntas();
}

function recarregarGridPergunta() {
    var data = [];

    $.each(_checklist.Perguntas.list, function (i, pergunta) {
        data.push({
            Codigo: pergunta.Codigo.val,
            Ordem: pergunta.Ordem.val,
            Descricao: pergunta.Descricao.val,
            CodigoIntegracao: pergunta.CodigoIntegracao.val,
            TipoResposta: pergunta.TipoResposta.val,
        });
    });

    _gridChecklistPergunta.CarregarGrid(data);
}

function limparCamposPerguntas() {

    _crudChecklistPergunta.Atualizar.visible(false);
    _crudChecklistPergunta.Excluir.visible(false);
    _crudChecklistPergunta.Cancelar.visible(false);
    _crudChecklistPergunta.Adicionar.visible(true);

    LimparCampos(_checklistPergunta);
    _checklistPergunta.Opcoes.list = [];

    RecarregarGridOpcoes();
}


function preencherPerguntas(listaRelacaoPergunta) {
    //_checklist.Perguntas.list = new Array();
    $.each(_checklist.Perguntas.list, function (i, pergunta) {
        if (_checklist.Perguntas.list[i].Codigo.val == pergunta.Codigo.val) {

            if (pergunta.Opcoes.list.length == 0) {
                _checklist.Perguntas.list[i].Opcoes.list = new Array();
                //_checklist.Perguntas.list[i].Opcoes.val = "";
            } else {
                var opcoes = new Array();

                $.each(_checklist.Perguntas.list[i].Opcoes.list, function (j, opcao) {
                    var opcao = {
                        Codigo: opcao.CodigoOpcao.val,
                        Ordem: opcao.OrdemOpcao.val,
                        Descricao: opcao.DescricaoOpcao.val,
                        Valor: opcao.ValorOpcao.val,
                        CodigoIntegracao: opcao.CodigoIntegracaoOpcao.val,
                        OpcaoImpeditiva: opcao.OpcaoImpeditiva.val,
                    };

                    opcoes.push(opcao);
                });

                _checklist.Perguntas.list[i].Opcoes.list = opcoes.slice();
                //_checklist.Perguntas.list[i].Opcoes.val = JSON.stringify(opcoes);
            }
        }
    });

    recarregarGridPergunta();

}

