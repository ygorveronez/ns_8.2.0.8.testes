using System;
using System.Collections.Generic;
using System.Linq;

namespace Servicos.Embarcador.CargaOcorrencia
{
    public class ValorParametroOcorrencia
    {

        public static Servicos.Embarcador.CargaOcorrencia.ValorParametroOcorrencia GetInstance()
        {
            return new ValorParametroOcorrencia();
        }

        public static Dominio.Entidades.Embarcador.Ocorrencias.ValorParametroOcorrencia ObterValorParametroOcorrencia(int tipoOcorrencia, int codigoCarga, DateTime data, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Ocorrencias.ValorParametroOcorrencia repValorParametroOcorrencia = new Repositorio.Embarcador.Ocorrencias.ValorParametroOcorrencia(unitOfWork);
            Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);

            Dominio.Entidades.Embarcador.Cargas.Carga carga = repCarga.BuscarPorCodigo(codigoCarga);

            int codigoTipoOperacao = 0;
            if (carga.TipoOperacao != null)
                codigoTipoOperacao = carga.TipoOperacao.Codigo;
            int grupoPessoa = carga.GrupoPessoaPrincipal?.Codigo ?? 0;
            double pessoa = 0;
            if (carga.Pedidos.Count() > 0)
                pessoa = carga.Pedidos.FirstOrDefault().Pedido?.Destinatario?.CPF_CNPJ ?? 0;


            return repValorParametroOcorrencia.BuscarPorTipoOcorrenciaPessoaGrupoVigencia(codigoTipoOperacao, tipoOcorrencia, pessoa, grupoPessoa, data);
        }

        public static List<Dominio.ObjetosDeValor.Embarcador.Ocorrencia.OcorrenciaPorParametro> CalcularOcorrencias(TimeSpan horaInicial, TimeSpan horaFinal, int kmInicial, int kmFinal, int codigoCarga, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoParametroCalculoOcorrencia tipoParametro, Dominio.Entidades.Embarcador.Ocorrencias.ValorParametroOcorrencia tabela, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);

            // Ocorrencia Resultante
            List<Dominio.ObjetosDeValor.Embarcador.Ocorrencia.OcorrenciaPorParametro> ocorrencias = new List<Dominio.ObjetosDeValor.Embarcador.Ocorrencia.OcorrenciaPorParametro>();

            // Carga da ocorrencia
            Dominio.Entidades.Embarcador.Cargas.Carga carga = repCarga.BuscarPorCodigo(codigoCarga);

            // Extrai parametros
            int modeloVeicular = 0;
            if (carga.ModeloVeicularCarga != null)
                modeloVeicular = carga.ModeloVeicularCarga.Codigo;
            else
                modeloVeicular = carga.Veiculo?.ModeloVeicularCarga?.Codigo ?? 0;
            int quantidadeAjudantes = (from pedido in carga.Pedidos select pedido.Pedido.QtdAjudantes).Sum();

            int fracaoCalculo = 15;

            switch (tipoParametro)
            {
                case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoParametroCalculoOcorrencia.HorasExtra:
                    return Servicos.Embarcador.CargaOcorrencia.ValorParametroOcorrencia.GerarOcorrenciasHoraExtra(horaInicial, horaFinal, tabela, modeloVeicular, quantidadeAjudantes, fracaoCalculo);
                case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoParametroCalculoOcorrencia.Estadia:
                    return Servicos.Embarcador.CargaOcorrencia.ValorParametroOcorrencia.GerarOcorrenciasEstadia(horaInicial, horaFinal, tabela, modeloVeicular, quantidadeAjudantes, fracaoCalculo);
                case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoParametroCalculoOcorrencia.Pernoite:
                    return Servicos.Embarcador.CargaOcorrencia.ValorParametroOcorrencia.GerarOcorrenciasPernoite(tabela, modeloVeicular);
                case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoParametroCalculoOcorrencia.Escolta:
                    return Servicos.Embarcador.CargaOcorrencia.ValorParametroOcorrencia.GerarOcorrenciasEscolta(horaInicial, horaFinal, kmInicial, kmFinal, tabela, modeloVeicular, fracaoCalculo);
            }

