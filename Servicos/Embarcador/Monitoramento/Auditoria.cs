namespace Servicos.Embarcador.Monitoramento
{
    public class Auditoria
    {
        #region Métodos públicos estáticos 

        public static void AuditarMonitoramento(Dominio.Entidades.Embarcador.Logistica.Monitoramento monitoramento, string descricao, Repositorio.UnitOfWork unitOfWork)
        {
            AuditarMonitoramento(monitoramento, descricao, null, unitOfWork);
        }

        public static void AuditarMonitoramento(Dominio.Entidades.Embarcador.Logistica.Monitoramento monitoramento, string descricao, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditoriaBase, Repositorio.UnitOfWork unitOfWork)
        {
            if (!monitoramento.IsInitialized()) monitoramento.Initialize();
            Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado = new Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado();
            if (auditoriaBase == null)
            {
                auditado.TipoAuditado = Dominio.ObjetosDeValor.Enumerador.TipoAuditado.Sistema;
                auditado.OrigemAuditado = Dominio.ObjetosDeValor.Enumerador.OrigemAuditado.Sistema;
            }
            else
            {
                auditado.TipoAuditado = auditoriaBase.TipoAuditado;
                auditado.OrigemAuditado = auditoriaBase.OrigemAuditado;
                auditado.Usuario = auditoriaBase.Usuario;
            }

            Servicos.Auditoria.Auditoria.Auditar(auditado, monitoramento, monitoramento.GetChanges(), descricao, unitOfWork);
        }

        #endregion

    }

}
