/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Globais.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="../../Consultas/RotaFrete.js" />
/// <reference path="../../Consultas/ModeloVeicularCarga.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var PrevisaoCarregamentoModel = function (instancia) {
    this.Grid = PropertyEntity({ type: types.local });
    this.GridModeloVeiculo = PropertyEntity({ type: types.local });

    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Descricao = PropertyEntity({ val: ko.observable(""), def: "", maxlength: 150, text: Localization.Resources.Gerais.Geral.Descricao.getRequiredFieldDescription(), required: true });
    this.QuantidadeCargas = PropertyEntity({ val: ko.observable(""), def: "", getType: typesKnockout.int, maxlength: 3, text: Localization.Resources.Logistica.ExcecaoCapacidadeCarregamento.QuantidadeCargas.getRequiredFieldDescription(), required: true, configInt: { precision: 0, allowZero: true } });
    this.QuantidadeCargasExcedentes = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int, maxlength: 3, text: Localization.Resources.Logistica.ExcecaoCapacidadeCarregamento.QtdCargasExcedentes.getRequiredFieldDescription(), required: true, configInt: { precision: 0, allowZero: true } });
    this.Rota = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Logistica.ExcecaoCapacidadeCarregamento.Rota.getRequiredFieldDescription(), idBtnSearch: guid(), required: true });
    this.ModeloVeiculo = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Logistica.ExcecaoCapacidadeCarregamento.AdicionarModeloVeiculo, idBtnSearch: guid(), enable: true });
    this.ModelosVeiculos = PropertyEntity({ type: types.map, getType: typesKnockout.dynamic, val: ko.observable(""), def: "", idGrid: guid() });

    this.Adicionar = PropertyEntity({ eventClick: instancia.Adicionar, type: types.event, text: Localization.Resources.Gerais.Geral.Adicionar, visible: ko.observable(true) });
}

