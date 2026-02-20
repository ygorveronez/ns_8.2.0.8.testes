var EnumFormaAutenticacaoUsuarioHelper = function () {
    this.Todas = "";
    this.AD = 1;
    this.UsuarioSenha = 2;
};

EnumFormaAutenticacaoUsuarioHelper.prototype = {
    obterOpcoes: function () {
        return [
            { text: Localization.Resources.Enumeradores.FormaAutenticacaoUsuario.Todas, value: this.Todas },
            { text: Localization.Resources.Enumeradores.FormaAutenticacaoUsuario.AD, value: this.AD },
            { text: Localization.Resources.Enumeradores.FormaAutenticacaoUsuario.UsuarioBarraSenha, value: this.UsuarioSenha },
        ];
    }
};

var EnumFormaAutenticacaoUsuario = Object.freeze(new EnumFormaAutenticacaoUsuarioHelper());