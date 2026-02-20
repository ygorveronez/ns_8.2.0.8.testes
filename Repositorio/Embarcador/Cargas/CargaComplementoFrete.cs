using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
using NHibernate.Linq;

namespace Repositorio.Embarcador.Cargas
{
    public class CargaComplementoFrete : RepositorioBase<Dominio.Entidades.Embarcador.Cargas.CargaComplementoFrete>
    {
        public CargaComplementoFrete(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.Embarcador.Cargas.CargaComplementoFrete BuscarPorCodigoComponente(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaComplementoFrete>();
            var resut = from obj in query where obj.ComponenteFrete.Codigo == codigo select obj;
            return resut.FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.Cargas.CargaComplementoFrete BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaComplementoFrete>();
            var resut = from obj in query where obj.Codigo == codigo select obj;
            return resut.FirstOrDefault();
        }
        public async Task<Dominio.Entidades.Embarcador.Cargas.CargaComplementoFrete> BuscarPorCodigoAsync(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaComplementoFrete>();
            var result = from obj in query where obj.Codigo == codigo select obj;
            return await result.FirstOrDefaultAsync();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.CargaComplementoFrete> BuscarPorCarga(int carga)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaComplementoFrete>();
            var resut = from obj in query where obj.Carga.Codigo == carga select obj;
            return resut.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.CargaComplementoFrete> BuscarPorCargas(List<int> codigosCarga)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaComplementoFrete>();
            var resut = from obj in query where codigosCarga.Contains(obj.Carga.Codigo) select obj;
            return resut
                .Fetch(o => o.Carga)
                .ToList();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.CargaComplementoFrete> BuscarPorCargaSemComponenteCompoeFreteValor(int carga)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaComplementoFrete>();
            var resut = from obj in query where obj.Carga.Codigo == carga select obj;
            return resut.Fetch(obj => obj.ComponenteFrete).ToList();
        }

        public int ContarNumeroComplementosNaoExtornadosOuRejeitados(int carga)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaComplementoFrete>();
            var resut = from obj in query where obj.Carga.Codigo == carga && obj.SituacaoComplementoFrete != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoComplementoFrete.Estornada && obj.SituacaoComplementoFrete != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoComplementoFrete.Rejeitada select obj;
            return resut.Count();
        }


        public List<Dominio.Entidades.Embarcador.Cargas.CargaComplementoFrete> BuscarPorSituacao(Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoComplementoFrete situacaoComplementoFrete)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaComplementoFrete>();

            if (situacaoComplementoFrete != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoComplementoFrete.Todas)
                query = from obj in query where obj.SituacaoComplementoFrete == situacaoComplementoFrete select obj;
            return query.ToList();
        }

