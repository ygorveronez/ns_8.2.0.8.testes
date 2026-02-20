using AdminMultisoftware.Dominio.Enumeradores;
using Dominio.ObjetosDeValor.WebService;
using Repositorio;
using System.Collections.Generic;

namespace Servicos.WebService.Faturamento
{
    public class Faturamento : ServicoWebServiceBase
    {
        #region Variaveis Privadas

        readonly Repositorio.UnitOfWork _unitOfWork;
        readonly Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado _auditado;
        readonly TipoServicoMultisoftware _tipoServicoMultisoftware;
        readonly AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente _clienteMultisoftware;
        readonly AdminMultisoftware.Dominio.Entidades.Pessoas.ClienteURLAcesso _clienteAcesso;
        readonly protected string _adminStringConexao;

        #endregion

        #region Constructores
        public Faturamento(UnitOfWork unitOfWork) : base(unitOfWork) { }
        public Faturamento(Repositorio.UnitOfWork unitOfWork, TipoServicoMultisoftware tipoServicoMultisoftware, AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente clienteMultisoftware, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado, AdminMultisoftware.Dominio.Entidades.Pessoas.ClienteURLAcesso clienteAcesso, string adminStringConexao) : base(unitOfWork, tipoServicoMultisoftware, clienteMultisoftware)
        {
            _unitOfWork = unitOfWork;
            _tipoServicoMultisoftware = tipoServicoMultisoftware;
            _clienteMultisoftware = clienteMultisoftware;
            _auditado = auditado;
            _clienteAcesso = clienteAcesso;
            _adminStringConexao = adminStringConexao;
        }
        #endregion

        #region Metodos Publicos

        public Retorno<bool> ConfirmarIntegracaoDocumentoFaturamento(int protocolo)
        {
            Repositorio.Embarcador.Financeiro.DocumentoFaturamento repDocumentoFaturamento = new Repositorio.Embarcador.Financeiro.DocumentoFaturamento(_unitOfWork);
            Dominio.Entidades.Embarcador.Financeiro.DocumentoFaturamento documentoFaturamento = repDocumentoFaturamento.BuscarPorCodigo(protocolo);

            if (documentoFaturamento == null)
                return Retorno<bool>.CriarRetornoExcecao("Documento faturamento informada não existe no Multi Embarcador");

            if (!documentoFaturamento.PendenteIntegracaoEmbarcador)
                return Retorno<bool>.CriarRetornoExcecao("Documento já teve sua confirmação de integração realizada anteriormente");

            documentoFaturamento.PendenteIntegracaoEmbarcador = false;
            repDocumentoFaturamento.Atualizar(documentoFaturamento);

            Servicos.Auditoria.Auditoria.Auditar(_auditado, documentoFaturamento, "Confirmou integração do documento faturamento.", _unitOfWork);
            return Retorno<bool>.CriarRetornoSucesso(true);

        }

