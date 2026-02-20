using Dominio.Entidades.Embarcador.Filiais;
using System.Collections.Generic;

namespace Dominio.Entidades.Embarcador.Pessoas
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_CLIENTE_DESCARGA", EntityName = "ClienteDescarga", Name = "Dominio.Entidades.Embarcador.Pessoas.ClienteDescarga", NameType = typeof(ClienteDescarga))]
    public class ClienteDescarga : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "CLD_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Cliente", Column = "CLI_CGCCPF", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Cliente Cliente { get; set; }
        
        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Filial", Column = "FIL_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Filial FilialResponsavelRedespacho { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorPorPallet", Column = "CLD_VALOR_FIXO", TypeType = typeof(decimal), Scale = 4, Precision = 18, NotNull = false)]
        public virtual decimal ValorPorPallet { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CLD_VALOR_POR_VOLUME", TypeType = typeof(decimal), Scale = 4, Precision = 18, NotNull = false)]
        public virtual decimal ValorPorVolume { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "HoraLimiteDescarga", Column = "CLD_HORA_LIMETE_DESCARGA", TypeType = typeof(string), Length = 10, NotNull = false)]
        public virtual string HoraLimiteDescarga { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "HoraInicioDescarga", Column = "CLD_HORA_INICIO_DESCARGA", TypeType = typeof(string), Length = 10, NotNull = false)]
        public virtual string HoraInicioDescarga { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DeixarReboqueParaDescarga", Column = "CLD_DEIXAR_REBOQUE_PARA_DESCARGA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool DeixarReboqueParaDescarga { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NaoRecebeCargaCompartilhada", Column = "CLD_NAO_RECEBE_CARGA_COMPARTILHADA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool NaoRecebeCargaCompartilhada { get; set; }

        /// <summary>
        /// No app, normalmente quando o Tipo de Operação tem uma Checklist de entrega, ela deve ser preenchida em todas entregas.
        /// Essa flag faz com que não seja necessário o preenchimento nesse cliente.
        /// </summary>
        [NHibernate.Mapping.Attributes.Property(0, Name = "NaoExigePreenchimentoDeChecklistEntrega", Column = "CLD_NAO_EXIGE_PREENCHIMENTO_DE_CHECKLIST_ENTREGA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool NaoExigePreenchimentoDeChecklistEntrega { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "RestricaoEntrega", Column = "REE_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Pessoas.RestricaoEntrega RestricaoEntrega { get; set; }

        [NHibernate.Mapping.Attributes.Set(0, Name = "RestricoesDescarga", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_CLIENTE_RESTRICAO_DESCARGA")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "CLD_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "RestricaoEntrega", Column = "REE_CODIGO")]
        public virtual ICollection<RestricaoEntrega> RestricoesDescarga { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Cliente", Column = "CLI_CGCCPF_ORIGEM", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Cliente ClienteOrigem { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Empresa", Column = "EMP_CODIGO_DISTRIBUIDOR", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Empresa Distribuidor { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Veiculo", Column = "VEI_CODIGO_DISTRIBUIDOR", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Veiculo VeiculoDistribuidor { get; set; }


        [NHibernate.Mapping.Attributes.Property(0, Name = "Domingo", Column = "CLD_DOMINGO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool Domingo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "TipoDeCarga", Column = "TCG_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Cargas.TipoDeCarga TipoDeCarga { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TempoAgendamento", Column = "CLD_TEMPO_AGENDAMENTO", TypeType = typeof(string), Length = 80, NotNull = false)]
        public virtual string TempoAgendamento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "FormaAgendamento", Column = "CLD_FORMA_AGENDAMENTO", TypeType = typeof(string), Length = 80, NotNull = false)]
        public virtual string FormaAgendamento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "LinkParaAgendamento", Column = "CLD_LINK_PARA_AGENDAMENTO", TypeType = typeof(string), Length = 200, NotNull = false)]
        public virtual string LinkParaAgendamento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "AgendamentoExigeNotaFiscal", Column = "CLD_AGENDAMENTO_EXIGE_NOTA_FISCAL", TypeType = typeof(bool), NotNull = false)]
        public virtual bool AgendamentoExigeNotaFiscal { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ExigeAgendamento", Column = "CLD_EXIGE_AGENDAMENTO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ExigeAgendamento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "AgendamentoDescargaObrigatorio", Column = "CLD_AGENDAMENTO_DESCARGA_OBRIGATORIO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool AgendamentoDescargaObrigatorio { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PossuiCanhotoDeDuasOuMaisPaginas", Column = "CLI_POSSUI_CANHOTO_DE_DUAS_OU_MAIS_PAGINAS", TypeType = typeof(bool), NotNull = false)]
        public virtual bool PossuiCanhotoDeDuasOuMaisPaginas { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "QuantidadeDePaginasDoCanhoto", Column = "CLI_QUANTIDADE_DE_PAGINAS_DO_CANHOTO", TypeType = typeof(int), NotNull = false)]
        public virtual int QuantidadeDePaginasDoCanhoto { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ExigeSenhaNoAgendamento", Column = "CLD_EXIGE_SENHA_NO_AGENDAMENTO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ExigeSenhaNoAgendamento { get; set; }

        public virtual string Descricao
        {
            get
            {
                return Cliente.Descricao;
            }
        }
    }
}
