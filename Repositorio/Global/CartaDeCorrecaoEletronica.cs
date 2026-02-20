using NHibernate.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Dynamic.Core;

namespace Repositorio
{
    public class CartaDeCorrecaoEletronica : RepositorioBase<Dominio.Entidades.CartaDeCorrecaoEletronica>, Dominio.Interfaces.Repositorios.CartaDeCorrecaoEletronica
    {
        public CartaDeCorrecaoEletronica(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.CartaDeCorrecaoEletronica BuscarPorProtocoloAutorizacao(int codigoProtocolo)
        {
            IQueryable<Dominio.Entidades.CartaDeCorrecaoEletronica> query = SessionNHiBernate.Query<Dominio.Entidades.CartaDeCorrecaoEletronica>();

            query = query.Where(o => o.CodigoIntegrador == codigoProtocolo);

            return query.FirstOrDefault();
        }

        public Dominio.Entidades.CartaDeCorrecaoEletronica BuscarPorProtocoloAutorizacao(int codigoProtocolo, string chaveCTe)
        {
            IQueryable<Dominio.Entidades.CartaDeCorrecaoEletronica> query = SessionNHiBernate.Query<Dominio.Entidades.CartaDeCorrecaoEletronica>();

            query = query.Where(o => o.CodigoIntegrador == codigoProtocolo && o.CTe.Chave == chaveCTe);

            return query.FirstOrDefault();
        }

        public Dominio.Entidades.CartaDeCorrecaoEletronica BuscarUltimaCCeAutorizadaPorCTe(int codigoCTe)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.CartaDeCorrecaoEletronica>();
            var result = from obj in query where obj.CTe.Codigo == codigoCTe && obj.Status == Dominio.Enumeradores.StatusCCe.Autorizado select obj;
            return result.OrderBy("NumeroSequencialEvento descending").FirstOrDefault();
        }
        public List<Dominio.Entidades.CartaDeCorrecaoEletronica> BuscarPorCTe(int codigoCTe)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.CartaDeCorrecaoEletronica>();
            var result = from obj in query where obj.CTe.Codigo == codigoCTe select obj;
            return result.ToList();
        }

