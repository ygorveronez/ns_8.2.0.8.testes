/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="CentroCarregamento.js" />

/*
 * Declaração de Objetos Globais do Arquivo
 */

var _centroCarregamentoMotorista;

/*
 * Declaração das Classes
 */

var CentroCarregamentoMotorista = function () {
    this.LimiteCargasPorMotoristaPorDia = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int, text: Localization.Resources.Logistica.CentroCarregamento.LimiteCargasPorMotoristaPorDia.getFieldDescription() });
    this.LimiteDeCargasAtivasPorMotorista = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int, text: Localization.Resources.Logistica.CentroCarregamento.LimiteDeCargasAtivasPorMotorista.getFieldDescription() });
}

/*
 * Declaração das Funções de Inicialização
 */

function loadoMotorista() {
    _centroCarregamentoMotorista = new CentroCarregamentoMotorista();
    KoBindings(_centroCarregamentoMotorista , "knockoutMotorista");

    if (_CONFIGURACAO_TMS.TipoServicoMultisoftware == EnumTipoServicoMultisoftware.MultiEmbarcador) {
        $("#liTabMotorista").show();
    }
}

/*
 * Declaração das Funções Associadas a Eventos
 */



/*
 * Declaração das Funções Públicas
 */

function preencherCentroCarregamentoMotorista(dadosMotorista) {
    PreencherObjetoKnout(_centroCarregamentoMotorista, { Data: dadosMotorista });
}

function preencherCentroCarregamentoMotoristaSalvar(centroCarregamento) {
    centroCarregamento["LimiteCargasPorMotoristaPorDia"] = _centroCarregamentoMotorista.LimiteCargasPorMotoristaPorDia.val();
    centroCarregamento["LimiteDeCargasAtivasPorMotorista"] = _centroCarregamentoMotorista.LimiteDeCargasAtivasPorMotorista.val();
}

function limparCamposCentroCarregamentoMotorista() {
    LimparCampos(_centroCarregamentoMotorista);
}

/*
 * Declaração das Funções
 */

