namespace Dominio.Entidades.Embarcador.Configuracoes
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_CONFIGURACAO_TECNORISK", EntityName = "IntegracaoTecnorisk", Name = "Dominio.Entidades.Embarcador.Configuracoes.IntegracaoTecnorisk", NameType = typeof(IntegracaoTecnorisk))]
    public class IntegracaoTecnorisk : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "CIT_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CIT_POSSUI_INTEGRACAO_TECNORISK", TypeType = typeof(bool), NotNull = false)]
        public virtual bool PossuiIntegracaoTecnorisk { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "URLIntegracaoTecnorisk", Column = "CIL_URL_TECNORISK", TypeType = typeof(string), Length = 300, NotNull = true)]
        public virtual string URLIntegracaoTecnorisk { get; set; }     
        
        [NHibernate.Mapping.Attributes.Property(0, Name = "UsuarioTecnorisk", Column = "CIT_USUARIO_TECNORISK", TypeType = typeof(string), Length = 150, NotNull = true)]
        public virtual string UsuarioTecnorisk { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "SenhaTecnorisk", Column = "CIT_SENHA_TECNORISK", TypeType = typeof(string), Length = 150, NotNull = true)]
        public virtual string SenhaTecnorisk { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "IDPGR", Column = "CIT_ID_PGR_TECNORISK", TypeType = typeof(int), NotNull = true)]
        public virtual int IDPGR { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "IDPropriedadeMonitoramento", Column = "CIT_ID_PROPRIEDADE_MONITORAMENTO", TypeType = typeof(int), NotNull = true)]
        public virtual int IDPropriedadeMonitoramento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CargaMercadoria", Column = "CIT_CARGA_MERCADORIA", TypeType = typeof(int), NotNull = false)]
        public virtual int CargaMercadoria { get; set; }
    }
}
