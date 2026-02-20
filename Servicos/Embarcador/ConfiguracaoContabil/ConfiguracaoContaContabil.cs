using System.Collections.Generic;
using System.Linq;

namespace Servicos.Embarcador.ConfiguracaoContabil
{
    public class ConfiguracaoContaContabil
    {

        public static List<Dominio.ObjetosDeValor.Embarcador.Escrituracao.ConfiguracaoContabil> ConverterPedidoCTeParaSubContratacaoContaContabilContabilizacao(Dominio.Entidades.Embarcador.Pedidos.PedidoCTeParaSubContratacao pedidoCTeParaSubContratacao, List<Dominio.Entidades.Embarcador.Pedidos.PedidoCTeParaSubContratacaoContaContabilContabilizacao> pedidoCTeParaSubContratacaoContaContabilContabilizacaosCarga)
        {
            List<Dominio.ObjetosDeValor.Embarcador.Escrituracao.ConfiguracaoContabil> configuracaoContabeis = new List<Dominio.ObjetosDeValor.Embarcador.Escrituracao.ConfiguracaoContabil>();

            List<Dominio.Entidades.Embarcador.Pedidos.PedidoCTeParaSubContratacaoContaContabilContabilizacao> pedidoCTeParaSubContratacaoContaContabilContabilizacaos = (from obj in pedidoCTeParaSubContratacaoContaContabilContabilizacaosCarga where obj.PedidoCTeParaSubContratacao.Codigo == pedidoCTeParaSubContratacao.Codigo select obj).ToList();

            foreach (Dominio.Entidades.Embarcador.Pedidos.PedidoCTeParaSubContratacaoContaContabilContabilizacao config in pedidoCTeParaSubContratacaoContaContabilContabilizacaos)
            {
                Dominio.ObjetosDeValor.Embarcador.Escrituracao.ConfiguracaoContabil configuracaoContabil = new Dominio.ObjetosDeValor.Embarcador.Escrituracao.ConfiguracaoContabil();
                configuracaoContabil.PlanoConta = config.PlanoConta;
                configuracaoContabil.TipoContabilizacao = config.TipoContabilizacao;
                configuracaoContabil.TipoContaContabil = config.TipoContaContabil;
                configuracaoContabeis.Add(configuracaoContabil);
            }

            return configuracaoContabeis;
        }

        public static List<Dominio.ObjetosDeValor.Embarcador.Escrituracao.ConfiguracaoContabil> ConverterPedidoXMLNotaFiscalContaContabilContabilizacao(Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal pedidoXMLNotaFiscal, List<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscalContaContabilContabilizacao> pedidosXMLNotaFiscalContaContabilContabilizacaoCarga)
        {
            List<Dominio.ObjetosDeValor.Embarcador.Escrituracao.ConfiguracaoContabil> configuracaoContabeis = new List<Dominio.ObjetosDeValor.Embarcador.Escrituracao.ConfiguracaoContabil>();

            List<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscalContaContabilContabilizacao> pedidosXMLNotaFiscalContaContabilContabilizacao = (from obj in pedidosXMLNotaFiscalContaContabilContabilizacaoCarga where obj.PedidoXMLNotaFiscal.Codigo == pedidoXMLNotaFiscal.Codigo select obj).ToList();

            foreach (Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscalContaContabilContabilizacao config in pedidosXMLNotaFiscalContaContabilContabilizacao)
            {
                Dominio.ObjetosDeValor.Embarcador.Escrituracao.ConfiguracaoContabil configuracaoContabil = new Dominio.ObjetosDeValor.Embarcador.Escrituracao.ConfiguracaoContabil();
                configuracaoContabil.PlanoConta = config.PlanoConta;
                configuracaoContabil.TipoContabilizacao = config.TipoContabilizacao;
                configuracaoContabil.TipoContaContabil = config.TipoContaContabil;
                configuracaoContabeis.Add(configuracaoContabil);
            }

            return configuracaoContabeis;
        }

