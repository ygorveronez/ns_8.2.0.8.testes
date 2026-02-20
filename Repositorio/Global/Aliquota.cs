using Dominio.ObjetosDeValor.Embarcador.Bidding.ImportacaoRota;
using NHibernate.Linq;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading;
using System.Threading.Tasks;

namespace Repositorio
{
    public class Aliquota : RepositorioBase<Dominio.Entidades.Aliquota>, Dominio.Interfaces.Repositorios.Aliquota
    {
        public Aliquota(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Aliquota(UnitOfWork unitOfWork, CancellationToken cancellationToken) : base(unitOfWork, cancellationToken) { }

        public Dominio.Entidades.Aliquota BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Aliquota>();

            var result = from
                             obj in query
                         where
                               obj.Codigo == codigo
                         select obj;

            return result.FirstOrDefault();
        }

        private IQueryable<Dominio.Entidades.Aliquota> _Consultar(string ufEmpresa, string ufOrigem, string ufDestino, string atividadeTomador, int CFOP)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Aliquota>();

            var result = from obj in query select obj;

            // Filtros
            if (!string.IsNullOrWhiteSpace(ufEmpresa))
                result = result.Where(o => o.EstadoEmpresa.Sigla == (ufEmpresa));

            if (!string.IsNullOrWhiteSpace(ufOrigem))
                result = result.Where(o => o.EstadoOrigem.Sigla == (ufOrigem));

            if (!string.IsNullOrWhiteSpace(ufDestino))
                result = result.Where(o => o.EstadoDestino.Sigla == (ufDestino));

            if (!string.IsNullOrWhiteSpace(atividadeTomador))
                result = result.Where(o => o.AtividadeTomador.Descricao == (atividadeTomador));

            if (CFOP > 0)
                result = result.Where(o => o.CFOP.CodigoCFOP == (CFOP));

            return result;
        }

        public Task<Dominio.Entidades.Aliquota> BuscarPorOrigemDestinoAsync(string ufOrigem, string ufDestino)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Aliquota>();

            if (!string.IsNullOrWhiteSpace(ufOrigem))
                query = query.Where(o => o.EstadoOrigem.Sigla == (ufOrigem));

            if (!string.IsNullOrWhiteSpace(ufDestino))
                query = query.Where(o => o.EstadoDestino.Sigla == (ufDestino));

