using Dominio.ObjetosDeValor.Embarcador.Consulta;
using System.Collections.Generic;
using System.Linq;

namespace Repositorio.Embarcador.Importacoes
{
    public class ImportacaoHierarquiaHistorico : RepositorioBase<Dominio.Entidades.Embarcador.Importacoes.ImportacaoHierarquiaHistorico>
    {
        public ImportacaoHierarquiaHistorico(UnitOfWork unitOfWork) : base(unitOfWork) { }

        #region Métodos Públicos

        public List<Dominio.Entidades.Embarcador.Importacoes.ImportacaoHierarquiaHistorico> Consultar(Dominio.ObjetosDeValor.Embarcador.Importacoes.FiltroPesquisaImportacaoHierarquia filtrosPesquisa, ParametroConsulta parametroConsulta)
        {
            IQueryable<Dominio.Entidades.Embarcador.Importacoes.ImportacaoHierarquiaHistorico> query = _Consultar(filtrosPesquisa);
            
            return ObterLista(query, parametroConsulta);
        }

        public int ContarConsulta(Dominio.ObjetosDeValor.Embarcador.Importacoes.FiltroPesquisaImportacaoHierarquia filtrosPesquisa)
        {
            IQueryable<Dominio.Entidades.Embarcador.Importacoes.ImportacaoHierarquiaHistorico> query = _Consultar(filtrosPesquisa);
            
            return query.Count();
        }

        #endregion

        #region Métodos Privados

        private IQueryable<Dominio.Entidades.Embarcador.Importacoes.ImportacaoHierarquiaHistorico> _Consultar(Dominio.ObjetosDeValor.Embarcador.Importacoes.FiltroPesquisaImportacaoHierarquia filtrosPesquisa)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Importacoes.ImportacaoHierarquiaHistorico>();

            if (filtrosPesquisa.DataInicial.HasValue)
                query = query.Where(obj => obj.Data.Date >= filtrosPesquisa.DataInicial.Value.Date);
            
            if (filtrosPesquisa.DataFinal.HasValue)
                query = query.Where(obj => obj.Data.Date <= filtrosPesquisa.DataFinal.Value.Date);

            return query;
        }
        
        #endregion
    }
}
