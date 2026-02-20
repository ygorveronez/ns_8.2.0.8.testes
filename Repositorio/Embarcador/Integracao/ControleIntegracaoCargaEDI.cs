using System.Collections.Generic;
using System.Linq;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using NHibernate.Linq;
using System.Linq.Dynamic.Core;
using System;
using System.Threading;

namespace Repositorio.Embarcador.Integracao
{
    public class ControleIntegracaoCargaEDI : RepositorioBase<Dominio.Entidades.Embarcador.Integracao.ControleIntegracaoCargaEDI>
    {
        #region Construtores

        public ControleIntegracaoCargaEDI(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public ControleIntegracaoCargaEDI(UnitOfWork unitOfWork, CancellationToken cancellationToken) : base(unitOfWork, cancellationToken)
        {
        }

        #endregion

        #region Métodos Privados 

        private IQueryable<Dominio.Entidades.Embarcador.Integracao.ControleIntegracaoCargaEDI> Consultar(Dominio.ObjetosDeValor.Embarcador.Integracao.FiltroPesquisaControleIntegracaoCargaEDI filtrosPesquisa)
        {
            var consultaIntegracoes = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Integracao.ControleIntegracaoCargaEDI>();

            if (!filtrosPesquisa.TelaCarga)
                consultaIntegracoes = consultaIntegracoes.Where(o => o.ArquivoImportacaoPedido == false);

            if (filtrosPesquisa.CodigoCarga > 0)
                consultaIntegracoes = consultaIntegracoes.Where(o => o.Cargas.Any(obj => obj.Codigo == filtrosPesquisa.CodigoCarga));

            if (!string.IsNullOrWhiteSpace(filtrosPesquisa.CodigoCargaEmbarcador))
                consultaIntegracoes = consultaIntegracoes.Where(o => o.NumerosDTs.Any(obj => obj == filtrosPesquisa.CodigoCargaEmbarcador) || o.NumeroDT == filtrosPesquisa.CodigoCargaEmbarcador);

            if (filtrosPesquisa.CodigoTransportador > 0)
                consultaIntegracoes = consultaIntegracoes.Where(o => o.Transportador.Codigo == filtrosPesquisa.CodigoTransportador);

            if (filtrosPesquisa.DataInicial.HasValue)
                consultaIntegracoes = consultaIntegracoes.Where(o => o.Data.Date >= filtrosPesquisa.DataInicial.Value);

            if (filtrosPesquisa.DataFinal.HasValue)
                consultaIntegracoes = consultaIntegracoes.Where(o => o.Data.Date <= filtrosPesquisa.DataFinal.Value);

            if (!string.IsNullOrWhiteSpace(filtrosPesquisa.IDOC))
                consultaIntegracoes = consultaIntegracoes.Where(o => o.IDOC == filtrosPesquisa.IDOC);

            if (!string.IsNullOrWhiteSpace(filtrosPesquisa.NomeArquivo))
                consultaIntegracoes = consultaIntegracoes.Where(o => o.NomeArquivo.Contains(filtrosPesquisa.NomeArquivo));

            if (!string.IsNullOrWhiteSpace(filtrosPesquisa.Placa))
                consultaIntegracoes = consultaIntegracoes.Where(o => o.Placa == filtrosPesquisa.Placa);

            if (filtrosPesquisa.Situacao != SituacaoIntegracaoCargaEDI.Todas)
                consultaIntegracoes = consultaIntegracoes.Where(o => o.SituacaoIntegracaoCargaEDI == filtrosPesquisa.Situacao);

            if (filtrosPesquisa.HoraInicial.HasValue || filtrosPesquisa.HoraFinal.HasValue)
            {
                var listaIntegracoes = consultaIntegracoes.ToList();

                if (filtrosPesquisa.HoraInicial.HasValue && filtrosPesquisa.HoraFinal.HasValue)
                {
                    if (filtrosPesquisa.HoraInicial.Value > filtrosPesquisa.HoraFinal.Value)
                        consultaIntegracoes = (from integracao in listaIntegracoes where (integracao.Data.TimeOfDay >= filtrosPesquisa.HoraInicial.Value) || (integracao.Data.TimeOfDay <= filtrosPesquisa.HoraFinal.Value) select integracao).AsQueryable();
                    else
                        consultaIntegracoes = (from integracao in listaIntegracoes where (integracao.Data.TimeOfDay >= filtrosPesquisa.HoraInicial.Value) && (integracao.Data.TimeOfDay <= filtrosPesquisa.HoraFinal.Value) select integracao).AsQueryable();
                }
                else
                {
                    if (filtrosPesquisa.HoraInicial.HasValue)
                        consultaIntegracoes = (from integracao in listaIntegracoes where (integracao.Data.TimeOfDay >= filtrosPesquisa.HoraInicial.Value) select integracao).AsQueryable();

                    if (filtrosPesquisa.HoraFinal.HasValue)
                        consultaIntegracoes = (from integracao in listaIntegracoes where (integracao.Data.TimeOfDay <= filtrosPesquisa.HoraFinal.Value) select integracao).AsQueryable();
                }
            }

            return consultaIntegracoes;
        }

        #endregion

        #region Métodos Públicos

        public List<Dominio.Entidades.Embarcador.Integracao.ControleIntegracaoCargaEDI> BuscarPendenteIntegracao(int inicio, int limite)
        {
            var listaIntegracoes = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Integracao.ControleIntegracaoCargaEDI>()
                .Where(o => o.SituacaoIntegracaoCargaEDI == SituacaoIntegracaoCargaEDI.AgIntegracao)
                .OrderBy(o => o.Data)
                .Skip(inicio)
                .Take(limite)
                .Fetch(obj => obj.LayoutEDI)
                .ToList();

            return listaIntegracoes;
        }

        public List<Dominio.Entidades.Embarcador.Integracao.ControleIntegracaoCargaEDI> BuscarPendenteIntegracaoEnvio(int inicio, int limite)
        {
            List<SituacaoCarga> situacaoCargasNaoPermitidas = new List<SituacaoCarga>() { SituacaoCarga.Cancelada, SituacaoCarga.Anulada };

            var listaIntegracoes = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Integracao.ControleIntegracaoCargaEDI>()
                .Where(o => o.SituacaoIntegracaoCargaEDI == SituacaoIntegracaoCargaEDI.AgIntegracao && o.TipoIntegracao != null && o.Cargas.Any(obj => obj.AutorizouTodosCTes && !situacaoCargasNaoPermitidas.Contains(obj.SituacaoCarga)))
                .OrderBy(o => o.Data)
                .Skip(inicio)
                .Take(limite)
                .Fetch(obj => obj.LayoutEDI)
                .ToList();

            return listaIntegracoes;
        }

        public List<Dominio.Entidades.Embarcador.Integracao.ControleIntegracaoCargaEDI> BuscarPendenteIntegracaoSemPrioridade(int inicio, int limite)
        {
            var listaIntegracoes = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Integracao.ControleIntegracaoCargaEDI>()
                .Where(o => o.SituacaoIntegracaoCargaEDI == SituacaoIntegracaoCargaEDI.AgIntegracao)
                .OrderByDescending(o => o.Prioritario)
                .OrderBy(o => o.Data)
                .Skip(inicio)
                .Take(limite)
                 .Fetch(obj => obj.LayoutEDI)
                .ToList();

            return listaIntegracoes;
        }

        public List<Dominio.Entidades.Embarcador.Integracao.ControleIntegracaoCargaEDI> BuscarPendenteIntegracaoPrioritario(int inicio, int limite)
        {
            var listaIntegracoes = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Integracao.ControleIntegracaoCargaEDI>()
                .Where(o => o.SituacaoIntegracaoCargaEDI == SituacaoIntegracaoCargaEDI.AgIntegracao && o.Prioritario == true)
                .OrderByDescending(o => o.Prioritario)
                .OrderBy(o => o.Data)
                .Skip(inicio)
                .Take(limite)
                .ToList();

            return listaIntegracoes;
        }

        public List<Dominio.Entidades.Embarcador.Integracao.ControleIntegracaoCargaEDI> BuscarPorTransportadorTipoLayoutEDIPeriodo(int codigoTransportador, Dominio.Enumeradores.TipoLayoutEDI tipoLayoutEDI, DateTime dataInicio, DateTime dataFim)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Integracao.ControleIntegracaoCargaEDI>()
                .Where(o => o.Data >= dataInicio && o.Data <= dataFim)
                .Where(o => o.LayoutEDI.Tipo == tipoLayoutEDI);

            if (codigoTransportador > 0)
            {
                query = query.Where(o => o.Transportador.Codigo == codigoTransportador || o.Transportador.Matriz.Any(t => t.Codigo == codigoTransportador));
            }

            var listaIntegracoes = query.ToList();
            return listaIntegracoes;
        }

