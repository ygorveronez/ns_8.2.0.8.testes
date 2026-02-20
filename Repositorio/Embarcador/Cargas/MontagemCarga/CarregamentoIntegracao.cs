using System.Linq;
using NHibernate.Linq;
using System.Linq.Dynamic.Core;
using System.Collections.Generic;
using System;

namespace Repositorio.Embarcador.Cargas.MontagemCarga
{
    public class CarregamentoIntegracao : RepositorioBase<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoIntegracao>
    {
        #region Construtores

        public CarregamentoIntegracao(UnitOfWork unitOfWork) : base(unitOfWork) { }

        #endregion

        #region Métodos Privados

        private IQueryable<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoIntegracao> Consultar(DateTime dataInicio, DateTime dataFim, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao? situacao, string numeroCarregamento, int codigoEmpresa, double emitente, string carga, int protocolo, string numeroExp, double destinatario)
        {
            var consultaCarregamentoIntegracao = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoIntegracao>();

            if (situacao.HasValue)
                consultaCarregamentoIntegracao = consultaCarregamentoIntegracao.Where(o => o.SituacaoIntegracao == situacao);

            if (dataInicio != DateTime.MinValue)
                consultaCarregamentoIntegracao = consultaCarregamentoIntegracao.Where(o => o.DataIntegracao >= dataInicio);

            if (dataFim != DateTime.MinValue)
                consultaCarregamentoIntegracao = consultaCarregamentoIntegracao.Where(o => o.DataIntegracao <= dataFim);

            if (emitente > 0)
                consultaCarregamentoIntegracao = consultaCarregamentoIntegracao.Where(o => o.Carregamento.Pedidos.Any(a => a.Pedido.Remetente.CPF_CNPJ == emitente));

            if (destinatario > 0)
                consultaCarregamentoIntegracao = consultaCarregamentoIntegracao.Where(o => o.Carregamento.Pedidos.Any(a => a.Pedido.Destinatario.CPF_CNPJ == destinatario));

            if (codigoEmpresa > 0)
                consultaCarregamentoIntegracao = consultaCarregamentoIntegracao.Where(o => o.Carregamento.Empresa.Codigo == codigoEmpresa);

            if (!string.IsNullOrEmpty(numeroCarregamento))
                consultaCarregamentoIntegracao = consultaCarregamentoIntegracao.Where(o => o.Carregamento.NumeroCarregamento == numeroCarregamento);

            if (!string.IsNullOrEmpty(carga))
            {
                IQueryable<Dominio.Entidades.Embarcador.Cargas.Carga> subQueryCarga = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.Carga>();
                consultaCarregamentoIntegracao = consultaCarregamentoIntegracao.Where(o => subQueryCarga.Any(obj => obj.Carregamento == o.Carregamento && obj.CodigoCargaEmbarcador == carga));
            }
            
            if (!string.IsNullOrEmpty(numeroExp))
            {
                IQueryable<Dominio.Entidades.Embarcador.Cargas.CargaPedido> subQueryCargaPedido = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedido>();
                consultaCarregamentoIntegracao = consultaCarregamentoIntegracao.Where(o => subQueryCargaPedido.Any(obj => obj.Carga.Carregamento.Codigo == o.Carregamento.Codigo && obj.Pedido.NumeroEXP == numeroExp));
            }

            if (protocolo > 0)
            {
                IQueryable<Dominio.Entidades.Embarcador.Cargas.Carga> subQueryCarga = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.Carga>();
                consultaCarregamentoIntegracao = consultaCarregamentoIntegracao.Where(o => subQueryCarga.Any(obj => obj.Carregamento == o.Carregamento && obj.Protocolo == protocolo));
            }
            return consultaCarregamentoIntegracao.Fetch(obj => obj.Carregamento).ThenFetch(obj => obj.Recebedor).ThenFetch(obj => obj.Localidade).Fetch(obj => obj.TipoIntegracao);
        }

        #endregion Métodos Privados

        #region Métodos Públicos

        public Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoIntegracao BuscarPorCodigo(int codigo)
        {
            var query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoIntegracao>();
            var result = query.Where(obj => obj.Codigo == codigo);
            return result.FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoIntegracao BuscarPorCodigoArquivo(int codigoArquivo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoIntegracao>();

            var result = from obj in query where obj.ArquivosTransacao.Any(o => o.Codigo == codigoArquivo) select obj;

            return result.FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoIntegracao BuscarPorCarregamentoETipoIntegracao(int codigoCarregamento, int codigoTipoIntegracao)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoIntegracao>();

            var result = from obj in query where obj.Carregamento.Codigo == codigoCarregamento && obj.TipoIntegracao.Codigo == codigoTipoIntegracao select obj;

            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoIntegracao> BuscarCarregamentoIntegracaoPendente(int limiteRegistros)
        {
            DateTime dataLimiteProximaTentativa = DateTime.Now.AddMinutes(-5d);
            int numeroTentativasLimite = 3;

            var consultaCarregamentoIntegracao = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoIntegracao>()
                .Where(o =>
                    o.SituacaoIntegracao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.AgIntegracao ||
                    (
                        o.SituacaoIntegracao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao &&
                        o.NumeroTentativas < numeroTentativasLimite &&
                        o.DataIntegracao <= dataLimiteProximaTentativa
                    )
                );

            consultaCarregamentoIntegracao = consultaCarregamentoIntegracao
                .Fetch(obj => obj.Carregamento)
                .Fetch(obj => obj.TipoIntegracao);

            return consultaCarregamentoIntegracao
                .OrderBy(o => o.Codigo)
                .Skip(0)
                .Take(limiteRegistros)
                .ToList();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoIntegracao> Consultar(DateTime dataInicio, DateTime dataFim, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao? situacao, string numeroCarregamento, int codigoEmpresa, double emitente, string carga, int protocolo, string numeroExp, double destinatario, string propOrdena, string dirOrdena, int inicioRegistros, int maximoRegistros)
        {
            var result = Consultar(dataInicio, dataFim, situacao, numeroCarregamento, codigoEmpresa, emitente, carga, protocolo, numeroExp, destinatario);

            if (!string.IsNullOrWhiteSpace(propOrdena))
                result = result.OrderBy(propOrdena + (dirOrdena == "asc" ? " ascending" : " descending"));

            if (inicioRegistros > 0)
                result = result.Skip(inicioRegistros);

            if (maximoRegistros > 0)
                result = result.Take(maximoRegistros);

            return result.ToList();
        }

        public int ContarConsulta(DateTime dataInicio, DateTime dataFim, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao? situacao, string numeroCarregamento, int codigoEmpresa, double emitente, string carga, int protocolo, string numeroExp, double destinatario)
        {
            var result = Consultar(dataInicio, dataFim, situacao, numeroCarregamento, codigoEmpresa, emitente, carga, protocolo, numeroExp, destinatario);

            return result.Count();
        }

        #endregion Métodos Públicos
    }
}
