var _dataFixaVencimento, _gridDataFixaVencimento;

var DataFixaVencimentoMap = function () {
    this.Codigo = PropertyEntity({ type: types.map, val: 0, def: 0, getType: typesKnockout.int });
    this.DiaInicialEmissao = PropertyEntity({ type: types.map, val: "" });
    this.DiaFinalEmissao = PropertyEntity({ type: types.map, val: "" });
    this.DiaVencimento = PropertyEntity({ type: types.map, val: "" });
};


var DataFixaVencimento = function () {
    this.DataFixaVencimento = PropertyEntity({ type: types.listEntity, idGrid: guid(), visible: ko.observable(true), enable: ko.observable(true) });

    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.DiaInicialEmissao = PropertyEntity({ text: "Dia inicial emissão: ", getType: typesKnockout.int, required: ko.observable(false), val: ko.observable(""), def: "", maxlength: 2, visible: ko.observable(true), configInt: { thousands: "" } });
    this.DiaFinalEmissao = PropertyEntity({ text: "Dia final emissão: ", getType: typesKnockout.int, required: ko.observable(false), val: ko.observable(""), def: "", maxlength: 2, visible: ko.observable(true), configInt: { thousands: "" } });
    this.DiaVencimento = PropertyEntity({ text: "Dia vencimento: ", getType: typesKnockout.int, required: ko.observable(false), val: ko.observable(""), def: "", maxlength: 2, visible: ko.observable(true), configInt: { thousands: "" } });
    this.AdicionarDataFixaVencimento = PropertyEntity({ eventClick: adicionarDataFixaVencimentoClick, text: "Adicionar", type: types.event, visible: ko.observable(true), enable: ko.observable(true) });
    this.AtualizarDataFixaVencimento = PropertyEntity({ eventClick: atualizarDataFixaVencimentoClick, type: types.event, text: "Atualizar", visible: ko.observable(false), enable: ko.observable(true) });
    this.ExcluirDataFixaVencimento = PropertyEntity({ eventClick: excluirDataFixaVencimentoClick, type: types.event, text: "Excluir", visible: ko.observable(false), enable: ko.observable(true) });
    this.CancelarDataFixaVencimento = PropertyEntity({ eventClick: limparDataFixaVencimentoClick, type: types.event, text: "Cancelar", visible: ko.observable(false), enable: ko.observable(true) });
};

function adicionarDataFixaVencimentoClick() {    
    if (isValidar()) {
        var dataFixaVencimento = new DataFixaVencimentoMap();
        dataFixaVencimento.Codigo.val = guid();
        dataFixaVencimento.DiaInicialEmissao.val = _dataFixaVencimento.DiaInicialEmissao.val();
        dataFixaVencimento.DiaFinalEmissao.val = _dataFixaVencimento.DiaFinalEmissao.val();
        dataFixaVencimento.DiaVencimento.val = _dataFixaVencimento.DiaVencimento.val();
        _pessoa.DataFixaVencimento.list.push(dataFixaVencimento);
        RecarregarGridDataFixaVencimento();
        limparDataFixaVencimento();
    }
}

function atualizarDataFixaVencimentoClick() {
    if (isValidar()) {
        const dataFixaVencimento = _pessoa.DataFixaVencimento.list.find(el => el.Codigo.val == _dataFixaVencimento.Codigo.val());
        dataFixaVencimento.DiaInicialEmissao.val = _dataFixaVencimento.DiaInicialEmissao.val();
        dataFixaVencimento.DiaFinalEmissao.val = _dataFixaVencimento.DiaFinalEmissao.val();
        dataFixaVencimento.DiaVencimento.val = _dataFixaVencimento.DiaVencimento.val();        
        RecarregarGridDataFixaVencimento();
        limparDataFixaVencimento();
    }
}

function excluirDataFixaVencimentoClick() {
    excluirDataFixaVencimento(_dataFixaVencimento.Codigo.val(), true);
}

function limparDataFixaVencimentoClick() {
    limparDataFixaVencimento();
}

function editarDataFixaVencimentoGridClick(data) {
    _dataFixaVencimento.Codigo.val(data.Codigo);
    _dataFixaVencimento.DiaInicialEmissao.val(data.DiaInicialEmissao);
    _dataFixaVencimento.DiaFinalEmissao.val(data.DiaFinalEmissao);
    _dataFixaVencimento.DiaVencimento.val(data.DiaVencimento);  
    _dataFixaVencimento.AdicionarDataFixaVencimento.visible(false);
    _dataFixaVencimento.AtualizarDataFixaVencimento.visible(true);
    _dataFixaVencimento.ExcluirDataFixaVencimento.visible(true);
    _dataFixaVencimento.CancelarDataFixaVencimento.visible(true);
}



function excluirDataFixaVencimentoGridClick(data) {
    excluirDataFixaVencimento(data.Codigo, false);
}


function excluirDataFixaVencimento(codigo, limpar) {
    exibirConfirmacao(Localization.Resources.Gerais.Geral.Confirmacao, "Realmente deseja excluir?",
        function () {        

            for (var i = 0; i < _pessoa.DataFixaVencimento.list.length; i++) {
                if (codigo == _pessoa.DataFixaVencimento.list[i].Codigo.val) {
                    _pessoa.DataFixaVencimento.list.splice(i, 1);
                    break;
                }
            }
            
            RecarregarGridDataFixaVencimento();

            if (limpar) {
                limparDataFixaVencimento();
            }
        }
    );
}


