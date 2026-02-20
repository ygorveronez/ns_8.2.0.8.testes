using System.Collections.Generic;
using System.Linq;

namespace Servicos.Embarcador.ConfiguracaoContabil
{
    public class ConfiguracaoCentroResultado
    {
        public Dominio.Entidades.Embarcador.ConfiguracaoContabil.ConfiguracaoCentroResultado ObterConfiguracaoCentroResultado(Dominio.Entidades.Cliente remetente, Dominio.Entidades.Cliente destinatario, Dominio.Entidades.Cliente expedidor, Dominio.Entidades.Cliente recebedor, Dominio.Entidades.Cliente tomador, Dominio.Entidades.Embarcador.Produtos.GrupoProduto grupoProduto, Dominio.Entidades.Empresa empresaLayout, Dominio.Entidades.Empresa empresa, Dominio.Entidades.Embarcador.Pedidos.TipoOperacao tipoOperacao, Dominio.Entidades.TipoDeOcorrenciaDeCTe tipoDeOcorrencia, Dominio.Entidades.RotaFrete rotaFrete, Dominio.Entidades.Embarcador.Filiais.Filial filial, Dominio.Entidades.Localidade origem, Repositorio.UnitOfWork unitOfWork)
        {


            Servicos.Embarcador.ConfiguracaoContabil.RegraConfiguracaoCentroResultado regraConfiguracaoCentroResultado = Servicos.Embarcador.ConfiguracaoContabil.RegraConfiguracaoCentroResultado.GetInstance(unitOfWork);
            if (regraConfiguracaoCentroResultado?.ConfiguracaoCentroResultado == null)
                return null;

            IQueryable<Dominio.Entidades.Embarcador.ConfiguracaoContabil.ConfiguracaoCentroResultado> queryListaConfiguracoes = regraConfiguracaoCentroResultado.ConfiguracaoCentroResultado.AsQueryable();


            Dominio.Entidades.Embarcador.ConfiguracaoContabil.ConfiguracaoCentroResultado ConfiguracaoCentroResultado = null;
            int grupoRemetente = remetente?.GrupoPessoas?.Codigo ?? 0;
            int grupoDestinatario = destinatario?.GrupoPessoas?.Codigo ?? 0;
            int grupoTomador = tomador?.GrupoPessoas?.Codigo ?? 0;
            int codigoGrupoProduto = grupoProduto?.Codigo ?? 0;
            int codigotipoOperacao = tipoOperacao?.Codigo ?? 0;
            int codigoRotaFrete = rotaFrete?.Codigo ?? 0;
            int codigoTipoOcorrencia = tipoDeOcorrencia?.Codigo ?? 0;
            int codigoEmpresaLayout = empresaLayout?.Codigo ?? 0;
            int codigoEmpresa = empresa?.Codigo ?? 0;
            double cnpjRemetente = remetente?.CPF_CNPJ ?? 0;
            double cnpjDestinatario = destinatario?.CPF_CNPJ ?? 0;
            double cnpjExpedidor = expedidor?.CPF_CNPJ ?? 0;
            double cnpjRecebedor = recebedor?.CPF_CNPJ ?? 0;
            double cnpjTomador = tomador?.CPF_CNPJ ?? 0;
            int codigoCategoriaDestinatario = destinatario?.Categoria?.Codigo ?? 0;
            int codigoCategoriaRemetente = remetente?.Categoria?.Codigo ?? 0;
            int codigoCategoriaTomador = tomador?.Categoria?.Codigo ?? 0;
            int codigoFilial = filial?.Codigo ?? 0;
            int codigoOrigem = origem?.Codigo ?? 0;

            Repositorio.Embarcador.ConfiguracaoContabil.ConfiguracaoCentroResultado repConfiguracaoCentroResultado = new Repositorio.Embarcador.ConfiguracaoContabil.ConfiguracaoCentroResultado(unitOfWork);

            if (codigoTipoOcorrencia > 0)
            {
                List<Dominio.Entidades.Embarcador.ConfiguracaoContabil.ConfiguracaoCentroResultado> configuracoesParticipantesEmpresaLayout = repConfiguracaoCentroResultado.FiltrarPorTipoOcorrencia(codigoTipoOcorrencia, queryListaConfiguracoes);
                if (configuracoesParticipantesEmpresaLayout.Count > 0)
                    ConfiguracaoCentroResultado = RetornarConfiguracaoValida(configuracoesParticipantesEmpresaLayout, cnpjRemetente, cnpjDestinatario, cnpjExpedidor, cnpjRecebedor, cnpjTomador, grupoDestinatario, grupoRemetente, grupoTomador, codigoGrupoProduto, codigoEmpresaLayout, codigoEmpresa, codigotipoOperacao, codigoRotaFrete, codigoCategoriaDestinatario, codigoCategoriaRemetente, codigoCategoriaTomador, codigoTipoOcorrencia, codigoFilial, codigoOrigem);
            }


            if (ConfiguracaoCentroResultado == null)
            {
                List<Dominio.Entidades.Embarcador.ConfiguracaoContabil.ConfiguracaoCentroResultado> configuracoesParticipantesEmpresaLayout = repConfiguracaoCentroResultado.FiltrarPorParticipantesEmpresa(codigoEmpresaLayout, cnpjRemetente, cnpjDestinatario, cnpjExpedidor, cnpjRecebedor, cnpjTomador, grupoRemetente, grupoDestinatario, grupoTomador, codigoCategoriaDestinatario, codigoCategoriaRemetente, codigoCategoriaTomador, queryListaConfiguracoes);
                if (configuracoesParticipantesEmpresaLayout.Count > 0)
                    ConfiguracaoCentroResultado = RetornarConfiguracaoValida(configuracoesParticipantesEmpresaLayout, cnpjRemetente, cnpjDestinatario, cnpjExpedidor, cnpjRecebedor, cnpjTomador, grupoDestinatario, grupoRemetente, grupoTomador, codigoGrupoProduto, codigoEmpresaLayout, codigoEmpresa, codigotipoOperacao, codigoRotaFrete, codigoCategoriaDestinatario, codigoCategoriaRemetente, codigoCategoriaTomador, codigoTipoOcorrencia, codigoFilial, codigoOrigem);
            }

            List<Dominio.Entidades.Embarcador.ConfiguracaoContabil.ConfiguracaoCentroResultado> configuracoesEmpresa = repConfiguracaoCentroResultado.FiltrarPorEmpresa(codigoEmpresaLayout, queryListaConfiguracoes);
            if (configuracoesEmpresa.Count > 0)
                ConfiguracaoCentroResultado = RetornarConfiguracaoValida(configuracoesEmpresa, cnpjRemetente, cnpjDestinatario, cnpjExpedidor, cnpjRecebedor, cnpjTomador, grupoDestinatario, grupoRemetente, grupoTomador, codigoGrupoProduto, codigoEmpresaLayout, codigoEmpresa, codigotipoOperacao, codigoRotaFrete, codigoCategoriaDestinatario, codigoCategoriaRemetente, codigoCategoriaTomador, codigoTipoOcorrencia, codigoFilial, codigoOrigem);



            if (ConfiguracaoCentroResultado == null)
            {
                List<Dominio.Entidades.Embarcador.ConfiguracaoContabil.ConfiguracaoCentroResultado> configuracoesParticipantes = repConfiguracaoCentroResultado.FiltrarPorParticipantes(cnpjRemetente, cnpjDestinatario, cnpjExpedidor, cnpjRecebedor, cnpjTomador, 0, 0, 0, 0, 0, 0, queryListaConfiguracoes);
                if (configuracoesParticipantes.Count > 0)
                    ConfiguracaoCentroResultado = RetornarConfiguracaoValida(configuracoesParticipantes, cnpjRemetente, cnpjDestinatario, cnpjExpedidor, cnpjRecebedor, cnpjTomador, grupoDestinatario, grupoRemetente, grupoTomador, codigoGrupoProduto, codigoEmpresaLayout, codigoEmpresa, codigotipoOperacao, codigoRotaFrete, codigoCategoriaDestinatario, codigoCategoriaRemetente, codigoCategoriaTomador, codigoTipoOcorrencia, codigoFilial, codigoOrigem);
            }

            if (ConfiguracaoCentroResultado == null)
            {
                List<Dominio.Entidades.Embarcador.ConfiguracaoContabil.ConfiguracaoCentroResultado> configuracoesParticipantes = repConfiguracaoCentroResultado.FiltrarPorParticipantes(cnpjRemetente, cnpjDestinatario, cnpjExpedidor, cnpjRecebedor, cnpjTomador, grupoRemetente, grupoDestinatario, grupoTomador, codigoCategoriaDestinatario, codigoCategoriaRemetente, codigoCategoriaTomador, queryListaConfiguracoes);
                if (configuracoesParticipantes.Count > 0)
                    ConfiguracaoCentroResultado = RetornarConfiguracaoValida(configuracoesParticipantes, cnpjRemetente, cnpjDestinatario, cnpjExpedidor, cnpjRecebedor, cnpjTomador, grupoDestinatario, grupoRemetente, grupoTomador, codigoGrupoProduto, codigoEmpresaLayout, codigoEmpresa, codigotipoOperacao, codigoRotaFrete, codigoCategoriaDestinatario, codigoCategoriaRemetente, codigoCategoriaTomador, codigoTipoOcorrencia, codigoFilial, codigoOrigem);
            }

            if (ConfiguracaoCentroResultado == null)
            {
                List<Dominio.Entidades.Embarcador.ConfiguracaoContabil.ConfiguracaoCentroResultado> configuracoesAtividade = repConfiguracaoCentroResultado.FiltrarPorGrupoProdutoEmbarcador(codigoGrupoProduto, queryListaConfiguracoes);
                if (configuracoesAtividade.Count > 0)
                    ConfiguracaoCentroResultado = RetornarConfiguracaoValida(configuracoesAtividade, cnpjRemetente, cnpjDestinatario, cnpjExpedidor, cnpjRecebedor, cnpjTomador, grupoDestinatario, grupoRemetente, grupoTomador, codigoGrupoProduto, codigoEmpresaLayout, codigoEmpresa, codigotipoOperacao, codigoRotaFrete, codigoCategoriaDestinatario, codigoCategoriaRemetente, codigoCategoriaTomador, codigoTipoOcorrencia, codigoFilial, codigoOrigem);
            }

            if (ConfiguracaoCentroResultado == null)
            {
                List<Dominio.Entidades.Embarcador.ConfiguracaoContabil.ConfiguracaoCentroResultado> configuracoesProdutoEmbarcador = repConfiguracaoCentroResultado.FiltrarPorRotaFrete(codigoRotaFrete, queryListaConfiguracoes);
                if (configuracoesProdutoEmbarcador.Count > 0)
                    ConfiguracaoCentroResultado = RetornarConfiguracaoValida(configuracoesProdutoEmbarcador, cnpjRemetente, cnpjDestinatario, cnpjExpedidor, cnpjRecebedor, cnpjTomador, grupoDestinatario, grupoRemetente, grupoTomador, codigoGrupoProduto, codigoEmpresaLayout, codigoEmpresa, codigotipoOperacao, codigoRotaFrete, codigoCategoriaDestinatario, codigoCategoriaRemetente, codigoCategoriaTomador, codigoTipoOcorrencia, codigoFilial, codigoOrigem);
            }

            if (ConfiguracaoCentroResultado == null)
            {
                List<Dominio.Entidades.Embarcador.ConfiguracaoContabil.ConfiguracaoCentroResultado> configuracoesProdutoEmbarcador = repConfiguracaoCentroResultado.FiltrarPorTipoOperacao(codigotipoOperacao, queryListaConfiguracoes);
                if (configuracoesProdutoEmbarcador.Count > 0)
                    ConfiguracaoCentroResultado = RetornarConfiguracaoValida(configuracoesProdutoEmbarcador, cnpjRemetente, cnpjDestinatario, cnpjExpedidor, cnpjRecebedor, cnpjTomador, grupoDestinatario, grupoRemetente, grupoTomador, codigoGrupoProduto, codigoEmpresaLayout, codigoEmpresa, codigotipoOperacao, codigoRotaFrete, codigoCategoriaDestinatario, codigoCategoriaRemetente, codigoCategoriaTomador, codigoTipoOcorrencia, codigoFilial, codigoOrigem);
            }
            return ConfiguracaoCentroResultado;
        }


