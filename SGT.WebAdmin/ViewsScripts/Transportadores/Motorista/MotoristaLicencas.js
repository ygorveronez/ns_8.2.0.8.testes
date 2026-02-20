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

var _gridMotoristaLicencas;

//*******MAPEAMENTO KNOUCKOUT*******

var MotoristaLicencaMap = function () {
    this.Codigo = PropertyEntity({ type: types.map, val: 0, def: 0, getType: typesKnockout.int });
    this.Descricao = PropertyEntity({ type: types.map, val: "" });
    this.Numero = PropertyEntity({ type: types.map, val: "" });
    this.DataEmissao = PropertyEntity({ type: types.map, val: "" });
    this.DataVencimento = PropertyEntity({ type: types.map, val: "" });
    this.FormaAlerta = PropertyEntity({ type: types.map, val: "" });
    this.Status = PropertyEntity({ type: types.map, val: "" });
    this.CodigoLicenca = PropertyEntity({ type: types.map, val: "" });
    this.DescricaoLicenca = PropertyEntity({ type: types.map, val: "" });
    this.BloquearCriacaoPedidoLicencaVencida = PropertyEntity({ type: types.map, val: "" });
    this.BloquearCriacaoPlanejamentoPedidoLicencaVencida = PropertyEntity({ type: types.map, val: "" });
    this.ConfirmadaLeituraPendencia = PropertyEntity({ type: types.map, val: "" });
};

//*******EVENTOS*******

function loadMotoristaLicenca() {
    new BuscarLicenca(_motorista.Licenca);

    var auditar = { descricao: "Auditar", id: guid(), metodo: OpcaoAuditoria("MotoristaLicenca"), icone: "", visibilidade: true };
    var editar = { descricao: Localization.Resources.Gerais.Geral.Editar, id: guid(), evento: "onclick", metodo: editarMotoristaLicenca, tamanho: "15", icone: "" };
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
        { data: "DescricaoLicenca", title: Localization.Resources.Transportadores.Motorista.Licenca, width: "20%", className: "text-align-left" },
        { data: "ConfirmadaLeituraPendencia", title: Localization.Resources.Transportadores.Motorista.ConfirmadaLeituraPendencia, width: "20%", className: "text-align-left", visible: _CONFIGURACAO_TMS.TipoServicoMultisoftware === EnumTipoServicoMultisoftware.MultiTMS },
        { data: "FormaAlerta", visible: false },
        { data: "Status", visible: false },
        { data: "BloquearCriacaoPedidoLicencaVencida", visible: false },
        { data: "BloquearCriacaoPlanejamentoPedidoLicencaVencida", visible: false }
    ];

    _gridMotoristaLicencas = new BasicDataTable(_motorista.GridMotoristaLicencas.idGrid, header, menuOpcoes);
    recarregarGridMotoristaLicencas();
}

function adicionarMotoristaLicencaClick() {
    var tudoCerto = true;
    if (_motorista.Descricao.val() == "")
        tudoCerto = false;
    if (_motorista.Numero.val() == "")
        tudoCerto = false;
    if (_motorista.DataEmissao.val() == "")
        tudoCerto = false;
    if (_motorista.DataVencimento.val() == "")
        tudoCerto = false;

    if (tudoCerto) {
        var existe = false;
        if (!existe) {
            var motoristaLicenca = new MotoristaLicencaMap();
            motoristaLicenca.Codigo.val = guid();
            motoristaLicenca.Descricao.val = _motorista.Descricao.val();
            motoristaLicenca.Numero.val = _motorista.Numero.val();
            motoristaLicenca.DataEmissao.val = _motorista.DataEmissao.val();
            motoristaLicenca.DataVencimento.val = _motorista.DataVencimento.val();
            motoristaLicenca.Status.val = _motorista.StatusLicenca.val();
            motoristaLicenca.DescricaoLicenca.val = _motorista.Licenca.val();
            motoristaLicenca.CodigoLicenca.val = _motorista.Licenca.codEntity();
            motoristaLicenca.FormaAlerta.val = JSON.stringify(_motorista.FormaAlerta.val());
            motoristaLicenca.BloquearCriacaoPedidoLicencaVencida.val = _motorista.BloquearCriacaoPedidoLicencaVencida.val();
            motoristaLicenca.BloquearCriacaoPlanejamentoPedidoLicencaVencida.val = _motorista.BloquearCriacaoPlanejamentoPedidoLicencaVencida.val();
            motoristaLicenca.ConfirmadaLeituraPendencia.val = false;

            _motorista.GridMotoristaLicencas.list.push(motoristaLicenca);
            recarregarGridMotoristaLicencas();
            $("#" + _motorista.Descricao.id).focus();
        }
        LimparCamposMotoristaLicencas();
    } else {
        exibirMensagem("atencao", Localization.Resources.Gerais.Geral.CamposObrigatorios, Localization.Resources.Gerais.Geral.InformeCamposObrigatorios);
    }
}

