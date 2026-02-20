namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum ChamadoResponsavelOcorrencia
    {
        Logistica = 1,
        Fabrica = 2,
        Comercial = 3,
        Representante = 4,
        Cliente = 5,
        CD = 6,
        Qualidade = 7,
        Fiscal = 8,
        Transportador = 9
    }

    public static class ChamadoResponsavelOcorrenciaHelper
    {
        public static string ObterDescricao(this ChamadoResponsavelOcorrencia responsavel)
        {
            switch (responsavel)
            {
                case ChamadoResponsavelOcorrencia.Comercial:
                    return "Comercial";
                case ChamadoResponsavelOcorrencia.Fabrica:
                    return "Fábrica";
                case ChamadoResponsavelOcorrencia.Logistica:
                    return "Logística";
                case ChamadoResponsavelOcorrencia.Representante:
                    return "Representante";
                case ChamadoResponsavelOcorrencia.Cliente:
                    return "Cliente";
                case ChamadoResponsavelOcorrencia.CD:
                    return "CD";
                case ChamadoResponsavelOcorrencia.Qualidade:
                    return "Qualidade";
                case ChamadoResponsavelOcorrencia.Fiscal:
                    return "Fiscal";
                case ChamadoResponsavelOcorrencia.Transportador:
                    return "Transportador";
                default:
                    return string.Empty;
            }
        }
    }
}
