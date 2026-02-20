using System;

namespace Dominio.Entidades.Embarcador.Cargas
{
    [NHibernate.Mapping.Attributes.Class(0, DynamicUpdate = true, Table = "T_CARGA_JANELA_CARREGAMENTO_TRANSPORTADOR_TERMO_ACEITE", EntityName = "CargaJanelaCarregamentoTransportadorTermoAceite", Name = "Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoTransportadorTermoAceite", NameType = typeof(CargaJanelaCarregamentoTransportadorTermoAceite))]
    public class CargaJanelaCarregamentoTransportadorTermoAceite : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "TTA_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "CargaJanelaCarregamentoTransportador", Column = "JCT_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual CargaJanelaCarregamentoTransportador CargaJanelaCarregamentoTransportador { get; set; }
        
        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Carga", Column = "CAR_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Carga Carga { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TTA_TERMO_ACEITE", Type = "StringClob", NotNull = true)]
        public virtual string TermoAceite { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Usuario", Column = "FUN_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Usuario UsuarioResponsavel { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TTA_DATA", TypeType = typeof(DateTime), NotNull = true)]
        public virtual DateTime Data { get; set; }

        public virtual string Descricao
        {
            get { return $"Termo de aceite {this.Carga?.CodigoCargaEmbarcador}" ?? string.Empty; }
        }
    }
}
