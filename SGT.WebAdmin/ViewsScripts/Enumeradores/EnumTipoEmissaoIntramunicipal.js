var EnumTipoEmissaoIntramunicipalHelper = function () {
    this.NaoEspecificado= 0;
    this.SempreNFSe = 1;//Sempre deve emitir NFSe, se o transportador não possuir configuração não pode informar
    this.SempreCTe = 2;//Sempre emite CTe
    this.SempreNFSManual = 7; //Sempre fará o controle da emissão da nota de serviço manualmente.
    this.NaoEmiteNenhumDocumento = 5; //Não fará a emissão de nenhum documento na viagem
    this.NFEsApenasEmpresasAptasDemaisCTe = 3;//Emite NFSe apenas para as empresas configuradas as demais emite CT-e
    this.NFEsApenasEmpresasAptasDemaisNFsManual = 4; //Emite NFSe apenas para as empresas configuradas as demais aguarda a importação da NFS manual
    this.NFEsApenasEmpresasAptasDemaisNaoEmiteNenhumDocumento = 6; //Emite NFSe apenas para as empresas configuradas as demais aguarda não emite nada
};

EnumTipoEmissaoIntramunicipalHelper.prototype = {
    obterOpcoes: function () {
        return [
            { text: Localization.Resources.Enumeradores.TipoEmissaoIntramunicipal.UsarPadrao, value: this.NaoEspecificado },
            { text: Localization.Resources.Enumeradores.TipoEmissaoIntramunicipal.SempreEmitirNFSe, value: this.SempreNFSe },
            { text: Localization.Resources.Enumeradores.TipoEmissaoIntramunicipal.SempreEmitirNFSManualmente, value: this.SempreNFSManual },
            { text: Localization.Resources.Enumeradores.TipoEmissaoIntramunicipal.SempreEmitirCTe, value: this.SempreCTe },
            { text: Localization.Resources.Enumeradores.TipoEmissaoIntramunicipal.NaoEmitirUenhumDocumento, value: this.NaoEmiteNenhumDocumento },
            { text: Localization.Resources.Enumeradores.TipoEmissaoIntramunicipal.EmitirNFSeApenasParaTransportadoresHabilitadosOsDemaisEmitirCTe, value: this.NFEsApenasEmpresasAptasDemaisCTe },
            { text: Localization.Resources.Enumeradores.TipoEmissaoIntramunicipal.EmitirNFSeApenasParaTransportadoresHabilitadosOsDemaisDevemEmitirNFSManualmente, value: this.NFEsApenasEmpresasAptasDemaisNFsManual },
            { text: Localization.Resources.Enumeradores.TipoEmissaoIntramunicipal.EmitirNFSeApenasParaTransportadoresHabilitadosOsDemaisNaoEmitirNenhumDocumento, value: this.NFEsApenasEmpresasAptasDemaisNaoEmiteNenhumDocumento }
        ];
    }
};

var EnumTipoEmissaoIntramunicipal = Object.freeze(new EnumTipoEmissaoIntramunicipalHelper());