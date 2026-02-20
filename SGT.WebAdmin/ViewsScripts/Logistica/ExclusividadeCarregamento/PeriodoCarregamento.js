/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Globais.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Rest.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var PeriodoCarregamentoModel = function (instancia) {
    this.Grid = PropertyEntity({ type: types.local });

    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.HoraInicio = PropertyEntity({ val: ko.observable(""), def: "", getType: typesKnockout.time, text: "*Início:", required: true });
    this.HoraTermino = PropertyEntity({ val: ko.observable(""), def: "", getType: typesKnockout.time, text: "*Término:", required: true });

    this.ContainerTipoOperacao = PropertyEntity({ type: types.listEntity, list: new Array(), idGrid: guid(), codEntity: ko.observable(0) });
    this.GridTipoOperacao = PropertyEntity({ type: types.local });
    this.TipoOperacao = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "*Tipo Operação:", idBtnSearch: guid() });
    this.CapacidadeTipoOperacao = PropertyEntity({ val: ko.observable(""), def: "", getType: typesKnockout.int, text: "*Quantidade:" });
    this.AdicionarTipoOperacao = PropertyEntity({ eventClick: instancia.AdicionarTipoOperacao, type: types.event, text: "Adicionar", visible: ko.observable(true) });

    this.Adicionar = PropertyEntity({ eventClick: instancia.Adicionar, type: types.event, text: "Adicionar", visible: ko.observable(true) });
    this.Atualizar = PropertyEntity({ eventClick: instancia.Atualizar, type: types.event, text: "Atualizar", visible: ko.observable(false) });
    this.Excluir = PropertyEntity({ eventClick: instancia.Excluir, type: types.event, text: "Excluir", visible: ko.observable(false) });
    this.Limpar = PropertyEntity({ eventClick: instancia.LimparCampos, type: types.event, text: "Limpar", visible: ko.observable(true) });
}

