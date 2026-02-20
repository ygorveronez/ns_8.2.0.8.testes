using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;

namespace Dominio.Entidades.Embarcador.GestaoPatio
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_CONTROLE_VISITA", EntityName = "ControleVisita", Name = "Dominio.Entidades.Embarcador.GestaoPatio.ControleVisita", NameType = typeof(ControleVisita))]
    public class ControleVisita : EntidadeBase
    {
        public ControleVisita() { }

        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "COV_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Numero", Column = "CTP_NUMERO", TypeType = typeof(int), NotNull = false)]
        public virtual int Numero { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "COV_DATA_HORA_ENTRADA", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataHoraEntrada { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "COV_DATA_HORA_SAIDA", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataHoraSaida { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "COV_DATA_HORA_PREVISAO_SAIDA", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataHoraPrevisaoSaida { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CPF", Column = "COV_CPF", TypeType = typeof(string), NotNull = false, Length = 20)]
        public virtual string CPF { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Nome", Column = "COV_NOME", TypeType = typeof(string), NotNull = false, Length = 2000)]
        public virtual string Nome { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "COV_DATA_NASCIMENTO", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataNascimento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Identidade", Column = "COV_IDENTIDADE", TypeType = typeof(string), NotNull = false, Length = 100)]
        public virtual string Identidade { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "OrgaoEmissor", Column = "COV_ORGAO_EMISSOR", TypeType = typeof(string), NotNull = false, Length = 100)]
        public virtual string OrgaoEmissor { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Estado", Column = "UF_SIGLA", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Estado Estado { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Empresa", Column = "COV_EMPRESA", TypeType = typeof(string), NotNull = false, Length = 2000)]
        public virtual string Empresa { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Usuario", Column = "FUN_AUTORIZADOR", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Usuario Autorizador { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Setor", Column = "SET_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Setor Setor { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PlacaVeiculo", Column = "COV_PLACA_VEICULO", TypeType = typeof(string), NotNull = false, Length = 20)]
        public virtual string PlacaVeiculo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ModeloVeiculo", Column = "COV_MODELO_VEICULO", TypeType = typeof(string), NotNull = false, Length = 2000)]
        public virtual string ModeloVeiculo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Observacao", Column = "COV_OBSERVACAO", TypeType = typeof(string), NotNull = false, Length = 2000)]
        public virtual string Observacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "COV_SITUACAO", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.SituacaoControleVisita), NotNull = false)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.SituacaoControleVisita Situacao { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ControleVisitaPessoa", Column = "CVP_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual ControleVisitaPessoa ControleVisitaPessoa { get; set; }

        public virtual string DescricaoSituacaoControleVisita
        {
            get { return this.Situacao.ObterDescricao(); }
        }

        public virtual string Descricao
        {
            get
            {
                return this.Numero.ToString();
            }
        }
    }
}