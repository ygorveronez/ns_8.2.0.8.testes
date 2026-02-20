using System;
using System.Collections.Generic;
using System.Linq;

namespace Servicos.Embarcador.Escrituracao
{
    public class Escrituracao
    {
        #region Atributos

        private readonly Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado _auditado;
        private readonly Repositorio.UnitOfWork _unitOfWork;
        private Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS _configuracaoEmbarcador;
        private Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoFinanceiro _configuracaoFinanceiro;

        #endregion Atributos

        #region Construtores

        public Escrituracao(Repositorio.UnitOfWork unitOfWork, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado) : this(unitOfWork, auditado, configuracaoEmbarcador: null, configuracaoFinanceiro: null) { }

        public Escrituracao(Repositorio.UnitOfWork unitOfWork, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoEmbarcador, Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoFinanceiro configuracaoFinanceiro) : this(unitOfWork, auditado: null, configuracaoEmbarcador, configuracaoFinanceiro) { }

        public Escrituracao(Repositorio.UnitOfWork unitOfWork, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoEmbarcador, Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoFinanceiro configuracaoFinanceiro)
        {
            _unitOfWork = unitOfWork;
            _auditado = auditado;
            _configuracaoEmbarcador = configuracaoEmbarcador;
            _configuracaoFinanceiro = configuracaoFinanceiro;
        }

        #endregion Construtores

        #region Métodos Privados

        private Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS ObterConfiguracaoEmbarcador()
        {
            if (_configuracaoEmbarcador == null)
                _configuracaoEmbarcador = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(_unitOfWork).BuscarConfiguracaoPadrao();

            return _configuracaoEmbarcador;
        }

        private Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoFinanceiro ObterConfiguracaoFinanceiro()
        {
            if (_configuracaoFinanceiro == null)
                _configuracaoFinanceiro = new Repositorio.Embarcador.Configuracoes.ConfiguracaoFinanceiro(_unitOfWork).BuscarPrimeiroRegistro();

            return _configuracaoFinanceiro;
        }

        #endregion Métodos Privados

        #region Métodos Públicos

        public Dominio.Entidades.Embarcador.Escrituracao.LoteEscrituracao AdicionarLoteEscrituracao(DateTime dataInicio, DateTime dataFim, Dominio.Entidades.Cliente tomador, Dominio.Entidades.Embarcador.Filiais.Filial filial, Dominio.Entidades.Empresa empresa, Dominio.Entidades.Embarcador.Pedidos.TipoOperacao tipoOperacao, Dominio.Entidades.ModeloDocumentoFiscal modeloDocumentoFiscal, List<Dominio.Entidades.Embarcador.Escrituracao.DocumentoEscrituracao> documentos)
        {
            Repositorio.Embarcador.Escrituracao.LoteEscrituracao repLoteEscrituracao = new Repositorio.Embarcador.Escrituracao.LoteEscrituracao(_unitOfWork);
            Repositorio.Embarcador.Escrituracao.DocumentoEscrituracao repDocumentoEscrituracao = new Repositorio.Embarcador.Escrituracao.DocumentoEscrituracao(_unitOfWork);

            Dominio.Entidades.Embarcador.Escrituracao.LoteEscrituracao loteEscrituracao = new Dominio.Entidades.Embarcador.Escrituracao.LoteEscrituracao();

            if (dataInicio != DateTime.MinValue)
                loteEscrituracao.DataInicial = dataInicio;

            if (dataFim != DateTime.MinValue)
                loteEscrituracao.DataFinal = dataFim;

            loteEscrituracao.DataGeracaoLote = DateTime.Now;

            loteEscrituracao.Numero = repLoteEscrituracao.ObterProximoLote();
            loteEscrituracao.Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoLoteEscrituracao.AgIntegracao;
            loteEscrituracao.ModeloDocumentoFiscal = modeloDocumentoFiscal;
            loteEscrituracao.Tomador = tomador;
            loteEscrituracao.Filial = filial;
            loteEscrituracao.Empresa = empresa;
            loteEscrituracao.TipoOperacao = tipoOperacao;
            // Persiste dados
            repLoteEscrituracao.Inserir(loteEscrituracao, _auditado);

            foreach (Dominio.Entidades.Embarcador.Escrituracao.DocumentoEscrituracao documento in documentos)
            {
                documento.LoteEscrituracao = loteEscrituracao;
                repDocumentoEscrituracao.Atualizar(documento);
                //Servicos.Auditoria.Auditoria.Auditar(Auditado, documento, null, "Criou o lote de escrituração.", unitOfWork);
            }

            return loteEscrituracao;

        }

