const EnumTipoAutenticacaoYMSHelper = function () {
    this.Basic = 1,
    this.BearerToken = 2
};

EnumTipoAutenticacaoYMSHelper.prototype = {
    obterOpcoes: function () {
        return [            
            { text: "Basic", value: this.Basic },
            { text: "Bearer Token", value: this.BearerToken }
        ];
    }   
}

const EnumTipoAutenticacaoYMS = Object.freeze(new EnumTipoAutenticacaoYMSHelper());