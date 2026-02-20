using System.Linq;

namespace Repositorio.Embarcador.Cargas
{
    public class CargaJanelaCarregamentoViagem : RepositorioBase<Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoViagem>
	{

		public CargaJanelaCarregamentoViagem(UnitOfWork unitOfWork) :base(unitOfWork) { }

		public Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoViagem BuscarPorNumeroViagem(string numeroViagem)
		{
			var consultaJanelaCarregamentoViagem = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoViagem>()
				.Where(o => o.NumeroViagem == numeroViagem);

			return consultaJanelaCarregamentoViagem.FirstOrDefault();
		}

		public Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoViagem BuscarPorJanelaCarregamento(int codigoCargaJanelaCarregamento)
		{
			var consultaJanelaCarregamentoViagem = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoViagem>()
				.Where(v => v.CargasJanelaCarregamento.Any(j => j.Codigo == codigoCargaJanelaCarregamento) && v.StatusLeilao != Dominio.ObjetosDeValor.Embarcador.Enumeradores.StatusLeilaoIntegracaoJanelaCarregamento.Cancelado);

			return consultaJanelaCarregamentoViagem.FirstOrDefault();
		}
	}
}
