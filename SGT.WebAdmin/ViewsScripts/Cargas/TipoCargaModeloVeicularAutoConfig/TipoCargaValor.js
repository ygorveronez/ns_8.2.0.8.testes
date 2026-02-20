/// <reference path="../../Enumeradores/EnumValorTipoAutomatizacaoTipoCarga.js" />
/// <reference path="../../../js/libs/jquery-2.1.1.js" />
/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/knockout-3.1.0.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/bootstrap/bootstrap.js" />
/// <reference path="../../../js/libs/jquery.blockui.js" />
/// <reference path="../../../js/Global/knoutViewsSlides.js" />
/// <reference path="../../../js/libs/jquery.maskMoney.js" />
/// <reference path="../../../js/plugin/datatables/jquery.dataTables.js" />
/// <reference path="../../Consultas/TipoCarga.js" />

/// <reference path="TipoCargaModeloVeicularAutoConfig.js" />


//*******MAPEAMENTO KNOUCKOUT*******

var _gridTipoCargaValor;

var TipoCargaValorMap = function () {
    this.Valor = PropertyEntity({ val: 0, def: 0 });
    this.Codigo = PropertyEntity({ val: 0, def: 0 });
    this.Descricao = PropertyEntity({ val: "" });
    this.TipoValor = PropertyEntity({ val: "" });
    this.DescricaoTipoValor = PropertyEntity({ val: "" });
    this.CodigoUFDestino = PropertyEntity({ val: "" });
    this.UFDestino = PropertyEntity({ val: "" });
}

//*******EVENTOS*******

function loadTipoCargaValor() {

    new BuscarTiposdeCarga(_tipoCargaModeloVeicularAutoConfig.TiposCargaValor);
    new BuscarEstados(_tipoCargaModeloVeicularAutoConfig.UFDestino);

    var menuOpcoes = { tipo: TypeOptionMenu.link, tamanho: 7, opcoes: [{ descricao: Localization.Resources.Gerais.Geral.Excluir, id: guid(), metodo: excluirTipoCargaValorClick }] };

    var header = [{ data: "Codigo", visible: false },
    { data: "TipoValor", visible: false },
    { data: "Descricao", title: Localization.Resources.Gerais.Geral.TipoCarga, width: "35%" },
    { data: "DescricaoTipoValor", title: Localization.Resources.Cargas.TipoCargaModeloVeicularAutoConfig.TiposValor, width: "20%" },
    { data: "Valor", title: Localization.Resources.Gerais.Geral.Valor, width: "20%" },
    { data: "CodigoUFDestino", visible: false },
    { data: "UFDestino", title: Localization.Resources.Gerais.Geral.Estado, width: "10%" }];

    _gridTipoCargaValor = new BasicDataTable(_tipoCargaModeloVeicularAutoConfig.GridTipoCargaValor.id, header, menuOpcoes, { column: 4, dir: orderDir.asc });

    recarregarGridTipoCargaValor();
}

function recarregarGridTipoCargaValor() {

    var data = new Array();

    $.each(_tipoCargaModeloVeicularAutoConfig.TiposCargaValor.list, function (i, tipoCargaValor) {
        var tipoCargaValorGrid = new Object();
        tipoCargaValorGrid.Codigo = tipoCargaValor.Codigo.val;
        tipoCargaValorGrid.TipoValor = tipoCargaValor.TipoValor.val;
        tipoCargaValorGrid.Descricao = tipoCargaValor.Descricao.val;
        tipoCargaValorGrid.DescricaoTipoValor = tipoCargaValor.TipoValor.val == EnumValorAutomatizacaoTipoCargaValor.Ate ? Localization.Resources.Cargas.TipoCargaModeloVeicularAutoConfig.Ate : Localization.Resources.Cargas.TipoCargaModeloVeicularAutoConfig.AcimaDe;
        tipoCargaValorGrid.Valor = tipoCargaValor.Valor.val;
        tipoCargaValorGrid.CodigoUFDestino = tipoCargaValor.CodigoUFDestino.val;
        tipoCargaValorGrid.UFDestino = tipoCargaValor.UFDestino.val;

        data.push(tipoCargaValorGrid);
    });

    _gridTipoCargaValor.CarregarGrid(data);
}

