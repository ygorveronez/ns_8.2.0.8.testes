namespace Dominio.Entidades.Embarcador.Frete.Pontuacao
{
    public abstract class PontuacaoBase : EntidadeBase
    {
        public abstract string Descricao { get; }

        public abstract int Pontuacao { get; set; }

        public virtual int PontuacaoConvertida {
            get { return Pontuacao; }
        }
    }
}
