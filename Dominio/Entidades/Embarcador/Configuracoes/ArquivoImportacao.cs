namespace Dominio.Entidades.Embarcador.Configuracoes
{
    public abstract class ArquivoImportacao<TArquivo> : EntidadeBase where TArquivo : EntidadeBase
    {
        public abstract int Codigo { get; set; }

        public abstract TArquivo Arquivo { get; set; }

        public abstract string Propriedade { get; set; }

        public abstract int Posicao { get; set; }

        public abstract Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCampo TipoPropriedade { get; set; }

        public virtual string Descricao
        {
            get
            {
                return this.Posicao.ToString() + " - " + this.Propriedade;
            }
        }
    }
}
