namespace Servicos.Extensions
{
	public static class ObjectExtensions
	{
		public static T FromJson<T>(this string json)
		{
			return Newtonsoft.Json.JsonConvert.DeserializeObject<T>(json);
		}
	}
}
