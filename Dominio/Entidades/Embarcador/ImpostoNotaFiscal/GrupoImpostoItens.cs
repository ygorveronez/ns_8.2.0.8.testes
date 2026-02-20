using System;

namespace Dominio.Entidades.Embarcador.ImpostoNotaFiscal
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_GRUPO_IMPOSTO_ITENS", EntityName = "GrupoImpostoItens", Name = "Dominio.Entidades.Embarcador.ImpostoNotaFiscal.GrupoImpostoItens", NameType = typeof(GrupoImpostoItens))]
    public class GrupoImpostoItens : EntidadeBase, IEquatable<Dominio.Entidades.Embarcador.ImpostoNotaFiscal.GrupoImpostoItens>
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "GII_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "UFOrigem", Column = "GII_UF_ORIGEM", TypeType = typeof(string), Length = 2, NotNull = false)]
        public virtual string UFOrigem { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "UFDestino", Column = "GII_UF_DESTINO", TypeType = typeof(string), Length = 2, NotNull = false)]
        public virtual string UFDestino { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "AliquotaICMSInternaVenda", Column = "GII_ALIQUOTA_ICMS_INTERNA_VENDA", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal AliquotaICMSInternaVenda { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "AliquotaICMSInterestadualVenda", Column = "GII_ALIQUOTA_ICMS_INTERESTADUAL_VENDA", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal AliquotaICMSInterestadualVenda { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "MVAVenda", Column = "GII_MVA_VENDA", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal MVAVenda { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CSTICMSVenda", Column = "GII_CST_CSOSN_VENDA", TypeType = typeof(Dominio.ObjetosDeValor.Embarcador.Enumeradores.CSTICMS), NotNull = false)]
        public virtual Dominio.ObjetosDeValor.Embarcador.Enumeradores.CSTICMS? CSTICMSVenda { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CFOPVenda", Column = "GII_CFOP_VENDA", TypeType = typeof(string), Length = 4, NotNull = false)]
        public virtual string CFOPVenda { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ReducaoMVAVenda", Column = "GII_REDUCAO_MVA_VENDA", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal ReducaoMVAVenda { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DiferencialAliquotaVenda", Column = "GII_DIFERENCIAL_ALIQUOTA_VENDA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool DiferencialAliquotaVenda { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ReducaoBCICMSVenda", Column = "GII_REDUCAO_BC_ICMS_VENDA", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal ReducaoBCICMSVenda { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CSTPISVenda", Column = "GII_CST_PIS_VENDA", TypeType = typeof(Dominio.ObjetosDeValor.Embarcador.Enumeradores.CSTPISCOFINS), NotNull = false)]
        public virtual Dominio.ObjetosDeValor.Embarcador.Enumeradores.CSTPISCOFINS? CSTPISVenda { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ReducaoBCPISVenda", Column = "GII_REDUCAO_BC_PIS_VENDA", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal ReducaoBCPISVenda { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "AliquotaPISVenda", Column = "GII_ALIQUOTA_PIS_VENDA", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal AliquotaPISVenda { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CSTCOFINSVenda", Column = "GII_CST_COFINS_VENDA", TypeType = typeof(Dominio.ObjetosDeValor.Embarcador.Enumeradores.CSTPISCOFINS), NotNull = false)]
        public virtual Dominio.ObjetosDeValor.Embarcador.Enumeradores.CSTPISCOFINS? CSTCOFINSVenda { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ReducaoBCCOFINSVenda", Column = "GII_REDUCAO_BC_COFINS_VENDA", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal ReducaoBCCOFINSVenda { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "AliquotaCOFINSVenda", Column = "GII_ALIQUOTA_COFINS_VENDA", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal AliquotaCOFINSVenda { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "AliquotaFCPVenda", Column = "GII_ALIQUOTA_FCP_VENDA", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal AliquotaFCPVenda { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "AliquotaDifalVenda", Column = "GII_ALIQUOTA_DIFAL_VENDA", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal AliquotaDifalVenda { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "AliquotaICMSCompra", Column = "GII_ALIQUOTA_ICMS_COMPRA", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal AliquotaICMSCompra { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "MVACompra", Column = "GII_MVA_COMPRA", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal MVACompra { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CSTICMSCompra", Column = "GII_CST_CSOSN_COMPRA", TypeType = typeof(Dominio.ObjetosDeValor.Embarcador.Enumeradores.CSTICMS), NotNull = false)]
        public virtual Dominio.ObjetosDeValor.Embarcador.Enumeradores.CSTICMS? CSTICMSCompra { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CFOPCompra", Column = "GII_CFOP_COMPRA", TypeType = typeof(string), Length = 4, NotNull = false)]
        public virtual string CFOPCompra { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ReducaoBCICMSCompra", Column = "GII_REDUCAO_BC_ICMS_COMPRA", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal ReducaoBCICMSCompra { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ReducaoMVACompra", Column = "GII_REDUCAO_MVA_COMPRA", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal ReducaoMVACompra { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CSTPISCompra", Column = "GII_CST_PIS_COMPRA", TypeType = typeof(Dominio.ObjetosDeValor.Embarcador.Enumeradores.CSTPISCOFINS), NotNull = false)]
        public virtual Dominio.ObjetosDeValor.Embarcador.Enumeradores.CSTPISCOFINS? CSTPISCompra { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ReducaoBCPISCompra", Column = "GII_REDUCAO_BC_PIS_COMPRA", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal ReducaoBCPISCompra { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "AliquotaPISCompra", Column = "GII_ALIQUOTA_PIS_COMPRA", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal AliquotaPISCompra { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CSTCOFINSCompra", Column = "GII_CST_COFINS_COMPRA", TypeType = typeof(Dominio.ObjetosDeValor.Embarcador.Enumeradores.CSTPISCOFINS), NotNull = false)]
        public virtual Dominio.ObjetosDeValor.Embarcador.Enumeradores.CSTPISCOFINS? CSTCOFINSCompra { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ReducaoBCCOFINSCompra", Column = "GII_REDUCAO_BC_COFINS_COMPRA", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal ReducaoBCCOFINSCompra { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "AliquotaCOFINSCompra", Column = "GII_ALIQUOTA_COFINS_COMPRA", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal AliquotaCOFINSCompra { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ObservacaoFiscal", Column = "OBF_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Embarcador.NotaFiscal.ObservacaoFiscal ObservacaoFiscal { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Atividade", Column = "ATI_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Atividade Atividade { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "GrupoImposto", Column = "GRI_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual GrupoImposto GrupoImposto { get; set; }

        public virtual bool Equals(GrupoImpostoItens other)
        {
            if (other.Codigo == this.Codigo)
                return true;
            else
                return false;
        }

        public virtual string DescricaoAliquotaICMSInternaVenda()
        {
            return "<span class='spnTipoValorItem' data-codigo-item='" + this.Codigo + "'></span> " + this.AliquotaICMSInternaVenda.ToString("n2");
        }
        public virtual string DescricaoAliquotaICMSInterestadualVenda()
        {
            return "<span class='spnTipoValorItem' data-codigo-item='" + this.Codigo + "'></span> " + this.AliquotaICMSInterestadualVenda.ToString("n2");
        }
        public virtual string DescricaoMVAVenda()
        {
            return "<span class='spnTipoValorItem' data-codigo-item='" + this.Codigo + "'></span> " + this.MVAVenda.ToString("n2");
        }
        public virtual string DescricaoCSTICMSVendaGrid()
        {
            return "<span class='spnTipoValorItem' data-codigo-item='" + this.Codigo + "'></span> " + this.NumeroCSTICMSVenda;
        }
        public virtual string DescricaoCFOPVenda()
        {
            return "<span class='spnTipoValorItem' data-codigo-item='" + this.Codigo + "'></span> " + this.CFOPVenda;
        }
        public virtual string DescricaoReducaoMVAVenda()
        {
            return "<span class='spnTipoValorItem' data-codigo-item='" + this.Codigo + "'></span> " + this.ReducaoMVAVenda.ToString("n2");
        }
        public virtual string DescricaoDiferencialAliquotaVendaGrid()
        {
            return "<span class='spnTipoValorItem' data-codigo-item='" + this.Codigo + "'></span> " + this.DescricaoReduzidoDiferencialAliquotaVenda;
        }
        public virtual string DescricaoReducaoBCICMSVenda()
        {
            return "<span class='spnTipoValorItem' data-codigo-item='" + this.Codigo + "'></span> " + this.ReducaoBCICMSVenda.ToString("n2");
        }
        public virtual string DescricaoCSTPISVendaGrid()
        {
            return "<span class='spnTipoValorItem' data-codigo-item='" + this.Codigo + "'></span> " + this.NumeroCSTPISVenda;
        }
        public virtual string DescricaoReducaoBCPISVenda()
        {
            return "<span class='spnTipoValorItem' data-codigo-item='" + this.Codigo + "'></span> " + this.ReducaoBCPISVenda.ToString("n2");
        }
        public virtual string DescricaoAliquotaPISVenda()
        {
            return "<span class='spnTipoValorItem' data-codigo-item='" + this.Codigo + "'></span> " + this.AliquotaPISVenda.ToString("n2");
        }
        public virtual string DescricaoCSTCOFINSVendaGrid()
        {
            return "<span class='spnTipoValorItem' data-codigo-item='" + this.Codigo + "'></span> " + this.NumeroCSTCOFINSVenda;
        }
        public virtual string DescricaoReducaoBCCOFINSVenda()
        {
            return "<span class='spnTipoValorItem' data-codigo-item='" + this.Codigo + "'></span> " + this.ReducaoBCCOFINSVenda.ToString("n2");
        }
        public virtual string DescricaoAliquotaCOFINSVenda()
        {
            return "<span class='spnTipoValorItem' data-codigo-item='" + this.Codigo + "'></span> " + this.AliquotaCOFINSVenda.ToString("n2");
        }
        public virtual string DescricaoAliquotaFCPVenda()
        {
            return "<span class='spnTipoValorItem' data-codigo-item='" + this.Codigo + "'></span> " + this.AliquotaFCPVenda.ToString("n2");
        }
        public virtual string DescricaoAliquotaDifalVenda()
        {
            return "<span class='spnTipoValorItem' data-codigo-item='" + this.Codigo + "'></span> " + this.AliquotaDifalVenda.ToString("n2");
        }
        public virtual string DescricaoAliquotaICMSCompra()
        {
            return "<span class='spnTipoValorItem' data-codigo-item='" + this.Codigo + "'></span> " + this.AliquotaICMSCompra.ToString("n2");
        }
        public virtual string DescricaoMVACompra()
        {
            return "<span class='spnTipoValorItem' data-codigo-item='" + this.Codigo + "'></span> " + this.MVACompra.ToString("n2");
        }
        public virtual string DescricaoCSTICMSCompraGrid()
        {
            return "<span class='spnTipoValorItem' data-codigo-item='" + this.Codigo + "'></span> " + this.NumeroCSTICMSCompra;
        }
        public virtual string DescricaoCFOPCompra()
        {
            return "<span class='spnTipoValorItem' data-codigo-item='" + this.Codigo + "'></span> " + this.CFOPCompra;
        }
        public virtual string DescricaoReducaoBCICMSCompra()
        {
            return "<span class='spnTipoValorItem' data-codigo-item='" + this.Codigo + "'></span> " + this.ReducaoBCICMSCompra.ToString("n2");
        }
        public virtual string DescricaoReducaoMVACompra()
        {
            return "<span class='spnTipoValorItem' data-codigo-item='" + this.Codigo + "'></span> " + this.ReducaoMVACompra.ToString("n2");
        }
        public virtual string DescricaoCSTPISCompraGrid()
        {
            return "<span class='spnTipoValorItem' data-codigo-item='" + this.Codigo + "'></span> " + this.NumeroCSTPISCompra;
        }
        public virtual string DescricaoReducaoBCPISCompra()
        {
            return "<span class='spnTipoValorItem' data-codigo-item='" + this.Codigo + "'></span> " + this.ReducaoBCPISCompra.ToString("n2");
        }
        public virtual string DescricaoAliquotaPISCompra()
        {
            return "<span class='spnTipoValorItem' data-codigo-item='" + this.Codigo + "'></span> " + this.AliquotaPISCompra.ToString("n2");
        }
        public virtual string DescricaoCSTCOFINSCompraGrid()
        {
            return "<span class='spnTipoValorItem' data-codigo-item='" + this.Codigo + "'></span> " + this.NumeroCSTCOFINSCompra;
        }
        public virtual string DescricaoReducaoBCCOFINSCompra()
        {
            return "<span class='spnTipoValorItem' data-codigo-item='" + this.Codigo + "'></span> " + this.ReducaoBCCOFINSCompra.ToString("n2");
        }
        public virtual string DescricaoAliquotaCOFINSCompra()
        {
            return "<span class='spnTipoValorItem' data-codigo-item='" + this.Codigo + "'></span> " + this.AliquotaCOFINSCompra.ToString("n2");
        }
        public virtual string DescricaoObservacaoFiscal()
        {
            if (this.ObservacaoFiscal == null)
                return "<span class='spnTipoValorItem' data-codigo-item='" + this.Codigo + "'></span> " + string.Empty;
            else
                return "<span class='spnTipoValorItem' data-codigo-item='" + this.Codigo + "'></span> " + this.ObservacaoFiscal.Codigo.ToString();
            //return "<span class='spnTipoValorItem' data-codigo-item='" + this.Codigo + "'></span> " + this.ObservacaoFiscal != null ? this.ObservacaoFiscal.Codigo.ToString() + " - " + this.ObservacaoFiscal.Observacao : string.Empty;
        }
        public virtual string DescricaoReduzidoDiferencialAliquotaVenda
        {
            get
            {
                if (this.DiferencialAliquotaVenda)
                    return "S";
                else
                    return "N";
            }
        }
        public virtual string DescricaoDiferencialAliquotaVenda
        {
            get
            {
                if (this.DiferencialAliquotaVenda)
                    return "Sim";
                else
                    return "Não";
            }
        }

        public virtual ObjetosDeValor.Embarcador.Enumeradores.CSTICMS? EnumeradorCSTICMS(string cst)
        {
            switch (cst)
            {
                case "101":
                    return ObjetosDeValor.Embarcador.Enumeradores.CSTICMS.CSOSN101;
                case "102":
                    return ObjetosDeValor.Embarcador.Enumeradores.CSTICMS.CSOSN102;
                case "103":
                    return ObjetosDeValor.Embarcador.Enumeradores.CSTICMS.CSOSN103;
                case "201":
                    return ObjetosDeValor.Embarcador.Enumeradores.CSTICMS.CSOSN201;
                case "202":
                    return ObjetosDeValor.Embarcador.Enumeradores.CSTICMS.CSOSN202;
                case "203":
                    return ObjetosDeValor.Embarcador.Enumeradores.CSTICMS.CSOSN203;
                case "300":
                    return ObjetosDeValor.Embarcador.Enumeradores.CSTICMS.CSOSN300;
                case "400":
                    return ObjetosDeValor.Embarcador.Enumeradores.CSTICMS.CSOSN400;
                case "500":
                    return ObjetosDeValor.Embarcador.Enumeradores.CSTICMS.CSOSN500;
                case "900":
                    return ObjetosDeValor.Embarcador.Enumeradores.CSTICMS.CSOSN900;
                case "00":
                    return ObjetosDeValor.Embarcador.Enumeradores.CSTICMS.CST00;
                case "10":
                    return ObjetosDeValor.Embarcador.Enumeradores.CSTICMS.CST10;
                case "20":
                    return ObjetosDeValor.Embarcador.Enumeradores.CSTICMS.CST20;
                case "30":
                    return ObjetosDeValor.Embarcador.Enumeradores.CSTICMS.CST30;
                case "40":
                    return ObjetosDeValor.Embarcador.Enumeradores.CSTICMS.CST40;
                case "41":
                    return ObjetosDeValor.Embarcador.Enumeradores.CSTICMS.CST41;
                case "50":
                    return ObjetosDeValor.Embarcador.Enumeradores.CSTICMS.CST50;
                case "51":
                    return ObjetosDeValor.Embarcador.Enumeradores.CSTICMS.CST51;
                case "60":
                    return ObjetosDeValor.Embarcador.Enumeradores.CSTICMS.CST60;
                case "70":
                    return ObjetosDeValor.Embarcador.Enumeradores.CSTICMS.CST70;
                case "90":
                    return ObjetosDeValor.Embarcador.Enumeradores.CSTICMS.CST90;
                case "":
                    return 0;
                case " ":
                    return 0;
                default:
                    return null;
            }
        }

        public virtual ObjetosDeValor.Embarcador.Enumeradores.CSTPISCOFINS? EnumeradorCSTPISCOFINS(string cst)
        {
            switch (cst)
            {
                case "01":
                    return ObjetosDeValor.Embarcador.Enumeradores.CSTPISCOFINS.CST01;
                case "02":
                    return ObjetosDeValor.Embarcador.Enumeradores.CSTPISCOFINS.CST02;
                case "03":
                    return ObjetosDeValor.Embarcador.Enumeradores.CSTPISCOFINS.CST03;
                case "04":
                    return ObjetosDeValor.Embarcador.Enumeradores.CSTPISCOFINS.CST04;
                case "05":
                    return ObjetosDeValor.Embarcador.Enumeradores.CSTPISCOFINS.CST05;
                case "06":
                    return ObjetosDeValor.Embarcador.Enumeradores.CSTPISCOFINS.CST06;
                case "07":
                    return ObjetosDeValor.Embarcador.Enumeradores.CSTPISCOFINS.CST07;
                case "08":
                    return ObjetosDeValor.Embarcador.Enumeradores.CSTPISCOFINS.CST08;
                case "09":
                    return ObjetosDeValor.Embarcador.Enumeradores.CSTPISCOFINS.CST09;
                case "49":
                    return ObjetosDeValor.Embarcador.Enumeradores.CSTPISCOFINS.CST49;
                case "50":
                    return ObjetosDeValor.Embarcador.Enumeradores.CSTPISCOFINS.CST50;
                case "51":
                    return ObjetosDeValor.Embarcador.Enumeradores.CSTPISCOFINS.CST51;
                case "52":
                    return ObjetosDeValor.Embarcador.Enumeradores.CSTPISCOFINS.CST52;
                case "53":
                    return ObjetosDeValor.Embarcador.Enumeradores.CSTPISCOFINS.CST53;
                case "54":
                    return ObjetosDeValor.Embarcador.Enumeradores.CSTPISCOFINS.CST54;
                case "55":
                    return ObjetosDeValor.Embarcador.Enumeradores.CSTPISCOFINS.CST55;
                case "56":
                    return ObjetosDeValor.Embarcador.Enumeradores.CSTPISCOFINS.CST56;
                case "60":
                    return ObjetosDeValor.Embarcador.Enumeradores.CSTPISCOFINS.CST60;
                case "61":
                    return ObjetosDeValor.Embarcador.Enumeradores.CSTPISCOFINS.CST61;
                case "62":
                    return ObjetosDeValor.Embarcador.Enumeradores.CSTPISCOFINS.CST62;
                case "63":
                    return ObjetosDeValor.Embarcador.Enumeradores.CSTPISCOFINS.CST63;
                case "64":
                    return ObjetosDeValor.Embarcador.Enumeradores.CSTPISCOFINS.CST64;
                case "65":
                    return ObjetosDeValor.Embarcador.Enumeradores.CSTPISCOFINS.CST65;
                case "66":
                    return ObjetosDeValor.Embarcador.Enumeradores.CSTPISCOFINS.CST66;
                case "67":
                    return ObjetosDeValor.Embarcador.Enumeradores.CSTPISCOFINS.CST67;
                case "70":
                    return ObjetosDeValor.Embarcador.Enumeradores.CSTPISCOFINS.CST70;
                case "71":
                    return ObjetosDeValor.Embarcador.Enumeradores.CSTPISCOFINS.CST71;
                case "72":
                    return ObjetosDeValor.Embarcador.Enumeradores.CSTPISCOFINS.CST72;
                case "73":
                    return ObjetosDeValor.Embarcador.Enumeradores.CSTPISCOFINS.CST73;
                case "74":
                    return ObjetosDeValor.Embarcador.Enumeradores.CSTPISCOFINS.CST74;
                case "75":
                    return ObjetosDeValor.Embarcador.Enumeradores.CSTPISCOFINS.CST75;
                case "98":
                    return ObjetosDeValor.Embarcador.Enumeradores.CSTPISCOFINS.CST98;
                case "99":
                    return ObjetosDeValor.Embarcador.Enumeradores.CSTPISCOFINS.CST99;
                case "":
                    return 0;
                case " ":
                    return 0;
                default:
                    return null;
            }

        }

        public virtual string NumeroCSTICMSVenda
        {
            get
            {
                switch (this.CSTICMSVenda)
                {
                    case ObjetosDeValor.Embarcador.Enumeradores.CSTICMS.CSOSN101:
                        return "101";
                    case ObjetosDeValor.Embarcador.Enumeradores.CSTICMS.CSOSN102:
                        return "102";
                    case ObjetosDeValor.Embarcador.Enumeradores.CSTICMS.CSOSN103:
                        return "103";
                    case ObjetosDeValor.Embarcador.Enumeradores.CSTICMS.CSOSN201:
                        return "201";
                    case ObjetosDeValor.Embarcador.Enumeradores.CSTICMS.CSOSN202:
                        return "202";
                    case ObjetosDeValor.Embarcador.Enumeradores.CSTICMS.CSOSN203:
                        return "203";
                    case ObjetosDeValor.Embarcador.Enumeradores.CSTICMS.CSOSN300:
                        return "300";
                    case ObjetosDeValor.Embarcador.Enumeradores.CSTICMS.CSOSN400:
                        return "400";
                    case ObjetosDeValor.Embarcador.Enumeradores.CSTICMS.CSOSN500:
                        return "500";
                    case ObjetosDeValor.Embarcador.Enumeradores.CSTICMS.CSOSN900:
                        return "900";
                    case ObjetosDeValor.Embarcador.Enumeradores.CSTICMS.CST00:
                        return "00";
                    case ObjetosDeValor.Embarcador.Enumeradores.CSTICMS.CST10:
                        return "10";
                    case ObjetosDeValor.Embarcador.Enumeradores.CSTICMS.CST20:
                        return "20";
                    case ObjetosDeValor.Embarcador.Enumeradores.CSTICMS.CST30:
                        return "30";
                    case ObjetosDeValor.Embarcador.Enumeradores.CSTICMS.CST40:
                        return "40";
                    case ObjetosDeValor.Embarcador.Enumeradores.CSTICMS.CST41:
                        return "41";
                    case ObjetosDeValor.Embarcador.Enumeradores.CSTICMS.CST50:
                        return "50";
                    case ObjetosDeValor.Embarcador.Enumeradores.CSTICMS.CST51:
                        return "51";
                    case ObjetosDeValor.Embarcador.Enumeradores.CSTICMS.CST60:
                        return "60";
                    case ObjetosDeValor.Embarcador.Enumeradores.CSTICMS.CST70:
                        return "70";
                    case ObjetosDeValor.Embarcador.Enumeradores.CSTICMS.CST90:
                        return "90";
                    default:
                        return "";
                }
            }
        }

        public virtual string NumeroCSTPISVenda
        {
            get
            {
                switch (this.CSTPISVenda)
                {
                    case ObjetosDeValor.Embarcador.Enumeradores.CSTPISCOFINS.CST01:
                        return "01";
                    case ObjetosDeValor.Embarcador.Enumeradores.CSTPISCOFINS.CST02:
                        return "02";
                    case ObjetosDeValor.Embarcador.Enumeradores.CSTPISCOFINS.CST03:
                        return "03";
                    case ObjetosDeValor.Embarcador.Enumeradores.CSTPISCOFINS.CST04:
                        return "04";
                    case ObjetosDeValor.Embarcador.Enumeradores.CSTPISCOFINS.CST05:
                        return "05";
                    case ObjetosDeValor.Embarcador.Enumeradores.CSTPISCOFINS.CST06:
                        return "06";
                    case ObjetosDeValor.Embarcador.Enumeradores.CSTPISCOFINS.CST07:
                        return "07";
                    case ObjetosDeValor.Embarcador.Enumeradores.CSTPISCOFINS.CST08:
                        return "08";
                    case ObjetosDeValor.Embarcador.Enumeradores.CSTPISCOFINS.CST09:
                        return "09";
                    case ObjetosDeValor.Embarcador.Enumeradores.CSTPISCOFINS.CST49:
                        return "49";
                    case ObjetosDeValor.Embarcador.Enumeradores.CSTPISCOFINS.CST50:
                        return "50";
                    case ObjetosDeValor.Embarcador.Enumeradores.CSTPISCOFINS.CST51:
                        return "51";
                    case ObjetosDeValor.Embarcador.Enumeradores.CSTPISCOFINS.CST52:
                        return "52";
                    case ObjetosDeValor.Embarcador.Enumeradores.CSTPISCOFINS.CST53:
                        return "53";
                    case ObjetosDeValor.Embarcador.Enumeradores.CSTPISCOFINS.CST54:
                        return "54";
                    case ObjetosDeValor.Embarcador.Enumeradores.CSTPISCOFINS.CST55:
                        return "55";
                    case ObjetosDeValor.Embarcador.Enumeradores.CSTPISCOFINS.CST56:
                        return "56";
                    case ObjetosDeValor.Embarcador.Enumeradores.CSTPISCOFINS.CST60:
                        return "60";
                    case ObjetosDeValor.Embarcador.Enumeradores.CSTPISCOFINS.CST61:
                        return "61";
                    case ObjetosDeValor.Embarcador.Enumeradores.CSTPISCOFINS.CST62:
                        return "62";
                    case ObjetosDeValor.Embarcador.Enumeradores.CSTPISCOFINS.CST63:
                        return "63";
                    case ObjetosDeValor.Embarcador.Enumeradores.CSTPISCOFINS.CST64:
                        return "64";
                    case ObjetosDeValor.Embarcador.Enumeradores.CSTPISCOFINS.CST65:
                        return "65";
                    case ObjetosDeValor.Embarcador.Enumeradores.CSTPISCOFINS.CST66:
                        return "66";
                    case ObjetosDeValor.Embarcador.Enumeradores.CSTPISCOFINS.CST67:
                        return "67";
                    case ObjetosDeValor.Embarcador.Enumeradores.CSTPISCOFINS.CST70:
                        return "70";
                    case ObjetosDeValor.Embarcador.Enumeradores.CSTPISCOFINS.CST71:
                        return "71";
                    case ObjetosDeValor.Embarcador.Enumeradores.CSTPISCOFINS.CST72:
                        return "72";
                    case ObjetosDeValor.Embarcador.Enumeradores.CSTPISCOFINS.CST73:
                        return "73";
                    case ObjetosDeValor.Embarcador.Enumeradores.CSTPISCOFINS.CST74:
                        return "74";
                    case ObjetosDeValor.Embarcador.Enumeradores.CSTPISCOFINS.CST75:
                        return "75";
                    case ObjetosDeValor.Embarcador.Enumeradores.CSTPISCOFINS.CST98:
                        return "98";
                    case ObjetosDeValor.Embarcador.Enumeradores.CSTPISCOFINS.CST99:
                        return "99";
                    default:
                        return "";
                }
            }
        }

        public virtual string NumeroCSTCOFINSVenda
        {
            get
            {
                switch (this.CSTCOFINSVenda)
                {
                    case ObjetosDeValor.Embarcador.Enumeradores.CSTPISCOFINS.CST01:
                        return "01";
                    case ObjetosDeValor.Embarcador.Enumeradores.CSTPISCOFINS.CST02:
                        return "02";
                    case ObjetosDeValor.Embarcador.Enumeradores.CSTPISCOFINS.CST03:
                        return "03";
                    case ObjetosDeValor.Embarcador.Enumeradores.CSTPISCOFINS.CST04:
                        return "04";
                    case ObjetosDeValor.Embarcador.Enumeradores.CSTPISCOFINS.CST05:
                        return "05";
                    case ObjetosDeValor.Embarcador.Enumeradores.CSTPISCOFINS.CST06:
                        return "06";
                    case ObjetosDeValor.Embarcador.Enumeradores.CSTPISCOFINS.CST07:
                        return "07";
                    case ObjetosDeValor.Embarcador.Enumeradores.CSTPISCOFINS.CST08:
                        return "08";
                    case ObjetosDeValor.Embarcador.Enumeradores.CSTPISCOFINS.CST09:
                        return "09";
                    case ObjetosDeValor.Embarcador.Enumeradores.CSTPISCOFINS.CST49:
                        return "49";
                    case ObjetosDeValor.Embarcador.Enumeradores.CSTPISCOFINS.CST50:
                        return "50";
                    case ObjetosDeValor.Embarcador.Enumeradores.CSTPISCOFINS.CST51:
                        return "51";
                    case ObjetosDeValor.Embarcador.Enumeradores.CSTPISCOFINS.CST52:
                        return "52";
                    case ObjetosDeValor.Embarcador.Enumeradores.CSTPISCOFINS.CST53:
                        return "53";
                    case ObjetosDeValor.Embarcador.Enumeradores.CSTPISCOFINS.CST54:
                        return "54";
                    case ObjetosDeValor.Embarcador.Enumeradores.CSTPISCOFINS.CST55:
                        return "55";
                    case ObjetosDeValor.Embarcador.Enumeradores.CSTPISCOFINS.CST56:
                        return "56";
                    case ObjetosDeValor.Embarcador.Enumeradores.CSTPISCOFINS.CST60:
                        return "60";
                    case ObjetosDeValor.Embarcador.Enumeradores.CSTPISCOFINS.CST61:
                        return "61";
                    case ObjetosDeValor.Embarcador.Enumeradores.CSTPISCOFINS.CST62:
                        return "62";
                    case ObjetosDeValor.Embarcador.Enumeradores.CSTPISCOFINS.CST63:
                        return "63";
                    case ObjetosDeValor.Embarcador.Enumeradores.CSTPISCOFINS.CST64:
                        return "64";
                    case ObjetosDeValor.Embarcador.Enumeradores.CSTPISCOFINS.CST65:
                        return "65";
                    case ObjetosDeValor.Embarcador.Enumeradores.CSTPISCOFINS.CST66:
                        return "66";
                    case ObjetosDeValor.Embarcador.Enumeradores.CSTPISCOFINS.CST67:
                        return "67";
                    case ObjetosDeValor.Embarcador.Enumeradores.CSTPISCOFINS.CST70:
                        return "70";
                    case ObjetosDeValor.Embarcador.Enumeradores.CSTPISCOFINS.CST71:
                        return "71";
                    case ObjetosDeValor.Embarcador.Enumeradores.CSTPISCOFINS.CST72:
                        return "72";
                    case ObjetosDeValor.Embarcador.Enumeradores.CSTPISCOFINS.CST73:
                        return "73";
                    case ObjetosDeValor.Embarcador.Enumeradores.CSTPISCOFINS.CST74:
                        return "74";
                    case ObjetosDeValor.Embarcador.Enumeradores.CSTPISCOFINS.CST75:
                        return "75";
                    case ObjetosDeValor.Embarcador.Enumeradores.CSTPISCOFINS.CST98:
                        return "98";
                    case ObjetosDeValor.Embarcador.Enumeradores.CSTPISCOFINS.CST99:
                        return "99";
                    default:
                        return "";
                }
            }
        }

        public virtual string NumeroCSTICMSCompra
        {
            get
            {
                switch (this.CSTICMSCompra)
                {
                    case ObjetosDeValor.Embarcador.Enumeradores.CSTICMS.CSOSN101:
                        return "101";
                    case ObjetosDeValor.Embarcador.Enumeradores.CSTICMS.CSOSN102:
                        return "102";
                    case ObjetosDeValor.Embarcador.Enumeradores.CSTICMS.CSOSN103:
                        return "103";
                    case ObjetosDeValor.Embarcador.Enumeradores.CSTICMS.CSOSN201:
                        return "201";
                    case ObjetosDeValor.Embarcador.Enumeradores.CSTICMS.CSOSN202:
                        return "202";
                    case ObjetosDeValor.Embarcador.Enumeradores.CSTICMS.CSOSN203:
                        return "203";
                    case ObjetosDeValor.Embarcador.Enumeradores.CSTICMS.CSOSN300:
                        return "300";
                    case ObjetosDeValor.Embarcador.Enumeradores.CSTICMS.CSOSN400:
                        return "400";
                    case ObjetosDeValor.Embarcador.Enumeradores.CSTICMS.CSOSN500:
                        return "500";
                    case ObjetosDeValor.Embarcador.Enumeradores.CSTICMS.CSOSN900:
                        return "900";
                    case ObjetosDeValor.Embarcador.Enumeradores.CSTICMS.CST00:
                        return "00";
                    case ObjetosDeValor.Embarcador.Enumeradores.CSTICMS.CST10:
                        return "10";
                    case ObjetosDeValor.Embarcador.Enumeradores.CSTICMS.CST20:
                        return "20";
                    case ObjetosDeValor.Embarcador.Enumeradores.CSTICMS.CST30:
                        return "30";
                    case ObjetosDeValor.Embarcador.Enumeradores.CSTICMS.CST40:
                        return "40";
                    case ObjetosDeValor.Embarcador.Enumeradores.CSTICMS.CST41:
                        return "41";
                    case ObjetosDeValor.Embarcador.Enumeradores.CSTICMS.CST50:
                        return "50";
                    case ObjetosDeValor.Embarcador.Enumeradores.CSTICMS.CST51:
                        return "51";
                    case ObjetosDeValor.Embarcador.Enumeradores.CSTICMS.CST60:
                        return "60";
                    case ObjetosDeValor.Embarcador.Enumeradores.CSTICMS.CST70:
                        return "70";
                    case ObjetosDeValor.Embarcador.Enumeradores.CSTICMS.CST90:
                        return "90";
                    default:
                        return "";
                }
            }
        }

        public virtual string NumeroCSTPISCompra
        {
            get
            {
                switch (this.CSTPISCompra)
                {
                    case ObjetosDeValor.Embarcador.Enumeradores.CSTPISCOFINS.CST01:
                        return "01";
                    case ObjetosDeValor.Embarcador.Enumeradores.CSTPISCOFINS.CST02:
                        return "02";
                    case ObjetosDeValor.Embarcador.Enumeradores.CSTPISCOFINS.CST03:
                        return "03";
                    case ObjetosDeValor.Embarcador.Enumeradores.CSTPISCOFINS.CST04:
                        return "04";
                    case ObjetosDeValor.Embarcador.Enumeradores.CSTPISCOFINS.CST05:
                        return "05";
                    case ObjetosDeValor.Embarcador.Enumeradores.CSTPISCOFINS.CST06:
                        return "06";
                    case ObjetosDeValor.Embarcador.Enumeradores.CSTPISCOFINS.CST07:
                        return "07";
                    case ObjetosDeValor.Embarcador.Enumeradores.CSTPISCOFINS.CST08:
                        return "08";
                    case ObjetosDeValor.Embarcador.Enumeradores.CSTPISCOFINS.CST09:
                        return "09";
                    case ObjetosDeValor.Embarcador.Enumeradores.CSTPISCOFINS.CST49:
                        return "49";
                    case ObjetosDeValor.Embarcador.Enumeradores.CSTPISCOFINS.CST50:
                        return "50";
                    case ObjetosDeValor.Embarcador.Enumeradores.CSTPISCOFINS.CST51:
                        return "51";
                    case ObjetosDeValor.Embarcador.Enumeradores.CSTPISCOFINS.CST52:
                        return "52";
                    case ObjetosDeValor.Embarcador.Enumeradores.CSTPISCOFINS.CST53:
                        return "53";
                    case ObjetosDeValor.Embarcador.Enumeradores.CSTPISCOFINS.CST54:
                        return "54";
                    case ObjetosDeValor.Embarcador.Enumeradores.CSTPISCOFINS.CST55:
                        return "55";
                    case ObjetosDeValor.Embarcador.Enumeradores.CSTPISCOFINS.CST56:
                        return "56";
                    case ObjetosDeValor.Embarcador.Enumeradores.CSTPISCOFINS.CST60:
                        return "60";
                    case ObjetosDeValor.Embarcador.Enumeradores.CSTPISCOFINS.CST61:
                        return "61";
                    case ObjetosDeValor.Embarcador.Enumeradores.CSTPISCOFINS.CST62:
                        return "62";
                    case ObjetosDeValor.Embarcador.Enumeradores.CSTPISCOFINS.CST63:
                        return "63";
                    case ObjetosDeValor.Embarcador.Enumeradores.CSTPISCOFINS.CST64:
                        return "64";
                    case ObjetosDeValor.Embarcador.Enumeradores.CSTPISCOFINS.CST65:
                        return "65";
                    case ObjetosDeValor.Embarcador.Enumeradores.CSTPISCOFINS.CST66:
                        return "66";
                    case ObjetosDeValor.Embarcador.Enumeradores.CSTPISCOFINS.CST67:
                        return "67";
                    case ObjetosDeValor.Embarcador.Enumeradores.CSTPISCOFINS.CST70:
                        return "70";
                    case ObjetosDeValor.Embarcador.Enumeradores.CSTPISCOFINS.CST71:
                        return "71";
                    case ObjetosDeValor.Embarcador.Enumeradores.CSTPISCOFINS.CST72:
                        return "72";
                    case ObjetosDeValor.Embarcador.Enumeradores.CSTPISCOFINS.CST73:
                        return "73";
                    case ObjetosDeValor.Embarcador.Enumeradores.CSTPISCOFINS.CST74:
                        return "74";
                    case ObjetosDeValor.Embarcador.Enumeradores.CSTPISCOFINS.CST75:
                        return "75";
                    case ObjetosDeValor.Embarcador.Enumeradores.CSTPISCOFINS.CST98:
                        return "98";
                    case ObjetosDeValor.Embarcador.Enumeradores.CSTPISCOFINS.CST99:
                        return "99";
                    default:
                        return "";
                }
            }
        }

        public virtual string NumeroCSTCOFINSCompra
        {
            get
            {
                switch (this.CSTCOFINSCompra)
                {
                    case ObjetosDeValor.Embarcador.Enumeradores.CSTPISCOFINS.CST01:
                        return "01";
                    case ObjetosDeValor.Embarcador.Enumeradores.CSTPISCOFINS.CST02:
                        return "02";
                    case ObjetosDeValor.Embarcador.Enumeradores.CSTPISCOFINS.CST03:
                        return "03";
                    case ObjetosDeValor.Embarcador.Enumeradores.CSTPISCOFINS.CST04:
                        return "04";
                    case ObjetosDeValor.Embarcador.Enumeradores.CSTPISCOFINS.CST05:
                        return "05";
                    case ObjetosDeValor.Embarcador.Enumeradores.CSTPISCOFINS.CST06:
                        return "06";
                    case ObjetosDeValor.Embarcador.Enumeradores.CSTPISCOFINS.CST07:
                        return "07";
                    case ObjetosDeValor.Embarcador.Enumeradores.CSTPISCOFINS.CST08:
                        return "08";
                    case ObjetosDeValor.Embarcador.Enumeradores.CSTPISCOFINS.CST09:
                        return "09";
                    case ObjetosDeValor.Embarcador.Enumeradores.CSTPISCOFINS.CST49:
                        return "49";
                    case ObjetosDeValor.Embarcador.Enumeradores.CSTPISCOFINS.CST50:
                        return "50";
                    case ObjetosDeValor.Embarcador.Enumeradores.CSTPISCOFINS.CST51:
                        return "51";
                    case ObjetosDeValor.Embarcador.Enumeradores.CSTPISCOFINS.CST52:
                        return "52";
                    case ObjetosDeValor.Embarcador.Enumeradores.CSTPISCOFINS.CST53:
                        return "53";
                    case ObjetosDeValor.Embarcador.Enumeradores.CSTPISCOFINS.CST54:
                        return "54";
                    case ObjetosDeValor.Embarcador.Enumeradores.CSTPISCOFINS.CST55:
                        return "55";
                    case ObjetosDeValor.Embarcador.Enumeradores.CSTPISCOFINS.CST56:
                        return "56";
                    case ObjetosDeValor.Embarcador.Enumeradores.CSTPISCOFINS.CST60:
                        return "60";
                    case ObjetosDeValor.Embarcador.Enumeradores.CSTPISCOFINS.CST61:
                        return "61";
                    case ObjetosDeValor.Embarcador.Enumeradores.CSTPISCOFINS.CST62:
                        return "62";
                    case ObjetosDeValor.Embarcador.Enumeradores.CSTPISCOFINS.CST63:
                        return "63";
                    case ObjetosDeValor.Embarcador.Enumeradores.CSTPISCOFINS.CST64:
                        return "64";
                    case ObjetosDeValor.Embarcador.Enumeradores.CSTPISCOFINS.CST65:
                        return "65";
                    case ObjetosDeValor.Embarcador.Enumeradores.CSTPISCOFINS.CST66:
                        return "66";
                    case ObjetosDeValor.Embarcador.Enumeradores.CSTPISCOFINS.CST67:
                        return "67";
                    case ObjetosDeValor.Embarcador.Enumeradores.CSTPISCOFINS.CST70:
                        return "70";
                    case ObjetosDeValor.Embarcador.Enumeradores.CSTPISCOFINS.CST71:
                        return "71";
                    case ObjetosDeValor.Embarcador.Enumeradores.CSTPISCOFINS.CST72:
                        return "72";
                    case ObjetosDeValor.Embarcador.Enumeradores.CSTPISCOFINS.CST73:
                        return "73";
                    case ObjetosDeValor.Embarcador.Enumeradores.CSTPISCOFINS.CST74:
                        return "74";
                    case ObjetosDeValor.Embarcador.Enumeradores.CSTPISCOFINS.CST75:
                        return "75";
                    case ObjetosDeValor.Embarcador.Enumeradores.CSTPISCOFINS.CST98:
                        return "98";
                    case ObjetosDeValor.Embarcador.Enumeradores.CSTPISCOFINS.CST99:
                        return "99";
                    default:
                        return "";
                }
            }
        }

        public virtual string DescricaoCSTICMSVenda
        {
            get
            {
                switch (this.CSTICMSVenda)
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

        public virtual string DescricaoCSTPISVenda
        {
            get
            {
                switch (this.CSTPISVenda)
                {
                    case ObjetosDeValor.Embarcador.Enumeradores.CSTPISCOFINS.CST01:
                        return "01 - Operação Tributável com Alíquota Básica";
                    case ObjetosDeValor.Embarcador.Enumeradores.CSTPISCOFINS.CST02:
                        return "02 - Operação Tributável com Alíquota Diferenciada";
                    case ObjetosDeValor.Embarcador.Enumeradores.CSTPISCOFINS.CST03:
                        return "03 - Operação Tributável com Alíquota por Unidade de Medida de Produto";
                    case ObjetosDeValor.Embarcador.Enumeradores.CSTPISCOFINS.CST04:
                        return "04 - Operação Tributável Monofásica - Revenda a Alíquota Zero";
                    case ObjetosDeValor.Embarcador.Enumeradores.CSTPISCOFINS.CST05:
                        return "05 - Operação Tributável por Substituição Tributária";
                    case ObjetosDeValor.Embarcador.Enumeradores.CSTPISCOFINS.CST06:
                        return "06 - Operação Tributável a Alíquota Zero";
                    case ObjetosDeValor.Embarcador.Enumeradores.CSTPISCOFINS.CST07:
                        return "07 - Operação Isenta da Contribuição";
                    case ObjetosDeValor.Embarcador.Enumeradores.CSTPISCOFINS.CST08:
                        return "08 - Operação sem Incidência da Contribuição";
                    case ObjetosDeValor.Embarcador.Enumeradores.CSTPISCOFINS.CST09:
                        return "09 - Operação com Suspensão da Contribuição";
                    case ObjetosDeValor.Embarcador.Enumeradores.CSTPISCOFINS.CST49:
                        return "49 - Outras Operações de Saída";
                    case ObjetosDeValor.Embarcador.Enumeradores.CSTPISCOFINS.CST50:
                        return "50 - Operação com Direito a Crédito - Vinculada Exclusivamente a Receita Tributada no Mercado Interno";
                    case ObjetosDeValor.Embarcador.Enumeradores.CSTPISCOFINS.CST51:
                        return "51 - Operação com Direito a Crédito – Vinculada Exclusivamente a Receita Não Tributada no Mercado Interno";
                    case ObjetosDeValor.Embarcador.Enumeradores.CSTPISCOFINS.CST52:
                        return "52 - Operação com Direito a Crédito - Vinculada Exclusivamente a Receita de Exportação";
                    case ObjetosDeValor.Embarcador.Enumeradores.CSTPISCOFINS.CST53:
                        return "53 - Operação com Direito a Crédito - Vinculada a Receitas Tributadas e Não - Tributadas no Mercado Interno";
                    case ObjetosDeValor.Embarcador.Enumeradores.CSTPISCOFINS.CST54:
                        return "54 - Operação com Direito a Crédito - Vinculada a Receitas Tributadas no Mercado Interno e de Exportação";
                    case ObjetosDeValor.Embarcador.Enumeradores.CSTPISCOFINS.CST55:
                        return "55 - Operação com Direito a Crédito - Vinculada a Receitas Não - Tributadas no Mercado Interno e de Exportação";
                    case ObjetosDeValor.Embarcador.Enumeradores.CSTPISCOFINS.CST56:
                        return "56 - Operação com Direito a Crédito - Vinculada a Receitas Tributadas e Não - Tributadas no Mercado Interno, e de Exportação";
                    case ObjetosDeValor.Embarcador.Enumeradores.CSTPISCOFINS.CST60:
                        return "60 - Crédito Presumido -Operação de Aquisição Vinculada Exclusivamente a Receita Tributada no Mercado Interno";
                    case ObjetosDeValor.Embarcador.Enumeradores.CSTPISCOFINS.CST61:
                        return "61 - Crédito Presumido -Operação de Aquisição Vinculada Exclusivamente a Receita Não - Tributada no Mercado Interno";
                    case ObjetosDeValor.Embarcador.Enumeradores.CSTPISCOFINS.CST62:
                        return "62 - Crédito Presumido -Operação de Aquisição Vinculada Exclusivamente a Receita de Exportação";
                    case ObjetosDeValor.Embarcador.Enumeradores.CSTPISCOFINS.CST63:
                        return "63 - Crédito Presumido -Operação de Aquisição Vinculada a Receitas Tributadas e Não-Tributadas no Mercado Interno";
                    case ObjetosDeValor.Embarcador.Enumeradores.CSTPISCOFINS.CST64:
                        return "64 - Crédito Presumido -Operação de Aquisição Vinculada a Receitas Tributadas no Mercado Interno e de Exportação";
                    case ObjetosDeValor.Embarcador.Enumeradores.CSTPISCOFINS.CST65:
                        return "65 - Crédito Presumido -Operação de Aquisição Vinculada a Receitas Não-Tributadas no Mercado Interno e de Exportação";
                    case ObjetosDeValor.Embarcador.Enumeradores.CSTPISCOFINS.CST66:
                        return "66 - Crédito Presumido -Operação de Aquisição Vinculada a Receitas Tributadas e Não-Tributadas no Mercado Interno, e de Exportação";
                    case ObjetosDeValor.Embarcador.Enumeradores.CSTPISCOFINS.CST67:
                        return "67 - Crédito Presumido -Outras Operações";
                    case ObjetosDeValor.Embarcador.Enumeradores.CSTPISCOFINS.CST70:
                        return "70 - Operação de Aquisição sem Direito a Crédito";
                    case ObjetosDeValor.Embarcador.Enumeradores.CSTPISCOFINS.CST71:
                        return "71 - Operação de Aquisição com Isenção";
                    case ObjetosDeValor.Embarcador.Enumeradores.CSTPISCOFINS.CST72:
                        return "72 - Operação de Aquisição com Suspensão";
                    case ObjetosDeValor.Embarcador.Enumeradores.CSTPISCOFINS.CST73:
                        return "73 - Operação de Aquisição a Alíquota Zero";
                    case ObjetosDeValor.Embarcador.Enumeradores.CSTPISCOFINS.CST74:
                        return "74 - Operação de Aquisição sem Incidência da Contribuição";
                    case ObjetosDeValor.Embarcador.Enumeradores.CSTPISCOFINS.CST75:
                        return "75 - Operação de Aquisição por Substituição Tributária";
                    case ObjetosDeValor.Embarcador.Enumeradores.CSTPISCOFINS.CST98:
                        return "98 - Outras Operações de Entrada";
                    case ObjetosDeValor.Embarcador.Enumeradores.CSTPISCOFINS.CST99:
                        return "99 - Outras Operações";
                    default:
                        return "";
                }
            }
        }

        public virtual string DescricaoCSTCOFINSVenda
        {
            get
            {
                switch (this.CSTCOFINSVenda)
                {
                    case ObjetosDeValor.Embarcador.Enumeradores.CSTPISCOFINS.CST01:
                        return "01 - Operação Tributável com Alíquota Básica";
                    case ObjetosDeValor.Embarcador.Enumeradores.CSTPISCOFINS.CST02:
                        return "02 - Operação Tributável com Alíquota Diferenciada";
                    case ObjetosDeValor.Embarcador.Enumeradores.CSTPISCOFINS.CST03:
                        return "03 - Operação Tributável com Alíquota por Unidade de Medida de Produto";
                    case ObjetosDeValor.Embarcador.Enumeradores.CSTPISCOFINS.CST04:
                        return "04 - Operação Tributável Monofásica - Revenda a Alíquota Zero";
                    case ObjetosDeValor.Embarcador.Enumeradores.CSTPISCOFINS.CST05:
                        return "05 - Operação Tributável por Substituição Tributária";
                    case ObjetosDeValor.Embarcador.Enumeradores.CSTPISCOFINS.CST06:
                        return "06 - Operação Tributável a Alíquota Zero";
                    case ObjetosDeValor.Embarcador.Enumeradores.CSTPISCOFINS.CST07:
                        return "07 - Operação Isenta da Contribuição";
                    case ObjetosDeValor.Embarcador.Enumeradores.CSTPISCOFINS.CST08:
                        return "08 - Operação sem Incidência da Contribuição";
                    case ObjetosDeValor.Embarcador.Enumeradores.CSTPISCOFINS.CST09:
                        return "09 - Operação com Suspensão da Contribuição";
                    case ObjetosDeValor.Embarcador.Enumeradores.CSTPISCOFINS.CST49:
                        return "49 - Outras Operações de Saída";
                    case ObjetosDeValor.Embarcador.Enumeradores.CSTPISCOFINS.CST50:
                        return "50 - Operação com Direito a Crédito - Vinculada Exclusivamente a Receita Tributada no Mercado Interno";
                    case ObjetosDeValor.Embarcador.Enumeradores.CSTPISCOFINS.CST51:
                        return "51 - Operação com Direito a Crédito – Vinculada Exclusivamente a Receita Não Tributada no Mercado Interno";
                    case ObjetosDeValor.Embarcador.Enumeradores.CSTPISCOFINS.CST52:
                        return "52 - Operação com Direito a Crédito - Vinculada Exclusivamente a Receita de Exportação";
                    case ObjetosDeValor.Embarcador.Enumeradores.CSTPISCOFINS.CST53:
                        return "53 - Operação com Direito a Crédito - Vinculada a Receitas Tributadas e Não - Tributadas no Mercado Interno";
                    case ObjetosDeValor.Embarcador.Enumeradores.CSTPISCOFINS.CST54:
                        return "54 - Operação com Direito a Crédito - Vinculada a Receitas Tributadas no Mercado Interno e de Exportação";
                    case ObjetosDeValor.Embarcador.Enumeradores.CSTPISCOFINS.CST55:
                        return "55 - Operação com Direito a Crédito - Vinculada a Receitas Não - Tributadas no Mercado Interno e de Exportação";
                    case ObjetosDeValor.Embarcador.Enumeradores.CSTPISCOFINS.CST56:
                        return "56 - Operação com Direito a Crédito - Vinculada a Receitas Tributadas e Não - Tributadas no Mercado Interno, e de Exportação";
                    case ObjetosDeValor.Embarcador.Enumeradores.CSTPISCOFINS.CST60:
                        return "60 - Crédito Presumido -Operação de Aquisição Vinculada Exclusivamente a Receita Tributada no Mercado Interno";
                    case ObjetosDeValor.Embarcador.Enumeradores.CSTPISCOFINS.CST61:
                        return "61 - Crédito Presumido -Operação de Aquisição Vinculada Exclusivamente a Receita Não - Tributada no Mercado Interno";
                    case ObjetosDeValor.Embarcador.Enumeradores.CSTPISCOFINS.CST62:
                        return "62 - Crédito Presumido -Operação de Aquisição Vinculada Exclusivamente a Receita de Exportação";
                    case ObjetosDeValor.Embarcador.Enumeradores.CSTPISCOFINS.CST63:
                        return "63 - Crédito Presumido -Operação de Aquisição Vinculada a Receitas Tributadas e Não-Tributadas no Mercado Interno";
                    case ObjetosDeValor.Embarcador.Enumeradores.CSTPISCOFINS.CST64:
                        return "64 - Crédito Presumido -Operação de Aquisição Vinculada a Receitas Tributadas no Mercado Interno e de Exportação";
                    case ObjetosDeValor.Embarcador.Enumeradores.CSTPISCOFINS.CST65:
                        return "65 - Crédito Presumido -Operação de Aquisição Vinculada a Receitas Não-Tributadas no Mercado Interno e de Exportação";
                    case ObjetosDeValor.Embarcador.Enumeradores.CSTPISCOFINS.CST66:
                        return "66 - Crédito Presumido -Operação de Aquisição Vinculada a Receitas Tributadas e Não-Tributadas no Mercado Interno, e de Exportação";
                    case ObjetosDeValor.Embarcador.Enumeradores.CSTPISCOFINS.CST67:
                        return "67 - Crédito Presumido -Outras Operações";
                    case ObjetosDeValor.Embarcador.Enumeradores.CSTPISCOFINS.CST70:
                        return "70 - Operação de Aquisição sem Direito a Crédito";
                    case ObjetosDeValor.Embarcador.Enumeradores.CSTPISCOFINS.CST71:
                        return "71 - Operação de Aquisição com Isenção";
                    case ObjetosDeValor.Embarcador.Enumeradores.CSTPISCOFINS.CST72:
                        return "72 - Operação de Aquisição com Suspensão";
                    case ObjetosDeValor.Embarcador.Enumeradores.CSTPISCOFINS.CST73:
                        return "73 - Operação de Aquisição a Alíquota Zero";
                    case ObjetosDeValor.Embarcador.Enumeradores.CSTPISCOFINS.CST74:
                        return "74 - Operação de Aquisição sem Incidência da Contribuição";
                    case ObjetosDeValor.Embarcador.Enumeradores.CSTPISCOFINS.CST75:
                        return "75 - Operação de Aquisição por Substituição Tributária";
                    case ObjetosDeValor.Embarcador.Enumeradores.CSTPISCOFINS.CST98:
                        return "98 - Outras Operações de Entrada";
                    case ObjetosDeValor.Embarcador.Enumeradores.CSTPISCOFINS.CST99:
                        return "99 - Outras Operações";
                    default:
                        return "";
                }
            }
        }

        public virtual string DescricaoCSTICMSCompra
        {
            get
            {
                switch (this.CSTICMSCompra)
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

        public virtual string DescricaoCSTPISCompra
        {
            get
            {
                switch (this.CSTPISCompra)
                {
                    case ObjetosDeValor.Embarcador.Enumeradores.CSTPISCOFINS.CST01:
                        return "01 - Operação Tributável com Alíquota Básica";
                    case ObjetosDeValor.Embarcador.Enumeradores.CSTPISCOFINS.CST02:
                        return "02 - Operação Tributável com Alíquota Diferenciada";
                    case ObjetosDeValor.Embarcador.Enumeradores.CSTPISCOFINS.CST03:
                        return "03 - Operação Tributável com Alíquota por Unidade de Medida de Produto";
                    case ObjetosDeValor.Embarcador.Enumeradores.CSTPISCOFINS.CST04:
                        return "04 - Operação Tributável Monofásica - Revenda a Alíquota Zero";
                    case ObjetosDeValor.Embarcador.Enumeradores.CSTPISCOFINS.CST05:
                        return "05 - Operação Tributável por Substituição Tributária";
                    case ObjetosDeValor.Embarcador.Enumeradores.CSTPISCOFINS.CST06:
                        return "06 - Operação Tributável a Alíquota Zero";
                    case ObjetosDeValor.Embarcador.Enumeradores.CSTPISCOFINS.CST07:
                        return "07 - Operação Isenta da Contribuição";
                    case ObjetosDeValor.Embarcador.Enumeradores.CSTPISCOFINS.CST08:
                        return "08 - Operação sem Incidência da Contribuição";
                    case ObjetosDeValor.Embarcador.Enumeradores.CSTPISCOFINS.CST09:
                        return "09 - Operação com Suspensão da Contribuição";
                    case ObjetosDeValor.Embarcador.Enumeradores.CSTPISCOFINS.CST49:
                        return "49 - Outras Operações de Saída";
                    case ObjetosDeValor.Embarcador.Enumeradores.CSTPISCOFINS.CST50:
                        return "50 - Operação com Direito a Crédito - Vinculada Exclusivamente a Receita Tributada no Mercado Interno";
                    case ObjetosDeValor.Embarcador.Enumeradores.CSTPISCOFINS.CST51:
                        return "51 - Operação com Direito a Crédito – Vinculada Exclusivamente a Receita Não Tributada no Mercado Interno";
                    case ObjetosDeValor.Embarcador.Enumeradores.CSTPISCOFINS.CST52:
                        return "52 - Operação com Direito a Crédito - Vinculada Exclusivamente a Receita de Exportação";
                    case ObjetosDeValor.Embarcador.Enumeradores.CSTPISCOFINS.CST53:
                        return "53 - Operação com Direito a Crédito - Vinculada a Receitas Tributadas e Não - Tributadas no Mercado Interno";
                    case ObjetosDeValor.Embarcador.Enumeradores.CSTPISCOFINS.CST54:
                        return "54 - Operação com Direito a Crédito - Vinculada a Receitas Tributadas no Mercado Interno e de Exportação";
                    case ObjetosDeValor.Embarcador.Enumeradores.CSTPISCOFINS.CST55:
                        return "55 - Operação com Direito a Crédito - Vinculada a Receitas Não - Tributadas no Mercado Interno e de Exportação";
                    case ObjetosDeValor.Embarcador.Enumeradores.CSTPISCOFINS.CST56:
                        return "56 - Operação com Direito a Crédito - Vinculada a Receitas Tributadas e Não - Tributadas no Mercado Interno, e de Exportação";
                    case ObjetosDeValor.Embarcador.Enumeradores.CSTPISCOFINS.CST60:
                        return "60 - Crédito Presumido -Operação de Aquisição Vinculada Exclusivamente a Receita Tributada no Mercado Interno";
                    case ObjetosDeValor.Embarcador.Enumeradores.CSTPISCOFINS.CST61:
                        return "61 - Crédito Presumido -Operação de Aquisição Vinculada Exclusivamente a Receita Não - Tributada no Mercado Interno";
                    case ObjetosDeValor.Embarcador.Enumeradores.CSTPISCOFINS.CST62:
                        return "62 - Crédito Presumido -Operação de Aquisição Vinculada Exclusivamente a Receita de Exportação";
                    case ObjetosDeValor.Embarcador.Enumeradores.CSTPISCOFINS.CST63:
                        return "63 - Crédito Presumido -Operação de Aquisição Vinculada a Receitas Tributadas e Não-Tributadas no Mercado Interno";
                    case ObjetosDeValor.Embarcador.Enumeradores.CSTPISCOFINS.CST64:
                        return "64 - Crédito Presumido -Operação de Aquisição Vinculada a Receitas Tributadas no Mercado Interno e de Exportação";
                    case ObjetosDeValor.Embarcador.Enumeradores.CSTPISCOFINS.CST65:
                        return "65 - Crédito Presumido -Operação de Aquisição Vinculada a Receitas Não-Tributadas no Mercado Interno e de Exportação";
                    case ObjetosDeValor.Embarcador.Enumeradores.CSTPISCOFINS.CST66:
                        return "66 - Crédito Presumido -Operação de Aquisição Vinculada a Receitas Tributadas e Não-Tributadas no Mercado Interno, e de Exportação";
                    case ObjetosDeValor.Embarcador.Enumeradores.CSTPISCOFINS.CST67:
                        return "67 - Crédito Presumido -Outras Operações";
                    case ObjetosDeValor.Embarcador.Enumeradores.CSTPISCOFINS.CST70:
                        return "70 - Operação de Aquisição sem Direito a Crédito";
                    case ObjetosDeValor.Embarcador.Enumeradores.CSTPISCOFINS.CST71:
                        return "71 - Operação de Aquisição com Isenção";
                    case ObjetosDeValor.Embarcador.Enumeradores.CSTPISCOFINS.CST72:
                        return "72 - Operação de Aquisição com Suspensão";
                    case ObjetosDeValor.Embarcador.Enumeradores.CSTPISCOFINS.CST73:
                        return "73 - Operação de Aquisição a Alíquota Zero";
                    case ObjetosDeValor.Embarcador.Enumeradores.CSTPISCOFINS.CST74:
                        return "74 - Operação de Aquisição sem Incidência da Contribuição";
                    case ObjetosDeValor.Embarcador.Enumeradores.CSTPISCOFINS.CST75:
                        return "75 - Operação de Aquisição por Substituição Tributária";
                    case ObjetosDeValor.Embarcador.Enumeradores.CSTPISCOFINS.CST98:
                        return "98 - Outras Operações de Entrada";
                    case ObjetosDeValor.Embarcador.Enumeradores.CSTPISCOFINS.CST99:
                        return "99 - Outras Operações";
                    default:
                        return "";
                }
            }
        }

        public virtual string DescricaoCSTCOFINSCompra
        {
            get
            {
                switch (this.CSTCOFINSCompra)
                {
                    case ObjetosDeValor.Embarcador.Enumeradores.CSTPISCOFINS.CST01:
                        return "01 - Operação Tributável com Alíquota Básica";
                    case ObjetosDeValor.Embarcador.Enumeradores.CSTPISCOFINS.CST02:
                        return "02 - Operação Tributável com Alíquota Diferenciada";
                    case ObjetosDeValor.Embarcador.Enumeradores.CSTPISCOFINS.CST03:
                        return "03 - Operação Tributável com Alíquota por Unidade de Medida de Produto";
                    case ObjetosDeValor.Embarcador.Enumeradores.CSTPISCOFINS.CST04:
                        return "04 - Operação Tributável Monofásica - Revenda a Alíquota Zero";
                    case ObjetosDeValor.Embarcador.Enumeradores.CSTPISCOFINS.CST05:
                        return "05 - Operação Tributável por Substituição Tributária";
                    case ObjetosDeValor.Embarcador.Enumeradores.CSTPISCOFINS.CST06:
                        return "06 - Operação Tributável a Alíquota Zero";
                    case ObjetosDeValor.Embarcador.Enumeradores.CSTPISCOFINS.CST07:
                        return "07 - Operação Isenta da Contribuição";
                    case ObjetosDeValor.Embarcador.Enumeradores.CSTPISCOFINS.CST08:
                        return "08 - Operação sem Incidência da Contribuição";
                    case ObjetosDeValor.Embarcador.Enumeradores.CSTPISCOFINS.CST09:
                        return "09 - Operação com Suspensão da Contribuição";
                    case ObjetosDeValor.Embarcador.Enumeradores.CSTPISCOFINS.CST49:
                        return "49 - Outras Operações de Saída";
                    case ObjetosDeValor.Embarcador.Enumeradores.CSTPISCOFINS.CST50:
                        return "50 - Operação com Direito a Crédito - Vinculada Exclusivamente a Receita Tributada no Mercado Interno";
                    case ObjetosDeValor.Embarcador.Enumeradores.CSTPISCOFINS.CST51:
                        return "51 - Operação com Direito a Crédito – Vinculada Exclusivamente a Receita Não Tributada no Mercado Interno";
                    case ObjetosDeValor.Embarcador.Enumeradores.CSTPISCOFINS.CST52:
                        return "52 - Operação com Direito a Crédito - Vinculada Exclusivamente a Receita de Exportação";
                    case ObjetosDeValor.Embarcador.Enumeradores.CSTPISCOFINS.CST53:
                        return "53 - Operação com Direito a Crédito - Vinculada a Receitas Tributadas e Não - Tributadas no Mercado Interno";
                    case ObjetosDeValor.Embarcador.Enumeradores.CSTPISCOFINS.CST54:
                        return "54 - Operação com Direito a Crédito - Vinculada a Receitas Tributadas no Mercado Interno e de Exportação";
                    case ObjetosDeValor.Embarcador.Enumeradores.CSTPISCOFINS.CST55:
                        return "55 - Operação com Direito a Crédito - Vinculada a Receitas Não - Tributadas no Mercado Interno e de Exportação";
                    case ObjetosDeValor.Embarcador.Enumeradores.CSTPISCOFINS.CST56:
                        return "56 - Operação com Direito a Crédito - Vinculada a Receitas Tributadas e Não - Tributadas no Mercado Interno, e de Exportação";
                    case ObjetosDeValor.Embarcador.Enumeradores.CSTPISCOFINS.CST60:
                        return "60 - Crédito Presumido -Operação de Aquisição Vinculada Exclusivamente a Receita Tributada no Mercado Interno";
                    case ObjetosDeValor.Embarcador.Enumeradores.CSTPISCOFINS.CST61:
                        return "61 - Crédito Presumido -Operação de Aquisição Vinculada Exclusivamente a Receita Não - Tributada no Mercado Interno";
                    case ObjetosDeValor.Embarcador.Enumeradores.CSTPISCOFINS.CST62:
                        return "62 - Crédito Presumido -Operação de Aquisição Vinculada Exclusivamente a Receita de Exportação";
                    case ObjetosDeValor.Embarcador.Enumeradores.CSTPISCOFINS.CST63:
                        return "63 - Crédito Presumido -Operação de Aquisição Vinculada a Receitas Tributadas e Não-Tributadas no Mercado Interno";
                    case ObjetosDeValor.Embarcador.Enumeradores.CSTPISCOFINS.CST64:
                        return "64 - Crédito Presumido -Operação de Aquisição Vinculada a Receitas Tributadas no Mercado Interno e de Exportação";
                    case ObjetosDeValor.Embarcador.Enumeradores.CSTPISCOFINS.CST65:
                        return "65 - Crédito Presumido -Operação de Aquisição Vinculada a Receitas Não-Tributadas no Mercado Interno e de Exportação";
                    case ObjetosDeValor.Embarcador.Enumeradores.CSTPISCOFINS.CST66:
                        return "66 - Crédito Presumido -Operação de Aquisição Vinculada a Receitas Tributadas e Não-Tributadas no Mercado Interno, e de Exportação";
                    case ObjetosDeValor.Embarcador.Enumeradores.CSTPISCOFINS.CST67:
                        return "67 - Crédito Presumido -Outras Operações";
                    case ObjetosDeValor.Embarcador.Enumeradores.CSTPISCOFINS.CST70:
                        return "70 - Operação de Aquisição sem Direito a Crédito";
                    case ObjetosDeValor.Embarcador.Enumeradores.CSTPISCOFINS.CST71:
                        return "71 - Operação de Aquisição com Isenção";
                    case ObjetosDeValor.Embarcador.Enumeradores.CSTPISCOFINS.CST72:
                        return "72 - Operação de Aquisição com Suspensão";
                    case ObjetosDeValor.Embarcador.Enumeradores.CSTPISCOFINS.CST73:
                        return "73 - Operação de Aquisição a Alíquota Zero";
                    case ObjetosDeValor.Embarcador.Enumeradores.CSTPISCOFINS.CST74:
                        return "74 - Operação de Aquisição sem Incidência da Contribuição";
                    case ObjetosDeValor.Embarcador.Enumeradores.CSTPISCOFINS.CST75:
                        return "75 - Operação de Aquisição por Substituição Tributária";
                    case ObjetosDeValor.Embarcador.Enumeradores.CSTPISCOFINS.CST98:
                        return "98 - Outras Operações de Entrada";
                    case ObjetosDeValor.Embarcador.Enumeradores.CSTPISCOFINS.CST99:
                        return "99 - Outras Operações";
                    default:
                        return "";
                }
            }
        }

        public virtual Dominio.Entidades.Embarcador.ImpostoNotaFiscal.GrupoImpostoItens Clonar()
        {
            return (Dominio.Entidades.Embarcador.ImpostoNotaFiscal.GrupoImpostoItens)this.MemberwiseClone();
        }
    }
}
