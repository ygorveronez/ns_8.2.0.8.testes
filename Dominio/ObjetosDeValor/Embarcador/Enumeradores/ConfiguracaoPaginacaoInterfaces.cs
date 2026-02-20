namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    /**
     * <summary>
     * ##### NaoInformada : 0
     * ##### Cargas_Carga : 1
     * ##### PagamentosMotoristas_PagamentoMotoristaTMS : 2
     * </summary>
     * */


    public enum ConfiguracaoPaginacaoInterfaces
    {
        NaoInformada = 0,
        Cargas_Carga = 1,
        PagamentosMotoristas_PagamentoMotoristaTMS = 2,
    }

    public static class ConfiguracaoPaginacaoInterfacesHelper
    {
        public static ConfiguracaoPaginacaoInterfaces ObterInterfaces(int codigo)
        {
            switch (codigo)
            {
                case 1: return ConfiguracaoPaginacaoInterfaces.Cargas_Carga;
                case 2: return ConfiguracaoPaginacaoInterfaces.PagamentosMotoristas_PagamentoMotoristaTMS;
                default: return 0;
            }
        }

        public static string ObterDescricaoInterfaces(this ConfiguracaoPaginacaoInterfaces interfaceSistema)
        {
            switch (interfaceSistema)
            {
                case ConfiguracaoPaginacaoInterfaces.Cargas_Carga: return "#Cargas/Carga";
                case ConfiguracaoPaginacaoInterfaces.PagamentosMotoristas_PagamentoMotoristaTMS: return "#PagamentosMotoristas/PagamentoMotoristaTMS";

                default: return "";
            }
        }
    }


}
