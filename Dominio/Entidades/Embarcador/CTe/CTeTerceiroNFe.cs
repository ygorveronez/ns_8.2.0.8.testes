using System;

namespace Dominio.Entidades.Embarcador.CTe
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_CTE_TERCEIRO_NFE", EntityName = "CTeTerceiroNFe", Name = "Dominio.Entidades.Embarcador.CTe.CTeTerceiroNFe", NameType = typeof(CTeTerceiroNFe))]
    public class CTeTerceiroNFe : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "CNE_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "CTeTerceiro", Column = "CPS_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual CTeTerceiro CTeTerceiro { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CNE_CHAVE", TypeType = typeof(string), Length = 44, NotNull = true)]
        public virtual string Chave { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CNE_NUMERO", TypeType = typeof(string), Length = 20, NotNull = false)]
        public virtual string Numero { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CNE_SERIE", TypeType = typeof(string), Length = 3, NotNull = false)]
        public virtual string Serie { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CNE_DATA_EMISSAO", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataEmissao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CNE_VALOR_TOTAL", TypeType = typeof(decimal), Scale = 6, Precision = 18, NotNull = false)]
        public virtual decimal ValorTotal { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CNE_VOLUMES", TypeType = typeof(decimal), Scale = 6, Precision = 18, NotNull = false)]
        public virtual decimal Volumes { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CNE_PESO", TypeType = typeof(decimal), Scale = 6, Precision = 18, NotNull = false)]
        public virtual decimal Peso { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CNE_PESO_CUBADO", TypeType = typeof(decimal), Scale = 6, Precision = 18, NotNull = false)]
        public virtual decimal PesoCubado { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CNE_NUMERO_ROMANEIO", TypeType = typeof(string), Length = 150, NotNull = false)]
        public virtual string NumeroRomaneio { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CNE_NUMERO_PEDIDO", TypeType = typeof(string), Length = 150, NotNull = false)]
        public virtual string NumeroPedido { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CNE_PROTOCOLO", TypeType = typeof(string), Length = 20, NotNull = false)]
        public virtual string Protocolo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CNE_PROTOCOLO_CLIENTE", TypeType = typeof(string), Length = 150, NotNull = false)]
        public virtual string ProtocoloCliente { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CNE_NUMERO_REFERENCIA_EDI", TypeType = typeof(string), Length = 50, NotNull = false)]
        public virtual string NumeroReferenciaEDI { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CNE_PIN_SUFRAMA", TypeType = typeof(string), Length = 50, NotNull = false)]
        public virtual string PINSuframa { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CNE_NCM", TypeType = typeof(string), Length = 20, NotNull = false)]
        public virtual string NCM { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CNE_NUMERO_CONTROLE_CLIENTE", TypeType = typeof(string), Length = 50, NotNull = false)]
        public virtual string NumeroControleCliente { get; set; }

        public virtual CTeTerceiroNFe Clonar()
        {
            return (CTeTerceiroNFe)this.MemberwiseClone();
        }
    }
}