        public Dominio.Entidades.Embarcador.Escrituracao.LoteEscrituracaoCancelamento AdicionarLoteEscrituracaoCancelamento(DateTime dataInicio, DateTime dataFim, Dominio.Entidades.Cliente tomador, Dominio.Entidades.Embarcador.Filiais.Filial filial, Dominio.Entidades.Empresa empresa, Dominio.Entidades.ModeloDocumentoFiscal modeloDocumentoFiscal, List<Dominio.Entidades.Embarcador.Escrituracao.DocumentoEscrituracaoCancelamento> documentos)
        {
            Repositorio.Embarcador.Escrituracao.LoteEscrituracaoCancelamento repLoteEscrituracaoCancelamento = new Repositorio.Embarcador.Escrituracao.LoteEscrituracaoCancelamento(_unitOfWork);
            Repositorio.Embarcador.Escrituracao.DocumentoEscrituracaoCancelamento repDocumentoEscrituracaoCancelamento = new Repositorio.Embarcador.Escrituracao.DocumentoEscrituracaoCancelamento(_unitOfWork);

            Dominio.Entidades.Embarcador.Escrituracao.LoteEscrituracaoCancelamento loteEscrituracaoCancelamento = new Dominio.Entidades.Embarcador.Escrituracao.LoteEscrituracaoCancelamento();

            if (dataInicio != DateTime.MinValue)
                loteEscrituracaoCancelamento.DataInicial = dataInicio;

            if (dataFim != DateTime.MinValue)
                loteEscrituracaoCancelamento.DataFinal = dataFim;

            loteEscrituracaoCancelamento.DataGeracaoLote = DateTime.Now;

            loteEscrituracaoCancelamento.Numero = repLoteEscrituracaoCancelamento.ObterProximoLote();
            loteEscrituracaoCancelamento.Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoLoteEscrituracaoCancelamento.AgIntegracao;
            loteEscrituracaoCancelamento.ModeloDocumentoFiscal = modeloDocumentoFiscal;
            loteEscrituracaoCancelamento.Tomador = tomador;
            loteEscrituracaoCancelamento.Filial = filial;
            loteEscrituracaoCancelamento.Empresa = empresa;

            // Persiste dados
            repLoteEscrituracaoCancelamento.Inserir(loteEscrituracaoCancelamento, _auditado);

            foreach (Dominio.Entidades.Embarcador.Escrituracao.DocumentoEscrituracaoCancelamento documento in documentos)
            {
                documento.LoteEscrituracaoCancelamento = loteEscrituracaoCancelamento;

                repDocumentoEscrituracaoCancelamento.Atualizar(documento);
            }

            return loteEscrituracaoCancelamento;

        }

