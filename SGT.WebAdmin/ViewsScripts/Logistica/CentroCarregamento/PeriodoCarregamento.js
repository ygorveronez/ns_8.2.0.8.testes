/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Globais.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="../../Consultas/ModeloVeicularCarga.js" />
/// <reference path="../../Enumeradores/EnumDiaSemana.js" />
/// <reference path="CentroCarregamento.js" />
/// <reference path="TipoCarga.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var PeriodoCarregamentoModel = function (instancia) {
    this.Grid = PropertyEntity({ type: types.local });

    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.DiaSemana = PropertyEntity({ val: ko.observable(instancia.DiaSemana), def: instancia.DiaSemana, getType: typesKnockout.int });
    this.Dia = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Mes = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.HoraInicio = PropertyEntity({ val: ko.observable(""), def: "", getType: typesKnockout.time, text: Localization.Resources.Logistica.CentroCarregamento.Inicio.getRequiredFieldDescription(), required: true });
    this.HoraTermino = PropertyEntity({ val: ko.observable(""), def: "", getType: typesKnockout.time, text: Localization.Resources.Logistica.CentroCarregamento.Termino.getRequiredFieldDescription(), required: true });
    this.CapacidadeCarregamentoVolume = PropertyEntity({ val: ko.observable(""), def: "", getType: typesKnockout.int, text: Localization.Resources.Logistica.CentroCarregamento.CapacidadeCarregamentoVolume.getFieldDescription(), required: false, visible: ko.observable(false), maxlength: 15 });
    this.CapacidadeCarregamentoCubagem = PropertyEntity({ val: ko.observable(""), def: "", getType: typesKnockout.int, text: Localization.Resources.Logistica.CentroCarregamento.CapacidadeCarregamentoCubagem.getFieldDescription(), required: false, visible: ko.observable(false), maxlength: 15 });
    this.CapacidadeCarregamentoSimultaneo = PropertyEntity({ val: ko.observable(""), def: "", getType: typesKnockout.int, text: Localization.Resources.Logistica.CentroCarregamento.CarregamentoSimultaneo.getRequiredFieldDescription(), required: true });

    this.ToleranciaExcessoTempo = PropertyEntity({ val: ko.observable(""), def: "", getType: typesKnockout.int, text: Localization.Resources.Logistica.CentroCarregamento.ExcessoDeTempo.getFieldDescription(), issue: 324 });

    this.Adicionar = PropertyEntity({ eventClick: instancia.Adicionar, type: types.event, text: Localization.Resources.Gerais.Geral.Adicionar, visible: ko.observable(true) });
    this.AdicionarDia = PropertyEntity({ eventClick: instancia.Adicionar, type: types.event, text: Localization.Resources.Gerais.Geral.Adicionar, visible: ko.observable(true) });
    this.AtualizarDia = PropertyEntity({ eventClick: instancia.Adicionar, type: types.event, text: Localization.Resources.Gerais.Geral.Adicionar, visible: ko.observable(true) });
    this.ImportarDeOutroDia = PropertyEntity({ eventClick: instancia.ImportarDeOutroDia, type: types.event, text: Localization.Resources.Logistica.CentroCarregamento.ImportarDeOutroDia, visible: ko.observable(true) });
    this.ImportarParaOsDemaisDias = PropertyEntity({ eventClick: instancia.ImportarParaOsDemaisDias, type: types.event, text: Localization.Resources.Logistica.CentroCarregamento.ImportarParaOsDemaisDias, visible: ko.observable(true) });
}

