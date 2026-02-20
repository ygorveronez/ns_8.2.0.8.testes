using Dominio.Interfaces.Database;
using System.Threading;

namespace Servicos.Embarcador.Financeiro
{
    public class BoletoGeracao : ServicoBase
    {
        public BoletoGeracao(Repositorio.UnitOfWork unitOfWork, CancellationToken cancelationToken = default) : base(unitOfWork, cancelationToken) { }
    }
}
