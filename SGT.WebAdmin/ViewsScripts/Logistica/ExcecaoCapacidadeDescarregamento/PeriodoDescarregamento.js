/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Globais.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="../../Consultas/CanalVenda.js" />
/// <reference path="../../Consultas/GrupoProduto.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var PeriodoDescarregamentoModel = function (instancia) {
    this.Grid = PropertyEntity({ type: types.local });

    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.HoraInicio = PropertyEntity({ val: ko.observable(""), def: "", getType: typesKnockout.time, text: Localization.Resources.Gerais.Geral.Inicio.getRequiredFieldDescription(), required: true });
    this.HoraTermino = PropertyEntity({ val: ko.observable(""), def: "", getType: typesKnockout.time, text: Localization.Resources.Gerais.Geral.Termino.getRequiredFieldDescription(), required: true });
    this.CapacidadeDescarregamento = PropertyEntity({ val: ko.observable(""), def: "", getType: typesKnockout.int, text: Localization.Resources.Logistica.ExcecaoCapacidadeDescarregamento.CapacidadeDescarregamentoKG.getFieldDescription(), required: false, visible: ko.observable(false), maxlength: 15 });
    this.CapacidadeDescarregamentoSimultaneo = PropertyEntity({ val: ko.observable(""), def: "", getType: typesKnockout.int, text: Localization.Resources.Logistica.ExcecaoCapacidadeDescarregamento.DescarregamentoSimultaneo.getRequiredFieldDescription(), required: true });
    this.CapacidadeDescarregamentoSimultaneoAdicional = PropertyEntity({ val: ko.observable(""), def: "", getType: typesKnockout.int, text: Localization.Resources.Logistica.ExcecaoCapacidadeDescarregamento.DescarregamentoSimultaneoAdicional.getFieldDescription() });
    this.ToleranciaExcessoTempo = PropertyEntity({ val: ko.observable(""), def: "", getType: typesKnockout.int, text: Localization.Resources.Logistica.ExcecaoCapacidadeDescarregamento.ExcessoTempo.getFieldDescription(), issue: 324 });
    this.CanaisVenda = PropertyEntity({ type: types.map, text: "Adicionar Canal de Venda", getType: typesKnockout.dynamic, val: ko.observable(new Array()), def: new Array(), idBtnSearch: guid(), idGrid: guid(), visible: ko.observable(false) });
    this.GruposProdutos = PropertyEntity({ type: types.map, text: "Adicionar Grupo de Produto", getType: typesKnockout.dynamic, val: ko.observable(new Array()), def: new Array(), idBtnSearch: guid(), idGrid: guid(), visible: ko.observable(false) });
    this.Remetentes = PropertyEntity({ type: types.map, text: "Adicionar Remetente", getType: typesKnockout.string, val: ko.observable(new Array()), def: new Array(), idBtnSearch: guid(), idGrid: guid(), visible: ko.observable(false) });
    this.TiposCarga = PropertyEntity({ type: types.map, text: "Adicionar Tipo de Carga", getType: typesKnockout.dynamic, val: ko.observable(new Array()), def: new Array(), idBtnSearch: guid(), idGrid: guid(), visible: ko.observable(false) });
    this.GruposPessoas = PropertyEntity({ type: types.map, text: "Adicionar Grupo de Pessoa", getType: typesKnockout.dynamic, val: ko.observable(new Array()), def: new Array(), idBtnSearch: guid(), idGrid: guid(), visible: ko.observable(false) });
    this.SkuDe = PropertyEntity({ val: ko.observable(""), def: "", getType: typesKnockout.int, text: "Qtd. de Itens De:", required: false });
    this.SkuAte = PropertyEntity({ val: ko.observable(""), def: "", getType: typesKnockout.int, text: "Qtd. de Itens Até:", required: false });

    this.Adicionar = PropertyEntity({ eventClick: instancia.Adicionar, type: types.event, text: Localization.Resources.Gerais.Geral.Adicionar, visible: ko.observable(true) });
    this.Atualizar = PropertyEntity({ eventClick: instancia.Atualizar, type: types.event, text: Localization.Resources.Gerais.Geral.Atualizar, visible: ko.observable(false) });
    this.Excluir = PropertyEntity({ eventClick: instancia.Excluir, type: types.event, text: Localization.Resources.Gerais.Geral.Excluir, visible: ko.observable(false) });
    this.Limpar = PropertyEntity({ eventClick: instancia.LimparCampos, type: types.event, text: Localization.Resources.Gerais.Geral.Limpar, visible: ko.observable(true) });
    this.ImportarDeOutroDia = PropertyEntity({ eventClick: instancia.ImportarDeOutroDia, type: types.event, text: Localization.Resources.Logistica.ExcecaoCapacidadeDescarregamento.ImportarDeOutroDia, visible: ko.observable(true) });
    this.ImportarParaOsDemaisDias = PropertyEntity({ eventClick: instancia.ImportarParaOsDemaisDias, type: types.event, text: Localization.Resources.Logistica.ExcecaoCapacidadeDescarregamento.ImportarParaDemaisDias, visible: ko.observable(true) });
}

