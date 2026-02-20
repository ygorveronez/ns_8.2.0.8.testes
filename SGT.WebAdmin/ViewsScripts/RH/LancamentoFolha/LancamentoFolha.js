/// <reference path="../../Consultas/Cliente.js" />
/// <reference path="../../Consultas/Empresa.js" />
/// <reference path="../../Consultas/TipoMovimento.js" />
/// <reference path="../../Consultas/TipoPagamentoRecebimento.js" />
/// <reference path="../../Consultas/Usuario.js" />
/// <reference path="../../Consultas/FolhaInformacao.js" />
/// <reference path="../../../js/libs/jquery-2.1.1.js" />
/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/bootstrap/bootstrap.js" />
/// <reference path="../../../js/libs/jquery.blockui.js" />
/// <reference path="../../../js/Global/knoutViewsSlides.js" />
/// <reference path="../../../js/libs/jquery.maskMoney.js" />
/// <reference path="../../../js/plugin/datatables/jquery.dataTables.js" />
/// <reference path="../../../js/libs/jquery.twbsPagination.js" />
/// <reference path="../../../js/libs/jquery.globalize.js" />
/// <reference path="../../../js/libs/jquery.globalize.pt-BR.js" />
/// <reference path="../../Enumeradores/EnumTipoTitulo.js" />
/// <reference path="../../Enumeradores/EnumPeriodicidade.js" />
/// <reference path="../../Enumeradores/EnumFinalidadeTipoMovimento.js" />
/// <reference path="../../Enumeradores/EnumFormaTitulo.js" />
/// <reference path="../RateioDespesaVeiculo/RateioDespesaVeiculo.js" />

var _HTMLLancamentoFolha = "";
var _gridTitulosSimulacao;

var _periodo = [
    { text: "Mensal", value: EnumPeriodicidade.Mensal },
    { text: "Semanal", value: EnumPeriodicidade.Semanal },
    { text: "Bimestral", value: EnumPeriodicidade.Bimestral },
    { text: "Trimestral", value: EnumPeriodicidade.Trimestral },
    { text: "Semestral", value: EnumPeriodicidade.Semestral },
    { text: "Anual", value: EnumPeriodicidade.Anual }
];

