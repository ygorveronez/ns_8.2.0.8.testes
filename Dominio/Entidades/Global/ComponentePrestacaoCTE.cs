namespace Dominio.Entidades
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_CTE_COMP_PREST", EntityName = "ComponentePrestacaoCTE", Name = "Dominio.Entidades.ComponentePrestacaoCTE", NameType = typeof(ComponentePrestacaoCTE))]
    public class ComponentePrestacaoCTE : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "CPT_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ConhecimentoDeTransporteEletronico", Column = "CON_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual ConhecimentoDeTransporteEletronico CTE { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ComponenteFrete", Column = "CFR_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Embarcador.Frete.ComponenteFrete ComponenteFrete { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NomeCTe", Column = "CPT_NOME", TypeType = typeof(string), Length = 15, NotNull = false)]
        public virtual string NomeCTe { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NovoCampoIdentificador", Column = "CPT_NOVO_CAMPO_IDENTIFICADOR", TypeType = typeof(string), Length = 50, NotNull = false)]
        public virtual string NovoCampoIdentificador { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Valor", Column = "CPT_VALOR", TypeType = typeof(decimal), Scale = 2, Precision = 13, NotNull = false)]
        public virtual decimal Valor { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "IncluiNaBaseDeCalculoDoICMS", Column = "CPT_INCLUI_BC_ICMS", TypeType = typeof(bool), NotNull = false)]
        public virtual bool IncluiNaBaseDeCalculoDoICMS { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "IncluiNoTotalAReceber", Column = "CPT_INCLUI_TOTAL_RECEBER", TypeType = typeof(bool), NotNull = false)]
        public virtual bool IncluiNoTotalAReceber { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "SourceSystemTransactionIdentifier", Column = "CPT_SOURCE_SYSTEM_TRANSACTION_ID", TypeType = typeof(string), NotNull = false)]
        public virtual string SourceSystemTransactionIdentifier { get; set; }

        public virtual decimal ValorFrete
        {
            get
            {
                return this.Nome.ToLower().Equals("valor frete") || this.Nome.ToLower().Equals("frete valor") ? this.Valor : 0m;
            }
        }

        public virtual decimal ValorDespacho
        {
            get
            {
                return this.Nome.ToLower().Contains("despacho") ? this.Valor : 0m;
            }
        }

        public virtual decimal ValorPedagio
        {
            get
            {
                string descricao = Nome.ToLower();

                return descricao.Contains("pedagio") || descricao.Contains("pedágio") ? this.Valor : 0m;
            }
        }

        public virtual decimal ValorSeguro
        {
            get
            {
                string descricao = Nome.ToLower();

                return descricao.Contains("seguro") || descricao.Contains("gris") ? this.Valor : 0m;
            }
        }

        public virtual decimal ValorAdicionalEntrega
        {
            get
            {
                return this.Nome.ToLower().Contains("adc entrega") ? this.Valor : 0m;
            }
        }

        public virtual decimal ValorDescarga
        {
            get
            {
                return this.Nome.ToLower().Contains("descarga") ? this.Valor : 0m;
            }
        }

        public virtual decimal ValorSECCAT
        {
            get
            {
                string descricao = Nome.ToLower();

                return descricao.Contains("sec-cat") || descricao.Contains("sec - cat") ? this.Valor : 0m;
            }
        }

        public virtual decimal ValorITR
        {
            get
            {
                string descricao = Nome.ToLower();

                return descricao.Contains("itr")  ? this.Valor : 0m;
            }
        }

        public virtual decimal ValorAdicionais
        {
            get
            {
                string descricao = Nome.ToLower();

                return descricao.Contains("adicionais") ? this.Valor : 0m;
            }
        }
        
        public virtual decimal ValorIRRF
        {
            get
            {
                string descricao = Nome.ToLower();

                return descricao.Contains("IRRF") ? this.Valor : 0m;
            }
        }        

        public virtual decimal ValorAdValorem
        {
            get
            {
                string descricao = Nome.ToLower();

                return descricao.Contains("ad valorem") || descricao.Contains("advalorem") ? this.Valor : 0m;
            }
        }

        public virtual decimal ValorGris
        {
            get
            {
                string descricao = Nome.ToLower();

                return descricao.Contains("gris") ? this.Valor : 0m;
            }
        }

        public virtual decimal ValorPesoVolume
        {
            get
            {
                string descricao = Nome.ToLower();

                return descricao.Contains("peso") || descricao.Contains("volume") ? this.Valor : 0m;
            }
        }

        public virtual decimal ValorOutrasDespesas
        {
            get
            {
                if (ValorFrete <= 0m && ValorDespacho <= 0 && ValorPedagio <= 0m && ValorSeguro <= 0m && ValorAdicionalEntrega <= 0m && ValorDescarga <= 0 && ValorAdValorem <= 0m)
                    return Valor;
                else
                    return 0m;
            }
        }

        public virtual decimal ValorOutrasDespesasSemDescargaEImpostos
        {
            get
            {
                if (ValorFrete <= 0m && ValorDespacho <= 0 && ValorPedagio <= 0m && ValorSeguro <= 0m && ValorAdicionalEntrega <= 0m && ValorAdValorem <= 0m && ValorImpostos <= 0m)
                    return Valor;
                else
                    return 0m;
            }
        }

        public virtual decimal ValorImpostos
        {
            get
            {
                return this.Nome.ToLower().Contains("impostos") ? this.Valor : 0m;
            }
        }

        public virtual string Nome
        {
            get
            {
                return NomeCTe != null ? NomeCTe.ToUpper() : string.Empty;
            }
            set
            {
                NomeCTe = value != null ? value.ToUpper() : value;
            }
        }

        public virtual ComponentePrestacaoCTE Clonar()
        {
            return (ComponentePrestacaoCTE)this.MemberwiseClone();
        }
    }
}