var PeriodoCarregamento = function (diaSemana, idKnockout, containerItens) {

    var $this = this;

    $this.DiaSemana = diaSemana;
    $this.IdKnockout = idKnockout;
    $this.Container = containerItens;

    this.Load = function () {
        $this.PeriodoCarregamento = new PeriodoCarregamentoModel($this);
        KoBindings($this.PeriodoCarregamento, $this.IdKnockout);

        var menuOpcoes = { tipo: TypeOptionMenu.link, tamanho: 7, opcoes: [{ descricao: Localization.Resources.Gerais.Geral.Editar, id: guid(), metodo: $this.Editar }, { descricao: Localization.Resources.Gerais.Geral.Excluir, id: guid(), metodo: $this.Excluir }] };

        var header = [
            { data: "Codigo", visible: false },
            { data: "HoraInicio", title: Localization.Resources.Logistica.CentroCarregamento.Inicio, width: "11%" },
            { data: "HoraTermino", title: Localization.Resources.Logistica.CentroCarregamento.Termino, width: "11%" },
            { data: "CapacidadeCarregamentoSimultaneo", title: Localization.Resources.Logistica.CentroCarregamento.CarregamentoSimultaneo, width: "17%" },
            { data: "ToleranciaExcessoTempo", title: Localization.Resources.Logistica.CentroCarregamento.Tolerancia, width: "13%" },
            { data: "CapacidadeCarregamentoVolume", title: Localization.Resources.Logistica.CentroCarregamento.CapacidadeCarregamentoVolume, width: "25%", visible: false },
            { data: "CapacidadeCarregamentoCubagem", title: Localization.Resources.Logistica.CentroCarregamento.CapacidadeCarregamentoCubagem, width: "25%" }
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
                periodoCarregamentoGrid.CapacidadeCarregamentoSimultaneo = periodoCarregamento.CapacidadeCarregamentoSimultaneo.val;
                periodoCarregamentoGrid.ToleranciaExcessoTempo = (periodoCarregamento.ToleranciaExcessoTempo.val || "0") + " min.";
                periodoCarregamentoGrid.CapacidadeCarregamentoVolume = periodoCarregamento.CapacidadeCarregamentoVolume.val;
                periodoCarregamentoGrid.CapacidadeCarregamentoCubagem = periodoCarregamento.CapacidadeCarregamentoCubagem.val;

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
                $this.PeriodoCarregamento.CapacidadeCarregamentoVolume.val($this.Container.list[i].CapacidadeCarregamentoVolume.val);
                $this.PeriodoCarregamento.CapacidadeCarregamentoSimultaneo.val($this.Container.list[i].CapacidadeCarregamentoSimultaneo.val);
                $this.PeriodoCarregamento.ToleranciaExcessoTempo.val($this.Container.list[i].ToleranciaExcessoTempo.val);
                $this.PeriodoCarregamento.CapacidadeCarregamentoCubagem.val($this.Container.list[i].CapacidadeCarregamentoCubagem.val);

                break;
            }
        }
        $this.RecarregarGrid();
    }

    this.Adicionar = function () {
        var valido = ValidarCamposObrigatorios($this.PeriodoCarregamento);

        if (!valido) {
            exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.CamposObrigatorios, Localization.Resources.Gerais.Geral.InformeCamposObrigatorios);
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
                        exibirMensagem(tipoMensagem.aviso, Localization.Resources.Logistica.CentroCarregamento.PeriodoJaExiste, Localization.Resources.Logistica.CentroCarregamento.PeriodoEntrouEmConfiltoComUmPeridoJaExistenteDeAte.format($this.Container.list[i].HoraInicio.val, $this.Container.list[i].HoraTermino.val));
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

        $this.RecarregarGrid();
        $this.LimparCampos();
    }

    this.ImportarDeOutroDia = function () {
        $("#" + _importacaoPeriodo.DiaSemana.id + " option").attr("disabled", false);
        $("#" + _importacaoPeriodo.DiaSemana.id + " option[value='" + $this.DiaSemana + "']").attr("disabled", true);
        $("#" + _importacaoPeriodo.DiaSemana.id).find('option:enabled:first').prop('selected', true);
        $("#" + _importacaoPeriodo.DiaSemana.id).change();
        _diaParaImportarPeriodo = $this.DiaSemana;
        Global.abrirModal('divModalImportacaoPeriodo');
    }

    this.ImportarParaOsDemaisDias = function () {

        var diasParaImportar = new Array();

        exibirConfirmacao(Localization.Resources.Gerais.Geral.Atencao, Localization.Resources.Logistica.CentroCarregamento.DesejaRealmenteImportarOsDadosDeParaOsDemaisDiasOsDadosExistentesNosDemaisDiasSeraoApagados.format(EnumDiaSemana.obterDescricaoResumida($this.DiaSemana)), function () {

            for (var i = $this.Container.list.length - 1; i >= 0; i--) {
                if ($this.Container.list[i].DiaSemana.val != $this.DiaSemana) {
                    $this.Container.list.splice(i, 1);
                } else {
                    for (var x = 0; x < _listaKnockoutPeriodosCarregamento.length; x++) {
                        if (_listaKnockoutPeriodosCarregamento[x] != null && _listaKnockoutPeriodosCarregamento[x].DiaSemana != $this.DiaSemana) {
                            var diaImportar = $.extend(true, {}, $this.Container.list[i]);
                            diaImportar.DiaSemana.val = _listaKnockoutPeriodosCarregamento[x].DiaSemana;
                            diaImportar.Codigo.val = guid();
                            diasParaImportar.push(diaImportar);
                        }
                    }
                }
            }

            $this.Container.list = $this.Container.list.concat(diasParaImportar);

            for (var i = 0; i < _listaKnockoutPeriodosCarregamento.length; i++)
                if (_listaKnockoutPeriodosCarregamento[i] != null)
                    _listaKnockoutPeriodosCarregamento[i].RecarregarGrid();

            exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, Localization.Resources.Gerais.Geral.DadosImportadosComSucesso);
        });
    }

    this.LimparCampos = function () {
        LimparCampos($this.PeriodoCarregamento);
    }

    this.ControlarExibicaoCapacidadeCarregamento = function (exibir) {
        var exibirCapacidadeCarregamentoCubagemPeriodo = (_capacidadeCarregamento.TipoCapacidadeCarregamento.val() == EnumTipoCapacidadeCarregamento.CubagemVolume && _centroCarregamento.TipoCapacidadeCarregamentoPorPeso.val() == EnumTipoCapacidadeCarregamentoPorPeso.PeriodoCarregamento);
        $this.PeriodoCarregamento.CapacidadeCarregamentoVolume.visible(exibir);
        $this.Grid.ControlarExibicaoColuna("CapacidadeCarregamentoVolume", exibir);

        $this.PeriodoCarregamento.CapacidadeCarregamentoCubagem.visible(exibirCapacidadeCarregamentoCubagemPeriodo);

        if (!exibir)
            $this.PeriodoCarregamento.CapacidadeCarregamentoVolume.val("");

        if (!exibirCapacidadeCarregamentoCubagemPeriodo)
            $this.PeriodoCarregamento.CapacidadeCarregamentoCubagem.val("");
    }
}
