var _permissoesWebService;
var _gridPermissoesWebService;

//*******MAPEAMENTO KNOUCKOUT*******

var PermissoesWebServiceMap = function () {
    this.Codigo = PropertyEntity({ type: types.map, val: 0, def: 0, getType: typesKnockout.int });
    this.NomeMetodo = PropertyEntity({ type: types.map, val: "" });
    this.RequisicoesMinuto = PropertyEntity({ type: types.map, val: "" });
    this.QtdRequisicoes = PropertyEntity({ type: types.map, val: "" });
    this.UltimoReset = PropertyEntity({ type: types.map, val: "" });
};


var PermissoesWebService = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.NomeMetodo = PropertyEntity({ text: Localization.Resources.Integracoes.Integradora.NomeMetodo.getRequiredFieldDescription(), required: true,enable: ko.observable(true), visible: ko.observable(true) });
    this.RequisicoesMinuto = PropertyEntity({ text: Localization.Resources.Integracoes.Integradora.RequisicaoPorMinuto.getRequiredFieldDescription(), required: true, enable: ko.observable(true), visible: ko.observable(true) });
    this.QtdRequisicoes = PropertyEntity({ text: Localization.Resources.Integracoes.Integradora.QuantRequisicoes.getRequiredFieldDescription(), required: true, enable: ko.observable(true), visible: ko.observable(true) });
    this.UltimoReset = PropertyEntity({ text: Localization.Resources.Integracoes.Integradora.UltimoReset.getRequiredFieldDescription(), getType: typesKnockout.date, required: true, enable: ko.observable(true), visible: ko.observable(true) });
    this.GridPermissoesWebService = PropertyEntity({ type: types.listEntity, list: new Array(), required: false, idGrid: guid(), codEntity: ko.observable(0), visible: ko.observable(true) });
    

    this.AdicionarPermissoesWebService = PropertyEntity({ eventClick: adicionarPermissoesWebServiceClick, type: types.event, text: Localization.Resources.Gerais.Geral.Adicionar, visible: ko.observable(true), enable: !_FormularioSomenteLeitura });
    this.AtualizarPermissoesWebService = PropertyEntity({ eventClick: atualizarPermissoesWebServiceClick, type: types.event, text: Localization.Resources.Gerais.Geral.Atualizar, visible: ko.observable(false), enable: !_FormularioSomenteLeitura });
    this.ExcluirPermissoesWebService = PropertyEntity({ eventClick: excluirPermissoesWebServiceClick, type: types.event, text: Localization.Resources.Gerais.Geral.Excluir, visible: ko.observable(false), enable: !_FormularioSomenteLeitura });
    this.CancelarPermissoesWebService = PropertyEntity({ eventClick: LimparCamposPermissoesWebService, type: types.event, text: Localization.Resources.Gerais.Geral.Cancelar, visible: ko.observable(false), enable: !_FormularioSomenteLeitura });
};

//*******EVENTOS*******

function loadPermissoesWebService() {
    _permissoesWebService = new PermissoesWebService();
    KoBindings(_permissoesWebService, "knockoutPermissoesWebService");

    new BuscarPermissoesWebService(_permissoesWebService.Codigo);
    loadGridPermissoesWebService();
}

function loadGridPermissoesWebService() {
    var editar = { descricao: Localization.Resources.Gerais.Geral.Editar, id: guid(), evento: "onclick", metodo: editarPermissoesWebService, tamanho: "15", icone: "" };
    var menuOpcoes = new Object();
    menuOpcoes.tipo = TypeOptionMenu.link;
    menuOpcoes.opcoes = new Array();
    menuOpcoes.opcoes.push(editar);
    /*    var header = [
            { data: "Codigo", visible: false },
            { data: "CodigoFuncionario", visible: false },
            { data: "CodigoTipoDeCarga", visible: false },
            { data: "Funcionario", title: Localization.Resources.Pessoas.Pessoa.Funcionario, width: "40%", className: "text-align-left" },
            { data: "TipoDeCarga", title: Localization.Resources.Pessoas.Pessoa.TipoDeCarga, width: "40%", className: "text-align-left", visible: _CONFIGURACAO_TMS.TipoServicoMultisoftware === EnumTipoServicoMultisoftware.MultiEmbarcador },
            { data: "PercentualComissao", title: Localization.Resources.Pessoas.Pessoa.PercentualComissao, width: "15%", className: "text-align-right" },
            { data: "DataInicioVigencia", title: Localization.Resources.Pessoas.Pessoa.DataInicioVigencia, width: "15%", className: "text-align-center" },
            { data: "DataFimVigencia", title: Localization.Resources.Pessoas.Pessoa.DataFimVigencia, width: "15%", className: "text-align-center" }
        ]; exemplo de percentual 
        */
    var header = [
        { data: "Codigo", visible: false },
        { data: "NomeMetodo", title: Localization.Resources.Integracoes.Integradora.NomeMetodo },
        { data: "RequisicoesMinuto", title: Localization.Resources.Integracoes.Integradora.RequisicaoPorMinuto },
        { data: "QtdRequisicoes", title: Localization.Resources.Integracoes.Integradora.QuantRequisicoes},
        { data: "UltimoReset", title: Localization.Resources.Integracoes.Integradora.UltimoReset }
    ];

    _gridPermissoesWebService = new BasicDataTable(_permissoesWebService.GridPermissoesWebService.idGrid, header, menuOpcoes);

    recarregarGridPermissoesWebService();
}

