using System;
using System.Collections.Generic;

namespace Dominio.Entidades.Embarcador.Usuarios
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_PERFIL_ACESSO", EntityName = "PerfilAcesso", Name = "Dominio.Entidades.Embarcador.Usuarios.PerfilAcesso", NameType = typeof(PerfilAcesso))]
    public class PerfilAcesso : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "PAC_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "PAC_DESCRICAO", TypeType = typeof(string), Length = 100, NotNull = false)]
        public virtual string Descricao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "PAC_ATIVO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool Ativo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "PAC_PERFIL_ADMINISTRADOR", TypeType = typeof(bool), NotNull = false)]
        public virtual bool PerfilAdministrador { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "HoraInicialAcesso", Column = "PAC_HORA_INICIAL_ACESSO", Type = "NHibernate.Type.TimeAsTimeSpanType", NotNull = false)]
        public virtual TimeSpan? HoraInicialAcesso { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "HoraFinalAcesso", Column = "PAC_HORA_FINAL_ACESSO", Type = "NHibernate.Type.TimeAsTimeSpanType", NotNull = false)]
        public virtual TimeSpan? HoraFinalAcesso { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "PAC_PERMITE_FATURAMENTO_PERMISSAO_EXCLUSIVA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool PermiteFaturamentoPermissaoExclusiva { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "PAC_PERMITE_SALVAR_NOVO_RELATORIO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool PermiteSalvarNovoRelatorio { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "PAC_PERMITE_TORNAR_RELATORIO_PADRAO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool PermiteTornarRelatorioPadrao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "PAC_PERMITE_SALVAR_CONFIGURACOES_RELATORIOS_PARA_TODOS", TypeType = typeof(bool), NotNull = false)]
        public virtual bool PermiteSalvarConfiguracoesRelatoriosParaTodos { get; set; }

        [NHibernate.Mapping.Attributes.Set(0, Name = "ModulosLiberados", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_PERFIL_MODULO")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "PAC_CODIGO")]
        [NHibernate.Mapping.Attributes.Element(2, Column = "MOD_CODIGO_MODULO", TypeType = typeof(int), NotNull = true)]
        public virtual ICollection<int> ModulosLiberados { get; set; }

        [NHibernate.Mapping.Attributes.Set(0, Name = "TiposPropostasMultimodal", Cascade = "all", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_PERFIL_ACESSO_TIPO_PROPOSTA")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "PAC_CODIGO")]
        [NHibernate.Mapping.Attributes.Element(2, Column = "PAC_TIPO_PROPOSTA_MULTIMODAL", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.TipoPropostaMultimodal), NotNull = true)]
        public virtual ICollection<ObjetosDeValor.Embarcador.Enumeradores.TipoPropostaMultimodal> TiposPropostasMultimodal { get; set; }

        /*[NHibernate.Mapping.Attributes.Bag(0, Name = "FormulariosLiberados", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_PERFIL_FORMULARIO")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "PAC_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "PerfilFormulario", Column = "PAF_CODIGO")]
        public virtual ICollection<Dominio.Entidades.Embarcador.Usuarios.PerfilFormulario> FormulariosLiberados { get; set; }*/

        [NHibernate.Mapping.Attributes.Property(0, Column = "PAC_PERMITIR_ABRIR_OCORRENCIA_POS_PRAZO_SOLICITACAO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool PermitirAbrirOcorrenciaAposPrazoSolicitacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CodigoIntegracao", Column = "PAC_CODIGO_INTEGRACAO", TypeType = typeof(string), Length = 50, NotNull = false)]
        public virtual string CodigoIntegracao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "PAC_VISUALIZAR_TITULOS_PAGAMENTO_SALARIO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool VisualizarTitulosPagamentoSalario { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Turno", Column = "TUR_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Embarcador.Filiais.Turno Turno { get; set; }

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
    }
}
