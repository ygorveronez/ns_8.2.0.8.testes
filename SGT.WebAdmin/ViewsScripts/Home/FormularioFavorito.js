var _acessoRapidoModulosFavoritosUsuario = null;

var AcessoRapidoModulosFavoritosUsuarioModel = function () {
    this.Modulos = ko.observableArray([]);
}

function LoadAcessoRapidoModulosFavoritosUsuario() {

    if ($("#knockoutAcessoRapido").length <= 0)
        return;

    _acessoRapidoModulosFavoritosUsuario = new AcessoRapidoModulosFavoritosUsuarioModel();
    KoBindings(_acessoRapidoModulosFavoritosUsuario, "knockoutAcessoRapido");

    RecarregarAcessoRapidoModulosFavoritosUsuario();

}

function RecarregarAcessoRapidoModulosFavoritosUsuario() {
    $(document).ready(function () {
        _acessoRapidoModulosFavoritosUsuario.Modulos.removeAll();

        let modulosAcessoRapido = new Array();

        for (let i = 0; i < _FormulariosFavoritos.length; i++) {
            if (!modulosAcessoRapido.some(obj => obj.Descricao == _FormulariosFavoritos[i].ModuloPai?.Descricao)) {
                modulosAcessoRapido.push(_FormulariosFavoritos[i].ModuloPai);
            }
        }

        modulosAcessoRapido.sort((a, b) => {
            let da = a.Descricao?.toLowerCase() || "",
                db = b.Descricao?.toLowerCase() || "";

            if (da < db)
                return -1;

            if (da > db)
                return 1;

            return 0;
        });

        for (let i = 0; i < modulosAcessoRapido.length; i++) {
            let moduloAdd = {
                Descricao: modulosAcessoRapido[i]?.Descricao,
                Icone: modulosAcessoRapido[i]?.Icone,
                Formularios: new Array()
            };

            for (let j = 0; j < _FormulariosFavoritos.length; j++) {
                let formulario = _FormulariosFavoritos[j];

                if (formulario.ModuloPai?.Descricao == moduloAdd.Descricao)
                    moduloAdd.Formularios.push(formulario);
            }

            _acessoRapidoModulosFavoritosUsuario.Modulos.push(moduloAdd);
        }
    });
}