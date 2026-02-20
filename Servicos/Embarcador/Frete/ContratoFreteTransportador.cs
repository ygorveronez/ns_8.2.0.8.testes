using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;
using System.Collections.Generic;
using System.Linq;
using Dominio.ObjetosDeValor.Relatorios;
using Servicos.Extensions;

namespace Servicos.Embarcador.Frete
{
    public class ContratoFreteTransportador
    {
        #region Atributos

        private readonly Repositorio.UnitOfWork _unitOfWork;

        #endregion Atributos

        #region Construtores

        public ContratoFreteTransportador(Repositorio.UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        #endregion Construtores

        #region Métodos Publicos

        public static void GerarSaldoContratoPorCarga(Dominio.Entidades.Embarcador.Cargas.Carga carga, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoEmbarcador, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Frete.ContratoSaldoMes repositorioContratoSaldoMes = new Repositorio.Embarcador.Frete.ContratoSaldoMes(unitOfWork);

            repositorioContratoSaldoMes.DeletarPorCarga(carga.Codigo);

            if (carga.ContratoFreteTransportador == null)
                return;

            if ((tipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS) && (carga.ContratoFreteTransportador.TipoEmissaoComplemento != TipoEmissaoComplementoContratoFreteTransportador.PorVeiculoEMotorista))
                return;

            Carga.Carga servicoCarga = new Carga.Carga(unitOfWork);
            decimal distancia = servicoCarga.ObterDistancia(carga, configuracaoEmbarcador, unitOfWork);

            if (carga.DistanciaExcedenteContrato > 0)
            {
                decimal distanciaNormal = distancia - carga.DistanciaExcedenteContrato;

                if (distanciaNormal > 0)
                {
                    Dominio.Entidades.Embarcador.Frete.ContratoSaldoMes contratoSaldoMesDistanciaNormal = new Dominio.Entidades.Embarcador.Frete.ContratoSaldoMes()
                    {
                        Carga = carga,
                        ContratoFreteTransportador = carga.ContratoFreteTransportador,
                        DataRegistro = carga.DataValorContrato ?? DateTime.Now,
                        Distancia = distanciaNormal,
                        ValorPagar = carga.ValorFreteContratoFrete
                    };

                    repositorioContratoSaldoMes.Inserir(contratoSaldoMesDistanciaNormal);
                }

                Dominio.Entidades.Embarcador.Frete.ContratoSaldoMes contratoSaldoMesDistanciaExcedente = new Dominio.Entidades.Embarcador.Frete.ContratoSaldoMes()
                {
                    Carga = carga,
                    ContratoFreteTransportador = carga.ContratoFreteTransportador,
                    DataRegistro = carga.DataValorContrato ?? DateTime.Now,
                    Distancia = carga.DistanciaExcedenteContrato,
                    Excedente = true,
                    ValorPagar = carga.ValorFreteContratoFreteExcedente
                };

                repositorioContratoSaldoMes.Inserir(contratoSaldoMesDistanciaExcedente);
            }
            else
            {
                Dominio.Entidades.Embarcador.Frete.ContratoSaldoMes contratoSaldoMes = new Dominio.Entidades.Embarcador.Frete.ContratoSaldoMes()
                {
                    Carga = carga,
                    ContratoFreteTransportador = carga.ContratoFreteTransportador,
                    DataRegistro = carga.DataValorContrato ?? DateTime.Now,
                    Distancia = distancia,
                    ValorPagar = configuracaoEmbarcador.TipoFechamentoFrete == TipoFechamentoFrete.FechamentoPorKm ? carga.ValorFreteContratoFrete : carga.ValorFreteAPagar
                };

                repositorioContratoSaldoMes.Inserir(contratoSaldoMes);
            }
        }

        public static void GerarSaldoContratoPorFechamento(Dominio.Entidades.Embarcador.Fechamento.FechamentoFrete fechamentoFrete, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Repositorio.UnitOfWork unitOfWork)
        {
            if ((tipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS) && (fechamentoFrete.Contrato.TipoEmissaoComplemento != TipoEmissaoComplementoContratoFreteTransportador.PorVeiculoEMotorista))
                return;

            Repositorio.Embarcador.Frete.ContratoSaldoMes repositorioContratoSaldoMes = new Repositorio.Embarcador.Frete.ContratoSaldoMes(unitOfWork);

            repositorioContratoSaldoMes.DeletarPorFechamentoFrete(fechamentoFrete.Codigo);

            Dominio.Entidades.Embarcador.Frete.ContratoSaldoMes contratoSaldoMes = new Dominio.Entidades.Embarcador.Frete.ContratoSaldoMes()
            {
                ContratoFreteTransportador = fechamentoFrete.Contrato,
                DataRegistro = fechamentoFrete.DataFim,
                Distancia = 0m,
                FechamentoFrete = fechamentoFrete,
                ValorPagar = fechamentoFrete.ValorComplementos
            };

            repositorioContratoSaldoMes.Inserir(contratoSaldoMes);
        }

        public static void RemoverSaldoContratoPorFechamento(Dominio.Entidades.Embarcador.Fechamento.FechamentoFrete fechamentoFrete, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Frete.ContratoSaldoMes repositorioContratoSaldoMes = new Repositorio.Embarcador.Frete.ContratoSaldoMes(unitOfWork);

            repositorioContratoSaldoMes.DeletarPorFechamentoFrete(fechamentoFrete.Codigo);
        }

        public static List<Dominio.Entidades.Embarcador.Frete.RegraContratoFreteTransportador> VerificarRegrasAutorizacao(Dominio.Entidades.Embarcador.Frete.ContratoFreteTransportador contrato, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Frete.RegraContratoFreteTransportador repRegraContratoFreteTransportador = new Repositorio.Embarcador.Frete.RegraContratoFreteTransportador(unitOfWork);
            List<Dominio.Entidades.Embarcador.Frete.RegraContratoFreteTransportador> listaRegras = new List<Dominio.Entidades.Embarcador.Frete.RegraContratoFreteTransportador>();
            List<Dominio.Entidades.Embarcador.Frete.RegraContratoFreteTransportador> listaFiltrada = new List<Dominio.Entidades.Embarcador.Frete.RegraContratoFreteTransportador>();
            List<Dominio.Entidades.Embarcador.Frete.RegraContratoFreteTransportador> alcadasCompativeis;

            if (contrato.Transportador != null)
            {
                alcadasCompativeis = repRegraContratoFreteTransportador.AlcadasPorTransportador(contrato.Transportador.Codigo, DateTime.Today);
                listaRegras.AddRange(alcadasCompativeis);
            }

            listaRegras.AddRange(repRegraContratoFreteTransportador.AlcadasPorValor(DateTime.Now));

            // Deposito
            if (contrato.Filiais != null)
            {
                alcadasCompativeis = repRegraContratoFreteTransportador.AlcadasPorFilial((from o in contrato.Filiais select o.Filial.Codigo).ToList(), DateTime.Today);
                listaRegras.AddRange(alcadasCompativeis);
            }

            listaRegras = listaRegras.Distinct().ToList();
            if (listaRegras.Count() > 0)
            {
                listaFiltrada.AddRange(listaRegras);
                foreach (Dominio.Entidades.Embarcador.Frete.RegraContratoFreteTransportador regra in listaRegras)
                {
                    if (regra.RegraPorTransportador)
                    {
                        bool valido = false;
                        if (regra.AlcadasTransportadores.All(o => o.Condicao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CondicaoAutorizao.IgualA && o.Juncao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.JuncaoAutorizao.E && o.Transportador.Codigo == contrato.Transportador.Codigo))
                            valido = true;
                        else if (regra.AlcadasTransportadores.Any(o => o.Condicao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CondicaoAutorizao.IgualA && o.Juncao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.JuncaoAutorizao.Ou && o.Transportador.Codigo == contrato.Transportador.Codigo))
                            valido = true;
                        else if (regra.AlcadasTransportadores.Any(o => o.Condicao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CondicaoAutorizao.DiferenteDe && o.Juncao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.JuncaoAutorizao.Ou && o.Transportador.Codigo != contrato.Transportador.Codigo))
                            valido = true;
                        else if (regra.AlcadasTransportadores.All(o => o.Condicao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CondicaoAutorizao.DiferenteDe && o.Juncao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.JuncaoAutorizao.E && o.Transportador.Codigo != contrato.Transportador.Codigo))
                            valido = true;

                        if (!valido)
                        {
                            listaFiltrada.Remove(regra);
                            continue;
                        }
                    }

                    if (regra.RegraPorFilial)
                    {
                        bool valido = false;
                        if (regra.AlcadasFilial.All(o => o.Condicao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CondicaoAutorizao.IgualA && o.Juncao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.JuncaoAutorizao.E && contrato.Filiais.Any(f => f.Filial.Codigo == o.Filial.Codigo)))
                            valido = true;
                        else if (regra.AlcadasFilial.Any(o => o.Condicao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CondicaoAutorizao.IgualA && o.Juncao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.JuncaoAutorizao.Ou && contrato.Filiais.Any(f => f.Filial.Codigo == o.Filial.Codigo)))
                            valido = true;
                        else if (regra.AlcadasFilial.Any(o => o.Condicao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CondicaoAutorizao.DiferenteDe && o.Juncao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.JuncaoAutorizao.Ou && contrato.Filiais.Any(f => f.Filial.Codigo != o.Filial.Codigo)))
                            valido = true;
                        else if (regra.AlcadasFilial.All(o => o.Condicao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CondicaoAutorizao.DiferenteDe && o.Juncao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.JuncaoAutorizao.E && contrato.Filiais.Any(f => f.Filial.Codigo != o.Filial.Codigo)))
                            valido = true;

                        if (!valido)
                        {
                            listaFiltrada.Remove(regra);
                            continue;
                        }
                    }

                    if (regra.RegraPorValorContrato)
                    {
                        decimal valorContrato = contrato.FranquiaContratoMensal;
                        if (contrato.TipoFranquia == Dominio.ObjetosDeValor.Embarcador.Enumeradores.PeriodoAcordoContratoFreteTransportador.NaoPossui)
                            valorContrato = contrato.ValorMensal;

                        bool valido = false;
                        if (regra.RegrasContratoValorContrato.All(o => o.Condicao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CondicaoAutorizao.IgualA && o.Juncao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.JuncaoAutorizao.E && o.Valor == valorContrato))
                            valido = true;
                        else if (regra.RegrasContratoValorContrato.Any(o => o.Condicao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CondicaoAutorizao.IgualA && o.Juncao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.JuncaoAutorizao.Ou && o.Valor == valorContrato))
                            valido = true;
                        else if (regra.RegrasContratoValorContrato.Any(o => o.Condicao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CondicaoAutorizao.DiferenteDe && o.Juncao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.JuncaoAutorizao.Ou && o.Valor != valorContrato))
                            valido = true;
                        else if (regra.RegrasContratoValorContrato.All(o => o.Condicao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CondicaoAutorizao.DiferenteDe && o.Juncao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.JuncaoAutorizao.E && o.Valor != valorContrato))
                            valido = true;
                        if (regra.RegrasContratoValorContrato.All(o => o.Condicao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CondicaoAutorizao.MaiorIgualQue && o.Juncao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.JuncaoAutorizao.E && valorContrato >= o.Valor))
                            valido = true;
                        else if (regra.RegrasContratoValorContrato.Any(o => o.Condicao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CondicaoAutorizao.MaiorIgualQue && o.Juncao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.JuncaoAutorizao.Ou && valorContrato >= o.Valor))
                            valido = true;
                        if (regra.RegrasContratoValorContrato.All(o => o.Condicao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CondicaoAutorizao.MenorIgualQue && o.Juncao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.JuncaoAutorizao.E && valorContrato <= o.Valor))
                            valido = true;
                        else if (regra.RegrasContratoValorContrato.Any(o => o.Condicao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CondicaoAutorizao.MenorIgualQue && o.Juncao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.JuncaoAutorizao.Ou && valorContrato <= o.Valor))
                            valido = true;
                        if (regra.RegrasContratoValorContrato.All(o => o.Condicao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CondicaoAutorizao.MaiorQue && o.Juncao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.JuncaoAutorizao.E && valorContrato > o.Valor))
                            valido = true;
                        else if (regra.RegrasContratoValorContrato.Any(o => o.Condicao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CondicaoAutorizao.MaiorQue && o.Juncao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.JuncaoAutorizao.Ou && valorContrato > o.Valor))
                            valido = true;
                        if (regra.RegrasContratoValorContrato.All(o => o.Condicao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CondicaoAutorizao.MenorQue && o.Juncao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.JuncaoAutorizao.E && valorContrato < o.Valor))
                            valido = true;
                        else if (regra.RegrasContratoValorContrato.Any(o => o.Condicao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CondicaoAutorizao.MenorQue && o.Juncao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.JuncaoAutorizao.Ou && valorContrato < o.Valor))
                            valido = true;

                        if (!valido)
                        {
                            listaFiltrada.Remove(regra);
                            continue;
                        }
                    }
                }
            }

            return listaFiltrada;
        }

        public static bool LiberarProximasHierarquiasDeAprovacao(Dominio.Entidades.Embarcador.Frete.ContratoFreteTransportador contrato, Dominio.Entidades.Usuario usuario, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, string stringConexao, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Frete.AprovacaoAlcadaContratoFreteTransportador repositorioAprovacaoAlcadaContratoFreteTransportador = new Repositorio.Embarcador.Frete.AprovacaoAlcadaContratoFreteTransportador(unitOfWork);
            List<Dominio.Entidades.Embarcador.Frete.AprovacaoAlcadaContratoFreteTransportador> alcadasAprovacao = repositorioAprovacaoAlcadaContratoFreteTransportador.BuscarPendentesPorContrato(contrato.Codigo);

            if (alcadasAprovacao.Count > 0)
            {
                int menorPrioridadeAprovacao = alcadasAprovacao.Select(obj => obj.RegraContratoFreteTransportador.PrioridadeAprovacao).Min();

                foreach (Dominio.Entidades.Embarcador.Frete.AprovacaoAlcadaContratoFreteTransportador aprovacao in alcadasAprovacao)
                {
                    if (aprovacao.RegraContratoFreteTransportador.PrioridadeAprovacao <= menorPrioridadeAprovacao)
                    {
                        aprovacao.Bloqueada = false;
                        repositorioAprovacaoAlcadaContratoFreteTransportador.Atualizar(aprovacao);

                        foreach (Dominio.Entidades.Usuario aprovador in aprovacao.RegraContratoFreteTransportador.Aprovadores)
                            NotificarUsuario(aprovador, contrato, usuario, tipoServicoMultisoftware, stringConexao, unitOfWork);
                    }
                }

                return false;
            }

            return true;
        }

        public static bool CriarRegrasAutorizacao(List<Dominio.Entidades.Embarcador.Frete.RegraContratoFreteTransportador> listaFiltrada, Dominio.Entidades.Embarcador.Frete.ContratoFreteTransportador contrato, Dominio.Entidades.Usuario usuario, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, string stringConexao, Repositorio.UnitOfWork unitOfWork)
        {
            if (listaFiltrada == null || listaFiltrada.Count() == 0)
                throw new ArgumentException("Lista de Regras deve ser maior que 0");

            Repositorio.Embarcador.Frete.AprovacaoAlcadaContratoFreteTransportador repositorioAprovacaoAlcadaContratoFreteTransportador = new Repositorio.Embarcador.Frete.AprovacaoAlcadaContratoFreteTransportador(unitOfWork);
            List<Dominio.Entidades.Usuario> aprovadoresPorTransportador;
            bool possuiRegraPendente = false;
            int menorPrioridadeAprovacao = listaFiltrada.Where(regra => regra.NumeroAprovadores > 0).Select(regra => (int?)regra.PrioridadeAprovacao).Min() ?? 0;

            if (listaFiltrada.Any(regra => regra.TipoAprovadorRegra == TipoAprovadorRegra.Transportador))
                aprovadoresPorTransportador = new Repositorio.Usuario(unitOfWork).BuscarUsuariosAcessoTransportador(contrato.Transportador?.Codigo ?? 0);
            else
                aprovadoresPorTransportador = new List<Dominio.Entidades.Usuario>();

            foreach (Dominio.Entidades.Embarcador.Frete.RegraContratoFreteTransportador regra in listaFiltrada)
            {
                if (regra.NumeroAprovadores > 0)
                {
                    possuiRegraPendente = true;
                    List<Dominio.Entidades.Usuario> aprovadores;

                    if (regra.TipoAprovadorRegra == TipoAprovadorRegra.Usuario)
                        aprovadores = regra.Aprovadores.ToList();
                    else
                        aprovadores = aprovadoresPorTransportador;

                    foreach (Dominio.Entidades.Usuario aprovador in aprovadores)
                    {
                        Dominio.Entidades.Embarcador.Frete.AprovacaoAlcadaContratoFreteTransportador autorizacao = new Dominio.Entidades.Embarcador.Frete.AprovacaoAlcadaContratoFreteTransportador
                        {
                            ContratoFreteTransportador = contrato,
                            Usuario = aprovador,
                            RegraContratoFreteTransportador = regra,
                            DataCriacao = contrato.DataAlteracao,
                            NumeroAprovadores = regra.NumeroAprovadores,
                            Bloqueada = regra.PrioridadeAprovacao > menorPrioridadeAprovacao,
                            TipoAprovadorRegra = regra.TipoAprovadorRegra
                        };

                        repositorioAprovacaoAlcadaContratoFreteTransportador.Inserir(autorizacao);

                        if (!autorizacao.Bloqueada)
                            NotificarUsuario(aprovador, contrato, usuario, tipoServicoMultisoftware, stringConexao, unitOfWork);
                    }
                }
                else
                {
                    Dominio.Entidades.Embarcador.Frete.AprovacaoAlcadaContratoFreteTransportador autorizacao = new Dominio.Entidades.Embarcador.Frete.AprovacaoAlcadaContratoFreteTransportador
                    {
                        ContratoFreteTransportador = contrato,
                        Usuario = null,
                        RegraContratoFreteTransportador = regra,
                        Situacao = SituacaoAlcadaRegra.Aprovada,
                        Data = DateTime.Now,
                        Motivo = $"Alçada aprovada pela Regra {regra.Descricao}",
                        DataCriacao = contrato.DataAlteracao,
                        TipoAprovadorRegra = regra.TipoAprovadorRegra
                    };

                    repositorioAprovacaoAlcadaContratoFreteTransportador.Inserir(autorizacao);
                }
            }

            return possuiRegraPendente;
        }

        public static void ContratoAprovado(Dominio.Entidades.Embarcador.Frete.ContratoFreteTransportador contrato, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado, Repositorio.UnitOfWork unitOfWork)
        {
            if (contrato.Situacao != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoContratoFreteTransportador.Aprovado)
                return;

            AtualizarVigenciaTabelaFrete(contrato, unitOfWork);
        }

        public static void VerificarContratosVencendo(Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Frete.ContratoFreteTransportador repContratoFreteTransportador = new Repositorio.Embarcador.Frete.ContratoFreteTransportador(unitOfWork);
            Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repConfiguracaoTMS = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork);


            Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao = repConfiguracaoTMS.BuscarConfiguracaoPadrao();

            int dias = configuracao?.DiasAvisoVencimentoCotratoFrete ?? 0;
            List<string> emails = ExtraiListaDeEmails(configuracao?.EmailsAvisoVencimentoCotratoFrete ?? "");

            if (dias > 0 && emails.Count > 0)
            {
                DateTime prazo = DateTime.Today.AddDays(dias);
                List<int> codigoContratos = repContratoFreteTransportador.BuscarContratosVencendo(prazo);
                string listaEmails = string.Join("; ", emails);

                for (int i = 0, s = codigoContratos.Count; i < s; i++)
                {
                    Dominio.Entidades.Embarcador.Frete.ContratoFreteTransportador contrato = repContratoFreteTransportador.BuscarPorCodigo(codigoContratos[i]);
                    NotificarContratoProximoDoVencimento(contrato, dias, listaEmails, unitOfWork);
                }
            }
        }

