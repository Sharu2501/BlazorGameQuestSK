namespace SharedModels
{
    public class Artifact
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public RarityEnum Rarity { get; set; }
        public Artifact()
        {
            Id = 0;
            Name = "default";
            Description = "default";
            Rarity = RarityEnum.COMMON;
        }
        public Artifact(int id, string name, string description, RarityEnum rarity)
        {
            Id = id;
            Name = name;
            Description = description;
            Rarity = rarity;
        }
    }
}