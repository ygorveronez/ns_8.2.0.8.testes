/// <reference path="../../../js/libs/jquery-2.1.1.js" />
/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/bootstrap/bootstrap.js" />
/// <reference path="../../../js/libs/jquery.blockui.js" />
/// <reference path="../../../js/Global/knoutViewsSlides.js" />
/// <reference path="../../../js/libs/jquery.maskMoney.js" />
/// <reference path="../../Consultas/ModeloVeicularCarga.js" />
/// <reference path="../../Consultas/Tranportador.js" />
/// <reference path="CentroCarregamento.js" />
/// <reference path="../../Enumeradores/EnumDiaSemana.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var DisponibilidadeFrotaModel = function (instancia) {
    this.Grid = PropertyEntity({ type: types.local });

    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.DiaSemana = PropertyEntity({ val: ko.observable(instancia.DiaSemana), def: instancia.DiaSemana, getType: typesKnockout.int });
    this.Quantidade = PropertyEntity({ val: ko.observable(""), def: "", getType: typesKnockout.int, text: Localization.Resources.Logistica.CentroCarregamento.Quantidade.getRequiredFieldDescription(), required: true, configInt: { precision: 0, allowZero: true } });
    this.ModeloVeicular = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Logistica.CentroCarregamento.ModeloVeicular.getRequiredFieldDescription(), idBtnSearch: guid(), required: true });
    this.Transportador = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Logistica.CentroCarregamento.Transportador.getFieldDescription(), idBtnSearch: guid(), required: false });

    this.Adicionar = PropertyEntity({ eventClick: instancia.Adicionar, type: types.event, text: Localization.Resources.Gerais.Geral.Adicionar, visible: ko.observable(true) });
    this.ImportarDeOutroDia = PropertyEntity({ eventClick: instancia.ImportarDeOutroDia, type: types.event, text: Localization.Resources.Logistica.CentroCarregamento.ImportarDeOutroDia, visible: ko.observable(true) });
    this.ImportarParaOsDemaisDias = PropertyEntity({ eventClick: instancia.ImportarParaOsDemaisDias, type: types.event, text: Localization.Resources.Logistica.CentroCarregamento.ImportarParaOsDemaisDias, visible: ko.observable(true) });
}

