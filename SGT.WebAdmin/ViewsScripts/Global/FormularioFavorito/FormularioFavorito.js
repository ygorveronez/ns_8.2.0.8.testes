var _formulariosFavoritosUsuario = null;

var FormulariosFavoritosUsuarioModel = function () {
    this.Formularios = ko.observableArray([]);
}

function loadFormulariosFavoritosUsuario() {

    _formulariosFavoritosUsuario = new FormulariosFavoritosUsuarioModel();
    KoBindings(_formulariosFavoritosUsuario, "knoutFormulariosFavoritosUsuario");

    RecarregarFormulariosFavoritos();

}

function RecarregarFormulariosFavoritos() {
    $(document).ready(function () {
        _FormulariosFavoritos.sort((a, b) => {
            let da = a.Descricao.toLowerCase(),
                db = b.Descricao.toLowerCase();

            if (da < db)
                return -1;

            if (da > db)
                return 1;

            return 0;
        });

        _formulariosFavoritosUsuario.Formularios.removeAll();

        for (let i = 0; i < _FormulariosFavoritos.length; i++)
            _formulariosFavoritosUsuario.Formularios.push(_FormulariosFavoritos[i]);
    });
}

function SalvarFormularioFavorito() {
    let url = location.href.split('#').splice(1).join('#');

    executarReST("FormularioFavorito/Salvar", { Path: url }, function (r) {
        if (r.Success) {
            if (r.Data) {

                exibirMensagem(tipoMensagem.ok, "Sucesso", "Favorito " + (r.Data.Favorito === true ? "adicionado" : "removido") + " com sucesso.");

                if (r.Data.Favorito === true) {
                    $(".setFavoriteUserPageIcon").addClass("fas").removeClass("far");
                    _FormulariosFavoritos.push(r.Data.Formulario);
                }
                else {
                    $(".setFavoriteUserPageIcon").addClass("far").removeClass("fas");
                    let idxPage = _FormulariosFavoritos.findIndex(o => o.Codigo == r.Data.Formulario.Codigo);
                    _FormulariosFavoritos.splice(idxPage, 1);
                }

                RecarregarFormulariosFavoritos();

            } else {
                exibirMensagem(tipoMensagem.aviso, "Atenção", r.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", r.Msg);
        }
    });
}