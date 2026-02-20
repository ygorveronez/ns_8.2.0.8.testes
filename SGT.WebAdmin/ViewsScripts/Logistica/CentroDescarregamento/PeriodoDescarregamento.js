/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Globais.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="../../Consultas/CanalVenda.js" />
/// <reference path="../../Consultas/Cliente.js" />
/// <reference path="../../Consultas/GrupoPessoa.js" />
/// <reference path="../../Consultas/TipoCarga.js" />
/// <reference path="../../Enumeradores/EnumDiaSemana.js" />
/// <reference path="CentroDescarregamento.js" />
/// <reference path="TipoCarga.js" />
/// <reference path="ImportacaoPeriodo.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var PeriodoDescarregamentoModel = function (instancia) {
    this.Grid = PropertyEntity({ type: types.local });

    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.DiaSemana = PropertyEntity({ val: ko.observable(instancia.DiaSemana), def: instancia.DiaSemana, getType: typesKnockout.int });
    this.Dia = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Mes = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.HoraInicio = PropertyEntity({ val: ko.observable(""), def: "", getType: typesKnockout.time, text: Localization.Resources.Gerais.Geral.Inicio.getRequiredFieldDescription(), required: true });
    this.HoraTermino = PropertyEntity({ val: ko.observable(""), def: "", getType: typesKnockout.time, text: Localization.Resources.Gerais.Geral.Termino.getRequiredFieldDescription(), required: true });
    this.SkuDe = PropertyEntity({ val: ko.observable(""), def: "", getType: typesKnockout.int, text: Localization.Resources.Logistica.CentroDescarregamento.QtdItensDe.getFieldDescription(), required: false });
    this.SkuAte = PropertyEntity({ val: ko.observable(""), def: "", getType: typesKnockout.int, text: Localization.Resources.Logistica.CentroDescarregamento.QtdItensAte.getFieldDescription(), required: false });
    this.CapacidadeDescarregamento = PropertyEntity({ val: ko.observable(""), def: "", getType: typesKnockout.int, text: Localization.Resources.Logistica.CentroDescarregamento.CapacidadeDescarregamento.getFieldDescription(), required: false, visible: ko.observable(false), maxlength: 15 });
    this.CapacidadeDescarregamentoSimultaneo = PropertyEntity({ val: ko.observable(""), def: "", getType: typesKnockout.int, text: Localization.Resources.Logistica.CentroDescarregamento.DescarregamentoSimultaneo.getRequiredFieldDescription(), required: true, visible: true, configInt: { precision: 0, allowZero: true, thousands: "" } });
    this.CapacidadeDescarregamentoSimultaneoAdicional = PropertyEntity({ val: ko.observable(""), def: "", getType: typesKnockout.int, text: Localization.Resources.Logistica.CentroDescarregamento.DescarregamentoSimultaneoAdicional.getFieldDescription(), visible: true, configInt: { precision: 0, allowZero: true, thousands: "" } });
    this.ToleranciaExcessoTempo = PropertyEntity({ val: ko.observable(""), def: "", getType: typesKnockout.int, text: Localization.Resources.Logistica.CentroDescarregamento.ExcessoTempo, issue: 324, visible: true });
    this.Remetentes = PropertyEntity({ type: types.map, text: Localization.Resources.Logistica.CentroDescarregamento.AdicionarRemetente, getType: typesKnockout.string, val: ko.observable(new Array()), def: new Array(), idBtnSearch: guid(), idGrid: guid(), visible: ko.observable(false) });
    this.TiposCarga = PropertyEntity({ type: types.map, text: Localization.Resources.Logistica.CentroDescarregamento.AdicionarTipoCarga, getType: typesKnockout.dynamic, val: ko.observable(new Array()), def: new Array(), idBtnSearch: guid(), idGrid: guid(), visible: ko.observable(false) });
    this.GruposPessoas = PropertyEntity({ type: types.map, text: Localization.Resources.Logistica.CentroDescarregamento.AdicionarGrupoPessoa, getType: typesKnockout.dynamic, val: ko.observable(new Array()), def: new Array(), idBtnSearch: guid(), idGrid: guid(), visible: ko.observable(false) });
    this.CanaisVenda = PropertyEntity({ type: types.map, text: Localization.Resources.Logistica.CentroDescarregamento.AdicionarCanalVenda, getType: typesKnockout.dynamic, val: ko.observable(new Array()), def: new Array(), idBtnSearch: guid(), idGrid: guid(), visible: ko.observable(false) });
    this.GrupoProduto = PropertyEntity({ type: types.map, text: Localization.Resources.Logistica.CentroDescarregamento.AdicionarGrupoProduto, getType: typesKnockout.dynamic, val: ko.observable(new Array()), def: new Array(), idBtnSearch: guid(), idGrid: guid(), visible: ko.observable(false) });

    this.Adicionar = PropertyEntity({ eventClick: instancia.Adicionar, type: types.event, text: Localization.Resources.Logistica.CentroDescarregamento.AdicionarPeriodo, visible: ko.observable(true) });
    this.SalvarDiaMes = PropertyEntity({ eventClick: instancia.SalvarDia, type: types.event, text: Localization.Resources.Gerais.Geral.Salvar, visible: ko.observable(true) });
    this.Atualizar = PropertyEntity({ eventClick: instancia.Atualizar, type: types.event, text: Localization.Resources.Gerais.Geral.Atualizar, visible: ko.observable(false) });
    this.Cancelar = PropertyEntity({ eventClick: instancia.Cancelar, type: types.event, text: Localization.Resources.Gerais.Geral.Cancelar, visible: ko.observable(false) });
    this.Excluir = PropertyEntity({ eventClick: instancia.Excluir, type: types.event, text: Localization.Resources.Gerais.Geral.Excluir, visible: ko.observable(false) });

    this.ImportarDeOutroDia = PropertyEntity({ eventClick: instancia.ImportarDeOutroDia, type: types.event, text: Localization.Resources.Logistica.CentroDescarregamento.ImportarOutroDia, visible: ko.observable(true) });
    this.ImportarParaOsDemaisDias = PropertyEntity({ eventClick: instancia.ImportarParaOsDemaisDias, type: types.event, text: Localization.Resources.Logistica.CentroDescarregamento.ImportarParaDemaisDias, visible: ko.observable(true) });

    this.ImportarDeOutroDiaMes = PropertyEntity({ eventClick: instancia.ImportarDeOutroDiaMes, type: types.event, text: Localization.Resources.Logistica.CentroDescarregamento.ImportarOutroDia, visible: ko.observable(true) });
    this.ImportarParaOsDemaisDiasMes = PropertyEntity({ eventClick: instancia.ImportarParaOsDemaisDiasMes, type: types.event, text: Localization.Resources.Logistica.CentroDescarregamento.ImportarParaDemaisDias, visible: ko.observable(true) });

    this.ImportarCapacidade = PropertyEntity({
        type: types.local,
        text: Localization.Resources.Logistica.CentroDescarregamento.ImportarPeriodo,
        visible: ko.observable(true),
        accept: ".xls,.xlsx,.csv",
        cssClass: "btn-default",
        UrlImportacao: "CentroDescarregamento/ImportarCapacidade",
        UrlConfiguracao: "CentroDescarregamento/ConfiguracaoImportacaoCapacidade",
        CodigoControleImportacao: EnumCodigoControleImportacao.O020_CargaDataCarregamentoImportacao,
        ParametrosRequisicao: function () {
            const parametros = { Codigo: _centroDescarregamento.Codigo.val() };
            return parametros;
        },
        CallbackImportacao: function (arg) {
            if (arg.Data.Retorno != null) 
                ObterCapacidadePorDiaMes();
        }
    });
}

