using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;

namespace Repositorio.Embarcador.Cargas.ControleEntrega
{
    public class CargaEntregaCheckListPergunta : RepositorioBase<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaCheckListPergunta>
    {
        public CargaEntregaCheckListPergunta(UnitOfWork unitOfWork) : base(unitOfWork) { }

        /// <summary>
        /// O padrão tipoCheckList é Coleta, pois antigamente todos eram de Coleta
        /// </summary>
        /// <param name="codigoCargaEntrega"></param>
        /// <param name=""></param>
        /// <returns></returns>
        public List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaCheckListPergunta> BuscarPerguntasOrdenadasPorCargaEntrega(int codigoCargaEntrega, TipoCheckList tipoCheckList = TipoCheckList.Coleta)
        {
            var query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaCheckListPergunta>();
            var result = query.Where(obj => obj.CargaEntregaCheckList.CargaEntrega.Codigo == codigoCargaEntrega && obj.CargaEntregaCheckList.TipoCheckList == tipoCheckList);
            result = result.OrderBy(o => o.Ordem);
            return result.ToList();
        }

        public bool PossuiChecklistPorCargasEntrega(List<int> codigosCargaEntrega, TipoCheckList tipoCheckList = TipoCheckList.Coleta, List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaCheckListPergunta> lstCargaEntregaCheckListPergunta = null)
        {
            var query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaCheckListPergunta>();
            if (lstCargaEntregaCheckListPergunta != null)
                query = lstCargaEntregaCheckListPergunta.AsQueryable();
            var result = query.Where(obj => codigosCargaEntrega.Contains(obj.CargaEntregaCheckList.CargaEntrega.Codigo) && obj.CargaEntregaCheckList.TipoCheckList == tipoCheckList);
            return result.Any();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaCheckListPergunta> BuscarPerguntasOrdenadasPorCargaEntregaCheckList(int codigoCargaEntregaCheckList)
        {
            var query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaCheckListPergunta>();
            var result = query.Where(obj => obj.CargaEntregaCheckList.Codigo == codigoCargaEntregaCheckList);
            result = result.OrderBy(o => o.Ordem);
            return result.ToList();
        }

        public Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaCheckListPergunta BuscarPorCodigoECheckList(int pergunta, int checklist)
        {
            var query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaCheckListPergunta>();
            var result = query.Where(obj => obj.CargaEntregaCheckList.Codigo == checklist && obj.Codigo == pergunta);
            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaCheckListPergunta> BuscarPorCargasEntregas(List<int> codigosCargaEntrega)
        {
            IQueryable<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaCheckListPergunta> query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaCheckListPergunta>();

            int quantidadeRegistrosConsultarPorVez = 1000;
            int quantidadeConsultas = codigosCargaEntrega.Count / quantidadeRegistrosConsultarPorVez;

            List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaCheckListPergunta> cargaEntregaCheckListPerguntas = new List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaCheckListPergunta>();

            for (int i = 0; i <= quantidadeConsultas; i++)
                cargaEntregaCheckListPerguntas.AddRange(query.Where(o => codigosCargaEntrega.Skip(i * quantidadeRegistrosConsultarPorVez).Take(quantidadeRegistrosConsultarPorVez).Contains(o.CargaEntregaCheckList.CargaEntrega.Codigo)).ToList());

            return cargaEntregaCheckListPerguntas;
        }

    }
}
