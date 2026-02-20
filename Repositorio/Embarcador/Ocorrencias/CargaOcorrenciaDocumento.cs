using NHibernate.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;

namespace Repositorio.Embarcador.Ocorrencias
{
    public class CargaOcorrenciaDocumento : RepositorioBase<Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrenciaDocumento>
    {
        public CargaOcorrenciaDocumento(UnitOfWork unitOfWork) : base(unitOfWork) { }

        #region Métodos Públicos

        public Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrenciaDocumento BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrenciaDocumento>();
            var result = from obj in query where obj.Codigo == codigo select obj;
            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrenciaDocumento> BuscarOcorrenciasPendentesCargaRemetenteDestinatario(int carga, double remetente, double destinatario)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrenciaDocumento>();
            var resut = from obj in query
                        where obj.CargaOcorrencia.Carga.Codigo == carga
                        && (obj.CargaOcorrencia.SituacaoOcorrencia != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoOcorrencia.Cancelada)
                        select obj;

            if (remetente > 0)
                resut = resut.Where(obj => obj.CargaCTe.CTe.Remetente.Cliente.CPF_CNPJ == remetente);

            if (destinatario > 0)
                resut = resut.Where(obj => obj.CargaCTe.CTe.Destinatario.Cliente.CPF_CNPJ == destinatario);

            return resut.OrderByDescending(obj => obj.Codigo).ToList();
        }

        public Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrenciaDocumento BuscarPorCargaRemetenteDestinatario(int carga, double remetente, double destinatario, int tipoOcorrencia, int numeroCTe)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrenciaDocumento>();
            var resut = from obj in query
                        where obj.CargaOcorrencia.Carga.Codigo == carga
      && (obj.CargaOcorrencia.SituacaoOcorrencia != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoOcorrencia.Cancelada
      && obj.CargaOcorrencia.SituacaoOcorrencia != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoOcorrencia.Finalizada
      && obj.CargaOcorrencia.SituacaoOcorrencia != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoOcorrencia.Rejeitada
      && obj.CargaOcorrencia.SituacaoOcorrencia != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoOcorrencia.RejeitadaEtapaEmissao
      )
                        select obj;

            if (tipoOcorrencia > 0)
                resut = resut.Where(obj => obj.CargaOcorrencia.TipoOcorrencia.Codigo == tipoOcorrencia);

            if (remetente > 0)
                resut = resut.Where(obj => obj.CargaCTe.CTe.Remetente.Cliente.CPF_CNPJ == remetente);

            if (destinatario > 0)
                resut = resut.Where(obj => (obj.CargaCTe.CTe.Destinatario.Cliente.CPF_CNPJ == destinatario || obj.CargaCTe.CTe.IndicadorGlobalizado == Dominio.Enumeradores.OpcaoSimNao.Sim));

            if (numeroCTe > 0)
                resut = resut.Where(obj => obj.CargaCTe.CTe.Numero == numeroCTe);

            return resut.FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrenciaDocumento BuscarPorCargaENotaFiscal(int carga, List<int> numerosNFe)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrenciaDocumento>();
            var resut = from obj in query
                        where obj.CargaOcorrencia.Carga.Codigo == carga
      && (obj.CargaOcorrencia.SituacaoOcorrencia != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoOcorrencia.Cancelada
      && obj.CargaOcorrencia.SituacaoOcorrencia != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoOcorrencia.Finalizada
      && obj.CargaOcorrencia.SituacaoOcorrencia != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoOcorrencia.Rejeitada
      && obj.CargaOcorrencia.SituacaoOcorrencia != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoOcorrencia.RejeitadaEtapaEmissao
      )
                        select obj;

            if (numerosNFe != null && numerosNFe.Any())
            {
                resut = resut.Where(obj => obj.CargaCTe.NotasFiscais.Any(nf => numerosNFe.Contains(nf.PedidoXMLNotaFiscal.XMLNotaFiscal.Numero)));
            }