        public static List<Dominio.ObjetosDeValor.Embarcador.Escrituracao.ConfiguracaoContabil> ConverterCargaPedidoContaContabilContabilizacao(Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido, List<Dominio.Entidades.Embarcador.Cargas.CargaPedidoContaContabilContabilizacao> cargaPedidosContaContabilContabilizacaoCarga)
        {
            List<Dominio.ObjetosDeValor.Embarcador.Escrituracao.ConfiguracaoContabil> configuracaoContabeis = new List<Dominio.ObjetosDeValor.Embarcador.Escrituracao.ConfiguracaoContabil>();

            List<Dominio.Entidades.Embarcador.Cargas.CargaPedidoContaContabilContabilizacao> cargaPedidoContaContabilContabilizacao = (from obj in cargaPedidosContaContabilContabilizacaoCarga where obj.CargaPedido.Codigo == cargaPedido.Codigo select obj).ToList();

            foreach (Dominio.Entidades.Embarcador.Cargas.CargaPedidoContaContabilContabilizacao config in cargaPedidoContaContabilContabilizacao)
            {
                Dominio.ObjetosDeValor.Embarcador.Escrituracao.ConfiguracaoContabil configuracaoContabil = new Dominio.ObjetosDeValor.Embarcador.Escrituracao.ConfiguracaoContabil();
                configuracaoContabil.PlanoConta = config.PlanoConta;
                configuracaoContabil.TipoContabilizacao = config.TipoContabilizacao;
                configuracaoContabil.TipoContaContabil = config.TipoContaContabil;
                configuracaoContabeis.Add(configuracaoContabil);
            }

            return configuracaoContabeis;
        }


        public static List<Dominio.ObjetosDeValor.Embarcador.Escrituracao.ConfiguracaoContabil> ConverterCargaCTeComplementoInfoContaContabilContabilizacao(Dominio.Entidades.Embarcador.Cargas.CargaCTeComplementoInfo cargaCTeComplementoInfo, List<Dominio.Entidades.Embarcador.Cargas.CargaCTeComplementoInfoContaContabilContabilizacao> cargaCTeComplementoInfoContaContabilContabilizacaoCarga)
        {
            List<Dominio.ObjetosDeValor.Embarcador.Escrituracao.ConfiguracaoContabil> configuracaoContabeis = new List<Dominio.ObjetosDeValor.Embarcador.Escrituracao.ConfiguracaoContabil>();

            List<Dominio.Entidades.Embarcador.Cargas.CargaCTeComplementoInfoContaContabilContabilizacao> cargasCTeComplementoInfoContaContabilContabilizacao = (from obj in cargaCTeComplementoInfoContaContabilContabilizacaoCarga where obj.CargaCTeComplementoInfo.Codigo == cargaCTeComplementoInfo.Codigo select obj).ToList();

            foreach (Dominio.Entidades.Embarcador.Cargas.CargaCTeComplementoInfoContaContabilContabilizacao config in cargasCTeComplementoInfoContaContabilContabilizacao)
            {
                Dominio.ObjetosDeValor.Embarcador.Escrituracao.ConfiguracaoContabil configuracaoContabil = new Dominio.ObjetosDeValor.Embarcador.Escrituracao.ConfiguracaoContabil();
                configuracaoContabil.PlanoConta = config.PlanoConta;
                configuracaoContabil.TipoContabilizacao = config.TipoContabilizacao;
                configuracaoContabil.TipoContaContabil = config.TipoContaContabil;
                configuracaoContabeis.Add(configuracaoContabil);
            }

            return configuracaoContabeis;
        }

