using System;

namespace Dominio.Entidades.Embarcador.Veiculos
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_VEICULO_TOLERANCIA_PESO_AUTORIZACAO_CARGA", EntityName = "VeiculoToleranciaPesoAutorizacaoCarga", Name = "Dominio.Entidades.Embarcador.Veiculos.VeiculoToleranciaPesoAutorizacaoCarga", NameType = typeof(VeiculoToleranciaPesoAutorizacaoCarga))]
    public class VeiculoToleranciaPesoAutorizacaoCarga : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "AVT_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }
        
        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Carga", Column = "CAR_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Cargas.Carga Carga { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ModeloVeicularCarga", Column = "MVC_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Cargas.ModeloVeicularCarga ModeloVeicularCarga { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Usuario", Column = "FUN_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Usuario Usuario { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "AVT_DATA_HORA", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? Data { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "AVT_SITUACAO", TypeType = typeof(Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAutorizacaoPesoCarga), NotNull = true)]
        public virtual Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAutorizacaoPesoCarga Situacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "AVT_MOTIVO_EXTORNO_AUTORIZACAO", TypeType = typeof(string), Length = 250, NotNull = false)]
        public virtual string MotivoExtornoAutorizacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "AVT_MENSAGEM", TypeType = typeof(string), Length = 500, NotNull = false)]
        public virtual string Mensagem { get; set; }

        public virtual string Descricao
        {
            get
            {
                return Carga.CodigoCargaEmbarcador;
            }
        }
    }
}