            return query.FirstOrDefaultAsync();
        }

        public List<Dominio.Entidades.Aliquota> BuscarPorUfEmpresa(string ufEmpresa)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Aliquota>();
            query = query.Where(o => o.EstadoEmpresa.Sigla == (ufEmpresa));
            return query.Fetch(o => o.CFOP).ThenFetch(o => o.NaturezaDaOperacao).ToList();
        }

        public List<Dominio.Entidades.Aliquota> Consultar(string ufEmpresa, string ufOrigem, string ufDestino, string atividadeTomador, int CFOP, string propOrdena, string dirOrdena, int inicioRegistros, int maximoRegistros)
        {
            var result = _Consultar(ufEmpresa, ufOrigem, ufDestino, atividadeTomador, CFOP);

            if (!string.IsNullOrWhiteSpace(propOrdena))
                result = result.OrderBy(propOrdena + (dirOrdena == "asc" ? " ascending" : " descending"));

            if (inicioRegistros > 0)
                result = result.Skip(inicioRegistros);

            if (maximoRegistros > 0)
                result = result.Take(maximoRegistros);

            return result.ToList();
        }

        public int ContarConsulta(string ufEmpresa, string ufOrigem, string ufDestino, string atividadeTomador, int CFOP)
        {
            var result = _Consultar(ufEmpresa, ufOrigem, ufDestino, atividadeTomador, CFOP);

            return result.Count();
        }

        public Dominio.Entidades.Aliquota BuscarParaCalculoDoICMS(string ufEmitente, string ufOrigem, string ufDestino, int codigoAtividadeTomador)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Aliquota>();

            var result = from
                             obj in query
                         where
                               obj.EstadoEmpresa.Sigla.Equals(ufEmitente) &&
                               obj.EstadoOrigem.Sigla.Equals(ufOrigem) &&
                               obj.EstadoDestino.Sigla.Equals(ufDestino) &&
                               obj.AtividadeTomador.Codigo == codigoAtividadeTomador
                         select obj;

            return result.FirstOrDefault();
        }
        
        public Task<Dominio.Entidades.Aliquota> BuscarParaCalculoDoICMSAsync(string ufEmitente, string ufOrigem, string ufDestino, int codigoAtividadeTomador)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Aliquota>();

            var result = from
                             obj in query
                         where
                               obj.EstadoEmpresa.Sigla.Equals(ufEmitente) &&
                               obj.EstadoOrigem.Sigla.Equals(ufOrigem) &&
                               obj.EstadoDestino.Sigla.Equals(ufDestino) &&
                               obj.AtividadeTomador.Codigo == codigoAtividadeTomador
                         select obj;

            return result.FirstOrDefaultAsync();
        }

        public List<Dominio.Entidades.Aliquota> BuscaAliquotas(string ufOrigem, string ufDestino, int codigoAtividadeTomador)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Aliquota>();

            var result = from
                             obj in query
                         where
                               obj.EstadoOrigem.Sigla.Equals(ufOrigem) &&
                               obj.EstadoDestino.Sigla.Equals(ufDestino) &&
                               obj.AtividadeTomador.Codigo == codigoAtividadeTomador
                         select obj;

            return result.ToList();
        }

        public Dominio.Entidades.Aliquota BuscarParaCalculoDoICMS(string ufEmitente, string ufOrigem, string ufDestino, int codigoAtividadeRemetente, int codigoAtividadeDestinatario)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Aliquota>();

            var result = from
                             obj in query
                         where
                               obj.EstadoEmpresa.Sigla.Equals(ufEmitente) &&
                               obj.EstadoOrigem.Sigla.Equals(ufOrigem) &&
                               obj.EstadoDestino.Sigla.Equals(ufDestino) &&
                               obj.AtividadeTomador.Codigo == codigoAtividadeRemetente &&
                               obj.AtividadeDestinatario.Codigo == codigoAtividadeDestinatario
                         select obj;

            return result.Fetch(o => o.CFOP).ThenFetch(o => o.NaturezaDaOperacao).FirstOrDefault();
        }
        
        public Task<Dominio.Entidades.Aliquota> BuscarParaCalculoDoICMSAsync(string ufEmitente, string ufOrigem, string ufDestino, int codigoAtividadeRemetente, int codigoAtividadeDestinatario)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Aliquota>();

            var result = from
                             obj in query
                         where
                               obj.EstadoEmpresa.Sigla.Equals(ufEmitente) &&
                               obj.EstadoOrigem.Sigla.Equals(ufOrigem) &&
                               obj.EstadoDestino.Sigla.Equals(ufDestino) &&
                               obj.AtividadeTomador.Codigo == codigoAtividadeRemetente &&
                               obj.AtividadeDestinatario.Codigo == codigoAtividadeDestinatario
                         select obj;

            return result.Fetch(o => o.CFOP).ThenFetch(o => o.NaturezaDaOperacao).FirstOrDefaultAsync();
        }

        public List<Dominio.Relatorios.Embarcador.DataSource.Financeiros.AliquotaICMSCTe> ConsultarRelatorio(string ufEmitente, string ufOrigem, string ufDestino, string propAgrupa, string dirAgrupa, string propOrdena, string dirOrdena, int inicio, int limite)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Aliquota>();

            if (!string.IsNullOrWhiteSpace(ufEmitente) && ufEmitente != "0")
                query = query.Where(o => o.EstadoEmpresa.Sigla == ufEmitente);

            if (!string.IsNullOrWhiteSpace(ufDestino) && ufDestino != "0")
                query = query.Where(o => o.EstadoDestino.Sigla == ufDestino);

            if (!string.IsNullOrWhiteSpace(ufOrigem) && ufOrigem != "0")
                query = query.Where(o => o.EstadoOrigem.Sigla == ufOrigem);

            string ordenacao = string.Empty;

            if (!string.IsNullOrWhiteSpace(propAgrupa))
                ordenacao = propAgrupa + " " + dirAgrupa;

            if (propOrdena != propAgrupa)
                ordenacao += (!string.IsNullOrWhiteSpace(ordenacao) ? ", " : "") + propOrdena + " " + dirOrdena;

            query = query.OrderBy(ordenacao);

            if (inicio > 0 || limite > 0)
                query = query.Skip(inicio).Take(limite);

            return query.Select(o => new Dominio.Relatorios.Embarcador.DataSource.Financeiros.AliquotaICMSCTe()
            {
                Aliquota = o.Percentual,
                Atividade = o.AtividadeTomador.Codigo + " - " + o.AtividadeTomador.Descricao,
                AtividadeDestinatario = o.AtividadeDestinatario.Codigo + " - " + o.AtividadeDestinatario.Descricao,
                CFOP = o.CFOP.CodigoCFOP,
                CST = o.CST,
                Codigo = o.Codigo,
                EstadoDestino = o.EstadoDestino.Sigla + " - " + o.EstadoDestino.Nome,
                EstadoEmpresa = o.EstadoEmpresa.Sigla + " - " + o.EstadoEmpresa.Nome,
                EstadoOrigem = o.EstadoOrigem.Sigla + " - " + o.EstadoOrigem.Nome
            }).ToList();
        }

        public int ContarConsultaRelatorio(string ufEmitente, string ufOrigem, string ufDestino)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Aliquota>();

            if (!string.IsNullOrWhiteSpace(ufEmitente) && ufEmitente != "0")
                query = query.Where(o => o.EstadoEmpresa.Sigla == ufEmitente);

            if (!string.IsNullOrWhiteSpace(ufDestino) && ufDestino != "0")
                query = query.Where(o => o.EstadoDestino.Sigla == ufDestino);

            if (!string.IsNullOrWhiteSpace(ufOrigem) && ufOrigem != "0")
                query = query.Where(o => o.EstadoOrigem.Sigla == ufOrigem);

            return query.Count();
        }

        public decimal ObterPercetualAliquota(string ufEmitente, string ufOrigem, string ufDestino)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Aliquota>();

            if (!string.IsNullOrWhiteSpace(ufEmitente) && ufEmitente != "0")
                query = query.Where(o => o.EstadoEmpresa.Sigla == ufEmitente);

            if (!string.IsNullOrWhiteSpace(ufDestino) && ufDestino != "0")
                query = query.Where(o => o.EstadoDestino.Sigla == ufDestino);

            if (!string.IsNullOrWhiteSpace(ufOrigem) && ufOrigem != "0")
                query = query.Where(o => o.EstadoOrigem.Sigla == ufOrigem);

            return query.Select(o => o.Percentual).FirstOrDefault();
        }

        public Task<List<AliquotaICMS>> BuscarTodasAliquotasParaBidding()
        {
            IQueryable<Dominio.Entidades.Aliquota> query = this.SessionNHiBernate.Query<Dominio.Entidades.Aliquota>();

            return query
                .Select(o => new Dominio.ObjetosDeValor.Embarcador.Bidding.ImportacaoRota.AliquotaICMS()
                {
                    Aliquota = o.Percentual,
                    EstadoOrigem = o.EstadoOrigem.Sigla,
                    EstadoDestino = o.EstadoDestino.Sigla
                })
                .ToListAsync();
        }

    }
}
