using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.Acerto
{
    public class AcertoOcorrencia : RepositorioBase<Dominio.Entidades.Embarcador.Acerto.AcertoOcorrencia>
    {
        #region Construtores

        public AcertoOcorrencia(UnitOfWork unitOfWork) : base(unitOfWork) { }

        #endregion Construtores

        #region Métodos Públicos

        public Dominio.Entidades.Embarcador.Acerto.AcertoOcorrencia BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Acerto.AcertoOcorrencia>();
            var result = from obj in query where obj.Codigo == codigo select obj;
            return result.FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.Acerto.AcertoOcorrencia BuscarPorOcorrenciaAcerto(int codigoOcorrencia, int codigoAcerto)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Acerto.AcertoOcorrencia>();
            var result = from obj in query where obj.AcertoViagem.Codigo == codigoAcerto && obj.CargaOcorrencia.Codigo == codigoOcorrencia select obj;
            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrencia> Consultar(int codigoAcerto, string propOrdenacao, string dirOrdenacao, int inicioRegistros, int maximoRegistros)
        {
            var consultaCargaOcorrencia = Consultar(codigoAcerto);

            return ObterLista(consultaCargaOcorrencia, propOrdenacao, dirOrdenacao, inicioRegistros, maximoRegistros);
        }

        public int ContarConsulta(int codigoAcerto)
        {
            var consultaCargaOcorrencia = Consultar(codigoAcerto);

            return consultaCargaOcorrencia.Count();
        }

        public List<Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrencia> ConsultarSemAcerto(int numeroOcorrencia, Dominio.Entidades.Embarcador.Acerto.AcertoViagem acertoViagem, bool buscarTodasOcorrencias, string propOrdenacao, string dirOrdenacao, int inicioRegistros, int maximoRegistros)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrencia>();
            var result = from obj in query select obj;

            //if (acertoViagem.Veiculos.Count > 0)
            //    result = result.Where(obj => (acertoViagem.Veiculos.Contains(obj.Carga.Veiculo)));

            if (numeroOcorrencia > 0)
                result = result.Where(obj => obj.NumeroOcorrencia == numeroOcorrencia);

            if (acertoViagem.Motorista != null && !buscarTodasOcorrencias)
                result = result.Where(obj => (obj.Carga.Motoristas.Contains(acertoViagem.Motorista)));

            if (acertoViagem.DataInicial != DateTime.MinValue)
                result = result.Where(obj => obj.DataOcorrencia.Date >= acertoViagem.DataInicial);

            if (acertoViagem.DataFinal != DateTime.MinValue)
                result = result.Where(obj => obj.DataOcorrencia.Date <= acertoViagem.DataFinal);

            var queryAcerto = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Acerto.AcertoOcorrencia>();
            var resultAcerto = from obj in queryAcerto where obj.AcertoViagem.Situacao != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAcertoViagem.Cancelado select obj;

            result = result.Where(obj => !resultAcerto.Select(a => a.CargaOcorrencia).Contains(obj));

            result = result.OrderBy(propOrdenacao + (dirOrdenacao == "asc" ? " ascending" : " descending"));

            if (inicioRegistros > 0)
                result = result.Skip(inicioRegistros);
            if (maximoRegistros > 0)
                result = result.Take(maximoRegistros);

            return result.Distinct().ToList();
        }

        public int ContarConsultaSemAcerto(int numeroOcorrencia, Dominio.Entidades.Embarcador.Acerto.AcertoViagem acertoViagem, bool buscarTodasOcorrencias)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrencia>();
            var result = from obj in query select obj;

            //if (acertoViagem.Veiculos.Count > 0)
            //    result = result.Where(obj => (acertoViagem.Veiculos.Contains(obj.Carga.Veiculo)));

            if (numeroOcorrencia > 0)
                result = result.Where(obj => obj.NumeroOcorrencia == numeroOcorrencia);

            if (acertoViagem.Motorista != null && !buscarTodasOcorrencias)
                result = result.Where(obj => (obj.Carga.Motoristas.Contains(acertoViagem.Motorista)));

            if (acertoViagem.DataInicial != DateTime.MinValue)
                result = result.Where(obj => obj.DataOcorrencia.Date >= acertoViagem.DataInicial);

            if (acertoViagem.DataFinal != DateTime.MinValue)
                result = result.Where(obj => obj.DataOcorrencia.Date <= acertoViagem.DataFinal);

            var queryAcerto = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Acerto.AcertoOcorrencia>();
            var resultAcerto = from obj in queryAcerto where obj.AcertoViagem.Situacao != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAcertoViagem.Cancelado select obj;

            result = result.Where(obj => !resultAcerto.Select(a => a.CargaOcorrencia).Contains(obj));

            return result.Distinct().Count();
        }

        #endregion Métodos Públicos

        #region Métodos Privados

        private IQueryable<Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrencia> Consultar(int codigoAcerto)
        {
            var consultaAcertoOcorrencia = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Acerto.AcertoOcorrencia>()
                .Where(acertoOcorrencia => acertoOcorrencia.AcertoViagem.Codigo == codigoAcerto);

            var consultaCargaOcorrencia = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrencia>()
                .Where(ocorrencia => consultaAcertoOcorrencia.Any(acertoOcorrencia => acertoOcorrencia.CargaOcorrencia.Codigo == ocorrencia.Codigo));

            return consultaCargaOcorrencia;
        }

        #endregion
    }
}
