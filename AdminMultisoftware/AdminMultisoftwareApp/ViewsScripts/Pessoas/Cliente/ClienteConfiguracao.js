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
/// <reference path="../../Consultas/InstanciaBase.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _configuracaoCliente;

var ConfiguracaoCliente = function () {
    this.ProducaoBase = PropertyEntity({ text: "*Base Produção: ", required: true });
    this.ProducaoInstanciaBase = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "*Instância da Base Produção:", idBtnSearch: guid(), required: true });
    this.ProducaoTipoServico = PropertyEntity({ val: ko.observable(EnumTipoServicoMultisoftware.MultiEmbarcador), def: EnumTipoServicoMultisoftware.MultiEmbarcador, text: "*Tipo de Serviço Produção:", options: ko.observable(EnumTipoServicoMultisoftware.obterOpcoes()), visible: ko.observable(true), required: true });
    
    this.HomologacaoBase = PropertyEntity({ text: "*Base Homologação: ", required: true });
    this.HomologacaoInstanciaBase = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "*Instância da Base Homologação:", idBtnSearch: guid(), required: true });
    this.HomologacaoTipoServico = PropertyEntity({ val: ko.observable(EnumTipoServicoMultisoftware.MultiEmbarcador), def: EnumTipoServicoMultisoftware.MultiEmbarcador, text: "*Tipo de Serviço Homologação:", options: ko.observable(EnumTipoServicoMultisoftware.obterOpcoes()), visible: ko.observable(true), required: true });
}

//*******EVENTOS*******


function loadClienteConfiguracao() {

    _configuracaoCliente = new ConfiguracaoCliente();
    KoBindings(_configuracaoCliente, "knockoutConfiguracaoCliente");

    BuscarInstanciaBase(_configuracaoCliente.ProducaoInstanciaBase);
    BuscarInstanciaBase(_configuracaoCliente.HomologacaoInstanciaBase);
}

//*******MÉTODOS*******

function limparCamposClienteConfiguracao() {
    LimparCampos(_configuracaoCliente);
}

function validarCamposObrigatoriosConfiguracaoCliente() {
    if (!ValidarCamposObrigatorios(_configuracaoCliente)) {
        exibirMensagem(tipoMensagem.atencao, "Aviso", "Por favor, informe os campos obrigatórios da aba Configuração do Cliente");
        return false;
    }
    return true;
}
