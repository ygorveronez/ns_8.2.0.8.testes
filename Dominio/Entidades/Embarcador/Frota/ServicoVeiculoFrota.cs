namespace Dominio.Entidades.Embarcador.Frota
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_FROTA_SERVICO_VEICULO", EntityName = "ServicoVeiculoFrota", Name = "Dominio.Entidades.Embarcador.Frota.ServicoVeiculoFrota", NameType = typeof(ServicoVeiculoFrota))]
    public class ServicoVeiculoFrota : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "SEV_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "PlanoConta", Column = "PLA_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Financeiro.PlanoConta PlanoConta { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CodigoIntegracao", Column = "SEV_CODIGO_INTEGRACAO", TypeType = typeof(string), Length = 50, NotNull = false)]
        public virtual string CodigoIntegracao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Descricao", Column = "SEV_DESCRICAO", TypeType = typeof(string), Length = 150, NotNull = true)]
        public virtual string Descricao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValidadeKM", Column = "SEV_VALIDADE_KM", TypeType = typeof(int), NotNull = false)]
        public virtual int ValidadeKM { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ToleranciaKM", Column = "SEV_TOLERANCIA_KM", TypeType = typeof(int), NotNull = false)]
        public virtual int ToleranciaKM { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValidadeDias", Column = "SEV_VALIDADE_DIAS", TypeType = typeof(int), NotNull = false)]
        public virtual int ValidadeDias { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ToleranciaDias", Column = "SEV_TOLERANCIA_DIAS", TypeType = typeof(int), NotNull = false)]
        public virtual int ToleranciaDias { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Observacao", Column = "SEV_OBSERVACAO", TypeType = typeof(string), Length = 400, NotNull = false)]
        public virtual string Observacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ExecucaoUnica", Column = "SEV_EXECUCAO_UNICA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ExecucaoUnica { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "SEV_PERMITE_LANCAMENTO_SEM_VALOR", TypeType = typeof(bool), NotNull = false)]
        public virtual bool PermiteLancamentoSemValor { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "SEV_OBRIGATORIO_PARA_REALIZAR_CARGA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ObrigatorioParaRealizarCarga { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Motivo", Column = "SEV_MOTIVO", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.MotivoServicoVeiculo), NotNull = false)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.MotivoServicoVeiculo Motivo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Tipo", Column = "SEV_TIPO", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.TipoServicoVeiculo), NotNull = true)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.TipoServicoVeiculo Tipo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Ativo", Column = "SEV_ATIVO", TypeType = typeof(bool), NotNull = true)]
        public virtual bool Ativo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Empresa", Column = "EMP_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Empresa Empresa { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TempoEstimado", Column = "SEV_TEMPO_ESTIMADO", TypeType = typeof(int), NotNull = false)]
        public virtual int TempoEstimado { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TipoManutencao", Column = "SEV_TIPO_MANUTENCAO", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.TipoManutencaoServicoVeiculo), NotNull = false)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.TipoManutencaoServicoVeiculo TipoManutencao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValidadeHorimetro", Column = "SEV_VALIDADE_HORIMETRO", TypeType = typeof(int), NotNull = false)]
        public virtual int ValidadeHorimetro { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ToleranciaHorimetro", Column = "SEV_TOLERANCIA_HORIMETRO", TypeType = typeof(int), NotNull = false)]
        public virtual int ToleranciaHorimetro { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "SEV_SERVICO_PARA_EQUIPAMENTO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ServicoParaEquipamento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Cores", Column = "SEV_CORES", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.Cores), NotNull = false)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.Cores Cores { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Prioridade", Column = "SEV_PRIORIDADE", TypeType = typeof(int), NotNull = false)]
        public virtual int Prioridade { get; set; }

        public virtual string DescricaoAtivo
        {
            get
            {
                if (Ativo)
                    return "Ativo";
                else
                    return "Inativo";
            }
        }
    }
}
