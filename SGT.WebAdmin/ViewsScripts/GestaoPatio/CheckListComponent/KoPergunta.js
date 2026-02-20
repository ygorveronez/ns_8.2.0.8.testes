/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="../../Enumeradores/EnumCheckListResposta.js" />
/// <reference path="../../Enumeradores/EnumTipoOpcaoCheckList.js" />
/// <reference path="../../Enumeradores/EnumTipoInformativo.js" />

/*
 * Declaração das Classes
 */

function PerguntaViewModel(params) {
    var self = this;

    this.Pergunta = processarPergunta(params.Pergunta);
    this.Init = function () {
        if (self.Pergunta.typeKnockout == typesKnockout.dateTime)
            ConfigurarCampoDateTime(self.Pergunta);
        else if (self.Pergunta.typeKnockout == typesKnockout.date)
            ConfigurarCampoDate(self.Pergunta);
        else if (self.Pergunta.typeKnockout == typesKnockout.time)
            ConfigurarCampoTime(self.Pergunta);
        else if (self.Pergunta.typeKnockout == typesKnockout.decimal)
            ConfigurarCampoDecimal(self.Pergunta);

        $("#pergunta-" + self.Pergunta.id).on("focus", "input, select", function () {
            $(this).parents(".pergunta-container").find('.state-error').removeClass("state-error");
        });

        if (self.Pergunta.Tipo == EnumTipoOpcaoCheckList.Escala) {
            $("#pergunta-" + self.Pergunta.id).on("change", "input", function () {
                var codigo = $(this).data('cod');
                for (var i = 0; i < self.Pergunta.Alternativas.length; i++) {
                    if (self.Pergunta.Alternativas[i].Codigo != codigo)
                        self.Pergunta.Alternativas[i].Marcado = false;
                }
            });
        }
        else if (self.Pergunta.Tipo == EnumTipoOpcaoCheckList.Informativo) {
            if (self.Pergunta.TipoInformativo == EnumTipoInformativo.TipoDecimal)
                $("#" + self.Pergunta.id).val(parseFloat(self.Pergunta.Observacao).toFixed(2).replace('.', ','));
            else
                $("#" + self.Pergunta.id).val(self.Pergunta.Observacao);

            if (self.Pergunta.TagIntegracao == "PesoTotalExtraidoFruta" || self.Pergunta.TagIntegracao == "PesoCorretoComRefugo") {
                $("#pergunta-" + self.Pergunta.id).on('keyup', function () {
                    if (typeof _checkList == 'undefined')
                        return;
                    var camposResultadoRendimento = _checkList.GrupoPerguntas.val()[0].Perguntas.filter(function (pergunta) { return pergunta.TipoRelacao == 4; });
                    var pesoTotal = _checkList.GrupoPerguntas.val()[0].Perguntas.filter(function (pergunta) { return pergunta.TagIntegracao == "PesoTotalExtraidoFruta"; })[0];
                    var pesoCorreto = _checkList.GrupoPerguntas.val()[0].Perguntas.filter(function (pergunta) { return pergunta.TagIntegracao == "PesoCorretoComRefugo"; })[0];

                    for (let i = 0; i < camposResultadoRendimento.length; i++) {
                        $("#info_" + camposResultadoRendimento[i].id).val(obterResultadoDivisao(pesoTotal.Resposta, pesoCorreto.Resposta));
                    }
                });
            }
        }
        $("#pergunta-" + self.Pergunta.id).on("change", "[data-na]", function () {
            var $e = $(this);
            var $p = $e.parents(".pergunta-container");
            var disabled = $e.is(":checked");

            $p.find('input, select').not('[data-na]').prop('disabled', disabled);
            if (disabled) {
                self.Pergunta.Resposta = null;
                if (self.Pergunta.Tipo == EnumTipoOpcaoCheckList.Selecoes || self.Pergunta.Tipo == EnumTipoOpcaoCheckList.Escala || self.Pergunta.Tipo == EnumTipoOpcaoCheckList.Opcoes) {
                    for (var i = 0; i < self.Pergunta.Alternativas.length; i++) {
                        self.Pergunta.Alternativas[i].Marcado = false;
                    }
                }

                $p.find('input[type=checkbox], input[type=radio]').not('[data-na]').prop('checked', false);
                $p.find('input[type=text], select').val('');
            }
        });
    };

    this.ExibirObservacao = PropertyEntity({ eventClick: observacaoClick, type: types.event, text: "Obs.", visible: ko.observable(true) });
}