function limparDataFixaVencimento() {   
    LimparCampo(_dataFixaVencimento.Codigo);
    LimparCampo(_dataFixaVencimento.DiaInicialEmissao);
    LimparCampo(_dataFixaVencimento.DiaFinalEmissao);
    LimparCampo(_dataFixaVencimento.DiaVencimento);
    _dataFixaVencimento.AdicionarDataFixaVencimento.visible(true);
    _dataFixaVencimento.AtualizarDataFixaVencimento.visible(false);
    _dataFixaVencimento.ExcluirDataFixaVencimento.visible(false);
    _dataFixaVencimento.CancelarDataFixaVencimento.visible(false);
}

function isValidar() {
    _dataFixaVencimento.DiaInicialEmissao.required(true);
    _dataFixaVencimento.DiaFinalEmissao.required(true);
    _dataFixaVencimento.DiaVencimento.required(true);

    var validaDiaInicialEmissao = ValidarCampoObrigatorioMap(_dataFixaVencimento.DiaInicialEmissao);
    var validaDiaFinalEmissao = ValidarCampoObrigatorioMap(_dataFixaVencimento.DiaFinalEmissao);
    var validaDiaVencimento = ValidarCampoObrigatorioMap(_dataFixaVencimento.DiaVencimento);
    var isValided = validaDiaInicialEmissao && validaDiaFinalEmissao && validaDiaVencimento;

    _dataFixaVencimento.DiaInicialEmissao.required(false);
    _dataFixaVencimento.DiaFinalEmissao.required(false);
    _dataFixaVencimento.DiaVencimento.required(false);

    if (!isValided) {
        exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.CamposObrigatorios, Localization.Resources.Gerais.Geral.InformeCamposObrigatorios);
        return false;
    }  

    if (!(0 < parseInt(_dataFixaVencimento.DiaInicialEmissao.val()) && parseInt(_dataFixaVencimento.DiaInicialEmissao.val()) <= 31) ||
        !(0 < parseInt(_dataFixaVencimento.DiaFinalEmissao.val()) && parseInt(_dataFixaVencimento.DiaFinalEmissao.val()) <= 31) ||
        !(0 < parseInt(_dataFixaVencimento.DiaVencimento.val()) && parseInt(_dataFixaVencimento.DiaVencimento.val()) <= 31)) {
        exibirMensagem(tipoMensagem.atencao, "Dias Fora do período", "Insira um dia valido");
        return false;
    } 

    return true;
}

function isIntervalContained(newStart, newEnd, existingStart, existingEnd) {
    return (
        (newStart < existingStart && newEnd > existingEnd) || // Cenário 1
        (newStart >= existingStart && newStart <= existingEnd && newEnd > existingEnd) || // Cenário 2
        (newStart < existingStart && newEnd >= existingStart && newEnd <= existingEnd) || // Cenário 3
        (newStart >= existingStart && newEnd <= existingEnd) // Cenário 4
    );
}


//*******EVENTOS*******
function LoadDataFixaVencimento() {
    _dataFixaVencimento = new DataFixaVencimento();
    KoBindings(_dataFixaVencimento, "knockoutDataFixaVencimento");
    CarregarGridDataFixaVencimento();
}

function RecarregarGridDataFixaVencimento() {
    var data = new Array();
    $.each(_pessoa.DataFixaVencimento.list, function (i, dataFixaVencimento) { 
        data.push({
            Codigo: dataFixaVencimento.Codigo.val,
            DiaInicialEmissao: dataFixaVencimento.DiaInicialEmissao.val,
            DiaFinalEmissao: dataFixaVencimento.DiaFinalEmissao.val,
            DiaVencimento: dataFixaVencimento.DiaVencimento.val
        });
    });
    _gridDataFixaVencimento.CarregarGrid(data);
}

function CarregarGridDataFixaVencimento() {
    let excluir = { descricao: "Excluir", id: guid(), evento: "onclick", metodo: excluirDataFixaVencimentoGridClick, visibilidade: true};
    let editar = { descricao: "Editar", id: guid(), evento: "onclick", metodo: editarDataFixaVencimentoGridClick, visibilidade: true };
    menuOpcoes = new Object();

    menuOpcoes.tipo = TypeOptionMenu.list;
    menuOpcoes.opcoes = new Array();
    menuOpcoes.opcoes.push(editar, excluir);

    var header = [
        { data: "Codigo", visible: false },
        { data: "DiaInicialEmissao", title: "Dia inicial emissão", width: "33%" },
        { data: "DiaFinalEmissao", title: "Dia final emissão", width: "33%" },
        { data: "DiaVencimento", title: "Dia vencimento", width: "33%" },
    ];
    _gridDataFixaVencimento = new BasicDataTable(_dataFixaVencimento.DataFixaVencimento.idGrid, header, menuOpcoes, { column: 1, dir: orderDir.asc });
    RecarregarGridDataFixaVencimento();
}