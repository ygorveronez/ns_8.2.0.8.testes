using NHibernate;
using NHibernate.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.Pedidos
{
    public class PedidoOcorrenciaColetaEntrega : RepositorioBase<Dominio.Entidades.Embarcador.Pedidos.PedidoOcorrenciaColetaEntrega>
    {
        #region Construtores

        public PedidoOcorrenciaColetaEntrega(UnitOfWork unitOfWork) : base(unitOfWork) { }

        #endregion

        #region Métodos Privados

        public IQueryable<Dominio.Entidades.Embarcador.Pedidos.PedidoOcorrenciaColetaEntrega> ConsultarPendentesIntegracao()
        {
            var consultaPedidoOcorrenciaColetaEntrega = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoOcorrenciaColetaEntrega>()
                .Where(o => o.PendenteIntegracaoERP == true);

            return consultaPedidoOcorrenciaColetaEntrega;
        }

        private string QueryConsultaUltimosRegistrosIntegradosPorTipoIntegracao(int codigoIntegracao)
        {
            string sql = $@"Select POC_CODIGO from T_PEDIDO_OCORRENCIA_COLETA_ENTREGA_INTEGRACAO where INT_CODIGO in
                            (
	                            SELECT Max(I.INT_CODIGO) Codigo
	                            FROM T_PEDIDO_OCORRENCIA_COLETA_ENTREGA_INTEGRACAO I
	                            INNER JOIN T_PEDIDO_OCORRENCIA_COLETA_ENTREGA OCE ON OCE.POC_CODIGO = I.POC_CODIGO
	                            INNER JOIN T_OCORRENCIA OCO ON OCO.OCO_CODIGO = OCE.OCO_CODIGO
	                            AND OCO.OCO_ATIVAR_ENVIO_AUTOMATICO_TIPO_OCORRENCIA = 1
	                            INNER JOIN T_CARGA C ON C.CAR_CODIGO = OCE.CAR_CODIGO
	                            AND C.CAR_SITUACAO NOT IN (11,
							                                13,
							                                18)
	                            WHERE I.TPI_CODIGO = {codigoIntegracao}
	                                AND INT_SITUACAO_INTEGRACAO = 1
	                            GROUP BY OCE.CAR_CODIGO
                            )";

            return sql;
        }

        #endregion

        #region Métodos Públicos

        public Dominio.Entidades.Embarcador.Pedidos.PedidoOcorrenciaColetaEntrega BuscarOcorrenciaFinalizadoraPorPedido(int codigoPedido)
        {
            var query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoOcorrenciaColetaEntrega>()
                .Where(o => o.Pedido.Codigo == codigoPedido && o.TipoDeOcorrencia.Tipo == "F");

            return query.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Pedidos.PedidoOcorrenciaColetaEntrega> BuscarOcorrenciasFinalizadorasPorPedido(int codigoPedido)
        {
            var query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoOcorrenciaColetaEntrega>()
                .Where(o => o.Pedido.Codigo == codigoPedido && o.TipoDeOcorrencia.Tipo == "F");

            return query.ToList();
        }

        public Dominio.Entidades.Embarcador.Pedidos.PedidoOcorrenciaColetaEntrega BuscarOcorrenciaFinalizadoraPorPedidos(List<int> codigosPedidos)
        {
            var query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoOcorrenciaColetaEntrega>()
                .Where(o => codigosPedidos.Contains(o.Pedido.Codigo) && o.TipoDeOcorrencia.Tipo == "F");

            return query.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Pedidos.PedidoOcorrenciaColetaEntrega> BuscarPorPedido(int pedido)
        {
            var query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoOcorrenciaColetaEntrega>();
            var result = query.Where(obj => obj.Pedido.Codigo == pedido);
            return result
                .Fetch(obj => obj.Pedido)
                .Fetch(obj => obj.TipoDeOcorrencia)
                .Fetch(obj => obj.Alvo)
                .ThenFetch(obj => obj.Localidade)
                .ToList();
        }

        public List<Dominio.Entidades.Embarcador.Pedidos.PedidoOcorrenciaColetaEntrega> BuscarPorPedidos(List<int> pedidos)
        {
            var query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoOcorrenciaColetaEntrega>();
            var result = query.Where(obj => pedidos.Contains(obj.Pedido.Codigo));
            return result
                .Fetch(obj => obj.Pedido)
                .Fetch(obj => obj.TipoDeOcorrencia)
                .Fetch(obj => obj.Alvo)
                .ThenFetch(obj => obj.Localidade)
                .ToList();
        }

        public Dominio.Entidades.Embarcador.Pedidos.PedidoOcorrenciaColetaEntrega BuscarPorPedidoPacoteTipoOcorrencia(int pedido, string pacote, int tipoOcorrencia)
        {
            var query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoOcorrenciaColetaEntrega>();
            var result = query.Where(obj => obj.Pedido.Codigo == pedido && obj.Pacote == pacote && obj.TipoDeOcorrencia.Codigo == tipoOcorrencia);
            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Pedidos.PedidoOcorrenciaColetaEntrega> BuscarPorNumeroPedidoEmbarcador(string numeroPedidoEmbarcador, DateTime dataInicial, DateTime dataFinal, int inicioRegistros, int limiteRegistros)
        {
            var query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoOcorrenciaColetaEntrega>();

            if (!string.IsNullOrWhiteSpace(numeroPedidoEmbarcador))
                query = query.Where(o => o.Pedido.NumeroPedidoEmbarcador == numeroPedidoEmbarcador);

            if (dataInicial.Date > DateTime.MinValue)
                query = query.Where(o => o.DataOcorrencia >= dataInicial);

            if (dataFinal.Date > DateTime.MinValue)
                query = query.Where(o => o.DataOcorrencia <= dataFinal);

            if (inicioRegistros > -1)
            {
                return query
                    .Fetch(obj => obj.Pedido)
                    .Fetch(obj => obj.TipoDeOcorrencia)
                    .Fetch(obj => obj.Alvo)
                    .ThenFetch(obj => obj.Localidade)
                    .Skip(inicioRegistros)
                    .Take(limiteRegistros)
                    .ToList();
            }
            else
            {
                return query
                    .Fetch(obj => obj.Pedido)
                    .Fetch(obj => obj.TipoDeOcorrencia)
                    .Fetch(obj => obj.Alvo)
                    .ThenFetch(obj => obj.Localidade)
                    .ToList();
            }
        }

        public List<Dominio.Entidades.Embarcador.Pedidos.PedidoOcorrenciaColetaEntrega> BuscarPorCarga(int codigoCarga)
        {
            var query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedido>();
            query = query.Where(o => o.Carga.Codigo == codigoCarga);

            var queryPeidoOcorrencia = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoOcorrenciaColetaEntrega>();
            var queryCargaPedido = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedido>();
            queryCargaPedido = queryCargaPedido.Where(c => query.Any(o => o.Carga == c.Carga));

            queryPeidoOcorrencia = queryPeidoOcorrencia.Where(c => queryCargaPedido.Any(o => o.Pedido == c.Pedido));

            return queryPeidoOcorrencia
                .Fetch(obj => obj.Pedido)
                .Fetch(obj => obj.TipoDeOcorrencia)
                .Fetch(obj => obj.Alvo)
                .ThenFetch(obj => obj.Localidade)
                .ToList();
        }

        public List<Dominio.Entidades.Embarcador.Pedidos.PedidoOcorrenciaColetaEntrega> BuscarPorCodigoCargaEmbarcador(string codigoCargaEmbarcador)
        {
            var query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedido>();
            query = query.Where(o => o.Carga.CodigoCargaEmbarcador == codigoCargaEmbarcador);

            var queryPeidoOcorrencia = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoOcorrenciaColetaEntrega>();
            var queryCargaPedido = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedido>();
            queryCargaPedido = queryCargaPedido.Where(c => query.Any(o => o.Carga == c.Carga));

            queryPeidoOcorrencia = queryPeidoOcorrencia.Where(c => queryCargaPedido.Any(o => o.Pedido == c.Pedido));

            return queryPeidoOcorrencia
                .Fetch(obj => obj.Pedido)
                .Fetch(obj => obj.TipoDeOcorrencia)
                .Fetch(obj => obj.Alvo)
                .ThenFetch(obj => obj.Localidade)
                .ToList();
        }

        public List<Dominio.Entidades.Embarcador.Pedidos.PedidoOcorrenciaColetaEntrega> BuscarVisiveisAoClientePorPedido(int pedido)
        {
            var query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoOcorrenciaColetaEntrega>();
            var result = query.Where(obj => obj.Pedido.Codigo == pedido && obj.TipoDeOcorrencia.NaoIndicarAoCliente != true);
            return result
                .Fetch(obj => obj.TipoDeOcorrencia)
                .Fetch(obj => obj.Alvo)
                .ThenFetch(obj => obj.Localidade)
                .ToList();
        }

        public List<int> BuscarCodigosPendentesEnvio(int maximo)
        {
            var query = SessionNHiBernate.CreateSQLQuery($@"
                select TOP {maximo} pedidoOcorrenciaColetaEntrega.POC_CODIGO as Codigo from T_PEDIDO_OCORRENCIA_COLETA_ENTREGA pedidoOcorrenciaColetaEntrega where pedidoOcorrenciaColetaEntrega.POC_PENDENTE_ENVIO_EMAIL = 1
                    union
                select TOP {maximo} pedidoOcorrenciaColetaEntrega.POC_CODIGO as Codigo from T_PEDIDO_OCORRENCIA_COLETA_ENTREGA pedidoOcorrenciaColetaEntrega where pedidoOcorrenciaColetaEntrega.POC_PENDENTE_ENVIO_SMS = 1
                    union
                select TOP {maximo} pedidoOcorrenciaColetaEntrega.POC_CODIGO as Codigo from T_PEDIDO_OCORRENCIA_COLETA_ENTREGA pedidoOcorrenciaColetaEntrega where pedidoOcorrenciaColetaEntrega.POC_PENDENTE_ENVIO_WHATSAPP = 1");

            return query.List<int>()?.ToList();
        }

        public Dominio.Entidades.Embarcador.Pedidos.PedidoOcorrenciaColetaEntrega BuscarPorPedidoTipoOcorrencia(int codigoOcorrencia, int codigoPedido)
        {
            var query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoOcorrenciaColetaEntrega>();
            query = query.Where(o => o.Pedido.Codigo == codigoPedido && o.TipoDeOcorrencia.Codigo == codigoOcorrencia);

            return query
                .FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.Pedidos.PedidoOcorrenciaColetaEntrega BuscarPorPedidoTipoEventoColetaEntrega(Dominio.ObjetosDeValor.Embarcador.Enumeradores.EventoColetaEntrega eventoColetaEntrega, int codigoPedido)
        {
            var query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoOcorrenciaColetaEntrega>();
            query = query.Where(o => o.Pedido.Codigo == codigoPedido && o.EventoColetaEntrega == eventoColetaEntrega);

            return query
                .FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.Pedidos.PedidoOcorrenciaColetaEntrega BuscarPorPedidoTipoOcorrenciaData(int codigoOcorrencia, int codigoPedido, DateTime dataOcorrencia)
        {
            var query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoOcorrenciaColetaEntrega>();
            query = query.Where(o => o.Pedido.Codigo == codigoPedido && o.TipoDeOcorrencia.Codigo == codigoOcorrencia && o.DataOcorrencia == dataOcorrencia);

            return query
                .FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Pedidos.PedidoOcorrenciaColetaEntrega> ConsultarPendentesIntegracao(int inicioRegistros, int limiteRegistros)
        {
            var consultaPedidoOcorrenciaColetaEntrega = ConsultarPendentesIntegracao();

            return consultaPedidoOcorrenciaColetaEntrega
                .Fetch(o => o.Alvo)
                .Fetch(o => o.TipoDeOcorrencia)
                .Skip(inicioRegistros)
                .Take(limiteRegistros)
                .ToList();
        }

        public int ContarConsultaPendentesIntegracao()
        {
            var consultaPedidoOcorrenciaColetaEntrega = ConsultarPendentesIntegracao();

            return consultaPedidoOcorrenciaColetaEntrega.Count();
        }

        public List<Dominio.Entidades.Embarcador.Pedidos.PedidoOcorrenciaColetaEntrega> ConsultarOcorrenciaConhecimento(int codigoConhecimento, string propOrdena, string dirOrdena, int inicioRegistros, int maximoRegistros)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoOcorrenciaColetaEntrega>();

            var queryPedidoCTe = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedidoXMLNotaFiscalCTe>();
            queryPedidoCTe = queryPedidoCTe.Where(c => c.CargaCTe.CTe.Codigo == codigoConhecimento);

            //var queryCargaCTe = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCTe>();
            //queryCargaCTe = queryCargaCTe.Where(c => c.CTe.Codigo == codigoConhecimento);

            //var queryCargaPedido = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedido>();
            //queryCargaPedido = queryCargaPedido.Where(c => queryCargaCTe.Any(o => o.Carga == c.Carga));

            query = query.Where(c => queryPedidoCTe.Any(o => o.PedidoXMLNotaFiscal.CargaPedido.Pedido == c.Pedido));
            //var resultCargaCTe = queryPedidoCTe.Join(query, vei => vei.PedidoXMLNotaFiscal.CargaPedido.Pedido.Codigo, emp => emp.Pedido.Codigo, (vei, emp) => emp);

            if (!string.IsNullOrWhiteSpace(propOrdena))
                query = query.OrderBy(propOrdena + (dirOrdena == "asc" ? " ascending" : " descending"));

            if (inicioRegistros > 0)
                query = query.Skip(inicioRegistros);

            if (maximoRegistros > 0)
                query = query.Take(maximoRegistros);

            return query.ToList();
        }

        public int ContarConsultarOcorrenciaConhecimento(int codigoConhecimento)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoOcorrenciaColetaEntrega>();

            var queryPedidoCTe = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedidoXMLNotaFiscalCTe>();
            queryPedidoCTe = queryPedidoCTe.Where(c => c.CargaCTe.CTe.Codigo == codigoConhecimento);

            //var queryCargaCTe = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCTe>();
            //queryCargaCTe = queryCargaCTe.Where(c => c.CTe.Codigo == codigoConhecimento);

            //var queryCargaPedido = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedido>();
            //queryCargaPedido = queryCargaPedido.Where(c => queryCargaCTe.Any(o => o.Carga == c.Carga));

            query = query.Where(c => queryPedidoCTe.Any(o => o.PedidoXMLNotaFiscal.CargaPedido.Pedido == c.Pedido));
            //var resultCargaCTe = queryPedidoCTe.Join(query, vei => vei.PedidoXMLNotaFiscal.CargaPedido.Pedido.Codigo, emp => emp.Pedido.Codigo, (vei, emp) => emp);

            return query.Count();
        }

        public List<Dominio.Entidades.Embarcador.Pedidos.PedidoOcorrenciaColetaEntrega> ConsultarOcorrenciasDocumento(double codigoRecebedor, string numeroPedidoCliente, string numeroSolicitacao, string numeroNota, int serieNota, int numeroCTe, int serieCTe, int numeroPedido, double codigoCliente, double codigoRemetente, double codigoDestinatario, int inicio, int limite, string numeroPedidoNoCliente)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCTe>();

            if (codigoCliente > 0)
                query = query.Where(o => o.CTe.XMLNotaFiscais.Any(nf => nf.Destinatario.CPF_CNPJ == codigoCliente || nf.Recebedor.CPF_CNPJ == codigoCliente || nf.Emitente.CPF_CNPJ == codigoCliente));

            if (!string.IsNullOrWhiteSpace(numeroNota))
                query = query.Where(o => o.CTe.Documentos.Any(nf => nf.Numero.Equals(numeroNota)));
            if (serieNota > 0)
                query = query.Where(o => o.CTe.Documentos.Any(nf => nf.Serie.Equals(serieNota.ToString())));

            if (numeroCTe > 0)
                query = query.Where(o => o.CTe.Numero == numeroCTe);
            if (serieCTe > 0)
                query = query.Where(o => o.CTe.Serie.Numero == serieCTe);


            if (!string.IsNullOrWhiteSpace(numeroSolicitacao))
                query = query.Where(o => o.CTe.XMLNotaFiscais.Any(nf => nf.NumeroSolicitacao == numeroSolicitacao));

            if (!string.IsNullOrWhiteSpace(numeroPedidoCliente))
                query = query.Where(o => o.CTe.Documentos.Any(r => r.NumeroPedido == numeroPedidoCliente));

            if (numeroPedido > 0)
                query = query.Where(o => o.Carga.Pedidos.Any(ped => ped.Pedido.Numero == numeroPedido));

            if (codigoRemetente > 0)
                query = query.Where(o => o.CTe.Remetente.Cliente.CPF_CNPJ == codigoRemetente);
            if (codigoDestinatario > 0)
                query = query.Where(o => o.CTe.Destinatario.Cliente.CPF_CNPJ == codigoDestinatario);
            if (codigoRecebedor > 0)
                query = query.Where(o => o.CTe.Recebedor.Cliente.CPF_CNPJ == codigoRecebedor);

            var queryPeidoOcorrencia = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoOcorrenciaColetaEntrega>();
            var queryCargaPedido = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedido>();
            if (!string.IsNullOrWhiteSpace(numeroPedidoNoCliente))
                queryCargaPedido = queryCargaPedido.Where(c => c.Pedido.CodigoPedidoCliente == numeroPedidoNoCliente);
            queryCargaPedido = queryCargaPedido.Where(c => query.Any(o => o.Carga == c.Carga));

            queryPeidoOcorrencia = queryPeidoOcorrencia.Where(c => queryCargaPedido.Any(o => o.Pedido == c.Pedido && (o.Pedido.Destinatario == c.Alvo || o.Pedido.Remetente == c.Alvo)));

            return queryPeidoOcorrencia
                .Fetch(o => o.TipoDeOcorrencia)
                .Skip(inicio).Take(limite)
                .ToList();
        }

        public int ContarOcorrenciasDocumento(double codigoRecebedor, string numeroPedidoCliente, string numeroSolicitacao, string numeroNota, int serieNota, int numeroCTe, int serieCTe, int numeroPedido, double codigoCliente, double codigoRemetente, double codigoDestinatario, string numeroPedidoNoCliente)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCTe>();

            if (codigoCliente > 0)
                query = query.Where(o => o.CTe.XMLNotaFiscais.Any(nf => nf.Destinatario.CPF_CNPJ == codigoCliente || nf.Recebedor.CPF_CNPJ == codigoCliente || nf.Emitente.CPF_CNPJ == codigoCliente));

            if (!string.IsNullOrWhiteSpace(numeroNota))
                query = query.Where(o => o.CTe.Documentos.Any(nf => nf.Numero.Equals(numeroNota)));
            if (serieNota > 0)
                query = query.Where(o => o.CTe.Documentos.Any(nf => nf.Serie.Equals(serieNota.ToString())));

            if (numeroCTe > 0)
                query = query.Where(o => o.CTe.Numero == numeroCTe);
            if (serieCTe > 0)
                query = query.Where(o => o.CTe.Serie.Numero == serieCTe);


            if (!string.IsNullOrWhiteSpace(numeroSolicitacao))
                query = query.Where(o => o.CTe.XMLNotaFiscais.Any(nf => nf.NumeroSolicitacao == numeroSolicitacao));

            if (!string.IsNullOrWhiteSpace(numeroPedidoCliente))
                query = query.Where(o => o.CTe.Documentos.Any(r => r.NumeroPedido == numeroPedidoCliente));

            if (numeroPedido > 0)
                query = query.Where(o => o.Carga.Pedidos.Any(ped => ped.Pedido.Numero == numeroPedido));

            if (codigoRemetente > 0)
                query = query.Where(o => o.CTe.Remetente.Cliente.CPF_CNPJ == codigoRemetente);
            if (codigoDestinatario > 0)
                query = query.Where(o => o.CTe.Destinatario.Cliente.CPF_CNPJ == codigoDestinatario);
            if (codigoRecebedor > 0)
                query = query.Where(o => o.CTe.Recebedor.Cliente.CPF_CNPJ == codigoRecebedor);

            var queryPeidoOcorrencia = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoOcorrenciaColetaEntrega>();
            var queryCargaPedido = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedido>();
            if (!string.IsNullOrWhiteSpace(numeroPedidoNoCliente))
                queryCargaPedido = queryCargaPedido.Where(c => c.Pedido.CodigoPedidoCliente == numeroPedidoNoCliente);
            queryCargaPedido = queryCargaPedido.Where(c => query.Any(o => o.Carga == c.Carga));

            queryPeidoOcorrencia = queryPeidoOcorrencia.Where(c => queryCargaPedido.Any(o => o.Pedido == c.Pedido && (o.Pedido.Destinatario == c.Alvo || o.Pedido.Remetente == c.Alvo)));

            return queryPeidoOcorrencia.Count();
        }

        public bool ExistePorPedidoTipoEventoColetaEntrega(Dominio.ObjetosDeValor.Embarcador.Enumeradores.EventoColetaEntrega eventoColetaEntrega, int codigoPedido)
        {
            var query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoOcorrenciaColetaEntrega>()
                .Where(o => o.Pedido.Codigo == codigoPedido && o.EventoColetaEntrega == eventoColetaEntrega);

            return query.Count() > 0;
        }

        public bool ExistePorPedidoTipoOcorrenciaEmTempo(int codigoTipoOcorrencia, int tempoRecalculo, int codigoPedido)
        {
            var query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoOcorrenciaColetaEntrega>()
                .Where(o => o.Pedido.Codigo == codigoPedido && o.TipoDeOcorrencia.Codigo == codigoTipoOcorrencia && o.DataOcorrencia <= DateTime.Now.AddMinutes(tempoRecalculo));

            return query.Count() > 0;
        }

        public List<Dominio.Entidades.Embarcador.Pedidos.PedidoOcorrenciaColetaEntrega> BuscarOcorrenciasPorUltimoRegistroIntegradoPorTipoIntegracao(int codigoIntegracao)
        {
            var consulta = this.SessionNHiBernate.CreateSQLQuery(QueryConsultaUltimosRegistrosIntegradosPorTipoIntegracao(codigoIntegracao));

            return BuscarPorCodigos(consulta.SetTimeout(120).List<int>());
        }

        public List<Dominio.Entidades.Embarcador.Pedidos.PedidoOcorrenciaColetaEntrega> BuscarPorCodigos(IList<int> codigos)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoOcorrenciaColetaEntrega>()
                .Where(o => codigos.Contains(o.Codigo));

            return query.ToList();
        }

        #endregion

        #region Relatórios

        public IList<Dominio.Relatorios.Embarcador.DataSource.Pedidos.PedidoOcorrencia> ConsultarRelatorioPedidoOcorrencia(Dominio.ObjetosDeValor.Embarcador.Pedido.FiltroPesquisaRelatorioPedidoOcorrencia filtrosPesquisa, List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedades, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            var query = new ConsultaPedidoOcorrencia().ObterSqlPesquisa(filtrosPesquisa, parametrosConsulta, propriedades).CriarSQLQuery(this.SessionNHiBernate);

            query.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Dominio.Relatorios.Embarcador.DataSource.Pedidos.PedidoOcorrencia)));

            return query.SetTimeout(600).List<Dominio.Relatorios.Embarcador.DataSource.Pedidos.PedidoOcorrencia>();
        }

        public int ContarConsultaRelatorioPedidoOcorrencia(Dominio.ObjetosDeValor.Embarcador.Pedido.FiltroPesquisaRelatorioPedidoOcorrencia filtrosPesquisa, List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedades)
        {
            var query = new ConsultaPedidoOcorrencia().ObterSqlContarPesquisa(filtrosPesquisa, propriedades).CriarSQLQuery(this.SessionNHiBernate);

            return query.SetTimeout(600).UniqueResult<int>();
        }

        #endregion
    }
}
