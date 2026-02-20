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
/// <reference path="Motorista.js" />

var _gridMotoristaLiberacoesGR;

//*******MAPEAMENTO KNOUCKOUT*******

var MotoristaLiberacaoGRMap = function () {
    this.Codigo = PropertyEntity({ type: types.map, val: 0, def: 0, getType: typesKnockout.int });
    this.Descricao = PropertyEntity({ type: types.map, val: "" });
    this.Numero = PropertyEntity({ type: types.map, val: "" });
    this.DataEmissao = PropertyEntity({ type: types.map, val: "" });
    this.DataVencimento = PropertyEntity({ type: types.map, val: "" });
    this.CodigoLicenca = PropertyEntity({ type: types.map, val: "" });
    this.DescricaoLicenca = PropertyEntity({ type: types.map, val: "" });
};

//*******EVENTOS*******

function loadMotoristaLiberacaoGR() {
    new BuscarLicenca(_motorista.LicencaLiberacaoGR);

    var auditar = { descricao: "Auditar", id: guid(), metodo: OpcaoAuditoria("MotoristaLiberacaoGR"), icone: "", visibilidade: true };
    var editar = { descricao: Localization.Resources.Gerais.Geral.Editar, id: guid(), evento: "onclick", metodo: editarMotoristaLiberacaoGR, tamanho: "15", icone: "" };
    var menuOpcoes = new Object();
    menuOpcoes.tipo = TypeOptionMenu.list;
    menuOpcoes.opcoes = new Array();
    menuOpcoes.opcoes.push(editar, auditar);

    var header = [
        { data: "Codigo", visible: false },
        { data: "CodigoLicenca", visible: false },
        { data: "Descricao", title: Localization.Resources.Gerais.Geral.Descricao, width: "35%", className: "text-align-left" },
        { data: "Numero", title: Localization.Resources.Transportadores.Motorista.Numero, width: "15%", className: "text-align-left" },
        { data: "DataEmissao", title: Localization.Resources.Transportadores.Motorista.DataEmissao, width: "15%", className: "text-align-center" },
        { data: "DataVencimento", title: Localization.Resources.Transportadores.Motorista.DataVencimento, width: "15%", className: "text-align-center" },
        { data: "DescricaoLicenca", title: Localization.Resources.Transportadores.Motorista.Licenca, width: "20%", className: "text-align-left" }
    ];

    _gridMotoristaLiberacoesGR = new BasicDataTable(_motorista.GridMotoristaLiberacoesGR.idGrid, header, menuOpcoes);
    recarregarGridMotoristaLiberacoesGR();
}

function adicionarMotoristaLiberacaoGRClick() {
    var tudoCerto = true;
    if (_motorista.DescricaoLiberacaoGR.val() == "")
        tudoCerto = false;
    if (_motorista.NumeroLiberacaoGR.val() == "")
        tudoCerto = false;
    if (_motorista.DataEmissaoLiberacaoGR.val() == "")
        tudoCerto = false;
    if (_motorista.DataVencimentoLiberacaoGR.val() == "")
        tudoCerto = false;
    if (_motorista.LicencaLiberacaoGR.val() == "")
        tudoCerto = false;

    if (tudoCerto) {
        var existe = false;
        if (!existe) {
            var motoristaLiberacaoGR = new MotoristaLiberacaoGRMap();
            motoristaLiberacaoGR.Codigo.val = guid();
            motoristaLiberacaoGR.Descricao.val = _motorista.DescricaoLiberacaoGR.val();
            motoristaLiberacaoGR.Numero.val = _motorista.NumeroLiberacaoGR.val();
            motoristaLiberacaoGR.DataEmissao.val = _motorista.DataEmissaoLiberacaoGR.val();
            motoristaLiberacaoGR.DataVencimento.val = _motorista.DataVencimentoLiberacaoGR.val();
            motoristaLiberacaoGR.DescricaoLicenca.val = _motorista.LicencaLiberacaoGR.val();
            motoristaLiberacaoGR.CodigoLicenca.val = _motorista.LicencaLiberacaoGR.codEntity();

            _motorista.GridMotoristaLiberacoesGR.list.push(motoristaLiberacaoGR);
            recarregarGridMotoristaLiberacoesGR();
            $("#" + _motorista.Descricao.id).focus();
        }
        LimparCamposMotoristaLiberacoesGR();
    } else {
        exibirMensagem("atencao", Localization.Resources.Gerais.Geral.CamposObrigatorios, Localization.Resources.Gerais.Geral.InformeCamposObrigatorios);
    }
    console.log(motoristaLiberacaoGR);
}

