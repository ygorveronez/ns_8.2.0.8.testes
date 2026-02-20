namespace Dominio.ObjetosDeValor.Embarcador.Logistica.GrupoMotoristas
{
    public class FiltroPesquisaRelacionamentosGrupoMotoristas
    {
        public virtual int CodigoGrupoMotoristas { get; set; }

        public Enumeradores.SituacaoIntegracao? SituacaoIntegracao { get; set; }

        public Enumeradores.TipoIntegracao TipoIntegracao { get; set; }
    }
}