        public void GerarLotesEscrituracao(AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            Servicos.Embarcador.Configuracoes.ConfiguracaoFinanceiroIntervaloTempoDocumentoEmitidoEscrituracao servicoConfiguracaoFinanceiroIntervaloTempoDocumentoEmitidoEscrituracao = new Servicos.Embarcador.Configuracoes.ConfiguracaoFinanceiroIntervaloTempoDocumentoEmitidoEscrituracao(_unitOfWork);

            Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoEmbarcador = ObterConfiguracaoEmbarcador();

            if (!configuracaoEmbarcador.AutomatizarGeracaoLoteEscrituracao)
                return;

            bool permitirGerarLotesEscrituracao = (DateTime.Now.Hour > 0) || (DateTime.Now.Minute >= 10);

            if (!permitirGerarLotesEscrituracao)
                return;

            int diasRetroativos = -2;

            if (tipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS)
                diasRetroativos = -1;
            else if ((DateTime.Now.Date.Day == 1) && (DateTime.Now.Hour >= 3))
                diasRetroativos = -1;

            Dominio.ObjetosDeValor.Embarcador.Escrituracao.FiltroPesquisaDocumentoEscrituracao filtrosPesquisa = new Dominio.ObjetosDeValor.Embarcador.Escrituracao.FiltroPesquisaDocumentoEscrituracao()
            {
                DataLimite = DateTime.Today.AddDays(diasRetroativos),
                SomentePagamentoLiberado = true,
                TipoServicoMultisoftware = tipoServicoMultisoftware,
                IntervaloParaEscrituracaoDocumento = servicoConfiguracaoFinanceiroIntervaloTempoDocumentoEmitidoEscrituracao.ObterIntervalo(),
            };

            Repositorio.Embarcador.Escrituracao.DocumentoEscrituracao repDocumentoEscrituracao = new Repositorio.Embarcador.Escrituracao.DocumentoEscrituracao(_unitOfWork);

            if (repDocumentoEscrituracao.ContarConsulta(filtrosPesquisa) == 0)
                return;
            
            Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametroConsulta = new Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta();
            List<Dominio.Entidades.Embarcador.Escrituracao.DocumentoEscrituracao> documentoEscrituracaos = repDocumentoEscrituracao.Consultar(filtrosPesquisa, parametroConsulta);
            List<Dominio.Entidades.Embarcador.Pedidos.TipoOperacao> tiposOperacao = (from obj in documentoEscrituracaos select obj.TipoOperacao).Distinct().ToList();

            foreach (Dominio.Entidades.Embarcador.Pedidos.TipoOperacao tipoOperacao in tiposOperacao)
            {
                List<Dominio.Entidades.Cliente> tomadores = (from obj in documentoEscrituracaos where obj.TipoOperacao == tipoOperacao select obj.CTe.TomadorPagador.Cliente).Distinct().ToList();
                    
                foreach (Dominio.Entidades.Cliente tomador in tomadores)
                {
                    List<Dominio.Entidades.Embarcador.Escrituracao.DocumentoEscrituracao> documentoEscrituracaosTomador = (from obj in documentoEscrituracaos where obj.TipoOperacao == tipoOperacao && obj.CTe.TomadorPagador.Cliente.CPF_CNPJ == tomador.CPF_CNPJ select obj).ToList();
                    List<Dominio.Entidades.Empresa> empresas = (from obj in documentoEscrituracaosTomador select obj.CTe.Empresa).Distinct().ToList();
                       
                    foreach (Dominio.Entidades.Empresa empresa in empresas)
                    {
                        List<Dominio.Entidades.Embarcador.Escrituracao.DocumentoEscrituracao> documentoEscrituracaosEmpresa = (from obj in documentoEscrituracaosTomador where obj.TipoOperacao == tipoOperacao && obj.CTe.TomadorPagador.Cliente.CPF_CNPJ == tomador.CPF_CNPJ && obj.CTe.Empresa.Codigo == empresa.Codigo select obj).ToList();
                        List<Dominio.Entidades.ModeloDocumentoFiscal> modelos = (from obj in documentoEscrituracaosEmpresa select obj.CTe.ModeloDocumentoFiscal).Distinct().ToList();
                            
                        foreach (Dominio.Entidades.ModeloDocumentoFiscal modelo in modelos)
                        {
                            List<Dominio.Entidades.Embarcador.Escrituracao.DocumentoEscrituracao> documentos = (from obj in documentoEscrituracaosEmpresa where obj.TipoOperacao == tipoOperacao && obj.CTe.TomadorPagador.Cliente.CPF_CNPJ == tomador.CPF_CNPJ && obj.CTe.Empresa.Codigo == empresa.Codigo && obj.CTe.ModeloDocumentoFiscal.Codigo == modelo.Codigo select obj).ToList();
                                
                            _unitOfWork.Start();
                                
                            AdicionarLoteEscrituracao(DateTime.MinValue, filtrosPesquisa.DataLimite.Value, tomador, null, empresa, tipoOperacao, modelo, documentos);

                            _unitOfWork.CommitChanges();
                        }
                    }
                }
            }
        }

