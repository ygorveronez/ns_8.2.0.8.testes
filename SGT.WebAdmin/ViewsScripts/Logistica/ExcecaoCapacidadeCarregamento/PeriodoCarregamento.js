/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Globais.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Rest.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var PeriodoCarregamentoModel = function (instancia) {
    this.Grid = PropertyEntity({ type: types.local });

    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.HoraInicio = PropertyEntity({ val: ko.observable(""), def: "", getType: typesKnockout.time, text: Localization.Resources.Gerais.Geral.Inicio.getRequiredFieldDescription(), required: true });
    this.HoraTermino = PropertyEntity({ val: ko.observable(""), def: "", getType: typesKnockout.time, text: Localization.Resources.Logistica.ExcecaoCapacidadeCarregamento.Termino.getRequiredFieldDescription(), required: true });
    this.CapacidadeCarregamento = PropertyEntity({ val: ko.observable(""), def: "", getType: typesKnockout.int, text: Localization.Resources.Logistica.ExcecaoCapacidadeCarregamento.CapacidadedeCarregamentoKG.getFieldDescription(), required: false, visible: ko.observable(false), maxlength: 15 });
    this.CapacidadeCarregamentoSimultaneo = PropertyEntity({ val: ko.observable(""), def: "", getType: typesKnockout.int, text: Localization.Resources.Logistica.ExcecaoCapacidadeCarregamento.CarregamentoSimultaneo.getRequiredFieldDescription(), required: true });
    this.ToleranciaExcessoTempo = PropertyEntity({ val: ko.observable(""), def: "", getType: typesKnockout.int, text: Localization.Resources.Logistica.ExcecaoCapacidadeCarregamento.ExcessoTempo.getFieldDescription(), issue: 324 });

    this.ContainerTipoOperacaoSimultaneo = PropertyEntity({ type: types.listEntity, list: new Array(), idGrid: guid(), codEntity: ko.observable(0) });
    this.GridTipoOperacaoSimultaneo = PropertyEntity({ type: types.local });
    this.TipoOperacaoSimultaneo = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Logistica.ExcecaoCapacidadeCarregamento.TipoOperacao.getRequiredFieldDescription(), idBtnSearch: guid() });
    this.CapacidadeSimultaneoTipoOperacao = PropertyEntity({ val: ko.observable(""), def: "", getType: typesKnockout.int, text: Localization.Resources.Logistica.ExcecaoCapacidadeCarregamento.CarregamentoSimultaneo.getRequiredFieldDescription() });
    this.AdicionarTipoOperacaoSimultaneo = PropertyEntity({ eventClick: instancia.AdicionarTipoOperacaoSimultaneo, type: types.event, text: Localization.Resources.Gerais.Geral.Adicionar, visible: ko.observable(true) });

    this.Adicionar = PropertyEntity({ eventClick: instancia.Adicionar, type: types.event, text: Localization.Resources.Gerais.Geral.Adicionar, visible: ko.observable(true) });
    this.Atualizar = PropertyEntity({ eventClick: instancia.Atualizar, type: types.event, text: Localization.Resources.Gerais.Geral.Atualizar, visible: ko.observable(false) });
    this.Excluir = PropertyEntity({ eventClick: instancia.Excluir, type: types.event, text: Localization.Resources.Gerais.Geral.Excluir, visible: ko.observable(false) });
    this.Limpar = PropertyEntity({ eventClick: instancia.LimparCampos, type: types.event, text: Localization.Resources.Logistica.ExcecaoCapacidadeCarregamento.Limpar, visible: ko.observable(true) });
    this.ImportarDeOutroDia = PropertyEntity({ eventClick: instancia.ImportarDeOutroDia, type: types.event, text: Localization.Resources.Logistica.ExcecaoCapacidadeCarregamento.ImportarOutroDia, visible: ko.observable(true) });
    this.ImportarParaOsDemaisDias = PropertyEntity({ eventClick: instancia.ImportarParaOsDemaisDias, type: types.event, text: Localization.Resources.Logistica.ExcecaoCapacidadeCarregamento.ImportarParaDemaisDias, visible: ko.observable(true) });
}

