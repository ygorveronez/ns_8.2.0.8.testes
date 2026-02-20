var EnumSituacaoFilialHelper = function () {
    this.SemSituacao = 0;
    this.RecibidoDeMRDR = 10;
    this.RequisitoDeCompra = 40;
    this.PrecoPadraoEstimado = 45;
    this.ProntoTodosUsuarios = 60;
    this.IntencaoDescontinuar = 70;
    this.EmDescontinuacao = 80;
    this.Descontinuado = 90;
    this.DesativadoCriadoComErro= 95;
};

EnumSituacaoFilialHelper.prototype = {
    obterOpcoes: function () {
        return [
            { text: "Sem Situacao", value: this.SemSituacao },
            { text: "Recibido de MRDR", value: this.RecibidoDeMRDR },
            { text: "Requisito de compra", value: this.RequisitoDeCompra },
            { text: "Preco padrão estimado", value: this.PrecoPadraoEstimado },
            { text: "Pronto para todos os usuário", value: this.ProntoTodosUsuarios },
            { text: "Intenção em descontinuar", value: this.IntencaoDescontinuar },
            { text: "Em descontinuação", value: this.EmDescontinuacao },
            { text: "Descontinuado", value: this.Descontinuado },
            { text: "Desativado criado com error", value: this.DesativadoCriadoComErro }
        ];
    },
    obterDescricao: function (codigo) {
        let opcoes = this.obterOpcoes();
        let [descricao] = opcoes.filter(item => item.value === codigo);

        if (!descricao)
            return "";

        return descricao.text;
    }
}

var EnumSituacaoFilial = Object.freeze(new EnumSituacaoFilialHelper());

