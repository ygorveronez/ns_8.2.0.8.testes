/// <reference path="ProdutoEmbarcador.js" />


var _gridOrganizacao;
var _organizacao;

var Organizacao = function () {
    this.Grid = PropertyEntity({ type: types.local });

    /* Campos aqui */
    this.Codigo = PropertyEntity({ getType: typesKnockout.int, val: ko.observable(0), def: 0 });
    this.Organizacao = PropertyEntity({ text: "Organização:", val: ko.observable(""), def: "", required: true });
    this.Canal = PropertyEntity({ text: "Canal:", val: ko.observable(""), def: "", required: true });
    this.Setor = PropertyEntity({ text: "Setor:", val: ko.observable(""), def: "", required: true });
    this.Nivel = PropertyEntity({ text: "Nível:", val: ko.observable(""), def: "", required: true });
    this.HierarquiaCodigo = PropertyEntity({ text: "Hierarquia:", val: ko.observable(""), def: "", required: true });
    this.HierarquiaDescricao = PropertyEntity({ text: "Desc. Hierarquia:", val: ko.observable(""), def: "", required: true });

    this.Adicionar = PropertyEntity({ eventClick: adicionarOrganizacaoClick, type: types.event, text: Localization.Resources.Gerais.Geral.Adicionar, visible: ko.observable(true) });
    this.Atualizar = PropertyEntity({ eventClick: atualizarOrganizacaoClick, type: types.event, text: Localization.Resources.Gerais.Geral.Atualizar, visible: ko.observable(false) });
    this.Excluir = PropertyEntity({ eventClick: excluirOrganizacaoClick, type: types.event, text: Localization.Resources.Gerais.Geral.Excluir, visible: ko.observable(false) });
    this.Cancelar = PropertyEntity({ eventClick: cancelarOrganizacaoClick, type: types.event, text: Localization.Resources.Gerais.Geral.Cancelar, visible: ko.observable(false) });
}

var OrganizacaoMap = function () {
    /* Campos aqui */
    this.Codigo = PropertyEntity({ type: types.map, val: 0, def: 0, getType: typesKnockout.int });
    this.Organizacao = PropertyEntity({ type: types.map, val: "" });
    this.Canal = PropertyEntity({ type: types.map, val: "" });
    this.Setor = PropertyEntity({ type: types.map, val: "" });
    this.Nivel = PropertyEntity({ type: types.map, val: "" });
    this.HierarquiaCodigo = PropertyEntity({ type: types.map, val: "" });
    this.HierarquiaDescricao = PropertyEntity({ type: types.map, val: "" });
};

function loadOrganizacao() {
    _organizacao = new Organizacao();
    KoBindings(_organizacao, "knockoutOrganizacao");

    loadGridOrganizacao();
    recarregarGridOrganizacao();
}

function loadGridOrganizacao() {
    var editar = { descricao: Localization.Resources.Gerais.Geral.Editar, id: guid(), evento: "onclick", metodo: editarOrganizacao, tamanho: "10", icone: "" };
    var menuOpcoes = { tipo: TypeOptionMenu.link, opcoes: [editar] };

    var header = [
        { data: "Codigo", visible: false },
        { data: "Organizacao", title: "Organização", width: "15%", className: "text-align-left" },
        { data: "Canal", title: "Canal", width: "15%", className: "text-align-left" },
        { data: "Setor", title: "Setor", width: "15%", className: "text-align-left" },
        { data: "Nivel", title: "Nível", width: "15%", className: "text-align-left" },
        { data: "HierarquiaCodigo", title: "Hierarquia", width: "15%", className: "text-align-left" },
        { data: "HierarquiaDescricao", title: "Desc. Hierarquia", width: "15%", className: "text-align-left" },
    ];

    _gridOrganizacao = new BasicDataTable(_organizacao.Grid.id, header, menuOpcoes, { column: 0, dir: orderDir.asc } );
    _gridOrganizacao.CarregarGrid([]);
}

function recarregarGridOrganizacao() {
    var data = [];

    $.each(_produtoEmbarcador.Organizacao.list, function (i, organizacao) {
        let objeto = new Object();
        objeto.Codigo = organizacao.Codigo.val;
        objeto.Organizacao = organizacao.Organizacao.val;
        objeto.Canal = organizacao.Canal.val;
        objeto.Setor = organizacao.Setor.val;
        objeto.Nivel = organizacao.Nivel.val;
        objeto.HierarquiaCodigo = organizacao.HierarquiaCodigo.val;
        objeto.HierarquiaDescricao = organizacao.HierarquiaDescricao.val;
        data.push(objeto);
    });

    _gridOrganizacao.CarregarGrid(data);
};