        public Dominio.Entidades.CartaDeCorrecaoEletronica BuscarPorCodigo(int codigo, int codigoCTe)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.CartaDeCorrecaoEletronica>();
            var result = from obj in query where obj.CTe.Codigo == codigoCTe && obj.Codigo == codigo select obj;
            return result.FirstOrDefault();
        }

        public Dominio.Entidades.CartaDeCorrecaoEletronica BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.CartaDeCorrecaoEletronica>();
            var result = from obj in query where obj.Codigo == codigo select obj;
            return result.FirstOrDefault();
        }

        public int BuscarUltimoNumeroSequencial(int codigoCTe)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.CartaDeCorrecaoEletronica>();
            var result = from obj in query where obj.CTe.Codigo == codigoCTe select obj;

            int? max = result.Max(o => (int?)o.NumeroSequencialEvento);

            return max.HasValue ? max.Value : 0;
        }

        public List<Dominio.Entidades.CartaDeCorrecaoEletronica> Consultar(int codigoCTe, int inicioRegistros, int maximoRegistros)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.CartaDeCorrecaoEletronica>();
            var result = from obj in query where obj.CTe.Codigo == codigoCTe select obj;
            return result.Fetch(o => o.MensagemStatus).OrderByDescending(o => o.NumeroSequencialEvento).Skip(inicioRegistros).Take(maximoRegistros).ToList();
        }

        //public int ContarConsulta(int codigoCTe)
        //{
        //    var query = this.SessionNHiBernate.Query<Dominio.Entidades.CartaDeCorrecaoEletronica>();
        //    var result = from obj in query where obj.CTe.Codigo == codigoCTe select obj.Codigo;
        //    return result.Count();
        //}

        public List<Dominio.Entidades.CartaDeCorrecaoEletronica> BuscarPorStatus(Dominio.Enumeradores.StatusCCe[] status)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.CartaDeCorrecaoEletronica>();
            var result = from obj in query where status.Contains(obj.Status) select obj;
            return result.ToList();
        }

        public List<Dominio.ObjetosDeValor.Relatorios.RelatorioCCs> ConsultaRelatorioCCe(int codigoEmpresa, Dominio.Enumeradores.StatusCCe? statusAux, int serieCTe, int numeroInicialCTe, int numeroFinalCTe, DateTime dataInicialCCe, DateTime dataFinalCCe, DateTime dataInicialCTe, DateTime dataFinalCTe)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.ItemCCe>();
            var result = from obj in query select obj;

            result = result.Where(o => o.CCe.CTe.Empresa.Codigo == codigoEmpresa);

            if (statusAux != null)
                result = result.Where(o => o.CCe.Status == statusAux.Value);

            if (serieCTe > 0)
                result = result.Where(o => o.CCe.CTe.Serie.Numero == serieCTe);

            if (numeroInicialCTe > 0)
                result = result.Where(o => o.CCe.CTe.Numero > numeroInicialCTe);

            if (numeroFinalCTe > 0)
                result = result.Where(o => o.CCe.CTe.Numero <= numeroFinalCTe);

            if (dataInicialCCe != DateTime.MinValue)
                result = result.Where(o => o.CCe.DataEmissao > dataInicialCCe.Date);

            if (dataFinalCCe != DateTime.MinValue)
                result = result.Where(o => o.CCe.DataEmissao < dataFinalCCe.Date.AddDays(1));

            if (dataInicialCTe != DateTime.MinValue)
                result = result.Where(o => o.CCe.CTe.DataEmissao > dataInicialCTe.Date);

            if (dataFinalCTe != DateTime.MinValue)
                result = result.Where(o => o.CCe.CTe.DataEmissao < dataFinalCTe.Date.AddDays(1));


            return result.Select(o => new Dominio.ObjetosDeValor.Relatorios.RelatorioCCs()
            {
                AgrupamentoCTe = o.CCe.CTe.Codigo,
                NumeroCTe = o.CCe.CTe.Numero,
                NumeroSerie = o.CCe.CTe.Serie.Numero,
                DataEmissaoCTe = o.CCe.CTe.DataEmissao.Value.ToString("dd/MM/yyyy"),

                AgrupamentoCCe = o.CCe.Codigo,
                NumeroCCe = o.CCe.NumeroSequencialEvento,
                Protocolo = o.CCe.Protocolo,
                DataEmissaoCCe = o.CCe.DataEmissao.Value.ToString("dd/MM/yyyy"),
                DataAutorizacaoCCe = o.CCe.DataRetornoSefaz.Value.ToString("dd/MM/yyyy hh:mm"),

                Campo = o.CampoAlterado.Descricao,
                Valor = o.ValorAlterado
            }).ToList();
        }

        public Dominio.Entidades.Embarcador.Cargas.CartaCorrecaoCCEIntegracaoArquivo BuscarIntergacaoCartaCorrecaoPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CartaCorrecaoCCEIntegracaoArquivo>();
            var result = from obj in query where obj.Codigo == codigo select obj;

            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.CartaCorrecaoCCEIntegracaoArquivo> BuscarArquivosPorIntergacao(int codigo, int inicio, int limite)
        {
            var queryCargaCTeIntegracao = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CartaCorrecaoIntegracao>();
            var resultCargaCTeIntegracao = from obj in queryCargaCTeIntegracao where obj.Codigo == codigo select obj;

            var queryCargaCTeIntegracaoArquivo = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CartaCorrecaoCCEIntegracaoArquivo>();
            var resultCargaCTeIntegracaoArquivo = from obj in queryCargaCTeIntegracaoArquivo where resultCargaCTeIntegracao.Any(p => p.ArquivosTransacao.Contains(obj)) orderby obj.Data descending select obj;

            return resultCargaCTeIntegracaoArquivo.Skip(inicio).Take(limite).ToList();
        }

        public int ContarBuscarArquivosPorIntergacao(int codigo)
        {
            var queryCargaCTeIntegracao = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CartaCorrecaoIntegracao>();
            var resultCargaCTeIntegracao = from obj in queryCargaCTeIntegracao where obj.Codigo == codigo select obj;

            var queryCargaCTeIntegracaoArquivo = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CartaCorrecaoCCEIntegracaoArquivo>();
            var resultCargaCTeIntegracaoArquivo = from obj in queryCargaCTeIntegracaoArquivo where resultCargaCTeIntegracao.Any(p => p.ArquivosTransacao.Contains(obj)) orderby obj.Data descending select obj;

            return resultCargaCTeIntegracaoArquivo.Count();
        }


        #region Consulta

        public List<Dominio.Entidades.CartaDeCorrecaoEletronica> Consultar(int codigoCTe, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            IQueryable<Dominio.Entidades.CartaDeCorrecaoEletronica> query = ObterQueryConsulta(codigoCTe);

            return query.Fetch(o => o.MensagemStatus)
                        .OrderBy(parametrosConsulta.PropriedadeOrdenar + " " + parametrosConsulta.DirecaoOrdenar)
                        .Skip(parametrosConsulta.InicioRegistros)
                        .Take(parametrosConsulta.LimiteRegistros)
                        .ToList();
        }

        public int ContarConsulta(int codigoCTe)
        {
            IQueryable<Dominio.Entidades.CartaDeCorrecaoEletronica> query = ObterQueryConsulta(codigoCTe);

            return query.Count();
        }

        public IQueryable<Dominio.Entidades.CartaDeCorrecaoEletronica> ObterQueryConsulta(int codigoCTe)
        {
            IQueryable<Dominio.Entidades.CartaDeCorrecaoEletronica> query = this.SessionNHiBernate.Query<Dominio.Entidades.CartaDeCorrecaoEletronica>();

            if (codigoCTe > 0)
                query = query.Where(o => o.CTe.Codigo == codigoCTe);

            return query;
        }


        #endregion 
    }
}