function atualizarMotoristaLiberacaoGRClick() {
    var tudoCerto = true;
    if (_motorista.DescricaoLiberacaoGR.val() == "")
        tudoCerto = false;
    if (_motorista.NumeroLiberacaoGR.val() == "")
        tudoCerto = false;
    if (_motorista.DataEmissaoLiberacaoGR.val() == "")
        tudoCerto = false;
    if (_motorista.DataVencimentoLiberacaoGR.val() == "")
        tudoCerto = false;
    if (_motorista.LicencaLiberacaoGR.val() == "")
        tudoCerto = false;
    if (tudoCerto) {
        $.each(_motorista.GridMotoristaLiberacoesGR.list, function (i, motoristaLiberacaoGR) {
            if (motoristaLiberacaoGR.Codigo.val == _motorista.CodigoLiberacaoGR.val()) {

                motoristaLiberacaoGR.Numero.val = _motorista.NumeroLiberacaoGR.val();
                motoristaLiberacaoGR.Descricao.val = _motorista.DescricaoLiberacaoGR.val();
                motoristaLiberacaoGR.DataEmissao.val = _motorista.DataEmissaoLiberacaoGR.val();
                motoristaLiberacaoGR.DataVencimento.val = _motorista.DataVencimentoLiberacaoGR.val();
                motoristaLiberacaoGR.DescricaoLicenca.val = _motorista.LicencaLiberacaoGR.val();
                motoristaLiberacaoGR.CodigoLicenca.val = _motorista.LicencaLiberacaoGR.codEntity();

                return false;
            }
        });
        recarregarGridMotoristaLiberacoesGR();
        LimparCamposMotoristaLiberacoesGR();
    } else {
        exibirMensagem("atencao", Localization.Resources.Gerais.Geral.CamposObrigatorios, Localization.Resources.Gerais.Geral.InformeCamposObrigatorios);
    }
}

function excluirMotoristaLiberacaoGRClick() {
    exibirConfirmacao(Localization.Resources.Gerais.Geral.Confirmacao, Localization.Resources.Transportadores.Motorista.RealmenteDesejaExcluirLiberacaoGRSelecionada, function () {
        var listaAtualizada = new Array();
        $.each(_motorista.GridMotoristaLiberacoesGR.list, function (i, motoristaLiberacaoGR) {
            if (motoristaLiberacaoGR.Codigo.val != _motorista.CodigoLiberacaoGR.val()) {
                listaAtualizada.push(motoristaLiberacaoGR);
            }
        });
        _motorista.GridMotoristaLiberacoesGR.list = listaAtualizada;
        recarregarGridMotoristaLiberacoesGR();
        LimparCamposMotoristaLiberacoesGR();
    });
}

//*******MÉTODOS*******

function recarregarGridMotoristaLiberacoesGR() {
    var data = new Array();
    $.each(_motorista.GridMotoristaLiberacoesGR.list, function (i, motorista) {
        var motoristaLiberacaoGR = new Object();

        motoristaLiberacaoGR.Codigo = motorista.Codigo.val;
        motoristaLiberacaoGR.Descricao = motorista.Descricao.val;
        motoristaLiberacaoGR.Numero = motorista.Numero.val;
        motoristaLiberacaoGR.DataEmissao = motorista.DataEmissao.val;
        motoristaLiberacaoGR.DataVencimento = motorista.DataVencimento.val;
        motoristaLiberacaoGR.CodigoLicenca = motorista.CodigoLicenca.val;
        motoristaLiberacaoGR.DescricaoLicenca = motorista.DescricaoLicenca.val;

        data.push(motoristaLiberacaoGR);
    });
    _gridMotoristaLiberacoesGR.CarregarGrid(data);
}

function editarMotoristaLiberacaoGR(data) {
    LimparCamposMotoristaLiberacoesGR();
    $.each(_motorista.GridMotoristaLiberacoesGR.list, function (i, motoristaLiberacaoGR) {
        if (motoristaLiberacaoGR.Codigo.val == data.Codigo) {
            _motorista.CodigoLiberacaoGR.val(motoristaLiberacaoGR.Codigo.val);
            _motorista.DescricaoLiberacaoGR.val(motoristaLiberacaoGR.Descricao.val);
            _motorista.NumeroLiberacaoGR.val(motoristaLiberacaoGR.Numero.val);
            _motorista.DataEmissaoLiberacaoGR.val(motoristaLiberacaoGR.DataEmissao.val);
            _motorista.DataVencimentoLiberacaoGR.val(motoristaLiberacaoGR.DataVencimento.val);
            _motorista.LicencaLiberacaoGR.val(motoristaLiberacaoGR.DescricaoLicenca.val);
            _motorista.LicencaLiberacaoGR.codEntity(motoristaLiberacaoGR.CodigoLicenca.val);

            return false;
        }
    });

    _motorista.AdicionarLiberacaoGR.visible(false);
    _motorista.AtualizarLiberacaoGR.visible(true);
    _motorista.ExcluirLiberacaoGR.visible(true);
    _motorista.CancelarLiberacaoGR.visible(true);
}

function LimparCamposMotoristaLiberacoesGR() {
    _motorista.DescricaoLiberacaoGR.val("");
    _motorista.NumeroLiberacaoGR.val("");
    _motorista.DataEmissaoLiberacaoGR.val("");
    _motorista.DataVencimentoLiberacaoGR.val("");
    LimparCampoEntity(_motorista.LicencaLiberacaoGR);

    _motorista.AdicionarLiberacaoGR.visible(true);
    _motorista.AtualizarLiberacaoGR.visible(false);
    _motorista.ExcluirLiberacaoGR.visible(false);
    _motorista.CancelarLiberacaoGR.visible(false);
}