function adicionarPermissoesWebServiceClick() {
    var valido = ValidarCamposObrigatorios(_permissoesWebService);
    if (valido) {
        var existe = false;
        $.each(_integradora.ListaPermissoesWebService.list, function (i, permissao) {
            if (permissao.NomeMetodo.val == _permissoesWebService.NomeMetodo.val()) {
                existe = true;
                return;
            }
        });

        if (existe) {
            exibirMensagem(tipoMensagem.aviso, Localization.Resources.Pessoas.Pessoa.FuncionarioJaExistente, Localization.Resources.Pessoas.Pessoa.FuncionarioJaEstaCadastrado.format(_permissoesWebService.Funcionario.val()));
            return;
        }

        var permissao = new PermissoesWebServiceMap();
        permissao.Codigo.val = guid();
        permissao.NomeMetodo.val = _permissoesWebService.NomeMetodo.val();
        permissao.RequisicoesMinuto.val = _permissoesWebService.RequisicoesMinuto.val();
        permissao.QtdRequisicoes.val = _permissoesWebService.QtdRequisicoes.val();
        permissao.UltimoReset.val = _permissoesWebService.UltimoReset.val();

        _integradora.ListaPermissoesWebService.list.push(permissao);
        recarregarGridPermissoesWebService();
        //$("#" + _permissoesWebService.Funcionario.id).focus();
        LimparCamposPermissoesWebService();
    } else {
        exibirMensagem(Localization.Resources.Gerais.Geral.Atencao, Localization.Resources.Gerais.Geral.CamposObrigatorios, Localization.Resources.Gerais.Geral.InformeCamposObrigatorios);
    }
}

function atualizarPermissoesWebServiceClick() {
    var valido = ValidarCamposObrigatorios(_permissoesWebService);

    if (valido) {
        $.each(_integradora.ListaPermissoesWebService.list, function (i, permissao) {
            if (permissao.Codigo.val == _permissoesWebService.Codigo.val()) {

                permissao.Codigo.val = _permissoesWebService.Codigo.val();
                permissao.NomeMetodo.val = _permissoesWebService.NomeMetodo.val();
                permissao.QtdRequisicoes.val = _permissoesWebService.QtdRequisicoes.val();
                permissao.UltimoReset.val = _permissoesWebService.UltimoReset.val();
                permissao.RequisicoesMinuto.val = _permissoesWebService.RequisicoesMinuto.val();
                return false;
            }
        });
        recarregarGridPermissoesWebService();
        LimparCamposPermissoesWebService();
    } else {
        exibirMensagem(Localization.Resources.Gerais.Geral.Atencao, Localization.Resources.Gerais.Geral.CamposObrigatorios, Localization.Resources.Gerais.Geral.InformeCamposObrigatorios);
    }
}

function excluirPermissoesWebServiceClick() {
    exibirConfirmacao(Localization.Resources.Gerais.Geral.Confirmacao, Localization.Resources.Integracoes.Integradora.NomeMetodo.DesejaExcluirPermissao, function () {
        var listaAtualizada = new Array();
        $.each(_integradora.ListaPermissoesWebService.list, function (i, permissao) {
            if (permissao.Codigo.val != _permissoesWebService.Codigo.val()) {
                listaAtualizada.push(permissao);
            }
        });
        _integradora.ListaPermissoesWebService.list = listaAtualizada;
        recarregarGridPermissoesWebService();
        LimparCamposPermissoesWebService();
    });
}

//*******MÉTODOS*******


function recarregarGridPermissoesWebService() {
    var data = new Array();
    $.each(_integradora.ListaPermissoesWebService.list, function (i, permissao) {
        var _permissao = new Object();
        _permissao.Codigo = permissao.Codigo.val;
        _permissao.NomeMetodo = permissao.NomeMetodo.val;
        _permissao.RequisicoesMinuto = permissao.RequisicoesMinuto.val;
        _permissao.QtdRequisicoes = permissao.QtdRequisicoes.val;
        _permissao.UltimoReset = permissao.UltimoReset.val;
        data.push(_permissao);
    });
    _gridPermissoesWebService.CarregarGrid(data);
}

function editarPermissoesWebService(data) {
    LimparCamposPermissoesWebService();
    $.each(_integradora.ListaPermissoesWebService.list, function (i, permissao) {
        if (permissao.Codigo.val == data.Codigo) {
            _permissoesWebService.Codigo.val(permissao.Codigo.val);
            _permissoesWebService.NomeMetodo.val(permissao.NomeMetodo.val);
            _permissoesWebService.RequisicoesMinuto.val(permissao.RequisicoesMinuto.val);
            _permissoesWebService.QtdRequisicoes.val(permissao.QtdRequisicoes.val);
            _permissoesWebService.UltimoReset.val(permissao.UltimoReset.val);
            return false;
        }
    });

    _permissoesWebService.AdicionarPermissoesWebService.visible(false);
    _permissoesWebService.AtualizarPermissoesWebService.visible(true);
    _permissoesWebService.ExcluirPermissoesWebService.visible(true);
    _permissoesWebService.CancelarPermissoesWebService.visible(true);
}

function LimparCamposPermissoesWebService() {

    _permissoesWebService.Codigo.val("");
    _permissoesWebService.NomeMetodo.val("");
    _permissoesWebService.RequisicoesMinuto.val("");
    _permissoesWebService.QtdRequisicoes.val("");
    _permissoesWebService.UltimoReset.val("");

    _permissoesWebService.AdicionarPermissoesWebService.visible(true);
    _permissoesWebService.AtualizarPermissoesWebService.visible(false);
    _permissoesWebService.ExcluirPermissoesWebService.visible(false);
    _permissoesWebService.CancelarPermissoesWebService.visible(false);
}