            return null;
        }

        public static List<Dominio.ObjetosDeValor.Embarcador.Ocorrencia.OcorrenciaPorParametro> GerarOcorrenciasEscolta(TimeSpan horaInicial, TimeSpan horaFinal, int kmInicial, int kmFinal, Dominio.Entidades.Embarcador.Ocorrencias.ValorParametroOcorrencia tabela, int modeloVeicular, int fracaoCalculo)
        {
            Dominio.Entidades.Embarcador.Ocorrencias.ValorParametroEscoltaOcorrencia configuracao = tabela.ValorParametroEscoltaOcorrencia;

            decimal minutos = 0;
            decimal horas = 0;
            decimal valorHoraExcedente = 0;

            if (horaInicial < horaFinal)
                minutos = (decimal)horaFinal.Subtract(horaInicial).TotalMinutes;
            else
            {
                minutos += (decimal)(new TimeSpan(24, 0, 0)).Subtract(horaInicial).TotalMinutes;
                minutos += (decimal)horaFinal.Subtract((new TimeSpan(0, 0, 0))).TotalMinutes;
            }

            minutos = Math.Floor(minutos / fracaoCalculo) * fracaoCalculo;
            horas = (minutos / 60) - ((decimal)(configuracao.HoraContratado.HasValue ? configuracao.HoraContratado.Value.TotalMinutes : 0)) / 60;
            if (horas > 0)
                valorHoraExcedente = horas * configuracao.ValorHoraExcedente;


            int kmExcedente = (kmFinal - kmInicial) - configuracao.KmContratado;
            decimal valorKmExcedente = 0;
            if (kmExcedente > 0)
                valorKmExcedente = kmExcedente * configuracao.ValorKmExcedente;

            return new List<Dominio.ObjetosDeValor.Embarcador.Ocorrencia.OcorrenciaPorParametro>()
            {
                new Dominio.ObjetosDeValor.Embarcador.Ocorrencia.OcorrenciaPorParametro()
                {
                    TipoParametro = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoParametroCalculoOcorrencia.Escolta,
                    Titulo = "Escolta",
                    Valor = valorKmExcedente + valorHoraExcedente,
                    ComponenteFrete = tabela.ValorParametroEscoltaOcorrencia.ComponenteFrete.Codigo
                }
            };
        }

        public static List<Dominio.ObjetosDeValor.Embarcador.Ocorrencia.OcorrenciaPorParametro> GerarOcorrenciasPernoite(Dominio.Entidades.Embarcador.Ocorrencias.ValorParametroOcorrencia tabela, int modeloVeicular)
        {
            ValorParametroOcorrencia instance = GetInstance();

            List<Dominio.ObjetosDeValor.Embarcador.Ocorrencia.OcorrenciaPorParametro> ocorrencias = new List<Dominio.ObjetosDeValor.Embarcador.Ocorrencia.OcorrenciaPorParametro>();

            Dominio.Entidades.Embarcador.Ocorrencias.ValorParametroPernoiteVeiculo configuracaoVeiculo = (from o in tabela.ValorParametroPernoiteOcorrencia.Veiculos where o.ModeloVeicular.Codigo == modeloVeicular select o).FirstOrDefault();

            if (configuracaoVeiculo != null)
            {
                ocorrencias.Add(new Dominio.ObjetosDeValor.Embarcador.Ocorrencia.OcorrenciaPorParametro()
                {
                    TipoParametro = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoParametroCalculoOcorrencia.Pernoite,
                    Titulo = "Pernoite",
                    Valor = configuracaoVeiculo.Valor,
                    ComponenteFrete = tabela.ValorParametroPernoiteOcorrencia.ComponenteFrete.Codigo
                });
            }


            return ocorrencias;
        }

        public static List<Dominio.ObjetosDeValor.Embarcador.Ocorrencia.OcorrenciaPorParametro> GerarOcorrenciasEstadia(TimeSpan horaInicial, TimeSpan horaFinal, Dominio.Entidades.Embarcador.Ocorrencias.ValorParametroOcorrencia tabela, int modeloVeicular, int quantidadeAjudantes, int fracaoCalculo)
        {
            ValorParametroOcorrencia instance = GetInstance();

            List<Dominio.ObjetosDeValor.Embarcador.Ocorrencia.OcorrenciaPorParametro> ocorrencias = new List<Dominio.ObjetosDeValor.Embarcador.Ocorrencia.OcorrenciaPorParametro>();

            List<Dominio.ObjetosDeValor.Embarcador.Ocorrencia.ConfiguracaoFaixaHorario> configuracoesVeiculos = null;
            configuracoesVeiculos = (from o in tabela.ValorParametroEstadiaOcorrencia.Veiculos
                                     where o.ModeloVeicular.Codigo == modeloVeicular
                                     orderby o.HoraInicial ascending
                                     select new Dominio.ObjetosDeValor.Embarcador.Ocorrencia.ConfiguracaoFaixaHorario
                                     {
                                         HoraInicial = o.HoraInicial,
                                         HoraFinal = o.HoraFinal,
                                         Valor = o.Valor
                                     }).ToList();

            List<Dominio.ObjetosDeValor.Embarcador.Ocorrencia.ConfiguracaoFaixaHorario> configuracoesAjudantes = null;
            configuracoesAjudantes = (from o in tabela.ValorParametroEstadiaOcorrencia.Ajudantes
                                      orderby o.HoraInicial ascending
                                      select new Dominio.ObjetosDeValor.Embarcador.Ocorrencia.ConfiguracaoFaixaHorario
                                      {
                                          HoraInicial = o.HoraInicial,
                                          HoraFinal = o.HoraFinal,
                                          Valor = o.Valor
                                      }).ToList();


            string tituloVeiculos = "Estadia Veículo";
            string tituloAjudantes = "Estadia Ajudante";

            if (horaInicial < horaFinal) // Não ha virada de dia
            {
                // Calcula do inicio ao fim
                if (tabela.ValorParametroEstadiaOcorrencia.ComponenteFreteVeiculo != null)
                    ocorrencias.Add(CalcularOcorrenciaPorFaixaDeHorario(horaInicial, horaFinal, configuracoesVeiculos, tituloVeiculos, fracaoCalculo, tabela.ValorParametroEstadiaOcorrencia.ComponenteFreteVeiculo.Codigo));

                if (quantidadeAjudantes > 0 && tabela.ValorParametroEstadiaOcorrencia.ComponenteFreteAjudante != null)
                    ocorrencias.Add(CalcularOcorrenciaPorFaixaDeHorario(horaInicial, horaFinal, configuracoesAjudantes, tituloAjudantes, fracaoCalculo, tabela.ValorParametroEstadiaOcorrencia.ComponenteFreteAjudante.Codigo, quantidadeAjudantes));
            }
            else
            {
                List<Dominio.ObjetosDeValor.Embarcador.Ocorrencia.OcorrenciaPorParametro> ocorrenciasCalculadasEmDoisTempos = new List<Dominio.ObjetosDeValor.Embarcador.Ocorrencia.OcorrenciaPorParametro>();

                if (tabela.ValorParametroEstadiaOcorrencia.ComponenteFreteVeiculo != null)
                {
                    ocorrenciasCalculadasEmDoisTempos = new List<Dominio.ObjetosDeValor.Embarcador.Ocorrencia.OcorrenciaPorParametro>
                    {
                        // Calcula primeiro pedaço 
                        CalcularOcorrenciaPorFaixaDeHorario(horaInicial, new TimeSpan(24, 0, 0), configuracoesVeiculos, tituloVeiculos, fracaoCalculo, tabela.ValorParametroEstadiaOcorrencia.ComponenteFreteVeiculo.Codigo),

                        // Calcula segundo pedaço
                        CalcularOcorrenciaPorFaixaDeHorario(new TimeSpan(0, 0, 0), horaFinal, configuracoesVeiculos, tituloVeiculos, fracaoCalculo, tabela.ValorParametroEstadiaOcorrencia.ComponenteFreteVeiculo.Codigo)
                    };
                    instance.SetaCalculoEmDoisTemposSumarizado(ref ocorrencias, ocorrenciasCalculadasEmDoisTempos);
                }


                if (quantidadeAjudantes > 0 && tabela.ValorParametroEstadiaOcorrencia.ComponenteFreteAjudante != null)
                {
                    ocorrenciasCalculadasEmDoisTempos = new List<Dominio.ObjetosDeValor.Embarcador.Ocorrencia.OcorrenciaPorParametro>
                    {
                        // Calcula primeiro pedaço 
                        CalcularOcorrenciaPorFaixaDeHorario(horaInicial, new TimeSpan(24, 0, 0), configuracoesAjudantes, tituloAjudantes, fracaoCalculo, tabela.ValorParametroEstadiaOcorrencia.ComponenteFreteAjudante.Codigo, quantidadeAjudantes),

                        // Calcula segundo pedaço
                        CalcularOcorrenciaPorFaixaDeHorario(new TimeSpan(0, 0, 0), horaFinal, configuracoesAjudantes, tituloAjudantes, fracaoCalculo, tabela.ValorParametroEstadiaOcorrencia.ComponenteFreteAjudante.Codigo, quantidadeAjudantes)
                    };
                    instance.SetaCalculoEmDoisTemposSumarizado(ref ocorrencias, ocorrenciasCalculadasEmDoisTempos);
                }

            }

            // Seta o tipo parametro
            foreach (var ocorrencia in ocorrencias)
                ocorrencia.TipoParametro = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoParametroCalculoOcorrencia.Estadia;

            return ocorrencias;
        }

        public static List<Dominio.ObjetosDeValor.Embarcador.Ocorrencia.OcorrenciaPorParametro> GerarOcorrenciasHoraExtra(TimeSpan horaInicial, TimeSpan horaFinal, Dominio.Entidades.Embarcador.Ocorrencias.ValorParametroOcorrencia tabela, int modeloVeicular, int quantidadeAjudantes, int fracaoCalculo)
        {
            ValorParametroOcorrencia instance = GetInstance();

            List<Dominio.ObjetosDeValor.Embarcador.Ocorrencia.OcorrenciaPorParametro> ocorrencias = new List<Dominio.ObjetosDeValor.Embarcador.Ocorrencia.OcorrenciaPorParametro>();

            List<Dominio.ObjetosDeValor.Embarcador.Ocorrencia.ConfiguracaoFaixaHorario> configuracoesVeiculos = null;
            configuracoesVeiculos = (from o in tabela.ValorParametroHoraExtraOcorrencia.Veiculos
                                     where o.ModeloVeicular.Codigo == modeloVeicular
                                     orderby o.HoraInicial ascending
                                     select new Dominio.ObjetosDeValor.Embarcador.Ocorrencia.ConfiguracaoFaixaHorario
                                     {
                                         HoraInicial = o.HoraInicial,
                                         HoraFinal = o.HoraFinal,
                                         Valor = o.Valor
                                     }).ToList();

            List<Dominio.ObjetosDeValor.Embarcador.Ocorrencia.ConfiguracaoFaixaHorario> configuracoesAjudantes = null;
            configuracoesAjudantes = (from o in tabela.ValorParametroHoraExtraOcorrencia.Ajudantes
                                      orderby o.HoraInicial ascending
                                      select new Dominio.ObjetosDeValor.Embarcador.Ocorrencia.ConfiguracaoFaixaHorario
                                      {
                                          HoraInicial = o.HoraInicial,
                                          HoraFinal = o.HoraFinal,
                                          Valor = o.Valor
                                      }).ToList();


            string tituloVeiculos = "Hora Extra Veículo";
            string tituloAjudantes = "Hora Extra Ajudante";

            if (horaInicial < horaFinal) // Não ha virada de dia
            {
                // Calcula do inicio ao fim
                if (tabela.ValorParametroHoraExtraOcorrencia.ComponenteFreteVeiculo != null)
                    ocorrencias.Add(CalcularOcorrenciaPorFaixaDeHorario(horaInicial, horaFinal, configuracoesVeiculos, tituloVeiculos, fracaoCalculo, tabela.ValorParametroHoraExtraOcorrencia.ComponenteFreteVeiculo.Codigo));

                if (tabela.ValorParametroHoraExtraOcorrencia.ComponenteFreteAjudante != null && quantidadeAjudantes > 0)
                    ocorrencias.Add(CalcularOcorrenciaPorFaixaDeHorario(horaInicial, horaFinal, configuracoesAjudantes, tituloAjudantes, fracaoCalculo, tabela.ValorParametroHoraExtraOcorrencia.ComponenteFreteAjudante.Codigo, quantidadeAjudantes));
            }
            else
            {
                List<Dominio.ObjetosDeValor.Embarcador.Ocorrencia.OcorrenciaPorParametro> ocorrenciasCalculadasEmDoisTempos = new List<Dominio.ObjetosDeValor.Embarcador.Ocorrencia.OcorrenciaPorParametro>();

                if (tabela.ValorParametroHoraExtraOcorrencia.ComponenteFreteVeiculo != null)
                {
                    ocorrenciasCalculadasEmDoisTempos = new List<Dominio.ObjetosDeValor.Embarcador.Ocorrencia.OcorrenciaPorParametro>
                    {
                        // Calcula primeiro pedaço 
                        CalcularOcorrenciaPorFaixaDeHorario(horaInicial, new TimeSpan(24, 0, 0), configuracoesVeiculos, tituloVeiculos, fracaoCalculo, tabela.ValorParametroHoraExtraOcorrencia.ComponenteFreteVeiculo.Codigo),

                        // Calcula segundo pedaço
                        CalcularOcorrenciaPorFaixaDeHorario(new TimeSpan(0, 0, 0), horaFinal, configuracoesVeiculos, tituloVeiculos, fracaoCalculo, tabela.ValorParametroHoraExtraOcorrencia.ComponenteFreteVeiculo.Codigo)
                    };
                    instance.SetaCalculoEmDoisTemposSumarizado(ref ocorrencias, ocorrenciasCalculadasEmDoisTempos);
                }


                if (tabela.ValorParametroHoraExtraOcorrencia.ComponenteFreteAjudante != null && quantidadeAjudantes > 0)
                {
                    ocorrenciasCalculadasEmDoisTempos = new List<Dominio.ObjetosDeValor.Embarcador.Ocorrencia.OcorrenciaPorParametro>
                    {
                        // Calcula primeiro pedaço 
                        CalcularOcorrenciaPorFaixaDeHorario(horaInicial, new TimeSpan(24, 0, 0), configuracoesAjudantes, tituloAjudantes, fracaoCalculo, tabela.ValorParametroHoraExtraOcorrencia.ComponenteFreteAjudante.Codigo, quantidadeAjudantes),

                        // Calcula segundo pedaço
                        CalcularOcorrenciaPorFaixaDeHorario(new TimeSpan(0, 0, 0), horaFinal, configuracoesAjudantes, tituloAjudantes, fracaoCalculo, tabela.ValorParametroHoraExtraOcorrencia.ComponenteFreteAjudante.Codigo, quantidadeAjudantes)
                    };
                    instance.SetaCalculoEmDoisTemposSumarizado(ref ocorrencias, ocorrenciasCalculadasEmDoisTempos);
                }

            }

            // Seta o tipo parametro
            foreach (var ocorrencia in ocorrencias)
                ocorrencia.TipoParametro = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoParametroCalculoOcorrencia.HorasExtra;

            return ocorrencias;
        }

        public static Dominio.ObjetosDeValor.Embarcador.Ocorrencia.OcorrenciaPorParametro CalcularOcorrenciaPorFaixaDeHorario(TimeSpan horaInicial, TimeSpan horaFinal, List<Dominio.ObjetosDeValor.Embarcador.Ocorrencia.ConfiguracaoFaixaHorario> configuracoes, string titulo, int fracaoCalculo, int componenteFrete, int multiplicador = 1)
        {
            ValorParametroOcorrencia instance = GetInstance();

            // Acompanha conforme o tempo passa na linha de tempo de calculo
            TimeSpan horaCalculo = horaInicial;

            // Range de tempo para calculo
            decimal minutosParaCalcular = (decimal)horaFinal.Subtract(horaInicial).TotalMinutes;

            // Sumarizador do valores
            decimal totalCalculado = 0;

            // Apenas informativo
            int minutosCalculados = 0;

            // Inibe que uma faixa de calculo fique presa infinitamente 
            bool possuiConfiguracao = false;

            // Se passar de 1000, ai deu merdica
            int antiPau = 0;

            // Finaliza quando não há mais horas minimas para calculo
            for (int i = 0, s = configuracoes.Count(); i < s && minutosParaCalcular > fracaoCalculo && horaCalculo < horaFinal; i++)
            {
                if (++antiPau > 1000)
                    throw new Exception("O sistema não conseguiu calcular o período informado");

                Dominio.ObjetosDeValor.Embarcador.Ocorrencia.ConfiguracaoFaixaHorario config = configuracoes[i];

                // Caso a configuração da faixa seja x9, usa-se nos calculos o valor redondo 
                TimeSpan configHoraInicial = instance.ArredondaTimeSpan(config.HoraInicial);
                TimeSpan configHoraFinal = instance.ArredondaTimeSpan(config.HoraFinal);

                // Zera a verificação sempre que reinica o for nas configs
                if (i == 0)
                    possuiConfiguracao = false;

                // Situação em que a faixa inicia em um dia e termina em outro
                bool horaInvertida = configHoraInicial > configHoraFinal;

                // Busca uma config compativel com o inicio da hora
                if (instance.ValidaTempoDentroDaFaixa(configHoraInicial, configHoraFinal, horaCalculo))
                {
                    possuiConfiguracao = true;
                    decimal minutosNaFaixa = 0;

                    TimeSpan horaFinalCalculo = configHoraFinal;
                    TimeSpan horaInicioCalculo = horaCalculo;


                    // Hora final do calculo inicia com a hora final da faixa
                    // Mas é setado a hora final passado por parametro quando o fim do calculo acontece antes do fim da faixa
                    // Ex:
                    // horaFinalCalculo = 17:00 -> final da faixa
                    // horaFinal = 13:00 -> calculo termina antes do fim da faixa
                    //
                    // Ou então, quando a hora for invertida (mais de um dia de duração)
                    // E quando o calculo acontece entre inicio da faixa e a meia noite (horaCalculo >= configHoraInicial)
                    if ((horaFinal < horaFinalCalculo) || (horaInvertida && (horaCalculo >= configHoraInicial && horaCalculo < horaFinal)))
                        horaFinalCalculo = horaFinal;

                    minutosNaFaixa = (decimal)(horaFinalCalculo.Subtract(horaInicioCalculo)).TotalMinutes;

                    // Minutos Arredondados
                    minutosNaFaixa = Math.Floor(minutosNaFaixa / fracaoCalculo) * fracaoCalculo;

                    // Remove do tempo total p calculo o tempo bruto
                    minutosParaCalcular -= minutosNaFaixa;

                    // Remove qtd nao fracionada
                    minutosCalculados += (int)minutosNaFaixa;

                    // Calcula o tempo com a fracao configurada
                    decimal horasNaFaixa = minutosNaFaixa / 60;
                    totalCalculado += config.Valor * horasNaFaixa;

                    // Seta a hora calculo como fim da faixa calculada
                    horaCalculo = configHoraFinal;
                    //horaCalculo = (horaCalculo >= configHoraInicial && horaCalculo < horaFinal) ? configHoraFinal : configHoraFinal;
                }

                // Zera contagem
                if ((i + 1) == s)
                {
                    i = -1;

                    // Sempre que não há configuracao compativel
                    // Avança uma fracao, isso é feito para não ficar preso no looping
                    // E também para todo tempo ter chance de ser calculado
                    if (!possuiConfiguracao)
                        horaCalculo.Add(new TimeSpan(0, fracaoCalculo, 0));
                }
            }

            return new Dominio.ObjetosDeValor.Embarcador.Ocorrencia.OcorrenciaPorParametro()
            {
                Titulo = titulo,
                Minutos = minutosCalculados,
                Valor = totalCalculado * multiplicador,
                ComponenteFrete = componenteFrete
            };
        }

        private void SetaCalculoEmDoisTemposSumarizado(ref List<Dominio.ObjetosDeValor.Embarcador.Ocorrencia.OcorrenciaPorParametro> ocorrencias, List<Dominio.ObjetosDeValor.Embarcador.Ocorrencia.OcorrenciaPorParametro> ocorrenciasCalculadasEmDoisTempos)
        {
            var sumarizado = this.SumarizaCalculoEmDoisTempos(ocorrenciasCalculadasEmDoisTempos);

            if (sumarizado != null)
                ocorrencias.Add(sumarizado);
        }

        private Dominio.ObjetosDeValor.Embarcador.Ocorrencia.OcorrenciaPorParametro SumarizaCalculoEmDoisTempos(List<Dominio.ObjetosDeValor.Embarcador.Ocorrencia.OcorrenciaPorParametro> ocorrenciasCalculadasEmDoisTempos)
        {
            if (ocorrenciasCalculadasEmDoisTempos.Count > 0)
            {
                return new Dominio.ObjetosDeValor.Embarcador.Ocorrencia.OcorrenciaPorParametro()
                {
                    Titulo = ocorrenciasCalculadasEmDoisTempos[0].Titulo,
                    Minutos = (from o in ocorrenciasCalculadasEmDoisTempos select o.Minutos).Sum(),
                    Valor = (from o in ocorrenciasCalculadasEmDoisTempos select o.Valor).Sum(),
                    ComponenteFrete = (from o in ocorrenciasCalculadasEmDoisTempos select o.ComponenteFrete).FirstOrDefault(),
                };
            }

            return null;
        }

        private TimeSpan ArredondaTimeSpan(TimeSpan timeSpan)
        {
            if ((timeSpan.Minutes + 1) % 10 == 0)
                timeSpan = timeSpan.Add(new TimeSpan(0, 1, 0));
            else if ((timeSpan.Minutes - 1) % 10 == 0)
                timeSpan = timeSpan.Add(new TimeSpan(0, -1, 0));

            return timeSpan;
        }

        private bool ValidaTempoDentroDaFaixa(TimeSpan horaInicio, TimeSpan horaFinal, TimeSpan horaTeste)
        {
            bool horaInvertida = horaInicio > horaFinal;

            if ((horaInicio <= horaTeste && horaFinal >= horaTeste) && !horaInvertida)
                return true;
            if ((horaTeste >= horaInicio || horaTeste <= horaFinal) && horaInvertida)
                return true;
            return false;
        }
    }
}