            return resut.FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrenciaDocumento BuscarPorCTeImportado(int cte)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrenciaDocumento>();
            var result = from obj in query where obj.CTeImportado.Codigo == cte select obj;
            return result.FirstOrDefault();
        }

        public bool ExisteOcorrenciaPendenteEmissaoPorCTeImportado(int codigoCTe)
        {
            List<Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoOcorrencia> situacoesPermitidas = new List<Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoOcorrencia>() {
                Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoOcorrencia.Cancelada,
                Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoOcorrencia.Anulada,
                Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoOcorrencia.Finalizada
            };

            IQueryable<Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrenciaDocumento> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrenciaDocumento>();

            query = query.Where(o => o.CTeImportado.Codigo == codigoCTe && !situacoesPermitidas.Contains(o.CargaOcorrencia.SituacaoOcorrencia));

            return query.Select(o => o.Codigo).Any();
        }

        public List<Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrenciaDocumento> BuscarPorCargaCTe(int codigoCargaCTe)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrenciaDocumento>();
            var result = from obj in query where obj.CargaCTe.Codigo == codigoCargaCTe select obj;
            return result.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrenciaDocumento> BuscarPorOcorrencia(int cargaOcorrencia)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrenciaDocumento>();
            var result = from obj in query where obj.CargaOcorrencia.Codigo == cargaOcorrencia select obj;
            return result
                .Fetch(obj => obj.CargaCTe)
                .ThenFetch(obj => obj.CTe)
                .ToList();
        }

        public Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrenciaDocumento BuscarPorCargaCTeEOcorrencia(int codigoCargaCTe, int cargaOcorrencia)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrenciaDocumento>();
            var result = from obj in query where obj.CargaCTe.Codigo == codigoCargaCTe && obj.CargaOcorrencia.Codigo == cargaOcorrencia select obj;
            return result
                .Fetch(obj => obj.CargaCTe)
                .ThenFetch(obj => obj.CTe)
                .FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrenciaDocumento BuscarPorCTeEOcorrencia(int codigoCTe, int cargaOcorrencia)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrenciaDocumento>();
            var result = from obj in query where obj.CargaCTe.CTe.Codigo == codigoCTe && obj.CargaOcorrencia.Codigo == cargaOcorrencia select obj;
            return result
                .Fetch(obj => obj.CargaCTe)
                .ThenFetch(obj => obj.CTe)
                .FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.CargaDocumentoParaEmissaoNFSManual> BuscarNFSManualPorOcorrencia(int cargaOcorrencia)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrenciaDocumento>()
                .Where(o => o.CargaOcorrencia.Codigo == cargaOcorrencia && o.CargaDocumentoParaEmissaoNFSManualComplementado != null)
                .Select(o => o.CargaDocumentoParaEmissaoNFSManualComplementado);

            return query.Fetch(obj => obj.CTe).ToList();
        }

        public List<Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrenciaDocumento> BuscarOcorrenciaPorCTe(int codigoCTe, string propOrdenacao, string dirOrdenacao, int inicioRegistros, int maximoRegistros)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrenciaDocumento>();
            var resut = from obj in query where obj.CargaCTe.CTe.Codigo == codigoCTe select obj;

            if (!string.IsNullOrWhiteSpace(propOrdenacao))
                resut = resut.OrderBy(propOrdenacao + (dirOrdenacao == "asc" ? " ascending" : " descending"));

            if (inicioRegistros > 0)
                resut = resut.Skip(inicioRegistros);
            if (maximoRegistros > 0)
                resut = resut.Take(maximoRegistros);

            return resut.ToList();
        }

        public int ContarBuscarOcorrenciaPorCTe(int codigoCTe)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrenciaDocumento>();
            var resut = from obj in query where obj.CargaCTe.CTe.Codigo == codigoCTe select obj;

            return resut.Count();
        }

        public List<int> ObterCodigoCtesPorOcorrencia(int codigoOcorrencia)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrenciaDocumento>();
            var resut = from obj in query where obj.CargaOcorrencia.Codigo == codigoOcorrencia where obj.CargaCTe.CTe != null select obj.CargaCTe.CTe.Codigo;

            return resut.ToList();
        }

        public Dominio.Entidades.Cliente ObterPrimeiroTomador(int codigoOcorrencia)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrenciaDocumento>();
            var resut = from obj in query where obj.CargaOcorrencia.Codigo == codigoOcorrencia where obj.CargaCTe.CTe != null select obj.CargaCTe.CTe.TomadorPagador.Cliente;

            return resut.FirstOrDefault();
        }

        public Dominio.Entidades.Cliente ObterPrimeiroDestinatario(int codigoOcorrencia)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrenciaDocumento>();
            var resut = from obj in query where obj.CargaOcorrencia.Codigo == codigoOcorrencia where obj.CargaCTe.CTe != null select obj.CargaCTe.CTe.Destinatario.Cliente;

            return resut.FirstOrDefault();
        }

        public List<Dominio.Entidades.ConhecimentoDeTransporteEletronico> BuscarCTesENFSesPorOcorrencia(int codigoOcorrencia)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrenciaDocumento>();
            var resut = from obj in query where obj.CargaOcorrencia.Codigo == codigoOcorrencia where obj.CargaCTe != null select obj.CargaCTe;

            var resutCTe = resut
                .Select(o => o.CTe)
                .Where(o => (
                    o.ModeloDocumentoFiscal.TipoDocumentoEmissao == Dominio.Enumeradores.TipoDocumento.CTe
                    || o.ModeloDocumentoFiscal.TipoDocumentoEmissao == Dominio.Enumeradores.TipoDocumento.NFSe
                    || o.ModeloDocumentoFiscal.TipoDocumentoEmissao == Dominio.Enumeradores.TipoDocumento.NFS
                ));


            return resutCTe.ToList();
        }

        public int ContarCTesPorOcorrencia(int codigoOcorrencia)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrenciaDocumento>();
            var resut = from obj in query where obj.CargaOcorrencia.Codigo == codigoOcorrencia where obj.CargaCTe != null select obj.CargaCTe;

            return resut.Count();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.CargaCTe> BuscarCTesPorOcorrencia(int codigoOcorrencia, string propOrdenacao, string dirOrdenacao, int inicioRegistros, int maximoRegistros)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrenciaDocumento>();
            var resut = from obj in query where obj.CargaOcorrencia.Codigo == codigoOcorrencia where obj.CargaCTe != null select obj.CargaCTe;

            resut = resut.Fetch(o => o.PreCTe).ThenFetch(o => o.Remetente).ThenFetch(o => o.Cliente)
                         .Fetch(o => o.PreCTe).ThenFetch(o => o.Destinatario).ThenFetch(o => o.Cliente)
                         .Fetch(o => o.PreCTe).ThenFetch(o => o.OutrosTomador).ThenFetch(o => o.Cliente)
                         .Fetch(o => o.PreCTe).ThenFetch(o => o.Expedidor).ThenFetch(o => o.Cliente)
                         .Fetch(o => o.PreCTe).ThenFetch(o => o.Recebedor).ThenFetch(o => o.Cliente)
                         .Fetch(o => o.PreCTe).ThenFetch(o => o.LocalidadeTerminoPrestacao)
                         .Fetch(o => o.CTe).ThenFetch(o => o.Remetente).ThenFetch(o => o.Cliente)
                         .Fetch(o => o.CTe).ThenFetch(o => o.Destinatario).ThenFetch(o => o.Cliente)
                         .Fetch(o => o.CTe).ThenFetch(o => o.TomadorPagador).ThenFetch(o => o.Cliente)
                         .Fetch(o => o.CTe).ThenFetch(o => o.MensagemStatus)
                         .Fetch(o => o.CTe).ThenFetch(o => o.Serie)
                         .Fetch(o => o.CTe).ThenFetch(o => o.LocalidadeTerminoPrestacao)
                         .Fetch(o => o.CTe).ThenFetch(o => o.ModeloDocumentoFiscal);

            if (!string.IsNullOrWhiteSpace(propOrdenacao))
                resut = resut.OrderBy(propOrdenacao + (dirOrdenacao == "asc" ? " ascending" : " descending"));

            if (inicioRegistros > 0)
                resut = resut.Skip(inicioRegistros);
            if (maximoRegistros > 0)
                resut = resut.Take(maximoRegistros);

            return resut.ToList();
        }

        public Dominio.Entidades.Embarcador.Cargas.CargaCTe BuscarCTePorOcorrencia(int codigoOcorrencia)
        {
            var consultaCargaOcorrenciaDocumento = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrenciaDocumento>()
                .Where(o => o.CargaOcorrencia.Codigo == codigoOcorrencia && o.CargaCTe != null);

            return consultaCargaOcorrenciaDocumento.Select(o => o.CargaCTe).FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.CargaDocumentoParaEmissaoNFSManual> BuscarDocumentosParaEmissaoNFManualPorOcorrencia(int codigoOcorrencia, string propOrdenacao, string dirOrdenacao, int inicioRegistros, int maximoRegistros)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCTeComplementoInfo>();
            var resut = from obj in query where obj.CargaOcorrencia.Codigo == codigoOcorrencia && obj.CargaDocumentoParaEmissaoNFSManualComplementado != null select obj.CargaDocumentoParaEmissaoNFSManualComplementado;

            resut = resut.Fetch(o => o.CTe).ThenFetch(o => o.Remetente).ThenFetch(o => o.Cliente)
                         .Fetch(o => o.CTe).ThenFetch(o => o.Destinatario).ThenFetch(o => o.Cliente)
                         .Fetch(o => o.CTe).ThenFetch(o => o.TomadorPagador).ThenFetch(o => o.Cliente)
                         .Fetch(o => o.CTe).ThenFetch(o => o.MensagemStatus)
                         .Fetch(o => o.CTe).ThenFetch(o => o.Serie)
                         .Fetch(o => o.CTe).ThenFetch(o => o.LocalidadeTerminoPrestacao)
                         .Fetch(o => o.CTe).ThenFetch(o => o.ModeloDocumentoFiscal);

            if (!string.IsNullOrWhiteSpace(propOrdenacao))
                resut = resut.OrderBy(propOrdenacao + (dirOrdenacao == "asc" ? " ascending" : " descending"));

            if (inicioRegistros > 0)
                resut = resut.Skip(inicioRegistros);
            if (maximoRegistros > 0)
                resut = resut.Take(maximoRegistros);

            return resut.ToList();
        }

        public int ContarDocumentosParaEmissaoNFManualPorOcorrencia(int codigoOcorrencia)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCTeComplementoInfo>();
            var resut = from obj in query where obj.CargaOcorrencia.Codigo == codigoOcorrencia && obj.CargaDocumentoParaEmissaoNFSManualComplementado != null select obj.CargaDocumentoParaEmissaoNFSManualComplementado;

            return resut.Count();
        }

        public int ContarPorCTEsOcorrencia(int codigoOcorrencia)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrenciaDocumento>();
            var resut = from obj in query where obj.CargaOcorrencia.Codigo == codigoOcorrencia && obj.CargaCTe != null select obj;

            return resut.Count();
        }

        public bool ExisteAtivoPorCTe(int codigoCTe)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrenciaDocumento>();

            query = query.Where(o => o.CTeImportado.Codigo == codigoCTe && o.CargaOcorrencia.SituacaoOcorrencia != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoOcorrencia.Cancelada);

            return query.Any();
        }

        public bool ExisteAtivoPorListaCTe(IEnumerable<int> codigosCTes)
        {
            IQueryable<Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrenciaDocumento> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrenciaDocumento>();

            query = query.Where(o => codigosCTes.Contains(o.CargaCTe.CTe.Codigo) &&
                                     o.CargaCTe.Carga.SituacaoCarga != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.Anulada &&
                                     o.CargaCTe.Carga.SituacaoCarga != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.Cancelada &&
                                     (o.CargaOcorrencia.SituacaoOcorrencia != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoOcorrencia.Anulada || o.CargaOcorrencia.SituacaoOcorrencia != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoOcorrencia.Cancelada));

            return query.Any();
        }

        public Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrenciaDocumento BuscarUltimaOcorrenciaPorCargaCTe(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrenciaDocumento>();

            var result = from o in query
                         where
                            o.CargaCTe.Codigo == codigo
                         orderby o.CargaOcorrencia.DataOcorrencia descending
                         select o;

            return result.FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrenciaDocumento BuscarUltimaOcorrenciaPorDocumento(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrenciaDocumento>();

            var result = from o in query
                         where
                            o.CTeImportado.Codigo == codigo ||
                            o.CargaCTe.CTe.Codigo == codigo
                         orderby o.CargaOcorrencia.DataOcorrencia descending
                         select o;

            return result.FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrenciaDocumento BuscarOcorrenciaFinalPorDocumento(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrenciaDocumento>();

            var result = from o in query
                         where o.CargaCTe.Codigo == codigo && o.CargaOcorrencia.TipoOcorrencia.Tipo == "F"

                         select o;

            return result.FirstOrDefault();
        }

        public bool ContemOcorrenciaFinalizadora(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrenciaDocumento>();

            var result = from o in query
                         where
                            o.CargaCTe.CTe.Codigo == codigo
                            && o.CargaOcorrencia.TipoOcorrencia.Tipo == "F"
                         select o;

            return result.Count() > 0;
        }

        public bool ExisteOcorrenciaPorChaveCTeNaoComplementar(string chave)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrenciaDocumento>();

            return query.Where(x => x.CargaCTe.CTe.Chave == chave && x.CargaCTe.CTe.TipoCTE != Dominio.Enumeradores.TipoCTE.Complemento).Any();
        }

        public async Task<int> DeletarPorOcorrenciaAsync(int ocorrencia)
        {
            string hql = "DELETE CargaOcorrenciaDocumento where Codigo in (select obj.Codigo from CargaOcorrenciaDocumento obj where obj.CargaOcorrencia.Codigo = :Ocorrencia)";
            var query = this.SessionNHiBernate.CreateQuery(hql);
            query.SetInt32("Ocorrencia", ocorrencia);

            return await query.ExecuteUpdateAsync();
        }

        #endregion
    }
}
