using System.Threading.Tasks;

namespace Dominio.Interfaces.Embarcador.Cargas.Ofertas
{
    public interface IRepositorioRelacionamentoParametrosOfertas
    {
        Task DeletarPorCodigoAsync(int codigo);
    }
}