        public static List<string> ExtraiListaDeEmails(string emails)
        {
            List<string> listaEmails = emails.Split(';').ToList();

            listaEmails = (from e in listaEmails select e.Trim()).ToList();
            listaEmails = (from e in listaEmails where !string.IsNullOrWhiteSpace(e) select e).ToList();

            return listaEmails;
        }

        public static void NotificarContratoProximoDoVencimento(Dominio.Entidades.Embarcador.Frete.ContratoFreteTransportador contrato, int dias, string listaEmails, Repositorio.UnitOfWork unitOfWork)
        {
            string subject = "Contrato de Frete próximo de vencimento";
            string body = $"O Contrato de número {contrato.Numero}{(contrato.Transportador != null ? $" do transportador {contrato.Transportador.Descricao}" : "")} vencerá em {dias} dias.";
            string emails = listaEmails;

            if ((contrato.Transportador != null) && (contrato.Transportador.StatusEmail == "A") && !string.IsNullOrEmpty(contrato.Transportador.Email))
                emails = listaEmails + "; " + contrato.Transportador.Email;

            List<System.Net.Mail.Attachment> anexos = null;
            if (contrato.TipoContratoFrete?.TipoAditivo == true)
            {
                byte[] pdf = RelatorioContratoFreteAditivo(contrato.Codigo, contrato.NumeroAditivo + 1, unitOfWork);
                anexos = new List<System.Net.Mail.Attachment>()
                {
                    new System.Net.Mail.Attachment(new System.IO.MemoryStream(pdf), "Contrato de Frete Transportador - " + contrato.Numero + ".pdf", "application/pdf")
                };
            }

            string[] splitEmail = emails.Split(';');
            string email = splitEmail[0];
            List<string> cc = new List<string>();

            if (splitEmail.Length > 1)
            {
                for (int i = 1; i < splitEmail.Length; i++)
                {
                    cc.Add(splitEmail[i]);
                }
            }

            if (!Servicos.Email.EnviarEmailAutenticado(email, subject, body, unitOfWork, out string msg, "", anexos, null, cc.ToArray()))
            {
                Servicos.Log.TratarErro("Falha ao enviar notificação de vencimento Contrato Frete n" + contrato.Numero + ": " + msg);
            }
        }