var LancamentoFolha = function () {
    this.Codigo = PropertyEntity({ getType: typesKnockout.int, enable: ko.observable(true) });
    this.IdModal = PropertyEntity({ getType: typesKnockout.string, enable: ko.observable(true), val: ko.observable("") });

    this.DataEmissao = PropertyEntity({ getType: typesKnockout.date, text: "*Data de Emissão:", required: true, enable: ko.observable(true) });
    this.DataCompetencia = PropertyEntity({ getType: typesKnockout.date, text: "Data de Competência:", required: false, enable: ko.observable(true) });

    this.ValorBase = PropertyEntity({ text: "*Valor Base: ", required: true, getType: typesKnockout.decimal, visible: ko.observable(true), enable: ko.observable(true) });
    this.ValorReferencia = PropertyEntity({ text: "Valor Referência: ", required: false, getType: typesKnockout.decimal, visible: ko.observable(true), enable: ko.observable(true) });
    this.Valor = PropertyEntity({ text: "Valor: ", required: false, getType: typesKnockout.decimal, visible: ko.observable(false), enable: ko.observable(true) });

    this.Descricao = PropertyEntity({ text: "*Descrição:", required: true, maxlength: 500 });
    this.NumeroEvento = PropertyEntity({ getType: typesKnockout.int, text: "*Número Evento:", required: true, visible: ko.observable(true), enable: ko.observable(true) });
    this.NumeroContrato = PropertyEntity({ getType: typesKnockout.int, text: "Número Contrato:", required: false, visible: ko.observable(true), enable: ko.observable(true) });

    this.Funcionario = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "*Funcionário:", idBtnSearch: guid(), required: true, enable: ko.observable(true) });
    this.FolhaInformacao = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "*Informação da Folha:", idBtnSearch: guid(), required: true, enable: ko.observable(true) });
    
    this.Repetir = PropertyEntity({ val: ko.observable(false), getType: typesKnockout.bool, text: ko.observable("Deseja REPETIR o lançamento? Isso gerará mais parcelas do mesmo valor informado."), def: false });
    this.Dividir = PropertyEntity({ val: ko.observable(false), getType: typesKnockout.bool, text: ko.observable("Deseja DIVIDIR o lançamento? Isso gerará mais parcelas com o valor informado dividido entre as parcelas."), def: false });    
    this.TipoRepetir = PropertyEntity({ val: ko.observable(EnumPeriodicidade.Mensal), def: EnumPeriodicidade.Mensal, options: _periodo, text: "Repetição:", required: false, visible: ko.observable(false), enable: ko.observable(true) });

    this.NumeroOcorrencia = PropertyEntity({ getType: typesKnockout.int, text: "Número Ocorrência:", visible: ko.observable(false), enable: ko.observable(true) });
    this.DiaVencimento = PropertyEntity({ getType: typesKnockout.int, text: "Dia do Vencimento:", visible: ko.observable(false), enable: ko.observable(true) });
    this.SimularParcelas = PropertyEntity({ val: ko.observable(false), getType: typesKnockout.bool, text: "Deseja simular/editar as parcelas?", def: false, visible: ko.observable(false) });
    
    this.Titulos = PropertyEntity({ type: types.map, val: ko.observable(""), list: new Array(), visible: ko.observable(false) });
    this.ListaTitulos = PropertyEntity({ type: types.map, getType: typesKnockout.dynamic, val: ko.observable(""), def: "", idGrid: guid() });
    
    this.SelecionarTodos = PropertyEntity({ val: ko.observable(true), def: true, type: types.map, getType: typesKnockout.bool, text: "Marcar/Desmarcar Todos", visible: ko.observable(false), enable: ko.observable(true) });

    this.Salvar = PropertyEntity({ type: types.event, text: "Salvar", visible: ko.observable(true), enable: ko.observable(true) });
    this.Cancelar = PropertyEntity({ type: types.event, text: "Cancelar", visible: ko.observable(true), enable: ko.observable(true) });
};

