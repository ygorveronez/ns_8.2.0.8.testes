namespace Dominio.ObjetosDeValor.Embarcador.Pedido
{
    public class FiltroPesquisaNavio
    {
        public string Descricao { get; set; }

        public string CodigoIntegracao { get; set; }

        public string CodigoIRIN { get; set; }

        public Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa  Status { get; set; }

        public Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEmbarcacao TipoEmbarcacao { get; set; }

    }
}
