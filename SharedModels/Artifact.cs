namespace SharedModels
{
    public class Artifact
    {
        public int Id { get; set; } = 0;
        public string Name { get; set; } = "default";
        public string Description { get; set; } = "default";
        public RarityEnum Rarity { get; set; } = RarityEnum.COMMON;
    }
}