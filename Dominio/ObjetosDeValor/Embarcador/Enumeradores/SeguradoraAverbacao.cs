namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum SeguradoraAverbacao
    {
        NaoDefinido = 0,
        ATM = 1,
        Bradesco = 2,
        PortoSeguro = 3,
        ELT = 4,
        Senig = 5
    }

    public static class SeguradoraAverbacaoHelper
    {
        public static string Descricao(this SeguradoraAverbacao seguradoraAverbacao)
        {
            switch (seguradoraAverbacao)
            {
                case SeguradoraAverbacao.NaoDefinido:
                    return "Não definido";
                case SeguradoraAverbacao.ATM:
                    return "ATM";
                case SeguradoraAverbacao.Bradesco:
                    return "Bradesco";
                case SeguradoraAverbacao.PortoSeguro:
                    return "Porto Seguro";
                case SeguradoraAverbacao.ELT:
                    return "ALT";
                case SeguradoraAverbacao.Senig:
                    return "Senig";
                default:
                    return string.Empty;
            }
        }
        public static string ObterSigla(this SeguradoraAverbacao seguradoraAverbacao)
        {
            switch (seguradoraAverbacao)
            {
                case SeguradoraAverbacao.ATM:
                    return "A";
                case SeguradoraAverbacao.Bradesco:
                    return "B";
                case SeguradoraAverbacao.PortoSeguro:
                    return "P";
                case SeguradoraAverbacao.Senig:
                    return "S";
                default:
                    return string.Empty;
            }
        }
    }
}
