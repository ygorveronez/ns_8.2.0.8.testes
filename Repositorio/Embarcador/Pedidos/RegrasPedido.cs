using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.Pedidos
{
    public class RegrasPedido : RepositorioBase<Dominio.Entidades.Embarcador.Pedidos.RegrasPedido>
    {
        public RegrasPedido(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.Embarcador.Pedidos.RegrasPedido BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.RegrasPedido>();
            var result = from obj in query where obj.Codigo == codigo select obj;
            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Pedidos.RegrasPedido> BuscarRegraPorGrupoPessoa(int codigo, DateTime data)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.RegrasPedidoGrupoPessoa>();
            var result = from obj in query
                         where
                            (obj.Condicao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CondicaoAutorizaoOcorrencia.IgualA && obj.GrupoPessoas.Codigo == codigo) ||
                            (obj.Condicao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CondicaoAutorizaoOcorrencia.DiferenteDe && obj.GrupoPessoas.Codigo != codigo)
                         select obj.RegrasPedido;

            result = result.Where(o => o.RegraPorGrupoPessoa == true && (o.Vigencia >= data || o.Vigencia == null));

            return result.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Pedidos.RegrasPedido> BuscarRegraPorTipoCarga(int codigo, DateTime data)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.RegrasPedidoTipoCarga>();
            var result = from obj in query
                         where
                            (obj.Condicao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CondicaoAutorizaoOcorrencia.IgualA && obj.TipoDeCarga.Codigo == codigo) ||
                            (obj.Condicao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CondicaoAutorizaoOcorrencia.DiferenteDe && obj.TipoDeCarga.Codigo != codigo)
                         select obj.RegrasPedido;

            result = result.Where(o => o.RegraPorTipoCarga == true && (o.Vigencia >= data || o.Vigencia == null));

            return result.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Pedidos.RegrasPedido> BuscarRegraPorTipoOperacao(int codigo, DateTime data)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.RegrasPedidoTipoOperacao>();
            var result = from obj in query
                         where
                            (obj.Condicao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CondicaoAutorizaoOcorrencia.IgualA && obj.TipoOperacao.Codigo == codigo) ||
                            (obj.Condicao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CondicaoAutorizaoOcorrencia.DiferenteDe && obj.TipoOperacao.Codigo != codigo)
                         select obj.RegrasPedido;

            result = result.Where(o => o.RegraPorTipoOperacao == true && (o.Vigencia >= data || o.Vigencia == null));

            return result.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Pedidos.RegrasPedido> BuscarRegraPorModeloVeicular(int codigo, DateTime data)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.RegrasPedidoModeloVeicular>();
            var result = from obj in query
                         where
                            (obj.Condicao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CondicaoAutorizaoOcorrencia.IgualA && obj.ModeloVeicularCarga.Codigo == codigo) ||
                            (obj.Condicao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CondicaoAutorizaoOcorrencia.DiferenteDe && obj.ModeloVeicularCarga.Codigo != codigo)
                         select obj.RegrasPedido;

            result = result.Where(o => o.RegraPorModeloVeicular == true && (o.Vigencia >= data || o.Vigencia == null));

            return result.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Pedidos.RegrasPedido> BuscarRegraPorSituacaoColaborador(int codigo, DateTime data)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.RegrasPedidoSituacaoColaborador>();
            var result = from obj in query
                         where
                            (obj.Condicao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CondicaoAutorizaoOcorrencia.IgualA && obj.ColaboradorSituacao.Codigo == codigo) ||
                            (obj.Condicao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CondicaoAutorizaoOcorrencia.DiferenteDe && obj.ColaboradorSituacao.Codigo != codigo)
                         select obj.RegrasPedido;

            result = result.Where(o => o.RegraPorSituacaoColaborador == true && (o.Vigencia >= data || o.Vigencia == null));

            return result.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Pedidos.RegrasPedido> BuscarRegraPorValor(decimal valorOcorrencia, DateTime data)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.RegrasPedidoValorFrete>();
            var result = from obj in query
                         where
                            (obj.Condicao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CondicaoAutorizaoOcorrencia.IgualA && obj.Valor == valorOcorrencia ||
                             obj.Condicao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CondicaoAutorizaoOcorrencia.DiferenteDe && obj.Valor != valorOcorrencia ||
                             obj.Condicao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CondicaoAutorizaoOcorrencia.MaiorIgualQue && obj.Valor <= valorOcorrencia ||
                             obj.Condicao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CondicaoAutorizaoOcorrencia.MaiorQue && obj.Valor < valorOcorrencia ||
                             obj.Condicao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CondicaoAutorizaoOcorrencia.MenorIgualQue && obj.Valor >= valorOcorrencia ||
                             obj.Condicao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CondicaoAutorizaoOcorrencia.MenorQue && obj.Valor > valorOcorrencia)
                         select obj.RegrasPedido;

            result = result.Where(o => o.RegraPorValorFrete == true && (o.Vigencia >= data || o.Vigencia == null));

            return result.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Pedidos.RegrasPedido> BuscarRegraPorDistancia(decimal distancia, DateTime data)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.RegrasPedidoDistancia>();
            var result = from obj in query
                         where
                            (obj.Condicao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CondicaoAutorizaoOcorrencia.IgualA && obj.Distancia == distancia ||
                             obj.Condicao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CondicaoAutorizaoOcorrencia.DiferenteDe && obj.Distancia != distancia ||
                             obj.Condicao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CondicaoAutorizaoOcorrencia.MaiorIgualQue && obj.Distancia <= distancia ||
                             obj.Condicao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CondicaoAutorizaoOcorrencia.MaiorQue && obj.Distancia < distancia ||
                             obj.Condicao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CondicaoAutorizaoOcorrencia.MenorIgualQue && obj.Distancia >= distancia ||
                             obj.Condicao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CondicaoAutorizaoOcorrencia.MenorQue && obj.Distancia > distancia)
                         select obj.RegrasPedido;

            result = result.Where(o => o.RegraPorDistancia == true && (o.Vigencia >= data || o.Vigencia == null));

            return result.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Pedidos.RegrasPedido> BuscarRegraPorDiferencaFreteLiquidoParaFreteTerceiro(decimal diferencaFreteLiquidoParaFreteTerceiro, DateTime data)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.RegrasPedidoPercentualDiferencaFreteLiquidoParaFreteTerceiro>();
            var result = from obj in query
                         where
                            (obj.Condicao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CondicaoAutorizaoOcorrencia.IgualA && obj.DiferencaFreteLiquidoParaFreteTerceiro == diferencaFreteLiquidoParaFreteTerceiro ||
                             obj.Condicao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CondicaoAutorizaoOcorrencia.DiferenteDe && obj.DiferencaFreteLiquidoParaFreteTerceiro != diferencaFreteLiquidoParaFreteTerceiro ||
                             obj.Condicao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CondicaoAutorizaoOcorrencia.MaiorIgualQue && obj.DiferencaFreteLiquidoParaFreteTerceiro <= diferencaFreteLiquidoParaFreteTerceiro ||
                             obj.Condicao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CondicaoAutorizaoOcorrencia.MaiorQue && obj.DiferencaFreteLiquidoParaFreteTerceiro < diferencaFreteLiquidoParaFreteTerceiro ||
                             obj.Condicao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CondicaoAutorizaoOcorrencia.MenorIgualQue && obj.DiferencaFreteLiquidoParaFreteTerceiro >= diferencaFreteLiquidoParaFreteTerceiro ||
                             obj.Condicao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CondicaoAutorizaoOcorrencia.MenorQue && obj.DiferencaFreteLiquidoParaFreteTerceiro > diferencaFreteLiquidoParaFreteTerceiro)
                         select obj.RegrasPedido;

            result = result.Where(o => o.RegraPorDiferencaFreteLiquidoParaFreteTerceiro == true && (o.Vigencia >= data || o.Vigencia == null));

            return result.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Pedidos.RegrasPedido> Consultar(string descricao, string propriedadeOrdenacao, string direcaoOrdenacao, int inicioRegistros, int maximoRegistros)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.RegrasPedido>();

            var result = from obj in query select obj;

            if (!string.IsNullOrWhiteSpace(descricao))
                result = result.Where(o => o.Descricao.Contains(descricao));

            return result.OrderBy(propriedadeOrdenacao + (direcaoOrdenacao == "asc" ? " ascending" : " descending")).Skip(inicioRegistros).Take(maximoRegistros).ToList();
        }

        public int ContarConsulta(string descricao)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.RegrasPedido>();

            var result = from obj in query select obj;

            if (!string.IsNullOrWhiteSpace(descricao))
                result = result.Where(o => o.Descricao.Contains(descricao));

            return result.Count();
        }

        public List<Dominio.Entidades.Embarcador.Pedidos.RegrasPedido> ConsultarRegras(DateTime? dataInicio, DateTime? dataFim, Dominio.Entidades.Usuario aprovador, string descricao, string propOrdenacao, string dirOrdenacao, int inicioRegistros, int maximoRegistros)
        {
            var result = _ConsultarRegras(dataInicio, dataFim, aprovador, descricao);

            if (inicioRegistros > 0 && maximoRegistros > inicioRegistros)
                result = result.Skip(inicioRegistros).Take(maximoRegistros);

            return result
                    .OrderBy(propOrdenacao + (dirOrdenacao == "asc" ? " ascending" : " descending"))
                    .ToList();
        }

        public int ContarConsultaRegras(DateTime? dataInicio, DateTime? dataFim, Dominio.Entidades.Usuario aprovador, string descricao)
        {
            var result = _ConsultarRegras(dataInicio, dataFim, aprovador, descricao);

            return result.Count();
        }

        private IQueryable<Dominio.Entidades.Embarcador.Pedidos.RegrasPedido> _ConsultarRegras(DateTime? dataInicio, DateTime? dataFim, Dominio.Entidades.Usuario aprovador, string descricao)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.RegrasPedido>();
            var result = from obj in query select obj;

            if (dataInicio.HasValue && dataFim.HasValue)
                result = result.Where(o => o.Vigencia >= dataInicio.Value && o.Vigencia < dataFim.Value.AddDays(1));
            else if (dataInicio.HasValue)
                result = result.Where(o => o.Vigencia >= dataInicio.Value);
            else if (dataFim.HasValue)
                result = result.Where(o => o.Vigencia < dataFim.Value.AddDays(1));

            if (aprovador != null)
                result = result.Where(o => o.Aprovadores.Contains(aprovador));

            if (!string.IsNullOrWhiteSpace(descricao))
                result = result.Where(o => o.Descricao.Contains(descricao));

            return result;
        }
    }
}
