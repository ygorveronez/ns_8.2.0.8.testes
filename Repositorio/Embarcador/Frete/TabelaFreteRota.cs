using System.Collections.Generic;
using NHibernate.Linq;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading;
using System.Threading.Tasks;

namespace Repositorio.Embarcador.Frete
{
    public class TabelaFreteRota : Repositorio.RepositorioBase<Dominio.Entidades.Embarcador.Frete.TabelaFreteRota>
    {
        public TabelaFreteRota(UnitOfWork unitOfWork) : base(unitOfWork) { }
        public TabelaFreteRota(UnitOfWork unitOfWork, CancellationToken cancellationToken) : base(unitOfWork, cancellationToken) { }


        public Dominio.Entidades.Embarcador.Frete.TabelaFreteRota BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frete.TabelaFreteRota>();
            var result = from obj in query where obj.Codigo == codigo select obj;
            return result.FirstOrDefault();
        }
        public async Task<Dominio.Entidades.Embarcador.Frete.TabelaFreteRota> BuscarPorCodigoAsync(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frete.TabelaFreteRota>();
            var result = from obj in query where obj.Codigo == codigo select obj;
            return await result.FirstOrDefaultAsync();
        }

        public Dominio.Entidades.Embarcador.Frete.TabelaFreteRota BuscarPorCodigoEmbarcador(string codigoEmbarcador)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frete.TabelaFreteRota>();
            var result = from obj in query where obj.CodigoEmbarcador == codigoEmbarcador select obj;
            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Frete.TabelaFreteRota> BuscarPorOrigemDestino(int codigoTabelaFrete, int origem, int destino, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa ativo = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Ativo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frete.TabelaFreteRota>();

            var result = from obj in query select obj;

            result = result.Where(obj => obj.Origem.Codigo == origem && obj.Destino.Codigo == destino && obj.TabelaFrete.Codigo == codigoTabelaFrete);

            if (ativo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Ativo)
                result = result.Where(obj => obj.Ativo == true);

            if (ativo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Inativo)
                result = result.Where(obj => obj.Ativo == false);

            return result.ToList();
        }


        public List<Dominio.Entidades.Embarcador.Frete.TabelaFreteRota> Consultar(int codigoTabelaFrete, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa ativo, int? codigoOrigem, int? codigoDestino, int? tipoCarga, string codigoEmbarcador, string propOrdenacao, string dirOrdenacao, int inicioRegistros, int maximoRegistros)
        {

            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frete.TabelaFreteRota>();

            var result = from obj in query select obj;

            result = result.Where(obj => obj.TabelaFrete.Codigo == codigoTabelaFrete);

            if (codigoOrigem != null)
            {
                result = result.Where(obj => obj.Origem.Codigo == codigoOrigem.Value);
            }

            if (ativo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Ativo)
                result = result.Where(obj => obj.Ativo == true);

            if (ativo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Inativo)
                result = result.Where(obj => obj.Ativo == false);
            
            if(!string.IsNullOrWhiteSpace(codigoEmbarcador))
                result = result.Where(obj => obj.CodigoEmbarcador == codigoEmbarcador);

            if (codigoDestino != null)
            {
                result = result.Where(obj => obj.Destino.Codigo == codigoDestino.Value);
            }


            return result.OrderBy(propOrdenacao + (dirOrdenacao == "asc" ? " ascending" : " descending")).Skip(inicioRegistros).Take(maximoRegistros).ToList();

        }

        public int ContarConsulta(int codigoTabelaFrete, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa ativo, int? codigoOrigem, int? codigoDestino, int? tipoCarga, string codigoEmbarcador)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frete.TabelaFreteRota>();

            var result = from obj in query select obj;

            result = result.Where(obj => obj.TabelaFrete.Codigo == codigoTabelaFrete);

            if (codigoOrigem != null)
            {
                result = result.Where(obj => obj.Origem.Codigo == codigoOrigem.Value);
            }

            if (ativo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Ativo)
                result = result.Where(obj => obj.Ativo == true);

            if (ativo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Inativo)
                result = result.Where(obj => obj.Ativo == false);

            if (!string.IsNullOrWhiteSpace(codigoEmbarcador))
                result = result.Where(obj => obj.CodigoEmbarcador == codigoEmbarcador);

            if (codigoDestino != null)
            {
                result = result.Where(obj => obj.Destino.Codigo == codigoDestino.Value);
            }

            return result.Count();
        }
    }
}
