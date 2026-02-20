using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using NHibernate.Linq;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Repositorio.Embarcador.Logistica
{
    public class ImportacaoProgramacaoColeta : RepositorioBase<Dominio.Entidades.Embarcador.Logistica.ImportacaoProgramacaoColeta>
    {
        #region Construtores

        public ImportacaoProgramacaoColeta(UnitOfWork unitOfWork) : base(unitOfWork) { }

        #endregion

        #region Métodos Públicos

        public Dominio.Entidades.Embarcador.Logistica.ImportacaoProgramacaoColeta BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.ImportacaoProgramacaoColeta>();
            var result = from obj in query where obj.Codigo == codigo select obj;
            return result.FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.Logistica.ImportacaoProgramacaoColeta BuscarPorCodigoComFetch(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.ImportacaoProgramacaoColeta>();

            var result = from obj in query where obj.Codigo == codigo select obj;

            return result
                .Fetch(o => o.ClienteDestino)
                .Fetch(o => o.TipoOperacao)
                .Fetch(o => o.Produto)
                .FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Logistica.ImportacaoProgramacaoColeta> BuscarProgramacoesEmAndamento()
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.ImportacaoProgramacaoColeta>();

            var result = from obj in query
                         where obj.SituacaoImportacaoProgramacaoColeta == SituacaoImportacaoProgramacaoColeta.EmAndamento &&
                            ((!obj.DataUltimaGeracaoAutomatica.HasValue && obj.DataCriacao.Date <= DateTime.Now.Date.AddDays(-obj.IntervaloDiasGeracao)) ||
                            (obj.DataUltimaGeracaoAutomatica.HasValue && obj.DataUltimaGeracaoAutomatica.Value.Date <= DateTime.Now.Date.AddDays(-obj.IntervaloDiasGeracao)))
                         select obj;

            return result
                .Fetch(o => o.ClienteDestino)
                .Fetch(o => o.TipoOperacao)
                .Fetch(o => o.Produto)
                .ToList();
        }

        public int BuscarProximoNumero()
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.ImportacaoProgramacaoColeta>();

            int? ultimoNumero = query.Max(o => (int?)o.NumeroImportacao);

            return ultimoNumero.HasValue ? (ultimoNumero.Value + 1) : 1;
        }

        public List<Dominio.Entidades.Embarcador.Logistica.ImportacaoProgramacaoColeta> Consultar(Dominio.ObjetosDeValor.Embarcador.Logistica.FiltroPesquisaImportacaoProgramacaoColeta filtrosPesquisa, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametroConsulta)
        {
            var result = Consultar(filtrosPesquisa);

            return ObterLista(result, parametroConsulta);
        }

        public int ContarConsulta(Dominio.ObjetosDeValor.Embarcador.Logistica.FiltroPesquisaImportacaoProgramacaoColeta filtrosPesquisa)
        {
            var result = Consultar(filtrosPesquisa);

            return result.Count();
        }

        #endregion

        #region Métodos Privados

        private IQueryable<Dominio.Entidades.Embarcador.Logistica.ImportacaoProgramacaoColeta> Consultar(Dominio.ObjetosDeValor.Embarcador.Logistica.FiltroPesquisaImportacaoProgramacaoColeta filtrosPesquisa)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.ImportacaoProgramacaoColeta>();

            var result = from obj in query select obj;

            if (filtrosPesquisa.NumeroImportacao > 0)
                result = result.Where(obj => obj.NumeroImportacao == filtrosPesquisa.NumeroImportacao);

            if (filtrosPesquisa.Situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoImportacaoProgramacaoColeta.EmCriacao)
                result = result.Where(o => o.SituacaoImportacaoProgramacaoColeta == filtrosPesquisa.Situacao);
            else if (filtrosPesquisa.Situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoImportacaoProgramacaoColeta.Finalizado)
                result = result.Where(o => o.SituacaoImportacaoProgramacaoColeta == filtrosPesquisa.Situacao);

            if (filtrosPesquisa.CnpjCpfDestino > 0)
                result = result.Where(obj => obj.ClienteDestino.CPF_CNPJ == filtrosPesquisa.CnpjCpfDestino);

            if (filtrosPesquisa.NumeroImportacao > 0)
                result = result.Where(obj => obj.TipoOperacao.Codigo == filtrosPesquisa.CodigoTipoOperacao);

            if (filtrosPesquisa.NumeroImportacao > 0)
                result = result.Where(obj => obj.Produto.Codigo == filtrosPesquisa.CodigoProduto);

            return result;
        }

        #endregion
    }
}