function editarOrganizacao(data) {
    limparCamposOrganizacao();
    $.each(_produtoEmbarcador.Organizacao.list, function (i, organizacao) {
        if (organizacao.Codigo.val == data.Codigo) {
            _organizacao.Codigo.val(organizacao.Codigo.val);
            _organizacao.Organizacao.val(organizacao.Organizacao.val);
            _organizacao.Canal.val(organizacao.Canal.val);
            _organizacao.Setor.val(organizacao.Setor.val);
            _organizacao.Nivel.val(organizacao.Nivel.val);
            _organizacao.HierarquiaCodigo.val(organizacao.HierarquiaCodigo.val);
            _organizacao.HierarquiaDescricao.val(organizacao.HierarquiaDescricao.val);
            
            return false;
        }
    });

    _organizacao.Adicionar.visible(false);
    _organizacao.Atualizar.visible(true);
    _organizacao.Excluir.visible(true);
    _organizacao.Cancelar.visible(true);
}

function adicionarOrganizacaoClick() {
    if (ValidarCamposObrigatorios(_organizacao)) {
        if (ValidarOrganizacao()) {
            var objeto = new OrganizacaoMap();

            objeto.Codigo.val = guid();
            objeto.Organizacao.val = _organizacao.Organizacao.val();
            objeto.Canal.val = _organizacao.Canal.val();
            objeto.Setor.val = _organizacao.Setor.val();
            objeto.Nivel.val = _organizacao.Nivel.val();
            objeto.HierarquiaCodigo.val = _organizacao.HierarquiaCodigo.val();
            objeto.HierarquiaDescricao.val = _organizacao.HierarquiaDescricao.val();

            _produtoEmbarcador.Organizacao.list.push(objeto);

            recarregarGridOrganizacao();
            limparCamposOrganizacao();
        }
    } else {
        exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.CamposObrigatorios, Localization.Resources.Gerais.Geral.PreenchaOsCamposObrigatorios);
    }
}

function atualizarOrganizacaoClick() {
    if (ValidarCamposObrigatorios(_organizacao)) {
        if (ValidarOrganizacao()) {
            $.each(_produtoEmbarcador.Organizacao.list, function (i, organizacao) {
                if (organizacao.Codigo.val == _organizacao.Codigo.val()) {
                    organizacao.Organizacao.val = _organizacao.Organizacao.val();
                    organizacao.Canal.val = _organizacao.Canal.val();
                    organizacao.Setor.val = _organizacao.Setor.val();
                    organizacao.Nivel.val = _organizacao.Nivel.val();
                    organizacao.HierarquiaCodigo.val = _organizacao.HierarquiaCodigo.val();
                    organizacao.HierarquiaDescricao.val = _organizacao.HierarquiaDescricao.val();

                    return false;
                }
            });
            recarregarGridOrganizacao();
            limparCamposOrganizacao();
        }
    } else {
        exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.CamposObrigatorios, Localization.Resources.Gerais.Geral.PreenchaOsCamposObrigatorios);
    }
}

function excluirOrganizacaoClick() {
    $.each(_produtoEmbarcador.Organizacao.list, function (i, organizacao) {
        if (organizacao.Codigo.val == _organizacao.Codigo.val()) {
            _produtoEmbarcador.Organizacao.list.splice(i, 1);
            return false;
        }
    });

    recarregarGridOrganizacao();
    limparCamposOrganizacao();
}

function cancelarOrganizacaoClick() {
    limparCamposOrganizacao();
}

function limparCamposOrganizacao() {
    _organizacao.Adicionar.visible(true);
    _organizacao.Atualizar.visible(false);
    _organizacao.Excluir.visible(false);
    _organizacao.Cancelar.visible(false);

    LimparCampos(_organizacao);
}

function ValidarOrganizacao() {
    for (var i = 0; i < _produtoEmbarcador.Organizacao.list.length; i++) {
        let organizacao = _produtoEmbarcador.Organizacao.list[i];

        /* VALIDAÇÕES AQUI*/
        if (organizacao.Codigo.val != _organizacao.Codigo.val() && organizacao.Organizacao.val == _organizacao.Organizacao.val() && organizacao.Canal.val == _organizacao.Canal.val() &&
            organizacao.Setor.val == _organizacao.Setor.val() && organizacao.Nivel.val == _organizacao.Nivel.val() &&
            organizacao.HierarquiaCodigo.val == _organizacao.HierarquiaCodigo.val() && organizacao.HierarquiaDescricao.val == _organizacao.HierarquiaDescricao.val())
        {
            exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.Aviso, "Não é possível cadastrar uma organização com uma composição já existente.");
            return false;
        }

    }

    return true;
}