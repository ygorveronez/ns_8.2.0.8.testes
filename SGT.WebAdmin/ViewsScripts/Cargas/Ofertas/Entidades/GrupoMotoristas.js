/// <reference path="../../../../wwwroot/js/libs/jquery-2.1.1.js" />
/// <reference path="../../../../wwwroot/js/Global/CRUD.js" />
/// <reference path="../../../../wwwroot/js/Global/Rest.js" />
/// <reference path="../../../../wwwroot/js/Global/Mensagem.js" />
/// <reference path="../../../../wwwroot/js/bootstrap/bootstrap.js" />
/// <reference path="../../../../wwwroot/js/Global/knoutViewsSlides.js" />
/// <reference path="../../../../wwwroot/js/plugin/datatables/jquery.dataTables.js" />
/// <reference path="../../../Consultas/GrupoMotoristas.js" />
/// <reference path="../ParametrosOfertas.js" />
/// <reference path="../Constantes.js" />

var _grupoMotoristas;

var GrupoMotoristas = function () {
    this.GrupoMotoristas = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Grupo de Motoristas", val: ko.observable(""), idBtnSearch: guid(), def: "", visible: ko.observable(true) });
}

function LoadGrupoMotoristasCRUD() {
    _grupoMotoristas = new GrupoMotoristas();
    KoBindings(_grupoMotoristas, "knockoutGrupoMotoristas");

    new BuscarGrupoMotoristas(_grupoMotoristas.GrupoMotoristas, (retorno) => callbackSelecao(retorno, _grupoMotoristas.GrupoMotoristas));
    _parametrosOfertas.GrupoMotoristas = _grupoMotoristas.GrupoMotoristas;
}

function callbackSelecao(selecionado, knout) {
    var integracoesDisponiveis = _parametrosOfertasTipoIntegracao.TipoIntegracao.options();
    if (selecionado.Situacao != EnumSituacaoGrupoMotoristas.obterDescricao(EnumSituacaoGrupoMotoristas.Finalizado) && integracoesDisponiveis.some(integracao => integracao.value == EnumTipoIntegracao.Trizy)) {
        exibirMensagem(tipoMensagem.atencao, "", `Não é possível selecionar um grupo de motoristas ${EnumSituacaoGrupoMotoristas.obterDescricao(EnumSituacaoGrupoMotoristas.Finalizado)} devido o sistema de integração: ${EnumTipoIntegracao.obterDescricao(EnumTipoIntegracao.Trizy)} estar ativo no sistema.`);
        return;
    }
    knout.codEntity(selecionado.Codigo);
    knout.val(selecionado.Descricao);
}