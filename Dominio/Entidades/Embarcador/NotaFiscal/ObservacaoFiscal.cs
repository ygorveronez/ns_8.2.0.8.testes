using System;

namespace Dominio.Entidades.Embarcador.NotaFiscal
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_OBSERVACOES_FISCAIS", EntityName = "ObservacaoFiscal", Name = "Dominio.Entidades.Embarcador.NotaFiscal.ObservacaoFiscal", NameType = typeof(ObservacaoFiscal))]
    public class ObservacaoFiscal : EntidadeBase, IEquatable<Dominio.Entidades.Embarcador.NotaFiscal.ObservacaoFiscal>
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "OBF_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Observacao", Column = "OBF_OBSERVACAO", TypeType = typeof(string), Length = 2000, NotNull = false)]
        public virtual string Observacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NCMProduto", Column = "OBF_NCM_PRODUTO", TypeType = typeof(string), Length = 8, NotNull = false)]
        public virtual string NCMProduto { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CSTICMS", Column = "OBF_CST_CSOSN", TypeType = typeof(Dominio.ObjetosDeValor.Embarcador.Enumeradores.CSTICMS), NotNull = false)]
        public virtual Dominio.ObjetosDeValor.Embarcador.Enumeradores.CSTICMS? CSTICMS { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Estado", Column = "UF_SIGLA", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Estado Estado { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Atividade", Column = "ATI_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Atividade Atividade { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "NaturezaDaOperacao", Column = "NAT_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual NaturezaDaOperacao NaturezaDaOperacao { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "CFOP", Column = "CFO_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual CFOP CFOP { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Empresa", Column = "EMP_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Empresa Empresa { get; set; }

        public virtual string Descricao
        {
            get
            {
                return this.NCMProduto;
            }
        }

        public virtual bool Equals(ObservacaoFiscal other)
        {
            if (other.Codigo == this.Codigo)
                return true;
            else
                return false;
        }

        public virtual string DescricaoCSTICMS
        {
            get
            {
                switch (this.CSTICMS)
                {
                    case ObjetosDeValor.Embarcador.Enumeradores.CSTICMS.CSOSN101:
                        return "101 - Tributada pelo Simples Nacional com permissão de crédito";
                    case ObjetosDeValor.Embarcador.Enumeradores.CSTICMS.CSOSN102:
                        return "102 - Tributada pelo Simples Nacional sem permissão de crédito";
                    case ObjetosDeValor.Embarcador.Enumeradores.CSTICMS.CSOSN103:
                        return "103 - Isenção do ICMS no Simples Nacional para faixa de receita bruta";
                    case ObjetosDeValor.Embarcador.Enumeradores.CSTICMS.CSOSN201:
                        return "201 - Tributada pelo Simples Nacional com permissão de crédito e com cobrança do ICMS por substituição tributária";
                    case ObjetosDeValor.Embarcador.Enumeradores.CSTICMS.CSOSN202:
                        return "202 - Tributada pelo Simples Nacional sem permissão de crédito e com cobrança do ICMS por substituição tributária";
                    case ObjetosDeValor.Embarcador.Enumeradores.CSTICMS.CSOSN203:
                        return "203 - Isenção do ICMS no Simples Nacional para faixa de receita bruta e com cobrança do ICMS por substituicao tributaria";
                    case ObjetosDeValor.Embarcador.Enumeradores.CSTICMS.CSOSN300:
                        return "300 - Imune";
                    case ObjetosDeValor.Embarcador.Enumeradores.CSTICMS.CSOSN400:
                        return "400 - Nao tributada pelo Simples Nacional";
                    case ObjetosDeValor.Embarcador.Enumeradores.CSTICMS.CSOSN500:
                        return "500 - ICMS cobrado anteriormente por substituicao tributaria (substituido) ou por antecipacao";
                    case ObjetosDeValor.Embarcador.Enumeradores.CSTICMS.CSOSN900:
                        return "900 - Outros";
                    case ObjetosDeValor.Embarcador.Enumeradores.CSTICMS.CST00:
                        return "00 - Tributada integralmente";
                    case ObjetosDeValor.Embarcador.Enumeradores.CSTICMS.CST10:
                        return "10 - Tributada e com cobrança do ICMS por substituição tributária";
                    case ObjetosDeValor.Embarcador.Enumeradores.CSTICMS.CST20:
                        return "20 - Com redução de base de cálculo";
                    case ObjetosDeValor.Embarcador.Enumeradores.CSTICMS.CST30:
                        return "30 - Isenta ou não tributada e com cobrança do ICMS por substituição";
                    case ObjetosDeValor.Embarcador.Enumeradores.CSTICMS.CST40:
                        return "40 - Isenta";
                    case ObjetosDeValor.Embarcador.Enumeradores.CSTICMS.CST41:
                        return "41 - Não Tributada";
                    case ObjetosDeValor.Embarcador.Enumeradores.CSTICMS.CST50:
                        return "50 - Suspensão";
                    case ObjetosDeValor.Embarcador.Enumeradores.CSTICMS.CST51:
                        return "51 - Diferimento";
                    case ObjetosDeValor.Embarcador.Enumeradores.CSTICMS.CST60:
                        return "60 - ICMS cobrado anteriormente por substituição tributária";
                    case ObjetosDeValor.Embarcador.Enumeradores.CSTICMS.CST70:
                        return "70 - Com redução de base de cálculo e cobrança do ICMS por substituição tributária";
                    case ObjetosDeValor.Embarcador.Enumeradores.CSTICMS.CST90:
                        return "90 - Outras";
                    default:
                        return "";
                }
            }
        }
    }
}
