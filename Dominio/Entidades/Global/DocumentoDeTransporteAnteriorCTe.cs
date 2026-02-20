using System;

namespace Dominio.Entidades
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_CTE_SUBCONTRATADO", EntityName = "DocumentoDeTransporteAnteriorCTe", Name = "Dominio.Entidades.DocumentoDeTransporteAnteriorCTe", NameType = typeof(DocumentoDeTransporteAnteriorCTe))]
    public class DocumentoDeTransporteAnteriorCTe: EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "CSU_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Cliente", Column = "CSU_CLIENTE_EMISSOR", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Cliente Emissor { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ConhecimentoDeTransporteEletronico", Column = "CON_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual ConhecimentoDeTransporteEletronico CTe { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Tipo", Column = "CSU_TIPO", TypeType = typeof(string), Length = 2, NotNull = false)]
        public virtual string Tipo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Serie", Column = "CSU_SERIE", TypeType = typeof(string), Length = 3, NotNull = false)]
        public virtual string Serie { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Numero", Column = "CSU_NUM", TypeType = typeof(string), Length = 20, NotNull = false)]
        public virtual string Numero { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataEmissao", Column = "CSU_DATA", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataEmissao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Chave", Column = "CSU_CHAVE", TypeType = typeof(string), Length = 44, NotNull = false)]
        public virtual string Chave { get; set; }

        public virtual string NumeroOuNumeroDaChave
        {
            get
            {
                if (!string.IsNullOrWhiteSpace(this.Chave))
                {
                    int numero = 0;

                    if (int.TryParse(this.Chave.Substring(25, 9), out numero))
                        return numero.ToString();
                    else
                        return this.Chave.Substring(25, 9);
                }
                else if (!string.IsNullOrWhiteSpace(this.Numero))
                {
                    return this.Numero;
                }
                else
                {
                    return string.Empty;
                }
            }
        }

        public virtual string SerieOuSerieDaChave
        {
            get
            {
                if (!string.IsNullOrWhiteSpace(this.Chave))
                {
                    int serie = 0;

                    if (int.TryParse(this.Chave.Substring(22, 3), out serie))
                        return serie.ToString();
                    else
                        return this.Chave.Substring(22, 3);
                }
                else if (!string.IsNullOrWhiteSpace(this.Serie))
                {
                    return this.Serie;
                }
                else
                {
                    return string.Empty;
                }
            }
        }

        public virtual string DescricaoTipo
        {
            get
            {
                switch (Tipo)
                {
                    case "00":
                        return "CTRC";
                    case "01":
                        return "CTAC";
                    case "02":
                        return "ACT";
                    case "03":
                        return "NF Modelo 7";
                    case "04":
                        return "NF Modelo 27";
                    case "05":
                        return "Conhecimento Aéreo Nacional";
                    case "06":
                        return "CTMC";
                    case "07":
                        return "ATRE";
                    case "08":
                        return "DTA (Despacho de Trânsito Aduaneiro)";
                    case "09":
                        return "Conhecimento Aéreo Internacional";
                    case "10":
                        return "Conhecimento - Carta de Porte Internacional";
                    case "11":
                        return "Conhecimento Avulso";
                    case "12":
                        return "TIF (Transporte Internacional Ferroviário)";
                    case "13":
                        return "BL (Bill of Landing)";
                    case "99":
                        return "Outros";
                    default:
                        return "";
                }
            }
        }

        public virtual DocumentoDeTransporteAnteriorCTe Clonar()
        {
            return (DocumentoDeTransporteAnteriorCTe)this.MemberwiseClone();
        }
    }
}
