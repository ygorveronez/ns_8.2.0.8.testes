using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using NHibernate.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.Pedidos
{
    public class ImportacaoPedido : RepositorioBase<Dominio.Entidades.Embarcador.Pedidos.ImportacaoPedido>
    {
        #region Construtores

        public ImportacaoPedido(UnitOfWork unitOfWork) : base(unitOfWork) { }

        #endregion

        #region Métodos Privados

        private IQueryable<Dominio.Entidades.Embarcador.Pedidos.ImportacaoPedido> Consultar(Dominio.ObjetosDeValor.Embarcador.Pedido.FiltroPesquisaImportacaoPedido filtrosPesquisa)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.ImportacaoPedido>();

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

            return query;
        }

        #endregion

        #region Métodos Públicos

        public Dominio.Entidades.Embarcador.Pedidos.ImportacaoPedido BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.ImportacaoPedido>();
            var result = from obj in query where obj.Codigo == codigo select obj;
            return result.FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.Pedidos.ImportacaoPedido BuscarProximaImportacaoPendente()
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.ImportacaoPedido>();
            var result = from obj in query where obj.Situacao == SituacaoImportacaoPedido.Pendente || obj.Situacao == SituacaoImportacaoPedido.Processando select obj;
            return result.OrderBy(obj => obj.DataImportacao).FirstOrDefault();
        }


        public List<Dominio.Entidades.Embarcador.Pedidos.ImportacaoPedido> BuscarProximasImportacaoPendente()
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.ImportacaoPedido>();
            var result = from obj in query where obj.Situacao == SituacaoImportacaoPedido.Pendente || obj.Situacao == SituacaoImportacaoPedido.Processando select obj;
            return query.OrderByDescending(obj => obj.DataImportacao).ToList();
        }


        public List<Dominio.Entidades.Embarcador.Pedidos.ImportacaoPedido> Consultar(Dominio.ObjetosDeValor.Embarcador.Pedido.FiltroPesquisaImportacaoPedido filtrosPesquisa, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
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

        public int ContarConsulta(Dominio.ObjetosDeValor.Embarcador.Pedido.FiltroPesquisaImportacaoPedido filtrosPesquisa)
        {
            var query = Consultar(filtrosPesquisa);
            return query.WithOptions(o => { o.SetTimeout(300); }).Count();
        }

        #endregion
    }
}
