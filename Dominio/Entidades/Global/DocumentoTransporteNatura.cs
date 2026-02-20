using System;
using System.Collections.Generic;

namespace Dominio.Entidades
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_NATURA_DOCUMENTO_TRANSPORTE", EntityName = "DocumentoTransporteNatura", Name = "Dominio.Entidades.DocumentoTransporteNatura", NameType = typeof(DocumentoTransporteNatura))]
    public class DocumentoTransporteNatura : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "DTN_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Empresa", Column = "EMP_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Empresa Empresa { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Veiculo", Column = "VEI_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Veiculo Veiculo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Usuario", Column = "FUN_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Usuario Motorista { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "TipoDeCarga", Column = "TCG_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Embarcador.Cargas.TipoDeCarga TipoCarga { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "TipoOperacao", Column = "TOP_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Embarcador.Pedidos.TipoOperacao TipoOperacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NumeroDT", Column = "DTN_NUMERO", TypeType = typeof(long), NotNull = true)]
        public virtual long NumeroDT { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataEmissao", Column = "DTN_DATA_EMISSAO", TypeType = typeof(DateTime), NotNull = true)]
        public virtual DateTime DataEmissao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataConsulta", Column = "DTN_DATA_CONSULTA", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataConsulta { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataRetorno", Column = "DTN_DATA_RETORNO", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataRetorno { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorFrete", Column = "DTN_VALOR_FRETE", TypeType = typeof(decimal), Scale = 6, Precision = 18, NotNull = true)]
        public virtual decimal ValorFrete { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorFreteCalculado", Column = "DTN_VALOR_FRETE_CALCULADO", TypeType = typeof(decimal), Scale = 6, Precision = 18, NotNull = true)]
        public virtual decimal ValorFreteCalculado { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Status", Column = "DTN_STATUS", TypeType = typeof(ObjetosDeValor.Enumerador.StatusDocumentoTransporteNatura), NotNull = true)]
        public virtual ObjetosDeValor.Enumerador.StatusDocumentoTransporteNatura Status { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Tipo", Column = "DTN_TIPO", TypeType = typeof(ObjetosDeValor.Enumerador.TipoDocumentoTransporteNatura), NotNull = false)]
        public virtual ObjetosDeValor.Enumerador.TipoDocumentoTransporteNatura Tipo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "XML", Column = "DTN_XML", Type = "StringClob", NotNull = false)]
        public virtual string XML { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Observacao", Column = " DTN_OBSERVACAO", TypeType = typeof(string), Length = 1000, NotNull = false)]
        public virtual string Observacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NumeroNotas", Formula = @"SUBSTRING((SELECT ', ' + CAST(notas.NDT_NUMERO AS NVARCHAR(20))
                                                                                    FROM T_NATURA_DOCUMENTO_TRANSPORTE_NOTA_FISCAL notas
                                                                                    WHERE notas.DTN_CODIGO = DTN_CODIGO FOR XML PATH('')), 3, 1000)", TypeType = typeof(string), Lazy = true)]
        public virtual string NumeroNotas { get; set; }

        [NHibernate.Mapping.Attributes.Set(0, Name = "NaturaXMLs", Cascade = "all", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.False, Table = "T_NATURA_DOCUMENTO_TRANSPORTE_XML")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "DTN_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "NaturaXML", Column = "NAX_CODIGO")]
        public virtual ICollection<NaturaXML> NaturaXMLs { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Name = "NotasFiscais", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_NATURA_DOCUMENTO_TRANSPORTE_NOTA_FISCAL")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "DTN_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "NotaFiscalDocumentoTransporteNatura", Column = "NDT_CODIGO")]
        public virtual IList<NotaFiscalDocumentoTransporteNatura> NotasFiscais { get; set; }

        public virtual string DescricaoStatus
        {
            get
            {
                switch (Status)
                {
                    case ObjetosDeValor.Enumerador.StatusDocumentoTransporteNatura.EmDigitacao:
                        return "Em Digitação";
                    case ObjetosDeValor.Enumerador.StatusDocumentoTransporteNatura.EmEmissao:
                        return "Em Emissão";
                    case ObjetosDeValor.Enumerador.StatusDocumentoTransporteNatura.Emitido:
                        return "Emitido";
                    case ObjetosDeValor.Enumerador.StatusDocumentoTransporteNatura.Finalizado:
                        return "Finalizado";
                    case ObjetosDeValor.Enumerador.StatusDocumentoTransporteNatura.Retornado:
                        return "Retornado";
                    case ObjetosDeValor.Enumerador.StatusDocumentoTransporteNatura.Cancelado:
                        return "Cancelado";
                    case ObjetosDeValor.Enumerador.StatusDocumentoTransporteNatura.Erro:
                        return "Erro Consulta";
                    case ObjetosDeValor.Enumerador.StatusDocumentoTransporteNatura.AguardandoEmissaoAutomatica:
                        return "Ag. emissão automática";
                    case ObjetosDeValor.Enumerador.StatusDocumentoTransporteNatura.AguardandoRetornoAutomatico:
                        return "Ag. retorno automático";
                    default:
                        return "";
                }
            }
        }
    }
}