        public static byte[] RelatorioContratoFrete(Dominio.Entidades.Embarcador.Frete.ContratoFreteTransportador contrato, bool excell, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware TipoServicoMultisoftware, Repositorio.UnitOfWork unitOfWork)
        {
            bool tipoAditivo = contrato.TipoContratoFrete?.TipoAditivo ?? false;

            if (tipoAditivo)
                return RelatorioContratoFreteAditivo(contrato.Codigo, contrato.NumeroAditivo, unitOfWork);

            return RelatorioContratoFretePadrao(contrato.Codigo, excell, TipoServicoMultisoftware, unitOfWork);
        }

        public static void ValidarContratosVencidos(Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Frete.ContratoFreteTransportador repContratoFreteTransportador = new Repositorio.Embarcador.Frete.ContratoFreteTransportador(unitOfWork);

            List<Dominio.Entidades.Embarcador.Frete.ContratoFreteTransportador> contratos = repContratoFreteTransportador.BuscarContratosVencidos(DateTime.Today);

            foreach (Dominio.Entidades.Embarcador.Frete.ContratoFreteTransportador contrato in contratos)
            {
                contrato.Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoContratoFreteTransportador.Vencido;
                repContratoFreteTransportador.Atualizar(contrato);
            }
        }

        public static void RatearContratoSaldoMesCTes(Dominio.Entidades.Embarcador.Cargas.Carga carga, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Frete.ContratoSaldoMes repositorioContratoSaldoMes = new Repositorio.Embarcador.Frete.ContratoSaldoMes(unitOfWork);
            Repositorio.Embarcador.Frete.ContratoSaldoMesCTe repositorioContratoSaldoMesCTe = new Repositorio.Embarcador.Frete.ContratoSaldoMesCTe(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaCTe repositorioCargaCTe = new Repositorio.Embarcador.Cargas.CargaCTe(unitOfWork);

            List<Dominio.Entidades.Embarcador.Frete.ContratoSaldoMes> saldoMes = repositorioContratoSaldoMes.BuscarPorCarga(carga.Codigo);
            if (saldoMes.Count == 0)
                return;

            List<Dominio.Entidades.Embarcador.Cargas.CargaCTe> cargaCTes = repositorioCargaCTe.BuscarPorCarga(carga.Codigo);
            if (cargaCTes.Count == 0)
                return;

            decimal valorTotalSaldo = saldoMes.Where(o => !o.Excedente).Sum(o => o.ValorPagar);
            decimal distanciaTotalSaldo = saldoMes.Where(o => !o.Excedente).Sum(o => o.Distancia);
            decimal valorExcedenteTotalSaldo = saldoMes.Where(o => o.Excedente).Sum(o => o.ValorPagar);
            decimal distanciaExcedenteTotalSaldo = saldoMes.Where(o => o.Excedente).Sum(o => o.Distancia);

            for (int i = 0, s = cargaCTes.Count; i < s; i++)
            {
                Dominio.Entidades.Embarcador.Cargas.CargaCTe cargaCTe = cargaCTes[i];
                bool isUltimoCTe = (i + 1) == s;

                Dominio.Entidades.Embarcador.Frete.ContratoSaldoMesCTe contratoSaldoMesCTe = new Dominio.Entidades.Embarcador.Frete.ContratoSaldoMesCTe()
                {
                    CTe = cargaCTe,
                    ValorPagar = Math.Round(valorTotalSaldo / s, 2, MidpointRounding.AwayFromZero),
                    ValorPagarExcedente = Math.Round(valorExcedenteTotalSaldo / s, 2, MidpointRounding.AwayFromZero),
                    Distancia = Math.Round(distanciaTotalSaldo / s, 2, MidpointRounding.AwayFromZero),
                    DistanciaExcedente = Math.Round(distanciaExcedenteTotalSaldo / s, 2, MidpointRounding.AwayFromZero),
                };

                if (isUltimoCTe)
                {
                    contratoSaldoMesCTe.ValorPagar = valorTotalSaldo - contratoSaldoMesCTe.ValorPagar * (s - 1);
                    contratoSaldoMesCTe.ValorPagarExcedente = valorExcedenteTotalSaldo - contratoSaldoMesCTe.ValorPagarExcedente * (s - 1);
                    contratoSaldoMesCTe.Distancia = distanciaTotalSaldo - contratoSaldoMesCTe.Distancia * (s - 1);
                    contratoSaldoMesCTe.DistanciaExcedente = distanciaExcedenteTotalSaldo - contratoSaldoMesCTe.DistanciaExcedente * (s - 1);
                }

                repositorioContratoSaldoMesCTe.Inserir(contratoSaldoMesCTe);
            }
        }

        public void VerificarIntegracoesPendentes()
        {
            Repositorio.Embarcador.Frete.ContratoFreteTransportadorIntegracao repositorioContratoFreteTransportador = new Repositorio.Embarcador.Frete.ContratoFreteTransportadorIntegracao(_unitOfWork);
            List<Dominio.Entidades.Embarcador.Frete.ContratoFreteTransportadorIntegracao> integracoesPendentesContratoFreteTransportador = repositorioContratoFreteTransportador.BuscarIntegracoesPendentes(3, 5, false, true);

            foreach (Dominio.Entidades.Embarcador.Frete.ContratoFreteTransportadorIntegracao integracaoPendenteContratoFreteTransportador in integracoesPendentesContratoFreteTransportador)
            {
                switch (integracaoPendenteContratoFreteTransportador.TipoIntegracao.Tipo)
                {
                    case TipoIntegracao.LBC:
                        new Embarcador.Integracao.LBC.IntegracaoLBC(_unitOfWork).IntegrarContratoFreteTransportador(integracaoPendenteContratoFreteTransportador);
                        break;

                    default:
                        integracaoPendenteContratoFreteTransportador.DataIntegracao = DateTime.Now;
                        integracaoPendenteContratoFreteTransportador.NumeroTentativas++;
                        integracaoPendenteContratoFreteTransportador.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
                        integracaoPendenteContratoFreteTransportador.ProblemaIntegracao = "Integração não configurada";

                        repositorioContratoFreteTransportador.Atualizar(integracaoPendenteContratoFreteTransportador);
                        break;
                }
            }
        }

        #endregion

        #region Métodos Privados

        private static void AtualizarVigenciaTabelaFrete(Dominio.Entidades.Embarcador.Frete.ContratoFreteTransportador contrato, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Frete.TabelaFrete repTabelaFrete = new Repositorio.Embarcador.Frete.TabelaFrete(unitOfWork);
            Repositorio.Embarcador.Frete.VigenciaTabelaFrete repVigenciaTabelaFrete = new Repositorio.Embarcador.Frete.VigenciaTabelaFrete(unitOfWork);

            List<Dominio.Entidades.Embarcador.Frete.TabelaFrete> tabelas = repTabelaFrete.BuscarPorContratoFreteTransportador(contrato.Codigo);

            foreach (var tabela in tabelas)
            {
                foreach (Dominio.Entidades.Embarcador.Frete.VigenciaTabelaFrete vigencia in tabela.Vigencias)
                {

                    vigencia.DataInicial = contrato.DataInicial;
                    vigencia.DataFinal = contrato.DataFinal;

                    repVigenciaTabelaFrete.Atualizar(vigencia);
                }
            }
        }

        private static void NotificarUsuario(Dominio.Entidades.Usuario aprovador, Dominio.Entidades.Embarcador.Frete.ContratoFreteTransportador contrato, Dominio.Entidades.Usuario usuario, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, string stringConexao, Repositorio.UnitOfWork unitOfWork)
        {
            Servicos.Embarcador.Notificacao.Notificacao serNotificacao = new Servicos.Embarcador.Notificacao.Notificacao(stringConexao, null, tipoServicoMultisoftware, string.Empty);

            string titulo = Localization.Resources.Fretes.ContratoFreteTransportador.TituloContratoFreteTransportador;
            string nota = string.Empty;
            nota = string.Format(Localization.Resources.Fretes.ContratoFreteTransportador.UsuarioAlterouContrato, usuario.Nome, contrato.Numero);
            serNotificacao.GerarNotificacaoEmail(aprovador, usuario, contrato.Codigo, "Fretes/ContratoFreteTransportador", titulo, nota, Dominio.ObjetosDeValor.Embarcador.Enumeradores.IconesNotificacao.cifra, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoNotificacao.credito, tipoServicoMultisoftware, unitOfWork);
        }

        private static byte[] RelatorioContratoFreteAditivo(int CodigoContrato, int numeroAditivo, Repositorio.UnitOfWork unitOfWork)
        {

            return ReportRequest.WithType(ReportType.RelatorioContratoFreteAditivo)
                .WithExecutionType(ExecutionType.Sync)
                .AddExtraData("CodigoContrato", CodigoContrato.ToString())
                .AddExtraData("NumeroAditivo", numeroAditivo.ToString())
                .CallReport()
                .GetContentFile();

        }

        private static byte[] RelatorioContratoFretePadrao(int codigoContrato, bool excell, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware TipoServicoMultisoftware, Repositorio.UnitOfWork unitOfWork)
        {

          return  ReportRequest.WithType(ReportType.RelatorioContratoFretePadrao)
                .WithExecutionType(ExecutionType.Sync)
                .AddExtraData("CodigoContrato", codigoContrato.ToString())
                .AddExtraData("Excel", excell.ToString())
                .CallReport()
                .GetContentFile();
        }

        #endregion
    }
}
