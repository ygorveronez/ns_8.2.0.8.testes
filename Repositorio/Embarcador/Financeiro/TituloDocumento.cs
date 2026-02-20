using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using NHibernate.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Text;

namespace Repositorio.Embarcador.Financeiro
{
    public class TituloDocumento : RepositorioBase<Dominio.Entidades.Embarcador.Financeiro.TituloDocumento>
    {
        public TituloDocumento(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public bool ExisteTituloPendentePorCTe(Dominio.Entidades.ConhecimentoDeTransporteEletronico cte)
        {
            IQueryable<Dominio.Entidades.Embarcador.Financeiro.TituloDocumento> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.TituloDocumento>();

            query = query.Where(o => o.CTe == cte && o.Titulo.StatusTitulo != StatusTitulo.Cancelado);

            return query.Select(o => o.Codigo).Any();
        }

        public List<Dominio.Entidades.Embarcador.Financeiro.TituloDocumento> BuscarPorTitulo(int codigoTitulo)
        {
            IQueryable<Dominio.Entidades.Embarcador.Financeiro.TituloDocumento> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.TituloDocumento>();

            query = query.Where(o => o.Titulo.Codigo == codigoTitulo);

            return query.ToList();
        }

        public Dominio.Entidades.Embarcador.Financeiro.TituloDocumento BuscarPrimeiroPorCTe(int codigoCTe)
        {
            IQueryable<Dominio.Entidades.Embarcador.Financeiro.TituloDocumento> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.TituloDocumento>();

            query = query.Where(o => o.CTe.Codigo == codigoCTe);

            return query.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Financeiro.TituloDocumento> BuscarPorTitulo(List<int> codigosTitulos)
        {
            IQueryable<Dominio.Entidades.Embarcador.Financeiro.TituloDocumento> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.TituloDocumento>();

            query = query.Where(o => codigosTitulos.Contains(o.Titulo.Codigo));

            return query.Fetch(o => o.FaturaDocumento).ToList();
        }

        public List<Dominio.Entidades.Embarcador.Financeiro.TituloDocumento> BuscarPorFatura(int codigoFatura)
        {
            IQueryable<Dominio.Entidades.Embarcador.Financeiro.TituloDocumento> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.TituloDocumento>();

            query = query.Where(o => o.FaturaDocumento.Fatura.Codigo == codigoFatura);

            return query.Fetch(o => o.FaturaDocumento).Fetch(o => o.Titulo).ToList();
        }

        public List<int> BuscarCodigosTitulosEmAbertoPorCargaECTe(int codigoCarga)
        {
            IQueryable<Dominio.Entidades.Embarcador.Financeiro.TituloDocumento> queryTituloDocumento = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.TituloDocumento>();
            IQueryable<Dominio.Entidades.Embarcador.Cargas.CargaCTe> queryCargaCTe = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCTe>();

            queryCargaCTe = queryCargaCTe.Where(o => o.Carga.Codigo == codigoCarga);

            queryTituloDocumento = queryTituloDocumento.Where(o => (queryCargaCTe.Select(cargaCTe => cargaCTe.CTe.Codigo).Contains(o.CTe.Codigo) || o.Carga.Codigo == codigoCarga) &&
                                                                   o.Titulo.StatusTitulo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.StatusTitulo.EmAberto);

            return queryTituloDocumento.Select(o => o.Titulo.Codigo).Distinct().ToList();
        }

        public List<int> BuscarCodigosTitulosEmAbertoPorCargaCTe(int codigoCarga)
        {
            IQueryable<Dominio.Entidades.Embarcador.Financeiro.TituloDocumento> queryTituloDocumento = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.TituloDocumento>();
            IQueryable<Dominio.Entidades.Embarcador.Cargas.CargaCTe> queryCargaCTe = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCTe>();

            queryCargaCTe = queryCargaCTe.Where(o => o.Carga.Codigo == codigoCarga);

            queryTituloDocumento = queryTituloDocumento.Where(o => queryCargaCTe.Select(cargaCTe => cargaCTe.CTe.Codigo).Contains(o.CTe.Codigo) &&
                                                                   o.Titulo.StatusTitulo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.StatusTitulo.EmAberto);

            return queryTituloDocumento.Select(o => o.Titulo.Codigo).Distinct().ToList();
        }

        public bool ContemDocumentosNoTitulo(int codigoTitulo)
        {
            IQueryable<Dominio.Entidades.Embarcador.Financeiro.TituloDocumento> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.TituloDocumento>();
            query = query.Where(o => o.Titulo.Codigo == codigoTitulo);

            return query.Any();
        }

        public List<int> BuscarCodigosTitulosEmAbertoPorCarga(int codigoCarga)
        {
            IQueryable<Dominio.Entidades.Embarcador.Financeiro.TituloDocumento> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.TituloDocumento>();

            query = query.Where(o => o.Carga.Codigo == codigoCarga && o.Titulo.StatusTitulo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.StatusTitulo.EmAberto);

            return query.Select(o => o.Titulo.Codigo).Distinct().ToList();
        }

        public List<Dominio.Entidades.Embarcador.Financeiro.Titulo> BuscarTitulosEmAbertoPorCTe(int codigoCTe)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.TituloDocumento>();

            query = query.Where(o => o.CTe.Codigo == codigoCTe && o.Titulo.StatusTitulo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.StatusTitulo.EmAberto);

            return query.Select(o => o.Titulo).Distinct().ToList();
        }

        public List<Dominio.Entidades.Embarcador.Financeiro.Titulo> BuscarTitulosEmAbertoPorCarga(int codigoCarga)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.TituloDocumento>();

            query = query.Where(o => o.Carga.Codigo == codigoCarga && o.Titulo.StatusTitulo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.StatusTitulo.EmAberto);

            return query.Select(o => o.Titulo).Distinct().ToList();
        }

