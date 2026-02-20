var _anulacaoCTe = null,
    _componentesAnulacaoCTe = null,
    _crudAnulacaoCTe = null;

var AnulacaoCTe = function () {
    this.CodigoCTe = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.CodigoCarga = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.DataEventoDesacordo = PropertyEntity({ text: "*Data do Evento de Desacordo:", val: ko.observable(""), def: "", getType: typesKnockout.date, required: true });
    this.ValorCTeSubstituicao = PropertyEntity({ text: "Valor do CT-e de Substituição:", val: ko.observable("0,00"), def: "0,00", getType: typesKnockout.decimal, configDecimal: { precision: 2, allowZero: true, allowNegative: false }, visible: ko.observable(true) });

    this.TomadorCTeSubstituto = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Alterar Tomador do CT-e Substituto:", idBtnSearch: guid(), visible: ko.observable(true) });
    this.ObservacaoCTeAnulacao = PropertyEntity({ text: "Observação para o CT-e de Anulação: ", maxlength: 1000, visible: ko.observable(true) });
    this.ObservacaoCTeSubstituicao = PropertyEntity({ text: "Observação para o CT-e de Substituição: ", maxlength: 1000, visible: ko.observable(true) });

    this.ComponentesFrete = PropertyEntity({ types: typesKnockout.dynamic, val: ko.observable(""), def: "" });
};

var CRUDAnulacaoCTe = function () {
    this.Confirmar = PropertyEntity({ eventClick: ConfirmarAnulacaoCTeClick, type: types.event, text: "Confirmar", visible: ko.observable(true), enable: ko.observable(true) });
    this.ConfirmarAnulacaoSemSubstituicao = PropertyEntity({ eventClick: ConfirmarAnulacaoSemSubstituicaoClick, type: types.event, text: "Confirmar", visible: ko.observable(true), enable: ko.observable(true) });
    this.ConfirmarSubstituicao = PropertyEntity({ eventClick: ConfirmarSubstituicaoClick, type: types.event, text: "Confirmar Substituição", visible: ko.observable(false), enable: ko.observable(true) });
    this.Cancelar = PropertyEntity({ eventClick: FecharTelaAnulacaoCTe, type: types.event, text: "Cancelar", visible: ko.observable(true), enable: ko.observable(true) });
};

function LoadAnulacaoCTe() {
    _anulacaoCTe = new AnulacaoCTe();
    KoBindings(_anulacaoCTe, "knockoutAnulacaoCTe");

    _crudAnulacaoCTe = new CRUDAnulacaoCTe();
    KoBindings(_crudAnulacaoCTe, "knockoutCRUDAnulacaoCTe");

    _componentesAnulacaoCTe = new ComponentePrestacaoServicoAnulacao(_anulacaoCTe);
    _componentesAnulacaoCTe.Load();

    new BuscarClientes(_anulacaoCTe.TomadorCTeSubstituto);
}

function AbrirTelaAnulacaoCTe(e) {
    LimparCampos(_anulacaoCTe);
    _anulacaoCTe.CodigoCTe.val(e.CodigoCTE);
    _anulacaoCTe.CodigoCarga.val(_cargaCTe.Carga.val());
    _anulacaoCTe.TomadorCTeSubstituto.visible(true);
    _anulacaoCTe.ValorCTeSubstituicao.visible(true);
    _anulacaoCTe.ObservacaoCTeAnulacao.visible(true);
    _anulacaoCTe.ObservacaoCTeSubstituicao.visible(true);

    _anulacaoCTe.Componentes = [];
    _componentesAnulacaoCTe.RecarregarGrid();

    _crudAnulacaoCTe.Confirmar.visible(true);
    _crudAnulacaoCTe.ConfirmarAnulacaoSemSubstituicao.visible(false);
    _crudAnulacaoCTe.ConfirmarSubstituicao.visible(false);

    $("#knockoutComponentesAnulacaoCTe").show();
    Global.abrirModal("divModalAnulacaoCTe");
}