function LancarFolhas(callbackInit) {

    let lancarFolhas = this;
    let valorJuroAnterior = 0;
    let valorDescontoAnterior = 0;

    this.LoadLancarFolhas = function () {
        lancarFolhas.IdModal = (guid());
        lancarFolhas.IdKnockoutLancarConta = "knockoutLancamentoFolha_" + lancarFolhas.IdModal;

        lancarFolhas.LancamentoFolha = new LancamentoFolha();
        lancarFolhas.LancamentoFolha.IdModal.val(lancarFolhas.IdModal);
        lancarFolhas.LancamentoFolha.DataEmissao.val(moment().format("DD/MM/YYYY"));

        lancarFolhas.RenderizarModalContas(lancarFolhas, callbackInit);
    };

    this.FecharModal = function () {
        lancarFolhas.Modal.hide();
    };

    this.Destroy = function () {
        lancarFolhas.Modal.dispose();
        $("#" + lancarFolhas.IdModal).remove();
        lancarFolhas = null;
    };

    this.HabilitarCamposDividir = function () {
        lancarFolhas.LancamentoFolha.Repetir.val(false);

        if (lancarFolhas.LancamentoFolha.Dividir.val() === false) {
            lancarFolhas.LancamentoFolha.TipoRepetir.val(EnumPeriodicidade.Mensal);
            lancarFolhas.LancamentoFolha.NumeroOcorrencia.val("");
            lancarFolhas.LancamentoFolha.DiaVencimento.val("");
            lancarFolhas.LancamentoFolha.SimularParcelas.val(false);

            lancarFolhas.LancamentoFolha.TipoRepetir.visible(false);
            lancarFolhas.LancamentoFolha.NumeroOcorrencia.visible(false);
            lancarFolhas.LancamentoFolha.DiaVencimento.visible(false);
            lancarFolhas.LancamentoFolha.SimularParcelas.visible(false);
        } else {
            lancarFolhas.LancamentoFolha.TipoRepetir.visible(true);
            lancarFolhas.LancamentoFolha.NumeroOcorrencia.visible(true);
            lancarFolhas.LancamentoFolha.DiaVencimento.visible(true);
            lancarFolhas.LancamentoFolha.SimularParcelas.visible(true);
        }
    };

    this.HabilitarCamposRepetir = function () {
        lancarFolhas.LancamentoFolha.Dividir.val(false);

        if (lancarFolhas.LancamentoFolha.Repetir.val() === false) {
            lancarFolhas.LancamentoFolha.TipoRepetir.val(EnumPeriodicidade.Mensal);
            lancarFolhas.LancamentoFolha.NumeroOcorrencia.val("");
            lancarFolhas.LancamentoFolha.DiaVencimento.val("");
            lancarFolhas.LancamentoFolha.SimularParcelas.val(false);

            lancarFolhas.LancamentoFolha.TipoRepetir.visible(false);
            lancarFolhas.LancamentoFolha.NumeroOcorrencia.visible(false);
            lancarFolhas.LancamentoFolha.DiaVencimento.visible(false);
            lancarFolhas.LancamentoFolha.SimularParcelas.visible(false);
        } else {
            lancarFolhas.LancamentoFolha.TipoRepetir.visible(true);
            lancarFolhas.LancamentoFolha.NumeroOcorrencia.visible(true);
            lancarFolhas.LancamentoFolha.DiaVencimento.visible(true);
            lancarFolhas.LancamentoFolha.SimularParcelas.visible(true);
        }
    };

    this.retornoFolhaInformacao = function (data) {
        lancarFolhas.LancamentoFolha.FolhaInformacao.val(data.Descricao);
        lancarFolhas.LancamentoFolha.FolhaInformacao.codEntity(data.Codigo);
        lancarFolhas.LancamentoFolha.NumeroEvento.val(data.CodigoIntegracao);
    }

    this.HabilitarCamposSimular = function () {

        if (lancarFolhas.LancamentoFolha.SimularParcelas.val() === false) {
            lancarFolhas.LancamentoFolha.Titulos.visible(false);
        } else {
            let valido = ValidarCamposObrigatorios(lancarFolhas.LancamentoFolha);
            lancarFolhas.LancamentoFolha.NumeroOcorrencia.requiredClass("form-control");

            if (lancarFolhas.LancamentoFolha.Repetir.val() === true || lancarFolhas.LancamentoFolha.Dividir.val() === true) {
                if (lancarFolhas.LancamentoFolha.TipoRepetir.val() === "" || lancarFolhas.LancamentoFolha.NumeroOcorrencia.val() <= 0) {
                    valido = false;
                    exibirMensagem(tipoMensagem.atencao, "Campos Obrigatórios", "Favor informe o Tipo e também o número de ocorrências (parcelas)!");
                    lancarFolhas.LancamentoFolha.TipoRepetir.requiredClass("form-control ");
                    lancarFolhas.LancamentoFolha.NumeroOcorrencia.requiredClass("form-control is-invalid");
                }
            }

            if (valido) {
                lancarFolhas.LancamentoFolha.Titulos.visible(true);
                lancarFolhas.LancamentoFolha.SelecionarTodos.val(true);

                let objeto = ObterObjetoContas(lancarFolhas);
                let dados = { Conta: objeto };
                executarReST("LancamentoFolha/PesquisaTitulos", dados, function (arg) {
                    if (arg.Success) {
                        recarregarGridTitulosSimulacao(arg.Data, lancarFolhas);
                    } else {
                        exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
                    }
                });
            } else {
                lancarFolhas.LancamentoFolha.Titulos.visible(false);
                lancarFolhas.LancamentoFolha.SimularParcelas.val(false);
            }
        }
    };

    this.AdicionarTitulos = function () {
        let valido = ValidarCamposObrigatorios(lancarFolhas.LancamentoFolha);        
        lancarFolhas.LancamentoFolha.TipoRepetir.requiredClass("form-control");
        lancarFolhas.LancamentoFolha.NumeroOcorrencia.requiredClass("form-control");       

        if (valido) {           
            if (lancarFolhas.LancamentoFolha.Repetir.val() === true || lancarFolhas.LancamentoFolha.Dividir.val() === true) {
                if (lancarFolhas.LancamentoFolha.TipoRepetir.val() === "" || lancarFolhas.LancamentoFolha.NumeroOcorrencia.val() <= 0) {
                    valido = false;
                    exibirMensagem(tipoMensagem.atencao, "Campos Obrigatórios", "Favor informe o Tipo e também o número de ocorrências (parcelas)!");
                    lancarFolhas.LancamentoFolha.TipoRepetir.requiredClass("form-control is-invalid");
                    lancarFolhas.LancamentoFolha.NumeroOcorrencia.requiredClass("form-control is-invalid");
                    return;
                }
            }
        }

        PreencherListaTitulosSimulacao(lancarFolhas);
        let objeto = ObterObjetoContas(lancarFolhas);
        if (valido) {
            let dados = { Conta: objeto, ListaTitulos: lancarFolhas.LancamentoFolha.ListaTitulos.val() };
            executarReST("LancamentoFolha/Salvar", dados, function (arg) {
                if (arg.Success) {
                    lancarFolhas.FecharModal();
                    if (arg.Data) {
                        exibirMensagem(tipoMensagem.ok, "Sucesso", "Folha(s) salvo(s) com sucesso");
                    } else {
                        exibirMensagem(tipoMensagem.atencao, "Atenção", arg.Msg, 60000);
                    }
                } else {
                    exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
                }
            });
        }
        else
            exibirMensagem("atencao", "Campos Obrigatórios", "Por favor, informe os campos obrigatórios.");
    };

    this.SalvarClick = function () {
        let valido = ValidarCamposObrigatorios(lancarFolhas.LancamentoFolha);

        if (valido) {
            lancarFolhas.AdicionarTitulos();
        } else
            exibirMensagem(tipoMensagem.atencao, "Campos Obrigatórios", "Informe os campos obrigatórios!");
    };

    this.RenderizarModalContas = function () {
        lancarFolhas.ObterHTMLLancamentoFolhas().then(function () {
            let html = _HTMLLancamentoFolha.replace(/#divModalLancamentoFolha/g, lancarFolhas.IdModal);
            $('#js-page-content').append(html);
            lancarFolhas.LancamentoFolha.Cancelar.eventClick = function (e) {
                lancarFolhas.FecharModal();
            };
            lancarFolhas.LancamentoFolha.Salvar.eventClick = function (e) {
                lancarFolhas.SalvarClick();
            };

            KoBindings(lancarFolhas.LancamentoFolha, lancarFolhas.IdKnockoutLancarConta);

            lancarFolhas.Modal = new bootstrap.Modal(document.getElementById(lancarFolhas.IdModal), { backdrop: 'static' });

            $("#" + lancarFolhas.LancamentoFolha.Repetir.id).click(lancarFolhas.HabilitarCamposRepetir);
            $("#" + lancarFolhas.LancamentoFolha.Dividir.id).click(lancarFolhas.HabilitarCamposDividir);
            $("#" + lancarFolhas.LancamentoFolha.SimularParcelas.id).click(lancarFolhas.HabilitarCamposSimular);

            new BuscarFuncionario(lancarFolhas.LancamentoFolha.Funcionario);
            new BuscarFolhaInformacao(lancarFolhas.LancamentoFolha.FolhaInformacao, lancarFolhas.retornoFolhaInformacao);

            lancarFolhas.Modal.show();

            $('#' + lancarFolhas.IdModal).one('hidden.bs.modal', function () {
                lancarFolhas.Destroy();
            });            

            let editarRespostaTitulo = {
                permite: true,
                callback: lancarFolhas.SalvarRetornoTituloGrid,
                atualizarRow: true
            };

            let _editableDecimalConfig = {
                editable: true,
                type: EnumTipoColunaEditavelGrid.decimal,
                numberMask: ConfigDecimal({ allowZero: true })
            };

            let _editableStringConfig = {
                editable: true,
                type: EnumTipoColunaEditavelGrid.string
            };

            let _editableDateConfig = {
                editable: true,
                type: EnumTipoColunaEditavelGrid.data
            };

            let _editableIntConfig = {
                editable: true,
                type: EnumTipoColunaEditavelGrid.int,
                numberMask: ConfigInt()
            };

            let header = [
                { data: "Codigo", visible: false },
                { data: "Sequencia", title: "Sequência", width: "10%", editableCell: _editableIntConfig },
                { data: "Descricao", title: "Descrição", width: "20%", editableCell: _editableStringConfig },
                { data: "NumeroEvento", title: "Nº Evento", width: "10%", editableCell: _editableIntConfig },
                { data: "DataEmissao", title: "Emissão", width: "15%", editableCell: _editableDateConfig },
                { data: "DataCompetencia", title: "Competência", width: "15%", editableCell: _editableDateConfig },                
                { data: "ValorBase", title: "Base", width: "10%", editableCell: _editableDecimalConfig },
                { data: "ValorReferencia", title: "Referência", width: "10%", editableCell: _editableDecimalConfig },
                { data: "Valor", title: "Valor", width: "10%", editableCell: _editableDecimalConfig }
            ];

            _gridTitulosSimulacao = new BasicDataTable(lancarFolhas.LancamentoFolha.Titulos.id, header, null, { column: 1, dir: orderDir.asc }, null, 5000, null, null, editarRespostaTitulo);

            if (callbackInit !== null && callbackInit !== undefined) {
                callbackInit();
            }
        });
    };

    this.SalvarRetornoTituloGrid = function (dataRow) {
        let data = lancarFolhas.GetTitulos();

        for (var i in data) {
            if (data[i].Codigo === dataRow.Codigo) {
                data[i].Sequencia = dataRow.Sequencia;
                data[i].Descricao = dataRow.Descricao;
                data[i].NumeroEvento = dataRow.NumeroEvento;
                data[i].DataEmissao = dataRow.DataEmissao;
                data[i].DataCompetencia = dataRow.DataCompetencia;
                data[i].ValorBase = dataRow.ValorBase;
                data[i].ValorReferencia = dataRow.ValorReferencia;
                data[i].Valor = dataRow.Valor;
                break;
            }
        }

        lancarFolhas.SetTitulos(data);
    };

    this.GetTitulos = function () {
        return lancarFolhas.LancamentoFolha.Titulos.list.slice();
    };

    this.SetTitulos = function (data) {
        return lancarFolhas.LancamentoFolha.Titulos.list = data.slice();
    };

    this.ObterHTMLLancamentoFolhas = function () {
        let p = new promise.Promise();
        if (string.IsNullOrWhiteSpace(_HTMLLancamentoFolha)) {
            $.get("Content/Static/RH/LancamentoFolha.html?dyn=" + guid(), function (data) {
                _HTMLLancamentoFolha = data;
                p.done();
            });
        } else {
            p.done();
        }
        return p;
    };

    setTimeout(function () {
        lancarFolhas.LoadLancarFolhas();
    }, 50);
}

function ObterObjetoContas(lancarFolhas) {
    let lancarConta = RetornarObjetoPesquisa(lancarFolhas.LancamentoFolha);

    return JSON.stringify(lancarConta);
}

function PreencherListaTitulosSimulacao(lancarFolhas) {
    let listaTitulo = new Array();

    $.each(_gridTitulosSimulacao.BuscarRegistros(), function (i, titulo) {
        listaTitulo.push({ Titulo: titulo });
    });

    lancarFolhas.LancamentoFolha.ListaTitulos.val(JSON.stringify(listaTitulo));
}

function recarregarGridTitulosSimulacao(data, lancarFolhas) {
    let dataGrid = new Array();

    $.each(data.ListaTitulos, function (i, titulo) {
        let obj = new Object();
        obj.DT_Enable = true;
        obj.Codigo = titulo.Codigo;
        obj.Sequencia = titulo.Sequencia;
        obj.Descricao = titulo.Descricao;
        obj.NumeroEvento = titulo.NumeroEvento;
        obj.DataEmissao = titulo.DataEmissao;
        obj.DataCompetencia = titulo.DataCompetencia;
        obj.ValorBase = titulo.ValorBase;
        obj.ValorReferencia = titulo.ValorReferencia;
        obj.Valor = titulo.Valor;

        dataGrid.push(obj);
    });
    lancarFolhas.SetTitulos(dataGrid);
    _gridTitulosSimulacao.CarregarGrid(dataGrid);
}