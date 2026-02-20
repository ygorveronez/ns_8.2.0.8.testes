using NHibernate.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.Documentos
{
    public class DocumentoDestinadoEmpresa : RepositorioBase<Dominio.Entidades.Embarcador.Documentos.DocumentoDestinadoEmpresa>
    {
        #region Construtores

        public DocumentoDestinadoEmpresa(UnitOfWork unitOfWork) : base(unitOfWork) { }

        #endregion

        #region Métodos Públicos

        public bool VerificarNotaCartaCorrecaoEmitente(string chave)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Documentos.DocumentoDestinadoEmpresa>();

            query = query.Where(o => o.TipoDocumento == TipoDocumentoDestinadoEmpresa.CCe && o.Chave == chave);

            return query.Any();
        }

        public bool VerificarNotaCanceladaEmitente(string chave)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Documentos.DocumentoDestinadoEmpresa>();

            query = query.Where(o => o.TipoDocumento == TipoDocumentoDestinadoEmpresa.CancelamentoNFe && o.Chave == chave);

            return query.Any();
        }

        public bool ExistePorNSU(int codigoEmpresa, long nsu, ModeloDocumentoDestinado modeloDocumentoDestinado)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Documentos.DocumentoDestinadoEmpresa>();

            query = query.Where(o => o.Empresa.Codigo == codigoEmpresa && o.ModeloDocumento == modeloDocumentoDestinado && o.NumeroSequencialUnico == nsu);

            return query.Any();
        }

        public List<long> BuscarPorTipoDocumento(TipoDocumentoDestinadoEmpresa tipoDocumentos)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Documentos.DocumentoDestinadoEmpresa>();

            query = query.Where(o => o.TipoDocumento == tipoDocumentos && o.DataEmissao.HasValue && o.DataEmissao.Value.Month == 12 && o.DataEmissao.Value.Year == 2023);

            return query.Select(c => c.Codigo).Timeout(99999).ToList();
        }

        public Dominio.Entidades.Embarcador.Documentos.DocumentoDestinadoEmpresa BuscarPorChaveETipoDocumento(TipoDocumentoDestinadoEmpresa[] tiposDocumentos, string chave)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Documentos.DocumentoDestinadoEmpresa>();

            query = query.Where(o => o.Chave == chave && tiposDocumentos.Contains(o.TipoDocumento));

            return query.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Documentos.DocumentoDestinadoEmpresa> BuscarDocumentosPorChaveETipoDocumento(string chave, TipoDocumentoDestinadoEmpresa[] tiposDocumentos)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Documentos.DocumentoDestinadoEmpresa>();

            query = query.Where(o => o.Chave == chave && tiposDocumentos.Contains(o.TipoDocumento));

            return query.ToList();
        }

        public Dominio.Entidades.Embarcador.Documentos.DocumentoDestinadoEmpresa BuscarPorChaveETipoDocumento(int codigoEmpresa, TipoDocumentoDestinadoEmpresa[] tiposDocumentos, string chave)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Documentos.DocumentoDestinadoEmpresa>();

            query = query.Where(o => o.Empresa.Codigo == codigoEmpresa && o.Chave == chave && tiposDocumentos.Contains(o.TipoDocumento));

            return query.FirstOrDefault();
        }

        public long BuscarPrimeiroNSU(Dominio.ObjetosDeValor.Embarcador.Enumeradores.ModeloDocumentoDestinado modeloDocumento)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Documentos.DocumentoDestinadoEmpresa>();

            query = query.Where(o => o.ModeloDocumento == modeloDocumento);

            return query.Min(o => (long?)o.NumeroSequencialUnico) ?? 0L;
        }
        public long BuscarPrimeiroNSUEmpresa(int codigoEmpresa, Dominio.ObjetosDeValor.Embarcador.Enumeradores.ModeloDocumentoDestinado modeloDocumento)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Documentos.DocumentoDestinadoEmpresa>();

            query = query.Where(o => o.Empresa.Codigo == codigoEmpresa && o.ModeloDocumento == modeloDocumento);

            return query.Min(o => (long?)o.NumeroSequencialUnico) ?? 0L;
        }
        public long BuscarUltimoNSUEmpresa(int codigoEmpresa, Dominio.ObjetosDeValor.Embarcador.Enumeradores.ModeloDocumentoDestinado modeloDocumento)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Documentos.DocumentoDestinadoEmpresa>();

            query = query.Where(o => o.Empresa.Codigo == codigoEmpresa && o.ModeloDocumento == modeloDocumento);

            return query.Max(o => (long?)o.NumeroSequencialUnico) ?? 0L;
        }

        public List<Dominio.Entidades.Embarcador.Documentos.DocumentoDestinadoEmpresa> BuscarPendentesEnvioIntegracao(int codigoEmpresa)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Documentos.DocumentoDestinadoEmpresa>();
            query = query.Where(o => o.Empresa.Codigo == codigoEmpresa &&
                o.GerouArquivoIntegracao == false &&
                (o.ModeloDocumento == ModeloDocumentoDestinado.NFe || o.ModeloDocumento == ModeloDocumentoDestinado.CTe) &&
                (o.TipoDocumento == TipoDocumentoDestinadoEmpresa.NFeDestinada || o.TipoDocumento == TipoDocumentoDestinadoEmpresa.CTeDestinadoTomador));//Alterado por solicitação de Ademilson 03/08/2019

            return query.Distinct().ToList();
        }
        public List<Dominio.Entidades.Embarcador.Documentos.DocumentoDestinadoEmpresa> BuscarPendentesEnvioImputIntegracao(int codigoEmpresa)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Documentos.DocumentoDestinadoEmpresa>();
            query = query.Where(o => o.Empresa.Codigo == codigoEmpresa &&
                o.EnviouXMLImputIntegracao == false && o.TentativasEnvioImputIntegracao <= 20 &&
                (o.ModeloDocumento == ModeloDocumentoDestinado.NFe || o.ModeloDocumento == ModeloDocumentoDestinado.CTe) &&
                (o.TipoDocumento == TipoDocumentoDestinadoEmpresa.NFeDestinada || o.TipoDocumento == TipoDocumentoDestinadoEmpresa.CTeDestinadoTomador));//Alterado por solicitação de Ademilson 03/08/2019

            return query.Distinct().ToList();
        }

        public Dominio.Entidades.Embarcador.Documentos.DocumentoDestinadoEmpresa BuscarNFSePorCPFCNPJEmitenteNumeroNota(string CPFCNPJEmitente, int numeroNota, DateTime dataEmissao)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Documentos.DocumentoDestinadoEmpresa>();
            query = query.Where(o => o.Numero == numeroNota && o.CPFCNPJEmitente == CPFCNPJEmitente && o.DataEmissao.Value.Date.Year == dataEmissao.Date.Year
            && o.ModeloDocumento == ModeloDocumentoDestinado.NFe && o.TipoDocumento == TipoDocumentoDestinadoEmpresa.NFSeDestinada);

            return query.FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.Documentos.DocumentoDestinadoEmpresa BuscarNFePorChave(string chaveNFe)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Documentos.DocumentoDestinadoEmpresa>();
            query = query.Where(o => o.Chave == chaveNFe &&
                (o.ModeloDocumento == ModeloDocumentoDestinado.NFe || o.ModeloDocumento == ModeloDocumentoDestinado.CTe) &&
                (o.TipoDocumento == TipoDocumentoDestinadoEmpresa.NFeDestinada || o.TipoDocumento == TipoDocumentoDestinadoEmpresa.NFeTransporte
                || o.TipoDocumento == TipoDocumentoDestinadoEmpresa.CTeDestinadoDestinatario || o.TipoDocumento == TipoDocumentoDestinadoEmpresa.CTeDestinadoEmitente
                || o.TipoDocumento == TipoDocumentoDestinadoEmpresa.CTeDestinadoExpedidor || o.TipoDocumento == TipoDocumentoDestinadoEmpresa.CTeDestinadoRecebedor
                || o.TipoDocumento == TipoDocumentoDestinadoEmpresa.CTeDestinadoRemetente || o.TipoDocumento == TipoDocumentoDestinadoEmpresa.CTeDestinadoTerceiro
                || o.TipoDocumento == TipoDocumentoDestinadoEmpresa.CTeDestinadoTomador));

            return query.FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.Documentos.DocumentoDestinadoEmpresa BuscarCancelamentoPorChave(string chaveNFe)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Documentos.DocumentoDestinadoEmpresa>();
            query = query.Where(o => o.Chave == chaveNFe &&
                (o.ModeloDocumento == ModeloDocumentoDestinado.NFe || o.ModeloDocumento == ModeloDocumentoDestinado.CTe) &&
                (o.TipoDocumento == TipoDocumentoDestinadoEmpresa.CancelamentoNFe || o.TipoDocumento == TipoDocumentoDestinadoEmpresa.CancelamentoCTe));

            return query.FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.Documentos.DocumentoDestinadoEmpresa BuscarPorCodigo(long codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Documentos.DocumentoDestinadoEmpresa>();

            query = query.Where(o => o.Codigo == codigo);

            return query.FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.Documentos.DocumentoDestinadoEmpresa BuscarPorChave(string chave)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Documentos.DocumentoDestinadoEmpresa>();

            query = query.Where(o => o.Chave == chave);

            return query.FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.Documentos.DocumentoDestinadoEmpresa BuscarPorChaveAguardandoDesacordo(string chave)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Documentos.DocumentoDestinadoEmpresa>();

            query = query.Where(o => o.Chave == chave);
            query = query.Where(o => o.SituacaoManifestacaoDestinatario == SituacaoManifestacaoDestinatario.AgAprovacaoDesacordoServico);

            return query.FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.Documentos.DocumentoDestinadoEmpresa BuscarPorNumeroSerieEEmitente(int numero, int serie, string cnpjEmitente)
        {
            IQueryable<Dominio.Entidades.Embarcador.Documentos.DocumentoDestinadoEmpresa> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Documentos.DocumentoDestinadoEmpresa>();

            query = query.Where(o => o.Numero == numero && o.Serie == serie && o.CPFCNPJEmitente == cnpjEmitente && o.TipoDocumento == TipoDocumentoDestinadoEmpresa.NFeTransporte);

            return query.FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.Documentos.DocumentoDestinadoEmpresa BuscarPorNumeroEEmitente(int numero, string cnpjEmitente)
        {
            IQueryable<Dominio.Entidades.Embarcador.Documentos.DocumentoDestinadoEmpresa> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Documentos.DocumentoDestinadoEmpresa>();

            query = query.Where(o => o.Numero == numero && o.CPFCNPJEmitente == cnpjEmitente && o.TipoDocumento == TipoDocumentoDestinadoEmpresa.NFeTransporte);

            return query.OrderByDescending(o => o.Codigo).FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Documentos.DocumentoDestinadoEmpresa> BuscarPorCodigos(int empresa, List<long> codigos)
        {
            IQueryable<Dominio.Entidades.Embarcador.Documentos.DocumentoDestinadoEmpresa> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Documentos.DocumentoDestinadoEmpresa>();

            query = query.Where(o => codigos.Contains(o.Codigo));

            if (empresa > 0)
                query = query.Where(o => o.Empresa.Codigo == empresa);

            return query.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Documentos.DocumentoDestinadoEmpresa> Consultar(Dominio.ObjetosDeValor.Embarcador.Documentos.FiltroPesquisaDocumentoDestinadoEmpresa filtrosPesquisa, string propOrdena, string dirOrdena, int inicio, int limite)
        {
            var consultaDocumentoDestinadoEmpresa = Consultar(filtrosPesquisa);

            if (propOrdena == "Destinatario")
                propOrdena = "Empresa.CNPJ";

            return ObterLista(consultaDocumentoDestinadoEmpresa, propOrdena, dirOrdena, inicio, limite);
        }

        public List<Dominio.Entidades.Embarcador.Documentos.DocumentoDestinadoEmpresa> Consultar(Dominio.ObjetosDeValor.Embarcador.Documentos.FiltroPesquisaDocumentoDestinadoEmpresa filtrosPesquisa, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            var consultaDocumentoDestinadoEmpresa = Consultar(filtrosPesquisa);

            return ObterLista(consultaDocumentoDestinadoEmpresa, parametrosConsulta);
        }

        public int ContarConsulta(Dominio.ObjetosDeValor.Embarcador.Documentos.FiltroPesquisaDocumentoDestinadoEmpresa filtrosPesquisa)
        {
            var consultaDocumentoDestinadoEmpresa = Consultar(filtrosPesquisa);

            return consultaDocumentoDestinadoEmpresa.Count();
        }

        public List<Dominio.Entidades.Embarcador.Documentos.DocumentoDestinadoEmpresa> BuscarPorDatas(Dominio.ObjetosDeValor.Embarcador.Documentos.FiltroPesquisaDocumentoDestinadoEmpresa filtrosPesquisa)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Documentos.DocumentoDestinadoEmpresa>();

            if(filtrosPesquisa.DataEmissaoInicial.HasValue)
                query = query.Where(x => x.DataEmissao >= filtrosPesquisa.DataEmissaoInicial);

            if (filtrosPesquisa.DataEmissaoFinal.HasValue)
                query = query.Where(x => x.DataEmissao <= filtrosPesquisa.DataEmissaoFinal);

            return query.Select(o => o).ToList();
        }


        public List<Dominio.Entidades.Embarcador.Documentos.DocumentoDestinadoEmpresa> ObterDocumentosDestinados(Dominio.ObjetosDeValor.Embarcador.Documentos.FiltroPesquisaDocumentoDestinadoEmpresa filtrosPesquisa, bool selecionarTodos, List<long> codigosTitulos)
        {
            var query = Consultar(filtrosPesquisa);

            if (selecionarTodos)
                query = query.Where(o => !codigosTitulos.Contains(o.Codigo));
            else
                query = query.Where(o => codigosTitulos.Contains(o.Codigo));

            return query.Select(o => o).ToList();
        }

        public List<Dominio.Entidades.Embarcador.Documentos.DocumentoDestinadoEmpresa> ConsultarMultiCTe(int codigoEmpresa, ModeloDocumentoDestinado? modeloDocumento, TipoDocumentoDestinadoEmpresa? tipoDocumento, DateTime dataAutorizacaoInicial, DateTime dataAutorizacaoFinal, DateTime dataEmissaoInicial, DateTime dataEmissaoFinal, int numeroInicial, int NumeroFinal, int serie, string cnpjEmissor, string nomeEmissor, string placa, string chave, bool? cancelado, bool? notasSemCTe, string cnpjRemetente, string nomeRemetente, string cnpjTomador, string nomeTomador, string ufDestinatario, bool filtrarEventos, int inicioRegistros, int maximoRegistros)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Documentos.DocumentoDestinadoEmpresa>();

            if (!filtrarEventos)
                query = query.Where(o => o.DescricaoEvento == string.Empty || o.DescricaoEvento == null);

            if (cancelado.HasValue)
            {
                if (cancelado.Value)
                    query = query.Where(o => o.Cancelado);
                else
                    query = query.Where(o => !o.Cancelado);
            }

            if (codigoEmpresa > 0)
                query = query.Where(o => o.Empresa.Codigo == codigoEmpresa);

            if (modeloDocumento.HasValue)
                query = query.Where(o => o.ModeloDocumento == modeloDocumento);

            if (tipoDocumento.HasValue)
                query = query.Where(o => o.TipoDocumento == tipoDocumento);

            if (numeroInicial > 0)
                query = query.Where(o => o.Numero >= numeroInicial);

            if (NumeroFinal > 0)
                query = query.Where(o => o.Numero <= NumeroFinal);

            if (serie > 0)
                query = query.Where(o => o.Serie == serie);

            if (!string.IsNullOrWhiteSpace(chave))
                query = query.Where(o => o.Chave == chave);

            if (!string.IsNullOrWhiteSpace(cnpjEmissor))
                query = query.Where(o => o.CPFCNPJEmitente.Contains(cnpjEmissor));

            if (!string.IsNullOrWhiteSpace(nomeEmissor))
                query = query.Where(o => o.NomeEmitente.Contains(nomeEmissor));

            if (!string.IsNullOrWhiteSpace(cnpjRemetente))
                query = query.Where(o => o.CPFCNPJRemetente.Contains(cnpjRemetente));

            if (!string.IsNullOrWhiteSpace(nomeRemetente))
                query = query.Where(o => o.NomeRemetente.Contains(nomeRemetente));

            if (!string.IsNullOrWhiteSpace(cnpjTomador))
                query = query.Where(o => o.CPFCNPJTomador.Contains(cnpjTomador));

            if (!string.IsNullOrWhiteSpace(nomeTomador))
                query = query.Where(o => o.NomeTomador.Contains(nomeTomador));

            if (!string.IsNullOrWhiteSpace(placa))
                query = query.Where(o => o.Placa == placa);

            if (dataAutorizacaoInicial != DateTime.MinValue)
                query = query.Where(o => o.DataAutorizacao >= dataAutorizacaoInicial.Date);

            if (dataAutorizacaoFinal != DateTime.MinValue)
                query = query.Where(o => o.DataAutorizacao < dataAutorizacaoFinal.AddDays(1).Date);

            if (dataEmissaoInicial != DateTime.MinValue)
                query = query.Where(o => o.DataEmissao >= dataEmissaoInicial.Date);

            if (dataEmissaoFinal != DateTime.MinValue)
                query = query.Where(o => o.DataEmissao < dataEmissaoFinal.AddDays(1).Date);

            if (!string.IsNullOrWhiteSpace(ufDestinatario))
                query = query.Where(o => o.UFDestinatario == ufDestinatario);

            if (notasSemCTe.HasValue)
            {
                var queryDocumentos = this.SessionNHiBernate.Query<Dominio.Entidades.DocumentosCTE>();

                //Pendentes emissão (Sem CTe Autorizado/Salvo)
                if (notasSemCTe.Value)
                    query = query.Where(o => !(from obj in queryDocumentos where obj.CTE.Empresa.Codigo == codigoEmpresa && (obj.CTE.Status == "A" || obj.CTE.Status == "S") select obj.ChaveNFE).Contains(o.Chave));
                else
                    query = query.Where(o => (from obj in queryDocumentos where obj.CTE.Empresa.Codigo == codigoEmpresa && (obj.CTE.Status == "A" || obj.CTE.Status == "S") select obj.ChaveNFE).Contains(o.Chave));
                //Com CTe Autorizado/Salvo
            }

            query = query.OrderBy("CPFCNPJEmitente, Numero ");

            if (inicioRegistros > 0)
                query = query.Skip(inicioRegistros);

            if (maximoRegistros > 0)
                query = query.Take(maximoRegistros);

            return query.ToList();
        }

        public int ContarMultiCTe(int codigoEmpresa, ModeloDocumentoDestinado? modeloDocumento, TipoDocumentoDestinadoEmpresa? tipoDocumento, DateTime dataAutorizacaoInicial, DateTime dataAutorizacaoFinal, DateTime dataEmissaoInicial, DateTime dataEmissaoFinal, int numeroInicial, int NumeroFinal, int serie, string cnpjEmissor, string nomeEmissor, string placa, string chave, bool? cancelado, bool? notasSemCTe, string cnpjRemetente, string nomeRemetente, string cnpjTomador, string nomeTomador, bool filtrarEventos, string ufDestinatario)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Documentos.DocumentoDestinadoEmpresa>();

            if (!filtrarEventos)
                query = query.Where(o => o.DescricaoEvento == string.Empty || o.DescricaoEvento == null);

            if (cancelado.HasValue)
            {
                if (cancelado.Value)
                    query = query.Where(o => o.Cancelado);
                else
                    query = query.Where(o => !o.Cancelado);
            }

            if (codigoEmpresa > 0)
                query = query.Where(o => o.Empresa.Codigo == codigoEmpresa);

            if (modeloDocumento.HasValue)
                query = query.Where(o => o.ModeloDocumento == modeloDocumento);

            if (tipoDocumento.HasValue)
                query = query.Where(o => o.TipoDocumento == tipoDocumento);

            if (numeroInicial > 0)
                query = query.Where(o => o.Numero >= numeroInicial);

            if (NumeroFinal > 0)
                query = query.Where(o => o.Numero <= NumeroFinal);

            if (serie > 0)
                query = query.Where(o => o.Serie == serie);

            if (!string.IsNullOrWhiteSpace(chave))
                query = query.Where(o => o.Chave == chave);

            if (!string.IsNullOrWhiteSpace(cnpjEmissor))
                query = query.Where(o => o.CPFCNPJEmitente.Contains(cnpjEmissor));

            if (!string.IsNullOrWhiteSpace(nomeEmissor))
                query = query.Where(o => o.NomeEmitente.Contains(nomeEmissor));

            if (!string.IsNullOrWhiteSpace(cnpjRemetente))
                query = query.Where(o => o.CPFCNPJRemetente.Contains(cnpjRemetente));

            if (!string.IsNullOrWhiteSpace(nomeRemetente))
                query = query.Where(o => o.NomeRemetente.Contains(nomeRemetente));

            if (!string.IsNullOrWhiteSpace(cnpjTomador))
                query = query.Where(o => o.CPFCNPJTomador.Contains(cnpjTomador));

            if (!string.IsNullOrWhiteSpace(nomeTomador))
                query = query.Where(o => o.NomeTomador.Contains(nomeTomador));

            if (!string.IsNullOrWhiteSpace(placa))
                query = query.Where(o => o.Placa == placa);

            if (dataAutorizacaoInicial != DateTime.MinValue)
                query = query.Where(o => o.DataAutorizacao >= dataAutorizacaoInicial.Date);

            if (dataAutorizacaoFinal != DateTime.MinValue)
                query = query.Where(o => o.DataAutorizacao < dataAutorizacaoFinal.AddDays(1).Date);

            if (dataEmissaoInicial != DateTime.MinValue)
                query = query.Where(o => o.DataEmissao >= dataEmissaoInicial.Date);

            if (dataEmissaoFinal != DateTime.MinValue)
                query = query.Where(o => o.DataEmissao < dataEmissaoFinal.AddDays(1).Date);

            if (!string.IsNullOrWhiteSpace(ufDestinatario))
                query = query.Where(o => o.UFDestinatario == ufDestinatario);

            if (notasSemCTe.HasValue)
            {
                var queryDocumentos = this.SessionNHiBernate.Query<Dominio.Entidades.DocumentosCTE>();

                //Pendentes emissão (Sem CTe Autorizado/Salvo)
                if (notasSemCTe.Value)
                    query = query.Where(o => !(from obj in queryDocumentos where obj.CTE.Empresa.Codigo == codigoEmpresa && (obj.CTE.Status == "A" || obj.CTE.Status == "S") select obj.ChaveNFE).Contains(o.Chave));
                else
                    query = query.Where(o => (from obj in queryDocumentos where obj.CTE.Empresa.Codigo == codigoEmpresa && (obj.CTE.Status == "A" || obj.CTE.Status == "S") select obj.ChaveNFE).Contains(o.Chave));
                //Com CTe Autorizado/Salvo
            }

            return query.Count();
        }

        public bool ContemEventoCancelamentoPorChave(string chave)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Documentos.DocumentoDestinadoEmpresa>();
            query = query.Where(o => o.Chave == chave);
            query = query.Where(o => o.TipoDocumento == TipoDocumentoDestinadoEmpresa.CancelamentoCTe || o.TipoDocumento == TipoDocumentoDestinadoEmpresa.CancelamentoMDFe || o.TipoDocumento == TipoDocumentoDestinadoEmpresa.CancelamentoNFe || o.TipoDocumento == TipoDocumentoDestinadoEmpresa.MDFECancelado);

            return query.Any();
        }

        #endregion

        #region Métodos Privados

        private IQueryable<Dominio.Entidades.Embarcador.Documentos.DocumentoDestinadoEmpresa> Consultar(Dominio.ObjetosDeValor.Embarcador.Documentos.FiltroPesquisaDocumentoDestinadoEmpresa filtrosPesquisa)
        {
            var consultaDocumentoDestinadoEmpresa = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Documentos.DocumentoDestinadoEmpresa>();

            if (filtrosPesquisa.PossuiDocumentoEntrada.HasValue)
            {
                if (filtrosPesquisa.PossuiDocumentoEntrada.Value)
                    consultaDocumentoDestinadoEmpresa = consultaDocumentoDestinadoEmpresa.Where(o => o.DocumentoEntrada != null);
                else
                    consultaDocumentoDestinadoEmpresa = consultaDocumentoDestinadoEmpresa.Where(o => o.DocumentoEntrada == null);
            }

            if (filtrosPesquisa.Cancelado.HasValue)
                consultaDocumentoDestinadoEmpresa = consultaDocumentoDestinadoEmpresa.Where(o => o.Cancelado == filtrosPesquisa.Cancelado.Value);

            if (filtrosPesquisa.CodigoEmpresa > 0)
                consultaDocumentoDestinadoEmpresa = consultaDocumentoDestinadoEmpresa.Where(o => o.Empresa.Codigo == filtrosPesquisa.CodigoEmpresa);

            if (filtrosPesquisa.TiposDocumento?.Count > 0)
                consultaDocumentoDestinadoEmpresa = consultaDocumentoDestinadoEmpresa.Where(o => filtrosPesquisa.TiposDocumento.Contains(o.TipoDocumento));

            if (filtrosPesquisa.SituacaoManifestacaoDestinatario?.Count > 0)
                consultaDocumentoDestinadoEmpresa = consultaDocumentoDestinadoEmpresa.Where(o => filtrosPesquisa.SituacaoManifestacaoDestinatario.Contains(o.SituacaoManifestacaoDestinatario));

            if (filtrosPesquisa.NumeroDe > 0 && filtrosPesquisa.NumeroAte > 0)
                consultaDocumentoDestinadoEmpresa = consultaDocumentoDestinadoEmpresa.Where(o => o.Numero >= filtrosPesquisa.NumeroDe && o.Numero <= filtrosPesquisa.NumeroAte);
            else if (filtrosPesquisa.NumeroDe > 0 && filtrosPesquisa.NumeroAte <= 0)
                consultaDocumentoDestinadoEmpresa = consultaDocumentoDestinadoEmpresa.Where(o => o.Numero == filtrosPesquisa.NumeroDe);
            else if (filtrosPesquisa.NumeroAte > 0 && filtrosPesquisa.NumeroDe <= 0)
                consultaDocumentoDestinadoEmpresa = consultaDocumentoDestinadoEmpresa.Where(o => o.Numero == filtrosPesquisa.NumeroAte);

            if (filtrosPesquisa.Serie > 0)
                consultaDocumentoDestinadoEmpresa = consultaDocumentoDestinadoEmpresa.Where(o => o.Serie == filtrosPesquisa.Serie);

            if (!string.IsNullOrWhiteSpace(filtrosPesquisa.Chave))
                consultaDocumentoDestinadoEmpresa = consultaDocumentoDestinadoEmpresa.Where(o => o.Chave == filtrosPesquisa.Chave);

            if (!string.IsNullOrWhiteSpace(filtrosPesquisa.CpfCnpjFornecedor))
                consultaDocumentoDestinadoEmpresa = consultaDocumentoDestinadoEmpresa.Where(o => o.CPFCNPJEmitente == filtrosPesquisa.CpfCnpjFornecedor);

            if (!string.IsNullOrWhiteSpace(filtrosPesquisa.NomeFornecedor))
                consultaDocumentoDestinadoEmpresa = consultaDocumentoDestinadoEmpresa.Where(o => o.NomeEmitente.Contains(filtrosPesquisa.NomeFornecedor));

            if (filtrosPesquisa.DataAutorizacaoInicial.HasValue && filtrosPesquisa.DataAutorizacaoInicial.Value > DateTime.MinValue)
                consultaDocumentoDestinadoEmpresa = consultaDocumentoDestinadoEmpresa.Where(o => o.DataAutorizacao >= filtrosPesquisa.DataAutorizacaoInicial.Value.Date);

            if (filtrosPesquisa.DataAutorizacaoFinal.HasValue && filtrosPesquisa.DataAutorizacaoFinal.Value > DateTime.MinValue)
                consultaDocumentoDestinadoEmpresa = consultaDocumentoDestinadoEmpresa.Where(o => o.DataAutorizacao < filtrosPesquisa.DataAutorizacaoFinal.Value.Date.Add(DateTime.MaxValue.TimeOfDay));

            if (filtrosPesquisa.DataEmissaoInicial.HasValue && filtrosPesquisa.DataEmissaoInicial.Value > DateTime.MinValue)
                consultaDocumentoDestinadoEmpresa = consultaDocumentoDestinadoEmpresa.Where(o => o.DataEmissao >= filtrosPesquisa.DataEmissaoInicial.Value.Date);

            if (filtrosPesquisa.DataEmissaoFinal.HasValue && filtrosPesquisa.DataEmissaoFinal.Value > DateTime.MinValue)
                consultaDocumentoDestinadoEmpresa = consultaDocumentoDestinadoEmpresa.Where(o => o.DataEmissao < filtrosPesquisa.DataEmissaoFinal.Value.Date.Add(DateTime.MaxValue.TimeOfDay));

            if (filtrosPesquisa.TipoOperacao.HasValue)
                consultaDocumentoDestinadoEmpresa = consultaDocumentoDestinadoEmpresa.Where(o => o.TipoOperacao == filtrosPesquisa.TipoOperacao.Value);

            if (filtrosPesquisa.SituacaoIntegracao.HasValue)
            {
                var queryDocumentoIntegracao = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Documentos.DocumentoDestinadoIntegracao>();
                var query2DocumentoIntegracao = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Documentos.DocumentoDestinadoIntegracao>();
                consultaDocumentoDestinadoEmpresa = consultaDocumentoDestinadoEmpresa.Where(o => queryDocumentoIntegracao.Any(x => o.Codigo == x.DocumentoDestinadoEmpresa.Codigo && query2DocumentoIntegracao.Where(p => p.DocumentoDestinadoEmpresa.Codigo == o.Codigo && p.SituacaoIntegracao == filtrosPesquisa.SituacaoIntegracao).Count() > 1));
            }

            return consultaDocumentoDestinadoEmpresa;
        }

        #endregion
    }
}
