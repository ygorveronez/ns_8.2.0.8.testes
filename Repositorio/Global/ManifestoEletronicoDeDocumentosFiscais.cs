using Dominio.Enumeradores;
using NHibernate.Criterion;
using NHibernate.Linq;
using Repositorio.Embarcador.Consulta;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading;
using System.Threading.Tasks;

namespace Repositorio
{
    public class ManifestoEletronicoDeDocumentosFiscais : RepositorioBase<Dominio.Entidades.ManifestoEletronicoDeDocumentosFiscais>, Dominio.Interfaces.Repositorios.ManifestoEletronicoDeDocumentosFiscais
    {
        public ManifestoEletronicoDeDocumentosFiscais(UnitOfWork unitOfWork) : base(unitOfWork) { }
        public ManifestoEletronicoDeDocumentosFiscais(UnitOfWork unitOfWork, CancellationToken cancellationToken) : base(unitOfWork, cancellationToken) { }

        #region Métodos Públicos

        public IList<Dominio.ObjetosDeValor.SeguroMDFeIntegracao> BuscarSegurosParaMDFe(int codigoCarga)
        {
            var sqlQuery = @"select DISTINCT REPLACE(STR(s.CLI_CGCCPF_SEGURADORA, 14, 0), ' ', '0') CNPJSeguradora,
                                 s.SEA_NOME NomeSeguradora,
                                 ap.APS_NUMERO_APOLICE NumeroApolice,
                                 case when ca.AVE_AVERBACAO IS NULL OR CA.AVE_AVERBACAO = '' THEN '99999' ELSE ca.AVE_AVERBACAO END NumeroAverbacao,
                                 ap.APS_RESPONSAVEL Responsavel,
                                 (case ap.APS_RESPONSAVEL 
                                 when 1 then emp.EMP_CNPJ
                                 else pct.PCT_CPF_CNPJ end) CNPJCPFResponsavel
                             from t_cte_Averbacao ca
                             join T_CARGA_PEDIDO_APOLICE_SEGURO_AVERBACAO cp on ca.cpa_codigo = cp.cpa_codigo
                             join T_APOLICE_SEGURO_GERAL ap on cp.APS_CODIGO = ap.APS_CODIGO
                             join T_SEGURADORA s on ap.SEA_CODIGO = s.SEA_CODIGO
                             join T_CTE CTe on CTe.CON_CODIGO = ca.CON_CODIGO
                             join T_CTE_PARTICIPANTE pct on pct.PCT_CODIGO = CTe.CON_TOMADOR_PAGADOR_CTE
                             join T_EMPRESA emp on emp.EMP_CODIGO = CTe.EMP_CODIGO
                             where CAR_CODIGO = " + codigoCarga.ToString();

            var query = this.SessionNHiBernate.CreateSQLQuery(sqlQuery);

            query.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Dominio.ObjetosDeValor.SeguroMDFeIntegracao)));

            return query.List<Dominio.ObjetosDeValor.SeguroMDFeIntegracao>();
        }

        public IList<Dominio.ObjetosDeValor.SeguroMDFeIntegracao> BuscarSegurosPedidosParaMDFe(int codigoCarga)
        {
            var sqlQuery = @"select DISTINCT REPLACE(STR(s.CLI_CGCCPF_SEGURADORA, 14, 0), ' ', '0') CNPJSeguradora,
                                 s.SEA_NOME NomeSeguradora,
                                 ap.APS_NUMERO_APOLICE NumeroApolice,
                                 '99999' NumeroAverbacao,
                                 ap.APS_RESPONSAVEL Responsavel,
                                 (case ap.APS_RESPONSAVEL 
                                 when 1 then emp.EMP_CNPJ
                                 else REPLACE(STR(ped.CLI_CODIGO_REMETENTE, 14, 0), ' ', '0') end) CNPJCPFResponsavel
                             from T_CARGA_PEDIDO_APOLICE_SEGURO_AVERBACAO CP
                             join T_APOLICE_SEGURO_GERAL ap on cp.APS_CODIGO = ap.APS_CODIGO
                             join T_SEGURADORA s on ap.SEA_CODIGO = s.SEA_CODIGO
							 join T_CARGA_PEDIDO p on p.CPE_CODIGO = cp.CPE_CODIGO
							 join T_PEDIDO ped on p.PED_CODIGO = ped.PED_CODIGO
							 join T_CARGA ca on ca.CAR_CODIGO = p.CAR_CODIGO
							 join T_EMPRESA emp ON emp.EMP_CODIGO = ca.EMP_CODIGO
                             where ca.CAR_CODIGO = " + codigoCarga.ToString();

            var query = this.SessionNHiBernate.CreateSQLQuery(sqlQuery);

            query.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Dominio.ObjetosDeValor.SeguroMDFeIntegracao)));

            return query.List<Dominio.ObjetosDeValor.SeguroMDFeIntegracao>();
        }

        public List<string> BuscarCPFCNPJTomadoresCTes(int codigoMDFe)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.DocumentoMunicipioDescarregamentoMDFe>();

            query = query.Where(o => o.MunicipioDescarregamento.MDFe.Codigo == codigoMDFe && !o.CTe.TomadorPagador.Exterior);

            return query.Select(o => o.CTe.TomadorPagador.CPF_CNPJ).Distinct().ToList();
        }

        public List<Dominio.Entidades.ParticipanteCTe> BuscarTomadoresCTes(int codigoMDFe)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.DocumentoMunicipioDescarregamentoMDFe>();

            query = query.Where(o => o.MunicipioDescarregamento.MDFe.Codigo == codigoMDFe && !o.CTe.TomadorPagador.Exterior);

            return query.Select(o => o.CTe.TomadorPagador).ToList();
        }

        public bool ExisteTomadorExterior(int codigoMDFe)
        {
            IQueryable<Dominio.Entidades.DocumentoMunicipioDescarregamentoMDFe> query = this.SessionNHiBernate.Query<Dominio.Entidades.DocumentoMunicipioDescarregamentoMDFe>();

            query = query.Where(o => o.MunicipioDescarregamento.MDFe.Codigo == codigoMDFe && o.CTe.TomadorPagador.Exterior);

            return query.Select(o => o.Codigo).Any();
        }

        public Dominio.Entidades.ManifestoEletronicoDeDocumentosFiscais BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.ManifestoEletronicoDeDocumentosFiscais>();
            var result = from obj in query where obj.Codigo == codigo select obj;
            return result.FirstOrDefault();
        }

        public Dominio.Entidades.ManifestoEletronicoDeDocumentosFiscais BuscarPorCodigoComFetchEmpresa(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.ManifestoEletronicoDeDocumentosFiscais>();
            var result = from obj in query where obj.Codigo == codigo select obj;
            return result.Fetch(o => o.Empresa).FirstOrDefault();
        }

        public Dominio.Entidades.ManifestoEletronicoDeDocumentosFiscais BuscarPorCodigoComFetch(int codigo)
        {
            IQueryable<Dominio.Entidades.ManifestoEletronicoDeDocumentosFiscais> query = SessionNHiBernate.Query<Dominio.Entidades.ManifestoEletronicoDeDocumentosFiscais>();

            query = query.Where(obj => obj.Codigo == codigo);

            return query.Fetch(o => o.Empresa).ThenFetch(o => o.Localidade).ThenFetch(o => o.Estado).FirstOrDefault();
        }

        public bool ContemMDFePorChave(string chaveMDFe)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.ManifestoEletronicoDeDocumentosFiscais>();
            var result = from obj in query where obj.Chave.Equals(chaveMDFe) select obj;
            return result.Any();
        }

        public Dominio.Entidades.ManifestoEletronicoDeDocumentosFiscais BuscarPorChave(string chaveMDFe)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.ManifestoEletronicoDeDocumentosFiscais>();
            var result = from obj in query where obj.Chave.Equals(chaveMDFe) select obj;
            return result.Fetch(o => o.Empresa).WithOptions(o => o.SetTimeout(120)).FirstOrDefault();
        }

        public Task<Dominio.Entidades.ManifestoEletronicoDeDocumentosFiscais> BuscarPorChaveAsync(string chaveMDFe)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.ManifestoEletronicoDeDocumentosFiscais>();
            var result = from obj in query where obj.Chave.Equals(chaveMDFe) select obj;

            return result.Fetch(o => o.Empresa).WithOptions(o => o.SetTimeout(120)).FirstOrDefaultAsync(CancellationToken);
        }

        public Dominio.Entidades.ManifestoEletronicoDeDocumentosFiscais BuscarPorCodigoIntegradorAutorizacao(int codigoIntegradorAutorizacao)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.ManifestoEletronicoDeDocumentosFiscais>();
            var result = from obj in query where obj.CodigoIntegradorAutorizacao == codigoIntegradorAutorizacao select obj;
            return result.FirstOrDefault();
        }

        public Dominio.Entidades.ManifestoEletronicoDeDocumentosFiscais BuscarPorCodigoIntegradorCancelamento(int codigoIntegradorCancelamento)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.ManifestoEletronicoDeDocumentosFiscais>();
            var result = from obj in query where obj.CodigoIntegradorCancelamento == codigoIntegradorCancelamento select obj;
            return result.FirstOrDefault();
        }

        public Dominio.Entidades.ManifestoEletronicoDeDocumentosFiscais BuscarPorCodigoIntegradorEncerramento(int codigoIntegradorEncerramento)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.ManifestoEletronicoDeDocumentosFiscais>();
            var result = from obj in query where obj.CodigoIntegradorEncerramento == codigoIntegradorEncerramento select obj;
            return result.FirstOrDefault();
        }

        public Dominio.Entidades.ManifestoEletronicoDeDocumentosFiscais BuscarPorCodigo(int codigo, int codigoEmpresa, Dominio.Enumeradores.StatusMDFe? status = null)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.ManifestoEletronicoDeDocumentosFiscais>();
            var result = from obj in query where obj.Codigo == codigo select obj;

            if (codigoEmpresa > 0)
                result = result.Where(o => o.Empresa.Codigo == codigoEmpresa);

            if (status != null)
                result = result.Where(o => o.Status == status);

            return result.FirstOrDefault();
        }

        public Task<Dominio.Entidades.ManifestoEletronicoDeDocumentosFiscais> BuscarPorCodigoAsync(int codigo, int codigoEmpresa, Dominio.Enumeradores.StatusMDFe? status = null)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.ManifestoEletronicoDeDocumentosFiscais>();
            var result = from obj in query where obj.Codigo == codigo select obj;

            if (codigoEmpresa > 0)
                result = result.Where(o => o.Empresa.Codigo == codigoEmpresa);

            if (status != null)
                result = result.Where(o => o.Status == status);

            return result.FirstOrDefaultAsync(CancellationToken);
        }

        public Dominio.Entidades.ManifestoEletronicoDeDocumentosFiscais BuscarPorPlacaEStatus(string cnpjEmpresa, string placaVeiculo, Dominio.Enumeradores.StatusMDFe status)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.ManifestoEletronicoDeDocumentosFiscais>();

            query = query.Where(o => o.Status == status && o.Empresa.CNPJ.Contains(cnpjEmpresa.Substring(0, 8)));

            if (!string.IsNullOrWhiteSpace(placaVeiculo))
                query = query.Where(obj => obj.Veiculos.Any(o => o.Placa == placaVeiculo) || obj.Reboques.Any(o => o.Placa == placaVeiculo));

            return query.FirstOrDefault();
        }

        public List<Dominio.Entidades.ManifestoEletronicoDeDocumentosFiscais> ConsultarMDFesSemCarga(int numero, string placaVeiculo, string ufCarregamento, string ufDescarregamento, string propOrdenacao, string dirOrdena, int inicio, int limite)
        {
            List<Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga> situacoesCargaNaoPermitidas = new List<Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga>()
            {
                 Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.Cancelada,
                 Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.Anulada
            };

            IQueryable<Dominio.Entidades.ManifestoEletronicoDeDocumentosFiscais> query = this.SessionNHiBernate.Query<Dominio.Entidades.ManifestoEletronicoDeDocumentosFiscais>();

            IQueryable<Dominio.Entidades.Embarcador.Cargas.CargaMDFe> queryCargaMDFe = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaMDFe>();
            IQueryable<Dominio.Entidades.Embarcador.Cargas.CargaPedidoDocumentoMDFe> queryCargaPedidoDocumentoMDFe = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedidoDocumentoMDFe>();

            queryCargaPedidoDocumentoMDFe = queryCargaPedidoDocumentoMDFe.Where(o => !situacoesCargaNaoPermitidas.Contains(o.CargaPedido.Carga.SituacaoCarga));
            //queryCargaMDFe = queryCargaMDFe.Where(o => !situacoesCargaNaoPermitidas.Contains(o.Carga.SituacaoCarga));

            query = query.Where(o => !queryCargaMDFe.Any(cmd => cmd.MDFe == o) &&
                                     !queryCargaPedidoDocumentoMDFe.Any(cmd => cmd.MDFe == o) &&
                                     (o.Status == StatusMDFe.Autorizado || o.Status == StatusMDFe.Encerrado));

            if (numero > 0)
                query = query.Where(o => o.Numero == numero);

            if (!string.IsNullOrWhiteSpace(ufCarregamento))
                query = query.Where(o => o.EstadoCarregamento.Sigla == ufCarregamento);

            if (!string.IsNullOrWhiteSpace(ufDescarregamento))
                query = query.Where(o => o.EstadoDescarregamento.Sigla == ufDescarregamento);

            if (!string.IsNullOrWhiteSpace(placaVeiculo))
                query = query.Where(obj => obj.Veiculos.Any(o => o.Placa == placaVeiculo) || obj.Reboques.Any(o => o.Placa == placaVeiculo));

            return query.Fetch(o => o.EstadoCarregamento)
                        .Fetch(o => o.EstadoDescarregamento)
                        .OrderBy(propOrdenacao + " " + dirOrdena)
                        .Skip(inicio)
                        .Take(limite)
                        .ToList();
        }

        public List<Dominio.Entidades.ManifestoEletronicoDeDocumentosFiscais> ConsultarMDFesSemCargaCompativeis(string placaVeiculo, int codigoOrigem, int codigoDestino)
        {
            List<Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga> situacoesCargaNaoPermitidas = new List<Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga>()
            {
                 Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.Cancelada,
                 Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.Anulada
            };

            IQueryable<Dominio.Entidades.ManifestoEletronicoDeDocumentosFiscais> query = this.SessionNHiBernate.Query<Dominio.Entidades.ManifestoEletronicoDeDocumentosFiscais>();

            IQueryable<Dominio.Entidades.Embarcador.Cargas.CargaMDFe> queryCargaMDFe = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaMDFe>();
            IQueryable<Dominio.Entidades.Embarcador.Cargas.CargaPedidoDocumentoMDFe> queryCargaPedidoDocumentoMDFe = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedidoDocumentoMDFe>();

            IQueryable<Dominio.Entidades.Embarcador.Cargas.CargaCTe> queryCargaCTe = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCTe>();
            IQueryable<Dominio.Entidades.Embarcador.Cargas.CargaPedidoDocumentoCTe> queryCargaPedidoDocumentoCTe = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedidoDocumentoCTe>();

            queryCargaPedidoDocumentoMDFe = queryCargaPedidoDocumentoMDFe.Where(o => !situacoesCargaNaoPermitidas.Contains(o.CargaPedido.Carga.SituacaoCarga));
            queryCargaPedidoDocumentoCTe = queryCargaPedidoDocumentoCTe.Where(o => !situacoesCargaNaoPermitidas.Contains(o.CargaPedido.Carga.SituacaoCarga));

            //queryCargaMDFe = queryCargaMDFe.Where(o => !situacoesCargaNaoPermitidas.Contains(o.Carga.SituacaoCarga));
            queryCargaCTe = queryCargaCTe.Where(o => !situacoesCargaNaoPermitidas.Contains(o.Carga.SituacaoCarga));

            query = query.Where(o => !o.MunicipiosDescarregamento.Any(mun => mun.Documentos.Any(doc => queryCargaCTe.Any(cct => cct.CTe == doc.CTe) || queryCargaPedidoDocumentoCTe.Any(cpd => cpd.CTe == doc.CTe))) &&
                                     !queryCargaMDFe.Any(cmd => cmd.MDFe == o) &&
                                     !queryCargaPedidoDocumentoMDFe.Any(cmd => cmd.MDFe == o) &&
                                     (o.Status == StatusMDFe.Autorizado || o.Status == StatusMDFe.Encerrado));

            if (!string.IsNullOrWhiteSpace(placaVeiculo))
                query = query.Where(obj => obj.Veiculos.Any(o => o.Placa == placaVeiculo));

            if (codigoOrigem > 0)
                query = query.Where(o => o.MunicipiosCarregamento.Any(m => m.Municipio.Codigo == codigoOrigem));

            if (codigoDestino > 0)
                query = query.Where(obj => obj.MunicipiosDescarregamento.Any(m => m.Municipio.Codigo == codigoDestino));

            return query.WithOptions(o => o.SetTimeout(90)).ToList();
        }

        public int ContarConsultaMDFesSemCarga(int numero, string placaVeiculo, string ufCarregamento, string ufDescarregamento)
        {
            List<Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga> situacoesCargaNaoPermitidas = new List<Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga>()
            {
                 Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.Cancelada,
                 Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.Anulada
            };

            IQueryable<Dominio.Entidades.ManifestoEletronicoDeDocumentosFiscais> query = this.SessionNHiBernate.Query<Dominio.Entidades.ManifestoEletronicoDeDocumentosFiscais>();

            IQueryable<Dominio.Entidades.Embarcador.Cargas.CargaMDFe> queryCargaMDFe = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaMDFe>();
            IQueryable<Dominio.Entidades.Embarcador.Cargas.CargaPedidoDocumentoMDFe> queryCargaPedidoDocumentoMDFe = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedidoDocumentoMDFe>();

            queryCargaPedidoDocumentoMDFe = queryCargaPedidoDocumentoMDFe.Where(o => !situacoesCargaNaoPermitidas.Contains(o.CargaPedido.Carga.SituacaoCarga));
            //queryCargaMDFe = queryCargaMDFe.Where(o => !situacoesCargaNaoPermitidas.Contains(o.Carga.SituacaoCarga));

            query = query.Where(o => !queryCargaMDFe.Any(cmd => cmd.MDFe == o) &&
                                     !queryCargaPedidoDocumentoMDFe.Any(cmd => cmd.MDFe == o) &&
                                     (o.Status == StatusMDFe.Autorizado || o.Status == StatusMDFe.Encerrado));

            if (numero > 0)
                query = query.Where(o => o.Numero == numero);

            if (!string.IsNullOrWhiteSpace(ufCarregamento))
                query = query.Where(o => o.EstadoCarregamento.Sigla == ufCarregamento);

            if (!string.IsNullOrWhiteSpace(ufDescarregamento))
                query = query.Where(o => o.EstadoDescarregamento.Sigla == ufDescarregamento);

            if (!string.IsNullOrWhiteSpace(placaVeiculo))
                query = query.Where(obj => obj.Veiculos.Any(o => o.Placa == placaVeiculo) || obj.Reboques.Any(o => o.Placa == placaVeiculo));

            return query.Count();
        }

        public List<Dominio.Entidades.ManifestoEletronicoDeDocumentosFiscais> ConsultarMDFes(int codigoCarga, int numero, string placaVeiculo, string ufCarregamento, string ufDescarregamento, Dominio.Enumeradores.StatusMDFe? statusMDFe, string propOrdenacao, string dirOrdena, int inicio, int limite)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.ManifestoEletronicoDeDocumentosFiscais>();

            if (statusMDFe.HasValue)
                query = query.Where(o => o.Status == statusMDFe);

            if (numero > 0)
                query = query.Where(o => o.Numero == numero);

            if (!string.IsNullOrWhiteSpace(ufCarregamento))
                query = query.Where(o => o.EstadoCarregamento.Sigla == ufCarregamento);

            if (!string.IsNullOrWhiteSpace(ufDescarregamento))
                query = query.Where(o => o.EstadoDescarregamento.Sigla == ufDescarregamento);

            if (!string.IsNullOrWhiteSpace(placaVeiculo))
                query = query.Where(obj => obj.Veiculos.Any(o => o.Placa == placaVeiculo) || obj.Reboques.Any(o => o.Placa == placaVeiculo));

            if (codigoCarga > 0)
            {
                var queryCarga = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaMDFe>();
                queryCarga = queryCarga.Where(o => o.Carga.Codigo == codigoCarga);
                query = query.Where(obj => queryCarga.Select(o => o.MDFe).Contains(obj));
            }

            return query.Fetch(o => o.EstadoCarregamento)
                        .Fetch(o => o.EstadoDescarregamento)
                        .OrderBy(propOrdenacao + " " + dirOrdena)
                        .Skip(inicio)
                        .Take(limite)
                        .ToList();
        }

        public int ContarConsultaMDFes(int codigoCarga, int numero, string placaVeiculo, string ufCarregamento, string ufDescarregamento, Dominio.Enumeradores.StatusMDFe? statusMDFe)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.ManifestoEletronicoDeDocumentosFiscais>();

            if (statusMDFe.HasValue)
                query = query.Where(o => o.Status == statusMDFe);

            if (numero > 0)
                query = query.Where(o => o.Numero == numero);

            if (!string.IsNullOrWhiteSpace(ufCarregamento))
                query = query.Where(o => o.EstadoCarregamento.Sigla == ufCarregamento);

            if (!string.IsNullOrWhiteSpace(ufDescarregamento))
                query = query.Where(o => o.EstadoDescarregamento.Sigla == ufDescarregamento);

            if (!string.IsNullOrWhiteSpace(placaVeiculo))
                query = query.Where(obj => obj.Veiculos.Any(o => o.Placa == placaVeiculo) || obj.Reboques.Any(o => o.Placa == placaVeiculo));

            if (codigoCarga > 0)
            {
                var queryCarga = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaMDFe>();
                queryCarga = queryCarga.Where(o => o.Carga.Codigo == codigoCarga);
                query = query.Where(obj => queryCarga.Select(o => o.MDFe).Contains(obj));
            }

            return query.Count();
        }

        public bool VerificarSeExisteMDFeGerado(string chaveCTe, int portoOrigem, int portoDestino)
        {
            var queryCTes = this.SessionNHiBernate.Query<Dominio.Entidades.MunicipioDescarregamentoMDFe>();
            queryCTes = queryCTes.Where(o => o.Documentos.Any(d => d.CTe.Chave == chaveCTe) && o.MDFe.Status == StatusMDFe.Autorizado && o.MDFe.PortoDestino.Codigo == portoDestino && o.MDFe.PortoOrigem.Codigo == portoOrigem);

            return queryCTes.Any();
        }

        public bool VerificarSeExisteMDFePendente(DateTime dataFechamento, StatusMDFe[] statusMDFe)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.ManifestoEletronicoDeDocumentosFiscais>();

            query = query.Where(o => o.DataEmissao < dataFechamento.AddDays(1).Date && statusMDFe.Contains(o.Status));

            return query.Any();
        }

        public List<Dominio.Entidades.ManifestoEletronicoDeDocumentosFiscais> BuscarMDFesPendentesEncerramento()
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.ManifestoEletronicoDeDocumentosFiscais>();
            var result = from obj in query where obj.DataPrevisaoEncerramento != null && obj.DataPrevisaoEncerramento.Value <= DateTime.Now && obj.Status == StatusMDFe.Autorizado && obj.TentativaEncerramentoAutomatico <= 3 select obj;

            return result.ToList();
        }

        public List<Dominio.Entidades.ManifestoEletronicoDeDocumentosFiscais> BuscarMDFesPendentesEncerramento(int codigoViagem, int codigoPortoAtracacao)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.ManifestoEletronicoDeDocumentosFiscais>();
            var result = from obj in query where obj.PortoDestino.Codigo == codigoPortoAtracacao && obj.PedidoViagemNavio.Codigo == codigoViagem && obj.Status == StatusMDFe.Autorizado select obj;

            return result.ToList();
        }

        public List<Dominio.Entidades.ManifestoEletronicoDeDocumentosFiscais> BuscarMDFesPorTempoLimiteEmissao(int minutosLimiteEmissao)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.ManifestoEletronicoDeDocumentosFiscais>();
            var result = from obj in query where obj.DataIntegracao < DateTime.Now.AddMinutes(-minutosLimiteEmissao) && obj.Status == StatusMDFe.Enviado select obj;

            return result.ToList();
        }

        public int ContarMDFesPorTempoLimiteEmissao(int minutosLimiteEmissao)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.ManifestoEletronicoDeDocumentosFiscais>();
            var result = from obj in query where obj.DataIntegracao < DateTime.Now.AddMinutes(-minutosLimiteEmissao) && obj.Status == StatusMDFe.Enviado select obj;

            return result.Count();
        }

        public List<Dominio.Entidades.ManifestoEletronicoDeDocumentosFiscais> BuscarMDFEsRejeitadosPorPendenciaEncerramento(int minutosLimiteEmissao, List<int> codigosErros, int tentativaEncerramentoAutomatico)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.ManifestoEletronicoDeDocumentosFiscais>();
            var result = from obj in query
                         where
                             obj.DataEmissao >= DateTime.Now.AddDays(-5) &&
                             obj.DataIntegracao < DateTime.Now.AddMinutes(-minutosLimiteEmissao) &&
                             obj.Status == StatusMDFe.Rejeicao && obj.TentativaReenvio <= tentativaEncerramentoAutomatico &&
                             (codigosErros.Contains(obj.MensagemStatus.CodigoDoErro) || obj.MensagemRetornoSefaz.Contains("pendente de encerramento") || obj.MensagemRetornoSefaz.Contains("não encerrado"))

                         select obj;

            return result.ToList();
        }

        public List<Dominio.Entidades.ManifestoEletronicoDeDocumentosFiscais> BuscarMDFesPorTempoEmissaoFalhaSefaz(int minutosLimiteEmissao, int tentativas, List<int> codigosErros)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.ManifestoEletronicoDeDocumentosFiscais>();
            var result = from obj in query where obj.DataIntegracao < DateTime.Now.AddMinutes(-minutosLimiteEmissao) && obj.Status == StatusMDFe.Rejeicao && codigosErros.Contains(obj.MensagemStatus.CodigoDoErro) && obj.TentativaReenvio <= tentativas select obj;

            return result.ToList();
        }

        public Dominio.Entidades.ManifestoEletronicoDeDocumentosFiscais BuscarPorNumeroESerie(int codigoEmpresa, int numero, Dominio.Entidades.EmpresaSerie serie)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.ManifestoEletronicoDeDocumentosFiscais>();
            var result = from obj in query where obj.Empresa.Codigo == codigoEmpresa && obj.Numero == numero && obj.Serie == serie select obj;
            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.ManifestoEletronicoDeDocumentosFiscais> BuscarPorStatus(Dominio.Enumeradores.StatusMDFe[] status)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.ManifestoEletronicoDeDocumentosFiscais>();
            var result = from obj in query where status.Contains(obj.Status) select obj;
            return result.Timeout(120).ToList();
        }

        public List<Dominio.Entidades.ManifestoEletronicoDeDocumentosFiscais> BuscarPorStatusEDataAutorizacao(Dominio.Enumeradores.StatusMDFe status, DateTime dataAutorizacao, int quantidadePercuros = 0, Dominio.Enumeradores.TipoAmbiente tipoAmbiente = TipoAmbiente.Producao)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.ManifestoEletronicoDeDocumentosFiscais>();

            var result = from obj in query
                         where obj.Status == status &&
                               obj.DataAutorizacao <= dataAutorizacao &&
                               obj.TipoAmbiente == tipoAmbiente &&
                               obj.MensagemRetornoSefaz != "MDFe importado" &&
                               obj.Empresa.Status == "A" &&
                               obj.Empresa.StatusFinanceiro == "N"

                         select obj;

            if (quantidadePercuros > 0)
                result = result.Where(o => o.Percursos.Count <= quantidadePercuros);

            return result.Timeout(240).ToList();
        }

        public List<Dominio.Entidades.ManifestoEletronicoDeDocumentosFiscais> BuscarPendentesEncerramento(Dominio.Enumeradores.StatusMDFe status, DateTime dataAutorizacao, DateTime dataAutorizacaoInicial)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.ManifestoEletronicoDeDocumentosFiscais>();

            var result = from obj in query
                         where obj.Status == status &&
                               obj.DataAutorizacao <= dataAutorizacao &&
                               obj.DataAutorizacao >= dataAutorizacaoInicial &&
                               obj.TipoAmbiente == Dominio.Enumeradores.TipoAmbiente.Producao &&
                               obj.MensagemRetornoSefaz != "MDFe importado" &&
                               obj.Empresa.Status == "A" &&
                               obj.Empresa.StatusFinanceiro == "N"

                         select obj;

            return result.Timeout(240).ToList();
        }

        public List<Dominio.Entidades.ManifestoEletronicoDeDocumentosFiscais> BuscarPendentesEncerramentos(int codigoEmpresa, string placaTracao, DateTime dataAutorizacao, int quantidadePercuros, string ufCarregamento, string ufDescarregamento)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.ManifestoEletronicoDeDocumentosFiscais>();

            var result = from obj in query
                         where obj.Status == Dominio.Enumeradores.StatusMDFe.Autorizado &&
                               obj.DataAutorizacao <= dataAutorizacao &&
                               obj.Empresa.Codigo == codigoEmpresa

                         select obj;

            if (!string.IsNullOrWhiteSpace(placaTracao))
            {
                var queryVeiculos = this.SessionNHiBernate.Query<Dominio.Entidades.VeiculoMDFe>();

                var resultVeiculos = from obj in queryVeiculos where obj.Placa.Equals(placaTracao) select obj.MDFe.Codigo;

                result = result.Where(o => resultVeiculos.Contains(o.Codigo));
            }

            if (quantidadePercuros > 0)
                result = result.Where(o => o.Percursos.Count <= quantidadePercuros);

            if (!string.IsNullOrWhiteSpace(ufCarregamento))
                result = result.Where(o => o.EstadoCarregamento.Sigla == ufCarregamento);

            if (!string.IsNullOrWhiteSpace(ufDescarregamento))
                result = result.Where(o => o.EstadoDescarregamento.Sigla == ufDescarregamento);

            return result.Timeout(60).ToList();
        }

        public List<Dominio.Entidades.ManifestoEletronicoDeDocumentosFiscais> BuscarPendentesEncerramentosRaiz(string raizCNPJEmissor, string placaTracao, DateTime dataAutorizacao, int quantidadePercuros, string ufCarregamento, string ufDescarregamento)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.ManifestoEletronicoDeDocumentosFiscais>();

            var result = from obj in query
                         where obj.Status == Dominio.Enumeradores.StatusMDFe.Autorizado &&
                               obj.DataAutorizacao <= dataAutorizacao &&
                               obj.Empresa.CNPJ.Contains(raizCNPJEmissor)

                         select obj;

            if (!string.IsNullOrWhiteSpace(placaTracao))
            {
                var queryVeiculos = this.SessionNHiBernate.Query<Dominio.Entidades.VeiculoMDFe>();

                var resultVeiculos = from obj in queryVeiculos where obj.Placa.Equals(placaTracao) select obj.MDFe.Codigo;

                result = result.Where(o => resultVeiculos.Contains(o.Codigo));
            }

            if (quantidadePercuros > 0)
                result = result.Where(o => o.Percursos.Count <= quantidadePercuros);

            if (!string.IsNullOrWhiteSpace(ufCarregamento))
                result = result.Where(o => o.EstadoCarregamento.Sigla == ufCarregamento);

            if (!string.IsNullOrWhiteSpace(ufDescarregamento))
                result = result.Where(o => o.EstadoDescarregamento.Sigla == ufDescarregamento);

            return result.Timeout(60).ToList();
        }

        public List<Dominio.Entidades.ManifestoEletronicoDeDocumentosFiscais> ConsultarMDFesParaCobrancaMensal(int codigoEmpresaPai, int codigoEmpresa, DateTime dataInicial, DateTime dataFinal, Dominio.Enumeradores.TipoAmbiente tipoAmbiente)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.ManifestoEletronicoDeDocumentosFiscais>();

            var result = from obj in query where obj.Empresa.EmpresaPai.Codigo == codigoEmpresaPai && obj.TipoAmbiente == tipoAmbiente select obj;

            if (codigoEmpresa > 0)
                result = result.Where(obj => obj.Empresa.Codigo == codigoEmpresa);

            if (dataInicial != DateTime.MinValue)
                query = query.Where(o => o.DataEmissao >= dataInicial.Date);

            if (dataFinal != DateTime.MinValue)
                query = query.Where(o => o.DataEmissao < dataFinal.AddDays(1).Date);

            return result.ToList();
        }

        public int ContarMDFesParaCobrancaMensal(int codigoEmpresaPai, int codigoEmpresa, DateTime dataInicial, DateTime dataFinal, Dominio.Enumeradores.TipoAmbiente tipoAmbiente, int[] series, int[] seriesDiferente)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.ManifestoEletronicoDeDocumentosFiscais>();

            var result = from obj in query where obj.TipoAmbiente == tipoAmbiente select obj;

            result = result.Where(obj => (obj.Status == StatusMDFe.Autorizado || obj.Status == StatusMDFe.Encerrado || obj.CobrarCancelamento));

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

        public List<Dominio.Entidades.ManifestoEletronicoDeDocumentosFiscais> BuscarPorStatusEPeriodo(int codigoEmpresa, Dominio.Enumeradores.StatusMDFe status, DateTime dataInicial, DateTime dataFinal)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.ManifestoEletronicoDeDocumentosFiscais>();

            var result = from obj in query
                         where obj.Status == status &&
                               obj.DataAutorizacao <= dataFinal.Date.AddDays(1) &&
                               obj.DataAutorizacao >= dataInicial.Date
                         select obj;

            if (codigoEmpresa > 0)
                result = result.Where(o => o.Empresa.Codigo == codigoEmpresa);

            return result.Timeout(120).ToList();
        }

        public List<int> BuscarCodigosPorStatusEPeriodo(int codigoEmpresa, Dominio.Enumeradores.StatusMDFe status, DateTime dataInicial, DateTime dataFinal)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.ManifestoEletronicoDeDocumentosFiscais>();

            var result = from obj in query
                         where obj.Status == status &&
                               obj.DataAutorizacao <= dataFinal.Date.AddDays(1) &&
                               obj.DataAutorizacao >= dataInicial.Date
                         select obj;

            if (codigoEmpresa > 0)
                result = result.Where(o => o.Empresa.Codigo == codigoEmpresa);

            return result.Select(o => o.Codigo).ToList();
        }

        public int ContarConsulta(Dominio.ObjetosDeValor.Embarcador.MDFe.FiltroConsultaMDFe filtrosPesquisa)
        {
            IQueryable<Dominio.Entidades.ManifestoEletronicoDeDocumentosFiscais> query = MontarConsulta(filtrosPesquisa);
            return query.Count();
        }

        public List<Dominio.Entidades.ManifestoEletronicoDeDocumentosFiscais> Consultar(Dominio.ObjetosDeValor.Embarcador.MDFe.FiltroConsultaMDFe filtrosPesquisa, string propOrdenacao, string dirOrdena, int inicio, int limite)
        {
            IQueryable<Dominio.Entidades.ManifestoEletronicoDeDocumentosFiscais> query = MontarConsulta(filtrosPesquisa);

            return query.Fetch(o => o.EstadoCarregamento)
                        .Fetch(o => o.EstadoDescarregamento)
                        .Fetch(o => o.Serie)
                        .Fetch(o => o.Empresa)
                        .OrderBy(propOrdenacao + " " + dirOrdena)
                        .Skip(inicio)
                        .Take(limite)
                        .ToList();

        }

        public List<int> ConsultarCodigos(Dominio.ObjetosDeValor.Embarcador.MDFe.FiltroConsultaMDFe filtrosPesquisa)
        {
            IQueryable<Dominio.Entidades.ManifestoEletronicoDeDocumentosFiscais> query = MontarConsulta(filtrosPesquisa);

            return query.Select(obj => obj.Codigo).ToList();
        }

        public List<string> ConsultarChaves(Dominio.ObjetosDeValor.Embarcador.MDFe.FiltroConsultaMDFe filtrosPesquisa)
        {
            IQueryable<Dominio.Entidades.ManifestoEletronicoDeDocumentosFiscais> query = MontarConsulta(filtrosPesquisa);

            return query.Select(obj => obj.Chave).ToList();
        }

        public int BuscarUltimoNumero(int codigoEmpresa, int codigoSerie, Dominio.Enumeradores.TipoAmbiente ambiente)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.ManifestoEletronicoDeDocumentosFiscais>();
            var result = from obj in query where obj.Empresa.Codigo == codigoEmpresa && obj.Serie.Codigo == codigoSerie && obj.TipoAmbiente == ambiente select obj;

            int? retorno = result.Max(o => (int?)o.Numero);

            return retorno.HasValue ? retorno.Value : 0;
        }

        public int BuscarUltimoNumeroLock(int codigoEmpresa, int codigoSerie, Dominio.Enumeradores.TipoAmbiente ambiente)
        {
            string tipoAmbiente = ambiente == Dominio.Enumeradores.TipoAmbiente.Homologacao ? "2" : "1";

            string query = @"SELECT MAX(MDF_NUMERO) NUMERO FROM T_MDFE (XLOCK) 
                             WHERE EMP_CODIGO = " + codigoEmpresa.ToString() +
                           "       AND ESE_CODIGO = " + codigoSerie.ToString() +
                           "       AND MDF_AMBIENTE = " + tipoAmbiente;

            var result = this.SessionNHiBernate.CreateSQLQuery(query).SetTimeout(120).UniqueResult();

            int numero = 0;

            if (result == null)
                return 0;

            int.TryParse(result.ToString(), out numero);

            return numero;
        }

        public List<Dominio.Entidades.ManifestoEletronicoDeDocumentosFiscais> ConsultarPendentesDeEncerramento(int codigoEmpresa, DateTime dataAutorizacao, Dominio.Enumeradores.TipoAmbiente tipoAmbiente, int inicioRegistros, int maximoRegistros)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.ManifestoEletronicoDeDocumentosFiscais>();

            var result = from obj in query where obj.Empresa.Codigo == codigoEmpresa && obj.DataAutorizacao <= dataAutorizacao && obj.Status == Dominio.Enumeradores.StatusMDFe.Autorizado && obj.TipoAmbiente == tipoAmbiente select obj;

            return result.Skip(inicioRegistros).Take(maximoRegistros).OrderBy(o => o.DataAutorizacao).Timeout(120).ToList();
        }

        public List<Dominio.Entidades.ManifestoEletronicoDeDocumentosFiscais> ConsultarNaoEncerrados(int codigoEmpresaPai, DateTime dataAutorizacao, List<int> codigosFiliais, List<double> codigosRecebedores, int inicioRegistros, int maximoRegistros)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.ManifestoEletronicoDeDocumentosFiscais>();

            var result = from obj in query where obj.Empresa.EmpresaPai.Codigo == codigoEmpresaPai && obj.DataAutorizacao <= dataAutorizacao.Date && obj.Status == Dominio.Enumeradores.StatusMDFe.Autorizado && obj.TipoAmbiente == Dominio.Enumeradores.TipoAmbiente.Producao select obj;

            if (codigosFiliais.Any(codigo => codigo == -1))
            {
                var subquery = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaMDFe>();
                query = query.Where(mdfe => subquery.Any(cargaMdfe => (cargaMdfe.MDFe.Codigo == mdfe.Codigo) && (codigosFiliais.Contains(cargaMdfe.Carga.Filial.Codigo)) || cargaMdfe.Carga.Pedidos.Any(pedido => codigosRecebedores.Contains(pedido.Recebedor.CPF_CNPJ))));
            }


            return result.Skip(inicioRegistros).Take(maximoRegistros).OrderBy(o => o.DataAutorizacao).Timeout(120).ToList();
        }

        public int ContarConsultaNaoEncerrados(int codigoEmpresaPai, DateTime dataAutorizacao)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.ManifestoEletronicoDeDocumentosFiscais>();

            var result = from obj in query where obj.Empresa.EmpresaPai.Codigo == codigoEmpresaPai && obj.DataAutorizacao <= dataAutorizacao.Date && obj.Status == Dominio.Enumeradores.StatusMDFe.Autorizado && obj.TipoAmbiente == Dominio.Enumeradores.TipoAmbiente.Producao select obj;

            return result.Count();
        }

        public Dominio.Entidades.ManifestoEletronicoDeDocumentosFiscais BuscarPorChaveCTe(string chave)
        {
            IQueryable<Dominio.Entidades.DocumentoMunicipioDescarregamentoMDFe> query = SessionNHiBernate.Query<Dominio.Entidades.DocumentoMunicipioDescarregamentoMDFe>();

            query = query.Where(o => o.Chave == chave);

            return query.Select(o => o.MunicipioDescarregamento.MDFe).OrderByDescending(o => o.Codigo).FirstOrDefault();
        }

        public Dominio.Entidades.DocumentoMunicipioDescarregamentoMDFe BuscarPorCargaPedidoDocumentoCTeEStatus(int codigoCarga, StatusMDFe[] statusMDFes)
        {
            IQueryable<Dominio.Entidades.Embarcador.Cargas.CargaPedidoDocumentoCTe> queryCargaPedidoDocumentoCTe = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedidoDocumentoCTe>();
            IQueryable<Dominio.Entidades.DocumentoMunicipioDescarregamentoMDFe> queryDocumentoMunicipioDescarregamentoMDFe = SessionNHiBernate.Query<Dominio.Entidades.DocumentoMunicipioDescarregamentoMDFe>();

            queryCargaPedidoDocumentoCTe = queryCargaPedidoDocumentoCTe.Where(o => o.CargaPedido.Carga.Codigo == codigoCarga);
            queryDocumentoMunicipioDescarregamentoMDFe = queryDocumentoMunicipioDescarregamentoMDFe.Where(o => statusMDFes.Contains(o.MunicipioDescarregamento.MDFe.Status));

            queryDocumentoMunicipioDescarregamentoMDFe = queryDocumentoMunicipioDescarregamentoMDFe.Where(o => queryCargaPedidoDocumentoCTe.Select(c => c.CTe.Chave).Contains(o.CTe.Chave));

            return queryDocumentoMunicipioDescarregamentoMDFe.FirstOrDefault();
        }

        public Dominio.Entidades.DocumentoMunicipioDescarregamentoMDFe BuscarPorCargaEStatus(int[] codigoCargas, StatusMDFe[] statusMDFes)
        {
            IQueryable<Dominio.Entidades.Embarcador.Cargas.CargaCTe> queryCargaCTe = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCTe>();
            IQueryable<Dominio.Entidades.DocumentoMunicipioDescarregamentoMDFe> queryDocumentoMunicipioDescarregamentoMDFe = SessionNHiBernate.Query<Dominio.Entidades.DocumentoMunicipioDescarregamentoMDFe>();

            queryCargaCTe = queryCargaCTe.Where(o => codigoCargas.Contains(o.Carga.Codigo));
            queryDocumentoMunicipioDescarregamentoMDFe = queryDocumentoMunicipioDescarregamentoMDFe.Where(o => statusMDFes.Contains(o.MunicipioDescarregamento.MDFe.Status));

            queryDocumentoMunicipioDescarregamentoMDFe = queryDocumentoMunicipioDescarregamentoMDFe.Where(o => queryCargaCTe.Select(c => c.CTe.Chave).Contains(o.CTe.Chave));

            var result = queryDocumentoMunicipioDescarregamentoMDFe.ToList();

            return result.FirstOrDefault();
        }

        public Dominio.Entidades.DocumentoMunicipioDescarregamentoMDFe BuscarPorCargaCTeEStatus(int[] codigoCargaCTes, StatusMDFe[] statusMDFes)
        {
            IQueryable<Dominio.Entidades.Embarcador.Cargas.CargaCTe> queryCargaCTe = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCTe>();
            IQueryable<Dominio.Entidades.DocumentoMunicipioDescarregamentoMDFe> queryDocumentoMunicipioDescarregamentoMDFe = SessionNHiBernate.Query<Dominio.Entidades.DocumentoMunicipioDescarregamentoMDFe>();

            queryCargaCTe = queryCargaCTe.Where(o => codigoCargaCTes.Contains(o.Codigo));
            queryDocumentoMunicipioDescarregamentoMDFe = queryDocumentoMunicipioDescarregamentoMDFe.Where(o => statusMDFes.Contains(o.MunicipioDescarregamento.MDFe.Status));

            queryDocumentoMunicipioDescarregamentoMDFe = queryDocumentoMunicipioDescarregamentoMDFe.Where(o => queryCargaCTe.Select(c => c.CTe.Chave).Contains(o.CTe.Chave));

            var result = queryDocumentoMunicipioDescarregamentoMDFe.ToList();

            return result.FirstOrDefault();
        }


        public List<Dominio.Entidades.ManifestoEletronicoDeDocumentosFiscais> BuscarPorCargaMDFeEStatus(int codigoCarga, StatusMDFe statusMDFes)
        {
            IQueryable<Dominio.Entidades.Embarcador.Cargas.CargaMDFe> queryCargaMDFe = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaMDFe>();

            var mdfes = queryCargaMDFe.Where(m => m.Carga.Codigo == codigoCarga && m.MDFe.Status == statusMDFes).Select(m => m.MDFe);

            return mdfes.ToList();
        }

        public Dominio.Entidades.ManifestoEletronicoDeDocumentosFiscais BuscarPorCodigoCTe(int codigoCTe)
        {
            IQueryable<Dominio.Entidades.DocumentoMunicipioDescarregamentoMDFe> query = SessionNHiBernate.Query<Dominio.Entidades.DocumentoMunicipioDescarregamentoMDFe>();

            query = query.Where(o => o.CTe.Codigo == codigoCTe);

            return query.Select(o => o.MunicipioDescarregamento.MDFe).OrderByDescending(o => o.Codigo).FirstOrDefault();
        }

        public Dominio.Entidades.ManifestoEletronicoDeDocumentosFiscais BuscarPrimeiroPorCarga(int codigoCarga)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaMDFe>();
            var result = from obj in query where obj.Carga.Codigo == codigoCarga select obj.MDFe;

            return result.FirstOrDefault();
        }

        public async Task<Dominio.Entidades.ManifestoEletronicoDeDocumentosFiscais> BuscarPrimeiroPorCargaAsync(int codigoCarga, CancellationToken cancellationToken)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaMDFe>();
            var result = from obj in query where obj.Carga.Codigo == codigoCarga select obj.MDFe;

            return await result.FirstOrDefaultAsync(cancellationToken);
        }

        public List<Dominio.Entidades.ManifestoEletronicoDeDocumentosFiscais> Consultar(
            int codigoEmpresa,
            int codigoMDFe,
            Dominio.Enumeradores.TipoAmbiente ambiente,
            int codigoSerie,
            int numeroInicial,
            int numeroFinal,
            DateTime dataInicial,
            DateTime dataFinal,
            Dominio.Enumeradores.StatusMDFe? status,
            string ufCarregamento,
            string ufDescarregamento,
            string placaVeiculo,
            string placaReboque,
            string cpfMotorista,
            string nomeMotorista,
            int[] series,
            int numeroCTe,
            int inicioRegistros,
            int maximoRegistros,
            string nomeUsuario = "",
            string numeroCarga = "",
            string numeroUnidade = "",
            int empresaPai = 0
        )
        {
            var result = _Consultar(codigoEmpresa, codigoMDFe, ambiente, codigoSerie, numeroInicial, numeroFinal, dataInicial, dataFinal, status, ufCarregamento, ufDescarregamento, placaVeiculo, placaReboque, cpfMotorista, nomeMotorista, series, numeroCTe, nomeUsuario, numeroCarga, numeroUnidade, empresaPai);

            return result.OrderByDescending(o => o.DataEmissao)
                         .ThenByDescending(o => o.Numero)
                         .Skip(inicioRegistros)
                         .Take(maximoRegistros)
                         .Fetch(o => o.EstadoCarregamento)
                         .Fetch(o => o.EstadoDescarregamento)
                         .Fetch(o => o.Serie)
                         .Fetch(o => o.MensagemStatus)
                         .Timeout(120)
                         .ToList();
        }

        public int ContarConsulta(
            int codigoEmpresa,
            int codigoMDFe,
            Dominio.Enumeradores.TipoAmbiente ambiente,
            int codigoSerie,
            int numeroInicial,
            int numeroFinal,
            DateTime dataInicial,
            DateTime dataFinal,
            Dominio.Enumeradores.StatusMDFe? status,
            string ufCarregamento,
            string ufDescarregamento,
            string placaVeiculo,
            string placaReboque,
            string cpfMotorista,
            string nomeMotorista,
            int[] series,
            int numeroCTe,
            string nomeUsuario = "",
            string numeroCarga = "",
            string numeroUnidade = "",
            int empresaPai = 0)
        {
            var result = _Consultar(codigoEmpresa, codigoMDFe, ambiente, codigoSerie, numeroInicial, numeroFinal, dataInicial, dataFinal, status, ufCarregamento, ufDescarregamento, placaVeiculo, placaReboque, cpfMotorista, nomeMotorista, series, numeroCTe, nomeUsuario, numeroCarga, numeroUnidade, empresaPai);

            return result.Count();
        }

        public List<string> BuscarListaChaveMDFes(int codigoEmpresa, int codigoMDFe, Dominio.Enumeradores.TipoAmbiente ambiente, int codigoSerie, int numeroInicial, int numeroFinal, DateTime dataInicial, DateTime dataFinal, Dominio.Enumeradores.StatusMDFe? status, string ufCarregamento, string ufDescarregamento, string placaVeiculo, string placaReboque, string cpfMotorista, string nomeMotorista, int[] series, string nomeUsuario = "", string numeroCarga = "", string numeroUnidade = "", int empresaPai = 0)
        {
            int numeroCTe = 0;
            var result = _Consultar(codigoEmpresa, codigoMDFe, ambiente, codigoSerie, numeroInicial, numeroFinal, dataInicial, dataFinal, status, ufCarregamento, ufDescarregamento, placaVeiculo, placaReboque, cpfMotorista, nomeMotorista, series, numeroCTe, nomeUsuario, numeroCarga, numeroUnidade, empresaPai);

            return result.Where(o => o.Chave != null).Select(o => o.Chave).ToList();
        }

        public List<int> ObterCodigosDownloadLote(int codigoEmpresa, Dominio.Enumeradores.TipoAmbiente ambiente, int codigoSerie, int numeroInicial, int numeroFinal, DateTime dataInicial, DateTime dataFinal, Dominio.Enumeradores.StatusMDFe? status, string ufCarregamento, string ufDescarregamento, string placaVeiculo, string placaReboque, string cpfMotorista, string nomeMotorista, int[] series, string nomeUsuario = "", string numeroCarga = "", string numeroUnidade = "", int empresaPai = 0)
        {
            int numeroCTe = 0;
            var result = _Consultar(codigoEmpresa, 0, ambiente, codigoSerie, numeroInicial, numeroFinal, dataInicial, dataFinal, status, ufCarregamento, ufDescarregamento, placaVeiculo, placaReboque, cpfMotorista, nomeMotorista, series, numeroCTe, nomeUsuario, numeroCarga, numeroUnidade, empresaPai);

            return result.Select(o => o.Codigo).ToList();
        }

        public IList<Dominio.ObjetosDeValor.Relatorios.RelatorioMDFeAgrupado> RelatorioMDFesAgrupados(int codigoEmpresaPai, int codigoEmpresaEmissora, DateTime dataInicial, DateTime dataFinal, DateTime dataAutorizacaoInicial, DateTime dataAutorizacaoFinal)
        {
            var criteria = this.SessionNHiBernate.CreateCriteria<Dominio.Entidades.ManifestoEletronicoDeDocumentosFiscais>();

            criteria.CreateAlias("Empresa", "empresa");

            criteria.Add(Restrictions.Eq("empresa.EmpresaPai.Codigo", codigoEmpresaPai));

            criteria.Add(Restrictions.Eq("TipoAmbiente", Dominio.Enumeradores.TipoAmbiente.Producao));

            criteria.Add(Restrictions.In("Status", new Dominio.Enumeradores.StatusMDFe[] { Dominio.Enumeradores.StatusMDFe.Autorizado, Dominio.Enumeradores.StatusMDFe.Cancelado, Dominio.Enumeradores.StatusMDFe.Encerrado }));

            if (codigoEmpresaEmissora > 0)
                criteria.Add(Restrictions.Eq("empresa.Codigo", codigoEmpresaEmissora));

            if (dataInicial != DateTime.MinValue)
                criteria.Add(Restrictions.Ge("DataEmissao", dataInicial));

            if (dataFinal != DateTime.MinValue)
                criteria.Add(Restrictions.Lt("DataEmissao", dataFinal.AddDays(1)));

            if (dataAutorizacaoInicial != DateTime.MinValue)
                criteria.Add(Restrictions.Ge("DataAutorizacao", dataAutorizacaoInicial));

            if (dataAutorizacaoFinal != DateTime.MinValue)
                criteria.Add(Restrictions.Lt("DataAutorizacao", dataAutorizacaoFinal.AddDays(1)));

            criteria.SetProjection(Projections.ProjectionList()
                .Add(Projections.GroupProperty("empresa.RazaoSocial"), "Empresa")
                .Add(Projections.GroupProperty("empresa.Codigo"), "CodigoEmpresa")
                .Add(Projections.GroupProperty("empresa.CNPJ"), "CNPJEmpresa")
                .Add(Projections.GroupProperty("Status"), "Status")
                .Add(Projections.RowCount(), "CountMDFes"));

            criteria.SetResultTransformer(NHibernate.Transform.Transformers.AliasToBean(typeof(Dominio.ObjetosDeValor.Relatorios.RelatorioMDFeAgrupado)));

            return criteria.List<Dominio.ObjetosDeValor.Relatorios.RelatorioMDFeAgrupado>();
        }

        public List<Dominio.ObjetosDeValor.Relatorios.RelatorioMDFesEmitidos> RelatorioMDFesEmitidos(int codigoEmpresa, DateTime dataInicial, DateTime dataFinal, int numeroInicial, int numeroFinal, string placaVeiculo, string cpfMotorista, string nomeMotorista, string ufCarregamento, string ufDescarregamento, Dominio.Enumeradores.StatusMDFe status, int empresaPai = 0, string cnpjRemetenteCTe = "", bool todosCNPJRaiz = false, string cnpjEmbarcadorUsuario = "", string nomeUsuario = "", bool tipoSeguradora = false, string numeroCarga = "", string numeroUnidade = "")
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.ManifestoEletronicoDeDocumentosFiscais>();

            var result = from obj in query where 1 == 1 select obj;

            if (empresaPai > 0)
                result = result.Where(o => o.Empresa.EmpresaPai.Codigo == empresaPai);

            if (codigoEmpresa > 0)
                result = result.Where(o => o.Empresa.Codigo == codigoEmpresa);

            if (dataInicial != DateTime.MinValue)
                result = result.Where(o => o.DataEmissao >= dataInicial.Date);

            if (dataFinal != DateTime.MinValue)
                result = result.Where(o => o.DataEmissao < dataFinal.Date.AddDays(1));

            if (numeroInicial > 0)
                result = result.Where(o => o.Numero >= numeroInicial);

            if (numeroFinal > 0)
                result = result.Where(o => o.Numero <= numeroFinal);

            if (!string.IsNullOrWhiteSpace(nomeUsuario))
                result = result.Where(o => (o.Log.Contains(nomeUsuario)));

            if (!string.IsNullOrWhiteSpace(placaVeiculo))
            {
                var queryVeiculos = this.SessionNHiBernate.Query<Dominio.Entidades.VeiculoMDFe>();

                var resultVeiculos = from obj in queryVeiculos select obj;

                if (!string.IsNullOrWhiteSpace(placaVeiculo))
                    resultVeiculos = resultVeiculos.Where(o => o.Placa.Equals(placaVeiculo));

                result = result.Where(o => resultVeiculos.Select(x => x.MDFe.Codigo).Contains(o.Codigo));
            }

            if (!string.IsNullOrWhiteSpace(cpfMotorista) || !string.IsNullOrWhiteSpace(nomeMotorista))
            {
                var queryMotoristas = this.SessionNHiBernate.Query<Dominio.Entidades.MotoristaMDFe>();

                var resultMotoristas = from obj in queryMotoristas select obj;

                if (!string.IsNullOrWhiteSpace(cpfMotorista))
                    resultMotoristas = resultMotoristas.Where(o => o.CPF.Equals(cpfMotorista));

                if (!string.IsNullOrWhiteSpace(nomeMotorista))
                    resultMotoristas = resultMotoristas.Where(o => o.Nome.Equals(nomeMotorista));

                result = result.Where(o => resultMotoristas.Select(x => x.MDFe.Codigo).Contains(o.Codigo));
            }

            if (!string.IsNullOrWhiteSpace(ufCarregamento))
                result = result.Where(o => o.EstadoCarregamento.Sigla.Equals(ufCarregamento));

            if (!string.IsNullOrWhiteSpace(ufDescarregamento))
                result = result.Where(o => o.EstadoDescarregamento.Sigla.Equals(ufDescarregamento));

            if (status != Dominio.Enumeradores.StatusMDFe.Todos)
                result = result.Where(o => o.Status == status);

            if (!string.IsNullOrWhiteSpace(cnpjRemetenteCTe))
            {
                var queryMunicipioDescarregamentoMDFe = this.SessionNHiBernate.Query<Dominio.Entidades.MunicipioDescarregamentoMDFe>();

                var resultMunicipioDescarregamentoMDFe = from obj in queryMunicipioDescarregamentoMDFe select obj;

                if (todosCNPJRaiz)
                    resultMunicipioDescarregamentoMDFe = resultMunicipioDescarregamentoMDFe.Where(o => o.Documentos.Any(x => x.CTe.Remetente.CPF_CNPJ.Contains(cnpjRemetenteCTe.Substring(0, 8))));
                else
                    resultMunicipioDescarregamentoMDFe = resultMunicipioDescarregamentoMDFe.Where(o => o.Documentos.Any(x => x.CTe.Remetente.CPF_CNPJ.Equals(cnpjRemetenteCTe)));

                result = result.Where(o => resultMunicipioDescarregamentoMDFe.Select(x => x.MDFe.Codigo).Contains(o.Codigo));

            }

            if (!string.IsNullOrWhiteSpace(cnpjEmbarcadorUsuario))
            {
                var queryMunicipioDescarregamentoMDFe = this.SessionNHiBernate.Query<Dominio.Entidades.MunicipioDescarregamentoMDFe>();

                var resultMunicipioDescarregamentoMDFe = from obj in queryMunicipioDescarregamentoMDFe select obj;

                if (cnpjEmbarcadorUsuario.Length == 8)
                    resultMunicipioDescarregamentoMDFe = resultMunicipioDescarregamentoMDFe.Where(o => o.Documentos.Any(x => x.CTe.Remetente.CPF_CNPJ.Contains(cnpjEmbarcadorUsuario)));
                else
                    resultMunicipioDescarregamentoMDFe = resultMunicipioDescarregamentoMDFe.Where(o => o.Documentos.Any(x => x.CTe.Remetente.CPF_CNPJ.Equals(cnpjEmbarcadorUsuario)));

                result = result.Where(o => resultMunicipioDescarregamentoMDFe.Select(x => x.MDFe.Codigo).Contains(o.Codigo));
            }

            if ((!string.IsNullOrWhiteSpace(numeroCarga) && numeroCarga != "0") || (!string.IsNullOrWhiteSpace(numeroUnidade) && numeroUnidade != "0"))
            {
                var queryIntegracaoMDFe = this.SessionNHiBernate.Query<Dominio.Entidades.IntegracaoMDFe>();
                var resultIntegracaoMDFe = from o in queryIntegracaoMDFe select o;
                if (!string.IsNullOrWhiteSpace(numeroCarga) && numeroCarga != "0")
                    resultIntegracaoMDFe = resultIntegracaoMDFe.Where(o => o.NumeroDaCarga == numeroCarga);
                if (!string.IsNullOrWhiteSpace(numeroUnidade) && numeroUnidade != "0")
                    resultIntegracaoMDFe = resultIntegracaoMDFe.Where(o => o.NumeroDaUnidade == numeroUnidade);

                result = result.Where(o => resultIntegracaoMDFe.Select(c => c.MDFe.Codigo).Contains(o.Codigo));
            }

            return result.Select(o => new Dominio.ObjetosDeValor.Relatorios.RelatorioMDFesEmitidos()
            {
                CodigoMDFe = o.Codigo,
                NomeTransportadora = !tipoSeguradora ? o.Empresa.CNPJ + " " + o.Empresa.RazaoSocial : o.Empresa.CNPJ,
                DataAutorizacao = o.DataAutorizacao,
                DataCancelamento = o.DataCancelamento,
                DataEmissao = o.DataEmissao,
                DataEncerramento = o.DataEncerramento,
                DescricaoUFCarregamento = !tipoSeguradora ? o.EstadoCarregamento.Nome : (from obj in this.SessionNHiBernate.Query<Dominio.Entidades.MunicipioCarregamentoMDFe>() where obj.MDFe.Codigo == o.Codigo select obj.Municipio.Descricao).FirstOrDefault(),
                UFCarregamento = o.EstadoCarregamento.Sigla,
                DescricaoUFDescarregamento = !tipoSeguradora ? o.EstadoDescarregamento.Nome : (from obj in this.SessionNHiBernate.Query<Dominio.Entidades.MunicipioDescarregamentoMDFe>() where obj.MDFe.Codigo == o.Codigo select obj.Municipio.Descricao).FirstOrDefault(),
                UFDescarregamento = o.EstadoDescarregamento.Sigla,
                Numero = o.Numero,
                PesoBrutoMercadoria = o.PesoBrutoMercadoria,
                Placa = (from obj in this.SessionNHiBernate.Query<Dominio.Entidades.VeiculoMDFe>() where obj.MDFe.Codigo == o.Codigo select obj.Placa).FirstOrDefault(),
                MotoristaNome = (from obj in this.SessionNHiBernate.Query<Dominio.Entidades.MotoristaMDFe>() where obj.MDFe.Codigo == o.Codigo select obj.Nome).FirstOrDefault(),
                MotoristaCPF = (from obj in this.SessionNHiBernate.Query<Dominio.Entidades.MotoristaMDFe>() where obj.MDFe.Codigo == o.Codigo select obj.CPF).FirstOrDefault(),
                Serie = o.Serie.Numero,
                UnidadeMedida = o.UnidadeMedidaMercadoria,
                ValorTotalMercadoria = o.ValorTotalMercadoria,
                Status = o.Status,
                CNPJEmbarcador = (from obj in this.SessionNHiBernate.Query<Dominio.Entidades.DocumentoMunicipioDescarregamentoMDFe>() where obj.MunicipioDescarregamento.MDFe.Codigo == o.Codigo select obj.CTe.Remetente.CPF_CNPJ).FirstOrDefault(),
                RazaoEmbarcador = (from obj in this.SessionNHiBernate.Query<Dominio.Entidades.DocumentoMunicipioDescarregamentoMDFe>() where obj.MunicipioDescarregamento.MDFe.Codigo == o.Codigo select obj.CTe.Remetente.Nome).FirstOrDefault()
            }).Timeout(120).ToList(); //.OrderBy(o => o.Numero).ThenBy(o => o.Serie)
        }

        public List<Dominio.ObjetosDeValor.Relatorios.RelatorioMDFesEmitidosAverbacoes> RelatorioMDFesEmitidosAverbacao(int codigoEmpresa, DateTime dataInicial, DateTime dataFinal, int numeroInicial, int numeroFinal, string placaVeiculo, string cpfMotorista, string nomeMotorista, string ufCarregamento, string ufDescarregamento, Dominio.Enumeradores.StatusMDFe status, int empresaPai, string cnpjRemetenteCTe, bool todosCNPJRaiz, string cnpjEmbarcadorUsuario, string nomeUsuario, bool tipoSeguradora, string numeroCarga, string numeroUnidade, bool averbacao99999)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.MDFeSeguro>();

            var result = from obj in query where 1 == 1 select obj;

            if (empresaPai > 0)
                result = result.Where(o => o.MDFe.Empresa.EmpresaPai.Codigo == empresaPai);

            if (codigoEmpresa > 0)
                result = result.Where(o => o.MDFe.Empresa.Codigo == codigoEmpresa);

            if (dataInicial != DateTime.MinValue)
                result = result.Where(o => o.MDFe.DataEmissao >= dataInicial.Date);

            if (dataFinal != DateTime.MinValue)
                result = result.Where(o => o.MDFe.DataEmissao < dataFinal.Date.AddDays(1));

            if (numeroInicial > 0)
                result = result.Where(o => o.MDFe.Numero >= numeroInicial);

            if (numeroFinal > 0)
                result = result.Where(o => o.MDFe.Numero <= numeroFinal);

            if (!string.IsNullOrWhiteSpace(nomeUsuario))
                result = result.Where(o => o.MDFe.Log.Contains(nomeUsuario));

            if (averbacao99999)
                result = result.Where(o => o.NumeroAverbacao.Equals("99999"));

            if (!string.IsNullOrWhiteSpace(placaVeiculo))
            {
                var queryVeiculos = this.SessionNHiBernate.Query<Dominio.Entidades.VeiculoMDFe>();

                var resultVeiculos = from obj in queryVeiculos select obj;

                if (!string.IsNullOrWhiteSpace(placaVeiculo))
                    resultVeiculos = resultVeiculos.Where(o => o.Placa.Equals(placaVeiculo));

                result = result.Where(o => resultVeiculos.Select(x => x.MDFe.Codigo).Contains(o.MDFe.Codigo));
            }

            if (!string.IsNullOrWhiteSpace(cpfMotorista) || !string.IsNullOrWhiteSpace(nomeMotorista))
            {
                var queryMotoristas = this.SessionNHiBernate.Query<Dominio.Entidades.MotoristaMDFe>();

                var resultMotoristas = from obj in queryMotoristas select obj;

                if (!string.IsNullOrWhiteSpace(cpfMotorista))
                    resultMotoristas = resultMotoristas.Where(o => o.CPF.Equals(cpfMotorista));

                if (!string.IsNullOrWhiteSpace(nomeMotorista))
                    resultMotoristas = resultMotoristas.Where(o => o.Nome.Equals(nomeMotorista));

                result = result.Where(o => resultMotoristas.Select(x => x.MDFe.Codigo).Contains(o.MDFe.Codigo));
            }

            if (!string.IsNullOrWhiteSpace(ufCarregamento))
                result = result.Where(o => o.MDFe.EstadoCarregamento.Sigla.Equals(ufCarregamento));

            if (!string.IsNullOrWhiteSpace(ufDescarregamento))
                result = result.Where(o => o.MDFe.EstadoDescarregamento.Sigla.Equals(ufDescarregamento));

            if (status != Dominio.Enumeradores.StatusMDFe.Todos)
                result = result.Where(o => o.MDFe.Status == status);

            if (!string.IsNullOrWhiteSpace(cnpjRemetenteCTe))
            {
                var queryMunicipioDescarregamentoMDFe = this.SessionNHiBernate.Query<Dominio.Entidades.MunicipioDescarregamentoMDFe>();

                var resultMunicipioDescarregamentoMDFe = from obj in queryMunicipioDescarregamentoMDFe select obj;

                if (todosCNPJRaiz)
                    resultMunicipioDescarregamentoMDFe = resultMunicipioDescarregamentoMDFe.Where(o => o.Documentos.Any(x => x.CTe.Remetente.CPF_CNPJ.Contains(cnpjRemetenteCTe.Substring(0, 8))));
                else
                    resultMunicipioDescarregamentoMDFe = resultMunicipioDescarregamentoMDFe.Where(o => o.Documentos.Any(x => x.CTe.Remetente.CPF_CNPJ.Equals(cnpjRemetenteCTe)));

                result = result.Where(o => resultMunicipioDescarregamentoMDFe.Select(x => x.MDFe.Codigo).Contains(o.MDFe.Codigo));

            }

            if (!string.IsNullOrWhiteSpace(cnpjEmbarcadorUsuario))
            {
                var queryMunicipioDescarregamentoMDFe = this.SessionNHiBernate.Query<Dominio.Entidades.MunicipioDescarregamentoMDFe>();

                var resultMunicipioDescarregamentoMDFe = from obj in queryMunicipioDescarregamentoMDFe select obj;

                if (cnpjEmbarcadorUsuario.Length == 8)
                    resultMunicipioDescarregamentoMDFe = resultMunicipioDescarregamentoMDFe.Where(o => o.Documentos.Any(x => x.CTe.Remetente.CPF_CNPJ.Contains(cnpjEmbarcadorUsuario)));
                else
                    resultMunicipioDescarregamentoMDFe = resultMunicipioDescarregamentoMDFe.Where(o => o.Documentos.Any(x => x.CTe.Remetente.CPF_CNPJ.Equals(cnpjEmbarcadorUsuario)));

                result = result.Where(o => resultMunicipioDescarregamentoMDFe.Select(x => x.MDFe.Codigo).Contains(o.MDFe.Codigo));
            }

            if ((!string.IsNullOrWhiteSpace(numeroCarga) && numeroCarga != "0") || (!string.IsNullOrWhiteSpace(numeroUnidade) && numeroUnidade != "0"))
            {
                var queryIntegracaoMDFe = this.SessionNHiBernate.Query<Dominio.Entidades.IntegracaoMDFe>();
                var resultIntegracaoMDFe = from o in queryIntegracaoMDFe select o;
                if (!string.IsNullOrWhiteSpace(numeroCarga) && numeroCarga != "0")
                    resultIntegracaoMDFe = resultIntegracaoMDFe.Where(o => o.NumeroDaCarga == numeroCarga);
                if (!string.IsNullOrWhiteSpace(numeroUnidade) && numeroUnidade != "0")
                    resultIntegracaoMDFe = resultIntegracaoMDFe.Where(o => o.NumeroDaUnidade == numeroUnidade);

                result = result.Where(o => resultIntegracaoMDFe.Select(c => c.MDFe.Codigo).Contains(o.MDFe.Codigo));
            }

            return result.Select(o => new Dominio.ObjetosDeValor.Relatorios.RelatorioMDFesEmitidosAverbacoes()
            {
                CodigoMDFe = o.MDFe.Codigo,
                NomeTransportadora = !tipoSeguradora ? o.MDFe.Empresa.CNPJ + " " + o.MDFe.Empresa.RazaoSocial : o.MDFe.Empresa.CNPJ,
                DataAutorizacao = o.MDFe.DataAutorizacao,
                DataCancelamento = o.MDFe.DataCancelamento,
                DataEmissao = o.MDFe.DataEmissao,
                DataEncerramento = o.MDFe.DataEncerramento,
                DescricaoUFCarregamento = !tipoSeguradora ? o.MDFe.EstadoCarregamento.Nome : (from obj in this.SessionNHiBernate.Query<Dominio.Entidades.MunicipioCarregamentoMDFe>() where obj.MDFe.Codigo == o.Codigo select obj.Municipio.Descricao).FirstOrDefault(),
                UFCarregamento = o.MDFe.EstadoCarregamento.Sigla,
                DescricaoUFDescarregamento = !tipoSeguradora ? o.MDFe.EstadoDescarregamento.Nome : (from obj in this.SessionNHiBernate.Query<Dominio.Entidades.MunicipioDescarregamentoMDFe>() where obj.MDFe.Codigo == o.Codigo select obj.Municipio.Descricao).FirstOrDefault(),
                UFDescarregamento = o.MDFe.EstadoDescarregamento.Sigla,
                Numero = o.MDFe.Numero,
                Chave = o.MDFe.Chave,
                PesoBrutoMercadoria = o.MDFe.PesoBrutoMercadoria,
                Placa = (from obj in this.SessionNHiBernate.Query<Dominio.Entidades.VeiculoMDFe>() where obj.MDFe.Codigo == o.MDFe.Codigo select obj.Placa).FirstOrDefault(),
                MotoristaNome = (from obj in this.SessionNHiBernate.Query<Dominio.Entidades.MotoristaMDFe>() where obj.MDFe.Codigo == o.MDFe.Codigo select obj.Nome).FirstOrDefault(),
                MotoristaCPF = (from obj in this.SessionNHiBernate.Query<Dominio.Entidades.MotoristaMDFe>() where obj.MDFe.Codigo == o.MDFe.Codigo select obj.CPF).FirstOrDefault(),
                Serie = o.MDFe.Serie.Numero,
                UnidadeMedida = o.MDFe.UnidadeMedidaMercadoria,
                ValorTotalMercadoria = o.MDFe.ValorTotalMercadoria,
                Status = o.MDFe.Status,
                ResponsavelSeguro = o.TipoResponsavel,
                CNPJSeguradora = o.CNPJSeguradora,
                NomeSeguradora = o.NomeSeguradora,
                NumeroApolice = o.NumeroApolice,
                NumeroAverbacao = o.NumeroAverbacao
            }).Timeout(120).ToList(); //.OrderBy(o => o.Numero).ThenBy(o => o.Serie)
        }

        public List<Dominio.ObjetosDeValor.Relatorios.DAMDFE> BuscarDAMDFE(int codigoMDFe)
        {
            throw new NotImplementedException();
        }

        public List<Dominio.Entidades.ManifestoEletronicoDeDocumentosFiscais> Consultar(int codigoEmpresa, Dominio.Enumeradores.TipoAmbiente tipoAmbiente, int numeroInicial, int numeroFinal, int inicioRegistros, int maximoRegistros)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.ManifestoEletronicoDeDocumentosFiscais>();
            var result = from obj in query where obj.Empresa.Codigo == codigoEmpresa && obj.TipoAmbiente == tipoAmbiente select obj;

            if (numeroInicial > 0)
                result = result.Where(o => o.Numero >= numeroInicial);

            if (numeroFinal > 0)
                result = result.Where(o => o.Numero <= numeroFinal);

            return result.OrderByDescending(o => o.Numero)
                         .ThenByDescending(o => o.Serie.Numero)
                         .Skip(inicioRegistros)
                         .Take(maximoRegistros)
                         .Fetch(o => o.EstadoCarregamento)
                         .Fetch(o => o.EstadoDescarregamento)
                         .Fetch(o => o.Serie)
                         .Timeout(120)
                         .ToList();
        }

        public int ContarConsulta(int codigoEmpresa, Dominio.Enumeradores.TipoAmbiente tipoAmbiente, int numeroInicial, int numeroFinal)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.ManifestoEletronicoDeDocumentosFiscais>();
            var result = from obj in query where obj.Empresa.Codigo == codigoEmpresa && obj.TipoAmbiente == tipoAmbiente select obj;

            if (numeroInicial > 0)
                result = result.Where(o => o.Numero >= numeroInicial);

            if (numeroFinal > 0)
                result = result.Where(o => o.Numero <= numeroFinal);

            return result.Count();
        }

        public List<int> BuscarListaCodigos(int codigoEmpresa, DateTime dataInicial, DateTime dataFinal, int serie, bool retornarEncerrados)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.ManifestoEletronicoDeDocumentosFiscais>();

            var result = from obj in query
                         where
                             obj.Empresa.Codigo == codigoEmpresa
                         select obj;

            if (retornarEncerrados)
                result = result.Where(o => o.Status == Dominio.Enumeradores.StatusMDFe.Autorizado || o.Status == Dominio.Enumeradores.StatusMDFe.Encerrado || o.Status == Dominio.Enumeradores.StatusMDFe.Cancelado);
            else
                result = result.Where(o => o.Status == Dominio.Enumeradores.StatusMDFe.Autorizado || o.Status == Dominio.Enumeradores.StatusMDFe.Cancelado);

            if (dataInicial.Date > DateTime.MinValue)
                result = result.Where(o => o.DataEmissao >= dataInicial.Date);

            if (dataFinal.Date > DateTime.MinValue)
                result = result.Where(o => o.DataEmissao < dataFinal.AddDays(1).Date);

            if (serie > 0)
                result = result.Where(o => o.Serie.Numero == serie);

            if (dataInicial.Date == DateTime.MinValue && dataFinal.Date == DateTime.MinValue)
            {
                var queryCTesPendentes = this.SessionNHiBernate.Query<Dominio.Entidades.RetornoXMLMDFe>();
                result = result.Where(o => !(from obj in queryCTesPendentes
                                             where obj.MDFe.Empresa.Codigo == codigoEmpresa &&
                                                   obj.Status == "C" ? obj.MDFe.Status == StatusMDFe.Cancelado :
                                                   obj.Status == "E" ? obj.MDFe.Status == StatusMDFe.Encerrado :
                                                   obj.MDFe.Status == StatusMDFe.Autorizado
                                             select obj.MDFe.Codigo).Contains(o.Codigo));
            }

            return result.Select(o => o.Codigo).ToList();
        }

        public List<int> BuscarListaCodigosAutorizacaoOracle(int codigoEmpresa, DateTime dataInicial, DateTime dataFinal)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.ManifestoEletronicoDeDocumentosFiscais>();

            var result = from obj in query where 1 == 1 select obj;

            if (codigoEmpresa > 0)
                result = result.Where(o => o.Empresa.Codigo == codigoEmpresa);

            if (dataInicial != DateTime.MinValue)
                result = result.Where(o => o.DataEmissao >= dataInicial.Date);

            if (dataFinal != DateTime.MinValue)
                result = result.Where(o => o.DataEmissao < dataFinal.Date.AddDays(1));

            result = result.Where(o => o.Status == Dominio.Enumeradores.StatusMDFe.Autorizado ||
                          o.Status == Dominio.Enumeradores.StatusMDFe.Cancelado ||
                          o.Status == Dominio.Enumeradores.StatusMDFe.Encerrado);

            return (from obj in result
                    select obj.Codigo).ToList();
        }

        public List<int> BuscarCodigosParaNotificacao(DateTime dataEmissao, DateTime dataNotificacao)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.ManifestoEletronicoDeDocumentosFiscais>();

            var result = from obj in query
                         where obj.Status == Dominio.Enumeradores.StatusMDFe.Autorizado &&
                               (obj.DataEnvioNotificacao == null || obj.DataEnvioNotificacao <= dataNotificacao) &&
                               obj.DataEmissao <= dataEmissao && obj.TipoAmbiente == Dominio.Enumeradores.TipoAmbiente.Producao
                         select obj.Codigo;

            return result.ToList();
        }

        public List<Dominio.Entidades.ManifestoEletronicoDeDocumentosFiscais> Consultar(int codigoTransportador, int numeroInicial, int numeroFinal, DateTime dataInicial, DateTime dataFinal, Dominio.Enumeradores.StatusMDFe? situacao, string ufCarregamento, string ufDescarregamento, string placaVeiculo, string propriedadeOrdena, string direcaoOrdena, int inicioRegistros, int maximoRegistros)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.ManifestoEletronicoDeDocumentosFiscais>();

            if (codigoTransportador > 0)
                query = query.Where(o => o.Empresa.Codigo == codigoTransportador && o.TipoAmbiente == o.Empresa.TipoAmbiente);

            if (numeroInicial > 0)
                query = query.Where(o => o.Numero >= numeroInicial);

            if (numeroFinal > 0)
                query = query.Where(o => o.Numero <= numeroFinal);

            if (dataInicial != DateTime.MinValue)
                query = query.Where(o => o.DataEmissao >= dataInicial.Date);

            if (dataFinal != DateTime.MinValue)
                query = query.Where(o => o.DataEmissao < dataFinal.AddDays(1).Date);

            if (situacao != null && situacao.Value != Dominio.Enumeradores.StatusMDFe.Todos)
                query = query.Where(o => o.Status == situacao.Value);

            if (!string.IsNullOrWhiteSpace(ufCarregamento) && ufCarregamento != "0")
                query = query.Where(o => o.EstadoCarregamento.Sigla.Equals(ufCarregamento));

            if (!string.IsNullOrWhiteSpace(ufDescarregamento) && ufDescarregamento != "0")
                query = query.Where(o => o.EstadoDescarregamento.Sigla.Equals(ufDescarregamento));

            if (!string.IsNullOrWhiteSpace(placaVeiculo))
            {
                var queryVeiculos = this.SessionNHiBernate.Query<Dominio.Entidades.VeiculoMDFe>();

                var resultVeiculos = from obj in queryVeiculos where obj.Placa.Equals(placaVeiculo) select obj.MDFe.Codigo;

                query = query.Where(o => resultVeiculos.Contains(o.Codigo));
            }

            return query.OrderBy(propriedadeOrdena + " " + direcaoOrdena)
                        .Skip(inicioRegistros)
                        .Take(maximoRegistros)
                        .Fetch(o => o.EstadoCarregamento)
                        .Fetch(o => o.EstadoDescarregamento)
                        .Fetch(o => o.Serie)
                        .Fetch(o => o.MensagemStatus)
                        .Timeout(120)
                        .ToList();
        }

        public int ContarConsulta(int codigoTransportador, int numeroInicial, int numeroFinal, DateTime dataInicial, DateTime dataFinal, Dominio.Enumeradores.StatusMDFe? situacao, string ufCarregamento, string ufDescarregamento, string placaVeiculo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.ManifestoEletronicoDeDocumentosFiscais>();

            if (codigoTransportador > 0)
                query = query.Where(o => o.Empresa.Codigo == codigoTransportador && o.TipoAmbiente == o.Empresa.TipoAmbiente);

            if (numeroInicial > 0)
                query = query.Where(o => o.Numero >= numeroInicial);

            if (numeroFinal > 0)
                query = query.Where(o => o.Numero <= numeroFinal);

            if (dataInicial != DateTime.MinValue)
                query = query.Where(o => o.DataEmissao >= dataInicial.Date);

            if (dataFinal != DateTime.MinValue)
                query = query.Where(o => o.DataEmissao < dataFinal.AddDays(1).Date);

            if (situacao != null && situacao.Value != Dominio.Enumeradores.StatusMDFe.Todos)
                query = query.Where(o => o.Status == situacao.Value);

            if (!string.IsNullOrWhiteSpace(ufCarregamento) && ufCarregamento != "0")
                query = query.Where(o => o.EstadoCarregamento.Sigla.Equals(ufCarregamento));

            if (!string.IsNullOrWhiteSpace(ufDescarregamento) && ufDescarregamento != "0")
                query = query.Where(o => o.EstadoDescarregamento.Sigla.Equals(ufDescarregamento));

            if (!string.IsNullOrWhiteSpace(placaVeiculo))
            {
                var queryVeiculos = this.SessionNHiBernate.Query<Dominio.Entidades.VeiculoMDFe>();

                var resultVeiculos = from obj in queryVeiculos where obj.Placa.Equals(placaVeiculo) select obj.MDFe.Codigo;

                query = query.Where(o => resultVeiculos.Contains(o.Codigo));
            }

            return query.Timeout(120)
                        .Count();
        }

        public int RelatorioCobrancaQuantidadeMDFes(int codigoEmpresa, DateTime dataInicial, DateTime dataFinal)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.ManifestoEletronicoDeDocumentosFiscais>();

            var result = from obj in query
                         where
                         obj.Empresa.Codigo == codigoEmpresa &&
                         (obj.Status == Dominio.Enumeradores.StatusMDFe.Autorizado ||
                          obj.Status == Dominio.Enumeradores.StatusMDFe.Cancelado ||
                          obj.Status == Dominio.Enumeradores.StatusMDFe.Encerrado)
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

        public IList<Dominio.Relatorios.Embarcador.DataSource.MDFe.Mdfe> ConsultarRelatorio(Dominio.ObjetosDeValor.Embarcador.MDFe.FiltroPesquisaMdfeRelatorio filtrosPesquisa, List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedades, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            var consultaMdfe = new ConsultaManifestoEletronicoDeDocumentosFiscais().ObterSqlPesquisa(filtrosPesquisa, parametrosConsulta, propriedades).CriarSQLQuery(this.SessionNHiBernate);

            consultaMdfe.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Dominio.Relatorios.Embarcador.DataSource.MDFe.Mdfe)));

            return consultaMdfe.SetTimeout(600).List<Dominio.Relatorios.Embarcador.DataSource.MDFe.Mdfe>();
        }

        public int ContarConsultaRelatorio(Dominio.ObjetosDeValor.Embarcador.MDFe.FiltroPesquisaMdfeRelatorio filtrosPesquisa, List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedades)
        {
            var consultaMdfe = new ConsultaManifestoEletronicoDeDocumentosFiscais().ObterSqlContarPesquisa(filtrosPesquisa, propriedades).CriarSQLQuery(this.SessionNHiBernate);

            return consultaMdfe.SetTimeout(600).UniqueResult<int>();
        }

        public IList<Dominio.Relatorios.Embarcador.DataSource.MDFe.MdfeConhecimentos> ConsultarConhecimentosDeFreteRelatorioMDFes(Dominio.ObjetosDeValor.Embarcador.MDFe.FiltroPesquisaMdfeRelatorio filtrosPesquisa)
        {
            var consultaMDFeConhecimentos = this.SessionNHiBernate.CreateSQLQuery(new ConsultaManifestoEletronicoDeDocumentosFiscais().ObterSqlPesquisaConhecimentosDeFrete(filtrosPesquisa));

            consultaMDFeConhecimentos.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Dominio.Relatorios.Embarcador.DataSource.MDFe.MdfeConhecimentos)));

            return consultaMDFeConhecimentos.List<Dominio.Relatorios.Embarcador.DataSource.MDFe.MdfeConhecimentos>();
        }

        public IList<Dominio.ObjetosDeValor.Embarcador.MDFe.ConsultaMDFeGeracaoCargaEmbarcador> ConsultarMDFesDisponiveisParaGerarCarga(Dominio.ObjetosDeValor.Embarcador.Carga.FiltroPesquisaMDFesDisponiveisGeracaoCargaEmbarcador filtro, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            var query = ObterSQLMDFesDisponiveisParaGerarCarga(filtro, parametrosConsulta);

            NHibernate.ISQLQuery sqlQuery = query.CriarSQLQuery(this.SessionNHiBernate);

            sqlQuery.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Dominio.ObjetosDeValor.Embarcador.MDFe.ConsultaMDFeGeracaoCargaEmbarcador)));

            return sqlQuery.SetTimeout(3000).List<Dominio.ObjetosDeValor.Embarcador.MDFe.ConsultaMDFeGeracaoCargaEmbarcador>();
        }

        public int ContarConsultaMDFesDisponiveisParaGerarCarga(Dominio.ObjetosDeValor.Embarcador.Carga.FiltroPesquisaMDFesDisponiveisGeracaoCargaEmbarcador filtro)
        {
            var query = ObterSQLMDFesDisponiveisParaGerarCarga(filtro, null);

            NHibernate.ISQLQuery sqlQuery = query.CriarSQLQuery(this.SessionNHiBernate);

            return sqlQuery.UniqueResult<int>();
        }

        public List<Dominio.Entidades.ManifestoEletronicoDeDocumentosFiscais> BuscarPorCodigo(List<int> codigos)
        {
            IQueryable<Dominio.Entidades.ManifestoEletronicoDeDocumentosFiscais> query = this.SessionNHiBernate.Query<Dominio.Entidades.ManifestoEletronicoDeDocumentosFiscais>();

            query = query.Where(o => codigos.Contains(o.Codigo));

            return query.ToList();
        }

        public List<Dominio.Entidades.ManifestoEletronicoDeDocumentosFiscais> BuscarPorNumeroPedido(string numeroPedido)
        {
            List<Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga> situacoesCargaNaoPermitidas = new List<Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga>()
            {
                 Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.Cancelada,
                 Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.Anulada
            };

            IQueryable<Dominio.Entidades.ManifestoEletronicoDeDocumentosFiscais> query = this.SessionNHiBernate.Query<Dominio.Entidades.ManifestoEletronicoDeDocumentosFiscais>();

            IQueryable<Dominio.Entidades.Embarcador.Cargas.CargaMDFe> queryCargaMDFe = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaMDFe>();
            IQueryable<Dominio.Entidades.Embarcador.Cargas.CargaPedidoDocumentoMDFe> queryCargaPedidoDocumentoMDFe = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedidoDocumentoMDFe>();

            IQueryable<Dominio.Entidades.Embarcador.Cargas.CargaCTe> queryCargaCTe = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCTe>();
            IQueryable<Dominio.Entidades.Embarcador.Cargas.CargaPedidoDocumentoCTe> queryCargaPedidoDocumentoCTe = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedidoDocumentoCTe>();

            queryCargaPedidoDocumentoMDFe = queryCargaPedidoDocumentoMDFe.Where(o => !situacoesCargaNaoPermitidas.Contains(o.CargaPedido.Carga.SituacaoCarga));
            queryCargaPedidoDocumentoCTe = queryCargaPedidoDocumentoCTe.Where(o => !situacoesCargaNaoPermitidas.Contains(o.CargaPedido.Carga.SituacaoCarga));

            queryCargaMDFe = queryCargaMDFe.Where(o => !situacoesCargaNaoPermitidas.Contains(o.Carga.SituacaoCarga));
            queryCargaCTe = queryCargaCTe.Where(o => !situacoesCargaNaoPermitidas.Contains(o.Carga.SituacaoCarga));

            query = query.Where(o => !o.MunicipiosDescarregamento.Any(mun => mun.Documentos.Any(doc => queryCargaCTe.Any(cct => cct.CTe == doc.CTe) || queryCargaPedidoDocumentoCTe.Any(cpd => cpd.CTe == doc.CTe))) &&
                                     !queryCargaMDFe.Any(cmd => cmd.MDFe == o) &&
                                     !queryCargaPedidoDocumentoMDFe.Any(cmd => cmd.MDFe == o) &&
                                     (o.Status == StatusMDFe.Autorizado || o.Status == StatusMDFe.Encerrado) &&
                                     o.NumeroPedido == numeroPedido);

            return query.ToList();
        }

        public IList<Dominio.ObjetosDeValor.Embarcador.MDFe.CamposPesquisaEncerramentoMDFeTMS> ConsultarMDFesEncerramentoTMS(Dominio.ObjetosDeValor.Embarcador.MDFe.FiltroPesquisaEncerramentoMDFeTMS filtrosPesquisa, int inicioRegistros, int maximoRegistros)
        {
            string sqlQuery = @"select
                                MDFe.MDF_CODIGO Codigo,
                                MDFeVeiculo.MDV_PLACA Veiculo, 
                                MDFe.MDF_NUMERO Numero,
                                MDFeSerie.ESE_NUMERO Serie,
                                MDFe.MDF_DATA_EMISSAO DataEmissao,
                                SUBSTRING((SELECT ', ' + Localidade.LOC_DESCRICAO + ' - ' + Localidade.UF_SIGLA FROM T_LOCALIDADES Localidade INNER JOIN T_MDFE_MUNICIPIO_CARREGAMENTO MDFeMunicipioCarregamento ON MDFeMunicipioCarregamento.LOC_CODIGO = Localidade.LOC_CODIGO WHERE MDFeMunicipioCarregamento.MDF_CODIGO = MDFe.MDF_CODIGO FOR XML PATH('')), 3, 1000) LocalidadeOrigem,
                                SUBSTRING((SELECT ', ' + Localidade.LOC_DESCRICAO + ' - ' + Localidade.UF_SIGLA FROM T_LOCALIDADES Localidade INNER JOIN T_MDFE_MUNICIPIO_DESCARREGAMENTO MDFeMunicipioDescarregamento ON MDFeMunicipioDescarregamento.LOC_CODIGO = Localidade.LOC_CODIGO WHERE MDFeMunicipioDescarregamento.MDF_CODIGO = MDFe.MDF_CODIGO FOR XML PATH('')), 3, 1000) LocalidadeDestino,
                                SUBSTRING((SELECT ', ' + MDFeMotorista.MDM_NOME FROM T_MDFE_MOTORISTA MDFeMotorista WHERE MDFeMotorista.MDF_CODIGO = MDFe.MDF_CODIGO FOR XML PATH('')), 3, 1000) Motorista,
                                SUBSTRING((SELECT ', ' + Carga.CAR_CODIGO_CARGA_EMBARCADOR FROM T_CARGA_MDFE CargaMDFe INNER JOIN T_CARGA Carga ON Carga.CAR_CODIGO = CargaMDFe.CAR_CODIGO WHERE CargaMDFe.MDF_CODIGO = MDFe.MDF_CODIGO AND Carga.CAR_SITUACAO NOT IN (13, 18) FOR XML PATH('')), 3, 1000) Carga,
                                (select MAX(m.MDF_DATA_EMISSAO) from T_MDFE m inner join T_MDFE_VEICULO v on m.MDF_CODIGO = v.MDF_CODIGO WHERE m.MDF_STATUS = 3 AND v.MDV_PLACA = MDFeVeiculo.MDV_PLACA) MaiorDataEmissao
                                from T_MDFE MDFe
                                inner join T_MDFE_VEICULO MDFeVeiculo on MDFe.MDF_CODIGO = MDFeVeiculo.MDF_CODIGO
                                inner join T_EMPRESA_SERIE MDFeSerie on MDFeSerie.ESE_CODIGO = MDFe.ESE_CODIGO
                                WHERE MDFe.MDF_STATUS = 3 AND
                                exists ( select MDFeVeiculo2.MDV_PLACA from T_MDFE MDFe2
                                                           inner join T_MDFE_VEICULO MDFeVeiculo2 on MDFe2.MDF_CODIGO = MDFeVeiculo2.MDF_CODIGO and MDFeVeiculo2.MDV_PLACA = MDFeVeiculo.MDV_PLACA
                                                           WHERE MDFe2.MDF_STATUS = 3
                                                           GROUP BY MDFeVeiculo2.MDV_PLACA
                                                           HAVING COUNT(MDFe2.MDF_CODIGO) > 1
						                                  ) ";

            if (!string.IsNullOrWhiteSpace(filtrosPesquisa.Placa))
                sqlQuery += $" AND MDFeVeiculo.MDV_PLACA = '{filtrosPesquisa.Placa}'";

            sqlQuery += $" ORDER BY MaiorDataEmissao desc, MDFeVeiculo.MDV_PLACA asc, MDFe.MDF_DATA_EMISSAO desc OFFSET {inicioRegistros} ROWS FETCH NEXT {maximoRegistros} ROWS ONLY";

            NHibernate.ISQLQuery query = this.SessionNHiBernate.CreateSQLQuery(sqlQuery);

            query.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Dominio.ObjetosDeValor.Embarcador.MDFe.CamposPesquisaEncerramentoMDFeTMS)));

            return query.SetTimeout(90000).List<Dominio.ObjetosDeValor.Embarcador.MDFe.CamposPesquisaEncerramentoMDFeTMS>();
        }

        public int ContarConsultaMDFesEncerramentoTMS(Dominio.ObjetosDeValor.Embarcador.MDFe.FiltroPesquisaEncerramentoMDFeTMS filtrosPesquisa)
        {
            string sqlQuery = @"select distinct(count(0) over ())
                                from T_MDFE MDFe
                                inner join T_MDFE_VEICULO MDFeVeiculo on MDFe.MDF_CODIGO = MDFeVeiculo.MDF_CODIGO
                                inner join T_EMPRESA_SERIE MDFeSerie on MDFeSerie.ESE_CODIGO = MDFe.ESE_CODIGO
                                WHERE MDFe.MDF_STATUS = 3 AND
                                exists ( select MDFeVeiculo2.MDV_PLACA from T_MDFE MDFe2
                                                           inner join T_MDFE_VEICULO MDFeVeiculo2 on MDFe2.MDF_CODIGO = MDFeVeiculo2.MDF_CODIGO and MDFeVeiculo2.MDV_PLACA = MDFeVeiculo.MDV_PLACA
                                                           WHERE MDFe2.MDF_STATUS = 3
                                                           GROUP BY MDFeVeiculo2.MDV_PLACA
                                                           HAVING COUNT(MDFe2.MDF_CODIGO) > 1
						                                  )";

            if (!string.IsNullOrWhiteSpace(filtrosPesquisa.Placa))
                sqlQuery += $" AND MDFeVeiculo.MDV_PLACA = '{filtrosPesquisa.Placa}'";

            NHibernate.ISQLQuery query = this.SessionNHiBernate.CreateSQLQuery(sqlQuery);

            return query.SetTimeout(90000).UniqueResult<int>();
        }

        public List<int> BuscarMDFesAutorizadosParaConsultaSituacao(int quantidadeRegistros)
        {
            IQueryable<Dominio.Entidades.ManifestoEletronicoDeDocumentosFiscais> query = this.SessionNHiBernate.Query<Dominio.Entidades.ManifestoEletronicoDeDocumentosFiscais>();

            //consulta apenas MDF-es com data de autorização até 180 dias atrás (regra SEFAZ) e autorizados há pelo menos 5 horas
            query = query.Where(o => o.DataAutorizacao >= DateTime.Now.AddDays(-180).Date && o.DataAutorizacao <= DateTime.Now.AddHours(-5) && o.Status == StatusMDFe.Autorizado);

            return query.OrderBy(o => o.DataUltimaConsultaSituacaoMDFe).ThenBy(o => o.DataAutorizacao).Select(o => o.Codigo).Take(quantidadeRegistros).ToList();
        }

        public Dominio.Entidades.ConhecimentoDeTransporteEletronico BuscarPrimeiroCTePorMDFe(int codigoMDFe)
        {
            IQueryable<Dominio.Entidades.DocumentoMunicipioDescarregamentoMDFe> query = SessionNHiBernate.Query<Dominio.Entidades.DocumentoMunicipioDescarregamentoMDFe>();

            query = query.Where(o => o.MunicipioDescarregamento.MDFe.Codigo == codigoMDFe && o.CTe != null);

            return query
                .Select(o => o.CTe)
                .Fetch(o => o.Remetente)
                .Fetch(o => o.Destinatario)
                .Fetch(o => o.Recebedor)
                .Fetch(o => o.Expedidor)
                .Fetch(o => o.OutrosTomador)
                .FirstOrDefault();
        }

        public List<int> BuscarCodigosCTePorMDFe(params int[] codigoMDFe)
        {
            IQueryable<Dominio.Entidades.DocumentoMunicipioDescarregamentoMDFe> query = SessionNHiBernate.Query<Dominio.Entidades.DocumentoMunicipioDescarregamentoMDFe>();

            query = query.Where(o => codigoMDFe.Contains(o.MunicipioDescarregamento.MDFe.Codigo) && o.CTe != null);

            return query
                .Select(o => o.CTe.Codigo)
                .ToList();
        }

        public decimal BuscarValorTotalFrete(int codigoMDFe)
        {
            string queryString = @"
                select SUM(C.CON_VALOR_RECEBER) 
                from T_MDFE_MUNICIPIO_DESCARREGAMENTO_DOC X
                join T_CTE C ON C.CON_CODIGO = X.CON_CODIGO 
                join T_MDFE_MUNICIPIO_DESCARREGAMENTO M ON M.MDD_CODIGO = X.MDD_CODIGO
                where M.MDF_CODIGO = :codigoMDFe";

            NHibernate.ISQLQuery query = SessionNHiBernate.CreateSQLQuery(queryString);
            query.SetParameter("codigoMDFe", codigoMDFe);

            return query.UniqueResult<decimal?>() ?? 0;
        }

        #endregion

        #region Métodos Privados

        private SQLDinamico ObterSQLMDFesDisponiveisParaGerarCarga(Dominio.ObjetosDeValor.Embarcador.Carga.FiltroPesquisaMDFesDisponiveisGeracaoCargaEmbarcador filtro, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            string sqlQuery = @"SELECT ";
            var parametros = new List<ParametroSQL>();

            if (parametrosConsulta == null)
                sqlQuery += "COUNT(MDFe.MDF_CODIGO) ";
            else
            {
                sqlQuery += @"MDFe.MDF_CODIGO Codigo,
                              MDFe.MDF_NUMERO Numero,
                              SerieMDFe.ESE_NUMERO Serie,
                              MDFe.MDF_DATA_EMISSAO DataEmissao,
                              Empresa.EMP_RAZAO + ' (' + LocalidadeEmpresa.LOC_DESCRICAO +' - ' + LocalidadeEmpresa.UF_SIGLA + ')' Empresa,
                              MDFe.UF_CARREGAMENTO UFOrigem,
                              MDFe.UF_DESCARREGAMENTO UFDestino,
                              SUBSTRING((SELECT DISTINCT ', ' + MunicipioCarregamento.LOC_DESCRICAO FROM T_MDFE_MUNICIPIO_CARREGAMENTO MunicipioCarregamentoMDFe INNER JOIN T_LOCALIDADES MunicipioCarregamento ON MunicipioCarregamentoMDFe.LOC_CODIGO = MunicipioCarregamento.LOC_CODIGO WHERE MunicipioCarregamentoMDFe.MDF_CODIGO = MDFe.MDF_CODIGO for xml path('')), 3, 200) Origem,
                              SUBSTRING((SELECT DISTINCT ', ' + MunicipioDescarregamento.LOC_DESCRICAO FROM T_MDFE_MUNICIPIO_DESCARREGAMENTO MunicipioDescarregamentoMDFe INNER JOIN T_LOCALIDADES MunicipioDescarregamento ON MunicipioDescarregamentoMDFe.LOC_CODIGO = MunicipioDescarregamento.LOC_CODIGO WHERE MunicipioDescarregamentoMDFe.MDF_CODIGO = MDFe.MDF_CODIGO for xml path('')), 3, 200) Destino,
                              (SELECT TOP 1 MDFeVeiculo.MDV_PLACA FROM T_MDFE_VEICULO MDFeVeiculo WHERE MDFeVeiculo.MDF_CODIGO = MDFe.MDF_CODIGO) + ISNULL((SELECT DISTINCT ', ' +ReboqueMDFe.MDR_PLACA from T_MDFE_REBOQUE ReboqueMDFe where ReboqueMDFe.MDF_CODIGO = MDFe.MDF_CODIGO for xml path('')), '') Veiculos,
                              (SELECT TOP 1 Segmento.VSE_DESCRICAO FROM T_MDFE_VEICULO MDFeVeiculo INNER JOIN T_VEICULO Veiculo ON Veiculo.VEI_PLACA = MDFeVeiculo.MDV_PLACA INNER JOIN T_VEICULO_SEGMENTO Segmento ON Veiculo.VSE_CODIGO = Segmento.VSE_CODIGO WHERE MDFeVeiculo.MDF_CODIGO = MDFe.MDF_CODIGO) Segmento ";
            }

            sqlQuery += @"FROM T_MDFE MDFe
                          inner join T_EMPRESA_SERIE SerieMDFe on MDFe.ESE_CODIGO = SerieMDFe.ESE_CODIGO
                          left join T_EMPRESA Empresa ON Empresa.EMP_CODIGO = MDFe.EMP_CODIGO
                          left join T_LOCALIDADES LocalidadeEmpresa on Empresa.LOC_CODIGO = LocalidadeEmpresa.LOC_CODIGO 
                          WHERE 
                          MDFe.MDF_STATUS IN (3, 5) AND
                          NOT EXISTS (SELECT CargaMDFe.MDF_CODIGO FROM T_CARGA_MDFE CargaMDFe INNER JOIN T_CARGA Carga ON Carga.CAR_CODIGO = CargaMDFe.CAR_CODIGO WHERE CargaMDFe.MDF_CODIGO = MDFe.MDF_CODIGO AND Carga.CAR_SITUACAO NOT IN (13, 18)) AND 
                          NOT EXISTS (SELECT MDF_CODIGO FROM T_CARGA_PEDIDO_DOCUMENTO_MDFE CargaPedidoDocumentoMDFe INNER JOIN T_CARGA_PEDIDO CargaPedido ON CargaPedido.CPE_CODIGO = CargaPedidoDocumentoMDFe.CPE_CODIGO INNER JOIN T_CARGA Carga ON Carga.CAR_CODIGO = CargaPedido.CAR_CODIGO WHERE CargaPedidoDocumentoMDFe.MDF_CODIGO = MDFe.MDF_CODIGO AND Carga.CAR_SITUACAO NOT IN (13, 18)) AND
                          NOT EXISTS (SELECT CargaIntegracaoEmbarcadorMDFe.MDF_CODIGO FROM T_CARGA_INTEGRACAO_EMBARCADOR_MDFE CargaIntegracaoEmbarcadorMDFe INNER JOIN T_CARGA_INTEGRACAO_EMBARCADOR CargaIntegracaoEmbarcador on CargaIntegracaoEmbarcadorMDFe.CIE_CODIGO = CargaIntegracaoEmbarcador.CIE_CODIGO WHERE CargaIntegracaoEmbarcador.CIE_SITUACAO <> 6 AND CargaIntegracaoEmbarcadorMDFe.MDF_CODIGO = 348)";

            if (filtro.NaoGerarCargaAutomaticamente.HasValue)
            {
                if (filtro.NaoGerarCargaAutomaticamente.Value)
                    sqlQuery += " AND SerieMDFe.ESE_NAO_GERAR_CARGA_AUTOMATICAMENTE = 1";
                else
                    sqlQuery += " AND (SerieMDFe.ESE_NAO_GERAR_CARGA_AUTOMATICAMENTE is null or SerieMDFe.ESE_NAO_GERAR_CARGA_AUTOMATICAMENTE = 0)";
            }

            if (filtro.ProblemaGeracaoCargaAutomaticamente.HasValue)
            {
                if (filtro.ProblemaGeracaoCargaAutomaticamente.Value)
                    sqlQuery += " AND MDFe.MDF_PROBLEMA_GERACAO_CARGA_AUTOMATICAMENTE = 1";
                else
                    sqlQuery += " AND (MDFe.MDF_PROBLEMA_GERACAO_CARGA_AUTOMATICAMENTE is null or MDFe.MDF_PROBLEMA_GERACAO_CARGA_AUTOMATICAMENTE = 0)";
            }

            if (filtro.CodigoLocalidadeDestino > 0)
                sqlQuery += $" AND EXISTS (SELECT MDF_CODIGO FROM T_MDFE_MUNICIPIO_DESCARREGAMENTO WHERE LOC_CODIGO = {filtro.CodigoLocalidadeDestino} AND MDF_CODIGO = MDFe.MDF_CODIGO)";

            if (filtro.CodigoLocalidadeOrigem > 0)
                sqlQuery += $" AND EXISTS (SELECT MDF_CODIGO FROM T_MDFE_MUNICIPIO_CARREGAMENTO WHERE LOC_CODIGO = {filtro.CodigoLocalidadeOrigem} AND MDF_CODIGO = MDFe.MDF_CODIGO)";

            if (!string.IsNullOrWhiteSpace(filtro.NomeMotorista))
            {
                sqlQuery += $" AND EXISTS (SELECT MDF_CODIGO FROM T_MDFE_MOTORISTA WHERE MDM_NOME LIKE :MDM_NOME) ";
                parametros.Add(new ParametroSQL("MDM_NOME", $"%{filtro.NomeMotorista}%"));
            }

            if (!string.IsNullOrWhiteSpace(filtro.PlacaVeiculo))
            {
                sqlQuery += $" AND (EXISTS (SELECT MDF_CODIGO FROM T_MDFE_VEICULO WHERE MDV_PLACA LIKE :MDV_PLACA AND MDFe.MDF_CODIGO = MDF_CODIGO) OR EXISTS (SELECT MDF_CODIGO FROM T_MDFE_REBOQUE WHERE MDR_PLACA LIKE :MDR_PLACA AND MDFe.MDF_CODIGO = MDF_CODIGO))";
                parametros.Add(new ParametroSQL("MDV_PLACA", $"%{filtro.PlacaVeiculo}%"));
                parametros.Add(new ParametroSQL("MDR_PLACA", $"%{filtro.PlacaVeiculo}%"));

            }

            if (!string.IsNullOrWhiteSpace(filtro.UFDestino) && filtro.UFDestino != "0")
            {
                sqlQuery += $" AND MDFe.UF_DESCARREGAMENTO = :MDFe_UF_DESCARREGAMENTO";
                parametros.Add(new ParametroSQL("MDFe_UF_DESCARREGAMENTO", filtro.UFDestino));
            }

            if (!string.IsNullOrWhiteSpace(filtro.UFOrigem) && filtro.UFOrigem != "0")
            {
                sqlQuery += $" AND MDFe.UF_CARREGAMENTO = :MDFe_UF_CARREGAMENTO";
                parametros.Add(new ParametroSQL("MDFe_UF_CARREGAMENTO", filtro.UFOrigem));
            }

            if (filtro.NumeroMDFe > 0)
                sqlQuery += $" AND MDFe.MDF_NUMERO = {filtro.NumeroMDFe}";

            if (filtro.CodigoEmpresa > 0)
                sqlQuery += $" AND MDFe.EMP_CODIGO = {filtro.CodigoEmpresa}";

            if (filtro.Serie > 0)
                sqlQuery += $" AND SerieMDFe.ESE_NUMERO = {filtro.Serie}";

            if (filtro.DataEmissaoInicial.HasValue)
                sqlQuery += $" AND MDFe.MDF_DATA_EMISSAO >= '{filtro.DataEmissaoInicial.Value.Date.ToString("yyyy-MM-dd")}'";

            if (filtro.DataEmissaoFinal.HasValue)
                sqlQuery += $" AND MDFe.MDF_DATA_EMISSAO < '{filtro.DataEmissaoFinal.Value.AddDays(1).Date.ToString("yyyy-MM-dd")}'";

            if (parametrosConsulta != null)
                sqlQuery += $" ORDER BY {parametrosConsulta.PropriedadeOrdenar} {parametrosConsulta.DirecaoOrdenar} OFFSET {parametrosConsulta.InicioRegistros} ROWS FETCH FIRST {parametrosConsulta.LimiteRegistros} ROWS ONLY;";

            return new SQLDinamico(sqlQuery, parametros);
        }

        private IQueryable<Dominio.Entidades.ManifestoEletronicoDeDocumentosFiscais> _Consultar(int codigoEmpresa, int codigoMDFe, Dominio.Enumeradores.TipoAmbiente ambiente, int codigoSerie, int numeroInicial, int numeroFinal, DateTime dataInicial, DateTime dataFinal, Dominio.Enumeradores.StatusMDFe? status, string ufCarregamento, string ufDescarregamento, string placaVeiculo, string placaReboque, string cpfMotorista, string nomeMotorista, int[] series, int numeroCTe, string nomeUsuario = "", string numeroCarga = "", string numeroUnidade = "", int empresaPai = 0)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.ManifestoEletronicoDeDocumentosFiscais>();
            var result = from obj in query where obj.TipoAmbiente == ambiente select obj;

            if (codigoMDFe > 0)
                result = result.Where(o => o.Codigo == codigoMDFe);
            else
            {
                if (codigoEmpresa > 0)
                    result = result.Where(o => o.Empresa.Codigo == codigoEmpresa);

                if (empresaPai > 0)
                    result = result.Where(o => o.Empresa.EmpresaPai.Codigo == empresaPai);

                if (codigoSerie > 0)
                    result = result.Where(o => o.Serie.Codigo == codigoSerie);
                else if (series != null && series.Count() > 0)
                    result = result.Where(o => series.Contains(o.Serie.Codigo));

                if (numeroInicial > 0)
                    result = result.Where(o => o.Numero >= numeroInicial);

                if (numeroFinal > 0)
                    result = result.Where(o => o.Numero <= numeroFinal);

                if (dataInicial != DateTime.MinValue)
                    result = result.Where(o => o.DataEmissao >= dataInicial.Date);

                if (dataFinal != DateTime.MinValue)
                    result = result.Where(o => o.DataEmissao < dataFinal.AddDays(1).Date);

                if (status != null && status.Value != Dominio.Enumeradores.StatusMDFe.Todos)
                    result = result.Where(o => o.Status == status.Value);

                if (!string.IsNullOrWhiteSpace(ufCarregamento))
                    result = result.Where(o => o.EstadoCarregamento.Sigla.Equals(ufCarregamento));

                if (!string.IsNullOrWhiteSpace(ufDescarregamento))
                    result = result.Where(o => o.EstadoDescarregamento.Sigla.Equals(ufDescarregamento));

                if (!string.IsNullOrWhiteSpace(nomeUsuario))
                    result = result.Where(o => o.Log.Contains(nomeUsuario));

                if (!string.IsNullOrWhiteSpace(placaVeiculo))
                {
                    var queryVeiculos = this.SessionNHiBernate.Query<Dominio.Entidades.VeiculoMDFe>();

                    var resultVeiculos = from obj in queryVeiculos where obj.Placa.Equals(placaVeiculo) select obj.MDFe.Codigo;

                    result = result.Where(o => resultVeiculos.Contains(o.Codigo));
                }

                if (!string.IsNullOrWhiteSpace(placaReboque))
                {
                    var queryVeiculos = this.SessionNHiBernate.Query<Dominio.Entidades.ReboqueMDFe>();

                    var resultVeiculos = from obj in queryVeiculos where obj.Placa.Equals(placaReboque) select obj.MDFe.Codigo;

                    result = result.Where(o => resultVeiculos.Contains(o.Codigo));
                }

                if (!string.IsNullOrWhiteSpace(cpfMotorista) || !string.IsNullOrWhiteSpace(nomeMotorista))
                {
                    var queryMotoristas = this.SessionNHiBernate.Query<Dominio.Entidades.MotoristaMDFe>();

                    var resultMotoristas = from obj in queryMotoristas select obj;

                    if (!string.IsNullOrWhiteSpace(cpfMotorista))
                        resultMotoristas = resultMotoristas.Where(o => o.CPF.Equals(cpfMotorista));

                    if (!string.IsNullOrWhiteSpace(nomeMotorista))
                        resultMotoristas = resultMotoristas.Where(o => o.Nome.Equals(nomeMotorista));

                    result = result.Where(o => resultMotoristas.Select(x => x.MDFe.Codigo).Contains(o.Codigo));
                }

                if ((!string.IsNullOrWhiteSpace(numeroCarga) && numeroCarga != "0") || (!string.IsNullOrWhiteSpace(numeroUnidade) && numeroUnidade != "0"))
                {
                    var queryIntegracaoMDFe = this.SessionNHiBernate.Query<Dominio.Entidades.IntegracaoMDFe>();
                    var resultIntegracaoMDFe = from o in queryIntegracaoMDFe select o;
                    if (!string.IsNullOrWhiteSpace(numeroCarga) && numeroCarga != "0")
                        resultIntegracaoMDFe = resultIntegracaoMDFe.Where(o => o.NumeroDaCarga == numeroCarga);
                    if (!string.IsNullOrWhiteSpace(numeroUnidade) && numeroUnidade != "0")
                        resultIntegracaoMDFe = resultIntegracaoMDFe.Where(o => o.NumeroDaUnidade == numeroUnidade);

                    result = result.Where(o => resultIntegracaoMDFe.Select(c => c.MDFe.Codigo).Contains(o.Codigo));
                }

                if (numeroCTe > 0)
                {
                    var queryCTes = this.SessionNHiBernate.Query<Dominio.Entidades.MunicipioDescarregamentoMDFe>();
                    var resultCTes = from obj in queryCTes where obj.Documentos.Any(o => o.CTe.Numero == numeroCTe) select obj.MDFe.Codigo;

                    result = result.Where(o => resultCTes.Contains(o.Codigo));
                }
            }

            return result;
        }

        private IQueryable<Dominio.Entidades.ManifestoEletronicoDeDocumentosFiscais> MontarConsulta(Dominio.ObjetosDeValor.Embarcador.MDFe.FiltroConsultaMDFe filtrosPesquisa)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.ManifestoEletronicoDeDocumentosFiscais>();

            if (filtrosPesquisa.CodigoEmpresa > 0)
                query = query.Where(o => o.Empresa.Codigo == filtrosPesquisa.CodigoEmpresa && o.TipoAmbiente == o.Empresa.TipoAmbiente);

            if (filtrosPesquisa.DataEmissaoInicial != DateTime.MinValue)
                query = query.Where(o => o.DataEmissao >= filtrosPesquisa.DataEmissaoInicial.Date);

            if (filtrosPesquisa.DataEmissaoFinal != DateTime.MinValue)
                query = query.Where(o => o.DataEmissao < filtrosPesquisa.DataEmissaoFinal.AddDays(1).Date);

            if (filtrosPesquisa.NumeroInicial > 0)
                query = query.Where(o => o.Numero >= filtrosPesquisa.NumeroInicial);

            if (filtrosPesquisa.NumeroFinal > 0)
                query = query.Where(o => o.Numero <= filtrosPesquisa.NumeroFinal);

            if (!string.IsNullOrWhiteSpace(filtrosPesquisa.EstadoOrigem) && filtrosPesquisa.EstadoOrigem != "0")
                query = query.Where(o => o.EstadoCarregamento.Sigla == filtrosPesquisa.EstadoOrigem);

            if (!string.IsNullOrWhiteSpace(filtrosPesquisa.EstadoDestino) && filtrosPesquisa.EstadoDestino != "0")
                query = query.Where(o => o.EstadoDescarregamento.Sigla == filtrosPesquisa.EstadoDestino);

            if (!string.IsNullOrWhiteSpace(filtrosPesquisa.ChaveCTe))
                query = query.Where(o => o.MunicipiosDescarregamento.Any(m => m.Documentos.Any(d => d.Chave == filtrosPesquisa.ChaveCTe)));

            if (filtrosPesquisa.CPFCNPJRemetente > 0D)
                query = query.Where(o => o.MunicipiosDescarregamento.Any(m => m.Documentos.Any(d => d.CTe.Remetente.Cliente.CPF_CNPJ == filtrosPesquisa.CPFCNPJRemetente)));

            if (filtrosPesquisa.Status.HasValue)
                query = query.Where(o => o.Status == filtrosPesquisa.Status);

            if (!string.IsNullOrWhiteSpace(filtrosPesquisa.Placa))
                query = query.Where(o => o.Veiculos.Any(v => v.Placa == filtrosPesquisa.Placa));

            if (filtrosPesquisa.Serie > 0)
                query = query.Where(o => o.Serie.Numero == filtrosPesquisa.Serie);

            if (filtrosPesquisa.CodigoCarga > 0)
            {
                var subquery = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaMDFe>();

                query = query.Where(mdfe => subquery.Any(cargaMdfe => (cargaMdfe.MDFe.Codigo == mdfe.Codigo) && cargaMdfe.Carga.Codigo == filtrosPesquisa.CodigoCarga));
            }

            if (filtrosPesquisa.CodigosFiliais.Any(codigo => codigo == -1))
            {
                var subquery = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaMDFe>();

                query = query.Where(mdfe => subquery.Any(cargaMdfe => (cargaMdfe.MDFe.Codigo == mdfe.Codigo) && (filtrosPesquisa.CodigosFiliais.Contains(cargaMdfe.Carga.Filial.Codigo) || cargaMdfe.Carga.Pedidos.Any(pedido => filtrosPesquisa.CodigosRecebedores.Contains(pedido.Recebedor.CPF_CNPJ)))));
            }

            return query;
        }

        #endregion
    }
}