        private Dominio.Entidades.Embarcador.ConfiguracaoContabil.ConfiguracaoCentroResultado RetornarConfiguracaoValida(List<Dominio.Entidades.Embarcador.ConfiguracaoContabil.ConfiguracaoCentroResultado> configuracoes, double remetente, double destinatario, double expedidor, double recebedor, double tomador, int grupoDestinatario, int grupoRemetente, int grupoTomador, int codigoGrupoProduto, int codigoEmpresaLayout, int codigoEmpresa, int codigoTipoOperacao, int codigoRotaFrete, int codigoCategoriaDestinatario, int codigoCategoriaRemetente, int codigoCategoriaTomador, int codigoTipoOcorrencia, int codigoFilial, int codigoOrigem)
        {
            Dominio.Entidades.Embarcador.ConfiguracaoContabil.ConfiguracaoCentroResultado configuracaoValida = null;

            int maximoParametrosCompativel = 0;
            foreach (Dominio.Entidades.Embarcador.ConfiguracaoContabil.ConfiguracaoCentroResultado configuracao in configuracoes)
            {
                int parametrosCompativeis = 0;
                if (configuracao.Empresa != null)
                {
                    if (configuracao.Empresa.Codigo != codigoEmpresaLayout && configuracao.Empresa.Codigo != codigoEmpresa)
                        continue;
                    parametrosCompativeis++;
                }

                if (configuracao.Destinatario != null)
                {
                    if (configuracao.Destinatario.CPF_CNPJ != destinatario)
                        continue;
                    parametrosCompativeis++;
                }

                if (configuracao.Remetente != null)
                {
                    if (configuracao.Remetente.CPF_CNPJ != remetente)
                        continue;
                    parametrosCompativeis++;
                }

                if (configuracao.Expedidor != null)
                {
                    if (configuracao.Expedidor.CPF_CNPJ != expedidor)
                        continue;
                    parametrosCompativeis++;
                }

                if (configuracao.Recebedor != null)
                {
                    if (configuracao.Recebedor.CPF_CNPJ != recebedor)
                        continue;
                    parametrosCompativeis++;
                }

                if (configuracao.Tomador != null)
                {
                    if (configuracao.Tomador.CPF_CNPJ != tomador)
                        continue;
                    parametrosCompativeis++;
                }

                if (configuracao.GrupoDestinatario != null)
                {
                    if (configuracao.GrupoDestinatario.Codigo != grupoDestinatario)
                        continue;
                    parametrosCompativeis++;
                }

                if (configuracao.GrupoRemetente != null)
                {
                    if (configuracao.GrupoRemetente.Codigo != grupoRemetente)
                        continue;
                    parametrosCompativeis++;
                }

                if (configuracao.GrupoTomador != null)
                {
                    if (configuracao.GrupoTomador.Codigo != grupoTomador)
                        continue;
                    parametrosCompativeis++;
                }

                if (configuracao.CategoriaDestinatario != null)
                {
                    if (configuracao.CategoriaDestinatario.Codigo != codigoCategoriaDestinatario)
                        continue;
                    parametrosCompativeis++;
                }

                if (configuracao.CategoriaRemetente != null)
                {
                    if (configuracao.CategoriaRemetente.Codigo != codigoCategoriaRemetente)
                        continue;
                    parametrosCompativeis++;
                }

                if (configuracao.CategoriaTomador != null)
                {
                    if (configuracao.CategoriaTomador.Codigo != codigoCategoriaTomador)
                        continue;
                    parametrosCompativeis++;
                }

                if (configuracao.GrupoProduto != null)
                {
                    if (configuracao.GrupoProduto.Codigo != codigoGrupoProduto)
                        continue;
                    parametrosCompativeis++;
                }

                if (configuracao.TipoOperacao != null)
                {
                    if (configuracao.TipoOperacao.Codigo != codigoTipoOperacao)
                        continue;
                    parametrosCompativeis++;
                }

                if (configuracao.RotaFrete != null)
                {
                    if (configuracao.RotaFrete.Codigo != codigoRotaFrete)
                        continue;
                    parametrosCompativeis++;
                }

                if (configuracao.TipoOcorrencia != null)
                {
                    if (configuracao.TipoOcorrencia.Codigo != codigoTipoOcorrencia)
                        continue;
                    parametrosCompativeis++;
                }

                if (configuracao.Filial != null)
                {
                    if (configuracao.Filial.Codigo != codigoFilial)
                        continue;
                    parametrosCompativeis++;
                }

                if (configuracao.Origem != null)
                {
                    if (configuracao.Origem.Codigo != codigoOrigem)
                        continue;
                    parametrosCompativeis++;
                }

                if (parametrosCompativeis > maximoParametrosCompativel)
                {
                    maximoParametrosCompativel = parametrosCompativeis;
                    configuracaoValida = configuracao;
                }
                else if (parametrosCompativeis == 0 && maximoParametrosCompativel == 0) //Todos os campos do centro eram nulos
                    configuracaoValida = configuracao;
            }

            return configuracaoValida;
        }


