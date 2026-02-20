using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.Fechamento
{
    public class FechamentoFreteCTeIntegracao : RepositorioBase<Dominio.Entidades.Embarcador.Fechamento.FechamentoFreteCTeIntegracao>
    {

        public FechamentoFreteCTeIntegracao(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.Embarcador.Fechamento.FechamentoFreteCTeIntegracao BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Fechamento.FechamentoFreteCTeIntegracao>();
            var result = from obj in query where obj.Codigo == codigo select obj;
            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Fechamento.FechamentoFreteCTeIntegracao> BuscarCTeIntegracaoPendente(int tentativasLimite, double tempoProximaTentativaMinutos)
        {
            var consultaFechamentoFreteCTeIntegracao = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Fechamento.FechamentoFreteCTeIntegracao>()
                .Where(o =>
                    o.SituacaoIntegracao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.AgIntegracao ||
                    (
                        o.SituacaoIntegracao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao &&
                        o.NumeroTentativas < tentativasLimite &&
                        o.DataIntegracao <= DateTime.Now.AddMinutes(-tempoProximaTentativaMinutos)
                    )
                );

            return consultaFechamentoFreteCTeIntegracao.ToList();
        }


        public int ContarPorFechamento(int codigoFechamento, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao situacao)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Fechamento.FechamentoFreteCTeIntegracao>();

            var result = from obj in query where obj.FechamentoFrete.Codigo == codigoFechamento && obj.SituacaoIntegracao == situacao select obj.Codigo;

            return result.Count();
        }

        private IQueryable<Dominio.Entidades.Embarcador.Fechamento.FechamentoFreteCTeIntegracao> _Consultar(int fechamentoFrete, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao? situacao)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Fechamento.FechamentoFreteCTeIntegracao>();

            var result = from obj in query where obj.FechamentoFrete.Codigo == fechamentoFrete select obj;

            // Filtros

            return result;
        }

        public List<Dominio.Entidades.Embarcador.Fechamento.FechamentoFreteCTeIntegracao> Consultar(int fechamentoFrete, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao? situacao, string propOrdena, string dirOrdena, int inicioRegistros, int maximoRegistros)
        {
            var result = _Consultar(fechamentoFrete, situacao);

            if (!string.IsNullOrWhiteSpace(propOrdena))
                result = result.OrderBy(propOrdena + (dirOrdena == "asc" ? " ascending" : " descending"));

            if (inicioRegistros > 0)
                result = result.Skip(inicioRegistros);

            if (maximoRegistros > 0)
                result = result.Take(maximoRegistros);

            return result.ToList();
        }

        public int ContarConsulta(int fechamentoFrete, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao? situacao)
        {
            var result = _Consultar(fechamentoFrete, situacao);

            return result.Count();
        }
    }
}