var DisponibilidadeFrota = function (diaSemana, idKnockout, containerItens) {

    var $this = this;

    $this.DiaSemana = diaSemana;
    $this.IdKnockout = idKnockout;
    $this.Container = containerItens;

    this.Load = function () {
        $this.DisponibilidadeFrota = new DisponibilidadeFrotaModel($this);
        KoBindings($this.DisponibilidadeFrota, $this.IdKnockout);

        new BuscarModelosVeicularesCarga($this.DisponibilidadeFrota.ModeloVeicular);
        new BuscarTransportadores($this.DisponibilidadeFrota.Transportador);

        var menuOpcoes = { tipo: TypeOptionMenu.link, tamanho: 7, opcoes: [{ descricao: Localization.Resources.Gerais.Geral.Editar, id: guid(), metodo: $this.Editar }, { descricao: Localization.Resources.Gerais.Geral.Excluir, id: guid(), metodo: $this.Excluir }] };

        var header = [{ data: "Codigo", visible: false },
        { data: "ModeloVeicular", title: Localization.Resources.Logistica.CentroCarregamento.ModeloVeicular, width: "30%" },
        { data: "Quantidade", title: Localization.Resources.Logistica.CentroCarregamento.Quantidade, width: "20%" },
        { data: "Transportador", title: Localization.Resources.Logistica.CentroCarregamento.Transportador, width: "30%" },
        ];

        $this.Grid = new BasicDataTable($this.DisponibilidadeFrota.Grid.id, header, menuOpcoes, { column: 1, dir: orderDir.asc });

        $this.RecarregarGrid();
    }

    this.RecarregarGrid = function () {
        var data = new Array();

        $.each($this.Container.list, function (i, DisponibilidadeFrota) {
            if (DisponibilidadeFrota.DiaSemana.val == $this.DiaSemana) {
                var DisponibilidadeFrotaGrid = new Object();

                DisponibilidadeFrotaGrid.Codigo = DisponibilidadeFrota.Codigo.val;
                DisponibilidadeFrotaGrid.Quantidade = DisponibilidadeFrota.Quantidade.val;
                DisponibilidadeFrotaGrid.ModeloVeicular = DisponibilidadeFrota.ModeloVeicular.val;
                DisponibilidadeFrotaGrid.Transportador = DisponibilidadeFrota.Transportador.val;
                data.push(DisponibilidadeFrotaGrid);
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
                $this.DisponibilidadeFrota.Codigo.val($this.Container.list[i].Codigo.val);
                $this.DisponibilidadeFrota.DiaSemana.val($this.Container.list[i].DiaSemana.val);
                $this.DisponibilidadeFrota.Quantidade.val($this.Container.list[i].Quantidade.val);
                $this.DisponibilidadeFrota.ModeloVeicular.codEntity($this.Container.list[i].ModeloVeicular.codEntity);
                $this.DisponibilidadeFrota.ModeloVeicular.val($this.Container.list[i].ModeloVeicular.val);
                $this.DisponibilidadeFrota.Transportador.codEntity($this.Container.list[i].Transportador.codEntity);
                $this.DisponibilidadeFrota.Transportador.val($this.Container.list[i].Transportador.val);
                break;
            }
        }
    }

    this.Adicionar = function () {
        var valido = ValidarCamposObrigatorios($this.DisponibilidadeFrota);

        if (!valido) {
            exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.CamposObrigatorios, Localization.Resources.Gerais.Geral.InformeCamposObrigatorios);
            return;
        }

        if ($this.DisponibilidadeFrota.Codigo.val() == '') {
            $this.DisponibilidadeFrota.Codigo.val(guid());
            $this.Container.list.push(SalvarListEntity($this.DisponibilidadeFrota));
        } else {
            for (var i = 0; i < $this.Container.list.length; i++) {
                if ($this.DisponibilidadeFrota.Codigo.val() == $this.Container.list[i].Codigo.val) {
                    $this.Container.list[i] = SalvarListEntity($this.DisponibilidadeFrota);
                    break;
                }
            }
        }

        $this.RecarregarGrid();
        $this.LimparCampos();
    }

    this.ImportarDeOutroDia = function () {
        $("#" + _importacaoDisponibilidade.DiaSemana.id + " option").attr("disabled", false);
        $("#" + _importacaoDisponibilidade.DiaSemana.id + " option[value='" + $this.DiaSemana + "']").attr("disabled", true);
        $("#" + _importacaoDisponibilidade.DiaSemana.id).find('option:enabled:first').prop('selected', true);
        $("#" + _importacaoDisponibilidade.DiaSemana.id).change();
        _diaParaImportarDisponibilidade = $this.DiaSemana;
        Global.abrirModal("divModalImportacaoDisponibilidade");
    }

    this.ImportarParaOsDemaisDias = function () {

        var diasParaImportar = new Array();

        exibirConfirmacao(Localization.Resources.Gerais.Geral.Atencao, Localization.Resources.Logistica.CentroCarregamento.DesejaRealmenteImportarOsDadosDeParaOsDemaisDiasOsDadosExistentesNosDemaisDiasSeraoApagados.format(EnumDiaSemana.obterDescricaoResumida($this.DiaSemana)), function () {

            for (var i = $this.Container.list.length - 1; i >= 0; i--) {
                if ($this.Container.list[i].DiaSemana.val != $this.DiaSemana) {
                    $this.Container.list.splice(i, 1);
                } else {
                    for (var x = 0; x < _listaKnockoutDisponibilidadeFrota.length; x++) {
                        if (_listaKnockoutDisponibilidadeFrota[x] != null && _listaKnockoutDisponibilidadeFrota[x].DiaSemana != $this.DiaSemana) {
                            var diaImportar = $.extend(true, {}, $this.Container.list[i]);
                            diaImportar.DiaSemana.val = _listaKnockoutDisponibilidadeFrota[x].DiaSemana;
                            diaImportar.Codigo.val = guid();
                            diasParaImportar.push(diaImportar);
                        }
                    }
                }
            }

            $this.Container.list = $this.Container.list.concat(diasParaImportar);

            for (var i = 0; i < _listaKnockoutDisponibilidadeFrota.length; i++)
                if (_listaKnockoutDisponibilidadeFrota[i] != null)
                    _listaKnockoutDisponibilidadeFrota[i].RecarregarGrid();

            exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, Localization.Resources.Gerais.Geral.DadosImportadosComSucesso);
        });
    }

    this.LimparCampos = function () {
        LimparCampos($this.DisponibilidadeFrota);
    }
}