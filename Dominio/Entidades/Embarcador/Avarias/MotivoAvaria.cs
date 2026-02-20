namespace Dominio.Entidades.Embarcador.Avarias
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_MOTIVO_AVARIA", EntityName = "MotivoAvaria", Name = "Dominio.Entidades.Embarcador.Avarias.MotivoAvaria", NameType = typeof(MotivoAvaria))]
    public class MotivoAvaria : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "MAV_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Descricao", Column = "MAV_DESCRICAO", TypeType = typeof(string), Length = 200, NotNull = false)]
        public virtual string Descricao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Responsavel", Column = "MAV_RESPONSAVEL", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.ResponsavelAvaria), NotNull = false)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.ResponsavelAvaria Responsavel { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Finalidade", Column = "MAV_FINALIDADE", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.FinalidadeMotivoAvaria), NotNull = false)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.FinalidadeMotivoAvaria Finalidade { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "PlanoConta", Column = "PLA_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Financeiro.PlanoConta ContaContabil { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Ativo", Column = "MAV_ATIVO", TypeType = typeof(bool), NotNull = true)]
        public virtual bool Ativo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Observacao", Column = "MAV_OBSERVACAO", TypeType = typeof(string), Length = 2000, NotNull = false)]
        public virtual string Observacao { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "TipoDeOcorrenciaDeCTe", Column = "OCO_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual TipoDeOcorrenciaDeCTe TipoOcorrencia { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "GerarOcorrenciaAutomaticamente", Column = "MAV_GERAR_OCORRENCIA_AUTOMATICAMENTE", TypeType = typeof(bool), NotNull = false)]
        public virtual bool GerarOcorrenciaAutomaticamente { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ObrigarInformarValorParaLiberarOcorrencia", Column = "MAV_OBRIGAR_INFORMAR_VALOR_PARA_LIBERAR_OCORRENCIA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ObrigarInformarValorParaLiberarOcorrencia { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "GerarCTeValorAnteriorTratativaReentrega", Column = "MAV_GERAR_CTE_VALOR_ANTERIOR_TRATATIVA_REENTREGA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool GerarCTeValorAnteriorTratativaReentrega { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CalcularOcorrenciaPorTabelaFrete", Column = "MAV_CALCULAR_OCORRENCIA_POR_TABELA_FRETE", TypeType = typeof(bool), NotNull = false)]
        public virtual bool CalcularOcorrenciaPorTabelaFrete { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ObrigarAnexo", Column = "MAV_OBRIGAR_ANEXO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ObrigarAnexo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NaoPermitirAberturaAvariasMesmoMotivoECarga", Column = "MAV_NAO_PERMITIR_ABERTURA_AVARIAS_MESMO_MOTIVO_E_CARGA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool NaoPermitirAberturaAvariasMesmoMotivoECarga { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PermitirInformarQuantidadeMaiorMercadoriaAvariada", Column = "MAV_PERMITIR_INFORMAR_QUANTIDADE_MAIOR_MERCADORIA_AVARIADA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool PermitirInformarQuantidadeMaiorMercadoriaAvariada { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DesabilitarBotaoTermo", Column = "MAV_DESABILITAR_BOTAO_TERMO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool DesabilitarBotaoTermo { get; set; }

        #region Propriedades Virtuais

        public virtual string DescricaoAtivo
        {
            get
            {
                if (this.Ativo)
                    return "Ativo";
                else
                    return "Inativo";
            }
        }

        public virtual string DescricaoFinalidade
        {
            get
            {
                switch (Finalidade)
                {
                    case ObjetosDeValor.Embarcador.Enumeradores.FinalidadeMotivoAvaria.AutorizacaoAvaria:
                        return "Autorização Avaria";
                    case ObjetosDeValor.Embarcador.Enumeradores.FinalidadeMotivoAvaria.MotivoAvaria:
                        return "Motivo Avaria";
                    default:
                        return "";
                }
            }
        }

        public virtual string DescricaoResponsavel
        {
            get
            {
                switch (Responsavel)
                {
                    case ObjetosDeValor.Embarcador.Enumeradores.ResponsavelAvaria.Transportador:
                        return "Transportador";
                    case ObjetosDeValor.Embarcador.Enumeradores.ResponsavelAvaria.CarregamentoDescarregamento:
                        return "Carregamento/Descarregamento";
                    default:
                        return "";
                }
            }
        }

        public virtual bool Equals(MotivoAvaria other)
        {
            if (other.Codigo == this.Codigo)
                return true;
            else
                return false;
        }

        #endregion
    }
}
