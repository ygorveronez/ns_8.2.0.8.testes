/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Globais.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="../../Consultas/TipoDeCarga.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var QuantidadePorTipoDeCargaModel = function (instancia) {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });

    this.Grid = PropertyEntity({ type: types.local });
    this.GridTiposDeCarga = PropertyEntity({ type: types.map, text: Localization.Resources.Logistica.ExcecaoCapacidadeDescarregamento.AdicionarTipoCarga, getType: typesKnockout.dynamic, val: ko.observable(new Array()), def: new Array(), idBtnSearch: guid(), idGrid: guid(), visible: ko.observable(false) });

    this.Tolerancia = PropertyEntity({ getType: typesKnockout.int, text: Localization.Resources.Logistica.ExcecaoCapacidadeDescarregamento.ToleranciaEmHoras.getFieldDescription(), val: ko.observable(0), def: 0, configInt: { precision: 0, allowZero: true } });
    this.ToleranciaCancelamentoAgendaConfirmada = PropertyEntity({ getType: typesKnockout.int, text: Localization.Resources.Logistica.ExcecaoCapacidadeDescarregamento.ToleranciaCancelamentoAgendasConfirmadasEmHoras.getFieldDescription(), val: ko.observable(0), def: 0, configInt: { precision: 0, allowZero: true } });
    this.ToleranciaCancelamentoAgendaNaoConfirmada = PropertyEntity({ getType: typesKnockout.int, text: Localization.Resources.Logistica.ExcecaoCapacidadeDescarregamento.TolerenciaCancelamento.getFieldDescription(), val: ko.observable(0), def: 0, configInt: { precision: 0, allowZero: true } });
    this.Volumes = PropertyEntity({ type: types.map, getType: typesKnockout.int, required: true, text: Localization.Resources.Logistica.ExcecaoCapacidadeDescarregamento.QuantidadeTipoCarga.getRequiredFieldDescription(), val: ko.observable(""), def: "" });
    this.TiposCarga = PropertyEntity({ type: types.map, text: Localization.Resources.Logistica.ExcecaoCapacidadeDescarregamento.AdicionarTipoCarga, getType: typesKnockout.dynamic, val: ko.observable(new Array()), def: new Array(), idBtnSearch: guid(), idGrid: guid(), visible: ko.observable(false) });

    this.Adicionar = PropertyEntity({ eventClick: instancia.Adicionar, type: types.event, text: Localization.Resources.Gerais.Geral.Adicionar, visible: ko.observable(true) });
}

