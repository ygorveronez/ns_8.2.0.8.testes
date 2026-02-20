using System;
using System.Collections.Generic;
using System.Linq;
using NHibernate.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.Escrituracao
{
    public class LoteEscrituracaoCancelamento : RepositorioBase<Dominio.Entidades.Embarcador.Escrituracao.LoteEscrituracaoCancelamento>
    {
        public LoteEscrituracaoCancelamento(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.Embarcador.Escrituracao.LoteEscrituracaoCancelamento BuscarPorCodigo(int codigo)
        {
            IQueryable<Dominio.Entidades.Embarcador.Escrituracao.LoteEscrituracaoCancelamento> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Escrituracao.LoteEscrituracaoCancelamento>();

            query = query.Where(o => o.Codigo == codigo);

            return query.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Escrituracao.LoteEscrituracaoCancelamento> BuscarLotesAgIntegracao(int maximoRegistros)
        {
            IQueryable<Dominio.Entidades.Embarcador.Escrituracao.LoteEscrituracaoCancelamento> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Escrituracao.LoteEscrituracaoCancelamento>();

            query = query.Where(o => o.Situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoLoteEscrituracaoCancelamento.AgIntegracao);

            if (maximoRegistros > 0)
                query = query.Take(maximoRegistros);

            return query.ToList();
        }

        public int ObterProximoLote()
        {
            IQueryable<Dominio.Entidades.Embarcador.Escrituracao.LoteEscrituracaoCancelamento> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Escrituracao.LoteEscrituracaoCancelamento>();

            int? retorno = query.Max(o => (int?)o.Numero);

            return retorno.HasValue ? (retorno.Value + 1) : 1;
        }

        public List<Dominio.Entidades.Embarcador.Escrituracao.LoteEscrituracaoCancelamento> Consultar(DateTime dataInicio, DateTime dataFim, int transportador, int filial, int carga, int ocorrencia, int numero, int numeroDOC, int localidadePrestacao, double tomador, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoLoteEscrituracaoCancelamento? situacaoLoteEscrituracao, string propOrdena, string dirOrdena, int inicioRegistros, int maximoRegistros)
        {
            IQueryable<Dominio.Entidades.Embarcador.Escrituracao.LoteEscrituracaoCancelamento> query = ObterQueryConsulta(dataInicio, dataFim, transportador, filial, carga, ocorrencia, numero, numeroDOC, localidadePrestacao, tomador, situacaoLoteEscrituracao);

            if (!string.IsNullOrWhiteSpace(propOrdena))
                query = query.OrderBy(propOrdena + (dirOrdena == "asc" ? " ascending" : " descending"));

            if (inicioRegistros > 0)
                query = query.Skip(inicioRegistros);

            if (maximoRegistros > 0)
                query = query.Take(maximoRegistros);

            return query.Fetch(obj => obj.Filial)
                        .Fetch(obj => obj.Empresa)
                        .ThenFetch(obj => obj.Localidade)
                        .Fetch(obj => obj.Tomador)
                        .ThenFetch(obj => obj.Localidade)
                        .ToList();
        }

        public int ContarConsulta(DateTime dataInicio, DateTime dataFim, int transportador, int filial, int carga, int ocorrencia, int numero, int numeroDOC, int localidadePrestacao, double tomador, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoLoteEscrituracaoCancelamento? situacaoLoteEscrituracao)
        {
            IQueryable<Dominio.Entidades.Embarcador.Escrituracao.LoteEscrituracaoCancelamento> query = ObterQueryConsulta(dataInicio, dataFim, transportador, filial, carga, ocorrencia, numero, numeroDOC, localidadePrestacao, tomador, situacaoLoteEscrituracao);

            return query.Count();
        }

        #region Métodos Privados

        private IQueryable<Dominio.Entidades.Embarcador.Escrituracao.LoteEscrituracaoCancelamento> ObterQueryConsulta(DateTime dataInicio, DateTime dataFim, int transportador, int filial, int carga, int ocorrencia, int numero, int numeroDOC, int localidadePrestacao, double tomador, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoLoteEscrituracaoCancelamento? situacaoLoteEscrituracao)
        {
            IQueryable<Dominio.Entidades.Embarcador.Escrituracao.LoteEscrituracaoCancelamento> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Escrituracao.LoteEscrituracaoCancelamento>();

            if (dataInicio != DateTime.MinValue)
                query = query.Where(obj => obj.DataInicial.Value.Date >= dataInicio);

            if (dataFim != DateTime.MinValue)
                query = query.Where(obj => obj.DataFinal.Value.Date <= dataFim);

            if (transportador > 0)
                query = query.Where(o => o.Empresa.Codigo == transportador);

            if (carga > 0)
                query = query.Where(o => o.DocumentosEscrituracao.Any(doc => doc.Carga.Codigo == carga));

            if (numeroDOC > 0)
                query = query.Where(o => o.DocumentosEscrituracao.Any(doc => doc.CTe.Numero == numeroDOC));

            if (tomador > 0D)
                query = query.Where(o => o.DocumentosEscrituracao.Any(doc => doc.CTe.TomadorPagador.Cliente.CPF_CNPJ == tomador));

            if (ocorrencia > 0)
                query = query.Where(o => o.DocumentosEscrituracao.Any(doc => doc.CargaOcorrencia.Codigo == ocorrencia));

            if (filial > 0)
                query = query.Where(o => o.Filial.Codigo == filial);

            if (numero > 0)
                query = query.Where(o => o.Numero == numero);

            if (situacaoLoteEscrituracao.HasValue)
                query = query.Where(o => o.Situacao == situacaoLoteEscrituracao);

            return query;
        }

        #endregion
    }
}
