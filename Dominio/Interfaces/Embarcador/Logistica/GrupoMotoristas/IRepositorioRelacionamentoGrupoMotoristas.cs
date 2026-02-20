using System.Threading.Tasks;

namespace Dominio.Interfaces.Embarcador.Logistica.GrupoMotoristas
{
    public interface IRepositorioRelacionamentoGrupoMotoristas
    {
        Task DeletarPorCodigoAsync(int codigo, ObjetosDeValor.Embarcador.Auditoria.Auditado auditado);

        string ObterNomeVerbosoDaEntidade();
    }
}