function atualizarMotoristaLicencaClick() {
    var tudoCerto = true;
    if (_motorista.Descricao.val() == "")
        tudoCerto = false;
    if (_motorista.Numero.val() == "")
        tudoCerto = false;
    if (_motorista.DataEmissao.val() == "")
        tudoCerto = false;
    if (_motorista.DataVencimento.val() == "")
        tudoCerto = false;

    if (tudoCerto) {
        $.each(_motorista.GridMotoristaLicencas.list, function (i, motoristaLicenca) {
            if (motoristaLicenca.Codigo.val == _motorista.CodigoLicenca.val()) {

                motoristaLicenca.Numero.val = _motorista.Numero.val();
                motoristaLicenca.Descricao.val = _motorista.Descricao.val();
                motoristaLicenca.DataEmissao.val = _motorista.DataEmissao.val();
                motoristaLicenca.DataVencimento.val = _motorista.DataVencimento.val();
                motoristaLicenca.Status.val = _motorista.StatusLicenca.val();
                motoristaLicenca.DescricaoLicenca.val = _motorista.Licenca.val();
                motoristaLicenca.CodigoLicenca.val = _motorista.Licenca.codEntity();
                motoristaLicenca.FormaAlerta.val = JSON.stringify(_motorista.FormaAlerta.val());
                motoristaLicenca.BloquearCriacaoPedidoLicencaVencida.val = _motorista.BloquearCriacaoPedidoLicencaVencida.val();
                motoristaLicenca.BloquearCriacaoPlanejamentoPedidoLicencaVencida.val = _motorista.BloquearCriacaoPlanejamentoPedidoLicencaVencida.val();

                return false;
            }
        });
        recarregarGridMotoristaLicencas();
        LimparCamposMotoristaLicencas();
    } else {
        exibirMensagem("atencao", Localization.Resources.Gerais.Geral.CamposObrigatorios, Localization.Resources.Gerais.Geral.InformeCamposObrigatorios);
    }
}

function excluirMotoristaLicencaClick() {
    exibirConfirmacao(Localization.Resources.Gerais.Geral.Confirmacao, Localization.Resources.Transportadores.Motorista.RealmenteDesejaExcluirLicencaSelecionada, function () {
        var listaAtualizada = new Array();
        $.each(_motorista.GridMotoristaLicencas.list, function (i, motoristaLicenca) {
            if (motoristaLicenca.Codigo.val != _motorista.CodigoLicenca.val()) {
                listaAtualizada.push(motoristaLicenca);
            }
        });
        _motorista.GridMotoristaLicencas.list = listaAtualizada;
        recarregarGridMotoristaLicencas();
        LimparCamposMotoristaLicencas();
    });
}

//*******MÉTODOS*******

