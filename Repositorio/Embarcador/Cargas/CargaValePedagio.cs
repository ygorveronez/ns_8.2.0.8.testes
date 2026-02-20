using NHibernate.Linq;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.Cargas
{
    public class CargaValePedagio : RepositorioBase<Dominio.Entidades.Embarcador.Cargas.CargaValePedagio>
    {
        public CargaValePedagio(UnitOfWork unitOfWork) : base(unitOfWork) { }

        #region Métodos Públicos

        public List<Dominio.Entidades.Embarcador.Cargas.CargaValePedagio> Consultar(int codigoCarga, string propOrdenacao, string dirOrdenacao, int inicioRegistros, int maximoRegistros)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaValePedagio>();

            var result = from obj in query where obj.Carga.Codigo == codigoCarga select obj;

            return result
                .Fetch(obj => obj.Fornecedor)
                .Fetch(obj => obj.Responsavel)
                .OrderBy(propOrdenacao + " " + dirOrdenacao)
                .Skip(inicioRegistros)
                .Take(maximoRegistros).ToList();
        }

        public int ContarConsulta(int codigoCarga)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaValePedagio>();

            var result = from obj in query where obj.Carga.Codigo == codigoCarga select obj;

            return result.Count();
        }

        public Dominio.Entidades.Embarcador.Cargas.CargaValePedagio BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaValePedagio>();

            var result = from obj in query where obj.Codigo == codigo select obj;

            return result.FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.Cargas.CargaValePedagio BuscarPorCodigoIntegracao(string codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaValePedagio>();

            var result = from obj in query where obj.CargaIntegracaoValePedagio.CodigoIntegracaoValePedagio.Contains(codigo) select obj;

            return result.FirstOrDefault();
        }

        public bool ExistePorCarga(int codigoCarga)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaValePedagio>();

            var result = from obj in query where obj.Carga.Codigo == codigoCarga select obj.Codigo;

            return result.Any();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.CargaValePedagio> BuscarPorCarga(int codigoCarga, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware? tipoServicoMultisoftware = null)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaValePedagio>();

            query = query.Where(o => o.Carga.Codigo == codigoCarga);

            if (tipoServicoMultisoftware != null && tipoServicoMultisoftware != AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS)
            {
                query = query.Where(o => (o.CargaIntegracaoValePedagio == null || o.CargaIntegracaoValePedagio.SituacaoValePedagio != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoValePedagio.Cancelada) && !o.NaoIncluirMDFe);
            }

            return query.ToList();
        }

        public decimal BuscarValorPedagioPorCarga(int codigoCarga, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware? tipoServicoMultisoftware = null)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaValePedagio>();

            query = query.Where(o => o.Carga.Codigo == codigoCarga);

            if (tipoServicoMultisoftware != null && tipoServicoMultisoftware != AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS)
            {
                query = query.Where(o => (o.CargaIntegracaoValePedagio == null || o.CargaIntegracaoValePedagio.SituacaoValePedagio != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoValePedagio.Cancelada) && !o.NaoIncluirMDFe);
            }

            return query.Sum(x => (decimal?)x.Valor) ?? 0;
        }

        public List<Dominio.Entidades.Embarcador.Cargas.CargaValePedagio> BuscarPorCargaENroComprovante(int codigoCarga, string numeroComprovante)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaValePedagio>();

            query = query.Where(o => o.Carga.Codigo == codigoCarga && o.NumeroComprovante == numeroComprovante);

            return query.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.CargaValePedagio> BuscarPorProtocoloCarga(int protocoloCarga)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaValePedagio>();

            query = query.Where(o => o.Carga.Protocolo == protocoloCarga);

            return query
                .Fetch(obj => obj.Carga)
                .ToList();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.CargaValePedagio> BuscarPorCargas(List<int> codigosCargas)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaValePedagio>();

            query = query.Where(o => codigosCargas.Contains(o.Carga.Codigo));

            return query.ToList();
        }

        public IQueryable<Dominio.Entidades.Embarcador.Cargas.CargaValePedagio> _BuscarPorCargas(List<int> codigosCarga)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaValePedagio>();
            query = query.Where(obj => codigosCarga.Contains(obj.Carga.Codigo));
            return query;
        }

        public List<Dominio.ObjetosDeValor.Embarcador.Carga.CargaValePedagio> BuscarParaGeracaoEDIPorPagamento(List<int> codigosCarga)
        {
            var query = _BuscarPorCargas(codigosCarga);
            var queryIntegracao = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.ValePedagio.CargaIntegracaoValePedagio>();

            return query
                .Join(queryIntegracao, o => o.NumeroComprovante, a => a.NumeroValePedagio, (vp, integracao) => new { vp, integracao })
                .GroupBy(obj => new
                {
                    CodigoCargaValePedagio = obj.vp.Codigo,
                    NumeroValePedagio = obj.vp.NumeroComprovante,
                    ValorValePedagio = obj.vp.Valor,
                    DataEmissao = obj.integracao.DataIntegracao,
                    CNPJEmpresa = obj.vp.Carga.Empresa.CNPJ,
                    NumeroCarga = obj.vp.Carga.CodigoCargaEmbarcador,
                    CodigoCarga = obj.vp.Carga.Codigo
                })
                .Select(obj => new Dominio.ObjetosDeValor.Embarcador.Carga.CargaValePedagio
                {
                    CodigoCargaValePedagio = obj.Key.CodigoCargaValePedagio,
                    NumeroValePedagio = obj.Key.NumeroValePedagio,
                    ValorValePedagio = obj.Key.ValorValePedagio,
                    DataEmissao = obj.Key.DataEmissao,
                    CNPJEmpresa = obj.Key.CNPJEmpresa,
                    NumeroCarga = obj.Key.NumeroCarga,
                    CodigoCarga = obj.Key.CodigoCarga
                })
                .ToList();
        }

        #endregion
    }
}
