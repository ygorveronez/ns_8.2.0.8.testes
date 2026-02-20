using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
using NHibernate.Linq;



namespace Repositorio.Embarcador.Pedidos
{
    public class RegraTomador : Repositorio.RepositorioBase<Dominio.Entidades.Embarcador.Pedidos.RegraTomador>
    {
        public RegraTomador(UnitOfWork unitOfWork) : base(unitOfWork) { }


        public bool PossuiRegras()
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.RegraTomador>();
            var result = from obj in query where obj.Ativo select obj;
            return result.Any();
        }

        public Task<bool> PossuiRegrasAsync()
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.RegraTomador>();
            var result = from obj in query where obj.Ativo select obj;
            return result.AnyAsync();
        }

        public Dominio.Entidades.Embarcador.Pedidos.RegraTomador BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.RegraTomador>();
            var result = from obj in query where obj.Codigo == codigo select obj;
            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Pedidos.RegraTomador> Consultar(double tomador, double remetente, double destinatario, bool? origemFilial, bool? destinoFilial, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa ativo, string propOrdenacao, string dirOrdenacao, int inicioRegistros, int maximoRegistros)
        {
            var query = MontarConsulta(tomador, remetente, destinatario, origemFilial, destinoFilial, ativo);
            return query.OrderBy(propOrdenacao + (dirOrdenacao == "asc" ? " ascending" : " descending")).Skip(inicioRegistros).Take(maximoRegistros).ToList();

        }

        public int ContarConsulta(double tomador, double remetente, double destinatario, bool? origemFilial, bool? destinoFilial, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa ativo)
        {
            var query = MontarConsulta(tomador, remetente, destinatario, origemFilial, destinoFilial, ativo);
            return query.Count();
        }

        private IQueryable<Dominio.Entidades.Embarcador.Pedidos.RegraTomador> MontarConsulta(double tomador, double remetente, double destinatario, bool? origemFilial, bool? destinoFilial, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa ativo)
        {

            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.RegraTomador>();

            var result = from obj in query select obj;

            if (remetente > 0)
                result = result.Where(obj => obj.Remetente.CPF_CNPJ == remetente);

            if (destinatario > 0)
                result = result.Where(obj => obj.Destinatario.CPF_CNPJ == destinatario);

            if (tomador > 0)
                result = result.Where(obj => obj.Tomador.CPF_CNPJ == tomador);

            if (origemFilial.HasValue)
                result = result.Where(obj => obj.OrigemFilial == origemFilial.Value);

            if (destinoFilial.HasValue)
                result = result.Where(obj => obj.DestinoFilial == destinoFilial.Value);

            if (ativo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Ativo)
                result = result.Where(obj => obj.Ativo == true);

            if (ativo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Inativo)
                result = result.Where(obj => obj.Ativo == false);


            return result;
        }

        public List<Dominio.Entidades.Embarcador.Pedidos.RegraTomador> BuscarPorFilialNaoFilial()
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.RegraTomador>();

            var result = from obj in query select obj;

            result = result.Where(obj => obj.Remetente == null);

            result = result.Where(obj => obj.Destinatario == null);

            result = result.Where(obj => obj.Ativo);

            return result.ToList();
        }

        public Task<List<Dominio.Entidades.Embarcador.Pedidos.RegraTomador>> BuscarPorFilialNaoFilialAsync()
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.RegraTomador>();

            var result = from obj in query select obj;

            result = result.Where(obj => obj.Remetente == null);

            result = result.Where(obj => obj.Destinatario == null);

            result = result.Where(obj => obj.Ativo);

            return result.ToListAsync();
        }

        public List<Dominio.Entidades.Embarcador.Pedidos.RegraTomador> BuscarPorRemetenteDestinatario(double remetente, double destinatario)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.RegraTomador>();

            var result = from obj in query select obj;

            if (remetente > 0)
                result = result.Where(obj => obj.Remetente.CPF_CNPJ == remetente);

            if (destinatario > 0)
                result = result.Where(obj => obj.Destinatario.CPF_CNPJ == destinatario);

            result = result.Where(obj => obj.Ativo == true);

            return result.ToList();
        }

        public Task<List<Dominio.Entidades.Embarcador.Pedidos.RegraTomador>> BuscarPorRemetenteDestinatarioAsync(double remetente, double destinatario)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.RegraTomador>();

            var result = from obj in query select obj;

            if (remetente > 0)
                result = result.Where(obj => obj.Remetente.CPF_CNPJ == remetente);

            if (destinatario > 0)
                result = result.Where(obj => obj.Destinatario.CPF_CNPJ == destinatario);

            result = result.Where(obj => obj.Ativo == true);

            return result.Fetch(x => x.Destinatario).Fetch(x => x.Remetente).ToListAsync();
        }

        public async Task<List<Dominio.Entidades.Embarcador.Pedidos.RegraTomador>> BuscarPorRemetentesOuDestinatariosAsync(List<double> remetentes, List<double> destinatarios)
        {
            const int lote = 2000;
            var resultadoFinal = new List<Dominio.Entidades.Embarcador.Pedidos.RegraTomador>();
            IQueryable<Dominio.Entidades.Embarcador.Pedidos.RegraTomador> baseQuery = SessionNHiBernate
                .Query<Dominio.Entidades.Embarcador.Pedidos.RegraTomador>()
                .Where(x => x.Ativo);

            for (int i = 0; i < remetentes.Count; i += lote)
            {
                List<double> subRemetentes = remetentes.Skip(i).Take(lote).ToList();
                for (int j = 0; j < destinatarios.Count; j += lote)
                {
                    List<double> subDestinatarios = destinatarios.Skip(j).Take(lote).ToList();

                    var parcial = await baseQuery
                        .Where(x => subRemetentes.Contains(x.Remetente.CPF_CNPJ) || subDestinatarios.Contains(x.Destinatario.CPF_CNPJ))
                        .Fetch(x => x.Remetente)
                        .Fetch(x => x.Destinatario)
                        .ToListAsync();

                    resultadoFinal.AddRange(parcial);
                }
            }

            return resultadoFinal.Distinct().ToList();
        }

        public Dominio.Entidades.Embarcador.Pedidos.RegraTomador BuscarPorParametros(double remetente, double destinatario, bool origemFilial, bool destinoFilial)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.RegraTomador>();

            var result = from obj in query select obj;

            if (remetente > 0)
                result = result.Where(obj => obj.Remetente.CPF_CNPJ == remetente);

            if (destinatario > 0)
                result = result.Where(obj => obj.Destinatario.CPF_CNPJ == destinatario);

            result = result.Where(obj => obj.Ativo == true && obj.DestinoFilial == destinoFilial && obj.OrigemFilial == origemFilial);


            return result.FirstOrDefault();
        }

    }
}
