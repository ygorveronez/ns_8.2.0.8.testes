using System;


namespace Dominio.Entidades
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_NATURA_XML", EntityName = "NaturaXML", Name = "Dominio.Entidades.NaturaXML", NameType = typeof(NaturaXML))]
    public class NaturaXML : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "NAX_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Data", Column = "NAX_DATA", TypeType = typeof(DateTime), NotNull = true)]
        public virtual DateTime Data { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Tipo", Column = "NAX_TIPO", TypeType = typeof(ObjetosDeValor.Enumerador.TipoXMLNatura), NotNull = true)]
        public virtual ObjetosDeValor.Enumerador.TipoXMLNatura Tipo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Usuario", Column = "NAX_USUARIO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Usuario Usuario { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "XMLEnvio", Column = "NAX_XML_ENVIO", Type = "StringClob", NotNull = false)]
        public virtual string XMLEnvio { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "XMLRetorno", Column = "NAX_XML_RETORNO", Type = "StringClob", NotNull = false)]
        public virtual string XMLRetorno { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Mensagem", Column = "NAX_MENSAGEM", TypeType = typeof(string), Length = 4000, NotNull = false)]
        public virtual string Mensagem { get; set; }

        public virtual string DescricaoTipo
        {
            get
            {
                switch (Tipo)
                {
                    case ObjetosDeValor.Enumerador.TipoXMLNatura.ConsultaDocumentoTransporte:
                        return "Consulta Documento Transporte";
                    case ObjetosDeValor.Enumerador.TipoXMLNatura.ConsultaPreFatura:
                        return "Consulta Pré-Fatura";
                    case ObjetosDeValor.Enumerador.TipoXMLNatura.RetornoDocumentoTransporte:
                        return "Retorno Documento Transporte";
                    case ObjetosDeValor.Enumerador.TipoXMLNatura.RetornoDocumentoTransporteComplementar:
                        return "Retorno Documento Transporte Complementar";
                    case ObjetosDeValor.Enumerador.TipoXMLNatura.EnvioFatura:
                        return "Envio Fatura";
                    default:
                        return "";
                }
            }
        }
    }
}
