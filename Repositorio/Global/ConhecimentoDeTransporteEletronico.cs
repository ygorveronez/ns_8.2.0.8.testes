using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Dominio.Relatorios.Embarcador.DataSource.CTe;
using Dominio.Relatorios.Embarcador.ObjetosDeValor;
using MongoDB.Driver;
using NHibernate.Criterion;
using NHibernate.Linq;
using Repositorio.Embarcador.Consulta;
using Repositorio.Embarcador.CTe;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Repositorio
{
    public class ConhecimentoDeTransporteEletronico : RepositorioBase<Dominio.Entidades.ConhecimentoDeTransporteEletronico>, Dominio.Interfaces.Repositorios.ConhecimentoDeTransporteEletronico
    {
        #region Contrutores

        public ConhecimentoDeTransporteEletronico(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public ConhecimentoDeTransporteEletronico(UnitOfWork unitOfWork, CancellationToken cancellationToken) : base(unitOfWork, cancellationToken) { }

        #endregion

        #region Métodos Públicos

        /// <summary>
        /// Remover, utilizado somente para migração dos dados do Tombini
        /// </summary>
        public IList<int> BuscarCodigoCTeVersao30EmCargaSemVeiculoParaMigracao()
        {
            var query = this.SessionNHiBernate.CreateSQLQuery("select con_codigo from t_cte where con_versao = '3.00' and con_codigo in (select con_codigo from t_carga_cte) and con_status = 'A' and con_codigo not in (select con_codigo from t_cte_veiculo)");

            return query.List<int>();
        }

        public DateTime? BuscarDataDescarga(int viagemCinco, int viagemQuatro, int viagemTres, int viagemDois, int viagemUm, int viagem, int portoDestino, int terminalDestino)
        {
            var query = this.SessionNHiBernate.CreateSQLQuery(@"SELECT TOP(1)
                ISNULL((SELECT TOP(1) Schedule.PVS_DATA_PREVISAO_CHEGADA_NAVIO FROM T_PEDIDO_VIAGEM_NAVIO_SCHEDULE Schedule WHERE Schedule.PVN_CODIGO = " + viagemCinco + @" AND Schedule.POT_CODIGO_ATRACACAO = " + portoDestino + @" AND Schedule.TTI_CODIGO_ATRACACAO = " + terminalDestino + @"),
                ISNULL((SELECT TOP(1) Schedule.PVS_DATA_PREVISAO_CHEGADA_NAVIO FROM T_PEDIDO_VIAGEM_NAVIO_SCHEDULE Schedule WHERE Schedule.PVN_CODIGO = " + viagemQuatro + @" AND Schedule.POT_CODIGO_ATRACACAO = " + portoDestino + @" AND Schedule.TTI_CODIGO_ATRACACAO = " + terminalDestino + @"),
                ISNULL((SELECT TOP(1) Schedule.PVS_DATA_PREVISAO_CHEGADA_NAVIO FROM T_PEDIDO_VIAGEM_NAVIO_SCHEDULE Schedule WHERE Schedule.PVN_CODIGO = " + viagemTres + @" AND Schedule.POT_CODIGO_ATRACACAO = " + portoDestino + @" AND Schedule.TTI_CODIGO_ATRACACAO = " + terminalDestino + @"),
                ISNULL((SELECT TOP(1) Schedule.PVS_DATA_PREVISAO_CHEGADA_NAVIO FROM T_PEDIDO_VIAGEM_NAVIO_SCHEDULE Schedule WHERE Schedule.PVN_CODIGO = " + viagemDois + @"  AND Schedule.POT_CODIGO_ATRACACAO = " + portoDestino + @" AND Schedule.TTI_CODIGO_ATRACACAO = " + terminalDestino + @"),
                ISNULL((SELECT TOP(1) Schedule.PVS_DATA_PREVISAO_CHEGADA_NAVIO FROM T_PEDIDO_VIAGEM_NAVIO_SCHEDULE Schedule WHERE Schedule.PVN_CODIGO = " + viagemUm + @" AND Schedule.POT_CODIGO_ATRACACAO = " + portoDestino + @" AND Schedule.TTI_CODIGO_ATRACACAO = " + terminalDestino + @"),
                (SELECT TOP(1) Schedule.PVS_DATA_PREVISAO_CHEGADA_NAVIO FROM T_PEDIDO_VIAGEM_NAVIO_SCHEDULE Schedule WHERE Schedule.PVN_CODIGO = " + viagem + @" AND Schedule.POT_CODIGO_ATRACACAO = " + portoDestino + @" AND Schedule.TTI_CODIGO_ATRACACAO = " + terminalDestino + @")))))) DataDescarga
                FROM T_EMPRESA WHERE 1 = 1");

            return query.UniqueResult<DateTime?>();
        }

        public DateTime? BuscarDataPrevisaoSaidaNavio(int viagemCinco, int viagemQuatro, int viagemTres, int viagemDois, int viagemUm, int viagem, int portoDestino, int terminalDestino)
        {
            var query = this.SessionNHiBernate.CreateSQLQuery(@"SELECT TOP(1)
                ISNULL((SELECT TOP(1) Schedule.PVS_DATA_PREVISAO_SAIDA_NAVIO FROM T_PEDIDO_VIAGEM_NAVIO_SCHEDULE Schedule WHERE Schedule.PVN_CODIGO = " + viagemCinco + @" AND Schedule.POT_CODIGO_ATRACACAO = " + portoDestino + @" AND Schedule.TTI_CODIGO_ATRACACAO = " + terminalDestino + @"),
                ISNULL((SELECT TOP(1) Schedule.PVS_DATA_PREVISAO_SAIDA_NAVIO FROM T_PEDIDO_VIAGEM_NAVIO_SCHEDULE Schedule WHERE Schedule.PVN_CODIGO = " + viagemQuatro + @" AND Schedule.POT_CODIGO_ATRACACAO = " + portoDestino + @" AND Schedule.TTI_CODIGO_ATRACACAO = " + terminalDestino + @"),
                ISNULL((SELECT TOP(1) Schedule.PVS_DATA_PREVISAO_SAIDA_NAVIO FROM T_PEDIDO_VIAGEM_NAVIO_SCHEDULE Schedule WHERE Schedule.PVN_CODIGO = " + viagemTres + @" AND Schedule.POT_CODIGO_ATRACACAO = " + portoDestino + @" AND Schedule.TTI_CODIGO_ATRACACAO = " + terminalDestino + @"),
                ISNULL((SELECT TOP(1) Schedule.PVS_DATA_PREVISAO_SAIDA_NAVIO FROM T_PEDIDO_VIAGEM_NAVIO_SCHEDULE Schedule WHERE Schedule.PVN_CODIGO = " + viagemDois + @"  AND Schedule.POT_CODIGO_ATRACACAO = " + portoDestino + @" AND Schedule.TTI_CODIGO_ATRACACAO = " + terminalDestino + @"),
                ISNULL((SELECT TOP(1) Schedule.PVS_DATA_PREVISAO_SAIDA_NAVIO FROM T_PEDIDO_VIAGEM_NAVIO_SCHEDULE Schedule WHERE Schedule.PVN_CODIGO = " + viagemUm + @" AND Schedule.POT_CODIGO_ATRACACAO = " + portoDestino + @" AND Schedule.TTI_CODIGO_ATRACACAO = " + terminalDestino + @"),
                (SELECT TOP(1) Schedule.PVS_DATA_PREVISAO_SAIDA_NAVIO FROM T_PEDIDO_VIAGEM_NAVIO_SCHEDULE Schedule WHERE Schedule.PVN_CODIGO = " + viagem + @" AND Schedule.POT_CODIGO_ATRACACAO = " + portoDestino + @" AND Schedule.TTI_CODIGO_ATRACACAO = " + terminalDestino + @")))))) DataPrevisaoSaidaNavio
                FROM T_EMPRESA WHERE 1 = 1");

            return query.UniqueResult<DateTime?>();
        }

        public int BuscarCodigoSchedule(int viagemCinco, int viagemQuatro, int viagemTres, int viagemDois, int viagemUm, int viagem, int portoDestino, int terminalDestino)
        {
            var query = this.SessionNHiBernate.CreateSQLQuery(@"SELECT TOP(1)
                ISNULL((SELECT TOP(1) Schedule.PVS_CODIGO FROM T_PEDIDO_VIAGEM_NAVIO_SCHEDULE Schedule WHERE Schedule.PVN_CODIGO = " + viagemCinco + @" AND Schedule.POT_CODIGO_ATRACACAO = " + portoDestino + @" AND Schedule.TTI_CODIGO_ATRACACAO = " + terminalDestino + @"),
                ISNULL((SELECT TOP(1) Schedule.PVS_CODIGO FROM T_PEDIDO_VIAGEM_NAVIO_SCHEDULE Schedule WHERE Schedule.PVN_CODIGO = " + viagemQuatro + @" AND Schedule.POT_CODIGO_ATRACACAO = " + portoDestino + @" AND Schedule.TTI_CODIGO_ATRACACAO = " + terminalDestino + @"),
                ISNULL((SELECT TOP(1) Schedule.PVS_CODIGO FROM T_PEDIDO_VIAGEM_NAVIO_SCHEDULE Schedule WHERE Schedule.PVN_CODIGO = " + viagemTres + @" AND Schedule.POT_CODIGO_ATRACACAO = " + portoDestino + @" AND Schedule.TTI_CODIGO_ATRACACAO = " + terminalDestino + @"),
                ISNULL((SELECT TOP(1) Schedule.PVS_CODIGO FROM T_PEDIDO_VIAGEM_NAVIO_SCHEDULE Schedule WHERE Schedule.PVN_CODIGO = " + viagemDois + @"  AND Schedule.POT_CODIGO_ATRACACAO = " + portoDestino + @" AND Schedule.TTI_CODIGO_ATRACACAO = " + terminalDestino + @"),
                ISNULL((SELECT TOP(1) Schedule.PVS_CODIGO FROM T_PEDIDO_VIAGEM_NAVIO_SCHEDULE Schedule WHERE Schedule.PVN_CODIGO = " + viagemUm + @" AND Schedule.POT_CODIGO_ATRACACAO = " + portoDestino + @" AND Schedule.TTI_CODIGO_ATRACACAO = " + terminalDestino + @"),
                (SELECT TOP(1) Schedule.PVS_CODIGO FROM T_PEDIDO_VIAGEM_NAVIO_SCHEDULE Schedule WHERE Schedule.PVN_CODIGO = " + viagem + @" AND Schedule.POT_CODIGO_ATRACACAO = " + portoDestino + @" AND Schedule.TTI_CODIGO_ATRACACAO = " + terminalDestino + @")))))) CodigoSchedule
                FROM T_EMPRESA WHERE 1 = 1");

            return query.UniqueResult<int>();
        }

        public List<int> BuscarNumerosPorCodigos(List<int> codigosCTes)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.ConhecimentoDeTransporteEletronico>();
            query = query.Where(o => codigosCTes.Contains(o.Codigo));
            return query.Select(o => o.Numero).Distinct().ToList();
        }

        public List<Dominio.Entidades.ConhecimentoDeTransporteEletronico> BuscarPorCargaEStatus(int codigoCarga, string statusCTE)
        {
            IQueryable<Dominio.Entidades.Embarcador.Cargas.CargaCTe> queryCargaCTe = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCTe>();

            var ctes = queryCargaCTe.Where(o => o.Carga.Codigo == codigoCarga && o.CTe.Status == statusCTE).Select(d => d.CTe).ToList();

            return ctes.ToList();
        }

        public List<int> BuscarCodigosPorDataAlteracao(DateTime dataUltimoProcessamento, DateTime dataProcessamentoAtual)
        {
            string[] status = new string[] { "A", "Z", "C" };

            IQueryable<Dominio.Entidades.Embarcador.Cargas.CargaCTe> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCTe>();

            //query = query.Where(o => o.Carga.TipoOperacao == null || !o.Carga.TipoOperacao.NaoGerarFaturamento);
            query = query.Where(o => status.Contains(o.CTe.Status) &&
                                     !o.CTe.ModeloDocumentoFiscal.NaoGerarFaturamento &&
                                     o.CTe.Veiculos.Count > 0 &&
                                     ((o.CTe.DataAutorizacao > dataUltimoProcessamento && o.CTe.DataAutorizacao <= dataProcessamentoAtual) ||
                                      (o.CTe.DataCancelamento > dataUltimoProcessamento && o.CTe.DataCancelamento <= dataProcessamentoAtual) ||
                                      (o.CTe.DataAnulacao > dataUltimoProcessamento && o.CTe.DataAnulacao <= dataProcessamentoAtual) ||
                                      (o.DataVinculoCarga > dataUltimoProcessamento && o.DataVinculoCarga <= dataProcessamentoAtual)));

            return query.Select(o => o.CTe.Codigo).ToList();
        }

        public List<Dominio.Entidades.ConhecimentoDeTransporteEletronico> ConsultarPorNumeroOS(int codigoCargaPedido, string numeroOS, string propOrdenacao, string dirOrdenacao, int inicioRegistros, int maximoRegistros)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.ConhecimentoDeTransporteEletronico>();

            query = query.Where(obj => obj.NumeroOS == numeroOS && obj.NumeroOS != "" && obj.Status == "A");

            return query.OrderBy(propOrdenacao + " " + dirOrdenacao)
                        .Skip(inicioRegistros)
                        .Take(maximoRegistros)
                        .ToList();
        }

        public int ContarPorNumeroOS(int codigoCargaPedido, string numeroOS)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.ConhecimentoDeTransporteEletronico>();

            query = query.Where(obj => obj.NumeroOS == numeroOS && obj.NumeroOS != "" && obj.Status == "A");

            return query.Count();
        }

        public Dominio.Entidades.ConhecimentoDeTransporteEletronico BuscarPorId(int codigoCTe, int codigoEmpresa)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.ConhecimentoDeTransporteEletronico>();

            var result = from obj in query where obj.Codigo == codigoCTe && obj.Empresa.Codigo == codigoEmpresa select obj;

            return result.FirstOrDefault();
        }

        public List<int> BuscarXMLNotaFiscalPorId(int codigoCTe)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.ConhecimentoDeTransporteEletronico>();

            var result = from obj in query where obj.Codigo == codigoCTe select obj;

            return result.Select(obj => obj.XMLNotaFiscais.Select(nf => nf.Codigo)).SelectMany(o => o).ToList();
        }

        public List<int> BuscarCTesMultimodalPendenteTerceiro(DateTime data, int codigoCarga)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.ConhecimentoDeTransporteEletronico>();
            query = query.Where(obj => obj.TipoModal == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoModal.Multimodal && obj.Status == "A"); ;
            if (codigoCarga == 0)
                query = query.Where(obj => obj.DataEmissao >= data);

            var queryTerceiro = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.CTe.CTeTerceiro>();
            query = query.Where(o => !queryTerceiro.Any(t => t.ChaveAcesso == o.Chave));

            if (codigoCarga > 0)
            {
                var queryCarga = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedidoDocumentoCTe>();
                query = query.Where(o => queryCarga.Any(t => t.CTe == o && t.CargaPedido.Carga.Codigo == codigoCarga));
            }

            return query.Select(obj => obj.Codigo).ToList();
        }

        public bool ContemCTeAutorizadoParaNotaFiscal(int codigoNota)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.ConhecimentoDeTransporteEletronico>();

            var result = from obj in query where obj.Status == "A" && obj.XMLNotaFiscais.Any(o => o.Codigo == codigoNota) select obj;

            return result.Any();
        }

        public List<Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal> BuscarXMLNotaFiscalPorCodigo(int codigoCTe)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.ConhecimentoDeTransporteEletronico>();

            var result = from obj in query where obj.Codigo == codigoCTe select obj;

            return result.Select(obj => obj.XMLNotaFiscais).SelectMany(o => o).ToList();
        }

        public List<Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal> BuscarXMLNotaFiscalPorChaves(List<string> chavesCTes)
        {
            IQueryable<Dominio.Entidades.ConhecimentoDeTransporteEletronico> query = this.SessionNHiBernate.Query<Dominio.Entidades.ConhecimentoDeTransporteEletronico>();

            int quantidadeRegistrosConsultarPorVez = 100;
            int quantidadeConsultas = chavesCTes.Count / quantidadeRegistrosConsultarPorVez;

            List<Dominio.Entidades.ConhecimentoDeTransporteEletronico> ctesRetornar = new List<Dominio.Entidades.ConhecimentoDeTransporteEletronico>();

            for (int i = 0; i <= quantidadeConsultas; i++)
                ctesRetornar.AddRange(query.Where(o => chavesCTes.Skip(i * quantidadeRegistrosConsultarPorVez).Take(quantidadeRegistrosConsultarPorVez).Contains(o.Chave)).ToList());

            return ctesRetornar.Select(obj => obj.XMLNotaFiscais).SelectMany(o => o).ToList();
        }

        public List<string> BuscarChaves(List<int> codigosCTes)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.ConhecimentoDeTransporteEletronico>();

            query = query.Where(o => codigosCTes.Contains(o.Codigo));

            return query.Select(o => o.Chave).ToList();
        }

        public List<Dominio.Entidades.ConhecimentoDeTransporteEletronico> BuscarCTesPorCodigos(List<int> codigosCTes)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.ConhecimentoDeTransporteEletronico>();

            query = query.Where(o => codigosCTes.Contains(o.Codigo));

            return query.ToList();
        }

        public List<int> BuscarPorNumeroControle(string numeroControle)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.ConhecimentoDeTransporteEletronico>();

            query = query.Where(o => o.NumeroControle == numeroControle);

            return query.Select(c => c.Codigo).ToList();
        }

        public List<int> BuscarPorNumeroCEMercante(string numeroCEMercante)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.ConhecimentoDeTransporteEletronico>();

            query = query.Where(o => o.NumeroCEMercante == numeroCEMercante);

            return query.Select(c => c.Codigo).ToList();
        }

        public List<Dominio.Entidades.ConhecimentoDeTransporteEletronico> BuscarPorCodigo(List<int> codigosCTes)
        {
            List<Dominio.Entidades.ConhecimentoDeTransporteEletronico> result = new List<Dominio.Entidades.ConhecimentoDeTransporteEletronico>();

            int take = 1000;
            int start = 0;
            while (start < codigosCTes?.Count)
            {
                List<int> tmp = codigosCTes.Skip(start).Take(take).ToList();

                var query = this.SessionNHiBernate.Query<Dominio.Entidades.ConhecimentoDeTransporteEletronico>();
                var filter = from obj in query
                             where tmp.Contains(obj.Codigo)
                             select obj;

                result.AddRange(filter.Fetch(o => o.Empresa).Fetch(o => o.ModeloDocumentoFiscal).Fetch(o => o.Serie).ToList());

                start += take;
            }

            return result;
        }

        public List<Dominio.Entidades.ConhecimentoDeTransporteEletronico> BuscarPorCodigoEDestinoPorta(List<int> codigosCTes)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.ConhecimentoDeTransporteEletronico>();
            var queryCargaPedido = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedido>();
            var queryCargaCTe = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCTe>();

            query = query.Where(o => codigosCTes.Contains(o.Codigo));

            queryCargaPedido = queryCargaPedido.Where(obj => obj.ModalPropostaMultimodal == Dominio.ObjetosDeValor.Embarcador.Enumeradores.ModalPropostaMultimodal.PortaPorta || obj.ModalPropostaMultimodal == Dominio.ObjetosDeValor.Embarcador.Enumeradores.ModalPropostaMultimodal.PortoPorta);
            queryCargaCTe = queryCargaCTe.Where(obj => queryCargaPedido.Any(o => o.Carga == obj.Carga));

            query = query.Where(obj => queryCargaCTe.Any(o => o.CTe == obj));
            return query.Fetch(o => o.Empresa).Fetch(o => o.ModeloDocumentoFiscal).Fetch(o => o.Serie).ToList();
        }

        public List<int> BuscarCodigosContaineres(List<int> codigosCTes)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.ContainerCTE>();

            query = query.Where(o => codigosCTes.Contains(o.CTE.Codigo) && o.Container != null);

            return query.Select(c => c.Container.Codigo).Distinct().ToList();
        }

        public List<Dominio.Entidades.ConhecimentoDeTransporteEletronico> BuscarPorCodigoEDestinoPorto(List<int> codigosCTes)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.ConhecimentoDeTransporteEletronico>();
            var queryCargaPedido = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedido>();
            var queryCargaCTe = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCTe>();

            query = query.Where(o => codigosCTes.Contains(o.Codigo));

            queryCargaPedido = queryCargaPedido.Where(obj => obj.ModalPropostaMultimodal == Dominio.ObjetosDeValor.Embarcador.Enumeradores.ModalPropostaMultimodal.PortaPorto || obj.ModalPropostaMultimodal == Dominio.ObjetosDeValor.Embarcador.Enumeradores.ModalPropostaMultimodal.PortoPorto);
            queryCargaCTe = queryCargaCTe.Where(obj => queryCargaPedido.Any(o => o.Carga == obj.Carga));

            query = query.Where(obj => queryCargaCTe.Any(o => o.CTe == obj));
            return query.Fetch(o => o.Empresa).Fetch(o => o.ModeloDocumentoFiscal).Fetch(o => o.Serie).ToList();
        }

        public List<KeyValuePair<string, string>> ConsultarSeExisteCTePendente(int codigoEmpresa, DateTime dataFechamento, string[] status)
        {
            IQueryable<Dominio.Entidades.ConhecimentoDeTransporteEletronico> query = this.SessionNHiBernate.Query<Dominio.Entidades.ConhecimentoDeTransporteEletronico>();

            query = query.Where(o => o.DataEmissao < dataFechamento.AddDays(1).Date && status.Contains(o.Status));

            if (codigoEmpresa > 0)
                query = query.Where(c => c.Empresa.Codigo == codigoEmpresa);

            return query.Select(o => new KeyValuePair<string, string>(o.ModeloDocumentoFiscal.Abreviacao, o.Numero + "-" + o.Serie.Numero)).Distinct().ToList();
        }

        public int ContarCTePorChaveUnica(int numeroCTe, int codigoSerie, int codigoModeloDocumento, int codigoEmpresa, Dominio.Enumeradores.TipoAmbiente tipoAmbiente)
        {
            IQueryable<Dominio.Entidades.ConhecimentoDeTransporteEletronico> query = this.SessionNHiBernate.Query<Dominio.Entidades.ConhecimentoDeTransporteEletronico>();

            query = query.Where(o => o.Numero == numeroCTe && o.Serie.Codigo == codigoSerie && o.Empresa.Codigo == codigoEmpresa && o.ModeloDocumentoFiscal.Codigo == codigoModeloDocumento && o.TipoAmbiente == tipoAmbiente);

            return query.Count();
        }
        public int ContarCTePorChaveUnicaEStatus(int numeroCTe, int codigoSerie, int codigoModeloDocumento, int codigoEmpresa, Dominio.Enumeradores.TipoAmbiente tipoAmbiente, string[] status)
        {
            IQueryable<Dominio.Entidades.ConhecimentoDeTransporteEletronico> query = this.SessionNHiBernate.Query<Dominio.Entidades.ConhecimentoDeTransporteEletronico>();

            query = query.Where(o => o.Numero == numeroCTe && o.Serie.Codigo == codigoSerie && o.Empresa.Codigo == codigoEmpresa && o.ModeloDocumentoFiscal.Codigo == codigoModeloDocumento && o.TipoAmbiente == tipoAmbiente && status.Contains(o.Status));

            return query.Count();
        }
        public int VerificaNFSeJaAutorizada(int numeroCTe, int codigoSerie, int codigoModeloDocumento, int codigoEmpresa, int ano, Dominio.Enumeradores.TipoAmbiente tipoAmbiente, string codigoVerificacao)
        {
            IQueryable<Dominio.Entidades.ConhecimentoDeTransporteEletronico> query = this.SessionNHiBernate.Query<Dominio.Entidades.ConhecimentoDeTransporteEletronico>();

            query = query.Where(o => o.Status == "A" && o.Numero == numeroCTe && o.Serie.Codigo == codigoSerie && o.Empresa.Codigo == codigoEmpresa && o.ModeloDocumentoFiscal.Codigo == codigoModeloDocumento && o.TipoAmbiente == tipoAmbiente && o.DataEmissao.Value.Year == ano && o.Protocolo == codigoVerificacao);

            return query.Count();
        }

        public Dominio.Entidades.ConhecimentoDeTransporteEletronico BuscarPorProtocoloIntegracaoOracle(int protocoloIntegracao)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.ConhecimentoDeTransporteEletronico>();

            var result = from obj in query where obj.CodigoCTeIntegrador == protocoloIntegracao && (obj.ModeloDocumentoFiscal.Numero.Equals("57") || obj.ModeloDocumentoFiscal.Numero.Equals("67")) select obj;

            return result.FirstOrDefault();
        }

        public Dominio.Entidades.ConhecimentoDeTransporteEletronico BuscarNFSePorProtocoloIntegracaoOracle(int protocoloIntegracao)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.ConhecimentoDeTransporteEletronico>();

            var result = from obj in query where obj.CodigoCTeIntegrador == protocoloIntegracao && obj.ModeloDocumentoFiscal.Numero == "39" select obj;

            return result.FirstOrDefault();
        }

        public Dominio.Entidades.ConhecimentoDeTransporteEletronico BuscarCTePorProtocoloIntegracaoOracle(int protocoloIntegracao)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.ConhecimentoDeTransporteEletronico>();

            var result = from obj in query where obj.CodigoCTeIntegrador == protocoloIntegracao && (obj.ModeloDocumentoFiscal.Numero.Equals("57") || obj.ModeloDocumentoFiscal.Numero.Equals("67")) select obj;

            return result.FirstOrDefault();
        }

        public Dominio.Entidades.ConhecimentoDeTransporteEletronico BuscarPorCodigoInutilizacao(string codigoInutilizacao)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.ConhecimentoDeTransporteEletronico>();

            var result = from obj in query where obj.ProtocoloCancelamentoInutilizacao == codigoInutilizacao select obj;

            return result.FirstOrDefault();
        }

        public Dominio.Entidades.ConhecimentoDeTransporteEletronico BuscarPorRPS(int codigoRPS)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.ConhecimentoDeTransporteEletronico>();

            var result = from obj in query where obj.RPS.Codigo == codigoRPS select obj;

            return result.FirstOrDefault();
        }

        public void SetarDACTEGerado(int codigoCTe)
        {
            string hql = "UPDATE ConhecimentoDeTransporteEletronico cte SET cte.GerouDACTE = :gerouDACTE where cte.Codigo = :codigoCTe";

            var query = this.SessionNHiBernate.CreateQuery(hql);

            query.SetInt32("codigoCTe", codigoCTe);
            query.SetBoolean("gerouDACTE", true);

            query.ExecuteUpdate();
        }
        public void AlterarStatusCTes(int codigoCTe, string status)
        {
            string sql = "UPDATE ConhecimentoDeTransporteEletronico cte SET cte.Status = :status WHERE Codigo = :codigoCTe";

            var query = this.SessionNHiBernate.CreateQuery(sql);

            query.SetInt32("codigoCTe", codigoCTe);
            query.SetString("status", status);

            query.ExecuteUpdate();
        }
        public void AutorizarFaturamentosCTe(int codigoCTe)
        {
            string sql = "UPDATE DocumentoFaturamento fat SET fat.Situacao = 1 WHERE fat.CTe.Codigo = :codigoCTe";

            var query = this.SessionNHiBernate.CreateQuery(sql);

            query.SetInt32("codigoCTe", codigoCTe);

            query.ExecuteUpdate();
        }

        public List<int> BuscarOutrosDocumentosParaGeracaoDACTE(int limiteRegistros)
        {
            IQueryable<Dominio.Entidades.ConhecimentoDeTransporteEletronico> query = this.SessionNHiBernate.Query<Dominio.Entidades.ConhecimentoDeTransporteEletronico>();

            query = query.Where(o => o.ModeloDocumentoFiscal.TipoDocumentoEmissao == Dominio.Enumeradores.TipoDocumento.Outros && (o.GerouDACTE == null || !o.GerouDACTE.Value) && o.Status == "A");

            return query.Select(o => o.Codigo).Take(limiteRegistros).ToList();
        }

        public IList<Dominio.ObjetosDeValor.Embarcador.Financeiro.CTeTituloReceber> ConsultarTituloReceberCTe(Dominio.ObjetosDeValor.Embarcador.Financeiro.FiltroPesquisaCTeTituloReceber filtrosPesquisa, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametroConsulta)
        {
            var consulta = this.SessionNHiBernate.CreateSQLQuery(QueryConsultarTituloReceberCTe(filtrosPesquisa, false, parametroConsulta));

            consulta.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Dominio.ObjetosDeValor.Embarcador.Financeiro.CTeTituloReceber)));

            return consulta.SetTimeout(600).List<Dominio.ObjetosDeValor.Embarcador.Financeiro.CTeTituloReceber>();
        }

        public int ContarTituloReceberCTe(Dominio.ObjetosDeValor.Embarcador.Financeiro.FiltroPesquisaCTeTituloReceber filtrosPesquisa)
        {
            var consulta = this.SessionNHiBernate.CreateSQLQuery(QueryConsultarTituloReceberCTe(filtrosPesquisa, true));

            return consulta.SetTimeout(600).UniqueResult<int>();
        }


        public IList<Dominio.ObjetosDeValor.Embarcador.Financeiro.DocumentoConciliacao> ConsultarDocumentosConciliacao(Dominio.ObjetosDeValor.Embarcador.Financeiro.FiltroPesquisaDocumentosConciliacao filtrosPesquisa, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametroConsulta)
        {
            var consulta = this.SessionNHiBernate.CreateSQLQuery(QueryConsultarDocumentosConciliacao(filtrosPesquisa, false, parametroConsulta));

            consulta.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Dominio.ObjetosDeValor.Embarcador.Financeiro.DocumentoConciliacao)));

            return consulta.SetTimeout(600).List<Dominio.ObjetosDeValor.Embarcador.Financeiro.DocumentoConciliacao>();
        }

        public int ContarDocumentosConciliacao(Dominio.ObjetosDeValor.Embarcador.Financeiro.FiltroPesquisaDocumentosConciliacao filtrosPesquisa)
        {
            var consulta = this.SessionNHiBernate.CreateSQLQuery(QueryConsultarDocumentosConciliacao(filtrosPesquisa, true));

            return consulta.SetTimeout(600).UniqueResult<int>();
        }

        public List<Dominio.Entidades.ConhecimentoDeTransporteEletronico> ConsultarCTesParaCobrancaMensal(int codigoEmpresaPai, int codigoEmpresa, DateTime dataInicial, DateTime dataFinal, Dominio.Enumeradores.TipoAmbiente tipoAmbiente)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.ConhecimentoDeTransporteEletronico>();

            var result = from obj in query where obj.Empresa.EmpresaPai.Codigo == codigoEmpresaPai && obj.TipoAmbiente == tipoAmbiente && (obj.ModeloDocumentoFiscal.Numero.Equals("57") || obj.ModeloDocumentoFiscal.Numero.Equals("67")) select obj;

            if (codigoEmpresa > 0)
                result = result.Where(obj => obj.Empresa.Codigo == codigoEmpresa);

            if (dataInicial != DateTime.MinValue)
                query = query.Where(o => o.DataEmissao >= dataInicial.Date);

            if (dataFinal != DateTime.MinValue)
                query = query.Where(o => o.DataEmissao < dataFinal.AddDays(1).Date);

            return result.ToList();
        }

        public int ContarCTesParaCobrancaMensal(int codigoEmpresaPai, int codigoEmpresa, DateTime dataInicial, DateTime dataFinal, Dominio.Enumeradores.TipoAmbiente tipoAmbiente, int[] series, int[] seriesDiferente)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.ConhecimentoDeTransporteEletronico>();

            var result = from obj in query where obj.TipoAmbiente == tipoAmbiente && (obj.ModeloDocumentoFiscal.Numero.Equals("57") || obj.ModeloDocumentoFiscal.Numero.Equals("67")) select obj;

            result = result.Where(obj => (obj.Status.Equals("A") || obj.CobrarCancelamento));

            if (codigoEmpresaPai > 0)
                result = result.Where(obj => obj.Empresa.EmpresaPai.Codigo == codigoEmpresaPai);

            if (codigoEmpresa > 0)
                result = result.Where(obj => obj.Empresa.Codigo == codigoEmpresa);

            if (dataInicial != DateTime.MinValue)
                result = result.Where(o => o.DataEmissao >= dataInicial.Date);

            if (dataFinal != DateTime.MinValue)
                result = result.Where(o => o.DataEmissao < dataFinal.AddDays(1).Date);

            if (series != null && series.Count() > 0)
                result = result.Where(o => series.Contains(o.Serie.Numero));

            if (seriesDiferente != null && seriesDiferente.Count() > 0)
                result = result.Where(o => !seriesDiferente.Contains(o.Serie.Numero));

            return result.Count();
        }

        public List<Dominio.Entidades.ConhecimentoDeTransporteEletronico> ConsultarCTesSemCargaComplementares(string StatusCTe)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.ConhecimentoDeTransporteEletronico>();

            var result = from obj in query where obj.CTeSemCarga && obj.TipoCTE == Dominio.Enumeradores.TipoCTE.Complemento select obj;

            if (!string.IsNullOrWhiteSpace(StatusCTe))
            {
                result = result.Where(obj => obj.Status == StatusCTe);
            }

            return result.ToList();
        }

        public List<Dominio.Entidades.ConhecimentoDeTransporteEletronico> ConsultarCTesSemCargaCompativeis(int veiculo, int origem, int destino, bool permiteCTeComplementar)
        {
            List<Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga> situacoesCargaNaoPermitidas = new List<Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga>()
            {
                 Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.Cancelada,
                 Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.Anulada
            };

            List<Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoOcorrencia> situacoesOcorrenciaNaoPermitidas = new List<Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoOcorrencia>()
            {
                 Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoOcorrencia.Cancelada,
                 Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoOcorrencia.Anulada
            };

            IQueryable<Dominio.Entidades.ConhecimentoDeTransporteEletronico> query = this.SessionNHiBernate.Query<Dominio.Entidades.ConhecimentoDeTransporteEletronico>();
            IQueryable<Dominio.Entidades.Embarcador.Cargas.CargaCTe> queryCargaCTe = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCTe>();
            IQueryable<Dominio.Entidades.Embarcador.Cargas.CargaPedidoDocumentoCTe> queryCargaPedidoDocumentoCTe = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedidoDocumentoCTe>();
            IQueryable<Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrenciaDocumento> queryCargaOcorrenciaDocumento = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrenciaDocumento>();

            queryCargaOcorrenciaDocumento = queryCargaOcorrenciaDocumento.Where(o => !situacoesOcorrenciaNaoPermitidas.Contains(o.CargaOcorrencia.SituacaoOcorrencia));
            queryCargaCTe = queryCargaCTe.Where(o => !situacoesCargaNaoPermitidas.Contains(o.Carga.SituacaoCarga));
            queryCargaPedidoDocumentoCTe = queryCargaPedidoDocumentoCTe.Where(o => !situacoesCargaNaoPermitidas.Contains(o.CargaPedido.Carga.SituacaoCarga));

            query = query.Where(o => !queryCargaCTe.Any(cct => cct.CTe == o) &&
                                     !queryCargaPedidoDocumentoCTe.Any(cpd => cpd.CTe == o) &&
                                     !queryCargaOcorrenciaDocumento.Any(cod => cod.CTeImportado == o) &&
                                     o.Status == "A" &&
                                     o.TipoCTE != Dominio.Enumeradores.TipoCTE.Substituto &&
                                     o.TipoCTE != Dominio.Enumeradores.TipoCTE.Anulacao &&
                                     (o.ModeloDocumentoFiscal == null || o.ModeloDocumentoFiscal.TipoDocumentoEmissao != Dominio.Enumeradores.TipoDocumento.NFSe));

            if (!permiteCTeComplementar)
                query = query.Where(obj => obj.TipoCTE != Dominio.Enumeradores.TipoCTE.Complemento);

            if (veiculo > 0)
                query = query.Where(obj => obj.Veiculos.Any(o => o.Veiculo.Codigo == veiculo));

            if (origem > 0)
                query = query.Where(obj => obj.LocalidadeInicioPrestacao.Codigo == origem);

            if (destino > 0)
                query = query.Where(obj => obj.LocalidadeTerminoPrestacao.Codigo == destino);

            return query.WithOptions(o => o.SetTimeout(90)).ToList();
        }

        public dynamic ConsultarCTesSemCarga(Dominio.ObjetosDeValor.Embarcador.CTe.FiltroPesquisaCTeSemCarga filtrosPesquisa, string propOrdenacao, string dirOrdenacao, int inicioRegistros, int maximoRegistros)
        {
            List<Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga> situacoesCargaNaoPermitidas = new List<Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga>()
            {
                 Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.Cancelada,
                 Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.Anulada
            };

            List<Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoOcorrencia> situacoesOcorrenciaNaoPermitidas = new List<Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoOcorrencia>()
            {
                 Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoOcorrencia.Cancelada,
                 Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoOcorrencia.Anulada
            };

            IQueryable<Dominio.Entidades.ConhecimentoDeTransporteEletronico> query = this.SessionNHiBernate.Query<Dominio.Entidades.ConhecimentoDeTransporteEletronico>();
            IQueryable<Dominio.Entidades.Embarcador.Cargas.CargaCTe> queryCargaCTe = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCTe>();
            IQueryable<Dominio.Entidades.Embarcador.Cargas.CargaPedidoDocumentoCTe> queryCargaPedidoDocumentoCTe = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedidoDocumentoCTe>();
            IQueryable<Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrenciaDocumento> queryCargaOcorrenciaDocumento = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrenciaDocumento>();

            queryCargaOcorrenciaDocumento = queryCargaOcorrenciaDocumento.Where(o => !situacoesOcorrenciaNaoPermitidas.Contains(o.CargaOcorrencia.SituacaoOcorrencia));
            queryCargaCTe = queryCargaCTe.Where(o => !situacoesCargaNaoPermitidas.Contains(o.Carga.SituacaoCarga));
            queryCargaPedidoDocumentoCTe = queryCargaPedidoDocumentoCTe.Where(o => !situacoesCargaNaoPermitidas.Contains(o.CargaPedido.Carga.SituacaoCarga));

            if (filtrosPesquisa.DataEmissaoInicial.HasValue)
                query = query.Where(o => o.DataEmissao >= filtrosPesquisa.DataEmissaoInicial.Value.Date);

            if (filtrosPesquisa.DataEmissaoFinal.HasValue)
                query = query.Where(o => o.DataEmissao < filtrosPesquisa.DataEmissaoInicial.Value.AddDays(1).Date);

            query = query.Where(o => o.Status == filtrosPesquisa.StatusCTe &&
                                     !queryCargaPedidoDocumentoCTe.Any(cpd => cpd.CTe == o) &&
                                     !queryCargaOcorrenciaDocumento.Any(cod => cod.CTeImportado == o) &&
                                     !queryCargaCTe.Any(cct => cct.CTe == o && !o.CTeSemCarga));

            if (filtrosPesquisa.NumeroInicial > 0)
                query = query.Where(obj => obj.Numero >= filtrosPesquisa.NumeroInicial);

            if (filtrosPesquisa.NumeroFinal > 0)
                query = query.Where(obj => obj.Numero <= filtrosPesquisa.NumeroFinal);

            if (filtrosPesquisa.CodigoEmpresa > 0)
                query = query.Where(obj => obj.Empresa.Codigo == filtrosPesquisa.CodigoEmpresa);

            if (filtrosPesquisa.CodigoVeiculo > 0)
                query = query.Where(obj => obj.Veiculos.Any(o => o.Veiculo.Codigo == filtrosPesquisa.CodigoVeiculo));

            if (filtrosPesquisa.CodigoGrupoPessoas > 0)
                query = query.Where(o => o.TomadorPagador.GrupoPessoas.Codigo == filtrosPesquisa.CodigoGrupoPessoas);

            if (filtrosPesquisa.CodigoRemetente > 0d)
                query = query.Where(o => o.Remetente.Cliente.CPF_CNPJ == filtrosPesquisa.CodigoRemetente);

            if (filtrosPesquisa.NumeroNF > 0 || !string.IsNullOrWhiteSpace(filtrosPesquisa.ChaveNF))
            {
                var queryDocumentos = this.SessionNHiBernate.Query<Dominio.Entidades.DocumentosCTE>();

                if (filtrosPesquisa.NumeroNF > 0)
                    query = query.Where(o => (from obj in queryDocumentos where obj.Numero.Equals(filtrosPesquisa.NumeroNF.ToString()) select obj.CTE.Codigo).Contains(o.Codigo));

                if (!string.IsNullOrWhiteSpace(filtrosPesquisa.ChaveNF))
                    query = query.Where(o => (from obj in queryDocumentos where obj.ChaveNFE.Equals(filtrosPesquisa.ChaveNF) select obj.CTE.Codigo).Contains(o.Codigo));
            }

            query = query.OrderBy(propOrdenacao + (dirOrdenacao == "asc" ? " ascending" : " descending")).Skip(inicioRegistros).Take(maximoRegistros).WithOptions(o => o.SetTimeout(60));

            return query.Select(obj => new
            {
                obj.Codigo,
                obj.Chave,
                obj.Numero,
                GrupoPessoas = obj.TomadorPagador.GrupoPessoas.Descricao,
                Origem = obj.LocalidadeInicioPrestacao.Descricao + " - " + obj.LocalidadeInicioPrestacao.Estado.Sigla,
                Destino = obj.LocalidadeTerminoPrestacao.Descricao + " - " + obj.LocalidadeTerminoPrestacao.Estado.Sigla,
                ValorFrete = obj.ValorAReceber.ToString("n2"),
                Aliquota = obj.AliquotaICMS.ToString("n2"),
                NotasFiscais = obj.NumeroNotas,
                SituacaoCTe = obj.Status,
                Observacao = obj.ObservacoesGerais,
                NumeroModeloDocumentoFiscal = obj.ModeloDocumentoFiscal.Numero,
                CodigoEmpresa = obj.Empresa.Codigo,
                DataEmissao = obj.DataEmissao
            }).ToList();
        }

        public int ContarConsultaCTesSemCarga(Dominio.ObjetosDeValor.Embarcador.CTe.FiltroPesquisaCTeSemCarga filtrosPesquisa)
        {
            List<Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga> situacoesCargaNaoPermitidas = new List<Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga>()
            {
                 Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.Cancelada,
                 Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.Anulada
            };

            List<Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoOcorrencia> situacoesOcorrenciaNaoPermitidas = new List<Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoOcorrencia>()
            {
                 Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoOcorrencia.Cancelada,
                 Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoOcorrencia.Anulada
            };

            IQueryable<Dominio.Entidades.ConhecimentoDeTransporteEletronico> query = this.SessionNHiBernate.Query<Dominio.Entidades.ConhecimentoDeTransporteEletronico>();
            IQueryable<Dominio.Entidades.Embarcador.Cargas.CargaCTe> queryCargaCTe = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCTe>();
            IQueryable<Dominio.Entidades.Embarcador.Cargas.CargaPedidoDocumentoCTe> queryCargaPedidoDocumentoCTe = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedidoDocumentoCTe>();
            IQueryable<Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrenciaDocumento> queryCargaOcorrenciaDocumento = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrenciaDocumento>();

            queryCargaOcorrenciaDocumento = queryCargaOcorrenciaDocumento.Where(o => !situacoesOcorrenciaNaoPermitidas.Contains(o.CargaOcorrencia.SituacaoOcorrencia));
            queryCargaCTe = queryCargaCTe.Where(o => !situacoesCargaNaoPermitidas.Contains(o.Carga.SituacaoCarga));
            queryCargaPedidoDocumentoCTe = queryCargaPedidoDocumentoCTe.Where(o => !situacoesCargaNaoPermitidas.Contains(o.CargaPedido.Carga.SituacaoCarga));

            if (filtrosPesquisa.DataEmissaoInicial.HasValue)
                query = query.Where(o => o.DataEmissao >= filtrosPesquisa.DataEmissaoInicial.Value.Date);

            if (filtrosPesquisa.DataEmissaoFinal.HasValue)
                query = query.Where(o => o.DataEmissao < filtrosPesquisa.DataEmissaoInicial.Value.AddDays(1).Date);

            query = query.Where(o => o.Status == filtrosPesquisa.StatusCTe &&
                                     !queryCargaPedidoDocumentoCTe.Any(cpd => cpd.CTe == o) &&
                                     !queryCargaOcorrenciaDocumento.Any(cod => cod.CTeImportado == o) &&
                                     !queryCargaCTe.Any(cct => cct.CTe == o && !o.CTeSemCarga));

            if (filtrosPesquisa.NumeroInicial > 0)
                query = query.Where(obj => obj.Numero >= filtrosPesquisa.NumeroInicial);

            if (filtrosPesquisa.NumeroFinal > 0)
                query = query.Where(obj => obj.Numero <= filtrosPesquisa.NumeroFinal);

            if (filtrosPesquisa.CodigoEmpresa > 0)
                query = query.Where(obj => obj.Empresa.Codigo == filtrosPesquisa.CodigoEmpresa);

            if (filtrosPesquisa.CodigoVeiculo > 0)
                query = query.Where(obj => obj.Veiculos.Any(o => o.Veiculo.Codigo == filtrosPesquisa.CodigoVeiculo));

            if (filtrosPesquisa.CodigoGrupoPessoas > 0)
                query = query.Where(o => o.TomadorPagador.GrupoPessoas.Codigo == filtrosPesquisa.CodigoGrupoPessoas);

            if (filtrosPesquisa.CodigoRemetente > 0d)
                query = query.Where(o => o.Remetente.Cliente.CPF_CNPJ == filtrosPesquisa.CodigoRemetente);

            if (filtrosPesquisa.NumeroNF > 0 || !string.IsNullOrWhiteSpace(filtrosPesquisa.ChaveNF))
            {
                var queryDocumentos = this.SessionNHiBernate.Query<Dominio.Entidades.DocumentosCTE>();

                if (filtrosPesquisa.NumeroNF > 0)
                    query = query.Where(o => (from obj in queryDocumentos where obj.Numero.Equals(filtrosPesquisa.NumeroNF.ToString()) select obj.CTE.Codigo).Contains(o.Codigo));

                if (!string.IsNullOrWhiteSpace(filtrosPesquisa.ChaveNF))
                    query = query.Where(o => (from obj in queryDocumentos where obj.ChaveNFE.Equals(filtrosPesquisa.ChaveNF) select obj.CTE.Codigo).Contains(o.Codigo));
            }

            return query.Count();
        }

        public Dominio.Entidades.ConhecimentoDeTransporteEletronico BuscarCodigoEStatus(int codigoCTe, string status, int codigoEmpresa = 0)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.ConhecimentoDeTransporteEletronico>();

            var result = from obj in query where obj.Codigo == codigoCTe && obj.Status == status select obj;

            if (codigoEmpresa > 0)
                result = result.Where(c => c.Empresa.Codigo == codigoEmpresa);

            return result.FirstOrDefault();
        }

        public Dominio.Entidades.ConhecimentoDeTransporteEletronico BuscarPorCodigo(int codigoCTe)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.ConhecimentoDeTransporteEletronico>();

            var result = from obj in query where obj.Codigo == codigoCTe select obj;

            return result.FirstOrDefault();
        }

        public Dominio.Entidades.ConhecimentoDeTransporteEletronico BuscarPorCodigoComFetchEmpresa(int codigoCTe)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.ConhecimentoDeTransporteEletronico>();

            var result = from obj in query where obj.Codigo == codigoCTe select obj;

            return result.Fetch(o => o.Empresa).FirstOrDefault();
        }

        public bool BuscarPorCodigoModeloNFSe(int codigoCTe)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.ConhecimentoDeTransporteEletronico>();

            var result = from obj in query where obj.Codigo == codigoCTe && obj.ModeloDocumentoFiscal.Numero == "39" select obj;

            return result.Any();
        }

        public Dominio.Entidades.ConhecimentoDeTransporteEletronico BuscarNFSePorEmpresaNumeroESerie(int codigoEmpresa, int numero, int serie, string status, DateTime dataEmissao)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.ConhecimentoDeTransporteEletronico>();

            var result = from obj in query where obj.ModeloDocumentoFiscal.Numero == "39" && obj.Empresa.Codigo == codigoEmpresa && obj.Numero == numero && obj.Serie.Numero == serie && obj.Status == status select obj;

            if (dataEmissao > DateTime.MinValue)
                result = result.Where(o => o.DataEmissao.Value.Year >= dataEmissao.Year);

            return result.FirstOrDefault();
        }

        public bool BuscarPorCodigoModeloCTe(int codigoCTe)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.ConhecimentoDeTransporteEletronico>();

            var result = from obj in query where obj.Codigo == codigoCTe && (obj.ModeloDocumentoFiscal.Numero.Equals("57") || obj.ModeloDocumentoFiscal.Numero.Equals("67")) select obj;

            return result.Any();
        }

        public Dominio.Entidades.ConhecimentoDeTransporteEletronico BuscarPorCodigo(int codigoEmpresa, int codigoCTe)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.ConhecimentoDeTransporteEletronico>();
            var result = from obj in query where obj.Codigo == codigoCTe select obj;

            if (codigoEmpresa > 0)
                result = result.Where(o => o.Empresa.Codigo == codigoEmpresa);

            return result.FirstOrDefault();
        }

        public Dominio.Entidades.ConhecimentoDeTransporteEletronico BuscarPorCodigoComFetch(int codigoCTe)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.ConhecimentoDeTransporteEletronico>();
            var result = from obj in query where obj.Codigo == codigoCTe select obj;

            return result.Fetch(o => o.Remetente)
                         .Fetch(o => o.Expedidor)
                         .Fetch(o => o.Recebedor)
                         .Fetch(o => o.Destinatario)
                         .Fetch(o => o.OutrosTomador)
                         .Fetch(o => o.CFOP)
                         .Fetch(o => o.NaturezaDaOperacao)
                         .Fetch(o => o.ModeloDocumentoFiscal)
                         .Fetch(o => o.Empresa)
                         .Fetch(o => o.ModalTransporte)
                         .Fetch(o => o.ClienteEntrega)
                         .Fetch(o => o.ClienteRetira)
                         .Fetch(o => o.LocalidadeEmissao)
                         .Fetch(o => o.LocalidadeInicioPrestacao)
                         .Fetch(o => o.LocalidadeTerminoPrestacao)
                         .FirstOrDefault();
        }

        public Dominio.Entidades.ConhecimentoDeTransporteEletronico BuscarPorCodigo(int codigoEmpresa, int codigoCTe, string status)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.ConhecimentoDeTransporteEletronico>();
            var result = from obj in query where obj.Codigo == codigoCTe && obj.Status.Equals(status) select obj;
            if (codigoEmpresa > 0)
                result = result.Where(o => o.Empresa.Codigo == codigoEmpresa);
            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.ConhecimentoDeTransporteEletronico> BuscarPorCodigo(int codigoEmpresa, int[] codigosCTe)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.ConhecimentoDeTransporteEletronico>();
            var result = from obj in query where codigosCTe.Contains(obj.Codigo) select obj;

            if (codigoEmpresa > 0)
                result = result.Where(o => o.Empresa.Codigo == codigoEmpresa);

            return result.ToList();
        }

        public List<int> BuscarListaCodigosCTes(Dominio.ObjetosDeValor.Embarcador.CTe.FiltroPesquisaConsultaCTe filtrosPesquisa)
        {
            var query = ObterConsultaFormularioConsultaCTe(filtrosPesquisa);

            return query.Select(o => o.Codigo).ToList();
        }

        public List<int> BuscarListaCodigosNFSe(Dominio.ObjetosDeValor.Embarcador.CTe.FiltroPesquisaConsultaCTe filtrosPesquisa)
        {
            filtrosPesquisa.TipoDocumentoEmissao = Dominio.Enumeradores.TipoDocumento.NFSe;

            var query = ObterConsultaFormularioConsultaCTe(filtrosPesquisa);

            return query.Select(o => o.Codigo).ToList();
        }

        public int ContarConsulta(Dominio.ObjetosDeValor.Embarcador.CTe.FiltroPesquisaConsultaCTe filtrosPesquisa)
        {
            var query = ObterConsultaFormularioConsultaCTe(filtrosPesquisa);

            return query.Count();
        }

        public List<Dominio.Entidades.ConhecimentoDeTransporteEletronico> Consultar(Dominio.ObjetosDeValor.Embarcador.CTe.FiltroPesquisaConsultaCTe filtrosPesquisa, string propOrdenacao, string dirOrdena, int inicio, int limite)
        {
            var query = ObterConsultaFormularioConsultaCTe(filtrosPesquisa);

            if (!string.IsNullOrWhiteSpace(propOrdenacao))
                query = query.OrderBy(propOrdenacao + " " + dirOrdena);

            if (inicio > 0 || limite > 0)
                query = query.Skip(inicio).Take(limite);

            return query.Fetch(o => o.Remetente)
                        .Fetch(o => o.Destinatario)
                        .Fetch(o => o.ModeloDocumentoFiscal)
                        .Fetch(o => o.LocalidadeInicioPrestacao)
                        .Fetch(o => o.LocalidadeTerminoPrestacao)
                        .Fetch(o => o.Serie)
                        .Fetch(o => o.Empresa)
                        .Fetch(o => o.Viagem)
                        .Fetch(o => o.PortoOrigem)
                        .Fetch(o => o.PortoDestino)
                        .ToList();
        }

        public List<Dominio.Entidades.ConhecimentoDeTransporteEletronico> ConsultarCTesSemCarga(string propOrdenacao, string dirOrdena, int inicio, int limite)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.ConhecimentoDeTransporteEletronico>();
            var queryCancelamentoCTe = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.CTe.CancelamentoCTe>();

            var listCTesEmCancelamento = queryCancelamentoCTe.Select(o => o.CTe.Codigo).ToList();

            query = query.Where(c => c.Status == "A" && c.CTeImportadoEmbarcador == null && !c.CargaCTes.Any());
            query = query.Where(c => !listCTesEmCancelamento.Contains(c.Codigo));

            if (!string.IsNullOrWhiteSpace(propOrdenacao))
                query = query.OrderBy(propOrdenacao + " " + dirOrdena);

            if (inicio > 0 || limite > 0)
                query = query.Skip(inicio).Take(limite);

            var result = query.Fetch(o => o.Remetente)
                        .Fetch(o => o.Destinatario)
                        .Fetch(o => o.ModeloDocumentoFiscal)
                        .Fetch(o => o.LocalidadeInicioPrestacao)
                        .Fetch(o => o.LocalidadeTerminoPrestacao)
                        .Fetch(o => o.Serie)
                        .Fetch(o => o.Empresa)
                        .Fetch(o => o.Viagem)
                        .Fetch(o => o.PortoOrigem)
                        .Fetch(o => o.PortoDestino)
                        .ToList();

            return result;
        }

        public int ContarConsulta(int codigoEmpresa, DateTime dataEmissaoInicial, DateTime dataEmissaoFinal, int numeroInicial, int numeroFinal, Dominio.Enumeradores.TipoCTE tipo, string statusCTe, string cpfCnpjRemetente, string cpfCnpjDestinatario, Dominio.Enumeradores.TipoAmbiente tipoAmbiente)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.ConhecimentoDeTransporteEletronico>();

            if (codigoEmpresa > 0)
                query = query.Where(o => o.Empresa.Codigo == codigoEmpresa);

            if (dataEmissaoInicial != DateTime.MinValue)
                query = query.Where(o => o.DataEmissao >= dataEmissaoInicial.Date);

            if (dataEmissaoFinal != DateTime.MinValue)
                query = query.Where(o => o.DataEmissao < dataEmissaoFinal.AddDays(1).Date);

            if (numeroInicial > 0)
                query = query.Where(o => o.Numero >= numeroInicial);

            if (numeroFinal > 0)
                query = query.Where(o => o.Numero <= numeroFinal);

            if (!string.IsNullOrWhiteSpace(cpfCnpjRemetente))
                query = query.Where(o => o.Remetente.CPF_CNPJ == cpfCnpjRemetente);

            if (!string.IsNullOrWhiteSpace(cpfCnpjDestinatario))
                query = query.Where(o => o.Destinatario.CPF_CNPJ == cpfCnpjDestinatario);

            if (!string.IsNullOrWhiteSpace(statusCTe))
                query = query.Where(o => o.Status == statusCTe);

            if (tipo >= 0)
                query = query.Where(o => o.TipoCTE == tipo);

            query = query.Where(o => o.TipoAmbiente == tipoAmbiente);

            return query.Count();
        }

        public List<Dominio.Entidades.ConhecimentoDeTransporteEletronico> Consultar(int codigoEmpresa, DateTime dataEmissaoInicial, DateTime dataEmissaoFinal, int numeroInicial, int numeroFinal, Dominio.Enumeradores.TipoCTE tipo, string statusCTe, string cpfCnpjRemetente, string cpfCnpjDestinatario, Dominio.Enumeradores.TipoAmbiente tipoAmbiente, string propOrdenacao, string dirOrdena, int inicio, int limite)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.ConhecimentoDeTransporteEletronico>();

            if (codigoEmpresa > 0)
                query = query.Where(o => o.Empresa.Codigo == codigoEmpresa);

            if (dataEmissaoInicial != DateTime.MinValue)
                query = query.Where(o => o.DataEmissao >= dataEmissaoInicial.Date);

            if (dataEmissaoFinal != DateTime.MinValue)
                query = query.Where(o => o.DataEmissao < dataEmissaoFinal.AddDays(1).Date);

            if (numeroInicial > 0)
                query = query.Where(o => o.Numero >= numeroInicial);

            if (numeroFinal > 0)
                query = query.Where(o => o.Numero <= numeroFinal);

            if (!string.IsNullOrWhiteSpace(cpfCnpjRemetente))
                query = query.Where(o => o.Remetente.CPF_CNPJ == cpfCnpjRemetente);

            if (!string.IsNullOrWhiteSpace(cpfCnpjDestinatario))
                query = query.Where(o => o.Destinatario.CPF_CNPJ == cpfCnpjDestinatario);

            if (!string.IsNullOrWhiteSpace(statusCTe))
                query = query.Where(o => o.Status == statusCTe);

            if (tipo >= 0)
                query = query.Where(o => o.TipoCTE == tipo);

            query = query.Where(o => o.TipoAmbiente == tipoAmbiente);

            return query.Fetch(o => o.Remetente)
                        .Fetch(o => o.Destinatario)
                        .Fetch(o => o.LocalidadeInicioPrestacao)
                        .Fetch(o => o.LocalidadeTerminoPrestacao)
                        .Fetch(o => o.Serie)
                        .OrderBy(propOrdenacao + " " + dirOrdena)
                        .Skip(inicio)
                        .Take(limite)
                        .ToList();
        }

        public List<Dominio.Entidades.ConhecimentoDeTransporteEletronico> BuscarPorChaves(List<string> chaves)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.ConhecimentoDeTransporteEletronico>();
            var result = from obj in query where chaves.Contains(obj.Chave) select obj;

            return result.ToList();
        }

        public List<Dominio.Entidades.ConhecimentoDeTransporteEletronico> BuscarPorCargaPedidoDocumentoCTe(int codigoCargaPedido)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedidoDocumentoCTe>();
            var result = from obj in query where obj.CargaPedido.Codigo == codigoCargaPedido select obj;

            return result.Select(c => c.CTe).ToList();
        }
        public List<Dominio.Entidades.ConhecimentoDeTransporteEletronico> BuscarCTeComplementaresPorChave(string chave)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.ConhecimentoDeTransporteEletronico>();
            var result = from obj in query where obj.ChaveCTESubComp.Equals(chave) select obj;

            return result.ToList();
        }

        public Dominio.Entidades.ConhecimentoDeTransporteEletronico BuscarCTeComplementarPorChave(string chave)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.ConhecimentoDeTransporteEletronico>();
            var result = from obj in query where obj.ChaveCTESubComp.Equals(chave) && obj.Status == "A" select obj;

            return result.FirstOrDefault();
        }

        public Dominio.Entidades.ConhecimentoDeTransporteEletronico BuscarPorChave(int codigoEmpresa, string chave)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.ConhecimentoDeTransporteEletronico>();
            var result = from obj in query where obj.Chave.Equals(chave) select obj;

            if (codigoEmpresa > 0)
                result = result.Where(o => o.Empresa.Codigo == codigoEmpresa);

            return result.FirstOrDefault();
        }

        public Task<Dominio.Entidades.ConhecimentoDeTransporteEletronico> BuscarPorChaveAsync(int codigoEmpresa, string chave)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.ConhecimentoDeTransporteEletronico>();

            var result = from obj in query where obj.Chave.Equals(chave) select obj;

            if (codigoEmpresa > 0)
                result = result.Where(o => o.Empresa.Codigo == codigoEmpresa);

            return result.FirstOrDefaultAsync(CancellationToken);
        }

        public Task<Dominio.Entidades.ConhecimentoDeTransporteEletronico> BuscarPorChaveAsync(string chave)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.ConhecimentoDeTransporteEletronico>();

            var result = from obj in query where obj.Chave.Equals(chave) select obj;

            return result.FirstOrDefaultAsync(CancellationToken);
        }

        public List<Dominio.Entidades.ConhecimentoDeTransporteEletronico> BuscarPorChave(List<string> chaves)
        {
            IQueryable<Dominio.Entidades.ConhecimentoDeTransporteEletronico> query = this.SessionNHiBernate.Query<Dominio.Entidades.ConhecimentoDeTransporteEletronico>();

            int quantidadeRegistrosConsultarPorVez = 100;
            int quantidadeConsultas = chaves.Count / quantidadeRegistrosConsultarPorVez;

            List<Dominio.Entidades.ConhecimentoDeTransporteEletronico> ctesRetornar = new List<Dominio.Entidades.ConhecimentoDeTransporteEletronico>();

            for (int i = 0; i <= quantidadeConsultas; i++)
                ctesRetornar.AddRange(query.Where(o => chaves.Skip(i * quantidadeRegistrosConsultarPorVez).Take(quantidadeRegistrosConsultarPorVez).Contains(o.Chave)).ToList());

            return ctesRetornar;
        }

        public Dominio.Entidades.ConhecimentoDeTransporteEletronico BuscarPorNumeroESerie(int codigoEmpresa, int numero, int serie, string modelo, Dominio.Enumeradores.TipoAmbiente ambiente)
        {
            IQueryable<Dominio.Entidades.ConhecimentoDeTransporteEletronico> query = this.SessionNHiBernate.Query<Dominio.Entidades.ConhecimentoDeTransporteEletronico>();

            query = query.Where(obj => obj.Numero == numero &&
                                       obj.Serie.Numero == serie &&
                                       obj.Empresa.Codigo == codigoEmpresa &&
                                       obj.ModeloDocumentoFiscal.Numero.Equals(modelo) &&
                                       obj.TipoAmbiente == ambiente);

            return query.FirstOrDefault();
        }

        public Dominio.Entidades.ConhecimentoDeTransporteEletronico BuscarPorNumeroERemetente(int numero, string cpfCnpjRemetente)
        {
            var consultaConhecimentoDeTransporteEletronico = this.SessionNHiBernate.Query<Dominio.Entidades.ConhecimentoDeTransporteEletronico>()
                .Where(o => o.Numero == numero && o.Remetente.CPF_CNPJ == cpfCnpjRemetente && o.Empresa.PermiteEmitirSubcontratacao);

            return consultaConhecimentoDeTransporteEletronico.FirstOrDefault();
        }

        public Dominio.Entidades.ConhecimentoDeTransporteEletronico Buscar(int codigoEmpresa, int numero, int serie, string modelo, Dominio.Enumeradores.TipoAmbiente tipoAmbiente)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.ConhecimentoDeTransporteEletronico>();
            var result = from obj in query where obj.Numero == numero && obj.Serie.Numero == serie && obj.ModeloDocumentoFiscal.Numero == modelo && obj.Empresa.Codigo == codigoEmpresa && obj.TipoAmbiente == tipoAmbiente && (obj.ModeloDocumentoFiscal.Numero.Equals("57") || obj.ModeloDocumentoFiscal.Numero.Equals("67")) select obj;
            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.Empresa> BuscarEmpresasEmissao(DateTime dataInicial, DateTime dataFinal)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.ConhecimentoDeTransporteEletronico>();
            var result = from obj in query where obj.TipoAmbiente == Dominio.Enumeradores.TipoAmbiente.Producao && obj.TipoControle == 1 select obj;

            if (dataInicial != DateTime.MinValue)
                result = result.Where(o => o.DataEmissao >= dataInicial.Date);

            if (dataFinal != DateTime.MinValue)
                result = result.Where(o => o.DataEmissao < dataFinal.AddDays(1).Date);

            return result.Select(o => o.Empresa).Distinct().ToList();
        }

        public List<Dominio.Entidades.EmpresaSerie> BuscarSeriesEmissao(DateTime dataInicial, DateTime dataFinal)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.ConhecimentoDeTransporteEletronico>();
            var result = from obj in query where obj.TipoAmbiente == Dominio.Enumeradores.TipoAmbiente.Producao && obj.TipoControle == 1 select obj;

            if (dataInicial != DateTime.MinValue)
                result = result.Where(o => o.DataEmissao >= dataInicial.Date);

            if (dataFinal != DateTime.MinValue)
                result = result.Where(o => o.DataEmissao < dataFinal.AddDays(1).Date);

            return result.Select(o => o.Serie).Distinct().ToList();
        }

        public Dominio.Entidades.Veiculo BuscarPrimeiroVeiculo(int codigoCTe)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.VeiculoCTE>();
            query = query.Where(c => c.CTE.Codigo == codigoCTe);
            return query.Select(c => c.Veiculo)?.FirstOrDefault();
        }

        public int BuscarPrimeiraNumeracao(DateTime dataInicial, DateTime dataFinal, int codigoSerie)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.ConhecimentoDeTransporteEletronico>();
            var result = from obj in query where obj.TipoAmbiente == Dominio.Enumeradores.TipoAmbiente.Producao && obj.TipoControle == 1 && obj.Serie.Codigo == codigoSerie select obj;

            if (dataInicial != DateTime.MinValue)
                result = result.Where(o => o.DataEmissao >= dataInicial.Date);

            if (dataFinal != DateTime.MinValue)
                result = result.Where(o => o.DataEmissao < dataFinal.AddDays(1).Date);

            return result.OrderBy(obj => obj.Numero).Select(o => o.Numero).FirstOrDefault();
        }

        public int BuscarUltimaNumeracao(DateTime dataInicial, DateTime dataFinal, int codigoSerie)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.ConhecimentoDeTransporteEletronico>();
            var result = from obj in query where obj.TipoAmbiente == Dominio.Enumeradores.TipoAmbiente.Producao && obj.TipoControle == 1 && obj.Serie.Codigo == codigoSerie select obj;

            if (dataInicial != DateTime.MinValue)
                result = result.Where(o => o.DataEmissao >= dataInicial.Date);

            if (dataFinal != DateTime.MinValue)
                result = result.Where(o => o.DataEmissao < dataFinal.AddDays(1).Date);

            return result.OrderByDescending(obj => obj.Numero).Select(o => o.Numero).FirstOrDefault();
        }

        public int BuscarSequenciaCTeBooking(int codigoCTe, string numeroBooking, Dominio.Enumeradores.TipoServico tipoServico, bool ignorarCTesImportados)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.ConhecimentoDeTransporteEletronico>();
            query = query.Where(obj => obj.SequenciaBooking >= 0 && obj.NumeroBooking == numeroBooking);

            if (!ignorarCTesImportados)
                query = query.Where(o => (o.CTeImportadoEmbarcador == false || o.CTeImportadoEmbarcador == null));

            if (codigoCTe > 0)
                query = query.Where(o => o.Codigo < codigoCTe);

            if (tipoServico == Dominio.Enumeradores.TipoServico.ServVinculadoMultimodal)
                query = query.Where(o => o.TipoServico == tipoServico);
            else if (tipoServico == Dominio.Enumeradores.TipoServico.ServVinculadoMultimodal)
                query = query.Where(o => o.TipoServico != tipoServico);

            return (query.Max(o => (int?)o.SequenciaBooking) ?? 0) + 1;
        }

        public bool ContemSequenciaControleDuplicado(int codigoCTe, int sequenciaBooking, string numeroBooking, Dominio.Enumeradores.TipoServico tipoServico, bool ignorarCTesImportados)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.ConhecimentoDeTransporteEletronico>();
            var result = from obj in query where obj.SequenciaBooking == sequenciaBooking && obj.NumeroBooking == numeroBooking select obj;

            if (!ignorarCTesImportados)
                result = result.Where(o => (o.CTeImportadoEmbarcador == false || o.CTeImportadoEmbarcador == null));

            if (codigoCTe > 0)
                result = result.Where(o => o.Codigo != codigoCTe);

            if (tipoServico == Dominio.Enumeradores.TipoServico.ServVinculadoMultimodal)
                result = result.Where(o => o.TipoServico == tipoServico);
            else if (tipoServico == Dominio.Enumeradores.TipoServico.ServVinculadoMultimodal)
                result = result.Where(o => o.TipoServico != tipoServico);

            return result.Any();
        }

        public bool ContemNumeroControleDuplicado(string numeroControle, int codigoEmpresa, int codigoCTe)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.ConhecimentoDeTransporteEletronico>();
            var result = from obj in query where obj.NumeroControle == numeroControle && obj.Empresa.Codigo == codigoEmpresa && obj.Codigo != codigoCTe select obj;

            return result.Any();
        }

        public List<string> BuscarRemetentes(int codigoEmpresa, DateTime dataInicial, DateTime dataFinal, string[] status, Dominio.Enumeradores.TipoAmbiente tipoAmbiente, List<int> codigosCTes = null, int codigoVeiculo = 0)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.ConhecimentoDeTransporteEletronico>();

            var result = from obj in query
                         where
                             obj.Empresa.Codigo == codigoEmpresa &&
                             obj.DataEmissao >= dataInicial.Date &&
                             obj.DataEmissao < dataFinal.AddDays(1).Date &&
                             status.Contains(obj.Status) &&
                             obj.TipoAmbiente == tipoAmbiente &&
                             (obj.ModeloDocumentoFiscal.Numero.Equals("57") || obj.ModeloDocumentoFiscal.Numero.Equals("67") || obj.ModeloDocumentoFiscal.Numero.Equals("39"))
                         select obj;

            if (codigosCTes != null && codigosCTes.Count() > 0)
                result = result.Where(o => codigosCTes.Contains(o.Codigo));

            if (codigoVeiculo > 0)
            {
                var queryVeiculos = this.SessionNHiBernate.Query<Dominio.Entidades.VeiculoCTE>();

                result = result.Where(o => (from obj in queryVeiculos where obj.CTE.Codigo == o.Codigo && obj.Veiculo.Codigo == codigoVeiculo select obj.CTE.Codigo).Contains(o.Codigo));
            }

            return result.Select(o => o.Remetente.CPF_CNPJ).Distinct().ToList();
        }

        public List<string> BuscarTomadores(int codigoEmpresa, DateTime dataInicial, DateTime dataFinal, string[] status, Dominio.Enumeradores.TipoAmbiente tipoAmbiente, List<int> codigosCTes = null, int codigoVeiculo = 0, string cpfCnpjRemetente = "", string cpfCnpjDestinatario = "")
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.ConhecimentoDeTransporteEletronico>();

            var result = from obj in query
                         where
                             obj.Empresa.Codigo == codigoEmpresa &&
                             obj.DataEmissao >= dataInicial.Date &&
                             obj.DataEmissao < dataFinal.AddDays(1).Date &&
                             status.Contains(obj.Status) &&
                             obj.TipoAmbiente == tipoAmbiente &&
                             (obj.ModeloDocumentoFiscal.Numero.Equals("57") || obj.ModeloDocumentoFiscal.Numero.Equals("67") || obj.ModeloDocumentoFiscal.Numero.Equals("39"))
                         select obj;

            if (codigosCTes != null && codigosCTes.Count() > 0)
                result = result.Where(o => codigosCTes.Contains(o.Codigo));

            if (!string.IsNullOrWhiteSpace(cpfCnpjRemetente))
                result = result.Where(o => o.Remetente.CPF_CNPJ.Equals(cpfCnpjRemetente));

            if (!string.IsNullOrWhiteSpace(cpfCnpjDestinatario))
                result = result.Where(o => o.Destinatario.CPF_CNPJ.Equals(cpfCnpjDestinatario));

            if (codigoVeiculo > 0)
            {
                var queryVeiculos = this.SessionNHiBernate.Query<Dominio.Entidades.VeiculoCTE>();

                result = result.Where(o => (from obj in queryVeiculos where obj.CTE.Codigo == o.Codigo && obj.Veiculo.Codigo == codigoVeiculo select obj.CTE.Codigo).Contains(o.Codigo));
            }

            return result.Select(o => o.TomadorPagador.CPF_CNPJ).Distinct().ToList();
        }

        public int BuscarUltimoNumero(int codigoEmpresa, int serie, Dominio.Enumeradores.TipoAmbiente ambiente, int modeloDocumento = 0)
        {
            var criteria = this.SessionNHiBernate.CreateCriteria<Dominio.Entidades.ConhecimentoDeTransporteEletronico>();
            criteria.Add(NHibernate.Criterion.Restrictions.Eq("Empresa.Codigo", codigoEmpresa));
            criteria.Add(NHibernate.Criterion.Restrictions.Eq("Serie.Codigo", serie));
            criteria.Add(NHibernate.Criterion.Restrictions.Eq("TipoAmbiente", ambiente));

            if (modeloDocumento > 0)
                criteria.Add(NHibernate.Criterion.Restrictions.Eq("ModeloDocumentoFiscal.Codigo", modeloDocumento));


            criteria.SetProjection(NHibernate.Criterion.Projections.Max("Numero"));
            return criteria.UniqueResult<int>();
        }


        public (int codigoCte, int numeroCTe) BuscarNumeroReutilizavel(int codigoEmpresa, int serie, Dominio.Enumeradores.TipoAmbiente ambiente, int modeloDocumento = 0)
        {
            var criteria = this.SessionNHiBernate.CreateCriteria<Dominio.Entidades.ConhecimentoDeTransporteEletronico>();
            criteria.Add(NHibernate.Criterion.Restrictions.Eq("Empresa.Codigo", codigoEmpresa));
            criteria.Add(NHibernate.Criterion.Restrictions.Eq("Serie.Codigo", serie));
            criteria.Add(NHibernate.Criterion.Restrictions.Eq("TipoAmbiente", ambiente));
            criteria.Add(NHibernate.Criterion.Restrictions.Eq("ReutilizaNumeracao", Dominio.Enumeradores.ReutilizaNumeracao.Sim));

            if (modeloDocumento > 0)
                criteria.Add(NHibernate.Criterion.Restrictions.Eq("ModeloDocumentoFiscal.Codigo", modeloDocumento));

            var projectionList = Projections.ProjectionList();
            projectionList.Add(Projections.Property("Codigo"));
            projectionList.Add(Projections.Property("Numero"));

            criteria.SetProjection(projectionList);
            criteria.AddOrder(Order.Asc("Numero"));
            criteria.SetMaxResults(1);

            var result = criteria.UniqueResult<object[]>();

            if (result != null)
                return ((int)result[0], (int)result[1]);

            return (0, 0);
        }

        public int BuscarUltimoNumeroCTe(int codigoEmpresa, int serie, Dominio.Enumeradores.TipoAmbiente ambiente, int modeloDocumento)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.ConhecimentoDeTransporteEletronico>();

            var result = from obj in query
                         where
                             obj.Empresa.Codigo == codigoEmpresa &&
                             obj.TipoAmbiente == ambiente &&
                             obj.Serie.Codigo == serie &&
                             obj.ModeloDocumentoFiscal.Codigo == modeloDocumento
                         select obj;

            return result.Count() > 0 ? result.Max(o => o.Numero) : 0;
        }

        public void AtulizarSituacaoReutilizacao(Dominio.Enumeradores.ReutilizaNumeracao situacaoReutilizacao, int codigoCTe)
        {
            string hql = "UPDATE T_CTE SET CON_REUTILIZA_NUMERACAO = :situacaoReutilizacao, CON_TIPO_CONTROLE = CON_CODIGO * -1 WHERE CON_CODIGO = :codigoConhecimento ";

            var query = this.SessionNHiBernate.CreateSQLQuery(hql);

            query.SetInt32("codigoConhecimento", codigoCTe);
            query.SetInt32("situacaoReutilizacao", (int)situacaoReutilizacao);

            query.SetTimeout(300).ExecuteUpdate();
        }

        public int BuscarUltimoSequencialPorOperacao(int codigoTipoOperacao)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.ConhecimentoDeTransporteEletronico>();

            var result = from obj in query
                         where
                             obj.TipoOperacao.Codigo == codigoTipoOperacao
                         select obj;

            return result.Count() > 0 ? result.Max(o => o.SequencialOperacao) : 0;
        }

        public long BuscarUltimoTipoControle()
        {
            var criteria = this.SessionNHiBernate.CreateCriteria<Dominio.Entidades.ConhecimentoDeTransporteEletronico>();
            criteria.SetProjection(NHibernate.Criterion.Projections.Max("TipoControle"));
            return criteria.UniqueResult<long>();
        }

        public long BuscarUltimoTipoControlePorModelo(int modeloDocumento)
        {
            var criteria = this.SessionNHiBernate.CreateCriteria<Dominio.Entidades.ConhecimentoDeTransporteEletronico>();

            if (modeloDocumento > 0)
                criteria.Add(NHibernate.Criterion.Restrictions.Eq("ModeloDocumentoFiscal.Codigo", modeloDocumento));

            criteria.SetProjection(NHibernate.Criterion.Projections.Max("TipoControle"));

            return criteria.UniqueResult<long>();
        }

        public IList<Dominio.Entidades.ConhecimentoDeTransporteEletronico> BuscarPorEmpresa(int codigoEmpresa, int inicioRegistros, int maximoRegistros, string propriedadeOrdenar, Dominio.Enumeradores.TipoAmbiente tipoAmbiente, bool decrescente = false)
        {
            var criteria = this.SessionNHiBernate.CreateCriteria<Dominio.Entidades.ConhecimentoDeTransporteEletronico>();
            if (!string.IsNullOrWhiteSpace(propriedadeOrdenar))
            {
                if (decrescente)
                    criteria.AddOrder(NHibernate.Criterion.Order.Desc(propriedadeOrdenar));
                else
                    criteria.AddOrder(NHibernate.Criterion.Order.Asc(propriedadeOrdenar));
            }
            criteria.Add(NHibernate.Criterion.Expression.Eq("TipoAmbiente", tipoAmbiente));
            criteria.Add(NHibernate.Criterion.Expression.Eq("Empresa.Codigo", codigoEmpresa));
            criteria.SetMaxResults(maximoRegistros);
            criteria.SetFirstResult(inicioRegistros);
            return criteria.List<Dominio.Entidades.ConhecimentoDeTransporteEletronico>();
        }

        public int ContarPorEmpresa(int codigoEmpresa, Dominio.Enumeradores.TipoAmbiente tipoAmbiente)
        {
            var criteria = this.SessionNHiBernate.CreateCriteria<Dominio.Entidades.ConhecimentoDeTransporteEletronico>();
            criteria.Add(NHibernate.Criterion.Expression.Eq("Empresa.Codigo", codigoEmpresa));
            criteria.Add(NHibernate.Criterion.Expression.Eq("TipoAmbiente", tipoAmbiente));
            criteria.SetProjection(NHibernate.Criterion.Projections.RowCount());
            return criteria.UniqueResult<int>();
        }

        public List<Dominio.ObjetosDeValor.ConsultaCTe> Consultar(int codigoEmpresa, Dominio.Enumeradores.TipoAmbiente tipoAmbiente, string[] status, int numeroInicial, int numeroFinal, string numeroCIOT, bool semCIOT, string placa, DateTime dataInicial, DateTime dataFinal, int inicioRegistros, int maximoRegistros)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.ConhecimentoDeTransporteEletronico>();

            var result = from obj in query
                         where
                             obj.TipoAmbiente == tipoAmbiente &&
                             obj.Empresa.Codigo == codigoEmpresa &&
                             status.Contains(obj.Status) &&
                             (obj.ModeloDocumentoFiscal.Numero.Equals("57") || obj.ModeloDocumentoFiscal.Numero.Equals("67"))
                         select obj;

            if (numeroInicial > 0)
                result = result.Where(o => o.Numero >= numeroInicial);

            if (numeroFinal > 0)
                result = result.Where(o => o.Numero <= numeroFinal);

            if (dataInicial != DateTime.MinValue)
                result = result.Where(o => o.DataEmissao >= dataInicial.Date);

            if (dataFinal != DateTime.MinValue)
                result = result.Where(o => o.DataEmissao < dataFinal.AddDays(1).Date);

            if (!string.IsNullOrWhiteSpace(placa))
            {
                var queryVeiculos = this.SessionNHiBernate.Query<Dominio.Entidades.VeiculoCTE>();
                result = result.Where(o => (from obj in queryVeiculos where obj.CTE.Codigo == o.Codigo && obj.Veiculo.Placa.Equals(placa) select obj.CTE.Codigo).Contains(o.Codigo));
            }

            if (semCIOT)
                result = result.Where(o => o.CIOT == null || o.CIOT.Equals(string.Empty));
            else if (!string.IsNullOrWhiteSpace(numeroCIOT))
                result = result.Where(o => o.CIOT.Equals(numeroCIOT));

            return result.Select(o => new Dominio.ObjetosDeValor.ConsultaCTe()
            {
                Codigo = o.Codigo,
                DataEmissao = o.DataEmissao,
                Destinatario = o.Destinatario,
                MensagemStatus = o.MensagemStatus,
                MensagemRetornoSefaz = o.MensagemRetornoSefaz,
                Numero = o.Numero,
                Remetente = o.Remetente,
                Serie = o.Serie.Numero,
                Status = o.Status,
                TipoCTe = o.TipoCTE,
                TipoServico = o.TipoServico,
                Valor = o.ValorFrete
            })
            .OrderByDescending(o => o.Numero)
            .Skip(inicioRegistros)
            .Take(maximoRegistros)
            .Timeout(120)
            .ToList();
        }

        public int ContarConsulta(int codigoEmpresa, Dominio.Enumeradores.TipoAmbiente tipoAmbiente, string[] status, int numeroInicial, int numeroFinal, string numeroCIOT, bool semCIOT, string placa, DateTime dataInicial, DateTime dataFinal)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.ConhecimentoDeTransporteEletronico>();

            var result = from obj in query
                         where
                             obj.TipoAmbiente == tipoAmbiente &&
                             obj.Empresa.Codigo == codigoEmpresa &&
                             status.Contains(obj.Status) &&
                             (obj.ModeloDocumentoFiscal.Numero.Equals("57") || obj.ModeloDocumentoFiscal.Numero.Equals("67"))
                         select obj;

            if (numeroInicial > 0)
                result = result.Where(o => o.Numero >= numeroInicial);

            if (numeroFinal > 0)
                result = result.Where(o => o.Numero <= numeroFinal);

            if (dataInicial != DateTime.MinValue)
                result = result.Where(o => o.DataEmissao >= dataInicial.Date);

            if (dataFinal != DateTime.MinValue)
                result = result.Where(o => o.DataEmissao < dataFinal.AddDays(1).Date);

            if (!string.IsNullOrWhiteSpace(placa))
            {
                var queryVeiculos = this.SessionNHiBernate.Query<Dominio.Entidades.VeiculoCTE>();
                result = result.Where(o => (from obj in queryVeiculos where obj.CTE.Codigo == o.Codigo && obj.Veiculo.Placa.Equals(placa) select obj.CTE.Codigo).Contains(o.Codigo));
            }

            if (semCIOT)
                result = result.Where(o => o.CIOT == null || o.CIOT.Equals(string.Empty));
            else if (!string.IsNullOrWhiteSpace(numeroCIOT))
                result = result.Where(o => o.CIOT.Equals(numeroCIOT));

            return result.Count();
        }

        public List<Dominio.ObjetosDeValor.ConsultaCTe> Consultar(int codigoEmpresa, DateTime dataEmissaoInicial, DateTime dataEmissaoFinal, int numeroInicial, int numeroFinal, string placa, string motorista, string cpfCnpjRemetente, string cpfCnpjDestinatario, string status, Dominio.Enumeradores.TipoCTE tipoCTe, Dominio.Enumeradores.TipoAmbiente tipoAmbiente, int[] series, int serie, string tipoOcorrencia, string numeroNF, bool contem, Dominio.Enumeradores.FiltroAverbacaoCTe? averbacaoCTe, int inicioRegistros, int maximoRegistros, int empresaPai = 0, int numeroCarga = 0)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.ConhecimentoDeTransporteEletronico>().Fetch(o => o.Empresa).Fetch(o => o.Serie).Fetch(o => o.Remetente).Fetch(o => o.Destinatario);

            var result = from obj in query
                         where
                             obj.TipoAmbiente == tipoAmbiente &&
                             (obj.ModeloDocumentoFiscal.Numero.Equals("57") || obj.ModeloDocumentoFiscal.Numero.Equals("67"))
                         select obj;

            if (codigoEmpresa > 0)
                result = result.Where(o => o.Empresa.Codigo == codigoEmpresa);

            if (empresaPai > 0)
                result = result.Where(o => o.Empresa.EmpresaPai.Codigo == empresaPai);

            if (dataEmissaoInicial != DateTime.MinValue)
                result = result.Where(o => o.DataEmissao >= dataEmissaoInicial.Date);

            if (dataEmissaoFinal != DateTime.MinValue)
                result = result.Where(o => o.DataEmissao < dataEmissaoFinal.AddDays(1).Date);

            if (numeroInicial > 0)
                result = result.Where(o => o.Numero >= numeroInicial);

            if (numeroFinal > 0)
                result = result.Where(o => o.Numero <= numeroFinal);

            if (!string.IsNullOrWhiteSpace(cpfCnpjDestinatario))
                result = result.Where(o => o.Destinatario.CPF_CNPJ.Equals(cpfCnpjDestinatario));

            if (!string.IsNullOrWhiteSpace(cpfCnpjRemetente))
                result = result.Where(o => o.Remetente.CPF_CNPJ.Equals(cpfCnpjRemetente));

            if (!string.IsNullOrWhiteSpace(status))
            {
                if (status == "V")
                    result = result.Where(o => o.MensagemRetornoSefaz.Contains("Vedado"));
                else
                    result = result.Where(o => o.Status.Equals(status));
            }

            if (tipoCTe != Dominio.Enumeradores.TipoCTE.Todos)
                result = result.Where(o => o.TipoCTE == tipoCTe);

            if (serie > 0)
                result = result.Where(o => o.Serie.Codigo == serie);
            else if (series != null && series.Count() > 0)
            {
                if (series.Count() == 1)
                    result = result.Where(o => o.Serie.Codigo == series.FirstOrDefault());
                else
                    result = result.Where(o => series.Contains(o.Serie.Codigo));
            }

            if (!string.IsNullOrWhiteSpace(placa))
            {
                var queryVeiculos = this.SessionNHiBernate.Query<Dominio.Entidades.VeiculoCTE>();

                result = result.Where(o => (from obj in queryVeiculos where obj.Placa.Equals(placa) select obj.CTE.Codigo).Contains(o.Codigo)); //obj.CTE.Codigo == o.Codigo &&
            }

            if (!string.IsNullOrWhiteSpace(motorista))
            {
                var queryMotorista = this.SessionNHiBernate.Query<Dominio.Entidades.MotoristaCTE>();

                result = result.Where(o => (from obj in queryMotorista where obj.NomeMotoristaCTe.Equals(motorista) select obj.CTE.Codigo).Contains(o.Codigo)); //obj.CTE.Codigo == o.Codigo &&
            }

            if (!string.IsNullOrWhiteSpace(tipoOcorrencia))
            {
                var queryOcorrencias = this.SessionNHiBernate.Query<Dominio.Entidades.OcorrenciaDeCTe>();

                if (tipoOcorrencia == "F")
                    result = result.Where(o => (from obj in queryOcorrencias where obj.CTe.Codigo == o.Codigo && obj.Ocorrencia.Tipo.Equals("F") select obj.CTe.Codigo).Contains(o.Codigo));
                else if (tipoOcorrencia == "P")
                    result = result.Where(o => !(from obj in queryOcorrencias where obj.CTe.Codigo == o.Codigo && obj.Ocorrencia.Tipo.Equals("F") select obj.CTe.Codigo).Contains(o.Codigo));
            }

            if (!string.IsNullOrWhiteSpace(numeroNF))
            {
                var queryDocumentos = this.SessionNHiBernate.Query<Dominio.Entidades.DocumentosCTE>();

                if (!contem)
                    result = result.Where(o => (from obj in queryDocumentos where obj.Numero.Equals(numeroNF) select obj.CTE.Codigo).Contains(o.Codigo)); //obj.CTE.Codigo == o.Codigo &&
                else
                    result = result.Where(o => (from obj in queryDocumentos where obj.Numero.Contains(numeroNF) select obj.CTE.Codigo).Contains(o.Codigo));
            }

            if (averbacaoCTe.HasValue)
            {
                var queryAverbacao = this.SessionNHiBernate.Query<Dominio.Entidades.AverbacaoCTe>();

                if (averbacaoCTe.Value == Dominio.Enumeradores.FiltroAverbacaoCTe.Averbados)
                    result = result.Where(o => (from obj in queryAverbacao where obj.CTe.Codigo == o.Codigo && obj.Status == Dominio.Enumeradores.StatusAverbacaoCTe.Sucesso select obj.CTe.Codigo).Contains(o.Codigo));

                if (averbacaoCTe.Value == Dominio.Enumeradores.FiltroAverbacaoCTe.NaoAverbados)
                    result = result.Where(o => !(from obj in queryAverbacao where obj.CTe.Codigo == o.Codigo && obj.Status == Dominio.Enumeradores.StatusAverbacaoCTe.Sucesso select obj.CTe.Codigo).Contains(o.Codigo));
            }

            if (numeroCarga > 0)
            {
                var queryIntegracaoCTe = this.SessionNHiBernate.Query<Dominio.Entidades.IntegracaoCTe>();
                var resultIntegracaoCTe = from o in queryIntegracaoCTe select o;
                if (numeroCarga > 0)
                    resultIntegracaoCTe = resultIntegracaoCTe.Where(o => o.NumeroDaCarga == numeroCarga);

                result = result.Where(o => resultIntegracaoCTe.Select(c => c.CTe.Codigo).Contains(o.Codigo));
            }

            return result.Select(o => new Dominio.ObjetosDeValor.ConsultaCTe()
            {
                Codigo = o.Codigo,
                DataEmissao = o.DataEmissao,
                Destinatario = o.Destinatario,
                MensagemStatus = o.MensagemStatus,
                MensagemRetornoSefaz = o.MensagemRetornoSefaz,
                Numero = o.Numero,
                Empresa = o.Empresa.Codigo,
                NomeEmpresa = o.Empresa.RazaoSocial,
                Remetente = o.Remetente,
                Serie = o.Serie.Numero,
                Status = o.Status,
                TipoCTe = o.TipoCTE,
                TipoServico = o.TipoServico,
                Valor = o.ValorFrete,
                TerminoPrestacao = o.LocalidadeTerminoPrestacao != null ? o.LocalidadeTerminoPrestacao.Descricao + " / " + o.LocalidadeTerminoPrestacao.Estado.Sigla : string.Empty,
                Placa = (from obj in this.SessionNHiBernate.Query<Dominio.Entidades.VeiculoCTE>() where obj.CTE.Codigo == o.Codigo select obj.Placa).FirstOrDefault(),
                Tomador = o.TomadorPagador
            })
                .OrderByDescending(o => o.DataEmissao)
                .ThenByDescending(o => o.Numero)
                .Skip(inicioRegistros)
                .Take(maximoRegistros)
                .Timeout(120)
                .ToList();
        }

        public int ContarConsulta(int codigoEmpresa, DateTime dataEmissaoInicial, DateTime dataEmissaoFinal, int numeroInicial, int numeroFinal, string placa, string motorista, string cpfCnpjRemetente, string cpfCnpjDestinatario, string status, Dominio.Enumeradores.TipoCTE tipoCTe, Dominio.Enumeradores.TipoAmbiente tipoAmbiente, int[] series, int serie, string tipoOcorrencia, string numeroNF, bool contem, Dominio.Enumeradores.FiltroAverbacaoCTe? averbacaoCTe, int empresaPai = 0, int numeroCarga = 0)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.ConhecimentoDeTransporteEletronico>();

            var result = from obj in query
                         where
                             obj.TipoAmbiente == tipoAmbiente &&
                             (obj.ModeloDocumentoFiscal.Numero.Equals("57") || obj.ModeloDocumentoFiscal.Numero.Equals("67"))
                         select obj;

            if (codigoEmpresa > 0)
                result = result.Where(o => o.Empresa.Codigo == codigoEmpresa);

            if (empresaPai > 0)
                result = result.Where(o => o.Empresa.EmpresaPai.Codigo == empresaPai);

            if (dataEmissaoInicial != DateTime.MinValue)
                result = result.Where(o => o.DataEmissao >= dataEmissaoInicial.Date);

            if (dataEmissaoFinal != DateTime.MinValue)
                result = result.Where(o => o.DataEmissao < dataEmissaoFinal.AddDays(1).Date);

            if (numeroInicial > 0)
                result = result.Where(o => o.Numero >= numeroInicial);

            if (numeroFinal > 0)
                result = result.Where(o => o.Numero <= numeroFinal);

            if (!string.IsNullOrWhiteSpace(cpfCnpjDestinatario))
                result = result.Where(o => o.Destinatario.CPF_CNPJ.Equals(cpfCnpjDestinatario));

            if (!string.IsNullOrWhiteSpace(cpfCnpjRemetente))
                result = result.Where(o => o.Remetente.CPF_CNPJ.Equals(cpfCnpjRemetente));

            if (!string.IsNullOrWhiteSpace(status))
                result = result.Where(o => o.Status.Equals(status));

            if (tipoCTe != Dominio.Enumeradores.TipoCTE.Todos)
                result = result.Where(o => o.TipoCTE == tipoCTe);

            if (serie > 0)
                result = result.Where(o => o.Serie.Codigo == serie);
            else if (series != null && series.Count() > 0)
            {
                if (series.Count() == 1)
                    result = result.Where(o => o.Serie.Codigo == series.FirstOrDefault());
                else
                    result = result.Where(o => series.Contains(o.Serie.Codigo));
            }

            if (!string.IsNullOrWhiteSpace(placa))
            {
                var queryVeiculos = this.SessionNHiBernate.Query<Dominio.Entidades.VeiculoCTE>();

                result = result.Where(o => (from obj in queryVeiculos where obj.Placa.Equals(placa) select obj.CTE.Codigo).Contains(o.Codigo)); //obj.CTE.Codigo == o.Codigo &&
            }

            if (!string.IsNullOrWhiteSpace(motorista))
            {
                var queryMotorista = this.SessionNHiBernate.Query<Dominio.Entidades.MotoristaCTE>();

                result = result.Where(o => (from obj in queryMotorista where obj.NomeMotoristaCTe.Equals(motorista) select obj.CTE.Codigo).Contains(o.Codigo)); //obj.CTE.Codigo == o.Codigo && 
            }

            if (!string.IsNullOrWhiteSpace(tipoOcorrencia))
            {
                var queryOcorrencias = this.SessionNHiBernate.Query<Dominio.Entidades.OcorrenciaDeCTe>();

                if (tipoOcorrencia == "F")
                    result = result.Where(o => (from obj in queryOcorrencias where obj.CTe.Codigo == o.Codigo && obj.Ocorrencia.Tipo.Equals("F") select obj.CTe.Codigo).Contains(o.Codigo));
                else if (tipoOcorrencia == "P")
                    result = result.Where(o => !(from obj in queryOcorrencias where obj.CTe.Codigo == o.Codigo && obj.Ocorrencia.Tipo.Equals("F") select obj.CTe.Codigo).Contains(o.Codigo));
            }

            if (!string.IsNullOrWhiteSpace(numeroNF))
            {
                var queryDocumentos = this.SessionNHiBernate.Query<Dominio.Entidades.DocumentosCTE>();

                if (!contem)
                    result = result.Where(o => (from obj in queryDocumentos where obj.Numero.Equals(numeroNF) select obj.CTE.Codigo).Contains(o.Codigo)); //obj.CTE.Codigo == o.Codigo &&
                else
                    result = result.Where(o => (from obj in queryDocumentos where obj.Numero.Contains(numeroNF) select obj.CTE.Codigo).Contains(o.Codigo));
            }

            if (averbacaoCTe.HasValue)
            {
                var queryAverbacao = this.SessionNHiBernate.Query<Dominio.Entidades.AverbacaoCTe>();

                if (averbacaoCTe.Value == Dominio.Enumeradores.FiltroAverbacaoCTe.Averbados)
                    result = result.Where(o => (from obj in queryAverbacao where obj.CTe.Codigo == o.Codigo && obj.Status == Dominio.Enumeradores.StatusAverbacaoCTe.Sucesso select obj.CTe.Codigo).Contains(o.Codigo));

                if (averbacaoCTe.Value == Dominio.Enumeradores.FiltroAverbacaoCTe.NaoAverbados)
                    result = result.Where(o => !(from obj in queryAverbacao where obj.CTe.Codigo == o.Codigo && obj.Status == Dominio.Enumeradores.StatusAverbacaoCTe.Sucesso select obj.CTe.Codigo).Contains(o.Codigo));
            }

            if (numeroCarga > 0)
            {
                var queryIntegracaoCTe = this.SessionNHiBernate.Query<Dominio.Entidades.IntegracaoCTe>();
                var resultIntegracaoCTe = from o in queryIntegracaoCTe select o;
                if (numeroCarga > 0)
                    resultIntegracaoCTe = resultIntegracaoCTe.Where(o => o.NumeroDaCarga == numeroCarga);

                result = result.Where(o => resultIntegracaoCTe.Select(c => c.CTe.Codigo).Contains(o.Codigo));
            }

            return result.Count();
        }

        public List<Dominio.ObjetosDeValor.ConsultaCTe> ConsultarEmissaoCTe(int codigoEmpresa, DateTime dataEmissaoInicial, DateTime dataEmissaoFinal, int numeroInicial, int numeroFinal, string placa, string motorista, string cpfCnpjRemetente, string cpfCnpjDestinatario, string status, Dominio.Enumeradores.TipoCTE tipoCTe, Dominio.Enumeradores.TipoAmbiente tipoAmbiente, int[] series, int serie, string tipoOcorrencia, string numeroNF, bool contem, Dominio.Enumeradores.FiltroAverbacaoCTe? averbacaoCTe, int inicioRegistros, int maximoRegistros, int empresaPai = 0, int numeroCarga = 0)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.ConhecimentoDeTransporteEletronico>().Fetch(o => o.Empresa).Fetch(o => o.Serie).Fetch(o => o.Remetente).Fetch(o => o.Destinatario);

            var result = from obj in query
                         where
                             obj.TipoAmbiente == tipoAmbiente &&
                             (obj.ModeloDocumentoFiscal.Numero.Equals("57") || obj.ModeloDocumentoFiscal.Numero.Equals("67"))
                         select obj;

            if (codigoEmpresa > 0)
                result = result.Where(o => o.Empresa.Codigo == codigoEmpresa);

            if (empresaPai > 0)
                result = result.Where(o => o.Empresa.EmpresaPai.Codigo == empresaPai);

            if (dataEmissaoInicial != DateTime.MinValue)
                result = result.Where(o => o.DataEmissao >= dataEmissaoInicial.Date);

            if (dataEmissaoFinal != DateTime.MinValue)
                result = result.Where(o => o.DataEmissao < dataEmissaoFinal.AddDays(1).Date);

            if (numeroInicial > 0)
                result = result.Where(o => o.Numero >= numeroInicial);

            if (numeroFinal > 0)
                result = result.Where(o => o.Numero <= numeroFinal);

            if (!string.IsNullOrWhiteSpace(cpfCnpjDestinatario))
                result = result.Where(o => o.Destinatario.CPF_CNPJ.Equals(cpfCnpjDestinatario));

            if (!string.IsNullOrWhiteSpace(cpfCnpjRemetente))
                result = result.Where(o => o.Remetente.CPF_CNPJ.Equals(cpfCnpjRemetente));

            if (!string.IsNullOrWhiteSpace(status))
            {
                if (status == "V")
                    result = result.Where(o => o.MensagemRetornoSefaz.Contains("Vedado"));
                else
                    result = result.Where(o => o.Status.Equals(status));
            }

            if (tipoCTe != Dominio.Enumeradores.TipoCTE.Todos)
                result = result.Where(o => o.TipoCTE == tipoCTe);

            if (serie > 0)
                result = result.Where(o => o.Serie.Numero == serie);
            else if (series != null && series.Count() > 0)
            {
                if (series.Count() == 1)
                    result = result.Where(o => o.Serie.Numero == series.FirstOrDefault());
                else
                    result = result.Where(o => series.Contains(o.Serie.Numero));
            }

            if (!string.IsNullOrWhiteSpace(placa))
            {
                var queryVeiculos = this.SessionNHiBernate.Query<Dominio.Entidades.VeiculoCTE>();

                result = result.Where(o => (from obj in queryVeiculos where obj.Placa.Equals(placa) select obj.CTE.Codigo).Contains(o.Codigo)); //obj.CTE.Codigo == o.Codigo &&
            }

            if (!string.IsNullOrWhiteSpace(motorista))
            {
                var queryMotorista = this.SessionNHiBernate.Query<Dominio.Entidades.MotoristaCTE>();

                result = result.Where(o => (from obj in queryMotorista where obj.NomeMotoristaCTe.Equals(motorista) select obj.CTE.Codigo).Contains(o.Codigo)); //obj.CTE.Codigo == o.Codigo &&
            }

            if (!string.IsNullOrWhiteSpace(tipoOcorrencia))
            {
                var queryOcorrencias = this.SessionNHiBernate.Query<Dominio.Entidades.OcorrenciaDeCTe>();

                if (tipoOcorrencia == "F")
                    result = result.Where(o => (from obj in queryOcorrencias where obj.CTe.Codigo == o.Codigo && obj.Ocorrencia.Tipo.Equals("F") select obj.CTe.Codigo).Contains(o.Codigo));
                else if (tipoOcorrencia == "P")
                    result = result.Where(o => !(from obj in queryOcorrencias where obj.CTe.Codigo == o.Codigo && obj.Ocorrencia.Tipo.Equals("F") select obj.CTe.Codigo).Contains(o.Codigo));
            }

            if (!string.IsNullOrWhiteSpace(numeroNF))
            {
                var queryDocumentos = this.SessionNHiBernate.Query<Dominio.Entidades.DocumentosCTE>();

                if (!contem)
                    result = result.Where(o => (from obj in queryDocumentos where obj.Numero.Equals(numeroNF) select obj.CTE.Codigo).Contains(o.Codigo)); //obj.CTE.Codigo == o.Codigo &&
                else
                    result = result.Where(o => (from obj in queryDocumentos where obj.Numero.Contains(numeroNF) select obj.CTE.Codigo).Contains(o.Codigo));
            }

            if (averbacaoCTe.HasValue)
            {
                var queryAverbacao = this.SessionNHiBernate.Query<Dominio.Entidades.AverbacaoCTe>();

                if (averbacaoCTe.Value == Dominio.Enumeradores.FiltroAverbacaoCTe.Averbados)
                    result = result.Where(o => (from obj in queryAverbacao where obj.CTe.Codigo == o.Codigo && obj.Status == Dominio.Enumeradores.StatusAverbacaoCTe.Sucesso select obj.CTe.Codigo).Contains(o.Codigo));

                if (averbacaoCTe.Value == Dominio.Enumeradores.FiltroAverbacaoCTe.NaoAverbados)
                    result = result.Where(o => !(from obj in queryAverbacao where obj.CTe.Codigo == o.Codigo && obj.Status == Dominio.Enumeradores.StatusAverbacaoCTe.Sucesso select obj.CTe.Codigo).Contains(o.Codigo));
            }

            if (numeroCarga > 0)
            {
                var queryIntegracaoCTe = this.SessionNHiBernate.Query<Dominio.Entidades.IntegracaoCTe>();
                var resultIntegracaoCTe = from o in queryIntegracaoCTe select o;
                if (numeroCarga > 0)
                    resultIntegracaoCTe = resultIntegracaoCTe.Where(o => o.NumeroDaCarga == numeroCarga);

                result = result.Where(o => resultIntegracaoCTe.Select(c => c.CTe.Codigo).Contains(o.Codigo));
            }

            return result.Select(o => new Dominio.ObjetosDeValor.ConsultaCTe()
            {
                Codigo = o.Codigo,
                DataEmissao = o.DataEmissao,
                Destinatario = o.Destinatario,
                MensagemStatus = o.MensagemStatus,
                MensagemRetornoSefaz = o.MensagemRetornoSefaz,
                Numero = o.Numero,
                Empresa = o.Empresa.Codigo,
                NomeEmpresa = o.Empresa.RazaoSocial,
                Remetente = o.Remetente,
                Serie = o.Serie.Numero,
                Status = o.Status,
                TipoCTe = o.TipoCTE,
                TipoServico = o.TipoServico,
                Valor = o.ValorFrete,
                TerminoPrestacao = o.LocalidadeTerminoPrestacao != null ? o.LocalidadeTerminoPrestacao.Descricao + " / " + o.LocalidadeTerminoPrestacao.Estado.Sigla : string.Empty,
                Placa = (from obj in this.SessionNHiBernate.Query<Dominio.Entidades.VeiculoCTE>() where obj.CTE.Codigo == o.Codigo select obj.Placa).FirstOrDefault(),
                Tomador = o.TomadorPagador
            })
                .OrderByDescending(o => o.DataEmissao)
                .ThenByDescending(o => o.Numero)
                .Skip(inicioRegistros)
                .Take(maximoRegistros)
                .Timeout(120)
                .ToList();
        }

        public int ContarConsultaEmissaoCTe(int codigoEmpresa, DateTime dataEmissaoInicial, DateTime dataEmissaoFinal, int numeroInicial, int numeroFinal, string placa, string motorista, string cpfCnpjRemetente, string cpfCnpjDestinatario, string status, Dominio.Enumeradores.TipoCTE tipoCTe, Dominio.Enumeradores.TipoAmbiente tipoAmbiente, int[] series, int serie, string tipoOcorrencia, string numeroNF, bool contem, Dominio.Enumeradores.FiltroAverbacaoCTe? averbacaoCTe, int empresaPai = 0, int numeroCarga = 0)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.ConhecimentoDeTransporteEletronico>();

            var result = from obj in query
                         where
                             obj.TipoAmbiente == tipoAmbiente &&
                             (obj.ModeloDocumentoFiscal.Numero.Equals("57") || obj.ModeloDocumentoFiscal.Numero.Equals("67"))
                         select obj;

            if (codigoEmpresa > 0)
                result = result.Where(o => o.Empresa.Codigo == codigoEmpresa);

            if (empresaPai > 0)
                result = result.Where(o => o.Empresa.EmpresaPai.Codigo == empresaPai);

            if (dataEmissaoInicial != DateTime.MinValue)
                result = result.Where(o => o.DataEmissao >= dataEmissaoInicial.Date);

            if (dataEmissaoFinal != DateTime.MinValue)
                result = result.Where(o => o.DataEmissao < dataEmissaoFinal.AddDays(1).Date);

            if (numeroInicial > 0)
                result = result.Where(o => o.Numero >= numeroInicial);

            if (numeroFinal > 0)
                result = result.Where(o => o.Numero <= numeroFinal);

            if (!string.IsNullOrWhiteSpace(cpfCnpjDestinatario))
                result = result.Where(o => o.Destinatario.CPF_CNPJ.Equals(cpfCnpjDestinatario));

            if (!string.IsNullOrWhiteSpace(cpfCnpjRemetente))
                result = result.Where(o => o.Remetente.CPF_CNPJ.Equals(cpfCnpjRemetente));

            if (!string.IsNullOrWhiteSpace(status))
                result = result.Where(o => o.Status.Equals(status));

            if (tipoCTe != Dominio.Enumeradores.TipoCTE.Todos)
                result = result.Where(o => o.TipoCTE == tipoCTe);

            if (serie > 0)
                result = result.Where(o => o.Serie.Numero == serie);
            else if (series != null && series.Count() > 0)
            {
                if (series.Count() == 1)
                    result = result.Where(o => o.Serie.Numero == series.FirstOrDefault());
                else
                    result = result.Where(o => series.Contains(o.Serie.Numero));
            }

            if (!string.IsNullOrWhiteSpace(placa))
            {
                var queryVeiculos = this.SessionNHiBernate.Query<Dominio.Entidades.VeiculoCTE>();

                result = result.Where(o => (from obj in queryVeiculos where obj.Placa.Equals(placa) select obj.CTE.Codigo).Contains(o.Codigo)); //obj.CTE.Codigo == o.Codigo &&
            }

            if (!string.IsNullOrWhiteSpace(motorista))
            {
                var queryMotorista = this.SessionNHiBernate.Query<Dominio.Entidades.MotoristaCTE>();

                result = result.Where(o => (from obj in queryMotorista where obj.NomeMotoristaCTe.Equals(motorista) select obj.CTE.Codigo).Contains(o.Codigo)); //obj.CTE.Codigo == o.Codigo && 
            }

            if (!string.IsNullOrWhiteSpace(tipoOcorrencia))
            {
                var queryOcorrencias = this.SessionNHiBernate.Query<Dominio.Entidades.OcorrenciaDeCTe>();

                if (tipoOcorrencia == "F")
                    result = result.Where(o => (from obj in queryOcorrencias where obj.CTe.Codigo == o.Codigo && obj.Ocorrencia.Tipo.Equals("F") select obj.CTe.Codigo).Contains(o.Codigo));
                else if (tipoOcorrencia == "P")
                    result = result.Where(o => !(from obj in queryOcorrencias where obj.CTe.Codigo == o.Codigo && obj.Ocorrencia.Tipo.Equals("F") select obj.CTe.Codigo).Contains(o.Codigo));
            }

            if (!string.IsNullOrWhiteSpace(numeroNF))
            {
                var queryDocumentos = this.SessionNHiBernate.Query<Dominio.Entidades.DocumentosCTE>();

                if (!contem)
                    result = result.Where(o => (from obj in queryDocumentos where obj.Numero.Equals(numeroNF) select obj.CTE.Codigo).Contains(o.Codigo)); //obj.CTE.Codigo == o.Codigo &&
                else
                    result = result.Where(o => (from obj in queryDocumentos where obj.Numero.Contains(numeroNF) select obj.CTE.Codigo).Contains(o.Codigo));
            }

            if (averbacaoCTe.HasValue)
            {
                var queryAverbacao = this.SessionNHiBernate.Query<Dominio.Entidades.AverbacaoCTe>();

                if (averbacaoCTe.Value == Dominio.Enumeradores.FiltroAverbacaoCTe.Averbados)
                    result = result.Where(o => (from obj in queryAverbacao where obj.CTe.Codigo == o.Codigo && obj.Status == Dominio.Enumeradores.StatusAverbacaoCTe.Sucesso select obj.CTe.Codigo).Contains(o.Codigo));

                if (averbacaoCTe.Value == Dominio.Enumeradores.FiltroAverbacaoCTe.NaoAverbados)
                    result = result.Where(o => !(from obj in queryAverbacao where obj.CTe.Codigo == o.Codigo && obj.Status == Dominio.Enumeradores.StatusAverbacaoCTe.Sucesso select obj.CTe.Codigo).Contains(o.Codigo));
            }

            if (numeroCarga > 0)
            {
                var queryIntegracaoCTe = this.SessionNHiBernate.Query<Dominio.Entidades.IntegracaoCTe>();
                var resultIntegracaoCTe = from o in queryIntegracaoCTe select o;
                if (numeroCarga > 0)
                    resultIntegracaoCTe = resultIntegracaoCTe.Where(o => o.NumeroDaCarga == numeroCarga);

                result = result.Where(o => resultIntegracaoCTe.Select(c => c.CTe.Codigo).Contains(o.Codigo));
            }

            return result.Count();
        }

        public List<Dominio.ObjetosDeValor.ConsultaCTe> ConsultarAdmin(int codigoEmpresa, DateTime dataEmissaoInicial, DateTime dataEmissaoFinal, int numeroInicial, int numeroFinal, string placa, string motorista, string cpfCnpjRemetente, string cpfCnpjDestinatario, string status, Dominio.Enumeradores.TipoCTE tipoCTe, Dominio.Enumeradores.TipoAmbiente tipoAmbiente, int[] series, int serie, string tipoOcorrencia, string numeroNF, bool contem, Dominio.Enumeradores.FiltroAverbacaoCTe? averbacaoCTe, int inicioRegistros, int maximoRegistros, int empresaPai = 0, int numeroCarga = 0)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.ConhecimentoDeTransporteEletronico>().Fetch(o => o.Empresa).Fetch(o => o.Serie).Fetch(o => o.Remetente).Fetch(o => o.Destinatario);

            var result = from obj in query
                         where
                             (obj.TipoAmbiente == tipoAmbiente || obj.TipoAmbiente == Dominio.Enumeradores.TipoAmbiente.Nenhum) &&
                             (obj.ModeloDocumentoFiscal.Numero.Equals("57") || obj.ModeloDocumentoFiscal.Numero.Equals("67"))
                         select obj;

            if (codigoEmpresa > 0)
                result = result.Where(o => o.Empresa.Codigo == codigoEmpresa);

            if (empresaPai > 0)
                result = result.Where(o => o.Empresa.EmpresaPai.Codigo == empresaPai);

            if (dataEmissaoInicial != DateTime.MinValue)
                result = result.Where(o => (o.DataEmissao >= dataEmissaoInicial.Date || o.DataEmissao == null));

            if (dataEmissaoFinal != DateTime.MinValue)
                result = result.Where(o => (o.DataEmissao < dataEmissaoFinal.AddDays(1).Date || o.DataEmissao == null));

            if (numeroInicial > 0)
                result = result.Where(o => o.Numero >= numeroInicial);

            if (numeroFinal > 0)
                result = result.Where(o => o.Numero <= numeroFinal);

            if (!string.IsNullOrWhiteSpace(cpfCnpjDestinatario))
                result = result.Where(o => o.Destinatario.CPF_CNPJ.Equals(cpfCnpjDestinatario));

            if (!string.IsNullOrWhiteSpace(cpfCnpjRemetente))
                result = result.Where(o => o.Remetente.CPF_CNPJ.Equals(cpfCnpjRemetente));

            if (!string.IsNullOrWhiteSpace(status))
                result = result.Where(o => o.Status.Equals(status));

            if (tipoCTe != Dominio.Enumeradores.TipoCTE.Todos)
                result = result.Where(o => o.TipoCTE == tipoCTe);

            if (serie > 0)
                result = result.Where(o => o.Serie.Codigo == serie);
            else if (series != null && series.Count() > 0)
            {
                if (series.Count() == 1)
                    result = result.Where(o => o.Serie.Codigo == series.FirstOrDefault());
                else
                    result = result.Where(o => series.Contains(o.Serie.Codigo));
            }

            if (!string.IsNullOrWhiteSpace(placa))
            {
                var queryVeiculos = this.SessionNHiBernate.Query<Dominio.Entidades.VeiculoCTE>();

                result = result.Where(o => (from obj in queryVeiculos where obj.Placa.Equals(placa) select obj.CTE.Codigo).Contains(o.Codigo)); //obj.CTE.Codigo == o.Codigo &&
            }

            if (!string.IsNullOrWhiteSpace(motorista))
            {
                var queryMotorista = this.SessionNHiBernate.Query<Dominio.Entidades.MotoristaCTE>();

                result = result.Where(o => (from obj in queryMotorista where obj.NomeMotoristaCTe.Equals(motorista) select obj.CTE.Codigo).Contains(o.Codigo)); //obj.CTE.Codigo == o.Codigo &&
            }

            if (!string.IsNullOrWhiteSpace(tipoOcorrencia))
            {
                var queryOcorrencias = this.SessionNHiBernate.Query<Dominio.Entidades.OcorrenciaDeCTe>();

                if (tipoOcorrencia == "F")
                    result = result.Where(o => (from obj in queryOcorrencias where obj.CTe.Codigo == o.Codigo && obj.Ocorrencia.Tipo.Equals("F") select obj.CTe.Codigo).Contains(o.Codigo));
                else if (tipoOcorrencia == "P")
                    result = result.Where(o => !(from obj in queryOcorrencias where obj.CTe.Codigo == o.Codigo && obj.Ocorrencia.Tipo.Equals("F") select obj.CTe.Codigo).Contains(o.Codigo));
            }

            if (!string.IsNullOrWhiteSpace(numeroNF))
            {
                var queryDocumentos = this.SessionNHiBernate.Query<Dominio.Entidades.DocumentosCTE>();

                if (!contem)
                    result = result.Where(o => (from obj in queryDocumentos where obj.Numero.Equals(numeroNF) select obj.CTE.Codigo).Contains(o.Codigo)); //obj.CTE.Codigo == o.Codigo &&
                else
                    result = result.Where(o => (from obj in queryDocumentos where obj.Numero.Contains(numeroNF) select obj.CTE.Codigo).Contains(o.Codigo));
            }

            if (averbacaoCTe.HasValue)
            {
                var queryAverbacao = this.SessionNHiBernate.Query<Dominio.Entidades.AverbacaoCTe>();

                if (averbacaoCTe.Value == Dominio.Enumeradores.FiltroAverbacaoCTe.Averbados)
                    result = result.Where(o => (from obj in queryAverbacao where obj.CTe.Codigo == o.Codigo && obj.Status == Dominio.Enumeradores.StatusAverbacaoCTe.Sucesso select obj.CTe.Codigo).Contains(o.Codigo));

                if (averbacaoCTe.Value == Dominio.Enumeradores.FiltroAverbacaoCTe.NaoAverbados)
                    result = result.Where(o => !(from obj in queryAverbacao where obj.CTe.Codigo == o.Codigo && obj.Status == Dominio.Enumeradores.StatusAverbacaoCTe.Sucesso select obj.CTe.Codigo).Contains(o.Codigo));
            }

            if (numeroCarga > 0)
            {
                var queryIntegracaoCTe = this.SessionNHiBernate.Query<Dominio.Entidades.IntegracaoCTe>();
                var resultIntegracaoCTe = from o in queryIntegracaoCTe select o;
                if (numeroCarga > 0)
                    resultIntegracaoCTe = resultIntegracaoCTe.Where(o => o.NumeroDaCarga == numeroCarga);

                result = result.Where(o => resultIntegracaoCTe.Select(c => c.CTe.Codigo).Contains(o.Codigo));
            }

            return result.Select(o => new Dominio.ObjetosDeValor.ConsultaCTe()
            {
                Codigo = o.Codigo,
                DataEmissao = o.DataEmissao,
                Destinatario = o.Destinatario,
                MensagemStatus = o.MensagemStatus,
                MensagemRetornoSefaz = o.MensagemRetornoSefaz,
                Numero = o.Numero,
                Empresa = o.Empresa.Codigo,
                NomeEmpresa = o.Empresa.RazaoSocial + " (" + o.Empresa.CNPJ + ")",
                Remetente = o.Remetente,
                Serie = o.Serie.Numero,
                Status = o.Status,
                TipoCTe = o.TipoCTE,
                TipoServico = o.TipoServico,
                Valor = o.ValorFrete,
                TerminoPrestacao = o.LocalidadeTerminoPrestacao != null ? o.LocalidadeTerminoPrestacao.Descricao + " / " + o.LocalidadeTerminoPrestacao.Estado.Sigla : string.Empty,
                Placa = (from obj in this.SessionNHiBernate.Query<Dominio.Entidades.VeiculoCTE>() where obj.CTE.Codigo == o.Codigo select obj.Placa).FirstOrDefault(),
                Tomador = o.TomadorPagador,
                StatusIntegracao = (from obj in this.SessionNHiBernate.Query<Dominio.Entidades.IntegracaoCTe>() where obj.CTe.Codigo == o.Codigo && obj.Tipo == Dominio.Enumeradores.TipoIntegracao.Emissao select obj.Status.ToString()).FirstOrDefault()
            })
                .OrderByDescending(o => o.DataEmissao)
                .ThenByDescending(o => o.Numero)
                .Skip(inicioRegistros)
                .Take(maximoRegistros)
                .Timeout(120)
                .ToList();
        }

        public int ContarConsultaAdmin(int codigoEmpresa, DateTime dataEmissaoInicial, DateTime dataEmissaoFinal, int numeroInicial, int numeroFinal, string placa, string motorista, string cpfCnpjRemetente, string cpfCnpjDestinatario, string status, Dominio.Enumeradores.TipoCTE tipoCTe, Dominio.Enumeradores.TipoAmbiente tipoAmbiente, int[] series, int serie, string tipoOcorrencia, string numeroNF, bool contem, Dominio.Enumeradores.FiltroAverbacaoCTe? averbacaoCTe, int empresaPai = 0, int numeroCarga = 0)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.ConhecimentoDeTransporteEletronico>();

            var result = from obj in query
                         where
                             (obj.TipoAmbiente == tipoAmbiente || obj.TipoAmbiente == Dominio.Enumeradores.TipoAmbiente.Nenhum) &&
                             (obj.ModeloDocumentoFiscal.Numero.Equals("57") || obj.ModeloDocumentoFiscal.Numero.Equals("67"))
                         select obj;

            if (codigoEmpresa > 0)
                result = result.Where(o => o.Empresa.Codigo == codigoEmpresa);

            if (empresaPai > 0)
                result = result.Where(o => o.Empresa.EmpresaPai.Codigo == empresaPai);

            if (dataEmissaoInicial != DateTime.MinValue)
                result = result.Where(o => o.DataEmissao >= dataEmissaoInicial.Date);

            if (dataEmissaoFinal != DateTime.MinValue)
                result = result.Where(o => o.DataEmissao < dataEmissaoFinal.AddDays(1).Date);

            if (numeroInicial > 0)
                result = result.Where(o => o.Numero >= numeroInicial);

            if (numeroFinal > 0)
                result = result.Where(o => o.Numero <= numeroFinal);

            if (!string.IsNullOrWhiteSpace(cpfCnpjDestinatario))
                result = result.Where(o => o.Destinatario.CPF_CNPJ.Equals(cpfCnpjDestinatario));

            if (!string.IsNullOrWhiteSpace(cpfCnpjRemetente))
                result = result.Where(o => o.Remetente.CPF_CNPJ.Equals(cpfCnpjRemetente));

            if (!string.IsNullOrWhiteSpace(status))
                result = result.Where(o => o.Status.Equals(status));

            if (tipoCTe != Dominio.Enumeradores.TipoCTE.Todos)
                result = result.Where(o => o.TipoCTE == tipoCTe);

            if (serie > 0)
                result = result.Where(o => o.Serie.Codigo == serie);
            else if (series != null && series.Count() > 0)
            {
                if (series.Count() == 1)
                    result = result.Where(o => o.Serie.Codigo == series.FirstOrDefault());
                else
                    result = result.Where(o => series.Contains(o.Serie.Codigo));
            }

            if (!string.IsNullOrWhiteSpace(placa))
            {
                var queryVeiculos = this.SessionNHiBernate.Query<Dominio.Entidades.VeiculoCTE>();

                result = result.Where(o => (from obj in queryVeiculos where obj.Placa.Equals(placa) select obj.CTE.Codigo).Contains(o.Codigo)); //obj.CTE.Codigo == o.Codigo &&
            }

            if (!string.IsNullOrWhiteSpace(motorista))
            {
                var queryMotorista = this.SessionNHiBernate.Query<Dominio.Entidades.MotoristaCTE>();

                result = result.Where(o => (from obj in queryMotorista where obj.NomeMotoristaCTe.Equals(motorista) select obj.CTE.Codigo).Contains(o.Codigo)); //obj.CTE.Codigo == o.Codigo && 
            }

            if (!string.IsNullOrWhiteSpace(tipoOcorrencia))
            {
                var queryOcorrencias = this.SessionNHiBernate.Query<Dominio.Entidades.OcorrenciaDeCTe>();

                if (tipoOcorrencia == "F")
                    result = result.Where(o => (from obj in queryOcorrencias where obj.CTe.Codigo == o.Codigo && obj.Ocorrencia.Tipo.Equals("F") select obj.CTe.Codigo).Contains(o.Codigo));
                else if (tipoOcorrencia == "P")
                    result = result.Where(o => !(from obj in queryOcorrencias where obj.CTe.Codigo == o.Codigo && obj.Ocorrencia.Tipo.Equals("F") select obj.CTe.Codigo).Contains(o.Codigo));
            }

            if (!string.IsNullOrWhiteSpace(numeroNF))
            {
                var queryDocumentos = this.SessionNHiBernate.Query<Dominio.Entidades.DocumentosCTE>();

                if (!contem)
                    result = result.Where(o => (from obj in queryDocumentos where obj.Numero.Equals(numeroNF) select obj.CTE.Codigo).Contains(o.Codigo)); //obj.CTE.Codigo == o.Codigo &&
                else
                    result = result.Where(o => (from obj in queryDocumentos where obj.Numero.Contains(numeroNF) select obj.CTE.Codigo).Contains(o.Codigo));
            }

            if (averbacaoCTe.HasValue)
            {
                var queryAverbacao = this.SessionNHiBernate.Query<Dominio.Entidades.AverbacaoCTe>();

                if (averbacaoCTe.Value == Dominio.Enumeradores.FiltroAverbacaoCTe.Averbados)
                    result = result.Where(o => (from obj in queryAverbacao where obj.CTe.Codigo == o.Codigo && obj.Status == Dominio.Enumeradores.StatusAverbacaoCTe.Sucesso select obj.CTe.Codigo).Contains(o.Codigo));

                if (averbacaoCTe.Value == Dominio.Enumeradores.FiltroAverbacaoCTe.NaoAverbados)
                    result = result.Where(o => !(from obj in queryAverbacao where obj.CTe.Codigo == o.Codigo && obj.Status == Dominio.Enumeradores.StatusAverbacaoCTe.Sucesso select obj.CTe.Codigo).Contains(o.Codigo));
            }

            if (numeroCarga > 0)
            {
                var queryIntegracaoCTe = this.SessionNHiBernate.Query<Dominio.Entidades.IntegracaoCTe>();
                var resultIntegracaoCTe = from o in queryIntegracaoCTe select o;
                if (numeroCarga > 0)
                    resultIntegracaoCTe = resultIntegracaoCTe.Where(o => o.NumeroDaCarga == numeroCarga);

                result = result.Where(o => resultIntegracaoCTe.Select(c => c.CTe.Codigo).Contains(o.Codigo));
            }

            return result.Count();
        }

        public List<Dominio.ObjetosDeValor.ConsultaCTe> ConsultarPorDuplicata(int codigoEmpresa, DateTime dataEmissaoInicial, DateTime dataEmissaoFinal, int numeroInicial, int numeroFinal, string placa, string motorista, string cpfCnpjRemetente, string cpfCnpjDestinatario, string[] status, Dominio.Enumeradores.TipoCTE tipoCTe, Dominio.Enumeradores.TipoAmbiente tipoAmbiente, int[] series, int serie, string tipoOcorrencia, string numeroNF, int inicioRegistros, int maximoRegistros, int codigoDuplicata, bool filtrarNFSe = true)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.ConhecimentoDeTransporteEletronico>();

            var result = from obj in query
                         where
                             obj.TipoAmbiente == tipoAmbiente &&
                             obj.Empresa.Codigo == codigoEmpresa
                         select obj;

            if (filtrarNFSe)
                result = result.Where(o => (o.ModeloDocumentoFiscal.Numero.Equals("57") || o.ModeloDocumentoFiscal.Numero.Equals("67") || o.ModeloDocumentoFiscal.Numero.Equals("39")));
            else
                result = result.Where(o => (o.ModeloDocumentoFiscal.Numero.Equals("57") || o.ModeloDocumentoFiscal.Numero.Equals("67")));

            if (codigoDuplicata > 0)
            {
                var queryDuplicatas = this.SessionNHiBernate.Query<Dominio.Entidades.DuplicataCtes>();

                result = result.Where(o => (from obj in queryDuplicatas where (obj.ConhecimentoDeTransporteEletronico.Codigo == o.Codigo) && (obj.Duplicata.Codigo == codigoDuplicata) select obj.ConhecimentoDeTransporteEletronico.Codigo).Contains(o.Codigo));
            }
            else
            {

                if (dataEmissaoInicial != DateTime.MinValue)
                    result = result.Where(o => o.DataEmissao >= dataEmissaoInicial.Date);

                if (dataEmissaoFinal != DateTime.MinValue)
                    result = result.Where(o => o.DataEmissao < dataEmissaoFinal.AddDays(1).Date);

                if (numeroInicial > 0)
                    result = result.Where(o => o.Numero >= numeroInicial);

                if (numeroFinal > 0)
                    result = result.Where(o => o.Numero <= numeroFinal);

                if (!string.IsNullOrWhiteSpace(cpfCnpjDestinatario))
                    result = result.Where(o => o.Destinatario.CPF_CNPJ.Equals(cpfCnpjDestinatario));

                if (!string.IsNullOrWhiteSpace(cpfCnpjRemetente))
                    result = result.Where(o => o.Remetente.CPF_CNPJ.Equals(cpfCnpjRemetente));

                if (status != null && status.Count() > 0)
                    result = result.Where(o => status.Contains(o.Status));

                if (tipoCTe != Dominio.Enumeradores.TipoCTE.Todos)
                    result = result.Where(o => o.TipoCTE == tipoCTe);

                if (serie > 0)
                    result = result.Where(o => (o.Serie.Codigo == serie || o.ModeloDocumentoFiscal.Numero.Equals("39")));
                else if (series != null && series.Count() > 0)
                    result = result.Where(o => (series.Contains(o.Serie.Codigo) || o.ModeloDocumentoFiscal.Numero.Equals("39")));

                if (!string.IsNullOrWhiteSpace(placa))
                {
                    var queryVeiculos = this.SessionNHiBernate.Query<Dominio.Entidades.VeiculoCTE>();

                    result = result.Where(o => (from obj in queryVeiculos where obj.Placa.Equals(placa) select obj.CTE.Codigo).Contains(o.Codigo)); //obj.CTE.Codigo == o.Codigo &&
                }

                if (!string.IsNullOrWhiteSpace(motorista))
                {
                    var queryMotorista = this.SessionNHiBernate.Query<Dominio.Entidades.MotoristaCTE>();

                    result = result.Where(o => (from obj in queryMotorista where obj.NomeMotoristaCTe.Equals(motorista) select obj.CTE.Codigo).Contains(o.Codigo)); //obj.CTE.Codigo == o.Codigo &&
                }

                if (!string.IsNullOrWhiteSpace(tipoOcorrencia))
                {
                    var queryOcorrencias = this.SessionNHiBernate.Query<Dominio.Entidades.OcorrenciaDeCTe>();

                    if (tipoOcorrencia == "F")
                        result = result.Where(o => (from obj in queryOcorrencias where obj.CTe.Codigo == o.Codigo && obj.Ocorrencia.Tipo.Equals("F") select obj.CTe.Codigo).Contains(o.Codigo));
                    else
                        result = result.Where(o => !(from obj in queryOcorrencias where obj.CTe.Codigo == o.Codigo && obj.Ocorrencia.Tipo.Equals("F") select obj.CTe.Codigo).Contains(o.Codigo));
                }

                if (!string.IsNullOrWhiteSpace(numeroNF))
                {
                    var queryDocumentos = this.SessionNHiBernate.Query<Dominio.Entidades.DocumentosCTE>();

                    result = result.Where(o => (from obj in queryDocumentos where obj.CTE.Codigo == o.Codigo && obj.Numero.Equals(numeroNF) select obj.CTE.Codigo).Contains(o.Codigo));
                }
            }

            return result.Select(o => new Dominio.ObjetosDeValor.ConsultaCTe()
            {
                Codigo = o.Codigo,
                DataEmissao = o.DataEmissao,
                Destinatario = o.Destinatario,
                MensagemStatus = o.MensagemStatus,
                MensagemRetornoSefaz = o.MensagemRetornoSefaz,
                Numero = o.Numero,
                Remetente = o.Remetente,
                Serie = o.Serie.Numero,
                Status = o.Status,
                TipoCTe = o.TipoCTE,
                TipoServico = o.TipoServico,
                Valor = o.ValorFrete,
                Placa = (from obj in this.SessionNHiBernate.Query<Dominio.Entidades.VeiculoCTE>() where obj.CTE.Codigo == o.Codigo select obj.Placa).FirstOrDefault(),
                Documento = o.ModeloDocumentoFiscal.Abreviacao
            })
                   .OrderByDescending(o => o.Numero)
                   .Skip(inicioRegistros)
                   .Take(maximoRegistros)
                   .Timeout(120)
                   .ToList();
        }

        public int ContarConsultaPorDuplicata(int codigoEmpresa, DateTime dataEmissaoInicial, DateTime dataEmissaoFinal, int numeroInicial, int numeroFinal, string placa, string motorista, string cpfCnpjRemetente, string cpfCnpjDestinatario, string[] status, Dominio.Enumeradores.TipoCTE tipoCTe, Dominio.Enumeradores.TipoAmbiente tipoAmbiente, int[] series, int serie, string tipoOcorrencia, string numeroNF, int codigoDuplicata = 0, bool filtrarNFSe = true)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.ConhecimentoDeTransporteEletronico>();

            var result = from obj in query
                         where
                             obj.TipoAmbiente == tipoAmbiente &&
                             obj.Empresa.Codigo == codigoEmpresa &&
                             (obj.ModeloDocumentoFiscal.Numero.Equals("57") || obj.ModeloDocumentoFiscal.Numero.Equals("67") || obj.ModeloDocumentoFiscal.Numero.Equals("39"))
                         select obj;

            if (filtrarNFSe)
                result = result.Where(o => (o.ModeloDocumentoFiscal.Numero.Equals("57") || o.ModeloDocumentoFiscal.Numero.Equals("67") || o.ModeloDocumentoFiscal.Numero.Equals("39")));
            else
                result = result.Where(o => (o.ModeloDocumentoFiscal.Numero.Equals("57") || o.ModeloDocumentoFiscal.Numero.Equals("67")));

            if (codigoDuplicata > 0)
            {
                var queryDuplicatas = this.SessionNHiBernate.Query<Dominio.Entidades.DuplicataCtes>();

                result = result.Where(o => (from obj in queryDuplicatas where (obj.ConhecimentoDeTransporteEletronico.Codigo == o.Codigo) && (obj.Duplicata.Codigo == codigoDuplicata) select obj.ConhecimentoDeTransporteEletronico.Codigo).Contains(o.Codigo));
            }
            else
            {
                if (dataEmissaoInicial != DateTime.MinValue)
                    result = result.Where(o => o.DataEmissao >= dataEmissaoInicial.Date);

                if (dataEmissaoFinal != DateTime.MinValue)
                    result = result.Where(o => o.DataEmissao < dataEmissaoFinal.AddDays(1).Date);

                if (numeroInicial > 0)
                    result = result.Where(o => o.Numero >= numeroInicial);

                if (numeroFinal > 0)
                    result = result.Where(o => o.Numero <= numeroFinal);

                if (!string.IsNullOrWhiteSpace(cpfCnpjDestinatario))
                    result = result.Where(o => o.Destinatario.CPF_CNPJ.Equals(cpfCnpjDestinatario));

                if (!string.IsNullOrWhiteSpace(cpfCnpjRemetente))
                    result = result.Where(o => o.Remetente.CPF_CNPJ.Equals(cpfCnpjRemetente));

                if (status != null && status.Count() > 0)
                    result = result.Where(o => status.Contains(o.Status));

                if (tipoCTe != Dominio.Enumeradores.TipoCTE.Todos)
                    result = result.Where(o => o.TipoCTE == tipoCTe);

                if (serie > 0)
                    result = result.Where(o => o.Serie.Codigo == serie);
                else if (series != null && series.Count() > 0)
                    result = result.Where(o => series.Contains(o.Serie.Codigo));

                if (!string.IsNullOrWhiteSpace(placa))
                {
                    var queryVeiculos = this.SessionNHiBernate.Query<Dominio.Entidades.VeiculoCTE>();

                    result = result.Where(o => (from obj in queryVeiculos where obj.CTE.Codigo == o.Codigo && obj.Placa.Equals(placa) select obj.CTE.Codigo).Contains(o.Codigo));
                }

                if (!string.IsNullOrWhiteSpace(motorista))
                {
                    var queryMotorista = this.SessionNHiBernate.Query<Dominio.Entidades.MotoristaCTE>();

                    result = result.Where(o => (from obj in queryMotorista where obj.CTE.Codigo == o.Codigo && obj.NomeMotoristaCTe.Contains(motorista) select obj.CTE.Codigo).Contains(o.Codigo));
                }

                if (!string.IsNullOrWhiteSpace(tipoOcorrencia))
                {
                    var queryOcorrencias = this.SessionNHiBernate.Query<Dominio.Entidades.OcorrenciaDeCTe>();

                    if (tipoOcorrencia == "F")
                        result = result.Where(o => (from obj in queryOcorrencias where obj.CTe.Codigo == o.Codigo && obj.Ocorrencia.Tipo.Equals("F") select obj.CTe.Codigo).Contains(o.Codigo));
                    else
                        result = result.Where(o => !(from obj in queryOcorrencias where obj.CTe.Codigo == o.Codigo && obj.Ocorrencia.Tipo.Equals("F") select obj.CTe.Codigo).Contains(o.Codigo));
                }

                if (!string.IsNullOrWhiteSpace(numeroNF))
                {
                    var queryDocumentos = this.SessionNHiBernate.Query<Dominio.Entidades.DocumentosCTE>();

                    result = result.Where(o => (from obj in queryDocumentos where obj.CTE.Codigo == o.Codigo && obj.Numero.Equals(numeroNF) select obj.CTE.Codigo).Contains(o.Codigo));
                }
            }

            return result.Count();
        }

        public List<Dominio.Entidades.ConhecimentoDeTransporteEletronico> ConsultarParaImportacaoDePreCTe(int codigoEmpresaPai, string nomeEmpresa, int numeroInicial, int numeroFinal, DateTime dataInicial, DateTime dataFinal, string status, Dominio.Enumeradores.TipoCTE finalidade, int inicioRegistros, int maximoRegistros)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.ConhecimentoDeTransporteEletronico>()
                                                    .Fetch(o => o.Destinatario)
                                                    .Fetch(o => o.Remetente)
                                                    .Fetch(o => o.MensagemStatus)
                                                    .Fetch(o => o.Serie)
                                                    .Fetch(o => o.Empresa);

            var result = from obj in query
                         where
                             obj.Empresa.EmpresaPai.Codigo == codigoEmpresaPai &&
                             obj.ImportacaoPreCTe == true &&
                             obj.TipoAmbiente == obj.Empresa.TipoAmbiente &&
                             obj.StatusImportacaoPreCTe == Dominio.Enumeradores.StatusImportacaoPreCTe.Pendente &&
                             (obj.ModeloDocumentoFiscal.Numero.Equals("57") || obj.ModeloDocumentoFiscal.Numero.Equals("67")) &&
                             obj.Status != "I" &&
                             obj.Status != "C"
                         select obj;

            if (!string.IsNullOrWhiteSpace(nomeEmpresa))
                result = result.Where(o => o.Empresa.NomeFantasia.Contains(nomeEmpresa) || o.Empresa.RazaoSocial.Contains(nomeEmpresa));

            if (numeroInicial > 0)
                result = result.Where(o => o.Numero >= numeroInicial);

            if (numeroFinal > 0)
                result = result.Where(o => o.Numero <= numeroFinal);

            if (dataInicial != DateTime.MinValue)
                result = result.Where(o => o.DataEmissao >= dataInicial);

            if (dataFinal != DateTime.MinValue)
                result = result.Where(o => o.DataEmissao < dataFinal.AddDays(1).Date);

            if (!string.IsNullOrWhiteSpace(status))
                result = result.Where(o => o.Status.Equals(status));

            if (finalidade != Dominio.Enumeradores.TipoCTE.Todos)
                result = result.Where(o => o.TipoCTE == finalidade);

            return result.OrderBy(o => o.DataEmissao).Skip(inicioRegistros).Take(maximoRegistros).ToList();
        }

        public int ContarConsultaParaImportacaoDePreCTe(int codigoEmpresaPai, string nomeEmpresa, int numeroInicial, int numeroFinal, DateTime dataInicial, DateTime dataFinal, string status, Dominio.Enumeradores.TipoCTE finalidade)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.ConhecimentoDeTransporteEletronico>();

            var result = from obj in query
                         where
                             obj.Empresa.EmpresaPai.Codigo == codigoEmpresaPai &&
                             obj.ImportacaoPreCTe == true &&
                             obj.TipoAmbiente == obj.Empresa.TipoAmbiente &&
                             obj.StatusImportacaoPreCTe == Dominio.Enumeradores.StatusImportacaoPreCTe.Pendente &&
                             (obj.ModeloDocumentoFiscal.Numero.Equals("57") || obj.ModeloDocumentoFiscal.Numero.Equals("67")) &&
                             obj.Status != "I" &&
                             obj.Status != "C"
                         select obj;

            if (!string.IsNullOrWhiteSpace(nomeEmpresa))
                result = result.Where(o => o.Empresa.NomeFantasia.Contains(nomeEmpresa) || o.Empresa.RazaoSocial.Contains(nomeEmpresa));

            if (numeroInicial > 0)
                result = result.Where(o => o.Numero >= numeroInicial);

            if (numeroFinal > 0)
                result = result.Where(o => o.Numero <= numeroFinal);

            if (dataInicial != DateTime.MinValue)
                result = result.Where(o => o.DataEmissao >= dataInicial);

            if (dataFinal != DateTime.MinValue)
                result = result.Where(o => o.DataEmissao < dataFinal.AddDays(1).Date);

            if (!string.IsNullOrWhiteSpace(status))
                result = result.Where(o => o.Status.Equals(status));

            if (finalidade != Dominio.Enumeradores.TipoCTE.Todos)
                result = result.Where(o => o.TipoCTE == finalidade);

            return result.Count();
        }

        public List<Dominio.Entidades.ConhecimentoDeTransporteEletronico> ConsultarPorTipo(int codigoEmpresa, DateTime dataEmissao, int numero, Dominio.Enumeradores.TipoCTE? tipo, Dominio.Enumeradores.TipoAmbiente tipoAmbiente, int[] series, string numeroDocumento, int inicioRegistros, int maximoRegistros, string status = "A")
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.ConhecimentoDeTransporteEletronico>();

            var result = from obj in query where obj.TipoAmbiente == tipoAmbiente && obj.Empresa.Codigo == codigoEmpresa && (obj.ModeloDocumentoFiscal.Numero.Equals("57") || obj.ModeloDocumentoFiscal.Numero.Equals("67")) select obj;

            if (!string.IsNullOrWhiteSpace(status))
                result = result.Where(o => o.Status.Equals(status));

            if (tipo.HasValue)
                result = result.Where(o => o.TipoCTE == tipo.Value);

            if (dataEmissao != DateTime.MinValue)
                result = result.Where(o => o.DataEmissao >= dataEmissao.Date && o.DataEmissao < dataEmissao.AddDays(1).Date);

            if (numero > 0)
                result = result.Where(o => o.Numero == numero);

            if (series.Count() > 0)
                result = result.Where(o => series.Contains(o.Serie.Codigo));

            if (!string.IsNullOrWhiteSpace(numeroDocumento))
                result = result.Where(o => (from obj in o.Documentos where obj.Numero.Equals(numeroDocumento) select obj.CTE.Codigo).Contains(o.Codigo));

            return result.OrderByDescending(o => o.Numero).Skip(inicioRegistros).Take(maximoRegistros).ToList();
        }

        public int ContarConsultaPorTipo(int codigoEmpresa, DateTime dataEmissao, int numero, Dominio.Enumeradores.TipoCTE? tipo, Dominio.Enumeradores.TipoAmbiente tipoAmbiente, int[] series, string numeroDocumento)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.ConhecimentoDeTransporteEletronico>();

            var result = from obj in query where obj.TipoAmbiente == tipoAmbiente && obj.Empresa.Codigo == codigoEmpresa && obj.Status.Equals("A") && (obj.ModeloDocumentoFiscal.Numero.Equals("57") || obj.ModeloDocumentoFiscal.Numero.Equals("67")) select obj;

            if (tipo.HasValue)
                result = result.Where(o => o.TipoCTE == tipo.Value);

            if (dataEmissao != DateTime.MinValue)
                result = result.Where(o => o.DataEmissao >= dataEmissao.Date && o.DataEmissao < dataEmissao.AddDays(1).Date);

            if (numero > 0)
                result = result.Where(o => o.Numero == numero);

            if (series.Count() > 0)
                result = result.Where(o => series.Contains(o.Serie.Codigo));

            if (!string.IsNullOrWhiteSpace(numeroDocumento))
                result = result.Where(o => (from obj in o.Documentos where obj.Numero.Equals(numeroDocumento) select obj.CTE.Codigo).Contains(o.Codigo));

            return result.Count();
        }

        public List<Dominio.Entidades.ConhecimentoDeTransporteEletronico> ConsultarCtesSemDuplicata(int codigoEmpresa, DateTime dataEmissaoInicial, DateTime dataEmissaoFinal, int numeroInicial, int numeroFinal, string cnpjTomador, string cnpjRemetente, string cnpjDestinatario, Dominio.Enumeradores.TipoAmbiente tipoAmbiente, int[] series, string numeroDocumento, bool filtrarTodosCTes, int inicioRegistros, int maximoRegistros)
        {
            var result = _ConsultarCtesSemDuplicata(codigoEmpresa, dataEmissaoInicial, dataEmissaoFinal, numeroInicial, numeroFinal, cnpjTomador, cnpjRemetente, cnpjDestinatario, tipoAmbiente, series, numeroDocumento, filtrarTodosCTes);

            result = result.OrderByDescending(o => o.Numero);

            if (inicioRegistros > 0)
                result = result.Skip(inicioRegistros);

            if (maximoRegistros > 0)
                result = result.Take(maximoRegistros);

            return result.Timeout(120).ToList();
        }

        public int ContarCtesSemDuplicata(int codigoEmpresa, DateTime dataEmissaoInicial, DateTime dataEmissaoFinal, int numeroInicial, int numeroFinal, string cnpjTomador, string cnpjDestinatario, string cnpjRemetente, Dominio.Enumeradores.TipoAmbiente tipoAmbiente, int[] series, string numeroDocumento, bool filtrarTodosCTes)
        {
            var result = _ConsultarCtesSemDuplicata(codigoEmpresa, dataEmissaoInicial, dataEmissaoFinal, numeroInicial, numeroFinal, cnpjTomador, cnpjRemetente, cnpjDestinatario, tipoAmbiente, series, numeroDocumento, filtrarTodosCTes);

            return result.Count();
        }

        public int ContarPorChaveDoCTeOriginalETipo(int codigoEmpresa, string chaveCTeOriginal, Dominio.Enumeradores.TipoCTE tipoCTe)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.ConhecimentoDeTransporteEletronico>();
            var result = from obj in query where obj.ChaveCTESubComp.Equals(chaveCTeOriginal) && obj.TipoCTE == tipoCTe && obj.Empresa.Codigo == codigoEmpresa && obj.Status.Equals("A") && (obj.ModeloDocumentoFiscal.Numero.Equals("57") || obj.ModeloDocumentoFiscal.Numero.Equals("67")) select obj.Codigo;
            return result.Count();
        }

        public Dominio.Entidades.ConhecimentoDeTransporteEletronico BuscarPorChave(string chaveCTe, int codigoEmpresa)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.ConhecimentoDeTransporteEletronico>();
            var result = from obj in query where obj.Chave.Equals(chaveCTe) && obj.Empresa.Codigo == codigoEmpresa select obj;
            return result.FirstOrDefault();
        }

        public Dominio.Entidades.ConhecimentoDeTransporteEletronico BuscarPorChaveNFe(string chaveNFe)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.ConhecimentoDeTransporteEletronico>();
            var result = from obj in query
                         where (obj.ModeloDocumentoFiscal.Numero.Equals("57") || obj.ModeloDocumentoFiscal.Numero.Equals("67"))
                         select obj;

            if (!string.IsNullOrWhiteSpace(chaveNFe))
            {
                var queryDocumentos = this.SessionNHiBernate.Query<Dominio.Entidades.DocumentosCTE>()
                    .Where(o => o.ChaveNFE == chaveNFe);

                result = result.Where(o => queryDocumentos.Where(obj => obj.CTE.Codigo == o.Codigo).Any());
            }

            return result.OrderByDescending(o => o.Codigo).WithOptions(o => { o.SetTimeout(120); }).FirstOrDefault();
        }

        public List<Dominio.Entidades.ConhecimentoDeTransporteEletronico> BuscarListaPorChaveNFe(string chaveNFe)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.ConhecimentoDeTransporteEletronico>();
            var result = from obj in query
                         where (obj.ModeloDocumentoFiscal.Numero.Equals("57") || obj.ModeloDocumentoFiscal.Numero.Equals("67"))
                         select obj;

            if (!string.IsNullOrWhiteSpace(chaveNFe))
            {
                var queryDocumentos = this.SessionNHiBernate.Query<Dominio.Entidades.DocumentosCTE>()
                    .Where(o => o.ChaveNFE == chaveNFe);

                result = result.Where(o => queryDocumentos.Where(obj => obj.CTE.Codigo == o.Codigo).Any());
            }

            return result.OrderByDescending(o => o.Codigo).WithOptions(o => { o.SetTimeout(120); }).ToList();
        }

        public Task<List<Dominio.Entidades.ConhecimentoDeTransporteEletronico>> BuscarListaPorChaveNFeAsync(string chaveNFe)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.ConhecimentoDeTransporteEletronico>()
                .Where(obj => obj.ModeloDocumentoFiscal.Numero.Equals("57") || obj.ModeloDocumentoFiscal.Numero.Equals("67"));

            if (!string.IsNullOrWhiteSpace(chaveNFe))
            {
                var queryDocumentos = this.SessionNHiBernate.Query<Dominio.Entidades.DocumentosCTE>()
                    .Where(o => o.ChaveNFE == chaveNFe);

                query = query.Where(o => queryDocumentos.Where(obj => obj.CTE.Codigo == o.Codigo).Any());
            }

            return query.OrderByDescending(o => o.Codigo).WithOptions(o => { o.SetTimeout(120); }).ToListAsync(CancellationToken);
        }

        public Dominio.Entidades.ConhecimentoDeTransporteEletronico BuscarPorChave(string chaveCTe)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.ConhecimentoDeTransporteEletronico>();
            var result = from obj in query where obj.Chave.Equals(chaveCTe) select obj;
            return result.FirstOrDefault();
        }

        public Dominio.Entidades.ConhecimentoDeTransporteEletronico BuscarPorChaveComFetchEmpresa(string chaveCTe)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.ConhecimentoDeTransporteEletronico>();
            var result = from obj in query where obj.Chave.Equals(chaveCTe) select obj;
            return result.Fetch(o => o.Empresa).FirstOrDefault();
        }

        public Dominio.Entidades.ConhecimentoDeTransporteEletronico BuscarPorChaveComFetch(string chaveCTe)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.ConhecimentoDeTransporteEletronico>();
            var result = from obj in query where obj.Chave.Equals(chaveCTe) select obj;
            return result
                .Fetch(obj => obj.Remetente)
                .ThenFetch(obj => obj.Cliente)
                .ThenFetch(obj => obj.Localidade)
                .Fetch(obj => obj.Expedidor)
                .ThenFetch(obj => obj.Cliente)
                .ThenFetch(obj => obj.Localidade)
                .Fetch(obj => obj.Destinatario)
                .ThenFetch(obj => obj.Cliente)
                .ThenFetch(obj => obj.Localidade)
                .Fetch(obj => obj.Recebedor)
                .ThenFetch(obj => obj.Cliente)
                .ThenFetch(obj => obj.Localidade)
                .Fetch(obj => obj.OutrosTomador)
                .ThenFetch(obj => obj.Cliente)
                .ThenFetch(obj => obj.Localidade)
                .Fetch(obj => obj.ModeloDocumentoFiscal)
                .Fetch(obj => obj.Serie)
                .Fetch(obj => obj.LocalidadeInicioPrestacao)
                .Fetch(obj => obj.LocalidadeTerminoPrestacao)
                .FirstOrDefault();
        }

        public int BuscarCodigoPorChave(string chaveCTe)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.ConhecimentoDeTransporteEletronico>();
            var result = from obj in query where obj.Chave.Equals(chaveCTe) select obj;
            return result.Select(o => o.Codigo).Timeout(30).FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.Produtos.ProdutoEmbarcador BuscarProdutoEmbarcador(int codigoCTe)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCTe>();
            query = query.Where(o => o.CTe.Codigo == codigoCTe);

            var queryPedido = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedido>();
            queryPedido = queryPedido.Where(o => query.Any(c => c.Carga == o.Carga));

            var queryProduto = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoProduto>();
            queryProduto = queryProduto.Where(o => queryPedido.Any(c => c.Pedido == o.Pedido));

            return queryProduto.Select(p => p.Produto)?.FirstOrDefault() ?? null;
        }

        public Dominio.ObjetosDeValor.Embarcador.Produtos.ProdutoEmbarcador BuscarDadosProdutoEmbarcador(int codigoCTe)
        {
            string sql = @"
        select top(1) Produto.PRO_CODIGO Codigo,
               Produto.PRO_CODIGO_PRODUTO_EMBARCADOR CodigoProdutoEmbarcador,
               Produto.PRO_CODIGO_DOCUMENTACAO CodigoDocumentacao,
               Produto.GRP_DESCRICAO Descricao,
               Produto.GRP_OBSERVACAO Observacao,
               Produto.PRO_CODIGO_CEAN CodigoCEAN,
               Produto.GRP_CODIGO GrupoProduto,
               Produto.PRO_ATIVO Ativo,
               Produto.PRO_INTEGRADO Integrado,
               Produto.PRO_TIPO_PESSOA TipoPessoa,
               Produto.GRP_CODIGO_PESSOA GrupoPessoas,
               Produto.CPF_CNPJ Cliente,
               Produto.TCG_CODIGO TipoDeCarga,
               Produto.PRO_TEMPERATURA_TRANSPORTE TemperaturaTransporte,
               Produto.PRO_PESO_UNITARIO PesoUnitario,
               Produto.PRO_FATOR_CONVERSAO FatorConversao,
               Produto.PRO_PESO_LIQUIDO_UNITARIO PesoLiquidoUnitario,
               Produto.PRO_SIGLA_UNIDADE SiglaUnidade,
               Produto.PRO_EXIBIR_EXPEDICAO_TEMPO_REAL ExibirExpedicaoEmTempoReal,
               Produto.PRO_DESCONTAR_PESO_PRODUTO_CALCULO_FRETE DescontarPesoProdutoCalculoFrete,
               Produto.PRO_DESCONTAR_VALOR_PRODUTO_CALCULO_FRETE DescontarValorProdutoCalculoFrete,
               Produto.PRO_OBRIGATORIO_GTA ObrigatorioGuiaTransporteAnimal,
               Produto.PRO_OBRIGATORIO_NF_PRODUTOR ObrigatorioNFProdutor,
               Produto.PRO_QUANTIDADE_CAIXA QuantidadeCaixa,
               Produto.PRO_QUANTIDADE_CAIXA_POR_PALLET QuantidadeCaixaPorPallet,
               Produto.PRO_QUANTIDADE_PALET QtdPalet,
               Produto.PRO_ALTURA_CM AlturaCM,
               Produto.PRO_LARGURA_CM LarguraCM,
               Produto.PRO_COMPRIMENTO_CM ComprimentoCM,
               Produto.PRO_METRO_CUBICO MetroCubito,
               Produto.PRO_POSSUI_INTEGRACAO_COLETA_MOBILE PossuiIntegracaoColetaMobile,
               Produto.PRO_OBRIGATORIO_INFORMAR_TEMPERATURA ObrigatorioInformarTemperatura,
               Produto.PRO_CODIGO_NCM CodigoNCM,
               Produto.PRO_EXIGE_INFORMAR_CAIXAS ExigeInformarCaixas,
               Produto.PRO_EXIGE_INFORMAR_IMUNOS ExigeInformarImunos,
               Produto.PRO_CODIGO_EAN CodigoEAN,
               Cte.CON_CHAVECTE ChaveNF
          from T_CTE Cte
          left join T_CARGA_CTE CargaCte on CargaCte.CON_CODIGO  = Cte.CON_CODIGO
          left join T_CARGA_PEDIDO_XML_NOTA_FISCAL_CTE NotaFiscalCte on NotaFiscalCte.CCT_CODIGO = CargaCte.CCT_CODIGO
          left join T_PEDIDO_XML_NOTA_FISCAL NotaFiscalPedido on NotaFiscalPedido.PNF_CODIGO = NotaFiscalCte.PNF_CODIGO
          left join T_CARGA_PEDIDO CargaPedido on CargaPedido.CPE_CODIGO = NotaFiscalPedido.CPE_CODIGO
          left join T_PEDIDO_PRODUTO PedidoProduto on PedidoProduto.PED_CODIGO = CargaPedido.PED_CODIGO
          left join T_PRODUTO_EMBARCADOR Produto on Produto.PRO_CODIGO = PedidoProduto.PRO_CODIGO 
         where Cte.CON_CODIGO = :codigoCTe";

            var consulta = this.SessionNHiBernate.CreateSQLQuery(sql);
            consulta.SetInt32("codigoCTe", codigoCTe);
            consulta.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Dominio.ObjetosDeValor.Embarcador.Produtos.ProdutoEmbarcador)));

            return consulta.SetTimeout(300).UniqueResult<Dominio.ObjetosDeValor.Embarcador.Produtos.ProdutoEmbarcador>();
        }


        public List<Dominio.ObjetosDeValor.Embarcador.Produtos.ProdutoEmbarcador> BuscarDadosProdutoEmbarcador(List<string> chaves)
        {
            var listaProdutoEmbarcadorRetornar = new List<Dominio.ObjetosDeValor.Embarcador.Produtos.ProdutoEmbarcador>();
            int quantidadeRegistrosConsultarPorVez = 2000;
            int quantidadeConsultas = chaves.Count / quantidadeRegistrosConsultarPorVez;

            for (int i = 0; i <= quantidadeConsultas; i++)
            {
                var chavesLote = chaves.Skip(i * quantidadeRegistrosConsultarPorVez).Take(quantidadeRegistrosConsultarPorVez).ToList();

                string sql = @"
            select distinct Produto.PRO_CODIGO Codigo,
                   Produto.PRO_CODIGO_PRODUTO_EMBARCADOR CodigoProdutoEmbarcador,
                   Produto.PRO_CODIGO_DOCUMENTACAO CodigoDocumentacao,
                   Produto.GRP_DESCRICAO Descricao,
                   Produto.GRP_OBSERVACAO Observacao,
                   Produto.PRO_CODIGO_CEAN CodigoCEAN,
                   Produto.GRP_CODIGO GrupoProduto,
                   Produto.PRO_ATIVO Ativo,
                   Produto.PRO_INTEGRADO Integrado,
                   Produto.PRO_TIPO_PESSOA TipoPessoa,
                   Produto.GRP_CODIGO_PESSOA GrupoPessoas,
                   Produto.CPF_CNPJ Cliente,
                   Produto.TCG_CODIGO TipoDeCarga,
                   Produto.PRO_TEMPERATURA_TRANSPORTE TemperaturaTransporte,
                   Produto.PRO_PESO_UNITARIO PesoUnitario,
                   Produto.PRO_FATOR_CONVERSAO FatorConversao,
                   Produto.PRO_PESO_LIQUIDO_UNITARIO PesoLiquidoUnitario,
                   Produto.PRO_SIGLA_UNIDADE SiglaUnidade,
                   Produto.PRO_EXIBIR_EXPEDICAO_TEMPO_REAL ExibirExpedicaoEmTempoReal,
                   Produto.PRO_DESCONTAR_PESO_PRODUTO_CALCULO_FRETE DescontarPesoProdutoCalculoFrete,
                   Produto.PRO_DESCONTAR_VALOR_PRODUTO_CALCULO_FRETE DescontarValorProdutoCalculoFrete,
                   Produto.PRO_OBRIGATORIO_GTA ObrigatorioGuiaTransporteAnimal,
                   Produto.PRO_OBRIGATORIO_NF_PRODUTOR ObrigatorioNFProdutor,
                   Produto.PRO_QUANTIDADE_CAIXA QuantidadeCaixa,
                   Produto.PRO_QUANTIDADE_CAIXA_POR_PALLET QuantidadeCaixaPorPallet,
                   Produto.PRO_QUANTIDADE_PALET QtdPalet,
                   Produto.PRO_ALTURA_CM AlturaCM,
                   Produto.PRO_LARGURA_CM LarguraCM,
                   Produto.PRO_COMPRIMENTO_CM ComprimentoCM,
                   Produto.PRO_METRO_CUBICO MetroCubito,
                   Produto.PRO_POSSUI_INTEGRACAO_COLETA_MOBILE PossuiIntegracaoColetaMobile,
                   Produto.PRO_OBRIGATORIO_INFORMAR_TEMPERATURA ObrigatorioInformarTemperatura,
                   Produto.PRO_CODIGO_NCM CodigoNCM,
                   Produto.PRO_EXIGE_INFORMAR_CAIXAS ExigeInformarCaixas,
                   Produto.PRO_EXIGE_INFORMAR_IMUNOS ExigeInformarImunos,
                   Produto.PRO_CODIGO_EAN CodigoEAN,
                   Cte.CON_CHAVECTE ChaveNF
              from T_CTE Cte
              left join T_CARGA_CTE CargaCte on CargaCte.CON_CODIGO  = Cte.CON_CODIGO
              left join T_CARGA_PEDIDO_XML_NOTA_FISCAL_CTE NotaFiscalCte on NotaFiscalCte.CCT_CODIGO = CargaCte.CCT_CODIGO
              left join T_PEDIDO_XML_NOTA_FISCAL NotaFiscalPedido on NotaFiscalPedido.PNF_CODIGO = NotaFiscalCte.PNF_CODIGO
              left join T_CARGA_PEDIDO CargaPedido on CargaPedido.CPE_CODIGO = NotaFiscalPedido.CPE_CODIGO
              left join T_PEDIDO_PRODUTO PedidoProduto on PedidoProduto.PED_CODIGO = CargaPedido.PED_CODIGO
              left join T_PRODUTO_EMBARCADOR Produto on Produto.PRO_CODIGO = PedidoProduto.PRO_CODIGO 
             where Cte.CON_CHAVECTE in (:chavesLote)";

                var consulta = this.SessionNHiBernate.CreateSQLQuery(sql);
                consulta.SetParameterList("chavesLote", chavesLote);
                consulta.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Dominio.ObjetosDeValor.Embarcador.Produtos.ProdutoEmbarcador)));

                listaProdutoEmbarcadorRetornar.AddRange(consulta.SetTimeout(600).List<Dominio.ObjetosDeValor.Embarcador.Produtos.ProdutoEmbarcador>());
            }

            return listaProdutoEmbarcadorRetornar;
        }

        public string BuscarChavePorTipoEChaveCTeOriginal(int codigoEmpresa, Dominio.Enumeradores.TipoCTE tipoCTe, string chaveCTe)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.ConhecimentoDeTransporteEletronico>();
            var result = from obj in query where obj.ChaveCTESubComp.Equals(chaveCTe) && obj.Empresa.Codigo == codigoEmpresa && obj.TipoCTE == tipoCTe select obj.Chave;
            return result.FirstOrDefault();
        }

        public Dominio.Entidades.ConhecimentoDeTransporteEletronico BuscarPorNFePlaca(int codigoEmpresa, Dominio.Enumeradores.TipoAmbiente tipoAmbiente, string cnpjRemetente, string cnpjDestinatario, string chaveNFe, string placa, DateTime dataEmissao, string[] status)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.ConhecimentoDeTransporteEletronico>();
            var result = from obj in query
                         where obj.TipoAmbiente == tipoAmbiente &&
                              obj.Empresa.Codigo == codigoEmpresa &&
                              status.Contains(obj.Status) &&
                              (obj.ModeloDocumentoFiscal.Numero.Equals("57") || obj.ModeloDocumentoFiscal.Numero.Equals("67"))
                         select obj;

            if (dataEmissao > DateTime.MinValue)
                result = result.Where(o => o.DataEmissao >= dataEmissao && o.DataEmissao <= dataEmissao.AddDays(1).Date);

            if (!string.IsNullOrWhiteSpace(cnpjRemetente))
                result = result.Where(o => o.Remetente.CPF_CNPJ.Equals(cnpjRemetente));

            if (!string.IsNullOrWhiteSpace(cnpjDestinatario))
                result = result.Where(o => o.Destinatario.CPF_CNPJ.Equals(cnpjDestinatario));

            if (!string.IsNullOrWhiteSpace(chaveNFe))
            {
                var queryDocumentos = this.SessionNHiBernate.Query<Dominio.Entidades.DocumentosCTE>();

                result = result.Where(o => (from obj in queryDocumentos where obj.CTE.Codigo == o.Codigo && obj.ChaveNFE == chaveNFe select obj.CTE.Codigo).Contains(o.Codigo));
            }

            if (!string.IsNullOrWhiteSpace(placa))
            {
                var queryVeiculos = this.SessionNHiBernate.Query<Dominio.Entidades.VeiculoCTE>();

                result = result.Where(o => (from obj in queryVeiculos where obj.CTE.Codigo == o.Codigo && obj.Placa.Equals(placa) select obj.CTE.Codigo).Contains(o.Codigo));
            }

            return result.OrderByDescending(o => o.Codigo).Timeout(120).FirstOrDefault();
        }

        public Dominio.Entidades.ConhecimentoDeTransporteEletronico BuscarPorNFeStatus(int codigoEmpresa, Dominio.Enumeradores.TipoAmbiente tipoAmbiente, DateTime dataEmissao, string chaveNFe, string status)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.ConhecimentoDeTransporteEletronico>();
            var result = from obj in query
                         where obj.TipoAmbiente == tipoAmbiente &&
                              obj.Empresa.Codigo == codigoEmpresa &&
                              (obj.ModeloDocumentoFiscal.Numero.Equals("57") || obj.ModeloDocumentoFiscal.Numero.Equals("67"))
                         select obj;

            if (!string.IsNullOrWhiteSpace(status))
                result = result.Where(o => o.Status.Equals(status));

            if (dataEmissao > DateTime.MinValue)
                result = result.Where(o => o.DataEmissao >= dataEmissao && o.DataEmissao <= dataEmissao.AddDays(1).Date);

            if (!string.IsNullOrWhiteSpace(chaveNFe))
            {
                var queryDocumentos = this.SessionNHiBernate.Query<Dominio.Entidades.DocumentosCTE>();

                result = result.Where(o => (from obj in queryDocumentos where obj.CTE.Codigo == o.Codigo && obj.ChaveNFE == chaveNFe select obj.CTE.Codigo).Contains(o.Codigo));
            }


            return result.OrderByDescending(o => o.Codigo).Timeout(120).FirstOrDefault();
        }

        public Dominio.Entidades.ConhecimentoDeTransporteEletronico BuscarPorDestinatario(int codigoEmpresa, Dominio.Enumeradores.TipoAmbiente tipoAmbiente, string remetente, string destinatario, DateTime dataEmissao, string[] status)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.ConhecimentoDeTransporteEletronico>();
            var result = from obj in query
                         where obj.TipoAmbiente == tipoAmbiente &&
                              obj.Empresa.Codigo == codigoEmpresa &&
                              status.Contains(obj.Status) &&
                              (obj.ModeloDocumentoFiscal.Numero.Equals("57") || obj.ModeloDocumentoFiscal.Numero.Equals("67"))
                         select obj;

            if (dataEmissao > DateTime.MinValue)
                result = result.Where(o => o.DataEmissao >= dataEmissao && o.DataEmissao <= dataEmissao.AddDays(1).Date);

            if (!string.IsNullOrWhiteSpace(remetente))
                result = result.Where(o => o.Remetente.CPF_CNPJ.Equals(remetente));

            if (!string.IsNullOrWhiteSpace(destinatario))
                result = result.Where(o => o.Destinatario.CPF_CNPJ.Equals(destinatario));

            return result.Timeout(120).FirstOrDefault();
        }

        public Dominio.Entidades.ConhecimentoDeTransporteEletronico BuscarPorObservacao(int codigoEmpresa, Dominio.Enumeradores.TipoAmbiente tipoAmbiente, string observacao, DateTime dataEmissao, string[] status)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.ConhecimentoDeTransporteEletronico>();
            var result = from obj in query
                         where obj.TipoAmbiente == tipoAmbiente &&
                              obj.Empresa.Codigo == codigoEmpresa &&
                              status.Contains(obj.Status) &&
                              (obj.ModeloDocumentoFiscal.Numero.Equals("57") || obj.ModeloDocumentoFiscal.Numero.Equals("67"))
                         select obj;

            if (dataEmissao > DateTime.MinValue)
                result = result.Where(o => o.DataEmissao >= dataEmissao && o.DataEmissao <= dataEmissao.AddDays(1).Date);

            if (!string.IsNullOrWhiteSpace(observacao))
                result = result.Where(o => o.ObservacoesGerais.Contains(observacao));

            return result.Timeout(120).FirstOrDefault();
        }

        public Dominio.Entidades.ConhecimentoDeTransporteEletronico BuscarUltimoCTeEmitido(int codigoEmpresa, int[] series = null)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.ConhecimentoDeTransporteEletronico>();
            var result = from obj in query where obj.Empresa.Codigo == codigoEmpresa select obj;
            if (series != null && series.Count() > 0)
                result = result.Where(o => series.Contains(o.Serie.Codigo));
            return result.OrderByDescending(o => o.Codigo).FirstOrDefault();
        }

        public Dominio.Entidades.EmpresaSerie BuscarSerieUltimoCTeEmitido(int codigoEmpresa, int[] series = null)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.ConhecimentoDeTransporteEletronico>();
            var result = from obj in query where obj.Empresa.Codigo == codigoEmpresa && obj.Status == "A" && (obj.ModeloDocumentoFiscal.Numero.Equals("57") || obj.ModeloDocumentoFiscal.Numero.Equals("67")) select obj.Serie;
            if (series != null && series.Count() > 0)
                result = result.Where(o => series.Contains(o.Codigo));
            return result.OrderByDescending(o => o.Codigo).FirstOrDefault();
        }

        public Dominio.Entidades.EmpresaSerie BuscarSerieUltimoCTeEmitidoNFSe(int codigoEmpresa, int[] series = null)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.ConhecimentoDeTransporteEletronico>();
            var result = from obj in query where obj.Empresa.Codigo == codigoEmpresa && obj.Status == "A" && obj.ModeloDocumentoFiscal.Numero == "36" select obj.Serie;
            if (series != null && series.Count() > 0)
                result = result.Where(o => series.Contains(o.Codigo));
            return result.OrderByDescending(o => o.Codigo).FirstOrDefault();
        }

        public Dominio.Entidades.ConhecimentoDeTransporteEletronico BuscarPorRemetenteEDestinatario(int codigoEmpresa, Dominio.Entidades.Cliente remetente, Dominio.Entidades.Cliente destinatario, Dominio.Enumeradores.TipoAmbiente tipoAmbiente)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.ConhecimentoDeTransporteEletronico>();

            DateTime dataInicio = DateTime.Today.AddMonths(-3);

            var result = from obj in query
                         where obj.TipoAmbiente == tipoAmbiente &&
                               obj.Empresa.Codigo == codigoEmpresa &&
                               obj.Remetente.CPF_CNPJ.Equals(remetente.CPF_CNPJ_SemFormato) &&
                               obj.Destinatario.CPF_CNPJ.Equals(destinatario.CPF_CNPJ_SemFormato) &&
                               obj.Status.Equals("A") &&
                               (obj.ModeloDocumentoFiscal.Numero.Equals("57") || obj.ModeloDocumentoFiscal.Numero.Equals("67")) &&
                               obj.DataEmissao >= dataInicio
                         orderby obj.DataEmissao descending
                         select obj;

            return result
                   .Fetch(o => o.Empresa)
                   .Fetch(o => o.Remetente)
                   .Fetch(o => o.Destinatario)
                   .Timeout(60)
                   .FirstOrDefault();
        }

        public Dominio.Entidades.ConhecimentoDeTransporteEletronico BuscarPorUFInicioPrestacaoEUFFimPrestacao(int codigoEmpresa, Dominio.Entidades.Estado ufInicioPrestacao, Dominio.Entidades.Estado ufFimPrestacao, Dominio.Enumeradores.TipoAmbiente tipoAmbiente)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.ConhecimentoDeTransporteEletronico>();

            DateTime dataInicio = DateTime.Today.AddMonths(-6);

            var result = from obj in query
                         where obj.LocalidadeInicioPrestacao.Estado.Sigla.Equals(ufInicioPrestacao.Sigla) &&
                               obj.LocalidadeTerminoPrestacao.Estado.Sigla.Equals(ufFimPrestacao.Sigla) &&
                               obj.TipoAmbiente == tipoAmbiente &&
                               obj.Empresa.Codigo == codigoEmpresa &&
                               obj.Status.Equals("A") &&
                               (obj.ModeloDocumentoFiscal.Numero.Equals("57") || obj.ModeloDocumentoFiscal.Numero.Equals("67")) &&
                               obj.DataEmissao >= dataInicio
                         orderby obj.DataEmissao descending
                         select obj;

            return result
                   .Fetch(o => o.Empresa)
                   .Fetch(o => o.LocalidadeInicioPrestacao)
                   .Fetch(o => o.LocalidadeTerminoPrestacao)
                   .Timeout(60)
                   .FirstOrDefault();
        }

        public List<Dominio.Entidades.ConhecimentoDeTransporteEletronico> Consultar(int codigoEmpresa, DateTime dataEmissaoInicial, DateTime dataEmissaoFinal, string status, Dominio.Enumeradores.TipoCTE tipoCTe, int codigoSerie, string cpfCnpjRemetente, string cpfCnpjExpedidor, string cpfCnpjRecebedor, string cpfCnpjDestinatario, string cpfCnpjTomador, int numeroInicial, int numeroFinal, string nomeMotorista, string cpfMotorista, string placaVeiculo, Dominio.Enumeradores.TipoAmbiente tipoAmbiente, int[] series, string tipoOcorrencia, string numeroNotaFiscal, Dominio.Enumeradores.StatusDuplicata? statusPagamento, int codigoLocalidadeInicioPrestacao, int codigoLocalidadeTerminoPrestacao, int inicioRegistros, int maximoRegistros, string ufInicio, string ufFim, string duplicata, bool importacao, bool exportacao, bool raizCNPJRemetente, bool raizCNPJExpedidor, bool raizCNPJRecebedor, bool raizCNPJDestinatario, bool raizCNPJTomador, string icmsCTe, Dominio.Enumeradores.TipoServico? tipoServico, string cstCTe, int codigoUsuario, string nomeUsuario, DateTime dataAutorizacaoInicial, DateTime dataAutorizacaoFinal, string observacao, bool removerCliente, int numeroCarga, int numeroUnidade, Dominio.Enumeradores.FiltroAverbacaoCTe? averbacaoCTe)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.ConhecimentoDeTransporteEletronico>();

            var result = from obj in query where obj.Empresa.Codigo == codigoEmpresa && obj.TipoAmbiente == tipoAmbiente select obj;

            result = result.Where(o => (o.ModeloDocumentoFiscal.Numero.Equals("57") || o.ModeloDocumentoFiscal.Numero.Equals("67")));

            if (dataEmissaoInicial != DateTime.MinValue)
                result = result.Where(o => o.DataEmissao >= dataEmissaoInicial.Date);

            if (dataEmissaoFinal != DateTime.MinValue)
                result = result.Where(o => o.DataEmissao < dataEmissaoFinal.AddDays(1).Date);

            if (dataAutorizacaoInicial != DateTime.MinValue)
                result = result.Where(o => o.DataRetornoSefaz >= dataAutorizacaoInicial.Date);

            if (dataAutorizacaoFinal != DateTime.MinValue)
                result = result.Where(o => o.DataRetornoSefaz < dataAutorizacaoFinal.AddDays(1).Date);

            if (!string.IsNullOrWhiteSpace(status))
                result = result.Where(o => o.Status.Equals(status));

            if (tipoCTe != Dominio.Enumeradores.TipoCTE.Todos)
                result = result.Where(o => o.TipoCTE == tipoCTe);

            if (codigoUsuario > 0 && !string.IsNullOrWhiteSpace(nomeUsuario))
                result = result.Where(o => o.Usuario.Codigo == codigoUsuario); //|| o.Log.Contains(nomeUsuario)

            if (!string.IsNullOrWhiteSpace(cpfCnpjRemetente))
            {
                if (removerCliente)
                {
                    if (raizCNPJRemetente)
                        result = result.Where(o => !o.Remetente.CPF_CNPJ.Contains(cpfCnpjRemetente.Substring(0, 8)));
                    else
                        result = result.Where(o => o.Remetente.CPF_CNPJ != cpfCnpjRemetente);
                }
                else
                {
                    if (raizCNPJRemetente)
                        result = result.Where(o => o.Remetente.CPF_CNPJ.Contains(cpfCnpjRemetente.Substring(0, 8)));
                    else
                        result = result.Where(o => o.Remetente.CPF_CNPJ == cpfCnpjRemetente);
                }
            }

            if (importacao)
                result = result.Where(o => o.Remetente.Exterior == true);

            if (!string.IsNullOrWhiteSpace(cpfCnpjDestinatario))
            {
                if (removerCliente)
                {
                    if (raizCNPJDestinatario)
                        result = result.Where(o => !o.Destinatario.CPF_CNPJ.Contains(cpfCnpjDestinatario.Substring(0, 8)));
                    else
                        result = result.Where(o => o.Destinatario.CPF_CNPJ != cpfCnpjDestinatario);
                }
                else
                {
                    if (raizCNPJDestinatario)
                        result = result.Where(o => o.Destinatario.CPF_CNPJ.Contains(cpfCnpjDestinatario.Substring(0, 8)));
                    else
                        result = result.Where(o => o.Destinatario.CPF_CNPJ == cpfCnpjDestinatario);
                }
            }

            if (exportacao)
                result = result.Where(o => o.Destinatario.Exterior == true);

            if (!string.IsNullOrWhiteSpace(cpfCnpjExpedidor))
            {
                if (removerCliente)
                {
                    if (raizCNPJExpedidor)
                        result = result.Where(o => !o.Expedidor.CPF_CNPJ.Contains(cpfCnpjExpedidor.Substring(0, 8)));
                    else
                        result = result.Where(o => o.Expedidor.CPF_CNPJ != cpfCnpjExpedidor);
                }
                else
                {
                    if (raizCNPJExpedidor)
                        result = result.Where(o => o.Expedidor.CPF_CNPJ.Contains(cpfCnpjExpedidor.Substring(0, 8)));
                    else
                        result = result.Where(o => o.Expedidor.CPF_CNPJ == cpfCnpjExpedidor);
                }
            }

            if (!string.IsNullOrWhiteSpace(cpfCnpjRecebedor))
            {
                if (removerCliente)
                {
                    if (raizCNPJRecebedor)
                        result = result.Where(o => !o.Recebedor.CPF_CNPJ.Contains(cpfCnpjRecebedor.Substring(0, 8)));
                    else
                        result = result.Where(o => o.Recebedor.CPF_CNPJ != cpfCnpjRecebedor);
                }
                else
                {
                    if (raizCNPJRecebedor)
                        result = result.Where(o => o.Recebedor.CPF_CNPJ.Contains(cpfCnpjRecebedor.Substring(0, 8)));
                    else
                        result = result.Where(o => o.Recebedor.CPF_CNPJ == cpfCnpjRecebedor);
                }
            }

            if (codigoLocalidadeInicioPrestacao > 0)
                result = result.Where(o => o.LocalidadeInicioPrestacao.Codigo == codigoLocalidadeInicioPrestacao);

            if (codigoLocalidadeTerminoPrestacao > 0)
                result = result.Where(o => o.LocalidadeTerminoPrestacao.Codigo == codigoLocalidadeTerminoPrestacao);

            if (codigoUsuario > 0 && !string.IsNullOrWhiteSpace(nomeUsuario))
                result = result.Where(o => (o.Usuario.Codigo == codigoUsuario)); // || o.Log.Contains(nomeUsuario)

            if (!string.IsNullOrWhiteSpace(ufInicio))
                result = result.Where(o => o.LocalidadeInicioPrestacao.Estado.Sigla == ufInicio);

            if (!string.IsNullOrWhiteSpace(ufFim))
                result = result.Where(o => o.LocalidadeTerminoPrestacao.Estado.Sigla == ufFim);

            if (!string.IsNullOrWhiteSpace(cpfCnpjTomador))
            {
                //if (raizCNPJTomador)
                //    result = result.Where(o => (o.TipoTomador == Dominio.Enumeradores.TipoTomador.Destinatario && o.Destinatario.CPF_CNPJ.Contains(cpfCnpjTomador.Substring(0, 8))) ||
                //                           (o.TipoTomador == Dominio.Enumeradores.TipoTomador.Expedidor && o.Expedidor.CPF_CNPJ.Contains(cpfCnpjTomador.Substring(0, 8))) ||
                //                           (o.TipoTomador == Dominio.Enumeradores.TipoTomador.Outros && o.OutrosTomador.CPF_CNPJ.Contains(cpfCnpjTomador.Substring(0, 8))) ||
                //                           (o.TipoTomador == Dominio.Enumeradores.TipoTomador.Recebedor && o.Recebedor.CPF_CNPJ.Contains(cpfCnpjTomador.Substring(0, 8))) ||
                //                           (o.TipoTomador == Dominio.Enumeradores.TipoTomador.Remetente && o.Remetente.CPF_CNPJ.Contains(cpfCnpjTomador.Substring(0, 8))));
                //else
                //    result = result.Where(o => (o.TipoTomador == Dominio.Enumeradores.TipoTomador.Destinatario && o.Destinatario.CPF_CNPJ == cpfCnpjTomador) ||
                //                               (o.TipoTomador == Dominio.Enumeradores.TipoTomador.Expedidor && o.Expedidor.CPF_CNPJ == cpfCnpjTomador) ||
                //                               (o.TipoTomador == Dominio.Enumeradores.TipoTomador.Outros && o.OutrosTomador.CPF_CNPJ == cpfCnpjTomador) ||
                //                               (o.TipoTomador == Dominio.Enumeradores.TipoTomador.Recebedor && o.Recebedor.CPF_CNPJ == cpfCnpjTomador) ||
                //                               (o.TipoTomador == Dominio.Enumeradores.TipoTomador.Remetente && o.Remetente.CPF_CNPJ == cpfCnpjTomador));
                if (removerCliente)
                {
                    if (raizCNPJTomador)
                        result = result.Where(o => !o.TomadorPagador.CPF_CNPJ.Contains(cpfCnpjTomador.Substring(0, 8)));
                    else
                        result = result.Where(o => o.TomadorPagador.CPF_CNPJ != cpfCnpjTomador);
                }
                else
                {
                    if (raizCNPJTomador)
                        result = result.Where(o => o.TomadorPagador.CPF_CNPJ.Contains(cpfCnpjTomador.Substring(0, 8)));
                    else
                        result = result.Where(o => o.TomadorPagador.CPF_CNPJ == cpfCnpjTomador);
                }
            }

            if (numeroInicial > 0)
                result = result.Where(o => o.Numero >= numeroInicial);

            if (numeroFinal > 0)
                result = result.Where(o => o.Numero <= numeroFinal);

            if (codigoSerie > 0)
                result = result.Where(o => o.Serie.Codigo == codigoSerie);
            else if (series.Count() > 0)
                result = result.Where(o => series.Contains(o.Serie.Codigo));

            if (!string.IsNullOrWhiteSpace(cpfMotorista) || !string.IsNullOrWhiteSpace(nomeMotorista))
            {
                var queryMotorista = this.SessionNHiBernate.Query<Dominio.Entidades.MotoristaCTE>();

                if (!string.IsNullOrWhiteSpace(cpfMotorista))
                    result = result.Where(o => (from obj in queryMotorista where obj.CPFMotorista.Equals(cpfMotorista) select obj.CTE.Codigo).Contains(o.Codigo));
                else
                    result = result.Where(o => (from obj in queryMotorista where obj.NomeMotoristaCTe.Contains(nomeMotorista) select obj.CTE.Codigo).Contains(o.Codigo));
            }

            if (!string.IsNullOrWhiteSpace(placaVeiculo))
            {
                var queryVeiculos = this.SessionNHiBernate.Query<Dominio.Entidades.VeiculoCTE>();

                result = result.Where(o => (from obj in queryVeiculos where obj.CTE.Codigo == o.Codigo && obj.Placa.Equals(placaVeiculo) select obj.CTE.Codigo).Contains(o.Codigo));
            }

            if (!string.IsNullOrWhiteSpace(numeroNotaFiscal))
            {
                var queryDocumentos = this.SessionNHiBernate.Query<Dominio.Entidades.DocumentosCTE>();

                result = result.Where(o => (from obj in queryDocumentos where obj.CTE.Codigo == o.Codigo && obj.Numero.Contains(numeroNotaFiscal) select obj.CTE.Codigo).Contains(o.Codigo));
            }

            if (!string.IsNullOrWhiteSpace(tipoOcorrencia))
            {
                var queryOcorrencias = this.SessionNHiBernate.Query<Dominio.Entidades.OcorrenciaDeCTe>();

                if (tipoOcorrencia == "F")
                    result = result.Where(o => (from obj in queryOcorrencias where obj.CTe.Codigo == o.Codigo && obj.Ocorrencia.Tipo.Equals("F") select obj.CTe.Codigo).Contains(o.Codigo));
                else
                    result = result.Where(o => !(from obj in queryOcorrencias where obj.CTe.Codigo == o.Codigo && obj.Ocorrencia.Tipo.Equals("F") select obj.CTe.Codigo).Contains(o.Codigo));
            }

            if (statusPagamento.HasValue)
            {
                var queryDuplicatas = this.SessionNHiBernate.Query<Dominio.Entidades.ParcelaCobrancaCTe>();

                result = result.Where(o => (from obj in queryDuplicatas select obj.Cobranca.CTe.Codigo).Contains(o.Codigo));

                if (statusPagamento.Value == Dominio.Enumeradores.StatusDuplicata.Paga)
                    result = result.Where(o => !(from obj in queryDuplicatas where obj.Status == Dominio.Enumeradores.StatusDuplicata.Pendente select obj.Cobranca.CTe.Codigo).Contains(o.Codigo));
                else
                    result = result.Where(o => (from obj in queryDuplicatas where obj.Status == Dominio.Enumeradores.StatusDuplicata.Pendente select obj.Cobranca.CTe.Codigo).Contains(o.Codigo));
            }

            if (!string.IsNullOrWhiteSpace(duplicata))
            {
                var queryDuplicata = this.SessionNHiBernate.Query<Dominio.Entidades.DuplicataCtes>();

                result = result.Where(o => (from obj in queryDuplicata where obj.Duplicata.Numero == int.Parse(duplicata) select obj.ConhecimentoDeTransporteEletronico.Codigo).Contains(o.Codigo));
            }

            if (icmsCTe == "0")
                result = result.Where(o => o.ValorICMS == 0);
            else
            if (icmsCTe == "1")
                result = result.Where(o => o.ValorICMS > 0);

            if (cstCTe != "0")
                result = result.Where(o => o.CST.Equals(cstCTe));

            if (tipoServico != null)
                result = result.Where(o => o.TipoServico == tipoServico);

            if (!string.IsNullOrWhiteSpace(observacao))
                result = result.Where(o => o.ObservacoesGerais.Contains(observacao));

            if (numeroCarga > 0 || numeroUnidade > 0)
            {
                var queryIntegracaoCTe = this.SessionNHiBernate.Query<Dominio.Entidades.IntegracaoCTe>();
                var resultIntegracaoCTe = from o in queryIntegracaoCTe select o;
                if (numeroCarga > 0)
                    resultIntegracaoCTe = resultIntegracaoCTe.Where(o => o.NumeroDaCarga == numeroCarga);
                if (numeroUnidade > 0)
                    resultIntegracaoCTe = resultIntegracaoCTe.Where(o => o.NumeroDaUnidade == numeroUnidade);

                result = result.Where(o => resultIntegracaoCTe.Select(c => c.CTe.Codigo).Contains(o.Codigo));
            }

            if (averbacaoCTe.HasValue)
            {
                var queryAverbacao = this.SessionNHiBernate.Query<Dominio.Entidades.AverbacaoCTe>();

                if (averbacaoCTe.Value == Dominio.Enumeradores.FiltroAverbacaoCTe.Averbados)
                    result = result.Where(o => (from obj in queryAverbacao where obj.CTe.Codigo == o.Codigo && obj.Status == Dominio.Enumeradores.StatusAverbacaoCTe.Sucesso select obj.CTe.Codigo).Contains(o.Codigo));

                if (averbacaoCTe.Value == Dominio.Enumeradores.FiltroAverbacaoCTe.NaoAverbados)
                    result = result.Where(o => !(from obj in queryAverbacao where obj.CTe.Codigo == o.Codigo && obj.Status == Dominio.Enumeradores.StatusAverbacaoCTe.Sucesso select obj.CTe.Codigo).Contains(o.Codigo));
            }


            return result
                   .Fetch(o => o.Empresa).Fetch(o => o.Serie).Fetch(o => o.Remetente).Fetch(o => o.Destinatario)
                   .OrderBy(o => o.Numero).Skip(inicioRegistros).Take(maximoRegistros).Timeout(240).ToList();
        }

        public int ContarConsulta(int codigoEmpresa, DateTime dataEmissaoInicial, DateTime dataEmissaoFinal, string status, Dominio.Enumeradores.TipoCTE tipoCTe, int codigoSerie, string cpfCnpjRemetente, string cpfCnpjExpedidor, string cpfCnpjRecebedor, string cpfCnpjDestinatario, string cpfCnpjTomador, int numeroInicial, int numeroFinal, string nomeMotorista, string cpfMotorista, string placaVeiculo, Dominio.Enumeradores.TipoAmbiente tipoAmbiente, int[] series, string tipoOcorrencia, string numeroNotaFiscal, Dominio.Enumeradores.StatusDuplicata? statusPagamento, int codigoLocalidadeInicioPrestacao, int codigoLocalidadeTerminoPrestacao, string ufInicio, string ufFim, string duplicata, bool importacao, bool exportacao, bool raizCNPJRemetente, bool raizCNPJExpedidor, bool raizCNPJRecebedor, bool raizCNPJDestinatario, bool raizCNPJTomador, string icmsCTe, Dominio.Enumeradores.TipoServico? tipoServico, string cstCTe, int codigoUsuario, string nomeUsuario, DateTime dataAutorizacaoInicial, DateTime dataAutorizacaoFinal, string observacao, bool removerCliente, int numeroCarga, int numeroUnidade, Dominio.Enumeradores.FiltroAverbacaoCTe? averbacaoCTe)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.ConhecimentoDeTransporteEletronico>();

            var result = from obj in query where obj.Empresa.Codigo == codigoEmpresa && obj.TipoAmbiente == tipoAmbiente && (obj.ModeloDocumentoFiscal.Numero.Equals("57") || obj.ModeloDocumentoFiscal.Numero.Equals("67")) select obj;

            if (dataEmissaoInicial != DateTime.MinValue)
                result = result.Where(o => o.DataEmissao >= dataEmissaoInicial.Date);

            if (dataEmissaoFinal != DateTime.MinValue)
                result = result.Where(o => o.DataEmissao < dataEmissaoFinal.AddDays(1).Date);

            if (dataAutorizacaoInicial != DateTime.MinValue)
                result = result.Where(o => o.DataRetornoSefaz >= dataAutorizacaoInicial.Date);

            if (dataAutorizacaoFinal != DateTime.MinValue)
                result = result.Where(o => o.DataRetornoSefaz < dataAutorizacaoFinal.AddDays(1).Date);

            if (!string.IsNullOrWhiteSpace(status))
                result = result.Where(o => o.Status.Equals(status));

            if (tipoCTe != Dominio.Enumeradores.TipoCTE.Todos)
                result = result.Where(o => o.TipoCTE == tipoCTe);

            if (codigoUsuario > 0 && !string.IsNullOrWhiteSpace(nomeUsuario))
                result = result.Where(o => o.Usuario.Codigo == codigoUsuario);//|| o.Log.Contains(nomeUsuario)

            if (!string.IsNullOrWhiteSpace(cpfCnpjRemetente))
            {
                if (removerCliente)
                {
                    if (raizCNPJRemetente)
                        result = result.Where(o => !o.Remetente.CPF_CNPJ.Contains(cpfCnpjRemetente.Substring(0, 8)));
                    else
                        result = result.Where(o => o.Remetente.CPF_CNPJ != cpfCnpjRemetente);
                }
                else
                {
                    if (raizCNPJRemetente)
                        result = result.Where(o => o.Remetente.CPF_CNPJ.Contains(cpfCnpjRemetente.Substring(0, 8)));
                    else
                        result = result.Where(o => o.Remetente.CPF_CNPJ == cpfCnpjRemetente);
                }
            }

            if (importacao)
                result = result.Where(o => o.Remetente.Exterior == true);

            if (!string.IsNullOrWhiteSpace(cpfCnpjDestinatario))
            {
                if (removerCliente)
                {
                    if (raizCNPJDestinatario)
                        result = result.Where(o => !o.Destinatario.CPF_CNPJ.Contains(cpfCnpjDestinatario.Substring(0, 8)));
                    else
                        result = result.Where(o => o.Destinatario.CPF_CNPJ != cpfCnpjDestinatario);
                }
                else
                {
                    if (raizCNPJDestinatario)
                        result = result.Where(o => o.Destinatario.CPF_CNPJ.Contains(cpfCnpjDestinatario.Substring(0, 8)));
                    else
                        result = result.Where(o => o.Destinatario.CPF_CNPJ == cpfCnpjDestinatario);
                }
            }

            if (exportacao)
                result = result.Where(o => o.Destinatario.Exterior == true);

            if (!string.IsNullOrWhiteSpace(cpfCnpjExpedidor))
            {
                if (removerCliente)
                {
                    if (raizCNPJExpedidor)
                        result = result.Where(o => !o.Expedidor.CPF_CNPJ.Contains(cpfCnpjExpedidor.Substring(0, 8)));
                    else
                        result = result.Where(o => o.Expedidor.CPF_CNPJ != cpfCnpjExpedidor);
                }
                else
                {
                    if (raizCNPJExpedidor)
                        result = result.Where(o => o.Expedidor.CPF_CNPJ.Contains(cpfCnpjExpedidor.Substring(0, 8)));
                    else
                        result = result.Where(o => o.Expedidor.CPF_CNPJ == cpfCnpjExpedidor);
                }
            }

            if (!string.IsNullOrWhiteSpace(cpfCnpjRecebedor))
            {
                if (removerCliente)
                {
                    if (raizCNPJRecebedor)
                        result = result.Where(o => !o.Recebedor.CPF_CNPJ.Contains(cpfCnpjRecebedor.Substring(0, 8)));
                    else
                        result = result.Where(o => o.Recebedor.CPF_CNPJ != cpfCnpjRecebedor);
                }
                else
                {
                    if (raizCNPJRecebedor)
                        result = result.Where(o => o.Recebedor.CPF_CNPJ.Contains(cpfCnpjRecebedor.Substring(0, 8)));
                    else
                        result = result.Where(o => o.Recebedor.CPF_CNPJ == cpfCnpjRecebedor);
                }
            }

            if (codigoLocalidadeInicioPrestacao > 0)
                result = result.Where(o => o.LocalidadeInicioPrestacao.Codigo == codigoLocalidadeInicioPrestacao);

            if (codigoLocalidadeTerminoPrestacao > 0)
                result = result.Where(o => o.LocalidadeTerminoPrestacao.Codigo == codigoLocalidadeTerminoPrestacao);

            if (codigoUsuario > 0 && !string.IsNullOrWhiteSpace(nomeUsuario))
                result = result.Where(o => (o.Usuario.Codigo == codigoUsuario)); //|| o.Log.Contains(nomeUsuario)

            if (!string.IsNullOrWhiteSpace(ufInicio))
                result = result.Where(o => o.LocalidadeInicioPrestacao.Estado.Sigla == ufInicio);

            if (!string.IsNullOrWhiteSpace(ufFim))
                result = result.Where(o => o.LocalidadeTerminoPrestacao.Estado.Sigla == ufFim);

            if (!string.IsNullOrWhiteSpace(cpfCnpjTomador))
            {
                //if (raizCNPJTomador)
                //    result = result.Where(o => (o.TipoTomador == Dominio.Enumeradores.TipoTomador.Destinatario && o.Destinatario.CPF_CNPJ.Contains(cpfCnpjTomador.Substring(0, 8))) ||
                //                           (o.TipoTomador == Dominio.Enumeradores.TipoTomador.Expedidor && o.Expedidor.CPF_CNPJ.Contains(cpfCnpjTomador.Substring(0, 8))) ||
                //                           (o.TipoTomador == Dominio.Enumeradores.TipoTomador.Outros && o.OutrosTomador.CPF_CNPJ.Contains(cpfCnpjTomador.Substring(0, 8))) ||
                //                           (o.TipoTomador == Dominio.Enumeradores.TipoTomador.Recebedor && o.Recebedor.CPF_CNPJ.Contains(cpfCnpjTomador.Substring(0, 8))) ||
                //                           (o.TipoTomador == Dominio.Enumeradores.TipoTomador.Remetente && o.Remetente.CPF_CNPJ.Contains(cpfCnpjTomador.Substring(0, 8))));
                //else
                //    result = result.Where(o => (o.TipoTomador == Dominio.Enumeradores.TipoTomador.Destinatario && o.Destinatario.CPF_CNPJ == cpfCnpjTomador) ||
                //                               (o.TipoTomador == Dominio.Enumeradores.TipoTomador.Expedidor && o.Expedidor.CPF_CNPJ == cpfCnpjTomador) ||
                //                               (o.TipoTomador == Dominio.Enumeradores.TipoTomador.Outros && o.OutrosTomador.CPF_CNPJ == cpfCnpjTomador) ||
                //                               (o.TipoTomador == Dominio.Enumeradores.TipoTomador.Recebedor && o.Recebedor.CPF_CNPJ == cpfCnpjTomador) ||
                //                               (o.TipoTomador == Dominio.Enumeradores.TipoTomador.Remetente && o.Remetente.CPF_CNPJ == cpfCnpjTomador));
                if (removerCliente)
                {
                    if (raizCNPJTomador)
                        result = result.Where(o => !o.TomadorPagador.CPF_CNPJ.Contains(cpfCnpjTomador.Substring(0, 8)));
                    else
                        result = result.Where(o => o.TomadorPagador.CPF_CNPJ != cpfCnpjTomador);
                }
                else
                {
                    if (raizCNPJTomador)
                        result = result.Where(o => o.TomadorPagador.CPF_CNPJ.Contains(cpfCnpjTomador.Substring(0, 8)));
                    else
                        result = result.Where(o => o.TomadorPagador.CPF_CNPJ == cpfCnpjTomador);
                }
            }

            if (numeroInicial > 0)
                result = result.Where(o => o.Numero >= numeroInicial);

            if (numeroFinal > 0)
                result = result.Where(o => o.Numero <= numeroFinal);

            if (codigoSerie > 0)
                result = result.Where(o => o.Serie.Codigo == codigoSerie);
            else if (series.Count() > 0)
                result = result.Where(o => series.Contains(o.Serie.Codigo));

            if (!string.IsNullOrWhiteSpace(cpfMotorista) || !string.IsNullOrWhiteSpace(nomeMotorista))
            {
                var queryMotorista = this.SessionNHiBernate.Query<Dominio.Entidades.MotoristaCTE>();

                if (!string.IsNullOrWhiteSpace(cpfMotorista))
                    result = result.Where(o => (from obj in queryMotorista where obj.CPFMotorista.Equals(cpfMotorista) select obj.CTE.Codigo).Contains(o.Codigo));
                else
                    result = result.Where(o => (from obj in queryMotorista where obj.NomeMotoristaCTe.Contains(nomeMotorista) select obj.CTE.Codigo).Contains(o.Codigo));
            }

            if (!string.IsNullOrWhiteSpace(placaVeiculo))
            {
                var queryVeiculos = this.SessionNHiBernate.Query<Dominio.Entidades.VeiculoCTE>();

                result = result.Where(o => (from obj in queryVeiculos where obj.CTE.Codigo == o.Codigo && obj.Placa.Equals(placaVeiculo) select obj.CTE.Codigo).Contains(o.Codigo));
            }

            if (!string.IsNullOrWhiteSpace(numeroNotaFiscal))
            {
                var queryDocumentos = this.SessionNHiBernate.Query<Dominio.Entidades.DocumentosCTE>();

                result = result.Where(o => (from obj in queryDocumentos where obj.CTE.Codigo == o.Codigo && obj.Numero.Contains(numeroNotaFiscal) select obj.CTE.Codigo).Contains(o.Codigo));
            }

            if (!string.IsNullOrWhiteSpace(tipoOcorrencia))
            {
                var queryOcorrencias = this.SessionNHiBernate.Query<Dominio.Entidades.OcorrenciaDeCTe>();

                if (tipoOcorrencia == "F")
                    result = result.Where(o => (from obj in queryOcorrencias where obj.CTe.Codigo == o.Codigo && obj.Ocorrencia.Tipo.Equals("F") select obj.CTe.Codigo).Contains(o.Codigo));
                else
                    result = result.Where(o => !(from obj in queryOcorrencias where obj.CTe.Codigo == o.Codigo && obj.Ocorrencia.Tipo.Equals("F") select obj.CTe.Codigo).Contains(o.Codigo));
            }

            if (statusPagamento.HasValue)
            {
                var queryDuplicatas = this.SessionNHiBernate.Query<Dominio.Entidades.ParcelaCobrancaCTe>();

                result = result.Where(o => (from obj in queryDuplicatas select obj.Cobranca.CTe.Codigo).Contains(o.Codigo));

                if (statusPagamento.Value == Dominio.Enumeradores.StatusDuplicata.Paga)
                    result = result.Where(o => !(from obj in queryDuplicatas where obj.Status == Dominio.Enumeradores.StatusDuplicata.Pendente select obj.Cobranca.CTe.Codigo).Contains(o.Codigo));
                else
                    result = result.Where(o => (from obj in queryDuplicatas where obj.Status == Dominio.Enumeradores.StatusDuplicata.Pendente select obj.Cobranca.CTe.Codigo).Contains(o.Codigo));
            }

            if (!string.IsNullOrWhiteSpace(duplicata))
            {
                var queryDuplicata = this.SessionNHiBernate.Query<Dominio.Entidades.DuplicataCtes>();

                result = result.Where(o => (from obj in queryDuplicata where obj.Duplicata.Numero == int.Parse(duplicata) select obj.ConhecimentoDeTransporteEletronico.Codigo).Contains(o.Codigo));
            }

            if (icmsCTe == "0")
                result = result.Where(o => o.ValorICMS == 0);
            else
            if (icmsCTe == "1")
                result = result.Where(o => o.ValorICMS > 0);

            if (cstCTe != "0")
                result = result.Where(o => o.CST.Equals(cstCTe));

            if (tipoServico != null)
                result = result.Where(o => o.TipoServico == tipoServico);

            if (!string.IsNullOrWhiteSpace(observacao))
                result = result.Where(o => o.ObservacoesGerais.Contains(observacao));


            if (numeroCarga > 0 || numeroUnidade > 0)
            {
                var queryIntegracaoCTe = this.SessionNHiBernate.Query<Dominio.Entidades.IntegracaoCTe>();
                var resultIntegracaoCTe = from o in queryIntegracaoCTe select o;
                if (numeroCarga > 0)
                    resultIntegracaoCTe = resultIntegracaoCTe.Where(o => o.NumeroDaCarga == numeroCarga);
                if (numeroUnidade > 0)
                    resultIntegracaoCTe = resultIntegracaoCTe.Where(o => o.NumeroDaUnidade == numeroUnidade);

                result = result.Where(o => resultIntegracaoCTe.Select(c => c.CTe.Codigo).Contains(o.Codigo));
            }

            if (averbacaoCTe.HasValue)
            {
                var queryAverbacao = this.SessionNHiBernate.Query<Dominio.Entidades.AverbacaoCTe>();

                if (averbacaoCTe.Value == Dominio.Enumeradores.FiltroAverbacaoCTe.Averbados)
                    result = result.Where(o => (from obj in queryAverbacao where obj.CTe.Codigo == o.Codigo && obj.Status == Dominio.Enumeradores.StatusAverbacaoCTe.Sucesso select obj.CTe.Codigo).Contains(o.Codigo));

                if (averbacaoCTe.Value == Dominio.Enumeradores.FiltroAverbacaoCTe.NaoAverbados)
                    result = result.Where(o => !(from obj in queryAverbacao where obj.CTe.Codigo == o.Codigo && obj.Status == Dominio.Enumeradores.StatusAverbacaoCTe.Sucesso select obj.CTe.Codigo).Contains(o.Codigo));
            }

            return result.Count();
        }

        public List<Dominio.ObjetosDeValor.Relatorios.RelatorioCTesEmitidos> RelatorioEmissao(int codigoEmpresa, DateTime dataEmissaoInicial, DateTime dataEmissaoFinal, string status, Dominio.Enumeradores.TipoCTE tipoCTe, int codigoSerie, string cpfCnpjRemetente, string cpfCnpjExpedidor, string cpfCnpjRecebedor, string cpfCnpjDestinatario, string cpfCnpjTomador, int numeroInicial, int numeroFinal, string nomeMotorista, string cpfMotorista, string placaVeiculo, Dominio.Enumeradores.TipoAmbiente tipoAmbiente, int[] series, string tipoOcorrencia, string numeroNotaFiscal, Dominio.Enumeradores.StatusDuplicata? statusPagamento, int codigoLocalidadeInicioPrestacao, int codigoLocalidadeTerminoPrestacao, string ufInicio, string ufFim, string duplicata, bool importacao, bool exportacao, bool raizCNPJRemetente, bool raizCNPJExpedidor, bool raizCNPJRecebedor, bool raizCNPJDestinatario, bool raizCNPJTomador, DateTime dataAutorizacaoInicial, DateTime dataAutorizacaoFinal, string icmsCTe = "", Dominio.Enumeradores.TipoServico? tipoServico = null, string cstCTe = "0", int codigoUsuario = 0, string nomeUsuario = "", string cnpjEmbarcadorUsuario = "", string observacao = "", bool removerCliente = false, int numeroCarga = 0, int numeroUnidade = 0)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.ConhecimentoDeTransporteEletronico>();

            var result = from obj in query where obj.TipoAmbiente == tipoAmbiente select obj;

            result = result.Where(o => (o.ModeloDocumentoFiscal.Numero.Equals("57") || o.ModeloDocumentoFiscal.Numero.Equals("67")));

            if (codigoEmpresa > 0)
                result = result.Where(o => o.Empresa.Codigo == codigoEmpresa);

            if (dataEmissaoInicial != DateTime.MinValue)
                result = result.Where(o => o.DataEmissao >= dataEmissaoInicial.Date);

            if (dataEmissaoFinal != DateTime.MinValue)
                result = result.Where(o => o.DataEmissao < dataEmissaoFinal.AddDays(1).Date);

            if (dataAutorizacaoInicial != DateTime.MinValue)
                result = result.Where(o => o.DataRetornoSefaz >= dataAutorizacaoInicial.Date);

            if (dataAutorizacaoFinal != DateTime.MinValue)
                result = result.Where(o => o.DataRetornoSefaz < dataAutorizacaoFinal.AddDays(1).Date);

            if (codigoUsuario > 0 && !string.IsNullOrWhiteSpace(nomeUsuario))
                result = result.Where(o => o.Usuario.Codigo == codigoUsuario); // || o.Log.Contains(nomeUsuario)

            if (!string.IsNullOrWhiteSpace(status))
                result = result.Where(o => o.Status.Equals(status));

            if (tipoCTe != Dominio.Enumeradores.TipoCTE.Todos)
                result = result.Where(o => o.TipoCTE == tipoCTe);

            if (!string.IsNullOrWhiteSpace(cpfCnpjRemetente) && cpfCnpjRemetente != "0")
            {
                if (removerCliente)
                {
                    if (raizCNPJRemetente)
                        result = result.Where(o => !o.Remetente.CPF_CNPJ.Contains(cpfCnpjRemetente.Substring(0, 8)));
                    else
                        result = result.Where(o => o.Remetente.CPF_CNPJ != cpfCnpjRemetente);
                }
                else
                {
                    if (raizCNPJRemetente)
                        result = result.Where(o => o.Remetente.CPF_CNPJ.Contains(cpfCnpjRemetente.Substring(0, 8)));
                    else
                        result = result.Where(o => o.Remetente.CPF_CNPJ == cpfCnpjRemetente);
                }
            }

            if (!string.IsNullOrWhiteSpace(cnpjEmbarcadorUsuario))
            {
                if (removerCliente)
                {
                    if (cnpjEmbarcadorUsuario.Length == 8)
                        result = result.Where(o => !o.Remetente.CPF_CNPJ.Contains(cnpjEmbarcadorUsuario));
                    else
                        result = result.Where(o => o.Remetente.CPF_CNPJ != cnpjEmbarcadorUsuario);
                }
                else
                {
                    if (cnpjEmbarcadorUsuario.Length == 8)
                        result = result.Where(o => o.Remetente.CPF_CNPJ.Contains(cnpjEmbarcadorUsuario));
                    else
                        result = result.Where(o => o.Remetente.CPF_CNPJ == cnpjEmbarcadorUsuario);
                }
            }

            if (importacao)
                result = result.Where(o => o.Remetente.Exterior == true);

            if (!string.IsNullOrWhiteSpace(cpfCnpjDestinatario))
            {
                if (removerCliente)
                {
                    if (raizCNPJDestinatario)
                        result = result.Where(o => !o.Destinatario.CPF_CNPJ.Contains(cpfCnpjDestinatario.Substring(0, 8)));
                    else
                        result = result.Where(o => o.Destinatario.CPF_CNPJ != cpfCnpjDestinatario);
                }
                else
                {
                    if (raizCNPJDestinatario)
                        result = result.Where(o => o.Destinatario.CPF_CNPJ.Contains(cpfCnpjDestinatario.Substring(0, 8)));
                    else
                        result = result.Where(o => o.Destinatario.CPF_CNPJ == cpfCnpjDestinatario);
                }
            }

            if (exportacao)
                result = result.Where(o => o.Destinatario.Exterior == true);

            if (!string.IsNullOrWhiteSpace(cpfCnpjExpedidor))
            {
                if (removerCliente)
                {
                    if (raizCNPJExpedidor)
                        result = result.Where(o => !o.Expedidor.CPF_CNPJ.Contains(cpfCnpjExpedidor.Substring(0, 8)));
                    else
                        result = result.Where(o => o.Expedidor.CPF_CNPJ != cpfCnpjExpedidor);
                }
                else
                {
                    if (raizCNPJExpedidor)
                        result = result.Where(o => o.Expedidor.CPF_CNPJ.Contains(cpfCnpjExpedidor.Substring(0, 8)));
                    else
                        result = result.Where(o => o.Expedidor.CPF_CNPJ == cpfCnpjExpedidor);
                }
            }

            if (!string.IsNullOrWhiteSpace(cpfCnpjRecebedor))
            {
                if (removerCliente)
                {
                    if (raizCNPJRecebedor)
                        result = result.Where(o => !o.Recebedor.CPF_CNPJ.Contains(cpfCnpjRecebedor.Substring(0, 8)));
                    else
                        result = result.Where(o => o.Recebedor.CPF_CNPJ != cpfCnpjRecebedor);
                }
                else
                {
                    if (raizCNPJRecebedor)
                        result = result.Where(o => o.Recebedor.CPF_CNPJ.Contains(cpfCnpjRecebedor.Substring(0, 8)));
                    else
                        result = result.Where(o => o.Recebedor.CPF_CNPJ == cpfCnpjRecebedor);
                }
            }

            if (codigoLocalidadeInicioPrestacao > 0)
                result = result.Where(o => o.LocalidadeInicioPrestacao.Codigo == codigoLocalidadeInicioPrestacao);

            if (codigoLocalidadeTerminoPrestacao > 0)
                result = result.Where(o => o.LocalidadeTerminoPrestacao.Codigo == codigoLocalidadeTerminoPrestacao);

            if (!string.IsNullOrWhiteSpace(ufInicio))
                result = result.Where(o => o.LocalidadeInicioPrestacao.Estado.Sigla == ufInicio);

            if (!string.IsNullOrWhiteSpace(ufFim))
                result = result.Where(o => o.LocalidadeTerminoPrestacao.Estado.Sigla == ufFim);

            if (!string.IsNullOrWhiteSpace(cpfCnpjTomador))
            {
                //if (raizCNPJTomador)
                //    result = result.Where(o => (o.TipoTomador == Dominio.Enumeradores.TipoTomador.Destinatario && o.Destinatario.CPF_CNPJ.Contains(cpfCnpjTomador.Substring(0, 8))) ||
                //                           (o.TipoTomador == Dominio.Enumeradores.TipoTomador.Expedidor && o.Expedidor.CPF_CNPJ.Contains(cpfCnpjTomador.Substring(0, 8))) ||
                //                           (o.TipoTomador == Dominio.Enumeradores.TipoTomador.Outros && o.OutrosTomador.CPF_CNPJ.Contains(cpfCnpjTomador.Substring(0, 8))) ||
                //                           (o.TipoTomador == Dominio.Enumeradores.TipoTomador.Recebedor && o.Recebedor.CPF_CNPJ.Contains(cpfCnpjTomador.Substring(0, 8))) ||
                //                           (o.TipoTomador == Dominio.Enumeradores.TipoTomador.Remetente && o.Remetente.CPF_CNPJ.Contains(cpfCnpjTomador.Substring(0, 8))));
                //else
                //    result = result.Where(o => (o.TipoTomador == Dominio.Enumeradores.TipoTomador.Destinatario && o.Destinatario.CPF_CNPJ == cpfCnpjTomador) ||
                //                               (o.TipoTomador == Dominio.Enumeradores.TipoTomador.Expedidor && o.Expedidor.CPF_CNPJ == cpfCnpjTomador) ||
                //                               (o.TipoTomador == Dominio.Enumeradores.TipoTomador.Outros && o.OutrosTomador.CPF_CNPJ == cpfCnpjTomador) ||
                //                               (o.TipoTomador == Dominio.Enumeradores.TipoTomador.Recebedor && o.Recebedor.CPF_CNPJ == cpfCnpjTomador) ||
                //                               (o.TipoTomador == Dominio.Enumeradores.TipoTomador.Remetente && o.Remetente.CPF_CNPJ == cpfCnpjTomador));
                if (removerCliente)
                {
                    if (raizCNPJTomador)
                        result = result.Where(o => !o.TomadorPagador.CPF_CNPJ.Contains(cpfCnpjTomador.Substring(0, 8)));
                    else
                        result = result.Where(o => o.TomadorPagador.CPF_CNPJ != cpfCnpjTomador);
                }
                else
                {
                    if (raizCNPJTomador)
                        result = result.Where(o => o.TomadorPagador.CPF_CNPJ.Contains(cpfCnpjTomador.Substring(0, 8)));
                    else
                        result = result.Where(o => o.TomadorPagador.CPF_CNPJ == cpfCnpjTomador);
                }
            }

            if (numeroInicial > 0)
                result = result.Where(o => o.Numero >= numeroInicial);

            if (numeroFinal > 0)
                result = result.Where(o => o.Numero <= numeroFinal);

            if (codigoSerie > 0)
                result = result.Where(o => o.Serie.Codigo == codigoSerie);
            else if (series != null && series.Count() > 0)
                result = result.Where(o => series.Contains(o.Serie.Codigo));

            if (!string.IsNullOrWhiteSpace(cpfMotorista) || !string.IsNullOrWhiteSpace(nomeMotorista))
            {
                var queryMotorista = this.SessionNHiBernate.Query<Dominio.Entidades.MotoristaCTE>();

                if (!string.IsNullOrWhiteSpace(cpfMotorista))
                    result = result.Where(o => (from obj in queryMotorista where obj.CPFMotorista.Equals(cpfMotorista) select obj.CTE.Codigo).Contains(o.Codigo));
                else
                    result = result.Where(o => (from obj in queryMotorista where obj.NomeMotoristaCTe.Contains(nomeMotorista) select obj.CTE.Codigo).Contains(o.Codigo));
            }

            if (!string.IsNullOrWhiteSpace(placaVeiculo))
            {
                var queryVeiculos = this.SessionNHiBernate.Query<Dominio.Entidades.VeiculoCTE>();

                result = result.Where(o => (from obj in queryVeiculos where obj.CTE.Codigo == o.Codigo && obj.Placa.Equals(placaVeiculo) select obj.CTE.Codigo).Contains(o.Codigo));
            }

            if (!string.IsNullOrWhiteSpace(numeroNotaFiscal))
            {
                var queryDocumentos = this.SessionNHiBernate.Query<Dominio.Entidades.DocumentosCTE>();

                result = result.Where(o => (from obj in queryDocumentos where obj.CTE.Codigo == o.Codigo && obj.Numero.Contains(numeroNotaFiscal) select obj.CTE.Codigo).Contains(o.Codigo));
            }

            if (!string.IsNullOrWhiteSpace(tipoOcorrencia))
            {
                var queryOcorrencias = this.SessionNHiBernate.Query<Dominio.Entidades.OcorrenciaDeCTe>();

                if (tipoOcorrencia == "F")
                    result = result.Where(o => (from obj in queryOcorrencias where obj.CTe.Codigo == o.Codigo && obj.Ocorrencia.Tipo.Equals("F") select obj.CTe.Codigo).Contains(o.Codigo));
                else
                    result = result.Where(o => !(from obj in queryOcorrencias where obj.CTe.Codigo == o.Codigo && obj.Ocorrencia.Tipo.Equals("F") select obj.CTe.Codigo).Contains(o.Codigo));
            }

            if (statusPagamento.HasValue)
            {
                var queryDuplicatas = this.SessionNHiBernate.Query<Dominio.Entidades.ParcelaCobrancaCTe>();

                result = result.Where(o => (from obj in queryDuplicatas select obj.Cobranca.CTe.Codigo).Contains(o.Codigo));

                if (statusPagamento.Value == Dominio.Enumeradores.StatusDuplicata.Paga)
                    result = result.Where(o => !(from obj in queryDuplicatas where obj.Status == Dominio.Enumeradores.StatusDuplicata.Pendente select obj.Cobranca.CTe.Codigo).Contains(o.Codigo));
                else
                    result = result.Where(o => (from obj in queryDuplicatas where obj.Status == Dominio.Enumeradores.StatusDuplicata.Pendente select obj.Cobranca.CTe.Codigo).Contains(o.Codigo));
            }

            if (!string.IsNullOrWhiteSpace(duplicata))
            {
                var queryDuplicata = this.SessionNHiBernate.Query<Dominio.Entidades.DuplicataCtes>();

                result = result.Where(o => (from obj in queryDuplicata where obj.Duplicata.Numero == int.Parse(duplicata) select obj.ConhecimentoDeTransporteEletronico.Codigo).Contains(o.Codigo));
            }

            if (icmsCTe == "0")
                result = result.Where(o => o.ValorICMS == 0);
            else
            if (icmsCTe == "1")
                result = result.Where(o => o.ValorICMS > 0);

            if (cstCTe != "0")
                result = result.Where(o => o.CST.Equals(cstCTe));

            if (tipoServico != null)
                result = result.Where(o => o.TipoServico == tipoServico);

            if (!string.IsNullOrWhiteSpace(observacao))
                result = result.Where(o => o.ObservacoesGerais.Contains(observacao));

            if (numeroCarga > 0 || numeroUnidade > 0)
            {
                var queryIntegracaoCTe = this.SessionNHiBernate.Query<Dominio.Entidades.IntegracaoCTe>();
                var resultIntegracaoCTe = from o in queryIntegracaoCTe select o;
                if (numeroCarga > 0)
                    resultIntegracaoCTe = resultIntegracaoCTe.Where(o => o.NumeroDaCarga == numeroCarga);
                if (numeroUnidade > 0)
                    resultIntegracaoCTe = resultIntegracaoCTe.Where(o => o.NumeroDaUnidade == numeroUnidade);

                result = result.Where(o => resultIntegracaoCTe.Select(c => c.CTe.Codigo).Contains(o.Codigo));
            }

            return result.Select(o => new Dominio.ObjetosDeValor.Relatorios.RelatorioCTesEmitidos()
            {
                CODIGO = o.Codigo,
                CODIGO_CTE = o.Numero,
                CST = o.CST,
                DATA_EMISSAO_CTE = o.DataEmissao.Value,
                DESTINATARIO_EXTERIOR = o.Destinatario != null ? o.Destinatario.Exterior : false,
                FRETE_ICMS = o.ValorFrete,
                UF_EMPRESA = o.Empresa.Localidade.Estado.Sigla,
                NOME_CIDADE_FIM = o.LocalidadeTerminoPrestacao.Descricao,
                NOME_CIDADE_FIM_EXTERIOR = o.Destinatario != null ? o.Destinatario.Cidade : string.Empty,
                NOME_CIDADE_INICIO = o.LocalidadeInicioPrestacao.Descricao,
                NOME_CIDADE_INICIO_EXTERIOR = o.Remetente != null ? o.Remetente.Cidade : string.Empty,
                NOME_RAZAO = o.Remetente != null ? o.Remetente.Nome : string.Empty,
                NOME_RAZAO_1 = o.Destinatario != null ? o.Destinatario.Nome : string.Empty,
                PAIS_FIM = o.Destinatario != null && o.Destinatario.Pais != null ? o.Destinatario.Pais.Nome : string.Empty,
                PAIS_INICIO = o.Remetente != null && o.Remetente.Pais != null ? o.Remetente.Pais.Nome : string.Empty,
                REMETENTE_EXTERIOR = o.Remetente != null ? o.Remetente.Exterior : false,
                SERIE = o.Serie.Numero,
                Status_Abrev = o.Status,
                TIPO_CTE = o.TipoCTE,
                UF_FIM = o.LocalidadeTerminoPrestacao.Estado.Sigla,
                UF_INICIO = o.LocalidadeInicioPrestacao.Estado.Sigla,
                BASE_CALCULO_ICMS = o.BaseCalculoICMS,
                ALIQUOTA_ICMS = o.AliquotaICMS,
                VALOR_ICMS = o.ValorICMS,
                VALOR_RECEBER = Math.Round(o.ValorAReceber, 2, MidpointRounding.ToEven),
                VALOR_SERVICO = o.ValorPrestacaoServico,
                QUANTIDADE = (from obj in this.SessionNHiBernate.Query<Dominio.Entidades.InformacaoCargaCTE>() where obj.CTE.Codigo == o.Codigo && obj.UnidadeMedida == "01" select (decimal?)obj.Quantidade).Sum() ?? 0m,
                QUANTIDADE_TOTAL = (from obj in this.SessionNHiBernate.Query<Dominio.Entidades.InformacaoCargaCTE>() where obj.CTE.Codigo == o.Codigo select (decimal?)obj.Quantidade).Sum() ?? 0m,
                LOCALIDADE_INICIO_PRESTACAO = o.LocalidadeInicioPrestacao.Descricao,
                ESTADO_INICIO_PRESTACAO = o.LocalidadeInicioPrestacao.Estado.Sigla,
                LOCALIDADE_TERMINO_PRESTACAO = o.LocalidadeTerminoPrestacao.Descricao,
                ESTADO_TERMINO_PRESTACAO = o.LocalidadeTerminoPrestacao.Estado.Sigla,
                CFOP = o.CFOP.CodigoCFOP,
                CHAVE = o.Chave,
                VALOR_MERCADORIA = o.ValorTotalMercadoria,
                PLACA = (from obj in this.SessionNHiBernate.Query<Dominio.Entidades.VeiculoCTE>() where obj.CTE.Codigo == o.Codigo select obj.Placa).FirstOrDefault() ?? string.Empty,
                CNPJ_TRANSPORTADOR = o.Empresa.CNPJ,
                TRANSPORTADOR = o.Empresa.RazaoSocial,
                CONDICAO_PAGAMENTO = o.TipoPagamento == Dominio.Enumeradores.TipoPagamento.Pago ? "CIF" : "FOB",
                NOTAS_FISCAIS = o.NumeroNotas,
                Usuario = o.Usuario != null ? !string.IsNullOrWhiteSpace(o.Usuario.Nome) ? o.Usuario.Nome : o.Usuario.Login : string.Empty,
                ALIQUOTA_PIS = o.AliquotaPIS,
                VALOR_PIS = o.ValorPIS,
                ALIQUOTA_COFINS = o.AliquotaCOFINS,
                VALOR_COFINS = o.ValorCOFINS,
                PRODUTO_PREDOMINANTE = o.ProdutoPredominanteCTe,
                NumeroCTeSubcontratado = o.TipoServico == Dominio.Enumeradores.TipoServico.SubContratacao ? (from obj in this.SessionNHiBernate.Query<Dominio.Entidades.DocumentoDeTransporteAnteriorCTe>() where obj.CTe.Codigo == o.Codigo select obj.Chave.Substring(25, 9)).FirstOrDefault() ?? string.Empty : string.Empty,
                SerieCTeSubcontratado = o.TipoServico == Dominio.Enumeradores.TipoServico.SubContratacao ? (from obj in this.SessionNHiBernate.Query<Dominio.Entidades.DocumentoDeTransporteAnteriorCTe>() where obj.CTe.Codigo == o.Codigo select obj.Chave.Substring(22, 3)).FirstOrDefault() ?? string.Empty : string.Empty,
                TIPO_SERVICO = o.TipoServico,
                CNPJ_REMETENTE = o.Remetente != null ? o.Remetente.CPF_CNPJ : string.Empty,
                CNPJ_DESTINATARIO = o.Destinatario != null ? o.Destinatario.CPF_CNPJ : string.Empty,
                ProtocoloCTe = o.Protocolo
            }).OrderBy(o => o.TRANSPORTADOR)
              .ThenBy(o => o.CODIGO_CTE)
              .Timeout(120)
              .ToList();
        }

        public List<Dominio.ObjetosDeValor.Relatorios.RelatorioCTesEmitidos> RelatorioEmissaoDuplicata(int codigoEmpresa, DateTime dataEmissaoInicial, DateTime dataEmissaoFinal, string status, Dominio.Enumeradores.TipoCTE tipoCTe, int codigoSerie, string cpfCnpjRemetente, string cpfCnpjExpedidor, string cpfCnpjRecebedor, string cpfCnpjDestinatario, string cpfCnpjTomador, int numeroInicial, int numeroFinal, string nomeMotorista, string cpfMotorista, string placaVeiculo, Dominio.Enumeradores.TipoAmbiente tipoAmbiente, int[] series, string tipoOcorrencia, string numeroNotaFiscal, Dominio.Enumeradores.StatusDuplicata? statusPagamento, int codigoLocalidadeInicioPrestacao, int codigoLocalidadeTerminoPrestacao, string ufInicio, string ufFim, string duplicata, bool importacao, bool exportacao, bool raizCNPJRemetente, bool raizCNPJExpedidor, bool raizCNPJRecebedor, bool raizCNPJDestinatario, bool raizCNPJTomador, DateTime dataAutorizacaoInicial, DateTime dataAutorizacaoFinal, string icmsCTe = "", Dominio.Enumeradores.TipoServico? tipoServico = null, string cstCTe = "0", int codigoUsuario = 0, string nomeUsuario = "", string cnpjEmbarcadorUsuario = "", string observacao = "", bool removerCliente = false, int numeroCarga = 0, int numeroUnidade = 0)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.ConhecimentoDeTransporteEletronico>();

            var result = from obj in query where obj.TipoAmbiente == tipoAmbiente select obj;

            result = result.Where(o => (o.ModeloDocumentoFiscal.Numero.Equals("57") || o.ModeloDocumentoFiscal.Numero.Equals("67")));

            if (codigoEmpresa > 0)
                result = result.Where(o => o.Empresa.Codigo == codigoEmpresa);

            if (dataEmissaoInicial != DateTime.MinValue)
                result = result.Where(o => o.DataEmissao >= dataEmissaoInicial.Date);

            if (dataEmissaoFinal != DateTime.MinValue)
                result = result.Where(o => o.DataEmissao < dataEmissaoFinal.AddDays(1).Date);

            if (dataAutorizacaoInicial != DateTime.MinValue)
                result = result.Where(o => o.DataRetornoSefaz >= dataAutorizacaoInicial.Date);

            if (dataAutorizacaoFinal != DateTime.MinValue)
                result = result.Where(o => o.DataRetornoSefaz < dataAutorizacaoFinal.AddDays(1).Date);

            if (codigoUsuario > 0 && !string.IsNullOrWhiteSpace(nomeUsuario))
                result = result.Where(o => o.Usuario.Codigo == codigoUsuario); // || o.Log.Contains(nomeUsuario)

            if (!string.IsNullOrWhiteSpace(status))
                result = result.Where(o => o.Status.Equals(status));

            if (tipoCTe != Dominio.Enumeradores.TipoCTE.Todos)
                result = result.Where(o => o.TipoCTE == tipoCTe);

            if (!string.IsNullOrWhiteSpace(cpfCnpjRemetente) && cpfCnpjRemetente != "0")
            {
                if (removerCliente)
                {
                    if (raizCNPJRemetente)
                        result = result.Where(o => !o.Remetente.CPF_CNPJ.Contains(cpfCnpjRemetente.Substring(0, 8)));
                    else
                        result = result.Where(o => o.Remetente.CPF_CNPJ != cpfCnpjRemetente);
                }
                else
                {
                    if (raizCNPJRemetente)
                        result = result.Where(o => o.Remetente.CPF_CNPJ.Contains(cpfCnpjRemetente.Substring(0, 8)));
                    else
                        result = result.Where(o => o.Remetente.CPF_CNPJ == cpfCnpjRemetente);
                }
            }

            if (!string.IsNullOrWhiteSpace(cnpjEmbarcadorUsuario))
            {
                if (removerCliente)
                {
                    if (cnpjEmbarcadorUsuario.Length == 8)
                        result = result.Where(o => !o.Remetente.CPF_CNPJ.Contains(cnpjEmbarcadorUsuario));
                    else
                        result = result.Where(o => o.Remetente.CPF_CNPJ != cnpjEmbarcadorUsuario);
                }
                else
                {
                    if (cnpjEmbarcadorUsuario.Length == 8)
                        result = result.Where(o => o.Remetente.CPF_CNPJ.Contains(cnpjEmbarcadorUsuario));
                    else
                        result = result.Where(o => o.Remetente.CPF_CNPJ == cnpjEmbarcadorUsuario);
                }
            }

            if (importacao)
                result = result.Where(o => o.Remetente.Exterior == true);

            if (!string.IsNullOrWhiteSpace(cpfCnpjDestinatario))
            {
                if (removerCliente)
                {
                    if (raizCNPJDestinatario)
                        result = result.Where(o => !o.Destinatario.CPF_CNPJ.Contains(cpfCnpjDestinatario.Substring(0, 8)));
                    else
                        result = result.Where(o => o.Destinatario.CPF_CNPJ != cpfCnpjDestinatario);
                }
                else
                {
                    if (raizCNPJDestinatario)
                        result = result.Where(o => o.Destinatario.CPF_CNPJ.Contains(cpfCnpjDestinatario.Substring(0, 8)));
                    else
                        result = result.Where(o => o.Destinatario.CPF_CNPJ == cpfCnpjDestinatario);
                }
            }

            if (exportacao)
                result = result.Where(o => o.Destinatario.Exterior == true);

            if (!string.IsNullOrWhiteSpace(cpfCnpjExpedidor))
            {
                if (removerCliente)
                {
                    if (raizCNPJExpedidor)
                        result = result.Where(o => !o.Expedidor.CPF_CNPJ.Contains(cpfCnpjExpedidor.Substring(0, 8)));
                    else
                        result = result.Where(o => o.Expedidor.CPF_CNPJ != cpfCnpjExpedidor);
                }
                else
                {
                    if (raizCNPJExpedidor)
                        result = result.Where(o => o.Expedidor.CPF_CNPJ.Contains(cpfCnpjExpedidor.Substring(0, 8)));
                    else
                        result = result.Where(o => o.Expedidor.CPF_CNPJ == cpfCnpjExpedidor);
                }
            }

            if (!string.IsNullOrWhiteSpace(cpfCnpjRecebedor))
            {
                if (removerCliente)
                {
                    if (raizCNPJRecebedor)
                        result = result.Where(o => !o.Recebedor.CPF_CNPJ.Contains(cpfCnpjRecebedor.Substring(0, 8)));
                    else
                        result = result.Where(o => o.Recebedor.CPF_CNPJ != cpfCnpjRecebedor);
                }
                else
                {
                    if (raizCNPJRecebedor)
                        result = result.Where(o => o.Recebedor.CPF_CNPJ.Contains(cpfCnpjRecebedor.Substring(0, 8)));
                    else
                        result = result.Where(o => o.Recebedor.CPF_CNPJ == cpfCnpjRecebedor);
                }
            }

            if (codigoLocalidadeInicioPrestacao > 0)
                result = result.Where(o => o.LocalidadeInicioPrestacao.Codigo == codigoLocalidadeInicioPrestacao);

            if (codigoLocalidadeTerminoPrestacao > 0)
                result = result.Where(o => o.LocalidadeTerminoPrestacao.Codigo == codigoLocalidadeTerminoPrestacao);

            if (!string.IsNullOrWhiteSpace(ufInicio))
                result = result.Where(o => o.LocalidadeInicioPrestacao.Estado.Sigla == ufInicio);

            if (!string.IsNullOrWhiteSpace(ufFim))
                result = result.Where(o => o.LocalidadeTerminoPrestacao.Estado.Sigla == ufFim);

            if (!string.IsNullOrWhiteSpace(cpfCnpjTomador))
            {
                //if (raizCNPJTomador)
                //    result = result.Where(o => (o.TipoTomador == Dominio.Enumeradores.TipoTomador.Destinatario && o.Destinatario.CPF_CNPJ.Contains(cpfCnpjTomador.Substring(0, 8))) ||
                //                           (o.TipoTomador == Dominio.Enumeradores.TipoTomador.Expedidor && o.Expedidor.CPF_CNPJ.Contains(cpfCnpjTomador.Substring(0, 8))) ||
                //                           (o.TipoTomador == Dominio.Enumeradores.TipoTomador.Outros && o.OutrosTomador.CPF_CNPJ.Contains(cpfCnpjTomador.Substring(0, 8))) ||
                //                           (o.TipoTomador == Dominio.Enumeradores.TipoTomador.Recebedor && o.Recebedor.CPF_CNPJ.Contains(cpfCnpjTomador.Substring(0, 8))) ||
                //                           (o.TipoTomador == Dominio.Enumeradores.TipoTomador.Remetente && o.Remetente.CPF_CNPJ.Contains(cpfCnpjTomador.Substring(0, 8))));
                //else
                //    result = result.Where(o => (o.TipoTomador == Dominio.Enumeradores.TipoTomador.Destinatario && o.Destinatario.CPF_CNPJ == cpfCnpjTomador) ||
                //                               (o.TipoTomador == Dominio.Enumeradores.TipoTomador.Expedidor && o.Expedidor.CPF_CNPJ == cpfCnpjTomador) ||
                //                               (o.TipoTomador == Dominio.Enumeradores.TipoTomador.Outros && o.OutrosTomador.CPF_CNPJ == cpfCnpjTomador) ||
                //                               (o.TipoTomador == Dominio.Enumeradores.TipoTomador.Recebedor && o.Recebedor.CPF_CNPJ == cpfCnpjTomador) ||
                //                               (o.TipoTomador == Dominio.Enumeradores.TipoTomador.Remetente && o.Remetente.CPF_CNPJ == cpfCnpjTomador));
                if (removerCliente)
                {
                    if (raizCNPJTomador)
                        result = result.Where(o => !o.TomadorPagador.CPF_CNPJ.Contains(cpfCnpjTomador.Substring(0, 8)));
                    else
                        result = result.Where(o => o.TomadorPagador.CPF_CNPJ != cpfCnpjTomador);
                }
                else
                {
                    if (raizCNPJTomador)
                        result = result.Where(o => o.TomadorPagador.CPF_CNPJ.Contains(cpfCnpjTomador.Substring(0, 8)));
                    else
                        result = result.Where(o => o.TomadorPagador.CPF_CNPJ == cpfCnpjTomador);
                }
            }

            if (numeroInicial > 0)
                result = result.Where(o => o.Numero >= numeroInicial);

            if (numeroFinal > 0)
                result = result.Where(o => o.Numero <= numeroFinal);

            if (codigoSerie > 0)
                result = result.Where(o => o.Serie.Codigo == codigoSerie);
            else if (series != null && series.Count() > 0)
                result = result.Where(o => series.Contains(o.Serie.Codigo));

            if (!string.IsNullOrWhiteSpace(cpfMotorista) || !string.IsNullOrWhiteSpace(nomeMotorista))
            {
                var queryMotorista = this.SessionNHiBernate.Query<Dominio.Entidades.MotoristaCTE>();

                if (!string.IsNullOrWhiteSpace(cpfMotorista))
                    result = result.Where(o => (from obj in queryMotorista where obj.CPFMotorista.Equals(cpfMotorista) select obj.CTE.Codigo).Contains(o.Codigo));
                else
                    result = result.Where(o => (from obj in queryMotorista where obj.NomeMotoristaCTe.Contains(nomeMotorista) select obj.CTE.Codigo).Contains(o.Codigo));
            }

            if (!string.IsNullOrWhiteSpace(placaVeiculo))
            {
                var queryVeiculos = this.SessionNHiBernate.Query<Dominio.Entidades.VeiculoCTE>();

                result = result.Where(o => (from obj in queryVeiculos where obj.CTE.Codigo == o.Codigo && obj.Placa.Equals(placaVeiculo) select obj.CTE.Codigo).Contains(o.Codigo));
            }

            if (!string.IsNullOrWhiteSpace(numeroNotaFiscal))
            {
                var queryDocumentos = this.SessionNHiBernate.Query<Dominio.Entidades.DocumentosCTE>();

                result = result.Where(o => (from obj in queryDocumentos where obj.CTE.Codigo == o.Codigo && obj.Numero.Contains(numeroNotaFiscal) select obj.CTE.Codigo).Contains(o.Codigo));
            }

            if (!string.IsNullOrWhiteSpace(tipoOcorrencia))
            {
                var queryOcorrencias = this.SessionNHiBernate.Query<Dominio.Entidades.OcorrenciaDeCTe>();

                if (tipoOcorrencia == "F")
                    result = result.Where(o => (from obj in queryOcorrencias where obj.CTe.Codigo == o.Codigo && obj.Ocorrencia.Tipo.Equals("F") select obj.CTe.Codigo).Contains(o.Codigo));
                else
                    result = result.Where(o => !(from obj in queryOcorrencias where obj.CTe.Codigo == o.Codigo && obj.Ocorrencia.Tipo.Equals("F") select obj.CTe.Codigo).Contains(o.Codigo));
            }

            if (statusPagamento.HasValue)
            {
                var queryDuplicatas = this.SessionNHiBernate.Query<Dominio.Entidades.ParcelaCobrancaCTe>();

                result = result.Where(o => (from obj in queryDuplicatas select obj.Cobranca.CTe.Codigo).Contains(o.Codigo));

                if (statusPagamento.Value == Dominio.Enumeradores.StatusDuplicata.Paga)
                    result = result.Where(o => !(from obj in queryDuplicatas where obj.Status == Dominio.Enumeradores.StatusDuplicata.Pendente select obj.Cobranca.CTe.Codigo).Contains(o.Codigo));
                else
                    result = result.Where(o => (from obj in queryDuplicatas where obj.Status == Dominio.Enumeradores.StatusDuplicata.Pendente select obj.Cobranca.CTe.Codigo).Contains(o.Codigo));
            }

            if (!string.IsNullOrWhiteSpace(duplicata))
            {
                var queryDuplicata = this.SessionNHiBernate.Query<Dominio.Entidades.DuplicataCtes>();

                result = result.Where(o => (from obj in queryDuplicata where obj.Duplicata.Numero == int.Parse(duplicata) select obj.ConhecimentoDeTransporteEletronico.Codigo).Contains(o.Codigo));
            }

            if (icmsCTe == "0")
                result = result.Where(o => o.ValorICMS == 0);
            else
            if (icmsCTe == "1")
                result = result.Where(o => o.ValorICMS > 0);

            if (cstCTe != "0")
                result = result.Where(o => o.CST.Equals(cstCTe));

            if (tipoServico != null)
                result = result.Where(o => o.TipoServico == tipoServico);

            if (!string.IsNullOrWhiteSpace(observacao))
                result = result.Where(o => o.ObservacoesGerais.Contains(observacao));

            if (numeroCarga > 0 || numeroUnidade > 0)
            {
                var queryIntegracaoCTe = this.SessionNHiBernate.Query<Dominio.Entidades.IntegracaoCTe>();
                var resultIntegracaoCTe = from o in queryIntegracaoCTe select o;
                if (numeroCarga > 0)
                    resultIntegracaoCTe = resultIntegracaoCTe.Where(o => o.NumeroDaCarga == numeroCarga);
                if (numeroUnidade > 0)
                    resultIntegracaoCTe = resultIntegracaoCTe.Where(o => o.NumeroDaUnidade == numeroUnidade);

                result = result.Where(o => resultIntegracaoCTe.Select(c => c.CTe.Codigo).Contains(o.Codigo));
            }

            return result.Select(o => new Dominio.ObjetosDeValor.Relatorios.RelatorioCTesEmitidos()
            {
                CODIGO = o.Codigo,
                CODIGO_CTE = o.Numero,
                CST = o.CST,
                DATA_EMISSAO_CTE = o.DataEmissao.Value,
                DESTINATARIO_EXTERIOR = o.Destinatario != null ? o.Destinatario.Exterior : false,
                FRETE_ICMS = o.ValorFrete,
                UF_EMPRESA = o.Empresa.Localidade.Estado.Sigla,
                NOME_CIDADE_FIM = o.LocalidadeTerminoPrestacao.Descricao,
                NOME_CIDADE_FIM_EXTERIOR = o.Destinatario != null ? o.Destinatario.Cidade : string.Empty,
                NOME_CIDADE_INICIO = o.LocalidadeInicioPrestacao.Descricao,
                NOME_CIDADE_INICIO_EXTERIOR = o.Remetente != null ? o.Remetente.Cidade : string.Empty,
                NOME_RAZAO = o.Remetente != null ? o.Remetente.Nome : string.Empty,
                NOME_RAZAO_1 = o.Destinatario != null ? o.Destinatario.Nome : string.Empty,
                PAIS_FIM = o.Destinatario != null && o.Destinatario.Pais != null ? o.Destinatario.Pais.Nome : string.Empty,
                PAIS_INICIO = o.Remetente != null && o.Remetente.Pais != null ? o.Remetente.Pais.Nome : string.Empty,
                REMETENTE_EXTERIOR = o.Remetente != null ? o.Remetente.Exterior : false,
                SERIE = o.Serie.Numero,
                Status_Abrev = o.Status,
                TIPO_CTE = o.TipoCTE,
                UF_FIM = o.LocalidadeTerminoPrestacao.Estado.Sigla,
                UF_INICIO = o.LocalidadeInicioPrestacao.Estado.Sigla,
                BASE_CALCULO_ICMS = o.BaseCalculoICMS,
                ALIQUOTA_ICMS = o.AliquotaICMS,
                VALOR_ICMS = o.ValorICMS,
                VALOR_RECEBER = Math.Round(o.ValorAReceber, 2, MidpointRounding.ToEven),
                VALOR_SERVICO = o.ValorPrestacaoServico,
                QUANTIDADE = (from obj in this.SessionNHiBernate.Query<Dominio.Entidades.InformacaoCargaCTE>() where obj.CTE.Codigo == o.Codigo && obj.UnidadeMedida == "01" select (decimal?)obj.Quantidade).Sum() ?? 0m,
                QUANTIDADE_TOTAL = (from obj in this.SessionNHiBernate.Query<Dominio.Entidades.InformacaoCargaCTE>() where obj.CTE.Codigo == o.Codigo select (decimal?)obj.Quantidade).Sum() ?? 0m,
                LOCALIDADE_INICIO_PRESTACAO = o.LocalidadeInicioPrestacao.Descricao,
                ESTADO_INICIO_PRESTACAO = o.LocalidadeInicioPrestacao.Estado.Sigla,
                LOCALIDADE_TERMINO_PRESTACAO = o.LocalidadeTerminoPrestacao.Descricao,
                ESTADO_TERMINO_PRESTACAO = o.LocalidadeTerminoPrestacao.Estado.Sigla,
                CFOP = o.CFOP.CodigoCFOP,
                CHAVE = o.Chave,
                VALOR_MERCADORIA = o.ValorTotalMercadoria,
                PLACA = (from obj in this.SessionNHiBernate.Query<Dominio.Entidades.VeiculoCTE>() where obj.CTE.Codigo == o.Codigo select obj.Placa).FirstOrDefault() ?? string.Empty,
                CNPJ_TRANSPORTADOR = o.Empresa.CNPJ,
                TRANSPORTADOR = o.Empresa.RazaoSocial,
                CONDICAO_PAGAMENTO = o.TipoPagamento == Dominio.Enumeradores.TipoPagamento.Pago ? "CIF" : "FOB",
                NOTAS_FISCAIS = o.NumeroNotas,
                Usuario = o.Usuario != null ? !string.IsNullOrWhiteSpace(o.Usuario.Nome) ? o.Usuario.Nome : o.Usuario.Login : string.Empty,
                ALIQUOTA_PIS = o.AliquotaPIS,
                VALOR_PIS = o.ValorPIS,
                ALIQUOTA_COFINS = o.AliquotaCOFINS,
                VALOR_COFINS = o.ValorCOFINS,
                PRODUTO_PREDOMINANTE = o.ProdutoPredominanteCTe,
                NumeroDuplicata = (from obj in this.SessionNHiBernate.Query<Dominio.Entidades.DuplicataCtes>() where obj.ConhecimentoDeTransporteEletronico.Codigo == o.Codigo && obj.Duplicata.Status != "I" select (int?)obj.Duplicata.Numero).FirstOrDefault() ?? 0,
            }).OrderBy(o => o.TRANSPORTADOR)
              .ThenBy(o => o.CODIGO_CTE)
              .Timeout(120)
              .ToList();
        }

        public List<Dominio.ObjetosDeValor.Relatorios.RelatorioCTesEmitidosComExpedidor> RelatorioEmissaoComExpedidor(int codigoEmpresa, DateTime dataEmissaoInicial, DateTime dataEmissaoFinal, string status, Dominio.Enumeradores.TipoCTE tipoCTe, int codigoSerie, string cpfCnpjRemetente, string cpfCnpjExpedidor, string cpfCnpjRecebedor, string cpfCnpjDestinatario, string cpfCnpjTomador, int numeroInicial, int numeroFinal, string nomeMotorista, string cpfMotorista, string placaVeiculo, Dominio.Enumeradores.TipoAmbiente tipoAmbiente, int[] series, string tipoOcorrencia, string numeroNotaFiscal, Dominio.Enumeradores.StatusDuplicata? statusPagamento, int codigoLocalidadeInicioPrestacao, int codigoLocalidadeTerminoPrestacao, string ufInicio, string ufFim, string duplicata, bool importacao, bool exportacao, bool raizCNPJRemetente, bool raizCNPJExpedidor, bool raizCNPJRecebedor, bool raizCNPJDestinatario, bool raizCNPJTomador, DateTime dataAutorizacaoInicial, DateTime dataAutorizacaoFinal, string icmsCTe = "", Dominio.Enumeradores.TipoServico? tipoServico = null, string cstCTe = "0", int codigoUsuario = 0, string nomeUsuario = "", string cnpjEmbarcadorUsuario = "", string observacao = "", bool removerCliente = false, int numeroCarga = 0, int numeroUnidade = 0)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.ConhecimentoDeTransporteEletronico>();

            var result = from obj in query where obj.TipoAmbiente == tipoAmbiente select obj;

            result = result.Where(o => (o.ModeloDocumentoFiscal.Numero.Equals("57") || o.ModeloDocumentoFiscal.Numero.Equals("67")));

            if (codigoEmpresa > 0)
                result = result.Where(o => o.Empresa.Codigo == codigoEmpresa);

            if (dataEmissaoInicial != DateTime.MinValue)
                result = result.Where(o => o.DataEmissao >= dataEmissaoInicial.Date);

            if (dataEmissaoFinal != DateTime.MinValue)
                result = result.Where(o => o.DataEmissao < dataEmissaoFinal.AddDays(1).Date);

            if (dataAutorizacaoInicial != DateTime.MinValue)
                result = result.Where(o => o.DataRetornoSefaz >= dataAutorizacaoInicial.Date);

            if (dataAutorizacaoFinal != DateTime.MinValue)
                result = result.Where(o => o.DataRetornoSefaz < dataAutorizacaoFinal.AddDays(1).Date);

            if (codigoUsuario > 0 && !string.IsNullOrWhiteSpace(nomeUsuario))
                result = result.Where(o => o.Usuario.Codigo == codigoUsuario); // || o.Log.Contains(nomeUsuario)

            if (!string.IsNullOrWhiteSpace(status))
                result = result.Where(o => o.Status.Equals(status));

            if (tipoCTe != Dominio.Enumeradores.TipoCTE.Todos)
                result = result.Where(o => o.TipoCTE == tipoCTe);

            if (!string.IsNullOrWhiteSpace(cpfCnpjRemetente) && cpfCnpjRemetente != "0")
            {
                if (removerCliente)
                {
                    if (raizCNPJRemetente)
                        result = result.Where(o => !o.Remetente.CPF_CNPJ.Contains(cpfCnpjRemetente.Substring(0, 8)));
                    else
                        result = result.Where(o => o.Remetente.CPF_CNPJ != cpfCnpjRemetente);
                }
                else
                {
                    if (raizCNPJRemetente)
                        result = result.Where(o => o.Remetente.CPF_CNPJ.Contains(cpfCnpjRemetente.Substring(0, 8)));
                    else
                        result = result.Where(o => o.Remetente.CPF_CNPJ == cpfCnpjRemetente);
                }
            }

            if (!string.IsNullOrWhiteSpace(cnpjEmbarcadorUsuario))
            {
                if (removerCliente)
                {
                    if (cnpjEmbarcadorUsuario.Length == 8)
                        result = result.Where(o => !o.Remetente.CPF_CNPJ.Contains(cnpjEmbarcadorUsuario));
                    else
                        result = result.Where(o => o.Remetente.CPF_CNPJ != cnpjEmbarcadorUsuario);
                }
                else
                {
                    if (cnpjEmbarcadorUsuario.Length == 8)
                        result = result.Where(o => o.Remetente.CPF_CNPJ.Contains(cnpjEmbarcadorUsuario));
                    else
                        result = result.Where(o => o.Remetente.CPF_CNPJ == cnpjEmbarcadorUsuario);
                }
            }

            if (importacao)
                result = result.Where(o => o.Remetente.Exterior == true);

            if (!string.IsNullOrWhiteSpace(cpfCnpjDestinatario))
            {
                if (removerCliente)
                {
                    if (raizCNPJDestinatario)
                        result = result.Where(o => !o.Destinatario.CPF_CNPJ.Contains(cpfCnpjDestinatario.Substring(0, 8)));
                    else
                        result = result.Where(o => o.Destinatario.CPF_CNPJ != cpfCnpjDestinatario);
                }
                else
                {
                    if (raizCNPJDestinatario)
                        result = result.Where(o => o.Destinatario.CPF_CNPJ.Contains(cpfCnpjDestinatario.Substring(0, 8)));
                    else
                        result = result.Where(o => o.Destinatario.CPF_CNPJ == cpfCnpjDestinatario);
                }
            }

            if (exportacao)
                result = result.Where(o => o.Destinatario.Exterior == true);

            if (!string.IsNullOrWhiteSpace(cpfCnpjExpedidor))
            {
                if (removerCliente)
                {
                    if (raizCNPJExpedidor)
                        result = result.Where(o => !o.Expedidor.CPF_CNPJ.Contains(cpfCnpjExpedidor.Substring(0, 8)));
                    else
                        result = result.Where(o => o.Expedidor.CPF_CNPJ != cpfCnpjExpedidor);
                }
                else
                {
                    if (raizCNPJExpedidor)
                        result = result.Where(o => o.Expedidor.CPF_CNPJ.Contains(cpfCnpjExpedidor.Substring(0, 8)));
                    else
                        result = result.Where(o => o.Expedidor.CPF_CNPJ == cpfCnpjExpedidor);
                }
            }

            if (!string.IsNullOrWhiteSpace(cpfCnpjRecebedor))
            {
                if (removerCliente)
                {
                    if (raizCNPJRecebedor)
                        result = result.Where(o => !o.Recebedor.CPF_CNPJ.Contains(cpfCnpjRecebedor.Substring(0, 8)));
                    else
                        result = result.Where(o => o.Recebedor.CPF_CNPJ != cpfCnpjRecebedor);
                }
                else
                {
                    if (raizCNPJRecebedor)
                        result = result.Where(o => o.Recebedor.CPF_CNPJ.Contains(cpfCnpjRecebedor.Substring(0, 8)));
                    else
                        result = result.Where(o => o.Recebedor.CPF_CNPJ == cpfCnpjRecebedor);
                }
            }

            if (codigoLocalidadeInicioPrestacao > 0)
                result = result.Where(o => o.LocalidadeInicioPrestacao.Codigo == codigoLocalidadeInicioPrestacao);

            if (codigoLocalidadeTerminoPrestacao > 0)
                result = result.Where(o => o.LocalidadeTerminoPrestacao.Codigo == codigoLocalidadeTerminoPrestacao);

            if (!string.IsNullOrWhiteSpace(ufInicio))
                result = result.Where(o => o.LocalidadeInicioPrestacao.Estado.Sigla == ufInicio);

            if (!string.IsNullOrWhiteSpace(ufFim))
                result = result.Where(o => o.LocalidadeTerminoPrestacao.Estado.Sigla == ufFim);

            if (!string.IsNullOrWhiteSpace(cpfCnpjTomador))
            {
                if (removerCliente)
                {
                    if (raizCNPJTomador)
                        result = result.Where(o => !o.TomadorPagador.CPF_CNPJ.Contains(cpfCnpjTomador.Substring(0, 8)));
                    else
                        result = result.Where(o => o.TomadorPagador.CPF_CNPJ != cpfCnpjTomador);
                }
                else
                {
                    if (raizCNPJTomador)
                        result = result.Where(o => o.TomadorPagador.CPF_CNPJ.Contains(cpfCnpjTomador.Substring(0, 8)));
                    else
                        result = result.Where(o => o.TomadorPagador.CPF_CNPJ == cpfCnpjTomador);
                }
            }

            if (numeroInicial > 0)
                result = result.Where(o => o.Numero >= numeroInicial);

            if (numeroFinal > 0)
                result = result.Where(o => o.Numero <= numeroFinal);

            if (codigoSerie > 0)
                result = result.Where(o => o.Serie.Codigo == codigoSerie);
            else if (series != null && series.Count() > 0)
                result = result.Where(o => series.Contains(o.Serie.Codigo));

            if (!string.IsNullOrWhiteSpace(cpfMotorista) || !string.IsNullOrWhiteSpace(nomeMotorista))
            {
                var queryMotorista = this.SessionNHiBernate.Query<Dominio.Entidades.MotoristaCTE>();

                if (!string.IsNullOrWhiteSpace(cpfMotorista))
                    result = result.Where(o => (from obj in queryMotorista where obj.CPFMotorista.Equals(cpfMotorista) select obj.CTE.Codigo).Contains(o.Codigo));
                else
                    result = result.Where(o => (from obj in queryMotorista where obj.NomeMotoristaCTe.Contains(nomeMotorista) select obj.CTE.Codigo).Contains(o.Codigo));
            }

            if (!string.IsNullOrWhiteSpace(placaVeiculo))
            {
                var queryVeiculos = this.SessionNHiBernate.Query<Dominio.Entidades.VeiculoCTE>();

                result = result.Where(o => (from obj in queryVeiculos where obj.CTE.Codigo == o.Codigo && obj.Placa.Equals(placaVeiculo) select obj.CTE.Codigo).Contains(o.Codigo));
            }

            if (!string.IsNullOrWhiteSpace(numeroNotaFiscal))
            {
                var queryDocumentos = this.SessionNHiBernate.Query<Dominio.Entidades.DocumentosCTE>();

                result = result.Where(o => (from obj in queryDocumentos where obj.CTE.Codigo == o.Codigo && obj.Numero.Contains(numeroNotaFiscal) select obj.CTE.Codigo).Contains(o.Codigo));
            }

            if (!string.IsNullOrWhiteSpace(tipoOcorrencia))
            {
                var queryOcorrencias = this.SessionNHiBernate.Query<Dominio.Entidades.OcorrenciaDeCTe>();

                if (tipoOcorrencia == "F")
                    result = result.Where(o => (from obj in queryOcorrencias where obj.CTe.Codigo == o.Codigo && obj.Ocorrencia.Tipo.Equals("F") select obj.CTe.Codigo).Contains(o.Codigo));
                else
                    result = result.Where(o => !(from obj in queryOcorrencias where obj.CTe.Codigo == o.Codigo && obj.Ocorrencia.Tipo.Equals("F") select obj.CTe.Codigo).Contains(o.Codigo));
            }

            if (statusPagamento.HasValue)
            {
                var queryDuplicatas = this.SessionNHiBernate.Query<Dominio.Entidades.ParcelaCobrancaCTe>();

                result = result.Where(o => (from obj in queryDuplicatas select obj.Cobranca.CTe.Codigo).Contains(o.Codigo));

                if (statusPagamento.Value == Dominio.Enumeradores.StatusDuplicata.Paga)
                    result = result.Where(o => !(from obj in queryDuplicatas where obj.Status == Dominio.Enumeradores.StatusDuplicata.Pendente select obj.Cobranca.CTe.Codigo).Contains(o.Codigo));
                else
                    result = result.Where(o => (from obj in queryDuplicatas where obj.Status == Dominio.Enumeradores.StatusDuplicata.Pendente select obj.Cobranca.CTe.Codigo).Contains(o.Codigo));
            }

            if (!string.IsNullOrWhiteSpace(duplicata))
            {
                var queryDuplicata = this.SessionNHiBernate.Query<Dominio.Entidades.DuplicataCtes>();

                result = result.Where(o => (from obj in queryDuplicata where obj.Duplicata.Numero == int.Parse(duplicata) select obj.ConhecimentoDeTransporteEletronico.Codigo).Contains(o.Codigo));
            }

            if (icmsCTe == "0")
                result = result.Where(o => o.ValorICMS == 0);
            else
            if (icmsCTe == "1")
                result = result.Where(o => o.ValorICMS > 0);

            if (cstCTe != "0")
                result = result.Where(o => o.CST.Equals(cstCTe));

            if (tipoServico != null)
                result = result.Where(o => o.TipoServico == tipoServico);

            if (!string.IsNullOrWhiteSpace(observacao))
                result = result.Where(o => o.ObservacoesGerais.Contains(observacao));

            if (numeroCarga > 0 || numeroUnidade > 0)
            {
                var queryIntegracaoCTe = this.SessionNHiBernate.Query<Dominio.Entidades.IntegracaoCTe>();
                var resultIntegracaoCTe = from o in queryIntegracaoCTe select o;
                if (numeroCarga > 0)
                    resultIntegracaoCTe = resultIntegracaoCTe.Where(o => o.NumeroDaCarga == numeroCarga);
                if (numeroUnidade > 0)
                    resultIntegracaoCTe = resultIntegracaoCTe.Where(o => o.NumeroDaUnidade == numeroUnidade);

                result = result.Where(o => resultIntegracaoCTe.Select(c => c.CTe.Codigo).Contains(o.Codigo));
            }

            return result.Select(o => new Dominio.ObjetosDeValor.Relatorios.RelatorioCTesEmitidosComExpedidor()
            {
                CodigoCTe = o.Codigo,
                NumeroCTe = o.Numero,
                SerieCTe = o.Serie.Numero,
                DataEmissao = o.DataEmissao.Value,
                ChaveCTe = o.Chave,
                TipoCTe = o.TipoCTE,
                Status = o.Status,
                NomeTransportador = o.Empresa.RazaoSocial,
                CNPJTransportador = o.Empresa.CNPJ,
                UFTransportador = o.Empresa.Localidade.Estado.Sigla,
                NomeRemetente = o.Remetente != null ? o.Remetente.Nome : string.Empty,
                CNPJRemetente = o.Remetente != null ? o.Remetente.CPF_CNPJ : string.Empty,
                UFRemetente = o.Remetente != null ? o.Remetente.Localidade.Estado.Sigla : string.Empty,
                NomeExpedidor = o.Expedidor != null ? o.Expedidor.Nome : string.Empty,
                CNPJExpedidor = o.Expedidor != null ? o.Expedidor.CPF_CNPJ : string.Empty,
                UFExpedidor = o.Expedidor != null ? o.Expedidor.Localidade.Estado.Sigla : string.Empty,
                NomeRecebedor = o.Recebedor != null ? o.Recebedor.Nome : string.Empty,
                CNPJRecebedor = o.Recebedor != null ? o.Recebedor.CPF_CNPJ : string.Empty,
                UFRecebedor = o.Recebedor != null ? o.Recebedor.Localidade.Estado.Sigla : string.Empty,
                NomeDestinatario = o.Destinatario != null ? o.Destinatario.Nome : string.Empty,
                CNPJDestinatario = o.Destinatario != null ? o.Destinatario.CPF_CNPJ : string.Empty,
                UFDestinatario = o.Destinatario != null ? o.Destinatario.Localidade.Estado.Sigla : string.Empty,
                NomeTomador = o.TomadorPagador != null ? o.TomadorPagador.Nome : string.Empty,
                CNPJTomador = o.TomadorPagador != null ? o.TomadorPagador.CPF_CNPJ : string.Empty,
                UFTomador = o.TomadorPagador != null ? o.TomadorPagador.Localidade.Estado.Sigla : string.Empty,
                CST = o.CST,
                PercentualICMS = o.AliquotaICMS,
                ValorICMS = o.ValorICMS,
                BaseCalculoICMS = o.BaseCalculoICMS,
                ValorFrete = o.ValorFrete,
                ValorTotalReceber = o.ValorAReceber
            }).OrderBy(o => o.CNPJTransportador)
              .ThenBy(o => o.SerieCTe)
              .ThenBy(o => o.NumeroCTe)
              .Timeout(120)
              .ToList();
        }

        public List<Dominio.ObjetosDeValor.Relatorios.RelatorioCTesEmitidos> RelatorioEmissaoComMDFe(int codigoEmpresa, DateTime dataEmissaoInicial, DateTime dataEmissaoFinal, string status, Dominio.Enumeradores.TipoCTE tipoCTe, int codigoSerie, string cpfCnpjRemetente, string cpfCnpjExpedidor, string cpfCnpjRecebedor, string cpfCnpjDestinatario, string cpfCnpjTomador, int numeroInicial, int numeroFinal, string nomeMotorista, string cpfMotorista, string placaVeiculo, Dominio.Enumeradores.TipoAmbiente tipoAmbiente, int[] series, string tipoOcorrencia, string numeroNotaFiscal, Dominio.Enumeradores.StatusDuplicata? statusPagamento, int codigoLocalidadeInicioPrestacao, int codigoLocalidadeTerminoPrestacao, string ufInicio, string ufFim, string duplicata, bool importacao, bool exportacao, bool raizCNPJRemetente, bool raizCNPJExpedidor, bool raizCNPJRecebedor, bool raizCNPJDestinatario, bool raizCNPJTomador, DateTime dataAutorizacaoInicial, DateTime dataAutorizacaoFinal, string icmsCTe = "", Dominio.Enumeradores.TipoServico? tipoServico = null, string cstCTe = "0", int codigoUsuario = 0, string nomeUsuario = "", string cnpjEmbarcadorUsuario = "", string observacao = "", bool removerCliente = false, int numeroCarga = 0, int numeroUnidade = 0)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.ConhecimentoDeTransporteEletronico>();

            var result = from obj in query where obj.TipoAmbiente == tipoAmbiente select obj;

            result = result.Where(o => (o.ModeloDocumentoFiscal.Numero.Equals("57") || o.ModeloDocumentoFiscal.Numero.Equals("67")));

            if (codigoEmpresa > 0)
                result = result.Where(o => o.Empresa.Codigo == codigoEmpresa);

            if (dataEmissaoInicial != DateTime.MinValue)
                result = result.Where(o => o.DataEmissao >= dataEmissaoInicial.Date);

            if (dataEmissaoFinal != DateTime.MinValue)
                result = result.Where(o => o.DataEmissao < dataEmissaoFinal.AddDays(1).Date);

            if (dataAutorizacaoInicial != DateTime.MinValue)
                result = result.Where(o => o.DataRetornoSefaz >= dataAutorizacaoInicial.Date);

            if (dataAutorizacaoFinal != DateTime.MinValue)
                result = result.Where(o => o.DataRetornoSefaz < dataAutorizacaoFinal.AddDays(1).Date);

            if (codigoUsuario > 0 && !string.IsNullOrWhiteSpace(nomeUsuario))
                result = result.Where(o => o.Usuario.Codigo == codigoUsuario); // || o.Log.Contains(nomeUsuario)

            if (!string.IsNullOrWhiteSpace(status))
                result = result.Where(o => o.Status.Equals(status));

            if (tipoCTe != Dominio.Enumeradores.TipoCTE.Todos)
                result = result.Where(o => o.TipoCTE == tipoCTe);

            if (!string.IsNullOrWhiteSpace(cpfCnpjRemetente) && cpfCnpjRemetente != "0")
            {
                if (removerCliente)
                {
                    if (raizCNPJRemetente)
                        result = result.Where(o => !o.Remetente.CPF_CNPJ.Contains(cpfCnpjRemetente.Substring(0, 8)));
                    else
                        result = result.Where(o => o.Remetente.CPF_CNPJ != cpfCnpjRemetente);
                }
                else
                {
                    if (raizCNPJRemetente)
                        result = result.Where(o => o.Remetente.CPF_CNPJ.Contains(cpfCnpjRemetente.Substring(0, 8)));
                    else
                        result = result.Where(o => o.Remetente.CPF_CNPJ == cpfCnpjRemetente);
                }
            }

            if (!string.IsNullOrWhiteSpace(cnpjEmbarcadorUsuario))
            {
                if (removerCliente)
                {
                    if (cnpjEmbarcadorUsuario.Length == 8)
                        result = result.Where(o => !o.Remetente.CPF_CNPJ.Contains(cnpjEmbarcadorUsuario));
                    else
                        result = result.Where(o => o.Remetente.CPF_CNPJ != cnpjEmbarcadorUsuario);
                }
                else
                {
                    if (cnpjEmbarcadorUsuario.Length == 8)
                        result = result.Where(o => o.Remetente.CPF_CNPJ.Contains(cnpjEmbarcadorUsuario));
                    else
                        result = result.Where(o => o.Remetente.CPF_CNPJ == cnpjEmbarcadorUsuario);
                }
            }

            if (importacao)
                result = result.Where(o => o.Remetente.Exterior == true);

            if (!string.IsNullOrWhiteSpace(cpfCnpjDestinatario))
            {
                if (removerCliente)
                {
                    if (raizCNPJDestinatario)
                        result = result.Where(o => !o.Destinatario.CPF_CNPJ.Contains(cpfCnpjDestinatario.Substring(0, 8)));
                    else
                        result = result.Where(o => o.Destinatario.CPF_CNPJ != cpfCnpjDestinatario);
                }
                else
                {
                    if (raizCNPJDestinatario)
                        result = result.Where(o => o.Destinatario.CPF_CNPJ.Contains(cpfCnpjDestinatario.Substring(0, 8)));
                    else
                        result = result.Where(o => o.Destinatario.CPF_CNPJ == cpfCnpjDestinatario);
                }
            }

            if (exportacao)
                result = result.Where(o => o.Destinatario.Exterior == true);

            if (!string.IsNullOrWhiteSpace(cpfCnpjExpedidor))
            {
                if (removerCliente)
                {
                    if (raizCNPJExpedidor)
                        result = result.Where(o => !o.Expedidor.CPF_CNPJ.Contains(cpfCnpjExpedidor.Substring(0, 8)));
                    else
                        result = result.Where(o => o.Expedidor.CPF_CNPJ != cpfCnpjExpedidor);
                }
                else
                {
                    if (raizCNPJExpedidor)
                        result = result.Where(o => o.Expedidor.CPF_CNPJ.Contains(cpfCnpjExpedidor.Substring(0, 8)));
                    else
                        result = result.Where(o => o.Expedidor.CPF_CNPJ == cpfCnpjExpedidor);
                }
            }

            if (!string.IsNullOrWhiteSpace(cpfCnpjRecebedor))
            {
                if (removerCliente)
                {
                    if (raizCNPJRecebedor)
                        result = result.Where(o => !o.Recebedor.CPF_CNPJ.Contains(cpfCnpjRecebedor.Substring(0, 8)));
                    else
                        result = result.Where(o => o.Recebedor.CPF_CNPJ != cpfCnpjRecebedor);
                }
                else
                {
                    if (raizCNPJRecebedor)
                        result = result.Where(o => o.Recebedor.CPF_CNPJ.Contains(cpfCnpjRecebedor.Substring(0, 8)));
                    else
                        result = result.Where(o => o.Recebedor.CPF_CNPJ == cpfCnpjRecebedor);
                }
            }

            if (codigoLocalidadeInicioPrestacao > 0)
                result = result.Where(o => o.LocalidadeInicioPrestacao.Codigo == codigoLocalidadeInicioPrestacao);

            if (codigoLocalidadeTerminoPrestacao > 0)
                result = result.Where(o => o.LocalidadeTerminoPrestacao.Codigo == codigoLocalidadeTerminoPrestacao);

            if (!string.IsNullOrWhiteSpace(ufInicio))
                result = result.Where(o => o.LocalidadeInicioPrestacao.Estado.Sigla == ufInicio);

            if (!string.IsNullOrWhiteSpace(ufFim))
                result = result.Where(o => o.LocalidadeTerminoPrestacao.Estado.Sigla == ufFim);

            if (!string.IsNullOrWhiteSpace(cpfCnpjTomador))
            {
                if (removerCliente)
                {
                    if (raizCNPJTomador)
                        result = result.Where(o => !o.TomadorPagador.CPF_CNPJ.Contains(cpfCnpjTomador.Substring(0, 8)));
                    else
                        result = result.Where(o => o.TomadorPagador.CPF_CNPJ != cpfCnpjTomador);
                }
                else
                {
                    if (raizCNPJTomador)
                        result = result.Where(o => o.TomadorPagador.CPF_CNPJ.Contains(cpfCnpjTomador.Substring(0, 8)));
                    else
                        result = result.Where(o => o.TomadorPagador.CPF_CNPJ == cpfCnpjTomador);
                }
            }

            if (numeroInicial > 0)
                result = result.Where(o => o.Numero >= numeroInicial);

            if (numeroFinal > 0)
                result = result.Where(o => o.Numero <= numeroFinal);

            if (codigoSerie > 0)
                result = result.Where(o => o.Serie.Codigo == codigoSerie);
            else if (series != null && series.Count() > 0)
                result = result.Where(o => series.Contains(o.Serie.Codigo));

            if (!string.IsNullOrWhiteSpace(cpfMotorista) || !string.IsNullOrWhiteSpace(nomeMotorista))
            {
                var queryMotorista = this.SessionNHiBernate.Query<Dominio.Entidades.MotoristaCTE>();

                if (!string.IsNullOrWhiteSpace(cpfMotorista))
                    result = result.Where(o => (from obj in queryMotorista where obj.CPFMotorista.Equals(cpfMotorista) select obj.CTE.Codigo).Contains(o.Codigo));
                else
                    result = result.Where(o => (from obj in queryMotorista where obj.NomeMotoristaCTe.Contains(nomeMotorista) select obj.CTE.Codigo).Contains(o.Codigo));
            }

            if (!string.IsNullOrWhiteSpace(placaVeiculo))
            {
                var queryVeiculos = this.SessionNHiBernate.Query<Dominio.Entidades.VeiculoCTE>();

                result = result.Where(o => (from obj in queryVeiculos where obj.CTE.Codigo == o.Codigo && obj.Placa.Equals(placaVeiculo) select obj.CTE.Codigo).Contains(o.Codigo));
            }

            if (!string.IsNullOrWhiteSpace(numeroNotaFiscal))
            {
                var queryDocumentos = this.SessionNHiBernate.Query<Dominio.Entidades.DocumentosCTE>();

                result = result.Where(o => (from obj in queryDocumentos where obj.CTE.Codigo == o.Codigo && obj.Numero.Contains(numeroNotaFiscal) select obj.CTE.Codigo).Contains(o.Codigo));
            }

            if (!string.IsNullOrWhiteSpace(tipoOcorrencia))
            {
                var queryOcorrencias = this.SessionNHiBernate.Query<Dominio.Entidades.OcorrenciaDeCTe>();

                if (tipoOcorrencia == "F")
                    result = result.Where(o => (from obj in queryOcorrencias where obj.CTe.Codigo == o.Codigo && obj.Ocorrencia.Tipo.Equals("F") select obj.CTe.Codigo).Contains(o.Codigo));
                else
                    result = result.Where(o => !(from obj in queryOcorrencias where obj.CTe.Codigo == o.Codigo && obj.Ocorrencia.Tipo.Equals("F") select obj.CTe.Codigo).Contains(o.Codigo));
            }

            if (statusPagamento.HasValue)
            {
                var queryDuplicatas = this.SessionNHiBernate.Query<Dominio.Entidades.ParcelaCobrancaCTe>();

                result = result.Where(o => (from obj in queryDuplicatas select obj.Cobranca.CTe.Codigo).Contains(o.Codigo));

                if (statusPagamento.Value == Dominio.Enumeradores.StatusDuplicata.Paga)
                    result = result.Where(o => !(from obj in queryDuplicatas where obj.Status == Dominio.Enumeradores.StatusDuplicata.Pendente select obj.Cobranca.CTe.Codigo).Contains(o.Codigo));
                else
                    result = result.Where(o => (from obj in queryDuplicatas where obj.Status == Dominio.Enumeradores.StatusDuplicata.Pendente select obj.Cobranca.CTe.Codigo).Contains(o.Codigo));
            }

            if (!string.IsNullOrWhiteSpace(duplicata))
            {
                var queryDuplicata = this.SessionNHiBernate.Query<Dominio.Entidades.DuplicataCtes>();

                result = result.Where(o => (from obj in queryDuplicata where obj.Duplicata.Numero == int.Parse(duplicata) select obj.ConhecimentoDeTransporteEletronico.Codigo).Contains(o.Codigo));
            }

            if (icmsCTe == "0")
                result = result.Where(o => o.ValorICMS == 0);
            else
            if (icmsCTe == "1")
                result = result.Where(o => o.ValorICMS > 0);

            if (cstCTe != "0")
                result = result.Where(o => o.CST.Equals(cstCTe));

            if (tipoServico != null)
                result = result.Where(o => o.TipoServico == tipoServico);

            if (!string.IsNullOrWhiteSpace(observacao))
                result = result.Where(o => o.ObservacoesGerais.Contains(observacao));

            if (numeroCarga > 0 || numeroUnidade > 0)
            {
                var queryIntegracaoCTe = this.SessionNHiBernate.Query<Dominio.Entidades.IntegracaoCTe>();
                var resultIntegracaoCTe = from o in queryIntegracaoCTe select o;
                if (numeroCarga > 0)
                    resultIntegracaoCTe = resultIntegracaoCTe.Where(o => o.NumeroDaCarga == numeroCarga);
                if (numeroUnidade > 0)
                    resultIntegracaoCTe = resultIntegracaoCTe.Where(o => o.NumeroDaUnidade == numeroUnidade);

                result = result.Where(o => resultIntegracaoCTe.Select(c => c.CTe.Codigo).Contains(o.Codigo));
            }

            return result.Select(o => new Dominio.ObjetosDeValor.Relatorios.RelatorioCTesEmitidos()
            {
                CODIGO = o.Codigo,
                CODIGO_CTE = o.Numero,
                CST = o.CST,
                DATA_EMISSAO_CTE = o.DataEmissao.Value,
                DESTINATARIO_EXTERIOR = o.Destinatario != null ? o.Destinatario.Exterior : false,
                FRETE_ICMS = o.ValorFrete,
                UF_EMPRESA = o.Empresa.Localidade.Estado.Sigla,
                NOME_CIDADE_FIM = o.LocalidadeTerminoPrestacao.Descricao,
                NOME_CIDADE_FIM_EXTERIOR = o.Destinatario != null ? o.Destinatario.Cidade : string.Empty,
                NOME_CIDADE_INICIO = o.LocalidadeInicioPrestacao.Descricao,
                NOME_CIDADE_INICIO_EXTERIOR = o.Remetente != null ? o.Remetente.Cidade : string.Empty,
                NOME_RAZAO = o.Remetente != null ? o.Remetente.Nome : string.Empty,
                NOME_RAZAO_1 = o.Destinatario != null ? o.Destinatario.Nome : string.Empty,
                PAIS_FIM = o.Destinatario != null && o.Destinatario.Pais != null ? o.Destinatario.Pais.Nome : string.Empty,
                PAIS_INICIO = o.Remetente != null && o.Remetente.Pais != null ? o.Remetente.Pais.Nome : string.Empty,
                REMETENTE_EXTERIOR = o.Remetente != null ? o.Remetente.Exterior : false,
                SERIE = o.Serie.Numero,
                Status_Abrev = o.Status,
                TIPO_CTE = o.TipoCTE,
                UF_FIM = o.LocalidadeTerminoPrestacao.Estado.Sigla,
                UF_INICIO = o.LocalidadeInicioPrestacao.Estado.Sigla,
                BASE_CALCULO_ICMS = o.BaseCalculoICMS,
                ALIQUOTA_ICMS = o.AliquotaICMS,
                VALOR_ICMS = o.ValorICMS,
                VALOR_RECEBER = Math.Round(o.ValorAReceber, 2, MidpointRounding.ToEven),
                VALOR_SERVICO = o.ValorPrestacaoServico,
                QUANTIDADE = (from obj in this.SessionNHiBernate.Query<Dominio.Entidades.InformacaoCargaCTE>() where obj.CTE.Codigo == o.Codigo && obj.UnidadeMedida == "01" select (decimal?)obj.Quantidade).Sum() ?? 0m,
                QUANTIDADE_TOTAL = (from obj in this.SessionNHiBernate.Query<Dominio.Entidades.InformacaoCargaCTE>() where obj.CTE.Codigo == o.Codigo select (decimal?)obj.Quantidade).Sum() ?? 0m,
                LOCALIDADE_INICIO_PRESTACAO = o.LocalidadeInicioPrestacao.Descricao,
                ESTADO_INICIO_PRESTACAO = o.LocalidadeInicioPrestacao.Estado.Sigla,
                LOCALIDADE_TERMINO_PRESTACAO = o.LocalidadeTerminoPrestacao.Descricao,
                ESTADO_TERMINO_PRESTACAO = o.LocalidadeTerminoPrestacao.Estado.Sigla,
                CFOP = o.CFOP.CodigoCFOP,
                CHAVE = o.Chave,
                VALOR_MERCADORIA = o.ValorTotalMercadoria,
                CNPJ_TRANSPORTADOR = o.Empresa.CNPJ,
                TRANSPORTADOR = o.Empresa.RazaoSocial,
                CONDICAO_PAGAMENTO = o.TipoPagamento == Dominio.Enumeradores.TipoPagamento.Pago ? "CIF" : "FOB",
                NOTAS_FISCAIS = o.NumeroNotas,
                Usuario = o.Usuario != null ? !string.IsNullOrWhiteSpace(o.Usuario.Nome) ? o.Usuario.Nome : o.Usuario.Login : string.Empty,
                ALIQUOTA_PIS = o.AliquotaPIS,
                VALOR_PIS = o.ValorPIS,
                ALIQUOTA_COFINS = o.AliquotaCOFINS,
                VALOR_COFINS = o.ValorCOFINS,
                MDFe = o.ListaMDFes,
                MOTORISTA = o.ListaMotoristasMDFe,
                PLACA = o.ListaVeiculosMDFe,
                PLACAS_ADICIONAIS = o.ListaReboquesMDFe
            }).OrderBy(o => o.TRANSPORTADOR)
              .ThenBy(o => o.CODIGO_CTE)
              .Timeout(120)
              .ToList();
        }

        public List<Dominio.ObjetosDeValor.Relatorios.RelatorioCTesEmitidosFatura> RelatorioCTesEmitidosFatura(int codigoEmpresa, DateTime dataEmissaoInicial, DateTime dataEmissaoFinal, string status, Dominio.Enumeradores.TipoCTE tipoCTe, int codigoSerie, string cpfCnpjRemetente, string cpfCnpjExpedidor, string cpfCnpjRecebedor, string cpfCnpjDestinatario, string cpfCnpjTomador, int numeroInicial, int numeroFinal, string nomeMotorista, string cpfMotorista, string placaVeiculo, Dominio.Enumeradores.TipoAmbiente tipoAmbiente, int[] series, string tipoOcorrencia, string numeroNotaFiscal, Dominio.Enumeradores.StatusDuplicata? statusPagamento, int codigoLocalidadeInicioPrestacao, int codigoLocalidadeTerminoPrestacao, string ufInicio, string ufFim, string duplicata, bool importacao, bool exportacao, bool raizCNPJRemetente, bool raizCNPJExpedidor, bool raizCNPJRecebedor, bool raizCNPJDestinatario, bool raizCNPJTomador, DateTime dataAutorizacaoInicial, DateTime dataAutorizacaoFinal, string icmsCTe = "", Dominio.Enumeradores.TipoServico? tipoServico = null, string cstCTe = "0", int codigoUsuario = 0, string nomeUsuario = "", string cnpjEmbarcadorUsuario = "", string observacao = "", bool removerCliente = false, int numeroCarga = 0, int numeroUnidade = 0)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.ConhecimentoDeTransporteEletronico>();

            var result = from obj in query where obj.TipoAmbiente == tipoAmbiente select obj;

            result = result.Where(o => (o.ModeloDocumentoFiscal.Numero.Equals("57") || o.ModeloDocumentoFiscal.Numero.Equals("67")));

            if (codigoEmpresa > 0)
                result = result.Where(o => o.Empresa.Codigo == codigoEmpresa);

            if (dataEmissaoInicial != DateTime.MinValue)
                result = result.Where(o => o.DataEmissao >= dataEmissaoInicial.Date);

            if (dataEmissaoFinal != DateTime.MinValue)
                result = result.Where(o => o.DataEmissao < dataEmissaoFinal.AddDays(1).Date);

            if (dataAutorizacaoInicial != DateTime.MinValue)
                result = result.Where(o => o.DataRetornoSefaz >= dataAutorizacaoInicial.Date);

            if (dataAutorizacaoFinal != DateTime.MinValue)
                result = result.Where(o => o.DataRetornoSefaz < dataAutorizacaoFinal.AddDays(1).Date);

            if (codigoUsuario > 0 && !string.IsNullOrWhiteSpace(nomeUsuario))
                result = result.Where(o => o.Usuario.Codigo == codigoUsuario); // || o.Log.Contains(nomeUsuario)

            if (!string.IsNullOrWhiteSpace(status))
                result = result.Where(o => o.Status.Equals(status));

            if (tipoCTe != Dominio.Enumeradores.TipoCTE.Todos)
                result = result.Where(o => o.TipoCTE == tipoCTe);

            if (!string.IsNullOrWhiteSpace(cpfCnpjRemetente) && cpfCnpjRemetente != "0")
            {
                if (removerCliente)
                {
                    if (raizCNPJRemetente)
                        result = result.Where(o => !o.Remetente.CPF_CNPJ.Contains(cpfCnpjRemetente.Substring(0, 8)));
                    else
                        result = result.Where(o => o.Remetente.CPF_CNPJ != cpfCnpjRemetente);
                }
                else
                {
                    if (raizCNPJRemetente)
                        result = result.Where(o => o.Remetente.CPF_CNPJ.Contains(cpfCnpjRemetente.Substring(0, 8)));
                    else
                        result = result.Where(o => o.Remetente.CPF_CNPJ == cpfCnpjRemetente);
                }
            }

            if (!string.IsNullOrWhiteSpace(cnpjEmbarcadorUsuario))
            {
                if (removerCliente)
                {
                    if (cnpjEmbarcadorUsuario.Length == 8)
                        result = result.Where(o => !o.Remetente.CPF_CNPJ.Contains(cnpjEmbarcadorUsuario));
                    else
                        result = result.Where(o => o.Remetente.CPF_CNPJ != cnpjEmbarcadorUsuario);
                }
                else
                {
                    if (cnpjEmbarcadorUsuario.Length == 8)
                        result = result.Where(o => o.Remetente.CPF_CNPJ.Contains(cnpjEmbarcadorUsuario));
                    else
                        result = result.Where(o => o.Remetente.CPF_CNPJ == cnpjEmbarcadorUsuario);
                }
            }

            if (importacao)
                result = result.Where(o => o.Remetente.Exterior == true);

            if (!string.IsNullOrWhiteSpace(cpfCnpjDestinatario))
            {
                if (removerCliente)
                {
                    if (raizCNPJDestinatario)
                        result = result.Where(o => !o.Destinatario.CPF_CNPJ.Contains(cpfCnpjDestinatario.Substring(0, 8)));
                    else
                        result = result.Where(o => o.Destinatario.CPF_CNPJ != cpfCnpjDestinatario);
                }
                else
                {
                    if (raizCNPJDestinatario)
                        result = result.Where(o => o.Destinatario.CPF_CNPJ.Contains(cpfCnpjDestinatario.Substring(0, 8)));
                    else
                        result = result.Where(o => o.Destinatario.CPF_CNPJ == cpfCnpjDestinatario);
                }
            }

            if (exportacao)
                result = result.Where(o => o.Destinatario.Exterior == true);

            if (!string.IsNullOrWhiteSpace(cpfCnpjExpedidor))
            {
                if (removerCliente)
                {
                    if (raizCNPJExpedidor)
                        result = result.Where(o => !o.Expedidor.CPF_CNPJ.Contains(cpfCnpjExpedidor.Substring(0, 8)));
                    else
                        result = result.Where(o => o.Expedidor.CPF_CNPJ != cpfCnpjExpedidor);
                }
                else
                {
                    if (raizCNPJExpedidor)
                        result = result.Where(o => o.Expedidor.CPF_CNPJ.Contains(cpfCnpjExpedidor.Substring(0, 8)));
                    else
                        result = result.Where(o => o.Expedidor.CPF_CNPJ == cpfCnpjExpedidor);
                }
            }

            if (!string.IsNullOrWhiteSpace(cpfCnpjRecebedor))
            {
                if (removerCliente)
                {
                    if (raizCNPJRecebedor)
                        result = result.Where(o => !o.Recebedor.CPF_CNPJ.Contains(cpfCnpjRecebedor.Substring(0, 8)));
                    else
                        result = result.Where(o => o.Recebedor.CPF_CNPJ != cpfCnpjRecebedor);
                }
                else
                {
                    if (raizCNPJRecebedor)
                        result = result.Where(o => o.Recebedor.CPF_CNPJ.Contains(cpfCnpjRecebedor.Substring(0, 8)));
                    else
                        result = result.Where(o => o.Recebedor.CPF_CNPJ == cpfCnpjRecebedor);
                }
            }

            if (codigoLocalidadeInicioPrestacao > 0)
                result = result.Where(o => o.LocalidadeInicioPrestacao.Codigo == codigoLocalidadeInicioPrestacao);

            if (codigoLocalidadeTerminoPrestacao > 0)
                result = result.Where(o => o.LocalidadeTerminoPrestacao.Codigo == codigoLocalidadeTerminoPrestacao);

            if (!string.IsNullOrWhiteSpace(ufInicio))
                result = result.Where(o => o.LocalidadeInicioPrestacao.Estado.Sigla == ufInicio);

            if (!string.IsNullOrWhiteSpace(ufFim))
                result = result.Where(o => o.LocalidadeTerminoPrestacao.Estado.Sigla == ufFim);

            if (!string.IsNullOrWhiteSpace(cpfCnpjTomador))
            {
                if (removerCliente)
                {
                    if (raizCNPJTomador)
                        result = result.Where(o => !o.TomadorPagador.CPF_CNPJ.Contains(cpfCnpjTomador.Substring(0, 8)));
                    else
                        result = result.Where(o => o.TomadorPagador.CPF_CNPJ != cpfCnpjTomador);
                }
                else
                {
                    if (raizCNPJTomador)
                        result = result.Where(o => o.TomadorPagador.CPF_CNPJ.Contains(cpfCnpjTomador.Substring(0, 8)));
                    else
                        result = result.Where(o => o.TomadorPagador.CPF_CNPJ == cpfCnpjTomador);
                }
            }

            if (numeroInicial > 0)
                result = result.Where(o => o.Numero >= numeroInicial);

            if (numeroFinal > 0)
                result = result.Where(o => o.Numero <= numeroFinal);

            if (codigoSerie > 0)
                result = result.Where(o => o.Serie.Codigo == codigoSerie);
            else if (series != null && series.Count() > 0)
                result = result.Where(o => series.Contains(o.Serie.Codigo));

            if (!string.IsNullOrWhiteSpace(cpfMotorista) || !string.IsNullOrWhiteSpace(nomeMotorista))
            {
                var queryMotorista = this.SessionNHiBernate.Query<Dominio.Entidades.MotoristaCTE>();

                if (!string.IsNullOrWhiteSpace(cpfMotorista))
                    result = result.Where(o => (from obj in queryMotorista where obj.CPFMotorista.Equals(cpfMotorista) select obj.CTE.Codigo).Contains(o.Codigo));
                else
                    result = result.Where(o => (from obj in queryMotorista where obj.NomeMotoristaCTe.Contains(nomeMotorista) select obj.CTE.Codigo).Contains(o.Codigo));
            }

            if (!string.IsNullOrWhiteSpace(placaVeiculo))
            {
                var queryVeiculos = this.SessionNHiBernate.Query<Dominio.Entidades.VeiculoCTE>();

                result = result.Where(o => (from obj in queryVeiculos where obj.CTE.Codigo == o.Codigo && obj.Placa.Equals(placaVeiculo) select obj.CTE.Codigo).Contains(o.Codigo));
            }

            if (!string.IsNullOrWhiteSpace(numeroNotaFiscal))
            {
                var queryDocumentos = this.SessionNHiBernate.Query<Dominio.Entidades.DocumentosCTE>();

                result = result.Where(o => (from obj in queryDocumentos where obj.CTE.Codigo == o.Codigo && obj.Numero.Contains(numeroNotaFiscal) select obj.CTE.Codigo).Contains(o.Codigo));
            }

            if (!string.IsNullOrWhiteSpace(tipoOcorrencia))
            {
                var queryOcorrencias = this.SessionNHiBernate.Query<Dominio.Entidades.OcorrenciaDeCTe>();

                if (tipoOcorrencia == "F")
                    result = result.Where(o => (from obj in queryOcorrencias where obj.CTe.Codigo == o.Codigo && obj.Ocorrencia.Tipo.Equals("F") select obj.CTe.Codigo).Contains(o.Codigo));
                else
                    result = result.Where(o => !(from obj in queryOcorrencias where obj.CTe.Codigo == o.Codigo && obj.Ocorrencia.Tipo.Equals("F") select obj.CTe.Codigo).Contains(o.Codigo));
            }

            if (statusPagamento.HasValue)
            {
                var queryDuplicatas = this.SessionNHiBernate.Query<Dominio.Entidades.ParcelaCobrancaCTe>();

                result = result.Where(o => (from obj in queryDuplicatas select obj.Cobranca.CTe.Codigo).Contains(o.Codigo));

                if (statusPagamento.Value == Dominio.Enumeradores.StatusDuplicata.Paga)
                    result = result.Where(o => !(from obj in queryDuplicatas where obj.Status == Dominio.Enumeradores.StatusDuplicata.Pendente select obj.Cobranca.CTe.Codigo).Contains(o.Codigo));
                else
                    result = result.Where(o => (from obj in queryDuplicatas where obj.Status == Dominio.Enumeradores.StatusDuplicata.Pendente select obj.Cobranca.CTe.Codigo).Contains(o.Codigo));
            }

            if (!string.IsNullOrWhiteSpace(duplicata))
            {
                var queryDuplicata = this.SessionNHiBernate.Query<Dominio.Entidades.DuplicataCtes>();

                result = result.Where(o => (from obj in queryDuplicata where obj.Duplicata.Numero == int.Parse(duplicata) select obj.ConhecimentoDeTransporteEletronico.Codigo).Contains(o.Codigo));
            }

            if (icmsCTe == "0")
                result = result.Where(o => o.ValorICMS == 0);
            else
            if (icmsCTe == "1")
                result = result.Where(o => o.ValorICMS > 0);

            if (cstCTe != "0")
                result = result.Where(o => o.CST.Equals(cstCTe));

            if (tipoServico != null)
                result = result.Where(o => o.TipoServico == tipoServico);

            if (!string.IsNullOrWhiteSpace(observacao))
                result = result.Where(o => o.ObservacoesGerais.Contains(observacao));

            if (numeroCarga > 0 || numeroUnidade > 0)
            {
                var queryIntegracaoCTe = this.SessionNHiBernate.Query<Dominio.Entidades.IntegracaoCTe>();
                var resultIntegracaoCTe = from o in queryIntegracaoCTe select o;
                if (numeroCarga > 0)
                    resultIntegracaoCTe = resultIntegracaoCTe.Where(o => o.NumeroDaCarga == numeroCarga);
                if (numeroUnidade > 0)
                    resultIntegracaoCTe = resultIntegracaoCTe.Where(o => o.NumeroDaUnidade == numeroUnidade);

                result = result.Where(o => resultIntegracaoCTe.Select(c => c.CTe.Codigo).Contains(o.Codigo));
            }

            return result.Select(o => new Dominio.ObjetosDeValor.Relatorios.RelatorioCTesEmitidosFatura()
            {
                Fatura = string.Empty,
                NumeroCte = o.Numero,
                NumeroNFe = o.NumeroNotas,
                DataEmissao = o.DataEmissao.Value,
                CEPOrigem = o.Remetente != null ? o.Remetente.CEP : string.Empty,
                CEPDestino = o.Destinatario != null ? o.Destinatario.CEP : string.Empty,
                CidadeDestino = o.Destinatario != null && o.Destinatario.Localidade != null ? o.Destinatario.Localidade.Descricao : string.Empty,
                CNPJDestino = o.Destinatario != null ? o.Destinatario.CPF_CNPJ : string.Empty,
                ValorNFe = o.ValorTotalMercadoria,
                PesoKG = (from obj in this.SessionNHiBernate.Query<Dominio.Entidades.InformacaoCargaCTE>() where obj.CTE.Codigo == o.Codigo && obj.UnidadeMedida == "01" select (decimal?)obj.Quantidade).Sum() ?? 0m,
                PesoCubado = (from obj in this.SessionNHiBernate.Query<Dominio.Entidades.InformacaoCargaCTE>() where obj.CTE.Codigo == o.Codigo && obj.UnidadeMedida == "00" select (decimal?)obj.Quantidade).Sum() ?? 0m,
                FretePeso = (from obj in this.SessionNHiBernate.Query<Dominio.Entidades.ComponentePrestacaoCTE>() where obj.CTE.Codigo == o.Codigo && obj.NomeCTe.Equals("FRETE PESO") select (decimal?)obj.Valor).Sum() ?? 0m,
                FreteValor = o.ValorFrete,
                ValorTDA = (from obj in this.SessionNHiBernate.Query<Dominio.Entidades.ComponentePrestacaoCTE>() where obj.CTE.Codigo == o.Codigo && obj.NomeCTe.Equals("TDA") select (decimal?)obj.Valor).Sum() ?? 0m,
                ValorTDE = (from obj in this.SessionNHiBernate.Query<Dominio.Entidades.ComponentePrestacaoCTE>() where obj.CTE.Codigo == o.Codigo && obj.NomeCTe.Equals("TDE") select (decimal?)obj.Valor).Sum() ?? 0m,
                ValorTRT = (from obj in this.SessionNHiBernate.Query<Dominio.Entidades.ComponentePrestacaoCTE>() where obj.CTE.Codigo == o.Codigo && obj.NomeCTe.Equals("TRT") select (decimal?)obj.Valor).Sum() ?? 0m,
                ValorGRIS = (from obj in this.SessionNHiBernate.Query<Dominio.Entidades.ComponentePrestacaoCTE>() where obj.CTE.Codigo == o.Codigo && obj.NomeCTe.Equals("GRIS") select (decimal?)obj.Valor).Sum() ?? 0m,
                ValorTAS = (from obj in this.SessionNHiBernate.Query<Dominio.Entidades.ComponentePrestacaoCTE>() where obj.CTE.Codigo == o.Codigo && obj.NomeCTe.Equals("TAS") select (decimal?)obj.Valor).Sum() ?? 0m,
                ValorOutros = (from obj in this.SessionNHiBernate.Query<Dominio.Entidades.ComponentePrestacaoCTE>() where obj.CTE.Codigo == o.Codigo && obj.NomeCTe.Equals("OUTROS") select (decimal?)obj.Valor).Sum() ?? 0m,
                ValorPedagio = (from obj in this.SessionNHiBernate.Query<Dominio.Entidades.ComponentePrestacaoCTE>() where obj.CTE.Codigo == o.Codigo && obj.NomeCTe.Equals("PEDAGIO") select (decimal?)obj.Valor).Sum() ?? 0m,
                FreteTotal = o.ValorAReceber
            }).OrderBy(o => o.NumeroCte)
              .Timeout(120)
              .ToList();
        }

        public List<Dominio.ObjetosDeValor.Relatorios.RelatorioContratoYamaha> RelatorioContratoYamaha(int codigoEmpresa, DateTime dataEmissaoInicial, DateTime dataEmissaoFinal, string status, Dominio.Enumeradores.TipoCTE tipoCTe, int codigoSerie, string cpfCnpjRemetente, string cpfCnpjExpedidor, string cpfCnpjRecebedor, string cpfCnpjDestinatario, string cpfCnpjTomador, int numeroInicial, int numeroFinal, string nomeMotorista, string cpfMotorista, string placaVeiculo, Dominio.Enumeradores.TipoAmbiente tipoAmbiente, int[] series, string tipoOcorrencia, string numeroNotaFiscal, Dominio.Enumeradores.StatusDuplicata? statusPagamento, int codigoLocalidadeInicioPrestacao, int codigoLocalidadeTerminoPrestacao, string ufInicio, string ufFim, string duplicata, bool importacao, bool exportacao, bool raizCNPJRemetente, bool raizCNPJExpedidor, bool raizCNPJRecebedor, bool raizCNPJDestinatario, bool raizCNPJTomador, DateTime dataAutorizacaoInicial, DateTime dataAutorizacaoFinal, string icmsCTe = "", Dominio.Enumeradores.TipoServico? tipoServico = null, string cstCTe = "0", int codigoUsuario = 0, string nomeUsuario = "", string cnpjEmbarcadorUsuario = "", string observacao = "", bool removerCliente = false, int numeroCarga = 0, int numeroUnidade = 0)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Subcontratacao>();

            var result = from obj in query where obj.DocumentoSubcontratacao.TipoAmbiente == tipoAmbiente select obj;

            result = result.Where(o => (o.DocumentoSubcontratacao.ModeloDocumentoFiscal.Numero.Equals("57") || o.DocumentoSubcontratacao.ModeloDocumentoFiscal.Numero.Equals("67")));

            if (codigoEmpresa > 0)
                result = result.Where(o => o.DocumentoSubcontratacao.Empresa.Codigo == codigoEmpresa);

            if (dataEmissaoInicial != DateTime.MinValue)
                result = result.Where(o => o.DocumentoSubcontratacao.DataEmissao >= dataEmissaoInicial.Date);

            if (dataEmissaoFinal != DateTime.MinValue)
                result = result.Where(o => o.DocumentoSubcontratacao.DataEmissao < dataEmissaoFinal.AddDays(1).Date);

            if (dataAutorizacaoInicial != DateTime.MinValue)
                result = result.Where(o => o.DocumentoSubcontratacao.DataRetornoSefaz >= dataAutorizacaoInicial.Date);

            if (dataAutorizacaoFinal != DateTime.MinValue)
                result = result.Where(o => o.DocumentoSubcontratacao.DataRetornoSefaz < dataAutorizacaoFinal.AddDays(1).Date);

            if (codigoUsuario > 0 && !string.IsNullOrWhiteSpace(nomeUsuario))
                result = result.Where(o => o.DocumentoSubcontratacao.Usuario.Codigo == codigoUsuario); // || o.Log.Contains(nomeUsuario)

            if (!string.IsNullOrWhiteSpace(status))
                result = result.Where(o => o.DocumentoSubcontratacao.Status.Equals(status));

            if (tipoCTe != Dominio.Enumeradores.TipoCTE.Todos)
                result = result.Where(o => o.DocumentoSubcontratacao.TipoCTE == tipoCTe);

            if (!string.IsNullOrWhiteSpace(cpfCnpjRemetente) && cpfCnpjRemetente != "0")
            {
                if (removerCliente)
                {
                    if (raizCNPJRemetente)
                        result = result.Where(o => !o.DocumentoSubcontratacao.Remetente.CPF_CNPJ.Contains(cpfCnpjRemetente.Substring(0, 8)));
                    else
                        result = result.Where(o => o.DocumentoSubcontratacao.Remetente.CPF_CNPJ != cpfCnpjRemetente);
                }
                else
                {
                    if (raizCNPJRemetente)
                        result = result.Where(o => o.DocumentoSubcontratacao.Remetente.CPF_CNPJ.Contains(cpfCnpjRemetente.Substring(0, 8)));
                    else
                        result = result.Where(o => o.DocumentoSubcontratacao.Remetente.CPF_CNPJ == cpfCnpjRemetente);
                }
            }

            if (!string.IsNullOrWhiteSpace(cnpjEmbarcadorUsuario))
            {
                if (removerCliente)
                {
                    if (cnpjEmbarcadorUsuario.Length == 8)
                        result = result.Where(o => !o.DocumentoSubcontratacao.Remetente.CPF_CNPJ.Contains(cnpjEmbarcadorUsuario));
                    else
                        result = result.Where(o => o.DocumentoSubcontratacao.Remetente.CPF_CNPJ != cnpjEmbarcadorUsuario);
                }
                else
                {
                    if (cnpjEmbarcadorUsuario.Length == 8)
                        result = result.Where(o => o.DocumentoSubcontratacao.Remetente.CPF_CNPJ.Contains(cnpjEmbarcadorUsuario));
                    else
                        result = result.Where(o => o.DocumentoSubcontratacao.Remetente.CPF_CNPJ == cnpjEmbarcadorUsuario);
                }
            }

            if (importacao)
                result = result.Where(o => o.DocumentoSubcontratacao.Remetente.Exterior == true);

            if (!string.IsNullOrWhiteSpace(cpfCnpjDestinatario))
            {
                if (removerCliente)
                {
                    if (raizCNPJDestinatario)
                        result = result.Where(o => !o.DocumentoSubcontratacao.Destinatario.CPF_CNPJ.Contains(cpfCnpjDestinatario.Substring(0, 8)));
                    else
                        result = result.Where(o => o.DocumentoSubcontratacao.Destinatario.CPF_CNPJ != cpfCnpjDestinatario);
                }
                else
                {
                    if (raizCNPJDestinatario)
                        result = result.Where(o => o.DocumentoSubcontratacao.Destinatario.CPF_CNPJ.Contains(cpfCnpjDestinatario.Substring(0, 8)));
                    else
                        result = result.Where(o => o.DocumentoSubcontratacao.Destinatario.CPF_CNPJ == cpfCnpjDestinatario);
                }
            }

            if (exportacao)
                result = result.Where(o => o.DocumentoSubcontratacao.Destinatario.Exterior == true);

            if (!string.IsNullOrWhiteSpace(cpfCnpjExpedidor))
            {
                if (removerCliente)
                {
                    if (raizCNPJExpedidor)
                        result = result.Where(o => !o.DocumentoSubcontratacao.Expedidor.CPF_CNPJ.Contains(cpfCnpjExpedidor.Substring(0, 8)));
                    else
                        result = result.Where(o => o.DocumentoSubcontratacao.Expedidor.CPF_CNPJ != cpfCnpjExpedidor);
                }
                else
                {
                    if (raizCNPJExpedidor)
                        result = result.Where(o => o.DocumentoSubcontratacao.Expedidor.CPF_CNPJ.Contains(cpfCnpjExpedidor.Substring(0, 8)));
                    else
                        result = result.Where(o => o.DocumentoSubcontratacao.Expedidor.CPF_CNPJ == cpfCnpjExpedidor);
                }
            }

            if (!string.IsNullOrWhiteSpace(cpfCnpjRecebedor))
            {
                if (removerCliente)
                {
                    if (raizCNPJRecebedor)
                        result = result.Where(o => !o.DocumentoSubcontratacao.Recebedor.CPF_CNPJ.Contains(cpfCnpjRecebedor.Substring(0, 8)));
                    else
                        result = result.Where(o => o.DocumentoSubcontratacao.Recebedor.CPF_CNPJ != cpfCnpjRecebedor);
                }
                else
                {
                    if (raizCNPJRecebedor)
                        result = result.Where(o => o.DocumentoSubcontratacao.Recebedor.CPF_CNPJ.Contains(cpfCnpjRecebedor.Substring(0, 8)));
                    else
                        result = result.Where(o => o.DocumentoSubcontratacao.Recebedor.CPF_CNPJ == cpfCnpjRecebedor);
                }
            }

            if (codigoLocalidadeInicioPrestacao > 0)
                result = result.Where(o => o.DocumentoSubcontratacao.LocalidadeInicioPrestacao.Codigo == codigoLocalidadeInicioPrestacao);

            if (codigoLocalidadeTerminoPrestacao > 0)
                result = result.Where(o => o.DocumentoSubcontratacao.LocalidadeTerminoPrestacao.Codigo == codigoLocalidadeTerminoPrestacao);

            if (!string.IsNullOrWhiteSpace(ufInicio))
                result = result.Where(o => o.DocumentoSubcontratacao.LocalidadeInicioPrestacao.Estado.Sigla == ufInicio);

            if (!string.IsNullOrWhiteSpace(ufFim))
                result = result.Where(o => o.DocumentoSubcontratacao.LocalidadeTerminoPrestacao.Estado.Sigla == ufFim);

            if (!string.IsNullOrWhiteSpace(cpfCnpjTomador))
            {
                if (removerCliente)
                {
                    if (raizCNPJTomador)
                        result = result.Where(o => !o.DocumentoSubcontratacao.TomadorPagador.CPF_CNPJ.Contains(cpfCnpjTomador.Substring(0, 8)));
                    else
                        result = result.Where(o => o.DocumentoSubcontratacao.TomadorPagador.CPF_CNPJ != cpfCnpjTomador);
                }
                else
                {
                    if (raizCNPJTomador)
                        result = result.Where(o => o.DocumentoSubcontratacao.TomadorPagador.CPF_CNPJ.Contains(cpfCnpjTomador.Substring(0, 8)));
                    else
                        result = result.Where(o => o.DocumentoSubcontratacao.TomadorPagador.CPF_CNPJ == cpfCnpjTomador);
                }
            }

            if (numeroInicial > 0)
                result = result.Where(o => o.DocumentoSubcontratacao.Numero >= numeroInicial);

            if (numeroFinal > 0)
                result = result.Where(o => o.DocumentoSubcontratacao.Numero <= numeroFinal);

            if (codigoSerie > 0)
                result = result.Where(o => o.DocumentoSubcontratacao.Serie.Codigo == codigoSerie);
            else if (series != null && series.Count() > 0)
                result = result.Where(o => series.Contains(o.DocumentoSubcontratacao.Serie.Codigo));

            if (!string.IsNullOrWhiteSpace(cpfMotorista) || !string.IsNullOrWhiteSpace(nomeMotorista))
            {
                var queryMotorista = this.SessionNHiBernate.Query<Dominio.Entidades.MotoristaCTE>();

                if (!string.IsNullOrWhiteSpace(cpfMotorista))
                    result = result.Where(o => (from obj in queryMotorista where obj.CPFMotorista.Equals(cpfMotorista) select obj.CTE.Codigo).Contains(o.Codigo));
                else
                    result = result.Where(o => (from obj in queryMotorista where obj.NomeMotoristaCTe.Contains(nomeMotorista) select obj.CTE.Codigo).Contains(o.Codigo));
            }

            if (!string.IsNullOrWhiteSpace(placaVeiculo))
            {
                var queryVeiculos = this.SessionNHiBernate.Query<Dominio.Entidades.VeiculoCTE>();

                result = result.Where(o => (from obj in queryVeiculos where obj.CTE.Codigo == o.Codigo && obj.Placa.Equals(placaVeiculo) select obj.CTE.Codigo).Contains(o.Codigo));
            }

            if (!string.IsNullOrWhiteSpace(numeroNotaFiscal))
            {
                var queryDocumentos = this.SessionNHiBernate.Query<Dominio.Entidades.DocumentosCTE>();

                result = result.Where(o => (from obj in queryDocumentos where obj.CTE.Codigo == o.Codigo && obj.Numero.Contains(numeroNotaFiscal) select obj.CTE.Codigo).Contains(o.Codigo));
            }

            if (!string.IsNullOrWhiteSpace(tipoOcorrencia))
            {
                var queryOcorrencias = this.SessionNHiBernate.Query<Dominio.Entidades.OcorrenciaDeCTe>();

                if (tipoOcorrencia == "F")
                    result = result.Where(o => (from obj in queryOcorrencias where obj.CTe.Codigo == o.Codigo && obj.Ocorrencia.Tipo.Equals("F") select obj.CTe.Codigo).Contains(o.Codigo));
                else
                    result = result.Where(o => !(from obj in queryOcorrencias where obj.CTe.Codigo == o.Codigo && obj.Ocorrencia.Tipo.Equals("F") select obj.CTe.Codigo).Contains(o.Codigo));
            }

            if (statusPagamento.HasValue)
            {
                var queryDuplicatas = this.SessionNHiBernate.Query<Dominio.Entidades.ParcelaCobrancaCTe>();

                result = result.Where(o => (from obj in queryDuplicatas select obj.Cobranca.CTe.Codigo).Contains(o.Codigo));

                if (statusPagamento.Value == Dominio.Enumeradores.StatusDuplicata.Paga)
                    result = result.Where(o => !(from obj in queryDuplicatas where obj.Status == Dominio.Enumeradores.StatusDuplicata.Pendente select obj.Cobranca.CTe.Codigo).Contains(o.Codigo));
                else
                    result = result.Where(o => (from obj in queryDuplicatas where obj.Status == Dominio.Enumeradores.StatusDuplicata.Pendente select obj.Cobranca.CTe.Codigo).Contains(o.Codigo));
            }

            if (!string.IsNullOrWhiteSpace(duplicata))
            {
                var queryDuplicata = this.SessionNHiBernate.Query<Dominio.Entidades.DuplicataCtes>();

                result = result.Where(o => (from obj in queryDuplicata where obj.Duplicata.Numero == int.Parse(duplicata) select obj.ConhecimentoDeTransporteEletronico.Codigo).Contains(o.Codigo));
            }

            if (icmsCTe == "0")
                result = result.Where(o => o.DocumentoSubcontratacao.ValorICMS == 0);
            else
            if (icmsCTe == "1")
                result = result.Where(o => o.DocumentoSubcontratacao.ValorICMS > 0);

            if (cstCTe != "0")
                result = result.Where(o => o.DocumentoSubcontratacao.CST.Equals(cstCTe));

            if (tipoServico != null)
                result = result.Where(o => o.TipoServico == tipoServico);

            if (!string.IsNullOrWhiteSpace(observacao))
                result = result.Where(o => o.DocumentoSubcontratacao.ObservacoesGerais.Contains(observacao));

            if (numeroCarga > 0 || numeroUnidade > 0)
            {
                var queryIntegracaoCTe = this.SessionNHiBernate.Query<Dominio.Entidades.IntegracaoCTe>();
                var resultIntegracaoCTe = from o in queryIntegracaoCTe select o;
                if (numeroCarga > 0)
                    resultIntegracaoCTe = resultIntegracaoCTe.Where(o => o.NumeroDaCarga == numeroCarga);
                if (numeroUnidade > 0)
                    resultIntegracaoCTe = resultIntegracaoCTe.Where(o => o.NumeroDaUnidade == numeroUnidade);

                result = result.Where(o => resultIntegracaoCTe.Select(c => c.CTe.Codigo).Contains(o.Codigo));
            }

            return result.Select(o => new Dominio.ObjetosDeValor.Relatorios.RelatorioContratoYamaha()
            {
                Codigo = o.Codigo,
                NumeroCte = o.DocumentoSubcontratacao.Numero,
                SerieCTe = o.DocumentoSubcontratacao.Serie.Numero,
                ChaveCTe = o.DocumentoSubcontratacao.Chave,
                DataEmissaoCTe = o.DocumentoSubcontratacao.DataEmissao.Value.ToString("dd/MM/yyyy HH:mm"),
                TipoCTe = o.DocumentoSubcontratacao.TipoServico == Dominio.Enumeradores.TipoServico.Redespacho ? "Redespacho" : o.DocumentoSubcontratacao.TipoServico == Dominio.Enumeradores.TipoServico.SubContratacao ? "Subcontratação" : "Normal",
                CFOP = o.DocumentoSubcontratacao.CFOP.CodigoCFOP.ToString(),
                NomeTransportadora = o.DocumentoSubcontratacao.Empresa.RazaoSocial,
                CNPJTransportadora = o.DocumentoSubcontratacao.Empresa.CNPJ,
                CRTTransportadora = o.DocumentoSubcontratacao.Empresa.RegimeTributarioCTe == Dominio.ObjetosDeValor.Embarcador.Enumeradores.RegimeTributarioCTe.SimplesNacional ? "Simples Nacional" :
                                    o.DocumentoSubcontratacao.Empresa.RegimeTributarioCTe == Dominio.ObjetosDeValor.Embarcador.Enumeradores.RegimeTributarioCTe.SimplesNacionalMEI ? "Simples Nacional - MEI" :
                                    o.DocumentoSubcontratacao.Empresa.RegimeTributarioCTe == Dominio.ObjetosDeValor.Embarcador.Enumeradores.RegimeTributarioCTe.SimplesNacionalExcessoReceita ? "Simples Nacional - Excesso Receita" :
                                    o.DocumentoSubcontratacao.Empresa.RegimeTributarioCTe == Dominio.ObjetosDeValor.Embarcador.Enumeradores.RegimeTributarioCTe.RegimeNormal ? "Regime Normal" : "Outros",
                NomeRemetente = o.DocumentoSubcontratacao.Remetente.Nome,
                CNPJRemetente = o.DocumentoSubcontratacao.Remetente.CPF_CNPJ,
                MunicipioRemetente = o.DocumentoSubcontratacao.Remetente.Localidade.Descricao + "-" + o.DocumentoSubcontratacao.Remetente.Localidade.Estado.Sigla,
                NomeDestinatario = o.DocumentoSubcontratacao.Destinatario.Nome,
                CNPJDestinatario = o.DocumentoSubcontratacao.Destinatario.CPF_CNPJ,
                MunicipioDestinatario = o.DocumentoSubcontratacao.Destinatario.Localidade.Descricao + "-" + o.DocumentoSubcontratacao.Destinatario.Localidade.Estado.Sigla,
                NomeExpedidor = o.DocumentoSubcontratacao.Expedidor.Nome,
                CNPJExpedidor = o.DocumentoSubcontratacao.Expedidor.CPF_CNPJ,
                MunicipioExpedidor = o.DocumentoSubcontratacao.Expedidor.Localidade.Descricao + "-" + o.DocumentoSubcontratacao.Expedidor.Localidade.Estado.Sigla,
                NomeRecebedor = o.DocumentoSubcontratacao.Recebedor.Nome,
                CNPJRecebedor = o.DocumentoSubcontratacao.Recebedor.CPF_CNPJ,
                MunicipioRecebedor = o.DocumentoSubcontratacao.Recebedor.Localidade.Descricao + "-" + o.DocumentoSubcontratacao.Recebedor.Localidade.Estado.Sigla,
                NomeTomador = o.DocumentoSubcontratacao.TomadorPagador.Nome,
                CNPJTomador = o.DocumentoSubcontratacao.TomadorPagador.CPF_CNPJ,
                MunicipioTomador = o.DocumentoSubcontratacao.TomadorPagador.Localidade.Descricao + "-" + o.DocumentoSubcontratacao.LocalidadeInicioPrestacao.Estado.Sigla,
                MunicipioOrigem = o.DocumentoSubcontratacao.LocalidadeInicioPrestacao.Descricao + "-" + o.DocumentoSubcontratacao.LocalidadeInicioPrestacao.Estado.Sigla,
                MunicipioDestino = o.DocumentoSubcontratacao.LocalidadeTerminoPrestacao.Descricao + "-" + o.DocumentoSubcontratacao.LocalidadeTerminoPrestacao.Estado.Sigla,
                CST = o.DocumentoSubcontratacao.CST,
                ValorReceber = o.DocumentoSubcontratacao.ValorAReceber,
                ValorFrete = o.DocumentoSubcontratacao.ValorFrete,
                BaseICMS = o.DocumentoSubcontratacao.BaseCalculoICMS,
                AliquotaICMS = o.DocumentoSubcontratacao.AliquotaICMS,
                ValorICMS = o.DocumentoSubcontratacao.ValorICMS,
                //NumeroCTeAnterior
                //SerieCTeAnterior
                ChaveCTeAnterior = String.Empty,
                CodigoTransporte = o.CodigoProcessoTransporte,
                DataEmissaoContrato = o.DataEmissaoContrato,
                PercursoContrato = o.DescricaoPercurso
            }).OrderBy(o => o.NumeroCte)
              .Timeout(120)
              .ToList();
        }

        public List<Dominio.ObjetosDeValor.Relatorios.RelatorioCTesEmitidos> RelatorioEmissaoPorDestinatario(int codigoEmpresa, DateTime dataEmissaoInicial, DateTime dataEmissaoFinal, string status, Dominio.Enumeradores.TipoCTE tipoCTe, int codigoSerie, string cpfCnpjRemetente, string cpfCnpjExpedidor, string cpfCnpjRecebedor, string cpfCnpjDestinatario, string cpfCnpjTomador, int numeroInicial, int numeroFinal, string nomeMotorista, string cpfMotorista, string placaVeiculo, Dominio.Enumeradores.TipoAmbiente tipoAmbiente, int[] series, string tipoOcorrencia, string numeroNotaFiscal, Dominio.Enumeradores.StatusDuplicata? statusPagamento, int codigoLocalidadeInicioPrestacao, int codigoLocalidadeTerminoPrestacao, string ufInicio, string ufFim, string duplicata, bool importacao, bool exportacao, bool raizCNPJRemetente, bool raizCNPJExpedidor, bool raizCNPJRecebedor, bool raizCNPJDestinatario, bool raizCNPJTomador, string icmsCTe, Dominio.Enumeradores.TipoServico? tipoServico, string cstCTe, int codigoUsuario, string nomeUsuario, DateTime dataAutorizacaoInicial, DateTime dataAutorizacaoFinal, string observacao, bool removerCliente, int numeroCarga, int numeroUnidade)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.ConhecimentoDeTransporteEletronico>();

            var result = from obj in query where obj.Empresa.Codigo == codigoEmpresa && obj.TipoAmbiente == tipoAmbiente select obj;

            result = result.Where(o => (o.ModeloDocumentoFiscal.Numero.Equals("57") || o.ModeloDocumentoFiscal.Numero.Equals("67")));

            if (dataEmissaoInicial != DateTime.MinValue)
                result = result.Where(o => o.DataEmissao >= dataEmissaoInicial.Date);

            if (dataEmissaoFinal != DateTime.MinValue)
                result = result.Where(o => o.DataEmissao < dataEmissaoFinal.AddDays(1).Date);

            if (dataAutorizacaoInicial != DateTime.MinValue)
                result = result.Where(o => o.DataRetornoSefaz >= dataAutorizacaoInicial.Date);

            if (dataAutorizacaoFinal != DateTime.MinValue)
                result = result.Where(o => o.DataRetornoSefaz < dataAutorizacaoFinal.AddDays(1).Date);

            if (codigoUsuario > 0 && !string.IsNullOrWhiteSpace(nomeUsuario))
                result = result.Where(o => o.Usuario.Codigo == codigoUsuario);//|| o.Log.Contains(nomeUsuario)

            if (!string.IsNullOrWhiteSpace(status))
                result = result.Where(o => o.Status.Equals(status));

            if (tipoCTe != Dominio.Enumeradores.TipoCTE.Todos)
                result = result.Where(o => o.TipoCTE == tipoCTe);

            if (!string.IsNullOrWhiteSpace(cpfCnpjRemetente))
            {
                if (removerCliente)
                {
                    if (raizCNPJRemetente)
                        result = result.Where(o => !o.Remetente.CPF_CNPJ.Contains(cpfCnpjRemetente.Substring(0, 8)));
                    else
                        result = result.Where(o => o.Remetente.CPF_CNPJ != cpfCnpjRemetente);
                }
                else
                {
                    if (raizCNPJRemetente)
                        result = result.Where(o => o.Remetente.CPF_CNPJ.Contains(cpfCnpjRemetente.Substring(0, 8)));
                    else
                        result = result.Where(o => o.Remetente.CPF_CNPJ == cpfCnpjRemetente);
                }
            }

            if (importacao)
                result = result.Where(o => o.Remetente.Exterior == true);

            if (!string.IsNullOrWhiteSpace(cpfCnpjDestinatario))
            {
                if (removerCliente)
                {
                    if (raizCNPJDestinatario)
                        result = result.Where(o => !o.Destinatario.CPF_CNPJ.Contains(cpfCnpjDestinatario.Substring(0, 8)));
                    else
                        result = result.Where(o => o.Destinatario.CPF_CNPJ != cpfCnpjDestinatario);
                }
                else
                {
                    if (raizCNPJDestinatario)
                        result = result.Where(o => o.Destinatario.CPF_CNPJ.Contains(cpfCnpjDestinatario.Substring(0, 8)));
                    else
                        result = result.Where(o => o.Destinatario.CPF_CNPJ == cpfCnpjDestinatario);
                }
            }

            if (exportacao)
                result = result.Where(o => o.Destinatario.Exterior == true);

            if (!string.IsNullOrWhiteSpace(cpfCnpjExpedidor))
            {
                if (removerCliente)
                {
                    if (raizCNPJExpedidor)
                        result = result.Where(o => !o.Expedidor.CPF_CNPJ.Contains(cpfCnpjExpedidor.Substring(0, 8)));
                    else
                        result = result.Where(o => o.Expedidor.CPF_CNPJ != cpfCnpjExpedidor);
                }
                else
                {
                    if (raizCNPJExpedidor)
                        result = result.Where(o => o.Expedidor.CPF_CNPJ.Contains(cpfCnpjExpedidor.Substring(0, 8)));
                    else
                        result = result.Where(o => o.Expedidor.CPF_CNPJ == cpfCnpjExpedidor);
                }
            }

            if (!string.IsNullOrWhiteSpace(cpfCnpjRecebedor))
            {
                if (removerCliente)
                {
                    if (raizCNPJRecebedor)
                        result = result.Where(o => !o.Recebedor.CPF_CNPJ.Contains(cpfCnpjRecebedor.Substring(0, 8)));
                    else
                        result = result.Where(o => o.Recebedor.CPF_CNPJ != cpfCnpjRecebedor);
                }
                else
                {
                    if (raizCNPJRecebedor)
                        result = result.Where(o => o.Recebedor.CPF_CNPJ.Contains(cpfCnpjRecebedor.Substring(0, 8)));
                    else
                        result = result.Where(o => o.Recebedor.CPF_CNPJ == cpfCnpjRecebedor);
                }
            }

            if (codigoLocalidadeInicioPrestacao > 0)
                result = result.Where(o => o.LocalidadeInicioPrestacao.Codigo == codigoLocalidadeInicioPrestacao);

            if (codigoLocalidadeTerminoPrestacao > 0)
                result = result.Where(o => o.LocalidadeTerminoPrestacao.Codigo == codigoLocalidadeTerminoPrestacao);

            if (!string.IsNullOrWhiteSpace(ufInicio))
                result = result.Where(o => o.LocalidadeInicioPrestacao.Estado.Sigla == ufInicio);

            if (!string.IsNullOrWhiteSpace(ufFim))
                result = result.Where(o => o.LocalidadeTerminoPrestacao.Estado.Sigla == ufFim);

            if (!string.IsNullOrWhiteSpace(cpfCnpjTomador))
            {
                //if (raizCNPJTomador)
                //    result = result.Where(o => (o.TipoTomador == Dominio.Enumeradores.TipoTomador.Destinatario && o.Destinatario.CPF_CNPJ.Contains(cpfCnpjTomador.Substring(0, 8))) ||
                //                           (o.TipoTomador == Dominio.Enumeradores.TipoTomador.Expedidor && o.Expedidor.CPF_CNPJ.Contains(cpfCnpjTomador.Substring(0, 8))) ||
                //                           (o.TipoTomador == Dominio.Enumeradores.TipoTomador.Outros && o.OutrosTomador.CPF_CNPJ.Contains(cpfCnpjTomador.Substring(0, 8))) ||
                //                           (o.TipoTomador == Dominio.Enumeradores.TipoTomador.Recebedor && o.Recebedor.CPF_CNPJ.Contains(cpfCnpjTomador.Substring(0, 8))) ||
                //                           (o.TipoTomador == Dominio.Enumeradores.TipoTomador.Remetente && o.Remetente.CPF_CNPJ.Contains(cpfCnpjTomador.Substring(0, 8))));
                //else
                //    result = result.Where(o => (o.TipoTomador == Dominio.Enumeradores.TipoTomador.Destinatario && o.Destinatario.CPF_CNPJ == cpfCnpjTomador) ||
                //                               (o.TipoTomador == Dominio.Enumeradores.TipoTomador.Expedidor && o.Expedidor.CPF_CNPJ == cpfCnpjTomador) ||
                //                               (o.TipoTomador == Dominio.Enumeradores.TipoTomador.Outros && o.OutrosTomador.CPF_CNPJ == cpfCnpjTomador) ||
                //                               (o.TipoTomador == Dominio.Enumeradores.TipoTomador.Recebedor && o.Recebedor.CPF_CNPJ == cpfCnpjTomador) ||
                //                               (o.TipoTomador == Dominio.Enumeradores.TipoTomador.Remetente && o.Remetente.CPF_CNPJ == cpfCnpjTomador));
                if (removerCliente)
                {
                    if (raizCNPJTomador)
                        result = result.Where(o => !o.TomadorPagador.CPF_CNPJ.Contains(cpfCnpjTomador.Substring(0, 8)));
                    else
                        result = result.Where(o => o.TomadorPagador.CPF_CNPJ != cpfCnpjTomador);
                }
                else
                {
                    if (raizCNPJTomador)
                        result = result.Where(o => o.TomadorPagador.CPF_CNPJ.Contains(cpfCnpjTomador.Substring(0, 8)));
                    else
                        result = result.Where(o => o.TomadorPagador.CPF_CNPJ == cpfCnpjTomador);
                }
            }

            if (numeroInicial > 0)
                result = result.Where(o => o.Numero >= numeroInicial);

            if (numeroFinal > 0)
                result = result.Where(o => o.Numero <= numeroFinal);

            if (codigoSerie > 0)
                result = result.Where(o => o.Serie.Codigo == codigoSerie);
            else if (series.Count() > 0)
                result = result.Where(o => series.Contains(o.Serie.Codigo));

            if (!string.IsNullOrWhiteSpace(cpfMotorista) || !string.IsNullOrWhiteSpace(nomeMotorista))
            {
                var queryMotorista = this.SessionNHiBernate.Query<Dominio.Entidades.MotoristaCTE>();

                if (!string.IsNullOrWhiteSpace(cpfMotorista))
                    result = result.Where(o => (from obj in queryMotorista where obj.CPFMotorista.Equals(cpfMotorista) select obj.CTE.Codigo).Contains(o.Codigo));
                else
                    result = result.Where(o => (from obj in queryMotorista where obj.NomeMotoristaCTe.Contains(nomeMotorista) select obj.CTE.Codigo).Contains(o.Codigo));
            }

            if (!string.IsNullOrWhiteSpace(placaVeiculo))
            {
                var queryVeiculos = this.SessionNHiBernate.Query<Dominio.Entidades.VeiculoCTE>();

                result = result.Where(o => (from obj in queryVeiculos where obj.CTE.Codigo == o.Codigo && obj.Placa.Equals(placaVeiculo) select obj.CTE.Codigo).Contains(o.Codigo));
            }

            if (!string.IsNullOrWhiteSpace(numeroNotaFiscal))
            {
                var queryDocumentos = this.SessionNHiBernate.Query<Dominio.Entidades.DocumentosCTE>();

                result = result.Where(o => (from obj in queryDocumentos where obj.CTE.Codigo == o.Codigo && obj.Numero.Contains(numeroNotaFiscal) select obj.CTE.Codigo).Contains(o.Codigo));
            }

            if (!string.IsNullOrWhiteSpace(tipoOcorrencia))
            {
                var queryOcorrencias = this.SessionNHiBernate.Query<Dominio.Entidades.OcorrenciaDeCTe>();

                if (tipoOcorrencia == "F")
                    result = result.Where(o => (from obj in queryOcorrencias where obj.CTe.Codigo == o.Codigo && obj.Ocorrencia.Tipo.Equals("F") select obj.CTe.Codigo).Contains(o.Codigo));
                else
                    result = result.Where(o => !(from obj in queryOcorrencias where obj.CTe.Codigo == o.Codigo && obj.Ocorrencia.Tipo.Equals("F") select obj.CTe.Codigo).Contains(o.Codigo));
            }

            if (statusPagamento.HasValue)
            {
                var queryDuplicatas = this.SessionNHiBernate.Query<Dominio.Entidades.ParcelaCobrancaCTe>();

                result = result.Where(o => (from obj in queryDuplicatas select obj.Cobranca.CTe.Codigo).Contains(o.Codigo));

                if (statusPagamento.Value == Dominio.Enumeradores.StatusDuplicata.Paga)
                    result = result.Where(o => !(from obj in queryDuplicatas where obj.Status == Dominio.Enumeradores.StatusDuplicata.Pendente select obj.Cobranca.CTe.Codigo).Contains(o.Codigo));
                else
                    result = result.Where(o => (from obj in queryDuplicatas where obj.Status == Dominio.Enumeradores.StatusDuplicata.Pendente select obj.Cobranca.CTe.Codigo).Contains(o.Codigo));
            }

            if (!string.IsNullOrWhiteSpace(duplicata))
            {
                var queryDuplicata = this.SessionNHiBernate.Query<Dominio.Entidades.DuplicataCtes>();

                result = result.Where(o => (from obj in queryDuplicata where obj.Duplicata.Numero == int.Parse(duplicata) select obj.ConhecimentoDeTransporteEletronico.Codigo).Contains(o.Codigo));
            }

            if (icmsCTe == "0")
                result = result.Where(o => o.ValorICMS == 0);
            else
            if (icmsCTe == "1")
                result = result.Where(o => o.ValorICMS > 0);

            if (cstCTe != "0")
                result = result.Where(o => o.CST.Equals(cstCTe));

            if (tipoServico != null)
                result = result.Where(o => o.TipoServico == tipoServico);

            if (!string.IsNullOrWhiteSpace(observacao))
                result = result.Where(o => o.ObservacoesGerais.Contains(observacao));

            if (numeroCarga > 0 || numeroUnidade > 0)
            {
                var queryIntegracaoCTe = this.SessionNHiBernate.Query<Dominio.Entidades.IntegracaoCTe>();
                var resultIntegracaoCTe = from o in queryIntegracaoCTe select o;
                if (numeroCarga > 0)
                    resultIntegracaoCTe = resultIntegracaoCTe.Where(o => o.NumeroDaCarga == numeroCarga);
                if (numeroUnidade > 0)
                    resultIntegracaoCTe = resultIntegracaoCTe.Where(o => o.NumeroDaUnidade == numeroUnidade);

                result = result.Where(o => resultIntegracaoCTe.Select(c => c.CTe.Codigo).Contains(o.Codigo));
            }

            return result.Select(o => new Dominio.ObjetosDeValor.Relatorios.RelatorioCTesEmitidos()
            {
                CODIGO = o.Codigo,
                CODIGO_CTE = o.Numero,
                CST = o.CST,
                DATA_EMISSAO_CTE = o.DataEmissao.Value,
                DESTINATARIO_EXTERIOR = o.Destinatario != null ? o.Destinatario.Exterior : false,
                FRETE_ICMS = o.ValorFrete,
                UF_EMPRESA = o.Empresa.Localidade.Estado.Sigla,
                NOME_CIDADE_FIM = o.LocalidadeTerminoPrestacao.Descricao,
                NOME_CIDADE_FIM_EXTERIOR = o.Destinatario != null ? o.Destinatario.Cidade : string.Empty,
                NOME_CIDADE_INICIO = o.LocalidadeInicioPrestacao.Descricao,
                NOME_CIDADE_INICIO_EXTERIOR = o.Remetente != null ? o.Remetente.Cidade : string.Empty,
                NOME_RAZAO = o.Remetente != null ? o.Remetente.CPF_CNPJ + " " + o.Remetente.Nome : string.Empty,
                NOME_RAZAO_1 = o.Destinatario != null ? o.Destinatario.CPF_CNPJ + " " + o.Destinatario.Nome : string.Empty,
                PAIS_FIM = o.Destinatario != null && o.Destinatario.Pais != null ? o.Destinatario.Pais.Nome : string.Empty,
                PAIS_INICIO = o.Remetente != null && o.Remetente.Pais != null ? o.Remetente.Pais.Nome : string.Empty,
                REMETENTE_EXTERIOR = o.Remetente != null ? o.Remetente.Exterior : false,
                SERIE = o.Serie.Numero,
                Status_Abrev = o.Status,
                TIPO_CTE = o.TipoCTE,
                UF_FIM = o.LocalidadeTerminoPrestacao.Estado.Sigla,
                UF_INICIO = o.LocalidadeInicioPrestacao.Estado.Sigla,
                BASE_CALCULO_ICMS = o.BaseCalculoICMS,
                ALIQUOTA_ICMS = o.AliquotaICMS,
                VALOR_ICMS = o.ValorICMS,
                VALOR_RECEBER = Math.Round(o.ValorAReceber, 2, MidpointRounding.ToEven),
                VALOR_SERVICO = o.ValorPrestacaoServico,
                QUANTIDADE = (from obj in this.SessionNHiBernate.Query<Dominio.Entidades.InformacaoCargaCTE>() where obj.CTE.Codigo == o.Codigo && obj.UnidadeMedida == "01" select (decimal?)obj.Quantidade).Sum() ?? 0m,
                QUANTIDADE_TOTAL = (from obj in this.SessionNHiBernate.Query<Dominio.Entidades.InformacaoCargaCTE>() where obj.CTE.Codigo == o.Codigo select (decimal?)obj.Quantidade).Sum() ?? 0m,
                LOCALIDADE_INICIO_PRESTACAO = o.LocalidadeInicioPrestacao.Descricao,
                ESTADO_INICIO_PRESTACAO = o.LocalidadeInicioPrestacao.Estado.Sigla,
                LOCALIDADE_TERMINO_PRESTACAO = o.LocalidadeTerminoPrestacao.Descricao,
                ESTADO_TERMINO_PRESTACAO = o.LocalidadeTerminoPrestacao.Estado.Sigla,
                CFOP = o.CFOP.CodigoCFOP,
                CHAVE = o.Chave,
                VALOR_MERCADORIA = o.ValorTotalMercadoria,
                PLACA = (from obj in this.SessionNHiBernate.Query<Dominio.Entidades.VeiculoCTE>() where obj.CTE.Codigo == o.Codigo select obj.Placa).FirstOrDefault() ?? string.Empty // o.Veiculos != null && o.Veiculos.Count > 0 ? o.Veiculos.FirstOrDefault().Placa : string.Empty
            }).OrderBy(o => o.NOME_RAZAO_1).Timeout(120).ToList();
        }

        public List<Dominio.ObjetosDeValor.Relatorios.RelatorioCTesEmitidos> RelatorioEmissaoPorTomador(int codigoEmpresa, DateTime dataEmissaoInicial, DateTime dataEmissaoFinal, string status, Dominio.Enumeradores.TipoCTE tipoCTe, int codigoSerie, string cpfCnpjRemetente, string cpfCnpjExpedidor, string cpfCnpjRecebedor, string cpfCnpjDestinatario, string cpfCnpjTomador, int numeroInicial, int numeroFinal, string nomeMotorista, string cpfMotorista, string placaVeiculo, Dominio.Enumeradores.TipoAmbiente tipoAmbiente, int[] series, string tipoOcorrencia, string numeroNotaFiscal, Dominio.Enumeradores.StatusDuplicata? statusPagamento, int codigoLocalidadeInicioPrestacao, int codigoLocalidadeTerminoPrestacao, string ufInicio, string ufFim, string duplicata, bool importacao, bool exportacao, bool raizCNPJRemetente, bool raizCNPJExpedidor, bool raizCNPJRecebedor, bool raizCNPJDestinatario, bool raizCNPJTomador, string icmsCTe, Dominio.Enumeradores.TipoServico? tipoServico, string cstCTe, int codigoUsuario, string nomeUsuario, DateTime dataAutorizacaoInicial, DateTime dataAutorizacaoFinal, string observacao, bool removerCliente, int numeroCarga, int numeroUnidade)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.ConhecimentoDeTransporteEletronico>();

            var result = from obj in query where obj.Empresa.Codigo == codigoEmpresa && obj.TipoAmbiente == tipoAmbiente select obj;

            result = result.Where(o => (o.ModeloDocumentoFiscal.Numero.Equals("57") || o.ModeloDocumentoFiscal.Numero.Equals("67")));

            if (dataEmissaoInicial != DateTime.MinValue)
                result = result.Where(o => o.DataEmissao >= dataEmissaoInicial.Date);

            if (dataEmissaoFinal != DateTime.MinValue)
                result = result.Where(o => o.DataEmissao < dataEmissaoFinal.AddDays(1).Date);

            if (dataAutorizacaoInicial != DateTime.MinValue)
                result = result.Where(o => o.DataRetornoSefaz >= dataAutorizacaoInicial.Date);

            if (dataAutorizacaoFinal != DateTime.MinValue)
                result = result.Where(o => o.DataRetornoSefaz < dataAutorizacaoFinal.AddDays(1).Date);

            if (codigoUsuario > 0 && !string.IsNullOrWhiteSpace(nomeUsuario))
                result = result.Where(o => o.Usuario.Codigo == codigoUsuario);//|| o.Log.Contains(nomeUsuario)

            if (!string.IsNullOrWhiteSpace(status))
                result = result.Where(o => o.Status.Equals(status));

            if (tipoCTe != Dominio.Enumeradores.TipoCTE.Todos)
                result = result.Where(o => o.TipoCTE == tipoCTe);

            if (!string.IsNullOrWhiteSpace(cpfCnpjRemetente))
            {
                if (removerCliente)
                {
                    if (raizCNPJRemetente)
                        result = result.Where(o => !o.Remetente.CPF_CNPJ.Contains(cpfCnpjRemetente.Substring(0, 8)));
                    else
                        result = result.Where(o => o.Remetente.CPF_CNPJ != cpfCnpjRemetente);
                }
                else
                {
                    if (raizCNPJRemetente)
                        result = result.Where(o => o.Remetente.CPF_CNPJ.Contains(cpfCnpjRemetente.Substring(0, 8)));
                    else
                        result = result.Where(o => o.Remetente.CPF_CNPJ == cpfCnpjRemetente);
                }
            }

            if (importacao)
                result = result.Where(o => o.Remetente.Exterior == true);

            if (!string.IsNullOrWhiteSpace(cpfCnpjDestinatario))
            {
                if (removerCliente)
                {
                    if (raizCNPJDestinatario)
                        result = result.Where(o => !o.Destinatario.CPF_CNPJ.Contains(cpfCnpjDestinatario.Substring(0, 8)));
                    else
                        result = result.Where(o => o.Destinatario.CPF_CNPJ != cpfCnpjDestinatario);
                }
                else
                {
                    if (raizCNPJDestinatario)
                        result = result.Where(o => o.Destinatario.CPF_CNPJ.Contains(cpfCnpjDestinatario.Substring(0, 8)));
                    else
                        result = result.Where(o => o.Destinatario.CPF_CNPJ == cpfCnpjDestinatario);
                }
            }

            if (exportacao)
                result = result.Where(o => o.Destinatario.Exterior == true);

            if (!string.IsNullOrWhiteSpace(cpfCnpjExpedidor))
            {
                if (removerCliente)
                {
                    if (raizCNPJExpedidor)
                        result = result.Where(o => !o.Expedidor.CPF_CNPJ.Contains(cpfCnpjExpedidor.Substring(0, 8)));
                    else
                        result = result.Where(o => o.Expedidor.CPF_CNPJ != cpfCnpjExpedidor);
                }
                else
                {
                    if (raizCNPJExpedidor)
                        result = result.Where(o => o.Expedidor.CPF_CNPJ.Contains(cpfCnpjExpedidor.Substring(0, 8)));
                    else
                        result = result.Where(o => o.Expedidor.CPF_CNPJ == cpfCnpjExpedidor);
                }
            }

            if (!string.IsNullOrWhiteSpace(cpfCnpjRecebedor))
            {
                if (removerCliente)
                {
                    if (raizCNPJRecebedor)
                        result = result.Where(o => !o.Recebedor.CPF_CNPJ.Contains(cpfCnpjRecebedor.Substring(0, 8)));
                    else
                        result = result.Where(o => o.Recebedor.CPF_CNPJ != cpfCnpjRecebedor);
                }
                else
                {
                    if (raizCNPJRecebedor)
                        result = result.Where(o => o.Recebedor.CPF_CNPJ.Contains(cpfCnpjRecebedor.Substring(0, 8)));
                    else
                        result = result.Where(o => o.Recebedor.CPF_CNPJ == cpfCnpjRecebedor);
                }
            }

            if (codigoLocalidadeInicioPrestacao > 0)
                result = result.Where(o => o.LocalidadeInicioPrestacao.Codigo == codigoLocalidadeInicioPrestacao);

            if (codigoLocalidadeTerminoPrestacao > 0)
                result = result.Where(o => o.LocalidadeTerminoPrestacao.Codigo == codigoLocalidadeTerminoPrestacao);

            if (!string.IsNullOrWhiteSpace(ufInicio))
                result = result.Where(o => o.LocalidadeInicioPrestacao.Estado.Sigla == ufInicio);

            if (!string.IsNullOrWhiteSpace(ufFim))
                result = result.Where(o => o.LocalidadeTerminoPrestacao.Estado.Sigla == ufFim);

            if (!string.IsNullOrWhiteSpace(cpfCnpjTomador))
            {
                //if (raizCNPJTomador)
                //    result = result.Where(o => (o.TipoTomador == Dominio.Enumeradores.TipoTomador.Destinatario && o.Destinatario.CPF_CNPJ.Contains(cpfCnpjTomador.Substring(0, 8))) ||
                //                           (o.TipoTomador == Dominio.Enumeradores.TipoTomador.Expedidor && o.Expedidor.CPF_CNPJ.Contains(cpfCnpjTomador.Substring(0, 8))) ||
                //                           (o.TipoTomador == Dominio.Enumeradores.TipoTomador.Outros && o.OutrosTomador.CPF_CNPJ.Contains(cpfCnpjTomador.Substring(0, 8))) ||
                //                           (o.TipoTomador == Dominio.Enumeradores.TipoTomador.Recebedor && o.Recebedor.CPF_CNPJ.Contains(cpfCnpjTomador.Substring(0, 8))) ||
                //                           (o.TipoTomador == Dominio.Enumeradores.TipoTomador.Remetente && o.Remetente.CPF_CNPJ.Contains(cpfCnpjTomador.Substring(0, 8))));
                //else
                //    result = result.Where(o => (o.TipoTomador == Dominio.Enumeradores.TipoTomador.Destinatario && o.Destinatario.CPF_CNPJ == cpfCnpjTomador) ||
                //                               (o.TipoTomador == Dominio.Enumeradores.TipoTomador.Expedidor && o.Expedidor.CPF_CNPJ == cpfCnpjTomador) ||
                //                               (o.TipoTomador == Dominio.Enumeradores.TipoTomador.Outros && o.OutrosTomador.CPF_CNPJ == cpfCnpjTomador) ||
                //                               (o.TipoTomador == Dominio.Enumeradores.TipoTomador.Recebedor && o.Recebedor.CPF_CNPJ == cpfCnpjTomador) ||
                //                               (o.TipoTomador == Dominio.Enumeradores.TipoTomador.Remetente && o.Remetente.CPF_CNPJ == cpfCnpjTomador));
                if (removerCliente)
                {
                    if (raizCNPJTomador)
                        result = result.Where(o => !o.TomadorPagador.CPF_CNPJ.Contains(cpfCnpjTomador.Substring(0, 8)));
                    else
                        result = result.Where(o => o.TomadorPagador.CPF_CNPJ != cpfCnpjTomador);
                }
                else
                {
                    if (raizCNPJTomador)
                        result = result.Where(o => o.TomadorPagador.CPF_CNPJ.Contains(cpfCnpjTomador.Substring(0, 8)));
                    else
                        result = result.Where(o => o.TomadorPagador.CPF_CNPJ == cpfCnpjTomador);
                }
            }

            if (numeroInicial > 0)
                result = result.Where(o => o.Numero >= numeroInicial);

            if (numeroFinal > 0)
                result = result.Where(o => o.Numero <= numeroFinal);

            if (codigoSerie > 0)
                result = result.Where(o => o.Serie.Codigo == codigoSerie);
            else if (series.Count() > 0)
                result = result.Where(o => series.Contains(o.Serie.Codigo));

            if (!string.IsNullOrWhiteSpace(cpfMotorista) || !string.IsNullOrWhiteSpace(nomeMotorista))
            {
                var queryMotorista = this.SessionNHiBernate.Query<Dominio.Entidades.MotoristaCTE>();

                if (!string.IsNullOrWhiteSpace(cpfMotorista))
                    result = result.Where(o => (from obj in queryMotorista where obj.CPFMotorista.Equals(cpfMotorista) select obj.CTE.Codigo).Contains(o.Codigo));
                else
                    result = result.Where(o => (from obj in queryMotorista where obj.NomeMotoristaCTe.Contains(nomeMotorista) select obj.CTE.Codigo).Contains(o.Codigo));
            }

            if (!string.IsNullOrWhiteSpace(placaVeiculo))
            {
                var queryVeiculos = this.SessionNHiBernate.Query<Dominio.Entidades.VeiculoCTE>();

                result = result.Where(o => (from obj in queryVeiculos where obj.CTE.Codigo == o.Codigo && obj.Placa.Equals(placaVeiculo) select obj.CTE.Codigo).Contains(o.Codigo));
            }

            if (!string.IsNullOrWhiteSpace(numeroNotaFiscal))
            {
                var queryDocumentos = this.SessionNHiBernate.Query<Dominio.Entidades.DocumentosCTE>();

                result = result.Where(o => (from obj in queryDocumentos where obj.CTE.Codigo == o.Codigo && obj.Numero.Contains(numeroNotaFiscal) select obj.CTE.Codigo).Contains(o.Codigo));
            }

            if (!string.IsNullOrWhiteSpace(tipoOcorrencia))
            {
                var queryOcorrencias = this.SessionNHiBernate.Query<Dominio.Entidades.OcorrenciaDeCTe>();

                if (tipoOcorrencia == "F")
                    result = result.Where(o => (from obj in queryOcorrencias where obj.CTe.Codigo == o.Codigo && obj.Ocorrencia.Tipo.Equals("F") select obj.CTe.Codigo).Contains(o.Codigo));
                else
                    result = result.Where(o => !(from obj in queryOcorrencias where obj.CTe.Codigo == o.Codigo && obj.Ocorrencia.Tipo.Equals("F") select obj.CTe.Codigo).Contains(o.Codigo));
            }

            if (statusPagamento.HasValue)
            {
                var queryDuplicatas = this.SessionNHiBernate.Query<Dominio.Entidades.ParcelaCobrancaCTe>();

                result = result.Where(o => (from obj in queryDuplicatas select obj.Cobranca.CTe.Codigo).Contains(o.Codigo));

                if (statusPagamento.Value == Dominio.Enumeradores.StatusDuplicata.Paga)
                    result = result.Where(o => !(from obj in queryDuplicatas where obj.Status == Dominio.Enumeradores.StatusDuplicata.Pendente select obj.Cobranca.CTe.Codigo).Contains(o.Codigo));
                else
                    result = result.Where(o => (from obj in queryDuplicatas where obj.Status == Dominio.Enumeradores.StatusDuplicata.Pendente select obj.Cobranca.CTe.Codigo).Contains(o.Codigo));
            }

            if (!string.IsNullOrWhiteSpace(duplicata))
            {
                var queryDuplicata = this.SessionNHiBernate.Query<Dominio.Entidades.DuplicataCtes>();

                result = result.Where(o => (from obj in queryDuplicata where obj.Duplicata.Numero == int.Parse(duplicata) select obj.ConhecimentoDeTransporteEletronico.Codigo).Contains(o.Codigo));
            }

            if (icmsCTe == "0")
                result = result.Where(o => o.ValorICMS == 0);
            else
            if (icmsCTe == "1")
                result = result.Where(o => o.ValorICMS > 0);

            if (cstCTe != "0")
                result = result.Where(o => o.CST.Equals(cstCTe));

            if (tipoServico != null)
                result = result.Where(o => o.TipoServico == tipoServico);

            if (!string.IsNullOrWhiteSpace(observacao))
                result = result.Where(o => o.ObservacoesGerais.Contains(observacao));

            if (numeroCarga > 0 || numeroUnidade > 0)
            {
                var queryIntegracaoCTe = this.SessionNHiBernate.Query<Dominio.Entidades.IntegracaoCTe>();
                var resultIntegracaoCTe = from o in queryIntegracaoCTe select o;
                if (numeroCarga > 0)
                    resultIntegracaoCTe = resultIntegracaoCTe.Where(o => o.NumeroDaCarga == numeroCarga);
                if (numeroUnidade > 0)
                    resultIntegracaoCTe = resultIntegracaoCTe.Where(o => o.NumeroDaUnidade == numeroUnidade);

                result = result.Where(o => resultIntegracaoCTe.Select(c => c.CTe.Codigo).Contains(o.Codigo));
            }

            return result.Select(o => new Dominio.ObjetosDeValor.Relatorios.RelatorioCTesEmitidos()
            {
                CODIGO = o.Codigo,
                CODIGO_CTE = o.Numero,
                CST = o.CST,
                DATA_EMISSAO_CTE = o.DataEmissao.Value,
                DESTINATARIO_EXTERIOR = o.Destinatario != null ? o.Destinatario.Exterior : false,
                FRETE_ICMS = o.ValorFrete,
                UF_EMPRESA = o.Empresa.Localidade.Estado.Sigla,
                NOME_CIDADE_FIM = o.LocalidadeTerminoPrestacao.Descricao,
                NOME_CIDADE_FIM_EXTERIOR = o.Destinatario != null ? o.Destinatario.Cidade : string.Empty,
                NOME_CIDADE_INICIO = o.LocalidadeInicioPrestacao.Descricao,
                NOME_CIDADE_INICIO_EXTERIOR = o.Remetente != null ? o.Remetente.Cidade : string.Empty,
                NOME_RAZAO = o.Remetente != null ? o.Remetente.CPF_CNPJ + " " + o.Remetente.Nome : string.Empty,
                NOME_RAZAO_1 = o.TomadorPagador != null ? o.TomadorPagador.CPF_CNPJ + " " + o.TomadorPagador.Cliente.Nome : string.Empty,
                PAIS_FIM = o.Destinatario != null && o.Destinatario.Pais != null ? o.Destinatario.Pais.Nome : string.Empty,
                PAIS_INICIO = o.Remetente != null && o.Remetente.Pais != null ? o.Remetente.Pais.Nome : string.Empty,
                REMETENTE_EXTERIOR = o.Remetente != null ? o.Remetente.Exterior : false,
                SERIE = o.Serie.Numero,
                Status_Abrev = o.Status,
                TIPO_CTE = o.TipoCTE,
                UF_FIM = o.LocalidadeTerminoPrestacao.Estado.Sigla,
                UF_INICIO = o.LocalidadeInicioPrestacao.Estado.Sigla,
                BASE_CALCULO_ICMS = o.BaseCalculoICMS,
                ALIQUOTA_ICMS = o.AliquotaICMS,
                VALOR_ICMS = o.ValorICMS,
                VALOR_RECEBER = Math.Round(o.ValorAReceber, 2, MidpointRounding.ToEven),
                VALOR_SERVICO = o.ValorPrestacaoServico,
                QUANTIDADE = (from obj in this.SessionNHiBernate.Query<Dominio.Entidades.InformacaoCargaCTE>() where obj.CTE.Codigo == o.Codigo && obj.UnidadeMedida == "01" select (decimal?)obj.Quantidade).Sum() ?? 0m,
                QUANTIDADE_TOTAL = (from obj in this.SessionNHiBernate.Query<Dominio.Entidades.InformacaoCargaCTE>() where obj.CTE.Codigo == o.Codigo select (decimal?)obj.Quantidade).Sum() ?? 0m,
                LOCALIDADE_INICIO_PRESTACAO = o.LocalidadeInicioPrestacao.Descricao,
                ESTADO_INICIO_PRESTACAO = o.LocalidadeInicioPrestacao.Estado.Sigla,
                LOCALIDADE_TERMINO_PRESTACAO = o.LocalidadeTerminoPrestacao.Descricao,
                ESTADO_TERMINO_PRESTACAO = o.LocalidadeTerminoPrestacao.Estado.Sigla,
                CFOP = o.CFOP.CodigoCFOP,
                CHAVE = o.Chave,
                VALOR_MERCADORIA = o.ValorTotalMercadoria,
                PLACA = (from obj in this.SessionNHiBernate.Query<Dominio.Entidades.VeiculoCTE>() where obj.CTE.Codigo == o.Codigo select obj.Placa).FirstOrDefault() ?? string.Empty // o.Veiculos != null && o.Veiculos.Count > 0 ? o.Veiculos.FirstOrDefault().Placa : string.Empty
            }).OrderBy(o => o.NOME_RAZAO_1).Timeout(120).ToList();
        }

        public List<Dominio.ObjetosDeValor.Relatorios.RelatorioCTesSumarizadoPorDocumento> RelatorioSumarizadoPorDocumento(int codigoEmpresa, DateTime dataEmissaoInicial, DateTime dataEmissaoFinal, string status, Dominio.Enumeradores.TipoCTE tipoCTe, int codigoSerie, string cpfCnpjRemetente, string cpfCnpjExpedidor, string cpfCnpjRecebedor, string cpfCnpjDestinatario, string cpfCnpjTomador, int numeroInicial, int numeroFinal, string nomeMotorista, string cpfMotorista, string placaVeiculo, Dominio.Enumeradores.TipoAmbiente tipoAmbiente, int[] series, string tipoOcorrencia, string numeroNotaFiscal, Dominio.Enumeradores.StatusDuplicata? statusPagamento, int codigoLocalidadeInicioPrestacao, int codigoLocalidadeTerminoPrestacao, string ufInicio, string ufFim, string duplicata, bool importacao, bool exportacao, bool raizCNPJRemetente, bool raizCNPJExpedidor, bool raizCNPJRecebedor, bool raizCNPJDestinatario, bool raizCNPJTomador, string icmsCTe, Dominio.Enumeradores.TipoServico? tipoServico, string cstCTe, int codigoUsuario, string nomeUsuario, DateTime dataAutorizacaoInicial, DateTime dataAutorizacaoFinal, string observacao, bool removerCliente, int numeroCarga, int numeroUnidade)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.DocumentosCTE>();

            var result = from obj in query where obj.CTE.Empresa.Codigo == codigoEmpresa && obj.CTE.TipoAmbiente == tipoAmbiente select obj;

            result = result.Where(o => (o.CTE.ModeloDocumentoFiscal.Numero.Equals("57") || o.CTE.ModeloDocumentoFiscal.Numero.Equals("67")));

            if (dataEmissaoInicial != DateTime.MinValue)
                result = result.Where(o => o.CTE.DataEmissao >= dataEmissaoInicial.Date);

            if (dataEmissaoFinal != DateTime.MinValue)
                result = result.Where(o => o.CTE.DataEmissao < dataEmissaoFinal.AddDays(1).Date);

            if (dataAutorizacaoInicial != DateTime.MinValue)
                result = result.Where(o => o.CTE.DataRetornoSefaz >= dataAutorizacaoInicial.Date);

            if (dataAutorizacaoFinal != DateTime.MinValue)
                result = result.Where(o => o.CTE.DataRetornoSefaz < dataAutorizacaoFinal.AddDays(1).Date);

            if (!string.IsNullOrWhiteSpace(status))
                result = result.Where(o => o.CTE.Status.Equals(status));

            if (tipoCTe != Dominio.Enumeradores.TipoCTE.Todos)
                result = result.Where(o => o.CTE.TipoCTE == tipoCTe);

            if (!string.IsNullOrWhiteSpace(cpfCnpjRemetente))
            {
                if (removerCliente)
                {
                    if (raizCNPJRemetente)
                        result = result.Where(o => !o.CTE.Remetente.CPF_CNPJ.Contains(cpfCnpjRemetente.Substring(0, 8)));
                    else
                        result = result.Where(o => o.CTE.Remetente.CPF_CNPJ != cpfCnpjRemetente);
                }
                else
                {
                    if (raizCNPJRemetente)
                        result = result.Where(o => o.CTE.Remetente.CPF_CNPJ.Contains(cpfCnpjRemetente.Substring(0, 8)));
                    else
                        result = result.Where(o => o.CTE.Remetente.CPF_CNPJ == cpfCnpjRemetente);
                }
            }

            if (importacao)
                result = result.Where(o => o.CTE.Remetente.Exterior == true);

            if (!string.IsNullOrWhiteSpace(cpfCnpjDestinatario))
            {
                if (removerCliente)
                {
                    if (raizCNPJDestinatario)
                        result = result.Where(o => !o.CTE.Destinatario.CPF_CNPJ.Contains(cpfCnpjDestinatario.Substring(0, 8)));
                    else
                        result = result.Where(o => o.CTE.Destinatario.CPF_CNPJ != cpfCnpjDestinatario);
                }
                else
                {
                    if (raizCNPJDestinatario)
                        result = result.Where(o => o.CTE.Destinatario.CPF_CNPJ.Contains(cpfCnpjDestinatario.Substring(0, 8)));
                    else
                        result = result.Where(o => o.CTE.Destinatario.CPF_CNPJ == cpfCnpjDestinatario);
                }
            }

            if (exportacao)
                result = result.Where(o => o.CTE.Destinatario.Exterior == true);

            if (!string.IsNullOrWhiteSpace(cpfCnpjExpedidor))
            {
                if (removerCliente)
                {
                    if (raizCNPJExpedidor)
                        result = result.Where(o => !o.CTE.Expedidor.CPF_CNPJ.Contains(cpfCnpjExpedidor.Substring(0, 8)));
                    else
                        result = result.Where(o => o.CTE.Expedidor.CPF_CNPJ != cpfCnpjExpedidor);
                }
                else
                {
                    if (raizCNPJExpedidor)
                        result = result.Where(o => o.CTE.Expedidor.CPF_CNPJ.Contains(cpfCnpjExpedidor.Substring(0, 8)));
                    else
                        result = result.Where(o => o.CTE.Expedidor.CPF_CNPJ == cpfCnpjExpedidor);
                }
            }

            if (!string.IsNullOrWhiteSpace(cpfCnpjRecebedor))
            {
                if (removerCliente)
                {
                    if (raizCNPJRecebedor)
                        result = result.Where(o => !o.CTE.Recebedor.CPF_CNPJ.Contains(cpfCnpjRecebedor.Substring(0, 8)));
                    else
                        result = result.Where(o => o.CTE.Recebedor.CPF_CNPJ != cpfCnpjRecebedor);
                }
                else
                {
                    if (raizCNPJRecebedor)
                        result = result.Where(o => o.CTE.Recebedor.CPF_CNPJ.Contains(cpfCnpjRecebedor.Substring(0, 8)));
                    else
                        result = result.Where(o => o.CTE.Recebedor.CPF_CNPJ == cpfCnpjRecebedor);
                }
            }

            if (codigoLocalidadeInicioPrestacao > 0)
                result = result.Where(o => o.CTE.LocalidadeInicioPrestacao.Codigo == codigoLocalidadeInicioPrestacao);

            if (codigoLocalidadeTerminoPrestacao > 0)
                result = result.Where(o => o.CTE.LocalidadeTerminoPrestacao.Codigo == codigoLocalidadeTerminoPrestacao);

            if (!string.IsNullOrWhiteSpace(ufInicio))
                result = result.Where(o => o.CTE.LocalidadeInicioPrestacao.Estado.Sigla == ufInicio);

            if (!string.IsNullOrWhiteSpace(ufFim))
                result = result.Where(o => o.CTE.LocalidadeTerminoPrestacao.Estado.Sigla == ufFim);

            if (!string.IsNullOrWhiteSpace(cpfCnpjTomador))
            {
                //if (raizCNPJTomador)
                //    result = result.Where(o => (o.CTE.TipoTomador == Dominio.Enumeradores.TipoTomador.Destinatario && o.CTE.Destinatario.CPF_CNPJ.Contains(cpfCnpjTomador.Substring(0, 8))) ||
                //                           (o.CTE.TipoTomador == Dominio.Enumeradores.TipoTomador.Expedidor && o.CTE.Expedidor.CPF_CNPJ.Contains(cpfCnpjTomador.Substring(0, 8))) ||
                //                           (o.CTE.TipoTomador == Dominio.Enumeradores.TipoTomador.Outros && o.CTE.OutrosTomador.CPF_CNPJ.Contains(cpfCnpjTomador.Substring(0, 8))) ||
                //                           (o.CTE.TipoTomador == Dominio.Enumeradores.TipoTomador.Recebedor && o.CTE.Recebedor.CPF_CNPJ.Contains(cpfCnpjTomador.Substring(0, 8))) ||
                //                           (o.CTE.TipoTomador == Dominio.Enumeradores.TipoTomador.Remetente && o.CTE.Remetente.CPF_CNPJ.Contains(cpfCnpjTomador.Substring(0, 8))));
                //else
                //    result = result.Where(o => (o.CTE.TipoTomador == Dominio.Enumeradores.TipoTomador.Destinatario && o.CTE.Destinatario.CPF_CNPJ == cpfCnpjTomador) ||
                //                               (o.CTE.TipoTomador == Dominio.Enumeradores.TipoTomador.Expedidor && o.CTE.Expedidor.CPF_CNPJ == cpfCnpjTomador) ||
                //                               (o.CTE.TipoTomador == Dominio.Enumeradores.TipoTomador.Outros && o.CTE.OutrosTomador.CPF_CNPJ == cpfCnpjTomador) ||
                //                               (o.CTE.TipoTomador == Dominio.Enumeradores.TipoTomador.Recebedor && o.CTE.Recebedor.CPF_CNPJ == cpfCnpjTomador) ||
                //                               (o.CTE.TipoTomador == Dominio.Enumeradores.TipoTomador.Remetente && o.CTE.Remetente.CPF_CNPJ == cpfCnpjTomador));
                if (removerCliente)
                {
                    if (raizCNPJTomador)
                        result = result.Where(o => !o.CTE.TomadorPagador.CPF_CNPJ.Contains(cpfCnpjTomador.Substring(0, 8)));
                    else
                        result = result.Where(o => o.CTE.TomadorPagador.CPF_CNPJ != cpfCnpjTomador);
                }
                else
                {
                    if (raizCNPJTomador)
                        result = result.Where(o => o.CTE.TomadorPagador.CPF_CNPJ.Contains(cpfCnpjTomador.Substring(0, 8)));
                    else
                        result = result.Where(o => o.CTE.TomadorPagador.CPF_CNPJ == cpfCnpjTomador);
                }
            }

            if (numeroInicial > 0)
                result = result.Where(o => o.CTE.Numero >= numeroInicial);

            if (numeroFinal > 0)
                result = result.Where(o => o.CTE.Numero <= numeroFinal);

            if (codigoSerie > 0)
                result = result.Where(o => o.CTE.Serie.Codigo == codigoSerie);
            else if (series.Count() > 0)
                result = result.Where(o => series.Contains(o.CTE.Serie.Codigo));

            if (!string.IsNullOrWhiteSpace(cpfMotorista) || !string.IsNullOrWhiteSpace(nomeMotorista))
            {
                var queryMotorista = this.SessionNHiBernate.Query<Dominio.Entidades.MotoristaCTE>();

                if (!string.IsNullOrWhiteSpace(cpfMotorista))
                    result = result.Where(o => (from obj in queryMotorista where obj.CPFMotorista.Equals(cpfMotorista) select obj.CTE.Codigo).Contains(o.Codigo));
                else
                    result = result.Where(o => (from obj in queryMotorista where obj.NomeMotoristaCTe.Contains(nomeMotorista) select obj.CTE.Codigo).Contains(o.Codigo));
            }

            if (!string.IsNullOrWhiteSpace(placaVeiculo))
            {
                var queryVeiculos = this.SessionNHiBernate.Query<Dominio.Entidades.VeiculoCTE>();

                result = result.Where(o => (from obj in queryVeiculos where obj.CTE.Codigo == o.CTE.Codigo && obj.Placa.Equals(placaVeiculo) select obj.CTE.Codigo).Contains(o.CTE.Codigo));
            }

            if (!string.IsNullOrWhiteSpace(numeroNotaFiscal))
            {
                result = result.Where(o => o.Numero.Contains(numeroNotaFiscal));
            }

            if (!string.IsNullOrWhiteSpace(tipoOcorrencia))
            {
                var queryOcorrencias = this.SessionNHiBernate.Query<Dominio.Entidades.OcorrenciaDeCTe>();

                if (tipoOcorrencia == "F")
                    result = result.Where(o => (from obj in queryOcorrencias where obj.CTe.Codigo == o.CTE.Codigo && obj.Ocorrencia.Tipo.Equals("F") select obj.CTe.Codigo).Contains(o.CTE.Codigo));
                else
                    result = result.Where(o => !(from obj in queryOcorrencias where obj.CTe.Codigo == o.CTE.Codigo && obj.Ocorrencia.Tipo.Equals("F") select obj.CTe.Codigo).Contains(o.CTE.Codigo));
            }

            if (statusPagamento.HasValue)
            {
                var queryDuplicatas = this.SessionNHiBernate.Query<Dominio.Entidades.ParcelaCobrancaCTe>();

                result = result.Where(o => (from obj in queryDuplicatas select obj.Cobranca.CTe.Codigo).Contains(o.CTE.Codigo));

                if (statusPagamento.Value == Dominio.Enumeradores.StatusDuplicata.Paga)
                    result = result.Where(o => !(from obj in queryDuplicatas where obj.Status == Dominio.Enumeradores.StatusDuplicata.Pendente select obj.Cobranca.CTe.Codigo).Contains(o.CTE.Codigo));
                else
                    result = result.Where(o => (from obj in queryDuplicatas where obj.Status == Dominio.Enumeradores.StatusDuplicata.Pendente select obj.Cobranca.CTe.Codigo).Contains(o.CTE.Codigo));
            }

            if (!string.IsNullOrWhiteSpace(duplicata))
            {
                var queryDuplicata = this.SessionNHiBernate.Query<Dominio.Entidades.DuplicataCtes>();

                result = result.Where(o => (from obj in queryDuplicata where obj.Duplicata.Numero == int.Parse(duplicata) select obj.ConhecimentoDeTransporteEletronico.Codigo).Contains(o.Codigo));
            }

            if (icmsCTe == "0")
                result = result.Where(o => o.CTE.ValorICMS == 0);
            else
            if (icmsCTe == "1")
                result = result.Where(o => o.CTE.ValorICMS > 0);

            if (cstCTe != "0")
                result = result.Where(o => o.CTE.CST.Equals(cstCTe));

            if (tipoServico != null)
                result = result.Where(o => o.CTE.TipoServico == tipoServico);

            if (codigoUsuario > 0 && !string.IsNullOrWhiteSpace(nomeUsuario))
                result = result.Where(o => o.CTE.Usuario.Codigo == codigoUsuario);//|| o.Log.Contains(nomeUsuario)

            if (!string.IsNullOrWhiteSpace(observacao))
                result = result.Where(o => o.CTE.ObservacoesGerais.Contains(observacao));

            if (numeroCarga > 0 || numeroUnidade > 0)
            {
                var queryIntegracaoCTe = this.SessionNHiBernate.Query<Dominio.Entidades.IntegracaoCTe>();
                var resultIntegracaoCTe = from o in queryIntegracaoCTe select o;
                if (numeroCarga > 0)
                    resultIntegracaoCTe = resultIntegracaoCTe.Where(o => o.NumeroDaCarga == numeroCarga);
                if (numeroUnidade > 0)
                    resultIntegracaoCTe = resultIntegracaoCTe.Where(o => o.NumeroDaUnidade == numeroUnidade);

                result = result.Where(o => resultIntegracaoCTe.Select(c => c.CTe.Codigo).Contains(o.Codigo));
            }

            return result.Select(o => new Dominio.ObjetosDeValor.Relatorios.RelatorioCTesSumarizadoPorDocumento()
            {
                CEPDestinatario = o.CTE.Destinatario.CEP,
                CidadeDestinatario = o.CTE.Destinatario.Localidade.Descricao,
                CodigoCTe = o.CTE.Codigo,
                DataEmissaoCTe = o.CTE.DataEmissao,
                NumeroCTe = o.CTE.Numero,
                NumeroDocumento = !string.IsNullOrWhiteSpace(o.Serie) ? o.Numero + "/" + o.Serie : !string.IsNullOrWhiteSpace(o.ChaveNFE) ? o.Numero + "/" + o.ChaveNFE.Substring(22, 3) : o.Numero,
                PesoDocumento = o.Peso,
                SerieCTe = o.CTE.Serie.Numero,
                UFDestinatario = o.CTE.Destinatario.Localidade.Estado.Sigla,
                ValorDocumento = o.Valor,
                PesoCTe = (from obj in this.SessionNHiBernate.Query<Dominio.Entidades.InformacaoCargaCTE>() where obj.CTE.Codigo == o.CTE.Codigo && obj.UnidadeMedida == "01" select (decimal?)obj.Quantidade).Sum() ?? 0m,
                PesoCTeTotal = (from obj in this.SessionNHiBernate.Query<Dominio.Entidades.InformacaoCargaCTE>() where obj.CTE.Codigo == o.CTE.Codigo select (decimal?)obj.Quantidade).Sum() ?? 0m,
                ValorFrete = o.CTE.ValorFrete,
                ValorAReceber = o.CTE.ValorAReceber,
                Remetente = o.CTE.Remetente.Nome,
                Destinatario = o.CTE.Destinatario.Nome,
                UFInicio = o.CTE.LocalidadeInicioPrestacao.Estado.Sigla,
                UFFim = o.CTE.LocalidadeTerminoPrestacao.Estado.Sigla,
                Placa = (from obj in this.SessionNHiBernate.Query<Dominio.Entidades.VeiculoCTE>() where obj.CTE.Codigo == o.Codigo select obj.Placa).FirstOrDefault() ?? string.Empty,
                BaseCalculoICMS = o.CTE.BaseCalculoICMS,
                ValorMercadoria = o.CTE.ValorTotalMercadoria,
                ObservacaoCTe = o.CTE.ObservacoesGerais,
                Usuario = o.CTE.Usuario != null && !string.IsNullOrWhiteSpace(o.CTE.Usuario.Nome) ? o.CTE.Usuario.Nome : string.Empty,
                ValorICMS = o.ValorICMS,
                Pagamento = o.CTE.TipoPagamento == Dominio.Enumeradores.TipoPagamento.Pago ? "CIF" : "FOB",
                PesoUnidade = (from obj in this.SessionNHiBernate.Query<Dominio.Entidades.InformacaoCargaCTE>() where obj.CTE.Codigo == o.CTE.Codigo select (decimal?)obj.Quantidade).FirstOrDefault() ?? 0m,
                UnidadeMedida = (from obj in this.SessionNHiBernate.Query<Dominio.Entidades.InformacaoCargaCTE>() where obj.CTE.Codigo == o.CTE.Codigo select obj.Tipo).FirstOrDefault() ?? string.Empty
            }).Timeout(120).ToList();
        }

        public List<Dominio.ObjetosDeValor.Relatorios.RelatorioCTesSumarizadoPorDocumento> RelatorioSumarizadoPorDocumentoObsFisco(int codigoEmpresa, DateTime dataEmissaoInicial, DateTime dataEmissaoFinal, string status, Dominio.Enumeradores.TipoCTE tipoCTe, int codigoSerie, string cpfCnpjRemetente, string cpfCnpjExpedidor, string cpfCnpjRecebedor, string cpfCnpjDestinatario, string cpfCnpjTomador, int numeroInicial, int numeroFinal, string nomeMotorista, string cpfMotorista, string placaVeiculo, Dominio.Enumeradores.TipoAmbiente tipoAmbiente, int[] series, string tipoOcorrencia, string numeroNotaFiscal, Dominio.Enumeradores.StatusDuplicata? statusPagamento, int codigoLocalidadeInicioPrestacao, int codigoLocalidadeTerminoPrestacao, string ufInicio, string ufFim, string duplicata, bool importacao, bool exportacao, bool raizCNPJRemetente, bool raizCNPJExpedidor, bool raizCNPJRecebedor, bool raizCNPJDestinatario, bool raizCNPJTomador, string icmsCTe, Dominio.Enumeradores.TipoServico? tipoServico, string cstCTe, int codigoUsuario, string nomeUsuario, DateTime dataAutorizacaoInicial, DateTime dataAutorizacaoFinal, string observacao, bool removerCliente, int numeroCarga, int numeroUnidade)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.DocumentosCTE>();

            var result = from obj in query where obj.CTE.Empresa.Codigo == codigoEmpresa && obj.CTE.TipoAmbiente == tipoAmbiente select obj;

            result = result.Where(o => (o.CTE.ModeloDocumentoFiscal.Numero.Equals("57") || o.CTE.ModeloDocumentoFiscal.Numero.Equals("67")));

            if (dataEmissaoInicial != DateTime.MinValue)
                result = result.Where(o => o.CTE.DataEmissao >= dataEmissaoInicial.Date);

            if (dataEmissaoFinal != DateTime.MinValue)
                result = result.Where(o => o.CTE.DataEmissao < dataEmissaoFinal.AddDays(1).Date);

            if (dataAutorizacaoInicial != DateTime.MinValue)
                result = result.Where(o => o.CTE.DataRetornoSefaz >= dataAutorizacaoInicial.Date);

            if (dataAutorizacaoFinal != DateTime.MinValue)
                result = result.Where(o => o.CTE.DataRetornoSefaz < dataAutorizacaoFinal.AddDays(1).Date);

            if (!string.IsNullOrWhiteSpace(status))
                result = result.Where(o => o.CTE.Status.Equals(status));

            if (tipoCTe != Dominio.Enumeradores.TipoCTE.Todos)
                result = result.Where(o => o.CTE.TipoCTE == tipoCTe);

            if (!string.IsNullOrWhiteSpace(cpfCnpjRemetente))
            {
                if (removerCliente)
                {
                    if (raizCNPJRemetente)
                        result = result.Where(o => !o.CTE.Remetente.CPF_CNPJ.Contains(cpfCnpjRemetente.Substring(0, 8)));
                    else
                        result = result.Where(o => o.CTE.Remetente.CPF_CNPJ != cpfCnpjRemetente);
                }
                else
                {
                    if (raizCNPJRemetente)
                        result = result.Where(o => o.CTE.Remetente.CPF_CNPJ.Contains(cpfCnpjRemetente.Substring(0, 8)));
                    else
                        result = result.Where(o => o.CTE.Remetente.CPF_CNPJ == cpfCnpjRemetente);
                }
            }

            if (importacao)
                result = result.Where(o => o.CTE.Remetente.Exterior == true);

            if (!string.IsNullOrWhiteSpace(cpfCnpjDestinatario))
            {
                if (removerCliente)
                {
                    if (raizCNPJDestinatario)
                        result = result.Where(o => !o.CTE.Destinatario.CPF_CNPJ.Contains(cpfCnpjDestinatario.Substring(0, 8)));
                    else
                        result = result.Where(o => o.CTE.Destinatario.CPF_CNPJ != cpfCnpjDestinatario);
                }
                else
                {
                    if (raizCNPJDestinatario)
                        result = result.Where(o => o.CTE.Destinatario.CPF_CNPJ.Contains(cpfCnpjDestinatario.Substring(0, 8)));
                    else
                        result = result.Where(o => o.CTE.Destinatario.CPF_CNPJ == cpfCnpjDestinatario);
                }
            }

            if (exportacao)
                result = result.Where(o => o.CTE.Destinatario.Exterior == true);

            if (!string.IsNullOrWhiteSpace(cpfCnpjExpedidor))
            {
                if (removerCliente)
                {
                    if (raizCNPJExpedidor)
                        result = result.Where(o => !o.CTE.Expedidor.CPF_CNPJ.Contains(cpfCnpjExpedidor.Substring(0, 8)));
                    else
                        result = result.Where(o => o.CTE.Expedidor.CPF_CNPJ != cpfCnpjExpedidor);
                }
                else
                {
                    if (raizCNPJExpedidor)
                        result = result.Where(o => o.CTE.Expedidor.CPF_CNPJ.Contains(cpfCnpjExpedidor.Substring(0, 8)));
                    else
                        result = result.Where(o => o.CTE.Expedidor.CPF_CNPJ == cpfCnpjExpedidor);
                }
            }

            if (!string.IsNullOrWhiteSpace(cpfCnpjRecebedor))
            {
                if (removerCliente)
                {
                    if (raizCNPJRecebedor)
                        result = result.Where(o => !o.CTE.Recebedor.CPF_CNPJ.Contains(cpfCnpjRecebedor.Substring(0, 8)));
                    else
                        result = result.Where(o => o.CTE.Recebedor.CPF_CNPJ != cpfCnpjRecebedor);
                }
                else
                {
                    if (raizCNPJRecebedor)
                        result = result.Where(o => o.CTE.Recebedor.CPF_CNPJ.Contains(cpfCnpjRecebedor.Substring(0, 8)));
                    else
                        result = result.Where(o => o.CTE.Recebedor.CPF_CNPJ == cpfCnpjRecebedor);
                }
            }

            if (codigoLocalidadeInicioPrestacao > 0)
                result = result.Where(o => o.CTE.LocalidadeInicioPrestacao.Codigo == codigoLocalidadeInicioPrestacao);

            if (codigoLocalidadeTerminoPrestacao > 0)
                result = result.Where(o => o.CTE.LocalidadeTerminoPrestacao.Codigo == codigoLocalidadeTerminoPrestacao);

            if (!string.IsNullOrWhiteSpace(ufInicio))
                result = result.Where(o => o.CTE.LocalidadeInicioPrestacao.Estado.Sigla == ufInicio);

            if (!string.IsNullOrWhiteSpace(ufFim))
                result = result.Where(o => o.CTE.LocalidadeTerminoPrestacao.Estado.Sigla == ufFim);

            if (!string.IsNullOrWhiteSpace(cpfCnpjTomador))
            {
                if (removerCliente)
                {
                    if (raizCNPJTomador)
                        result = result.Where(o => !o.CTE.TomadorPagador.CPF_CNPJ.Contains(cpfCnpjTomador.Substring(0, 8)));
                    else
                        result = result.Where(o => o.CTE.TomadorPagador.CPF_CNPJ != cpfCnpjTomador);
                }
                else
                {
                    if (raizCNPJTomador)
                        result = result.Where(o => o.CTE.TomadorPagador.CPF_CNPJ.Contains(cpfCnpjTomador.Substring(0, 8)));
                    else
                        result = result.Where(o => o.CTE.TomadorPagador.CPF_CNPJ == cpfCnpjTomador);
                }
            }

            if (numeroInicial > 0)
                result = result.Where(o => o.CTE.Numero >= numeroInicial);

            if (numeroFinal > 0)
                result = result.Where(o => o.CTE.Numero <= numeroFinal);

            if (codigoSerie > 0)
                result = result.Where(o => o.CTE.Serie.Codigo == codigoSerie);
            else if (series.Count() > 0)
                result = result.Where(o => series.Contains(o.CTE.Serie.Codigo));

            if (!string.IsNullOrWhiteSpace(cpfMotorista) || !string.IsNullOrWhiteSpace(nomeMotorista))
            {
                var queryMotorista = this.SessionNHiBernate.Query<Dominio.Entidades.MotoristaCTE>();

                if (!string.IsNullOrWhiteSpace(cpfMotorista))
                    result = result.Where(o => (from obj in queryMotorista where obj.CPFMotorista.Equals(cpfMotorista) select obj.CTE.Codigo).Contains(o.Codigo));
                else
                    result = result.Where(o => (from obj in queryMotorista where obj.NomeMotoristaCTe.Contains(nomeMotorista) select obj.CTE.Codigo).Contains(o.Codigo));
            }

            if (!string.IsNullOrWhiteSpace(placaVeiculo))
            {
                var queryVeiculos = this.SessionNHiBernate.Query<Dominio.Entidades.VeiculoCTE>();

                result = result.Where(o => (from obj in queryVeiculos where obj.CTE.Codigo == o.CTE.Codigo && obj.Placa.Equals(placaVeiculo) select obj.CTE.Codigo).Contains(o.CTE.Codigo));
            }

            if (!string.IsNullOrWhiteSpace(numeroNotaFiscal))
            {
                result = result.Where(o => o.Numero.Contains(numeroNotaFiscal));
            }

            if (!string.IsNullOrWhiteSpace(tipoOcorrencia))
            {
                var queryOcorrencias = this.SessionNHiBernate.Query<Dominio.Entidades.OcorrenciaDeCTe>();

                if (tipoOcorrencia == "F")
                    result = result.Where(o => (from obj in queryOcorrencias where obj.CTe.Codigo == o.CTE.Codigo && obj.Ocorrencia.Tipo.Equals("F") select obj.CTe.Codigo).Contains(o.CTE.Codigo));
                else
                    result = result.Where(o => !(from obj in queryOcorrencias where obj.CTe.Codigo == o.CTE.Codigo && obj.Ocorrencia.Tipo.Equals("F") select obj.CTe.Codigo).Contains(o.CTE.Codigo));
            }

            if (statusPagamento.HasValue)
            {
                var queryDuplicatas = this.SessionNHiBernate.Query<Dominio.Entidades.ParcelaCobrancaCTe>();

                result = result.Where(o => (from obj in queryDuplicatas select obj.Cobranca.CTe.Codigo).Contains(o.CTE.Codigo));

                if (statusPagamento.Value == Dominio.Enumeradores.StatusDuplicata.Paga)
                    result = result.Where(o => !(from obj in queryDuplicatas where obj.Status == Dominio.Enumeradores.StatusDuplicata.Pendente select obj.Cobranca.CTe.Codigo).Contains(o.CTE.Codigo));
                else
                    result = result.Where(o => (from obj in queryDuplicatas where obj.Status == Dominio.Enumeradores.StatusDuplicata.Pendente select obj.Cobranca.CTe.Codigo).Contains(o.CTE.Codigo));
            }

            if (!string.IsNullOrWhiteSpace(duplicata))
            {
                var queryDuplicata = this.SessionNHiBernate.Query<Dominio.Entidades.DuplicataCtes>();

                result = result.Where(o => (from obj in queryDuplicata where obj.Duplicata.Numero == int.Parse(duplicata) select obj.ConhecimentoDeTransporteEletronico.Codigo).Contains(o.Codigo));
            }

            if (icmsCTe == "0")
                result = result.Where(o => o.CTE.ValorICMS == 0);
            else
            if (icmsCTe == "1")
                result = result.Where(o => o.CTE.ValorICMS > 0);

            if (cstCTe != "0")
                result = result.Where(o => o.CTE.CST.Equals(cstCTe));

            if (tipoServico != null)
                result = result.Where(o => o.CTE.TipoServico == tipoServico);

            if (codigoUsuario > 0 && !string.IsNullOrWhiteSpace(nomeUsuario))
                result = result.Where(o => o.CTE.Usuario.Codigo == codigoUsuario);

            if (!string.IsNullOrWhiteSpace(observacao))
                result = result.Where(o => o.CTE.ObservacoesGerais.Contains(observacao));

            if (numeroCarga > 0 || numeroUnidade > 0)
            {
                var queryIntegracaoCTe = this.SessionNHiBernate.Query<Dominio.Entidades.IntegracaoCTe>();
                var resultIntegracaoCTe = from o in queryIntegracaoCTe select o;
                if (numeroCarga > 0)
                    resultIntegracaoCTe = resultIntegracaoCTe.Where(o => o.NumeroDaCarga == numeroCarga);
                if (numeroUnidade > 0)
                    resultIntegracaoCTe = resultIntegracaoCTe.Where(o => o.NumeroDaUnidade == numeroUnidade);

                result = result.Where(o => resultIntegracaoCTe.Select(c => c.CTe.Codigo).Contains(o.Codigo));
            }

            return result.Select(o => new Dominio.ObjetosDeValor.Relatorios.RelatorioCTesSumarizadoPorDocumento()
            {
                CEPDestinatario = o.CTE.Destinatario.CEP,
                CidadeDestinatario = o.CTE.Destinatario.Localidade.Descricao,
                CodigoCTe = o.CTE.Codigo,
                DataEmissaoCTe = o.CTE.DataEmissao,
                NumeroCTe = o.CTE.Numero,
                NumeroDocumento = !string.IsNullOrWhiteSpace(o.Serie) ? o.Numero + "/" + o.Serie : !string.IsNullOrWhiteSpace(o.ChaveNFE) ? o.Numero + "/" + o.ChaveNFE.Substring(22, 3) : o.Numero,
                PesoDocumento = o.Peso,
                SerieCTe = o.CTE.Serie.Numero,
                UFDestinatario = o.CTE.Destinatario.Localidade.Estado.Sigla,
                ValorDocumento = o.Valor,
                PesoCTe = (from obj in this.SessionNHiBernate.Query<Dominio.Entidades.InformacaoCargaCTE>() where obj.CTE.Codigo == o.CTE.Codigo && obj.UnidadeMedida == "01" select (decimal?)obj.Quantidade).Sum() ?? 0m,
                PesoCTeTotal = (from obj in this.SessionNHiBernate.Query<Dominio.Entidades.InformacaoCargaCTE>() where obj.CTE.Codigo == o.CTE.Codigo select (decimal?)obj.Quantidade).Sum() ?? 0m,
                ValorFrete = o.CTE.ValorFrete,
                ValorAReceber = o.CTE.ValorAReceber,
                Remetente = o.CTE.Remetente.Nome,
                Destinatario = o.CTE.Destinatario.Nome,
                UFInicio = o.CTE.LocalidadeInicioPrestacao.Estado.Sigla,
                UFFim = o.CTE.LocalidadeTerminoPrestacao.Estado.Sigla,
                Placa = (from obj in this.SessionNHiBernate.Query<Dominio.Entidades.VeiculoCTE>() where obj.CTE.Codigo == o.Codigo select obj.Placa).FirstOrDefault() ?? string.Empty,
                BaseCalculoICMS = o.CTE.BaseCalculoICMS,
                ValorMercadoria = o.CTE.ValorTotalMercadoria,
                ObservacaoCTe = o.CTE.ListaObservacoesContribuinte + " | " + o.CTE.ListaObservacoesFisco,
                Usuario = o.CTE.Usuario != null && !string.IsNullOrWhiteSpace(o.CTE.Usuario.Nome) ? o.CTE.Usuario.Nome : string.Empty,
                ValorICMS = o.ValorICMS,
                Pagamento = o.CTE.TipoPagamento == Dominio.Enumeradores.TipoPagamento.Pago ? "CIF" : "FOB",
            }).Timeout(120).ToList();
        }

        public List<Dominio.ObjetosDeValor.Relatorios.RelatorioCTesEmitidos> RelatorioEmissaoPorVeiculo(int codigoEmpresa, DateTime dataEmissaoInicial, DateTime dataEmissaoFinal, string status, Dominio.Enumeradores.TipoCTE tipoCTe, int codigoSerie, string cpfCnpjRemetente, string cpfCnpjExpedidor, string cpfCnpjRecebedor, string cpfCnpjDestinatario, string cpfCnpjTomador, int numeroInicial, int numeroFinal, string nomeMotorista, string cpfMotorista, string placaVeiculo, Dominio.Enumeradores.TipoAmbiente tipoAmbiente, int[] series, string tipoOcorrencia, string numeroNotaFiscal, Dominio.Enumeradores.StatusDuplicata? statusPagamento, int codigoLocalidadeInicioPrestacao, int codigoLocalidadeTerminoPrestacao, string ufInicio, string ufFim, string duplicata, bool importacao, bool exportacao, bool raizCNPJRemetente, bool raizCNPJExpedidor, bool raizCNPJRecebedor, bool raizCNPJDestinatario, bool raizCNPJTomador, string icmsCTe, Dominio.Enumeradores.TipoServico? tipoServico, string cstCTe, int codigoUsuario, string nomeUsuario, DateTime dataAutorizacaoInicial, DateTime dataAutorizacaoFinal, string observacao, bool removerCliente, int numeroCarga, int numeroUnidade)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.VeiculoCTE>();

            var result = from obj in query where obj.CTE.Empresa.Codigo == codigoEmpresa && obj.CTE.TipoAmbiente == tipoAmbiente && obj.TipoVeiculo.Equals("0") select obj;

            result = result.Where(o => (o.CTE.ModeloDocumentoFiscal.Numero.Equals("57") || o.CTE.ModeloDocumentoFiscal.Numero.Equals("67")));

            if (dataEmissaoInicial != DateTime.MinValue)
                result = result.Where(o => o.CTE.DataEmissao >= dataEmissaoInicial.Date);

            if (dataEmissaoFinal != DateTime.MinValue)
                result = result.Where(o => o.CTE.DataEmissao < dataEmissaoFinal.AddDays(1).Date);

            if (dataAutorizacaoInicial != DateTime.MinValue)
                result = result.Where(o => o.CTE.DataRetornoSefaz >= dataAutorizacaoInicial.Date);

            if (dataAutorizacaoFinal != DateTime.MinValue)
                result = result.Where(o => o.CTE.DataRetornoSefaz < dataAutorizacaoFinal.AddDays(1).Date);

            if (!string.IsNullOrWhiteSpace(status))
                result = result.Where(o => o.CTE.Status.Equals(status));

            if (tipoCTe != Dominio.Enumeradores.TipoCTE.Todos)
                result = result.Where(o => o.CTE.TipoCTE == tipoCTe);

            if (!string.IsNullOrWhiteSpace(cpfCnpjRemetente))
            {
                if (removerCliente)
                {
                    if (raizCNPJRemetente)
                        result = result.Where(o => !o.CTE.Remetente.CPF_CNPJ.Contains(cpfCnpjRemetente.Substring(0, 8)));
                    else
                        result = result.Where(o => o.CTE.Remetente.CPF_CNPJ != cpfCnpjRemetente);
                }
                else
                {
                    if (raizCNPJRemetente)
                        result = result.Where(o => o.CTE.Remetente.CPF_CNPJ.Contains(cpfCnpjRemetente.Substring(0, 8)));
                    else
                        result = result.Where(o => o.CTE.Remetente.CPF_CNPJ == cpfCnpjRemetente);
                }
            }

            if (importacao)
                result = result.Where(o => o.CTE.Remetente.Exterior == true);

            if (!string.IsNullOrWhiteSpace(cpfCnpjDestinatario))
            {
                if (removerCliente)
                {
                    if (raizCNPJDestinatario)
                        result = result.Where(o => !o.CTE.Destinatario.CPF_CNPJ.Contains(cpfCnpjDestinatario.Substring(0, 8)));
                    else
                        result = result.Where(o => o.CTE.Destinatario.CPF_CNPJ != cpfCnpjDestinatario);
                }
                else
                {
                    if (raizCNPJDestinatario)
                        result = result.Where(o => o.CTE.Destinatario.CPF_CNPJ.Contains(cpfCnpjDestinatario.Substring(0, 8)));
                    else
                        result = result.Where(o => o.CTE.Destinatario.CPF_CNPJ == cpfCnpjDestinatario);
                }
            }

            if (exportacao)
                result = result.Where(o => o.CTE.Destinatario.Exterior == true);

            if (!string.IsNullOrWhiteSpace(cpfCnpjExpedidor))
            {
                if (removerCliente)
                {
                    if (raizCNPJExpedidor)
                        result = result.Where(o => !o.CTE.Expedidor.CPF_CNPJ.Contains(cpfCnpjExpedidor.Substring(0, 8)));
                    else
                        result = result.Where(o => o.CTE.Expedidor.CPF_CNPJ != cpfCnpjExpedidor);
                }
                else
                {
                    if (raizCNPJExpedidor)
                        result = result.Where(o => o.CTE.Expedidor.CPF_CNPJ.Contains(cpfCnpjExpedidor.Substring(0, 8)));
                    else
                        result = result.Where(o => o.CTE.Expedidor.CPF_CNPJ == cpfCnpjExpedidor);
                }
            }


            if (!string.IsNullOrWhiteSpace(cpfCnpjRecebedor))
            {
                if (removerCliente)
                {
                    if (raizCNPJRecebedor)
                        result = result.Where(o => !o.CTE.Recebedor.CPF_CNPJ.Contains(cpfCnpjRecebedor.Substring(0, 8)));
                    else
                        result = result.Where(o => o.CTE.Recebedor.CPF_CNPJ != cpfCnpjRecebedor);
                }
                else
                {
                    if (raizCNPJRecebedor)
                        result = result.Where(o => o.CTE.Recebedor.CPF_CNPJ.Contains(cpfCnpjRecebedor.Substring(0, 8)));
                    else
                        result = result.Where(o => o.CTE.Recebedor.CPF_CNPJ == cpfCnpjRecebedor);
                }
            }

            if (codigoLocalidadeInicioPrestacao > 0)
                result = result.Where(o => o.CTE.LocalidadeInicioPrestacao.Codigo == codigoLocalidadeInicioPrestacao);

            if (codigoLocalidadeTerminoPrestacao > 0)
                result = result.Where(o => o.CTE.LocalidadeTerminoPrestacao.Codigo == codigoLocalidadeTerminoPrestacao);

            if (!string.IsNullOrWhiteSpace(ufInicio))
                result = result.Where(o => o.CTE.LocalidadeInicioPrestacao.Estado.Sigla == ufInicio);

            if (!string.IsNullOrWhiteSpace(ufFim))
                result = result.Where(o => o.CTE.LocalidadeTerminoPrestacao.Estado.Sigla == ufFim);

            if (!string.IsNullOrWhiteSpace(cpfCnpjTomador))
            {
                //if (raizCNPJTomador)
                //    result = result.Where(o => (o.CTE.TipoTomador == Dominio.Enumeradores.TipoTomador.Destinatario && o.CTE.Destinatario.CPF_CNPJ.Contains(cpfCnpjTomador.Substring(0, 8))) ||
                //                           (o.CTE.TipoTomador == Dominio.Enumeradores.TipoTomador.Expedidor && o.CTE.Expedidor.CPF_CNPJ.Contains(cpfCnpjTomador.Substring(0, 8))) ||
                //                           (o.CTE.TipoTomador == Dominio.Enumeradores.TipoTomador.Outros && o.CTE.OutrosTomador.CPF_CNPJ.Contains(cpfCnpjTomador.Substring(0, 8))) ||
                //                           (o.CTE.TipoTomador == Dominio.Enumeradores.TipoTomador.Recebedor && o.CTE.Recebedor.CPF_CNPJ.Contains(cpfCnpjTomador.Substring(0, 8))) ||
                //                           (o.CTE.TipoTomador == Dominio.Enumeradores.TipoTomador.Remetente && o.CTE.Remetente.CPF_CNPJ.Contains(cpfCnpjTomador.Substring(0, 8))));
                //else
                //    result = result.Where(o => (o.CTE.TipoTomador == Dominio.Enumeradores.TipoTomador.Destinatario && o.CTE.Destinatario.CPF_CNPJ == cpfCnpjTomador) ||
                //                               (o.CTE.TipoTomador == Dominio.Enumeradores.TipoTomador.Expedidor && o.CTE.Expedidor.CPF_CNPJ == cpfCnpjTomador) ||
                //                               (o.CTE.TipoTomador == Dominio.Enumeradores.TipoTomador.Outros && o.CTE.OutrosTomador.CPF_CNPJ == cpfCnpjTomador) ||
                //                               (o.CTE.TipoTomador == Dominio.Enumeradores.TipoTomador.Recebedor && o.CTE.Recebedor.CPF_CNPJ == cpfCnpjTomador) ||
                //                               (o.CTE.TipoTomador == Dominio.Enumeradores.TipoTomador.Remetente && o.CTE.Remetente.CPF_CNPJ == cpfCnpjTomador));
                if (removerCliente)
                {
                    if (raizCNPJTomador)
                        result = result.Where(o => !o.CTE.TomadorPagador.CPF_CNPJ.Contains(cpfCnpjTomador.Substring(0, 8)));
                    else
                        result = result.Where(o => o.CTE.TomadorPagador.CPF_CNPJ != cpfCnpjTomador);
                }
                else
                {
                    if (raizCNPJTomador)
                        result = result.Where(o => o.CTE.TomadorPagador.CPF_CNPJ.Contains(cpfCnpjTomador.Substring(0, 8)));
                    else
                        result = result.Where(o => o.CTE.TomadorPagador.CPF_CNPJ == cpfCnpjTomador);
                }
            }

            if (numeroInicial > 0)
                result = result.Where(o => o.CTE.Numero >= numeroInicial);

            if (numeroFinal > 0)
                result = result.Where(o => o.CTE.Numero <= numeroFinal);

            if (codigoSerie > 0)
                result = result.Where(o => o.CTE.Serie.Codigo == codigoSerie);
            else if (series.Count() > 0)
                result = result.Where(o => series.Contains(o.CTE.Serie.Codigo));

            if (!string.IsNullOrWhiteSpace(cpfMotorista) || !string.IsNullOrWhiteSpace(nomeMotorista))
            {
                var queryMotorista = this.SessionNHiBernate.Query<Dominio.Entidades.MotoristaCTE>();

                if (!string.IsNullOrWhiteSpace(cpfMotorista))
                    result = result.Where(o => (from obj in queryMotorista where obj.CPFMotorista.Equals(cpfMotorista) select obj.CTE.Codigo).Contains(o.Codigo));
                else
                    result = result.Where(o => (from obj in queryMotorista where obj.NomeMotoristaCTe.Contains(nomeMotorista) select obj.CTE.Codigo).Contains(o.Codigo));
            }

            if (!string.IsNullOrWhiteSpace(placaVeiculo))
                result = result.Where(o => o.Placa.Equals(placaVeiculo));


            if (!string.IsNullOrWhiteSpace(numeroNotaFiscal))
            {
                var queryDocumentos = this.SessionNHiBernate.Query<Dominio.Entidades.DocumentosCTE>();

                result = result.Where(o => (from obj in queryDocumentos where obj.CTE.Codigo == o.Codigo && obj.Numero.Contains(numeroNotaFiscal) select obj.CTE.Codigo).Contains(o.CTE.Codigo));
            }

            if (!string.IsNullOrWhiteSpace(tipoOcorrencia))
            {
                var queryOcorrencias = this.SessionNHiBernate.Query<Dominio.Entidades.OcorrenciaDeCTe>();

                if (tipoOcorrencia == "F")
                    result = result.Where(o => (from obj in queryOcorrencias where obj.CTe.Codigo == o.Codigo && obj.Ocorrencia.Tipo.Equals("F") select obj.CTe.Codigo).Contains(o.CTE.Codigo));
                else
                    result = result.Where(o => !(from obj in queryOcorrencias where obj.CTe.Codigo == o.Codigo && obj.Ocorrencia.Tipo.Equals("F") select obj.CTe.Codigo).Contains(o.CTE.Codigo));
            }

            if (statusPagamento.HasValue)
            {
                var queryDuplicatas = this.SessionNHiBernate.Query<Dominio.Entidades.ParcelaCobrancaCTe>();

                result = result.Where(o => (from obj in queryDuplicatas select obj.Cobranca.CTe.Codigo).Contains(o.Codigo));

                if (statusPagamento.Value == Dominio.Enumeradores.StatusDuplicata.Paga)
                    result = result.Where(o => !(from obj in queryDuplicatas where obj.Status == Dominio.Enumeradores.StatusDuplicata.Pendente select obj.Cobranca.CTe.Codigo).Contains(o.Codigo));
                else
                    result = result.Where(o => (from obj in queryDuplicatas where obj.Status == Dominio.Enumeradores.StatusDuplicata.Pendente select obj.Cobranca.CTe.Codigo).Contains(o.Codigo));
            }

            if (!string.IsNullOrWhiteSpace(duplicata))
            {
                var queryDuplicata = this.SessionNHiBernate.Query<Dominio.Entidades.DuplicataCtes>();

                result = result.Where(o => (from obj in queryDuplicata where obj.Duplicata.Numero == int.Parse(duplicata) select obj.ConhecimentoDeTransporteEletronico.Codigo).Contains(o.Codigo));
            }

            if (icmsCTe == "0")
                result = result.Where(o => o.CTE.ValorICMS == 0);
            else
            if (icmsCTe == "1")
                result = result.Where(o => o.CTE.ValorICMS > 0);

            if (cstCTe != "0")
                result = result.Where(o => o.CTE.CST.Equals(cstCTe));

            if (tipoServico != null)
                result = result.Where(o => o.CTE.TipoServico == tipoServico);

            if (codigoUsuario > 0 && !string.IsNullOrWhiteSpace(nomeUsuario))
                result = result.Where(o => o.CTE.Usuario.Codigo == codigoUsuario);//|| o.Log.Contains(nomeUsuario)

            if (!string.IsNullOrWhiteSpace(observacao))
                result = result.Where(o => o.CTE.ObservacoesGerais.Contains(observacao));

            if (numeroCarga > 0 || numeroUnidade > 0)
            {
                var queryIntegracaoCTe = this.SessionNHiBernate.Query<Dominio.Entidades.IntegracaoCTe>();
                var resultIntegracaoCTe = from o in queryIntegracaoCTe select o;
                if (numeroCarga > 0)
                    resultIntegracaoCTe = resultIntegracaoCTe.Where(o => o.NumeroDaCarga == numeroCarga);
                if (numeroUnidade > 0)
                    resultIntegracaoCTe = resultIntegracaoCTe.Where(o => o.NumeroDaUnidade == numeroUnidade);

                result = result.Where(o => resultIntegracaoCTe.Select(c => c.CTe.Codigo).Contains(o.Codigo));
            }

            return result.Select(o => new Dominio.ObjetosDeValor.Relatorios.RelatorioCTesEmitidos()
            {
                CODIGO = o.CTE.Codigo,
                CODIGO_CTE = o.CTE.Numero,
                NUMERO_DOCUMENTO = "",
                //NUMERO_DOCUMENTO = exibirNumeroDocumento ? String.Join(",", (from d in this.SessionNHiBernate.Query<Dominio.Entidades.DocumentosCTE>() where d.CTE.Codigo == o.CTE.Codigo select !string.IsNullOrWhiteSpace(d.Serie) ? d.Numero + "/" + d.Serie : !string.IsNullOrWhiteSpace(d.ChaveNFE) ? d.Numero + "/" + d.ChaveNFE.Substring(22, 3) : d.Numero)) : "",
                MOTORISTA = "",
                CST = o.CTE.CST,
                VALOR_MERCADORIA = o.CTE.ValorTotalMercadoria,
                DATA_EMISSAO_CTE = o.CTE.DataEmissao.Value,
                DESTINATARIO_EXTERIOR = o.CTE.Destinatario != null ? o.CTE.Destinatario.Exterior : false,
                FRETE_ICMS = o.CTE.ValorFrete,
                UF_EMPRESA = o.CTE.Empresa.Localidade.Estado.Sigla,
                NOME_CIDADE_FIM = o.CTE.LocalidadeTerminoPrestacao.Descricao,
                NOME_CIDADE_FIM_EXTERIOR = o.CTE.Destinatario != null ? o.CTE.Destinatario.Cidade : string.Empty,
                NOME_CIDADE_INICIO = o.CTE.LocalidadeInicioPrestacao.Descricao,
                NOME_CIDADE_INICIO_EXTERIOR = o.CTE.Remetente != null ? o.CTE.Remetente.Cidade : string.Empty,
                NOME_RAZAO = o.CTE.Remetente != null ? o.CTE.Remetente.Nome : string.Empty,
                NOME_RAZAO_1 = o.CTE.Destinatario != null ? o.CTE.Destinatario.Nome : string.Empty,
                PAIS_FIM = o.CTE.Destinatario != null && o.CTE.Destinatario.Pais != null ? o.CTE.Destinatario.Pais.Nome : string.Empty,
                PAIS_INICIO = o.CTE.Remetente != null && o.CTE.Remetente.Pais != null ? o.CTE.Remetente.Pais.Nome : string.Empty,
                REMETENTE_EXTERIOR = o.CTE.Remetente != null ? o.CTE.Remetente.Exterior : false,
                SERIE = o.CTE.Serie.Numero,
                Status_Abrev = o.CTE.Status,
                TIPO_CTE = o.CTE.TipoCTE,
                UF_FIM = o.CTE.LocalidadeTerminoPrestacao.Estado.Sigla,
                UF_INICIO = o.CTE.LocalidadeInicioPrestacao.Estado.Sigla,
                BASE_CALCULO_ICMS = o.CTE.BaseCalculoICMS,
                ALIQUOTA_ICMS = o.CTE.AliquotaICMS,
                VALOR_ICMS = o.CTE.ValorICMS,
                CONDICAO_PAGAMENTO = o.CTE.TipoPagamento == Dominio.Enumeradores.TipoPagamento.Pago ? "CIF" : "FOB",
                VALOR_RECEBER = Math.Round(o.CTE.ValorAReceber, 2, MidpointRounding.ToEven),
                QUANTIDADE = (from obj in this.SessionNHiBernate.Query<Dominio.Entidades.InformacaoCargaCTE>() where obj.CTE.Codigo == o.CTE.Codigo && obj.UnidadeMedida == "01" select (decimal?)obj.Quantidade).Sum() ?? 0m,
                VOLUME = (from obj in this.SessionNHiBernate.Query<Dominio.Entidades.InformacaoCargaCTE>() where obj.CTE.Codigo == o.CTE.Codigo && obj.UnidadeMedida == "03" select (decimal?)obj.Quantidade).Sum() ?? 0m,
                QUANTIDADE_TOTAL = (from obj in this.SessionNHiBernate.Query<Dominio.Entidades.InformacaoCargaCTE>() where obj.CTE.Codigo == o.CTE.Codigo select (decimal?)obj.Quantidade).Sum() ?? 0m,
                PLACA = o.Placa
            }).Timeout(120).ToList();
        }

        public List<Dominio.ObjetosDeValor.Relatorios.RelatorioCTesEmitidos> RelatorioEmissaoComponentes(int codigoEmpresa, DateTime dataEmissaoInicial, DateTime dataEmissaoFinal, string status, Dominio.Enumeradores.TipoCTE tipoCTe, int codigoSerie, string cpfCnpjRemetente, string cpfCnpjExpedidor, string cpfCnpjRecebedor, string cpfCnpjDestinatario, string cpfCnpjTomador, int numeroInicial, int numeroFinal, string nomeMotorista, string cpfMotorista, string placaVeiculo, Dominio.Enumeradores.TipoAmbiente tipoAmbiente, int[] series, string tipoOcorrencia, string numeroNotaFiscal, Dominio.Enumeradores.StatusDuplicata? statusPagamento, int codigoLocalidadeInicioPrestacao, int codigoLocalidadeTerminoPrestacao, string ufInicio, string ufFim, string duplicata, bool importacao, bool exportacao, bool raizCNPJRemetente, bool raizCNPJExpedidor, bool raizCNPJRecebedor, bool raizCNPJDestinatario, bool raizCNPJTomador, string icmsCTe, Dominio.Enumeradores.TipoServico? tipoServico, string cstCTe, int codigoUsuario, string nomeUsuario, DateTime dataAutorizacaoInicial, DateTime dataAutorizacaoFinal, bool exibirCNPJParticipantes, string observacao, bool removerCliente, int numeroCarga, int numeroUnidade)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.ConhecimentoDeTransporteEletronico>();

            var result = from obj in query where obj.Empresa.Codigo == codigoEmpresa && obj.TipoAmbiente == tipoAmbiente select obj;

            result = result.Where(o => (o.ModeloDocumentoFiscal.Numero.Equals("57") || o.ModeloDocumentoFiscal.Numero.Equals("67")));

            if (dataEmissaoInicial != DateTime.MinValue)
                result = result.Where(o => o.DataEmissao >= dataEmissaoInicial.Date);

            if (dataEmissaoFinal != DateTime.MinValue)
                result = result.Where(o => o.DataEmissao < dataEmissaoFinal.AddDays(1).Date);

            if (dataAutorizacaoInicial != DateTime.MinValue)
                result = result.Where(o => o.DataRetornoSefaz >= dataAutorizacaoInicial.Date);

            if (dataAutorizacaoFinal != DateTime.MinValue)
                result = result.Where(o => o.DataRetornoSefaz < dataAutorizacaoFinal.AddDays(1).Date);

            if (!string.IsNullOrWhiteSpace(status))
                result = result.Where(o => o.Status.Equals(status));

            if (codigoUsuario > 0 && !string.IsNullOrWhiteSpace(nomeUsuario))
                result = result.Where(o => o.Usuario.Codigo == codigoUsuario);//|| o.Log.Contains(nomeUsuario)

            if (tipoCTe != Dominio.Enumeradores.TipoCTE.Todos)
                result = result.Where(o => o.TipoCTE == tipoCTe);

            if (!string.IsNullOrWhiteSpace(cpfCnpjRemetente))
            {
                if (removerCliente)
                {
                    if (raizCNPJRemetente)
                        result = result.Where(o => !o.Remetente.CPF_CNPJ.Contains(cpfCnpjRemetente.Substring(0, 8)));
                    else
                        result = result.Where(o => o.Remetente.CPF_CNPJ != cpfCnpjRemetente);
                }
                else
                {
                    if (raizCNPJRemetente)
                        result = result.Where(o => o.Remetente.CPF_CNPJ.Contains(cpfCnpjRemetente.Substring(0, 8)));
                    else
                        result = result.Where(o => o.Remetente.CPF_CNPJ == cpfCnpjRemetente);
                }
            }

            if (importacao)
                result = result.Where(o => o.Remetente.Exterior == true);

            if (!string.IsNullOrWhiteSpace(cpfCnpjDestinatario))
            {
                if (removerCliente)
                {
                    if (raizCNPJDestinatario)
                        result = result.Where(o => !o.Destinatario.CPF_CNPJ.Contains(cpfCnpjDestinatario.Substring(0, 8)));
                    else
                        result = result.Where(o => o.Destinatario.CPF_CNPJ != cpfCnpjDestinatario);
                }
                else
                {
                    if (raizCNPJDestinatario)
                        result = result.Where(o => o.Destinatario.CPF_CNPJ.Contains(cpfCnpjDestinatario.Substring(0, 8)));
                    else
                        result = result.Where(o => o.Destinatario.CPF_CNPJ == cpfCnpjDestinatario);
                }
            }

            if (exportacao)
                result = result.Where(o => o.Destinatario.Exterior == true);

            if (!string.IsNullOrWhiteSpace(cpfCnpjExpedidor))
            {
                if (removerCliente)
                {
                    if (raizCNPJExpedidor)
                        result = result.Where(o => !o.Expedidor.CPF_CNPJ.Contains(cpfCnpjExpedidor.Substring(0, 8)));
                    else
                        result = result.Where(o => o.Expedidor.CPF_CNPJ != cpfCnpjExpedidor);
                }
                {
                    if (raizCNPJExpedidor)
                        result = result.Where(o => o.Expedidor.CPF_CNPJ.Contains(cpfCnpjExpedidor.Substring(0, 8)));
                    else
                        result = result.Where(o => o.Expedidor.CPF_CNPJ == cpfCnpjExpedidor);
                }
            }


            if (!string.IsNullOrWhiteSpace(cpfCnpjRecebedor))
            {
                if (removerCliente)
                {
                    if (raizCNPJRecebedor)
                        result = result.Where(o => !o.Recebedor.CPF_CNPJ.Contains(cpfCnpjRecebedor.Substring(0, 8)));
                    else
                        result = result.Where(o => o.Recebedor.CPF_CNPJ != cpfCnpjRecebedor);
                }
                else
                {
                    if (raizCNPJRecebedor)
                        result = result.Where(o => o.Recebedor.CPF_CNPJ.Contains(cpfCnpjRecebedor.Substring(0, 8)));
                    else
                        result = result.Where(o => o.Recebedor.CPF_CNPJ == cpfCnpjRecebedor);
                }
            }

            if (codigoLocalidadeInicioPrestacao > 0)
                result = result.Where(o => o.LocalidadeInicioPrestacao.Codigo == codigoLocalidadeInicioPrestacao);

            if (codigoLocalidadeTerminoPrestacao > 0)
                result = result.Where(o => o.LocalidadeTerminoPrestacao.Codigo == codigoLocalidadeTerminoPrestacao);

            if (!string.IsNullOrWhiteSpace(ufInicio))
                result = result.Where(o => o.LocalidadeInicioPrestacao.Estado.Sigla == ufInicio);

            if (!string.IsNullOrWhiteSpace(ufFim))
                result = result.Where(o => o.LocalidadeTerminoPrestacao.Estado.Sigla == ufFim);

            if (!string.IsNullOrWhiteSpace(cpfCnpjTomador))
            {
                //if (raizCNPJTomador)
                //    result = result.Where(o => (o.TipoTomador == Dominio.Enumeradores.TipoTomador.Destinatario && o.Destinatario.CPF_CNPJ.Contains(cpfCnpjTomador.Substring(0, 8))) ||
                //                           (o.TipoTomador == Dominio.Enumeradores.TipoTomador.Expedidor && o.Expedidor.CPF_CNPJ.Contains(cpfCnpjTomador.Substring(0, 8))) ||
                //                           (o.TipoTomador == Dominio.Enumeradores.TipoTomador.Outros && o.OutrosTomador.CPF_CNPJ.Contains(cpfCnpjTomador.Substring(0, 8))) ||
                //                           (o.TipoTomador == Dominio.Enumeradores.TipoTomador.Recebedor && o.Recebedor.CPF_CNPJ.Contains(cpfCnpjTomador.Substring(0, 8))) ||
                //                           (o.TipoTomador == Dominio.Enumeradores.TipoTomador.Remetente && o.Remetente.CPF_CNPJ.Contains(cpfCnpjTomador.Substring(0, 8))));
                //else
                //    result = result.Where(o => (o.TipoTomador == Dominio.Enumeradores.TipoTomador.Destinatario && o.Destinatario.CPF_CNPJ == cpfCnpjTomador) ||
                //                               (o.TipoTomador == Dominio.Enumeradores.TipoTomador.Expedidor && o.Expedidor.CPF_CNPJ == cpfCnpjTomador) ||
                //                               (o.TipoTomador == Dominio.Enumeradores.TipoTomador.Outros && o.OutrosTomador.CPF_CNPJ == cpfCnpjTomador) ||
                //                               (o.TipoTomador == Dominio.Enumeradores.TipoTomador.Recebedor && o.Recebedor.CPF_CNPJ == cpfCnpjTomador) ||
                //                               (o.TipoTomador == Dominio.Enumeradores.TipoTomador.Remetente && o.Remetente.CPF_CNPJ == cpfCnpjTomador));
                if (removerCliente)
                {
                    if (raizCNPJTomador)
                        result = result.Where(o => !o.TomadorPagador.CPF_CNPJ.Contains(cpfCnpjTomador.Substring(0, 8)));
                    else
                        result = result.Where(o => o.TomadorPagador.CPF_CNPJ != cpfCnpjTomador);
                }
                else
                {
                    if (raizCNPJTomador)
                        result = result.Where(o => o.TomadorPagador.CPF_CNPJ.Contains(cpfCnpjTomador.Substring(0, 8)));
                    else
                        result = result.Where(o => o.TomadorPagador.CPF_CNPJ == cpfCnpjTomador);
                }
            }

            if (numeroInicial > 0)
                result = result.Where(o => o.Numero >= numeroInicial);

            if (numeroFinal > 0)
                result = result.Where(o => o.Numero <= numeroFinal);

            if (codigoSerie > 0)
                result = result.Where(o => o.Serie.Codigo == codigoSerie);
            else if (series.Count() > 0)
                result = result.Where(o => series.Contains(o.Serie.Codigo));

            if (!string.IsNullOrWhiteSpace(cpfMotorista) || !string.IsNullOrWhiteSpace(nomeMotorista))
            {
                var queryMotorista = this.SessionNHiBernate.Query<Dominio.Entidades.MotoristaCTE>();

                if (!string.IsNullOrWhiteSpace(cpfMotorista))
                    result = result.Where(o => (from obj in queryMotorista where obj.CPFMotorista.Equals(cpfMotorista) select obj.CTE.Codigo).Contains(o.Codigo));
                else
                    result = result.Where(o => (from obj in queryMotorista where obj.NomeMotoristaCTe.Contains(nomeMotorista) select obj.CTE.Codigo).Contains(o.Codigo));
            }

            if (!string.IsNullOrWhiteSpace(placaVeiculo))
            {
                var queryVeiculos = this.SessionNHiBernate.Query<Dominio.Entidades.VeiculoCTE>();

                result = result.Where(o => (from obj in queryVeiculos where obj.CTE.Codigo == o.Codigo && obj.Placa.Equals(placaVeiculo) select obj.CTE.Codigo).Contains(o.Codigo));
            }


            if (!string.IsNullOrWhiteSpace(numeroNotaFiscal))
            {
                var queryDocumentos = this.SessionNHiBernate.Query<Dominio.Entidades.DocumentosCTE>();

                result = result.Where(o => (from obj in queryDocumentos where obj.CTE.Codigo == o.Codigo && obj.Numero.Contains(numeroNotaFiscal) select obj.CTE.Codigo).Contains(o.Codigo));
            }

            if (!string.IsNullOrWhiteSpace(tipoOcorrencia))
            {
                var queryOcorrencias = this.SessionNHiBernate.Query<Dominio.Entidades.OcorrenciaDeCTe>();

                if (tipoOcorrencia == "F")
                    result = result.Where(o => (from obj in queryOcorrencias where obj.CTe.Codigo == o.Codigo && obj.Ocorrencia.Tipo.Equals("F") select obj.CTe.Codigo).Contains(o.Codigo));
                else
                    result = result.Where(o => !(from obj in queryOcorrencias where obj.CTe.Codigo == o.Codigo && obj.Ocorrencia.Tipo.Equals("F") select obj.CTe.Codigo).Contains(o.Codigo));
            }

            if (statusPagamento.HasValue)
            {
                var queryDuplicatas = this.SessionNHiBernate.Query<Dominio.Entidades.ParcelaCobrancaCTe>();

                result = result.Where(o => (from obj in queryDuplicatas select obj.Cobranca.CTe.Codigo).Contains(o.Codigo));

                if (statusPagamento.Value == Dominio.Enumeradores.StatusDuplicata.Paga)
                    result = result.Where(o => !(from obj in queryDuplicatas where obj.Status == Dominio.Enumeradores.StatusDuplicata.Pendente select obj.Cobranca.CTe.Codigo).Contains(o.Codigo));
                else
                    result = result.Where(o => (from obj in queryDuplicatas where obj.Status == Dominio.Enumeradores.StatusDuplicata.Pendente select obj.Cobranca.CTe.Codigo).Contains(o.Codigo));
            }

            if (!string.IsNullOrWhiteSpace(duplicata))
            {
                var queryDuplicata = this.SessionNHiBernate.Query<Dominio.Entidades.DuplicataCtes>();

                result = result.Where(o => (from obj in queryDuplicata where obj.Duplicata.Numero == int.Parse(duplicata) select obj.ConhecimentoDeTransporteEletronico.Codigo).Contains(o.Codigo));
            }

            if (icmsCTe == "0")
                result = result.Where(o => o.ValorICMS == 0);
            else
            if (icmsCTe == "1")
                result = result.Where(o => o.ValorICMS > 0);

            if (cstCTe != "0")
                result = result.Where(o => o.CST.Equals(cstCTe));

            if (tipoServico != null)
                result = result.Where(o => o.TipoServico == tipoServico);

            if (!string.IsNullOrWhiteSpace(observacao))
                result = result.Where(o => o.ObservacoesGerais.Contains(observacao));

            if (numeroCarga > 0 || numeroUnidade > 0)
            {
                var queryIntegracaoCTe = this.SessionNHiBernate.Query<Dominio.Entidades.IntegracaoCTe>();
                var resultIntegracaoCTe = from o in queryIntegracaoCTe select o;
                if (numeroCarga > 0)
                    resultIntegracaoCTe = resultIntegracaoCTe.Where(o => o.NumeroDaCarga == numeroCarga);
                if (numeroUnidade > 0)
                    resultIntegracaoCTe = resultIntegracaoCTe.Where(o => o.NumeroDaUnidade == numeroUnidade);

                result = result.Where(o => resultIntegracaoCTe.Select(c => c.CTe.Codigo).Contains(o.Codigo));
            }

            return result.Select(o => new Dominio.ObjetosDeValor.Relatorios.RelatorioCTesEmitidos()
            {
                CODIGO = o.Codigo,
                CODIGO_CTE = o.Numero,
                CST = o.CST,
                DATA_EMISSAO_CTE = o.DataEmissao.Value,
                DESTINATARIO_EXTERIOR = o.Destinatario != null ? o.Destinatario.Exterior : false,
                FRETE_ICMS = o.ValorFrete,
                UF_EMPRESA = o.Empresa.Localidade.Estado.Sigla,
                NOME_CIDADE_FIM = o.LocalidadeTerminoPrestacao.Descricao,
                NOME_CIDADE_FIM_EXTERIOR = o.Destinatario != null ? o.Destinatario.Cidade : string.Empty,
                NOME_CIDADE_INICIO = o.LocalidadeInicioPrestacao.Descricao,
                NOME_CIDADE_INICIO_EXTERIOR = o.Remetente != null ? o.Remetente.Cidade : string.Empty,
                NOME_RAZAO = o.Remetente != null ? exibirCNPJParticipantes ? String.Concat(o.Remetente.CPF_CNPJ, " ", o.Remetente.Nome) : o.Remetente.Nome : string.Empty,
                NOME_RAZAO_1 = o.Destinatario != null ? exibirCNPJParticipantes ? String.Concat(o.Destinatario.CPF_CNPJ, " ", o.Destinatario.Nome) : o.Destinatario.Nome : string.Empty,
                PAIS_FIM = o.Destinatario != null && o.Destinatario.Pais != null ? o.Destinatario.Pais.Nome : string.Empty,
                PAIS_INICIO = o.Remetente != null && o.Remetente.Pais != null ? o.Remetente.Pais.Nome : string.Empty,
                REMETENTE_EXTERIOR = o.Remetente != null ? o.Remetente.Exterior : false,
                SERIE = o.Serie.Numero,
                Status_Abrev = o.Status,
                TIPO_CTE = o.TipoCTE,
                UF_FIM = o.LocalidadeTerminoPrestacao.Estado.Sigla,
                UF_INICIO = o.LocalidadeInicioPrestacao.Estado.Sigla,
                BASE_CALCULO_ICMS = o.BaseCalculoICMS,
                ALIQUOTA_ICMS = o.AliquotaICMS,
                VALOR_ICMS = o.ValorICMS,
                VALOR_RECEBER = Math.Round(o.ValorAReceber, 2, MidpointRounding.ToEven),
                QUANTIDADE = (from obj in this.SessionNHiBernate.Query<Dominio.Entidades.InformacaoCargaCTE>() where obj.CTE.Codigo == o.Codigo && obj.UnidadeMedida == "01" select (decimal?)obj.Quantidade).Sum() ?? 0m,
                QUANTIDADE_TOTAL = (from obj in this.SessionNHiBernate.Query<Dominio.Entidades.InformacaoCargaCTE>() where obj.CTE.Codigo == o.Codigo select (decimal?)obj.Quantidade).Sum() ?? 0m,
                PLACA = (from obj in this.SessionNHiBernate.Query<Dominio.Entidades.VeiculoCTE>() where obj.CTE.Codigo == o.Codigo && obj.TipoVeiculo == "0" select obj.Placa).FirstOrDefault() ?? string.Empty,
                VALOR_COMPONENTES = (from obj in this.SessionNHiBernate.Query<Dominio.Entidades.ComponentePrestacaoCTE>() where obj.CTE.Codigo == o.Codigo && !obj.NomeCTe.Contains("FRETE VALOR") && !obj.NomeCTe.Contains("IMPOSTOS") select (decimal?)obj.Valor).Sum() ?? 0m,
                MODELO = o.ModeloDocumentoFiscal.Abreviacao,
                MOTORISTA = (from obj in this.SessionNHiBernate.Query<Dominio.Entidades.MotoristaCTE>() where obj.CTE.Codigo == o.Codigo select obj.NomeMotoristaCTe).FirstOrDefault() ?? string.Empty,
                NOME_RAZAO_TOMADOR = o.TomadorPagador != null ? exibirCNPJParticipantes ? String.Concat(o.TomadorPagador.CPF_CNPJ, " ", o.TomadorPagador.Nome) : o.TomadorPagador.Nome : string.Empty,
                Observacao = o.ObservacoesGerais,
                VALOR_MERCADORIA = o.ValorTotalMercadoria
            }).Timeout(120).ToList();
        }

        public List<int> BuscarListaCodigosCTEs(int codigoEmpresa, DateTime dataEmissaoInicial, DateTime dataEmissaoFinal, string status, Dominio.Enumeradores.TipoCTE tipoCTe, int codigoSerie, string cpfCnpjRemetente, string cpfCnpjExpedidor, string cpfCnpjRecebedor, string cpfCnpjDestinatario, string cpfCnpjTomador, int numeroInicial, int numeroFinal, string nomeMotorista, string cpfMotorista, string placaVeiculo, Dominio.Enumeradores.TipoAmbiente tipoAmbiente, int[] series, string tipoOcorrencia, string numeroNotaFiscal, Dominio.Enumeradores.StatusDuplicata? statusPagamento, int codigoLocalidadeInicioPrestacao, int codigoLocalidadeTerminoPrestacao, string duplicata, bool importacao, bool exportacao, bool raizCNPJRemetente, bool raizCNPJExpedidor, bool raizCNPJRecebedor, bool raizCNPJDestinatario, bool raizCNPJTomador, string icmsCTe, string cstCTe, DateTime dataAutorizacaoInicial, DateTime dataAutorizacaoFinal, string ufInicio, string ufFim, string nomeUsuario, int codigoUsuario, string observacao, bool removerCliente, Dominio.Enumeradores.TipoServico? tipoServico, int numeroCarga, int numeroUnidade)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.ConhecimentoDeTransporteEletronico>();

            var result = from obj in query where obj.Empresa.Codigo == codigoEmpresa && obj.TipoAmbiente == tipoAmbiente select obj;

            result = result.Where(o => (o.ModeloDocumentoFiscal.Numero.Equals("57") || o.ModeloDocumentoFiscal.Numero.Equals("67")));

            if (dataEmissaoInicial != DateTime.MinValue)
                result = result.Where(o => o.DataEmissao >= dataEmissaoInicial.Date);

            if (dataEmissaoFinal != DateTime.MinValue)
                result = result.Where(o => o.DataEmissao < dataEmissaoFinal.AddDays(1).Date);

            if (dataAutorizacaoInicial != DateTime.MinValue)
                result = result.Where(o => o.DataRetornoSefaz >= dataAutorizacaoInicial.Date);

            if (dataAutorizacaoFinal != DateTime.MinValue)
                result = result.Where(o => o.DataRetornoSefaz < dataAutorizacaoFinal.AddDays(1).Date);

            if (!string.IsNullOrWhiteSpace(status))
                result = result.Where(o => o.Status.Equals(status));

            if (tipoCTe != Dominio.Enumeradores.TipoCTE.Todos)
                result = result.Where(o => o.TipoCTE == tipoCTe);

            if (!string.IsNullOrWhiteSpace(cpfCnpjRemetente))
            {
                if (removerCliente)
                {
                    if (raizCNPJRemetente)
                        result = result.Where(o => !o.Remetente.CPF_CNPJ.Contains(cpfCnpjRemetente.Substring(0, 8)));
                    else
                        result = result.Where(o => o.Remetente.CPF_CNPJ != cpfCnpjRemetente);
                }
                else
                {
                    if (raizCNPJRemetente)
                        result = result.Where(o => o.Remetente.CPF_CNPJ.Contains(cpfCnpjRemetente.Substring(0, 8)));
                    else
                        result = result.Where(o => o.Remetente.CPF_CNPJ == cpfCnpjRemetente);
                }
            }

            if (importacao)
                result = result.Where(o => o.Remetente.Exterior == true);

            if (!string.IsNullOrWhiteSpace(cpfCnpjDestinatario))
            {
                if (removerCliente)
                {
                    if (raizCNPJDestinatario)
                        result = result.Where(o => !o.Destinatario.CPF_CNPJ.Contains(cpfCnpjDestinatario.Substring(0, 8)));
                    else
                        result = result.Where(o => o.Destinatario.CPF_CNPJ != cpfCnpjDestinatario);
                }
                else
                {
                    if (raizCNPJDestinatario)
                        result = result.Where(o => o.Destinatario.CPF_CNPJ.Contains(cpfCnpjDestinatario.Substring(0, 8)));
                    else
                        result = result.Where(o => o.Destinatario.CPF_CNPJ == cpfCnpjDestinatario);
                }
            }

            if (exportacao)
                result = result.Where(o => o.Destinatario.Exterior == true);

            if (!string.IsNullOrWhiteSpace(cpfCnpjExpedidor))
            {
                if (removerCliente)
                {
                    if (raizCNPJExpedidor)
                        result = result.Where(o => !o.Expedidor.CPF_CNPJ.Contains(cpfCnpjExpedidor.Substring(0, 8)));
                    else
                        result = result.Where(o => o.Expedidor.CPF_CNPJ != cpfCnpjExpedidor);
                }
                else
                {
                    if (raizCNPJExpedidor)
                        result = result.Where(o => o.Expedidor.CPF_CNPJ.Contains(cpfCnpjExpedidor.Substring(0, 8)));
                    else
                        result = result.Where(o => o.Expedidor.CPF_CNPJ == cpfCnpjExpedidor);
                }
            }

            if (!string.IsNullOrWhiteSpace(cpfCnpjRecebedor))
            {
                if (removerCliente)
                {
                    if (raizCNPJRecebedor)
                        result = result.Where(o => !o.Recebedor.CPF_CNPJ.Contains(cpfCnpjRecebedor.Substring(0, 8)));
                    else
                        result = result.Where(o => o.Recebedor.CPF_CNPJ != cpfCnpjRecebedor);
                }
                else
                {
                    if (raizCNPJRecebedor)
                        result = result.Where(o => o.Recebedor.CPF_CNPJ.Contains(cpfCnpjRecebedor.Substring(0, 8)));
                    else
                        result = result.Where(o => o.Recebedor.CPF_CNPJ == cpfCnpjRecebedor);
                }
            }

            if (codigoLocalidadeInicioPrestacao > 0)
                result = result.Where(o => o.LocalidadeInicioPrestacao.Codigo == codigoLocalidadeInicioPrestacao);

            if (codigoLocalidadeTerminoPrestacao > 0)
                result = result.Where(o => o.LocalidadeTerminoPrestacao.Codigo == codigoLocalidadeTerminoPrestacao);

            if (!string.IsNullOrWhiteSpace(cpfCnpjTomador))
            {
                //if (raizCNPJTomador)
                //    result = result.Where(o => (o.TipoTomador == Dominio.Enumeradores.TipoTomador.Destinatario && o.Destinatario.CPF_CNPJ.Contains(cpfCnpjTomador.Substring(0, 8))) ||
                //                           (o.TipoTomador == Dominio.Enumeradores.TipoTomador.Expedidor && o.Expedidor.CPF_CNPJ.Contains(cpfCnpjTomador.Substring(0, 8))) ||
                //                           (o.TipoTomador == Dominio.Enumeradores.TipoTomador.Outros && o.OutrosTomador.CPF_CNPJ.Contains(cpfCnpjTomador.Substring(0, 8))) ||
                //                           (o.TipoTomador == Dominio.Enumeradores.TipoTomador.Recebedor && o.Recebedor.CPF_CNPJ.Contains(cpfCnpjTomador.Substring(0, 8))) ||
                //                           (o.TipoTomador == Dominio.Enumeradores.TipoTomador.Remetente && o.Remetente.CPF_CNPJ.Contains(cpfCnpjTomador.Substring(0, 8))));
                //else
                //    result = result.Where(o => (o.TipoTomador == Dominio.Enumeradores.TipoTomador.Destinatario && o.Destinatario.CPF_CNPJ == cpfCnpjTomador) ||
                //                               (o.TipoTomador == Dominio.Enumeradores.TipoTomador.Expedidor && o.Expedidor.CPF_CNPJ == cpfCnpjTomador) ||
                //                               (o.TipoTomador == Dominio.Enumeradores.TipoTomador.Outros && o.OutrosTomador.CPF_CNPJ == cpfCnpjTomador) ||
                //                               (o.TipoTomador == Dominio.Enumeradores.TipoTomador.Recebedor && o.Recebedor.CPF_CNPJ == cpfCnpjTomador) ||
                //                               (o.TipoTomador == Dominio.Enumeradores.TipoTomador.Remetente && o.Remetente.CPF_CNPJ == cpfCnpjTomador));
                if (removerCliente)
                {
                    if (raizCNPJTomador)
                        result = result.Where(o => !o.TomadorPagador.CPF_CNPJ.Contains(cpfCnpjTomador.Substring(0, 8)));
                    else
                        result = result.Where(o => o.TomadorPagador.CPF_CNPJ != cpfCnpjTomador);
                }
                else
                {
                    if (raizCNPJTomador)
                        result = result.Where(o => o.TomadorPagador.CPF_CNPJ.Contains(cpfCnpjTomador.Substring(0, 8)));
                    else
                        result = result.Where(o => o.TomadorPagador.CPF_CNPJ == cpfCnpjTomador);
                }
            }


            if (numeroInicial > 0)
                result = result.Where(o => o.Numero >= numeroInicial);

            if (numeroFinal > 0)
                result = result.Where(o => o.Numero <= numeroFinal);

            if (codigoSerie > 0)
                result = result.Where(o => o.Serie.Codigo == codigoSerie);
            else if (series.Count() > 0)
                result = result.Where(o => series.Contains(o.Serie.Codigo));

            if (!string.IsNullOrWhiteSpace(cpfMotorista) || !string.IsNullOrWhiteSpace(nomeMotorista))
            {
                var queryMotorista = this.SessionNHiBernate.Query<Dominio.Entidades.MotoristaCTE>();

                if (!string.IsNullOrWhiteSpace(cpfMotorista))
                    result = result.Where(o => (from obj in queryMotorista where obj.CPFMotorista.Equals(cpfMotorista) select obj.CTE.Codigo).Contains(o.Codigo));
                else
                    result = result.Where(o => (from obj in queryMotorista where obj.NomeMotoristaCTe.Contains(nomeMotorista) select obj.CTE.Codigo).Contains(o.Codigo));
            }

            if (!string.IsNullOrWhiteSpace(placaVeiculo))
            {
                var queryVeiculos = this.SessionNHiBernate.Query<Dominio.Entidades.VeiculoCTE>();

                result = result.Where(o => (from obj in queryVeiculos where obj.CTE.Codigo == o.Codigo && obj.Placa.Equals(placaVeiculo) select obj.CTE.Codigo).Contains(o.Codigo));
            }

            if (!string.IsNullOrWhiteSpace(numeroNotaFiscal))
            {
                var queryDocumentos = this.SessionNHiBernate.Query<Dominio.Entidades.DocumentosCTE>();

                result = result.Where(o => (from obj in queryDocumentos where obj.CTE.Codigo == o.Codigo && obj.Numero.Contains(numeroNotaFiscal) select obj.CTE.Codigo).Contains(o.Codigo));
            }

            if (!string.IsNullOrWhiteSpace(tipoOcorrencia))
            {
                var queryOcorrencias = this.SessionNHiBernate.Query<Dominio.Entidades.OcorrenciaDeCTe>();

                if (tipoOcorrencia == "F")
                    result = result.Where(o => (from obj in queryOcorrencias where obj.CTe.Codigo == o.Codigo && obj.Ocorrencia.Tipo.Equals("F") select obj.CTe.Codigo).Contains(o.Codigo));
                else
                    result = result.Where(o => !(from obj in queryOcorrencias where obj.CTe.Codigo == o.Codigo && obj.Ocorrencia.Tipo.Equals("F") select obj.CTe.Codigo).Contains(o.Codigo));
            }

            if (statusPagamento.HasValue)
            {
                var queryDuplicatas = this.SessionNHiBernate.Query<Dominio.Entidades.ParcelaCobrancaCTe>();

                result = result.Where(o => (from obj in queryDuplicatas select obj.Cobranca.CTe.Codigo).Contains(o.Codigo));

                if (statusPagamento.Value == Dominio.Enumeradores.StatusDuplicata.Paga)
                    result = result.Where(o => !(from obj in queryDuplicatas where obj.Status == Dominio.Enumeradores.StatusDuplicata.Pendente select obj.Cobranca.CTe.Codigo).Contains(o.Codigo));
                else
                    result = result.Where(o => (from obj in queryDuplicatas where obj.Status == Dominio.Enumeradores.StatusDuplicata.Pendente select obj.Cobranca.CTe.Codigo).Contains(o.Codigo));
            }

            if (!string.IsNullOrWhiteSpace(duplicata))
            {
                var queryDuplicata = this.SessionNHiBernate.Query<Dominio.Entidades.DuplicataCtes>();

                result = result.Where(o => (from obj in queryDuplicata where obj.Duplicata.Numero == int.Parse(duplicata) select obj.ConhecimentoDeTransporteEletronico.Codigo).Contains(o.Codigo));
            }

            if (icmsCTe == "0")
                result = result.Where(o => o.ValorICMS == 0);
            else
            if (icmsCTe == "1")
                result = result.Where(o => o.ValorICMS > 0);

            if (cstCTe != "0")
                result = result.Where(o => o.CST.Equals(cstCTe));

            if (codigoUsuario > 0 && !string.IsNullOrWhiteSpace(nomeUsuario))
                result = result.Where(o => o.Usuario.Codigo == codigoUsuario); //|| o.Log.Contains(nomeUsuario)

            if (!string.IsNullOrWhiteSpace(ufInicio))
                result = result.Where(o => o.LocalidadeInicioPrestacao.Estado.Sigla == ufInicio);

            if (!string.IsNullOrWhiteSpace(ufFim))
                result = result.Where(o => o.LocalidadeTerminoPrestacao.Estado.Sigla == ufFim);

            if (!string.IsNullOrWhiteSpace(observacao))
                result = result.Where(o => o.ObservacoesGerais.Contains(observacao));

            if (tipoServico != null)
                result = result.Where(o => o.TipoServico == tipoServico);

            if (numeroCarga > 0 || numeroUnidade > 0)
            {
                var queryIntegracaoCTe = this.SessionNHiBernate.Query<Dominio.Entidades.IntegracaoCTe>();
                var resultIntegracaoCTe = from o in queryIntegracaoCTe select o;
                if (numeroCarga > 0)
                    resultIntegracaoCTe = resultIntegracaoCTe.Where(o => o.NumeroDaCarga == numeroCarga);
                if (numeroUnidade > 0)
                    resultIntegracaoCTe = resultIntegracaoCTe.Where(o => o.NumeroDaUnidade == numeroUnidade);

                result = result.Where(o => resultIntegracaoCTe.Select(c => c.CTe.Codigo).Contains(o.Codigo));
            }

            return result.Select(o => o.Codigo).Timeout(120).ToList();
        }

        public List<string> BuscarListaChavesCTes(int codigoEmpresa, DateTime dataEmissaoInicial, DateTime dataEmissaoFinal, string status, Dominio.Enumeradores.TipoCTE tipoCTe, int codigoSerie, string cpfCnpjRemetente, string cpfCnpjExpedidor, string cpfCnpjRecebedor, string cpfCnpjDestinatario, string cpfCnpjTomador, int numeroInicial, int numeroFinal, string nomeMotorista, string cpfMotorista, string placaVeiculo, Dominio.Enumeradores.TipoAmbiente tipoAmbiente, int[] series, string tipoOcorrencia, string numeroNotaFiscal, Dominio.Enumeradores.StatusDuplicata? statusPagamento, int codigoLocalidadeInicioPrestacao, int codigoLocalidadeTerminoPrestacao, string duplicata, bool importacao, bool exportacao, bool raizCNPJRemetente, bool raizCNPJExpedidor, bool raizCNPJRecebedor, bool raizCNPJDestinatario, bool raizCNPJTomador, string icmsCTe, string cstCTe, DateTime dataAutorizacaoInicial, DateTime dataAutorizacaoFinal, string ufInicio, string ufFim, string nomeUsuario, int codigoUsuario, string observacao, bool removerCliente, Dominio.Enumeradores.TipoServico? tipoServico, int numeroCarga, int numeroUnidade)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.ConhecimentoDeTransporteEletronico>();

            var result = from obj in query where obj.Empresa.Codigo == codigoEmpresa && obj.TipoAmbiente == tipoAmbiente && obj.Chave != null select obj;

            result = result.Where(o => (o.ModeloDocumentoFiscal.Numero.Equals("57") || o.ModeloDocumentoFiscal.Numero.Equals("67")));

            if (dataEmissaoInicial != DateTime.MinValue)
                result = result.Where(o => o.DataEmissao >= dataEmissaoInicial.Date);

            if (dataEmissaoFinal != DateTime.MinValue)
                result = result.Where(o => o.DataEmissao < dataEmissaoFinal.AddDays(1).Date);

            if (dataAutorizacaoInicial != DateTime.MinValue)
                result = result.Where(o => o.DataRetornoSefaz >= dataAutorizacaoInicial.Date);

            if (dataAutorizacaoFinal != DateTime.MinValue)
                result = result.Where(o => o.DataRetornoSefaz < dataAutorizacaoFinal.AddDays(1).Date);

            if (!string.IsNullOrWhiteSpace(status))
                result = result.Where(o => o.Status.Equals(status));

            if (tipoCTe != Dominio.Enumeradores.TipoCTE.Todos)
                result = result.Where(o => o.TipoCTE == tipoCTe);

            if (!string.IsNullOrWhiteSpace(cpfCnpjRemetente))
            {
                if (removerCliente)
                {
                    if (raizCNPJRemetente)
                        result = result.Where(o => !o.Remetente.CPF_CNPJ.Contains(cpfCnpjRemetente.Substring(0, 8)));
                    else
                        result = result.Where(o => o.Remetente.CPF_CNPJ != cpfCnpjRemetente);
                }
                else
                {
                    if (raizCNPJRemetente)
                        result = result.Where(o => o.Remetente.CPF_CNPJ.Contains(cpfCnpjRemetente.Substring(0, 8)));
                    else
                        result = result.Where(o => o.Remetente.CPF_CNPJ == cpfCnpjRemetente);
                }
            }

            if (importacao)
                result = result.Where(o => o.Remetente.Exterior == true);

            if (!string.IsNullOrWhiteSpace(cpfCnpjDestinatario))
            {
                if (removerCliente)
                {
                    if (raizCNPJDestinatario)
                        result = result.Where(o => !o.Destinatario.CPF_CNPJ.Contains(cpfCnpjDestinatario.Substring(0, 8)));
                    else
                        result = result.Where(o => o.Destinatario.CPF_CNPJ != cpfCnpjDestinatario);
                }
                else
                {
                    if (raizCNPJDestinatario)
                        result = result.Where(o => o.Destinatario.CPF_CNPJ.Contains(cpfCnpjDestinatario.Substring(0, 8)));
                    else
                        result = result.Where(o => o.Destinatario.CPF_CNPJ == cpfCnpjDestinatario);
                }
            }

            if (exportacao)
                result = result.Where(o => o.Destinatario.Exterior == true);

            if (!string.IsNullOrWhiteSpace(cpfCnpjExpedidor))
            {
                if (removerCliente)
                {
                    if (raizCNPJExpedidor)
                        result = result.Where(o => !o.Expedidor.CPF_CNPJ.Contains(cpfCnpjExpedidor.Substring(0, 8)));
                    else
                        result = result.Where(o => o.Expedidor.CPF_CNPJ != cpfCnpjExpedidor);
                }
                else
                {
                    if (raizCNPJExpedidor)
                        result = result.Where(o => o.Expedidor.CPF_CNPJ.Contains(cpfCnpjExpedidor.Substring(0, 8)));
                    else
                        result = result.Where(o => o.Expedidor.CPF_CNPJ == cpfCnpjExpedidor);
                }
            }

            if (!string.IsNullOrWhiteSpace(cpfCnpjRecebedor))
            {
                if (removerCliente)
                {
                    if (raizCNPJRecebedor)
                        result = result.Where(o => !o.Recebedor.CPF_CNPJ.Contains(cpfCnpjRecebedor.Substring(0, 8)));
                    else
                        result = result.Where(o => o.Recebedor.CPF_CNPJ != cpfCnpjRecebedor);
                }
                else
                {
                    if (raizCNPJRecebedor)
                        result = result.Where(o => o.Recebedor.CPF_CNPJ.Contains(cpfCnpjRecebedor.Substring(0, 8)));
                    else
                        result = result.Where(o => o.Recebedor.CPF_CNPJ == cpfCnpjRecebedor);
                }
            }

            if (codigoLocalidadeInicioPrestacao > 0)
                result = result.Where(o => o.LocalidadeInicioPrestacao.Codigo == codigoLocalidadeInicioPrestacao);

            if (codigoLocalidadeTerminoPrestacao > 0)
                result = result.Where(o => o.LocalidadeTerminoPrestacao.Codigo == codigoLocalidadeTerminoPrestacao);

            if (!string.IsNullOrWhiteSpace(cpfCnpjTomador))
            {
                //if (raizCNPJTomador)
                //    result = result.Where(o => (o.TipoTomador == Dominio.Enumeradores.TipoTomador.Destinatario && o.Destinatario.CPF_CNPJ.Contains(cpfCnpjTomador.Substring(0, 8))) ||
                //                           (o.TipoTomador == Dominio.Enumeradores.TipoTomador.Expedidor && o.Expedidor.CPF_CNPJ.Contains(cpfCnpjTomador.Substring(0, 8))) ||
                //                           (o.TipoTomador == Dominio.Enumeradores.TipoTomador.Outros && o.OutrosTomador.CPF_CNPJ.Contains(cpfCnpjTomador.Substring(0, 8))) ||
                //                           (o.TipoTomador == Dominio.Enumeradores.TipoTomador.Recebedor && o.Recebedor.CPF_CNPJ.Contains(cpfCnpjTomador.Substring(0, 8))) ||
                //                           (o.TipoTomador == Dominio.Enumeradores.TipoTomador.Remetente && o.Remetente.CPF_CNPJ.Contains(cpfCnpjTomador.Substring(0, 8))));
                //else
                //    result = result.Where(o => (o.TipoTomador == Dominio.Enumeradores.TipoTomador.Destinatario && o.Destinatario.CPF_CNPJ == cpfCnpjTomador) ||
                //                               (o.TipoTomador == Dominio.Enumeradores.TipoTomador.Expedidor && o.Expedidor.CPF_CNPJ == cpfCnpjTomador) ||
                //                               (o.TipoTomador == Dominio.Enumeradores.TipoTomador.Outros && o.OutrosTomador.CPF_CNPJ == cpfCnpjTomador) ||
                //                               (o.TipoTomador == Dominio.Enumeradores.TipoTomador.Recebedor && o.Recebedor.CPF_CNPJ == cpfCnpjTomador) ||
                //                               (o.TipoTomador == Dominio.Enumeradores.TipoTomador.Remetente && o.Remetente.CPF_CNPJ == cpfCnpjTomador));
                if (removerCliente)
                {
                    if (raizCNPJTomador)
                        result = result.Where(o => !o.TomadorPagador.CPF_CNPJ.Contains(cpfCnpjTomador.Substring(0, 8)));
                    else
                        result = result.Where(o => o.TomadorPagador.CPF_CNPJ != cpfCnpjTomador);
                }
                else
                {
                    if (raizCNPJTomador)
                        result = result.Where(o => o.TomadorPagador.CPF_CNPJ.Contains(cpfCnpjTomador.Substring(0, 8)));
                    else
                        result = result.Where(o => o.TomadorPagador.CPF_CNPJ == cpfCnpjTomador);
                }
            }

            if (numeroInicial > 0)
                result = result.Where(o => o.Numero >= numeroInicial);

            if (numeroFinal > 0)
                result = result.Where(o => o.Numero <= numeroFinal);

            if (codigoSerie > 0)
                result = result.Where(o => o.Serie.Codigo == codigoSerie);
            else if (series.Count() > 0)
                result = result.Where(o => series.Contains(o.Serie.Codigo));

            if (!string.IsNullOrWhiteSpace(cpfMotorista) || !string.IsNullOrWhiteSpace(nomeMotorista))
            {
                var queryMotorista = this.SessionNHiBernate.Query<Dominio.Entidades.MotoristaCTE>();

                if (!string.IsNullOrWhiteSpace(cpfMotorista))
                    result = result.Where(o => (from obj in queryMotorista where obj.CPFMotorista.Equals(cpfMotorista) select obj.CTE.Codigo).Contains(o.Codigo));
                else
                    result = result.Where(o => (from obj in queryMotorista where obj.NomeMotoristaCTe.Contains(nomeMotorista) select obj.CTE.Codigo).Contains(o.Codigo));
            }

            if (!string.IsNullOrWhiteSpace(placaVeiculo))
            {
                var queryVeiculos = this.SessionNHiBernate.Query<Dominio.Entidades.VeiculoCTE>();

                result = result.Where(o => (from obj in queryVeiculos where obj.CTE.Codigo == o.Codigo && obj.Placa.Equals(placaVeiculo) select obj.CTE.Codigo).Contains(o.Codigo));
            }

            if (!string.IsNullOrWhiteSpace(numeroNotaFiscal))
            {
                var queryDocumentos = this.SessionNHiBernate.Query<Dominio.Entidades.DocumentosCTE>();

                result = result.Where(o => (from obj in queryDocumentos where obj.CTE.Codigo == o.Codigo && obj.Numero.Contains(numeroNotaFiscal) select obj.CTE.Codigo).Contains(o.Codigo));
            }

            if (!string.IsNullOrWhiteSpace(tipoOcorrencia))
            {
                var queryOcorrencias = this.SessionNHiBernate.Query<Dominio.Entidades.OcorrenciaDeCTe>();

                if (tipoOcorrencia == "F")
                    result = result.Where(o => (from obj in queryOcorrencias where obj.CTe.Codigo == o.Codigo && obj.Ocorrencia.Tipo.Equals("F") select obj.CTe.Codigo).Contains(o.Codigo));
                else
                    result = result.Where(o => !(from obj in queryOcorrencias where obj.CTe.Codigo == o.Codigo && obj.Ocorrencia.Tipo.Equals("F") select obj.CTe.Codigo).Contains(o.Codigo));
            }

            if (statusPagamento.HasValue)
            {
                var queryDuplicatas = this.SessionNHiBernate.Query<Dominio.Entidades.ParcelaCobrancaCTe>();

                result = result.Where(o => (from obj in queryDuplicatas select obj.Cobranca.CTe.Codigo).Contains(o.Codigo));

                if (statusPagamento.Value == Dominio.Enumeradores.StatusDuplicata.Paga)
                    result = result.Where(o => !(from obj in queryDuplicatas where obj.Status == Dominio.Enumeradores.StatusDuplicata.Pendente select obj.Cobranca.CTe.Codigo).Contains(o.Codigo));
                else
                    result = result.Where(o => (from obj in queryDuplicatas where obj.Status == Dominio.Enumeradores.StatusDuplicata.Pendente select obj.Cobranca.CTe.Codigo).Contains(o.Codigo));
            }

            if (!string.IsNullOrWhiteSpace(duplicata))
            {
                var queryDuplicata = this.SessionNHiBernate.Query<Dominio.Entidades.DuplicataCtes>();

                result = result.Where(o => (from obj in queryDuplicata where obj.Duplicata.Numero == int.Parse(duplicata) select obj.ConhecimentoDeTransporteEletronico.Codigo).Contains(o.Codigo));
            }

            if (icmsCTe == "0")
                result = result.Where(o => o.ValorICMS == 0);
            else
            if (icmsCTe == "1")
                result = result.Where(o => o.ValorICMS > 0);

            if (cstCTe != "0")
                result = result.Where(o => o.CST.Equals(cstCTe));

            if (codigoUsuario > 0 && !string.IsNullOrWhiteSpace(nomeUsuario))
                result = result.Where(o => o.Usuario.Codigo == codigoUsuario); // || o.Log.Contains(nomeUsuario)

            if (!string.IsNullOrWhiteSpace(ufInicio))
                result = result.Where(o => o.LocalidadeInicioPrestacao.Estado.Sigla == ufInicio);

            if (!string.IsNullOrWhiteSpace(ufFim))
                result = result.Where(o => o.LocalidadeTerminoPrestacao.Estado.Sigla == ufFim);

            if (!string.IsNullOrWhiteSpace(observacao))
                result = result.Where(o => o.ObservacoesGerais.Contains(observacao));

            if (tipoServico != null)
                result = result.Where(o => o.TipoServico == tipoServico);

            if (numeroCarga > 0 || numeroUnidade > 0)
            {
                var queryIntegracaoCTe = this.SessionNHiBernate.Query<Dominio.Entidades.IntegracaoCTe>();
                var resultIntegracaoCTe = from o in queryIntegracaoCTe select o;
                if (numeroCarga > 0)
                    resultIntegracaoCTe = resultIntegracaoCTe.Where(o => o.NumeroDaCarga == numeroCarga);
                if (numeroUnidade > 0)
                    resultIntegracaoCTe = resultIntegracaoCTe.Where(o => o.NumeroDaUnidade == numeroUnidade);

                result = result.Where(o => resultIntegracaoCTe.Select(c => c.CTe.Codigo).Contains(o.Codigo));
            }

            return result.Select(o => o.Chave).Timeout(120).ToList();
        }

        public IList<Dominio.ObjetosDeValor.Relatorios.RelatorioCTeAgrupado> RelatorioCTesAgrupados(int codigoEmpresa, int codigoEmpresaEmissora, DateTime dataInicial, DateTime dataFinal, DateTime dataAutorizacaoInicial, DateTime dataAutorizacaoFinal, Dominio.Enumeradores.EmissaoPor? emissaoPor, int serie = 0, int tipoEmissao = 0)
        {
            var criteria = this.SessionNHiBernate.CreateCriteria<Dominio.Entidades.ConhecimentoDeTransporteEletronico>();
            criteria.CreateAlias("Empresa", "empresa");
            criteria.CreateAlias("ModeloDocumentoFiscal", "modelo");
            criteria.CreateAlias("Serie", "serie");
            if (emissaoPor.HasValue)
                criteria.CreateAlias("Usuario", "usuario");

            criteria.Add(Restrictions.Eq("empresa.EmpresaPai.Codigo", codigoEmpresa));
            criteria.Add(Restrictions.Eq("TipoAmbiente", Dominio.Enumeradores.TipoAmbiente.Producao));
            criteria.Add(Restrictions.Or(Restrictions.Eq("modelo.Numero", "57"), Restrictions.Eq("modelo.Numero", "67")));
            criteria.Add(Restrictions.In("Status", new string[] { "A", "C", "I" }));
            if (codigoEmpresaEmissora > 0)
                criteria.Add(Restrictions.Eq("empresa.Codigo", codigoEmpresaEmissora));

            if (dataInicial != DateTime.MinValue)
                criteria.Add(Restrictions.Ge("DataEmissao", dataInicial));

            if (dataFinal != DateTime.MinValue)
                criteria.Add(Restrictions.Lt("DataEmissao", dataFinal.AddDays(1)));

            if (dataAutorizacaoInicial != DateTime.MinValue)
                criteria.Add(Restrictions.Ge("DataRetornoSefaz", dataAutorizacaoInicial));

            if (dataAutorizacaoFinal != DateTime.MinValue)
                criteria.Add(Restrictions.Lt("DataRetornoSefaz", dataAutorizacaoFinal.AddDays(1)));

            if (serie > 0)
                criteria.Add(Restrictions.Eq("serie.Numero", serie));

            if (tipoEmissao == 1) //Integrados
            {
                var subCriteriaIntegracao = DetachedCriteria.ForEntityName("IntegracaoCTe");

                subCriteriaIntegracao.SetProjection(Projections.Property("CTe.Codigo"));

                criteria.Add(Subqueries.PropertyIn("Codigo", subCriteriaIntegracao));
            }
            if (tipoEmissao == 2) //Manuais
            {
                var subCriteriaIntegracao = DetachedCriteria.ForEntityName("IntegracaoCTe");

                subCriteriaIntegracao.SetProjection(Projections.Property("CTe.Codigo"));

                criteria.Add(Subqueries.PropertyNotIn("Codigo", subCriteriaIntegracao));
            }

            if (emissaoPor.HasValue && emissaoPor.Value == Dominio.Enumeradores.EmissaoPor.CallCenter)
            {
                criteria.Add(Restrictions.Eq("usuario.TipoAcesso", Dominio.Enumeradores.TipoAcesso.Admin));
            }
            else if (emissaoPor.HasValue && emissaoPor.Value == Dominio.Enumeradores.EmissaoPor.Web)
            {
                criteria.Add(Restrictions.IsNotNull("Usuario"));
                criteria.Add(Restrictions.Not(Restrictions.Eq("usuario.TipoAcesso", Dominio.Enumeradores.TipoAcesso.Admin)));
            }

            criteria.SetProjection(Projections.ProjectionList()
                .Add(Projections.GroupProperty("empresa.RazaoSocial"), "Empresa")
                .Add(Projections.GroupProperty("empresa.Codigo"), "CodigoEmpresa")
                .Add(Projections.GroupProperty("empresa.CNPJ"), "CNPJEmpresa")
                .Add(Projections.GroupProperty("Status"), "Status")
                .Add(Projections.RowCount(), "CountCTes"));

            criteria.SetResultTransformer(NHibernate.Transform.Transformers.AliasToBean(typeof(Dominio.ObjetosDeValor.Relatorios.RelatorioCTeAgrupado)));

            return criteria.List<Dominio.ObjetosDeValor.Relatorios.RelatorioCTeAgrupado>();
        }

        public List<Dominio.ObjetosDeValor.Relatorios.RelatorioCTeAgrupadoGrafico> GraficosCTesAgrupados(int codigoEmpresa, int codigoEmpresaEmissora, DateTime dataInicial, DateTime dataFinal, DateTime dataAutorizacaoInicial, DateTime dataAutorizacaoFinal, Dominio.Enumeradores.EmissaoPor? emissaoPor, int serie = 0, int tipoEmissao = 0)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.ConhecimentoDeTransporteEletronico>();

            string[] status = new string[] { "A", "C", "I" };

            var result = from obj in query
                         where
                         obj.Empresa.EmpresaPai.Codigo == codigoEmpresa
#if DEBUG
                         && obj.TipoAmbiente == Dominio.Enumeradores.TipoAmbiente.Homologacao
#else
                         && obj.TipoAmbiente == Dominio.Enumeradores.TipoAmbiente.Producao
#endif
                         && (obj.ModeloDocumentoFiscal.Numero.Equals("57") || obj.ModeloDocumentoFiscal.Numero.Equals("67"))
                         && obj.DataEmissao.HasValue
                         && status.Contains(obj.Status)
                         select obj;



            if (codigoEmpresaEmissora > 0)
                result = result.Where(o => o.Empresa.Codigo == codigoEmpresaEmissora);

            if (dataInicial != DateTime.MinValue)
                result = result.Where(o => o.DataEmissao >= dataInicial);

            if (dataFinal != DateTime.MinValue)
                result = result.Where(o => o.DataEmissao < dataFinal.AddDays(1));

            if (dataAutorizacaoInicial != DateTime.MinValue)
                result = result.Where(o => o.DataRetornoSefaz >= dataAutorizacaoInicial);


            if (dataAutorizacaoFinal != DateTime.MinValue)
                result = result.Where(o => o.DataEmissao < dataAutorizacaoFinal.AddDays(1));

            if (serie > 0)
                result = result.Where(o => o.Serie.Numero == serie);

            if (tipoEmissao != 0)
            {
                var queryIntegracaoCTe = this.SessionNHiBernate.Query<Dominio.Entidades.IntegracaoCTe>();
                var resultIntegracaoCTe = from obj in queryIntegracaoCTe select obj.CTe;

                if (tipoEmissao == 1) //Integrados
                    result = result.Where(o => resultIntegracaoCTe.Contains(o));
                if (tipoEmissao == 2) //Manuais
                    result = result.Where(o => !resultIntegracaoCTe.Contains(o));
            }

            if (emissaoPor.HasValue && emissaoPor.Value == Dominio.Enumeradores.EmissaoPor.CallCenter)
                result = result.Where(o => o.Usuario.TipoAcesso == Dominio.Enumeradores.TipoAcesso.Admin);
            else if (emissaoPor.HasValue && emissaoPor.Value == Dominio.Enumeradores.EmissaoPor.Web)
                result = result.Where(o => o.Usuario != null && o.Usuario.TipoAcesso != Dominio.Enumeradores.TipoAcesso.Admin);


            return result
                .GroupBy(o => o.DataEmissao.Value.Date)
                .Select(o => new Dominio.ObjetosDeValor.Relatorios.RelatorioCTeAgrupadoGrafico
                {
                    DataEmissao = o.Key,
                    Quantidade = o.Count()
                })
                .ToList();
        }

        public List<Dominio.ObjetosDeValor.Relatorios.RelatorioCTesCancelados> RelatorioCTesCancelados(int codigoEmpresa, int codigoEmpresaEmissora, DateTime dataInicial, DateTime dataFinal, int serie = 0)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.ConhecimentoDeTransporteEletronico>();
            var result = from obj in query
                         where obj.Status.Equals("C") && obj.Empresa.EmpresaPai.Codigo == codigoEmpresa && obj.TipoAmbiente == Dominio.Enumeradores.TipoAmbiente.Producao && (obj.ModeloDocumentoFiscal.Numero.Equals("57") || obj.ModeloDocumentoFiscal.Numero.Equals("67"))
                         select new Dominio.ObjetosDeValor.Relatorios.RelatorioCTesCancelados()
                         {
                             CodigoCTe = obj.Codigo,
                             CodigoEmpresa = obj.Empresa.Codigo,
                             DataEmissao = obj.DataEmissao,
                             HistoricoDeAlteracoes = obj.Log,
                             Justificativa = obj.ObservacaoCancelamento,
                             NomeEmpresa = obj.Empresa.RazaoSocial,
                             NumeroCTe = obj.Numero,
                             Serie = obj.Serie.Numero,
                             Cobrar = obj.CobrarCancelamento ? "Sim" : "Não"
                         };
            if (dataInicial != DateTime.MinValue)
                result = result.Where(o => o.DataEmissao >= dataInicial.Date);
            if (dataFinal != DateTime.MinValue)
                result = result.Where(o => o.DataEmissao < dataFinal.AddDays(1).Date);
            if (codigoEmpresaEmissora > 0)
                result = result.Where(o => o.CodigoEmpresa == codigoEmpresaEmissora);
            if (serie > 0)
                result = result.Where(o => o.Serie == serie);

            return result.ToList();
        }

        public IList<Dominio.ObjetosDeValor.Relatorios.RelatorioCTeAgrupado> RelatorioCTesAgrupadosEmpresasPai(int codigoEmpresaAdministradora, int codigoEmpresaPai, DateTime dataInicial, DateTime dataFinal, DateTime dataAutorizacaoInicial, DateTime dataAutorizacaoFinal)
        {
            var criteria = this.SessionNHiBernate.CreateCriteria<Dominio.Entidades.ConhecimentoDeTransporteEletronico>();
            criteria.CreateAlias("Empresa", "empresa");
            criteria.CreateAlias("empresa.EmpresaPai", "empresaPai");
            criteria.CreateAlias("ModeloDocumentoFiscal", "modelo");

            criteria.Add(Restrictions.Eq("empresaPai.EmpresaAdministradora.Codigo", codigoEmpresaAdministradora));
            criteria.Add(Restrictions.Eq("TipoAmbiente", Dominio.Enumeradores.TipoAmbiente.Producao));
            criteria.Add(Restrictions.In("Status", new string[] { "A", "C", "I" }));
            criteria.Add(Restrictions.Eq("modelo.Numero", "57"));

            if (codigoEmpresaPai > 0)
                criteria.Add(Restrictions.Eq("empresaPai.Codigo", codigoEmpresaPai));

            if (dataInicial != DateTime.MinValue)
                criteria.Add(Restrictions.Ge("DataEmissao", dataInicial));

            if (dataFinal != DateTime.MinValue)
                criteria.Add(Restrictions.Lt("DataEmissao", dataFinal.AddDays(1)));

            if (dataAutorizacaoInicial != DateTime.MinValue)
                criteria.Add(Restrictions.Ge("DataRetornoSefaz", dataAutorizacaoInicial));

            if (dataAutorizacaoFinal != DateTime.MinValue)
                criteria.Add(Restrictions.Lt("DataRetornoSefaz", dataAutorizacaoFinal.AddDays(1)));

            criteria.SetProjection(Projections.ProjectionList()
                .Add(Projections.GroupProperty("empresa.RazaoSocial"), "Empresa")
                .Add(Projections.GroupProperty("empresa.Codigo"), "CodigoEmpresa")
                .Add(Projections.GroupProperty("empresa.CNPJ"), "CNPJEmpresa")
                .Add(Projections.GroupProperty("empresaPai.RazaoSocial"), "EmpresaPai")
                .Add(Projections.GroupProperty("empresaPai.Codigo"), "CodigoEmpresaPai")
                .Add(Projections.GroupProperty("empresaPai.CNPJ"), "CNPJEmpresaPai")
                .Add(Projections.GroupProperty("Status"), "Status")
                .Add(Projections.RowCount(), "CountCTes"));

            criteria.SetResultTransformer(NHibernate.Transform.Transformers.AliasToBean(typeof(Dominio.ObjetosDeValor.Relatorios.RelatorioCTeAgrupado)));

            return criteria.List<Dominio.ObjetosDeValor.Relatorios.RelatorioCTeAgrupado>();
        }

        public List<Dominio.Entidades.ConhecimentoDeTransporteEletronico> BuscarPorViagem(int codigoViagem, int codigoPortoOrigem, int codigoTerminalDestino, int codigoPortoDestino)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.ConhecimentoDeTransporteEletronico>();
            query = query.Where(obj => obj.TipoCTE != Dominio.Enumeradores.TipoCTE.Complemento && obj.Viagem.Codigo == codigoViagem && obj.PortoOrigem.Codigo == codigoPortoOrigem && obj.TerminalDestino.Codigo == codigoTerminalDestino && obj.Status == "A" && obj.TipoModal == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoModal.Aquaviario);
            if (codigoPortoDestino > 0)
                query = query.Where(obj => obj.PortoDestino.Codigo == codigoPortoDestino);
            return query.ToList();
        }

        public Dominio.Entidades.ConhecimentoDeTransporteEletronico BuscarPorViagemBKControle(int codigoViagem, string booking, string numeroControle)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.ConhecimentoDeTransporteEletronico>();
            query = query.Where(obj => obj.Viagem.Codigo == codigoViagem && obj.NumeroBooking == booking && obj.NumeroControle == numeroControle);
            return query.FirstOrDefault();
        }

        public List<int> BuscarCodigosPorStatus(string status)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.ConhecimentoDeTransporteEletronico>();
            var result = from obj in query where obj.Status.Equals(status) && (obj.ModeloDocumentoFiscal.Numero.Equals("57") || obj.ModeloDocumentoFiscal.Numero.Equals("67")) select obj;
            return result.OrderBy(o => o.Codigo).Select(o => o.Codigo).ToList();
        }

        public List<Dominio.Entidades.ConhecimentoDeTransporteEletronico> BuscarTodosPorStatus(string status)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.ConhecimentoDeTransporteEletronico>();
            var result = from obj in query where obj.Status.Equals(status) && (obj.ModeloDocumentoFiscal.Numero.Equals("57") || obj.ModeloDocumentoFiscal.Numero.Equals("67")) select obj;
            return result.ToList();
        }

        public List<Dominio.Entidades.ConhecimentoDeTransporteEletronico> BuscarCTesImportadosSemDadosMultimodal()
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.ConhecimentoDeTransporteEletronico>();
            var result = from obj in query where obj.CTeImportadoEmbarcador == true && obj.Viagem == null select obj;
            return result.ToList();
        }

        public List<Dominio.Entidades.ConhecimentoDeTransporteEletronico> BuscarCTesEmitidosManualmenteSemXML()
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCTe>();
            var queryXML = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedidoXMLNotaFiscalCTe>();

            query = query.Where(obj => obj.CTe.GeradoManualmente == true);
            query = query.Where(obj => !queryXML.Any(c => c.CargaCTe == obj));

            query = query.Where(obj => obj.CTe.Chave == "26220302427026001541570010001782781017827894" ||
 obj.CTe.Chave == "23220302427026001894570010001241271012412799" ||
 obj.CTe.Chave == "23220102427026001894570010001212251012122596" ||
 obj.CTe.Chave == "23220102427026001894570010001212261012122690" ||
 obj.CTe.Chave == "26220102427026001541570010001729491017294995" ||
 obj.CTe.Chave == "26220402427026001541570010001797631017976396" ||
 obj.CTe.Chave == "26220502427026001541570010001837061018370690" ||
 obj.CTe.Chave == "26220502427026001541570010001813561018135691" ||
 obj.CTe.Chave == "26220502427026001541570010001813571018135796" ||
 obj.CTe.Chave == "26220502427026001541570010001837141018371493" ||
 obj.CTe.Chave == "26220502427026001541570010001837191018371996" ||
 obj.CTe.Chave == "42220502427026000901570010002596811025968190" ||
 obj.CTe.Chave == "42220502427026000901570010002596821025968294" ||
 obj.CTe.Chave == "26220502427026001541570010001836881018368890" ||
 obj.CTe.Chave == "26220502427026001541570010001837031018370396" ||
 obj.CTe.Chave == "13211102427026000812570010006896361068963690" ||
 obj.CTe.Chave == "13211202427026000812570010006909621069096291" ||
 obj.CTe.Chave == "42220502427026000901570010002566841025668496" ||
 obj.CTe.Chave == "35220602427026001207570010002650551026505590" ||
 obj.CTe.Chave == "43220602427026001703570010000936561009365694" ||
 obj.CTe.Chave == "26200502427026001541570040000503611005036190" ||
 obj.CTe.Chave == "26200502427026001541570040000504431005044395" ||
 obj.CTe.Chave == "23200502427026001894570040000405211004052190" ||
 obj.CTe.Chave == "26200602427026001541570040000510831005108395" ||
 obj.CTe.Chave == "35200702427026001207570040001016441010164492");

            return query.Select(c => c.CTe).ToList();
        }

        public decimal BuscarPesoBrutoContainer(int codigoContainer, int codigoCTe)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.CTe.CTeContainerDocumento>();
            query = query.Where(o => o.ContainerCTE.Codigo == codigoContainer);

            var queryXMLNota = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedidoXMLNotaFiscalCTe>();
            queryXMLNota = queryXMLNota.Where(x => x.CargaCTe != null && x.CargaCTe.CTe != null && x.CargaCTe.CTe.Codigo == codigoCTe && x.PedidoXMLNotaFiscal.XMLNotaFiscal.nfAtiva == true && query.Any(c => c.Chave == x.PedidoXMLNotaFiscal.XMLNotaFiscal.Chave || c.Numero == x.PedidoXMLNotaFiscal.XMLNotaFiscal.Numero.ToString()));

            return queryXMLNota.Count() > 0 ? queryXMLNota.Sum(x => (decimal)x.PedidoXMLNotaFiscal.XMLNotaFiscal.Peso) : 0m;
            //result.Sum(o => (decimal?)x.Peso);
        }

        public decimal BuscarPesoConhecimentos(List<int> codigoCTe)
        {
            var querycte = this.SessionNHiBernate.Query<Dominio.Entidades.InformacaoCargaCTE>();
            querycte = querycte.Where(x => codigoCTe.Contains(x.CTE.Codigo));

            return querycte.Timeout(7000).Count() > 0 ? querycte.Timeout(7000).Sum(x => (decimal)x.Quantidade) : 0m;
        }

        public decimal BuscarPesoNotasConhecimento(int codigoCTe)
        {
            var queryXMLNota = this.SessionNHiBernate.Query<Dominio.Entidades.DocumentosCTE>();
            queryXMLNota = queryXMLNota.Where(x => x.CTE.Codigo == codigoCTe);

            return queryXMLNota.Timeout(7000).Count() > 0 ? queryXMLNota.Timeout(7000).Sum(x => (decimal)x.Peso) : 0m;
            //result.Sum(o => (decimal?)x.Peso);
        }

        public decimal BuscarPesoNotasConhecimento(List<int> codigoCTe)
        {
            var queryXMLNota = this.SessionNHiBernate.Query<Dominio.Entidades.DocumentosCTE>();
            queryXMLNota = queryXMLNota.Where(x => codigoCTe.Contains(x.CTE.Codigo));

            return queryXMLNota.Timeout(7000).Count() > 0 ? queryXMLNota.Timeout(7000).Sum(x => (decimal)x.Peso) : 0m;
            //result.Sum(o => (decimal?)x.Peso);
        }

        public decimal BuscarPesoNotasConhecimento(int codigoContainer, List<int> codigoCTe)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.CTe.CTeContainerDocumento>();
            query = query.Where(o => o.ContainerCTE.Container.Codigo == codigoContainer);

            var queryXMLNota = this.SessionNHiBernate.Query<Dominio.Entidades.DocumentosCTE>();
            queryXMLNota = queryXMLNota.Where(x => codigoCTe.Contains(x.CTE.Codigo) && query.Any(c => c.Chave == x.ChaveNFE));

            return queryXMLNota.Timeout(7000).Count() > 0 ? queryXMLNota.Timeout(7000).Sum(x => (decimal)x.Peso) : 0m;
            //result.Sum(o => (decimal?)x.Peso);
        }

        public decimal BuscarPesoNotasConhecimento(int codigoContainer, int codigoCTe)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.CTe.CTeContainerDocumento>();
            query = query.Where(o => o.ContainerCTE.Codigo == codigoContainer);

            var queryXMLNota = this.SessionNHiBernate.Query<Dominio.Entidades.DocumentosCTE>();
            queryXMLNota = queryXMLNota.Where(x => x.CTE.Codigo == codigoCTe && query.Any(c => c.Chave == x.ChaveNFE));

            return queryXMLNota.Timeout(7000).Count() > 0 ? queryXMLNota.Timeout(7000).Sum(x => (decimal)x.Peso) : 0m;
            //result.Sum(o => (decimal?)x.Peso);
        }
        public string BuscarTaraContainer(int codigoContainer, int codigoCTe)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCTe>();
            query = query.Where(o => o.CTe.Codigo == codigoCTe);

            var queryContainer = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedido>();
            queryContainer = queryContainer.Where(x => x.Pedido != null && x.Pedido.Container != null && x.Pedido.Container.Codigo == codigoContainer);

            queryContainer = queryContainer.Where(c => query.Any(a => a.Carga == c.Carga));

            return queryContainer.Timeout(7000).Count() > 0 ? queryContainer.Timeout(7000).FirstOrDefault()?.Pedido?.TaraContainer : "";
        }

        public decimal BuscarPesoCubicoContainer(int codigoContainer)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.CTe.CTeContainerDocumento>();
            query = query.Where(o => o.ContainerCTE.Codigo == codigoContainer);

            var queryXMLNota = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal>();
            queryXMLNota = queryXMLNota.Where(x => x.nfAtiva == true && query.Any(c => c.Chave == x.Chave));

            return queryXMLNota.Count() > 0 ? queryXMLNota.Sum(x => x.MetrosCubicos) : 0m;
        }

        public List<Dominio.Entidades.ConhecimentoDeTransporteEletronico> BuscarTodosPorStatus(string[] status)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.ConhecimentoDeTransporteEletronico>();

            var result = from obj in query where status.Contains(obj.Status) && (obj.ModeloDocumentoFiscal.Numero.Equals("57") || obj.ModeloDocumentoFiscal.Numero.Equals("67")) select obj;

            return result.Timeout(120).ToList();
        }

        public List<Dominio.Entidades.ConhecimentoDeTransporteEletronico> BuscarTodosPorStatus(string status, int codigoEmpresa, string modelo = "")
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.ConhecimentoDeTransporteEletronico>();
            var result = from obj in query where obj.Status.Equals(status) && obj.Empresa.Codigo == codigoEmpresa && obj.TipoAmbiente == obj.Empresa.TipoAmbiente select obj;

            if (!string.IsNullOrWhiteSpace(modelo))
                result = result.Where(o => o.ModeloDocumentoFiscal.Numero.Equals(modelo));
            else
                result = result.Where(o => (o.ModeloDocumentoFiscal.Numero.Equals("57") || o.ModeloDocumentoFiscal.Numero.Equals("67")));

            return result.Timeout(180).ToList();
        }

        public List<Dominio.Entidades.ConhecimentoDeTransporteEletronico> BuscarTodosPorStatus(int codigoEmpresa, DateTime dataEmissaoInicial, DateTime dataEmissaoFinal, string[] status, Dominio.Enumeradores.TipoAmbiente tipoAmbiente, string modelo = "", double remetente = 0)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.ConhecimentoDeTransporteEletronico>();
            var result = from obj in query
                         where obj.Empresa.Codigo == codigoEmpresa &&
                               obj.DataEmissao >= dataEmissaoInicial &&
                               obj.DataEmissao < dataEmissaoFinal.AddDays(1).Date &&
                               status.Contains(obj.Status) &&
                               obj.TipoAmbiente == tipoAmbiente
                         select obj;

            if (!string.IsNullOrWhiteSpace(modelo))
                result = result.Where(o => o.ModeloDocumentoFiscal.Numero.Equals(modelo));
            else
                result = result.Where(o => (o.ModeloDocumentoFiscal.Numero.Equals("57") || o.ModeloDocumentoFiscal.Numero.Equals("67")));

            if (remetente > 0)
                result = result.Where(o => o.Remetente.CPF_CNPJ.Equals(remetente));

            return result.ToList();
        }

        public IList<Dominio.ObjetosDeValor.Relatorios.RelatorioEmissaoPorUsuario> RelatorioEmissaoPorUsuario(int codigoEmpresaPai, int codigoUsuario, DateTime dataInicial, DateTime dataFinal, bool agruparPorData)
        {
            string where = "CTe.CON_CANCELADO = 'N' ";

            if (dataInicial != DateTime.MinValue)
                where += "AND CTe.CON_DATA_AUTORIZACAO >= '" + dataInicial.ToString("yyyy-MM-dd HH:mm") + "' ";

            if (dataFinal != DateTime.MinValue)
                where += "AND CTe.CON_DATA_AUTORIZACAO <= '" + dataFinal.ToString("yyyy-MM-dd HH:mm") + "' ";

            if (codigoUsuario > 0)
                where += "AND Usuarios.FUN_CODIGO = " + codigoUsuario.ToString();

            where += "AND Usuarios.EMP_CODIGO = " + codigoEmpresaPai.ToString();

            string sql = @" SELECT 
	                            " + (agruparPorData ? "CONVERT(DATE, CTe.CON_DATA_AUTORIZACAO, 103) DataEmissao," : "") + @"
	                            COUNT(*) Quantidade, 
	                            Usuarios.FUN_NOME Nome
                            FROM 
	                            T_CTE CTe
                            JOIN
	                            T_FUNCIONARIO Usuarios ON CTe.CON_USUARIO = Usuarios.FUN_CODIGO
                            WHERE " + where + @"
                            GROUP BY 
                                " + (agruparPorData ? "CONVERT (DATE, CTe.CON_DATA_AUTORIZACAO, 103)," : "") + @"
	                            Usuarios.FUN_NOME ORDER BY FUN_NOME";

            var query = this.SessionNHiBernate.CreateSQLQuery(sql);

            query.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Dominio.ObjetosDeValor.Relatorios.RelatorioEmissaoPorUsuario)));

            return query.List<Dominio.ObjetosDeValor.Relatorios.RelatorioEmissaoPorUsuario>();
        }

        public decimal BuscarTotalICMS(int codigoEmpresa, DateTime dataEmissaoInicial, DateTime dataEmissaoFinal, string[] status, Dominio.Enumeradores.TipoAmbiente tipoAmbiente, string modelo = "")
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.ConhecimentoDeTransporteEletronico>();

            var result = from obj in query
                         where obj.Empresa.Codigo == codigoEmpresa &&
                               obj.DataEmissao >= dataEmissaoInicial &&
                               obj.DataEmissao < dataEmissaoFinal.AddDays(1).Date &&
                               status.Contains(obj.Status) &&
                               obj.TipoAmbiente == tipoAmbiente
                         select obj;

            if (!string.IsNullOrWhiteSpace(modelo))
                result = result.Where(o => o.ModeloDocumentoFiscal.Numero.Equals(modelo));
            else
                result = result.Where(o => (o.ModeloDocumentoFiscal.Numero.Equals("57") || o.ModeloDocumentoFiscal.Numero.Equals("67")));

            decimal? valor = result.Sum(o => (decimal?)o.ValorICMS);

            return valor.HasValue ? valor.Value : 0m;
        }

        public List<int> BuscarListaCodigos(int codigoEmpresa, DateTime dataInicial, DateTime dataFinal, int serie, int timeOutConsulta, string modelo = "", bool filtroPorDataEvento = false)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.XMLCTe>();
            var result = from obj in query where obj.CTe.Empresa.Codigo == codigoEmpresa && (obj.CTe.Status.Equals("A") || obj.CTe.Status.Equals("C") || obj.CTe.Status.Equals("I")) select obj.CTe;

            if (filtroPorDataEvento && (dataInicial.Date > DateTime.MinValue) && (dataFinal.Date > DateTime.MinValue))
            {
                result = result.Where(o => (o.Status == "A" && o.DataAutorizacao >= dataInicial.Date && o.DataAutorizacao < dataFinal.AddDays(1).Date) || ((o.Status == "C" || o.Status == "I") && o.DataCancelamento >= dataInicial.Date && o.DataCancelamento < dataFinal.AddDays(1).Date));
            }
            else
            {
                if (dataInicial.Date > DateTime.MinValue)
                    result = result.Where(o => o.DataEmissao >= dataInicial.Date);

                if (dataFinal.Date > DateTime.MinValue)
                    result = result.Where(o => o.DataEmissao < dataFinal.AddDays(1).Date);
            }

            //if ( !(dataInicial.Date > DateTime.MinValue) && !(dataFinal.Date > DateTime.MinValue))
            //    result = result.Where(o => o.DataEmissao >= DateTime.Today.AddDays(-5));

            if (serie > 0)
                result = result.Where(o => o.Serie.Numero == serie);

            if (!string.IsNullOrWhiteSpace(modelo))
                result = result.Where(o => o.ModeloDocumentoFiscal.Numero.Equals(modelo));
            else
                result = result.Where(o => (o.ModeloDocumentoFiscal.Numero.Equals("57") || o.ModeloDocumentoFiscal.Numero.Equals("67")));

            if (dataInicial.Date == DateTime.MinValue && dataFinal.Date == DateTime.MinValue)
            {
                var queryCTesPendentes = this.SessionNHiBernate.Query<Dominio.Entidades.RetornoXMLCTe>();
                //result = result.Where(o => !(from obj in queryCTesPendentes where obj.CTe.Empresa.Codigo == codigoEmpresa && obj.CTe.Status == obj.Status select obj.CTe.Codigo).Contains(o.Codigo));
                //result = result.Where(o => !(from obj in queryCTesPendentes where obj.CTe.Status == obj.Status && obj.CTe.Codigo == o.Codigo select obj.CTe.Codigo).Contains(o.Codigo));
                result = result.Where(o => !queryCTesPendentes.Any(obj => obj.CTe.Status == obj.Status && obj.CTe.Codigo == o.Codigo));
            }

            if (timeOutConsulta == 0)
                return result.Select(o => o.Codigo).Timeout(15).ToList();
            else
                return result.Select(o => o.Codigo).Timeout(timeOutConsulta).ToList();

        }

        public IList<Dominio.ObjetosDeValor.WebService.CTe.ProtocoloCTe> BuscarListaCodigosPorSQL(int codigoEmpresa, DateTime dataInicial, DateTime dataFinal, int serie, int timeOutConsulta, string modelo, bool filtroPorDataEvento, int diasRetroativos, int quantidadeProtocolosRetorno)
        {
            var parametros = new List<ParametroSQL>();

            var sql = @"SELECT TOP " + quantidadeProtocolosRetorno.ToString() + @" C.CON_CODIGO CodigoCTe
                        FROM T_CTE C
                        JOIN T_MODDOCFISCAL MO ON C.CON_MODELODOC = MO.MOD_CODIGO
                        JOIN T_CTE_XML CX ON C.CON_CODIGO = CX.CON_CODIGO
                        JOIN T_EMPRESA_SERIE S ON S.ESE_CODIGO = C.CON_SERIE
                        WHERE C.EMP_CODIGO = " + codigoEmpresa.ToString() +
                      @" AND CON_STATUS IN ('A','C','I') "; // SQL-INJECTION-SAFE


            if (filtroPorDataEvento && (dataInicial.Date > DateTime.MinValue) && (dataFinal.Date > DateTime.MinValue))
            {
                sql += @" AND ((C.CON_STATUS == 'A' AND C.CON_DATA_AUTORIZACAO between '" + dataInicial.ToString("yyyy-MM-dd 00:00:00") + "' AND  '" + dataFinal.ToString("yyyy-MM-dd 23:59:59") + "' )  " +
                       @"      OR ((C.CON_STATUS == 'C' OR C.CON_STATUS == 'I') and  AND C.CON_DATA_CANCELAMENTO between '" + dataInicial.ToString("yyyy-MM-dd 00:00:00") + "' AND  '" + dataFinal.ToString("yyyy-MM-dd 23:59:59") + "' )) ";
            }
            else
            {
                if (dataInicial.Date > DateTime.MinValue)
                    sql += @"  AND C.CON_DATAHORAEMISSAO >= '" + dataInicial.ToString("yyyy-MM-dd 00:00:00") + "' ";
                else
                {
                    if (serie == 0 && diasRetroativos > 0)
                    {
                        sql += @"  AND C.CON_DATAHORAEMISSAO >= '" + DateTime.Now.AddDays(diasRetroativos * -1).ToString("yyyy-MM-dd 00:00:00") + "' ";
                    }
                }

                if (dataFinal.Date > DateTime.MinValue)
                    sql += @"  AND C.CON_DATAHORAEMISSAO <= '" + dataFinal.ToString("yyyy-MM-dd 23:59:59") + "' ";
            }

            if (serie > 0)
                sql += @" AND S.ESE_NUMERO = " + serie.ToString();

            if (!string.IsNullOrWhiteSpace(modelo))
            {
                sql += @" AND MO.MOD_NUM = :MO_MOD_NUM";
                parametros.Add(new ParametroSQL("MO_MOD_NUM", modelo));
            }
            else
                sql += @" AND MO.MOD_NUM IN ('57','67') ";

            if (dataInicial.Date == DateTime.MinValue && dataFinal.Date == DateTime.MinValue)
                sql += @"AND NOT EXISTS (SELECT X.CON_CODIGO FROM T_RETORNO_XML_CTE X JOIN T_CTE CT ON X.CON_CODIGO = C.CON_CODIGO WHERE CT.CON_STATUS = X.RXC_STATUS and CT.CON_CODIGO=C.CON_CODIGO)";

            var consultaCodigosCTes = new SQLDinamico(sql, parametros).CriarSQLQuery(this.SessionNHiBernate);

            consultaCodigosCTes.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Dominio.ObjetosDeValor.WebService.CTe.ProtocoloCTe)));

            if (timeOutConsulta == 0)
                return consultaCodigosCTes.SetTimeout(15).List<Dominio.ObjetosDeValor.WebService.CTe.ProtocoloCTe>();
            else
                return consultaCodigosCTes.SetTimeout(timeOutConsulta).List<Dominio.ObjetosDeValor.WebService.CTe.ProtocoloCTe>();

        }

        public Dominio.ObjetosDeValor.ValoresCTe BuscarValores(int codigoEmpresa, DateTime dataInicial, DateTime dataFinal, Dominio.Enumeradores.TipoAmbiente tipoAmbiente, string modelo = "")
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.ConhecimentoDeTransporteEletronico>();
            var result = from obj in query
                         where
                             obj.Empresa.Codigo == codigoEmpresa &&
                             obj.DataEmissao >= dataInicial.Date &&
                             obj.DataEmissao <= dataFinal.Date &&
                             obj.TipoAmbiente == tipoAmbiente &&
                             obj.Status.Equals("A")
                         select obj;

            if (!string.IsNullOrWhiteSpace(modelo))
                result = result.Where(o => o.ModeloDocumentoFiscal.Numero.Equals(modelo));
            else
                result = result.Where(o => (o.ModeloDocumentoFiscal.Numero.Equals("57") || o.ModeloDocumentoFiscal.Numero.Equals("67")));

            return new Dominio.ObjetosDeValor.ValoresCTe()
            {
                MaiorValor = result.Max(o => (decimal?)o.ValorAReceber),
                MenorValor = result.Min(o => (decimal?)o.ValorAReceber),
                ValorMedio = result.Average(o => (decimal?)o.ValorAReceber)
            };
        }

        public List<Dominio.ObjetosDeValor.QuantidadesCTe> BuscarQuantidades(int codigoEmpresa, DateTime dataInicial, DateTime dataFinal, Dominio.Enumeradores.TipoAmbiente tipoAmbiente, string modelo = "57")
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.ConhecimentoDeTransporteEletronico>();
            var result = from obj in query
                         where
                             obj.Empresa.Codigo == codigoEmpresa &&
                             obj.DataEmissao >= dataInicial.Date &&
                             obj.DataEmissao <= dataFinal.Date &&
                             obj.ModeloDocumentoFiscal.Numero.Equals(modelo) &&
                             obj.TipoAmbiente == tipoAmbiente
                         group obj by obj.Status into contadores
                         select new Dominio.ObjetosDeValor.QuantidadesCTe()
                         {
                             Quantidade = contadores.Count(),
                             Status = contadores.Key
                         };

            return result.ToList();
        }

        public List<Dominio.Entidades.ConhecimentoDeTransporteEletronico> BuscarPorRemetente(int codigoEmpresa, string cpfCnpjRemetente, DateTime dataInicial, DateTime dataFinal, string[] status, Dominio.Enumeradores.TipoAmbiente tipoAmbiente, List<int> codigosCTes = null, int codigoVeiculo = 0, int[] series = null, string cpfCnpjDestinatario = "")
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.ConhecimentoDeTransporteEletronico>();

            var result = from obj in
                             query
                         where
                             obj.Empresa.Codigo == codigoEmpresa &&
                             obj.DataEmissao >= dataInicial.Date &&
                             obj.DataEmissao < dataFinal.Date.AddDays(1) &&
                             obj.TipoAmbiente == tipoAmbiente &&
                             status.Contains(obj.Status) &&
                             (obj.ModeloDocumentoFiscal.Numero.Equals("57") || obj.ModeloDocumentoFiscal.Numero.Equals("67") || obj.ModeloDocumentoFiscal.Numero.Equals("39"))
                         select obj;

            if (!string.IsNullOrWhiteSpace(cpfCnpjRemetente))
                result = result.Where(o => o.Remetente.CPF_CNPJ.Equals(cpfCnpjRemetente));

            if (!string.IsNullOrWhiteSpace(cpfCnpjDestinatario))
                result = result.Where(o => o.Destinatario.CPF_CNPJ.Equals(cpfCnpjDestinatario));

            if (codigosCTes != null && codigosCTes.Count() > 0)
                result = result.Where(o => codigosCTes.Contains(o.Codigo));

            if (series != null && series.Count() > 0)
                result = result.Where(o => series.Contains(o.Serie.Codigo));

            if (codigoVeiculo > 0)
            {
                var queryVeiculos = this.SessionNHiBernate.Query<Dominio.Entidades.VeiculoCTE>();

                result = result.Where(o => (from obj in queryVeiculos where obj.CTE.Codigo == o.Codigo && obj.Veiculo.Codigo == codigoVeiculo select obj.CTE.Codigo).Contains(o.Codigo));
            }

            return result.Fetch(o => o.Remetente)
                         .Fetch(o => o.Destinatario)
                         .Fetch(o => o.Empresa)
                         .Fetch(o => o.Serie)
                         .Fetch(o => o.CFOP)
                         .Fetch(o => o.NaturezaDaOperacao)
                         .Fetch(o => o.ModeloDocumentoFiscal)
                         .Timeout(2400)
                         .ToList();
        }

        public List<Dominio.Entidades.ConhecimentoDeTransporteEletronico> BuscarPorTomador(int codigoEmpresa, string cpfCnpjTomador, DateTime dataInicial, DateTime dataFinal, string[] status, Dominio.Enumeradores.TipoAmbiente tipoAmbiente, List<int> codigosCTes = null, int codigoVeiculo = 0, int[] series = null, string cpfCnpjRemetente = "", string cpfCnpjDestinatario = "")
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.ConhecimentoDeTransporteEletronico>();

            var result = from obj in
                             query
                         where
                             obj.Empresa.Codigo == codigoEmpresa &&
                             obj.DataEmissao >= dataInicial.Date &&
                             obj.DataEmissao < dataFinal.Date.AddDays(1) &&
                             obj.TipoAmbiente == tipoAmbiente &&
                             status.Contains(obj.Status) &&
                             (obj.ModeloDocumentoFiscal.Numero.Equals("57") || obj.ModeloDocumentoFiscal.Numero.Equals("67") || obj.ModeloDocumentoFiscal.Numero.Equals("39"))
                         select obj;

            if (!string.IsNullOrWhiteSpace(cpfCnpjTomador))
                result = result.Where(o => o.TomadorPagador.CPF_CNPJ.Equals(cpfCnpjTomador));

            if (!string.IsNullOrWhiteSpace(cpfCnpjRemetente))
                result = result.Where(o => o.Remetente.CPF_CNPJ.Equals(cpfCnpjRemetente));

            if (!string.IsNullOrWhiteSpace(cpfCnpjDestinatario))
                result = result.Where(o => o.Destinatario.CPF_CNPJ.Equals(cpfCnpjDestinatario));

            if (codigosCTes != null && codigosCTes.Count() > 0)
                result = result.Where(o => codigosCTes.Contains(o.Codigo));

            if (series != null && series.Count() > 0)
                result = result.Where(o => series.Contains(o.Serie.Codigo));

            if (codigoVeiculo > 0)
            {
                var queryVeiculos = this.SessionNHiBernate.Query<Dominio.Entidades.VeiculoCTE>();

                result = result.Where(o => (from obj in queryVeiculos where obj.CTE.Codigo == o.Codigo && obj.Veiculo.Codigo == codigoVeiculo select obj.CTE.Codigo).Contains(o.Codigo));
            }

            return result.Fetch(o => o.Remetente)
                         .Fetch(o => o.Destinatario)
                         .Fetch(o => o.Empresa)
                         .Fetch(o => o.Serie)
                         .Fetch(o => o.CFOP)
                         .Fetch(o => o.NaturezaDaOperacao)
                         .Fetch(o => o.ModeloDocumentoFiscal)
                         .Timeout(2400)
                         .ToList();
        }

        public List<Dominio.Entidades.ConhecimentoDeTransporteEletronico> BuscarPorRemetenteEBS(int codigoEmpresa, string cpfCnpjRemetente, DateTime dataInicial, DateTime dataFinal, string[] status, Dominio.Enumeradores.TipoAmbiente tipoAmbiente, List<int> codigosCTes = null, int codigoVeiculo = 0, int[] series = null)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.ConhecimentoDeTransporteEletronico>();

            var result = from obj in
                             query
                         where
                             obj.Empresa.Codigo == codigoEmpresa &&
                             obj.DataEmissao >= dataInicial.Date &&
                             obj.DataEmissao < dataFinal.Date.AddDays(1) &&
                             obj.TipoAmbiente == tipoAmbiente &&
                             status.Contains(obj.Status) &&
                             (obj.ModeloDocumentoFiscal.Numero.Equals("57") || obj.ModeloDocumentoFiscal.Numero.Equals("67"))
                         select obj;

            if (!string.IsNullOrWhiteSpace(cpfCnpjRemetente))
                result = result.Where(o => o.Remetente.CPF_CNPJ.Equals(cpfCnpjRemetente));

            if (codigosCTes != null && codigosCTes.Count() > 0)
                result = result.Where(o => codigosCTes.Contains(o.Codigo));

            if (series != null && series.Count() > 0)
                result = result.Where(o => series.Contains(o.Serie.Codigo));

            if (codigoVeiculo > 0)
            {
                var queryVeiculos = this.SessionNHiBernate.Query<Dominio.Entidades.VeiculoCTE>();

                result = result.Where(o => (from obj in queryVeiculos where obj.CTE.Codigo == o.Codigo && obj.Veiculo.Codigo == codigoVeiculo select obj.CTE.Codigo).Contains(o.Codigo));
            }

            return result.Fetch(o => o.Remetente)
                         .Fetch(o => o.Destinatario)
                         .Fetch(o => o.Empresa)
                         .Fetch(o => o.Serie)
                         .Fetch(o => o.CFOP)
                         .Fetch(o => o.NaturezaDaOperacao)
                         .Fetch(o => o.ModeloDocumentoFiscal)
                         .Timeout(2400)
                         .ToList();
        }

        public List<Dominio.Entidades.ConhecimentoDeTransporteEletronico> BuscarPorDuplicata(int codigoEmpresa, int codigoDuplicata)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.ConhecimentoDeTransporteEletronico>();

            var result = from obj in
                             query
                         where
                             obj.Empresa.Codigo == codigoEmpresa
                         select obj;

            if (codigoDuplicata > 0)
            {
                var queryDuplicata = this.SessionNHiBernate.Query<Dominio.Entidades.DuplicataCtes>();

                result = result.Where(o => (from obj in queryDuplicata where obj.Duplicata.Codigo == codigoDuplicata select obj.ConhecimentoDeTransporteEletronico.Codigo).Contains(o.Codigo)); //obj.ConhecimentoDeTransporteEletronico.Codigo == o.Codigo &&
            }

            return result.Fetch(o => o.Remetente)
                         .Fetch(o => o.Destinatario)
                         .Fetch(o => o.Empresa)
                         .Fetch(o => o.Serie)
                         .Fetch(o => o.CFOP)
                         .Fetch(o => o.NaturezaDaOperacao)
                         .Fetch(o => o.ModeloDocumentoFiscal)
                         .ToList();
        }

        public int ContarCTesPendentesDeEntrega(int codigoEmpresa, DateTime dataInicial, DateTime dataFinal, Dominio.Enumeradores.TipoAmbiente tipoAmbiente)
        {
            var criteria = this.SessionNHiBernate.CreateCriteria<Dominio.Entidades.ConhecimentoDeTransporteEletronico>();

            criteria.CreateAlias("ModeloDocumentoFiscal", "modelo");

            criteria.Add(Restrictions.Eq("Empresa.Codigo", codigoEmpresa));
            criteria.Add(Restrictions.Eq("TipoAmbiente", tipoAmbiente));
            criteria.Add(Restrictions.Ge("DataEmissao", dataInicial));
            criteria.Add(Restrictions.Lt("DataEmissao", dataFinal.AddDays(1)));
            criteria.Add(Restrictions.Eq("Status", "A"));
            criteria.Add(Restrictions.Eq("modelo.Numero", "57"));

            var subCriteriaOcorrencia = DetachedCriteria.ForEntityName("OcorrenciaDeCTe");
            subCriteriaOcorrencia.CreateAlias("Ocorrencia", "tipoOcorrencia");
            subCriteriaOcorrencia.CreateAlias("CTe", "cte");

            subCriteriaOcorrencia.Add(Restrictions.Eq("tipoOcorrencia.Tipo", "F"));
            subCriteriaOcorrencia.Add(Restrictions.Eq("cte.Empresa.Codigo", codigoEmpresa));
            subCriteriaOcorrencia.Add(Restrictions.Eq("cte.TipoAmbiente", tipoAmbiente));
            subCriteriaOcorrencia.Add(Restrictions.Ge("cte.DataEmissao", dataInicial));
            subCriteriaOcorrencia.Add(Restrictions.Lt("cte.DataEmissao", dataFinal.AddDays(1)));
            subCriteriaOcorrencia.Add(Restrictions.Eq("cte.Status", "A"));

            subCriteriaOcorrencia.SetProjection(Projections.Property("cte.Codigo"));

            criteria.Add(Subqueries.PropertyNotIn("Codigo", subCriteriaOcorrencia));

            criteria.SetProjection(Projections.RowCount());

            return criteria.UniqueResult<int>();
        }

        public List<Dominio.ObjetosDeValor.ConsultaCTeParaMDFe> ConsultarParaEmissaoDeMDFe(int codigoEmpresa, Dominio.Enumeradores.TipoAmbiente ambiente, int codigoMunicipioTerminoPrestacao, string ufCarregamento, string ufDescarregamento, int numeroInicial, int numeroFinal, DateTime dataInicial, DateTime dataFinal, string placaVeiculo, string nomeMotorista, string cpfMotorista, Dominio.Enumeradores.TipoServico? tipoServico, Dominio.Enumeradores.TipoCTE? tipoCTe, List<int> numerosSeries, int inicioRegistros, int maximoRegistros, int codigoCTe = 0)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.ConhecimentoDeTransporteEletronico>();

            var result = from obj in query where obj.Empresa.Codigo == codigoEmpresa && obj.TipoAmbiente == ambiente && obj.Status.Equals("A") && (obj.ModeloDocumentoFiscal.Numero.Equals("57") || obj.ModeloDocumentoFiscal.Numero.Equals("67")) select obj;

            if (codigoCTe > 0)
                result = result.Where(o => o.Codigo == codigoCTe);

            if (codigoMunicipioTerminoPrestacao > 0)
                result = result.Where(o => o.LocalidadeTerminoPrestacao.Codigo == codigoMunicipioTerminoPrestacao);

            if (numeroInicial > 0)
                result = result.Where(o => o.Numero >= numeroInicial);

            if (numeroFinal > 0)
                result = result.Where(o => o.Numero <= numeroFinal);

            if (dataInicial != DateTime.MinValue)
                result = result.Where(o => o.DataEmissao >= dataInicial.Date);

            if (dataFinal != DateTime.MinValue)
                result = result.Where(o => o.DataEmissao < dataFinal.AddDays(1).Date);

            if (!string.IsNullOrWhiteSpace(ufCarregamento))
                result = result.Where(o => o.LocalidadeInicioPrestacao.Estado.Sigla.Equals(ufCarregamento));

            if (!string.IsNullOrWhiteSpace(ufDescarregamento))
                result = result.Where(o => o.LocalidadeTerminoPrestacao.Estado.Sigla.Equals(ufDescarregamento));

            if (tipoServico.HasValue)
                result = result.Where(o => o.TipoCTE == tipoCTe.Value);

            if (!string.IsNullOrWhiteSpace(placaVeiculo))
            {
                var queryVeiculos = this.SessionNHiBernate.Query<Dominio.Entidades.VeiculoCTE>();

                result = result.Where(o => (from obj in queryVeiculos where obj.CTE.Codigo == o.Codigo && obj.Placa.Equals(placaVeiculo) select obj.CTE.Codigo).Contains(o.Codigo));
            }

            if (!string.IsNullOrWhiteSpace(nomeMotorista) || !string.IsNullOrWhiteSpace(cpfMotorista))
            {
                var queryMotorista = this.SessionNHiBernate.Query<Dominio.Entidades.MotoristaCTE>();

                if (!string.IsNullOrWhiteSpace(cpfMotorista))
                    result = result.Where(o => (from obj in queryMotorista where obj.CPFMotorista.Equals(cpfMotorista) select obj.CTE.Codigo).Contains(o.Codigo));
                else
                    result = result.Where(o => (from obj in queryMotorista where obj.NomeMotoristaCTe.Contains(nomeMotorista) select obj.CTE.Codigo).Contains(o.Codigo));
            }

            if (numerosSeries.Count > 0)
                result = result.Where(o => numerosSeries.Contains(o.Serie.Numero));

            return result.OrderByDescending(o => o.Numero).ThenBy(o => o.Serie.Numero).Skip(inicioRegistros).Take(maximoRegistros).Select(o => new Dominio.ObjetosDeValor.ConsultaCTeParaMDFe
            {
                Codigo = o.Codigo,
                DataEmissao = o.DataEmissao,
                LocalidadeInicioPrestacao = o.LocalidadeInicioPrestacao.Descricao,
                LocalidadeTerminoPrestacao = o.LocalidadeTerminoPrestacao.Descricao,
                Numero = o.Numero,
                PesoTotal = (from obj in this.SessionNHiBernate.Query<Dominio.Entidades.InformacaoCargaCTE>() where obj.CTE.Codigo == o.Codigo select obj).Sum(x => (decimal?)x.Quantidade) ?? 0m,
                Serie = o.Serie.Numero,
                UFCarregamento = o.LocalidadeInicioPrestacao.Estado.Sigla,
                UFDescarregamento = o.LocalidadeTerminoPrestacao.Estado.Sigla,
                UFInicioPrestacao = o.LocalidadeInicioPrestacao.Estado.Sigla,
                UFTerminoPrestacao = o.LocalidadeTerminoPrestacao.Estado.Sigla,
                ValorFrete = o.ValorFrete,
                ValorTotalMercadoria = o.ValorTotalMercadoria,
                PesoKgTotal = (from obj in this.SessionNHiBernate.Query<Dominio.Entidades.InformacaoCargaCTE>() where obj.CTE.Codigo == o.Codigo && obj.UnidadeMedida == "01" select obj).Sum(x => (decimal?)x.Quantidade) ?? 0m,
                Averbado = (from obj in this.SessionNHiBernate.Query<Dominio.Entidades.AverbacaoCTe>() where obj.CTe.Codigo == o.Codigo && obj.Status == Dominio.Enumeradores.StatusAverbacaoCTe.Sucesso select obj).Any() ? "Sim" : "Não",
            }).ToList();
        }

        public int ContarConsultaParaEmissaoDeMDFe(int codigoEmpresa, Dominio.Enumeradores.TipoAmbiente ambiente, int codigoMunicipioTerminoPrestacao, string ufCarregamento, string ufDescarregamento, int numeroInicial, int numeroFinal, DateTime dataInicial, DateTime dataFinal, string placaVeiculo, string nomeMotorista, string cpfMotorista, Dominio.Enumeradores.TipoServico? tipoServico, Dominio.Enumeradores.TipoCTE? tipoCTe, List<int> numerosSeries, int codigoCTe = 0)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.ConhecimentoDeTransporteEletronico>();

            var result = from obj in query where obj.Empresa.Codigo == codigoEmpresa && obj.TipoAmbiente == ambiente && obj.Status.Equals("A") && (obj.ModeloDocumentoFiscal.Numero.Equals("57") || obj.ModeloDocumentoFiscal.Numero.Equals("67")) select obj;

            if (codigoCTe > 0)
                result = result.Where(o => o.Codigo == codigoCTe);

            if (codigoMunicipioTerminoPrestacao > 0)
                result = result.Where(o => o.LocalidadeTerminoPrestacao.Codigo == codigoMunicipioTerminoPrestacao);

            if (numeroInicial > 0)
                result = result.Where(o => o.Numero >= numeroInicial);

            if (numeroFinal > 0)
                result = result.Where(o => o.Numero <= numeroFinal);

            if (dataInicial != DateTime.MinValue)
                result = result.Where(o => o.DataEmissao >= dataInicial.Date);

            if (dataFinal != DateTime.MinValue)
                result = result.Where(o => o.DataEmissao < dataFinal.AddDays(1).Date);

            if (!string.IsNullOrWhiteSpace(ufCarregamento))
                result = result.Where(o => o.LocalidadeInicioPrestacao.Estado.Sigla.Equals(ufCarregamento));

            if (!string.IsNullOrWhiteSpace(ufDescarregamento))
                result = result.Where(o => o.LocalidadeTerminoPrestacao.Estado.Sigla.Equals(ufDescarregamento));

            if (tipoServico.HasValue)
                result = result.Where(o => o.TipoCTE == tipoCTe.Value);

            if (!string.IsNullOrWhiteSpace(placaVeiculo))
            {
                var queryVeiculos = this.SessionNHiBernate.Query<Dominio.Entidades.VeiculoCTE>();

                result = result.Where(o => (from obj in queryVeiculos where obj.CTE.Codigo == o.Codigo && obj.Placa.Equals(placaVeiculo) select obj.CTE.Codigo).Contains(o.Codigo));
            }

            if (!string.IsNullOrWhiteSpace(nomeMotorista) || !string.IsNullOrWhiteSpace(cpfMotorista))
            {
                var queryMotorista = this.SessionNHiBernate.Query<Dominio.Entidades.MotoristaCTE>();

                if (!string.IsNullOrWhiteSpace(cpfMotorista))
                    result = result.Where(o => (from obj in queryMotorista where obj.CPFMotorista.Equals(cpfMotorista) select obj.CTE.Codigo).Contains(o.Codigo));
                else
                    result = result.Where(o => (from obj in queryMotorista where obj.NomeMotoristaCTe.Contains(nomeMotorista) select obj.CTE.Codigo).Contains(o.Codigo));
            }

            if (numerosSeries.Count > 0)
                result = result.Where(o => numerosSeries.Contains(o.Serie.Numero));

            return result.Count();
        }

        public List<int> BuscarCTesSemNumeroDeRecibo()
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.ConhecimentoDeTransporteEletronico>();

            var result = from obj in
                             query
                         where
                             (obj.Status.Equals("A") ||
                              obj.Status.Equals("C")) &&
                             (obj.NumeroRecibo == null ||
                              obj.NumeroRecibo == string.Empty)
                         select obj.Codigo;

            return result.ToList();
        }

        public List<int> BuscarCTesSemParticipantes()
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.ConhecimentoDeTransporteEletronico>();

            var result = from obj in query where obj.Remetente == null && obj.Destinatario == null && obj.Expedidor == null && obj.Recebedor == null && obj.OutrosTomador == null select obj.Codigo;

            return result.ToList();
        }

        public List<Dominio.ObjetosDeValor.Relatorios.DACTE> BuscarDACTE(int codigoEmpresa, int codigoCTe)
        {
            throw new NotImplementedException();
        }

        public int ContarRelatorioCTesEmitidosPorEmbarcador(int codigoEmpresaPai, int codigoEmpresa, string cpfCnpjEmbarcador, DateTime dataAutorizacaoInicial, DateTime dataAutorizacaoFinal, DateTime dataEmissaoInicial, DateTime dataEmissaoFinal, bool todosCNPJdaRaizEmbarcador = false)
        {
            string query = @"SELECT COUNT(0) as CONTADOR
                             FROM T_CTE cte
                             INNER JOIN T_EMPRESA_SERIE serie ON serie.ESE_CODIGO = cte.CON_SERIE
                             INNER JOIN T_EMPRESA empresa ON empresa.EMP_CODIGO = cte.EMP_CODIGO
                             INNER JOIN T_MODDOCFISCAL modelo ON modelo.MOD_CODIGO = cte.CON_MODELODOC
                             LEFT OUTER JOIN T_CTE_PARTICIPANTE remetente ON remetente.PCT_CODIGO = cte.CON_REMETENTE_CTE
                             LEFT OUTER JOIN T_CTE_PARTICIPANTE destinatario ON destinatario.PCT_CODIGO = cte.CON_DESTINATARIO_CTE
                             WHERE empresa.EMP_EMPRESA = " + codigoEmpresaPai.ToString() + " AND cte.CON_TIPO_AMBIENTE = 1 AND cte.CON_STATUS IN ('A', 'C', 'I') AND modelo.MOD_NUM in ('57','67') ";

            if (codigoEmpresa > 0)
                query += " AND empresa.EMP_CODIGO = " + codigoEmpresa.ToString();

            if (!string.IsNullOrWhiteSpace(cpfCnpjEmbarcador))
            {
                if (todosCNPJdaRaizEmbarcador)
                    query += " AND remetente.PCT_CPF_CNPJ like '" + cpfCnpjEmbarcador.Substring(0, 8) + "%'";
                else
                    query += " AND remetente.PCT_CPF_CNPJ = '" + cpfCnpjEmbarcador + "'";
            }

            if (dataAutorizacaoInicial != DateTime.MinValue)
                query += " AND cte.CON_RETORNOCTEDATA >= '" + dataAutorizacaoInicial.ToString("MM/dd/yyyy") + "'";

            if (dataAutorizacaoFinal != DateTime.MinValue)
                query += " AND cte.CON_RETORNOCTEDATA < '" + dataAutorizacaoFinal.AddDays(1).ToString("MM/dd/yyyy") + "'";

            if (dataEmissaoInicial != DateTime.MinValue)
                query += " AND cte.CON_DATAHORAEMISSAO >= '" + dataEmissaoInicial.ToString("MM/dd/yyyy") + "'";

            if (dataEmissaoFinal != DateTime.MinValue)
                query += " AND cte.CON_DATAHORAEMISSAO < '" + dataEmissaoFinal.AddDays(1).ToString("MM/dd/yyyy") + "'";


            var nhQuery = this.SessionNHiBernate.CreateSQLQuery(query);

            return nhQuery.UniqueResult<int>();
        }

        public IList<Dominio.ObjetosDeValor.Relatorios.RelatorioCTesEmitidosPorEmbarcador> RelatorioCTesEmitidosPorEmbarcador(int codigoEmpresaPai, int codigoEmpresa, string cpfCnpjEmbarcador, DateTime dataAutorizacaoInicial, DateTime dataAutorizacaoFinal, DateTime dataEmissaoInicial, DateTime dataEmissaoFinal, string propGrupo, string dirOrdenacaoGrupo, string propOrdenacao, string dirOrdenacao, int inicioRegistros, int maximoRegistros, bool paginar = true, bool todosCNPJdaRaizEmbarcador = false)
        {
            string query = @"SELECT 
							     cte.CON_CODIGO Codigo,
							     cte.CON_NUM Numero,
							     serie.ESE_NUMERO Serie,
							     cte.CON_DATAHORAEMISSAO DataEmissao,
							     cte.CON_RETORNOCTEDATA DataAutorizacao,
							     cte.CON_LOG Log,
                                 modeloDocumento.MOD_ABREVIACAO AbreviacaoModeloDocumentoFiscal,
                                 cte.CON_STATUS Status,
                                 remetente.PCT_CPF_CNPJ CPFCNPJRemetente,
                                 remetente.PCT_NOME Remetente,
                                 destinatario.PCT_CPF_CNPJ CPFCNPJDestinatario,
                                 destinatario.PCT_NOME Destinatario,
                                 empresa.EMP_CNPJ CNPJTransportador,
                                 empresa.EMP_RAZAO Transportador,
                                 cte.CON_VALOR_RECEBER ValorAReceber,
                                 cte.CON_VAL_ICMS ValorICMS,
                                 cte.CON_VALOR_FRETE ValorFrete,
                                 cte.CON_OBSGERAIS Observacao,
                                 PlacaVeiculo = (SELECT TOP(1) veiculo.CVE_PLACA FROM T_CTE_VEICULO veiculo WHERE veiculo.CON_CODIGO = cte.CON_CODIGO),
                                 NumeroNotaFiscal = (SELECT TOP(1) documento.NFC_NUMERO FROM T_CTE_DOCS documento WHERE documento.CON_CODIGO = cte.CON_CODIGO),
                                 Carga = (SELECT TOP(1) integracao.ICT_NUMERO_CARGA FROM T_INTEGRACAO_CTE integracao WHERE integracao.CON_CODIGO = cte.CON_CODIGO),
                                 CNF = (SELECT TOP(1) integracao.ICT_CODIGO_CONTROLE_INTERNO_CLIENTE FROM T_INTEGRACAO_CTE integracao WHERE integracao.CON_CODIGO = cte.CON_CODIGO),
							     ValorPedagio = (SELECT SUM(componente.CPT_VALOR) FROM T_CTE_COMP_PREST componente WHERE componente.CPT_NOME like 'ped%gio' AND componente.CON_CODIGO = cte.CON_CODIGO),
                                 empresaLocalidade.UF_SIGLA AS UFTransportador,
                                 remetenteLocalidade.UF_SIGLA AS UFRemetente,
                                 destinatarioLocalidade.UF_SIGLA AS UFDestinatario,
                                 subcontratacao.SUB_CODIGO_PROCESSO_TRANSPORTE Contrato

                             FROM T_CTE cte

                             INNER JOIN T_EMPRESA_SERIE serie ON serie.ESE_CODIGO = cte.CON_SERIE
                             INNER JOIN T_EMPRESA empresa ON empresa.EMP_CODIGO = cte.EMP_CODIGO
                             INNER JOIN T_LOCALIDADES empresaLocalidade on Empresa.LOC_CODIGO = empresaLocalidade.LOC_CODIGO
                             INNER JOIN T_MODDOCFISCAL modelo ON modelo.MOD_CODIGO = cte.CON_MODELODOC
                             LEFT OUTER JOIN T_CTE_PARTICIPANTE remetente ON remetente.PCT_CODIGO = cte.CON_REMETENTE_CTE
                             LEFT OUTER JOIN T_LOCALIDADES remetenteLocalidade on remetente.LOC_CODIGO = remetenteLocalidade.LOC_CODIGO
                             LEFT OUTER JOIN T_CTE_PARTICIPANTE destinatario ON destinatario.PCT_CODIGO = cte.CON_DESTINATARIO_CTE
                             LEFT OUTER JOIN T_LOCALIDADES destinatarioLocalidade on destinatario.LOC_CODIGO = destinatarioLocalidade.LOC_CODIGO
                             LEFT OUTER JOIN T_MODDOCFISCAL modeloDocumento on cte.CON_MODELODOC = modeloDocumento.MOD_CODIGO
                             LEFT OUTER JOIN T_SUBCONTRATACAO subcontratacao on cte.CON_CODIGO = subcontratacao.CON_CODIGO_SUBCONTRATACAO

                             WHERE
                                empresa.EMP_EMPRESA = " + codigoEmpresaPai.ToString() + @" AND
                                cte.CON_TIPO_AMBIENTE = 1 AND
                                cte.CON_STATUS IN ('A', 'C', 'I')
                                AND modelo.MOD_NUM in ('57','67') ";

            if (codigoEmpresa > 0)
                query += " AND empresa.EMP_CODIGO = " + codigoEmpresa.ToString();

            if (!string.IsNullOrWhiteSpace(cpfCnpjEmbarcador))
            {
                if (todosCNPJdaRaizEmbarcador)
                    query += " AND remetente.PCT_CPF_CNPJ like '" + cpfCnpjEmbarcador.Substring(0, 8) + "%'";
                else
                    query += " AND remetente.PCT_CPF_CNPJ = '" + cpfCnpjEmbarcador + "'";
            }

            if (dataAutorizacaoInicial != DateTime.MinValue)
                query += " AND cte.CON_RETORNOCTEDATA >= '" + dataAutorizacaoInicial.ToString("MM/dd/yyyy") + "'";

            if (dataAutorizacaoFinal != DateTime.MinValue)
                query += " AND cte.CON_RETORNOCTEDATA < '" + dataAutorizacaoFinal.AddDays(1).ToString("MM/dd/yyyy") + "'";

            if (dataEmissaoInicial != DateTime.MinValue)
                query += " AND cte.CON_DATAHORAEMISSAO >= '" + dataEmissaoInicial.ToString("yyyy-MM-dd") + "'";

            if (dataEmissaoFinal != DateTime.MinValue)
                query += " AND cte.CON_DATAHORAEMISSAO < '" + dataEmissaoFinal.AddDays(1).ToString("yyyy-MM-dd") + "'";

            var agrup = false;
            if (!string.IsNullOrWhiteSpace(propGrupo))
            {
                agrup = true;
                query += " order by " + propGrupo + " " + dirOrdenacaoGrupo;
            }

            if (!string.IsNullOrWhiteSpace(propOrdenacao) && propGrupo != propOrdenacao)
            {
                if (agrup)
                {
                    query += ", " + propOrdenacao + " " + dirOrdenacao;
                }
                else
                {
                    query += " order by " + propOrdenacao + " " + dirOrdenacao;
                }


            }

            if (paginar)
                query += " OFFSET " + inicioRegistros + " ROWS FETCH FIRST " + maximoRegistros + " ROWS ONLY";


            var nhQuery = this.SessionNHiBernate.CreateSQLQuery(query);

            nhQuery.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Dominio.ObjetosDeValor.Relatorios.RelatorioCTesEmitidosPorEmbarcador)));

            return nhQuery.List<Dominio.ObjetosDeValor.Relatorios.RelatorioCTesEmitidosPorEmbarcador>();
        }

        public IList<Dominio.ObjetosDeValor.Relatorios.RelatorioCTesRedespacho> RelatorioCTesRedespacho(Dominio.ObjetosDeValor.Embarcador.CTe.FiltroPesquisaRelatorioCTeRedespacho filtrosPesquisa)
        {
            string query = @" SELECT distinct
                                        cte.CON_CODIGO									AS Codigo, 
                                        cte.CON_NUM										AS Numero, 
                                        serie.ESE_NUMERO								AS Serie, 
                                        cte.CON_DATAHORAEMISSAO							AS DataEmissao, 
	                                    subcontratacao.SUB_DATA_EMISSAO_CONTRATO		AS DataEmissaoContrato, 
	                                    cte.CON_TIPO_SERVICO							AS TipoServico,
                                        CFOP = (SELECT TOP(1) cfop.CFO_CFOP FROM T_CFOP cfop WHERE cfop.CFO_CODIGO = cte.CFO_CODIGO),
                                        empresa.EMP_RAZAO								AS Transportador, 
	                                    empresa.EMP_CNPJ								AS CNPJTransportador, 
                                        remetente.PCT_NOME								AS Remetente, 
	                                    remetente.PCT_CPF_CNPJ							AS CPFCNPJRemetente, 
	                                    remetenteLocalidade.UF_SIGLA					AS UFRemetente, 
                                        destinatario.PCT_NOME							AS Destinatario, 
	                                    destinatario.PCT_CPF_CNPJ						AS CPFCNPJDestinatario, 
                                        destinatarioLocalidade.UF_SIGLA					AS UFDestinatario,
	                                    expedidor.PCT_NOME								AS Expedidor, 
	                                    expedidor.PCT_CPF_CNPJ							AS CPFCNPJExpedidor, 
	                                    recebedor.PCT_NOME								AS Recebedor, 
	                                    recebedor.PCT_CPF_CNPJ							AS CPFCNPJRecebedor, 
                                        tomadorPagador.PCT_NOME							AS Tomador,
	                                    tomadorPagador.PCT_CPF_CNPJ						AS CPFCNPJTomador, 
	                                    InicioPrestacaoLocalidade.UF_SIGLA				AS UFOrigemPrestacao,
	                                    TerminoPrestacaoLocalidade.UF_SIGLA				AS UFTerminoPrestacao,
	                                    cte.CON_CST										AS CSTICMS,
	                                    cte.CON_VAL_ICMS								AS ValorICMS, 
	                                    cte.CON_BC_ICMS									AS BaseCalculoICMS,
	                                    cte.CON_ALIQ_ICMS								AS AliquotaICMS,
                                        cte.CON_VALOR_RECEBER							AS ValorAReceber, 
                                        cte.CON_VALOR_FRETE								AS ValorFrete,
	                                    NumeroCteAnterior = (
											SUBSTRING((
												SELECT ', ' + ISNULL(cteSubContratado.CSU_NUM, '') 
												FROM T_CTE_SUBCONTRATADO cteSubContratado
												WHERE cteSubContratado.CON_CODIGO = cte.CON_CODIGO
												FOR XML PATH('')
											), 3, 1000)
										),
	                                    ChaveCteAnterior = (
											SUBSTRING((
												SELECT ', ' + ISNULL(cteSubContratado.CSU_CHAVE, '')
												FROM T_CTE_SUBCONTRATADO cteSubContratado
												WHERE cteSubContratado.CON_CODIGO = cte.CON_CODIGO
												FOR XML PATH('')
											), 3, 1000)
											
										),
										SerieCteAnterior = (
											SUBSTRING((
												SELECT ', ' + ISNULL(cteSubContratado.CSU_SERIE, '')
												FROM T_CTE_SUBCONTRATADO cteSubContratado
												WHERE cteSubContratado.CON_CODIGO = cte.CON_CODIGO
												FOR XML PATH('')
											), 3, 1000) 
										),
	                                    empresa.EMP_CODIGO_REGIME_TRIBUTARIO_CTE		AS RegimeTributarioCTe,
	                                    subcontratacao.SUB_CODIGO_PROCESSO_TRANSPORTE	AS CodigoProcessoTransporte,
	                                    subcontratacao.SUB_VALOR						AS ValorFreteContrato,
	                                    subcontratacao.SUB_DESCRICAO_PERCURSO			AS Percurso
                                    FROM 
                                        T_CTE cte
                                    LEFT OUTER JOIN 
                                        T_LOCALIDADES InicioPrestacaoLocalidade 
                                            ON cte.CON_LOCTERMINOPRESTACAO = InicioPrestacaoLocalidade.LOC_CODIGO
									LEFT JOIN
										T_EMPRESA empresaCTe 
											ON empresaCTe.EMP_CODIGO = cte.EMP_CODIGO
                                    LEFT OUTER JOIN 
                                        T_LOCALIDADES TerminoPrestacaoLocalidade 
                                            ON cte.CON_LOCTERMINOPRESTACAO = TerminoPrestacaoLocalidade.LOC_CODIGO
                                    INNER JOIN 
                                        T_EMPRESA_SERIE serie 
                                            ON serie.ESE_CODIGO = cte.CON_SERIE
                                    INNER JOIN 
                                        T_EMPRESA empresa 
                                            ON empresa.EMP_CODIGO = cte.EMP_CODIGO
                                    INNER JOIN 
                                        T_MODDOCFISCAL modelo 
                                            ON modelo.MOD_CODIGO = cte.CON_MODELODOC
                                    LEFT OUTER JOIN 
                                        T_CTE_PARTICIPANTE remetente 
                                            ON remetente.PCT_CODIGO = cte.CON_REMETENTE_CTE
                                    LEFT OUTER JOIN 
                                        T_CTE_PARTICIPANTE destinatario 
                                            ON destinatario.PCT_CODIGO = cte.CON_DESTINATARIO_CTE
                                    LEFT OUTER JOIN 
                                        T_LOCALIDADES remetenteLocalidade 
                                            ON remetente.LOC_CODIGO = remetenteLocalidade.LOC_CODIGO
                                    LEFT OUTER JOIN 
                                        T_LOCALIDADES destinatarioLocalidade 
                                            ON destinatario.LOC_CODIGO = destinatarioLocalidade.LOC_CODIGO
                                    LEFT OUTER JOIN 
                                        T_CTE_PARTICIPANTE expedidor 
                                            ON expedidor.PCT_CODIGO = cte.CON_EXPEDIDOR_CTE
                                    LEFT OUTER JOIN 
                                        T_CTE_PARTICIPANTE recebedor 
                                            ON recebedor.PCT_CODIGO = cte.CON_RECEBEDOR_CTE
                                    LEFT OUTER JOIN 
                                        T_CTE_PARTICIPANTE tomadorPagador 
                                            ON tomadorPagador.PCT_CODIGO = cte.CON_TOMADOR_PAGADOR_CTE
                                    LEFT OUTER JOIN 
                                         T_SUBCONTRATACAO subcontratacao 
                                            on cte.CON_CODIGO = subcontratacao.CON_CODIGO_SUBCONTRATACAO
                                WHERE 
                                    cte.CON_STATUS IN('A', 'C', 'I') AND
                                    modelo.MOD_NUM in ('57', '67')";

            if (filtrosPesquisa.TipoAmbiente != Dominio.Enumeradores.TipoAmbiente.Nenhum)
                query += $" AND cte.CON_TIPO_AMBIENTE = {filtrosPesquisa.TipoAmbiente.ToString("d")}";

            if (filtrosPesquisa.CodigoEmpresaPai > 0)
                query += $" AND empresa.EMP_EMPRESA = {filtrosPesquisa.CodigoEmpresaPai}";

            if (filtrosPesquisa.CodigoEmpresa > 0)
                query += $" AND empresa.EMP_CODIGO = {filtrosPesquisa.CodigoEmpresa}";

            if (!string.IsNullOrWhiteSpace(filtrosPesquisa.CpfCnpjEmbarcador))
                query += $" AND remetente.PCT_CPF_CNPJ = '{filtrosPesquisa.CpfCnpjEmbarcador}'";

            if (!string.IsNullOrWhiteSpace(filtrosPesquisa.CpfCnpjEmbarcadorUsuario))
            {
                if (filtrosPesquisa.CpfCnpjEmbarcadorUsuario.Length == 8)
                    query += $" AND remetente.PCT_CPF_CNPJ like '{filtrosPesquisa.CpfCnpjEmbarcadorUsuario}%'";
                else
                    query += $" AND remetente.PCT_CPF_CNPJ = '{filtrosPesquisa.CpfCnpjEmbarcadorUsuario}'";
            }

            if (!string.IsNullOrWhiteSpace(filtrosPesquisa.SerieEmissao))
                query += $" AND serie.ESE_NUMERO = '{filtrosPesquisa.SerieEmissao}'";

            if (filtrosPesquisa.DataEmissaoInicial != default)
                query += $" AND cte.CON_DATAHORAEMISSAO >= '{filtrosPesquisa.DataEmissaoInicial.ToString("yyyy-MM-dd")}'";

            if (filtrosPesquisa.DataEmissaoFinal != default)
                query += $" AND cte.CON_DATAHORAEMISSAO < '{filtrosPesquisa.DataEmissaoFinal.AddDays(1).ToString("yyyy-MM-dd")}'";

            NHibernate.ISQLQuery nhQuery = this.SessionNHiBernate.CreateSQLQuery(query);

            nhQuery.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Dominio.ObjetosDeValor.Relatorios.RelatorioCTesRedespacho)));

            return nhQuery.List<Dominio.ObjetosDeValor.Relatorios.RelatorioCTesRedespacho>();
        }

        public IList<Dominio.ObjetosDeValor.Relatorios.RelatorioCTesEmitidosPorEmbarcador> RelatorioCTesEmitidosPorEmbarcador(int codigoEmpresaPai, int codigoEmpresa, string cpfCnpjEmbarcador, DateTime dataAutorizacaoInicial, DateTime dataAutorizacaoFinal, DateTime dataEmissaoInicial, DateTime dataEmissaoFinal, bool todosCNPJdaRaizEmbarcador = false, string cnpjEmbarcadorUsuario = "", string tipoEmissao = "", string tipoCliente = "")
        {
            string query = @"
                             SELECT distinct 
                                cte.CON_CODIGO Codigo, 
                                cte.CON_NUM Numero, 
                                serie.ESE_NUMERO Serie, 
                                cte.CON_DATAHORAEMISSAO DataEmissao, 
                                cte.CON_RETORNOCTEDATA DataAutorizacao, 
                                cte.CON_LOG Log,
                                cte.CON_STATUS Status, 
                                remetente.PCT_CPF_CNPJ CPFCNPJRemetente, 
                                remetente.PCT_NOME Remetente, 
                                destinatario.PCT_CPF_CNPJ CPFCNPJDestinatario, 
                                destinatario.PCT_NOME Destinatario, 
                                empresa.EMP_CNPJ CNPJTransportador, 
                                empresa.EMP_RAZAO Transportador, 
                                cte.CON_VALOR_RECEBER ValorAReceber, 
                                cte.CON_VAL_ICMS ValorICMS, 
                                cte.CON_VALOR_FRETE ValorFrete, 
                                cte.CON_OBSGERAIS Observacao,
                                CFOP = (SELECT TOP(1) cfop.CFO_CFOP FROM T_CFOP cfop WHERE cfop.CFO_CODIGO = cte.CFO_CODIGO),
                                PlacaVeiculo = (SELECT TOP(1) veiculo.CVE_PLACA FROM T_CTE_VEICULO veiculo WHERE veiculo.CON_CODIGO = cte.CON_CODIGO),
                                NumeroNotaFiscal = (SELECT TOP(1) documento.NFC_NUMERO FROM T_CTE_DOCS documento WHERE documento.CON_CODIGO = cte.CON_CODIGO),
                                ChaveNotaFiscal = (SELECT TOP(1) documento.NFC_CHAVENFE FROM T_CTE_DOCS documento WHERE documento.CON_CODIGO = cte.CON_CODIGO),
                                Carga = (SELECT TOP(1) integracao.ICT_NUMERO_CARGA FROM T_INTEGRACAO_CTE integracao WHERE integracao.CON_CODIGO = cte.CON_CODIGO),
                                CNF = (SELECT TOP(1) integracao.ICT_CODIGO_CONTROLE_INTERNO_CLIENTE FROM T_INTEGRACAO_CTE integracao WHERE integracao.CON_CODIGO = cte.CON_CODIGO),
    							ValorPedagio = (SELECT SUM(componente.CPT_VALOR) FROM T_CTE_COMP_PREST componente WHERE componente.CPT_NOME like 'ped%gio' AND componente.CON_CODIGO = cte.CON_CODIGO),
                                ValorAdicional = (SELECT SUM(componente.CPT_VALOR) FROM T_CTE_COMP_PREST componente WHERE componente.CPT_NOME like 'adicional' AND componente.CON_CODIGO = cte.CON_CODIGO),
                                cte.CON_CHAVECTE ChaveCTe, 
                                empresaLocalidade.UF_SIGLA AS UFTransportador, 
                                remetenteLocalidade.UF_SIGLA AS UFRemetente, 
                                destinatarioLocalidade.UF_SIGLA AS UFDestinatario,
                                CodIntegracao = (SELECT TOP(1) cast(integracao.ICT_CODIGO as varchar(18)) FROM T_INTEGRACAO_CTE integracao WHERE integracao.CON_CODIGO = cte.CON_CODIGO),
                                tomadorPagador.PCT_CPF_CNPJ CPFCNPJTomador, 
                                tomadorPagador.PCT_NOME Tomador ,
                                cte.CON_VALOR_TOTAL_MERC ValorTotalMercadoria,
                                subcontratacao.SUB_CODIGO_PROCESSO_TRANSPORTE Contrato

                            FROM 
                                T_CTE cte

                            INNER JOIN 
                                T_EMPRESA_SERIE serie 
                                    ON serie.ESE_CODIGO = cte.CON_SERIE
                            INNER JOIN 
                                T_EMPRESA empresa 
                                    ON empresa.EMP_CODIGO = cte.EMP_CODIGO
                            INNER JOIN 
                                T_LOCALIDADES empresaLocalidade 
                                    ON Empresa.LOC_CODIGO = empresaLocalidade.LOC_CODIGO
                            INNER JOIN 
                                T_MODDOCFISCAL modelo 
                                    ON modelo.MOD_CODIGO = cte.CON_MODELODOC
                            LEFT OUTER JOIN 
                                T_CTE_PARTICIPANTE remetente 
                                    ON remetente.PCT_CODIGO = cte.CON_REMETENTE_CTE
                            LEFT OUTER JOIN 
                                T_CTE_PARTICIPANTE destinatario 
                                    ON destinatario.PCT_CODIGO = cte.CON_DESTINATARIO_CTE
                            LEFT OUTER JOIN 
                                T_LOCALIDADES remetenteLocalidade 
                                    ON remetente.LOC_CODIGO = remetenteLocalidade.LOC_CODIGO
                            LEFT OUTER JOIN 
                                T_LOCALIDADES destinatarioLocalidade 
                                    ON destinatario.LOC_CODIGO = destinatarioLocalidade.LOC_CODIGO
                            LEFT OUTER JOIN 
                                T_INTEGRACAO_CTE integracao 
                                    ON cte.CON_CODIGO = integracao.CON_CODIGO
                            LEFT OUTER JOIN 
                                T_CTE_PARTICIPANTE expedidor 
                                    ON expedidor.PCT_CODIGO = cte.CON_EXPEDIDOR_CTE
                            LEFT OUTER JOIN 
                                T_CTE_PARTICIPANTE recebedor 
                                    ON recebedor.PCT_CODIGO = cte.CON_RECEBEDOR_CTE
                            LEFT OUTER JOIN 
                                T_CTE_PARTICIPANTE tomadorPagador 
                                    ON tomadorPagador.PCT_CODIGO = cte.CON_TOMADOR_PAGADOR_CTE
                             LEFT OUTER JOIN 
                                 T_SUBCONTRATACAO subcontratacao 
                                    on cte.CON_CODIGO = subcontratacao.CON_CODIGO_SUBCONTRATACAO
                            WHERE 
                                empresa.EMP_EMPRESA = " + codigoEmpresaPai.ToString() + @" AND 
                                cte.CON_STATUS IN('A', 'C', 'I') AND
                                modelo.MOD_NUM in ('57', '67')";

#if DEBUG

#else
            query += "AND cte.CON_TIPO_AMBIENTE = 1";
#endif

            if (codigoEmpresa > 0)
                query += " AND empresa.EMP_CODIGO = " + codigoEmpresa.ToString();

            if (!string.IsNullOrWhiteSpace(cpfCnpjEmbarcador))
            {
                if (tipoCliente == "0")
                {
                    if (todosCNPJdaRaizEmbarcador)
                        query += " AND remetente.PCT_CPF_CNPJ like '" + cpfCnpjEmbarcador.Substring(0, 8) + "%'";
                    else
                        query += " AND remetente.PCT_CPF_CNPJ = '" + cpfCnpjEmbarcador + "'";
                }
                else if (tipoCliente == "1")
                {
                    if (todosCNPJdaRaizEmbarcador)
                        query += " AND expedidor.PCT_CPF_CNPJ like '" + cpfCnpjEmbarcador.Substring(0, 8) + "%'";
                    else
                        query += " AND expedidor.PCT_CPF_CNPJ = '" + cpfCnpjEmbarcador + "'";
                }
                else if (tipoCliente == "2")
                {
                    if (todosCNPJdaRaizEmbarcador)
                        query += " AND recebedor.PCT_CPF_CNPJ like '" + cpfCnpjEmbarcador.Substring(0, 8) + "%'";
                    else
                        query += " AND recebedor.PCT_CPF_CNPJ = '" + cpfCnpjEmbarcador + "'";
                }
                else if (tipoCliente == "3")
                {
                    if (todosCNPJdaRaizEmbarcador)
                        query += " AND destinatario.PCT_CPF_CNPJ like '" + cpfCnpjEmbarcador.Substring(0, 8) + "%'";
                    else
                        query += " AND destinatario.PCT_CPF_CNPJ = '" + cpfCnpjEmbarcador + "'";
                }
                else if (tipoCliente == "4")
                {
                    if (todosCNPJdaRaizEmbarcador)
                        query += " AND tomadorPagador.PCT_CPF_CNPJ like '" + cpfCnpjEmbarcador.Substring(0, 8) + "%'";
                    else
                        query += " AND tomadorPagador.PCT_CPF_CNPJ = '" + cpfCnpjEmbarcador + "'";
                }
                else
                {
                    if (todosCNPJdaRaizEmbarcador)
                        query += " AND remetente.PCT_CPF_CNPJ like '" + cpfCnpjEmbarcador.Substring(0, 8) + "%'";
                    else
                        query += " AND remetente.PCT_CPF_CNPJ = '" + cpfCnpjEmbarcador + "'";
                }
            }

            if (!string.IsNullOrWhiteSpace(cnpjEmbarcadorUsuario))
            {
                if (cnpjEmbarcadorUsuario.Length == 8)
                    query += " AND remetente.PCT_CPF_CNPJ like '" + cnpjEmbarcadorUsuario + "%'";
                else
                    query += " AND remetente.PCT_CPF_CNPJ = '" + cnpjEmbarcadorUsuario + "'";
            }

            if (dataAutorizacaoInicial != DateTime.MinValue)
                query += " AND cte.CON_RETORNOCTEDATA >= '" + dataAutorizacaoInicial.ToString("yyyy-MM-dd") + "'";

            if (dataAutorizacaoFinal != DateTime.MinValue)
                query += " AND cte.CON_RETORNOCTEDATA < '" + dataAutorizacaoFinal.AddDays(1).ToString("yyyy-MM-dd") + "'";

            if (dataEmissaoInicial != DateTime.MinValue)
                query += " AND cte.CON_DATAHORAEMISSAO >= '" + dataEmissaoInicial.ToString("yyyy-MM-dd") + "'";

            if (dataEmissaoFinal != DateTime.MinValue)
                query += " AND cte.CON_DATAHORAEMISSAO < '" + dataEmissaoFinal.AddDays(1).ToString("yyyy-MM-dd") + "'";

            if (tipoEmissao == "0")
                query += " AND cte.CON_CODIGO NOT IN (SELECT CON_CODIGO FROM T_INTEGRACAO_CTE)";
            else if (tipoEmissao == "1")
                query += " AND cte.CON_CODIGO IN (SELECT CON_CODIGO FROM T_INTEGRACAO_CTE)";

            var nhQuery = this.SessionNHiBernate.CreateSQLQuery(query);

            nhQuery.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Dominio.ObjetosDeValor.Relatorios.RelatorioCTesEmitidosPorEmbarcador)));

            return nhQuery.SetTimeout(240).List<Dominio.ObjetosDeValor.Relatorios.RelatorioCTesEmitidosPorEmbarcador>();
        }

        public IList<Dominio.ObjetosDeValor.Relatorios.RelatorioCTesEmitidosPorEmbarcador> RelatorioCTesEmitidosPorEmbarcadorSumarizado(int codigoEmpresaPai, int codigoEmpresa, string cpfCnpjEmbarcador, DateTime dataAutorizacaoInicial, DateTime dataAutorizacaoFinal, DateTime dataEmissaoInicial, DateTime dataEmissaoFinal, bool todosCNPJdaRaizEmbarcador = false, string cnpjEmbarcadorUsuario = "", string tipoEmissao = "", string tipoCliente = "")
        {
            string query = @"
                     SELECT distinct 
                        part.PCT_CPF_CNPJ CNPJTransportador,
                        part.PCT_NOME Transportador,
                        COUNT(CON_CODIGO) QuantidadeCTe,
                        SUM(cte.CON_VALOR_RECEBER) ValorFrete
                    FROM 
                        T_CTE cte

                    INNER JOIN 
                        T_CTE_PARTICIPANTE part 
                            ON cte.CON_REMETENTE_CTE = part.PCT_CODIGO
                    INNER JOIN 
                        T_EMPRESA empresa 
                            ON empresa.EMP_CODIGO = cte.EMP_CODIGO
                    LEFT OUTER JOIN 
                        T_CTE_PARTICIPANTE remetente 
                            ON remetente.PCT_CODIGO = cte.CON_REMETENTE_CTE
                    INNER JOIN 
                        T_MODDOCFISCAL modelo 
                            ON modelo.MOD_CODIGO = cte.CON_MODELODOC
                    LEFT OUTER JOIN 
                        T_CTE_PARTICIPANTE destinatario 
                            ON destinatario.PCT_CODIGO = cte.CON_REMETENTE_CTE
                    LEFT OUTER JOIN 
                        T_CTE_PARTICIPANTE expedidor 
                            ON expedidor.PCT_CODIGO = CTE.CON_EXPEDIDOR_CTE
                    LEFT OUTER JOIN 
                        T_CTE_PARTICIPANTE recebedor 
                            ON recebedor.PCT_CODIGO = CTE.CON_RECEBEDOR_CTE
                    LEFT OUTER JOIN 
                        T_CTE_PARTICIPANTE tomadorPagador 
                            ON tomadorPagador.PCT_CODIGO = CTE.CON_TOMADOR_PAGADOR_CTE

                    WHERE 
                        empresa.EMP_EMPRESA = " + codigoEmpresaPai.ToString() + @" AND 
                        cte.CON_STATUS IN('A', 'C', 'I') AND
                        modelo.MOD_NUM in ('57', '67')";

#if DEBUG

#else
            query += "AND cte.CON_TIPO_AMBIENTE = 1";
#endif

            if (codigoEmpresa > 0)
                query += " AND empresa.EMP_CODIGO = " + codigoEmpresa.ToString();

            if (!string.IsNullOrWhiteSpace(cpfCnpjEmbarcador))
            {
                if (tipoCliente == "0")
                {
                    if (todosCNPJdaRaizEmbarcador)
                        query += " AND remetente.PCT_CPF_CNPJ like '" + cpfCnpjEmbarcador.Substring(0, 8) + "%'";
                    else
                        query += " AND remetente.PCT_CPF_CNPJ = '" + cpfCnpjEmbarcador + "'";
                }
                else if (tipoCliente == "1")
                {
                    if (todosCNPJdaRaizEmbarcador)
                        query += " AND expedidor.PCT_CPF_CNPJ like '" + cpfCnpjEmbarcador.Substring(0, 8) + "%'";
                    else
                        query += " AND expedidor.PCT_CPF_CNPJ = '" + cpfCnpjEmbarcador + "'";
                }
                else if (tipoCliente == "2")
                {
                    if (todosCNPJdaRaizEmbarcador)
                        query += " AND recebedor.PCT_CPF_CNPJ like '" + cpfCnpjEmbarcador.Substring(0, 8) + "%'";
                    else
                        query += " AND recebedor.PCT_CPF_CNPJ = '" + cpfCnpjEmbarcador + "'";
                }
                else if (tipoCliente == "3")
                {
                    if (todosCNPJdaRaizEmbarcador)
                        query += " AND destinatario.PCT_CPF_CNPJ like '" + cpfCnpjEmbarcador.Substring(0, 8) + "%'";
                    else
                        query += " AND destinatario.PCT_CPF_CNPJ = '" + cpfCnpjEmbarcador + "'";
                }
                else if (tipoCliente == "4")
                {
                    if (todosCNPJdaRaizEmbarcador)
                        query += " AND tomadorPagador.PCT_CPF_CNPJ like '" + cpfCnpjEmbarcador.Substring(0, 8) + "%'";
                    else
                        query += " AND tomadorPagador.PCT_CPF_CNPJ = '" + cpfCnpjEmbarcador + "'";
                }
                else
                {
                    if (todosCNPJdaRaizEmbarcador)
                        query += " AND remetente.PCT_CPF_CNPJ like '" + cpfCnpjEmbarcador.Substring(0, 8) + "%'";
                    else
                        query += " AND remetente.PCT_CPF_CNPJ = '" + cpfCnpjEmbarcador + "'";
                }
            }

            if (!string.IsNullOrWhiteSpace(cnpjEmbarcadorUsuario))
            {
                if (cnpjEmbarcadorUsuario.Length == 8)
                    query += " AND remetente.PCT_CPF_CNPJ like '" + cnpjEmbarcadorUsuario + "%'";
                else
                    query += " AND remetente.PCT_CPF_CNPJ = '" + cnpjEmbarcadorUsuario + "'";

            }

            if (dataAutorizacaoInicial != DateTime.MinValue)
                query += " AND cte.CON_RETORNOCTEDATA >= '" + dataAutorizacaoInicial.ToString("yyyy-MM-dd") + "'";

            if (dataAutorizacaoFinal != DateTime.MinValue)
                query += " AND cte.CON_RETORNOCTEDATA < '" + dataAutorizacaoFinal.AddDays(1).ToString("yyyy-MM-dd") + "'";

            if (dataEmissaoInicial != DateTime.MinValue)
                query += " AND cte.CON_DATAHORAEMISSAO >= '" + dataEmissaoInicial.ToString("yyyy-MM-dd") + "'";

            if (dataEmissaoFinal != DateTime.MinValue)
                query += " AND cte.CON_DATAHORAEMISSAO < '" + dataEmissaoFinal.AddDays(1).ToString("yyyy-MM-dd") + "'";

            if (tipoEmissao == "0")
                query += " AND cte.CON_CODIGO NOT IN (SELECT CON_CODIGO FROM T_INTEGRACAO_CTE)";
            else if (tipoEmissao == "1")
                query += " AND cte.CON_CODIGO IN (SELECT CON_CODIGO FROM T_INTEGRACAO_CTE)";


            query += @"GROUP BY part.PCT_CPF_CNPJ, part.PCT_NOME 
                       ORDER BY COUNT(CON_CODIGO)";

            var nhQuery = this.SessionNHiBernate.CreateSQLQuery(query);

            nhQuery.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Dominio.ObjetosDeValor.Relatorios.RelatorioCTesEmitidosPorEmbarcador)));

            return nhQuery.SetTimeout(120).List<Dominio.ObjetosDeValor.Relatorios.RelatorioCTesEmitidosPorEmbarcador>();
        }

        public dynamic ConsutarParaEntregas(int codigoEmpresa, int numeroInicial, int numeroFinal, DateTime dataEmissaoInicial, DateTime dataEmissaoFinal, string cpfCnpjRemetente, string cpfCnpjDestinatario, int codigoLocalidadeTerminoPrestacao, string numeroDocumento, int inicioRegistros, int maximoRegistros)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.ConhecimentoDeTransporteEletronico>();

            var result = from obj in query
                         where obj.Empresa.Codigo == codigoEmpresa &&
                               obj.Status.Equals("A") &&
                               (obj.ModeloDocumentoFiscal.Numero.Equals("57") || obj.ModeloDocumentoFiscal.Numero.Equals("67"))
                         select obj;

            if (numeroInicial > 0)
                result = result.Where(o => o.Numero >= numeroInicial);

            if (numeroFinal > 0)
                result = result.Where(o => o.Numero <= numeroFinal);

            if (dataEmissaoInicial != DateTime.MinValue)
                result = result.Where(o => o.DataEmissao.Value >= dataEmissaoInicial.Date);

            if (dataEmissaoFinal != DateTime.MinValue)
                result = result.Where(o => o.DataEmissao.Value < dataEmissaoFinal.AddDays(1).Date);

            if (!string.IsNullOrWhiteSpace(cpfCnpjRemetente))
                result = result.Where(o => o.Remetente.CPF_CNPJ.Equals(cpfCnpjRemetente));

            if (!string.IsNullOrWhiteSpace(cpfCnpjDestinatario))
                result = result.Where(o => o.Destinatario.CPF_CNPJ.Equals(cpfCnpjDestinatario));

            if (codigoLocalidadeTerminoPrestacao > 0)
                result = result.Where(o => o.LocalidadeTerminoPrestacao.Codigo == codigoLocalidadeTerminoPrestacao);

            if (!string.IsNullOrWhiteSpace(numeroDocumento))
                result = result.Where(o => (from obj in o.Documentos where obj.Numero.Equals(numeroDocumento) select obj.CTE.Codigo).Contains(o.Codigo));

            result = result.Where(o => !(from obj in this.SessionNHiBernate.Query<Dominio.Entidades.Entrega>() where obj.CTes.Contains(o) select o).Contains(o));

            return result.OrderByDescending(o => o.Numero).ThenByDescending(o => o.Serie.Numero).Skip(inicioRegistros).Take(maximoRegistros).Select(obj => new
            {
                obj.Codigo,
                Numero = obj.Numero + " - " + obj.Serie.Numero,
                InicioPrestacao = obj.LocalidadeInicioPrestacao.Estado.Sigla + " / " + obj.LocalidadeInicioPrestacao.Descricao,
                TerminoPrestacao = obj.LocalidadeTerminoPrestacao.Estado.Sigla + " / " + obj.LocalidadeTerminoPrestacao.Descricao,
                Destinatario = obj.Destinatario.CPF_CNPJ + " - " + obj.Destinatario.Nome,
                ValorFrete = obj.ValorFrete.ToString("n2")
            }).ToList();
        }

        public int ContarConsutaParaEntregas(int codigoEmpresa, int numeroInicial, int numeroFinal, DateTime dataEmissaoInicial, DateTime dataEmissaoFinal, string cpfCnpjRemetente, string cpfCnpjDestinatario, int codigoLocalidadeTerminoPrestacao, string numeroDocumento)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.ConhecimentoDeTransporteEletronico>();

            var result = from obj in query
                         where obj.Empresa.Codigo == codigoEmpresa &&
                               obj.Status.Equals("A") &&
                               (obj.ModeloDocumentoFiscal.Numero.Equals("57") || obj.ModeloDocumentoFiscal.Numero.Equals("67"))
                         select obj;

            if (numeroInicial > 0)
                result = result.Where(o => o.Numero >= numeroInicial);

            if (numeroFinal > 0)
                result = result.Where(o => o.Numero <= numeroFinal);

            if (dataEmissaoInicial != DateTime.MinValue)
                result = result.Where(o => o.DataEmissao.Value >= dataEmissaoInicial.Date);

            if (dataEmissaoFinal != DateTime.MinValue)
                result = result.Where(o => o.DataEmissao.Value < dataEmissaoFinal.AddDays(1).Date);

            if (!string.IsNullOrWhiteSpace(cpfCnpjRemetente))
                result = result.Where(o => o.Remetente.CPF_CNPJ.Equals(cpfCnpjRemetente));

            if (!string.IsNullOrWhiteSpace(cpfCnpjDestinatario))
                result = result.Where(o => o.Destinatario.CPF_CNPJ.Equals(cpfCnpjDestinatario));

            if (codigoLocalidadeTerminoPrestacao > 0)
                result = result.Where(o => o.LocalidadeTerminoPrestacao.Codigo == codigoLocalidadeTerminoPrestacao);

            if (!string.IsNullOrWhiteSpace(numeroDocumento))
                result = result.Where(o => (from obj in o.Documentos where obj.Numero.Equals(numeroDocumento) select obj.CTE.Codigo).Contains(o.Codigo));

            result = result.Where(o => !(from obj in this.SessionNHiBernate.Query<Dominio.Entidades.Entrega>() where obj.CTes.Contains(o) select o).Contains(o));

            return result.Count();
        }

        public List<Dominio.Entidades.ConhecimentoDeTransporteEletronico> Consultar(int codigoEmpresa, DateTime dataEmissaoInicial, DateTime dataEmissaoFinal, int numeroInicial, int numeroFinal, string cpfCnpjTomador, string status, int inicioRegistros, int maximoRegistros)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.ConhecimentoDeTransporteEletronico>();

            var result = from obj in query where obj.Empresa.Codigo == codigoEmpresa && obj.TipoAmbiente == obj.Empresa.TipoAmbiente && (obj.ModeloDocumentoFiscal.Numero.Equals("57") || obj.ModeloDocumentoFiscal.Numero.Equals("67")) select obj;

            if (!string.IsNullOrWhiteSpace(status))
                result = result.Where(o => o.Status.Equals(status));

            if (dataEmissaoInicial != DateTime.MinValue)
                result = result.Where(o => o.DataEmissao >= dataEmissaoInicial.Date);

            if (dataEmissaoFinal != DateTime.MinValue)
                result = result.Where(o => o.DataEmissao < dataEmissaoFinal.AddDays(1).Date);

            if (numeroInicial > 0)
                result = result.Where(o => o.Numero >= numeroInicial);

            if (numeroFinal > 0)
                result = result.Where(o => o.Numero <= numeroFinal);

            if (!string.IsNullOrWhiteSpace(cpfCnpjTomador))
            {
                result = result.Where(o => (o.TipoTomador == Dominio.Enumeradores.TipoTomador.Destinatario && o.Destinatario.CPF_CNPJ == cpfCnpjTomador) ||
                                           (o.TipoTomador == Dominio.Enumeradores.TipoTomador.Expedidor && o.Expedidor.CPF_CNPJ == cpfCnpjTomador) ||
                                           (o.TipoTomador == Dominio.Enumeradores.TipoTomador.Outros && o.OutrosTomador.CPF_CNPJ == cpfCnpjTomador) ||
                                           (o.TipoTomador == Dominio.Enumeradores.TipoTomador.Recebedor && o.Recebedor.CPF_CNPJ == cpfCnpjTomador) ||
                                           (o.TipoTomador == Dominio.Enumeradores.TipoTomador.Remetente && o.Remetente.CPF_CNPJ == cpfCnpjTomador));
            }

            result = result.OrderBy(o => o.Numero);

            if (inicioRegistros > 0)
                result = result.Skip(inicioRegistros);

            if (maximoRegistros > 0)
                result = result.Take(maximoRegistros);

            return result.ToList();
        }

        public int ContarConsulta(int codigoEmpresa, DateTime dataEmissaoInicial, DateTime dataEmissaoFinal, int numeroInicial, int numeroFinal, string cpfCnpjTomador, string status)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.ConhecimentoDeTransporteEletronico>();

            var result = from obj in query where obj.Empresa.Codigo == codigoEmpresa && obj.TipoAmbiente == obj.Empresa.TipoAmbiente && (obj.ModeloDocumentoFiscal.Numero.Equals("57") || obj.ModeloDocumentoFiscal.Numero.Equals("67")) select obj;

            if (!string.IsNullOrWhiteSpace(status))
                result = result.Where(o => o.Status.Equals(status));

            if (dataEmissaoInicial != DateTime.MinValue)
                result = result.Where(o => o.DataEmissao >= dataEmissaoInicial.Date);

            if (dataEmissaoFinal != DateTime.MinValue)
                result = result.Where(o => o.DataEmissao < dataEmissaoFinal.AddDays(1).Date);

            if (numeroInicial > 0)
                result = result.Where(o => o.Numero >= numeroInicial);

            if (numeroFinal > 0)
                result = result.Where(o => o.Numero <= numeroFinal);

            if (!string.IsNullOrWhiteSpace(cpfCnpjTomador))
            {
                result = result.Where(o => (o.TipoTomador == Dominio.Enumeradores.TipoTomador.Destinatario && o.Destinatario.CPF_CNPJ == cpfCnpjTomador) ||
                                           (o.TipoTomador == Dominio.Enumeradores.TipoTomador.Expedidor && o.Expedidor.CPF_CNPJ == cpfCnpjTomador) ||
                                           (o.TipoTomador == Dominio.Enumeradores.TipoTomador.Outros && o.OutrosTomador.CPF_CNPJ == cpfCnpjTomador) ||
                                           (o.TipoTomador == Dominio.Enumeradores.TipoTomador.Recebedor && o.Recebedor.CPF_CNPJ == cpfCnpjTomador) ||
                                           (o.TipoTomador == Dominio.Enumeradores.TipoTomador.Remetente && o.Remetente.CPF_CNPJ == cpfCnpjTomador));
            }

            return result.Count();
        }

        public List<Dominio.Entidades.ConhecimentoDeTransporteEletronico> ConsultarPendentesCancelamento(int codigoEmpresaPai, int inicioRegistros, int maximoRegistros)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.ConhecimentoDeTransporteEletronico>();

            var result = from obj in query where obj.DataEmissao >= DateTime.Now.AddDays(-7) && obj.Status.Equals("A") && (obj.ModeloDocumentoFiscal.Numero.Equals("57") || obj.ModeloDocumentoFiscal.Numero.Equals("67")) select obj;

            if (codigoEmpresaPai > 0)
            {
                result = result.Where(cte => cte.Empresa.EmpresaPai.Codigo == codigoEmpresaPai);
            }

            result = result.Where(cte => cte.MensagemRetornoSefaz.Contains("Vedado"));

            return result.Fetch(cte => cte.Empresa).OrderBy(cte => cte.Empresa.NomeFantasia).Skip(inicioRegistros).Take(maximoRegistros).ToList();
        }

        public int ContarConsultarPendentesCancelamento(int codigoEmpresaPai)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.ConhecimentoDeTransporteEletronico>();

            var result = from obj in query where obj.DataEmissao >= DateTime.Now.AddDays(-7) && obj.Status.Equals("A") && (obj.ModeloDocumentoFiscal.Numero.Equals("57") || obj.ModeloDocumentoFiscal.Numero.Equals("67")) select obj;

            if (codigoEmpresaPai > 0)
            {
                result = result.Where(o => o.Empresa.EmpresaPai.Codigo == codigoEmpresaPai);
            }

            result = result.Where(o => o.MensagemRetornoSefaz.Contains("Vedado"));

            return result.Count();
        }

        public List<Dominio.ObjetosDeValor.ConsultaCTe> ConsultarCTesEmbarcador(int codigoEmpresaPai, string cpfCnpjRemetente, Dominio.Enumeradores.TipoAmbiente tipoAmbiente, string[] status, int numeroInicial, int numeroFinal, int inicioRegistros, int maximoRegistros)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.ConhecimentoDeTransporteEletronico>();

            DateTime dataInicio = DateTime.Now;
            dataInicio = dataInicio.AddDays(-30);

            var result = from obj in query
                         where
                             obj.DataEmissao >= dataInicio &&
                             obj.TipoAmbiente == tipoAmbiente &&
                             obj.Empresa.EmpresaPai.Codigo == codigoEmpresaPai &&
                             status.Contains(obj.Status) &&
                             (obj.ModeloDocumentoFiscal.Numero.Equals("57") || obj.ModeloDocumentoFiscal.Numero.Equals("67"))
                         select obj;

            if (!string.IsNullOrWhiteSpace(cpfCnpjRemetente))
                result = result.Where(o => o.Remetente.CPF_CNPJ.Equals(cpfCnpjRemetente));

            if (numeroInicial > 0)
                result = result.Where(o => o.Numero >= numeroInicial);

            if (numeroFinal > 0)
                result = result.Where(o => o.Numero <= numeroFinal);

            return result.Select(o => new Dominio.ObjetosDeValor.ConsultaCTe()
            {
                Codigo = o.Codigo,
                DataEmissao = o.DataEmissao,
                Destinatario = o.Destinatario,
                MensagemStatus = o.MensagemStatus,
                MensagemRetornoSefaz = o.MensagemRetornoSefaz,
                Numero = o.Numero,
                Remetente = o.Remetente,
                Serie = o.Serie.Numero,
                Status = o.Status,
                TipoCTe = o.TipoCTE,
                TipoServico = o.TipoServico,
                Valor = o.ValorFrete
            })
            .OrderByDescending(o => o.Numero)
            .Skip(inicioRegistros)
            .Take(maximoRegistros)
            .Timeout(120)
            .ToList();
        }

        public int ContarConsultaCTesEmbarcador(int codigoEmpresaPai, string cpfCnpjRemetente, Dominio.Enumeradores.TipoAmbiente tipoAmbiente, string[] status, int numeroInicial, int numeroFinal)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.ConhecimentoDeTransporteEletronico>();

            DateTime dataInicio = DateTime.Now;
            dataInicio = dataInicio.AddDays(-30);

            var result = from obj in query
                         where
                             obj.DataEmissao >= dataInicio &&
                             obj.TipoAmbiente == tipoAmbiente &&
                             obj.Empresa.EmpresaPai.Codigo == codigoEmpresaPai &&
                             status.Contains(obj.Status) &&
                             (obj.ModeloDocumentoFiscal.Numero.Equals("57") || obj.ModeloDocumentoFiscal.Numero.Equals("67"))
                         select obj;

            if (!string.IsNullOrWhiteSpace(cpfCnpjRemetente))
                result = result.Where(o => o.Remetente.CPF_CNPJ.Equals(cpfCnpjRemetente));

            if (numeroInicial > 0)
                result = result.Where(o => o.Numero >= numeroInicial);

            if (numeroFinal > 0)
                result = result.Where(o => o.Numero <= numeroFinal);

            return result.Count();
        }

        public IList<Dominio.Relatorios.Embarcador.DataSource.CTe.CTes.FaturamentoPorGrupoPessoas> ConsultarRelatorioFaturamentoPorGrupoPessoas(bool count, Dominio.ObjetosDeValor.Embarcador.CTe.FiltroPesquisaRelatorioFaturamentoPorGrupoPessoas filtrosPesquisa, List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedades, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            var sqlDinamico = ObterSelectConsultaRelatorioFaturamentoPorGrupoPessoas(count, filtrosPesquisa, propriedades, parametrosConsulta);

            var query = sqlDinamico.CriarSQLQuery(this.SessionNHiBernate);

            query.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Dominio.Relatorios.Embarcador.DataSource.CTe.CTes.FaturamentoPorGrupoPessoas)));

            return query.SetTimeout(300).List<Dominio.Relatorios.Embarcador.DataSource.CTe.CTes.FaturamentoPorGrupoPessoas>();
        }

        public int ContarConsultaRelatorioFaturamentoPorGrupoPessoas(bool count, Dominio.ObjetosDeValor.Embarcador.CTe.FiltroPesquisaRelatorioFaturamentoPorGrupoPessoas filtrosPesquisa, List<PropriedadeAgrupamento> propriedades)
        {
            var sqlDinamico = ObterSelectConsultaRelatorioFaturamentoPorGrupoPessoas(count, filtrosPesquisa, propriedades, null);

            var query = sqlDinamico.CriarSQLQuery(this.SessionNHiBernate);

            return query.SetTimeout(300).UniqueResult<int>();
        }

        public IList<Dominio.Relatorios.Embarcador.DataSource.CTe.CTes.FalhaNumeracaoCTe> ConsultarRelatorioFalhaNumeracaoCTe(DateTime dataInicial, DateTime dataFinal, int codigoEmpresa, int codigoModelo, int codigoSerie, string numeracaoInicialFinal)
        {
            var sql = @"declare @max int, @curr int, @modelo int, @serie int, @tipo_ambiente int, @tipo_controle int, @data_inicio datetime, @data_fim datetime, @empresa int
                    declare @numeros table (numero int, serie int, empresa int)
                    set @empresa = " + codigoEmpresa.ToString() + @"
                    set @modelo = " + codigoModelo.ToString() + @"
                    set @serie = " + codigoSerie.ToString() + @"
                    set @tipo_ambiente = 1
                    set @tipo_controle = 1
                    set @data_inicio = '" + dataInicial.ToString("yyyy-MM-dd") + @"'
                    set @data_fim = '" + dataFinal.AddDays(1).ToString("yyyy-MM-dd") + @"'
                    select @max = max(con_num) from t_cte where emp_codigo = @empresa and con_serie = @serie and CON_MODELODOC = @modelo and con_tipo_ambiente = @tipo_ambiente and con_tipo_controle = @tipo_controle and con_Datahoraemissao <= @data_fim
                    select @curr = min(con_num) from t_cte where emp_codigo = @empresa and con_serie = @serie and CON_MODELODOC = @modelo and con_tipo_ambiente = @tipo_ambiente and con_tipo_controle = @tipo_controle and con_datahoraemissao >= @data_inicio
                    while @curr < @max
                    begin
                    set @curr = @curr + 1
                    if(not EXISTS(select con_num from t_cte where emp_codigo = @empresa and con_num = @curr and con_serie = @serie and CON_MODELODOC = @modelo and con_tipo_ambiente = @tipo_ambiente and con_tipo_controle = @tipo_controle ))
                    begin
                    insert into @numeros (numero, serie, empresa) values (@curr, @serie, @empresa)
                    end
                    end
                    select numero NumeroCTe, ese_numero SerieCTe, emp_cnpj CNPJEmpresa, emp_razao RazaoEmpresa, '" + numeracaoInicialFinal + @"' NumeracaoInicialFinal from @numeros n
                    join t_empresa e on e.emp_codigo = n.empresa
                    join t_empresa_serie s on s.ese_codigo = n.serie
                    order by numero";

            var query = this.SessionNHiBernate.CreateSQLQuery(sql);

            query.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Dominio.Relatorios.Embarcador.DataSource.CTe.CTes.FalhaNumeracaoCTe)));

            return query.SetTimeout(600).List<Dominio.Relatorios.Embarcador.DataSource.CTe.CTes.FalhaNumeracaoCTe>();
        }

        public IList<Dominio.Relatorios.Embarcador.DataSource.CTe.CTes.CTe> ConsultarRelatorioCTesMagazineLuiza(DateTime dataInicial, DateTime dataFinal)
        {
            var sql = @"select 
                        CTe.CON_NUM NumeroCTe,
                        SerieCTe.ESE_NUMERO SerieCTe, 
                        CTe.CON_DATAHORAEMISSAO DataEmissao,
                        StatusCTe = CASE CTe.CON_STATUS WHEN 'A' THEN 'Autorizado' WHEN 'C' THEN 'Cancelado' ELSE '' END,
                        RemetenteCTe.PCT_CPF_CNPJ CPFCNPJRemetente, 
                        RemetenteCTe.PCT_NOME Remetente, 
                        DestinatarioCTe.PCT_CPF_CNPJ CPFCNPJDestinatario, 
                        DestinatarioCTe.PCT_NOME Destinatario,
                        CPFCNPJTomador = CASE CTe.CON_PAGOAPAGAR WHEN 0 THEN RemetenteCTe.PCT_CPF_CNPJ WHEN 1 THEN DestinatarioCTe.PCT_CPF_CNPJ WHEN 2 THEN(select TomadorCTe.PCT_CPF_CNPJ from T_CTE_PARTICIPANTE TomadorCTe where TomadorCTe.PCT_CODIGO = CTe.CON_TOMADOR_CTE) ELSE '' END, 
                        Tomador = CASE CTe.CON_PAGOAPAGAR WHEN 0 THEN RemetenteCTe.PCT_NOME WHEN 1 THEN DestinatarioCTe.PCT_NOME WHEN 2 THEN(select TomadorCTe.PCT_NOME from T_CTE_PARTICIPANTE TomadorCTe where TomadorCTe.PCT_CODIGO = CTe.CON_TOMADOR_CTE) ELSE '' END,
                        InicioPrestacaoCTe.LOC_DESCRICAO InicioPrestacao,
                        InicioPrestacaoCTe.UF_SIGLA UFInicioPrestacao,  
                        FimPrestacaoCTe.LOC_DESCRICAO FimPrestacao,  
                        FimPrestacaoCTe.UF_SIGLA UFFimPrestacao,  
                        TransportadorCTe.EMP_RAZAO Transportador,
                        substring((select ', ' + notaFiscal1.NFC_NUMERO from T_CTE_DOCS notaFiscal1 where notaFiscal1.CON_CODIGO = CTe.CON_CODIGO for xml path('')), 3, 1000) NumeroNotaFiscal, 
                        substring((select ', ' + veiculo1.VEI_PLACA from T_CTE_VEICULO veiculoCTe1 inner join T_VEICULO veiculo1 on veiculoCTe1.VEI_CODIGO = veiculo1.VEI_CODIGO where veiculoCTe1.CON_CODIGO = CTe.CON_CODIGO for xml path('')), 3, 1000) Veiculo, 
                        CON_VALOR_TOTAL_MERC ValorMercadoria, 
                        CON_VALOR_RECEBER ValorReceber, 
                        CON_VALOR_FRETE ValorFrete, 
                        CON_VAL_ICMS ValorICMS, 
                        isnull((select cpt_valor from t_cte_comp_prest where con_codigo = CTe.con_codigo and cpt_nome = 'AD-VALOREM'), 0) ValorComponente1, --'Ad Valorem'
                        isnull((select isnull(cpt_valor, 0) from t_cte_comp_prest where con_codigo = CTe.con_codigo and cpt_nome = 'GRIS'), 0) ValorComponente2, --'GRIS'
                        isnull((select isnull(cpt_valor, 0) from t_cte_comp_prest where con_codigo = CTe.con_codigo and cpt_nome = 'PEDAGIO'), 0) ValorComponente3 --'Pedágio'
                        from 
                        T_CTE CTe 
                        left outer join T_EMPRESA TransportadorCTe on CTe.EMP_CODIGO = TransportadorCTe.EMP_CODIGO 
                        left outer join T_LOCALIDADES FimPrestacaoCTe on CTe.CON_LOCTERMINOPRESTACAO = FimPrestacaoCTe.LOC_CODIGO 
                        left outer join T_LOCALIDADES InicioPrestacaoCTe on CTe.CON_LOCINICIOPRESTACAO = InicioPrestacaoCTe.LOC_CODIGO 
                        left outer join T_CTE_PARTICIPANTE DestinatarioCTe on CTe.CON_DESTINATARIO_CTE = DestinatarioCTe.PCT_CODIGO 
                        left outer join T_CTE_PARTICIPANTE RemetenteCTe on CTe.CON_REMETENTE_CTE = RemetenteCTe.PCT_CODIGO 
                        left outer join T_EMPRESA_SERIE SerieCTe on CTe.CON_SERIE = SerieCTe.ESE_CODIGO 
                        left outer join T_CLIENTE ClienteRemetente on RemetenteCTe.PCT_CPF_CNPJ = ClienteRemetente.CLI_CGCCPF  
                        where 
                        CTe.CON_DATAHORAEMISSAO >= '" + dataInicial.ToString("yyyy-MM-dd") + @"' and 
                        CTe.CON_DATAHORAEMISSAO < '" + dataFinal.AddDays(1).ToString("yyyy-MM-dd") + @"' and 
                        CTe.CON_STATUS = 'A' and 
                        ClienteRemetente.GRP_CODIGO = 5 
                        order by CTe.CON_DATAHORAEMISSAO asc;";

            var query = this.SessionNHiBernate.CreateSQLQuery(sql);

            query.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Dominio.Relatorios.Embarcador.DataSource.CTe.CTes.CTe)));

            return query.SetTimeout(600).List<Dominio.Relatorios.Embarcador.DataSource.CTe.CTes.CTe>();
        }

        public int ContarPorEmpresa(int codigoEmpresa, DateTime dataInicial, DateTime dataFinal, Dominio.Enumeradores.TipoAmbiente tipoAmbiente, string[] status)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.ConhecimentoDeTransporteEletronico>();

            query = query.Where(o => o.Empresa.Codigo == codigoEmpresa && o.TipoAmbiente == tipoAmbiente && (o.ModeloDocumentoFiscal.Numero.Equals("57") || o.ModeloDocumentoFiscal.Numero.Equals("67")));

            if (dataInicial != DateTime.MinValue)
                query = query.Where(o => o.DataEmissao >= dataInicial.Date);

            if (dataFinal != DateTime.MinValue)
                query = query.Where(o => o.DataEmissao < dataFinal.AddDays(1).Date);

            if (status != null && status.Count() > 0)
                query = query.Where(o => status.Contains(o.Status));

            return query.Count();
        }

        public List<Dominio.Entidades.ConhecimentoDeTransporteEletronico> Consultar(int numeroInicial, int numeroFinal, int serie, string chave, int empresa, int codigoModeloDocumento, int codigoGrupoPessoas, decimal valorInicial, decimal valorFinal, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoLiquidacaoRelatorioDocumentoFaturamento? tipoLiquidacao, string propOrdenacao, string dirOrdenacao, int inicioRegistros, int maximoRegistros)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.ConhecimentoDeTransporteEletronico>();

            if (numeroInicial > 0)
                query = query.Where(obj => obj.Numero >= numeroInicial);

            if (numeroFinal > 0)
                query = query.Where(obj => obj.Numero <= numeroFinal);

            if (serie > 0)
                query = query.Where(obj => obj.Serie.Numero == serie);

            if (!string.IsNullOrWhiteSpace(chave))
                query = query.Where(obj => obj.Chave == chave);

            if (empresa > 0)
                query = query.Where(obj => obj.Empresa.Codigo == empresa);

            if (codigoModeloDocumento > 0)
                query = query.Where(o => o.ModeloDocumentoFiscal.Codigo == codigoModeloDocumento);

            if (valorInicial > 0m)
                query = query.Where(o => o.ValorAReceber >= valorInicial);

            if (valorFinal > 0m)
                query = query.Where(o => o.ValorAReceber <= valorFinal);

            if (codigoGrupoPessoas > 0)
                query = query.Where(o => o.TomadorPagador.GrupoPessoas.Codigo == codigoGrupoPessoas);

            if (tipoLiquidacao.HasValue)
            {
                if (tipoLiquidacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoLiquidacaoRelatorioDocumentoFaturamento.Liquidado)
                {
                    query = query.Where(o => o.DocumentosFaturamento.Any(dfa => dfa.ValorPago == (dfa.ValorDocumento + dfa.ValorAcrescimo - dfa.ValorDesconto)));
                }
                else if (tipoLiquidacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoLiquidacaoRelatorioDocumentoFaturamento.Pendente)
                {
                    query = query.Where(o => o.DocumentosFaturamento.Any(dfa => dfa.ValorPago != (dfa.ValorDocumento + dfa.ValorAcrescimo - dfa.ValorDesconto)));
                }
            }

            return query.Fetch(o => o.Serie)
                        .Fetch(o => o.ModeloDocumentoFiscal)
                        .Fetch(o => o.TomadorPagador)
                        .ThenFetch(o => o.GrupoPessoas)
                        .OrderBy(propOrdenacao + " " + dirOrdenacao)
                        .Skip(inicioRegistros)
                        .Take(maximoRegistros)
                        .ToList();
        }

        public int ContarConsulta(int numeroInicial, int numeroFinal, int serie, string chave, int empresa, int codigoModeloDocumento, int codigoGrupoPessoas, decimal valorInicial, decimal valorFinal, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoLiquidacaoRelatorioDocumentoFaturamento? tipoLiquidacao)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.ConhecimentoDeTransporteEletronico>();

            if (numeroInicial > 0)
                query = query.Where(obj => obj.Numero >= numeroInicial);

            if (numeroFinal > 0)
                query = query.Where(obj => obj.Numero <= numeroFinal);

            if (serie > 0)
                query = query.Where(obj => obj.Serie.Numero == serie);

            if (!string.IsNullOrWhiteSpace(chave))
                query = query.Where(obj => obj.Chave == chave);

            if (empresa > 0)
                query = query.Where(obj => obj.Empresa.Codigo == empresa);

            if (codigoModeloDocumento > 0)
                query = query.Where(o => o.ModeloDocumentoFiscal.Codigo == codigoModeloDocumento);

            if (valorInicial > 0m)
                query = query.Where(o => o.ValorAReceber >= valorInicial);

            if (valorFinal > 0m)
                query = query.Where(o => o.ValorAReceber <= valorFinal);

            if (codigoGrupoPessoas > 0)
                query = query.Where(o => o.TomadorPagador.GrupoPessoas.Codigo == codigoGrupoPessoas);

            if (tipoLiquidacao.HasValue)
            {
                if (tipoLiquidacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoLiquidacaoRelatorioDocumentoFaturamento.Liquidado)
                {
                    query = query.Where(o => o.DocumentosFaturamento.Any(dfa => dfa.ValorPago == (dfa.ValorDocumento + dfa.ValorAcrescimo - dfa.ValorDesconto)));
                }
                else if (tipoLiquidacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoLiquidacaoRelatorioDocumentoFaturamento.Pendente)
                {
                    query = query.Where(o => o.DocumentosFaturamento.Any(dfa => dfa.ValorPago != (dfa.ValorDocumento + dfa.ValorAcrescimo - dfa.ValorDesconto)));
                }
            }

            return query.Count();
        }

        public List<Dominio.Entidades.ConhecimentoDeTransporteEletronico> ConsultarNFSe(int numeroInicial, int numeroFinal, int serie, string protocolo, int empresa, DateTime dataInicial, DateTime dataFinal, int naturezaOperacao, string cnpjcpfPessoa, Dominio.Enumeradores.TipoAmbiente tipoAmbiente, string status, int localidadeTransportador, string propOrdenacao, string dirOrdenacao, int inicioRegistros, int maximoRegistros)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.ConhecimentoDeTransporteEletronico>();

            var result = from obj in query select obj;

            result = result.Where(obj => obj.ModeloDocumentoFiscal.Numero == "39");
            result = result.Where(obj => obj.TipoAmbiente == tipoAmbiente);

            if (numeroInicial > 0 && numeroFinal > 0)
                result = result.Where(obj => obj.Numero >= numeroInicial && obj.Numero <= numeroFinal);
            else if (numeroInicial > 0)
                result = result.Where(obj => obj.Numero == numeroInicial);
            else if (numeroFinal > 0)
                result = result.Where(obj => obj.Numero == numeroFinal);

            if (dataInicial > DateTime.MinValue && dataFinal > DateTime.MinValue)
                result = result.Where(obj => obj.DataEmissao.Value.Date >= dataInicial && obj.DataEmissao.Value.Date <= dataFinal);
            else if (dataInicial > DateTime.MinValue)
                result = result.Where(obj => obj.DataEmissao.Value.Date == dataInicial);
            else if (dataFinal > DateTime.MinValue)
                result = result.Where(obj => obj.DataEmissao.Value.Date == dataFinal);

            if (serie > 0)
                result = result.Where(obj => obj.Serie.Numero == serie);

            if (!string.IsNullOrWhiteSpace(protocolo))
                result = result.Where(obj => obj.Protocolo.Contains(protocolo));

            if (naturezaOperacao > 0)
                result = result.Where(obj => obj.NaturezaNFSe.Codigo == naturezaOperacao);

            if (!string.IsNullOrWhiteSpace(cnpjcpfPessoa) && cnpjcpfPessoa != "0")
                result = result.Where(obj => obj.Remetente.CPF_CNPJ == cnpjcpfPessoa);

            if (!string.IsNullOrWhiteSpace(status))
                result = result.Where(obj => obj.Status.Equals(status));

            if (empresa > 0)
                result = result.Where(obj => obj.Empresa.Codigo.Equals(empresa));

            if (localidadeTransportador > 0)
                result = result.Where(obj => obj.Empresa.Localidade.Codigo.Equals(localidadeTransportador));

            return result.OrderBy(propOrdenacao + (dirOrdenacao == "asc" ? " ascending" : " descending")).Skip(inicioRegistros).Take(maximoRegistros).ToList();
        }

        public int ContarConsultaNFSe(int numeroInicial, int numeroFinal, int serie, string protocolo, int empresa, DateTime dataInicial, DateTime dataFinal, int naturezaOperacao, string cnpjcpfPessoa, Dominio.Enumeradores.TipoAmbiente tipoAmbiente, string status, int localidadeTransportador)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.ConhecimentoDeTransporteEletronico>();

            var result = from obj in query select obj;

            result = result.Where(obj => obj.ModeloDocumentoFiscal.Numero == "39");
            result = result.Where(obj => obj.TipoAmbiente == tipoAmbiente);

            if (numeroInicial > 0 && numeroFinal > 0)
                result = result.Where(obj => obj.Numero >= numeroInicial && obj.Numero <= numeroFinal);
            else if (numeroInicial > 0)
                result = result.Where(obj => obj.Numero == numeroInicial);
            else if (numeroFinal > 0)
                result = result.Where(obj => obj.Numero == numeroFinal);

            if (dataInicial > DateTime.MinValue && dataFinal > DateTime.MinValue)
                result = result.Where(obj => obj.DataEmissao.Value.Date >= dataInicial && obj.DataEmissao.Value.Date <= dataFinal);
            else if (dataInicial > DateTime.MinValue)
                result = result.Where(obj => obj.DataEmissao.Value.Date == dataInicial);
            else if (dataFinal > DateTime.MinValue)
                result = result.Where(obj => obj.DataEmissao.Value.Date == dataFinal);

            if (serie > 0)
                result = result.Where(obj => obj.Serie.Numero == serie);

            if (!string.IsNullOrWhiteSpace(protocolo))
                result = result.Where(obj => obj.Protocolo.Contains(protocolo));

            if (naturezaOperacao > 0)
                result = result.Where(obj => obj.NaturezaNFSe.Codigo == naturezaOperacao);

            if (!string.IsNullOrWhiteSpace(cnpjcpfPessoa) && cnpjcpfPessoa != "0")
                result = result.Where(obj => obj.Remetente.CPF_CNPJ == cnpjcpfPessoa);

            if (!string.IsNullOrWhiteSpace(status))
                result = result.Where(obj => obj.Status.Equals(status));

            if (empresa > 0)
                result = result.Where(obj => obj.Empresa.Codigo.Equals(empresa));

            if (localidadeTransportador > 0)
                result = result.Where(obj => obj.Empresa.Localidade.Codigo.Equals(localidadeTransportador));

            return result.Count();
        }

        public int QuantidadeNotaServico(int codigoEmpresa, Dominio.Enumeradores.TipoAmbiente tipoAmbiente, DateTime dataConsulta)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.ConhecimentoDeTransporteEletronico>();

            var result = from obj in query select obj;

            result = result.Where(obj => obj.ModeloDocumentoFiscal.Numero == "39");
            result = result.Where(obj => obj.TipoAmbiente == tipoAmbiente);

            if (dataConsulta > DateTime.MinValue)
                result = result.Where(obj => obj.DataEmissao.Value.Month == dataConsulta.Month && obj.DataEmissao.Value.Year == dataConsulta.Year);

            if (codigoEmpresa > 0)
                result = result.Where(obj => obj.Empresa.Codigo.Equals(codigoEmpresa));

            return result.Count();
        }

        public IList<int> ConsultarCTeSemmCargaGPA(DateTime dataInicialEmissao, int codigoEmpresa)
        {
            var sql = @"select 
                        CTe.CON_CODIGO Codigo
                        from 
                        T_CTE CTe 
                        where 
                        CTe.CON_DATAHORAEMISSAO >= '" + dataInicialEmissao.ToString("yyyy-MM-dd") + @"' and 
                        CTe.CON_STATUS = 'A' and CTe.EMP_CODIGO = '" + codigoEmpresa.ToString() + @"' and
                        CTe.CON_SERIE = 9 and
                        CTe.CON_CHAVECTE NOT IN (select car_codigo_carga_embarcador from t_carga)
                        order by CTe.CON_CODIGO;";

            var query = this.SessionNHiBernate.CreateSQLQuery(sql);

            return query.List<int>();
        }

        public int RelatorioCobrancaQuantidadeCTes(int codigoEmpresa, DateTime dataInicial, DateTime dataFinal)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.ConhecimentoDeTransporteEletronico>();
            string[] status = new string[] { "A", "C" };
            var result = from obj in query
                         where
                         obj.Empresa.Codigo == codigoEmpresa &&
                         status.Contains(obj.Status)
#if DEBUG
#else
                         && obj.TipoAmbiente == Dominio.Enumeradores.TipoAmbiente.Producao
#endif
                         select obj;

            if (dataInicial != DateTime.MinValue)
                result = result.Where(o => o.DataEmissao >= dataInicial);

            if (dataFinal != DateTime.MinValue)
                result = result.Where(o => o.DataEmissao <= dataFinal);

            return result.Count();
        }

        public List<Dominio.Entidades.ConhecimentoDeTransporteEletronico> BuscarPorStatusEPeriodo(int codigoEmpresa, string[] status, DateTime dataInicial, DateTime dataFinal)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.ConhecimentoDeTransporteEletronico>();

            var result = from obj in query
                         where status.Contains(obj.Status) &&
                               obj.DataEmissao <= dataFinal.Date.AddDays(1) &&
                               obj.DataEmissao >= dataInicial.Date &&
                               (obj.ModeloDocumentoFiscal.Numero.Equals("57") || obj.ModeloDocumentoFiscal.Numero.Equals("67"))
                         select obj;

            if (codigoEmpresa > 0)
                result = result.Where(o => o.Empresa.Codigo == codigoEmpresa);

            return result.Timeout(120).ToList();
        }

        public List<int> BuscarCodigosPorStatusEPeriodo(int codigoEmpresa, string[] status, DateTime dataInicial, DateTime dataFinal)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.ConhecimentoDeTransporteEletronico>();

            var result = from obj in query
                         where status.Contains(obj.Status) &&
                               obj.DataEmissao <= dataFinal.Date.AddDays(1) &&
                               obj.DataEmissao >= dataInicial.Date &&
                               (obj.ModeloDocumentoFiscal.Numero.Equals("57") || obj.ModeloDocumentoFiscal.Numero.Equals("67"))
                         select obj;

            if (codigoEmpresa > 0)
                result = result.Where(o => o.Empresa.Codigo == codigoEmpresa);

            return result.Select(o => o.Codigo).ToList();
        }

        public List<Dominio.Entidades.ConhecimentoDeTransporteEletronico> BuscarCTesPorPeriodo(DateTime dataInicial, DateTime dataFinal)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.ConhecimentoDeTransporteEletronico>();

            var result = from obj in query
                         where obj.DataEmissao <= dataFinal.Date.AddDays(1) &&
                               obj.DataEmissao >= dataInicial.Date &&
                               (obj.ModeloDocumentoFiscal.Numero.Equals("57") || obj.ModeloDocumentoFiscal.Numero.Equals("67"))
                         select obj;


            return result.ToList();
        }

        public int ProximoNumeroSequencialCRT(string sigla, string licenca)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.ConhecimentoDeTransporteEletronico>()
                .Where(obj => obj.SiglaPaisOrigemCRT == sigla && obj.NumeroLicencaTNTICRT == licenca);

            return (query.Max(c => (int?)c.NumeroSequencialCRT) ?? 1) + 1;
        }

        public List<Dominio.Entidades.ConhecimentoDeTransporteEletronico> ConsultarCTesPorPeriodo(DateTime dataInicial, DateTime dataFinal, bool retornarTodos, int inicio, int limite, string status = "A")
        {
            var query = ConsultarCTesPorPeriodo(dataInicial, dataFinal, retornarTodos, status);

            query = query.WithOptions(o => o.SetTimeout(240));

            return ObterLista(query, "Codigo", "desc", inicio, limite);
        }

        public int ContarConsultaCTesPorPeriodo(DateTime dataInicial, DateTime dataFinal, bool retornarTodos, string status = "A")
        {
            var query = ConsultarCTesPorPeriodo(dataInicial, dataFinal, retornarTodos, status);

            return query.Count();
        }

        public IList<Tomador> ConsultarRelatorioTomadores(List<PropriedadeAgrupamento> agrupamentos, DateTime dataCadastroInicial, DateTime dataCadastroFinal, double cpfCnpjTomador, int codigoGrupoPessoas, string propAgrupa, string dirAgrupa, string propOrdena, string dirOrdena, int inicio, int limite)
        {
            var query = this.SessionNHiBernate.CreateSQLQuery(ObterSelectConsultaRelatorioTomadores(false, agrupamentos, dataCadastroInicial, dataCadastroFinal, cpfCnpjTomador, codigoGrupoPessoas, propAgrupa, dirAgrupa, propOrdena, dirOrdena, inicio, limite));

            query.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Tomador)));

            return query.SetTimeout(300).List<Tomador>();
        }

        public async Task<IList<Tomador>> ConsultarRelatorioTomadoresAsync(List<PropriedadeAgrupamento> agrupamentos, DateTime dataCadastroInicial, DateTime dataCadastroFinal, double cpfCnpjTomador, int codigoGrupoPessoas, string propAgrupa, string dirAgrupa, string propOrdena, string dirOrdena, int inicio, int limite)
        {
            var query = this.SessionNHiBernate.CreateSQLQuery(ObterSelectConsultaRelatorioTomadores(false, agrupamentos, dataCadastroInicial, dataCadastroFinal, cpfCnpjTomador, codigoGrupoPessoas, propAgrupa, dirAgrupa, propOrdena, dirOrdena, inicio, limite));

            query.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Tomador)));

            return await query.SetTimeout(300).ListAsync<Tomador>();
        }

        public int ContarConsultaRelatorioTomadores(List<PropriedadeAgrupamento> agrupamentos, DateTime dataCadastroInicial, DateTime dataCadastroFinal, double cpfCnpjTomador, int codigoGrupoPessoas, string propAgrupa, string dirAgrupa, string propOrdena, string dirOrdena, int inicio, int limite)
        {
            var query = this.SessionNHiBernate.CreateSQLQuery(ObterSelectConsultaRelatorioTomadores(true, agrupamentos, dataCadastroInicial, dataCadastroFinal, cpfCnpjTomador, codigoGrupoPessoas, propAgrupa, dirAgrupa, propOrdena, dirOrdena, inicio, limite));

            return query.SetTimeout(300).UniqueResult<int>();
        }

        public List<Dominio.Entidades.ConhecimentoDeTransporteEletronico> ConsultarConhecimentosImportados()
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.ConhecimentoDeTransporteEletronico>();

            var result = from obj in query where obj.MensagemRetornoSefaz == "CTe Processado (Importação)." select obj;

            return result.ToList();
        }

        public List<int> ConsultarConhecimentosNaoImportados()
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.ConhecimentoDeTransporteEletronico>();

            var result = from obj in query where obj.MensagemRetornoSefaz != "CTe Processado (Importação)." && obj.DataPreviaVencimento == null select obj;

            return result.Select(p => p.Codigo).ToList();
        }

        public List<Dominio.Entidades.ConhecimentoDeTransporteEletronico> ConsultarImportadosPortal(int codigoEmpresa, int numero, string cnpjEmitente, DateTime dataEmissao, int inicio, int limite)
        {
            var result = _ConsultarImportadosPortal(codigoEmpresa, numero, cnpjEmitente, dataEmissao);

            if (inicio > 0)
                result = result.Skip(inicio);

            if (limite > 0)
                result = result.Take(limite);

            return result.ToList();
        }

        public int ContarConsultaImportadosPortal(int codigoEmpresa, int numero, string cnpjEmitente, DateTime dataEmissao)
        {
            var result = _ConsultarImportadosPortal(codigoEmpresa, numero, cnpjEmitente, dataEmissao);

            return result.Count();
        }

        public IQueryable<Dominio.Entidades.ConhecimentoDeTransporteEletronico> _ConsultarImportadosPortal(int codigoEmpresa, int numero, string cnpjEmitente, DateTime dataEmissao)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.ConhecimentoDeTransporteEletronico>();

            var result = from obj in query
                         where obj.Empresa.Codigo == codigoEmpresa &&
                               obj.ModeloDocumentoFiscal.Numero.Equals("957")
                         select obj;

            if (numero > 0)
                result = result.Where(o => o.Numero == numero);

            if (!string.IsNullOrWhiteSpace(cnpjEmitente))
                result = result.Where(o => o.Intermediario.CPF_CNPJ.Contains(cnpjEmitente));

            if (dataEmissao != DateTime.MinValue)
                result = result.Where(o => o.DataEmissao.Value.Date == dataEmissao.Date);

            return result;
        }

        public List<Dominio.Entidades.ConhecimentoDeTransporteEletronico> BuscarCTesPorTempoLimiteEmissao(int minutosLimiteEmissao)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.ConhecimentoDeTransporteEletronico>();

            var result = from obj in query where obj.DataIntegracao < DateTime.Now.AddMinutes(-minutosLimiteEmissao) && obj.Status.Equals("E") select obj;

            return result.ToList();
        }

        public int ContarCTesPorTempoLimiteEmissao(int minutosLimiteEmissao)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.ConhecimentoDeTransporteEletronico>();

            var result = from obj in query where obj.DataIntegracao < DateTime.Now.AddMinutes(-minutosLimiteEmissao) && obj.Status.Equals("E") select obj;

            return result.Count();
        }

        public List<Dominio.Entidades.ConhecimentoDeTransporteEletronico> BuscarCTesPorTempoEmissaoFalhaSefaz(int minutosLimiteEmissao, List<int> codigosErros, int tentativasReenvio)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.ConhecimentoDeTransporteEletronico>();
            var result = from obj in query
                         where
                             //(obj.Serie.Numero == 5 || obj.Serie.Numero == 6) &&
                             obj.DataEmissao >= DateTime.Now.AddDays(-5) &&
                             obj.DataIntegracao < DateTime.Now.AddMinutes(-minutosLimiteEmissao) &&
                             obj.Status.Equals("R") && obj.TentativaReenvio <= tentativasReenvio &&
                             (codigosErros.Contains(obj.MensagemStatus.CodigoDoErro) ||
                              obj.MensagemRetornoSefaz.Contains("HTTP: 0") ||
                              obj.MensagemRetornoSefaz.Contains("HTTP: 4") ||
                              obj.MensagemRetornoSefaz.Contains("HTTP: 503") ||
                              obj.MensagemRetornoSefaz.Contains("INDEVIDO") ||
                              //obj.MensagemRetornoSefaz.Contains("sefaz") || estava pegando rejeições qua não pode reenviar
                              obj.MensagemRetornoSefaz.Contains("lote em") ||
                              obj.MensagemRetornoSefaz.Contains("não consta") ||
                              obj.MensagemRetornoSefaz.Contains("nao consta") ||
                              obj.MensagemRetornoSefaz.Contains("nao catalogado") ||
                              obj.MensagemRetornoSefaz.Contains("Falha ao conectar") ||
                              obj.MensagemRetornoSefaz.Contains("888 -") ||
                              obj.MensagemRetornoSefaz.Contains("Status: 0 - Motivo:") ||
                              obj.MensagemRetornoSefaz.Contains("paralisado"))

                         select obj;

            return result.ToList();
        }

        public List<Dominio.Entidades.ConhecimentoDeTransporteEletronico> BuscarCTesEPEC(int tentativasReenvio)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.ConhecimentoDeTransporteEletronico>();
            var result = from obj in query
                         where
                             obj.DataEmissao <= DateTime.Now.AddDays(-1) &&
                             obj.Status.Equals("Q") &&
                             obj.TentativaReenvio <= tentativasReenvio

                         select obj;

            return result.ToList();
        }

        public List<int> BuscarCTesParaGeracaoTitulosAutomatico()
        {
            var query = SessionNHiBernate.Query<Dominio.Entidades.ConhecimentoDeTransporteEletronico>();
            var result = query.Where(obj => obj.Status == "A" && obj.ModeloDocumentoFiscal.Numero == "57" && obj.Remetente != null);// && obj.TipoTomador != Dominio.Enumeradores.TipoTomador.Remetente);

            var queryTitulo = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.Titulo>();
            var resultTitulo = queryTitulo.Where(obj => obj.ConhecimentoDeTransporteEletronico != null);

            result = result.Where(obj => !resultTitulo.Select(p => p.ConhecimentoDeTransporteEletronico.Codigo).Contains(obj.Codigo));

            return result.Select(p => p.Codigo).ToList();
        }

        public List<Dominio.Entidades.Embarcador.Financeiro.Titulo> BuscarTitulosDeCTesParaCancelamento()
        {
            var query = SessionNHiBernate.Query<Dominio.Entidades.ConhecimentoDeTransporteEletronico>();
            var result = query.Where(obj => obj.Status == "C" && obj.ModeloDocumentoFiscal.Numero == "57" && obj.Remetente != null);// && obj.TipoTomador != Dominio.Enumeradores.TipoTomador.Remetente);

            var queryTitulo = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.Titulo>();
            var resultTitulo = queryTitulo.Where(obj => obj.ConhecimentoDeTransporteEletronico != null && obj.StatusTitulo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.StatusTitulo.EmAberto);

            resultTitulo = resultTitulo.Where(obj => result.Select(p => p.Codigo).Contains(obj.ConhecimentoDeTransporteEletronico.Codigo));

            return resultTitulo.ToList();
        }

        public IList<Dominio.Relatorios.Embarcador.DataSource.CTe.RelatorioPreDACTE> RelatorioPreDACTE(int codigoCTe)
        {
            string query = @"   SELECT C.CON_NUM Numero, 
                                ES.ESE_NUMERO Serie,

                                E.EMP_CNPJ CNPJEmpresa, 
                                E.EMP_INSCRICAO IEEmpresa, 
                                E.EMP_RAZAO RazaoEmpresa, 
                                E.EMP_FONE FoneEmpresa, 
                                E.EMP_CEP CEPEmpresa, 
                                E.EMP_ENDERECO EnderecoEmpresa, 
                                E.EMP_NUMERO NumeroEmpresa, 
                                E.EMP_COMPLEMENTO ComplementoEmpresa, 
                                E.EMP_BAIRRO BairroEmpresa, 
                                L.LOC_DESCRICAO CidadeEmpresa, 
                                L.UF_SIGLA EstadoEmpresa,

                                C.CON_DATAHORAEMISSAO DataEmissao, 
                                CASE
	                                WHEN C.CON_TIPO_CTE = 0 THEN 'Normal' 
	                                WHEN C.CON_TIPO_CTE = 1 THEN 'Complemento' 
	                                WHEN C.CON_TIPO_CTE = 2 THEN 'Anulação' 
	                                WHEN C.CON_TIPO_CTE = 3 THEN 'Substituto' 
	                                ELSE ''
                                END TipoCTE,
                                CASE
	                                WHEN C.CON_TIPO_SERVICO = 0 THEN 'Normal' 
	                                WHEN C.CON_TIPO_SERVICO = 1 THEN 'Subcontratação' 
	                                WHEN C.CON_TIPO_SERVICO = 2 THEN 'Redespacho' 
	                                WHEN C.CON_TIPO_SERVICO = 3 THEN 'Redespacho Intermediário' 
	                                ELSE ''
                                END TipoServico,
                                CASE
	                                WHEN C.CON_TOMADOR = 0 THEN 'Remetente' 
	                                WHEN C.CON_TOMADOR = 1 THEN 'Expedidor' 
	                                WHEN C.CON_TOMADOR = 2 THEN 'Recebedor' 
	                                WHEN C.CON_TOMADOR = 3 THEN 'Destinatário' 
	                                WHEN C.CON_TOMADOR = 4 THEN 'Outros' 
	                                WHEN C.CON_TOMADOR = 5 THEN 'Intermediário' 
	                                ELSE ''
                                END Tomador,
                                C.CON_TOMADOR TipoTomador,
                                CASE
	                                WHEN C.CON_IND_GLOBALIZADO = 1 THEN 'Sim'
	                                ELSE 'Não'
                                END IndicadorGlobalizado,
                                CFOP.CFO_CFOP CFOP, 
                                CFOP.CFO_DESCRICAO DescricaoCFOP,
                                LIP.LOC_DESCRICAO CidadeInicioPrestacao, 
                                LIP.UF_SIGLA EstadoInicioPrestacao,
                                LTP.LOC_DESCRICAO CidadeTerminoPrestacao, 
                                LTP.UF_SIGLA EstadoTerminoPrestacao,

                                CPR.PCT_CPF_CNPJ CNPJRemetente, 
                                CPR.PCT_IERG IERemetente, 
                                CPR.PCT_NOME RazaoRemetente, 
                                CPR.PCT_FONE FoneRemetente, 
                                CPR.PCT_CEP CEPRemetente, 
                                CPR.PCT_ENDERECO EnderecoRemetente, 
                                CPR.PCT_NUMERO NumeroRemetente, 
                                CPR.PCT_COMPLEMENTO ComplementoRemetente, 
                                CPR.PCT_BAIRRO BairroRemetente, 
                                LCPR.LOC_DESCRICAO CidadeRemetente, 
                                LCPR.UF_SIGLA EstadoRemetente,

                                CPD.PCT_CPF_CNPJ CNPJDestinatario, 
                                CPD.PCT_IERG IEDestinatario, 
                                CPD.PCT_NOME RazaoDestinatario, 
                                CPD.PCT_FONE FoneDestinatario, 
                                CPD.PCT_CEP CEPDestinatario, 
                                CPD.PCT_ENDERECO EnderecoDestinatario, 
                                CPD.PCT_NUMERO NumeroDestinatario, 
                                CPD.PCT_COMPLEMENTO ComplementoDestinatario, 
                                CPD.PCT_BAIRRO BairroDestinatario, 
                                LCPD.LOC_DESCRICAO CidadeDestinatario, 
                                LCPD.UF_SIGLA EstadoDestinatario,

                                CPE.PCT_CPF_CNPJ CNPJExpedidor, 
                                CPE.PCT_IERG IEExpedidor, 
                                CPE.PCT_NOME RazaoExpedidor, 
                                CPE.PCT_FONE FoneExpedidor, 
                                CPE.PCT_CEP CEPExpedidor, 
                                CPE.PCT_ENDERECO EnderecoExpedidor, 
                                CPE.PCT_NUMERO NumeroExpedidor, 
                                CPE.PCT_COMPLEMENTO ComplementoExpedidor, 
                                CPE.PCT_BAIRRO BairroExpedidor, 
                                LCPE.LOC_DESCRICAO CidadeExpedidor, 
                                LCPE.UF_SIGLA EstadoExpedidor,

                                CPRC.PCT_CPF_CNPJ CNPJRecebedor, 
                                CPRC.PCT_IERG IERecebedor, 
                                CPRC.PCT_NOME RazaoRecebedor, 
                                CPRC.PCT_FONE FoneRecebedor, 
                                CPRC.PCT_CEP CEPRecebedor, 
                                CPRC.PCT_ENDERECO EnderecoRecebedor, 
                                CPRC.PCT_NUMERO NumeroRecebedor, 
                                CPRC.PCT_COMPLEMENTO ComplementoRecebedor, 
                                CPRC.PCT_BAIRRO BairroRecebedor, 
                                LCPRC.LOC_DESCRICAO CidadeRecebedor, 
                                LCPRC.UF_SIGLA EstadoRecebedor,

                                CON_PRODUTO_PRED ProdutoPredominante,
                                CON_OUTRAS_CARAC_CARGA OutrasCaracteristicasDaCarga,
                                CON_VALOR_TOTAL_MERC ValorTotalMercadoria,
                                CON_VALOR_FRETE ValorFrete,
                                CON_VALOR_PREST_SERVICO ValorPrestacaoServico,
                                CON_VALOR_RECEBER ValorAReceber,
                                CON_CST CST,
                                CON_BC_ICMS BaseCalculoICMS,
                                CON_ALIQ_ICMS AliquotaICMS,
                                CON_VAL_ICMS ValorICMS,
                                CON_OBSGERAIS ObservacoesGerais,
                                CON_RNTRC RNTRC,

                                NFC_CHAVENFE ChaveNFE,
                                NFC_VALOR ValorNFE,
                                NFC_NUMERO NumeroNFE,
                                NFC_DESCRICAO DescricaoNFE,
                                MOD.MOD_DESCRICAO ModeloNFE,

                                SUBSTRING((SELECT DISTINCT ', ' + CAST(OBC_DESCRICAO AS NVARCHAR(160))
                                FROM T_OBS_CONTRIBUINTE OBC
                                WHERE OBC.CON_CODIGO = C.CON_CODIGO FOR XML PATH('')), 3, 1000) AS ObservacoesContribuinte,
                                SUBSTRING((SELECT DISTINCT ', ' + CAST(OBF_DESCRICAO AS NVARCHAR(60))
                                FROM T_OBS_FISCO OBF
                                WHERE OBF.CON_CODIGO = C.CON_CODIGO FOR XML PATH('')), 3, 1000) AS ObservacoesFisco

                                FROM T_CTE C
                                JOIN T_EMPRESA_SERIE ES ON ES.ESE_CODIGO = C.CON_SERIE
                                JOIN T_EMPRESA E ON E.EMP_CODIGO = C.EMP_CODIGO
                                JOIN T_LOCALIDADES L ON L.LOC_CODIGO = E.LOC_CODIGO
                                JOIN T_CFOP CFOP ON CFOP.CFO_CODIGO = C.CFO_CODIGO
                                JOIN T_LOCALIDADES LIP ON LIP.LOC_CODIGO = C.CON_LOCINICIOPRESTACAO
                                JOIN T_LOCALIDADES LTP ON LTP.LOC_CODIGO = C.CON_LOCTERMINOPRESTACAO
                                JOIN T_CTE_PARTICIPANTE CPR ON CPR.PCT_CODIGO = C.CON_REMETENTE_CTE
                                JOIN T_LOCALIDADES LCPR ON LCPR.LOC_CODIGO = CPR.LOC_CODIGO
                                JOIN T_CTE_PARTICIPANTE CPD ON CPD.PCT_CODIGO = C.CON_DESTINATARIO_CTE
                                JOIN T_LOCALIDADES LCPD ON LCPD.LOC_CODIGO = CPD.LOC_CODIGO
                                LEFT OUTER JOIN T_CTE_PARTICIPANTE CPE ON CPE.PCT_CODIGO = C.CON_EXPEDIDOR_CTE
                                LEFT OUTER JOIN T_LOCALIDADES LCPE ON LCPE.LOC_CODIGO = CPE.LOC_CODIGO
                                LEFT OUTER JOIN T_CTE_PARTICIPANTE CPRC ON CPRC.PCT_CODIGO = C.CON_RECEBEDOR_CTE
                                LEFT OUTER JOIN T_LOCALIDADES LCPRC ON LCPRC.LOC_CODIGO = CPRC.LOC_CODIGO
                                JOIN T_CTE_DOCS CDOC ON CDOC.CON_CODIGO = C.CON_CODIGO
                                JOIN T_MODDOCFISCAL MOD ON MOD.MOD_CODIGO = CDOC.MOD_CODIGO
                                WHERE 1 = 1";

            if (codigoCTe > 0)
                query += " AND C.CON_CODIGO = " + codigoCTe.ToString();

            var nhQuery = this.SessionNHiBernate.CreateSQLQuery(query);

            nhQuery.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Dominio.Relatorios.Embarcador.DataSource.CTe.RelatorioPreDACTE)));

            return nhQuery.List<Dominio.Relatorios.Embarcador.DataSource.CTe.RelatorioPreDACTE>();
        }

        public IList<Dominio.Relatorios.Embarcador.DataSource.CTe.RelatorioPreDACTECarga> RelatorioPreDACTECarga(int codigoCTe)
        {
            string query = @" SELECT 
                CASE
	                WHEN ICA_UN = '00' THEN 'M3'
	                WHEN ICA_UN = '01' THEN 'KG'
	                WHEN ICA_UN = '02' THEN 'TON'
	                WHEN ICA_UN = '03' THEN 'UN'
	                WHEN ICA_UN = '04' THEN 'LT'
	                WHEN ICA_UN = '05' THEN 'MMBTU'
	                WHEN ICA_UN = '99' THEN 'M3_ST'
	                ELSE ''
                END UnidadeMedida,
                ICA_QTD Quantidade
                FROM T_CTE_INF_CARGA  ";

            if (codigoCTe > 0)
                query += " WHERE CON_CODIGO = " + codigoCTe.ToString();

            var nhQuery = this.SessionNHiBernate.CreateSQLQuery(query);

            nhQuery.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Dominio.Relatorios.Embarcador.DataSource.CTe.RelatorioPreDACTECarga)));

            return nhQuery.List<Dominio.Relatorios.Embarcador.DataSource.CTe.RelatorioPreDACTECarga>();
        }

        public IList<Dominio.Relatorios.Embarcador.DataSource.CTe.RelatorioPreDACTEComponente> RelatorioPreDACTEComponente(int codigoCTe)
        {
            string query = @" SELECT CPT_NOME Descricao, CPT_VALOR Valor FROM T_CTE_COMP_PREST ";

            if (codigoCTe > 0)
                query += " WHERE CON_CODIGO = " + codigoCTe.ToString();

            var nhQuery = this.SessionNHiBernate.CreateSQLQuery(query);

            nhQuery.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Dominio.Relatorios.Embarcador.DataSource.CTe.RelatorioPreDACTEComponente)));

            return nhQuery.List<Dominio.Relatorios.Embarcador.DataSource.CTe.RelatorioPreDACTEComponente>();
        }

        public List<Dominio.Entidades.ConhecimentoDeTransporteEletronico> BuscarPorEmpresaRemetenteData(int codigoEmpresa, string cnpjRemetente, DateTime dataInicio, DateTime dataFim)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.ConhecimentoDeTransporteEletronico>();
            var result = from obj in query where obj.Status == "A" select obj;

            if (codigoEmpresa > 0)
                result = result.Where(o => o.Empresa.Codigo == codigoEmpresa);

            if (string.IsNullOrWhiteSpace(cnpjRemetente))
                result = result.Where(o => o.Remetente.CPF_CNPJ.Equals(cnpjRemetente));

            if (dataInicio > DateTime.MinValue && dataFim > DateTime.MinValue)
                result = result.Where(o => o.DataEmissao >= dataInicio && o.DataEmissao <= dataFim);

            return result.ToList();
        }

        public List<Dominio.Entidades.ConhecimentoDeTransporteEletronico> BuscarPorDocumentoAgregado(bool utilizarNovoLayoutPagamentoAgregado, int codigoVeiculo, DateTime? dataInicialOcorrencia, DateTime? dataFinalOcorrencia, bool consultaSomentePendentes, int codigoTipoOcorrencia, int numero, DateTime dataEmissao, DateTime dataFinal, int codigoMotorista, string cnpjAgregado, int codigoPagamentoAgregado, double cnpjDestinatario, string propOrdenacao, string dirOrdenacao, int inicioRegistros, int maximoRegistros)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.ConhecimentoDeTransporteEletronico>();
            query = query.Where(c => c.Status == "A");

            if (utilizarNovoLayoutPagamentoAgregado)
                query = query.Where(obj => obj.TipoCTE == Dominio.Enumeradores.TipoCTE.Normal);

            if (numero > 0)
                query = query.Where(obj => obj.Numero == numero);

            if (codigoMotorista > 0)
                query = query.Where(obj => obj.CargaCTes.Any(c => c.Carga.Motoristas.Any(p => p.Codigo == codigoMotorista)));

            if (codigoVeiculo > 0)
                query = query.Where(obj => obj.Veiculos.Any(c => c.Veiculo.Codigo == codigoVeiculo));

            if (!string.IsNullOrWhiteSpace(cnpjAgregado))
                query = query.Where(obj => obj.CargaCTes.Any(c => c.Carga.Terceiro.CPF_CNPJ == double.Parse(cnpjAgregado)));

            if (dataEmissao != DateTime.MinValue)
                query = query.Where(obj => obj.DataEmissao.Value.Date >= dataEmissao.Date);

            if (dataFinal != DateTime.MinValue)
                query = query.Where(obj => obj.DataEmissao.Value.Date <= dataFinal.Date);

            if (cnpjDestinatario > 0)
                query = query.Where(obj => obj.Destinatario.Cliente.CPF_CNPJ == cnpjDestinatario);

            var queryPagamento = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.PagamentoAgregado.PagamentoAgregadoDocumento>();

            if (codigoPagamentoAgregado > 0)
            {
                if (consultaSomentePendentes)
                    queryPagamento = queryPagamento.Where(obj => obj.PagamentoAgregado.Situacao != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoPagamentoAgregado.Rejeitada && obj.PagamentoAgregado.Situacao != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoPagamentoAgregado.Cancelado);
                else
                    queryPagamento = queryPagamento.Where(obj => obj.PagamentoAgregado.Codigo != codigoPagamentoAgregado && obj.PagamentoAgregado.Situacao != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoPagamentoAgregado.Rejeitada && obj.PagamentoAgregado.Situacao != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoPagamentoAgregado.Cancelado);
            }

            if (queryPagamento.Count() > 0)
                query = query.Where(obj => !queryPagamento.Select(o => o.ConhecimentoDeTransporteEletronico).Contains(obj));

            if (codigoTipoOcorrencia > 0 || dataInicialOcorrencia.HasValue || dataFinalOcorrencia.HasValue)
            {
                var queryTipoOcorrencia = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrenciaDocumento>();

                if (codigoTipoOcorrencia > 0)
                    queryTipoOcorrencia = queryTipoOcorrencia.Where(obj => obj.CargaOcorrencia.TipoOcorrencia.Codigo == codigoTipoOcorrencia);

                if (dataInicialOcorrencia.HasValue)
                    queryTipoOcorrencia = queryTipoOcorrencia.Where(obj => obj.CargaOcorrencia.DataOcorrencia.Date >= dataInicialOcorrencia.Value.Date);

                if (dataFinalOcorrencia.HasValue)
                    queryTipoOcorrencia = queryTipoOcorrencia.Where(obj => obj.CargaOcorrencia.DataOcorrencia.Date <= dataFinalOcorrencia.Value.Date);

                if (dataInicialOcorrencia.HasValue || dataFinalOcorrencia.HasValue)
                    queryTipoOcorrencia = queryTipoOcorrencia.Where(obj => obj.CargaOcorrencia.TipoOcorrencia.Tipo == "F");

                if (queryTipoOcorrencia.Count() > 0)
                    query = query.Where(obj => queryTipoOcorrencia.Select(o => o.CargaCTe.CTe).Contains(obj));
            }

            if (maximoRegistros > 0)
                return query.OrderBy(propOrdenacao + (dirOrdenacao == "asc" ? " ascending" : " descending")).Skip(inicioRegistros).Take(maximoRegistros).Distinct().ToList();
            else
                return query.Distinct().ToList();
        }

        public int ContarBuscarPorDocumentoAgregado(int codigoVeiculo, DateTime? dataInicialOcorrencia, DateTime? dataFinalOcorrencia, bool consultaSomentePendentes, int codigoTipoOcorrencia, int numero, DateTime dataEmissao, DateTime dataFinal, int codigoMotorista, string cnpjAgregado, int codigoPagamentoAgregado, double cnpjDestinatario)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.ConhecimentoDeTransporteEletronico>();
            query = query.Where(c => c.Status == "A");

            if (numero > 0)
                query = query.Where(obj => obj.Numero == numero);

            if (codigoMotorista > 0)
                query = query.Where(obj => obj.CargaCTes.Any(c => c.Carga.Motoristas.Any(p => p.Codigo == codigoMotorista)));

            if (codigoVeiculo > 0)
                query = query.Where(obj => obj.Veiculos.Any(c => c.Veiculo.Codigo == codigoVeiculo));

            if (!string.IsNullOrWhiteSpace(cnpjAgregado))
                query = query.Where(obj => obj.CargaCTes.Any(c => c.Carga.Terceiro.CPF_CNPJ == double.Parse(cnpjAgregado)));

            if (dataEmissao != DateTime.MinValue)
                query = query.Where(obj => obj.DataEmissao.Value.Date >= dataEmissao.Date);

            if (dataFinal != DateTime.MinValue)
                query = query.Where(obj => obj.DataEmissao.Value.Date <= dataFinal.Date);

            if (cnpjDestinatario > 0)
                query = query.Where(obj => obj.Destinatario.Cliente.CPF_CNPJ == cnpjDestinatario);

            var queryPagamento = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.PagamentoAgregado.PagamentoAgregadoDocumento>();

            if (codigoPagamentoAgregado > 0)
            {
                if (consultaSomentePendentes)
                    queryPagamento = queryPagamento.Where(obj => obj.PagamentoAgregado.Situacao != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoPagamentoAgregado.Rejeitada && obj.PagamentoAgregado.Situacao != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoPagamentoAgregado.Cancelado);
                else
                    queryPagamento = queryPagamento.Where(obj => obj.PagamentoAgregado.Codigo != codigoPagamentoAgregado && obj.PagamentoAgregado.Situacao != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoPagamentoAgregado.Rejeitada && obj.PagamentoAgregado.Situacao != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoPagamentoAgregado.Cancelado);
            }

            if (queryPagamento.Count() > 0)
                query = query.Where(obj => !queryPagamento.Select(o => o.ConhecimentoDeTransporteEletronico).Contains(obj));

            if (codigoTipoOcorrencia > 0 || dataInicialOcorrencia.HasValue || dataFinalOcorrencia.HasValue)
            {
                var queryTipoOcorrencia = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrenciaDocumento>();

                if (codigoTipoOcorrencia > 0)
                    queryTipoOcorrencia = queryTipoOcorrencia.Where(obj => obj.CargaOcorrencia.TipoOcorrencia.Codigo == codigoTipoOcorrencia);

                if (dataInicialOcorrencia.HasValue)
                    queryTipoOcorrencia = queryTipoOcorrencia.Where(obj => obj.CargaOcorrencia.DataOcorrencia.Date >= dataInicialOcorrencia.Value.Date);

                if (dataFinalOcorrencia.HasValue)
                    queryTipoOcorrencia = queryTipoOcorrencia.Where(obj => obj.CargaOcorrencia.DataOcorrencia.Date <= dataFinalOcorrencia.Value.Date);

                if (dataInicialOcorrencia.HasValue || dataFinalOcorrencia.HasValue)
                    queryTipoOcorrencia = queryTipoOcorrencia.Where(obj => obj.CargaOcorrencia.TipoOcorrencia.Tipo == "F");

                if (queryTipoOcorrencia.Count() > 0)
                    query = query.Where(obj => queryTipoOcorrencia.Select(o => o.CargaCTe.CTe).Contains(obj));
            }

            return query.Count();
        }

        public void RemoveTitulo(int cte)
        {
            string hql = "update ConhecimentoDeTransporteEletronico cte set cte.Titulo = null where cte.Codigo = :Conhecimento ";
            var query = this.SessionNHiBernate.CreateQuery(hql);

            query.SetInt32("Conhecimento", cte);

            query.ExecuteUpdate();
        }

        public void SetaTitulo(int cte, int titulo)
        {
            string hql = "update ConhecimentoDeTransporteEletronico cte set cte.Titulo = :Titulo where cte.Codigo = :Conhecimento ";
            var query = this.SessionNHiBernate.CreateQuery(hql);

            query.SetInt32("Titulo", titulo);
            query.SetInt32("Conhecimento", cte);

            query.ExecuteUpdate();
        }

        public int ContarCTesCanceladosAguardandoConsulta(bool retornarCTeIntulizadoNoFluxoCancelamento, int codigoEmpresa)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.ConhecimentoDeTransporteEletronico>();
            if (retornarCTeIntulizadoNoFluxoCancelamento)
                query = query.Where(o => o.Status == "C" || o.Status == "I");
            else
                query = query.Where(o => o.Status == "C");

            if (codigoEmpresa > 0)
                query = query.Where(c => c.Empresa.Codigo == codigoEmpresa);

            var queryCTesPendentes = this.SessionNHiBernate.Query<Dominio.Entidades.RetornoXMLCTe>();
            query = query.Where(o => !(from obj in queryCTesPendentes where obj.CTe.Status == obj.Status select obj.CTe.Codigo).Contains(o.Codigo));

            return query.Distinct().Count();
        }

        public List<int> BuscarListaCodigosCTesCanceladosAguardandoConsulta(bool retornarCTeIntulizadoNoFluxoCancelamento, int codigoEmpresa, int inicio, int limite)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.ConhecimentoDeTransporteEletronico>();
            if (retornarCTeIntulizadoNoFluxoCancelamento)
                query = query.Where(o => o.Status == "C" || o.Status == "I");
            else
                query = query.Where(o => o.Status == "C");

            if (codigoEmpresa > 0)
                query = query.Where(c => c.Empresa.Codigo == codigoEmpresa);

            var queryCTesPendentes = this.SessionNHiBernate.Query<Dominio.Entidades.RetornoXMLCTe>();
            query = query.Where(o => !(from obj in queryCTesPendentes where obj.CTe.Status == obj.Status select obj.CTe.Codigo).Contains(o.Codigo));

            query = query.OrderBy(o => o.Codigo).Skip(inicio).Take(limite);

            return query.Select(o => o.Codigo).Timeout(120).Distinct().ToList();
        }

        public List<Dominio.Entidades.ConhecimentoDeTransporteEletronico> BuscarCTesReferencia(int codigoEmpresa, int inicioRegistros, int maximoRegistros)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.ConhecimentoDeTransporteEletronico>();

            var result = from obj in query where obj.Empresa.Codigo == codigoEmpresa && obj.ReferenciaEmissao select obj;

            return result.Skip(inicioRegistros).Take(maximoRegistros).ToList();
        }

        public List<Dominio.Entidades.ConhecimentoDeTransporteEletronico> BuscarCTePorFaturasDocumento(List<int> codigosFaturaDocumento)
        {
            var queryFaturaDocumento = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Fatura.FaturaDocumento>()
                .Where(o => codigosFaturaDocumento.Contains(o.Codigo))
                .Select(o => o.Documento.CTe);

            return queryFaturaDocumento.ToList();
        }

        public List<Dominio.Entidades.ConhecimentoDeTransporteEletronico> BuscarCTePorFatura(int codigosFatura)
        {
            var queryFaturaDocumento = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Fatura.FaturaDocumento>()
                .Where(o => o.Fatura.Codigo == codigosFatura)
                .Select(o => o.Documento.CTe);

            return queryFaturaDocumento.ToList();
        }

        public int ContarCTesReferencia(int codigoEmpresa)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.ConhecimentoDeTransporteEletronico>();

            var result = from obj in query where obj.Empresa.Codigo == codigoEmpresa && obj.ReferenciaEmissao select obj;

            return result.Count();
        }

        public IList<Dominio.ObjetosDeValor.Embarcador.Pedido.Container.CTeAnteriorBookingContainer> BuscarCTeAnteriorBookingContainerPendenteVinculo()
        {
            string sql = @"SELECT CTe.CON_CODIGO Codigo, CTe.CON_NUMERO_BOOKING_OBSERVACAO NumeroBooking, CTe.CON_NUMERO_CONTAINER_OBSERVACAO NumeroContainer,
                    Remetente.CLI_CODIGO CodigoRemetente, Tomador.CLI_CODIGO CodigoTomador
                    FROM T_CTE CTe
                    JOIN T_CTE_PARTICIPANTE Remetente on Remetente.PCT_CODIGO = CTe.CON_REMETENTE_CTE
                    JOIN T_CTE_PARTICIPANTE Tomador on Tomador.PCT_CODIGO = CTe.CON_TOMADOR_PAGADOR_CTE
                    WHERE CTe.CON_NUMERO_BOOKING_OBSERVACAO <> '' AND CTe.CON_NUMERO_BOOKING_OBSERVACAO IS NOT NULL
                    AND CTe.CON_NUMERO_CONTAINER_OBSERVACAO <> '' AND CTe.CON_NUMERO_CONTAINER_OBSERVACAO IS NOT NULL
                    --AND CTe.CON_CTE_IMPORTADO_EMBARCADOR = 1
                    AND NOT exists (SELECT Carga.CON_CODIGO FROM T_CARGA_PEDIDO_DOCUMENTO_CTE Carga where Carga.CON_CODIGO = CTe.CON_CODIGO)";
            var query = this.SessionNHiBernate.CreateSQLQuery(sql);

            query.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Dominio.ObjetosDeValor.Embarcador.Pedido.Container.CTeAnteriorBookingContainer)));

            return query.SetTimeout(6000).List<Dominio.ObjetosDeValor.Embarcador.Pedido.Container.CTeAnteriorBookingContainer>();
        }
        public Dominio.Entidades.ConhecimentoDeTransporteEletronico BuscarReferenciaPorClienteOrigemDestino(int codigoEmpresa, string cnpjRemetente, string UFOrigem, string UFDestino, Dominio.Enumeradores.TipoServico tipoServico)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.ConhecimentoDeTransporteEletronico>();

            var result = from obj in query
                         where
                                obj.Empresa.Codigo == codigoEmpresa &&
                                obj.ReferenciaEmissao &&
                                obj.Remetente.CPF_CNPJ == cnpjRemetente &&
                                obj.LocalidadeInicioPrestacao.Estado.Sigla == UFOrigem &&
                                obj.LocalidadeTerminoPrestacao.Estado.Sigla == UFDestino &&
                                obj.TipoServico == tipoServico
                         select obj;

            return result.FirstOrDefault();
        }

        public Dominio.Entidades.ConhecimentoDeTransporteEletronico BuscarReferenciaSintravir(int codigoEmpresa, string cnpjRemetente, string cnpjDestinatario, string cnpjExpedidor, string cnpjRecebedor, int codigoOrigem, int codigoDestino, Dominio.Enumeradores.TipoServico tipoServico)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.ConhecimentoDeTransporteEletronico>();

            var result = from obj in query
                         where
                                obj.Empresa.Codigo == codigoEmpresa &&
                                obj.ReferenciaEmissao &&
                                obj.Remetente.CPF_CNPJ == cnpjRemetente &&
                                obj.LocalidadeInicioPrestacao.Codigo == codigoOrigem &&
                                obj.LocalidadeTerminoPrestacao.Codigo == codigoDestino &&
                                obj.TipoServico == tipoServico
                         select obj;

            if (!string.IsNullOrWhiteSpace(cnpjDestinatario))
                result = result.Where(obj => obj.Destinatario.CPF_CNPJ == cnpjDestinatario);

            if (!string.IsNullOrWhiteSpace(cnpjExpedidor))
                result = result.Where(obj => obj.Expedidor.CPF_CNPJ == cnpjExpedidor);

            if (!string.IsNullOrWhiteSpace(cnpjRecebedor))
                result = result.Where(obj => obj.Recebedor.CPF_CNPJ == cnpjRecebedor);

            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.ConhecimentoDeTransporteEletronico> Consultar(Dominio.ObjetosDeValor.Embarcador.CTe.FiltroPesquisaAutorizacaoCTeLote filtrosPesquisa, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametroConsulta)
        {
            var consultaCTe = Consultar(filtrosPesquisa);

            return ObterLista(consultaCTe, parametroConsulta);
        }

        public int ContarConsulta(Dominio.ObjetosDeValor.Embarcador.CTe.FiltroPesquisaAutorizacaoCTeLote filtrosPesquisa)
        {
            var consultaCTe = Consultar(filtrosPesquisa);

            return consultaCTe.Count();
        }

        public List<Dominio.Entidades.ConhecimentoDeTransporteEletronico> BuscarPorNumeroPedido(string numeroPedido, bool permiteCTeComplementar)
        {
            List<Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga> situacoesCargaNaoPermitidas = new List<Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga>()
            {
                 Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.Cancelada,
                 Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.Anulada
            };

            IQueryable<Dominio.Entidades.ConhecimentoDeTransporteEletronico> query = this.SessionNHiBernate.Query<Dominio.Entidades.ConhecimentoDeTransporteEletronico>();
            IQueryable<Dominio.Entidades.Embarcador.Cargas.CargaCTe> queryCargaCTe = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCTe>();
            IQueryable<Dominio.Entidades.Embarcador.Cargas.CargaPedidoDocumentoCTe> queryCargaPedidoDocumentoCTe = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedidoDocumentoCTe>();

            queryCargaCTe = queryCargaCTe.Where(o => !situacoesCargaNaoPermitidas.Contains(o.Carga.SituacaoCarga));
            queryCargaPedidoDocumentoCTe = queryCargaPedidoDocumentoCTe.Where(o => !situacoesCargaNaoPermitidas.Contains(o.CargaPedido.Carga.SituacaoCarga));

            query = query.Where(o => !queryCargaCTe.Any(cct => cct.CTe == o));
            query = query.Where(o => !queryCargaPedidoDocumentoCTe.Any(cpd => cpd.CTe == o));
            query = query.Where(o => o.Status == "A");
            query = query.Where(o => o.NumeroPedido == numeroPedido);
            query = query.Where(o => o.TipoCTE != Dominio.Enumeradores.TipoCTE.Anulacao);
            query = query.Where(o => o.TipoCTE != Dominio.Enumeradores.TipoCTE.Substituto);
            query = query.Where(o => o.ModeloDocumentoFiscal == null || o.ModeloDocumentoFiscal.TipoDocumentoEmissao != Dominio.Enumeradores.TipoDocumento.NFSe);

            if (!permiteCTeComplementar)
                query = query.Where(o => o.TipoCTE != Dominio.Enumeradores.TipoCTE.Complemento);

            return query.ToList();
        }

        public List<Dominio.Entidades.ConhecimentoDeTransporteEletronico> ConsultaParaArquivoMercante(int codigoTerminalOrigem, int codigoViagem, string numeroControle)
        {
            IQueryable<Dominio.Entidades.ConhecimentoDeTransporteEletronico> query = this.SessionNHiBernate.Query<Dominio.Entidades.ConhecimentoDeTransporteEletronico>();

            query = query.Where(o => o.TerminalOrigem.Codigo == codigoTerminalOrigem && o.Viagem.Codigo == codigoViagem && o.NumeroControle == numeroControle);

            return query.ToList();
        }

        public Dominio.Entidades.ConhecimentoDeTransporteEletronico ConsultaParaArquivoMercanteUnica(int codigoTerminalOrigem, int codigoViagem, string numeroControle)
        {
            IQueryable<Dominio.Entidades.ConhecimentoDeTransporteEletronico> query = this.SessionNHiBernate.Query<Dominio.Entidades.ConhecimentoDeTransporteEletronico>();

            query = query.Where(o => o.TerminalOrigem.Codigo == codigoTerminalOrigem && o.Viagem.Codigo == codigoViagem && o.NumeroControle == numeroControle);

            return query.FirstOrDefault();
        }

        public Dominio.Relatorios.Embarcador.DataSource.Contabeis.AlteracaoArquivoMercante BuscarPorCTe(Dominio.ObjetosDeValor.Embarcador.Contabil.FiltroPesquisaAlteracaoMercante filtrosPesquisa)
        {
            var consulta = this.SessionNHiBernate.CreateSQLQuery(QueryConhecimentosAlteracaoArquivoMercante(filtrosPesquisa, false));

            consulta.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Dominio.Relatorios.Embarcador.DataSource.Contabeis.AlteracaoArquivoMercante)));

            return consulta.SetTimeout(600).List<Dominio.Relatorios.Embarcador.DataSource.Contabeis.AlteracaoArquivoMercante>().FirstOrDefault();
        }

        public IList<Dominio.Relatorios.Embarcador.DataSource.Contabeis.AlteracaoArquivoMercante> ConsultarConhecimentosAlteracaoArquivoMercante(Dominio.ObjetosDeValor.Embarcador.Contabil.FiltroPesquisaAlteracaoMercante filtrosPesquisa, string propOrdenacao, string dirOrdenacao, int inicioRegistros, int maximoRegistros)
        {
            var consulta = this.SessionNHiBernate.CreateSQLQuery(QueryConhecimentosAlteracaoArquivoMercante(filtrosPesquisa, false, propOrdenacao, dirOrdenacao, inicioRegistros, maximoRegistros));

            consulta.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Dominio.Relatorios.Embarcador.DataSource.Contabeis.AlteracaoArquivoMercante)));

            return consulta.SetTimeout(600).List<Dominio.Relatorios.Embarcador.DataSource.Contabeis.AlteracaoArquivoMercante>();
        }

        public int ContarConsultaConhecimentosAlteracaoArquivoMercante(Dominio.ObjetosDeValor.Embarcador.Contabil.FiltroPesquisaAlteracaoMercante filtrosPesquisa)
        {
            var consulta = this.SessionNHiBernate.CreateSQLQuery(QueryConhecimentosAlteracaoArquivoMercante(filtrosPesquisa, true));

            return consulta.SetTimeout(600).UniqueResult<int>();
        }

        public List<int> BuscarCTesFaturadosPendenteDeIntegracao(int inicioRegistros, int maximoRegistros)
        {
            IQueryable<Dominio.Entidades.ConhecimentoDeTransporteEletronico> query = SessionNHiBernate.Query<Dominio.Entidades.ConhecimentoDeTransporteEletronico>();

            query = query.Where(obj => obj.CTePendenteIntegracaoFatura == true);

            return query.Select(o => o.Codigo).Skip(inicioRegistros).Take(maximoRegistros).ToList();
        }

        public int ContarBuscarCTesFaturadosPendenteDeIntegracao()
        {
            IQueryable<Dominio.Entidades.ConhecimentoDeTransporteEletronico> query = SessionNHiBernate.Query<Dominio.Entidades.ConhecimentoDeTransporteEletronico>();

            query = query.Where(obj => obj.CTePendenteIntegracaoFatura == true);

            return query.Count();
        }

        public IList<Dominio.ObjetosDeValor.Embarcador.CTe.ConsultaCTeGeracaoCargaEmbarcador> ConsultarCTesDisponiveisParaGerarCarga(Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            NHibernate.ISQLQuery sqlQuery = this.SessionNHiBernate.CreateSQLQuery(ObterSQLCTesDisponiveisParaGerarCarga(parametrosConsulta));

            sqlQuery.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Dominio.ObjetosDeValor.Embarcador.CTe.ConsultaCTeGeracaoCargaEmbarcador)));

            return sqlQuery.SetTimeout(3000).List<Dominio.ObjetosDeValor.Embarcador.CTe.ConsultaCTeGeracaoCargaEmbarcador>();
        }

        public int ContarConsultaCTesDisponiveisParaGerarCarga()
        {
            NHibernate.ISQLQuery sqlQuery = this.SessionNHiBernate.CreateSQLQuery(ObterSQLCTesDisponiveisParaGerarCarga(null));

            return sqlQuery.UniqueResult<int>();
        }

        public Dominio.Entidades.ConhecimentoDeTransporteEletronico BuscarNFSe(int numero, int serie, int codigoEmpresa)
        {
            IQueryable<Dominio.Entidades.ConhecimentoDeTransporteEletronico> query = this.SessionNHiBernate.Query<Dominio.Entidades.ConhecimentoDeTransporteEletronico>();

            query = query.Where(o => o.Numero == numero && o.Serie.Numero == serie && o.Empresa.Codigo == codigoEmpresa && o.ModeloDocumentoFiscal.Numero == "39");

            return query.FirstOrDefault();
        }

        public Dominio.Entidades.ConhecimentoDeTransporteEletronico BuscarNFSe(int numero, int serie, int codigoEmpresa, string protocolo)
        {
            IQueryable<Dominio.Entidades.ConhecimentoDeTransporteEletronico> query = this.SessionNHiBernate.Query<Dominio.Entidades.ConhecimentoDeTransporteEletronico>();

            query = query.Where(o => o.Numero == numero && o.Serie.Numero == serie && o.Empresa.Codigo == codigoEmpresa && o.ModeloDocumentoFiscal.Numero == "39" && o.Protocolo == protocolo);

            return query.FirstOrDefault();
        }

        public List<int> BuscarCTesSemTitulosOuNaoQuitadosEmConciliacao(DateTime? DataCorte)
        {
            DateTime dataLimite = new DateTime(2021, 08, 1);
            if (DataCorte.HasValue && DataCorte != DateTime.MinValue)
            {
                dataLimite = DataCorte.Value;
            }

            var query = SessionNHiBernate.Query<Dominio.Entidades.ConhecimentoDeTransporteEletronico>();
            var result = query.Where(obj => obj.Status == "A" && obj.DataEmissao >= dataLimite && obj.Remetente != null && (obj.Titulo == null || obj.Titulo.StatusTitulo != Dominio.ObjetosDeValor.Embarcador.Enumeradores.StatusTitulo.Quitada) && obj.ConciliacaoTransportador != null);

            return result.Select(p => p.Codigo).ToList();
        }

        public List<Dominio.Entidades.ConhecimentoDeTransporteEletronico> BuscarPorCodigosConciliacaoTransportador(List<int> codigosConciliacaoTransportador)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.ConhecimentoDeTransporteEletronico>();
            query = query.Where(o => codigosConciliacaoTransportador.Contains(o.ConciliacaoTransportador.Codigo));
            return query.Fetch(o => o.Titulo).ToList();
        }

        public List<Dominio.Entidades.ConhecimentoDeTransporteEletronico> BuscarSemControleDocumentoComDataCorte(DateTime dataCorte, int limit)
        {
            var queryCD = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Documentos.ControleDocumento>().Select(o => o.CTe.Codigo);
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.ConhecimentoDeTransporteEletronico>().Where(o => o.DataEmissao >= dataCorte && !queryCD.Contains(o.Codigo));

            return query.Take(limit).ToList();
        }

        public List<Dominio.Entidades.ConhecimentoDeTransporteEletronico> BustarCTesParaFaturamento()
        {
            IQueryable<Dominio.Entidades.ConhecimentoDeTransporteEletronico> queryConhecimento = this.SessionNHiBernate.Query<Dominio.Entidades.ConhecimentoDeTransporteEletronico>();
            IQueryable<Dominio.Entidades.Embarcador.Fatura.FaturaDocumento> queryFaturaDocumento = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Fatura.FaturaDocumento>();
            IQueryable<Dominio.Entidades.Embarcador.Financeiro.TituloDocumento> queryTituloDocumento = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.TituloDocumento>();

            var resultConhecimento = from obj in queryConhecimento select obj;
            List<Dominio.Enumeradores.TipoCTE> lsttipocte = new List<Dominio.Enumeradores.TipoCTE>() { Dominio.Enumeradores.TipoCTE.Normal, Dominio.Enumeradores.TipoCTE.Complemento };

            resultConhecimento = resultConhecimento.Where(obj => obj.DataAutorizacao <= System.DateTime.Now.AddHours(-24)
                && (obj.TomadorPagador.Cliente.GrupoPessoas.GerarFaturaAutomaticaCte == true || obj.TomadorPagador.Cliente.GerarFaturaAutomaticaCte == true)
                && obj.Status == "A" && lsttipocte.Contains(obj.TipoCTE)
                && !queryFaturaDocumento.Any(o => o.Documento.CTe.Codigo == obj.Codigo && o.Fatura.Situacao != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoFatura.Cancelado)
                && !queryTituloDocumento.Any(o => o.CTe.Codigo == obj.Codigo && o.Titulo.StatusTitulo != Dominio.ObjetosDeValor.Embarcador.Enumeradores.StatusTitulo.Cancelado)
                );

            return resultConhecimento.ToList();
        }

        public IList<(int CodigoCTe, bool ImprimirTabelaTemperaturaVersoCTe)> BuscarInformacaoImpressaoTabelaTemperaturaVersoCTe(List<int> codigosCTe)
        {
            string sql = $@"
                select
                    CTe.CON_CODIGO CodigoCTe,
                    ISNULL(
                        TipoDeCarga.TCG_IMPRIMIR_TABELA_TEMPERATURA_VERSO_CTE,
                        0
                    ) ImprimirTabelaTemperaturaVersoCTe
                from
                    T_CTE CTe
                    JOIN T_CARGA_CTE CargaCte on CTe.CON_CODIGO = CargaCte.CON_CODIGO
                    JOIN T_CARGA_PEDIDO_XML_NOTA_FISCAL_CTE CargaPedidoXmlNotaFiscal on CargaCte.CCT_CODIGO = CargaPedidoXmlNotaFiscal.CCT_CODIGO
                    JOIN T_PEDIDO_XML_NOTA_FISCAL PedidoXmlNotaFiscal on CargaPedidoXmlNotaFiscal.PNF_CODIGO = PedidoXmlNotaFiscal.PNF_CODIGO
                    JOIN T_CARGA_PEDIDO CargaPedido on PedidoXmlNotaFiscal.CPE_CODIGO = CargaPedido.CPE_CODIGO
                    JOIN T_PEDIDO Pedido on CargaPedido.PED_CODIGO = Pedido.PED_CODIGO
                    LEFT JOIN T_TIPO_DE_CARGA TipoDeCarga on Pedido.TCG_CODIGO = TipoDeCarga.TCG_CODIGO
                WHERE
                    CTe.CON_CODIGO in ({string.Join(", ", codigosCTe)});";

            var consultaCTeImprimirTabelaTemperaturaVersoCTe = this.SessionNHiBernate.CreateSQLQuery(sql);

            consultaCTeImprimirTabelaTemperaturaVersoCTe.SetResultTransformer(new NHibernate.Transform.AliasToBeanConstructorResultTransformer(typeof((int CodigoCTe, bool ImprimirTabelaTemperaturaVersoCTe)).GetConstructors().FirstOrDefault()));

            return consultaCTeImprimirTabelaTemperaturaVersoCTe.SetTimeout(600).List<(int CodigoCTe, bool ImprimirTabelaTemperaturaVersoCTe)>();
        }

        public List<Dominio.Entidades.ConhecimentoDeTransporteEletronico> BuscarListaNFSePendentesIntegracaoMigrate(int numeroTentativas, double minutosACadaTentativa, string propOrdenacao, string dirOrdenacao, int numeroRegistrosPorVez)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.ConhecimentoDeTransporteEletronico>();

            var result = from obj in query
                         where
                               obj.SistemaEmissor == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEmissorDocumento.Migrate
                               && obj.RPS.QuantidadeTentativaConsulta <= numeroTentativas
                               && (obj.RPS.DataUltimaConsulta.Value.AddMinutes(minutosACadaTentativa) <= DateTime.Now || obj.RPS.DataUltimaConsulta == null)
                               && obj.Status == "E"
                               && obj.ModeloDocumentoFiscal.TipoDocumentoEmissao == Dominio.Enumeradores.TipoDocumento.NFSe
                         select obj;

            return result.OrderBy(propOrdenacao + (dirOrdenacao == "asc" ? " ascending" : " descending")).Skip(0).Take(numeroRegistrosPorVez).ToList();
        }

        public string BuscarNumeroBooking(int codigoCTe)
        {
            var query = this.SessionNHiBernate
                .Query<Dominio.Entidades.Embarcador.Cargas.CargaCTe>()
                .Where(c => c.CTe.Codigo == codigoCTe);

            var queryPedido = this.SessionNHiBernate
                .Query<Dominio.Entidades.Embarcador.Cargas.CargaPedido>()
                .Where(p => query.Any(c => c.Carga == p.Carga));

            return string.Join(", ", queryPedido.Select(c => c.Pedido.NumeroBooking).ToList());
        }

        public string BuscarViagem(int codigoCTe)
        {
            var query = this.SessionNHiBernate
                .Query<Dominio.Entidades.Embarcador.Cargas.CargaCTe>()
                .Where(c => c.CTe.Codigo == codigoCTe);

            var queryPedido = this.SessionNHiBernate
                .Query<Dominio.Entidades.Embarcador.Cargas.CargaPedido>()
                .Where(p => query.Any(c => c.Carga == p.Carga))
                .ToList();

            return string.Join(", ", queryPedido.Select(c => c.Pedido?.PedidoViagemNavio?.Descricao ?? ""));
        }

        public string BuscarNumeroControleCliente(int codigoCTe)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCTe>();
            query = query.Where(obj => obj.CTe.Codigo == codigoCTe);

            var queryPedido = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedido>();
            queryPedido = queryPedido.Where(obj => query.Any(p => p.Carga == obj.Carga));

            var queryPedidoXMLNotaFiscal = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal>();
            queryPedidoXMLNotaFiscal = queryPedidoXMLNotaFiscal.Where(obj => queryPedido.Any(p => p == obj.CargaPedido));

            var queryXMLNotaFiscal = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal>();
            queryXMLNotaFiscal = queryXMLNotaFiscal.Where(obj => queryPedidoXMLNotaFiscal.Any(p => p.XMLNotaFiscal == obj));

            return string.Join(", ", queryXMLNotaFiscal
                .Select(c => c.NumeroControleCliente ?? "")
                .ToList());
        }

        public List<Dominio.Entidades.ConhecimentoDeTransporteEletronico> BuscarCTesPorCargaPedido(int cargaPedidoId)
        {
            var query = this.SessionNHiBernate
                .Query<Dominio.Entidades.Embarcador.Cargas.CargaPedidoXMLNotaFiscalCTe>()
                .Where(cpct => cpct.PedidoXMLNotaFiscal.CargaPedido.Codigo == cargaPedidoId)
                .Select(cpct => cpct.CargaCTe != null ? cpct.CargaCTe.CTe : null)
                .Where(cte => cte != null)
                .Distinct();

            return query.ToList();
        }

        public List<int> BuscarCTesSemXML(int maximoRegistros)
        {
            IQueryable<Dominio.Entidades.ConhecimentoDeTransporteEletronico> query = this.SessionNHiBernate.Query<Dominio.Entidades.ConhecimentoDeTransporteEletronico>()
                .Where(obj => obj.SistemaEmissor == TipoEmissorDocumento.NSTech && obj.Status == "A");

            IQueryable<Dominio.Entidades.XMLCTe> queryXMLCTe = this.SessionNHiBernate.Query<Dominio.Entidades.XMLCTe>();

            query = query.Where(cte => !queryXMLCTe.Any(obj => obj.CTe.Codigo == cte.Codigo));

            return query.OrderBy(o => o.Codigo).Select(obj => obj.Codigo).Take(maximoRegistros).ToList();
        }

        #endregion

        #region Métodos Privados

        public IQueryable<Dominio.Entidades.ConhecimentoDeTransporteEletronico> ConsultarCTesPorPeriodo(DateTime dataInicial, DateTime dataFinal, bool retornarTodos, string status)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.ConhecimentoDeTransporteEletronico>();

            query = from obj in query
                    where (obj.ModeloDocumentoFiscal.Numero.Equals("57") || obj.ModeloDocumentoFiscal.Numero.Equals("67"))
                    select obj;

            if (dataInicial > DateTime.MinValue)
                query = query.Where(obj => obj.DataEmissao >= dataInicial.Date);

            if (dataFinal > DateTime.MinValue)
                query = query.Where(obj => obj.DataEmissao < dataFinal.Date.AddDays(1));

            if (!string.IsNullOrWhiteSpace(status))
                query = query.Where(obj => obj.Status == status);

            if (!retornarTodos)
                query = query.Where(obj => obj.Titulo != null);

            return query;
        }

        private string ObterSQLCTesDisponiveisParaGerarCarga(Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            string sqlQuery = @"SELECT ";

            if (parametrosConsulta == null)
                sqlQuery += "COUNT(CTe.CON_CODIGO) ";
            else
            {
                sqlQuery += @"CTe.CON_CODIGO Codigo,
                                CTe.CON_NUM Numero,
                                SerieCTe.ESE_NUMERO Serie,
                                CTe.CON_DATAHORAEMISSAO DataEmissao,
                                Empresa.EMP_RAZAO + ' (' + LocalidadeEmpresa.LOC_DESCRICAO +' - ' + LocalidadeEmpresa.UF_SIGLA + ')' Empresa,
                                Origem.UF_SIGLA UFOrigem,
                                Destino.UF_SIGLA UFDestino,
                                Origem.LOC_DESCRICAO Origem,
                                Destino.LOC_DESCRICAO Destino,
                                Veiculo.VEI_CODIGO CodigoVeiculo,
                                Veiculo.VEI_PLACA Veiculo ";
            }

            sqlQuery += @"FROM T_CTE CTe
                            inner join T_EMPRESA_SERIE SerieCTe on CTe.CON_SERIE = SerieCTe.ESE_CODIGO
                            left join T_EMPRESA Empresa ON Empresa.EMP_CODIGO = CTe.EMP_CODIGO
                            left join T_LOCALIDADES LocalidadeEmpresa on Empresa.LOC_CODIGO = LocalidadeEmpresa.LOC_CODIGO 
                            join T_LOCALIDADES Origem on Origem.LOC_CODIGO = CTe.CON_LOCINICIOPRESTACAO
                            join T_LOCALIDADES Destino on Destino.LOC_CODIGO = CTe.CON_LOCTERMINOPRESTACAO
                            JOIN T_CTE_VEICULO CTeVeiculo ON CTeVeiculo.CON_CODIGO = CTe.CON_CODIGO
                            JOIN T_VEICULO Veiculo on Veiculo.VEI_CODIGO = CTeVeiculo.VEI_CODIGO
                            WHERE
                            CTe.CON_STATUS = 'A' AND Veiculo.VEI_TIPOVEICULO = '0' AND (CTe.CON_PROBLEMA_GERACAO_CARGA_AUTOMATICAMENTE IS NULL OR CON_PROBLEMA_GERACAO_CARGA_AUTOMATICAMENTE = 0) AND
                            CTe.CON_DESTINATARIO_CTE is not null AND CTe.CON_REMETENTE_CTE is not null AND
                            NOT EXISTS (SELECT CargaCTe.CON_CODIGO FROM T_CARGA_CTE CargaCTe INNER JOIN T_CARGA Carga ON Carga.CAR_CODIGO = CargaCTe.CAR_CODIGO WHERE CargaCTe.CON_CODIGO = CTe.CON_CODIGO AND Carga.CAR_SITUACAO NOT IN (13, 18)) AND 
                            NOT EXISTS (SELECT CON_CODIGO FROM T_CARGA_PEDIDO_DOCUMENTO_CTe CargaPedidoDocumentoCTe INNER JOIN T_CARGA_PEDIDO CargaPedido ON CargaPedido.CPE_CODIGO = CargaPedidoDocumentoCTe.CPE_CODIGO INNER JOIN T_CARGA Carga ON Carga.CAR_CODIGO = CargaPedido.CAR_CODIGO WHERE CargaPedidoDocumentoCTe.CON_CODIGO = CTe.CON_CODIGO AND Carga.CAR_SITUACAO NOT IN (13, 18)) AND
                            NOT EXISTS (SELECT CON_CODIGO FROM T_CARGA_INTEGRACAO_EMBARCADOR_CTe WHERE CON_CODIGO = CTe.CON_CODIGO) AND
                            NOT EXISTS (SELECT CON_CODIGO FROM T_MDFE_MUNICIPIO_DESCARREGAMENTO_DOC WHERE CON_CODIGO = CTe.CON_CODIGO)";

            if (parametrosConsulta != null)
                sqlQuery += $" ORDER BY {parametrosConsulta.PropriedadeOrdenar} {parametrosConsulta.DirecaoOrdenar} OFFSET {parametrosConsulta.InicioRegistros} ROWS FETCH FIRST {parametrosConsulta.LimiteRegistros} ROWS ONLY;";

            return sqlQuery;
        }

        private IQueryable<Dominio.Entidades.ConhecimentoDeTransporteEletronico> _ConsultarCtesSemDuplicata(int codigoEmpresa, DateTime dataEmissaoInicial, DateTime dataEmissaoFinal, int numeroInicial, int numeroFinal, string cnpjTomador, string cnpjRemetente, string cnpjDestinatario, Dominio.Enumeradores.TipoAmbiente tipoAmbiente, int[] series, string numeroDocumento, bool filtrarTodosCTes)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.ConhecimentoDeTransporteEletronico>();

            var result = from obj in query
                         where obj.TipoAmbiente == tipoAmbiente && obj.Empresa.Codigo == codigoEmpresa && obj.Status.Equals("A") &&
                                (obj.ModeloDocumentoFiscal.Numero.Equals("57") || obj.ModeloDocumentoFiscal.Numero.Equals("67") || obj.ModeloDocumentoFiscal.Numero.Equals("39"))
                         select obj;

            if (dataEmissaoInicial != DateTime.MinValue)
                result = result.Where(o => o.DataEmissao.Value.Date >= dataEmissaoInicial.Date);
            if (dataEmissaoFinal != DateTime.MinValue)
                result = result.Where(o => o.DataEmissao.Value.Date <= dataEmissaoFinal.Date);

            if (numeroInicial > 0)
                result = result.Where(o => o.Numero >= numeroInicial);
            if (numeroFinal > 0)
                result = result.Where(o => o.Numero <= numeroFinal);

            if (series.Count() > 0)
                result = result.Where(o => series.Contains(o.Serie.Codigo));

            if (!string.IsNullOrWhiteSpace(cnpjTomador))
                result = result.Where(o => (o.TomadorPagador.CPF_CNPJ.Equals(cnpjTomador)));

            if (!string.IsNullOrWhiteSpace(cnpjRemetente))
                result = result.Where(o => (o.Remetente.CPF_CNPJ.Equals(cnpjRemetente)));

            if (!string.IsNullOrWhiteSpace(cnpjDestinatario))
                result = result.Where(o => (o.Destinatario.CPF_CNPJ.Equals(cnpjDestinatario)));

            if (!string.IsNullOrWhiteSpace(numeroDocumento))
                result = result.Where(o => (from obj in o.Documentos where obj.Numero.Equals(numeroDocumento) select obj.CTE.Codigo).Contains(o.Codigo));

            if (!filtrarTodosCTes)
            {
                var queryDuplicataCtes = this.SessionNHiBernate.Query<Dominio.Entidades.DuplicataCtes>();
                result = result.Where(o => !(from obj in queryDuplicataCtes where obj.Duplicata.Status == "A" && obj.ConhecimentoDeTransporteEletronico.Codigo == o.Codigo select obj.ConhecimentoDeTransporteEletronico.Codigo).Contains(o.Codigo));
            }

            return result;
        }

        private IQueryable<Dominio.Entidades.ConhecimentoDeTransporteEletronico> Consultar(Dominio.ObjetosDeValor.Embarcador.CTe.FiltroPesquisaAutorizacaoCTeLote filtrosPesquisa)
        {
            var consultaCTe = this.SessionNHiBernate.Query<Dominio.Entidades.ConhecimentoDeTransporteEletronico>();

            if (filtrosPesquisa.DataInicial != DateTime.MinValue)
                consultaCTe = consultaCTe.Where(o => o.DataEmissao.Value.Date >= filtrosPesquisa.DataInicial.Date);

            if (filtrosPesquisa.DataFinal != DateTime.MinValue)
                consultaCTe = consultaCTe.Where(o => o.DataEmissao.Value.Date <= filtrosPesquisa.DataFinal.Date);

            if (filtrosPesquisa.NumeroInicial > 0)
                consultaCTe = consultaCTe.Where(o => o.Numero >= filtrosPesquisa.NumeroInicial);

            if (filtrosPesquisa.NumeroFinal > 0)
                consultaCTe = consultaCTe.Where(o => o.Numero <= filtrosPesquisa.NumeroFinal);

            if (filtrosPesquisa.TipoModal != null && filtrosPesquisa.TipoModal.Count > 0)
            {
                if (filtrosPesquisa.TipoModal.Count == 1)
                    consultaCTe = consultaCTe.Where(o => o.TipoModal == filtrosPesquisa.TipoModal[0]);
                else
                    consultaCTe = consultaCTe.Where(o => filtrosPesquisa.TipoModal.Contains(o.TipoModal));
            }

            if (filtrosPesquisa.Status.Count > 0)
                consultaCTe = consultaCTe.Where(o => filtrosPesquisa.Status.Contains(o.Status));
            else
                consultaCTe = consultaCTe.Where(o => o.Status.Equals("P") || o.Status.Equals("R") || o.Status.Equals("S") || o.Status.Equals("F"));

            if (filtrosPesquisa.CodigoEmpresa > 0)
                consultaCTe = consultaCTe.Where(o => o.Empresa.Codigo == filtrosPesquisa.CodigoEmpresa);

            if (filtrosPesquisa.CodigoPortoOrigem > 0)
                consultaCTe = consultaCTe.Where(o => o.PortoOrigem.Codigo == filtrosPesquisa.CodigoPortoOrigem);

            if (filtrosPesquisa.CodigoTerminalOrigem > 0)
                consultaCTe = consultaCTe.Where(o => o.TerminalOrigem.Codigo == filtrosPesquisa.CodigoTerminalOrigem);

            if (filtrosPesquisa.CodigoTerminalDestino > 0)
                consultaCTe = consultaCTe.Where(o => o.TerminalDestino.Codigo == filtrosPesquisa.CodigoTerminalDestino);

            if (filtrosPesquisa.CodigoViagem > 0)
                consultaCTe = consultaCTe.Where(o => o.Viagem.Codigo == filtrosPesquisa.CodigoViagem);

            if (!string.IsNullOrWhiteSpace(filtrosPesquisa.NumeroBooking))
                consultaCTe = consultaCTe.Where(o => o.NumeroBooking == filtrosPesquisa.NumeroBooking);

            if (!string.IsNullOrWhiteSpace(filtrosPesquisa.NumeroOS))
                consultaCTe = consultaCTe.Where(o => o.NumeroOS == filtrosPesquisa.NumeroOS);

            if (!string.IsNullOrWhiteSpace(filtrosPesquisa.NumeroControle))
                consultaCTe = consultaCTe.Where(o => o.NumeroControle == filtrosPesquisa.NumeroControle);

            if (filtrosPesquisa.CodigoContainer > 0)
                consultaCTe = consultaCTe.Where(o => o.Containers.Any(c => c.Container.Codigo == filtrosPesquisa.CodigoContainer));

            if (!string.IsNullOrWhiteSpace(filtrosPesquisa.NumeroNF))
                consultaCTe = consultaCTe.Where(o => o.Documentos.Any(c => c.Numero == filtrosPesquisa.NumeroNF));

            return consultaCTe.Fetch(o => o.Remetente)
                        .Fetch(o => o.LocalidadeInicioPrestacao)
                        .Fetch(o => o.PortoOrigem)
                        .Fetch(o => o.TerminalOrigem)
                        .Fetch(o => o.Viagem)
                        .Fetch(o => o.Serie);
        }

        private IQueryable<Dominio.Entidades.ConhecimentoDeTransporteEletronico> ObterConsultaFormularioConsultaCTe(Dominio.ObjetosDeValor.Embarcador.CTe.FiltroPesquisaConsultaCTe filtrosPesquisa)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.ConhecimentoDeTransporteEletronico>();

            if (filtrosPesquisa.TipoDocumentoEmissao != Dominio.Enumeradores.TipoDocumento.Todos)
                query = query.Where(o => o.ModeloDocumentoFiscal.TipoDocumentoEmissao == filtrosPesquisa.TipoDocumentoEmissao);
            else if (filtrosPesquisa.CodigoModeloDocumento > 0)
                query = query.Where(o => o.ModeloDocumentoFiscal.Codigo == filtrosPesquisa.CodigoModeloDocumento);

            if (filtrosPesquisa.CodigoEmpresa > 0)
                query = query.Where(o => o.Empresa.Codigo == filtrosPesquisa.CodigoEmpresa && o.Empresa.TipoAmbiente == o.TipoAmbiente);

            if (filtrosPesquisa.DataEmissaoInicial != DateTime.MinValue)
                query = query.Where(o => o.DataEmissao.Value.Date >= filtrosPesquisa.DataEmissaoInicial.Date);

            if (filtrosPesquisa.DataEmissaoFinal != DateTime.MinValue)
                query = query.Where(o => o.DataEmissao.Value.Date <= filtrosPesquisa.DataEmissaoFinal.Date);

            if (filtrosPesquisa.NumeroInicial > 0)
                query = query.Where(o => o.Numero >= filtrosPesquisa.NumeroInicial);

            if (filtrosPesquisa.NumeroFinal > 0)
                query = query.Where(o => o.Numero <= filtrosPesquisa.NumeroFinal);

            if (filtrosPesquisa.Serie > 0)
                query = query.Where(o => o.Serie.Numero == filtrosPesquisa.Serie);

            if (filtrosPesquisa.CodigoOrigem > 0)
                query = query.Where(o => o.LocalidadeInicioPrestacao.Codigo == filtrosPesquisa.CodigoOrigem);

            if (filtrosPesquisa.CodigoDestino > 0)
                query = query.Where(o => o.LocalidadeTerminoPrestacao.Codigo == filtrosPesquisa.CodigoDestino);

            if (filtrosPesquisa.CodigoCarga > 0)
                query = query.Where(o => o.CargaCTes.Any(c => c.Carga.Codigo == filtrosPesquisa.CodigoCarga));

            if (filtrosPesquisa.CodigoGrupoPessoas > 0)
                query = query.Where(o => o.TomadorPagador.Cliente.GrupoPessoas.Codigo == filtrosPesquisa.CodigoGrupoPessoas);

            if (filtrosPesquisa.CpfCnpjTomadores?.Count > 0)
                query = query.Where(o => filtrosPesquisa.CpfCnpjTomadores.Contains(o.TomadorPagador.CPF_CNPJ));
            else if (!string.IsNullOrWhiteSpace(filtrosPesquisa.CpfCnpjTomador))
                query = query.Where(o => o.TomadorPagador.CPF_CNPJ == filtrosPesquisa.CpfCnpjTomador);

            if (!string.IsNullOrWhiteSpace(filtrosPesquisa.CpfCnpjRemetente))
                query = query.Where(o => o.Remetente.CPF_CNPJ == filtrosPesquisa.CpfCnpjRemetente);

            if (!string.IsNullOrWhiteSpace(filtrosPesquisa.CpfCnpjDestinatario))
                query = query.Where(o => o.Destinatario.CPF_CNPJ == filtrosPesquisa.CpfCnpjDestinatario);

            if (!string.IsNullOrWhiteSpace(filtrosPesquisa.Chave))
                query = query.Where(o => o.Chave == filtrosPesquisa.Chave);

            if (filtrosPesquisa.StatusCTe?.Count > 0)
                query = query.Where(o => filtrosPesquisa.StatusCTe.Contains(o.Status));
            else
                query = query.Where(o => new string[] { "A", "C", "Z", "I", "D" }.Contains(o.Status));

            if (!string.IsNullOrWhiteSpace(filtrosPesquisa.CpfCnpjTransportadorTerceiro))
                query = query.Where(obj => (obj.CargaCTes.Any(c => c.Carga.Veiculo.Proprietario.CPF_CNPJ == double.Parse(filtrosPesquisa.CpfCnpjTransportadorTerceiro))) || (obj.Veiculos.Any(o => o.Proprietario.CPF_CNPJ == filtrosPesquisa.CpfCnpjTransportadorTerceiro) || obj.Empresa.CNPJ == filtrosPesquisa.CpfCnpjTransportadorTerceiro));

            if (filtrosPesquisa.CodigosFatura?.Count > 0)
            {
                var queryFaturaDocumento = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Fatura.FaturaDocumento>();

                queryFaturaDocumento = queryFaturaDocumento.Where(o => filtrosPesquisa.CodigosFatura.Contains(o.Fatura.Codigo) && o.Documento.TipoDocumento == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoFaturamento.CTe);

                query = query.Where(o => queryFaturaDocumento.Select(fd => fd.Documento.CTe.Codigo).Contains(o.Codigo));
            }

            if (filtrosPesquisa.CodigosTipoOperacao?.Count > 0)
            {
                var queryCarga = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCTe>();

                queryCarga = queryCarga.Where(o => filtrosPesquisa.CodigosTipoOperacao.Contains(o.Carga.TipoOperacao.Codigo));

                query = query.Where(o => queryCarga.Select(qc => qc.CTe.Codigo).Contains(o.Codigo));
            }

            if (filtrosPesquisa.CodigosFiliais.Any(codigo => codigo == -1))
            {
                var subquery = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaMDFe>();
                query = query.Where(mdfe => subquery.Any(cargaMdfe => (cargaMdfe.MDFe.Codigo == mdfe.Codigo) && (filtrosPesquisa.CodigosFiliais.Contains(cargaMdfe.Carga.Filial.Codigo)) || cargaMdfe.Carga.Pedidos.Any(pedido => filtrosPesquisa.CodigosRecebedores.Contains(pedido.Recebedor.CPF_CNPJ))));
            }
            else if (filtrosPesquisa.CodigosFiliais.Count > 0)
                query = query.Where(o => o.CargaCTes.Any(c => filtrosPesquisa.CodigosFiliais.Contains(c.Carga.Filial.Codigo)));

            if (filtrosPesquisa.CodigosRecebedores.Count > 0)
                query = query.Where(o => filtrosPesquisa.CodigosRecebedores.Contains(o.Recebedor.Cliente.CPF_CNPJ));

            if (filtrosPesquisa.CodigoTransportador > 0)
                query = query.Where(o => o.Empresa.Codigo == filtrosPesquisa.CodigoTransportador);

            if (filtrosPesquisa.VinculoCarga.HasValue)
            {
                IQueryable<Dominio.Entidades.Embarcador.Cargas.CargaCTe> queryCargaCTe = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCTe>();

                if (!filtrosPesquisa.VinculoCarga.Value)
                    query = query.Where(o => !queryCargaCTe.Any(cct => cct.CTe.Codigo == o.Codigo));
                else
                    query = query.Where(o => queryCargaCTe.Any(cct => cct.CTe.Codigo == o.Codigo));
            }

            if (!string.IsNullOrWhiteSpace(filtrosPesquisa.NumeroBooking))
                query = query.Where(obj => obj.NumeroBooking == filtrosPesquisa.NumeroBooking);

            if (!string.IsNullOrWhiteSpace(filtrosPesquisa.NumeroOS))
                query = query.Where(obj => obj.NumeroOS == filtrosPesquisa.NumeroOS);

            if (filtrosPesquisa.NumeroSerie > 0)
                query = query.Where(obj => obj.Serie.Numero == filtrosPesquisa.NumeroSerie);

            if (!string.IsNullOrWhiteSpace(filtrosPesquisa.NumeroControleCliente))
                query = query.Where(obj => obj.XMLNotaFiscais.Any(o => o.NumeroControleCliente == filtrosPesquisa.NumeroControleCliente));

            if (!string.IsNullOrWhiteSpace(filtrosPesquisa.NumeroControle))
                query = query.Where(obj => obj.NumeroControle == filtrosPesquisa.NumeroControle);

            if (filtrosPesquisa.CodigoTerminalOrigem > 0)
                query = query.Where(obj => obj.TerminalOrigem.Codigo == filtrosPesquisa.CodigoTerminalOrigem);

            if (filtrosPesquisa.CodigoTerminalDestino > 0)
                query = query.Where(obj => obj.TerminalDestino.Codigo == filtrosPesquisa.CodigoTerminalDestino);

            if (filtrosPesquisa.CodigoViagem > 0)
                query = query.Where(obj => obj.Viagem.Codigo == filtrosPesquisa.CodigoViagem);

            if (filtrosPesquisa.CodigoPortoOrigem > 0)
                query = query.Where(obj => obj.PortoOrigem.Codigo == filtrosPesquisa.CodigoPortoOrigem);

            if (filtrosPesquisa.CodigoPortoDestino > 0)
                query = query.Where(obj => obj.PortoDestino.Codigo == filtrosPesquisa.CodigoPortoDestino);

            if (filtrosPesquisa.CodigoPortoTransbordo > 0)
                query = query.Where(obj => obj.PortoPassagemUm.Codigo == filtrosPesquisa.CodigoPortoTransbordo);

            if (filtrosPesquisa.CodigoViagemTransbordo > 0)
                query = query.Where(obj => obj.ViagemPassagemUm.Codigo == filtrosPesquisa.CodigoViagemTransbordo);

            if (filtrosPesquisa.TipoProposta.Count > 0)
                query = query.Where(obj => obj.CargaCTes.Any(o => o.Carga.CargaOrigemPedidos.Any(p => filtrosPesquisa.TipoProposta.Contains(p.TipoPropostaMultimodal))));

            if (filtrosPesquisa.TipoServicoCarga?.Count > 0)
                query = query.Where(obj => obj.CargaCTes.Any(o => o.Carga.CargaOrigemPedidos.Any(p => filtrosPesquisa.TipoServicoCarga.Contains(p.TipoServicoMultimodal))));

            if (filtrosPesquisa.TipoServico.Count > 0)
                query = query.Where(obj => filtrosPesquisa.TipoServico.Contains(obj.TipoServico));

            if (filtrosPesquisa.Documento.HasValue)
            {
                if (filtrosPesquisa.Documento.Value)
                    query = query.Where(obj => obj.TipoCTE == Dominio.Enumeradores.TipoCTE.Normal);
                else
                    query = query.Where(obj => obj.TipoCTE != Dominio.Enumeradores.TipoCTE.Normal);
            }

            if (!string.IsNullOrWhiteSpace(filtrosPesquisa.Placa))
                query = query.Where(obj => obj.Veiculos.Any(o => o.Placa == filtrosPesquisa.Placa));

            if (filtrosPesquisa.TipoCTe != Dominio.Enumeradores.TipoCTE.Todos)
                query = query.Where(obj => obj.TipoCTE == filtrosPesquisa.TipoCTe);

            if (filtrosPesquisa.VeioPorImportacao != Dominio.Enumeradores.OpcaoSimNaoPesquisa.Todos)
            {
                if (filtrosPesquisa.VeioPorImportacao == Dominio.Enumeradores.OpcaoSimNaoPesquisa.Sim)
                    query = query.Where(obj => obj.CTeImportadoEmbarcador.Value);
                else
                    query = query.Where(obj => obj.CTeImportadoEmbarcador == null || !obj.CTeImportadoEmbarcador.Value);
            }

            if (filtrosPesquisa.SomenteCTeSubstituido)
            {
                IQueryable<Dominio.Entidades.ConhecimentoDeTransporteEletronico> queryCTe = this.SessionNHiBernate.Query<Dominio.Entidades.ConhecimentoDeTransporteEletronico>();

                query = query.Where(o => queryCTe.Any(ct => ct.ChaveCTESubComp.Equals(o.Chave)));
            }

            if (filtrosPesquisa.TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiCTe)
            {
                var queryTipoOcorrencia = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCTeComplementoInfo>()
                    .Where(obj =>
                        obj.CTe != null &&
                        obj.CargaOcorrencia.TipoOcorrencia.BloquearVisualizacaoTipoOcorrenciaTransportador == true &&
                        obj.CargaOcorrencia.TipoOcorrencia.PermitirConsultarCTesComEsseTipoDeOcorrencia == false);

                query = query.Where(cte => !queryTipoOcorrencia.Any(o => o.CTe.Codigo == cte.Codigo));
            }

            if (filtrosPesquisa.CodigosCTe?.Count > 0)
                query = query.Where(obj => filtrosPesquisa.CodigosCTe.Contains(obj.Codigo));

            if (filtrosPesquisa.CodigoConciliacaoTransportador > 0)
            {
                query = query.Where(obj => obj.ConciliacaoTransportador.Codigo == filtrosPesquisa.CodigoConciliacaoTransportador);
            }

            if (filtrosPesquisa.TipoPagamento.HasValue)
            {
                if (filtrosPesquisa.TipoPagamento == Dominio.Enumeradores.TipoPagamento.Pago) //só vai buscar onde tem titulos e estao quitados
                    query = query.Where(obj => obj.Titulo.StatusTitulo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.StatusTitulo.Quitada && obj.TipoPagamento == filtrosPesquisa.TipoPagamento.Value);
                else
                {
                    query = query.Where(obj => obj.Titulo == null || (obj.Titulo != null && obj.Titulo.StatusTitulo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.StatusTitulo.EmAberto));
                }
            }

            return query;
        }

        private string QueryConhecimentosAlteracaoArquivoMercante(Dominio.ObjetosDeValor.Embarcador.Contabil.FiltroPesquisaAlteracaoMercante filtrosPesquisa, bool somenteContarNumeroRegistros, string propOrdenacao = "", string dirOrdenacao = "", int inicioRegistros = 0, int maximoRegistros = 0)
        {
            string sql;

            if (somenteContarNumeroRegistros)
                sql = "select distinct(count(0) over ()) ";
            else
                sql = @"select
                        CTe.CON_CODIGO Codigo,
                        Viagem.PVN_DESCRICAO Viagem,
                        TerminalOrigem.TTI_DESCRICAO TerminalOrigem,
                        TerminalDestino.TTI_DESCRICAO TerminalDestino,
                        CTe.CON_NUMERO_BOOKING NumeroBooking,
                        CTe.CON_NUMERO_CONTROLE NumeroControle,
                        Container.CTR_DESCRICAO Container,
                        CTe.CON_NUM NumeroCTe,
                        CTe.CON_NUMERO_MANIFESTO NumeroManifesto,
                        CTe.CON_NUMERO_MANIFESTO_TRANSBORDO NumeroManifestoTransbordo,
                        CTe.CON_NUMERO_CE_MERCANTE NumeroCE,
                        (CASE WHEN CTe.POT_CODIGO_PASSAGEM_UM IS NOT NULL THEN 'Sim' ELSE 'Não' END) PossuiTransbordo,
                        StatusCTe = CASE CTe.CON_STATUS 
		                        WHEN 'A' THEN 'Autorizado' 
		                        WHEN 'P' THEN 'Pendente' 
		                        WHEN 'E' THEN 'Enviado' 
		                        WHEN 'R' THEN 'Rejeitado' 
		                        WHEN 'C' THEN 'Cancelado' 
		                        WHEN 'I' THEN 'Inutilizado' 
		                        WHEN 'D' THEN 'Denegado' 
		                        WHEN 'S' THEN 'Em Digitação' 
		                        WHEN 'K' THEN 'Em Cancelamento' 
		                        WHEN 'L' THEN 'Em Inutilização' 
                                WHEN 'Z' THEN 'Anulado' 
		                        ELSE '' END,
                        concat(PortoPassagemUm.POT_DESCRICAO
                                           , CASE WHEN PortoPassagemDois.POT_CODIGO IS NOT NULL THEN ', ' + PortoPassagemDois.POT_DESCRICAO ELSE '' END
                                           , CASE WHEN PortoPassagemTres.POT_CODIGO IS NOT NULL THEN ', ' + PortoPassagemTres.POT_DESCRICAO ELSE '' END
                                           , CASE WHEN PortoPassagemQuatro.POT_CODIGO IS NOT NULL THEN ', ' + PortoPassagemQuatro.POT_DESCRICAO ELSE '' END
                                           , CASE WHEN PortoPassagemCinco.POT_CODIGO IS NOT NULL THEN ', ' + PortoPassagemCinco.POT_DESCRICAO ELSE '' END
                        ) PortoTransbordo,
                        SUBSTRING((SELECT DISTINCT ', ' + navio.PVN_DESCRICAO
                                        from T_PEDIDO_VIAGEM_NAVIO navio
                                        inner join T_PEDIDO_TRANSBORDO pedidoTransbordo on pedidoTransbordo.PVN_CODIGO = navio.PVN_CODIGO 
                                        inner join T_CARGA_PEDIDO CargaPedido on CargaPedido.PED_CODIGO = pedidoTransbordo.PED_CODIGO 
                                        inner join T_CARGA_CTE CargaCTe on CargaCTe.CAR_CODIGO = CargaPedido.CAR_CODIGO 
                                 WHERE CargaCTe.CON_CODIGO = CTe.CON_CODIGO for xml path('')), 3, 1000) NavioTransbordo,
                        Balsa.NAV_DESCRICAO Balsa";

            sql += @"   from T_CTE CTe
                        join T_PEDIDO_VIAGEM_NAVIO Viagem on Viagem.PVN_CODIGO = CTe.CON_VIAGEM
                        join T_TIPO_TERMINAL_IMPORTACAO TerminalOrigem on TerminalOrigem.TTI_CODIGO = CTe.CON_TERMINAL_ORIGEM
                        join T_TIPO_TERMINAL_IMPORTACAO TerminalDestino on TerminalDestino.TTI_CODIGO = CTe.CON_TERMINAL_DESTINO
                        left join T_CTE_CONTAINER CTeContainer on CTe.CON_CODIGO = CTeContainer.CON_CODIGO
                        left join T_CONTAINER Container on Container.CTR_CODIGO = CTeContainer.CTR_CODIGO
                        left join T_PORTO PortoPassagemUm on PortoPassagemUm.POT_CODIGO = CTe.POT_CODIGO_PASSAGEM_UM
                        left join T_PORTO PortoPassagemDois on PortoPassagemDois.POT_CODIGO = CTe.POT_CODIGO_PASSAGEM_DOIS
                        left join T_PORTO PortoPassagemTres on PortoPassagemTres.POT_CODIGO = CTe.POT_CODIGO_PASSAGEM_TRES
                        left join T_PORTO PortoPassagemQuatro on PortoPassagemQuatro.POT_CODIGO = CTe.POT_CODIGO_PASSAGEM_QUATRO
                        left join T_PORTO PortoPassagemCinco on PortoPassagemCinco.POT_CODIGO = CTe.POT_CODIGO_PASSAGEM_CINCO
                        left join T_NAVIO Balsa On Balsa.NAV_CODIGO = CTe.NAV_CODIGO_BALSA";

            //             MTL - Multimodal                                      VAS e Feeder
            sql += @" where CTe.CON_TIPO_MODAL <> 6 and CTe.CON_CODIGO not in ( select distinct CargaCTe.CON_CODIGO from T_CARGA_CTE CargaCTe
                                                                                inner join T_CARGA_PEDIDO CargaPedido on CargaCTe.CAR_CODIGO = CargaPedido.CAR_CODIGO
                                                                                where CargaPedido.TBF_TIPO_PROPOSTA_MULTIMODAL in (3, 4, 5, 7, 6)) ";

            sql += @" and CTe.CON_TIPO_CTE <> 2 and (CTe.CON_NAO_ENVIAR_PARA_MERCANTE is null or CTe.CON_NAO_ENVIAR_PARA_MERCANTE = 0)";

            if (filtrosPesquisa.CodigoViagem > 0)
                sql += $" and CTe.CON_VIAGEM = {filtrosPesquisa.CodigoViagem}";

            if (filtrosPesquisa.CodigoTerminalOrigem > 0)
                sql += $" and CTe.CON_TERMINAL_ORIGEM = {filtrosPesquisa.CodigoTerminalOrigem}";

            if (filtrosPesquisa.CodigoTerminalDestino > 0)
                sql += $" and CTe.CON_TERMINAL_DESTINO = {filtrosPesquisa.CodigoTerminalDestino}";

            if (filtrosPesquisa.CodigoContainer > 0)
                sql += $" and Container.CTR_CODIGO = {filtrosPesquisa.CodigoContainer}";

            if (!string.IsNullOrWhiteSpace(filtrosPesquisa.NumeroControle))
                sql += $" and CTe.CON_NUMERO_CONTROLE = '{filtrosPesquisa.NumeroControle}'";

            if (!string.IsNullOrWhiteSpace(filtrosPesquisa.NumeroBooking))
                sql += $" and CTe.CON_NUMERO_BOOKING = '{filtrosPesquisa.NumeroBooking}'";

            if (!string.IsNullOrWhiteSpace(filtrosPesquisa.NumeroCE))
                sql += $" and CTe.CON_NUMERO_CE_MERCANTE = '{filtrosPesquisa.NumeroCE}'";

            if (!string.IsNullOrWhiteSpace(filtrosPesquisa.NumeroManifesto))
                sql += $" and CTe.CON_NUMERO_MANIFESTO = '{filtrosPesquisa.NumeroManifesto}'";

            if (!string.IsNullOrWhiteSpace(filtrosPesquisa.NumeroManifestoTransbordo))
                sql += $" and CTe.CON_NUMERO_MANIFESTO_TRANSBORDO = '{filtrosPesquisa.NumeroManifestoTransbordo}'";

            if (filtrosPesquisa.PossuiTransbordo != Dominio.Enumeradores.OpcaoSimNaoPesquisa.Todos && filtrosPesquisa.CodigoCTe == 0)
            {
                if (filtrosPesquisa.PossuiTransbordo == Dominio.Enumeradores.OpcaoSimNaoPesquisa.Sim)
                    sql += $" and CTe.POT_CODIGO_PASSAGEM_UM IS NOT NULL";
                else
                    sql += $" and CTe.POT_CODIGO_PASSAGEM_UM IS NULL";
            }

            if (filtrosPesquisa.ManifestoTransbordo != Dominio.Enumeradores.GeradoPendente.Todos && filtrosPesquisa.CodigoCTe == 0)
            {
                if (filtrosPesquisa.ManifestoTransbordo == Dominio.Enumeradores.GeradoPendente.Gerado)
                    sql += $" and CTe.POT_CODIGO_PASSAGEM_UM IS NOT NULL and CTe.CON_NUMERO_MANIFESTO_TRANSBORDO IS NOT NULL and CTe.CON_NUMERO_MANIFESTO_TRANSBORDO <> ''";
                else
                    sql += $" and CTe.POT_CODIGO_PASSAGEM_UM IS NOT NULL and CTe.CON_NUMERO_MANIFESTO IS NOT NULL and CTe.CON_NUMERO_MANIFESTO <> '' and CTe.CON_NUMERO_CE_MERCANTE IS NOT NULL and CTe.CON_NUMERO_CE_MERCANTE <> '' and (CTe.CON_NUMERO_MANIFESTO_TRANSBORDO IS NULL or CTe.CON_NUMERO_MANIFESTO_TRANSBORDO = '')";
            }

            if (filtrosPesquisa.Manifesto != Dominio.Enumeradores.GeradoPendente.Todos && filtrosPesquisa.CodigoCTe == 0)
            {
                if (filtrosPesquisa.Manifesto == Dominio.Enumeradores.GeradoPendente.Gerado)
                    sql += $" and CTe.CON_NUMERO_MANIFESTO IS NOT NULL and CTe.CON_NUMERO_MANIFESTO <> ''";
                else
                    sql += $" and (CTe.CON_NUMERO_MANIFESTO IS NULL or CTe.CON_NUMERO_MANIFESTO = '')";
            }

            if (filtrosPesquisa.CE != Dominio.Enumeradores.GeradoPendente.Todos && filtrosPesquisa.CodigoCTe == 0)
            {
                if (filtrosPesquisa.CE == Dominio.Enumeradores.GeradoPendente.Gerado)
                    sql += $" and CTe.CON_NUMERO_CE_MERCANTE IS NOT NULL and CTe.CON_NUMERO_CE_MERCANTE <> ''";
                else
                    sql += $" and (CTe.CON_NUMERO_CE_MERCANTE IS NULL or CTe.CON_NUMERO_CE_MERCANTE = '')";
            }

            if (filtrosPesquisa.StatusCTe != null && filtrosPesquisa.StatusCTe.Count > 0)
                sql += " and CTe.CON_STATUS in ('" + string.Join("', '", filtrosPesquisa.StatusCTe) + "')";

            if (filtrosPesquisa.CodigoCTe > 0)
                sql += " and CTe.CON_CODIGO = " + filtrosPesquisa.CodigoCTe;

            if (filtrosPesquisa.CodigoNavioTransbordo > 0)
                sql += $@" and CTe.CON_CODIGO in (  select CargaCTe.CON_CODIGO from T_CARGA_CTE CargaCTe
                                                    inner join T_CARGA_PEDIDO CargaPedido on CargaCTe.CAR_CODIGO = CargaPedido.CAR_CODIGO 
                                                    inner join T_PEDIDO_TRANSBORDO pedidoTransbordo on CargaPedido.PED_CODIGO = pedidoTransbordo.PED_CODIGO 
                                                    where pedidoTransbordo.PVN_CODIGO = {filtrosPesquisa.CodigoNavioTransbordo})";

            if (filtrosPesquisa.TipoCTe != null && filtrosPesquisa.TipoCTe.Count > 0)
                sql += $" and CTe.CON_TIPO_CTE in ({string.Join(", ", filtrosPesquisa.TipoCTe.Select(o => o.ToString("d")))})";

            if (filtrosPesquisa.CodigoPortoTransbordo > 0)
                sql += $@" and (CTe.POT_CODIGO_PASSAGEM_UM = {filtrosPesquisa.CodigoPortoTransbordo} or CTe.POT_CODIGO_PASSAGEM_DOIS = {filtrosPesquisa.CodigoPortoTransbordo}
                                or CTe.POT_CODIGO_PASSAGEM_TRES = {filtrosPesquisa.CodigoPortoTransbordo} or CTe.POT_CODIGO_PASSAGEM_QUATRO = {filtrosPesquisa.CodigoPortoTransbordo}
                                or CTe.POT_CODIGO_PASSAGEM_CINCO = {filtrosPesquisa.CodigoPortoTransbordo})";

            if (filtrosPesquisa.CodigoCarga > 0)
                sql += $@" and exists (SELECT CargaCTe.CON_CODIGO FROM T_CARGA_CTE CargaCTe where CargaCTe.CON_CODIGO = CTe.CON_CODIGO and CargaCTe.CAR_CODIGO = {filtrosPesquisa.CodigoCarga}) ";

            if (filtrosPesquisa.CodigoBalsa > 0)
                sql += " and CTe.NAV_CODIGO_BALSA = " + filtrosPesquisa.CodigoBalsa;

            if (!somenteContarNumeroRegistros && !string.IsNullOrWhiteSpace(propOrdenacao))
            {
                sql += $" order by {propOrdenacao} {dirOrdenacao}";

                if ((inicioRegistros > 0) || (maximoRegistros > 0))
                    sql += $" offset {inicioRegistros} rows fetch next {maximoRegistros} rows only;";
            }

            return sql;
        }

        public string QueryConsultarTituloReceberCTe(Dominio.ObjetosDeValor.Embarcador.Financeiro.FiltroPesquisaCTeTituloReceber filtrosPesquisa, bool somenteContarNumeroRegistros, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametroConsulta = null)
        {
            string sql;

            if (somenteContarNumeroRegistros)
                sql = "select distinct(count(0) over ()) ";
            else
                sql = @"select 
	                            CTe.CON_CODIGO Codigo,
	                            CTe.CON_NUM Numero,
	                            Empresa.EMP_CODIGO CodigoEmpresa,
	                            Empresa.EMP_RAZAO Transportador,
	                            Titulo.TIT_OBSERVACAO Observacao,
	                            Serie.ESE_NUMERO Serie,
	                            (InicioPrestacaoCTe.LOC_DESCRICAO + ' - ' + InicioPrestacaoCTe.UF_SIGLA) Origem,
	                            (FimPrestacaoCTe.LOC_DESCRICAO + ' - ' + FimPrestacaoCTe.UF_SIGLA) Destino,
	                            CTe.CON_DATAHORAEMISSAO DataEmissao,
	                            Titulo.TIT_DATA_VENCIMENTO DataVencimento,
	                            Titulo.TIT_DATA_LIQUIDACAO DataLiquidacao,
                                CTe.CON_VALOR_RECEBER ValorAReceber,
                                ISNULL(Titulo.TIT_STATUS, 1) StatusTitulo,
                                ModeloDocumento.MOD_ABREVIACAO TipoDocumento,
                                ModeloDocumento.MOD_TIPO_DOCUMENTO_CREDITO_DEBITO ModeloDocumento,
                                Titulo.TIT_NUMERO_FATURA NumeroFatura,
                                Titulo.TIT_NUMERO_PAGAMENTO NumeroPagamento,
                                Titulo.TIT_SEQUENCIA SequenciaParcela,
                                Titulo.TIT_VALOR ValorParcelaPaga,
                                CargaOcorrencia.COC_QUANTIDADE_PARCELAS QuantidadeParcelas,
                                Titulo.TIT_SEQUENCIA_PAGA SequenciaParcelaPaga,
                                Titulo.TIT_CODIGO CodigoTitulo";

            sql += @"       from T_CTE CTe 
                            inner join T_EMPRESA Empresa ON Empresa.EMP_CODIGO = CTe.EMP_CODIGO
                            left outer join T_TITULO Titulo ON Titulo.TIT_CODIGO = CTe.TIT_CODIGO
                            left outer join T_EMPRESA_SERIE Serie ON Serie.ESE_CODIGO = CTe.CON_SERIE
                            left outer join T_LOCALIDADES InicioPrestacaoCTe ON InicioPrestacaoCTe.LOC_CODIGO = CTe.CON_LOCINICIOPRESTACAO
                            left outer join T_LOCALIDADES FimPrestacaoCTe ON FimPrestacaoCTe.LOC_CODIGO = CTe.CON_LOCTERMINOPRESTACAO
                            left outer join T_MODDOCFISCAL ModeloDocumento on ModeloDocumento.MOD_CODIGO = CTe.CON_MODELODOC
                            left outer join T_CARGA_CTE_COMPLEMENTO_INFO CTeComplementoInfo ON CTeComplementoInfo.CON_CODIGO = CTe.CON_CODIGO
                            left outer join T_CARGA_OCORRENCIA CargaOcorrencia ON CargaOcorrencia.COC_CODIGO = CTeComplementoInfo.COC_CODIGO
                            where CTe.CON_STATUS = 'A' ";

            string pattern = "yyyy-MM-dd";

            if (filtrosPesquisa.DataInicio != DateTime.MinValue)
                sql += $" and CTe.CON_DATAHORAEMISSAO >= '{filtrosPesquisa.DataInicio.ToString(pattern)}'";

            if (filtrosPesquisa.DataFim != DateTime.MinValue)
                sql += $" and CTe.CON_DATAHORAEMISSAO < '{filtrosPesquisa.DataFim.AddDays(1).ToString(pattern)}'";

            if (filtrosPesquisa.CodigoEmpresa > 0)
                sql += $" and Empresa.EMP_CODIGO = {filtrosPesquisa.CodigoEmpresa}";

            if (filtrosPesquisa.NumeroCTe > 0)
                sql += $" and CTe.CON_NUM = {filtrosPesquisa.NumeroCTe}";

            if (filtrosPesquisa.StatusTitulo != Dominio.ObjetosDeValor.Embarcador.Enumeradores.StatusTitulo.Todos)
            {
                if (filtrosPesquisa.StatusTitulo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.StatusTitulo.Atrazada)
                    sql += $" and CTe.TIT_CODIGO is null";
                else if (filtrosPesquisa.StatusTitulo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.StatusTitulo.EmAberto)
                    sql += $" and (CTe.TIT_CODIGO is null or Titulo.TIT_STATUS = {filtrosPesquisa.StatusTitulo.ToString("d")})";
                else
                    sql += $" and Titulo.TIT_STATUS = {filtrosPesquisa.StatusTitulo.ToString("d")}";
            }

            if (filtrosPesquisa.TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiCTe)
            {
                sql += $@" and CTe.CON_CODIGO NOT IN(
                                      SELECT CCI.CON_CODIGO FROM T_CARGA_CTE_COMPLEMENTO_INFO CCI
                                        JOIN T_CARGA_OCORRENCIA CO ON CO.COC_CODIGO = CCI.COC_CODIGO
                                        JOIN T_OCORRENCIA O ON O.OCO_CODIGO = CO.OCO_CODIGO
                                        WHERE O.OCO_BLOQUEAR_VISUALIZACAO_PORTAL_TRANSPORTADOR = 1 and CON_CODIGO is not null
                                    )";
            }

            if (!somenteContarNumeroRegistros)
            {
                sql += $" order by {parametroConsulta.PropriedadeOrdenar} {parametroConsulta.DirecaoOrdenar}";

                if ((parametroConsulta.InicioRegistros > 0) || (parametroConsulta.LimiteRegistros > 0))
                    sql += $" offset {parametroConsulta.InicioRegistros} rows fetch next {parametroConsulta.LimiteRegistros} rows only;";
            }

            return sql;
        }


        public string QueryConsultarDocumentosConciliacao(Dominio.ObjetosDeValor.Embarcador.Financeiro.FiltroPesquisaDocumentosConciliacao filtrosPesquisa, bool somenteContarNumeroRegistros, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametroConsulta = null)
        {
            string sql;

            if (somenteContarNumeroRegistros)
                sql = "select distinct(count(0) over ()) ";
            else
                sql = @"select 
	                            CTe.CON_CODIGO Codigo,
                                CTe.CON_NUM Numero,
                                Empresa.EMP_CODIGO CodigoEmpresa,
                                Empresa.EMP_RAZAO Transportador,
		                        Empresa.EMP_CNPJ CNPJTransportador,
                                Titulo.TIT_ACRESCIMO ValorAcrescimo,
		                        Titulo.TIT_DESCONTO ValorDecrescimo,
                                Titulo.TIT_VALOR_PAGO ValorLiquidacao,
                                Carga.CAR_SITUACAO SituacaoCarga,
                                CTe.CON_STATUS Status,
                                CAST(CAST(ROUND(Titulo.TIT_ACRESCIMO, 2, 1) AS DECIMAL(18,2)) AS VARCHAR(30)) Acrescimos,
		                        CAST(CAST(ROUND(Titulo.TIT_DESCONTO, 2, 1) AS DECIMAL(18,2)) AS VARCHAR(30)) Decrescimos, 
                                Serie.ESE_NUMERO Serie,
                                IsNull(substring((SELECT DISTINCT ', ' + CAST(Fatura.FAT_NUMERO AS VARCHAR(20))
                                        FROM T_FATURA Fatura
                                        JOIN T_FATURA_DOCUMENTO FaturaDocumento ON FaturaDocumento.FAT_CODIGO = Fatura.FAT_CODIGO
                                        JOIN T_DOCUMENTO_FATURAMENTO DocumentoFaturamento ON DocumentoFaturamento.DFA_CODIGO = FaturaDocumento.DFA_CODIGO
                                        WHERE DocumentoFaturamento.CON_CODIGO = CTe.CON_CODIGO FOR XML path('')), 3, 1000), titulo.TIT_NUMERO_FATURA) NumeroFatura,
                                ModeloDocumento.MOD_TIPO_DOCUMENTO_EMISSAO TipoDocumentoEmissao,
		                        Carga.CAR_CODIGO_CARGA_EMBARCADOR NumeroCarga,
		                        Filial.FIL_DESCRICAO Filial,
                                CTe.CON_DATAHORAEMISSAO DataEmissao,
                                Titulo.TIT_DATA_VENCIMENTO DataVencimento,
                                Titulo.TIT_DATA_LIQUIDACAO DataLiquidacao,
                                CTe.CON_VALOR_RECEBER ValorAReceber,
                                ISNULL(Titulo.TIT_STATUS, 1) StatusTitulo,
		                        Integracao.IMT_DATA_CONSULTA DataConsulta";

            sql += @" from
                        T_CTE CTe inner join T_EMPRESA Empresa 
                                ON Empresa.EMP_CODIGO = CTe.EMP_CODIGO
                        inner join T_CONCILIACAO_TRANSPORTADOR Conciliacao 
                                ON Conciliacao.COT_CODIGO = CTe.COT_CODIGO
                        left join T_CTE_PARTICIPANTE RemetenteCTe 
                                on CTe.CON_REMETENTE_CTE = RemetenteCTe.PCT_CODIGO
                        left join T_CLIENTE ClienteRemetente 
                                on ClienteRemetente.CLI_CGCCPF = RemetenteCTe.CLI_CODIGO
                        left outer join T_TITULO Titulo 
                                ON Titulo.TIT_CODIGO = CTe.TIT_CODIGO
                        left outer join T_MODDOCFISCAL ModeloDocumento 
                                ON ModeloDocumento.MOD_CODIGO = CTe.CON_MODELODOC 
	                    left outer join T_CARGA_CTE CargaCte 
                                ON CargaCte.CON_CODIGO = CTe.CON_CODIGO 
	                    left outer join T_CARGA Carga 
                                ON Carga.CAR_CODIGO = CargaCte.CAR_CODIGO
                        left outer join T_FILIAL Filial 
                                ON Filial.FIL_CODIGO = Carga.FIL_CODIGO
                        left outer join T_EMPRESA_SERIE Serie 
                                ON Serie.ESE_CODIGO = CTe.CON_SERIE
                        left outer join T_INTEGRACAO_MARFRIG_CTE_TITULOS_RECEBER Integracao
			                    ON Integracao.CON_CODIGO = CTe.CON_CODIGO 
                            where CTe.CON_STATUS in ( 'P' ,'E' , 'R' ,'A' , 'C' , 'I' , 'D' , 'S' , 'K' , 'L' , 'Z' , 'X' , 'V' , 'B' , 'M' , 'F' , 'Q' , 'Y' , 'N' ) ";

            string pattern = "yyyy-MM-dd";

            if (filtrosPesquisa.CnpjCpfRemetente > 0d)
            {
                sql += $" and ClienteRemetente.CLI_CGCCPF = " + filtrosPesquisa.CnpjCpfRemetente.ToString("F0");
            }

            if (filtrosPesquisa.CodigoFilial > 0)
            {
                sql += $@" and CTe.CON_CODIGO in (select CargaCTe.CON_CODIGO from T_CARGA_CTE CargaCTe 
                                                        inner join T_CARGA Carga on CargaCTe.CAR_CODIGO_ORIGEM = Carga.CAR_CODIGO 
                                                        where Carga.FIL_CODIGO = {filtrosPesquisa.CodigoFilial})";
            }

            if (filtrosPesquisa.NumeroFatura > 0)
            {
                sql += $@" and ( titulo.TIT_NUMERO_FATURA = {filtrosPesquisa.NumeroFatura}
                                  or
                                    CTe.CON_CODIGO in (
                                      select 
                                        DocumentoFaturamento.CON_CODIGO 
                                      FROM 
                                        T_DOCUMENTO_FATURAMENTO DocumentoFaturamento
                                        JOIN T_FATURA_DOCUMENTO FaturaDocumento ON FaturaDocumento.DFA_CODIGO = DocumentoFaturamento.DFA_CODIGO 
                                        JOIN T_FATURA Fatura on FaturaDocumento.FAT_CODIGO = Fatura.FAT_CODIGO
                                      WHERE 
                                        DocumentoFaturamento.TIT_CODIGO = titulo.TIT_CODIGO and Fatura.FAT_NUMERO = {filtrosPesquisa.NumeroFatura}
                                    ) 
                                  )";
            }

            if (filtrosPesquisa.DataInicio != DateTime.MinValue)
                sql += $" and CTe.CON_DATAHORAEMISSAO >= '{filtrosPesquisa.DataInicio.ToString(pattern)}'";

            if (filtrosPesquisa.DataFim != DateTime.MinValue)
                sql += $" and CTe.CON_DATAHORAEMISSAO < '{filtrosPesquisa.DataFim.AddDays(1).ToString(pattern)}'";

            if (filtrosPesquisa.CodigoEmpresa > 0)
                sql += $" and Empresa.EMP_CODIGO = {filtrosPesquisa.CodigoEmpresa}";

            if (!string.IsNullOrEmpty(filtrosPesquisa.CodigoCargaEmbarcador))
                sql += $" and Carga.CAR_CODIGO_CARGA_EMBARCADOR = '{filtrosPesquisa.CodigoCargaEmbarcador}'";

            if (filtrosPesquisa.NumeroCTe > 0)
                sql += $" and CTe.CON_NUM = {filtrosPesquisa.NumeroCTe}";

            if (filtrosPesquisa.StatusTitulo != Dominio.ObjetosDeValor.Embarcador.Enumeradores.StatusTitulo.Todos)
            {
                if (filtrosPesquisa.StatusTitulo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.StatusTitulo.Atrazada)
                    sql += $" and CTe.TIT_CODIGO is null";
                else if (filtrosPesquisa.StatusTitulo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.StatusTitulo.EmAberto)
                    sql += $" and (CTe.TIT_CODIGO is null or Titulo.TIT_STATUS = {filtrosPesquisa.StatusTitulo.ToString("d")})";
                else
                    sql += $" and Titulo.TIT_STATUS = {filtrosPesquisa.StatusTitulo.ToString("d")}";
            }

            if (filtrosPesquisa.TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiCTe)
            {
                sql += $@" and CTe.CON_CODIGO NOT IN(
                                      SELECT CCI.CON_CODIGO FROM T_CARGA_CTE_COMPLEMENTO_INFO CCI
                                        JOIN T_CARGA_OCORRENCIA CO ON CO.COC_CODIGO = CCI.COC_CODIGO
                                        JOIN T_OCORRENCIA O ON O.OCO_CODIGO = CO.OCO_CODIGO
                                        WHERE O.OCO_BLOQUEAR_VISUALIZACAO_PORTAL_TRANSPORTADOR = 1 and CON_CODIGO is not null
                                    )";
            }

            if (!somenteContarNumeroRegistros)
            {
                if (parametroConsulta.PropriedadeOrdenar == "DescricaoTipoDocumento")
                    parametroConsulta.PropriedadeOrdenar = "TipoDocumentoEmissao";
                else if (parametroConsulta.PropriedadeOrdenar == "ValorLiquidacaoFormatado")
                    parametroConsulta.PropriedadeOrdenar = "ValorLiquidacao";
                else if (parametroConsulta.PropriedadeOrdenar == "ValorAReceberFormatado")
                    parametroConsulta.PropriedadeOrdenar = "ValorAReceber";
                else if (parametroConsulta.PropriedadeOrdenar == "ValorOriginalFormatado")
                    parametroConsulta.PropriedadeOrdenar = "ValorAReceber";

                sql += $" order by {parametroConsulta.PropriedadeOrdenar} {parametroConsulta.DirecaoOrdenar}";

                if ((parametroConsulta.InicioRegistros > 0) || (parametroConsulta.LimiteRegistros > 0))
                    sql += $" offset {parametroConsulta.InicioRegistros} rows fetch next {parametroConsulta.LimiteRegistros} rows only;";
            }

            return sql;
        }


        public Dominio.Entidades.ConhecimentoDeTransporteEletronico BuscarPorNumeroSerieEEmpresa(int numero, int serie, int codigoEmpresa)
        {
            var consultaConhecimentoDeTransporteEletronico = this.SessionNHiBernate.Query<Dominio.Entidades.ConhecimentoDeTransporteEletronico>();
            consultaConhecimentoDeTransporteEletronico = consultaConhecimentoDeTransporteEletronico.Where(o => o.Numero == numero && o.Empresa.Codigo == codigoEmpresa);
            if (serie > 0)
                consultaConhecimentoDeTransporteEletronico = consultaConhecimentoDeTransporteEletronico.Where(o => o.Serie.Numero == serie);

            consultaConhecimentoDeTransporteEletronico = consultaConhecimentoDeTransporteEletronico.Where(o => o.ModeloDocumentoFiscal.Numero != "57" && o.ModeloDocumentoFiscal.Numero != "55" && o.ModeloDocumentoFiscal.Numero != "58");

            return consultaConhecimentoDeTransporteEletronico.FirstOrDefault();
        }

        public Dominio.Entidades.ConhecimentoDeTransporteEletronico BuscarPorNumeroModeloSerieEEmpresa(int numero, string modeloDocumentoFiscal, int serie, int codigoEmpresa)
        {
            var consultaConhecimentoDeTransporteEletronico = this.SessionNHiBernate.Query<Dominio.Entidades.ConhecimentoDeTransporteEletronico>();
            consultaConhecimentoDeTransporteEletronico = consultaConhecimentoDeTransporteEletronico.Where(o => o.Numero == numero && o.Empresa.Codigo == codigoEmpresa);
            if (serie > 0)
                consultaConhecimentoDeTransporteEletronico = consultaConhecimentoDeTransporteEletronico.Where(o => o.Serie.Numero == serie);

            consultaConhecimentoDeTransporteEletronico = consultaConhecimentoDeTransporteEletronico.Where(o => o.ModeloDocumentoFiscal.Numero == modeloDocumentoFiscal);

            return consultaConhecimentoDeTransporteEletronico.FirstOrDefault();
        }

        #endregion

        #region Métodos Privados Relatório Tomadores

        private string ObterSelectConsultaRelatorioTomadores(bool count, List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedades, DateTime dataCadastroInicial, DateTime dataCadastroFinal, double cpfCnpjTomador, int codigoGrupoPessoas, string propAgrupa, string dirAgrupa, string propOrdena, string dirOrdena, int inicio, int limite)
        {
            string select = "Tomador.CLI_FISJUR TipoPessoa, ",
                   groupBy = "Tomador.CLI_FISJUR, ",
                   joins = string.Empty,
                   where = string.Empty,
                   orderBy = string.Empty;

            for (var i = propriedades.Count - 1; i >= 0; i--)
                SetarSelectConsultaRelatorioTomadores(propriedades[i].Propriedade, propriedades[i].CodigoDinamico, ref select, ref groupBy, ref joins, count);

            SetarWhereConsultaRelatorioTomadores(ref where, ref groupBy, ref joins, dataCadastroInicial, dataCadastroFinal, cpfCnpjTomador, codigoGrupoPessoas);

            if (!count)
            {
                if (!string.IsNullOrWhiteSpace(propAgrupa))
                {
                    SetarSelectConsultaRelatorioTomadores(propAgrupa, 0, ref select, ref groupBy, ref joins, count);

                    if (select.Contains(propAgrupa))
                        orderBy = propAgrupa + " " + dirAgrupa;
                }

                if (!string.IsNullOrWhiteSpace(propOrdena))
                {
                    if (propOrdena != propAgrupa && select.Contains(propOrdena))
                        orderBy += (orderBy.Length > 0 ? ", " : string.Empty) + propOrdena + " " + dirOrdena;
                }
            }

            return (count ? "select distinct(count(0) over ())" : "select " + (select.Length > 0 ? select.Substring(0, select.Length - 2) : string.Empty)) +
                   " from T_CTE CTe inner join T_CTE_PARTICIPANTE ParticipanteTomador on CTe.CON_TOMADOR_PAGADOR_CTE = ParticipanteTomador.PCT_CODIGO inner join T_CLIENTE Tomador on Tomador.CLI_CGCCPF = ParticipanteTomador.CLI_CODIGO " + joins +
                   " where 1=1" + where +
                   (groupBy.Length > 0 ? " group by " + groupBy.Substring(0, groupBy.Length - 2) : string.Empty) +
                   (count ? string.Empty : (orderBy.Length > 0 ? " order by " + orderBy : " order by 1 asc ")) +
                   (count || (inicio <= 0 && limite <= 0) ? "" : " offset " + inicio.ToString() + " rows fetch next " + limite.ToString() + " rows only;");
        }

        private void SetarWhereConsultaRelatorioTomadores(ref string where, ref string groupBy, ref string joins, DateTime dataCadastroInicial, DateTime dataCadastroFinal, double cpfCnpjTomador, int codigoGrupoPessoas)
        {
            if (dataCadastroInicial != DateTime.MinValue)
                where += " and Tomador.CLI_DATACAD >= '" + dataCadastroInicial.ToString("yyyy-MM-dd") + "'";

            if (dataCadastroFinal != DateTime.MinValue)
                where += " and Tomador.CLI_DATACAD < '" + dataCadastroFinal.AddDays(1).ToString("yyyy-MM-dd") + "'";

            if (cpfCnpjTomador > 0d)
                where += " and Tomador.CLI_CGCCPF = " + cpfCnpjTomador.ToString();

            if (codigoGrupoPessoas > 0)
                where += " and Tomador.GRP_CODIGO = " + codigoGrupoPessoas.ToString();
        }

        private void SetarSelectConsultaRelatorioTomadores(string propriedade, int codigoDinamico, ref string select, ref string groupBy, ref string joins, bool count)
        {
            switch (propriedade)
            {
                case "CPFCNPJFormatado":
                    if (!select.Contains(" CPFCNPJ"))
                    {
                        select += "Tomador.CLI_CGCCPF CPFCNPJ, ";

                        if (!groupBy.Contains("Tomador.CLI_CGCCPF"))
                            groupBy += "Tomador.CLI_CGCCPF, ";
                    }
                    break;
                case "InscricaoEstadual":
                    if (!select.Contains(" InscricaoEstadual"))
                    {
                        select += "Tomador.CLI_IERG InscricaoEstadual, ";

                        if (!groupBy.Contains("Tomador.CLI_IERG"))
                            groupBy += "Tomador.CLI_IERG, ";
                    }
                    break;
                case "RazaoSocial":
                    if (!select.Contains(" RazaoSocial"))
                    {
                        select += "Tomador.CLI_NOME RazaoSocial, ";

                        if (!groupBy.Contains("Tomador.CLI_NOME"))
                            groupBy += "Tomador.CLI_NOME, ";
                    }
                    break;
                case "NomeFantasia":
                    if (!select.Contains(" NomeFantasia"))
                    {
                        select += "Tomador.CLI_NOMEFANTASIA NomeFantasia, ";

                        if (!groupBy.Contains("Tomador.CLI_NOMEFANTASIA"))
                            groupBy += "Tomador.CLI_NOMEFANTASIA, ";
                    }
                    break;
                case "Telefone":
                    if (!select.Contains(" Telefone"))
                    {
                        select += "Tomador.CLI_FONE Telefone, ";

                        if (!groupBy.Contains("Tomador.CLI_FONE"))
                            groupBy += "Tomador.CLI_FONE, ";
                    }
                    break;
                case "CEP":
                    if (!select.Contains(" CEP"))
                    {
                        select += "Tomador.CLI_CEP CEP, ";

                        if (!groupBy.Contains("Tomador.CLI_CEP"))
                            groupBy += "Tomador.CLI_CEP, ";
                    }
                    break;
                case "Endereco":
                    if (!select.Contains(" Endereco"))
                    {
                        select += "Tomador.CLI_ENDERECO Endereco, ";

                        if (!groupBy.Contains("Tomador.CLI_ENDERECO"))
                            groupBy += "Tomador.CLI_ENDERECO, ";
                    }
                    break;
                case "Bairro":
                    if (!select.Contains(" Bairro"))
                    {
                        select += "Tomador.CLI_BAIRRO Bairro, ";

                        if (!groupBy.Contains("Tomador.CLI_BAIRRO"))
                            groupBy += "Tomador.CLI_BAIRRO, ";
                    }
                    break;
                case "Complemento":
                    if (!select.Contains(" Complemento"))
                    {
                        select += "Tomador.CLI_COMPLEMENTO Complemento, ";

                        if (!groupBy.Contains("Tomador.CLI_COMPLEMENTO"))
                            groupBy += "Tomador.CLI_COMPLEMENTO, ";
                    }
                    break;
                case "Cidade":
                    if (!select.Contains(" Cidade,"))
                    {
                        select += "CidadeTomador.LOC_DESCRICAO Cidade, ";

                        if (!groupBy.Contains("CidadeTomador.LOC_DESCRICAO"))
                            groupBy += "CidadeTomador.LOC_DESCRICAO, ";

                        if (!joins.Contains(" CidadeTomador "))
                            joins += " left outer join T_LOCALIDADES CidadeTomador on Tomador.LOC_CODIGO = CidadeTomador.LOC_CODIGO ";
                    }
                    break;
                case "Estado":
                    if (!select.Contains(" Estado"))
                    {
                        select += "CidadeTomador.UF_SIGLA Estado, ";

                        if (!groupBy.Contains("CidadeTomador.UF_SIGLA"))
                            groupBy += "CidadeTomador.UF_SIGLA, ";

                        if (!joins.Contains(" CidadeTomador "))
                            joins += " left outer join T_LOCALIDADES CidadeTomador on Tomador.LOC_CODIGO = CidadeTomador.LOC_CODIGO ";
                    }
                    break;
                case "DataCadastro":
                    if (!select.Contains(" DataCadastro"))
                    {
                        select += "Tomador.CLI_DATACAD DataCadastro, ";

                        if (!groupBy.Contains("Tomador.CLI_DATACAD"))
                            groupBy += "Tomador.CLI_DATACAD, ";
                    }
                    break;
                case "Email":
                    if (!select.Contains(" Email,"))
                    {
                        select += "Tomador.CLI_EMAIL Email, ";

                        if (!groupBy.Contains("Tomador.CLI_EMAIL,"))
                            groupBy += "Tomador.CLI_EMAIL, ";
                    }
                    break;
                case "SituacaoEmail":
                    if (!select.Contains(" SituacaoEmail,"))
                    {
                        select += "CASE Tomador.CLI_EMAIL_STATUS WHEN 'A' THEN 'Ativo' ELSE 'Inativo' END SituacaoEmail, ";

                        if (!groupBy.Contains("Tomador.CLI_EMAIL_STATUS,"))
                            groupBy += "Tomador.CLI_EMAIL_STATUS, ";
                    }
                    break;
                case "GrupoPessoas":
                    if (!select.Contains(" GrupoPessoas"))
                    {
                        select += "GrupoTomador.GRP_DESCRICAO GrupoPessoas, ";

                        if (!groupBy.Contains("GrupoTomador.GRP_DESCRICAO"))
                            groupBy += "GrupoTomador.GRP_DESCRICAO, ";

                        if (!joins.Contains(" GrupoTomador "))
                            joins += " left outer join T_GRUPO_PESSOAS GrupoTomador on GrupoTomador.GRP_CODIGO = Tomador.GRP_CODIGO ";
                    }
                    break;
                case "EmailGrupo":
                    if (!select.Contains(" EmailGrupo,"))
                    {
                        select += "GrupoTomador.GRP_EMAIL EmailGrupo, ";

                        if (!groupBy.Contains("GrupoTomador.GRP_EMAIL,"))
                            groupBy += "GrupoTomador.GRP_EMAIL, ";

                        if (!joins.Contains(" GrupoTomador "))
                            joins += " left outer join T_GRUPO_PESSOAS GrupoTomador on GrupoTomador.GRP_CODIGO = Tomador.GRP_CODIGO ";
                    }
                    break;
                case "SituacaoEmailGrupo":
                    if (!select.Contains(" SituacaoEmailGrupo,"))
                    {
                        select += "CASE GrupoTomador.GRP_ENVIAR_XML_CTE_EMAIL WHEN 1 THEN 'Ativo' ELSE 'Inativo' END SituacaoEmailGrupo, ";

                        if (!groupBy.Contains("GrupoTomador.GRP_ENVIAR_XML_CTE_EMAIL"))
                            groupBy += "GrupoTomador.GRP_ENVIAR_XML_CTE_EMAIL, ";

                        if (!joins.Contains(" GrupoTomador "))
                            joins += " left outer join T_GRUPO_PESSOAS GrupoTomador on GrupoTomador.GRP_CODIGO = Tomador.GRP_CODIGO ";
                    }
                    break;
                default:
                    break;
            }
        }

        #endregion

        #region Métodos Privados Relatório de Faturamento por Grupo de Pessoas

        private SQLDinamico ObterSelectConsultaRelatorioFaturamentoPorGrupoPessoas(bool count, Dominio.ObjetosDeValor.Embarcador.CTe.FiltroPesquisaRelatorioFaturamentoPorGrupoPessoas filtrosPesquisa, List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedadesAgrupamento, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            string select = string.Empty,
                   selectExterno = string.Empty,
                   groupBy = string.Empty,
                   groupByExterno = string.Empty,
                   joins = string.Empty,
                   where = string.Empty,
                   orderBy = string.Empty;

            var parametros = new List<ParametroSQL>();

            for (var i = propriedadesAgrupamento.Count - 1; i >= 0; i--)
                SetarSelectRelatorioFaturamentoPorGrupoPessoas(propriedadesAgrupamento[i].Propriedade, propriedadesAgrupamento[i].CodigoDinamico, ref select, ref selectExterno, ref groupBy, ref groupByExterno, ref joins, count);

            SetarWhereRelatorioFaturamentoPorGrupoPessoas(ref where, ref joins, ref parametros, filtrosPesquisa);

            if (!string.IsNullOrWhiteSpace(parametrosConsulta?.PropriedadeAgrupar))
            {
                SetarSelectRelatorioFaturamentoPorGrupoPessoas(parametrosConsulta?.PropriedadeAgrupar, 0, ref select, ref selectExterno, ref groupBy, ref groupByExterno, ref joins, count);

                orderBy = " ORDER BY " + parametrosConsulta?.PropriedadeAgrupar + " " + parametrosConsulta?.DirecaoAgrupar;
            }

            if (!string.IsNullOrWhiteSpace(parametrosConsulta?.PropriedadeOrdenar))
            {
                if (parametrosConsulta.PropriedadeOrdenar != parametrosConsulta?.PropriedadeAgrupar)
                    orderBy += (orderBy.Length <= 0 ? " ORDER BY " : ", ") + parametrosConsulta?.PropriedadeOrdenar + " " + parametrosConsulta?.DirecaoOrdenar;
            }

            string query = string.Empty;

            query = "WITH Dados AS (SELECT " + (select.Length > 0 ? select.Substring(0, select.Length - 2) : string.Empty) +
                   " FROM T_CTE CTe LEFT JOIN T_CTE_PARTICIPANTE Tomador on CTe.CON_TOMADOR_PAGADOR_CTE = Tomador.PCT_CODIGO LEFT JOIN T_CLIENTE Cliente on Tomador.CLI_CODIGO = Cliente.CLI_CGCCPF LEFT JOIN T_GRUPO_PESSOAS GrupoPessoas on Tomador.GRP_CODIGO = GrupoPessoas.GRP_CODIGO LEFT JOIN T_MODDOCFISCAL ModeloDocumento on ModeloDocumento.MOD_CODIGO = CTe.CON_MODELODOC " + joins +
                   " WHERE CTe.CON_STATUS = 'A' AND CTe.CON_TIPO_CTE <> 2 " + where +
                   " GROUP BY " + (groupBy.Length > 0 ? groupBy.Substring(0, groupBy.Length - 2) : "GrupoPessoas.GRP_DESCRICAO") + ")";

            string queryExterna = string.Empty;

            queryExterna = "SELECT " + (selectExterno.Length > 0 ? selectExterno.Substring(0, selectExterno.Length - 2) : string.Empty) +
                           " FROM Dados GROUP BY " + (groupByExterno.Length > 0 ? groupByExterno.Substring(0, groupByExterno.Length - 2) : "GrupoPessoas") +
                            (count ? "" : orderBy) +
                            (count || (parametrosConsulta.InicioRegistros <= 0 && parametrosConsulta.LimiteRegistros <= 0) ? "" : " OFFSET " + parametrosConsulta.InicioRegistros.ToString() + " ROWS FETCH NEXT " + parametrosConsulta.LimiteRegistros.ToString() + " ROWS ONLY;");

            if (count)
                queryExterna = "SELECT COUNT (1) FROM (" + queryExterna + ") AS DadosExtraidos";

            return new SQLDinamico(query + queryExterna, parametros);
        }

        private void SetarSelectRelatorioFaturamentoPorGrupoPessoas(string propriedade, int codigoDinamico, ref string select, ref string selectExterno, ref string groupBy, ref string groupByExterno, ref string joins, bool count)
        {
            switch (propriedade)
            {
                case "GrupoPessoas":
                    if (!select.Contains("GrupoPessoas"))
                    {
                        select += "GrupoPessoas.GRP_DESCRICAO GrupoPessoas, ";
                        groupBy += "GrupoPessoas.GRP_DESCRICAO, ";

                        selectExterno += "GrupoPessoas, ";
                        groupByExterno += "GrupoPessoas, ";
                    }
                    break;
                case "Veiculo":
                    if (!select.Contains(" Veiculo "))
                    {
                        select += "substring((select ', ' + veiculo1.VEI_PLACA from T_CTE_VEICULO veiculoCTe1 inner join T_VEICULO veiculo1 on veiculoCTe1.VEI_CODIGO = veiculo1.VEI_CODIGO where veiculoCTe1.CON_CODIGO = CTe.CON_CODIGO for xml path('')), 3, 1000) Veiculo, ";

                        if (!groupBy.Contains("CTe.CON_CODIGO"))
                            groupBy += "CTe.CON_CODIGO, ";

                        selectExterno += "Veiculo, ";
                        groupByExterno += "Veiculo, ";
                    }
                    break;
                case "PropriedadeVeiculo":
                    if (!select.Contains(" PropriedadeVeiculo "))
                    {
                        select += "(select top(1) CASE veiculoCTe1.CVE_TIPO_PROPRIEDADE WHEN 'T' THEN 'Terceiros' WHEN 'P' THEN 'Próprio' ELSE 'Não informado' END PropriedadeVeiculo from T_CTE_VEICULO veiculoCTe1 where veiculoCTe1.CVE_TIPO_VEICULO = '0' and veiculoCTe1.CON_CODIGO = CTe.CON_CODIGO) PropriedadeVeiculo, ";

                        if (!groupBy.Contains("CTe.CON_CODIGO"))
                            groupBy += "CTe.CON_CODIGO, ";

                        selectExterno += "PropriedadeVeiculo, ";
                        groupByExterno += "PropriedadeVeiculo, ";
                    }
                    break;
                case "ProprietarioVeiculo":
                    if (!select.Contains(" ProprietarioVeiculo "))
                    {
                        select += "(select top(1) proprietario1.PVE_NOME from T_CTE_VEICULO veiculoCTe1 inner join T_CTE_VEICULO_PROPRIETARIO proprietario1 on proprietario1.PVE_CODIGO = veiculoCTe1.PVE_CODIGO where veiculoCTe1.CVE_TIPO_VEICULO = '0' and veiculoCTe1.CON_CODIGO = CTe.CON_CODIGO) ProprietarioVeiculo, ";

                        if (!groupBy.Contains("CTe.CON_CODIGO"))
                            groupBy += "CTe.CON_CODIGO, ";

                        selectExterno += "ProprietarioVeiculo, ";
                        groupByExterno += "ProprietarioVeiculo, ";
                    }
                    break;
                case "Transportador":
                    if (!select.Contains("Transportador"))
                    {
                        select += "Transportador.EMP_FANTASIA Transportador, ";
                        groupBy += "Transportador.EMP_FANTASIA, ";

                        if (!joins.Contains(" Transportador "))
                            joins += "inner join T_EMPRESA Transportador on Transportador.EMP_CODIGO = CTe.EMP_CODIGO ";

                        selectExterno += "Transportador, ";
                        groupByExterno += "Transportador, ";
                    }
                    break;
                case "InicioPrestacao":
                    if (!select.Contains("InicioPrestacao"))
                    {
                        select += "InicioPrestacao.UF_SIGLA + ' - ' + InicioPrestacao.LOC_DESCRICAO InicioPrestacao, ";
                        groupBy += "InicioPrestacao.UF_SIGLA, InicioPrestacao.LOC_DESCRICAO, ";

                        if (!joins.Contains(" InicioPrestacao "))
                            joins += "inner join T_LOCALIDADES InicioPrestacao on InicioPrestacao.LOC_CODIGO = CTe.CON_LOCINICIOPRESTACAO ";

                        selectExterno += "InicioPrestacao, ";
                        groupByExterno += "InicioPrestacao, ";
                    }
                    break;
                case "FimPrestacao":
                    if (!select.Contains("FimPrestacao"))
                    {
                        select += "FimPrestacao.UF_SIGLA + ' - ' + FimPrestacao.LOC_DESCRICAO FimPrestacao, ";
                        groupBy += "FimPrestacao.UF_SIGLA, FimPrestacao.LOC_DESCRICAO, ";

                        if (!joins.Contains(" FimPrestacao "))
                            joins += "inner join T_LOCALIDADES FimPrestacao on FimPrestacao.LOC_CODIGO = CTe.CON_LOCTERMINOPRESTACAO ";

                        selectExterno += "FimPrestacao, ";
                        groupByExterno += "FimPrestacao, ";
                    }
                    break;
                case "Remetente":
                    if (!select.Contains("Remetente"))
                    {
                        select += "Remetente.PCT_NOME Remetente, ";
                        groupBy += "Remetente.PCT_NOME, ";

                        if (!joins.Contains(" Remetente "))
                            joins += "inner join T_CTE_PARTICIPANTE Remetente on CTe.CON_REMETENTE_CTE = Remetente.PCT_CODIGO ";

                        selectExterno += "Remetente, ";
                        groupByExterno += "Remetente, ";
                    }
                    break;
                case "Destinatario":
                    if (!select.Contains("Destinatario"))
                    {
                        select += "Destinatario.PCT_NOME Destinatario, ";
                        groupBy += "Destinatario.PCT_NOME, ";

                        if (!joins.Contains(" Destinatario "))
                            joins += "inner join T_CTE_PARTICIPANTE Destinatario on CTe.CON_DESTINATARIO_CTE = Destinatario.PCT_CODIGO ";

                        selectExterno += "Destinatario, ";
                        groupByExterno += "Destinatario, ";
                    }
                    break;
                case "QuantidadeCTes":
                    if (!count && !select.Contains("QuantidadeCTes"))
                    {
                        select += "COUNT(CTe.CON_CODIGO) QuantidadeCTes, ";
                        selectExterno += "SUM(QuantidadeCTes) QuantidadeCTes, ";
                    }
                    break;
                case "ValorFrete":
                    if (!count && !select.Contains("ValorFrete"))
                    {
                        select += "SUM(CTe.CON_VALOR_FRETE) ValorFrete, ";
                        selectExterno += "SUM(ValorFrete) ValorFrete, ";
                    }
                    break;
                case "ValorServico":
                    if (!count && !select.Contains("ValorServico"))
                    {
                        select += "SUM(CTe.CON_VALOR_PREST_SERVICO) ValorServico, ";
                        selectExterno += "SUM(ValorServico) ValorServico, ";
                    }
                    break;
                case "ValorReceber":
                    if (!count && !select.Contains("ValorReceber"))
                    {
                        select += "SUM(CTe.CON_VALOR_RECEBER) ValorReceber, ";
                        selectExterno += "SUM(ValorReceber) ValorReceber, ";
                    }
                    break;
                case "ValorICMS":
                    if (!count && !select.Contains("ValorICMS"))
                    {
                        select += "SUM(CTe.CON_VAL_ICMS) ValorICMS, ";
                        selectExterno += "SUM(ValorICMS) ValorICMS, ";
                    }
                    break;
                case "SemImposto":
                    if (!count && !select.Contains("SemImposto"))
                    {
                        select += "SUM(CTe.CON_VALOR_RECEBER) - ISNULL(SUM(CTe.CON_VAL_ICMS), 0) SemImposto, ";
                        selectExterno += "SUM(SemImposto) SemImposto, ";
                    }
                    break;
                case "Volumes":
                    if (!count && !select.Contains("Volumes"))
                    {
                        select += " CONVERT(int, SUM(CTe.CON_VOLUMES)) Volumes, ";
                        selectExterno += "SUM(Volumes) Volumes, ";
                    }
                    break;
                case "PesoDigitado":
                    if (!count && !select.Contains("PesoDigitado"))
                    {
                        select += " SUM(CTe.CON_PESO) PesoDigitado, ";
                        selectExterno += "SUM(PesoDigitado) PesoDigitado, ";
                    }
                    break;
                case "PesoFaturado":
                    if (!count && !select.Contains("PesoFaturado"))
                    {
                        select += " SUM(CTe.CON_PESO_FATURADO) PesoFaturado, ";
                        selectExterno += "SUM(PesoFaturado) PesoFaturado, ";
                    }
                    break;
                case "VlrFretePesFat":
                    if (!count && !select.Contains("VlrFretePesFat"))
                    {
                        select += " SUM(CTe.CON_VALOR_RECEBER) / CASE WHEN ISNULL(SUM(CTe.CON_PESO_FATURADO), 0) > 0 THEN SUM(CTe.CON_PESO_FATURADO) ELSE 1 END VlrFretePesFat, ";
                        selectExterno += "SUM(VlrFretePesFat) VlrFretePesFat, ";
                    }
                    break;
                case "ValorMercadoria":
                    if (!count && !select.Contains("ValorMercadoria"))
                    {
                        select += "SUM(CTe.CON_VALOR_TOTAL_MERC) ValorMercadoria, ";
                        selectExterno += "SUM(ValorMercadoria) ValorMercadoria, ";
                    }
                    break;
                case "PercVlrFreteVlrMerc":
                    if (!count && !select.Contains("PercVlrFreteVlrMerc"))
                    {
                        select += "(((SUM(CTe.CON_VALOR_RECEBER) - ISNULL(SUM(CTe.CON_VAL_ICMS), 0)) / CASE WHEN SUM(CTe.CON_VALOR_TOTAL_MERC) > 0 THEN SUM(CTe.CON_VALOR_TOTAL_MERC) ELSE 1 END) * 100) PercVlrFreteVlrMerc, ";
                        selectExterno += "SUM(PercVlrFreteVlrMerc) PercVlrFreteVlrMerc, ";
                    }
                    break;
                case "PesFatQtdCTe":
                    if (!count && !select.Contains("PesFatQtdCTe"))
                    {
                        select += " ISNULL(SUM(CTe.CON_PESO_FATURADO), 0) / COUNT(CTe.CON_CODIGO) PesFatQtdCTe, ";
                        selectExterno += "SUM(PesFatQtdCTe) PesFatQtdCTe, ";
                    }
                    break;
                case "VlrFreteQtdCTe":
                    if (!count && !select.Contains("VlrFreteQtdCTe"))
                    {
                        select += "(SUM(CTe.CON_VALOR_RECEBER) - ISNULL(SUM(CTe.CON_VAL_ICMS), 0)) / COUNT(CTe.CON_CODIGO) VlrFreteQtdCTe, ";
                        selectExterno += "SUM(VlrFreteQtdCTe) VlrFreteQtdCTe, ";
                    }
                    break;
                default:
                    if (!count && propriedade.Contains("ValorComponente"))
                    {
                        select += $"(SELECT SUM(ComponentePrestacao.CPT_VALOR) from T_CTE_COMP_PREST ComponentePrestacao WHERE ComponentePrestacao.CON_CODIGO = CTe.CON_CODIGO AND ComponentePrestacao.CFR_CODIGO = {codigoDinamico}) {propriedade}, ";

                        if (!groupBy.Contains("CTe.CON_CODIGO"))
                            groupBy += "CTe.CON_CODIGO, ";

                        selectExterno += $"SUM({propriedade}) {propriedade}, ";
                    }
                    break;
            }
        }

        private void SetarWhereRelatorioFaturamentoPorGrupoPessoas(ref string where, ref string joins, ref List<ParametroSQL> parametros, Dominio.ObjetosDeValor.Embarcador.CTe.FiltroPesquisaRelatorioFaturamentoPorGrupoPessoas filtrosPesquisa)
        {
            if (filtrosPesquisa.DocumentoFaturavel.HasValue)
            {
                if (filtrosPesquisa.DocumentoFaturavel.Value)
                    where += " and ModeloDocumento.MOD_NAO_GERAR_FATURAMENTO <> 1";
                else
                    where += " and ModeloDocumento.MOD_NAO_GERAR_FATURAMENTO = 1";
            }

            if (filtrosPesquisa.DataInicialEmissao != DateTime.MinValue)
                where += " and CTe.CON_DATAHORAEMISSAO >= '" + filtrosPesquisa.DataInicialEmissao.ToString("yyyy-MM-dd") + "'";

            if (filtrosPesquisa.DataFinalEmissao != DateTime.MinValue)
                where += " and CTe.CON_DATAHORAEMISSAO < '" + filtrosPesquisa.DataFinalEmissao.AddDays(1).ToString("yyyy-MM-dd") + "'";

            if (filtrosPesquisa.DataInicialAutorizacao != DateTime.MinValue)
                where += " and CTe.CON_DATA_AUTORIZACAO >= '" + filtrosPesquisa.DataInicialAutorizacao.ToString("yyyy-MM-dd") + "'";

            if (filtrosPesquisa.DataFinalAutorizacao != DateTime.MinValue)
                where += " and CTe.CON_DATA_AUTORIZACAO < '" + filtrosPesquisa.DataFinalAutorizacao.AddDays(1).ToString("yyyy-MM-dd") + "'";

            if (filtrosPesquisa.CodigosGruposPessoas?.Count > 0)
                where += " and Cliente.GRP_CODIGO in (" + string.Join(",", filtrosPesquisa.CodigosGruposPessoas) + ")";

            if (filtrosPesquisa.CodigosModeloDocumentoFiscal?.Count > 0)
                where += " and CTe.CON_MODELODOC in (" + string.Join(",", filtrosPesquisa.CodigosModeloDocumentoFiscal) + ")";

            if (!string.IsNullOrWhiteSpace(filtrosPesquisa.PropriedadeVeiculo))
            {
                if (filtrosPesquisa.PropriedadeVeiculo == "O") //não se encontra nem em próprio e nem em terceiro "Outros"
                    where += " and not exists (select vei_codigo from t_cte_veiculo vei where vei.CON_CODIGO = CTe.CON_CODIGO and vei.CVE_TIPO_VEICULO = 0 and vei.CVE_TIPO_PROPRIEDADE in ('P', 'T'))";
                else
                {
                    where += " and exists (select vei_codigo from t_cte_veiculo vei where vei.CON_CODIGO = CTe.CON_CODIGO and vei.CVE_TIPO_VEICULO = 0 and vei.CVE_TIPO_PROPRIEDADE = :VEI_CVE_TIPO_PROPRIEDADE)";
                    parametros.Add(new ParametroSQL("VEI_CVE_TIPO_PROPRIEDADE", filtrosPesquisa.PropriedadeVeiculo));
                }
            }

            if (filtrosPesquisa.SomenteCTesDeMinutas.HasValue && filtrosPesquisa.SomenteCTesDeMinutas.Value)
                where += " and CTe.CON_CODIGO in (select CON_CODIGO from T_NATURA_DOCUMENTO_TRANSPORTE_NOTA_FISCAL union all select con_codigo from T_AVON_MANIFESTO_DOCUMENTO)";

            if (filtrosPesquisa.TipoProposta.HasValue)
            {
                where += $" and CTe.CON_CODIGO in (select CargaCTe.CON_CODIGO from T_CARGA_CTE CargaCTe " +
                             $"join T_CARGA_PEDIDO CargaPedido on CargaPedido.CAR_CODIGO = CargaCTe.CAR_CODIGO " +
                             $"and CargaPedido.TBF_TIPO_PROPOSTA_MULTIMODAL = {(int)filtrosPesquisa.TipoProposta.Value})";
            }

            if (filtrosPesquisa.VinculoCarga.HasValue)
                where += " and CTe.CON_CODIGO " + (filtrosPesquisa.VinculoCarga.Value ? "in" : "not in") + " (select CON_CODIGO from T_CARGA_CTE) ";

            where += " and CTe.CON_TIPO_AMBIENTE = " + (int)filtrosPesquisa.TipoAmbiente;
        }

        #endregion

        #region Relatório Comparativo de Faturamento por Grupo de Pessoas 

        public IList<Dominio.Relatorios.Embarcador.DataSource.CTe.CTes.ComparativoFaturamentoGrupoPessoas> ConsultarRelatorioComparativoMensalFaturamentoGrupoPessoas(List<Dominio.ObjetosDeValor.Embarcador.CTe.Periodo> periodos, List<int> gruposPessoas, string propriedadeVeiculo, Dominio.Enumeradores.TipoAmbiente ambiente)
        {
            StringBuilder sql = new StringBuilder();
            var parametros = new List<ParametroSQL>();

            sql.Append(@"SELECT  
                         GrupoPessoas.GRP_CODIGO CodigoGrupoPessoas,
                         GrupoPessoas.GRP_DESCRICAO GrupoPessoas,
                         MONTH(CTe.CON_DATAHORAEMISSAO) Mes,
                         YEAR(CTe.CON_DATAHORAEMISSAO) Ano,
                         SUM(CTe.CON_VALOR_RECEBER) ValorReceber
                         FROM 
                         T_CTE CTe 
                         left outer join T_CTE_PARTICIPANTE Tomador on CTe.CON_TOMADOR_PAGADOR_CTE = Tomador.PCT_CODIGO 
                         left outer join T_CLIENTE Cliente on Tomador.CLI_CODIGO = Cliente.CLI_CGCCPF 
                         left outer join T_GRUPO_PESSOAS GrupoPessoas on Cliente.GRP_CODIGO = GrupoPessoas.GRP_CODIGO 
                         left outer join T_MODDOCFISCAL ModeloDocumento on ModeloDocumento.MOD_CODIGO = CTe.CON_MODELODOC  
                         WHERE 
                         CTe.CON_STATUS = 'A' AND 
                         CTe.CON_TIPO_CTE <> 2 AND
                         ModeloDocumento.MOD_NAO_GERAR_FATURAMENTO <> 1  AND
                         CTe.CON_TIPO_AMBIENTE = ").Append((int)ambiente);

            if (gruposPessoas != null && gruposPessoas.Count > 0)
                sql.Append(" and Cliente.GRP_CODIGO in (").Append(string.Join(",", gruposPessoas)).Append(")");

            if (!string.IsNullOrWhiteSpace(propriedadeVeiculo))
            {
                if (propriedadeVeiculo == "O") //não se encontra nem em próprio e nem em terceiro "Outros"
                    sql.Append(" and not exists (select vei_codigo from t_cte_veiculo vei where vei.CON_CODIGO = CTe.CON_CODIGO and vei.CVE_TIPO_VEICULO = 0 and vei.CVE_TIPO_PROPRIEDADE in ('P', 'T'))");
                else
                {
                    sql.Append(" and exists (select vei_codigo from t_cte_veiculo vei where vei.CON_CODIGO = CTe.CON_CODIGO and vei.CVE_TIPO_VEICULO = 0 and vei.CVE_TIPO_PROPRIEDADE = :VEI_CVE_TIPO_PROPRIEDADE)");
                    parametros.Add(new ParametroSQL("VEI_CVE_TIPO_PROPRIEDADE", propriedadeVeiculo));
                }
            }

            sql.Append(" AND (");

            foreach (Dominio.ObjetosDeValor.Embarcador.CTe.Periodo periodo in periodos)
                sql.Append("(CTe.CON_DATAHORAEMISSAO >= '").Append(periodo.DataInicial.ToString("yyyy-MM-dd")).Append("' and CTe.CON_DATAHORAEMISSAO < '").Append(periodo.DataFinal.AddDays(1).ToString("yyyy-MM-dd")).Append("') or");

            sql.Remove(sql.Length - 2, 2);

            sql.Append(@") 
                         group by GrupoPessoas.GRP_CODIGO, GrupoPessoas.GRP_DESCRICAO, MONTH(CTe.CON_DATAHORAEMISSAO), YEAR(CTe.CON_DATAHORAEMISSAO)
                         order by GrupoPessoas, Ano, Mes");

            var query = new SQLDinamico(sql.ToString(), parametros).CriarQuery(this.SessionNHiBernate);

            query.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Dominio.Relatorios.Embarcador.DataSource.CTe.CTes.ComparativoFaturamentoGrupoPessoas)));

            return query.SetTimeout(300).List<Dominio.Relatorios.Embarcador.DataSource.CTe.CTes.ComparativoFaturamentoGrupoPessoas>();
        }

        #endregion

        #region Relatório de AFRMM Control

        public IList<Dominio.Relatorios.Embarcador.DataSource.CTe.AFRMMControl> ConsultarRelatorioAFRMMControl(Dominio.ObjetosDeValor.Embarcador.CTe.FiltroPesquisaRelatorioAFRMMControl filtrosPesquisa, List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedades, string propAgrupa, string dirAgrupa, string propOrdena, string dirOrdena, int inicio, int limite)
        {
            string sql = ObterSelectConsultaRelatorioAFRMMControl(filtrosPesquisa, false, propriedades, propAgrupa, dirAgrupa, propOrdena, dirOrdena, inicio, limite);
            var query = this.SessionNHiBernate.CreateSQLQuery(sql);

            query.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Dominio.Relatorios.Embarcador.DataSource.CTe.AFRMMControl)));

            return query.SetTimeout(600).List<Dominio.Relatorios.Embarcador.DataSource.CTe.AFRMMControl>();
        }

        public async Task<IList<Dominio.Relatorios.Embarcador.DataSource.CTe.AFRMMControl>> ConsultarRelatorioAFRMMControlAsync(Dominio.ObjetosDeValor.Embarcador.CTe.FiltroPesquisaRelatorioAFRMMControl filtrosPesquisa, List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedades, string propAgrupa, string dirAgrupa, string propOrdena, string dirOrdena, int inicio, int limite)
        {
            string sql = ObterSelectConsultaRelatorioAFRMMControl(filtrosPesquisa, false, propriedades, propAgrupa, dirAgrupa, propOrdena, dirOrdena, inicio, limite);
            var query = this.SessionNHiBernate.CreateSQLQuery(sql);

            query.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Dominio.Relatorios.Embarcador.DataSource.CTe.AFRMMControl)));

            return await query.SetTimeout(600).ListAsync<Dominio.Relatorios.Embarcador.DataSource.CTe.AFRMMControl>();
        }

        public int ContarConsultaRelatorioAFRMMControl(Dominio.ObjetosDeValor.Embarcador.CTe.FiltroPesquisaRelatorioAFRMMControl filtrosPesquisa, List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedades)
        {
            string sql = ObterSelectConsultaRelatorioAFRMMControl(filtrosPesquisa, true, propriedades, null, null, null, null, 0, 0);
            var query = this.SessionNHiBernate.CreateSQLQuery(sql);

            return query.SetTimeout(600).UniqueResult<int>();
        }

        private string ObterSelectConsultaRelatorioAFRMMControl(Dominio.ObjetosDeValor.Embarcador.CTe.FiltroPesquisaRelatorioAFRMMControl filtrosPesquisa, bool count, List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedades, string propAgrupa, string dirAgrupa, string propOrdena, string dirOrdena, int inicio, int limite)
        {
            string select = string.Empty,
                   groupBy = string.Empty,
                   joins = string.Empty,
                   where = string.Empty,
                   orderBy = string.Empty;

            for (var i = propriedades.Count - 1; i >= 0; i--)
                SetarSelectRelatorioConsultaRelatorioAFRMMControl(propriedades[i].Propriedade, propriedades[i].CodigoDinamico, ref select, ref groupBy, ref joins, count);

            SetarWhereRelatorioConsultaRelatorioAFRMMControl(ref where, ref groupBy, ref joins, filtrosPesquisa);

            if (!count)
            {
                if (!string.IsNullOrWhiteSpace(propAgrupa))
                {
                    SetarSelectRelatorioConsultaRelatorioAFRMMControl(propAgrupa, 0, ref select, ref groupBy, ref joins, count);

                    if (select.Contains(propAgrupa))
                        orderBy = propAgrupa + " " + dirAgrupa;
                }

                if (!string.IsNullOrWhiteSpace(propOrdena))
                {
                    if (propOrdena != propAgrupa && select.Contains(propOrdena) && propOrdena != "Codigo")
                        orderBy += (orderBy.Length > 0 ? ", " : string.Empty) + propOrdena + " " + dirOrdena;
                }
            }

            // SELECT
            string query = "SELECT ";

            if (count)
                query += "DISTINCT(COUNT(0) OVER())";
            else if (select.Length > 0)
                query += select.Substring(0, select.Length - 2);

            // FROM
            query += @" FROM T_CTE CTe 
                        JOIN T_MODDOCFISCAL M ON M.MOD_CODIGO = CTe.CON_MODELODOC AND M.MOD_NUM = '57' ";

            // JOIN
            query += joins;

            // WHERE
            query += " WHERE CTe.CON_TIPO_MODAL in (3, 6) " + where;

            // GROUP BY
            if (groupBy.Length > 0)
                query += " GROUP BY " + groupBy.Substring(0, groupBy.Length - 2);

            // ORDER BY
            if (orderBy.Length > 0)
                query += " ORDER BY " + orderBy;
            else if (!count)
                query += " ORDER BY 1 ASC";

            // LIMIT
            if (!count && limite > 0)
                query += " OFFSET " + inicio.ToString() + " ROWS FETCH NEXT " + limite.ToString() + " ROWS ONLY";

            return query;
        }

        private void SetarSelectRelatorioConsultaRelatorioAFRMMControl(string propriedade, int codigoDinamico, ref string select, ref string groupBy, ref string joins, bool count)
        {
            switch (propriedade)
            {
                case "Codigo":
                    if (!select.Contains(" Codigo, "))
                    {
                        select += "CTe.CON_CODIGO Codigo, ";
                        groupBy += "CTe.CON_CODIGO, ";
                    }
                    break;
                case "CTAC":
                    if (!select.Contains(" CTAC, "))
                    {
                        select += "CASE WHEN CTe.CON_TIPO_MODAL = 6 THEN '' ELSE CTe.CON_NUMERO_CONTROLE END CTAC, ";
                        groupBy += "CTe.CON_NUMERO_CONTROLE, CTe.CON_TIPO_MODAL, ";
                    }
                    break;
                case "DataEmissaoFormatada":
                    if (!select.Contains(" DataEmissao, "))
                    {
                        select += "CTe.CON_DATAHORAEMISSAO DataEmissao, ";
                        groupBy += "CTe.CON_DATAHORAEMISSAO, ";
                    }
                    break;
                case "NumeroFiscal":
                    if (!select.Contains(" NumeroFiscal, "))
                    {
                        select += "CTe.CON_NUM NumeroFiscal, ";
                        groupBy += "CTe.CON_NUM, ";
                    }
                    break;
                case "ChaveCTe":
                    if (!select.Contains(" ChaveCTe, "))
                    {
                        select += "CTe.CON_CHAVECTE ChaveCTe, ";
                        groupBy += "CTe.CON_CHAVECTE, ";
                    }
                    break;
                case "MotivoCancelamento":
                    if (!select.Contains(" MotivoCancelamento, "))
                    {
                        select += "CTe.CON_OBS_CANCELAMENTO MotivoCancelamento, ";
                        groupBy += "CTe.CON_OBS_CANCELAMENTO, ";
                    }
                    break;
                case "StatusCTe":
                    if (!select.Contains(" StatusCTe, "))
                    {
                        select += @"CASE 
                                        WHEN CTe.CON_STATUS = 'P' THEN 'PENDENTE'
                                        WHEN CTe.CON_STATUS = 'E' THEN 'ENVIADO'
                                        WHEN CTe.CON_STATUS = 'R' THEN 'REJEICAO'
                                        WHEN CTe.CON_STATUS = 'A' THEN 'Ativo'
                                        WHEN CTe.CON_STATUS = 'C' THEN 'Cancelado'
                                        WHEN CTe.CON_STATUS = 'I' THEN 'INUTILIZADO'
                                        WHEN CTe.CON_STATUS = 'D' THEN 'DENEGADO'
                                        WHEN CTe.CON_STATUS = 'S' THEN 'EM DIGITACAO'
                                        WHEN CTe.CON_STATUS = 'K' THEN 'EM CANCELAMENTO'
                                        WHEN CTe.CON_STATUS = 'L' THEN 'EM INUTILIZACAO'
                                        WHEN CTe.CON_STATUS = 'Z' THEN 'ANULADO (AUTORIZADO, MAS GERENCIALMENTE FICA CANCELADO)'
                                        WHEN CTe.CON_STATUS = 'X' THEN 'AGUARDANDO ASSINATURA (CERTIFICADO A3)'
                                        WHEN CTe.CON_STATUS = 'V' THEN 'AGUARDANDO ASSINATURA CANCELAMENTO (CERTIFICADO A3)'
                                        WHEN CTe.CON_STATUS = 'B' THEN 'AGUARDANDO ASSINATURA INUTILIZACAO (CERTIFICADO A3)'
                                        WHEN CTe.CON_STATUS = 'M' THEN 'AGUARDANDO EMISSAO E-MAIL'
                                        WHEN CTe.CON_STATUS = 'F' THEN 'EMITIDO EM CONTINGÊNCIA FSDA'
                                        WHEN CTe.CON_STATUS = 'Q' THEN 'EMITIDO EM CONTINGÊNCIA EPEC'
                                        WHEN CTe.CON_STATUS = 'Y' THEN 'AGUARDANDO FINALIZAR CARGA INTEGRACAO'
                                        WHEN CTe.CON_STATUS = 'N' THEN 'AGUARDANDO NFSE AUTORIZAR (RPS PROCESSADO)'
                                    ELSE ''
                                    END StatusCTe, ";
                        groupBy += "CTe.CON_STATUS, ";
                    }
                    break;

                case "FilialFormatada":
                    if (!select.Contains(" Filial, "))
                    {
                        if (!joins.Contains(" Empresa "))
                            joins += " JOIN T_EMPRESA Empresa ON Empresa.EMP_CODIGO = CTe.EMP_CODIGO";

                        select += "Empresa.EMP_FANTASIA Filial, Empresa.EMP_CNPJ CNPJFilial, ";
                        groupBy += "Empresa.EMP_FANTASIA, Empresa.EMP_CNPJ, ";
                    }
                    break;
                case "CodigoFilial":
                    if (!select.Contains(" CodigoFilial, "))
                    {
                        if (!joins.Contains(" Empresa "))
                            joins += " JOIN T_EMPRESA Empresa ON Empresa.EMP_CODIGO = CTe.EMP_CODIGO";

                        select += "Empresa.EMP_CODIGO_EMPRESA CodigoFilial, ";
                        groupBy += "Empresa.EMP_CODIGO_EMPRESA, ";
                    }
                    break;

                case "Viagem":
                    if (!select.Contains(" Viagem, "))
                    {
                        if (!joins.Contains(" Viagem "))
                            joins += " JOIN T_PEDIDO_VIAGEM_NAVIO Viagem ON Viagem.PVN_CODIGO = CTe.CON_VIAGEM";

                        select += "Viagem.PVN_NUMERO_VIAGEM Viagem, ";
                        groupBy += "Viagem.PVN_NUMERO_VIAGEM, ";
                    }
                    break;
                case "Direcao":
                    if (!select.Contains(" Direcao, "))
                    {
                        if (!joins.Contains(" Viagem "))
                            joins += " JOIN T_PEDIDO_VIAGEM_NAVIO Viagem ON Viagem.PVN_CODIGO = CTe.CON_VIAGEM";

                        select += @"CASE
                                        WHEN Viagem.PVN_DIRECAO = 1 THEN 'N'
                                        WHEN Viagem.PVN_DIRECAO = 2 THEN 'S'
                                        WHEN Viagem.PVN_DIRECAO = 3 THEN 'E'
                                        WHEN Viagem.PVN_DIRECAO = 4 THEN 'W'
                                    ELSE ''
                                    END Direcao, ";
                        groupBy += "Viagem.PVN_DIRECAO, ";
                    }
                    break;
                case "Navio":
                    if (!select.Contains(" Navio, "))
                    {
                        if (!joins.Contains(" Viagem "))
                            joins += " JOIN T_PEDIDO_VIAGEM_NAVIO Viagem ON Viagem.PVN_CODIGO = CTe.CON_VIAGEM";

                        if (!joins.Contains(" Navio "))
                            joins += " JOIN T_NAVIO Navio ON Navio.NAV_CODIGO = Viagem.NAV_CODIGO";

                        select += "Navio.NAV_CODIGO_DOCUMENTO Navio, ";
                        groupBy += "Navio.NAV_CODIGO_DOCUMENTO, ";
                    }
                    break;

                case "POL":
                    if (!select.Contains(" POL, "))
                    {
                        if (!joins.Contains(" PortoOrigem "))
                            joins += " JOIN T_PORTO PortoOrigem ON PortoOrigem.POT_CODIGO = CTe.POT_CODIGO_ORIGEM";

                        select += "PortoOrigem.POT_DESCRICAO POL, ";
                        groupBy += "PortoOrigem.POT_DESCRICAO, ";
                    }
                    break;
                case "POLCodigo":
                    if (!select.Contains(" POLCodigo, "))
                    {
                        if (!joins.Contains(" PortoOrigem "))
                            joins += " JOIN T_PORTO PortoOrigem ON PortoOrigem.POT_CODIGO = CTe.POT_CODIGO_ORIGEM";

                        select += "PortoOrigem.POT_CODIGO_DOCUMENTO POLCodigo, ";
                        groupBy += "PortoOrigem.POT_CODIGO_DOCUMENTO, ";
                    }
                    break;

                case "POD":
                    if (!select.Contains(" POD, "))
                    {
                        if (!joins.Contains(" PortoDestino "))
                            joins += " JOIN T_PORTO PortoDestino ON PortoDestino.POT_CODIGO = CTe.POT_CODIGO_DESTINO";

                        select += "PortoDestino.POT_DESCRICAO POD, ";
                        groupBy += "PortoDestino.POT_DESCRICAO, ";
                    }
                    break;
                case "PODCodigo":
                    if (!select.Contains(" PODCodigo, "))
                    {
                        if (!joins.Contains(" PortoDestino "))
                            joins += " JOIN T_PORTO PortoDestino ON PortoDestino.POT_CODIGO = CTe.POT_CODIGO_DESTINO";

                        select += "PortoDestino.POT_CODIGO_DOCUMENTO PODCodigo, ";
                        groupBy += "PortoDestino.POT_CODIGO_DOCUMENTO, ";
                    }
                    break;

                case "ChaveNFe":
                    if (!select.Contains(" ChaveNFe, "))
                    {
                        //ChaveCTeCliente
                        if (!joins.Contains(" CTeSubcontratado "))
                            joins += @" LEFT JOIN T_CTE_SUBCONTRATADO CTeSubcontratado ON CTeSubcontratado.CON_CODIGO = CTe.CON_CODIGO 
                                            AND CTeSubcontratado.CSU_CHAVE <> '' AND CTeSubcontratado.CSU_CHAVE IS NOT NULL";

                        if (!joins.Contains(" CTeDoSubcontratado "))
                            joins += " LEFT JOIN T_CTE CTeDoSubcontratado ON CTeDoSubcontratado.CON_CHAVECTE = CTeSubcontratado.CSU_CHAVE";

                        if (!joins.Contains(" CTeSubcontratadoDoCTeDoSubcontratado "))
                            joins += @" LEFT JOIN T_CTE_SUBCONTRATADO CTeSubcontratadoDoCTeDoSubcontratado ON CTeSubcontratadoDoCTeDoSubcontratado.CON_CODIGO = CTeDoSubcontratado.CON_CODIGO 
                                            AND CTeSubcontratadoDoCTeDoSubcontratado.CSU_CHAVE <> '' AND CTeSubcontratadoDoCTeDoSubcontratado.CSU_CHAVE IS NOT NULL";

                        if (!joins.Contains(" CTeDoSubcontratadoDoCTeSubcontratadoDoCTeDoSubcontratado "))
                            joins += " LEFT JOIN T_CTE CTeDoSubcontratadoDoCTeSubcontratadoDoCTeDoSubcontratado ON CTeDoSubcontratadoDoCTeSubcontratadoDoCTeDoSubcontratado.CON_CHAVECTE = CTeSubcontratadoDoCTeDoSubcontratado.CSU_CHAVE";

                        if (!joins.Contains(" CTeDocumentosCTeCliente "))
                            joins += " LEFT JOIN T_CTE_DOCS CTeDocumentosCTeCliente ON CTeDocumentosCTeCliente.CON_CODIGO = CTeDoSubcontratadoDoCTeSubcontratadoDoCTeDoSubcontratado.CON_CODIGO";

                        //ChaveCTM
                        if (!joins.Contains(" CTeSubcontratado "))
                            joins += @" LEFT JOIN T_CTE_SUBCONTRATADO CTeSubcontratado ON CTeSubcontratado.CON_CODIGO = CTe.CON_CODIGO 
                                            AND CTeSubcontratado.CSU_CHAVE <> '' AND CTeSubcontratado.CSU_CHAVE IS NOT NULL";

                        if (!joins.Contains(" CTeDoSubcontratado "))
                            joins += " LEFT JOIN T_CTE CTeDoSubcontratado ON CTeDoSubcontratado.CON_CHAVECTE = CTeSubcontratado.CSU_CHAVE";

                        if (!joins.Contains(" CTeDocumentosCTM "))
                            joins += " LEFT JOIN T_CTE_DOCS CTeDocumentosCTM ON CTeDocumentosCTM.CON_CODIGO = CTeDoSubcontratado.CON_CODIGO";

                        //ChaveCTe
                        if (!joins.Contains(" CTeDocumentos "))
                            joins += " LEFT JOIN T_CTE_DOCS CTeDocumentos ON CTeDocumentos.CON_CODIGO = CTe.CON_CODIGO";

                        select += "ISNULL(CTeDocumentosCTeCliente.NFC_CHAVENFE, ISNULL(CTeDocumentosCTM.NFC_CHAVENFE, CTeDocumentos.NFC_CHAVENFE)) ChaveNFe, ";
                        groupBy += "CTeDocumentosCTeCliente.NFC_CHAVENFE, CTeDocumentosCTM.NFC_CHAVENFE, CTeDocumentos.NFC_CHAVENFE, ";
                    }
                    break;

                case "CTM":
                    if (!select.Contains(" CTM, "))
                    {
                        if (!joins.Contains(" CTeSubcontratado "))
                            joins += @" LEFT JOIN T_CTE_SUBCONTRATADO CTeSubcontratado ON CTeSubcontratado.CON_CODIGO = CTe.CON_CODIGO 
                                            AND CTeSubcontratado.CSU_CHAVE <> '' AND CTeSubcontratado.CSU_CHAVE IS NOT NULL";

                        if (!joins.Contains(" CTeDoSubcontratado "))
                            joins += " LEFT JOIN T_CTE CTeDoSubcontratado ON CTeDoSubcontratado.CON_CHAVECTE = CTeSubcontratado.CSU_CHAVE";

                        select += "CASE WHEN CTe.CON_TIPO_MODAL = 6 THEN CTe.CON_NUMERO_CONTROLE ELSE ISNULL(CTeDoSubcontratado.CON_NUMERO_CONTROLE, ISNULL(CTeDoSubcontratado.CON_NUM, SUBSTRING(CTeSubcontratado.CSU_CHAVE, 25, 9))) END CTM, ";
                        groupBy += "CTe.CON_TIPO_MODAL, CTe.CON_NUMERO_CONTROLE, CTeDoSubcontratado.CON_NUMERO_CONTROLE, CTeDoSubcontratado.CON_NUM,  CTeSubcontratado.CSU_CHAVE, ";
                    }
                    break;
                case "ChaveCTM":
                    if (!select.Contains(" ChaveCTM, "))
                    {
                        if (!joins.Contains(" CTeSubcontratado "))
                            joins += @" LEFT JOIN T_CTE_SUBCONTRATADO CTeSubcontratado ON CTeSubcontratado.CON_CODIGO = CTe.CON_CODIGO 
                                            AND CTeSubcontratado.CSU_CHAVE <> '' AND CTeSubcontratado.CSU_CHAVE IS NOT NULL";

                        select += "CTeSubcontratado.CSU_CHAVE ChaveCTM, ";
                        groupBy += "CTeSubcontratado.CSU_CHAVE, ";
                    }
                    break;

                case "TipoDocumento":
                    if (!select.Contains(" TipoDocumento, "))
                    {
                        select += @"CASE 
                                        WHEN CTe.CON_TIPO_CTE = 0 THEN 'Regular'
                                    ELSE 'Manual'
                                    END TipoDocumento, ";
                        groupBy += "CTe.CON_TIPO_CTE, ";
                    }
                    break;

                case "ObservacaoComplementar":
                    if (!select.Contains(" ObservacaoComplementar, "))
                    {
                        select += @"SUBSTRING((SELECT DISTINCT ', ' + CAST(REPLACE(REPLACE(CON_OBSGERAIS,';',''),'.','') AS NVARCHAR(2000))
		                            FROM T_CTE C
		                            WHERE C.CON_STATUS = 'A' AND C.CON_CHAVE_CTE_SUB_COMP = CTe.CON_CHAVECTE FOR XML PATH('')), 3, 5000) AS ObservacaoComplementar, ";

                        if (!groupBy.Contains("CTe.CON_CHAVECTE, "))
                            groupBy += "CTe.CON_CHAVECTE, ";
                    }
                    break;

                case "ChaveCTeCliente":
                    if (!select.Contains(" ChaveCTeCliente, "))
                    {
                        if (!joins.Contains(" CTeSubcontratado "))
                            joins += @" LEFT JOIN T_CTE_SUBCONTRATADO CTeSubcontratado ON CTeSubcontratado.CON_CODIGO = CTe.CON_CODIGO 
                                            AND CTeSubcontratado.CSU_CHAVE <> '' AND CTeSubcontratado.CSU_CHAVE IS NOT NULL";

                        if (!joins.Contains(" CTeDoSubcontratado "))
                            joins += " LEFT JOIN T_CTE CTeDoSubcontratado ON CTeDoSubcontratado.CON_CHAVECTE = CTeSubcontratado.CSU_CHAVE";

                        if (!joins.Contains(" CTeSubcontratadoDoCTeDoSubcontratado "))
                            joins += @" LEFT JOIN T_CTE_SUBCONTRATADO CTeSubcontratadoDoCTeDoSubcontratado ON CTeSubcontratadoDoCTeDoSubcontratado.CON_CODIGO = CTeDoSubcontratado.CON_CODIGO 
                                            AND CTeSubcontratadoDoCTeDoSubcontratado.CSU_CHAVE <> '' AND CTeSubcontratadoDoCTeDoSubcontratado.CSU_CHAVE IS NOT NULL";

                        select += "CTeSubcontratadoDoCTeDoSubcontratado.CSU_CHAVE ChaveCTeCliente, ";
                        groupBy += "CTeSubcontratadoDoCTeDoSubcontratado.CSU_CHAVE, ";
                    }
                    break;

                default:
                    break;
            }
        }

        private void SetarWhereRelatorioConsultaRelatorioAFRMMControl(ref string where, ref string groupBy, ref string joins, Dominio.ObjetosDeValor.Embarcador.CTe.FiltroPesquisaRelatorioAFRMMControl filtrosPesquisa)
        {
            string datePattern = "yyyy-MM-dd";

            where += " AND ((CTe.CON_TIPO_MODAL = 6 and CTe.CON_STATUS = 'A') or (CTe.CON_TIPO_MODAL = 3 and CTe.CON_STATUS in ('A', 'C'))) ";
            where += " AND ((CTe.CON_TIPO_MODAL = 6 and CTe.CON_CODIGO in (SELECT DISTINCT ISNULL(CC.CON_CODIGO, 0) FROM T_CTE_SUBCONTRATADO CC)) or (CTe.CON_TIPO_MODAL = 3)) ";

            if (filtrosPesquisa.DataInicial != DateTime.MinValue)
                where += " AND CTe.CON_DATAHORAEMISSAO >= '" + filtrosPesquisa.DataInicial.Date.ToString(datePattern) + "'";

            if (filtrosPesquisa.DataFinal != DateTime.MinValue)
                where += " AND CTe.CON_DATAHORAEMISSAO < '" + filtrosPesquisa.DataFinal.Date.AddDays(1).ToString(datePattern) + "'";
        }

        #endregion

        #region Relatórios

        public IList<Dominio.Relatorios.Embarcador.DataSource.CTe.CTes.CTe> ConsultarRelatorioCTes(Dominio.ObjetosDeValor.Embarcador.CTe.FiltroPesquisaCteRelatorio filtrosPesquisa, List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedades, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            var sqlDinamico = new ConsultaConhecimentoDeTransporteEletronico().ObterSqlPesquisa(filtrosPesquisa, parametrosConsulta, propriedades);

            var consultaCte = sqlDinamico.CriarQuery(this.SessionNHiBernate);

            Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repConfiguracaoTMS = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(UnitOfWork);
            Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoTMS = repConfiguracaoTMS.BuscarConfiguracaoPadrao();

            Dominio.Relatorios.Embarcador.DataSource.CTe.CTes.FormatacaoContext.ExportarCamposFormatado = configuracaoTMS.ExportarCNPJEChaveDeAcessoFormatado;

            consultaCte.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Dominio.Relatorios.Embarcador.DataSource.CTe.CTes.CTe)));

            return consultaCte.SetTimeout(600).List<Dominio.Relatorios.Embarcador.DataSource.CTe.CTes.CTe>();
        }

        public int ContarConsultaRelatorioCTes(Dominio.ObjetosDeValor.Embarcador.CTe.FiltroPesquisaCteRelatorio filtrosPesquisa, List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedades)
        {
            var sqlDinamico = new ConsultaConhecimentoDeTransporteEletronico().ObterSqlContarPesquisa(filtrosPesquisa, propriedades);

            return sqlDinamico.CriarQuery(this.SessionNHiBernate).SetTimeout(600).UniqueResult<int>();
        }

        public IList<Dominio.Relatorios.Embarcador.DataSource.CTe.CTes.CTeSubcontratado> ConsultarRelatorioCTesSubcontratados(Dominio.ObjetosDeValor.Embarcador.CTe.FiltroPesquisaRelatorioCTesSubcontratados filtrosPesquisa, List<PropriedadeAgrupamento> propriedades, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            var sqlDinamico = new ConsultaConhecimentoDeTransporteEletronicoSubcontratado().ObterSqlPesquisa(filtrosPesquisa, parametrosConsulta, propriedades);

            var consultaCte = sqlDinamico.CriarQuery(this.SessionNHiBernate);

            consultaCte.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Dominio.Relatorios.Embarcador.DataSource.CTe.CTes.CTeSubcontratado)));

            return consultaCte.SetTimeout(600).List<Dominio.Relatorios.Embarcador.DataSource.CTe.CTes.CTeSubcontratado>();
        }

        public int ContarConsultaRelatorioCTesSubcontratados(Dominio.ObjetosDeValor.Embarcador.CTe.FiltroPesquisaRelatorioCTesSubcontratados filtrosPesquisa, List<PropriedadeAgrupamento> propriedades)
        {
            var query = new ConsultaConhecimentoDeTransporteEletronicoSubcontratado().ObterSqlContarPesquisa(filtrosPesquisa, propriedades).CriarSQLQuery(this.SessionNHiBernate);

            return query.SetTimeout(600).UniqueResult<int>();
        }

        public IList<Dominio.Relatorios.Embarcador.DataSource.CTe.CTes.CustoRentabilidadeCteCrt> ConsultarRelatorioCustoRentabilidadeCteCrt(Dominio.ObjetosDeValor.Embarcador.CTe.FiltroPesquisaRelatorioCustoRentabilidadeCteCrt filtrosPesquisa, List<PropriedadeAgrupamento> propriedades, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            var sqlDinamico = new ConsultaCustoRentabilidadeCteCrt().ObterSqlPesquisa(filtrosPesquisa, parametrosConsulta, propriedades);

            var consultaCte = sqlDinamico.CriarQuery(this.SessionNHiBernate);

            consultaCte.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Dominio.Relatorios.Embarcador.DataSource.CTe.CTes.CustoRentabilidadeCteCrt)));

            return consultaCte.SetTimeout(600).List<Dominio.Relatorios.Embarcador.DataSource.CTe.CTes.CustoRentabilidadeCteCrt>();
        }

        public int ContarConsultaRelatorioCustoRentabilidadeCteCrt(Dominio.ObjetosDeValor.Embarcador.CTe.FiltroPesquisaRelatorioCustoRentabilidadeCteCrt filtrosPesquisa, List<PropriedadeAgrupamento> propriedades)
        {
            var query = new ConsultaCustoRentabilidadeCteCrt().ObterSqlContarPesquisa(filtrosPesquisa, propriedades).CriarSQLQuery(this.SessionNHiBernate);

            return query.SetTimeout(600).UniqueResult<int>();
        }

        public IList<Dominio.Relatorios.Embarcador.DataSource.CTe.CTes.NotaFiscal> ConsultarNotasFiscaisRelatorioCTes(Dominio.ObjetosDeValor.Embarcador.CTe.FiltroPesquisaCteRelatorio filtrosPesquisa)
        {
            var consultaCteNotasFiscais = this.SessionNHiBernate.CreateSQLQuery(new ConsultaConhecimentoDeTransporteEletronico().ObterSqlPesquisaNotasFiscais(filtrosPesquisa));

            consultaCteNotasFiscais.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Dominio.Relatorios.Embarcador.DataSource.CTe.CTes.NotaFiscal)));

            return consultaCteNotasFiscais.List<Dominio.Relatorios.Embarcador.DataSource.CTe.CTes.NotaFiscal>();
        }

        public IList<Dominio.Relatorios.Embarcador.DataSource.CTe.ApuracaoICMS> ConsultarRelatorioApuracaoICMS(Dominio.ObjetosDeValor.Embarcador.CTe.FiltroPesquisaRelatorioApuracaoICMS filtrosPesquisa, List<PropriedadeAgrupamento> propriedades, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            var sqlDinamico = new ConsultaApuracaoICMS().ObterSqlPesquisa(filtrosPesquisa, parametrosConsulta, propriedades);

            var query = sqlDinamico.CriarQuery(this.SessionNHiBernate);

            query.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Dominio.Relatorios.Embarcador.DataSource.CTe.ApuracaoICMS)));

            return query.SetTimeout(600).List<Dominio.Relatorios.Embarcador.DataSource.CTe.ApuracaoICMS>();
        }

        public async Task<IList<Dominio.Relatorios.Embarcador.DataSource.CTe.ApuracaoICMS>> ConsultarRelatorioApuracaoICMSAsync(Dominio.ObjetosDeValor.Embarcador.CTe.FiltroPesquisaRelatorioApuracaoICMS filtrosPesquisa, List<PropriedadeAgrupamento> propriedades, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            var sqlDinamico = new ConsultaApuracaoICMS().ObterSqlPesquisa(filtrosPesquisa, parametrosConsulta, propriedades);
            var query = sqlDinamico.CriarQuery(this.SessionNHiBernate);

            query.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Dominio.Relatorios.Embarcador.DataSource.CTe.ApuracaoICMS)));

            return await query.SetTimeout(600).ListAsync<Dominio.Relatorios.Embarcador.DataSource.CTe.ApuracaoICMS>();
        }

        public int ContarConsultaRelatorioApuracaoICMS(Dominio.ObjetosDeValor.Embarcador.CTe.FiltroPesquisaRelatorioApuracaoICMS filtrosPesquisa, List<PropriedadeAgrupamento> propriedades)
        {
            var query = new ConsultaApuracaoICMS().ObterSqlContarPesquisa(filtrosPesquisa, propriedades).CriarSQLQuery(this.SessionNHiBernate);

            return query.SetTimeout(600).UniqueResult<int>();
        }

        public async Task<int> ContarConsultaRelatorioApuracaoICMSAsync(Dominio.ObjetosDeValor.Embarcador.CTe.FiltroPesquisaRelatorioApuracaoICMS filtrosPesquisa, List<PropriedadeAgrupamento> propriedades)
        {
            var query = new ConsultaApuracaoICMS().ObterSqlContarPesquisa(filtrosPesquisa, propriedades).CriarSQLQuery(this.SessionNHiBernate);

            return await query.SetTimeout(600).UniqueResultAsync<int>();
        }

        public IList<Dominio.Relatorios.Embarcador.DataSource.CTe.FaturamentoPorCTe> ConsultarRelatorioFaturamentoPorCTe(Dominio.ObjetosDeValor.Embarcador.CTe.FiltroPesquisaRelatorioFaturamentoPorCTe filtrosPesquisa, List<PropriedadeAgrupamento> propriedades, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            var sqlDinamico = new ConsultaFaturamentoPorCTe().ObterSqlPesquisa(filtrosPesquisa, parametrosConsulta, propriedades);

            var query = sqlDinamico.CriarQuery(this.SessionNHiBernate);

            query.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Dominio.Relatorios.Embarcador.DataSource.CTe.FaturamentoPorCTe)));

            return query.SetTimeout(600).List<Dominio.Relatorios.Embarcador.DataSource.CTe.FaturamentoPorCTe>();
        }

        public int ContarConsultaRelatorioFaturamentoPorCTe(Dominio.ObjetosDeValor.Embarcador.CTe.FiltroPesquisaRelatorioFaturamentoPorCTe filtrosPesquisa, List<PropriedadeAgrupamento> propriedades)
        {
            var query = new ConsultaFaturamentoPorCTe().ObterSqlContarPesquisa(filtrosPesquisa, propriedades).CriarSQLQuery(this.SessionNHiBernate);

            return query.SetTimeout(600).UniqueResult<int>();
        }

        public IList<Dominio.Relatorios.Embarcador.DataSource.CTe.AFRMMControlMercante> ConsultarRelatorioAFRMMControlMercante(Dominio.ObjetosDeValor.Embarcador.CTe.FiltroPesquisaRelatorioAFRMMControlMercante filtrosPesquisa, List<PropriedadeAgrupamento> propriedades, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            var sqlDinamico = new ConsultaAFRMMControlMercante().ObterSqlPesquisa(filtrosPesquisa, parametrosConsulta, propriedades);

            var query = sqlDinamico.CriarQuery(this.SessionNHiBernate);

            query.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Dominio.Relatorios.Embarcador.DataSource.CTe.AFRMMControlMercante)));

            return query.SetTimeout(600).List<Dominio.Relatorios.Embarcador.DataSource.CTe.AFRMMControlMercante>();
        }

        public async Task<IList<Dominio.Relatorios.Embarcador.DataSource.CTe.AFRMMControlMercante>> ConsultarRelatorioAFRMMControlMercanteAsync(Dominio.ObjetosDeValor.Embarcador.CTe.FiltroPesquisaRelatorioAFRMMControlMercante filtrosPesquisa, List<PropriedadeAgrupamento> propriedades, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            var sqlDinamico = new ConsultaAFRMMControlMercante().ObterSqlPesquisa(filtrosPesquisa, parametrosConsulta, propriedades);

            var query = sqlDinamico.CriarQuery(this.SessionNHiBernate);

            query.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Dominio.Relatorios.Embarcador.DataSource.CTe.AFRMMControlMercante)));

            return await query.SetTimeout(600).ListAsync<Dominio.Relatorios.Embarcador.DataSource.CTe.AFRMMControlMercante>();
        }

        public int ContarConsultaRelatorioAFRMMControlMercante(Dominio.ObjetosDeValor.Embarcador.CTe.FiltroPesquisaRelatorioAFRMMControlMercante filtrosPesquisa, List<PropriedadeAgrupamento> propriedades)
        {
            var query = new ConsultaAFRMMControlMercante().ObterSqlContarPesquisa(filtrosPesquisa, propriedades).CriarSQLQuery(this.SessionNHiBernate);

            return query.SetTimeout(600).UniqueResult<int>();
        }

        public IList<Dominio.Relatorios.Embarcador.DataSource.CTe.CTes.ValePedagio> ConsultarRelatorioValePedagio(Dominio.ObjetosDeValor.Embarcador.CTe.FiltroPesquisaValePedagioRelatorio filtrosPesquisa, List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedades, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            var sqlDinamico = new Repositorio.Global.Consulta.ConsultaValePedagio().ObterSqlPesquisa(filtrosPesquisa, parametrosConsulta, propriedades);

            var consultaValePedagio = sqlDinamico.CriarQuery(this.SessionNHiBernate);

            consultaValePedagio.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Dominio.Relatorios.Embarcador.DataSource.CTe.CTes.ValePedagio)));

            return consultaValePedagio.SetTimeout(600).List<Dominio.Relatorios.Embarcador.DataSource.CTe.CTes.ValePedagio>();
        }

        public async Task<IList<Dominio.Relatorios.Embarcador.DataSource.CTe.CTes.ValePedagio>> ConsultarRelatorioValePedagioAsync(Dominio.ObjetosDeValor.Embarcador.CTe.FiltroPesquisaValePedagioRelatorio filtrosPesquisa, List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedades, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            var sqlDinamico = new Repositorio.Global.Consulta.ConsultaValePedagio().ObterSqlPesquisa(filtrosPesquisa, parametrosConsulta, propriedades);

            var consultaValePedagio = sqlDinamico.CriarQuery(this.SessionNHiBernate);

            consultaValePedagio.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Dominio.Relatorios.Embarcador.DataSource.CTe.CTes.ValePedagio)));

            return await consultaValePedagio.SetTimeout(600).ListAsync<Dominio.Relatorios.Embarcador.DataSource.CTe.CTes.ValePedagio>();
        }

        public int ContarConsultaRelatorioValePedagio(Dominio.ObjetosDeValor.Embarcador.CTe.FiltroPesquisaValePedagioRelatorio filtrosPesquisa, List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedades)
        {
            var query = new Repositorio.Global.Consulta.ConsultaValePedagio().ObterSqlContarPesquisa(filtrosPesquisa, propriedades).CriarSQLQuery(this.SessionNHiBernate);

            return query.SetTimeout(600).UniqueResult<int>();
        }

        public async Task<int> ContarConsultaRelatorioValePedagioAsync(Dominio.ObjetosDeValor.Embarcador.CTe.FiltroPesquisaValePedagioRelatorio filtrosPesquisa, List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedades)
        {
            var query = new Repositorio.Global.Consulta.ConsultaValePedagio().ObterSqlContarPesquisa(filtrosPesquisa, propriedades).CriarSQLQuery(this.SessionNHiBernate);

            return await query.SetTimeout(600).UniqueResultAsync<int>();
        }

        public IList<Dominio.Relatorios.Embarcador.DataSource.CTe.TakeOrPay> ConsultarRelatorioTakeOrPay(Dominio.ObjetosDeValor.Embarcador.CTe.FiltroPesquisaRelatorioTakeOrPay filtrosPesquisa, List<PropriedadeAgrupamento> propriedades, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            var sqlDinamico = new ConsultaTakeOrPay().ObterSqlPesquisa(filtrosPesquisa, parametrosConsulta, propriedades);

            var query = sqlDinamico.CriarQuery(this.SessionNHiBernate);

            query.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Dominio.Relatorios.Embarcador.DataSource.CTe.TakeOrPay)));

            return query.SetTimeout(600).List<Dominio.Relatorios.Embarcador.DataSource.CTe.TakeOrPay>();
        }

        public async Task<IList<Dominio.Relatorios.Embarcador.DataSource.CTe.TakeOrPay>> ConsultarRelatorioTakeOrPayAsync(Dominio.ObjetosDeValor.Embarcador.CTe.FiltroPesquisaRelatorioTakeOrPay filtrosPesquisa, List<PropriedadeAgrupamento> propriedades, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            var sqlDinamico = new ConsultaTakeOrPay().ObterSqlPesquisa(filtrosPesquisa, parametrosConsulta, propriedades);

            var query = sqlDinamico.CriarQuery(this.SessionNHiBernate);

            query.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Dominio.Relatorios.Embarcador.DataSource.CTe.TakeOrPay)));

            return await query.SetTimeout(600).ListAsync<Dominio.Relatorios.Embarcador.DataSource.CTe.TakeOrPay>();
        }

        public int ContarConsultaRelatorioTakeOrPay(Dominio.ObjetosDeValor.Embarcador.CTe.FiltroPesquisaRelatorioTakeOrPay filtrosPesquisa, List<PropriedadeAgrupamento> propriedades)
        {
            var query = new ConsultaTakeOrPay().ObterSqlContarPesquisa(filtrosPesquisa, propriedades).CriarSQLQuery(this.SessionNHiBernate);

            return query.SetTimeout(600).UniqueResult<int>();
        }

        public IList<Dominio.Relatorios.Embarcador.DataSource.Financeiros.CTeTituloReceber> ConsultarRelatorioCTeTituloReceber(Dominio.ObjetosDeValor.Embarcador.Financeiro.FiltroPesquisaRelatorioCTeTituloReceber filtrosPesquisa, List<PropriedadeAgrupamento> propriedades, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            var sqlDinamico = new Repositorio.Embarcador.Financeiro.ConsultaCTeTituloReceber().ObterSqlPesquisa(filtrosPesquisa, parametrosConsulta, propriedades);

            var query = sqlDinamico.CriarQuery(this.SessionNHiBernate);

            query.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Dominio.Relatorios.Embarcador.DataSource.Financeiros.CTeTituloReceber)));

            return query.SetTimeout(600).List<Dominio.Relatorios.Embarcador.DataSource.Financeiros.CTeTituloReceber>();
        }

        public int ContarConsultaRelatorioCTeTituloReceber(Dominio.ObjetosDeValor.Embarcador.Financeiro.FiltroPesquisaRelatorioCTeTituloReceber filtrosPesquisa, List<PropriedadeAgrupamento> propriedades)
        {
            var query = new Repositorio.Embarcador.Financeiro.ConsultaCTeTituloReceber().ObterSqlContarPesquisa(filtrosPesquisa, propriedades).CriarSQLQuery(this.SessionNHiBernate);

            return query.SetTimeout(600).UniqueResult<int>();
        }

        public IList<Dominio.Relatorios.Embarcador.DataSource.CTe.AuditoriaCTe> ConsultarRelatorioAuditoriaCTe(Dominio.ObjetosDeValor.Embarcador.CTe.FiltroPesquisaRelatorioAuditoriaCTe filtrosPesquisa, List<PropriedadeAgrupamento> propriedades, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            var sqlDinamico = new ConsultaAuditoriaCTe().ObterSqlPesquisa(filtrosPesquisa, parametrosConsulta, propriedades);

            var query = sqlDinamico.CriarQuery(this.SessionNHiBernate);

            query.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Dominio.Relatorios.Embarcador.DataSource.CTe.AuditoriaCTe)));

            return query.SetTimeout(600).List<Dominio.Relatorios.Embarcador.DataSource.CTe.AuditoriaCTe>();
        }

        public async Task<IList<Dominio.Relatorios.Embarcador.DataSource.CTe.AuditoriaCTe>> ConsultarRelatorioAuditoriaCTeAsync(Dominio.ObjetosDeValor.Embarcador.CTe.FiltroPesquisaRelatorioAuditoriaCTe filtrosPesquisa, List<PropriedadeAgrupamento> propriedades, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            var sqlDinamico = new ConsultaAuditoriaCTe().ObterSqlPesquisa(filtrosPesquisa, parametrosConsulta, propriedades);

            var query = sqlDinamico.CriarQuery(this.SessionNHiBernate);

            query.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Dominio.Relatorios.Embarcador.DataSource.CTe.AuditoriaCTe)));

            return await query.SetTimeout(600).ListAsync<Dominio.Relatorios.Embarcador.DataSource.CTe.AuditoriaCTe>();
        }

        public int ContarConsultaRelatorioAuditoriaCTe(Dominio.ObjetosDeValor.Embarcador.CTe.FiltroPesquisaRelatorioAuditoriaCTe filtrosPesquisa, List<PropriedadeAgrupamento> propriedades)
        {
            var query = new ConsultaAuditoriaCTe().ObterSqlContarPesquisa(filtrosPesquisa, propriedades).CriarSQLQuery(this.SessionNHiBernate);

            return query.SetTimeout(600).UniqueResult<int>();
        }

        public IList<Dominio.Relatorios.Embarcador.DataSource.Financeiros.ConferenciaFiscal> ConsultarRelatorioConferenciaFiscal(Dominio.ObjetosDeValor.Embarcador.Financeiro.FiltroPesquisaRelatorioConferenciaFiscal filtrosPesquisa, List<PropriedadeAgrupamento> propriedades, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            var sqlDinamico = new Repositorio.Embarcador.Financeiro.ConsultaConferenciaFiscal().ObterSqlPesquisa(filtrosPesquisa, parametrosConsulta, propriedades);

            var query = sqlDinamico.CriarQuery(this.SessionNHiBernate);

            query.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Dominio.Relatorios.Embarcador.DataSource.Financeiros.ConferenciaFiscal)));

            return query.SetTimeout(600).List<Dominio.Relatorios.Embarcador.DataSource.Financeiros.ConferenciaFiscal>();
        }

        public int ContarConsultaRelatorioConferenciaFiscal(Dominio.ObjetosDeValor.Embarcador.Financeiro.FiltroPesquisaRelatorioConferenciaFiscal filtrosPesquisa, List<PropriedadeAgrupamento> propriedades)
        {
            var query = new Repositorio.Embarcador.Financeiro.ConsultaConferenciaFiscal().ObterSqlContarPesquisa(filtrosPesquisa, propriedades).CriarSQLQuery(this.SessionNHiBernate);

            return query.SetTimeout(600).UniqueResult<int>();
        }

        public IList<Dominio.Relatorios.Embarcador.DataSource.CTe.ComissaoVendedorCTe> ConsultarRelatorioComissaoVendedorCTe(Dominio.ObjetosDeValor.Embarcador.CTe.FiltroPesquisaRelatorioComissaoVendedorCTe filtrosPesquisa, List<PropriedadeAgrupamento> propriedades, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            var sqlDinamico = new ConsultaComissaoVendedorCTe().ObterSqlPesquisa(filtrosPesquisa, parametrosConsulta, propriedades);

            var query = sqlDinamico.CriarQuery(this.SessionNHiBernate);

            query.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Dominio.Relatorios.Embarcador.DataSource.CTe.ComissaoVendedorCTe)));

            return query.SetTimeout(600).List<Dominio.Relatorios.Embarcador.DataSource.CTe.ComissaoVendedorCTe>();
        }

        public async Task<IList<Dominio.Relatorios.Embarcador.DataSource.CTe.ComissaoVendedorCTe>> ConsultarRelatorioComissaoVendedorCTeAsync(Dominio.ObjetosDeValor.Embarcador.CTe.FiltroPesquisaRelatorioComissaoVendedorCTe filtrosPesquisa, List<PropriedadeAgrupamento> propriedades, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            var sqlDinamico = new ConsultaComissaoVendedorCTe().ObterSqlPesquisa(filtrosPesquisa, parametrosConsulta, propriedades);

            var query = sqlDinamico.CriarQuery(this.SessionNHiBernate);

            query.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Dominio.Relatorios.Embarcador.DataSource.CTe.ComissaoVendedorCTe)));

            return await query.SetTimeout(600).ListAsync<Dominio.Relatorios.Embarcador.DataSource.CTe.ComissaoVendedorCTe>();
        }

        public int ContarConsultaRelatorioComissaoVendedorCTe(Dominio.ObjetosDeValor.Embarcador.CTe.FiltroPesquisaRelatorioComissaoVendedorCTe filtrosPesquisa, List<PropriedadeAgrupamento> propriedades)
        {
            var query = new ConsultaComissaoVendedorCTe().ObterSqlContarPesquisa(filtrosPesquisa, propriedades).CriarSQLQuery(this.SessionNHiBernate);

            return query.SetTimeout(600).UniqueResult<int>();
        }

        public List<int> BuscarCTesSemVinculos()
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.ConhecimentoDeTransporteEletronico>();
            var subQuery = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCTe>();

            query = query.Where(x => !subQuery.Any(p => p.CTe.Codigo == x.Codigo) && x.TipoCTE == Dominio.Enumeradores.TipoCTE.Normal);

            return query.Select(x => x.Codigo).ToList();
        }

        public List<Dominio.ObjetosDeValor.Embarcador.Integracao.Efesus.DocumentoCTeFaturaIntegracao> BuscarDocumentosCTesFaturaIntegracao(int codigoFatura)
        {
            return ConsultarDadosResumidosPorCargas(codigoFatura).Select(obj => new Dominio.ObjetosDeValor.Embarcador.Integracao.Efesus.DocumentoCTeFaturaIntegracao()
            {
                NumeroCTe = obj.Documento.CTe.Numero,
                SerieCTe = obj.Documento.CTe.Serie.Numero,
                ChaveCTe = obj.Documento.CTe.Chave,
                DataEmissaoCTe = obj.Documento.CTe.DataEmissao.Value,
                ValorFrete = obj.Documento.CTe.ValorAReceber,
            }).ToList();
        }

        public IQueryable<Dominio.Entidades.Embarcador.Fatura.FaturaDocumento> ConsultarDadosResumidosPorCargas(int codigoFatura)
        {
            IQueryable<Dominio.Entidades.Embarcador.Fatura.FaturaDocumento> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Fatura.FaturaDocumento>();

            query = query.Where(o => o.Fatura.Codigo == codigoFatura);

            return query;
        }

        #endregion
    }
}
