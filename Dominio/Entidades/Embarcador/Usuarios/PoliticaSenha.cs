namespace Dominio.Entidades.Embarcador.Usuarios
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Security.Cryptography;

    public static class PoliticaSenhaExt
    {
        public static string CriarNovaSenha(
            this PoliticaSenha politicaSenha,
            int length = 16,
            bool requireUpper = true,
            bool requireLower = true,
            bool requireDigit = true,
            bool requireSpecial = true,
            bool avoidAmbiguous = true)
        {
            if (length < 8)
                throw new ArgumentException("Use length >= 8 para uma senha forte", nameof(length));

            const string upper = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            const string lower = "abcdefghijklmnopqrstuvwxyz";
            const string digits = "0123456789";
            const string special = "!@#$%^&*()-_=+[]{};:,.<>?";
            const string ambiguous = "0O1lI|`'\".,:;";

            var pools = new List<string>();
            if (requireUpper) pools.Add(avoidAmbiguous ? FilterAmbiguous(upper, ambiguous) : upper);
            if (requireLower) pools.Add(avoidAmbiguous ? FilterAmbiguous(lower, ambiguous) : lower);
            if (requireDigit) pools.Add(avoidAmbiguous ? FilterAmbiguous(digits, ambiguous) : digits);
            if (requireSpecial) pools.Add(avoidAmbiguous ? FilterAmbiguous(special, ambiguous) : special);

            if (!pools.Any())
            {
                var all = upper + lower + digits + special;
                pools.Add(avoidAmbiguous ? FilterAmbiguous(all, ambiguous) : all);
            }

            var allAllowed = string.Concat(pools).Distinct().ToArray();
            var senhaChars = new List<char>();

            foreach (var pool in pools)
                senhaChars.Add(GetRandomCharFrom(pool));

            int restantes = length - senhaChars.Count;
            for (int i = 0; i < restantes; i++)
                senhaChars.Add(GetRandomCharFrom(allAllowed));

            ShuffleInPlace(senhaChars);

            return new string(senhaChars.ToArray());
        }

        private static string FilterAmbiguous(string input, string ambiguous)
        {
            var amb = new HashSet<char>(ambiguous);
            return new string(input.Where(c => !amb.Contains(c)).ToArray());
        }

        private static char GetRandomCharFrom(string pool)
        {
            return pool[GetRandomInt(0, pool.Length)];
        }

        private static char GetRandomCharFrom(char[] pool)
        {
            return pool[GetRandomInt(0, pool.Length)];
        }

        // Fisher-Yates com RNG cripto
        private static void ShuffleInPlace<T>(IList<T> list)
        {
            for (int i = list.Count - 1; i > 0; i--)
            {
                int j = GetRandomInt(0, i + 1);
                (list[i], list[j]) = (list[j], list[i]);
            }
        }

        // 🔐 Compatível com todas as versões do .NET
        private static int GetRandomInt(int minValue, int maxValue)
        {
            if (minValue >= maxValue)
                throw new ArgumentOutOfRangeException(nameof(maxValue), "maxValue deve ser maior que minValue");

            // Range seguro, exclusivo do maxValue
            long range = (long)maxValue - minValue;
            long limit = (long.MaxValue / range) * range;

            byte[] bytes = new byte[8];
            long value;
            using (var rng = RandomNumberGenerator.Create())
            {
                do
                {
                    rng.GetBytes(bytes);
                    value = Math.Abs(BitConverter.ToInt64(bytes, 0));
                }
                while (value >= limit);
            }

            return (int)(minValue + (value % range));
        }
    }


    [NHibernate.Mapping.Attributes.Class(0, Table = "T_POLITICA_SENHA", EntityName = "PoliticaSenha", Name = "Dominio.Entidades.Embarcador.Usuarios.PoliticaSenha", NameType = typeof(PoliticaSenha))]
    public class PoliticaSenha : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "PLS_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "PLS_HABILITAR_POLITICA_SENHA", TypeType = typeof(bool), NotNull = true)]
        public virtual bool HabilitarPoliticaSenha { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "PLS_EXIGIR_TROCA_SENHA_PRIMEIRO_ACESSO", TypeType = typeof(bool), NotNull = true)]
        public virtual bool ExigirTrocaSenhaPrimeiroAcesso { get; set; }

        // Campo removido do mapeamento NHibernate - não será mais persistido no banco
        [Obsolete("Campo 'Senha padrão primeiro acesso' foi ocultado da interface e não deve mais ser utilizado.", true)]
        public virtual string SenhaPadraoPrimeiroAcesso { get; set; }

        [Obsolete("Campo foi removido pois causa previsibilidade de senha.", true)]
        [NHibernate.Mapping.Attributes.Property(0, Column = "PLS_USAR_CPF_USUARIO_SENHA_PADRAO_PRIMEIRO_ACESSO", TypeType = typeof(bool), NotNull = true)]
        public virtual bool UsarCPFUsuarioComoSenhaPadraoPrimeiroAcesso { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "PLS_NUMERO_MINIMO_CARACTERES_SENHA", TypeType = typeof(int), NotNull = true)]
        public virtual int NumeroMinimoCaracteresSenha { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "PLS_BLOQUEAR_USUARIO_APOS_QUANTIDADE_TENTATIVAS", TypeType = typeof(int), NotNull = true)]
        public virtual int BloquearUsuarioAposQuantidadeTentativas { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "PLS_TEMPO_EM_MINUTOS_BLOQUEIO_USUARIO", TypeType = typeof(int), NotNull = false)]
        public virtual int TempoEmMinutosBloqueioUsuario { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "PLS_QUANTAS_SENHAS_ANTERIORES_NAO_REPETIR", TypeType = typeof(int), NotNull = false)]
        public virtual int QuantasSenhasAnterioresNaoRepetir { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "PLS_NAO_PERMITIR_ACESSOS_SIMULTANEOS", TypeType = typeof(bool), NotNull = false)]
        public virtual bool NaoPermitirAcessosSimultaneos { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "PLS_PRAZO_EXPIRA_SENHA", TypeType = typeof(int), NotNull = true)]
        public virtual int PrazoExpiraSenha { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "PLS_EXIGIR_SENHA_FORTE", TypeType = typeof(bool), NotNull = true)]
        public virtual bool ExigirSenhaForte { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "PLS_HABILITAR_CRIPTOGRAFIA", TypeType = typeof(bool), NotNull = true)]
        public virtual bool HabilitarCriptografia { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Usuario", Column = "FUN_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Usuario Usuario { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataUltimaModificacao", Column = "PLS_DATA_ULTIMA_MODIFICACAO", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime DataUltimaModificacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TipoServico", Column = "PLS_TIPO_SERVICO", TypeType = typeof(AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware), NotNull = false)]
        public virtual AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware? TipoServico { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "PLS_INATIVAR_USUARIO_APOS_DIAS_SEM_ACESSAR_SISTEMA", TypeType = typeof(int), NotNull = false)]
        public virtual int InativarUsuarioAposDiasSemAcessarSistema { get; set; }


        public virtual string Descricao
        {
            get
            {
                return "Política";
            }
        }

    }
}
