using Dominio.Excecoes.Embarcador;
using System;
using System.IO;
using Dominio.ObjetosDeValor.Relatorios;
using Servicos.Extensions;

namespace Servicos.Embarcador.Financeiro
{
    public class TermoQuitacaoControleDocumento : ServicoBase
    {        
        #region Construtores

        public TermoQuitacaoControleDocumento(Repositorio.UnitOfWork unitOfWork) : base(unitOfWork) { }

        #endregion

        #region Métodos Públicos

        public void AtualizarPDF(Dominio.Entidades.Embarcador.Financeiro.TermoQuitacaoFinanceiro termo)
        {
            Repositorio.Embarcador.Financeiro.TermoQuitacaoControleDocumento repDocumento = new Repositorio.Embarcador.Financeiro.TermoQuitacaoControleDocumento(_unitOfWork);


            string caminhoRaiz = Configuracoes.ConfigurationInstance.GetInstance(_unitOfWork).ObterConfiguracaoArquivo().Anexos;
            string caminho = Utilidades.IO.FileStorageService.Storage.Combine(caminhoRaiz, "TermoQuitacaoFinanceiro", "ControleDocumentos");

            string nomeArquivo = $"{Guid.NewGuid().ToString().Replace("_", "")}";
            string nomeCompletoArquivo = Utilidades.IO.FileStorageService.Storage.Combine(caminho, $"{nomeArquivo}.pdf");

            DeletarAntigos(termo, caminho);

            Dominio.Entidades.Embarcador.Financeiro.TermoQuitacaoControleDocumento documento = new Dominio.Entidades.Embarcador.Financeiro.TermoQuitacaoControleDocumento()
            {
                Descricao = string.Empty,
                NomeArquivo = $"Termo de Quitação {termo.Codigo}",
                GuidArquivo = nomeArquivo,
                EntidadeAnexo = termo,
            };

            try
            {
                if (Utilidades.IO.FileStorageService.Storage.Exists(nomeCompletoArquivo))
                    throw new ServicoException($"Já existe um arquivo com o mesmo nome");

                byte[] pdf = ObterArrayPDF(termo);
                Utilidades.IO.FileStorageService.Storage.WriteAllBytes(nomeCompletoArquivo, pdf);

                repDocumento.Inserir(documento);
            }
            catch (Exception ex)
            {
                Utilidades.IO.FileStorageService.Storage.Delete(nomeCompletoArquivo);
                Servicos.Log.TratarErro(ex);
            }
        }

        #endregion

        private byte[] ObterArrayPDF(Dominio.Entidades.Embarcador.Financeiro.TermoQuitacaoFinanceiro termoQuitacao)
        {
            return ReportRequest.WithType(ReportType.TermoQuitacao)
                .WithExecutionType(ExecutionType.Sync)
                .AddExtraData("CodigoTermoQuitacao", termoQuitacao.Codigo.ToString())
                .CallReport()
                .GetContentFile();
        }

        private void DeletarAntigos(Dominio.Entidades.Embarcador.Financeiro.TermoQuitacaoFinanceiro termo, string caminho)
        {
            Repositorio.Embarcador.Financeiro.TermoQuitacaoControleDocumento repDocumento = new Repositorio.Embarcador.Financeiro.TermoQuitacaoControleDocumento(_unitOfWork);

            Dominio.Entidades.Embarcador.Financeiro.TermoQuitacaoControleDocumento documentoDeletar = repDocumento.BuscarAnexoPorEntidade(termo.Codigo);

            if (documentoDeletar != null)
            {
                string caminhoArquivo = Utilidades.IO.FileStorageService.Storage.Combine(caminho, $"{documentoDeletar.GuidArquivo}.pdf");
                
                if (Utilidades.IO.FileStorageService.Storage.Exists(caminhoArquivo))
                    Utilidades.IO.FileStorageService.Storage.Delete(caminhoArquivo);

                repDocumento.Deletar(documentoDeletar);
            }
        }
    }
}
