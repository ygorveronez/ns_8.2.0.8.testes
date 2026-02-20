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
/// <reference path="CentroDescarregamento.js" />
/// <reference path="../../Enumeradores/EnumDiaSemana.js" />
/// <reference path="../../Consultas/RotaFrete.js" />
/// <reference path="../../Consultas/ModeloVeicularCarga.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var PrevisaoDescarregamentoModel = function (instancia) {
    this.Grid = PropertyEntity({ type: types.local });
    this.GridModeloVeiculo = PropertyEntity({ type: types.local });

    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.DiaSemana = PropertyEntity({ val: ko.observable(instancia.DiaSemana), def: instancia.DiaSemana, getType: typesKnockout.int });
    this.Dia = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Mes = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Descricao = PropertyEntity({ val: ko.observable(""), def: "", maxlength: 150, text: Localization.Resources.Gerais.Geral.Descricao.getRequiredFieldDescription(), required: true });
    this.QuantidadeCargas = PropertyEntity({ val: ko.observable(""), def: "", getType: typesKnockout.int, maxlength: 3, text: Localization.Resources.Logistica.CentroDescarregamento.QuantidadeCargas.getFieldDescription(), required: true });
    this.QuantidadeCargasExcedentes = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int, maxlength: 3, text: Localization.Resources.Logistica.CentroDescarregamento.QtdCargasExcedentes.getRequiredFieldDescription(), required: true, configInt: { precision: 0, allowZero: true }, });
    this.Rota = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Gerais.Geral.Rota.getRequiredFieldDescription, idBtnSearch: guid(), required: true });
    this.ModeloVeiculo = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Logistica.CentroDescarregamento.AdicionarModeloVeiculo.getFieldDescription(), idBtnSearch: guid(), enable: true });
    this.ModelosVeiculos = PropertyEntity({ type: types.map, getType: typesKnockout.dynamic, val: ko.observable(""), def: "", idGrid: guid() });

    this.Adicionar = PropertyEntity({ eventClick: instancia.Adicionar, type: types.event, text: Localization.Resources.Gerais.Geral.Adicionar.getFieldDescription(), visible: ko.observable(true) });
    this.SalvarDiaMes = PropertyEntity({ eventClick: instancia.SalvarDia, type: types.event, text: Localization.Resources.Gerais.Geral.Salvar, visible: ko.observable(true) });
    this.Atualizar = PropertyEntity({ eventClick: instancia.Atualizar, type: types.event, text: Localization.Resources.Gerais.Geral.Atualizar, visible: ko.observable(false) });
    this.Cancelar = PropertyEntity({ eventClick: instancia.Cancelar, type: types.event, text: Localization.Resources.Gerais.Geral.Cancelar, visible: ko.observable(false) });
    this.Excluir = PropertyEntity({ eventClick: instancia.Excluir, type: types.event, text: Localization.Resources.Gerais.Geral.Excluir, visible: ko.observable(false) });
    this.ImportarDeOutroDia = PropertyEntity({ eventClick: instancia.ImportarDeOutroDia, type: types.event, text: Localization.Resources.Logistica.CentroDescarregamento.ImportarOutroDia, visible: ko.observable(true) });
    this.ImportarParaOsDemaisDias = PropertyEntity({ eventClick: instancia.ImportarParaOsDemaisDias, type: types.event, text: Localization.Resources.Logistica.CentroDescarregamento.ImportarParaDemaisDias, visible: ko.observable(true) });

    this.ImportarDeOutroDiaMes = PropertyEntity({ eventClick: instancia.ImportarDeOutroDiaMes, type: types.event, text: Localization.Resources.Logistica.CentroDescarregamento.ImportarOutroDia, visible: ko.observable(true) });
    this.ImportarParaOsDemaisDiasMes = PropertyEntity({ eventClick: instancia.ImportarParaOsDemaisDiasMes, type: types.event, text: Localization.Resources.Logistica.CentroDescarregamento.ImportarOutroDiaMes, visible: ko.observable(true) });
}