        #region versao antiga Comentada
        //public Dominio.Entidades.Embarcador.ConfiguracaoContabil.ConfiguracaoCentroResultado ObterConfiguracaoCentroResultado(Dominio.Entidades.Cliente remetente, Dominio.Entidades.Cliente destinatario, Dominio.Entidades.Cliente tomador, Dominio.Entidades.Embarcador.Produtos.GrupoProduto grupoProduto, Dominio.Entidades.Empresa empresaLayout, Dominio.Entidades.Empresa empresa, Dominio.Entidades.Embarcador.Pedidos.TipoOperacao tipoOperacao, Dominio.Entidades.RotaFrete rotaFrete, Repositorio.UnitOfWork unitOfWork)
        //{
        //    Dominio.Entidades.Embarcador.ConfiguracaoContabil.ConfiguracaoCentroResultado ConfiguracaoCentroResultado = null;
        //    int grupoRemetente = remetente?.GrupoPessoas?.Codigo ?? 0;
        //    int grupoDestinatario = destinatario?.GrupoPessoas?.Codigo ?? 0;
        //    int grupoTomador = tomador?.GrupoPessoas?.Codigo ?? 0;
        //    int codigoGrupoProduto = grupoProduto?.Codigo ?? 0;
        //    int codigotipoOperacao = tipoOperacao?.Codigo ?? 0;
        //    int codigoRotaFrete = rotaFrete?.Codigo ?? 0;
        //    int codigoEmpresaLayout = empresaLayout?.Codigo ?? 0;
        //    int codigoEmpresa = empresa?.Codigo ?? 0;
        //    double cnpjRemetente = remetente?.CPF_CNPJ ?? 0;
        //    double cnpjDestinatario = destinatario?.CPF_CNPJ ?? 0;
        //    double cnpjTomador = tomador?.CPF_CNPJ ?? 0;
        //    int codigoCategoriaDestinatario = destinatario?.Categoria?.Codigo ?? 0;
        //    int codigoCategoriaRemetente = remetente?.Categoria?.Codigo ?? 0;
        //    int codigoCategoriaTomador = tomador?.Categoria?.Codigo ?? 0;

