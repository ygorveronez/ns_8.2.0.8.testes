using System.Collections.Generic;
using System.Linq;
using NHibernate.Linq;
using System.Linq.Dynamic.Core;



namespace Repositorio.Embarcador.Cargas.MontagemCarga
{
    public class CarregamentoCarga : RepositorioBase<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoCarga>
    {
        public CarregamentoCarga(UnitOfWork unitOfWork) : base(unitOfWork) { }


        public List<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoCarga> BuscarPorCarregamento(int carregamento)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoCarga>();

            var result = from obj in query where obj.Carregamento.Codigo == carregamento select obj;

            return result
                .Fetch(obj => obj.Carregamento)
                .Fetch(obj => obj.Carga)
                .ThenFetch(obj => obj.DadosSumarizados)
                .Fetch(obj => obj.Carga)
                .ThenFetch(obj => obj.TipoOperacao)
                .ToList();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoCarga> BuscarPorCargas(List<int> codigosCarga)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoCarga>();

            var result = from obj in query where codigosCarga.Contains(obj.Codigo) select obj;

            return result
                .Fetch(obj => obj.Carregamento)
                .Fetch(obj => obj.Carga)
                .ThenFetch(obj => obj.DadosSumarizados)
                .Fetch(obj => obj.Carga)
                .ThenFetch(obj => obj.TipoOperacao)
                .ToList();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoCarga> BuscarPorNumeroCargaEmbarcador(List<string> codigosCargaEmbarcador)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoCarga>();
            var result = from obj in query where codigosCargaEmbarcador.Contains(obj.Carga.CodigoCargaEmbarcador) select obj;
            return result.ToList();
        }
        public int ContarPorCarregamento(int carregamento)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoCarga>();

            var result = from obj in query where obj.Carregamento.Codigo == carregamento select obj;

            return result.Count();
        }

    }
}
