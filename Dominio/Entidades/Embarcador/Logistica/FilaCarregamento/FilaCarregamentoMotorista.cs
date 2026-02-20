using System;

namespace Dominio.Entidades.Embarcador.Logistica
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_FILA_CARREGAMENTO_MOTORISTA", EntityName = "FilaCarregamentoMotorista", Name = "Dominio.Entidades.Embarcador.Logistica.FilaCarregamentoMotorista", NameType = typeof(FilaCarregamentoMotorista))]
    public class FilaCarregamentoMotorista : EntidadeBase, IEquatable<FilaCarregamentoMotorista>
    {
        #region Propriedades Públicas

        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "FLM_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataEntrada", Column = "FLM_DATA_ENTRADA", TypeType = typeof(DateTime), NotNull = true)]
        public virtual DateTime DataEntrada { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DistanciaCentroCarregamento", Column = "FLM_DISTANCIA_CENTRO_CARREGAMENTO", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = true)]
        public virtual decimal DistanciaCentroCarregamento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "FLM_LOJA_PROXIMIDADE", TypeType = typeof(bool), NotNull = true)]
        public virtual bool LojaProximidade { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Posicao", Column = "FLM_POSICAO", TypeType = typeof(int), NotNull = true)]
        public virtual int Posicao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "FLM_SITUACAO", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.SituacaoFilaCarregamentoMotorista), NotNull = true)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.SituacaoFilaCarregamentoMotorista Situacao { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Usuario", Column = "FLM_CODIGO_MOTORISTA", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Usuario Motorista { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "CentroCarregamento", Column = "CEC_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual CentroCarregamento CentroCarregamento { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "FilaCarregamentoConjuntoVeiculo", Column = "FCV_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual FilaCarregamentoConjuntoVeiculo ConjuntoVeiculo { get; set; }

        #endregion

        #region Métodos Públicos

        public virtual string ObterCorFonte()
        {
            switch (Situacao)
            {
                case ObjetosDeValor.Embarcador.Enumeradores.SituacaoFilaCarregamentoMotorista.ReboqueAtrelado: return "#212529";
                default: return string.Empty;
            }
        }

        public virtual string ObterCorLinha()
        {
            switch (Situacao)
            {
                case ObjetosDeValor.Embarcador.Enumeradores.SituacaoFilaCarregamentoMotorista.ReboqueAtrelado: return "#53c685";
                default: return string.Empty;
            }
        }

        public virtual bool Equals(FilaCarregamentoMotorista other)
        {
            return (other.Codigo == this.Codigo);
        }

        #endregion
    }
}