        public void GerarLotesEscrituracaoCancelamento(AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoEmbarcador = ObterConfiguracaoEmbarcador();

            if (!configuracaoEmbarcador.AutomatizarGeracaoLoteEscrituracaoCancelamento)
                return;

            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoFinanceiro configuracaoFinanceiro = ObterConfiguracaoFinanceiro();
            List<int> diasFechamento = new List<int>();

            if (!string.IsNullOrWhiteSpace(configuracaoFinanceiro?.DiasDeFechamentoParaGeracaoPagamentoEscrituracaoAutomatico))
            {
                string[] splitDias = configuracaoFinanceiro.DiasDeFechamentoParaGeracaoPagamentoEscrituracaoAutomatico.Split(',');
                for (int i = 0; i < splitDias.Length; i++)
                {
                    int.TryParse(splitDias[i], out int dia);
                    if (dia > 0)
                        diasFechamento.Add(dia);
                }
            }
            else
            {
                diasFechamento.Add(1);
            }

            bool permitirGerarLotesEscrituracao = (DateTime.Now.Hour > 0) || (DateTime.Now.Minute >= 10);

            if (!permitirGerarLotesEscrituracao)
                return;

            int diasRetroativos = -2;

            if (tipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS)
                diasRetroativos = -1;
            else if (diasFechamento.Contains(DateTime.Now.Date.Day) && (DateTime.Now.Hour >= 10))
                diasRetroativos = -1;

            Dominio.ObjetosDeValor.Embarcador.Escrituracao.FiltroPesquisaDocumentoEscrituracaoCancelamento filtrosPesquisa = new Dominio.ObjetosDeValor.Embarcador.Escrituracao.FiltroPesquisaDocumentoEscrituracaoCancelamento()
            {
                DataLimite = DateTime.Today.AddDays(diasRetroativos),
                SomentePagamentoLiberado = true
            };

            Repositorio.Embarcador.Escrituracao.DocumentoEscrituracaoCancelamento repDocumentoEscrituracaoCancelamento = new Repositorio.Embarcador.Escrituracao.DocumentoEscrituracaoCancelamento(_unitOfWork);

            if (repDocumentoEscrituracaoCancelamento.ContarConsulta(filtrosPesquisa) == 0)
                return;

            Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametroConsulta = new Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta();
            List<Dominio.Entidades.Embarcador.Escrituracao.DocumentoEscrituracaoCancelamento> documentosEscrituracao = repDocumentoEscrituracaoCancelamento.Consultar(filtrosPesquisa, parametroConsulta);
            List<Dominio.Entidades.Cliente> tomadores = documentosEscrituracao.Select(o => o.CTe.TomadorPagador.Cliente).Distinct().ToList();

            foreach (Dominio.Entidades.Cliente tomador in tomadores)
            {
                List<Dominio.Entidades.Embarcador.Escrituracao.DocumentoEscrituracaoCancelamento> documentoEscrituracaosTomador = documentosEscrituracao.Where(o => o.CTe.TomadorPagador.Cliente.CPF_CNPJ == tomador.CPF_CNPJ).ToList();
                List<Dominio.Entidades.Empresa> empresas = documentoEscrituracaosTomador.Select(o => o.CTe.Empresa).Distinct().ToList();
                
                foreach (Dominio.Entidades.Empresa empresa in empresas)
                {
                    List<Dominio.Entidades.Embarcador.Escrituracao.DocumentoEscrituracaoCancelamento> documentoEscrituracaosEmpresa = documentoEscrituracaosTomador.Where(o => o.CTe.Empresa.Codigo == empresa.Codigo).ToList();
                    List<Dominio.Entidades.ModeloDocumentoFiscal> modelos = documentoEscrituracaosEmpresa.Select(o => o.CTe.ModeloDocumentoFiscal).Distinct().ToList();
                    
                    foreach (Dominio.Entidades.ModeloDocumentoFiscal modelo in modelos)
                    {
                        List<Dominio.Entidades.Embarcador.Escrituracao.DocumentoEscrituracaoCancelamento> documentos = documentoEscrituracaosEmpresa.Where(o => o.CTe.ModeloDocumentoFiscal.Codigo == modelo.Codigo).ToList();

                        _unitOfWork.Start();

                        AdicionarLoteEscrituracaoCancelamento(DateTime.MinValue, filtrosPesquisa.DataLimite.Value, tomador, null, empresa, modelo, documentos);

                        _unitOfWork.CommitChanges();
                    }
                }
            }
        }

        #endregion Métodos Públicos
    }
}