        //    Repositorio.Embarcador.ConfiguracaoContabil.ConfiguracaoCentroResultado repConfiguracaoCentroResultado = new Repositorio.Embarcador.ConfiguracaoContabil.ConfiguracaoCentroResultado(unitOfWork);

        //    //int configuracoesCadastradas = repConfiguracaoCentroResultado.ContarRegrasCadastradas();

        //    //if (configuracoesCadastradas > 5)
        //    //{

        //    if (ConfiguracaoCentroResultado == null)
        //    {
        //        List<Dominio.Entidades.Embarcador.ConfiguracaoContabil.ConfiguracaoCentroResultado> configuracoesParticipantesEmpresaLayout = repConfiguracaoCentroResultado.FiltrarPorParticipantesEmpresa(codigoEmpresaLayout, cnpjRemetente, cnpjDestinatario, cnpjTomador, grupoRemetente, grupoDestinatario, grupoTomador, codigoCategoriaDestinatario, codigoCategoriaRemetente, codigoCategoriaTomador);
        //        if (configuracoesParticipantesEmpresaLayout.Count > 0)
        //            ConfiguracaoCentroResultado = RetornarConfiguracaoValida(configuracoesParticipantesEmpresaLayout, cnpjRemetente, cnpjDestinatario, cnpjTomador, grupoDestinatario, grupoRemetente, grupoTomador, codigoGrupoProduto, codigoEmpresaLayout, codigoEmpresa, codigotipoOperacao, codigoRotaFrete, codigoCategoriaDestinatario, codigoCategoriaRemetente, codigoCategoriaTomador);
        //    }

