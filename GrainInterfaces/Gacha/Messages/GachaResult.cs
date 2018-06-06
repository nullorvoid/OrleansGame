namespace GrainInterfaces.Gacha.Messages
{
    public class GachaResult
    {
		public string Id { get; }

		public GachaResult(string id)
		{
			Id = id;
		}
    }
}
