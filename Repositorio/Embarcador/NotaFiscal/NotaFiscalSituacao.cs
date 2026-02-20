using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System.Collections.Generic;
using System.Linq;

namespace Repositorio.Embarcador.NotaFiscal
{
    public class NotaFiscalSituacao : RepositorioBase<Dominio.Entidades.Embarcador.NotaFiscal.NotaFiscalSituacao>
    {
        public NotaFiscalSituacao(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public List<Dominio.Entidades.Embarcador.NotaFiscal.NotaFiscalSituacao> Consultar(Dominio.ObjetosDeValor.Embarcador.NotaFiscal.FiltroPesquisaNotaFiscalSituacao filtrosPesquisa, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            var query = Consultar(filtrosPesquisa);
            
            return ObterLista(query, parametrosConsulta);
        }

        public int ContarConsulta(Dominio.ObjetosDeValor.Embarcador.NotaFiscal.FiltroPesquisaNotaFiscalSituacao filtrosPesquisa)
        {
            var query = Consultar(filtrosPesquisa);

            return query.Count();
        }
        
        public bool VerificarExistenciaRegistroAtivoGatilho(Dominio.Entidades.Embarcador.NotaFiscal.NotaFiscalSituacao notaFiscalSituacao)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.NotaFiscal.NotaFiscalSituacao>();
            
            return query.Where(obj => obj.Ativo && obj.NotaFiscalSituacaoGatilho == notaFiscalSituacao.NotaFiscalSituacaoGatilho && obj.Codigo != notaFiscalSituacao.Codigo).Count() > 0;
        }

        public Dominio.Entidades.Embarcador.NotaFiscal.NotaFiscalSituacao BuscarPorGatilho(NotaFiscalSituacaoGatilho gatilho)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.NotaFiscal.NotaFiscalSituacao>()
                .Where(obj => obj.Ativo && obj.NotaFiscalSituacaoGatilho == gatilho);
            
            return query.FirstOrDefault();
        }

        private IQueryable<Dominio.Entidades.Embarcador.NotaFiscal.NotaFiscalSituacao> Consultar(Dominio.ObjetosDeValor.Embarcador.NotaFiscal.FiltroPesquisaNotaFiscalSituacao filtrosPesquisa)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.NotaFiscal.NotaFiscalSituacao>();

            if (filtrosPesquisa.SituacaoAtivo.HasValue)
                query = query.Where(obj => obj.Ativo == filtrosPesquisa.SituacaoAtivo);

            if (!string.IsNullOrWhiteSpace(filtrosPesquisa.Descricao))
                query = query.Where(obj => obj.Descricao.Contains(filtrosPesquisa.Descricao));
            
            return query;
        }
    }
}
