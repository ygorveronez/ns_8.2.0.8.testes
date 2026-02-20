using System;
using System.Collections.Generic;
using System.Linq;
using NHibernate.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.Escrituracao
{
    public class LoteEscrituracao : RepositorioBase<Dominio.Entidades.Embarcador.Escrituracao.LoteEscrituracao>
    {

        public LoteEscrituracao(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.Embarcador.Escrituracao.LoteEscrituracao BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Escrituracao.LoteEscrituracao>();
            var result = from obj in query where obj.Codigo == codigo select obj;
            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Escrituracao.LoteEscrituracao> BuscarLotesAgIntegracao(int maximoRegistros)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Escrituracao.LoteEscrituracao>();
            var result = from obj in query where obj.Situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoLoteEscrituracao.AgIntegracao select obj;

            if (maximoRegistros > 0)
                result = result.Take(maximoRegistros);

            return result.ToList();
        }

        public int ObterProximoLote()
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Escrituracao.LoteEscrituracao>();

            var result = from obj in query select obj;

            int? retorno = result.Max(o => (int?)o.Numero);
            return retorno.HasValue ? (retorno.Value + 1) : 1;

        }

        private IQueryable<Dominio.Entidades.Embarcador.Escrituracao.LoteEscrituracao> _Consultar(DateTime dataInicio, DateTime dataFim, int transportador, int filial, int carga, int ocorrencia, int numero, int numeroDOC, int localidadePrestacao, double tomador, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoLoteEscrituracao situacaoLoteEscrituracao)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Escrituracao.LoteEscrituracao>();

            var result = from obj in query select obj;

            if (dataInicio != DateTime.MinValue)
                result = result.Where(obj => !obj.DataInicial.HasValue || obj.DataInicial >= dataInicio.Date);

            if (dataFim != DateTime.MinValue)
                result = result.Where(obj => !obj.DataFinal.HasValue || obj.DataFinal < dataFim.AddDays(1).Date);

            if (transportador > 0)
                result = result.Where(o => o.Empresa.Codigo == transportador);

            if (carga > 0)
                result = result.Where(o => o.DocumentosEscrituracao.Any(doc => doc.Carga.Codigo == carga));

            if (numeroDOC > 0)
                result = result.Where(o => o.DocumentosEscrituracao.Any(doc => doc.CTe.Numero == numeroDOC));

            if (tomador > 0)
                result = result.Where(o => o.DocumentosEscrituracao.Any(doc => doc.CTe.TomadorPagador.Cliente.CPF_CNPJ == tomador));

            if (ocorrencia > 0)
                result = result.Where(o => o.DocumentosEscrituracao.Any(doc => doc.CargaOcorrencia.Codigo == ocorrencia));

            if (filial > 0)
                result = result.Where(o => o.Filial.Codigo == filial);

            if (numero > 0)
                result = result.Where(o => o.Numero == numero);

            if (situacaoLoteEscrituracao != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoLoteEscrituracao.Todos)
                result = result.Where(o => o.Situacao == situacaoLoteEscrituracao);

            return result;
        }

        public List<Dominio.Entidades.Embarcador.Escrituracao.LoteEscrituracao> Consultar(DateTime dataInicio, DateTime dataFim, int transportador, int filial, int carga, int ocorrencia, int numero, int numeroDOC, int localidadePrestacao, double tomador, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoLoteEscrituracao situacaoLoteEscrituracao, string propOrdena, string dirOrdena, int inicioRegistros, int maximoRegistros)
        {

            var result = _Consultar(dataInicio, dataFim, transportador, filial, carga, ocorrencia, numero, numeroDOC, localidadePrestacao, tomador, situacaoLoteEscrituracao);

            if (!string.IsNullOrWhiteSpace(propOrdena))
                result = result.OrderBy(propOrdena + (dirOrdena == "asc" ? " ascending" : " descending"));

            if (inicioRegistros > 0)
                result = result.Skip(inicioRegistros);

            if (maximoRegistros > 0)
                result = result.Take(maximoRegistros);

            return result
                .Fetch(obj => obj.Filial)
                .Fetch(obj => obj.Empresa)
                .ThenFetch(obj => obj.Localidade)
                .Fetch(obj => obj.Tomador)
                .ThenFetch(obj => obj.Localidade)
                .ToList();
        }

        public int ContarConsulta(DateTime dataInicio, DateTime dataFim, int transportador, int filial, int carga, int ocorrencia, int numero, int numeroDOC, int localidadePrestacao, double tomador, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoLoteEscrituracao situacaoLoteEscrituracao)
        {
            var result = _Consultar(dataInicio, dataFim, transportador, filial, carga, ocorrencia, numero, numeroDOC, localidadePrestacao, tomador, situacaoLoteEscrituracao);

            return result.Count();
        }

    }
}
