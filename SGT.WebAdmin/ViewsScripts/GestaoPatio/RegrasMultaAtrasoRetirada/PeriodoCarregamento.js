/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Globais.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="../../Consultas/ModeloVeicularCarga.js" />
/// <reference path="../../Enumeradores/EnumDiaSemana.js" />
/// <reference path="RegrasMultaAtrasoRetirada.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var PeriodoCarregamentoModel = function (instancia) {
    this.Grid = PropertyEntity({ type: types.local });

    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.DiaSemana = PropertyEntity({ val: ko.observable(instancia.DiaSemana), def: instancia.DiaSemana, getType: typesKnockout.int });
    this.HoraInicio = PropertyEntity({ val: ko.observable(""), def: "", getType: typesKnockout.time, text: "*Início:", required: true });
    this.HoraTermino = PropertyEntity({ val: ko.observable(""), def: "", getType: typesKnockout.time, text: "*Término:", required: true });
    this.QuantidadeCargas = PropertyEntity({ val: ko.observable(""), def: "", getType: typesKnockout.int, text: "*Quantidade Cargas:", required: true });
    this.QuantidadeHorasContrato = PropertyEntity({ val: ko.observable(""), def: "", getType: typesKnockout.int, text: "Quantidade Horas Contrato:", required: false });

    this.Adicionar = PropertyEntity({ eventClick: instancia.Adicionar, type: types.event, text: ko.observable("Adicionar"), visible: ko.observable(true) });
    this.Cancelar = PropertyEntity({ eventClick: instancia.Cancelar, type: types.event, text: "Cancelar", visible: ko.observable(false) });
}

var PeriodoCarregamento = function (diaSemana, idKnockout, containerItens) {

    var $this = this;

    $this.DiaSemana = diaSemana;
    $this.IdKnockout = idKnockout;
    $this.Container = containerItens;

    this.Load = function () {
        $this.PeriodoCarregamento = new PeriodoCarregamentoModel($this);
        KoBindings($this.PeriodoCarregamento, $this.IdKnockout);

        var menuOpcoes = { tipo: TypeOptionMenu.link, tamanho: 7, opcoes: [{ descricao: "Editar", id: guid(), metodo: $this.Editar }, { descricao: "Excluir", id: guid(), metodo: $this.Excluir }] };

        var header = [
            { data: "Codigo", visible: false },
            { data: "HoraInicio", title: "Início", width: "15%" },
            { data: "HoraTermino", title: "Término", width: "15%" },
            { data: "QuantidadeCargas", title: "Quantidade Cargas", width: "15%" },
            { data: "QuantidadeHorasContrato", title: "Quantidade Horas Contrato", width: "15%" }
        ];

        $this.Grid = new BasicDataTable($this.PeriodoCarregamento.Grid.id, header, menuOpcoes, { column: 1, dir: orderDir.asc });

        $this.RecarregarGrid();
    }

    this.RecarregarGrid = function () {

        var data = new Array();

        $.each($this.Container.list, function (i, periodoCarregamento) {
            if (periodoCarregamento.DiaSemana.val == $this.DiaSemana) {
                var periodoCarregamentoGrid = new Object();

                periodoCarregamentoGrid.Codigo = periodoCarregamento.Codigo.val;
                periodoCarregamentoGrid.HoraInicio = periodoCarregamento.HoraInicio.val;
                periodoCarregamentoGrid.HoraTermino = periodoCarregamento.HoraTermino.val;
                periodoCarregamentoGrid.QuantidadeCargas = periodoCarregamento.QuantidadeCargas.val;
                periodoCarregamentoGrid.QuantidadeHorasContrato = periodoCarregamento.QuantidadeHorasContrato.val;

                data.push(periodoCarregamentoGrid);
            }
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

        $this.RecarregarGrid();
    }

    this.Editar = function (data) {
        for (var i = 0; i < $this.Container.list.length; i++) {
            if (data.Codigo == $this.Container.list[i].Codigo.val) {

                $this.PeriodoCarregamento.Codigo.val($this.Container.list[i].Codigo.val);
                $this.PeriodoCarregamento.DiaSemana.val($this.Container.list[i].DiaSemana.val);
                $this.PeriodoCarregamento.HoraInicio.val($this.Container.list[i].HoraInicio.val);
                $this.PeriodoCarregamento.HoraTermino.val($this.Container.list[i].HoraTermino.val);
                $this.PeriodoCarregamento.QuantidadeCargas.val($this.Container.list[i].QuantidadeCargas.val);
                $this.PeriodoCarregamento.QuantidadeHorasContrato.val($this.Container.list[i].QuantidadeHorasContrato.val);

                break;
            }
        }
        $this.RecarregarGrid();
        $this.PeriodoCarregamento.Cancelar.visible(true);
        $this.PeriodoCarregamento.Adicionar.text("Atualizar");
    }

    this.Adicionar = function () {
        $this.PeriodoCarregamento.Adicionar.text("Adicionar");
        var valido = ValidarCamposObrigatorios($this.PeriodoCarregamento);

        if (!valido) {
            exibirMensagem(tipoMensagem.atencao, "Campos Obrigatórios", "Informe os Campos Obrigatórios");
            return;
        }

        var horaInicio = moment($this.PeriodoCarregamento.HoraInicio.val(), "HH:mm");
        var horaTermino = moment($this.PeriodoCarregamento.HoraTermino.val(), "HH:mm");

        for (var i = 0; i < $this.Container.list.length; i++) {
            if ($this.Container.list[i].DiaSemana.val == $this.DiaSemana) {
                var horaInicioGrid = moment($this.Container.list[i].HoraInicio.val, "HH:mm");
                var horaTerminoGrid = moment($this.Container.list[i].HoraTermino.val, "HH:mm");

                if ($this.PeriodoCarregamento.Codigo.val() == '') {
                    if (horaInicio.isBetween(horaInicioGrid, horaTerminoGrid) ||
                        horaTermino.isBetween(horaInicioGrid, horaTerminoGrid) ||
                        horaInicio.diff(horaInicioGrid) == 0 ||
                        horaInicio.diff(horaTerminoGrid) == 0 ||
                        horaTermino.diff(horaTerminoGrid) == 0 ||
                        horaTermino.diff(horaInicioGrid) == 0) {
                        exibirMensagem(tipoMensagem.aviso, "Período já exise", "Período entrou em conflito com um período já existente " + $this.Container.list[i].HoraInicio.val + " " + $this.Container.list[i].HoraTermino.val);
                        return;
                    }
                }
            }
        }

        if ($this.PeriodoCarregamento.Codigo.val() == '') {
            $this.PeriodoCarregamento.Codigo.val(guid());
            $this.Container.list.push(SalvarListEntity($this.PeriodoCarregamento));
        } else {
            for (var i = 0; i < $this.Container.list.length; i++) {
                if ($this.PeriodoCarregamento.Codigo.val() == $this.Container.list[i].Codigo.val) {
                    $this.Container.list[i] = SalvarListEntity($this.PeriodoCarregamento);
                    break;
                }
            }
        }

        $this.PeriodoCarregamento.Cancelar.visible(false);
        $this.RecarregarGrid();
        $this.LimparCampos();
    }

    this.LimparCampos = function () {
        LimparCampos($this.PeriodoCarregamento);
    }

    this.Cancelar = function () {
        $this.PeriodoCarregamento.Cancelar.visible(false);
        $this.PeriodoCarregamento.Adicionar.text("Adicionar");
        $this.LimparCampos();
    }
}