        //    List<Dominio.Entidades.Embarcador.ConfiguracaoContabil.ConfiguracaoCentroResultado> configuracoesEmpresa = repConfiguracaoCentroResultado.FiltrarPorEmpresa(codigoEmpresaLayout);
        //    if (configuracoesEmpresa.Count > 0)
        //        ConfiguracaoCentroResultado = RetornarConfiguracaoValida(configuracoesEmpresa, cnpjRemetente, cnpjDestinatario, cnpjTomador, grupoDestinatario, grupoRemetente, grupoTomador, codigoGrupoProduto, codigoEmpresaLayout, codigoEmpresa, codigotipoOperacao, codigoRotaFrete, codigoCategoriaDestinatario, codigoCategoriaRemetente, codigoCategoriaTomador);


        //    if (ConfiguracaoCentroResultado == null)
        //    {
        //        List<Dominio.Entidades.Embarcador.ConfiguracaoContabil.ConfiguracaoCentroResultado> configuracoesParticipantes = repConfiguracaoCentroResultado.FiltrarPorParticipantes(cnpjRemetente, cnpjDestinatario, cnpjTomador, 0, 0, 0, 0, 0, 0);
        //        if (configuracoesParticipantes.Count > 0)
        //            ConfiguracaoCentroResultado = RetornarConfiguracaoValida(configuracoesParticipantes, cnpjRemetente, cnpjDestinatario, cnpjTomador, grupoDestinatario, grupoRemetente, grupoTomador, codigoGrupoProduto, codigoEmpresaLayout, codigoEmpresa, codigotipoOperacao, codigoRotaFrete, codigoCategoriaDestinatario, codigoCategoriaRemetente, codigoCategoriaTomador);
        //    }