var PrevisaoDescarregamento = function (diaSemana, idKnockout, containerItens) {

    var $this = this;

    $this.DiaSemana = diaSemana;
    $this.IdKnockout = idKnockout;
    $this.Container = containerItens;

    this.Load = function () {
        $this.PrevisaoDescarregamento = new PrevisaoDescarregamentoModel($this);
        KoBindings($this.PrevisaoDescarregamento, $this.IdKnockout);

        new BuscarRotasFrete($this.PrevisaoDescarregamento.Rota);

        var menuOpcoes = { tipo: TypeOptionMenu.link, tamanho: 7, opcoes: [{ descricao: Localization.Resources.Gerais.Geral.Editar, id: guid(), metodo: $this.Editar }] };

        var header = [{ data: "Codigo", visible: false },
            { data: "Rota", title: Localization.Resources.Gerais.Geral.Rota, width: "25%" },
            { data: "Descricao", title: Localization.Resources.Gerais.Geral.Descricao, width: "25%" },
            { data: "QuantidadeCargas", title: Localization.Resources.Logistica.CentroDescarregamento.QuantidadeCargas, width: "18%" },
            { data: "QuantidadeCargasExcedentes", title: Localization.Resources.Logistica.CentroDescarregamento.CargasExcedentes, width: "18%" }];

        $this.Grid = new BasicDataTable($this.PrevisaoDescarregamento.Grid.id, header, menuOpcoes, { column: 1, dir: orderDir.asc });

        $this.RecarregarGrid();

        $this.LoadModeloVeiculo();
    }

    this.LoadModeloVeiculo = function () {
        var menuOpcoes = {
            tipo: TypeOptionMenu.link, tamanho: 7, opcoes: [{
                descricao: Localization.Resources.Gerais.Geral.Excluir, id: guid(), metodo: function (data) {
                    var registros = $this.GridModeloVeiculo.BuscarRegistros();
                    for (var i = 0; i < registros.length; i++) {
                        if (registros[i].Codigo == data.Codigo) {
                            registros.splice(i, 1);
                            $this.GridModeloVeiculo.CarregarGrid(registros);
                            break;
                        }
                    }
                }
            }]
        };

        var header = [{ data: "Codigo", visible: false },
        { data: "Descricao", title: Localization.Resources.Gerais.Geral.Descricao, width: "80%" }];

        $this.GridModeloVeiculo = new BasicDataTable($this.PrevisaoDescarregamento.GridModeloVeiculo.id, header, menuOpcoes, { column: 1, dir: orderDir.asc }, null, 5);

        new BuscarModelosVeicularesCarga($this.PrevisaoDescarregamento.ModeloVeiculo, null, null, null, null, null, $this.GridModeloVeiculo);

        $this.PrevisaoDescarregamento.ModeloVeiculo.basicTable = $this.GridModeloVeiculo;
        $this.GridModeloVeiculo.CarregarGrid(new Array());
    }

    this.RecarregarGrid = function () {

        var data = new Array();

        $.each($this.Container.list, function (i, previsaoDescarregamento) {
            if (previsaoDescarregamento.DiaSemana.val == $this.DiaSemana) {
                var previsaoDescarregamentoGrid = new Object();

                previsaoDescarregamentoGrid.Codigo = previsaoDescarregamento.Codigo.val;
                previsaoDescarregamentoGrid.Rota = previsaoDescarregamento.Rota.val;
                previsaoDescarregamentoGrid.Descricao = previsaoDescarregamento.Descricao.val;
                previsaoDescarregamentoGrid.QuantidadeCargas = previsaoDescarregamento.QuantidadeCargas.val;
                previsaoDescarregamentoGrid.QuantidadeCargasExcedentes = previsaoDescarregamento.QuantidadeCargasExcedentes.val;
                data.push(previsaoDescarregamentoGrid);
            }
        });

        $this.Grid.CarregarGrid(data);
    }

    this.Editar = function (data) {
        for (var i = 0; i < $this.Container.list.length; i++) {
            if (data.Codigo == $this.Container.list[i].Codigo.val) {

                var informacao = $this.Container.list[i];
                $this.PrevisaoDescarregamento.Codigo.val(informacao.Codigo.val);
                $this.PrevisaoDescarregamento.DiaSemana.val(informacao.DiaSemana.val);
                $this.PrevisaoDescarregamento.Descricao.val(informacao.Descricao.val);
                $this.PrevisaoDescarregamento.QuantidadeCargas.val(informacao.QuantidadeCargas.val);
                $this.PrevisaoDescarregamento.QuantidadeCargasExcedentes.val(informacao.QuantidadeCargasExcedentes.val);
                $this.PrevisaoDescarregamento.Rota.codEntity(informacao.Rota.codEntity);
                $this.PrevisaoDescarregamento.Rota.val(informacao.Rota.val);

                var modelosExistentes = JSON.parse(informacao.ModelosVeiculos.val);
                $this.GridModeloVeiculo.CarregarGrid(modelosExistentes);

                break;
            }
        }

        $this.PrevisaoDescarregamento.Adicionar.visible(false);
        $this.PrevisaoDescarregamento.Atualizar.visible(true);
        $this.PrevisaoDescarregamento.Cancelar.visible(true);
        $this.PrevisaoDescarregamento.Excluir.visible(true);
    }

    this.Excluir = function (data) {
        for (var i = 0; i < $this.Container.list.length; i++) {
            if (data.Codigo.val() == $this.Container.list[i].Codigo.val) {
                $this.Container.list.splice(i, 1);
                break;
            }
        }

        $this.RecarregarGrid();
        $this.LimparCampos();
    }

    this.Cancelar = function () {
        $this.LimparCampos();
    }

    this.Adicionar = function () {
        var valido = ValidarCamposObrigatorios($this.PrevisaoDescarregamento);

        if (!valido) {
            exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.CamposObrigatorios, Localization.Resources.Gerais.Geral.InformeCamposObrigatorios);
            return;
        }

        for (var i = 0; i < $this.Container.list.length; i++) {
            if ($this.Container.list[i].DiaSemana.val == $this.DiaSemana &&
                $this.Container.list[i].Rota.codEntity == $this.PrevisaoDescarregamento.Rota.codEntity()) {

                if ($this.Container.list[i].ModelosVeiculos.list != null)
                    $this.Container.list[i].ModelosVeiculos.val = JSON.stringify($this.Container.list[i].ModelosVeiculos.list);

                var modelosExistentes = JSON.parse($this.Container.list[i].ModelosVeiculos.val);
                var modelosSalvar = $this.GridModeloVeiculo.BuscarRegistros();
                var existeModeloIgual = false;

                for (var x = 0; x < modelosSalvar.length; x++) {
                    for (var y = 0; y < modelosExistentes.length; y++) {
                        if (modelosSalvar[x].Codigo == modelosExistentes[y].Codigo) {
                            exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.APrevisaoJaExiste, Localization.Resources.Gerais.Geral.JaExistePrevisaoCadastradaRotaModeloVeiculo);
                            return;
                        }
                    }
                }
            }
        }

        $this.PrevisaoDescarregamento.Codigo.val(guid());

        var previsaoSalvar = SalvarListEntity($this.PrevisaoDescarregamento);

        previsaoSalvar.ModelosVeiculos.val = JSON.stringify($this.GridModeloVeiculo.BuscarRegistros());

        $this.Container.list.push(previsaoSalvar);

        $this.RecarregarGrid();
        $this.LimparCampos();
    }

    this.SalvarDia = function () {

        var data = RetornarObjetoPesquisa(_centroDescarregamento);
        data.CodigoCentroDescarregamento = _centroDescarregamento.Codigo.val();
        data.Mes = _centroDescarregamentoDiasNoMes.Mes.val();
        data.Dia = _centroDescarregamentoDiasNoMes.Dia.val();

        executarReST("CentroDescarregamento/SalvarCapacidadeCarregamento", data, function (arg) {
            if (arg.Success) {
                if (arg.Data) {
                    recarregarGridsCapacidadeDescarregamento();
                    exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, Localization.Resources.Gerais.Geral.SalvoComSucesso);
                }
                else
                    exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Atencao, arg.Msg);
            }
            else
                exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, arg.Msg);
        });
    }

    this.ImportarDeOutroDiaMes = function () {
        Global.abrirModal("divModalImportacaoPeriodoDiaMes");
    }

    this.ImportarParaOsDemaisDiasMes = function () {

        exibirConfirmacao(Localization.Resources.Gerais.Geral.AtencaoAcentuado, Localization.Resources.Logistica.CentroDescarregamento.DesejaRealmenteImportarDadosDe + ("00" + _centroDescarregamentoDiasNoMes.Dia.val()).slice(-2) + "/" + ("00" + _centroDescarregamentoDiasNoMes.Mes.val()).slice(-2) + Localization.Resources.Logistica.CentroDescarregamento.ParaDemaisDiasMesDadosExistenteDemaiDiasSeraoApagados, function () {
            var data = RetornarObjetoPesquisa(_centroDescarregamento);
            data.CodigoCentroDescarregamento = _centroDescarregamento.Codigo.val();
            data.Mes = _centroDescarregamentoDiasNoMes.Mes.val();
            data.Dia = _centroDescarregamentoDiasNoMes.Dia.val();
            data.Ano = new Date().getFullYear();

            executarReST("CentroDescarregamento/CopiarCapacidadeCarregamentoDiaParaMes", data, function (arg) {
                if (arg.Success) {
                    if (arg.Data) {
                        recarregarGridsCapacidadeDescarregamento();
                        exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, Localization.Resources.Gerais.Geral.SalvoComSucesso);
                    }
                    else
                        exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Atencao, arg.Msg);
                }
                else
                    exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, arg.Msg);
            });

            exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, Localization.Resources.Gerais.Geral.DadosimportadosComSucesso);
        });
    }

    this.Atualizar = function () {
        var valido = ValidarCamposObrigatorios($this.PrevisaoDescarregamento);
        if (!valido) {
            exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.CamposObrigatorios, Localization.Resources.Gerais.Geral.InformeCamposObrigatorios);
            return;
        }
        
        for (var i = 0; i < $this.Container.list.length; i++) {
            if (
                $this.Container.list[i].DiaSemana.val == $this.DiaSemana &&
                $this.Container.list[i].Rota.codEntity == $this.PrevisaoDescarregamento.Rota.codEntity() &&
                $this.Container.list[i].Codigo.val != $this.PrevisaoDescarregamento.Codigo.val()
            ) {

                if ($this.Container.list[i].ModelosVeiculos.list != null)
                    $this.Container.list[i].ModelosVeiculos.val = JSON.stringify($this.Container.list[i].ModelosVeiculos.list);

                var modelosExistentes = JSON.parse($this.Container.list[i].ModelosVeiculos.val);
                var modelosSalvar = $this.GridModeloVeiculo.BuscarRegistros();
                var existeModeloIgual = false;

                for (var x = 0; x < modelosSalvar.length; x++) {
                    for (var y = 0; y < modelosExistentes.length; y++) {
                        if (modelosSalvar[x].Codigo == modelosExistentes[y].Codigo) {
                            exibirMensagem(tipoMensagem.aviso, Localization.Resources.Logistica.CentroDescarregamento.APrevisaoJaExiste, Localization.Resources.Logistica.CentroDescarregamento.JaExistePrevisaoCadastradaRotaModeloVeiculo);
                            return;
                        }
                    }
                }
            }
        }
        

        var previsaoSalvar = SalvarListEntity($this.PrevisaoDescarregamento);
        previsaoSalvar.ModelosVeiculos.val = JSON.stringify($this.GridModeloVeiculo.BuscarRegistros());

        for (var i = 0; i < $this.Container.list.length; i++) {
            if ($this.Container.list[i].Codigo.val == $this.PrevisaoDescarregamento.Codigo.val()){
                $this.Container.list[i] = previsaoSalvar;
                break;
            }
        }

        $this.RecarregarGrid();
        $this.LimparCampos();
    }

    this.AtualizarDia = function () {
        var valido = ValidarCamposObrigatorios($this.PrevisaoDescarregamento);
        if (!valido) {
            exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.CamposObrigatorios, Localization.Resources.Gerais.Geral.InformeCamposObrigatorios);
            return;
        }

        for (var i = 0; i < $this.Container.list.length; i++) {
            if (
                $this.Container.list[i].DiaSemana.val == $this.DiaSemana &&
                $this.Container.list[i].Rota.codEntity == $this.PrevisaoDescarregamento.Rota.codEntity() &&
                $this.Container.list[i].Codigo.val != $this.PrevisaoDescarregamento.Codigo.val()
            ) {

                if ($this.Container.list[i].ModelosVeiculos.list != null)
                    $this.Container.list[i].ModelosVeiculos.val = JSON.stringify($this.Container.list[i].ModelosVeiculos.list);

                var modelosExistentes = JSON.parse($this.Container.list[i].ModelosVeiculos.val);
                var modelosSalvar = $this.GridModeloVeiculo.BuscarRegistros();
                var existeModeloIgual = false;

                for (var x = 0; x < modelosSalvar.length; x++) {
                    for (var y = 0; y < modelosExistentes.length; y++) {
                        if (modelosSalvar[x].Codigo == modelosExistentes[y].Codigo) {
                            exibirMensagem(tipoMensagem.aviso, Localization.Resources.Logistica.CentroDescarregamento.APrevisaoJaExiste, Localization.Resources.Logistica.CentroDescarregamento.JaExistePrevisaoCadastradaRotaModeloVeiculo);
                            return;
                        }
                    }
                }
            }
        }


        var previsaoSalvar = SalvarListEntity($this.PrevisaoDescarregamento);
        previsaoSalvar.ModelosVeiculos.val = JSON.stringify($this.GridModeloVeiculo.BuscarRegistros());

        for (var i = 0; i < $this.Container.list.length; i++) {
            if ($this.Container.list[i].Codigo.val == $this.PrevisaoDescarregamento.Codigo.val()) {
                $this.Container.list[i] = previsaoSalvar;
                break;
            }
        }

        $this.RecarregarGrid();
        $this.LimparCampos();
    }


    this.ImportarDeOutroDia = function () {
        $("#" + _importacaoPrevisao.DiaSemana.id + " option").attr("disabled", false);
        $("#" + _importacaoPrevisao.DiaSemana.id + " option[value='" + $this.DiaSemana + "']").attr("disabled", true);
        $("#" + _importacaoPrevisao.DiaSemana.id).find('option:enabled:first').prop('selected', true);
        $("#" + _importacaoPrevisao.DiaSemana.id).change();
        _diaParaImportarPrevisao = $this.DiaSemana;
        Global.abrirModal('divModalImportacaoPrevisao');
    }

    this.ImportarParaOsDemaisDias = function () {

        var diasParaImportar = new Array();

        exibirConfirmacao(Localization.Resources.Gerais.Geral.AtencaoAcentuado, Localization.Resources.Logistica.CentroDescarregamento.DesejaRealmenteImportarDadosDe + EnumDiaSemana.obterDescricaoResumida($this.DiaSemana) + Localization.Resources.Logistica.CentroDescarregamento.ParaDemaisDiasOsDadosExistentesDemaisDiasSeraoApagados, function () {

            for (var i = $this.Container.list.length - 1; i >= 0; i--) {
                if ($this.Container.list[i].DiaSemana.val != $this.DiaSemana) {
                    $this.Container.list.splice(i, 1);
                } else {
                    for (var x = 0; x < _listaKnockoutPrevisoesDescarregamento.length; x++) {
                        if (_listaKnockoutPrevisoesDescarregamento[x] != null && _listaKnockoutPrevisoesDescarregamento[x].DiaSemana != $this.DiaSemana) {
                            var diaImportar = $.extend(true, {}, $this.Container.list[i]);
                            diaImportar.DiaSemana.val = _listaKnockoutPrevisoesDescarregamento[x].DiaSemana;
                            diaImportar.Codigo.val = guid();
                            diasParaImportar.push(diaImportar);
                        }
                    }
                }
            }

            $this.Container.list = $this.Container.list.concat(diasParaImportar);

            for (var i = 0; i < _listaKnockoutPrevisoesDescarregamento.length; i++)
                if (_listaKnockoutPrevisoesDescarregamento[i] != null)
                    _listaKnockoutPrevisoesDescarregamento[i].RecarregarGrid();

            exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, Localization.Resources.Gerais.Geral.DadosimportadosComSucesso);
        });
    }

    this.LimparCampos = function () {
        LimparCampos($this.PrevisaoDescarregamento);
        $this.GridModeloVeiculo.CarregarGrid(new Array());

        $this.PrevisaoDescarregamento.Adicionar.visible(true);
        $this.PrevisaoDescarregamento.Atualizar.visible(false);
        $this.PrevisaoDescarregamento.Cancelar.visible(false);
        $this.PrevisaoDescarregamento.Excluir.visible(false);
    }
}