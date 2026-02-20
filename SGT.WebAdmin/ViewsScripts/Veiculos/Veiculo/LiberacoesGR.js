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
/// <reference path="../../Consultas/Licenca.js" />

var _gridVeiculoLiberacoesGR;

//*******MAPEAMENTO KNOUCKOUT*******

var VeiculoLiberacaoGRMap = function () {
    this.Codigo = PropertyEntity({ type: types.map, val: 0, def: 0, getType: typesKnockout.int });
    this.Descricao = PropertyEntity({ type: types.map, val: "" });
    this.Numero = PropertyEntity({ type: types.map, val: 0 });
    this.DataEmissao = PropertyEntity({ type: types.map, val: "" });
    this.DataVencimento = PropertyEntity({ type: types.map, val: "" });
    this.CodigoLicenca = PropertyEntity({ type: types.map, val: "" });
    this.DescricaoLicenca = PropertyEntity({ type: types.map, val: "" });
};

//*******EVENTOS*******

function loadVeiculoLiberacaoGR() {

    limparCamposVeiculoLiberacoesGR();

    BuscarLicenca(_veiculo.LicencaLiberacaoGR);

    let auditar = { descricao: "Auditar", id: guid(), metodo: OpcaoAuditoria("VeiculoLiberacaoGR"), icone: "", visibilidade: true };
    let editar = { descricao: Localization.Resources.Gerais.Geral.Editar, id: guid(), evento: "onclick", metodo: editarVeiculoLiberacaoGR, tamanho: "15", icone: "" };
    let menuOpcoes = new Object();

    menuOpcoes.tipo = TypeOptionMenu.list;
    menuOpcoes.opcoes = new Array();
    menuOpcoes.opcoes.push(editar, auditar);

    let header = [
        { data: "Codigo", visible: false },
        { data: "CodigoLicenca", visible: false },
        { data: "Descricao", title: Localization.Resources.Gerais.Geral.Descricao, width: "35%", className: "text-align-left" },
        { data: "Numero", title: Localization.Resources.Veiculos.Veiculo.Numero, width: "15%", className: "text-align-left" },
        { data: "DataEmissao", title: Localization.Resources.Veiculos.Veiculo.DataEmissao, width: "15%", className: "text-align-center" },
        { data: "DataVencimento", title: Localization.Resources.Veiculos.Veiculo.DataVencimento, width: "15%", className: "text-align-center" },
        { data: "DescricaoLicenca", title: Localization.Resources.Veiculos.Veiculo.Licenca, width: "20%", className: "text-align-left" }
    ];

    _gridVeiculoLiberacoesGR = new BasicDataTable(_veiculo.GridVeiculoLiberacoesGR.idGrid, header, menuOpcoes);
    recarregarGridVeiculoLiberacoesGR();
}

function adicionarVeiculoLiberacaoGRClick() {

    if (_veiculo.DescricaoLiberacaoGR.val() === "" || _veiculo.NumeroLiberacaoGR.val() === "" || _veiculo.DataEmissaoLiberacaoGR.val() === "" || _veiculo.DataVencimentoLiberacaoGR.val() === "" || _veiculo.LicencaLiberacaoGR.val() === "") {
        exibirMensagem("atencao", Localization.Resources.Gerais.Geral.CamposObrigatorios, Localization.Resources.Gerais.Geral.InformeCamposObrigatorios);
        return;
    }

    let map = new VeiculoLiberacaoGRMap();

    map.Codigo.val = guid();
    map.Descricao.val = _veiculo.DescricaoLiberacaoGR.val();
    map.Numero.val = _veiculo.NumeroLiberacaoGR.val();
    map.DataEmissao.val = _veiculo.DataEmissaoLiberacaoGR.val();
    map.DataVencimento.val = _veiculo.DataVencimentoLiberacaoGR.val();
    map.DescricaoLicenca.val = _veiculo.LicencaLiberacaoGR.val();
    map.CodigoLicenca.val = _veiculo.LicencaLiberacaoGR.codEntity();

    _veiculo.GridVeiculoLiberacoesGR.list.push(map);

    recarregarGridVeiculoLiberacoesGR();

    $("#" + _veiculo.DescricaoLiberacaoGR.id).focus();

    limparCamposVeiculoLiberacoesGR();
}

function atualizarVeiculoLiberacaoGRClick() {

    if (_veiculo.DescricaoLiberacaoGR.val() === "" || _veiculo.NumeroLiberacaoGR.val() === "" || _veiculo.DataEmissaoLiberacaoGR.val() === "" || _veiculo.DataVencimentoLiberacaoGR.val() === "" || _veiculo.LicencaLiberacaoGR.val() === "") {
        exibirMensagem("atencao", Localization.Resources.Gerais.Geral.CamposObrigatorios, Localization.Resources.Gerais.Geral.InformeCamposObrigatorios);
        return;
    }

    $.each(_veiculo.GridVeiculoLiberacoesGR.list, function (i, item) {
        if (item.Codigo.val == _veiculo.CodigoLiberacaoGR.val()) {
            item.Numero.val = _veiculo.NumeroLiberacaoGR.val();
            item.Descricao.val = _veiculo.DescricaoLiberacaoGR.val();
            item.DataEmissao.val = _veiculo.DataEmissaoLiberacaoGR.val();
            item.DataVencimento.val = _veiculo.DataVencimentoLiberacaoGR.val();
            item.DescricaoLicenca.val = _veiculo.LicencaLiberacaoGR.val();
            item.CodigoLicenca.val = _veiculo.LicencaLiberacaoGR.codEntity();
            return false;
        }
    });

    recarregarGridVeiculoLiberacoesGR();
    limparCamposVeiculoLiberacoesGR();
}

