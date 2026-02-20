namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum TipoEmissaoIntramunicipal
    {
        NaoEspecificado = 0,
        SempreNFSe = 1,//Sempre deve emitir NFSe, se o transportador não possuir configuração não pode informar
        SempreCTe = 2,//Sempre emite CTe
        SempreNFSManual = 7, //Sempre fará o controle da emissão da nota de serviço manualmente.
        NaoEmiteNenhumDocumento = 5, //Não fará a emissão de nenhum documento na viagem
        NFEsApenasEmpresasAptasDemaisCTe = 3,//Emite NFSe apenas para as empresas configuradas as demais emite CT-e
        NFEsApenasEmpresasAptasDemaisNFsManual = 4, //Emite NFSe apenas para as empresas configuradas as demais aguarda a importação da NFS manual
        NFEsApenasEmpresasAptasDemaisNaoEmiteNenhumDocumento = 6, //Emite NFSe apenas para as empresas configuradas as demais aguarda não emite nada
    }
}
