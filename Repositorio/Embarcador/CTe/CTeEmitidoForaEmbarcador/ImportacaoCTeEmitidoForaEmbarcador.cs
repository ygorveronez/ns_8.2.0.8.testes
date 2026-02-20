using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using NHibernate.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.CTe
{
    public class ImportacaoCTeEmitidoForaEmbarcador : RepositorioBase<Dominio.Entidades.Embarcador.CTe.ImportacaoCTeEmitidoForaEmbarcador>
    {
        #region Construtores

        public ImportacaoCTeEmitidoForaEmbarcador(UnitOfWork unitOfWork) : base(unitOfWork) { }

        #endregion

        #region Métodos Privados

        private IQueryable<Dominio.Entidades.Embarcador.CTe.ImportacaoCTeEmitidoForaEmbarcador> Consultar(Dominio.ObjetosDeValor.Embarcador.CTe.FiltroPesquisaImportacaoCTeEmitidoForaEmbarcador filtrosPesquisa)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.CTe.ImportacaoCTeEmitidoForaEmbarcador>();

            if (filtrosPesquisa.CodigoUsuario > 0)
                query = query.Where(o => o.Usuario.Codigo == filtrosPesquisa.CodigoUsuario);

            if (!string.IsNullOrWhiteSpace(filtrosPesquisa.Planilha))
                query = query.Where(o => o.Planilha.Contains(filtrosPesquisa.Planilha));

            if (filtrosPesquisa.DataImportacaoInicial.HasValue)
                query = query.Where(o => o.DataImportacao >= filtrosPesquisa.DataImportacaoInicial.Value.Date);

            if (filtrosPesquisa.DataImportacaoFinal.HasValue)
                query = query.Where(o => o.DataImportacao <= filtrosPesquisa.DataImportacaoFinal.Value.Date.Add(DateTime.MaxValue.TimeOfDay));

            if (filtrosPesquisa.Situacao != SituacaoImportacaoCTeEmitidoForaEmbarcador.Todas)
                query = query.Where(o => o.Situacao == filtrosPesquisa.Situacao);

            if (!string.IsNullOrWhiteSpace(filtrosPesquisa.Mensagem))
                query = query.Where(o => o.Mensagem.Contains(filtrosPesquisa.Mensagem));

            return query;
        }

        #endregion

        #region Métodos Públicos

        public Dominio.Entidades.Embarcador.CTe.ImportacaoCTeEmitidoForaEmbarcador BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.CTe.ImportacaoCTeEmitidoForaEmbarcador>();
            var result = from obj in query where obj.Codigo == codigo select obj;
            return result.FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.CTe.ImportacaoCTeEmitidoForaEmbarcador BuscarProximaImportacaoPendente()
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.CTe.ImportacaoCTeEmitidoForaEmbarcador>();
            var result = from obj in query where obj.Situacao == SituacaoImportacaoCTeEmitidoForaEmbarcador.Pendente || obj.Situacao == SituacaoImportacaoCTeEmitidoForaEmbarcador.Processando select obj;
            return result.OrderBy(obj => obj.DataImportacao).FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.CTe.ImportacaoCTeEmitidoForaEmbarcador> Consultar(Dominio.ObjetosDeValor.Embarcador.CTe.FiltroPesquisaImportacaoCTeEmitidoForaEmbarcador filtrosPesquisa, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            var query = Consultar(filtrosPesquisa);

            query = query.OrderBy($"{parametrosConsulta.PropriedadeOrdenar} {(parametrosConsulta.DirecaoOrdenar == "asc" ? "ascending" : "descending")}");

            if (parametrosConsulta.LimiteRegistros > 0)
                query = query.Skip(parametrosConsulta.InicioRegistros).Take(parametrosConsulta.LimiteRegistros);

            return query
                .Fetch(o => o.Usuario)
                .WithOptions(o => { o.SetTimeout(300); })
                .ToList();
        }

        public int ContarConsulta(Dominio.ObjetosDeValor.Embarcador.CTe.FiltroPesquisaImportacaoCTeEmitidoForaEmbarcador filtrosPesquisa)
        {
            var query = Consultar(filtrosPesquisa);
            return query.WithOptions(o => { o.SetTimeout(300); }).Count();
        }

        #endregion
    }
}
