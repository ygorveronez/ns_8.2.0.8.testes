using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.Pedidos
{
    public class PedidoAutorizacao : RepositorioBase<Dominio.Entidades.Embarcador.Pedidos.PedidoAutorizacao>
    {
        public PedidoAutorizacao(UnitOfWork unitOfWork) : base(unitOfWork) { }

        #region Métodos Públicos

        public List<Dominio.Entidades.Embarcador.Pedidos.PedidoAutorizacao> BuscarPorPedido(int codigoPedido)
        {
            IQueryable<Dominio.Entidades.Embarcador.Pedidos.PedidoAutorizacao> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoAutorizacao>();

            query = query.Where(o => o.Pedido.Codigo == codigoPedido);

            return query.ToList();
        }

        public Dominio.Entidades.Embarcador.Pedidos.PedidoAutorizacao BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoAutorizacao>();
            var result = from obj in query where obj.Codigo == codigo select obj;
            return result.FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.Pedidos.PedidoAutorizacao BuscarAutorizacaoPedido(int codigoPedido)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoAutorizacao>();
            var result = from obj in query where obj.Pedido.Codigo == codigoPedido select obj;
            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Pedidos.PedidoAutorizacao> BuscarAutorizacoesPorPedido(int codigoPedido)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoAutorizacao>();
            var result = from obj in query where obj.Pedido.Codigo == codigoPedido select obj;
            return result.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Pedidos.PedidoAutorizacao> ConsultarAutorizacoesPorPedido(int codigoPedido, string propOrdenacao, string dirOrdenacao, int inicioRegistros, int maximoRegistros)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoAutorizacao>();
            var result = from obj in query
                         where obj.Pedido.Codigo == codigoPedido
                         select obj;

            return result.OrderBy(propOrdenacao + (dirOrdenacao == "asc" ? " ascending" : " descending")).Skip(inicioRegistros).Take(maximoRegistros).ToList();
        }

        public List<Dominio.Entidades.Embarcador.Pedidos.PedidoAutorizacao> BuscarPorCarga(int codigoCarga)
        {
            var queryCargaPedido = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedido>();
            var resultCargaPedido = queryCargaPedido.Where(c => c.Carga.Codigo == codigoCarga).Select(o => o.Pedido.Codigo);

            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoAutorizacao>();
            query = query.Where(p => resultCargaPedido.Contains(p.Pedido.Codigo));

            return query.ToList();
        }

        public int ContarConsultarAutorizacoesPorPedido(int codigoPedido)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoAutorizacao>();
            var result = from obj in query where obj.Pedido.Codigo == codigoPedido select obj;


            return result.Count();
        }

        public int ContarRejeitadas(int codigoPedido, int codigoRegra)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoAutorizacao>();
            var resut = from obj in query
                        where
                            obj.Pedido.Codigo == codigoPedido
                            && obj.Situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoOcorrenciaAutorizacao.Rejeitada
                            && (obj.RegrasPedido.Codigo == codigoRegra || obj.RegrasPedido == null)
                        select obj;

            return resut.Count();
        }

        public int ContarPendentes(int codigoPedido, int codigoRegra)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoAutorizacao>();
            var resut = from obj in query
                        where
                            obj.Pedido.Codigo == codigoPedido
                            && obj.Situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoOcorrenciaAutorizacao.Pendente
                            && (obj.RegrasPedido.Codigo == codigoRegra || obj.RegrasPedido == null)
                        select obj;

            return resut.Count();
        }

        public int ContarAprovacoesOcorrencia(int codigoPedido, int codigoRegra)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoAutorizacao>();
            var resut = from obj in query
                        where
                            obj.Pedido.Codigo == codigoPedido
                            && obj.Situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoOcorrenciaAutorizacao.Aprovada
                            && (obj.RegrasPedido.Codigo == codigoRegra || obj.RegrasPedido == null)
                        select obj;

            return resut.Count();
        }

        public List<Dominio.Entidades.Embarcador.Pedidos.RegrasPedido> BuscarRegrasPedido(int codigoOcorrencia)
        {
            var queryGroup = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoAutorizacao>();
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.RegrasPedido>();

            var resultGroup = from obj in queryGroup where obj.Pedido.Codigo == codigoOcorrencia select obj.RegrasPedido;
            var result = from obj in query where resultGroup.Contains(obj) select obj;

            return result.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Pedidos.PedidoAutorizacao> BuscarPorPedidoUsuario(int codigo, int usuario)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoAutorizacao>();
            var result = from obj in query where obj.Pedido.Codigo == codigo select obj;

            if (usuario > 0)
                result = result.Where(o => o.Usuario.Codigo == usuario);

            return result.ToList();
        }

        public bool VerificarSePodeAprovar(int codigoPedido, int codigoRegra, int codigoUsuario)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoAutorizacao>();
            var resut = from obj in query
                        where
                            obj.Codigo == codigoRegra
                            && obj.Pedido.Codigo == codigoPedido
                            && obj.Usuario.Codigo == codigoUsuario
                            && obj.Situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoOcorrenciaAutorizacao.Pendente
                        select obj;

            return resut.Count() > 0;
        }

        public List<Dominio.Entidades.Embarcador.Pedidos.PedidoAutorizacao> BuscarPendentesPorPedidoEUsuario(int codigo, int usuario)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoAutorizacao>();
            var result = from obj in query
                         where
                            obj.Pedido.Codigo == codigo &&
                            //obj.Usuario.Codigo == usuario &&
                            obj.Situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoOcorrenciaAutorizacao.Pendente
                         select obj;
            return result.ToList();
        }

        public List<Dominio.Entidades.Usuario> ResponsavelOcorrencia(int codigoPedido)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoAutorizacao>();
            var queryUsuario = this.SessionNHiBernate.Query<Dominio.Entidades.Usuario>();

            var result = from obj in query where obj.Pedido.Codigo == codigoPedido select obj.Usuario;
            var resultUsuario = from obj in queryUsuario where result.Contains(obj) select obj;


            return result.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Pedidos.Pedido> Consultar(int usuario, DateTime dataInicio, DateTime dataFim, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoPedido situacao, int numeroPedidos, int codigoGrupoPessoa, int codigoTipoCarga, int codigoTipoOperacao, string propOrdenacao, string dirOrdenacao, int inicioRegistros, int maximoRegistros, int modeloVeicularCarga, int ModeloVeiculo, string TipoCarroceria, int ModeloCarroceria)
        {
            var result = _Consultar(usuario, dataInicio, dataFim, situacao, numeroPedidos, codigoGrupoPessoa, codigoTipoCarga, codigoTipoOperacao, modeloVeicularCarga, ModeloVeiculo, TipoCarroceria, ModeloCarroceria);

            result = result
                .OrderBy(propOrdenacao + (dirOrdenacao == "asc" ? " ascending" : " descending"))
                .Skip(inicioRegistros);

            if (maximoRegistros > 0)
                result = result.Take(maximoRegistros);

            return result.ToList();
        }

        public int ContarConsulta(int usuario, DateTime dataInicio, DateTime dataFim, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoPedido situacao, int numeroPedidos, int codigoGrupoPessoa, int codigoTipoCarga, int codigoTipoOperacao, int modeloVeicularCarga, int ModeloVeiculo, string TipoCarroceria, int ModeloCarroceria)
        {
            var result = _Consultar(usuario, dataInicio, dataFim, situacao, numeroPedidos, codigoGrupoPessoa, codigoTipoCarga, codigoTipoOperacao, modeloVeicularCarga, ModeloVeiculo, TipoCarroceria, ModeloCarroceria);

            return result.Count();
        }

        #endregion

        #region Métodos Privados

        private IQueryable<Dominio.Entidades.Embarcador.Pedidos.Pedido> _Consultar(int usuario, DateTime dataInicio, DateTime dataFim, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoPedido situacao, int numeroPedidos, int codigoGrupoPessoa, int codigoTipoCarga, int codigoTipoOperacao, int modeloVeicularCarga, int ModeloVeiculo, string TipoCarroceria, int ModeloCarroceria)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.Pedido>();
            var queryAutorizacao = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoAutorizacao>();

            var resultAutorizacao = from obj in queryAutorizacao select obj.Pedido;
            var result = from obj in query where resultAutorizacao.Contains(obj) select obj;

            // Filtros da ocorrencia
            if (codigoGrupoPessoa > 0)
                result = result.Where(obj => obj.GrupoPessoas.Codigo == codigoGrupoPessoa);

            if (numeroPedidos > 0)
                result = result.Where(obj => obj.Numero == numeroPedidos);

            if (dataInicio != DateTime.MinValue)
                result = result.Where(obj => obj.DataCarregamentoPedido.Value.Date >= dataInicio);

            if (dataFim != DateTime.MinValue)
                result = result.Where(obj => obj.DataCarregamentoPedido.Value.Date <= dataFim);

            if (situacao != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoPedido.Todos)
            {
                if (situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoPedido.AutorizacaoPendente)
                    result = result.Where(o => o.SituacaoPedido == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoPedido.AgAprovacao || o.SituacaoPedido == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoPedido.AutorizacaoPendente);
                else
                    result = result.Where(obj => obj.SituacaoPedido == situacao);
            }

            if (codigoTipoCarga > 0)
                result = result.Where(obj => obj.TipoCarga.Codigo == codigoTipoCarga);

            if (codigoTipoOperacao > 0)
                result = result.Where(obj => obj.TipoOperacao.Codigo == codigoTipoOperacao);

            // Filtros da autorizacao
            if (usuario > 0)
                result = result.Where(obj => obj.PedidoAutorizacao.Any(aut => aut.Usuario.Codigo == usuario));

            // O filtro que mais influcencia é o de situacao da ocorrencia, pois AgAprovacao, AgAutorizacaoEmissao e AutorizacaoPendente tem o mesmo comportamento
            bool situacaoPendentes = situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoPedido.AgAprovacao || situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoPedido.AutorizacaoPendente;
            Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoOcorrenciaAutorizacao situacaoAutorizacaoPendente = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoOcorrenciaAutorizacao.Pendente;

            if (situacaoPendentes)
                result = result.Where(obj => obj.PedidoAutorizacao.Any(aut => usuario > 0 ? (aut.Usuario.Codigo == usuario && aut.Situacao == situacaoAutorizacaoPendente) : (aut.Situacao == situacaoAutorizacaoPendente)));

            if (modeloVeicularCarga > 0)
                result = result.Where(obj => obj.PedidoAutorizacao.Any(aut => aut.Pedido.ModeloVeicularCarga.Codigo == modeloVeicularCarga));

            if (ModeloVeiculo > 0)
                result = result.Where(obj => obj.PedidoAutorizacao.Any(aut => aut.Pedido.VeiculoTracao.Modelo.Codigo == ModeloVeiculo));

            if (!TipoCarroceria.Equals("99"))
                result = result.Where(obj => obj.PedidoAutorizacao.Any(aut => aut.Pedido.VeiculoTracao.TipoCarroceria == TipoCarroceria));

            if (ModeloCarroceria > 0)
                result = result.Where(obj => obj.PedidoAutorizacao.Any(aut => aut.Pedido.VeiculoTracao.ModeloCarroceria.Codigo == ModeloCarroceria
                || aut.Pedido.Veiculos.Any(veiculo => veiculo.ModeloCarroceria.Codigo == ModeloCarroceria)));

            return result;
        }

        #endregion
    }
}