var QuantidadePorDeTipoCarga = function (idKnockout, containerItens) {
    var $this = this;

    $this.IdKnockout = idKnockout;
    $this.Container = containerItens;

    this.Load = function () {
        $this.QuantidadePorTipoDeCarga = new QuantidadePorTipoDeCargaModel($this);
        KoBindings($this.QuantidadePorTipoDeCarga, $this.IdKnockout);

        var menuOpcoes = { tipo: TypeOptionMenu.link, tamanho: 7, opcoes: [{ descricao: Localization.Resources.Gerais.Geral.Excluir, id: guid(), metodo: $this.Excluir }] };

        var header = [
            { data: "Codigo", visible: false },
            { data: "TiposCarga", visible: false },
            { data: "DescricaoTipoCarga", title: Localization.Resources.Gerais.Geral.TipoCarga, width: "25%" },
            { data: "Volumes", title: Localization.Resources.Logistica.ExcecaoCapacidadeDescarregamento.QuantDeCaixas, width: "10%" },
            { data: "Tolerancia", title: Localization.Resources.Gerais.Geral.Tolerancia, width: "10%" },
            { data: "ToleranciaCancelamentoAgendaConfirmada", title: Localization.Resources.Logistica.ExcecaoCapacidadeDescarregamento.ToleranciaCancelamentoAgendasConfirmadasEmHoras, width: "20%" },
            { data: "ToleranciaCancelamentoAgendaNaoConfirmada", title: Localization.Resources.Logistica.ExcecaoCapacidadeDescarregamento.ToleranciaCancelamentoAgendaNaoConfirmada, width: "20%" }
        ];

        $this.Grid = new BasicDataTable($this.QuantidadePorTipoDeCarga.Grid.id, header, menuOpcoes, { column: 1, dir: orderDir.asc });

        $this.CarregarGrid();
        $this.LoadGridTiposCarga();
    }

    this.LoadGridTiposCarga = function () {
        var linhasPorPaginas = 5;
        var opcaoRemover = { descricao: Localization.Resources.Gerais.Geral.Remover, id: guid(), metodo: $this.ExcluirTipoCarga, icone: "" };
        var menuOpcoes = { tipo: TypeOptionMenu.link, tamanho: 21, opcoes: [opcaoRemover] };

        var header = [
            { data: "Codigo", visible: false },
            { data: "Descricao", title: Localization.Resources.Gerais.Geral.Descricao, width: "75%", className: "text-align-left" }
        ];

        $this.GridTiposDeCarga = new BasicDataTable($this.QuantidadePorTipoDeCarga.GridTiposDeCarga.idGrid, header, menuOpcoes, null, null, linhasPorPaginas);

        new BuscarTiposdeCarga($this.QuantidadePorTipoDeCarga.GridTiposDeCarga, null, null, $this.GridTiposDeCarga);
        $this.QuantidadePorTipoDeCarga.GridTiposDeCarga.basicTable = $this.GridTiposDeCarga;

        $this.GridTiposDeCarga.CarregarGrid([]);
    }

    this.CarregarGrid = function () {

        var data = new Array();

        $.each($this.Container.list, function (i, quantidadePorTipoDeCarga) {
            var quantidadePorTipoDeCargaGrid = new Object();

            quantidadePorTipoDeCargaGrid.Codigo = quantidadePorTipoDeCarga.Codigo.val;
            quantidadePorTipoDeCargaGrid.Tolerancia = quantidadePorTipoDeCarga.Tolerancia.val;
            quantidadePorTipoDeCargaGrid.ToleranciaCancelamentoAgendaConfirmada = quantidadePorTipoDeCarga.ToleranciaCancelamentoAgendaConfirmada.val;
            quantidadePorTipoDeCargaGrid.ToleranciaCancelamentoAgendaNaoConfirmada = quantidadePorTipoDeCarga.ToleranciaCancelamentoAgendaNaoConfirmada.val;
            quantidadePorTipoDeCargaGrid.Volumes = quantidadePorTipoDeCarga.Volumes.val;
            quantidadePorTipoDeCargaGrid.DescricaoTipoCarga = quantidadePorTipoDeCarga.DescricaoTipoCarga.val;
            quantidadePorTipoDeCargaGrid.TiposCarga = quantidadePorTipoDeCarga.TiposCarga.list;

            data.push(quantidadePorTipoDeCargaGrid);
        });

        $this.Grid.CarregarGrid(data);
    }

    this.Excluir = function (data) {
        for (var i = 0; i < $this.Container.list.length; i++) {
            if (data.Codigo == $this.Container.list[i].Codigo.val) {
                $this.Container.list.splice(i, 1);
                break;
            }
        }

        $this.CarregarGrid();
    }

    this.ExcluirTipoCarga = function (data) {
        var registros = $this.GridTiposDeCarga.BuscarRegistros();

        for (var i = 0; i < registros.length; i++) {
            if (data.Codigo == registros[i].Codigo) {
                registros.splice(i, 1);
                break;
            }
        }

        $this.GridTiposDeCarga.CarregarGrid(registros);
    }

    this.Adicionar = function () {
        var valido = ValidarCamposObrigatorios($this.QuantidadePorTipoDeCarga);

        if (!valido) {
            exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.CamposObrigatorios, Localization.Resources.Gerais.Geral.InformeCamposObrigatorios);
            return;
        }

        $this.QuantidadePorTipoDeCarga.Codigo.val(guid());

        var registro = SalvarListEntity($this.QuantidadePorTipoDeCarga);

        registro["TiposCarga"] = new PropertyEntity({ val: _quantidadePorTipoDeCarga.QuantidadePorTipoDeCarga.GridTiposDeCarga.val(), getType: _quantidadePorTipoDeCarga.QuantidadePorTipoDeCarga.GridTiposDeCarga.getType, list: $this.ObterQuantidadeTipoCargaTiposDeCargaSalvar(), type: types.listEntity });
        registro["DescricaoTipoCarga"] = new PropertyEntity({ val: $this.ObterDescricaoTiposCarga(), getType: typesKnockout.string });

        $this.Container.list.push(registro);

        $this.CarregarGrid();
        $this.LimparCampos();
        Global.ResetarAba("abasQuantidadeTipoDeCarga");
    }

    this.LimparCampos = function () {
        LimparCampos($this.QuantidadePorTipoDeCarga);
        $this.GridTiposDeCarga.CarregarGrid([]);
    }

    this.ObterDescricaoTiposCarga = function () {
        var descricoesTipoCarga = [];

        $this.GridTiposDeCarga.BuscarRegistros().forEach(function (reg) {
            descricoesTipoCarga.push(reg.Descricao);
        });

        return descricoesTipoCarga.join(', ');
    }
    
    this.ObterQuantidadeTipoCargaTiposDeCargaSalvar = function () {
        var listaTiposCarga = $this.GridTiposDeCarga.BuscarRegistros();
        var listaRetornar = new Array();

        listaTiposCarga.forEach(function (tipoCarga) {
            listaRetornar.push({
                Codigo: { val: tipoCarga.Codigo, getType: "int", type: "map" },
                Descricao: { val: tipoCarga.Descricao, getType: "string", type: "map" }
            });
        });
        
        return listaRetornar;
    }
}
