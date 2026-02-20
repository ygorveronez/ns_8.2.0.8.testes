using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using NHibernate.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.Frota
{
    public class ImportacaoPedagio : RepositorioBase<Dominio.Entidades.Embarcador.Frota.ImportacaoPedagio>
    {
        #region Construtores

        public ImportacaoPedagio(UnitOfWork unitOfWork) : base(unitOfWork) { }

        #endregion

        #region Métodos Privados
        
        private IQueryable<Dominio.Entidades.Embarcador.Frota.ImportacaoPedagio> Consultar(Dominio.ObjetosDeValor.Embarcador.Frota.FiltroPesquisaImportacaoPedagio filtrosPesquisa)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frota.ImportacaoPedagio>();

            if (filtrosPesquisa.CodigoUsuario > 0)
                query = query.Where(o => o.Usuario.Codigo == filtrosPesquisa.CodigoUsuario);

            if (!string.IsNullOrWhiteSpace(filtrosPesquisa.Planilha))
                query = query.Where(o => o.Planilha.Contains(filtrosPesquisa.Planilha));

            if (filtrosPesquisa.DataImportacaoInicial.HasValue)
                query = query.Where(o => o.DataImportacao >= filtrosPesquisa.DataImportacaoInicial.Value.Date);

            if (filtrosPesquisa.DataImportacaoFinal.HasValue)
                query = query.Where(o => o.DataImportacao <= filtrosPesquisa.DataImportacaoFinal.Value.Date.Add(DateTime.MaxValue.TimeOfDay));

            if (filtrosPesquisa.Situacao != SituacaoImportacaoPedagio.Todas)
                query = query.Where(o => o.Situacao == filtrosPesquisa.Situacao);
            
            if (!string.IsNullOrWhiteSpace(filtrosPesquisa.Mensagem))
                query = query.Where(o => o.Mensagem.Contains(filtrosPesquisa.Mensagem));

            return query;
        }

        #endregion

        #region Métodos Públicos
        
        public Dominio.Entidades.Embarcador.Frota.ImportacaoPedagio BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frota.ImportacaoPedagio>();
            query = from obj in query where obj.Codigo == codigo select obj;
            
            return query.FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.Frota.ImportacaoPedagio BuscarProximaImportacaoPendente()
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frota.ImportacaoPedagio>();
            query = from obj in query where obj.Situacao == SituacaoImportacaoPedagio.Pendente || obj.Situacao == SituacaoImportacaoPedagio.Processando select obj;
            
            return query
                .OrderBy(obj => obj.DataImportacao)
                .FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Frota.ImportacaoPedagio> Consultar(Dominio.ObjetosDeValor.Embarcador.Frota.FiltroPesquisaImportacaoPedagio filtrosPesquisa, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            var query = Consultar(filtrosPesquisa)
                .Fetch(o => o.Usuario);

            return ObterLista(query, parametrosConsulta);
        }

        public int ContarConsulta(Dominio.ObjetosDeValor.Embarcador.Frota.FiltroPesquisaImportacaoPedagio filtrosPesquisa)
        {
            var query = Consultar(filtrosPesquisa);
            return query.WithOptions(o => { o.SetTimeout(300); }).Count();
        }

        #endregion
    }
}
