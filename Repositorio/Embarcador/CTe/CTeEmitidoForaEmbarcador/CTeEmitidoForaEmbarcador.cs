using System.Linq;

namespace Repositorio.Embarcador.CTe
{
    public class CTeEmitidoForaEmbarcador : RepositorioBase<Dominio.Entidades.Embarcador.CTe.CTeEmitidoForaEmbarcador>
    {
        public CTeEmitidoForaEmbarcador(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.Embarcador.CTe.CTeEmitidoForaEmbarcador BuscarPorNumero(int numero, int serie, int codigoEmpresa, string Chave)
        {
            var consultaCTeEmitidoForaEmbarcador = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.CTe.CTeEmitidoForaEmbarcador>();

            if (numero > 0)
                consultaCTeEmitidoForaEmbarcador = consultaCTeEmitidoForaEmbarcador.Where(obj => obj.Numero == numero);

            if (serie > 0)
                consultaCTeEmitidoForaEmbarcador = consultaCTeEmitidoForaEmbarcador.Where(obj => obj.Serie == serie);

            if (codigoEmpresa > 0)
                consultaCTeEmitidoForaEmbarcador = consultaCTeEmitidoForaEmbarcador.Where(obj => obj.Empresa.Codigo == codigoEmpresa);

            if (!string.IsNullOrWhiteSpace(Chave))
                consultaCTeEmitidoForaEmbarcador = consultaCTeEmitidoForaEmbarcador.Where(obj => obj.Chave == Chave);

            return consultaCTeEmitidoForaEmbarcador
                .OrderByDescending(o => o.Codigo)
                .FirstOrDefault();
        }
    }
}