        public Retorno<Dominio.ObjetosDeValor.WebService.Paginacao<Dominio.ObjetosDeValor.Embarcador.Financeiro.DocumentoFaturamento>> BuscarDocumentosPagamentoLiberado(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoRetorno tipoDocumentoRetorno, int inicio, int limite)
        {
            if (limite >= 50)
                return Retorno<Dominio.ObjetosDeValor.WebService.Paginacao<Dominio.ObjetosDeValor.Embarcador.Financeiro.DocumentoFaturamento>>.CriarRetornoExcecao("O limite não pode ser maior que 50");

            Dominio.ObjetosDeValor.WebService.Paginacao<Dominio.ObjetosDeValor.Embarcador.Financeiro.DocumentoFaturamento> retorno = new Dominio.ObjetosDeValor.WebService.Paginacao<Dominio.ObjetosDeValor.Embarcador.Financeiro.DocumentoFaturamento>()
            {
                Itens = new List<Dominio.ObjetosDeValor.Embarcador.Financeiro.DocumentoFaturamento>(),
                NumeroTotalDeRegistro = 0
            };

            Repositorio.Embarcador.Financeiro.DocumentoFaturamento repDocumentoFaturamento = new Repositorio.Embarcador.Financeiro.DocumentoFaturamento(_unitOfWork);
            Repositorio.CTeContaContabilContabilizacao repCTeContaContabilContabilizacao = new Repositorio.CTeContaContabilContabilizacao(_unitOfWork);
            Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repConfiguracaoTMS = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(_unitOfWork);
            Servicos.WebService.CTe.CTe serWSCTe = new Servicos.WebService.CTe.CTe(_unitOfWork);
            Servicos.WebService.NFS.NFS serWSNFS = new Servicos.WebService.NFS.NFS(_unitOfWork);

            Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoTMS = repConfiguracaoTMS.BuscarConfiguracaoPadrao();

            List<Dominio.Entidades.Embarcador.Financeiro.DocumentoFaturamento> documentoFaturamentos = repDocumentoFaturamento.BuscarPendentesIntegracaoEmbarcador(inicio, limite);

            foreach (Dominio.Entidades.Embarcador.Financeiro.DocumentoFaturamento documentoFaturamento in documentoFaturamentos)
            {
                Dominio.ObjetosDeValor.Embarcador.Financeiro.DocumentoFaturamento doc = new Dominio.ObjetosDeValor.Embarcador.Financeiro.DocumentoFaturamento()
                {
                    DadosContaveis = new List<Dominio.ObjetosDeValor.WebService.Faturamento.DadoContabel>()
                };

                BuscarDadosContabeis(doc, documentoFaturamento);

                doc.ProtocoloDocumento = documentoFaturamento.Codigo;
                List<Dominio.Entidades.CTeContaContabilContabilizacao> cTeContaContabilContabilizacaos = repCTeContaContabilContabilizacao.BuscarPorCTe(documentoFaturamento.CTe.Codigo);
                if (documentoFaturamento.CTe.ModeloDocumentoFiscal.TipoDocumentoEmissao == Dominio.Enumeradores.TipoDocumento.CTe)
                    doc.CTe = serWSCTe.ConverterObjetoCTe(documentoFaturamento.CTe, cTeContaContabilContabilizacaos, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoRetorno.XML, _unitOfWork, configuracaoTMS.UtilizarCodificacaoUTF8ConversaoPDF);
                else
                    doc.NFS = serWSNFS.ConverterObjetoNFS(documentoFaturamento.CTe, cTeContaContabilContabilizacaos, tipoDocumentoRetorno, configuracaoTMS.UtilizarCodificacaoUTF8ConversaoPDF, _unitOfWork);

                retorno.Itens.Add(doc);
            }

            retorno.NumeroTotalDeRegistro = repDocumentoFaturamento.ContarPendentesIntegracaoEmbarcador();

            Servicos.Auditoria.Auditoria.AuditarConsulta(_auditado, "Buscou documentações pendentes de integração", _unitOfWork);

            return Retorno<Dominio.ObjetosDeValor.WebService.Paginacao<Dominio.ObjetosDeValor.Embarcador.Financeiro.DocumentoFaturamento>>.CriarRetornoSucesso(retorno);
        }

        #endregion

        #region Metodos Privados

        public void BuscarDadosContabeis(Dominio.ObjetosDeValor.Embarcador.Financeiro.DocumentoFaturamento objDocumentoFaturamento, Dominio.Entidades.Embarcador.Financeiro.DocumentoFaturamento documentoFaturamento)
        {
            Repositorio.Embarcador.ConfiguracaoContabil.DocumentoContabil repositorioDocumentoContabil = new Repositorio.Embarcador.ConfiguracaoContabil.DocumentoContabil(_unitOfWork);
            List<Dominio.Entidades.Embarcador.ConfiguracaoContabil.DocumentoContabil> listaDocumentoContabeis = repositorioDocumentoContabil.BuscarPorDocumentoFaturamento(documentoFaturamento.Codigo);

            foreach (Dominio.Entidades.Embarcador.ConfiguracaoContabil.DocumentoContabil documento in listaDocumentoContabeis)
            {
                objDocumentoFaturamento.DadosContaveis.Add(new Dominio.ObjetosDeValor.WebService.Faturamento.DadoContabel()
                {
                    CentroResultado = new Dominio.ObjetosDeValor.WebService.Faturamento.CentroResultado()
                    {
                        Codigo = documento?.CentroResultado?.Codigo ?? 0,
                        Descripcao = documento?.CentroResultado?.Descricao ?? string.Empty,
                        Numero = documento?.CentroResultado?.Codigo ?? 0
                    },
                    ContaContabil = new Dominio.ObjetosDeValor.WebService.Faturamento.ContaContabil()
                    {
                        Codigo = documento?.PlanoConta.Codigo ?? 0,
                        Descripcao = documento?.PlanoConta?.Descricao ?? string.Empty,
                        Numero = documento?.PlanoConta.Codigo ?? 0
                    },
                    Valor = documento?.ValorContabilizacao ?? 0m
                });
            }

        }
        #endregion
    }
}
