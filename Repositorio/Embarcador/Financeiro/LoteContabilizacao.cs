using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading;

namespace Repositorio.Embarcador.Financeiro
{
    public class LoteContabilizacao : RepositorioBase<Dominio.Entidades.Embarcador.Financeiro.LoteContabilizacao>
    {
        #region Métodos Públicos

        public LoteContabilizacao(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public LoteContabilizacao(UnitOfWork unitOfWork, CancellationToken cancellationToken) : base(unitOfWork, cancellationToken) { }

        public int BuscarUltimoNumero()
        {
            IQueryable<Dominio.Entidades.Embarcador.Financeiro.LoteContabilizacao> query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.LoteContabilizacao>();

            return query.Max(o => (int?)o.Numero) ?? 0;
        }

        public List<Dominio.Entidades.Embarcador.Financeiro.LoteContabilizacao> Consultar(int numero, int codigoEmpresa, double cpfCnpjTomador, DateTime? dataInicial, DateTime? dataFinal, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoMovimentoExportacao? tipo, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoLoteContabilizacao? situacao, string numeroDocumento, string propriedadeOrdenar, string dirOrdena, int inicio, int limite)
        {
            IQueryable<Dominio.Entidades.Embarcador.Financeiro.LoteContabilizacao> query = ObterQueryConsulta(numero, codigoEmpresa, cpfCnpjTomador, dataInicial, dataFinal, tipo, situacao, numeroDocumento);

            return query.OrderBy(propriedadeOrdenar + " " + dirOrdena).Skip(inicio).Take(limite).ToList();
        }

        public int ContarConsulta(int numero, int codigoEmpresa, double cpfCnpjTomador, DateTime? dataInicial, DateTime? dataFinal, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoMovimentoExportacao? tipo, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoLoteContabilizacao? situacao, string numeroDocumento)
        {
            IQueryable<Dominio.Entidades.Embarcador.Financeiro.LoteContabilizacao> query = ObterQueryConsulta(numero, codigoEmpresa, cpfCnpjTomador, dataInicial, dataFinal, tipo, situacao, numeroDocumento);

            return query.Count();
        }

        public List<Dominio.Entidades.Embarcador.Financeiro.LoteContabilizacao> BuscarLotesAgIntegracao(int quantidadeRegistros)
        {
            IQueryable<Dominio.Entidades.Embarcador.Financeiro.LoteContabilizacao> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.LoteContabilizacao>();

            query = query.Where(o => o.Situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoLoteContabilizacao.AgIntegracao && !o.GerouIntegracoes);

            return query.OrderByDescending(o => o.Codigo).Take(quantidadeRegistros).ToList();
        }

        #endregion

        #region Métodos Privados

        private IQueryable<Dominio.Entidades.Embarcador.Financeiro.LoteContabilizacao> ObterQueryConsulta(int numero, int codigoEmpresa, double cpfCnpjTomador, DateTime? dataInicial, DateTime? dataFinal, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoMovimentoExportacao? tipo, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoLoteContabilizacao? situacao, string numeroDocumento)
        {
            IQueryable<Dominio.Entidades.Embarcador.Financeiro.LoteContabilizacao> query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.LoteContabilizacao>();

            if (dataInicial.HasValue && dataFinal.HasValue)
                query = query.Where(obj => obj.DataInicial >= dataInicial.Value.Date && obj.DataFinal <= dataFinal.Value.Date);
            else
            {
                if (dataInicial.HasValue)
                    query = query.Where(obj => obj.DataInicial == dataInicial.Value.Date);

                if (dataFinal.HasValue)
                    query = query.Where(obj => obj.DataFinal == dataFinal.Value.Date);
            }

            if (tipo.HasValue)
                query = query.Where(o => o.Tipo == tipo);

            if (situacao.HasValue)
                query = query.Where(o => o.Situacao == situacao);

            if (numero > 0)
                query = query.Where(o => o.Numero == numero);

            if (cpfCnpjTomador > 0d)
                query = query.Where(o => o.Tomador.CPF_CNPJ == cpfCnpjTomador || o.Documentos.Any(d => d.Tomador.CPF_CNPJ == cpfCnpjTomador));

            if (codigoEmpresa > 0)
                query = query.Where(o => o.Empresa.Codigo == codigoEmpresa);

            if (!string.IsNullOrWhiteSpace(numeroDocumento))
                query = query.Where(o => o.Documentos.Any(d => d.Numero == numeroDocumento));

            return query;
        }

        #endregion
    }
}
