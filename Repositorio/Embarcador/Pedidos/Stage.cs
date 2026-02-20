using NHibernate.Linq;
using NHibernate.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Repositorio.Embarcador.Pedidos
{
    public class Stage : RepositorioBase<Dominio.Entidades.Embarcador.Pedidos.Stage>
    {
        #region Construtores

        public Stage(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Stage(UnitOfWork unitOfWork, CancellationToken cancellationToken) : base(unitOfWork, cancellationToken) { }

        #endregion

        #region Métodos Públicos

        #endregion
        #region Metodos Publicos
        public List<int> BuscarStagesAgProcessamento(int numeroRegistrosPorVez)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.Stage>();
            var result = from obj in query
                         where obj.Processado == true
                         select obj;
            return result
                .OrderBy(o => o.Codigo)
                .Select(o => o.Codigo)
                .Skip(0)
                .Take(numeroRegistrosPorVez)
                .ToList();
        }

        public Dominio.Entidades.Embarcador.Pedidos.Stage BuscarStagePorNumeroECargaDT(string numero, int carga)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.Stage>();
            var result = from obj in query
                         where obj.NumeroStage == numero && obj.CargaDT.Codigo == carga
                         select obj;

            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Pedidos.Stage> BuscarStagesPorNumeroECargaDT(List<string> numero, int carga)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.Stage>();
            var result = from obj in query
                         where numero.Contains(obj.NumeroStage) && obj.CargaDT.Codigo == carga
                         select obj;

            return result
                .Fetch(obj => obj.CanalEntrega)
                .Fetch(obj => obj.CanalVenda)
                .Fetch(obj => obj.Recebedor)
                .ThenFetch(obj => obj.Localidade)
                .Fetch(obj => obj.Expedidor)
                .ThenFetch(obj => obj.Localidade)
                .ToList();
        }

        public int BuscarQuantidadeStageNumaCarga(int codigoCarga)
        {
            IQueryable<Dominio.Entidades.Embarcador.Pedidos.Stage> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.Stage>();
            var result = from obj in query
                         where obj.CargaDT.Codigo == codigoCarga
                         select obj;

            return result.Count();

        }

        public List<Dominio.Entidades.Embarcador.Pedidos.Stage> BuscarporAgrupamento(int codigoAgrupamentoStage)
        {
            IQueryable<Dominio.Entidades.Embarcador.Pedidos.Stage> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.Stage>();
            var result = from obj in query
                         where obj.StageAgrupamento.Codigo == codigoAgrupamentoStage
                         select obj;

            return result.ToList();

        }

        public int ContarStagesPorAgrupamento(int codigoAgrupamentoStage)
        {
            IQueryable<Dominio.Entidades.Embarcador.Pedidos.Stage> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.Stage>();
            var result = from obj in query
                         where obj.StageAgrupamento.Codigo == codigoAgrupamentoStage
                         select obj;

            return result.Count();

        }

        public List<Dominio.Entidades.Embarcador.Pedidos.Stage> BuscarporCodigosAgrupamentos(List<int> codigosAgrupamentoStage)
        {
            IQueryable<Dominio.Entidades.Embarcador.Pedidos.Stage> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.Stage>();
            var result = from obj in query
                         where codigosAgrupamentoStage.Contains(obj.StageAgrupamento.Codigo)
                         select obj;

            return result.ToList();

        }

        public void AtualizarStageProcessadoPorAgrupamento(int codigoAgrupamento, bool status)
        {
            UnitOfWork.Sessao.CreateQuery("UPDATE Stage SET Processado = :processando WHERE StageAgrupamento.Codigo = :codigoAgrupamento")
               .SetInt32("codigoAgrupamento", codigoAgrupamento)
               .SetBoolean("processando", status)
               .ExecuteUpdate();
        }

        public void AtualizarStageProcessadoPorCargaDT(int codigoCarga, bool status)
        {
            UnitOfWork.Sessao.CreateQuery("UPDATE Stage SET Processado = :processando WHERE CargaDT.Codigo = :codigoCarga")
               .SetInt32("codigoCarga", codigoCarga)
               .SetBoolean("processando", status)
               .ExecuteUpdate();
        }

        public bool BuscarQuantidadeStage()
        {
            var query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.Stage>().FirstOrDefault();
            return query != null;
        }

        public async Task<bool> BuscarQuantidadeStageAsync()
        {
            var query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.Stage>();
            return await query.FirstOrDefaultAsync() != null;
        }

        public List<Dominio.Entidades.Embarcador.Pedidos.Stage> BuscarPorCargaCTe(int codigoCarga, int codigoCTe)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.Stage>()
                .Where(o => o.CargaDT.Codigo == codigoCarga);

            var consultaNotaFiscalPorCte = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedidoXMLNotaFiscalCTe>()
                .Where(notaFiscalCte => notaFiscalCte.CargaCTe.Carga.Codigo == codigoCarga && notaFiscalCte.CargaCTe.CTe.Codigo == codigoCTe);

            var consultaDocumentoProvisao = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Escrituracao.DocumentoProvisao>()
                .Where(documento => documento.Carga.Codigo == codigoCarga && consultaNotaFiscalPorCte.Any(notaFiscalCte => notaFiscalCte.PedidoXMLNotaFiscal.XMLNotaFiscal.Codigo == documento.XMLNotaFiscal.Codigo));

            query = query.Where(stage => consultaDocumentoProvisao.Select(doc => doc.Stage.Codigo).Contains(stage.Codigo));

            return query.ToList();
        }

        public Dominio.Entidades.Embarcador.Pedidos.Stage BuscarPrimeiraPorCarga(int codigoCarga, bool? relevanteVP = null)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.Stage>()
                .Where(o => o.CargaDT.Codigo == codigoCarga);

            if (relevanteVP.HasValue)
                query = query.Where(x => x.RelevanteVP == relevanteVP);

            return query.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Pedidos.Stage> BuscarStagesPorCarga(int codigoCarga, bool? relevanteVP = null)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.Stage>()
                .Where(o => o.CargaDT.Codigo == codigoCarga);

            if (relevanteVP.HasValue)
                query = query.Where(x => x.RelevanteVP == relevanteVP);

            return query.Fetch(x => x.StageAgrupamento).ToList();
        }

        public List<Dominio.Entidades.Embarcador.Pedidos.Stage> BuscarStagesPorCargaSemPercuso(int codigoCarga, bool? relevanteVP = null)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.Stage>()
                .Where(o => o.CargaDT.Codigo == codigoCarga && o.TipoPercurso != Dominio.ObjetosDeValor.Embarcador.Enumeradores.Vazio.PercursoRegreso);

            if (relevanteVP.HasValue)
                query = query.Where(x => x.RelevanteVP == relevanteVP);

            return query.ToList();
        }

        public List<string> ObterNumerosStagesPorAgrupamento(int codigoAgrupamento)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.Stage>();
            query = from obj in query where obj.StageAgrupamento.Codigo == codigoAgrupamento select obj;

            return query?.ToList()?.Select(s => s.NumeroStage)?.ToList() ?? new List<string>();

        }

        public List<string> ObterNumerosStagesPorAgrupamento(List<int> codigoAgrupamento)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.Stage>();
            query = from obj in query where codigoAgrupamento.Contains(obj.StageAgrupamento.Codigo) select obj;

            return query?.ToList()?.Select(s => s.NumeroStage)?.ToList() ?? new List<string>();

        }
        #endregion
    }
}
