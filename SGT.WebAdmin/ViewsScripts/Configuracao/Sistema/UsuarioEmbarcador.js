///// <reference path="../../../js/libs/jquery-2.1.1.js" />
///// <reference path="../../../js/Global/CRUD.js" />
///// <reference path="../../../js/Global/knockout-3.1.0.js" />
///// <reference path="../../../js/Global/Rest.js" />
///// <reference path="../../../js/Global/Mensagem.js" />
///// <reference path="../../../js/Global/Grid.js" />
///// <reference path="../../../js/bootstrap/bootstrap.js" />
///// <reference path="../../../js/libs/jquery.blockui.js" />
///// <reference path="../../../js/Global/knoutViewsSlides.js" />
///// <reference path="../../../js/libs/jquery.maskMoney.js" />
///// <reference path="../../../js/plugin/datatables/jquery.dataTables.js" />
///// <reference path="../../../js/libs/jquery.twbsPagination.js" />
///// <reference path="../../Consultas/Localidade.js" />
///// <reference path="../../Consultas/ModeloVeicularCarga.js" />
///// <reference path="../../Consultas/TipoCarga.js" />
///// <reference path="../../Consultas/Motorista.js" />
///// <reference path="../../Consultas/Veiculo.js" />
///// <reference path="../../../js/libs/jquery.globalize.js" />
///// <reference path="../../../js/libs/jquery.globalize.pt-BR.js" />

//var _USUARIO_EMBARCADOR = {
//    Nome: "",
//    Permissoes: new Array()
//};

//_USUARIO_EMBARCADOR.Permissao = function (pagina) {
//    for (var i = 0; i < this.Permissoes.length; i++) {
//        if (this.Permissoes[i].Pagina == pagina) {
//            return this.Permissoes[i];
//        }
//    }
//    return null;
//};

//function loadUsuarioEmbarcador() {
//    executarReST("Usuario/BuscarDadosUsuarioLogado", null, function (arg) {
//        if (arg.Success) {
//            var data = arg.Data;
//            _USUARIO_EMBARCADOR.Permissoes = data.Permissoes;
//            _USUARIO_EMBARCADOR.Nome = data.Nome;
//        } else {
//            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
//        }
//    });
//}