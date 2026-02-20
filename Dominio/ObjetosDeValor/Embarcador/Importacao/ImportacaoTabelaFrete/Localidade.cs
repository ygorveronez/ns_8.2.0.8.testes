namespace Dominio.ObjetosDeValor.Embarcador.Importacao.ImportacaoTabelaFrete
{
    public sealed class Localidade
    {
        public int Codigo { get; set; }

        public int CodigoIbge { get; set; }

        public string Descricao { get; set; }

        public string EstadoSigla { get; set; }

        public string PaisAbreviacao { get; set; }

        public string PaisNome { get; set; }

        public bool PossuiPais { get; set; }

        public string DescricaoCidadeEstado
        {
            get
            {
                if ((CodigoIbge != 9999999) || !PossuiPais)
                    return $"{Descricao} - {EstadoSigla}";
                else if (!string.IsNullOrWhiteSpace(PaisAbreviacao))
                    return $"{Descricao} - {PaisAbreviacao}";

                return $"{Descricao} - {PaisNome}";
            }
        }
    }
}
