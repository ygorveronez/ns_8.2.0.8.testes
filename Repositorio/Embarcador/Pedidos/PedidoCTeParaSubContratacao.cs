using NHibernate.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading;
using System.Threading.Tasks;

namespace Repositorio.Embarcador.Pedidos
{
    public class PedidoCTeParaSubContratacao : RepositorioBase<Dominio.Entidades.Embarcador.Pedidos.PedidoCTeParaSubContratacao>
    {
        public PedidoCTeParaSubContratacao(UnitOfWork unitOfWork) : base(unitOfWork) { }
        public PedidoCTeParaSubContratacao(UnitOfWork unitOfWork, CancellationToken cancellationToken) : base(unitOfWork, cancellationToken) { }

        public bool PossuiEmitenteDiferenteDoCNPJ(int codigoCargaPedido, string cnpjGrupo)
        {
            IQueryable<Dominio.Entidades.Embarcador.Pedidos.PedidoCTeParaSubContratacao> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoCTeParaSubContratacao>();

            query = query.Where(o => o.CargaPedido.Codigo == codigoCargaPedido && !o.CTeTerceiro.Emitente.CPF_CNPJ.StartsWith(cnpjGrupo));

            return query.Select(o => o.Codigo).Any();
        }

        public Dominio.Entidades.Embarcador.Pedidos.PedidoCTeParaSubContratacao BuscarPorChaveECarga(int carga, string chave)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoCTeParaSubContratacao>();
            var result = from obj in query where obj.CargaPedido.CargaOrigem.Codigo == carga && obj.CTeTerceiro.ChaveAcesso == chave select obj;
            return result.Fetch(obj => obj.CTeTerceiro).FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.Pedidos.PedidoCTeParaSubContratacao BuscarPorChave(string chave)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoCTeParaSubContratacao>();
            var result = from obj in query where obj.CTeTerceiro.ChaveAcesso == chave && (obj.CargaPedido.Carga.SituacaoCarga == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.Nova || obj.CargaPedido.Carga.SituacaoCarga == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.AgTransportador) && obj.CargaPedido.Carga.Empresa == null && obj.CargaPedido.Carga.NaoExigeVeiculoParaEmissao select obj;
            return result.Fetch(obj => obj.CTeTerceiro).FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Pedidos.PedidoCTeParaSubContratacao> BuscarListaPorChaveECarga(int carga, string chave)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoCTeParaSubContratacao>();
            var result = from obj in query where obj.CargaPedido.CargaOrigem.Codigo == carga && obj.CTeTerceiro.ChaveAcesso == chave select obj;
            return result.Fetch(obj => obj.CTeTerceiro).ToList();
        }