        public Dominio.ObjetosDeValor.Embarcador.ConfiguracaoContabil.ConfiguracaoContaContabil ObterConfiguracaoContaContabil(Dominio.Entidades.Cliente remetente, Dominio.Entidades.Cliente destinatario, Dominio.Entidades.Cliente tomador, Dominio.Entidades.Embarcador.Produtos.GrupoProduto grupoProduto, Dominio.Entidades.Empresa empresa, Dominio.Entidades.Embarcador.Pedidos.TipoOperacao tipoOperacao, Dominio.Entidades.RotaFrete rotaFrete, Dominio.Entidades.ModeloDocumentoFiscal modeloDocumento, Dominio.Entidades.TipoDeOcorrenciaDeCTe tipoDeOcorrencia, Repositorio.UnitOfWork unitOfWork)
        {

            Servicos.Embarcador.ConfiguracaoContabil.RegraConfiguracaoContaContabil regraConfiguracaoContaContabil = Servicos.Embarcador.ConfiguracaoContabil.RegraConfiguracaoContaContabil.GetInstance(unitOfWork);

            if (regraConfiguracaoContaContabil?.ConfiguracoesContaContabil == null)
                return null;

            IList<Dominio.ObjetosDeValor.Embarcador.ConfiguracaoContabil.ConfiguracaoContaContabil> configuracaoContaContabilCarregada = regraConfiguracaoContaContabil.ConfiguracoesContaContabil;

            Dominio.ObjetosDeValor.Embarcador.ConfiguracaoContabil.ConfiguracaoContaContabil ConfiguracaoContaContabil = null;

            Dominio.ObjetosDeValor.Embarcador.ConfiguracaoContabil.ConfiguracaoContaContabil ConfiguracaoContaContabilValidacao = new Dominio.ObjetosDeValor.Embarcador.ConfiguracaoContabil.ConfiguracaoContaContabil()
            {
                CodigoGrupoRemetente = remetente?.GrupoPessoas?.Codigo ?? 0,
                CodigoGrupoDestinatario = destinatario?.GrupoPessoas?.Codigo ?? 0,
                CodigoGrupoTomador = tomador?.GrupoPessoas?.Codigo ?? 0,
                CodigoGrupoProduto = grupoProduto?.Codigo ?? 0,
                CodigoTipoOperacao = tipoOperacao?.Codigo ?? 0,
                CodigoRotaFrete = rotaFrete?.Codigo ?? 0,
                CodigoEmpresa = empresa?.Codigo ?? 0,
                CodigoModeloDocumentoFiscal = modeloDocumento?.Codigo ?? 0,
                CodigoTipoOcorrencia = tipoDeOcorrencia?.Codigo ?? 0,
                CodigoRemetente = remetente?.CPF_CNPJ ?? 0,
                CodigoDestinatario = destinatario?.CPF_CNPJ ?? 0,
                CodigoTomador = tomador?.CPF_CNPJ ?? 0,
                CodigoCategoriaDestinatario = destinatario?.Categoria?.Codigo ?? 0,
                CodigoCategoriaRemetente = remetente?.Categoria?.Codigo ?? 0,
                CodigoCategoriaTomador = tomador?.Categoria?.Codigo ?? 0,
            };

            Repositorio.Embarcador.ConfiguracaoContabil.ConfiguracaoContaContabil repConfiguracaoContaContabil = new Repositorio.Embarcador.ConfiguracaoContabil.ConfiguracaoContaContabil(unitOfWork);

            List<Dominio.ObjetosDeValor.Embarcador.ConfiguracaoContabil.ConfiguracaoContaContabil> configuracoesEmpresa = FiltrarPorEmpresa(ConfiguracaoContaContabilValidacao.CodigoEmpresa, configuracaoContaContabilCarregada);

            if (regraConfiguracaoContaContabil.ConfiguracoesContaContabil.Count > 0)
                ConfiguracaoContaContabil = RetornarConfiguracaoValida(configuracoesEmpresa, ConfiguracaoContaContabilValidacao);

            if (ConfiguracaoContaContabilValidacao.CodigoTipoOcorrencia > 0)
            {
                List<Dominio.ObjetosDeValor.Embarcador.ConfiguracaoContabil.ConfiguracaoContaContabil> configuracoesTipoOcorrencia = FiltrarPorTipoOcorrencia(ConfiguracaoContaContabilValidacao.CodigoTipoOcorrencia, configuracaoContaContabilCarregada);
                if (configuracoesTipoOcorrencia.Count > 0)
                    ConfiguracaoContaContabil = RetornarConfiguracaoValida(configuracoesTipoOcorrencia, ConfiguracaoContaContabilValidacao);
            }

            if (ConfiguracaoContaContabil == null)
            {
                List<Dominio.ObjetosDeValor.Embarcador.ConfiguracaoContabil.ConfiguracaoContaContabil> configuracoesParticipantes = repConfiguracaoContaContabil.FiltrarPorParticipantes(ConfiguracaoContaContabilValidacao, configuracaoContaContabilCarregada, true);
                if (configuracoesParticipantes.Count > 0)
                    ConfiguracaoContaContabil = RetornarConfiguracaoValida(configuracoesParticipantes, ConfiguracaoContaContabilValidacao);
            }

            if (ConfiguracaoContaContabil == null)
            {
                List<Dominio.ObjetosDeValor.Embarcador.ConfiguracaoContabil.ConfiguracaoContaContabil> configuracoesParticipantes = repConfiguracaoContaContabil.FiltrarPorParticipantes(ConfiguracaoContaContabilValidacao, configuracaoContaContabilCarregada);
                if (configuracoesParticipantes.Count > 0)
                    ConfiguracaoContaContabil = RetornarConfiguracaoValida(configuracoesParticipantes, ConfiguracaoContaContabilValidacao);
            }

            if (ConfiguracaoContaContabil == null)
            {
                List<Dominio.ObjetosDeValor.Embarcador.ConfiguracaoContabil.ConfiguracaoContaContabil> configuracoesAtividade = FiltrarPorGrupoProdutoEmbarcador(ConfiguracaoContaContabilValidacao.CodigoGrupoProduto, configuracaoContaContabilCarregada);
                if (configuracoesAtividade.Count > 0)
                    ConfiguracaoContaContabil = RetornarConfiguracaoValida(configuracoesAtividade, ConfiguracaoContaContabilValidacao);
            }

            if (ConfiguracaoContaContabil == null)
            {
                List<Dominio.ObjetosDeValor.Embarcador.ConfiguracaoContabil.ConfiguracaoContaContabil> configuracoesProdutoEmbarcador = FiltrarPorRotaFrete(ConfiguracaoContaContabilValidacao.CodigoRotaFrete, configuracaoContaContabilCarregada);
                if (configuracoesProdutoEmbarcador.Count > 0)
                    ConfiguracaoContaContabil = RetornarConfiguracaoValida(configuracoesProdutoEmbarcador, ConfiguracaoContaContabilValidacao);
            }

            if (ConfiguracaoContaContabil == null)
            {
                List<Dominio.ObjetosDeValor.Embarcador.ConfiguracaoContabil.ConfiguracaoContaContabil> configuracoesProdutoEmbarcador = FiltrarPorTipoOperacao(ConfiguracaoContaContabilValidacao.CodigoTipoOperacao, configuracaoContaContabilCarregada);
                if (configuracoesProdutoEmbarcador.Count > 0)
                    ConfiguracaoContaContabil = RetornarConfiguracaoValida(configuracoesProdutoEmbarcador, ConfiguracaoContaContabilValidacao);
            }

            if (ConfiguracaoContaContabil == null && configuracaoContaContabilCarregada.Count > 0)
            {
                ConfiguracaoContaContabil = RetornarConfiguracaoValida(configuracaoContaContabilCarregada, ConfiguracaoContaContabilValidacao);
            }

            return ConfiguracaoContaContabil;
        }

