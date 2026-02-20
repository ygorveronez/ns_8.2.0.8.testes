/// <reference path="../../Consultas/Filial.js" />
/// <reference path="../../Enumeradores/EnumSituacaoFilial.js" />
/// <reference path="ProdutoEmbarcador.js" />

var _gridFiliais;
var _filial;

var Filial = function () {
    this.Grid = PropertyEntity({ type: types.local });
    
    this.Codigo = PropertyEntity({ getType: typesKnockout.int, val: ko.observable(0), def: 0 });
    this.Filial = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "*Filial:", idBtnSearch: guid(), required: true });
    this.Situacao = PropertyEntity({ text: "Status: ", options: _status, val: ko.observable(true), def: true, visible: false });
    this.SituacaoCodigo = PropertyEntity({ text: "Situação", options: EnumSituacaoFilial.obterOpcoes(), val: ko.observable(EnumSituacaoFilial.SemSituacao), def: EnumSituacaoFilial.SemSituacao});
    this.NCM = PropertyEntity({ text: "NCM", val: ko.observable("") });
    this.UsoMaterial = PropertyEntity({ text: "Uso Material", val: ko.observable(EnumUsoMaterial.Revenda), options: EnumUsoMaterial.obterOpcoes(), def: EnumUsoMaterial.Revenda });

    this.Adicionar = PropertyEntity({ eventClick: adicionarFilialClick, type: types.event, text: Localization.Resources.Gerais.Geral.Adicionar, visible: ko.observable(true) });
    this.Atualizar = PropertyEntity({ eventClick: atualizarFilialClick, type: types.event, text: Localization.Resources.Gerais.Geral.Atualizar, visible: ko.observable(false) });
    this.Excluir = PropertyEntity({ eventClick: excluirFilialClick, type: types.event, text: Localization.Resources.Gerais.Geral.Excluir, visible: ko.observable(false) });
    this.Cancelar = PropertyEntity({ eventClick: cancelarFilialClick, type: types.event, text: Localization.Resources.Gerais.Geral.Cancelar, visible: ko.observable(false) });
}

var FilialMap = function () {
    this.Codigo = PropertyEntity({ type: types.map, val: 0, def: 0, getType: typesKnockout.int });
    this.CodigoFilial = PropertyEntity({ type: types.map, val: "" });
    this.Filial = PropertyEntity({ type: types.map, val: "" });
    this.Situacao = PropertyEntity({ type: types.map, val: "" });
    this.SituacaoCodigo = PropertyEntity({ type: types.map, val: "" });
    this.SituacaoDescricaoCodigo = PropertyEntity({ type: types.map, val: "" });
    this.NCM = PropertyEntity({ type: types.map, val: "" });
    this.UsoMaterialDescricao = PropertyEntity({ type: types.map, val: "" });
    this.UsoMaterial = PropertyEntity({ type: types.map, val: "" });
};

function loadFiliais() {
    _filial = new Filial();
    KoBindings(_filial, "knockoutFilial");

    new BuscarFilial(_filial.Filial);
   
    loadGridFiliais();
    recarregarGridFiliais();
}

function loadGridFiliais() {
    var editar = { descricao: Localization.Resources.Gerais.Geral.Editar, id: guid(), evento: "onclick", metodo: editarFilial, tamanho: "20", icone: "" };
    var menuOpcoes = { tipo: TypeOptionMenu.link, opcoes: [editar] };

    var header = [
        { data: "Codigo", visible: false },
        { data: "CodigoFilial", visible: false },
        { data: "SituacaoCodigo", visible: false },
        { data: "DescricaoSituacao", visible: false },
        { data: "UsoMaterial", visible: false },
        { data: "Filial", title: "Filial", width: "50%", className: "text-align-left" },
        { data: "NCM", title: "NCM", width: "30%", className: "text-align-center" },
        { data: "SituacaoFilial", title: "Situação", width: "30%", className: "text-align-center" },
        { data: "UsoMaterialDescricao", title: "Uso Material", width: "30%", className: "text-align-center" },
    ];

    _gridFiliais = new BasicDataTable(_filial.Grid.id, header, menuOpcoes, { column: 0, dir: orderDir.asc });
    _gridFiliais.CarregarGrid([]);
}

function recarregarGridFiliais() {
    var data = [];

    $.each(_produtoEmbarcador.Filiais.list, function (i, filial) {
      
        let objeto = new Object();
        objeto.Codigo = filial.Codigo.val;
        objeto.CodigoFilial = filial.CodigoFilial.val;
        objeto.Filial = filial.Filial.val;
        objeto.Situacao = filial.Situacao.val;
        objeto.DescricaoSituacao = filial.Situacao.val ? "Ativo" : "Inativo";
        objeto.SituacaoCodigo = filial.SituacaoCodigo.val;
        objeto.SituacaoFilial = filial.SituacaoDescricaoCodigo.val;
        objeto.NCM = filial.NCM.val;
        objeto.UsoMaterial = filial.UsoMaterial.val;
        objeto.UsoMaterialDescricao = filial.UsoMaterialDescricao.val;

        data.push(objeto);
    });

    _gridFiliais.CarregarGrid(data);
};

