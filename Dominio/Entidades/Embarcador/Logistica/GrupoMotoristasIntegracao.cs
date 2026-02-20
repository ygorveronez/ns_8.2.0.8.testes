using Dominio.Interfaces.Embarcador.Integracao;
using NHibernate.Mapping.Attributes;
using System.Collections.Generic;

namespace Dominio.Entidades.Embarcador.Logistica
{
    [Class(0, Table = "T_GRUPO_MOTORISTAS_INTEGRACAO", EntityName = "GrupoMotoristasIntegracao", DynamicUpdate = true, Name = "Dominio.Entidades.Embarcador.Logistica.GrupoMotoristasIntegracao", NameType = typeof(GrupoMotoristasIntegracao))]
    public class GrupoMotoristasIntegracao : Integracao.Integracao, IIntegracaoComArquivo<GrupoMotoristasIntegracaoArquivos>, Interfaces.Embarcador.Logistica.GrupoMotoristas.IEntidadeRelacionamentoGrupoMotoristas
    {
        [Id(Name = "Codigo", Type = "int", Column = "GMI_CODIGO")]
        [Generator(Class = "native")]
        public virtual int Codigo { get; set; }

        [ManyToOne(0, Class = "GrupoMotoristas", Column = "GMO_CODIGO", NotNull = true, Lazy = Laziness.Proxy)]
        public virtual GrupoMotoristas GrupoMotoristas { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Tipo", Column = "GMO_TIPO", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracaoGrupoMotorista), NotNull = false)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracaoGrupoMotorista Tipo { get; set; }

        [NHibernate.Mapping.Attributes.Set(0, Name = "ArquivosTransacao", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_GRUPO_MOTORISTAS_INTEGRACAO_ARQUIVOS_ARQUIVOS")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "GMI_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "GrupoMotoristasIntegracaoArquivos", Column = "GMA_CODIGO")]
        public virtual ICollection<GrupoMotoristasIntegracaoArquivos> ArquivosTransacao { get; set; }

        public virtual string Descricao { get { return DescricaoSituacaoIntegracao; } }
    }
}