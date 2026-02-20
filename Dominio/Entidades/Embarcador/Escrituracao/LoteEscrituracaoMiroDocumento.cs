using System;

namespace Dominio.Entidades.Embarcador.Escrituracao
{
    [Obsolete("Classe nao deve ser usada. Informações da MIRO estao em DocumentoFaturamento.cs")]
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_LOTE_ESCRITURACAO_MIRO_DOCUMENTO", EntityName = "LoteEscrituracaoMiroDocumento", Name = "Dominio.Entidades.Embarcador.Escrituracao.LoteEscrituracaoMiroDocumento", NameType = typeof(LoteEscrituracaoMiroDocumento))]
    public class LoteEscrituracaoMiroDocumento : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "LED_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ControleDocumento", Column = "COD_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Documentos.ControleDocumento ControleDocumento { get; set; }       
        
        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "LoteEscrituracaoMiro", Column = "LEM_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Escrituracao.LoteEscrituracaoMiro LoteEscrituracaoMiro { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ChaveDocumento", Column = "LED_CHAVE_DOCUMENTO", TypeType = typeof(string), NotNull = false, Length = 50)]
        public virtual string ChaveDocumento { get; set; }    
        
        [NHibernate.Mapping.Attributes.Property(0, Name = "NumeroFolha", Column = "LED_NUMERO_FOLHA", TypeType = typeof(string), NotNull = false, Length = 50)]
        public virtual string NumeroFolha { get; set; }
        
        [NHibernate.Mapping.Attributes.Property(0, Name = "NumeroMiro", Column = "LED_NUMERO_MIRO", TypeType = typeof(string), NotNull = false, Length = 50)]
        public virtual string NumeroMiro { get; set; }      
        
        [NHibernate.Mapping.Attributes.Property(0, Name = "NumeroEstorno", Column = "LED_NUMERO_ESTORNO", TypeType = typeof(string), NotNull = false, Length = 50)]
        public virtual string NumeroEstorno { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Bloqueio", Column = "LED_BLOQUEIO", TypeType = typeof(string), NotNull = false, Length = 50)]
        public virtual string Bloqueio { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataMiro", Column = "LED_DATA_MIRO", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataMiro { get; set; }      
        
        [NHibernate.Mapping.Attributes.Property(0, Name = "Vencimento", Column = "LED_VENCIMENTO", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? Vencimento { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "TermosPagamento", Column = "TPG_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Frete.TermosPagamento TermosPagamento { get; set; }
    }
}