        public Dominio.Entidades.Embarcador.Cargas.CargaComplementoFrete BuscarPorCodigoSolicitacaoCredito(int codigoSolicitacao)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaComplementoFrete>();
            var resut = from obj in query where obj.SolicitacaoCredito.Codigo == codigoSolicitacao select obj;
            return resut.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.CargaComplementoFrete> BuscarComplementosPendentesEmissaoCTeComplementar(int carga)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaComplementoFrete>();
            var resut = from obj in query where obj.Carga.Codigo == carga && obj.SituacaoComplementoFrete == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoComplementoFrete.AgEmissaoCTeComplementar select obj;
            return resut.ToList();
        }


        public List<Dominio.Entidades.Embarcador.Cargas.CargaComplementoFrete> ConsultarRelatorioCargaComplementosFrete(DateTime dataInicio, DateTime dataFim, int codigoTransportador, int codigoVeiculo, int operador, int motivoAdicionalFrete, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoComplementoFrete situacaoComplementoFrete, List<int> codigosFilial, List<double> codigosRecebedor, List<int> codigosTipoCarga, List<int> codigosTipoOperacao, string propGrupo, string dirOrdenacaoGrupo, string propriedadeOrdenar, string direcaoOrdenar)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaComplementoFrete>();

            var result = MontarQueryRelatorio(query, dataInicio, dataFim, codigoTransportador, codigoVeiculo, operador, motivoAdicionalFrete, situacaoComplementoFrete, codigosFilial, codigosRecebedor, codigosTipoCarga, codigosTipoOperacao);

            if (!string.IsNullOrWhiteSpace(propGrupo) && propGrupo != propriedadeOrdenar)
                result = result.OrderBy(propGrupo + (dirOrdenacaoGrupo == "asc" ? " ascending" : " descending"));

            var retorno = result.OrderBy(propriedadeOrdenar + " " + direcaoOrdenar).ToList();

            return retorno;
        }

        public List<Dominio.Entidades.Embarcador.Cargas.CargaComplementoFrete> ConsultarRelatorioCargaComplementosFrete(DateTime dataInicio, DateTime dataFim, int codigoTransportador, int codigoVeiculo, int operador, int motivoAdicionalFrete, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoComplementoFrete situacaoComplementoFrete, List<int> codigosFilial, List<double> codigosRecebedor, List<int> codigosTipoCarga, List<int> codigosTipoOperacao, string propGrupo, string dirOrdenacaoGrupo, string propriedadeOrdenar, string direcaoOrdenar, int inicioRegistros, int maximoRegistros)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaComplementoFrete>();

            var result = MontarQueryRelatorio(query, dataInicio, dataFim, codigoTransportador, codigoVeiculo, operador, motivoAdicionalFrete, situacaoComplementoFrete, codigosFilial, codigosRecebedor, codigosTipoCarga, codigosTipoOperacao);

            if (!string.IsNullOrWhiteSpace(propGrupo) && propGrupo != propriedadeOrdenar)
                result = result.OrderBy(propGrupo + (dirOrdenacaoGrupo == "asc" ? " ascending" : " descending"));

            var retorno = result.OrderBy(propriedadeOrdenar + " " + direcaoOrdenar).Skip(inicioRegistros).Take(maximoRegistros).ToList();

            return retorno;
        }

        public int ContarConsultaRelatorioCargaComplementosFrete(DateTime dataInicio, DateTime dataFim, int codigoTransportador, int codigoVeiculo, int operador, int motivoAdicionalFrete, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoComplementoFrete situacaoComplementoFrete, List<int> codigosFilial, List<double> codigosRecebedor, List<int> codigosTipoCarga, List<int> codigosTipoOperacao)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaComplementoFrete>();

            var result = MontarQueryRelatorio(query, dataInicio, dataFim, codigoTransportador, codigoVeiculo, operador, motivoAdicionalFrete, situacaoComplementoFrete, codigosFilial, codigosRecebedor, codigosTipoCarga, codigosTipoOperacao);

            return result.Count();
        }

        private IQueryable<Dominio.Entidades.Embarcador.Cargas.CargaComplementoFrete> MontarQueryRelatorio(IQueryable<Dominio.Entidades.Embarcador.Cargas.CargaComplementoFrete> query, DateTime dataInicio, DateTime dataFim, int codigoTransportador, int codigoVeiculo, int operador, int motivoAdicionalFrete, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoComplementoFrete situacaoComplementoFrete, List<int> codigosFilial, List<double> codigosRecebedor, List<int> codigosTipoCarga, List<int> codigosTipoOperacao)
        {
            query = query.Where(obj => obj.Carga.SituacaoCarga != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.Cancelada);

            if (dataInicio != DateTime.MinValue)
                query = query.Where(o => o.DataAlteracao >= dataInicio);

            if (dataFim != DateTime.MinValue)
                query = query.Where(o => o.DataAlteracao < dataFim.AddDays(1));

            if (codigoVeiculo > 0)
                query = query.Where(o => o.Carga.Veiculo.Codigo == codigoVeiculo || o.Carga.VeiculosVinculados.Any(obj => obj.Codigo == codigoVeiculo));

            if (codigoTransportador > 0)
                query = query.Where(o => o.Carga.Empresa.Codigo == codigoTransportador);

            if (codigosFilial?.Count > 0)
                query = query.Where(o => codigosFilial.Contains(o.Carga.Filial.Codigo) || o.Carga.Pedidos.Any(pedido => codigosRecebedor.Contains(pedido.Recebedor.CPF_CNPJ)));

            if (codigosTipoCarga?.Count > 0)
                query = query.Where(o => codigosTipoCarga.Contains(o.Carga.TipoDeCarga.Codigo) || (codigosTipoCarga.Contains(-1) && o.Carga.TipoDeCarga == null));

            if (codigosTipoOperacao?.Count > 0)
                query = query.Where(o => codigosTipoOperacao.Contains(o.Carga.TipoOperacao.Codigo) || (codigosTipoOperacao.Contains(-1) && o.Carga.TipoOperacao == null));

            if (motivoAdicionalFrete > 0)
                query = query.Where(o => o.MotivoAdicionalFrete.Codigo == motivoAdicionalFrete);

            if (operador > 0)
                query = query.Where(o => o.Usuario.Codigo == operador);

            if (situacaoComplementoFrete != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoComplementoFrete.Todas)
                query = query.Where(o => o.SituacaoComplementoFrete == situacaoComplementoFrete);

            return query;
        }


    }
}
