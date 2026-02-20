const EnumTipoAutenticacaoHelper = function () {
    this.Todos = 0;
    this.Token = 1;
    this.UsuarioESenha = 2;
};

EnumTipoAutenticacaoHelper.prototype = {
    obterOpcoes: function () {
        return [
            { text: Localization.Resources.Enumeradores.TipoAutenticacao.Token, value: this.Token },
            { text: Localization.Resources.Enumeradores.TipoAutenticacao.UsuarioESenha, value: this.UsuarioESenha }
        ];
    },
    obterOpcoesPesquisa: function () {
        return [{ text: Localization.Resources.Enumeradores.TipoAutenticacao.Todos, value: this.Todos }].concat(this.obterOpcoes());
    }
}

const EnumTipoAutenticacao = Object.freeze(new EnumTipoAutenticacaoHelper());