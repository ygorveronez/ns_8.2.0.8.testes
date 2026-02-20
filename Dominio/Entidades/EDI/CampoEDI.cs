namespace Dominio.Entidades
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_EDI_CAMPO", EntityName = "CampoEDI", Name = "Dominio.Entidades.CampoEDI", NameType = typeof(CampoEDI))]
    public class CampoEDI : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "CAM_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Descricao", Column = "CAM_DESCRICAO", TypeType = typeof(string), Length = 1000, NotNull = true)]
        public virtual string Descricao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Tipo", Column = "CAM_TIPO", TypeType = typeof(Enumeradores.TipoCampoEDI), NotNull = true)]
        public virtual Enumeradores.TipoCampoEDI Tipo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Condicao", Column = "CAM_CONDICAO", TypeType = typeof(Enumeradores.CondicaoCampoEDI), NotNull = true)]
        public virtual Enumeradores.CondicaoCampoEDI Condicao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Indice", Column = "CAM_INDICE", TypeType = typeof(int), NotNull = false)]
        public virtual int Indice { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "QuantidadeCaracteres", Column = "CAM_QTD_CARACTERES", TypeType = typeof(int), NotNull = false)]
        public virtual int QuantidadeCaracteres { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "QuantidadeInteiros", Column = "CAM_QTD_INTEIROS", TypeType = typeof(int), NotNull = false)]
        public virtual int QuantidadeInteiros { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "QuantidadeDecimais", Column = "CAM_QTD_DECIMAIS", TypeType = typeof(int), NotNull = false)]
        public virtual int QuantidadeDecimais { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Alinhamento", Column = "CAM_ALINHAMENTO", TypeType = typeof(Enumeradores.AlinhamentoCampoEDI), NotNull = true)]
        public virtual Enumeradores.AlinhamentoCampoEDI Alinhamento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "IdentificadorRegistro", Column = "CAM_IDENTIFICADOR_REGISTRO", TypeType = typeof(string), Length = 20, NotNull = false)]
        public virtual string IdentificadorRegistro { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorFixo", Column = "CAM_VALOR_FIXO", TypeType = typeof(string), Length = 1000, NotNull = true)]
        public virtual string ValorFixo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Objeto", Column = "CAM_OBJETO", TypeType = typeof(Enumeradores.ObjetoCampoEDI), Length = 1000, NotNull = false)]
        public virtual Enumeradores.ObjetoCampoEDI Objeto { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PropriedadeObjeto", Column = "CAM_PROPRIEDADE_OBJETO", TypeType = typeof(string), Length = 1000, NotNull = false)]
        public virtual string PropriedadeObjeto { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PropriedadeObjetoPai", Column = "CAM_PROPRIEDADE_OBJETO_PAI", TypeType = typeof(string), Length = 1000, NotNull = false)]
        public virtual string PropriedadeObjetoPai { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Ordem", Column = "CAM_ORDEM", TypeType = typeof(int), NotNull = true)]
        public virtual int Ordem { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Mascara", Column = "CAM_MASCARA", TypeType = typeof(string), Length = 30, NotNull = true)]
        public virtual string Mascara { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Repetir", Column = "CAM_REPETIR", TypeType = typeof(bool), NotNull = true)]
        public virtual bool Repetir { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Status", Column = "CAM_STATUS", TypeType = typeof(string), Length = 1, NotNull = true)]
        public virtual string Status { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "IdentificadorRegistroPai", Column = "CAM_REGISTRO_PAI", TypeType = typeof(string), Length = 20, NotNull = false)]
        public virtual string IdentificadorRegistroPai { get; set; }
        
        [NHibernate.Mapping.Attributes.Property(0, Name = "Expressao", Column = "CAM_EXPRESSAO", TypeType = typeof(string), Length = 4000, NotNull = false)]
        public virtual string Expressao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NaoEscreverRegistro", Column = "CAM_NAO_ESCREVER_REGISTRO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool NaoEscreverRegistro { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CAM_REMOVER_CARACTERES_ESPECIAIS", TypeType = typeof(bool), NotNull = false)]
        public virtual bool RemoverCaracteresEspeciais { get; set; }
    }
}
