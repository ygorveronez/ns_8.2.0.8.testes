using System.Collections.Generic;

namespace Servicos.Embarcador.ConfiguracaoContabil
{
    public class ConfiguracaoNaturezaOperacao
    {
        public Dominio.Entidades.Embarcador.ConfiguracaoContabil.ConfiguracaoNaturezaOperacao ObterConfiguracaoNaturezaOperacao(Dominio.Entidades.Cliente remetente, Dominio.Entidades.Cliente destinatario, Dominio.Entidades.Cliente tomador, Dominio.Entidades.ModeloDocumentoFiscal modeloDocumento, Dominio.Entidades.Estado ufOrigem, Dominio.Entidades.Estado ufDestino, Dominio.Entidades.Embarcador.Produtos.GrupoProduto grupoProduto, Dominio.Entidades.Embarcador.Pedidos.TipoOperacao tipoOperacao, Dominio.Entidades.RotaFrete rotaFrete, Dominio.Entidades.Empresa empresa, Dominio.Entidades.Empresa empresaEmissora, Repositorio.UnitOfWork unitOfWork)
        {
            Dominio.Entidades.Embarcador.ConfiguracaoContabil.ConfiguracaoNaturezaOperacao ConfiguracaoNaturezaOperacao = null;
            int grupoRemetente = remetente?.GrupoPessoas?.Codigo ?? 0;
            int grupoDestinatario = destinatario?.GrupoPessoas?.Codigo ?? 0;
            int grupoTomador = tomador?.GrupoPessoas?.Codigo ?? 0;
            int codigoGrupoProduto = grupoProduto?.Codigo ?? 0;
            int codigotipoOperacao = tipoOperacao?.Codigo ?? 0;
            int codigoRotaFrete = rotaFrete?.Codigo ?? 0;
            int codigoEmpresa = empresa?.Codigo ?? 0;
            int atividadeRemetente = remetente?.Atividade.Codigo ?? 0;
            int atividadeDestinatario = destinatario?.Atividade.Codigo ?? 0;
            int atividadeTomador = tomador?.Atividade.Codigo ?? 0;
            double cnpjRemetente = remetente?.CPF_CNPJ ?? 0;
            double cnpjDestinatario = destinatario?.CPF_CNPJ ?? 0;
            double cnpjTomador = tomador?.CPF_CNPJ ?? 0;
            string siglaOrigem = ufOrigem?.Sigla ?? "";
            string siglaDestino = ufDestino?.Sigla ?? "";
            int codigoCategoriaDestinatario = destinatario?.Categoria?.Codigo ?? 0;
            int codigoCategoriaRemetente = remetente?.Categoria?.Codigo ?? 0;
            int codigoCategoriaTomador = tomador?.Categoria?.Codigo ?? 0;

            Repositorio.Embarcador.ConfiguracaoContabil.ConfiguracaoNaturezaOperacao repConfiguracaoNaturezaOperacao = new Repositorio.Embarcador.ConfiguracaoContabil.ConfiguracaoNaturezaOperacao(unitOfWork);

            //int configuracoesCadastradas = repConfiguracaoNaturezaOperacao.ContarRegrasCadastradas();

            //if (configuracoesCadastradas > 5)
            //{
            if (empresa != null)
            {
                List<Dominio.Entidades.Embarcador.ConfiguracaoContabil.ConfiguracaoNaturezaOperacao> configuracoesEmpresa = repConfiguracaoNaturezaOperacao.FiltrarPorEmpresa(empresa.Codigo);
                if (configuracoesEmpresa.Count > 0)
                    ConfiguracaoNaturezaOperacao = RetornarConfiguracaoValida(configuracoesEmpresa, atividadeRemetente, atividadeDestinatario, atividadeTomador, cnpjRemetente, cnpjDestinatario, cnpjTomador, empresa, empresaEmissora, grupoDestinatario, grupoRemetente, grupoTomador, siglaOrigem, siglaDestino, modeloDocumento?.Codigo ?? 0, codigoGrupoProduto, codigotipoOperacao, codigoRotaFrete, codigoCategoriaDestinatario, codigoCategoriaRemetente, codigoCategoriaTomador);
            }

            if (ConfiguracaoNaturezaOperacao == null)
            {
                List<Dominio.Entidades.Embarcador.ConfiguracaoContabil.ConfiguracaoNaturezaOperacao> configuracoesParticipantes = repConfiguracaoNaturezaOperacao.FiltrarPorParticipantes(cnpjRemetente, cnpjDestinatario, cnpjTomador, grupoRemetente, grupoDestinatario, grupoTomador, codigoCategoriaDestinatario, codigoCategoriaRemetente, codigoCategoriaTomador);
                if (configuracoesParticipantes.Count > 0)
                    ConfiguracaoNaturezaOperacao = RetornarConfiguracaoValida(configuracoesParticipantes, atividadeRemetente, atividadeDestinatario, atividadeTomador, cnpjRemetente, cnpjDestinatario, cnpjTomador, empresa, empresaEmissora, grupoDestinatario, grupoRemetente, grupoTomador, siglaOrigem, siglaDestino, modeloDocumento?.Codigo ?? 0, codigoGrupoProduto, codigotipoOperacao, codigoRotaFrete, codigoCategoriaDestinatario, codigoCategoriaRemetente, codigoCategoriaTomador);
            }

            if (ConfiguracaoNaturezaOperacao == null && grupoProduto != null)
            {
                List<Dominio.Entidades.Embarcador.ConfiguracaoContabil.ConfiguracaoNaturezaOperacao> configuracoesGrupoProduto = repConfiguracaoNaturezaOperacao.FiltrarPorGrupoProdutoEmbarcador(codigoGrupoProduto);
                if (configuracoesGrupoProduto.Count > 0)
                    ConfiguracaoNaturezaOperacao = RetornarConfiguracaoValida(configuracoesGrupoProduto, atividadeRemetente, atividadeDestinatario, atividadeTomador, cnpjRemetente, cnpjDestinatario, cnpjTomador, empresa, empresaEmissora, grupoDestinatario, grupoRemetente, grupoTomador, siglaOrigem, siglaDestino, modeloDocumento?.Codigo ?? 0, codigoGrupoProduto, codigotipoOperacao, codigoRotaFrete, codigoCategoriaDestinatario, codigoCategoriaRemetente, codigoCategoriaTomador);
            }

            if (ConfiguracaoNaturezaOperacao == null && rotaFrete != null)
            {
                List<Dominio.Entidades.Embarcador.ConfiguracaoContabil.ConfiguracaoNaturezaOperacao> configuracoesProdutoEmbarcador = repConfiguracaoNaturezaOperacao.FiltrarPorRotaFrete(codigoRotaFrete);
                if (configuracoesProdutoEmbarcador.Count > 0)
                    ConfiguracaoNaturezaOperacao = RetornarConfiguracaoValida(configuracoesProdutoEmbarcador, atividadeRemetente, atividadeDestinatario, atividadeTomador, cnpjRemetente, cnpjDestinatario, cnpjTomador, empresa, empresaEmissora, grupoDestinatario, grupoRemetente, grupoTomador, siglaOrigem, siglaDestino, modeloDocumento?.Codigo ?? 0, codigoGrupoProduto, codigotipoOperacao, codigoRotaFrete, codigoCategoriaDestinatario, codigoCategoriaRemetente, codigoCategoriaTomador);
            }


            if (ConfiguracaoNaturezaOperacao == null)
            {
                List<Dominio.Entidades.Embarcador.ConfiguracaoContabil.ConfiguracaoNaturezaOperacao> configuracoesPorEstado = repConfiguracaoNaturezaOperacao.FiltrarPorEstados(siglaOrigem, siglaDestino);
                if (configuracoesPorEstado.Count > 0)
                    ConfiguracaoNaturezaOperacao = RetornarConfiguracaoValida(configuracoesPorEstado, atividadeRemetente, atividadeDestinatario, atividadeTomador, cnpjRemetente, cnpjDestinatario, cnpjTomador, empresa, empresaEmissora, grupoDestinatario, grupoRemetente, grupoTomador, siglaOrigem, siglaDestino, modeloDocumento?.Codigo ?? 0, codigoGrupoProduto, codigotipoOperacao, codigoRotaFrete, codigoCategoriaDestinatario, codigoCategoriaRemetente, codigoCategoriaTomador);
            }


            if (ConfiguracaoNaturezaOperacao == null)
            {
                List<Dominio.Entidades.Embarcador.ConfiguracaoContabil.ConfiguracaoNaturezaOperacao> configuracoesPorEstadoRegraGlobal = repConfiguracaoNaturezaOperacao.FiltrarPorEstadosRegraGlobal(siglaOrigem, siglaDestino);
                if (configuracoesPorEstadoRegraGlobal.Count > 0)
                    ConfiguracaoNaturezaOperacao = RetornarConfiguracaoValida(configuracoesPorEstadoRegraGlobal, atividadeRemetente, atividadeDestinatario, atividadeTomador, cnpjRemetente, cnpjDestinatario, cnpjTomador, empresa, empresaEmissora, grupoDestinatario, grupoRemetente, grupoTomador, siglaOrigem, siglaDestino, modeloDocumento?.Codigo ?? 0, codigoGrupoProduto, codigotipoOperacao, codigoRotaFrete, codigoCategoriaDestinatario, codigoCategoriaRemetente, codigoCategoriaTomador);
            }


            if (ConfiguracaoNaturezaOperacao == null)
            {
                List<Dominio.Entidades.Embarcador.ConfiguracaoContabil.ConfiguracaoNaturezaOperacao> configuracoesPorAtividade = repConfiguracaoNaturezaOperacao.FiltrarPorAtividade(atividadeRemetente, atividadeDestinatario, atividadeTomador);
                if (configuracoesPorAtividade.Count > 0)
                    ConfiguracaoNaturezaOperacao = RetornarConfiguracaoValida(configuracoesPorAtividade, atividadeRemetente, atividadeDestinatario, atividadeTomador, cnpjRemetente, cnpjDestinatario, cnpjTomador, empresa, empresaEmissora, grupoDestinatario, grupoRemetente, grupoTomador, siglaOrigem, siglaDestino, modeloDocumento?.Codigo ?? 0, codigoGrupoProduto, codigotipoOperacao, codigoRotaFrete, codigoCategoriaDestinatario, codigoCategoriaRemetente, codigoCategoriaTomador);
            }

            if (ConfiguracaoNaturezaOperacao == null)
            {
                List<Dominio.Entidades.Embarcador.ConfiguracaoContabil.ConfiguracaoNaturezaOperacao> configuracoesProdutoEmbarcador = repConfiguracaoNaturezaOperacao.FiltrarPorTipoOperacao(codigotipoOperacao);
                if (configuracoesProdutoEmbarcador.Count > 0)
                    ConfiguracaoNaturezaOperacao = RetornarConfiguracaoValida(configuracoesProdutoEmbarcador, atividadeRemetente, atividadeDestinatario, atividadeTomador, cnpjRemetente, cnpjDestinatario, cnpjTomador, empresa, empresaEmissora, grupoDestinatario, grupoRemetente, grupoTomador, siglaOrigem, siglaDestino, modeloDocumento?.Codigo ?? 0, codigoGrupoProduto, codigotipoOperacao, codigoRotaFrete, codigoCategoriaDestinatario, codigoCategoriaRemetente, codigoCategoriaTomador);
            }

            //}
            //else
            //{
            //    if (configuracoesCadastradas > 0)
            //    {
            //        List<Dominio.Entidades.Embarcador.ConfiguracaoContabil.ConfiguracaoNaturezaOperacao> configuracoes = repConfiguracaoNaturezaOperacao.FiltrarTodasRegrasCadastradas();
            //        if (configuracoes.Count > 0)
            //            ConfiguracaoNaturezaOperacao = RetornarConfiguracaoValida(configuracoes, atividadeRemetente, atividadeDestinatario, atividadeTomador, cnpjRemetente, cnpjDestinatario, cnpjTomador, empresa, grupoDestinatario, grupoRemetente, grupoTomador, siglaOrigem, siglaDestino, modeloDocumento?.Codigo ?? 0, codigoGrupoProduto, codigotipoOperacao, codigoRotaFrete);
            //    }
            //}
            return ConfiguracaoNaturezaOperacao;
        }


