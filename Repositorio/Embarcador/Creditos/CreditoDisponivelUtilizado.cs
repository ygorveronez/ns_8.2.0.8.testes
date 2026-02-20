using System.Collections.Generic;
using System.Linq;
using NHibernate.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;

namespace Repositorio.Embarcador.Creditos
{
    public class CreditoDisponivelUtilizado : RepositorioBase<Dominio.Entidades.Embarcador.Creditos.CreditoDisponivelUtilizado>
    {
        public CreditoDisponivelUtilizado(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.Embarcador.Creditos.CreditoDisponivelUtilizado BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Creditos.CreditoDisponivelUtilizado>();
            var result = from obj in query where obj.Codigo == codigo select obj;
            return result.FirstOrDefault();
        }
        
        public List<Dominio.Entidades.Embarcador.Creditos.CreditoDisponivelUtilizado> BuscarPorCreditoExtra(int creditoExtra)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Creditos.CreditoDisponivelUtilizado>();
            var result = from obj in query where obj.CreditoExtra.Codigo == creditoExtra && obj.SituacaoCreditoUtilizado != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCreditoUtilizado.Estornado select obj;
            return result.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Creditos.CreditoDisponivelUtilizado> BuscarPorCreditoDisponivelOrigem(int creditoDisponivelOrigem)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Creditos.CreditoDisponivelUtilizado>();
            var result = from obj in query where obj.CreditoDisponivelOrigem.Codigo == creditoDisponivelOrigem && obj.SituacaoCreditoUtilizado != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCreditoUtilizado.Estornado select obj;
            return result.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Creditos.CreditoDisponivelUtilizado> BuscarTodosPorCreditoDisponivelOrigem(int creditoDisponivelOrigem)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Creditos.CreditoDisponivelUtilizado>();
            var result = from obj in query where obj.CreditoDisponivelOrigem.Codigo == creditoDisponivelOrigem select obj;
            return result.ToList();
        }


        public List<Dominio.Entidades.Embarcador.Creditos.CreditoDisponivelUtilizado> BuscarPorCreditoDisponivelDestino(int creditoDisponivelDestino)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Creditos.CreditoDisponivelUtilizado>();
            var result = from obj in query where obj.CreditoDisponivelDestino.Codigo == creditoDisponivelDestino && obj.SituacaoCreditoUtilizado != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCreditoUtilizado.Estornado select obj;
            return result
                .Fetch(obj => obj.CreditoDisponivelOrigem)
                .ThenFetch(obj => obj.Creditor)
                .ToList();
        }

        public List<Dominio.Entidades.Embarcador.Creditos.CreditoDisponivelUtilizado> BuscarPorCreditoDisponivelDestinoTodos(int creditoDisponivelDestino)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Creditos.CreditoDisponivelUtilizado>();
            var result = from obj in query where obj.CreditoDisponivelDestino.Codigo == creditoDisponivelDestino select obj;
            return result.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Creditos.CreditoDisponivelUtilizado> BuscarPorCreditoComplementoDeFrete(int complementoDeFrete)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Creditos.CreditoDisponivelUtilizado>();
            var result = from obj in query where obj.CargaComplementoFrete.Codigo == complementoDeFrete && obj.SituacaoCreditoUtilizado != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCreditoUtilizado.Estornado select obj;
            return result.ToList();
        }
        public async Task<List<Dominio.Entidades.Embarcador.Creditos.CreditoDisponivelUtilizado>> BuscarPorCreditoComplementoDeFreteAsync(int complementoDeFrete)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Creditos.CreditoDisponivelUtilizado>();
            var result = from obj in query where obj.CargaComplementoFrete.Codigo == complementoDeFrete && obj.SituacaoCreditoUtilizado != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCreditoUtilizado.Estornado select obj;
            return await result.ToListAsync();
        }

        public List<Dominio.Entidades.Embarcador.Creditos.CreditoDisponivelUtilizado> BuscarPorCreditoSolicitacaoCredito(int solicitacaoCredito)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Creditos.CreditoDisponivelUtilizado>();
            var result = from obj in query where obj.SolicitacaoCredito.Codigo == solicitacaoCredito && obj.SituacaoCreditoUtilizado != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCreditoUtilizado.Estornado select obj;
            return result.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Creditos.CreditoDisponivelUtilizado> BuscarPorOcorrencia(int ocorrencia)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Creditos.CreditoDisponivelUtilizado>();
            var result = from obj in query where obj.CargaOcorrencia.Codigo == ocorrencia && obj.SituacaoCreditoUtilizado != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCreditoUtilizado.Estornado select obj;
            return result.ToList();
        }
        
    }
}
