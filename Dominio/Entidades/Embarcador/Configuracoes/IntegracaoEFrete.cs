using System;

namespace Dominio.Entidades.Embarcador.Configuracoes
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_CONFIGURACAO_INTEGRACAO_EFRETE", EntityName = "IntegracaoEFrete", Name = "Dominio.Entidades.Embarcador.Configuracoes.IntegracaoEFrete", NameType = typeof(IntegracaoEFrete))]
    public class IntegracaoEFrete : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "CIE_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "URL", Column = "CIE_URL", TypeType = typeof(string), Length = 200, NotNull = false)]
        public virtual string URL { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CodigoIntegrador", Column = "CIE_CODIGO_INTEGRADOR", TypeType = typeof(string), Length = 100, NotNull = false)]
        public virtual string CodigoIntegrador { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Usuario", Column = "CIE_USUARIO", TypeType = typeof(string), Length = 100, NotNull = false)]
        public virtual string Usuario { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Senha", Column = "CIE_SENHA", TypeType = typeof(string), Length = 100, NotNull = false)]
        public virtual string Senha { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Cliente", Column = "CLI_CGCCPF", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Cliente Cliente { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Cliente", Column = "CLI_CGCCPF_FORNECEDOR_VP", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Cliente FornecedorValePedagio { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CIE_DIAS_PRAZO", TypeType = typeof(int), NotNull = false)]
        public virtual int DiasPrazo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CIE_NOTIFICAR_TRANSPORTADOR_POR_EMAIL", TypeType = typeof(bool), NotNull = false)]
        public virtual bool NotificarTransportadorPorEmail { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CIE_ENVIAR_PONTOS_PASSAGEM_ROTA_FRETE", TypeType = typeof(bool), NotNull = false)]
        public virtual bool EnviarPontosPassagemRotaFrete { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CIE_ENVIAR_POLILINHA_ROTEIRIZACAO_CARGA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool EnviarPolilinhaRoteirizacaoCarga { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CIE_ENVIAR_TIPO_VEICULO_NA_INTEGRACAO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool EnviarTipoVeiculoNaIntegracao { get; set; }

        #region Obsolete
        [Obsolete("Mudado para configuração geral da e-frete")]
        [NHibernate.Mapping.Attributes.Property(0, Name = "PossuiIntegracaoRecebivel", Column = "CIE_POSSUI_INTEGRACAO_RECEBIVEL", TypeType = typeof(bool), Length = 200, NotNull = false)]
        public virtual bool PossuiIntegracaoRecebivel { get; set; }

        [Obsolete("Mudado para configuração geral da e-frete")]
        [NHibernate.Mapping.Attributes.Property(0, Name = "URLRecebivel", Column = "CIE_URL_RECEBIVEL", TypeType = typeof(string), Length = 200, NotNull = false)]
        public virtual string URLRecebivel { get; set; }

        [Obsolete("Mudado para configuração geral da e-frete")]
        [NHibernate.Mapping.Attributes.Property(0, Name = "URLAutenticacao", Column = "CIE_URL_AUTENTICACAO", TypeType = typeof(string), Length = 200, NotNull = false)]
        public virtual string URLAutenticacao { get; set; }

        [Obsolete("Mudado para configuração geral da e-frete")]
        [NHibernate.Mapping.Attributes.Property(0, Name = "URLCancelamentoRecebivel", Column = "CIE_URL_CANCELAMENTO", TypeType = typeof(string), Length = 200, NotNull = false)]
        public virtual string URLCancelamentoRecebivel { get; set; }

        [Obsolete("Mudado para configuração geral da e-frete")]
        [NHibernate.Mapping.Attributes.Property(0, Name = "URLPagamentoRecebivel", Column = "CIE_URL_PAGAMENTO_RECEBIVEL", TypeType = typeof(string), Length = 200, NotNull = false)]
        public virtual string URLPagamentoRecebivel { get; set; }

        [Obsolete("Mudado para configuração geral da e-frete")]
        [NHibernate.Mapping.Attributes.Property(0, Name = "APIKey", Column = "CIE_API_KEY", TypeType = typeof(string), Length = 200, NotNull = false)]
        public virtual string APIKey { get; set; }

        [Obsolete("Mudado para configuração geral da e-frete")]
        [NHibernate.Mapping.Attributes.Property(0, Name = "UsuarioRecebivel", Column = "CIE_USUARIO_RECEBIVEL", TypeType = typeof(string), Length = 200, NotNull = false)]
        public virtual string UsuarioRecebivel { get; set; }

        [Obsolete("Mudado para configuração geral da e-frete")]
        [NHibernate.Mapping.Attributes.Property(0, Name = "SenhaRecebivel", Column = "CIE_SENHA_RECEBIVEL", TypeType = typeof(string), Length = 200, NotNull = false)]
        public virtual string SenhaRecebivel { get; set; }

        [Obsolete("Mudado para configuração geral da e-frete")]
        [NHibernate.Mapping.Attributes.Property(0, Name = "CodigoIntegracaoRecebivel", Column = "CIE_CODIGO_INTEGRACAO_RECEBIVEL", TypeType = typeof(string), Length = 200, NotNull = false)]
        public virtual string CodigoIntegracaoRecebivel { get; set; }
        #endregion Obsolete

        public virtual string Descricao
        {
            get { return "Configuração Integração e-Frete"; }
        }
    }
}
