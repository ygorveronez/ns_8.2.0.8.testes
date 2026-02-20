/// <reference path="../../../js/libs/jquery-2.1.1.js" />
/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/bootstrap/bootstrap.js" />
/// <reference path="../../../js/libs/jquery.blockui.js" />
/// <reference path="../../../js/Global/knoutViewsSlides.js" />
/// <reference path="../../../js/libs/jquery.maskMoney.js" />
/// <reference path="../../../js/plugin/datatables/jquery.dataTables.js" />
/// <reference path="Equipamento.js" />

var _equipamentoNovoHorimetro,
    _gridEquipamentoHorimetro;

var EquipamentoNovoHorimetro = function () {
    this.Codigo = PropertyEntity({ type: types.map, val: 0, def: 0, getType: typesKnockout.int });    
    this.DataAlteracao = PropertyEntity({ text: "Data Alteração", required: true, getType: typesKnockout.date });
    this.HorimetroAtual = PropertyEntity({ type: types.map, val: "" });
    this.Observacao = PropertyEntity({ type: types.map, val: "" });
}


function loadEquipamentoNovoHorimetro() {

    var editar = { descricao: Localization.Resources.Gerais.Geral.Editar, id: guid(), evento: "onclick", metodo: EditarNovoHorimetro, tamanho: "15", icone: "" };
    var menuOpcoes = new Object();
    menuOpcoes.tipo = TypeOptionMenu.link;
    menuOpcoes.opcoes = new Array();
    menuOpcoes.opcoes.push(editar);    

    var header = [
        { data: "Codigo", visible: false },
        { data: "DataAlteracao", title: Localization.Resources.Gerais.Geral.DataAlteracao, width: "25%", className: "text-align-center" },        
        { data: "HorimetroAtual", title: Localization.Resources.Consultas.Equipamento.NovoHorimetro, width: "15%" },
        { data: "Observacao", title: Localization.Resources.Gerais.Geral.Observacao, width: "30%" }
    ];  
    _gridEquipamentoHorimetro = new BasicDataTable(_equipamento.HistoricoHorimetros.idGrid, header, menuOpcoes);    
    recarregarGridEquipamentoNovosHorimetros();
}

function AdicionarNovoHorimetroClick() {
    if (_equipamento.NovoHorimetro.val() == "") {
        exibirMensagem("atencao", Localization.Resources.Gerais.Geral.CamposObrigatorios, Localization.Resources.Gerais.Geral.InformeCamposObrigatorios);
        return;
    }
    if (_equipamento.DataAlteracao.val() == "") {
        exibirMensagem("atencao", Localization.Resources.Gerais.Geral.CamposObrigatorios, Localization.Resources.Gerais.Geral.InformeCamposObrigatorios);
        return;
    }
    if (_equipamento.ObservacaoNovoHorimetro.val() == "") {
        exibirMensagem("atencao", Localization.Resources.Gerais.Geral.CamposObrigatorios, Localization.Resources.Gerais.Geral.InformeCamposObrigatorios);
        return;
    }

    let horimetro = new EquipamentoNovoHorimetro();
    horimetro.Codigo.val = guid();
    horimetro.DataAlteracao.val = _equipamento.DataAlteracao.val();
    horimetro.HorimetroAtual.val = _equipamento.NovoHorimetro.val();
    horimetro.Observacao.val = _equipamento.ObservacaoNovoHorimetro.val();

    _equipamento.HistoricoHorimetros.list.push(horimetro);
    recarregarGridEquipamentoNovosHorimetros();
    LimparCamposNovoHorimetro();
    
}


function AtualizarNovoHorimetroClick() {
    if (_equipamento.NovoHorimetro.val() == "") {
        exibirMensagem("atencao", Localization.Resources.Gerais.Geral.CamposObrigatorios, Localization.Resources.Gerais.Geral.InformeCamposObrigatorios);
        return;
    }
    if (_equipamento.DataAlteracao.val() == "") {
        exibirMensagem("atencao", Localization.Resources.Gerais.Geral.CamposObrigatorios, Localization.Resources.Gerais.Geral.InformeCamposObrigatorios);
        return;
    }
    if (_equipamento.ObservacaoNovoHorimetro.val() == "") {
        exibirMensagem("atencao", Localization.Resources.Gerais.Geral.CamposObrigatorios, Localization.Resources.Gerais.Geral.InformeCamposObrigatorios);
        return;
    }

    $.each(_equipamento.HistoricoHorimetros.list, function (i, horimetro) {
        console.log('horimetro', horimetro);
        if (horimetro.Codigo.val == _equipamento.Codigo.val()) {
            horimetro.DataAlteracao.val = _equipamento.DataAlteracao.val();
            horimetro.HorimetroAtual.val = _equipamento.NovoHorimetro.val();
            horimetro.Observacao.val = _equipamento.ObservacaoNovoHorimetro.val();

            return false;
        }
    });
    recarregarGridEquipamentoNovosHorimetros();
    LimparCamposNovoHorimetro();
}

function EditarNovoHorimetro(data) {
    LimparCamposNovoHorimetro();
    $.each(_equipamento.HistoricoHorimetros.list, function (i, item) {
        if (item.Codigo.val == data.Codigo) {
            _equipamento.DataAlteracao.val(item.DataAlteracao.val);
            _equipamento.NovoHorimetro.val(item.HorimetroAtual.val);
            _equipamento.ObservacaoNovoHorimetro.val(item.Observacao.val);

            return false;
        }
    });

    _equipamento.AdicionarNovoHorimetro.visible(false);
    _equipamento.AtualizarNovoHorimetro.visible(true);
    _equipamento.CancelarNovoHorimetro.visible(true);
}

function recarregarGridEquipamentoNovosHorimetros() {
    var data = new Array();
    $.each(_equipamento.HistoricoHorimetros.list, function (i, Horimetro) {
        var obj = new Object();
        obj.Codigo = Horimetro.Codigo.val;
        obj.DataAlteracao = Horimetro.DataAlteracao.val;
        obj.HorimetroAtual = Horimetro.HorimetroAtual.val;
        obj.Observacao = Horimetro.Observacao.val;
        data.push(obj);
    });
    _gridEquipamentoHorimetro.CarregarGrid(data);
}

function carregarGridHorimetros(lista) {

    if (lista === "undefined" || lista === null || lista.length === 0) {
        return;
    }

    for (let i = 0; i <= lista.length - 1; i++) {

        let map = new EquipamentoNovoHorimetro();

        map.Codigo = lista[i].Codigo;
        map.DataAlteracao = lista[i].DataAlteracao;
        map.HorimetroAtual = lista[i].HorimetroAtual;
        map.Observacao = lista[i].Observacao;
    }

    recarregarGridEquipamentoNovosHorimetros();
}

function LimparCamposNovoHorimetro() {    
    //LimparCampoEntity(_equipamento.Equipamento);

    _equipamento.DataAlteracao.val("");
    _equipamento.NovoHorimetro.val("");
    _equipamento.ObservacaoNovoHorimetro.val("");

    _equipamento.AdicionarNovoHorimetro.visible(true);
    _equipamento.AtualizarNovoHorimetro.visible(false);
    _equipamento.CancelarNovoHorimetro.visible(false);

    
}