using NHibernate.Mapping.Attributes;

namespace Dominio.Entidades.Embarcador.Logistica
{
    [Class(0, Table = "T_GRUPO_MOTORISTAS_TIPO_INTEGRACAO", EntityName = "GrupoMotoristasTipoIntegracao", DynamicUpdate = true, Name = "Dominio.Entidades.Embarcador.Logistica.GrupoMotoristasTipoIntegracao", NameType = typeof(GrupoMotoristasTipoIntegracao))]
    public class GrupoMotoristasTipoIntegracao : EntidadeBase, Interfaces.Embarcador.Logistica.GrupoMotoristas.IEntidadeRelacionamentoGrupoMotoristas
    {
        [Id(Name = "Codigo", Type = "int", Column = "GMT_CODIGO")]
        [Generator(Class = "native")]
        public virtual int Codigo { get; set; }

        [ManyToOne(0, Class = "GrupoMotoristas", Column = "GMO_CODIGO", NotNull = true, Lazy = Laziness.Proxy)]
        public virtual GrupoMotoristas GrupoMotoristas { get; set; }

        [Property(Name = "TipoIntegracao", Column = "GMT_TIPO", TypeType = typeof(int), NotNull = true)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao TipoIntegracao { get; set; }

        public virtual string Descricao { get { return ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracaoHelper.ObterDescricao(TipoIntegracao); } }

    }
}