var PeriodoCarregamento = function (idKnockout, containerItens) {
    var $this = this;

    $this.IdKnockout = idKnockout;
    $this.Container = containerItens;

    this.Load = function () {
        $this.PeriodoCarregamento = new PeriodoCarregamentoModel($this);
        KoBindings($this.PeriodoCarregamento, $this.IdKnockout);

        new BuscarTiposOperacao($this.PeriodoCarregamento.TipoOperacao);

        $this.LoadGrid();
        $this.LoadGridTipoOperacao();
    }

    this.LoadGrid = function () {
        var menuOpcoes = { tipo: TypeOptionMenu.link, tamanho: 7, opcoes: [{ descricao: "Editar", id: guid(), metodo: $this.Editar }] };

        var header = [
            { data: "Codigo", visible: false },
            { data: "HoraInicio", title: "Início", width: "15%" },
            { data: "HoraTermino", title: "Término", width: "15%" },
        ];

        $this.Grid = new BasicDataTable($this.PeriodoCarregamento.Grid.id, header, menuOpcoes, { column: 1, dir: orderDir.asc });

        $this.RecarregarGrid();
    }

    this.LoadGridTipoOperacao = function () {
        var menuOpcoes = { tipo: TypeOptionMenu.link, tamanho: 7, opcoes: [{ descricao: "Excluir", id: guid(), metodo: $this.ExcluirTipoOperacao }] };

        var header = [
            { data: "Codigo", visible: false },
            { data: "TipoOperacao", title: "Tipo Operação", width: "30%" },
            { data: "Capacidade", title: "Capacidade", width: "15%" }
        ];

        $this.GridTipoOperacao = new BasicDataTable($this.PeriodoCarregamento.GridTipoOperacao.id, header, menuOpcoes, { column: 2, dir: orderDir.desc });

        $this.RecarregarGridTipoOperacao();
    }

    this.RecarregarGrid = function () {

        var data = new Array();

        $.each($this.Container.list, function (i, periodoCarregamento) {
            var periodoCarregamentoGrid = new Object();

            periodoCarregamentoGrid.Codigo = periodoCarregamento.Codigo.val;
            periodoCarregamentoGrid.HoraInicio = periodoCarregamento.HoraInicio.val;
            periodoCarregamentoGrid.HoraTermino = periodoCarregamento.HoraTermino.val;

            data.push(periodoCarregamentoGrid);
        });

        $this.Grid.CarregarGrid(data);
    }

    this.RecarregarGridTipoOperacao = function () {
        var data = new Array();

        $.each($this.PeriodoCarregamento.ContainerTipoOperacao.list, function (i, tipoOperacao) {
            data.push({
                Codigo: tipoOperacao.TipoOperacao.codEntity,
                TipoOperacao: tipoOperacao.TipoOperacao.val,
                Capacidade: tipoOperacao.CapacidadeTipoOperacao.val,
            });
        });

        $this.GridTipoOperacao.CarregarGrid(data);
    }

    this.Editar = function (row) {
        for (var i = 0; i < $this.Container.list.length; i++) {
            if (row.Codigo == $this.Container.list[i].Codigo.val) {
                $this.LimparCampos();

                var data = $this.Container.list[i];

                PreencherEditarListEntity($this.PeriodoCarregamento, data);
                $this.PeriodoCarregamento.ContainerTipoOperacao.list = data.ContainerTipoOperacao.list.slice();
                $this.RecarregarGridTipoOperacao();

                $this.PeriodoCarregamento.Adicionar.visible(false);
                $this.PeriodoCarregamento.Atualizar.visible(true);
                $this.PeriodoCarregamento.Excluir.visible(true);

                return;
            }
        }
    }

    this.ExcluirTipoOperacao = function (data) {
        for (var i = 0; i < $this.PeriodoCarregamento.ContainerTipoOperacao.list.length; i++) {
            if (data.Codigo == $this.PeriodoCarregamento.ContainerTipoOperacao.list[i].TipoOperacao.codEntity) {
                $this.PeriodoCarregamento.ContainerTipoOperacao.list.splice(i, 1);
                break;
            }
        }

        $this.RecarregarGridTipoOperacao();
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

    this.ValidarDados = function () {
        var valido = ValidarCamposObrigatorios($this.PeriodoCarregamento);

        if (!valido) {
            exibirMensagem(tipoMensagem.atencao, "Campos Obrigatórios", "Informe os campos obrigatórios!");
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
                exibirMensagem(tipoMensagem.aviso, "Período já existe", "O período entrou em conflito com um período já existente: de " + $this.Container.list[i].HoraInicio.val + " até " + $this.Container.list[i].HoraTermino.val + ".");
                return false;
            }
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

    this.AdicionarTipoOperacao = function () {
        if ($this.PeriodoCarregamento.TipoOperacao.codEntity() == 0 || $this.PeriodoCarregamento.CapacidadeTipoOperacao.val() == 0) {
            exibirMensagem(tipoMensagem.atencao, "Campos Obrigatórios", "Informe os campos obrigatórios!");
            return;
        }

        var codigoTipoOperacao = $this.PeriodoCarregamento.TipoOperacao.codEntity();
        for (var i = 0; i < $this.PeriodoCarregamento.ContainerTipoOperacao.list.length; i++) {
            if ($this.PeriodoCarregamento.ContainerTipoOperacao.list[i].TipoOperacao.codEntity == codigoTipoOperacao) {
                exibirMensagem(tipoMensagem.aviso, "Tipo de Operação já existe", "Já existe uma quantidade informada com o tipo de operação " + $this.PeriodoCarregamento.TipoOperacao.val() + ".");
                return;
            }
        }

        var data = SalvarListEntity({
            TipoOperacao: $this.PeriodoCarregamento.TipoOperacao,
            CapacidadeTipoOperacao: $this.PeriodoCarregamento.CapacidadeTipoOperacao,
        });

        $this.PeriodoCarregamento.ContainerTipoOperacao.list.push(data);

        $this.RecarregarGridTipoOperacao();
        $this.LimparCamposTipoOperacao();
    }

    this.LimparCampos = function () {
        LimparCampos($this.PeriodoCarregamento);

        $this.PeriodoCarregamento.Adicionar.visible(true);
        $this.PeriodoCarregamento.Atualizar.visible(false);
        $this.PeriodoCarregamento.Excluir.visible(false);

        $this.PeriodoCarregamento.ContainerTipoOperacao.list = [];
        $this.RecarregarGridTipoOperacao();
    }

    this.LimparCamposTipoOperacao = function () {
        $this.PeriodoCarregamento.TipoOperacao.codEntity($this.PeriodoCarregamento.TipoOperacao.defCodEntity);
        $this.PeriodoCarregamento.TipoOperacao.val($this.PeriodoCarregamento.TipoOperacao.def);
        $this.PeriodoCarregamento.CapacidadeTipoOperacao.val($this.PeriodoCarregamento.CapacidadeTipoOperacao.def);
    }
}