        public List<Dominio.Entidades.Embarcador.Integracao.ControleIntegracaoCargaEDI> BuscarPorDT(string numeroDT, int codigoControleAtual)
        {
            var integracao = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Integracao.ControleIntegracaoCargaEDI>()
                .Where(o => o.NumeroDT == numeroDT && o.SituacaoIntegracaoCargaEDI == SituacaoIntegracaoCargaEDI.AgIntegracao && o.Codigo != codigoControleAtual)
                .ToList();

            return integracao;
        }

        public Dominio.Entidades.Embarcador.Integracao.ControleIntegracaoCargaEDI BuscarPorCodigo(int codigo)
        {
            var integracao = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Integracao.ControleIntegracaoCargaEDI>()
                .Where(o => o.Codigo == codigo)
                .FirstOrDefault();

            return integracao;
        }

        public List<Dominio.Entidades.Embarcador.Integracao.ControleIntegracaoCargaEDI> Consultar(Dominio.ObjetosDeValor.Embarcador.Integracao.FiltroPesquisaControleIntegracaoCargaEDI filtrosPesquisa, string propriedadeOrdenar, string direcaoOrdenacao, int inicioRegistros, int maximoRegistros)
        {
            var consultaIntegracoes = Consultar(filtrosPesquisa);

            return ObterLista(consultaIntegracoes, propriedadeOrdenar, direcaoOrdenacao, inicioRegistros, maximoRegistros);
        }

