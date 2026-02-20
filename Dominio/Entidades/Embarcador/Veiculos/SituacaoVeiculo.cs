using Dominio.Entidades.Embarcador.Frota;
using System;
using System.Collections.Generic;

namespace Dominio.Entidades.Embarcador.Veiculos
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_VEICULO_SITUACAO", EntityName = "SituacaoVeiculo", Name = "Dominio.Entidades.Embarcador.Veiculos.SituacaoVeiculo", NameType = typeof(SituacaoVeiculo))]
    public class SituacaoVeiculo : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "VSI_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Veiculo", Column = "VEI_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Veiculo Veiculo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Usuario", Column = "FUM_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Usuario Usuario { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Usuario", Column = "FUM_CODIGO_MOTORISTA", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Usuario Motorista { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Localidade", Column = "LOC_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Localidade Localidade { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataHoraEmissao", Column = "VSI_DATA_HORA_EMISSAO", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataHoraEmissao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataHoraSituacao", Column = "VSI_DATA_HORA_SITUACAO", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataHoraSituacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "VeiculoVazio", Column = "VSI_VAZIO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool VeiculoVazio { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "AvisadoCarregamento", Column = "VSI_AVISADO_CARREGAMENTO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool AvisadoCarregamento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "VSI_SITUACAO_VEICULO", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.SituacaoVeiculo), NotNull = false)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.SituacaoVeiculo Situacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataHoraSaidaInicioViagem", Column = "VSI_DATA_HORA_SAIDA_INICIO_VIAGEM", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataHoraSaidaInicioViagem { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataHoraPrevisaoRetornoInicioViagem", Column = "VSI_DATA_HORA_PREVISAO_RETORNO_INICIO_VIAGEM", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataHoraPrevisaoRetornoInicioViagem { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Localidade", Column = "LOC_CODIGO_DESTINO_INICIO_VIAGEM", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Localidade LocalidadeDestinoInicioViagem { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataHoraRetornoViagem", Column = "VSI_DATA_HORA_RETORNO_VIAGEM", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataHoraRetornoViagem { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Localidade", Column = "LOC_CODIGO_RETORNO_VIAGEM", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Localidade LocalidadeRetornoViagem { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataHoraEntradaManutencao", Column = "VSI_DATA_HORA_ENTRADA_MANUTENCAO", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataHoraEntradaManutencao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataHoraPrevisaoSaidaManutencao", Column = "VSI_DATA_HORA_PREVISAO_SAIDA_MANUTENCAO", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataHoraPrevisaoSaidaManutencao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataHoraSaidaManutencao", Column = "VSI_DATA_HORA_SAIDA_MANUTENCAO", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataHoraSaidaManutencao { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "OrdemServicoFrota", Column = "OSE_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Frota.OrdemServicoFrota OrdemServicoFrota { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "OrdemServicoFrotaTipo", Column = "FOT_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual OrdemServicoFrotaTipo TipoOrdemServico { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Name = "VeiculosVinculadosSituacao", Cascade = "all", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_VEICULO_CONJUNTO_SITUACAO")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "VEC_CODIGO_PAI", ForeignKey = "VEI_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "Veiculo", ForeignKey = "VEI_CODIGO", Column = "VEC_CODIGO_FILHO", NotFound = NHibernate.Mapping.Attributes.NotFoundMode.Ignore)]
        public virtual IList<Dominio.Entidades.Veiculo> VeiculosVinculadosSituacao { get; set; }

        public virtual string Descricao
        {
            get
            {
                return this.Codigo.ToString();
            }
        }

        public virtual string DescricaoSituacao
        {
            get
            {
                switch (Situacao)
                {
                    case ObjetosDeValor.Embarcador.Enumeradores.SituacaoVeiculo.EmManutencao:
                        return "Em Manutenção";
                    case ObjetosDeValor.Embarcador.Enumeradores.SituacaoVeiculo.EmViagem:
                        return "Em Viagem";
                    case ObjetosDeValor.Embarcador.Enumeradores.SituacaoVeiculo.Disponivel:
                        return "Disponível";
                    case ObjetosDeValor.Embarcador.Enumeradores.SituacaoVeiculo.AvisoCarregamento:
                        return "Aviso de Carregamento";
                    case ObjetosDeValor.Embarcador.Enumeradores.SituacaoVeiculo.Vazio:
                        return "Veículo Vazio";
                    case ObjetosDeValor.Embarcador.Enumeradores.SituacaoVeiculo.EmFila:
                        return "Em Fila";
                    default:
                        return "Disponível";
                }
            }
        }
    }
}
