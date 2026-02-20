namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum SituacaoManobraTracao
    {
        EmIntervalo = 1,
        EmManobra = 2,
        Ocioso = 3,
        SemMotorista = 4
    }

    public static class SituacaoManobraTracaoHelper
    {
        public static string ObterClasseCor(this SituacaoManobraTracao situacao)
        {
            switch (situacao)
            {
                case SituacaoManobraTracao.EmIntervalo: return "well-yellow";
                case SituacaoManobraTracao.EmManobra: return "well-blue";
                case SituacaoManobraTracao.Ocioso: return "well-green";
                case SituacaoManobraTracao.SemMotorista: return "well-white";
                default: return string.Empty;
            }
        }

        public static string ObterDescricao(this SituacaoManobraTracao situacao)
        {
            switch (situacao)
            {
                case SituacaoManobraTracao.EmIntervalo: return "Em Intervalo";
                case SituacaoManobraTracao.EmManobra: return "Em Manobra";
                case SituacaoManobraTracao.Ocioso: return "Ocioso";
                case SituacaoManobraTracao.SemMotorista: return "Sem Motorista";
                default: return string.Empty;
            }
        }
    }
}