function excluirVeiculoLiberacaoGRClick() {

    exibirConfirmacao(Localization.Resources.Gerais.Geral.Confirmacao, Localization.Resources.Veiculos.Veiculo.RealmenteDesejaExcluirLiberacaoGRSelecionada,
        function () {
            _veiculo.GridVeiculoLiberacoesGR.list = _veiculo.GridVeiculoLiberacoesGR.list.filter(liberacaoGR => liberacaoGR.Codigo.val != _veiculo.CodigoLiberacaoGR.val());
            recarregarGridVeiculoLiberacoesGR();
            limparCamposVeiculoLiberacoesGR();
        }
    );
}

function recarregarGridVeiculoLiberacoesGR() {

    let dados = _veiculo.GridVeiculoLiberacoesGR.list.map(item => ({
        Codigo: item.Codigo.val,
        Descricao: item.Descricao.val,
        Numero: item.Numero.val,
        DataEmissao: item.DataEmissao.val,
        DataVencimento: item.DataVencimento.val,
        CodigoLicenca: item.CodigoLicenca.val,
        DescricaoLicenca: item.DescricaoLicenca.val
    }));

    _gridVeiculoLiberacoesGR.CarregarGrid(dados);
}

function carregarGridVeiculoLiberacoesGR(lista) {

    if (lista === "undefined" || lista === null || lista.length === 0) {
        return;
    }

    for (let i = 0; i <= lista.length - 1; i++) {
        let map = new VeiculoLiberacaoGRMap();
        map.Codigo.val = lista[i].Codigo;
        map.Descricao.val = lista[i].Descricao;
        map.Numero.val = lista[i].Numero;
        map.DataEmissao.val = lista[i].DataEmissao;
        map.DataVencimento.val = lista[i].DataVencimento;
        map.CodigoLicenca.val = lista[i].Licenca.Codigo;
        map.DescricaoLicenca.val = lista[i].Licenca.Descricao;        
        _veiculo.GridVeiculoLiberacoesGR.list.push(map);
    }

    recarregarGridVeiculoLiberacoesGR();
}

function editarVeiculoLiberacaoGR(data) {

    limparCamposVeiculoLiberacoesGR();

    $.each(_veiculo.GridVeiculoLiberacoesGR.list, function (i, item) {
        if (item.Codigo.val == data.Codigo) {
            _veiculo.CodigoLiberacaoGR.val(item.Codigo.val);
            _veiculo.DescricaoLiberacaoGR.val(item.Descricao.val);
            _veiculo.NumeroLiberacaoGR.val(item.Numero.val);
            _veiculo.DataEmissaoLiberacaoGR.val(item.DataEmissao.val);
            _veiculo.DataVencimentoLiberacaoGR.val(item.DataVencimento.val);
            _veiculo.LicencaLiberacaoGR.val(item.DescricaoLicenca.val);
            _veiculo.LicencaLiberacaoGR.codEntity(item.CodigoLicenca.codEntity);
            return false;
        }
    });

    _veiculo.AdicionarLiberacaoGR.visible(false);
    _veiculo.AtualizarLiberacaoGR.visible(true);
    _veiculo.ExcluirLiberacaoGR.visible(true);
    _veiculo.CancelarLiberacaoGR.visible(true);
}

function obterListaLiberacoesGR() {

    if (_veiculo.GridVeiculoLiberacoesGR.list.length == 0)
        return "";

    let dados = _veiculo.GridVeiculoLiberacoesGR.list.map(item => ({
        Codigo: item.Codigo.val,
        Descricao: item.Descricao.val,
        Numero: item.Numero.val,
        DataEmissao: item.DataEmissao.val,
        DataVencimento: item.DataVencimento.val,
        CodigoLicenca: item.CodigoLicenca.val,
        DescricaoLicenca: item.DescricaoLicenca.val
    }));
    
    return JSON.stringify(dados);
}

function limparCamposVeiculoLiberacoesGR() {
    _veiculo.DescricaoLiberacaoGR.val("");
    _veiculo.NumeroLiberacaoGR.val("");
    _veiculo.DataEmissaoLiberacaoGR.val("");
    _veiculo.DataVencimentoLiberacaoGR.val("");
    _veiculo.AdicionarLiberacaoGR.visible(true);
    _veiculo.AtualizarLiberacaoGR.visible(false);
    _veiculo.ExcluirLiberacaoGR.visible(false);
    _veiculo.CancelarLiberacaoGR.visible(false);
    LimparCampoEntity(_veiculo.LicencaLiberacaoGR);
}

//function configurarTabsPorTipoSistema() {
//    if (_CONFIGURACAO_TMS.TipoServicoMultisoftware === EnumTipoServicoMultisoftware.MultiEmbarcador) {

//        $("#liTabLiberacaoGR").hide();

//    }
//}