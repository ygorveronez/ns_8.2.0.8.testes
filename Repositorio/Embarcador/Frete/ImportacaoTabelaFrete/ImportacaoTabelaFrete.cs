using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using NHibernate.Linq;

namespace Repositorio.Embarcador.Frete.ImportacaoTabelaFrete
{
    public class ImportacaoTabelaFrete : RepositorioBase<Dominio.Entidades.Embarcador.Frete.ImportacaoTabelaFrete.ImportacaoTabelaFrete>
    {
        public ImportacaoTabelaFrete(UnitOfWork unitOfWork) : base(unitOfWork) { }

        #region Métodos Públicos 

        public List<Dominio.Entidades.Embarcador.Frete.ImportacaoTabelaFrete.ImportacaoTabelaFrete> Consultar(Dominio.ObjetosDeValor.Embarcador.Frete.FiltroPesquisaImportacaoTabelaFrete filtrosPesquisa, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            return ObterQueryConsulta(filtrosPesquisa, parametrosConsulta).Fetch(o => o.Usuario).ToList();
        }

        public int ContarConsulta(Dominio.ObjetosDeValor.Embarcador.Frete.FiltroPesquisaImportacaoTabelaFrete filtrosPesquisa)
        {
            return ObterQueryConsulta(filtrosPesquisa, null).Count();
        }

        public List<Dominio.Entidades.Embarcador.Frete.ImportacaoTabelaFrete.ImportacaoTabelaFrete> BuscarPlanilhasProcessando()
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frete.ImportacaoTabelaFrete.ImportacaoTabelaFrete>();
            var result = from obj in query where obj.Situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.Frete.SituacaoImportacaoTabelaFrete.Processando select obj;
            return result.OrderBy(obj => obj.DataImportacao).ToList();
        }

        public List<Dominio.Entidades.Embarcador.Frete.ImportacaoTabelaFrete.ImportacaoTabelaFrete> BuscarPlanilhasImportacaoPendente()
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frete.ImportacaoTabelaFrete.ImportacaoTabelaFrete>();
            var result = from obj in query where obj.Situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.Frete.SituacaoImportacaoTabelaFrete.Pendente select obj;
            return result.OrderBy(obj => obj.DataImportacao).ToList();
        }

        public bool VerificarUsuarioCancelouImportacaoEmAndamento(int codigoImportacao)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frete.ImportacaoTabelaFrete.ImportacaoTabelaFrete>();
            var result = from obj in query where obj.Codigo == codigoImportacao && obj.Situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.Frete.SituacaoImportacaoTabelaFrete.Cancelado select obj;
            return result.Count() > 0;
        }

        #endregion

        #region Métodos Privados

        public IQueryable<Dominio.Entidades.Embarcador.Frete.ImportacaoTabelaFrete.ImportacaoTabelaFrete> ObterQueryConsulta(Dominio.ObjetosDeValor.Embarcador.Frete.FiltroPesquisaImportacaoTabelaFrete filtrosPesquisa, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            IQueryable<Dominio.Entidades.Embarcador.Frete.ImportacaoTabelaFrete.ImportacaoTabelaFrete> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frete.ImportacaoTabelaFrete.ImportacaoTabelaFrete>();

            if (filtrosPesquisa.CodigoTabelaFrete > 0)
                query = query.Where(o => o.TabelaFrete.Codigo == filtrosPesquisa.CodigoTabelaFrete);

            if (filtrosPesquisa.CodigoUsuario > 0)
                query = query.Where(o => o.Usuario.Codigo == filtrosPesquisa.CodigoUsuario);

            if (filtrosPesquisa.DataImportacaoFinal.HasValue)
                query = query.Where(o => o.DataImportacao < filtrosPesquisa.DataImportacaoFinal.Value.AddDays(1).Date);

            if (filtrosPesquisa.DataImportacaoInicial.HasValue)
                query = query.Where(o => o.DataImportacao >= filtrosPesquisa.DataImportacaoInicial.Value.Date);

            if (!string.IsNullOrWhiteSpace(filtrosPesquisa.NomeArquivo))
                query = query.Where(o => o.NomeArquivo.Contains(filtrosPesquisa.NomeArquivo));

            if (filtrosPesquisa.Situacao.HasValue && filtrosPesquisa.Situacao != Dominio.ObjetosDeValor.Embarcador.Enumeradores.Frete.SituacaoImportacaoTabelaFrete.Todas)
                query = query.Where(o => o.Situacao == filtrosPesquisa.Situacao);

            if (parametrosConsulta != null)
            {
                if (!string.IsNullOrWhiteSpace(parametrosConsulta.DirecaoOrdenar) && !string.IsNullOrWhiteSpace(parametrosConsulta.PropriedadeOrdenar))
                    query = query.OrderBy(parametrosConsulta.PropriedadeOrdenar + " " + parametrosConsulta.DirecaoOrdenar);

                if (parametrosConsulta.InicioRegistros > 0 || parametrosConsulta.LimiteRegistros > 0)
                    query = query.Skip(parametrosConsulta.InicioRegistros).Take(parametrosConsulta.LimiteRegistros);
            }

            if (!string.IsNullOrWhiteSpace(filtrosPesquisa.Mensagem))
                query = query.Where(o => o.Mensagem.Contains(filtrosPesquisa.Mensagem));

            return query;
        }

        #endregion 
    }
}