/*
 * Declaração das Funções de Inicialização
 */
function loadComponenteChecklistPergunta() {
    registrarComponentePergunta();
}

/*
 * Declaração das Funções Associadas a Eventos
 */

function observacaoClick(pergunta) {
    if (editarObservacaoPergunta) {
        editarObservacaoPergunta(pergunta.Pergunta);
    }
}

/*
 * Declaração das Funções Privadas
 */
function processarPergunta(pergunta) {
    pergunta.id = guid();
    pergunta.typeKnockout = typesKnockout.string;

    if (!('Obrigatorio' in pergunta)) pergunta.Obrigatorio = true;
    if (!('Enable' in pergunta)) pergunta.Enable = true;
    if (!('PermiteNaoAplica' in pergunta)) pergunta.PermiteNaoAplica = false;

    var alternativaNula = { Descricao: 'Selecione', Codigo: undefined };
    var alternativasCustomizadas = [alternativaNula];

    if (pergunta.Tipo == EnumTipoOpcaoCheckList.Selecoes) {
        pergunta.Alternativas = alternativasCustomizadas.concat(pergunta.Alternativas);
    }

    if (pergunta.Tipo == EnumTipoOpcaoCheckList.Informativo) {
        if (pergunta.TipoInformativo == EnumTipoInformativo.TipoDataHora)
            pergunta.typeKnockout = typesKnockout.dateTime;
        else if (pergunta.TipoInformativo == EnumTipoInformativo.TipoData)
            pergunta.typeKnockout = typesKnockout.date;
        else if (pergunta.TipoInformativo == EnumTipoInformativo.TipoHora)
            pergunta.typeKnockout = typesKnockout.time;
        else if (pergunta.TipoInformativo == EnumTipoInformativo.TipoDecimal)
            pergunta.typeKnockout = typesKnockout.decimal;
        else if (pergunta.TipoInformativo == EnumTipoInformativo.TipoTexto)
            pergunta.typeKnockout = typesKnockout.string;
    }

    return pergunta;
}

