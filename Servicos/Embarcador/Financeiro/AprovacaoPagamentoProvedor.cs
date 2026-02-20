using Dominio.Entidades.Embarcador.Financeiro;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Servicos.Embarcador.Financeiro
{
    public sealed class AprovacaoPagamentoProvedor : RegraAutorizacao.AprovacaoAlcada
    <
        Dominio.Entidades.Embarcador.Financeiro.PagamentoProvedor,
        Dominio.Entidades.Embarcador.Escrituracao.AlcadasPagamentoProvedor.RegraPagamentoProvedor,
        Dominio.Entidades.Embarcador.Escrituracao.AlcadasPagamentoProvedor.AprovacaoAlcadaPagamentoProvedor
    >
    {
        #region Atributos Privados

        private Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS _configuracaoEmbarcador;
        private readonly string _urlAcesso;

        #endregion

        #region Construtores

        public AprovacaoPagamentoProvedor(Repositorio.UnitOfWork unitOfWork) : this(unitOfWork, configuracaoEmbarcador: null, urlAcesso: null) { }

        public AprovacaoPagamentoProvedor(Repositorio.UnitOfWork unitOfWork, string urlAcesso) : this(unitOfWork, configuracaoEmbarcador: null, urlAcesso) { }

        public AprovacaoPagamentoProvedor(Repositorio.UnitOfWork unitOfWork, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoEmbarcador, string urlAcesso) : base(unitOfWork)
        {
            _configuracaoEmbarcador = configuracaoEmbarcador;
            _urlAcesso = urlAcesso;
        }

        #endregion

        #region Métodos Públicos 

        public Dominio.ObjetosDeValor.Embarcador.CTe.CTe LerCTeDeArquivo(Dominio.Entidades.Embarcador.Integracao.ArquivoIntegracao arquivoIntegracao)
        {
            if (arquivoIntegracao == null || !Utilidades.IO.FileStorageService.Storage.Exists(arquivoIntegracao.NomeArquivo))
                return null;

            using (Stream fStream = Utilidades.IO.FileStorageService.Storage.OpenRead(arquivoIntegracao.NomeArquivo))
            {
                var serCte = new Servicos.Embarcador.CTe.CTe(_unitOfWork);
                var cteLido = MultiSoftware.CTe.Servicos.Leitura.Ler(fStream);
                return serCte.ConverterProcCTeParaCTePorObjeto(cteLido);
            }
        }

        public Dominio.Entidades.Embarcador.Integracao.ArquivoIntegracao ObterArquivoIntegracao(Dominio.Entidades.Embarcador.Financeiro.PagamentoProvedor pagamentoProvedor)
        {
            if (pagamentoProvedor.TipoDocumentoProvedor == TipoDocumentoProvedor.CTe || pagamentoProvedor.TipoDocumentoProvedor == TipoDocumentoProvedor.CTeComplementar)
                return pagamentoProvedor.ArquivoXMLCTe;

            if (pagamentoProvedor.TipoDocumentoProvedor == TipoDocumentoProvedor.NFSe)
                return pagamentoProvedor.ArquivoXMLNFSe;

            return null;
        }

        public void CriarAprovacao(Dominio.Entidades.Embarcador.Financeiro.PagamentoProvedor pagamentoProvedor, Dominio.Entidades.Embarcador.Financeiro.PagamentoProvedorCarga pagamentoProvedorCarga, bool houveDivergenciaEntreCampos, decimal valorTotalProvedorCargas, Repositorio.Embarcador.Financeiro.PagamentoProvedor repositorioPagamento, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            List<Dominio.Entidades.Embarcador.Escrituracao.AlcadasPagamentoProvedor.RegraPagamentoProvedor> regras = ObterRegrasAutorizacao(pagamentoProvedor, pagamentoProvedorCarga, houveDivergenciaEntreCampos, valorTotalProvedorCargas);

            if (regras.Count > 0)
                CriarRegrasAprovacao(pagamentoProvedor, regras, tipoServicoMultisoftware);
            else
            {
                pagamentoProvedor.EtapaLiberacaoPagamentoProvedor = EtapaLiberacaoPagamentoProvedor.Liberacao;
                pagamentoProvedor.SituacaoLiberacaoPagamentoProvedor = SituacaoLiberacaoPagamentoProvedor.Finalizada;

                repositorioPagamento.Atualizar(pagamentoProvedor);
            }
        }

        public bool LiberarProximaPrioridadeAprovacao(Dominio.Entidades.Embarcador.Financeiro.PagamentoProvedor origemAprovacao, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            Repositorio.Embarcador.RegraAutorizacao.AprovacaoAlcada<Dominio.Entidades.Embarcador.Escrituracao.AlcadasPagamentoProvedor.AprovacaoAlcadaPagamentoProvedor, Dominio.Entidades.Embarcador.Escrituracao.AlcadasPagamentoProvedor.RegraPagamentoProvedor, Dominio.Entidades.Embarcador.Financeiro.PagamentoProvedor> repositorioAutorizacao = new Repositorio.Embarcador.RegraAutorizacao.AprovacaoAlcada<Dominio.Entidades.Embarcador.Escrituracao.AlcadasPagamentoProvedor.AprovacaoAlcadaPagamentoProvedor, Dominio.Entidades.Embarcador.Escrituracao.AlcadasPagamentoProvedor.RegraPagamentoProvedor, Dominio.Entidades.Embarcador.Financeiro.PagamentoProvedor>(_unitOfWork);
            List<Dominio.Entidades.Embarcador.Escrituracao.AlcadasPagamentoProvedor.AprovacaoAlcadaPagamentoProvedor> alcadasAprovacao = repositorioAutorizacao.BuscarPendentesBloqueadas(origemAprovacao.Codigo);

            if (alcadasAprovacao.Count > 0)
            {
                int menorPrioridadeAprovacao = alcadasAprovacao.Select(alcada => alcada.RegraAutorizacao.PrioridadeAprovacao).Min();

                foreach (Dominio.Entidades.Embarcador.Escrituracao.AlcadasPagamentoProvedor.AprovacaoAlcadaPagamentoProvedor aprovacao in alcadasAprovacao)
                {
                    if (aprovacao.RegraAutorizacao.PrioridadeAprovacao == menorPrioridadeAprovacao)
                    {
                        aprovacao.Bloqueada = false;
                        repositorioAutorizacao.Atualizar(aprovacao);

                        NotificarAprovador(origemAprovacao, aprovacao, tipoServicoMultisoftware);
                    }
                }

                return false;
            }

            return true;
        }

        #endregion

        #region Métodos Privados

        private void CriarRegrasAprovacao(Dominio.Entidades.Embarcador.Financeiro.PagamentoProvedor pagamentoProvedor, List<Dominio.Entidades.Embarcador.Escrituracao.AlcadasPagamentoProvedor.RegraPagamentoProvedor> regras, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            bool existeRegraSemAprovacao = false;
            Repositorio.Embarcador.Financeiro.AprovacaoAlcadaPagamentoProvedor repositorio = new Repositorio.Embarcador.Financeiro.AprovacaoAlcadaPagamentoProvedor(_unitOfWork);
            int menorPrioridadeAprovacao = regras.Where(regra => regra.NumeroAprovadores > 0).Select(regra => (int?)regra.PrioridadeAprovacao).Min() ?? 0;

            foreach (Dominio.Entidades.Embarcador.Escrituracao.AlcadasPagamentoProvedor.RegraPagamentoProvedor regra in regras)
            {
                if (regra.NumeroAprovadores > 0)
                {
                    existeRegraSemAprovacao = true;

                    foreach (var aprovador in regra.Aprovadores)
                    {
                        Dominio.Entidades.Embarcador.Escrituracao.AlcadasPagamentoProvedor.AprovacaoAlcadaPagamentoProvedor aprovacao = new Dominio.Entidades.Embarcador.Escrituracao.AlcadasPagamentoProvedor.AprovacaoAlcadaPagamentoProvedor()
                        {
                            OrigemAprovacao = pagamentoProvedor,
                            Bloqueada = regra.PrioridadeAprovacao > menorPrioridadeAprovacao,
                            Usuario = aprovador,
                            RegraAutorizacao = regra,
                            Situacao = SituacaoAlcadaRegra.Pendente,
                            DataCriacao = pagamentoProvedor.DataInicial,
                            NumeroAprovadores = regra.NumeroAprovadores
                        };

                        repositorio.Inserir(aprovacao);

                        if (!aprovacao.Bloqueada)
                            NotificarAprovador(pagamentoProvedor, aprovacao, tipoServicoMultisoftware);
                    }
                }
                else
                {
                    Dominio.Entidades.Embarcador.Escrituracao.AlcadasPagamentoProvedor.AprovacaoAlcadaPagamentoProvedor aprovacao = new Dominio.Entidades.Embarcador.Escrituracao.AlcadasPagamentoProvedor.AprovacaoAlcadaPagamentoProvedor()
                    {
                        OrigemAprovacao = pagamentoProvedor,
                        Usuario = null,
                        RegraAutorizacao = regra,
                        Situacao = SituacaoAlcadaRegra.Aprovada,
                        Data = System.DateTime.Now,
                        Motivo = $"Alçada aprovada pela Regra {regra.Descricao}",
                        DataCriacao = pagamentoProvedor.DataInicial,
                    };

                    repositorio.Inserir(aprovacao);
                }
            }

            pagamentoProvedor.EtapaLiberacaoPagamentoProvedor = existeRegraSemAprovacao ? EtapaLiberacaoPagamentoProvedor.Aprovacao : EtapaLiberacaoPagamentoProvedor.Liberacao;

            if (pagamentoProvedor.EtapaLiberacaoPagamentoProvedor == EtapaLiberacaoPagamentoProvedor.Liberacao)
                pagamentoProvedor.SituacaoLiberacaoPagamentoProvedor = SituacaoLiberacaoPagamentoProvedor.Finalizada;
        }

        private List<Dominio.Entidades.Embarcador.Escrituracao.AlcadasPagamentoProvedor.RegraPagamentoProvedor> ObterRegrasAutorizacao(Dominio.Entidades.Embarcador.Financeiro.PagamentoProvedor pagamentoProvedor, Dominio.Entidades.Embarcador.Financeiro.PagamentoProvedorCarga pagamentoProvedorCarga, bool houveDivergenciaEntreCampos, decimal valorTotalProvedorCargas)
        {
            Repositorio.Embarcador.Financeiro.RegraPagamentoProvedor repositorioRegraPagamentoProvedor = new Repositorio.Embarcador.Financeiro.RegraPagamentoProvedor(_unitOfWork);

            List<Dominio.Entidades.Embarcador.Escrituracao.AlcadasPagamentoProvedor.RegraPagamentoProvedor> listaRegras =
                pagamentoProvedor.MultiplosCTe ?
                repositorioRegraPagamentoProvedor.BuscarMultiplosCTe() :
                repositorioRegraPagamentoProvedor.BuscarPorAtivaOuConfiguracao(pagamentoProvedor.TipoDocumentoProvedor);

            List<Dominio.Entidades.Embarcador.Escrituracao.AlcadasPagamentoProvedor.RegraPagamentoProvedor> listaRegrasFiltradas = new List<Dominio.Entidades.Embarcador.Escrituracao.AlcadasPagamentoProvedor.RegraPagamentoProvedor>();

            decimal valor = CalcularValor(pagamentoProvedor, pagamentoProvedorCarga, valorTotalProvedorCargas);

            foreach (Dominio.Entidades.Embarcador.Escrituracao.AlcadasPagamentoProvedor.RegraPagamentoProvedor regra in listaRegras)
            {
                if (regra.RegraPorDiferencaValor && !ValidarAlcadas<Dominio.Entidades.Embarcador.Escrituracao.AlcadasPagamentoProvedor.AlcadasDiferencaValor, decimal>(regra.AlcadasDiferencaValor, Math.Abs(valor)))
                    continue;

                if (regra.RegraPorDiferencaValorMaior && valor > 0)
                    continue;

                if (valor <= 0 && regra.RegraPorDiferencaValorMaior && !ValidarAlcadas<Dominio.Entidades.Embarcador.Escrituracao.AlcadasPagamentoProvedor.AlcadasDiferencaValorMaior, decimal>(regra.AlcadasDiferencaValorMaior, Math.Abs(valor)))
                    continue;

                if (regra.RegraPorDiferencaValorMenor && valor < 0)
                    continue;

                if (valor >= 0 && regra.RegraPorDiferencaValorMenor && !ValidarAlcadas<Dominio.Entidades.Embarcador.Escrituracao.AlcadasPagamentoProvedor.AlcadasDiferencaValorMenor, decimal>(regra.AlcadasDiferencaValorMenor, valor))
                    continue;

                if (regra.ValidarTodosCamposAuditoriaDocumentoProvedor && !houveDivergenciaEntreCampos)
                    continue;

                if (regra.BloquearPagamentoMultiplosCTe && !pagamentoProvedor.MultiplosCTe)
                    continue;

                listaRegrasFiltradas.Add(regra);
            }

            return listaRegrasFiltradas;
        }

        private decimal CalcularValor(PagamentoProvedor pagamentoProvedor, Dominio.Entidades.Embarcador.Financeiro.PagamentoProvedorCarga pagamentoProvedorCarga, decimal valorTotalProvedorCargas)
        {
            decimal valor = 0;

            if (pagamentoProvedorCarga?.PagamentoProvedor == null)
                throw new Exception("Registro não encontrado para localizar a regra.");

            if (pagamentoProvedor.TipoDocumentoProvedor == TipoDocumentoProvedor.NFSe)
                valor = valorTotalProvedorCargas - pagamentoProvedor.ValorProvedor;
            else if (pagamentoProvedor.TipoDocumentoProvedor == TipoDocumentoProvedor.CTe || pagamentoProvedor.TipoDocumentoProvedor == TipoDocumentoProvedor.CTeComplementar)
            {
                if (!pagamentoProvedor.MultiplosCTe)
                {
                    Dominio.Entidades.Embarcador.Integracao.ArquivoIntegracao arquivoIntegracao = ObterArquivoIntegracao(pagamentoProvedorCarga.PagamentoProvedor);

                    Dominio.ObjetosDeValor.Embarcador.CTe.CTe cte = LerCTeDeArquivo(arquivoIntegracao);

                    valor = (pagamentoProvedorCarga.Carga?.ValorTotalProvedor ?? 0) - pagamentoProvedor.ValorAReceberCTe;
                }
                else
                {
                    valor = (pagamentoProvedorCarga.Carga?.ValorTotalProvedor ?? 0) - pagamentoProvedor.ValorCTes;
                }
            }

            return valor;
        }

        #endregion

        #region Métodos Protegidos Sobrescritos

        protected override void NotificarAprovador(Dominio.Entidades.Embarcador.Financeiro.PagamentoProvedor pagamentoProvedor, Dominio.Entidades.Embarcador.Escrituracao.AlcadasPagamentoProvedor.AprovacaoAlcadaPagamentoProvedor aprovacao, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            Notificacao.Notificacao servicoNotificacao = new(_unitOfWork.StringConexao, cliente: null, tipoServicoMultisoftware: tipoServicoMultisoftware, adminStringConexao: string.Empty);
            StringBuilder nota = new(string.Format(Localization.Resources.Escrituracao.PagamentoAprovacao.CriadaSolicitacaoLiberacaoPagamento, pagamentoProvedor.NumeroNFSe));

            if (!string.IsNullOrWhiteSpace(_urlAcesso))
                nota.AppendLine(".").AppendLine().Append($"{(Configuracoes.Ambiente.Seguro(_unitOfWork) ? "https" : "http")}://{_urlAcesso}");

            servicoNotificacao.GerarNotificacaoEmail(
                usuario: aprovacao.Usuario,
                usuarioGerouNotificacao: null,
                codigoObjeto: pagamentoProvedor.Codigo,
                URLPagina: "Escrituracao/Pagamento",
                titulo: Localization.Resources.Escrituracao.PagamentoAprovacao.Pagamento,
                nota: nota.ToString(),
                icone: IconesNotificacao.cifra,
                tipoNotificacao: TipoNotificacao.credito,
                tipoServicoMultisoftwareNotificar: tipoServicoMultisoftware,
                unitOfWork: _unitOfWork
            );
        }

        #endregion
    }
}
