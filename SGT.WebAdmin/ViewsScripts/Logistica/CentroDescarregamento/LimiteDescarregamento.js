/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Globais.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="../../Consultas/TipoCarga.js" />
/// <reference path="../../Enumeradores/EnumDiaSemana.js" />
/// <reference path="CentroDescarregamento.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var LimiteDescarregamentoModel = function (instancia) {
    this.Grid = PropertyEntity({ type: types.local });

    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.DiaSemana = PropertyEntity({ val: ko.observable(instancia.DiaSemana), def: instancia.DiaSemana, getType: typesKnockout.int });
    this.Dia = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Mes = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.HorasAntecedencia = PropertyEntity({ val: ko.observable(""), def: "", text: Localization.Resources.Logistica.CentroDescarregamento.HorasAntecedencia.getRequiredFieldDescription(), getType: typesKnockout.int, maxlength: 3, required: true });
    this.TipoCarga = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Gerais.Geral.TipoCarga.getRequiredFieldDescription(), idBtnSearch: guid(), required: true });

    this.Adicionar = PropertyEntity({ eventClick: instancia.Adicionar, type: types.event, text: Localization.Resources.Logistica.CentroDescarregamento.AdicionarLimite, visible: ko.observable(true) });
    this.SalvarDiaMes = PropertyEntity({ eventClick: instancia.SalvarDia, type: types.event, text: Localization.Resources.Gerais.Geral.Salvar, visible: ko.observable(true) });

    this.ImportarDeOutroDiaMes = PropertyEntity({ eventClick: instancia.ImportarDeOutroDiaMes, type: types.event, text: Localization.Resources.Logistica.CentroDescarregamento.ImportarOutroDia, visible: ko.observable(true) });
    this.ImportarParaOsDemaisDiasMes = PropertyEntity({ eventClick: instancia.ImportarParaOsDemaisDiasMes, type: types.event, text: Localization.Resources.Logistica.CentroDescarregamento.ImportarParaDemaisDias, visible: ko.observable(true) });
    
}

var LimiteDescarregamento = function (diaSemana, idKnockout, containerItens) {

    var $this = this;

    $this.DiaSemana = diaSemana;
    $this.IdKnockout = idKnockout;
    $this.Container = containerItens;

    this.Load = function () {
        $this.LimiteDescarregamento = new LimiteDescarregamentoModel($this);
        KoBindings($this.LimiteDescarregamento, $this.IdKnockout);

        new BuscarTiposdeCarga($this.LimiteDescarregamento.TipoCarga);

        var menuOpcoes = { tipo: TypeOptionMenu.link, tamanho: 7, opcoes: [{ descricao: Localization.Resources.Gerais.Geral.Excluir, id: guid(), metodo: $this.Excluir }] };

        var header = [
            { data: "Codigo", visible: false },
            { data: "TipoCarga", title: Localization.Resources.Gerais.Geral.TipoCarga, width: "50%" },
            { data: "HorasAntecedencia", title: Localization.Resources.Logistica.CentroDescarregamento.HorasAntecedencia, width: "30%" }
        ];

        $this.Grid = new BasicDataTable($this.LimiteDescarregamento.Grid.id, header, menuOpcoes, { column: 1, dir: orderDir.asc });

        $this.RecarregarGrid();
    }

    this.RecarregarGrid = function () {
        var data = new Array();

        $.each($this.Container.list, function (i, LimiteDescarregamento) {
            if (LimiteDescarregamento.DiaSemana.val == $this.DiaSemana) {
                var LimiteDescarregamentoGrid = new Object();

                LimiteDescarregamentoGrid.Codigo = LimiteDescarregamento.Codigo.val;
                LimiteDescarregamentoGrid.HorasAntecedencia = LimiteDescarregamento.HorasAntecedencia.val;
                LimiteDescarregamentoGrid.TipoCarga = LimiteDescarregamento.TipoCarga.val;

                data.push(LimiteDescarregamentoGrid);
            }
        });

        $this.Grid.CarregarGrid(data);
    }

    this.SalvarDia = function () {

        var data = RetornarObjetoPesquisa(_centroDescarregamento);
        data.CodigoCentroDescarregamento = _centroDescarregamento.Codigo.val();
        data.Dia = _centroDescarregamentoDiasNoMes.Dia.val();
        data.Mes = _centroDescarregamentoDiasNoMes.Mes.val();

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

            exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, Localization.Resources.Gerais.Geral.DadosImportadosComSucesso);
        });
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
        var valido = ValidarCamposObrigatorios($this.LimiteDescarregamento);

        if (!valido) {
            exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.CamposObrigatorios, Localization.Resources.Gerais.Geral.InformeCamposObrigatorios);
            return;
        }


        $this.LimiteDescarregamento.Codigo.val(guid());
        $this.Container.list.push(SalvarListEntity($this.LimiteDescarregamento));

        $this.RecarregarGrid();
        $this.LimparCampos();
    }

    this.LimparCampos = function () {
        LimparCampos($this.LimiteDescarregamento);
    }
}