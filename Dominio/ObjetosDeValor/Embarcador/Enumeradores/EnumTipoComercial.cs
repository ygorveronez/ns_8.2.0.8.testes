namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum TipoComercial
    {
        NaoComercial = 0,
        Vendedor = 1,
        Gerente = 2,
        Supervisor = 3,
        GerenteNacional = 42,
        GerenteRede = 89,
        GerenteArea = 6,
        Promotor = 46,
        SupervisorDanone = 5,
    }

    public static class TipoComercialHelper
    {
        public static string ObterDescricao(this TipoComercial situacao)
        {
            switch (situacao)
            {
                case TipoComercial.NaoComercial: return "Não Comercial";
                case TipoComercial.Vendedor: return "Vendedor";
                case TipoComercial.Gerente: return "Gerente";
                case TipoComercial.Supervisor: return "Supervisor";
                case TipoComercial.GerenteNacional: return "Gerente Nacional (GN)";
                case TipoComercial.GerenteRede: return "Gerente de Rede (GR)";
                case TipoComercial.GerenteArea: return "Gerente de Área/Conta (GA/GC)";
                case TipoComercial.SupervisorDanone: return "Supervisor";
                case TipoComercial.Promotor: return "Promotor";
                default: return "Não Comercial";
            }
        }


        public static TipoComercial obterTipo(TipoComercial tipo)
        {
            if (tipo == TipoComercial.NaoComercial)
                return TipoComercial.NaoComercial;

            if (tipo == TipoComercial.Vendedor)
                return TipoComercial.Vendedor;

            if (tipo == TipoComercial.Gerente)
                return TipoComercial.Gerente;

            if (tipo == TipoComercial.Supervisor)
                return TipoComercial.Supervisor;

            if (tipo == TipoComercial.GerenteNacional)
                return TipoComercial.GerenteNacional;

            if (tipo == TipoComercial.GerenteRede)
                return TipoComercial.GerenteRede;

            if (tipo == TipoComercial.GerenteArea)
                return TipoComercial.GerenteArea;

            if (tipo == TipoComercial.SupervisorDanone)
                return TipoComercial.SupervisorDanone;

            if (tipo == TipoComercial.Promotor)
                return TipoComercial.Promotor;

            return TipoComercial.NaoComercial;
        }
    }
}
