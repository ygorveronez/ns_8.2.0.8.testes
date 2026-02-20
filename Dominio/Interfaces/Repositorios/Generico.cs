namespace Dominio.Interfaces.Repositorios
{
    public interface Generico<T>
    {
        long Inserir(T objeto, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado Auditado = null, Dominio.Entidades.Auditoria.HistoricoObjeto historioPai = null, string descricaoAcao = "");

        Dominio.Entidades.Auditoria.HistoricoObjeto Atualizar(T objeto, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado Auditado = null, Dominio.Entidades.Auditoria.HistoricoObjeto historioPai = null, string descricaoAcao = "");

        void Deletar(T objeto, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado Auditado = null, Dominio.Entidades.Auditoria.HistoricoObjeto historioPai = null);
    }
}