var PrevisaoCarregamento = function (idKnockout, containerItens) {
    var $this = this;

    $this.IdKnockout = idKnockout;
    $this.Container = containerItens;

    this.Load = function () {
        $this.PrevisaoCarregamento = new PrevisaoCarregamentoModel($this);
        KoBindings($this.PrevisaoCarregamento, $this.IdKnockout);

        new BuscarRotasFrete($this.PrevisaoCarregamento.Rota);

        var menuOpcoes = { tipo: TypeOptionMenu.link, tamanho: 7, opcoes: [{ descricao: Localization.Resources.Gerais.Geral.Excluir, id: guid(), metodo: $this.Excluir }] };

        var header = [
            { data: "Codigo", visible: false },
            { data: "Rota", title: Localization.Resources.Logistica.ExcecaoCapacidadeCarregamento.Rota, width: "25%" },
            { data: "Descricao", title: Localization.Resources.Gerais.Geral.Descricao, width: "25%" },
            { data: "QuantidadeCargas", title: Localization.Resources.Logistica.ExcecaoCapacidadeCarregamento.QuantidadeCargas, width: "18%" },
            { data: "QuantidadeCargasExcedentes", title: Localization.Resources.Logistica.ExcecaoCapacidadeCarregamento.CargasExcedentes, width: "18%" }
        ];

        $this.Grid = new BasicDataTable($this.PrevisaoCarregamento.Grid.id, header, menuOpcoes, { column: 1, dir: orderDir.asc });

        $this.RecarregarGrid();

        $this.LoadModeloVeiculo();
    }

    this.LoadModeloVeiculo = function () {
        var menuOpcoes = {
            tipo: TypeOptionMenu.link, tamanho: 7, opcoes: [{
                descricao: "Excluir", id: guid(), metodo: function (data) {
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

        var header = [
            { data: "Codigo", visible: false },
            { data: "Descricao", title: Localization.Resources.Gerais.Geral.Descricao, width: "80%" }
        ];

        $this.GridModeloVeiculo = new BasicDataTable($this.PrevisaoCarregamento.GridModeloVeiculo.id, header, menuOpcoes, { column: 1, dir: orderDir.asc }, null, 5);

        new BuscarModelosVeicularesCarga($this.PrevisaoCarregamento.ModeloVeiculo, null, null, null, null, null, $this.GridModeloVeiculo);

        $this.PrevisaoCarregamento.ModeloVeiculo.basicTable = $this.GridModeloVeiculo;
        $this.GridModeloVeiculo.CarregarGrid(new Array());
    }

    this.RecarregarGrid = function () {

        var data = new Array();

        $.each($this.Container.list, function (i, previsaoCarregamento) {
            var previsaoCarregamentoGrid = new Object();

            previsaoCarregamentoGrid.Codigo = previsaoCarregamento.Codigo.val;
            previsaoCarregamentoGrid.Rota = previsaoCarregamento.Rota.val;
            previsaoCarregamentoGrid.Descricao = previsaoCarregamento.Descricao.val;
            previsaoCarregamentoGrid.QuantidadeCargas = previsaoCarregamento.QuantidadeCargas.val;
            previsaoCarregamentoGrid.QuantidadeCargasExcedentes = previsaoCarregamento.QuantidadeCargasExcedentes.val;
            data.push(previsaoCarregamentoGrid);
        });

        $this.Grid.CarregarGrid(data);
    }

    this.Editar = function (data) {
        for (var i = 0; i < $this.Container.list.length; i++) {
            if (data.Codigo == $this.Container.list[i].Codigo.val) {

                var informacao = $this.Container.list[i];
                $this.PrevisaoCarregamento.Codigo.val(informacao.Codigo.val);
                $this.PrevisaoCarregamento.Descricao.val(informacao.Descricao.val);
                $this.PrevisaoCarregamento.QuantidadeCargas.val(informacao.QuantidadeCargas.val);
                $this.PrevisaoCarregamento.QuantidadeCargasExcedentes.val(informacao.QuantidadeCargasExcedentes.val);
                $this.PrevisaoCarregamento.Rota.codEntity(informacao.Rota.codEntity);
                $this.PrevisaoCarregamento.Rota.val(informacao.Rota.val);

                var modelos = new Array();
                for (var x = 0; x < informacao.ModelosVeiculos.list.length; x++) {
                    var modelo = { Codigo: informacao.ModelosVeiculos.list[x].Codigo.val, Descricao: informacao.ModelosVeiculos.list[x].Descricao.val };
                    modelos.push(modelo);
                }
                $this.GridModeloVeiculo.CarregarGrid(modelos);

                break;
            }
        }

        $this.RecarregarGrid();
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

    this.Adicionar = function () {
        var valido = ValidarCamposObrigatorios($this.PrevisaoCarregamento);

        if (!valido) {
            exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.CamposObrigatorios, "Informe os campos obrigatórios!");
            return;
        }

        for (var i = 0; i < $this.Container.list.length; i++) {
            if ($this.Container.list[i].Rota.codEntity == $this.PrevisaoCarregamento.Rota.codEntity()) {

                if ($this.Container.list[i].ModelosVeiculos.list != null)
                    $this.Container.list[i].ModelosVeiculos.val = JSON.stringify($this.Container.list[i].ModelosVeiculos.list);

                var modelosExistentes = JSON.parse($this.Container.list[i].ModelosVeiculos.val);
                var modelosSalvar = $this.GridModeloVeiculo.BuscarRegistros();
                var existeModeloIgual = false;

                for (var x = 0; x < modelosSalvar.length; x++) {
                    for (var y = 0; y < modelosExistentes.length; y++) {
                        if (modelosSalvar[x].Codigo == modelosExistentes[y].Codigo) {
                            exibirMensagem(tipoMensagem.aviso, Localization.Resources.Logistica.ExcecaoCapacidadeCarregamento.PrevisaoJaExiste, "Já existe uma previsão cadastrada para esta rota e modelo de veículo.");
                            return;
                        }
                    }
                }
            }
        }

        $this.PrevisaoCarregamento.Codigo.val(guid());

        var previsaoSalvar = SalvarListEntity($this.PrevisaoCarregamento);
        previsaoSalvar.ModelosVeiculos.val = JSON.stringify($this.GridModeloVeiculo.BuscarRegistros());

        $this.Container.list.push(previsaoSalvar);

        $this.RecarregarGrid();
        $this.LimparCampos();
    }

    this.LimparCampos = function () {
        LimparCampos($this.PrevisaoCarregamento);
        $this.GridModeloVeiculo.CarregarGrid(new Array());
    }
}