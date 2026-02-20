namespace Dominio.ObjetosDeValor.Embarcador.Pedido.DadosPedido
{
    public class Localidade
    {
        #region Propriedades

        public int Codigo { get; set; }

        public int CodigoIbge { get; set; }

        public string Descricao { get; set; }

        public Estado Estado { get; set; }

        public Pais Pais { get; set; }

        #endregion Propriedades

        #region Propriedades com Regras

        public string DescricaoCidadeEstado
        {
            get
            {
                if ((CodigoIbge != 9999999) || (Pais == null))
                    return $"{Descricao} - {Estado.Sigla}";

                if (Pais.Abreviacao != null)
                    return $"{Descricao} - {Pais.Abreviacao}";
                
                return $"{Descricao} - {Pais.Nome}";
            }
        }

        #endregion Propriedades com Regras
    }
}