function editarFilial(data) {
    limparCamposFilial();
    $.each(_produtoEmbarcador.Filiais.list, function (i, filial) {
        if (filial.Codigo.val == data.Codigo) {

            _filial.Codigo.val(filial.Codigo.val);
            _filial.Filial.codEntity(filial.CodigoFilial.val);
            _filial.Filial.val(filial.Filial.val);
            _filial.Situacao.val(filial.Situacao.val);
            _filial.SituacaoCodigo.val(filial.SituacaoCodigo.val);
            _filial.NCM.val(filial.NCM.val);
            _filial.UsoMaterial.val(filial.UsoMaterial.val);

            return false;
        }
    });

    _filial.Adicionar.visible(false);
    _filial.Atualizar.visible(true);
    _filial.Excluir.visible(true);
    _filial.Cancelar.visible(true);
}

function adicionarFilialClick() {
    if (ValidarCamposObrigatorios(_filial)) {
        if (ValidarFilial()) {
            var objeto = new FilialMap();
            objeto.Codigo.val = guid();
            objeto.CodigoFilial.val = _filial.Filial.codEntity();
            objeto.Filial.val = _filial.Filial.val();
            objeto.Situacao.val = _filial.Situacao.val();
            objeto.SituacaoCodigo.val = _filial.SituacaoCodigo.val()
            objeto.SituacaoDescricaoCodigo.val = EnumSituacaoFilial.obterDescricao(_filial.SituacaoCodigo.val());
            objeto.NCM.val = _filial.NCM.val();
            objeto.UsoMaterial.val = _filial.UsoMaterial.val();
            objeto.UsoMaterialDescricao.val = EnumUsoMaterial.obterDescricao(_filial.UsoMaterial.val());

            _produtoEmbarcador.Filiais.list.push(objeto);

            recarregarGridFiliais();
            limparCamposFilial();
        }
    } else {
        exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.CamposObrigatorios, Localization.Resources.Gerais.Geral.PreenchaOsCamposObrigatorios);
    }
}

function atualizarFilialClick() {
    if (ValidarCamposObrigatorios(_filial)) {
        if (ValidarFilial()) {
            $.each(_produtoEmbarcador.Filiais.list, function (i, filial) {
                if (filial.Codigo.val == _filial.Codigo.val()) {
                    filial.CodigoFilial.val = _filial.Filial.codEntity();
                    filial.Filial.val = _filial.Filial.val();
                    filial.Situacao.val = _filial.Situacao.val();
                    filial.SituacaoCodigo.val = _filial.SituacaoCodigo.val();
                    filial.SituacaoDescricaoCodigo.val = _filial.SituacaoDescricaoCodigo.val();
                    filial.NCM.val = _filial.NCM.val();
                    filial.UsoMaterial.val = _filial.UsoMaterial.val();
                    filial.UsoMaterialDescricao.val = _filial.UsoMaterialDescricao.val();
                    return false;
                }
            });
            recarregarGridFiliais();
            limparCamposFilial();
        }
    } else {
        exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.CamposObrigatorios, Localization.Resources.Gerais.Geral.PreenchaOsCamposObrigatorios);
    }
}

function excluirFilialClick() {
    $.each(_produtoEmbarcador.Filiais.list, function (i, filial) {
        if (filial.Codigo.val == _filial.Codigo.val()) {
            _produtoEmbarcador.Filiais.list.splice(i, 1);
            return false;
        }
    });

    recarregarGridFiliais();
    limparCamposFilial();
}

function cancelarFilialClick() {
    limparCamposFilial();
}

function limparCamposFilial() {
    _filial.Adicionar.visible(true);
    _filial.Atualizar.visible(false);
    _filial.Excluir.visible(false);
    _filial.Cancelar.visible(false);
   
    LimparCampos(_filial);
}

function ValidarFilial() {
    for (var i = 0; i < _produtoEmbarcador.Filiais.list.length; i++) {
        let filial = _produtoEmbarcador.Filiais.list[i];
        if (filial.CodigoFilial.val == _filial.Filial.codEntity() && filial.Codigo.val != _filial.Codigo.val()) {
            exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.Aviso, "Esta filial já esta vinculada a este produto.");
            return false;
        }
    }

    return true;
}