        public List<int> BuscarNumeroTituloPorCTe(int codigoCTe)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.TituloDocumento>();

            query = query.Where(o => o.CTe.Codigo == codigoCTe && (o.Titulo.StatusTitulo != Dominio.ObjetosDeValor.Embarcador.Enumeradores.StatusTitulo.Cancelado && o.Titulo.StatusTitulo != Dominio.ObjetosDeValor.Embarcador.Enumeradores.StatusTitulo.EmAberto));

            return query.Select(o => o.Titulo.Codigo).Distinct().ToList();
        }

        public Dominio.Entidades.Embarcador.Financeiro.Titulo BuscarTituloPorCTe(int codigoCTe)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.TituloDocumento>();

            query = query.Where(o => o.CTe.Codigo == codigoCTe && (o.Titulo.StatusTitulo != Dominio.ObjetosDeValor.Embarcador.Enumeradores.StatusTitulo.Cancelado) && o.Titulo.TipoTitulo == TipoTitulo.Receber);

            return query.Select(o => o.Titulo)?.FirstOrDefault();
        }

        public List<int> BuscarNumeroTituloPorCTe(int[] codigosCTes)
        {
            IQueryable<Dominio.Entidades.Embarcador.Financeiro.TituloDocumento> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.TituloDocumento>();

            query = query.Where(o => codigosCTes.Contains(o.CTe.Codigo) &&
                                     o.Titulo.StatusTitulo != Dominio.ObjetosDeValor.Embarcador.Enumeradores.StatusTitulo.Cancelado &&
                                     o.Titulo.StatusTitulo != Dominio.ObjetosDeValor.Embarcador.Enumeradores.StatusTitulo.EmAberto);

            return query.Select(o => o.Titulo.Codigo).Distinct().ToList();
        }

        public List<int> BuscarNumeroTituloPorCargaCTe(int codigoCarga)
        {
            IQueryable<Dominio.Entidades.Embarcador.Financeiro.TituloDocumento> queryTituloDocumento = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.TituloDocumento>();
            IQueryable<Dominio.Entidades.Embarcador.Cargas.CargaCTe> queryCargaCTe = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCTe>();

            queryCargaCTe = queryCargaCTe.Where(o => o.CTe != null && o.Carga.Codigo == codigoCarga);

            queryTituloDocumento = queryTituloDocumento.Where(o => queryCargaCTe.Select(cargaCTe => cargaCTe.CTe.Codigo).Contains(o.CTe.Codigo) &&
                                                                   o.Titulo.StatusTitulo != Dominio.ObjetosDeValor.Embarcador.Enumeradores.StatusTitulo.Cancelado &&
                                                                   o.Titulo.StatusTitulo != Dominio.ObjetosDeValor.Embarcador.Enumeradores.StatusTitulo.EmAberto);

            return queryTituloDocumento.Select(o => o.Titulo.Codigo).Distinct().ToList();
        }

        public List<int> BuscarNumeroTituloPorOcorrencia(int codigoCargaOcorrencia)
        {
            IQueryable<Dominio.Entidades.Embarcador.Financeiro.TituloDocumento> queryTituloDocumento = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.TituloDocumento>();
            IQueryable<Dominio.Entidades.Embarcador.Cargas.CargaCTe> queryCargaCTe = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCTe>();

            queryCargaCTe = queryCargaCTe.Where(o => o.CTe != null && o.CargaCTeComplementoInfo.CargaOcorrencia.Codigo == codigoCargaOcorrencia);

            queryTituloDocumento = queryTituloDocumento.Where(o => queryCargaCTe.Any(cargaCTe => cargaCTe.CTe == o.CTe) &&
                                                                   o.Titulo.StatusTitulo != Dominio.ObjetosDeValor.Embarcador.Enumeradores.StatusTitulo.Cancelado &&
                                                                   o.Titulo.StatusTitulo != Dominio.ObjetosDeValor.Embarcador.Enumeradores.StatusTitulo.EmAberto);

            return queryTituloDocumento.Select(o => o.Titulo.Codigo).Distinct().ToList();
        }

        public List<int> BuscarNumeroTituloPorCarga(int codigoCarga)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.TituloDocumento>();

            query = query.Where(o => o.Carga.Codigo == codigoCarga && (o.Titulo.StatusTitulo != Dominio.ObjetosDeValor.Embarcador.Enumeradores.StatusTitulo.Cancelado && o.Titulo.StatusTitulo != Dominio.ObjetosDeValor.Embarcador.Enumeradores.StatusTitulo.EmAberto));

            return query.Select(o => o.Titulo.Codigo).Distinct().ToList();
        }

        public List<Dominio.Entidades.Embarcador.Financeiro.TituloDocumento> BuscarPorCarga(int codigoCarga)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.TituloDocumento>();

            query = query.Where(o => o.Carga.Codigo == codigoCarga);

            return query.ToList();
        }

        public List<int> BuscarNumeroBoletoTituloPorCTe(int codigoCTe)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.TituloDocumento>();

            query = query.Where(o => o.CTe.Codigo == codigoCTe && o.Titulo.NossoNumero != null && o.Titulo.NossoNumero != "");

            return query.Select(o => o.Titulo.Codigo).Distinct().ToList();
        }

        public List<int> BuscarNumeroBoletoTituloPorCTe(int[] codigosCTes)
        {
            IQueryable<Dominio.Entidades.Embarcador.Financeiro.TituloDocumento> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.TituloDocumento>();

            query = query.Where(o => codigosCTes.Contains(o.CTe.Codigo) && o.Titulo.NossoNumero != null && o.Titulo.NossoNumero != "");

            return query.Select(o => o.Titulo.Codigo).Distinct().ToList();
        }

        public List<int> BuscarNumeroBoletoTituloPorCargaCTe(int codigoCarga)
        {
            IQueryable<Dominio.Entidades.Embarcador.Financeiro.TituloDocumento> queryTituloDocumento = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.TituloDocumento>();
            IQueryable<Dominio.Entidades.Embarcador.Cargas.CargaCTe> queryCargaCTe = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCTe>();

            queryCargaCTe = queryCargaCTe.Where(o => o.CTe != null && o.Carga.Codigo == codigoCarga);

            queryTituloDocumento = queryTituloDocumento.Where(o => queryCargaCTe.Select(cargaCTe => cargaCTe.CTe.Codigo).Contains(o.CTe.Codigo) &&
                                                                   o.Titulo.NossoNumero != null &&
                                                                   o.Titulo.NossoNumero != "");

            return queryTituloDocumento.Select(o => o.Titulo.Codigo).Distinct().ToList();
        }

        public List<int> BuscarNumeroBoletoTituloPorOcorrencia(int codigoCargaOcorrencia)
        {
            IQueryable<Dominio.Entidades.Embarcador.Financeiro.TituloDocumento> queryTituloDocumento = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.TituloDocumento>();
            IQueryable<Dominio.Entidades.Embarcador.Cargas.CargaCTe> queryCargaCTe = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCTe>();

            queryCargaCTe = queryCargaCTe.Where(o => o.CTe != null && o.CargaCTeComplementoInfo.CargaOcorrencia.Codigo == codigoCargaOcorrencia);

            queryTituloDocumento = queryTituloDocumento.Where(o => queryCargaCTe.Any(cargaCTe => cargaCTe.CTe == o.CTe) &&
                                                                   o.Titulo.NossoNumero != null &&
                                                                   o.Titulo.NossoNumero != "");

            return queryTituloDocumento.Select(o => o.Titulo.Codigo).Distinct().ToList();
        }

        public List<int> BuscarNumeroBoletoTituloPorCarga(int codigoCarga)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.TituloDocumento>();

            query = query.Where(o => o.Carga.Codigo == codigoCarga && o.Titulo.NossoNumero != null && o.Titulo.NossoNumero != "");

            return query.Select(o => o.Titulo.Codigo).Distinct().ToList();
        }

        public List<Dominio.Entidades.Embarcador.Financeiro.TituloDocumento> ConsultarPorTitulo(int codigoTitulo, string propOrdenar, string dirOrdena, int inicio, int limite)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.TituloDocumento>();

            query = query.Where(o => o.Titulo.Codigo == codigoTitulo);

            return query.Fetch(o => o.Carga)
                        .Fetch(o => o.CTe)
                        .OrderBy(propOrdenar + " " + dirOrdena)
                        .Skip(inicio)
                        .Take(limite)
                        .ToList();
        }

        public int ContarConsultaPorTitulo(int codigoTitulo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.TituloDocumento>();

            query = query.Where(o => o.Titulo.Codigo == codigoTitulo);

            return query.Count();
        }

        public void AjustarTitulosDocumentosCancelamentoCarga(int codigoFaturaDocumento, StatusTitulo situacao)
        {
            var query = this.SessionNHiBernate.CreateQuery("UPDATE TituloDocumento tituloDocumento SET tituloDocumento.FaturaDocumento = null WHERE tituloDocumento.FaturaDocumento = :codigoFaturaDocumento AND EXISTS (SELECT titulo.Codigo FROM Titulo titulo WHERE titulo.Codigo = tituloDocumento.Titulo AND titulo.StatusTitulo = :situacao)");

            query.SetInt32("codigoFaturaDocumento", codigoFaturaDocumento);
            query.SetEnum("situacao", situacao);

            query.ExecuteUpdate();
        }

        public List<Dominio.Entidades.ConhecimentoDeTransporteEletronico> BuscarCTesPorTitulo(int codigoTitulo)
        {
            IQueryable<Dominio.Entidades.Embarcador.Financeiro.TituloDocumento> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.TituloDocumento>();

            query = query.Where(o => o.Titulo.Codigo == codigoTitulo);

            return query.Select(o => o.CTe).ToList();
        }

    }
}
