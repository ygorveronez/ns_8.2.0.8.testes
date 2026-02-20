using Dominio.ObjetosDeValor.Embarcador.Consulta;
using Dominio.ObjetosDeValor.Embarcador.Ocorrencia;
using System.Collections.Generic;
using System.Linq;

namespace Repositorio.Embarcador.Ocorrencias
{
    public class GrupoTipoDeOcorrenciaDeCTe : RepositorioBase<Dominio.Entidades.Embarcador.Ocorrencias.GrupoTipoDeOcorrenciaDeCTe>
    {
        public GrupoTipoDeOcorrenciaDeCTe(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public List<Dominio.Entidades.Embarcador.Ocorrencias.GrupoTipoDeOcorrenciaDeCTe> Consultar(FiltroPesquisaGrupoTipoDeOcorrenciaDeCTe filtrosPesquisa, ParametroConsulta parametrosConsulta)
        {
            IQueryable<Dominio.Entidades.Embarcador.Ocorrencias.GrupoTipoDeOcorrenciaDeCTe> query = Consultar(filtrosPesquisa);

            return ObterLista(query, parametrosConsulta);
        }
        
        public int ContarConsulta(FiltroPesquisaGrupoTipoDeOcorrenciaDeCTe filtrosPesquisa)
        {
            IQueryable<Dominio.Entidades.Embarcador.Ocorrencias.GrupoTipoDeOcorrenciaDeCTe> query = Consultar(filtrosPesquisa);

            return query.Count();
        }

        private IQueryable<Dominio.Entidades.Embarcador.Ocorrencias.GrupoTipoDeOcorrenciaDeCTe> Consultar(FiltroPesquisaGrupoTipoDeOcorrenciaDeCTe filtrosPesquisa)
        {
            IQueryable<Dominio.Entidades.Embarcador.Ocorrencias.GrupoTipoDeOcorrenciaDeCTe> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Ocorrencias.GrupoTipoDeOcorrenciaDeCTe>();

            if (!string.IsNullOrWhiteSpace(filtrosPesquisa.Descricao))
                query = query.Where(obj => obj.Descricao == filtrosPesquisa.Descricao);

            if (!string.IsNullOrWhiteSpace(filtrosPesquisa.CodigoIntegracao))
                query = query.Where(obj => obj.CodigoIntegracao == filtrosPesquisa.CodigoIntegracao);

            if (filtrosPesquisa.SituacaoAtivo != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivaPesquisa.Todos)
            {
                if (filtrosPesquisa.SituacaoAtivo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivaPesquisa.Ativa)
                    query = query.Where(obj => obj.Ativo);
                else
                    query = query.Where(obj => !obj.Ativo);
            }

            return query;
        }
    }
}
