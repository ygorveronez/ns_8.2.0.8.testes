using System;

namespace Dominio.Entidades.Embarcador.RH
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_COMISSAO_FUNCIONARIO", EntityName = "ComissaoFuncionario", Name = "Dominio.Entidades.Embarcador.RH.ComissaoFuncionario", NameType = typeof(ComissaoFuncionario))]
    public class ComissaoFuncionario : EntidadeBase, IEquatable<Dominio.Entidades.Embarcador.RH.ComissaoFuncionario>
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "CMF_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Localidade", Column = "LOC_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Localidade Localidade { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Usuario", Column = "FUN_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Usuario UsuarioGerouComissao { get; set; }
        
        [NHibernate.Mapping.Attributes.Property(0, Name = "DataAlteracao", Column = "CMF_DATA_ALTERACAO", TypeType = typeof(DateTime), NotNull = true)]
        public virtual DateTime DataAlteracao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataInicio", Column = "CMF_DATA_INICIO", TypeType = typeof(DateTime), NotNull = true)]
        public virtual DateTime DataInicio { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataFim", Column = "CMF_DATA_FIM", TypeType = typeof(DateTime), NotNull = true)]
        public virtual DateTime DataFim { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PercentualComissao", Column = "CMF_PERCENTUAL_COMISSAO", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal PercentualComissao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NumeroDiasEmViagem", Column = "CMF_NUMERO_DIAS_EM_VIAGEM", TypeType = typeof(int), NotNull = false)]
        public virtual int NumeroDiasEmViagem { get; set; }


        [NHibernate.Mapping.Attributes.Property(0, Name = "PercentualBaseCalculoComissao", Column = "CMF_PERCENTUAL_BASE_CALCULO_COMISSAO", TypeType = typeof(decimal), Scale = 6, Precision = 18, NotNull = false)]
        public virtual decimal PercentualBaseCalculoComissao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorDiaria", Column = "CMF_VALOR_DIARIA", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal ValorDiaria { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "MesagemBaseCalculoComissao", Column = "CMF_MENSAGEM_BASE_CALCULO_COMISSAO", Type = "StringClob", NotNull = false)]
        public virtual string MesagemBaseCalculoComissao { get; set; }
        
        [NHibernate.Mapping.Attributes.Property(0, Name = "SituacaoComissaoFuncionario", Column = "CMF_SITUACAO_COMISSAO_FUNCIONARIO", TypeType = typeof(Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoComissaoFuncionario), NotNull = true)]
        public virtual Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoComissaoFuncionario SituacaoComissaoFuncionario { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TotalFuncionarios", Column = "CMF_TOTAL_FUNCIONARIO", TypeType = typeof(int), NotNull = false)]
        public virtual int TotalFuncionarios { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PercentualGerado", Column = "CMF_PERCENTUAL_GERADO", TypeType = typeof(int), NotNull = false)]
        public virtual int PercentualGerado { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "MensagemFalhaGeracao", Column = "CMF_MENSAGEM_FALHA_GERACAO", TypeType = typeof(string), Length = 200, NotNull = false)]
        public virtual string MensagemFalhaGeracao { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Usuario", Column = "FUN_CODIGO_MOTORISTA", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Usuario Motorista { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Cargo", Column = "CRG_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Pessoas.Cargo CargoMotorista { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "CentroResultado", Column = "CRE_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Financeiro.CentroResultado CentroResultado { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ImportarPlanilhaListagemMotoristas", Column = "CRE_IMPORTAR_PLANILHA_LISTAGEM_MOTORISTAS", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ImportarPlanilhaListagemMotoristas { get; set; }

        public virtual string Descricao
        {
            get
            {
                return this.DataInicio.ToString("dd/MM/yyyy") + " - " + this.DataFim.ToString("dd/MM/yyyy");
            }
        }

        public virtual bool Equals(ComissaoFuncionario other)
        {
            if (other.Codigo == this.Codigo)
                return true;
            else
                return false;
        }


        public virtual string DescricaoSituacaoComissaoFuncionario
        {
            get
            {
                switch (this.SituacaoComissaoFuncionario)
                {
                    case ObjetosDeValor.Embarcador.Enumeradores.SituacaoComissaoFuncionario.Cancelada:
                        return "Comissão Cancelada";
                    case ObjetosDeValor.Embarcador.Enumeradores.SituacaoComissaoFuncionario.EmGeracao:
                        return "Gerando a Comissão";
                    case ObjetosDeValor.Embarcador.Enumeradores.SituacaoComissaoFuncionario.Finalizada:
                        return "Comissão Finalizada";
                    case ObjetosDeValor.Embarcador.Enumeradores.SituacaoComissaoFuncionario.Gerada:
                        return "Comissão Gerada";
                    case ObjetosDeValor.Embarcador.Enumeradores.SituacaoComissaoFuncionario.FalhaNaGeracao:
                        return "Falha ao Gerar a Comissão";
                    case ObjetosDeValor.Embarcador.Enumeradores.SituacaoComissaoFuncionario.AgImportacaoPlanilha:
                        return "Aguardando Importação de planilha";
                    default:
                        return "";
                }
            }
        }
    }
}
