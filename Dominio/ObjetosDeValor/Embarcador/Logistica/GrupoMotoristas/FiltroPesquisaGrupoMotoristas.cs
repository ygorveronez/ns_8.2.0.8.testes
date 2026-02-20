namespace Dominio.ObjetosDeValor.Embarcador.Logistica.GrupoMotoristas
{
    public class FiltroPesquisaGrupoMotoristas
    {
        public virtual Enumeradores.SituacaoIntegracaoGrupoMotoristas? Situacao { get; set; }
        public virtual bool? Ativo { get; set; }
        public virtual string Descricao { get; set; }
        public virtual string CodigoIntegracao { get; set; }
    }
}