function registrarComponentePergunta() {
    if (ko.components.isRegistered('pergunta'))
        return;

    ko.components.register('pergunta', {
        viewModel: PerguntaViewModel,
        template:
            "<div class=\"pergunta-container smart-form\" data-bind=\"attr: {id: 'pergunta-' + Pergunta.id}, template: { afterRender: Init }\">" +
            "    <!-- ko if: Pergunta.Tipo == " + EnumTipoOpcaoCheckList.Aprovacao + " -->" +
            "    <div class=\"col-12\">" +
            "        <div class=\"form-group\">" +
            "            <label class=\"form-label\" data-bind=\"text: Pergunta.Descricao, attr: { for: Pergunta.id + '_' + $index() }\"></label>" +
            "            <select class=\"form-control\" data-bind=\"valueAllowUnset: true, options: Pergunta.Alternativas, enable: Pergunta.Enable, optionsText: 'Descricao', optionsValue: 'Codigo', value: Pergunta.Resposta, attr: { name: Pergunta.Codigo + '_' + $index(), id : Pergunta.id + '_' + $index()}\"></select>" +
            "        </div>" +
            "    </div>" +
            "    <!-- /ko -->" +
            "    <!-- ko if: Pergunta.Tipo == " + EnumTipoOpcaoCheckList.SimNao + " -->" +
            "    <div class=\"col-md-12 simnao-container\">" +
            "       <div class=\"form-group\">" +
            "          <label class=\"form-label\" data-bind=\"text: Pergunta.Descricao, attr: { id: Pergunta.id + '_label' }\"></label>" +
            "          <div class=\"custom-control custom-radio custom-control-inline\">" +
            "              <input class=\"custom-control-input\" type=\"radio\" value=\"true\" data-bind=\"checked: Pergunta.Resposta, enable: Pergunta.Enable && !Pergunta.RespostaNaoSeAplica, attr: { name: 'simnao_' + Pergunta.Codigo, id: 'sim_' + Pergunta.id }\" />" +
            "              <label class=\"custom-control-label\" data-bind=\"attr: { for: 'sim_' + Pergunta.id }\">Sim</label>" +
            "          </div>" +
            "          <div class=\"custom-control custom-radio custom-control-inline\">" +
            "              <input class=\"custom-control-input\" type=\"radio\" value=\"false\" data-bind=\"checked: Pergunta.Resposta, enable: Pergunta.Enable && !Pergunta.RespostaNaoSeAplica, attr: { name: 'simnao_' + Pergunta.Codigo, id: 'nao_' + Pergunta.id }\" />" +
            "              <label class=\"custom-control-label\" data-bind=\"attr: { for: 'nao_' + Pergunta.id }\">Não</label>" +
            "          </div>" +
            "          <div class=\"custom-control custom-checkbox custom-control-inline\" data-bind=\"visible: Pergunta.PermiteNaoAplica \">" +
            "              <input type=\"checkbox\" data-na data-bind=\"checked: Pergunta.RespostaNaoSeAplica, enable: Pergunta.Enable, valueUpdate: 'afterkeydown', attr: { name: 'simnao_' + Pergunta.Codigo, id: 'na_' + Pergunta.id}\" />" +
            "              <label class=\"custom-control-label\" data-bind=\"attr: { for: 'na_' + Pergunta.id }\">Não se Aplica (N/A)</label>" +
            "          </div>" +
            "       </div>" +
            "    </div>" +
            "    <!-- /ko -->" +
            "    <!-- ko if: Pergunta.Tipo == " + EnumTipoOpcaoCheckList.Escala + " -->" +
            "    <div class=\"col-12\">" +
            "       <div class=\"form-group\">" +
            "          <label class=\"form-label\" data-bind=\"text: Pergunta.Descricao\"></label>" +
            "          <!-- ko foreach: { data: Pergunta.Alternativas, as: 'Alternativa' } -->" +
            "          <div class=\"custom-control custom-radio custom-control-inline\">" +
            "              <input class=\"custom-control-input\" type=\"radio\" data-bind=\"checked: Alternativa.Marcado, value: true, enable: $parent.Pergunta.Enable && !$parent.Pergunta.RespostaNaoSeAplica, valueUpdate: 'afterkeydown', attr: { 'data-cod': Alternativa.Codigo, name: $parent.Pergunta.Codigo, id : $parent.Pergunta.id + '_' + $index()}\" />" +
            "              <label class=\"custom-control-label\" data-bind=\"text: Alternativa.Descricao, attr: { for: $parent.Pergunta.id + '_' + $index() }\"></label>" +
            "          </div>" +
            "          <!-- /ko -->" +
            "          <div class=\"custom-control custom-checkbox custom-control-inline\" data-bind=\"visible: Pergunta.PermiteNaoAplica \">" +
            "              <input type=\"checkbox\" data-na data-bind=\"checked: Pergunta.RespostaNaoSeAplica, enable: Pergunta.Enable, valueUpdate: 'afterkeydown', attr: { name: 'simnao_' + Pergunta.Codigo, id: 'na_' + Pergunta.id}\" />" +
            "              <label class=\"custom-control-label\" data-bind=\"attr: { for: 'na_' + Pergunta.id }\">Não se Aplica (N/A)</label>" +
            "          </div>" +
            "       </div>" +
            "    </div>" +
            "    <!-- /ko -->" +
            "    <!-- ko if: Pergunta.Tipo == " + EnumTipoOpcaoCheckList.Opcoes + " -->" +
            "    <div class=\"col-12\">" +
            "       <div class=\"form-group\">" +
            "          <label class=\"form-label\" data-bind=\"text: Pergunta.Descricao\"></label>" +
            "          <!-- ko foreach: { data: Pergunta.Alternativas, as: 'Alternativa' } -->" +
            "          <div class=\"custom-control custom-checkbox custom-control-inline\">" +
            "              <input class=\"custom-control-input\" type=\"checkbox\" data-bind=\"checked: Alternativa.Marcado, enable: $parent.Pergunta.Enable && !$parent.Pergunta.RespostaNaoSeAplica, valueUpdate: 'afterkeydown', attr: { name: $parent.Pergunta.Codigo + '_' + $index(), id : $parent.Pergunta.id + '_' + $index()}\" />" +
            "              <label class=\"custom-control-label\" data-bind=\"text: Alternativa.Descricao, attr: { for: $parent.Pergunta.id + '_' + $index() }\"></label>" +
            "          </div>" +
            "          <!-- /ko -->" +
            "          <div class=\"custom-control custom-checkbox custom-control-inline\" data-bind=\"visible: Pergunta.PermiteNaoAplica \">" +
            "              <input type=\"checkbox\" data-na data-bind=\"checked: Pergunta.RespostaNaoSeAplica, enable: Pergunta.Enable, valueUpdate: 'afterkeydown', attr: { name: 'simnao_' + Pergunta.Codigo, id: 'na_' + Pergunta.id}\" />" +
            "              <label class=\"custom-control-label\" data-bind=\"attr: { for: 'na_' + Pergunta.id }\">Não se Aplica (N/A)</label>" +
            "          </div>" +
            "       </div>" +
            "    </div>" +
            "    <!-- /ko -->" +
            "    <!-- ko if: Pergunta.Tipo == " + EnumTipoOpcaoCheckList.Informativo + " -->" +
            "    <div class=\"col-12\">" +
            "        <div class=\"form-group\">" +
            "           <label class=\"form-label\" data-bind=\"text: Pergunta.Descricao, attr: { for: 'info_' + Pergunta.id }\"></label>" +
            "           <input type=\"text\" class=\"form-control\" data-bind=\"value: Pergunta.Resposta, enable: Pergunta.Enable && !Pergunta.RespostaNaoSeAplica, valueUpdate: 'afterkeydown', attr: { id : 'info_' + Pergunta.id}\" />" +
            "        </div>" +
            "    </div>" +
            "    <div class=\"col-12\" data-bind=\"visible: Pergunta.PermiteNaoAplica\">" +
            "        <div class=\"custom-control custom-checkbox\">" +
            "            <input type=\"checkbox\" data-na data-bind=\"checked: Pergunta.RespostaNaoSeAplica, enable: Pergunta.Enable, valueUpdate: 'afterkeydown', attr: { name: 'simnao_' + Pergunta.Codigo, id: 'na_' + Pergunta.id}\" />" +
            "            <label class=\"custom-control-label\" data-bind=\"attr: { for: 'na_' + Pergunta.id }\">Não se Aplica (N/A)</label>" +
            "        </div>" +
            "    </div>" +
            "    <!-- /ko -->" +
            "    <!-- ko if: Pergunta.Tipo == " + EnumTipoOpcaoCheckList.Selecoes + " -->" +
            "    <div class=\"col-12\">" +
            "        <div class=\"form-group\">" +
            "            <label class=\"form-label\" data-bind=\"text: Pergunta.Descricao, attr: { for: Pergunta.id + '_' + $index() }\"></label>" +
            "            <select class=\"form-control\" data-bind=\"valueAllowUnset: true, options: Pergunta.Alternativas, enable: Pergunta.Enable && !Pergunta.RespostaNaoSeAplica, optionsText: 'Descricao', optionsValue: 'Codigo', value: Pergunta.Resposta, attr: { name: Pergunta.Codigo + '_' + $index(), id: Pergunta.id + '_' + $index()}\"></select>" +
            "        </div>" +
            "    </div>" +
            "    <div class=\"col-12\" data-bind=\"visible: Pergunta.PermiteNaoAplica\">" +
            "        <div class=\"custom-control custom-checkbox\">" +
            "            <input type=\"checkbox\" data-na data-bind=\"checked: Pergunta.RespostaNaoSeAplica, enable: Pergunta.Enable, valueUpdate: 'afterkeydown', attr: { name: 'simnao_' + Pergunta.Codigo, id: 'na_' + Pergunta.id}\" />" +
            "            <label class=\"custom-control-label\" data-bind=\"attr: { for: 'na_' + Pergunta.id }\">Não se Aplica (N/A)</label>" +
            "        </div>" +
            "    </div>" +
            "    <!-- /ko -->" +
            "    </section>" +
            "    <!-- ko if: Pergunta.Tipo == " + EnumTipoOpcaoCheckList.Aprovacao + " -->" +
            "    <div class=\"col-sm-12 col-md-4\">" +
            "        <div class=\"form-group\">" +
            "            <input type=\"button\" data-bind=\"click: ExibirObservacao.eventClick, attr: { value: ExibirObservacao.text }\" class=\"btn btn-default waves-effect waves-themed\" />" +
            "        </div>" +
            "    </div>" +
            "    <!-- /ko -->" +
            "</div>"
    });
};