function AbrirTelaAnulacaoSemSubstituicao(e) {
    LimparCampos(_anulacaoCTe);
    _anulacaoCTe.CodigoCTe.val(e.CodigoCTE);
    _anulacaoCTe.CodigoCarga.val(_cargaCTe.Carga.val());
    _anulacaoCTe.TomadorCTeSubstituto.visible(false);
    _anulacaoCTe.ValorCTeSubstituicao.visible(false);
    _anulacaoCTe.ObservacaoCTeAnulacao.visible(true);
    _anulacaoCTe.ObservacaoCTeSubstituicao.visible(false);

    _anulacaoCTe.Componentes = [];
    _componentesAnulacaoCTe.RecarregarGrid();

    _crudAnulacaoCTe.Confirmar.visible(false);
    _crudAnulacaoCTe.ConfirmarAnulacaoSemSubstituicao.visible(true);
    _crudAnulacaoCTe.ConfirmarSubstituicao.visible(false);

    $("#knockoutComponentesAnulacaoCTe").hide();
    Global.abrirModal("divModalAnulacaoCTe");
}

function AbrirTelaSubstituicaoCTe(e) {
    LimparCampos(_anulacaoCTe);
    _anulacaoCTe.CodigoCTe.val(e.CodigoCTE);
    _anulacaoCTe.CodigoCarga.val(_cargaCTe.Carga.val());
    _anulacaoCTe.TomadorCTeSubstituto.visible(true);
    _anulacaoCTe.ValorCTeSubstituicao.visible(true);
    _anulacaoCTe.ObservacaoCTeAnulacao.visible(false);
    _anulacaoCTe.ObservacaoCTeSubstituicao.visible(true);

    _anulacaoCTe.Componentes = [];
    _componentesAnulacaoCTe.RecarregarGrid();

    _crudAnulacaoCTe.Confirmar.visible(false);
    _crudAnulacaoCTe.ConfirmarAnulacaoSemSubstituicao.visible(false);
    _crudAnulacaoCTe.ConfirmarSubstituicao.visible(true);

    $("#knockoutComponentesAnulacaoCTe").show();
    Global.abrirModal("divModalAnulacaoCTe");
}

function FecharTelaAnulacaoCTe() {
    LimparCampos(_anulacaoCTe);
    Global.fecharModal('divModalAnulacaoCTe');
}