        #region Métodos Privados

        private Dominio.ObjetosDeValor.Embarcador.ConfiguracaoContabil.ConfiguracaoContaContabil RetornarConfiguracaoValida(IList<Dominio.ObjetosDeValor.Embarcador.ConfiguracaoContabil.ConfiguracaoContaContabil> configuracoes, Dominio.ObjetosDeValor.Embarcador.ConfiguracaoContabil.ConfiguracaoContaContabil configuracaoContaContabilValidacao)
        {
            Dominio.ObjetosDeValor.Embarcador.ConfiguracaoContabil.ConfiguracaoContaContabil configuracaoValida = null;
            foreach (Dominio.ObjetosDeValor.Embarcador.ConfiguracaoContabil.ConfiguracaoContaContabil configuracao in configuracoes)
            {
                bool valido = true;
                if (configuracao.CodigoEmpresa > 0 && configuracao.CodigoEmpresa != configuracaoContaContabilValidacao.CodigoEmpresa)
                    valido = false;
                else if (configuracao.CodigoDestinatario > 0 && configuracao.CodigoDestinatario != configuracaoContaContabilValidacao.CodigoDestinatario)
                    valido = false;
                else if (configuracao.CodigoRemetente > 0 && configuracao.CodigoRemetente != configuracaoContaContabilValidacao.CodigoRemetente)
                    valido = false;
                else if (configuracao.CodigoTomador > 0 && configuracao.CodigoTomador != configuracaoContaContabilValidacao.CodigoTomador)
                    valido = false;
                else if (configuracao.CodigoGrupoDestinatario > 0 && configuracao.CodigoGrupoDestinatario != configuracaoContaContabilValidacao.CodigoGrupoDestinatario)
                    valido = false;
                else if (configuracao.CodigoGrupoRemetente > 0 && configuracao.CodigoGrupoRemetente != configuracaoContaContabilValidacao.CodigoGrupoRemetente)
                    valido = false;
                else if (configuracao.CodigoGrupoTomador > 0 && configuracao.CodigoGrupoTomador != configuracaoContaContabilValidacao.CodigoGrupoTomador)
                    valido = false;
                else if (configuracao.CodigoCategoriaDestinatario > 0 && configuracao.CodigoCategoriaDestinatario != configuracaoContaContabilValidacao.CodigoCategoriaDestinatario)
                    valido = false;
                else if (configuracao.CodigoCategoriaRemetente > 0 && configuracao.CodigoCategoriaRemetente != configuracaoContaContabilValidacao.CodigoCategoriaRemetente)
                    valido = false;
                else if (configuracao.CodigoCategoriaTomador > 0 && configuracao.CodigoCategoriaTomador != configuracaoContaContabilValidacao.CodigoCategoriaTomador)
                    valido = false;
                else if (configuracao.CodigoGrupoProduto > 0 && configuracao.CodigoGrupoProduto != configuracaoContaContabilValidacao.CodigoGrupoProduto)
                    valido = false;
                else if (configuracao.CodigoTipoOperacao > 0 && configuracao.CodigoTipoOperacao != configuracaoContaContabilValidacao.CodigoTipoOperacao)
                    valido = false;
                else if (configuracao.CodigoRotaFrete > 0 && configuracao.CodigoRotaFrete != configuracaoContaContabilValidacao.CodigoRotaFrete)
                    valido = false;
                else if (configuracao.CodigoModeloDocumentoFiscal > 0 && configuracao.CodigoModeloDocumentoFiscal != configuracaoContaContabilValidacao.CodigoModeloDocumentoFiscal)
                    valido = false;
                else if (configuracao.CodigoTipoOcorrencia > 0 && configuracao.CodigoTipoOcorrencia != configuracaoContaContabilValidacao.CodigoTipoOcorrencia)
                    valido = false;

                if (valido)
                {
                    configuracaoValida = configuracao;
                    break;
                }

            }
            return configuracaoValida;
        }

