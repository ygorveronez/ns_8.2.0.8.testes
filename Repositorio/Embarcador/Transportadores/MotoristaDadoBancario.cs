using System.Linq;

namespace Repositorio.Embarcador.Transportadores
{
    public class MotoristaDadoBancario : RepositorioBase<Dominio.Entidades.Embarcador.Transportadores.MotoristaDadoBancario>
    {
        public MotoristaDadoBancario(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.Embarcador.Transportadores.MotoristaDadoBancario BuscarPorBancoAgenciaConta(int codigoBanco, string agencia, string conta, int codigoFuncionario)
        {
            var consultaMotoristaDadoBancario = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Transportadores.MotoristaDadoBancario>()
              .Where(o => o.Banco.Codigo.Equals(codigoBanco) && o.Agencia.Equals(agencia) && o.NumeroConta.Equals(conta) && o.Motorista.Codigo.Equals(codigoFuncionario));

            return consultaMotoristaDadoBancario.FirstOrDefault();
        }
    }
}