function ConfirmarAnulacaoCTeClick() {
    _anulacaoCTe.ComponentesFrete.val(JSON.stringify(_anulacaoCTe.Componentes));

    Salvar(_anulacaoCTe, "CargaCTeManual/AnularCTe", function (r) {
        if (r.Success) {
            if (r.Data) {
                if (r.Data.EmitidoComSucesso)
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "O CT-e de anulação foi gerado com sucesso.");
                else
                    exibirMensagem(tipoMensagem.ok, "Atenção", "O CT-e de anulação foi gerado com sucesso, porém, ocorreram problemas ao enviar para a SEFAZ. Tente reenviar o mesmo ou contate o suporte técnico.");

                FecharTelaAnulacaoCTe();
                _gridCargaCTe.CarregarGrid();

            } else {
                exibirMensagem(tipoMensagem.atencao, "Atenção", r.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", r.Msg);
        }
    });
}

function ConfirmarAnulacaoSemSubstituicaoClick() {
    Salvar(_anulacaoCTe, "CargaCTeManual/AnularCTeSemSubstituicao", function (r) {
        if (r.Success) {
            if (r.Data) {
                if (r.Data.EmitidoComSucesso)
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "O CT-e de anulação foi gerado com sucesso.");
                else
                    exibirMensagem(tipoMensagem.ok, "Atenção", "O CT-e de anulação foi gerado com sucesso, porém, ocorreram problemas ao enviar para a SEFAZ. Tente reenviar o mesmo ou contate o suporte técnico.");

                FecharTelaAnulacaoCTe();
                _gridCargaCTe.CarregarGrid();

            } else {
                exibirMensagem(tipoMensagem.atencao, "Atenção", r.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", r.Msg);
        }
    });
}

function ConfirmarSubstituicaoClick() {
    _anulacaoCTe.ComponentesFrete.val(JSON.stringify(_anulacaoCTe.Componentes));

    Salvar(_anulacaoCTe, "CargaCTeManual/SubstituirCTe", function (r) {
        if (r.Success) {
            if (r.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "A geração do CT-e de substituição foi iniciada com sucesso. Aguarde a geração do mesmo!");

                FecharTelaAnulacaoCTe();
                _gridCargaCTe.CarregarGrid();
            } else {
                exibirMensagem(tipoMensagem.atencao, "Atenção", r.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", r.Msg);
        }
    });
}

var ComponentePrestacaoServicoAnulacao = function (cte) {

    let instancia = this;

    this.Grid = PropertyEntity({ type: types.local });

    this.Valor = PropertyEntity({ def: "0,00", val: ko.observable("0,00"), text: "*Valor:", getType: typesKnockout.decimal, maxlength: 20, required: true, visible: ko.observable(true), enable: ko.observable(true), configDecimal: { precision: 2, allowZero: true } });
    this.Componente = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "*Componente:", idBtnSearch: guid(), required: true, visible: ko.observable(true), enable: ko.observable(true) });
    this.IncluirBaseCalculoICMS = PropertyEntity({ text: "Inc. na B. C. do ICMS", val: ko.observable(true), def: true, getType: typesKnockout.bool, enable: ko.observable(true), visible: ko.observable(true) });
    this.IncluirTotalReceber = PropertyEntity({ text: "Inc. no tot. a receber", val: ko.observable(true), def: true, getType: typesKnockout.bool, enable: ko.observable(true), visible: ko.observable(false) });

    this.Adicionar = PropertyEntity({ eventClick: function () { instancia.AdicionarComponente() }, type: types.event, text: "Adicionar", visible: ko.observable(true), enable: ko.observable(true) });

    this.Load = function () {
        cte.Componentes = new Array();

        KoBindings(instancia, "knockoutComponentesAnulacaoCTe");

        new BuscarComponentesDeFrete(instancia.Componente, instancia.RetornoConsultaComponenteFrete);

        let menuOpcoes = { tipo: TypeOptionMenu.link, tamanho: 7, opcoes: [{ descricao: "Excluir", id: guid(), metodo: instancia.Excluir }] };

        let header = [
            { data: "Codigo", visible: false },
            { data: "Descricao", title: "Descrição", width: "25%" },
            { data: "Valor", title: "Valor", width: "15%" },
            { data: "IncluirBaseCalculoICMS", title: "Incluir no ICMS", width: "15%" },
            { data: "IncluirTotalReceber", title: "Incluir no Total a Receber", width: "15%" }
        ];

        cte.GridComponente = new BasicDataTable(instancia.Grid.id, header, menuOpcoes, { column: 1, dir: orderDir.asc }, null, 5);

        instancia.RecarregarGrid();
    };

    this.RetornoConsultaComponenteFrete = function (componente) {
        instancia.Componente.val(componente.Descricao);
        instancia.Componente.codEntity(componente.Codigo);
        $("#" + instancia.Valor.id).focus();
    };

    this.DestivarComponentePrestacaoServico = function () {
        DesabilitarCamposInstanciasCTe(instancia);
        cte.GridComponente.CarregarGrid(instancia.BuscarComponentes(), false);
    };

    this.AdicionarComponente = function () {
        let valido = ValidarCamposObrigatorios(instancia);

        if (valido) {

            if (cte.Componentes.some(function (componente) { return componente.CodigoComponente == instancia.Componente.codEntity(); })) {
                exibirMensagem(tipoMensagem.atencao, "Atenção!", "Já existe um componente " + instancia.Componente.val() + " registrado.");
                return;
            }

            let valor = Globalize.parseFloat(instancia.Valor.val());

            cte.Componentes.push({
                Codigo: guid(),
                CodigoComponente: instancia.Componente.codEntity(),
                DescricaoComponente: instancia.Componente.val(),
                Valor: Globalize.format(valor, "n2"),
                IncluirBaseCalculoICMS: instancia.IncluirBaseCalculoICMS.val(),
                IncluirTotalReceber: instancia.IncluirTotalReceber.val()
            });

            instancia.RecarregarGrid();

            LimparCampos(instancia);

        } else {
            exibirMensagem(tipoMensagem.atencao, "Campos Obrigatórios", "Informe os campos obrigatórios!");
        }
    };

    this.Excluir = function (componente) {
        for (let i = 0; i < cte.Componentes.length; i++) {
            if (componente.Codigo == cte.Componentes[i].Codigo) {
                cte.Componentes.splice(i, 1);
                break;
            }
        }

        instancia.RecarregarGrid();
    };

    this.RecarregarGrid = function () {
        cte.GridComponente.CarregarGrid(instancia.BuscarComponentes());
    };

    this.BuscarComponentes = function () {
        let componentesGrid = new Array();
        for (let i = 0; i < cte.Componentes.length; i++) {
            let componente = cte.Componentes[i];
            componentesGrid.push({
                Codigo: componente.Codigo,
                Descricao: componente.DescricaoComponente,
                Valor: componente.Valor,
                IncluirBaseCalculoICMS: componente.IncluirBaseCalculoICMS ? "Sim" : "Não",
                IncluirTotalReceber: componente.IncluirTotalReceber ? "Sim" : "Não"
            });
        }
        return componentesGrid;
    };
};

