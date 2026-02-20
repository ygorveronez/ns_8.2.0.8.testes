using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.AbastecimentoInterno
{
    public class LiberacaoAbastecimentoAutomatizado : RepositorioBase<Dominio.Entidades.Embarcador.AbastecimentoInterno.LiberacaoAbastecimentoAutomatizado>
    {
        public LiberacaoAbastecimentoAutomatizado(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public List<Dominio.Entidades.Embarcador.AbastecimentoInterno.LiberacaoAbastecimentoAutomatizado> Consultar(string descricao, int codigoVeiculo, int motorista, int codigoBomba, string propOrdenacao, string dirOrdenacao, int inicioRegistros, int maximoRegistros, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracaoAbastecimento? situacao)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.AbastecimentoInterno.LiberacaoAbastecimentoAutomatizado>();

            var result = from obj in query select obj;

            if (codigoVeiculo > 0)
                result = result.Where(obj => obj.Veiculo.Codigo == codigoVeiculo);

            if (motorista > 0)
                result = result.Where(obj => obj.Motorista.Codigo == motorista);

            if (codigoBomba > 0)
                result = result.Where(obj => obj.BombaAbastecimento.Codigo == codigoBomba);

            if (situacao.HasValue)
                result = result.Where(o => o.SituacaoAbastecimento == situacao.Value);

            return result.OrderBy(propOrdenacao + (dirOrdenacao == "asc" ? " ascending" : " descending")).Skip(inicioRegistros).Take(maximoRegistros).Distinct().ToList();
        }

        public int ContarConsulta(string descricao, int codigoVeiculo, int motorista, int codigoBomba, string propOrdenacao, string dirOrdenacao, int inicioRegistros, int maximoRegistros, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracaoAbastecimento? situacao)
        {
            List<Dominio.Entidades.Embarcador.AbastecimentoInterno.LiberacaoAbastecimentoAutomatizado> result = Consultar(descricao, codigoVeiculo, motorista, codigoBomba, propOrdenacao, dirOrdenacao, inicioRegistros, maximoRegistros, situacao);

            return result.Count();
        }

        public List<Dominio.Entidades.Embarcador.AbastecimentoInterno.LiberacaoAbastecimentoAutomatizado> BuscarLiberacoesPendentes()
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.AbastecimentoInterno.LiberacaoAbastecimentoAutomatizado>();

            var result = from obj in query where obj.SituacaoAbastecimento == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracaoAbastecimento.Pendente select obj;

            return result.OrderByDescending(x => x.DataHoraUltimaExecucao).ToList();
        }

        public bool ConsultarDisponibilidadeDaBomba(long codigoBombaAbastecimento)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.AbastecimentoInterno.LiberacaoAbastecimentoAutomatizado>()
                .Where(liberacao => liberacao.BombaAbastecimento.Codigo == codigoBombaAbastecimento && liberacao.SituacaoAbastecimento != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracaoAbastecimento.Finalizado);

            if (query.Count() > 0)
                return false;

            return true;
        }
    }
}