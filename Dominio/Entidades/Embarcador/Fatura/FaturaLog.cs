using System;

namespace Dominio.Entidades.Embarcador.Fatura
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_FATURA_LOG", EntityName = "FaturaLog", Name = "Dominio.Entidades.Embarcador.Fatura.FaturaLog", NameType = typeof(FaturaLog))]
    public class FaturaLog : EntidadeBase, IEquatable<Dominio.Entidades.Embarcador.Fatura.FaturaLog>
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "FTL_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Fatura", Column = "FAT_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Fatura.Fatura Fatura { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Usuario", Column = "FUN_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Usuario Usuario { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TipoLogFatura", Column = "FLT_TIPO_LOG", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoFatura), NotNull = true)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.TipoLogFatura TipoLogFatura { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataHora", Column = "FLT_DATA_HORA", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime DataHora { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "EmailDestinoEDI", Column = "FLT_EMAIL_DESTINO_EDI", TypeType = typeof(string), Length = 3000, NotNull = false)]
        public virtual string EmailDestinoEDI { get; set; }

        public virtual string DescricaoTipoLogFatura
        {
            get
            {
                switch (this.TipoLogFatura)
                {
                    case ObjetosDeValor.Embarcador.Enumeradores.TipoLogFatura.CancelouFatura:
                        return "Cancelou Fatura";
                    case ObjetosDeValor.Embarcador.Enumeradores.TipoLogFatura.EnviouEDI:
                        return "Enviou EDI";
                    case ObjetosDeValor.Embarcador.Enumeradores.TipoLogFatura.EnviouFatura:
                        return "Enviou Fatura";
                    case ObjetosDeValor.Embarcador.Enumeradores.TipoLogFatura.FechouFatura:
                        return "Fechou Fatura";
                    case ObjetosDeValor.Embarcador.Enumeradores.TipoLogFatura.GerouParcela:
                        return "Gerou as parcelas";
                    case ObjetosDeValor.Embarcador.Enumeradores.TipoLogFatura.IniciouFatura:
                        return "Iniciou Fatura";
                    case ObjetosDeValor.Embarcador.Enumeradores.TipoLogFatura.ReabriuFatura:
                        return "Reabriu Fatura";
                    case ObjetosDeValor.Embarcador.Enumeradores.TipoLogFatura.SalvouCargas:
                        return "Salvou Cargas";
                    case ObjetosDeValor.Embarcador.Enumeradores.TipoLogFatura.SalvouValorFatura:
                        return "Salvou valor Fatura";
                    case ObjetosDeValor.Embarcador.Enumeradores.TipoLogFatura.VisualizouFatura:
                        return "Visualizou Fatura";
                    case ObjetosDeValor.Embarcador.Enumeradores.TipoLogFatura.AdicionouNovaCarga:
                        return "Adicionou nova carga";
                    case ObjetosDeValor.Embarcador.Enumeradores.TipoLogFatura.AlterouParcela:
                        return "Alterou parcela gerada";
                    default:
                        return "";
                }
            }
        }

        public virtual bool Equals(FaturaLog other)
        {
            if (other.Codigo == this.Codigo)
                return true;
            else
                return false;
        }
    }
}