        private List<Dominio.ObjetosDeValor.Embarcador.ConfiguracaoContabil.ConfiguracaoContaContabil> FiltrarPorEmpresa(int empresa, IList<Dominio.ObjetosDeValor.Embarcador.ConfiguracaoContabil.ConfiguracaoContaContabil> configuracaoContaContabils)
        {
            List<Dominio.ObjetosDeValor.Embarcador.ConfiguracaoContabil.ConfiguracaoContaContabil> result = configuracaoContaContabils.Where(obj => obj.Ativo && obj.CodigoEmpresa != 0 && obj.CodigoEmpresa == empresa).ToList();
            return result.OrderByDescending(obj => obj.CodigoModeloDocumentoFiscal > 0).ToList();
        }

        private List<Dominio.ObjetosDeValor.Embarcador.ConfiguracaoContabil.ConfiguracaoContaContabil> FiltrarPorTipoOcorrencia(int tipoOcorrencia, IList<Dominio.ObjetosDeValor.Embarcador.ConfiguracaoContabil.ConfiguracaoContaContabil> configuracaoContaContabils)
        {
            List<Dominio.ObjetosDeValor.Embarcador.ConfiguracaoContabil.ConfiguracaoContaContabil> result = tipoOcorrencia > 0
                ? configuracaoContaContabils.Where(obj => obj.CodigoTipoOcorrencia > 0 && obj.CodigoTipoOcorrencia == tipoOcorrencia).ToList()
                : new List<Dominio.ObjetosDeValor.Embarcador.ConfiguracaoContabil.ConfiguracaoContaContabil>();

            result = result.Where(obj =>
                obj.CodigoRemetente == 0 &&
                obj.CodigoDestinatario == 0 &&
                obj.CodigoTomador == 0 &&
                obj.CodigoGrupoDestinatario == 0 &&
                obj.CodigoGrupoRemetente == 0 &&
                obj.CodigoGrupoTomador == 0 &&
                obj.CodigoCategoriaDestinatario == 0 &&
                obj.CodigoCategoriaRemetente == 0 &&
                obj.CodigoCategoriaTomador == 0 &&
                obj.CodigoGrupoProduto == 0 &&
                obj.CodigoTipoOperacao == 0
            ).ToList();

            return result.OrderByDescending(obj => obj.CodigoModeloDocumentoFiscal > 0).ToList();
        }

