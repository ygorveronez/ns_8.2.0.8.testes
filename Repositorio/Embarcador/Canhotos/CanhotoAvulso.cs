using NHibernate.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;


namespace Repositorio.Embarcador.Canhotos
{
    public class CanhotoAvulso : RepositorioBase<Dominio.Entidades.Embarcador.Canhotos.CanhotoAvulso>
    {
        public CanhotoAvulso(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.Embarcador.Canhotos.CanhotoAvulso BuscarPorCodigo(int codigo)
        {
            IQueryable<Dominio.Entidades.Embarcador.Canhotos.CanhotoAvulso> query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Canhotos.CanhotoAvulso>();
            IQueryable<Dominio.Entidades.Embarcador.Canhotos.CanhotoAvulso> result = query.Where(obj => obj.Codigo == codigo);
            return result.FirstOrDefault();
        }

        public int BuscarProximoCodigo()
        {
            IQueryable<Dominio.Entidades.Embarcador.Canhotos.CanhotoAvulso> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Canhotos.CanhotoAvulso>();

            int? retorno = query.Max(o => (int?)o.Numero);

            return retorno.HasValue ? retorno.Value + 1 : 1;
        }

        public List<Dominio.Entidades.Embarcador.Canhotos.Canhoto> BuscarCanhotoPorCarga(int carga)
        {
            IQueryable<Dominio.Entidades.Embarcador.Canhotos.Canhoto> query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Canhotos.Canhoto>();
            IQueryable<Dominio.Entidades.Embarcador.Canhotos.Canhoto> result = query.Where(obj => obj.TipoCanhoto == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCanhoto.Avulso && obj.Carga.Codigo == carga && obj.CanhotoAvulso.Ativo);
            return result.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Canhotos.CanhotoAvulso> BuscarPorCargaPedido(int codigoCargaPedido)
        {
            IQueryable<Dominio.Entidades.Embarcador.Canhotos.CanhotoAvulso> query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Canhotos.CanhotoAvulso>();
            query = query.Where(obj => obj.PedidosXMLNotasFiscais.Any(xml => xml.CargaPedido.Codigo == codigoCargaPedido));

            return query
                .Select(obj => obj)
                .ToList();
        }

        public List<Dominio.Entidades.Embarcador.Canhotos.CanhotoAvulso> BuscarPorPedidoXMLNotaFiscal(int codigoPedidoXMLNotaFiscal)
        {
            IQueryable<Dominio.Entidades.Embarcador.Canhotos.CanhotoAvulso> consultaCanhotoAvulso = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Canhotos.CanhotoAvulso>()
                .Where(canhoto => canhoto.PedidosXMLNotasFiscais.Any(notaPedido => notaPedido.Codigo == codigoPedidoXMLNotaFiscal));

            return consultaCanhotoAvulso.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Canhotos.CanhotoAvulso> BuscarTodosPorCarga(int carga)
        {
            IQueryable<Dominio.Entidades.Embarcador.Canhotos.Canhoto> query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Canhotos.Canhoto>();
            IQueryable<Dominio.Entidades.Embarcador.Canhotos.Canhoto> result = query.Where(obj => obj.TipoCanhoto == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCanhoto.Avulso && obj.Carga.Codigo == carga && obj.CanhotoAvulso.Ativo);
            return result.Select(obj => obj.CanhotoAvulso).ToList();
        }

        public List<Dominio.Entidades.Embarcador.Canhotos.Canhoto> ConsultarPorCarga(int carga, string propOrdenacao, string dirOrdenacao, int inicioRegistros, int maximoRegistros)
        {
            IQueryable<Dominio.Entidades.Embarcador.Canhotos.Canhoto> query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Canhotos.Canhoto>();
            IQueryable<Dominio.Entidades.Embarcador.Canhotos.Canhoto> result = query.Where(obj => obj.TipoCanhoto == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCanhoto.Avulso && obj.Carga.Codigo == carga && obj.CanhotoAvulso.Ativo);

            return result.OrderBy(propOrdenacao + (dirOrdenacao == "asc" ? " ascending" : " descending"))
                .Fetch(obj => obj.Destinatario)
                .Fetch(obj => obj.CanhotoAvulso)
                .Fetch(obj => obj.XMLNotaFiscal)
                .Skip(inicioRegistros)
                .Take(maximoRegistros).ToList();
        }

        public List<Dominio.Entidades.Embarcador.Canhotos.Canhoto> ConsultarPorProtocoloCarga(int protocoloCarga, string propOrdenacao, string dirOrdenacao, int inicioRegistros, int maximoRegistros)
        {
            IQueryable<Dominio.Entidades.Embarcador.Canhotos.Canhoto> query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Canhotos.Canhoto>();

            query = query.Where(obj => obj.TipoCanhoto == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCanhoto.Avulso && obj.Carga.Protocolo == protocoloCarga && obj.CanhotoAvulso.Ativo);

            return query.OrderBy(propOrdenacao + (dirOrdenacao == "asc" ? " ascending" : " descending"))
                        .Fetch(obj => obj.Destinatario)
                        .Fetch(obj => obj.CanhotoAvulso)
                        .Fetch(obj => obj.XMLNotaFiscal)
                        .Skip(inicioRegistros)
                        .Take(maximoRegistros).ToList();
        }

        public int ContarConsultaPorProtocoloCarga(int protocoloCarga)
        {
            IQueryable<Dominio.Entidades.Embarcador.Canhotos.Canhoto> query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Canhotos.Canhoto>();

            query = query.Where(obj => obj.TipoCanhoto == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCanhoto.Avulso && obj.Carga.Protocolo == protocoloCarga && obj.CanhotoAvulso.Ativo);

            return query.Count();
        }

        public int ContarConsultaPorCarga(int carga)
        {
            IQueryable<Dominio.Entidades.Embarcador.Canhotos.Canhoto> query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Canhotos.Canhoto>();
            IQueryable<Dominio.Entidades.Embarcador.Canhotos.Canhoto> result = query.Where(obj => obj.TipoCanhoto == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCanhoto.Avulso && obj.Carga.Codigo == carga && obj.CanhotoAvulso.Ativo);

            return result.Count();
        }

        public Dominio.Entidades.Embarcador.Canhotos.Canhoto BuscarCanhotoPorNumeroCanhotoAvulso(int numeroCanhotoAvulso)
        {
            IQueryable<Dominio.Entidades.Embarcador.Canhotos.Canhoto> query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Canhotos.Canhoto>();
            IQueryable<Dominio.Entidades.Embarcador.Canhotos.Canhoto> result = query.Where(obj => obj.TipoCanhoto == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCanhoto.Avulso && obj.CanhotoAvulso.Numero == numeroCanhotoAvulso && obj.CanhotoAvulso.Ativo);

            return result.FirstOrDefault();
        }

        public DateTime BuscarDataFaturamentoPrimeiraNota(int codigoCanhotoAvulso)
        {
            return SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Canhotos.CanhotoAvulso>()
                .Where(canhoto => canhoto.Codigo == codigoCanhotoAvulso)
                .Select(canhoto => canhoto.PedidosXMLNotasFiscais.Select(x => x.XMLNotaFiscal.DataEmissao).FirstOrDefault())
                .FirstOrDefault();
        }
    }
}
