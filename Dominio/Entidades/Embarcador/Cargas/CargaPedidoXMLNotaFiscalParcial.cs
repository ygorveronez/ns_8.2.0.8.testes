using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;

namespace Dominio.Entidades.Embarcador.Cargas
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_CARGA_PEDIDO_XML_NOTA_FISCAL_PARCIAL", EntityName = "CargaPedidoXMLNotaFiscalParcial", Name = "Dominio.Entidades.Embarcador.Cargas.CargaPedidoXMLNotaFiscalParcial", NameType = typeof(CargaPedidoXMLNotaFiscalParcial))]
    public class CargaPedidoXMLNotaFiscalParcial : EntidadeBase, IEquatable<Dominio.Entidades.Embarcador.Cargas.CargaPedidoXMLNotaFiscalParcial>
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "CFP_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "CargaPedido", Column = "CPE_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Cargas.CargaPedido CargaPedido { get; set; }
        
        [NHibernate.Mapping.Attributes.Property(0, Name = "Numero", Column = "CFP_NUMERO", TypeType = typeof(int), NotNull = false)]
        public virtual int Numero { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Pedido", Column = "CPF_NUMERO_PEDIDO", TypeType = typeof(string), Length = 50, NotNull = false)]
        public virtual string Pedido { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Chave", Column = "CPF_CHAVE", TypeType = typeof(string), Length = 50, NotNull = false)]
        public virtual string Chave { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NotaEnviadaIntegralmente", Column = "CPF_NOTA_ENVIADA_INTEGRALMENTE", TypeType = typeof(bool), NotNull = false)]
        public virtual bool NotaEnviadaIntegralmente { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "XMLNotaFiscal", Column = "NFX_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal XMLNotaFiscal { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CFP_VINCULAR_NOTA_FISCAL_POR_PROCESSO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool VincularNotaFiscalPorProcesso { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CFP_DATA_ULTIMA_TENTATIVA_VINCULO", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataUltimaTentativaVinculo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CFP_NUMERO_TENTATIVAS_VINCULO", TypeType = typeof(int), NotNull = false)]
        public virtual int NumeroTentativasVinculo { get; set; }

        public virtual string Descricao => $"Pedido: {Pedido} - Número: {Numero}";

        //FUCK!! JA TINHA UMA CHAVE EM CIMA WHATA FUCK CRIAR ESSA
        //[NHibernate.Mapping.Attributes.Property(0, Name = "ChaveNota", Column = "CPF_CHAVE_NOTA", TypeType = typeof(string), Length = 44, NotNull = false)]
        //public virtual string ChaveNota { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Status", Column = "CPF_STATUS", TypeType = typeof(Dominio.ObjetosDeValor.Embarcador.Enumeradores.StatusNfe), NotNull = false)]
        public virtual Dominio.ObjetosDeValor.Embarcador.Enumeradores.StatusNfe Status { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CFP_NUMERO_FATURA", TypeType = typeof(string), Length = 250, NotNull = false)]
        public virtual string NumeroFatura { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CFP_TIPO_NOTA_FISCAL_INTEGRADA", TypeType = typeof(TipoNotaFiscalIntegrada), NotNull = false)]
        public virtual TipoNotaFiscalIntegrada TipoNotaFiscalIntegrada { get; set; }

        public virtual bool Equals(CargaPedidoXMLNotaFiscalParcial other)
        {
            if (other.Codigo == this.Codigo)
                return true;
            else
                return false;
        }

    }
}