        private Dominio.Entidades.Embarcador.ConfiguracaoContabil.ConfiguracaoNaturezaOperacao RetornarConfiguracaoValida(List<Dominio.Entidades.Embarcador.ConfiguracaoContabil.ConfiguracaoNaturezaOperacao> configuracoes, int atividadeRemetente, int atividadeDestinatario, int atividadeTomador, double remetente, double destinatario, double tomador, Dominio.Entidades.Empresa empresa, Dominio.Entidades.Empresa empresaEmissora, int grupoDestinatario, int grupoRemetente, int grupoTomador, string ufOrigem, string ufDestino, int modeloDocumento, int codigoGrupoProduto, int codigoTipoOperacao, int codigoRotaFrete, int codigoCategoriaDestinatario, int codigoCategoriaRemetente, int codigoCategoriaTomador)
        {
            Dominio.Entidades.Embarcador.ConfiguracaoContabil.ConfiguracaoNaturezaOperacao configuracaoValida = null;
            foreach (Dominio.Entidades.Embarcador.ConfiguracaoContabil.ConfiguracaoNaturezaOperacao configuracao in configuracoes)
            {
                bool valido = true;
                if (configuracao.Destinatario != null && configuracao.Destinatario.CPF_CNPJ != destinatario)
                    valido = false;
                else if (configuracao.Empresa != null && (empresa == null || configuracao.Empresa.Codigo != empresa.Codigo))
                    valido = false;
                else if (configuracao.Remetente != null && configuracao.Remetente.CPF_CNPJ != remetente)
                    valido = false;
                else if (configuracao.Tomador != null && configuracao.Tomador.CPF_CNPJ != tomador)
                    valido = false;
                else if (configuracao.GrupoDestinatario != null && configuracao.GrupoDestinatario.Codigo != grupoDestinatario)
                    valido = false;
                else if (configuracao.GrupoRemetente != null && configuracao.GrupoRemetente.Codigo != grupoRemetente)
                    valido = false;
                else if (configuracao.GrupoTomador != null && configuracao.GrupoTomador.Codigo != grupoTomador)
                    valido = false;
                else if (configuracao.CategoriaDestinatario != null && configuracao.CategoriaDestinatario.Codigo != codigoCategoriaDestinatario)
                    valido = false;
                else if (configuracao.CategoriaRemetente != null && configuracao.CategoriaRemetente.Codigo != codigoCategoriaRemetente)
                    valido = false;
                else if (configuracao.CategoriaTomador != null && configuracao.CategoriaTomador.Codigo != codigoCategoriaTomador)
                    valido = false;
                if (configuracao.AtividadeDestinatario != null && configuracao.AtividadeDestinatario.Codigo != atividadeDestinatario)
                    valido = false;
                else if (configuracao.AtividadeRemetente != null && configuracao.AtividadeRemetente.Codigo != atividadeRemetente)
                    valido = false;
                else if (configuracao.AtividadeTomador != null && configuracao.AtividadeTomador.Codigo != atividadeTomador)
                    valido = false;
                else if (configuracao.GrupoProduto != null && configuracao.GrupoProduto.Codigo != codigoGrupoProduto)
                    valido = false;
                else if (configuracao.TipoOperacao != null && configuracao.TipoOperacao.Codigo != codigoTipoOperacao)
                    valido = false;
                else if (configuracao.EstadoOrigemDiferenteUFDestino && ufDestino == ufOrigem)
                    valido = false;
                else if (configuracao.EstadoOrigemIgualUFDestino && ufDestino != ufOrigem)
                    valido = false;
                else if (configuracao.ModeloDocumentoFiscal != null && configuracao.ModeloDocumentoFiscal.Codigo != modeloDocumento)
                    valido = false;
                else if (configuracao.RotaFrete != null && configuracao.RotaFrete.Codigo != codigoRotaFrete)
                    valido = false;
                else if (!configuracao.EstadoDestinoDiferente ? (configuracao.UFDestino != null && configuracao.UFDestino.Sigla != ufDestino) : (configuracao.UFDestino != null && configuracao.UFDestino.Sigla == ufDestino)) ///else if (regra.UFDestino != null && regra.UFDestino.Sigla != destino.Estado.Sigla)
                    valido = false;
                else if (!configuracao.EstadoOrigemDiferente ? (configuracao.UFOrigem != null && configuracao.UFOrigem.Sigla != ufOrigem) : (configuracao.UFOrigem != null && configuracao.UFOrigem.Sigla == ufOrigem))//else if (regra.UFOrigem != null && regra.UFOrigem.Sigla != origem.Estado.Sigla)
                    valido = false;
                else if (configuracao.EstadoEmissorDiferenteUFOrigem && (empresaEmissora == null || ufOrigem == empresaEmissora.Localidade.Estado.Sigla))
                    valido = false;

                if (valido)
                {
                    configuracaoValida = configuracao;
                    break;
                }

            }
            return configuracaoValida;
        }

    }
}
