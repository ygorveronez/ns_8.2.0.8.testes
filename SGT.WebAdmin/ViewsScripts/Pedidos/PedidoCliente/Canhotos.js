var ModalCanhotosPedidos = function () {
    var self = this;
    var _listaCanhotos;
    let idModal;
    /**
     * Definições Knockout
     */
    var ListaCanhotos = function () {
        this.Imagens = PropertyEntity({ val: ko.observable([]) });
        this.Codigo = PropertyEntity({});
    };

    /**
     * Eventos Knockout
     */
    var NavegarParaTelaPedidos = function () {

    }

    /**
     * Métodos Público
     */
    this.Load = function (id) {
        _listaCanhotos = new ListaCanhotos();
        KoBindings(_listaCanhotos, id);
        idModal = id;
    }

    this.CarregarCanhotoNota = function (data) {
        _listaCanhotos.Codigo.val(data.Codigo);
        carregarImagens();
    }

    this.AbrirModal = function (data) {
        Global.abrirModal(idModal);
    }

    this.FecharModal = function (data) {
        Global.fecharModal(idModal);
    }

    /**
     * Métodos Privados
     */
    var carregarImagens = function () {
        executarReST("PedidoCliente/BuscarCanhotosNotaFiscal", { Codigo: _listaCanhotos.Codigo.val() }, function (arg) {
            if (!arg.Success)
                return exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);

            if (!arg.Data)
                return exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);

            _listaCanhotos.Imagens.val(arg.Data);

            if (arg.Data.length > 0)
                self.AbrirModal();
        });
    }
}