function adicionarTipoCargaValorClick(e, sender) {
    var tudoCerto = ValidarCampoObrigatorioEntity(_tipoCargaModeloVeicularAutoConfig.TiposCargaValor);
    if (tudoCerto) {
        var existe = false;
        $.each(_tipoCargaModeloVeicularAutoConfig.TiposCargaValor.list, function (i, tipoCargaValor) {
            if (tipoCargaValor.Codigo.val == _tipoCargaModeloVeicularAutoConfig.TiposCargaValor.codEntity() && tipoCargaValor.CodigoUFDestino.val == _tipoCargaModeloVeicularAutoConfig.UFDestino.codEntity()) {
                existe = true;
                return false;
            }
        });
        if (!existe) {
            var tipoCargaValor = new TipoCargaValorMap();
            tipoCargaValor.Codigo.val = parseInt(_tipoCargaModeloVeicularAutoConfig.TiposCargaValor.codEntity());
            tipoCargaValor.Descricao.val = _tipoCargaModeloVeicularAutoConfig.TiposCargaValor.val();
            tipoCargaValor.TipoValor.val = _tipoCargaModeloVeicularAutoConfig.TipoValorTipoCargaValor.val();
            tipoCargaValor.Valor.val = _tipoCargaModeloVeicularAutoConfig.ValorTipoCargaValor.val();
            tipoCargaValor.CodigoUFDestino.val = _tipoCargaModeloVeicularAutoConfig.UFDestino.codEntity();
            tipoCargaValor.UFDestino.val = _tipoCargaModeloVeicularAutoConfig.UFDestino.val();

            _tipoCargaModeloVeicularAutoConfig.TiposCargaValor.list.push(tipoCargaValor);
            recarregarGridTipoCargaValor();
            LimparCampoEntity(_tipoCargaModeloVeicularAutoConfig.TiposCargaValor);
            _tipoCargaModeloVeicularAutoConfig.TipoValorTipoCargaValor.val(_tipoCargaModeloVeicularAutoConfig.TipoValorTipoCargaValor.def);
            _tipoCargaModeloVeicularAutoConfig.ValorTipoCargaValor.val(_tipoCargaModeloVeicularAutoConfig.ValorTipoCargaValor.def);

            $("#" + _tipoCargaModeloVeicularAutoConfig.TiposCargaValor.id).focus();
        } else {
            exibirMensagem(tipoMensagem.aviso, Localization.Resources.Cargas.TipoCargaModeloVeicularAutoConfig.TipoCargaJaInformado, Localization.Resources.Cargas.TipoCargaModeloVeicularAutoConfig.TipoCargaInformadoFoiAdicionado);
        }
    } else {
        exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.CampoObrigatorio, Localization.Resources.Cargas.TipoCargaModeloVeicularAutoConfig.InformeTipoCarga);
    }
}

function excluirTipoCargaValorClick(data) {
    exibirConfirmacao(Localization.Resources.Gerais.Geral.Confirmacao, Localization.Resources.Gerais.Geral.DesejaRealmenteRemoverTipoCarga + data.Descricao + " ?", function () {
        for (var i = 0; i < _tipoCargaModeloVeicularAutoConfig.TiposCargaValor.list.length; i++) {
            if (data.Codigo == _tipoCargaModeloVeicularAutoConfig.TiposCargaValor.list[i].Codigo.val) {
                _tipoCargaModeloVeicularAutoConfig.TiposCargaValor.list.splice(i, 1);
                break;
            }
        }

        recarregarGridTipoCargaValor();
    });
}