        //    if (ConfiguracaoCentroResultado == null)
        //    {
        //        List<Dominio.Entidades.Embarcador.ConfiguracaoContabil.ConfiguracaoCentroResultado> configuracoesParticipantes = repConfiguracaoCentroResultado.FiltrarPorParticipantes(cnpjRemetente, cnpjDestinatario, cnpjTomador, grupoRemetente, grupoDestinatario, grupoTomador, codigoCategoriaDestinatario, codigoCategoriaRemetente, codigoCategoriaTomador);
        //        if (configuracoesParticipantes.Count > 0)
        //            ConfiguracaoCentroResultado = RetornarConfiguracaoValida(configuracoesParticipantes, cnpjRemetente, cnpjDestinatario, cnpjTomador, grupoDestinatario, grupoRemetente, grupoTomador, codigoGrupoProduto, codigoEmpresaLayout, codigoEmpresa, codigotipoOperacao, codigoRotaFrete, codigoCategoriaDestinatario, codigoCategoriaRemetente, codigoCategoriaTomador);
        //    }

        //    if (ConfiguracaoCentroResultado == null)
        //    {
        //        List<Dominio.Entidades.Embarcador.ConfiguracaoContabil.ConfiguracaoCentroResultado> configuracoesAtividade = repConfiguracaoCentroResultado.FiltrarPorGrupoProdutoEmbarcador(codigoGrupoProduto);
        //        if (configuracoesAtividade.Count > 0)
        //            ConfiguracaoCentroResultado = RetornarConfiguracaoValida(configuracoesAtividade, cnpjRemetente, cnpjDestinatario, cnpjTomador, grupoDestinatario, grupoRemetente, grupoTomador, codigoGrupoProduto, codigoEmpresaLayout, codigoEmpresa, codigotipoOperacao, codigoRotaFrete, codigoCategoriaDestinatario, codigoCategoriaRemetente, codigoCategoriaTomador);
        //    }

