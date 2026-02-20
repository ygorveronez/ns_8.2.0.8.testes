using System;

namespace Dominio.Entidades
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_OBS_PADRAO", EntityName = "Observacao", Name = "Dominio.Entidades.Observacao", NameType = typeof(Observacao))]
    public class Observacao : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "OBS_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Empresa", Column = "EMP_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Empresa Empresa { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Tipo", Column = "OBS_TIPO", TypeType = typeof(Dominio.Enumeradores.TipoObservacao), NotNull = false)]
        public virtual Dominio.Enumeradores.TipoObservacao Tipo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Descricao", Column = "OBS_DESCRICAO", TypeType = typeof(string), Length = 2000, NotNull = false)]
        public virtual string Descricao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataCadastro", Column = "OBS_DATA", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataCadastro { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Status", Column = "OBS_STATUS", TypeType = typeof(string), Length = 1, NotNull = false)]
        public virtual string Status { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CST", Column = "OBS_CST", TypeType = typeof(string), Length = 2, NotNull = false)]
        public virtual string CST { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "UF", Column = "OBS_UF", TypeType = typeof(string), Length = 2, NotNull = false)]
        public virtual string UF { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "UFDestino", Column = "OBS_UF_DESTINO", TypeType = typeof(string), Length = 2, NotNull = false)]
        public virtual string UFDestino { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Log", Column = "OBS_LOG", Type = "StringClob", NotNull = false)]
        public virtual string Log { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TipoCTe", Column = "OBS_TIPO_CTE", TypeType = typeof(Dominio.Enumeradores.TipoCTE), NotNull = false)]
        public virtual Dominio.Enumeradores.TipoCTE TipoCTe { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Operacao", Column = "OBS_OPERACAO", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.Operacao), NotNull = false)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.Operacao? Operacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Automatica", Column = "OBS_AUTOMATICA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool Automatica { get; set; }

        public virtual string DescricaoTipoCTe
        {
            get
            {
                switch (TipoCTe)
                {
                    case Enumeradores.TipoCTE.Normal:
                        return "Normal";
                    case Enumeradores.TipoCTE.Complemento:
                        return "Complemento";
                    case Enumeradores.TipoCTE.Anulacao:
                        return "Anulação";
                    case Enumeradores.TipoCTE.Substituto:
                        return "Substituto";
                    default:
                        return "";
                }
            }
        }

        public virtual string DescricaoTipoObservacao
        {
            get
            {
                switch (Tipo)
                {
                    case Enumeradores.TipoObservacao.Fisco:
                        return "Fisco";
                    case Enumeradores.TipoObservacao.Contribuinte:
                        return "Contribuinte";
                    case Enumeradores.TipoObservacao.Geral:
                        return "Geral";
                    default:
                        return "";
                }
            }
        }

        public virtual string DescricaoOperacao
        {
            get
            {
                switch (Operacao)
                {
                    case ObjetosDeValor.Embarcador.Enumeradores.Operacao.Todos:
                        return "Todos";
                    case ObjetosDeValor.Embarcador.Enumeradores.Operacao.Interestadual:
                        return "Interestadual";
                    case ObjetosDeValor.Embarcador.Enumeradores.Operacao.Intraestadual:
                        return "Intraestadual";
                    default:
                        return "";
                }
            }
        }
    }
}
