/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Globais.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="../../Consultas/TipoCarga.js" />
/// <reference path="../../Enumeradores/EnumDiaSemana.js" />
/// <reference path="CentroCarregamento.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var LimiteCarregamentoModel = function (instancia) {
    this.Grid = PropertyEntity({ type: types.local });

    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.DiaSemana = PropertyEntity({ val: ko.observable(instancia.DiaSemana), def: instancia.DiaSemana, getType: typesKnockout.int });
    this.DiasAntecedencia = PropertyEntity({ val: ko.observable(""), def: "", text: Localization.Resources.Logistica.CentroCarregamento.DiasDeAntecedencia.getRequiredFieldDescription(), getType: typesKnockout.int, configInt: { precision: 0, allowZero: true }, maxlength: 2, required: true });
    this.HoraLimite = PropertyEntity({ val: ko.observable(""), def: "", getType: typesKnockout.time, text: Localization.Resources.Logistica.CentroCarregamento.HoraLimite.getRequiredFieldDescription(), required: true });
    this.TipoCarga = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Logistica.CentroCarregamento.TipoDeCarga.getRequiredFieldDescription(), idBtnSearch: guid(), required: true });

    this.Adicionar = PropertyEntity({ eventClick: instancia.Adicionar, type: types.event, text: Localization.Resources.Gerais.Geral.Adicionar, visible: ko.observable(true) });
}

var LimiteCarregamento = function (diaSemana, idKnockout, containerItens) {

    var $this = this;

    $this.DiaSemana = diaSemana;
    $this.IdKnockout = idKnockout;
    $this.Container = containerItens;

    this.Load = function () {
        $this.LimiteCarregamento = new LimiteCarregamentoModel($this);
        KoBindings($this.LimiteCarregamento, $this.IdKnockout);

        new BuscarTiposdeCarga($this.LimiteCarregamento.TipoCarga);

        var menuOpcoes = { tipo: TypeOptionMenu.link, tamanho: 7, opcoes: [{ descricao: Localization.Resources.Gerais.Geral.Excluir, id: guid(), metodo: $this.Excluir }] };

        var header = [
            { data: "Codigo", visible: false },
            { data: "TipoCarga", title: Localization.Resources.Logistica.CentroCarregamento.TipoDeCarga, width: "40%" },
            { data: "DiasAntecedencia", title: Localization.Resources.Logistica.CentroCarregamento.DiasDeAntecedencia, width: "20%" },
            { data: "HoraLimite", title: Localization.Resources.Logistica.CentroCarregamento.HoraLimite, width: "20%" }
        ];

        $this.Grid = new BasicDataTable($this.LimiteCarregamento.Grid.id, header, menuOpcoes, { column: 1, dir: orderDir.asc });

        $this.RecarregarGrid();
    }

    this.RecarregarGrid = function () {
        var data = new Array();

        $.each($this.Container.list, function (i, LimiteCarregamento) {
            if (LimiteCarregamento.DiaSemana.val == $this.DiaSemana) {
                var LimiteCarregamentoGrid = new Object();

                LimiteCarregamentoGrid.Codigo = LimiteCarregamento.Codigo.val;
                LimiteCarregamentoGrid.DiasAntecedencia = LimiteCarregamento.DiasAntecedencia.val;
                LimiteCarregamentoGrid.HoraLimite = LimiteCarregamento.HoraLimite.val;
                LimiteCarregamentoGrid.TipoCarga = LimiteCarregamento.TipoCarga.val;

                data.push(LimiteCarregamentoGrid);
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

    this.Adicionar = function () {
        var valido = ValidarCamposObrigatorios($this.LimiteCarregamento);

        if (!valido) {
            exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.CamposObrigatorios, Localization.Resources.Gerais.Geral.InformeCamposObrigatorios);
            return;
        }


        $this.LimiteCarregamento.Codigo.val(guid());
        $this.Container.list.push(SalvarListEntity($this.LimiteCarregamento));

        $this.RecarregarGrid();
        $this.LimparCampos();
    }

    this.LimparCampos = function () {
        LimparCampos($this.LimiteCarregamento);
    }
}