function recarregarGridMotoristaLicencas() {
    var data = new Array();
    $.each(_motorista.GridMotoristaLicencas.list, function (i, motorista) {
        var motoristaLicenca = new Object();

        motoristaLicenca.Codigo = motorista.Codigo.val;
        motoristaLicenca.Descricao = motorista.Descricao.val;
        motoristaLicenca.Numero = motorista.Numero.val;
        motoristaLicenca.DataEmissao = motorista.DataEmissao.val;
        motoristaLicenca.DataVencimento = motorista.DataVencimento.val;
        motoristaLicenca.Status = motorista.Status.val;
        motoristaLicenca.FormaAlerta = motorista.FormaAlerta.val;
        motoristaLicenca.CodigoLicenca = motorista.CodigoLicenca.val;
        motoristaLicenca.DescricaoLicenca = motorista.DescricaoLicenca.val;
        motoristaLicenca.BloquearCriacaoPedidoLicencaVencida = motorista.BloquearCriacaoPedidoLicencaVencida.val;
        motoristaLicenca.BloquearCriacaoPlanejamentoPedidoLicencaVencida = motorista.BloquearCriacaoPlanejamentoPedidoLicencaVencida.val;
        motoristaLicenca.ConfirmadaLeituraPendencia = motorista.ConfirmadaLeituraPendencia.val ? "Sim" : "Não";

        data.push(motoristaLicenca);
    });
    _gridMotoristaLicencas.CarregarGrid(data);
}

function editarMotoristaLicenca(data) {
    LimparCamposMotoristaLicencas();
    $.each(_motorista.GridMotoristaLicencas.list, function (i, motoristaLicenca) {
        if (motoristaLicenca.Codigo.val == data.Codigo) {
            _motorista.CodigoLicenca.val(motoristaLicenca.Codigo.val);
            _motorista.Descricao.val(motoristaLicenca.Descricao.val);
            _motorista.Numero.val(motoristaLicenca.Numero.val);
            _motorista.DataEmissao.val(motoristaLicenca.DataEmissao.val);
            _motorista.DataVencimento.val(motoristaLicenca.DataVencimento.val);
            _motorista.StatusLicenca.val(motoristaLicenca.Status.val);
            //_motorista.FormaAlerta.val(JSON.parse(motoristaLicenca.FormaAlerta.val));
            _motorista.Licenca.val(motoristaLicenca.DescricaoLicenca.val);
            _motorista.Licenca.codEntity(motoristaLicenca.CodigoLicenca.codEntity);

            $("#" + _motorista.FormaAlerta.id).selectpicker('val', JSON.parse(motoristaLicenca.FormaAlerta.val));
            
            _motorista.BloquearCriacaoPedidoLicencaVencida.val(motoristaLicenca.BloquearCriacaoPedidoLicencaVencida.val);
            _motorista.BloquearCriacaoPlanejamentoPedidoLicencaVencida.val(motoristaLicenca.BloquearCriacaoPlanejamentoPedidoLicencaVencida.val);

            return false;
        }
    });

    _motorista.AdicionarLicenca.visible(false);
    _motorista.AtualizarLicenca.visible(true);
    _motorista.ExcluirLicenca.visible(true);
    _motorista.CancelarLicenca.visible(true);
}

function LimparCamposMotoristaLicencas() {
    _motorista.Descricao.val("");
    _motorista.Numero.val("");
    _motorista.DataEmissao.val("");
    _motorista.DataVencimento.val("");
    LimparCampoEntity(_motorista.Licenca);
    _motorista.StatusLicenca.val(EnumStatusLicenca.Vigente);
    //_motorista.FormaAlerta.val(new Array());

    $("#" + _motorista.FormaAlerta.id).selectpicker('val', []);

    _motorista.BloquearCriacaoPedidoLicencaVencida.val(false);
    _motorista.BloquearCriacaoPlanejamentoPedidoLicencaVencida.val(false);

    _motorista.AdicionarLicenca.visible(true);
    _motorista.AtualizarLicenca.visible(false);
    _motorista.ExcluirLicenca.visible(false);
    _motorista.CancelarLicenca.visible(false);
}