using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;

namespace Repositorio.Embarcador.Canhotos
{
    public class ControleLeituraImagemCanhoto : RepositorioBase<Dominio.Entidades.Embarcador.Canhotos.ControleLeituraImagemCanhoto>
    {
        #region Construtores

        public ControleLeituraImagemCanhoto(UnitOfWork unitOfWork) : base(unitOfWork) { }

        #endregion

        #region Métodos Privados

        private IQueryable<Dominio.Entidades.Embarcador.Canhotos.ControleLeituraImagemCanhoto> Consultar(DateTime dataInicio, DateTime dataFim, SituacaoleituraImagemCanhoto? situacao, string numeroDocumento, int codigoEmpresa)
        {
            var consultaControleLeituraImagemCanhoto = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Canhotos.ControleLeituraImagemCanhoto>();

            if (situacao.HasValue)
                consultaControleLeituraImagemCanhoto = consultaControleLeituraImagemCanhoto.Where(o => o.SituacaoleituraImagemCanhoto == situacao);

            if (dataInicio != DateTime.MinValue)
                consultaControleLeituraImagemCanhoto = consultaControleLeituraImagemCanhoto.Where(o => o.Data.Date >= dataInicio);

            if (dataFim != DateTime.MinValue)
                consultaControleLeituraImagemCanhoto = consultaControleLeituraImagemCanhoto.Where(o => o.Data.Date <= dataFim);

            if (codigoEmpresa > 0)
                consultaControleLeituraImagemCanhoto = consultaControleLeituraImagemCanhoto.Where(o => o.Canhoto.Empresa.Codigo == codigoEmpresa);

            if (!string.IsNullOrWhiteSpace(numeroDocumento))
                consultaControleLeituraImagemCanhoto = consultaControleLeituraImagemCanhoto.Where(o =>
                    o.NumeroDocumento == numeroDocumento ||
                    o.NumeroDocumento.StartsWith($"{numeroDocumento}, ") ||
                    o.NumeroDocumento.EndsWith($", {numeroDocumento}") ||
                    o.NumeroDocumento.Contains($", {numeroDocumento}, ")
                );

            return consultaControleLeituraImagemCanhoto;
        }

        #endregion

        #region Métodos Públicos

        public Dominio.Entidades.Embarcador.Canhotos.ControleLeituraImagemCanhoto BuscarPorCodigo(int codigo)
        {
            var consultaControleLeituraImagemCanhoto = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Canhotos.ControleLeituraImagemCanhoto>()
                .Where(o => o.Codigo == codigo);

            return consultaControleLeituraImagemCanhoto.FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.Canhotos.ControleLeituraImagemCanhoto BuscarPorCodigoOuCanhoto(int codigo, int canhoto)
        {
            var consultaControleLeituraImagemCanhoto = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Canhotos.ControleLeituraImagemCanhoto>();

            if (codigo > 0)
                consultaControleLeituraImagemCanhoto = consultaControleLeituraImagemCanhoto.Where(o => o.Codigo == codigo);

            if (canhoto > 0)
                consultaControleLeituraImagemCanhoto = consultaControleLeituraImagemCanhoto.Where(o => o.Canhoto.Codigo == canhoto);

            return consultaControleLeituraImagemCanhoto.OrderByDescending(o => o.Codigo).FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Canhotos.ControleLeituraImagemCanhoto> BuscarPorTodosCodigoOuCanhoto(int codigo, int canhoto)
        {
            var consultaControleLeituraImagemCanhoto = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Canhotos.ControleLeituraImagemCanhoto>();

            if (codigo > 0)
                consultaControleLeituraImagemCanhoto = consultaControleLeituraImagemCanhoto.Where(o => o.Codigo == codigo);

            if (canhoto > 0)
                consultaControleLeituraImagemCanhoto = consultaControleLeituraImagemCanhoto.Where(o => o.Canhoto.Codigo == canhoto);

            return consultaControleLeituraImagemCanhoto.OrderBy(o => o.Codigo).ToList();
        }

        public Dominio.Entidades.Embarcador.Canhotos.ControleLeituraImagemCanhoto BuscarAguardandoImagemPorGuidArquivo(string guidArquivo)
        {
            var consultaControleLeituraImagemCanhoto = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Canhotos.ControleLeituraImagemCanhoto>()
                .Where(o => o.GuidArquivo == guidArquivo && o.SituacaoleituraImagemCanhoto == SituacaoleituraImagemCanhoto.AgImagem);

            return consultaControleLeituraImagemCanhoto.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Canhotos.ControleLeituraImagemCanhoto> BuscarAgProcessamento(int inicio, int limite)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Canhotos.ControleLeituraImagemCanhoto>();

            var result = from obj in query where obj.SituacaoleituraImagemCanhoto == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoleituraImagemCanhoto.AgProcessamento select obj;

            return result.Skip(inicio).Take(limite).OrderBy(obj => obj.Data).ToList();
        }

        public List<Dominio.Entidades.Embarcador.Canhotos.ControleLeituraImagemCanhoto> Consultar(DateTime dataInicio, DateTime dataFim, SituacaoleituraImagemCanhoto? situacao, string numeroDocumento, int codigoEmpresa, string propOrdena, string dirOrdena, int inicioRegistros, int maximoRegistros)
        {
            var result = Consultar(dataInicio, dataFim, situacao, numeroDocumento, codigoEmpresa);

            if (!string.IsNullOrWhiteSpace(propOrdena))
                result = result.OrderBy(propOrdena + (dirOrdena == "asc" ? " ascending" : " descending"));

            if (inicioRegistros > 0)
                result = result.Skip(inicioRegistros);

            if (maximoRegistros > 0)
                result = result.Take(maximoRegistros);

            return result.ToList();
        }

        public int ContarConsulta(DateTime dataInicio, DateTime dataFim, SituacaoleituraImagemCanhoto? situacao, string numeroDocumento, int codigoEmpresa)
        {
            var result = Consultar(dataInicio, dataFim, situacao, numeroDocumento, codigoEmpresa);

            return result.Count();
        }

        #endregion
    }
}
