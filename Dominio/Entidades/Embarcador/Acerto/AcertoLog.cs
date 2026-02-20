using System;

namespace Dominio.Entidades.Embarcador.Acerto
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_ACERTO_LOG_USUARIO", EntityName = "AcertoLog", Name = "Dominio.Entidades.Embarcador.Acerto.AcertoLog", NameType = typeof(AcertoLog))]
    public class AcertoLog : EntidadeBase, IEquatable<Dominio.Entidades.Embarcador.Acerto.AcertoLog>
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "ALO_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "AcertoViagem", Column = "ACV_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Acerto.AcertoViagem AcertoViagem { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Usuario", Column = "FUN_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Usuario Usuario { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataHora", Column = "ALO_DATA_HORA", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime DataHora { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TipoAcao", Column = "ALO_TIPO_ACAO", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.TipoAcaoLogAcertoViagem), NotNull = true)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.TipoAcaoLogAcertoViagem TipoAcao { get; set; }

        public virtual string DescricaoEtapa
        {
            get
            {
                switch (this.TipoAcao)
                {
                    case ObjetosDeValor.Embarcador.Enumeradores.TipoAcaoLogAcertoViagem.Abastecimentos:
                        return "Informou abastecimentos";
                    case ObjetosDeValor.Embarcador.Enumeradores.TipoAcaoLogAcertoViagem.Inicio:
                        return "Iniciou o acerto";
                    case ObjetosDeValor.Embarcador.Enumeradores.TipoAcaoLogAcertoViagem.Cargas:
                        return "Informou cargas";                    
                    case ObjetosDeValor.Embarcador.Enumeradores.TipoAcaoLogAcertoViagem.OutrasDespesas:
                        return "Informou outras despesas";
                    case ObjetosDeValor.Embarcador.Enumeradores.TipoAcaoLogAcertoViagem.Pedagios:
                        return "Informou pedágios";
                    case ObjetosDeValor.Embarcador.Enumeradores.TipoAcaoLogAcertoViagem.AlteradoBonificacaoCliente:
                        return "Alterou a Bonificação do Cliente";
                    case ObjetosDeValor.Embarcador.Enumeradores.TipoAcaoLogAcertoViagem.AlteradoCargaFracionada:
                        return "Alterou a Carga Fracionada";
                    case ObjetosDeValor.Embarcador.Enumeradores.TipoAcaoLogAcertoViagem.AlteradoKMFinal:
                        return "Alterou o KM Final";
                    case ObjetosDeValor.Embarcador.Enumeradores.TipoAcaoLogAcertoViagem.AlteradoVinculosCarga:
                        return "Alterou os vínculos da Carga";
                    default:
                        return "";
                }
            }
        }

        public virtual bool Equals(AcertoLog other)
        {
            if (other.Codigo == this.Codigo)
                return true;
            else
                return false;
        }
    }
}
