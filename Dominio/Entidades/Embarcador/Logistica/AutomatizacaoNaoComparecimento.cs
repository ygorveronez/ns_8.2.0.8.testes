using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;

namespace Dominio.Entidades.Embarcador.Logistica
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_AUTOMATIZACAO_NAO_COMPARECIMENTO", EntityName = "AutomatizacaoNaoComparecimento", Name = "Dominio.Entidades.Embarcador.Logistica.AutomatizacaoNaoComparecimento", NameType = typeof(AutomatizacaoNaoComparecimento))]
    public class AutomatizacaoNaoComparecimento : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "ANC_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "CentroCarregamento", Column = "CEC_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual CentroCarregamento CentroCarregamento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataCadastro", Column = "ANC_DATA_CADASTRO", TypeType = typeof(DateTime), NotNull = true)]
        public virtual DateTime DataCadastro { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Gatilho", Column = "ANC_GATILHO", TypeType = typeof(GatilhoAutomatizacaoNaoComparecimento), NotNull = true)]
        public virtual GatilhoAutomatizacaoNaoComparecimento Gatilho { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "HorasTolerancia", Column = "ANC_HORAS_TOLERANCIA", TypeType = typeof(int), NotNull = true)]
        public virtual int HorasTolerancia { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "ANC_BLOQUEAR_CARGA", TypeType = typeof(bool), NotNull = true)]
        public virtual bool BloquearCarga { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "ANC_ENVIAR_EMAIL_TRANSPORTADOR", TypeType = typeof(bool), NotNull = true)]
        public virtual bool EnviarEmailTransportador { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "ANC_RETORNAR_CARGA_PARA_EXCEDENTE", TypeType = typeof(bool), NotNull = false)]
        public virtual bool RetornarCargaParaExcedente { get; set; }

        public virtual string Descricao
        {
            get
            {
                return $"Automatização de no-show para {Gatilho.ObterDescricao().ToLower()}";
            }
        }
    }
}
