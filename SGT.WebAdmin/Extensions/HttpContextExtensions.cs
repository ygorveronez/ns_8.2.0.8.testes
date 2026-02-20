namespace SGT.WebAdmin.Controllers
{
    public static class HttpContextExtensions
    {
        public static List<Servicos.DTO.CustomFile> GetFiles(this HttpContext context)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            List<Servicos.DTO.CustomFile> customFiles = context.Items["CustomFiles"] as List<Servicos.DTO.CustomFile>;

            if (customFiles == null)
                customFiles = new List<Servicos.DTO.CustomFile>();

            return customFiles;
        }

        public static List<Servicos.DTO.CustomFile> GetFiles(this HttpContext context, string key)
        {
            List<Servicos.DTO.CustomFile> files = context.GetFiles();

            return files?.FindAll(file => string.Equals(key, file.Key, StringComparison.OrdinalIgnoreCase)) ?? new List<Servicos.DTO.CustomFile>();
        }

        public static Servicos.DTO.CustomFile GetFile(this HttpContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            List<Servicos.DTO.CustomFile> customFiles = context.Items["CustomFiles"] as List<Servicos.DTO.CustomFile>;

            return customFiles?.FirstOrDefault();
        }

        public static Servicos.DTO.CustomFile GetFile(this HttpContext context, string key)
        {
            List<Servicos.DTO.CustomFile> files = context.GetFiles();

            return files?.Find(file => string.Equals(key, file.Key, StringComparison.OrdinalIgnoreCase));
        }
    }
}