var PeriodoDescarregamento = function (idKnockout, containerItens) {
    let $this = this;
    let gridPeriodoDescarregamentoCanaisVenda;
    let gridPeriodoDescarregamentoGruposProdutos
    let gridPeriodoDescarregamentoGruposPessoas;
    let gridPeriodoDescarregamentoRemetentes;
    let gridPeriodoDescarregamentoTiposDeCarga;

    $this.IdKnockout = idKnockout;
    $this.Container = containerItens;
    $this._isEditando = false;

    this.Load = function () {
        $this.PeriodoDescarregamento = new PeriodoDescarregamentoModel($this);
        KoBindings($this.PeriodoDescarregamento, $this.IdKnockout);

        let menuOpcoes = { tipo: TypeOptionMenu.link, tamanho: 7, opcoes: [{ descricao: Localization.Resources.Gerais.Geral.Editar, id: guid(), metodo: $this.Editar }] };

        let header = [
            { data: "Codigo", visible: false },
            { data: "HoraInicio", title: Localization.Resources.Gerais.Geral.Inicio, width: "15%" },
            { data: "HoraTermino", title: Localization.Resources.Gerais.Geral.Termino, width: "15%" },
            { data: "ResumoCanaisVenda", title: Localization.Resources.Logistica.ExcecaoCapacidadeDescarregamento.CanaisVenda, width: "15%" },
            { data: "ResumoTiposCarga", title: Localization.Resources.Gerais.Geral.TiposCarga, width: "15%" },
            { data: "ResumoRemetentes", title: Localization.Resources.Logistica.ExcecaoCapacidadeDescarregamento.Remetentes, width: "15%" },
            { data: "ResumoGrupoPessoas", title: Localization.Resources.Gerais.Geral.GrupoPessoas, width: "15%" },
            { data: "CapacidadeDescarregamentoSimultaneo", title: Localization.Resources.Logistica.ExcecaoCapacidadeDescarregamento.CarSimultaneo, width: "15%" },
            { data: "CapacidadeDescarregamentoSimultaneoAdicional", title: Localization.Resources.Logistica.ExcecaoCapacidadeDescarregamento.DescarregamentoSimultaneoAdicional, width: "15%" },
            { data: "ToleranciaExcessoTempo", title: Localization.Resources.Gerais.Geral.Tolerancia, width: "15%" },
            { data: "CapacidadeDescarregamento", title: Localization.Resources.Gerais.Geral.Capacidade, width: "15%" },
            { data: "SKU", title: Localization.Resources.Logistica.ExcecaoCapacidadeDescarregamento.QtdItens, width: "15%" },
        ];

        $this.Grid = new BasicDataTable($this.PeriodoDescarregamento.Grid.id, header, menuOpcoes, { column: 1, dir: orderDir.asc });
        $this.CarregarGrid();

        loadGridPeriodoDescarregamentoCanaisVenda();
        loadGridPeriodoDescarregamentoGruposProdutos();
        loadGridPeriodoDescarregamentoGruposPessoas();
        loadGridPeriodoDescarregamentoRemetente();
        loadGridPeriodoDescarregamentoTipoDeCarga();
    }

    this.SetIsEditando = function (isEditando) {
        $this._isEditando = isEditando;
    }

    this.GetIsEditando = function () {
        return $this._isEditando || false;
    }

    this.CarregarGrid = function () {

        let data = new Array();

        $.each($this.Container.list, function (i, periodoDescarregamento) {
            let periodoDescarregamentoGrid = new Object();

            periodoDescarregamentoGrid.Codigo = periodoDescarregamento.Codigo.val;
            periodoDescarregamentoGrid.HoraInicio = periodoDescarregamento.HoraInicio.val;
            periodoDescarregamentoGrid.HoraTermino = periodoDescarregamento.HoraTermino.val;
            periodoDescarregamentoGrid.CapacidadeDescarregamentoSimultaneo = periodoDescarregamento.CapacidadeDescarregamentoSimultaneo.val;
            periodoDescarregamentoGrid.CapacidadeDescarregamentoSimultaneoAdicional = periodoDescarregamento.CapacidadeDescarregamentoSimultaneoAdicional.val;
            periodoDescarregamentoGrid.ToleranciaExcessoTempo = (periodoDescarregamento.ToleranciaExcessoTempo.val || "0") + " min.";

            let sku = "";
            if (periodoDescarregamento.SkuDe.val) sku += Localization.Resources.Gerais.Geral.De + " " + periodoDescarregamento.SkuDe.val;
            if (periodoDescarregamento.SkuAte.val) sku += " " + Localization.Resources.Gerais.Geral.Ate + " " + periodoDescarregamento.SkuAte.val;

            periodoDescarregamentoGrid.SKU = sku;
            periodoDescarregamentoGrid.CapacidadeDescarregamento = periodoDescarregamento.CapacidadeDescarregamento.val;
            periodoDescarregamentoGrid.ResumoCanaisVenda = obterDescricaoCanaisVenda(periodoDescarregamento.CanaisVenda.list);
            periodoDescarregamentoGrid.ResumoGrupoPessoas = obterDescricaoGrupoPessoas(periodoDescarregamento.GruposPessoas.list);
            periodoDescarregamentoGrid.ResumoTiposCarga = obterDescricaoTiposCarga(periodoDescarregamento.TiposCarga.list);
            periodoDescarregamentoGrid.ResumoRemetentes = obterDescricaoRemetentes(periodoDescarregamento.Remetentes.list);

            data.push(periodoDescarregamentoGrid);
        });

        $this.Grid.CarregarGrid(data);
    }

    this.Editar = function (data) {
        for (let i = 0; i < $this.Container.list.length; i++) {
            let periodo = $this.Container.list[i];

            if (data.Codigo == periodo.Codigo.val) {
                $this.LimparCampos();
                $this.SetIsEditando(true);

                PreencherEditarListEntity($this.PeriodoDescarregamento, periodo);

                $this.PeriodoDescarregamento.SkuDe.val(periodo.SkuDe.val);
                $this.PeriodoDescarregamento.SkuAte.val(periodo.SkuAte.val);

                $this.PeriodoDescarregamento.Adicionar.visible(false);
                $this.PeriodoDescarregamento.Atualizar.visible(true);
                $this.PeriodoDescarregamento.Excluir.visible(true);

                recarregarGridPeriodoDescarregamentoCanaisVenda(periodo.CanaisVenda.list);
                recarregarGridPeriodoDescarregamentoGruposProdutos(periodo.GruposProdutos.list);
                recarregarGridPeriodoDescarregamentoGruposPessoas(periodo.GruposPessoas.list);
                recarregarGridPeriodoDescarregamentoRemetente(periodo.Remetentes.list);
                recarregarGridPeriodoDescarregamentoTipoDeCarga(periodo.TiposCarga.list);

                return;
            }
        }
    }

    this.Excluir = function (data) {
        for (let i = 0; i < $this.Container.list.length; i++) {
            if (data.Codigo.val() == $this.Container.list[i].Codigo.val) {
                $this.Container.list.splice(i, 1);
                break;
            }
        }

        $this.CarregarGrid();
        $this.LimparCampos();
    }

    this.ValidarDados = function () {
        let valido = ValidarCamposObrigatorios($this.PeriodoDescarregamento);

        let skuDe = parseInt($this.PeriodoDescarregamento.SkuDe.val()) || 0;
        let skuAte = parseInt($this.PeriodoDescarregamento.SkuAte.val());

        if (!isNaN(skuAte) && skuDe > skuAte) {
            exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.FaixaItens, "O valor de \"Qtd. de Itens De\" deve ser menor que \"Qtd. de Itens Até\".");
            return false;
        }

        if (!valido) {
            exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.CamposObrigatorios, Localization.Resources.Gerais.Geral.PorFavorInformeOsCamposObrigatorios);
            return false;
        }

        const horaInicio = moment($this.PeriodoDescarregamento.HoraInicio.val(), "HH:mm");
        const horaTermino = moment($this.PeriodoDescarregamento.HoraTermino.val(), "HH:mm");
        const codigo = $this.PeriodoDescarregamento.Codigo.val();

        for (let i = 0; i < $this.Container.list.length; i++) {
            const horaInicioGrid = moment($this.Container.list[i].HoraInicio.val, "HH:mm");
            const horaTerminoGrid = moment($this.Container.list[i].HoraTermino.val, "HH:mm");

            const horaInvalida = horaInicio.isBetween(horaInicioGrid, horaTerminoGrid) ||
                horaTermino.isBetween(horaInicioGrid, horaTerminoGrid) ||
                horaInicio.diff(horaInicioGrid) == 0 ||
                horaInicio.diff(horaTerminoGrid) == 0 ||
                horaTermino.diff(horaTerminoGrid) == 0 ||
                horaTermino.diff(horaInicioGrid) == 0;

            if (horaInvalida && codigo != $this.Container.list[i].Codigo.val) {
                exibirMensagem(tipoMensagem.aviso, Localization.Resources.Logistica.ExcecaoCapacidadeDescarregamento.PeriodoJaExiste, Localization.Resources.Logistica.ExcecaoCapacidadeDescarregamento.PeriodoEntrouEmConflito + " " + $this.Container.list[i].HoraInicio.val + " " + Localization.Resources.Logistica.ExcecaoCapacidadeDescarregamento.Ate + " " + $this.Container.list[i].HoraTermino.val + ".");
                return false;
            }
        }


        return true;
    }

    this.Atualizar = function () {
        if (!$this.ValidarDados())
            return;

        for (let i = 0; i < $this.Container.list.length; i++) {
            if ($this.PeriodoDescarregamento.Codigo.val() == $this.Container.list[i].Codigo.val) {
                $this.Container.list[i] = obterPeriodoSalvar();
                break;
            }
        }

        $this.CarregarGrid();
        $this.LimparCampos();
    }

    this.Adicionar = function () {
        if (!$this.ValidarDados())
            return;

        $this.PeriodoDescarregamento.Codigo.val(guid());
        $this.Container.list.push(obterPeriodoSalvar());

        $this.CarregarGrid();
        $this.LimparCampos();
    }

    this.LimparCampos = function () {
        LimparCampos($this.PeriodoDescarregamento);
        $this.SetIsEditando(false);

        $this.PeriodoDescarregamento.Adicionar.visible(true);
        $this.PeriodoDescarregamento.Atualizar.visible(false);
        $this.PeriodoDescarregamento.Excluir.visible(false);

        limparGridPeriodoDescarregamentoCanaisVenda();
        limparGridPeriodoDescarregamentoGruposProdutos();
        limparGridPeriodoDescarregamentoGruposPessoas();
        limparGridPeriodoDescarregamentoTipoDeCarga();
        limparGridPeriodoDescarregamentoRemetente();
    }

    this.ControlarExibicaoCapacidadeDescarregamento = function (exibir) {
        $this.PeriodoDescarregamento.CapacidadeDescarregamento.visible(exibir);
        $this.Grid.ControlarExibicaoColuna("CapacidadeDescarregamento", exibir);

        if (!exibir)
            $this.PeriodoDescarregamento.CapacidadeDescarregamento.val("");
    }

    const limparGridPeriodoDescarregamentoCanaisVenda = function () {
        gridPeriodoDescarregamentoCanaisVenda.CarregarGrid([]);
    }

    const limparGridPeriodoDescarregamentoGruposProdutos = function () {
        gridPeriodoDescarregamentoGruposProdutos.CarregarGrid([]);
    }

    const limparGridPeriodoDescarregamentoGruposPessoas = function () {
        gridPeriodoDescarregamentoGruposPessoas.CarregarGrid([]);
    }

    const limparGridPeriodoDescarregamentoRemetente = function () {
        gridPeriodoDescarregamentoRemetentes.CarregarGrid([]);
    }

    const limparGridPeriodoDescarregamentoTipoDeCarga = function () {
        gridPeriodoDescarregamentoTiposDeCarga.CarregarGrid([]);
    }

    const loadGridPeriodoDescarregamentoCanaisVenda = function () {
        const linhasPorPaginas = 5;
        const opcaoRemover = { descricao: "Remover", id: guid(), metodo: removerPeriodoDescarregamentoCanaisVenda, icone: "" };
        const menuOpcoes = { tipo: TypeOptionMenu.link, tamanho: 21, opcoes: [opcaoRemover] };

        const header = [
            { data: "Codigo", visible: false },
            { data: "Descricao", title: "Descrição", width: "75%", className: "text-align-left" }
        ];

        gridPeriodoDescarregamentoCanaisVenda = new BasicDataTable($this.PeriodoDescarregamento.CanaisVenda.idGrid, header, menuOpcoes, null, null, linhasPorPaginas);

        BuscarCanaisVenda($this.PeriodoDescarregamento.CanaisVenda, null, gridPeriodoDescarregamentoCanaisVenda);

        gridPeriodoDescarregamentoCanaisVenda.CarregarGrid([]);
    }
    const loadGridPeriodoDescarregamentoGruposProdutos = function () {
        const linhasPorPaginas = 5;
        const opcaoRemover = { descricao: "Remover", id: guid(), metodo: removerPeriodoDescarregamentoGrupoProduto, icone: "" };
        const menuOpcoes = { tipo: TypeOptionMenu.link, tamanho: 21, opcoes: [opcaoRemover] };

        const header = [
            { data: "Codigo", visible: false },
            { data: "Descricao", title: "Descrição", width: "75%", className: "text-align-left" }
        ];

       gridPeriodoDescarregamentoGruposProdutos = new BasicDataTable($this.PeriodoDescarregamento.GruposProdutos.idGrid, header, menuOpcoes, null, null, linhasPorPaginas);

        BuscarGruposProdutos($this.PeriodoDescarregamento.GruposProdutos, null, gridPeriodoDescarregamentoGruposProdutos);

        gridPeriodoDescarregamentoGruposProdutos.CarregarGrid([]);
    }

    const loadGridPeriodoDescarregamentoGruposPessoas = function () {
        const linhasPorPaginas = 5;
        const opcaoRemover = { descricao: Localization.Resources.Gerais.Geral.Remover, id: guid(), metodo: removerPeriodoDescarregamentoGruposPessoas, icone: "" };
        const menuOpcoes = { tipo: TypeOptionMenu.link, tamanho: 21, opcoes: [opcaoRemover] };

        const header = [
            { data: "Codigo", visible: false },
            { data: "Descricao", title: Localization.Resources.Gerais.Geral.Descricao, width: "75%", className: "text-align-left" }
        ];

        gridPeriodoDescarregamentoGruposPessoas = new BasicDataTable($this.PeriodoDescarregamento.GruposPessoas.idGrid, header, menuOpcoes, null, null, linhasPorPaginas);

        BuscarGruposPessoas($this.PeriodoDescarregamento.GruposPessoas, null, null, gridPeriodoDescarregamentoGruposPessoas);

        gridPeriodoDescarregamentoGruposPessoas.CarregarGrid([]);
    }

    const loadGridPeriodoDescarregamentoRemetente = function () {
        const linhasPorPaginas = 5;
        const opcaoRemover = { descricao: Localization.Resources.Gerais.Geral.Remover, id: guid(), metodo: removerPeriodoDescarregamentoRemetente, icone: "" };
        const menuOpcoes = { tipo: TypeOptionMenu.link, tamanho: 21, opcoes: [opcaoRemover] };

        const header = [
            { data: "Codigo", visible: false },
            { data: "Descricao", title: Localization.Resources.Gerais.Geral.Descricao, width: "75%", className: "text-align-left" }
        ];

        gridPeriodoDescarregamentoRemetentes = new BasicDataTable($this.PeriodoDescarregamento.Remetentes.idGrid, header, menuOpcoes, null, null, linhasPorPaginas);

        BuscarClientes($this.PeriodoDescarregamento.Remetentes, null, null, null, null, gridPeriodoDescarregamentoRemetentes);

        gridPeriodoDescarregamentoRemetentes.CarregarGrid([]);
    }

    const loadGridPeriodoDescarregamentoTipoDeCarga = function () {
        const linhasPorPaginas = 5;
        const opcaoRemover = { descricao: Localization.Resources.Gerais.Geral.Remover, id: guid(), metodo: removerPeriodoDescarregamentoTipoDeCarga, icone: "" };
        const menuOpcoes = { tipo: TypeOptionMenu.link, tamanho: 21, opcoes: [opcaoRemover] };

        const header = [
            { data: "Codigo", visible: false },
            { data: "Descricao", title: Localization.Resources.Gerais.Geral.Descricao, width: "75%", className: "text-align-left" }
        ];

        gridPeriodoDescarregamentoTiposDeCarga = new BasicDataTable($this.PeriodoDescarregamento.TiposCarga.idGrid, header, menuOpcoes, null, null, linhasPorPaginas);

        BuscarTiposdeCarga($this.PeriodoDescarregamento.TiposCarga, null, null, gridPeriodoDescarregamentoTiposDeCarga);

        gridPeriodoDescarregamentoTiposDeCarga.CarregarGrid([]);
    }

    var obterDescricaoCanaisVenda = function (canaisVenda) {
        let lista = "";
        for (var i = 0; i < canaisVenda.length; i++) {
            if (lista != "")
                lista = lista + ", ";

            lista = lista + canaisVenda[i].Descricao.val;
        }

        return lista;
    }

    const obterDescricaoTiposCarga = function (tiposCarga) {
        let lista = "";
        for (var i = 0; i < tiposCarga.length; i++) {
            if (lista != "")
                lista = lista + ", ";

            lista = lista + tiposCarga[i].Descricao.val;
        }

        return lista;
    }

    const obterDescricaoGrupoPessoas = function (grupoPessoas) {
        let lista = "";
        for (var i = 0; i < grupoPessoas.length; i++) {
            if (lista != "")
                lista = lista + ", ";

            lista = lista + grupoPessoas[i].Descricao.val;
        }

        return lista;
    }

    const obterDescricaoRemetentes = function (remetentes) {
        let lista = "";
        for (var i = 0; i < remetentes.length; i++) {
            if (lista != "")
                lista = lista + ", ";

            lista = lista + remetentes[i].Descricao.val;
        }

        return lista;
    }

    const obterListaPeriodoDescarregamentoCanaisVenda = function () {
        return gridPeriodoDescarregamentoCanaisVenda.BuscarRegistros();
    }

    const obterListaPeriodoDescarregamentoGruposProdutos = function () {
        return gridPeriodoDescarregamentoGruposProdutos.BuscarRegistros();
    }

    const obterListaPeriodoDescarregamentoTiposDeCarga = function () {
        return gridPeriodoDescarregamentoTiposDeCarga.BuscarRegistros();
    }

    const obterListaPeriodoDescarregamentoGruposPessoas = function () {
        return gridPeriodoDescarregamentoGruposPessoas.BuscarRegistros();
    }
    const obterListaPeriodoDescarregamentoRemetentes = function () {
        return gridPeriodoDescarregamentoRemetentes.BuscarRegistros();
    }   

    const obterPeriodoDescarregamentoCanaisVendaSalvar = function () {
        const listaCanaisVenda = obterListaPeriodoDescarregamentoCanaisVenda();
        let listaRetornar = new Array();

        listaCanaisVenda.forEach(function (canalVenda) {
            listaRetornar.push({
                Codigo: { val: canalVenda.Codigo, getType: "int", type: "map" },
                Descricao: { val: canalVenda.Descricao, getType: "string", type: "map" }
            });
        });

        return listaRetornar;
    }

    const obterPeriodoDescarregamentoGruposProdutosSalvar = function () {
        const listaGruposProdutos = obterListaPeriodoDescarregamentoGruposProdutos();
        let listaRetornar = new Array();

        listaGruposProdutos.forEach(function (grupoProduto) {
            listaRetornar.push({
                Codigo: { val: grupoProduto.Codigo, getType: "int", type: "map" },
                Descricao: { val: grupoProduto.Descricao, getType: "string", type: "map" }
            });
        });

        return listaRetornar;
    }

    const obterPeriodoDescarregamentoRemetentesSalvar = function () {
        const listaRemetentes = obterListaPeriodoDescarregamentoRemetentes();
        let listaRetornar = new Array();

        listaRemetentes.forEach(function (remetente) {
            listaRetornar.push({
                Codigo: { val: remetente.Codigo, getType: "double", type: "map" },
                Descricao: { val: remetente.Descricao, getType: "string", type: "map" }
            });
        });

        return listaRetornar;
    }

    const obterPeriodoDescarregamentoTiposDeCargaSalvar = function () {
        const listaTiposCarga = obterListaPeriodoDescarregamentoTiposDeCarga();
        let listaRetornar = new Array();

        listaTiposCarga.forEach(function (tipoCarga) {
            listaRetornar.push({
                Codigo: { val: tipoCarga.Codigo, getType: "int", type: "map" },
                Descricao: { val: tipoCarga.Descricao, getType: "string", type: "map" }
            });
        });

        return listaRetornar;
    }

    const obterPeriodoDescarregamentoGruposPessoasSalvar = function () {
        const listaGruposPessoas = obterListaPeriodoDescarregamentoGruposPessoas();
        let listaRetornar = new Array();

        listaGruposPessoas.forEach(function (grupoPessoa) {
            listaRetornar.push({
                Codigo: { val: grupoPessoa.Codigo, getType: "int", type: "map" },
                Descricao: { val: grupoPessoa.Descricao, getType: "string", type: "map" }
            });
        });

        return listaRetornar;
    }

    const obterPeriodoSalvar = function () {
        let periodo = SalvarListEntity($this.PeriodoDescarregamento);;

        periodo["CanaisVenda"] = new PropertyEntity({ val: $this.PeriodoDescarregamento.CanaisVenda.val(), getType: $this.PeriodoDescarregamento.CanaisVenda.getType, list: obterPeriodoDescarregamentoCanaisVendaSalvar(), type: types.listEntity });
        periodo["GruposProdutos"] = new PropertyEntity({ val: $this.PeriodoDescarregamento.GruposProdutos.val(), getType: $this.PeriodoDescarregamento.GruposProdutos.getType, list: obterPeriodoDescarregamentoGruposProdutosSalvar(), type: types.listEntity });
        periodo["Remetentes"] = new PropertyEntity({ val: $this.PeriodoDescarregamento.Remetentes.val(), getType: $this.PeriodoDescarregamento.Remetentes.getType, list: obterPeriodoDescarregamentoRemetentesSalvar(), type: types.listEntity });
        periodo["TiposCarga"] = new PropertyEntity({ val: $this.PeriodoDescarregamento.TiposCarga.val(), getType: $this.PeriodoDescarregamento.TiposCarga.getType, list: obterPeriodoDescarregamentoTiposDeCargaSalvar(), type: types.listEntity });
        periodo["GruposPessoas"] = new PropertyEntity({ val: $this.PeriodoDescarregamento.GruposPessoas.val(), getType: $this.PeriodoDescarregamento.GruposPessoas.getType, list: obterPeriodoDescarregamentoGruposPessoasSalvar(), type: types.listEntity });

        return periodo;
    }
        
    const recarregarGridPeriodoDescarregamentoCanaisVenda = function (canaisVenda) {

        let dados = new Array();

        $.each(canaisVenda, function (i, canalVenda) {
            dados.push({
                Codigo: canalVenda.Codigo.val,
                Descricao: canalVenda.Descricao.val
            });
        });

        gridPeriodoDescarregamentoCanaisVenda.CarregarGrid(dados);
    }

    const recarregarGridPeriodoDescarregamentoGruposProdutos = function (gruposProdutos) {

        let dados = new Array();

        $.each(gruposProdutos, function (i, grupoProduto) {
            dados.push({
                Codigo: grupoProduto.Codigo.val,
                Descricao: grupoProduto.Descricao.val
            });
        });

        gridPeriodoDescarregamentoGruposProdutos.CarregarGrid(dados);
    }
    const recarregarGridPeriodoDescarregamentoGruposPessoas = function (gruposPessoas) {

        let dados = new Array();

        $.each(gruposPessoas, function (i, grupoPessoa) {
            dados.push({
                Codigo: grupoPessoa.Codigo.val,
                Descricao: grupoPessoa.Descricao.val
            });
        });

        gridPeriodoDescarregamentoGruposPessoas.CarregarGrid(dados);
    }

    const recarregarGridPeriodoDescarregamentoRemetente = function (remetentes) {
        let dados = new Array();

        $.each(remetentes, function (i, remetente) {
            dados.push({
                Codigo: remetente.Codigo.val,
                Descricao: remetente.Descricao.val
            });
        });

        gridPeriodoDescarregamentoRemetentes.CarregarGrid(dados);
    }

    const recarregarGridPeriodoDescarregamentoTipoDeCarga = function (tiposCarga) {

        let dados = new Array();

        $.each(tiposCarga, function (i, tipoCarga) {
            dados.push({
                Codigo: tipoCarga.Codigo.val,
                Descricao: tipoCarga.Descricao.val
            });
        });

        gridPeriodoDescarregamentoTiposDeCarga.CarregarGrid(dados);
    }

    const removerPeriodoDescarregamentoCanaisVenda = function (registroSelecionado) {
        let lista = obterListaPeriodoDescarregamentoCanaisVenda();

        for (let i = 0; i < lista.length; i++) {
            if (registroSelecionado.Codigo == lista[i].Codigo) {
                lista.splice(i, 1);
                break;
            }
        }

        gridPeriodoDescarregamentoCanaisVenda.CarregarGrid(lista);
    }

    const removerPeriodoDescarregamentoGrupoProduto = function (registroSelecionado) {
        let lista = obterListaPeriodoDescarregamentoGruposProdutos();

        for (let i = 0; i < lista.length; i++) {
            if (registroSelecionado.Codigo == lista[i].Codigo) {
                lista.splice(i, 1);
                break;
            }
        }

        gridPeriodoDescarregamentoGruposProdutos.CarregarGrid(lista);
    }

    const removerPeriodoDescarregamentoGruposPessoas = function (registroSelecionado) {
        let lista = obterListaPeriodoDescarregamentoGruposPessoas();

        for (let i = 0; i < lista.length; i++) {
            if (registroSelecionado.Codigo == lista[i].Codigo) {
                lista.splice(i, 1);
                break;
            }
        }

        gridPeriodoDescarregamentoGruposPessoas.CarregarGrid(lista);
    }

    const removerPeriodoDescarregamentoRemetente = function (registroSelecionado) {
        let lista = obterListaPeriodoDescarregamentoRemetentes();

        for (let i = 0; i < lista.length; i++) {
            if (registroSelecionado.Codigo == lista[i].Codigo) {
                lista.splice(i, 1);
                break;
            }
        }

        gridPeriodoDescarregamentoRemetentes.CarregarGrid(lista);
    }

    const removerPeriodoDescarregamentoTipoDeCarga = function (registroSelecionado) {
        let lista = obterListaPeriodoDescarregamentoTiposDeCarga();

        for (let i = 0; i < lista.length; i++) {
            if (registroSelecionado.Codigo == lista[i].Codigo) {
                lista.splice(i, 1);
                break;
            }
        }

        gridPeriodoDescarregamentoTiposDeCarga.CarregarGrid(lista);
    }

}
