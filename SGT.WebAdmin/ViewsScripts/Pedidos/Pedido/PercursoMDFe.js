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
/// <reference path="Pedido.js" />
/// <reference path="../../Logistica/Mapas/MapaBrasil.js" />
/// <reference path="../Mapas/MapaBrasil.js" />


//*******MAPEAMENTO KNOUCKOUT*******

var _mapaMDFe;
var _percursosEntreEstados;

var PassagemPercursoEstadoMap = function () {
    this.Ordem = PropertyEntity({ val: 0, def: 0 });
    this.Codigo = PropertyEntity({ val: 0, def: 0 });
    this.EstadoDePassagem = PropertyEntity({ val: "AC", def: "AC", options: _estados });
    this.DescricaoEstado = PropertyEntity({ type: types.local, val: "" });
}

var PercursosEntreEstados = function () {
    this.EstadoOrigem = PropertyEntity({ val: ko.observable("SC"), options: _estados, def: "SC", text: Localization.Resources.Pedidos.Pedido.EstadoOrigem.getRequiredFieldDescription(), required: true });
    this.EstadoDestino = PropertyEntity({ val: ko.observable("SC"), options: _estados, def: "SC", text: Localization.Resources.Pedidos.Pedido.EstadoDestino.getRequiredFieldDescription(), required: true });
    this.PassagemPercursoEstado = PropertyEntity({ type: types.listEntity, list: new Array(), options: _estados, val: ko.observable("AC"), def: "AC", codEntity: ko.observable("AC"), defCodEntity: "AC", text: Localization.Resources.Pedidos.Pedido.EstadoPassagem.getRequiredFieldDescription() });
}

//*******EVENTOS*******

function loadPercusoMDFe() {
    _percursosEntreEstados = new PercursosEntreEstados();

    _mapaMDFe = new MapaMDFe();
    _mapaMDFe.LoadMapaMDFe("mapa", function () {
        if (_pedido.UFOrigem.val() != "" && _pedido.UFDestino.val() != "") {
            _percursosEntreEstados.EstadoOrigem.val(_pedido.UFOrigem.val());
            _percursosEntreEstados.EstadoDestino.val(_pedido.UFDestino.val());
            setarOrigemDestinoMapa();
        }
    });
}

function mapaOrigemDestinoChange(e, sender) {
    _percursosEntreEstados.PassagemPercursoEstado.list = new Array();
    if (_pedido.UFOrigem.val() != "" && _pedido.UFDestino.val() != "") {
        _percursosEntreEstados.EstadoOrigem.val(_pedido.UFOrigem.val());
        _percursosEntreEstados.EstadoDestino.val(_pedido.UFDestino.val());
        setarOrigemDestinoMapa();

        if (!_pedido.TelaResumida.val())
            $("#tabPercursoMDFe").show();
    }
    else
        $("#tabPercursoMDFe").hide();
}

function setarOrigemDestinoMapa() {
    _mapaMDFe.LimparMapa();
    _mapaMDFe.AddOrigemDestino(_pedido.UFOrigem.val(), _pedido.UFDestino.val());

    _mapaMDFe.SetEstadoDestino(_percursosEntreEstados.EstadoDestino.val());

    _mapaMDFe.AddLocalidadesBuscaAPI(DescricaoCapitalEstado(_percursosEntreEstados.EstadoOrigem.val()));
    _mapaMDFe.AddLocalidadesBuscaAPI(DescricaoCapitalEstado(_percursosEntreEstados.EstadoDestino.val()));

    $.each(_percursosEntreEstados.PassagemPercursoEstado.list, function (i, estado) {
        _mapaMDFe.AddEstadoPassagem(estado.EstadoDePassagem.val, estado.Ordem.val);
    });
    _mapaMDFe.AtualizarDisplayMapa();
}

//*******MÉTODOS*******

function SetarPassagens() {
    if (_pedido.UFOrigem.val() != "" && _pedido.UFDestino.val() != "" && _mapaMDFe.GetEstadosPassagem().length > 0) {

        _percursosEntreEstados.EstadoOrigem.val(_pedido.UFOrigem.val());
        _percursosEntreEstados.EstadoDestino.val(_pedido.UFDestino.val());

        _percursosEntreEstados.PassagemPercursoEstado.list = new Array();
        _pedido.PassagemPercursoEstado.list = new Array();
        var valido = _mapaMDFe.ValidarPassagens();
        if (valido) {
            var estadosPassagem = _mapaMDFe.GetEstadosPassagem();
            var origensDestino = _mapaMDFe.GetOrigensDestinos();
            $.each(origensDestino, function (i, origemDestino) {
                $.each(origemDestino.Passagens, function (i, estado) {
                    var passagem = new PassagemPercursoEstadoMap();
                    passagem.Ordem.val = estado.Posicao;
                    passagem.EstadoDePassagem.val = estado.Sigla;
                    _percursosEntreEstados.PassagemPercursoEstado.list.push(passagem);
                    _pedido.PassagemPercursoEstado.list.push(passagem);
                });
            });
        }

        if (_pedido.TelaResumida.val())
            return true;

        if (!valido)
            $("#myTab a:eq(5)").tab("show");
        return valido;
    } else
        return true;
}