function obterCheckListComponent(grupoPerguntas) {
    var objetoRetorno = [];

    grupoPerguntas.map(function (grupo) {
        grupo.Perguntas.map(function (pergunta) {
            var resposta = {
                Codigo: pergunta.Codigo,
                Tipo: pergunta.Tipo,
                Resposta: pergunta.Resposta,
                NaoAplica: pergunta.RespostaNaoSeAplica === true ? true : false,
                Alternativas: []
            };

            switch (pergunta.Tipo) {
                case EnumTipoOpcaoCheckList.Escala:
                case EnumTipoOpcaoCheckList.Opcoes:
                    resposta.Alternativas = pergunta.Alternativas.filter(function (a) { return a.Marcado }).map(function (a) {
                        return {
                            Codigo: a.Codigo,
                        };
                    });
                    break;
            }

            objetoRetorno.push(resposta);
        });
    });

    return objetoRetorno;
}

function validarCheckListComponent(grupoPerguntas) {
    var preenchimentoValido = true;

    grupoPerguntas.map(function (grupo) {
        grupo.Perguntas.map(function (pergunta) {
            if (!pergunta.Obrigatorio || (pergunta.PermiteNaoAplica && pergunta.RespostaNaoSeAplica)) {
                return;
            }

            switch (pergunta.Tipo) {
                case EnumTipoOpcaoCheckList.Escala:
                    if (!pergunta.Alternativas.find(function (a) { return a.Marcado })) {
                        $("#pergunta-" + pergunta.id + " label.radio").addClass("is-invalid");
                        preenchimentoValido = false;
                    }
                    break;

                case EnumTipoOpcaoCheckList.Informativo:
                    if (!validarTipoInformativo(pergunta.TipoInformativo, pergunta.Resposta)) {
                        $("#pergunta-" + pergunta.id + " label.input").addClass("is-invalid");
                        preenchimentoValido = false;
                    }
                    break;

                case EnumTipoOpcaoCheckList.Opcoes:
                    if (!pergunta.Alternativas.find(function (a) { return a.Marcado })) {
                        $("#pergunta-" + pergunta.id + " label.checkbox").addClass("is-invalid");
                        preenchimentoValido = false;
                    }
                    break;

                case EnumTipoOpcaoCheckList.Selecoes:
                    if ((parseInt(pergunta.Resposta) || 0) == 0) {
                        $("#pergunta-" + pergunta.id + " label.select").addClass("is-invalid");
                        preenchimentoValido = false;
                    }
                    break;

                case EnumTipoOpcaoCheckList.SimNao:
                    if (pergunta.Resposta != "true" && pergunta.Resposta != "false") {
                        $(`#${pergunta.id}_label`).css("color", "#dc3545");
                        preenchimentoValido = false;
                    }
                    break;
            }
        });
    });

    if (!preenchimentoValido)
        exibirMensagem(tipoMensagem.atencao, "Atenção!", "É necessário preencher os itens destacados.");

    return preenchimentoValido;
}

function validarTipoInformativo(tipo, resposta) {
    if (tipo == EnumTipoInformativo.TipoDecimal)
        return (parseFloat(resposta.replace(",", ".")) <= 0);

    return Boolean(resposta);
}

function obterResultadoDivisao(dividendo, divisor) {
    if (dividendo == 0 || divisor == 0)
        return 0;
    dividendo = parseFloat(dividendo.toString().replace(".", "").replace(",", "."));
    divisor = parseFloat(divisor.toString().replace(".", "").replace(",", "."));
    return Global.roundNumber(dividendo / divisor, 6).toString().replace(".", ",");
}
