using NHibernate.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading;

namespace Repositorio.Embarcador.CTe
{
    public class CancelamentoCTe : RepositorioBase<Dominio.Entidades.Embarcador.CTe.CancelamentoCTe>
    {
        public CancelamentoCTe(UnitOfWork unitOfWork) : base(unitOfWork) { }
        public CancelamentoCTe(UnitOfWork unitOfWork, CancellationToken cancellationToken) : base(unitOfWork, cancellationToken) { }

        #region Métodos Publicos
        public Dominio.Entidades.Embarcador.CTe.CancelamentoCTe BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.CTe.CancelamentoCTe>();

            var result = from obj in query where obj.Codigo == codigo select obj;

            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.CTe.CancelamentoCTe> BuscarPorCodigoCancelamento(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.CTe.CancelamentoCTe>();

            var result = from obj in query where obj.CancelamentoCTeSemCarga.Codigo == codigo select obj;

            return result.ToList();
        }


        public List<Dominio.Entidades.ConhecimentoDeTransporteEletronico> BuscarPorCodigoCancelamentoCTe(int codigoCancelamento)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.CTe.CancelamentoCTe>();

            query = query.Where(c => c.CancelamentoCTeSemCarga.Codigo == codigoCancelamento);

            var result = query.Fetch(o => o.CTe)
                              .Fetch(o => o.CTe).ThenFetch(o => o.Remetente).ThenFetch(o => o.Cliente)
                              .Fetch(o => o.CTe).ThenFetch(o => o.Destinatario).ThenFetch(o => o.Cliente)
                              .Fetch(o => o.CTe).ThenFetch(o => o.OutrosTomador).ThenFetch(o => o.Cliente)
                              .Fetch(o => o.CTe).ThenFetch(o => o.Expedidor).ThenFetch(o => o.Cliente)
                              .Fetch(o => o.CTe).ThenFetch(o => o.Recebedor).ThenFetch(o => o.Cliente)
                              .Fetch(o => o.CTe).ThenFetch(o => o.MensagemStatus)
                              .Fetch(o => o.CTe).ThenFetch(o => o.Serie)
                              .Fetch(o => o.CTe).ThenFetch(o => o.LocalidadeTerminoPrestacao)
                              .Fetch(o => o.CTe).ThenFetch(o => o.ModeloDocumentoFiscal).ToList();

            return result.Select(o => o.CTe).ToList();
        }


        public Dominio.Entidades.ConhecimentoDeTransporteEletronico BuscarCtePorCodigoCancelamento(int codigoCancelamento)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.CTe.CancelamentoCTe>();

            query = query.Where(c => c.Codigo == codigoCancelamento);

            var result = query.Fetch(o => o.CTe)
                              .Fetch(o => o.CTe).ThenFetch(o => o.Remetente).ThenFetch(o => o.Cliente)
                              .Fetch(o => o.CTe).ThenFetch(o => o.Destinatario).ThenFetch(o => o.Cliente)
                              .Fetch(o => o.CTe).ThenFetch(o => o.OutrosTomador).ThenFetch(o => o.Cliente)
                              .Fetch(o => o.CTe).ThenFetch(o => o.Expedidor).ThenFetch(o => o.Cliente)
                              .Fetch(o => o.CTe).ThenFetch(o => o.Recebedor).ThenFetch(o => o.Cliente)
                              .Fetch(o => o.CTe).ThenFetch(o => o.MensagemStatus)
                              .Fetch(o => o.CTe).ThenFetch(o => o.Serie)
                              .Fetch(o => o.CTe).ThenFetch(o => o.LocalidadeTerminoPrestacao)
                              .Fetch(o => o.CTe).ThenFetch(o => o.ModeloDocumentoFiscal).ToList();

            return result.Select(o => o.CTe).FirstOrDefault();
        }

        //public List<Dominio.Entidades.Embarcador.CTe.CancelamentoCTe> Consultar (Dominio.ObjetosDeValor.Embarcador.CTe.FiltroPesquisaCancelamentoCTeSemCarga filtrosPesquisa, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametroConsulta)
        //{
        //    var query = MontarConsulta(filtrosPesquisa);

        //    query = query.Fetch(obj => obj.CancelamentoCTe);

        //    return ObterLista(query, parametroConsulta);
        //}
        #endregion

        #region Métodos Privados
        //private IQueryable<Dominio.Entidades.Embarcador.CTe.CancelamentoCTeSemCarga> MontarConsulta(Dominio.ObjetosDeValor.Embarcador.CTe.FiltroPesquisaCancelamentoCTeSemCarga filtrosPesquisa)
        //{
        //    var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.CTe.CancelamentoCTeSemCarga>();

        //    if (filtrosPesquisa.DataInicial != DateTime.MinValue)
        //        query = query.Where(o => o.DataInclusao >= filtrosPesquisa.DataInicial.Date);

        //    if (filtrosPesquisa.DataFinal != DateTime.MinValue)
        //        query = query.Where(o => o.DataAtualizacao < filtrosPesquisa.DataFinal.Date);

        //    if (filtrosPesquisa.NumeroInicial > 0)
        //        query = query.Where(o => o.CancelamentoCTe.Where(o => o.CTes.Where(c => c.Numero >= filtrosPesquisa.NumeroInicial).Any()).Any());

        //    if (filtrosPesquisa.NumeroFinal> 0)
        //        query = query.Where(o => o.CancelamentoCTe.Where(o => o.CTes.Where(c => c.Numero >= filtrosPesquisa.NumeroFinal).Any()).Any());

        //    return query;
        //}
        #endregion 
    }
}
