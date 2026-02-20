using System;
using System.Collections.Generic;

namespace Dominio.Entidades.Embarcador.Atendimento
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_ATENDIMENTO", EntityName = "Atendimento", Name = "Dominio.Entidades.Embarcador.Atendimento.Atendimento", NameType = typeof(Atendimento))]
    public class Atendimento : EntidadeBase, IEquatable<Dominio.Entidades.Embarcador.Atendimento.Atendimento>
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "ATE_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataInicial", Column = "ATE_DATA_INICIAL", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataInicial { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataFinal", Column = "ATE_DATA_FINAL", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataFinal { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Numero", Column = "PEV_NUMERO", TypeType = typeof(int), NotNull = false)]
        public virtual int Numero { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Status", Column = "ATE_STATUS", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.StatusAtendimento), NotNull = false)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.StatusAtendimento Status { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ContatoAtendimento", Column = "ATE_CONTATO_ATENDIMENTO", TypeType = typeof(string), Length = 300, NotNull = false)]
        public virtual string ContatoAtendimento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TipoContato", Column = "ATE_TIPO_CONTATO", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.TipoContatoAtendimento), NotNull = false)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.TipoContatoAtendimento TipoContato { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TipoAcessoRemoto", Column = "ATE_TIPO_ACESSO_REMOTO", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.TipoAcessoRemoto), NotNull = false)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.TipoAcessoRemoto TipoAcessoRemoto { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Observacao", Column = "ATE_OBSERVACAO", TypeType = typeof(string), Length = 5000, NotNull = false)]
        public virtual string Observacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DuracaoTotal", Column = "ATE_DURACAO_TOTAL", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DuracaoTotal { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ObservacaoSuporte", Column = "ATE_OBSERVACAO_SUPORTE", TypeType = typeof(string), Length = 300, NotNull = false)]
        public virtual string ObservacaoSuporte { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Prioridade", Column = "ATE_PRIORIDADE", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.PrioridadeAtendimento), NotNull = false)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.PrioridadeAtendimento Prioridade { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NivelSatisfacao", Column = "ATE_SATISFACAO", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.NivelSatisfacao), NotNull = false)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.NivelSatisfacao NivelSatisfacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TipoSistema", Column = "ATE_TIPO_SISTEMA", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.TipoSistema), NotNull = false)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.TipoSistema TipoSistema { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "AtendimentoTipo", Column = "ATT_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Atendimento.AtendimentoTipo AtendimentoTipo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Usuario", Column = "FUN_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Usuario Funcionario { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Cliente", Column = "CLI_CGCCPF", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Cliente Cliente { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Empresa", Column = "EMP_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Empresa Empresa { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Empresa", Column = "EMP_CODIGO_FILHO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Empresa EmpresaFilho { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Usuario", Column = "FUN_CODIGO_ERRO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Usuario FuncionarioErro { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "ATE_FAQ", TypeType = typeof(bool), NotNull = false)]
        public virtual bool Faq { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "ATE_NECESSITOU_AUXILIO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool NecessitouAuxilio { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Telas", Formula = @"ISNULL(SUBSTRING((SELECT DISTINCT ', ' + CAST(ISNULL(T.ATL_DESCRICAO, '') AS NVARCHAR(500))
                                                                                        FROM T_ATENDIMENTO_TAREFA AT
                                                                                        JOIN T_ATENDIMENTO_TELA T ON AT.ATL_CODIGO = T.ATL_CODIGO
                                                                                        WHERE AT.ATE_CODIGO = ATE_CODIGO FOR XML PATH('')), 3, 1000), '')", TypeType = typeof(string), Lazy = true)]
        public virtual string Telas { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Modulos", Formula = @"ISNULL(SUBSTRING((SELECT DISTINCT ', ' + CAST(ISNULL(M.ATM_DESCRICAO, '') AS NVARCHAR(500))
                                                                                        FROM T_ATENDIMENTO_TAREFA AT
                                                                                        JOIN T_ATENDIMENTO_MODULO M ON M.ATM_CODIGO = AT.ATM_CODIGO
                                                                                        WHERE AT.ATE_CODIGO = ATE_CODIGO FOR XML PATH('')), 3, 1000), '')", TypeType = typeof(string), Lazy = true)]
        public virtual string Modulos { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Motivos", Formula = @"ISNULL(SUBSTRING((SELECT DISTINCT ', ' + CAST(ISNULL(AT.ATC_MOTIVO_PROBLEMA, '') AS NVARCHAR(4000))
                                                                                        FROM T_ATENDIMENTO_TAREFA AT
                                                                                        WHERE AT.ATE_CODIGO = ATE_CODIGO FOR XML PATH('')), 3, 1000), '')", TypeType = typeof(string), Lazy = true)]
        public virtual string Motivos { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Titulos", Formula = @"ISNULL(SUBSTRING((SELECT DISTINCT ', ' + CAST(ISNULL(AT.ATC_TITULO, '') AS NVARCHAR(4000))
                                                                                        FROM T_ATENDIMENTO_TAREFA AT
                                                                                        WHERE AT.ATE_CODIGO = ATE_CODIGO FOR XML PATH('')), 3, 1000), '')", TypeType = typeof(string), Lazy = true)]
        public virtual string Titulos { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Name = "Respostas", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_ATENDIMENTO_RESPOSTA")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "ATE_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "AtendimentoResposta", Column = "ATR_CODIGO")]
        public virtual IList<AtendimentoResposta> Respostas { get; set; }
        public virtual string Descricao
        {
            get
            {
                return this.Numero.ToString();
            }
        }

        public virtual string DescricaoStatus
        {
            get
            {
                switch (this.Status)
                {
                    case ObjetosDeValor.Embarcador.Enumeradores.StatusAtendimento.Aberto:
                        return "Aberto";
                    case ObjetosDeValor.Embarcador.Enumeradores.StatusAtendimento.Cancelado:
                        return "Cancelado";
                    case ObjetosDeValor.Embarcador.Enumeradores.StatusAtendimento.EmAndamento:
                        return "Em Andamento";
                    case ObjetosDeValor.Embarcador.Enumeradores.StatusAtendimento.Finalizado:
                        return "Finalizado";
                    default:
                        return "";
                }
            }
        }

        public virtual string DescricaoPrioridade
        {
            get
            {
                switch (this.Prioridade)
                {
                    case ObjetosDeValor.Embarcador.Enumeradores.PrioridadeAtendimento.Alta:
                        return "Alta";
                    case ObjetosDeValor.Embarcador.Enumeradores.PrioridadeAtendimento.Baixa:
                        return "Baixa";
                    case ObjetosDeValor.Embarcador.Enumeradores.PrioridadeAtendimento.Normal:
                        return "Normal";
                    default:
                        return "";
                }
            }
        }

        public virtual bool Equals(Atendimento other)
        {
            if (other.Codigo == this.Codigo)
                return true;
            else
                return false;
        }
    }
}