        //    if (ConfiguracaoCentroResultado == null)
        //    {
        //        List<Dominio.Entidades.Embarcador.ConfiguracaoContabil.ConfiguracaoCentroResultado> configuracoesProdutoEmbarcador = repConfiguracaoCentroResultado.FiltrarPorRotaFrete(codigoRotaFrete);
        //        if (configuracoesProdutoEmbarcador.Count > 0)
        //            ConfiguracaoCentroResultado = RetornarConfiguracaoValida(configuracoesProdutoEmbarcador, cnpjRemetente, cnpjDestinatario, cnpjTomador, grupoDestinatario, grupoRemetente, grupoTomador, codigoGrupoProduto, codigoEmpresaLayout, codigoEmpresa, codigotipoOperacao, codigoRotaFrete, codigoCategoriaDestinatario, codigoCategoriaRemetente, codigoCategoriaTomador);
        //    }

        //    if (ConfiguracaoCentroResultado == null)
        //    {
        //        List<Dominio.Entidades.Embarcador.ConfiguracaoContabil.ConfiguracaoCentroResultado> configuracoesProdutoEmbarcador = repConfiguracaoCentroResultado.FiltrarPorTipoOperacao(codigotipoOperacao);
        //        if (configuracoesProdutoEmbarcador.Count > 0)
        //            ConfiguracaoCentroResultado = RetornarConfiguracaoValida(configuracoesProdutoEmbarcador, cnpjRemetente, cnpjDestinatario, cnpjTomador, grupoDestinatario, grupoRemetente, grupoTomador, codigoGrupoProduto, codigoEmpresaLayout, codigoEmpresa, codigotipoOperacao, codigoRotaFrete, codigoCategoriaDestinatario, codigoCategoriaRemetente, codigoCategoriaTomador);
        //    }

