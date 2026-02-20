/// <reference path="Configuracao.js" />

var _gridConfiguracoesPaginacaoInterfaces;
//*******MAPEAMENTO KNOUCKOUT*******
var ConfiguracaoPaginacaoMap = function () {
    this.Codigo = PropertyEntity({ type: types.map, val: 0, def: 0, getType: typesKnockout.int });
    this.Interface = PropertyEntity({ type: types.map, val: "" });
    this.Dias = PropertyEntity({ type: types.map, val: "" });
    this.DescricaoInterface = PropertyEntity({ type: types.map, val: "" });
};

function loadConfiguracaoPaginacao() {
    loadGridConfiguracoesPaginacao();
}

function adicionarConfiguracaoPaginacaoClick() {
    if (_configuracaoEmbarcador.ConfiguracoesPaginacaoDias.val() === "") {
        exibirMensagem("atencao", Localization.Resources.Gerais.Geral.CamposObrigatorios, Localization.Resources.Gerais.Geral.InformeCamposObrigatorios);
        return;
    }

    var configuracaoInterface = new ConfiguracaoPaginacaoMap();
    configuracaoInterface.Codigo.val = guid();
    configuracaoInterface.Interface.val = _configuracaoEmbarcador.ConfiguracoesPaginacaoInterfaces.val();
    configuracaoInterface.Dias.val = _configuracaoEmbarcador.ConfiguracoesPaginacaoDias.val();
    configuracaoInterface.DescricaoInterface.val = EnumConfiguracaoPaginacaoInterfaces.obterDescricao(configuracaoInterface.Interface.val); 

    var listaPaginacaoInterfaces = _configuracaoEmbarcador.GridConfiguracoesPaginacaoInterfaces.list;

    for (var i = 0; i < listaPaginacaoInterfaces.length; i++) {
        if (listaPaginacaoInterfaces[i].Interface.val == configuracaoInterface.Interface.val) {
            exibirMensagem("atencao", "Interface já cadastrada!", "Já existe uma configuração de paginação cadastrada para esta interface.");
            return;
        }
    }

    _configuracaoEmbarcador.GridConfiguracoesPaginacaoInterfaces.list.push(configuracaoInterface);
    recarregarGridConfiguracoesPaginacaoInterfaces();
}

function loadGridConfiguracoesPaginacao() {
    var editar = { descricao: Localization.Resources.Gerais.Geral.Remover, id: guid(), evento: "onclick", metodo: excluirConfiguracaoPaginacaoClick, tamanho: "15", icone: "" };
    var menuOpcoes = new Object();
    menuOpcoes.tipo = TypeOptionMenu.link;
    menuOpcoes.opcoes = new Array();
    menuOpcoes.opcoes.push(editar);
    var header = [
        { data: "Codigo", visible: false },
        { data: "Interface", visible: false },
        { data: "DescricaoInterface", title: "Interfaces", width: "50%" },
        { data: "Dias", title: "Consultar registros a partir de", width: "50%" }
    ];
    _gridConfiguracoesPaginacaoInterfaces = new BasicDataTable(_configuracaoEmbarcador.GridConfiguracoesPaginacaoInterfaces.idGrid, header, menuOpcoes);
    recarregarGridConfiguracoesPaginacaoInterfaces();
}

function recarregarGridConfiguracoesPaginacaoInterfaces() {
    var data = new Array();
    
    $.each(_configuracaoEmbarcador.GridConfiguracoesPaginacaoInterfaces.list, function (i, configuracao) {
        var obj = new Object(); 
        obj.Codigo = configuracao.Codigo.val;
        obj.Dias = configuracao.Dias.val;
        obj.Interface = configuracao.Interface.val;
        obj.DescricaoInterface = configuracao.DescricaoInterface.val;

        data.push(obj);
    });
    _gridConfiguracoesPaginacaoInterfaces.CarregarGrid(data);
}

function excluirConfiguracaoPaginacaoClick(data) {
    var listaAtualizada = new Array();
    $.each(_configuracaoEmbarcador.GridConfiguracoesPaginacaoInterfaces.list, function (i, configuracaoPaginacao) {
        if (configuracaoPaginacao.Codigo.val != data.Codigo) {
            listaAtualizada.push(configuracaoPaginacao);
        }
    });
    _configuracaoEmbarcador.GridConfiguracoesPaginacaoInterfaces.list = listaAtualizada;
    recarregarGridConfiguracoesPaginacaoInterfaces();
    
}