        public List<Dominio.Entidades.Embarcador.Integracao.ControleIntegracaoCargaEDI> ConsultarPorCarga(int codigoCarga)
        {
            var consultaIntegracoes = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Integracao.ControleIntegracaoCargaEDI>();

            if (codigoCarga > 0)
                consultaIntegracoes = consultaIntegracoes.Where(o => o.Cargas.Any(obj => obj.Codigo == codigoCarga));

            return consultaIntegracoes.ToList();
        }

        public int ContarConsulta(Dominio.ObjetosDeValor.Embarcador.Integracao.FiltroPesquisaControleIntegracaoCargaEDI filtrosPesquisa)
        {
            var consultaIntegracoes = Consultar(filtrosPesquisa);

            return consultaIntegracoes.Count();
        }

        public List<Dominio.Entidades.Embarcador.Integracao.ControleIntegracaoCargaEDI> ConsultarReenvio(Dominio.ObjetosDeValor.Embarcador.Integracao.FiltroPesquisaControleIntegracaoCargaEDI filtrosPesquisa)
        {
            var consultaIntegracoes = Consultar(filtrosPesquisa);

            return consultaIntegracoes.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Integracao.ControleIntegracaoCargaEDI> ConsultarPorCargaComLayoutEDI(int codigoCarga)
        {
            var consultaIntegracoes = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Integracao.ControleIntegracaoCargaEDI>();

            if (codigoCarga > 0)
                consultaIntegracoes = consultaIntegracoes.Where(o => o.Cargas.Any(obj => obj.Codigo == codigoCarga));

            consultaIntegracoes = consultaIntegracoes.Where(o => o.LayoutEDI != null);
            return consultaIntegracoes.ToList();
        }

        public int ContarPorCarga(int codigoCarga, SituacaoIntegracaoCargaEDI situacao)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Integracao.ControleIntegracaoCargaEDI>();
            var result = from obj in query where obj.Cargas.Any(o => o.Codigo == codigoCarga) && obj.SituacaoIntegracaoCargaEDI == situacao select obj;
            return result.Count();
        }

        #endregion
    }
}
