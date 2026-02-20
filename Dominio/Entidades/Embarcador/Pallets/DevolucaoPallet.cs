using Dominio.Interfaces.Embarcador.Entidade;
using System;
using System.Collections.Generic;

namespace Dominio.Entidades.Embarcador.Pallets
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_PALLET_DEVOLUCAO", EntityName = "DevolucaoPallet", Name = "Dominio.Entidades.Embarcador.Pallets.DevolucaoPallet", NameType = typeof(DevolucaoPallet))]
    public class DevolucaoPallet : EntidadeBase, IEntidade
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "PDE_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NumeroDevolucao", Column = "PDE_NUMERO_DEVOLUCAO", TypeType = typeof(int), NotNull = true)]
        public virtual int NumeroDevolucao { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "CargaPedido", Column = "CPE_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Cargas.CargaPedido CargaPedido { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "XMLNotaFiscal", Column = "NFX_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Pedidos.XMLNotaFiscal XMLNotaFiscal { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Empresa", Column = "EMP_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Empresa Transportador { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Filial", Column = "FIL_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Filiais.Filial Filial { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Usuario", Column = "FUN_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Usuario Usuario { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Cliente", Column = "CLI_CGCCPF", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Cliente Cliente { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataDevolucao", Column = "PDE_DATA_DEVOLUCAO", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataDevolucao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "QuantidadePallets", Column = "PDE_QUANTIDADE_PALLETS", TypeType = typeof(int), NotNull = true)]
        public virtual int QuantidadePallets { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PesoTotalPallets", Column = "PDE_PESO_TOTAL_PALLETS", TypeType = typeof(decimal), Scale = 6, Precision = 18, NotNull = false)]
        public virtual decimal PesoTotalPallets { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorTotalPallets", Column = "PDE_VALOR_TOTAL_PALLETS", TypeType = typeof(decimal), Scale = 3, Precision = 18, NotNull = false)]
        public virtual decimal ValorTotalPallets { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Observacao", Column = "PDE_OBSERVACAO", TypeType = typeof(string), Length = 400, NotNull = true)]
        public virtual string Observacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Situacao", Column = "PDE_SITUACAO", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.SituacaoDevolucaoPallet), NotNull = true)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.SituacaoDevolucaoPallet Situacao { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Name = "Situacoes", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_PALLET_DEVOLUCAO_SITUACAO")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "PDE_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "DevolucaoPalletSituacao", Column = "PDS_CODIGO")]
        public virtual IList<DevolucaoPalletSituacao> Situacoes { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "FechamentoPallets", Column = "FEP_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual FechamentoPallets Fechamento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "PDE_ADICIONAR_FECHAMENTO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool AdicionarAoFechamento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataTransporte", Formula = @"
        (
             select isnull(Carga.CAR_DATA_CARREGAMENTO, XmlNotaFiscal.NF_DATA_EMISSAO) 
               from T_PALLET_DEVOLUCAO DevolucaoPallet
               left join T_CARGA_PEDIDO CargaPedido on CargaPedido.CPE_CODIGO = DevolucaoPallet.CPE_CODIGO
               left join T_CARGA Carga on Carga.CAR_CODIGO = CargaPedido.CAR_CODIGO
               left join T_XML_NOTA_FISCAL XmlNotaFiscal on XmlNotaFiscal.NFX_CODIGO = DevolucaoPallet.NFX_CODIGO
              where DevolucaoPallet.PDE_CODIGO = PDE_CODIGO
        )", TypeType = typeof(DateTime), Lazy = true)]
        public virtual DateTime? DataTransporte { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "MotivoContestacao", Column = "PDE_MOTIVO_CONTESTACAO", TypeType = typeof(string), Length = 400, NotNull = false)]
        public virtual string MotivoContestacao { get; set; }

        public virtual string Descricao
        {
            get { return $"{(CargaPedido == null ? "Sem carga" : $"Carga: {CargaPedido.Carga.CodigoCargaEmbarcador}")}{(XMLNotaFiscal == null ? "" : $" | NF-e {XMLNotaFiscal.Numero}")}"; }
        }

        public virtual string DescricaoSituacao
        {
            get
            {
                switch (Situacao)
                {
                    case ObjetosDeValor.Embarcador.Enumeradores.SituacaoDevolucaoPallet.AgEntrega:
                        return "Ag. Entrega";
                    case ObjetosDeValor.Embarcador.Enumeradores.SituacaoDevolucaoPallet.Entregue:
                        return "Entregue";
                    case ObjetosDeValor.Embarcador.Enumeradores.SituacaoDevolucaoPallet.Liquidado:
                        return ObjetosDeValor.Embarcador.Enumeradores.SituacaoDevolucaoPalletHelper.ObterDescricao(Situacao);
                    default:
                        return string.Empty;
                }
            }
        }
    }
}
