using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using NHibernate.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.NFS
{
    public class LancamentoNFSManual : RepositorioBase<Dominio.Entidades.Embarcador.NFS.LancamentoNFSManual>
    {

        public LancamentoNFSManual(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.Embarcador.NFS.LancamentoNFSManual BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.NFS.LancamentoNFSManual>();
            var result = from obj in query where obj.Codigo == codigo select obj;
            return result
                .Fetch(obj => obj.DadosNFS)
                .ThenFetch(obj => obj.Serie)
                .Fetch(obj => obj.Transportador)
                .ThenFetch(obj => obj.Configuracao)
                .FirstOrDefault();
        }
        public Dominio.Entidades.Embarcador.NFS.LancamentoNFSManual BuscarPorNumeroTomadorSituacao(int numero, double cnpjTomador, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoLancamentoNFSManual situacao)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.NFS.LancamentoNFSManual>();
            var result = from obj in query where obj.DadosNFS.Numero == numero && obj.Tomador.CPF_CNPJ == cnpjTomador && obj.Situacao == situacao select obj;
            return result.FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.NFS.LancamentoNFSManual BuscarPorNumeroRPSTomadorSituacao(int numeroRPS, double cnpjTomador, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoLancamentoNFSManual situacao)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.NFS.LancamentoNFSManual>();
            var result = from obj in query where obj.DadosNFS.NumeroRPS == numeroRPS && obj.Tomador.CPF_CNPJ == cnpjTomador && obj.Situacao == situacao select obj;
            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.NFS.LancamentoNFSManual> BuscarPorDadosNFS(int lancamento)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.NFS.LancamentoNFSManual>();
            var result = from obj in query where obj.DadosNFS.Codigo == lancamento select obj;
            return result.ToList();
        }

        public Dominio.Entidades.Embarcador.NFS.LancamentoNFSManual BuscarPorCTe(int cte)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.NFS.LancamentoNFSManual>();
            var result = from obj in query where obj.CTe.Codigo == cte select obj;
            return result.FirstOrDefault();
        }

        public List<int> BuscarNFSManualPendentesConfirmacao(int inicioRegistros, int maximoRegistros)
        {
            IQueryable<Dominio.Entidades.Embarcador.NFS.LancamentoNFSManual> query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.NFS.LancamentoNFSManual>();

            query = query.Where(obj => obj.CTe != null && obj.CTe.Status == "A" && obj.CTe.Codigo > 0 && (obj.CTe.NFsManualIntegrada == null || obj.CTe.NFsManualIntegrada == false));

            return query.Select(o => o.CTe.Codigo).Skip(inicioRegistros).Take(maximoRegistros).ToList();
        }

        public int ContarNFSManualPendentesConfirmacao()
        {
            IQueryable<Dominio.Entidades.Embarcador.NFS.LancamentoNFSManual> query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.NFS.LancamentoNFSManual>();

            query = query.Where(obj => obj.CTe != null && obj.CTe.Status == "A" && obj.CTe.Codigo > 0 && (obj.CTe.NFsManualIntegrada == null || obj.CTe.NFsManualIntegrada == false));

            return query.Select(o => o.CTe).Count();
        }

        public bool PossuiCTeSemIntegracao(int codigoLancamentoNFSManual, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao tipoIntegracao)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.NFS.LancamentoNFSManual>();

            var subQueryIntegracoes = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.NFS.NFSManualCTeIntegracao>().Where(o => o.LancamentoNFSManual.Codigo == codigoLancamentoNFSManual && o.TipoIntegracao.Tipo == tipoIntegracao).Select(o => o.LancamentoNFSManual.Codigo);

            query = query.Where(o => o.Codigo == codigoLancamentoNFSManual && !subQueryIntegracoes.Contains(o.Codigo));

            return query.Select(o => o.Codigo).Any();
        }

        public bool PossuiCTeComIntegracao(int codigoLancamentoNFSManual, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao tipoIntegracao)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.NFS.LancamentoNFSManual>();

            var subQueryIntegracoes = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.NFS.NFSManualCTeIntegracao>().Where(o => o.LancamentoNFSManual.Codigo == codigoLancamentoNFSManual && o.TipoIntegracao.Tipo == tipoIntegracao).Select(o => o.Codigo);

            query = query.Where(o => o.Codigo == codigoLancamentoNFSManual && subQueryIntegracoes.Contains(o.Codigo));

            return query.Select(o => o.Codigo).Any();
        }

        public Dominio.Entidades.Embarcador.NFS.LancamentoNFSManual BuscarPorNFSe(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.NFS.LancamentoNFSManual>();
            var result = from obj in query where obj.Documentos.Any(d => d.DocumentosNFSe.NFSe.Codigo == codigo) select obj;
            return result.FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.NFS.LancamentoNFSManual BuscarPorCargaNFS(int codigoCarga, int codigoNotaFiscal)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.NFS.LancamentoNFSManual>();
            var result = from obj in query where obj.Documentos.Any(d => d.Carga.Codigo == codigoCarga && d.PedidoXMLNotaFiscal.XMLNotaFiscal.Codigo == codigoNotaFiscal) select obj;
            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.NFS.LancamentoNFSManual> ConsultarPorLancamento(int lancamento, int inicioRegistros, int maximoRegistros)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.NFS.LancamentoNFSManual>();
            var result = from obj in query
                         where
                            obj.Codigo == lancamento
                            && obj.CTe != null
                         select obj;

            if (inicioRegistros > 0)
                result = result.Skip(inicioRegistros);

            if (maximoRegistros > 0)
                result = result.Take(maximoRegistros);

            return result.ToList();
        }

        public int ContarConsultaPorLancamento(int lancamento)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.NFS.LancamentoNFSManual>();
            var result = from obj in query
                         where
                            obj.Codigo == lancamento
                            && obj.CTe != null
                         select obj;

            return result.Count();
        }

        public List<Dominio.Entidades.Embarcador.NFS.LancamentoNFSManual> BuscarLancamentosEmEmissao()
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.NFS.LancamentoNFSManual>();
            var result = from obj in query where obj.Situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoLancamentoNFSManual.EmEmissao select obj;

            return result.ToList();
        }

        public List<Dominio.Entidades.Embarcador.NFS.LancamentoNFSManual> BuscarLancamentosAgIntegracao()
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.NFS.LancamentoNFSManual>();
            var result = from obj in query where obj.Situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoLancamentoNFSManual.AgIntegracao select obj;

            return result.ToList();
        }

        public List<Dominio.Entidades.Embarcador.NFS.LancamentoNFSManual> BuscarLancamentosAgEmissao(int limite)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.NFS.LancamentoNFSManual>();
            var result = from obj in query where obj.Situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoLancamentoNFSManual.AgEmissao && obj.CTe == null select obj;

            return result.Take(limite).Fetch(obj => obj.Transportador).ToList();
        }

        private IQueryable<Dominio.Entidades.Embarcador.NFS.LancamentoNFSManual> _Consultar(List<int> filiais, DateTime dataInicio, DateTime dataFim, int transportador, int filial, int carga, int numero, int numeroDOC, int localidadePrestacao, double tomador, bool somenteAtivo, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoLancamentoNFSManual situacaoLancamentoNFSManual, string numeroPedidoCliente, int ocorrencia, bool? residual)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.NFS.LancamentoNFSManual>();

            var result = from obj in query select obj;

            List<SituacaoLancamentoNFSManual> lancamentoNFSManualCancelado = new List<SituacaoLancamentoNFSManual>()
                {
                    SituacaoLancamentoNFSManual.Cancelada,
                    SituacaoLancamentoNFSManual.Reprovada,
                    SituacaoLancamentoNFSManual.Anulada
                };

            var consultaCargaDocumentoParaEmissaoNFSManualCancelada = this.SessionNHiBernate
                    .Query<Dominio.Entidades.Embarcador.NFS.CargaDocumentoParaEmissaoNFSManualCancelada>()
                    .Where(o => lancamentoNFSManualCancelado.Contains(o.LancamentoNFSManual.Situacao));

            if (dataInicio != DateTime.MinValue)
                result = result.Where(obj => obj.DataCriacao.Date >= dataInicio);

            if (dataFim != DateTime.MinValue)
                result = result.Where(obj => obj.DataCriacao.Date <= dataFim);

            if (transportador > 0)
                result = result.Where(o => o.Transportador.Codigo == transportador);

            if (tomador > 0)
                result = result.Where(o => o.Tomador.CPF_CNPJ == tomador);

            if (localidadePrestacao > 0)
                result = result.Where(o => o.LocalidadePrestacao.Codigo == localidadePrestacao);

            if (carga > 0)
            {
                var consulta = consultaCargaDocumentoParaEmissaoNFSManualCancelada;

                consulta = consulta.Where(lancamentoNFSManualCancelado => lancamentoNFSManualCancelado.CargaDocumentoParaEmissaoNFSManual.CargaOrigem.Codigo == carga);
                result = result.Where(lancamentoNFSManual => consulta.Where(lancamentoNFSManualCancelado => lancamentoNFSManual.Codigo == lancamentoNFSManualCancelado.LancamentoNFSManual.Codigo).Any() || lancamentoNFSManual.Documentos.Any(doc => doc.Carga.Codigo == carga));
            }

            if (filiais != null && filiais.Count > 0)
                result = result.Where(o => filiais.Contains(o.Filial.Codigo));

            if (filial > 0)
                result = result.Where(o => o.Filial.Codigo == filial);

            if (numero > 0)
                result = result.Where(o => o.DadosNFS.Numero == numero);

            if (ocorrencia > 0)
            {
                var consulta = consultaCargaDocumentoParaEmissaoNFSManualCancelada;

                consulta = consulta.Where(lancamentoNFSManualCancelado => lancamentoNFSManualCancelado.CargaDocumentoParaEmissaoNFSManual.CargaOcorrencia.NumeroOcorrencia == ocorrencia);
                result = result.Where(lancamentoNFSManual => consulta.Where(lancamentoNFSManualCancelado => lancamentoNFSManual.Codigo == lancamentoNFSManualCancelado.LancamentoNFSManual.Codigo).Any() || lancamentoNFSManual.Documentos.Any(doc => doc.CargaOcorrencia.NumeroOcorrencia == ocorrencia));
            }

            if (numeroDOC > 0)
            {
                var consulta = consultaCargaDocumentoParaEmissaoNFSManualCancelada;

                consulta = consulta.Where(lancamentoNFSManualCancelado => lancamentoNFSManualCancelado.CargaDocumentoParaEmissaoNFSManual.Numero == numeroDOC);
                result = result.Where(lancamentoNFSManual => consulta.Where(lancamentoNFSManualCancelado => lancamentoNFSManual.Codigo == lancamentoNFSManualCancelado.LancamentoNFSManual.Codigo).Any() || lancamentoNFSManual.Documentos.Any(doc => doc.Numero == numeroDOC));
            }

            if (situacaoLancamentoNFSManual != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoLancamentoNFSManual.Todas)
                result = result.Where(o => o.Situacao == situacaoLancamentoNFSManual);

            if (!string.IsNullOrEmpty(numeroPedidoCliente))
            {
                var consulta = consultaCargaDocumentoParaEmissaoNFSManualCancelada;

                consulta = consulta.Where(lancamentoNFSManualCancelado => lancamentoNFSManualCancelado.CargaDocumentoParaEmissaoNFSManual.NumeroPedidoCliente == numeroPedidoCliente);
                result = result.Where(lancamentoNFSManual => consulta.Where(lancamentoNFSManualCancelado => lancamentoNFSManual.Codigo == lancamentoNFSManualCancelado.LancamentoNFSManual.Codigo).Any() || lancamentoNFSManual.Documentos.Any(doc => doc.NumeroPedidoCliente == numeroPedidoCliente));
            }

            if (residual.HasValue)
            {
                if (residual.Value == true)
                    result = result.Where(o => o.NFSResidual);
                else
                    result = result.Where(o => !o.NFSResidual);
            }

            if (somenteAtivo)
                result = result.Where(o => o.Situacao != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoLancamentoNFSManual.Cancelada && o.Situacao != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoLancamentoNFSManual.Anulada);

            return result;
        }

        public List<Dominio.Entidades.Embarcador.NFS.LancamentoNFSManual> Consultar(List<int> filiais, DateTime dataInicio, DateTime dataFim, int transportador, int filial, int carga, int numero, int numeroDOC, int localidadePrestacao, double tomador, bool somenteAtivo, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoLancamentoNFSManual situacaoLancamentoNFSManual, string numeroPedidoCliente, int ocorrencia, bool? residual, string propOrdena, string dirOrdena, int inicioRegistros, int maximoRegistros)
        {

            var result = _Consultar(filiais, dataInicio, dataFim, transportador, filial, carga, numero, numeroDOC, localidadePrestacao, tomador, somenteAtivo, situacaoLancamentoNFSManual, numeroPedidoCliente, ocorrencia, residual);

            if (!string.IsNullOrWhiteSpace(propOrdena))
                result = result.OrderBy(propOrdena + (dirOrdena == "asc" ? " ascending" : " descending"));

            if (inicioRegistros > 0)
                result = result.Skip(inicioRegistros);

            if (maximoRegistros > 0)
                result = result.Take(maximoRegistros);

            return result.ToList();
        }

        public int ContarConsulta(List<int> filiais, DateTime dataInicio, DateTime dataFim, int transportador, int filial, int carga, int numero, int numeroDOC, int localidadePrestacao, double tomador, bool somenteAtivo, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoLancamentoNFSManual situacaoLancamentoNFSManual, string numeroPedidoCliente, int ocorrencia, bool? residual)
        {
            var result = _Consultar(filiais, dataInicio, dataFim, transportador, filial, carga, numero, numeroDOC, localidadePrestacao, tomador, somenteAtivo, situacaoLancamentoNFSManual, numeroPedidoCliente, ocorrencia, residual);

            return result.Count();
        }

        public bool ExisteNFSHabilitadaComMesmoNumero(int numero, int serie, int codigoModeloDocumentoFiscal, int codigoEmpresa)
        {
            IQueryable<Dominio.Entidades.Embarcador.NFS.LancamentoNFSManual> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.NFS.LancamentoNFSManual>();

            query = query.Where(o => o.CTe != null && o.DadosNFS.Numero == numero && o.DadosNFS.Serie.Numero == serie && o.DadosNFS.ModeloDocumentoFiscal.Codigo == codigoModeloDocumentoFiscal && o.Situacao != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoLancamentoNFSManual.Cancelada && o.Situacao != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoLancamentoNFSManual.Reprovada && o.Transportador.Codigo == codigoEmpresa && (!o.CTe.Desabilitado.HasValue || !o.CTe.Desabilitado.Value));

            return query.Select(o => o.Codigo).Any();
        }
    }
}