        public Dominio.Entidades.Embarcador.Pedidos.PedidoCTeParaSubContratacao BuscarPorChavePesoDestinatario(string chave, decimal pesoBruto, double cnpjDestinatario)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoCTeParaSubContratacao>();
            var result = from obj in query where obj.CTeTerceiro.ChaveAcesso == chave && obj.CTeTerceiro.Peso == pesoBruto && obj.CTeTerceiro.Destinatario.Cliente.CPF_CNPJ == cnpjDestinatario select obj;
            return result.Fetch(obj => obj.CTeTerceiro).FirstOrDefault();
        }

        public List<string> ContemCargaSubcontratada(int codigoCarga)
        {
            var queryCargaCTe = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCTe>();
            queryCargaCTe = queryCargaCTe.Where(o => o.Carga.Codigo == codigoCarga);

            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoCTeParaSubContratacao>();
            query = query.Where(o => queryCargaCTe.Any(p => p.CTe.Chave == o.CTeTerceiro.ChaveAcesso) && o.CargaPedido.Carga.Codigo != codigoCarga && o.CargaPedido.Carga.SituacaoCarga != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.Cancelada && o.CargaPedido.Carga.SituacaoCarga != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.Anulada);

            return query.Select(o => o.CargaPedido.Carga.CodigoCargaEmbarcador).ToList();
        }

        public Dominio.Entidades.Embarcador.Pedidos.PedidoCTeParaSubContratacao BuscarPorCTeSubContratacaoECargaPedido(int codigo, int codigoCargaPedido)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoCTeParaSubContratacao>();
            var result = from obj in query where obj.CargaPedido.Codigo == codigoCargaPedido && obj.CTeTerceiro.Codigo == codigo select obj;
            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Pedidos.PedidoCTeParaSubContratacao> BuscarPorCTeSubContratacaoECargaPedido(List<Dominio.Entidades.Embarcador.CTe.CTeTerceiro> ctesTerceiro, Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido)
        {
            IQueryable<Dominio.Entidades.Embarcador.Pedidos.PedidoCTeParaSubContratacao> query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoCTeParaSubContratacao>();

            int quantidadeRegistrosConsultarPorVez = 1000;
            int quantidadeConsultas = ctesTerceiro.Count / quantidadeRegistrosConsultarPorVez;

            List<Dominio.Entidades.Embarcador.Pedidos.PedidoCTeParaSubContratacao> pedidoCTesParaSubcontratacaoRetornar = new List<Dominio.Entidades.Embarcador.Pedidos.PedidoCTeParaSubContratacao>();

            for (int i = 0; i <= quantidadeConsultas; i++)
                pedidoCTesParaSubcontratacaoRetornar.AddRange(query.Where(o => o.CargaPedido == cargaPedido && ctesTerceiro.Skip(i * quantidadeRegistrosConsultarPorVez).Take(quantidadeRegistrosConsultarPorVez).Contains(o.CTeTerceiro)).Fetch(o => o.CTeTerceiro).ToList());

            return pedidoCTesParaSubcontratacaoRetornar;
        }

        public Dominio.Entidades.Embarcador.Pedidos.PedidoCTeParaSubContratacao BuscarPorCTeSubContratacaoECargaPedido(string chave, int codigoCargaPedido)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoCTeParaSubContratacao>();
            var result = from obj in query where obj.CargaPedido.Codigo == codigoCargaPedido && obj.CTeTerceiro.ChaveAcesso == chave select obj;
            return result.FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.CTe.CTeTerceiro BuscarPorCTeTerceiroPorChaveECargaPedido(string chave, int codigoCargaPedido)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoCTeParaSubContratacao>();
            var result = from obj in query where obj.CargaPedido.Codigo == codigoCargaPedido && obj.CTeTerceiro.ChaveAcesso == chave select obj;
            return result.Select(o => o.CTeTerceiro).FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.CTe.CTeTerceiro BuscarPorCTeTerceiroFilialEmissoraCargaPedido(int codigoCargaPedido)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoCTeParaSubContratacao>();
            var result = from obj in query where obj.CargaPedido.Codigo == codigoCargaPedido && obj.CteSubContratacaoFilialEmissora == true select obj;
            return result.Select(o => o.CTeTerceiro).FirstOrDefault();
        }

        public int ContarPorCargaPedidoPacoteComCodigoCarga(int codigoCarga)
        {
            string sql = $@"SELECT COUNT(CTeTerceiro.CPS_CODIGO)
                            FROM T_CTE_TERCEIRO CTeTerceiro
                            LEFT JOIN T_PACOTE Pacote ON Pacote.PCT_LOG_KEY = CTeTerceiro.CPS_IDENTIFICACAO_PACOTE
                            LEFT JOIN T_CARGA_PEDIDO_PACOTE CargaPedidoPacote ON Pacote.PCT_CODIGO = CargaPedidoPacote.PCT_CODIGO
                            LEFT JOIN T_CARGA_PEDIDO CargaPedido on CargaPedidoPacote.CPE_CODIGO = CargaPedido.CPE_CODIGO
                            WHERE CargaPedido.car_codigo = {codigoCarga}";

            var query = this.SessionNHiBernate.CreateSQLQuery(sql);
            var result = query.UniqueResult();

            return Convert.ToInt32(result);
        }

        public List<Dominio.Entidades.Embarcador.CTe.CTeTerceiro> BuscarPorCTePorCarga(int codigoCarga)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoCTeParaSubContratacao>();
            var result = from obj in query where obj.CargaPedido.Carga.Codigo == codigoCarga select obj;
            return result.Select(o => o.CTeTerceiro).ToList();
        }

        public int ContarPedidoCTeParaSubContratacaoPorCarga(int codigoCarga)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoCTeParaSubContratacao>();
            var result = from obj in query where obj.CargaPedido.Carga.Codigo == codigoCarga select obj;
            return result.Count();
        }

        public bool ExisteEmOutroPedido(int codigoCTeTerceiro, int codigoCargaPedido)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoCTeParaSubContratacao>();

            query = query.Where(o => o.CargaPedido.Codigo != codigoCargaPedido && o.CTeTerceiro.Codigo == codigoCTeTerceiro && o.CTeTerceiro.Ativo);

            return query.Any();
        }

        public List<Dominio.Entidades.Embarcador.Pedidos.PedidoCTeParaSubContratacao> BuscarPorCarga(int codigoCarga)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoCTeParaSubContratacao>();
            var result = from obj in query where obj.CargaPedido.Carga.Codigo == codigoCarga && obj.CTeTerceiro.Ativo select obj;
            return result
                .Fetch(obj => obj.CTeTerceiro)
                .ThenFetch(obj => obj.Emitente)
                .Fetch(obj => obj.CargaPedido)
                .Fetch(obj => obj.CTeTerceiro)
                .ThenFetch(obj => obj.Remetente)
                .Fetch(obj => obj.CTeTerceiro)
                .ThenFetch(obj => obj.Destinatario)
                .ToList();
        }

        public List<Dominio.Entidades.Embarcador.Pedidos.PedidoCTeParaSubContratacao> BuscarPorCargaSemFetch(int codigoCarga)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoCTeParaSubContratacao>();

            var result = from obj in query where obj.CargaPedido.Carga.Codigo == codigoCarga && obj.CTeTerceiro.Ativo select obj;

            return result.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Pedidos.PedidoCTeParaSubContratacao> BuscarPorCargaPedido(int codigoCargaPedido)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoCTeParaSubContratacao>();
            var result = from obj in query where obj.CargaPedido.Codigo == codigoCargaPedido && obj.CTeTerceiro.Ativo select obj;
            return result
                .Fetch(obj => obj.CTeTerceiro)
                .ThenFetch(obj => obj.Emitente)
                .Fetch(obj => obj.CargaPedido)
                .Fetch(obj => obj.CTeTerceiro)
                .ThenFetch(obj => obj.Remetente)
                .Fetch(obj => obj.CTeTerceiro)
                .ThenFetch(obj => obj.Destinatario)
                .ToList();
        }

        public List<Dominio.ObjetosDeValor.Embarcador.Pedido.PedidoCTeParaSubContratacao> BuscarPorCargaPedidoParaProcessamento(int codigoCargaPedido)
        {
            IQueryable<Dominio.Entidades.Embarcador.Pedidos.PedidoCTeParaSubContratacao> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoCTeParaSubContratacao>()
                .Where(o => o.CargaPedido.Codigo == codigoCargaPedido && o.CTeTerceiro.Ativo);

            return query.Select(o => new Dominio.ObjetosDeValor.Embarcador.Pedido.PedidoCTeParaSubContratacao()
            {
                Codigo = o.Codigo,
                AliquotaIR = o.AliquotaIR,
                BaseCalculoIR = o.BaseCalculoIR,
                BaseCalculoISS = o.BaseCalculoISS,
                CST = o.CST,
                IncluirICMSBaseCalculo = o.IncluirICMSBaseCalculo,
                IncluirISSBaseCalculo = o.IncluirISSBaseCalculo,
                ObservacaoRegraICMSCTe = o.ObservacaoRegraICMSCTe,
                PercentualAliquota = o.PercentualAliquota,
                PercentualAliquotaInternaDifal = o.PercentualAliquotaInternaDifal,
                PercentualAliquotaISS = o.PercentualAliquotaISS,
                PercentualIncluirBaseCalculo = o.PercentualIncluirBaseCalculo,
                PercentualReducaoBC = o.PercentualReducaoBC,
                PercentualRetencaoISS = o.PercentualRetencaoISS,
                PossuiCTe = o.PossuiCTe,
                PossuiNFS = o.PossuiNFS,
                PossuiNFSManual = o.PossuiNFSManual,
                ReterIR = o.ReterIR,
                ValorIR = o.ValorIR,
                ValorRetencaoISS = o.ValorRetencaoISS,
                CFOP = o.CFOP == null ? null : new Dominio.ObjetosDeValor.Embarcador.Financeiro.CFOP()
                {
                    CodigoCFOP = o.CFOP.Codigo
                },
                ModeloDocumentoFiscal = o.ModeloDocumentoFiscal == null ? null : new Dominio.ObjetosDeValor.Embarcador.NotaFiscal.ModeloDocumentoFiscal()
                {
                    Codigo = o.ModeloDocumentoFiscal.Codigo
                },
                CTeTerceiro = new Dominio.ObjetosDeValor.Embarcador.CTe.CTeTerceiro()
                {
                    Codigo = o.CTeTerceiro.Codigo,
                    CST = o.CTeTerceiro.CST,
                    DataEmissao = o.CTeTerceiro.DataEmissao,
                    Numero = o.CTeTerceiro.Numero,
                    Peso = o.CTeTerceiro.Peso,
                    Serie = o.CTeTerceiro.Serie,
                    ValorAReceber = o.CTeTerceiro.ValorAReceber,
                    ValorICMS = o.CTeTerceiro.ValorICMS,
                    ValorPrestacaoServico = o.CTeTerceiro.ValorPrestacaoServico,
                    Remetente = new Dominio.ObjetosDeValor.Cliente()
                    {
                        CPFCNPJ = o.CTeTerceiro.Remetente.Cliente.Tipo == "E" ? o.CTeTerceiro.Remetente.Cliente.CPF_CNPJ.ToString("F0") : o.CTeTerceiro.Remetente.CPF_CNPJ.ToString(),
                        Tipo = o.CTeTerceiro.Remetente.Cliente.Tipo
                    },
                    Destinatario = new Dominio.ObjetosDeValor.Cliente()
                    {
                        CPFCNPJ = o.CTeTerceiro.Destinatario.Cliente.Tipo == "E" ? o.CTeTerceiro.Destinatario.Cliente.CPF_CNPJ.ToString("F0") : o.CTeTerceiro.Destinatario.CPF_CNPJ.ToString()
                    }
                },
                IBSCBS = new Dominio.ObjetosDeValor.CTe.IBSCBS
                {
                    AliquotaCBS = o.AliquotaCBS,
                    AliquotaIBSEstadual = o.AliquotaIBSEstadual,
                    AliquotaIBSMunicipal = o.AliquotaIBSMunicipal,
                    BaseCalculoIBSCBS = o.BaseCalculoIBSCBS,
                    ClassificacaoTributariaIBSCBS = o.ClassificacaoTributariaIBSCBS,
                    CodigoIndicadorOperacao = o.CodigoIndicadorOperacao,
                    CSTIBSCBS = o.CSTIBSCBS,
                    CodigoOutrasAliquotas = o.OutrasAliquotas != null ? o.OutrasAliquotas.Codigo : 0,
                    NBS = o.NBS,
                    PercentualReducaoCBS = o.PercentualReducaoCBS,
                    PercentualReducaoIBSEstadual = o.PercentualReducaoIBSEstadual,
                    PercentualReducaoIBSMunicipal = o.PercentualReducaoIBSMunicipal,
                    ValorCBS = o.ValorCBS,
                    ValorIBSEstadual = o.ValorIBSEstadual,
                    ValorIBSMunicipal = o.ValorIBSMunicipal
                }
            }).ToList();
        }

        public List<Dominio.Entidades.Embarcador.Pedidos.PedidoCTeParaSubContratacao> BuscarPorCargaPedido(int codigoCargaPedido, bool complementoFilialEmissora)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoCTeParaSubContratacao>();
            var result = from obj in query where obj.CargaPedido.Codigo == codigoCargaPedido && obj.CteSubContratacaoFilialEmissora == complementoFilialEmissora && obj.CTeTerceiro.Ativo select obj;
            return result
                .Fetch(obj => obj.CTeTerceiro)
                .ThenFetch(obj => obj.Emitente)
                .Fetch(obj => obj.CargaPedido)
                .Fetch(obj => obj.CTeTerceiro)
                .ThenFetch(obj => obj.Remetente)
                .Fetch(obj => obj.CTeTerceiro)
                .ThenFetch(obj => obj.Destinatario)
                .ToList();
        }

        public List<int> BuscarCodigosPorCargaPedido(int codigoCargaPedido)
        {
            IQueryable<Dominio.Entidades.Embarcador.Pedidos.PedidoCTeParaSubContratacao> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoCTeParaSubContratacao>();

            query = query.Where(obj => obj.CargaPedido.Codigo == codigoCargaPedido && obj.CTeTerceiro.Ativo);

            return query.Select(o => o.Codigo).ToList();
        }

        public List<int> BuscarCodigosPorCargaPedido(int codigoCargaPedido, bool filialEmissora)
        {
            IQueryable<Dominio.Entidades.Embarcador.Pedidos.PedidoCTeParaSubContratacao> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoCTeParaSubContratacao>();

            query = query.Where(obj => obj.CargaPedido.Codigo == codigoCargaPedido && obj.CteSubContratacaoFilialEmissora == filialEmissora && obj.CTeTerceiro.Ativo);

            return query.Select(o => o.Codigo).ToList();
        }

        public List<int> BuscarCodigosPorCargaPedidoTerminalOrigemTerminalDestinoSacado(int codigoCargaPedido, int codigoTerminalOrigem, int codigoTerminalDestino, string cnpjTomador)
        {
            IQueryable<Dominio.Entidades.Embarcador.Pedidos.PedidoCTeParaSubContratacao> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoCTeParaSubContratacao>();
            query = query.Where(obj => obj.CargaPedido.Codigo == codigoCargaPedido && obj.CTeTerceiro.Ativo && obj.CTeTerceiro != null);

            query = query.Where(obj => obj.CTeTerceiro.TipoTomador == Dominio.Enumeradores.TipoTomador.Destinatario ? obj.CTeTerceiro.Destinatario.CPF_CNPJ == cnpjTomador :
                                      obj.CTeTerceiro.TipoTomador == Dominio.Enumeradores.TipoTomador.Remetente ? obj.CTeTerceiro.Remetente.CPF_CNPJ == cnpjTomador :
                                      obj.CTeTerceiro.TipoTomador == Dominio.Enumeradores.TipoTomador.Expedidor ? obj.CTeTerceiro.Expedidor.CPF_CNPJ == cnpjTomador :
                                      obj.CTeTerceiro.TipoTomador == Dominio.Enumeradores.TipoTomador.Recebedor ? obj.CTeTerceiro.Recebedor.CPF_CNPJ == cnpjTomador :
                                      obj.CTeTerceiro.TipoTomador == Dominio.Enumeradores.TipoTomador.Outros ? obj.CTeTerceiro.OutrosTomador.CPF_CNPJ == cnpjTomador :
                                      obj.CTeTerceiro.Remetente.CPF_CNPJ == cnpjTomador);

            IQueryable<Dominio.Entidades.ConhecimentoDeTransporteEletronico> queryCTe = this.SessionNHiBernate.Query<Dominio.Entidades.ConhecimentoDeTransporteEletronico>();
            queryCTe = queryCTe.Where(obj => obj.Status == "A" && obj.TipoModal == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoModal.Multimodal && obj.TerminalDestino != null && obj.TerminalOrigem != null && obj.TerminalDestino.Codigo == codigoTerminalDestino && obj.TerminalOrigem.Codigo == codigoTerminalOrigem);

            return query.Where(o => queryCTe.Any(c => c.Chave == o.CTeTerceiro.ChaveAcesso)).Select(o => o.Codigo).ToList();
        }

        public List<int> BuscarCodigosPorCargaPedidoTerminalOrigemTerminalDestino(int codigoCargaPedido, int codigoTerminalOrigem, int codigoTerminalDestino)
        {
            IQueryable<Dominio.Entidades.Embarcador.Pedidos.PedidoCTeParaSubContratacao> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoCTeParaSubContratacao>();
            query = query.Where(obj => obj.CargaPedido.Codigo == codigoCargaPedido && obj.CTeTerceiro.Ativo && obj.CTeTerceiro != null);

            IQueryable<Dominio.Entidades.ConhecimentoDeTransporteEletronico> queryCTe = this.SessionNHiBernate.Query<Dominio.Entidades.ConhecimentoDeTransporteEletronico>();
            queryCTe = queryCTe.Where(obj => obj.Status == "A" && obj.TipoModal == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoModal.Multimodal && obj.TerminalDestino != null && obj.TerminalOrigem != null && obj.TerminalDestino.Codigo == codigoTerminalDestino && obj.TerminalOrigem.Codigo == codigoTerminalOrigem);

            return query.Where(o => queryCTe.Any(c => c.Chave == o.CTeTerceiro.ChaveAcesso)).Select(o => o.Codigo).ToList();
        }

        public List<int> BuscarCodigosPorDestinatarioRemetenteSacado(int codigoCargaPedido, string cnpjDestinatario, string cnpjRemetente, string cnpjTomador)
        {
            IQueryable<Dominio.Entidades.Embarcador.Pedidos.PedidoCTeParaSubContratacao> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoCTeParaSubContratacao>();

            query = query.Where(obj => obj.CTeTerceiro.TipoTomador == Dominio.Enumeradores.TipoTomador.Destinatario ? obj.CTeTerceiro.Destinatario.CPF_CNPJ == cnpjTomador :
                                      obj.CTeTerceiro.TipoTomador == Dominio.Enumeradores.TipoTomador.Remetente ? obj.CTeTerceiro.Remetente.CPF_CNPJ == cnpjTomador :
                                      obj.CTeTerceiro.TipoTomador == Dominio.Enumeradores.TipoTomador.Expedidor ? obj.CTeTerceiro.Expedidor.CPF_CNPJ == cnpjTomador :
                                      obj.CTeTerceiro.TipoTomador == Dominio.Enumeradores.TipoTomador.Recebedor ? obj.CTeTerceiro.Recebedor.CPF_CNPJ == cnpjTomador :
                                      obj.CTeTerceiro.TipoTomador == Dominio.Enumeradores.TipoTomador.Outros ? obj.CTeTerceiro.OutrosTomador.CPF_CNPJ == cnpjTomador :
                                      obj.CTeTerceiro.Remetente.CPF_CNPJ == cnpjTomador);

            query = query.Where(obj => obj.CargaPedido.Codigo == codigoCargaPedido && obj.CTeTerceiro.Ativo && obj.CTeTerceiro != null && obj.CTeTerceiro.Destinatario.CPF_CNPJ == cnpjDestinatario && obj.CTeTerceiro.Remetente.CPF_CNPJ == cnpjRemetente);

            return query.Select(o => o.Codigo).ToList();
        }

        public List<int> BuscarCodigosPorDestinatarioRemetente(int codigoCargaPedido, string cnpjDestinatario, string cnpjRemetente)
        {
            IQueryable<Dominio.Entidades.Embarcador.Pedidos.PedidoCTeParaSubContratacao> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoCTeParaSubContratacao>();

            query = query.Where(obj => obj.CargaPedido.Codigo == codigoCargaPedido && obj.CTeTerceiro.Ativo && obj.CTeTerceiro != null && obj.CTeTerceiro.Destinatario.CPF_CNPJ == cnpjDestinatario && obj.CTeTerceiro.Remetente.CPF_CNPJ == cnpjRemetente);

            return query.Select(o => o.Codigo).ToList();
        }

        public List<int> BuscarCodigosPorTomador(int codigoCargaPedido, string cnpjTomador)
        {
            IQueryable<Dominio.Entidades.Embarcador.Pedidos.PedidoCTeParaSubContratacao> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoCTeParaSubContratacao>();

            query = query.Where(obj => obj.CargaPedido.Codigo == codigoCargaPedido && obj.CTeTerceiro.Ativo && obj.CTeTerceiro != null);
            query = query.Where(obj => obj.CTeTerceiro.TipoTomador == Dominio.Enumeradores.TipoTomador.Destinatario ? obj.CTeTerceiro.Destinatario.CPF_CNPJ == cnpjTomador :
                                       obj.CTeTerceiro.TipoTomador == Dominio.Enumeradores.TipoTomador.Remetente ? obj.CTeTerceiro.Remetente.CPF_CNPJ == cnpjTomador :
                                       obj.CTeTerceiro.TipoTomador == Dominio.Enumeradores.TipoTomador.Expedidor ? obj.CTeTerceiro.Expedidor.CPF_CNPJ == cnpjTomador :
                                       obj.CTeTerceiro.TipoTomador == Dominio.Enumeradores.TipoTomador.Recebedor ? obj.CTeTerceiro.Recebedor.CPF_CNPJ == cnpjTomador :
                                       obj.CTeTerceiro.TipoTomador == Dominio.Enumeradores.TipoTomador.Outros ? obj.CTeTerceiro.OutrosTomador.CPF_CNPJ == cnpjTomador :
                                       obj.CTeTerceiro.Remetente.CPF_CNPJ == cnpjTomador);

            return query.Select(o => o.Codigo).ToList();
        }

        public List<int> BuscarCodigosPorBookingRemetenteDestinatarioExpedidorRecebedorSacado(int codigoCargaPedido, string numeroBooking, string cnpjTomador, string cnpjRemetente, string cnpjDestinatario, string cnpjExpedidor, string cnpjRecebedor)
        {
            IQueryable<Dominio.Entidades.Embarcador.Cargas.CargaPedido> queryCargaPedido = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedido>();
            queryCargaPedido = queryCargaPedido.Where(obj => obj.Pedido.NumeroBooking == numeroBooking);

            IQueryable<Dominio.Entidades.Embarcador.Cargas.CargaCTe> queryCargaCTe = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCTe>();
            queryCargaCTe = queryCargaCTe.Where(obj => queryCargaPedido.Any(o => o.Carga == obj.Carga));

            IQueryable<Dominio.Entidades.Embarcador.Pedidos.PedidoCTeParaSubContratacao> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoCTeParaSubContratacao>();
            query = query.Where(obj => obj.CargaPedido.Codigo == codigoCargaPedido && obj.CTeTerceiro.Ativo && obj.CTeTerceiro != null);

            query = query.Where(obj => obj.CTeTerceiro.TipoTomador == Dominio.Enumeradores.TipoTomador.Destinatario ? obj.CTeTerceiro.Destinatario.CPF_CNPJ == cnpjTomador :
                                       obj.CTeTerceiro.TipoTomador == Dominio.Enumeradores.TipoTomador.Remetente ? obj.CTeTerceiro.Remetente.CPF_CNPJ == cnpjTomador :
                                       obj.CTeTerceiro.TipoTomador == Dominio.Enumeradores.TipoTomador.Expedidor ? obj.CTeTerceiro.Expedidor.CPF_CNPJ == cnpjTomador :
                                       obj.CTeTerceiro.TipoTomador == Dominio.Enumeradores.TipoTomador.Recebedor ? obj.CTeTerceiro.Recebedor.CPF_CNPJ == cnpjTomador :
                                       obj.CTeTerceiro.TipoTomador == Dominio.Enumeradores.TipoTomador.Outros ? obj.CTeTerceiro.OutrosTomador.CPF_CNPJ == cnpjTomador :
                                       obj.CTeTerceiro.Remetente.CPF_CNPJ == cnpjTomador);

            query = query.Where(obj => obj.CargaPedido.Codigo == codigoCargaPedido && obj.CTeTerceiro.Ativo && obj.CTeTerceiro != null && obj.CTeTerceiro.Destinatario.CPF_CNPJ == cnpjDestinatario && obj.CTeTerceiro.Remetente.CPF_CNPJ == cnpjRemetente);

            if (cnpjRecebedor != "" && cnpjRecebedor != "0")
                query = query.Where(obj => obj.CTeTerceiro.Recebedor.CPF_CNPJ == cnpjRecebedor);

            if (cnpjExpedidor != "" && cnpjExpedidor != "0")
                query = query.Where(obj => obj.CTeTerceiro.Expedidor.CPF_CNPJ == cnpjExpedidor);

            IQueryable<Dominio.Entidades.ConhecimentoDeTransporteEletronico> queryCTe = this.SessionNHiBernate.Query<Dominio.Entidades.ConhecimentoDeTransporteEletronico>();
            queryCTe = queryCTe.Where(obj => obj.Status == "A" && obj.TipoModal == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoModal.Multimodal);
            queryCTe = queryCTe.Where(obj => queryCargaCTe.Any(o => o.CTe == obj));

            return query.Where(o => queryCTe.Any(c => c.Chave == o.CTeTerceiro.ChaveAcesso)).Select(o => o.Codigo).ToList();
        }

        public List<int> BuscarCodigosPorBookingSacado(int codigoCargaPedido, string numeroBooking, string cnpjTomador)
        {
            IQueryable<Dominio.Entidades.Embarcador.Cargas.CargaPedido> queryCargaPedido = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedido>();
            queryCargaPedido = queryCargaPedido.Where(obj => obj.Pedido.NumeroBooking == numeroBooking);

            IQueryable<Dominio.Entidades.Embarcador.Cargas.CargaCTe> queryCargaCTe = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCTe>();
            queryCargaCTe = queryCargaCTe.Where(obj => queryCargaPedido.Any(o => o.Carga == obj.Carga));

            IQueryable<Dominio.Entidades.Embarcador.Pedidos.PedidoCTeParaSubContratacao> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoCTeParaSubContratacao>();
            query = query.Where(obj => obj.CargaPedido.Codigo == codigoCargaPedido && obj.CTeTerceiro.Ativo && obj.CTeTerceiro != null);

            query = query.Where(obj => obj.CTeTerceiro.TipoTomador == Dominio.Enumeradores.TipoTomador.Destinatario ? obj.CTeTerceiro.Destinatario.CPF_CNPJ == cnpjTomador :
                                       obj.CTeTerceiro.TipoTomador == Dominio.Enumeradores.TipoTomador.Remetente ? obj.CTeTerceiro.Remetente.CPF_CNPJ == cnpjTomador :
                                       obj.CTeTerceiro.TipoTomador == Dominio.Enumeradores.TipoTomador.Expedidor ? obj.CTeTerceiro.Expedidor.CPF_CNPJ == cnpjTomador :
                                       obj.CTeTerceiro.TipoTomador == Dominio.Enumeradores.TipoTomador.Recebedor ? obj.CTeTerceiro.Recebedor.CPF_CNPJ == cnpjTomador :
                                       obj.CTeTerceiro.TipoTomador == Dominio.Enumeradores.TipoTomador.Outros ? obj.CTeTerceiro.OutrosTomador.CPF_CNPJ == cnpjTomador :
                                       obj.CTeTerceiro.Remetente.CPF_CNPJ == cnpjTomador);

            IQueryable<Dominio.Entidades.ConhecimentoDeTransporteEletronico> queryCTe = this.SessionNHiBernate.Query<Dominio.Entidades.ConhecimentoDeTransporteEletronico>();
            queryCTe = queryCTe.Where(obj => obj.Status == "A" && obj.TipoModal == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoModal.Multimodal);
            queryCTe = queryCTe.Where(obj => queryCargaCTe.Any(o => o.CTe == obj));

            return query.Where(o => queryCTe.Any(c => c.Chave == o.CTeTerceiro.ChaveAcesso)).Select(o => o.Codigo).ToList();
        }

        public List<int> BuscarCodigosPorBooking(int codigoCargaPedido, string numeroBooking)
        {
            IQueryable<Dominio.Entidades.Embarcador.Cargas.CargaPedido> queryCargaPedido = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedido>();
            queryCargaPedido = queryCargaPedido.Where(obj => obj.Pedido.NumeroBooking == numeroBooking);

            IQueryable<Dominio.Entidades.Embarcador.Cargas.CargaCTe> queryCargaCTe = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCTe>();
            queryCargaCTe = queryCargaCTe.Where(obj => queryCargaPedido.Any(o => o.Carga == obj.Carga));

            IQueryable<Dominio.Entidades.Embarcador.Pedidos.PedidoCTeParaSubContratacao> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoCTeParaSubContratacao>();
            query = query.Where(obj => obj.CargaPedido.Codigo == codigoCargaPedido && obj.CTeTerceiro.Ativo && obj.CTeTerceiro != null);

            IQueryable<Dominio.Entidades.ConhecimentoDeTransporteEletronico> queryCTe = this.SessionNHiBernate.Query<Dominio.Entidades.ConhecimentoDeTransporteEletronico>();
            queryCTe = queryCTe.Where(obj => obj.Status == "A" && obj.TipoModal == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoModal.Multimodal);
            queryCTe = queryCTe.Where(obj => queryCargaCTe.Any(o => o.CTe == obj));

            return query.Where(o => queryCTe.Any(c => c.Chave == o.CTeTerceiro.ChaveAcesso)).Select(o => o.Codigo).ToList();
        }

        public List<int> BuscarCodigosPorContainer(int codigoCargaPedido, string numeroContainer)
        {
            IQueryable<Dominio.Entidades.Embarcador.Cargas.CargaPedido> queryCargaPedido = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedido>();
            queryCargaPedido = queryCargaPedido.Where(obj => obj.Pedido.Container != null && obj.Pedido.Container.Numero == numeroContainer);

            IQueryable<Dominio.Entidades.Embarcador.Cargas.CargaCTe> queryCargaCTe = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCTe>();
            queryCargaCTe = queryCargaCTe.Where(obj => queryCargaPedido.Any(o => o.Carga == obj.Carga));

            IQueryable<Dominio.Entidades.Embarcador.Pedidos.PedidoCTeParaSubContratacao> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoCTeParaSubContratacao>();
            query = query.Where(obj => obj.CargaPedido.Codigo == codigoCargaPedido && obj.CTeTerceiro.Ativo && obj.CTeTerceiro != null);

            IQueryable<Dominio.Entidades.ConhecimentoDeTransporteEletronico> queryCTe = this.SessionNHiBernate.Query<Dominio.Entidades.ConhecimentoDeTransporteEletronico>();
            queryCTe = queryCTe.Where(obj => obj.Status == "A" && obj.TipoModal == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoModal.Multimodal);
            queryCTe = queryCTe.Where(obj => queryCargaCTe.Any(o => o.CTe == obj));

            return query.Where(o => queryCTe.Any(c => c.Chave == o.CTeTerceiro.ChaveAcesso)).Select(o => o.Codigo).ToList();
        }

        public List<int> BuscarCodigosTerminaisOrigemPorCargaPedido(int codigoCargaPedido)
        {
            IQueryable<Dominio.Entidades.Embarcador.Pedidos.PedidoCTeParaSubContratacao> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoCTeParaSubContratacao>();
            query = query.Where(obj => obj.CargaPedido.Codigo == codigoCargaPedido && obj.CTeTerceiro.Ativo && obj.CTeTerceiro != null);

            IQueryable<Dominio.Entidades.ConhecimentoDeTransporteEletronico> queryCTe = this.SessionNHiBernate.Query<Dominio.Entidades.ConhecimentoDeTransporteEletronico>();
            queryCTe = queryCTe.Where(obj => obj.Status == "A" && obj.TipoModal == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoModal.Multimodal && obj.TerminalDestino != null && obj.TerminalOrigem != null);

            queryCTe = queryCTe.Where(obj => query.Any(o => o.CTeTerceiro.ChaveAcesso == obj.Chave));

            return queryCTe.Select(o => o.TerminalOrigem.Codigo).Distinct().ToList();
        }

        public List<int> BuscarCodigosTerminaisDestinoPorCargaPedido(int codigoCargaPedido)
        {
            IQueryable<Dominio.Entidades.Embarcador.Pedidos.PedidoCTeParaSubContratacao> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoCTeParaSubContratacao>();
            query = query.Where(obj => obj.CargaPedido.Codigo == codigoCargaPedido && obj.CTeTerceiro.Ativo && obj.CTeTerceiro != null);

            IQueryable<Dominio.Entidades.ConhecimentoDeTransporteEletronico> queryCTe = this.SessionNHiBernate.Query<Dominio.Entidades.ConhecimentoDeTransporteEletronico>();
            queryCTe = queryCTe.Where(obj => obj.Status == "A" && obj.TipoModal == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoModal.Multimodal && obj.TerminalDestino != null && obj.TerminalOrigem != null);

            queryCTe = queryCTe.Where(obj => query.Any(o => o.CTeTerceiro.ChaveAcesso == obj.Chave));

            return queryCTe.Select(o => o.TerminalDestino.Codigo).Distinct().ToList();
        }

        public List<string> BuscarBookingsPorCargaPedido(int codigoCargaPedido)
        {
            IQueryable<Dominio.Entidades.Embarcador.Pedidos.PedidoCTeParaSubContratacao> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoCTeParaSubContratacao>();
            query = query.Where(obj => obj.CargaPedido.Codigo == codigoCargaPedido && obj.CTeTerceiro.Ativo && obj.CTeTerceiro != null);

            IQueryable<Dominio.Entidades.ConhecimentoDeTransporteEletronico> queryCTe = this.SessionNHiBernate.Query<Dominio.Entidades.ConhecimentoDeTransporteEletronico>();
            queryCTe = queryCTe.Where(obj => obj.Status == "A" && obj.TipoModal == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoModal.Multimodal);
            queryCTe = queryCTe.Where(obj => query.Any(o => o.CTeTerceiro.ChaveAcesso == obj.Chave));

            IQueryable<Dominio.Entidades.Embarcador.Cargas.CargaCTe> queryCargaCTe = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCTe>();
            queryCargaCTe = queryCargaCTe.Where(obj => queryCTe.Any(o => o == obj.CTe));

            IQueryable<Dominio.Entidades.Embarcador.Cargas.CargaPedido> queryCargaPedido = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedido>();
            queryCargaPedido = queryCargaPedido.Where(obj => queryCargaCTe.Any(o => o.Carga == obj.Carga));

            return queryCargaPedido.Select(o => o.Pedido.NumeroBooking).Distinct().ToList();
        }

        public List<string> BuscarContainerPorCargaPedido(int codigoCargaPedido)
        {
            IQueryable<Dominio.Entidades.Embarcador.Pedidos.PedidoCTeParaSubContratacao> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoCTeParaSubContratacao>();
            query = query.Where(obj => obj.CargaPedido.Codigo == codigoCargaPedido && obj.CTeTerceiro.Ativo && obj.CTeTerceiro != null);

            IQueryable<Dominio.Entidades.ConhecimentoDeTransporteEletronico> queryCTe = this.SessionNHiBernate.Query<Dominio.Entidades.ConhecimentoDeTransporteEletronico>();
            queryCTe = queryCTe.Where(obj => obj.Status == "A" && obj.TipoModal == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoModal.Multimodal);
            queryCTe = queryCTe.Where(obj => query.Any(o => o.CTeTerceiro.ChaveAcesso == obj.Chave));

            IQueryable<Dominio.Entidades.Embarcador.Cargas.CargaCTe> queryCargaCTe = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCTe>();
            queryCargaCTe = queryCargaCTe.Where(obj => queryCTe.Any(o => o == obj.CTe));

            IQueryable<Dominio.Entidades.Embarcador.Cargas.CargaPedido> queryCargaPedido = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedido>();
            queryCargaPedido = queryCargaPedido.Where(obj => obj.Pedido.Container != null && queryCargaCTe.Any(o => o.Carga == obj.Carga));

            return queryCargaPedido.Select(o => o.Pedido.Container.Numero).Distinct().ToList();
        }

        public Dominio.Entidades.Embarcador.Pedidos.PedidoCTeParaSubContratacao BuscarPorCodigoComFetch(int codigoPedidoCTeParaSubcontratacao)
        {
            IQueryable<Dominio.Entidades.Embarcador.Pedidos.PedidoCTeParaSubContratacao> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoCTeParaSubContratacao>();

            query = query.Where(obj => obj.Codigo == codigoPedidoCTeParaSubcontratacao);

            return query.Fetch(obj => obj.CTeTerceiro)
                        .ThenFetch(obj => obj.Emitente)
                        .Fetch(obj => obj.CTeTerceiro)
                        .ThenFetch(obj => obj.Remetente)
                        .Fetch(obj => obj.CTeTerceiro)
                        .ThenFetch(obj => obj.Destinatario)
                        .Fetch(obj => obj.CargaPedido)
                        .FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Pedidos.PedidoCTeParaSubContratacao> BuscarPorCargaPedidoComFetch(List<int> codigoCargaPedido, bool filialEmissora)
        {
            IQueryable<Dominio.Entidades.Embarcador.Pedidos.PedidoCTeParaSubContratacao> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoCTeParaSubContratacao>();

            query = query.Where(obj => codigoCargaPedido.Contains(obj.CargaPedido.Codigo) && obj.CteSubContratacaoFilialEmissora == filialEmissora && obj.CTeTerceiro.Ativo);

            return query.Fetch(obj => obj.CTeTerceiro)
                        .ThenFetch(obj => obj.Emitente)
                        .ThenFetch(obj => obj.Localidade)
                        .ThenFetch(obj => obj.Pais)
                        .Fetch(obj => obj.CTeTerceiro)
                        .ThenFetch(obj => obj.Remetente)
                        .ThenFetch(obj => obj.Cliente)
                        .Fetch(obj => obj.CTeTerceiro)
                        .ThenFetch(obj => obj.Destinatario)
                        .ThenFetch(obj => obj.Cliente)
                        .Fetch(obj => obj.CargaPedido).ToList();
        }

        public List<Dominio.Entidades.Embarcador.Pedidos.PedidoCTeParaSubContratacao> BuscarPorCargaPedidoComFetch(int codigoCargaPedido)
        {
            IQueryable<Dominio.Entidades.Embarcador.Pedidos.PedidoCTeParaSubContratacao> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoCTeParaSubContratacao>();

            query = query.Where(obj => obj.CargaPedido.Codigo == codigoCargaPedido && obj.CTeTerceiro.Ativo);

            return query.Fetch(obj => obj.CTeTerceiro)
                        .ThenFetch(obj => obj.Emitente)
                        .Fetch(obj => obj.CTeTerceiro)
                        .ThenFetch(obj => obj.Remetente)
                        .ThenFetch(obj => obj.Cliente)
                        .Fetch(obj => obj.CTeTerceiro)
                        .ThenFetch(obj => obj.Destinatario)
                        .ThenFetch(obj => obj.Cliente)
                        .Fetch(obj => obj.CargaPedido).ToList();
        }

        public List<Dominio.Entidades.Embarcador.Pedidos.PedidoCTeParaSubContratacao> BuscarPorCodigosComFetch(List<int> codigosPedidoCTeParaSubcontratacao)
        {
            IQueryable<Dominio.Entidades.Embarcador.Pedidos.PedidoCTeParaSubContratacao> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoCTeParaSubContratacao>();

            query = query.Where(obj => codigosPedidoCTeParaSubcontratacao.Contains(obj.Codigo));

            return query.Fetch(obj => obj.CTeTerceiro).ThenFetch(obj => obj.Emitente)
                        .Fetch(obj => obj.CargaPedido)
                        .Distinct().ToList();
        }

        public List<string> BuscarCodigosDestinatarioPorCargaPedido(int codigoCargaPedido)
        {
            IQueryable<Dominio.Entidades.Embarcador.Pedidos.PedidoCTeParaSubContratacao> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoCTeParaSubContratacao>();

            query = query.Where(obj => obj.CargaPedido.Codigo == codigoCargaPedido && obj.CTeTerceiro.Ativo && obj.CTeTerceiro != null);

            return query.Select(o => o.CTeTerceiro.Destinatario.CPF_CNPJ).Distinct().ToList();
        }

        public List<string> BuscarCodigosRecebedorPorCargaPedido(int codigoCargaPedido)
        {
            IQueryable<Dominio.Entidades.Embarcador.Pedidos.PedidoCTeParaSubContratacao> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoCTeParaSubContratacao>();

            query = query.Where(obj => obj.CargaPedido.Codigo == codigoCargaPedido && obj.CTeTerceiro.Ativo && obj.CTeTerceiro != null);

            return query.Select(o => o.CTeTerceiro.Recebedor.CPF_CNPJ).Distinct().ToList();
        }

        public List<string> BuscarCodigosExpedidorPorCargaPedido(int codigoCargaPedido)
        {
            IQueryable<Dominio.Entidades.Embarcador.Pedidos.PedidoCTeParaSubContratacao> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoCTeParaSubContratacao>();

            query = query.Where(obj => obj.CargaPedido.Codigo == codigoCargaPedido && obj.CTeTerceiro.Ativo && obj.CTeTerceiro != null);

            return query.Select(o => o.CTeTerceiro.Expedidor.CPF_CNPJ).Distinct().ToList();
        }

        public List<string> BuscarCodigosRemetentePorCargaPedido(int codigoCargaPedido)
        {
            IQueryable<Dominio.Entidades.Embarcador.Pedidos.PedidoCTeParaSubContratacao> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoCTeParaSubContratacao>();

            query = query.Where(obj => obj.CargaPedido.Codigo == codigoCargaPedido && obj.CTeTerceiro.Ativo && obj.CTeTerceiro != null);

            return query.Select(o => o.CTeTerceiro.Remetente.CPF_CNPJ).Distinct().ToList();
        }

        public List<string> BuscarCodigosTomadorPorCargaPedido(int codigoCargaPedido)
        {
            IQueryable<Dominio.Entidades.Embarcador.Pedidos.PedidoCTeParaSubContratacao> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoCTeParaSubContratacao>();

            query = query.Where(obj => obj.CargaPedido.Codigo == codigoCargaPedido && obj.CTeTerceiro.Ativo && obj.CTeTerceiro != null);

            return query.Select(obj => obj.CTeTerceiro.TipoTomador == Dominio.Enumeradores.TipoTomador.Destinatario ? obj.CTeTerceiro.Destinatario.CPF_CNPJ :
                                       obj.CTeTerceiro.TipoTomador == Dominio.Enumeradores.TipoTomador.Remetente ? obj.CTeTerceiro.Remetente.CPF_CNPJ :
                                       obj.CTeTerceiro.TipoTomador == Dominio.Enumeradores.TipoTomador.Expedidor ? obj.CTeTerceiro.Expedidor.CPF_CNPJ :
                                       obj.CTeTerceiro.TipoTomador == Dominio.Enumeradores.TipoTomador.Recebedor ? obj.CTeTerceiro.Recebedor.CPF_CNPJ :
                                       obj.CTeTerceiro.TipoTomador == Dominio.Enumeradores.TipoTomador.Outros ? obj.CTeTerceiro.OutrosTomador.CPF_CNPJ :
                                       obj.CTeTerceiro.Remetente.CPF_CNPJ).Distinct().ToList();
        }

        public bool ExistePorCargaPedido(int codigoCargaPedido)
        {
            IQueryable<Dominio.Entidades.Embarcador.Pedidos.PedidoCTeParaSubContratacao> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoCTeParaSubContratacao>();

            query = query.Where(o => o.CargaPedido.Codigo == codigoCargaPedido && o.CTeTerceiro.Ativo);

            return query.Any();
        }

        public bool ExistePorCargaPedidoEChave(int codigoCargaPedido, string chave)
        {
            IQueryable<Dominio.Entidades.Embarcador.Pedidos.PedidoCTeParaSubContratacao> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoCTeParaSubContratacao>();

            query = query.Where(o => o.CargaPedido.Codigo == codigoCargaPedido && o.CTeTerceiro.ChaveAcesso == chave);

            return query.Any();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.CargaCTe> ObterCargaCTe(List<string> chaveCTe)
        {
            IQueryable<Dominio.Entidades.Embarcador.Cargas.CargaCTe> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCTe>();
            query = query.Where(o => chaveCTe.Contains(o.CTe.Chave));

            return query.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> ObterCargaPedido(int codigoCargaPedido)
        {
            IQueryable<Dominio.Entidades.Embarcador.Cargas.CargaPedidoDocumentoCTe> queryCTes = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedidoDocumentoCTe>();
            queryCTes = queryCTes.Where(o => o.CargaPedido.Codigo == codigoCargaPedido);

            IQueryable<Dominio.Entidades.Embarcador.Cargas.CargaCTe> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCTe>();
            query = query.Where(o => queryCTes.Any(c => c.CTe == o.CTe));

            IQueryable<Dominio.Entidades.Embarcador.Cargas.CargaPedido> queryCargaPedido = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedido>();
            queryCargaPedido = queryCargaPedido.Where(o => query.Any(p => p.Carga == o.Carga));

            return queryCargaPedido.ToList();
        }

        public string ObterNumeroBookingPedido(string chaveCTe)
        {
            IQueryable<Dominio.Entidades.Embarcador.Cargas.CargaCTe> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCTe>();
            query = query.Where(o => o.CTe.Chave == chaveCTe);

            IQueryable<Dominio.Entidades.Embarcador.Cargas.CargaPedido> queryCargaPedido = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedido>();
            queryCargaPedido = queryCargaPedido.Where(o => query.Any(p => p.Carga == o.Carga));

            return queryCargaPedido.Select(o => o.Pedido.NumeroBooking).FirstOrDefault() ?? "";
        }

        public decimal ObterDencidadeProdutoPedido(string chaveCTe)
        {
            IQueryable<Dominio.Entidades.Embarcador.Cargas.CargaCTe> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCTe>();
            query = query.Where(o => o.CTe.Chave == chaveCTe);

            IQueryable<Dominio.Entidades.Embarcador.Cargas.CargaPedido> queryCargaPedido = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedido>();
            queryCargaPedido = queryCargaPedido.Where(o => query.Any(p => p.Carga == o.Carga));

            IQueryable<Dominio.Entidades.Embarcador.Pedidos.PedidoProduto> queryPedidoProduto = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoProduto>();
            queryPedidoProduto = queryPedidoProduto.Where(o => queryCargaPedido.Any(p => p.Pedido == o.Pedido));

            return queryPedidoProduto.Count() > 0 ? queryPedidoProduto?.Average(o => o.Produto.MetroCubito) ?? 0 : 0;
        }

        public string ObterDescricaoCarrierPedido(string chaveCTe)
        {
            IQueryable<Dominio.Entidades.Embarcador.Cargas.CargaCTe> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCTe>();
            query = query.Where(o => o.CTe.Chave == chaveCTe);

            IQueryable<Dominio.Entidades.Embarcador.Cargas.CargaPedido> queryCargaPedido = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedido>();
            queryCargaPedido = queryCargaPedido.Where(o => query.Any(p => p.Carga == o.Carga));

            return queryCargaPedido.Select(o => o.Pedido.DescricaoCarrierNavioViagem).FirstOrDefault() ?? "";
        }

        public List<Dominio.Entidades.Embarcador.Pedidos.PedidoTransbordo> ObterTransbordosPedido(string chaveCTe)
        {
            IQueryable<Dominio.Entidades.Embarcador.Cargas.CargaCTe> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCTe>();
            query = query.Where(o => o.CTe.Chave == chaveCTe);

            IQueryable<Dominio.Entidades.Embarcador.Cargas.CargaPedido> queryCargaPedido = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedido>();
            queryCargaPedido = queryCargaPedido.Where(o => query.Any(p => p.Carga == o.Carga));

            IQueryable<Dominio.Entidades.Embarcador.Pedidos.PedidoTransbordo> queryPedidoTransbordo = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoTransbordo>();
            queryPedidoTransbordo = queryPedidoTransbordo.Where(o => queryCargaPedido.Any(p => p.Pedido == o.Pedido));

            return queryPedidoTransbordo.ToList();
        }

        public Dominio.Enumeradores.TipoPropostaFeeder ObterTipoPropostaFeederPedido(string chaveCTe)
        {
            IQueryable<Dominio.Entidades.Embarcador.Cargas.CargaCTe> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCTe>();
            query = query.Where(o => o.CTe.Chave == chaveCTe);

            IQueryable<Dominio.Entidades.Embarcador.Cargas.CargaPedido> queryCargaPedido = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedido>();
            queryCargaPedido = queryCargaPedido.Where(o => query.Any(p => p.Carga == o.Carga));

            return queryCargaPedido.Select(o => o.Pedido.TipoPropostaFeeder).FirstOrDefault();
        }

        public string ObterLacrePedido(string chaveCTe, int numeroLacre)
        {
            IQueryable<Dominio.Entidades.Embarcador.Cargas.CargaCTe> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCTe>();
            query = query.Where(o => o.CTe.Chave == chaveCTe);

            IQueryable<Dominio.Entidades.Embarcador.Cargas.CargaPedido> queryCargaPedido = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedido>();
            queryCargaPedido = queryCargaPedido.Where(o => query.Any(p => p.Carga == o.Carga));

            if (numeroLacre == 1)
                return queryCargaPedido.Select(o => o.Pedido.LacreContainerUm).FirstOrDefault();
            else if (numeroLacre == 2)
                return queryCargaPedido.Select(o => o.Pedido.LacreContainerDois).FirstOrDefault();
            else if (numeroLacre == 3)
                return queryCargaPedido.Select(o => o.Pedido.LacreContainerTres).FirstOrDefault();
            else
                return "";
        }

        public Dominio.Entidades.Embarcador.Pedidos.Container ObterContainerPedido(string chaveCTe)
        {
            IQueryable<Dominio.Entidades.Embarcador.Cargas.CargaCTe> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCTe>();
            query = query.Where(o => o.CTe.Chave == chaveCTe);

            IQueryable<Dominio.Entidades.Embarcador.Cargas.CargaPedido> queryCargaPedido = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedido>();
            queryCargaPedido = queryCargaPedido.Where(o => query.Any(p => p.Carga == o.Carga));

            return queryCargaPedido.Select(o => o.Pedido.Container).FirstOrDefault();
        }

        public decimal ObterValorFreteNegociavelPedido(int codigoCargaPedido)
        {
            IQueryable<Dominio.Entidades.Embarcador.Cargas.CargaPedidoDocumentoCTe> queryCTes = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedidoDocumentoCTe>();
            queryCTes = queryCTes.Where(o => o.CargaPedido.Codigo == codigoCargaPedido);

            IQueryable<Dominio.Entidades.Embarcador.Cargas.CargaCTe> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCTe>();
            query = query.Where(o => queryCTes.Any(c => c.CTe == o.CTe));

            IQueryable<Dominio.Entidades.Embarcador.Cargas.CargaPedido> queryCargaPedido = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedido>();
            queryCargaPedido = queryCargaPedido.Where(o => query.Any(p => p.Carga == o.Carga));

            return queryCargaPedido.Average(o => (decimal?)o.Pedido.ValorFreteNegociado) ?? 0m;
        }

        public decimal ObterAliquota(int codigoCargaPedido)
        {
            IQueryable<Dominio.Entidades.Embarcador.Cargas.CargaPedidoDocumentoCTe> queryCTes = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedidoDocumentoCTe>();
            queryCTes = queryCTes.Where(o => o.CargaPedido.Codigo == codigoCargaPedido);

            IQueryable<Dominio.Entidades.Embarcador.Cargas.CargaCTe> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCTe>();
            query = query.Where(o => queryCTes.Any(c => c.CTe == o.CTe));

            return query.Where(o => o.CTe.CST != "40" && o.CTe.CST != "41" && o.CTe.CST != "51").Average(o => (decimal?)o.CTe.AliquotaICMS) ?? 0m;
        }

        public decimal ObterValorFreteNegociavelPedido(string chaveCTe)
        {
            IQueryable<Dominio.Entidades.Embarcador.Cargas.CargaCTe> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCTe>();
            query = query.Where(o => o.CTe.Chave == chaveCTe);

            IQueryable<Dominio.Entidades.Embarcador.Cargas.CargaPedido> queryCargaPedido = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedido>();
            queryCargaPedido = queryCargaPedido.Where(o => query.Any(p => p.Carga == o.Carga));

            return queryCargaPedido.Average(o => (decimal?)o.Pedido.ValorFreteNegociado) ?? 0m;
        }

        public decimal ObterAliquota(string chaveCTe)
        {
            IQueryable<Dominio.Entidades.Embarcador.Cargas.CargaCTe> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCTe>();
            query = query.Where(o => o.CTe.Chave == chaveCTe);

            return query.Where(o => o.CTe.CST != "40" && o.CTe.CST != "41" && o.CTe.CST != "51").Average(o => (decimal?)o.CTe.AliquotaICMS) ?? 0m;
        }

        public decimal ObterValorCusteioSVMPedido(int codigoCargaPedido)
        {
            IQueryable<Dominio.Entidades.Embarcador.Cargas.CargaPedidoDocumentoCTe> queryCTes = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedidoDocumentoCTe>();
            queryCTes = queryCTes.Where(o => o.CargaPedido.Codigo == codigoCargaPedido);

            IQueryable<Dominio.Entidades.Embarcador.Cargas.CargaCTe> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCTe>();
            query = query.Where(o => queryCTes.Any(c => c.CTe == o.CTe));

            IQueryable<Dominio.Entidades.Embarcador.Cargas.CargaPedido> queryCargaPedido = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedido>();
            queryCargaPedido = queryCargaPedido.Where(o => query.Any(p => p.Carga == o.Carga));

            return queryCargaPedido.Average(o => (decimal?)o.Pedido.ValorCusteioSVM) ?? 0m;
        }

        public decimal ObterValorCusteioSVMPedido(string chaveCTe)
        {
            IQueryable<Dominio.Entidades.Embarcador.Cargas.CargaCTe> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCTe>();
            query = query.Where(o => o.CTe.Chave == chaveCTe);

            IQueryable<Dominio.Entidades.Embarcador.Cargas.CargaPedido> queryCargaPedido = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedido>();
            queryCargaPedido = queryCargaPedido.Where(o => query.Any(p => p.Carga == o.Carga));

            return queryCargaPedido.Average(o => (decimal?)o.Pedido.ValorCusteioSVM) ?? 0m;
        }

        public decimal ObterPesoPorCargaPedido(int codigoCargaPedido)
        {
            IQueryable<Dominio.Entidades.Embarcador.Pedidos.PedidoCTeParaSubContratacao> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoCTeParaSubContratacao>();

            query = query.Where(o => o.CargaPedido.Codigo == codigoCargaPedido && o.CTeTerceiro.Ativo);

            return query.Sum(o => (decimal?)o.CTeTerceiro.Peso) ?? 0m;
        }

        public Dominio.Entidades.Embarcador.Pedidos.PedidoCTeParaSubContratacao BuscarPorCargaOrigemECTeTerceiro(int codigoCarga, int codigoCTe)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoCTeParaSubContratacao>();
            var result = from obj in query where obj.CargaPedido.CargaOrigem.Codigo == codigoCarga && obj.CTeTerceiro.Codigo == codigoCTe && obj.CTeTerceiro.Ativo select obj;
            return result.Fetch(obj => obj.CargaPedido).FirstOrDefault();
        }

        public int ContarPorCargaEChave(int codigoCarga, string chaveCte)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoCTeParaSubContratacao>();
            var result = from obj in query where obj.CargaPedido.Carga.Codigo == codigoCarga && obj.CTeTerceiro.ChaveAcesso == chaveCte && obj.CTeTerceiro.Ativo select obj;
            return result.Count();
        }

        public int ContarPorCargaPedidoEChave(int codigoCargaPedido, string chaveCte)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoCTeParaSubContratacao>();
            var result = from obj in query where obj.CargaPedido.Codigo == codigoCargaPedido && obj.CTeTerceiro.ChaveAcesso == chaveCte && obj.CTeTerceiro.Ativo select obj;
            return result.Count();
        }

        public bool ContemPorCargaPedidoEChave(int codigoCargaPedido, string chaveCte)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoCTeParaSubContratacao>();
            var result = from obj in query where obj.CargaPedido.Codigo == codigoCargaPedido && obj.CTeTerceiro.ChaveAcesso == chaveCte && obj.CTeTerceiro.Ativo select obj;
            return result.Any();
        }

        public List<int> BuscarNumeroPorCargaEChaveNFe(int codigoCarga, List<string> chaveNFe)
        {
            IQueryable<Dominio.Entidades.Embarcador.Pedidos.PedidoCTeParaSubContratacao> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoCTeParaSubContratacao>();

            query = query.Where(o => o.CTeTerceiro.Ativo && o.CargaPedido.Carga.Codigo == codigoCarga && o.CTeTerceiro.CTesTerceiroNFes.Any(nfe => chaveNFe.Contains(nfe.Chave)));

            return query.Select(o => o.CTeTerceiro.Numero).ToList();
        }

        public bool ContemDocumentoSemNCM(int codigoCarga)
        {
            IQueryable<Dominio.Entidades.Embarcador.Pedidos.PedidoCTeParaSubContratacao> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoCTeParaSubContratacao>();

            query = query.Where(o => o.CargaPedido.Carga.Codigo == codigoCarga && o.CTeTerceiro.CTesTerceiroNFes.Any(nfe => nfe.NCM == "" || nfe.NCM == null));

            return query.Any();
        }

        public bool ContemDocumentoSemPeso(int codigoCarga)
        {
            IQueryable<Dominio.Entidades.Embarcador.Pedidos.PedidoCTeParaSubContratacao> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoCTeParaSubContratacao>();

            query = query.Where(o => o.CargaPedido.Carga.Codigo == codigoCarga && o.CTeTerceiro.CTesTerceiroNFes.Any(nfe => nfe.Peso == 0));

            return query.Any();
        }

        public bool ContemOutroDocumentoSemNCM(int codigoCarga)
        {
            IQueryable<Dominio.Entidades.Embarcador.Pedidos.PedidoCTeParaSubContratacao> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoCTeParaSubContratacao>();

            query = query.Where(o => o.CargaPedido.Carga.Codigo == codigoCarga && o.CTeTerceiro.CTesTerceiroOutrosDocumentos.Any(nfe => nfe.NCM == "" || nfe.NCM == null));

            return query.Any();
        }

        public bool ContemNotaEletronicaNaCarga(int codigoCarga)
        {
            IQueryable<Dominio.Entidades.Embarcador.Pedidos.PedidoCTeParaSubContratacao> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoCTeParaSubContratacao>();

            query = query.Where(o => o.CargaPedido.Carga.Codigo == codigoCarga && o.CTeTerceiro.CTesTerceiroNFes.Any(nfe => nfe.Chave != null && nfe.Chave != "" && nfe.Chave.Length == 44));

            return query.Any();
        }

        public int ContarPorCargaPedido(int codigoCargaPedido)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoCTeParaSubContratacao>();
            var result = from obj in query where obj.CargaPedido.Codigo == codigoCargaPedido && obj.CTeTerceiro.Ativo select obj;
            return result.Count();
        }

        public List<Dominio.ObjetosDeValor.Embarcador.Carga.CargaPedidoCteSubContratacao> BuscarPedidoCTeParaSubContratacao(int codigoCarga)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoCTeParaSubContratacao>();
            var result = from obj in query where obj.CargaPedido.Carga.Codigo == codigoCarga select obj;

            return result.Select(o => new Dominio.ObjetosDeValor.Embarcador.Carga.CargaPedidoCteSubContratacao()
            {
                CargaPedido = o.CargaPedido.Codigo,
                PedidoCteParaSubContratacao = o.Codigo
            }).ToList();
        }

        public List<Dominio.ObjetosDeValor.Embarcador.Carga.CargaPedido> BuscarTotalSumarizadoPorCarga(int codigoCarga)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoCTeParaSubContratacao>();
            var result = from obj in query where obj.CargaPedido.Carga.Codigo == codigoCarga select obj;
            result = result.Where(obj => obj.CTeTerceiro.Ativo == true);

            return result.GroupBy(o => new { o.CargaPedido.Codigo }).Select(obj => new Dominio.ObjetosDeValor.Embarcador.Carga.CargaPedido()
            {
                Codigo = obj.Key.Codigo,
                ValorTotalNotaFiscal = obj.Sum(c => c.CTeTerceiro.ValorTotalMercadoria)
            }).ToList();
        }

        public decimal BuscarTotalPorCargaPedido(int codigoCargaPedido)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoCTeParaSubContratacao>();
            var result = from obj in query where obj.CargaPedido.Codigo == codigoCargaPedido select obj;
            result = result.Where(obj => obj.CTeTerceiro.Ativo == true);
            return result.Sum(obj => (decimal?)obj.CTeTerceiro.ValorTotalMercadoria) ?? 0m;
        }
        public async Task<decimal> BuscarTotalPorCargaPedidoAsync(int codigoCargaPedido)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoCTeParaSubContratacao>();
            var result = from obj in query where obj.CargaPedido.Codigo == codigoCargaPedido select obj;
            result = result.Where(obj => obj.CTeTerceiro.Ativo == true);
            return await result.SumAsync(obj => (decimal?)obj.CTeTerceiro.ValorTotalMercadoria) ?? 0m;
        }

        public List<Dominio.Entidades.Embarcador.CTe.CTeTerceiro> BuscarCTesPorCargaPedido(int codigoCargaPedido, bool ativa, string propOrdenacao, string dirOrdenacao, int inicioRegistros, int maximoRegistros)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoCTeParaSubContratacao>();
            var result = from obj in query where obj.CargaPedido.Codigo == codigoCargaPedido select obj;
            result = result.Where(obj => obj.CTeTerceiro.Ativo == ativa);

            return result.Select(obj => obj.CTeTerceiro)
                .Fetch(o => o.TransportadorTerceiro).ThenFetch(o => o.Localidade)
                .Fetch(o => o.Remetente).ThenFetch(o => o.Localidade)
                .Fetch(o => o.Destinatario).ThenFetch(o => o.Localidade)
                .OrderBy(propOrdenacao + (dirOrdenacao == "asc" ? " ascending" : " descending")).Skip(inicioRegistros).Take(maximoRegistros).ToList();
        }

        public List<Dominio.Entidades.Embarcador.CTe.CTeTerceiro> BuscarCTesPorCargaPedidoPacote(int codigoCargaPedido, bool ativa, string propOrdenacao, string dirOrdenacao, int inicioRegistros, int maximoRegistros)
        {
            var queryCTeTerceiro = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.CTe.CTeTerceiro>();
            var queryPacote = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.Pacote>();
            var queryCargaPedidoPacote = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedidoPacote>();

            var result = from cteTerceiro in queryCTeTerceiro
                         join pct in queryPacote on cteTerceiro.IdentifacaoPacote equals pct.LogKey
                         join cppct in queryCargaPedidoPacote on pct.Codigo equals cppct.Pacote.Codigo
                         where cppct.CargaPedido.Codigo == codigoCargaPedido
                         select cteTerceiro;
            result = result.Where(obj => obj.Ativo == ativa);

            return result.Select(obj => obj)
                .Fetch(o => o.TransportadorTerceiro).ThenFetch(o => o.Localidade)
                .Fetch(o => o.Remetente).ThenFetch(o => o.Localidade)
                .Fetch(o => o.Destinatario).ThenFetch(o => o.Localidade)
                .OrderBy(propOrdenacao + (dirOrdenacao == "asc" ? " ascending" : " descending")).Skip(inicioRegistros).Take(maximoRegistros).ToList();
        }

        public int ContarCTesPorCargaPedido(int codigoCargaPedido, bool ativa)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoCTeParaSubContratacao>();
            var result = from obj in query where obj.CargaPedido.Codigo == codigoCargaPedido select obj;
            result = result.Where(obj => obj.CTeTerceiro.Ativo == ativa);
            return result.Select(obj => obj.CTeTerceiro).Count();
        }

        public int ContarPorCargaPedidoPacoteComCodigoCargaPedido(int codigoCargaPedido)
        {
            string sql = $@"SELECT COUNT(CTeTerceiro.CPS_CODIGO)
                            FROM T_CTE_TERCEIRO CTeTerceiro
                            LEFT JOIN T_PACOTE Pacote ON Pacote.PCT_LOG_KEY = CTeTerceiro.CPS_IDENTIFICACAO_PACOTE
                            LEFT JOIN T_CARGA_PEDIDO_PACOTE CargaPedidoPacote ON Pacote.PCT_CODIGO = CargaPedidoPacote.PCT_CODIGO
                            WHERE CargaPedidoPacote.CPE_CODIGO = {codigoCargaPedido}";

            var query = this.SessionNHiBernate.CreateSQLQuery(sql);
            var result = query.UniqueResult();

            return Convert.ToInt32(result);
        }

        public bool VerificarSeExistePorCargaPedido(int codigoCargaPedido)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoCTeParaSubContratacao>();

            query = query.Where(o => o.CargaPedido.Codigo == codigoCargaPedido && o.CTeTerceiro.Ativo);

            return query.Any();
        }

        public bool VerificarSeExistePorCarga(int codigoCarga)
        {
            IQueryable<Dominio.Entidades.Embarcador.Pedidos.PedidoCTeParaSubContratacao> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoCTeParaSubContratacao>();

            query = query.Where(o => o.CargaPedido.Carga.Codigo == codigoCarga);

            return query.Select(o => o.Codigo).Any();
        }

        public bool VerificarSePedidosNaoPossuemCTeAnteriorPorCarga(int carga)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedido>();

            query = query.Where(o => o.Carga.Codigo == carga && !o.PedidoCTesParaSubContratacao.Any(obj => obj.CTeTerceiro.Ativo));

            return query.Any();
        }

        public Dominio.Entidades.Embarcador.Pedidos.PedidoCTeParaSubContratacao BuscarPorCTeSubContratacao(int codigoCTeTerceiro)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoCTeParaSubContratacao>();

            query = query.Where(o => o.CTeTerceiro.Codigo == codigoCTeTerceiro);

            return query.FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.Pedidos.PedidoCTeParaSubContratacao BuscarPorCTeSubContratacaoAtivo(int codigoCTeTerceiro)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoCTeParaSubContratacao>();

            query = query.Where(o => o.CTeTerceiro.Codigo == codigoCTeTerceiro && o.CargaPedido.Carga.SituacaoCarga != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.Anulada && o.CargaPedido.Carga.SituacaoCarga != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.Cancelada);

            return query.FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.Pedidos.PedidoCTeParaSubContratacao BuscarPorCTeSubContratacaoAtivo(int codigoCTeTerceiro, int codigoPortoOrigem)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoCTeParaSubContratacao>();

            query = query.Where(o => o.CTeTerceiro.Codigo == codigoCTeTerceiro && o.CargaPedido.Carga.SituacaoCarga != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.Anulada && o.CargaPedido.Carga.SituacaoCarga != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.Cancelada && o.CargaPedido.Pedido.Porto != null && o.CargaPedido.Pedido.Porto.Codigo == codigoPortoOrigem);

            return query.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Pedidos.PedidoCTeParaSubContratacao> BuscarTodosPorCTeSubContratacao(int codigoCTeTerceiro)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoCTeParaSubContratacao>();

            query = query.Where(o => o.CTeTerceiro.Codigo == codigoCTeTerceiro);

            return query.ToList();
        }

        public List<Dominio.ObjetosDeValor.Embarcador.Frete.ParticipantesCalculoFrete> BuscarParticipantesPorCargaPedido(Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido)
        {
            IQueryable<Dominio.Entidades.Embarcador.Pedidos.PedidoCTeParaSubContratacao> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoCTeParaSubContratacao>();

            query = query.Where(o => o.CargaPedido.Codigo == cargaPedido.Codigo && o.CTeTerceiro.Ativo);

            if (cargaPedido.TipoEmissaoCTeParticipantes == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEmissaoCTeParticipantes.ComExpedidor)
            {
                return query.Select(o => new Dominio.ObjetosDeValor.Embarcador.Frete.ParticipantesCalculoFrete()
                {
                    Tomador = o.CTeTerceiro.Emitente.Cliente.CPF_CNPJ,
                    Remetente = o.CTeTerceiro.Remetente.Cliente.CPF_CNPJ,
                    Expedidor = (double?)o.CTeTerceiro.Expedidor.Cliente.CPF_CNPJ ?? o.CargaPedido.Expedidor.CPF_CNPJ,
                    Recebedor = o.CTeTerceiro.Recebedor.Cliente.CPF_CNPJ,
                    Destinatario = o.CTeTerceiro.Destinatario.Cliente.CPF_CNPJ
                }).Distinct().ToList();
            }

            if (cargaPedido.TipoEmissaoCTeParticipantes == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEmissaoCTeParticipantes.ComRecebedor)
            {
                return query.Select(o => new Dominio.ObjetosDeValor.Embarcador.Frete.ParticipantesCalculoFrete()
                {
                    Tomador = o.CTeTerceiro.Emitente.Cliente.CPF_CNPJ,
                    Remetente = o.CTeTerceiro.Remetente.Cliente.CPF_CNPJ,
                    Expedidor = o.CTeTerceiro.Expedidor.Cliente.CPF_CNPJ,
                    Recebedor = (double?)o.CTeTerceiro.Recebedor.Cliente.CPF_CNPJ ?? o.CargaPedido.Recebedor.CPF_CNPJ,
                    Destinatario = o.CTeTerceiro.Destinatario.Cliente.CPF_CNPJ
                }).Distinct().ToList();
            }

            if (cargaPedido.TipoEmissaoCTeParticipantes == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEmissaoCTeParticipantes.ComExpedidorERecebedor)
            {
                return query.Select(o => new Dominio.ObjetosDeValor.Embarcador.Frete.ParticipantesCalculoFrete()
                {
                    Tomador = o.CTeTerceiro.Emitente.Cliente.CPF_CNPJ,
                    Remetente = (double?)o.CTeTerceiro.Expedidor.Cliente.CPF_CNPJ ?? o.CargaPedido.Expedidor.CPF_CNPJ,
                    Expedidor = o.CTeTerceiro.Expedidor.Cliente.CPF_CNPJ,
                    Recebedor = (double?)o.CTeTerceiro.Recebedor.Cliente.CPF_CNPJ ?? o.CargaPedido.Recebedor.CPF_CNPJ,
                    Destinatario = o.CTeTerceiro.Destinatario.Cliente.CPF_CNPJ
                }).Distinct().ToList();
            }

            return query.Select(o => new Dominio.ObjetosDeValor.Embarcador.Frete.ParticipantesCalculoFrete()
            {
                Tomador = o.CTeTerceiro.Emitente.Cliente.CPF_CNPJ,
                Remetente = o.CTeTerceiro.Remetente.Cliente.CPF_CNPJ,
                Expedidor = o.CTeTerceiro.Expedidor.Cliente.CPF_CNPJ,
                Recebedor = o.CTeTerceiro.Recebedor.Cliente.CPF_CNPJ,
                Destinatario = o.CTeTerceiro.Destinatario.Cliente.CPF_CNPJ
            }).Distinct().ToList();
        }

        public List<Dominio.Entidades.Embarcador.Pedidos.PedidoCTeParaSubContratacao> BuscarPorCargaPedidoEParticipantes(int codigoCargaPedido, double emitente, double remetente, double? expedidor, double? recebedor, double destinatario, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEmissaoCTeParticipantes tipoEmissaoCTeParticipantes)
        {
            IQueryable<Dominio.Entidades.Embarcador.Pedidos.PedidoCTeParaSubContratacao> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoCTeParaSubContratacao>();

            query = query.Where(o => o.CargaPedido.Codigo == codigoCargaPedido && o.CTeTerceiro.Ativo && o.CTeTerceiro.Emitente.Cliente.CPF_CNPJ == emitente && o.CTeTerceiro.Remetente.Cliente.CPF_CNPJ == remetente && o.CTeTerceiro.Destinatario.Cliente.CPF_CNPJ == destinatario);

            if (tipoEmissaoCTeParticipantes == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEmissaoCTeParticipantes.ComExpedidor || tipoEmissaoCTeParticipantes == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEmissaoCTeParticipantes.ComExpedidorERecebedor)
            {
                if (expedidor > 0D)
                    query = query.Where(o => o.CTeTerceiro.Expedidor == null || o.CTeTerceiro.Expedidor.Cliente.CPF_CNPJ == expedidor);
                else
                    query = query.Where(o => o.CTeTerceiro.Expedidor == null);
            }

            if (tipoEmissaoCTeParticipantes == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEmissaoCTeParticipantes.ComRecebedor || tipoEmissaoCTeParticipantes == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEmissaoCTeParticipantes.ComExpedidorERecebedor)
            {
                if (recebedor > 0D)
                    query = query.Where(o => o.CTeTerceiro.Recebedor == null || o.CTeTerceiro.Recebedor.Cliente.CPF_CNPJ == recebedor);
                else
                    query = query.Where(o => o.CTeTerceiro.Recebedor == null);
            }

            return query.ToList();
        }

        public int VerificarPorCargaSePossuiValorZerado(int carga)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoCTeParaSubContratacao>();
            var result = from obj in query where obj.CargaPedido.Carga.Codigo == carga && obj.ValorFrete <= 0 && obj.ValorTotalComponentes <= 0 select obj;
            return result.Count();
        }

        public decimal BuscarValorTotalMercadoriaPorCarga(int codigoCarga)
        {
            IQueryable<Dominio.Entidades.Embarcador.Pedidos.PedidoCTeParaSubContratacao> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoCTeParaSubContratacao>();

            query = query.Where(o => o.CargaPedido.Carga.Codigo == codigoCarga && o.CTeTerceiro.Ativo);

            return query.Sum(o => (decimal?)o.CTeTerceiro.ValorTotalMercadoria) ?? 0m;
        }
        public async Task<decimal> BuscarValorTotalMercadoriaPorCargaAsync(int codigoCarga)
        {
            IQueryable<Dominio.Entidades.Embarcador.Pedidos.PedidoCTeParaSubContratacao> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoCTeParaSubContratacao>();

            query = query.Where(o => o.CargaPedido.Carga.Codigo == codigoCarga && o.CTeTerceiro.Ativo);

            return await query.SumAsync(o => (decimal?)o.CTeTerceiro.ValorTotalMercadoria) ?? 0m;
        }

        public decimal BuscarValorTotalMercadoriaPorCargaPedido(int codigo)
        {
            IQueryable<Dominio.Entidades.Embarcador.Pedidos.PedidoCTeParaSubContratacao> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoCTeParaSubContratacao>();

            query = query.Where(o => o.CargaPedido.Codigo == codigo && o.CTeTerceiro.Ativo);

            return query.Sum(o => (decimal?)o.CTeTerceiro.ValorTotalMercadoria) ?? 0m;
        }

        public List<int> BuscarCodigosCTesParaSubcontratacaoSemCanhotoGerado(bool gerarCanhotoSempre)
        {
            Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga[] situacoes = new Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga[]
            {
                  Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.EmTransporte,
                  Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.Encerrada
            };

            IQueryable<Dominio.Entidades.Embarcador.Pedidos.PedidoCTeParaSubContratacao> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoCTeParaSubContratacao>();
            IQueryable<Dominio.Entidades.Embarcador.Canhotos.Canhoto> queryCanhotos = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Canhotos.Canhoto>();

            query = query.Where(o => o.CTeTerceiro.Ativo &&
                                     situacoes.Contains(o.CargaPedido.Carga.SituacaoCarga) && !o.CargaPedido.Carga.CargaTransbordo && o.CargaPedido.Carga.TipoFreteEscolhido != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoFreteEscolhido.Cliente &&
                                     !queryCanhotos.Any(c => c.CTeSubcontratacao == o.CTeTerceiro) &&
                                     (gerarCanhotoSempre ||
                                      (o.CargaPedido.TipoTomador == Dominio.Enumeradores.TipoTomador.Remetente && (o.CargaPedido.Pedido.Remetente.ExigeCanhotoFisico.Value || o.CargaPedido.Pedido.Remetente.GrupoPessoas.ExigeCanhotoFisico.Value)) ||
                                      (o.CargaPedido.TipoTomador == Dominio.Enumeradores.TipoTomador.Destinatario && (o.CargaPedido.Pedido.Destinatario.ExigeCanhotoFisico.Value || o.CargaPedido.Pedido.Destinatario.GrupoPessoas.ExigeCanhotoFisico.Value)) ||
                                      (o.CargaPedido.TipoTomador == Dominio.Enumeradores.TipoTomador.Expedidor && (o.CargaPedido.Expedidor.ExigeCanhotoFisico.Value || o.CargaPedido.Expedidor.GrupoPessoas.ExigeCanhotoFisico.Value)) ||
                                      (o.CargaPedido.TipoTomador == Dominio.Enumeradores.TipoTomador.Recebedor && (o.CargaPedido.Recebedor.ExigeCanhotoFisico.Value || o.CargaPedido.Recebedor.GrupoPessoas.ExigeCanhotoFisico.Value)) ||
                                      (o.CargaPedido.TipoTomador == Dominio.Enumeradores.TipoTomador.Outros && (o.CargaPedido.Tomador.ExigeCanhotoFisico.Value || o.CargaPedido.Tomador.GrupoPessoas.ExigeCanhotoFisico.Value))));

            return query.Select(o => o.Codigo).WithOptions(o => { o.SetTimeout(6000); }).ToList();
        }

        public Dominio.Entidades.Embarcador.Pedidos.PedidoCTeParaSubContratacao BuscarPorChaveComCargaEmitida(string chave)
        {
            List<Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga> situacoesCargas = new List<Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga>() {
                Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.Encerrada,
                Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.EmTransporte,
                Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.AgIntegracao
            };

            IQueryable<Dominio.Entidades.Embarcador.Pedidos.PedidoCTeParaSubContratacao> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoCTeParaSubContratacao>();

            query = query.Where(o => o.CTeTerceiro.ChaveAcesso == chave && situacoesCargas.Contains(o.CargaPedido.Carga.SituacaoCarga));

            return query.FirstOrDefault();
        }
    }
}
