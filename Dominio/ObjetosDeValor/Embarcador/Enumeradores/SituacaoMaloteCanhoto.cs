namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum SituacaoMaloteCanhoto
    {
        Gerado = 1,
        Cancelado = 2,
        Confirmado = 3,
        Inconsistente = 4
    }

    public static class SituacaoMaloteCanhotoDescricao
    {
        public static string Descricao(this SituacaoMaloteCanhoto situacao)
        {
            switch (situacao)
            {
                case SituacaoMaloteCanhoto.Gerado: return "Gerado";
                case SituacaoMaloteCanhoto.Cancelado: return "Cancelado";
                case SituacaoMaloteCanhoto.Confirmado: return "Confirmado";
                case SituacaoMaloteCanhoto.Inconsistente: return "Inconsistente";
                default: return "";
            }
        }
    }
}