var PeriodoCarregamento = function (idKnockout, containerItens) {
    var $this = this;

    $this.IdKnockout = idKnockout;
    $this.Container = containerItens;
    $this._isEditando = false;

    this.Load = function () {
        $this.PeriodoCarregamento = new PeriodoCarregamentoModel($this);
        KoBindings($this.PeriodoCarregamento, $this.IdKnockout);

        new BuscarTiposOperacao($this.PeriodoCarregamento.TipoOperacaoSimultaneo);

        $this.LoadGrid();
        $this.LoadGridTipoOperacaoSimultaneo();
    }

    this.LoadGrid = function () {
        var menuOpcoes = { tipo: TypeOptionMenu.link, tamanho: 7, opcoes: [{ descricao: Localization.Resources.Gerais.Geral.Editar, id: guid(), metodo: $this.Editar }] };

        var header = [
            { data: "Codigo", visible: false },
            { data: "HoraInicio", title: Localization.Resources.Gerais.Geral.Inicio, width: "15%" },
            { data: "HoraTermino", title: Localization.Resources.Logistica.ExcecaoCapacidadeCarregamento.Termino, width: "15%" },
            { data: "CapacidadeCarregamentoSimultaneo", title: Localization.Resources.Logistica.ExcecaoCapacidadeCarregamento.CarSimultaneo, width: "15%" },
            { data: "ToleranciaExcessoTempo", title: Localization.Resources.Logistica.ExcecaoCapacidadeCarregamento.Tolerancia, width: "15%" },
            { data: "CapacidadeCarregamento", title: Localization.Resources.Logistica.ExcecaoCapacidadeCarregamento.Capacidade, width: "15%", visible: false }
        ];

        $this.Grid = new BasicDataTable($this.PeriodoCarregamento.Grid.id, header, menuOpcoes, { column: 1, dir: orderDir.asc });

        $this.RecarregarGrid();
    }

    this.LoadGridTipoOperacaoSimultaneo = function () {
        var menuOpcoes = { tipo: TypeOptionMenu.link, tamanho: 7, opcoes: [{ descricao: Localization.Resources.Gerais.Geral.Excluir, id: guid(), metodo: $this.ExcluirTipoOperacaoSimultaneo }] };

        var header = [
            { data: "Codigo", visible: false },
            { data: "TipoOperacao", title: Localization.Resources.Logistica.ExcecaoCapacidadeCarregamento.TipoOperacao, width: "30%" },
            { data: "CapacidadeSimultaneo", title: Localization.Resources.Logistica.ExcecaoCapacidadeCarregamento.Capacidade, width: "15%" }
        ];

        $this.GridTipoOperacaoSimultaneo = new BasicDataTable($this.PeriodoCarregamento.GridTipoOperacaoSimultaneo.id, header, menuOpcoes, { column: 2, dir: orderDir.desc });

        $this.RecarregarGridTipoOperacaoSimultaneo();
    }

    this.SetIsEditando = function (isEditando) {
        $this._isEditando = isEditando;
    }

    this.GetIsEditando = function () {
        return $this._isEditando || false;
    }

    this.RecarregarGrid = function () {

        var data = new Array();

        $.each($this.Container.list, function (i, periodoCarregamento) {
            var periodoCarregamentoGrid = new Object();

            periodoCarregamentoGrid.Codigo = periodoCarregamento.Codigo.val;
            periodoCarregamentoGrid.HoraInicio = periodoCarregamento.HoraInicio.val;
            periodoCarregamentoGrid.HoraTermino = periodoCarregamento.HoraTermino.val;
            periodoCarregamentoGrid.CapacidadeCarregamentoSimultaneo = periodoCarregamento.CapacidadeCarregamentoSimultaneo.val;
            periodoCarregamentoGrid.ToleranciaExcessoTempo = (periodoCarregamento.ToleranciaExcessoTempo.val || "0") + " min.";
            periodoCarregamentoGrid.CapacidadeCarregamento = periodoCarregamento.CapacidadeCarregamentoVolume.val;

            data.push(periodoCarregamentoGrid);
        });

        $this.Grid.CarregarGrid(data);
    }

    this.RecarregarGridTipoOperacaoSimultaneo = function () {
        var data = new Array();

        $.each($this.PeriodoCarregamento.ContainerTipoOperacaoSimultaneo.list, function (i, tipoOperacao) {
            data.push({
                Codigo: tipoOperacao.TipoOperacaoSimultaneo.codEntity,
                TipoOperacao: tipoOperacao.TipoOperacaoSimultaneo.val,
                CapacidadeSimultaneo: tipoOperacao.CapacidadeSimultaneoTipoOperacao.val,
            });
        });

        $this.GridTipoOperacaoSimultaneo.CarregarGrid(data);
    }

    this.Editar = function (data) {
        for (var i = 0; i < $this.Container.list.length; i++) {
            if (data.Codigo == $this.Container.list[i].Codigo.val) {
                $this.LimparCampos();
                $this.SetIsEditando(true);

                var data = $this.Container.list[i];

                PreencherEditarListEntity($this.PeriodoCarregamento, data);
                $this.PeriodoCarregamento.ContainerTipoOperacaoSimultaneo.list = data.ContainerTipoOperacaoSimultaneo.list.slice();
                $this.RecarregarGridTipoOperacaoSimultaneo();

                $this.PeriodoCarregamento.Adicionar.visible(false);
                $this.PeriodoCarregamento.Atualizar.visible(true);
                $this.PeriodoCarregamento.Excluir.visible(true);

                return;
            }
        }
    }

    this.Excluir = function () {
        for (var i = 0; i < $this.Container.list.length; i++) {
            if ($this.PeriodoCarregamento.Codigo.val() == $this.Container.list[i].Codigo.val) {
                $this.Container.list.splice(i, 1);
                break;
            }
        }

        $this.RecarregarGrid();
        $this.LimparCampos();
    }

    this.ExcluirTipoOperacaoSimultaneo = function (data) {
        for (var i = 0; i < $this.PeriodoCarregamento.ContainerTipoOperacaoSimultaneo.list.length; i++) {
            if (data.Codigo == $this.PeriodoCarregamento.ContainerTipoOperacaoSimultaneo.list[i].TipoOperacaoSimultaneo.codEntity) {
                $this.PeriodoCarregamento.ContainerTipoOperacaoSimultaneo.list.splice(i, 1);
                break;
            }
        }

        $this.RecarregarGridTipoOperacaoSimultaneo();
    }

    this.ObterSomaCapacidades = function () {
        var somaCapacidade = 0;

        for (var i = 0; i < $this.PeriodoCarregamento.ContainerTipoOperacaoSimultaneo.list.length; i++) {
            somaCapacidade += parseInt($this.PeriodoCarregamento.ContainerTipoOperacaoSimultaneo.list[i].CapacidadeSimultaneoTipoOperacao.val) || 0
        }

        return somaCapacidade;
    }

    this.ValidarDados = function () {
        var valido = ValidarCamposObrigatorios($this.PeriodoCarregamento);

        if (!valido) {
            exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.CamposObrigatorios, "Informe os campos obrigatórios!");
            return false;
        }

        var horaInicio = moment($this.PeriodoCarregamento.HoraInicio.val(), "HH:mm");
        var horaTermino = moment($this.PeriodoCarregamento.HoraTermino.val(), "HH:mm");
        var codigo = $this.PeriodoCarregamento.Codigo.val();

        for (var i = 0; i < $this.Container.list.length; i++) {
            var horaInicioGrid = moment($this.Container.list[i].HoraInicio.val, "HH:mm");
            var horaTerminoGrid = moment($this.Container.list[i].HoraTermino.val, "HH:mm");

            var horaInvalida = horaInicio.isBetween(horaInicioGrid, horaTerminoGrid) ||
                horaTermino.isBetween(horaInicioGrid, horaTerminoGrid) ||
                horaInicio.diff(horaInicioGrid) == 0 ||
                horaInicio.diff(horaTerminoGrid) == 0 ||
                horaTermino.diff(horaTerminoGrid) == 0 ||
                horaTermino.diff(horaInicioGrid) == 0;

            if (horaInvalida && codigo != $this.Container.list[i].Codigo.val) {
                exibirMensagem(tipoMensagem.aviso, Localization.Resources.Logistica.ExcecaoCapacidadeCarregamento.PeriodoExiste, "O período entrou em conflito com um período já existente: de " + $this.Container.list[i].HoraInicio.val + " até " + $this.Container.list[i].HoraTermino.val + ".");
                return false;
            }
        }

        var somaCapacidade = $this.ObterSomaCapacidades();
        var capacidadeSimultanea = parseInt($this.PeriodoCarregamento.CapacidadeCarregamentoSimultaneo.val()) || 0;

        if (somaCapacidade > capacidadeSimultanea) {
            exibirMensagem(tipoMensagem.aviso, Localization.Resources.Logistica.ExcecaoCapacidadeCarregamento.QuantidadeSimultaneaInvalida, "A soma das capacidades (" + somaCapacidade + ") é superior ao carregamento simultâneo (" + $this.PeriodoCarregamento.CapacidadeCarregamentoSimultaneo.val() + ").");
            return false;
        }

        return true;
    }

    this.Atualizar = function () {
        if (!$this.ValidarDados())
            return;

        for (var i = 0; i < $this.Container.list.length; i++) {
            if ($this.PeriodoCarregamento.Codigo.val() == $this.Container.list[i].Codigo.val) {
                $this.Container.list[i] = SalvarListEntity($this.PeriodoCarregamento);
                break;
            }
        }

        $this.RecarregarGrid();
        $this.LimparCampos();
    }

    this.Adicionar = function () {
        if (!$this.ValidarDados())
            return;

        $this.PeriodoCarregamento.Codigo.val(guid());
        $this.Container.list.push(SalvarListEntity($this.PeriodoCarregamento));

        $this.RecarregarGrid();
        $this.LimparCampos();
    }

    this.AdicionarTipoOperacaoSimultaneo = function () {
        if ($this.PeriodoCarregamento.TipoOperacaoSimultaneo.codEntity() == 0 || $this.PeriodoCarregamento.CapacidadeSimultaneoTipoOperacao.val() == 0) {
            exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.CamposObrigatorios, Localization.Resources.Gerais.Geral.InformeCamposObrigatorios);
            return;
        }

        var codigoTipoOperacao = $this.PeriodoCarregamento.TipoOperacaoSimultaneo.codEntity();
        for (var i = 0; i < $this.PeriodoCarregamento.ContainerTipoOperacaoSimultaneo.list.length; i++) {
            if ($this.PeriodoCarregamento.ContainerTipoOperacaoSimultaneo.list[i].TipoOperacaoSimultaneo.codEntity == codigoTipoOperacao) {
                exibirMensagem(tipoMensagem.aviso, Localization.Resources.Logistica.ExcecaoCapacidadeCarregamento.TipoOperacaoExiste, "Já existe uma quantidade informada com o tipo de operação " + $this.PeriodoCarregamento.TipoOperacaoSimultaneo.val() + ".");
                return;
            }
        }

        var somaCapacidade = $this.ObterSomaCapacidades() + (parseInt($this.PeriodoCarregamento.CapacidadeSimultaneoTipoOperacao.val()) || 0);
        var capacidadeSimultanea = parseInt($this.PeriodoCarregamento.CapacidadeCarregamentoSimultaneo.val()) || 0;

        if (somaCapacidade > capacidadeSimultanea) {
            exibirMensagem(tipoMensagem.aviso, Localization.Resources.Logistica.ExcecaoCapacidadeCarregamento.QuantidadeSimultaneaInvalida, "A soma das capacidades (" + somaCapacidade + ") é superior ao carregamento simultâneo (" + $this.PeriodoCarregamento.CapacidadeCarregamentoSimultaneo.val() + ").");
            return;
        }

        var data = SalvarListEntity({
            TipoOperacaoSimultaneo: $this.PeriodoCarregamento.TipoOperacaoSimultaneo,
            CapacidadeSimultaneoTipoOperacao: $this.PeriodoCarregamento.CapacidadeSimultaneoTipoOperacao,
        });

        $this.PeriodoCarregamento.ContainerTipoOperacaoSimultaneo.list.push(data);

        $this.RecarregarGridTipoOperacaoSimultaneo();
        $this.LimparCamposTipoOperacaoSimultaneo();
    }

    this.LimparCampos = function () {
        LimparCampos($this.PeriodoCarregamento);
        $this.SetIsEditando(false);

        $this.PeriodoCarregamento.Adicionar.visible(true);
        $this.PeriodoCarregamento.Atualizar.visible(false);
        $this.PeriodoCarregamento.Excluir.visible(false);

        $this.PeriodoCarregamento.ContainerTipoOperacaoSimultaneo.list = [];
        $this.RecarregarGridTipoOperacaoSimultaneo();
    }

    this.LimparCamposTipoOperacaoSimultaneo = function () {
        $this.PeriodoCarregamento.TipoOperacaoSimultaneo.codEntity($this.PeriodoCarregamento.TipoOperacaoSimultaneo.defCodEntity);
        $this.PeriodoCarregamento.TipoOperacaoSimultaneo.val($this.PeriodoCarregamento.TipoOperacaoSimultaneo.def);
        $this.PeriodoCarregamento.CapacidadeSimultaneoTipoOperacao.val($this.PeriodoCarregamento.CapacidadeSimultaneoTipoOperacao.def);
    }

    this.ControlarExibicaoCapacidadeCarregamento = function (exibir) {
        $this.PeriodoCarregamento.CapacidadeCarregamento.visible(exibir);
        $this.Grid.ControlarExibicaoColuna("CapacidadeCarregamento", exibir);

        if (!exibir)
            $this.PeriodoCarregamento.CapacidadeCarregamento.val("");
    }
}