        //    //}
        //    //else
        //    //{
        //    //    if (configuracoesCadastradas > 0)
        //    //    {
        //    //        List<Dominio.Entidades.Embarcador.ConfiguracaoContabil.ConfiguracaoCentroResultado> configuracoes = repConfiguracaoCentroResultado.FiltrarTodasRegrasCadastradas();
        //    //        if (configuracoes.Count > 0)
        //    //            ConfiguracaoCentroResultado = RetornarConfiguracaoValida(configuracoes, cnpjRemetente, cnpjDestinatario, cnpjTomador, grupoDestinatario, grupoRemetente, grupoTomador, codigoGrupoProduto, codigoEmpresa, codigotipoOperacao, codigoRotaFrete, codigoCategoriaDestinatario, codigoCategoriaRemetente, codigoCategoriaTomador);
        //    //    }
        //    //}
        //    return ConfiguracaoCentroResultado;
        //}


        //private Dominio.Entidades.Embarcador.ConfiguracaoContabil.ConfiguracaoCentroResultado RetornarConfiguracaoValida(List<Dominio.Entidades.Embarcador.ConfiguracaoContabil.ConfiguracaoCentroResultado> configuracoes, double remetente, double destinatario, double tomador, int grupoDestinatario, int grupoRemetente, int grupoTomador, int codigoGrupoProduto, int codigoEmpresaLayout, int codigoEmpresa, int codigoTipoOperacao, int codigoRotaFrete, int codigoCategoriaDestinatario, int codigoCategoriaRemetente, int codigoCategoriaTomador)
        //{
        //    Dominio.Entidades.Embarcador.ConfiguracaoContabil.ConfiguracaoCentroResultado configuracaoValida = null;
        //    foreach (Dominio.Entidades.Embarcador.ConfiguracaoContabil.ConfiguracaoCentroResultado configuracao in configuracoes)
        //    {
        //        bool valido = true;
        //        if (configuracao.Empresa != null && (configuracao.Empresa.Codigo != codigoEmpresaLayout && configuracao.Empresa.Codigo != codigoEmpresa))
        //            valido = false;
        //        else if (configuracao.Destinatario != null && configuracao.Destinatario.CPF_CNPJ != destinatario)
        //            valido = false;
        //        else if (configuracao.Remetente != null && configuracao.Remetente.CPF_CNPJ != remetente)
        //            valido = false;
        //        else if (configuracao.Tomador != null && configuracao.Tomador.CPF_CNPJ != tomador)
        //            valido = false;
        //        else if (configuracao.GrupoDestinatario != null && configuracao.GrupoDestinatario.Codigo != grupoDestinatario)
        //            valido = false;
        //        else if (configuracao.GrupoRemetente != null && configuracao.GrupoRemetente.Codigo != grupoRemetente)
        //            valido = false;
        //        else if (configuracao.GrupoTomador != null && configuracao.GrupoTomador.Codigo != grupoTomador)
        //            valido = false;
        //        else if (configuracao.CategoriaDestinatario != null && configuracao.CategoriaDestinatario.Codigo != codigoCategoriaDestinatario)
        //            valido = false;
        //        else if (configuracao.CategoriaRemetente != null && configuracao.CategoriaRemetente.Codigo != codigoCategoriaRemetente)
        //            valido = false;
        //        else if (configuracao.CategoriaTomador != null && configuracao.CategoriaTomador.Codigo != codigoCategoriaTomador)
        //            valido = false;
        //        else if (configuracao.GrupoProduto != null && configuracao.GrupoProduto.Codigo != codigoGrupoProduto)
        //            valido = false;
        //        else if (configuracao.TipoOperacao != null && configuracao.TipoOperacao.Codigo != codigoTipoOperacao)
        //            valido = false;
        //        else if (configuracao.RotaFrete != null && configuracao.RotaFrete.Codigo != codigoRotaFrete)
        //            valido = false;


        //        if (valido)
        //        {
        //            configuracaoValida = configuracao;
        //            break;
        //        }

        //    }
        //    return configuracaoValida;
        //}

        #endregion


    }
}