var PeriodoDescarregamento = function (diaSemana, idKnockout, containerItens) {
    let $this = this;
    let gridPeriodoDescarregamentoCanaisVenda;
    let gridPeriodoDescarregamentoGruposPessoas;
    let gridPeriodoDescarregamentoRemetente;
    let gridPeriodoDescarregamentoTipoDeCarga;
    let gridPeriodoDescarregamentoGrupoProduto;
    
    $this.DiaSemana = diaSemana;
    $this.IdKnockout = idKnockout;
    $this.Container = containerItens;

    this.Load = function () {
        $this.PeriodoDescarregamento = new PeriodoDescarregamentoModel($this);
        KoBindings($this.PeriodoDescarregamento, $this.IdKnockout);

        const menuOpcoes = { tipo: TypeOptionMenu.link, tamanho: 7, opcoes: [{ descricao: Localization.Resources.Gerais.Geral.Editar, id: guid(), metodo: $this.Editar }] };

        const header = [
            { data: "Codigo", visible: false },
            { data: "HoraInicio", title: Localization.Resources.Gerais.Geral.Inicio, width: "8%" },
            { data: "HoraTermino", title: Localization.Resources.Gerais.Geral.Termino, width: "8%" },
            { data: "ResumoTiposCarga", title: Localization.Resources.Gerais.Geral.TiposCarga, width: "15%" },
            { data: "ResumoRemetentes", title: Localization.Resources.Logistica.CentroDescarregamento.Remetentes, width: "15%" },
            { data: "ResumoGruposPessoas", title: Localization.Resources.Gerais.Geral.GrupoPessoas, width: "15%" },
            { data: "ResumoGruposProdutos", title: Localization.Resources.Logistica.CentroDescarregamento.GruposProdutos, width: "15%" },
            { data: "ResumoCanaisVenda", title: Localization.Resources.Logistica.CentroDescarregamento.CanaisVenda, width: "15%" },
            { data: "CapacidadeDescarregamentoSimultaneo", title: Localization.Resources.Logistica.CentroDescarregamento.DescarregamentoSimultaneo, width: "10%" },
            { data: "CapacidadeDescarregamentoSimultaneoAdicional", title: Localization.Resources.Logistica.CentroDescarregamento.DescarregamentoSimultaneoAdicional, width: "10%" },
            { data: "CapacidadeDescarregamento", title: Localization.Resources.Gerais.Geral.Capacidade, width: "10%", visible: false },
            { data: "SKU", title: Localization.Resources.Logistica.CentroDescarregamento.QtdItens, width: "15%" }
        ];

        $this.Grid = new BasicDataTable($this.PeriodoDescarregamento.Grid.id, header, menuOpcoes, { column: 1, dir: orderDir.asc });
        $this.RecarregarGrid();

        loadGridPeriodoDescarregamentoCanaisVenda();
        loadGridPeriodoDescarregamentoGruposPessoas();
        loadGridPeriodoDescarregamentoRemetente();
        loadGridPeriodoDescarregamentoTipoDeCarga();
        loadGridPeriodoDescarregamentoGrupoProduto();
    }

    this.RecarregarGrid = function () {

        const data = new Array();

        $.each($this.Container.list, function (i, periodoDescarregamento) {
            if (periodoDescarregamento.DiaSemana.val == $this.DiaSemana) {
                let periodoDescarregamentoGrid = new Object();

                periodoDescarregamentoGrid.Codigo = periodoDescarregamento.Codigo.val;
                periodoDescarregamentoGrid.HoraInicio = periodoDescarregamento.HoraInicio.val;
                periodoDescarregamentoGrid.HoraTermino = periodoDescarregamento.HoraTermino.val;
                periodoDescarregamentoGrid.CapacidadeDescarregamentoSimultaneo = periodoDescarregamento.CapacidadeDescarregamentoSimultaneo.val;
                periodoDescarregamentoGrid.CapacidadeDescarregamentoSimultaneoAdicional = periodoDescarregamento.CapacidadeDescarregamentoSimultaneoAdicional.val;
                periodoDescarregamentoGrid.CapacidadeDescarregamento = periodoDescarregamento.CapacidadeDescarregamento.val;
                periodoDescarregamentoGrid.ToleranciaExcessoTempo = (periodoDescarregamento.ToleranciaExcessoTempo.val || "0") + " min.";

                let sku = "";
                if (periodoDescarregamento.SkuDe.val) sku += Localization.Resources.Gerais.Geral.De + periodoDescarregamento.SkuDe.val;
                if (periodoDescarregamento.SkuAte.val) sku += Localization.Resources.Gerais.Geral.Ate + periodoDescarregamento.SkuAte.val;

                periodoDescarregamentoGrid.SKU = sku.trim();
                periodoDescarregamentoGrid.ResumoCanaisVenda = obterDescricaoCanaisVenda(periodoDescarregamento.CanaisVenda.list);
                periodoDescarregamentoGrid.ResumoGruposPessoas = obterDescricaoGruposPessoas(periodoDescarregamento.GruposPessoas.list);
                periodoDescarregamentoGrid.ResumoRemetentes = obterDescricaoRemetentes(periodoDescarregamento.Remetentes.list);
                periodoDescarregamentoGrid.ResumoTiposCarga = obterDescricaoTiposCarga(periodoDescarregamento.TiposCarga.list);
                periodoDescarregamentoGrid.ResumoGruposProdutos = obterDescricaoGrupoProduto(periodoDescarregamento.GrupoProduto.list);

                data.push(periodoDescarregamentoGrid);
            }
        });

        $this.Grid.CarregarGrid(data);
    }

    this.Excluir = function (data) {
        for (let i = 0; i < $this.Container.list.length; i++) {
            if (data.Codigo.val() == $this.Container.list[i].Codigo.val) {
                $this.Container.list.splice(i, 1);
                break;
            }
        }

        $this.RecarregarGrid();
        $this.LimparCampos();
    }

    this.Editar = function (data) {
        let periodo = null;
        for (let i = 0; i < $this.Container.list.length; i++) {
            if (data.Codigo == $this.Container.list[i].Codigo.val) {
                periodo = $this.Container.list[i];
                break;
            }
        }

        if (periodo == null)
            return false;

        $this.LimparCampos();

        $this.PeriodoDescarregamento.Codigo.val(periodo.Codigo.val);
        $this.PeriodoDescarregamento.DiaSemana.val(periodo.DiaSemana.val);
        $this.PeriodoDescarregamento.HoraInicio.val(periodo.HoraInicio.val);
        $this.PeriodoDescarregamento.HoraTermino.val(periodo.HoraTermino.val);
        $this.PeriodoDescarregamento.SkuDe.val(periodo.SkuDe.val);
        $this.PeriodoDescarregamento.SkuAte.val(periodo.SkuAte.val);
        $this.PeriodoDescarregamento.CapacidadeDescarregamentoSimultaneo.val(periodo.CapacidadeDescarregamentoSimultaneo.val);
        $this.PeriodoDescarregamento.CapacidadeDescarregamentoSimultaneoAdicional.val(periodo.CapacidadeDescarregamentoSimultaneoAdicional.val);
        $this.PeriodoDescarregamento.ToleranciaExcessoTempo.val(periodo.ToleranciaExcessoTempo.val);
        $this.PeriodoDescarregamento.CapacidadeDescarregamento.val(periodo.CapacidadeDescarregamento.val);

        $this.PeriodoDescarregamento.Adicionar.visible(false);
        $this.PeriodoDescarregamento.Atualizar.visible(true);
        $this.PeriodoDescarregamento.Cancelar.visible(true);
        $this.PeriodoDescarregamento.Excluir.visible(true);

        recarregarGridPeriodoDescarregamentoCanaisVenda(periodo.CanaisVenda.list);
        recarregarGridPeriodoDescarregamentoGruposPessoas(periodo.GruposPessoas.list);
        recarregarGridPeriodoDescarregamentoRemetente(periodo.Remetentes.list);
        recarregarGridPeriodoDescarregamentoTipoDeCarga(periodo.TiposCarga.list);
        recarregarGridPeriodoDescarregamentoGrupoProduto(periodo.GrupoProduto.list);
    }

    this.Cancelar = function () {
        $this.LimparCampos();
    }

    this.Atualizar = function () {
        const valido = ValidarCamposObrigatorios($this.PeriodoDescarregamento);

        if (!valido) {
            exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.CamposObrigatorios, Localization.Resources.Gerais.Geral.InformeCamposObrigatorios);
            return;
        }

        if (!$this.ValidarCampos())
            return;

        if (!$this.ValidarRepetidos($this.PeriodoDescarregamento.Codigo.val()))
            return;

        for (let i = 0; i < $this.Container.list.length; i++) {
            if ($this.Container.list[i].DiaSemana.val == $this.DiaSemana) {

                if ($this.PeriodoDescarregamento.Codigo.val() == $this.Container.list[i].Codigo.val) {
                    $this.Container.list[i] = obterPeriodoSalvar();
                    break;
                }
            }
        }

        $this.RecarregarGrid();
        $this.LimparCampos();
    }

    this.Adicionar = function () {
        const valido = ValidarCamposObrigatorios($this.PeriodoDescarregamento);

        if (!valido) {
            exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.CamposObrigatorios, Localization.Resources.Gerais.Geral.InformeCamposObrigatorios);
            return;
        }

        if (!$this.ValidarCampos())
            return;

        if (!$this.ValidarRepetidos($this.PeriodoDescarregamento.Codigo.val()))
            return;

        $this.PeriodoDescarregamento.Codigo.val(guid());
        $this.Container.list.push(obterPeriodoSalvar());

        $this.RecarregarGrid();
        $this.LimparCampos();
        }

    this.SalvarDia = function () {

        const data = RetornarObjetoPesquisa(_centroDescarregamento);
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


        exibirConfirmacao(Localization.Resources.Gerais.Geral.AtencaoAcentuado, Localization.Resources.Logistica.CentroDescarregamento.DesejaRealmenteImportarDadoDe + ("00" + _centroDescarregamentoDiasNoMes.Dia.val()).slice(-2) + "/" + ("00" + _centroDescarregamentoDiasNoMes.Mes.val()).slice(-2) + " para os demais dias do mês? Os dados existentes nos demais dias serão apagados.", function () {

            const data = RetornarObjetoPesquisa(_centroDescarregamento);
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

    

    this.ImportarDeOutroDia = function () {
        $("#" + _importacaoPeriodo.DiaSemana.id + " option").attr("disabled", false);
        $("#" + _importacaoPeriodo.DiaSemana.id + " option[value='" + $this.DiaSemana + "']").attr("disabled", true);
        $("#" + _importacaoPeriodo.DiaSemana.id).find('option:enabled:first').prop('selected', true);
        $("#" + _importacaoPeriodo.DiaSemana.id).change();

        // Varíavel de ImportacaoPeriodo.js
        _diaParaImportarPeriodo = $this.DiaSemana;

        Global.abrirModal("divModalImportacaoPeriodo");
    }

    this.ImportarParaOsDemaisDias = function () {

        const diasParaImportar = new Array();

        exibirConfirmacao(Localization.Resources.Gerais.Geral.AtencaoAcentuado, Localization.Resources.Logistica.CentroDescarregamento.DesejaRealmenteImportarDadoDe + EnumDiaSemana.obterDescricaoResumida($this.DiaSemana) + Localization.Resources.Logistica.CentroDescarregamento.ParaDemaisDiasOsDadosExistentesDemaisDiasSeraoApagados, function () {

            for (let i = $this.Container.list.length - 1; i >= 0; i--) {
                if ($this.Container.list[i].DiaSemana.val != $this.DiaSemana) {
                    $this.Container.list.splice(i, 1);
                } else {
                    for (let x = 0; x < _listaKnockoutPeriodosDescarregamento.length; x++) {
                        if (_listaKnockoutPeriodosDescarregamento[x] != null && _listaKnockoutPeriodosDescarregamento[x].DiaSemana != $this.DiaSemana) {
                            let diaImportar = $.extend(true, {}, $this.Container.list[i]);
                            diaImportar.DiaSemana.val = _listaKnockoutPeriodosDescarregamento[x].DiaSemana;
                            diaImportar.Codigo.val = guid();
                            diasParaImportar.push(diaImportar);
                        }
                    }
                }
            }

            $this.Container.list = $this.Container.list.concat(diasParaImportar);

            for (let i = 0; i < _listaKnockoutPeriodosDescarregamento.length; i++)
                if (_listaKnockoutPeriodosDescarregamento[i] != null)
                    _listaKnockoutPeriodosDescarregamento[i].RecarregarGrid();

            exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, Localization.Resources.Gerais.Geral.DadosImportadosComSucesso);
        });
    }

    this.LimparCampos = function () {
        LimparCampos($this.PeriodoDescarregamento);

        $this.PeriodoDescarregamento.Adicionar.visible(true);
        $this.PeriodoDescarregamento.Atualizar.visible(false);
        $this.PeriodoDescarregamento.Cancelar.visible(false);
        $this.PeriodoDescarregamento.Excluir.visible(false);

        limparGridPeriodoDescarregamentoCanaisVenda();
        limparGridPeriodoDescarregamentoGruposPessoas();
        limparGridPeriodoDescarregamentoRemetente();
        limparGridPeriodoDescarregamentoTipoDeCarga();
        limparGridPeriodoDescarregamentoGrupoProduto();
    }

    this.ControlarExibicaoCapacidadeDescarregamento = function (exibir) {
        $this.PeriodoDescarregamento.CapacidadeDescarregamento.visible(exibir);
        $this.Grid.ControlarExibicaoColuna("CapacidadeDescarregamento", exibir);

        if (!exibir)
            $this.PeriodoDescarregamento.CapacidadeDescarregamento.val("");
    }

    this.ValidarCampos = function () {
        const skuDe = parseInt($this.PeriodoDescarregamento.SkuDe.val()) || 0;
        const skuAte = parseInt($this.PeriodoDescarregamento.SkuAte.val());

        if (!isNaN(skuAte) && skuDe > skuAte) {
            exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.FaixaItens, "O valor de \"Qtd. de Itens De\" deve ser menor que \"Qtd. de Itens Até\".");
            return false;
        }

        return true;
    }

    this.ValidarRepetidos = function (codigo) {
        return true; //Validar posteriormente incluindo tipo de carta e remetentes
        //codigo = codigo || 0;

        //var horaInicio = moment($this.PeriodoDescarregamento.HoraInicio.val(), "HH:mm");
        //var horaTermino = moment($this.PeriodoDescarregamento.HoraTermino.val(), "HH:mm");

        //for (var i = 0; i < $this.Container.list.length; i++) {
        //    if ($this.Container.list[i].DiaSemana.val == $this.DiaSemana) {
        //        var horaInicioGrid = moment($this.Container.list[i].HoraInicio.val, "HH:mm");
        //        var horaTerminoGrid = moment($this.Container.list[i].HoraTermino.val, "HH:mm");

        //        if (
        //            ($this.Container.list[i].Codigo.val != codigo || codigo == 0) && (
        //                horaInicio.isBetween(horaInicioGrid, horaTerminoGrid) ||
        //                horaTermino.isBetween(horaInicioGrid, horaTerminoGrid) ||
        //                horaInicio.diff(horaInicioGrid) == 0 ||
        //                horaInicio.diff(horaTerminoGrid) == 0 ||
        //                horaTermino.diff(horaTerminoGrid) == 0 ||
        //                horaTermino.diff(horaInicioGrid) == 0)
        //        ) {
        //            exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.PeriodoJaExiste, Localization.Resources.Gerais.Geral.OPeriodoEntrouConflitoPeriodoJaExistenteDe + $this.Container.list[i].HoraInicio.val + Localization.Resources.Gerais.Geral.Ate + $this.Container.list[i].HoraTermino.val + ".");
        //            return false;
        //        }
        //    }
        //}

        //return true;
    }

    const limparGridPeriodoDescarregamentoCanaisVenda = function () {
        gridPeriodoDescarregamentoCanaisVenda.CarregarGrid([]);
    }

    const limparGridPeriodoDescarregamentoGruposPessoas = function () {
        gridPeriodoDescarregamentoTipoDeCarga.CarregarGrid([]);
    }

    const limparGridPeriodoDescarregamentoRemetente = function () {
        gridPeriodoDescarregamentoRemetente.CarregarGrid([]);
    }

    const limparGridPeriodoDescarregamentoTipoDeCarga = function () {
        gridPeriodoDescarregamentoTipoDeCarga.CarregarGrid([]);
    }

    const limparGridPeriodoDescarregamentoGrupoProduto = function () {
        gridPeriodoDescarregamentoGrupoProduto.CarregarGrid([]);
    }

    const loadGridPeriodoDescarregamentoCanaisVenda = function () {
        const linhasPorPaginas = 5;
        const opcaoRemover = { descricao: Localization.Resources.Gerais.Geral.Remover, id: guid(), metodo: removerPeriodoDescarregamentoCanaisVenda, icone: "" };
        const menuOpcoes = { tipo: TypeOptionMenu.link, tamanho: 21, opcoes: [opcaoRemover] };

        const header = [
            { data: "Codigo", visible: false },
            { data: "Descricao", title: Localization.Resources.Gerais.Geral.Descricao, width: "75%", className: "text-align-left" }
        ];

        gridPeriodoDescarregamentoCanaisVenda = new BasicDataTable($this.PeriodoDescarregamento.CanaisVenda.idGrid, header, menuOpcoes, null, null, linhasPorPaginas);

        BuscarCanaisVenda($this.PeriodoDescarregamento.CanaisVenda, null, gridPeriodoDescarregamentoCanaisVenda);

        gridPeriodoDescarregamentoCanaisVenda.CarregarGrid([]);
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

        gridPeriodoDescarregamentoRemetente = new BasicDataTable($this.PeriodoDescarregamento.Remetentes.idGrid, header, menuOpcoes, null, null, linhasPorPaginas);

        BuscarClientes($this.PeriodoDescarregamento.Remetentes, null, null, null, null, gridPeriodoDescarregamentoRemetente);

        gridPeriodoDescarregamentoRemetente.CarregarGrid([]);
    }

    const loadGridPeriodoDescarregamentoTipoDeCarga = function () {
        const linhasPorPaginas = 5;
        const opcaoRemover = { descricao: Localization.Resources.Gerais.Geral.Remover, id: guid(), metodo: removerPeriodoDescarregamentoTipoDeCarga, icone: "" };
        const menuOpcoes = { tipo: TypeOptionMenu.link, tamanho: 21, opcoes: [opcaoRemover] };

        const header = [
            { data: "Codigo", visible: false },
            { data: "Descricao", title: Localization.Resources.Gerais.Geral.Descricao, width: "75%", className: "text-align-left" }
        ];

        gridPeriodoDescarregamentoTipoDeCarga = new BasicDataTable($this.PeriodoDescarregamento.TiposCarga.idGrid, header, menuOpcoes, null, null, linhasPorPaginas);

        BuscarTiposdeCarga($this.PeriodoDescarregamento.TiposCarga, null, null, gridPeriodoDescarregamentoTipoDeCarga);

        gridPeriodoDescarregamentoTipoDeCarga.CarregarGrid([]);
    }

    const loadGridPeriodoDescarregamentoGrupoProduto = function () {
        const linhasPorPaginas = 5;
        const opcaoRemover = { descricao: Localization.Resources.Gerais.Geral.Remover, id: guid(), metodo: removerPeriodoDescarregamentoGrupoProduto, icone: "" };
        const menuOpcoes = { tipo: TypeOptionMenu.link, tamanho: 21, opcoes: [opcaoRemover] };

        const header = [
            { data: "Codigo", visible: false },
            { data: "Descricao", title: Localization.Resources.Gerais.Geral.Descricao, width: "75%", className: "text-align-left" }
        ];

        gridPeriodoDescarregamentoGrupoProduto = new BasicDataTable($this.PeriodoDescarregamento.GrupoProduto.idGrid, header, menuOpcoes, null, null, linhasPorPaginas);

        BuscarGruposProdutos($this.PeriodoDescarregamento.GrupoProduto, null, gridPeriodoDescarregamentoGrupoProduto);

        gridPeriodoDescarregamentoGrupoProduto.CarregarGrid([]);
    }

    const obterDescricaoCanaisVenda = function (canaisVenda) {
        let lista = "";
        for (let i = 0; i < canaisVenda.length; i++) {
            if (lista != "")
                lista = lista + ", ";

            lista = lista + canaisVenda[i].Descricao.val;
        }

        return lista;
    }

    const obterDescricaoGruposPessoas = function (gruposPessoas) {
        let lista = "";
        for (let i = 0; i < gruposPessoas.length; i++) {
            if (lista != "")
                lista = lista + ", ";

            lista = lista + gruposPessoas[i].Descricao.val;
        }

        return lista;
    }

    const obterDescricaoRemetentes = function (remetentes) {
        let lista = "";
        for (let i = 0; i < remetentes.length; i++) {
            if (lista != "")
                lista = lista + ", ";

            lista = lista + remetentes[i].Descricao.val;
        }

        return lista;
    }

    const obterDescricaoTiposCarga = function (tiposCarga) {
        let lista = "";
        for (let i = 0; i < tiposCarga.length; i++) {
            if (lista != "")
                lista = lista + ", ";

            lista = lista + tiposCarga[i].Descricao.val;
        }

        return lista;
    }

    const obterDescricaoGrupoProduto = function (gruposProduto) {
        let lista = "";
        for (let i = 0; i < gruposProduto.length; i++) {
            if (lista != "")
                lista = lista + ", ";

            lista = lista + gruposProduto[i].Descricao.val;
        }

        return lista;
    }

    const obterListaPeriodoDescarregamentoCanaisVenda = function () {
        return gridPeriodoDescarregamentoCanaisVenda.BuscarRegistros();
    }

    const obterListaPeriodoDescarregamentoGruposPessoas = function () {
        return gridPeriodoDescarregamentoGruposPessoas.BuscarRegistros();
    }

    const obterListaPeriodoDescarregamentoRemetente = function () {
        return gridPeriodoDescarregamentoRemetente.BuscarRegistros();
    }

    const obterListaPeriodoDescarregamentoTipoDeCarga = function () {
        return gridPeriodoDescarregamentoTipoDeCarga.BuscarRegistros();
    }

    const obterListaPeriodoDescarregamentoGrupoProduto = function () {
        return gridPeriodoDescarregamentoGrupoProduto.BuscarRegistros();
    }

    const obterPeriodoDescarregamentoCanaisVendaSalvar = function () {
        const listaCanaisVenda = obterListaPeriodoDescarregamentoCanaisVenda();
        const listaRetornar = new Array();

        listaCanaisVenda.forEach(function (canalVenda) {
            listaRetornar.push({
                Codigo: { val: canalVenda.Codigo, getType: "int", type: "map" },
                Descricao: { val: canalVenda.Descricao, getType: "string", type: "map" }
            });
        });

        return listaRetornar;
    }

    const obterPeriodoDescarregamentoGruposPessoasSalvar = function () {
        const listaGruposPessoas = obterListaPeriodoDescarregamentoGruposPessoas();
        const listaRetornar = new Array();

        listaGruposPessoas.forEach(function (grupoPessoa) {
            listaRetornar.push({
                Codigo: { val: grupoPessoa.Codigo, getType: "int", type: "map" },
                Descricao: { val: grupoPessoa.Descricao, getType: "string", type: "map" }
            });
        });

        return listaRetornar;
    }

    const obterPeriodoDescarregamentoRemetenteSalvar = function () {
        const listaRemetente = obterListaPeriodoDescarregamentoRemetente();
        const listaRetornar = new Array();

        listaRemetente.forEach(function (remetente) {
            listaRetornar.push({
                Codigo: { val: remetente.Codigo, getType: "int", type: "map" },
                Descricao: { val: remetente.Descricao, getType: "string", type: "map" }
            });
        });

        return listaRetornar;
    }

    const obterPeriodoDescarregamentoTipoDeCargaSalvar = function () {
        const listaTiposCarga = obterListaPeriodoDescarregamentoTipoDeCarga();
        const listaRetornar = new Array();

        listaTiposCarga.forEach(function (tipoCarga) {
            listaRetornar.push({
                Codigo: { val: tipoCarga.Codigo, getType: "int", type: "map" },
                Descricao: { val: tipoCarga.Descricao, getType: "string", type: "map" }
            });
        });

        return listaRetornar;
    }

    const obterPeriodoDescarregamentoGrupoProdutoSalvar = function () {
        const listaGruposProduto = obterListaPeriodoDescarregamentoGrupoProduto();
        const listaRetornar = new Array();

        listaGruposProduto.forEach(function (grupoProduto) {
            listaRetornar.push({
                Codigo: { val: grupoProduto.Codigo, getType: "int", type: "map" },
                Descricao: { val: grupoProduto.Descricao, getType: "string", type: "map" }
            });
        });

        return listaRetornar;
    }

    const obterPeriodoSalvar = function () {
        const periodo = SalvarListEntity($this.PeriodoDescarregamento);;

        periodo["CanaisVenda"] = new PropertyEntity({ val: $this.PeriodoDescarregamento.CanaisVenda.val(), getType: $this.PeriodoDescarregamento.CanaisVenda.getType, list: obterPeriodoDescarregamentoCanaisVendaSalvar(), type: types.listEntity });
        periodo["GruposPessoas"] = new PropertyEntity({ val: $this.PeriodoDescarregamento.GruposPessoas.val(), getType: $this.PeriodoDescarregamento.GruposPessoas.getType, list: obterPeriodoDescarregamentoGruposPessoasSalvar(), type: types.listEntity });
        periodo["Remetentes"] = new PropertyEntity({ val: $this.PeriodoDescarregamento.Remetentes.val(), getType: $this.PeriodoDescarregamento.Remetentes.getType, list: obterPeriodoDescarregamentoRemetenteSalvar(), type: types.listEntity });
        periodo["TiposCarga"] = new PropertyEntity({ val: $this.PeriodoDescarregamento.TiposCarga.val(), getType: $this.PeriodoDescarregamento.TiposCarga.getType, list: obterPeriodoDescarregamentoTipoDeCargaSalvar(), type: types.listEntity });
        periodo["GrupoProduto"] = new PropertyEntity({ val: $this.PeriodoDescarregamento.GrupoProduto.val(), getType: $this.PeriodoDescarregamento.GrupoProduto.getType, list: obterPeriodoDescarregamentoGrupoProdutoSalvar(), type: types.listEntity });
        periodo["Dia"] = new PropertyEntity({ val: _centroDescarregamentoDiasNoMes.Dia.val(), def: 0, getType: $this.PeriodoDescarregamento.Dia.getType, type: types.int })
        periodo["Mes"] = new PropertyEntity({ val: _centroDescarregamentoDiasNoMes.Mes.val(), def: 0, getType: $this.PeriodoDescarregamento.Mes.getType, type: types.int })

        return periodo;
    }

    const recarregarGridPeriodoDescarregamentoCanaisVenda = function (canaisVenda) {
        const dados = new Array();

        $.each(canaisVenda, function (i, canalVenda) {
            dados.push({
                Codigo: canalVenda.Codigo.val,
                Descricao: canalVenda.Descricao.val
            });
        });

        gridPeriodoDescarregamentoCanaisVenda.CarregarGrid(dados);
    }

    const recarregarGridPeriodoDescarregamentoGruposPessoas = function (gruposPessoas) {
        const dados = new Array();

        $.each(gruposPessoas, function (i, grupoPessoa) {
            dados.push({
                Codigo: grupoPessoa.Codigo.val,
                Descricao: grupoPessoa.Descricao.val
            });
        });

        gridPeriodoDescarregamentoGruposPessoas.CarregarGrid(dados);
    }

    const recarregarGridPeriodoDescarregamentoRemetente = function (remetentes) {
        const dados = new Array();

        $.each(remetentes, function (i, remetente) {
            dados.push({
                Codigo: remetente.Codigo.val,
                Descricao: remetente.Descricao.val
            });
        });

        gridPeriodoDescarregamentoRemetente.CarregarGrid(dados);
    }

    const recarregarGridPeriodoDescarregamentoTipoDeCarga = function (tiposCarga) {
        const dados = new Array();

        $.each(tiposCarga, function (i, tipoCarga) {
            dados.push({
                Codigo: tipoCarga.Codigo.val,
                Descricao: tipoCarga.Descricao.val
            });
        });

        gridPeriodoDescarregamentoTipoDeCarga.CarregarGrid(dados);
    }

    const recarregarGridPeriodoDescarregamentoGrupoProduto = function (gruposProduto) {
        const dados = new Array();

        $.each(gruposProduto, function (i, grupoPessoa) {
            dados.push({
                Codigo: grupoPessoa.Codigo.val,
                Descricao: grupoPessoa.Descricao.val
            });
        });

        gridPeriodoDescarregamentoGrupoProduto.CarregarGrid(dados);
    }

    const removerPeriodoDescarregamentoCanaisVenda = function (registroSelecionado) {
        const lista = obterListaPeriodoDescarregamentoCanaisVenda();

        for (let i = 0; i < lista.length; i++) {
            if (registroSelecionado.Codigo == lista[i].Codigo) {
                lista.splice(i, 1);
                break;
            }
        }

        gridPeriodoDescarregamentoCanaisVenda.CarregarGrid(lista);
    }

    const removerPeriodoDescarregamentoGrupoProduto = function (registroSelecionado) {
        const lista = obterListaPeriodoDescarregamentoGrupoProduto();

        for (let i = 0; i < lista.length; i++) {
            if (registroSelecionado.Codigo == lista[i].Codigo) {
                lista.splice(i, 1);
                break;
            }
        }

        gridPeriodoDescarregamentoGrupoProduto.CarregarGrid(lista);
    }

    const removerPeriodoDescarregamentoGruposPessoas = function (registroSelecionado) {
        const lista = obterListaPeriodoDescarregamentoGruposPessoas();

        for (let i = 0; i < lista.length; i++) {
            if (registroSelecionado.Codigo == lista[i].Codigo) {
                lista.splice(i, 1);
                break;
            }
        }

        gridPeriodoDescarregamentoGruposPessoas.CarregarGrid(lista);
    }

    const removerPeriodoDescarregamentoRemetente = function (registroSelecionado) {
        const lista = obterListaPeriodoDescarregamentoRemetente();

        for (let i = 0; i < lista.length; i++) {
            if (registroSelecionado.Codigo == lista[i].Codigo) {
                lista.splice(i, 1);
                break;
            }
        }

        gridPeriodoDescarregamentoRemetente.CarregarGrid(lista);
    }

    const removerPeriodoDescarregamentoTipoDeCarga = function (registroSelecionado) {
        const lista = obterListaPeriodoDescarregamentoTipoDeCarga();

        for (let i = 0; i < lista.length; i++) {
            if (registroSelecionado.Codigo == lista[i].Codigo) {
                lista.splice(i, 1);
                break;
            }
        }

        gridPeriodoDescarregamentoTipoDeCarga.CarregarGrid(lista);
    }
}
