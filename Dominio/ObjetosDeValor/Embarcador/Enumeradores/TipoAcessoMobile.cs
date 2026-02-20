namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum TipoAcessoMobile
    {
        Motorista = 0,
        Funcionario = 1,
        Transportador = 2,
        Cliente = 3
    }


    public static class TipoAcessoMobileHelper
    {
        public static string ObterDescricao(this TipoAcessoMobile tipoCanhoto)
        {
            switch (tipoCanhoto)
            {
                case ObjetosDeValor.Embarcador.Enumeradores.TipoAcessoMobile.Motorista:
                    return "Motorista";
                case ObjetosDeValor.Embarcador.Enumeradores.TipoAcessoMobile.Funcionario:
                    return "Funcionário";
                case ObjetosDeValor.Embarcador.Enumeradores.TipoAcessoMobile.Transportador:
                    return "Transportador";
                case ObjetosDeValor.Embarcador.Enumeradores.TipoAcessoMobile.Cliente:
                    return "Cliente";
                default:
                    return "";
            }
        }
    }
}
