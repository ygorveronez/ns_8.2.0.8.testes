using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using NHibernate.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.Logistica
{
    public class AssociacaoBalsa : RepositorioBase<Dominio.Entidades.Embarcador.Logistica.AssociacaoBalsa>
    {
        #region Construtores

        public AssociacaoBalsa(UnitOfWork unitOfWork) : base(unitOfWork) { }

        #endregion

        #region Métodos Privados

        private IQueryable<Dominio.Entidades.Embarcador.Logistica.AssociacaoBalsa> Consultar(Dominio.ObjetosDeValor.Embarcador.Logistica.FiltroPesquisaAssociacaoBalsa filtrosPesquisa)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.AssociacaoBalsa>();

            if (filtrosPesquisa.EntidadePai.HasValue)
                query = query.Where(o => o.EntidadePai == filtrosPesquisa.EntidadePai.Value);
            else
                query = query.Where(o => !o.EntidadePai);

            if (filtrosPesquisa.CodigoUsuario > 0)
                query = query.Where(o => o.Usuario.Codigo == filtrosPesquisa.CodigoUsuario);

            if (!string.IsNullOrWhiteSpace(filtrosPesquisa.Planilha))
                query = query.Where(o => o.Planilha.Contains(filtrosPesquisa.Planilha));

            if (filtrosPesquisa.DataImportacaoInicial.HasValue)
                query = query.Where(o => o.DataImportacao >= filtrosPesquisa.DataImportacaoInicial.Value.Date);

            if (filtrosPesquisa.DataImportacaoFinal.HasValue)
                query = query.Where(o => o.DataImportacao <= filtrosPesquisa.DataImportacaoFinal.Value.Date.Add(DateTime.MaxValue.TimeOfDay));

            if (filtrosPesquisa.Situacao != SituacaoImportacaoPedido.Todas)
                query = query.Where(o => o.Situacao == filtrosPesquisa.Situacao);

            if (!string.IsNullOrWhiteSpace(filtrosPesquisa.Mensagem))
                query = query.Where(o => o.Mensagem.Contains(filtrosPesquisa.Mensagem));

            if (!string.IsNullOrWhiteSpace(filtrosPesquisa.Booking))
                query = query.Where(o => o.AssociacaoBalsas.Any(a => a.Booking.Contains(filtrosPesquisa.Booking)));

            return query;
        }

        #endregion

        #region Métodos Públicos

        public Dominio.Entidades.Embarcador.Logistica.AssociacaoBalsa BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.AssociacaoBalsa>();
            var result = from obj in query where obj.Codigo == codigo select obj;
            return result.Fetch(c => c.Usuario)
                         .FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.Logistica.AssociacaoBalsa BuscarProximaImportacaoPendente()
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.AssociacaoBalsa>();
            var result = from obj in query where obj.EntidadePai && (obj.Situacao == SituacaoImportacaoPedido.Pendente || obj.Situacao == SituacaoImportacaoPedido.Processando) select obj;
            return result.Fetch(c => c.Usuario).OrderByDescending(obj => obj.DataImportacao).FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Logistica.AssociacaoBalsa> Consultar(Dominio.ObjetosDeValor.Embarcador.Logistica.FiltroPesquisaAssociacaoBalsa filtrosPesquisa, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
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

        public int ContarConsulta(Dominio.ObjetosDeValor.Embarcador.Logistica.FiltroPesquisaAssociacaoBalsa filtrosPesquisa)
        {
            var query = Consultar(filtrosPesquisa);
            return query.WithOptions(o => { o.SetTimeout(300); }).Count();
        }

        #endregion
    }
}