        private List<Dominio.ObjetosDeValor.Embarcador.ConfiguracaoContabil.ConfiguracaoContaContabil> FiltrarPorGrupoProdutoEmbarcador(int grupoProdutoEmbarcador, IList<Dominio.ObjetosDeValor.Embarcador.ConfiguracaoContabil.ConfiguracaoContaContabil> configuracaoContaContabils)
        {

            List<Dominio.ObjetosDeValor.Embarcador.ConfiguracaoContabil.ConfiguracaoContaContabil> result = grupoProdutoEmbarcador > 0
                ? configuracaoContaContabils.Where(obj => obj.CodigoGrupoProduto > 0 && obj.CodigoGrupoProduto == grupoProdutoEmbarcador).ToList()
                : new List<Dominio.ObjetosDeValor.Embarcador.ConfiguracaoContabil.ConfiguracaoContaContabil>();

            result = result.Where(obj =>
                obj.CodigoRemetente == 0 &&
                obj.CodigoDestinatario == 0 &&
                obj.CodigoTomador == 0 &&
                obj.CodigoGrupoDestinatario == 0 &&
                obj.CodigoGrupoRemetente == 0 &&
                obj.CodigoGrupoTomador == 0 &&
                obj.CodigoCategoriaDestinatario == 0 &&
                obj.CodigoCategoriaRemetente == 0 &&
                obj.CodigoCategoriaTomador == 0 &&
                obj.CodigoTipoOperacao == 0 &&
                obj.CodigoRotaFrete == 0 &&
                obj.CodigoTipoOcorrencia == 0
            ).ToList();

            return result.OrderByDescending(obj => obj.CodigoModeloDocumentoFiscal > 0).ToList();
        }

        private List<Dominio.ObjetosDeValor.Embarcador.ConfiguracaoContabil.ConfiguracaoContaContabil> FiltrarPorRotaFrete(int rotaFrete, IList<Dominio.ObjetosDeValor.Embarcador.ConfiguracaoContabil.ConfiguracaoContaContabil> configuracaoContaContabils)
        {
            List<Dominio.ObjetosDeValor.Embarcador.ConfiguracaoContabil.ConfiguracaoContaContabil> result = rotaFrete > 0
                            ? configuracaoContaContabils.Where(obj => obj.CodigoRotaFrete > 0 && obj.CodigoRotaFrete == rotaFrete).ToList()
                            : new List<Dominio.ObjetosDeValor.Embarcador.ConfiguracaoContabil.ConfiguracaoContaContabil>();

            result = result.Where(obj =>
                obj.CodigoRemetente == 0 &&
                obj.CodigoDestinatario == 0 &&
                obj.CodigoTomador == 0 &&
                obj.CodigoGrupoDestinatario == 0 &&
                obj.CodigoGrupoRemetente == 0 &&
                obj.CodigoGrupoTomador == 0 &&
                obj.CodigoCategoriaDestinatario == 0 &&
                obj.CodigoCategoriaRemetente == 0 &&
                obj.CodigoCategoriaTomador == 0 &&
                obj.CodigoGrupoProduto == 0 &&
                obj.CodigoTipoOperacao == 0 &&
                obj.CodigoTipoOcorrencia == 0
            ).ToList();

            return result.OrderByDescending(obj => obj.CodigoModeloDocumentoFiscal > 0).ToList();
        }

        private List<Dominio.ObjetosDeValor.Embarcador.ConfiguracaoContabil.ConfiguracaoContaContabil> FiltrarPorTipoOperacao(int tipoOperacao, IList<Dominio.ObjetosDeValor.Embarcador.ConfiguracaoContabil.ConfiguracaoContaContabil> configuracaoContaContabils)
        {

            List<Dominio.ObjetosDeValor.Embarcador.ConfiguracaoContabil.ConfiguracaoContaContabil> result = tipoOperacao > 0
                                        ? configuracaoContaContabils.Where(obj => obj.CodigoTipoOperacao > 0 && obj.CodigoTipoOperacao == tipoOperacao).ToList()
                                        : new List<Dominio.ObjetosDeValor.Embarcador.ConfiguracaoContabil.ConfiguracaoContaContabil>();

            result = result.Where(obj =>
                obj.CodigoRemetente == 0 &&
                obj.CodigoDestinatario == 0 &&
                obj.CodigoTomador == 0 &&
                obj.CodigoGrupoDestinatario == 0 &&
                obj.CodigoGrupoRemetente == 0 &&
                obj.CodigoGrupoTomador == 0 &&
                obj.CodigoCategoriaDestinatario == 0 &&
                obj.CodigoCategoriaRemetente == 0 &&
                obj.CodigoCategoriaTomador == 0 &&
                obj.CodigoGrupoProduto == 0 &&
                obj.CodigoRotaFrete == 0 &&
                obj.CodigoTipoOcorrencia == 0
            ).ToList();

            return result.OrderByDescending(obj => obj.CodigoModeloDocumentoFiscal > 0).ToList();
        }

        #endregion
    }
}
