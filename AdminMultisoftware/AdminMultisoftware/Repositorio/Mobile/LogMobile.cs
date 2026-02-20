using System.Collections.Generic;
using System.Linq;
using NHibernate.Linq;
using System.Linq.Dynamic.Core;
using AdminMultisoftware.Dominio.ObjetosDeValor.Mobile;

namespace AdminMultisoftware.Repositorio.Mobile
{
    public class LogMobile : RepositorioBase<Dominio.Entidades.Mobile.LogMobile>
    {
        public LogMobile(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public List<Dominio.Entidades.Mobile.LogMobile> Consulta(FiltroPesquisaLogMobile filtrosPesquisa, string propOrdenacao, string dirOrdenacao, int inicioRegistros, int maximoRegistros)
        {
            IQueryable<Dominio.Entidades.Mobile.LogMobile> result = Consulta(filtrosPesquisa);
            result = result.Fetch(o => o.Motorista);

            if (!string.IsNullOrWhiteSpace(propOrdenacao))
                result = result.OrderBy(propOrdenacao + (dirOrdenacao == "asc" ? " ascending" : " descending"));

            if (inicioRegistros > 0)
                result = result.Skip(inicioRegistros);

            if (maximoRegistros > 0)
                result = result.Take(maximoRegistros);

            return result.ToList();
        }

        public int ContaConsulta(FiltroPesquisaLogMobile filtrosPesquisa)
        {
            IQueryable<Dominio.Entidades.Mobile.LogMobile> result = Consulta(filtrosPesquisa);
            return result.Count();
        }

        private IQueryable<Dominio.Entidades.Mobile.LogMobile> Consulta(FiltroPesquisaLogMobile filtrosPesquisa)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Mobile.LogMobile>();

            var result = from obj in query select obj;

            if (filtrosPesquisa.Motorista > 0)
                result = result.Where(obj => obj.Motorista.Codigo == filtrosPesquisa.Motorista);

            if (filtrosPesquisa.IdClienteMultisoftware > 0)
                result = result.Where(obj => obj.IdClienteMultisoftware == filtrosPesquisa.IdClienteMultisoftware);

            if (filtrosPesquisa.InicioDataRegistroApp.HasValue)
                result = result.Where(obj => obj.DataRegistroApp >= filtrosPesquisa.InicioDataRegistroApp);

            if (filtrosPesquisa.FimDataRegistroApp.HasValue)
                result = result.Where(obj => obj.DataRegistroApp <= filtrosPesquisa.FimDataRegistroApp);

            if (filtrosPesquisa.InicioDataCriacao.HasValue)
                result = result.Where(obj => obj.DataCriacao >= filtrosPesquisa.InicioDataCriacao);

            if (filtrosPesquisa.FimDataCriacao.HasValue)
                result = result.Where(obj => obj.DataCriacao <= filtrosPesquisa.FimDataCriacao);

            if (!string.IsNullOrEmpty(filtrosPesquisa.Mensagem))
                result = result.Where(obj => obj.Mensagem.ToLower().Contains(filtrosPesquisa.Mensagem.ToLower()));

            if (filtrosPesquisa.Erro.HasValue)
                result = result.Where(obj => obj.Erro == filtrosPesquisa.Erro);

            return result;
        }

    }
}
